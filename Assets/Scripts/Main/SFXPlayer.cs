using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

public class SFXPlayer : MonoBehaviour
{
    public static SFXPlayer Instance;

    [Header("Pool Settings")]
    [SerializeField] private int poolSize = 10;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixerGroup sfxMixerGroup;

    private readonly List<AudioSource> audioSources = new();

    private int currentIndex = 0;

    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        CreatePool();
    }

    private void CreatePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();

            source.playOnAwake = false;
            source.loop = false;

            // Audio Mixer Group 적용
            source.outputAudioMixerGroup = sfxMixerGroup;

            audioSources.Add(source);
        }
    }

    /// <summary>
    /// 효과음 재생
    /// </summary>
    public void Play(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;

        AudioSource source = GetAvailableSource();

        source.pitch = 1f;
        source.volume = volume;
        source.clip = clip;

        source.Play();
    }

    /// <summary>
    /// 랜덤 피치 효과음 재생
    /// </summary>
    public void PlayRandomPitch(
        AudioClip clip,
        float volume = 1f,
        float minPitch = 0.9f,
        float maxPitch = 1.1f)
    {
        if (clip == null) return;

        AudioSource source = GetAvailableSource();

        source.pitch = Random.Range(minPitch, maxPitch);
        source.volume = volume;
        source.clip = clip;

        source.Play();
    }

    /// <summary>
    /// 사용 가능한 AudioSource 반환
    /// </summary>
    private AudioSource GetAvailableSource()
    {
        foreach (AudioSource source in audioSources)
        {
            if (!source.isPlaying)
                return source;
        }

        AudioSource fallback = audioSources[currentIndex];

        currentIndex++;
        currentIndex %= audioSources.Count;

        return fallback;
    }
}