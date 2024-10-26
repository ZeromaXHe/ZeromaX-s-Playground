namespace FrontEnd4IdleStrategyFS.Common

module BackEndUtil =
    open Godot

    let toBackEnd (vec: Vector2I) = (vec.X, vec.Y)

    let fromBackEndI (x: int, y: int) = Vector2I(x, y)

    let fromBackEnd (x: int, y: int) = Vector2(float32 x, float32 y)