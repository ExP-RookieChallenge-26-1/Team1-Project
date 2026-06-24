using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class AdvancedMain : MonoBehaviour
{
    public static AdvancedMain Inst;

    public AdvancedTong tong;
    public AudioClip unplugTongClip, swipeClip, cookedClip, mergeClip, bellClip, doorbellClip;
    public CanvasGroup previewInCounter;

    [Header("SPECIAL CUSTOMER")] 
    public Stage1Customer stage1Customer;
    
    private CustomerRuntimeState _currentCustomerState;
    public bool allStageEnded;
    public bool enableSubmit;
    private bool _stageEnded;

    private void Awake()
    {
        Inst = this;
        if (Stage5Mode.Inst == null)
        {
            gameObject.AddComponent<Stage5Mode>();
        }
        
        GameEvents.OnNewCustomerAppeared += GameEventsOnOnNewCustomerAppeared;
        GameEvents.OnStageChanged += GameEventsOnOnStageChanged;
        GameEvents.OnAllStagesCleared += GameEventsOnOnAllStagesCleared;
    }

    private void Start()
    {
        StartFlow();
    }

    public void StartFlow() => StartCoroutine(CorStartGame());
    
    private void GameEventsOnOnAllStagesCleared()
    {
        _stageEnded = true;
        allStageEnded = true;
    }

    private void GameEventsOnOnStageChanged(int index)
    {
        _stageEnded = true;

    }

    private void GameEventsOnOnNewCustomerAppeared(CustomerRuntimeState customerState)
    {
        _currentCustomerState = customerState;
    }

    //손님 등장 ~ 뒤집개 드래그 전까지
    IEnumerator CorStartGame()
    {
        enableSubmit = false;
        tong.ResetTong();
        tong.enableDrag = false;
        yield return new WaitForSeconds(StageFlowManager.Inst.currentStageIndex == 0 && StageFlowManager.Inst.servedCount == 0 ? 3.5f : 1);
        SFXPlayer.Instance.Play(doorbellClip);
        yield return new WaitForSeconds(2);
        var current = StageFlowManager.Inst.CustomerQueueManager.GetCurrentCustomer();
        CustomerStateManager.Inst.ShowCustomer(current, _currentCustomerState);
        if (!string.IsNullOrEmpty(current.GetDialogue()))
        {
            var texts = current.GetDialogue().Split('\n');
            AdvancedDialogue.Inst.SetTexts(texts);    
        }
        
        List<IngredientType> types = new();
        
        if (current.CustomerName == "PD" && Stage5Mode.Inst != null)
        {
            types = Stage5Mode.Inst.GetOrder();
        }
        else foreach (var t in current.Recipe)
        {
            types.Add(t.IngredientType);
        }
        AdvancedDialogue.Inst.SetPreview(types);
        
        if (current.CustomerName == "Kid")
        {
            AdvancedDialogue.Inst.blockDialogInput = true;
            CustomerStateManager.Inst.currentSpecialCustomer = stage1Customer;
            stage1Customer.StartAnimation();
            /*AdvancedDialogue.Inst.blockDialogInput = true;
            specialCustomer.StartAnimation();

            var chatRect = AdvancedDialogue.Inst.chatImg.GetComponent<RectTransform>();
            chatRect.anchoredPosition3D = chatRect.anchoredPosition3D.SetY(198);
            
            var previewRect = AdvancedDialogue.Inst.previewBg.GetComponent<RectTransform>();
            previewRect.anchoredPosition3D = previewRect.anchoredPosition3D.SetY(341);*/
        }
        else if (current.CustomerName == "WOW")
        {
            //ETC
        }
        else
        {
            yield return new WaitForSeconds(1);
            AdvancedDialogue.Inst.ShowNextDialogue();    
            var chatRect = AdvancedDialogue.Inst.chatImg.GetComponent<RectTransform>();
            chatRect.anchoredPosition3D = chatRect.anchoredPosition3D.SetY(533);
            
            var previewRect = AdvancedDialogue.Inst.previewBg.GetComponent<RectTransform>();
            previewRect.anchoredPosition3D = previewRect.anchoredPosition3D.SetY(670);
        }
        
    }

    //뒤집개 드래그했을때
    public void OnEndDragTong()
    {
        enableSubmit = true;
        tong.enableDrag = false;
        SFXPlayer.Instance.Play(unplugTongClip);
        GameManager.Inst.StartGame();
        if (Stage5Mode.Inst != null && Stage5Mode.Inst.IsOn())
        {
            Stage5Mode.Inst.StartTimer();
        }
        AdvancedDialogue.Inst.CloseChat();
    }
    
    //제출 버튼
    public void OnClickSubmit()
    {
        if(!enableSubmit)
            return;
        SFXPlayer.Instance.Play(bellClip);
        if (Stage5Mode.Inst != null && Stage5Mode.Inst.IsOn())
        {
            Stage5Mode.Inst.Submit();
            enableSubmit = Stage5Mode.Inst.IsPlaying();
            return;
        }
        StartCoroutine(CorSubmitBurger());
        enableSubmit = false;
    }

    IEnumerator CorSubmitBurger()
    {
        _stageEnded = false;
        MainUIManager.Inst.CloseGameView();
        previewInCounter.DOFade(1, 0.5f);
        foreach (Transform child in previewInCounter.transform)
        {
            if (child.TryGetComponent<Image>(out var img))
                img.color = img.color.SetAlpha(1);
        }
        yield return new WaitForSeconds(2);
        var data = GameManager.Inst.GetBestBurgerData();
        GameManager.Inst.OnSubmitInput();
        var oldCustomer = StageFlowManager.Inst.CustomerQueueManager.GetCurrentCustomer();
        var result = StageFlowManager.Inst.OnBurgerSubmitted(data); 
        
        //CustomerStateManager.Inst.UpdateEmotionUI(_currentCustomerState.CurrentEmotion);
        //표정, 대사 적용
        Debug.Log($"평판: {StageFlowManager.Inst.ScoreCalculationSystem.CurrentReputation}");
        CustomerStateManager.Inst.UpdateEmotionUI(StageFlowManager.Inst.oldEmotion);
        if (oldCustomer.GetReputationDialogue(result, out string dialogue))
        {
            var txts = dialogue.Split('\n');
            AdvancedDialogue.Inst.SetTexts(txts);
            AdvancedDialogue.Inst.ShowNextDialogue();
        }
        else
        {
            AdvancedDialogue.Inst.isDialogEnd = true;
            yield return new WaitForSeconds(1);
        }


        yield return new WaitUntil(() => AdvancedDialogue.Inst.isDialogEnd);
        //StageFlowManager.Inst.CustomerQueueManager.GetCurrentCustomer().GetReputationDialogue(StageFlowManager.Inst.ScoreCalculationSystem.CurrentReputation);
        CustomerStateManager.Inst.HideCustomer();
        
        yield return new WaitForSeconds(1);

        if (_stageEnded)
        {
            int currentStageIndex = StageFlowManager.Inst.currentStageIndex;
            StageData currentStage = StageFlowManager.Inst.Stages[currentStageIndex];
            int myScore = StageFlowManager.Inst.ScoreCalculationSystem.CurrentReputation;
            Debug.Log(myScore);
            int maxScore = 0;
            foreach (var customer in currentStage.CustomerPool)
            {
                maxScore += 30;

                if (customer is SpecialCustomerData)
                {
                    maxScore += 15;
                }
            }
            EndScreen.Inst.ShowEndScreen(myScore, maxScore);
        }
        else
        {
            StartFlow();
        }
    }
    
}
