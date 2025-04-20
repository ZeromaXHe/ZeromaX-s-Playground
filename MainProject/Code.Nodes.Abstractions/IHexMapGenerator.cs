using Godot;
using GodotNodes.Abstractions;

namespace Nodes.Abstractions;

public enum LandGeneratorType
{
    Erosion, // Catlike Coding 的随机升降土地，然后应用侵蚀算法
    FractalNoise, // 叠加的分形噪声
    RealEarth, // 真实地球
}

public class MapRegion
{
    public int[] IcosahedronId;
}

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-17 10:33:17
public interface IHexMapGenerator : INode
{
    delegate int CreatingErosionLandEvent(RandomNumberGenerator rng, List<MapRegion> regions);

    event CreatingErosionLandEvent? CreatingErosionLand;
    int EmitCreatingErosionLand(RandomNumberGenerator rng, List<MapRegion> regions);
    event Action<RandomNumberGenerator>? ErodingLand;
    void EmitErodingLand(RandomNumberGenerator rng);

    delegate int CreatingFractalNoiseLandEvent(RandomNumberGenerator rng);

    event CreatingFractalNoiseLandEvent? CreatingFractalNoiseLand;
    int EmitCreatingFractalNoiseLand(RandomNumberGenerator rng);

    delegate int CreatingRealEarthLandEvent();

    event CreatingRealEarthLandEvent? CreatingRealEarthLand;
    int EmitCreatingRealEarthLand();

    int MapBoardX { get; }
    int MapBoardZ { get; }
    int RegionBorder { get; }
    int RegionCount { get; }
    LandGeneratorType LandGeneratorType { get; }
    float EvaporationFactor { get; }
    float PrecipitationFactor { get; }
    float RunoffFactor { get; }
    float SeepageFactor { get; }
    int WindDirection { get; }
    float WindStrength { get; }
    float StartingMoisture { get; }
    float RiverPercentage { get; }
    float ExtraLakeProbability { get; }
    float LowTemperature { get; }
    float HighTemperature { get; }
    float TemperatureJitter { get; }
    bool UseFixedSeed { get; }
    int Seed { get; set; }
}