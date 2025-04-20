using Godot;
using GodotNodes.Abstractions;
using GodotNodes.Abstractions.Addition;

namespace Infras.Readers.Abstractions.Bases;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 10:28:18
public interface ISingletonNodeRepo<T> where T : INode
{
    event Action? Ready;
    event Action? TreeExiting;
    event Action<double>? Processed;
    event Action<double>? PhysicsProcessed;
    event Action<InputEvent>? Inputted;
    T? Singleton { get; }

    // 返回 bool 对应是否覆盖了之前存在的单例节点
    bool Register(T singleton);
    bool IsRegistered();
}