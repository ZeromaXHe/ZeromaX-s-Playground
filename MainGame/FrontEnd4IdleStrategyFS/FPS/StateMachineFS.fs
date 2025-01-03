namespace FrontEnd4IdleStrategyFS.FPS

open FrontEnd4IdleStrategyFS.Global
open FrontEndCommonFS.Util
open Godot
open Godot.Collections

type StateMachineFS() =
    inherit Node()

    let states = Dictionary<string, StateFS>()

    member val currentState = Unchecked.defaultof<StateFS> with get, set

    member this.OnChildTransition newStateName =
        if states.ContainsKey newStateName then
            let newState = states[newStateName]

            if newState <> this.currentState then
                this.currentState.Exit()
                newState.Enter(this.currentState)
                this.currentState <- newState
        else
            GD.PushWarning("State 不存在")
        // F# 的 unit 在 Callable.From 里面会报错，所以返回一个 1 让出参类型为 int
        // VariantUtils.generic.cs:16
        // @ Godot.NativeInterop.godot_variant Godot.NativeInterop.VariantUtils+GenericConversion`1.ToVariant(T&):
        // System.InvalidOperationException: The type is not supported for conversion to/from Variant: 'Microsoft.FSharp.Core.Unit'
        1

    override this._Ready() =
        for child in this.GetChildren() do
            match child with
            | :? StateFS as c ->
                states[child.Name] <- c
                c.Connect(c.TransitionSignal, Callable.From this.OnChildTransition) |> ignore
            | _ -> GD.PushWarning("状态机包含不兼容子节点")

        async {
            let! _ =
                this.ToSignal(this.Owner, Node.SignalName.Ready)
                |> AwaitUtil.awaiterToTask
                |> Async.AwaitTask

            this.currentState.Enter(this.currentState)
        }
        // 这里用 Async.Start 会报错，不能使用新线程，而必须使用 StartImmediate 用当前线程：
        // SignalAwaiter.cs:18 @ Godot.SignalAwaiter..ctor(Godot.GodotObject, Godot.StringName, Godot.GodotObject):
        // Caller thread can't call this function in this node (/root/FpsDemo/FpsController).
        // Use call_deferred() or call_thread_group() instead.
        |> Async.StartImmediate

    override this._Process delta =
        this.currentState.Update delta
        FpsGlobalNodeFS.Instance.debug.AddProperty "Current State" this.currentState.Name 1

    override this._PhysicsProcess delta = this.currentState.PhysicsUpdate delta
