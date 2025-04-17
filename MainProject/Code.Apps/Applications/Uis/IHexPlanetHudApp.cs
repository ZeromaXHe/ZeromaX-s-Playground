using Apps.Applications.Base;
using Apps.Models.Responses;
using Domains.Models.Entities.PlanetGenerates;
using Godot;

namespace Apps.Applications.Uis;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-13 13:00:13
public interface IHexPlanetHudApp: INodeApp
{
    TileInfoRespDto GetTileInfo(Tile tile);
    ImageTexture GenerateRectMap();
}