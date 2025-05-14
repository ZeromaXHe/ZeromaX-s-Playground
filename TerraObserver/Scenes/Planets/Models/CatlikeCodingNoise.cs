using Godot;

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
        }
    }

    private ulong _seed = 1234;
}