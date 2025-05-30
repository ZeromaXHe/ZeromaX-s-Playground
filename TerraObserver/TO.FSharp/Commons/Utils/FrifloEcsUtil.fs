namespace TO.FSharp.Commons.Utils

open Friflo.Engine.ECS

type ForEachLambda<'T when 'T: (new: unit -> 'T) and 'T: struct and 'T :> System.ValueType> = 'T -> Entity -> unit

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-27 09:48:27
module FrifloEcsUtil =
    let toComponentSeq<'T when 'T: (new: unit -> 'T) and 'T: struct and 'T :> System.ValueType>
        (query: 'T ArchetypeQuery)
        =
        seq {
            for chunks in query.Chunks do
                let chunk, _ = chunks.Deconstruct()

                for i in 0 .. chunk.Length - 1 do
                    chunk.Span[i]
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

    let truncate<'T when 'T: (new: unit -> 'T) and 'T: struct and 'T :> System.ValueType> (query: 'T ArchetypeQuery) =
        query.ForEachEntity(fun _ tileEntity -> tileEntity.DeleteEntity())
