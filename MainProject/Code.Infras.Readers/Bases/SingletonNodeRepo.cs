using Godot;
using GodotNodes.Abstractions;
using Infras.Readers.Abstractions.Bases;

namespace Infras.Readers.Bases;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 10:39:54
public abstract class SingletonNodeRepo<T> : ISingletonNodeRepo<T> where T : INode
{
    public event Action? Ready;
    public event Action? TreeExiting;
    public event Action<double>? Processed;
    public event Action<double>? PhysicsProcessed;
    public event Action<InputEvent>? Inputted;
    private void EmitReady() => Ready?.Invoke();
    private void EmitTreeExiting() => TreeExiting?.Invoke();
    private void EmitProcessed(double delta) => Processed?.Invoke(delta);
    private void EmitPhysicsProcessed(double delta) => PhysicsProcessed?.Invoke(delta);
    private void EmitInputted(InputEvent inputEvent) => Inputted?.Invoke(inputEvent);
    public T? Singleton { get; private set; }

    public bool Register(T singleton)
    {
        var result = Singleton is not null;
        Singleton = singleton;
        Singleton.TreeExiting += Unregister;

        Singleton.Ready += EmitReady;
        Singleton.TreeExiting += EmitTreeExiting;
        Singleton.NodeEvent?.Connect(EmitProcessed, EmitPhysicsProcessed, EmitInputted);
        ConnectNodeEvents();
        return result;
    }

    protected virtual void ConnectNodeEvents()
    {
    }

    protected virtual void DisconnectNodeEvents()
    {
    }

    private void Unregister()
    {
        if (Singleton == null)
        {
            GD.PrintErr("很奇怪，单例节点取消注册时已经为空了！");
            return;
        }

        Singleton.TreeExiting -= Unregister;

        Singleton.Ready -= EmitReady;
        Singleton.TreeExiting -= EmitTreeExiting;
        Singleton.NodeEvent?.Disconnect(EmitProcessed, EmitPhysicsProcessed, EmitInputted);
        DisconnectNodeEvents();
        Singleton = default;
    }
    public bool IsRegistered() => Singleton != null;
}