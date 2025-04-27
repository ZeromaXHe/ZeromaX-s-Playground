using Domains.Services.Abstractions.Nodes.Singletons.Planets;
using Infras.Readers.Abstractions.Nodes.Singletons.Planets;

namespace Apps.Commands.Nodes.Singletons.Planets;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 20:40:53
public class CelestialMotionManagerCommander
{
    private readonly ICelestialMotionManagerService _celestialMotionManagerService;
    private readonly ICelestialMotionManagerRepo _celestialMotionManagerRepo;

    public CelestialMotionManagerCommander(ICelestialMotionManagerService celestialMotionManagerService,
        ICelestialMotionManagerRepo celestialMotionManagerRepo)
    {
        _celestialMotionManagerService = celestialMotionManagerService;
        _celestialMotionManagerRepo = celestialMotionManagerRepo;
        _celestialMotionManagerRepo.Ready += OnReady;
        _celestialMotionManagerRepo.Processed += OnProcessed;
        _celestialMotionManagerRepo.SatelliteDistRatioChanged += OnSatelliteDistRatioChanged;
        _celestialMotionManagerRepo.SatelliteRadiusRatioChanged += OnSatelliteRadiusRatioChanged;
        _celestialMotionManagerRepo.StarMoveStatusToggled += _celestialMotionManagerService.ToggleStarMoveStatus;
        _celestialMotionManagerRepo.PlanetMoveStatusToggled += _celestialMotionManagerService.TogglePlanetMoveStatus;
        _celestialMotionManagerRepo.SatelliteMoveStatusToggled +=
            _celestialMotionManagerService.ToggleSatelliteMoveStatus;
    }

    public void ReleaseEvents()
    {
        _celestialMotionManagerRepo.Ready -= OnReady;
        _celestialMotionManagerRepo.Processed -= OnProcessed;
        _celestialMotionManagerRepo.SatelliteDistRatioChanged -= OnSatelliteDistRatioChanged;
        _celestialMotionManagerRepo.SatelliteRadiusRatioChanged -= OnSatelliteRadiusRatioChanged;
    }

    // 会在 _Ready() 后被 Ready 信号触发
    private void OnReady()
    {
        _celestialMotionManagerService.UpdateLunarDist();
        _celestialMotionManagerService.UpdateMoonMeshRadius();
    }

    private void OnProcessed(double delta)
    {
        if (!_celestialMotionManagerRepo.IsRegistered()) return;
        _celestialMotionManagerService.UpdateStellarRotation((float)delta);
    }

    private void OnSatelliteDistRatioChanged(float obj)
    {
        _celestialMotionManagerService.UpdateLunarDist();
    }

    private void OnSatelliteRadiusRatioChanged(float value)
    {
        _celestialMotionManagerService.UpdateMoonMeshRadius();
    }
}