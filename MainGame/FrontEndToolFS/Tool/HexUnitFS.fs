namespace FrontEndToolFS.Tool

open System.Collections.Generic
open System.IO
open FrontEndCommonFS.Util
open FrontEndToolFS.HexPlane
open Godot
open Microsoft.FSharp.Core

type IGrid =
    interface
        abstract member AddUnit: PackedScene -> HexCellFS option -> float32 -> unit
        abstract member GetCell: HexCoordinates -> HexCellFS option
        abstract member IncreaseVisibility: HexCellFS -> int -> unit
        abstract member DecreaseVisibility: HexCellFS -> int -> unit
    end

module Bezier =
    let getPoint (a: Vector3) (b: Vector3) (c: Vector3) t =
        let r = 1f - t
        r * r * a + 2f * r * t * b + t * t * c

    let getDerivative (a: Vector3) (b: Vector3) (c: Vector3) t = 2f * ((1f - t) * (b - a) + t * (c - b))

type HexUnitFS() as this =
    inherit CsgBox3D()

    let mutable location: HexCellFS option = None
    let mutable currentTravelLocation: HexCellFS option = None
    let mutable orientation = 0f
    let mutable pathToTravel: HexCellFS List option = None
    let travelSpeed = 4.0
    let rotationSpeed = 180.0f
    let mutable iTravel = 1
    let mutable tTravel = 0.0
    let mutable angleT = 0f

    let onDrawGizmos () =
        // // 通过这个方式临时拿到父节点 HexGridFS 的 PathShower 子节点
        // let pathShower = this.GetNode<Node3D> "../PathShower"
        // pathShower.GetChildren() |> Seq.iter _.QueueFree()
        //
        // if pathToTravel.IsNone || pathToTravel.Value.Count = 0 then
        //     ()
        // else
        //     let drawSphere (pos: Vector3) (r: float32) (pathShower: Node3D) =
        //         let ins = new CsgSphere3D()
        //         ins.Radius <- r
        //         pathShower.AddChild ins
        //         // GlobalPosition 只有在场景树内才能修改
        //         ins.GlobalPosition <- pos
        //
        //     let mutable a = pathToTravel.Value[0].Position
        //     let mutable b = a
        //     let mutable c = a
        //
        //     for i in 1 .. pathToTravel.Value.Count - 1 do
        //         a <- c
        //         b <- pathToTravel.Value[i - 1].Position
        //         c <- (b + pathToTravel.Value[i].Position) * 0.5f
        //
        //         for t in 0.0f..0.1f..0.9f do
        //             drawSphere (Bezier.getPoint a b c t) 2f pathShower
        //
        //     a <- c
        //     b <- pathToTravel.Value[pathToTravel.Value.Count - 1].Position
        //     c <- b
        //
        //     for t in 0.0f..0.1f..0.9f do
        //         drawSphere (Bezier.getPoint a b c t) 2f pathShower
        ()

    let lookAt (point: Vector3) =
        let point = Vector3Util.changeY point this.Position.Y
        this.LookAt point
        orientation <- this.RotationDegrees.Y

    let visionRange = 3

    member this.Location
        with get () = location
        and set value =
            if location.IsSome then
                this.Grid.DecreaseVisibility location.Value visionRange
                location.Value.Unit <- None

            location <- value
            value.Value.Unit <- Some this
            this.Grid.IncreaseVisibility location.Value visionRange
            this.Position <- value.Value.Position

    member this.Orientation
        with get () = orientation
        and set value =
            orientation <- value
            this.RotationDegrees <- Vector3(0f, value, 0f)

    interface IUnit with
        override this.ValidateLocation() = this.ValidateLocation()
        override this.Die() = this.Die()

    [<DefaultValue>]
    val mutable Grid: IGrid

    member this.ValidateLocation() =
        this.Position <- location.Value.Position

    member this.Die() =
        if location.IsSome then
            this.Grid.DecreaseVisibility location.Value visionRange

        location.Value.Unit <- None
        this.QueueFree()

    member this.Save(writer: BinaryWriter) =
        location.Value.Coordinates.Save writer
        writer.Write orientation

    static member val unitPrefab: PackedScene = null with get, set

    static member Load (reader: BinaryReader) (grid: IGrid) =
        let coordinates = HexCoordinates.Load reader
        let orientation = reader.ReadSingle()
        grid.AddUnit <| HexUnitFS.unitPrefab <| grid.GetCell coordinates <| orientation

    member this.IsValidDestination(cell: HexCellFS) =
        cell.IsExplored && not cell.IsUnderWater && cell.Unit.IsNone

    member this.Travel(path: HexCellFS List) =
        location.Value.Unit <- None
        location <- Some path[path.Count - 1]
        location.Value.Unit <- Some this
        pathToTravel <- Some path
        onDrawGizmos ()
        // 替代以下逻辑：
        // StopAllCoroutines();
        // StartCoroutine(TravelPath());
        this.Position <- pathToTravel.Value[0].Position
        iTravel <- 1
        tTravel <- 0.0 // this.GetProcessDeltaTime() * travelSpeed

        angleT <-
            Mathf.RadToDeg(
                this.Basis.Z.SignedAngleTo(
                    pathToTravel.Value[0].Position.DirectionTo pathToTravel.Value[1].Position,
                    Vector3.Up
                )
            )

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
                        this.Grid.DecreaseVisibility pathToTravel.Value[iTravel - 1] visionRange
                    // 确保最后一定停在准确位置
                    if iTravel = pathToTravel.Value.Count then
                        this.Position <- pathToTravel.Value[iTravel - 1].Position
                        this.Orientation <- this.RotationDegrees.Y
                        pathToTravel <- None
                // 修改可见性
                if tTravel = 0.0 && pathToTravel.IsSome then
                    if iTravel = 1 then
                        let decrCell = currentTravelLocation |> Option.defaultValue pathToTravel.Value[0]
                        this.Grid.DecreaseVisibility decrCell visionRange

                    if iTravel < pathToTravel.Value.Count then
                        currentTravelLocation <- Some pathToTravel.Value[iTravel]
                        this.Grid.IncreaseVisibility pathToTravel.Value[iTravel] visionRange
                    else
                        currentTravelLocation <- None

                if pathToTravel.IsNone then
                    ()
                elif iTravel < pathToTravel.Value.Count then
                    let a =
                        if iTravel = 1 then
                            pathToTravel.Value[0].Position
                        else
                            (pathToTravel.Value[iTravel - 2].Position
                             + pathToTravel.Value[iTravel - 1].Position)
                            * 0.5f

                    let b = pathToTravel.Value[iTravel - 1].Position
                    let c = (b + currentTravelLocation.Value.Position) * 0.5f
                    this.Position <- Bezier.getPoint a b c (float32 tTravel)
                    let d = Bezier.getDerivative a b c (float32 tTravel)

                    if not <| d.IsZeroApprox() then
                        this.LookAt(this.Position + d, useModelFront = true)

                    tTravel <- tTravel + delta * travelSpeed
                elif iTravel = pathToTravel.Value.Count then
                    let a =
                        (pathToTravel.Value[iTravel - 2].Position
                         + pathToTravel.Value[iTravel - 1].Position)
                        * 0.5f

                    let b = location.Value.Position
                    let c = b
                    this.Position <- Bezier.getPoint a b c (float32 tTravel)
                    let d = Bezier.getDerivative a b c (float32 tTravel)

                    if not <| d.IsZeroApprox() then
                        this.LookAt(this.Position + d, useModelFront = true)

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
