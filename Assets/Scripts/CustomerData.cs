using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Customer", menuName = "Objects/Customer")]
public abstract class CustomerData : ScriptableObject
{

    [SerializeField] private string customerName;   // 손님 이름
    public string CustomerName => customerName;

    [SerializeField] private List<IngredientData> recipe;   // 주문한 메뉴
    public IReadOnlyList<IngredientData> Recipe => recipe.AsReadOnly();

    // 평판 결과에 따른 손님 표정
    public virtual CustomerEmotion GetEmotion(ReputationResult result)
    {
        switch (result)
        {
            case ReputationResult.Perfect: return CustomerEmotion.Happy;
            case ReputationResult.Incomplete: return CustomerEmotion.Neutral;
            default: return CustomerEmotion.Angry;
        }
    }
    public virtual int GetBonusScore(ReputationResult result) => 0;

    public abstract string GetDialogue();

    public abstract bool GetReputationDialogue(ReputationResult result, out string dialogue);

    public virtual void SetState(CustomerRuntimeState state) { }
}
