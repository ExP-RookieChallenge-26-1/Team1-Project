using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainUIManager : MonoBehaviour
{
    public static MainUIManager Inst;

    public RectTransform scrollRect, gameViewRect;
    private float _scrollDelta;

    void Awake()
    {
        Inst = this;
    }

    private void Start()
    {
        _scrollDelta = gameViewRect.anchoredPosition3D.y;
    }

    public void OpenGameView()
    {
        scrollRect.DOAnchorPos3DY(scrollRect.anchoredPosition3D.y - _scrollDelta, 0.5f);
        gameViewRect.DOAnchorPos3DY(0, 0.5f);
    }

    public void CloseGameView()
    {
        scrollRect.DOAnchorPos3DY(scrollRect.anchoredPosition3D.y + _scrollDelta, 0.5f);
        gameViewRect.DOAnchorPos3DY(_scrollDelta, 0.5f);
    }
}
