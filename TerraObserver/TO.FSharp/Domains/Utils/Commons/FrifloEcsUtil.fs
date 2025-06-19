namespace TO.FSharp.Domains.Utils.Commons

open Friflo.Engine.ECS

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-27 09:48:27
module FrifloEcsUtil =
    /// 单泛型参数查询转为 Seq
    let toComponentSeq<'T when 'T: (new: unit -> 'T) and 'T: struct and 'T :> System.ValueType>
        (query: 'T ArchetypeQuery)
        =
        seq {
            for chunks in query.Chunks do
                let chunk, _ = chunks.Deconstruct()

                for i in 0 .. chunk.Length - 1 do
                    chunk.Span[i]
        }

    /// 双泛型参数查询转为 Seq
    let toComponentSeq2<'T, 'T2
        when 'T: (new: unit -> 'T)
        and 'T: struct
        and 'T :> System.ValueType
        and 'T2: (new: unit -> 'T2)
        and 'T2: struct
        and 'T2 :> System.ValueType>
        (query: ArchetypeQuery<'T, 'T2>)
        =
        seq {
            for chunks in query.Chunks do
                let chunk1, chunk2, _ = chunks.Deconstruct()

                for i in 0 .. chunk1.Length - 1 do
                    (chunk1.Span[i], chunk2.Span[i])
        }

    let tryHeadEntity<'T when 'T: (new: unit -> 'T) and 'T: struct and 'T :> System.ValueType>
        (query: 'T ArchetypeQuery)
        =
        let chunks = query.Chunks

        if chunks.Count = 0 then
            None
        else
            chunks
            |> Seq.tryHead
            |> Option.map (fun chunk ->
                let _, entities = chunk.Deconstruct()
                entities.EntityAt(0)) // 只返回第一个

    let commitCommands (store: EntityStore) =
        let cb = store.GetCommandBuffer()
        cb.Playback()
        cb.Clear()
