using Godot;
using TO.Abstractions.Models.Planets;
using TO.Domains.Structs.Tiles;

namespace TerraObserver.Scenes.Planets.Models;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-06 16:59:41
[Tool]
[GlobalClass]
public partial class CatlikeCodingNoise : Resource, ICatlikeCodingNoise
{
    #region Export 属性

    [ExportGroup("噪声配置")]
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

    [ExportGroup("哈希配置")]
    [Export]
    public ulong Seed
    {
        get => _seed;
        set { _seed = value; }
    }

    private ulong _seed = 1234;

    #endregion

    #region 普通属性

    public Image? NoiseSourceImage { get; private set; }
    private const int HashGridSizeConst = 256;
    public int HashGridSize => HashGridSizeConst;
    public HexHash[] HashGrid { get; } = new HexHash[HashGridSizeConst * HashGridSizeConst];
    public RandomNumberGenerator Rng { get; } = new();

    #endregion
}