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

    [MenuItem("Tools/Go To 5Stage")]
    public static void GoTo5Stage()
    {

        PlayerPrefs.SetInt("SavedStage", 4);
        PlayerPrefs.SetInt("SavedServedCount", 0);
        PlayerPrefs.Save();

        Debug.Log("Go to 5 stage!");
    }
}


    
   

