using System;
using UnityEngine;
using UnityEngine.Audio;

public class DataLoader : MonoBehaviour
{
    public AudioMixer mixer;
    public static AudioMixer Mixer;
    
    public static float BGMValue;
    public static float SFXValue;

    // 저장 키. 데이터 초기화(엔딩, 처음부터) 때 음량을 백업/복원하는 곳도 이 키를 써야 한다.
    public const string BGM_KEY = "BGMValue";
    public const string SFX_KEY = "SFXValue";

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
    }

    void LoadData()
    {
        if (Loaded)
            return;

        Loaded = true;
        
        BGMValue = PlayerPrefs.GetFloat(BGM_KEY, DEFAULT_BGM_VALUE);
        SFXValue = PlayerPrefs.GetFloat(SFX_KEY, DEFAULT_SFX_VALUE);
        
        Debug.Log($"BGMValue: {BGMValue} | SFXValue: {SFXValue}");
        ApplyValue();
    }
    

    /// <summary>
    /// BGM 값 적용 및 PlayerPrefs 저장
    /// </summary>
    public static void SetBGMValue(float v)
    {
        BGMValue = v;
        PlayerPrefs.SetFloat(BGM_KEY, v);
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
        PlayerPrefs.SetFloat(SFX_KEY, v);
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
