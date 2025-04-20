using Apps.Queries.Contexts;
using Contexts;
using Godot;
using GodotNodes.Abstractions.Addition;
using Infras.Readers.Abstractions.Nodes.Singletons;
using Infras.Writers.Abstractions.PlanetGenerates;
using Nodes.Abstractions.LandGenerators;
using Nodes.Abstractions.Resources.LandGenerators;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Resources.LandGenerators;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Nodes.LandGenerators;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// 基于 Sebastian 视频中的叠加多个分形噪声原理的陆地生成器
/// Author: Zhu XH
/// Date: 2025-03-20 19:51:43
[Tool]
public partial class FractalNoiseLandGenerator : Node, IFractalNoiseLandGenerator
{
    public FractalNoiseLandGenerator()
    {
        NodeContext.Instance.RegisterSingleton<IFractalNoiseLandGenerator>(this);
        Context.RegisterSingletonToHolder<IFractalNoiseLandGenerator>(this);
    }

    public override void _ExitTree() => NodeContext.Instance.DestroySingleton<IFractalNoiseLandGenerator>();
    public NodeEvent? NodeEvent => null;

    [Export] public LayeredFastNoise LayeredNoises { get; set; } = new();
    public ILayeredFastNoise GetLayeredNoises() => LayeredNoises; 
}