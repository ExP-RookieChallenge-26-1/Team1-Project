using UnityEngine;

public class RandomAppearanceGenerator : ICustomerAppearanceGenerator
{
    private const int BODY_COUNT = 4;
    private const int CLOTHES_TYPE_COUNT = 9;
    private const int HAIR_TYPE_COUNT = 9;

    public CustomerAppearance Generate()
    {
        int genderRoll = Random.Range(0, 2);

        return new CustomerAppearance
        {
            Gender = (genderRoll == 0) ? CustomerGender.Male : CustomerGender.Female,
            BodyTypeIndex = Random.Range(0, BODY_COUNT),
            ClothesTypeIndex = Random.Range(0, CLOTHES_TYPE_COUNT),
            HairTypeIndex = Random.Range(0, HAIR_TYPE_COUNT)
        };
    }
}
