using UnityEngine;
using UnityEngine.Analytics;

[CreateAssetMenu(fileName = "DefaultCustomer", menuName = "Objects/Customer/Default")]
public class DefaultCustomerData : CustomerData
{
    private ICustomerAppearanceGenerator apperanceGenerator = new RandomAppearanceGenerator();
    
    // 기본 손님 state
    public override void SetState(CustomerRuntimeState state)
    {
        state.Appearance = apperanceGenerator.Generate();
    }

    [TextArea] public string[] randomDialogues;
    [TextArea] public string[] perfectDialogues;
    [TextArea] public string[] incompleteDialogues;
    [TextArea] public string[] wrongDialogues;

    // 기본 손님 대사
    public override string GetDialogue()
    {
        if (randomDialogues != null && randomDialogues.Length > 0)
            return randomDialogues[Random.Range(0, randomDialogues.Length)];
        return "버거 주세요!";
    }

    // 평판에 따른 손님 대사
    public override bool GetReputationDialogue(ReputationResult result, out string resultDialogue)
    {
        switch (result)
        {
            case ReputationResult.Perfect:
                //return "기쁨";
                return perfectDialogues.GetRandomTxt(out resultDialogue,"기쁨");
            case ReputationResult.Wrong:
                //return "실망";
                return wrongDialogues.GetRandomTxt(out resultDialogue,"실망");
            default:
                //return "기본";
                return incompleteDialogues.GetRandomTxt(out resultDialogue,"기본");
        }
    }
}
