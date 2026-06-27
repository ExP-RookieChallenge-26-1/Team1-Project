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
    [MenuItem("Tools/Go To Stage 4")]
    public static void GoToStage4()
    {
        PlayerPrefs.SetInt("SavedStage", 3);
        PlayerPrefs.SetInt("SavedServedCount", 0);
        PlayerPrefs.SetInt("SavedReputation", 0);
        PlayerPrefs.Save();

        Debug.Log("Moved to Stage 4!");
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
        PlayerPrefs.SetInt("SavedTotalReputation", 1000);
        PlayerPrefs.Save();

        Debug.Log("Go to final!");
    }
    [MenuItem("Tools/set score-600SCore")]
    public static void scoreset_600()
    {

        PlayerPrefs.SetInt("SavedTotalReputation", 600);
        PlayerPrefs.Save();

    }
    [MenuItem("Tools/set score-450SCore")]
    public static void scoreset_450()
    {

        PlayerPrefs.SetInt("SavedTotalReputation", 450);
        PlayerPrefs.Save();

    }
    [MenuItem("Tools/set score-300SCore")]
    public static void scoreset_300()
    {

        PlayerPrefs.SetInt("SavedTotalReputation", 300);
        PlayerPrefs.Save();

        Debug.Log("Go to final!");
    }
    [MenuItem("Tools/set score-0SCore")]
    public static void scoreset_0()
    {

        PlayerPrefs.SetInt("SavedTotalReputation", 0);
        PlayerPrefs.Save();

        Debug.Log("Go to final!");
    }
}


    
   

