namespace TO.FSharp.Repos.Models.HexSpheres.Tiles

open Friflo.Engine.ECS

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-09 16:47:09
type TileFlagEnum =
    | Empty = 0
    | Road0 = 0b000001
    | Road1 = 0b000010
    | Road2 = 0b000100
    | Road3 = 0b001000
    | Road4 = 0b010000
    | Road5 = 0b100000
    | Roads = 0b111111
    | RiverIn0 = 0b000001_000000
    | RiverIn1 = 0b000010_000000
    | RiverIn2 = 0b000100_000000
    | RiverIn3 = 0b001000_000000
    | RiverIn4 = 0b010000_000000
    | RiverIn6 = 0b100000_000000
    | RiverIn = 0b111111_000000
    | RiverOut0 = 0b000001_000000_000000
    | RiverOut1 = 0b000010_000000_000000
    | RiverOut2 = 0b000100_000000_000000
    | RiverOut3 = 0b001000_000000_000000
    | RiverOut4 = 0b010000_000000_000000
    | RiverOut5 = 0b100000_000000_000000
    | RiverOut = 0b111111_000000_000000
    | River = 0b111111_111111_000000
    | Walled = 0b1_000000_000000_000000
    | Explored = 0b010_000000_000000_000000
    | Explorable = 0b100_000000_000000_000000

[<Struct>]
type TileFlag =
    interface IComponent
    val Flag: TileFlagEnum
    new(flag) = { Flag = flag }
    member this.HasAny(mask: TileFlagEnum) = this.Flag &&& mask <> TileFlagEnum.Empty
    member this.HasAll(mask: TileFlagEnum) = this.Flag &&& mask = mask
    member this.HasNone(mask: TileFlagEnum) = this.Flag &&& mask = TileFlagEnum.Empty
    member this.With(mask: TileFlagEnum) = this.Flag ||| mask
    member this.Without(mask: TileFlagEnum) = this.Flag &&& ~~~mask

    member this.Has(start: TileFlagEnum, direction: int) =
        int this.Flag &&& (int start <<< direction) <> 0

    member this.With(start: TileFlagEnum, direction: int) =
        this.Flag ||| enum<TileFlagEnum> (int start <<< direction)

    member this.Without(start: TileFlagEnum, direction: int) =
        this.Flag &&& ~~~(enum<TileFlagEnum> (int start <<< direction))

    member this.HasRoad(direction: int) = this.Has(TileFlagEnum.Road0, direction)

    member this.WithRoad(direction: int) =
        this.With(TileFlagEnum.Road0, direction)

    member this.WithoutRoad(direction: int) =
        this.Without(TileFlagEnum.Road0, direction)

    member this.HasRiverIn(direction: int) =
        this.Has(TileFlagEnum.RiverIn0, direction)

    member this.WithRiverIn(direction: int) =
        this.With(TileFlagEnum.RiverIn0, direction)

    member this.WithoutRiverIn(direction: int) =
        this.Without(TileFlagEnum.RiverIn0, direction)

    member this.HasRiverOut(direction: int) =
        this.Has(TileFlagEnum.RiverOut0, direction)

    member this.WithRiverOut(direction: int) =
        this.With(TileFlagEnum.RiverOut0, direction)

    member this.WithoutRiverOut(direction: int) =
        this.Without(TileFlagEnum.RiverOut0, direction)

    member this.HasRiver(direction: int) =
        this.HasRiverIn direction || this.HasRiverOut direction

    member this.ToDirection shift =
        match (int this.Flag >>> shift) &&& 0b111111 with
        | 0b000001 -> 0
        | 0b000010 -> 1
        | 0b000100 -> 2
        | 0b001000 -> 3
        | 0b010000 -> 4
        | 0b100000 -> 5
        | _ -> -1

    member this.Walled = this.HasAny TileFlagEnum.Walled
    member this.HasRoads = this.HasAny TileFlagEnum.Roads
    member this.HasRoadThroughEdge(direction: int) = this.HasRoad direction
    member this.HasRivers = this.HasAny TileFlagEnum.River
    member this.HasIncomingRiver = this.HasAny TileFlagEnum.RiverIn
    member this.HasOutgoingRiver = this.HasAny TileFlagEnum.RiverOut
    member this.HasRiverBeginOrEnd = this.HasIncomingRiver <> this.HasOutgoingRiver
    member this.HasIncomingRiverThoughEdge(direction: int) = this.HasRiverIn direction

    member this.HasRiverThroughEdge(direction: int) =
        this.HasRiverIn direction || this.HasRiverOut direction

    member this.RiverInDirection = this.ToDirection 6
    member this.RiverOutDirection = this.ToDirection 12
    member this.IsExplored = this.HasAll(TileFlagEnum.Explored ||| TileFlagEnum.Explorable)
    member this.IsExplorable = this.HasAll TileFlagEnum.Explorable
