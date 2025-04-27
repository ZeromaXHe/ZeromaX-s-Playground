using Domains.Services.Abstractions.Nodes.IdInstances;
using Domains.Services.Abstractions.Nodes.Singletons.Planets;
using Infras.Readers.Abstractions.Nodes.Singletons.Planets;
using Infras.Writers.Abstractions.PlanetGenerates;

namespace Apps.Commands.Nodes.Singletons.Planets;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 20:41:27
public class UnitManagerCommander
{
    private readonly IUnitManagerRepo _unitManagerRepo;

    private readonly ISelectTileViewerService _selectTileViewerService;
    private readonly IHexUnitService _hexUnitService;
    private readonly ITileRepo _tileRepo;

    public UnitManagerCommander(IUnitManagerRepo unitManagerRepo, ISelectTileViewerService selectTileViewerService,
        IHexUnitService hexUnitService, ITileRepo tileRepo)
    {
        _unitManagerRepo = unitManagerRepo;
        _unitManagerRepo.PathFromTileIdSetZero += OnPathFromTileIdSetZero;

        _selectTileViewerService = selectTileViewerService;
        _hexUnitService = hexUnitService;
        _tileRepo = tileRepo;
        _tileRepo.UnitValidateLocation += OnTileServiceUnitValidateLocation;
    }

    public void ReleaseEvents()
    {
        _unitManagerRepo.PathFromTileIdSetZero -= OnPathFromTileIdSetZero;
        _tileRepo.UnitValidateLocation -= OnTileServiceUnitValidateLocation;
    }

    private void OnPathFromTileIdSetZero()
    {
        _selectTileViewerService.ClearPath();
    }

    private void OnTileServiceUnitValidateLocation(int unitId) =>
        _hexUnitService.ValidateLocation(_unitManagerRepo.Singleton?.Units[unitId]!);
}