using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CustomerStateManager : MonoBehaviour
{
    public static CustomerStateManager Inst;
    
    [Header("UI References")] 
    public RectTransform customerRect;
    public Image genderImage;
    public Image skinImage;
    public Image emotionImage;
    public Image clothesImage;
    public Image hairImage;
    public Image specialImage;

    [Header("Databases")]
    public CustomerStateDB stateDB;

    private CustomerData currentCustomerData;
    public SpecialBase currentSpecialCustomer;

    private void Awake()
    {
        Inst = this;
    }

    private void Start()
    {
        customerRect.gameObject.SetActive(false);
    }

    public void ShowCustomer(CustomerData data, CustomerRuntimeState state)
    {
        currentCustomerData = data;

        if (data is SpecialCustomerData specialData)
        {
            if (specialImage != null && specialData.specialSprites.Length > 0)
            {
                specialImage.gameObject.SetActive(true);
                specialImage.sprite = specialData.specialSprites[0];
            }

            if (genderImage != null) genderImage.gameObject.SetActive(false);
            if (skinImage != null) skinImage.gameObject.SetActive(false);
            if (clothesImage != null) clothesImage.gameObject.SetActive(false);
            if (hairImage != null) hairImage.gameObject.SetActive(false);
            if (emotionImage != null) emotionImage.gameObject.SetActive(false);
        }

        else if (data is DefaultCustomerData)
        {
            if (specialImage != null) specialImage.gameObject.SetActive(false);

            if (genderImage != null)
            {
                genderImage.gameObject.SetActive(true);
                genderImage.sprite = stateDB.genderSprites[(int)state.Gender];    
            }


            if (skinImage != null)
            {
                skinImage.gameObject.SetActive(true);
                skinImage.sprite = stateDB.skinSprites[(int)state.Skin];
            }


            if (clothesImage != null)
            {
                clothesImage.gameObject.SetActive(true);
                clothesImage.sprite = stateDB.clothesSprites[(int)state.Clothes];
            }


            if (hairImage != null)
            {
                hairImage.gameObject.SetActive(true);
                hairImage.sprite = stateDB.hairSprites[(int)state.Hair];
            }

            if (emotionImage != null)
            {
                emotionImage.gameObject.SetActive(true);
                emotionImage.sprite = stateDB.emotionSprites[1];
            }
                

            gameObject.SetActive(true);
        }

        customerRect.GetComponent<CanvasGroup>().alpha = 1;
        customerRect.gameObject.SetActive(true);
        customerRect.transform.localScale = new Vector3(1, 0.8f, 1);
        customerRect.anchoredPosition3D = customerRect.anchoredPosition3D.SetY(-680);
        customerRect.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.InOutBack);
        customerRect.DOAnchorPos3DY(-89, 0.15f);
    }

    public void HideCustomer()
    {
        customerRect.GetComponent<CanvasGroup>().DOFade(0, 1f);
        if (currentSpecialCustomer != null)
        {
            currentSpecialCustomer.HideAnimation();
        }


        VisualManager.Inst.previewUIRoot.GetComponent<CanvasGroup>().DOFade(0, 1f);
        SideBurgerMaker.Inst.ClearVisualIng();
    }

    // 평판에 따른 손님 감정 변화 UI
    public void UpdateEmotionUI(CustomerEmotion newEmotion)
    {
        if (currentSpecialCustomer != null)
        {
            currentSpecialCustomer.UpdateEmotion(newEmotion);
            return;
        }
  
        if (emotionImage == null) return;
        emotionImage.gameObject.SetActive(true);
        if (currentCustomerData is SpecialCustomerData specialData)
        {
            if (specialData.emotionSprites.Length > (int)newEmotion)
            {
                emotionImage.sprite = specialData.emotionSprites[(int)newEmotion];
                emotionImage.SetNativeSize();
            }
        }
        else
        {
            emotionImage.sprite = stateDB.emotionSprites[(int)newEmotion];
            emotionImage.SetNativeSize();
        }
    }
}
