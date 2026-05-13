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
    // 기존 SpriteRenderer 방식 대신, 재료별 Sprite 이미지를 UI Image에 넣어서 표시한다.
    public void UpdateVisuals(BurgerTile[,] gameBoard)
    {
        ClearVisualList(activeVisuals);

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

                // 한 칸에 쌓인 재료들을 같은 위치에 겹쳐서 생성한다.
                // 아래 재료가 투명한 이미지라면 자연스럽게 비쳐 보일 수 있다.
                for (int i = 0; i < stack.Count; i++)
                {
                    GameObject newObj = Instantiate(ingredientUIPrefab, boardUIRoot);
                    activeVisuals.Add(newObj);

                    RectTransform rect = newObj.GetComponent<RectTransform>();

                    if (rect != null)
                    {
                        float posX = (x - 1.5f) * cellSize;
                        float posY = (2f - y) * cellSize;

                        rect.anchoredPosition = new Vector2(posX, posY);
                    }

                    SetIngredientImage(newObj, stack[i]);

                    // 가장 위에 보이는 재료에만 스택 개수를 표시한다.
                    SetStackText(newObj, i == stack.Count - 1 ? stack.Count.ToString() : "");

                    // 나중에 생성된 재료가 더 위에 보이도록 한다.
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