using TO.GodotNodes.Abstractions;
using TO.Infras.Readers.Abstractions.Nodes.Singletons;
using TO.Nodes.Abstractions;

namespace TO.Infras.Readers.Abstractions.Nodes;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-28 16:57:50
public class NodeRegister(
    // 单例
    IPlanetRepo planetRepo)
{
    public bool Register<T>(T node) where T : INode
    {
        return node switch
        {
            // 单例
            IPlanet planet => planetRepo.Register(planet),
            _ => throw new ArgumentException($"暂不支持的单例节点：{typeof(T).Name}")
        };
    }

    private static bool RegisterIdInstance<T>(T instance, Action<T> register) where T : INode
    {
        register.Invoke(instance);
        return false; // 多例永远不会发生覆盖
    }
}