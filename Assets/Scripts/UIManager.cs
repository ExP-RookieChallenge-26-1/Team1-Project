using UnityEngine;

public class UIManager : MonoBehaviour
{
    private GameManager gameManager;

    void Start()
    {
        gameManager = GetComponent<GameManager>();
    }

    public void OnClickAddIngredient(int ingredientNumber)
    {
        if (gameManager.isPlaying) return; 

        Ingredient selected = (Ingredient)ingredientNumber;
        gameManager.orderList.Add(selected);
        
        // 텍스트 대신, VisualManager에게 그림을 새로 쌓으라고 지시!
        gameManager.UpdateOrderVisual(); 
    }

    public void OnClickStart()
    {
        gameManager.StartGame();
    }
}