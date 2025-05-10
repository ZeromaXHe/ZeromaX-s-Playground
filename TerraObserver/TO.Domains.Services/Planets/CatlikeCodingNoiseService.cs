using Godot;
using TO.Commons.Utils;
using TO.Domains.Services.Abstractions.Planets;
using TO.Nodes.Abstractions.Planets.Models;

namespace TO.Domains.Services.Planets;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-05-10 13:25:49
public class CatlikeCodingNoiseService(ICatlikeCodingNoise catlikeCodingNoise, IHexSphereConfigs hexSphereConfigs)
    : ICatlikeCodingNoiseService
{
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
        return GetPixelBilinear(catlikeCodingNoise.NoiseSourceImage!,
            (position.X - position.Y * 0.5f - position.Z * 0.5f) * NoiseScale / hexSphereConfigs.StandardScale,
            (position.Y - position.Z) * HexMetrics.OuterToInner * NoiseScale / hexSphereConfigs.StandardScale);
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
            * hexSphereConfigs.StandardScale * position.Length() / HexMetrics.StandardRadius;
        var z = vecZ * (sample.Z * 2f - 1f) * CellPerturbStrength
            * hexSphereConfigs.StandardScale * position.Length() / HexMetrics.StandardRadius;
        return position + x + z;
    }
}