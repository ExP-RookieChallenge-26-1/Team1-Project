using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Inst;
    
    private VisualManager visualManager;
    private BurgerTile[,] gameBoard = new BurgerTile[4, 5];
    
    // 현재 스폰 대기 중인 남은 주문 리스트
    public List<IngredientType> orderList = new List<IngredientType>();
    
    // 처음 게임 시작 시 들어온 전체 주문 리스트 (제출 시 비교용)
    private List<IngredientType> initialOrderList = new List<IngredientType>();

    public bool isPlaying = false;
    
    private void Awake()
    {
        Inst = this;
    }
    
    void Start()
    {
        visualManager = GetComponent<VisualManager>();
        visualManager.SetBackgroundColor();
        InitializeBoard();
        visualManager.DrawBackgroundGrid();
    }

    void InitializeBoard()
    {
        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                gameBoard[x, y] = new BurgerTile();
                // 맨 아랫줄(y=4)은 그릴 타일로 설정
                if (y == 4) gameBoard[x, y].isGrill = true;
            }
        }
    }
    
    //뒤집개를 충분히 드래그해서, 조리탭이 올라왔을때 호출됩니다
    public void OnTongEndDrag()
    {
        //여기부터 게임 로직이 시작되면 될 것 같아요
        Debug.Log("OnTongEndDrag");
        StartGame();
    }


    public void StartGame()
    {

        if (StageFlowManager.Inst)
        {
            var currentCustomer = StageFlowManager.Inst.customerQueueManager.GetCurrentCustomer();
            orderList = new();
            foreach (var data in currentCustomer.Recipe)
            {
                orderList.Add(data.IngredientType);
            }
        }
        else
        {
            if (orderList.Count == 0)
            {
                Debug.LogWarning("주문 리스트가 비어있습니다!");
                return;
            }
        }
        
        // 원본 주문 내역을 백업해둠
        initialOrderList = new List<IngredientType>(orderList);
        
        isPlaying = true;
        Debug.Log($"[게임 시작] 처음 주문 목록: {string.Join(", ", initialOrderList)}");
        
        SpawnNextIngredient();
        UpdateAllVisuals();
    }

    public void UpdateOrderVisual()
    {
        visualManager.DrawOrderList(orderList);
    }

    public void OnMoveInput(string direction)
    {
        if (!isPlaying) return;

        bool hasMoved = MoveTiles(direction);
        
        if (hasMoved) 
        {
            Debug.Log($"[{direction}] 방향으로 스와이프 완료");
            SpawnNextIngredient();
        }

        UpdateAllVisuals();
    }

    public void OnSubmitInput()
    {
        if (!isPlaying) return;
        
        BurgerTile bestBurger = GetBestBurger();
        string submittedIngredients = (bestBurger != null && bestBurger.stackedIngredients.Count > 0) 
            ? string.Join(", ", bestBurger.stackedIngredients) 
            : "빈 접시";
        
        string initialOrder = string.Join(", ", initialOrderList);

        // 콘솔에 제출 결과 출력
        Debug.Log("=========================");
        Debug.Log("🔔 버거 완성 및 제출!");
        Debug.Log($"목표 주문: {initialOrder}");
        Debug.Log($"제출된 버거: {submittedIngredients}");
        Debug.Log("=========================");

        SubmitAndClear();
        UpdateAllVisuals();
    }

    public IReadOnlyList<IngredientData> GetBestBurgerData()
    {
        var list = new List<IngredientData>();
        BurgerTile bestBurger = GetBestBurger();
        foreach (var ingredientType in bestBurger.stackedIngredients)
        {
            var data = new IngredientData();
            data.ingredientType = ingredientType;
            list.Add(data);
        }

        list.Reverse();
        return list;
    }

    public void OnResetInput()
    {
        Debug.Log("보드판 및 주문 초기화됨");
        
        orderList.Clear();
        initialOrderList.Clear();
        isPlaying = false;
        
        SubmitAndClear();
        visualManager.DrawOrderList(orderList);
        UpdateAllVisuals();
    }

    private void UpdateAllVisuals()
    {
        visualManager.UpdateVisuals(gameBoard);
        visualManager.DrawPreview(GetBestBurger());
    }

    // 우상단(3, 0)에서 가장 가까운 최고점 블록 탐색 (거리 동일 시 높이(Y축) 우선)
    public BurgerTile GetBestBurger()
    {
        BurgerTile best = null;
        int maxHeight = 0;
        float minDistance = float.MaxValue;
        int minY = int.MaxValue; 
        
        Vector2 targetPos = new Vector2(3, 0);

        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                int height = gameBoard[x, y].stackedIngredients.Count;
                if (height > 0)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), targetPos);

                    if (height > maxHeight)
                    {
                        maxHeight = height;
                        minDistance = dist;
                        minY = y;
                        best = gameBoard[x, y];
                    }
                    else if (height == maxHeight)
                    {
                        if (Mathf.Abs(dist - minDistance) < 0.01f)
                        {
                            if (y < minY)
                            {
                                minDistance = dist;
                                minY = y;
                                best = gameBoard[x, y];
                            }
                        }
                        else if (dist < minDistance)
                        {
                            minDistance = dist;
                            minY = y;
                            best = gameBoard[x, y];
                        }
                    }
                }
            }
        }
        return best;
    }

    public void SpawnNextIngredient()
    {
        if (orderList.Count == 0) return;

        List<BurgerTile> emptyTiles = new List<BurgerTile>();
        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                if (gameBoard[x, y].stackedIngredients.Count == 0)
                {
                    emptyTiles.Add(gameBoard[x, y]);
                }
            }
        }

        if (emptyTiles.Count > 0)
        {
            int randomIndex = Random.Range(0, emptyTiles.Count);
            IngredientType nextMaterial = orderList[0];

            // 생성되는 패티는 항상 생패티 상태로 스폰
            if (nextMaterial == IngredientType.BakedPatty) nextMaterial = IngredientType.FrozenPatty;

            BurgerTile targetTile = emptyTiles[randomIndex];
            targetTile.AddIngredient(nextMaterial);
            
            orderList.RemoveAt(0);
            visualManager.DrawOrderList(orderList); 

            // 스폰된 위치가 그릴이라면 즉시 굽기 처리
            if (targetTile.isGrill && targetTile.stackedIngredients[0] == IngredientType.BakedPatty)
            {
                targetTile.stackedIngredients[0] = IngredientType.BakedPatty;
            }
        }
    }

    bool MoveTiles(string direction)
    {
        bool didAnyMove = false;
        int dx = 0, dy = 0;

        if (direction == "Right") dx = 1;
        if (direction == "Left") dx = -1;
        if (direction == "Up") dy = -1;
        if (direction == "Down") dy = 1;

        int startX = (dx == 1) ? 3 : 0;
        int endX = (dx == 1) ? -1 : 4;
        int stepX = (dx == 1) ? -1 : 1;

        int startY = (dy == 1) ? 4 : 0;
        int endY = (dy == 1) ? -1 : 5;
        int stepY = (dy == 1) ? -1 : 1;

        bool changedInPass = true;
        
        // 연쇄적인 2048 조합 처리를 위한 반복 루프 (더 이상 합쳐지지 않을 때까지)
        while (changedInPass)
        {
            changedInPass = false;

            for (int y = startY; y != endY; y += stepY)
            {
                for (int x = startX; x != endX; x += stepX)
                {
                    if (gameBoard[x, y].stackedIngredients.Count > 0)
                    {
                        int nx = x + dx;
                        int ny = y + dy;

                        if (nx < 0 || nx >= 4 || ny < 0 || ny >= 5) continue;

                        List<IngredientType> currStack = gameBoard[x, y].stackedIngredients;
                        List<IngredientType> targetStack = gameBoard[nx, ny].stackedIngredients;

                        if (targetStack.Count == 0)
                        {
                            // 빈칸으로 이동
                            targetStack.AddRange(currStack);
                            currStack.Clear();
                            changedInPass = true;
                            didAnyMove = true;
                        }
                        else if (currStack.Count == targetStack.Count)
                        {
                            // 블록 높이가 같을 경우 병합 (재료 종류 무관)
                            List<IngredientType> mergedStack = new List<IngredientType>();

                            if (dx != 0) 
                            {
                                // 좌우 이동: 무조건 오른쪽(X가 큰 쪽)이 상단으로 올라감
                                if (x > nx) 
                                {
                                    mergedStack.AddRange(targetStack); 
                                    mergedStack.AddRange(currStack);   
                                }
                                else 
                                {
                                    mergedStack.AddRange(currStack);   
                                    mergedStack.AddRange(targetStack); 
                                }
                            }
                            else 
                            {
                                // 상하 이동: 무조건 더 위쪽(Y가 작은 쪽)이 상단으로 올라감
                                if (y < ny) 
                                {
                                    mergedStack.AddRange(targetStack); 
                                    mergedStack.AddRange(currStack);   
                                }
                                else 
                                {
                                    mergedStack.AddRange(currStack);   
                                    mergedStack.AddRange(targetStack); 
                                }
                            }

                            targetStack.Clear();
                            targetStack.AddRange(mergedStack);
                            currStack.Clear();
                            changedInPass = true;
                            didAnyMove = true;
                        }
                    }
                }
            }
        }

        // 이동 완료 후 맨 아랫줄(그릴) 검사 및 패티 굽기 처리
        for (int x = 0; x < 4; x++)
        {
            if (gameBoard[x, 4].stackedIngredients.Count > 0)
            {
                for (int i = 0; i < gameBoard[x, 4].stackedIngredients.Count; i++)
                {
                    if (gameBoard[x, 4].stackedIngredients[i] == IngredientType.FrozenPatty)
                    {
                        gameBoard[x, 4].stackedIngredients[i] = IngredientType.BakedPatty;
                        didAnyMove = true; 
                    }
                }
            }
        }

        return didAnyMove;
    }

    void SubmitAndClear()
    {
        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                gameBoard[x, y].stackedIngredients.Clear();
            }
        }
    }
}