using TO.Apps.Commands.Abstractions.Planets;
using TO.Domains.Services.Abstractions.Planets;

namespace TO.Apps.Commands.Planets;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-28 22:09:27
public class PlanetCommander : IPlanetCommander
{
    public void DrawHexSphereMesh()
    {
        // ClearOldData();
        // InitHexSphere();
        // InitCivilization();
        // RefreshAllTiles();
    }

    // private void ClearOldData()
    // {
    //     // 必须先清理单位，否则相关可见度事件会查询地块，放最后会空引用异常
    //     unitManagerService.ClearAllUnits(); // unitManager 不是 [Tool]，在编辑器时会是 null
    //     chunkRepo.Truncate();
    //     tileRepo.Truncate();
    //     pointRepo.Truncate();
    //     faceRepo.Truncate();
    //     civRepo.Truncate();
    //     selectTileViewerService.ClearPath();
    //     // 清空分块
    //     hexGridChunkRepo.ClearOldData();
    //     chunkLoaderRepo.Singleton!.ClearOldData();
    //     featurePreviewManagerRepo.Singleton!.ClearForData();
    //     featureMeshManagerRepo.Singleton!.ClearOldData();
    //     lodMeshCache.RemoveAllLodMeshes();
    // }
}