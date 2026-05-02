using System.Collections.Generic;

// 타일 하나에 대한 설정
public class BurgerTile
{
    public List<Ingredient> stackedIngredients = new List<Ingredient>();

    public bool isGrill = false;

    public void AddIngredient(Ingredient newIngredient)
    {
        stackedIngredients.Add(newIngredient);
    }
}

public enum Ingredient
{
    Empty = 0,

    RawPatty = 10,
    CookedPatty = 11,

    Cheese = 20,
    Onion = 21,
    Lettuce = 22,
    Tomato = 23
}