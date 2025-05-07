using TO.GodotNodes.Abstractions;

namespace TO.Infras.Readers.Abstractions.Bases;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-28 21:48:28
public interface ISingletonNodeRepo<T> where T : INode
{
    event Action? Ready;
    event Action? TreeExiting;
    T? Singleton { get; }

    // 返回 bool 对应是否覆盖了之前存在的单例节点
    bool Register(T singleton);
    bool IsRegistered();
}