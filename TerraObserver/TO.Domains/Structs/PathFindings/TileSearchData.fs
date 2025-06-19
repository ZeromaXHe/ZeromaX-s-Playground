namespace TO.Domains.Structs.PathFindings

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-06 21:19:06
[<Struct>]
type TileSearchData =
    val mutable Distance: int
    val mutable NextWithSamePriority: int
    val mutable PathFrom: int
    val mutable Heuristic: int
    val mutable SearchPhase: int

    new(distance, nextWithSamePriority, pathFrom, heuristic, searchPhase) =
        { Distance = distance
          NextWithSamePriority = nextWithSamePriority
          PathFrom = pathFrom
          Heuristic = heuristic
          SearchPhase = searchPhase }

    member this.SearchPriority = this.Distance + this.Heuristic
