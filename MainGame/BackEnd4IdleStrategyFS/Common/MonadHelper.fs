namespace BackEnd4IdleStrategyFS.Common

open FSharpPlus
open FSharpPlus.Data

module MonadHelper =
    /// 返回一个泛型 State
    let stateReturn t = State(fun s -> t, s)

    /// State seq 折叠器
    let stateFolder (acc: State<'s, seq<'a>>) (next: State<'s, 'a>) =
        State.bind (fun results -> State.bind (fun result -> Seq.append results [ result ] |> stateReturn) next) acc

    /// State seq 折叠器（monad 实现）
    /// 好像 Seq.fold stateFolderM (State.Return Seq.empty) 直接就是对应 Seq.sequence
    /// 白写了……
    let stateFolderM (acc: State<'s, seq<'a>>) (next: State<'s, 'a>) =
        monad {
            let! results = acc
            let! result = next
            return Seq.append results [ result ]
        }

    /// 返回一个泛型 Reader
    let readerReturn t = Reader(fun _ -> t)
    
    /// Reader seq 折叠器
    let readerFolder (acc: Reader<'r, seq<'a>>) (next: Reader<'r, 'a>) =
        Reader.bind (fun results -> Reader.bind (fun result -> Seq.append results [ result ] |> readerReturn) next) acc

    /// Reader seq 折叠器（monad 实现）
    let readerFolderM (acc: Reader<'r, seq<'a>>) (next: Reader<'r, 'a>) =
        monad {
            let! results = acc
            let! result = next
            return Seq.append results [ result ]
        }
    
    /// 返回一个 Reader<State>
    let readerStateReturn t = Reader(fun _ -> State(fun s -> t, s))

    /// Reader<State> seq 折叠器
    let readerStateFolder (acc: Reader<'a, State<'s, seq<'b>>>) (next: Reader<'a, State<'s, 'b>>) =
        Reader(fun r ->
            let sAcc = acc |> Reader.run <| r
            let sNext = next |> Reader.run <| r

            State(fun s ->
                let accSeq, s' = sAcc |> State.run <| s
                let nextValue, s'' = sNext |> State.run <| s'
                Seq.append accSeq [ nextValue ], s''))

    // 返回一个 StateT<Reader>
    let stateTReaderReturn a = Reader(fun _ -> a) |> StateT.lift

    /// StateT<Reader> seq 折叠器 (StateT.Return 没有用，目前实现是报错的，注释掉)
    let stateTReaderFolder (acc: StateT<'s, Reader<'a, seq<'b> * 's>>) (next: StateT<'s, Reader<'a, 'b * 's>>) =
        StateT.bind
            (fun results -> StateT.bind (fun result -> Seq.append results [ result ] |> stateTReaderReturn) next)
            acc

    /// StateT<Reader> seq 折叠器（monad 实现）
    /// 有的情况直接 Seq.sequence 即可，类型推断不出来的时候还是得用 Seq.fold stateTReaderFolderM (StateT.Return Seq.empty)
    let stateTReaderFolderM (acc: StateT<'s, Reader<'a, seq<'b> * 's>>) (next: StateT<'s, Reader<'a, 'b * 's>>) =
        monad {
            let! results = acc
            let! result = next
            return Seq.append results [ result ]
        }

    /// 没有类型类，好像这里就提取不出这个通用逻辑？
    // let folderM acc next =
    //    monad {
    //        let! (results: seq<'a>) = acc
    //        let! (result: 'a) = next
    //        return Seq.append results [result]
    //    }

    /// 返回 unit 的 State
    let stateReturnUnit = State(fun s -> (), s)

    /// State unit 折叠器（monad 实现）
    let stateFolderUnitM (acc: State<'s, unit>) (next: State<'s, unit>) =
        monad {
            do! acc
            do! next
            return ()
        }

    let readerToState (reader: Reader<'env, 'a>) : State<'env, 'a> =
        State(fun env ->
            let result = Reader.run reader env
            (result, env))

    let stateTReaderToStateReader (stateTReader: StateT<'s, Reader<'r, 'a * 's>>) : Reader<'r, State<'s, 'a>> =
        Reader(fun r -> State(fun s -> StateT.run stateTReader s |> Reader.run <| r))
