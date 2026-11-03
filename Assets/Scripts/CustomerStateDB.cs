using UnityEngine;

<<<<<<< Updated upstream
=======
// ê°œìˆ˜ë³„ ì†ë‹˜ state ì¢…ë¥˜
[System.Serializable]
public class GenderSpriteSet
{
    [Header("Appearance Types")]
    public Sprite[] bodyTypes;
    public Sprite[] clothesTypes;
    public Sprite[] hairTypes;
    public Sprite[] faceTypes;
}

>>>>>>> Stashed changes
[CreateAssetMenu(fileName = "CustomerStateDB", menuName = "Objects/CustomerStateDB")]

// default ¼Õ´Ô Á¾·ù ÀÌ¹ÌÁö
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
