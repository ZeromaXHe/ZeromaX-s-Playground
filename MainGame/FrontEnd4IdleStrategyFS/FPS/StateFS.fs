namespace FrontEnd4IdleStrategyFS.FPS

open Godot

[<AbstractClass>]
type StateFS() =
    inherit Node()

    abstract TransitionSignal: StringName with get

    member this.Enter() = ()
    member this.Exit() = ()

    abstract Update: float -> unit

    member this.PhysicsUpdate(delta: float) = ()