using System.Collections.Generic;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entities;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Services;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-01 00:45
public interface IUnitService
{
    Unit Add();

    #region 透传存储库方法

    Unit GetById(int id);
    IEnumerable<Unit> GetAll();
    void Delete(int id);
    void Truncate();

    #endregion
}