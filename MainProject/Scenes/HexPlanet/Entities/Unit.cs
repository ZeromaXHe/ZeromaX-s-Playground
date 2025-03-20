using ZeromaXsPlaygroundProject.Scenes.Framework.Base;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entities;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-01 12:45
public class Unit(int id = -1) : AEntity(id)
{
    public const int VisionRange = 3;
    public int TileId { get; set; }
}