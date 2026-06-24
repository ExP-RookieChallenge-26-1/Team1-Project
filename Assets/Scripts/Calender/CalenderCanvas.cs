using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CalenderCanvas : MonoBehaviour
{
    public static CalenderCanvas Inst;
    
    public int[] dayNums;
    public RectTransform target;
    public RectTransform originalCrop;
    public RectTransform safeArea;
    public float duration = 0.3f;

    private void Awake()
    {
        Inst = this;
    }

    public void SetDayTxt(int stageIndex)
    {
        if (stageIndex <= dayNums.Length - 1)
        {
            originalCrop.transform.GetChild(0).transform.GetComponent<TextMeshProUGUI>().text =
                dayNums[stageIndex].ToString();   
        }
    }

    public IEnumerator PlayRoutine(int stageIndex)
    {
        PlayAnimationToCenter();
        yield return new WaitForSeconds(1.3f);

        SetDayTxt(stageIndex+1);
        
        var newCrop = Instantiate(originalCrop, target);
        newCrop.SetParent(safeArea);
        SetPivotKeepPosition(newCrop, Vector2.zero);
        
        newCrop.transform.GetChild(0).transform.GetComponent<TextMeshProUGUI>().text =
            dayNums[stageIndex].ToString();
     
        newCrop.DOLocalRotate(new Vector3(0, 0, -20), 1);
        newCrop.DOAnchorPos3DY(newCrop.anchoredPosition3D.y - 200, 1);
        newCrop.GetComponent<CanvasGroup>().DOFade(0, 1.1f).OnComplete(() =>
        {
            Destroy(newCrop.gameObject);
        });
        
        yield return new WaitForSeconds(2);
        PlayAnimationToLT();
    }

    public void PlayAnimationToLT()
    {
        Sequence seq = DOTween.Sequence();

        seq.Join(target.DOAnchorPos(new Vector2(49.671f, -54.875f), duration));
        seq.Join(target.DOScale(0.45f, duration));

        seq.Join(DOTween.To(
            () => target.anchorMin,
            x => target.anchorMin = x,
            new Vector2(0f, 1f),
            duration));

        seq.Join(DOTween.To(
            () => target.anchorMax,
            x => target.anchorMax = x,
            new Vector2(0f, 1f),
            duration));

        seq.Join(DOTween.To(
            () => target.pivot,
            x => target.pivot = x,
            new Vector2(0f, 1f),
            duration));
    }

    public void PlayAnimationToCenter()
    {
        Sequence seq = DOTween.Sequence();

        seq.Join(target.DOAnchorPos(Vector2.zero, duration));
        seq.Join(target.DOScale(1.4f, duration));

        seq.Join(DOTween.To(
            () => target.anchorMin,
            x => target.anchorMin = x,
            new Vector2(0.5f, 0.5f),
            duration));

        seq.Join(DOTween.To(
            () => target.anchorMax,
            x => target.anchorMax = x,
            new Vector2(0.5f, 0.5f),
            duration));

        seq.Join(DOTween.To(
            () => target.pivot,
            x => target.pivot = x,
            new Vector2(0.5f, 0.5f),
            duration));
    }
    
    public static void SetPivotKeepPosition(RectTransform rect, Vector2 newPivot)
    {
        Vector2 size = rect.rect.size;
        Vector2 deltaPivot = newPivot - rect.pivot;

        Vector3 delta = new Vector3(
            deltaPivot.x * size.x,
            deltaPivot.y * size.y);

        rect.pivot = newPivot;
        rect.localPosition += rect.TransformVector(delta);
    }
}
