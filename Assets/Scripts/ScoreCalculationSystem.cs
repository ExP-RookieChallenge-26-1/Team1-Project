using UnityEngine;

/// <summary>
/// 점수 누적. 손님 한 명의 점수 계산 규칙은 ScoreRules
/// (더미 재료 개수 × 100 × 요구사항 충족 2배 / 미충족 0.5배).
/// </summary>
public class ScoreCalculationSystem : MonoBehaviour
{

<<<<<<< Updated upstream
    private int currentReputation = 0;  // ���� ���� ����
    public int CurrentReputation => currentReputation;
    public int oldReputation;
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


    public void AddReputation(ReputationResult result, int bonus = 0)
    {
        switch (result)
        {
            // ������ Perfect��� perfectScore��ŭ ���� ����
            case ReputationResult.Perfect:
                currentReputation += perfectScore;
                break;
            // ������ Incomplete��� incompleteScore��ŭ ���� ����
            case ReputationResult.Incomplete:
                currentReputation += incompleteScore;
                break;
            // ������ Wrong�̶�� wrongScore��ŭ ���� ����(�ּ� ���� 0��)
            case ReputationResult.Wrong:
                currentReputation = Mathf.Max(0, currentReputation + wrongScore);
                break;
        }
        
        // Ư�� �մ��� �ִ� ���ʽ� ���� �ջ�
        currentReputation += bonus;
        oldReputation = currentReputation;
        // ���� ���� ���� ���
        GameEvents.TriggerReputationChanged(currentReputation);
    }

    public void SetReputation(int savedScore)
    {
        currentReputation = savedScore;
        oldReputation = savedScore;
        GameEvents.TriggerReputationChanged(currentReputation);
=======
    public int customerScore { get; private set; } = 0;   // 마지막 손님에게 받은 점수
    public int stageScore { get; private set; } = 0;      // 이번 스테이지 누적
    public int totalScore = 0;                             // 전체 누적

    public void AddScore(int score)
    {
        customerScore = score;
        stageScore += score;
        totalScore += score;
    }

    public void ResetStageScore()
    {
        stageScore = 0;
    }
    public void SetLoadedScore(int savedTotal, int savedStageScore)
    {
        totalScore = savedTotal;
        stageScore = savedStageScore;
>>>>>>> Stashed changes
    }
}
