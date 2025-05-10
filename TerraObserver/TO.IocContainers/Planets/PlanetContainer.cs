using Autofac;
using TO.Apps.Commands.Abstractions.Planets;
using TO.Apps.Commands.Planets;
using TO.Domains.Services.Abstractions.Planets;
using TO.Domains.Services.Planets;
using TO.Nodes.Abstractions.Planets.Models;
using TO.Nodes.Abstractions.Planets.Views;

namespace TO.IocContainers.Planets;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-05-09 21:57:33
public class PlanetContainer : IDisposable
{
    private IContainer? _container;

    // 代码入口
    private IPlanetCommander? _planetCommander;
    public IPlanetCommander PlanetCommander => _planetCommander!;

    public PlanetContainer(IPlanet planet, IHexSphereConfigs hexSphereConfigs)
    {
        // 测试过，RegisterType 的顺序不影响注入结果（就是说不要求被依赖的放在前面），毕竟只是 Builder 的顺序
        // 默认是瞬态 Instance，单例需要加 .SingleInstance()
        // 单例在根生存周期域内，释放不了，所以可以创建生存周期域 .InstancePerMatchingLifetimeScope("xxx")
        // .BeginLoadContextLifetimeScope() 可在卸载程序集时释放所有 Autofac 管理的对象。
        var builder = new ContainerBuilder();
        // ===== 显示层 =====
        builder.RegisterInstance(planet).ExternallyOwned();
        builder.RegisterInstance(hexSphereConfigs).ExternallyOwned();
        // ===== 领域层 =====
        // 领域服务
        builder.RegisterType<CatlikeCodingNoiseService>().As<ICatlikeCodingNoiseService>().SingleInstance();
        // ===== 应用层 =====
        // 节点命令
        builder.RegisterType<PlanetCommander>().As<IPlanetCommander>().SingleInstance();
        _container = builder.Build();

        _planetCommander = _container.Resolve<IPlanetCommander>();
    }

    public void Dispose()
    {
        _planetCommander = null;
        _container?.Dispose();
        _container = null;
    }
}