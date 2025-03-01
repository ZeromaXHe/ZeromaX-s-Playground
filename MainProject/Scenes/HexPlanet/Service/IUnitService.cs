using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service;

public interface IUnitService
{
    Unit Add();

    #region 透传存储库方法

    void Delete(int id);
    void Truncate();

    #endregion
}