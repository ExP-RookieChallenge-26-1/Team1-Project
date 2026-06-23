using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private GameManager gameManager;

    void Start()
    {
        gameManager = GetComponent<GameManager>();
        CreateRestartButton();
    }

    public void OnClickAddIngredient(int ingredientNumber)
    {
        if (gameManager.isPlaying) return; 

        IngredientType selected = (IngredientType)ingredientNumber;
        gameManager.orderList.Add(selected);
        
        // 텍스트 대신, VisualManager에게 그림을 새로 쌓으라고 지시!
        gameManager.UpdateOrderVisual(); 
    }

    public void OnClickStart()
    {
        gameManager.StartGame();
    }

    public void OnClickRestartStage()
    {
        gameManager.RestartStage();
    }

    private void CreateRestartButton()
    {
        if (GameObject.Find("RestartStageButton") != null) return;

        Canvas canvas = FindRestartButtonCanvas();
        if (canvas == null)
        {
            Debug.LogWarning("Restart button could not find an active Canvas.");
            return;
        }

        GameObject buttonObject = new GameObject("RestartStageButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(canvas.transform, false);
        buttonObject.transform.SetAsLastSibling();

        RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(1f, 1f);
        buttonRect.anchorMax = new Vector2(1f, 1f);
        buttonRect.pivot = new Vector2(1f, 1f);
        buttonRect.anchoredPosition = new Vector2(-24f, -24f);
        buttonRect.sizeDelta = new Vector2(180f, 54f);

        Image buttonImage = buttonObject.GetComponent<Image>();
        buttonImage.color = new Color(0.14f, 0.14f, 0.14f, 0.9f);

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = buttonImage;
        button.onClick.AddListener(OnClickRestartStage);

        GameObject textObject = new GameObject("Text (TMP)", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(buttonObject.transform, false);

        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        TextMeshProUGUI label = textObject.GetComponent<TextMeshProUGUI>();
        label.text = "RESTART";
        label.fontSize = 24;
        label.alignment = TextAlignmentOptions.Center;
        label.color = Color.white;
        label.raycastTarget = false;
    }

    private Canvas FindRestartButtonCanvas()
    {
        Canvas namedCanvas = GameObject.Find("Canvas")?.GetComponent<Canvas>();
        if (namedCanvas != null && namedCanvas.isActiveAndEnabled)
        {
            return namedCanvas;
        }

        Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        Canvas bestCanvas = null;
        foreach (Canvas canvas in canvases)
        {
            if (!canvas.isActiveAndEnabled) continue;
            if (bestCanvas == null || canvas.sortingOrder > bestCanvas.sortingOrder)
            {
                bestCanvas = canvas;
            }
        }

        return bestCanvas;
    }
}
