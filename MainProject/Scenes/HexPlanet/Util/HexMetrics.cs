using System;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Enum;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Struct;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

public static class HexMetrics
{
    public const float StandardRadius = 150f; // 150f 半径时才是标准大小，其他时候需要按比例缩放
    private static float _radius = 150f;

    public static float Radius
    {
        get => _radius;
        set
        {
            _radius = value;
            CalcUnitHeight();
        }
    }

    public const float StandardDivisions = 10f; // 10 细分时才是标准大小，其他时候需要按比例缩放
    private static int _divisions = 10;

    public static int Divisions
    {
        get => _divisions;
        set
        {
            _divisions = value;
            CalcUnitHeight();
        }
    }

    // 单位高度
    public static float UnitHeight { get; private set; } = 1.5f;
    public static float MaxHeight { get; private set; } = 15f;
    public static float MaxHeightRatio { get; private set; } = 0.1f;
    private const float MaxHeightRadiusRatio = 0.1f;

    private static void CalcUnitHeight()
    {
        MaxHeightRatio = StandardScale * MaxHeightRadiusRatio;
        MaxHeight = Radius * MaxHeightRatio;
        UnitHeight = MaxHeight / ElevationStep;
    }

    public static float StandardScale => Radius / StandardRadius * StandardDivisions / Divisions;

    public const float OuterToInner = 0.8660254037f; // √3/2 = 0.8660254037f
    public const float InnerToOuter = 1f / OuterToInner;
    public const float SolidFactor = 0.8f;
    public const float BlendFactor = 1f - SolidFactor;

    public const int ElevationStep = 10; // 这里对应含义是 Elevation 分为几级

    private const int TerracesPerSlope = 2;
    public const int TerraceSteps = TerracesPerSlope * 2 + 1;
    private const float HorizontalTerraceStepSize = 1f / TerraceSteps;
    private const float VerticalTerraceStepSize = 1f / (TerracesPerSlope + 1);

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

    #region 噪声扰动

    // 模拟 Unity Texture2D.GetPixelBilinear API
    // （入参是 0 ~ 1 的 float，超过范围则取小数部分，即“包裹模式进行重复”）
    // 参考：https://docs.unity3d.com/cn/2021.3/ScriptReference/Texture2D.GetPixelBilinear.html
    private static Vector4 GetPixelBilinear(Image img, float u, float v)
    {
        var x = (int)Mathf.PosMod(u * img.GetWidth(), img.GetWidth());
        var y = (int)Mathf.PosMod(v * img.GetHeight(), img.GetHeight());
        // 这里现在 Godot（4.3）有 bug 啊，文档说 PosMod 返回 [0, b), 结果我居然取到了 b……
        if (x == img.GetWidth())
        {
            // GD.PrintErr($"WTF! PosMod not working for ({u}, {v}) => ({img.GetWidth()}, {img.GetHeight()}) => ({x}, {y})");
            x = 0;
        }

        if (y == img.GetHeight())
        {
            // GD.PrintErr($"WTF! PosMod not working for ({u}, {v}) => ({img.GetWidth()}, {img.GetHeight()}) => ({x}, {y})");
            y = 0;
        }

        var color = img.GetPixel(x, y);
        return new Vector4(color.R, color.G, color.B, color.A);
    }

    public static Image NoiseSource { get; set; }

    private const float NoiseScale = 0.003f;

    // 球面噪声采样逻辑
    // 按照六边形坐标映射 XYZ，使得各个方向的移动都会使得采样变化
    // X 映射向右，Y 映射为左向上 60 度，Z 映射为左向下 60 度
    public static Vector4 SampleNoise(Vector3 position) =>
        GetPixelBilinear(NoiseSource,
            (position.X - position.Y * 0.5f - position.Z * 0.5f) * NoiseScale,
            (position.Y - position.Z) * OuterToInner * NoiseScale);

    private const float CellPerturbStrength = 4f;
    public const float ElevationPerturbStrength = 0.5f;

