using Domains.Services.Abstractions.Nodes.LandGenerators;
using Godot;
using Infras.Readers.Abstractions.Nodes.Singletons;
using Infras.Readers.Abstractions.Nodes.Singletons.LandGenerators;
using Infras.Writers.Abstractions.PlanetGenerates;
using Nodes.Abstractions.LandGenerators;

namespace Domains.Services.Nodes.LandGenerators;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 20:14:56
public class FractalNoiseLandGeneratorService(
    IFractalNoiseLandGeneratorRepo fractalNoiseLandGeneratorRepo,
    IHexPlanetManagerRepo hexPlanetManagerRepo,
    ITileRepo tileRepo) : IFractalNoiseLandGeneratorService
{
    private IFractalNoiseLandGenerator Self => fractalNoiseLandGeneratorRepo.Singleton!;
    
    public int CreateLand(RandomNumberGenerator random)
    {
        var origin = new Vector3(random.Randf(), random.Randf(), random.Randf()) * hexPlanetManagerRepo.Radius;
        var minNoise = float.MaxValue;
        var maxNoise = float.MinValue;
        foreach (var tile in tileRepo.GetAll())
        {
            var noise = Self.GetLayeredNoises().GetLayeredNoise3Dv(tile.UnitCentroid * hexPlanetManagerRepo.Radius + origin);
            if (noise > maxNoise) maxNoise = noise;
            if (noise < minNoise) minNoise = noise;
        }

        var landCount = 0;
        foreach (var tile in tileRepo.GetAll())
        {
            var noise = Self.GetLayeredNoises().GetLayeredNoise3Dv(tile.UnitCentroid * hexPlanetManagerRepo.Radius + origin);
            var noiseDiff = noise > 0 ? noise : noise - minNoise;
            var elevation = Mathf.RoundToInt(noise > 0
                ? hexPlanetManagerRepo.DefaultWaterLevel +
                  (hexPlanetManagerRepo.ElevationStep - hexPlanetManagerRepo.DefaultWaterLevel) * noiseDiff / maxNoise
                : hexPlanetManagerRepo.DefaultWaterLevel * noiseDiff / -minNoise);
            tile.Data = tile.Data with { Values = tile.Data.Values.WithElevation(elevation) };
            if (!tile.Data.IsUnderwater)
                landCount++;
        }

        GD.Print($"------ land tiles {landCount}, min noise: {minNoise}, max noise: {maxNoise} ------");
        return landCount;
    }
}