using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PeopleManager : MonoBehaviour
{
    public Image peopleImg;
    public CanvasGroup txtChatGroup, imgChatGroup;
    public RectTransform txtChatRect, imgChatRect;
    public TextMeshProUGUI chatTxt;
    public RectTransform burgerContainer;

    //MaterialType의 인덱스와 대응되게
    public Sprite[] materialSprites;
    public Sprite downBurn, upBurn;
    public Image materialImgPrefab;
    
    private float _txtStartY, _imgStartY;
    
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

    private void Update()
    {
        if(Keyboard.current.jKey.isPressed)
            ShowPeople();
        
        if(Keyboard.current.kKey.isPressed)
            ShowChat("안녕하세요!\n\n싸이버거를 주세요!");

        if (Keyboard.current.lKey.isPressed)
        {
            var list = new List<MaterialType> { MaterialType.Cheese , MaterialType.GrilledPatty, MaterialType.Lettuce, MaterialType.Tomato};
            ShowHamburger(list);
        }
            
    }

    public void ShowPeople()
    {
        peopleImg.gameObject.SetActive(true);
        peopleImg.color = peopleImg.color.SetAlpha(0);
        peopleImg.DOFade(1, 0.5f);
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

    public void ShowHamburger(List<MaterialType> materialTypes)
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
        upBurnMaterial.sprite = upBurn;
        
        foreach (var materialType in materialTypes)
        {
            var index = (int)materialType;
            if (index < 0 || index >= materialSprites.Length)
            {
                Debug.LogWarning($"{materialType}({index}) 스프라이트 없음");
                continue;
            }

            var newMaterial = Instantiate(materialImgPrefab, burgerContainer);
            newMaterial.sprite = materialSprites[index];
        }
        
        var downBurnMaterial = Instantiate(materialImgPrefab, burgerContainer);
        downBurnMaterial.sprite = downBurn;
    }

}
