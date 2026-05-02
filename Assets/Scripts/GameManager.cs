using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public BurgerTile[, ] gameBoard = new BurgerTile[4, 5];
    
    // 주문 햄버거 재료 리스트
    public List<Ingredient> orderList = new List<Ingredient>();


    // --- 시각화 관련 변수  ---
    [Header("Visual Settings")]
    public GameObject ingredientPrefab; // 화면에 생성될 기본 2D 오브젝트
    
   public Color colorRawPatty = Color.gray;
    public Color colorCookedPatty = new Color(0.4f, 0.2f, 0f); // 짙은 갈색
    public Color colorCheese = Color.yellow;
    public Color colorOnion = Color.white;
    public Color colorLettuce = Color.green;
    public Color colorTomato = Color.red;


    // 화면에 생성된 2D 오브젝트들을 추적하고 관리하기 위한 리스트
    private List<GameObject> activeVisuals = new List<GameObject>();
    
    //시작하면
    void Start()
    {
        InitializeBoard();

        DrawBackgroundGrid();
        // 👇 테스트용 주문 넣기 (예: 치즈 -> 구워진 패티 -> 양상추 순서)
        
        orderList.Add(Ingredient.Cheese);
        orderList.Add(Ingredient.CookedPatty); 
        orderList.Add(Ingredient.Lettuce);
        orderList.Add(Ingredient.Onion);
        // 시작할 때 1개 올려두기 
        SpawnNextIngredient();
        UpdateVisuals();
        Debug.Log("시작");
        
    }

    // 키보드   
    void Update()
    {
        // 키보드 연결 상태 확인 (에러 방지)
        if (UnityEngine.InputSystem.Keyboard.current == null) return;

        // 최신 Input System을 사용한 키 입력 감지
        if (UnityEngine.InputSystem.Keyboard.current.rightArrowKey.wasPressedThisFrame) MoveTiles("Right");
        if (UnityEngine.InputSystem.Keyboard.current.leftArrowKey.wasPressedThisFrame) MoveTiles("Left");
        if (UnityEngine.InputSystem.Keyboard.current.upArrowKey.wasPressedThisFrame) MoveTiles("Up");
        if (UnityEngine.InputSystem.Keyboard.current.downArrowKey.wasPressedThisFrame) MoveTiles("Down");

        if (UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame) SubmitAndClear();
    }

    
    void InitializeBoard()
    {
        for (int y = 0; y < 5; y++) 
        {
            for (int x = 0; x < 4; x++) 
            {
                gameBoard[x, y] = new BurgerTile(); 
                if (y == 4) 
                {
                    gameBoard[x, y].isGrill = true; 
                }
            }
        }
    }

    public void SpawnNextIngredient()
    {
        // 주문 리스트 없으면
        if (orderList.Count==0)
        {
            Debug.Log("더 이상 스폰할 재료가 없습니다.");
            return;
        }

        // 빈 칸 찾기
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
        // 빈칸이 있다면
        if (emptyTiles.Count > 0)
        {
            int randomIndex = Random.Range(0, emptyTiles.Count);
            Ingredient nextMaterial = orderList [0];
            if (nextMaterial == Ingredient.CookedPatty)
            {
                nextMaterial = Ingredient.RawPatty;
            }
            // 랜덤 재료 생성
            emptyTiles[randomIndex].AddIngredient(nextMaterial);
            orderList.RemoveAt(0);
        }
    }
    
    void SubmitAndClear()
    {
        int maxCount = 0;
        BurgerTile targetTile = null; // 최종 제출될 버거 타일 (우승자)
        int targetX = -1; // 우승자의 X 위치
        int targetY = -1; // 우승자의 Y 위치

        // 1. 가장 높이 쌓인 햄버거 찾기 (동점자 처리 포함)
        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                int currentCount = gameBoard[x, y].stackedIngredients.Count;
            
                // 빈 접시면 넘어가기
                if (currentCount == 0) continue; 

                // 더 높은 숫자 나오면 교체
                if (currentCount > maxCount)
                {
                    maxCount = currentCount;
                    targetTile = gameBoard[x, y];
                    targetX = x;
                    targetY = y;
                }
                // 같은 숫자가 나오면!!
                else if (currentCount == maxCount)
                {
                    // 오른쪽 위 모서리 (x: 3, y: 0) 까지의 거리 계산
                    // 피타고라스의 정리 (a^2 + b^2 = c^2) 활용
                    float currentDist = (3 - x) * (3 - x) + (0 - y) * (0 - y);
                    float targetDist = (3 - targetX) * (3 - targetX) + (0 - targetY) * (0 - targetY);

                    // 거리가 짧으면 교체
                    if (currentDist < targetDist)
                    {
                        targetTile = gameBoard[x, y];
                        targetX = x;
                        targetY = y;
                    }
                    // 거리가 완전히 똑같다면 -> 더 위쪽을 우선시함
                    else if (currentDist == targetDist)
                    {
                        if (y < targetY) 
                        {
                            targetTile = gameBoard[x, y];
                            targetX = x;
                            targetY = y;
                        }
                    }
                }
            }
        }

        // 2. 제출된 블록 정보 읽고 로그에 띄우기
        if (targetTile != null)
        {
            // 버거 안에 있는 재료들을 글자로
            string submittedList = "";
            for (int i = 0; i < targetTile.stackedIngredients.Count; i++)
            {
                // 재료 이름을 글자에 하나씩 더하기
                submittedList += targetTile.stackedIngredients[i].ToString() + " "; 
            }

            Debug.Log($"제출 완료.");
            Debug.Log($"선택된 위치: ({targetX}, {targetY}) / 갯수: {maxCount}개");
            Debug.Log($"제출된 버거 재료: [ {submittedList}]");
        }
        else
        {
            Debug.Log("제출할 햄버거가 없습니다!");
        

        // 3. 모든 블록 사라지게 하기
        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                gameBoard[x, y].stackedIngredients.Clear(); 
            }
        }
    
        // 제출하고 나서 다음 주문을 바로 스폰할 거면 켜두고, 아니면 지워도 됩니다.
        //SpawnNextIngredient();
        }
        UpdateVisuals();

    }

    void MoveTiles(string direction)
    {
        Debug.Log($"[{direction}] 방향으로 슬라이드");

        // 왼쪽이면
        if (direction == "Left")
        {
            // 1. 가로줄(y)을 아래에서부터 한 줄씩 확인 (총 5줄)
            for (int y = 0; y < 5; y++)
            {
                // [작업 1] 이 줄에 있는 햄버거들만 순서대로 꺼내서 임시로 담기
                List<List<Ingredient>> extractedStacks = new List<List<Ingredient>>();
                for (int x = 0; x < 4; x++)
                {
                    if (gameBoard[x, y].stackedIngredients.Count > 0)
                    {
                        // 햄버거 묶음을 통째로 복사해서 담아두고, 보드판 자리는 일단 비움.
                        extractedStacks.Add(new List<Ingredient>(gameBoard[x, y].stackedIngredients));
                        gameBoard[x, y].stackedIngredients.Clear();
                    }
                }

                // [작업 2] 꺼낸 햄버거들을 하나씩 비교하면서 합치기
                List<List<Ingredient>> mergedStacks = new List<List<Ingredient>>();
                for (int i = 0; i < extractedStacks.Count; i++)
                {
                    // 일단 새로운 결과 바구니에 하나 넣기.
                    mergedStacks.Add(extractedStacks[i]);

                    // 방금 넣은 것이 그 앞의 것과 높이(개수)가 같다면 무한히 합침.
                    while (mergedStacks.Count >= 2)
                    {
                        int lastIdx = mergedStacks.Count - 1;
                        List<Ingredient> rightStack = mergedStacks[lastIdx];     // 방금 들어온 오른쪽 햄버거
                        List<Ingredient> leftStack = mergedStacks[lastIdx - 1]; // 원래 있던 왼쪽 햄버거

                        // 개수가 똑같으면 합치기
                        if (leftStack.Count == rightStack.Count)
                        {
                            // 왼쪽으로 스와이프 시 -> 오른쪽 재료가 위로 감
                            leftStack.AddRange(rightStack);
                        
                            // 왼쪽으로 흡수 후 오른쪽꺼 삭제
                            mergedStacks.RemoveAt(lastIdx);
                        }
                        else
                        {
                            // 개수가 다르면 안 합쳐지므로 종료
                            break; 
                        }
                    }
                }

                // [작업 3] 다 합쳐진 햄버거들을 다시 보드판 맨 왼쪽(x=0)부터 차례로 배치
                for (int i = 0; i < mergedStacks.Count; i++)
                {
                    gameBoard[i, y].stackedIngredients = mergedStacks[i];
                }
            }
        }

        // 오른쪽이면
        else if (direction == "Right")
    {
        for (int y = 0; y < 5; y++)
        {
            List<List<Ingredient>> extractedStacks = new List<List<Ingredient>>();
            for (int x = 3; x >= 0; x--) // 오른쪽부터 왼쪽으로 훑기 (x=3 부터 시작!)
            {
                if (gameBoard[x, y].stackedIngredients.Count > 0)
                {
                    extractedStacks.Add(new List<Ingredient>(gameBoard[x, y].stackedIngredients));
                    gameBoard[x, y].stackedIngredients.Clear();
                }
            }

            List<List<Ingredient>> mergedStacks = new List<List<Ingredient>>();
            for (int i = 0; i < extractedStacks.Count; i++)
            {
                mergedStacks.Add(extractedStacks[i]);
                while (mergedStacks.Count >= 2)
                {
                    int lastIdx = mergedStacks.Count - 1;
                    List<Ingredient> rightStack = mergedStacks[lastIdx - 1]; 
                    List<Ingredient> leftStack = mergedStacks[lastIdx];

                    if (rightStack.Count == leftStack.Count)
                    {
                        // 오른쪽 스와이프: 왼쪽 재료가 위로 감
                        rightStack.AddRange(leftStack);
                        mergedStacks.RemoveAt(lastIdx);
                    }
                    else break;
                }
            }

            for (int i = 0; i < mergedStacks.Count; i++)
            {
                gameBoard[3 - i, y].stackedIngredients = mergedStacks[i]; // 맨 오른쪽(3)부터 배치
            }
        }
    }
    else if (direction == "Up")
    {
        for (int x = 0; x < 4; x++) // 이번엔 가로(x) 단위로 쪼갭니다.
        {
            List<List<Ingredient>> extractedStacks = new List<List<Ingredient>>();
            for (int y = 0; y < 5; y++) // 위쪽부터 아래쪽으로 훑기 (y=0 이 맨 위!)
            {
                if (gameBoard[x, y].stackedIngredients.Count > 0)
                {
                    extractedStacks.Add(new List<Ingredient>(gameBoard[x, y].stackedIngredients));
                    gameBoard[x, y].stackedIngredients.Clear();
                }
            }

            List<List<Ingredient>> mergedStacks = new List<List<Ingredient>>();
            for (int i = 0; i < extractedStacks.Count; i++)
            {
                mergedStacks.Add(extractedStacks[i]);
                while (mergedStacks.Count >= 2)
                {
                    int lastIdx = mergedStacks.Count - 1;
                    List<Ingredient> topStack = mergedStacks[lastIdx - 1]; 
                    List<Ingredient> bottomStack = mergedStacks[lastIdx];

                    if (topStack.Count == bottomStack.Count)
                    {
                        // 위쪽 스와이프: 무조건 위에 있던 재료가 위로 감
                        // 아래쪽 재료를 먼저 깔고(base), 그 위에 위쪽 재료를 얹어야 함!
                        bottomStack.AddRange(topStack);
                        mergedStacks[lastIdx - 1] = bottomStack; // 자리를 교체해줌
                        mergedStacks.RemoveAt(lastIdx);
                    }
                    else break;
                }
            }

            for (int i = 0; i < mergedStacks.Count; i++)
            {
                gameBoard[x, i].stackedIngredients = mergedStacks[i]; // 맨 위쪽(0)부터 배치
            }
        }
    }
    else if (direction == "Down")
    {
        for (int x = 0; x < 4; x++) 
        {
            List<List<Ingredient>> extractedStacks = new List<List<Ingredient>>();
            for (int y = 4; y >= 0; y--) // 아래쪽부터 위쪽으로 훑기 (y=4 부터 시작!)
            {
                if (gameBoard[x, y].stackedIngredients.Count > 0)
                {
                    extractedStacks.Add(new List<Ingredient>(gameBoard[x, y].stackedIngredients));
                    gameBoard[x, y].stackedIngredients.Clear();
                }
            }

            List<List<Ingredient>> mergedStacks = new List<List<Ingredient>>();
            for (int i = 0; i < extractedStacks.Count; i++)
            {
                mergedStacks.Add(extractedStacks[i]);
                while (mergedStacks.Count >= 2)
                {
                    int lastIdx = mergedStacks.Count - 1;
                    List<Ingredient> bottomStack = mergedStacks[lastIdx - 1]; 
                    List<Ingredient> topStack = mergedStacks[lastIdx];

                    if (bottomStack.Count == topStack.Count)
                    {
                        // 아래쪽 스와이프: 역시 무조건 위에 있던 재료가 위로 감
                        bottomStack.AddRange(topStack);
                        mergedStacks.RemoveAt(lastIdx);
                    }
                    else break;
                }
            }

            for (int i = 0; i < mergedStacks.Count; i++)
            {
                gameBoard[x, 4 - i].stackedIngredients = mergedStacks[i]; // 맨 아래쪽(4)부터 배치
            }
        }
    }

    // 이동이 다 끝난 후, 맨 아랫줄(그릴, y=4) 확인
    for (int x = 0; x < 4; x++)
    {
        BurgerTile bottomTile = gameBoard[x, 4];
        if (bottomTile.stackedIngredients.Count > 0)
        {
            // 스택의 맨 밑바닥([0])에 있는 재료가 냉동 패티라면
            if (bottomTile.stackedIngredients[0] == Ingredient.RawPatty)
            {
                // 구워진 패티로 교체!
                bottomTile.stackedIngredients[0] = Ingredient.CookedPatty;
                Debug.Log($"({x}, 4) 위치의 패티가 그릴에서 구워졌습니다!");
            }
        }
    }

    // 슬라이드가 끝났으니 새로운 주문 재료 하나 스폰
    SpawnNextIngredient(); 
    UpdateVisuals();
}



   // 보드판의 데이터 상태를 화면에 반영하는 함수
    public void UpdateVisuals()
    {
        foreach (GameObject obj in activeVisuals)
        {
            Destroy(obj);
        }
        activeVisuals.Clear();

        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                List<Ingredient> stack = gameBoard[x, y].stackedIngredients;
                
                for (int i = 0; i < stack.Count; i++)
                {
                    Ingredient currentIngredient = stack[i];
                    GameObject newObj = Instantiate(ingredientPrefab);
                    activeVisuals.Add(newObj);

                    float posX = x * 1.5f; 
                    float posY = y * -1.5f; 
                    posY += (i * 0.2f); 

                    newObj.transform.position = new Vector3(posX, posY, 0);

                    SpriteRenderer sr = newObj.GetComponent<SpriteRenderer>();
                    
                    // 이미지 교체 대신, 프리팹의 색상을 변경합니다.
                    sr.color = GetIngredientColor(currentIngredient);
                    
                    sr.sortingOrder = i; 
                }
            }
        }
    }

    
    // 재료에 맞는 색상을 반환합니다.
    private Color GetIngredientColor(Ingredient ingredientType)
    {
        switch (ingredientType)
        {
            case Ingredient.RawPatty: return colorRawPatty;
            case Ingredient.CookedPatty: return colorCookedPatty;
            case Ingredient.Cheese: return colorCheese;
            case Ingredient.Onion: return colorOnion;
            case Ingredient.Lettuce: return colorLettuce;
            case Ingredient.Tomato: return colorTomato;
            default: return Color.white;
        }
    }
    // 배경 4x5 프레임을 그려주는 함수
    void DrawBackgroundGrid()
    {
        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                GameObject bgTile = Instantiate(ingredientPrefab);
                
                float posX = x * 1.5f; 
                float posY = y * -1.5f; 
                bgTile.transform.position = new Vector3(posX, posY, 0);

                SpriteRenderer sr = bgTile.GetComponent<SpriteRenderer>();
                
                // 맨 아랫줄(y=4)은 그릴이므로 아주 어두운 회색, 나머지는 밝은 회색으로 설정
                if (y == 4) 
                {
                    sr.color = new Color(0.2f, 0.2f, 0.2f); 
                }
                else 
                {
                    sr.color = new Color(0.8f, 0.8f, 0.8f); 
                }

                // 배경이므로 재료들보다 항상 뒤에(-1) 그려지도록 설정
                sr.sortingOrder = -1; 
            }
        }
    }

}    




