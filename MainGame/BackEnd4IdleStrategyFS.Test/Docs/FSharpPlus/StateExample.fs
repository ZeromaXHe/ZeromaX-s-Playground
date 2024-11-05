module BackEnd4IdleStrategyFS.Test.Docs.FSharpPlus.StateExample

open FSharpPlus
open FSharpPlus.Data

let rec playGame =
    function
    | [] ->
        monad {
            let! _, score = State.get
            return score
        }
    | x :: xs ->
        monad {
            let! on, score = State.get

            match x with
            | 'a' when on -> do! State.put (on, score + 1)
            | 'b' when on -> do! State.put (on, score - 1)
            | 'c' -> do! State.put (not on, score)
            | _ -> do! State.put (on, score)

            return! playGame xs
        }

let startState = (false, 0)
let moves = toList "abcaaacbbcabbab"
State.eval (playGame moves) startState |> ignore
let score, finalState = State.run (playGame moves) startState
