using System.Collections.Generic;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service;

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