using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SideBurgerRenderer : MonoBehaviour
{
    public static SideBurgerRenderer Inst;
    [System.Serializable]
    public struct IngredientSpriteMapping
    {
        public IngredientType type;
        public Sprite sprite;
    }

    [Header("재료 스프라이트 설정")]
    [SerializeField] private List<IngredientSpriteMapping> ingredientSprites;
    
    private Dictionary<IngredientType, Sprite> spriteDict;

    private void Awake()
    {
        Inst = this;
        spriteDict = new Dictionary<IngredientType, Sprite>();
        foreach (var mapping in ingredientSprites)
        {
            if (!spriteDict.ContainsKey(mapping.type))
                spriteDict.Add(mapping.type, mapping.sprite);
        }
    }

    public List<GameObject> BuildBurger(List<IngredientType> recipe, RectTransform burgerContainer)
    {
        List<GameObject> createdIngredients = new List<GameObject>();

        if (burgerContainer == null || recipe == null || recipe.Count == 0) 
            return createdIngredients;

        // 부모 컨테이너 Pivot/Anchor 바닥 중앙으로 강제 설정
       

        // 기존 자식 오브젝트 즉시 파괴
        for (int j = burgerContainer.childCount - 1; j >= 0; j--)
        {
            DestroyImmediate(burgerContainer.GetChild(j).gameObject);
        }

        float currentAnchorY = 0f; 

        for (int i = 0; i < recipe.Count; i++)
        {
            IngredientType currentType = recipe[i];
            
            if (!spriteDict.TryGetValue(currentType, out Sprite sprite) || sprite == null)
            {
                Debug.LogWarning($"{currentType}에 해당하는 스프라이트가 없습니다.");
                continue;
            }

            // 1. 오브젝트 생성 및 구조 설정
            GameObject ingredientObj = new GameObject(currentType.ToString(), typeof(RectTransform), typeof(Image));
            ingredientObj.transform.SetParent(burgerContainer, false);
            ingredientObj.transform.SetAsLastSibling(); // 새 재료가 항상 화면 맨 앞으로 오도록

            Image image = ingredientObj.GetComponent<Image>();
            image.sprite = sprite;
            image.SetNativeSize(); 

            RectTransform rectTransform = ingredientObj.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0f);
            rectTransform.anchorMax = new Vector2(0.5f, 0f);
            rectTransform.pivot = new Vector2(0.5f, 0f); 

            // [변환의 핵심 1] 이미지 컴포넌트가 캔버스 상에서 실제로 가지는 Rect 크기와 스프라이트 원본 픽셀 크기 비교
            // 이 비율을 곱해줘야 실제 기획서상의 픽셀 수치(32px, 12px)가 화면에 정확히 반영됩니다.
            float pixelToRectRatio = rectTransform.rect.height / sprite.rect.height;
            float spriteHeightInRect = rectTransform.rect.height;

            // 2. 기획서 기준 순수 픽셀(Pixel) 겹침 값 설정
            float overlapPixels = 0f;

            if (i > 0)
            {
                IngredientType prevType = recipe[i - 1];

                if (currentType == IngredientType.Cheese)
                {
                    overlapPixels = 32f*3;
                }
                else if (currentType == IngredientType.Lettuce)
                {
                    overlapPixels = 12f*3;
                }
                else if (currentType == IngredientType.RawPatty || currentType == IngredientType.CookedPatty)
                {
                    overlapPixels = 5f;
                }
                else if (prevType == IngredientType.RawPatty || prevType == IngredientType.CookedPatty)
                {
                    overlapPixels = 5f;
                }
            }

            // [변환의 핵심 2] 기획서 픽셀 수치에 비율을 곱해 UI Rect 좌표계 수치로 최종 변경!
            float overlapInRect = overlapPixels * pixelToRectRatio;

            // 3. UI 위치 계산 및 배치
            float finalY = currentAnchorY - overlapInRect-30;
            rectTransform.anchoredPosition = new Vector2(0f, finalY);

            // 4. 다음 재료를 위한 Rect 기준 높이 누적
            currentAnchorY = finalY + spriteHeightInRect;

            createdIngredients.Add(ingredientObj);
        }

        return createdIngredients;
    }
}