namespace TO.Infras.Planets.Utils

open Friflo.Engine.ECS

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-27 09:48:27
module FrifloEcsUtil =
    let toComponentSeq<'T when 'T: (new: unit -> 'T) and 'T: struct and 'T :> System.ValueType>
        (query: ArchetypeQuery<'T>)
        =
        seq {
            for chunks in query.Chunks do
                let chunk, _ = chunks.Deconstruct()

                for i in 0 .. chunk.Length - 1 do
                    chunk.Span[i]
        }

    let tryHeadEntity<'T when 'T: (new: unit -> 'T) and 'T: struct and 'T :> System.ValueType>
        (query: ArchetypeQuery<'T>)
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

    let forEachEntity<'T when 'T: (new: unit -> 'T) and 'T: struct and 'T :> System.ValueType>
        (query: ArchetypeQuery<'T>)
        (forEachLambda: 'T -> Entity -> unit)
        =
        // 必须显式把 lambda 重写一遍（请忽略 IDE 的可简化提示），然后构造委托，不然报错：
        // This function value is being used to construct a delegate type whose signature includes a byref argument. You must use an explicit lambda expression taking 2 arguments.
        query.ForEachEntity
        <| ForEachEntity<'T>(fun comp entity -> forEachLambda comp entity)
