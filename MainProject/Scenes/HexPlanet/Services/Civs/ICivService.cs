using System.Collections.Generic;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entities.Civs;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Services.Civs;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-10 10:03:10
public interface ICivService
{
    Civ Add(Color color);

    #region 透传存储库方法

    Civ GetById(int id);
    IEnumerable<Civ> GetAll();
    void Truncate();

    #endregion
}