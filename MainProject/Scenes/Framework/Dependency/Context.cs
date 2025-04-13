#nullable enable
using System.Collections.Generic;
using Apps.Applications.Features;
using Apps.Applications.Features.Impl;
using Apps.Applications.Tiles;
using Apps.Applications.Tiles.Impl;
using Apps.Applications.Uis;
using Apps.Applications.Uis.Impl;
using Commons.Frameworks;
using Domains.Events.Tiles;
using Domains.Models.Singletons.Planets;
using Domains.Models.Singletons.Planets.Impl;
using Domains.Repos.Civs;
using Domains.Repos.PlanetGenerates;
using Domains.Services.Caches;
using Domains.Services.Caches.Impl;
using Domains.Services.Civs;
using Domains.Services.Civs.Impl;
using Domains.Services.Navigations;
using Domains.Services.Navigations.Impl;
using Domains.Services.PlanetGenerates;
using Domains.Services.PlanetGenerates.Impl;
using Domains.Services.Shaders;
using Domains.Services.Shaders.Impl;
using Domains.Services.Uis;
using Domains.Services.Uis.Impl;
using Infras.Repos.Impl.Civs;
using Infras.Repos.Impl.PlanetGenerates;

namespace ZeromaXsPlaygroundProject.Scenes.Framework.Dependency;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-02-24 13:35
public class Context : IContext
{
    public static T GetBeanFromHolder<T>() where T : class
    {
        ContextHolder.Context ??= new Context();
        return ContextHolder.Context.GetBean<T>()!;
    }

    // 目前逻辑不校验类型是否正确，依赖于使用者自己保证正确
    private readonly Dictionary<string, object> _singletons = new();
    private bool _initialized;
    private void Register(string singleton, object bean) => _singletons.Add(singleton, bean);
    private bool Destroy(string singleton) => _singletons.Remove(singleton);
    private void Reboot() => _singletons.Clear();

    // 仿 setter 注入写法：
    // private readonly Lazy<ITileRepo> _tileRepo = new(() => Context.GetBean<ITileRepo>());
    public T? GetBean<T>() where T : class
    {
        // 现在 4.4 的生命周期有点看不懂了，运行游戏时居然先调用 HexGridChunk 的构造函数而不是 HexPlanetManager 的？！
        // 所以只能在这里初始化，否则直接 GetBean null 容易把编辑器和游戏运行搞崩。
        if (!_initialized) Init();
        // 不能直接 nameof(T)，因为结果是 "T"
        return _singletons.GetValueOrDefault(typeof(T).Name) as T;
    }

    private void Init()
    {
        Reboot();
        _initialized = true;
        // 单例
        var planetConfig = new PlanetConfig();
        var noiseConfig = new NoiseConfig(planetConfig);
        Register(nameof(IPlanetConfig), planetConfig);
        Register(nameof(INoiseConfig), noiseConfig);
        // 存储
        var chunkRepo = new ChunkRepo();
        var tileRepo = new TileRepo(planetConfig, noiseConfig);
        var featureRepo = new FeatureRepo();
        var faceRepo = new FaceRepo();
        var pointRepo = new PointRepo();
        var unitRepo = new UnitRepo();
        var civRepo = new CivRepo();
        Register(nameof(IChunkRepo), chunkRepo);
        Register(nameof(ITileRepo), tileRepo);
        Register(nameof(IFeatureRepo), featureRepo);
        Register(nameof(IFaceRepo), faceRepo);
        Register(nameof(IPointRepo), pointRepo);
        Register(nameof(IUnitRepo), unitRepo);
        Register(nameof(ICivRepo), civRepo);
        // 服务
        var lodMeshCacheService = new LodMeshCacheService();
        var unitService = new UnitService(unitRepo);
        var civService = new CivService(civRepo);
        var pointService = new PointService(faceRepo, pointRepo);
        var chunkService = new ChunkService(pointService, pointRepo, faceRepo, planetConfig, chunkRepo);
        var tileService = new TileService(chunkService, faceRepo, pointService, pointRepo, planetConfig, tileRepo);
        var tileSearchService = new TileSearchService(pointRepo, chunkRepo, tileRepo, planetConfig);
        var tileShaderService = new TileShaderService(tileRepo, unitService, civService, planetConfig);
        var editorService = new EditorService(tileRepo);
        var miniMapService = new MiniMapService(tileRepo);
        var selectViewService = new SelectViewService(chunkRepo, tileService, tileRepo, tileSearchService,
            planetConfig, editorService);
        Register(nameof(ILodMeshCacheService), lodMeshCacheService);
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
        // 应用
        var featureApplication = new FeatureApplication(featureRepo);
        TileShaderEvent.Instance.TileExplored += featureApplication.ExploreFeatures;
        var tileShaderApplication = new TileShaderApplication(tileSearchService, tileShaderService);
        TileShaderEvent.Instance.RangeVisibilityIncreased += tileShaderApplication.IncreaseVisibility;
        var hexPlanetHudApplication = new HexPlanetHudApplication(planetConfig, chunkRepo, tileRepo, pointRepo,
            editorService, miniMapService);
        Register(nameof(IFeatureApplication), featureApplication);
        Register(nameof(ITileShaderApplication), tileShaderApplication);
        Register(nameof(IHexPlanetHudApplication), hexPlanetHudApplication);
    }
}