using UnityEngine;

// 개수별 손님 state 종류
[System.Serializable]
public class GenderSpriteSet
{
    [Header("Appearance Types")]
    public Sprite[] bodyTypes;
    public Sprite[] clothesTypes;
    public Sprite[] hairTypes;
    public Sprite[] faceTypes;
}

[CreateAssetMenu(fileName = "CustomerStateDB", menuName = "Objects/CustomerStateDB")]

public class CustomerStateDB : ScriptableObject
{
    [Header("Gender Specific Sets")]
    public GenderSpriteSet maleSet;
    public GenderSpriteSet femaleSet;

    [Header("Common Sprites")]
    public Sprite[] emotionSprites;

    public GenderSpriteSet GetSpriteSet(CustomerGender gender)
    {
        return gender == CustomerGender.Male ? maleSet : femaleSet;
    }
}

public enum CustomerEmotion { Happy, Neutral, Angry };
