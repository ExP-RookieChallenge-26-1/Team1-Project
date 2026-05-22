using UnityEngine;
using System.Collections.Generic;

public class CustomerQueueManager : MonoBehaviour
{
    private Queue<CustomerData> customerQueue = new Queue<CustomerData>();
    private CustomerData currentCustomerData;
    private CustomerRuntimeState currentCustomerState;

    public void PrepareQueue(IReadOnlyList<CustomerData> customerPool)
    {
        // �մ� ������ Queue�� ����
        customerQueue = new Queue<CustomerData>(customerPool);
    }

    public CustomerData GetNextCustomer()
    {
        if (customerQueue.Count > 0)
        {

            currentCustomerData = customerQueue.Dequeue();
            currentCustomerState = new CustomerRuntimeState(currentCustomerData);
            GameEvents.TriggerNewCustomerAppeared(currentCustomerState);
            // ���� �մ� ���� ��ȯ
            return currentCustomerData;
        }

        // ��⿭�� �մ��� ���� ��� null ��ȯ
        currentCustomerData = null;
        currentCustomerState = null;
        return null;
    }

    public CustomerData GetCurrentCustomer() => currentCustomerData;    // ���� �մ� ���� Ȯ��
    public CustomerRuntimeState GetCurrentCustomerState() => currentCustomerState;  // ���� �մ� ���� Ȯ��
}
