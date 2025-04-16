using Apps.Models.Responses;
using Domains.Models.Entities.PlanetGenerates;
using Godot;

namespace Apps.Applications.Uis;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-13 13:00:13
public interface IHexPlanetHudApplication
{
    #region 上下文节点

    void OnReady();
    void OnExitTree();
    void OnProcess(double delta);

    #endregion

    TileInfoRespDto GetTileInfo(Tile tile);
    ImageTexture GenerateRectMap();
}