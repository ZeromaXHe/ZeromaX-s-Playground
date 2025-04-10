using System.Collections.Generic;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entities.Civs;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Repos.Civs;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Services.Civs.Impl;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-10 10:03:58
public class CivService(ICivRepo civRepo) : ICivService
{
    public Civ Add(Color color) => civRepo.Add(color);

    #region 透传存储库方法

    public Civ GetById(int id) => civRepo.GetById(id);
    public IEnumerable<Civ> GetAll() => civRepo.GetAll();
    public void Truncate() => civRepo.Truncate();

    #endregion
}