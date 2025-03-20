using Godot;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Services;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-12 07:52
public interface IPlanetSettingService
{
    float Radius { get; set; }
    int ChunkDivisions { get; set; }
    int Divisions { get; set; }
    float UnitHeight { get; }
    float MaxHeight { get; }
    float MaxHeightRatio { get; }
    int ElevationStep { get; set; } // 对应高程最大值
    float StandardScale { get; }
    int DefaultWaterLevel { get; set; }

    #region 特征

    float[] GetFeatureThreshold(int level);
    float GetWallHeight();
    float GetWallThickness();
    Vector3 WallThicknessOffset(Vector3 near, Vector3 far, bool toNear, float thickness);
    Vector3 WallLerp(Vector3 near, Vector3 far);

    #endregion
}