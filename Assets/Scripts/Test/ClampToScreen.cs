using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClampToScreen : MonoBehaviour
{
    public RectTransform canvas;
    [SerializeField] RectTransform target;

    void Start()
    {
        ClampTop();
    }

    private void Update()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            ClampTop();
        }
    }

    public void ClampTop()
    {
        float safeTop = Screen.safeArea.yMax;

        float canvasHeight = canvas.rect.height;
        float safeTopCanvas = safeTop / Screen.height * canvasHeight - canvasHeight * 0.5f;

        float topY = target.anchoredPosition.y + target.rect.height * 0.5f;

        if (topY > safeTopCanvas)
        {
            target.anchoredPosition += Vector2.up * (safeTopCanvas - topY);
        }
    }
}