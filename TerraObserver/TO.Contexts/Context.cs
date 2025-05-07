using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Loader;
using Autofac;
using Godot;
using TO.Apps.Commands.Nodes.Singletons;
using TO.Domains.Services.Abstractions.Nodes.Singletons;
using TO.Domains.Services.Nodes.Singletons;
using TO.GodotNodes.Abstractions;
using TO.Infras.Readers.Abstractions.Nodes;
using TO.Infras.Readers.Abstractions.Nodes.Singletons;
using TO.Infras.Readers.Nodes.Singletons;

namespace TO.Contexts;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-28 16:48:45
public class Context
{
    private IContainer? _container;
    private const string BuildScopeName = "build";
    private ILifetimeScope? _buildLifetimeScope;
    private NodeRegister? _nodeRegister;

    private static Context? Singleton { get; set; } = new();

    public static bool RegisterToHolder<T>(T singleton) where T : INode => Singleton!.RegisterNode(singleton);

    private bool RegisterNode<T>(T singleton) where T : INode
    {
        if (_nodeRegister == null) Init();
        return _nodeRegister.Register(singleton);
    }

    [MemberNotNull(nameof(_nodeRegister), nameof(_container), nameof(_buildLifetimeScope))]
    private void Init()
    {
        // 测试过，RegisterType 的顺序不影响注入结果（就是说不要求被依赖的放在前面），毕竟只是 Builder 的顺序
        var builder = new ContainerBuilder();
        // 默认是瞬态 Instance，单例需要加 .SingleInstance()
        // 单例在根生存周期域内，释放不了，所以要创建一个新的生存周期域 build，在卸载程序集时释放所有 Autofac 管理的对象。
        // TODO: 替换为扫描程序集注入？总之不是这样手写，不然容易漏……（一旦漏了，并不会报错，只是拿不到依赖）
        // ===== 基础设施层 =====
        // ==写库==
        // ==读库==
        // 节点存储
        builder.RegisterType<NodeRegister>().InstancePerMatchingLifetimeScope(BuildScopeName);
        // 单例存储
        builder.RegisterType<PlanetRepo>().As<IPlanetRepo>().InstancePerMatchingLifetimeScope(BuildScopeName);
        // 多例存储
        // ===== 领域层 =====
        // 领域服务
        // 单例节点服务
        builder.RegisterType<PlanetService>().As<IPlanetService>().InstancePerMatchingLifetimeScope(BuildScopeName);
        // 多例节点服务
        // ===== 应用层 =====
        // 单例节点命令
        builder.RegisterType<PlanetCommand>().InstancePerMatchingLifetimeScope(BuildScopeName);
        // 多例节点命令
        _container = builder.Build();
        _buildLifetimeScope = _container.BeginLifetimeScope(BuildScopeName);
        _nodeRegister = _buildLifetimeScope.Resolve<NodeRegister>();
        // 这种构造函数有初始化逻辑的，必须先 Resolve()，否则构造函数并没有被调用
        // 单例
        _buildLifetimeScope.Resolve<PlanetCommand>();
    }

    public static void UnloadThis(AssemblyLoadContext context)
    {
        GD.Print($"static Unloading");
        // context.Unloading -= UnloadThis;
        Singleton?.Unload(context);
    }

    private void Unload(AssemblyLoadContext context)
    {
        GD.Print($"AssemblyLoadContext {context} Unloading");
        _nodeRegister = null;
        _buildLifetimeScope?.Dispose();
        _buildLifetimeScope = null;
        _container?.Dispose();
        _container = null;
        Singleton = null;
        GD.Print($"AssemblyLoadContext {context} Unloaded");
    }
}