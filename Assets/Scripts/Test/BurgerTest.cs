using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BurgerTest : MonoBehaviour
{
    public RectTransform canvas;
    public Sprite[] burgerSprites;
    public Sprite topSprite, bottomSprite;
    public RectTransform container, dialogue;
    public CanvasGroup dialogueGroup;
    public Image burgerPrefab;
    public int Count = 5;

    public void Make()
    {
        StartCoroutine(MakeBurgerRoutine());
    }

    IEnumerator MakeBurgerRoutine()
    {
        dialogueGroup.alpha = 0;
        MakeBurger();
        yield return new WaitForSeconds(0.1f);
        ClampTop();
        dialogueGroup.DOFade(1, 0.2f);
    }
    public void MakeBurger()
    {
        foreach (Transform t in container)
        {
            Destroy(t.gameObject);
        }
        var bottom = Instantiate(burgerPrefab, container);
        bottom.sprite = bottomSprite;
        bottom.SetNativeSize();
        var rect = bottom.GetComponent<RectTransform>();
        rect.pivot = new Vector2(0.5f, 0.52f);
        
        var list = GetRandomIngredients(Count);
        foreach (IngredientType inType in list)
        {
            var ing = Instantiate(burgerPrefab, container);
            ing.sprite = GetRandomBurgerSprite(inType);
            ing.SetNativeSize();
        }
        
        

        
        var top = Instantiate(burgerPrefab, container);
        top.sprite = topSprite;
        top.SetNativeSize();
        
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(container);
        ClampTop();
    }
    
    public void ClampTop()
    {
        float safeTop = Screen.safeArea.yMax;

        float canvasHeight = canvas.rect.height;
        float safeTopCanvas = safeTop / Screen.height * canvasHeight - canvasHeight * 0.5f;

        float topY = dialogue.anchoredPosition.y + dialogue.rect.height * 0.5f;

        if (topY > safeTopCanvas)
        {
            dialogue.anchoredPosition += Vector2.up * (safeTopCanvas - topY);
        }
    }
    
    Sprite GetRandomBurgerSprite(IngredientType ingredient)
    {
        switch (ingredient)
        {
            case IngredientType.RawPatty:
                return burgerSprites[0];
            case IngredientType.CookedPatty:
                return burgerSprites[1];
            case IngredientType.Cheese:
                return burgerSprites[2];
            case IngredientType.Onion:
                return burgerSprites[3];
            case IngredientType.Lettuce:
                return burgerSprites[4];
            case IngredientType.Tomato:
                return burgerSprites[5];
            default:
                return null;
        }
    }
    
    List<IngredientType> GetRandomIngredients(int count)
    {
        IngredientType[] available =
        {
            IngredientType.RawPatty,
            IngredientType.CookedPatty,
            IngredientType.Cheese,
            IngredientType.Onion,
            IngredientType.Lettuce,
            IngredientType.Tomato
        };

        List<IngredientType> result = new();

        for (int i = 0; i < count; i++)
        {
            result.Add(available[UnityEngine.Random.Range(0, available.Length)]);
        }

        return result;
    }
}
