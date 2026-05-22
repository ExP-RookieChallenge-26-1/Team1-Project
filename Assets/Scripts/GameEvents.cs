using UnityEngine;
using System;

public static class GameEvents
{
    // ���� ������ �ٲ�� ���
    public static event Action<int> OnReputationChanged;

    // �� �մ��� �����ϸ� ���
    public static event Action<CustomerRuntimeState> OnNewCustomerAppeared;

    // �������� ���� �� ���
    public static event Action<int> OnStageChanged;

    // �������� ���� Ŭ���� �� ���
    public static event Action OnAllStagesCleared;

    public static void TriggerReputationChanged(int currentReputation)
    {
        OnReputationChanged?.Invoke(currentReputation);
    }


    public static void TriggerNewCustomerAppeared(CustomerRuntimeState customerState)
    {
        OnNewCustomerAppeared?.Invoke(customerState);
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