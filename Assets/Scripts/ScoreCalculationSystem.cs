using UnityEngine;

public class ScoreCalculationSystem : MonoBehaviour
{

    public int customerReputation { get; private set; } = 0;
    public int stageReputation { get; private set; } = 0;
    public int totalReputation = 0;

    [SerializeField] private int perfectScore = 30;
    [SerializeField] private int incompleteScore = 15;
    [SerializeField] private int wrongScore = -10;

    public void AddReputation(ReputationResult result, int bonus = 0)
    {
        customerReputation = 0;

         switch (result)
        {
            // ������ Perfect��� perfectScore��ŭ ���� ����
            case ReputationResult.Perfect:
                Debug.Log("완벽! ");
                customerReputation += perfectScore;
                break;
            // ������ Incomplete��� incompleteScore��ŭ ���� ����
            case ReputationResult.Incomplete:
                Debug.Log("기본! ");
                customerReputation += incompleteScore;
                break;
            // ������ Wrong�̶�� wrongScore��ŭ ���� ����(�ּ� ���� 0��)
            case ReputationResult.Wrong:
                Debug.Log("실패! ");
                customerReputation = wrongScore;
                break;
        }
        
        // Ư�� �մ��� �ִ� ���ʽ� ���� �ջ�
        customerReputation += bonus;
        stageReputation = Mathf.Max(0, stageReputation + customerReputation);
        totalReputation = Mathf.Max(0, totalReputation + customerReputation);
        // ���� ���� ���� ���
        GameEvents.TriggerReputationChanged(totalReputation);
    }

    public void ResetStageReputation()
    {
        stageReputation = 0;
    }
    public void SetLoadedReputation(int savedTotal, int savedStageScore)
    {
        totalReputation = savedTotal;
        stageReputation = savedStageScore;

        GameEvents.TriggerReputationChanged(totalReputation);
    }
}
