namespace FrontEnd4IdleStrategyFS.Global.Common

open System.Threading
open FSharp.Control.Reactive

/// Observable 在 Godot 同步上下文中 subscribe 的工具类
module ObservableSyncContextUtil =
    /// 普通 GD.Print 日志输出之类的无需展示层同步的 Observable 订阅可以直接用本方法。
    let subscribe sub observable =
        observable
        |> Observable.subscribeOnContext SynchronizationContext.Current
        |> Observable.subscribe sub
        |> ignore

    /// 涉及到 Godot 节点展示层绘制的内容必须在同步上下文中 Post 执行。
    /// 如果使用 CallDeferred / 信号等，还是会一样报错（信号是因为 EmitSignal 也必须在主线程里）。
    /// 目前貌似只有同步上下文 Post 这种方式可以解决。
    let subscribePost (sub: 'a -> unit) observable =
        // 必须在外面获取，不能简化到 subscribe 里面。里面就不是 Godot 的上下文了
        let syncContext = SynchronizationContext.Current
        observable |> subscribe (fun e -> syncContext.Post((fun _ -> sub e), null))
