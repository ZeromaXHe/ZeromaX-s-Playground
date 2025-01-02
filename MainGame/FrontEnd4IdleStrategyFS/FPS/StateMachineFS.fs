namespace FrontEnd4IdleStrategyFS.FPS

open FrontEnd4IdleStrategyFS.Global
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
                newState.Enter()
                this.currentState <- newState
        else
            GD.PushWarning("State 不存在")
    
    override this._Ready() =
        for child in this.GetChildren() do
            match child with
            | :? StateFS as c ->
                states[child.Name] <- c
                c.Connect(c.TransitionSignal, Callable.From this.OnChildTransition) |> ignore
            | _ -> GD.PushWarning("状态机包含不兼容子节点")
        this.currentState.Enter()

    override this._Process delta =
        this.currentState.Update delta
        FpsGlobalNodeFS.Instance.debug.AddProperty "Current State" this.currentState.Name 1

    override this._PhysicsProcess delta =
        this.currentState.PhysicsUpdate delta
    
    