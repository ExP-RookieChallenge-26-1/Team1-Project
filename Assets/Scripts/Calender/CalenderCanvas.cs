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
    private TextMeshProUGUI dayText;
    private Vector2 baseTextPos;
    private bool hasBaseTextPos;

    private void Awake()
    {
        Inst = this;
        FindDayText();
    }

    public void SetDayTxt(int stageIndex)
    {
        SetDayTextVisible(true);
        SetTimerTextPos(false);
        if (stageIndex <= dayNums.Length - 1)
        {
            SetText(dayNums[stageIndex].ToString());   
        }
    }

    public TextMeshProUGUI GetText()
    {
        return originalCrop.transform.GetChild(0).transform.GetComponent<TextMeshProUGUI>();
    }

    public void SetText(string text)
    {
        GetText().text = text;
    }

    public void SetTimerTxt(int seconds)
    {
        SetDayTextVisible(false);
        SetTimerTextPos(true);
        SetText($"<size=42>Time left</size>\n<size=130>{seconds}</size>");
    }

    public IEnumerator PlayTimerRoutine(int seconds)
    {
        PlayAnimationToCenter();
        SetTimerTxt(seconds);
        yield return new WaitForSeconds(0.8f);
        PlayAnimationToLT();
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

    private void FindDayText()
    {
        if (target == null) return;

        TextMeshProUGUI mainText = GetText();
        foreach (TextMeshProUGUI text in target.GetComponentsInChildren<TextMeshProUGUI>(true))
        {
            if (text != mainText && text.text.Trim().ToLower() == "day")
            {
                dayText = text;
                return;
            }
        }
    }

    private void SetDayTextVisible(bool visible)
    {
        if (dayText == null)
        {
            FindDayText();
        }

        if (dayText != null)
        {
            dayText.gameObject.SetActive(visible);
        }
    }

    private void SetTimerTextPos(bool isTimer)
    {
        RectTransform textRect = GetText().rectTransform;
        if (!hasBaseTextPos)
        {
            baseTextPos = textRect.anchoredPosition;
            hasBaseTextPos = true;
        }

        textRect.anchoredPosition = isTimer ? baseTextPos + Vector2.up * 30f : baseTextPos;
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
