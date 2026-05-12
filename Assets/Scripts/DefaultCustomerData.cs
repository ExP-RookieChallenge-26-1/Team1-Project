using UnityEngine;
using UnityEngine.Analytics;

[CreateAssetMenu(fileName = "DefaultCustomer", menuName = "Objects/Customer/Default")]
public class DefaultCustomerData : CustomerData
{
    // 기본 손님 state
    public override void SetState(CustomerRuntimeState state)
    {
        state.Gender = (CustomerGender)Random.Range(0, 2);
        state.Skin = (CustomerSkin)Random.Range(0, 4);
        state.Clothes = (CustomerClothes)Random.Range(0, 3);
        state.Hair = (CustomerHair)Random.Range(0, 3);
    }

    // 기본 손님 대사
    [TextArea] public string[] randomDialogues;
    public override string GetDialogue()
    {
        if (randomDialogues != null && randomDialogues.Length > 0)
            return randomDialogues[Random.Range(0, randomDialogues.Length)];
        return "버거 주세요!";
    }
}
