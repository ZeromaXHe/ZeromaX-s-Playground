namespace TO.Domains.Functions.Friflos

open Friflo.Engine.ECS
open TO.Domains.Types.Friflos

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-25 09:13:25
module EntityStoreQuery =
    let getEntityById (env: #IEntityStoreQuery) : GetEntityById = env.EntityStore.GetEntityById

    let query<'T, 'E
        when 'T: (new: unit -> 'T)
        and 'T: struct
        and 'T :> IComponent
        and 'T :> System.ValueType
        and 'E :> IEntityStoreQuery>
        (env: 'E)
        =
        fun () -> env.EntityStore.Query<'T>()

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-29 19:43:29
module EntityStoreCommand =
    let executeInCommandBuffer (env: #IEntityStoreQuery) : ExecuteInCommandBuffer =
        fun (execution: CommandBuffer -> unit) ->
            let cb = env.EntityStore.GetCommandBuffer()
            execution cb
            cb.Playback()
            cb.Clear()
