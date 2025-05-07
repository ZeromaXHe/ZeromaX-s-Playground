using TO.Domains.Services.Abstractions.Nodes.Singletons;
using TO.Infras.Readers.Abstractions.Nodes.Singletons;

namespace TO.Apps.Commands.Nodes.Singletons;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-28 22:09:27
public class PlanetCommand : IDisposable
{
    private readonly IPlanetService _planetService;
    private readonly IPlanetRepo _planetRepo;

    public PlanetCommand(IPlanetService planetService, IPlanetRepo planetRepo)
    {
        _planetService = planetService;
        _planetRepo = planetRepo;
        _planetRepo.Processed += _planetService.OnProcessed;
        _planetRepo.RadiusChanged += _planetService.OnRadiusChanged;
    }

    public void Dispose()
    {
        _planetRepo.Processed -= _planetService.OnProcessed;
        _planetRepo.RadiusChanged -= _planetService.OnRadiusChanged;
    }
}