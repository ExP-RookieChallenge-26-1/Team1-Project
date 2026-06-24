using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Stage5Customer : SpecialBase
{
    public RectTransform rect;
    public CanvasGroup canvasGroup;
    [SerializeField] RectTransform body;
    [SerializeField] RectTransform arm;

    private void Start()
    {
        CreateIdle();
    }
    
    public override IEnumerator AnimationRoutine()
    {
        var pos = rect.anchoredPosition;
        pos.x = -1018;
        rect.anchoredPosition = pos;
        rect.DOAnchorPos3DX(-124, 0.3f).SetEase(Ease.OutBack);
        canvasGroup.DOFade(1, 0.1f);
        AdvancedDialogue.Inst.ShowNextDialogue();
        yield return new WaitForSeconds(1);
        AdvancedDialogue.Inst.blockDialogInput = false;
    }

    void CreateIdle()
    {
        Sequence seq = DOTween.Sequence();

        seq.AppendCallback(() =>
        {
            body.DOAnchorPosY(3f, 1.3f)
                .SetEase(Ease.InOutSine);

            arm.DOAnchorPosY(8f, 1.3f)
                .SetEase(Ease.InOutSine);

            arm.DOLocalRotate(
                new Vector3(0, 0, 2f),
                1.3f);
        });

        seq.AppendInterval(1.3f);

        seq.AppendCallback(() =>
        {
            body.DOAnchorPosY(-2f, 1.5f)
                .SetEase(Ease.InOutSine);

            arm.DOAnchorPosY(-8f, 1.5f)
                .SetEase(Ease.InOutSine);

            arm.DOLocalRotate(
                new Vector3(0, 0, -2f),
                1.5f);
        });

        seq.AppendInterval(1.5f);

        seq.SetLoops(-1);

        seq.Play();
    }
}