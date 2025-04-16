using Domains.Models.Entities.PlanetGenerates;
using Godot;
using GodotNodes.Abstractions;

namespace Apps.Nodes;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-16 15:34:16
public interface IHexPlanetManager : INode3D
{
    delegate void NewPlanetGeneratedEvent();

    event NewPlanetGeneratedEvent NewPlanetGenerated;

    float Radius { get; set; }
    int Divisions { get; set; }
    int ChunkDivisions { get; set; }

    // 行星公转
    bool PlanetRevolution { set; }

    // 行星自转
    bool PlanetRotation { set; }

    // 卫星公转
    bool SatelliteRevolution { set; }

    // 卫星自转
    bool SatelliteRotation { set; }

    bool UpdateUiInEditMode();
    Tile? GetTileUnderCursor();
    void SelectEditingTile(Tile tile);
    void CleanEditingTile();
    void FindPath(Tile? tile);

    // 锁定经纬网的显示
    void FixLatLon(bool toggle);
    Vector3 GetOrbitCameraFocusPos();
    Vector3 ToPlanetLocal(Vector3 global);
}