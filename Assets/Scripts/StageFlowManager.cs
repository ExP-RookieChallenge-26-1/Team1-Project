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
    public int currentStageIndex { get; private set; } = 0;  // 현재 스테이지 번호
    public int servedCount { get; private set; } = 0;    // 이번 스테이지에서 응대한 손님 수
    public CustomerEmotion oldEmotion;

    private void Awake()
    {
        Inst = this;
        customerQueueManager = GetComponent<CustomerQueueManager>();
        scoreCalculationSystem = GetComponent<ScoreCalculationSystem>();
    }

    private void Start()
    {
        SaveDataManager.LoadProgress(out int savedStage, out int savedServed, out int savedScore); ;
        currentStageIndex = savedStage;
        servedCount = savedServed;
        scoreCalculationSystem.SetLoadedScore(savedTotalScore, savedStageScore);

        LoadStage(currentStageIndex);
        AdvancedMain.Inst.StartFlow();
    }

    private void LoadStage(int index)
    {
        // 남은 스테이지가 없다면 게임 클리어
        if (index >= stages.Count)
        {
            isAllGameCleared = true;
            return;
        }

        var remainingCustomers = stages[index].CustomerPool.Skip(servedCount).ToList();
        customerQueueManager.PrepareQueue(remainingCustomers);
        GameEvents.TriggerStageChanged(stages[index].StageLevel);

        // 첫 번째 손님 호출
        customerQueueManager.GetNextCustomer();
    }

    /// <summary>
    /// 버거 제출 처리: 요구사항 판정 → 점수(더미 개수 × 100 × 2 또는 0.5) → 손님 감정 → 다음 손님.
    /// </summary>
    public OrderResult OnBurgerSubmitted(IReadOnlyList<Ingredient> playerBurger, out int score)
    {
        CustomerData currentCustomer = customerQueueManager.GetCurrentCustomer();
        if (currentCustomer == null)
        {
            score = 0;
            return OrderResult.Unsatisfied;
        }

        OrderResult result = ScoreRules.Evaluate(currentCustomer.Requirement, playerBurger);
        score = ScoreRules.Calculate(playerBurger, result);
        Debug.Log($"[제출] {currentCustomer.CustomerName} | 요구사항: {currentCustomer.Requirement?.Describe()} | " +
                  $"재료 {playerBurger.Count}개 → {result}, {score}점");
        scoreCalculationSystem.AddScore(score);

        CustomerRuntimeState currentState = customerQueueManager.GetCurrentCustomerState();
        if (currentState != null)
        {
            oldEmotion = currentState.UpdateEmotion(result);
        }

        servedCount++;

        SaveDataManager.SaveProgress(currentStageIndex, servedCount, scoreCalculationSystem.totalScore, scoreCalculationSystem.stageScore);

        CheckStageProgress();
        return result;
    }

    // 5스테이지(PD) 전용: 제한시간 동안 여러 번 제출한 결과를 한 번에 반영
    public void FinishStage5(OrderResult result, int score)
    {
        CustomerRuntimeState currentState = customerQueueManager.GetCurrentCustomerState();
        if (currentState != null)
        {
            oldEmotion = currentState.UpdateEmotion(result);
        }

        scoreCalculationSystem.AddScore(score);
        servedCount = stages[currentStageIndex].TargetClearCount;

        SaveDataManager.SaveProgress(currentStageIndex, servedCount, scoreCalculationSystem.totalScore, scoreCalculationSystem.stageScore);
        CheckStageProgress();
    }

    private void CheckStageProgress()
    {
        {
            // 이번 스테이지의 목표 손님 수를 채웠다면 다음 스테이지로
            if (servedCount >= stages[currentStageIndex].TargetClearCount)
            {
                LoadStage(currentStageIndex);
                AdvanceToNextStage();
            }
            else
            {
                CustomerData nextCustomer = customerQueueManager.GetNextCustomer();
                // 남은 손님이 없다면 다음 스테이지로
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