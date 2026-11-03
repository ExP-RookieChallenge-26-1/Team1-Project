using UnityEngine;
using System;

public static class GameEvents
{
    // 새 손님이 등장하면 호출
    public static event Action<CustomerRuntimeState> OnNewCustomerAppeared;

    // 스테이지가 바뀌면 호출
    public static event Action<int> OnStageChanged;

    public static void TriggerNewCustomerAppeared(CustomerRuntimeState customerState)
    {
        OnNewCustomerAppeared?.Invoke(customerState);
    }

    public static void TriggerStageChanged(int stageLevel)
    {
        OnStageChanged?.Invoke(stageLevel);
    }
}
