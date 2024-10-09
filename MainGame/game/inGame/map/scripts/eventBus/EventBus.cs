using System;
using Godot;

namespace ZeromaXPlayground.game.inGame.map.scripts.eventBus;

public sealed partial class EventBus : GodotObject
{
    /**
     * 部队到达目的地事件
     */
    [Signal]
    public delegate void MarchingArmyArrivedDestinationEventHandler(int marchingArmyId);

    /**
     * 延迟加载的懒汉式单例初始化（原理估计和 Java 类似，lambda 也是匿名内部类？）
     */
    public static EventBus Instance => Lazy.Value;

    private static readonly Lazy<EventBus> Lazy = new(() => new EventBus());

    private EventBus()
    {
    }
}