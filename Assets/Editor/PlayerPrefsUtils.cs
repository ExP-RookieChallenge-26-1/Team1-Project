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
    
    [MenuItem("Tools/Go To Stage 6")]
    public static void GoToStage6()
    {
        PlayerPrefs.SetInt("SavedStage", 5);
        PlayerPrefs.SetInt("SavedServedCount", 0);
        PlayerPrefs.SetInt("SavedReputation", 0);
        PlayerPrefs.Save();

        Debug.Log("Moved to Stage 6!");
    }

    [MenuItem("Tools/Stage 5/Set Timer To 5 Seconds")]
    public static void SetStage5TimerTo5Seconds()
    {
        if (Stage5Mode.Inst == null)
        {
            Debug.LogWarning("Stage5Mode is not active.");
            return;
        }

        Stage5Mode.Inst.SetTimeForTest(5f);
        Debug.Log("Stage 5 timer set to 5 seconds.");
    }

    [MenuItem("Tools/Go To Final")]
    public static void GoTo5Stage()
    {

        PlayerPrefs.SetInt("SavedStage", 5);
        PlayerPrefs.SetInt("SavedServedCount", 2);
        PlayerPrefs.Save();

        Debug.Log("Go to final!");
    }
    
    [MenuItem("Tools/Go To Final-1000SCore")]
    public static void GoTo5Stage100Score()
    {

        PlayerPrefs.SetInt("SavedStage", 5);
        PlayerPrefs.SetInt("SavedServedCount", 2);
        PlayerPrefs.SetInt("SavedReputation", 1000);
        PlayerPrefs.Save();

        Debug.Log("Go to final!");
    }
}


    
   

