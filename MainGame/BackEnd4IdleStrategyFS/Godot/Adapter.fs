namespace BackEnd4IdleStrategyFS.Godot

module Adapter =
    type IAStar2D =
        interface
            abstract member GetPointConnections: int -> int seq
            abstract member AddPoint: int -> int * int -> unit
            abstract member ConnectPoints: int -> int -> unit
        end
