using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 보드 게임 연결: 재료 버튼 → 생성, 스와이프 → 이동, 버튼/카운터 미리보기 갱신.
/// 손님 흐름(AdvancedMain)이 BeginRound로 시작하고 TakeBestStack으로 제출받는다.
/// 손님 흐름이 없는 씬에서는 바로 플레이 가능 상태로 시작한다 (보드 단독 테스트용).
/// </summary>
public class GameManager : MonoBehaviour
{
<<<<<<< Updated upstream
    public static GameManager Inst;
    public Image nextIngrdient;
    private VisualManager visualManager;
    private BurgerTile[,] gameBoard = new BurgerTile[4, 5];
=======
    public static GameManager Inst { get; private set; }
>>>>>>> Stashed changes

    [SerializeField] int boardSize = 4;
    [SerializeField] BoardView boardView;
    [SerializeField] SwipeInput swipeInput;
    [SerializeField] IngredientButton[] ingredientButtons;
    [Tooltip("씬에 AdvancedMain이 없으면 시작하자마자 플레이 가능 상태로 둔다 (보드 단독 테스트용)")]
    [SerializeField] bool autoStartWithoutFlow = true;

    /// <summary>재료 버튼으로 재료가 보드에 생성됐을 때. 손님 제한시간 시작에 사용.</summary>
    public event Action<Ingredient> IngredientSpawned;

    public BoardSystem Model { get; private set; }
    /// <summary>재료 생성/슬라이드가 가능한 상태인지 (손님 응대 중).</summary>
    public bool IsPlaying { get; private set; }

    void Awake()
    {
        Inst = this;
        Model = new BoardSystem(boardSize);
    }

    void Start()
    {
        boardView.Build(boardSize);
        swipeInput.Swiped += OnSwiped;
        if (autoStartWithoutFlow && AdvancedMain.Inst == null) BeginRound();
        else RefreshState();
    }

    void OnDestroy()
    {
<<<<<<< Updated upstream
        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                gameBoard[x, y] = new BurgerTile();
                // 留??꾨옯以?y=4)? 洹몃┫ ??쇰줈 ?ㅼ젙
                if (y == 0) gameBoard[x, y].isGrill = true;
            }
        }
=======
        if (swipeInput != null) swipeInput.Swiped -= OnSwiped;
        if (Inst == this) Inst = null;
>>>>>>> Stashed changes
    }

    /// <summary>새 손님 응대 시작: 보드를 비우고 입력을 연다.</summary>
    public void BeginRound()
    {
        ClearBoard();
        IsPlaying = true;
        RefreshState();
    }

    /// <summary>입력을 막는다 (제출 연출, 시간 초과 등). 보드는 그대로 둔다.</summary>
    public void EndRound()
    {
<<<<<<< Updated upstream

        if (StageFlowManager.Inst)
        {
            var currentCustomer = StageFlowManager.Inst.CustomerQueueManager.GetCurrentCustomer();
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
                Debug.LogWarning("二쇰Ц 由ъ뒪?멸? 鍮꾩뼱?덉뒿?덈떎!");
                return;
            }
        }

        // ?먮낯 二쇰Ц ?댁뿭??諛깆뾽?대몺
        initialOrderList = new List<IngredientType>(orderList);

        isPlaying = true;
        Debug.Log($"[寃뚯엫 ?쒖옉] 泥섏쓬 二쇰Ц 紐⑸줉: {string.Join(", ", initialOrderList)}");

        SpawnNextIngredient();
        UpdateAllVisuals();
=======
        IsPlaying = false;
        RefreshState();
