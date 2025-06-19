namespace TO.FSharp.Repos.Commands.Friflos

open Friflo.Engine.ECS
open TO.FSharp.Repos.Data.Commons
open TO.FSharp.Repos.Queries.Friflos

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-19 15:47:19
module EntityStoreCommand =
    let commitCommands (env: #IEntityStore) =
        let cb = env.EntityStore.GetCommandBuffer()
        cb.Playback()
        cb.Clear()

    let truncate<'T when 'T: (new: unit -> 'T) and 'T: struct and 'T :> IComponent and 'T :> System.ValueType>
        (env: #IEntityStore)
        =
        let cb = env.EntityStore.GetCommandBuffer()
        (EntityStoreQuery.query<'T> env).ForEachEntity(fun _ e -> cb.DeleteEntity(e.Id))
        cb.Playback()
        cb.Clear()
