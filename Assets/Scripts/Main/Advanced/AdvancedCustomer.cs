using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class AdvancedCustomer : MonoBehaviour
{
    public RectTransform customerRect;
    
    public void ShowCustomer()
    {
        customerRect.transform.localScale = new Vector3(1, 0.8f, 1);
        customerRect.anchoredPosition3D = customerRect.anchoredPosition3D.SetY(-680);
        customerRect.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.InOutBack);
        customerRect.DOAnchorPos3DY(-89, 0.15f);
    }
}
