using UnityEngine;

public class SaveDataManager
{
<<<<<<< Updated upstream
    // ÁøÇàµµ ÀúÀå
    public static void SaveProgress(int stageIndex, int servedCount, int reputation)
=======
    // ì§„í–‰ë„ ì €ìž¥
    public static void SaveProgress(int stageIndex, int servedCount, int totalReputation, int stageReputation)
>>>>>>> Stashed changes
    {
        PlayerPrefs.SetInt("SavedStage", stageIndex);
        PlayerPrefs.SetInt("SavedServedCount", servedCount);
        PlayerPrefs.SetInt("SavedReputation", reputation);
        PlayerPrefs.Save();
    }

<<<<<<< Updated upstream
    // ÁøÇàµµ ºÒ·¯¿À±â
    public static void LoadProgress(out int stageIndex, out int servedCount, out int reputation)
=======
    // ì§„í–‰ë„ ë¶ˆëŸ¬ì˜¤ê¸°
    public static void LoadProgress(out int stageIndex, out int servedCount, out int totalReputation, out int stageReputation)
>>>>>>> Stashed changes
    {
        stageIndex = PlayerPrefs.GetInt("SavedStage", 0);
        servedCount = PlayerPrefs.GetInt("SavedServedCount", 0);
        reputation = PlayerPrefs.GetInt("SavedReputation", 0);
    }
}
