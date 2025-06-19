namespace TO.FSharp.Domains.Components.HexSpheres.Tiles

open Friflo.Engine.ECS
open TO.FSharp.Domains.Utils.HexSpheres

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-09 16:37:09
[<Struct>]
type TileValue =
    interface IComponent
    val Values: int
    new(values) = { Values = values }

    member private this.Get mask shift =
        int (uint this.Values >>> shift) &&& mask // C# 的 >>>（无符号右移）需要在 F# 中转为 uint 再右移

    member private this.With value mask shift =
        TileValue((this.Values &&& ~~~(mask <<< shift)) ||| ((value &&& mask) <<< shift))

    member this.Elevation = (this.Get 31 0) - 15
    member this.WithElevation value = this.With (value + 15) 31 0

    member this.GetEdgeType(otherTile: TileValue) =
        HexMetrics.getEdgeType this.Elevation otherTile.Elevation

    member this.WaterLevel = this.Get 31 5
    member this.ViewElevation = max this.Elevation this.WaterLevel
    member this.IsUnderwater = this.WaterLevel > this.Elevation
    member this.WithWaterLevel value = this.With value 31 5

    member this.StreamBedY(unitHeight: float32) =
        (float32 this.Elevation + HexMetrics.streamBedElevationOffset) * unitHeight

    member this.RiverSurfaceY(unitHeight: float32) =
        (float32 this.Elevation + HexMetrics.waterElevationOffset) * unitHeight

    member this.WaterSurfaceY(unitHeight: float32) =
        (float32 this.WaterLevel + HexMetrics.waterElevationOffset) * unitHeight

    member this.UrbanLevel = this.Get 3 10
    member this.WithUrbanLevel value = this.With value 3 10
    member this.FarmLevel = this.Get 3 12
    member this.WithFarmLevel value = this.With value 3 12
    member this.PlantLevel = this.Get 3 14
    member this.WithPlantLevel value = this.With value 3 14
    member this.SpecialIndex = this.Get 255 16
    member this.IsSpecial = this.SpecialIndex > 0
    member this.WithSpecialIndex index = this.With index 255 16
    member this.TerrainTypeIndex = this.Get 255 24
    member this.WithTerrainTypeIndex index = this.With index 255 24
