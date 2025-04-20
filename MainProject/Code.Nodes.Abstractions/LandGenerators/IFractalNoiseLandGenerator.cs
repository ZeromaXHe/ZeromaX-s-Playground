using GodotNodes.Abstractions;
using Nodes.Abstractions.Resources.LandGenerators;

namespace Nodes.Abstractions.LandGenerators;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-17 10:30:17
public interface IFractalNoiseLandGenerator : INode
{
    ILayeredFastNoise GetLayeredNoises();
}