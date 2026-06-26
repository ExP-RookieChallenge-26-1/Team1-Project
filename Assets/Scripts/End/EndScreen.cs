using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{
    public static EndScreen Inst;
    
    public Image fadeBG;
    public Image windowImg;
    public Sprite goodBg, sosoBg, badBg;
    public CanvasGroup windowGroup;
    public Image[] starFills;
    public CanvasGroup[] stars;
    public GameObject endCanvas;
    public TextMeshProUGUI endTxt, todayRatingTxt;
    public Button nextDayBtn;
    public AudioClip endMyDaySfx;

    private void Awake()
    {
        Inst = this;
    }

    private void Start()
    {
        endCanvas.SetActive(false);
    }


    IEnumerator GoToNextStageRoutine()
    {
        windowGroup.DOFade(0, 0.5f);
        yield return StartCoroutine(CalenderCanvas.Inst.PlayRoutine(StageFlowManager.Inst.currentStageIndex - 1));
        endCanvas.SetActive(false);
        /*if (AdvancedMain.Inst.allStageEnded)
        {
            SceneManager.LoadScene("Intro");
        }
        else
        {
            AdvancedMain.Inst.StartFlow();   
        }*/
        AdvancedMain.Inst.StartFlow();
    }
    public void GoToNextStage()
    {
        StartCoroutine(GoToNextStageRoutine());
    }

    /*private void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            ShowEndScreen(65,100);
        }
    }*/

    public void ShowEndScreen(float score, float maxScore)
    {
        endCanvas.SetActive(true);
        
        //UI초기화
        fadeBG.color = Color.black.SetAlpha(0);
        endTxt.transform.localScale = Vector2.zero;
        todayRatingTxt.transform.localScale = Vector2.zero;
        nextDayBtn.transform.localScale = Vector3.zero;
        windowGroup.alpha = 0;
        

        foreach (var start in stars)
        {
            start.alpha = 0;
        }

        foreach (var starFill in starFills)
        {
            starFill.fillAmount = 0;
        }

        float ratio = maxScore > 0 ? score / maxScore : 0f;
        ratio = Mathf.Clamp01(ratio);

        if (ratio <= 0.2f)
        {
            windowImg.sprite = badBg;
        } 
        else if (ratio <= 0.6f)
        {
            windowImg.sprite = sosoBg;
        }
        else
        {
            windowImg.sprite = goodBg;
        }

        float totalStarsToFill = ratio * starFills.Length;

        fadeBG.DOFade(0.9f, 0.5f).OnComplete(() =>
        {
            windowGroup.DOFade(1, 0.5f).OnComplete(() =>
            {
                SFXPlayer.Instance.Play(endMyDaySfx);
                for (int i = 0; i < stars.Length; i++)
                {
                    stars[i].DOFade(1, 0.2f).SetDelay(i*0.25f + 0.5f);
                }
                    
                float delay = (stars.Length - 1 ) *0.25f + 2f;
                for (int i = 0; i < starFills.Length; i++)
                {
                    int index = i;
                    var star = starFills[index];

                    float v = Mathf.Clamp01(totalStarsToFill - index);

                    DOVirtual.Float(0, v, 0.3f, (x) =>
                        {
                            star.fillAmount = x;
                        })
                        .SetDelay(delay + index * 0.2f)
                        .SetEase(Ease.Linear);
                }
                    
                nextDayBtn.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutElastic).SetDelay(delay + 1 + starFills.Length * 0.2f);
            });
        });
    }
}
