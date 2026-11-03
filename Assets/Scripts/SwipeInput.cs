using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// <summary>
/// 보드 영역에서 시작한 스와이프(터치/마우스 드래그)와 에디터용 방향키를 Direction으로 바꿔 알린다.
/// 새 Input System / 기존 Input Manager 둘 다 지원.
/// </summary>
public class SwipeInput : MonoBehaviour
{
    [Tooltip("스와이프로 인정하는 최소 거리 (인치 단위, 화면 DPI 기준)")]
    [SerializeField] float minSwipeInches = 0.2f;
    [SerializeField] RectTransform swipeArea;

    public event Action<Direction> Swiped;

    Vector2? pressStart;

    void Update()
    {
        ReadPointer(out bool pressed, out bool released, out Vector2 pos);

        if (pressed && (swipeArea == null || RectTransformUtility.RectangleContainsScreenPoint(swipeArea, pos, null)))
            pressStart = pos;

        if (released && pressStart.HasValue)
        {
            Vector2 delta = pos - pressStart.Value;
            pressStart = null;
            float dpi = Screen.dpi > 0 ? Screen.dpi : 160f;
            if (delta.magnitude >= minSwipeInches * dpi)
            {
                if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                    Swiped?.Invoke(delta.x > 0 ? Direction.Right : Direction.Left);
                else
                    Swiped?.Invoke(delta.y > 0 ? Direction.Up : Direction.Down);   // 화면 좌표는 y가 위로 증가
            }
        }

        if (ReadArrowKey(out var dir)) Swiped?.Invoke(dir);
    }

#if ENABLE_INPUT_SYSTEM
    static void ReadPointer(out bool pressed, out bool released, out Vector2 pos)
    {
        var p = Pointer.current;
        pressed = released = false;
        pos = Vector2.zero;
        if (p == null) return;
        pos = p.position.ReadValue();
        pressed = p.press.wasPressedThisFrame;
        released = p.press.wasReleasedThisFrame;
    }

    static bool ReadArrowKey(out Direction dir)
    {
        dir = default;
        var k = Keyboard.current;
        if (k == null) return false;
        if (k.leftArrowKey.wasPressedThisFrame)  { dir = Direction.Left;  return true; }
        if (k.rightArrowKey.wasPressedThisFrame) { dir = Direction.Right; return true; }
        if (k.upArrowKey.wasPressedThisFrame)    { dir = Direction.Up;    return true; }
        if (k.downArrowKey.wasPressedThisFrame)  { dir = Direction.Down;  return true; }
        return false;
    }
#else
    // Input Manager는 기본적으로 터치를 마우스 입력으로도 넘겨준다 (Input.simulateMouseWithTouches)
    static void ReadPointer(out bool pressed, out bool released, out Vector2 pos)
    {
        pos = Input.mousePosition;
        pressed = Input.GetMouseButtonDown(0);
        released = Input.GetMouseButtonUp(0);
    }

    static bool ReadArrowKey(out Direction dir)
    {
        dir = default;
        if (Input.GetKeyDown(KeyCode.LeftArrow)) { dir = Direction.Left; return true; }
        if (Input.GetKeyDown(KeyCode.RightArrow)) { dir = Direction.Right; return true; }
        if (Input.GetKeyDown(KeyCode.UpArrow)) { dir = Direction.Up; return true; }
        if (Input.GetKeyDown(KeyCode.DownArrow)) { dir = Direction.Down; return true; }
        return false;
    }
#endif
}
