using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Base;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entities.Civs;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Repos.Civs;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-08 16:55:08
public interface ICivRepo : IRepository<Civ>
{
    Civ Add(Color color);
}