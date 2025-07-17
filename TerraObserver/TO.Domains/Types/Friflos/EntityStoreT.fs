namespace TO.Domains.Types.Friflos

open Friflo.Engine.ECS

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-30 04:21:30
type GetEntityById = int -> Entity

[<Interface>]
type IEntityStoreQuery =
    abstract EntityStore: EntityStore
    abstract GetEntityById: GetEntityById

    abstract Query<'T when 'T: (new: unit -> 'T) and 'T: struct and 'T :> IComponent and 'T :> System.ValueType> :
        unit -> 'T ArchetypeQuery

type ExecuteInCommandBuffer = (CommandBuffer -> unit) -> unit

[<Interface>]
type IEntityStoreCommand =
    abstract ExecuteInCommandBuffer: ExecuteInCommandBuffer
