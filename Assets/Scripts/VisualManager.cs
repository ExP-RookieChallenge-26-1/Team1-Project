using System.Collections.Generic;
using UnityEngine;

public class VisualManager : MonoBehaviour
{
    [Header("Visual Settings")]
    public GameObject ingredientPrefab;

    public Color colorRawPatty = Color.gray;
    public Color colorCookedPatty = new Color(0.4f, 0.2f, 0f);
    public Color colorCheese = Color.yellow;
    public Color colorOnion = Color.white;
    public Color colorLettuce = Color.green;
    public Color colorTomato = Color.red;

    private List<GameObject> activeVisuals = new List<GameObject>();
    private List<GameObject> previewVisuals = new List<GameObject>(); 
    private List<GameObject> orderVisuals = new List<GameObject>(); 

    // 화면 정중앙을 맞추기 위한 보정 좌표
    private float offsetX = -2.25f;
    private float offsetY = 3.0f;

    public void SetBackgroundColor()
    {
        Camera.main.clearFlags = CameraClearFlags.SolidColor;
        Camera.main.backgroundColor = new Color(0.9f, 0.95f, 1f); 
    }

    public void DrawBackgroundGrid()
    {
        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                GameObject bgTile = Instantiate(ingredientPrefab);
                
                // 오프셋을 적용해 정중앙으로 이동
                float posX = (x * 1.5f) + offsetX;
                float posY = (y * -1.5f) + offsetY;
                bgTile.transform.position = new Vector3(posX, posY, 0);

                SpriteRenderer sr = bgTile.GetComponent<SpriteRenderer>();
                if (y == 4) sr.color = new Color(0.2f, 0.2f, 0.2f);
                else sr.color = new Color(0.8f, 0.8f, 0.8f);

                sr.sortingOrder = -1;
            }
        }
    }

    public void UpdateVisuals(BurgerTile[,] gameBoard)
    {
        foreach (GameObject obj in activeVisuals) Destroy(obj);
        activeVisuals.Clear();

        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                List<Ingredient> stack = gameBoard[x, y].stackedIngredients;
                for (int i = 0; i < stack.Count; i++)
                {
                    GameObject newObj = Instantiate(ingredientPrefab);
                    activeVisuals.Add(newObj);

                    // 오프셋 적용
                    float posX = (x * 1.5f) + offsetX;
                    float posY = (y * -1.5f) + offsetY + (i * 0.2f);
                    newObj.transform.position = new Vector3(posX, posY, 0);

                    SpriteRenderer sr = newObj.GetComponent<SpriteRenderer>();
                    sr.color = GetIngredientColor(stack[i]);
                    sr.sortingOrder = i;
                }
            }
        }
    }

    public void DrawPreview(BurgerTile bestBurger)
    {
        foreach (GameObject obj in previewVisuals) Destroy(obj);
        previewVisuals.Clear();

        if (bestBurger == null || bestBurger.stackedIngredients.Count == 0) return;

        for (int i = 0; i < bestBurger.stackedIngredients.Count; i++)
        {
            GameObject newObj = Instantiate(ingredientPrefab);
            previewVisuals.Add(newObj);

            // 중앙 보드판 오른쪽 바깥에 띄움
            float posX = 4.5f; 
            float posY = 0f + (i * 0.2f);
            newObj.transform.position = new Vector3(posX, posY, 0);

            SpriteRenderer sr = newObj.GetComponent<SpriteRenderer>();
            sr.color = GetIngredientColor(bestBurger.stackedIngredients[i]);
            sr.sortingOrder = i;
        }
    }

    public void DrawOrderList(List<Ingredient> orderList)
    {
        foreach (GameObject obj in orderVisuals) Destroy(obj);
        orderVisuals.Clear();

        for (int i = 0; i < orderList.Count; i++)
        {
            GameObject newObj = Instantiate(ingredientPrefab);
            orderVisuals.Add(newObj);

            // 중앙 보드판 왼쪽 바깥에 띄움
            float posX = -4.5f; 
            float posY = -1.5f + (i * 0.2f);
            newObj.transform.position = new Vector3(posX, posY, 0);

            SpriteRenderer sr = newObj.GetComponent<SpriteRenderer>();
            sr.color = GetIngredientColor(orderList[i]);
            sr.sortingOrder = i;
        }
    }

    private Color GetIngredientColor(Ingredient type)
    {
        switch (type)
        {
            case Ingredient.RawPatty: return colorRawPatty;
            case Ingredient.CookedPatty: return colorCookedPatty;
            case Ingredient.Cheese: return colorCheese;
            case Ingredient.Onion: return colorOnion;
            case Ingredient.Lettuce: return colorLettuce;
            case Ingredient.Tomato: return colorTomato;
            default: return Color.white;
        }
    }
}