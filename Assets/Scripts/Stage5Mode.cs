using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Collections;
using DG.Tweening;

/// <summary>
/// 5스테이지 특수 손님(PD) 기믹: 제한시간 동안 버거를 여러 번 제출한다.
/// - 종을 칠 때마다 가장 높은 더미를 제출하고 보드를 비운다 (손님은 그대로).
/// - 제출마다 PD의 요구사항으로 점수(재료 점수 합 × 2/0.5)를 계산해 누적한다.
/// - 요구사항을 충족한 버거가 goalCount개 이상이면 충족(기쁨), 아니면 미충족(화남).
/// </summary>
public class Stage5Mode : MonoBehaviour
{
    public static Stage5Mode Inst;

    public TMP_Text timeText;
    public TMP_Text countText;
    public float limit = 60f;
    public int goalCount = 10;
    public CanvasGroup previewInCounter;

    private float time;
    private int count;          // 요구사항을 충족한 버거 수
    private int earnedScore;    // 이번 PD 손님에게 받은 점수 합
    private bool timerOn;
    private bool ended;
    private AudioClip timeUpClip;

    private void Awake()
    {
        var preview = GameObject.Find("PreviewI");
        if (preview != null) previewInCounter = preview.GetComponent<CanvasGroup>();
        Inst = this;
        timeUpClip = Resources.Load<AudioClip>("SFX/Stage5TimeUp");
    }

    private void Update()
    {
        if (!timerOn) return;

        time -= Time.deltaTime;
        if (time <= 0f)
        {
            time = 0f;
            EndStage(true);
            return;
        }

        ShowTime();
    }

    public bool IsOn()
    {
        CustomerData customer = StageFlowManager.Inst.CustomerQueueManager.GetCurrentCustomer();
        return customer != null && customer.CustomerName == "PD";
    }

    public bool IsPlaying()
    {
        return timerOn && !ended;
    }

    public void StartTimer()
    {
        if (!IsOn()) return;
        if (timeText == null)
        {
            timeText = CalenderCanvas.Inst.GetText();
        }

        count = 0;
        earnedScore = 0;
        time = limit;
        ended = false;
        timerOn = true;

        EnsureCountText();

        countText.gameObject.SetActive(true);
        countText.text = "Perfect\nBurger\nX 0";

        StartCoroutine(CalenderCanvas.Inst.PlayTimerRoutine(Mathf.CeilToInt(time)));
        ShowTime();
    }

    public bool Submit()
    {
        if (!IsOn()) return false;
        if (ended) return true;

        CustomerData customer = StageFlowManager.Inst.CustomerQueueManager.GetCurrentCustomer();
        List<Ingredient> burger = GameManager.Inst.TakeBestStack();   // 보드를 비우고 계속 진행
        OrderResult result = ScoreRules.Evaluate(customer.Requirement, burger);
        int score = ScoreRules.Calculate(burger, result);
        earnedScore += score;

        if (result == OrderResult.Satisfied)
        {
            count++;

            if (countText != null)
            {
                countText.text = $"Perfect\nBurger\nX {count}";
            }
        }

        Debug.Log($"[Stage5] burger: {result}, +{score}점, count: {count}, submitted: {string.Join(" > ", burger)}");

        // 보드가 비면서 카운터 미리보기도 GameManager가 바로 지운다
        return true;
    }

    private void EndStage(bool timeUp = false)
    {
        if (ended) return;

        ended = true;
        timerOn = false;
        AdvancedMain.Inst.enableSubmit = false;
        GameManager.Inst.EndRound();

        StartCoroutine(EndStageRoutine(timeUp));
    }

    private IEnumerator EndStageRoutine(bool timeUp)
    {

        if (timeUp)
        {
            ShowTime();
            SFXPlayer.Instance.Play(timeUpClip);
            yield return new WaitForSeconds(1f);
        }

        if (countText != null)
        {
            countText.gameObject.SetActive(false);
        }

        OrderResult result = count >= goalCount ? OrderResult.Satisfied : OrderResult.Unsatisfied;

        CustomerData oldCustomer = StageFlowManager.Inst.CustomerQueueManager.GetCurrentCustomer();

        MainUIManager.Inst.CloseGameView();
        GameManager.Inst.TakeBestStack();   // 남은 재료는 버린다
        if (previewInCounter != null) previewInCounter.DOFade(0, 0.2f);
        yield return StartCoroutine(SideBurgerMaker.Inst.FadeOutPreviewRoutine());

        int stageScoreBefore = StageFlowManager.Inst.ScoreCalculationSystem.stageScore;
        StageFlowManager.Inst.FinishStage5(result, earnedScore);
        CustomerStateManager.Inst.UpdateEmotionUI(StageFlowManager.Inst.oldEmotion);

        if (oldCustomer != null && oldCustomer.GetResultDialogue(result, out string dialogue))
        {
            var txts = dialogue.Split('\n');
            AdvancedDialogue.Inst.SetTexts(txts);
            AdvancedDialogue.Inst.ShowNextDialogue();
        }
        else
        {
            AdvancedDialogue.Inst.isDialogEnd = true;
            yield return new WaitForSeconds(1f);
        }

        yield return new WaitUntil(() => AdvancedDialogue.Inst.isDialogEnd);

        CustomerStateManager.Inst.HideCustomer();

        yield return new WaitForSeconds(1f);

        EndScreen.Inst.ShowEndScreen(stageScoreBefore + earnedScore, AdvancedMain.Inst.expectedScorePerCustomer * goalCount);
        StageFlowManager.Inst.ScoreCalculationSystem.ResetStageScore();
        CalenderCanvas.Inst.SetDayTxt(StageFlowManager.Inst.currentStageIndex-1);
    }

    private void ShowTime()
    {
        if (timeText == null) return;
        CalenderCanvas.Inst.SetTimerTxt(Mathf.CeilToInt(time));
    }

    public void SetTimeForTest(float seconds)
    {
        if (!timerOn || ended) return;

        time = seconds;
        ShowTime();
    }

    private void EnsureCountText()
    {
        if (countText != null) return;

        if (CalenderCanvas.Inst == null)
        {
            Debug.LogWarning("Stage5CountText: CalenderCanvas not found.");
            return;
        }

        TMP_Text baseText = CalenderCanvas.Inst.GetText();
        GameObject obj = new GameObject("Stage5CountText");
        obj.transform.SetParent(CalenderCanvas.Inst.transform, false);

        countText = obj.AddComponent<TextMeshProUGUI>();
        countText.font = baseText.font;
        countText.fontSize = 56;
        countText.color = Color.white;
        countText.alignment = TextAlignmentOptions.Center;
        countText.raycastTarget = false;

        RectTransform rect = countText.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(320f, 80f);
        rect.anchoredPosition = new Vector2(409f, -387f);
    }
}
