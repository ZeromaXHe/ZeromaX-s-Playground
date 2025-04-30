using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Loader;
using Apps.Commands.Nodes.IdInstances;
using Apps.Commands.Nodes.Singletons;
using Apps.Commands.Nodes.Singletons.ChunkManagers;
using Apps.Commands.Nodes.Singletons.LandGenerators;
using Apps.Commands.Nodes.Singletons.Planets;
using Autofac;
using Contexts.Abstractions;
using Domains.Services.Abstractions.Nodes.IdInstances;
using Domains.Services.Abstractions.Nodes.Singletons;
using Domains.Services.Abstractions.Nodes.Singletons.ChunkManagers;
using Domains.Services.Abstractions.Nodes.Singletons.LandGenerators;
using Domains.Services.Abstractions.Nodes.Singletons.Planets;
using Domains.Services.Abstractions.PlanetGenerates;
using Domains.Services.Abstractions.Searches;
using Domains.Services.Abstractions.Shaders;
using Domains.Services.Abstractions.Uis;
using Domains.Services.Nodes.IdInstances;
using Domains.Services.Nodes.Singletons;
using Domains.Services.Nodes.Singletons.ChunkManagers;
using Domains.Services.Nodes.Singletons.LandGenerators;
using Domains.Services.Nodes.Singletons.Planets;
using Domains.Services.PlanetGenerates;
using Domains.Services.Searches;
using Domains.Services.Shaders;
using Domains.Services.Uis;
using Godot;
using GodotNodes.Abstractions;
using Infras.Readers.Abstractions.Caches;
using Infras.Readers.Abstractions.Nodes;
using Infras.Readers.Abstractions.Nodes.IdInstances;
using Infras.Readers.Abstractions.Nodes.Singletons;
using Infras.Readers.Abstractions.Nodes.Singletons.ChunkManagers;
using Infras.Readers.Abstractions.Nodes.Singletons.LandGenerators;
using Infras.Readers.Abstractions.Nodes.Singletons.Planets;
using Infras.Readers.Caches;
using Infras.Readers.Nodes.IdInstances;
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

    public static bool RegisterToHolder<T>(T singleton) where T : INode
    {
        ContextHolder.BeanContext ??= new Context();
        return ContextHolder.BeanContext.RegisterNode(singleton);
    }

    public T GetBean<T>() where T : class
    {
        // 现在 4.4 的生命周期有点看不懂了，运行游戏时居然先调用 HexGridChunk 的构造函数而不是 HexPlanetManager 的？！
        // 所以只能在这里初始化，否则直接 GetBean null 容易把编辑器和游戏运行搞崩。
        if (_buildLifetimeScope == null) Init();
        return _buildLifetimeScope.Resolve<T>();
    }

    private IContainer? _container;
    private ILifetimeScope? _buildLifetimeScope;
    private NodeRegister? _nodeRegister;

    public bool RegisterNode<T>(T singleton) where T : INode
    {
        if (_nodeRegister == null) Init();
        return _nodeRegister.Register(singleton);
    }

    public static void InitBeanContext()
    {
        ContextHolder.BeanContext ??= new Context();
        ContextHolder.BeanContext.Init();
    }
    
    [MemberNotNull(nameof(_nodeRegister), nameof(_container), nameof(_buildLifetimeScope))]
    public void Init()
    {
        // 测试过，RegisterType 的顺序不影响注入结果（就是说不要求被依赖的放在前面），毕竟只是 Builder 的顺序
        var builder = new ContainerBuilder();
        // 默认是瞬态 Instance，单例需要加 .SingleInstance()
        // 单例在根生存周期域内，释放不了，所以要创建一个新的生存周期域 build，在卸载程序集时释放所有 Autofac 管理的对象。
        // TODO: 替换为扫描程序集注入？总之不是这样手写，不然容易漏……（一旦漏了，并不会报错，只是拿不到依赖）
        // ===== 基础设施层 =====
        // 写库
        builder.RegisterType<ChunkRepo>().As<IChunkRepo>().InstancePerMatchingLifetimeScope("build");
        builder.RegisterType<TileRepo>().As<ITileRepo>().InstancePerMatchingLifetimeScope("build");
        builder.RegisterType<FeatureRepo>().As<IFeatureRepo>().InstancePerMatchingLifetimeScope("build");
        builder.RegisterType<FaceRepo>().As<IFaceRepo>().InstancePerMatchingLifetimeScope("build");
        builder.RegisterType<PointRepo>().As<IPointRepo>().InstancePerMatchingLifetimeScope("build");
        builder.RegisterType<UnitRepo>().As<IUnitRepo>().InstancePerMatchingLifetimeScope("build");
        builder.RegisterType<CivRepo>().As<ICivRepo>().InstancePerMatchingLifetimeScope("build");
        // 读库
        // 缓存
        builder.RegisterType<LodMeshCache>().As<ILodMeshCache>().InstancePerMatchingLifetimeScope("build");
        // 节点存储
        builder.RegisterType<NodeRegister>().InstancePerMatchingLifetimeScope("build");
        // 单例存储
        builder.RegisterType<ChunkLoaderRepo>().As<IChunkLoaderRepo>().InstancePerMatchingLifetimeScope("build")
            .OnRelease(repo => repo.Unregister());
        builder.RegisterType<FeatureMeshManagerRepo>().As<IFeatureMeshManagerRepo>()
            .InstancePerMatchingLifetimeScope("build")
            .OnRelease(repo => repo.Unregister());
        builder.RegisterType<FeaturePreviewManagerRepo>().As<IFeaturePreviewManagerRepo>()
            .InstancePerMatchingLifetimeScope("build")
            .OnRelease(repo => repo.Unregister());
        builder.RegisterType<ErosionLandGeneratorRepo>().As<IErosionLandGeneratorRepo>()
            .InstancePerMatchingLifetimeScope("build")
            .OnRelease(repo => repo.Unregister());
        builder.RegisterType<FractalNoiseLandGeneratorRepo>().As<IFractalNoiseLandGeneratorRepo>()
            .InstancePerMatchingLifetimeScope("build")
            .OnRelease(repo => repo.Unregister());
        builder.RegisterType<RealEarthLandGeneratorRepo>().As<IRealEarthLandGeneratorRepo>()
            .InstancePerMatchingLifetimeScope("build")
            .OnRelease(repo => repo.Unregister());
        builder.RegisterType<CelestialMotionManagerRepo>().As<ICelestialMotionManagerRepo>()
            .InstancePerMatchingLifetimeScope("build")
            .OnRelease(repo => repo.Unregister());
        builder.RegisterType<SelectTileViewerRepo>().As<ISelectTileViewerRepo>()
            .InstancePerMatchingLifetimeScope("build")
            .OnRelease(repo => repo.Unregister());
        builder.RegisterType<UnitManagerRepo>().As<IUnitManagerRepo>().InstancePerMatchingLifetimeScope("build")
            .OnRelease(repo => repo.Unregister());
        builder.RegisterType<ChunkManagerRepo>().As<IChunkManagerRepo>().InstancePerMatchingLifetimeScope("build")
            .OnRelease(repo => repo.Unregister());
        builder.RegisterType<EditPreviewChunkRepo>().As<IEditPreviewChunkRepo>()
            .InstancePerMatchingLifetimeScope("build")
            .OnRelease(repo => repo.Unregister());
        builder.RegisterType<HexMapGeneratorRepo>().As<IHexMapGeneratorRepo>().InstancePerMatchingLifetimeScope("build")
            .OnRelease(repo => repo.Unregister());
        builder.RegisterType<HexPlanetHudRepo>().As<IHexPlanetHudRepo>().InstancePerMatchingLifetimeScope("build")
            .OnRelease(repo => repo.Unregister());
        builder.RegisterType<HexPlanetManagerRepo>().As<IHexPlanetManagerRepo>()
            .InstancePerMatchingLifetimeScope("build")
            .OnRelease(repo => repo.Unregister());
        builder.RegisterType<LongitudeLatitudeRepo>().As<ILongitudeLatitudeRepo>()
            .InstancePerMatchingLifetimeScope("build")
            .OnRelease(repo => repo.Unregister());
        builder.RegisterType<MiniMapManagerRepo>().As<IMiniMapManagerRepo>().InstancePerMatchingLifetimeScope("build")
            .OnRelease(repo => repo.Unregister());
        builder.RegisterType<OrbitCameraRepo>().As<IOrbitCameraRepo>().InstancePerMatchingLifetimeScope("build")
            .OnRelease(repo => repo.Unregister());
        // 多例存储
        builder.RegisterType<HexGridChunkRepo>().As<IHexGridChunkRepo>().InstancePerMatchingLifetimeScope("build")
            .OnRelease(repo => repo.UnregisterAll());
        builder.RegisterType<HexUnitRepo>().As<IHexUnitRepo>().InstancePerMatchingLifetimeScope("build")
            .OnRelease(repo => repo.UnregisterAll());
        // ===== 领域层 =====
        // 领域服务
        builder.RegisterType<PointService>().As<IPointService>().InstancePerMatchingLifetimeScope("build");
        builder.RegisterType<ChunkService>().As<IChunkService>().InstancePerMatchingLifetimeScope("build");
        builder.RegisterType<TileService>().As<ITileService>().InstancePerMatchingLifetimeScope("build");
        builder.RegisterType<TileSearchService>().As<ITileSearchService>().InstancePerMatchingLifetimeScope("build");
        builder.RegisterType<TileShaderService>().As<ITileShaderService>().InstancePerMatchingLifetimeScope("build")
            .OnRelease(service => service.ReleaseEvents());
        builder.RegisterType<MiniMapService>().As<IMiniMapService>().InstancePerMatchingLifetimeScope("build");
        // 单例节点服务
        builder.RegisterType<ChunkLoaderService>().As<IChunkLoaderService>().InstancePerMatchingLifetimeScope("build");
        builder.RegisterType<ChunkTriangulationService>().As<IChunkTriangulationService>()
            .InstancePerMatchingLifetimeScope("build");
        builder.RegisterType<FeatureMeshManagerService>().As<IFeatureMeshManagerService>()
            .InstancePerMatchingLifetimeScope("build");
        builder.RegisterType<FeaturePreviewManagerService>().As<IFeaturePreviewManagerService>()
            .InstancePerMatchingLifetimeScope("build");
        builder.RegisterType<ErosionLandGeneratorService>().As<IErosionLandGeneratorService>()
            .InstancePerMatchingLifetimeScope("build");
        builder.RegisterType<FractalNoiseLandGeneratorService>().As<IFractalNoiseLandGeneratorService>()
            .InstancePerMatchingLifetimeScope("build");
        builder.RegisterType<RealEarthLandGeneratorService>().As<IRealEarthLandGeneratorService>()
            .InstancePerMatchingLifetimeScope("build");
        builder.RegisterType<CelestialMotionManagerService>().As<ICelestialMotionManagerService>()
            .InstancePerMatchingLifetimeScope("build");
        builder.RegisterType<SelectTileViewerService>().As<ISelectTileViewerService>()
            .InstancePerMatchingLifetimeScope("build");
        builder.RegisterType<UnitManagerService>().As<IUnitManagerService>().InstancePerMatchingLifetimeScope("build");
        builder.RegisterType<ChunkManagerService>().As<IChunkManagerService>()
            .InstancePerMatchingLifetimeScope("build");
        builder.RegisterType<EditPreviewChunkService>().As<IEditPreviewChunkService>()
            .InstancePerMatchingLifetimeScope("build");
        builder.RegisterType<HexMapGeneratorService>().As<IHexMapGeneratorService>()
            .InstancePerMatchingLifetimeScope("build");
        builder.RegisterType<HexPlanetHudService>().As<IHexPlanetHudService>()
            .InstancePerMatchingLifetimeScope("build");
        builder.RegisterType<HexPlanetManagerService>().As<IHexPlanetManagerService>()
            .InstancePerMatchingLifetimeScope("build");
        builder.RegisterType<LongitudeLatitudeService>().As<ILongitudeLatitudeService>()
            .InstancePerMatchingLifetimeScope("build");
        builder.RegisterType<MiniMapManagerService>().As<IMiniMapManagerService>()
            .InstancePerMatchingLifetimeScope("build");
        builder.RegisterType<OrbitCameraService>().As<IOrbitCameraService>().InstancePerMatchingLifetimeScope("build");
        // 多例节点服务
        builder.RegisterType<HexGridChunkService>().As<IHexGridChunkService>()
            .InstancePerMatchingLifetimeScope("build");
        builder.RegisterType<HexUnitService>().As<IHexUnitService>().InstancePerMatchingLifetimeScope("build");
        // ===== 应用层 =====
        // 单例节点命令
        builder.RegisterType<ChunkLoaderCommander>().InstancePerMatchingLifetimeScope("build")
            .OnRelease(cmd => cmd.ReleaseEvents());
        builder.RegisterType<FeatureMeshManagerCommander>().InstancePerMatchingLifetimeScope("build")
            .OnRelease(cmd => cmd.ReleaseEvents());
        builder.RegisterType<FeaturePreviewManagerCommander>().InstancePerMatchingLifetimeScope("build")
            .OnRelease(cmd => cmd.ReleaseEvents());
        builder.RegisterType<ErosionLandGeneratorCommander>().InstancePerMatchingLifetimeScope("build")
            .OnRelease(cmd => cmd.ReleaseEvents());
        builder.RegisterType<FractalNoiseLandGeneratorCommander>().InstancePerMatchingLifetimeScope("build")
            .OnRelease(cmd => cmd.ReleaseEvents());
        builder.RegisterType<RealEarthLandGeneratorCommander>().InstancePerMatchingLifetimeScope("build")
            .OnRelease(cmd => cmd.ReleaseEvents());
        builder.RegisterType<CelestialMotionManagerCommander>().InstancePerMatchingLifetimeScope("build")
            .OnRelease(cmd => cmd.ReleaseEvents());
        builder.RegisterType<SelectTileViewerCommander>().InstancePerMatchingLifetimeScope("build")
            .OnRelease(cmd => cmd.ReleaseEvents());
        builder.RegisterType<UnitManagerCommander>().InstancePerMatchingLifetimeScope("build")
            .OnRelease(cmd => cmd.ReleaseEvents());
        builder.RegisterType<ChunkManagerCommander>().InstancePerMatchingLifetimeScope("build");
        builder.RegisterType<EditPreviewChunkCommander>().InstancePerMatchingLifetimeScope("build")
            .OnRelease(cmd => cmd.ReleaseEvents());
        builder.RegisterType<HexMapGeneratorCommander>().InstancePerMatchingLifetimeScope("build");
        builder.RegisterType<HexPlanetHudCommander>().InstancePerMatchingLifetimeScope("build")
            .OnRelease(cmd => cmd.ReleaseEvents());
        builder.RegisterType<HexPlanetManagerCommander>().InstancePerMatchingLifetimeScope("build")
            .OnRelease(cmd => cmd.ReleaseEvents());
        builder.RegisterType<LongitudeLatitudeCommander>().InstancePerMatchingLifetimeScope("build")
            .OnRelease(cmd => cmd.ReleaseEvents());
        builder.RegisterType<MiniMapManagerCommander>().InstancePerMatchingLifetimeScope("build")
            .OnRelease(cmd => cmd.ReleaseEvents());
        builder.RegisterType<OrbitCameraCommander>().InstancePerMatchingLifetimeScope("build")
            .OnRelease(cmd => cmd.ReleaseEvents());
        // 多例节点命令
        builder.RegisterType<HexGridChunkCommander>().InstancePerMatchingLifetimeScope("build")
            .OnRelease(cmd => cmd.ReleaseEvents());
        builder.RegisterType<HexUnitCommander>().InstancePerMatchingLifetimeScope("build")
            .OnRelease(cmd => cmd.ReleaseEvents());
        _container = builder.Build();
        _buildLifetimeScope = _container.BeginLifetimeScope("build");
        _nodeRegister = _buildLifetimeScope.Resolve<NodeRegister>();
        // 这种构造函数有初始化逻辑的，必须先 Resolve()，否则构造函数并没有被调用
        // 单例
        _buildLifetimeScope.Resolve<ChunkLoaderCommander>();
        _buildLifetimeScope.Resolve<FeatureMeshManagerCommander>();
        _buildLifetimeScope.Resolve<FeaturePreviewManagerCommander>();
        _buildLifetimeScope.Resolve<ErosionLandGeneratorCommander>();
        _buildLifetimeScope.Resolve<FractalNoiseLandGeneratorCommander>();
        _buildLifetimeScope.Resolve<RealEarthLandGeneratorCommander>();
        _buildLifetimeScope.Resolve<EditPreviewChunkCommander>();
        _buildLifetimeScope.Resolve<CelestialMotionManagerCommander>();
        _buildLifetimeScope.Resolve<SelectTileViewerCommander>();
        _buildLifetimeScope.Resolve<UnitManagerCommander>();
        _buildLifetimeScope.Resolve<HexPlanetHudCommander>();
        _buildLifetimeScope.Resolve<HexPlanetManagerCommander>();
        _buildLifetimeScope.Resolve<LongitudeLatitudeCommander>();
        _buildLifetimeScope.Resolve<MiniMapManagerCommander>();
        _buildLifetimeScope.Resolve<OrbitCameraCommander>();
        // 多例
        _buildLifetimeScope.Resolve<HexGridChunkCommander>();
        _buildLifetimeScope.Resolve<HexUnitCommander>();
    }

    public static void UnloadBeanContext() => ContextHolder.BeanContext?.Unload();

    public void Unload()
    {
        var context = AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly())!;
        GD.Print($"AssemblyLoadContext {context} Unloading");
        _buildLifetimeScope?.Dispose();
        _buildLifetimeScope = null;
        _container?.Dispose();
        _container = null;
        _nodeRegister = null;
        ContextHolder.BeanContext = null;
    }
}