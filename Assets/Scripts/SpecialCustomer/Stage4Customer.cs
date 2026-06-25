using System.Collections;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Stage4Customer : SpecialBase
{
    public RectTransform stage4CustomerRect;
    public CanvasGroup canvasGroup;
    public CanvasGroup burgerPreviewGroup;
    public GameObject copyTarget;
    public Sprite[] sprites;
    public Image bodyImg;
    public RectTransform hand, mouth;
    public AudioClip mukbangClip, openClip;
    
    public override IEnumerator AnimationRoutine()
    {
        AdvancedDialogue.Inst.currentHummingClip = mukbangClip;
        canvasGroup.DOFade(1, 0.5f);
        
        AdvancedMain.Inst.onResultNormal += OnResultNormal;
        AdvancedMain.Inst.onResultBad += OnResultBad;
        
        yield return new WaitForSeconds(1);
        AdvancedDialogue.Inst.ShowNextDialogue();
        AdvancedDialogue.Inst.blockDialogInput = false;
    }

    private void OnResultBad()
    {
        AdvancedDialogue.Inst.actionByIndexDic = new();
        AdvancedDialogue.Inst.actionByIndexDic.Add(0, ()=>StartCoroutine(EatingRoutine(2)));
        
    }

    private void OnResultNormal()
    {
        AdvancedDialogue.Inst.actionByIndexDic = new();
        AdvancedDialogue.Inst.actionByIndexDic.Add(0, ()=>StartCoroutine(EatingRoutine(0)));
    }

    IEnumerator EatingRoutine(int nextIndex)
    {
        AdvancedDialogue.Inst.blockDialogInput = true;
        yield return new WaitForSeconds(2);
        AdvancedDialogue.Inst.CloseChatOnlyVisual();
        burgerPreviewGroup.DOFade(0, 0.5f);
        yield return new WaitForSeconds(0.5f);
        bodyImg.sprite = sprites[3];
        hand.gameObject.SetActive(true);
        mouth.gameObject.SetActive(true);
        //97
        var newObj = Instantiate(copyTarget, stage4CustomerRect);
        var newRect = newObj.GetComponent<RectTransform>();
        newRect.transform.SetParent(hand);
        newRect.anchorMax = new Vector2(0.5f, 1);
        newRect.anchorMin = new Vector2(0.5f, 1);
        newRect.pivot = new Vector2(0.5f, 1);
        newRect.transform.localScale = Vector3.one * 0.35f;
        newRect.anchoredPosition3D = new Vector3(20.4f, -44.9f, 0);
        var newGroup = newRect.GetComponent<CanvasGroup>();
        hand.DOAnchorPos3DY(-100, 1f);
        newGroup.alpha = 1;
        newGroup.DOFade(0, 0.25f).SetDelay(0.9f);
        RectTransformUtil.FitToChildren(newRect);
        yield return new WaitForSeconds(1.5f);
        hand.gameObject.SetActive(false);
        mouth.gameObject.SetActive(false);
        bodyImg.sprite = sprites[nextIndex];
        AdvancedDialogue.Inst.ShowNextDialogue();
        AdvancedDialogue.Inst.blockDialogInput = false;
    }

    public override void HideAnimation()
    {
        canvasGroup.DOFade(0, 0.5f);
    }
}
