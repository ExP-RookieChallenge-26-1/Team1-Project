using System.Collections.Generic;
using UnityEngine;

public class StageFlowManager : MonoBehaviour
{
    public static StageFlowManager Inst;
    
    [SerializeField] private List<StageData> stages;

    [HideInInspector] public CustomerQueueManager customerQueueManager;
    [HideInInspector] public  ScoreCalculationSystem scoreCalculationSystem;
    [HideInInspector] public IOrderEvaluator evaluator;

    public int currentStageIndex = 0;  // 현재 스테이지 번호
    public int servedCount = 0;    // 음식을 제작한 횟수

    private void Awake()
    {
        Inst = this;
        
        customerQueueManager = GetComponent<CustomerQueueManager>();
        scoreCalculationSystem = GetComponent<ScoreCalculationSystem>();
        evaluator = new RecipeChecker();
    }

    private void Start() => LoadStage(currentStageIndex);

    public void LoadStage(int index)
    {
        Debug.Log($"ASD: {index}");
        // index가 스테이지 수보다 많다면 종료
        if (index >= stages.Count)
        {
            GameEvents.TriggerAllStagesCleared();
            return;
        }

        servedCount = 0;
        customerQueueManager.PrepareQueue(stages[index].CustomerPool);

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
        scoreCalculationSystem.AddReputation(result);
        servedCount++;

        CheckStageProgress();
    }

    private void CheckStageProgress()
    {
        // 해당 스테이지에서 손님을 모두 받았다면 다음 스테이지로 넘어가기
        if (servedCount >= stages[currentStageIndex].TargetClearCount)
        {
            currentStageIndex++;
            Debug.Log($"새로운 스테이지 인덱스: {currentStageIndex}");
            LoadStage(currentStageIndex);
        }
        else
        {
            CustomerData nextCustomer = customerQueueManager.GetNextCustomer();
            // 손님이 부족한 경우 예외 처리로 다음 스테이지로 넘어가기
            if (nextCustomer == null)
            {
                currentStageIndex++;
                LoadStage(currentStageIndex);
            }
        }
    }
}
