using UnityEngine;

[CreateAssetMenu(fileName = "SpecialCustomer", menuName = "Objects/Customer/Special")]

// 특수 손님 state
public class SpecialCustomerData : CustomerData
{
    [TextArea]
    [SerializeField] private string fixedDialogue = "";
    [TextArea] public string[] perfectDialogue;
    [TextArea] public string[] incompleteDialogue;
    [TextArea] public string[] wrongDialogue;
    [Header("Special Sprites")]
    public Sprite[] specialSprites;

    [Header("Emotion Sprites (Happy, Neutral, Angry)")]
    public Sprite[] emotionSprites;

    // 특수 손님 대사
    public override string GetDialogue()
    {
        return fixedDialogue;
    }

    // 평판에 따른 손님 대사
    public override string GetReputationDialogue(ReputationResult result)
    {
        switch (result)
        {
            case ReputationResult.Perfect:
                return "기쁨";
            case ReputationResult.Wrong:
                return "실망";
            default:
                return "기본";
        }
    }

    // 특수 손님 보너스 점수
    public override int GetBonusScore(ReputationResult result)
    {
        // 평판이 Perfect 혹은 Incomplete라면 bonus 15점
        if (result == ReputationResult.Perfect || result == ReputationResult.Incomplete) return 15;
        else return 0;
    }
}
