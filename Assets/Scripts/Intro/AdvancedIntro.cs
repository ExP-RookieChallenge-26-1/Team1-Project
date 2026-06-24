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
    public Tween loopTween;
    public RectTransform resetWindow;
    public CanvasGroup resetGroup;
    public Image resetBg;

    private void Awake()
    {
        neonSignEffect = GetComponent<NeonSignEffect>();
    }
    
    private void Start()
    {
        loopTween = press2Startxt.DOFade(0.2f, 0.8f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    public void OnClickOpen()
    {
        StartCoroutine(CorAnim());
    }

    IEnumerator CorAnim()
    {
        loopTween.Kill(false);
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

    public void OpenReset()
    {
        resetBg.gameObject.SetActive(true);
        
        //초기값 설정
        resetBg.color = Color.black.SetAlpha(0);
        resetWindow.anchoredPosition3D = new Vector3(0, -185f, 0);
        resetGroup.alpha = 0;
        
        //애니메이션
        resetWindow.DOAnchorPos3DY(-6, 0.15f);
        resetGroup.DOFade(1, 0.15f);
        resetBg.DOFade(0.9f, 0.3f);
    }

    public void CloseReset()
    {
        resetGroup.DOFade(0, 0.3f);
        resetBg.DOFade(0, 0.3f).OnComplete(() =>
        {
            resetBg.gameObject.SetActive(false);
        });
    }

    public void ResetData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        SceneManager.LoadScene("Intro");
    }
}
