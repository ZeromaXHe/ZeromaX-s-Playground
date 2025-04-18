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
using Domains.Models.Singletons.Planets;
using Domains.Models.Singletons.Planets.Impl;
using Domains.Services.Abstractions.Events;
using Domains.Services.Abstractions.PlanetGenerates;
using Domains.Services.Abstractions.Searches;
using Domains.Services.Abstractions.Shaders;
using Domains.Services.Abstractions.Uis;
using Domains.Services.PlanetGenerates;
using Domains.Services.Searches;
using Domains.Services.Shaders;
using Domains.Services.Uis;
using Infras.Readers.Abstractions.Caches;
using Infras.Readers.Caches;
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

    public T GetBean<T>() where T : class
    {
        // 现在 4.4 的生命周期有点看不懂了，运行游戏时居然先调用 HexGridChunk 的构造函数而不是 HexPlanetManager 的？！
        // 所以只能在这里初始化，否则直接 GetBean null 容易把编辑器和游戏运行搞崩。
        if (_container == null) Init();
        return _container!.Resolve<T>();
    }

    private IContainer? _container;

    private void Init()
    {
        // 测试过，RegisterType 的顺序不影响注入结果（就是说不要求被依赖的放在前面），毕竟只是 Builder 的顺序
        var builder = new ContainerBuilder();
        // 单例
        // 默认是瞬态 Instance，需要加 .SingleInstance()
        builder.RegisterType<PlanetConfig>().As<IPlanetConfig>().SingleInstance();
        builder.RegisterType<NoiseConfig>().As<INoiseConfig>().SingleInstance();
        builder.RegisterType<LodMeshCache>().As<ILodMeshCache>().SingleInstance();
        // 存储
        builder.RegisterType<ChunkRepo>().As<IChunkRepo>().SingleInstance();
        builder.RegisterType<TileRepo>().As<ITileRepo>().SingleInstance();
        builder.RegisterType<FeatureRepo>().As<IFeatureRepo>().SingleInstance();
        builder.RegisterType<FaceRepo>().As<IFaceRepo>().SingleInstance();
        builder.RegisterType<PointRepo>().As<IPointRepo>().SingleInstance();
        builder.RegisterType<UnitRepo>().As<IUnitRepo>().SingleInstance();
        builder.RegisterType<CivRepo>().As<ICivRepo>().SingleInstance();
        // 服务
        builder.RegisterType<PointService>().As<IPointService>().SingleInstance();
        builder.RegisterType<ChunkService>().As<IChunkService>().SingleInstance();
        builder.RegisterType<TileService>().As<ITileService>().SingleInstance();
        builder.RegisterType<TileSearchService>().As<ITileSearchService>().SingleInstance();
        builder.RegisterType<TileShaderService>().As<ITileShaderService>().SingleInstance();
        builder.RegisterType<EditorService>().As<IEditorService>().SingleInstance();
        builder.RegisterType<MiniMapService>().As<IMiniMapService>().SingleInstance();
        builder.RegisterType<SelectViewService>().As<ISelectViewService>().SingleInstance();
        // 应用
        builder.RegisterType<FeatureApplication>().As<IFeatureApplication>().SingleInstance();
        builder.RegisterType<TileShaderApplication>().As<ITileShaderApplication>().SingleInstance();
        builder.RegisterType<HexPlanetHudApp>().As<IHexPlanetHudApp>().SingleInstance();
        builder.RegisterType<HexPlanetManagerApp>().As<IHexPlanetManagerApp>().SingleInstance();
        builder.RegisterType<MiniMapManagerApp>().As<IMiniMapManagerApp>().SingleInstance();
        _container = builder.Build();
        var featureApplication = _container.Resolve<IFeatureApplication>();
        TileShaderEvent.Instance.TileExplored += featureApplication.ExploreFeatures;
        var tileShaderApplication = _container.Resolve<ITileShaderApplication>();
        TileShaderEvent.Instance.RangeVisibilityIncreased += tileShaderApplication.IncreaseVisibility;
    }
}