using UnityEngine;
using System.Collections.Generic;

public class CustomerQueueManager : MonoBehaviour
{
    private Queue<CustomerData> customerQueue = new Queue<CustomerData>();
    private CustomerData currentCustomer;

    public void PrepareQueue(IReadOnlyList<CustomerData> customerPool)
    {
        // 손님 정보를 Queue에 삽입
        customerQueue = new Queue<CustomerData>(customerPool);
    }

    public CustomerData GetNextCustomer()
    {
        if (customerQueue.Count > 0)
        {
            currentCustomer = customerQueue.Dequeue();
            GameEvents.TriggerNewCustomerAppeared(currentCustomer);
            // 현재 손님 정보 반환
            return currentCustomer;
        }

        // 대기열에 손님이 없을 경우 null 반환
        return null;
    }

    public CustomerData GetCurrentCustomer() => currentCustomer;    // 현재 손님 정보 확인
}
