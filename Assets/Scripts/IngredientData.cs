using UnityEngine;

public enum IngredientType
{
    Burn = 0,
    TopBurn = 100,
    RawPatty = -1,
    CookedPatty = 1,
    Cheese = 2,
    Onion = 3,
    Lettuce = 4,
    Tomato = 5
}

[CreateAssetMenu(fileName = "Ingredient", menuName = "Objects/Ingredient")]
public class IngredientData : ScriptableObject
{
    [SerializeField] private IngredientType ingredientType; // ��� ����
    public IngredientType IngredientType
    {
        get => ingredientType; set => ingredientType = value;
    }
    [SerializeField] private string ingredientName; // ��� �̸�

}
