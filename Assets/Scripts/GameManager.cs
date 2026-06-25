using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Inst;
    public Image nextIngrdient;
    private VisualManager visualManager;
    private BurgerTile[,] gameBoard = new BurgerTile[4, 5];
    private static readonly Vector2Int blockedCellPosition = new Vector2Int(0,0);

    // ?꾩옱 ?ㅽ룿 ?湲?以묒씤 ?⑥? 二쇰Ц 由ъ뒪??
    public List<IngredientType> orderList = new List<IngredientType>();

    // 泥섏쓬 寃뚯엫 ?쒖옉 ???ㅼ뼱???꾩껜 二쇰Ц 由ъ뒪??(?쒖텧 ??鍮꾧탳??
    private List<IngredientType> initialOrderList = new List<IngredientType>();

    public bool isPlaying = false;

    // ?щ즺 UI ?대룞 ?좊땲硫붿씠?섏쓣 ?꾪븳 ?대룞 湲곕줉
    public class TileMoveRecord
    {
        public Vector2Int from;
        public Vector2Int to;
        public bool merged;
        public int step;
    }

    private readonly List<TileMoveRecord> moveRecords = new List<TileMoveRecord>();
    private bool isAnimating = false;

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
                if (x == blockedCellPosition.x && y == blockedCellPosition.y)
                {
                    gameBoard[x, y].isBlocked = true;
                    continue;
                }
                // 留??꾨옯以?y=4)? 洹몃┫ ??쇰줈 ?ㅼ젙
                if (y == 0) gameBoard[x, y].isGrill = true;
            }
        }
    }

    //?ㅼ쭛媛쒕? 異⑸텇???쒕옒洹명빐?? 議곕━??씠 ?щ씪?붿쓣???몄텧?⑸땲??
    public void OnTongEndDrag()
    {
        //?ш린遺??寃뚯엫 濡쒖쭅???쒖옉?섎㈃ ??寃?媛숈븘??
        Debug.Log("OnTongEndDrag");
        StartGame();
    }

    [ContextMenu("START")]
    public void StartGame()
    {
        isAnimating = false;
        moveRecords.Clear();

        if (StageFlowManager.Inst)
        {
            if (AdvancedMain.Inst.isFaking)
            {
                orderList = AdvancedMain.Inst.stage6Customer.fakeBurgers;
            }
            else
            {
                var currentCustomer = StageFlowManager.Inst.CustomerQueueManager.GetCurrentCustomer();
                orderList = new();
                foreach (var data in currentCustomer.Recipe)
                {
                    orderList.Add(data.IngredientType);
                }   
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
        if (Stage5Mode.Inst != null && Stage5Mode.Inst.IsOn())
        {
            Stage5Mode.Inst.SetOrder();
        }

        initialOrderList = new List<IngredientType>(orderList);

        isPlaying = true;
        Debug.Log($"[寃뚯엫 ?쒖옉] 泥섏쓬 二쇰Ц 紐⑸줉: {string.Join(", ", initialOrderList)}");

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
        if (isAnimating) return;

        StartCoroutine(CorMoveInput(direction));
    }

    // ?대룞 ?낅젰??泥섎━?섍퀬, ?ㅼ젣 UI ?대룞 ?좊땲硫붿씠?섏씠 ?앸궃 ??理쒖쥌 ?붾㈃??媛깆떊?쒕떎.
    private IEnumerator CorMoveInput(string direction)
    {
        isAnimating = true;
        moveRecords.Clear();

        bool hasMoved = MoveTiles(direction, moveRecords);

        if (hasMoved)
        {
           // Debug.Log($"[{direction}] 諛⑺뼢?쇰줈 ?ㅼ??댄봽 ?꾨즺");

            // ?ㅼ젣 ?щ즺 UI瑜?異쒕컻 移몄뿉???꾩갑 移멸퉴吏 ?대룞?쒗궓??
            yield return StartCoroutine(visualManager.PlayMoveAnimation(moveRecords));

            // ?대룞???앸궃 ?????щ즺瑜??ㅽ룿?쒕떎.
            SpawnNextIngredient();
        }

        UpdateAllVisuals();
        isAnimating = false;
    }

    public void OnSubmitInput()
    {
        if (!isPlaying) return;

        BurgerTile bestBurger = GetBestBurger();
        string submittedIngredients = (bestBurger != null && bestBurger.stackedIngredients.Count > 0)
            ? string.Join(", ", bestBurger.stackedIngredients)
            : "鍮??묒떆";

        string initialOrder = string.Join(", ", initialOrderList);

        // 肄섏넄???쒖텧 寃곌낵 異쒕젰
        Debug.Log("=========================");
        Debug.Log("?뵒 踰꾧굅 ?꾩꽦 諛??쒖텧!");
        Debug.Log($"紐⑺몴 二쇰Ц: {initialOrder}");
        Debug.Log($"?쒖텧??踰꾧굅: {submittedIngredients}");
        Debug.Log("=========================");

        SubmitAndClear();
        UpdateAllVisuals();
    }

    public IReadOnlyList<IngredientData> GetBestBurgerData()
    {
        var list = new List<IngredientData>();
        BurgerTile bestBurger = GetBestBurger();
        if (bestBurger == null)
        {
            Debug.LogWarning("GetBestBurgerData: ?쒖텧???щ즺 ?⑹뼱由ш? ?놁뒿?덈떎.");
            return list;
        }
        foreach (var ingredientType in bestBurger.stackedIngredients)
        {
            var data = ScriptableObject.CreateInstance<IngredientData>();
            data.IngredientType = ingredientType;
            list.Add(data);
        }

        list.Reverse();
        return list;
    }

    public void OnResetInput()
    {
        Debug.Log("蹂대뱶??諛?二쇰Ц 珥덇린?붾맖");

        isAnimating = false;
        moveRecords.Clear();
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
    {
        visualManager.UpdateVisuals(gameBoard);
        visualManager.DrawPreview(GetBestBurger());
    }

    // ?곗긽??3, 0)?먯꽌 媛??媛源뚯슫 理쒓퀬??釉붾줉 ?먯깋 (嫄곕━ ?숈씪 ???믪씠(Y異? ?곗꽑)
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
                if (gameBoard[x, y].isBlocked)
                {
                    continue;

                }
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
                if (!gameBoard[x, y].isBlocked && gameBoard[x, y].stackedIngredients.Count == 0)
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
                    if (gameBoard[x, y].isBlocked || gameBoard[x, y].stackedIngredients.Count == 0)
                    {
                        continue;
                    }

                    int nx = x + dx;
                    int ny = y + dy;

                    if (nx < 0 || nx >= 4 || ny < 0 || ny >= 5)
                    {
                        continue;
                    }

                    if (gameBoard[nx, ny].isBlocked)
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
    }
}

