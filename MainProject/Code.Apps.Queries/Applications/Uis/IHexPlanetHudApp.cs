using Apps.Models.Responses;
using Apps.Queries.Applications.Base;
using Domains.Models.Entities.PlanetGenerates;
using Godot;

namespace Apps.Queries.Applications.Uis;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-13 13:00:13
public interface IHexPlanetHudApp: INodeApp
{
    TileInfoRespDto GetTileInfo(Tile tile);
    ImageTexture GenerateRectMap();
}