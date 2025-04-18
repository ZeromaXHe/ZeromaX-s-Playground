using Domains.Models.Entities.PlanetGenerates;
using Godot;
using GodotNodes.Abstractions;

namespace Nodes.Abstractions;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-16 15:34:16
public interface IHexPlanetManager : INode3D
{
    delegate void NewPlanetGeneratedEvent();

    event NewPlanetGeneratedEvent NewPlanetGenerated;

    void EmitNewPlanetGenerated();

    #region export 变量

    float Radius { get; set; }
    int Divisions { get; set; }
    int ChunkDivisions { get; set; }
    Texture2D? NoiseSource { get; }
    ulong Seed { get; set; }

    // 行星公转
    bool PlanetRevolution { set; }

    // 行星自转
    bool PlanetRotation { set; }

    // 卫星公转
    bool SatelliteRevolution { set; }

    // 卫星自转
    bool SatelliteRotation { set; }

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

    // TODO: 下面两个方法，相关逻辑在 APP 层和节点层上下翻飞，需要重构
    bool UpdateUiInEditMode();
    Tile? GetTileUnderCursor();
    Vector3 GetTileCollisionPositionUnderCursor();

    // 锁定经纬网的显示
    void FixLatLon(bool toggle);
    Vector3 ToPlanetLocal(Vector3 global);
}