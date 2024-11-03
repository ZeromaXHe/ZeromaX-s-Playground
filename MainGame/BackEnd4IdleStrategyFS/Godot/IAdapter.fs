namespace BackEnd4IdleStrategyFS.Godot

module IAdapter =
    type IVector2I = int * int
    type IAStar2D<'s> =
        interface
            abstract member GetPointConnections: int -> 's -> int seq
            abstract member AddPoint: int -> IVector2I -> 's -> 's
            abstract member ConnectPoints: int -> int -> 's -> 's
        end

    type ITileMapLayer<'r> =
        interface
            abstract member GetUsedCells: 'r -> IVector2I seq
            abstract member GetSurroundingCells: IVector2I -> 'r -> IVector2I seq
        end