namespace TO.FSharp.Domains.Enums.Tiles

open System

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-19 15:02:19
[<Flags>]
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