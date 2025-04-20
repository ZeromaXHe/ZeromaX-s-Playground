using Godot;
using Infras.Readers.Abstractions.Bases;
using Nodes.Abstractions;

namespace Infras.Readers.Abstractions.Nodes.Singletons;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 19:19:18
public interface IHexMapGeneratorRepo : ISingletonNodeRepo<IHexMapGenerator>
{
    delegate int CreatingErosionLandEvent(RandomNumberGenerator rng, List<MapRegion> regions);

    event CreatingErosionLandEvent? CreatingErosionLand;
    event Action<RandomNumberGenerator>? ErodingLand;

    delegate int CreatingFractalNoiseLandEvent(RandomNumberGenerator rng);

    event CreatingFractalNoiseLandEvent? CreatingFractalNoiseLand;

    delegate int CreatingRealEarthLandEvent();

    event CreatingRealEarthLandEvent? CreatingRealEarthLand;
}