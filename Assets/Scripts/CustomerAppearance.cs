public enum CustomerGender { Male, Female }

[System.Serializable]
public struct CustomerAppearance
{
    public CustomerGender Gender;
    public int BodyTypeIndex;
    public int ClothesTypeIndex;
    public int HairTypeIndex;
    public int FaceTypeIndex;
}

public interface ICustomerAppearanceGenerator
{
    CustomerAppearance Generate();
}