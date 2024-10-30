# Grokking 函数式编程（11篇系列）

[Grokking Functional Programming (11 Part Series)](https://dev.to/choc13/series/12008)



# 1 Grokking Monads

https://dev.to/choc13/grokking-monads-in-f-3j7f


在这篇文章中，我们将通过一个工作示例独立“发现”它们来探索单子。

## 小 F# 简介（primer）

我们将使用 F#，但如果你以前没有使用过它，它应该很容易理解。你只需要理解以下几点。

- F# 具有 `option` 类型。它通过 `None` 值表示不存在或 `Some` 值表示存在。它通常用来代替 `null` 来表示缺失的值。

- `option` 类型的模式匹配如下：

  ```F#
  match anOptionalValue with
  | Some x -> // expression when the value exists
  | None -> // expression when the value doesn't exist.
  ```

- F# 有一个管道运算符，表示为 `|>`。它是一个中缀运算符，将左侧的值应用于右侧的函数。例如，如果 `toLower` 接收一个字符串并将其转换为小写，那么 `"ABC" |> toLower` 将输出 `"abc"`。

## 场景

假设我们正在编写一些需要向用户信用卡收费的代码。如果用户存在，并且他们的个人资料中保存了信用卡，我们可以对其进行收费，否则我们将不得不发出什么都没发生的信号。

F# 中的数据模型 

```F#
type CreditCard =
    { Number: string
      Expiry: string
      Cvv: string }

type User = 
    { Id: UserId
      CreditCard: CreditCard option }
```

*请注意，`User` 记录中的 `CreditCard` 字段是一个 `option`，因为它可能缺失。*

我们想用以下签名编写一个 `chargeUserCard` 函数

```F#
double -> UserId -> TransactionId option
```

它应该取一个类型为 `double` 的金额，即 `UserId`，如果用户的卡已成功充值，则返回 `Some TransactionId`，否则返回 `None`，表示该卡未充值。

## 我们的首次实现

让我们尝试实现 `chargeUserCard`。我们将首先定义几个辅助函数，我们将列出这些函数来查找用户并实际向卡收费。

```F#
let chargeCard (amount: double) (card: CreditCard): TransactionId option =
    // synchronously charges the card and returns 
    // Some TransactionId if successful, otherwise None

let lookupUser (userId: UserId): User option =
    // synchronously lookup a user that might not exist

let chargeUserCard (amount: double) (userId: UserId): TransactionId option =
    let user = lookupUser userId
    match user with
    | Some u ->
        match u.CreditCard with
        | Some cc -> chargeCard amount cc
        | None -> None
    | None -> None
```

完成了，但有点乱。双模式匹配并不是最清晰的代码。在这个简单的例子中，这可能是可以管理的，但如果有第三个或第四个嵌套匹配，就不会了。我们可以通过分解一些函数来解决这个问题，但还有另一个问题。请注意，在这两种 `None` 情况下，我们都返回 `None`。这看起来是无害的，因为默认值很简单，只重复两次，但我们应该能做得更好。

我们真正想要的是能够说，“如果在任何时候我们因为缺少一些数据而无法继续，那么停止并返回 `None`”。

## 我们期望的实现

让我们想象一下，数据总是存在的，我们别无选择。让我们称之为 `chargeUserCardSafe`，它看起来像这样。

```F#
let chargeUserCardSafe (amount: double) (userId: UserId): TransactionId =
    let user = lookupUser userId
    let creditCard = u.CreditCard
    chargeCard amount creditCard
```

*请注意，它现在如何返回 `TransactionId` 而不是 `TransactionId option`，因为它永远不会失败。*

如果我们能编写这样的代码，即使在缺少数据的情况下，也会很棒。为了实现这一点，我们需要在每一行之间放置一些东西，使类型对齐并粘合在一起。

## 重构以实现更清洁的实现

那块胶水应该怎么用？好吧，如果上一步中的值为 `None`，它应该终止计算，否则它应该从 `Some` 中解包该值并将其提供给下一行。实际上，进行我们上面第一次写的模式匹配。

那么，让我们看看能否排除模式匹配。首先，我们将以流水线的方式重写不能失败的函数，这样我们就可以更容易地在后面的步骤之间注入新函数。

```F#
// This helper is just here so we can easily chain all the steps
let getCreditCard (user: User): CreditCard option =
    u.CreditCard

let chargeUserCardSafe (amount: double) (userId: UserId): TransactionId = 
    userId 
    |> lookupUser 
    |> getCreditCard 
    |> chargeCard amount
```

我们在这里所做的只是将其转化为一系列由管道运算符组成的步骤。

现在，如果我们允许 `lookupUser` 和 `lookupCreditCard` 再次返回 `option`，那么它将不再编译。问题是我们不能写

```F#
userId |> lookupUser |> getCreditCard
```

因为 `lookupUser` 返回 `User option`，我们正试图将其管道化到一个需要普通 `User` 的函数中。

因此，我们面临两种选择来编译它。

1. 编写一个类型为 `User option -> User` 的函数，展开该选项，以便可以通过管道传输。这意味着忽略 `None` 情况会丢弃一些信息。命令式程序员可以通过抛出异常来解决这个问题。但是函数式编程应该给我们安全，所以我们不想在这里这样做。
2. 转换管道右侧的函数，使其可以接受 `User option`，而不仅仅是 `User`。所以我们需要做的是编写一个高阶函数。也就是说，将一个函数作为输入并将其转换为另一个函数的东西。

我们知道这个高阶函数的类型应该是 `(User -> CreditCard option) -> (User option -> CreditCard option)`。
所以，让我们按照类型来写。我们将称之为 `liftGetCreditCard`，因为它“提升”了 `getCreditCard` 函数，使其能够处理 `option` 输入而不是普通输入。

```F#
let liftGetCreditCard getCreditCard (user: User option): CreditCard option =
    match user with
    | Some u -> u |> getCreditCard
    | None -> None
```

很好，现在我们离我们想要的 `chargeUserCard` 函数越来越近了。现在变成了。

```F#
let chargeUserCard (amount: double) (userId: UserId): TransactionId option = 
    userId 
    |> lookupUser 
    |> liftGetCreditCard getCreditCard 
    |> chargeCard double
```

通过将 `getCreditCard` 部分应用于 `liftGetCreditCard`，我们创建了一个函数，其签名为 `User option -> CreditCard option`，这正是我们想要的。

不完全是，我们现在也有同样的问题，只是在链条的下游。`chargeCard` 期待 `CreditCard`，但我们正试图为其提供 `CreditCard option`。没问题，让我们再次应用同样的技巧。

```F#
let liftGetCreditCard getCreditCard (user: User option): CreditCard option =
    match user with
    | Some u -> u |> getCreditCard
    | None -> None

let liftChargeCard chargeCard (card: CreditCard option): TransactionId option =
    match card with
    | Some cc -> cc |> chargeCard
    | None -> None

let chargeUserCard (amount: double) (userId: UserId): TransactionId option = 
    userId 
    |> lookupUser 
    |> liftGetCreditCard getCreditCard 
    |> liftChargeCard (chargeCard amount)
```

## 即将被发现🗺

注意这两个 `lift...` 函数非常相似。还要注意它们如何不太依赖于第一个参数的类型。只要它是一个从选项中包含的值到另一个可选值的函数。让我们看看我们是否可以编写一个版本来满足这两个要求，我们可以通过将第一个参数重命名为 `f`（指 function）并删除大部分类型提示来实现，因为 F# 将为我们推断泛型。

```F#
let lift f x =
    match x with
    | Some y -> y |> f
    | None -> None
```

F# 为 `lift` 推断的类型是 `('a->'b option) -> ('a option -> 'b option)`。其中 `'a` 和 `'b` 是泛型类型。这相当冗长和抽象，但让我们把它和上面更具体的 `liftGetCreditCard` 签名放在一起。

```F#
(User -> CreditCard option) -> (User option -> CreditCard option)

('a -> 'b option) -> ('a option -> 'b option`)
```

具体的 `User` 类型已被泛型类型 `'a` 替换，具体的 `CreditCard` 类型已被泛化类型 `'b` 替换。这是因为 `lift` 不关心 `option` 框内的内容，它只是说“给我一些函数 'f'，如果该值存在，我会将其应用于 'x' 中包含的值。”唯一的约束是函数 `f` 接受 `option` 内的类型。

好的，现在我们可以进一步清理 `chargeUserCard` 了。

```F#
let chargeUserCard (amount: double) (userId: UserId): TransactionId option = 
    userId 
    |> lookupUser 
    |> lift getCreditCard 
    |> lift (chargeCard amount)
```

现在，它看起来真的很接近没有可选数据的版本。最后一件事，让我们将 `lift` 重命名为 `andThen`，因为直观上我们可以将该函数视为在数据存在时继续计算。所以我们可以说，“做点什么，如果成功了，就做另一件事”。

```F#
let chargeUserCard (amount: double) (userId: UserId): TransactionId option = 
    userId 
    |> lookupUser 
    |> andThen getCreditCard 
    |> andThen (chargeCard amount)
```

这读起来很好，也很好地解释了我们对这个函数的看法。我们查找用户，然后如果他们存在，我们就会得到他们的信用卡信息，最后如果存在，我们会从他们的卡中扣款。

## 你刚刚发现了单子👏

我们编写的 `lift` / `andThen` 函数使 `option` 值成为单子。通常，当谈论单子时，它被称为 `bind`，但这并不重要。重要的是，你可以看到我们为什么定义它以及它是如何工作的。单子只是定义了这种“然后可以”函数类型的一类东西^1^。

> [1] 范畴学家们，请原谅我

## 嘿，我认得你！🕵️‍♀️

我把 `lift` 改名为 `andThen` 还有另一个原因。如果你是一名 JavaScript 开发人员，那么对于带有 `then` 方法的 `Promise` 来说，这应该很熟悉。在这种情况下，你可能已经尝过（grokked）单子了。`Promise` 也是一个单子。与 `option` 完全一样，它有一个 `then`，它接受另一个函数作为输入，如果成功，则根据 `Promise` 的结果调用它。

## 单子只是“能够然后（then-able）”的容器📦

另一个直观了解单子的好方法是将它们视为值容器。`option` 是一个包含值或为空的容器。`Promise` 是一个容器，如果它成功返回，则“承诺”保存某些异步计算的值。

当然，还有其他方法，比如 `List`（它保存了许多计算的值）和 `Result`，如果计算成功，则包含一个值，如果计算失败，则包含错误。对于这些容器中的每一个，我们都可以定义一个 `andThen` 函数，该函数定义了如何将需要容器内的东西的函数应用于包裹在容器中的东西。

## 在野外发现 Monads

如果你发现自己使用的函数接受一些简单的输入，比如 `int`、`string` 或 `User`，并且执行一些副作用，从而返回类似 `option`、`Promise` 或 `Result` 的东西，那么可能有一个 monad 潜伏在周围。特别是如果你有几个这样的函数，你想在链中按顺序调用。

## 我们学到了什么？👨‍🎓

我们了解到，monad 只是为其定义了“then-able”函数的容器类型，该函数名为 `bind`。我们可以使用此函数将操作链接在一起，这些操作本机从未包装的值转换为不同类型的包装值。

这很有用，因为这是一种常见的模式，适用于许多不同的类型，通过提取这个 `bind` 函数，我们可以在处理这些类型时消除很多问题。单子只是给遵循这种模式的事物起的一个名字，就像理查德·费曼说的那样，名字并不构成知识。

## 下次

如果你还记得我们在开始重构之旅时为自己设定的最初目标，你就会知道我们想写这样的东西

```F#
let chargeUserCard (amount: double) (userId: UserId): TransactionId option =
    let user = lookupUser userId
    let creditCard = u.CreditCard
    chargeCard amount creditCard
```

但在处理可选值时，它仍然有效。我们在这里没有完全实现这个目标。在下一篇文章中，我们将看到如何使用 F# 的计算表达式来恢复这种更“命令式”的编程风格，即使在使用 monad 时也是如此。



# 2 Grokking Monads，命令式

https://dev.to/choc13/grokking-monads-imperatively-394a

之前，在 Grokking Monads 中，我们通过一个工作示例发现了 Monads。到最后，我们已经为 `option` 类型创建了 `andThen` 函数形式的基本机制，但我们还没有完全达到最终目标。我们希望以与不必处理 `option` 值时相同的方式编写代码。我们想用一种更“命令式”的风格来写。在这篇文章中，我们将看到如何用 F# 的计算表达式来实现这一点，同时加深我们对单子的直觉。

## 扼要重述

让我们快速回顾一下领域模型

```F#
type CreditCard =
    { Number: string
      Expiry: string
      Cvv: string }

type User = 
    { Id: UserId
      CreditCard: CreditCard option }
```

我们想用这个签名写 `chargeUserCard`

```F#
UserId -> TransactionId option
```

如果我们不必处理 `option` 值，那么我们可以用“命令式”风格编写它。

```F#
let chargeUserCardSafe (userId: UserId): TransactionId =
    let user = lookupUser userId
    let creditCard = u.CreditCard
    chargeCard creditCard
```

我们的目标是编写这样的东西，即使我们必须在计算过程中处理可选值。

我们已经把以下 `andThen` 函数分解出来了

```F#
let andThen f x =
    match x with
    | Some y -> y |> f
    | None -> None
```

我们最多只能使用以下版本的 `chargeUserCard`

```F#
let chargeUserCard (amount: double) (userId: UserId): TransactionId option = 
    userId 
    |> lookupUser 
    |> andThen getCreditCard 
    |> andThen (chargeCard amount)
```

## 有什么问题吗🤷‍♂️

你可能会想知道这里的问题是什么。`chargeUserCard` 的最终实现是可读的。它非常清楚地描述了我们的意图，我们消除了重复的代码。那么，这只是一个美学案例吗？

为了理解为什么使用“命令式”风格的能力超越了简单的美学，让我们引入另一个要求。这将突出我们目前的执行情况，并显示其弱点。

用户现在必须在其个人资料中设置支出限制。为了向后兼容，该限制被建模为一个 `option`。如果存在限制，我们必须检查支出是否低于限制，如果用户尚未设置限制，我们终止计算并返回 `None`。

此限制存储在 `User` 模型中，如下所示。

```F#
type User =
    { Id: UserId
      CreditCard: CreditCard option
      Limit: double option }
```

让我们首先按照以下思路实现一个 `getLimit` 函数（就像我们对 `getCreditCard` 所做的那样）。

```F#
let getLimit (user: User): double option =
    user.Limit
```

那么，我们如何更新 `chargeUserCard` 以考虑支出限额呢？我们需要执行以下步骤：

1. 根据用户的 id 查找用户
2. 如果用户存在，请查找信用卡
3. 如果用户存在，请查找限制
4. 如果存在限额和信用卡，则向金额低于限额的卡收费

让我们首先写这个，就好像没有 `option` 值以“命令式”风格为我们提供目标一样。

```F#
let chargeUserCardSafe (amount: double) (userId: UserId) =
    let user = lookupUser userId
    let card = getCreditCard user
    let limit = getLimit user
    if amount <= limit then
        chargeCard amount card
    else 
        None
```

让我们重新引入 `option` 值，并使用 `andThen` 将其天真地转换为管道形式。

```F#
let chargeUserCard (amount: double) (userId: UserId): TransactionId option = 
    userId 
    |> lookupUser 
    |> andThen getCreditCard 
    |> andThen getLimit
    |> andThen 
        (fun limit -> 
            if amount <= limit then 
                chargeCard amount ??
            else
                None)
```

这将无法编译，原因有二：

1. 我们不能在 `getCreditCard` 之后写 `andThen getLimit`，因为此时我们可以访问 `CreditCard`，但我们需要将 `User` 传递给 `getLimit`。
2. 在我们想要调用 `chargeCard` 的时候，我们无法访问 `CreditCard` 值。

## 打破枷锁🔗

似乎不再有一个连续的数据流。这是因为我们需要使用 `user` 来查找 `CreditCard` 和 `limit`，我们需要这两样东西都存在，然后才能对卡进行收费。

经过一些挠头，我们可以找到一种方法，使用 `andThen` 以管道风格编写这个函数，但要小心，它会变得很麻烦！

```F#
let chargeUserCard (amount: double) (userId: UserId) : TransactionId option =
    let applyDiscount discount = amount * (1. - discount)

    userId
    |> lookupUser
    |> andThen
        (fun user ->
            user
            |> getCreditCard
            |> andThen
                (fun cc ->
                    user
                    |> getLimit
                    |> Option.map (fun limit -> {| Card = cc; Limit = limit |})))
    |> andThen
        (fun details ->
            if amount <= details.Limit then
                chargeCard amount details.Card
            else
                None)
```

哇！情况迅速升级！

如果你还没有完全理解这个实现，不要担心。这就是重点，它变得很难理解，我们需要找到一种方法来驯服它。

当这种情况发生时，它会使链接变得繁琐。为了继续使用 `|>` 运算符，我们需要积累越来越多的状态，以便我们最终可以在链的末尾使用所有状态。这就是我们在表达式的深度嵌套部分使用匿名记录的原因。

我们可能会开始怀疑函数式编程是否真的那么棒。在过去的命令式日子里，我们只是将这些值赋给一个变量，然后在需要时在方法末尾引用它们。

## 吃我们的蛋糕🍰

幸运的是，有办法摆脱这种混乱。

这一次，我们将发明一些新的语法，使其与 `option` 值一起工作。我们将定义 `let!`。它类似于 `let`，但它不是将名称绑定到表达式，而是将名称绑定至 `option`（如果存在）中的值。如果该值不存在，则它将立即以 `None` 值终止函数。这与我们之前发明的 `andThen` 函数的行为完全相同，只是现在它允许我们命名结果。

使用这种新语法，`chargeUserCard` 很简单

```F#
let chargeUserCard (amount: double) (userId: UserId) =
    let! user = lookupUser userId
    let! card = getCreditCard user
    let! limit = getLimit user
    if amount <= limit then
        chargeCard amount card
    else 
        None
```

与没有 `option` 的版本几乎没有任何区别。“太好了”，我听到你说，“但你不能只是发明新的语法！”。幸运的是，我们不必这样做。F# 提供 `let!` 作为名为计算表达式的功能的一部分，它开箱即用。

## 计算表达式 != 魔术🪄

F#并不完全是魔法，我们必须教它如何 `let!` 应该为给定的单子行为。我们必须定义一个新的计算表达式。

我不会在这里详细介绍如何做到这一点，[F# 文档](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/computation-expressions#creating-a-new-type-of-computation-expression)是一个很好的起点。与我们相关的是，F# 要求我们使用 `Bind` 方法创建一个类型。我们已经知道如何编写 `Bind`，因为我们发现了它并将其命名为 `andThen`。`option` 的计算表达式生成器最终看起来像这样。

```F#
let andThen f x =
    match x with
    | Some y -> y |> f
    | None -> None

type OptionBuilder() =
    member _.Bind(x, f) = andThen f x
    member _.Return(x) = Some x
    member _.ReturnFrom(x) = x
```

我们还需要定义 `Return`，它允许我们通过将一个普通值包裹在 `Some` 中而从计算表达式中返回；和 `ReturnFrom`，允许我们返回生成 `option` 的表达式的结果。

*`ReturnFrom` 似乎是多余的，因为它太简单了。然而，在其他计算表达式中，我们可能需要更复杂的行为。通过使其可扩展，F# 在这种情况下以牺牲一些琐碎的样板为代价授予了我们这种能力。*

计算表达式就绪后，`chargeUserCard` 的最终实现变为

```F#
let chargeUserCard (amount: double) (userId: UserId) =
    option {
        let! user = lookupUser userId
        let! card = getCreditCard user
        let! limit = getLimit user

        return!
            if amount <= limit then
                chargeCard amount card
            else
                None
    }
```

很漂亮！我们只需要将正文包裹在 `option {}` 中，以表明我们想使用刚才定义的选项计算表达式。我们也必须使用 `return!` 在最后一行告诉它返回该表达式生成的 `option`值。

## 测试驱动计算表达式🚘

为了深入了解我们刚才定义的计算表达式，并证明它确实如我们所愿，让我们在 F# repl 中运行一些测试。

```F#
> option {
-     let! x = None
-     let! y = None
-     return x + y 
- };;
val it : int option = None
```

因此，当 `x` 和 `y` 都为 `None` 时，结果为 `None`。当只有 `x` 或 `y` 为 `None` 时呢？

```F#
> option {
-     let! x = Some 1
-     let! y = None
-     return x + y 
- };;
val it : int option = None

