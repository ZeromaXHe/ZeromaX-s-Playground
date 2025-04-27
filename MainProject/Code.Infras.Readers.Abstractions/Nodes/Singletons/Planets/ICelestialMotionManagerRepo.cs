using Infras.Readers.Abstractions.Bases;
using Nodes.Abstractions.Planets;

namespace Infras.Readers.Abstractions.Nodes.Singletons.Planets;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 19:39:18
public interface ICelestialMotionManagerRepo : ISingletonNodeRepo<ICelestialMotionManager>
{
    event Action<float>? SatelliteDistRatioChanged;
    event Action<float>? SatelliteRadiusRatioChanged;
    event Action? StarMoveStatusToggled;
    event Action? PlanetMoveStatusToggled;
    event Action? SatelliteMoveStatusToggled;
}