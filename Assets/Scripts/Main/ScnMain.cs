using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScnMain : MonoBehaviour
{
    public static ScnMain Inst;
    
    public Tong tong;
    public MainBG mainBG;
    public TextMeshProUGUI dayText;
    public Button submitBtn;
    public bool allStageEnded;
    
    private CustomerData _currentCustomerData;
    private bool _stageEnded;
    
    private void Awake()
    {
        Inst = this;
        
        GameEvents.OnNewCustomerAppeared += GameEventsOnOnNewCustomerAppeared;
        GameEvents.OnStageChanged += GameEventsOnOnStageChanged;
        GameEvents.OnAllStagesCleared += GameEventsOnOnAllStagesCleared;
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

    private void GameEventsOnOnNewCustomerAppeared(CustomerData data)
    {
        _currentCustomerData = data;
    }

    private void Start()
    {
        StartFlow();
    }

    public void StartFlow()
    {
        StartCoroutine(CorCustomerFlow());
    }
    
    //손님등장 ~ 햄버거 표시
    IEnumerator CorCustomerFlow()
    {
        tong.enableDrag = false;
        submitBtn.interactable = false;
        tong.ResetTongPos();

        dayText.text = $"Day {StageFlowManager.Inst.servedCount + 1} | Stage {StageFlowManager.Inst.currentStageIndex + 1}";
        yield return new WaitForSeconds(3);
        if (!_currentCustomerData)
            yield break;
        
        PeopleManager.Inst.ShowPeople();
        yield return new WaitForSeconds(1);
        string chat = $"[{_currentCustomerData.customerName}]\n안녕하세요!\n\n[버거 이름]을 주세요!";
        PeopleManager.Inst.ShowChat(chat);
        yield return new WaitForSeconds(5);
        PeopleManager.Inst.ShowHamburger(_currentCustomerData.Recipe);
        yield return new WaitForSeconds(1.5f);
        tong.enableDrag = true;
        submitBtn.interactable = true;
    }

    //제출 버튼눌렀을떄
    public void SubmitBurger()
    {
        StartCoroutine(CorSubmitBurger());
    }

    IEnumerator CorSubmitBurger()
    {
        _stageEnded = false;
        mainBG.rect.DOAnchorPos3D(Vector3.zero, 1f);
        yield return new WaitForSeconds(1);
        Debug.Log("FALSE");
        var data = GameManager.Inst.GetBestBurgerData();
        GameManager.Inst.OnSubmitInput();
        StageFlowManager.Inst.OnBurgerSubmitted(data); 
        Debug.Log(_stageEnded);
        //표정, 대사 적용
        PeopleManager.Inst.ShowChat($"평판: {StageFlowManager.Inst.scoreCalculationSystem.currentReputation}");
        yield return new WaitForSeconds(3);
        PeopleManager.Inst.HidePeople();
        yield return new WaitForSeconds(1);
        if (_stageEnded)
        {
            EndScreen.Inst.ShowEndScreen(StageFlowManager.Inst.scoreCalculationSystem.currentReputation);
        }
        else
        {
            StartCoroutine(CorCustomerFlow());    
        }
        
    }
}
