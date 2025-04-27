using Commons.Constants;
using Domains.Services.Abstractions.Nodes.Singletons.Planets;
using Godot;
using Infras.Readers.Abstractions.Nodes.Singletons;
using Infras.Readers.Abstractions.Nodes.Singletons.Planets;
using Nodes.Abstractions;
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
    private IHexPlanetManager HexPlanetManager => hexPlanetManagerRepo.Singleton!;

    public void UpdateLunarDist() => Self.LunarDist!.Position =
        Vector3.Back * Mathf.Clamp(hexPlanetManagerRepo.Radius * Self.SatelliteDistRatio,
            hexPlanetManagerRepo.Radius * (1 + Self.SatelliteRadiusRatio), 800f);

    public void UpdateMoonMeshRadius()
    {
        var moonMesh = Self.MoonMesh!.Mesh as SphereMesh;
        moonMesh?.SetRadius(hexPlanetManagerRepo.Radius * Self.SatelliteRadiusRatio);
        moonMesh?.SetHeight(hexPlanetManagerRepo.Radius * Self.SatelliteRadiusRatio * 2);
    }

    // 更新天体旋转
    public void UpdateStellarRotation(float delta)
    {
        if (HexPlanetManager.PlanetRevolution || HexPlanetManager.PlanetRotation)
        {
            RenderingServer.GlobalShaderParameterSet(GlobalShaderParam.DirToSun,
                Self.PlanetAxis!.ToLocal(Self.SunMesh!.GlobalPosition.Normalized()));
            // 行星公转
            if (HexPlanetManager.PlanetRevolution)
                Self.SunRevolution!.RotationDegrees = Self.RotationTimeFactor * Vector3.Up * Mathf.Wrap(
                    Self.SunRevolution.RotationDegrees.Y + Self.PlanetRevolutionSpeed * delta, 0f, 360f);
            // 行星自转
            if (HexPlanetManager.PlanetRotation)
            {
                Self.PlanetAxis.RotationDegrees = Self.RotationTimeFactor * Vector3.Up * Mathf.Wrap(
                    Self.PlanetAxis.RotationDegrees.Y + Self.PlanetRotationSpeed * delta, 0f, 360f);
                RenderingServer.GlobalShaderParameterSet(GlobalShaderParam.InvPlanetMatrix,
                    Self.PlanetAxis.Transform.Inverse());
            }
        }

        // 卫星公转
        if (HexPlanetManager.SatelliteRevolution)
            Self.LunarRevolution!.RotationDegrees = Self.RotationTimeFactor * Vector3.Up * Mathf.Wrap(
                Self.LunarRevolution.RotationDegrees.Y + Self.SatelliteRevolutionSpeed * delta, 0f, 360f);
        // 卫星自转
        if (HexPlanetManager.SatelliteRotation)
            Self.MoonAxis!.RotationDegrees = Self.RotationTimeFactor * Vector3.Up * Mathf.Wrap(
                Self.MoonAxis.RotationDegrees.Y + Self.SatelliteRotationSpeed * delta, 0f, 360f);
    }

    public void ToggleStarMoveStatus()
    {
        if (HexPlanetManager.PlanetRevolution)
        {
            HexPlanetManager.PlanetRevolution = false;
            Self.SunRevolution!.RotationDegrees = Vector3.Up * 180f;
            RenderingServer.GlobalShaderParameterSet(GlobalShaderParam.DirToSun,
                Self.SunMesh!.GlobalPosition.Normalized());
        }
        else
            HexPlanetManager.PlanetRevolution = true;
    }

    public void TogglePlanetMoveStatus()
    {
        if (HexPlanetManager.PlanetRotation)
        {
            HexPlanetManager.PlanetRotation = false;
            Self.PlanetAxis!.Rotation = Vector3.Zero;
        }
        else
            HexPlanetManager.PlanetRotation = true;
    }

    public void ToggleSatelliteMoveStatus()
    {
        if (HexPlanetManager.SatelliteRevolution || HexPlanetManager.SatelliteRotation)
        {
            HexPlanetManager.SatelliteRevolution = false;
            HexPlanetManager.SatelliteRotation = false;
            Self.LunarRevolution!.RotationDegrees = Vector3.Up * 180f;
            Self.MoonAxis!.Rotation = Vector3.Zero;
        }
        else
        {
            HexPlanetManager.SatelliteRevolution = true;
            HexPlanetManager.SatelliteRotation = true;
        }
    }
}