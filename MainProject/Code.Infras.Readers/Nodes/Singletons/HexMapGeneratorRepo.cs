using Godot;
using Infras.Readers.Abstractions.Nodes.Singletons;
using Infras.Readers.Bases;
using Nodes.Abstractions;

namespace Infras.Readers.Nodes.Singletons;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 19:20:02
public class HexMapGeneratorRepo : SingletonNodeRepo<IHexMapGenerator>, IHexMapGeneratorRepo
{
    public event IHexMapGeneratorRepo.CreatingErosionLandEvent? CreatingErosionLand;
    public event Action<RandomNumberGenerator>? ErodingLand;
    public event IHexMapGeneratorRepo.CreatingFractalNoiseLandEvent? CreatingFractalNoiseLand;
    public event IHexMapGeneratorRepo.CreatingRealEarthLandEvent? CreatingRealEarthLand;

    protected override void ConnectNodeEvents()
    {
        Singleton!.CreatingErosionLand += OnCreatingErosionLand;
        Singleton.ErodingLand += OnErodingLand;
        Singleton.CreatingFractalNoiseLand += OnCreatingFractalNoiseLand;
        Singleton.CreatingRealEarthLand += OnCreatingRealEarthLand;
    }

    protected override void DisconnectNodeEvents()
    {
        Singleton!.CreatingErosionLand -= OnCreatingErosionLand;
        Singleton.ErodingLand -= OnErodingLand;
        Singleton.CreatingFractalNoiseLand -= OnCreatingFractalNoiseLand;
        Singleton.CreatingRealEarthLand -= OnCreatingRealEarthLand;
    }

    private int OnCreatingErosionLand(RandomNumberGenerator rng, List<MapRegion> regions) =>
        CreatingErosionLand?.Invoke(rng, regions) ?? 0;

    private void OnErodingLand(RandomNumberGenerator rng) => ErodingLand?.Invoke(rng);
    private int OnCreatingFractalNoiseLand(RandomNumberGenerator rng) => CreatingFractalNoiseLand?.Invoke(rng) ?? 0;
    private int OnCreatingRealEarthLand() => CreatingRealEarthLand?.Invoke() ?? 0;
}