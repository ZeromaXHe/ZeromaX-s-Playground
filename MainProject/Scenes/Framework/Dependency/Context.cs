using System.Collections.Generic;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Repo;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Repo.Impl;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service.Impl;

namespace ZeromaXsPlaygroundProject.Scenes.Framework.Dependency;

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
        Register(nameof(IChunkRepo), chunkRepo);
        Register(nameof(ITileRepo), tileRepo);
        Register(nameof(IFaceRepo), faceRepo);
        Register(nameof(IPointRepo), pointRepo);
        Register(nameof(IUnitRepo), unitRepo);
        var unitService = new UnitService(unitRepo);
        var faceService = new FaceService(faceRepo, pointRepo);
        var pointService = new PointService(faceService, pointRepo);
        var chunkService = new ChunkService(pointService, chunkRepo);
        var tileService = new TileService(chunkService, faceService, pointService, tileRepo);
        var tileSearchService = new TileSearchService(tileService);
        var tileShaderService = new TileShaderService(tileService, tileSearchService, unitService);
        var selectViewService = new SelectViewService(tileService, tileSearchService);
        Register(nameof(IUnitService), unitService);
        Register(nameof(IFaceService), faceService);
        Register(nameof(IPointService), pointService);
        Register(nameof(IChunkService), chunkService);
        Register(nameof(ITileService), tileService);
        Register(nameof(ITileSearchService), tileSearchService);
        Register(nameof(ITileShaderService), tileShaderService);
        Register(nameof(ISelectViewService), selectViewService);
    }
}