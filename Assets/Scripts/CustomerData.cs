using UnityEngine;

[CreateAssetMenu(fileName = "Customer", menuName = "Objects/Customer")]
public abstract class CustomerData : ScriptableObject
{

    [SerializeField] private string customerName;   // 손님 이름
    public string CustomerName => customerName;

    [Header("선호조건 (손님당 1개)")]
    [SerializeField] private CustomerRequirement requirement = new CustomerRequirement();
    public CustomerRequirement Requirement => requirement;

    [Tooltip("이 손님의 제한시간(초). 0 이하이면 AdvancedMain의 기본 제한시간을 사용")]
    [SerializeField] private float timeLimit = 0f;
    public float TimeLimit => timeLimit;

    // 요구사항 충족 여부에 따른 손님 표정: 충족 → 기쁨, 미충족 → 화남
    public virtual CustomerEmotion GetEmotion(OrderResult result)
    {
        return result == OrderResult.Satisfied ? CustomerEmotion.Happy : CustomerEmotion.Angry;
    }

    public abstract string GetDialogue();

    public abstract bool GetResultDialogue(OrderResult result, out string dialogue);

    public virtual void SetState(CustomerRuntimeState state) { }
}
