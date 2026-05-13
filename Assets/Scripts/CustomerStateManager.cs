using UnityEngine;
using UnityEngine.UI;

public class CustomerStateManager : MonoBehaviour
{
    public static CustomerStateManager Inst;

    [Header("UI References")]
    public Image genderImage;
    public Image skinImage;
    public Image emotionImage;
    public Image clothesImage;
    public Image hairImage;

    [Header("Databases")]
    public CustomerStateDB stateDB;

    private void Awake()
    {
        Inst = this;
    }

    public void ShowCustomer(CustomerRuntimeState state)
    {
        if (genderImage != null)
            genderImage.sprite = stateDB.genderSprites[(int)state.Gender];

        if (skinImage != null)
            skinImage.sprite = stateDB.skinSprites[(int)state.Skin];

        if (clothesImage != null)
            clothesImage.sprite = stateDB.clothesSprites[(int)state.Clothes];

        if (hairImage != null)
            hairImage.sprite = stateDB.hairSprites[(int)state.Hair];

        gameObject.SetActive(true);
    }

    // 평판에 따른 손님 감정 변화 UI
    public void UpdateEmotionUI(CustomerEmotion newEmotion)
    {
        if (emotionImage != null)
        {
            emotionImage.sprite = stateDB.emotionSprites[(int)newEmotion];
        }
    }
}
