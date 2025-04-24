using Domains.Services.Abstractions.Nodes.Singletons;
using Domains.Services.Abstractions.PlanetGenerates;
using Infras.Readers.Abstractions.Nodes.Singletons;
using Infras.Writers.Abstractions.PlanetGenerates;

namespace Apps.Commands.Nodes.Singletons;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 20:43:52
public class MiniMapManagerCommander
{
    private readonly IMiniMapManagerService _miniMapManagerService;
    private readonly IMiniMapManagerRepo _miniMapManagerRepo;

    private readonly ITileService _tileService;
    private readonly IHexPlanetManagerRepo _hexPlanetManagerRepo;
    private readonly IOrbitCameraRepo _orbitCameraRepo;
    private readonly ITileRepo _tileRepo;
    private readonly IPointRepo _pointRepo;

    public MiniMapManagerCommander(IMiniMapManagerService miniMapManagerService, IMiniMapManagerRepo miniMapManagerRepo,
        ITileService tileService, IHexPlanetManagerRepo hexPlanetManagerRepo, IOrbitCameraRepo orbitCameraRepo,
        ITileRepo tileRepo, IPointRepo pointRepo)
    {
        _miniMapManagerService = miniMapManagerService;
        _miniMapManagerRepo = miniMapManagerRepo;

        _tileService = tileService;
        _hexPlanetManagerRepo = hexPlanetManagerRepo;
        _hexPlanetManagerRepo.NewPlanetGenerated += InitMiniMap;
        _orbitCameraRepo = orbitCameraRepo;
        _orbitCameraRepo.Moved += _miniMapManagerService.SyncCameraIconPos;
        _tileRepo = tileRepo;
        _tileRepo.RefreshTerrainShader += RefreshTile;
        _pointRepo = pointRepo;
    }

    public void ReleaseEvents()
    {
        _hexPlanetManagerRepo.NewPlanetGenerated -= InitMiniMap;
        _orbitCameraRepo.Moved -= _miniMapManagerService.SyncCameraIconPos;
        _tileRepo.RefreshTerrainShader -= RefreshTile;
    }

    private void InitMiniMap()
    {
        if (_miniMapManagerRepo.IsRegistered() && _miniMapManagerRepo.Singleton!.IsNodeReady())
            _miniMapManagerService.Init(_orbitCameraRepo.Singleton!.GetFocusBasePos());
    }

    private void RefreshTile(int tileId)
    {
        var tile = _tileRepo.GetById(tileId)!;
        var sphereAxial = _pointRepo.GetSphereAxial(tile);
        _miniMapManagerRepo.Singleton!.TerrainLayer!.SetCell(sphereAxial.Coords.ToVector2I(), 0,
            IMiniMapManagerService.TerrainAtlas(tile));
    }
}