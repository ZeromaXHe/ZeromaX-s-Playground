using Godot;

namespace Domains.Services.Abstractions.Nodes.Singletons.LandGenerators;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 20:07:18
public interface IFractalNoiseLandGeneratorService
{
    int CreateLand(RandomNumberGenerator random);
}