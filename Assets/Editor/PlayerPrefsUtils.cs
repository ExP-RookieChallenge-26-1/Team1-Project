using UnityEngine;
using UnityEditor;

public static class PlayerPrefsUtils
{
    [MenuItem("Tools/Clear PlayerPrefs")]
    public static void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        Debug.Log("PlayerPrefs cleared!");
    }

    [MenuItem("Tools/Go To Stage 5")]
    public static void GoToStage5()
    {
        PlayerPrefs.SetInt("SavedStage", 4);
        PlayerPrefs.SetInt("SavedServedCount", 0);
        PlayerPrefs.SetInt("SavedReputation", 0);
        PlayerPrefs.Save();

        Debug.Log("Moved to Stage 5!");
    }
}
