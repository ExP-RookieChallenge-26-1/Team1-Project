using UnityEngine;

public static class Utils
{
    public static bool GetRandomTxt(this string[] array, out string dialogue,  string defaultTxt = "")
    {
        if (array.Length == 0)
        {
            dialogue = defaultTxt;
            return false;
            
        }

        dialogue= array[Random.Range(0, array.Length)];
        return true;
    }
    
    public static Color SetAlpha(this Color c, float a)
    {
        return new Color(c.r, c.g, c.b, a);
    }

    public static Vector3 SetX(this Vector3 vec, float x)
    {
        return new Vector3(x, vec.y, vec.z);
    }
    
    public static Vector3 SetY(this Vector3 vec, float y)
    {
        return new Vector3(vec.x, y, vec.z);
    }
    
    public static Vector3 SetZ(this Vector3 vec, float z)
    {
        return new Vector3(vec.x, vec.y, z);
    }
    
    public static Vector3 AddX(this Vector3 vec, float x)
    {
        return new Vector3(vec.x + x, vec.y, vec.z);
    }
    
    public static Vector3 AddY(this Vector3 vec, float y)
    {
        return new Vector3(vec.x, vec.y + y, vec.z);
    }
    
    public static Vector3 AddZ(this Vector3 vec, float z)
    {
        return new Vector3(vec.x, vec.y, vec.z + z);
    }

}
