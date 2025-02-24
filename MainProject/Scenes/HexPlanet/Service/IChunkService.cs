using System.Collections.Generic;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service;

public interface IChunkService
{
    #region 透传存储库方法

    Chunk GetById(int id);
    int GetCount();
    IEnumerable<Chunk> GetAll();

    #endregion
    // 最近邻搜索
    Chunk SearchNearest(Vector3 pos);
    int GetChunkCount();
    void ClearData();
    void InitChunks(int chunkDivisions);
}