using UnityEngine;
using UnityEngine.UI;

public class NeonSignEffect : MonoBehaviour
{
    public AudioSource audioSource;

    [Header("Image")]
    public Image neonImage;

    [Header("Sprites")]
    public Sprite normalSprite;
    public Sprite flashSprite;

    // 소리 타이밍
    public float[] beatTimes =
    {
        1.33f,
        1.65f
    };

    private int currentIndex = 0;

    private bool flashing = false;
    private float flashTimer = 0f;

    public float flashDuration = 0.1f;

    void Update()
    {
        if (!audioSource.isPlaying)
            return;

        // 박자 체크
        if (currentIndex < beatTimes.Length &&
            audioSource.time >= beatTimes[currentIndex])
        {
            Flash();

            currentIndex++;
        }

        // 깜박임 종료
        if (flashing)
        {
            flashTimer -= Time.deltaTime;

            if (flashTimer <= 0f)
            {
                neonImage.sprite = normalSprite;
                flashing = false;
            }
        }
    }

    void Flash()
    {
        neonImage.sprite = flashSprite;

        flashing = true;
        flashTimer = flashDuration;
    }

    public void Play()
    {
        currentIndex = 0;

        neonImage.sprite = normalSprite;

        audioSource.Play();
    }
}