using Domains.Models.Entities.Civs;
using Godot;
using Infras.Writers.Abstractions.Bases;

namespace Infras.Writers.Abstractions.Civs;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-08 16:55:08
public interface ICivRepo : IRepository<Civ>
{
    Civ Add(Color color);
}