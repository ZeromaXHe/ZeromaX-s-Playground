namespace TO.Domains.Types.Chunks

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-29 20:59:29
[<Interface>]
type IHexGridChunk =
    inherit IChunk
    abstract Id: int with get, set
    abstract HideOutOfSight: unit -> unit
    abstract ApplyNewData: unit -> unit
    abstract ClearOldData: unit -> unit
