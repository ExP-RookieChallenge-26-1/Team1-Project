using UnityEngine;

public enum IngredientType
{
    FrozenPatty = 0,
    BakedPatty = 1,
    Cheese = 2,
    Onion = 3,
    Lettuce = 4,
    Tomato = 5,
    Burn = 100
}

[CreateAssetMenu(fileName = "Ingredient", menuName = "Objects/Ingredient")]
public class IngredientData : ScriptableObject
{
    [SerializeField] public IngredientType ingredientType; // 재료 종류
    public IngredientType IngredientType => ingredientType;
    [SerializeField] private string ingredientName; // 재료 이름
}
