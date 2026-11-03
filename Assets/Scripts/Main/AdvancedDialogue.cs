using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
<<<<<<< Updated upstream
using UnityEngine.UI;
=======
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
>>>>>>> Stashed changes

public enum DialogueType {Text, Preview}


public class Dialogue
{
    public DialogueType dialogueType;
    public string text;
<<<<<<< Updated upstream
    public List<IngredientType> types;
=======
    public bool isFakeRecipe;
    public List<Ingredient> types;
>>>>>>> Stashed changes

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
<<<<<<< Updated upstream
    public RectTransform previewRect;
    public GameObject ingredientUIPrefab;

    public bool isDialogEnd, blockDialogInput;
    public float previewStackOffset = 18;
    
    [Header("Ingredient Sprites")]

    [Tooltip("구운 패티 이미지")]
    public Sprite spriteBakedPatty;

    [Tooltip("치즈 이미지")]
    public Sprite spriteCheese;

    [Tooltip("양파 이미지")]
    public Sprite spriteOnion;

    [Tooltip("양상추 이미지")]
    public Sprite spriteLettuce;

    [Tooltip("토마토 이미지")]
    public Sprite spriteTomato;

    [Tooltip("탄 재료 이미지")]
    public Sprite spriteUnderBurn, spriteTopBurn;

    public List<GameObject> chatIngredients;
=======
    public AudioClip currentHummingClip;

    public bool isDialogEnd, blockDialogInput;
    public bool isPlayingEndingDialogue = false;
    /// <summary>
    /// 지금 대사가 주문 대사인지. 주문 대사가 끝날 때만 집게를 켠다.
    /// (결과 대사/엔딩 대사 뒤에 집게가 켜지면, 다음 손님이 없는데도 보드를 열 수 있게 된다)
    /// </summary>
    public bool enableTongOnEnd;

    public Dictionary<int, Action> actionByIndexDic;

>>>>>>> Stashed changes
    private List<Dialogue> _dialogues;
    private bool _isDialoging;
    private int _dialogueIndex;
    
    private void Awake()
    {
        Inst = this;
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
            if(blockDialogInput)
                return;
            
            if (Keyboard.current.anyKey.wasPressedThisFrame)
            {
<<<<<<< Updated upstream
                if (_dialogueIndex + 1> _dialogues.Count)
=======
                if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
                {
                    wasPressedThisFrame = true;
                }
            }

            if ((Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame) || wasPressedThisFrame)
            {
                if (_dialogueIndex + 1 > _dialogues.Count || _isFakePreview)
>>>>>>> Stashed changes
                {
                    _isDialoging = false;
                    isDialogEnd = true;
                    CloseChat();
                    print("대화 종료");
<<<<<<< Updated upstream
                    AdvancedMain.Inst.tong.enableDrag = true;
=======

                    if (isPlayingEndingDialogue && !_isFakePreview)
                    {
                        Debug.Log("게임 종료");
                        return;
                    }

                    if (enableTongOnEnd) EnableTong();
                    actionByIndexDic = null;
>>>>>>> Stashed changes
                }
                else
                {
                    ShowNextDialogue();
<<<<<<< Updated upstream
                    if (_dialogueIndex > _dialogues.Count - 1)
                        AdvancedMain.Inst.tong.enableDrag = true;
=======
                    if (_dialogueIndex > _dialogues.Count - 1 && enableTongOnEnd) EnableTong();
>>>>>>> Stashed changes
                }
            }
        }
    }

<<<<<<< Updated upstream
=======
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

>>>>>>> Stashed changes
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

<<<<<<< Updated upstream
    public void SetPreview(List<IngredientType> list)
    {
        if (_dialogues == null)
            _dialogues = new List<Dialogue>();
        _dialogues.Add(new Dialogue(list));
        
        _isDialoging = true;
        isDialogEnd = false;
        _dialogueIndex = 0;
   
    }

=======
>>>>>>> Stashed changes
    public void ShowNextDialogue()
    {
        if (_dialogueIndex >= _dialogues.Count) return;
        
        var dialogue = _dialogues[_dialogueIndex];
        if (dialogue.dialogueType == DialogueType.Text)
        {
            ShowTextChat(dialogue.text);
        }
        else
        {
            ShowPreviewChat(dialogue.types);
        }
        
        
        _dialogueIndex += 1;
    }

<<<<<<< Updated upstream
    void ShowTextChat(string t)
    {
        chatImg.alpha = 0;
        chatImg.DOFade(1, 0.3f);
        chatTxt.text = "";
        chatTxt.DOText(t, 0.5f);
=======
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
>>>>>>> Stashed changes
    }

    public void CloseChat()
    {
        chatImg.DOFade(0, 0.3f);
        previewBg.DOFade(0, 0.3f);

        _dialogueIndex = 0;
        _dialogues.Clear();
    }

    public void ShowPreviewChat(List<Ingredient> stack)
    {
<<<<<<< Updated upstream
        foreach (var obj in chatIngredients)
        {
            Destroy(obj);
        }

        /*stack.Insert(0, IngredientType.Burn);
        stack.Add(IngredientType.TopBurn);*/


        SideBurgerMaker.Inst.Make(stack);
        //chatIngredients = SideBurgerRenderer.Inst.BuildBurger(stack, previewRect);
        
      
        chatImg.alpha = 0;
        /*previewBg.alpha = 0;
        previewBg.DOFade(1, 0.3f);*/
    }
    
    public void SetIngredientImage(GameObject targetObj, IngredientType type)
    {
        Image image = targetObj.GetComponent<Image>();

        if (image == null)
        {
            Debug.LogWarning("VisualManager: IngredientUI 프리팹에 Image 컴포넌트가 없습니다.");
            return;
        }

        image.sprite = GetIngredientSprite(type);
        image.color = Color.white;
        image.preserveAspect = true;
    }
    
    private Sprite GetIngredientSprite(IngredientType type)
    {
        switch (type)
        {

            case IngredientType.CookedPatty:
                return spriteBakedPatty;

            case IngredientType.Cheese:
                return spriteCheese;

            case IngredientType.Onion:
                return spriteOnion;

            case IngredientType.Lettuce:
                return spriteLettuce;

            case IngredientType.Tomato:
                return spriteTomato;

            case IngredientType.Burn:
                return spriteUnderBurn;
            case IngredientType.TopBurn:
                return spriteTopBurn;
            default:
                return null;
        }
=======
        chatImg.alpha = 0;
        SideBurgerMaker.Inst.Make(stack);
>>>>>>> Stashed changes
    }
}
