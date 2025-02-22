using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Enum;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

public static class HexMetrics
{
    public const float SolidFactor = 0.75f;
    public const float BlendFactor = 1f - SolidFactor;

    public const float MaxHeightRadiusRatio = 0.1f;

    public const int TerracesPerSlope = 2;
    public const int TerraceSteps = TerracesPerSlope * 2 + 1;
    public const float HorizontalTerraceStepSize = 1f / TerraceSteps;
    public const float VerticalTerraceStepSize = 1f / (TerracesPerSlope + 1);

    // 适用于球面的阶地 Lerp
    public static Vector3 TerraceLerp(Vector3 a, Vector3 b, int step)
    {
        var bWithAHeight = Math3dUtil.ProjectToSphere(b, a.Length());
        var h = step * HorizontalTerraceStepSize;
        var horizontal = a.Slerp(bWithAHeight, h);
        var v = ((step + 1) / 2) * VerticalTerraceStepSize;
        var vertical = Mathf.Lerp(a.Length(), b.Length(), v);
        return Math3dUtil.ProjectToSphere(horizontal, vertical);
    }

    public static Color TerraceLerp(Color a, Color b, int step)
    {
        var h = step * HorizontalTerraceStepSize;
        return a.Lerp(b, h);
    }

    public static HexEdgeType GetEdgeType(int elevation1, int elevation2)
    {
        if (elevation1 == elevation2)
            return HexEdgeType.Flat;
        return Mathf.Abs(elevation1 - elevation2) == 1 ? HexEdgeType.Slope : HexEdgeType.Cliff;
    }
}