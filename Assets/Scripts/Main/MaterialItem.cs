using UnityEngine;
using UnityEngine.UI;

public class MaterialItem : MonoBehaviour
{
    public Image img;
    
    public void Init(IngredientType ingredientType)
    {
        if (VisualManager.Inst)
        {
            img.color = VisualManager.Inst.GetIngredientColor(ingredientType);
        }
    }
}
