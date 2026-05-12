using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Customer", menuName = "Objects/Customer")]
public class CustomerData : ScriptableObject
{
    [SerializeField] private string customerName;   // 손님 이름
    [SerializeField] private List<IngredientData> recipe;   // 주문한 메뉴
    public IReadOnlyList<IngredientData> Recipe => recipe.AsReadOnly();
}
