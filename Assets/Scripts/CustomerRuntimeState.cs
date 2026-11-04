using UnityEngine;

public class CustomerRuntimeState
{
    public CustomerData BaseData;
    public CustomerAppearance Appearance;

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
