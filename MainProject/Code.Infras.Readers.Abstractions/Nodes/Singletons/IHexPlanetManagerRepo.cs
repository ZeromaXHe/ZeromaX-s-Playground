using Domains.Models.Entities.PlanetGenerates;
using Domains.Models.ValueObjects.PlanetGenerates;
using Godot;
using Infras.Readers.Abstractions.Bases;
using Nodes.Abstractions;

namespace Infras.Readers.Abstractions.Nodes.Singletons;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 10:27:18
public interface IHexPlanetManagerRepo : ISingletonNodeRepo<IHexPlanetManager>
{
    float Radius { get; set; }
    int Divisions { get; set; }
    int ChunkDivisions { get; set; }
    int ElevationStep { get; set; }
    float StandardScale { get; }

    #region 噪声相关

    float ElevationPerturbStrength { get; }

    // 球面噪声采样逻辑
    Vector4 SampleNoise(Vector3 position);

    // 球面的扰动逻辑
    Vector3 Perturb(Vector3 position);
    void InitializeHashGrid(ulong seed);
    HexHash SampleHashGrid(Vector3 position);

    #endregion

    #region 特征

    float[] GetFeatureThreshold(int level);
    float GetWallHeight();
    float GetWallThickness();
    Vector3 WallThicknessOffset(Vector3 near, Vector3 far, bool toNear, float thickness);
    Vector3 WallLerp(Vector3 near, Vector3 far);

    #endregion

    #region 高度

    float UnitHeight { get; }
    int DefaultWaterLevel { get; set; }
    float MaxHeight { get; }
    float MaxHeightRatio { get; }
    float GetHeight(Tile tile);
    float GetOverrideHeight(Tile tile, HexTileDataOverrider tileDataOverrider);

    #endregion
}