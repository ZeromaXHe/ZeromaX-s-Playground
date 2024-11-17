# [返回主 Markdown](./FSharpForFunAndProfit翻译.md)



# 1 依赖注入的六种方法

*Part of the "Dependency Injection" series (*[link](https://fsharpforfunandprofit.com/posts/dependencies/#series-toc)*)*

2020年12月20日

https://fsharpforfunandprofit.com/posts/dependencies/

> 这篇文章是 2020 年 F# 降临日历的一部分。查看那里的所有其他精彩帖子！特别感谢 Sergey Tihon 组织这次活动。

在本系列文章中，我们将介绍六种不同的“依赖注入”方法。

这篇文章的灵感来自 Mark Seemann 的类似系列文章，以略微不同的方式涵盖了相同的想法。Bartosz Sypytkowski 和 Carsten König 在这个话题上还有其他好文章。它们都值得一读！

我们将探讨的六种方法是：

- **依赖保留（Dependency retention）**，其中我们不担心管理依赖关系；我们只是内联和硬编码一切！
- **依赖拒绝（Dependency rejection）**，这是一个很好的术语（由 Mark Seemann 在上文中创造），其中我们避免在核心业务逻辑代码中有任何依赖。我们通过将所有 I/O 和其他不纯代码保持在域的“边缘”来实现这一点。
- **依赖参数化（Dependency parameterization）**，其中我们将所有依赖作为参数传递。这通常与部分应用程序结合使用。
- **依赖注入（Dependency injection）和 Reader monad**，在其中，我们在其余代码构建完成后传递依赖关系。在 OO 风格的代码中，这通常是通过构造函数注入完成的，在 FP 风格的代码里，这对应于 Reader monad。
- **依赖关系解释（Dependency Interpretation）**，其中我们用稍后解释的数据结构替换对依赖关系的调用。这种方法在 OO（解释器模式）和 FP（例如自由单子）中都有使用

对于每种方法，我们将查看一个示例实现，然后讨论每种方法的优缺点。作为奖励，在本系列的最后一篇文章中，我们将采用一个不同的示例，并再次以六种不同的方式实现它。

*注意：我很久以前也写过类似的文章。那篇文章现在被这些文章取代了。*

## 什么是“依赖”？

在我们开始之前，让我们定义一下这篇文章的“依赖性”是什么意思。我会说，当函数 A 调用函数 B 时，函数 A 对函数 B 有依赖关系。因此，这是一种调用者/被调用者依赖关系，而不是数据依赖关系、库依赖关系或我们在软件开发中处理的任何其他类型的依赖关系。

但这种情况经常发生，那么什么样的依赖关系是有问题的呢？

首先，我们通常希望创建可预测和确定性（纯）的代码。任何非确定性的呼叫都会把事情搞砸。这些非确定性调用包括各种 I/O、随机数生成器、获取当前日期和时间等。因此，我们希望管理和控制不纯的依赖关系。

其次，即使是纯代码，我们也可能经常希望在运行时通过传递不同的实现来改变行为，而不是硬编码实现。在面向对象设计中，我们可能会使用策略模式，在 FP 设计中，可能会传入一个“策略”函数作为参数。

所有其他依赖关系都不需要特殊的管理。如果一个类/模块/函数只有一个实现，并且是纯的，那么只需在代码中直接调用它。如果不需要，则无需模拟或添加额外的抽象！

总结一下，我们有两种依赖关系：

- 不纯的依赖关系，引入了非确定性，使测试更加困难。
- “策略”依赖关系，支持使用多种实现。

## 面向工作流的设计

在接下来的所有代码中，我将使用“面向工作流”的设计，其中“工作流”表示业务事务、故事、用例等。有关此方法的更多详细信息，请参阅我的“重新发明事务脚本”演讲（或 Jimmy Bogard 的“垂直切片架构”演讲）。

## 要求

让我们采用一些非常简单的需求，并使用这六种不同的方法来实现它们。

要求如下：

- 从输入中读取两个字符串
- 比较它们
- 打印第一个字符串是大于、小于还是等于第二个字符串

就是这样。很简单，但让我们看看我们能把它弄得多复杂！

## 方法 #1：依赖保留

让我们从最简单的需求实现开始：

```F#
let compareTwoStrings() =
  printfn "Enter the first value"
  let str1 = Console.ReadLine()
  printfn "Enter the second value"
  let str2 = Console.ReadLine()

  if str1 > str2 then
    printfn "The first value is bigger"
  else if str1 < str2 then
    printfn "The first value is smaller"
  else
    printfn "The values are equal"
```

如您所见，这直接实现了需求，没有额外的抽象或复杂性。

这种方法的优点正是：实现是显而易见的，易于理解。事实上，对于非常小的项目，添加抽象可能会使代码的可维护性降低，而不是增加。

缺点是无法测试此功能。如果你看函数签名，它是 `unit -> unit`。换句话说，它不接受有用的输入，也不发出有用的输出。它只能通过人机交互进行测试，反复运行并处理输入。

因此，我建议采用这种方法：

- 不值得测试或创建抽象的简单脚本。
- 一次性草图或原型，重点是快速制作东西，这样你就可以更多地了解需求。
- “业务逻辑”最小的程序，基本上是将大量的输入和输出粘合在一起。ETL 管道就是一个例子。许多数据科学涉及将脚本组合在一起并手动检查结果。在这些情况下，重点是数据和数据转换，编写测试或添加额外的抽象可能没有意义。

## 方法 #2：依赖拒绝

使代码可预测和可测试的最简单方法之一是消除代码中的任何不纯依赖关系，只留下纯代码。我们称之为“依赖拒绝”。

例如，在我们上面的第一个实现中，不纯的 I/O 调用（`printfn` 和 `ReadLine`）与纯决策混合在一起（`if str1>str2`）。

![img](https://fsharpforfunandprofit.com/posts/dependencies/Dependencies1a.jpg)

如果我们只想在代码中有纯粹的决策，那么我们必须改变什么？

首先，从控制台读取的所有内容都必须作为参数传入
其次，决策必须作为纯数据结构返回，而不是进行任何I/O操作
经过这些更改，代码现在看起来像这样：

```F#
module PureCore =

  type ComparisonResult =
    | Bigger
    | Smaller
    | Equal

  let compareTwoStrings str1 str2 =
    if str1 > str2 then
      Bigger
    else if str1 < str2 then
      Smaller
    else
      Equal
```

在这个新的实现中，现在消除了与 I/O 有关的所有内容。

这段代码是完全确定的，因此易于测试，就像下面的小测试套件（使用 Expecto 测试库）一样。

```F#
testCase "smaller" <| fun () ->
  let expected = PureCore.Smaller
  let actual = PureCore.compareTwoStrings "a" "b"
  Expect.equal actual expected "a < b"

testCase "equal" <| fun () ->
  let expected = PureCore.Equal
  let actual = PureCore.compareTwoStrings "a" "a"
  Expect.equal actual expected "a = a"

testCase "bigger" <| fun () ->
  let expected = PureCore.Bigger
  let actual = PureCore.compareTwoStrings "b" "a"
  Expect.equal actual expected "b > a"
```

但是我们实际上如何使用这些纯代码呢？好吧，我们需要调用者提供输入并对输出采取行动。通常，I/O 应该在调用堆栈中尽可能高的位置完成。“顶层”可以用许多不同的名称来称呼，例如“api 层”、“shell 层”、”组合根“，或者简称为“程序”。

以下是调用者代码的样子：

```F#
module Program =
  open PureCore

  let program() =
    // ----------- impure section -----------
    printfn "Enter the first value"
    let str1 = Console.ReadLine()
    printfn "Enter the second value"
    let str2 = Console.ReadLine()

    // ----------- pure section -----------
    let result = PureCore.compareTwoStrings str1 str2

    // ----------- impure section -----------
    match result with
    | Bigger ->
      printfn "The first value is bigger"
    | Smaller ->
      printfn "The first value is smaller"
    | Equal ->
      printfn "The values are equal"
```

通过使用“依赖拒绝”方法，我们可以看到我们现在有一个不纯/纯/不纯的三明治：

![img](https://fsharpforfunandprofit.com/posts/dependencies/Dependencies1b.jpg)

一般来说，我们希望我们的功能管道看起来像这样：

- 一些 I/O 或其他非确定性代码，例如从控制台/文件/数据库等读取
- 决策的纯粹商业逻辑
- 更多的 I/O，例如将结果保存到文件/数据库等

![img](https://fsharpforfunandprofit.com/posts/dependencies/Dependencies2a.jpg)


这样做的好处是，I/O 段可以特定于此特定的工作流。例如，我们不需要为每个可能的用例创建一个包含数百种方法的 `IRepository`，只需要实现这个工作流的要求，这反过来又可以使整个代码库更清晰。

### 多层三明治

如果您在决策过程中需要一些额外的 I/O，该怎么办？在这种情况下，您可以创建一个多层三明治，如下所示：

![img](https://fsharpforfunandprofit.com/posts/dependencies/Dependencies2c.jpg)

出于上述所有原因，重要的是将 I/O 段与决策段分开。

### 测试

这种方法的另一个好处是测试边界变得非常清晰。您在中心对纯代码进行单元测试，并在整个管道中进行集成测试。

![img](https://fsharpforfunandprofit.com/posts/dependencies/Dependencies2b.jpg)

## 摘要

在这篇文章中，我们研究了六种方法中的两种：“依赖保留”，即内联依赖，以及“依赖拒绝”，即消除所有 I/O，只在核心域中留下纯代码。

鉴于其明显的好处，应尽可能使用“依赖拒绝”方法。唯一的缺点是需要额外的间接性：

- 您可能需要定义一个特殊的数据结构来表示纯代码返回的决策。
- 您需要一个更高级别的层来运行不纯的代码并将结果传递给纯代码，然后解释结果并将其转换回I/O操作。

这篇文章的源代码可以在以下注册表中找到：

- [DependencyRejection.fsx](https://gist.github.com/swlaschin/cbc9a5992695a88e32e3f39fbf1ecf79)
- [DependencyRetention.fsx](https://gist.github.com/swlaschin/d35b59795a85a62723124df1a79d2388)

在下一篇文章中，我们将介绍“依赖参数化”。也就是说，将依赖关系作为标准函数参数传递。

# 2 使用参数进行依赖注入

*Part of the "Dependency Injection" series (*[link](https://fsharpforfunandprofit.com/posts/dependencies-2/#series-toc)*)*

依赖注入的六种方法，第2部分
2020年12月21日

https://fsharpforfunandprofit.com/posts/dependencies-2/

在本系列中，我们将介绍六种不同的依赖注入方法。

- 在第一篇文章中，我们研究了“依赖保留”（内联依赖）和“依赖拒绝”（将I/O保持在实现的边缘）。
- 在这篇文章中，我们将探讨“依赖参数化”作为管理依赖关系的一种方式。

## 依赖关系参数化

鉴于您已经努力将纯代码与不纯代码分开，您可能仍然需要管理其他依赖关系。例如：

- 我们如何调整之前的代码以支持不同的比较算法？
- 我们如何调整前面的代码以支持模拟I/O？（假设我们想模拟I/O，而不仅仅是进行集成测试）。

为了实现这些类型的“参数化”要求，一种简单而明显的方法就是将要参数化的行为作为函数传递到主代码中。

例如，如果我们想支持不同的比较算法，我们可以添加比较选项作为参数，如下所示：

```F#
let compareTwoStrings (comparison:StringComparison) str1 str2 =
  // The StringComparison enum lets you pick culture and case-sensitivity options
  let result = String.Compare(str1,str2,comparison)
  if result > 0 then
    Bigger
  else if result < 0 then
    Smaller
  else
    Equal
```

此函数现在有三个参数，而不是原来的两个。

![img](https://fsharpforfunandprofit.com/posts/dependencies-2/Dependencies3a.jpg)

但是通过添加一个额外的参数，我们打破了 `compareWoStrings` 的原始合约，该合约只有两个输入：

```F#
type CompareTwoStrings = string -> string -> ComparisonResult
```

没问题！我们可以部分应用比较来获得符合合同的新函数。

```F#
// these both have the same type as `CompareTwoStrings`
let compareCaseSensitive = compareTwoStrings StringComparison.CurrentCulture
let compareCaseInsensitive = compareTwoStrings StringComparison.CurrentCultureIgnoreCase
```

![img](https://fsharpforfunandprofit.com/posts/dependencies-2/Dependencies3b.jpg)

请注意，“策略”参数被故意定位为第一个参数，以使部分应用更容易。

### I/O的依赖性参数化

如果我们想支持 I/O 功能或其他基础设施服务的多种实现，我们也可以使用相同的参数化方法。我们只是将它们作为参数传递。

```F#
// "infrastructure services" passed in as parameters
let compareTwoStrings (readLn:unit->string) (writeLn:string->unit) =
  writeLn "Enter the first value"
  let str1 = readLn()
  writeLn "Enter the second value"
  let str2 = readLn()
  // etc
```

顶层代码可以定义  `readLn` 和 `writeLn` 的实现，然后调用上面的函数：

```F#
let program() =
  let readLn() = Console.ReadLine()
  let writeLn str = printfn "%s" str
  // call the parameterized function
  compareTwoStrings readLn writeLn
```

当然，我们可以用使用文件、套接字或其他任何东西的控制台实现来替换这些控制台实现。

### 将多个依赖项组合到一个参数中

如果函数依赖于许多基础设施服务，那么使用接口或函数记录将它们组合成一个对象通常更容易，而不是将每个函数作为单独的参数传递。

```F#
type IConsole =
  abstract ReadLn : unit -> string
  abstract WriteLn : string -> unit
```

然后，主函数将此接口作为单个参数接受：

```F#
// All "infrastructure services" passed in as a single interface
let compareTwoStrings (console:IConsole)  =
  console.WriteLn "Enter the first value"
  let str1 = console.ReadLn()
  console.WriteLn "Enter the second value"
  let str2 = console.ReadLn()
  // etc
```

最后，最顶层的函数（“组合根”）构建所需的接口，并使用它调用 main 函数：

```F#
let program() =
  let console = {
    new IConsole with
      member this.ReadLn() = Console.ReadLine()
      member this.WriteLn str = printfn "%s" str
    }
  // call the parameterized function
  compareTwoStrings console
```

## 依赖参数化的优缺点

对于“策略”样式依赖关系，参数化是标准方法。这太普遍了，甚至不值得注意。例如，它几乎出现在所有的集合函数中，如 `List.map`、`List.sortBy` 等。

对于参数化基础设施服务和其他非确定性依赖关系，其好处不太明显。让我们来看看你可能想或不想这样做的一些原因。

**可模仿性**。是的，这种方法确实允许您模拟基础设施，但另一方面，如果您将I/O保持在边缘，则根本不需要使用模拟，因为您将只对管道的纯段进行单元测试。

**为了避免供应商锁定**。有些人会认为，通过参数化基础设施（例如数据库访问），这将使以后的切换实现变得更加容易。但是，如果你保持 I/O 分离，我认为在边缘硬编码一个特定的数据库实现是完全可以的。它与（纯）决策代码解耦，如果你需要切换到不同的供应商，这个过程将非常简单。此外，通过不太通用，您可以利用服务的供应商特定功能。（如果你不想利用供应商特定的功能，那么你为什么还要使用那个供应商呢？）

**封装**。如果在 I/O 密集的管道中有一长串组件（具有最小的业务逻辑），并且每个组件都需要不同的基础设施服务，那么将服务作为部分应用的参数直接传递给每个组件，然后将组件连接在一起，通常会简单得多，如下所示：

![img](https://fsharpforfunandprofit.com/posts/dependencies-2/Dependencies4a.jpg)

这使管道中的组件保持解耦。即使你违反了一些纯度规则，F#也不是Haskell，如果管道的I/O量很大，我个人对使用这种方法没有问题。如果业务逻辑很重，那么我建议您坚持依赖拒绝方法。

## 边栏：纯函数可以有不纯的参数吗？

如果非确定性依赖关系用作函数的参数，那么该函数是否不纯？在我看来，不是的。你也可以向 `List.map` 传递一个不纯的参数—— `List.map` 不会突然变得不纯。

在 Haskell 中，任何“不纯”的函数都通过其类型中有 `IO` 来表示。`IO` 类型会“污染”调用堆栈，因此主函数的输出也会有 `IO`，并且会被明确地标记为不纯。在 F# 中，编译器不会强制执行此操作。有些人喜欢使用 `Async` 作为Haskell `IO` 的等价物，作为非确定性的指标。我个人对此持不可知论态度——这在某些情况下可能会有所帮助，但我不会将其作为一般原则来执行。

## 如何管理日志？

有时，您需要在纯域代码的深处进行 I/O 或其他非确定性操作。在这种情况下，依赖拒绝将不起作用，您将不得不以某种方式传递依赖。

发生这种情况的常见情况是日志记录。假设您需要在核心域中记录各种操作，并且您有一个如下所示的记录器界面：

```F#
type ILogger =
  abstract Debug : string -> unit
  abstract Info : string -> unit
  abstract Error : string -> unit
```

如何从域内访问记录器的实现？

最简单的选择就是访问全局对象（单例记录器或创建记录器的“工厂”）。一般来说，全局变量是一个坏主意，但对于日志记录，我认为它是可以接受的，以换取干净的代码。

如果你想显式的，那么你需要将一个 logger 作为参数传递给每个需要它的函数，如下所示：

```F#
let compareTwoStrings (logger:ILogger) str1 str2 =
  logger.Debug "compareTwoStrings: Starting"

  let result =
    if str1 > str2 then
      Bigger
    else if str1 < str2 then
      Smaller
    else
      Equal

  logger.Info (sprintf "compareTwoStrings: result=%A" result)
  logger.Debug "compareTwoStrings: Finished"
  result
```

这样做的优点是，此功能完全独立，易于单独测试。缺点是，如果你有很多深度嵌套的函数，这种方法可能会变得非常乏味。在接下来的两篇文章中，我们将探讨使用读取器 monad 和解释器模式处理此问题的其他方法。

## 摘要

在这篇文章中，我们研究了使用常规函数参数传递依赖关系。

这与上一篇文章中的“依赖拒绝”相比如何？我想说，你应该始终从“依赖拒绝”方法开始，尽可能地将 I/O 依赖移到边缘，远离核心。

但在某些情况下，传递 I/O 依赖关系是完全可以接受的——无论如何，在我看来！I/O 繁重的管道，或者需要日志记录的地方，是直接传递依赖关系可能有意义的情况。

如果你真的想严格要求纯度，请继续关注！在下一篇文章中，我们将介绍阅读器 monad。

这篇文章的源代码可以在这里找到。

# 3 使用 Reader monad 进行依赖注入

*Part of the "Dependency Injection" series (*[link](https://fsharpforfunandprofit.com/posts/dependencies-3/#series-toc)*)*

依赖注入的六种方法，第3部分
2020年12月22日

https://fsharpforfunandprofit.com/posts/dependencies-3/

在本系列中，我们将介绍六种不同的依赖注入方法。

- 在第一篇文章中，我们研究了“依赖保留”（内联依赖）和“依赖拒绝”（将 I/O 保持在实现的边缘）。
- 在第二篇文章中，我们研究了将依赖关系作为标准函数参数注入。
- 在这篇文章中，我们将使用经典的 OO 风格的依赖注入和 FP 等效物来研究依赖处理：Reader monad

## 重新审视日志问题

在上一篇文章中，我简要讨论了日志问题。如何从域的深处访问依赖关系？

这是一个问题的例子。比较两个字符串的代码（纯），但也需要一个记录器。显而易见的解决方案是将 ILogger 作为参数传递。

```F#
let compareTwoStrings (logger:ILogger) str1 str2 =
  logger.Debug "compareTwoStrings: Starting"

  let result =
    if str1 > str2 then
      Bigger
    else if str1 < str2 then
      Smaller
    else
      Equal

  logger.Info (sprintf "compareTwoStrings: result=%A" result)
  logger.Debug "compareTwoStrings: Finished"
  result
```

## “注入”依赖关系

正如我们上面看到的，将依赖关系作为参数传递的标准方法是将它们放在第一位，这样它们就可以部分应用。如果我们根据上面代码的函数签名制作一个图表，它看起来像这样：

![img](https://fsharpforfunandprofit.com/posts/dependencies-3/Dependencies5a.jpg)

但是，如果我们在最后传递了任何依赖关系呢？所以函数签名看起来像这样：

![img](https://fsharpforfunandprofit.com/posts/dependencies-3/Dependencies5b.jpg)

这样做有什么好处？好处是，您可以重新解释该签名，使其看起来像这样：

![img](https://fsharpforfunandprofit.com/posts/dependencies-3/Dependencies5c.jpg)

因此，我们的函数不是返回原始的 `ComparisonResult`，而是返回一个函数，一个签名为 `ILogger -> ComparisonResult` 的函数。

我们正在做的是推迟对依赖性的需求。该函数现在表示：我将在假设依赖关系可用的情况下进行工作，然后，您将实际给我该依赖关系。

## OO 风格的依赖注入

仔细想想，这正是传统 OO 风格依赖注入的方式。

- 首先，你实现一个类及其方法，假设稍后会有一个依赖项可用。
- 稍后，在构造类时传递实际的依赖关系。

这是一个 F# 中类定义的示例

```F#
// logger passed in via the constructor
type StringComparisons(logger:ILogger) =

  member __.CompareTwoStrings str1 str2  =
    logger.Debug "compareTwoStrings: Starting"

    let result = ...

    logger.Info (sprintf "compareTwoStrings: result=%A" result)
    logger.Debug "compareTwoStrings: Finished"
    result
```

下面是稍后使用 logger 实例构造的类：

```F#
// create the logger
let logger : ILogger = defaultLogger
// construct the class
let stringComparisons = StringComparisons logger
// call the method
stringComparisons.CompareTwoStrings "a" "b"
```

有趣的是，在 F# 中，对类构造函数 `StringComparisons logger` 的调用看起来就像一个函数调用，将 logger 作为“最后一个”参数传递给类。

## FP 风格依赖注入：返回函数

FP 版本的“稍后传递依赖关系”是什么？正如我们上面看到的，它只是意味着返回一个函数，该函数有一个稍后提供的 `ILogger` 参数。

这是 `compareWoStrings` 函数，但现在 `ILogger` 依赖项作为最后一个参数：

```F#
let compareTwoStrings str1 str2 (logger:ILogger) =
  logger.Debug "compareTwoStrings: Starting"

  let result = ...

  logger.Info (sprintf "compareTwoStrings: result=%A" result)
  logger.Debug "compareTwoStrings: Finished"
  result
```

这是一个完全相同的函数，经过重新解释，返回值是一个 `ILogger -> ComparisonResult` 函数。

```F#
let compareTwoStrings str1 str2 =
  fun (logger:ILogger) ->
    logger.Debug "compareTwoStrings: Starting"

    let result = ...

    logger.Info (sprintf "compareTwoStrings: result=%A" result)
    logger.Debug "compareTwoStrings: Finished"
    result
```

## 阅读器 monad

事实证明，这是 FP 中非常常见的模式，以至于它有了一个名字：“Reader monad”或“Environment monad”。

使用可怕的m字听起来很复杂，但我们所做的只是给一个函数命名，这个函数有某种上下文或环境作为参数。在我们的例子中，环境是 `ILogger` 依赖项。

![img](https://fsharpforfunandprofit.com/posts/dependencies-3/Dependencies5d.jpg)

为了便于使用，我们将把这个函数包装成泛型类型，如下所示：

```F#
type Reader<'env,'a> = Reader of action:('env -> 'a)
```

您可以将其理解为：Reader 包含一个函数，该函数将某个环境 `'env` 作为输入，并返回一个值 `'a`

如果我们调整代码，将返回的函数包装在 `Reader` 类型中，那么我们的新实现看起来像这样：

```F#
let compareTwoStrings str1 str2 :Reader<ILogger,ComparisonResult> =
  fun (logger:ILogger) ->
    logger.Debug "compareTwoStrings: Starting"

    let result = ...

    logger.Info (sprintf "compareTwoStrings: result=%A" result)
    logger.Debug "compareTwoStrings: Finished"
    result
  |> Reader // <------------------ NEW!!!
```

请注意，返回类型现在已从 `ILogger -> ComparisonResult` 更改为 `Reader<ILogger, ComparisonResult>`

好吧，那么我们为什么要做这么多额外的工作呢？何必麻烦？

原因是 `Reader` 类型可以像 `Option`、`Result`、`List` 或 `Async` 类型一样进行组合、转换和链接。如果你熟悉我的面向铁路的编程文章，你可以使用与链接“结果返回”函数相同的模式来链接“Reader 返回”函数。你可以为它编写一个 `map` 函数，为它编写 `bind`/`flatMap` 函数，等等。这是一个单子！

这是一个包含一些有用 `Reader` 函数的模块：

```F#
module Reader =
  /// Run a Reader with a given environment
  let run env (Reader action)  =
    action env  // simply call the inner function

  /// Create a Reader which returns the environment itself
  let ask = Reader id

  /// Map a function over a Reader
  let map f reader =
    Reader (fun env -> f (run env reader))

  /// flatMap a function over a Reader
  let bind f reader =
    let newAction env =
      let x = run env reader
      run env (f x)
    Reader newAction
```

### `reader` 计算表达式

如果我们有一个 `bind` 函数，我们也可以很容易地创建一个计算表达式。以下是我们如何为 `Reader` 定义一个基本的计算表达式。

```F#
type ReaderBuilder() =
  member __.Return(x) = Reader (fun _ -> x)
  member __.Bind(x,f) = Reader.bind f x
  member __.Zero() = Reader (fun _ -> ())

// the builder instance
let reader = ReaderBuilder()
```

我们不必使用 `reader` 计算表达式，但如果我们这样做，它通常会让我们的生活更轻松。

## 构建阅读器返回函数

让我们看看这一切是如何在实践中发挥作用的。让我们从第一篇文章中获取原始代码，并将其分为三个部分：读取字符串、比较字符串和打印输出。

下面是 `compareTwoStrings` 重写为使用 `reader` 计算表达式：

```F#
let compareTwoStrings str1 str2  =
  reader {
    let! (logger:ILogger) = Reader.ask
    logger.Debug "compareTwoStrings: Starting"

    let result = ...

    logger.Info (sprintf "compareTwoStrings: result=%A" result)
    logger.Debug "compareTwoStrings: Finished"
    return result
    }
```

它看起来与之前的实现非常相似，但有几点需要注意：

- 所有内容都包含在 `reader {...}` 计算表达式中。
- `ILogger` 参数已消失。相反，我们可以直接使用 `Reader.ask` 访问环境值（在本例中为 `ILogger`）。
- 就像在所有计算表达式中一样，我们可以使用 `let!` 并 `do!` “解包” Reader 值的内容。在这种情况下，我们使用 `let!` 打开 `ask` Reader 以获取环境（`ILogger`）。
- 我在 `let! (logger:ILogger) = Reader.ask` 中添加了一个显式的类型。这允许编译器推断读取器的类型，而无需显式注释整个函数。

我们可以对从控制台读取字符串的函数执行相同的操作：

```F#
let readFromConsole() =
  reader {
    let! (console:IConsole) = Reader.ask

    console.WriteLn "Enter the first value"
    let str1 = console.ReadLn()
    console.WriteLn "Enter the second value"
    let str2 = console.ReadLn()

    return str1,str2
    }
```

这一次，`ask` 被注释为 `IConsole` 类型。

但是，如果我们需要两种不同的服务呢？我们可以试着写这样的东西：

```F#
let readFromConsole() =
  reader {
    let! (console:IConsole) = Reader.ask
    let! (logger:ILogger) = Reader.ask     // error
    ...
```

但这会导致编译器错误。这是因为第一行意味着 Reader 类型是 `Reader<IConsole, _>`，第二行意味着阅读器类型是 `Reader<ILogger, _>`。这些类型不兼容。

我们可以使用几种不同的方法来解决这个问题。

### 方法 1：使用推断继承

在 F# 中，我们可以利用继承的技巧。我们可以要求控制台从 `IConsole` 继承，记录器从 `ILogger` 继承。编译器现在将推断出 Reader 类型是从 `IConsole` 和 `ILogger` 继承的。问题解决了！

在 F# 中指示继承约束的最简单方法是在类型注释前使用 `#` 符号，如下所示：

```F#
let readFromConsole() =
  reader {
    let! (console:#IConsole) = Reader.ask
    let! (logger:#ILogger) = Reader.ask     // OK now!
    ...
```

现在推断出 Reader 类型没有错误。实际推断的类型是 `Reader<'a,...> when 'a :> ILogger and 'a :> IConsole`。

让我们以相同的方式调整 `compareTwoStrings`：

```F#
let compareTwoStrings str1 str2  =
  reader {
    let! (logger:#ILogger) = Reader.ask
    logger.Debug "Starting"
```

我们还可以实现一个函数来写入结果：

```F#
let writeToConsole (result:ComparisonResult) =
  reader {
    let! (console:#IConsole) = Reader.ask

    match result with
    | Bigger ->
      console.WriteLn "The first value is bigger"
    | Smaller ->
      console.WriteLn "The first value is smaller"
    | Equal ->
      console.WriteLn "The values are equal"

    }
```

### 编写具有推断继承的 Reader 返回函数

最后，我们可以组合这三个函数，每个函数都是一个 Reader 返回函数。

首先，我们需要定义一个同时实现 `ILogger` 和 `IConsole` 的东西

```F#
type IServices =
    inherit ILogger
    inherit IConsole
```

现在我们创建一个包含所有三个函数的计算表达式。

```F#
let program :Reader<IServices,_> = reader {
  let! str1,str2 = readFromConsole()
  let! result = compareTwoStrings str1 str2
  do! writeToConsole result
  }
```

重要的是要明白，此时 `program` 尚未运行。就像 `Async` 值或自制解析器一样，它也有可能运行，但我们需要传入 `IServices` 才能实际运行它。

假设我们有控制台和记录器的默认实现，我们可以这样实现 `IServices`：

```F#
let services =
  { new IServices
    interface IConsole with
      member __.ReadLn() = defaultConsole.ReadLn()
      member __.WriteLn str = defaultConsole.WriteLn str
    interface ILogger with
      member __.Debug str = defaultLogger.Debug str
      member __.Info str = defaultLogger.Info str
      member __.Error str = defaultLogger.Error str
    }
```

最后，我们可以运行整个过程：

```F#
Reader.run services program
```

### 方法2：绘制环境图

继承方法很好，但很快就会变得难以处理，因为要实现的方法很多。这可以通过具有只有一个成员的中间接口来减少。Bartosz Sypytkowski 在这篇文章中对此进行了很好的介绍，所以我不会在这里介绍。

相反，让我们看看另一种根本不使用继承的方法。

我们像以前一样定义函数，这次每个函数都要求它需要的确切类型，而不是子类。如果一个函数需要多个服务，它会向环境请求一个元组。

```F#
let readFromConsole() =
  reader {
    // ask for an IConsole,ILogger pair
    let! (console:IConsole),(logger:ILogger) = Reader.ask  // a tuple
    ...
    return str1,str2
    }

let compareTwoStrings str1 str2  =
  reader {
    // ask for an ILogger
    let! (logger:ILogger) = Reader.ask
    logger.Debug "compareTwoStrings: Starting"

    let result = ...

    return result
    }

let writeToConsole (result:ComparisonResult) =
  reader {
    // ask for an IConsole
    let! (console:IConsole) = Reader.ask

    match result with
    ...
    }
```

现在，如果我们试图在计算表达式中组合它们，我们会得到很多错误：

```F#
let program_bad = reader {
  let! str1, str2 = readFromConsole()
  let! result = compareTwoStrings str1 str2 // error
  do! writeToConsole result // error
  }
```

原因是所有的读取器都是不同的类型：`readFromConsole` 需要 `IConsole * ILogger` 环境，而 `compareTwoStrings` 需要 `ILogger` 的环境，以此类推。

为了解决这个问题，我们需要做的是创建一个可以转换为任何所需环境的“超类型”。这是：

```F#
type Services = {
  Logger : ILogger
  Console : IConsole
  }
```

接下来，我们需要一种从 `Services` 类型映射到各个子环境的方法。我会用 `withEnv` 来称呼这个：

```F#
/// Transform a Reader's environment from subtype to supertype.
let withEnv (f:'superEnv->'subEnv) reader =
  Reader (fun superEnv -> (run (f superEnv) reader))
  // The new Reader environment is now "superEnv"
```

*旁白：`withEnv` 的类型签名看起来很像“map”的签名。我们正在将 `Reader<subEnvironment>` 转换为 `Reader<superEnvironment>`，并传递一个映射函数 `f` 来实现这一点。与法线贴图函数不同，`f` 中的类型向另一个方向移动（`superEnv -> subEnv`，而不是 `subEnv->superEnv`）。这个签名的行话是“反映射（contramap）”。*

现在我们可以使用每个函数返回的 Reader，并使用 `Reader.withEnv` 转换其环境，如下所示：

```F#
let program = reader {
  // helper functions to transform the environment
  let getConsole services = services.Console
  let getLogger services = services.Logger
  let getConsoleAndLogger services = services.Console,services.Logger // a tuple

  let! str1, str2 =
    readFromConsole()
    |> Reader.withEnv getConsoleAndLogger
  let! result =
    compareTwoStrings str1 str2
    |> Reader.withEnv getLogger
  do! writeToConsole result
    |> Reader.withEnv getConsole
  }
```

通过使用 `withEnv`，我们使计算表达式中的代码变得更加复杂，以换取使服务的实现更加灵活。

同样，该 `program` 尚未运行。我们需要传入一个服务来实际运行它，如下所示：

```F#
let services = {
  Console = defaultConsole
  Logger = defaultLogger
  }

Reader.run services program
```

## 进一步阅读

有关使用 Reader monad 的另一个示例，请参阅本系列的最后一篇文章。

Reader 方法很少用于 F#，但通常用于 Haskell 和 FP 风格的 Scala。Carsten König 和 Matthew Podwysokci 发表了一些关于在 F# 中使用它的好文章。

## 后期传递依赖关系的优缺点

OO 风格的依赖注入和 FP 风格的读取器都依赖于在代码开发后的最后一步传递依赖关系。

哪一种更好，什么时候应该使用？

首先，如果你正在与一个进行依赖注入的 C# 框架（如 ASP.NET）交互，如果你设计的 F# 代码与这种方法兼容，你的生活会容易得多。

否则，使用 Reader monad 有很多很好的特性：它消除了前一篇文章中讨论的“依赖参数化”方法中使用的丑陋的依赖参数，它比 OO 风格的依赖注入更具可组合性，并且你有标准的工具，如 `map` 和 `bind` 来转换和调整它们。

但这并不都是好消息。Reader monad 与所有以 monad 为中心的方法都有同样的主要问题：很难将它们与其他类型混合搭配。

例如，如果你想返回一个结果和一个阅读器，你不能简单地快速集成这两种类型。如果你也想在设计中添加 `Async`，它可能会变得更加复杂。是的，有一个解决方案，但很容易陷入“俄罗斯方块类型”，花太多时间试图让类型匹配。事实上，对于代码的“边缘”部分，即大量的 I/O，由于这些不匹配的痛苦，我根本不会使用 Reader。保存阅读器方法，将记录器等依赖项注入到纯代码中。

总之，我认为阅读器是你工具箱里的一个很好的工具，特别是如果你热衷于保持代码的纯粹性和 Haskell 风格。但是 F# 不是 Haskell，所以我认为默认使用 Reader 是多余的。根据具体情况，我可能会首先讨论本系列中讨论的其他方法之一。

我们还没结束呢！在下一篇文章中，我们将介绍另一种管理依赖关系的方法：解释器模式。

*这篇文章的源代码可以在这里找到。*

# 4 依赖解释

*Part of the "Dependency Injection" series (*[link](https://fsharpforfunandprofit.com/posts/dependencies-4/#series-toc)*)*

依赖注入的六种方法，第4部分
2020年12月23日

https://fsharpforfunandprofit.com/posts/dependencies-4/

在本系列中，我们将介绍六种不同的依赖注入方法。

- 在第一篇文章中，我们研究了“依赖保留”（内联依赖）和“依赖拒绝”（将 I/O 保持在实现的边缘）。
- 在第二篇文章中，我们研究了使用标准函数参数注入依赖关系。
- 在第三篇文章中，我们研究了使用经典 OO 风格的依赖注入和 FP 等效物（Reader monad）进行依赖处理。
- 在这篇文章中，我们将通过使用解释器模式来完全避免依赖关系。
- 在下一篇文章中，我们将重新审视所讨论的所有技术，并将其应用于一个新的示例。

本文中的示例基于前几篇文章中的示例，因此请先阅读。

## 依赖解释

在“依赖拒绝”方法中，我们展示了如何返回表示决策的数据结构（通常是“选择”类型）。然后，管道的最后一段将根据提供的选择执行各种 I/O 操作。这保持了核心代码的纯净，并将所有 I/O 推到了边缘。

我们可以进一步采用这种方法，并将其扩展到涵盖所有 I/O。我们将返回一个数据结构，作为以后执行各种 I/O 操作的指令，而不是直接执行 I/O。

对于我们的第一次尝试，让我们返回一个 I/O 指令列表，如下所示：

```F#
type Instruction =
  | ReadLn
  | WriteLn of string

let readFromConsole() =
  let cmd1 = WriteLn "Enter the first value"
  let cmd2 = ReadLn
  let cmd3 = WriteLn "Enter the second value"
  let cmd4 = ReadLn

  // return all the instructions I want the I/O part to do
  [cmd1; cmd2; cmd3; cmd4]
```

然后分别解释这些说明，如下所示：

```F#
let interpretInstruction instruction =
  match instruction with
  | ReadLn -> Console.ReadLine()
  | WriteLn str -> printfn "%s" str
```

然而，这种方法存在很多问题。首先，`interpretInstruction` 甚至不能编译！这是因为 `match instruction` 表达式中的每个分支返回不同类型的数据。

更严重的是，没有办法在代码中间使用解释器的输出。例如，假设我想使用第一个 `ReadLn` 的结果来更改第二个 `WriteLn` 的输出。在上述设计中，这根本不可能。

我们想做的是一种如下图所示的方法，其中解释器每一步的输出都可以用于我们代码的下一行。

![img](https://fsharpforfunandprofit.com/posts/dependencies-4/Dependencies6a.jpg)

我们真的可以做到！诀窍在于，当我们创建一条指令时，我们还提供了一个“下一个”函数，在解释发生后调用。解释器执行一条指令后，它会调用“next”函数并返回执行结果，这反过来又会调用我们控制下的代码。

让我们调用一整套操作来解释“程序”。程序由普通的纯代码和要解释的指令混合组成。然后，对于每条指令，我们需要向解释器传递一对： `interpreterInput * (interpreterOutput -> Program)`。

![img](https://fsharpforfunandprofit.com/posts/dependencies-4/Dependencies6b.jpg)

举个具体的例子，让我们看看 `ReadLn`。一个普通的 `ReadLn` 函数具有签名 `unit -> string`。在我们的新方法中，我们将给解释器一个 `unit`，并希望它给我们一个 `string`。但这并不完全正确。解释器不会将字符串反馈给我们，而是将该字符串传递给我们提供给解释器的“下一个”函数。因此，下一个函数将具有签名 `string -> Program`，其中 `Program` 是代码的其余部分。

同样，普通 `WriteLn` 的签名是 `string -> unit`，但在解释方法中，我们需要传递给解释器的对将是 `string * (unit -> Program)`。换句话说，我们对解释器的输入是一个字符串，然后，在解释之后，解释器调用下一个带 `unit` 的函数来获得一个新的 `Program`。

让我们看看实现所有这些的代码。

首先，我们定义指令集。根据上述约定，我将把整个指令集称为 `Program`。

```F#
type Program<'a> =
  | ReadLn  of unit    * next:(string  -> Program<'a>)
  | WriteLn of string  * next:(unit    -> Program<'a>)
  | Stop    of 'a
```

这个特殊的程序有三条指令：

`ReadLn` 对解释器没有输入。为了处理此指令，解释器将从某个地方读取一行文本，然后使用读取的行调用相关的“next”函数 `string->Program<'a>`。然后，该函数将返回一个新的 `Program`，准备再次解释。

`WriteLn` 有一个字符串输入到解释器。为了处理此指令，解释器将把该字符串写入某处，然后调用相关的“下一个”函数 `unit->Program<'a>`。然后，该函数将返回一个新的 `Program`，准备再次解释。

如果我们只有这两条指令，我们就会有一个无休止的循环，所以我们需要一条额外的指令来告诉解释器停止。我称之为 `Stop`，但你也可以称之为 `Done` 或 `Return` 或类似的东西。（出于某种原因，Haskeller 可能会称之为 `Pure` 或 `Unit`）。`Stop` 有一个关联的值，但没有可调用的“next”函数。当解释器看到此指令时，它停止递归，只返回相关值。与 `Stop` 关联的值可以是任何值，因此迫使我们将整个 `Program` 类型设置为泛型（`Program<'a>`）。

以下是使用这些指令的一些代码的样子：

```F#
let readFromConsole =
  WriteLn ("Enter the first value" , fun () ->
  ReadLn  ( ()                     , fun str1 ->
  WriteLn ("Enter the second value", fun () ->
  ReadLn  ( ()                     , fun str2 ->
  Stop  (str1,str2)        // no "next" function
  ))))
```

你可以看到，在每条指令之后，都有一个包含更多指令的函数，以此类推，直到我们到达 `Stop`，在那里我们将这两个字符串作为元组返回。

重要的是要理解 `readFromConsole` 是一个数据结构，而不是函数！它是一个包含 `ReadLn` 的 `WriteLn`，里面的 `ReadLn` 包含另一个 `WriteLn`，它里面又包含了一个包含 `Stop` 的 `ReadLn`。数据结构包含函数，但实际上还没有执行任何操作。

现在我们已经构建了一个数据结构，我们需要一个解释器来“执行”数据结构。如果你已经理解了到目前为止的解释，那么实现应该很容易理解。请注意，对于 `ReadLn` 和 `WriteLn`，它是递归的，但在 `Stop` 时，它停止递归并返回提供的值。

```F#
let rec interpret program =
  match program with
  | ReadLn ((), next) ->
      // 1. interpret the meaning of "ReadLn" to do actual I/O
      let str = Console.ReadLine()
      // 2. call "next" with the output of the interpretation.
      // This gives us another Program
      let nextProgram = next str
      // 3. interpret the new Program (recursively)
      interpret nextProgram
  | WriteLn (str,next) ->
      printfn "%s" str
      let nextProgram = next()
      interpret nextProgram
  | Stop value ->
      // return the overall result of the Program
      value
```

我们现在可以测试它是否有效：

```F#
interpret readFromConsole
```

确实如此！您可以使用本文底部链接的要点中的代码亲自尝试。

## 使用计算表达式使生活更轻松

上面的 `readFromConsole` 实现很难编写，看起来也很难看。我们能让这类代码的编写和读取更容易吗？

是的，我们可以。每行末尾的连续序列（`fun ... -> ...`）正是计算表达式旨在解决的问题！

现在让我们为这些指令构建一个计算表达式。首先，我们需要实现一个 `bind` 函数。它可以使用以下规则进行机械创建：

- 对于 `Stop` 情况，将 `f` 参数应用于返回值。
- 对于所有其他情况，将 `next` 函数替换为 `next >> bind f`。

```F#
module Program =
  let rec bind f program =
    match program with
    | ReadLn ((),next) -> ReadLn ((),next >> bind f)
    | WriteLn (str,next) -> WriteLn (str, next >> bind f)
    | Stop x -> f x
```

请注意，`bind` 必须使用 `let rec` 定义，以便可以递归使用。

一旦我们有了 `bind`，我们就可以定义计算表达式及其相关的“构建器”类。

- `Bind` 方法使用上面定义的 `bind`。
- `Return` 和 `Zero` 方法使用 `Stop` 返回值

```F#
type ProgramBuilder() =
  member __.Return(x) = Stop x
  member __.Bind(x,f) = Program.bind f x
  member __.Zero() = Stop ()

// the builder instance
let program = ProgramBuilder()
```

在计算表达式中定义一些辅助函数也很方便。这些辅助函数不“做”任何事情，它们只是创建一个数据结构。

```F#
// helpers to use within the computation expression
let writeLn str = WriteLn (str,Stop)
let readLn() = ReadLn ((),Stop)
```

现在我们可以使用 `program` 计算表达式和两个辅助函数以更简洁的方式重新实现 `readFromConsole` 函数：

```F#
let readFromConsole = program {
    do! writeLn "Enter the first value"
    let! str1 = readLn()
    do! writeLn "Enter the second value"
    let! str2 = readLn()
    return (str1,str2)
    }
```

令人惊讶的是，这段代码看起来几乎与第一个“依赖保留”示例中的代码完全相同。没有传递依赖关系，一切都很干净。当然，幕后还有更多的复杂性，与“依赖保留”的例子不同，我们还没有完成。我们还需要编写口译员！

## 为我们的示例设计指令和解释器

现在，让我们扩展这种解释器方法，以构建我们在本系列中使用的示例。

首先，我们需要定义程序中的指令。与其将所有指令放在一个 `Program` 类型下，不如让我们看看如何从较小的部分构建它。当我们有一个更复杂的系统时，这正是我们需要做的事情。

我们将定义两个单独的指令集：一个用于控制台指令，一个用于记录器指令，如下所示。

```F#
type ConsoleInstruction<'a> =
  | ReadLn  of unit    * next:(string -> 'a)
  | WriteLn of string  * next:(unit   -> 'a)

type LoggerInstruction<'a> =
  | LogDebug of string * next:(unit -> 'a)
  | LogInfo of string  * next:(unit -> 'a)
```

现在我们可以使用两条指令定义我们的 `Program` 类型，再加上像以前一样的 `Stop`：

```F#
type Program<'a> =
  | ConsoleInstruction of ConsoleInstruction<Program<'a>>
  | LoggerInstruction of LoggerInstruction<Program<'a>>
  | Stop of 'a
```

如果我们需要更多的指令，我们只需将其添加为新的选项。

*注意：如果我们能将所有这些选项折叠成一个以类型为参数的高阶选项，那就太好了。我们很快就会看到一种 F# 友好的方法。*

接下来，我们需要为程序实现一个 `bind` 函数。现在事实证明，我们不需要为每条指令实现 `bind`，我们只需要实现一个 `map` 函数。`bind` 函数只需要整个程序。

以下是两个 `map` 功能：

```F#
module ConsoleInstruction =
  let rec map f program =
    match program with
    | ReadLn ((),next) -> ReadLn ((),next >> f)
    | WriteLn (str,next) -> WriteLn (str, next >> f)

module LoggerInstruction =
  let rec map f program =
    match program with
    | LogDebug (str,next) -> LogDebug (str,next >> f)
    | LogInfo (str,next) -> LogInfo (str,next >> f)
```

这是程序的 `bind` 函数：

```F#
module Program =
  let rec bind f program =
    match program with
    | ConsoleInstruction inst ->
        inst |> ConsoleInstruction.map (bind f) |> ConsoleInstruction
    | LoggerInstruction inst ->
        inst |> LoggerInstruction.map (bind f) |> LoggerInstruction
    | Stop x ->
        f x
```

计算表达式的代码与之前完全相同：

```F#
type ProgramBuilder() =
  member __.Return(x) = Stop x
  member __.Bind(x,f) = Program.bind f x
  member __.Zero() = Stop ()

// the builder instance
let program = ProgramBuilder()
```

最后，我们可以以与之前相同的方式实现解释器，除了这个解释器有两个子解释器用于两个指令集：

```F#
let rec interpret program =

  let interpretConsole inst =
    match inst with
    | ReadLn ((), next) ->
        let str = Console.ReadLine()
        interpret (next str)
    | WriteLn (str,next) ->
        printfn "%s" str
        interpret (next())

  let interpretLogger inst =
    match inst with
    | LogDebug (str, next) ->
        printfn "DEBUG %s" str
        interpret (next())
    | LogInfo (str, next) ->
        printfn "INFO %s" str
        interpret (next())

  match program with
  | ConsoleInstruction inst -> interpretConsole inst
  | LoggerInstruction inst -> interpretLogger inst
  | Stop value -> value
```

### 建造管道

在上一篇文章的 Reader 方法中，我们将迷你应用程序分为三个组件：

- readFromConsole
- compareTwoStrings
- writeToConsole

我们也将在解释器方法中重复使用这种相同的分区。

首先，让我们定义一些为我们构建 `Program` 的助手。

```F#
let writeLn str = ConsoleInstruction (WriteLn (str,Stop))
let readLn() = ConsoleInstruction (ReadLn ((),Stop))
let logDebug str = LoggerInstruction (LogDebug (str,Stop))
let logInfo str = LoggerInstruction (LogInfo (str,Stop))
```

现在我们可以创建迷你应用程序的三个组件：

```F#
let readFromConsole = program {
  do! writeLn "Enter the first value"
  let! str1 = readLn()
  do! writeLn "Enter the second value"
  let! str2 = readLn()
  return  (str1,str2)
  }
```

和

```F#
let compareTwoStrings str1 str2 = program {
  do! logDebug "compareTwoStrings: Starting"

  let result =
    if str1 > str2 then
      Bigger
    else if str1 < str2 then
      Smaller
    else
      Equal

  do! logInfo (sprintf "compareTwoStrings: result=%A" result)
  do! logDebug "compareTwoStrings: Finished"
  return result
  }
```

和

```F#
let writeToConsole (result:ComparisonResult) = program {
  match result with
  | Bigger ->
      do! writeLn "The first value is bigger"
  | Smaller ->
      do! writeLn "The first value is smaller"
  | Equal ->
      do! writeLn "The values are equal"
  }
```

把它们放在一起，我们就有了最终的计划：

```F#
let myProgram = program {
  let! str1, str2 = readFromConsole
  let! result = compareTwoStrings str1 str2
  do! writeToConsole result
  }
```

为了“执行”这个程序，我们只需将其传递给解释器：

```F#
interpret myProgram
```

## 处理多个指令集的模块化方法

前一种方法的缺点是，每次我们需要添加一组新的指令时，我们都需要修改主 `Program` 类型，这是脆弱和反模块的。那么，让我们快速看看另一种方法。

在 Haskell 和其他支持类型类（特别是 Functor）的语言中，我们可以构造一个“Free Monad”。我们正在编写 F#，而不是 Haskell，所以让我们使用接口吧！

首先，我们定义一个指令必须实现的接口。它有一个成员，`Map` 方法：

```F#
type IInstruction<'a> =
  abstract member Map : ('a->'b) -> IInstruction<'b>
```

然后我们定义我们的 `Program` 以使用该类型

```F#
type Program<'a> =
  | Instruction of IInstruction<Program<'a>>
  | Stop  of 'a
```

最后，我们可以使用与指令关联的 `map` 方法定义 `bind`：

```F#
module Program =
  let rec bind f program =
    match program with
    | Instruction inst ->
        inst.Map (bind f) |> Instruction
    | Stop x ->
        f x
```

计算表达式生成器保持不变。

到目前为止，这是完全通用和可重用的代码，不知道任何特定的指令集。

### 定义说明

为了实现特定的工作流程，我们首先定义一些指令及其映射方法。这些指令中的每一条都不知道其他指令，因此与其他指令解耦。

```F#
type ConsoleInstruction<'a> =
  | ReadLn  of unit    * next:(string -> 'a)
  | WriteLn of string  * next:(unit   -> 'a)
  interface IInstruction<'a> with
    member this.Map f =
      match this with
      | ReadLn ((),next) -> ReadLn ((),next >> f)
      | WriteLn (str,next) -> WriteLn (str, next >> f)
      :> IInstruction<'b>

type LoggerInstruction<'a> =
  | LogDebug of string * next:(unit -> 'a)
  | LogInfo of string  * next:(unit -> 'a)
  interface IInstruction<'a> with
    member this.Map f =
      match this with
      | LogDebug (str,next) -> LogDebug (str,next >> f)
      | LogInfo (str,next) -> LogInfo (str,next >> f)
      :> IInstruction<'b>
```

与早期实现的唯一区别是，`Map` 方法必须将结果转换回 `IInstruction`。

在计算表达式中使用的辅助函数几乎相同，但现在它们使用更通用的 `Instruction` 案例。

```F#
let writeLn str = Instruction (WriteLn (str,Stop))
let readLn() = Instruction (ReadLn ((),Stop))
let logDebug str = Instruction (LogDebug (str,Stop))
let logInfo str = Instruction (LogInfo (str,Stop))
```

尽管我们使用了新的泛型 `Program` 类型，但辅助函数隐藏了任何更改，导致主应用程序代码不变。例如，`readFromConsole` 看起来与之前完全相同：

```F#
let readFromConsole = program {
  do! writeLn "Enter the first value"
  let! str1 = readLn()
  do! writeLn "Enter the second value"
  let! str2 = readLn()
  return  (str1,str2)
  }
```

### 构建模块化解释器

我们也希望以模块化的方式构建解释器。同样，为了保持解耦，我们希望特定指令集的解释器不知道顶级解释器，因此我们将把 `interpret` 函数作为参数传入：

```F#
// modular interpreter for ConsoleInstruction
let interpretConsole interpret inst =
  match inst with
  | ReadLn ((), next) ->
      let str = Console.ReadLine()
      interpret (next str)
  | WriteLn (str,next) ->
      printfn "%s" str
      interpret (next())

// modular interpreter for LoggerInstruction
let interpretLogger interpret inst =
  match inst with
  | LogDebug (str, next) ->
      printfn "DEBUG %s" str
      interpret (next())
  | LogInfo (str, next) ->
      printfn "INFO %s" str
      interpret (next())
```

为了完成所有工作，我们只需要定义顶级解释器。同样，这与早期的实现非常相似，除了我们现在根据指令的类型进行匹配，而不是详尽地匹配固定的案例列表。这并不像让编译器为你检查一切那么安全，但如果你忘记处理指令，很快就会很明显！

```F#
let rec interpret program =
  match program with
  | Instruction inst ->
      match inst with
      | :? ConsoleInstruction<Program<_>> as i ->
             interpretConsole interpret i
      | :? LoggerInstruction<Program<_>> as i ->
             interpretLogger interpret i
      | _ -> failwithf "unknown instruction type %O" (inst.GetType())
  | Stop value ->
      value
```

这种方法的优点是它更加模块化。我们可以使用不同的指令集彼此独立地编写子组件，然后在以后组合它们。唯一需要更改的是特定工作流的顶级解释器，而该解释器又可以由多个独立的子解释器构建。

## 进一步阅读

有关使用解释器方法的另一个示例，请参阅本系列的最后一篇文章。

我在这里使用的解释器方法与 Haskell 和 FP 风格的 Scala 中使用的“Free Monad”方法密切相关。Free Monad 更加抽象，并使用更多的数学术语来命名 `Program` 类型中的案例，即“Free”和“Pure”，而不是“Instruction”和“Stop”。然而，我认为即使你很少在实践中使用它，也值得花一些时间来理解它。

Mark Seemann 写了一些关于 F# 中自由单子的很好的文章，比如一篇关于你可以遵循的“食谱”，另一篇关于如何将自由单子“堆叠”在一起。

关于在实践中使用 Interpreter/Free Monad 的真实故事，这里有 Chris Myers 的精彩演讲，尽管他正在使用 Scala。关于反例，请参阅 Kelley Robinson 的《免费单子不是免费的》。

## 解释器的利弊

如您所见，使用解释器可以生成非常干净的代码，其中所有依赖项都被隐藏。处理 IO（例如 `Async`）的所有麻烦都消失了（或者更确切地说，被推给了解释器）。

另一个好处是，如果你需要使用不同的基础设施，你可以很容易地切换出解释器。例如，我可以更改记录器解释以使用 Serilog 等记录器，也可以更改控制台解释以使用文件或套接字。“全局”值（如记录器）可以在解释器循环中轻松管理，而不会影响主程序逻辑。

但和往常一样，也有权衡。

首先，有很多额外的工作。您必须定义和解释工作流所需的每一个可能的 I/O 操作，这可能会很乏味。如果你不小心，手术的数量很容易失控。用小型独立工作流构建系统的一个优点是，对于任何特定的工作流来说，操作数量都不应该太高。

其次，如果你还不熟悉这种方法，就很难理解发生了什么。与不需要任何特殊知识的“依赖拒绝”和“依赖参数化”技术不同，阅读器和解释器方法都需要相当多的专业知识。如果你需要在调试器中逐步执行代码，那么嵌套很深的 continuation 会让事情变得非常复杂。

接下来，与往常一样，计算表达式的缺点之一是很难混合和匹配它们。例如，在上一篇文章中，我提到将 `Reader` 表达式与 `Result` 表达式和 `Async` 表达式混合使用会很棘手。解释器方法稍微缓解了这个问题，因为你永远不必在主“程序”代码中处理 `Async` 之类的事情，大多数时候甚至不必处理 `Result`。但即便如此，当你确实需要处理这个问题时，这可能会很痛苦。

最后，另一个问题是性能。如果你有一个包含 1000 条指令的大型程序，那么你将拥有一个非常非常嵌套的数据结构。解释可能很慢，使用大量内存，触发更多垃圾收集，甚至可能导致堆栈溢出。有一些变通方法（如“蹦床”），但这使代码变得更加复杂。

因此，总而言之，只有在以下情况下，我才会推荐这种方法：（a）你真的关心将 I/O 与纯代码分离；（b）团队中的每个人都熟悉这种技术；（c）你有技能和专业知识来处理可能出现的任何性能问题。

在下一篇文章中，我们将重新审视所讨论的所有技术，并将其应用于一个新的示例。

这篇文章的源代码可以在这里找到。

# 5 重新审视六种方法

*Part of the "Dependency Injection" series (*[link](https://fsharpforfunandprofit.com/posts/dependencies-5/#series-toc)*)*

依赖注入的六种方法，第5部分
2020年12月24日

https://fsharpforfunandprofit.com/posts/dependencies-5/

在本系列中，我们研究了六种不同的依赖注入方法。

- 在第一篇文章中，我们研究了“依赖保留”（内联依赖）和“依赖拒绝”，即将I/O保持在实现的边缘。
- 在第二篇文章中，我们研究了使用标准函数参数注入依赖关系。
- 在第三篇文章中，我们研究了使用经典 OO 风格的依赖注入和 FP 等效物（Reader monad）进行依赖处理。
- 在第四篇文章中，我们研究了通过使用解释器模式来完全避免依赖关系。

在最后一篇文章中，我们将使用所有六种方法实现一些简单的需求，以便您可以看到其中的差异。我不会详细解释发生了什么。为此，你应该阅读之前的帖子。

## 要求

让我们来看一个具体的用例，我们可以将其作为实验不同实现的基础。

假设我们有一个带有用户的网络应用程序，每个用户都有一个“个人资料”，上面有他们的姓名、电子邮件、偏好等。更新他们的个人资料的用例可能是这样的：

- 接收一个新的配置文件（比如从 JSON 请求解析）
- 从数据库中读取用户的当前配置文件
- 如果配置文件已更改，请更新数据库中的用户配置文件
- 如果电子邮件已更改，请向用户的新电子邮件发送验证电子邮件

我们还将在组合中添加一点日志记录。

## 领域

让我们从要使用的域类型开始：

```F#
module Domain =
  type UserId = UserId of int
  type UserName = string
  type EmailAddress = EmailAddress of string

  type Profile = {
    UserId : UserId
    Name : UserName
    EmailAddress : EmailAddress
  }

  type EmailMessage = {
    To : EmailAddress
    Body : string
    }
```

以下是用于日志记录、数据库和电子邮件的基础设施服务：

```F#
module Infrastructure =
  open Domain

  type ILogger =
    abstract Info : string -> unit
    abstract Error : string -> unit

  type InfrastructureError =
    | DbError of string
    | SmtpError of string

  type DbConnection = DbConnection of unit // dummy definition

  type IDbService =
    abstract NewDbConnection :
      unit -> DbConnection
    abstract QueryProfile :
      DbConnection -> UserId -> Async<Result<Profile,InfrastructureError>>
    abstract UpdateProfile :
      DbConnection -> Profile -> Async<Result<unit,InfrastructureError>>

  type SmtpCredentials = SmtpCredentials of unit // dummy definition

  type IEmailService =
    abstract SendChangeNotification :
      SmtpCredentials -> EmailMessage -> Async<Result<unit,InfrastructureError>>
```

关于基础设施，有几点需要注意：

- DB 和电子邮件服务分别接受一个额外的参数：`DbConnection` 和 `SmtpCredentials`。我们必须以某种方式传递它，但最好隐藏它，因为它不是功能的核心部分。
- DB 和电子邮件服务返回 `AsyncResult`，这表明它们是不纯的，也可能因 `InfrastructureErrors` 而失败。这很有帮助，但也意味着将它们与其他效果（如阅读器）结合起来会很烦人。
- 记录器不会返回 `AsyncResult`，即使它是不纯的。在域代码中间使用记录器不应对业务逻辑产生任何影响。

我们将假设有一个全局记录器和这些服务的默认实现可供我们使用。

## 方法 #1：依赖保留

我们的第一个实现将直接使用所有依赖关系，而不尝试抽象或参数化。

笔记：

- 基础设施服务返回 `AsyncResult`，因此我们使用 `asyncResult` 计算表达式使代码更容易编写和理解。
- 决策（`if currentProfile <> newProfile`）和不纯代码混合在一起。

```F#
let updateCustomerProfile (newProfile:Profile) =
  let dbConnection = defaultDbService.NewDbConnection()
  let smtpCredentials = defaultSmtpCredentials
  asyncResult {
    let! currentProfile =
      defaultDbService.QueryProfile dbConnection newProfile.UserId

    if currentProfile <> newProfile then
      globalLogger.Info("Updating Profile")
      do! defaultDbService.UpdateProfile dbConnection newProfile

    if currentProfile.EmailAddress <> newProfile.EmailAddress then
      let emailMessage = {
        To = newProfile.EmailAddress
        Body = "Please verify your email"
        }
      globalLogger.Info("Sending email")
      do! defaultEmailService.SendChangeNotification smtpCredentials emailMessage
    }
```

正如我们在第一篇文章中讨论的那样，我认为这种方法适用于小脚本，或者用于快速组装原型或草图。但这段代码很难正确测试，如果它变得更复杂，我强烈建议重构，将纯代码与不纯代码分开——“依赖拒绝”方法。

## 方法 #2：依赖拒绝

当我在之前的一篇文章中讨论“依赖拒绝”时，我使用这个图来展示最终目标：将纯的、确定性的代码与不纯的、非确定性的代码分开。

![img](https://fsharpforfunandprofit.com/posts/dependencies/Dependencies2a.jpg)

那么，让我们将这种方法应用于我们的示例。决定如下：

- 什么都不做
- 仅更新数据库
- 更新数据库并发送验证电子邮件

所以，让我们把这个决定编码为一个类型。

```F#
type Decision =
  | NoAction
  | UpdateProfileOnly of Profile
  | UpdateProfileAndNotify of Profile * EmailMessage
```

现在，代码中纯粹的决策部分可以这样实现：

```F#
let updateCustomerProfile (newProfile:Profile) (currentProfile:Profile) =
  if currentProfile <> newProfile then
    globalLogger.Info("Updating Profile")
    if currentProfile.EmailAddress <> newProfile.EmailAddress then
      let emailMessage = {
        To = newProfile.EmailAddress
        Body = "Please verify your email"
        }
      globalLogger.Info("Sending email")
      UpdateProfileAndNotify (newProfile, emailMessage)
    else
      UpdateProfileOnly newProfile
  else
    NoAction
```

在这个实现中，我们不从数据库中读取。相反，我们将 `currentProfile` 作为参数传入。我们不会写入数据库。相反，我们返回 `Decision` 类型来告诉后面的不纯部分该做什么。

因此，这段代码很容易测试。

请注意，记录器不是作为参数传递的——我们只是使用 `globalLogger`。我认为，在某些情况下，日志记录可能是访问全局变量规则的例外。如果这让你感到困扰，在下一节中，我们将把它变成一个参数！

现在代码的“纯”决策部分已经完成，我们可以实现顶级代码。很明显，我们现在有一个不纯/纯/不纯的三明治，正如我们想要的那样：

```F#
let updateCustomerProfile (newProfile:Profile) =
  let dbConnection = defaultDbService.NewDbConnection()
  let smtpCredentials = defaultSmtpCredentials
  asyncResult {
    // ----------- impure ----------------
    let! currentProfile =
      defaultDbService.QueryProfile dbConnection newProfile.UserId

    // ----------- pure ----------------
    let decision = Pure.updateCustomerProfile newProfile currentProfile

    // ----------- impure ----------------
    match decision with
    | NoAction ->
        ()
    | UpdateProfileOnly profile ->
        do! defaultDbService.UpdateProfile dbConnection profile
    | UpdateProfileAndNotify (profile,emailMessage) ->
        do! defaultDbService.UpdateProfile dbConnection profile
        do! defaultEmailService.SendChangeNotification smtpCredentials emailMessage
    }
```

像这样把代码分成两部分很容易，而且有很多好处。因此，“依赖拒绝”应该始终是你做的第一次重构。

在本文的其余部分，即使我们使用了其他技术，我们也会将决策部分和 IO 使用部分分开。

## 方法 #3：依赖参数化

我们现在已经将纯代码与不纯代码分开，除了记录器，它不能轻易地与纯代码分离。

让我们解决这个记录器问题。至少，使测试更容易的最简单方法是将记录器作为参数传递给纯内核，如下所示：

```F#
let updateCustomerProfile (logger:ILogger) (newProfile:Profile) (currentProfile:Profile) =
  if currentProfile <> newProfile then
    logger.Info("Updating Profile")
    if currentProfile.EmailAddress <> newProfile.EmailAddress then
      ...
      logger.Info("Sending email")
      UpdateProfileAndNotify (newProfile, emailMessage)
    else
      UpdateProfileOnly newProfile
  else
    NoAction
```

如果我们愿意，我们也可以在顶级不纯代码中对服务进行参数化。如果有很多基础设施服务，通常会将它们捆绑成一种类型：

```F#
type IServices = {
  Logger : ILogger
  DbService : IDbService
  EmailService : IEmailService
  }
```

然后，可以将此类型的参数传递到顶级代码中，如下所示。我们之前直接使用 `defaultDbService` 的地方，现在都在使用 `services` 参数。请注意，`logger` 是从服务中提取出来的，然后作为参数传递给我们上面实现的纯函数。

```F#
let updateCustomerProfile (services:IServices) (newProfile:Profile) =
  let dbConnection = services.DbService.NewDbConnection()
  let smtpCredentials = defaultSmtpCredentials
  let logger = services.Logger

  asyncResult {
    // ----------- Impure ----------------
    let! currentProfile =
      services.DbService.QueryProfile dbConnection newProfile.UserId

    // ----------- pure ----------------
    let decision = Pure.updateCustomerProfile logger newProfile currentProfile

    // ----------- Impure ----------------
    match decision with
    | NoAction ->
        ()
    | UpdateProfileOnly profile ->
        do! services.DbService.UpdateProfile dbConnection profile
    | UpdateProfileAndNotify (profile,emailMessage) ->
        do! services.DbService.UpdateProfile dbConnection profile
        do! services.EmailService.SendChangeNotification smtpCredentials emailMessage
    }
```

传递这样的 `services` 参数可以很容易地模拟服务或更改实现。这是一个简单的重构，不需要任何特殊的专业知识，因此与“依赖拒绝”一样，如果代码变得难以测试，这是我首先要做的重构之一。

## 方法 #4a：OO 风格的依赖注入

OO 传递依赖关系的方式通常是在创建对象时将它们传递给构造函数。这不是功能优先设计的默认方法，但如果你正在编写将从 C# 使用的 F# 代码，或者你在一个期望这种依赖注入的 C# 框架内工作，那么这就是你应该使用的技术。

```F#
// define a class with a constructor that accepts the dependencies
type MyWorkflow (services:IServices) =

  member this.UpdateCustomerProfile (newProfile:Profile) =
    let dbConnection = services.DbService.NewDbConnection()
    let smtpCredentials = defaultSmtpCredentials
    let logger = services.Logger

    asyncResult {
      // ----------- Impure ----------------
      let! currentProfile = services.DbService.QueryProfile dbConnection newProfile.UserId

      // ----------- pure ----------------
      let decision = Pure.updateCustomerProfile logger newProfile currentProfile

      // ----------- Impure ----------------
      match decision with
      | NoAction ->
          ()
      | UpdateProfileOnly profile ->
          do! services.DbService.UpdateProfile dbConnection profile
      | UpdateProfileAndNotify (profile,emailMessage) ->
          do! services.DbService.UpdateProfile dbConnection profile
          do! services.EmailService.SendChangeNotification smtpCredentials emailMessage
      }
```

如您所见，`UpdateCustomerProfile` 方法没有显式的服务参数，而是使用整个类范围内的 `services` 字段。

好处是方法调用本身更简单。缺点是该方法现在依赖于类的上下文，这使得独立重构和测试变得更加困难。

## 方法 #4b：阅读器单子

延迟依赖注入的 FP 等效物是 `Reader` 类型及其相关工具，如 `reader` 计算表达式。有关 Reader monad 的更多讨论，请参阅前面的文章。

这是为返回包含 `ILogger` 作为环境的 `Reader` 而编写的代码的纯部分。

```F#
let updateCustomerProfile (newProfile:Profile) (currentProfile:Profile) =
  reader {
    let! (logger:ILogger) = Reader.ask

    let decision =
      if currentProfile <> newProfile then
        logger.Info("Updating Profile")
        if currentProfile.EmailAddress <> newProfile.EmailAddress then
          let emailMessage = {
            To = newProfile.EmailAddress
            Body = "Please verify your email"
            }
          logger.Info("Sending email")
          UpdateProfileAndNotify (newProfile, emailMessage)
        else
          UpdateProfileOnly newProfile
      else
        NoAction

    return decision
  }
```

`updateCustomerProfile` 的返回类型是 `Reader<ILogger, Decision>`，正如我们所希望的那样。

我们可以从顶层代码运行阅读器，如下所示：

```F#
let updateCustomerProfile (services:IServices) (newProfile:Profile) =
  let logger = services.Logger

  asyncResult {
    // ----------- impure ----------------
    let! currentProfile = ...

    // ----------- pure ----------------
    let decision =
      Pure.updateCustomerProfile newProfile currentProfile
      |> Reader.run logger

    // ----------- impure ----------------
    match decision with
	... etc
```

### 对顶级依赖项也使用 Reader

如果你真的想使用 Reader，我建议你只使用它来隐藏纯代码中的“无效”依赖关系，比如日志记录。如果你使用 Reader 来处理返回不同效果的不纯代码，比如 `AsyncResult`，它可能会变得非常混乱。

为了证明这一点，让我们将不纯的代码划分为两个新函数，每个函数都返回一个 Reader：

第一个函数将从数据库中读取配置文件。它需要一个 `IServices` 作为读取器的环境，并将返回 `AsyncResult<Profile,InfrastructureError>`。因此，总体返回类型将是 `Reader<IServices, AsyncResult<Profile,InfrastructureError>>` ，这相当粗糙。

```F#
let getProfile (userId:UserId) =
  reader {
    let! (services:IServices) = Reader.ask
    let dbConnection = services.DbService.NewDbConnection()
    return services.DbService.QueryProfile dbConnection userId
  }
```

第二个函数将处理决策，并在需要时更新数据库中的配置文件。同样，它需要一个 `IServices` 作为读取器的环境，它将返回一个封装在 `AsyncResult` 中的 `unit`。因此，总体返回类型将是 `Reader<IServices, AsyncResult<unit,InfrastructureError>>`。

```F#
let handleDecision (decision:Decision) =
  reader {
    let! (services:IServices) = Reader.ask
    let dbConnection = services.DbService.NewDbConnection()
    let smtpCredentials = defaultSmtpCredentials
    let action = asyncResult {
      match decision with
      | NoAction ->
          ()
      | UpdateProfileOnly profile ->
          do! services.DbService.UpdateProfile dbConnection profile
      | UpdateProfileAndNotify (profile,emailMessage) ->
          do! services.DbService.UpdateProfile dbConnection profile
          do! services.EmailService.SendChangeNotification smtpCredentials emailMessage
      }
    return action
  }
```

同时使用多种不同的效果（在本例中为 `Reader`、`Async` 和 `Result`）是非常痛苦的。像 Haskell 这样的语言有一些变通方法，但 F# 并不是真正为实现这一点而设计的。最简单的方法是为组合效果集编写自定义计算表达式。`Async` 和 `Result` 效果经常一起使用，因此使用特殊的 `asyncResult` 计算表达式是有意义的。但是，如果我们将 `Reader` 添加到组合中，我们需要一个类似 `readerAsyncResult` 计算表达式的东西。

在下面的实现中，我懒得这样做。相反，我只是在整个 `asyncResult` 表达式中根据需要为每个组件函数运行 Reader。它很丑，但它有效。

```F#
let updateCustomerProfile (newProfile:Profile) =
  reader {
    let! (services:IServices) = Reader.ask
    let getLogger services = services.Logger

    return asyncResult {
      // ----------- impure ----------------
      let! currentProfile =
        getProfile newProfile.UserId
        |> Reader.run services

      // ----------- pure ----------------
      let decision =
        Pure.updateCustomerProfile newProfile currentProfile
        |> Reader.withEnv getLogger
        |> Reader.run services

      // ----------- impure ----------------
      do! (handleDecision decision) |> Reader.run services
      }
  }
```

## 方法 #5：依赖关系解释

最后，我们将看看如何应用上一篇文章中讨论的解释器方法。

要编写程序，我们需要：

- 定义我们要使用的指令集。这些将是数据结构，而不是函数。
- 为每个指令集实现 `IInstruction`，以便它可以与我们在上一篇文章中定义的通用“程序”库一起使用。
- 创建一些辅助函数，以便更容易创建指令
- 然后我们可以使用 `program` 计算表达式编写代码

完成后，我们需要解释程序：

- 我们将为每个指令集创建子解释器
- 然后，我们将为整个程序创建一个顶级解释器，根据需要调用子解释器。

我们可以选择只对代码的纯部分这样做，也可以对代码的不纯部分这样做。让我们从纯粹的部分开始。

### 开发纯组件

首先，我们需要为纯代码定义指令集。现在，我们唯一需要的就是记录。因此，我们需要：

- `LoggerInstruction` 类型，每个日志记录操作都有一个案例
- `IIconstruction` 及其关联 `Map` 方法的实现
- 一些辅助函数来构建各种指令

代码如下：

```F#
type LoggerInstruction<'a> =
  | LogInfo of string  * next:(unit -> 'a)
  | LogError of string * next:(unit -> 'a)
  interface IInstruction<'a> with
    member this.Map f  =
      match this with
      | LogInfo (str,next) ->
          LogInfo (str,next >> f)
      | LogError (str,next) ->
          LogError (str,next >> f)
      :> IInstruction<_>

// helpers to use within the computation expression
let logInfo str = Instruction (LogInfo (str,Stop))
let logError str = Instruction (LogError (str,Stop))
```

使用此指令集，我们可以编写纯部分，抽象掉我们在早期实现中所需的 logger 参数。

```F#
let updateCustomerProfile (newProfile:Profile) (currentProfile:Profile) =
  if currentProfile <> newProfile then program {
    do! logInfo("Updating Profile")
    if currentProfile.EmailAddress <> newProfile.EmailAddress then
      let emailMessage = {
        To = newProfile.EmailAddress
        Body = "Please verify your email"
        }
      do! logInfo("Sending email")
      return UpdateProfileAndNotify (newProfile, emailMessage)
    else
      return UpdateProfileOnly newProfile
    }
  else program {
    return NoAction
    }
```

`updateCustomerProfile` 的返回类型只是 `Program<Decision>`。在任何地方都没有提到具体的 `ILogger`！

请注意，主 if/then/else 表达式的每个分支都有子 `programs`。在计算表达式中嵌套 `let!` 和 `do!` 的规则的构造不是特别直观，您可能会遇到诸如“此构造只能在计算表达式中使用”之类的错误。有时需要稍作调整才能使其正确。

### 开发不纯成分

如果我们想用解释型调用替换所有直接 I/O 调用，那么我们需要为它们创建指令集。因此，我们将使用如下所示的指令类型，而不是 `IDbService` 和 `IEmailService` 接口：

```F#
type DbInstruction<'a> =
  | QueryProfile of UserId * next:(Profile -> 'a)
  | UpdateProfile of Profile * next:(unit -> 'a)
  interface IInstruction<'a> with
    member this.Map f  =
      match this with
      | QueryProfile (userId,next) ->
          QueryProfile (userId,next >> f)
      | UpdateProfile (profile,next) ->
          UpdateProfile (profile, next >> f)
      :> IInstruction<_>

type EmailInstruction<'a> =
  | SendChangeNotification of EmailMessage * next:(unit-> 'a)
  interface IInstruction<'a> with
    member this.Map f  =
      match this with
      | SendChangeNotification (message,next) ->
          SendChangeNotification (message,next >> f)
      :> IInstruction<_>
```

以及在计算表达式中使用的助手：

```F#
let queryProfile userId =
  Instruction (QueryProfile(userId,Stop))
let updateProfile profile =
  Instruction (UpdateProfile(profile,Stop))
let sendChangeNotification message =
  Instruction (SendChangeNotification(message,Stop))
```

### 编写 shell 程序

与 Reader 实现一样，我们将把系统分解为三个组件：

- `getProfile`。将从数据库中读取配置文件的不纯部分。
- `updateCustomerProfile`。我们上面实现的纯部分。
- `handleDecision`。一个不纯的部分，将处理决策并在需要时更新数据库中的配置文件。

这是使用 `queryProfile` 助手实现 `getProfile` 的过程，作为提醒，它实际上创建了 `QueryProfile` 指令，但什么也不做。

```F#
let getProfile (userId:UserId) :Program<Profile> =
  program {
    return! queryProfile userId
  }
```

下面是 `handleDecision` 的实现。请注意，对于 `NoAction` 的情况，我想返回 `unit`，但包裹在 `Program` 中。这正是什么 `program.Zero()` 是。我也可以使用 `program { return() }` 来达到同样的效果。

```F#
let handleDecision (decision:Decision) :Program<unit> =
    match decision with
    | NoAction ->
        program.Zero()
    | UpdateProfileOnly profile ->
        updateProfile profile
    | UpdateProfileAndNotify (profile,emailMessage) ->
        program {
        do! updateProfile profile
        do! sendChangeNotification emailMessage
        }
```

有了这三个函数，实现顶级函数就很简单了。

```F#
let updateCustomerProfile (newProfile:Profile) =
  program {
    let! currentProfile = getProfile newProfile.UserId
    let! decision = Pure.updateCustomerProfile newProfile currentProfile
    do! handleDecision decision
  }
```

它看起来非常干净——任何地方都没有 `AsyncResults`！这使得它比之前实现的 Reader 版本更干净。

### 创建子解释器

但现在我们来到了棘手的部分：实现子解释器和顶级解释器。由于基础设施服务都返回 `AsyncResult`，这一点变得更加复杂。我们所做的一切都必须纳入这一背景。

让我们先浏览一下 `DbInstruction` 的解释器。（在下面的代码中，我添加了一个“AS”后缀，以显示哪些值是 AsyncResults。）

为了理解发生了什么，让我们从一条指令开始，即 `QueryProfile` 的解释器。

```F#
| QueryProfile (userId, next) ->
    let profileAS = defaultDbService.QueryProfile dbConnection userId
    let newProgramAS = (AsyncResult.map next) profileAS
    interpret newProgramAS
```

首先，我们调用基础设施服务，它返回 AsyncResult。

```F#
let profileAS = defaultDbService.QueryProfile dbConnection userId
```

然后我们调用 `next` 函数来获取下一个要解释的程序。但是 `next` 函数不适用于 AsyncResult，所以我们必须使用 `AsyncResult.map` 将其“提升”到一个适用的函数中。此时，我们可以使用 `profileAS` 调用它，并返回一个封装在 AsyncResult 中的新程序。

```F#
let newProgramAS = (AsyncResult.map next) profileAS
```

最后，我们可以解释程序。通常，解释器会接受一个 `Program<'a>` 并返回一个 `'a`。但由于 AsyncResult 污染了一切，`interpret` 函数需要接受一个 `AsyncResult<Program<'a>>` 并返回 `AsyncResult<'a>`。

```F#
interpret newProgramAS   // returns an AsyncResult<'a,InfrastructureError>
```

以下是 `interpetDbInstruction` 的完整实现：

```F#
let interpretDbInstruction (dbConnection:DbConnection) interpret inst =
  match inst with
  | QueryProfile (userId, next) ->
      let profileAS = defaultDbService.QueryProfile dbConnection userId
      let newProgramAS = (AsyncResult.map next) profileAS
      interpret newProgramAS
  | UpdateProfile (profile, next) ->
      let unitAS = defaultDbService.UpdateProfile dbConnection profile
      let newProgramAS = (AsyncResult.map next) unitAS
      interpret newProgramAS
```

另请注意，`interpretDbInstruction` 将 `dbConnection` 作为参数。打电话的人必须把它传进去。

`EmailInstruction` 的解释器实现类似。

对于 `LoggerInstruction` 解释器，我们需要稍作调整，因为记录器服务不使用 AsyncResult。在这种情况下，我们通过以通常的方式调用 `next` 来创建一个新程序，但随后使用 AsyncResult 将结果“提升”为 `AsyncResult.return`。

```F#
let interpretLogger interpret inst =
  match inst with
  | LogInfo (str, next) ->
      globalLogger.Info str
      let newProgramAS = next() |> asyncResult.Return
      interpret newProgramAS
  | LogError (str, next) ->
      ...
```

### 创建顶级解释器

尽管我们已经为每个指令集构建了子解释器，但我们不能放松。顶级口译员也相当复杂！

这是：

```F#
let interpret program =
  // 1. get the extra parameters and partially apply them to make all the interpreters
  // have a consistent shape
  let smtpCredentials = defaultSmtpCredentials
  let dbConnection = defaultDbService.NewDbConnection()
  let interpretDbInstruction' = interpretDbInstruction dbConnection
  let interpretEmailInstruction' = interpretEmailInstruction smtpCredentials

  // 2. define a recursive loop function. It has signature:
  //   AsyncResult<Program<'a>,InfrastructureError>) -> AsyncResult<'a,InfrastructureError>
  let rec loop programAS =
    asyncResult {
      let! program = programAS
      return!
        match program with
        | Instruction inst ->
            match inst with
            | :? LoggerInstruction<Program<_>> as inst -> interpretLogger loop inst
            | :? DbInstruction<Program<_>> as inst -> interpretDbInstruction' loop inst
            | :? EmailInstruction<Program<_>> as inst -> interpretEmailInstruction' loop inst
            | _ -> failwithf "unknown instruction type %O" (inst.GetType())
        | Stop value ->
            value |> asyncResult.Return
      }

  // 3. start the loop
  let initialProgram = program |> asyncResult.Return
  loop initialProgram
```

我把它分成三个部分。让我们依次浏览它们。

首先，我们获取额外的参数（`smtpCredentials` 和 `dbConnection`），并使用部分应用的这些参数创建解释器的本地变体。这使得所有解释器函数都处于相同的“形状”中。这不是绝对必要的，但我认为它有点干净。

接下来，我们定义一个本地“循环”函数，这是实际的解释器循环。使用这样的局部函数有很多优点。

- 它可以重用范围内的值，在这种情况下，在解释过程中一直使用相同的 `dbConnection`。
- 它可能具有与主要 `interpret` 不同的签名。在这种情况下，循环接受包装在 AsyncResults 中的程序，而不是普通的程序。

在 `loop` 函数内部，它处理程序的两种情况：

- 对于 `Instruction` 的情况，`loop` 函数调用子解释器，自行传递以递归解释下一步。
- 对于 `Stop` 的情况，它接受正常值，并使用 AsyncResult 将其包装成 `asyncResult.Return`

最后，在底部，我们开始循环。它需要一个 AsyncResult 作为输入，因此我们必须再次使用 `asyncResult.Return` 来提升初始输入程序。

现在有了解释器，可以完成最顶层的功能。其工作原理如下：

- 调用 `Shell.updateCustomerProfile`，返回一个 `Program`
- 然后使用 `interpret` 解释该程序，它返回 `AsyncResult`
- 然后运行 `AsyncResult` 以获得最终响应（这反过来可能需要转换为 HTTP 代码或类似代码）

```F#
let updateCustomerProfileApi (newProfile:Profile) =
  Shell.updateCustomerProfile newProfile
  |> interpret
  |> Async.RunSynchronously
```

### 解释器方法回顾

正如我们在上一篇文章中看到的，正如我们在这里看到的，解释器方法产生了非常干净的代码，所有的依赖关系都被隐藏了。处理 IO 和堆叠多个效果（例如 `Async` 包装 `Result`）的所有麻烦都消失了，或者更确切地说，被推给了解释器。

但是，要获得干净的代码需要做很多额外的工作。对于这个程序，我们只需要五条指令，但我们必须额外编写大约 100 行代码来支持它们！这是解释器的简单版本，只处理一种效果，AsyncResult。此外，在实践中，您可能还需要通过添加蹦床来避免堆栈溢出，这会使代码更加复杂。总的来说，我想说，对于大多数情况来说，这太费力了。

那么，什么时候这是个好主意呢？

- 如果你有一个用例，需要创建一个 DSL 或库供他人使用，并且有少量的指令，那么“前端”使用的简单性可能会超过“后端”解释器的复杂性。
- 如果你需要进行优化，如批处理 I/O 请求、缓存以前的结果等。通过将程序和解释分开，你可以在幕后进行这些优化，同时仍然有一个干净的前端。

这些要求适用于推特，推特的工程团队开发了一个名为 Stitch 的库，可以做这样的事情。这段视频有很好的解释，或者看这篇文章。脸书工程部门也有一个类似的库，名为 Haxl，也是出于同样的原因开发的。

## 摘要

在这篇文章中，我们将六种不同的技术应用于同一个例子。你最喜欢哪一个？

以下是我对每种方法的个人看法：

- **依赖保留**适用于小型脚本或不需要测试的地方。
- **依赖拒绝**始终是一个好主意，应该始终使用（低决策、高I/O工作流除外）。
- **依赖参数化**通常是使纯代码可测试的好主意。不需要在I/O密集的“边缘”对基础设施服务进行参数化，但通常很有用。
- 如果您与 OO 风格的 C# 或 OO 风格的框架交互，则应使用 **OO 风格的依赖注入**。不要让自己的生活变得艰难！
- **Reader monad** 不是我推荐的一种技术，除非你能在这里看到它比其他技术有明显的优势。
- **依赖解释**也不是我推荐的一种技术，除非你有一个其他技术都不起作用的特定用例。

不管我怎么看，所有的技术都可以放在你的工具箱里。特别是，了解阅读器和解释器实现的工作原理是很好的，即使你在实践中不怎么使用它们。

本文中所有示例代码的源代码都可以在此处找到。