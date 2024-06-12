using UnityEngine;

public static class VectorUtils
{
    // Extension method for Vector3 to Vector2Int
    public static Vector2Int ToVector2Int(this Vector3 vector)
    {
        return new Vector2Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y));
    }

    // Extension method for Vector2 to Vector2Int
    public static Vector2Int ToVector2Int(this Vector2 vector)
    {
        return new Vector2Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y));
    }
}