    // 球面的扰动逻辑
    public static Vector3 Perturb(Vector3 position)
    {
        var sample = SampleNoise(Math3dUtil.ProjectToSphere(position, StandardRadius));
        var vecX = position is { X: 0, Z: 0 }
            ? position.Cross(Vector3.Back).Normalized()
            : Vector3.Up.Cross(position).Normalized();
        var vecZ = vecX.Cross(position).Normalized();
        var x = vecX * (sample.X * 2f - 1f) * CellPerturbStrength * position.Length() / StandardRadius;
        var z = vecZ * (sample.Z * 2f - 1f) * CellPerturbStrength * position.Length() / StandardRadius;
        return position + x + z;
    }

    #endregion

    #region 河流与水面

    private const float StreamBedElevationOffset = -1.75f;
    private const float WaterElevationOffset = -0.5f;
    public const float WaterFactor = 0.6f;
    public const float WaterBlendFactor = 1f - WaterFactor;
    public static float GetStreamBedHeight(int elevation) => (elevation + StreamBedElevationOffset) * UnitHeight;
    public static float GetWaterSurfaceHeight(int level) => (level + WaterElevationOffset) * UnitHeight;

    #endregion

    #region 哈希网格

    public const int HashGridSize = 256;
    private static readonly HexHash[] HashGrid = new HexHash[HashGridSize * HashGridSize];
    private static readonly RandomNumberGenerator Rng = new();

    public static void InitializeHashGrid(ulong seed)
    {
        var initState = Rng.State;
        Rng.Seed = seed;
        for (var i = 0; i < HashGrid.Length; i++)
            HashGrid[i] = HexHash.Create();
        Rng.State = initState;
    }

    public static HexHash SampleHashGrid(Vector3 position)
    {
        position = Math3dUtil.ProjectToSphere(position, StandardRadius);
        var x = (int)Mathf.PosMod(position.X - position.Y * 0.5f - position.Z * 0.5f, HashGridSize);
        if (x == HashGridSize) // 前面噪声扰动那里说过 PosMod 文档返回 [0, b), 结果取到了 b，所以怕了…… 加个防御性处理
            x = 0;
        var z = (int)Mathf.PosMod((position.Y - position.Z) * OuterToInner, HashGridSize);
        if (z == HashGridSize)
            z = 0;
        return HashGrid[x + z * HashGridSize];
    }

    #endregion

    #region 特征

    private static readonly float[][] FeatureThresholds =
    [
        [0.0f, 0.0f, 0.4f],
        [0.0f, 0.4f, 0.6f],
        [0.4f, 0.6f, 0.8f]
    ];

    public static float[] GetFeatureThreshold(int level) => FeatureThresholds[level];
    private const float WallHeight = 2f;
    private const float WallYOffset = -0.5f;
    private const float WallThickness = 0.375f;
    public static float GetWallHeight() => UnitHeight * WallHeight;
    public static float GetWallThickness() => UnitHeight * WallThickness;

    /// <summary>
    /// 按照厚度找到墙偏移向量
    /// </summary>
    /// <param name="near">近端坐标</param>
    /// <param name="far">远端坐标</param>
    /// <param name="toNear">true，则求偏移向近端的方向；false，则向远端</param>
    /// <param name="thickness">墙厚度</param>
    /// <returns>从近端和远端等平均高度位置的球面中点，向墙厚位置的偏移向量</returns>
    public static Vector3 WallThicknessOffset(Vector3 near, Vector3 far, bool toNear, float thickness)
    {
        var avgHeight = (near.Length() + far.Length()) / 2f;
        near = Math3dUtil.ProjectToSphere(near, avgHeight);
        far = Math3dUtil.ProjectToSphere(far, avgHeight);
        var mid = near.Slerp(far, 0.5f);
        var sphereDistance = near.AngleTo(far) * avgHeight;
        var target = toNear ? near : far;
        return mid.Slerp(target, thickness / sphereDistance);
    }

    private const float WallElevationOffset = VerticalTerraceStepSize;

    public static Vector3 WallLerp(Vector3 near, Vector3 far)
    {
        var mid = near.Slerp(far, 0.5f);
        var v = near.Length() < far.Length() ? WallElevationOffset : 1f - WallElevationOffset;
        return Math3dUtil.ProjectToSphere(mid,
            Mathf.Lerp(near.Length(), far.Length(), v) + UnitHeight * WallYOffset);
    }

    public const float WallTowerThreshold = 0.6f;
    public const float BridgeDesignLength = 7f;

    #endregion
}