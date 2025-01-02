namespace FrontEnd4IdleStrategyFS.FPS

open Godot

[<AbstractClass>]
type StateFS() =
    inherit Node()

    abstract TransitionSignal: StringName with get

    abstract Enter: previousState: StateFS -> unit
    abstract Exit: unit -> unit
    abstract Update: delta: float -> unit
    member this.PhysicsUpdate(delta: float) = ()
