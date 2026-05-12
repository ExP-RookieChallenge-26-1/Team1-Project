using UnityEngine;
using System.Collections.Generic;

public class CustomerQueueManager : MonoBehaviour
{
    private Queue<CustomerData> customerQueue = new Queue<CustomerData>();
    private CustomerData currentCustomerData;
    private CustomerRuntimeState currentCustomerState;

    public void PrepareQueue(IReadOnlyList<CustomerData> customerPool)
    {
        // 손님 정보를 Queue에 삽입
        customerQueue = new Queue<CustomerData>(customerPool);
    }

    public CustomerData GetNextCustomer()
    {
        if (customerQueue.Count > 0)
        {
            currentCustomerData = customerQueue.Dequeue();
            currentCustomerState = new CustomerRuntimeState(currentCustomerData);
            GameEvents.TriggerNewCustomerAppeared(currentCustomerState);
            // 현재 손님 정보 반환
            return currentCustomerData;
        }

        // 대기열에 손님이 없을 경우 null 반환
        currentCustomerData = null;
        currentCustomerState = null;
        return null;
    }

    public CustomerData GetCurrentCustomer() => currentCustomerData;    // 현재 손님 정보 확인
    public CustomerRuntimeState GetCurrentCustomerState() => currentCustomerState;  // 현재 손님 상태 확인
}
