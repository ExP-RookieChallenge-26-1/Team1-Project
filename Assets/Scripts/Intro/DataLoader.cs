using System;
using UnityEngine;

public class DataLoader : MonoBehaviour
{
    public static float BGMValue;
    public static float SFXValue;

    public const float DEFAULT_BGM_VALUE = 1;
    public const float DEFAULT_SFX_VALUE = 0.7f;

    public static bool Loaded;
    
    private void Awake()
    {
        LoadData();
    }

    void LoadData()
    {
        if (Loaded)
            return;

        Loaded = true;
        
        BGMValue = PlayerPrefs.GetFloat("BGMValue", DEFAULT_BGM_VALUE);
        SFXValue = PlayerPrefs.GetFloat("SFXValue", DEFAULT_SFX_VALUE);
        
        Debug.Log($"BGMValue: {BGMValue} | SFXValue: {SFXValue}");
    }
    

    /// <summary>
    /// BGM 값 적용 및 PlayerPrefs 저장
    /// </summary>
    public static void SetBGMValue(float v)
    {
        BGMValue = v;
        PlayerPrefs.SetFloat("BGMValue", v);
        PlayerPrefs.Save();
        
        Debug.Log($"BGMValue: {BGMValue}");
    }
    
    /// <summary>
    /// SFX 값 적용 및 PlayerPrefs 저장
    /// </summary>
    public static void SetSFXValue(float v)
    {
        SFXValue = v;
        PlayerPrefs.SetFloat("SFXValue", v);
        PlayerPrefs.Save();
        
        Debug.Log($"SFXValue: {SFXValue}");
    }
}
