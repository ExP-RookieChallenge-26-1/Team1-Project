using UnityEngine;

public class SaveDataManager
{
    // 진행도 저장
    public static void SaveProgress(int stageIndex, int servedCount, int reputation)
    {
        PlayerPrefs.SetInt("SavedStage", stageIndex);
        PlayerPrefs.SetInt("SavedServedCount", servedCount);
        PlayerPrefs.SetInt("SavedReputation", reputation);
        PlayerPrefs.Save();
    }

    // 진행도 불러오기
    public static void LoadProgress(out int stageIndex, out int servedCount, out int reputation)
    {
        stageIndex = PlayerPrefs.GetInt("SavedStage", 0);
        servedCount = PlayerPrefs.GetInt("SavedServedCount", 0);
        reputation = PlayerPrefs.GetInt("SavedReputation", 0);
    }
}
