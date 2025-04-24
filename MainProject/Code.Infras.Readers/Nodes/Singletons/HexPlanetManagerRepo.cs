using Commons.Utils;
using Domains.Models.Entities.PlanetGenerates;
using Domains.Models.ValueObjects.PlanetGenerates;
using Godot;
using Infras.Readers.Abstractions.Nodes.Singletons;
using Infras.Readers.Bases;
using Nodes.Abstractions;

namespace Infras.Readers.Nodes.Singletons;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 10:45:38
public class HexPlanetManagerRepo : SingletonNodeRepo<IHexPlanetManager>, IHexPlanetManagerRepo
{
    public event Action? NewPlanetGenerated;
    private void OnNewPlanetGenerated() => NewPlanetGenerated?.Invoke();
    public event Action<float>? RadiusChanged;
    private void OnRadiusChanged(float radius) => RadiusChanged?.Invoke(radius);

    protected override void ConnectNodeEvents()
    {
        Singleton!.NewPlanetGenerated += OnNewPlanetGenerated;
        Singleton.RadiusChanged += OnRadiusChanged;
    }

    protected override void DisconnectNodeEvents()
    {
        Singleton!.NewPlanetGenerated -= OnNewPlanetGenerated;
        Singleton.RadiusChanged -= OnRadiusChanged;
    }

    public float Radius
    {
        get => IsRegistered() ? Singleton!.Radius : 100f;
        set
        {
            if (IsRegistered())
                Singleton!.Radius = value;
        }
    }

    public int Divisions
    {
        get => IsRegistered() ? Singleton!.Divisions : 20;
        set
        {
            if (IsRegistered())
                Singleton!.Divisions = value;
        }
    }

    public int ChunkDivisions
    {
        get => IsRegistered() ? Singleton!.ChunkDivisions : 5;
        set
        {
            if (IsRegistered())
                Singleton!.ChunkDivisions = value;
        }
    }

    public int ElevationStep
    {
        get => IsRegistered() ? Singleton!.ElevationStep : 10;
        set
        {
            if (IsRegistered())
                Singleton!.ElevationStep = value;
        }
    }

    public float StandardScale => IsRegistered() ? Singleton!.StandardScale : 1f;

    #region 噪声相关

    private const float NoiseScale = 0.003f;

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

    // 球面噪声采样逻辑
    // 按照六边形坐标映射 XYZ，使得各个方向的移动都会使得采样变化
    // X 映射向右，Y 映射为左向上 60 度，Z 映射为左向下 60 度
    public Vector4 SampleNoise(Vector3 position)
    {
        if (IsRegistered())
            return GetPixelBilinear(Singleton!.NoiseSourceImage!,
                (position.X - position.Y * 0.5f - position.Z * 0.5f) * NoiseScale / StandardScale,
                (position.Y - position.Z) * HexMetrics.OuterToInner * NoiseScale / StandardScale);
        return Vector4.Zero; // BUG: 现在编辑器里 HUD 场景第一次编译运行显示分块悬浮在空中，是此处防止单例为空的逻辑导致的
    }

    private const float CellPerturbStrength = 4f;
    public float ElevationPerturbStrength => 0.5f;

    // 球面的扰动逻辑
    public Vector3 Perturb(Vector3 position)
    {
        var sample = SampleNoise(Math3dUtil.ProjectToSphere(position, HexMetrics.StandardRadius));
        var vecX = position is { X: 0, Z: 0 }
            ? position.Cross(Vector3.Back).Normalized()
            : Vector3.Up.Cross(position).Normalized();
        var vecZ = vecX.Cross(position).Normalized();
        var x = vecX * (sample.X * 2f - 1f) * CellPerturbStrength
            * StandardScale * position.Length() / HexMetrics.StandardRadius;
        var z = vecZ * (sample.Z * 2f - 1f) * CellPerturbStrength
            * StandardScale * position.Length() / HexMetrics.StandardRadius;
        return position + x + z;
    }

    private const int HashGridSize = 256;
    private static readonly HexHash[] HashGrid = new HexHash[HashGridSize * HashGridSize];
    private static readonly RandomNumberGenerator Rng = new();

