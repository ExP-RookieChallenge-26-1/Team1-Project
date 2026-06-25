using UnityEngine;

[CreateAssetMenu(fileName = "CustomerStateDB", menuName = "Objects/CustomerStateDB")]

// default 손님 종류 이미지
public class CustomerStateDB : ScriptableObject
{
    [Header("Gender Sprites (Male, Female)")]
    public Sprite[] genderSprites;

    [Header("Skin Sprites (Light, Medium, Tan, Dark)")]
    public Sprite[] skinSprites;

    [Header("Emotion Sprites (Happy, Neutral, Angry)")]
    public Sprite[] emotionSprites;

    [Header("Hair Sprites (Style1, Style2, Style3)")]
    public Sprite[] hairSprites;

    [Header("Clothes Sprites (Style1, Style2, Style3)")]
    public Sprite[] clothesSprites;
}
