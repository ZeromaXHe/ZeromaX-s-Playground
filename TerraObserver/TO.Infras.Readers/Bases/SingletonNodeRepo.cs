using Godot;
using TO.GodotNodes.Abstractions;
using TO.Infras.Readers.Abstractions.Bases;

namespace TO.Infras.Readers.Bases;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-28 21:51:15
public class SingletonNodeRepo<T> : ISingletonNodeRepo<T> where T : INode
{
    public event Action? Ready;
    private void EmitReady() => Ready?.Invoke();
    public event Action? TreeExiting;
    private void EmitTreeExiting() => TreeExiting?.Invoke();

    public T? Singleton { get; private set; }

    public bool Register(T singleton)
    {
        var result = Singleton is not null;
        Singleton = singleton;
        Singleton.TreeExiting += Unregister;

        Singleton.Ready += EmitReady;
        Singleton.TreeExiting += EmitTreeExiting;
        ConnectNodeEvents();
        return result;
    }

    public void Unregister()
    {
        if (Singleton == null)
        {
            GD.PrintErr("很奇怪，单例节点取消注册时已经为空了！");
            return;
        }

        Singleton.TreeExiting -= Unregister;

        Singleton.Ready -= EmitReady;
        Singleton.TreeExiting -= EmitTreeExiting;
        DisconnectNodeEvents();
        Singleton = default;
    }

    protected virtual void ConnectNodeEvents()
    {
    }

    protected virtual void DisconnectNodeEvents()
    {
    }

    public bool IsRegistered() => Singleton != null;
}