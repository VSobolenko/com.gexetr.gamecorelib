using UnityEngine;

namespace Game.Extensions
{
public static class VectorExtensions
{
    public static Vector3 SetX(this Vector3 v, float x)
    {
        var tmp = v;
        tmp.x = x;
        v = tmp;

        return v;
    }

    public static Vector3 SetY(this Vector3 v, float y)
    {
        var tmp = v;
        tmp.y = y;
        v = tmp;

        return v;
    }

    public static Vector3 SetZ(this Vector3 v, float z)
    {
        var tmp = v;
        tmp.z = z;
        v = tmp;

        return v;
    }

    public static Vector2 SetX(this Vector2 v, float x)
    {
        var tmp = v;
        tmp.x = x;
        v = tmp;

        return v;
    }

    public static Vector2 SetY(this Vector2 v, float y)
    {
        var tmp = v;
        tmp.y = y;
        v = tmp;

        return v;
    }

    public static Vector3 AddX(this Vector3 v, float xDelta)
    {
        var tmp = v;
        tmp.x += xDelta;
        v = tmp;

        return v;
    }

    public static Vector3 AddY(this Vector3 v, float yDelta)
    {
        var tmp = v;
        tmp.y += yDelta;
        v = tmp;

        return v;
    }

    public static Vector3 AddZ(this Vector3 v, float zDelta)
    {
        var tmp = v;
        tmp.z += zDelta;
        v = tmp;

        return v;
    }
    
    public static Vector2 AddX(this Vector2 v, float xDelta)
    {
        var tmp = v;
        tmp.x += xDelta;
        v = tmp;

        return v;
    }

    public static Vector2 AddY(this Vector2 v, float yDelta)
    {
        var tmp = v;
        tmp.y += yDelta;
        v = tmp;

        return v;
    }
    
    public static Vector2 Multiply(this Vector2 a, Vector2 b) => new Vector2(a.x * b.x, a.y * b.y);

    public static Vector3 Multiply(this Vector3 a, Vector3 b) => new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);

    public static Vector2 Abs(this Vector2 v) => new Vector2(Mathf.Abs(v.x), Mathf.Abs(v.y));

    public static Vector3 Abs(this Vector3 v) => new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
}
}