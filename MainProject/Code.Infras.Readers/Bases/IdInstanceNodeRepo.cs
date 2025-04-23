using Godot;
using GodotNodes.Abstractions;
using Infras.Readers.Abstractions.Bases;

namespace Infras.Readers.Bases;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-22 11:41:59
public abstract class IdInstanceNodeRepo<T> : IIdInstanceNodeRepo<T> where T : INode
{
    public event Action<T>? Ready;
    public event Action<T>? TreeExiting;
    public event Action<T, double>? Processed;
    public event Action<T, double>? PhysicsProcessed;
    public event Action<T, InputEvent>? Inputted;
    private Action EmitReady(T instance) => () => Ready?.Invoke(instance);
    private Action EmitTreeExiting(T instance) => () => TreeExiting?.Invoke(instance);
    private Action<double> EmitProcessed(T instance) => delta => Processed?.Invoke(instance, delta);
    private Action<double> EmitPhysicsProcessed(T instance) => delta => PhysicsProcessed?.Invoke(instance, delta);
    private Action<InputEvent> EmitInputted(T instance) => inputEvent => Inputted?.Invoke(instance, inputEvent);

    public HashSet<T> Instances { get; } = [];
    private readonly Dictionary<T, Action> _unregisters = new();

    public void Register(T instance)
    {
        Instances.Add(instance);
        // 对于这种必须持有 instance 的闭包的委托监听函数情况，必须持有当时监听的委托，用于后面解绑事件监听
        var emitReady = EmitReady(instance);
        var emitTreeExiting = EmitTreeExiting(instance);
        var emitProcessed = EmitProcessed(instance);
        var emitPhysicsProcessed = EmitPhysicsProcessed(instance);
        var emitInputted = EmitInputted(instance);
        var unregister = Unregister(instance, emitReady, emitTreeExiting, emitProcessed,
            emitPhysicsProcessed, emitInputted);
        _unregisters.Add(instance, unregister);
        instance.TreeExiting += unregister;

        instance.Ready += emitReady;
        instance.TreeExiting += emitTreeExiting;
        instance.NodeEvent?.Connect(emitProcessed, emitPhysicsProcessed, emitInputted);
        RegisterHook(instance);
    }

    private Action Unregister(T instance, Action emitReady, Action emitTreeExiting, Action<double> emitProcessed,
        Action<double> emitPhysicsProcessed, Action<InputEvent> emitInputted) => () =>
    {
        if (!Instances.Contains(instance))
        {
            GD.PrintErr("很奇怪，多例节点取消注册时已经不在了！");
            return;
        }

        _unregisters.Remove(instance, out var unregister);
        instance.TreeExiting -= unregister;

        instance.Ready -= emitReady;
        instance.TreeExiting -= emitTreeExiting;
        instance.NodeEvent?.Disconnect(emitProcessed, emitPhysicsProcessed, emitInputted);
        UnregisterHook(instance);
        Instances.Remove(instance);
    };

    protected virtual void RegisterHook(T instance)
    {
    }

    protected virtual void UnregisterHook(T instance)
    {
    }
}