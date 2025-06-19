namespace TO.FSharp.Repos.Queries.Friflos

open Friflo.Engine.ECS
open TO.FSharp.Repos.Data.Commons

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-19 15:45:19
module EntityStoreQuery =
    let getEntityById (env: #IEntityStore) (id: int) = env.EntityStore.GetEntityById id

    let query<'T when 'T: (new: unit -> 'T) and 'T: struct and 'T :> IComponent and 'T :> System.ValueType>
        (env: #IEntityStore)
        =
        env.EntityStore.Query<'T>()

    let getCommandBuffer (env: #IEntityStore) = env.EntityStore.GetCommandBuffer()