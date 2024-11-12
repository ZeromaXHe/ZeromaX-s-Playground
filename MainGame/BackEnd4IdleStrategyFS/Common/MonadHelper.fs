namespace BackEnd4IdleStrategyFS.Common

open FSharpPlus
open FSharpPlus.Data

module MonadHelper =
    /// 返回一个泛型 State
    let stateReturn t = State(fun s -> t, s)

    /// State seq 折叠器
    /// 好像 Seq.fold stateFolderM (stateReturn Seq.empty) 直接就是对应 Seq.sequence
    /// 白写了……
    let stateFolder (acc: State<'s, seq<'a>>) (next: State<'s, 'a>) =
        monad {
            let! results = acc
            let! result = next
            return Seq.append results [ result ]
        }

    /// 返回一个泛型 Reader
    let readerReturn t = Reader(fun _ -> t)

    /// Reader seq 折叠器
    let readerFolder (acc: Reader<'r, 'a seq>) (next: Reader<'r, 'a>) =
        monad {
            let! results = acc
            let! result = next
            return Seq.append results [ result ]
        }

    /// 返回一个 Reader<State>
    let readerStateReturn t = Reader(fun _ -> State(fun s -> t, s))

    /// Reader<State> seq 折叠器
    let readerStateFolder (acc: Reader<'a, State<'s, 'b seq>>) (next: Reader<'a, State<'s, 'b>>) =
        Reader(fun r ->
            let sAcc = acc |> Reader.run <| r
            let sNext = next |> Reader.run <| r

            State(fun s ->
                let accSeq, s' = sAcc |> State.run <| s
                let nextValue, s'' = sNext |> State.run <| s'
                Seq.append accSeq [ nextValue ], s''))

    // 返回一个 StateT<Reader>
    let stateTReaderReturn a = Reader(fun _ -> a) |> StateT.lift

    /// StateT<Reader> seq 折叠器
    /// 有的情况直接 Seq.sequence 即可
    /// 类型推断不出来的时候还是得用 Seq.fold stateTReaderFolder (stateTReaderReturn Seq.empty)
    let stateTReaderFolder (acc: StateT<'s, Reader<'a, 'b seq * 's>>) (next: StateT<'s, Reader<'a, 'b * 's>>) =
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

    /// State unit 折叠器
    let stateFolderUnit (acc: State<'s, unit>) (next: State<'s, unit>) =
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
