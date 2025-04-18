using Apps.Queries.Applications.Base;
using Domains.Models.Entities.PlanetGenerates;

namespace Apps.Queries.Applications.Planets;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-14 16:00:14
public interface IHexPlanetManagerApp: INodeApp
{
    bool UpdateUiInEditMode();
    Tile? GetTileUnderCursor();
}