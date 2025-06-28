namespace TO.Repos.Queries.Friflos

open Friflo.Engine.ECS

type GetEntityById = int -> Entity
type GetEntityStore = unit -> EntityStore

[<Interface>]
type IEntityStoreQuery =
    abstract GetEntityById: GetEntityById
    abstract GetEntityStore: GetEntityStore

    abstract Query<'T when 'T: (new: unit -> 'T) and 'T: struct and 'T :> IComponent and 'T :> System.ValueType> :
        unit -> 'T ArchetypeQuery

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-25 09:13:25
module EntityStoreQuery =
    let getEntityById (store: EntityStore) : GetEntityById = store.GetEntityById
    // Query<'T> 之类的方法带泛型封装不了，所以暂且这样
    let getEntityStore (store: EntityStore) : GetEntityStore = fun () -> store

    let query<'T when 'T: (new: unit -> 'T) and 'T: struct and 'T :> IComponent and 'T :> System.ValueType>
        (store: EntityStore)
        =
        fun () -> store.Query<'T>()
