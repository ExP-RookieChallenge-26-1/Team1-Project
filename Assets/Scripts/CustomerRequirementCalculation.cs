using System.Collections.Generic;

/// <summary>
/// 요구사항 종류. 손님 데이터에는 숫자로 저장되므로 새 종류는 항상 맨 아래에 추가한다.
/// </summary>
public enum RequirementType
{
    Exclude,        // 특정 재료를 넣지 않기 (예: 양파 빼주세요)
    AtLeastCount,   // 특정 재료를 count개 이상
    AtMostCount,    // 특정 재료를 count개 이하
    AtLeastRatio,   // 특정 재료가 전체의 ratio(0~1) 이상
    AtMostRatio,    // 특정 재료가 전체의 ratio(0~1) 이하
    Only,           // 특정 재료만 넣기 (예: 고기만 넣어주세요)
}

/// <summary>
/// 요구사항 충족 판정과 설명 문구를 한곳에서 관리한다 (Unity 비의존).
/// 손님마다 함수를 만들지 않고, 요구사항 "종류"마다 함수 하나를 두고 재료/개수/비율 값만 손님 데이터에서 바꿔 쓴다.
/// 새 요구사항 추가: RequirementType에 항목 추가 → 판정 함수 작성 → IsSatisfied, Describe, TypeLabel, UsesCount/UsesRatio에 연결.
/// </summary>
public static class CustomerRequirementCalculation
{
    /// <summary>에디터(손님 데이터 Inspector)의 판정 종류 목록에 보일 이름.</summary>
    public static string TypeLabel(RequirementType type)
    {
        switch (type)
        {
            case RequirementType.Exclude:      return "재료 빼기";
            case RequirementType.AtLeastCount: return "재료 n개 이상";
            case RequirementType.AtMostCount:  return "재료 n개 이하";
            case RequirementType.AtLeastRatio: return "재료 비율 이상";
            case RequirementType.AtMostRatio:  return "재료 비율 이하";
            case RequirementType.Only:         return "재료만 넣기";
            default:                           return type.ToString();
        }
    }

    /// <summary>이 판정이 개수(count) 값을 쓰는지. Inspector에서 쓰는 칸만 보여주는 데 사용.</summary>
    public static bool UsesCount(RequirementType type) =>
        type == RequirementType.AtLeastCount || type == RequirementType.AtMostCount;

    /// <summary>이 판정이 비율(ratio) 값을 쓰는지.</summary>
    public static bool UsesRatio(RequirementType type) =>
        type == RequirementType.AtLeastRatio || type == RequirementType.AtMostRatio;

    /// <summary>제출한 버거가 요구사항을 만족하는지. 빈 버거는 항상 불만족.</summary>
    public static bool IsSatisfied(CustomerRequirement requirement, IReadOnlyList<Ingredient> burger)
    {
        if (burger == null || burger.Count == 0) return false;

        var ingredient = requirement.ingredient;
        switch (requirement.type)
        {
            case RequirementType.Exclude:      return Exclude(burger, ingredient);
            case RequirementType.AtLeastCount: return AtLeastCount(burger, ingredient, requirement.count);
            case RequirementType.AtMostCount:  return AtMostCount(burger, ingredient, requirement.count);
            case RequirementType.AtLeastRatio: return AtLeastRatio(burger, ingredient, requirement.ratio);
            case RequirementType.AtMostRatio:  return AtMostRatio(burger, ingredient, requirement.ratio);
            case RequirementType.Only:         return Only(burger, ingredient);
            default:                           return false;
        }
    }

    /// <summary>디버그/대사 대체용 설명.</summary>
    public static string Describe(CustomerRequirement requirement)
    {
        var ingredient = requirement.ingredient;
        switch (requirement.type)
        {
            case RequirementType.Exclude:      return $"{ingredient} 빼기";
            case RequirementType.AtLeastCount: return $"{ingredient} {requirement.count}개 이상";
            case RequirementType.AtMostCount:  return $"{ingredient} {requirement.count}개 이하";
            case RequirementType.AtLeastRatio: return $"{ingredient} {requirement.ratio:P0} 이상";
            case RequirementType.AtMostRatio:  return $"{ingredient} {requirement.ratio:P0} 이하";
            case RequirementType.Only:         return $"{ingredient}만 넣기";
            default:                           return requirement.type.ToString();
        }
    }

    // ---- 요구사항 종류별 판정 ----

    static bool Exclude(IReadOnlyList<Ingredient> burger, Ingredient ingredient) => CountOf(burger, ingredient) == 0;

    static bool AtLeastCount(IReadOnlyList<Ingredient> burger, Ingredient ingredient, int count) => CountOf(burger, ingredient) >= count;

    static bool AtMostCount(IReadOnlyList<Ingredient> burger, Ingredient ingredient, int count) => CountOf(burger, ingredient) <= count;

    static bool AtLeastRatio(IReadOnlyList<Ingredient> burger, Ingredient ingredient, float ratio) => ShareOf(burger, ingredient) >= ratio;

    static bool AtMostRatio(IReadOnlyList<Ingredient> burger, Ingredient ingredient, float ratio) => ShareOf(burger, ingredient) <= ratio;

    static bool Only(IReadOnlyList<Ingredient> burger, Ingredient ingredient) => CountOf(burger, ingredient) == burger.Count;

    // ---- 공용 계산 ----

    static int CountOf(IReadOnlyList<Ingredient> burger, Ingredient ingredient)
    {
        int matched = 0;
        foreach (var i in burger)
            if (i == ingredient) matched++;
        return matched;
    }

    static float ShareOf(IReadOnlyList<Ingredient> burger, Ingredient ingredient) => (float)CountOf(burger, ingredient) / burger.Count;
}
