using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum ReputationResult { Perfect, Incomplete, Wrong}  // 평판 종류

public interface IOrderEvaluator
{
    ReputationResult Evaluate(IReadOnlyList<IngredientData> goal, IReadOnlyList<IngredientData> submitted);
}
public class RecipeChecker : IOrderEvaluator
{
    public ReputationResult Evaluate(IReadOnlyList<IngredientData> goal, IReadOnlyList<IngredientData> submitted)
    {
        // 순서가 모두 같다면 Perfect
        if (goal.SequenceEqual(submitted)) return ReputationResult.Perfect;

        // 주문메뉴의 재료 확인 및 정렬
        var goalIDs = goal.Select(x => x.IngredientType).OrderBy(id => id);
        // 제작메뉴의 재료 확인 및 정렬
        var submittedIDs = submitted.Select(x => x.IngredientType).OrderBy(id => id);

        // 재료가 모두 같다면 Incomplete
        if (goalIDs.SequenceEqual(submittedIDs)) return ReputationResult.Incomplete;

        // 그 외는 Wrong
        return ReputationResult.Wrong;
    }
}