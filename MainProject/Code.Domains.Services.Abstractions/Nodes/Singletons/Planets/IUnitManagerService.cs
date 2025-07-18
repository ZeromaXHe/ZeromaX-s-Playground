using Domains.Models.Entities.PlanetGenerates;

namespace Domains.Services.Abstractions.Nodes.Singletons.Planets;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 20:11:18
public interface IUnitManagerService
{
    void FindPath(Tile? tile);
    void ClearAllUnits();
}