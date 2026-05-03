using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{
    public Image fadeBG;
    public Image[] starFills;
    public CanvasGroup[] stars;
    public GameObject endCanvas;
    public TextMeshProUGUI endTxt, todayRatingTxt;
    public Button nextDayBtn;

    private void Start()
    {
        endCanvas.SetActive(false);
    }

    private void Update()
    {
        if(Keyboard.current.spaceKey.isPressed)
            ShowEndScreen(65);
    }

    public void ShowEndScreen(float score)
    {
        endCanvas.SetActive(true);
        
        //UI초기화
        fadeBG.color = Color.black.SetAlpha(0);
        endTxt.transform.localScale = Vector2.zero;
        todayRatingTxt.transform.localScale = Vector2.zero;
        nextDayBtn.transform.localScale = Vector3.zero;

        foreach (var start in stars)
        {
            start.alpha = 0;
        }

        foreach (var starFill in starFills)
        {
            starFill.fillAmount = 0;
        }
        
        fadeBG.DOFade(0.9f, 0.5f).OnComplete(() =>
        {
            endTxt.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutElastic).OnComplete(() =>
            {
                todayRatingTxt.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutElastic).SetDelay(1.5f).OnComplete(() =>
                {
                    for (int i = 0; i < stars.Length; i++)
                    {
                        stars[i].DOFade(1, 0.2f).SetDelay(i*0.25f + 1.5f);
                    }
                    
                    float delay = (stars.Length - 1 ) *0.25f + 2f;
                    for (int i = 0; i < starFills.Length; i++)
                    {
                        int index = i;
                        var star = starFills[index];

                        float v = Mathf.Clamp01((score - index * 15f) / 15f);

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
        });
    }
}
