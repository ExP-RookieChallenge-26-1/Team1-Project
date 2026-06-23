using UnityEngine;

public class CustomerRuntimeState
{
    public CustomerData BaseData;
    public CustomerAppearance Appearance;
    public string DialogueText;

    private CustomerEmotion currentEmotion;
    public CustomerEmotion CurrentEmotion => currentEmotion;

    // 손님 초기 외형, 대사, 감정
    public CustomerRuntimeState(CustomerData data)
    {
        BaseData = data;
        DialogueText = data.GetDialogue();
        currentEmotion = CustomerEmotion.Neutral;

        data.SetState(this);
    }

    // 평판에 따른 손님 감정 변화
    public CustomerEmotion UpdateEmotion(ReputationResult result)
    {
        currentEmotion = BaseData.GetEmotion(result);
        return currentEmotion;
    }
}
