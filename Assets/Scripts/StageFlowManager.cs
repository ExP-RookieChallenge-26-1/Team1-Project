using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StageFlowManager : MonoBehaviour
{
    [SerializeField] private List<StageData> stages;
    public IReadOnlyList<StageData> Stages => stages.AsReadOnly();

    private CustomerQueueManager customerQueueManager;
    private ScoreCalculationSystem scoreCalculationSystem;
    private IOrderEvaluator evaluator;
    private int currentStageIndex = 0;  // 현재 스테이지 번호
    private int servedCount = 0;    // 음식을 제작한 횟수
    public int currentStageIndex { get; private set; } = 0;  // 현재 스테이지 번호
    public int servedCount { get; private set; } = 0;    // 음식을 제작한 횟수

    private void Awake()
    {
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
        // index가 스테이지 수보다 많다면 종료
        if (index >= stages.Count)
        {
            GameEvents.TriggerAllStagesCleared();
            return;
        }

        var remainingCustomers = stages[index].CustomerPool.Skip(servedCount).ToList();
        customerQueueManager.PrepareQueue(remainingCustomers);
        GameEvents.TriggerStageChanged(stages[index].StageLevel);

        // 첫 번째 손님 호출
        customerQueueManager.GetNextCustomer();
    }

    public void OnBurgerSubmitted(IReadOnlyList<IngredientData> playerBurger)
    {
        // 현재 고객 정보 가져오기
        CustomerData currentCustomer = customerQueueManager.GetCurrentCustomer();
        if (currentCustomer == null) return;

        // 평가 결과 가져오기
        ReputationResult result = evaluator.Evaluate(currentCustomer.Recipe, playerBurger);

        // 점수 계산
        int bonus = currentCustomer.GetBonusScore(result);
        scoreCalculationSystem.AddReputation(result, bonus);

        CustomerRuntimeState currentState = customerQueueManager.GetCurrentCustomerState();
        if (currentState != null)
        {
            currentState.UpdateEmotion(result);
        }

        servedCount++;

        SaveDataManager.SaveProgress(currentStageIndex, servedCount, scoreCalculationSystem.CurrentReputation);

        CheckStageProgress();
    }
    private void CheckStageProgress()
    {
        // 해당 스테이지에서 손님을 모두 받았다면 다음 스테이지로 넘어가기
        if (servedCount >= stages[currentStageIndex].TargetClearCount)
        {
            currentStageIndex++;
            LoadStage(currentStageIndex);
            AdvanceToNextStage();
        }
        else
        {
            CustomerData nextCustomer = customerQueueManager.GetNextCustomer();
            // 손님이 부족한 경우 예외 처리로 다음 스테이지로 넘어가기
            if (nextCustomer == null)
            {
                AdvanceToNextStage();
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
