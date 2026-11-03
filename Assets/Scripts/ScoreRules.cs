using System.Collections.Generic;

/// <summary>손님 요구사항 충족 여부. 충족 → 기쁨, 미충족 → 화남.</summary>
public enum OrderResult
{
    Satisfied,
    Unsatisfied,
}

/// <summary>
/// 제출 점수 규칙 (Unity 비의존).
/// 점수 = 제출한 더미의 재료 점수 합 (구운 패티 150, 얼린 패티 0, 나머지 100) × (요구사항 충족 2배 / 미충족 0.5배)
/// </summary>
public static class ScoreRules
{
    public const int PointsPerIngredient = 100;
    /// <summary>구운 패티 한 장 점수.</summary>
    public const int PointsPerPatty = 150;
    /// <summary>얼린 패티는 합칠 수 없으므로 점수가 없다.</summary>
    public const int PointsPerRawPatty = 0;
    public const float SatisfiedMultiplier = 2f;
    public const float UnsatisfiedMultiplier = 0.5f;

    public static OrderResult Evaluate(CustomerRequirement requirement, IReadOnlyList<Ingredient> stack)
    {
        // 요구사항이 없는 손님은 무엇을 내든 충족으로 본다 (빈 더미 제외)
        bool satisfied = requirement == null
            ? stack != null && stack.Count > 0
            : requirement.IsSatisfiedBy(stack);
        return satisfied ? OrderResult.Satisfied : OrderResult.Unsatisfied;
    }

    public static int PointsFor(Ingredient ingredient)
    {
        switch (ingredient)
        {
            case Ingredient.RawPatty: return PointsPerRawPatty;
            case Ingredient.CookedPatty: return PointsPerPatty;
            default: return PointsPerIngredient;
        }
    }

    /// <summary>배율 적용 전 기본 점수: 재료별 점수의 합.</summary>
    public static int BaseScore(IReadOnlyList<Ingredient> stack)
    {
        int sum = 0;
        if (stack != null)
            foreach (var ingredient in stack) sum += PointsFor(ingredient);
        return sum;
    }

    public static int Calculate(IReadOnlyList<Ingredient> stack, OrderResult result)
    {
        float multiplier = result == OrderResult.Satisfied ? SatisfiedMultiplier : UnsatisfiedMultiplier;
        return (int)System.Math.Round(BaseScore(stack) * multiplier);
    }
}
