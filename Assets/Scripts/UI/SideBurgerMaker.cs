using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SideBurgerMaker : MonoBehaviour
{
    public static SideBurgerMaker Inst;
    
    public RectTransform canvas;
    public Sprite[] burgerSprites;
    public Sprite topSprite, bottomSprite;
    public RectTransform container, dialogue;
    public RectTransform counterContainer, counterPreview;
    public RectTransform counterPreviewPivot;
    public CanvasGroup dialogueGroup;
    public Image burgerPrefab;
    public int Count = 5;

    private void Awake()
    {
        Inst = this;
    }

    public void Make(List<IngredientType> list, bool isChat = true)
    {
        StartCoroutine(MakeBurgerRoutine(list, isChat));
    }
    
    

    IEnumerator MakeBurgerRoutine(List<IngredientType> list, bool isChat = true)
    {
        if(isChat)
            dialogueGroup.alpha = 0;

        counterPreview.GetComponent<CanvasGroup>().alpha = 0.7f;
        MakeBurger(list, isChat);
        yield return new WaitForSeconds(0.1f);
        if (isChat)
        {
            ClampTop();   
            dialogueGroup.DOFade(1, 0.2f);
        }
        else
        {
            counterPreviewPivot.gameObject.SetActive(false);
            LayoutRebuilder.ForceRebuildLayoutImmediate(counterPreviewPivot);
            counterPreviewPivot.gameObject.SetActive(true);
        }
    }
    public void MakeBurger(List<IngredientType> list, bool isChat = true)
    {
        var curContainer = isChat ? container : counterContainer;
        foreach (Transform t in curContainer)
        {
            Destroy(t.gameObject);
        }
        var bottom = Instantiate(burgerPrefab, curContainer);
        bottom.sprite = bottomSprite;
        bottom.SetNativeSize();
        var rect = bottom.GetComponent<RectTransform>();
        rect.pivot = new Vector2(0.5f, 0.52f);
        
        foreach (IngredientType inType in list)
        {
            var ing = Instantiate(burgerPrefab, curContainer);
            ing.sprite = GetRandomBurgerSprite(inType);
            ing.SetNativeSize();
        }
        
        

        
        var top = Instantiate(burgerPrefab, curContainer);
        top.sprite = topSprite;
        top.SetNativeSize();
        
        counterPreviewPivot.gameObject.SetActive(false);
        LayoutRebuilder.ForceRebuildLayoutImmediate(curContainer);
        counterPreviewPivot.gameObject.SetActive(true);
        
        if(isChat)
            ClampTop();
    }

    public void MakeVisualIngredient(RectTransform target)
    {
        var visual = Instantiate(burgerPrefab, counterPreviewPivot);
        visual.sprite = target.GetComponent<Image>().sprite;
        visual.GetComponent<RectTransform>().DOAnchorPos3D(target.anchoredPosition3D, 0.2f).SetEase(Ease.InQuart);
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
