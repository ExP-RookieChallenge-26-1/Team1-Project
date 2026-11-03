using System;
using UnityEngine;

/// <summary>
/// 손님 한 명의 제한시간. 남은 시간은 달력(CalenderCanvas)에 표시하고,
/// 시간이 다 되면 onTimeUp을 호출한다. (AdvancedMain이 자동으로 붙인다)
/// waitForStart로 시작하면 남은 시간만 보여주고 기다리다가 BeginCountdown 때부터 줄어든다.
/// </summary>
public class CustomerTimer : MonoBehaviour
{
    public float Remaining { get; private set; }
    public bool IsRunning { get; private set; }
    /// <summary>시간을 보여주고 시작 신호(BeginCountdown)를 기다리는 중.</summary>
    public bool IsWaitingForStart { get; private set; }

    private Action _onTimeUp;
    private int _lastShownSecond = -1;

    public void StartTimer(float seconds, Action onTimeUp, bool waitForStart = false)
    {
        Remaining = seconds;
        _onTimeUp = onTimeUp;
        IsWaitingForStart = seconds > 0 && waitForStart;
        IsRunning = seconds > 0 && !waitForStart;
        _lastShownSecond = -1;

        if (seconds > 0 && CalenderCanvas.Inst != null)
            StartCoroutine(CalenderCanvas.Inst.PlayTimerRoutine(Mathf.CeilToInt(seconds)));
    }

    /// <summary>기다리던 타이머의 시간을 줄이기 시작한다. 기다리는 중이 아니면 아무것도 하지 않는다.</summary>
    public void BeginCountdown()
    {
        if (!IsWaitingForStart) return;
        IsWaitingForStart = false;
        IsRunning = true;
    }

    public void Stop()
    {
        IsRunning = false;
        IsWaitingForStart = false;
        _onTimeUp = null;
    }

    private void Update()
    {
        if (!IsRunning) return;

        Remaining -= Time.deltaTime;
        if (Remaining <= 0f)
        {
            Remaining = 0f;
            ShowTime();
            var callback = _onTimeUp;
            Stop();
            callback?.Invoke();
            return;
        }

        ShowTime();
    }

    private void ShowTime()
    {
        int second = Mathf.CeilToInt(Remaining);
        if (second == _lastShownSecond || CalenderCanvas.Inst == null) return;
        _lastShownSecond = second;
        CalenderCanvas.Inst.SetTimerTxt(second);
    }
}
