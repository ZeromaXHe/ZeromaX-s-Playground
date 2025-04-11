using Commons.Utils;
using Domains.Models.ValueObjects.PlanetGenerates;
using Godot;

namespace Domains.Services.PlanetGenerates.Impl;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-12 07:52
public class NoiseService(IPlanetSettingService planetSettingService) : INoiseService
{
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

    public Image? NoiseSource { get; set; }

    private const float NoiseScale = 0.003f;

    // 球面噪声采样逻辑
    // 按照六边形坐标映射 XYZ，使得各个方向的移动都会使得采样变化
    // X 映射向右，Y 映射为左向上 60 度，Z 映射为左向下 60 度
    public Vector4 SampleNoise(Vector3 position) =>
        GetPixelBilinear(NoiseSource!,
            (position.X - position.Y * 0.5f - position.Z * 0.5f) * NoiseScale / planetSettingService.StandardScale,
            (position.Y - position.Z) * HexMetrics.OuterToInner * NoiseScale / planetSettingService.StandardScale);

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
            * planetSettingService.StandardScale * position.Length() / HexMetrics.StandardRadius;
        var z = vecZ * (sample.Z * 2f - 1f) * CellPerturbStrength
            * planetSettingService.StandardScale * position.Length() / HexMetrics.StandardRadius;
        return position + x + z;
    }

    #endregion

    #region 哈希网格

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
}