using UnityEngine;

public class CustomerRuntimeState
{
    public CustomerData BaseData;
<<<<<<< Updated upstream
    public CustomerGender Gender;
    public CustomerSkin Skin;
    public CustomerClothes Clothes;
    public CustomerHair Hair;
    public string DialogueText;
=======
    public CustomerAppearance Appearance;
>>>>>>> Stashed changes

    // 손님 초기 외형
    public CustomerRuntimeState(CustomerData data)
    {
        BaseData = data;

        data.SetState(this);
    }

    // 요구사항 충족 여부에 따른 손님 감정 변화
    public CustomerEmotion UpdateEmotion(OrderResult result)
    {
        return BaseData.GetEmotion(result);
    }
}
