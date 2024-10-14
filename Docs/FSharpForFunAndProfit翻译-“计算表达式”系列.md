# [返回主 Markdown](./FSharpForFunAndProfit翻译.md)



# 1 计算表达式：简介

*Part of the "Computation Expressions" series (*[link](https://fsharpforfunandprofit.com/posts/computation-expressions-intro/#series-toc)*)*

解开谜团。。。
2013年1月20日 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/computation-expressions-intro/

根据大众的要求，是时候谈谈计算表达式的奥秘了，它们是什么，以及它们在实践中如何有用（我会尽量避免使用被禁止的m字）。

在本系列中，您将学习什么是计算表达式，如何创建自己的表达式，以及涉及它们的一些常见模式。在此过程中，我们还将研究continuation、bind函数、包装器类型等。

## 背景

计算表达式似乎以深奥和难以理解而闻名。

一方面，它们很容易使用。任何写过很多F#代码的人肯定都使用过 `seq{...}` 或 `async{...}` 等标准代码。

但你如何制作一个新的这样的东西呢？他们是如何在幕后工作的？

不幸的是，许多解释似乎让事情变得更加混乱。似乎有某种心理桥梁你必须跨越。一旦你站在另一边，这一切都是显而易见的，但对于站在这一边的人来说，这是令人困惑的。

如果我们转向 MSDN 官方文档寻求指导，它很明确，但对初学者来说毫无帮助。

例如，当你在计算表达式中看到以下代码时：

```F#
{| let! pattern = expr in cexpr |}
```

它只是这个方法调用的语法糖：

```F#
builder.Bind(expr, (fun pattern -> {| cexpr |}))
```

但是…这到底意味着什么？

我希望到本系列结束时，上述文档将变得显而易见。不相信我？继续读！

## 实践中的计算表达式

在深入了解计算表达式的机制之前，让我们来看几个简单的例子，这些例子显示了使用计算表达式前后相同的代码。

让我们从一个简单的开始。假设我们有一些代码，我们想记录每一步。因此，我们定义了一个小日志函数，并在每个值创建后调用它，如下所示：

```F#
let log p = printfn "expression is %A" p

let loggedWorkflow =
    let x = 42
    log x
    let y = 43
    log y
    let z = x + y
    log z
    //return
    z
```

如果你运行这个，你会看到输出：

```
expression is 42
expression is 43
expression is 85
```

很简单。

但是每次都必须显式地写入所有日志语句，这很烦人。有办法把它们藏起来吗？

有趣的是，你应该问……计算表达式可以做到这一点。这里有一个做完全相同的事情。

首先，我们定义一个名为  `LoggingBuilder` 的新类型：

```F#
type LoggingBuilder() =
    let log p = printfn "expression is %A" p

    member this.Bind(x, f) =
        log x
        f x

    member this.Return(x) =
        x
```

*不要担心神秘的 `Bind` 和 `Return` 是为了什么——它们很快就会得到解释。*

> 请注意，计算表达式上下文中的“构建器”与用于构造和验证对象的 OO“构建器模式”不同。这里有一篇关于“构建器模式”的帖子。

接下来，我们创建一个 `logger` 类型的实例。

```F#
let logger = new LoggingBuilder()
```

因此，有了这个 `logger` 值，我们可以像这样重写原始的日志示例：

```F#
let loggedWorkflow =
    logger
        {
        let! x = 42
        let! y = 43
        let! z = x + y
        return z
        }
```

如果你运行这个，你会得到完全相同的输出，但你可以看到，使用 `logger{...}` 工作流使我们能够隐藏重复的代码。

### 安全除法

现在让我们看一个老栗子。

假设我们想一个接一个地除一系列数字，但其中一个可能是零。我们该如何应对？抛出异常是丑陋的。不过，听起来与 `option` 类型很匹配。

首先，我们需要创建一个辅助函数来进行除法运算，并返回一个 `int option`。如果一切正常，我们得到 `Some`，如果除法失败，我们得到一个 `None`。

然后，我们可以将各个部门联系在一起，在每个部门之后，我们需要测试它是否失败，只有在成功的情况下才能继续进行。

首先是辅助函数，然后是主工作流：

```F#
let divideBy bottom top =
    if bottom = 0
    then None
    else Some(top/bottom)
```

请注意，我已将除数放在参数列表的第一位。这样我们就可以编写一个类似 `12 |> divideBy 3`的表达式，这使得链接更容易。

让我们使用它。以下是一个试图将起始数字分割三次的工作流：

```F#
let divideByWorkflow init x y z =
    let a = init |> divideBy x
    match a with
    | None -> None  // give up
    | Some a' ->    // keep going
        let b = a' |> divideBy y
        match b with
        | None -> None  // give up
        | Some b' ->    // keep going
            let c = b' |> divideBy z
            match c with
            | None -> None  // give up
            | Some c' ->    // keep going
                //return
                Some c'
```

它在这里使用：

```F#
let good = divideByWorkflow 12 3 2 1
let bad = divideByWorkflow 12 3 0 1
```

`bad` 的工作流程在第三步失败，并在整个过程中返回 `None`。

值得注意的是，整个工作流程也必须返回一个 `int option`。它不能只返回一个 `int`，因为在坏的情况下它会得到什么？你能看到我们在工作流“内部”使用的类型，即选项类型，必须与最后出现的类型相同吗。记住这一点——它稍后会再次出现。

不管怎样，这种持续的测试和分支真的很丑陋！将其转化为计算表达式有帮助吗？

我们再次定义一个新类型（`MaybeBuilder`）并创建该类型的实例（`maybe`）。

```F#
type MaybeBuilder() =

    member this.Bind(x, f) =
        match x with
        | None -> None
        | Some a -> f a

    member this.Return(x) =
        Some x

let maybe = new MaybeBuilder()
```

我称这个为 `MaybeBuilder` 而不是 `divideByBuilder`，因为使用计算表达式以这种方式处理选项类型的问题很常见，`maybe` 这是它的标准名称。

现在我们已经定义了 `maybe` 工作流，让我们重写原始代码来使用它。

```F#
let divideByWorkflow init x y z =
    maybe
        {
        let! a = init |> divideBy x
        let! b = a |> divideBy y
        let! c = b |> divideBy z
        return c
        }
```

好多了。`maybe` 表达式完全隐藏了分支逻辑！

如果我们测试它，我们会得到与之前相同的结果：

```F#
let good = divideByWorkflow 12 3 2 1
let bad = divideByWorkflow 12 3 0 1
```

### 一连串的“否则”测试

在前面的“除以”示例中，我们只想在每一步都成功的情况下继续。

但有时情况正好相反。有时，控制流取决于一系列“否则”的测试。尝试一件事，如果成功了，你就完成了。否则，尝试另一件事，如果失败了，尝试第三件事，依此类推。

让我们来看一个简单的例子。假设我们有三个字典，我们想找到与一个键对应的值。每次查找都可能成功或失败，因此我们需要将查找链接成一系列。

```F#
let map1 = [ ("1","One"); ("2","Two") ] |> Map.ofList
let map2 = [ ("A","Alice"); ("B","Bob") ] |> Map.ofList
let map3 = [ ("CA","California"); ("NY","New York") ] |> Map.ofList

let multiLookup key =
    match map1.TryFind key with
    | Some result1 -> Some result1   // success
    | None ->   // failure
        match map2.TryFind key with
        | Some result2 -> Some result2 // success
        | None ->   // failure
            match map3.TryFind key with
            | Some result3 -> Some result3  // success
            | None -> None // failure
```

因为所有东西都是 F# 中的表达式，所以我们不能提前返回，我们必须在一个表达式中级联所有测试。

以下是使用方法：

```F#
multiLookup "A" |> printfn "Result for A is %A"
multiLookup "CA" |> printfn "Result for CA is %A"
multiLookup "X" |> printfn "Result for X is %A"
```

它工作得很好，但可以简化吗？

确实如此。下面是一个“否则”构建器，它允许我们简化这些类型的查找：

```F#
type OrElseBuilder() =
    member this.ReturnFrom(x) = x
    member this.Combine (a,b) =
        match a with
        | Some _ -> a  // a succeeds -- use it
        | None -> b    // a fails -- use b instead
    member this.Delay(f) = f()

let orElse = new OrElseBuilder()
```

以下是如何更改查找代码以使用它：

```F#
let map1 = [ ("1","One"); ("2","Two") ] |> Map.ofList
let map2 = [ ("A","Alice"); ("B","Bob") ] |> Map.ofList
let map3 = [ ("CA","California"); ("NY","New York") ] |> Map.ofList

let multiLookup key = orElse {
    return! map1.TryFind key
    return! map2.TryFind key
    return! map3.TryFind key
    }
```

我们可以再次确认代码按预期工作。

```F#
multiLookup "A" |> printfn "Result for A is %A"
multiLookup "CA" |> printfn "Result for CA is %A"
multiLookup "X" |> printfn "Result for X is %A"
```

### 带有回调的异步调用

最后，让我们看看回调。在 .NET 中执行异步操作的标准方法将使用 AsyncCallback 委托，该委托在异步操作完成时被调用。

以下是一个使用此技术下载网页的示例：

```F#
open System.Net
let req1 = HttpWebRequest.Create("http://fsharp.org")
let req2 = HttpWebRequest.Create("http://google.com")
let req3 = HttpWebRequest.Create("http://bing.com")

req1.BeginGetResponse((fun r1 ->
    use resp1 = req1.EndGetResponse(r1)
    printfn "Downloaded %O" resp1.ResponseUri

    req2.BeginGetResponse((fun r2 ->
        use resp2 = req2.EndGetResponse(r2)
        printfn "Downloaded %O" resp2.ResponseUri

        req3.BeginGetResponse((fun r3 ->
            use resp3 = req3.EndGetResponse(r3)
            printfn "Downloaded %O" resp3.ResponseUri

            ),null) |> ignore
        ),null) |> ignore
    ),null) |> ignore
```

对 `BeginGetResponse` 和 `EndGetResponse` 的大量调用，以及嵌套 lambdas 的使用，使得理解这一点变得相当复杂。重要的代码（在这种情况下，只是打印语句）被回调逻辑所掩盖。

事实上，管理这种级联方法在需要回调链的代码中总是一个问题；它甚至被称为“末日金字塔”（尽管解决方案都不太优雅，IMO）。

当然，我们永远不会用 F# 编写这种代码，因为 F# 内置了异步计算表达式，这既简化了逻辑，又使代码扁平化。

```F#
open System.Net
let req1 = HttpWebRequest.Create("http://fsharp.org")
let req2 = HttpWebRequest.Create("http://google.com")
let req3 = HttpWebRequest.Create("http://bing.com")

async {
    use! resp1 = req1.AsyncGetResponse()
    printfn "Downloaded %O" resp1.ResponseUri

    use! resp2 = req2.AsyncGetResponse()
    printfn "Downloaded %O" resp2.ResponseUri

    use! resp3 = req3.AsyncGetResponse()
    printfn "Downloaded %O" resp3.ResponseUri

    } |> Async.RunSynchronously
```

我们将在本系列后面详细了解 `async` 工作流是如何实现的。

## 摘要

因此，我们看到了一些非常简单的计算表达式示例，包括“之前”和“之后”，它们很好地代表了计算表达式所适用的各种问题。

- 在日志示例中，我们希望在每个步骤之间执行一些副作用。
- 在安全除法示例中，我们希望优雅地处理错误，以便我们可以专注于快乐的道路。
- 在多字典查找示例中，我们希望在第一次成功时尽早返回。
- 最后，在异步示例中，我们想隐藏回调的使用，避免“厄运金字塔”。

所有情况的共同点是，计算表达式在每个表达式之间都是“在幕后做一些事情”。

如果你想做一个糟糕的类比，你可以把计算表达式想象成 SVN 或 git 的提交后钩子，或者每次更新时都会调用的数据库触发器。实际上，这就是计算表达式的全部内容：它允许你偷偷地把自己的代码放在后台调用，这反过来又允许你专注于前台的重要代码。

为什么它们被称为“计算表达式”？好吧，这显然是某种表达，所以这一点是显而易见的。我相信 F# 团队最初确实想称之为“在每个 let 之间在后台做一些事情的表达式”，但出于某种原因，人们认为这有点笨拙，所以他们决定使用较短的名称“计算表达式”。

至于“计算表达式”和“工作流”之间的区别，我用“计算表达式“来表示 `{…}` 和 `let!` 语法，并在适当的情况下为特定的实现保留“工作流”。并非所有计算表达式实现都是工作流。例如，谈论“异步工作流”或“maybe 工作流”是合适的，但“seq 工作流”听起来不太对。

换言之，在下面的代码中，我会说 `maybe` 是我们正在使用的工作流，而特定的代码块 `{let! a = .... return c}` 是计算表达式。

```F#
maybe
    {
    let! a = x |> divideBy y
    let! b = a |> divideBy w
    let! c = b |> divideBy z
    return c
    }
```

您现在可能想开始创建自己的计算表达式，但首先我们需要绕一小段路进入延续。这是下一个。

2015年1月11日更新：我删除了使用“状态”计算表达式的计数示例。它太令人困惑，分散了人们对主要概念的注意力。

# 2 理解延续

*Part of the "Computation Expressions" series (*[link](https://fsharpforfunandprofit.com/posts/computation-expressions-continuations/#series-toc)*)*

“let”如何在幕后运作
2013年1月21日 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/computation-expressions-continuations/

在上一篇文章中，我们看到了如何使用计算表达式压缩一些复杂的代码。

以下是使用计算表达式之前的代码：

```F#
let log p = printfn "expression is %A" p

let loggedWorkflow =
    let x = 42
    log x
    let y = 43
    log y
    let z = x + y
    log z
    //return
    z
```

这是使用计算表达式后的相同代码：

```F#
let loggedWorkflow =
    logger
        {
        let! x = 42
        let! y = 43
        let! z = x + y
        return z
        }
```

使用 `let!` 而不是一个正常的 `let` 很重要。我们能自己模仿一下吗，这样我们才能理解发生了什么？是的，但我们需要先了解延续。

## 延续（Continuations）

在命令式编程中，我们有从函数“返回”的概念。当你调用一个函数时，你“进去”，然后“出来”，就像压入和弹出一个堆栈一样。

以下是一些典型的 C# 代码，其工作原理如下。请注意 `return` 关键字的使用。

```c#
public int Divide(int top, int bottom)
{
    if (bottom==0)
    {
        throw new InvalidOperationException("div by 0");
    }
    else
    {
        return top/bottom;
    }
}

public bool IsEven(int aNumber)
{
    var isEven = (aNumber % 2 == 0);
    return isEven;
}
```

你已经看过无数次了，但这种方法有一个你可能没有考虑过的微妙之处：被调用的函数总是决定要做什么。

例如，`Divide` 的实现决定要抛出一个异常。但如果我不想有异常呢？也许我想要一个可以为 `nullable<int>`，或者也许我要在屏幕上显示为“#DIV/0”。为什么要抛出一个我必须立即捕捉的异常？换句话说，为什么不让呼叫者而不是被呼叫者决定应该发生什么。

同样，在 `IsEven` 示例中，我该如何处理布尔返回值？分支在上面？或者在报告中打印出来？我不知道，但同样，与其返回调用者必须处理的布尔值，为什么不让调用者告诉被调用者下一步该做什么呢？

这就是延续。**continuation** 只是一个函数，您可以将其传递给另一个函数来告诉它下一步要做什么。

这是重写的相同 C# 代码，允许调用者传入被调用者用于处理每种情况的函数。如果这有帮助，您可以将其视为某种类似于访问者模式。或者也许不是。

```c#
public T Divide<T>(int top, int bottom, Func<T> ifZero, Func<int,T> ifSuccess)
{
    if (bottom==0)
    {
        return ifZero();
    }
    else
    {
        return ifSuccess( top/bottom );
    }
}

public T IsEven<T>(int aNumber, Func<int,T> ifOdd, Func<int,T> ifEven)
{
    if (aNumber % 2 == 0)
    {
        return ifEven(aNumber);
    }
    else
    {   return ifOdd(aNumber);
    }
}
```

请注意，C# 函数现在已更改为返回泛型 `T`，并且两个 continuation 都是返回 `T` 的 `Func`。

好吧，在 C# 中传递大量的 `Func` 参数总是看起来很难看，所以不经常这样做。但是在 F# 中传递函数很容易，所以让我们看看这段代码是如何移植的。

以下是“之前”的代码：

```F#
let divide top bottom =
    if (bottom=0)
    then invalidOp "div by 0"
    else (top/bottom)

let isEven aNumber =
    aNumber % 2 = 0
```

以下是“之后”的代码：

```F#
let divide ifZero ifSuccess top bottom =
    if (bottom=0)
    then ifZero()
    else ifSuccess (top/bottom)

let isEven ifOdd ifEven aNumber =
    if (aNumber % 2 = 0)
    then aNumber |> ifEven
    else aNumber |> ifOdd
```

有几件事需要注意。首先，您可以看到，我将额外的函数（`ifZero` 等）放在参数列表的第一位，而不是像 C# 示例中那样放在最后。为什么？因为我可能想使用部分应用程序。

此外，在 `isEven` 示例中，我编写了 `aNumber |> ifEven` 和 `aNumber |> ifOdd`。这清楚地表明，我们正在将当前值管道化到延续中，延续始终是评估的最后一步。我们将在本文后面使用完全相同的模式，所以请确保您了解这里发生了什么。

## 延续例子

有了延续的力量，我们可以根据调用者的需求，以三种完全不同的方式使用相同的除法函数。

以下是我们可以快速创建的三个场景：

- 将结果管道化为消息并打印出来，
- 将结果转换为一个选项，对于坏情况使用 `None`，对于好情况使用 `Some`，
- 或者在坏的情况下抛出异常，在好的情况下返回结果。

```F#
// Scenario 1: pipe the result into a message
// ----------------------------------------
// setup the functions to print a message
let ifZero1 () = printfn "bad"
let ifSuccess1 x = printfn "good %i" x

// use partial application
let divide1  = divide ifZero1 ifSuccess1

//test
let good1 = divide1 6 3
let bad1 = divide1 6 0

// Scenario 2: convert the result to an option
// ----------------------------------------
// setup the functions to return an Option
let ifZero2() = None
let ifSuccess2 x = Some x
let divide2  = divide ifZero2 ifSuccess2

//test
let good2 = divide2 6 3
let bad2 = divide2 6 0

// Scenario 3: throw an exception in the bad case
// ----------------------------------------
// setup the functions to throw exception
let ifZero3() = failwith "div by 0"
let ifSuccess3 x = x
let divide3  = divide ifZero3 ifSuccess3

//test
let good3 = divide3 6 3
let bad3 = divide3 6 0
```

请注意，使用这种方法，调用者永远不必在任何地方捕获 `divide` 的异常。调用者决定是否抛出异常，而不是被调用者。因此，不仅除法函数在不同的上下文中变得更加可重用，而且圈复杂度也下降了一个级别。

同样的三种场景可以应用于 `isEven` 实现：

```F#
// Scenario 1: pipe the result into a message
// ----------------------------------------
// setup the functions to print a message
let ifOdd1 x = printfn "isOdd %i" x
let ifEven1 x = printfn "isEven %i" x

// use partial application
let isEven1  = isEven ifOdd1 ifEven1

//test
let good1 = isEven1 6
let bad1 = isEven1 5

// Scenario 2: convert the result to an option
// ----------------------------------------
// setup the functions to return an Option
let ifOdd2 _ = None
let ifEven2 x = Some x
let isEven2  = isEven ifOdd2 ifEven2

//test
let good2 = isEven2 6
let bad2 = isEven2 5

// Scenario 3: throw an exception in the bad case
// ----------------------------------------
// setup the functions to throw exception
let ifOdd3 _ = failwith "assert failed"
let ifEven3 x = x
let isEven3  = isEven ifOdd3 ifEven3

//test
let good3 = isEven3 6
let bad3 = isEven3 5
```

在这种情况下，好处更微妙，但都是一样的：调用者永远不必在任何地方用 `if/then/else` 处理布尔值。复杂性更低，出错的可能性更小。

这似乎是一个微不足道的区别，但通过像这样传递函数，我们可以使用所有我们喜欢的函数技术，如组合、部分应用等。

我们之前也遇到过延续，在关于用类型设计的系列中。我们看到，它们的使用使调用者能够决定在构造函数中可能出现验证错误的情况下会发生什么，而不仅仅是抛出异常。

```F#
type EmailAddress = EmailAddress of string

let CreateEmailAddressWithContinuations success failure (s:string) =
    if System.Text.RegularExpressions.Regex.IsMatch(s,@"^\S+@\S+\.\S+$")
        then success (EmailAddress s)
        else failure "Email address must contain an @ sign"
```

success 函数接受电子邮件作为参数，error 函数接受字符串。这两个函数必须返回相同的类型，但类型由您决定。

这是一个使用延续的简单例子。这两个函数都执行 printf，不返回任何值（即 unit）。

```F#
// setup the functions
let success (EmailAddress s) = printfn "success creating email %s" s
let failure  msg = printfn "error creating email: %s" msg
let createEmail = CreateEmailAddressWithContinuations success failure

// test
let goodEmail = createEmail "x@example.com"
let badEmail = createEmail "example.com"
```

### 连续传递风格

使用这样的 continuation 会导致一种称为“continuation passing style”（或 CPS）的编程风格，在这种风格下，每个函数都会被调用一个额外的“下一步该做什么”函数参数。

为了看出区别，让我们看看标准的、直接的编程风格。

当你使用直接风格时，你会像这样“输入”和“输出”函数

```
call a function ->
   <- return from the function
call another function ->
   <- return from the function
call yet another function ->
   <- return from the function
```

另一方面，在连续传递风格中，您最终会得到一系列函数，如下所示：

```
evaluate something and pass it into ->
   a function that evaluates something and passes it into ->
      another function that evaluates something and passes it into ->
         yet another function that evaluates something and passes it into ->
            ...etc...
```

这两种风格之间显然有很大的区别。

在直接风格中，有一个函数层次结构。顶层函数是一种“主控制器”，它调用一个子程序，然后调用另一个子例程，决定何时分支，何时循环，并明确地协调控制流。

然而，在连续传递风格中，没有“主控制器”。相反，有一种“管道”，不是数据管道，而是控制流管道，其中“负责的函数”随着执行逻辑在管道中的流动而变化。

如果您曾经将事件处理程序附加到 GUI 中的按钮单击，或者使用 BeginInvoke 的回调，那么您在不知不觉中使用了这种风格。事实上，这种风格将是理解异步工作流的关键，我将在本系列后面讨论。

## 继续和“let”

那么，这一切与 `let` 有什么关系呢？

让我们回过头来重新审视“`let`”实际上做了什么。

请记住，（非顶级）“let”永远不能单独使用——它必须始终是更大代码块的一部分。

即：

```F#
let x = someExpression
```

真正意味着：

```F#
let x = someExpression in [an expression involving x]
```

然后，每次在第二个表达式（body 表达式）中看到 `x` 时，用第一个表达式（`someExpression`）替换它。

例如，表达式：

```F#
let x = 42
let y = 43
let z = x + y
```

真的意味着（使用 verbose `in` 关键字）：

```F#
let x = 42 in
  let y = 43 in
    let z = x + y in
       z    // the result
```

有趣的是，lambda 看起来与 `let` 非常相似：

```F#
fun x -> [an expression involving x]
```

如果我们也输入 `x` 的值，我们得到以下结果：

```F#
someExpression |> (fun x -> [an expression involving x] )
```

在你看来，这不是很像 `let` 吗？下面是一个 let 和一个 lambda 并排：

```F#
// let
let x = someExpression in [an expression involving x]

// pipe a value into a lambda
someExpression |> (fun x -> [an expression involving x] )
```

它们都有一个 `x` 和一个 `someExpression`，在 lambda 的主体中看到 `x` 的任何地方，都可以用 `someExpression` 替换它。是的，在 lambda 的情况下，`x` 和 `someExpression` 是相反的，但除此之外，它基本上与 `let` 是一样的。

因此，使用这种技术，我们可以用这种风格重写原始示例：

```F#
42 |> (fun x ->
  43 |> (fun y ->
     x + y |> (fun z ->
       z)))
```

当它以这种方式编写时，你可以看到我们已经将 `let` 风格转变为 continuating passing 风格！

- 在第一行中，我们有一个值 `42` ——我们想用它做什么？让我们把它传递到一个延续中，就像我们之前对 `isEven` 函数所做的那样。在继续的上下文中，我们将把 `42` 重新标记为 `x`。
- 在第二行中，我们有一个值 `43`——我们想用它做什么？让我们把它也变成一个延续，在这种情况下称之为 `y`。
- 在第三行中，我们将 x 和 y 相加以创建一个新值。我们想用它做什么？另一个延续，另一个标签（`z`）。
  最后，在最后一行，我们完成了，整个表达式的计算结果为 `z`。

### 将延续包裹在函数中

让我们去掉显式管道，编写一个小函数来包装这个逻辑。我们不能称之为“let”，因为这是一个保留字，更重要的是，参数是从“let”向后的。“x”在右手边，“someExpression”在左手边。所以我们暂时称之为 `pipeInto`。

`pipeInto` 的定义非常明显：

```F#
let pipeInto (someExpression,lambda) =
    someExpression |> lambda
```

*请注意，我们使用元组同时传递这两个参数，而不是用空格分隔的两个不同参数。他们将永远是一对。*

因此，使用这个 `pipeTo` 函数，我们可以再次将示例重写为：

```F#
pipeInto (42, fun x ->
  pipeInto (43, fun y ->
    pipeInto (x + y, fun z ->
       z)))
```

或者我们可以去掉缩进，这样写：

```F#
pipeInto (42, fun x ->
pipeInto (43, fun y ->
pipeInto (x + y, fun z ->
z)))
```

你可能会想：那又怎样？为什么要费心把管道包装成函数？

答案是，我们可以在 `pipeTo` 函数中添加额外的代码来“幕后”做一些事情，就像在计算表达式中一样。

### 重新审视“日志记录”示例

让我们重新定义 `pipeInto` 以添加一点日志记录，如下所示：

```F#
let pipeInto (someExpression,lambda) =
   printfn "expression is %A" someExpression
   someExpression |> lambda
```

现在…再次运行该代码。

```F#
pipeInto (42, fun x ->
pipeInto (43, fun y ->
pipeInto (x + y, fun z ->
z
)))
```

输出是什么？

```
expression is 42
expression is 43
expression is 85
```

这与我们在早期实现中的输出完全相同。我们创建了自己的小计算表达式工作流！

如果我们将其与计算表达式版本进行并排比较，我们可以看到我们的自制程序版本与 `let!` 非常相似，除了我们颠倒了参数，并且我们有用于延续的显式箭头。



### 重新审视“安全除法”的例子

让我们对“安全除法”示例做同样的事情。以下是原始代码：

```F#
let divideBy bottom top =
    if bottom = 0
    then None
    else Some(top/bottom)

let divideByWorkflow x y w z =
    let a = x |> divideBy y
    match a with
    | None -> None  // give up
    | Some a' ->    // keep going
        let b = a' |> divideBy w
        match b with
        | None -> None  // give up
        | Some b' ->    // keep going
            let c = b' |> divideBy z
            match c with
            | None -> None  // give up
            | Some c' ->    // keep going
                //return
                Some c'
```

现在你应该看到，这种“阶梯式”风格是一个明显的线索，表明我们真的应该使用延续。

让我们看看是否可以向 `pipeInto` 添加额外的代码来为我们进行匹配。我们想要的逻辑是：

- 如果 `someExpression` 参数为 `None`，则不要调用延续 lambda。
- 如果 `someExpression` 参数为 `Some`，则调用延续 lambda，传递 `Some` 的内容。

这是：

```F#
let pipeInto (someExpression,lambda) =
   match someExpression with
   | None ->
       None
   | Some x ->
       x |> lambda
```

有了这个新版本的 `pipeInto`，我们可以像这样重写原始代码：

```F#
let divideByWorkflow x y w z =
    let a = x |> divideBy y
    pipeInto (a, fun a' ->
        let b = a' |> divideBy w
        pipeInto (b, fun b' ->
            let c = b' |> divideBy z
            pipeInto (c, fun c' ->
                Some c' //return
                )))
```

我们可以稍微清理一下。

首先，我们可以删除 `a`、`b` 和 `c`，并直接用 `divideBy` 表达式替换它们。因此：

```F#
let a = x |> divideBy y
pipeInto (a, fun a' ->
```

变成了这样：

```F#
pipeInto (x |> divideBy y, fun a' ->
```

现在我们可以将 `a'` 重新标记为 `a`，依此类推，我们还可以删除阶梯缩进，这样我们就得到了：

```F#
let divideByResult x y w z =
    pipeInto (x |> divideBy y, fun a ->
    pipeInto (a |> divideBy w, fun b ->
    pipeInto (b |> divideBy z, fun c ->
    Some c //return
    )))
```

最后，我们将创建一个名为 `return'` 的小助手函数，将结果包装在一个选项中。把它们放在一起，代码看起来像这样：

```F#
let divideBy bottom top =
    if bottom = 0
    then None
    else Some(top/bottom)

let pipeInto (someExpression,lambda) =
   match someExpression with
   | None ->
       None
   | Some x ->
       x |> lambda

let return' c = Some c

let divideByWorkflow x y w z =
    pipeInto (x |> divideBy y, fun a ->
    pipeInto (a |> divideBy w, fun b ->
    pipeInto (b |> divideBy z, fun c ->
    return' c
    )))

let good = divideByWorkflow 12 3 2 1
let bad = divideByWorkflow 12 3 0 1
```

同样，如果我们把这个版本和计算表达式版本并排比较，我们可以看到我们的自制版本在含义上是相同的。只是语法不同。

计算表达式：日志记录

## 摘要

在这篇文章中，我们讨论了 continuation 和 continuation 传递风格，以及我们如何将 `let` 视为一种在幕后进行 continuation 的好语法。

所以现在我们有了开始创建自己版本的 `let` 所需的一切。在下一篇文章中，我们将把这些知识付诸实践。

# 3 介绍“bind”

迈向创造我们自己的“let!”
2013年1月22日 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/computation-expressions-bind/

在上一篇文章中，我们讨论了如何将 `let` 视为一种在幕后进行延续的好语法。我们引入了一个 `pipeInto` 函数，允许我们在延续管道中添加钩子。

现在，我们准备看看我们的第一个构建器方法 `Bind`，它将这种方法形式化，是任何计算表达式的核心。

> 请注意，计算表达式上下文中的“构建器”与用于构造和验证对象的OO“构建器模式”不同。这里有一篇关于“构建器模式”的帖子。

## 介绍“绑定”

MSDN 计算表达式页面描述了 `let!` 表达式作为 `Bind` 方法的语法糖。让我们再看看这个：

这里是 `let!` 表达式文档，以及一个真实的示例：

```F#
// documentation
{| let! pattern = expr in cexpr |}

// real example
let! x = 43 in some expression
```

以下是 `Bind` 方法文档，以及一个真实的示例：

```F#
// documentation
builder.Bind(expr, (fun pattern -> {| cexpr |}))

// real example
builder.Bind(43, (fun x -> some expression))
```

注意一些有趣的事情：

- `Bind` 接受两个参数，一个表达式（`43`）和一个 lambda。
- lambda（`x`）的参数绑定到作为第一个参数传入的表达式。（至少在这种情况下是这样。稍后将详细介绍。）
- `Bind` 的参数顺序与 `let!` 中的顺序相反。

所以换句话说，如果我们链上有几个 `let!` 这样的表达式组合在一起：

```F#
let! x = 1
let! y = 2
let! z = x + y
```

编译器将其转换为对 `Bind` 的调用，如下所示：

```F#
Bind(1, fun x ->
Bind(2, fun y ->
Bind(x + y, fun z ->
etc
```

我想你现在可以看到我们要做什么了。

事实上，我们的 `pipeInto` 函数与 `Bind` 方法完全相同。

这是一个关键的见解：计算表达式只是为我们自己可以做的事情创建良好语法的一种方式。

### 独立绑定函数

拥有这样的“绑定”函数实际上是一种标准的函数模式，它根本不依赖于计算表达式。

首先，为什么它被称为“bind”？正如我们所看到的，“绑定”函数或方法可以被认为是向函数提供输入值。这被称为将值“绑定”到函数的参数（记住所有函数只有一个参数）。

所以当你想到这种 `bind` 方式时，你可以看到它类似于管道或组合。

事实上，你可以把它变成一个中缀操作，如下所示：

```F#
let (>>=) m f = pipeInto(m,f)
```

顺便说一句，这个符号“»=”是将 bind 写成中缀运算符的标准方式。如果你在其他 F# 代码中看到过它，那可能就是它所代表的。

回到安全除法的例子，我们现在可以在一行中编写工作流，如下所示：

```F#
let divideByWorkflow x y w z =
    x |> divideBy y >>= divideBy w >>= divideBy z
```

您可能想知道这与正常的管道或组成有何不同？这并不明显。

答案有两个：

- 首先，`bind` 函数对每种情况都有额外的定制行为。它不是一个通用的函数，就像管道或组合一样。
- 其次，值参数的输入类型（如上所述的 `m`）不一定与函数参数的输出类型（上所述的 `f`）相同，因此绑定所做的一件事就是优雅地处理这种不匹配，以便可以链接函数。

正如我们将在下一篇文章中看到的，bind 通常适用于某种“包装器”类型。值参数可能是 `WrapperType<TypeA>`，然后 `bind` 函数的函数参数的签名始终是 `TypeA->WrapperType<TypeB>`。

在安全除法 `bind` 的特定情况下，包装器类型为 `Option`。值参数的类型（上面的 `m`）是 `Option<int>`，函数参数的签名（上面的 `f`）是 `int -> Option<int>`。

要查看在不同上下文中使用的绑定，以下是使用中缀绑定函数表示的日志工作流程的示例：

```F#
let (>>=) m f =
    printfn "expression is %A" m
    f m

let loggingWorkflow =
    1 >>= (+) 2 >>= (*) 42 >>= id
```

在这种情况下，没有包装器类型。一切都是 `int`。但即便如此，`bind` 也有在幕后执行日志记录的特殊行为。

## Option.bind 和“maybe”工作流程重新审视

在 F# 库中，您将在许多地方看到 `Bind` 函数或方法。现在你知道它们是用来干什么的了！

一个特别有用的是 `Option.bind`，它完全符合我们上面手写的内容，即

- 如果输入参数为 `None`，则不要调用 continuation 函数。
- 如果输入参数为 `Some`，则调用 continuation 函数，传递 `Some` 的内容。

这是我们手工制作的功能：

```F#
let pipeInto (m,f) =
   match m with
   | None ->
       None
   | Some x ->
       x |> f
```

下面是 `Option.bind` 的实现：

```F#
module Option =
    let bind f m =
       match m with
       | None ->
           None
       | Some x ->
           x |> f
```

这其中有一个寓意——不要急于写自己的函数。很可能有一些库函数可以重用。

这是“maybe”的工作流程，重写为使用 `Option.bind`：

```F#
type MaybeBuilder() =
    member this.Bind(m, f) = Option.bind f m
    member this.Return(x) = Some x
```

## 回顾迄今为止的不同方法

到目前为止，我们已经为“安全除法”示例使用了四种不同的方法。让我们把它们并排放在一起，再比较一次。

注意：我已将原始的 `pipeInto` 函数重命名为 `bind`，并使用 `Option.bind` 代替了我们最初的自定义实现。

首先是原始版本，使用明确的工作流程：

```F#
module DivideByExplicit =

    let divideBy bottom top =
        if bottom = 0
        then None
        else Some(top/bottom)

    let divideByWorkflow x y w z =
        let a = x |> divideBy y
        match a with
        | None -> None  // give up
        | Some a' ->    // keep going
            let b = a' |> divideBy w
            match b with
            | None -> None  // give up
            | Some b' ->    // keep going
                let c = b' |> divideBy z
                match c with
                | None -> None  // give up
                | Some c' ->    // keep going
                    //return
                    Some c'
    // test
    let good = divideByWorkflow 12 3 2 1
    let bad = divideByWorkflow 12 3 0 1
```

接下来，使用我们自己的“bind”版本（又称“pipeInto”）

```F#
module DivideByWithBindFunction =

    let divideBy bottom top =
        if bottom = 0
        then None
        else Some(top/bottom)

    let bind (m,f) =
        Option.bind f m

    let return' x = Some x

    let divideByWorkflow x y w z =
        bind (x |> divideBy y, fun a ->
        bind (a |> divideBy w, fun b ->
        bind (b |> divideBy z, fun c ->
        return' c
        )))

    // test
    let good = divideByWorkflow 12 3 2 1
    let bad = divideByWorkflow 12 3 0 1
```

接下来，使用计算表达式：

```F#
module DivideByWithCompExpr =

    let divideBy bottom top =
        if bottom = 0
        then None
        else Some(top/bottom)

    type MaybeBuilder() =
        member this.Bind(m, f) = Option.bind f m
        member this.Return(x) = Some x

    let maybe = new MaybeBuilder()

    let divideByWorkflow x y w z =
        maybe
            {
            let! a = x |> divideBy y
            let! b = a |> divideBy w
            let! c = b |> divideBy z
            return c
            }

    // test
    let good = divideByWorkflow 12 3 2 1
    let bad = divideByWorkflow 12 3 0 1
```

最后，使用 bind 作为中缀操作：

```F#
module DivideByWithBindOperator =

    let divideBy bottom top =
        if bottom = 0
        then None
        else Some(top/bottom)

    let (>>=) m f = Option.bind f m

    let divideByWorkflow x y w z =
        x |> divideBy y
        >>= divideBy w
        >>= divideBy z

    // test
    let good = divideByWorkflow 12 3 2 1
    let bad = divideByWorkflow 12 3 0 1
```

绑定函数非常强大。在下一篇文章中，我们将看到结合 `bind` 和包装器类型创建了一种在后台传递额外信息的优雅方式。

## 练习：你理解得有多好？

在你继续下一篇文章之前，你为什么不测试一下自己，看看到目前为止你是否理解了所有内容？

这里有一个小练习给你。

### 第 1 部分-创建工作流

首先，创建一个将字符串解析为 int 的函数：

```F#
let strToInt str = ???
```

然后创建自己的计算表达式构建器类，以便在工作流中使用它，如下所示。

```F#
let stringAddWorkflow x y z =
    yourWorkflow
        {
        let! a = strToInt x
        let! b = strToInt y
        let! c = strToInt z
        return a + b + c
        }

// test
let good = stringAddWorkflow "12" "3" "2"
let bad = stringAddWorkflow "12" "xyz" "2"
```

### 第 2 部分-创建绑定函数

一旦你有了第一个部分，通过添加两个功能来扩展这个想法：

```F#
let strAdd str i = ???
let (>>=) m f = ???
```

然后使用这些函数，您应该能够编写这样的代码：

```F#
let good = strToInt "1" >>= strAdd "2" >>= strAdd "3"
let bad = strToInt "1" >>= strAdd "xyz" >>= strAdd "3"
```

## 摘要

以下是本文所涵盖要点的总结：

- 计算表达式为连续传递提供了良好的语法，为我们隐藏了链式逻辑。
- `bind` 是将一个步骤的输出链接到下一个步骤输入的关键函数。
- 符号 `>>=` 是将 bind 写成中缀运算符的标准方式。

# 4 计算表达式和包装器类型

*Part of the "Computation Expressions" series (*[link](https://fsharpforfunandprofit.com/posts/computation-expressions-wrapper-types/#series-toc)*)*

使用类型来辅助工作流程
2013年1月23日 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/computation-expressions-wrapper-types/

在上一篇文章中，我们介绍了“maybe”工作流程，它允许我们隐藏将选项类型链接在一起的混乱。

“maybe”工作流的典型用法如下：

```F#
let result =
    maybe
        {
        let! anInt = expression of Option<int>
        let! anInt2 = expression of Option<int>
        return anInt + anInt2
        }
```

正如我们之前看到的，这里发生了一些明显奇怪的行为：

- 在 `let!` 行，equals 右侧的表达式是一个 `int option`，但左侧的值只是一个 `int`。`let!` 在将选项绑定到值之前将其“解包”。
- 在 `return` 行中，情况正好相反。返回的表达式是 `int`，但整个工作流（`result`）的值是 `int option`。也就是说，`return` 将原始值“包装”回一个选项中。

我们将在这篇文章中跟进这些观察结果，我们将看到这导致了计算表达式的主要用途之一：即隐式解包和重写存储在某种包装器类型中的值。

## 另一个例子

让我们来看另一个例子。假设我们正在访问一个数据库，我们想在 Success/Error 联合类型中捕获结果，如下所示：

```F#
type DbResult<'a> =
    | Success of 'a
    | Error of string
```

然后，我们在数据库访问方法中使用这种类型。以下是一些非常简单的存根，让您了解如何使用 `DbResult` 类型：

```F#
let getCustomerId name =
    if (name = "")
    then Error "getCustomerId failed"
    else Success "Cust42"

let getLastOrderForCustomer custId =
    if (custId = "")
    then Error "getLastOrderForCustomer failed"
    else Success "Order123"

let getLastProductForOrder orderId =
    if (orderId  = "")
    then Error "getLastProductForOrder failed"
    else Success "Product456"
```

现在，假设我们想将这些调用链接在一起。首先从名称中获取客户 id，然后获取客户 id 的订单，然后从订单中获取产品。

这是最明确的方法。正如你所看到的，我们必须在每一步进行模式匹配。

```F#
let product =
    let r1 = getCustomerId "Alice"
    match r1 with
    | Error _ -> r1
    | Success custId ->
        let r2 = getLastOrderForCustomer custId
        match r2 with
        | Error _ -> r2
        | Success orderId ->
            let r3 = getLastProductForOrder orderId
            match r3 with
            | Error _ -> r3
            | Success productId ->
                printfn "Product is %s" productId
                r3
```

非常丑陋的代码。顶层流已经淹没在错误处理逻辑中。

计算表达式来拯救！我们可以编写一个在幕后处理成功/错误分支的程序：

```F#
type DbResultBuilder() =

    member this.Bind(m, f) =
        match m with
        | Error _ -> m
        | Success a ->
            printfn "\tSuccessful: %s" a
            f a

    member this.Return(x) =
        Success x

let dbresult = new DbResultBuilder()
```

> 请注意，计算表达式上下文中的“构建器”与用于构造和验证对象的 OO “构建器模式”不同。这里有一篇关于“构建器模式”的帖子。

通过这个工作流程，我们可以专注于大局，编写更清晰的代码：

```F#
let product' =
    dbresult {
        let! custId = getCustomerId "Alice"
        let! orderId = getLastOrderForCustomer custId
        let! productId = getLastProductForOrder orderId
        printfn "Product is %s" productId
        return productId
        }
printfn "%A" product'
```

如果有错误，工作流会很好地捕捉它们，并告诉错误在哪里，如下面的示例所示：

```F#
let product'' =
    dbresult {
        let! custId = getCustomerId "Alice"
        let! orderId = getLastOrderForCustomer "" // error!
        let! productId = getLastProductForOrder orderId
        printfn "Product is %s" productId
        return productId
        }
printfn "%A" product''
```

## 包装器类型在工作流中的作用

因此，现在我们已经看到了两个工作流（`maybe` 工作流和 `dbresult` 工作流），每个工作流都有自己相应的包装器类型（分别为 `Option<T>` 和 `DbResult<T>`）。

这些不仅仅是特例。事实上，每个计算表达式都必须有一个关联的包装器类型。包装器类型通常是专门为与我们想要管理的工作流程紧密结合而设计的。

上面的例子清楚地证明了这一点。我们创建的 `DbResult` 类型不仅仅是一个简单的返回值类型；它实际上在工作流中起着至关重要的作用，因为它“存储”了工作流的当前状态，以及它在每个步骤中是成功还是失败。通过使用类型本身的各种情况，`dbresult` 工作流可以为我们管理转换，将它们隐藏起来，使我们能够专注于大局。

我们将在本系列的后面学习如何设计一个好的包装器类型，但首先让我们看看它们是如何被操纵的。

## 绑定、返回和包装类型

让我们再次看看计算表达式的 `Bind` 和 `Return` 方法的定义。

我们将从简单的一个开始，`Return`。MSDN 上记录的 `Return` 签名如下：

```F#
member Return : 'T -> M<'T>
```

换句话说，对于某些类型 `T`，`Return` 方法只是将其包装在包装器类型中。

*注意：在签名中，包装器类型通常称为 `M`，因此 `M<int>` 是应用于 `int` 的包装器类型，`M<string>` 是应用于 `string` 的包装器型，以此类推。*

我们已经看到了两个这种用法的例子。`maybe` 工作流返回 `Some`，这是一个选项类型，`dbresult` 工作流返回 `Success`，它是 `DbResult` 类型的一部分。

```F#
// return for the maybe workflow
member this.Return(x) =
    Some x

// return for the dbresult workflow
member this.Return(x) =
    Success x
```

现在让我们来看看 `Bind`。`Bind` 的签名是这样的：

```F#
member Bind : M<'T> * ('T -> M<'U>) -> M<'U>
```

它看起来很复杂，所以让我们把它分解一下。它接受一个元组 `M<'T>*（'T->M<'U>）` 并返回一个 `M<'U>`，其中 `M<'U>` 表示应用于类型 `U` 的包装器类型。

元组又分为两部分：

- `M<'T>` 是 `T` 类型的包装器，以及
- `'T -> M<'U>` 是一个函数，它接受一个未包裹的 `T` 并创建一个包裹的 `U`。

换句话说，`Bind` 所做的是：

- 取一个包装好的值。
- 解开它，做任何特殊的“幕后”逻辑。
- 然后，可以选择将该函数应用于未包裹的值，以创建新的包裹值。
- 即使未应用该函数，`Bind` 仍必须返回一个包装好的 `U`。

有了这种理解，以下是我们已经看到的 `Bind` 方法：

```F#
// return for the maybe workflow
member this.Bind(m,f) =
   match m with
   | None -> None
   | Some x -> f x

// return for the dbresult workflow
member this.Bind(m, f) =
    match m with
    | Error _ -> m
    | Success x ->
        printfn "\tSuccessful: %s" x
        f x
```

仔细查看这段代码，确保你理解为什么这些方法确实遵循上述模式。

最后，图片总是有用的。以下是各种类型和功能的示意图：

绑定图

- 对于 `Bind`，我们从一个包装值（这里是 `m`）开始，将其解包为 `T` 类型的原始值，然后（maybe）对其应用函数 `f` 以获得 `U` 类型的包装值。
- 对于 `Return`，我们从一个值（这里是 `x`）开始，然后简单地包装它。

### 类型包装器是泛型的

请注意，所有函数都使用泛型类型（`T` 和 `U`），而不是包装器类型本身，包装器类型必须始终相同。例如，没有什么能阻止 `maybe` 绑定函数接受一个 `int` 并返回一个 `Option<string>`，或者接受一个 `string` 然后返回一个 `Options<bool>`。唯一的要求是它总是返回一个 `Option<something>`。

为了理解这一点，我们可以重新审视上面的例子，但我们不会在所有地方都使用字符串，而是为客户 id、订单 id 和产品 id 创建特殊类型。这意味着链中的每一步都将使用不同的类型。

我们将再次从类型开始，这次定义 `CustomerId` 等。

```F#
type DbResult<'a> =
    | Success of 'a
    | Error of string

type CustomerId =  CustomerId of string
type OrderId =  OrderId of int
type ProductId =  ProductId of string
```

除了在 `Success` 行中使用了新类型之外，代码几乎完全相同。

```F#
let getCustomerId name =
    if (name = "")
    then Error "getCustomerId failed"
    else Success (CustomerId "Cust42")

let getLastOrderForCustomer (CustomerId custId) =
    if (custId = "")
    then Error "getLastOrderForCustomer failed"
    else Success (OrderId 123)

let getLastProductForOrder (OrderId orderId) =
    if (orderId  = 0)
    then Error "getLastProductForOrder failed"
    else Success (ProductId "Product456")
```

这又是长篇大论的版本。

```F#
let product =
    let r1 = getCustomerId "Alice"
    match r1 with
    | Error e -> Error e
    | Success custId ->
        let r2 = getLastOrderForCustomer custId
        match r2 with
        | Error e -> Error e
        | Success orderId ->
            let r3 = getLastProductForOrder orderId
            match r3 with
            | Error e -> Error e
            | Success productId ->
                printfn "Product is %A" productId
                r3
```

有几个变化值得讨论：

- 首先，底部的 `printfn` 使用“%A”格式说明符，而不是“%s”。这是必需的，因为 `ProductId` 类型现在是联合类型。
- 更微妙的是，错误行中似乎有不必要的代码。为什么写 `| Error e -> Error e`？原因是，正在匹配的传入错误的类型为 `DbResult<CustomerId>` 或 `DbResult<OrderId>`，但返回值的类型必须为 `DbResult<ProductId>`。因此，尽管这两个错误看起来相同，但它们实际上是不同类型的。

接下来是构建器，除了 `|Error e -> Error e` 行之外，它根本没有改变。

```F#
type DbResultBuilder() =

    member this.Bind(m, f) =
        match m with
        | Error e -> Error e
        | Success a ->
            printfn "\tSuccessful: %A" a
            f a

    member this.Return(x) =
        Success x

let dbresult = new DbResultBuilder()
```

最后，我们可以像以前一样使用工作流。

```F#
let product' =
    dbresult {
        let! custId = getCustomerId "Alice"
        let! orderId = getLastOrderForCustomer custId
        let! productId = getLastProductForOrder orderId
        printfn "Product is %A" productId
        return productId
        }
printfn "%A" product'
```

在每一行，返回的值都是不同类型的（`DbResult<CustomerId>`、`DbResult<OrderId>` 等），但由于它们具有相同的包装器类型，绑定按预期工作。

最后，这是带有错误案例的工作流程。

```F#
let product'' =
    dbresult {
        let! custId = getCustomerId "Alice"
        let! orderId = getLastOrderForCustomer (CustomerId "") //error
        let! productId = getLastProductForOrder orderId
        printfn "Product is %A" productId
        return productId
        }
printfn "%A" product''
```

## 计算表达式的组成

我们已经看到，每个计算表达式都必须有一个相关的包装器类型。这种包装器类型在 `Bind` 和 `Return` 中都使用，这带来了一个关键的好处：

- `Return` 的输出可以馈送到 `Bind` 的输入

换句话说，因为工作流返回一个包装器类型，而且因为 `let!` 使用包装器类型，您可以在 `let!` 表达式的右侧放置一个“子”工作流。

例如，假设您有一个名为 `myworkflow` 的工作流。然后你可以写下以下内容：

```F#
let subworkflow1 = myworkflow { return 42 }
let subworkflow2 = myworkflow { return 43 }

let aWrappedValue =
    myworkflow {
        let! unwrappedValue1 = subworkflow1
        let! unwrappedValue2 = subworkflow2
        return unwrappedValue1 + unwrappedValue2
        }
```

或者你甚至可以像这样“内联”它们：

```F#
let aWrappedValue =
    myworkflow {
        let! unwrappedValue1 = myworkflow {
            let! x = myworkflow { return 1 }
            return x
            }
        let! unwrappedValue2 = myworkflow {
            let! y = myworkflow { return 2 }
            return y
            }
        return unwrappedValue1 + unwrappedValue2
        }
```

如果你使用过 `async` 工作流，你可能已经这样做了，因为异步工作流通常包含其他嵌入其中的异步：

```F#
let a =
    async {
        let! x = doAsyncThing  // nested workflow
        let! y = doNextAsyncThing x // nested workflow
        return x + y
    }
```

## 介绍“ReturnFrom”

我们一直使用 `return` 作为一种轻松包装未包装返回值的方法。

但有时我们有一个已经返回包装值的函数，我们想直接返回它。`return` 对此没有好处，因为它需要一个解包类型作为输入。

解决方案是一个名为 `return!` 的 `return` 变体，它将包装类型作为输入并返回。

“builder”类中的相应方法称为 `ReturnFrom`。通常，实现只会“原样”返回包装好的类型（当然，您始终可以在幕后添加额外的逻辑）。

以下是“maybe”工作流的一个变体，展示了如何使用它：

```F#
type MaybeBuilder() =
    member this.Bind(m, f) = Option.bind f m
    member this.Return(x) =
        printfn "Wrapping a raw value into an option"
        Some x
    member this.ReturnFrom(m) =
        printfn "Returning an option directly"
        m

let maybe = new MaybeBuilder()
```

与正常 `return` 相比，它在这里被使用。

```F#
// return an int
maybe { return 1  }

// return an Option
maybe { return! (Some 2)  }
```

举个更现实的例子，这里是 `return!` 与 `divideBy`:

```F#
// using return
maybe
    {
    let! x = 12 |> divideBy 3
    let! y = x |> divideBy 2
    return y  // return an int
    }

// using return!
maybe
    {
    let! x = 12 |> divideBy 3
    return! x |> divideBy 2  // return an Option
    }
```

## 摘要

这篇文章介绍了包装器类型以及它们与任何构建器类的核心方法 `Bind`、`Return` 和 `ReturnFrom` 的关系。

在下一篇文章中，我们将继续研究包装器类型，包括使用列表作为包装器类型。

# 5 更多关于包装类型的信息

*Part of the "Computation Expressions" series (*[link](https://fsharpforfunandprofit.com/posts/computation-expressions-wrapper-types-part2/#series-toc)*)*

我们发现，即使是列表也可以是包装器类型
2013年1月24日 这篇文章已有3年多的历史了

https://fsharpforfunandprofit.com/posts/computation-expressions-wrapper-types-part2/

在上一篇文章中，我们研究了“包装器类型”的概念及其与计算表达式的关系。在这篇文章中，我们将研究哪些类型适合作为包装器类型。

## 哪些类型可以是包装类型？

如果每个计算表达式都必须有一个关联的包装器类型，那么什么类型的类型可以用作包装器类型？是否有任何适用的特殊约束或限制？

有一个一般规则，那就是：

- **任何具有泛型参数的类型都可以用作包装器类型**

例如，您可以使用 `Option<T>`、`DbResult<T>` 等作为包装器类型，如我们所见。您可以使用限制类型参数的包装器类型，例如 `Vector<int>`。

但是，其他泛型类型（如 `List<T>` 或 `IEnumerable<T>`）呢？它们肯定不能使用吗？事实上，是的，它们可以使用！我们拭目以待。

## 非泛型包装器类型可以工作吗？

是否可以使用没有泛型参数的包装器类型？

例如，我们在前面的示例中看到了对字符串进行加法的尝试，如下所示：`"1" + "2"`。在这种情况下，我们不能聪明地将 `string` 视为 `int` 的包装类型吗？那会很酷，是吗？

让我们试试。我们可以使用 `Bind` 和 `Return` 的签名来指导我们的实现。

- `Bind` 接受一个元组。元组的第一部分是包装类型（在这种情况下是 `string`），元组的第二部分是一个函数，它接受一个未包装的类型并将其转换为包装类型。在这种情况下，它将是 `int->string`。
- `Return` 接受一个未包装的类型（在本例中为 `int`）并将其转换为包装类型。因此，在这种情况下，`Return` 的签名将是 `int -> string`。

这如何指导实施？

- “重写”函数 `int -> string` 的实现很容易。它只是 int 上的“toString”。
- bind 函数必须将字符串解包为 int，然后将其传递给函数。我们可以使用 `int.Parse` 来实现这一点。
- 但是，如果绑定函数无法解包字符串，因为它不是一个有效的数字，会发生什么？在这种情况下，bind 函数仍然必须返回一个包装类型（字符串），因此我们可以只返回一个字符串，如“error”。

下面是构建器类的实现：

```F#
type StringIntBuilder() =

    member this.Bind(m, f) =
        let b,i = System.Int32.TryParse(m)
        match b,i with
        | false,_ -> "error"
        | true,i -> f i

    member this.Return(x) =
        sprintf "%i" x

let stringint = new StringIntBuilder()
```

现在我们可以尝试使用它：

```F#
let good =
    stringint {
        let! i = "42"
        let! j = "43"
        return i+j
        }
printfn "good=%s" good
```

如果其中一个字符串无效，会发生什么？

```F#
let bad =
    stringint {
        let! i = "42"
        let! j = "xxx"
        return i+j
        }
printfn "bad=%s" bad
```

这看起来真的很好——我们可以在工作流程中将字符串视为整数！

但是等等，有个问题。

假设我们给工作流一个输入，打开它（用 `let!`），然后立即重写它（用 `return`），而不做任何其他事情。应该怎么办？

```F#
let g1 = "99"
let g2 = stringint {
            let! i = g1
            return i
            }
printfn "g1=%s g2=%s" g1 g2
```

没问题。正如我们所料，输入 `g1` 和输出 `g2` 是相同的值。

但是错误案例呢？

```F#
let b1 = "xxx"
let b2 = stringint {
            let! i = b1
            return i
            }
printfn "b1=%s b2=%s" b1 b2
```

在这种情况下，我们出现了一些意想不到的行为。输入 `b1` 和输出 `b2` 不是相同的值。我们引入了一种不一致性。

这在实践中会成为问题吗？我不知道。但我会避免它，并使用一种不同的方法，比如选项，在所有情况下都是一致的。

## 使用包装器类型的工作流规则

这里有个问题？这两个代码片段之间有什么区别，它们的行为应该不同吗？

```F#
// fragment before refactoring
myworkflow {
    let wrapped = // some wrapped value
    let! unwrapped = wrapped
    return unwrapped
    }

// refactored fragment
myworkflow {
    let wrapped = // some wrapped value
    return! wrapped
    }
```

答案是否定的，他们不应该有不同的行为。唯一的区别是，在第二个例子中，未包装的值已被重构，包装后的值直接返回。

但正如我们在上一节中看到的，如果不小心，可能会出现不一致。因此，您创建的任何实现都应该遵循一些标准规则，这些规则是：

### 规则 1：如果你从一个未包装的值开始，然后包装它（使用 `return`），然后解包它（使用 `bind`），你应该总是能得到原始的未包装值。

这条规则和下一条规则是关于在打包和解包值时不丢失信息。显然，这是一个明智的要求，也是重构按预期工作所必需的。

在代码中，这将表示为这样：

```F#
myworkflow {
    let originalUnwrapped = something

    // wrap it
    let wrapped = myworkflow { return originalUnwrapped }

    // unwrap it
    let! newUnwrapped = wrapped

    // assert they are the same
    assertEqual newUnwrapped originalUnwrapped
    }
```

### 规则 2：如果你从一个包装好的值开始，然后你打开它（使用 `bind`），然后包装它（使用 `return`），你应该总是能得到原始的包装值。

这就是上面 `stringInt` 工作流打破的规则。与规则 1 一样，这显然是一项要求。

在代码中，这将表示为这样：

```F#
myworkflow {
    let originalWrapped = something

    let newWrapped = myworkflow {

        // unwrap it
        let! unwrapped = originalWrapped

        // wrap it
        return unwrapped
        }

    // assert they are the same
    assertEqual newWrapped originalWrapped
    }
```

### 规则 3：如果你创建了一个子工作流，它必须产生与你在主工作流中“内联”逻辑相同的结果。

此规则是组合正常运行所必需的，同样，“提取”重构只有在满足此条件时才能正常工作。

一般来说，如果你遵循一些指导方针（将在稍后的文章中解释），你会免费得到这个。

在代码中，这将表示为这样：

```F#
// inlined
let result1 = myworkflow {
    let! x = originalWrapped
    let! y = f x  // some function on x
    return! g y   // some function on y
    }

// using a child workflow ("extraction" refactoring)
let result2 = myworkflow {
    let! y = myworkflow {
        let! x = originalWrapped
        return! f x // some function on x
        }
    return! g y     // some function on y
    }

// rule
assertEqual result1 result2
```

## 按包装器类型列出

我之前说过，`List<T>` 或 `IEnumerable<T>` 等类型可以用作包装器类型。但这怎么可能呢？包装类型和未包装类型之间没有一一对应的关系！

这就是“包装器类型”类比有点误导的地方。相反，让我们回到绑定的想法，将 `bind` 视为一种将一个表达式的输出与另一个表达式输入连接起来的方式。

正如我们所看到的，`bind` 函数“解包”类型，并将连续函数应用于解包的值。但定义中并没有说必须只有一个未包裹的值。我们没有理由不能依次对列表中的每个项目应用延续函数。

换句话说，我们应该能够编写一个包含列表和延续函数的 `bind`，其中延续函数一次处理一个元素，如下所示：

```F#
bind( [1;2;3], fun elem -> // expression using a single element )
```

有了这个概念，我们应该能够像这样将一些绑定链接在一起：

```F#
let add =
    bind( [1;2;3], fun elem1 ->
    bind( [10;11;12], fun elem2 ->
        elem1 + elem2
    ))
```

但我们错过了一些重要的东西。传递到 `bind` 中的延续函数需要具有特定的签名。它需要一个未包裹的类型，但它会产生一个包裹的类型。

换句话说，continuation 函数必须始终创建一个新的列表作为其结果。

```F#
bind( [1;2;3], fun elem -> // expression using a single element, returning a list )
```

链式示例必须写成这样，`elem1 + elem2` 结果变成一个列表：

```F#
let add =
    bind( [1;2;3], fun elem1 ->
    bind( [10;11;12], fun elem2 ->
        [elem1 + elem2] // a list!
    ))
```

因此，我们 bind 方法的逻辑现在看起来像这样：

```F#
let bind(list,f) =
    // 1) for each element in list, apply f
    // 2) f will return a list (as required by its signature)
    // 3) the result is a list of lists
```

我们现在还有另一个问题。`Bind` 本身必须产生一个包装类型，这意味着“列表列表”是不好的。我们需要把它们重新变成一个简单的“一级”列表。

但这很容易——有一个列表模块函数可以做到这一点，称为 `concat`。

所以把它放在一起，我们有：

```F#
let bind(list,f) =
    list
    |> List.map f
    |> List.concat

let added =
    bind( [1;2;3], fun elem1 ->
    bind( [10;11;12], fun elem2 ->
//       elem1 + elem2    // error.
        [elem1 + elem2]   // correctly returns a list.
    ))
```

现在我们了解了 `bind` 本身是如何工作的，我们可以创建一个“列表工作流”。

- `Bind` 将连续函数应用于传入列表的每个元素，然后将生成的列表平铺为一级列表。`List.collect` 是一个库函数，它正是这样做的。
- `Return` 从未包装转换为包装。在这种情况下，这只意味着在列表中包装一个元素。

```F#
type ListWorkflowBuilder() =

    member this.Bind(list, f) =
        list |> List.collect f

    member this.Return(x) =
        [x]

let listWorkflow = new ListWorkflowBuilder()
```

以下是正在使用的工作流程：

```F#
let added =
    listWorkflow {
        let! i = [1;2;3]
        let! j = [10;11;12]
        return i+j
        }
printfn "added=%A" added

let multiplied =
    listWorkflow {
        let! i = [1;2;3]
        let! j = [10;11;12]
        return i*j
        }
printfn "multiplied=%A" multiplied
```

结果表明，第一个集合中的每个元素都已与第二个集合的每个元素组合在一起：

```F#
val added : int list = [11; 12; 13; 12; 13; 14; 13; 14; 15]
val multiplied : int list = [10; 11; 12; 20; 22; 24; 30; 33; 36]
```

这真是太神奇了。我们完全隐藏了列表枚举逻辑，只留下工作流本身。

### “for”的句法糖

如果我们将列表和序列视为特例，我们可以添加一些很好的语法糖来代替 `let!` 用一些更自然的东西。

我们能做的就是更换 `let!` 用 `for .. in .. do` 表达式：

```F#
// let version
let! i = [1;2;3] in [some expression]

// for..in..do version
for i in [1;2;3] do [some expression]
```

这两种变体的含义完全相同，只是看起来不同。

为了使 F# 编译器能够做到这一点，我们需要在构建器类中添加一个 `For` 方法。它通常具有与普通 `Bind` 方法完全相同的实现，但需要接受序列类型。

```F#
type ListWorkflowBuilder() =

    member this.Bind(list, f) =
        list |> List.collect f

    member this.Return(x) =
        [x]

    member this.For(list, f) =
        this.Bind(list, f)

let listWorkflow = new ListWorkflowBuilder()
```

以下是它的使用方法：

```F#
let multiplied =
    listWorkflow {
        for i in [1;2;3] do
        for j in [10;11;12] do
        return i*j
        }
printfn "multiplied=%A" multiplied
```

### LINQ和“列表工作流”

`for element in collection do` 看起来很熟悉吗？它与 LINQ 使用的语法 `from element in collection ...` 非常接近。事实上，LINQ 使用基本相同的技术来从像 `from element in collection ...` 的查询表达式语法转换为在幕后的实际方法调用。

在 F# 中，正如我们所看到的，`bind` 使用 `List.collect` 函数。LINQ 中 `List.collect` 的等价物是 `SelectMany` 扩展方法。一旦你了解了 `SelectMany` 的工作原理，你就可以自己实现相同类型的查询。Jon Skeet写了一篇有用的博客文章来解释这一点。

## 标识（identify）“包装器类型”

因此，我们在这篇文章中看到了许多包装器类型，并指出每个计算表达式都必须有一个相关的包装器类型。

但是上一篇文章中的日志示例呢？那里没有包装类型。有一个 `let!` 这在幕后做了一些事情，但输入类型与输出类型相同。类型保持不变。

对此的简短回答是，您可以将任何类型视为其自己的“包装器”。但还有另一种更深入的方法来理解这一点。

让我们退一步考虑一下像 `List<T>` 这样的包装器类型定义的真正含义。

如果你有一个像 `List<T>` 这样的类型，它实际上根本不是一个“真正的”类型。`List<int>` 是一个实类型，`List<string>` 是一种实类型。但是 `List<T>` 本身是不完整的。它缺少成为真正类型所需的参数。

考虑 `List<T>` 的一种方式是，它是一个函数，而不是一个类型。它是一个抽象类型世界中的函数，而不是普通值的具体世界，但就像任何函数一样，它将值映射到其他值，除了这种情况，输入值是类型（比如 `int` 或 `string`），输出值是其他类型（`List<int>` 和 `List<string>`）。与任何函数一样，它接受一个参数，在本例中为“类型参数”。这就是为什么 .NET 开发人员称之为“泛型”的这个概念在计算机科学术语中被称为“参数多态性”。

一旦我们掌握了从另一个类型生成一个类型的函数的概念（称为“类型构造函数”），我们就可以看到我们真正所说的“包装器类型”只是一个类型构造函数。

但是，如果一个“包装器类型”只是一个将一个类型映射到另一个类型的函数，那么一个将类型映射到同一类型的函数肯定属于这一类吗？确实如此。类型的“identity”函数符合我们的定义，可以用作计算表达式的包装器类型。

回到一些真实的代码，我们可以将“身份（identity）工作流”定义为工作流构建器的最简单实现。

```F#
type IdentityBuilder() =
    member this.Bind(m, f) = f m
    member this.Return(x) = x
    member this.ReturnFrom(x) = x

let identity = new IdentityBuilder()

let result = identity {
    let! x = 1
    let! y = 2
    return x + y
    }
```

有了这个，您可以看到前面讨论的日志示例只是添加了一些日志的身份工作流。

## 摘要

另一篇很长的帖子，我们讨论了很多话题，但我希望包装器类型的作用现在更清楚了。在本系列后面介绍常见的工作流（如“编写器工作流”和“状态工作流”）时，我们将看到如何在实践中使用包装器类型。

以下是本文所涵盖要点的总结：

- 计算表达式的一个主要用途是解包和重写存储在某种包装器类型中的值。
- 您可以轻松地组合计算表达式，因为 `Return` 的输出可以馈送到 `Bind` 的输入。
- 每个计算表达式都必须有一个关联的包装器类型。
- 任何具有泛型参数的类型都可以用作包装器类型，甚至是列表。
- 在创建工作流时，您应该确保您的实现符合关于包装和解包以及组合的三个合理规则。

# 6 实现 CE：Zero 和 Yield

*Part of the "Computation Expressions" series (*[link](https://fsharpforfunandprofit.com/posts/computation-expressions-builder-part1/#series-toc)*)*

开始使用基本的构建器方法
2013年1月25日 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/computation-expressions-builder-part1/

在介绍了 bind 和 continuation 以及包装器类型的使用后，我们终于准备好采用与“构建器”类相关的全套方法。

> 请注意，计算表达式上下文中的“构建器”与用于构造和验证对象的 OO “构建器模式”不同。这里有一篇关于“构建器模式”的帖子。

如果你查看 MSDN 文档，你不仅会看到 `Bind` 和 `Return`，还会看到其他名字奇怪的方法，如 `Delay` 和 `Zero`。它们是干什么用的？这就是这篇文章和接下来的几篇文章将要回答的问题。

## 行动计划

为了演示如何创建构建器类，我们将创建一个使用所有可能的构建器方法的自定义工作流。

但是，我们不会从顶部开始，试图在没有上下文的情况下解释这些方法的含义，而是从底部开始，从一个简单的工作流程开始，只在需要解决问题或错误时添加方法。在此过程中，您将详细了解 F# 如何处理计算表达式。

该过程的概述如下：

- 第 1 部分：在第一部分中，我们将了解基本工作流需要哪些方法。我们将介绍 `Zero`、`Yield`、`Combine` 和 `For`。
- 第 2 部分：接下来，我们将研究如何延迟代码的执行，以便只在需要时对其进行评估。我们将介绍 `Delay` 和 `Run`，并看看懒惰计算。
- 第 3 部分：最后，我们将介绍其余的方法：`While`、`Using` 和异常处理。

## 在我们开始之前

在我们开始创建工作流之前，这里有一些一般性的评论。

### 计算表达式的文档

首先，正如您可能已经注意到的那样，MSDN 的计算表达式文档充其量是很少的，虽然并不不准确，但可能会产生误导。例如，构建器方法的签名比它们看起来更灵活，这可用于实现一些如果仅从文档中工作可能不明显的功能。稍后我们将展示一个例子。

如果你想要更详细的文档，我可以推荐两个来源。关于计算表达式背后的概念的详细概述，Tomas Petricek 和 Don Syme 的论文《F# 表达式动物园》是一个很好的资源。对于最准确的最新技术文档，您应该阅读 F# 语言规范，其中有一节是关于计算表达式的。

### 包装和未包装类型

当你试图理解记录的签名时，请记住，我一直称之为“解包”类型的通常写为“`'T`”，而“打包”类型通常写为 `M<'T>`。也就是说，当你看到 `Return` 方法的签名为 `'T->M<'T>` 时，这意味着 `Return` 接受一个未包装的类型并返回一个包装的类型。

正如我在本系列前面的文章中所说的那样，我将继续使用“解包”和“包裹”来描述这些类型之间的关系，但随着我们向前发展，这些术语将被扩展到临界点，因此我也将开始使用其他术语，如“计算类型”而不是“包裹类型”。我希望，当我们达到这一点时，改变的原因将是明确和可以理解的。

此外，在我的示例中，我通常会使用以下代码来保持简单：

```F#
let! x = ...wrapped type value...
```

但这实际上过于简单化了。准确地说，“x”可以是任何模式，而不仅仅是一个值，当然，“包装类型”值可以是一个计算结果为包装类型的表达式。MSDN 文档使用了这种更精确的方法。它在定义中使用“模式”和“表达式”，例如 `let! pattern = expr in cexpr`。

以下是在 `maybe` 计算表达式中使用模式和表达式的一些示例，其中 `Option` 是包装类型，右侧表达式是 `options`：

```F#
// let! pattern = expr in cexpr
maybe {
    let! x,y = Some(1,2)
    let! head::tail = Some( [1;2;3] )
    // etc
    }
```

话虽如此，我将继续使用过于简单的例子，以免给已经复杂的话题增加额外的复杂性！

### 在构建器类中实现特殊方法（或不实现）

MSDN 文档显示，每个特殊操作（如 `for..in` 或 `yield`）都被转换为对构建器类中方法的一个或多个调用。

并不总是一对一的对应关系，但一般来说，为了支持特殊操作的语法，您必须在构建器类中实现相应的方法，否则编译器会抱怨并给您一个错误。

另一方面，如果你不需要语法，你不需要实现每一个方法。例如，我们已经很好地实现了 `maybe` 工作流，只实现了 `Bind` 和 `Return` 两个方法。如果我们不需要使用 `Delay`, `Use` 等，我们就不需要实现它们。

为了了解如果你没有实现一个方法会发生什么，让我们尝试使用 `for..in..do` 语法在我们的 `maybe` 工作流中是这样的：

```F#
maybe { for i in [1;2;3] do i }
```

我们将得到编译器错误：

```
This control construct may only be used if the computation expression builder defines a 'For' method
```

有时你会遇到一些可能很神秘的错误，除非你知道幕后发生了什么。例如，如果你忘记在工作流程中加入 `return`，如下所示：

```F#
maybe { 1 }
```

您将收到编译器错误：

```F#
This control construct may only be used if the computation expression builder defines a 'Zero' method
```

你可能会问：什么是Zero方法？我为什么需要它？答案就在眼前。

### 有和没有“!”的操作

显然，许多特殊操作都是成对的，有和没有“!”符号。例如：`let` 和 `let!`（发音为“let-bang”），`return` 和 `return!`，`yield` 和 `yield!` 等等。

当你意识到没有“!”的操作总是在右侧有解包类型时，这种区别很容易记住，而有“!”操作总是有包装类型。

例如，使用 `maybe` 工作流，其中 `Option` 是包装类型，我们可以比较不同的语法：

```F#
let x = 1           // 1 is an "unwrapped" type
let! x = (Some 1)   // Some 1 is a "wrapped" type
return 1            // 1 is an "unwrapped" type
return! (Some 1)    // Some 1 is a "wrapped" type
yield 1             // 1 is an "unwrapped" type
yield! (Some 1)     // Some 1 is a "wrapped" type
```

“!”版本对于组合尤为重要，因为包装类型可能是同一类型的另一个计算表达式的结果。

```F#
let! x = maybe {...)       // "maybe" returns a "wrapped" type

// bind another workflow of the same type using let!
let! aMaybe = maybe {...)  // create a "wrapped" type
return! aMaybe             // return it

// bind two child asyncs inside a parent async using let!
let processUri uri = async {
    let! html = webClient.AsyncDownloadString(uri)
    let! links = extractLinks html
    ... etc ...
    }
```

## 深入研究-创建工作流的最小实现

开始吧！我们将首先创建一个最小版本的“maybe”工作流（我们将重命名为“trace”），其中包含每个方法，这样我们就可以看到发生了什么。我们将在整篇文章中使用它作为我们的测试平台。

以下是 `trace` 工作流第一个版本的代码：

```F#
type TraceBuilder() =
    member this.Bind(m, f) =
        match m with
        | None ->
            printfn "Binding with None. Exiting."
        | Some a ->
            printfn "Binding with Some(%A). Continuing" a
        Option.bind f m

    member this.Return(x) =
        printfn "Returning a unwrapped %A as an option" x
        Some x

    member this.ReturnFrom(m) =
        printfn "Returning an option (%A) directly" m
        m

// make an instance of the workflow
let trace = new TraceBuilder()
```

我希望这里没有什么新鲜事。我们以前已经见过所有这些方法。

现在让我们运行一些示例代码：

```F#
trace {
    return 1
    } |> printfn "Result 1: %A"

trace {
    return! Some 2
    } |> printfn "Result 2: %A"

trace {
    let! x = Some 1
    let! y = Some 2
    return x + y
    } |> printfn "Result 3: %A"

trace {
    let! x = None
    let! y = Some 1
    return x + y
    } |> printfn "Result 4: %A"
```

一切都应该按预期工作，特别是，你应该能够看到，在第四个例子中使用 `None` 导致接下来的两行（`let! y = ... return x+y`）被跳过，整个表达式的结果为 `None`。

## 介绍“do!”

我们的表达支持 `let!`，但是 `do!` 呢？

在普通 F# 中，`do` 就像 `let`，除了表达式不返回任何有用的东西（即单位值）。

在计算表达式中，`do!` 非常相似。就像 `let!` 将包装好的结果传递给 `Bind` 方法，`do!` 也可以，除了 `do!` “result”是 unit 值，因此将 unit 的包装版本传递给 bind 方法。

以下是一个使用 `trace` 工作流的简单演示：

```F#
trace {
    do! Some (printfn "...expression that returns unit")
    do! Some (printfn "...another expression that returns unit")
    let! x = Some (1)
    return x
    } |> printfn "Result from do: %A"
```

输出如下：

```
...expression that returns unit
Binding with Some(<null>). Continuing
...another expression that returns unit
Binding with Some(<null>). Continuing
Binding with Some(1). Continuing
Returning a unwrapped 1 as an option
Result from do: Some 1
```

您可以自己验证每次 `do!` 后是否会将 `unit option` 传递给 `Bind`。

## 引入“零”

你能逃脱惩罚的最小计算表达式是什么？让我们什么都不尝试：

```F#
trace {
    } |> printfn "Result for empty: %A"
```

我们立即得到一个错误：

```
This value is not a function and cannot be applied
```

很公平。如果你仔细想想，在计算表达式中什么都没有是没有意义的。毕竟，它的目的是将表达式链接在一起。

接下来，一个没有 `let!` 或 `return` 的简单表达式怎么样？

```F#
trace {
    printfn "hello world"
    } |> printfn "Result for simple expression: %A"
```

现在我们得到一个不同的错误：

```
This control construct may only be used if the computation expression builder defines a 'Zero' method
```

那么，为什么现在需要 `Zero` 方法，而我们以前不需要它呢？答案是，在这种特殊情况下，我们没有显式返回任何内容，但计算表达式作为一个整体必须返回一个包装值。那么它应该返回什么值呢？

事实上，只要没有明确给出计算表达式的返回值，这种情况就会发生。如果你有一个没有 else 子句的 `if..then` 表达式，也会发生同样的事情。

```F#
trace {
    if false then return 1
    } |> printfn "Result for if without else: %A"
```

在正常的 F# 代码中，没有“else”的“if..then”会产生一个单位值，但在计算表达式中，特定的返回值必须是包装类型的成员，编译器不知道这是什么值。

修复方法是告诉编译器使用什么——这就是 `Zero` 方法的目的。

### 你应该为 Zero 使用什么值？

那么，你应该为 `Zero` 使用哪个值呢？这取决于您创建的工作流类型。

以下是一些可能有所帮助的指南：

- **工作流程是否有“成功”或“失败”的概念**？如果是这样，请将“failure”值设为 `Zero`。例如，在我们的 `trace` 工作流中，我们使用 `None` 表示失败，因此我们可以使用 `None` 作为零值。
- **工作流是否有“顺序处理”的概念**？也就是说，在您的工作流程中，您会执行一个步骤，然后是另一个步骤和一些幕后处理。在正常的 F# 代码中，一个没有显式返回任何内容的表达式将计算为 unit。因此，与此情况类似，您的 `Zero` 应该是 unit 的包装版本。例如，在基于选项的工作流的变体中，我们可能会使用 `Some()` 表示 `Zero`（顺便说一句，这也总是与 `Return()` 相同）。
- **工作流程主要涉及操纵数据结构吗**？如果是这样，`Zero` 应该是“空”数据结构。例如，在“列表生成器”工作流中，我们将使用空列表作为零值。

在组合包装类型时，零值也起着重要作用。请继续关注，我们将在下一篇文章中再次讨论 `Zero`。

### Zero 实现

现在，让我们用一个返回 `None` 的 `Zero` 方法扩展我们的 testbed 类，然后重试。

```F#
type TraceBuilder() =
    // other members as before
    member this.Zero() =
        printfn "Zero"
        None

// make a new instance
let trace = new TraceBuilder()

// test
trace {
    printfn "hello world"
    } |> printfn "Result for simple expression: %A"

trace {
    if false then return 1
    } |> printfn "Result for if without else: %A"
```

测试代码清楚地表明，`Zero` 是在幕后调用的。`None` 是整个表达式的返回值。*注意：没有一个可以打印为 `<null>`。你可以忽略这一点。*

### 你总是需要一个 Zero 吗？

记住，你不需要有一个 `Zero`，但前提是它在工作流的上下文中有意义。例如，`seq` 不允许为零，但 `async` 允许为零：

```F#
let s = seq {printfn "zero" }    // Error
let a = async {printfn "zero" }  // OK
```

## 引入“Yield”

在 C# 中，有一个“yield”语句，在迭代器中，用于提前返回，然后在返回时从您停止的地方继续。

查看文档，F# 计算表达式中也有一个“yield”。它有什么作用？让我们试试看。

```F#
trace {
    yield 1
    } |> printfn "Result for yield: %A"
```

我们得到错误：

```F#
This control construct may only be used if the computation expression builder defines a 'Yield' method
```

这并不奇怪。那么，“yield”方法的实施应该是什么样子的呢？MSDN 文档说它具有签名 `'T->M<T>`，这与 `Return` 方法的签名完全相同。它必须取一个未包装的值并包装它。

因此，让我们以与 `Return` 相同的方式实现它，然后重试测试表达式。

```F#
type TraceBuilder() =
    // other members as before

    member this.Yield(x) =
        printfn "Yield an unwrapped %A as an option" x
        Some x

// make a new instance
let trace = new TraceBuilder()

// test
trace {
    yield 1
    } |> printfn "Result for yield: %A"
```

这现在奏效了，而且似乎可以作为 `return` 的精确替代品。

还有一个与 `ReturnFrom` 方法类似的 `YieldFrom` 方法。它的行为方式也是一样的，允许您生成一个包裹的值，而不是一个未包裹的值。

因此，让我们也将其添加到构建器方法列表中：

```F#
type TraceBuilder() =
    // other members as before

    member this.YieldFrom(m) =
        printfn "Yield an option (%A) directly" m
        m

// make a new instance
let trace = new TraceBuilder()

// test
trace {
    yield! Some 1
    } |> printfn "Result for yield!: %A"
```

此时，您可能会想：如果 `return` 和 `yield` 基本上是同一件事，为什么有两个不同的关键字？答案主要是为了通过实现一个而不是另一个来强制执行适当的语法。例如，`seq` 表达式允许 `yield` 但不允许 `return`，而 `async` 允许 `return` 但不允许 `yield`，如下面的代码片段所示。

```F#
let s = seq {yield 1}    // OK
let s = seq {return 1}   // error

let a = async {return 1} // OK
let a = async {yield 1}  // error
```

事实上，您可以为 `return` 和 `yield` 创建稍微不同的行为，例如，使用 `return` 会停止计算表达式的其余部分，而 `yield` 则不会。

当然，更一般地说，`yield` 应用于序列/枚举语义，而 `return` 通常每个表达式使用一次。（我们将在下一篇文章中看到如何多次使用 `yield`。）

## 重温“For”

我们讨论了 `for..in..do` 语法在上一篇文章中。现在，让我们重新审视我们之前讨论过的“列表生成器”，并添加额外的方法。我们在上一篇文章中已经看到了如何为列表定义 `Bind` 和 `Return`，所以我们只需要实现额外的方法。

- `Zero` 方法只返回一个空列表。
- `Yield` 方法的实现方式与 `Return` 相同。
- `For` 方法的实现方式与 `Bind` 相同。

```F#
type ListBuilder() =
    member this.Bind(m, f) =
        m |> List.collect f

    member this.Zero() =
        printfn "Zero"
        []

    member this.Return(x) =
        printfn "Return an unwrapped %A as a list" x
        [x]

    member this.Yield(x) =
        printfn "Yield an unwrapped %A as a list" x
        [x]

    member this.For(m,f) =
        printfn "For %A" m
        this.Bind(m,f)

// make an instance of the workflow
let listbuilder = new ListBuilder()
```

以下是使用 `let!` 的代码：

```F#
listbuilder {
    let! x = [1..3]
    let! y = [10;20;30]
    return x + y
    } |> printfn "Result: %A"
```

以下是用 `for` 的等效代码：

```F#
listbuilder {
    for x in [1..3] do
    for y in [10;20;30] do
    return x + y
    } |> printfn "Result: %A"
```

你可以看到这两种方法都给出了相同的结果。

## 摘要

在这篇文章中，我们看到了如何实现一个简单计算表达式的基本方法。

有几点需要重申：

- 对于简单的表达式，您不需要实现所有的方法。
- 有感叹号的东西在右手边有包裹型。
- 没有感叹号的东西在右手边有未包裹的类型。
- 如果你想要一个不显式返回值的工作流，你需要实现 `Zero`。
- `Yield` 基本上等同于 `Return`，但 `Yield` 应用于序列/枚举语义。
- `For` 在简单情况下基本等同于 `Bind`。

在下一篇文章中，我们将看看当我们需要组合多个值时会发生什么。

# 7 实现 CE：Combine

如何一次返回多个值
2013年1月26日 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/computation-expressions-builder-part2/

在这篇文章中，我们将研究使用 `Combine` 方法从计算表达式中返回多个值。

> 请注意，计算表达式上下文中的“构建器”与用于构造和验证对象的 OO“构建器模式”不同。这里有一篇关于“构建器模式”的帖子。

## 到目前为止的故事…

到目前为止，我们的表达式构建器类看起来像这样：

```F#
type TraceBuilder() =
    member this.Bind(m, f) =
        match m with
        | None ->
            printfn "Binding with None. Exiting."
        | Some a ->
            printfn "Binding with Some(%A). Continuing" a
        Option.bind f m

    member this.Return(x) =
        printfn "Returning a unwrapped %A as an option" x
        Some x

    member this.ReturnFrom(m) =
        printfn "Returning an option (%A) directly" m
        m

    member this.Zero() =
        printfn "Zero"
        None

    member this.Yield(x) =
        printfn "Yield an unwrapped %A as an option" x
        Some x

    member this.YieldFrom(m) =
        printfn "Yield an option (%A) directly" m
        m

// make an instance of the workflow
let trace = new TraceBuilder()
```

到目前为止，这门课做得很好。但我们即将遇到一个问题…

## 两个“yield”的问题

之前，我们看到了如何使用 `yield` 来返回值，就像 `return` 一样。

当然，通常情况下，`yield` 不是只使用一次，而是多次使用，以便在枚举等过程的不同阶段返回值。那么，让我们尝试一下：

```F#
trace {
    yield 1
    yield 2
    } |> printfn "Result for yield then yield: %A"
```

但是，我们收到一条错误消息：

```
This control construct may only be used if the computation expression builder defines a 'Combine' method.
```

如果你使用 `return` 而不是 `yield`，你会得到同样的错误。

```F#
trace {
    return 1
    return 2
    } |> printfn "Result for return then return: %A"
```

这个问题也发生在其他情况下。例如，如果我们想做某事然后返回，就像这样：

```F#
trace {
    if true then printfn "hello"
    return 1
    } |> printfn "Result for if then return: %A"
```

我们收到了关于缺少“Combine”方法的相同错误消息。

## 理解问题

那么，这里发生了什么？

为了理解，让我们回到计算表达式的幕后视图。我们已经看到，`return` 和 `yield` 实际上只是一系列延续中的最后一步，就像这样：

```F#
Bind(1,fun x ->
   Bind(2,fun y ->
     Bind(x + y,fun z ->
        Return(z)  // or Yield
```

如果你愿意，你可以把 `return`（或 `yield`）看作是“重置”缩进。因此，当我们 `return/yield`，然后再次 `return/yield` 时，我们生成的代码如下：

```F#
Bind(1,fun x ->
   Bind(2,fun y ->
     Bind(x + y,fun z ->
        Yield(z)
// start a new expression
Bind(3,fun w ->
   Bind(4,fun u ->
     Bind(w + u,fun v ->
        Yield(v)
```

但实际上，这可以简化为：

```F#
let value1 = some expression
let value2 = some other expression
```

换句话说，我们的计算表达式中现在有两个值。然后一个明显的问题是，这两个值应该如何组合起来，为整个计算表达式提供一个单一的结果？

这一点非常重要。**Return 和 yield 不会从计算表达式中生成早期返回**。不，整个计算表达式，一直到最后一个花括号，总是被求值并得到一个值。让我重复一遍。计算表达式的每个部分都会被求值——不会发生短路。如果我们想短路并提前返回，我们必须编写自己的代码来实现这一点（稍后我们将看到如何实现）。

那么，回到紧迫的问题。我们有两个表达式产生两个值：如何将这些多个值组合成一个？

## 介绍“Combine”

答案是使用 `Combine` 方法，该方法接受两个包装值并将其组合成另一个包装值。具体如何运作取决于我们。

在我们的例子中，我们专门处理 `int options`，所以一个简单的实现会让人想到把数字加在一起。当然，每个参数都是一个 `option`（包装类型），因此我们需要将它们分开并处理四种可能的情况：

```F#
type TraceBuilder() =
    // other members as before

    member this.Combine (a,b) =
        match a,b with
        | Some a', Some b' ->
            printfn "combining %A and %A" a' b'
            Some (a' + b')
        | Some a', None ->
            printfn "combining %A with None" a'
            Some a'
        | None, Some b' ->
            printfn "combining None with %A" b'
            Some b'
        | None, None ->
            printfn "combining None with None"
            None

// make a new instance
let trace = new TraceBuilder()
```

再次运行测试代码：

```F#
trace {
    yield 1
    yield 2
    } |> printfn "Result for yield then yield: %A"
```

但现在我们得到一个不同的错误消息：

```
This control construct may only be used if the computation expression builder defines a 'Delay' method
```

`Delay` 方法是一个钩子，它允许您将计算表达式的求值延迟到需要的时候——我们很快就会详细讨论这个问题；但现在，让我们创建一个默认实现：

```F#
type TraceBuilder() =
    // other members as before

    member this.Delay(f) =
        printfn "Delay"
        f()

// make a new instance
let trace = new TraceBuilder()
```

再次运行测试代码：

```F#
trace {
    yield 1
    yield 2
    } |> printfn "Result for yield then yield: %A"
```

最后，我们完成了代码。

```
Delay
Yield an unwrapped 1 as an option
Delay
Yield an unwrapped 2 as an option
combining 1 and 2
Result for yield then yield: Some 3
```

整个工作流程的结果是所有收益的总和，即 `Some 3`。

如果工作流中出现“失败”（例如“`None`”），则不会出现第二个结果，总体结果为 `Some 1`。

```F#
trace {
    yield 1
    let! x = None
    yield 2
    } |> printfn "Result for yield then None: %A"
```

我们可以有三个 `yield`，而不是两个：

```F#
trace {
    yield 1
    yield 2
    yield 3
    } |> printfn "Result for yield x 3: %A"
```

结果正如你所料，`Some 6`。

我们甚至可以尝试将 `yield` 和 `return` 混合在一起。除了语法上的差异，整体效果是相同的。

```F#
trace {
    yield 1
    return 2
    } |> printfn "Result for yield then return: %A"

trace {
    return 1
    return 2
    } |> printfn "Result for return then return: %A"
```

## 使用 Combine 生成序列

将数字相加并不是真正的 `yield` 点，尽管您可能会使用类似的想法来构造连接的字符串，有点像 `StringBuilder`。

不，`yield` 自然被用作序列生成的一部分，现在我们了解了 `Combine`，我们可以用所需的方法扩展我们的“ListBuilder”工作流（从上次开始）。

- `Combine` 方法只是列表连接。
- `Delay` 方法现在可以使用默认实现。

以下是整个类：

```F#
type ListBuilder() =
    member this.Bind(m, f) =
        m |> List.collect f

    member this.Zero() =
        printfn "Zero"
        []

    member this.Yield(x) =
        printfn "Yield an unwrapped %A as a list" x
        [x]

    member this.YieldFrom(m) =
        printfn "Yield a list (%A) directly" m
        m

    member this.For(m,f) =
        printfn "For %A" m
        this.Bind(m,f)

    member this.Combine (a,b) =
        printfn "combining %A and %A" a b
        List.concat [a;b]

    member this.Delay(f) =
        printfn "Delay"
        f()

// make an instance of the workflow
let listbuilder = new ListBuilder()
```

它在这里使用：

```F#
listbuilder {
    yield 1
    yield 2
    } |> printfn "Result for yield then yield: %A"

listbuilder {
    yield 1
    yield! [2;3]
    } |> printfn "Result for yield then yield! : %A"
```

这里有一个更复杂的例子，有一个 `for` 循环和一些 `yield`。

```F#
listbuilder {
    for i in ["red";"blue"] do
        yield i
        for j in ["hat";"tie"] do
            yield! [i + " " + j;"-"]
    } |> printfn "Result for for..in..do : %A"
```

结果是：

```
["red"; "red hat"; "-"; "red tie"; "-"; "blue"; "blue hat"; "-"; "blue tie"; "-"]
```

你可以看到，通过结合 `for..in..do` 和 `yield`，我们离内置的 `seq` 表达式语法不远（当然，除了 `seq` 是懒惰的）。

我强烈建议你尝试一下，直到你清楚幕后发生了什么。正如你从上面的例子中看到的，你可以创造性地使用 `yield` 来生成各种不规则的列表，而不仅仅是简单的列表。

*注意：如果你想知道 `While`，我们会推迟一段时间，直到我们在下一篇文章中看到 `Delay`。*

## “combine”的处理顺序

`Combine` 方法只有两个参数。那么，当你组合两个以上的值时会发生什么？例如，这里有四个值要组合：

```F#
listbuilder {
    yield 1
    yield 2
    yield 3
    yield 4
    } |> printfn "Result for yield x 4: %A"
```

如果你查看输出，你可以看到这些值是成对组合的，正如你所料。

```
combining [3] and [4]
combining [2] and [3; 4]
combining [1] and [2; 3; 4]
Result for yield x 4: [1; 2; 3; 4]
```

一个微妙但重要的一点是，它们是“反向”组合的，从最后一个值开始。首先将“3”与“4”组合，然后将其结果与“2”组合，依此类推。

合并

## 非序列组合

在我们前面的第二个有问题的例子中，我们没有序列；我们刚刚连续有两个单独的表达式。

```F#
trace {
    if true then printfn "hello"  //expression 1
    return 1                      //expression 2
    } |> printfn "Result for combine: %A"
```

这些表达应该如何组合？

根据工作流支持的概念，有许多常见的方法可以做到这一点。

### 实施“成功”或“失败”的工作流组合

如果工作流有“成功”或“失败”的概念，那么标准方法是：

- 如果第一个表达式“success”（无论在上下文中是什么意思），则使用该值。
- 否则，使用第二个表达式的值。

在这种情况下，我们通常也使用“failure”值作为 `Zero`。

这种方法对于将一系列“否则”表达式链接在一起非常有用，其中第一个成功“获胜”并成为整体结果。

```
if (do first expression)
or else (do second expression)
or else (do third expression)
```

例如，对于 `maybe` 工作流，如果第一个表达式是 `Some`，则通常返回第一个表达式，否则返回第二个表达式，如下所示：

```F#
type TraceBuilder() =
    // other members as before

    member this.Zero() =
        printfn "Zero"
        None  // failure

    member this.Combine (a,b) =
        printfn "Combining %A with %A" a b
        match a with
        | Some _ -> a  // a succeeds -- use it
        | None -> b    // a fails -- use b instead

// make a new instance
let trace = new TraceBuilder()
```

### 示例：解析

让我们用这个实现来尝试一个解析示例：

```F#
type IntOrBool = I of int | B of bool

let parseInt s =
    match System.Int32.TryParse(s) with
    | true,i -> Some (I i)
    | false,_ -> None

let parseBool s =
    match System.Boolean.TryParse(s) with
    | true,i -> Some (B i)
    | false,_ -> None

trace {
    return! parseBool "42"  // fails
    return! parseInt "42"
    } |> printfn "Result for parsing: %A"
```

我们得到以下结果：

```
Some (I 42)
```

你可以看到第一次 `return!` 表达式为 `None`，并被忽略。因此，总体结果是第二个表达式 `Some(I 42)`。

### 示例：字典查找

在这个例子中，我们将尝试在多个字典中查找相同的键，并在找到值时返回：

```F#
let map1 = [ ("1","One"); ("2","Two") ] |> Map.ofList
let map2 = [ ("A","Alice"); ("B","Bob") ] |> Map.ofList

trace {
    return! map1.TryFind "A"
    return! map2.TryFind "A"
    } |> printfn "Result for map lookup: %A"
```

我们得到以下结果：

```
Result for map lookup: Some "Alice"
```

您可以看到第一次查找为“`None`”，并被忽略。因此，总体结果是第二次查找。

正如您所看到的，在解析或评估一系列（可能不成功）操作时，这种技术非常方便。

### 为具有连续步骤的工作流实施组合

如果工作流程具有顺序步骤的概念，那么总体结果只是最后一步的值，并且所有先前步骤仅针对其副作用进行评估。

在普通 F# 中，这将被写为：

```
do some expression
do some other expression
final expression
```

或者使用分号语法，只需：

```
some expression; some other expression; final expression
```

在普通 F# 中，每个表达式（除最后一个表达式外）的计算结果都是单位值。

计算表达式的等效方法是将每个表达式（除最后一个表达式外）视为包装的单位值，并将其“传递给”下一个表达式，依此类推，直到到达最后一个表达。

当然，这正是 bind 所做的，因此最简单的实现就是重用 `Bind` 方法本身。此外，为了使这种方法奏效，`Zero` 是包装unit 值非常重要。

```F#
type TraceBuilder() =
    // other members as before

    member this.Zero() =
        printfn "Zero"
        this.Return ()  // unit not None

    member this.Combine (a,b) =
        printfn "Combining %A with %A" a b
        this.Bind( a, fun ()-> b )

// make a new instance
let trace = new TraceBuilder()
```

与普通绑定的不同之处在于，延续有一个单位参数，其计算结果为 `b`。这反过来又迫使 `a` 通常为 `WrapperType<unit>` 类型，或者在我们的情况下为 `unit option` 类型。

下面是一个使用 `Combine` 实现的顺序处理示例：

```F#
trace {
    if true then printfn "hello......."
    if false then printfn ".......world"
    return 1
    } |> printfn "Result for sequential combine: %A"
```

这是以下的 trace。请注意，整个表达式的结果是序列中最后一个表达式的结果，就像正常的 F# 代码一样。

```
hello.......
Zero
Returning a unwrapped <null> as an option
Zero
Returning a unwrapped <null> as an option
Returning a unwrapped 1 as an option
Combining Some null with Some 1
Combining Some null with Some 1
Result for sequential combine: Some 1
```

### 为构建数据结构的工作流实施组合

最后，工作流的另一种常见模式是构建数据结构。在这种情况下，`Combine` 应该以任何合适的方式合并这两个数据结构。如果需要（甚至可能），`Zero` 方法应该创建一个空数据结构。

在上面的“列表生成器”示例中，我们正是使用了这种方法。`Combine` 只是列表连接，`Zero` 是空列表。

## 混合“Combine”和“Zero”的指南

我们已经研究了选项类型的 `Combine` 的两种不同实现。

- 第一个使用选项作为“成功/失败”指标，当第一个成功“获胜”时。在这种情况下，`Zero` 被定义为 `None`
- 第二个是连续的，在这种情况下，`Zero` 被定义为 `Some ()`

这两种情况都很好，但这是运气，还是有正确实现 `Combine` 和 `Zero` 的指导方针？

首先，请注意，如果交换参数，`Combine` 不必给出相同的结果。也就是说，`Combine(a, b)` 不必与 `Combine(b, a)` 相同。列表生成器就是一个很好的例子。

另一方面，有一个有用的规则将 `Zero` 和 `Combine` 联系起来。

### 规则：`Combine(a, Zero)` 应与 `Combine(Zero, a)` 相同，后者应仅与 `a` 相同。

使用算术中的类比，你可以想到类似于 `Combine` 的加法（这是一个不错的类比——它实际上是“加”两个值）。当然，`Zero` 只是数字零！因此，上述规则可以表示为：

### 规则：`a + 0` 与 `0 + a` 相同，且与 `a` 相同，其中 `+` 表示 `Combine`，`0` 表示 `Zero`。

如果你查看选项类型的第一个 `Combine` 实现（“成功/失败”），你会发现它确实符合这一规则，第二个实现（用 `Some()` 进行“绑定”）也是如此。

另一方面，如果我们使用 `Combine` 的“绑定”实现，但将 `Zero` 定义为 `None`，它就不会遵守加法规则，这将是我们出了问题的一个线索。

## 不用绑定“合并”

与所有构建器方法一样，如果你不需要它们，你也不需要实现它们。因此，对于一个顺序性很强的工作流，你可以很容易地创建一个包含 `Combine`、`Zero` 和 `Yield` 的构建器类，而根本不必实现 `Bind` 和 `Return`。

以下是一个可行的最小实现示例：

```F#
type TraceBuilder() =

    member this.ReturnFrom(x) = x

    member this.Zero() = Some ()

    member this.Combine (a,b) =
        a |> Option.bind (fun ()-> b )

    member this.Delay(f) = f()

// make an instance of the workflow
let trace = new TraceBuilder()
```

它在这里使用：

```F#
trace {
    if true then printfn "hello......."
    if false then printfn ".......world"
    return! Some 1
    } |> printfn "Result for minimal combine: %A"
```

同样，如果你有一个面向数据结构的工作流，你可以实现 `Combine` 和其他一些助手。例如，这是我们的列表生成器类的一个最小实现：

```F#
type ListBuilder() =

    member this.Yield(x) = [x]

    member this.For(m,f) =
        m |> List.collect f

    member this.Combine (a,b) =
        List.concat [a;b]

    member this.Delay(f) = f()

// make an instance of the workflow
let listbuilder = new ListBuilder()
```

即使是最小的实现，我们也可以编写这样的代码：

```F#
listbuilder {
    yield 1
    yield 2
    } |> printfn "Result: %A"

listbuilder {
    for i in [1..5] do yield i + 2
    yield 42
    } |> printfn "Result: %A"
```

## 独立的“合并”功能

在上一篇文章中，我们看到“bind”函数经常被用作独立函数，并且通常被赋予运算符 `>>=`。

`Combine` 函数也经常被用作独立函数。与 bind 不同，它没有标准符号——它可以根据组合函数的工作方式而变化。

对称组合操作通常写成 `++` 或 `<+>`。我们之前用于选项的“左偏”组合（即，只有在第一个表达式失败时才执行第二个表达式）有时被写为 `<++`。

因此，这是一个独立的左偏选项组合的示例，如字典查找示例中使用的。

```F#
module StandaloneCombine =

    let combine a b =
        match a with
        | Some _ -> a  // a succeeds -- use it
        | None -> b    // a fails -- use b instead

    // create an infix version
    let ( <++ ) = combine

    let map1 = [ ("1","One"); ("2","Two") ] |> Map.ofList
    let map2 = [ ("A","Alice"); ("B","Bob") ] |> Map.ofList

    let result =
        (map1.TryFind "A")
        <++ (map1.TryFind "B")
        <++ (map2.TryFind "A")
        <++ (map2.TryFind "B")
        |> printfn "Result of adding options is: %A"
```

## 摘要

我们在这篇文章中学到了什么关于 `Combine` 的知识？

- 如果需要在计算表达式中组合或“添加”多个包装值，则需要实现 `Combine`（和 `Delay`）。
- `Combine` 从最后一个到第一个成对组合值。
- `Combine` 没有在所有情况下都有效的通用实现——它需要根据工作流程的特定需求进行定制。
- 有一条合理的规则将 `Combine` 与 `Zero` 联系起来。
- `Combine` 不需要实现 `Bind`。
- `Combine` 可以作为独立函数公开

在下一篇文章中，我们将添加逻辑来精确控制何时对内部表达式进行求值，并介绍真正的短路和延迟求值。

# 8 实现 CE：Delay 和 Run

*Part of the "Computation Expressions" series (*[link](https://fsharpforfunandprofit.com/posts/computation-expressions-builder-part3/#series-toc)*)*

控制功能何时执行
27 Jan 2013 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/computation-expressions-builder-part3/

在最近的几篇文章中，我们介绍了创建自己的计算表达式构建器所需的所有基本方法（Bind、Return、Zero 和 Combine）。在这篇文章中，我们将通过控制表达式的求值时间来了解使工作流更高效所需的一些额外功能。

> 请注意，计算表达式上下文中的“构建器”与用于构造和验证对象的OO“构建器模式”不同。这里有一篇关于“构建器模式”的帖子。

## 问题：避免不必要的评估

假设我们像以前一样创建了一个“maybe”风格的工作流。但这一次，我们希望使用“return”关键字提前返回并停止任何正在进行的处理。

这是我们完整的建设者类。要查看的关键方法是 `Combine`，在该方法中，我们只需忽略第一个返回值后的任何二级表达式。

```F#
type TraceBuilder() =
    member this.Bind(m, f) =
        match m with
        | None ->
            printfn "Binding with None. Exiting."
        | Some a ->
            printfn "Binding with Some(%A). Continuing" a
        Option.bind f m

    member this.Return(x) =
        printfn "Return an unwrapped %A as an option" x
        Some x

    member this.Zero() =
        printfn "Zero"
        None

    member this.Combine (a,b) =
        printfn "Returning early with %A. Ignoring second part: %A" a b
        a

    member this.Delay(f) =
        printfn "Delay"
        f()

// make an instance of the workflow
let trace = new TraceBuilder()
```

让我们通过打印一些东西、返回，然后打印其他东西来看看它是如何工作的：

```F#
trace {
    printfn "Part 1: about to return 1"
    return 1
    printfn "Part 2: after return has happened"
    } |> printfn "Result for Part1 without Part2: %A"
```

调试输出应该如下所示，我已经对其进行了注释：

```
// first expression, up to "return"
Delay
Part 1: about to return 1
Return an unwrapped 1 as an option

// second expression, up to last curly brace.
Delay
Part 2: after return has happened
Zero   // zero here because no explicit return was given for this part

// combining the two expressions
Returning early with Some 1. Ignoring second part: <null>

// final result
Result for Part1 without Part2: Some 1
```

我们可以在这里看到一个问题。尽管我们试图早点回来，但“第二部分：回来后”还是打印出来了。

为什么？好吧，我将重复我在上一篇文章中说过的话：**return 和 yield 不会从计算表达式中生成早期返回**。整个计算表达式，一直到最后一个花括号，总是被求值并得到一个值。

这是一个问题，因为您可能会得到不必要的副作用（例如在这种情况下打印消息），并且您的代码正在做一些不必要的事情，这可能会导致性能问题。

那么，我们如何避免在需要之前评估第二部分呢？

## 引入“Delay”

这个问题的答案很简单——只需将表达式的第 2 部分包装在一个函数中，只在需要时调用这个函数，就像这样。

```F#
let part2 =
    fun () ->
        printfn "Part 2: after return has happened"
        // do other stuff
        // return Zero

// only evaluate if needed
if needed then
   let result = part2()
```

使用此技术，可以完全处理计算表达式的第 2 部分，但由于表达式返回一个函数，因此在调用函数之前实际上不会发生任何事情。但是 `Combine` 方法永远不会调用它，因此它里面的代码根本不会运行。

这正是 `Delay` 方法的用途。`Return` 或 `Yield` 的任何结果都会立即被包裹在这样的“延迟”函数中，然后您可以选择是否运行它。

让我们更改构建器以实现延迟：

```F#
type TraceBuilder() =
    // other members as before

    member this.Delay(funcToDelay) =
        let delayed = fun () ->
            printfn "%A - Starting Delayed Fn." funcToDelay
            let delayedResult = funcToDelay()
            printfn "%A - Finished Delayed Fn. Result is %A" funcToDelay delayedResult
            delayedResult  // return the result

        printfn "%A - Delaying using %A" funcToDelay delayed
        delayed // return the new function
```

如您所见，`Delay` 方法有一个要执行的函数。之前，我们立即执行了它。我们现在正在做的是将这个函数包装在另一个函数中，并返回延迟函数。在函数包装前后，我添加了许多跟踪语句。

如果您编译此代码，您可以看到 `Delay` 的签名已更改。在更改之前，它返回了一个具体值（在这种情况下是一个选项），但现在它返回一个函数。

```F#
// signature BEFORE the change
member Delay : f:(unit -> 'a) -> 'a

// signature AFTER the change
member Delay : f:(unit -> 'b) -> (unit -> 'b)
```

顺便说一句，我们本可以用一种更简单的方式实现 `Delay`，而不需要任何跟踪，只需返回传入的相同函数，如下所示：

```F#
member this.Delay(f) =
    f
```

更简洁！但在这种情况下，我也想添加一些详细的跟踪信息。

现在让我们再试一次：

```F#
trace {
    printfn "Part 1: about to return 1"
    return 1
    printfn "Part 2: after return has happened"
    } |> printfn "Result for Part1 without Part2: %A"
```

噢。这次什么都没发生！出了什么问题？

如果我们查看输出，我们会看到：

```
Result for Part1 without Part2: <fun:Delay@84-5>
```

嗯，整个 `trace` 表达式的输出现在是一个函数，而不是一个选项。为什么？因为我们创建了所有这些延迟，但我们从未通过实际调用函数来“取消延迟”它们！

一种方法是将计算表达式的输出赋给一个函数值，比如 `f`，然后对其进行求值。

```F#
let f = trace {
    printfn "Part 1: about to return 1"
    return 1
    printfn "Part 2: after return has happened"
    }
f() |> printfn "Result for Part1 without Part2: %A"
```

这按预期工作，但有没有办法从计算表达式本身内部做到这一点？当然有！

## 介绍“Run”

`Run` 方法的存在正是出于这个原因。它被称为计算表达式求值过程中的最后一步，可用于消除延迟。

下面是一个实现：

```F#
type TraceBuilder() =
    // other members as before

    member this.Run(funcToRun) =
        printfn "%A - Run Start." funcToRun
        let runResult = funcToRun()
        printfn "%A - Run End. Result is %A" funcToRun runResult
        runResult // return the result of running the delayed function
```

让我们再试一次：

```F#
trace {
    printfn "Part 1: about to return 1"
    return 1
    printfn "Part 2: after return has happened"
    } |> printfn "Result for Part1 without Part2: %A"
```

结果正是我们想要的。第一部分进行了评估，但第二部分没有。整个计算表达式的结果是一个选项，而不是一个函数。

## 什么时候叫延迟？

一旦你理解了延迟，将 `Delay` 插入工作流的方式就很简单了。

- 底部（或最内部）表达式延迟。
- 如果将其与之前的表达式结合使用，则 `Combine` 的输出也会延迟。
- 以此类推，直到最后的延迟被输入 `Run`。

使用这些知识，让我们回顾一下上面例子中发生的事情：

- 表达式的第一部分是 print 语句加上 `return 1`。
- 表达式的第二部分是没有显式返回的 print 语句，这意味着调用了 `Zero()`
- 从 `Zero` 开始的 `None` 被输入到 `Delay` 中，从而产生一个“延迟选项”，即一个在调用时将评估为 `option` 的函数。
- 第 1 部分的选项和第 2 部分的延迟选项在 `Combine` 中合并，第二个选项被丢弃。
- 合并的结果变成了另一个“延迟选项”。
- 最后，延迟选项被馈送到 `Run`，Run 对其进行评估并返回一个正常选项。

这是一个直观地表示此过程的图表：

延误

如果我们查看上面示例的调试跟踪，我们可以详细了解发生了什么。这有点令人困惑，所以我对其进行了注释。此外，记住沿着此跟踪工作与从上图底部工作是一样的，因为最外层的代码是先运行的。

```
// delaying the overall expression (the output of Combine)
<fun:clo@160-66> - Delaying using <fun:delayed@141-3>

// running the outermost delayed expression (the output of Combine)
<fun:delayed@141-3> - Run Start.
<fun:clo@160-66> - Starting Delayed Fn.

// the first expression results in Some(1)
Part 1: about to return 1
Return an unwrapped 1 as an option

// the second expression is wrapped in a delay
<fun:clo@162-67> - Delaying using <fun:delayed@141-3>

// the first and second expressions are combined
Combine. Returning early with Some 1. Ignoring <fun:delayed@141-3>

// overall delayed expression (the output of Combine) is complete
<fun:clo@160-66> - Finished Delayed Fn. Result is Some 1
<fun:delayed@141-3> - Run End. Result is Some 1

// the result is now an Option not a function
Result for Part1 without Part2: Some 1
```

## “延迟”更改了“合并”的签名

当像这样将 `Delay` 引入管道时，它会对 `Combine` 的签名产生影响。

当我们最初编写 `Combine` 时，我们希望它能处理选项。但现在它正在处理 `Delay` 的输出，这是一个函数。

如果我们用 `int option` 类型注释对 `Combine` 期望的类型进行硬编码，我们可以看到这一点，如下所示：

```F#
member this.Combine (a: int option,b: int option) =
    printfn "Returning early with %A. Ignoring %A" a b
    a
```

如果这样做，我们会在“return”表达式中得到一个编译器错误：

```F#
trace {
    printfn "Part 1: about to return 1"
    return 1
    printfn "Part 2: after return has happened"
    } |> printfn "Result for Part1 without Part2: %A"
```

错误是：

```
error FS0001: This expression was expected to have type
    int option
but here has type
    unit -> 'a
```

换句话说，`Combine` 被传递了一个延迟函数（`unit->'a`），这与我们的显式签名不匹配。

那么，当我们确实想组合参数，但它们是作为函数而不是简单值传递的时，会发生什么呢？

答案很简单：只需调用传入的函数即可获取底层值。

让我们使用上一篇文章中的添加示例来演示一下。

```F#
type TraceBuilder() =
    // other members as before

    member this.Combine (m,f) =
        printfn "Combine. Starting second param %A" f
        let y = f()
        printfn "Combine. Finished second param %A. Result is %A" f y

        match m,y with
        | Some a, Some b ->
            printfn "combining %A and %A" a b
            Some (a + b)
        | Some a, None ->
            printfn "combining %A with None" a
            Some a
        | None, Some b ->
            printfn "combining None with %A" b
            Some b
        | None, None ->
            printfn "combining None with None"
            None
```

在这个新版本的 `Combine` 中，第二个参数现在是一个函数，而不是 `int option`。因此，要组合它们，我们必须在执行组合逻辑之前首先评估函数。

如果我们测试一下：

```F#
trace {
    return 1
    return 2
    } |> printfn "Result for return then return: %A"
```

我们得到以下（带注释的）跟踪：

```
// entire expression is delayed
<fun:clo@318-69> - Delaying using <fun:delayed@295-6>

// entire expression is run
<fun:delayed@295-6> - Run Start.

// delayed entire expression is run
<fun:clo@318-69> - Starting Delayed Fn.

// first return
Returning a unwrapped 1 as an option

// delaying second return
<fun:clo@319-70> - Delaying using <fun:delayed@295-6>

// combine starts
Combine. Starting second param <fun:delayed@295-6>

    // delayed second return is run inside Combine
    <fun:clo@319-70> - Starting Delayed Fn.
    Returning a unwrapped 2 as an option
    <fun:clo@319-70> - Finished Delayed Fn. Result is Some 2
    // delayed second return is complete

Combine. Finished second param <fun:delayed@295-6>. Result is Some 2
combining 1 and 2
// combine is complete

<fun:clo@318-69> - Finished Delayed Fn. Result is Some 3
// delayed entire expression is complete

<fun:delayed@295-6> - Run End. Result is Some 3
// Run is complete

// final result is printed
Result for return then return: Some 3
```

## 理解类型约束

到目前为止，在构建器的实现中，我们只使用了“包装类型”（例如 `int option`）和延迟版本（例如 `unit -> int option`）。

但事实上，如果我们愿意，我们可以在某些约束下使用其他类型。事实上，准确理解计算表达式中的类型约束可以阐明所有内容是如何组合在一起的。

例如，我们已经看到：

- `Return` 的输出被传递给 `Delay`，因此它们必须具有兼容的类型。
- `Delay` 的输出被传递给 `Combine` 的第二个参数。
- `Delay` 的输出也传递给 `Run`。

但是 `Return` 的输出不必是我们的“公共”包装类型。它可以是内部定义的类型。

延误

同样，延迟类型不一定是一个简单的函数，它可以是满足约束的任何类型。

因此，给定一组简单的返回表达式，如下所示：

```F#
    trace {
        return 1
        return 2
        return 3
        } |> printfn "Result for return x 3: %A"
```

然后，一个表示各种类型及其流程的图表如下：

延误

为了证明这是有效的，这里有一个 `Internal` 和 `Delay`类型不同的实现：

```F#
type Internal = Internal of int option
type Delayed = Delayed of (unit -> Internal)

type TraceBuilder() =
    member this.Bind(m, f) =
        match m with
        | None ->
            printfn "Binding with None. Exiting."
        | Some a ->
            printfn "Binding with Some(%A). Continuing" a
        Option.bind f m

    member this.Return(x) =
        printfn "Returning a unwrapped %A as an option" x
        Internal (Some x)

    member this.ReturnFrom(m) =
        printfn "Returning an option (%A) directly" m
        Internal m

    member this.Zero() =
        printfn "Zero"
        Internal None

    member this.Combine (Internal x, Delayed g) : Internal =
        printfn "Combine. Starting %A" g
        let (Internal y) = g()
        printfn "Combine. Finished %A. Result is %A" g y
        let o =
            match x,y with
            | Some a, Some b ->
                printfn "Combining %A and %A" a b
                Some (a + b)
            | Some a, None ->
                printfn "combining %A with None" a
                Some a
            | None, Some b ->
                printfn "combining None with %A" b
                Some b
            | None, None ->
                printfn "combining None with None"
                None
        // return the new value wrapped in a Internal
        Internal o

    member this.Delay(funcToDelay) =
        let delayed = fun () ->
            printfn "%A - Starting Delayed Fn." funcToDelay
            let delayedResult = funcToDelay()
            printfn "%A - Finished Delayed Fn. Result is %A" funcToDelay delayedResult
            delayedResult  // return the result

        printfn "%A - Delaying using %A" funcToDelay delayed
        Delayed delayed // return the new function wrapped in a Delay

    member this.Run(Delayed funcToRun) =
        printfn "%A - Run Start." funcToRun
        let (Internal runResult) = funcToRun()
        printfn "%A - Run End. Result is %A" funcToRun runResult
        runResult // return the result of running the delayed function

// make an instance of the workflow
let trace = new TraceBuilder()
```

构建器类方法中的方法签名如下：

```F#
type Internal = | Internal of int option
type Delayed = | Delayed of (unit -> Internal)

type TraceBuilder =
class
  new : unit -> TraceBuilder
  member Bind : m:'a option * f:('a -> 'b option) -> 'b option
  member Combine : Internal * Delayed -> Internal
  member Delay : funcToDelay:(unit -> Internal) -> Delayed
  member Return : x:int -> Internal
  member ReturnFrom : m:int option -> Internal
  member Run : Delayed -> int option
  member Zero : unit -> Internal
end
```

创建这个人工构建器当然有点过头了，但签名清楚地表明了各种方法是如何结合在一起的。

## 摘要

在这篇文章中，我们看到了：

- 如果你想在计算表达式中延迟执行，你需要实现 `Delay` 和 `Run`。
- 使用 `Delay` 会更改 `Combine` 的签名。
- `Delay` 和 `Combine` 可以使用不向计算表达式的客户端公开的内部类型。

下一个合乎逻辑的步骤是希望在计算表达式之外延迟执行，直到你准备好了，这将是下一篇文章的主题。但首先，我们将绕道讨论方法重载。

# 9 实现 CE：重载

*Part of the "Computation Expressions" series (*[link](https://fsharpforfunandprofit.com/posts/computation-expressions-builder-part4/#series-toc)*)*

愚蠢的方法技巧
2013年1月28日 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/computation-expressions-builder-part4/

在这篇文章中，我们将绕道而行，看看你可以在计算表达式构建器中使用方法的一些技巧。

最终，这种迂回将导致一条死胡同，但我希望这段旅程能为设计自己的计算表达式提供更多关于良好实践的见解。

> 请注意，计算表达式上下文中的“构建器”与用于构造和验证对象的 OO“构建器模式”不同。这里有一篇关于“构建器模式”的帖子。

## 一个见解：构建器方法可以重载

在某些时候，你可能会有一个见解：

- 构建器方法只是普通的类方法，与独立函数不同，方法可以支持不同参数类型的重载，这意味着我们可以创建任何方法的不同实现，只要参数类型不同。

所以，你可能会对此以及如何使用它感到兴奋。但事实证明，它并没有你想象的那么有用。让我们来看一些例子。

## 重载“return”

假设你有一个联合类型。您可以考虑为每个联合情况使用多个实现重载 `Return` 或 `Yield`。

例如，这里有一个非常简单的例子，其中 `Return` 有两个重载：

```F#
type SuccessOrError =
| Success of int
| Error of string

type SuccessOrErrorBuilder() =

    member this.Bind(m, f) =
        match m with
        | Success s -> f s
        | Error _ -> m

    /// overloaded to accept ints
    member this.Return(x:int) =
        printfn "Return a success %i" x
        Success x

    /// overloaded to accept strings
    member this.Return(x:string) =
        printfn "Return an error %s" x
        Error x

// make an instance of the workflow
let successOrError = new SuccessOrErrorBuilder()
```

它在这里使用：

```F#
successOrError {
    return 42
    } |> printfn "Result for success: %A"
// Result for success: Success 42

successOrError {
    return "error for step 1"
    } |> printfn "Result for error: %A"
//Result for error: Error "error for step 1"
```

你可能会想，这有什么错？

首先，如果我们回到关于包装器类型的讨论，我们指出包装器类型应该是泛型的。工作流应该尽可能地可重用——为什么要将实现绑定到任何特定的原始类型？

在这种情况下，这意味着联合类型应该改为这样：

```F#
type SuccessOrError<'a,'b> =
| Success of 'a
| Error of 'b
```

但是由于泛型的存在，`Return` 方法不能再重载了！

其次，无论如何，像这样在表达式中暴露类型的内部可能不是一个好主意。“成功”和“失败”情况的概念是有用的，但更好的方法是隐藏“失败”的情况并在 `Bind` 中自动处理，如下所示：

```F#
type SuccessOrError<'a,'b> =
| Success of 'a
| Error of 'b

type SuccessOrErrorBuilder() =

    member this.Bind(m, f) =
        match m with
        | Success s ->
            try
                f s
            with
            | e -> Error e.Message
        | Error _ -> m

    member this.Return(x) =
        Success x

// make an instance of the workflow
let successOrError = new SuccessOrErrorBuilder()
```

在这种方法中，`Return` 只用于成功，而失败的情况是隐藏的。

```F#
successOrError {
    return 42
    } |> printfn "Result for success: %A"

successOrError {
    let! x = Success 1
    return x/0
    } |> printfn "Result for error: %A"
```

我们将在下一篇文章中看到更多这种技术。

## 多个 Combine 实现

在实现 `Combine` 时，您可能会试图重载方法。

让我们重新审视 `trace` 工作流的 `Combine` 方法。如果你还记得，在之前的 `Combine` 实现中，我们只是将数字加在一起。

但是，如果我们改变我们的要求，并说：

- 如果我们在 `trace` 工作流中产生多个值，那么我们希望将它们组合成一个列表。

第一次尝试使用 combine 可能看起来像这样：

```F#
member this.Combine (a,b) =
    match a,b with
    | Some a', Some b' ->
        printfn "combining %A and %A" a' b'
        Some [a';b']
    | Some a', None ->
        printfn "combining %A with None" a'
        Some [a']
    | None, Some b' ->
        printfn "combining None with %A" b'
        Some [b']
    | None, None ->
        printfn "combining None with None"
        None
```

在 `Combine` 方法中，我们从传入的选项中解包值，并将它们组合成一个用 `Some` 包装的列表（例如 `Some[a'；b']`）。

对于两种 yield，它按预期工作：

```F#
trace {
    yield 1
    yield 2
    } |> printfn "Result for yield then yield: %A"

// Result for yield then yield: Some [1; 2]
```

对于一个 `None`，它也按预期工作：

```F#
trace {
    yield 1
    yield! None
    } |> printfn "Result for yield then None: %A"

// Result for yield then None: Some [1]
```

但是，如果有三个值要组合，会发生什么？这样地：

```F#
trace {
    yield 1
    yield 2
    yield 3
    } |> printfn "Result for yield x 3: %A"
```

如果我们尝试这样做，我们会得到一个编译器错误：

```
error FS0001: Type mismatch. Expecting a
    int option
but given a
    'a list option
The type 'int' does not match the type ''a list'
```

有什么问题？

答案是，在组合第 2 个和第 3 个值（`yield 2; yield 3`）后，我们得到一个包含整数列表或 `int list option`。当我们试图将第一个值（`Some 1`）与组合值（`Some[2;3]`）组合时，就会发生错误。也就是说，我们传递了一个 `int list option` 作为 `Combine` 的第二个参数，但第一个参数仍然是一个普通的 `int option`。编译器告诉您，它希望第二个参数与第一个参数的类型相同。

但是，这就是我们可能想要使用重载技巧的地方。我们可以创建两个不同的 `Combine` 实现，第二个参数有不同的类型，一个接受 `int option`，另一个接受 `int list option`。

以下是两种不同参数类型的方法：

```F#
/// combine with a list option
member this.Combine (a, listOption) =
    match a,listOption with
    | Some a', Some list ->
        printfn "combining %A and %A" a' list
        Some ([a'] @ list)
    | Some a', None ->
        printfn "combining %A with None" a'
        Some [a']
    | None, Some list ->
        printfn "combining None with %A" list
        Some list
    | None, None ->
        printfn "combining None with None"
        None

/// combine with a non-list option
member this.Combine (a,b) =
    match a,b with
    | Some a', Some b' ->
        printfn "combining %A and %A" a' b'
        Some [a';b']
    | Some a', None ->
        printfn "combining %A with None" a'
        Some [a']
    | None, Some b' ->
        printfn "combining None with %A" b'
        Some [b']
    | None, None ->
        printfn "combining None with None"
        None
```

现在，如果我们像以前一样尝试组合三个结果，我们就会得到我们期望的结果。

```F#
trace {
    yield 1
    yield 2
    yield 3
    } |> printfn "Result for yield x 3: %A"

// Result for yield x 3: Some [1; 2; 3]
```

不幸的是，这个技巧破坏了之前的一些代码！如果你现在尝试产生 `None`，你会得到一个编译器错误。

```F#
trace {
    yield 1
    yield! None
    } |> printfn "Result for yield then None: %A"
```

错误是：

```
error FS0041: A unique overload for method 'Combine' could not be determined based on type information prior to this program point. A type annotation may be needed.
```

但是等一下，在你太恼火之前，试着像编译器一样思考。如果你是编译器，你得到一个 `None`，你会调用哪个方法？

没有正确答案，因为 `None` 可以作为第二个参数传递给任一方法。编译器不知道这是 `int list option` 类型的 None（第一种方法）还是 `int option` 类型的 None（第二种方法）。

正如编译器提醒我们的那样，类型注释会有所帮助，所以让我们给它一个。我们将强制 None 为 `int option`。

```F#
trace {
    yield 1
    let x:int option = None
    yield! x
    } |> printfn "Result for yield then None: %A"
```

当然，这很丑陋，但在实践中可能不会经常发生。

更重要的是，这表明我们的设计很糟糕。有时计算表达式返回“`'a option`”，有时返回“`'a list option`”。我们的设计应该是一致的，这样计算表达式总是返回相同的类型，无论其中有多少个 `yield`。

也就是说，如果我们确实想允许多个 `yield`，那么我们应该首先使用“`'a list option`”作为包装器类型，而不仅仅是一个普通选项。在这种情况下，`Yield` 方法将创建列表选项，`Combine` 方法可以再次折叠为单个方法。

这是我们第三个版本的代码：

```F#
type TraceBuilder() =
    member this.Bind(m, f) =
        match m with
        | None ->
            printfn "Binding with None. Exiting."
        | Some a ->
            printfn "Binding with Some(%A). Continuing" a
        Option.bind f m

    member this.Zero() =
        printfn "Zero"
        None

    member this.Yield(x) =
        printfn "Yield an unwrapped %A as a list option" x
        Some [x]

    member this.YieldFrom(m) =
        printfn "Yield an option (%A) directly" m
        m

    member this.Combine (a, b) =
        match a,b with
        | Some a', Some b' ->
            printfn "combining %A and %A" a' b'
            Some (a' @ b')
        | Some a', None ->
            printfn "combining %A with None" a'
            Some a'
        | None, Some b' ->
            printfn "combining None with %A" b'
            Some b'
        | None, None ->
            printfn "combining None with None"
            None

    member this.Delay(f) =
        printfn "Delay"
        f()

// make an instance of the workflow
let trace = new TraceBuilder()
```

现在，示例按预期工作，没有任何特殊技巧：

```F#
trace {
    yield 1
    yield 2
    } |> printfn "Result for yield then yield: %A"

// Result for yield then yield: Some [1; 2]

trace {
    yield 1
    yield 2
    yield 3
    } |> printfn "Result for yield x 3: %A"

// Result for yield x 3: Some [1; 2; 3]

trace {
    yield 1
    yield! None
    } |> printfn "Result for yield then None: %A"

// Result for yield then None: Some [1]
```

不仅代码更简洁，而且与 `Return` 示例一样，我们也使代码更通用，从特定类型（`int option`）变为更通用的类型（`'a option`）。

## 重载“For”

可能需要重载的一个合法情况是 `For` 方法。一些可能的原因：

- 您可能希望支持不同类型的集合（例如 list 和 `IEnumerable`）
- 对于某些类型的集合，您可能有一个更有效的循环实现。
- 您可能有一个列表的“包装”版本（例如 LazyList），并且您希望支持对未包装和包装值的循环。

以下是我们的列表生成器的一个示例，它已扩展为支持序列和列表：

```F#
type ListBuilder() =
    member this.Bind(m, f) =
        m |> List.collect f

    member this.Yield(x) =
        printfn "Yield an unwrapped %A as a list" x
        [x]

    member this.For(m,f) =
        printfn "For %A" m
        this.Bind(m,f)

    member this.For(m:_ seq,f) =
        printfn "For %A using seq" m
        let m2 = List.ofSeq m
        this.Bind(m2,f)

// make an instance of the workflow
let listbuilder = new ListBuilder()
```

以下是它的使用情况：

```F#
listbuilder {
    let list = [1..10]
    for i in list do yield i
    } |> printfn "Result for list: %A"

listbuilder {
    let s = seq {1..10}
    for i in s do yield i
    } |> printfn "Result for seq : %A"
```

如果你注释掉第二个 `For` 方法，你会看到“`sequence`”示例确实无法编译。因此需要重载。

## 摘要

因此，我们已经看到，如果需要，方法可以重载，但要小心立即跳到这个解决方案，因为必须这样做可能是设计薄弱的标志。

在下一篇文章中，我们将回到精确控制表达式何时求值的问题，这次是使用构建器外部的延迟。

# 10 实现 CE：增加惰性

*Part of the "Computation Expressions" series (*[link](https://fsharpforfunandprofit.com/posts/computation-expressions-builder-part5/#series-toc)*)*

在外部延迟工作流程
2013年1月29日 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/computation-expressions-builder-part5/

在上一篇文章中，我们看到了如何避免在需要之前对工作流中的表达式进行不必要的求值。

但这种方法是为工作流中的表达式而设计的。如果我们想将整个工作流程本身延迟到需要时会发生什么。

> 请注意，计算表达式上下文中的“构建器”与用于构造和验证对象的 OO“构建器模式”不同。这里有一篇关于“构建器模式”的帖子。

## 问题

这是我们的“maybe”构建器类的代码。这段代码基于前一篇文章中的 `trace` 构建器，但去掉了所有的跟踪，所以它很干净。

```F#
type MaybeBuilder() =

    member this.Bind(m, f) =
        Option.bind f m

    member this.Return(x) =
        Some x

    member this.ReturnFrom(x) =
        x

    member this.Zero() =
        None

    member this.Combine (a,b) =
        match a with
        | Some _ -> a  // if a is good, skip b
        | None -> b()  // if a is bad, run b

    member this.Delay(f) =
        f

    member this.Run(f) =
        f()

// make an instance of the workflow
let maybe = new MaybeBuilder()
```

在继续之前，请确保您了解这是如何工作的。如果我们使用前一篇文章的术语来分析这一点，我们可以看到使用的类型是：

- 包装器类型：`'a option`
- 内部类型：`'a option`
- 延迟类型： `unit -> 'a option`

现在让我们检查这段代码，确保一切按预期工作。

```F#
maybe {
    printfn "Part 1: about to return 1"
    return 1
    printfn "Part 2: after return has happened"
    } |> printfn "Result for Part1 but not Part2: %A"

// result - second part is NOT evaluated

maybe {
    printfn "Part 1: about to return None"
    return! None
    printfn "Part 2: after None, keep going"
    } |> printfn "Result for Part1 and then Part2: %A"

// result - second part IS evaluated
```

但是，如果我们将代码重构为子工作流，会发生什么，如下所示：

```F#
let childWorkflow =
    maybe {printfn "Child workflow"}

maybe {
    printfn "Part 1: about to return 1"
    return 1
    return! childWorkflow
    } |> printfn "Result for Part1 but not childWorkflow: %A"
```

输出显示，即使最终不需要子工作流，也对其进行了评估。在这种情况下，这可能不是问题，但在许多情况下，我们可能不希望发生这种情况。

那么，如何避免呢？

## 将内部类型包裹在延迟中

最明显的方法是将构建器的整个结果包装在一个延迟函数中，然后“运行”结果，我们只需对延迟函数进行求值。

所以，这是我们的新包装类型：

```F#
type Maybe<'a> = Maybe of (unit -> 'a option)
```

我们用一个计算结果为选项的函数替换了一个简单的 `option`，然后将该函数封装在一个单案例联合中以获得良好的度量。

现在我们也需要更改 `Run` 方法。以前，它评估了传递给它的延迟函数，但现在它应该不进行评估，并将其包装在我们的新包装器类型中：

```F#
// before
member this.Run(f) =
    f()

// after
member this.Run(f) =
    Maybe f
```

*我忘了修另一种方法——你知道是哪一种吗？我们很快就会碰到它！*

还有一件事——我们现在需要一种“运行”结果的方法。

```F#
let run (Maybe f) = f()
```

让我们在前面的例子中尝试一下我们的新类型：

```F#
let m1 = maybe {
    printfn "Part 1: about to return 1"
    return 1
    printfn "Part 2: after return has happened"
    }
```

运行这个，我们得到这样的结果：

```F#
val m1 : Maybe<int> = Maybe <fun:m1@123-7>
```

看起来不错；没有其他印刷品。

现在运行它：

```F#
run m1 |> printfn "Result for Part1 but not Part2: %A"
```

我们得到输出：

```
Part 1: about to return 1
Result for Part1 but not Part2: Some 1
```

太好了！第二部分没有运行。

但我们在下一个例子中遇到了一个问题：

```F#
let m2 = maybe {
    printfn "Part 1: about to return None"
    return! None
    printfn "Part 2: after None, keep going"
    }
```

哎呀！我们忘了修理 `ReturnFrom`！众所周知，该方法采用包装类型，我们现在重新定义了包装类型。

解决方法如下：

```F#
member this.ReturnFrom(Maybe f) =
    f()
```

我们将接受来自外部的“`Maybe`”，然后立即运行它以获得 option。

但现在我们有了另一个问题——我们不能再在 `return! None` 返回显式的 `None`，我们必须返回 `Maybe` 类型。我们将如何创建其中一个？

好吧，我们可以创建一个辅助函数来为我们构造一个。但有一个更简单的答案：你可以通过使用 `Maybe` 表达式来创建一个新的 `Maybe` 类型！

```F#
let m2 = maybe {
    return! maybe {printfn "Part 1: about to return None"}
    printfn "Part 2: after None, keep going"
    }
```

这就是为什么 `Zero` 方法很有用。使用 `Zero` 和构建器实例，您可以创建该类型的新实例，即使它们什么都不做。

但现在我们还有一个错误——可怕的“值限制”：

```
Value restriction. The value 'm2' has been inferred to have generic type
```

发生这种情况的原因是两个表达式都返回 `None`。但是编译器不知道 `None` 是什么类型。代码正在使用 `Option<obj>` 类型的 `None`（可能是因为隐式装箱），但编译器知道该类型可以更通用。

有两种修复方法。一种是使类型显式：

```F#
let m2_int: Maybe<int> = maybe {
    return! maybe {printfn "Part 1: about to return None"}
    printfn "Part 2: after None, keep going;"
    }
```

或者我们可以只返回一些非 None 值：

```F#
let m2 = maybe {
    return! maybe {printfn "Part 1: about to return None"}
    printfn "Part 2: after None, keep going;"
    return 1
    }
```

这两种解决方案都将解决问题。

现在，如果我们运行这个例子，我们会看到结果与预期一致。第二部分这次运行。

```F#
run m2 |> printfn "Result for Part1 and then Part2: %A"
```

跟踪输出：

```
Part 1: about to return None
Part 2: after None, keep going;
Result for Part1 and then Part2: Some 1
```

最后，我们将再次尝试子工作流示例：

```F#
let childWorkflow =
    maybe {printfn "Child workflow"}

let m3 = maybe {
    printfn "Part 1: about to return 1"
    return 1
    return! childWorkflow
    }

run m3 |> printfn "Result for Part1 but not childWorkflow: %A"
```

现在，子工作流没有按照我们的要求进行评估。

如果我们确实需要评估子工作流，这也是可行的：

```F#
let m4 = maybe {
    return! maybe {printfn "Part 1: about to return None"}
    return! childWorkflow
    }

run m4 |> printfn "Result for Part1 and then childWorkflow: %A"
```

### 复习构建器类

让我们再次查看新构建器类中的所有代码：

```F#
type Maybe<'a> = Maybe of (unit -> 'a option)

type MaybeBuilder() =

    member this.Bind(m, f) =
        Option.bind f m

    member this.Return(x) =
        Some x

    member this.ReturnFrom(Maybe f) =
        f()

    member this.Zero() =
        None

    member this.Combine (a,b) =
        match a with
        | Some _' -> a    // if a is good, skip b
        | None -> b()     // if a is bad, run b

    member this.Delay(f) =
        f

    member this.Run(f) =
        Maybe f

// make an instance of the workflow
let maybe = new MaybeBuilder()

let run (Maybe f) = f()
```

如果我们使用前一篇文章的术语来分析这个新的构建器，我们可以看到使用的类型是：

- 包装类型：`Maybe<'a>`
- 内部类型：`'a option`
- 延迟类型：`unit -> 'a option`

请注意，在这种情况下，使用标准的 `'a option` 作为内部类型很方便，因为我们根本不需要修改 `Bind` 或 `Return`。

另一种设计可能也会使用 `Maybe<'a>` 作为内部类型，这会使事情更加一致，但会使代码更难阅读。

## 真正的惰性

让我们看看最后一个例子的变体：

```F#
let child_twice: Maybe<unit> = maybe {
    let workflow = maybe {printfn "Child workflow"}

    return! maybe {printfn "Part 1: about to return None"}
    return! workflow
    return! workflow
    }

run child_twice |> printfn "Result for childWorkflow twice: %A"
```

应该怎么办？子工作流应该运行多少次？

上述延迟实现确实确保了子工作流仅在需要时进行评估，但不会阻止它运行两次。

在某些情况下，您可能需要保证工作流最多只运行一次，然后缓存（“记忆”）。使用 F# 内置的 `Lazy` 类型很容易做到这一点。

我们需要做出的改变是：

- 更改 `Maybe` 包装一个 `Lazy` 而不是延迟
- 更改 `ReturnFrom` 并 `run` 以强制评估懒惰值
- 更改 `Run` 以从 `lazy` 内部运行延迟

以下是更改后的新类：

```F#
type Maybe<'a> = Maybe of Lazy<'a option>

type MaybeBuilder() =

    member this.Bind(m, f) =
        Option.bind f m

    member this.Return(x) =
        Some x

    member this.ReturnFrom(Maybe f) =
        f.Force()

    member this.Zero() =
        None

    member this.Combine (a,b) =
        match a with
        | Some _' -> a    // if a is good, skip b
        | None -> b()     // if a is bad, run b

    member this.Delay(f) =
        f

    member this.Run(f) =
        Maybe (lazy f())

// make an instance of the workflow
let maybe = new MaybeBuilder()

let run (Maybe f) = f.Force()
```

如果我们从上面运行“`child_twice`”代码，我们得到：

```
Part 1: about to return None
Child workflow
Result for childWorkflow twice: <null>
```

由此可以清楚地看出，子工作流只运行了一次。

## 总结：即时 vs. 延迟 vs. 惰性

在这个页面上，我们看到了 `maybe` 工作流的三种不同实现。一个总是立即评估，一个使用延迟函数，一个在记忆中使用懒惰。

那么…你应该使用哪种方法？

没有单一的“正确”答案。你的选择取决于许多因素：

- 表达式中的代码执行起来是否便宜，而且没有重要的副作用？如果是这样，请坚持使用第一个即时版本。它简单易懂，这正是大多数 `maybe` 工作流实现所使用的。
- 表达式中的代码执行起来是否很昂贵，结果是否会随着每次调用而变化（例如非确定性），或者是否有重要的副作用？如果是这样，请使用第二个延迟版本。这正是大多数其他工作流所做的，尤其是与I/O相关的工作流（如 `async`）。
- F# 并不试图成为一种纯粹的函数式语言，因此几乎所有的 F# 代码都属于这两类之一。但是，如果你需要以保证无副作用的方式编码，或者你只是想确保昂贵的代码最多只被评估一次，那么就使用第三个惰性选项。

无论你选择什么，都要在文档中明确说明。例如，延迟与惰性实现在客户端看来完全相同，但它们具有非常不同的语义，并且客户端代码必须针对每种情况编写不同的代码。

现在我们已经完成了延迟和惰性，我们可以回到构建器方法并完成它们。

# 11 实现 CE：其他标准方法

实现 While、Using 和异常处理
2013年1月30日 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/computation-expressions-builder-part6/

我们现在正进入最后冲刺阶段。只需要介绍几个构建器方法，然后你就可以处理任何事情了！

这些方法是：

- `While` 对于重复。
- `TryWith` 和 `TryFinally` 用于处理异常。
- `Use` 用于管理 disposables

一如既往地记住，并非所有方法都需要实现。如果 `While` 与你无关，不要费心。

在我们开始之前，有一点很重要：**这里讨论的所有方法都依赖于使用的延迟**。如果你没有使用延迟函数，那么这些方法都不会给出预期的结果。

> 请注意，计算表达式上下文中的“构建器”与用于构造和验证对象的OO“构建器模式”不同。这里有一篇关于“构建器模式”的帖子。

## 实现“While”

我们都知道“while”在普通代码中是什么意思，但在计算表达式的上下文中又是什么意思呢？为了理解，我们必须再次重新审视延续的概念。

在之前的文章中，我们看到一系列表达式被转换为这样的连续链：

```F#
Bind(1,fun x ->
   Bind(2,fun y ->
     Bind(x + y,fun z ->
        Return(z)  // or Yield
```

这是理解“while”循环的关键——它可以以同样的方式扩展。

首先，一些术语。while 循环有两部分：

- 在“while”循环的顶部有一个测试，每次都会对其进行评估，以确定是否应该运行身体。当评估结果为 false 时，while 循环“退出”。在计算表达式中，测试部分被称为“**保护（guard）**”。测试函数没有参数，返回一个 bool，所以它的签名当然是 `unit->bool`。
- 还有“while”循环的主体，每次都会进行评估，直到“while”测试失败。在计算表达式中，这是一个延迟函数，其计算结果为包裹值。由于 while 循环的主体总是相同的，因此每次都会计算相同的函数。body 函数没有参数，也不返回任何值，因此它的签名只是 `unit -> wrapped unit`。

有了这个，我们可以使用 continuation 为 while 循环创建伪代码：

```F#
// evaluate test function
let bool = guard()
if not bool
then
    // exit loop
    return what??
else
    // evaluate the body function
    body()

    // back to the top of the while loop

    // evaluate test function again
    let bool' = guard()
    if not bool'
    then
        // exit loop
        return what??
    else
        // evaluate the body function again
        body()

        // back to the top of the while loop

        // evaluate test function a third time
        let bool'' = guard()
        if not bool''
        then
            // exit loop
            return what??
        else
            // evaluate the body function a third time
            body()

            // etc
```

一个显而易见的问题是：当 while 循环测试失败时，应该返回什么？好吧，我们以前见过这个， `if..then..`，答案当然是使用 `Zero` 值。

接下来，`body()` 结果将被丢弃。是的，它是一个单位函数，所以没有值可以返回，但即便如此，在我们的表达式中，我们希望能够钩住它，这样我们就可以在幕后添加行为。当然，这需要使用 `Bind` 函数。

因此，这是使用 `Zero` 和 `Bind` 的伪代码的修订版本：

```F#
// evaluate test function
let bool = guard()
if not bool
then
    // exit loop
    return Zero
else
    // evaluate the body function
    Bind( body(), fun () ->

        // evaluate test function again
        let bool' = guard()
        if not bool'
        then
            // exit loop
            return Zero
        else
            // evaluate the body function again
            Bind( body(), fun () ->

                // evaluate test function a third time
                let bool'' = guard()
                if not bool''
                then
                    // exit loop
                    return Zero
                else
                    // evaluate the body function again
                    Bind( body(), fun () ->

                    // etc
```

在这种情况下，传递给 `Bind` 的 continuation 函数有一个 unit 参数，因为 `body` 函数没有值。

最后，伪代码可以通过将其折叠成递归函数来简化，如下所示：

```F#
member this.While(guard, body) =
    // evaluate test function
    if not (guard())
    then
        // exit loop
        this.Zero()
    else
        // evaluate the body function
        this.Bind( body(), fun () ->
            // call recursively
            this.While(guard, body))
```

事实上，这是 `While` 在几乎所有构建器类中的标准“锅炉板”实现。

必须正确选择 `Zero` 的值，这是一个微妙但重要的一点。在之前的帖子中，我们看到可以根据工作流程将 `Zero` 的值设置为 `None` 或 `Some ()`。然而，为了使 `While` 正常工作，必须将 `Zero` 设置为 `Some ()` 而不是 `None`，因为将 `None` 传递给 `Bind` 会导致整个过程提前中止。

还要注意，虽然这是一个递归函数，但我们不需要 `rec` 关键字。它只需要用于递归的独立函数，而不是方法。

### “While”在使用中

让我们看看它在 `trace` 构建器中的使用情况。这是完整的构建器类，带有 `While` 方法：

```F#
type TraceBuilder() =
    member this.Bind(m, f) =
        match m with
        | None ->
            printfn "Binding with None. Exiting."
        | Some a ->
            printfn "Binding with Some(%A). Continuing" a
        Option.bind f m

    member this.Return(x) =
        Some x

    member this.ReturnFrom(x) =
        x

    member this.Zero() =
        printfn "Zero"
        this.Return ()

    member this.Delay(f) =
        printfn "Delay"
        f

    member this.Run(f) =
        f()

    member this.While(guard, body) =
        printfn "While: test"
        if not (guard())
        then
            printfn "While: zero"
            this.Zero()
        else
            printfn "While: body"
            this.Bind( body(), fun () ->
                this.While(guard, body))

// make an instance of the workflow
let trace = new TraceBuilder()
```

如果你查看 `While` 的签名，你会看到 `body` 参数是 `unit -> unit option`，即一个延迟函数。如上所述，如果你没有正确实现 `Delay`，你会得到意想不到的行为和神秘的编译器错误。

```F#
type TraceBuilder =
    // other members
    member
      While : guard:(unit -> bool) * body:(unit -> unit option) -> unit option
```

这是一个使用可变值的简单循环，该值每轮递增一次。

```F#
let mutable i = 1
let test() = i < 5
let inc() = i <- i + 1

let m = trace {
    while test() do
        printfn "i is %i" i
        inc()
    }
```

## 使用“try..with”处理异常

异常处理以类似的方式实现。

如果我们 `try..with` 表达式为例，它有两个部分：

- 有“try”的主体，评估一次。在计算表达式中，这将是一个延迟函数，其计算结果为包装值。body 函数没有参数，因此它的签名只是 `unit -> wrapped type`。
- “with”部分处理异常。它有一个异常作为参数，并返回与“try”部分相同的类型，因此它的签名是 `exception -> wrapped type`。

有了这个，我们可以为异常处理程序创建伪代码：

```F#
try
    let wrapped = delayedBody()
    wrapped  // return a wrapped value
with
| e -> handlerPart e
```

这与标准实现完全对应：

```F#
member this.TryWith(body, handler) =
    try
        printfn "TryWith Body"
        this.ReturnFrom(body())
    with
        e ->
            printfn "TryWith Exception handling"
            handler e
```

如您所见，通过 `ReturnFrom` 传递返回值是很常见的，这样它就可以得到与其他包装值相同的处理。

以下是一个示例片段，用于测试处理方式：

```F#
trace {
    try
        failwith "bang"
    with
    | e -> printfn "Exception! %s" e.Message
    } |> printfn "Result %A"
```

## 实现“try..finally”

`try..finally` 是非常相似的 `try..with`。

- 有“尝试”的主体，评估一次。body 函数没有参数，因此它的签名是 `unit -> wrapped type`。
- “最终”部分总是被称为。它没有参数，返回一个单位，因此它的签名是 `unit->unit`。

就像 `try..with`，标准的实现是显而易见的。

```F#
member this.TryFinally(body, compensation) =
    try
        printfn "TryFinally Body"
        this.ReturnFrom(body())
    finally
        printfn "TryFinally compensation"
        compensation()
```

另一个小片段：

```F#
trace {
    try
        failwith "bang"
    finally
        printfn "ok"
    } |> printfn "Result %A"
```

## 实现“using”

最后一种实现方法是 `Using`。这是实现 `use!` 关键字的构建器方法。

这是 MSDN 文档中关于 `use!` 的说明：

```F#
{| use! value = expr in cexpr |}
```

翻译为：

```F#
builder.Bind(expr, (fun value -> builder.Using(value, (fun value -> {| cexpr |} ))))
```

换句话说，`use!` 关键字同时触发 `Bind` 和 `Using`。首先执行 `Bind` 以解包包装好的值，然后将解包的一次性值传递给 `Using` 以确保处置，并将 continuation 函数作为第二个参数。

实现这一点很简单。与其他方法类似，我们有一个“using”表达式的主体或延续部分，只计算一次。此 body 函数有一个“一次性（disposable）”参数，因此其签名为 `#IDisposable -> wrapped type`。

当然，我们希望确保不管发生什么，可支配值都会被处理，因此我们需要将对 body 函数的调用封装在 `TryFinally` 中。

以下是一个标准实现：

```F#
member this.Using(disposable:#System.IDisposable, body) =
    let body' = fun () -> body disposable
    this.TryFinally(body', fun () ->
        match disposable with
            | null -> ()
            | disp -> disp.Dispose())
```

笔记：

- `TryFinally` 的参数是一个 `unit -> wrapped` 的，第一个参数是 unit，因此我们创建了一个传递的延迟版本的主体。
- Disposable 是一个类，所以它可能是 `null`，我们必须特别处理这种情况。否则，我们只需在“最终”的延续中处理它。

这是一个实际 `Using` 的演示。请注意，`makeResource` 做了一个包装好的一次性物品（disposable）。如果没有包装，我们就不需要特殊 `use!` 了并且可以仅使用正常 `use`。

```F#
let makeResource name =
    Some {
    new System.IDisposable with
    member this.Dispose() = printfn "Disposing %s" name
    }

trace {
    use! x = makeResource "hello"
    printfn "Disposable in use"
    return 1
    } |> printfn "Result: %A"
```

## 重新审视“For”

最后，我们可以重新审视 `For` 是如何实现的。在前面的示例中，`For` 采用了一个简单的列表参数。但是，使用 `Using` 和 `While`，我们可以将其更改为接受任何 `IEnumerable<_>` 或序列。

以下是 `For` 目前的标准实现：

```F#
member this.For(sequence:seq<_>, body) =
       this.Using(sequence.GetEnumerator(),fun enum ->
            this.While(enum.MoveNext,
                this.Delay(fun () -> body enum.Current)))
```

如您所见，为了处理泛型 `IEnumerable<_>`，它与之前的实现有很大不同。

- 我们使用 `IEnumerator<_>` 进行显式迭代。
- `IEnumerator<_>` 实现了 `IDisposable`，因此我们将枚举数包装在 `Using` 中。
- 我们使用 `While .. MoveNext` 进行迭代。
- 接下来，我们传递 `enum.Current` 进入身体功能
- 最后，我们使用 `Delay` 延迟对 body 函数的调用

## 无跟踪的完整代码

到目前为止，由于添加了跟踪和打印表达式，所有的构建器方法都变得比所需的更复杂。追踪有助于理解正在发生的事情，但它可能会掩盖方法的简单性。

因此，作为最后一步，让我们看看“跟踪”构建器类的完整代码，但这次没有任何无关的代码。尽管代码很神秘，但现在您应该已经熟悉了每个方法的目的和实现。

```F#
type TraceBuilder() =

    member this.Bind(m, f) =
        Option.bind f m

    member this.Return(x) = Some x

    member this.ReturnFrom(x) = x

    member this.Yield(x) = Some x

    member this.YieldFrom(x) = x

    member this.Zero() = this.Return ()

    member this.Delay(f) = f

    member this.Run(f) = f()

    member this.While(guard, body) =
        if not (guard())
        then this.Zero()
        else this.Bind( body(), fun () ->
            this.While(guard, body))

    member this.TryWith(body, handler) =
        try this.ReturnFrom(body())
        with e -> handler e

    member this.TryFinally(body, compensation) =
        try this.ReturnFrom(body())
        finally compensation()

    member this.Using(disposable:#System.IDisposable, body) =
        let body' = fun () -> body disposable
        this.TryFinally(body', fun () ->
            match disposable with
                | null -> ()
                | disp -> disp.Dispose())

    member this.For(sequence:seq<_>, body) =
        this.Using(sequence.GetEnumerator(),fun enum ->
            this.While(enum.MoveNext,
                this.Delay(fun () -> body enum.Current)))
```

经过这么多讨论，代码现在看起来很小。然而，这个构建器实现了每个标准方法，使用了延迟函数。在短短几行代码中就有很多功能！