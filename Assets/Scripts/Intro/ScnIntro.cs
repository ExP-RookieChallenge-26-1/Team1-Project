using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class ScnIntro : MonoBehaviour
{
    public RectTransform openBtnRect, bellRect;
    public Button openBtn;
    public TextMeshProUGUI openBtnTxt;
    public Image fadeImg;
    public Animator mainAnim;

    private void Start()
    {
        //UI 초기화
        openBtnTxt.text = "CLOSED";
        openBtn.onClick.AddListener(OnClickOpenBtn);
    }

    void OnClickOpenBtn()
    {
        //중복 클릭 방지
        openBtn.enabled = false;
        
        //펫말 회전 애니메이션
        Sequence seq = DOTween.Sequence();
        
        seq.Append(openBtnRect.DOScaleX(0, 0.2f).SetEase(Ease.InQuad));
        seq.Insert(0.07f, DOTween.Sequence().AppendCallback(() =>
        {
            openBtnTxt.transform.localScale = new Vector3(-1, 1, 1);
            openBtnTxt.text = "OPENED";
        }));
        seq.Join(openBtnRect.DORotate(new Vector3(0, 90, -6), 0.2f, RotateMode.Fast));

        
        seq.Append(openBtnRect.DOScaleX(1, 0.2f).SetEase(Ease.OutQuad));
        seq.Join(openBtnRect.DORotate(new Vector3(0, 180, 6), 0.2f, RotateMode.Fast));

        seq.OnComplete(() =>
        {
            //벨 애니메이션
           
            bellRect.localRotation = Quaternion.Euler(0, 0, 0);

            Sequence bellSeq = DOTween.Sequence();

            bellSeq.Append(bellRect.DOLocalRotate(new Vector3(0, 0, 10), 0.08f));
            bellSeq.Append(bellRect.DOLocalRotate(new Vector3(0, 0, -8), 0.1f));
            bellSeq.Append(bellRect.DOLocalRotate(new Vector3(0, 0, 5), 0.1f));
            bellSeq.Append(bellRect.DOLocalRotate(new Vector3(0, 0, -3), 0.1f));
            bellSeq.Append(bellRect.DOLocalRotate(new Vector3(0, 0, 0), 0.1f));

            bellSeq.SetLoops(2).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                mainAnim.SetTrigger("ZoomIn");
                fadeImg.DOFade(1, 0.6f).OnComplete(() =>
                {
                    GoToGameScene();
                });
            });
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
