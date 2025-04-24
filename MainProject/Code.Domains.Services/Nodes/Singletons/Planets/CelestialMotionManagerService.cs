using Domains.Services.Abstractions.Nodes.Singletons.Planets;
using Godot;
using Infras.Readers.Abstractions.Nodes.Singletons;
using Infras.Readers.Abstractions.Nodes.Singletons.Planets;
using Nodes.Abstractions.Planets;

namespace Domains.Services.Nodes.Singletons.Planets;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 20:16:30
public class CelestialMotionManagerService(
    ICelestialMotionManagerRepo celestialMotionManagerRepo,
    IHexPlanetManagerRepo hexPlanetManagerRepo) : ICelestialMotionManagerService
{
    private ICelestialMotionManager Self => celestialMotionManagerRepo.Singleton!;

    public void UpdateLunarDist() => Self.LunarDist!.Position =
        Vector3.Back * Mathf.Clamp(hexPlanetManagerRepo.Radius * Self.SatelliteDistRatio,
            hexPlanetManagerRepo.Radius * (1 + Self.SatelliteRadiusRatio), 800f);

    public void UpdateMoonMeshRadius()
    {
        var moonMesh = Self.MoonMesh!.Mesh as SphereMesh;
        moonMesh?.SetRadius(hexPlanetManagerRepo.Radius * Self.SatelliteRadiusRatio);
        moonMesh?.SetHeight(hexPlanetManagerRepo.Radius * Self.SatelliteRadiusRatio * 2);
    }
}