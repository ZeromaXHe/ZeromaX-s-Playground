namespace Domains.Services.Abstractions.Nodes.Singletons.Planets;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 20:10:18
public interface ICelestialMotionManagerService
{
    void UpdateLunarDist();
    void UpdateMoonMeshRadius();

    // 更新天体旋转
    void UpdateStellarRotation(float delta);
    void ToggleStarMoveStatus();
    void TogglePlanetMoveStatus();
    void ToggleSatelliteMoveStatus();
}