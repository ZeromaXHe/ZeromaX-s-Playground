using System.Diagnostics.CodeAnalysis;
using Apps.Commands;
using Apps.Commands.ChunkManagers;
using Apps.Commands.LandGenerators;
using Apps.Commands.Planets;
using Apps.Queries.Abstractions.Features;
using Apps.Queries.Abstractions.Tiles;
using Apps.Queries.Applications.Features;
using Apps.Queries.Applications.Planets;
using Apps.Queries.Applications.Planets.Impl;
using Apps.Queries.Applications.Tiles;
using Apps.Queries.Applications.Uis;
using Apps.Queries.Applications.Uis.Impl;
using Autofac;
using Contexts.Abstractions;
using Domains.Services.Abstractions.Nodes;
using Domains.Services.Abstractions.Nodes.ChunkManagers;
using Domains.Services.Abstractions.Nodes.LandGenerators;
using Domains.Services.Abstractions.Nodes.Planets;
using Domains.Services.Abstractions.PlanetGenerates;
using Domains.Services.Abstractions.Searches;
using Domains.Services.Abstractions.Shaders;
using Domains.Services.Abstractions.Uis;
using Domains.Services.Nodes;
using Domains.Services.Nodes.ChunkManagers;
using Domains.Services.Nodes.LandGenerators;
using Domains.Services.Nodes.Planets;
using Domains.Services.PlanetGenerates;
using Domains.Services.Searches;
using Domains.Services.Shaders;
using Domains.Services.Uis;
using GodotNodes.Abstractions;
using Infras.Readers.Abstractions.Caches;
using Infras.Readers.Abstractions.Nodes;
using Infras.Readers.Abstractions.Nodes.Singletons;
using Infras.Readers.Abstractions.Nodes.Singletons.ChunkManagers;
using Infras.Readers.Abstractions.Nodes.Singletons.LandGenerators;
using Infras.Readers.Abstractions.Nodes.Singletons.Planets;
using Infras.Readers.Caches;
using Infras.Readers.Nodes.Singletons;
using Infras.Readers.Nodes.Singletons.ChunkManagers;
using Infras.Readers.Nodes.Singletons.LandGenerators;
using Infras.Readers.Nodes.Singletons.Planets;
using Infras.Writers.Abstractions.Civs;
using Infras.Writers.Abstractions.PlanetGenerates;
using Infras.Writers.Civs;
using Infras.Writers.PlanetGenerates;

namespace Contexts;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-02-24 13:35
public class Context : IContext
{
    public static T GetBeanFromHolder<T>() where T : class
    {
        ContextHolder.BeanContext ??= new Context();
        return ContextHolder.BeanContext.GetBean<T>()!;
    }

    public static bool RegisterSingletonToHolder<T>(T singleton) where T : INode
    {
        ContextHolder.BeanContext ??= new Context();
        return ContextHolder.BeanContext.RegisterSingletonNode(singleton);
    }

    private IContainer? _container;

    public T GetBean<T>() where T : class
    {
        // 现在 4.4 的生命周期有点看不懂了，运行游戏时居然先调用 HexGridChunk 的构造函数而不是 HexPlanetManager 的？！
        // 所以只能在这里初始化，否则直接 GetBean null 容易把编辑器和游戏运行搞崩。
        if (_container == null) Init();
        return _container.Resolve<T>();
    }

    private NodeRegister? _nodeRegister;

    public bool RegisterSingletonNode<T>(T singleton) where T : INode
    {
        if (_nodeRegister == null) Init();
        return _nodeRegister.RegisterSingleton(singleton);
    }

