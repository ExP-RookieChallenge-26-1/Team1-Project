using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum DialogueType {Text, Preview}


public class Dialogue
{
    public DialogueType dialogueType;
    public string text;
    public List<IngredientType> types;

    public Dialogue(string t)
    {
        dialogueType = DialogueType.Text;
        text = t;
    }

    public Dialogue(List<IngredientType> t)
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
    public RectTransform previewRect;
    public GameObject ingredientUIPrefab;

    public bool isDialogEnd;
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
            if (Keyboard.current.anyKey.wasPressedThisFrame)
            {
                if (_dialogueIndex + 1> _dialogues.Count)
                {
                    _isDialoging = false;
                    isDialogEnd = true;
                    CloseChat();
                    print("대화 종료");
                }
                else
                {
                    ShowNextDialogue();
                }
            }
        }
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

    public void SetPreview(List<IngredientType> list)
    {
        if (_dialogues == null)
            _dialogues = new List<Dialogue>();
        _dialogues.Add(new Dialogue(list));
        
        _isDialoging = true;
        isDialogEnd = false;
        _dialogueIndex = 0;
   
    }

    public void ShowNextDialogue()
    {
        
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

    void ShowTextChat(string t)
    {
        chatImg.alpha = 0;
        chatImg.DOFade(1, 0.3f);
        chatTxt.text = "";
        chatTxt.DOText(t, 0.5f);
    }

    public void CloseChat()
    {
        chatImg.DOFade(0, 0.3f);
        previewBg.DOFade(0, 0.3f);

        _dialogueIndex = 0;
        _dialogues.Clear();
    }

    public void ShowPreviewChat(List<IngredientType> stack)
    {
        foreach (var obj in chatIngredients)
        {
            Destroy(obj);
        }
        stack.Insert(0, IngredientType.TopBurn);
        stack.Add(IngredientType.Burn);
        for (int i = 0; i < stack.Count; i++)
        {
            GameObject newObj = Instantiate(ingredientUIPrefab, previewRect);
            //previewVisuals.Add(newObj);
            chatIngredients.Add(newObj);

            RectTransform rect = newObj.GetComponent<RectTransform>();

            if (rect != null)
            {
                // 프리뷰는 재료 순서를 보기 쉽도록 살짝 위로 쌓아서 보여준다.
                rect.anchoredPosition = new Vector2(0f, i * previewStackOffset);
                rect.localScale = Vector3.one;
            }

            SetIngredientImage(newObj, stack[i]);

            // 프리뷰도 가장 위 재료에만 전체 스택 개수를 표시한다.
            //SetStackText(newObj, i == stack.Count - 1 ? stack.Count.ToString() : "");

            newObj.transform.SetAsLastSibling();
        }
        
      
        chatImg.alpha = 0;
        previewBg.alpha = 0;
        previewBg.DOFade(1, 0.3f);
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
    }
}
