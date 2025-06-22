namespace TO.Abstractions.Views.Maps;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-07 10:15:07
public interface IHexMapGenerator
{
    int MapBoardX { get; }
    int MapBoardZ { get; }
    int RegionBorder { get; }
    int RegionCount { get; }

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

    #region Catlike Coding 侵蚀算法设置

    int LandPercentage { get; }
    int ChunkSizeMin { get; }
    int ChunkSizeMax { get; }
    float HighRiseProbability { get; }
    float SinkProbability { get; }
    float JitterProbability { get; }
    int ErosionPercentage { get; }

    #endregion
}