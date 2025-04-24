using Contexts;
using Godot;
using GodotNodes.Abstractions.Addition;
using Nodes.Abstractions.LandGenerators;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Nodes.LandGenerators;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// 真实地球生成器
/// Author: Zhu XH
/// Date: 2025-03-21 19:35:33
[Tool]
public partial class RealEarthLandGenerator : Node, IRealEarthLandGenerator
{
    public RealEarthLandGenerator()
    {
        Context.RegisterToHolder<IRealEarthLandGenerator>(this);
    }

    public NodeEvent? NodeEvent => null;

    // 其实这里可以直接导入 Image, 在导入界面选择导入类型。但是导入 Image 的场景 tscn 文件会大得吓人……（等于直接按像素写一遍）
    // 这里是否改为 Image 需要权衡，目前 Texture2D.GetImage() 会耗时比较长，但是提前把 Image 加载到内存的话，消耗很大。
    // 所以目前把四张图片压到同一张图片的 RGB 通道里了（陆地掩码（去除湖区） R，地形 G，海洋测深 B），并压缩大小到 4096x2048
    [Export] public Texture2D? WorldMap { get; set; }
}