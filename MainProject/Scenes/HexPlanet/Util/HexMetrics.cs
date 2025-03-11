using System;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Enum;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Struct;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util.HexSphereGrid;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

public static class HexMetrics
{
    public const float StandardRadius = 150f; // 150f 半径时才是标准大小，其他时候需要按比例缩放
    public const float StandardDivisions = 10f; // 10 细分时才是标准大小，其他时候需要按比例缩放

    public const float OuterToInner = 0.8660254037f; // √3/2 = 0.8660254037f
    public const float InnerToOuter = 1f / OuterToInner;
    public const float SolidFactor = 0.8f;
    public const float BlendFactor = 1f - SolidFactor;

    private const int TerracesPerSlope = 2;
    public const int TerraceSteps = TerracesPerSlope * 2 + 1;
    private const float HorizontalTerraceStepSize = 1f / TerraceSteps;
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

    #region 河流与水面

    public const float StreamBedElevationOffset = -1.75f;
    public const float WaterElevationOffset = -0.5f;
    public const float WaterFactor = 0.6f;
    public const float WaterBlendFactor = 1f - WaterFactor;

    #endregion

    #region 特征

    public const float WallTowerThreshold = 0.6f;
    public const float BridgeDesignLength = 7f;

    #endregion
}