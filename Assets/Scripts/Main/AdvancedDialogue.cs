using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public enum DialogueType {Text, Preview}


public class Dialogue
{
    public DialogueType dialogueType;
    public string text;
    public bool isFakeRecipe;
    public List<Ingredient> types;

    public Dialogue(string t)
    {
        dialogueType = DialogueType.Text;
        text = t;
    }

    public Dialogue(List<Ingredient> t)
    {
        dialogueType = DialogueType.Preview;
        types = t;
    }
}

public class AdvancedDialogue : MonoBehaviour
{
    public static AdvancedDialogue Inst;
    public TextMeshProUGUI chatTxt;
    public CanvasGroup chatImg;
    public CanvasGroup previewBg;
    public AudioClip currentHummingClip;

    public bool isDialogEnd, blockDialogInput;
    public bool isPlayingEndingDialogue = false;
    /// <summary>
    /// 지금 대사가 주문 대사인지. 주문 대사가 끝날 때만 집게를 켠다.
    /// (결과 대사/엔딩 대사 뒤에 집게가 켜지면, 다음 손님이 없는데도 보드를 열 수 있게 된다)
    /// </summary>
    public bool enableTongOnEnd;

    public Dictionary<int, Action> actionByIndexDic;

    private List<Dialogue> _dialogues;
    private bool _isDialoging;
    private int _dialogueIndex;
    private bool _isFakePreview;

    private void Awake()
    {
        Inst = this;
    }
    
    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    private void Start()
    {
        chatImg.alpha = 0;
        previewBg.alpha = 0;
    }

    private void Update()
    {
        if (_isDialoging)
        {
            if (blockDialogInput)
                return;


            bool wasPressedThisFrame = false;
            foreach (var touch in Touch.activeTouches)
            {
                if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
                {
                    wasPressedThisFrame = true;
                }
            }

            if (Keyboard.current.anyKey.wasPressedThisFrame || wasPressedThisFrame)
            {
                if (_dialogueIndex + 1 > _dialogues.Count || _isFakePreview)
                {
                    _isDialoging = false;
                    isDialogEnd = true;
                    CloseChat();
                    print("대화 종료");

                    if (isPlayingEndingDialogue && !_isFakePreview)
                    {
                        Debug.Log("게임 종료");
                        return;
                    }

                    if (enableTongOnEnd) EnableTong();
                    actionByIndexDic = null;
                }

                else
                {
                    ShowNextDialogue();
                    if (_dialogueIndex > _dialogues.Count - 1 && enableTongOnEnd) EnableTong();
                }
            }
        }
    }

    static void EnableTong()
    {
        AdvancedMain.Inst.tong.enableDrag = true;
        AdvancedMain.Inst.tong.tongImg.sprite = AdvancedMain.Inst.tongs[1];
    }

    public void ResumeDialogue()
    {
        _isDialoging = true;
        isDialogEnd = false;
    }

    public void InsertFakePreview(List<Ingredient> list, int index)
    {
        _dialogues.Insert(index, new Dialogue(list) {isFakeRecipe = true});
    }

    public void SetTexts(string[] texts)
    {
        if (_dialogues == null)
            _dialogues = new List<Dialogue>();
        foreach (var t in texts)
        {
            _dialogues.Add(new Dialogue(t));
        }
        
        _isDialoging = true;
        isDialogEnd = false;
        _dialogueIndex = 0;
    }

    public void ShowNextDialogue()
    {
        if (_dialogueIndex >= _dialogues.Count) return;

        var dialogue = _dialogues[_dialogueIndex];
        _isFakePreview = dialogue.isFakeRecipe;
        if (dialogue.dialogueType == DialogueType.Text)
        {
            ShowTextChat(dialogue.text);
        }
        else
        {
            ShowPreviewChat(dialogue.types);
        }


        if (actionByIndexDic != null && actionByIndexDic.TryGetValue(_dialogueIndex, out var action))
        {
            action?.Invoke();
        }
        
        if(currentHummingClip != null)
            SFXPlayer.Instance.PlayRandomPitch(currentHummingClip);
        
        

        _dialogueIndex += 1;
    }
    void ShowTextChat(string text)
    {
        chatImg.alpha = 0;
        chatImg.DOFade(1, 0.3f);

        // 전체 텍스트 세팅
        chatTxt.text = text;

        // 최종 높이 계산
        chatTxt.ForceMeshUpdate();

        var bubbleRect = chatImg.GetComponent<RectTransform>();

        float targetHeight = chatTxt.preferredHeight + 40f; // 패딩

        // 시작 높이
        bubbleRect.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Vertical,
            40f);

        // 말풍선 확장
        bubbleRect.DOSizeDelta(
            new Vector2(bubbleRect.sizeDelta.x, targetHeight),
            0.3f
        ).SetEase(Ease.OutCubic);

        // 글자 숨기기
        chatTxt.maxVisibleCharacters = 0;

        // 타이핑 효과
        DOTween.To(
            () => chatTxt.maxVisibleCharacters,
            x => chatTxt.maxVisibleCharacters = x,
            text.Length,
            0.5f
        ).SetEase(Ease.Linear);
    }

    public void CloseChat()
    {
        chatImg.DOFade(0, 0.3f);
        previewBg.DOFade(0, 0.3f);

        if (!_isFakePreview)
        {
            _dialogueIndex = 0;
            _dialogues.Clear();   
        }
    }

    public void CloseChatOnlyVisual()
    {
        chatImg.DOFade(0, 0.3f);
        previewBg.DOFade(0, 0.3f);
    }

    public void ShowPreviewChat(List<Ingredient> stack)
    {
        chatImg.alpha = 0;
        SideBurgerMaker.Inst.Make(stack);
    }

    public void StartEndingDialogue(string[] endingText)
    {
        isPlayingEndingDialogue = true;
        SetTexts(endingText);

        ShowNextDialogue();
    }
}
