using UnityEngine;
using UnityEngine.UI;

/// <summary>하단 재료 버튼. 누르면 GameManager에 해당 재료 생성을 요청한다.</summary>
[RequireComponent(typeof(Button))]
public class IngredientButton : MonoBehaviour
{
    [SerializeField] Ingredient ingredient;
    [SerializeField] GameManager gameManager;

    public Button Button { get; private set; }

    void Awake()
    {
        Button = GetComponent<Button>();
        Button.onClick.AddListener(() => gameManager.SpawnIngredient(ingredient));
    }
}
