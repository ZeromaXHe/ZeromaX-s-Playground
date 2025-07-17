namespace TO.Domains.Functions.HexSpheres.Components.Tiles

open TO.Domains.Types.HexSpheres.Components.Tiles

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-29 17:45:29
module TileFlag =
    let hasAny (mask: TileFlagEnum) (this: TileFlag) =
        this.Flag &&& mask <> TileFlagEnum.Empty

    let hasAll (mask: TileFlagEnum) (this: TileFlag) = this.Flag &&& mask = mask
    let hasNone (mask: TileFlagEnum) (this: TileFlag) = this.Flag &&& mask = TileFlagEnum.Empty
    let withMask (mask: TileFlagEnum) (this: TileFlag) = this.Flag ||| mask
    let withoutMask (mask: TileFlagEnum) (this: TileFlag) = this.Flag &&& ~~~mask

    let has (start: TileFlagEnum) (direction: int) (this: TileFlag) =
        int this.Flag &&& (int start <<< direction) <> 0

    let withStartAndDirection (start: TileFlagEnum) (direction: int) (this: TileFlag) =
        this.Flag ||| enum<TileFlagEnum> (int start <<< direction)

    let withoutStartAndDirection (start: TileFlagEnum) (direction: int) (this: TileFlag) =
        this.Flag &&& ~~~(enum<TileFlagEnum> (int start <<< direction))

    let hasRoad (direction: int) (this: TileFlag) = has TileFlagEnum.Road0 direction this

    let withRoad (direction: int) (this: TileFlag) =
        withStartAndDirection TileFlagEnum.Road0 direction this

    let withoutRoad (direction: int) (this: TileFlag) =
        withoutStartAndDirection TileFlagEnum.Road0 direction this

    let hasRiverIn (direction: int) (this: TileFlag) =
        has TileFlagEnum.RiverIn0 direction this

    let withRiverIn (direction: int) (this: TileFlag) =
        withStartAndDirection TileFlagEnum.RiverIn0 direction this

    let withoutRiverIn (direction: int) (this: TileFlag) =
        withoutStartAndDirection TileFlagEnum.RiverIn0 direction this

    let hasRiverOut (direction: int) (this: TileFlag) =
        has TileFlagEnum.RiverOut0 direction this

    let withRiverOut (direction: int) (this: TileFlag) =
        withStartAndDirection TileFlagEnum.RiverOut0 direction this

    let withoutRiverOut (direction: int) (this: TileFlag) =
        withoutStartAndDirection TileFlagEnum.RiverOut0 direction this

    let hasRiver (direction: int) (this: TileFlag) =
        hasRiverIn direction this || hasRiverOut direction this

    let toDirection shift (this: TileFlag) =
        match (int this.Flag >>> shift) &&& 0b111111 with
        | 0b000001 -> 0
        | 0b000010 -> 1
        | 0b000100 -> 2
        | 0b001000 -> 3
        | 0b010000 -> 4
        | 0b100000 -> 5
        | _ -> -1

    let walled (this: TileFlag) = hasAny TileFlagEnum.Walled this
    let hasRoads (this: TileFlag) = hasAny TileFlagEnum.Roads this
    let hasRivers (this: TileFlag) = hasAny TileFlagEnum.River this
    let hasIncomingRiver (this: TileFlag) = hasAny TileFlagEnum.RiverIn this
    let hasOutgoingRiver (this: TileFlag) = hasAny TileFlagEnum.RiverOut this

    let hasRiverBeginOrEnd (this: TileFlag) =
        hasIncomingRiver this <> hasOutgoingRiver this

    let riverInDirection (this: TileFlag) = toDirection 6 this
    let riverOutDirection (this: TileFlag) = toDirection 12 this

    let isExplored (this: TileFlag) =
        hasAll (TileFlagEnum.Explored ||| TileFlagEnum.Explorable) this

    let isExplorable (this: TileFlag) = hasAll TileFlagEnum.Explorable this
