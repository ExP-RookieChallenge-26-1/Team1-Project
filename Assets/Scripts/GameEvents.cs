using UnityEngine;
using System;

public static class GameEvents
{
    // 평판 점수가 바뀌면 방송
    public static event Action<int> OnReputationChanged;

    // 새 손님이 등장하면 방송
    public static event Action<CustomerData> OnNewCustomerAppeared;

    // 스테이지 변경 시 방송
    public static event Action<int> OnStageChanged;

    // 스테이지 전부 클리어 시 방송
    public static event Action OnAllStagesCleared;

    public static void TriggerReputationChanged(int currentReputation)
    {
        OnReputationChanged?.Invoke(currentReputation);
    }

    public static void TriggerNewCustomerAppeared(CustomerData customer)
    {
        OnNewCustomerAppeared?.Invoke(customer);
    }

    public static void TriggerStageChanged(int stageLevel)
    {
        OnStageChanged?.Invoke(stageLevel);
    }

    public static void TriggerAllStagesCleared()
    {
        OnAllStagesCleared?.Invoke();
    }
}