using Godot;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

public static class Math3dUtil
{
    public static Vector3 GetNormal(Vector3 v0, Vector3 v1, Vector3 v2)
    {
        var side1 = v1 - v0;
        var side2 = v2 - v0;
        return -side1.Cross(side2).Normalized();
    }

    public static bool IsNormalAwayFromOrigin(Vector3 surface, Vector3 normal, Vector3 origin) =>
        (surface - origin).Dot(normal) > 0;

    public static Vector3 ProjectToSphere(Vector3 p, float radius, float scale = 1f)
    {
        var projectionPoint = radius / p.Length();
        return p * projectionPoint * scale;
    }

    // 判断是否 v0, v1, v2 的顺序是合适的缠绕方向
    public static bool IsRightVSeq(Vector3 origin, Vector3 v0, Vector3 v1, Vector3 v2)
    {
        var center = (v0 + v1 + v2) / 3f;
        // 决定缠绕顺序
        var normal = GetNormal(v0, v1, v2);
        return IsNormalAwayFromOrigin(center, normal, origin);
    }
}