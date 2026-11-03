using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "SpecialCustomer", menuName = "Objects/Customer/Special")]

// 특수 손님 state
public class SpecialCustomerData : CustomerData
{
    [TextArea]
    [SerializeField] private string fixedDialogue = "";
    // FormerlySerializedAs: 기존 에셋의 perfect/wrong 대사를 그대로 가져온다
    [FormerlySerializedAs("perfectDialogue")]
    [TextArea] public string[] satisfiedDialogue;
    [FormerlySerializedAs("wrongDialogue")]
    [TextArea] public string[] unsatisfiedDialogue;
    [Header("Emotion Sprites (Neutral, Happy, Angry)")]
    public Sprite[] emotionSprites;

    // 특수 손님 대사
    public override string GetDialogue()
    {
        return fixedDialogue;
    }

    // 요구사항 충족 여부에 따른 손님 대사
    public override bool GetResultDialogue(OrderResult result, out string resultDialogue)
    {
        return result == OrderResult.Satisfied
            ? satisfiedDialogue.GetRandomTxt(out resultDialogue, "기쁨")
            : unsatisfiedDialogue.GetRandomTxt(out resultDialogue, "화남");
    }
}
