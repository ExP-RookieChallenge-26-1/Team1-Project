using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Collections;

public class Stage5Mode : MonoBehaviour
{
    public static Stage5Mode Inst;

    public TMP_Text timeText;
    public TMP_Text countText;
    public float limit = 60f;
    public int goalCount = 10;

    private float time;
    private int count;
    private bool timerOn;
    private bool ended;
    private bool submitting;
    private AudioClip timeUpClip;

    private readonly List<IngredientType> order = new()
    {
        IngredientType.Tomato,
        IngredientType.Lettuce,
        IngredientType.CookedPatty,
        IngredientType.Cheese
    };

    private void Awake()
    {
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

    public List<IngredientType> GetOrder()
    {
        return new List<IngredientType>(order);
    }

    public void StartTimer()
    {
        if (!IsOn()) return;
        if (timeText == null)
        {
            timeText = CalenderCanvas.Inst.GetText();
        }

        count = 0;
        time = limit;
        ended = false;
        timerOn = true;
        
        EnsureCountText();

        countText.gameObject.SetActive(true);
        countText.text = "X 0";

        StartCoroutine(CalenderCanvas.Inst.PlayTimerRoutine(Mathf.CeilToInt(time)));
        ShowTime();
    }

    public void SetOrder()
    {
        if (!IsOn()) return;

        GameManager.Inst.orderList = new List<IngredientType>(order);
        GameManager.Inst.UpdateOrderVisual();
    }

    public bool Submit()
    {
        if (!IsOn()) return false;
        if (ended) return true;
        if (submitting) return true;

        IReadOnlyList<IngredientData> burger = GameManager.Inst.GetBestBurgerData();
        ReputationResult result = Check(burger);
        if (result == ReputationResult.Perfect)
        {
            count++;

            if (countText != null)
            {
                countText.text = $"X {count}";
            }
        }

        Debug.Log($"[Stage5] burger: {result}, count: {count}, submitted: {BurgerToText(burger)}");

        StartCoroutine(SubmitRoutine());
        return true;
    }

    private ReputationResult Check(IReadOnlyList<IngredientData> burger)
    {
        if (burger.Count != order.Count) return ReputationResult.Wrong;

        if (HasSameOrder(burger, false) || HasSameOrder(burger, true))
        {
            return ReputationResult.Perfect;
        }

        if (HasSameIngredients(burger))
        {
            return ReputationResult.Incomplete;
        }

        return ReputationResult.Wrong;
    }

    private bool HasSameOrder(IReadOnlyList<IngredientData> burger, bool reverse)
    {
        bool sameOrder = true;
        for (int i = 0; i < order.Count; i++)
        {
            int index = reverse ? order.Count - 1 - i : i;
            if (burger[index].IngredientType != order[i])
            {
                sameOrder = false;
                break;
            }
        }

        return sameOrder;
    }

    private bool HasSameIngredients(IReadOnlyList<IngredientData> burger)
    {
        List<IngredientType> target = new List<IngredientType>(order);
        foreach (IngredientData data in burger)
        {
            if (!target.Remove(data.IngredientType))
            {
                return false;
            }
        }

        return target.Count == 0;
    }

    private string BurgerToText(IReadOnlyList<IngredientData> burger)
    {
        List<string> names = new List<string>();
        foreach (IngredientData data in burger)
        {
            names.Add(data.IngredientType.ToString());
        }

        return string.Join(" > ", names);
    }

    private void EndStage(bool timeUp = false)
    {
        if (ended) return;

        ended = true;
        timerOn = false;
        AdvancedMain.Inst.enableSubmit = false;

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

        ReputationResult result = ReputationResult.Wrong;
        int reward = 0;

        if (count >= goalCount)
        {
            result = ReputationResult.Perfect;
            reward = 200;
        }
        else if (count >= 5)
        {
            result = ReputationResult.Incomplete;
            reward = 150;
        }

        int finalScore = StageFlowManager.Inst.ScoreCalculationSystem.totalReputation + reward;
        CustomerData oldCustomer = StageFlowManager.Inst.CustomerQueueManager.GetCurrentCustomer();

        MainUIManager.Inst.CloseGameView();
        GameManager.Inst.OnResetInput();

        StageFlowManager.Inst.FinishStage5(result, reward);
        CustomerStateManager.Inst.UpdateEmotionUI(StageFlowManager.Inst.oldEmotion);

        if (oldCustomer != null && oldCustomer.GetReputationDialogue(result, out string dialogue))
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

        EndScreen.Inst.ShowEndScreen(finalScore, 200);
        CalenderCanvas.Inst.SetDayTxt(StageFlowManager.Inst.currentStageIndex);
    }

    private void ShowTime()
    {
        if (timeText == null) return;
        CalenderCanvas.Inst.SetTimerTxt(Mathf.CeilToInt(time));
    }

    private IEnumerator SubmitRoutine()
    {
        submitting = true;

        GameManager.Inst.OnSubmitInput();

        yield return StartCoroutine(SideBurgerMaker.Inst.FadeOutPreviewRoutine());

        SetOrder();
        GameManager.Inst.SpawnNextIngredient();
        submitting = false;
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
        countText.color = Color.black;
        countText.alignment = TextAlignmentOptions.Center;
        countText.raycastTarget = false;

        RectTransform rect = countText.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(160f, 80f);
        rect.anchoredPosition = new Vector2(350f, -255f);
    }
}
