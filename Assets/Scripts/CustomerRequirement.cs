using System;
using System.Collections.Generic;

/// <summary>
/// 손님의 선호조건(요구사항) 값. 손님 한 명당 하나.
/// 판정 종류/재료/개수/비율/대사는 기획에 맞춰 손님 데이터(ScriptableObject)에서 설정한다.
/// 판정과 설명 문구는 CustomerRequirementCalculation에서 관리한다.
/// </summary>
[Serializable]
public class CustomerRequirement
{
    public RequirementType type = RequirementType.Exclude;
    public Ingredient ingredient = Ingredient.Onion;
    public int count = 1;
    public float ratio = 0.5f;
    /// <summary>손님이 요구사항을 말하는 대사 (예: "양파는 빼주세요!"). 줄바꿈마다 대사 한 번.</summary>
    public string dialogue = "";

    /// <summary>제출한 버거가 조건을 만족하는지. 빈 버거는 항상 불만족.</summary>
    public bool IsSatisfiedBy(IReadOnlyList<Ingredient> stack) => CustomerRequirementCalculation.IsSatisfied(this, stack);

    /// <summary>디버그/UI용 설명.</summary>
    public string Describe() => CustomerRequirementCalculation.Describe(this);
}
