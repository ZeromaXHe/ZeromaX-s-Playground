using Domains.Models.Entities.PlanetGenerates;
using Domains.Models.ValueObjects.PlanetGenerates;
using Godot;
using GodotNodes.Abstractions;
using GodotNodes.Abstractions.Addition;

namespace Nodes.Abstractions;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-16 15:34:16
public interface IHexPlanetManager : INode3D
{
    event Action? NewPlanetGenerated;
    event Action<float>? RadiusChanged;

    void EmitNewPlanetGenerated();

    #region export 变量

    float Radius { get; set; }
    int Divisions { get; set; }
    int ChunkDivisions { get; set; }
    Texture2D? NoiseSource { get; }
    ulong Seed { get; set; }

    // 行星公转
    bool PlanetRevolution { get; set; }

    // 行星自转
    bool PlanetRotation { get; set; }

    // 卫星公转
    bool SatelliteRevolution { get; set; }

    // 卫星自转
    bool SatelliteRotation { get; set; }

    #endregion

    float OldRadius { get; set; }
    int OldDivisions { get; set; }
    int OldChunkDivisions { get; set; }
    float LastUpdated { get; set; }

    #region on-ready 节点

    Node3D? PlanetContainer { get; }
    Node3D? PlanetAtmosphere { get; }
    MeshInstance3D? GroundPlaceHolder { get; }
    Camera3D? PlanetCamera { get; }

    #endregion

    #region 噪声相关

    Image? NoiseSourceImage { get; set; }

    #endregion

    #region 星球设置

    float UnitHeight { get; }
    float MaxHeight { get; }
    float MaxHeightRatio { get; }
    int ElevationStep { get; set; } // 对应高程最大值
    float StandardScale { get; }
    int DefaultWaterLevel { get; set; }

    #endregion

    Vector3 GetTileCollisionPositionUnderCursor();

    Vector3 ToPlanetLocal(Vector3 global);
}