using Godot;

namespace GodotNodes.Abstractions.Addition;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 14:03:18
/// Description: 自定义的 INode 事件
public class NodeEvent(bool process = false, bool physicsProcess = false, bool input = false)
{
    public bool Process => process;
    public bool PhysicsProcess => physicsProcess;
    public bool Input => input;

    private event Action<double>? Processed;
    private event Action<double>? PhysicsProcessed;
    private event Action<InputEvent>? Inputted;
    public void EmitProcessed(double delta) => Processed?.Invoke(delta);
    public void EmitPhysicsProcessed(double delta) => PhysicsProcessed?.Invoke(delta);
    public void EmitInputted(InputEvent inputEvent) => Inputted?.Invoke(inputEvent);

    public void Connect(Action<double> onProcessed, Action<double> onPhysicsProcessed, Action<InputEvent> onInputted)
    {
        if (process) Processed += onProcessed;
        if (physicsProcess) PhysicsProcessed += onPhysicsProcessed;
        if (input) Inputted += onInputted;
    }

    public void Disconnect(Action<double> onProcessed, Action<double> onPhysicsProcessed, Action<InputEvent> onInputted)
    {
        if (process) Processed -= onProcessed;
        if (physicsProcess) PhysicsProcessed -= onPhysicsProcessed;
        if (input) Inputted -= onInputted;
    }
}