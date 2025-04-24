using Infras.Readers.Abstractions.Nodes.Singletons.Planets;
using Infras.Readers.Bases;
using Nodes.Abstractions.Planets;

namespace Infras.Readers.Nodes.Singletons.Planets;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 19:41:11
public class CelestialMotionManagerRepo : SingletonNodeRepo<ICelestialMotionManager>, ICelestialMotionManagerRepo
{
    public event Action<float>? SatelliteDistRatioChanged;
    private void OnSatelliteDistRatioChanged(float value) => SatelliteDistRatioChanged?.Invoke(value);
    public event Action<float>? SatelliteRadiusRatioChanged;
    private void OnSatelliteRadiusRatioChanged(float value) => SatelliteRadiusRatioChanged?.Invoke(value);

    protected override void ConnectNodeEvents()
    {
        Singleton!.SatelliteDistRatioChanged += OnSatelliteDistRatioChanged;
        Singleton.SatelliteRadiusRatioChanged += OnSatelliteRadiusRatioChanged;
    }

    protected override void DisconnectNodeEvents()
    {
        Singleton!.SatelliteDistRatioChanged -= OnSatelliteDistRatioChanged;
        Singleton.SatelliteRadiusRatioChanged -= OnSatelliteRadiusRatioChanged;
    }
}