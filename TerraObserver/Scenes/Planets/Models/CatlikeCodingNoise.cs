using Godot;
using TO.Commons.Utils;
using TO.Domains.Models.ValueObjects.Planets;

namespace TerraObserver.Scenes.Planets.Models;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-05-10 08:14:29
[GlobalClass]
[Tool]
public partial class CatlikeCodingNoise : Resource
{
    // 其实这里可以直接导入 Image, 在导入界面选择导入类型。但是导入 Image 的场景 tscn 文件会大得吓人……（等于直接按像素写一遍）
    [Export]
    public Texture2D? NoiseSource
    {
        get => _noiseSource;
        set
        {
            _noiseSource = value;
            NoiseSourceImage = value?.GetImage();
        }
    }

    private Texture2D? _noiseSource;

    public Image? NoiseSourceImage { get; private set; }

    [Export]
    public ulong Seed
    {
        get => _seed;
        set
        {
            _seed = value;
            InitializeHashGrid(value);
        }
    }

    private ulong _seed = 1234;

    private const int HashGridSize = 256;
    private static readonly HexHash[] HashGrid = new HexHash[HashGridSize * HashGridSize];
    private static readonly RandomNumberGenerator Rng = new();

    private void InitializeHashGrid(ulong seed)
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
}