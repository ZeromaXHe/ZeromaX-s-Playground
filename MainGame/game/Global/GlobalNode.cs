using Godot;
using System;
using BackEnd4IdleStrategy.Framework;

public partial class GlobalNode : Node
{
    // TODO: 暂时先直接在这里初始化跑通逻辑
    public readonly GameControllerContainer GameControllerContainer = new();
}
