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
    private CustomerRuntimeState _oldstate;

    [Header("SPECIAL CUSTOMER")] 
    public Stage1Customer stage1Customer;
    
    private CustomerRuntimeState _currentCustomerState;
    public bool allStageEnded;
    public bool enableSubmit;
    private bool _stageEnded;

    private void Awake()
    {
        Inst = this;
        
        GameEvents.OnNewCustomerAppeared += GameEventsOnOnNewCustomerAppeared;
        GameEvents.OnStageChanged += GameEventsOnOnStageChanged;
        GameEvents.OnAllStagesCleared += GameEventsOnOnAllStagesCleared;
    }

    private void Start()
    {
        StartFlow();
    }

    public void StartFlow()
    {
        if (StageFlowManager.Inst.isAllGameCleared || allStageEnded)
        {
            StopAllCoroutines();
            StartCoroutine(CorPlayFinalVIPEnding());
            return;
        }

        StartCoroutine(CorStartGame());
    }

    IEnumerator CorPlayFinalVIPEnding()
    {
        if (EndScreen.Inst != null) EndScreen.Inst.gameObject.SetActive(false);

        int totalScore = StageFlowManager.Inst.ScoreCalculationSystem.totalReputation;
        EndingManager.Inst.PlayEnding(totalScore);

        AdvancedDialogue.Inst.isDialogEnd = false;
        AdvancedDialogue.Inst.ShowNextDialogue();

        var chatRect = AdvancedDialogue.Inst.chatImg.GetComponent<RectTransform>();
        chatRect.anchoredPosition3D = chatRect.anchoredPosition3D.SetY(533);
        var previewRect = AdvancedDialogue.Inst.previewBg.GetComponent<RectTransform>();
        previewRect.anchoredPosition3D = previewRect.anchoredPosition3D.SetY(670);

        while (!AdvancedDialogue.Inst.isDialogEnd)
        {
            if (Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                AdvancedDialogue.Inst.ShowNextDialogue();
            }
            yield return null;
        }

        AdvancedDialogue.Inst.CloseChat();
        CustomerStateManager.Inst.HideCustomer();

        yield return new WaitForSeconds(0.5f);

        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.KeypadEnter));

        UnityEngine.SceneManagement.SceneManager.LoadScene("IntroScene"); 
    }
    
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

        if (current == null)
        {
            yield break;
        }

        var currentState = StageFlowManager.Inst.CustomerQueueManager.GetCurrentCustomerState();
        _oldstate = currentState;
        Debug.Log($"오래된성별:{_oldstate.Appearance.Gender}");    
        CustomerStateManager.Inst.ShowCustomer(current, _currentCustomerState);
        if (!string.IsNullOrEmpty(current.GetDialogue()))
        {
            var texts = current.GetDialogue().Split('\n');
            AdvancedDialogue.Inst.SetTexts(texts);    
        }
        
        List<IngredientType> types = new();
        
        foreach (var t in current.Recipe)
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
        AdvancedDialogue.Inst.CloseChat();
    }
    
    //제출 버튼
    public void OnClickSubmit()
    {
        if(!enableSubmit)
            return;
        SFXPlayer.Instance.Play(bellClip);
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
     
        int clampedStageIndex = Mathf.Clamp(StageFlowManager.Inst.currentStageIndex, 0, StageFlowManager.Inst.Stages.Count - 1);
        StageData timeCapsuleStage = StageFlowManager.Inst.Stages[clampedStageIndex];
        int myTargetCount = timeCapsuleStage.TargetClearCount;
        int nextServedCount = StageFlowManager.Inst.servedCount + 1;
        bool isAllCleared = ((clampedStageIndex == (StageFlowManager.Inst.Stages.Count - 1)) && (nextServedCount >= myTargetCount))
                            || StageFlowManager.Inst.isAllGameCleared;
        var data = GameManager.Inst.GetBestBurgerData();
        GameManager.Inst.OnSubmitInput();
        var oldCustomer = StageFlowManager.Inst.CustomerQueueManager.GetCurrentCustomer();
        var result = StageFlowManager.Inst.OnBurgerSubmitted(data); 
        
        //CustomerStateManager.Inst.UpdateEmotionUI(_currentCustomerState.CurrentEmotion);
        //표정, 대사 적용
        Debug.Log($"현재 평판: {StageFlowManager.Inst.ScoreCalculationSystem.customerReputation}");
        Debug.Log($"스테이지 평판: {StageFlowManager.Inst.ScoreCalculationSystem.stageReputation}");
        Debug.Log($"전체 평판: {StageFlowManager.Inst.ScoreCalculationSystem.totalReputation}");

        
        Debug.Log($"종류: {oldCustomer}");
        if (oldCustomer is DefaultCustomerData)
        {
            Debug.Log($"현재 손님 성별: {_oldstate.Appearance.Gender}");
            CustomerStateManager.Inst.UpdateEmotionUI(
                StageFlowManager.Inst.oldEmotion,

                _oldstate.Appearance
            );
        }
        else
        {
            CustomerStateManager.Inst.UpdateEmotionUI(StageFlowManager.Inst.oldEmotion);
        }
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

        int totalScore = StageFlowManager.Inst.ScoreCalculationSystem.totalReputation;
        int stageScore = StageFlowManager.Inst.ScoreCalculationSystem.stageReputation;
        int maxScore = 0;

        foreach (var customer in timeCapsuleStage.CustomerPool)
        {
            maxScore += 30;
            if (customer is SpecialCustomerData) maxScore += 15;
        }

        if (isAllCleared)
        {
            StageFlowManager.Inst.isAllGameCleared = true;
            allStageEnded = true;

            EndScreen.Inst.ShowEndScreen(stageScore, maxScore);

            yield break;
        }
        else if (_stageEnded)
        {
            EndScreen.Inst.ShowEndScreen(stageScore, maxScore);

            StageFlowManager.Inst.ScoreCalculationSystem.ResetStageReputation();
        }
        else
        {
            StartFlow();
        }
    }

}
