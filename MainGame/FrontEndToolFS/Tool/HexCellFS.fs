namespace FrontEndToolFS.Tool

open FrontEndToolFS.HexPlane
open FrontEndToolFS.HexPlane.HexDirection
open FrontEndToolFS.HexPlane.HexFlags
open Godot
open Microsoft.FSharp.Core

type IChunk =
    abstract member Refresh: unit -> unit

type IUnit =
    abstract member ValidateLocation: unit -> unit
    abstract member Die: unit -> unit

type IGridForCell =
    interface
        abstract member GetCell: HexCoordinates -> HexCellFS option
        abstract member RefreshCell: int -> unit
        abstract member RefreshCellPosition: int -> unit
        abstract member RefreshCellWithDependents: int -> unit
        abstract member ShaderData: HexCellShaderData
        abstract member CellData: HexCellData array
        abstract member CellPositions: Vector3 array
        abstract member GetCellUnits: int -> IUnit option
        abstract member SetCellUnits: int -> IUnit option -> unit
    end

and(* [<CustomEquality; NoComparison>]*) HexCellFS =
    struct
        val index: int
        val grid: IGridForCell
        public new(index, grid) = { index = index; grid = grid }
        member this.Index = this.index
        member this.Grid = this.grid

        /// 不知道为啥这就会死循环栈溢出
        // static member (=)(a: HexCellFS, b: HexCellFS) = a.Index = b.Index && a.Grid = b.Grid
        // static member (<>)(a: HexCellFS, b: HexCellFS) = a.Index <> b.Index || a.Grid <> b.Grid
        //
        // override this.Equals(other: obj) =
        //     match other with
        //     | :? HexCellFS as other -> this = other
        //     | _ -> false
        //
        // override this.GetHashCode() =
        //     this.index.GetHashCode() ^^^ this.grid.GetHashCode()

        member this.Coordinates = this.Grid.CellData[this.Index].coordinates
        member this.Position = this.Grid.CellPositions[this.Index]
        // 标志
        member this.Flags
            with get () = this.Grid.CellData[this.Index].flags
            and set value = this.Grid.CellData[this.Index].flags <- value
        // 值
        member this.Values
            with get () = this.Grid.CellData[this.Index].values
            and set value = this.Grid.CellData[this.Index].values <- value

        member this.Refresh() = this.Grid.RefreshCell this.Index

        member this.GetNeighbor(direction: HexDirection) =
            this.Grid.GetCell <| this.Coordinates.Step direction

        member this.SetTerrainTypeIndex terrainTypeIndex =
            if this.Values.TerrainTypeIndex <> terrainTypeIndex then
                let mutable this = this
                this.Values <- this.Values.WithTerrainTypeIndex terrainTypeIndex
                this.Grid.ShaderData.RefreshTerrain this.Index

        /// 高度
        member this.SetElevation elevation =
            if this.Values.Elevation <> elevation then
                let mutable this = this
                this.Values <- this.Values.WithElevation elevation
                this.Grid.ShaderData.ViewElevationChanged this.Index
                this.Grid.RefreshCellPosition this.Index
                this.ValidateRivers()
                let flags = this.Flags

                for d in allHexDirs () do
                    if flags.HasRoad d then
                        let neighbor = this.GetNeighbor d

                        if Mathf.Abs(elevation - neighbor.Value.Values.Elevation) > 1 then
                            this.RemoveRoad d

                this.Grid.RefreshCellWithDependents this.Index

        static member CanRiverFlow (fromV: HexValues) (toV: HexValues) =
            fromV.Elevation >= toV.Elevation || fromV.WaterLevel = toV.Elevation

        /// 移除流出河流
        member this.RemoveOutgoingRiver() =
            if this.Flags.HasAny HexFlags.RiverOut then
                match this.GetNeighbor this.Flags.RiverOutDirection.Value with
                | Some neighbor ->
                    let mutable this = this
                    this.Flags <- this.Flags.Without HexFlags.RiverOut
                    let mutable neighbor = neighbor
                    neighbor.Flags <- neighbor.Flags.Without HexFlags.RiverIn
                    neighbor.Refresh()
                    this.Refresh()
                | None -> ()

        /// 移除流入河流
        member this.RemoveIncomingRiver() =
            if this.Flags.HasAny HexFlags.RiverIn then
                match this.GetNeighbor this.Flags.RiverInDirection.Value with
                | Some neighbor ->
                    let mutable this = this
                    this.Flags <- this.Flags.Without HexFlags.RiverIn
                    let mutable neighbor = neighbor
                    neighbor.Flags <- neighbor.Flags.Without HexFlags.RiverOut
                    neighbor.Refresh()
                    this.Refresh()
                | None -> ()

        /// 移除河流
        member this.RemoveRiver() =
            this.RemoveIncomingRiver()
            this.RemoveOutgoingRiver()

        member this.SetOutgoingRiver(direction: HexDirection) =
            if this.Flags.HasRiverOut direction then
                GD.Print $"SetOutgoingRiver already river {direction}"
                ()
            else
                let neighborOpt = this.GetNeighbor direction

                if
                    neighborOpt.IsSome
                    && HexCellFS.CanRiverFlow this.Values neighborOpt.Value.Values
                then
                    let mutable neighbor = neighborOpt.Value
                    // GD.Print $"SetOutgoingRiver refresh {direction}"
                    this.RemoveOutgoingRiver()

                    if this.Flags.HasRiverIn direction then
                        this.RemoveIncomingRiver()

                    let mutable this = this
                    this.Flags <- this.Flags.WithRiverOut direction
                    this.Values <- this.Values.WithSpecialIndex 0
                    neighbor.RemoveIncomingRiver()
                    neighbor.Flags <- neighbor.Flags.WithRiverIn direction.Opposite
                    neighbor.Values <- neighbor.Values.WithSpecialIndex 0
                    this.RemoveRoad direction
                else
                    GD.Print $"SetOutgoingRiver no neighbor or low {direction}"
                    ()

        /// 某个方向上是否有道路
        member this.HasRoadThroughEdge(direction: HexDirection) = this.Flags.HasRoad direction

        member this.RemoveRoad(direction: HexDirection) =
            let mutable this = this
            this.Flags <- this.Flags.WithoutRoad direction

            match this.GetNeighbor direction with
            | Some neighbor ->
                let mutable neighbor = neighbor
                neighbor.Flags <- neighbor.Flags.WithoutRoad direction.Opposite
                neighbor.Refresh()
                this.Refresh()
            | None -> ()

        /// 移除道路
        member this.RemoveRoads() =
            let flags = this.Flags
            allHexDirs () |> List.filter flags.HasRoad |> List.iter this.RemoveRoad

        /// 添加道路
        member this.AddRoad(direction: HexDirection) =
            let flags = this.Flags

            match this.GetNeighbor direction with
            | Some neighbor ->
                if
                    not <| flags.HasRoad direction
                    && not <| flags.HasRiver direction
                    && this.Values.SpecialIndex = 0
                    && neighbor.Values.SpecialIndex = 0
                    && Mathf.Abs(this.Values.Elevation - neighbor.Values.Elevation) <= 1
                then
                    let mutable this = this
                    this.Flags <- flags.WithRoad direction
                    let mutable neighbor = neighbor
                    neighbor.Flags <- neighbor.Flags.WithRoad direction.Opposite
                    neighbor.Refresh()
                    this.Refresh()
            | None -> ()

        /// 水位
        member this.SetWaterLevel waterLevel =
            if this.Values.WaterLevel <> waterLevel then
                let mutable this = this
                this.Values <- this.Values.WithWaterLevel waterLevel
                this.Grid.ShaderData.ViewElevationChanged this.Index
                this.ValidateRivers()
                this.Grid.RefreshCellWithDependents this.Index

        member this.ValidateRivers() =
            let flags = this.Flags

            if
                flags.HasAny HexFlags.RiverOut
                && not
                   <| HexCellFS.CanRiverFlow this.Values (this.GetNeighbor flags.RiverOutDirection.Value).Value.Values
            then
                this.RemoveOutgoingRiver()

            if
                flags.HasAny HexFlags.RiverIn
                && not
                   <| HexCellFS.CanRiverFlow this.Values (this.GetNeighbor flags.RiverInDirection.Value).Value.Values
            then
                this.RemoveIncomingRiver()

        // 城市级别
        member this.SetUrbanLevel urbanLevel =
            if this.Values.UrbanLevel <> urbanLevel then
                let mutable this = this
                this.Values <- this.Values.WithUrbanLevel urbanLevel
                this.Refresh()
        // 农场级别
        member this.SetFarmLevel farmLevel =
            if this.Values.FarmLevel <> farmLevel then
                let mutable this = this
                this.Values <- this.Values.WithFarmLevel farmLevel
                this.Refresh()
        // 植物级别
        member this.SetPlantLevel plantLevel =
            if this.Values.PlantLevel <> plantLevel then
                let mutable this = this
                this.Values <- this.Values.WithPlantLevel plantLevel
                this.Refresh()

        // 围墙
        member this.SetWalled walled =
            let flags = this.Flags

            let newFlags =
                if walled then
                    flags.With HexFlags.Walled
                else
                    flags.Without HexFlags.Walled

            if flags <> newFlags then
                let mutable this = this
                this.Flags <- newFlags
                this.Grid.RefreshCellWithDependents this.Index
        // 特殊特征
        member this.SetSpecialIndex specialIndex =
            if
                this.Values.SpecialIndex <> specialIndex
                && (this.Flags.HasNone HexFlags.River || specialIndex = 0)
            then
                let mutable this = this
                this.Values <- this.Values.WithSpecialIndex specialIndex

                if specialIndex <> 0 then
                    this.RemoveRoads()

                this.Refresh()
        // 单位
        member this.Unit
            with get () = this.Grid.GetCellUnits this.Index
            and set v = this.Grid.SetCellUnits this.Index v

    // member this.SetMapData data =
    //     // GD.Print $"Setting {this.Coordinates} map data {data}"
    //     this.Grid.ShaderData.SetMapData this data
    end
