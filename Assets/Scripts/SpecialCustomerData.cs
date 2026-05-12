using UnityEngine;

[CreateAssetMenu(fileName = "SpecialCustomer", menuName = "Objects/Customer/Special")]

// 특수 손님 state
public class SpecialCustomerData : CustomerData
{
    [Header("Special Customer State")]
    public CustomerGender fixedGender;
    public CustomerSkin fixedSkin;
    public CustomerClothes fixedClothes;
    public CustomerHair fixedHair;
    [TextArea]
    [SerializeField] private string fixedDialogue = "";

    public override void SetState(CustomerRuntimeState state)
    {
        state.Gender = fixedGender;
        state.Skin = fixedSkin;
        state.Clothes = fixedClothes;
        state.Hair = fixedHair;
    }

    // 특수 손님 대사
    public override string GetDialogue()
    {
        return fixedDialogue;
    }

    // 특수 손님 보너스 점수
    public override int GetBonusScore(ReputationResult result)
    {
        // 평판이 Perfect 혹은 Incomplete라면 bonus 15점
        if (result == ReputationResult.Perfect || result == ReputationResult.Incomplete) return 15;
        else return 0;
    }
}