    public void InitializeHashGrid(ulong seed)
    {
        var initState = Rng.State;
        Rng.Seed = seed;
        for (var i = 0; i < HashGrid.Length; i++)
            HashGrid[i] = HexHash.Create();
        Rng.State = initState;
    }

    public HexHash SampleHashGrid(Vector3 position)
    {
        position = Math3dUtil.ProjectToSphere(position, HexMetrics.StandardRadius);
        var x = (int)Mathf.PosMod(position.X - position.Y * 0.5f - position.Z * 0.5f, HashGridSize);
        if (x == HashGridSize) // 前面噪声扰动那里说过 PosMod 文档返回 [0, b), 结果取到了 b，所以怕了…… 加个防御性处理
            x = 0;
        var z = (int)Mathf.PosMod((position.Y - position.Z) * HexMetrics.OuterToInner, HashGridSize);
        if (z == HashGridSize)
            z = 0;
        return HashGrid[x + z * HashGridSize];
    }

    #endregion

    #region 特征

    private readonly float[][] _featureThresholds =
    [
        [0.0f, 0.0f, 0.4f],
        [0.0f, 0.4f, 0.6f],
        [0.4f, 0.6f, 0.8f]
    ];

    public float[] GetFeatureThreshold(int level) => _featureThresholds[level];
    private const float WallHeight = 2f;
    private const float WallYOffset = -0.5f;
    private const float WallThickness = 0.375f;
    public float GetWallHeight() => UnitHeight * WallHeight;
    public float GetWallThickness() => UnitHeight * WallThickness;

    /// <summary>
    /// 按照厚度找到墙偏移向量
    /// </summary>
    /// <param name="near">近端坐标</param>
    /// <param name="far">远端坐标</param>
    /// <param name="toNear">true，则求偏移向近端的方向；false，则向远端</param>
    /// <param name="thickness">墙厚度</param>
    /// <returns>从近端和远端等平均高度位置的球面中点，向墙厚位置的偏移向量</returns>
    public Vector3 WallThicknessOffset(Vector3 near, Vector3 far, bool toNear, float thickness)
    {
        var avgHeight = (near.Length() + far.Length()) / 2f;
        near = Math3dUtil.ProjectToSphere(near, avgHeight);
        far = Math3dUtil.ProjectToSphere(far, avgHeight);
        var mid = near.Slerp(far, 0.5f);
        var sphereDistance = near.AngleTo(far) * avgHeight;
        var target = toNear ? near : far;
        return mid.Slerp(target, thickness / sphereDistance);
    }

    private const float WallElevationOffset = HexMetrics.VerticalTerraceStepSize;

    public Vector3 WallLerp(Vector3 near, Vector3 far)
    {
        var mid = near.Slerp(far, 0.5f);
        var v = near.Length() < far.Length() ? WallElevationOffset : 1f - WallElevationOffset;
        return Math3dUtil.ProjectToSphere(mid,
            Mathf.Lerp(near.Length(), far.Length(), v) + UnitHeight * WallYOffset);
    }

    #endregion

    #region 高度

    public float UnitHeight => IsRegistered() ? Singleton!.UnitHeight : 1.5f;

    public int DefaultWaterLevel
    {
        get => IsRegistered() ? Singleton!.DefaultWaterLevel : 5;
        set
        {
            if (IsRegistered())
                Singleton!.DefaultWaterLevel = value;
        }
    }

    public float MaxHeight => IsRegistered() ? Singleton!.MaxHeight : 15f;
    public float MaxHeightRatio => IsRegistered() ? Singleton!.MaxHeightRatio : 0.1f;

    public float GetHeight(Tile tile) =>
        (tile.Data.Elevation + GetPerturbHeight(tile)) * UnitHeight;

    public float GetOverrideHeight(Tile tile, HexTileDataOverrider tileDataOverrider) =>
        (tileDataOverrider.Elevation(tile) + GetPerturbHeight(tile) + 0.05f) * UnitHeight;

    private float GetPerturbHeight(Tile tile) =>
        (SampleNoise(tile.GetCentroid(HexMetrics.StandardRadius)).Y * 2f - 1f)
        * ElevationPerturbStrength * UnitHeight;

    #endregion
}