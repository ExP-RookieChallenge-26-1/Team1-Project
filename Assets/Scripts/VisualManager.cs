using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VisualManager : MonoBehaviour
{
    public static VisualManager Inst;

    [Header("UI Roots")]
    [Tooltip("Board root for spawned ingredient UI")]
    public RectTransform boardUIRoot;

    [Tooltip("Preview root for burger preview UI")]
    public RectTransform previewUIRoot;

    [Header("UI Prefab")]
    [Tooltip("Ingredient UI prefab with Image and stack text")]
    public GameObject ingredientUIPrefab;

    [Header("Board UI Settings")]
    [Tooltip("Board cell size and spacing")]
    public float cellSize = 110f;

    public Vector2 cellStartPos, cellSpacing;

    [Header("Preview UI Settings")]
    [Tooltip("Vertical offset between preview ingredients")]
    public float previewStackOffset = 18f;

    [Header("Animation Settings")]
    [Tooltip("Duration for ingredient movement animation")]
    public float moveDuration = 0.12f;

    [Tooltip("Scale used for merge pop effect")]
    public float popScale = 1.15f;

    [Tooltip("Duration for merge pop effect")]
    public float popDuration = 0.08f;

    [Header("Ingredient Sprites")]
    [Tooltip("Frozen patty sprite")]
    public Sprite spriteFrozenPatty;

    [Tooltip("Cooked patty sprite")]
    public Sprite spriteBakedPatty;

    [Tooltip("Cheese sprite")]
    public Sprite spriteCheese;

    [Tooltip("Onion sprite")]
    public Sprite spriteOnion;

    [Tooltip("Lettuce sprite")]
    public Sprite spriteLettuce;

    [Tooltip("Tomato sprite")]
    public Sprite spriteTomato;

    [Tooltip("Burnt ingredient sprite")]
    public Sprite spriteBurn;

    private readonly List<GameObject> activeVisuals = new List<GameObject>();
    private readonly List<GameObject> previewVisuals = new List<GameObject>();
    private readonly List<GameObject> orderVisuals = new List<GameObject>();

    private readonly Dictionary<Vector2Int, RectTransform> activeTileRoots = new Dictionary<Vector2Int, RectTransform>();

    private void Awake()
    {
        Inst = this;
    }

    // 湲곗〈 GameManager.Start()?먯꽌 ?몄텧?섍퀬 ?덉뼱???④꺼???⑥닔
    // ?꾩옱 Canvas UI 諛⑹떇?먯꽌??移대찓??諛곌꼍?됱쓣 吏곸젒 諛붽씀吏 ?딅뒗??
    public void SetBackgroundColor()
    {
    }

    // 湲곗〈 GameManager.Start()?먯꽌 ?몄텧?섍퀬 ?덉뼱???④꺼???⑥닔
    // 議곕━??諛곌꼍? Canvas??諛곗튂??UI ?대?吏媛 ?대떦?쒕떎怨?媛?뺥븳??
    public void DrawBackgroundGrid()
    {
    }

    // ?꾩옱 4x5 gameBoard ?곹깭瑜?Canvas UI???ㅼ떆 洹몃━???⑥닔
    // 媛?移몃쭏??TileRoot瑜?留뚮뱾怨? 洹??꾨옒???щ즺 UI?ㅼ쓣 寃뱀퀜??諛곗튂?쒕떎.
    public void UpdateVisuals(BurgerTile[,] gameBoard)
    {
        ClearVisualList(activeVisuals);
        activeTileRoots.Clear();

        if (boardUIRoot == null || ingredientUIPrefab == null)
        {
            Debug.LogWarning("VisualManager: BoardUIRoot ?먮뒗 IngredientUIPrefab???곌껐?섏? ?딆븯?듬땲??");
            return;
        }

        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                List<IngredientType> stack = gameBoard[x, y].stackedIngredients;

                if (stack == null || stack.Count == 0)
                {
                    continue;
                }

                Vector2Int boardPos = new Vector2Int(x, y);
                RectTransform tileRoot = CreateTileRoot(boardPos);
                activeTileRoots[boardPos] = tileRoot;

                // ??移몄뿉 ?볦씤 ?щ즺?ㅼ쓣 媛숈? ?꾩튂??寃뱀퀜???앹꽦?쒕떎.
                for (int i = 0; i < stack.Count; i++)
                {
                    GameObject newObj = Instantiate(ingredientUIPrefab, tileRoot);
                    RectTransform rect = newObj.GetComponent<RectTransform>();

                    if (rect != null)
                    {
                        rect.anchoredPosition = Vector2.zero;
                        rect.localScale = Vector3.one;
                    }

                    SetIngredientImage(newObj, stack[i]);

                    // 媛???꾩뿉 蹂댁씠???щ즺?먮쭔 ?ㅽ깮 媛쒖닔瑜??쒖떆?쒕떎.
                    SetStackText(newObj, i == stack.Count - 1 ? stack.Count.ToString() : "");

                    newObj.transform.SetAsLastSibling();
                }
            }
        }
    }

    // ?꾩옱 ?쒖텧??踰꾧굅 ?⑹뼱由щ? ?ㅻⅨ履????꾨━酉?UI??蹂댁뿬二쇰뒗 ?⑥닔
    public void DrawPreview(BurgerTile bestBurger)
    {
        ClearVisualList(previewVisuals);

        if (previewUIRoot == null || ingredientUIPrefab == null)
        {
            return;
        }

        if (bestBurger == null || bestBurger.stackedIngredients == null || bestBurger.stackedIngredients.Count == 0)
        {
            return;
        }

        List<IngredientType> stack = new List<IngredientType>(bestBurger.stackedIngredients);
        stack.Insert(0, IngredientType.Burn);
        stack.Add(IngredientType.TopBurn);
        SideBurgerRenderer.Inst.BuildBurger(stack, previewUIRoot);
        previewUIRoot.GetComponent<CanvasGroup>().alpha = 0.7f;
        return;

        for (int i = 0; i < stack.Count; i++)
        {
            GameObject newObj = Instantiate(ingredientUIPrefab, previewUIRoot);
            previewVisuals.Add(newObj);

            RectTransform rect = newObj.GetComponent<RectTransform>();

            if (rect != null)
            {
                // ?꾨━酉곕뒗 ?щ즺 ?쒖꽌瑜?蹂닿린 ?쎈룄濡??댁쭩 ?꾨줈 ?볦븘??蹂댁뿬以??
                rect.anchoredPosition = new Vector2(0f, i * previewStackOffset);
                rect.localScale = Vector3.one;
            }

            SetIngredientImage(newObj, stack[i]);

            // ?꾨━酉곕룄 媛?????щ즺?먮쭔 ?꾩껜 ?ㅽ깮 媛쒖닔瑜??쒖떆?쒕떎.
            SetStackText(newObj, i == stack.Count - 1 ? stack.Count.ToString() : "");

            newObj.transform.SetAsLastSibling();
        }
    }

    // 議곕━???쇱そ???⑥? ?щ즺 紐⑸줉???쒖떆?섎뜕 ?⑥닔
    // 湲고쉷 蹂寃쎌쑝濡??댁젣 ?⑥? ?щ즺 紐⑸줉? ?쒖떆?섏? ?딅뒗??
    public void DrawOrderList(List<IngredientType> orderList)
    {
        ClearVisualList(orderVisuals);

        // ?⑥? ?щ즺 紐⑸줉 UI?????댁긽 ?쒖떆?섏? ?딆쓬
    }

    // 蹂대뱶 醫뚰몴瑜?UI 醫뚰몴濡?諛붽씔??
    private Vector2 BoardToUIPosition(Vector2Int boardPos)
    {
        float posX = cellStartPos.x +boardPos.x * (cellSize + cellSpacing.x);
        float posY = cellStartPos.y + boardPos.y * (cellSize + cellSpacing.y);

        return new Vector2(posX, posY);
    }

    // ?뱀젙 蹂대뱶 移몄뿉 ?대떦?섎뒗 UI 臾띠쓬 ?ㅻ툕?앺듃瑜?留뚮뱺??
    private RectTransform CreateTileRoot(Vector2Int boardPos)
    {
        GameObject rootObj = new GameObject($"TileRoot_{boardPos.x}_{boardPos.y}", typeof(RectTransform));
        rootObj.transform.SetParent(boardUIRoot, false);

        RectTransform rect = rootObj.GetComponent<RectTransform>();
        rect.anchoredPosition = BoardToUIPosition(boardPos);
        rect.localScale = Vector3.one;

        activeVisuals.Add(rootObj);

        return rect;
    }

    // GameManager?먯꽌 ?섍꺼以 ?대룞 湲곕줉?濡??ㅼ젣 UI瑜?異쒕컻 移몄뿉???꾩갑 移멸퉴吏 ?吏곸씤??
    public IEnumerator PlayMoveAnimation(List<GameManager.TileMoveRecord> records)
    {
        if (records == null || records.Count == 0)
        {
            yield break;
        }

        int maxStep = 0;
        foreach (GameManager.TileMoveRecord record in records)
        {
            if (record.step > maxStep)
            {
                maxStep = record.step;
            }
        }
        SFXPlayer.Instance.Play(AdvancedMain.Inst.swipeClip);
        for (int step = 0; step <= maxStep; step++)
        {
            List<GameManager.TileMoveRecord> stepRecords = records.FindAll(record => record.step == step);
            List<GameManager.TileMoveRecord> animatedRecords = new List<GameManager.TileMoveRecord>();
            List<RectTransform> movingRoots = new List<RectTransform>();
            List<Vector2> targetPositions = new List<Vector2>();

            foreach (GameManager.TileMoveRecord record in stepRecords)
            {
                if (!activeTileRoots.TryGetValue(record.from, out RectTransform movingRoot))
                {
                    continue;
                }

                animatedRecords.Add(record);
                movingRoots.Add(movingRoot);
                targetPositions.Add(BoardToUIPosition(record.to));
            }

            if (movingRoots.Count > 0)
            {
                yield return StartCoroutine(MoveBatch(movingRoots, targetPositions, moveDuration));
            }

            List<RectTransform> popTargets = new List<RectTransform>();

            for (int i = 0; i < animatedRecords.Count; i++)
            {
                GameManager.TileMoveRecord record = animatedRecords[i];
                RectTransform movingRoot = movingRoots[i];

                activeTileRoots.Remove(record.from);

                if (record.merged)
                {
                    SFXPlayer.Instance.Play(AdvancedMain.Inst.mergeClip);
                    if (activeTileRoots.TryGetValue(record.to, out RectTransform targetRoot) && targetRoot != movingRoot)
                    {
                        Destroy(movingRoot.gameObject);
                        if (!popTargets.Contains(targetRoot))
                        {
                            popTargets.Add(targetRoot);
                        }
                    }
                    else
                    {
                        activeTileRoots[record.to] = movingRoot;
                        if (!popTargets.Contains(movingRoot))
                        {
                            popTargets.Add(movingRoot);
                        }
                    }
                }
                else
                {
                    activeTileRoots[record.to] = movingRoot;
                }
            }

            foreach (RectTransform popTarget in popTargets)
            {
                yield return StartCoroutine(PopEffect(popTarget));
            }
        }
    }

    private IEnumerator MoveBatch(List<RectTransform> rects, List<Vector2> targetPositions, float duration)
    {
        List<Vector2> startPositions = new List<Vector2>(rects.Count);
        for (int i = 0; i < rects.Count; i++)
        {
            startPositions.Add(rects[i].anchoredPosition);
        }

        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(time / duration));

            for (int i = 0; i < rects.Count; i++)
            {
                rects[i].anchoredPosition = Vector2.Lerp(startPositions[i], targetPositions[i], t);
            }

            yield return null;
        }

        for (int i = 0; i < rects.Count; i++)
        {
            rects[i].anchoredPosition = targetPositions[i];
        }
    }

    // UI ?ㅻ툕?앺듃瑜??꾩옱 ?꾩튂?먯꽌 紐⑺몴 ?꾩튂源뚯? 遺?쒕읇寃??대룞?쒗궓??
    private IEnumerator MoveToPosition(RectTransform rect, Vector2 targetPos, float duration)
    {
        Vector2 startPos = rect.anchoredPosition;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(time / duration));

            rect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);

            yield return null;
        }

        rect.anchoredPosition = targetPos;
    }

    // ?⑹퀜吏??щ즺 ?⑹뼱由ш? ?댁쭩 而ㅼ죱?ㅺ? ?먮옒 ?ш린濡??뚯븘?ㅻ뒗 ?곗텧
    private IEnumerator PopEffect(RectTransform rect)
    {
        Vector3 originalScale = Vector3.one;
        Vector3 targetScale = Vector3.one * popScale;

        float time = 0f;

        while (time < popDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / popDuration);

            rect.localScale = Vector3.Lerp(originalScale, targetScale, t);

            yield return null;
        }

        time = 0f;

        while (time < popDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / popDuration);

            rect.localScale = Vector3.Lerp(targetScale, originalScale, t);

            yield return null;
        }

        rect.localScale = originalScale;
    }

    // ?앹꽦??UI ?ㅻ툕?앺듃?ㅼ쓣 ??젣?섍퀬 由ъ뒪?몃? 鍮꾩슫??
    private void ClearVisualList(List<GameObject> visuals)
    {
        foreach (GameObject obj in visuals)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }

        visuals.Clear();
    }

    // ?щ즺 UI ?ㅻ툕?앺듃??Image???щ즺 醫낅쪟??留욌뒗 Sprite瑜??ｋ뒗??
    public void SetIngredientImage(GameObject targetObj, IngredientType type)
    {
        Image image = targetObj.GetComponent<Image>();

        if (image == null)
        {
            Debug.LogWarning("VisualManager: IngredientUI ?꾨━?뱀뿉 Image 而댄룷?뚰듃媛 ?놁뒿?덈떎.");
            return;
        }

        image.sprite = GetIngredientSprite(type);
        image.color = Color.white;
        image.preserveAspect = true;
    }

    // ?щ즺 UI ?ㅻ툕?앺듃 ?덉쓽 StackText???ㅽ깮 媛쒖닔瑜??쒖떆?쒕떎.
    private void SetStackText(GameObject targetObj, string text)
    {
        TMP_Text stackText = targetObj.GetComponentInChildren<TMP_Text>();

        if (stackText != null)
        {
            stackText.text = text;
        }
    }

    // ?щ즺 醫낅쪟??留욌뒗 Sprite瑜?諛섑솚?쒕떎.
    public Sprite GetIngredientSprite(IngredientType type)
    {
        switch (type)
        {
            case IngredientType.RawPatty:
                return spriteFrozenPatty;

            case IngredientType.CookedPatty:
                return spriteBakedPatty;

            case IngredientType.Cheese:
                return spriteCheese;

            case IngredientType.Onion:
                return spriteOnion;

            case IngredientType.Lettuce:
                return spriteLettuce;

            case IngredientType.Tomato:
                return spriteTomato;

            case IngredientType.Burn:
                return spriteBurn;

            default:
                return null;
        }
    }

    // 湲곗〈 ?ㅻⅨ ?ㅽ겕由쏀듃????명솚???꾪빐 ?④꺼???⑥닔
    // MaterialItem ?깆뿉???щ즺 ??낅퀎 ?됱긽???꾩슂?????ъ슜?쒕떎.
    public Color GetIngredientColor(IngredientType type)
    {
        switch (type)
        {
            case IngredientType.RawPatty:
                return Color.gray;

            case IngredientType.CookedPatty:
                return new Color(0.4f, 0.2f, 0f);

            case IngredientType.Cheese:
                return Color.yellow;

            case IngredientType.Onion:
                return Color.white;

            case IngredientType.Lettuce:
                return Color.green;

            case IngredientType.Tomato:
                return Color.red;

            case IngredientType.Burn:
                return new Color(1f, 0.5f, 0.3f);

            default:
                return Color.white;
        }
    }
}