using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Dependency;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Repos;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Resources.LandGenerators;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Services;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Nodes.LandGenerators;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// 基于 Sebastian 视频中的叠加多个分形噪声原理的陆地生成器
/// Author: Zhu XH
/// Date: 2025-03-20 19:51:43
[Tool]
public partial class FractalNoiseLandGenerator : Node
{
    public FractalNoiseLandGenerator() => InitServices();
    [Export] public LayeredFastNoise LayeredNoises { get; set; } = new();

    #region 服务与存储

    private ITileService _tileService;
    private ITileRepo _tileRepo;
    private IPlanetSettingService _planetSettingService;

    private void InitServices()
    {
        _tileService = Context.GetBean<ITileService>();
        _tileRepo = Context.GetBean<ITileRepo>();
        _planetSettingService = Context.GetBean<IPlanetSettingService>();
    }

    #endregion

    public int CreateLand(RandomNumberGenerator random)
    {
        var origin = new Vector3(random.Randf(), random.Randf(), random.Randf()) * _planetSettingService.Radius;
        var minNoise = float.MaxValue;
        var maxNoise = float.MinValue;
        foreach (var tile in _tileRepo.GetAll())
        {
            var noise = LayeredNoises.GetLayeredNoise3Dv(tile.UnitCentroid * _planetSettingService.Radius + origin);
            if (noise > maxNoise) maxNoise = noise;
            if (noise < minNoise) minNoise = noise;
        }

        var landCount = 0;
        foreach (var tile in _tileRepo.GetAll())
        {
            var noise = LayeredNoises.GetLayeredNoise3Dv(tile.UnitCentroid * _planetSettingService.Radius + origin);
            var noiseDiff = noise > 0 ? noise : noise - minNoise;
            var elevation = Mathf.RoundToInt(noise > 0
                ? _planetSettingService.DefaultWaterLevel +
                  (_planetSettingService.ElevationStep - _planetSettingService.DefaultWaterLevel) * noiseDiff / maxNoise
                : _planetSettingService.DefaultWaterLevel * noiseDiff / -minNoise);
            tile.Data = tile.Data with { Values = tile.Data.Values.WithElevation(elevation) };
            if (!tile.Data.IsUnderwater)
                landCount++;
        }

        GD.Print($"------ land tiles {landCount}, min noise: {minNoise}, max noise: {maxNoise} ------");
        return landCount;
    }
}