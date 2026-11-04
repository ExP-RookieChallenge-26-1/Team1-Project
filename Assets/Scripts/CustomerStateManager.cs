using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CustomerStateManager : MonoBehaviour
{
    public static CustomerStateManager Inst;

    public enum ExitStyle
    {
        FadeOut,        // 제자리에서 모든 파츠가 함께 투명해진다
        SinkAndFade,    // 카운터 뒤로 내려가며 마지막에 짧게 투명해진다
    }

    [Header("일반 손님 퇴장 연출")]
    public ExitStyle exitStyle = ExitStyle.FadeOut;
    [Tooltip("퇴장 연출 시간(초). 다음 손님이 나오기 전에 끝나도록 0.5초 안팎을 권장")]
    public float exitDuration = 0.5f;
    
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

        string appLog = state != null ? $"성별:{state.Appearance.Gender}, Body:{state.Appearance.BodyTypeIndex}, Clothes:{state.Appearance.ClothesTypeIndex}, Hair:{state.Appearance.HairTypeIndex}" : "상태값 없음(Null)";
        Debug.Log($"손님:{data.CustomerName} | 타입:{data.GetType().Name} | 생성된 외형 묶음 => {appLog}");

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

        // 이전 손님 퇴장 연출이 남아 있으면 멈추고 새로 등장시킨다
        KillCustomerTweens(customerRect.GetComponent<CanvasGroup>());
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
        if (emotionImage != null)
        {
            Debug.Log($"인덱스값:{appearance.FaceTypeIndex}");

            emotionImage.sprite = targetSet.faceTypes[appearance.FaceTypeIndex];
            emotionImage.SetNativeSize();
        }
    }

    private void SetStandardAppearanceActive(bool isActive)
    {
        if (bodyTypeImage != null) bodyTypeImage.gameObject.SetActive(isActive);
        if (clothesTypeImage != null) clothesTypeImage.gameObject.SetActive(isActive);
        if (hairTypeImage != null) hairTypeImage.gameObject.SetActive(isActive);
        if (emotionImage != null) emotionImage.gameObject.SetActive(isActive);
    }

    public void HideCustomer()
    {
        var customerGroup = customerRect.GetComponent<CanvasGroup>();
        if (currentSpecialCustomer != null)
        {
            customerGroup.DOFade(0, 1f);
            currentSpecialCustomer.HideAnimation();
        }
        else
        {
            PlayExitAnimation(customerGroup);
        }


        // 카운터 위 버거 미리보기 숨기기 (기존 VisualManager.previewUIRoot 대신)
        if (SideBurgerMaker.Inst != null && SideBurgerMaker.Inst.counterPreview != null &&
            SideBurgerMaker.Inst.counterPreview.TryGetComponent<CanvasGroup>(out var previewGroup))
            previewGroup.DOFade(0, 1f);
        SideBurgerMaker.Inst.ClearVisualIng();
    }

    // 일반 손님 퇴장: 몸/옷/머리/표정 파츠가 모두 customerRect 아래에 있으므로 한 덩어리로 함께 사라진다.
    // 다음 손님이 나오기 전에 끝나야 이전 손님 파츠가 새 손님 그림으로 하나씩 바뀌어 보이지 않는다.
    void PlayExitAnimation(CanvasGroup customerGroup)
    {
        float duration = Mathf.Max(0.05f, exitDuration);
        KillCustomerTweens(customerGroup);

        if (exitStyle == ExitStyle.FadeOut)
        {
            customerGroup.DOFade(0, duration).SetEase(Ease.OutQuad);
            return;
        }

        // 겹친 파츠가 반투명하게 서로 비치지 않도록, 대부분 카운터 뒤로 숨은 뒤에만 짧게 투명해진다
        DOTween.Sequence()
            .Append(customerRect.DOAnchorPos3DY(-680, duration).SetEase(Ease.InBack))
            .Join(customerRect.transform.DOScale(new Vector3(1, 0.9f, 1), duration).SetEase(Ease.InQuad))
            .Insert(duration * 0.6f, customerGroup.DOFade(0, duration * 0.4f))
            .SetTarget(customerRect);
    }

    void KillCustomerTweens(CanvasGroup customerGroup)
    {
        customerRect.DOKill();
        customerRect.transform.DOKill();
        customerGroup.DOKill();
    }

    // 요구사항 충족 여부에 따른 손님 감정 변화 UI
    public void UpdateEmotionUI(CustomerEmotion newEmotion, CustomerAppearance? appearance = null)
    {
        if (currentSpecialCustomer != null)
        {
            Debug.Log($"야르");
            currentSpecialCustomer.UpdateEmotion(newEmotion);
            return;
        }
  
        if (emotionImage == null) return;
        emotionImage.gameObject.SetActive(true);
        if (currentCustomerData is SpecialCustomerData specialData)
        {
            Debug.Log($"야르일까");
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
