using Domains.Models.Entities.PlanetGenerates;
using GodotNodes.Abstractions;

namespace Nodes.Abstractions;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-17 10:36:17
public interface IHexUnitPathPool : INode3D
{
    IHexUnitPath NewTask(IHexUnit unit, List<Tile> pathTiles);
}