using UnityEngine;

public class ScoreCalculationSystem : MonoBehaviour
{

    private int currentReputation = 0;  // ���� ���� ����
    public int CurrentReputation => currentReputation;

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

        // ���� ���� ���� ���
        GameEvents.TriggerReputationChanged(currentReputation);
    }

    public void SetReputation(int savedScore)
    {
        currentReputation = savedScore;
        GameEvents.TriggerReputationChanged(currentReputation);
    }
}
