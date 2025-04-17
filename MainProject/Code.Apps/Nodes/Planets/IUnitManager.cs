using Domains.Models.Entities.PlanetGenerates;
using GodotNodes.Abstractions;

namespace Apps.Nodes.Planets;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-17 10:25:17
public interface IUnitManager : INode3D
{
    int PathFromTileId { get; set; }
    void ClearAllUnits();
    void AddUnit(int tileId, float orientation);
    void RemoveUnit(int unitId);
    void FindPath(Tile? tile);
}