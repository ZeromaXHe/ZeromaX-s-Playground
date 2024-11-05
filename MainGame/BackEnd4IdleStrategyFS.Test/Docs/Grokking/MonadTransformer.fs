module BackEnd4IdleStrategyFS.Test.Docs.Grokking.MonadTransformer
// https://dev.to/choc13/grokking-monad-transformers-3l3

// 场景
type UserId = UserId of string

type TransactionId = TransactionId of string

type CreditCard =
    { Number: string
      Expiry: string
      Cvv: string }

type User =
    { Id: UserId
      CreditCard: CreditCard option }

let lookupUser userId : Async<option<User>> = async.Return None

let chargeCard amount card : Async<option<TransactionId>> = async.Return None

let emailReceipt transactionId : Async<TransactionId> = async.Return(TransactionId "1")

// 发明一种单子
let bind f x =
    async {
        match! x with
        | Some y -> return! f y
        | None -> return None
    }

// 把自己从洞里抬出来
let hoist a = async { return a }

// 我们学到了什么？
open FSharpPlus
open FSharpPlus.Data

// type OptionT<'``Monad<option<'a>>``> = OptionT of '``Monad<option<'a>>``
//
// module OptionT =
//     let run (OptionT m) = m
//
//     let inline bind (f: 'a -> '``Monad<option<'b>>``) (OptionT m: OptionT<'``Monad<option<'a>>``>) =
//         monad {
//             match! m with
//             | Some value -> return! f value
//             | None -> return None
//         }
//         |> OptionT
//
//     let inline lift (x: '``Monad<'a>``) = x |> map Some |> OptionT
//
//     let inline hoist (x: 'a option) : OptionT<'``Monad<option<'a>>``> = x |> result |> OptionT

let chargeUser amount userId : Async<option<TransactionId>> =
    monad {
        let! user = (lookupUser userId) |> OptionT
        let! card = user.CreditCard |> OptionT.hoist
        let! transactionId = (chargeCard amount card) |> OptionT
        return! (emailReceipt transactionId) |> OptionT.lift
    }
    |> OptionT.run
