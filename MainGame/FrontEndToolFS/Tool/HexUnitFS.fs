namespace FrontEndToolFS.Tool

open System.Collections.Generic
open System.IO
open FrontEndCommonFS.Util
open FrontEndToolFS.HexPlane
open Godot
open Microsoft.FSharp.Core

type IGridForUnit =
    interface
        abstract member MakeChildOfColumn: Node -> int -> unit
        abstract member AddUnit: PackedScene -> HexCellFS option -> float32 -> unit
        abstract member GetCell: HexCoordinates -> HexCellFS option
        abstract member GetCell: int -> HexCellFS
        abstract member IncreaseVisibility: HexCellFS -> int -> unit
        abstract member DecreaseVisibility: HexCellFS -> int -> unit
        abstract member GetPathShower: Node3D
        abstract member PathShowerOn: bool
    end

module Bezier =
    let getPoint (a: Vector3) (b: Vector3) (c: Vector3) t =
        let r = 1f - t
        r * r * a + 2f * r * t * b + t * t * c

    let getDerivative (a: Vector3) (b: Vector3) (c: Vector3) t = 2f * ((1f - t) * (b - a) + t * (c - b))

type HexUnitFS() as this =
    inherit CsgBox3D()

    let mutable locationCellIndex = -1
    let mutable orientation = 0f
    let travelSpeed = 4.0
    let rotationSpeed = 180.0f

    member this.VisionRange = 3

    member this.Location
        with get () = this.Grid.GetCell locationCellIndex
        and set (value: HexCellFS) =
            if locationCellIndex >= 0 then
                let location = this.Grid.GetCell locationCellIndex
                this.Grid.DecreaseVisibility location this.VisionRange
                location.Unit <- None

            locationCellIndex <- value.Index
            value.Unit <- Some this
            this.Grid.IncreaseVisibility value this.VisionRange
            this.Position <- value.Position
            this.Grid.MakeChildOfColumn this value.ColumnIndex

    member this.Orientation
        with get () = orientation
        and set value =
            orientation <- value
            this.RotationDegrees <- Vector3(0f, value, 0f)

    interface IUnit with
        override this.ValidateLocation() = this.ValidateLocation()
        override this.Die() = this.Die()

    [<DefaultValue>]
    val mutable Grid: IGridForUnit

    member this.ValidateLocation() =
        this.Position <- (this.Grid.GetCell locationCellIndex).Position

    member this.Die() =
        let location = this.Grid.GetCell locationCellIndex
        this.Grid.DecreaseVisibility location this.VisionRange
        location.Unit <- None
        this.QueueFree()

    member this.Save(writer: BinaryWriter) =
        (this.Grid.GetCell locationCellIndex).Coordinates.Save writer
        writer.Write orientation

    static member val unitPrefab: PackedScene = null with get, set

    static member Load (reader: BinaryReader) (grid: IGridForUnit) =
        let coordinates = HexCoordinates.Load reader
        let orientation = reader.ReadSingle()
        grid.AddUnit <| HexUnitFS.unitPrefab <| grid.GetCell coordinates <| orientation

    member this.IsValidDestination(cell: HexCellFS) =
        cell.IsExplored && not cell.IsUnderWater && cell.Unit.IsNone

    let mutable currentTravelLocation: HexCellFS option = None
    let mutable currentTravelLocationIndex = -1
    let mutable currentColumn = -1
    let mutable pathToTravel: int List option = None
    let mutable iTravel = 1
    let mutable aTravel = Vector3.Zero
    let mutable bTravel = Vector3.Zero
    let mutable cTravel = Vector3.Zero
    let mutable tTravel = 0.0
    let mutable angleT = 0f

    let lookAt (point: Vector3) =
        let mutable point = point
        point.Y <- this.Position.Y
        this.LookAt(point, useModelFront = true)
        orientation <- this.RotationDegrees.Y

    let onDrawGizmos () =
        if this.Grid.PathShowerOn then
            let pathShower = this.Grid.GetPathShower
            pathShower.GetChildren() |> Seq.iter _.QueueFree()

            if pathToTravel.IsNone || pathToTravel.Value.Count = 0 then
                ()
            else
                let drawSphere (pos: Vector3) (r: float32) (pathShower: Node3D) =
                    let ins = new CsgSphere3D()
                    ins.Radius <- r
                    pathShower.AddChild ins
                    // GlobalPosition 只有在场景树内才能修改
                    ins.GlobalPosition <- pos

                let mutable a = (this.Grid.GetCell pathToTravel.Value[0]).Position
                let mutable b = a
                let mutable c = a

                for i in 1 .. pathToTravel.Value.Count - 1 do
                    a <- c
                    b <- (this.Grid.GetCell pathToTravel.Value[i - 1]).Position
                    c <- (b + (this.Grid.GetCell pathToTravel.Value[i]).Position) * 0.5f

                    for t in 0.0f..0.1f..0.9f do
                        drawSphere (Bezier.getPoint a b c t) 2f pathShower

                a <- c
                b <- (this.Grid.GetCell pathToTravel.Value[pathToTravel.Value.Count - 1]).Position
                c <- b

                for t in 0.0f..0.1f..0.9f do
                    drawSphere (Bezier.getPoint a b c t) 2f pathShower

    member this.Travel(path: int List) =
        let location = this.Grid.GetCell locationCellIndex
        location.Unit <- None
        let location = this.Grid.GetCell path[path.Count - 1]
        locationCellIndex <- location.Index
        location.Unit <- Some this
        pathToTravel <- Some path
        onDrawGizmos ()
        // 替代以下逻辑：
        // StopAllCoroutines();
        // StartCoroutine(TravelPath());
        let path0 = this.Grid.GetCell path[0]
        this.Position <- path0.Position
        iTravel <- 1
        tTravel <- 0.0 // this.GetProcessDeltaTime() * travelSpeed
        aTravel <- path0.Position
        bTravel <- path0.Position
        cTravel <- path0.Position
        let mutable point = (this.Grid.GetCell pathToTravel.Value[1]).Position
        point.Y <- this.Position.Y

        if HexMetrics.wrapping () then
            let xDistance = point.X - this.Position.X

            if xDistance < -HexMetrics.innerRadius * float32 HexMetrics.wrapSize then
                point.X <- point.X + HexMetrics.innerDiameter * float32 HexMetrics.wrapSize
            elif xDistance > HexMetrics.innerRadius * float32 HexMetrics.wrapSize then
                point.X <- point.X - HexMetrics.innerDiameter * float32 HexMetrics.wrapSize

        angleT <- Mathf.RadToDeg(this.Basis.Z.SignedAngleTo(path0.Position.DirectionTo point, Vector3.Up))

    // TODO: 现在这里因为没有 Unity 的协程，直接用 _Process 做的，代码丑的一比
    member this.TravelPath delta =
        if pathToTravel.IsSome && iTravel <= pathToTravel.Value.Count then
            if abs angleT > 0f then
                // 如果一开始方面没对准下一个单元格，则旋转方向
                let rotDeg =
                    if angleT > 0f then
                        Mathf.Min(rotationSpeed * float32 delta, angleT)
                    else
                        Mathf.Max(-rotationSpeed * float32 delta, angleT)

                this.RotationDegrees <- Vector3Util.changeY this.RotationDegrees <| this.RotationDegrees.Y + rotDeg

                angleT <- angleT - rotDeg
            else
                // 后续向目的地移动的逻辑
                if tTravel >= 1.0 then
                    tTravel <- 0.0
                    iTravel <- iTravel + 1
                    // 移除前一格可见性
                    if iTravel < pathToTravel.Value.Count then
                        this.Grid.DecreaseVisibility
                            (this.Grid.GetCell pathToTravel.Value[iTravel - 1])
                            this.VisionRange
                    // 确保最后一定停在准确位置
                    if iTravel = pathToTravel.Value.Count then
                        this.Position <- (this.Grid.GetCell pathToTravel.Value[iTravel - 1]).Position
                        this.Orientation <- this.RotationDegrees.Y
                        pathToTravel <- None
                // 每个循环的数据初始化：可见性、a、b、c
                if tTravel = 0.0 && pathToTravel.IsSome then
                    if iTravel = 1 then
                        if currentTravelLocationIndex < 0 then
                            currentTravelLocationIndex <- pathToTravel.Value[0]

                        currentTravelLocation <- Some <| this.Grid.GetCell currentTravelLocationIndex
                        this.Grid.DecreaseVisibility currentTravelLocation.Value this.VisionRange
                        currentColumn <- currentTravelLocation.Value.ColumnIndex

                    if iTravel < pathToTravel.Value.Count then
                        currentTravelLocation <- Some <| this.Grid.GetCell pathToTravel.Value[iTravel]
                        currentTravelLocationIndex <- currentTravelLocation.Value.Index
                        aTravel <- cTravel
                        bTravel <- (this.Grid.GetCell pathToTravel.Value[iTravel - 1]).Position
                        let nextColumn = currentTravelLocation.Value.ColumnIndex

                        if currentColumn <> nextColumn then
                            if nextColumn < currentColumn - 1 then
                                aTravel.X <- aTravel.X - HexMetrics.innerDiameter * float32 HexMetrics.wrapSize
                                bTravel.X <- bTravel.X - HexMetrics.innerDiameter * float32 HexMetrics.wrapSize
                            elif nextColumn > currentColumn + 1 then
                                aTravel.X <- aTravel.X + HexMetrics.innerDiameter * float32 HexMetrics.wrapSize
                                bTravel.X <- bTravel.X + HexMetrics.innerDiameter * float32 HexMetrics.wrapSize

                            this.Grid.MakeChildOfColumn this nextColumn
                            currentColumn <- nextColumn

                        cTravel <- (bTravel + currentTravelLocation.Value.Position) * 0.5f
                        this.Grid.IncreaseVisibility (this.Grid.GetCell pathToTravel.Value[iTravel]) this.VisionRange
                    else
                        currentTravelLocationIndex <- -1
                        let location = this.Grid.GetCell locationCellIndex
                        aTravel <- cTravel
                        bTravel <- location.Position
                        cTravel <- bTravel
                        this.Grid.IncreaseVisibility location this.VisionRange

                if pathToTravel.IsNone then
                    ()
                else
                    this.Position <- Bezier.getPoint aTravel bTravel cTravel (float32 tTravel)
                    let d = Bezier.getDerivative aTravel bTravel cTravel (float32 tTravel)

                    if not <| d.IsZeroApprox() then
                        // 让 d 大一点是因为在块列 0 向左边界上会转向错误，我猜是 d 太小导致的
                        lookAt <| this.Position + d * 1000f

                    tTravel <- tTravel + delta * travelSpeed

    member this.GetMoveCost (fromCell: HexCellFS) (toCell: HexCellFS) direction =
        let edgeType = fromCell.GetEdgeType toCell

        if edgeType = HexEdgeType.Cliff then
            -1
        elif fromCell.HasRoadThroughEdge direction then
            1
        elif fromCell.Walled <> toCell.Walled then
            -1 // 被墙阻挡就得直接跳出逻辑
        else
            if edgeType = HexEdgeType.Flat then 5 else 10
            + toCell.UrbanLevel
            + toCell.FarmLevel
            + toCell.PlantLevel

    member this.Speed = 24

    override this._Process(delta) = this.TravelPath delta
