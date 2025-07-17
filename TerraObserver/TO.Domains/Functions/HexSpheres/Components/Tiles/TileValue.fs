namespace TO.Domains.Functions.HexSpheres.Components.Tiles

open TO.Domains.Functions.HexMetrics
open TO.Domains.Types.HexSpheres.Components.Tiles

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-29 17:48:29
module TileValue =
    let private get mask shift (this: TileValue) =
        int (uint this.Values >>> shift) &&& mask // C# 的 >>>（无符号右移）需要在 F# 中转为 uint 再右移

    let private withValue value mask shift (this: TileValue) =
        TileValue((this.Values &&& ~~~(mask <<< shift)) ||| ((value &&& mask) <<< shift))

    let elevation (this: TileValue) = (get 31 0 this) - 15
    let withElevation value (this: TileValue) = withValue (value + 15) 31 0 this

    let getEdgeType (otherTile: TileValue) (this: TileValue) =
        HexMetrics.getEdgeType <| elevation otherTile <| elevation this

    let waterLevel (this: TileValue) = get 31 5 this

    let viewElevation (this: TileValue) =
        max <| elevation this <| waterLevel this

    let isUnderwater (this: TileValue) = waterLevel this > elevation this
    let withWaterLevel value (this: TileValue) = withValue value 31 5 this

    let streamBedY unitHeight (this: TileValue) =
        (float32 (elevation this) + HexMetrics.streamBedElevationOffset) * unitHeight

    let riverSurfaceY unitHeight (this: TileValue) =
        (float32 (elevation this) + HexMetrics.waterElevationOffset) * unitHeight

    let waterSurfaceY unitHeight (this: TileValue) =
        (float32 (waterLevel this) + HexMetrics.waterElevationOffset) * unitHeight

    let urbanLevel (this: TileValue) = get 3 10 this
    let withUrbanLevel value (this: TileValue) = withValue value 3 10 this
    let farmLevel (this: TileValue) = get 3 12 this
    let withFarmLevel value (this: TileValue) = withValue value 3 12 this
    let plantLevel (this: TileValue) = get 3 14 this
    let withPlantLevel value (this: TileValue) = withValue value 3 14 this
    let specialIndex (this: TileValue) = get 255 16 this
    let isSpecial (this: TileValue) = specialIndex this > 0
    let withSpecialIndex index (this: TileValue) = withValue index 255 16 this
    let terrainTypeIndex (this: TileValue) = get 255 24 this
    let withTerrainTypeIndex index (this: TileValue) = withValue index 255 24 this