> option {
-     let! x = None
-     let! y = Some 2
-     return x + y 
- };;
val it : int option = None
```

三对三！我们只需要确保当 `x` 和 `y` 都有值时，它确实返回了一个包含加法的 `Some`。

```F#
> option {
-     let! x = Some 1
-     let! y = Some 2
-     return x + y 
- };;
val it : int option = Some 3
```

满分！🎉

## 更多单子直觉

这种“命令式”风格可能看起来很熟悉。如果这是一个 `async` 计算，那么 `let!` 就像 `await`。人们喜欢 `async/await` 的原因，尤其是那些记得嵌套 promise 回调地狱时代的人，是因为它允许我们编写程序，就像它们不是异步的一样。它消除了与必须处理结果不会立即可用且可能失败的事实相关的所有噪音。

F# 的计算表达式允许我们使用所有 monads 风格，而不仅仅是 `async`。这真的很强大，因为我们现在可以以易于理解的“命令式”风格编写代码，但没有完全命令式编程的可变状态和其他副作用。

## 我必须自己滚吗🗞

[F# 核心库](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/computation-expressions#built-in-computation-expressions)包括一些用于序列、异步工作流和 LINQ 查询表达式的内置计算表达式。开源库中还实现了许多有用的库。[FSharpPlus](https://fsprojects.github.io/FSharpPlus//computation-expressions.html) 甚至更进一步，创建了一个适用于多种单子类型的 `monad` 计算表达式。

## 我们学到了什么🧑‍🎓

我们已经看到，虽然 `andThen` 函数是链接单子计算的底层机制，但当我们想要执行的操作没有如此明显的线性序列时，直接使用它会很快变得很麻烦。通过利用 F# 的计算表达式，我们可以隐藏这种“管道”，而是像不处理一元计算一样编写代码。这正是 `async/await` 所做的，但只是狭义的 `Tasks` 或 `Promises`。因此，如果你已经摸索了（grokked） `async/await`，那么你就很好地掌握了 monad 和计算表达式。



# 3 Grokking Functors

https://dev.to/choc13/grokking-functors-bla

在这篇文章中，我们将通过一个工作示例“发现” functor 来探索它们。

## 小 F# 简介（primer）

如果你对 F# 有基本的了解，就跳过这个步骤。
如果你以前没有使用过 F#，那么它应该很容易理解。你只需要理解以下几点。

- F# 具有 `option` 类型。它通过 `None` 值表示不存在或 `Some` 值表示存在。它通常用来代替 `null` 来表示缺失的值。

- `option` 类型的模式匹配如下：

  ```F#
  match anOptionalValue with
  | Some x -> // expression when the value exists
  | None -> // expression when the value doesn't exist.
  ```

- F# 有一个管道运算符，表示为 `|>`。它是一个中缀运算符，将左侧的值应用于右侧的函数。例如，如果 `toLower` 接收一个字符串并将其转换为小写，那么 `"ABC" |> toLower` 将输出 `"abc"`。

## 场景

假设我们被要求编写一个函数来打印用户的信用卡详细信息。数据模型很简单，我们有一个 `CreditCard` 类型和一个 `User` 类型。

```F#
type CreditCard =
    { Number: string
      Expiry: string
      Cvv: string }

type User =
    { Id: UserId
      CreditCard: CreditCard }
```

## 我们的首次实现

我们想要一个函数，它接受一个 `User` 并返回其 `CreditCard` 详细信息的 `string` 表示。这相当容易实现。

```F#
let printUserCreditCard (user: User): string =
    let creditCard = user.CreditCard

    $"Number: {creditCard.Number}
    Exiry: {creditCard.Expiry}
    Cvv: {creditCard.Cvv}"
```

我们甚至可以通过编写 `getCreditCard` 函数和 `printCreditCard` 函数来解决这个问题，然后我们可以在想要打印用户信用卡详细信息时将它们组合起来。

```F#
let getCreditCard (user: User) : CreditCard = user.CreditCard

let printCreditCard (card: CreditCard) : string =
    $"Number: {card.Number}
    Exiry: {card.Expiry}
    Cvv: {card.Cvv}"

let printUserCreditCard (user: User) : string =
    user
    |> getCreditCard 
    |> printCreditCard
```

美丽的！👌

## 一个转折🌪

一切都很好，直到我们意识到我们首先需要从数据库中按用户的 id 查找用户。幸运的是，已经为此实现了一个函数。

```F#
let lookupUser (id: UserId): User option =
    // read user from DB, if they exist return Some, else None
```

不幸的是，它返回的是 `User option` 而不是 `User`，所以我们不能只写

```F#
userId
|> lookupUser
|> getCreditCard
|> printCreditCard
```

因为 `getCreditCard` 需要一个 `User`，而不是一个 `option User`。

让我们看看是否可以转换 `getCreditCard`，使其可以接受 `option` 作为输入，而无需更改原始的 `getCreditCard` 函数本身。为此，我们需要编写一个函数来包装它。让我们称之为 `liftGetCreditCard`，因为我们可以把它看作是“提升” `getCreditCard` 函数来处理选项输入。

刚开始写起来可能有点棘手，但我们知道 `liftGetCreditCard` 有两个输入。第一个是 `getCreditCard` 函数，第二个是 `User option`。我们也知道我们需要返回 `CreditCard option`。因此，签名应该是 `(User -> CreditCard) -> User option -> CreditCard option`。

通过遵循类型，实际上只需要做一件事，尝试通过 `option` 的模式匹配将函数应用于 `User` 值。如果用户不存在，则无法应用该函数，因此必须返回 `None`。

```F#
let liftGetCreditCard getCreditCard (user: User option): CreditCard option =
    match user with
    | Some u -> u |> getCreditCard |> Some
    | None -> None
```

请注意，在 `Some` 情况下，我们必须将 `getCreditCard` 的输出包装在 `Some` 中。这是因为我们必须确保两个分支机构返回相同的类型，而唯一的方法就是让它们返回 `CreditCard option`。

有了这个，我们的管道现在

```F#
userId
|> lookupUser
|> liftGetCreditCard getCreditCard
|> printCreditCard
```

通过将 `getCreditCard` 部分应用于 `liftGetCreditCard`，我们创建了一个函数，其签名为 `User option -> CreditCard option`，这就是我们想要的。

差不多，除了最后一行，我们遇到了和以前一样的问题。`printCreditCard` 只知道如何处理 `CreditCard` 值，而不知道 `CreditCard option` 值。所以，让我们再次应用同样的技巧，写 `liftPrintCreditCard`。

```F#
let liftPrintCreditCard printCreditCard (card: CreditCard option): CreditCard option =
    match card with
    | Some cc -> cc |> printCreditCard |> Some
    | None -> None
```

我们的管道现在

```F#
userId
|> lookupUser
|> liftGetCreditCard getCreditCard
|> liftPrintCreditCard printCreditCard
```

## 我看到的是一个函子吗🔭

如果我们缩小一点，我们可能会注意到这两个 `lift...` 函数非常相似。它们都执行相同的基本任务，即解包 `option`，将函数应用于值（如果存在），然后将此值打包为新 `option`。它们并不真正依赖于 `option` 内部的内容或函数是什么。唯一的约束是函数接受 `option` 内部可能存在的值作为输入。

让我们看看我们是否可以编写一个名为 `lift` 的单一版本，为任何有效的函数对定义这种行为，我们将调用 `f` 和一个 `option`，我们将称之为 `x`。我们现在可以删除类型定义，让 F# 为我们推断它们。

```F#
let lift f x =
    match x with
    | Some y -> y |> f |> Some
    | None -> None
```

整洁，但可能有点抽象。让我们看看推断的类型告诉我们什么。F# 推断它具有类型  `('a -> 'b) -> 'a option -> 'b option`。其中 `'a` 和 `'b` 是泛型类型。让我们将其与 `liftGetCreditCard` 并排放置，以帮助我们使其更加具体。

```F#
(User -> CreditCard) -> User option -> CreditCard option

('a -> 'b) -> 'a option -> 'b option
```

具体的 `User` 被泛型类型 `'a` 替换，具体的 `CreditCard` 类型被泛型类型 `'b` 替换。这是因为 `lift` 不在乎 `option` 框内有什么，它只是说“给我一些函数 `f`，如果存在，我会将其应用于 `x` 中包含的值，并将其重新打包为新的选项”。

一个更直观的 `lift` 名称是 `map`，因为我们只是映射 `option` 内的内容。

因此，有了这个新的 `map` 函数，让我们在管道中使用它。

```F#
userId
|> lookupUser
|> map getCreditCard
|> map printCreditCard
```

不错！这与我们在选择重磅炸弹之前编写的版本几乎相同。因此，代码仍然非常可读。

## 你刚刚发现了函子👏

我们编写的 `map` 函数使 `option` 值成为函子。函子只是一类“可映射”的东西。幸运的是，F# 已经在 `Option` 模块中定义了 `map`，所以我们实际上可以使用它来编写代码

```F#
userId
|> lookupUser
|> Option.map getCreditCard
|> Option.map printCreditCard
```

## 函子只是“可映射”的容器📦

另一个直观了解函数子的好方法是将它们视为值容器。对于每种类型的容器，我们只需要定义一种能够映射或转换其内容的方法。

我们刚刚发现了如何为 `option` 做到这一点，但我们也可以将更多的容器转换为函子。`Result` 是一个具有值或错误的容器，我们希望能够映射该值，而不管错误是什么。

最常见的容器是 `List` 或 `Array`。大多数程序员都遇到过这样的情况，他们以前需要转换列表中的所有元素。如果你曾经在 C# 中使用过 `Select`，或者在 JavaScript、Java 等中使用过 `map`，那么即使你没有意识到这一点，你可能已经摸索过（grokked） functor 了。

## 考验自己🧑‍🏫

看看你是否可以为 `Result<'a, 'b>` 和 `List<'a>` 类型编写 `map`。答案如下，在你先尝试之前不要偷看！

结果解决方案

```F#
let map f x =
    match x with
    | Ok y -> y |> f |> Ok
    | Error e -> Error e
```

这个几乎与 `option` 相同，因为我们只将函数应用于 `Ok` 情况的值，否则我们只会传播 `Error`。


List 解决方案

```F#
let rec map f x =
    match x with
    | y:ys -> f y :: map f ys
    | [] -> []
```

这比其他方法有点棘手，但基本思想是一样的。如果列表中有一些项目，我们从列表的头部选取，将该函数应用于头部，并将其添加到通过映射到此列表的尾部而创建的新列表的前面。如果列表为空，我们只返回另一个空列表。每次我们再次调用 `map` 时，通过将列表大小减小一个，我们保证最终我们达到了终止递归的空列表的基本情况。

## 我们学到了什么🧑‍🎓

我们了解到，函子只是为其定义了 `map` 函数的容器类型。我们可以使用此函数转换容器内的内容。这允许我们将使用常规值的函数链接在一起，即使这些值被打包在这些容器类型之一中。

这很有用，因为这种模式发生在许多不同的情况下，通过提取 `map` 函数，我们可以消除很多样板，否则我们必须通过不断的模式匹配来做到这一点。

## 更进一步

