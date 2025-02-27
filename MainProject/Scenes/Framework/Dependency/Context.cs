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
    public static void Register(string singleton, object bean) => Singletons.Add(singleton, bean);
    public static bool Destroy(string singleton) => Singletons.Remove(singleton);
    public static void Reboot() => Singletons.Clear();

    // 仿 setter 注入写法：
    // private readonly Lazy<ITileRepo> _tileRepo = new(() => Context.GetBean<ITileRepo>());
    public static T GetBean<T>() where T : class =>
        Singletons.GetValueOrDefault(typeof(T).Name) as T; // 不能直接 nameof(T)，因为结果是 "T"

    public static void Init()
    {
        Reboot();
        var chunkRepo = new ChunkRepo();
        var tileRepo = new TileRepo();
        var faceRepo = new FaceRepo();
        var pointRepo = new PointRepo();
        Register(nameof(IChunkRepo), chunkRepo);
        Register(nameof(ITileRepo), tileRepo);
        Register(nameof(IFaceRepo), faceRepo);
        Register(nameof(IPointRepo), pointRepo);
        var chunkService = new ChunkService(chunkRepo);
        var faceService = new FaceService(faceRepo, pointRepo);
        var pointService = new PointService(faceService, pointRepo);
        var tileService = new TileService(chunkService, faceService, tileRepo, faceRepo, pointRepo);
        var selectViewService = new SelectViewService(tileService);
        Register(nameof(IChunkService), chunkService);
        Register(nameof(IFaceService), faceService);
        Register(nameof(IPointService), pointService);
        Register(nameof(ITileService), tileService);
        Register(nameof(ISelectViewService), selectViewService);
    }
}