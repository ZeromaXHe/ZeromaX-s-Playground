namespace FrontEnd4IdleStrategyFS.Global.Common

open Godot

module BackEndUtil =
    let toI (vec: Vector2I) = vec.X, vec.Y
    let fromI (x, y) = Vector2I(x, y)
    let from (x, y) = Vector2(float32 x, float32 y)
    let to3 (vec: Vector3) = vec.X, vec.Y, vec.Z
