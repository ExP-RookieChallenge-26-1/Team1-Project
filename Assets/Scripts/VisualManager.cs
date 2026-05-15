using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VisualManager : MonoBehaviour
{
    public static VisualManager Inst;

    [Header("UI Roots")]
    [Tooltip("4x5 조리판 위에 생성되는 재료 UI들의 기준 위치")]
    public RectTransform boardUIRoot;

    [Tooltip("오른쪽 위 프리뷰에 생성되는 재료 UI들의 기준 위치")]
    public RectTransform previewUIRoot;

    [Header("UI Prefab")]
    [Tooltip("재료 하나를 표시할 UI 프리팹. Image 컴포넌트와 StackText 자식이 있어야 함")]
    public GameObject ingredientUIPrefab;

    [Header("Board UI Settings")]
    [Tooltip("조리판 한 칸 사이의 UI 간격")]
    public float cellSize = 110f;

    [Header("Preview UI Settings")]
    [Tooltip("프리뷰에서 재료가 위로 쌓여 보이는 간격")]
    public float previewStackOffset = 18f;

    [Header("Animation Settings")]
    [Tooltip("재료가 한 칸 이동하는 데 걸리는 시간")]
    public float moveDuration = 0.12f;

    [Tooltip("합쳐질 때 살짝 커지는 크기")]
    public float popScale = 1.15f;

    [Tooltip("합쳐질 때 팝 연출 시간")]
    public float popDuration = 0.08f;

    [Header("Ingredient Sprites")]
    [Tooltip("생패티 이미지")]
    public Sprite spriteFrozenPatty;

    [Tooltip("구운 패티 이미지")]
    public Sprite spriteBakedPatty;

    [Tooltip("치즈 이미지")]
    public Sprite spriteCheese;

    [Tooltip("양파 이미지")]
    public Sprite spriteOnion;

    [Tooltip("양상추 이미지")]
    public Sprite spriteLettuce;

    [Tooltip("토마토 이미지")]
    public Sprite spriteTomato;

    [Tooltip("탄 재료 이미지")]
    public Sprite spriteBurn;

    private readonly List<GameObject> activeVisuals = new List<GameObject>();
    private readonly List<GameObject> previewVisuals = new List<GameObject>();
    private readonly List<GameObject> orderVisuals = new List<GameObject>();

    private readonly Dictionary<Vector2Int, RectTransform> activeTileRoots = new Dictionary<Vector2Int, RectTransform>();

    private void Awake()
    {
        Inst = this;
    }

    // 기존 GameManager.Start()에서 호출하고 있어서 남겨둔 함수
    // 현재 Canvas UI 방식에서는 카메라 배경색을 직접 바꾸지 않는다.
    public void SetBackgroundColor()
    {
    }

    // 기존 GameManager.Start()에서 호출하고 있어서 남겨둔 함수
    // 조리판 배경은 Canvas에 배치된 UI 이미지가 담당한다고 가정한다.
    public void DrawBackgroundGrid()
    {
    }

    // 현재 4x5 gameBoard 상태를 Canvas UI에 다시 그리는 함수
    // 각 칸마다 TileRoot를 만들고, 그 아래에 재료 UI들을 겹쳐서 배치한다.
    public void UpdateVisuals(BurgerTile[,] gameBoard)
    {
        ClearVisualList(activeVisuals);
        activeTileRoots.Clear();

        if (boardUIRoot == null || ingredientUIPrefab == null)
        {
            Debug.LogWarning("VisualManager: BoardUIRoot 또는 IngredientUIPrefab이 연결되지 않았습니다.");
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

                // 한 칸에 쌓인 재료들을 같은 위치에 겹쳐서 생성한다.
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

                    // 가장 위에 보이는 재료에만 스택 개수를 표시한다.
                    SetStackText(newObj, i == stack.Count - 1 ? stack.Count.ToString() : "");

                    newObj.transform.SetAsLastSibling();
                }
            }
        }
    }

    // 현재 제출될 버거 덩어리를 오른쪽 위 프리뷰 UI에 보여주는 함수
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

        List<IngredientType> stack = bestBurger.stackedIngredients;

        for (int i = 0; i < stack.Count; i++)
        {
            GameObject newObj = Instantiate(ingredientUIPrefab, previewUIRoot);
            previewVisuals.Add(newObj);

            RectTransform rect = newObj.GetComponent<RectTransform>();

            if (rect != null)
            {
                // 프리뷰는 재료 순서를 보기 쉽도록 살짝 위로 쌓아서 보여준다.
                rect.anchoredPosition = new Vector2(0f, i * previewStackOffset);
                rect.localScale = Vector3.one;
            }

            SetIngredientImage(newObj, stack[i]);

            // 프리뷰도 가장 위 재료에만 전체 스택 개수를 표시한다.
            SetStackText(newObj, i == stack.Count - 1 ? stack.Count.ToString() : "");

            newObj.transform.SetAsLastSibling();
        }
    }

    // 조리탭 왼쪽에 남은 재료 목록을 표시하던 함수
    // 기획 변경으로 이제 남은 재료 목록은 표시하지 않는다.
    public void DrawOrderList(List<IngredientType> orderList)
    {
        ClearVisualList(orderVisuals);

        // 남은 재료 목록 UI는 더 이상 표시하지 않음
    }

    // 보드 좌표를 UI 좌표로 바꾼다.
    private Vector2 BoardToUIPosition(Vector2Int boardPos)
    {
        float posX = (boardPos.x - 1.5f) * cellSize;
        float posY = (2f - boardPos.y) * cellSize;

        return new Vector2(posX, posY);
    }

    // 특정 보드 칸에 해당하는 UI 묶음 오브젝트를 만든다.
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

    // GameManager에서 넘겨준 이동 기록대로 실제 UI를 출발 칸에서 도착 칸까지 움직인다.
    public IEnumerator PlayMoveAnimation(List<GameManager.TileMoveRecord> records)
    {
        if (records == null || records.Count == 0)
        {
            yield break;
        }

        foreach (GameManager.TileMoveRecord record in records)
        {
            if (!activeTileRoots.TryGetValue(record.from, out RectTransform movingRoot))
            {
                continue;
            }

            Vector2 targetPos = BoardToUIPosition(record.to);

            yield return StartCoroutine(MoveToPosition(movingRoot, targetPos, moveDuration));

            if (record.merged)
            {
                // 병합된 경우: 움직인 쪽은 사라지고, 도착 칸 쪽이 팝 연출을 한다.
                activeTileRoots.Remove(record.from);

                if (activeTileRoots.TryGetValue(record.to, out RectTransform targetRoot) && targetRoot != movingRoot)
                {
                    Destroy(movingRoot.gameObject);
                    yield return StartCoroutine(PopEffect(targetRoot));
                }
                else
                {
                    activeTileRoots[record.to] = movingRoot;
                    yield return StartCoroutine(PopEffect(movingRoot));
                }
            }
            else
            {
                // 단순 이동인 경우: 위치 기록만 갱신한다.
                activeTileRoots.Remove(record.from);
                activeTileRoots[record.to] = movingRoot;
            }
        }
    }

    // UI 오브젝트를 현재 위치에서 목표 위치까지 부드럽게 이동시킨다.
    private IEnumerator MoveToPosition(RectTransform rect, Vector2 targetPos, float duration)
    {
        Vector2 startPos = rect.anchoredPosition;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);

            rect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);

            yield return null;
        }

        rect.anchoredPosition = targetPos;
    }

    // 합쳐진 재료 덩어리가 살짝 커졌다가 원래 크기로 돌아오는 연출
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

    // 생성된 UI 오브젝트들을 삭제하고 리스트를 비운다.
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

    // 재료 UI 오브젝트의 Image에 재료 종류에 맞는 Sprite를 넣는다.
    private void SetIngredientImage(GameObject targetObj, IngredientType type)
    {
        Image image = targetObj.GetComponent<Image>();

        if (image == null)
        {
            Debug.LogWarning("VisualManager: IngredientUI 프리팹에 Image 컴포넌트가 없습니다.");
            return;
        }

        image.sprite = GetIngredientSprite(type);
        image.color = Color.white;
        image.preserveAspect = true;
    }

    // 재료 UI 오브젝트 안의 StackText에 스택 개수를 표시한다.
    private void SetStackText(GameObject targetObj, string text)
    {
        TMP_Text stackText = targetObj.GetComponentInChildren<TMP_Text>();

        if (stackText != null)
        {
            stackText.text = text;
        }
    }

    // 재료 종류에 맞는 Sprite를 반환한다.
    private Sprite GetIngredientSprite(IngredientType type)
    {
        switch (type)
        {
            case IngredientType.FrozenPatty:
                return spriteFrozenPatty;

            case IngredientType.BakedPatty:
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

    // 기존 다른 스크립트와의 호환을 위해 남겨둔 함수
    // MaterialItem 등에서 재료 타입별 색상이 필요할 때 사용한다.
    public Color GetIngredientColor(IngredientType type)
    {
        switch (type)
        {
            case IngredientType.FrozenPatty:
                return Color.gray;

            case IngredientType.BakedPatty:
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