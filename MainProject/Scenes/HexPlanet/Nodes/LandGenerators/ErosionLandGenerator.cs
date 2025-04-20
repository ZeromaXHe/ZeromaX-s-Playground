using Apps.Queries.Contexts;
using Contexts;
using Godot;
using GodotNodes.Abstractions.Addition;
using Nodes.Abstractions.LandGenerators;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Nodes.LandGenerators;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// 基于 Catlike Coding 六边形地图教程中的随机升降土地，再运用侵蚀算法原理的陆地生成器
/// 速度较慢，生成十万级地块星球需要约半分钟
/// Author: Zhu XH
/// Date: 2025-03-20 19:01:10
[Tool]
public partial class ErosionLandGenerator : Node, IErosionLandGenerator
{
    public ErosionLandGenerator()
    {
        NodeContext.Instance.RegisterSingleton<IErosionLandGenerator>(this);
        Context.RegisterSingletonToHolder<IErosionLandGenerator>(this);
    }

    public NodeEvent? NodeEvent => null;
    public override void _ExitTree() => NodeContext.Instance.DestroySingleton<IErosionLandGenerator>();

    [Export(PropertyHint.Range, "5, 95")] public int LandPercentage { get; set; } = 50;

    [Export(PropertyHint.Range, "20, 200")]
    public int ChunkSizeMin { get; set; } = 30;

    [Export(PropertyHint.Range, "20, 200")]
    public int ChunkSizeMax { get; set; } = 100;

    [Export(PropertyHint.Range, "0.0, 1.0")]
    public float HighRiseProbability { get; set; } = 0.25f;

    [Export(PropertyHint.Range, "0.0, 0.4")]
    public float SinkProbability { get; set; } = 0.2f;

    [Export(PropertyHint.Range, "0, 0.5")] public float JitterProbability { get; set; } = 0.25f;

    [Export(PropertyHint.Range, "0, 100")] public int ErosionPercentage { get; set; } = 50;
}