using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Stage", menuName = "Objects/Stage")]
public class StageData : ScriptableObject
{
    [SerializeField] private int stageLevel;    // 현재 스테이지
    public int StageLevel => stageLevel;
    [SerializeField] private List<CustomerData> customerPool;   // 손님 명단
    public IReadOnlyList<CustomerData> CustomerPool => customerPool.AsReadOnly();
    [SerializeField] private int targetClearCount;  // 목표 손님 수
    public int TargetClearCount => targetClearCount;

}
