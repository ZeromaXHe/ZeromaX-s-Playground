using Domains.Bases;

namespace Domains.Models.Entities.Civs;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-01 12:45
public class Unit(int id = -1) : Entity(id)
{
    public const int VisionRange = 3;
    public int TileId { get; set; }
}