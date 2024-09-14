using System;
using Godot;

namespace ZeromaXPlayground.game.inGame.map.scripts.eventBus;

public sealed partial class EventBus : GodotObject
{
    /**
     * 占领领土事件
     * Godot 信号无法传递 int?（用来传递 null 表示无主之地），只能用 int -1（Constants.NullId）表示了。
     */
    [Signal]
    public delegate void TerritoryConqueredEventHandler(int conquerorId, int loserId, Vector2I vec);

    /**
     * 计时器 tick 事件
     */
    [Signal]
    public delegate void TimeTickedEventHandler();

    /**
     * 延迟加载的懒汉式单例初始化（原理估计和 Java 类似，lambda 也是匿名内部类？）
     */
    public static EventBus Instance => Lazy.Value;

    private static readonly Lazy<EventBus> Lazy = new(() => new EventBus());

    private EventBus()
    {
    }
}