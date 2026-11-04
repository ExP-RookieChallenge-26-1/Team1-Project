using UnityEngine;

/// <summary>
/// 점수 누적. 손님 한 명의 점수 계산 규칙은 ScoreRules
/// (더미 재료 개수 × 100 × 요구사항 충족 2배 / 미충족 0.5배).
/// </summary>
public class ScoreCalculationSystem : MonoBehaviour
{

    public int customerScore { get; private set; } = 0;   // 마지막 손님에게 받은 점수
    public int stageScore { get; private set; } = 0;      // 이번 스테이지 누적
    public int totalScore = 0;                             // 전체 누적

    public void AddScore(int score)
    {
        customerScore = score;
        stageScore += score;
        totalScore += score;
    }

    public void ResetStageScore()
    {
        stageScore = 0;
    }
    public void SetLoadedScore(int savedTotal, int savedStageScore)
    {
        totalScore = savedTotal;
        stageScore = savedStageScore;
    }
}