如果你喜欢 grokking functors，你会爱上 [Grokking Monads](https://dev.to/choc13/grokking-monads-in-f-3j7f) 的。在那里，我们遵循类似的配方，发现了一种不同类型的函数，这也是非常有用的。



# 4 Grokking Applicatives

https://dev.to/choc13/grokking-applicatives-44o1

应用子（Applicatives）可能是单子的不太知名的兄弟，但同样重要，可以解决一个不同但相关的问题。一旦你发现了它们，你会发现它们是一个有用的工具，用于编写我们不关心计算顺序的代码。

在这篇文章中，我们将通过一个工作示例“发现”应用子来探索它们。

## 小 F# 简介

如果你对 F# 有基本的了解，就跳过这个步骤。

如果你以前没有使用过 F#，那么遵循它应该很容易，你只需要理解以下几点：

- F# 具有 `Result<T, E>` 类型。它表示可能失败的计算结果。它有一个 `Ok` case 构造函数，其中包含一个 `T` 类型的值，还有一个 `Error` case 构造函数，它包含一个 `E` 类型的错误值。

- 我们可以对 `Result` 进行模式匹配，如下所示：

  ```F#
  match aResult with
  | Ok x -> // expression based on the good value x
  | Error e -> // expression based on the error value e
  ```

## 场景

假设我们被要求编写一个验证信用卡的函数，其建模方式如下。

```F#
type CreditCard =
    { Number: string
      Expiry: string
      Cvv: string }
```

我们需要实现的功能是

```F#
let validateCreditCard (creditCard: CreditCard): Result<CreditCard, string>
```

令人高兴的是，其他人已经编写了一些用于验证信用卡号、到期日和 CVV 的函数。它们看起来像这样。

```F#
let validateNumber number: Result<string, string> =
    if String.length number > 16 then
        Error "A credit card number must be less than 16 characters"
    else
        Ok number

let validateExpiry expiry: Result<string, string> =
    // Some validation checks for an expiry

let validateCvv cvv: Result<string, string> =
   // Some validation checks for a CVV
```

验证检查的细节在这里并不重要，我刚刚展示了一个卡号的基本示例，但实际上它们会更复杂。需要注意的主要事情是，每个函数都接受一个未验证的字符串，并返回一个 `Result<string, string>`，表示该值是否有效。如果该值无效，则它将返回一个包含错误消息的字符串。

## 我们的第一次尝试

真幸运！我们所要做的就是以某种方式组合这些函数来验证信用卡实例。我们试试吧！

```F#
let validateCreditCard card =
    let validatedNumber = validateNumber card.Number
    let validatedExpiry = validateExpiry card.Expiry
    let validatedCvv = validateCvv card.Cvv

    { Number = validatedNumber
      Expiry = validatedExpiry
      Cvv = validatedCvv }
```

嗯，这不符合要求。问题是，我们试图将 `Result<string, string>` 传递给 `CreditCard` 末尾的字段，但 `CreditCard` 的字段的类型为 `string`。

## 我用我的小眼睛窥探到一些以 M 开头的东西👀

鉴于我们现在已经[浏览了单子](https://dev.to/choc13/grokking-monads-in-f-3j7f)，我们可能会注意到，我们可以使用单子来解决我们的问题。每个验证函数都接收一个 `string` 并将其提升为 `Result<string, string>`，我们似乎想将其中几个函数链接在一起。我们在《Grokking Monads》中看到，我们可以将 `bind` 用于这种类型的链接。作为参考，`Result` 的 `bind` 如下。

```F#
let bind f result =
    match result with
    | Ok x -> f x
    | Error e -> Error e
```

因此，让我们尝试使用 `bind` 将 `validateCreditCard` 编写为链。

```F#
let validateCard card =
    validateNumber card.Number
    |> bind (validateExpiry card.Expiry)
    |> bind (validateCvv card.Cvv)
    |> fun number expiry cvv ->
        { Number = number
          Expiry = expiry
          Cvv = cvv }
```

看起来很整洁，但它仍然无法编译！

绑定调用需要一个函数，该函数将前一次计算中的验证值作为输入。在第一种情况下，它将是传递给 `validateExpiry` 函数的已验证 `number`。但是，`validateExpiry` 不需要已验证的 `number` 作为输入，它需要未验证的到期日，但我们确实需要以某种方式跟踪该已验证的号码，直到结束，以便我们可以使用它来构建有效的 `CreditCard` 实例。

通过在过程中积累这些中间验证结果，可以纠正这些问题。

```F#
let validateCard card =
    validateNumber card.Number
    |> bind
        (fun number ->
            validateExpiry card.Expiry
            |> Result.map (fun expiry -> number, expiry))
    |> bind
        (fun (number, expiry) ->
            validateCvv card.Cvv
            |> Result.map
                (fun cvv ->
                    { Number = number
                      Expiry = expiry
                      Cvv = cvv }))
```

诶呀😱 相当混乱，肯定比我们想要的更令人困惑。在每个阶段，我们都必须创建一个 lambda，它将上一步的验证值作为输入，验证另一条数据，然后将其全部累积在一个元组中，直到我们最终拥有构建整个 `CreditCard` 的所有比特。

我们的简单验证任务已经迷失在 lambdas 和中间元组对象的海洋中。想象一下，如果我们在 `CreditCard` 上有更多需要验证的字段，会有多混乱。我们需要的是一个解决方案，避免我们必须创建如此多的中间对象。

## 应用子前来救援🦸

另一种积累价值的方法是通过部分应用。这允许我们接受一个有 `n` 个参数的函数，并返回一个有 `n - 1` 个参数的函数。例如，让我们定义一个名为 `createCreditCard` 的函数，它处理纯字符串输入。

```F#
let createCreditCard number expiry cvv =
    { Number = number
      Expiry = expiry
      Cvv = cvv }
```

我们可以通过将值应用于函数来逐步积累这些值。

```F#
let number = "1234"
let numberApplied = createCreditCard number
```

`numberApplied` 是一个带有签名 `string -> string -> CreditCard` 的函数，或者用于命名这些参数 `expire -> cvv -> CreditCard`。因此，我们能够“存储”该数字以备后用，而无需创建中间元组。

因此，让我们发明一个名为 `apply` 的函数，该函数使用部分应用，但用于包装在其他结构（如 `Result`）中的值，并将其放在每个参数之前，如下所示。

```F#
let validateCreditCard card: Result<CreditCard, string> =
    Ok (createCreditCard)
    |> apply (validateNumber card.Number)
    |> apply (validateExpiry card.Expiry)
    |> apply (validateCvv card.Cvv)
```

您可能想知道为什么我们需要将 `createCreditCard` 包装在 `Ok` 中。这是因为此函数将返回 `Result<CreditCard, string>`，因此 `apply` 必须返回 `Result`。这意味着，为了将它们链接在一起，它还必须接受一个结果作为输入。因此，我们需要首先将 `createCardFunction` “提升”到 `Result` 中，以正确的类型启动链。

函数的 `Result` 可能看起来很奇怪，但请记住，我们将使用部分应用在每次调用 `apply` 后逐渐累积状态。所以我们在这里做的是从一个 `Ok` 的空容器开始，逐步用数据填充它，在每一步检查新数据是否 `Ok`。

像往常一样，我们可以让类型来指导我们编写这个函数。在链的每个阶段，我们需要做的就是接受两个论点。第一个是 `Result<T, E>`，第二个是 `Result<(T -> V), E>`。我们想尝试解包类型 `T` 的值和类型 `T -> V` 的函数，如果它们都是 `Ok` 的，我们可以将该值应用于函数。

类型 `T -> V` 可能看起来像一个只有一个参数的函数，但没什么好说 `V` 本身不能是另一个函数。因此，虽然这可能看起来只在函数输入有一个参数时有效，但事实上，只要第一个参数与我们希望应用的 `Result` 中包含的值类型匹配，它就适用于任何数量的参数的函数。

因此，`apply` 应该具有签名 `Result<T, E> -> Result<(T -> V), E> -> Result<V, E>`, 但我们会看到，仅凭这些相当抽象的信息，实现起来就很简单了。

```F#
let apply a f =
    match f, a with
    | Ok g, Ok x -> g x |> Ok
    | Error e, Ok _ -> e |> Error
    | Ok _, Error e -> e |> Error
    | Error e1, Error _ -> e1 |> Error
```

基本上，我们所能做的就是对函数 `f` 和参数 `a` 进行模式匹配，然后进行案例分析，这给了我们四个需要仔细研究的案例。在第一种情况下，这两个值都是 `Ok` 的，所以我们只需解包它们并将值应用于函数，然后在 `Ok` 中重新打包。在所有其他情况下，我们至少有一个错误，所以我们返回它。最后一个案例很有趣，因为我们有两个错误，我们决定只保留第一个。

## 测试一下

让我们测试 FSharp repl 中的 `apply` 函数，以确保其行为正确。这也将有助于我们提高认识。

```F#
> Ok (createCreditCard) 
-    |> apply ((Ok "1234"): Result<string, string>) 
-    |> apply (Ok "08/19") 
-    |> apply (Ok "123")

val it : Result<CreditCard,string> = Ok { Number = "1234"
                                          Expiry = "08/19"
                                          Cvv = "123" }
```

看起来不错，如果所有输入都有效，那么我们就会得到一张有效的 `CreditCard`。让我们看看当其中一个输入不好时会发生什么。

```F#
> Ok (createCreditCard) 
-    |> apply (Ok "1234") 
-    |> apply (Ok "08/19") 
-    |> apply (Error "Invalid CVV")

val it : Result<CreditCard,string> = Error "Invalid CVV"
```

太好了，正如我们所希望的那样。最后，如果我们有多个坏输入怎么办。

```F#
> Ok (createCreditCard) 
-    |> apply (Error "Invalid card number") 
-    |> apply (Ok "08/19") 
-    |> apply (Error "Invalid CVV")

val it : Result<CreditCard,string> = Error "Invalid card number"
```

这也是我们设计的目的。在这里，它在第一个错误的输入上失败了。你们中的许多人可能会想知道这是否可取，当然最好返回所有错误。在下一篇文章中，我们将看看如何做到这一点。

## 你刚刚发现了应用子👏

`apply` 函数是使某物具有应用子性的东西。希望通过看到它们解决的问题，你能比仅仅盯着 `apply` 的类型签名和阅读应用子的定律更深入、更直观地理解它们。

应用子类似于单子，因为它们提供了一种组合多个计算输出的方法，但当这些计算是独立的时，应用子是有用的，而对于单子，我们取一个的结果并将其用作另一个的输入。

## 稍微整理一下🧹

如果你不喜欢必须将 `createCreditCard` 函数包装在 `Ok` 中，那么我们可以去掉它。如果你已经准备好 [Grokking Functors](https://dev.to/choc13/grokking-functors-bla)，那么你会看到可以为 `Result` 定义 `map`，使其成为一个 functor。我们知道 `map` 接受一个函数，如果它正常，则将其称为 `Result` 的内容。因此，我们实际上可以这样使用它来启动链。

```F#
let validateCard card =
    (validateNumber card.Number)
    |> Result.map createCreditCard
    |> apply (validateExpiry card.Expiry)
    |> apply (validateCvv card.Cvv)
```

不过这有点尴尬，因为流似乎都与 3 个参数中间的 `createCreditCard` 函数混淆了。为了解决这个问题，定义一个 `<!>` 是很常见的 `map` 的中缀运算符，然后读取。

```F#
let validateCard card =
    createCreditCard 
    <!> validateNumber card.Number
    |> apply (validateExpiry card.Expiry)
    |> apply (validateCvv card.Cvv)
```

最后，通常也会使用 `<*>` 来表示 `apply`，这给了我们这个。

```F#
let validateCard card =
    createCreditCard 
    <!> validateNumber card.Number
    <*> validateExpiry card.Expiry
    <*> validateCvv card.Cvv
```

如果你觉得这很困惑，不要被它吓倒，它们只是符号。探索应用子（Grokking applicatives）是为了了解 `apply` 是如何工作的，它解决了什么问题，而不是关于这种稍微深奥的语法。我在这里只指出这一点，因为以这种方式使用它们是相当常见的。

## 在野外发现应用子🐗

每当你发现自己需要调用一个有多个参数的函数，但你必须提供的值被包裹在一个类似 `Result` 的东西中时，应用子可能会帮助你解决问题。

除了 `Result` 之外，还可以使更多的类型具有应用子性，我们所要做的就是为它定义适当的 `apply` 函数。例如，我们可以为 `option` 定义它。正如我们上面暗示的那样，实现这样一个函数的方法也可能不止一种，所以请确保您选择了具有所需语义的方法。

## 考验自己🧑‍🏫

看看你是否可以为 `option` 类型写 `apply`。答案如下，在你先尝试之前不要偷看！

选项解决方案

```F#
let applyOpt a f =
    match f, a with
    | Some g, Some x -> g x |> Some
    | None, _
    | _, None -> None
```

这就像 `Result` 一样，但因为在 `None` 的情况下没有额外的信息，我们可以将所有至少包含一个 `None` 的模式组合成一个返回 `None` 的表达式。

## 我们学到了什么？🧑‍🎓

通过定义一个 `apply` 函数，我们能够将包装在 `Result` 中的参数*应用*于需要常规 `string` 参数的函数。我们看到了这样做如何使我们能够使用部分应用作为逐步积累数据的一种手段，这有效地使我们能够以与不必处理无效输入时非常相似的风格编写代码。



# 5 Grokking Applicative Validation

https://dev.to/choc13/grokking-applicative-validation-lh6

在之前的 [Grokking Applicates](https://dev.to/choc13/grokking-applicatives-44o1) 中，我们发现了 Applicatives，并更具体地发明了 `apply` 函数。我们通过考虑验证信用卡字段的示例来实现这一点。`apply` 函数使我们能够轻松地将单独验证号码、到期日和 CVV 的结果组合成一个代表整个信用卡验证状态的 `Result<CreditCard>`。您可能还记得，当多个字段无效时，我们稍微掩盖了错误处理。我们走了一条容易的路，只返回了发生的第一个错误。

## 不满意的顾客😡

本着敏捷的精神，我们决定发布我们之前的实现，因为，好吧，总比没有好。过了一会儿，顾客开始抱怨。所有的抱怨都是这样的。

> “我在你的网站上输入了我的信用卡详细信息，经过三次尝试才最终被接受。我提交了表格，每次都会出现新的错误。你为什么不能一次告诉我所有的错误呢？”

为了更清楚地看到这一点，请考虑一个以 JSON 形式输入以下数据的客户。

```json
{
    “number”: “a bad number”,
    “expiry”: “invalid expiry”,
    “cvv”: “not a CVV”
}
```

他们第一次提交表格时会收到一个错误，比如`“‘坏号码’不是有效的信用卡号”`。因此，他们修复了该问题并重新提交。然后他们会收到一条消息，比如`“无效的到期日”不是有效的到期日`。因此，他们修复了这个问题，第三次提交时仍然收到一个错误，大意是`“‘不是CVV’不是有效的CVV”`。真烦人！

我们应该能够做得更好，一次返回所有错误。我们之前甚至指出，所有的现场级验证功能都是相互独立的。因此，没有充分的理由不运行所有函数并聚合错误（如果有的话），我们只是懒惰！

## 更好的验证应用子💪

让我们首先更新 `validateCreditCard` 的签名，以表示我们希望返回所有发现的验证错误。

```F#
let validateCreditCard (card: CreditCard): Result<CreditCard, string list>
```

这里唯一的变化是，我们现在返回的是错误消息 `list`，而不是单个错误消息。我们应该如何更新我们的实现以满足这个新的签名？

让我们回到之前定义的 `apply` 函数，看看是否可以在那里修复它。如果我们所要做的就是修改 `apply` 并保持 `validateCreditCard` 不变，那就太好了。

作为参考，这是我们上次编写的 `apply` 函数，它返回遇到的第一个错误。

```F#
let apply a f =
    match f, a with
    | Ok g, Ok x -> g x |> Ok
    | Error e, Ok _ -> e |> Error
    | Ok _, Error e -> e |> Error
    | Error e1, Error _ -> e1 |> Error
```

从这里我们可以看出，这只是我们需要处理多个错误的最后一种情况，所以我们只需要解决这些问题。最简单的修复方法是将这两个错误连接起来。这会在每次使用无效数据调用 `apply` 时建立一个错误列表。让我们看看那是什么样子。

```F#
let apply a f =
    match f, a with
    | Ok g, Ok x -> g x |> Ok
    | Error e, Ok _ -> e |> Error
    | Ok _, Error e -> e |> Error
    | Error e1, Error e2 -> (e1 @ e2) |> Error
```

这很容易，我们只是在两边都是 `Error` 的情况下使用 `@` 来连接这两个列表。其他一切都保持不变。

让我们以客户之前提供的错误数据为例，逐步验证信用卡。首先，我们调用 `Ok (createCreditCard) |> apply (validateNumber card.Number)`。这符合 `apply` 中模式匹配的第三种情况，因为 `f` 是 `Ok`，但参数 `a` 是 `Error`。这将返回类似于 `Error [ “Invalid number” ]` 的内容，但其类型仍然是 `Result<string -> string -> CreditCard, string list>`。

然后，我们像 `|> apply (validateExpiry card.Expiry)` 一样对其进行管道传输。这符合模式匹配中的最后一种情况，因为现在 `f` 和 `a` 都是 `Error`。这意味着 `@` 运算符用于将错误连接在一起，以创建类似 `Error [ “Invalid expiry”; “Invalid number” ]` 的内容。其类型现在是 `Result<string -> CreditCard, string list>` ，因为我们现在只需要提供一个 CVV 来完成创建 `CreditCard`。

因此，在最后一步中，我们正是这样做的，并将这个结果像 `|> apply (validateCvv card.Cvv)` 一样进行管道传输。就像最后一步一样，我们遇到了 `f` 和 `a` 都是 `Error` 的情况，因此我们将它们合并。现在，我们得到了一个我们想要的 `Result<CreditCard, string list>` 类型的东西，其值类似于 `Error [ “Invalid CVV”; “Invalid expiry”; “Invalid number” ]`。

## 一个小的编译时错误

您可能已经发现，我们现在实际上已经更改了 `apply` 函数的类型。通过使用 `@` 运算符，F# 推断出错误必须是 `list`。所以现在 `apply` 的签名是 `Result<T, E list> -> Result<T -> V, E list> -> Result<V, E list>`。

我们现在有一个适用于 `Result<T，E list>` 的 `apply`。也就是说，它适用于任何错误包含在 `list` 中的结果，而不是像 `string` 那样的单个值。关于这一点有几个有趣的地方：

1. 列表中的错误可以是任何类型，只要它们都是同一类型的。
2. 如果我们想将它们与 `apply` 一起使用，我们所有经过验证的结果现在都必须有一个错误 `list`。

第 1 点很有用，因为它允许我们以更有意义的方式对错误进行建模，而不仅仅是使用字符串。尽管在本文的其余部分，我们将继续使用 `string`，以保持简单。建模错误值得一篇博客文章。

然而，第 2 点给我们带来了一个必须解决的小问题。我们最初的字段级验证函数仍然返回 `Result<string, string>`，因此它们不再适用于我们的新版本的 `apply`。

在解决这个问题时，我们有两个选择。我们可以保持函数的原样，并通过将结果的错误（如果存在）包装在 `list` 中来转换它们的输出。它可能看起来像这样。

```F#
let validateCreditCard (card: CreditCard): Result<CreditCard, string list> =
    let liftError result =
        match result with
        | Ok x -> Ok x
        | Error e -> Error [ e ]
    Ok (createCreditCard)
    |> apply (card.Number |> validateNumber |> liftError)
    |> apply (card.Expiry |> validateExpiry |> liftError)
    |> apply (card.Cvv |> validateCvv |> liftError)
```

另一种选择是更新这些字段验证函数，以便它们根据需要返回 `Result<string, string list>`。选择第一个可能很诱人，如果我们无法控制这些功能，我们就必须这样做。然而，通过让这些字段级函数返回一个列表，我们赋予了它们进行更复杂验证的灵活性，并可能指示多个错误。

例如，`validateNumber` 函数可以指示长度问题和存在无效字符，如下图所示。

```F#
let validateNumber number: Result<string, string list> =
    let errors = 
        if String.length num > 16 then
            [ "Too long" ]
        else
            []
    let errors = 
        if num |> Seq.forall Char.IsDigit then
            errors
        else 
            "Invalid characters" :: errors

    if errors |> Seq.isEmpty then
        Ok num
    else
        Error errors
```

始终使用 `Result<T, E list>` 为我们提供了一个更具可组合性和灵活性的 api，使我们能够在不影响程序其余部分的情况下重构将来从这些函数返回的错误。

因此，鉴于它们是我们领域中的函数，那么我们将采用这种方法。让我们尝试一下，看看使用这个新版本的 `apply` 时它是什么样子的。

```F#
let validateNumber num: Result<string, string list> =
    if String.length num > 16 then
        Error [ “Too long” ]
    else
        Ok num

let validateExpiry expiry: Result<string, string list> =
    // validate expiry and return all errors we find

let validateCvv cvv: Result<string, string list> =
    // validate cvv and return all cvv errors we find

let validateCreditCard (card: CreditCard): Result<CreditCard, string list> =
    Ok (createCreditCard)
    |> apply (validateNumber card.Number)
    |> apply (validateExpiry card.Expiry)
    |> apply (validateCvv card.Cvv)
```

干得好！除了将错误提升到 `validateNumber` 等列表中的几个小更改外，其余的都保持不变。特别是，`validateCreditCard` 的主体完全没有变化。

## 我必须为错误使用列表吗

我们对错误类型的唯一要求是，我们可以使用 `@` 运算符将错误联系在一起。因此，只要错误是能连接的（concat-able），我们就可以在这里使用不同的类型。它的奇特的范畴学名称是半群（semi-group）。半群是指至少定义了一个 concat 运算符的任何东西。这里使用的一种常见类型是 `NonEmptyList`，因为我们知道，如果结果是 `Error`，那么它们将至少是列表中的一个项目。

## 两个应用子的故事📗

现在我们已经看到了 `apply` for `Result` 的两个实现。我们可以两者兼得吗？遗憾的是，这两者都没有在 F# 的 `Result` 模块中定义。为了做到这一点，F# 必须能够根据错误类型是否支持 concat 来决定使用哪种类型，如果没有显式的类型注释，这甚至可能不明显。即使这样，我们也可能会得到不希望的结果，因为字符串支持 concat，但我们不太可能将单个错误消息合并到一个长字符串中。

那么，我们应该如何判断哪一个是正确的呢？好吧，我们不必这样做。我们可以定义另一种类型，称为 `Validation`，它有一个 `Success` 案例和一个 `Failure` 案例，类似于 `Result` 的 `Ok` 和 `Error`。不同之处在于，对于 `Validation`，我们可以使用我们在这篇文章中创建的版本来定义 `apply`，该版本会累积错误，而对于 `Result`，使用 `apply` 函数来短路并返回我们在上一篇文章中看到的第一个错误。幸运的是，优秀的 [FSharpPlus](https://fsprojects.github.io/FSharpPlus/) 库已经做到了这一点。

## 我们学到了什么🧑‍🏫

我们已经看到，在编写验证代码时，应用子是一个很好的工具。它们允许我们为每个字段编写验证函数，然后轻松组合这些函数，以创建用于验证由这些字段组成的较大结构的函数。

我们还看到，虽然应用计算通常是相互独立的，但没有什么能保证 `apply` 的特定实现会充分利用这一点。具体来说，在进行验证时，我们希望确保 `apply` 会累积所有错误，因此我们应该确保使用 FSharpPlus 中的 `Validation` 类型来获得这种行为。



# 6 Grokking Traversable

https://dev.to/choc13/grokking-traversable-bla

一旦你摸索过可遍历的东西，你会想知道没有它们你是怎么生活的。试图通过盯着类型签名来获得对它们的直觉，从来没有给我带来多少快乐。因此，在这篇文章中，我们将采取一种不同的方法，通过解决一个实际问题来自己发明它们。这将帮助我们达到“啊哈”的时刻，我们最终了解它们是如何工作的，以及何时使用它们。

## 场景

想象一下，我们正在为一个电子商务网站工作，在那里我们出售一次性优惠，这样当所有库存都卖完时，我们就再也没有了。当用户下订单时，我们必须检查库存水平。如果有空位，我们会在让他们继续结账之前临时预订他们要求的金额。

我们的具体任务是编写一个 `createCheckout` 函数，该函数将接收 `Basket` 并尝试预订其中的商品。如果可以成功预订，它将创建一个 `Checkout`，其中包括商品的总价以及我们可能需要支付的其他元数据。

我们的域模型看起来像这样。

```F#
type BasketItem = 
    { ItemId: ItemId
      Quantity: float }

type Basket = 
    { Id: BasketId; 
      Items: BasketItem list }

type ReservedBasketItem =
    { ItemId: ItemId
      Price: float }

type Checkout = 
    { Id: CheckoutId
      BasketId: BasketId
      Price: float }
```

`createCheckout` 函数将返回 `Checkout option`。如果所有项目都可用，它将返回 `Some`，如果其中任何一个项目都不可用，则返回 `None`。*更好的实现将返回 `Result` 并详细说明具体错误，但我们将使用 `option` 来保持示例简单。*

```F#
let createCheckout (basket: Basket): Checkout option
```

幸运的是，其他人已经编写了一个函数，可以在有库存的情况下保留 `BasketItem`，如下所示。

```F#
let reserveBasketItem (item: BasketItem): ReservedBasketItem option
```

同样，如果库存中没有足够的物品，这将返回 `None`。

## 我们的首次实现

因此，我们似乎只需要编写一个函数，为购物篮中的每个项目调用 `reserveBasketItem`。如果他们都成功了，那么它会计算总价以创建 `Checkout`。我们试试吧。

```F#
let createCheckout basket =
    let reservedItems =
        basket.Items |> List.map reserveBasketItem

    let totalPrice =
        reservedItems
        |> List.sumBy (fun item -> item.Price)

    { Id = CheckoutId "some-checkout-id"
      BasketId = basket.Id
      Price = totalPrice }
```

在这里，我们只是映射购物篮中的商品以预订每一个，然后将它们的单独价格相加，得到购物篮的总价格。这似乎很简单，只是它不会编译，因为它不太正确。

问题是 `reservedItems` 的类型是 `list<option<ReservedBasketItem>>`，但我们需要它是 `option<list<ReservedBasketItem>>>`，如果任何一个保留失败，它就是 `None`。这样，我们只能计算总价，并在所有商品都可用的情况下创建结账。让我们想象一下，我们编写了一个名为 `reserveItems` 的函数，它确实返回了这种类型，并更新了 `createCheckout` 以使用它。

```F#
let reserveItems (items: BasketItem list): option<list<ReservedBasketItem>>

let createCheckout basket =
    let reservedItems = basket.Items |> reserveItems

    reservedItems
    |> Option.map
        (fun items ->
            { Id = CheckoutId "some-checkout-id"
              BasketId = basket.Id
              Price = items |> List.sumBy (fun x -> x.Price) })
```

好多了！现在，如果所有项目都是保留的，并且 `reservedItems` 返回 `Some`，那么我们可以访问 `ReservedBasketItem` 的列表，并使用它们创建 `Checkout`。如果任何项目无法保留，则 `reservedItems` 返回 `None`，`Option.map` 只是短路，这意味着 `createCheckout` 也将返回 `None`，正如我们所希望的那样。

因此，我们将任务简化为实现 `reserveItems`。我们已经看到，我们不能只调用 `List.map reserveBasketItem`，因为这给了我们一个 `list<option<ReservedBasketItem>>`，所以 `list` 和 `option` 的方式是错误的。我们需要一种方法来反转它们。

## 反转器🙃

让我们发明一个名为 `invert` 的函数，将 `list<option<ReservedBasketItem>` 转换为 `option<list<ReservedBasketItem>`。如果我们能做到这一点，那么我们就可以实现这样的 `reserveItems`。

```F#
let invert (reservedItems: list<option<ReservedBasketItem>>) : option<list<ReservedBasketItem>>

let reserveItems (items: BasketItem list) : option<list<ReservedBasketItem>> =
    items 
    |> List.map reserveBasketItem 
    |> invert
```

为了实现 `invert`，让我们从列表上的模式匹配开始。

```F#
let invert (reservedItems: list<option<ReservedBasketItem>>) : option<list<ReservedBasketItem>> =
    match reservedItems with
    | head :: tail -> // do something when the list isn't empty
    | [] -> // do something when the list is empty
```

所以我们有两种情况需要处理，当列表中至少有一个项目时，以及当列表为空时。让我们从基本情况开始，因为它很琐碎。如果列表为空，则不包含任何失败，因此我们应该只返回 `Some[]`。

在非空的情况下，我们必须对 `head` 做点什么，head 是一个 `ReservedBaskedItem option`，`tail` 是一个 `list<option<ReservedBasketItem>>`。我们知道我们的目标是将 `list<option<ReservedBasketItem>>` 转换为 `option<list<ReservedBaskedItem>>`，所以我们可以递归地在 `tail` 调用 `invert` 来实现这一点。

```F#
let rec invert (reservedItems: list<option<ReservedBasketItem>>) : option<list<ReservedBasketItem>> =
    match reservedItems with
    | head :: tail -> 
        let invertedTail = invert tail
        // Need to recombine the head and the inverted tail
    | [] -> Some []
```

现在，我们只需要一种方法将 `ReservedBasketItem option` 与 `option<list<ReservedBasket Item>>` 组合在一起。如果这两个都没有被包装在 `option` 中，那么我们只会使用 `::` 运算符来“cons”它们，所以让我们编写一个 `consOptions` 函数，除了 `option` 参数之外，它还能做到这一点。

```F#
let consOptions (head: option 'a) (tail: option<list<'a>>): option<list<'a>> = 
    match head, tail with 
    | Some h, Some t -> Some (h :: t) 
    | _ -> None
```

这里没有太复杂的事情。只需检查 `head` 和 `tail` 是否都是 `Some`，如果是的话，用 `::` 运算符将它们括起来，然后用 `Some` 括起来。否则，如果其中任何一个为 `None`，则返回 `None`。

把所有这些放在一起，我们最终可以实现这样的 `invert`。

```F#
let rec invert (reservedItems: list<option<'a>>) : option<list<'a>> =
    match reservedItems with
    | head :: tail -> consOptions head (invert tail)
    | [] -> Some []
```

我们还能够使它在列表中的类型上完全通用，因为它不以任何方式依赖于 `ReservedBasketItem`。

## 用应用子清理🧽

如果你熟悉应用程序，也许是因为你已经阅读了本系列并阅读了 [Grokking Applicatives](https://dev.to/choc13/grokking-applicative-validation-lh6)，那么你可能已经发现 `consOptions` 看起来有点像 `apply` 的专业版本。`consOptions` 试图做的是获取一些包裹在 `options` 中的值，并将其应用于函数，在本例中为 cons。

让我们利用 `apply` 和清理 `invert`。

```F#
let rec invert list =
    // An alias for :: so we can pass it as a function below
    let cons head tail = head :: tail

    match list with
    | head :: tail -> Some cons |> apply head |> apply (invert tail)
    | [] -> Some []
```

事实上，一个适当的 `Applicative` 实例也应该具有 `pure` 函数。`pure` 操作只需为 `Applicative` 创建默认值。在 `option` 的情况下，`pure` 只是 `Some`。让我们用 `pure` 来代替 `Some` 用法。

```F#
let rec invert list =
    let cons head tail = head :: tail

    match list with
    | head :: tail -> pure cons |> apply head |> apply (invert tail)
    | [] -> pure []
```

这似乎不是什么大的变化，但我们所做的是消除对 `option` 的所有直接依赖。理论上，这可以适用于任何应用程序，如 `Result` 或 `Validation`，它所做的就是从 `list<Applicative<_>>` 转到 `Applicative<list<_>>`。然而，在实践中，F# 并不完全允许这样的抽象，因此我们必须为我们想要使用的每种应用子类型创建一个 `invert` 版本。

*从技术上讲，您可以通过[静态解析的类型参数](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/generics/statically-resolved-type-parameters)来解决这个问题。如果你想要这个抽象，我建议你看看 [FSharpPlus](https://fsprojects.github.io/FSharpPlus/)，而不是自己动手。*

## 你刚刚发现了 `sequence`👏

`invert` 通常被称为 `sequence`，它是 `Traversable` 类型为我们提供的函数之一。正如我们所看到的，`sequence` 像 `option` 一样接受一组包装值，并将其转换为包装集合。你可以把 `sequence` 看作是翻转这两种类型。

`sequence` 也适用于各种其他类型的组合。例如，你可以把一个 `list<Result<'a>>` 翻转成一个 `Result<list<'a>>`。你甚至可以将它用于不同的集合类型，有些甚至看起来不像典型的集合，例如，你可以从 `Result<option<'a>，'e>` 转到 `option<Result<'a，'e>>`。

## 在 `sequence` 上测试自己🧑‍🏫

看看你是否可以实现 `list<Result<_>>` 到 `Result<list<_>>` 的 `sequence`。

解决方案

```F#
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
```

没错，如果我们为 `Result` 使用应用子 `Result.apply` 和 `Result.pure` 函数，它与 `list<option<_>>` 完全相同。我也在上面的 `Result` 模块中包含了它们的定义。

## 还有更多的土地有待发现🏞

让我们回到最初的程序，看看它与我们的新 `sequence` 发现是什么样子的。

```F#
let createCheckout basket =
    let reservedItems = 
        basket.Items 
        |> List.map reserveBasketItem 
        |> sequence

    reservedItems
    |> Option.map
        (fun items ->
            { Id = CheckoutId "some-checkout-id"
              BasketId = basket.Id
              Price = items |> Seq.sumBy (fun x -> x.Price) })
```

这很好，但我们创建 `reservedItems` 时必须对 `basket.Items` 做两次传递。在第一轮中，我们尝试保留每个项目，然后在第二轮中，将所有这些保留合并在一起，以确定整个操作是否成功。如果我们能一次性完成，那就太好了。

让我们看看我们是否能在 `sequence` 中完成这一切。这意味着我们需要将 `reserveBasketItem` 函数传递给 `sequence`，最终得到以下签名。

```F#
let sequence (f: 'a -> 'b option) (list: 'a list): option<list<'b>>
```

因此，我们从一个列表开始，我们想将函数 `f` 应用于它的每个元素。虽然，我们不只是映射到列表上并返回 `list<option<'b>>`，而是想将所有 `option` 值累积到一个 `option<list<'b>` 中，如果任何元素 `f` 产生 `None`，则该选项为 `None`。

```F#
let rec sequence f list =
    let cons head tail = head :: tail

    match list with
    | head :: tail -> Some cons |> apply (f head) |> apply (sequence tail f)
    | [] -> Some []
```

这与之前基本相同，只是现在我们只需将 `f` 应用于 `head` 并将其传递给递归调用，以便也转换 `tail` 元素。我们所做的就是将生成 `option` 值的操作与将它们组合在一起的行为组合成列表中的单个 `option`。

## 你刚刚发现了 `traverse`🙌

事实证明，当我们同时组合排序和映射时，我们通常会调用函数 `traverse`。因此，`Traversable` 实际上有两个与之相关的函数，称为 `sequence` 和 `traverse`。事实上，`sequence` 只是 `traverse` 的一个特例，我们为 `f` 提供恒等式函数 `id`。所以我们实际上可以这样写。

```F#
let sequence = traverse id
```

有了 `traverse`，我们终于可以完成任务，并像这样很好地编写 `checkoutBasket`。

```F#
let createCheckout basket =
    basket.Items 
    |> traverse reserveBasketItem
    |> Option.map
        (fun items ->
            { Id = CheckoutId "some-checkout-id"
              BasketId = basket.Id
              Price = items |> Seq.sumBy (fun x -> x.Price) })
```

## 在 `traverse` 中测试自己🧑‍🏫

看看当输入是 `option<'a>` 并且函数是 `'a -> Result<'b, 'c>` 时，你是否可以实现遍历，这样它就返回了一个 `Result<option<'b>,'c>`。

解决方案

```F#
module Result =
    let apply a f =
        match f, a with
        | Ok g, Ok x -> g x |> Ok
        | Error e, Ok _ -> e |> Error
        | Ok _, Error e -> e |> Error
        | Error e1, Error _ -> e1 |> Error

    let pure = Ok

let traverse f opt =
    match opt with
    | Some x -> Result.pure Some |> Result.apply (f x)
    | None -> Result.pure None
```

在这里，我已经包含了 `Result` 的 `apply` 和 `pure` 的定义，然后使用这些定义实现了 `traverse`。希望这能更清楚地说明遍历操作的哪些部分与外部 `option` 类型相关，哪些部分与内部 `Result` 类型相关。

这种转换的一个具体用例可能是，如果我们试图编写一个解析器。解析器函数可能会说将 `string` 解析为 `Result<int, ParseError>` ，但我们必须提供一个 `string option`。当然，我们可以自己对 `option` 进行模式匹配，然后只在 `Some` 情况下运行解析器，但我们也可以编写 `myOptionalValue |> traverse parseInt`。

另一个有趣的情况是，当我们处理一个常规函数时，比如字符串，它只是将参数转换为字符串。看看你是否能弄清楚在这种情况下遍历应该是什么样子。具体来说，如果我们想写 `[1；2；3] |> traverse string` 并将其输出 `["1"; "2"; "3"]`。

解决方案

```F#
module Identity = 
    let apply a f = f a
    let pure f = f

let rec traverse list f =
    let cons head tail = head :: tail

    match list with
    | head :: tail -> Identity.pure cons |> Identity.apply (f head) |> Identity.apply (traverse tail f)
    | [] -> Identity.pure []
```

我以与其他人相同的风格写了这篇文章，提取了一个 `Identity` functor/applicative。`Identity` 实际上是应用子的退化情况，因为所有 `apply` 所做的都是用参数调用函数，而所有 `pure` 操作都是原封不动地返回函数。所以，并没有像其他应用程序那样进行包装。不过这很有趣，因为 `traverse` 现在有类型 `list<'a> -> ('a -> 'b) -> list<'b>`，你可能会从 [Grokking Functors](https://dev.to/choc13/grokking-functors-bla) 中认出它是map。因此，当内部类型只是 `Identity` 应用子时，`map` 实际上是 `traverse` 的一个特例。

## 在野外发现 `Traversable`🐾

每当你有一些值的集合被包装在 `option` 或 `Result` 中，而你真正需要的是 `option<list<'a>>` 或 `Result<list<'a>, 'e>` 等，那么 `sequence` 可能就是你需要使用的。同样，如果你必须在产生包装值的集合上运行计算，那么你可以使用 `traverse`，并将映射和翻转组合成一个操作。

## 警告，前面有两种错误处理方式⚠️

当我们对 `list<option<_>>` 进行排序时，我们只需要知道至少有一个元素是 `None`，就可以返回 `None`。然而，当使用类似 `list<Result<'a, 'e>>` 的东西时，我们实际上可能会关心收集所有错误。正如我们在 [Grokking 应用子验证](https://dev.to/choc13/grokking-applicative-validation-lh6)中指出的那样，可能存在第一个错误短路或累积所有错误的应用实例。这同样适用于 `Traversable`。让我们用 FSharpPlus 在 F# REPL 中快速运行一些实验，看看它是如何处理事情的。

```F#
> [Ok 1; Error "first error"; Error "second error"] |> sequence;;
val it : Result<int list, string> = Error "first error"

[Success 1; Failure ["first error"]; Failure ["second error"]] |> sequence;;
val it : Validation<string list, int list> =
  Failure ["first error"; "second error"]
```

在第一种情况下，当使用 `Result` 时，我们看到它只返回遇到的第一个错误，而使用 `Validation` 时，它实际上为我们累积了所有错误。

## 我们学到了什么🧑‍🎓

`Traversable` 是 `map` 的一个更强大的版本，当我们有一个需要在一系列值上运行（或已经运行）的计算时，它特别有用，如果任何一个值失败，我们希望将其视为失败。我们也可以通过意识到它翻转了两种外部类型来探索它。当我们仍然需要运行计算时，我们使用 `traverse`，而当我们得到计算结果列表时，我们则使用 `sequence`。



# 7 Grokking the Reader Monad

https://dev.to/choc13/grokking-the-reader-monad-4f45

从它的名字来看，阅读器 monad 并没有给出太多关于它在哪里有用的线索。在这篇文章中，我们将通过自己发明它来探索它，以解决一个真正的软件问题。由此我们可以看出，这实际上是函数式编程中进行依赖注入的一种方式。

## 先决条件

这里不会有任何理论，但如果你已经摸索过单子，那就更容易了。如果你还没有，请查看本系列前面的 [Grokking Monads](https://dev.to/choc13/grokking-monads-in-f-3j7f)，然后回到这里。

## 场景

让我们想象一下，我们被要求编写一些向用户信用卡收费的代码。为此，我们需要从数据库中查找一些信息，并致电支付提供商。

我们的域模型将是这样的。

```F#
type CreditCard =
   { Number: string
     Expiry: string
     Cvv: string }

type EmailAddress = EmailAddress of string 
type UserId = UserId of string

type User =
    { Id: UserId
      CreditCard: CreditCard
      EmailAddress: EmailAddress }
```

我们还将从一个 `Database` 模块开始，其中包含一个可以读取用户的函数，以及一个 `PaymentProvider` 模块，其中包含可以向 `CreditCard` 收费的函数。它们看起来像这样。

```F#
type ISqlConnection =
    abstract Query : string -> 'T

module Database =

    let getUser (UserId id) : User =
        let connection = SqlConnection("my-connection-string")
        connection.Query($"SELECT * FROM User AS u WHERE u.Id = {id}")

type IPaymentClient =
    abstract Charge : CreditCard -> float -> PaymentId

module PaymentProvider =
    let chargeCard (card: CreditCard) amount = 
        let client = new PaymentClient("my-payment-api-secret")
        client.Charge card amount
```

## 我们的首次实现

让我们从我们能想到的最简单的解决方案开始。我们将调用数据库查找用户，从他们的个人资料中获取信用卡，并致电支付提供商进行收费。

```F#
let chargeUser userId amount =
    let user = Database.getUser userId
    PaymentProvider.chargeCard user.CreditCard amount
```

超级简单，因为我们已经准备好使用 `Database.getUser` 和 `PaymentProvider.chargeCard`。

不过，这里的耦合量可能会让你感到有点不舒服。直接调用 `getUser` 和 `chargeCard` 函数本身并不是问题。问题实际上在于这些函数本身是如何实现的。在这两种情况下，他们都在实例化新的客户端，如 `SqlConnection` 和 `PaymentClient`，这会产生一些问题：

1. 硬编码的连接字符串意味着我们在所有环境中都只能与同一个数据库实例通信。
2. 连接字符串通常包含现在签入源代码管理的机密。
3. 编写单元测试是不可能的，因为它将调用生产数据库和支付提供商。我想这是在运行所有这些单元测试时支付CI账单的一种方式😜

## 控制反转🔄

当你得知解决这个问题的方法是反转这些依赖关系时，你可能不会感到惊讶。控制反转（IoC）超越了范式，它在 OOP 和 FP 中都是一种有用的技术。只是 OOP 倾向于通过反射来利用构造函数注入，在 FP 中我们会看到还有其他解决方案可供我们使用。

那么，函数最简单的 IoC 技术是什么？只需将这些依赖项作为参数传入即可。这就像 OOP 类依赖关系，但在函数级别。

```F#
module Database =
    let getUser (connection: ISqlConnection) (UserId id) : User =
        connection.Query($"SELECT * FROM User AS u WHERE u.Id = {id}")

module PaymentProvider =
    let chargeCard (client: IPaymentClient) (card: CreditCard) amount = 
        client.Charge card amount

let chargeUser sqlConnection paymentClient userId amount =
    let user = Database.getUser sqlConnection userId
    PaymentProvider.chargeCard paymentClient user.CreditCard amount
```

这并不奇怪。我们刚刚提供了必要的客户端作为参数，并将其传递给需要它们的函数调用。不过，这种解决方案确实有缺点。主要原因是，随着依赖关系数量的增加，函数参数的数量可能会变得难以控制。

除此之外，大多数应用程序都有一定程度的分层。随着我们引入更多的层，为了分解和隔离各个功能的职责，我们开始需要通过许多层传递一些依赖关系。这是任何 IoC 解决方案的典型特征，一旦你翻转了这些依赖关系，它就会直接级联到应用程序的所有层。[这是~~海龟们~~一路颠倒的依赖关系](https://en.wikipedia.org/wiki/Turtles_all_the_way_down)。

## *部分*解决方案

我们想避免的是，必须将这些可传递的依赖关系显式地传递给 `chargeUser` 等函数，而这些函数并没有被直接使用。另一方面，我们不想因为回到基于反射的依赖注入而失去编译时检查。

如果我们将这些依赖参数移动到函数签名的末尾呢？这样，我们就可以使用部分应用程序将它们的提供推迟到最后一刻，当我们准备好“连接应用程序”时。让我们先试试这些服务模块。

```F#
module Database =
    let getUser (UserId id) (connection: ISqlConnection) : User =
        connection.Query($"SELECT * FROM User AS u  WHERE u.Id = {id}")

module PaymentProvider =
    let chargeCard (card: CreditCard) amount (client: IPaymentClient) = 
        client.Charge card amount
```

有了它，我们可以创建一个新函数，通过简单地编写以下内容，在传递连接时获取用户。

```F#
let userFromConnection: (ISqlConnection -> User) = Database.getUser userId
```

在给卡充值时，我们也可以做类似的事情。

```F#
let chargeCard: (IPaymentClient -> PaymentId) = PaymentProvider.chargeCard card amount
```

好吧，让我们把它粘在一起，重写我们的 `chargeUser` 函数。

```F#
let chargeUser userId amount =
    let user = Database.getUser userId
    // Problem, we haven’t got the user now, but a function that needs a ISqlConnection to get it
    PaymentProvider.chargeCard user.CreditCard amount
    // So the last line can’t access the CreditCard property 
```

这很好，但不太对！我们已经从 `chargeUser` 函数中删除了这两个依赖参数，但它无法编译。正如注释所指出的，我们没有我们需要的 `User`，而是一个 `ISqlConnection -> User` 类型的函数。这是因为我们只部分应用了 `Database.getUser`，为了完成调用并实际解析用户，我们仍然需要为其提供 `ISqlConnection`。

这是否意味着我们需要再次将 `ISqlConnection` 传递给 `chargeUser`？好吧，如果我们能找到一种方法来提升 `PaymentProvider.chargeCard`，使其可以与 `ISqlConnection->User` 一起工作，而不仅仅是 `User`，那么我们就可以编译它了。为了做到这一点，我们需要创建一个新函数，该函数接受 `ISqlConnection`，以及在给定 `ISqlConnection` 和我们想向用户收取的金额的情况下创建用户的函数。

我们并没有一个好的名字来命名这个函数，因为在这种情况下，拥有一个依赖于 `ISqlConnection` 的 `chargeCard` 函数是没有意义的。所以我们可以做的是在 `chargeUser` 内部创建一个匿名函数，一个 lambda，为我们完成这个提升。

```F#
let chargeUser userId amount: (ISqlConnection -> IPaymentClient -> PaymentId) =
    let userFromConnection = Database.getUser userId

    fun connection ->
        let user = userFromConnection connection
        PaymentProvider.chargeCard user.CreditCard amount
```

我已经注释了 `chargeUser` 的返回类型，以强调它现在返回一个新函数，当提供 `ISqlConnection` 和 `IPaymentClient` 的依赖关系时，它将向用户收费。

此时，我们已经设法推迟了任何依赖关系的应用，但解决方案仍然有点麻烦。如果在以后，我们需要在 `chargeUser` 中进行更多需要更多依赖关系的计算，那么我们将面临更多的 lambda 编写。例如，想象一下，我们想给用户发一封带有 `PaymentId` 的收据。那我们就得写这样的东西。

```F#
let chargeUser userId amount =
    let userFromConnection = Database.getUser userId

    fun connection ->
        let user = userFromConnection connection
        let paymentIdFromClient = PaymentProvider.chargeCard user.CreditCard amount

        fun paymentClient ->
            let (PaymentId paymentId) = paymentIdFromClient paymentClient
            let email = EmailBody $"Your payment id is {paymentId}"
            Email.sendMail user.EmailAddress email
```

😱

嵌套变得难以控制，代码编写变得很累，我们最终需要为此函数提供的依赖关系数量也变得难以处理。我们在这里有点进退两难。

## 约束我们走出困境

让我们看看是否可以编写一个名为 `injectSqlConnection` 的函数，通过消除我们编写提供 `ISqlConnection` 的 lambda 的需要来简化 `chargeUser`。这样做的目的是让我们能够像这样编写 `chargeUser`。

```F#
let chargeUser userId amount =
    Database.getUser userId
    |> injectSqlConnection (fun user -> PaymentProvider.chargeCard user.CreditCard amount)
```

因此，`injectSqlConnection` 需要接受一个需要 `User` 作为第一个参数的函数，以及一个可以在给定 `ISqlConnection` 的情况下创建 `User` 的函数作为第二个参数。让我们实施它。

```F#
let injectSqlConnection f valueFromConnection =
    fun connection ->
        let value = valueFromConnection connection
        f value
```

事实上，该函数无论如何都不依赖于 `ISqlConnection`。它适用于任何需要值 `a` 的函数 `f`，该值可以在传递某些依赖关系时创建。所以，从现在开始，让我们称之为 `inject`，以确认它适用于任何类型的依赖。

## 你刚刚发现了阅读器 monad🤓

`inject` 函数允许我们对每个依赖于上次计算返回的包装值的计算进行排序。在这种情况下，值被包装在一个需要依赖关系的函数中。这种模式看起来应该很熟悉，因为我们是在 [Grokking Monads](https://dev.to/choc13/grokking-monads-in-f-3j7f) 时发现的。事实证明，我们实际上又发现了 `bind`，但这次是针对一个新的 monad。

这个新的单子通常被称为 `Reader`，因为它可以被认为是从环境中读取一些值。在我们的例子中，我们可以称之为 `DependencyInjector`，因为它将一些依赖关系应用于函数，以返回我们想要的值。在这里弥合心理差距的方法是，将依赖注入视为从包含依赖关系的某个环境中读取值的一种方式。

## 一个小小的谎言🤥

实际上，上面 `inject` 的实现并不完全正确。如果我们使用 `inject` 重写更复杂的 `chargeUser`，也就是发送电子邮件的 chargeUser，那么我们将看到它是如何崩溃的。

```F#
let chargeUser userId amount =
    Database.getUser userId
    |> inject (fun user -> PaymentProvider.chargeCard user.CreditCard amount)
    |> inject (fun (PaymentId paymentId) -> 
        let email =
            EmailBody $"Your payment id is {paymentId}"

        let address = EmailAddress "a.customer@example.com"
        Email.sendMail address email)
```

这实际上在第二次 `inject` 时失败了。这是因为在第一次调用注入后，它返回以下类型 `ISqlConnection -> IPaymentClient -> PaymentId`。现在，在第二次调用 `inject` 时，我们有两个依赖关系需要处理，但我们的 `inject` 函数只设计为提供一个依赖关系，所以一切都失败了。

解决这个问题的方法是创建一个可以表示所有依赖关系的单一类型。基本上，我们希望 `chargeUser` 函数具有签名 `UserId -> float -> Dependencies -> TransactionId`，而不是 `UserId -> float -> ISqlConnection -> IPaymentClient -> TransactionId`。如果我们能做到这一点，那么我们只需要做一个小的调整来 `inject`，让事情重新开始。

```F#
let inject f valueThatNeedsDep =
    fun deps ->
        let value = valueThatNeedsDep deps
        f value deps
```

请注意，这次我们如何在最后一行向 `f` 提供 `deps`？这很微妙，但它将注入的返回类型更改为 `('deps->'c)`，其中 `'deps` 是 `valueThatNeedsDep` 也需要的依赖类型。

所以这里发生的事情是，我们现在已经将 `inject` 的输出限制为一个新函数，该函数需要与原始函数相同类型的 `'deps`。这很重要，因为这意味着我们的依赖关系现在统一为一种类型，我们可以愉快地将需要这些依赖关系的计算链接在一起。

## 统一依赖关系👭

有几种方法可以将所有依赖关系统一到一个类型中，例如显式创建一个具有字段的类型来表示每个类型。F#最简洁的方法之一是使用推断继承。推断继承意味着我们让编译器推断一个实现我们所需的所有依赖接口的类型。

为了使用推断继承，我们需要在每个依赖项的类型注释前面添加一个 `#`。让我们在数据库和 `PaymentProvider` 模块中进行更改，看看它是什么样子的。

```F#
module Database =
    let getUser (UserId id) (connection: #ISqlConnection) : User =
    // Implementation of getUser

module PaymentProvider =
    let chargeCard (card: CreditCard) amount (client: #IPaymentClient): TransactionId =
    // Implementation of chargeCard
```

我们所做的更改只是写 `#ISqlConnection` 而不是 `ISqlConnection`，写 `#IPaymentClient` 而不是 `IPaymentClient`。当 F# 遇到需要同时满足这两个约束的情况时，它可以将这些类型联合在一起。然后，在应用程序的根节点，我们只需创建一个实现这两个接口的对象，以满足约束。

其结果是，F# 推断 `chargeUser` 的类型签名为 `UserId -> float -> ('deps -> unit)`，并要求 `'deps` 从 `ISqlConnection` 和 `IPaymentProvider` 继承。

## 最后的改进

现在，我们几乎已经实现了消除函数之间传递的所有显式依赖关系的既定目标。然而，我认为在调用 `inject` 来组合操作时，我们必须不断创建 lambdas 来访问 `user` 和 `paymentId` 等值，这仍然有点烦人。我们之前在 [Grokking Monads, Imperatively](https://dev.to/choc13/grokking-monads-imperatively-394a) 中已经看到，通过使用计算表达式，可以以命令式风格编写单子性的代码。

我们所要做的就是使用我们之前编写的 `inject` 函数创建计算表达式构建器，因为这是我们的单子性的 `bind`。我们将称之为计算表达式 `injector`，因为这与我们的用例更相关，但通常它会被称为 `reader`。

```F#
type InjectorBuilder() =
    member _.Return(x) = fun _ -> x
    member _.Bind(x, f) = inject f x
    member _.Zero() = fun _ -> ()
    member _.ReturnFrom x = x

let injector = InjectorBuilder()

let chargeUser userId amount =
    injector {
        let! user = Database.getUser userId
        let! paymentId = PaymentProvider.chargeCard user.CreditCard amount
        let email =
            EmailBody $"Your payment id is {paymentId}"

        return! Email.sendMail user.Email email
    }
```

😍
通过简单地将实现封装在 `injector { }` 中，我们基本上回到了第一个天真的实现，除了这一次所有的控制都被正确地反转了。虽然传递依赖关系被很好地隐藏起来，但它们仍在进行类型检查。事实上，如果我们稍后添加更多需要新依赖关系的操作，那么 F# 会自动将它们添加到必须为 `'deps` 类型实现的所需接口列表中，以便最终调用此函数。

当我们最终准备好调用此函数时，比如在应用程序根目录下，我们手头有所有配置来创建依赖关系，然后我们可以这样做。

```F#
type IDeps =
    inherit IPaymentClient
    inherit ISqlConnection
    inherit IEmailClient

let deps =
    { new IDeps with
        member _.Charge card amount =
            // create PaymentClient and call it

        member _.SendMail address body =
            // create SMTP client and call it

        member _.Query x =
            // create sql connection and invoke it 
            }

let paymentId = chargeUser (UserId "1") 2.50 deps
```

我们使用对象表达式动态实现新的 `IDeps` 接口，使其满足 `chargeUser` 所需的所有推断类型。

## 快速回顾🧑‍🎓

我们首先尝试实现控制反转，以删除硬编码的依赖关系和配置。我们看到，天真地这样做可能会导致函数参数数量的爆炸式增长，并且可以直接在应用中级联。为了解决这个问题，我们从部分应用开始，推迟提供这些参数，直到我们到达应用根，在那里我们有必要的配置。然而，这种解决方案意味着我们无法轻松组合需要依赖关系的函数，当它们需要不同类型的依赖关系时，情况就更加棘手了。

所以我们发明了一个 `inject` 函数，为我们处理了这个管道，并意识到我们实际上发现了一个新版本的 `bind`，从而发现了一种新型的 monad。这个新的单子通常被称为 `Reader`，当你需要组合几个函数时，它很有用，这些函数都需要一些常见环境类型提供的值（或依赖关系）。

如果你想在实践中使用 reader monad，那么你可以在 [FSharpPlus](https://fsprojects.github.io/FSharpPlus/reference/fsharpplus-data-reader.html) 库中找到一个可以滚动的实现。

## 附录

阅读器 monad 的格式在实践中通常与这里的呈现方式略有不同。如果您想了解更多详细信息，请展开下面的部分。

通常在实现 reader monad 时，我们会创建一个新的类型来表示它，称为 `Reader`，以便将其与常规函数类型区分开来。我在上面省略了它，因为在探索概念时，这不是一个重要的细节，但如果你想使用这种技术，那么你可能会遇到这种包装形式的技术。这是一个微不足道的变化，代码看起来就像这样。

```F#
type Reader<'env, 'a> = Reader of ('env -> 'a)

module Reader =
    let run (Reader x) = x
    let map f reader = Reader((run reader) >> f)

    let bind f reader =
        Reader
            (fun env ->
                let a = run reader env
                let newReader = f a
                run newReader env)

    let ask = Reader id

type ReaderBuilder() =
    member _.Return(x) = Reader (fun _ -> x)
    member _.Bind(x, f) = Reader.bind f x
    member _.Zero() = Reader (fun _ -> ())
    member _.ReturnFrom x = x

let reader = ReaderBuilder()

let chargeUser userId amount = 
    reader {
        let! (sqlConnection: #ISqlConnection) = Reader.ask
        let! (paymentClient: #IPaymentClient) = Reader.ask
        let! (emailClient: #IEmailClient) = Reader.ask
        let user = Database.getUser userId sqlConnection
        let paymentId = PaymentProvider.chargeCard user.CreditCard amount paymentClient

        let email =
            EmailBody $"Your payment id is {paymentId}"

        return Email.sendMail user.Email email emailClient
    }
```

`chargeUser` 的返回类型现在是 `Reader<'deps, unit>`，其中 `'deps` 必须像以前一样满足所有标有 `#` 的接口。

在这种情况下，我还必须使用 `Reader.ask` 来实际从环境中获取依赖关系。这样做的原因是，像 `Database.getUser` 这样的函数不会以当前形式返回 `Reader`。我们可以通过执行 `Reader (Database.getUser userId)` 来动态创建 `Reader`，但有时这也可能很麻烦，特别是当我们使用客户端类而不是函数时，这种情况经常发生。因此，在我们的工具包中添加 `ask` 是一种很好的方法，可以掌握依赖关系并在当前范围内显式使用它。



# 8 Grokking Monad Transformers

https://dev.to/choc13/grokking-monad-transformers-3l3

在本系列的早期，在 [Grokking Monads](https://dev.to/choc13/grokking-monads-in-f-3j7f) 中，我们发现 Monads 允许我们抽象出链式计算的机制。例如，在处理可选值时，它们在后台为我们处理故障路径，并让我们可以像数据始终存在一样编写代码。当我们有多个想使用的单子时，会发生什么，我们如何将它们混合在一起？

就像在本系列的其余部分一样，我们将通过解决一个真正的软件设计问题来发明 monad 转换器。最后，我们将看到我们已经发现了 monad 转换器，通过这样做，我们将更直观地理解它。

## 场景

让我们重新审视我们在 [Grokking Monads](https://dev.to/choc13/grokking-monads-in-f-3j7f) 遇到的同样的场景，我们想向用户的信用卡收费。如果用户存在，并且他们的个人资料中保存了信用卡，我们可以向其收费并通过电子邮件向他们发送收据，否则我们将不得不发出没有发生任何事情的信号。然而，这一次，我们将使 `lookupUser`、`chargeCard` 和 `emailReceipt` 函数异步，因为它们调用外部服务。

我们将从以下数据模型和操作开始。

```F#
type UserId = UserId of string

type TransactionId = TransactionId of string

type CreditCard =
    { Number: string
      Expiry: string
      Cvv: string }

type User = 
    { Id: UserId
      CreditCard: CreditCard option }

let lookupUser userId: Async<option<User>>

let chargeCard amount card: Async<option<TransactionId>>

let emailReceipt transactionId: Async<TransactionId>
```

与以前的唯一区别是 `lookupUser`、`chargeCard` 和 `emailReceipt` 现在返回 `Async`，因为实际上它们将调用数据库、外部支付网关并发送消息。

## 我们的首次实现

使用我们从 [Grokking Monads, Imperatively](https://dev.to/choc13/grokking-monads-imperatively-394a) 中学到的知识，我们可能会立即使用 `async` 计算表达式，因为这是我们在这里处理的主要 monad。那么，让我们从这个开始。

```F#
let chargeUser amount userId = 
    async {
        let! user = lookupUser userId
        let card = user.CreditCard
        let! transactionId = chargeCard amount card
        return! emailReceipt transactionId
    }
```

看起来很简单，它抓住了我们需要做的事情的本质，但这是不对的。行 `let card = user.CreditCard` 不会编译，因为此时 `user` 的类型是 `User option`。在写 `chargeCard amount card` 时，我们也遇到了类似的问题，因为我们实际上有一个 `CreditCard option`。

解决这个问题的一种方法是，我们自己开始编写模式匹配逻辑，以访问这些选项中的值，以便我们可以使用它们。让我们看看那是什么样子。

```F#
let chargeUser amount userId =
    async {
        match! lookupUser userId with 
        | Some user -> 
            match user.CreditCard with
            | Some card -> 
                match! chargeCard amount card with
                | Some transactionId -> return! (emailReceipt transactionId) |> Async.map Some
                | None -> return None
            | None -> return None
        | None -> return None
    }
```

这比以前麻烦得多，而且这个函数的相当简单的逻辑被嵌套的模式匹配（即[厄运金字塔](https://en.wikipedia.org/wiki/Pyramid_of_doom_(programming))）所掩盖。我们基本上回到了我们在 [Grokking Monads](https://dev.to/choc13/grokking-monads-in-f-3j7f) 中首次引入这一概念时的情况。似乎一旦我们有多个单子需要处理，外部单子内部的所有东西都必须通过持续的模式匹配再次手动处理故障路径。

## 发明一种新的单子

此时，我们可能会想，我们为什么不发明一个新的单子呢？一个封装了我们希望执行返回可选结果的异步计算的事实。它的行为应该像异步操作失败时的 `async` monad 和异步操作产生丢失数据时的 `option` monad一样。我们称之为 `AsyncOption`。

那么，我们需要做的就是弄清楚如何为这个新的单子实现绑定。让我们从类型开始，然后使用它们来指导我们编写它。在这种情况下，它将具有签名 `(a' -> Async<option<'b>>) -> Async<option<'a>> -> Async<option<'b>>`。

因此，这告诉我们，我们得到了一个函数，它需要一些类型为 `'a` 的值，并将返回一个新的值，该值封装在 `Async<option<_>>` 类型中。我们还得到了这个单子对的一个实例，它封装了一个类型为 `'a` 的值。因此，直观地说，我们需要解包 `Async` 层和 `option` 层，以获得类型为 `'a` 的值，然后将其应用于函数。

```F#
let bind f x =
    async {
        match! x with
        | Some y -> return! f y
        | None -> return None
    }
```

在这里，我们通过使用 `async` 计算表达式实现了这一点。这使我们能够使用 `match!` 它同时打开内部 `option` 上的异步值和模式匹配，以便我们也可以从中提取值。

我们不得不处理三种可能的情况：

1. 如果 `x` 是一个成功的异步计算，返回了 `Some` 值，那么我们可以将函数 `f` 应用于该值。
2. 如果异步操作成功返回 `None`，那么我们只需通过使用 `return` 将 `None` 值包装在新的 `async` 中来传播它。
3. 最后，如果异步计算失败，那么我们只需让 `async` 计算表达式处理并传播该失败，而无需调用 `f`。

因此，使用 `bind` 很容易创建 `asyncOption` 计算表达式，我们可以使用它编写函数。

```F#
let chargeUser amount userId =
    asyncOption {
        let! user = lookupUser userId
        let! card = user.CreditCard
        let! transactionId = chargeCard amount card
        return! emailReceipt transactionId
    }
```

好多了，但鹰眼可能已经发现了我们计划的问题。当我们尝试呼叫 `user.CreditCard` 时，它不起作用。问题在于 `user.CreditCard` 返回一个普通 `option`，我们的 `bind`（因此是 `let!`）被设计为与 `Async<option<_>>` 一起使用。

除此之外，在最后一行，我们也有类似的问题。`emailReceipt` 函数返回一个普通的 `Async<_>`，所以我们不能只写 `return!` 因为它没有生成 `Async<option<_>>`。似乎我们一直需要所有东西都使用完全相同的单子，否则就会出现问题。

## 把自己从洞里抬出来🏋️

解决第一个问题的一个简单方法是将 vanilla `option` 包装在默认的 `Async` 值中。默认的 `Async` 是什么？好吧，我们只想把它当作一个成功的异步计算，可以立即解析，所以让我们写一个名为 `hoist` 的函数，将它的参数包装在一个即时异步计算中。

```F#
let hoist a =
    async {
        return a
    }
```

如果你是一名 C# 开发人员，这就像 `Task.FromResult` 一样，如果你是一名 JavaScript 开发人员，那么它类似于 `Promise.resolve`。

为了解决第二个问题，我们需要一种方法将 `Async` 中的值封装在默认 `option` 值中。在这种情况下，默认 `option` 值将是 `Some`，我们在 [Grokking Functors](https://dev.to/choc13/grokking-functors-bla) 中看到，修改包装值内容的方法是使用 `map`。所以，让我们创建一个名为 `lift` 的函数，它只使用 `Some` 调用 `map`。

```F#
let lift (a: Async<‘a>): Async<option<‘a>> = a |> Async.map Some
```

因此，有了这一点，我们终于可以完成 `chargeUser` 函数。

```F#
let chargeUser amount userId =
    asyncOption {
        let! user = lookupUser userId
        let! card = user.CreditCard |> hoist
        let! transactionId = chargeCard amount card
        return! (emailReceipt transactionId) |> lift
    }
```

现在看起来非常整洁，逻辑清晰可见，不再隐藏在嵌套的错误处理代码中。那么，这就是 monad transformers 的全部功能吗？嗯，不太。。。

## 组合爆炸🤯

为了便于讨论，我们想使用 `Task` 而不是 `Async` 计算。或者，我们现在就开始返回一个 `Result`，而不是一个 `option`。如果我们也想使用 `Reader` 呢？

如果我们需要创建一个新的单子来表示每对单子，你可能会看到所有这些不同单子的组合将如何失控。更不用说我们可能想创建两个以上的组合了。

如果我们能找到一种方法来编写一个通用的单子转换器，那岂不是很好？一个可以让我们组合任何两个单子来创建一个新的单子。让我们看看我们能否发明它。

我们从哪里开始？现在我们知道，要创建一个 monad，我们需要为它实现 `bind`。我们还看到了如何为从 monad 的 `Async` 和 `option` 对创建的新 monad 实现绑定。我们基本上需要做的就是剥离每个单子层，以访问内部单子层中包含的值，然后将该值应用于函数以生成新的单子对。

让我们想象一下，我们有一个通用的 `monad` 计算表达式，它根据被调用的特定单子实例，通过确定要使用哪个版本来调用正确的绑定。有了它，我们应该能够剥离两个单子层，很容易地访问内部值。

```F#
let bindForAnyMonadPair (f: 'a -> 'Outer<'Inner<'b>>) (x: 'Outer<'Inner<'a>>) =
    monad {
        let! innerMonad = x
        monad {
            let! innerValue = innerMonad
            return! f innerValue
        }
    }
```

不幸的是，事实证明这不起作用。问题是，当我们写 `return! f value` 时不太对。在代码的这一点上，我们处于内部 monad 计算表达式的上下文中，因此 `return!` 期望 `f` 返回一个与内部 monad 相同的值，但我们知道它返回 `'Outer<'Inner<'b>>`，因为这是我们新绑定所需要的。

看起来好像有办法解决这个问题。毕竟，我们有我们需要提供给 `f` 的值，所以我们必须能够调用它并以某种方式生成我们需要的值。但是，我们必须记住计算表达式和 `let!` 只是 `bind` 的语法糖。所以我们真正想写的是。

```F#
let bindForAnyMonadPair (f: 'a -> Outer<Inner<'b>>) (x: Outer<Inner<'a>>) =
    x 
    |> bind 
        (fun innerMonad ->
            innerMonad
            |> bind (fun value -> f value))
```

然后（也许）更明显的是，`f` 不能与内部 monad 的 `bind` 一起使用，因为它不会返回正确的类型。因此，我们似乎可以深入挖掘这两个单子，以通用的方式得到值，但我们没有通用的方法将它们重新组合在一起。

## 还有希望🤞

我们可能无法创建一个真正通用的单子转换器，但我们不必完全放弃。如果我们能使这对单子中的哪怕只有一个单子是泛型的，那么这将大大减少我们需要编写的单子组合的数量。

直觉上，你可能会考虑让内心变得通用，我知道我做到了。然而，你会看到，我们陷入了与之前试图使两者通用时完全相同的陷阱，所以这行不通。

在这种情况下，我们唯一的希望就是尝试使外部 monad 通用。让我们假设我们仍然有通用的 `monad` 计算表达式，看看我们是否可以编写一个在内部 monad 是一个 `option` 时都能工作的版本。

```F#
let bindWhenInnerIsOption (f: 'a -> 'Outer<option<'b>>) (x: 'Outer<option<'a>>) =
    monad {
        match! option with
        | Some value -> return! f value
        | None -> return None
    }
```

🙌 它奏效了！我们这次能够成功的原因是我们可以使用 `return!` 调用 `f` 时，因为我们仍处于外部 monad 计算表达式的上下文中。所以 `return!` 能够返回一个 `Outer<option<_>>` 类型的值，这正是 `f` 返回的值。

我们也需要 `hoist` 和 `lift` 的通用版本，但它们并不难写。

```F#
let lift x = x |> map Some

let hoist = result x
```

为了编写 `lift`，我们假设 `Outer` monad 已经为其定义了 `map`，并且该 `map` 可以选择正确的 map，因为此时我们不知道该在哪个 monad 上调用 `map`。

`hoist` 还使用了一个通用的 `result` 函数，该函数是 `return` 的别名，因为 `return` 是 F# 中的保留关键字。从技术上讲，每个 monad 都应该定义 `return` 和 `bind`。我们之前没有提到 `return`，因为它太简单了，但它只是将任何普通值封装在 monad 中。例如，`option` 的 `result` 将是 `Some`。

## 你刚刚发现了 Monad Transformer👏

通过我们发明的 `bind`、`lift` 和 `hoist`，对于内部单子是 `option` 的情况，我们发明了 `option` 单子转换器。通常，这被称为 `OptionT`，实际上被包裹在一个单案例联合中，使其成为一种新类型，我将在附录中展示，但在探索这个概念时，这并不是一个重要的细节。

重要的是要意识到，当你需要处理多个单子时，你不必求助于厄运金字塔。相反，您可以使用 monad 转换器来表示组合，并轻松地从一对现有的 monad 中创建一个新的 monad。请记住，我们是为内部单子定义转换器。

## 考验自己

看看你是否可以为 `Result` 单子实施 `bind`、`lift` 和 `hoist`。

解决方案

```F#
module ResultT =
    let inline bind (f: 'a -> '``Monad<Result<'b>>``) (m: '``Monad<Result<'a>>``) =
        monad {
            match! m with
            | Ok value -> return! f value
            | Error e -> return Error e
        }

    let inline lift (x: '``Monad<'a>``) = x |> map Ok

    let inline hoist (x: Result<'a, 'e>) : '``Monad<Result<'a>>`` = x |> result
```

## 这真的有效吗？😐

当我们为 `OptionT` 发明 `bind` 时，我们想象我们手头有一个功能强大的 `monad` 计算表达式，可以用于任何单子。你可能想知道这样的东西是否存在？尤其是 F# 中是否存在这样的东西。

看来我们需要具备使用泛型的泛型的能力。换句话说，我们需要能够处理任何本身可以包含任何值的单子。这被称为高级类型，你可能知道 F# 不支持它们。

幸运的是，优秀的 [FSharpPlus](https://fsprojects.github.io/FSharpPlus/abstraction-monad.html) 已经找到了一种模拟更高级类型的方法，并且确实定义了这样一个抽象的 `monad` 计算表达式。它还有很多单子转换器，比如 `OptionT`，可以随时使用。

## 我应该使用单子转换器吗？

Monad Transformer 当然非常强大，可以帮助我们从编写大量嵌套代码中恢复过来。另一方面，它们并不是免费的午餐。在使用它们之前，有几件事需要考虑。

1. 如果 monad 堆栈变大，跟踪它本身就会变得相当麻烦。例如，类型可能会变大，跨多个层的提升可能会变得很累。
2. 这是一个将 F# 推向极限的领域。虽然 FSharpPlus 在弄清楚如何模拟更高级的类型方面做得很好，但如果你在使用 monad 转换器时在某个地方出现类型不匹配，它可能会导致非常神秘的编译时错误。
3. 它还可以减缓编译时间，因为它将类型推理推到了它真正设计的范围之外。

在某些情况下，你最好自己定义一个新的单子并为它编写 `bind` 等。如果你的应用程序通常处理相同的单子堆栈，那么改进的编译器错误可能会超过你自己编写代码的相对较小的维护负担。

## 我们学到了什么？🧑‍🎓

我们现在发现，可以将单子组合成新的单子，这让我们在必须编写嵌套模式匹配的情况下编写更整洁的代码。我们还看到，虽然不可能为任何一对创建通用的单子变换器，但至少可以为固定的内部类型定义一个单子变换器。这意味着我们只需要为每个单子编写一个转换器，就可以开始创建更复杂的单子组合。

附录

如上所述，monad 转换器通常有一个新的类型与之关联。下面我将展示 `OptionT` monad 转换器的外观，然后将其与 FSharpPlus 的泛型 `monad` 计算表达式一起用于实现 `chargeUser` 函数。

```F#
#r "nuget: FSharpPlus"

open FSharpPlus

type OptionT<'``Monad<option<'a>>``> = OptionT of '``Monad<option<'a>>``

module OptionT =
    let run (OptionT m) = m

    let inline bind (f: 'a -> '``Monad<option<'b>>``) (OptionT m: OptionT<'``Monad<option<'a>>``>) =
        monad {
            match! m with
            | Some value -> return! f value
            | None -> return None
        }
        |> OptionT

    let inline lift (x: '``Monad<'a>``) = x |> map Some |> OptionT

    let inline hoist (x: 'a option) : OptionT<'``Monad<option<'a>>``> = x |> result |> OptionT

let chargeUser amount userId : Async<option<TransactionId>> =
    monad {
        let! user = lookupUser userId |> OptionT
        let! card = user.CreditCard |> OptionT.hoist
        let! transactionId = (chargeCard amount card) |> OptionT
        return! (emailReceipt transactionId) |> lift
    }
    |> OptionT.run
```

如果你想知道这些类型注释，比如

``` '``Monad<'a>`` ```

那么它们真的只是花哨的标签。我们用过

```  `` ```

引用只是给一个更有意义的名字，以表明它们代表了一些泛型的 `Monad`。这相当于文档，但不幸的是，它并没有真正进行任何有意义的类型检查。就编译器而言，就像任何其他泛型类型一样。我们本可以很容易地编写 `type OptionT<'a> = OptionT of 'a`。因此，在实现这些函数时，我们有责任确保我们确实把它写成一个泛型单子，而不仅仅是任何泛型值。



# 9 Grokking Free Monads

https://dev.to/choc13/grokking-free-monads-9jd

在这篇文章中，我将尝试揭开自由单子（free monads）的神秘面纱，并向您展示它们不是一些奇怪的抽象生物，但事实上对于解决某些问题非常有用。我们的目标不是专注于理论，而是对自由单子有一个坚实的直觉，然后你会发现学习理论要容易得多。因此，为了与本系列的其余部分保持一致，我们将通过解决一个真正的软件问题来发现自由单子。

## 预备知识

我试图让这些帖子尽可能地相互独立，但在这种情况下，你可能需要已经摸索过单子，这一事实并没有多大意义。如果你还没有这样做，那么浏览一下 [Grokking Monads](https://dev.to/choc13/grokking-monads-in-f-3j7f)，一旦你完成了，你就可以在这里继续了。

## 场景

假设我们在一家电子商务商店工作，我们需要实现 `chargeUser` 函数。此函数应接受 `UserId` 和 `amount`。它应该查找用户的个人资料以获得信用卡，然后它应该向用户的卡收取指定的金额。如果用户有电子邮件地址，它应该向他们发送收据。

```F#
type EmailAddress = EmailAddress of string

type Email = 
    { To: EmailAddress
      Body: string }

type CreditCard =
    { Number: string
      Expiry: string
      Cvv: string }

type TransactionId = TransactionId of string

type UserId = UserId of string

type User =
    { Id: UserId
      CreditCard: CreditCard
      EmailAddress: EmailAddress option }

let chargeUser (amount: float) (userId: UserId): TransactionId =
    // TODO: Implement this as part of the domain model
```

我们在这篇文章中的主要目的是能够在我们的域模型中编写 `chargeUser` 函数。通过域模型，我们指的是我们最初编写程序的东西。在这种情况下，因为我们是一家电子商务商店，这意味着我们的域模型包括用户资料、产品和订单等。

通常，当我们编写应用程序时，我们希望保持我们的领域模型与任何基础设施或应用程序层代码完全解耦，因为这些都是我们必须解决的附带复杂性。我们的域模型应该是纯粹和抽象的，因为如果我们使用不同的数据库或不同的云提供商，域模型应该不受影响。

在我们的域层中编写类型来表示模型中的对象很容易，而不会引入任何不必要的耦合，但是 `chargeUser` 等函数呢？一方面，我们知道它需要调用外部服务，那么这是否意味着我们应该在可以访问数据库等的域模型之外定义它呢？另一方面，在这样的功能中做出决定并不罕见，例如我们是否应该通过电子邮件向用户发送收据，这种逻辑肯定感觉像是我们想要独立于数据库进行测试的域逻辑。

## 作为数据发挥作用

有几种方法可以使域操作变得纯粹，不受任何基础设施问题的影响。我们之前在 [Grokking the Reader Monad](https://dev.to/choc13/grokking-monads-in-f-3j7f) 中提到过一个。不过，一种有趣的方法是将函数视为数据。

函数作为数据是什么意思？理解这一点的最好方法是查看一些代码。让我们使用 `chargeUser` 函数并编写一个数据模型来描述它需要执行的操作。

```F#
type ChargeUserOperations =
    | LookupUser of (UserId -> User)
    | ChargeCreditCard of (float -> CreditCard -> TransactionId)
    | EmailReceipt of (Email -> unit)
```

我们创建了一个名为 `ChargeUserOperations` 的类型，它为我们希望作为 `chargeUser` 的一部分执行的每个操作都有一个案例。每个案例都由我们希望它具有的函数签名参数化。因此，我们没有调用函数，而是有一些抽象数据来表示我们想要调用的函数，我们希望这样使用它。

```F#
let chargeUser amount userId: TransactionId =
    let user = LookupUser userId
    let transactionId = ChargeCreditCard amount user.CreditCard
    match user.EmailAddress with
    | Some emailAddress ->
        let email = 
          { To = emailAddress
            Body =  $"TransactionId {transactionId}" }
        EmailReceipt email
        return transactionId
    | None -> return transactionId
```

显然，这行不通。我们不能简单地编写 `LookupUser userId` 并将其分配给 `User` 类型的东西。对于初学者来说，`LookupUser` 期望函数作为参数，而不是 `UserId`。不过，函数作为数据的想法很有趣，所以让我们看看我们是否能找到一种方法来实现它。

试图从数据中提取返回值是没有意义的。我们真正能做的就是创建数据。那么，如果我们创建每个操作，并在其中嵌套另一个操作，就像一个回调函数，它将获取当前计算的输出并产生一个新的输出。像这样的东西。

```F#
type ChargeUserOperation =
    | LookupUser of (UserId * (User -> ChargeUserOperation)
    | ChargeCreditCard of (float * CreditCard * (TransactionId -> ChargeUserOperation)
    | EmailReceipt of (Email * (unit -> ChargeUserOperation)
```

我们在这里做了一些更改。首先，每个操作现在都由元组而不是函数参数化。我们可以将元组视为函数的参数列表。其次，元组中的最后一个参数是我们的回调函数。这意味着，当你创建一个操作时，你应该告诉它下一个要执行的操作需要这个操作的结果。让我们试试这种新格式。

```F#
let chargeUser (amount: float) (userId: UserId): TransactionId =
    LookupUser(
        userId,
        (fun user ->
            ChargeCreditCard(
                (amount, user.CreditCard),
                (fun transactionId ->
                    match user.EmailAddress with
                    | Some emailAddress ->
                        EmailReceipt(
                            { To = emailAddress
                              Body = $"TransactionId {transactionId}" },
                            (fun _ -> // Hmmm, how do we get out of this?)
                        )
                    | None -> // Hmmm, how do we get out of this?)
            ))
    )
```

好的，情况正在好转。我们可以看到，这种数据结构捕获了 `chargeUser` 函数需要做什么的抽象逻辑，而实际上并不依赖于任何特定的实现。唯一的问题是我们没有办法在最后返回值。我们的每个操作都被定义为需要传递另一个回调，那么我们如何发出信号，表明我们实际上应该返回一个值呢？

我们需要的是 `ChargeUserOperation` 中的一个案例，它不需要回调，只需要“返回”一个值。让我们称之为 `Return`。我们还需要使 `ChargeUserOperation` 在返回类型上通用，以封装每个操作返回一些值，但每个操作返回的值可能不同的事实。

```F#
type ChargeUserOperation<'next> =
    | LookupUser of (UserId * (User -> ChargeUserOperation<'next>))
    | ChargeCreditCard of (float * CreditCard * (TransactionId -> ChargeUserOperation<'next>))
    | EmailReceipt of (Email * (unit -> ChargeUserOperation<'next>))
    | Return of 'next
```

我们为泛型参数选择了名称 `'next`，以表示它是链中“next”计算返回的值。在 `Return` 的情况下，它会立即“返回”。我们现在终于可以写 `chargeUser` 了。

```F#
let chargeUser (amount: float) (userId: UserId): ChargeUserOperation<TransactionId> =
    LookupUser(
        userId,
        (fun user ->
            ChargeCreditCard(
                (amount, user.CreditCard),
                (fun transactionId ->
                    match user.EmailAddress with
                    | Some emailAddress ->
                        EmailReceipt(
                            { To = emailAddress
                              Body = $"TransactionId {transactionId}" },
                            (fun _ -> Return transactionId)
                        )
                    | None -> Return transactionId)
            ))
    )
```

就是这样！我们在一个完全抽象的数据结构中捕获了 `chargeUser` 的逻辑。我们知道它不依赖于任何基础设施，因为我们纯粹是用数据类型构建的。通过将计算建模为数据，我们将领域建模提升到了一个新的水平！✅

需要注意的是，`chargeUser` 现在返回 `ChargeUserOperation<TransactionId>`。这可能看起来很奇怪，但我们可以这样想；`chargeUser` 现在是一个生成数据结构的函数，该数据结构表示计费和用户的域操作，并返回 `TransactionId`。

如果你已经摸索了这么远，那么你已经实现了根本的精神飞跃；我们只是将计算表示为数据。这篇文章的其余部分将专门用于清理这个问题，使 `chargeUser` 的读写更容易。事情可能会变得有点抽象，但请记住，我们所做的只是试图构建这个数据结构来表示我们的计算。

## 夷平金字塔⏪

`chargeUser` 当前形式的一个问题是，我们回到了嵌套回调地狱（又名[末日金字塔](https://en.wikipedia.org/wiki/Pyramid_of_doom_(programming))）。我们已经知道 monad 在扁平化嵌套计算方面很有用，所以让我们看看我们是否可以将 `ChargeUserOperation` 变成 monad。

使某物成为单子的秘诀是为该类型实现 `bind`。我们首先定义函数签名的类型，并用它来指导我们。在这种情况下，签名是。

```F#
('a -> ChargeUserOperation<'b>) -> ChargeUserOperation<'a> -> ChargeUserOperation<'b>
```

因此，我们必须解包 `ChargeUserOperation` 以获得值 `'a`，然后将该值应用于我们传递的函数以生成 `ChargeUserOperating<'b>`。让我们陷入困境。

```F#
let bind (f: 'a -> ChargeUserOperation<'b>) (a: ChargeUserOperation<'a>) =
    match a with
    | LookupUser (userId, next) -> ??
    | ChargeCreditCard (amount, card, next) -> ??
    | EmailReceipt (unit, next) -> ??
    | Return x -> f x
```

像往常一样，我们使用模式匹配来解包 `ChargeUserOperation`，以获取内部值。在 `Return` 的情况下，只需对值 `x` 调用 `f` 即可。但对于其他操作呢？我们手头没有 `'a` 类型的值，那么我们如何调用 `f` 呢？

那么，我们接下来要做的是，当提供一个值时，它能够产生一个新的 `ChargeUserOperation`。所以我们能做的就是调用它，并递归地传递这个新的 `ChargeUserOperation` 到 `bind`。这个想法是，通过递归调用 `bind`，我们最终会遇到 `Return` 情况，此时我们可以成功地提取值并对其调用 `f`。

```F#
module ChargeUserOperation =
    let rec bind (f: 'a -> ChargeUserOperation<'b>) (a: ChargeUserOperation<'a>) =
        match a with
        | LookupUser (userId, next) -> LookupUser(userId, (fun user -> bind f (next user)))
        | ChargeCreditCard (amount, card, next) ->
            ChargeCreditCard(amount, card, (fun transactionId -> bind f (next transactionId)))
        | EmailReceipt (email, next) ->
            EmailReceipt(email, (fun () -> bind f (next())))
        | Return x -> f x
```

这可能有点令人费解，但另一种观点是，我们只是在做与之前编写 `chargeUser` 时被迫手动做的完全相同的回调嵌套。除了现在我们已经将嵌套这些操作的行为隐藏在 `bind` 函数中。

每次调用 bind 都会引入另一层嵌套，并将 `Return` 向下推到这个新层中。例如，如果我们编写了 `LookupUser(userId, Return) |> bind (fun user -> ChargeCreditCard(amount, user.CreditCard, Return))`，则相当于以嵌套形式编写它，如 `LookupUser(userId, (fun user -> ChargeCreditCard(amount, user.CreditCard, Return))`。

有了它，我们可以很容易地编写一个名为 `chargeUserOperation` 的计算表达式，并使用它来压平 `chargeUser` 中的金字塔。

```F#
type ChargeUserOperationBuilder() =
    member _.Bind(a, f) = ChargeUserOperation.bind f a
    member x.Combine(a, b) = x.Bind(a, (fun () -> b))
    member _.Return(x) = Return x
    member _.ReturnFrom(x) = x
    member _.Zero() = Return()

let chargeUserOperation = ChargeUserOperationBuilder()

let chargeUser (amount: float) (userId: UserId) =
    chargeUserOperation {
        let! user = LookupUser(userId, Return)
        let! transactionId = ChargeCreditCard((amount, user.CreditCard), Return)

        match user.EmailAddress with
        | Some emailAddress ->
            let email =
                { To = emailAddress
                  Body = $"TransactionId {transactionId}" }

            do! EmailReceipt(email, Return)
            return transactionId
        | None -> return transactionId
    }
```

如果 `do!` 如果不熟悉，那基本上就是 `let!` 除了它忽略了结果。在向他们发送电子邮件时，我们并不关心这一点，因为它无论如何都会返回 `unit`。

## 使数据看起来像函数🥸

该函数现在看起来相当不错，但必须编写 `LookupUser(userId, Return)` 而不仅仅是 `LookupUser userId` 可能有点不自然。必须不断将 `Return` 作为 `ChargeUserOperation` 案例构造函数的最后一个参数也有点烦人。好吧，很容易解决这个问题，我们可以为每个隐藏细节的案例编写一个“智能构造函数”。

```F#
let lookupUser userId = LookupUser(userId, Return)

let chargeCreditCard amount card = ChargeCreditCard(amount, card, Return)

let emailReceipt email = EmailReceipt(email, Return)

let chargeUser (amount: float) (userId: UserId) =
    chargeUserWorkflow {
        let! user = lookupUser userId
        let! transactionId = chargeCreditCard amount user.CreditCard

        match user.EmailAdress with
        | Some emailAddress ->
            do!
                emailReceipt
                    { To = emailAddress
                      Body = $"TransactionId {transactionId}" }

            return transactionId

        | None -> return transactionId
    }
```

🔥 不错！现在，该函数完美地表达了我们操作的逻辑。它看起来就像一个常规的一元函数，除了在幕后，它实际上是在构建一个代表我们所需计算的抽象数据结构，而不是调用任何对真实基础设施的实际调用。

## 析出一个函子

我们的 `chargeUser` 功能现在看起来相当不错，但我们可以对 `ChargeUserOperation` 的定义进行一些优化。让我们考虑一下，如果我们想编写一个不同的计算，会发生什么。我们必须为我们想要支持的每个操作编写一个带有case的数据类型，再加上一个 `Return` case，然后最后为它实现 `bind`。如果我们可以为任何计算类型实现一次 `bind`，那岂不是很好？

让我们再次看看 `ChargeUserOperation` 的 `bind` 定义，看看是否可以将其重构为更通用的东西。

```F#
let rec bind (f: 'a -> ChargeUserOperation<'b>) (a: ChargeUserOperation<'a>) =
    match a with
    | LookupUser (userId, next) -> LookupUser(userId, (fun user -> bind f (next user)))
    | ChargeCreditCard (amount, card, next) ->
        ChargeCreditCard(amount, card, (fun transactionId -> bind f (next transactionId)))
    | EmailReceipt (email, next) ->
        EmailReceipt(email, (fun () -> bind f (next ())))
    | Return x -> f x
```

如果我们强制每个操作必须采用 `Operation of ('inputs * (‘output -> Operation<'next>)` 的形式，那么它们只在参数类型上有所不同，我们可以将其设为泛型。不过，我们应该如何为 `ChargeCreditCard` 做到这一点，因为目前有两个输入。好吧，我们可以将输入组合成一个元组，比如 `ChargeCreditCard of ((float * CreditCard) * (TransactionId -> ChargeUserOperation<'next>))`。

每个操作的 `bind` 形式现在都是相同的，具体来说，它是 `Operation(inputs, next) -> Operation(inputs, (fun output -> bind f (next output))`。所以说真的，我们实际上只需要考虑两种情况，要么是 `Operation`，要么是 `Return`。因此，让我们创建一个名为 `Computation` 的类型来封装它。

```F#
type Computation<'op, 'next> =
    | Operation of 'op
    | Return of 'next
```

我们可以编写 `bind` 来将其转换为monad。

```F#
let rec inline bind (f: 'a -> Computation< ^op, 'b >) (a: Computation< ^op, 'a >) =
    match a with
    | Operation op -> Operation(op |> map (bind f))
    | Return x -> f x
```

在 `Operation` 案例中实现这一点的诀窍是要注意，我们要求每个 `Operation` 都是可映射的。也就是说，我们要求它是一个 functor。映射操作只是将函数应用于返回值以将其转换为其他值的情况。因此，通过递归调用 `bind f`，就像我们为这个 `ChargeUserOperation` 编写代码时所做的那样，我们最终遇到了 `Return` 的情况，获得了对返回值的访问权限，并通过调用 `map` 将当前 `op` 应用于它。

因此，现在当我们编写操作时，我们已经将必须实现 `bind` 的任务减少到必须实现 `map`，这是一项更容易的任务。例如，我们可以这样表示 `ChargeUserOperation`。

```F#
type ChargeUserOperation<'next> =
    | LookupUser of UserId * (User -> 'next)
    | ChargeCreditCard of (float * CreditCard) * (TransactionId -> 'next)
    | EmailReceipt of Email * (unit -> 'next)
    static member Map(op, f) =
        match op with
        | LookupUser (x, next) -> LookupUser(x, next >> f)
        | ChargeCreditCard (x, next) -> ChargeCreditCard(x, next >> f)
        | EmailReceipt (x, next) -> EmailReceipt(x, next >> f)
```

不幸的是，我们无法在 F# 中消除更多的样板。在 Haskell 等其他语言中，可以自动为操作 functor 导出 `Map` 函数，但在 F# 中，使用 FSharpPlus，我们今天能做的最好的事情就是自己编写 `static member Map`。然后，FSharpPlus 为我们提供了 `map` 函数，当通过使用静态解析的类型参数映射 `ChargeUserOperation` 的实例时，该函数将通过调用此 `static member Map` 自动选择正确的一个。

我们只需要对智能构造函数进行最后一次更改。现在 `ChargeUserOperation` 只是一个 functor，我们需要通过将它们包装在一个 `Operation` 中，将它们提升到 `Computation` 单子中。

```F#
let lookupUser userId = LookupUser(userId, Return) |> Operation

let chargeCreditCard amount card =
    ChargeCreditCard((amount, card), Return) |> Operation

let emailReceipt email =
    EmailReceipt(email, Return) |> Operation

let chargeUser (amount: float) (userId: UserId) =
    computation {
        let! user = lookupUser userId
        let! transactionId = chargeCreditCard amount user.CreditCard
        match user.EmailAddress with
        | Some emailAddress ->
            do!
                emailReceipt
                    { To = emailAddress
                      Body = $"TransactionId {transactionId}" }

            return transactionId
        | None -> return transactionId
    }
```

## 你刚刚发现了自由单子🥳

我们称之为 `Computation` 的数据类型通常称为 `Free`，`Operation` 情况通常称为 `Roll`，`Return` 情况通常称之为 `Pure`。除此之外，我们还发现了自由单子的基础。它只是一个数据类型和相关的 `bind` 函数，从根本上描述了顺序计算。

如果你是一名 C# 开发人员，并且熟悉 LINQ，那么这对你来说可能很熟悉。LINQ 提供了一种构建计算并将其计算推迟到稍后某个时间的方法。这使得 LINQ 能够在不同的环境中运行，例如在数据库中，因为人们能够为它编写解释，将 LINQ 语句转换为数据库服务器上的 SQL 等。

## 我应该使用自由单子吗🤔

您可能想知道是否在您的项目中使用 F# 中的自由单子。一方面，当涉及到在域模型中定义计算时，它们提供了一种极好的抽象方法。测试它们也是一种乐趣，因为我们可以将它们解释为纯数据，并验证对于给定的输入集，我们是否产生了正确的数据结构，从而进行了计算；不需要再 mocking 了🙌.

另一个优点是，使用自由单子，我们实际上实现了面向对象程序员所说的接口隔离原则。每个计算只能访问它完成工作所需的操作。不再向域处理程序注入宽接口，然后必须编写测试来验证我们没有调用错误的操作；在这种设计下，这简直是不可能的！

另一方面，它似乎将 F# 推向了极限，因为它在技术上需要更高级的类型等特性，而 F# 在技术上并不支持这些特性。因此，我们不得不大量使用静态解析的类型参数来使其工作。你可能会发现它们非常抽象，尽管我希望这篇文章至少有助于使它们的使用看起来更直观，即使内部实现仍然非常抽象。

总的来说，我认为这里没有一个放之四海而皆准的答案。你将不得不权衡你的项目和团队的利弊，并决定这种纯度是否值得，以便在事情不一致时克服初始学习曲线和潜在的隐性编译器错误。

如果你想冒险一试，那么我建议你使用 [FSharpPlus](https://fsprojects.github.io/FSharpPlus/reference/fsharpplus-data-free.html)，它为你定义了自由单子机制。另请参阅末尾的附录，了解使用 FSharpPlus 的完整示例。

## 我们学到了什么🧑‍🎓

free monad 这个名字起初可能很神秘，甚至会产生误导，但这个概念相对简单。自由单子只是一种数据结构，它代表了一系列应该按顺序运行的计算。通过构建数据结构，我们可以将其留给其他人，让他们以他们认为合适的方式来解释它。只要他们尊重我们交给他们的数据结构中的计算顺序，他们就可以“自由”地按照自己的需要去做。

自由单子只是我们用非常抽象的术语描述计算的一种方式。我们对计算必须做的事情施加了尽可能少的限制，并且对应该如何做没有做出任何假设。我们已经将“什么”与“如何”完全解耦，这是良好的领域驱动设计的基本支柱之一，因为这意味着领域模型是手头问题的纯粹抽象表示，不受其托管细节的影响。

## 下次⏭

我们在这篇文章中已经介绍了很多，但我们还没有讨论我们实际上是如何运行这些计算的。到目前为止，我们只是在数据中构建了它们的一些抽象表示。下一次，我们将看看如何实际解释它们以进行一些实际工作。

## 附录

如果你想看到一个完整的、自上而下的、使用 FSharpPlus 编写自由单子工作流的示例，那么我在下面的部分中包含了一个。

```F#
#r "nuget: FSharpPlus"

open FSharpPlus
open FSharpPlus.Data

type ChargeUserOperation<'next> =
    | LookupUser of UserId * (User -> 'next)
    | ChargeCreditCard of (float * CreditCard) * (TransactionId -> 'next)
    | EmailReceipt of TransactionId * (TransactionId -> 'next)
    static member Map(op, f) =
        match op with
        | LookupUser (x, next) -> LookupUser(x, next >> f)
        | ChargeCreditCard (x, next) -> ChargeCreditCard(x, next >> f)
        | EmailReceipt (x, next) -> EmailReceipt(x, next >> f)

let lookupUser userId = LookupUser(userId, id) |> Free.liftF

let chargeCreditCard amount card =
    ChargeCreditCard((amount, card), id) |> Free.liftF

let emailReceipt email =
    EmailReceipt(email, id) |> Free.liftF

let chargeUser (amount: float) (userId: UserId) =
    monad {
        let! user = lookupUser userId
        let! transactionId = chargeCreditCard amount user.CreditCard
        match user.EmailAddress with
        | Some emailAddress ->
            do!
                emailReceipt
                    { To = emailAddress
                      Body = $"TransactionId {transactionId}" }

            return transactionId
        | None -> return transactionId
    }
```

在这里编写智能构造函数时，例如 `lookUser`，我们传递恒等元函数 `id` 作为第二个参数。这是因为 `Free.liftF` 将 functor 映射为 `Pure`，然后用 `Roll` 将其提升。因此，通过使用 `id` 然后编写 `Free.liftF`，我们最终得到了所需的 `Roll (LookupUser(userId, Pure))`。这里思考 `id` 的另一种方式是，创建操作时的默认“回调”只是返回此计算产生的值，而不做任何其他事情。



# 10 Interpreting Free Monads

https://dev.to/choc13/interpreting-free-monads-3l3e

在本系列的最后一篇文章中，我们[探索了 Free Monads](https://dev.to/choc13/grokking-free-monads-9jd)，发现它们为我们提供了一种仅使用数据巧妙构建计算抽象表示的方法。当我们编写领域模型时，这一切都很好，但最终我们需要做一些真正的计算来运行副作用并产生结果。在这篇文章中，我们将学习如何为自由单子编写解释器；首先是一个在应用程序上下文中运行计算的解释器，然后是一个不同的解释器，它允许我们为域操作编写“无模拟（mockless）”单元测试。

## 扼要重述🧢

让我们快速提醒自己上次我们解决的问题。我们想编写一个 `chargeUser` 函数，通过用户的 id 查找用户，然后向他们的卡收取指定的金额，最后如果他们的个人资料上有电子邮件地址，则通过电子邮件向他们发送收据。这是我们使用的域模型。

```F#
type EmailAddress = EmailAddress of string

type Email = 
    { To: EmailAddress
      Body: string }

type CreditCard =
    { Number: string
      Expiry: string
      Cvv: string }

type TransactionId = TransactionId of string

type UserId = UserId of string

type User =
    { Id: UserId
      CreditCard: CreditCard
      EmailAddress: EmailAddress option }
```

然后，我们编写了一个有区别的联合来表示我们需要在 `chargeUser` 计算中执行的操作，看起来像这样。

```F#
type ChargeUserOperation<'next> =
    | LookupUser of UserId * (User -> 'next)
    | ChargeCreditCard of (float * CreditCard) * (TransactionId -> 'next)
    | EmailReceipt of Email * (unit -> 'next)
    static member Map(op, f) =
        match op with
        | LookupUser (x, next) -> LookupUser(x, next >> f)
        | ChargeCreditCard (x, next) -> ChargeCreditCard(x, next >> f)
        | EmailReceipt (x, next) -> EmailReceipt(x, next >> f)
```

最后，在一些智能构造函数的帮助下，我们可以编写 `chargeUser` 的抽象版本，它构建了一个自由单子来表示我们最终想要执行的计算。

```F#
#r "nuget: FSharpPlus"

open FSharpPlus
open FSharpPlus.Data

let lookupUser userId = LookupUser(userId, id) |> Free.liftF

let chargeCreditCard amount card =
    ChargeCreditCard((amount, card), id) |> Free.liftF

let emailReceipt email = EmailReceipt(email, id) |> Free.liftF

let chargeUser (amount: float) (userId: UserId) =
    monad {
        let! user = lookupUser userId
        let! transactionId = chargeCreditCard amount user.CreditCard

        match user.EmailAddress with
        | Some emailAddress ->
            do!
                emailReceipt
                    { To = emailAddress
                      Body = $"TransactionId {transactionId}" }

            return transactionId

        | None -> return transactionId
    }
```

我在这里使用 `FSharpPlus` 做了几件事：

1. `monad` 计算表达式，它只是一个适用于任何 monad 的通用计算表达式。
2. `Free` 数据类型，使用 `Roll` 和 `Pure` 名称代替我们上次创建的 `Operation` 和 `Return` 名称。它还为我们提供了 `Free.liftF`，意思是“提升 functor”，它将操作 functor 提升到自由单子。正如我们上次发现的，只要我们的操作是可映射的，我们总是可以将它们提升到一个自由单子中。

## 一个平凡的解释器

我们之前在编写域模型时的目标是从域模型中删除对基础设施函数的“真实”调用。我们现在将把重点转移到编写应用层上，在那里我们想真正做“真正的”工作。

应用层负责接收外部请求（例如通过 REST API），然后调用域模型来处理它们。它应该能够“连接”实际的基础设施，以实现像 `LookupUser` 这样的操作。因此，我们的工作是在域模型中获取 `chargeUser` 输出的抽象数据结构，并将其转化为具有真实副作用的真实计算——这是解释器的工作。

我们需要做的是编写一个名为 `interpret` 的函数，该函数从 `Free<ChargeUserOperation<TransactionId>, TransactionId>` 变为 `TransactionId`。这可能看起来有点棘手，所以让我们先写下 `chargeUser` 的“长臂”，以便更清楚地了解我们需要解释的是什么。

```F#
let chargeUser amount userId =
    Roll(
        LookupUser(
            userId,
            fun user ->
                Roll(
                    ChargeCreditCard(
                        (amount, user.CreditCard),
                        fun transactionId ->
                            match user.EmailAddress with
                            | Some emailAddress ->
                                Roll(
                                    EmailReceipt(
                                        { To = emailAddress
                                          Body = $"TransactionId {transactionId}" },
                                        fun () -> Pure transactionId
                                    )
                                )
                            | None -> Pure transactionId
                    )
                )
        )
    )
```

希望现在我们已经删除了 `monad` 计算表达式的语法糖，编写 `interpret` 的策略更清晰了。我们可以看到，我们需要在 `Roll operation` 上递归进行模式匹配，直到达到 `Pure value` 情况。让我们试试看。

```F#
let rec interpret chargeUserOutput : TransactionId =
    match chargeUserOutput with
    | Roll op ->
        match op with
        | LookupUser (userId, next) ->
            let user = // some hard coded user value
            user |> next |> interpret
        | ChargeCreditCard ((amount, card), next) ->
            TransactionId "123" |> next |> interpret
        | EmailReceipt (email, next) -> 
            () |> next |> interpret
    | Pure x -> x
```

正如我们计划的那样，如果我们在 `Roll` 案例中进行匹配，那么我们打开一个 `ChargeUserOperation`，然后我们可以再次进行模式匹配。当我们匹配一个操作时，我们唯一能做的明智的事情就是使用它所期望的输入类型调用 `next`（例如，`LookupUser` 中的 `next` 想要一个 `User`）。这为我们生成了另一个 `Free<_>`，因此我们递归调用 `interpret` 来解释该结果，直到我们遇到 `Pure` 情况，我们就可以返回结果。*注意，如果你想玩这段代码，那么你实际上需要编写 `match Free.run chargeUserOutput`，因为 FSharpPlus 在内部实现了 Free。*

在这个例子中，我们只是硬编码了一堆值，所以我们仍然没有做任何“真正的”工作，但我们现在至少可以在 F# REPL 中测试一下，看看会发生什么。

```F#
> chargeUser 1.0 (UserId "1") |> interpret;;                                                            
val it : TransactionId = TransactionId "123"
```

👌 太好了，它生成了硬编码的 `TransactionId`。

从这个简单的 `interpret` 实现中可以看出，外部模式匹配根本不依赖于操作类型。所以，让我们看看我们是否可以重构 `interpret`，使其适用于任何类型的操作。

```F#
let rec inline interpret interpretOp freeMonad =
    match freeMonad with
    | Roll op -> 
        let nextFreeMonad = op |> interpretOp 
        interpret interpretOp nextFreeMonad
    | Pure x -> x
```

在 `Roll` 的情况下，我们现在将解释操作的工作委托给 `interpretOp` 函数。使用 `op` 调用此函数会返回嵌套回调，这是另一个自由 monad，然后我们可以将其传递回解释器。例如，让我们写出 `interpretChargeUserOp`，这只是我们上面第一个 `interpret` 函数的内部模式匹配。

```F#
let interpretChargeUserOp op =
    match op with
    | LookupUser (userId, next) ->
        let user = // some hard coded user value
        user |> next
    | ChargeCreditCard ((amount, card), next) -> 
        TransactionId "123" |> next
    | EmailReceipt (email, next) -> 
        () |> next
```

这很好，因为我们现在有了一个通用的函数来解释任何自由的单子。我们只需要为它提供一个解释特定用例操作的功能。我们现在可以这样解释 F# REPL 中 `chargeUser` 生成的自由单子。

```F#
> chargeUser 1.0 (UserId "1") |> interpret interpretChargeUserOp;;                                                            
val it : TransactionId = TransactionId "123"
```

为了更具体地说明这一点，让我们逐步了解当我们解释第一个操作时会发生什么：

1. `interpret` 函数看到 `Roll(LookupUser(...))`，因此它在 `Roll` 上匹配。
2. 然后，它要求 `interpretChargeUserOp` 处理 `LookupUser`，并通过模式匹配来处理。1.在我们的小例子中，这只是将一个硬编码的用户传递给 `next`，我们知道（通过写 `chargeUser` 为“长手牌”）`next` 将是 `Roll(ChargeCreditCard(...))`。
3. 因此，当控制返回 `interpret` 时，它将递归地将 `Roll(ChargeCreditCard(...))` 与相同的 `interpretChargeUserOp` 函数一起传递回自身。
4. 这将继续，直到它找到一个只继续 `Pure` 的操作。🥵

如果你遵循了这一点，那么你就已经摸索过了！我们现在可以继续为我们的应用程序编写一个合适的解释器🙌

## 现实世界的解释器

受够了这些抽象的废话，我们有一个应用程序要发布。所以，让我们坚持下去，写一个真正的解释器，做一些有用的事情。我们将假设我们已经在一些适当的项目或库的其他地方定义了以下基础设施代码。

```F#
module DB =
    let lookupUser userId =
        async {
            // Create a DB connection
            // Query the DB for the user
            // Return the user
        }

module PaymentGateway =
    let chargeCard amount card = 
        async {
            // Perform an async operation to charge the card
            // Return the transaction id
        }

module EmailClient = 
    let send email = async { // send the email }
```

这些已经写好了，为我们的操作编写解释器是一项简单的工作。

```F#
let interpretChargeUserOp (op: ChargeUserOperation<'a>): Async<'a> =
    match op with
    | LookupUser (userId, next) ->
        async {
            let! user = DB.lookupUser userId
            return user |> next
        }
    | ChargeCreditCard ((amount, card), next) ->
        async {
            let! transactionId = PaymentGateway.chargeCard amount card
            return transactionId |> next
        }
    | EmailReceipt (email, next) ->
        async {
            do! EmailClient.send email
            return () |> next
        }
```

从 `interpretChargeUserOp` 的类型可以看出，我们正在将每个域操作转换为 `Async` 操作，这正是我们在开始自由单子航行时想要实现的分离。我们的域模型甚至不需要知道实际上操作将是异步的，唯一的要求是它们是单子性的。应用程序可以自由选择它真正想要使用的单子，它可以很容易地使用 `Task`。

我们快到家了，我们只需要确保这会产生正确的结果。我们尝试编写 `chargeUser 1.0 (UserId "1") |> interpret interpretChargeUserOp`，但编译器说没有！发生什么事了？

如果我们更仔细地观察 `interpret` 的签名，我们会发现它期望 `interpretOp` 返回一个 `Free<_>`。问题是我们现在从 `interpretChargeCardOp` 返回 `Async<_>`。一般来说，我们希望将我们的操作解释为其他单子，如 `Async`、`Task`、`State` 等，而不仅仅是普通值，因为这些操作会产生副作用。所以我们需要做一个小的改变来 `interpret`，我们现在希望它具有以下签名。

```F#
let rec inline interpret
    (interpretOp: '``Functor<'T>`` -> '``Monad<'T>``)
    (freeMonad: Free<'``Functor<'U>``, 'U>)
    : '``Monad<'U>``
```

也就是说，给定一个函数，可以将包含 `T` 类型值的 functor（我们的操作是一个 functor）转换为包含相同 `T` 类型的 `Monad`，然后我们可以使用它将基于这些操作的自由 Monad 转换为不同的 Monad。为了实现这一点，我们现在必须修复 `Roll` 的情况，在将 `interpretOp` 产生的 monad 传递回递归调用进行 `interpret` 之前，对其进行解包。

```F#
let rec inline interpret
    (interpretOp: '``Functor<'T>`` -> '``Monad<'T>``)
    (freeMonad: Free<'``Functor<'U>``, 'U>)
    : '``Monad<'U>`` =
    match freeMonad with
    | Roll op ->
        monad {
            let! nextFreeMonad = interpretOp op
            return! interpret interpretOp nextFreeMonad
        }
    | Pure x -> monad { return x }
```

我们还必须稍微改变 `Pure`，这样它基本上也会将值提升到目标 monad 类型。例如，如果我们试图将自由 monad 转换为异步计算，你可以这样阅读上面的内容。

```F#
let rec inline interpretAsync
    (interpretOp: '``Functor<'T>`` -> Async<_>)
    (freeMonad: Free<'``Functor<'U>``, 'U>)
    : Async<_> =
    match freeMonad with
    | Roll op ->
        async {
            let! nextFreeMonad = interpretOp op
            return! interpretAsync interpretOp nextFreeMonad
        }
    | Pure x -> async { return x }
```

最后，让我们确保这个新的解释器现在可以工作。

```F#
> chargeUser 1.0 (UserId "1")
-     |> interpret interpretChargeUserOp
-     |> Async.RunSynchronously;;
val it : TransactionId = TransactionId "123"
```

## 你刚刚发现了 `Free.fold`📃

我们一直称之为 `interpret` 的实际上是 `Free` 数据类型的 `fold` 函数。它将数据结构的所有抽象 `Roll opertion` 元素替换为调用函数 `f` 的结果，函数 f 在特定的单子中运行操作函数，如 `Async`。我们在这里看到了如何在应用层使用它，将抽象域计算转换为对数据库的“真实”调用等。不过，乐趣并不止于此，因为我们已经将“什么”与“如何”解耦，我们可以用很多不同的方式来解释我们的领域模型，让我们来看看另一种有用的折叠方法。

## 测试解释器🧪

让我们想象一下，我们想验证 `chargeUser` 只有在用户有电子邮件地址的情况下才会发送电子邮件。我们如何测试这个？当然，我们可以编写一个不同的解释器，让我们跟踪 `EmailReceipt` 案例在计算中出现的次数。

我们需要解释器做几件事：

1. 从 `LookupUser` 返回一个特定的 `User` 对象，以便我们可以控制配置文件上是否有 `EmailAddress`。
2. 计算 `EmailReceipt` 在计算中出现的次数。

处理第 1 点很容易，因为我们在写的第一个简单的解释器中已经看到了如何返回硬编码值。那么第二点呢？好吧，我们知道我们可以将我们的自由单子解释为任何其他单子，所以为什么不选择一个让我们跟踪某个状态的单子，其中该状态是 `EmailReceipt` 被调用次数的计数器。为此，我们可以使用 `Writer` monad，它只是一个monad，让我们更新通过整个计算传递的一些值。使用它，我们的测试可能看起来像这样。

```F#
module Tests =
    let shouldOnlySendReceiptWhenUserHasEmailAddress user =
        let output =
            chargeUser 1. (UserId "1")
            |> Free.fold
                (function
                | LookupUser (_, next) -> 
                    monad {
                        return user |> next
                    }
                | ChargeCreditCard (_, next) ->
                    monad {
                        return TransactionId "123" |> next
                    }
                | EmailReceipt (_, next) ->
                    monad {
                        do! Writer.tell ((+) 1)
                        return () |> next
                    })

        Writer.exec output 0
```

对于 `LookupUser` 操作，我们只需将传递给此测试函数的 `user` 参数传递给 `next`。这使我们能够精确控制特定测试运行中使用的用户。在这里解释 `ChargeCreditCard` 很无聊，我们只是硬编码一个 `TransactionId` 并将其传递给 `next`，因为我们对这个测试的那部分不感兴趣。在 `EmailReceipt` 中，我们使用 `Writer` monad 来递增计数器，以跟踪已对 `EmailReceict` 进行调用的事实。最后，我们调用 `Writer.exec output 0` 来运行初始计数为 `0` 的计算。

让我们在 REPL 中尝试一下，看看会得到什么结果。

```F#
> let userNoEmail =                                       
-     { Id = UserId "1"
-       EmailAddress = None
-       CreditCard =
-           { Number = "1234"
-             Expiry = "12"
-             Cvv = "123" } };;

> Tests.shouldOnlySendReceiptWhenUserHasEmailAddress userNoEmail;;
val it : int = 0
```

✅ 当用户没有电子邮件地址时，呼叫计数为 `0`。

```F#
> let userWithEmail =
-     { Id = UserId "1"
-       EmailAddress = Some(EmailAddress "a@example.com")
-       CreditCard =
-           { Number = "1234"
-             Expiry = "12"
-             Cvv = "123" } };;

> Tests.shouldOnlySendReceiptWhenUserHasEmailAddress userWithEmail;;  
val it : int = 1
```

✅ 当用户确实有电子邮件地址时，呼叫计数为 `1`。

## 但我不需要单子🤷‍♀️

有时，特别是在测试时，您不需要将操作解释为单子。例如，您可能只想根据一些输入或从数据库调用返回的数据来检查计算的输出。在这种情况下，您可以使用 `Identity` 单子。让我们看看那是什么样子。

```F#
let shouldReturnTransactionIdFromPayment user =
    let output =
        chargeUser 1. (UserId "1")
        |> Free.fold
            (function
            | LookupUser (_, next) ->
                monad {
                    return user |> next
                }
            | ChargeCreditCard (_, next) ->
                monad {
                    return TransactionId "1" |> next
                }
            | EmailReceipt (_, next) ->
                monad {
                    return () |> next
                })

    output |> Identity.run
```

## 我们学到了什么？🤓

我们已经看到，自由单子提供了一种将“什么”与“如何”解耦的极好方法。它们允许我们以声明方式编写域模型计算，并将如何完成计算的解释留给应用程序的另一层。在这篇文章中，我们了解到我们可以使用 `fold` 来解释一个自由单子，我们所要做的就是为它提供一个函数，将我们的特定操作解释为某个目标单子。

在测试我们的领域模型时，这种解耦尤其强大，因为它使我们能够精确控制被测试的函数。我们不必担心测试套件中的 `async` 调用，因为我们可以选择在测试中同步解释计算。它甚至消除了对任何模拟框架的需要，因为对我们来说，检查特定操作的调用次数或验证特定参数是否提供给函数都是轻而易举的。

自由单子可能是抽象的，但我们的领域模型和这两者应该很好地结合在一起。



# 11 Grokking Lenses

https://dev.to/choc13/grokking-lenses-2jgp

在大多数函数式编程语言中，数据结构默认情况下是不可变的，这很好，因为不可变性消除了我们代码中的一大堆问题，让我们的大脑可以担心我们试图解决的更高层次的问题。不变性的一个缺点是修改嵌套数据结构有多么麻烦。在这篇文章中，我们将独立发现一种更好的“更新”不可变数据的方法，并在此过程中重新发明镜头。

## 场景

在这篇文章中，我们将想象我们正在使用以下数据模型。

```F#
type Postcode = Postcode of string

type Address =
    { HouseNumber: string
      Postcode: Postcode }

type CreditCard =
    { Number: string
      Expiry: string
      Cvv: string
      Address: Address }

type User = { CreditCard: CreditCard }
```

因此，`User` 有一张有 `Address` 的 `CreditCard`。现在想象一下，我们被要求编写一些代码，让用户更新他们的信用卡地址的邮政编码。很容易，对吧？

```F#
let setCreditCardPostcode postcode user =
    { user with
          CreditCard =
              { user.CreditCard with
                    Address =
                        { user.CreditCard.Address with
                              Postcode = postcode } } }
```

诶呀这可不好看。将其与 C# 等命令式版本进行比较。

```F#
public static User SetCreditCardPostcode(User user, Postcode postcode)
{
    user.CreditCard.Address.Postcode = postcode;
    return user;
}
```

好吧，数据模型可能是可变的，方法声明中可能会有更多的问题，但很难反驳这样一个事实，即实际的集合操作在命令式风格中要清晰得多。

## 制定解决方案🎼

本能地，我们想做的是编写函数来设置模型中各自的位，然后在我们想要设置嵌套在更大结构中的数据时组合它们。例如，让我们在各自的模块中为 `Address`、`CreditCard` 和 `User` 编写一些设置程序。

```F#
module Address =
    let setPostcode postcode address = 
        { address with Postcode = postcode }

module CreditCard =
    let setAddress address card =
        { card with Address = address }

module User =
    let setCreditCard card user =
        { user with CreditCard = card }
```

*为了简洁起见，我省略了为每个属性编写 setter。*

这些函数很好，因为它们非常专注于数据模型的单个部分。理想情况下，要编写 `setCreditCardPostcode`，我们可以组合这些单独的函数来创建一个新函数，该函数可以更新用户信用卡内的邮政编码。像这样的东西。

```F#
let setCreditCardPostcode: (Postcode -> User -> User) =
    Address.setPostcode
    >> CreditCard.setAddress
    >> User.setCreditCard
```

*旁白：`>>` 是函数组合运算符，因此 `f >> g` 等价于 `fun x -> x |> f |> g`。更具体地说，如果我们`let addOne x = x + 1`，那么我们可以写 `let addTwo = addOne >> addOne`。*

但是。。。它不会编译！问题是 `Address.setPostcode` 有签名 `Postcode -> Address -> Address` 和 `CreditCard.setAddress` 有签名 `Address -> CreditCard -> CreditCard`。因此，当我们编写 `Address.setPostcode >> CreditCard.setAddress` 时，`Address.setPostCode` 的输出（即 `Address -> Address`）与 `CreditCard.sedAddress` 的输入（即 `Address`）不匹配。

## 对齐类型

我们的第一次尝试虽然不太正确，但非常接近。类型几乎排成一列。让我们看看是否可以对齐这些类型，以便一个设置器的输出可以直接输入到下一个的输入中。

如果我们再次查看 `Address.setPostcode postcode` 的输出，我们会看到它是一个签名为 `Address -> Address` 的函数。也就是说，当我们部分地将 `postcode` 应用于 `setPostcode` 时，它会创建一个函数，通过将 `Postcode` 属性设置为我们部分应用的值来转换地址。那么，如果我们将 `CreditCard.setAddress` 的输入更改为接受地址转换函数，而不仅仅是一个新的地址值，怎么样。事实上，`CreditCard.setAddress` 并没有什么特别之处，所以让我们对所有的 setter 进行更改。

```F#
module Address =
    let setPostcode (transformer: Postcode -> Postcode) address =
        { address with
              Postcode = address.Postcode |> transformer }

module CreditCard =
    let setAddress (transformer: Address -> Address) card =
        { card with
              Address = card.Address |> transformer }

module User =
    let setCreditCard (transformer: CreditCard -> CreditCard) user =
        { user with
              CreditCard = user.CreditCard |> transformer }
```

你可能会觉得这是一个让组合工作的 hack，但我们实际上做的是一个更强大的“设置器”函数。每个“设置器”现在都可以接受任何应用于当前值的转换函数，然后返回经过此修改的新版本数据。如果我们仔细想想，设置属性只是这种更一般的转换的一个特例，我们忽略了现有的值。

我们在这里实际创建的更像是属性修饰符，而不仅仅是 setter。每个修饰符都有签名 `('child -> 'child) -> ('parent -> 'parent)`，这意味着给定一个可以修改某些子属性的函数，然后我将返回一个更新父类型的函数。因此，让我们将它们重命名为 `modifyX`，看看我们现在是否可以按照我们想要的组合样式创建 `setCreditCardPostcode`。

```F#
let setCreditCardPostcode: (Postcode -> User -> User) =
    Address.modifyPostcode
    >> CreditCard.modifyAddress
    >> User.modifyCreditCard
```

嗯，还是不太对。`setCreditCardPostcode` 的类型实际上是 `(Postcode -> Postcode) -> (User -> User)`，事后看来很明显，因为我们所做的只是组合修饰符，而不是 setter。所以我们实际上刚刚在这里创建了一个新的“修饰符”，可以修改用户信用卡的邮政编码属性。为了进行“set”操作，我们只需将执行“set”的转换应用于“修饰符”。

```F#
let setCreditCardPostcode (postcode: Postcode): User -> User =
    (Address.modifyPostcode
     >> CreditCard.modifyAddress
     >> User.modifyCreditCard)
        (fun _ -> postcode)
```

因此，我们组合修饰符，然后使用一个转换器部分应用它，该转换器忽略输入并将值设置为提供的 `postcode`。

如果你已经跟进到这一点，那么你已经探索了核心原则，即如果我们有知道如何更新模型中一部分的修饰符函数，那么我们可以将它们链接在一起，构建在更大数据结构的许多嵌套层上运行的修饰符。从现在开始，接下来的一切都只是整理和提取通用部分。

## 通用属性修改器

从 `setCreditCardPostcode` 的最后一个实现中可以清楚地看出，为了设置嵌套属性，我们做了两件事。

1. 编写必要的属性修饰符，以创建一个可以跨嵌套数据结构的许多层操作的属性修饰符。
2. 应用一个转换函数，忽略当前值，只返回我们想要设置属性的新值。

假设我们所有的属性修饰符都是 `('child -> 'child) -> ('parent -> 'parent)` 的形式，我们应该能够编写一个适用于任何修饰符的 `set` 函数。这真的很简单，看起来就是这样。

```F#
let set modifier (value: 'child) (parent: 'parent) =
    modifier (fun _ -> value) parent
```

我们甚至可以在 `User` 模块中定义 `modifyCreditCardPostcode`。

```F#
module User =
    let modifyCreditCardPostcode =
        Address.modifyPostcode
        >> CreditCard.modifyAddress
        >> modifyCreditCard
```

然后，只要我们想设置新值，就可以使用它，比如在 `user |> set User.modifyCreditCard "A POSTCODE"` 中，我们也可以使用它来转换 `Postcode`，比如在 `user |> User.modifyCreditCardPostcode (fun (Postcode postcode) -> postcode |> String.toUpper |> PostCode)` 中。这是一个很好的关注点分离。

## 结合 getter 和 setter

我们可能会忍不住停下来，为了解决我们最初关于尴尬数据更新的问题，我们已经实现了目标，但如果我们能让属性修饰符的概念更加普遍，那就太好了。特别是如果我们能将获取和设置属性的密切相关的行为组合在一个函数中。

如果我们再次查看 `Address.modifyPostcode`，我们会看到它包含对 `Postcode` 属性的“get”操作。

```F#
module Address =
    let modifyPostcode (transformer: Postcode -> Postcode) address =
        { address with
              Postcode = address.Postcode |> transformer }
                       // ^ getting here ^
```

可以稍微重新排列一下，把“get”放在第一位，然后把它输送到一个执行“设置”的函数中。

```F#
let modifyPostcode transformer address =
    address.Postcode
    |> transformer
    |> (fun postcode -> { address with Postcode = postcode })
```

现在可以清楚地看到，我们的修饰符执行以下操作。

1. 获取孩子的数据。
2. 转换子数据。
3. 用转换后的子值更新父级。

所以，如果我们能找到一种方法跳过最后一步，那么我们就会有一个吸气剂。不过，我们唯一能做的影响 `modifyPostcode` 行为的事情就是提供一个不同的 `transformer`。不幸的是，尽管我们可能会尝试，但我们在这里无法提供任何函数来阻止最后的“设置器”步骤也运行。

不过，我们可以做的一个技巧是让 `transformer` 返回一个函数子，如果你需要回顾，请参阅 [Grokking Functors](https://dev.to/choc13/grokking-functors-bla)。如果我们这样做，那么为了调用最后的“setter”步骤，我们需要对其进行 `map`，以便我们可以将这个“setter”应用于从 `transformer` 返回的 functor 的内容。例如，`modifyCodeProperty` 看起来像这样。

```F#
let modifyPostcode (transformer: Postcode -> '``Functor<Postcode>``) address =
    address.Postcode
    |> transformer
    // Everything's the same until the final line where we call map
    |> map (fun postcode -> { address with Postcode = postcode })
```

你可能仍然想知道，这如何让我们避免称之为最后的“设定者”步骤？现在，我们可以利用 `map` 函数来改变 `modifyPostcode` 的行为。如果我们记得 functor 是如何工作的，那么 `map` 是在每个 functor 的基础上定义的，因此通过从 `transformer` 返回不同的 functor，我们可以在最后得到不同的映射行为。

我们需要的是一个functor，它的 `map` 实例只是忽略了应用于它的函数。一个只返回输入而不进行转换的functor。幸运的是，对我们来说，这样一个名为 `Const` 的 functor 已经存在，它的定义如下。

```F#
type Const<'Value, 'Ignored> =
    | Const of 'Value
    static member inline Map(Const x, _) = Const x
```

`Const` 的 `Map` 只返回输入 `x`。这样我们就可以编写一个通用的 `get` 函数，从我们的任何修饰符中提取子值。

```F#
let inline get modifier parent =
    let (Const value) = modifier Const parent
    value
```

我们的 `set` 函数呢？我们应该从那里的 `transformer` 返回哪个函数？我们需要一个只运行函数而不进行修改的函数，它恰好也是一个众所周知的函数，名为 `Identity`。`Identity` 是这样定义的。

```F#
type Identity<'t> =
    | Identity of 't
    static member inline Map(Identity x, f) = Identity(f x)
```

它的 `map` 实例只是调用输入 `x` 上的函数 `f`，并将结果包装回另一个 `Identity` 构造函数中。人们经常想知道为什么我们需要这样一个无聊的函子，但在这种情况下它会派上用场。使用该 `set` 只需要比以前稍作修改。

```F#
let inline set modifier value parent =
    let (Identity modifiedParent) =
        modifier (fun _ -> Identity value) parent

    modifiedParent
```

## 把它们放在一起🧩

我们现在对事情做了很多改变，所以让我们一起看看。我们将从修饰符必须具有的签名开始，然后展示适用于任何此类修饰符的 `get` 和 `set` 函数，最后展示如何使用它们来解决我们最初的问题。

```F#
// Modifier signature - notice how the output is completely generic now which supports both our get and set use cases
('child -> '``Functor<child>``) -> ('parent -> 'a)

let inline get modifier parent =
    let (Const child) = modifier Const parent
    child

let inline set modifier value parent =
    let (Identity modifiedParent) =
        modifier (fun _ -> Identity value) parent

    modifiedParent

module Address =
    let modifyPostcode (transformer: Postcode -> '``Functor<Postcode>``) address =
        address.Postcode
        |> transformer
        |> map (fun postcode -> { address with Postcode = 
    postcode })

module CreditCard =
    let modifyAddress (transformer: Address -> '``Functor<Address>``) card =
        card.Address
        |> transformer
        |> map (fun address -> { card with Address = 
    address })

module User =
    let modifyCreditCard (transformer: CreditCard -> '``Functor<CreditCard>``) card =
        user.CreditCard
        |> transformer
        |> map (fun card -> { user with CreditCard = 
    card })

let setCreditCardPostcode postcode user =
    user
    |> set
        (User.modifyCreditCard
         << CreditCard.modifyAddress
         << Address.modifyPostcode)
        postcode

let getCreditCardPostcode user =
    user
    |> get (
        User.modifyCreditCard
        << CreditCard.modifyAddress
        << Address.modifyPostcode
    )
```

我们的代码现在非常接近命令式样式设置器。事实上，通过将组合运算符从 `>>` 反转到 `<<` 并切换修饰符的顺序，我们甚至能够以命令式程序员熟悉的方式对属性访问进行排序，从最外层到最内层。*使用 `<<` 通常是不受欢迎的，因为它可能会令人困惑，所以请根据自己的判断使用它。*

## 你刚刚发现了Lenses🔍

这些东西我们一直称之为“修饰符（modifiers）”，它们更为人所知的是镜片（lenses）。Lenses 是一个更好的名字，因为它们实际上并没有做任何修改，它们只是专注于数据结构特定部分的可组合函数。我们可以定义像 `get`（通常称为 `view`）和 `set`（通常称之为 `setl`（用于set lens））这样的函数，让我们读取或写入任何透镜指向的值，因为透镜的结构是完全通用的。

我们还可以用镜头做更多的事情，这是光学这一更广泛主题的一部分，我们在这里没有涉及。例如，我们可以轻松地处理可能缺失的数据，或者将镜头聚焦在列表中每个元素的特定部分。

镜片也不仅仅是可组合的吸气剂和定型剂。它们还为我们的代码提供了一个抽象障碍。如果我们通过镜头而不是直接访问数据，这意味着如果我们以后重构数据结构，我们只需要修改镜头，其余代码将不受影响。

## 野外镜头🐗

在这个阶段，有一些镜头“惯例”可能值得指出，因为这就是你在野外看到它们的方式。这只是我们已经发现的语法糖，比如特殊运算符，它只是让它们写起来更愉快。下面是上面的相同示例，但使用 FSharpPlus 镜头库编写。

```F#
#r "nuget: FSharpPlus"

open FSharpPlus.Lens // <- bring the lens operators in to scope

module Address =
    // Lenses are usually named with a leading underscore
    let inline _postcode f address =
        f address.Postcode <&> fun postcode -> { address with Postcode = postcode }

module CreditCard =
    // We also usually just name after the property they point to
    let inline _address f card =
        f card.Address
        <&> fun address -> { card with Address = address }

module User =
    // The <&> is just an infix version of map
    let inline _creditCard f user =
        f user.CreditCard
        <&> fun card -> { user with CreditCard = card }

let setCreditCardPostcode postcode user =
    // We can use the .-> as an infix version of setl
    user
    |> (User._creditCard
        << CreditCard._address
        << Address._postcode)
       .-> postcode

let getCreditCardPostcode user =
    // We can use the ^. operator as an infix version of view
    user
    ^. (User._creditCard
        << CreditCard._address
        << Address._postcode)
```

这里要指出几点：

1. 通常，镜头的名称类似于 `_propertyName`。
2. 我们过去称之为 `transformer` 的东西，我们通常只表示为 `f`。
3. 通常使用 `<&>` 运算符来写镜头，而不是写 `map`。这只是映射的翻转中缀版本，它允许我们从 getter（位于运算符左侧）和 setter（位于运算符右侧）创建镜头。
4. 我们可以使用 `.->` 操作符作为 `setl` 的中缀版本，它为我们提供了一个更具命令风格的 setter。
5. 我们也可以使用 `.^` 而不是 `view` 来获取值，这有点类似于 `.` OOP中的操作符。

## 我们学到了什么？🧑‍🎓

Lenses 允许我们编写属性访问器，我们可以组合这些属性访问器来专注于大数据模型的不同部分。然后，我们可以将它们传递给 `view` 或 `setl` 等函数，以实际查看或设置数据。

透镜也是一个很大的抽象障碍，我们可以用它来将代码与数据模型当前结构的细节解耦。它们还允许我们进行其他有用的转换，这些转换我们在这里没有讨论过。镜头，以及更广泛的光学话题，是一个很大的话题，但通过这篇介绍，你会发现探索它们还能提供什么要容易得多。