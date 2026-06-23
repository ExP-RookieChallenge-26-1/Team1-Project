using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StageFlowManager : MonoBehaviour
{
    public static StageFlowManager Inst;
    [SerializeField] private List<StageData> stages;
    public IReadOnlyList<StageData> Stages => stages.AsReadOnly();

    private CustomerQueueManager customerQueueManager;
    public CustomerQueueManager CustomerQueueManager => customerQueueManager;
    private ScoreCalculationSystem scoreCalculationSystem;
    public ScoreCalculationSystem ScoreCalculationSystem => scoreCalculationSystem;
    private IOrderEvaluator evaluator;
    public int currentStageIndex { get; private set; } = 0;  // ���� �������� ��ȣ
    public int servedCount { get; private set; } = 0;    // ������ ������ Ƚ��
    public CustomerEmotion oldEmotion;

    private void Awake()
    {
        Inst = this;
        customerQueueManager = GetComponent<CustomerQueueManager>();
        scoreCalculationSystem = GetComponent<ScoreCalculationSystem>();
        evaluator = new RecipeChecker();
    }

    private void Start()
    {
        SaveDataManager.LoadProgress(out int savedStage, out int savedServed, out int savedScore); ;
        currentStageIndex = savedStage;
        servedCount = savedServed;
        scoreCalculationSystem.SetReputation(savedScore);

        LoadStage(currentStageIndex);
    }

    private void LoadStage(int index)
    {
        // index�� �������� ������ ���ٸ� ����
        if (index >= stages.Count)
        {
            GameEvents.TriggerAllStagesCleared();
            return;
        }

        var remainingCustomers = stages[index].CustomerPool.Skip(servedCount).ToList();
        customerQueueManager.PrepareQueue(remainingCustomers);
        GameEvents.TriggerStageChanged(stages[index].StageLevel);

        // ù ��° �մ� ȣ��
        customerQueueManager.GetNextCustomer();
    }

    public ReputationResult OnBurgerSubmitted(IReadOnlyList<IngredientData> playerBurger)
    {
        // ���� ���� ���� ��������
        CustomerData currentCustomer = customerQueueManager.GetCurrentCustomer();
        if (currentCustomer == null) return ReputationResult.Incomplete;

        // �� ��� ��������
        ReputationResult result = evaluator.Evaluate(currentCustomer.Recipe, playerBurger);

        // ���� ���
        int bonus = currentCustomer.GetBonusScore(result);
        scoreCalculationSystem.AddReputation(result, bonus);

        CustomerRuntimeState currentState = customerQueueManager.GetCurrentCustomerState();
        if (currentState != null)
        {
            oldEmotion = currentState.UpdateEmotion(result);
        }

        servedCount++;

        SaveDataManager.SaveProgress(currentStageIndex, servedCount, scoreCalculationSystem.CurrentReputation);

        CheckStageProgress();
        return result;
    }
    private void CheckStageProgress()
    {
        {
            // �ش� ������������ �մ��� ��� �޾Ҵٸ� ���� ���������� �Ѿ��
            if (servedCount >= stages[currentStageIndex].TargetClearCount)
            {
                AdvanceToNextStage();
            }
            else
            {
                CustomerData nextCustomer = customerQueueManager.GetNextCustomer();
                // �մ��� ������ ��� ���� ó���� ���� ���������� �Ѿ��
                if (nextCustomer == null)
                {
                    AdvanceToNextStage();
                }
            }
        }
    }

    private void AdvanceToNextStage()
    {
        currentStageIndex++;
        servedCount = 0;

        LoadStage(currentStageIndex);
    }
}