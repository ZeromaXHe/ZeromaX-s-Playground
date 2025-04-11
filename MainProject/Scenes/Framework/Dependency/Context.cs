using System.Collections.Generic;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Repos;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Repos.Civs;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Repos.Civs.Impl;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Repos.Impl;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Services;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Services.Civs;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Services.Civs.Impl;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Services.Impl;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Services.MiniMap;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Services.MiniMap.Impl;

namespace ZeromaXsPlaygroundProject.Scenes.Framework.Dependency;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-02-24 13:35
public static class Context
{
    // 目前逻辑不校验类型是否正确，依赖于使用者自己保证正确
    private static readonly Dictionary<string, object> Singletons = new();
    private static bool _initialized;
    public static void Register(string singleton, object bean) => Singletons.Add(singleton, bean);
    public static bool Destroy(string singleton) => Singletons.Remove(singleton);
    public static void Reboot() => Singletons.Clear();

    // 仿 setter 注入写法：
    // private readonly Lazy<ITileRepo> _tileRepo = new(() => Context.GetBean<ITileRepo>());
    public static T GetBean<T>() where T : class
    {
        // 现在 4.4 的生命周期有点看不懂了，运行游戏时居然先调用 HexGridChunk 的构造函数而不是 HexPlanetManager 的？！
        // 所以只能在这里初始化，否则直接 GetBean null 容易把编辑器和游戏运行搞崩。
        if (!_initialized) Init();
        // 不能直接 nameof(T)，因为结果是 "T"
        return Singletons.GetValueOrDefault(typeof(T).Name) as T;
    }

    public static void Init()
    {
        Reboot();
        _initialized = true;
        var chunkRepo = new ChunkRepo();
        var tileRepo = new TileRepo();
        var faceRepo = new FaceRepo();
        var pointRepo = new PointRepo();
        var unitRepo = new UnitRepo();
        var civRepo = new CivRepo();
        Register(nameof(IChunkRepo), chunkRepo);
        Register(nameof(ITileRepo), tileRepo);
        Register(nameof(IFaceRepo), faceRepo);
        Register(nameof(IPointRepo), pointRepo);
        Register(nameof(IUnitRepo), unitRepo);
        Register(nameof(ICivRepo), civRepo);
        var lodMeshCacheService = new LodMeshCacheService();
        var planetSettingService = new PlanetSettingService();
        var noiseService = new NoiseService(planetSettingService);
        var unitService = new UnitService(unitRepo);
        var civService = new CivService(civRepo);
        var pointService = new PointService(faceRepo, pointRepo);
        var chunkService = new ChunkService(pointService, pointRepo, faceRepo, planetSettingService, chunkRepo);
        var tileService = new TileService(chunkService, chunkRepo, faceRepo, pointService, pointRepo,
            planetSettingService, noiseService, tileRepo);
        var tileSearchService = new TileSearchService(pointRepo, chunkRepo, tileRepo, planetSettingService);
        var tileShaderService = new TileShaderService(tileRepo, tileSearchService, unitService,
            civService, planetSettingService);
        var editorService = new EditorService(tileRepo);
        var miniMapService = new MiniMapService(tileService, tileRepo);
        var selectViewService = new SelectViewService(chunkRepo, tileService, tileRepo, tileSearchService,
            planetSettingService, editorService);
        Register(nameof(ILodMeshCacheService), lodMeshCacheService);
        Register(nameof(IPlanetSettingService), planetSettingService);
        Register(nameof(INoiseService), noiseService);
        Register(nameof(IUnitService), unitService);
        Register(nameof(ICivService), civService);
        Register(nameof(IPointService), pointService);
        Register(nameof(IChunkService), chunkService);
        Register(nameof(ITileService), tileService);
        Register(nameof(ITileSearchService), tileSearchService);
        Register(nameof(ITileShaderService), tileShaderService);
        Register(nameof(IEditorService), editorService);
        Register(nameof(IMiniMapService), miniMapService);
        Register(nameof(ISelectViewService), selectViewService);
    }
}