using Domains.Models.Bases;
using Godot;

namespace Domains.Models.Entities.Civs;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-08 16:55:51
public class Civ(Color color, string name, int id) : Entity(id)
{
    public Color Color { get; } = color;
    public string Name { get; } = name;
    public List<int> TileIds = [];
}