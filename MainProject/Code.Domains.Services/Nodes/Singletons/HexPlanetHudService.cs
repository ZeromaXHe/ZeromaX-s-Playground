using Domains.Services.Abstractions.Nodes;
using Domains.Services.Abstractions.Nodes.Singletons;
using Infras.Readers.Abstractions.Nodes.Singletons;

namespace Domains.Services.Nodes.Singletons;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 20:21:49
public class HexPlanetHudService(IHexPlanetHudRepo hexPlanetHudRepo, IHexPlanetManagerRepo hexPlanetManagerRepo)
    : IHexPlanetHudService
{
    public void InitElevationAndWaterVSlider()
    {
        // 按照指定的高程分割数量确定 UI
        var hud = hexPlanetHudRepo.Singleton!;
        hud.ElevationVSlider!.MaxValue = hexPlanetManagerRepo.ElevationStep;
        hud.ElevationVSlider.TickCount = hexPlanetManagerRepo.ElevationStep + 1;
        hud.WaterVSlider!.MaxValue = hexPlanetManagerRepo.ElevationStep;
        hud.WaterVSlider.TickCount = hexPlanetManagerRepo.ElevationStep + 1;
    }
}