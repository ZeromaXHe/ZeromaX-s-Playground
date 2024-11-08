module BackEnd4IdleStrategyFS.Test.Docs.Grokking.Traversable
// https://dev.to/choc13/grokking-traversable-bla

open FSharpPlus

type ItemId = ItemId of string
type BasketId = BasketId of string
type CheckoutId = CheckoutId of string

// 场景
type BasketItem = { ItemId: ItemId; Quantity: float }

type Basket =
    { Id: BasketId; Items: BasketItem list }

type ReservedBasketItem = { ItemId: ItemId; Price: float }

type Checkout =
    { Id: CheckoutId
      BasketId: BasketId
      Price: float }

let reserveBasketItem (item: BasketItem) : ReservedBasketItem option = None

// 我们的首次实现
let reserveItems (items: BasketItem list) : option<list<ReservedBasketItem>> = None

let createCheckout basket =
    let reservedItems = basket.Items |> reserveItems

    reservedItems
    |> Option.map (fun items ->
        { Id = CheckoutId "some-checkout-id"
          BasketId = basket.Id
          Price = items |> List.sumBy (_.Price) })

// 反转器
let consOptions (head: 'a option) (tail: option<list<'a>>) : option<list<'a>> =
    match head, tail with
    | Some h, Some t -> Some(h :: t)
    | _ -> None

let rec invert (reservedItems: list<option<'a>>) : option<list<'a>> =
    match reservedItems with
    | head :: tail -> consOptions head (invert tail)
    | [] -> Some []

let reserveItems' (items: BasketItem list) : option<list<ReservedBasketItem>> =
    items |> List.map reserveBasketItem |> invert

// 用应用子清理
let rec invert' list =
    let cons head tail = head :: tail

    match list with
    | head :: tail -> (result cons) <*> head <*> (invert tail)
    | [] -> (result [])

// 在 sequence 上测试自己
module Result =
    let apply a f =
        match f, a with
        | Ok g, Ok x -> g x |> Ok
        | Error e, Ok _ -> e |> Error
        | Ok _, Error e -> e |> Error
        | Error e1, Error _ -> e1 |> Error

    let pure = Ok

let rec sequence list =
    let cons head tail = head :: tail

    match list with
    | head :: tail -> Result.pure cons |> Result.apply head |> Result.apply (sequence tail)
    | [] -> Result.pure []

// 还有更多的土地有待发现
let createCheckout' basket =
    let reservedItems = basket.Items |> List.map reserveBasketItem |> invert'

    reservedItems
    |> Option.map (fun items ->
        { Id = CheckoutId "some-checkout-id"
          BasketId = basket.Id
          Price = items |> Seq.sumBy (_.Price) })

let rec sequence' f list =
    let cons head tail = head :: tail

    match list with
    | head :: tail -> (Some cons) <*> (f head) <*> (sequence' f tail)
    | [] -> Some []

// 你刚刚发现了 traverse
let traverse = sequence'

let sequence'' list = traverse id list // 这个代码不两边加 list 编译过不了，类型推断不出来 'a = 'b option

let createCheckout'' basket =
    basket.Items
    |> traverse reserveBasketItem
    |> Option.map (fun items ->
        { Id = CheckoutId "some-checkout-id"
          BasketId = basket.Id
          Price = items |> Seq.sumBy _.Price })

// 在 `traverse` 中测试自己
let traverse' f opt =
    match opt with
    | Some x -> Result.pure Some |> Result.apply (f x)
    | None -> Result.pure None

module Identity =
    let apply a f = f a
    let pure f = f

let rec traverse'' list f =
    let cons head tail = head :: tail

    match list with
    | head :: tail ->
        Identity.pure cons
        |> Identity.apply (f head)
        |> Identity.apply (traverse'' tail f)
    | [] -> Identity.pure []
