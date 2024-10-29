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

因此，让我们发明一个名为 `apply` 的函数，该函数使用部分应用程序，但用于包装在其他结构（如 `Result`）中的值，并将其放在每个参数之前，如下所示。

```F#
let validateCreditCard card: Result<CreditCard, string> =
    Ok (createCreditCard)
    |> apply (validateNumber card.Number)
    |> apply (validateExpiry card.Expiry)
    |> apply (validateCvv card.Cvv)
```

您可能想知道为什么我们需要将 `createCreditCard` 包装在 `Ok` 中。这是因为此函数将返回 `Result<CreditCard, string>`，因此 `apply` 必须返回 `Result`。这意味着，为了将它们链接在一起，它还必须接受一个结果作为输入。因此，我们需要首先将 `createCardFunction` “提升”到 `Result` 中，以正确的类型启动链。

函数的 `Result` 可能看起来很奇怪，但请记住，我们将使用部分应用程序在每次调用 `apply` 后逐渐累积状态。所以我们在这里做的是从一个 `Ok` 的空容器开始，逐步用数据填充它，在每一步检查新数据是否 `Ok`。

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

每当你发现自己需要调用一个有多个参数的函数，但你必须提供的值被包裹在一个类似 `Result` 的东西中时，应用程序可能会帮助你解决问题。

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