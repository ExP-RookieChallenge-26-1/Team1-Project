using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CustomerStateManager : MonoBehaviour
{
    public static CustomerStateManager Inst;

    public Stage1Customer stage1Customer;
    
    [Header("Appearance")] 
    public RectTransform customerRect;
    public Image bodyTypeImage;
    public Image clothesTypeImage;
    public Image hairTypeImage;
    public Image specialImage;

    [Header("Emotion")]
    public Image emotionImage;

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
        if (data == null)
        {
            return;
        }

        currentCustomerData = data;

        string appLog = state != null ? $"성별:{state.Appearance.Gender}, Body:{state.Appearance.BodyTypeIndex}, Clothes:{state.Appearance.ClothesTypeIndex}, Hair:{state.Appearance.HairTypeIndex}" : "상태값 없음(Null)";
        Debug.Log($"손님:{data.CustomerName} | 타입:{data.GetType().Name} | 생성된 외형 묶음 => {appLog}");

        if (data is SpecialCustomerData specialData)
        {
            SetStandardAppearanceActive(false);
            if (specialImage != null) specialImage.gameObject.SetActive(false);
            if (emotionImage != null) emotionImage.gameObject.SetActive(false);
        }
        else if (data is DefaultCustomerData)
        {
            if (currentSpecialCustomer != null)
            {
                currentSpecialCustomer.gameObject.SetActive(false);
                currentSpecialCustomer = null;
            }

            if (specialImage != null) specialImage.gameObject.SetActive(false);

            ApplyAppearanceSprites(state.Appearance);
            SetStandardAppearanceActive(true);

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

    public void ShowEndingVIP(Sprite body)
    {
        gameObject.SetActive(true);

        customerRect.GetComponent<CanvasGroup>().alpha = 1;
        customerRect.gameObject.SetActive(true);
        customerRect.localScale = Vector3.one;
        customerRect.anchoredPosition3D = customerRect.anchoredPosition3D.SetY(-89);

        if (currentSpecialCustomer != null)
        {
            currentSpecialCustomer.gameObject.SetActive(false);
            currentSpecialCustomer = null;
        }
        if (specialImage != null) specialImage.gameObject.SetActive(false);
        if (clothesTypeImage != null) clothesTypeImage.gameObject.SetActive(false);
        if (hairTypeImage != null) hairTypeImage.gameObject.SetActive(false);
        if (emotionImage != null) emotionImage.gameObject.SetActive(false);

        if (bodyTypeImage != null)
        {
            bodyTypeImage.gameObject.SetActive(true);
            bodyTypeImage.sprite = body;
            bodyTypeImage.SetNativeSize();
        }
    }

    public void ApplyAppearanceSprites(CustomerAppearance appearance)
    {
        GenderSpriteSet targetSet = stateDB.GetSpriteSet(appearance.Gender);

        if (bodyTypeImage != null)
        {
            bodyTypeImage.sprite = targetSet.bodyTypes[appearance.BodyTypeIndex];
            bodyTypeImage.SetNativeSize();
        }
        if (clothesTypeImage != null)
        {
            clothesTypeImage.sprite = targetSet.clothesTypes[appearance.ClothesTypeIndex];
            clothesTypeImage.SetNativeSize();
        }
        if (hairTypeImage != null)
        {
            hairTypeImage.sprite = targetSet.hairTypes[appearance.HairTypeIndex];
            hairTypeImage.SetNativeSize();
        }
    }

    private void SetStandardAppearanceActive(bool isActive)
    {
        if (bodyTypeImage != null) bodyTypeImage.gameObject.SetActive(isActive);
        if (clothesTypeImage != null) clothesTypeImage.gameObject.SetActive(isActive);
        if (hairTypeImage != null) hairTypeImage.gameObject.SetActive(isActive);
    }

    public void HideCustomer()
    {
        customerRect.GetComponent<CanvasGroup>().DOFade(0, 1f);
        if (currentSpecialCustomer != null)
        {
            currentSpecialCustomer.HideAnimation();
        }


        VisualManager.Inst.previewUIRoot.GetComponent<CanvasGroup>().DOFade(0, 1f);
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

    public void ApplyEndingAppearance(Sprite body, Sprite clothes, Sprite hair, Sprite emotion)
    {
        if (bodyTypeImage != null && body != null) bodyTypeImage.sprite = body;
        if (clothesTypeImage != null && clothes != null) clothesTypeImage.sprite = clothes;
        if (hairTypeImage != null && hair != null) hairTypeImage.sprite = hair;
        if (emotionImage != null && emotion != null) emotionImage.sprite = emotion;
    }
}
