using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using Random = UnityEngine.Random;

using UnityEngine.InputSystem;

public class AdvancedMain : MonoBehaviour
{
    public static AdvancedMain Inst;

    public AdvancedTong tong;
    public AudioClip unplugTongClip, swipeClip, cookedClip, mergeClip, bellClip, doorbellClip, doorbell2Clip;
    public CanvasGroup previewInCounter;
    private CustomerRuntimeState _oldstate;
    public AudioClip[] manClips, womanClips;

    [Header("SPECIAL CUSTOMER")] 
    public Stage1Customer stage1Customer;
    public Stage2Customer stage2Customer;
    public Stage3Customer stage3Customer;
    public Stage4Customer stage4Customer;
    public Stage5Customer stage5Customer;
    public Stage6Customer stage6Customer;
    public bool isFaking;

    public Action onResultNormal;
    public Action onResultBad;
    
    private CustomerRuntimeState _currentCustomerState;
    public bool allStageEnded;
    public bool enableSubmit;
    private bool _stageEnded;
    public GameObject specialEndingPanel;
    public UnityEngine.UI.Image specialEndingImage;
    public TMPro.TextMeshProUGUI specialEndingTitle;
    public TMPro.TextMeshProUGUI specialEndingText;
    public Sprite[] endingSprites;

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

    public void StartFlow()
    {
        if (StageFlowManager.Inst.isAllGameCleared)
        {
            StopAllCoroutines();
            int totalReputation = StageFlowManager.Inst.ScoreCalculationSystem.totalReputation;
            StartCoroutine(CorPlaySpecialEnding(totalReputation));
            return;
        }

        StartCoroutine(CorStartGame());
    }

