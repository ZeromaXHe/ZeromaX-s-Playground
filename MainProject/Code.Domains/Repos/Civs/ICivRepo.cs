using Domains.Bases;
using Domains.Models.Entities.Civs;
using Godot;

namespace Domains.Repos.Civs;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-08 16:55:08
public interface ICivRepo : IRepository<Civ>
{
    Civ Add(Color color);
}