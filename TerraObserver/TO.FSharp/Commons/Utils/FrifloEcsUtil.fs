namespace TO.FSharp.Commons.Utils

open System.Collections.Generic
open Friflo.Engine.ECS

type ForEachLambda<'T when 'T: (new: unit -> 'T) and 'T: struct and 'T :> System.ValueType> = 'T -> Entity -> unit

[<Interface>]
type IWithLength<'T> =
    inherit IEnumerable<'T>
    abstract Length: int
    abstract Item: int -> 'T // 只读的索引属性的接口抽象形式，就是直接这样写！

type WithLengthEnumerator<'T>(withLength: 'T IWithLength) =
    let mutable currentIdx = -1

    interface 'T IEnumerator with
        override this.Current = withLength[currentIdx]
        override this.Current: obj = withLength[currentIdx] // IEnumerator 非泛型版本的 get 属性

        override this.MoveNext() =
            currentIdx <- currentIdx + 1
            currentIdx < withLength.Length

        override this.Reset() = currentIdx <- -1
        override this.Dispose() = ()

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

    let truncate<'T when 'T: (new: unit -> 'T) and 'T: struct and 'T :> System.ValueType> (query: 'T ArchetypeQuery) =
        query.ForEachEntity(fun _ tileEntity -> tileEntity.DeleteEntity())

    let commitCommands (store: EntityStore) =
        let cb = store.GetCommandBuffer()
        cb.Playback()
        cb.Clear()
