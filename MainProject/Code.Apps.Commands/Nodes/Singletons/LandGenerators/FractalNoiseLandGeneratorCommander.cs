using Domains.Services.Abstractions.Nodes.Singletons.LandGenerators;
using Infras.Readers.Abstractions.Nodes.Singletons;
using Infras.Readers.Abstractions.Nodes.Singletons.LandGenerators;

namespace Apps.Commands.Nodes.Singletons.LandGenerators;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 20:40:10
public class FractalNoiseLandGeneratorCommander
{
    private readonly IFractalNoiseLandGeneratorService _fractalNoiseLandGeneratorService;
    private readonly IFractalNoiseLandGeneratorRepo _fractalNoiseLandGeneratorRepo;

    private readonly IHexMapGeneratorRepo _hexMapGeneratorRepo;

    public FractalNoiseLandGeneratorCommander(IFractalNoiseLandGeneratorService fractalNoiseLandGeneratorService,
        IFractalNoiseLandGeneratorRepo fractalNoiseLandGeneratorRepo, IHexMapGeneratorRepo hexMapGeneratorRepo)
    {
        _fractalNoiseLandGeneratorService = fractalNoiseLandGeneratorService;
        _fractalNoiseLandGeneratorRepo = fractalNoiseLandGeneratorRepo;
        _fractalNoiseLandGeneratorRepo.Ready += OnReady;
        _fractalNoiseLandGeneratorRepo.TreeExiting += OnTreeExiting;
        _hexMapGeneratorRepo = hexMapGeneratorRepo;
    }

    public void ReleaseEvents()
    {
        _fractalNoiseLandGeneratorRepo.Ready -= OnReady;
        _fractalNoiseLandGeneratorRepo.TreeExiting -= OnTreeExiting;
    }

    private void OnReady()
    {
        _hexMapGeneratorRepo.CreatingFractalNoiseLand += _fractalNoiseLandGeneratorService.CreateLand;
    }

    private void OnTreeExiting()
    {
        _hexMapGeneratorRepo.CreatingFractalNoiseLand -= _fractalNoiseLandGeneratorService.CreateLand;
    }
}