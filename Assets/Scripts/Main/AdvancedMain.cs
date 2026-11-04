using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdvancedMain : MonoBehaviour
{
    public static AdvancedMain Inst;

    public AdvancedTong tong;
    public AudioClip unplugTongClip, swipeClip, cookedClip, mergeClip, bellClip, doorbellClip, doorbell2Clip;
    public CanvasGroup previewInCounter;

    [Header("SPECIAL CUSTOMER")] 
    public Stage1Customer stage1Customer;
    public Stage5Customer stage5Customer;
    
    private CustomerRuntimeState _currentCustomerState;
    public bool allStageEnded;
    public bool enableSubmit;
    private bool _stageEnded;

    [Header("제한시간 / 점수")]
    [Tooltip("손님 데이터에 제한시간이 없을 때(0 이하) 쓰는 손님당 제한시간(초)")]
    public float defaultTimeLimit = 60f;
    [Tooltip("결과 화면 별점 계산용: 손님 한 명에게 기대하는 점수 (스테이지 만점 = 이 값 × 손님 수)")]
    public int expectedScorePerCustomer = 1600;
    private CustomerTimer _timer;
    private bool _submitting;

    private void Awake()
    {
        Inst = this;
        if (Stage5Mode.Inst == null)
        {
            gameObject.AddComponent<Stage5Mode>();
        }
        _timer = GetComponent<CustomerTimer>();
        if (_timer == null) _timer = gameObject.AddComponent<CustomerTimer>();
        
        GameEvents.OnNewCustomerAppeared += GameEventsOnOnNewCustomerAppeared;
        GameEvents.OnStageChanged += GameEventsOnOnStageChanged;
    }

    private void Start()
    {
        // 손님 제한시간은 첫 재료를 꺼낼 때부터 줄어든다
        if (GameManager.Inst != null) GameManager.Inst.IngredientSpawned += OnIngredientSpawned;
    }

    private void OnIngredientSpawned(Ingredient ingredient)
    {
        _timer.BeginCountdown();
    }

    public void StartFlow()
    {
        if (StageFlowManager.Inst.isAllGameCleared)
        {
            StopAllCoroutines();
            int totalScore = StageFlowManager.Inst.ScoreCalculationSystem.totalScore;
            StartCoroutine(CorPlaySpecialEnding(totalScore));
            return;
        }

        StartCoroutine(CorStartGame());
    }
    

    IEnumerator CorPlaySpecialEnding(int totalScore)
    {
        int currentEndingIndex = 0;
        int enterCountForImage = 3;
        Sprite endingSprite = null;
        string endingTitle = "";
        string endingText = "";
        // 엔딩 기준 점수는 EndingManager 인스펙터에서 설정
        EndingTier tier = EndingManager.Inst.GetTier(totalScore);

        if (tier == EndingTier.Perfect)
        {
            currentEndingIndex = 0;
            enterCountForImage = 8;
            if (endingSprites.Length > 0) endingSprite = endingSprites[0];
            endingTitle = "퍼펙트 엔딩";
            endingText = "최고의 식당으로 등극했다.";
        }
        else if (tier == EndingTier.Good)
        {
            currentEndingIndex = 1;
            enterCountForImage = 8;
            if (endingSprites.Length > 1) endingSprite = endingSprites[1];
            endingTitle = "해피 엔딩";
            endingText = "당신의 노력은 결실을 맺었다.";
        }
        else if (tier == EndingTier.Normal)
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
            specialEndingCanvas.SetActive(true);
            StartCoroutine(PlayEndingStickerAnimation());

            yield return new WaitUntil(() =>
            {
                // 키보드
                if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
                    return true;

                // 터치
                foreach (var touch in Touch.activeTouches)
                {
                    if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
                        return true;
                }

                return false;
            });
            SaveEndingResetData(currentEndingIndex);
            StageFlowManager.Inst.CustomerQueueManager.ResetCustomerQueue();
            StageFlowManager.Inst.isAllGameCleared = false;
            fadeImg.DOFade(1, 0.8f).OnComplete(() =>
            {
                GoToGameScene();
            }); 
            yield break;
        }

        EndingManager.Inst.PlayEnding(totalScore);

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
                    
                    if (specialEndingCanvas != null)
                    {
                        specialEndingCanvas.SetActive(true);
                        StartCoroutine(PlayEndingStickerAnimation());
                        yield return new WaitUntil(() =>
                        {
                            // 키보드
                            if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
                                return true;

                            // 터치
                            foreach (var touch in Touch.activeTouches)
                            {
                                if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
                                    return true;
                            }

                            return false;
                        });
                        SaveEndingResetData(currentEndingIndex);
                        StageFlowManager.Inst.CustomerQueueManager.ResetCustomerQueue();
                        StageFlowManager.Inst.isAllGameCleared = false;
                        fadeImg.DOFade(1, 0.8f).OnComplete(() =>
                        {
                            GoToGameScene();
                        });
                        yield break;
                    }
                }

                else specialEndingCanvas.SetActive(false);
            }
            foreach (var touch in Touch.activeTouches)
            {
                if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
                {

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

                            if (specialEndingCanvas != null)
                            {
                                specialEndingCanvas.SetActive(true);
                                StartCoroutine(PlayEndingStickerAnimation());
                            }
                        }

                        else specialEndingCanvas.SetActive(false);
                    }
                }
              
            }
            yield return null;
        }

        yield return null;
        yield return new WaitUntil(() =>
        {
            // 키보드
            if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
                return true;

            // 터치
            foreach (var touch in Touch.activeTouches)
            {
                if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
                    return true;
            }

            return false;
        });
        CustomerStateManager.Inst.HideCustomer();
        AdvancedDialogue.Inst.CloseChat();

        yield return new WaitForSeconds(0.5f);

        
        SaveEndingResetData(currentEndingIndex);
        StageFlowManager.Inst.CustomerQueueManager.ResetCustomerQueue();
        StageFlowManager.Inst.isAllGameCleared = false;
        if (!_movingToIntro)
        {
            _movingToIntro = true;
            fadeImg.DOFade(1, 0.8f).OnComplete(() =>
            {
                GoToGameScene();
            });   
        }
    }


    void GoToGameScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Intro");
        if (PausedController.Inst)
        {
            PausedController.Inst.fullFadeImg.gameObject.SetActive(true);
            PausedController.Inst.fullFadeImg.color = Color.black;
            PausedController.Inst.fullFadeImg.DOFade(0, 1f).SetDelay(1.5f).OnComplete(() =>
            {
                PausedController.Inst.fullFadeImg.gameObject.SetActive(false);
            });
        }
    }

    IEnumerator PlayEndingStickerAnimation()
    {
        endingPanelBG.color = Color.black.SetAlpha(0.9f);
        var endingImg = specialEndingImage.GetComponent<RectTransform>();
        endingImg.localScale = Vector3.one * 0.7f;
        specialEndingImage.color = Color.white.SetAlpha(0);
        specialEndingText.color = specialEndingText.color.SetAlpha(0);
        specialEndingTitle.color = specialEndingTitle.color.SetAlpha(0);
                        
        yield return new WaitForSeconds(1);
                        
        DOTween.Sequence()
            .Join(endingImg.DOScale(1f, 0.3f).SetEase(Ease.OutBack))
            .Join(specialEndingImage.DOFade(1f, 0.2f)).OnComplete(delegate
            {
                specialEndingTitle.DOFade(1, 0.5f).SetDelay(0.5f);
                specialEndingText.DOFade(1, 0.5f).SetDelay(0.7f);
            }).Play();
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
        
        var current = StageFlowManager.Inst.CustomerQueueManager.GetCurrentCustomer();
        CustomerStateManager.Inst.ShowCustomer(current, _currentCustomerState);
        // 레시피 미리보기 대신 손님 대사로 요구사항을 전달한다: 손님 대사 → 요구사항 대사 순서.
        // 둘 다 비어 있으면 요구사항 설명(예: "Onion 빼기")을 대신 보여준다.
        string dialogueText = current.GetDialogue();
        string requirementText = current.Requirement?.dialogue;
        if (!string.IsNullOrEmpty(requirementText))
            dialogueText = string.IsNullOrEmpty(dialogueText) ? requirementText : dialogueText + "\n" + requirementText;
        if (string.IsNullOrEmpty(dialogueText))
            dialogueText = current.Requirement != null ? current.Requirement.Describe() : "버거 주세요!";
        AdvancedDialogue.Inst.SetTexts(dialogueText.Split('\n'));
        AdvancedDialogue.Inst.enableTongOnEnd = true;   // 주문 대사가 끝나면 집게를 켠다
        Debug.Log($"[손님] {current.CustomerName} | 요구사항: {current.Requirement?.Describe()}");

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
        GameManager.Inst.BeginRound();   // 보드를 비우고 재료 생성/슬라이드 시작
        // 6스테이지 가짜 주문 뒤에는 남은 주문 대사가 끝날 때 다시 집게를 켜야 하므로 유지한다
        if (!isFaking) AdvancedDialogue.Inst.enableTongOnEnd = false;
        if (Stage5Mode.Inst != null && Stage5Mode.Inst.IsOn())
        {
            Stage5Mode.Inst.StartTimer();   // PD는 자체 제한시간 기믹 사용
        }
        else if (!isFaking)
        {
            // 손님마다의 제한시간. 첫 재료를 꺼낼 때부터 줄어들고, 다 되면 지금 보드 상태 그대로 자동 제출
            var customer = StageFlowManager.Inst.CustomerQueueManager.GetCurrentCustomer();
            float limit = customer != null && customer.TimeLimit > 0 ? customer.TimeLimit : defaultTimeLimit;
            _timer.StartTimer(limit, OnTimeUp, waitForStart: true);
        }
        AdvancedDialogue.Inst.CloseChat();
    }

    //제출 버튼 (종)
    public void OnClickSubmit()
    {
        if(!enableSubmit)
            return;
        SFXPlayer.Instance.Play(bellClip);
        if (Stage5Mode.Inst != null && Stage5Mode.Inst.IsOn())
        {
            Stage5Mode.Inst.Submit();
            enableSubmit = Stage5Mode.Inst.IsPlaying();
            bellImg.sprite = bells[enableSubmit ? 1 : 0];
            return;
        }
        Submit();
    }

    // 제한시간 초과: 지금 보드에서 가장 높은 더미를 자동 제출
    private void OnTimeUp()
    {
        if (!enableSubmit) return;
        Submit();
    }

    private void Submit()
    {
        if (_submitting) return;
        _timer.Stop();
        StartCoroutine(CorSubmitBurger());
        enableSubmit = false;
    }

    IEnumerator CorSubmitBurger()
    {
        _submitting = true;
        _stageEnded = false;
        GameManager.Inst.EndRound();   // 제출 연출 중에는 재료 생성/슬라이드 금지
        MainUIManager.Inst.CloseGameView();
        previewInCounter.DOFade(0, 0.2f);
       
        yield return StartCoroutine(SideBurgerMaker.Inst.FallingRoutine());
        yield return new WaitForSeconds(2);

        if (isFaking)
        {
            // 6스테이지 가짜 주문: 제출한 버거는 점수 없이 버린다
            GameManager.Inst.TakeBestStack();
            _submitting = false;
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

        // 가장 높은 더미 1개 제출 (동점이면 오른쪽 위에 가까운 것) → 보드 전부 비우기
        var data = GameManager.Inst.TakeBestStack();
        var oldCustomer = StageFlowManager.Inst.CustomerQueueManager.GetCurrentCustomer();
        var result = StageFlowManager.Inst.OnBurgerSubmitted(data, out int earned);
        
        // 특수 손님 연출 분기: 충족 → Normal, 미충족 → Bad
        if(result == OrderResult.Satisfied)
            onResultNormal?.Invoke();
        else
            onResultBad?.Invoke();
        // 특수 손님은 자기 차례에 연출을 등록하므로 한 번 쓰고 비운다.
        // 비우지 않으면 다음 손님들의 결과 대사에도 이전 특수 손님(꺼진 오브젝트)의 연출이 끼어든다.
        onResultNormal = null;
        onResultBad = null;
        
        //표정, 대사 적용

        Debug.Log($"이번 손님 점수: {earned}");
        Debug.Log($"스테이지 점수: {StageFlowManager.Inst.ScoreCalculationSystem.stageScore}");
        Debug.Log($"전체 점수: {StageFlowManager.Inst.ScoreCalculationSystem.totalScore}");

        
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

        if (oldCustomer.GetResultDialogue(result, out string dialogue))
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
        CustomerStateManager.Inst.HideCustomer();
        
        yield return new WaitForSeconds(1);



        int totalScore = StageFlowManager.Inst.ScoreCalculationSystem.totalScore;
        int stageScore = StageFlowManager.Inst.ScoreCalculationSystem.stageScore;
        // 결과 화면 별점 기준: 손님당 기대 점수 × 손님 수
        int maxScore = expectedScorePerCustomer * timeCapsuleStage.CustomerPool.Count;
        _submitting = false;

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

            StageFlowManager.Inst.ScoreCalculationSystem.ResetStageScore();
        }
        else
        {
            StartFlow();
        }
    }

    private void SaveEndingResetData(int endingIndex)
    {
        float backupBGM = PlayerPrefs.GetFloat(DataLoader.BGM_KEY, DataLoader.DEFAULT_BGM_VALUE);
        float backupSFX = PlayerPrefs.GetFloat(DataLoader.SFX_KEY, DataLoader.DEFAULT_SFX_VALUE);

        int[] backupEndings = new int[4];
        for (int i=0; i<4; i++)
        {
            backupEndings[i] = PlayerPrefs.GetInt("SeenEnding: " + i, 0);
        }

        backupEndings[endingIndex] = 1;

        PlayerPrefs.DeleteAll();

        PlayerPrefs.SetFloat(DataLoader.BGM_KEY, backupBGM);
        PlayerPrefs.SetFloat(DataLoader.SFX_KEY, backupSFX);

        for (int i=0; i<4; i++)
        {
            if (backupEndings[i] == 1) PlayerPrefs.SetInt("SeenEnding: " + i, 1);
        }

        PlayerPrefs.Save();

        if (StageFlowManager.Inst != null && StageFlowManager.Inst.ScoreCalculationSystem != null)
        {
            StageFlowManager.Inst.ScoreCalculationSystem.totalScore = 0;
        }
    }
}
