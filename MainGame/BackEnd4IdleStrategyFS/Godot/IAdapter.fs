namespace BackEnd4IdleStrategyFS.Godot

module IAdapter =
    type IVector2I = int * int

    type IAStar2D =
        interface
            abstract member GetPointConnections: int -> int seq
            abstract member AddPoint: int -> IVector2I -> unit
            abstract member ConnectPoints: int -> int -> unit
        end

    type ITileMapLayer =
        interface
            abstract member GetUsedCells: unit -> IVector2I seq
            abstract member GetSurroundingCells: IVector2I -> IVector2I seq
        end
