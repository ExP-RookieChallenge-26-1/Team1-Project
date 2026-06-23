using System.Collections;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AdvancedIntro : MonoBehaviour
{
    public NeonSignEffect neonSignEffect;
    public Image fadeImg;
    public AudioClip bellAudio;
    public CanvasGroup press2Startxt;
    public Animator anim;

    private void Awake()
    {
        neonSignEffect = GetComponent<NeonSignEffect>();
    }

    public void OnClickOpen()
    {
        StartCoroutine(CorAnim());
    }

    IEnumerator CorAnim()
    {
        press2Startxt.DOFade(0, 0.25f);
        neonSignEffect.Play();
        yield return new WaitForSeconds(2f);
        
        SFXPlayer.Instance.Play(bellAudio);
        anim.SetTrigger("ZoomIn");
        fadeImg.DOFade(1, 0.8f).OnComplete(() =>
        {
            GoToGameScene();
        });
    }

    void GoToGameScene()
    {
        SceneManager.LoadScene("Main");
        if (PausedController.Inst)
        {
            PausedController.Inst.fullFadeImg.gameObject.SetActive(true);
            PausedController.Inst.fullFadeImg.color = Color.black;
            PausedController.Inst.fullFadeImg.DOFade(0, 1f).SetDelay(1.5f).OnComplete(() =>
            {
                PausedController.Inst.fullFadeImg.gameObject.SetActive(false);
            });
        }
    }
}