>>>>>>> Stashed changes
    }

    /// <summary>
    /// 지금 제출하면 나갈 버거 (가장 높은 더미, 동점이면 오른쪽 위에 가까운 것). 보드는 그대로.
    /// 얼린 패티는 버거에 들어가지 않는다 (미리보기/쌓이는 연출/요구사항 판정 모두 제외).
    /// </summary>
    public List<Ingredient> PeekBestStack()
    {
        var best = Model.GetBestStack();
        var burger = best == null ? new List<Ingredient>() : new List<Ingredient>(best.Stack);
        burger.RemoveAll(i => i == Ingredient.RawPatty);
        return burger;
    }

    /// <summary>제출: 가장 높은 더미를 꺼내고 보드를 전부 비운다 (다음 손님 준비).</summary>
    public List<Ingredient> TakeBestStack()
    {
        var stack = PeekBestStack();
        ClearBoard();
        RefreshState();
        return stack;
    }

    public void SpawnIngredient(Ingredient ingredient)
    {
        if (!IsPlaying || boardView.IsAnimating) return;
        // 패티 버튼은 항상 얼린 패티(RawPatty)를 꺼낸다. 4행에 생성되면 바로 구워진다.
        if (!Model.TrySpawn(ingredient, out var tile, out int row, out int col, out bool cooked)) return;   // 빈 칸 없음
        boardView.AddTile(tile, row, col);
        if (cooked) PlayCookedSfx();
        RefreshState();
        IngredientSpawned?.Invoke(ingredient);
    }

    void OnSwiped(Direction dir)
    {
        if (!IsPlaying || boardView.IsAnimating) return;
        var result = Model.Move(dir);
        if (!result.Changed) return;
        if (result.CookedTiles.Count > 0) PlayCookedSfx();
        // 슬라이드 후에는 새 재료를 만들지 않는다
        boardView.PlayMove(result, RefreshState);
    }

    // 패티가 구워질 때 효과음 (기존 AdvancedMain.cookedClip 사용, 손님 흐름이 있는 씬에서만)
    void PlayCookedSfx()
    {
<<<<<<< Updated upstream
        var list = new List<IngredientData>();
        BurgerTile bestBurger = GetBestBurger();
        if (bestBurger == null)
        {
            Debug.LogWarning("GetBestBurgerData: ?쒖텧???щ즺 ?⑹뼱由ш? ?놁뒿?덈떎.");
            return list;
        }
        foreach (var ingredientType in bestBurger.stackedIngredients)
        {
            var data = new IngredientData();
            data.IngredientType = ingredientType;
            list.Add(data);
        }

        list.Reverse();
        return list;
=======
        if (AdvancedMain.Inst != null && AdvancedMain.Inst.cookedClip != null && SFXPlayer.Instance != null)
            SFXPlayer.Instance.Play(AdvancedMain.Inst.cookedClip);
>>>>>>> Stashed changes
    }

    void ClearBoard()
    {
<<<<<<< Updated upstream
        Debug.Log("蹂대뱶??諛?二쇰Ц 珥덇린?붾맖");

        orderList.Clear();
        initialOrderList.Clear();
        isPlaying = false;

        SubmitAndClear();
        visualManager.DrawOrderList(orderList);
        UpdateAllVisuals();
    }
    
    public void RestartStage()
    {
        if (initialOrderList.Count == 0)
        {
            Debug.LogWarning("No active order is available to restart.");
            return;
        }

        StopAllCoroutines();
        isAnimating = false;
        moveRecords.Clear();

        SubmitAndClear();
        orderList = new List<IngredientType>(initialOrderList);
        isPlaying = true;

        SpawnNextIngredient();
        UpdateAllVisuals();
    }
    private void UpdateAllVisuals()
=======
        Model.Reset();
        boardView.Clear();
    }

    void RefreshState()
