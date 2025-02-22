using System;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Enum;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

public static class HexMetrics
{
    public const float SolidFactor = 0.8f;
    public const float BlendFactor = 1f - SolidFactor;

    public const int ElevationStep = 10; // 这里对应含义是 Elevation 分为几级
    public const float MaxHeightRadiusRatio = 0.1f;

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
            (position.Y - position.Z) * 0.8660254037f * NoiseScale); // √3/2 = 0.8660254037f

    private const float CellPerturbStrength = 4f;
    public const float ElevationPerturbStrength = 1.5f;

    // 球面的扰动逻辑
    public static Vector3 Perturb(Vector3 position)
    {
        var sample = SampleNoise(position);
        var vecX = position is { X: 0, Z: 0 }
            ? position.Cross(Vector3.Back).Normalized()
            : Vector3.Up.Cross(position).Normalized();
        var vecZ = vecX.Cross(position).Normalized();
        var x = vecX * (sample.X * 2f - 1f) * CellPerturbStrength;
        var z = vecZ * (sample.Z * 2f - 1f) * CellPerturbStrength;
        return position + x + z;
    }
}