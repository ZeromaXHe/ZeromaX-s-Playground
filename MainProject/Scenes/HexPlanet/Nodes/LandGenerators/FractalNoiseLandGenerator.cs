using Domains.Models.Singletons.Planets;
using Domains.Repos.PlanetGenerates;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Dependency;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Resources.LandGenerators;

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

    private ITileRepo? _tileRepo;
    private IPlanetConfig? _planetConfig;

    private void InitServices()
    {
        _tileRepo = Context.GetBeanFromHolder<ITileRepo>();
        _planetConfig = Context.GetBeanFromHolder<IPlanetConfig>();
    }

    #endregion

    public int CreateLand(RandomNumberGenerator random)
    {
        var origin = new Vector3(random.Randf(), random.Randf(), random.Randf()) * _planetConfig!.Radius;
        var minNoise = float.MaxValue;
        var maxNoise = float.MinValue;
        foreach (var tile in _tileRepo!.GetAll())
        {
            var noise = LayeredNoises.GetLayeredNoise3Dv(tile.UnitCentroid * _planetConfig.Radius + origin);
            if (noise > maxNoise) maxNoise = noise;
            if (noise < minNoise) minNoise = noise;
        }

        var landCount = 0;
        foreach (var tile in _tileRepo.GetAll())
        {
            var noise = LayeredNoises.GetLayeredNoise3Dv(tile.UnitCentroid * _planetConfig.Radius + origin);
            var noiseDiff = noise > 0 ? noise : noise - minNoise;
            var elevation = Mathf.RoundToInt(noise > 0
                ? _planetConfig.DefaultWaterLevel +
                  (_planetConfig.ElevationStep - _planetConfig.DefaultWaterLevel) * noiseDiff / maxNoise
                : _planetConfig.DefaultWaterLevel * noiseDiff / -minNoise);
            tile.Data = tile.Data with { Values = tile.Data.Values.WithElevation(elevation) };
            if (!tile.Data.IsUnderwater)
                landCount++;
        }

        GD.Print($"------ land tiles {landCount}, min noise: {minNoise}, max noise: {maxNoise} ------");
        return landCount;
    }
}