>>>>>>> Stashed changes
    {
        bool canSpawn = IsPlaying && Model.HasEmptyCell;
        foreach (var b in ingredientButtons)
            if (b != null && b.Button != null) b.Button.interactable = canSpawn;

        UpdateCounterPreview();
    }

    // 카운터 위에 "지금 제출하면 나갈 버거"를 미리 보여준다 (SideBurgerMaker가 있는 씬에서만).
    // 플레이 중에만 갱신한다: 제출 연출(FallingRoutine)이 이 미리보기를 그대로 사용하기 때문.
    void UpdateCounterPreview()
    {
<<<<<<< Updated upstream
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

            // ?앹꽦?섎뒗 ?⑦떚????긽 ?앺뙣???곹깭濡??ㅽ룿
            if (nextMaterial == IngredientType.CookedPatty) nextMaterial = IngredientType.RawPatty;

            BurgerTile targetTile = emptyTiles[randomIndex];
            targetTile.AddIngredient(nextMaterial);

            orderList.RemoveAt(0);
            visualManager.DrawOrderList(orderList);

            // ?ㅽ룿???꾩튂媛 洹몃┫?대씪硫?利됱떆 援쎄린 泥섎━
            if (targetTile.isGrill && targetTile.stackedIngredients[0] == IngredientType.RawPatty)
            {
                targetTile.stackedIngredients[0] = IngredientType.CookedPatty;
                SFXPlayer.Instance.Play(AdvancedMain.Inst.cookedClip);
            }

            nextIngrdient.gameObject.SetActive(true);
            if (orderList.Count == 0)
            {
                nextIngrdient.gameObject.SetActive(false);
                return;
            }
            nextIngrdient.sprite = visualManager.GetIngredientSprite(orderList[0] == IngredientType.CookedPatty ? IngredientType.RawPatty : orderList[0]);
        }
    }
    bool MoveTiles(string direction, List<TileMoveRecord> records)
    {
        bool didAnyMove = false;
        int dx = 0, dy = 0;

        if (direction == "Right") dx = 1;
        if (direction == "Left") dx = -1;
        if (direction == "Up") dy = 1;
        if (direction == "Down") dy = -1;

        int startX = (dx == 1) ? 3 : 0;
        int endX = (dx == 1) ? -1 : 4;
        int stepX = (dx == 1) ? -1 : 1;

        int startY = (dy == 1) ? 4 : 0;
        int endY = (dy == 1) ? -1 : 5;
        int stepY = (dy == 1) ? -1 : 1;

        bool changedInPass = true;
        int passIndex = 0;

        while (changedInPass)
        {
            changedInPass = false;

            for (int y = startY; y != endY; y += stepY)
            {
                for (int x = startX; x != endX; x += stepX)
                {
                    if (gameBoard[x, y].stackedIngredients.Count == 0)
                    {
                        continue;
                    }

                    int nx = x + dx;
                    int ny = y + dy;

                    if (nx < 0 || nx >= 4 || ny < 0 || ny >= 5)
                    {
                        continue;
                    }

                    List<IngredientType> currStack = gameBoard[x, y].stackedIngredients;
                    List<IngredientType> targetStack = gameBoard[nx, ny].stackedIngredients;

                    if (targetStack.Count == 0)
                    {
                        records.Add(new TileMoveRecord
                        {
                            from = new Vector2Int(x, y),
                            to = new Vector2Int(nx, ny),
                            merged = false,
                            step = passIndex
                        });

                        targetStack.AddRange(currStack);
                        currStack.Clear();
                        changedInPass = true;
                        didAnyMove = true;
                    }
                    else if (currStack.Count == targetStack.Count && CanMergeStacks(currStack, targetStack))
                    {
                        records.Add(new TileMoveRecord
                        {
                            from = new Vector2Int(x, y),
                            to = new Vector2Int(nx, ny),
                            merged = true,
                            step = passIndex
                        });

                        List<IngredientType> mergedStack = new List<IngredientType>(currStack.Count + targetStack.Count);

                        if (dx != 0)
                        {
                            mergedStack.AddRange(targetStack);
                            mergedStack.AddRange(currStack);
                        }
                        else if (dy > 0)
                        {
                            mergedStack.AddRange(currStack);
                            mergedStack.AddRange(targetStack);
                        }
                        else
                        {
                            mergedStack.AddRange(targetStack);
                            mergedStack.AddRange(currStack);
                        }

                        targetStack.Clear();
                        targetStack.AddRange(mergedStack);
                        currStack.Clear();
                        changedInPass = true;
                        didAnyMove = true;
                    }
                }
            }

            if (changedInPass)
            {
                passIndex++;
            }
        }

        for (int x = 0; x < 4; x++)
        {
            if (gameBoard[x, 0].stackedIngredients.Count == 0)
            {
                continue;
            }

            for (int i = 0; i < gameBoard[x, 0].stackedIngredients.Count; i++)
            {
                if (gameBoard[x, 0].stackedIngredients[i] == IngredientType.RawPatty)
                {
                    gameBoard[x, 0].stackedIngredients[i] = IngredientType.CookedPatty;
                    SFXPlayer.Instance.Play(AdvancedMain.Inst.cookedClip);
                    didAnyMove = true;
                }
            }
        }

        return didAnyMove;
    }

    bool CanMergeStacks(List<IngredientType> currStack, List<IngredientType> targetStack)
    {
        return !currStack.Contains(IngredientType.RawPatty) && !targetStack.Contains(IngredientType.RawPatty);
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
=======
        if (SideBurgerMaker.Inst == null || !IsPlaying) return;
        var stack = PeekBestStack();
        if (stack.Count == 0) SideBurgerMaker.Inst.ClearPreview();
        else SideBurgerMaker.Inst.Make(stack, isChat: false);
>>>>>>> Stashed changes
    }
}
