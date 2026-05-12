using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class PausedController : MonoBehaviour
{
    public static PausedController Inst;
    public GameObject pausedPanel;
    public Image fadeImg;
    public Image fullFadeImg;
    public CanvasGroup windowGroup;
    public RectTransform windowRect;
    public Slider bgmSlider, sfxSlider;
    public Button homeBtn;

    private bool _opened;
    private bool _openAnimating;
    private bool _goIntroAnimating;

    private void Awake()
    {
        //싱글톤
        if (Inst)
        {
            Destroy(gameObject);
        }
        else
        {
            Inst = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void OpenPausedMenu()
    {
        if (_opened)
            return;

        _opened = true;
        pausedPanel.SetActive(true);
        fullFadeImg.gameObject.SetActive(false);
        
        //초기값 설정
        fadeImg.color = Color.black.SetAlpha(0);
        windowRect.anchoredPosition3D = new Vector3(0, -185f, 0);
        windowGroup.alpha = 0;
        
        //애니메이션
        _openAnimating = true;
        windowRect.DOAnchorPos3DY(-6, 0.15f);
        windowGroup.DOFade(1, 0.15f);
        fadeImg.DOFade(0.9f, 0.3f).OnComplete(()=>_openAnimating = false);
        
        //슬라이더 값 적용
        bgmSlider.value = DataLoader.BGMValue;
        sfxSlider.value = DataLoader.SFXValue;
        
        bgmSlider.onValueChanged.RemoveAllListeners();
        bgmSlider.onValueChanged.AddListener((v) =>
        {
            DataLoader.SetBGMValue(v);
        });
        
        sfxSlider.onValueChanged.RemoveAllListeners();
        sfxSlider.onValueChanged.AddListener((v) =>
        {
            DataLoader.SetSFXValue(v);
        });
        
        //Intro 씬에서는 홈화면 버튼 없애기
        homeBtn.gameObject.SetActive(!SceneManager.GetSceneByName("Intro").isLoaded);

    }

    public void ClosePausedMenu()
    {
        if(_openAnimating)
            return;
        
        windowGroup.DOFade(0, 0.15f);
        fadeImg.DOFade(0, 0.3f).OnComplete(() =>
        {
            _opened = false;
            pausedPanel.SetActive(false);
        });
    }

    public void GoToIntro()
    {
        if(_goIntroAnimating)
            return;

        _goIntroAnimating = true;
        fullFadeImg.gameObject.SetActive(true);
        fullFadeImg.color = Color.black.SetAlpha(0);
        fullFadeImg.DOFade(1, 1f).OnComplete(() =>
        {
            ClosePausedMenu();
            SceneManager.LoadScene("Intro");
            fullFadeImg.DOFade(0, 1f).SetDelay(1.5f).OnComplete(() =>
            {
                _goIntroAnimating = false;
                fullFadeImg.gameObject.SetActive(false);
            });
        });
    }

    public void GoToCredit()
    {
        ClosePausedMenu();
        SceneManager.LoadScene("Credit");
    }
}
