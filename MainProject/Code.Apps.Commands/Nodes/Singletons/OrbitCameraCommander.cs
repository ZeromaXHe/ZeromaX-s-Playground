using Godot;
using Infras.Readers.Abstractions.Nodes.Singletons;

namespace Apps.Commands.Nodes.Singletons;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 20:44:14
public class OrbitCameraCommander
{
    private readonly IOrbitCameraRepo _orbitCameraRepo;

    private readonly IHexPlanetManagerRepo _hexPlanetManagerRepo;
    private readonly IMiniMapManagerRepo _miniMapManagerRepo;

    public OrbitCameraCommander(IOrbitCameraRepo orbitCameraRepo, IHexPlanetManagerRepo hexPlanetManagerRepo,
        IMiniMapManagerRepo miniMapManagerRepo)
    {
        _orbitCameraRepo = orbitCameraRepo;
        _orbitCameraRepo.Ready += OnReady;
        _orbitCameraRepo.RadiusChanged += OnRadiusChanged;
        _orbitCameraRepo.ZoomChanged += OnZoomChanged;

        _hexPlanetManagerRepo = hexPlanetManagerRepo;
        _miniMapManagerRepo = miniMapManagerRepo;
        _miniMapManagerRepo.Clicked += OnMiniMapClicked;
    }

    public void ReleaseEvents()
    {
        _orbitCameraRepo.Ready -= OnReady;
        _orbitCameraRepo.RadiusChanged -= OnRadiusChanged;
        _orbitCameraRepo.ZoomChanged -= OnZoomChanged;
        _miniMapManagerRepo.Clicked -= OnMiniMapClicked;
    }

    private void OnReady()
    {
        if (!Engine.IsEditorHint())
            _orbitCameraRepo.Singleton!.Reset(_hexPlanetManagerRepo.Radius);
    }

    private void OnRadiusChanged(float radius)
    {
        _orbitCameraRepo.Singleton!.SetRadius(radius, _hexPlanetManagerRepo.MaxHeightRatio,
            _hexPlanetManagerRepo.StandardScale);
    }

    private void OnZoomChanged(float zoom)
    {
        _orbitCameraRepo.Singleton!.SetZoom(zoom, _hexPlanetManagerRepo.StandardScale);
    }

    private void OnMiniMapClicked(Vector3 destinationDirection)
    {
        _orbitCameraRepo.Singleton!.SetAutoPilot(destinationDirection);
    }
}