    IEnumerator CorPlaySpecialEnding(int totalReputation)
    {
        int currentEndingIndex = 0;
        int enterCountForImage = 3;
        Sprite endingSprite = null;
        string endingTitle = "";
        string endingText = "";

        if (totalReputation >= 750)
        {
            currentEndingIndex = 0;
            enterCountForImage = 8;
            if (endingSprites.Length > 0) endingSprite = endingSprites[0];
            endingTitle = "퍼펙트 엔딩";
            endingText = "최고의 식당으로 등극했다.";
        }
        else if (totalReputation >= 600)
        {
            currentEndingIndex = 1;
            enterCountForImage = 8;
            if (endingSprites.Length > 1) endingSprite = endingSprites[1];
            endingTitle = "해피 엔딩";
            endingText = "당신의 노력은 결실을 맺었다.";
        }
        else if (totalReputation >= 450)
        {
            currentEndingIndex = 2;
            enterCountForImage = 3;
            if (endingSprites.Length > 2) endingSprite = endingSprites[2];
            endingTitle = "노말 엔딩";
            endingText = "당신은 인기보다 더 가치있는 것을 얻었다.";
        }
        else
        {
            currentEndingIndex = 3;
            if (endingSprites.Length > 3) specialEndingImage.sprite = endingSprites[3];
            specialEndingTitle.text = "폐업 엔딩";
            specialEndingText.text = "당신은 최선을 다했지만 성공하지 못했다.";
            specialEndingPanel.SetActive(true);

            yield return new WaitUntil(() => Keyboard.current != null && (Keyboard.current.anyKey.wasPressedThisFrame));
            SaveEndingResetData(currentEndingIndex);
            StageFlowManager.Inst.CustomerQueueManager.ResetCustomerQueue();
            StageFlowManager.Inst.isAllGameCleared = false;
            UnityEngine.SceneManagement.SceneManager.LoadScene("Intro");
            yield break;
        }

        EndingManager.Inst.PlayEnding(totalReputation);

        int enterCount = 0;
        while (!AdvancedDialogue.Inst.isDialogEnd)
        {
            if (Keyboard.current != null && (Keyboard.current.anyKey.wasPressedThisFrame))
            {
                enterCount++;
                if (enterCount == enterCountForImage)
                {
                    if (specialEndingImage != null && endingSprite != null)
                    {
                        specialEndingImage.sprite = endingSprite;
                    }

                    if (specialEndingTitle != null)
                    {
                        specialEndingTitle.text = endingTitle;
                    }

                    if (specialEndingText != null)
                    {
                        specialEndingText.text = endingText;
                    }
                    
                    if (specialEndingPanel != null)
                    {
                        specialEndingPanel.SetActive(true);
                    }
                }

                else specialEndingPanel.SetActive(false);
            }
            yield return null;
        }

        yield return null;
        yield return new WaitUntil(() => Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame);
        CustomerStateManager.Inst.HideCustomer();
        AdvancedDialogue.Inst.CloseChat();

        yield return new WaitForSeconds(0.5f);

        yield return new WaitUntil(() => Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame);
        SaveEndingResetData(currentEndingIndex);
        StageFlowManager.Inst.CustomerQueueManager.ResetCustomerQueue();
        StageFlowManager.Inst.isAllGameCleared = false;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Intro");
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
        CalenderCanvas.Inst.SetDayTxt(StageFlowManager.Inst.currentStageIndex);
        yield return new WaitForSeconds(StageFlowManager.Inst.currentStageIndex == 0 && StageFlowManager.Inst.servedCount == 0 ? 3.5f : 1);
        var clip = UnityEngine.Random.Range(0, 2) == 0 ? doorbellClip : doorbell2Clip;
        SFXPlayer.Instance.Play(clip);
        yield return new WaitForSeconds(2);
        Debug.Log("WGY??????????????????????????" +StageFlowManager.Inst.currentStageIndex);
        
        //var current = StageFlowManager.Inst.CustomerQueueManager.GetNextCustomer();
        //Debug.Log(current == null);
        var current = StageFlowManager.Inst.CustomerQueueManager.GetCurrentCustomer();

        if (current == null)
        {
            yield break;
        }

        var currentState = StageFlowManager.Inst.CustomerQueueManager.GetCurrentCustomerState();
        _oldstate = currentState;
        Debug.Log($"오래된성별:{_oldstate.Appearance.Gender}");
        AdvancedDialogue.Inst.currentHummingClip = currentState.Appearance.Gender == CustomerGender.Male
            ? manClips[Random.Range(0, manClips.Length)]
            : womanClips[Random.Range(0, womanClips.Length)];
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

        AdvancedDialogue.Inst.actionByIndexDic = null;
        
        if (current.CustomerName == "Kid")
        {
            AdvancedDialogue.Inst.blockDialogInput = true;
            CustomerStateManager.Inst.currentSpecialCustomer = stage1Customer;
            stage1Customer.StartAnimation();
        }
        //5스테이지
        else if (current.CustomerName == "PD")
        {
            AdvancedDialogue.Inst.blockDialogInput = true;
            CustomerStateManager.Inst.currentSpecialCustomer = stage5Customer;
            stage5Customer.StartAnimation();
        }
        //3스테이지
        else if (current.CustomerName == "Influencer")
        {
            AdvancedDialogue.Inst.blockDialogInput = true;
            CustomerStateManager.Inst.currentSpecialCustomer = stage3Customer;
            stage3Customer.StartAnimation();
        }
        //4스테이지
        else if (current.CustomerName == "mukbang")
        {
            AdvancedDialogue.Inst.blockDialogInput = true;
            CustomerStateManager.Inst.currentSpecialCustomer = stage4Customer;
            stage4Customer.StartAnimation();
        }
        //6스테이지
        else if (current.CustomerName == "Jury")
        {
            AdvancedDialogue.Inst.blockDialogInput = true;
            CustomerStateManager.Inst.currentSpecialCustomer = stage6Customer;
            stage6Customer.StartAnimation();
        }
        //2스테이지
        else if (current.CustomerName == "Knight")
        {
            AdvancedDialogue.Inst.blockDialogInput = true;
            CustomerStateManager.Inst.currentSpecialCustomer = stage2Customer;
            stage2Customer.StartAnimation();
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

    public int DebugScore;

    IEnumerator CorSubmitBurger()
    {
        _stageEnded = false;
        MainUIManager.Inst.CloseGameView();
        previewInCounter.DOFade(0, 0.2f);
        /*foreach (Transform child in previewInCounter.transform)
        {
            if (child.TryGetComponent<Image>(out var img))
                img.color = img.color.SetAlpha(1);
        }*/
       
        yield return StartCoroutine(SideBurgerMaker.Inst.FallingRoutine());
        yield return new WaitForSeconds(2);

        if (isFaking)
        {
            AdvancedDialogue.Inst.ResumeDialogue();
            AdvancedDialogue.Inst.ShowNextDialogue();
            AdvancedDialogue.Inst.blockDialogInput = false;
            stage6Customer.SetActionsAfterFake();
            isFaking = false;
            tong.ResetTong();
            yield break;
        }

     
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
        
        if(result == ReputationResult.Perfect || result == ReputationResult.Incomplete)
            onResultNormal?.Invoke();
        else
            onResultBad?.Invoke();
        
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

    private void SaveEndingResetData(int endingIndex)
    {
        float backupBGM = PlayerPrefs.GetFloat("BgmVolume", 1f);
        float backupSFX = PlayerPrefs.GetFloat("SfxVolume", 1f);

        int[] backupEndings = new int[4];
        for (int i=0; i<4; i++)
        {
            backupEndings[i] = PlayerPrefs.GetInt("SeenEnding: " + i, 0);
        }

        backupEndings[endingIndex] = 1;

        PlayerPrefs.DeleteAll();

        PlayerPrefs.SetFloat("BgmVolume", backupBGM);
        PlayerPrefs.SetFloat("SfxVolume", backupSFX);

        for (int i=0; i<4; i++)
        {
            if (backupEndings[i] == 1) PlayerPrefs.SetInt("SeenEnding: " + i, 1);
        }

        PlayerPrefs.Save();

        if (StageFlowManager.Inst != null && StageFlowManager.Inst.ScoreCalculationSystem != null)
        {
            StageFlowManager.Inst.ScoreCalculationSystem.totalReputation = 0;
        }
    }
}
