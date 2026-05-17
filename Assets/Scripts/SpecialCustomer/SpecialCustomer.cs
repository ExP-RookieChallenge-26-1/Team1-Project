using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.UI;

/*
 * 1스테이지 전용
 * 중간 발표 이후 2스테이지에도 적용 가능하도록 수정 예정입니다.
 * 
 */
public class SpecialCustomer : MonoBehaviour
{
    public RectTransform handRect, bodyRect, hand2Rect;
    public Sprite[] sprites;
    public Image body;
  
    public void StartAnimation()
    {
        StartCoroutine(CorAnim());
    }

    IEnumerator CorAnim()
    {
        ShowHand1();
        yield return new WaitForSeconds(1f);
        ShowHand2();
        yield return new WaitForSeconds(0.5f);
        AdvancedDialogue.Inst.ShowNextDialogue();
        yield return new WaitForSeconds(1.5f);
        ShowBody();
        yield return new WaitForSeconds(1);
        AdvancedDialogue.Inst.blockDialogInput = false;
    }

    void ShowHand1()
    {
        handRect.gameObject.SetActive(true);
        handRect.transform.localScale = new Vector3(0.39f, 0.2f, 1);
        handRect.anchoredPosition3D = handRect.anchoredPosition3D.SetY(-400);
        handRect.transform.DOScale(new Vector3(0.39f, 0.39f, 1), 0.3f).SetEase(Ease.InOutBack);
        handRect.DOAnchorPos3DY(-369, 0.15f);
    }

    void ShowHand2()
    {
        handRect.gameObject.SetActive(false);
        hand2Rect.gameObject.SetActive(true);
    }

    void ShowBody()
    {
        handRect.gameObject.SetActive(false);
        hand2Rect.gameObject.SetActive(false);
        bodyRect.gameObject.SetActive(true);
        bodyRect.transform.localScale = new Vector3(0.39f, 0.25f, 1);
        bodyRect.anchoredPosition3D = bodyRect.anchoredPosition3D.SetY(-540);
        bodyRect.transform.DOScale(new Vector3(0.39f, 0.39f, 1), 0.3f).SetEase(Ease.InOutBack);
        bodyRect.DOAnchorPos3DY(-512, 0.15f);
    }
}
