using Godot;
using GodotNodes.Abstractions;

namespace Infras.Readers.Abstractions.Bases;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-22 11:02:22
public interface IIdInstanceNodeRepo<T> where T : INode
{
    event Action<T>? Ready;
    event Action<T>? TreeExiting;
    event Action<T, double>? Processed;
    event Action<T, double>? PhysicsProcessed;
    event Action<T, InputEvent>? Inputted;
    HashSet<T> Instances { get; }
    void Register(T instance);
}