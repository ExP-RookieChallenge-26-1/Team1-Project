using System.Collections.Generic;

// 타일 하나에 대한 설정
public class BurgerTile
{
    public List<IngredientType> stackedIngredients = new List<IngredientType>();

    public bool isGrill = false;

    public void AddIngredient(IngredientType newIngredient)
    {
        stackedIngredients.Add(newIngredient);
    }
}
