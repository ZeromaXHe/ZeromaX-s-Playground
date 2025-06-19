namespace TO.FSharp.Repos.Data.Commons

open Friflo.Engine.ECS

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-19 15:43:19
[<Interface>]
type IEntityStore =
    abstract EntityStore: EntityStore