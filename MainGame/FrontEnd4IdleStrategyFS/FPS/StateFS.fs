namespace FrontEnd4IdleStrategyFS.FPS

open Godot

[<AbstractClass>]
type StateFS() =
    inherit Node()

    abstract TransitionSignal: StringName with get

    abstract Enter: unit -> unit
    abstract Exit: unit -> unit
    abstract Update: float -> unit
    member this.PhysicsUpdate(delta: float) = ()
