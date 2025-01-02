namespace FrontEnd4IdleStrategyFS.Global

open Godot

type IDebug =
    abstract AddProperty<'a>: title: string -> value: 'a -> order: int -> unit

type IPlayer =
    abstract speedDefault: float32
    abstract speedSprinting: float32
    abstract speed: float32 with get, set
    abstract Velocity: Vector3
    abstract IsOnFloor: unit -> bool

type FpsGlobalNodeFS() =
    inherit Node()
    member val debug = Unchecked.defaultof<IDebug> with get, set
    member val player = Unchecked.defaultof<IPlayer> with get, set

    // 通过这个方式简化对全局节点的调用（Godot 官方文档“编写脚本 - 核心特性 - 单例（自动加载）”中有介绍）
    static member val Instance = Unchecked.defaultof<FpsGlobalNodeFS> with get, set
    
    override this._Ready() =
        FpsGlobalNodeFS.Instance <- this