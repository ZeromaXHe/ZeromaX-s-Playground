using Godot;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

public static class Math3dUtil
{
    public static Vector3 GetNormal(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        var side1 = v2 - v1;
        var side2 = v3 - v1;
        return side1.Cross(side2).Normalized();
    }
    
    public static bool IsNormalAwayFromOrigin(Vector3 surface, Vector3 normal, Vector3 origin) =>
        (surface - origin).Dot(normal) > 0;

    public static Vector3 ProjectToSphere(Vector3 p, float radius, float scale = 1f)
    {
        var projectionPoint = radius / p.Length();
        return p * projectionPoint * scale;
    }
}