using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Dependency;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Resources.LandGenerators;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node.LandGenerators;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// 基于 Sebastian 视频中的叠加多个分形噪声原理的陆地生成器
/// Author: Zhu XH
/// Date: 2025-03-20 19:51:43
[Tool]
public partial class SebastianFractalNoiseLandGenerator : Node3D
{
    public SebastianFractalNoiseLandGenerator() => InitServices();
    [Export] public LayeredFastNoise LayeredNoises { get; set; } = new();

    #region 服务

    private ITileService _tileService;
    private IPlanetSettingService _planetSettingService;

    private void InitServices()
    {
        _tileService = Context.GetBean<ITileService>();
        _planetSettingService = Context.GetBean<IPlanetSettingService>();
    }

    #endregion

    public int CreateLand(RandomNumberGenerator random)
    {
        var origin = new Vector3(random.Randf(), random.Randf(), random.Randf()) * _planetSettingService.Radius;
        var minNoise = float.MaxValue;
        var maxNoise = float.MinValue;
        foreach (var tile in _tileService.GetAll())
        {
            var noise = LayeredNoises.GetLayeredNoise3Dv(tile.UnitCentroid * _planetSettingService.Radius + origin);
            if (noise > maxNoise) maxNoise = noise;
            if (noise < minNoise) minNoise = noise;
        }

        var minStep = Mathf.Abs(minNoise / _planetSettingService.DefaultWaterLevel);
        var maxStep = Mathf.Abs(maxNoise / (_planetSettingService.ElevationStep + 1 - _planetSettingService.DefaultWaterLevel));
        var step = Mathf.Max(minStep, maxStep);
        var landCount = 0;
        foreach (var tile in _tileService.GetAll())
        {
            var noise = LayeredNoises.GetLayeredNoise3Dv(tile.UnitCentroid * _planetSettingService.Radius + origin);
            var elevation = _planetSettingService.DefaultWaterLevel + (int)(noise / step);
            tile.Data = tile.Data with { Values = tile.Data.Values.WithElevation(elevation) };
            if (!tile.Data.IsUnderwater)
                landCount++;
        }

        GD.Print($"------ land tiles {landCount}, min noise: {minNoise}, max noise: {maxNoise}, maxStep: {
            maxStep}, minStep: {minStep}, step: {step} ------");
        return landCount;
    }
}