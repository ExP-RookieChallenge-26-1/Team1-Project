using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Stage3Customer : SpecialBase
{
    public CanvasGroup canvasGroup;
    public Image bodyImg;
    public Sprite[] sprites;
    public Sprite[] posSprites;
    public Image flashImg;
    public AudioClip[] clips;

    private bool _isGood;
    
    public override IEnumerator AnimationRoutine()
    {
        AdvancedDialogue.Inst.currentHummingClip = clips[0];
        AdvancedDialogue.Inst.actionByIndexDic = new();
        AdvancedDialogue.Inst.actionByIndexDic.Add(3, ()=>ChangeSprite(0));
        AdvancedDialogue.Inst.actionByIndexDic.Add(4, ()=>ChangeSprite(1));
        AdvancedDialogue.Inst.actionByIndexDic.Add(9, () => StartCoroutine(PosRoutine(0)));
        AdvancedDialogue.Inst.actionByIndexDic.Add(12, ()=>ChangeSprite(1));
        AdvancedDialogue.Inst.actionByIndexDic.Add(14, () => StartCoroutine(PosRoutine(1)));
        AdvancedDialogue.Inst.actionByIndexDic.Add(18, ()=>ChangeSprite(0));
        AdvancedDialogue.Inst.actionByIndexDic.Add(19, ()=>ChangeSprite(1));
        
        AdvancedMain.Inst.onResultNormal += OnResultNormal;
        AdvancedMain.Inst.onResultBad += OnResultBad;

        
        canvasGroup.DOFade(1, 0.5f);
        yield return new WaitForSeconds(1);
        AdvancedDialogue.Inst.ShowNextDialogue();
        AdvancedDialogue.Inst.blockDialogInput = false;
    }

    public override void HideAnimation()
    {
        if(_isGood)
            Flash();

        bodyImg.DOFade(0, 0.5f);
    }

    private void OnResultBad()
    {
        _isGood = false;
        AdvancedDialogue.Inst.actionByIndexDic = new();
        AdvancedDialogue.Inst.actionByIndexDic.Add(1, ()=>ChangeSprite(2));
    }

    private void OnResultNormal()
    {
        _isGood = true;
        AdvancedDialogue.Inst.actionByIndexDic = new();
        AdvancedDialogue.Inst.actionByIndexDic.Add(0, ()=>StartCoroutine(PosRoutine2(0)));
        AdvancedDialogue.Inst.actionByIndexDic.Add(1, ()=>StartCoroutine(PosRoutine2(1)));
        AdvancedDialogue.Inst.actionByIndexDic.Add(2, ()=>StartCoroutine(PosRoutine2(2)));
        AdvancedDialogue.Inst.actionByIndexDic.Add(3, ()=>ChangeSprite(0));
        AdvancedDialogue.Inst.actionByIndexDic.Add(4, ()=>ChangeSprite(1));
        AdvancedDialogue.Inst.actionByIndexDic.Add(6, ()=>ChangeSprite(0));
    }

    void ChangeSprite(int index) => bodyImg.sprite = sprites[index];

    IEnumerator PosRoutine2(int posIndex)
    {
        AdvancedDialogue.Inst.blockDialogInput = true;
        yield return new WaitForSeconds(2);
        AdvancedDialogue.Inst.CloseChatOnlyVisual();
        bodyImg.sprite = posSprites[posIndex];
        SFXPlayer.Instance.Play(clips[posIndex]);
        Flash();
        yield return new WaitForSeconds(1);
        flashImg.gameObject.SetActive(false);
        AdvancedDialogue.Inst.ShowNextDialogue();
        bodyImg.sprite = sprites[1];
        AdvancedDialogue.Inst.blockDialogInput = false;
    }

    IEnumerator PosRoutine(int endIndex)
    {
        AdvancedDialogue.Inst.blockDialogInput = true;
        yield return new WaitForSeconds(1);
        AdvancedDialogue.Inst.CloseChatOnlyVisual();
        int i = 0;
        foreach (var sprite in posSprites)
        {
            yield return new WaitForSeconds(1f);
            bodyImg.sprite = sprite;
            SFXPlayer.Instance.Play(clips[i]);
            Flash();
            i++;
        }

        yield return new WaitForSeconds(1);
        flashImg.gameObject.SetActive(false);
        AdvancedDialogue.Inst.ShowNextDialogue();
        bodyImg.sprite = sprites[endIndex];
        AdvancedDialogue.Inst.blockDialogInput = false;
    }
    
    void Flash()
    {
        flashImg.color = Color.white;
        flashImg.gameObject.SetActive(true);
        Sequence seq = DOTween.Sequence();
        seq.Append(flashImg.DOFade(1f, 0f));
        seq.Append(flashImg.DOFade(0f, 0.15f));
        seq.Play();
    }
}
