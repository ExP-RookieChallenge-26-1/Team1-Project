using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Stage6Customer : SpecialBase
{
    public AudioClip humming;
    public RectTransform bodyRect;
    public CanvasGroup canvasGroup;
    public Image body;
    public Sprite[] sprites;
    public List<IngredientType> fakeBurgers;
    public CanvasGroup previewOnCounterGroup;
    public RectTransform shakerRect;

    #region 애니메이션
    [SerializeField] private RectTransform target;

    [Header("랜덤 대기 시간")]
    [SerializeField] private float minWait = 2f;
    [SerializeField] private float maxWait = 6f;

    [Header("감시 시간")]
    [SerializeField] private float watchDuration = 1.5f;

    [Header("전환 시간")]
    [SerializeField] private float moveDuration = 0.4f;

    private readonly Vector2 idlePos = Vector2.zero;
    private readonly Vector2 watchPos = new Vector2(0, -324);

    private const float idleRot = 0f;
    private const float watchRot = -10.21f;
    #endregion


    public override IEnumerator AnimationRoutine()
    {
        AdvancedMain.Inst.isFaking = true;
        AdvancedDialogue.Inst.currentHummingClip = humming;
        AdvancedDialogue.Inst.InsertFakePreview(fakeBurgers, 1);
        canvasGroup.DOFade(1, 0.5f);
        yield return new WaitForSeconds(1);
        AdvancedDialogue.Inst.ShowNextDialogue();
        AdvancedDialogue.Inst.blockDialogInput = false;
    }

    private void OnResultBad()
    {
        AdvancedDialogue.Inst.actionByIndexDic = new();
        AdvancedDialogue.Inst.actionByIndexDic.Add(0, ()=>body.sprite = sprites[4]);
        AdvancedDialogue.Inst.actionByIndexDic.Add(3, ()=>body.sprite = sprites[2]);
    }

    private void OnResultNormal()
    {
        AdvancedDialogue.Inst.actionByIndexDic = new();
        AdvancedDialogue.Inst.actionByIndexDic.Add(3, ()=>body.sprite = sprites[3]);
    }

    public void SetActionsAfterFake()
    {
        AdvancedDialogue.Inst.actionByIndexDic = new();
        AdvancedDialogue.Inst.actionByIndexDic.Add(3, ()=>StartCoroutine(FakeRoutine()));
    }

    public override void HideAnimation()
    {
        canvasGroup.DOFade(0, 0.5f);
    }

    IEnumerator FakeRoutine()
    {
        previewOnCounterGroup.DOFade(0, 0.5f);
        AdvancedDialogue.Inst.blockDialogInput = true;
        yield return new WaitForSeconds(1);
        AdvancedDialogue.Inst.CloseChatOnlyVisual();
   

        for (int i = 0; i < sprites.Length; i++)
        {
            if(i==3)
                break;
            yield return new WaitForSeconds(1);
            body.sprite = sprites[i];
        }
   
        body.transform.localScale = new Vector3(1, 0.8f, 1);
        bodyRect.anchoredPosition3D = bodyRect.anchoredPosition3D.SetY(-680);
        bodyRect.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.InOutBack);
        bodyRect.DOAnchorPos3DY(-89, 0.15f);
    
        yield return new WaitForSeconds(1);
        AdvancedDialogue.Inst.blockDialogInput = false;
        AdvancedDialogue.Inst.ShowNextDialogue();
        AdvancedMain.Inst.tong.enableDrag = true;
        for (int i = 0; i < previewOnCounterGroup.transform.childCount; i++)
        {
            if ( i == 0)
                continue;
            Destroy(previewOnCounterGroup.transform.GetChild(i).gameObject);
        }

        previewOnCounterGroup.alpha = 1;
        SideBurgerMaker.Inst.counterPreview.GetComponent<CanvasGroup>().alpha = 0;
        AdvancedMain.Inst.onResultNormal += OnResultNormal;
        AdvancedMain.Inst.onResultBad += OnResultBad;

        StartCoroutine(WatchRoutine());
    }
    
    private IEnumerator WatchRoutine()
    {
        while (true)
        {
            // 조건이 만족될 때까지 대기
            yield return new WaitUntil(() =>
                !AdvancedMain.Inst.isFaking &&
                AdvancedMain.Inst.enableSubmit);

            // 랜덤 대기
            yield return new WaitForSeconds(UnityEngine.Random.Range(minWait, maxWait));

            // 대기 중 조건이 바뀌었으면 다시 처음부터
            if (AdvancedMain.Inst.isFaking ||
                !AdvancedMain.Inst.enableSubmit)
                continue;

            // 감시 시작
            Sequence seq = DOTween.Sequence();
            body.sprite = sprites[5];
            seq.Join(target.DOAnchorPos(watchPos, moveDuration)
                .SetEase(Ease.OutBack));

            seq.Join(target.DORotate(
                new Vector3(0, 0, watchRot),
                moveDuration));

            yield return seq.WaitForCompletion();
            
            Tween shakeTween = shakerRect.DOShakeAnchorPos(
                duration: 999f,
                strength: 4.5f,
                vibrato: 20,
                randomness: 90f,
                snapping: false,
                fadeOut: false);

            // 감시 중에도 조건이 깨지면 즉시 복귀
            float timer = 0f;
            while (timer < watchDuration)
            {
                if (AdvancedMain.Inst.isFaking ||
                    !AdvancedMain.Inst.enableSubmit)
                    break;

                timer += Time.deltaTime;
                yield return null;
            }

            // 감시 종료
            seq = DOTween.Sequence();
            body.sprite = sprites[2];
            seq.Join(target.DOAnchorPos(idlePos, moveDuration)
                .SetEase(Ease.InOutSine));

            seq.Join(target.DORotate(
                new Vector3(0, 0, idleRot),
                moveDuration));
            
            shakeTween.Kill();

            shakerRect.DOAnchorPos(Vector2.zero, 0.1f);

            yield return seq.WaitForCompletion();
        }
    }
}
