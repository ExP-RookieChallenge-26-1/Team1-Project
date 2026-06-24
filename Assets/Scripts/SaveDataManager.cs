using UnityEngine;

public class SaveDataManager
{
    // 진행도 저장
    public static void SaveProgress(int stageIndex, int servedCount, int totalReputation, int stageReputation)
    {
        PlayerPrefs.SetInt("SavedStage", stageIndex);
        PlayerPrefs.SetInt("SavedServedCount", servedCount);
        PlayerPrefs.SetInt("SavedTotalReputation", totalReputation);
        PlayerPrefs.SetInt("SavedStageReputation", stageReputation);
        PlayerPrefs.Save();
    }

    // 진행도 불러오기
    public static void LoadProgress(out int stageIndex, out int servedCount, out int totalReputation, out int stageReputation)
    {
        stageIndex = PlayerPrefs.GetInt("SavedStage", 0);
        servedCount = PlayerPrefs.GetInt("SavedServedCount", 0);
        totalReputation = PlayerPrefs.GetInt("SavedTotalReputation", 0);
        stageReputation = PlayerPrefs.GetInt("SavedStageReputation", 0);
    }
}