    [MemberNotNull(nameof(_nodeRegister), nameof(_container))]
    private void Init()
    {
        // 测试过，RegisterType 的顺序不影响注入结果（就是说不要求被依赖的放在前面），毕竟只是 Builder 的顺序
        var builder = new ContainerBuilder();
        // 默认是瞬态 Instance，单例需要加 .SingleInstance()
        // TODO: 替换为扫描程序集注入？总之不是这样手写，不然容易漏……（一旦漏了，并不会报错，只是拿不到依赖）
        // ===== 基础设施层 =====
        // 写库
        builder.RegisterType<ChunkRepo>().As<IChunkRepo>().SingleInstance();
        builder.RegisterType<TileRepo>().As<ITileRepo>().SingleInstance();
        builder.RegisterType<FeatureRepo>().As<IFeatureRepo>().SingleInstance();
        builder.RegisterType<FaceRepo>().As<IFaceRepo>().SingleInstance();
        builder.RegisterType<PointRepo>().As<IPointRepo>().SingleInstance();
        builder.RegisterType<UnitRepo>().As<IUnitRepo>().SingleInstance();
        builder.RegisterType<CivRepo>().As<ICivRepo>().SingleInstance();
        // 读库
        // 缓存
        builder.RegisterType<LodMeshCache>().As<ILodMeshCache>().SingleInstance();
        // 节点存储
        builder.RegisterType<NodeRegister>().SingleInstance();
        builder.RegisterType<ChunkLoaderRepo>().As<IChunkLoaderRepo>().SingleInstance();
        builder.RegisterType<FeatureMeshManagerRepo>().As<IFeatureMeshManagerRepo>().SingleInstance();
        builder.RegisterType<FeaturePreviewManagerRepo>().As<IFeaturePreviewManagerRepo>().SingleInstance();
        builder.RegisterType<ErosionLandGeneratorRepo>().As<IErosionLandGeneratorRepo>().SingleInstance();
        builder.RegisterType<FractalNoiseLandGeneratorRepo>().As<IFractalNoiseLandGeneratorRepo>().SingleInstance();
        builder.RegisterType<RealEarthLandGeneratorRepo>().As<IRealEarthLandGeneratorRepo>().SingleInstance();
        builder.RegisterType<CelestialMotionManagerRepo>().As<ICelestialMotionManagerRepo>().SingleInstance();
        builder.RegisterType<SelectTileViewerRepo>().As<ISelectTileViewerRepo>().SingleInstance();
        builder.RegisterType<UnitManagerRepo>().As<IUnitManagerRepo>().SingleInstance();
        builder.RegisterType<ChunkManagerRepo>().As<IChunkManagerRepo>().SingleInstance();
        builder.RegisterType<EditPreviewChunkRepo>().As<IEditPreviewChunkRepo>().SingleInstance();
        builder.RegisterType<HexMapGeneratorRepo>().As<IHexMapGeneratorRepo>().SingleInstance();
        builder.RegisterType<HexPlanetHudRepo>().As<IHexPlanetHudRepo>().SingleInstance();
        builder.RegisterType<HexPlanetManagerRepo>().As<IHexPlanetManagerRepo>().SingleInstance();
        builder.RegisterType<LongitudeLatitudeRepo>().As<ILongitudeLatitudeRepo>().SingleInstance();
        builder.RegisterType<MiniMapManagerRepo>().As<IMiniMapManagerRepo>().SingleInstance();
        builder.RegisterType<OrbitCameraRepo>().As<IOrbitCameraRepo>().SingleInstance();
        // ===== 领域层 =====
        // 领域服务
        builder.RegisterType<PointService>().As<IPointService>().SingleInstance();
        builder.RegisterType<ChunkService>().As<IChunkService>().SingleInstance();
        builder.RegisterType<TileService>().As<ITileService>().SingleInstance();
        builder.RegisterType<TileSearchService>().As<ITileSearchService>().SingleInstance();
        builder.RegisterType<TileShaderService>().As<ITileShaderService>().SingleInstance();
        builder.RegisterType<MiniMapService>().As<IMiniMapService>().SingleInstance();
        builder.RegisterType<SelectViewService>().As<ISelectViewService>().SingleInstance();
        // 节点服务
        builder.RegisterType<ChunkLoaderService>().As<IChunkLoaderService>().SingleInstance();
        builder.RegisterType<FeatureMeshManagerService>().As<IFeatureMeshManagerService>().SingleInstance();
        builder.RegisterType<FeaturePreviewManagerService>().As<IFeaturePreviewManagerService>().SingleInstance();
        builder.RegisterType<ErosionLandGeneratorService>().As<IErosionLandGeneratorService>().SingleInstance();
        builder.RegisterType<FractalNoiseLandGeneratorService>().As<IFractalNoiseLandGeneratorService>()
            .SingleInstance();
        builder.RegisterType<RealEarthLandGeneratorService>().As<IRealEarthLandGeneratorService>().SingleInstance();
        builder.RegisterType<CelestialMotionManagerService>().As<ICelestialMotionManagerService>().SingleInstance();
        builder.RegisterType<SelectTileViewerService>().As<ISelectTileViewerService>().SingleInstance();
        builder.RegisterType<ChunkManagerService>().As<IChunkManagerService>().SingleInstance();
        builder.RegisterType<EditPreviewChunkService>().As<IEditPreviewChunkService>().SingleInstance();
        builder.RegisterType<HexMapGeneratorService>().As<IHexMapGeneratorService>().SingleInstance();
        builder.RegisterType<HexPlanetHudService>().As<IHexPlanetHudService>().SingleInstance();
        builder.RegisterType<HexPlanetManagerService>().As<IHexPlanetManagerService>().SingleInstance();
        builder.RegisterType<LongitudeLatitudeService>().As<ILongitudeLatitudeService>().SingleInstance();
        builder.RegisterType<MiniMapManagerService>().As<IMiniMapManagerService>().SingleInstance();
        builder.RegisterType<OrbitCameraService>().As<IOrbitCameraService>().SingleInstance();
        // ===== 应用层 =====
        // 查询
        builder.RegisterType<FeatureApplication>().As<IFeatureApplication>().SingleInstance();
        builder.RegisterType<TileShaderApplication>().As<ITileShaderApplication>().SingleInstance();
        builder.RegisterType<HexPlanetHudApp>().As<IHexPlanetHudApp>().SingleInstance();
        builder.RegisterType<HexPlanetManagerApp>().As<IHexPlanetManagerApp>().SingleInstance();
        builder.RegisterType<MiniMapManagerApp>().As<IMiniMapManagerApp>().SingleInstance();
        // 命令
        builder.RegisterType<ChunkLoaderCommander>().SingleInstance();
        builder.RegisterType<FeatureMeshManagerCommander>().SingleInstance();
        builder.RegisterType<FeaturePreviewManagerCommander>().SingleInstance();
        builder.RegisterType<ErosionLandGeneratorCommander>().SingleInstance();
        builder.RegisterType<FractalNoiseLandGeneratorCommander>().SingleInstance();
        builder.RegisterType<RealEarthLandGeneratorCommander>().SingleInstance();
        builder.RegisterType<CelestialMotionManagerCommander>().SingleInstance();
        builder.RegisterType<SelectTileViewerCommander>().SingleInstance();
        builder.RegisterType<ChunkManagerCommander>().SingleInstance();
        builder.RegisterType<EditPreviewChunkCommander>().SingleInstance();
        builder.RegisterType<HexMapGeneratorCommander>().SingleInstance();
        builder.RegisterType<HexPlanetHudCommander>().SingleInstance();
        builder.RegisterType<HexPlanetManagerCommander>().SingleInstance();
        builder.RegisterType<LongitudeLatitudeCommander>().SingleInstance();
        builder.RegisterType<MiniMapManagerCommander>().SingleInstance();
        builder.RegisterType<OrbitCameraCommander>().SingleInstance();
        _container = builder.Build();
        _nodeRegister = _container.Resolve<NodeRegister>();
        // 这种构造函数有初始化逻辑的，必须先 Resolve()，否则构造函数并没有被调用
        _container.Resolve<ChunkLoaderCommander>();
        _container.Resolve<FeatureMeshManagerCommander>();
        _container.Resolve<FeaturePreviewManagerCommander>();
        _container.Resolve<ErosionLandGeneratorCommander>();
        _container.Resolve<FractalNoiseLandGeneratorCommander>();
        _container.Resolve<RealEarthLandGeneratorCommander>();

        var tileShaderService = _container.Resolve<ITileShaderService>();
#if FEATURE_NEW
        var featureApplication = _container.Resolve<IFeatureApplication>();
        tileShaderService.TileExplored += featureApplication.ExploreFeatures;
#endif
        var tileShaderApplication = _container.Resolve<ITileShaderApplication>();
        tileShaderService.RangeVisibilityIncreased += tileShaderApplication.IncreaseVisibility;
    }
}