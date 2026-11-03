// 이름과 값은 기존 IngredientType(RawPatty=-1, CookedPatty=1, Cheese=2, Onion=3, Lettuce=4, Tomato=5)과 같다.
// 기존 씬/에셋에 저장된 재료 값과 SideBurgerMaker.burgerSprites 순서를 그대로 쓸 수 있다.
// (RawPatty = 얼린 패티. 스프라이트 이름은 FrozenPatty)
public enum Ingredient
{
    RawPatty = -1,
    CookedPatty = 1,
    Cheese = 2,
    Onion = 3,
    Lettuce = 4,
    Tomato = 5,
}

public enum Direction
{
    Up,
    Down,
    Left,
    Right,
}
