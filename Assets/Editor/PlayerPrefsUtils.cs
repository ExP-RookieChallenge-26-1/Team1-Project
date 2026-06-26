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
    
    [MenuItem("Tools/Go To 5Stage")]
    public static void GoTo5Stage()
    {
        
        PlayerPrefs.SetInt("SavedStage", 4);
        PlayerPrefs.SetInt("SavedServedCount", 0);
        PlayerPrefs.Save();

        Debug.Log("Go to 5 stage!");
    }
}