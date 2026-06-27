using System;
using UnityEngine;
using UnityEngine.Audio;

public class DataLoader : MonoBehaviour
{
    public AudioMixer mixer;
    public static AudioMixer Mixer;
    
    public static float BGMValue;
    public static float SFXValue;

    public const float DEFAULT_BGM_VALUE = 1;
    public const float DEFAULT_SFX_VALUE = 0.7f;

    public static bool Loaded;
    
    private void Awake()
    {
        LoadData();
        Mixer = mixer;
    }

    private void Start()
    {
        //Test
        Screen.SetResolution(540, 1170, FullScreenMode.Windowed);
        Application.targetFrameRate = 120;
        ApplyValue();
    }

    void LoadData()
    {
        if (Loaded)
            return;

        Loaded = true;
        
        BGMValue = PlayerPrefs.GetFloat("BGMValue", DEFAULT_BGM_VALUE);
        SFXValue = PlayerPrefs.GetFloat("SFXValue", DEFAULT_SFX_VALUE);
        
        Debug.Log($"BGMValue: {BGMValue} | SFXValue: {SFXValue}");
        ApplyValue();
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
        ApplyValue();
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
        ApplyValue();
    }

    public static void ApplyValue()
    {
        
        if (Mixer != null)
        {
            float bgm = Mathf.Clamp(BGMValue, 0.0001f, 1f);
            float sfx = Mathf.Clamp(SFXValue, 0.0001f, 1f);

            Mixer.SetFloat("BGM", Mathf.Log10(bgm) * 20f);
            Mixer.SetFloat("SFX", Mathf.Log10(sfx) * 20f);
        }
    }
}
