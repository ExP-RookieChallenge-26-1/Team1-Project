using UnityEngine;

public class ScoreCalculationSystem : MonoBehaviour
{
    public int currentReputation = 0;  // 현재 평판 점수
    [SerializeField] private int perfectScore = 30;
    [SerializeField] private int incompleteScore = 15;
    [SerializeField] private int wrongScore = -10;

    private void OnEnable()
    {
        GameEvents.OnStageChanged += ResetReputation;
    }

    private void OnDisable()
    {
        GameEvents.OnStageChanged -= ResetReputation;
    }

    private void ResetReputation(int stageLevel)
    {
        currentReputation = 0;
        GameEvents.TriggerReputationChanged(currentReputation);
    }

    public void AddReputation(ReputationResult result)
    {
        switch (result)
        {
            // 평판이 Perfect라면 perfectScore만큼 점수 증가
            case ReputationResult.Perfect:
                currentReputation += perfectScore;
                break;
            // 평판이 Incomplete라면 incompleteScore만큼 점수 증가
            case ReputationResult.Incomplete:
                currentReputation += incompleteScore;
                break;
            // 평판이 Wrong이라면 wrongScore만큼 점수 증가(최소 점수 0점)
            case ReputationResult.Wrong:
                currentReputation = Mathf.Max(0, currentReputation + wrongScore);
                break;
        }

        // 현재 평판 점수 방송
        GameEvents.TriggerReputationChanged(currentReputation);
    }
}
