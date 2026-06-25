using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Stage2Customer : SpecialBase
{
    public AudioClip humming;
    public CanvasGroup canvasGroup;
    public Image body;
    public Sprite[] sprites;

    public override IEnumerator AnimationRoutine()
    {
        AdvancedMain.Inst.onResultBad += OnResultBad;
        AdvancedMain.Inst.onResultNormal += OnResultNormal;
        
        AdvancedDialogue.Inst.currentHummingClip = humming;
        AdvancedDialogue.Inst.blockDialogInput = false;
        AdvancedDialogue.Inst.ShowNextDialogue();
        canvasGroup.DOFade(1, 0.5f);

        AdvancedDialogue.Inst.actionByIndexDic = new();
        AdvancedDialogue.Inst.actionByIndexDic.Add(3, ()=>body.sprite=sprites[0]);
        AdvancedDialogue.Inst.actionByIndexDic.Add(4, ()=>body.sprite=sprites[1]);
        AdvancedDialogue.Inst.actionByIndexDic.Add(14, ()=>body.sprite=sprites[2]);
        AdvancedDialogue.Inst.actionByIndexDic.Add(15, ()=>body.sprite=sprites[1]);
        yield break;
    }

    private void OnResultNormal()
    {
        AdvancedDialogue.Inst.actionByIndexDic = new();
        AdvancedDialogue.Inst.actionByIndexDic.Add(0, ()=>body.sprite=sprites[0]);
        AdvancedDialogue.Inst.actionByIndexDic.Add(5, ()=>body.sprite=sprites[1]);
    }

    private void OnResultBad()
    {
        AdvancedDialogue.Inst.actionByIndexDic = new();
        AdvancedDialogue.Inst.actionByIndexDic.Add(1, ()=>body.sprite=sprites[2]);
        AdvancedDialogue.Inst.actionByIndexDic.Add(3, ()=>body.sprite=sprites[1]);
    }

    public override void HideAnimation()
    {
        body.GetComponent<RectTransform>().DOAnchorPos3DX(-1018, 1f).SetEase(Ease.OutBack);
        body.DOFade(0, 0.5f);
    }

    public override void UpdateEmotion(CustomerEmotion emotion)
    {
        body.sprite = sprites[(int)emotion];
    }
}
