using GodotNodes.Abstractions;

namespace Nodes.Abstractions.Planets;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-17 10:25:17
public interface IUnitManager : INode3D
{
    event Action? PathFromTileIdSetZero;
    int PathFromTileId { get; set; }
    Dictionary<int, IHexUnit> Units { get; }
    IHexUnitPathPool? GetHexUnitPathPool();
    void AddUnit(int tileId, float orientation);
    void RemoveUnit(int unitId);
}