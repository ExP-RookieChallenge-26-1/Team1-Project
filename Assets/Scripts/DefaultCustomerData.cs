using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "DefaultCustomer", menuName = "Objects/Customer/Default")]
public class DefaultCustomerData : CustomerData
{
    private ICustomerAppearanceGenerator apperanceGenerator = new RandomAppearanceGenerator();

    // 기본 손님 state
    public override void SetState(CustomerRuntimeState state)
    {
        state.Gender = (CustomerGender)Random.Range(0, 2);
        state.Skin = (CustomerSkin)Random.Range(0, 4);
        state.Clothes = (CustomerClothes)Random.Range(0, 3);
        state.Hair = (CustomerHair)Random.Range(0, 3);
    }

    [TextArea] public string[] randomDialogues;
    // FormerlySerializedAs: 기존 에셋의 perfect/wrong 대사를 그대로 가져온다
    [FormerlySerializedAs("perfectDialogues")]
    [TextArea] public string[] satisfiedDialogues;
    [FormerlySerializedAs("wrongDialogues")]
    [TextArea] public string[] unsatisfiedDialogues;

    // 기본 손님 대사
    public override string GetDialogue()
    {
        if (randomDialogues != null && randomDialogues.Length > 0)
            return randomDialogues[Random.Range(0, randomDialogues.Length)];
        return "버거 주세요!";
    }

    // 요구사항 충족 여부에 따른 손님 대사
    public override bool GetResultDialogue(OrderResult result, out string resultDialogue)
    {
        return result == OrderResult.Satisfied
            ? satisfiedDialogues.GetRandomTxt(out resultDialogue, "기쁨")
            : unsatisfiedDialogues.GetRandomTxt(out resultDialogue, "화남");
    }
}
