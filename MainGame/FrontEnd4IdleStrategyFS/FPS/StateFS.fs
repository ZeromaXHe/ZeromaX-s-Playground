namespace FrontEnd4IdleStrategyFS.FPS

open Godot

[<AbstractClass>]
type StateFS() =
    inherit Node()

    abstract TransitionSignal: StringName with get

    abstract Enter: unit -> unit
    member this.Exit() = ()
    abstract Update: float -> unit
    member this.PhysicsUpdate(delta: float) = ()
