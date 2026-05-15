using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PeopleManager : MonoBehaviour
{
    public static PeopleManager Inst;
    
    public Image peopleImg;
    public CanvasGroup txtChatGroup, imgChatGroup;
    public RectTransform txtChatRect, imgChatRect;
    public TextMeshProUGUI chatTxt;
    public RectTransform burgerContainer;

    public MaterialItem materialImgPrefab;
    
    private float _txtStartY, _imgStartY;

    private void Awake()
    {
        Inst = this;
    }

    void Start()
    {
        //UI 초기화
        txtChatGroup.gameObject.SetActive(false);
        txtChatGroup.alpha = 0;
        
        imgChatGroup.gameObject.SetActive(false);
        imgChatGroup.alpha = 0;

        foreach (Transform b in burgerContainer)
        {
            Destroy(b.gameObject);
        }

        _txtStartY = txtChatRect.anchoredPosition3D.y;
        _imgStartY = imgChatRect.anchoredPosition3D.y;

        peopleImg.color = peopleImg.color.SetAlpha(0);
        peopleImg.gameObject.SetActive(false);
    }

    public void ShowPeople()
    {
        peopleImg.gameObject.SetActive(true);
        peopleImg.color = peopleImg.color.SetAlpha(0);
        peopleImg.DOFade(1, 0.5f);
    }

    public void HidePeople()
    {
        peopleImg.DOFade(0, 0.5f);
        imgChatGroup.DOFade(0, 0.5f);
        txtChatGroup.DOFade(0, 0.5f);
    }

    public void ShowChat(string chat)
    {
        imgChatGroup.gameObject.SetActive(false);
        
        txtChatGroup.alpha = 0;
        
        txtChatGroup.gameObject.SetActive(true);
        txtChatGroup.DOFade(1, 0.3f);
        chatTxt.text = "";
        chatTxt.DOText(chat, 1.5f);

        txtChatRect.anchoredPosition3D = txtChatRect.anchoredPosition3D.SetY(_txtStartY-50);
        txtChatRect.DOAnchorPos3DY(_txtStartY, 0.3f);
    }

    public void ShowHamburger(IReadOnlyList<IngredientData> datas)
    {
        txtChatGroup.gameObject.SetActive(false);

        imgChatGroup.alpha = 0;
        imgChatGroup.gameObject.SetActive(true);
        imgChatGroup.DOFade(1, 0.3f);
        
        imgChatRect.anchoredPosition3D = txtChatRect.anchoredPosition3D.SetY(_imgStartY-50);
        imgChatRect.DOAnchorPos3DY(_imgStartY, 0.3f);
        
        
        foreach (Transform b in burgerContainer)
        {
            Destroy(b.gameObject);
        }
        
        //햄버거 이미지 생성
        var upBurnMaterial = Instantiate(materialImgPrefab, burgerContainer);
        upBurnMaterial.Init(IngredientType.Burn);

        var reverse = datas.Reverse();
        foreach (var data in reverse)
        {
            var newMaterial = Instantiate(materialImgPrefab, burgerContainer);
            newMaterial.Init(data.IngredientType);
        }
        
        var downBurnMaterial = Instantiate(materialImgPrefab, burgerContainer);
        downBurnMaterial.Init(IngredientType.Burn);
    }
}
