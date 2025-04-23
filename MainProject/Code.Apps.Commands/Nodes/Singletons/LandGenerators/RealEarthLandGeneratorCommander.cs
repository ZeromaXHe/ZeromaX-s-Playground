using Domains.Services.Abstractions.Nodes.Singletons.LandGenerators;
using Infras.Readers.Abstractions.Nodes.Singletons;
using Infras.Readers.Abstractions.Nodes.Singletons.LandGenerators;

namespace Apps.Commands.Nodes.Singletons.LandGenerators;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 20:40:29
public class RealEarthLandGeneratorCommander
{
    private readonly IRealEarthLandGeneratorService _realEarthLandGeneratorService;
    private readonly IRealEarthLandGeneratorRepo _realEarthLandGeneratorRepo;

    private readonly IHexMapGeneratorRepo _hexMapGeneratorRepo;

    public RealEarthLandGeneratorCommander(IRealEarthLandGeneratorService realEarthLandGeneratorService,
        IRealEarthLandGeneratorRepo realEarthLandGeneratorRepo, IHexMapGeneratorRepo hexMapGeneratorRepo)
    {
        _realEarthLandGeneratorService = realEarthLandGeneratorService;
        _realEarthLandGeneratorRepo = realEarthLandGeneratorRepo;
        _realEarthLandGeneratorRepo.Ready += OnReady;
        _realEarthLandGeneratorRepo.TreeExiting += OnTreeExiting;
        _hexMapGeneratorRepo = hexMapGeneratorRepo;
    }

    public void ReleaseEvents()
    {
        _realEarthLandGeneratorRepo.Ready -= OnReady;
        _realEarthLandGeneratorRepo.TreeExiting -= OnTreeExiting;
    }

    private void OnReady()
    {
        _hexMapGeneratorRepo.CreatingRealEarthLand += _realEarthLandGeneratorService.CreateLand;
    }

    private void OnTreeExiting()
    {
        _hexMapGeneratorRepo.CreatingRealEarthLand -= _realEarthLandGeneratorService.CreateLand;
    }
}