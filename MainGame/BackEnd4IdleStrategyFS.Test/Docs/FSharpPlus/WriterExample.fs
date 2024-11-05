module BackEnd4IdleStrategyFS.Test.Docs.FSharpPlus.WriterExample

open FSharpPlus
open FSharpPlus.Data

type LogEntry =
    { msg: string }

    static member create x = { msg = x }

let output x = Writer.tell [ LogEntry.create x ]

let calc =
    monad {
        do! output "I'm going to start a heavy computation" // start logging
        let y = sum [ 1..100_000 ]
        do! output (string y)
        do! output "The computation finished"
        return y // return the result of the computation
    }

let logs = Writer.exec calc
let y, logs' = Writer.run calc

// 使用常规列表会对性能产生一些影响，这就是为什么在这些场景中应该使用 DList
let output' x =
    Writer.tell <| DList.ofSeq [ LogEntry.create x ]

let calc' =
    monad {
        do! output' "I'm going to start a heavy computation" // start logging
        let y = sum [ 1..100_000 ]
        do! output' (string y)
        do! output' "The computation finished"
        return y // return the result of the computation
    }

let logs2 = Writer.exec calc'
let y', logs2' = Writer.run calc'
