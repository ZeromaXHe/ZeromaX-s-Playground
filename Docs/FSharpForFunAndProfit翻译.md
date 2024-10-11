# F# for Fun and Profit

# 你是一位经验丰富的开发人员吗？

https://fsharpforfunandprofit.com/

## 你想了解函数式编程的细枝末节（fuss）是什么吗？

本网站将向您介绍 F#，并向您展示 F# 如何帮助主流商业软件的日常开发。在路上，我希望让你对函数式编程的乐趣敞开心扉——它真的很有趣！
如果你从未听说过 F#，它是一种通用函数式/混合编程语言，非常适合应对几乎任何类型的软件挑战。F# 是免费开源的，可以在 Linux、Mac、Windows 等平台上运行。更多信息请访问 F# 基金会。

### 学会函数式思考

“函数式思维”对于充分利用 F# 至关重要，因此我将花大量时间掌握基础知识，并且通常会避免过多讨论混合和 OO 特性。

### 有用的例子

该网站将主要关注主流商业问题，如领域驱动设计、网站开发、数据处理、商业规则等。在示例中，我将尝试使用客户、产品和订单等商业概念，而不是过于学术化的概念。

### 别害怕

如果你在没有任何背景的情况下查看复杂的代码，F#可能会看起来非常吓人。一开始，我会保持简单，我试图预测函数式编程概念新手会遇到的问题。如果你慢慢地（按照正确的顺序）完成这些例子，你应该能理解一切。

### 玩得高兴！

许多人声称，学习函数式思维会“让你大吃一惊”。嗯，这是真的！学习一个全新的范式是令人兴奋和刺激的。你可能会再次爱上编程。

## 开始使用

如果您是 F# 的新手，请了解更多关于 F# 以及 F# 基金会如何使用F#的信息。要下载并安装 F#，请阅读安装和使用 F# 页面以开始。

接下来，在随机浏览帖子之前，你应该先阅读“为什么使用 F#？”页面，然后阅读整个“为什么使用 F#”系列。之后，“探索网站”页面提供了进一步阅读函数、类型等方面的建议。

有一个页面提供了一些关于学习 F# 的建议，如果你在编译代码时遇到问题，故障排除 F# 页面可能会有所帮助。

如果你更喜欢视频和幻灯片，而不是阅读冗长无聊的博客文章，为什么不看看视频页面呢？

我假设您不需要编程基础知识的指导，并且您熟悉 C#、Java 或类似的 C 语言。如果您熟悉 Mono/.NET库，它也会有所帮助。

另一方面，我不会假设你有数学或计算机科学背景。不会有数学符号，也不会有“函子”、“范畴论”和“变形论”等神秘概念。如果你已经熟悉 Haskell 或 ML，那么这里可能不适合你！

此外，我不会试图涵盖高度技术或数学的应用。F# 是这些领域的优秀工具，但它需要一种不同于商业软件的方法。

# 浏览此网站

游客指南

https://fsharpforfunandprofit.com/site-contents/

## 开始使用

- 为什么使用F#？F#的一页导览。如果你喜欢，有一个30页的系列，详细介绍了每个功能。
- 安装和使用F#将帮助您开始。
- F#语法60秒
- 学习F#有一些技巧可以帮助你更有效地学习。
- 当您在编译代码时遇到问题时，对F#进行故障排除。

然后你可以试试…

- 在工作中使用 F# 的 26 种低风险方法。你现在就可以开始——不需要许可！

## 函数式思维

- 函数式思维从基础开始，解释函数如何以及为什么以这种方式工作。
- “函数式设计模式介绍”讲座和“面向铁路的编程”讲座提供了更多函数式思维方式的例子。

## 理解F#

以下是关于 F# 关键概念的教程。

- 演讲：C# 开发者的 F#
- 表达式和语法涵盖了模式匹配等常见表达式，并有一篇关于缩进的文章。
- 了解 F# 类型解释了如何定义和使用各种类型，包括元组、记录、联合和选项。
- 在收集函数之间进行选择。如果你从 C# 开始学习 F#，那么大量的列表函数可能会让你不知所措，所以我写了这篇文章来帮助你找到你想要的。
- 理解计算表达式可以揭开它们的神秘面纱，并展示如何创建自己的表达式。
- F# 中的面向对象编程。
- 在项目中组织模块，我们查看 F# 项目的整体结构。特别是：（a）哪些代码应该包含在哪些模块中，以及（b）如何在项目中组织这些模块。
- 依赖循环。关于 F# 最常见的抱怨之一是，它要求代码按照依赖顺序排列。也就是说，您不能对编译器尚未看到的代码使用正向引用。在本系列中，我将讨论依赖循环，为什么它们是坏的，以及如何摆脱它们。
- 从 C# 移植。您想将 C# 代码移植到 F# 吗？在本系列文章中，我们将探讨实现这一目标的各种方法，以及所涉及的设计决策和权衡。

## 函数式设计

这些演讲和帖子展示了面向 FP 的设计与面向对象的设计有何不同。

- 讨论：使用 F# 类型系统进行领域建模。像 F# 这样的静态类型函数式编程语言鼓励人们以一种非常不同的方式思考类型。类型系统是你的朋友，而不是烦人的东西，它可以以许多面向对象程序员可能不熟悉的方式使用。类型可用于以细粒度、自文档化的方式表示域。在许多情况下，类型甚至可以用来对业务规则进行编码，这样你就不会创建错误的代码。然后，您可以将静态类型检查几乎用作即时单元测试，以确保您的代码在编译时是正确的。
- 使用类型进行设计解释了如何将类型用作设计过程的一部分，使非法状态无法表示。
- 代数类型大小和域建模
- 演讲：组合的力量。组合是函数式编程的基本原则，但它与面向对象的方法有什么不同，你在实践中如何使用它？在本次面向初学者的演讲中，我们将首先介绍函数式编程的基本概念，然后探讨一些不同的方法，即组合可以用来从小事物构建大事物。
- 演讲：观察乌龟的 13 种方法 演示了实现乌龟图形 API 的许多不同技术，包括状态 monad、代理、解释器等等！有关相关帖子，请参阅链接页面。
- 演讲：企业 Tic Tac Toe。跟随我荒谬地过度设计了一个简单的游戏，以演示如何使用函数式编程来创建一个现实世界的“企业就绪”应用程序。
- 演讲：使用能力进行设计展示了使用“能力”和最小权限原则进行设计的一种非常不同的方法。我将展示如何在整个核心领域（而不仅仅是在 API 边界）使用这些设计技术，从而产生设计良好的模块化代码。

## 函数式模式

这些讲座和帖子解释了函数式编程中的一些核心模式，如“map”、“bind”、monad 等概念。

- 讲座：函数式设计模式介绍。
- 讲座：面向铁路的编程：一种函数式错误处理方法。有关相关帖子，请参阅链接页面。
- 没有眼泪的 Monoids：对一种常见函数式模式的无数学讨论。
- 讲座：理解解析器组合器：从头开始创建解析器组合器库。有关相关帖子，请参阅链接页面。
- 演讲：《状态 Monad》：用《弗兰肯函子（Frankenfunctor）博士和 Monadster》的故事介绍如何处理状态。有关相关帖子，请参阅链接页面。
- Reader Monad：重塑阅读者单子。
- 映射（map）、绑定（bind）、应用（apply）、提升（lift）、序列（sequence）和遍历（traverse）：描述处理泛型数据类型的一些核心函数的系列。
- 折叠和递归类型：看看递归类型、同态、尾部递归、左折叠和右折叠之间的区别等等。
- “函数式授权方法”系列。如何使用“能力（capabilities）”处理授权的常见安全挑战。也可作为讲座。

## 测试

- 基于属性的测试介绍
- 为基于属性的测试选择属性
- 谈话：基于属性的测试：懒惰程序员编写 1000 个测试的指南。

## 示例和演练

这些帖子提供了大量代码的详细工作示例！

- 对“罗马数字 Kata 及其注释”的评论。我对罗马数字 Kata 的方法。
- 示例：为正确性而设计：如何使非法状态不可代表（购物车示例）。
- 示例：基于堆栈的计算器：使用一个简单的堆栈来演示组合子的威力。
- 示例：解析命令行：结合自定义类型使用模式匹配。
- 示例：罗马数字：另一个模式匹配示例。
- 计算器详解：设计计算器的类型优先方法。
- 企业 Tic Tac Toe：纯函数式实现中的设计决策演练
- 编写JSON解析器。

## F中的特定主题#

概述：

- 区分F#与标准命令式语言的四个关键概念。
- 理解F#缩进。
- 使用方法的缺点。
- 使用 `printf` 格式化文本。

函数：

- 柯里化。
- 部分应用。

控制流程：

- 匹配.. 使用表达式并创建折叠以隐藏匹配。
- If-then-else 和循环。
- 异常。

类型：

- 选项类型，特别是关于为什么None不等于null。
- 记录类型。
- 元组类型。
- 可区分联合（Discriminated Union）。
- 代数类型大小和域建模。

C#开发者的F#：

- 演讲：F#面向C#开发人员。
- 从C#移植。

## 其他文章

- 不使用静态类型函数式编程语言的十个理由。对我不理解的东西的咆哮。
- 为什么我不写monad教程
- 你的编程语言不合理吗？或者，为什么可预测性很重要。
- 我们不需要糟糕的UML图，或者为什么在许多情况下，使用UML绘制类图是不必要的。
- 内向和外向的编程语言
- 使用编译器指令交换类型安全以实现高性能

# 为什么使用F#？

为什么你应该考虑在下一个项目中使用F#

https://fsharpforfunandprofit.com/why-use-fsharp/

尽管F#非常适合科学或数据分析等专业领域，但它也是企业发展的绝佳选择。以下是您应该考虑在下一个项目中使用F#的五个很好的理由。

## 简洁

F#不会被花括号、分号等编码“噪音”所困扰。

得益于强大的类型推理系统，您几乎无需指定对象的类型。而且，与C#相比，解决同样的问题通常需要更少的代码行。

```F#
// one-liners
[1..100] |> List.sum |> printfn "sum=%d"

// no curly braces, semicolons or parentheses
let square x = x * x
let sq = square 42

// simple types in one line
type Person = {First:string; Last:string}

// complex types in a few lines
type Employee =
  | Worker of Person
  | Manager of Employee list

// type inference
let jdoe = {First="John"; Last="Doe"}
let worker = Worker jdoe
```

## 便利性

许多常见的编程任务在F#中要简单得多。这包括创建和使用复杂类型定义、执行列表处理、比较和相等、状态机等等。

由于函数是一级对象，因此通过创建具有其他函数作为参数的函数，或者组合现有函数以创建新功能，可以很容易地创建功能强大且可重用的代码。

```F#
// automatic equality and comparison
type Person = {First:string; Last:string}
let person1 = {First="john"; Last="Doe"}
let person2 = {First="john"; Last="Doe"}
printfn "Equal? %A"  (person1 = person2)

// easy IDisposable logic with "use" keyword
use reader = new StreamReader(..)

// easy composition of functions
let add2times3 = (+) 2 >> (*) 3
let result = add2times3 5
```

## 正确性

F# 有一个强大的类型系统，可以防止许多常见错误，如空引用异常。

默认情况下，值是不可变的，这可以防止出现大量错误。

此外，您通常可以使用类型系统本身对业务逻辑进行编码，这样实际上就不可能编写错误的代码或混淆度量单位，从而大大减少了对单元测试的需求。

```F#
// strict type checking
printfn "print string %s" 123 //compile error

// all values immutable by default
person1.First <- "new name"  //assignment error

// never have to check for nulls
let makeNewString str =
   //str can always be appended to safely
   let newString = str + " new!"
   newString

// embed business logic into types
emptyShoppingCart.remove   // compile error!

// units of measure
let distance = 10<m> + 10<ft> // error!
```

## 并发性

F#有许多内置库，可以在一次发生多件事时提供帮助。异步编程非常简单，并行性也是如此。F#还有一个内置的actor模型，对事件处理和函数式响应式编程有很好的支持。

当然，由于数据结构在默认情况下是不可变的，因此共享状态和避免锁要容易得多。

```F#
// easy async logic with "async" keyword
let! result = async {something}

// easy parallelism
Async.Parallel [ for i in 0..40 ->
      async { return fib(i) } ]

// message queues
MailboxProcessor.Start(fun inbox-> async{
	let! msg = inbox.Receive()
	printfn "message is: %s" msg
	})
```

## 完整性

虽然F#本质上是一种函数式语言，但它确实支持其他并非100%纯的风格，这使得它更容易与网站、数据库、其他应用程序等非纯世界进行交互。特别是，F#被设计为混合函数式/OO语言，因此它几乎可以做C#能做的一切。

当然，F# 是 .NET 生态系统其中的一部分，让您无缝访问所有第三方 .NET 库和工具。它可以在大多数平台上运行，包括Linux和智能手机（通过Mono）。

最后，它与Visual Studio很好地集成在一起，这意味着您可以获得一个具有 IntelliSense 支持的优秀 IDE、一个调试器和许多用于单元测试、源代码控制和其他开发任务的插件。或者在 Linux 上，您可以使用 MonoDevelop IDE。

```F#
// impure code when needed
let mutable counter = 0

// create C# compatible classes and interfaces
type IEnumerator<'a> =
    abstract member Current : 'a
    abstract MoveNext : unit -> bool

// extension methods
type System.Int32 with
    member this.IsEven = this % 2 = 0

let i=20
if i.IsEven then printfn "'%i' is even" i

// UI code
open System.Windows.Forms
let form = new Form(Width = 400, Height = 300,
   Visible = true, Text = "Hello World")
form.TopMost <- true
form.Click.Add (fun args -> printfn "clicked!")
form.Show()
```

## 想要了解更多细节吗？

如果你想了解更多信息，“为什么使用F#？”系列文章将更详细地介绍这些要点。

# “为什么使用 F#？”系列

https://fsharpforfunandprofit.com/series/why-use-fsharp/

本系列文章将为您介绍 F# 的主要功能，然后向您展示 F# 如何帮助您进行日常开发。

1. “为什么使用F#”系列介绍
   F的好处概述#
2. 60秒内完成F#语法
   关于如何阅读F#代码的快速概述
3. F#与C#的比较：一个简单的和
   其中，我们试图在不使用循环的情况下对1到N的平方进行求和
4. F#与C#的比较：排序
   其中我们看到F#比C#更具声明性，我们介绍了模式匹配。
5. F#与C#的比较：下载网页
   其中我们看到F#擅长回调，我们被介绍给了“use”关键字
6. 四个关键概念
   区分F#与标准命令式语言的概念
7. 简洁
   为什么简洁很重要？
8. 类型推断
   如何避免被复杂的类型语法分心
9. 低开销类型定义
   制造新型号不受处罚
10. 使用函数提取样板代码
    DRY原理的函数式方法
11. 将函数用作构建块
    函数组合和迷你语言使代码更具可读性
12. 简洁的图案匹配
    模式匹配可以在一个步骤中进行匹配和绑定
13. 便利性
    减少编程繁琐和样板代码的功能
14. 类型的开箱即用行为
    不可变和内置相等性，无需编码
15. 作为接口的函数
    使用函数时，OO 设计模式可能微不足道
16. 部分应用
    如何修复函数的一些参数
17. 活跃模式
    动态模式，实现强力匹配
18. 正确性
    如何编写“编译时单元测试”
19. 不可变性
    让你的代码可预测
20. 穷举的模式匹配
    一种确保正确性的强大技术
21. 使用类型系统确保代码正确
    在 F# 中，类型系统是你的朋友，而不是你的敌人
22. 工作示例：为正确性而设计
    如何使非法状态不具代表性
23. 并发性
    我们如何编写软件的下一次重大革命？
24. 异步编程
    用Async类封装后台任务
25. 消息和代理
    更容易思考并发性
26. 函数式反应式编程
    将事件转化为流
27. 完整性
    F# 是 .NET 生态系统整体的一部分
28. 与 .NET 库无缝互操作
    一些便于使用的函数式 .NET 库
29. C#能做的任何事情。。。
    F#中面向对象代码的旋风之旅
30. 为什么使用F#：结论

## [跳转系列独立 markdown](./FSharpForFunAndProfit翻译-“为什么使用 F#？”系列.md)



# 学习 F#

函数式编程语言需要一种不同的方法

https://fsharpforfunandprofit.com/learning-fsharp/

函数式语言与标准命令式语言非常不同，最初可能很难掌握窍门。本页提供了一些如何有效学习 F# 的技巧。

## 以初学者的身份学习 F#

如果你有 C# 和  Java 等语言的经验，你可能会发现，即使你不熟悉关键字或库，你也可以很好地理解用其他类似语言编写的源代码。这是因为所有命令式语言都使用相同的思维方式，一种语言的经验可以很容易地转移到另一种语言。

如果你和许多人一样，学习一门新编程语言的标准方法是找出如何实现你已经熟悉的概念。你可能会问“如何分配变量？”或“如何进行循环？”，有了这些答案，你就可以很快地完成一些基本的编程。

学习 F# 时，**你不应该试图把旧的命令式概念带到身边**。在纯函数式语言中，没有变量，没有循环，也没有对象！

是的，F# 是一种混合语言，确实支持这些概念。但如果你以初学者的心态开始，你会学得更快。

## 改变你的思维方式

重要的是要理解函数式编程不仅仅是风格上的差异；这是一种完全不同的编程思维方式，就像真正的面向对象编程（在Smalltalk中）也是一种与C等传统命令式语言不同的思维方式一样。

F# 确实允许非函数式风格，并且很容易保留你已经熟悉的习惯。你可能只是以一种非函数式的方式使用 F#，而不会真正改变你的思维方式，也不会意识到你错过了什么。为了充分利用 F#，并熟练掌握函数式编程，你必须从函数式而非命令式的角度思考。

到目前为止，你能做的最重要的事情就是花时间和精力准确理解 F# 的工作原理，尤其是涉及函数和类型系统的核心概念。因此，在开始认真编码之前，请反复阅读“函数式思维”和“理解 F# 类型”系列文章，尝试使用示例，并熟悉这些想法。如果你不了解函数和类型是如何工作的，那么你将很难提高效率。

## 注意事项

以下是一系列的注意事项，这将鼓励你进行函数式思考。这些起初会很难，但就像学习一门外语一样，你必须全身心投入，强迫自己像当地人一样说话。

- 初学者**根本**不要使用mutable关键字。在不依赖可变状态的情况下编写复杂函数将真正迫使你理解函数范式。
- 不要使用 for 循环或 if-then-else。使用模式匹配来测试布尔值和遍历列表。
- 不要使用“点符号”。与其“点入”对象，不如尝试对所有对象都使用函数。也就是说，写 `String.length "hello"`而不是 `"hello".Length`。这可能看起来像是额外的工作，但在使用管道和 `List.map` 等高阶函数时，这种工作方式是必不可少的。也不要写自己的方法！请参阅这篇文章了解详细信息。
- 因此，不要创建类。仅使用纯 F# 类型，如元组、记录和联合。
- 不要使用调试器。如果你依赖调试器来查找和修复不正确的代码，你会受到严重的打击。在F#中，你可能不会走那么远，因为编译器在很多方面都要严格得多。当然，没有工具可以“调试”编译器并逐步处理它。调试编译器错误的最佳工具是你的大脑，F# 迫使你使用它！

另一方面：

- 创建大量的“小类型”，尤其是联合类型。它们轻便易用，使用它们将有助于记录您的域模型并确保正确性。
- 请务必理解 `list` 和 `seq` 类型及其关联的库模块。像 `List.fold` 和 `List.map` 这样的函数非常强大。一旦你了解了如何使用它们，你就可以很好地理解高阶函数了。
- 一旦你理解了集合模块，尽量避免递归。递归很容易出错，而且很难确保它是正确的尾部递归。当你使用`List.fold` 时，你永远不会遇到这个问题。
- 尽可能多地使用管道（`|>`）和组合（`>>`）。这种风格比嵌套函数调用（如 `f(g(x))`）更地道
  了解部分应用程序的工作原理，并尝试适应无点（隐性）风格。
- 使用交互式窗口测试代码片段，逐步开发代码。如果你盲目地创建大量代码，然后试图一次编译所有代码，你可能会遇到许多痛苦且难以调试的编译错误。

## 故障排除

初学者会犯一些非常常见的错误，如果你对编译代码感到沮丧，请阅读“F#故障排除”页面。



# 故障排除F#

为什么我的代码无法编译？

https://fsharpforfunandprofit.com/troubleshooting-fsharp/

俗话说，“如果它编译成功，它就是正确的”，但仅仅试图让代码编译起来可能会非常令人沮丧！因此，本页致力于帮助您对 F# 代码进行故障排除。

我将首先介绍一些关于故障排除的一般建议，以及初学者最常见的一些错误。之后，我将详细描述每个常见的错误消息，并给出它们如何发生以及如何纠正的示例。

（跳到错误号）

## 故障排除的一般指南

到目前为止，你能做的最重要的事情就是花时间和精力准确理解 F# 的工作原理，尤其是涉及函数和类型系统的核心概念。因此，在开始认真编码之前，请反复阅读“函数式思维”和“理解F#类型”系列文章，尝试使用示例，并熟悉这些想法。如果你不了解函数和类型是如何工作的，那么编译器错误就没有任何意义。

如果你来自 C# 等命令式语言，你可能已经养成了一些坏习惯，因为你依赖调试器来查找和修复不正确的代码。在 F# 中，你可能不会走那么远，因为编译器在很多方面都要严格得多。当然，没有工具可以“调试”编译器并逐步处理它。调试编译器错误的最佳工具是你的大脑，F# 迫使你使用它！

尽管如此，初学者还是会犯一些非常常见的错误，我将很快介绍一下。

### 调用函数时不要使用括号

在 F# 中，空格是函数参数的标准分隔符。您很少需要使用括号，特别是在调用函数时不要使用括号。

```F#
let add x y = x + y
let result = add (1 2)  //wrong
    // error FS0003: This value is not a function and cannot be applied
let result = add 1 2    //correct
```

### 不要混淆具有多个参数的元组

如果它有逗号，它就是一个元组。元组是一个对象，而不是两个。因此，您将收到传递错误类型的参数或参数太少的错误。

```F#
addTwoParams (1,2)  // trying to pass a single tuple rather than two args
   // error FS0001: This expression was expected to have type
   //               int but here has type 'a * 'b
```

编译器将 `(1,2)` 视为泛型元组，并尝试将其传递给“addTwoParams”。然后它抱怨 addTwoParams 的第一个参数是一个 int，我们试图传递一个元组。

如果你试图向一个需要一个元组的函数传递两个参数，你会得到另一个模糊的错误。

```F#
addTuple 1 2   // trying to pass two args rather than one tuple
  // error FS0003: This value is not a function and cannot be applied
```

### 注意参数太少或太多

如果你向函数传递的参数太少（事实上“部分应用程序”是一个重要特性），F# 编译器不会抱怨，但如果你不了解发生了什么，你以后经常会遇到奇怪的“类型不匹配”错误。

同样，参数过多的错误通常是“此值不是函数”，而不是更直接的错误。

“printf”函数族在这方面非常严格。参数计数必须精确。

这是一个非常重要的话题——了解部分应用程序的工作原理至关重要。有关更详细的讨论，请参阅“函数式思维”系列。

### 使用分号作为列表分隔符

在 F# 需要显式分隔符的少数地方，如列表和记录，使用分号。逗号从未被使用过。（就像一个破记录一样，我会提醒你逗号是用于元组的）。

```F#
let list1 = [1,2,3]    // wrong! This is a ONE-element list containing
                       // a three-element tuple
let list1 = [1;2;3]    // correct

type Customer = {Name:string, Address: string}  // wrong
type Customer = {Name:string; Address: string}  // correct
```

### 不要用 ! 表示非或 != 表示不等

感叹号符号不是“not”运算符。它是可变引用的区分运算符。如果你错误地使用它，你会得到以下错误：

```F#
let y = true
let z = !y
// => error FS0001: This expression was expected to have
//    type 'a ref but here has type bool
```

正确的构造是使用“not”关键字。考虑 SQL 或 VB 语法，而不是 C 语法。

```F#
let y = true
let z = not y       //correct
```

对于“不相等”，使用“<>”，同样类似于SQL或VB。

```F#
let z = 1 <> 2      //correct
```

### 不要在赋值中使用 =

如果你使用可变值，赋值操作会写为“`<-`”。如果你使用等号符号，你甚至可能不会得到错误，只是一个意外的结果。

```F#
let mutable x = 1
x = x + 1          // returns false. x is not equal to x+1
x <- x + 1         // assigns x+1 to x
```

### 注意隐藏的标签字符

缩进规则非常简单，很容易掌握。但不允许使用制表符，只能使用空格。

```F#
let add x y =
{tab}x + y
// => error FS1161: TABs are not allowed in F# code
```

请确保将编辑器设置为将制表符转换为空格。注意你是否从其他地方粘贴代码。如果您确实遇到了一些代码的持续问题，请尝试删除空白并重新添加。

### 不要把简单值误认为函数值

如果你试图创建一个函数指针或委托，请注意不要意外创建一个已经计算过的简单值。

如果你想要一个可以重用的无参数函数，你需要显式传递一个单元参数，或者将其定义为lambda。

```F#
let reader = new System.IO.StringReader("hello")
let nextLineFn   =  reader.ReadLine()  //wrong
let nextLineFn() =  reader.ReadLine()  //correct
let nextLineFn   =  fun() -> reader.ReadLine()  //correct

let r = new System.Random()
let randomFn   =  r.Next()  //wrong
let randomFn() =  r.Next()  //correct
let randomFn   =  fun () -> r.Next()  //correct
```

有关无参数函数的更多讨论，请参阅“函数式思维”系列。

### 排除“信息不足”错误的提示

F# 编译器目前是一个从左到右的单程编译器，因此如果程序中的类型信息尚未解析，编译器将无法使用该信息。

这可能会导致许多错误，例如“FS0072:查找不确定类型的对象”和“FS0041:无法确定的唯一重载”。下面描述了针对每种特定情况的建议修复，但如果编译器抱怨缺少类型或信息不足，一些一般原则可能会有所帮助。这些指导方针是：

- 在使用之前定义东西（这包括确保文件以正确的顺序编译）
- 把“已知类型”的东西放在“未知类型”的前面。特别是，您可以重新排序管道和类似的链式函数，以便类型化对象排在第一位。
- 根据需要进行注释。一个常见的技巧是添加注释，直到一切正常，然后逐一删除注释，直到达到所需的最小值。

如果可能的话，尽量避免注释。它不仅不美观，而且使代码更加脆弱。如果没有明确的依赖关系，更改类型会容易得多。

## F# 编译器错误

以下是我认为值得记录的主要错误列表。我没有记录任何不言自明的错误，只有那些对初学者来说似乎很模糊的错误。

我将在未来继续在名单上添加，我欢迎任何添加建议。

- [FS0001: The type ‘X’ does not match the type ‘Y’](https://fsharpforfunandprofit.com/troubleshooting-fsharp/#FS0001)
- [FS0003: This value is not a function and cannot be applied](https://fsharpforfunandprofit.com/troubleshooting-fsharp/#FS0003)
- [FS0008: This runtime coercion or type test involves an indeterminate type](https://fsharpforfunandprofit.com/troubleshooting-fsharp/#FS0008)
- [FS0010: Unexpected identifier in binding](https://fsharpforfunandprofit.com/troubleshooting-fsharp/#FS0010a)
- [FS0010: Incomplete structured construct](https://fsharpforfunandprofit.com/troubleshooting-fsharp/#FS0010b)
- [FS0013: The static coercion from type X to Y involves an indeterminate type](https://fsharpforfunandprofit.com/troubleshooting-fsharp/#FS0013)
- [FS0020: This expression should have type ‘unit’](https://fsharpforfunandprofit.com/troubleshooting-fsharp/#FS0020)
- [FS0030: Value restriction](https://fsharpforfunandprofit.com/troubleshooting-fsharp/#FS0030)
- [FS0035: This construct is deprecated](https://fsharpforfunandprofit.com/troubleshooting-fsharp/#FS0035)
- [FS0039: The field, constructor or member X is not defined](https://fsharpforfunandprofit.com/troubleshooting-fsharp/#FS0039)
- [FS0041: A unique overload for could not be determined](https://fsharpforfunandprofit.com/troubleshooting-fsharp/#FS0041)
- [FS0049: Uppercase variable identifiers should not generally be used in patterns](https://fsharpforfunandprofit.com/troubleshooting-fsharp/#FS0049)
- [FS0072: Lookup on object of indeterminate type](https://fsharpforfunandprofit.com/troubleshooting-fsharp/#FS0072)
- [FS0588: Block following this ‘let’ is unfinished](https://fsharpforfunandprofit.com/troubleshooting-fsharp/#FS0588)

*（下面省略）*



# 函数式编程设计模式

我演讲中的幻灯片和视频

https://fsharpforfunandprofit.com/fppatterns/

本页包含我演讲“函数式编程设计模式”中的幻灯片和代码的链接。YouTube上的浏览量超过160000！

下面是简介：

> 在面向对象开发中，我们都熟悉策略模式和装饰器模式等设计模式，以及SOLID等设计原则。
>
> 函数式编程社区也有设计模式和原则。
>
> 本次演讲将概述其中一些，并展示FP设计在实践中的一些演示。

## 视频

以下是2014年伦敦NDC录制的视频（点击图片查看视频）。这是一个小时，我以最快的速度疾驰而过！

我在2015年1月的伦敦F#聚会上做了同样的演讲。这一次，谈话中有问题，我走得慢了一点。因此，它大约有两个小时！

## 幻灯片

以下是我在2014年伦敦NDC上使用的幻灯片：

函数式编程模式（NDC London 2014），摘自我在Slideshare上的幻灯片

> 如果你喜欢我用图片解释事物的方式，看看我的《使用函数式领域建模》一书！这是对领域驱动设计、类型建模和函数式编程的友好介绍。

## 推特风暴！

在我在BuildStuff 2014上做了这个演讲后，一张特别的幻灯片被转发了很多。

> 我喜欢@ScottWlaschin关于FP“模式”的演讲中的这张幻灯片pic.twitter.com/8UuwVqlelD
>
> --结，不是徒劳！（@jerodhaas）2014年11月21日

唉，它被亲FP和反FP的人误解了！

> @abt_programming这张幻灯片是FP偏执的一个开放和封闭的案例@斯科塔拉辛@jeoldhaas@sgoguen
>
> --耶胡达·卡茨（@wycats）2014年11月24日

就连鲍勃叔叔也写了一篇关于它的帖子！

哦，天哪！幽默不会脱离上下文。因此，在NDC和技能问题版本的演讲中，我决定更清楚地表明，我是在取笑生活在象牙塔里的FP人：

希望你喜欢这次谈话！

# “函数式思维”系列

https://fsharpforfunandprofit.com/series/thinking-functionally/

本系列文章将向您介绍函数式编程的基础知识——“函数式编程”的真正含义是什么，以及这种方法与面向对象或命令式编程有何不同。

1. 函数式思维：引言
   函数式编程的基础知识
2. 数学函数
   函数式编程背后的动力
3. 函数值和简单值
   绑定不赋值
4. 类型如何与函数协同工作
   理解类型符号
5. 货币
   将多参数函数分解为更小的单参数函数
6. 部分应用
   烘焙函数的某些参数
7. 函数关联性和组成
   从现有函数构建新函数
8. 定义函数
   Lambdas和更多
9. 函数签名
   函数签名可以让你对它的作用有所了解
10. 组织函数
    嵌套函数和模块
11. 将函数附加到类型
    以F#方式创建方法
12. 工作示例：基于堆栈的计算器
    使用组合子构建功能

## [跳转系列独立 markdown](./FSharpForFunAndProfit翻译-“函数式思维”系列.md)



# “用类型设计”系列

https://fsharpforfunandprofit.com/series/designing-with-types/

在本系列中，我们将探讨在设计过程中使用类型的一些方法。特别是，周到地使用类型可以使设计更加透明，同时提高正确性。

本系列将聚焦于设计的“微观层面”。也就是说，在单个类型和函数的最低级别上工作。更高层次的设计方法，以及使用函数式或面向对象风格的相关决策，将在另一个系列中讨论。

许多建议在 C# 或 Java 中也是可行的，但 F# 类型的轻量级特性意味着我们更有可能进行这种重构。

1. 用类型设计：简介
   使设计更加透明，提高正确性
2. 用类型设计：单箱联合类型
   为基本类型添加意义
3. 用类型设计：使非法状态无法表示
   以类型编码业务逻辑
4. 用类型设计：发现新概念
   深入了解该领域
5. 用类型设计：明确状态
   使用状态机确保正确性
6. 使用类型进行设计：约束字符串
   向基元类型添加更多语义信息
7. 使用类型进行设计：非字符串类型
   安全地处理整数和日期
8. 用类型设计：结论
   前后对比



## [跳转系列独立 markdown](./FSharpForFunAndProfit翻译-“用类型设计”系列.md)



# 面向铁路的程序设计

解释错误处理功能方法的幻灯片和视频

https://fsharpforfunandprofit.com/rop/

本页包含我演讲“面向铁路的编程”中的幻灯片和代码的链接。

以下是演讲的简介：

> 函数式编程中的许多例子都假设你总是走在“快乐的道路”上。但是，要创建一个健壮的现实世界应用程序，您必须处理验证、日志记录、网络和服务错误以及其他烦人的问题。
>
> 那么，你如何以一种干净的功能方式处理所有这些呢？
>
> 本次演讲将简要介绍这一主题，使用一个有趣且易于理解的铁路类比。

我也计划很快上传一些关于这些主题的帖子。同时，请参阅功能应用程序系列的配方，其中涵盖了类似的内容。

如果你想看到一些真实的代码，我在 Github 上创建了这个项目，它使用 ROP 方法比较了普通的 C# 和 F#

警告：这是一种有用的错误处理方法，但请不要走极端！请参阅我在“反对面向铁路的编程”上的帖子。

## 视频

我在 2014 年伦敦 NDC 上就这个话题发表了演讲（点击图片观看视频）

2014 年伦敦 NDC 视频

本次演讲的其他视频可从 2014 年奥斯陆 NDC 和 2014 年 Functional Programming eXchange 获得

## 幻灯片

函数式编程交换幻灯片，2014年3月14日

从我在 Slideshare 上的幻灯片看面向铁路的编程

幻灯片也可以在 Github 上找到。请随时向他们借！

> 如果你喜欢我用图片解释事物的方式，看看我的《领域建模使函数化》一书！这是对领域驱动设计、类型建模和函数式编程的友好介绍。

## 与 Either monad 和 Kleisli 组合的关系

任何读到这篇文章的 Haskeller 都会立即将这种方法识别为 `Either` 类型，专门用于在 Left 情况下使用自定义错误类型列表。在 Haskell 中，类似于：`type TwoTrack a b = Either [a] (b,[a])`

我当然不是想声称我发明了这种方法（尽管我确实声称这是一个愚蠢的类比）。那么，为什么我没有使用标准的 Haskell 术语呢？

首先，**这篇文章并不是试图成为 monad 教程**，而是专注于解决错误处理的具体问题。

大多数来 F# 的人都不熟悉 monads。我宁愿提出一种直观、不令人生畏、对许多人来说通常更直观的方法。

我坚信“从具体开始，转向抽象”的教学方法。根据我的经验，一旦你熟悉了这种特殊的方法，以后更高层次的抽象就更容易掌握。

其次，如果我声称我的带有 bind 的双轨类型是 monad，那我就错了——monad 比这更复杂，我只是不想在这里讨论 monad 定律。

第三，也是最重要的一点，`Either` 是一个过于笼统的概念。**我想展示一个食谱，而不是一个工具**。

例如，如果我想要一个制作面包的食谱，说“只用面粉和烤箱”并不是很有帮助。

因此，同样地，如果我想要一个处理错误的方法，说“只需用 bind 使用 Either”也没有多大帮助。

因此，在这种方法中，我将介绍一系列技术：

- 在 Either 的左侧和右侧使用自定义错误类型列表（而不是 `Either String a`）。
- “bind”（`>>=`）用于将一元函数集成到流水线中。
- Kleisli 组合（`>=>`）用于组合一元函数。
- “映射”（`fmap`）用于将非一元函数集成到流水线中。
- “tee”用于将单元函数集成到管道中（因为 F# 不使用 IO monad）。
- 从异常到错误案例的映射。
- `&&&` 用于并行组合一元函数（例如用于验证）。
- 自定义错误类型对域驱动设计的好处。
- 以及日志记录、域事件、补偿事务等的明显扩展。

我希望你能看到，这是一种比“只使用 Either monad”更全面的方法！

我在这里的目标是提供一个模板，它足够通用，几乎可以在所有情况下使用，但又足够受约束，以强制执行一致的风格。也就是说，基本上只有一种编写代码的方法。这对以后必须维护代码的人来说非常有帮助，因为他们可以立即理解代码是如何组合在一起的。

我并不是说这是唯一的方法。但我认为这种方法是一个良好的开端。

顺便说一句，即使在 Haskell 社区中，也没有一致的错误处理方法，这可能会让初学者感到困惑。我知道有很多关于单个错误处理技术的内容，但我不知道有哪个文档将所有这些工具全面地结合在一起。

## 我如何在自己的代码中使用它？

- 如果你想要一个与 NuGet 兼容的现成 F# 库，请查看 Chessie 项目。
- 如果你想看到一个使用这些技术的示例 web 服务，我在 GitHub 上创建了一个项目。
- 您还可以看到应用于 FizzBuzz 的 ROP 方法！

F# 没有类型类，所以你真的没有一种可重用的方法来做单子（尽管 FSharpX 库有一种有用的方法）。这意味着 Rop.fs 库从头开始定义了它的所有函数。（不过，在某些方面，这种隔离可能会有所帮助，因为根本没有外部依赖关系。）

## 进一步阅读

> “一个 bind 不等于一个单子(monad)”——亚里士多德

正如我上面提到的，我远离 monad 的一个原因是，正确定义 monad 不仅仅是实现“绑定（bind）”和“返回（return）”的问题。这是一种代数结构，需要遵守单子定律（在特定情况下，单子定律（monad laws）只是幺半群定律（monoid laws）），这是我在这次演讲中不想走的路。

但是，如果您对 Either 和 Kleisi 组合的更多细节感兴趣，这里有一些可能有用的链接：

- **一般来说的单子**。
  - Stack overflow 上关于单子的回答
  - Stack overflow 上 Eric Lippert 的回答
  - 图片中的单子
  - “你本可以发明单子”
  - Haskell 教程
  - nLab 的硬核定义
- **`Either` monad**
  - Haskell 学院
  - 《真实世界 Haskell》在错误处理方面的内容（半途而废）
  - LYAH 谈错误处理（中途）
- **Kleisli 范畴和组合**
  - 在 FPComplete 发表的文章
  - 由 Bartosz Milewski 发布的文章
  - nLab 的硬核定义
- **全面的错误处理方法**
  - 这篇文章的第 5 项
  - 我不知道还有其他方法涵盖了本次演讲中讨论的所有技术。如果你知道任何消息，请在评论中给我打电话，我会更新此页面。



# “基于属性的测试”系列

https://fsharpforfunandprofit.com/series/property-based-testing/

本系列文章将向您介绍基于属性的测试的基本原理：它与传统的基于示例的测试有何不同，为什么它很重要，以及如何在理论和实践中使用属性。

还有一个后续系列：《企业开发者从地狱归来》。

基于这些帖子，还有一个关于基于属性的测试的讨论。幻灯片和视频在这里。

1. 来自地狱的企业开发者
   发现恶意遵守基于属性的测试
2. 了解 FsCheck
   生成器、收缩器等
3. 为基于属性的测试选择属性
   或者，我想使用 PBT，但我永远想不出任何属性可以使用
4. 在实践中选择属性，第1部分
   列表函数的属性
5. 在实践中选择属性，第2部分
   罗马数字转换的性质
6. 在实践中选择属性，第3部分
   美元对象的属性



# “EDFH的回归”系列

https://fsharpforfunandprofit.com/series/return-of-the-edfh/

本系列文章是我之前关于基于属性的测试和来自地狱的企业开发人员的系列文章的后续。它重新审视了基于属性的测试的基本原理，以及如何使用 EDFH 来帮助设计有用和有效的测试。

1. 企业开发者从地狱归来
   更多的恶意合规性，更多的基于属性的测试
2. 为基于属性的测试生成有趣的输入
   以及如何对它们进行分类
3. EDFH 再次被击败



# 观察乌龟的十三种方法

API、依赖项注入、状态 monad 等示例！
05十二月2015 这篇文章已经超过3岁了

https://fsharpforfunandprofit.com/posts/13-ways-of-looking-at-a-turtle/

*更新：[我关于这个话题的演讲幻灯片和视频](https://fsharpforfunandprofit.com/turtle/)*

> 这篇文章是 2015 年英语 F# 降临日历项目的一部分。查看那里的所有其他精彩帖子！特别感谢 Sergey Tihon 组织这次活动。

不久前，我在讨论如何实现一个简单的乌龟图形系统，我突然想到，由于乌龟的要求非常简单且众所周知，这将为演示一系列不同的技术奠定坚实的基础。

因此，在这篇由两部分组成的巨型文章中，我将把乌龟模型扩展到极限，同时演示以下内容：部分应用、成功/失败结果的验证、“提升”的概念、带有消息队列的代理、依赖注入、状态单子、事件源、流处理，最后是自定义解释器！

闲话少说，我在此介绍 13 种实现乌龟的不同方法：

- 方式1。一种基本的面向对象方法，其中我们创建了一个具有可变状态的类。
- 方式2。一种基本的函数方法，其中我们创建了一个具有不可变状态的函数模块。
- 方式3。具有面向对象核心的 API，其中我们创建了一个面向对象的 API，该 API 调用有状态核心类。
- 方式4。一个带有函数式核心的 API，其中我们创建了一个使用无状态核心函数的有状态 API。
- 方式5。代理前面的 API，其中我们创建了一个 API，该 API 使用消息队列与代理通信。
- 方式6。使用接口的依赖注入，其中我们使用接口或函数记录将实现与 API 解耦。
- 方式7。使用函数的依赖注入，其中我们通过传递函数参数将实现与 API 解耦。
- 方式8。使用状态单子进行批处理，其中我们创建了一个特殊的“海龟工作流”计算表达式来为我们跟踪状态。
- 方式9。使用命令对象进行批处理，其中我们创建一个类型来表示 turtle 命令，然后一次处理一系列命令。
- 间奏：有意识地使用数据类型解耦。关于使用数据与接口进行解耦的几点说明。
- 方式10。事件溯源，其中状态是根据过去的事件列表构建的。
- 方式11。函数式回溯编程（流处理），其中业务逻辑基于对早期事件的反应。
- 第五集：乌龟反击战，乌龟 API 发生变化，一些命令可能会失败。
- 方式12。一元控制流，其中我们根据早期命令的结果在海龟工作流中做出决策。
- 方式13。一个 turtle 解释器，其中我们将 turtle 编程与 turtle 实现完全解耦，几乎遇到了 free monad。
- 回顾所有使用的技术。

扩展版有 2 种奖励方式：

- 方式14。抽象数据海龟，其中我们使用抽象数据类型封装海龟实现的细节。
- 方式15。基于能力的 Turtle，我们根据乌龟的当前状态控制客户可以使用哪些乌龟功能。

这篇文章的所有源代码都可以在 github 上找到。

## 海龟的要求

乌龟支持四条指令：

- 沿当前方向移动一段距离。
- 顺时针或逆时针转动一定角度。
- 把笔放下或举起。当笔放下时，移动乌龟会画一条线。
- 设置钢笔颜色（黑色、蓝色或红色之一）。

这些要求自然会导致某种“海龟接口”，如下所示：

- `Move aDistance`
- `Turn anAngle`
- `PenUp`
- `PenDown`
- `SetColor aColor`

以下所有实现都将基于此接口或其变体。

请注意，乌龟必须将这些指令转换为在画布或其他图形上下文中绘制线条。因此，实现可能需要以某种方式跟踪乌龟的位置和当前状态。

## 通用代码

在开始实现之前，让我们先处理一些常见代码。

首先，我们需要一些类型来表示距离、角度、笔状态和笔颜色。

```F#
/// An alias for a float
type Distance = float

/// Use a unit of measure to make it clear that the angle is in degrees, not radians
type [<Measure>] Degrees

/// An alias for a float of Degrees
type Angle  = float<Degrees>

/// Enumeration of available pen states
type PenState = Up | Down

/// Enumeration of available pen colors
type PenColor = Black | Red | Blue
```

我们还需要一个类型来表示乌龟的位置：

```F#
/// A structure to store the (x,y) coordinates
type Position = {x:float; y:float}
```

我们还需要一个辅助函数，根据以特定角度移动特定距离来计算新位置：

```F#
// round a float to two places to make it easier to read
let round2 (flt:float) = Math.Round(flt,2)

/// calculate a new position from the current position given an angle and a distance
let calcNewPosition (distance:Distance) (angle:Angle) currentPos =
    // Convert degrees to radians with 180.0 degrees = 1 pi radian
    let angleInRads = angle * (Math.PI/180.0) * 1.0<1/Degrees>
    // current pos
    let x0 = currentPos.x
    let y0 = currentPos.y
    // new pos
    let x1 = x0 + (distance * cos angleInRads)
    let y1 = y0 + (distance * sin angleInRads)
    // return a new Position
    {x=round2 x1; y=round2 y1}
```

让我们定义一只乌龟的初始状态：

```F#
/// Default initial state
let initialPosition,initialColor,initialPenState =
    {x=0.0; y=0.0}, Black, Down
```

还有一个假装在画布上画线的助手：

```F#
let dummyDrawLine log oldPos newPos color =
    // for now just log it
    log (sprintf "...Draw line from (%0.1f,%0.1f) to (%0.1f,%0.1f) using %A" oldPos.x oldPos.y newPos.x newPos.y color)
```

现在，我们已经为首次实现做好了准备！

## 1：基本 OO——一个具有可变状态的类

在第一个设计中，我们将使用面向对象的方法，用一个简单的类来表示乌龟。

- 状态将存储在可变的本地字段（`currentPosition`、`currentAngle` 等）中。
- 我们将注入一个日志函数 `log`，以便我们可以监视发生了什么。

这是完整的代码，应该是不言自明的：

```F#
type Turtle(log) =

    let mutable currentPosition = initialPosition
    let mutable currentAngle = 0.0<Degrees>
    let mutable currentColor = initialColor
    let mutable currentPenState = initialPenState

    member this.Move(distance) =
        log (sprintf "Move %0.1f" distance)
        // calculate new position
        let newPosition = calcNewPosition distance currentAngle currentPosition
        // draw line if needed
        if currentPenState = Down then
            dummyDrawLine log currentPosition newPosition currentColor
        // update the state
        currentPosition <- newPosition

    member this.Turn(angle) =
        log (sprintf "Turn %0.1f" angle)
        // calculate new angle
        let newAngle = (currentAngle + angle) % 360.0<Degrees>
        // update the state
        currentAngle <- newAngle

    member this.PenUp() =
        log "Pen up"
        currentPenState <- Up

    member this.PenDown() =
        log "Pen down"
        currentPenState <- Down

    member this.SetColor(color) =
        log (sprintf "SetColor %A" color)
        currentColor <- color
```

### 调用乌龟对象

客户端代码实例化乌龟并直接与它对话：

```F#
/// Function to log a message
let log message =
    printfn "%s" message

let drawTriangle() =
    let turtle = Turtle(log)
    turtle.Move 100.0
    turtle.Turn 120.0<Degrees>
    turtle.Move 100.0
    turtle.Turn 120.0<Degrees>
    turtle.Move 100.0
    turtle.Turn 120.0<Degrees>
    // back home at (0,0) with angle 0
```

`drawTriangle()` 的记录输出为：

```
Move 100.0
...Draw line from (0.0,0.0) to (100.0,0.0) using Black
Turn 120.0
Move 100.0
...Draw line from (100.0,0.0) to (50.0,86.6) using Black
Turn 120.0
Move 100.0
...Draw line from (50.0,86.6) to (0.0,0.0) using Black
Turn 120.0
```

同样，以下是绘制多边形的代码：

```F#
let drawPolygon n =
    let angle = 180.0 - (360.0/float n)
    let angleDegrees = angle * 1.0<Degrees>
    let turtle = Turtle(log)

    // define a function that draws one side
    let drawOneSide() =
        turtle.Move 100.0
        turtle.Turn angleDegrees

    // repeat for all sides
    for i in [1..n] do
        drawOneSide()
```

请注意，`drawOneSide()` 不返回任何东西——所有的代码都是命令式和有状态的。将其与下一个示例中的代码进行比较，该示例采用纯函数方法。

### 优点和缺点

那么，这种简单方法的优缺点是什么？

*优势*

- 它很容易实现和理解。

*缺点*

- 有状态代码更难测试。在测试之前，我们必须将对象置于已知状态，在这种情况下这很简单，但对于更复杂的对象来说可能会冗长且容易出错。
- 客户端耦合到特定的实现。这里没有接口！我们稍后将探讨如何使用接口。

此版本的源代码可在此处（turtle 类）和此处（客户端）获得。

## 2：Basic FP - 一个具有不可变状态的函数模块

下一个设计将使用纯粹的功能方法。定义了一个不可变的 `TurtleState`，然后各种 turtle 函数接受一个状态作为输入，并返回一个新的状态作为输出。

在这种方法中，客户端负责跟踪当前状态并将其传递给下一个函数调用。



以下是TurtleState的定义和初始状态的值：

```F#
module Turtle =

    type TurtleState = {
        position : Position
        angle : float<Degrees>
        color : PenColor
        penState : PenState
    }

    let initialTurtleState = {
        position = initialPosition
        angle = 0.0<Degrees>
        color = initialColor
        penState = initialPenState
    }
```

以下是“api”函数，所有这些函数都接受一个状态参数并返回一个新的状态：

```F#
module Turtle =

    // [state type snipped]

    let move log distance state =
        log (sprintf "Move %0.1f" distance)
        // calculate new position
        let newPosition = calcNewPosition distance state.angle state.position
        // draw line if needed
        if state.penState = Down then
            dummyDrawLine log state.position newPosition state.color
        // update the state
        {state with position = newPosition}

    let turn log angle state =
        log (sprintf "Turn %0.1f" angle)
        // calculate new angle
        let newAngle = (state.angle + angle) % 360.0<Degrees>
        // update the state
        {state with angle = newAngle}

    let penUp log state =
        log "Pen up"
        {state with penState = Up}

    let penDown log state =
        log "Pen down"
        {state with penState = Down}

    let setColor log color state =
        log (sprintf "SetColor %A" color)
        {state with color = color}
```

请注意，`state` 始终是最后一个参数——这使得使用“管道”习惯用法更容易。

### 使用乌龟函数

客户端现在每次都必须将日志函数和状态传递给每个函数！

我们可以通过使用部分应用程序创建带有内置记录器的函数的新版本来消除传递日志函数的需要：

```F#
/// Function to log a message
let log message =
    printfn "%s" message

// versions with log baked in (via partial application)
let move = Turtle.move log
let turn = Turtle.turn log
let penDown = Turtle.penDown log
let penUp = Turtle.penUp log
let setColor = Turtle.setColor log
```

使用这些更简单的版本，客户端可以以自然的方式传输状态：

```F#
let drawTriangle() =
    Turtle.initialTurtleState
    |> move 100.0
    |> turn 120.0<Degrees>
    |> move 100.0
    |> turn 120.0<Degrees>
    |> move 100.0
    |> turn 120.0<Degrees>
    // back home at (0,0) with angle 0
```

当涉及到绘制多边形时，它有点复杂，因为我们必须通过每边的重复来“折叠”状态：

```F#
let drawPolygon n =
    let angle = 180.0 - (360.0/float n)
    let angleDegrees = angle * 1.0<Degrees>

    // define a function that draws one side
    let oneSide state sideNumber =
        state
        |> move 100.0
        |> turn angleDegrees

    // repeat for all sides
    [1..n]
    |> List.fold oneSide Turtle.initialTurtleState
```

### 优点和缺点

这种纯功能方法的优缺点是什么？

*优势*

- 同样，它很容易实现和理解。
- 无状态函数更容易测试。我们总是提供当前状态作为输入，因此不需要设置来使对象进入已知状态。
- 因为没有全局状态，所以这些函数是模块化的，可以在其他环境中重用（正如我们将在本文稍后看到的）。

*缺点*

- 如前所述，客户端与特定的实现相耦合。
- 客户端必须跟踪状态（但本文稍后将介绍一些使这更容易的解决方案）。

此版本的源代码可在此处（turtle函数）和此处（客户端）获得。

## 3：具有面向对象核心的 API

让我们使用 API 对实现隐藏客户端！

在这种情况下，API 将是基于字符串的，带有文本命令，如“`move 100`”或“`turn 90`”。API 必须验证这些命令，并将它们转换为对 turtle 的方法调用（我们将再次使用有状态 `Turtle` 类的 OO 方法）。



如果命令无效，API 必须将其指示给客户端。由于我们使用的是 OO 方法，我们将通过抛出包含字符串的 `TurtleApiException` 来实现这一点，如下所示。

```F#
exception TurtleApiException of string
```

接下来，我们需要一些函数来验证命令文本：

```F#
// convert the distance parameter to a float, or throw an exception
let validateDistance distanceStr =
    try
        float distanceStr
    with
    | ex ->
        let msg = sprintf "Invalid distance '%s' [%s]" distanceStr  ex.Message
        raise (TurtleApiException msg)

// convert the angle parameter to a float<Degrees>, or throw an exception
let validateAngle angleStr =
    try
        (float angleStr) * 1.0<Degrees>
    with
    | ex ->
        let msg = sprintf "Invalid angle '%s' [%s]" angleStr ex.Message
        raise (TurtleApiException msg)

// convert the color parameter to a PenColor, or throw an exception
let validateColor colorStr =
    match colorStr with
    | "Black" -> Black
    | "Blue" -> Blue
    | "Red" -> Red
    | _ ->
        let msg = sprintf "Color '%s' is not recognized" colorStr
        raise (TurtleApiException msg)
```

有了这些，我们就可以创建 API。

解析命令文本的逻辑是将命令文本拆分为标记，然后将第一个标记与“`move`”、“`turn`”等进行匹配。

代码如下：

```F#
type TurtleApi() =

    let turtle = Turtle(log)

    member this.Exec (commandStr:string) =
        let tokens = commandStr.Split(' ') |> List.ofArray |> List.map trimString
        match tokens with
        | [ "Move"; distanceStr ] ->
            let distance = validateDistance distanceStr
            turtle.Move distance
        | [ "Turn"; angleStr ] ->
            let angle = validateAngle angleStr
            turtle.Turn angle
        | [ "Pen"; "Up" ] ->
            turtle.PenUp()
        | [ "Pen"; "Down" ] ->
            turtle.PenDown()
        | [ "SetColor"; colorStr ] ->
            let color = validateColor colorStr
            turtle.SetColor color
        | _ ->
            let msg = sprintf "Instruction '%s' is not recognized" commandStr
            raise (TurtleApiException msg)
```

### 使用 API

以下是使用 `TurtleApi` 类实现 `drawPolygon` 的方法：

```F#
let drawPolygon n =
    let angle = 180.0 - (360.0/float n)
    let api = TurtleApi()

    // define a function that draws one side
    let drawOneSide() =
        api.Exec "Move 100.0"
        api.Exec (sprintf "Turn %f" angle)

    // repeat for all sides
    for i in [1..n] do
        drawOneSide()
```

您可以看到，代码与早期的 OO 版本非常相似，使用的是直接调用 `turtle.Move 100.0`，替换为间接 API 调用 `api.Exec "Move 100.0"`。

现在，如果我们使用错误的命令如  `api.Exec "Move bad"`，如下所示：

```F#
let triggerError() =
    let api = TurtleApi()
    api.Exec "Move bad"
```

则抛出预期的异常：

```
Exception of type 'TurtleApiException' was thrown.
```

### 优点和缺点

像这样的API层有哪些优点和缺点？

- 海龟实现现在对客户端隐藏。
- 服务边界上的 API 支持验证，并可以扩展到支持监控、内部路由、负载平衡等。

*缺点*

- API 耦合到特定的实现，即使客户端不是。
- 该系统非常有状态。尽管客户端不知道 API 背后的实现，但客户端仍然通过共享状态间接耦合到内核，这反过来会使测试更加困难。

此版本的源代码可在此处获得。

## 4：具有函数式核心的 API

这种情况的另一种方法是使用混合设计，其中应用程序的核心由纯函数组成，而边界是命令式和有状态的。

Gary Bernhardt 将这种方法命名为“函数式核心/命令式外壳”。

应用到我们的 API 示例中，API 层仅使用纯乌龟函数，但 API 层通过存储可变乌龟状态来管理状态（而不是客户端）。

此外，为了更函数式，如果命令文本无效，API 将不会抛出异常，而是返回具有 `Success` 和 `Failure` 情况的 `Result` 值，其中 `Failure` 情况用于任何错误。（有关此技术的更深入讨论，请参阅我关于错误处理的函数方法的演讲）。



让我们从实现 API 类开始。这一次，它包含了一个 `mutable` 乌龟状态：

```F#
type TurtleApi() =

    let mutable state = initialTurtleState

    /// Update the mutable state value
    let updateState newState =
        state <- newState
```

验证函数不再抛出异常，而是返回 `Success` 或 `Failure`：

```F#
let validateDistance distanceStr =
    try
        Success (float distanceStr)
    with
    | ex ->
        Failure (InvalidDistance distanceStr)
```

错误案例以自己的类型记录：

```F#
type ErrorMessage =
    | InvalidDistance of string
    | InvalidAngle of string
    | InvalidColor of string
    | InvalidCommand of string
```

现在，由于验证函数现在返回 `Result<Distance>` 而不是“原始”距离，因此需要将 `move` 函数提升到 `Results` 世界，就像当前状态一样。

在处理 `Result`s 时，我们将使用三个函数：`returnR`、`mapR` 和 `lift2R`。

- `returnR` 将“正常”值转换为 Results 世界中的值：
- `mapR` 将 Results 世界中的“正常”单参数函数转换为单参数函数：
- `lift2R` 将“正常”双参数函数转换为 Results 世界中的双参数函数：


例如，使用这些辅助函数，我们可以将正常的 `move` 函数转换为 Results 世界中的函数：

- 距离参数已在 `Result` 世界中
- 使用 `returnR` 将状态参数提升到 `Result` 世界中
- 使用 `lift2R` 将 `move` 函数提升到 `Result` 世界

```F#
// lift current state to Result
let stateR = returnR state

// get the distance as a Result
let distanceR = validateDistance distanceStr

// call "move" lifted to the world of Results
lift2R move distanceR stateR
```

*（有关将功能提升到 `Result` world 的更多详细信息，请参阅关于“提升”的帖子）*

以下是 `Exec` 的完整代码：

```F#
/// Execute the command string, and return a Result
/// Exec : commandStr:string -> Result<unit,ErrorMessage>
member this.Exec (commandStr:string) =
    let tokens = commandStr.Split(' ') |> List.ofArray |> List.map trimString

    // lift current state to Result
    let stateR = returnR state

    // calculate the new state
    let newStateR =
        match tokens with
        | [ "Move"; distanceStr ] ->
            // get the distance as a Result
            let distanceR = validateDistance distanceStr

            // call "move" lifted to the world of Results
            lift2R move distanceR stateR

        | [ "Turn"; angleStr ] ->
            let angleR = validateAngle angleStr
            lift2R turn angleR stateR

        | [ "Pen"; "Up" ] ->
            returnR (penUp state)

        | [ "Pen"; "Down" ] ->
            returnR (penDown state)

        | [ "SetColor"; colorStr ] ->
            let colorR = validateColor colorStr
            lift2R setColor colorR stateR

        | _ ->
            Failure (InvalidCommand commandStr)

    // Lift `updateState` into the world of Results and
    // call it with the new state.
    mapR updateState newStateR

    // Return the final result (output of updateState)
```

### 使用 API

API 返回一个 `Result`，因此客户端不能再按顺序调用每个函数，因为我们需要处理来自调用的任何错误并放弃其余步骤。

为了让我们的生活更轻松，我们将使用 `result` 计算表达式（或工作流）来链接调用，并保留 OO 版本的命令式“感觉”。

```F#
let drawTriangle() =
    let api = TurtleApi()
    result {
        do! api.Exec "Move 100"
        do! api.Exec "Turn 120"
        do! api.Exec "Move 100"
        do! api.Exec "Turn 120"
        do! api.Exec "Move 100"
        do! api.Exec "Turn 120"
        }
```

*`result` 计算表达式的源代码可以在[这里](https://github.com/swlaschin/13-ways-of-looking-at-a-turtle/blob/master/Common.fsx#L70)找到。*

同样，对于 `drawPolygon` 代码，我们可以创建一个助手来绘制一侧，然后在 `result` 表达式中调用它 `n` 次。

```F#
let drawPolygon n =
    let angle = 180.0 - (360.0/float n)
    let api = TurtleApi()

    // define a function that draws one side
    let drawOneSide() = result {
        do! api.Exec "Move 100.0"
        do! api.Exec (sprintf "Turn %f" angle)
        }

    // repeat for all sides
    result {
        for i in [1..n] do
            do! drawOneSide()
    }
```

该代码看起来是命令式的，但实际上纯粹是函数式的，因为返回的 `Result` 值由 `result` 工作流透明地处理。

### 优点和缺点

*优势*

- 与 API 的 OO 版本相同——乌龟实现对客户端隐藏，可以进行验证等。
- 系统中唯一有状态的部分位于边界处。核心是无状态的，这使得测试更容易。

*缺点*

- API 仍然耦合到特定的实现。

此版本的源代码在这里（api 助手函数）和这里（api 和客户端）都可用。

## 5：代理面前的API

在该设计中，API 层通过消息队列与 `TurtleAgent` 通信，客户端与 API 层进行通信，与以前一样。



API（或任何地方）中没有可变项。`TurtleAgent` 通过将当前状态作为参数存储在递归消息处理循环中来管理状态。

现在，由于 `TurtleAgent` 有一个类型化的消息队列，其中所有消息都是相同的类型，我们必须将所有可能的命令组合成一个可区分的联合类型（`TurtleCommand`）。

```F#
type TurtleCommand =
    | Move of Distance
    | Turn of Angle
    | PenUp
    | PenDown
    | SetColor of PenColor
```

代理实现类似于前面的实现，但我们现在不对传入的命令进行模式匹配，以决定调用哪个函数，而不是直接公开 turtle 函数：

```F#
type TurtleAgent() =

    /// Function to log a message
    let log message =
        printfn "%s" message

    // logged versions
    let move = Turtle.move log
    let turn = Turtle.turn log
    let penDown = Turtle.penDown log
    let penUp = Turtle.penUp log
    let setColor = Turtle.setColor log

    let mailboxProc = MailboxProcessor.Start(fun inbox ->
        let rec loop turtleState = async {
            // read a command message from the queue
            let! command = inbox.Receive()
            // create a new state from handling the message
            let newState =
                match command with
                | Move distance ->
                    move distance turtleState
                | Turn angle ->
                    turn angle turtleState
                | PenUp ->
                    penUp turtleState
                | PenDown ->
                    penDown turtleState
                | SetColor color ->
                    setColor color turtleState
            return! loop newState
            }
        loop Turtle.initialTurtleState )

    // expose the queue externally
    member this.Post(command) =
        mailboxProc.Post command
```

### 向代理发送命令

API 通过构造 `TurtleCommand` 并将其发布到代理的队列来调用代理。

这一次，而不是使用之前的“提升” `move` 命令的方法：

```F#
let stateR = returnR state
let distanceR = validateDistance distanceStr
lift2R move distanceR stateR
```

我们将使用 `result` 计算表达式，因此上面的代码看起来像这样：

```F#
result {
    let! distance = validateDistance distanceStr
    move distance state
    }
```

在代理实现中，我们没有调用 `move` 命令，而是创建了 `command` 类型的 `Move` 案例，因此代码如下：

```F#
result {
    let! distance = validateDistance distanceStr
    let command = Move distance
    turtleAgent.Post command
    }
```

以下是完整的代码：

```F#
member this.Exec (commandStr:string) =
    let tokens = commandStr.Split(' ') |> List.ofArray |> List.map trimString

    // calculate the new state
    let result =
        match tokens with
        | [ "Move"; distanceStr ] -> result {
            let! distance = validateDistance distanceStr
            let command = Move distance
            turtleAgent.Post command
            }

        | [ "Turn"; angleStr ] -> result {
            let! angle = validateAngle angleStr
            let command = Turn angle
            turtleAgent.Post command
            }

        | [ "Pen"; "Up" ] -> result {
            let command = PenUp
            turtleAgent.Post command
            }

        | [ "Pen"; "Down" ] -> result {
            let command = PenDown
            turtleAgent.Post command
            }

        | [ "SetColor"; colorStr ] -> result {
            let! color = validateColor colorStr
            let command = SetColor color
            turtleAgent.Post command
            }

        | _ ->
            Failure (InvalidCommand commandStr)

    // return any errors
    result
```

### Agent 方法的优缺点

*优势*

- 一种不用锁保护可变状态的好方法。
- API 通过消息队列与特定实现解耦。`TurtleCommand` 充当一种将队列两端解耦的协议。
- 海龟代理自然是异步的。
- 代理可以很容易地水平缩放。

缺点

- 代理是有状态的，与有状态对象有同样的问题：
  - 更难对代码进行推理。
  - 测试更难。
  - 在参与者之间创建一个复杂的依赖关系网太容易了。
- 代理的稳健实现可能会变得相当复杂，因为您可能需要对监视者、心跳、背压等的支持。

此版本的源代码可在此处获得。

## 6：使用接口进行依赖注入

到目前为止，所有的实现都与乌龟函数的特定实现绑定在一起，但代理版本除外，在代理版本中，API 通过队列间接通信。

因此，让我们来看看将 API 与实现脱钩的一些方法。

### 设计面向对象风格的接口

我们将从经典的 OO 解耦实现方式开始：使用接口。

将这种方法应用于乌龟域，我们可以看到我们的 API 层将需要与 `ITurtle` 接口通信，而不是与特定的乌龟实现通信。客户端稍后通过 API 的构造函数注入乌龟实现。

以下是接口定义：

```F#
type ITurtle =
    abstract Move : Distance -> unit
    abstract Turn : Angle -> unit
    abstract PenUp : unit -> unit
    abstract PenDown : unit -> unit
    abstract SetColor : PenColor -> unit
```

请注意，这些函数中有很多 `unit`。函数签名中的 `unit` 意味着副作用，事实上，`TurtleState` 并没有在任何地方使用，因为这是一种基于 OO 的方法，可变状态被封装在对象中。

接下来，我们需要通过在 `TurtleApi` 的构造函数中注入接口来更改 API 层以使用该接口。除此之外，API 代码的其余部分保持不变，如下面的代码片段所示：

```F#
type TurtleApi(turtle: ITurtle) =

    // other code

    member this.Exec (commandStr:string) =
        let tokens = commandStr.Split(' ') |> List.ofArray |> List.map trimString
        match tokens with
        | [ "Move"; distanceStr ] ->
            let distance = validateDistance distanceStr
            turtle.Move distance
        | [ "Turn"; angleStr ] ->
            let angle = validateAngle angleStr
            turtle.Turn angle
        // etc
```

### 创建 OO 接口的一些实现

现在，让我们创建并测试一些实现。

第一个实现将被称为 `normalSize`，并且将是原始实现。第二个将被称为 `halfSize`，并将所有距离减半。

对于 `normalSize`，我们可以返回并修改原始的 `Turtle` 类以支持 `ITurtle` 接口。但我讨厌改变工作代码！相反，我们可以在原始的 `Turtle` 类周围创建一个“代理”包装器，代理在其中实现新的接口。

在某些语言中，创建代理包装器可能很冗长，但在 F# 中，您可以使用 object 表达式快速实现接口：

```F#
let normalSize() =
    let log = printfn "%s"
    let turtle = Turtle(log)

    // return an interface wrapped around the Turtle
    {new ITurtle with
        member this.Move dist = turtle.Move dist
        member this.Turn angle = turtle.Turn angle
        member this.PenUp() = turtle.PenUp()
        member this.PenDown() = turtle.PenDown()
        member this.SetColor color = turtle.SetColor color
    }
```

为了创建 `halfSize` 版本，我们做了同样的事情，但拦截了 `Move` 和将距离参数减半的调用：

```F#
let halfSize() =
    let normalSize = normalSize()

    // return a decorated interface
    {new ITurtle with
        member this.Move dist = normalSize.Move (dist/2.0)   // halved!!
        member this.Turn angle = normalSize.Turn angle
        member this.PenUp() = normalSize.PenUp()
        member this.PenDown() = normalSize.PenDown()
        member this.SetColor color = normalSize.SetColor color
    }
```

这实际上是工作中的“装饰器”模式：我们将 `normalSize` 包装在一个具有相同接口的代理中，然后更改其中一些方法的行为，同时传递其他方法，尽管它们没有受到影响。

### 注入依赖关系，OO 风格

现在让我们看看将依赖项注入 API 的客户端代码。

首先，一些绘制三角形的代码，其中传递了 `TurtleApi`：

```F#
let drawTriangle(api:TurtleApi) =
    api.Exec "Move 100"
    api.Exec "Turn 120"
    api.Exec "Move 100"
    api.Exec "Turn 120"
    api.Exec "Move 100"
    api.Exec "Turn 120"
```

现在让我们尝试通过用普通接口实例化 API 对象来绘制三角形：

```F#
let iTurtle = normalSize()   // an ITurtle type
let api = TurtleApi(iTurtle)
drawTriangle(api)
```

显然，在真实的系统中，依赖注入将在调用站点之外发生，使用 IoC 容器或类似容器。

如果我们运行它，`drawTriangle` 的输出与之前一样：

```
Move 100.0
...Draw line from (0.0,0.0) to (100.0,0.0) using Black
Turn 120.0
Move 100.0
...Draw line from (100.0,0.0) to (50.0,86.6) using Black
Turn 120.0
Move 100.0
...Draw line from (50.0,86.6) to (0.0,0.0) using Black
Turn 120.0
```

现在有了半尺寸的接口。。

```F#
let iTurtle = halfSize()
let api = TurtleApi(iTurtle)
drawTriangle(api)
```

…正如我们所希望的那样，尺寸是原来的一半！

```
Move 50.0
...Draw line from (0.0,0.0) to (50.0,0.0) using Black
Turn 120.0
Move 50.0
...Draw line from (50.0,0.0) to (25.0,43.3) using Black
Turn 120.0
Move 50.0
...Draw line from (25.0,43.3) to (0.0,0.0) using Black
Turn 120.0
```

### 设计接口，函数式风格

在纯 FP 世界中，OO 风格的接口是不存在的。但是，您可以通过使用包含函数的记录来模拟它们，接口中的每个方法对应一个函数。

因此，让我们创建一个依赖项注入的替代版本，这一次API层将使用函数记录，而不是接口。

函数记录是普通记录，但字段的类型是函数类型。以下是我们将使用的定义：

```F#
type TurtleFunctions = {
    move : Distance -> TurtleState -> TurtleState
    turn : Angle -> TurtleState -> TurtleState
    penUp : TurtleState -> TurtleState
    penDown : TurtleState -> TurtleState
    setColor : PenColor -> TurtleState -> TurtleState
    }
```

请注意，与 OO 版本不同，这些函数签名中没有 `unit`。相反，`TurtleState` 被明确地传入并返回。

还要注意，也没有日志记录。创建记录时，日志记录方法将内置到函数中。

`TurtleApi` 构造函数现在采用 `TurtleFunctions` 记录，而不是 `ITurtle`，但由于这些函数是纯函数，API 需要使用 `mutable` 字段再次管理状态。

```F#
type TurtleApi(turtleFunctions: TurtleFunctions) =

    let mutable state = initialTurtleState
```

主 `Exec` 方法的实现与我们之前看到的非常相似，但有以下区别：

- 函数从记录中提取（例如 `turtleFunctions.move`）。
- 所有活动都发生在 `result` 计算表达式中，因此可以使用验证的结果。

代码如下：

```F#
member this.Exec (commandStr:string) =
    let tokens = commandStr.Split(' ') |> List.ofArray |> List.map trimString

    // return Success of unit, or Failure
    match tokens with
    | [ "Move"; distanceStr ] -> result {
        let! distance = validateDistance distanceStr
        let newState = turtleFunctions.move distance state
        updateState newState
        }
    | [ "Turn"; angleStr ] -> result {
        let! angle = validateAngle angleStr
        let newState = turtleFunctions.turn angle state
        updateState newState
        }
    // etc
```

### 创建“函数记录”的一些实现

现在让我们创建一些实现。

同样，我们将有一个 `normalSize` 实现和一个 `halfSize` 实现。

对于 `normalSize`，我们只需要使用原始 `Turtle` 模块中的函数，并使用部分应用程序内置日志：

```F#
let normalSize() =
    let log = printfn "%s"
    // return a record of functions
    {
        move = Turtle.move log
        turn = Turtle.turn log
        penUp = Turtle.penUp log
        penDown = Turtle.penDown log
        setColor = Turtle.setColor log
    }
```

为了创建 `halfSize` 版本，我们克隆记录，只更改 `move` 函数：

```F#
let halfSize() =
    let normalSize = normalSize()
    // return a reduced turtle
    { normalSize with
        move = fun dist -> normalSize.move (dist/2.0)
    }
```

克隆记录而不是代理接口的好处是，我们不必重新实现记录中的每个功能，只需要重新实现我们关心的功能。

### 再次注入依赖项

将依赖项注入 API 的客户端代码实现得与您期望的一样。API 是一个带有构造函数的类，因此函数的记录可以以与 `ITurtle` 接口完全相同的方式传递到构造函数中：

```F#
let turtleFns = normalSize()  // a TurtleFunctions type
let api = TurtleApi(turtleFns)
drawTriangle(api)
```

如您所见，`ITurtle` 版本和 `TurtleFunctions` 版本中的客户端代码看起来完全相同！如果不是因为不同的类型，你就无法区分它们。

### 使用接口的优缺点

OO 风格的接口和 FP 风格的“函数记录”非常相似，尽管与 OO 接口不同，FP 函数是无状态的。

*优势*

- API 通过接口与特定实现解耦。
- 对于 FP “函数记录”方法（与 OO 接口相比）：
  - 函数的记录比接口更容易克隆。
  - 函数是无状态的

缺点

- 接口比单个函数更为单一，很容易包含太多不相关的方法，如果不小心，就会破坏接口隔离原则。
- 接口是不可组合的（与单个函数不同）。
- 有关此方法的更多问题，请参阅 Mark Seemann 的 Stack Overflow答案。
- 特别是对于 OO 接口方法：
  - 在重构接口时，您可能需要修改现有的类。
- 对于 FP “函数记录”方法：
  - 与 OO 接口相比，工具支持较少，互操作性较差。

这些版本的源代码可以在这里（接口）和这里（函数记录）找到。

## 7：使用函数进行依赖注入

“接口”方法的两个主要缺点是接口是不可组合的，它们打破了“只传递你需要的依赖关系”的规则，这是函数式设计的关键部分。

在真正的函数式方法中，我们会传递函数。也就是说，API 层通过作为参数传递给 API 调用的一个或多个函数进行通信。这些函数通常被部分应用，以便调用站点与“注入”解耦。

没有接口传递给构造函数，因为通常没有构造函数！（这里我只使用一个 API 类来包装可变的乌龟状态。）

在本节的方法中，我将展示两种使用函数传递注入依赖关系的替代方案：

- 在第一种方法中，每个依赖项（turtle 函数）都是单独传递的。
- 在第二种方法中，只传入一个函数。因此，为了确定使用哪个特定的 turtle 函数，定义了一个判别联合类型。

### 方法 1 - 将每个依赖关系作为单独的函数传递

管理依赖关系的最简单方法总是将所有依赖关系作为参数传递给需要它们的函数。

在我们的例子中，`Exec` 方法是唯一需要控制 turtle 的函数，因此我们可以直接将它们传递给它：

```F#
member this.Exec move turn penUp penDown setColor (commandStr:string) =
    ...
```

再次强调这一点：在这种方法中，依赖关系总是“及时”传递给需要它们的函数。构造函数中不使用依赖项，以后再使用。

以下是使用这些函数的 `Exec` 方法的更大片段：

```F#
member this.Exec move turn penUp penDown setColor (commandStr:string) =
    ...

    // return Success of unit, or Failure
    match tokens with
    | [ "Move"; distanceStr ] -> result {
        let! distance = validateDistance distanceStr
        let newState = move distance state   // use `move` function that was passed in
        updateState newState
        }
    | [ "Turn"; angleStr ] -> result {
        let! angle = validateAngle angleStr
        let newState = turn angle state   // use `turn` function that was passed in
        updateState newState
        }
    ...
```

### 在实现中使用部分应用程序进行烘焙

要创建 `Exec` 的常规或半尺寸版本，我们只需传入不同的函数：

```F#
let log = printfn "%s"
let move = Turtle.move log
let turn = Turtle.turn log
let penUp = Turtle.penUp log
let penDown = Turtle.penDown log
let setColor = Turtle.setColor log

let normalSize() =
    let api = TurtleApi()
    // partially apply the functions
    api.Exec move turn penUp penDown setColor
    // the return value is a function:
    //     string -> Result<unit,ErrorMessage>

let halfSize() =
    let moveHalf dist = move (dist/2.0)
    let api = TurtleApi()
    // partially apply the functions
    api.Exec moveHalf turn penUp penDown setColor
    // the return value is a function:
    //     string -> Result<unit,ErrorMessage>
```

在这两种情况下，我们都返回一个 `string -> Result<unit，ErrorMessage>` 类型的函数。

### 使用纯函数式的 API

所以现在，当我们想绘制一些东西时，我们只需要传入任何 `string -> Result<unit，ErrorMessage>` 类型的函数。`TurtleApi` 不再被需要或提及！

```F#
// the API type is just a function
type ApiFunction = string -> Result<unit,ErrorMessage>

let drawTriangle(api:ApiFunction) =
    result {
        do! api "Move 100"
        do! api "Turn 120"
        do! api "Move 100"
        do! api "Turn 120"
        do! api "Move 100"
        do! api "Turn 120"
        }
```

以下是 API 的使用方法：

```F#
let apiFn = normalSize()  // string -> Result<unit,ErrorMessage>
drawTriangle(apiFn)

let apiFn = halfSize()
drawTriangle(apiFn)
```

因此，尽管我们在 `TurtleApi` 中确实有可变状态，但最终的“已发布”api是一个隐藏这一事实的函数。

这种将 api 作为单个函数的方法使得模拟测试变得非常容易！

```F#
let mockApi s =
    printfn "[MockAPI] %s" s
    Success ()

drawTriangle(mockApi)
```

### 方法 2 - 传递一个处理所有命令的函数

在上面的版本中，我们传递了 5 个单独的函数！

一般来说，当你传递三到四个以上的参数时，这意味着你的设计需要调整。如果这些函数是真正独立的，你不应该真的需要那么多。

但在我们的例子中，这五个函数不是独立的——它们是一个集合——那么我们如何在不使用“函数记录”方法的情况下将它们一起传递呢？

诀窍是只传入一个函数！但是一个函数如何处理五个不同的动作呢？简单-通过使用有区别的联合来表示可能的命令。

我们之前在代理示例中已经看到过这样做，所以让我们再次回顾一下这种类型：

```F#
type TurtleCommand =
    | Move of Distance
    | Turn of Angle
    | PenUp
    | PenDown
    | SetColor of PenColor
```

我们现在需要的是一个处理这种类型的每个案例的函数。

不过，在我们这样做之前，让我们看看 `Exec` 方法实现的更改：

```F#
member this.Exec turtleFn (commandStr:string) =
    ...

    // return Success of unit, or Failure
    match tokens with
    | [ "Move"; distanceStr ] -> result {
        let! distance = validateDistance distanceStr
        let command =  Move distance      // create a Command object
        let newState = turtleFn command state
        updateState newState
        }
    | [ "Turn"; angleStr ] -> result {
        let! angle = validateAngle angleStr
        let command =  Turn angle      // create a Command object
        let newState = turtleFn command state
        updateState newState
        }
    ...
```

请注意，正在创建一个 `command` 对象，然后使用它调用 `turtleFn` 参数。

顺便说一句，这段代码与使用 `turtleAgent.Post command` 而不是 `newState = turtleFn command` 的代理实现非常相似：

### 在实现中使用部分应用程序进行烘焙

让我们使用这种方法创建两个实现：

```F#
let log = printfn "%s"
let move = Turtle.move log
let turn = Turtle.turn log
let penUp = Turtle.penUp log
let penDown = Turtle.penDown log
let setColor = Turtle.setColor log

let normalSize() =
    let turtleFn = function
        | Move dist -> move dist
        | Turn angle -> turn angle
        | PenUp -> penUp
        | PenDown -> penDown
        | SetColor color -> setColor color

    // partially apply the function to the API
    let api = TurtleApi()
    api.Exec turtleFn
    // the return value is a function:
    //     string -> Result<unit,ErrorMessage>

let halfSize() =
    let turtleFn = function
        | Move dist -> move (dist/2.0)
        | Turn angle -> turn angle
        | PenUp -> penUp
        | PenDown -> penDown
        | SetColor color -> setColor color

    // partially apply the function to the API
    let api = TurtleApi()
    api.Exec turtleFn
    // the return value is a function:
    //     string -> Result<unit,ErrorMessage>
```

如前所述，在这两种情况下，我们都返回一个 `string -> Result<unit, ErrorMessage>` 类型的函数，。我们可以将其传递给前面定义的 `drawTriangle` 函数：

```F#
let api = normalSize()
drawTriangle(api)

let api = halfSize()
drawTriangle(api)
```

### 使用函数的优缺点

*优势*

- API 通过参数化与特定实现解耦。
- 因为依赖关系是在使用点（“当着你的面”）传递的，而不是在构造函数（“看不见”）中传递的，所以依赖关系倍增的趋势大大降低了。
- 任何功能参数都是自动的“一种方法接口”，因此不需要改装。
- 常规的部分应用程序可用于烘焙“依赖注入”的参数。不需要特殊的模式或 IoC 容器。

*缺点*

- 如果依赖函数的数量太大（比如超过四个），将它们全部作为单独的参数传递可能会变得很尴尬（因此，第二种方法）。
- 区分的联合类型可能比接口更难处理。

这些版本的源代码可以在这里（五个函数参数）和这里（一个函数参数”）找到。

## 8：使用状态单子进行批处理

在接下来的两节中，我们将从“交互”模式切换到“批处理”模式，在“交互式”模式下，指令一次处理一个，在“批处理模式下，一系列指令被分组在一起，然后作为一个单元运行。

在第一个设计中，我们将回到客户端直接使用 Turtle 函数的模型。

和以前一样，客户端必须跟踪当前状态并将其传递给下一个函数调用，但这次我们将通过使用所谓的“状态单子”将状态线程化到各种指令中，从而使状态不可见。因此，任何地方都没有可变性！

这不是一个通用的状态单子，而是一个仅用于本演示的简化单子。我称之为 `turtle` 工作流程。

（有关状态 monad 的更多信息，请参阅我关于解析器组合子的“monaster”演讲和帖子）

### 定义 `turtle` 工作流程

我们在一开始定义的核心乌龟函数遵循与许多其他状态转换函数相同的“形状”，即输入加乌龟状态，输出加乌龟状态。



*（确实，到目前为止，我们还没有海龟函数的任何可用输出，但在后面的示例中，我们将看到这个输出被用于做出决策。）*

有一种标准的方法来处理这类函数——“状态单子”。

让我们看看它是如何构建的。

首先，请注意，由于 currying，我们可以将这种形状的函数重新定义为两个单独的单参数函数：处理输入会生成另一个函数，该函数反过来将状态作为参数：



然后，我们可以将海龟函数视为接受输入并返回新函数的东西，如下所示：



在我们的例子中，使用 `TurtleState` 作为状态，返回的函数如下：

```F#
TurtleState -> 'a * TurtleState
```

最后，为了更容易使用，我们可以将返回的函数视为一个独立的东西，给它起一个名字，比如 `TurtleStateComputation`：



在实现中，我们通常会用一个区分大小写的联合来包装函数，如下所示：

```F#
type TurtleStateComputation<'a> =
    TurtleStateComputation of (Turtle.TurtleState -> 'a * Turtle.TurtleState)
```

这就是“状态单子”背后的基本思想。然而，重要的是要意识到，状态单子不仅仅由这种类型组成——你还需要一些遵守一些合理规律的函数（“return”和“bind”）。

我不会在这里定义 `returnT` 和 `bindT` 函数，但您可以在完整的源代码中看到它们的定义。

我们还需要一些额外的辅助函数。（我将为所有函数添加一个 `T` 作为 Turtle 后缀）。

特别是，我们需要一种方法将一些状态输入到 `TurtleStateComputation` 中以“运行”它：

```F#
let runT turtle state =
    // pattern match against the turtle
    // to extract the inner function
    let (TurtleStateComputation innerFn) = turtle
    // run the inner function with the passed in state
    innerFn state
```

最后，我们可以创建一个 `turtle` 工作流，这是一个计算表达式，可以更容易地使用 `TurtleStateComputation` 类型：

```F#
// define a computation expression builder
type TurtleBuilder() =
    member this.Return(x) = returnT x
    member this.Bind(x,f) = bindT f x

// create an instance of the computation expression builder
let turtle = TurtleBuilder()
```

### 使用 Turtle 工作流

要使用 `turtle` 工作流，我们首先需要创建 turtle 函数的“lified”或“monadic”版本：

```F#
let move dist =
    toUnitComputation (Turtle.move log dist)
// val move : Distance -> TurtleStateComputation<unit>

let turn angle =
    toUnitComputation (Turtle.turn log angle)
// val turn : Angle -> TurtleStateComputation<unit>

let penDown =
    toUnitComputation (Turtle.penDown log)
// val penDown : TurtleStateComputation<unit>

let penUp =
    toUnitComputation (Turtle.penUp log)
// val penUp : TurtleStateComputation<unit>

let setColor color =
    toUnitComputation (Turtle.setColor log color)
// val setColor : PenColor -> TurtleStateComputation<unit>
```

`toUnitComputation` 辅助函数执行提升操作。不用担心它是如何工作的，但效果是 `move` 函数的原始版本（`Distance -> TurtleState -> TurtleState`）重生为返回 `TurtleStateComputation` 的函数（`Distance -> TurtleStateCalculation<unit>`）

一旦我们有了这些“一元(monadic)”版本，我们就可以在 `turtle` 工作流中使用它们，如下所示：

```F#
let drawTriangle() =
    // define a set of instructions
    let t = turtle {
        do! move 100.0
        do! turn 120.0<Degrees>
        do! move 100.0
        do! turn 120.0<Degrees>
        do! move 100.0
        do! turn 120.0<Degrees>
        }

    // finally, run them using the initial state as input
    runT t initialTurtleState
```

`drawTriangle` 链的第一部分有六条指令，但重要的是，*不*运行它们。只有在最后使用 `runT` 函数时，指令才会实际执行。

`drawPolygon` 示例稍微复杂一些。首先，我们定义一个绘制一侧的工作流程：

```F#
let oneSide = turtle {
    do! move 100.0
    do! turn angleDegrees
    }
```

但是，我们需要一种将所有方面结合到一个工作流程中的方法。有几种方法可以做到这一点。我将创建一个成对组合器 `chain`，然后使用 `reduce` 将所有边组合成一个操作。

```F#
// chain two turtle operations in sequence
let chain f g  = turtle {
    do! f
    do! g
    }

// create a list of operations, one for each side
let sides = List.replicate n oneSide

// chain all the sides into one operation
let all = sides |> List.reduce chain
```

以下是 `drawPolygon` 的完整代码：

```F#
let drawPolygon n =
    let angle = 180.0 - (360.0/float n)
    let angleDegrees = angle * 1.0<Degrees>

    // define a function that draws one side
    let oneSide = turtle {
        do! move 100.0
        do! turn angleDegrees
        }

    // chain two turtle operations in sequence
    let chain f g  = turtle {
        do! f
        do! g
        }

    // create a list of operations, one for each side
    let sides = List.replicate n oneSide

    // chain all the sides into one operation
    let all = sides |> List.reduce chain

    // finally, run them using the initial state
    runT all initialTurtleState
```

### `turtle` 工作流程的优点和缺点

*优势*

- 客户端代码类似于命令式代码，但保留了不变性。
- 工作流是可组合的——您可以定义两个工作流，然后组合它们以创建另一个工作流。

*缺点*

- 耦合乌龟功能的特定实现。
- 比明确跟踪状态更复杂。
- 一堆嵌套的单子/工作流很难使用。

作为最后一点的一个例子，假设我们有一个包含 `result` 工作流的 `seq`，其中包含一个 `turtle` 工作流，我们想反转它们，使 `turtle` 工作流位于外部。你会怎么做？这并不明显！

此版本的源代码可在此处获得。

## 9：使用命令对象进行批处理

另一种面向批处理的方法是以新的方式重用 `TurtleCommand` 类型。客户端创建了一个将作为一个组运行的命令列表，而不是立即调用函数。

当你“运行”命令列表时，你可以使用标准的 Turtle 库函数依次执行每个命令，使用 `fold` 将状态贯穿序列。



由于所有命令都是同时运行的，这种方法意味着客户端在调用之间不需要持久化任何状态。

以下是 `TurtleCommand` 的定义：

```F#
type TurtleCommand =
    | Move of Distance
    | Turn of Angle
    | PenUp
    | PenDown
    | SetColor of PenColor
```

为了处理一系列命令，我们需要折叠它们，将状态线程化，因此我们需要一个函数，将单个命令应用于一个状态并返回一个新的状态：

```F#
/// Apply a command to the turtle state and return the new state
let applyCommand state command =
    match command with
    | Move distance ->
        move distance state
    | Turn angle ->
        turn angle state
    | PenUp ->
        penUp state
    | PenDown ->
        penDown state
    | SetColor color ->
        setColor color state
```

然后，要运行所有命令，我们只需使用 `fold`：

```F#
/// Run list of commands in one go
let run aListOfCommands =
    aListOfCommands
    |> List.fold applyCommand Turtle.initialTurtleState
```

### 运行一批命令

例如，要绘制一个三角形，我们只需创建一个命令列表，然后运行它们：

```F#
let drawTriangle() =
    // create the list of commands
    let commands = [
        Move 100.0
        Turn 120.0<Degrees>
        Move 100.0
        Turn 120.0<Degrees>
        Move 100.0
        Turn 120.0<Degrees>
        ]
    // run them
    run commands
```

现在，由于命令只是一个集合，我们可以很容易地从较小的集合构建更大的集合。

这是一个 `drawPolygon` 的示例，其中 `drawOneSide` 返回一组命令，并且该集合在每一侧都是重复的：

```F#
let drawPolygon n =
    let angle = 180.0 - (360.0/float n)
    let angleDegrees = angle * 1.0<Degrees>

    // define a function that draws one side
    let drawOneSide sideNumber = [
        Move 100.0
        Turn angleDegrees
        ]

    // repeat for all sides
    let commands =
        [1..n] |> List.collect drawOneSide

    // run the commands
    run commands
```

### 批处理命令的优缺点

*优势*

- 比工作流或单子更容易构建和使用。
- 只有一个函数耦合到特定的实现。客户端的其余部分是解耦的。

缺点

- 仅面向批处理。
- 仅适用于控制流不基于先前命令的响应的情况。如果您确实需要对每个命令的结果做出响应，请考虑使用稍后讨论的“解释器”方法。

此版本的源代码可在此处获得。

## 间奏：有意识地使用数据类型解耦

在迄今为止的三个示例（代理、函数依赖项注入和批处理）中，我们使用了 `Command` 类型——一个有区别联合，每个 API 调用都包含一个事例。在下一篇文章中，我们还将看到类似的东西用于事件溯源和解释器方法。

这不是意外。面向对象设计和函数式设计之间的区别之一是，面向对象设计侧重于行为，而函数式设计侧重于数据转换。

因此，他们解耦的方法也不同。OO 设计更喜欢通过共享封装行为包（“接口”）来提供解耦，而函数式设计则更喜欢通过商定一种通用数据类型（有时称为“协议（protocol）”）来实现解耦（尽管我更喜欢用这个词来描述消息交换模式）。

一旦就通用数据类型达成一致，任何发出该类型的函数都可以使用常规函数组合连接到使用该类型的任何函数。

您还可以将这两种方法视为类似于 web 服务中 RPC 或面向消息的 API 之间的选择，正如基于消息的设计比 RPC 有许多优势一样，基于数据的解耦也比基于行为的解耦有相似的优势。

使用数据解耦的一些优点包括：

- 使用共享数据类型意味着组合是微不足道的。编写基于行为的接口更难。
- 可以说，每个函数都已经“解耦”了，因此在重构时不需要改造现有的函数。最坏的情况下，你可能需要将一种数据类型转换为另一种，但使用…moar函数和 moar 函数组合可以很容易地完成！
- 如果需要将代码拆分为物理上独立的服务，数据结构很容易序列化为远程服务。
- 数据结构易于安全地演化。例如，如果我添加了第六个 turtle 动作，或删除了一个动作，或更改了动作的参数，则区分的联合类型将发生变化，共享类型的所有客户端将无法编译，直到第六个 turtle 动作被考虑在内，等等。另一方面，如果你不希望现有代码中断，你可以使用版本友好的数据序列化格式，如 protobuf。当使用接口时，这两个选项都不那么容易。

## 摘要

> 模因正在传播。乌龟一定在划水 - 《看乌龟的十三种方式》，华莱士·D·科瑞萨

你好？还有人吗？谢谢你走这么远！

所以，是时候休息了！在下一篇文章中，我们将介绍观察乌龟的其余四种方法。

这篇文章的源代码可以在 github 上找到。



# 观察乌龟的十三种方式（part 2）

继续介绍事件源、FRP、一元控制流和解释器的示例。
2015年12月6日 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/13-ways-of-looking-at-a-turtle-2/#series-toc

更新：[我关于这个话题的演讲幻灯片和视频](https://fsharpforfunandprofit.com/turtle/)

这篇文章是 2015 年英语 F# 降临日历项目的一部分。查看那里的所有其他精彩帖子！特别感谢 Sergey Tihon 组织这次活动。

在这篇由两部分组成的大型帖子中，我将把简单的乌龟图形模型扩展到极限，同时演示了部分应用、验证、“提升”的概念、带有消息队列的代理、依赖注入、状态单子、事件溯源、流处理和解释器！

在上一篇文章中，我们介绍了观察乌龟的前九种方法。在这篇文章中，我们将看看剩下的四个。

作为提醒，这里有十三种方法：

- 方式1。一种基本的面向对象方法，其中我们创建了一个具有可变状态的类。
- 方式2。一种基本的函数方法，其中我们创建了一个具有不可变状态的函数模块。
- 方式3。具有面向对象核心的 API，其中我们创建了一个面向对象的 API，该 API 调用有状态核心类。
- 方式4。一个带有函数式核心的 API，其中我们创建了一个使用无状态核心函数的有状态 API。
- 方式5。代理前面的 API，其中我们创建了一个 API，该 API 使用消息队列与代理通信。
- 方式6。使用接口的依赖注入，其中我们使用接口或函数记录将实现与 API 解耦。
- 方式7。使用函数的依赖注入，其中我们通过传递函数参数将实现与 API 解耦。
- 方式8。使用状态单子进行批处理，其中我们创建了一个特殊的“海龟工作流”计算表达式来为我们跟踪状态。
- 方式9。使用命令对象进行批处理，其中我们创建一个类型来表示 turtle 命令，然后一次处理一系列命令。
- 间奏：有意识地使用数据类型解耦。关于使用数据与接口进行解耦的几点说明。
- 方式10。事件溯源，其中状态是根据过去的事件列表构建的。
- 方式11。函数式回溯编程（流处理），其中业务逻辑基于对早期事件的反应。
- 第五集：乌龟反击战，乌龟 API 发生变化，一些命令可能会失败。
- 方式12。一元控制流，其中我们根据早期命令的结果在海龟工作流中做出决策。
- 方式13。一个 turtle 解释器，其中我们将 turtle 编程与 turtle 实现完全解耦，几乎遇到了 free monad。
- 回顾所有使用的技术。

扩展版有 2 种奖励方式：

- 方式14。抽象数据海龟，其中我们使用抽象数据类型封装海龟实现的细节。
- 方式15。基于能力的 Turtle，我们根据乌龟的当前状态控制客户可以使用哪些乌龟功能。

一路下来都是乌龟！

这篇文章的所有源代码都可以在 github 上找到。

## 10：事件溯源——根据过去的事件列表建立状态

在这个设计中，我们基于 Agent（方式 5）和 Batch（方式 9）方法中使用的“命令”概念，但将“命令”替换为“事件”作为更新状态的方法。

它的工作方式是：

- 客户端向 `CommandHandler` 发送 `Command`。
- 在处理 `Command` 之前，`CommandHandler` 首先使用与特定乌龟相关的过去事件从头开始重建当前状态。
- 然后，`CommandHandler` 验证命令，并根据当前（重建）状态决定要做什么。它生成一个（可能为空）事件列表。
- 生成的事件存储在 `EventStore` 中，供下一个命令使用。


这样，客户端和命令处理程序都不需要跟踪状态。只有 `EventStore` 是可变的。

### 命令和事件类型

我们将首先定义与我们的事件源系统相关的类型。首先，与命令相关的类型：

```F#
type TurtleId = System.Guid

/// A desired action on a turtle
type TurtleCommandAction =
    | Move of Distance
    | Turn of Angle
    | PenUp
    | PenDown
    | SetColor of PenColor

/// A command representing a desired action addressed to a specific turtle
type TurtleCommand = {
    turtleId : TurtleId
    action : TurtleCommandAction
    }
```

请注意，该命令是使用 `TurtleId` 发送给特定海龟的。

接下来，我们将定义可以从命令生成的两种事件：

- `StateChangedEvent`，表示状态中发生了什么变化
- 一个 `MovedEvent`，表示乌龟运动的开始和结束位置。

```F#
/// An event representing a state change that happened
type StateChangedEvent =
    | Moved of Distance
    | Turned of Angle
    | PenWentUp
    | PenWentDown
    | ColorChanged of PenColor

/// An event representing a move that happened
/// This can be easily translated into a line-drawing activity on a canvas
type MovedEvent = {
    startPos : Position
    endPos : Position
    penColor : PenColor option
    }

/// A union of all possible events
type TurtleEvent =
    | StateChangedEvent of StateChangedEvent
    | MovedEvent of MovedEvent
```

事件来源的一个重要部分是，所有事件都以过去时标记：`Moved` 和 `Turned`，而不是 `Move` 和 `Turn`。这件事是事实——它们发生在过去。

### 命令处理程序

下一步是定义将命令转换为事件的函数。

我们需要：

- 一个（私有）`applyEvent` 函数，用于更新前一个事件的状态。
- 一个（私有）`eventsFromCommand` 函数，根据命令和状态确定要生成哪些事件。
- 一个公共的 `commandHandler` 函数，用于处理命令、从事件存储中读取事件并调用其他两个函数。

这是 `applyEvent`。您可以看到，它与我们在前面的批处理示例中看到的 `applyCommand` 函数非常相似。

```F#
/// Apply an event to the current state and return the new state of the turtle
let applyEvent log oldState event =
    match event with
    | Moved distance ->
        Turtle.move log distance oldState
    | Turned angle ->
        Turtle.turn log angle oldState
    | PenWentUp ->
        Turtle.penUp log oldState
    | PenWentDown ->
        Turtle.penDown log oldState
    | ColorChanged color ->
        Turtle.setColor log color oldState
```

`eventsFromCommand` 函数包含用于验证命令和创建事件的关键逻辑。

- 在这种特殊的设计中，命令始终有效，因此至少返回一个事件。
- `StateChangedEvent` 是由 `TurtleCommand` 在案例的直接一对一映射中创建的。
- 只有当海龟改变了位置时，才会从 `TurtleCommand` 中创建 `MovedEvent`。

```F#
// Determine what events to generate, based on the command and the state.
let eventsFromCommand log command stateBeforeCommand =

    // --------------------------
    // create the StateChangedEvent from the TurtleCommand
    let stateChangedEvent =
        match command.action with
        | Move dist -> Moved dist
        | Turn angle -> Turned angle
        | PenUp -> PenWentUp
        | PenDown -> PenWentDown
        | SetColor color -> ColorChanged color

    // --------------------------
    // calculate the current state from the new event
    let stateAfterCommand =
        applyEvent log stateBeforeCommand stateChangedEvent

    // --------------------------
    // create the MovedEvent
    let startPos = stateBeforeCommand.position
    let endPos = stateAfterCommand.position
    let penColor =
        if stateBeforeCommand.penState=Down then
            Some stateBeforeCommand.color
        else
            None

    let movedEvent = {
        startPos = startPos
        endPos = endPos
        penColor = penColor
        }

    // --------------------------
    // return the list of events
    if startPos <> endPos then
        // if the turtle has moved, return both the stateChangedEvent and the movedEvent
        // lifted into the common TurtleEvent type
        [ StateChangedEvent stateChangedEvent; MovedEvent movedEvent]
    else
        // if the turtle has not moved, return just the stateChangedEvent
        [ StateChangedEvent stateChangedEvent]
```

最后，`commandHandler` 是公共接口。它在某些依赖关系中作为参数传递：一个日志记录函数，一个从事件存储中检索历史事件的函数，以及一个将新生成的事件保存到事件存储中的函数。

```F#
/// The type representing a function that gets the StateChangedEvents for a turtle id
/// The oldest events are first
type GetStateChangedEventsForId =
     TurtleId -> StateChangedEvent list

/// The type representing a function that saves a TurtleEvent
type SaveTurtleEvent =
    TurtleId -> TurtleEvent -> unit

/// main function : process a command
let commandHandler
    (log:string -> unit)
    (getEvents:GetStateChangedEventsForId)
    (saveEvent:SaveTurtleEvent)
    (command:TurtleCommand) =

    /// First load all the events from the event store
    let eventHistory =
        getEvents command.turtleId

    /// Then, recreate the state before the command
    let stateBeforeCommand =
        let nolog = ignore // no logging when recreating state
        eventHistory
        |> List.fold (applyEvent nolog) Turtle.initialTurtleState

    /// Construct the events from the command and the stateBeforeCommand
    /// Do use the supplied logger for this bit
    let events = eventsFromCommand log command stateBeforeCommand

    // store the events in the event store
    events |> List.iter (saveEvent command.turtleId)
```

### 调用命令处理程序

现在，我们已经准备好将事件发送到命令处理程序。

首先，我们需要一些创建命令的辅助函数：

```F#
// Command versions of standard actions
let turtleId = System.Guid.NewGuid()
let move dist = {turtleId=turtleId; action=Move dist}
let turn angle = {turtleId=turtleId; action=Turn angle}
let penDown = {turtleId=turtleId; action=PenDown}
let penUp = {turtleId=turtleId; action=PenUp}
let setColor color = {turtleId=turtleId; action=SetColor color}
```

然后我们可以通过向命令处理程序发送各种命令来绘制一个图形：

```F#
let drawTriangle() =
    let handler = makeCommandHandler()
    handler (move 100.0)
    handler (turn 120.0<Degrees>)
    handler (move 100.0)
    handler (turn 120.0<Degrees>)
    handler (move 100.0)
    handler (turn 120.0<Degrees>)
```

注意：我没有展示如何创建命令处理程序或事件存储，有关完整详细信息，请参阅代码。

### 事件溯源的优缺点

*优势*

- 所有代码都是无状态的，因此易于测试。
- 支持事件回放。

*缺点*

- 实现起来可能比 CRUD 方法更复杂（或者至少工具和库的支持更少）。
- 如果不小心，命令处理程序可能会变得过于复杂，并演变为实现过多的业务逻辑。

此版本的源代码可在此处获得。

## 11：函数式回溯编程（流处理）

在上面的事件源示例中，所有域逻辑（在我们的例子中，只是跟踪状态）都嵌入在命令处理程序中。这样做的一个缺点是，随着应用程序的发展，命令处理程序中的逻辑可能会变得非常复杂。

避免这种情况的一种方法是将“函数式反应式编程”与事件源相结合，通过监听事件存储发出的事件（“信号”），创建一种在“读取侧”执行域逻辑的设计。

在这种方法中，“写端”遵循与事件源示例相同的模式。客户端向 `commandHandler` 发送 `Command`，`commandHandler` 将其转换为事件列表并将其存储在 `EventStore` 中。

然而，`commandHandler` 只做最少量的工作，比如更新状态，不做任何复杂的域逻辑。复杂的逻辑由订阅事件流的一个或多个下游“处理器”（有时也称为“聚合器”）执行。



您甚至可以将这些事件视为对处理器的“命令”，当然，处理器可以生成新的事件供另一个处理器使用，因此这种方法可以扩展到一种架构风格，其中应用程序由一组由事件存储链接的命令处理程序组成。

这种技术通常被称为“流处理”。然而，Jessica Kerr 曾将这种方法称为“函数式回溯编程（Functional Retroactive Programming）”——我喜欢这样，所以我要盗用这个名字！



### 实现设计

对于此实现，`commandHandler` 函数与事件源示例中的函数相同，只是根本不做任何工作（只是记录！）。命令处理程序仅重建状态并生成事件。如何将事件用于业务逻辑不再在其范围内。

新的东西出现在制造处理器上。

然而，在我们创建处理器之前，我们需要一些辅助函数来过滤事件存储提要，使其仅包含特定于乌龟的事件，其中只有 `StateChangedEvent`s 或 `MovedEvent`s。

```F#
// filter to choose only TurtleEvents
let turtleFilter ev =
    match box ev with
    | :? TurtleEvent as tev -> Some tev
    | _ -> None

// filter to choose only MovedEvents from TurtleEvents
let moveFilter = function
    | MovedEvent ev -> Some ev
    | _ -> None

// filter to choose only StateChangedEvent from TurtleEvents
let stateChangedEventFilter = function
    | StateChangedEvent ev -> Some ev
    | _ -> None
```

现在，让我们创建一个处理器，用于监听移动事件，并在虚拟乌龟移动时移动物理乌龟。

我们将使处理器的输入成为 `IObservable`（事件流），这样它就不会耦合到任何特定的源，如 `EventStore`。配置应用程序时，我们将把 `EventStore` “保存”事件连接到此处理器。

```F#
/// Physically move the turtle
let physicalTurtleProcessor (eventStream:IObservable<Guid*obj>) =

    // the function that handles the input from the observable
    let subscriberFn (ev:MovedEvent) =
        let colorText =
            match ev.penColor with
            | Some color -> sprintf "line of color %A" color
            | None -> "no line"
        printfn "[turtle  ]: Moved from (%0.2f,%0.2f) to (%0.2f,%0.2f) with %s"
            ev.startPos.x ev.startPos.y ev.endPos.x ev.endPos.y colorText

    // start with all events
    eventStream
    // filter the stream on just TurtleEvents
    |> Observable.choose (function (id,ev) -> turtleFilter ev)
    // filter on just MovedEvents
    |> Observable.choose moveFilter
    // handle these
    |> Observable.subscribe subscriberFn
```

在这种情况下，我们只是在打印运动——我将把一只真正的乐高头脑风暴乌龟的建造留给读者作为练习！

让我们还创建一个在图形显示器上绘制线条的处理器：

```F#
/// Draw lines on a graphics device
let graphicsProcessor (eventStream:IObservable<Guid*obj>) =

    // the function that handles the input from the observable
    let subscriberFn (ev:MovedEvent) =
        match ev.penColor with
        | Some color ->
            printfn "[graphics]: Draw line from (%0.2f,%0.2f) to (%0.2f,%0.2f) with color %A"
                ev.startPos.x ev.startPos.y ev.endPos.x ev.endPos.y color
        | None ->
            ()  // do nothing

    // start with all events
    eventStream
    // filter the stream on just TurtleEvents
    |> Observable.choose (function (id,ev) -> turtleFilter ev)
    // filter on just MovedEvents
    |> Observable.choose moveFilter
    // handle these
    |> Observable.subscribe subscriberFn
```

最后，让我们创建一个处理器来累积移动的总距离，这样我们就可以跟踪使用了多少墨水。

```F#
/// Listen for "moved" events and aggregate them to keep
/// track of the total ink used
let inkUsedProcessor (eventStream:IObservable<Guid*obj>) =

    // Accumulate the total distance moved so far when a new event happens
    let accumulate distanceSoFar (ev:StateChangedEvent) =
        match ev with
        | Moved dist ->
            distanceSoFar + dist
        | _ ->
            distanceSoFar

    // the function that handles the input from the observable
    let subscriberFn distanceSoFar  =
        printfn "[ink used]: %0.2f" distanceSoFar

    // start with all events
    eventStream
    // filter the stream on just TurtleEvents
    |> Observable.choose (function (id,ev) -> turtleFilter ev)
    // filter on just StateChangedEvent
    |> Observable.choose stateChangedEventFilter
    // accumulate total distance
    |> Observable.scan accumulate 0.0
    // handle these
    |> Observable.subscribe subscriberFn
```

该处理器使用 `Observable.scan` 将事件累积为一个值——总行驶距离。

### 实践中的处理器

让我们试试这些！

例如，这里是 `drawTriangle`：

```F#
let drawTriangle() =
    // clear older events
    eventStore.Clear turtleId

    // create an event stream from an IEvent
    let eventStream = eventStore.SaveEvent :> IObservable<Guid*obj>

    // register the processors
    use physicalTurtleProcessor = EventProcessors.physicalTurtleProcessor eventStream
    use graphicsProcessor = EventProcessors.graphicsProcessor eventStream
    use inkUsedProcessor = EventProcessors.inkUsedProcessor eventStream

    let handler = makeCommandHandler
    handler (move 100.0)
    handler (turn 120.0<Degrees>)
    handler (move 100.0)
    handler (turn 120.0<Degrees>)
    handler (move 100.0)
    handler (turn 120.0<Degrees>)
```

请注意 `eventStore.SaveEvent` 在作为参数传递给处理器之前，会被转换为 `IObservable<Guid*obj>`（即事件流）。

`drawTriangle` 生成以下输出：

```
[ink used]: 100.00
[turtle  ]: Moved from (0.00,0.00) to (100.00,0.00) with line of color Black
[graphics]: Draw line from (0.00,0.00) to (100.00,0.00) with color Black
[ink used]: 100.00
[ink used]: 200.00
[turtle  ]: Moved from (100.00,0.00) to (50.00,86.60) with line of color Black
[graphics]: Draw line from (100.00,0.00) to (50.00,86.60) with color Black
[ink used]: 200.00
[ink used]: 300.00
[turtle  ]: Moved from (50.00,86.60) to (0.00,0.00) with line of color Black
[graphics]: Draw line from (50.00,86.60) to (0.00,0.00) with color Black
[ink used]: 300.00
```

您可以看到所有处理器都在成功处理事件。

乌龟在移动，图形处理器在画线，使用墨水的处理器正确计算出移动的总距离为 300 个单位。

不过，请注意，使用墨水的处理器在每次状态变化（如转动）时都会发出输出，而不仅仅是在实际移动时。

我们可以通过在流中放入一对 `(previousDistance, currentDistance)`，然后过滤掉值相同的事件来解决这个问题。

这是新的 `inkUsedProcessor` 代码，有以下更改：

- `accumulate` 函数现在发出一对。
- 新过滤器 `changedDistanceOnly`。

```F#
/// Listen for "moved" events and aggregate them to keep
/// track of the total distance moved
/// NEW! No duplicate events!
let inkUsedProcessor (eventStream:IObservable<Guid*obj>) =

    // Accumulate the total distance moved so far when a new event happens
    let accumulate (prevDist,currDist) (ev:StateChangedEvent) =
        let newDist =
            match ev with
            | Moved dist ->
                currDist + dist
            | _ ->
                currDist
        (currDist, newDist)

    // convert unchanged events to None so they can be filtered out with "choose"
    let changedDistanceOnly (currDist, newDist) =
        if currDist <> newDist then
            Some newDist
        else
            None

    // the function that handles the input from the observable
    let subscriberFn distanceSoFar  =
        printfn "[ink used]: %0.2f" distanceSoFar

    // start with all events
    eventStream
    // filter the stream on just TurtleEvents
    |> Observable.choose (function (id,ev) -> turtleFilter ev)
    // filter on just StateChangedEvent
    |> Observable.choose stateChangedEventFilter
    // NEW! accumulate total distance as pairs
    |> Observable.scan accumulate (0.0,0.0)
    // NEW! filter out when distance has not changed
    |> Observable.choose changedDistanceOnly
    // handle these
    |> Observable.subscribe subscriberFn
```

经过这些更改，`drawTriangle` 的输出如下：

```
[ink used]: 100.00
[turtle  ]: Moved from (0.00,0.00) to (100.00,0.00) with line of color Black
[graphics]: Draw line from (0.00,0.00) to (100.00,0.00) with color Black
[ink used]: 200.00
[turtle  ]: Moved from (100.00,0.00) to (50.00,86.60) with line of color Black
[graphics]: Draw line from (100.00,0.00) to (50.00,86.60) with color Black
[ink used]: 300.00
[turtle  ]: Moved from (50.00,86.60) to (0.00,0.00) with line of color Black
[graphics]: Draw line from (50.00,86.60) to (0.00,0.00) with color Black
```

并且不再有来自 `inkUsedProcessor` 的任何重复消息。

### 流处理的优缺点

*优势*

- 与事件溯源具有相同的优势。
- 将有状态逻辑与其他非内在逻辑解耦。
- 易于添加和删除域逻辑，而不会影响核心命令处理程序。

*缺点*

- 实施起来更复杂。

此版本的源代码可在此处获得。

## 第五集：海龟反击

到目前为止，我们还没有根据海龟的状态做出决定。因此，对于最后两种方法，我们将更改乌龟 API，这样一些命令可能会失败。

例如，我们可以说乌龟必须在有限的竞技场内移动，`move` 指令可能会导致乌龟撞上障碍物。在这种情况下，`move` 指令可以返回 `MovedOk` 或 `HitBarrier` 选项。

或者说，彩色墨水的数量是有限的。在这种情况下，尝试设置颜色可能会返回“墨水不足”的响应。

那么，让我们用这些案例更新 turtle 函数。首先是 `move` 和 `setColor` 的新响应类型：

```F#
type MoveResponse =
    | MoveOk
    | HitABarrier

type SetColorResponse =
    | ColorOk
    | OutOfInk
```

我们需要一个边界检查器来查看乌龟是否在竞技场内。假设如果位置试图超出正方形（0,0,100,100），则响应为 `HitABarrier`：

```F#
// if the position is outside the square (0,0,100,100)
// then constrain the position and return HitABarrier
let checkPosition position =
    let isOutOfBounds p =
        p > 100.0 || p < 0.0
    let bringInsideBounds p =
        max (min p 100.0) 0.0

    if isOutOfBounds position.x || isOutOfBounds position.y then
        let newPos = {
            x = bringInsideBounds position.x
            y = bringInsideBounds position.y }
        HitABarrier,newPos
    else
        MoveOk,position
```

最后，`move` 函数需要一个额外的行来检查新的位置：

```F#
let move log distance state =
    let newPosition = ...

    // adjust the new position if out of bounds
    let moveResult, newPosition = checkPosition newPosition

    ...
```

以下是完整的 `move` 功能：

```F#
let move log distance state =
    log (sprintf "Move %0.1f" distance)
    // calculate new position
    let newPosition = calcNewPosition distance state.angle state.position
    // adjust the new position if out of bounds
    let moveResult, newPosition = checkPosition newPosition
    // draw line if needed
    if state.penState = Down then
        dummyDrawLine log state.position newPosition state.color
    // return the new state and the Move result
    let newState = {state with position = newPosition}
    (moveResult,newState)
```

我们也将对 `setColor` 函数进行类似的更改，如果我们试图将颜色设置为 `Red`，则返回 `OutOfInk`。

```F#
let setColor log color state =
    let colorResult =
        if color = Red then OutOfInk else ColorOk
    log (sprintf "SetColor %A" color)
    // return the new state and the SetColor result
    let newState = {state with color = color}
    (colorResult,newState)
```

有了新版本的 turtle 函数，我们必须创建能够响应错误情况的实现。这将在接下来的两个例子中完成。

*新的 turtle 函数的源代码可以在这里找到。*

## 12：Monadic 控制流

在这种方法中，我们将重用方式 8 中的 turtle 工作流。不过，这一次，我们将根据上一个命令的结果为下一个命令做出决定。

不过，在我们这样做之前，让我们看看 `move` 的更改会对我们的代码产生什么影响。假设我们想使用 `move 40.0` 向前移动几次。

如果我们与之前一样使用 `do!` 编写代码，我们遇到了一个严重的编译器错误：

```F#
let drawShape() =
    // define a set of instructions
    let t = turtle {
        do! move 60.0
        // error FS0001:
        // This expression was expected to have type
        //    Turtle.MoveResponse
        // but here has type
        //     unit
        do! move 60.0
        }
    // etc
```

相反，我们需要使用 `let!` 并将响应分配给某个对象。

在下面的代码中，我们将响应赋给一个值，然后忽略它！

```F#
let drawShapeWithoutResponding() =
    // define a set of instructions
    let t = turtle {
        let! response = move 60.0
        let! response = move 60.0
        let! response = move 60.0
        return ()
        }

    // finally, run the monad using the initial state
    runT t initialTurtleState
```

代码确实可以编译和工作，但如果我们运行它，输出显示，在第三次调用时，我们正在把乌龟撞到墙上（100,0），没有移动到任何地方。

```
Move 60.0
...Draw line from (0.0,0.0) to (60.0,0.0) using Black
Move 60.0
...Draw line from (60.0,0.0) to (100.0,0.0) using Black
Move 60.0
...Draw line from (100.0,0.0) to (100.0,0.0) using Black
```

### 根据响应做出决策

假设我们对返回 `HitABarrier` 的 `move` 的反应是旋转 90 度并等待下一个命令。不是最聪明的算法，但它可以用于演示目的！

让我们设计一个函数来实现这一点。输入将是 `MoveResponse`，但输出将是什么？我们想以某种方式对 `turn` 动作进行编码，但原始 `turn` 函数需要我们没有的状态输入。因此，让我们返回一个 `turn` 工作流，当状态可用时（在 `run` 命令中），它表示我们想要执行的指令。

以下是代码：

```F#
let handleMoveResponse moveResponse = turtle {
    match moveResponse with
    | Turtle.MoveOk ->
        () // do nothing
    | Turtle.HitABarrier ->
        // turn 90 before trying again
        printfn "Oops -- hit a barrier -- turning"
        do! turn 90.0<Degrees>
    }
```

类型签名看起来像这样：

```F#
val handleMoveResponse : MoveResponse -> TurtleStateComputation<unit>
```

这意味着它是一个一元（或“对角线”）函数——一个从正常世界开始到 `TurtleStateComputation` 世界结束的函数。

这些正是我们可以在计算表达式 `let!` 或 `do!` 中使用“bind”的函数。

现在，我们可以在 turtle 工作流中 `move` 后添加此 `handleMoveResponse` 步骤：

```F#
let drawShape() =
    // define a set of instructions
    let t = turtle {
        let! response = move 60.0
        do! handleMoveResponse response

        let! response = move 60.0
        do! handleMoveResponse response

        let! response = move 60.0
        do! handleMoveResponse response
        }

    // finally, run the monad using the initial state
    runT t initialTurtleState
```

运行它的结果是：

```
Move 60.0
...Draw line from (0.0,0.0) to (60.0,0.0) using Black
Move 60.0
...Draw line from (60.0,0.0) to (100.0,0.0) using Black
Oops -- hit a barrier -- turning
Turn 90.0
Move 60.0
...Draw line from (100.0,0.0) to (100.0,60.0) using Black
```

你可以看到移动响应是有效的。当乌龟在（100,0）处碰到边缘时，它转了90度，下一步成功了（从（100,0）到（100,60））。

好了！此代码演示了在幕后传递状态时，如何在 `turtle` 工作流中做出决策。

### 优点和缺点

*优势*

- 计算表达式允许代码专注于逻辑，同时处理“管道”——在本例中是乌龟状态。

*缺点*

- 仍然与海龟功能的特定实现相结合。
- 计算表达式的实现可能很复杂，对于初学者来说，它们的工作原理并不明显。

此版本的源代码可在此处获得。

## 13：海龟解释器

对于我们的最后一种方法，我们将研究一种将海龟的编程与其解释完全解耦的方法。

这类似于使用命令对象的批处理方法，但经过增强以支持对命令输出的响应。

### 设计一个解释器

我们将采取的方法是为一组乌龟命令设计一个“解释器”，客户端向乌龟提供命令，并对乌龟的输出做出响应，但实际的乌龟功能稍后由特定的实现提供。

换句话说，我们有一系列交错的命令和乌龟函数，看起来像这样：



那么，我们如何在代码中对这种设计进行建模呢？

对于第一次尝试，让我们将链建模为请求/响应对的序列。我们向乌龟发送一个命令，它会以 `MoveResponse` 或其他方式做出适当的响应，如下所示：

```F#
// we send this to the turtle...
type TurtleCommand =
    | Move of Distance
    | Turn of Angle
    | PenUp
    | PenDown
    | SetColor of PenColor

// ... and the turtle replies with one of these
type TurtleResponse =
    | Moved of MoveResponse
    | Turned
    | PenWentUp
    | PenWentDown
    | ColorSet of SetColorResponse
```

问题是，我们无法确定响应是否与命令正确匹配。例如，如果我发送了一个 `Move` 命令，我希望得到一个 `MoveResponse`，而不是一个 `SetColorResponse`。但这个实现并没有强制执行！

我们想让非法状态无法表达——我们怎么能做到呢？

诀窍是将请求和响应成对组合。也就是说，对于 `Move` 命令，有一个关联的函数，该函数以 `MoveResponse` 作为输入，对于其他组合也是如此。目前，没有响应的命令可以被视为返回 `unit`。

```F#
Move command => pair of (Move command parameters), (function MoveResponse -> something)
Turn command => pair of (Turn command parameters), (function unit -> something)
etc
```

其工作原理如下：

- 客户端创建一个命令，比如 `Move 100`，并提供处理响应的附加功能。
- Move 命令的 turtle 实现（在解释器内）处理输入（`Distance`），然后生成 `MoveResponse`。
- 然后，解释器接收此 `MoveResponse` 并调用客户端提供的配对中的相关函数。

通过以这种方式将 `Move` 命令与函数相关联，我们可以保证内部 turtle 实现必须接受距离并返回 `MoveResponse`，正如我们所希望的那样。

下一个问题是：输出 `something` 是什么？它是客户端处理响应后的输出，即另一个命令/响应链！

因此，我们可以将整个成对链建模为递归结构：



或者在代码中：

```F#
type TurtleProgram =
    //         (input params)  (response)
    | Move     of Distance   * (MoveResponse -> TurtleProgram)
    | Turn     of Angle      * (unit -> TurtleProgram)
    | PenUp    of (* none *)   (unit -> TurtleProgram)
    | PenDown  of (* none *)   (unit -> TurtleProgram)
    | SetColor of PenColor   * (SetColorResponse -> TurtleProgram)
```

我将类型从 `TurtleCommand` 重命名为 `TurtleProgram`，因为它不再只是一个命令，而是一个完整的命令链和相关的响应处理程序。

不过有个问题！每一步都需要另一个 `TurtleProgram` 来跟进——那么它什么时候会停止呢？我们需要某种方式来表明没有下一个命令。

为了解决这个问题，我们将在程序类型中添加一个特殊的 `Stop` 案例：

```F#
type TurtleProgram =
    //         (input params)  (response)
    | Stop
    | Move     of Distance   * (MoveResponse -> TurtleProgram)
    | Turn     of Angle      * (unit -> TurtleProgram)
    | PenUp    of (* none *)   (unit -> TurtleProgram)
    | PenDown  of (* none *)   (unit -> TurtleProgram)
    | SetColor of PenColor   * (SetColorResponse -> TurtleProgram)
```

请注意，此结构中没有提到 `TurtleState`。海龟状态的管理方式是解释器内部的，可以说不是“指令集”的一部分。

`TurtleProgram` 是抽象语法树（AST）的一个例子，AST 是一种表示要解释（或编译）的程序的结构。

### 测试解释器

让我们使用这个模型创建一个小程序。这是我们的老朋友 `drawTriangle`：

```F#
let drawTriangle =
    Move (100.0, fun response ->
    Turn (120.0<Degrees>, fun () ->
    Move (100.0, fun response ->
    Turn (120.0<Degrees>, fun () ->
    Move (100.0, fun response ->
    Turn (120.0<Degrees>, fun () ->
    Stop))))))
```

这个程序是一个只包含客户端命令和响应的数据结构，其中任何地方都没有实际的乌龟函数！是的，现在真的很难看，但我们很快就会解决这个问题。

现在下一步是解释这个数据结构。

让我们创建一个调用真正的 turtle 函数的解释器。比如说，我们将如何实施 `Move` 案例？

正如前文所述：

- 从 `Move` 案例中获取距离和相关功能
- 使用距离和当前乌龟状态调用真实的乌龟函数，以获得 `MoveResult` 和新的乌龟状态。
- 通过将 `MoveResult` 传递给相关函数来获取程序的下一步
- 最后，使用新程序和新的 turtle 状态再次（递归）调用解释器。

```F#
let rec interpretAsTurtle state program =
    ...
    match program  with
    | Move (dist,next) ->
        let result,newState = Turtle.move log dist state
        let nextProgram = next result  // compute the next step
        interpretAsTurtle newState nextProgram
    ...
```

您可以看到，更新后的 turtle 状态作为参数传递给下一个递归调用，因此不需要可变字段。

以下是 `interpretAsTurtle` 的完整代码：

```F#
let rec interpretAsTurtle state program =
    let log = printfn "%s"

    match program  with
    | Stop ->
        state
    | Move (dist,next) ->
        let result,newState = Turtle.move log dist state
        let nextProgram = next result  // compute the next step
        interpretAsTurtle newState nextProgram
    | Turn (angle,next) ->
        let newState = Turtle.turn log angle state
        let nextProgram = next()       // compute the next step
        interpretAsTurtle newState nextProgram
    | PenUp next ->
        let newState = Turtle.penUp log state
        let nextProgram = next()
        interpretAsTurtle newState nextProgram
    | PenDown next ->
        let newState = Turtle.penDown log state
        let nextProgram = next()
        interpretAsTurtle newState nextProgram
    | SetColor (color,next) ->
        let result,newState = Turtle.setColor log color state
        let nextProgram = next result
        interpretAsTurtle newState nextProgram
```

让我们运行它：

```F#
let program = drawTriangle
let interpret = interpretAsTurtle   // choose an interpreter
let initialState = Turtle.initialTurtleState
interpret initialState program |> ignore
```

输出正是我们之前看到的：

```
Move 100.0
...Draw line from (0.0,0.0) to (100.0,0.0) using Black
Turn 120.0
Move 100.0
...Draw line from (100.0,0.0) to (50.0,86.6) using Black
Turn 120.0
Move 100.0
...Draw line from (50.0,86.6) to (0.0,0.0) using Black
Turn 120.0
```

但与之前的所有方法不同，我们可以采用完全相同的程序并以新的方式对其进行解释。我们不需要设置任何类型的依赖注入，我们只需要使用不同的解释器。

因此，让我们创建另一个解释器来聚合旅行的距离，而不关心乌龟的状态：

```F#
let rec interpretAsDistance distanceSoFar program =
    let recurse = interpretAsDistance
    let log = printfn "%s"

    match program with
    | Stop ->
        distanceSoFar
    | Move (dist,next) ->
        let newDistanceSoFar = distanceSoFar + dist
        let result = Turtle.MoveOk   // hard-code result
        let nextProgram = next result
        recurse newDistanceSoFar nextProgram
    | Turn (angle,next) ->
        // no change in distanceSoFar
        let nextProgram = next()
        recurse distanceSoFar nextProgram
    | PenUp next ->
        // no change in distanceSoFar
        let nextProgram = next()
        recurse distanceSoFar nextProgram
    | PenDown next ->
        // no change in distanceSoFar
        let nextProgram = next()
        recurse distanceSoFar nextProgram
    | SetColor (color,next) ->
        // no change in distanceSoFar
        let result = Turtle.ColorOk   // hard-code result
        let nextProgram = next result
        recurse distanceSoFar nextProgram
```

在这种情况下，我将 `interpretAsDistance` 设置本地别名为 `recurse`，以明确发生了什么样的递归。

让我们用这个新的解释器运行同样的程序：

```F#
let program = drawTriangle           // same program
let interpret = interpretAsDistance  // choose an interpreter
let initialState = 0.0
interpret initialState program |> printfn "Total distance moved is %0.1f"
```

输出再次完全符合我们的预期：

```
Total distance moved is 300.0
```

### 创建“海龟程序”工作流程

创建要解释的程序的代码非常丑陋！我们可以创建一个计算表达式来让它看起来更好吗？

为了创建计算表达式，我们需要 `return` 和 `bind` 函数，这些函数要求 `TurtleProgram` 类型是泛型的。

没问题！那么，让我们将 `TurtleProgram` 设为泛型：

```F#
type TurtleProgram<'a> =
    | Stop     of 'a
    | Move     of Distance * (MoveResponse -> TurtleProgram<'a>)
    | Turn     of Angle    * (unit -> TurtleProgram<'a>)
    | PenUp    of            (unit -> TurtleProgram<'a>)
    | PenDown  of            (unit -> TurtleProgram<'a>)
    | SetColor of PenColor * (SetColorResponse -> TurtleProgram<'a>)
```

请注意，`Stop` 案例现在有一个与之关联的 `'a` 类型的值。这是我们正确实现 `return` 所必需的：

```F#
let returnT x =
    Stop x
```

`bind` 函数的实现更为复杂。现在不用担心它是如何工作的——重要的是类型匹配并且编译！

```F#
let rec bindT f inst  =
    match inst with
    | Stop x ->
        f x
    | Move(dist,next) ->
        (*
        Move(dist,fun moveResponse -> (bindT f)(next moveResponse))
        *)
        // "next >> bindT f" is a shorter version of function response
        Move(dist,next >> bindT f)
    | Turn(angle,next) ->
        Turn(angle,next >> bindT f)
    | PenUp(next) ->
        PenUp(next >> bindT f)
    | PenDown(next) ->
        PenDown(next >> bindT f)
    | SetColor(color,next) ->
        SetColor(color,next >> bindT f)
```

有了 `bind` 和 `return`，我们可以创建一个计算表达式：

```F#
// define a computation expression builder
type TurtleProgramBuilder() =
    member this.Return(x) = returnT x
    member this.Bind(x,f) = bindT f x
    member this.Zero(x) = returnT ()

// create an instance of the computation expression builder
let turtleProgram = TurtleProgramBuilder()
```

我们现在可以创建一个处理 `MoveResponse`s 的工作流，就像前面的一元控制流示例（方式 12）一样。

```F#
// helper functions
let stop = fun x -> Stop x
let move dist  = Move (dist, stop)
let turn angle  = Turn (angle, stop)
let penUp  = PenUp stop
let penDown  = PenDown stop
let setColor color = SetColor (color,stop)

let handleMoveResponse log moveResponse = turtleProgram {
    match moveResponse with
    | Turtle.MoveOk ->
        ()
    | Turtle.HitABarrier ->
        // turn 90 before trying again
        log "Oops -- hit a barrier -- turning"
        let! x = turn 90.0<Degrees>
        ()
    }

// example
let drawTwoLines log = turtleProgram {
    let! response = move 60.0
    do! handleMoveResponse log response
    let! response = move 60.0
    do! handleMoveResponse log response
    }
```

让我们使用真实的 turtle 函数来解释这一点（假设 `interpretAsTurtle` 函数已被修改以处理新的泛型结构）：

```F#
let log = printfn "%s"
let program = drawTwoLines log
let interpret = interpretAsTurtle
let initialState = Turtle.initialTurtleState
interpret initialState program |> ignore
```

输出显示，当遇到障碍时，`MoveResponse` 确实得到了正确处理：

```
Move 60.0
...Draw line from (0.0,0.0) to (60.0,0.0) using Black
Move 60.0
...Draw line from (60.0,0.0) to (100.0,0.0) using Black
Oops -- hit a barrier -- turning
Turn 90.0
```

### 重构 `TurtleProgram` 分为两部分

这种方法工作得很好，但让我感到困扰的是，`TurtleProgram` 类型中有一个特殊的 `Stop` 案例。如果我们能以某种方式专注于五只乌龟的行为而忽略它，那就太好了。

事实证明，有一种方法可以做到这一点。在 Haskell 和 Scalaz 中，它被称为“free monad”，但由于 F# 不支持类型类，我将称之为“free monad 模式”，你可以用它来解决这个问题。你必须写一些样板，但不多。

诀窍是将 api 案例和“stop”/“keep going”逻辑分为两种不同的类型，如下所示：

```F#
/// Create a type to represent each instruction
type TurtleInstruction<'next> =
    | Move     of Distance * (MoveResponse -> 'next)
    | Turn     of Angle    * 'next
    | PenUp    of            'next
    | PenDown  of            'next
    | SetColor of PenColor * (SetColorResponse -> 'next)

/// Create a type to represent the Turtle Program
type TurtleProgram<'a> =
    | Stop of 'a
    | KeepGoing of TurtleInstruction<TurtleProgram<'a>>
```

请注意，我还将 `Turn`、`PenUp` 和 `PenDown` 的响应更改为单个值，而不是 unit 函数。`Move` 和 `SetColor` 仍然是函数。

在这种新的“自由 monad”方法中，我们需要编写的唯一自定义代码是 api 类型的简单映射函数，在本例中为 `TurtleInstruction`：

```F#
let mapInstr f inst  =
    match inst with
    | Move(dist,next) ->      Move(dist,next >> f)
    | Turn(angle,next) ->     Turn(angle,f next)
    | PenUp(next) ->          PenUp(f next)
    | PenDown(next) ->        PenDown(f next)
    | SetColor(color,next) -> SetColor(color,next >> f)
```

其余的代码（`return`、`bind` 和计算表达式）总是以完全相同的方式实现，而不管特定的 api 是什么。也就是说，需要更多的样板，但需要更少的思考！

解释器需要更换，以处理新的案例。以下是新版 `interpretAsTurtle` 的一个片段：

```F#
let rec interpretAsTurtle log state program =
    let recurse = interpretAsTurtle log

    match program with
    | Stop a ->
        state
    | KeepGoing (Move (dist,next)) ->
        let result,newState = Turtle.move log dist state
        let nextProgram = next result // compute next program
        recurse newState nextProgram
    | KeepGoing (Turn (angle,next)) ->
        let newState = Turtle.turn log angle state
        let nextProgram = next        // use next program directly
        recurse newState nextProgram
```

在创建工作流时，我们还需要调整辅助函数。您可以在下面看到，我们现在有稍微复杂的代码，如 `KeepGoing(Move(dist, Stop))`，而不是原始解释器中的简单代码。

```F#
// helper functions
let stop = Stop()
let move dist  = KeepGoing (Move (dist, Stop))    // "Stop" is a function
let turn angle  = KeepGoing (Turn (angle, stop))  // "stop" is a value
let penUp  = KeepGoing (PenUp stop)
let penDown  = KeepGoing (PenDown stop)
let setColor color = KeepGoing (SetColor (color,Stop))

let handleMoveResponse log moveResponse = turtleProgram {
    ... // as before

// example
let drawTwoLines log = turtleProgram {
    let! response = move 60.0
    do! handleMoveResponse log response
    let! response = move 60.0
    do! handleMoveResponse log response
    }
```

但有了这些更改，我们就完成了，代码和以前一样工作。

### 解释器模式的优缺点

*优势*

- *解耦*。抽象语法树将程序流与实现完全解耦，并提供了很大的灵活性。
- *优化*。抽象语法树可以在运行之前进行操作和更改，以便进行优化或其他转换。例如，对于 turtle 程序，我们可以处理树并将 `Turn` 的所有连续序列折叠为单个 `Turn` 操作。这是一个简单的优化，可以节省我们与实体乌龟交流的次数。推特的 Stitch 库做了这样的事情，但显然是以一种更复杂的方式。这段视频有很好的解释。
- *低功耗代码*。创建抽象语法树的“free monad”方法允许您专注于 API 而忽略 Stop / KeepGoing 逻辑，这也意味着只需要定制最少量的代码。有关免费单子（free monad）的更多信息，请从这个优秀的视频开始，然后查看这篇文章和这篇文章。

*缺点*

- 理解起来很复杂。
- 只有在执行的操作有限的情况下才能很好地工作。
- 如果AST变得太大，可能会效率低下。

此版本的源代码可在此处（原始版本）和此处（“免费 monad”版本）获得。

## 对所用技术的回顾

在这篇文章中，我们研究了使用多种不同技术实现海龟 API 的十三种不同方法。让我们快速总结一下所使用的所有技术：

- **纯无状态函数**。如所有面向 FP 的示例所示。所有这些都很容易测试和模拟。
- **部分应用**。如最简单的 FP 示例（方式 2）所示，当海龟函数应用了日志功能，使主流可以使用管道时，它被广泛使用，特别是在“使用函数进行依赖注入的方法”（方式 7）中。
- **对象表达式**，用于在不创建类的情况下实现接口，如方式 6 所示。
- **结果类型**（也称为“Either 单子”）。在所有函数式 API 示例中使用（例如方式 4），以返回错误而不是抛出异常。
- **应用“提升”**（如 `lift2`）将正常功能提升到结果世界，同样以方式4和其他方式。
- **管理状态的多种方式**：
  - 可变字段（方式 1）
  - 明确管理状态并通过一系列函数将其管道化（方式 2）
  - 仅在边缘具有状态（方式 4 中的函数式核心/命令外壳）
  - 在代理中隐藏状态（方式 5）
  - 在 state monad 中线程化幕后状态（方式 8 和 12 中的 `turtle`工作流）
  - 通过使用批量命令（方式 9）或批量事件（方式 10）或解释器（方式 13）来完全避免状态
- **将函数包装在类型中**。在方式 8 中用于管理状态（状态单子），在方式 13 中用于存储响应。
- **计算表达式**，很多！我们创建并使用了三个：
  - 处理错误的 `result`
  - `turtle` 管理乌龟状态
  - `turtleProgram` 用于在解释器方法中构建 AST（方法 13）。
- 在 `result` 和 `turtle` 工作流中**链接一元函数**。底层函数是一元函数（monadic，“对角线（diagonal）”），通常不会正确组合，但在工作流中，它们可以轻松透明地排序。
- 在“函数依赖注入”示例中（方式 7），**将行为表示为数据结构**，以便可以传入单个函数而不是整个接口。
- **使用以数据为中心的协议进行解耦**，如代理、批处理命令、事件源和解释器示例所示。
- **使用代理进行无锁异步处理**（方式 5）。
- **“构建”计算 vs “运行”计算的分离**，如 `turtle` 工作流（方式 8 和 12）和 `turtleProgram` 工作流（方式 13：解释器）所示。
- **使用事件溯源从头开始重建状态**，而不是在内存中维护可变状态，如事件溯源（方式 10）和 FRP（方式 11）示例所示。
- **使用事件流**和 FRP（方式 11）将业务逻辑分解为小型、独立和解耦的处理器，而不是具有单片对象。

我希望很明显，研究这十三种方法只是一个有趣的练习，我并不是建议你立即将所有代码转换为使用流处理器和解释器！而且，特别是如果你和函数式编程的新手一起工作，我倾向于坚持使用早期（和更简单）的方法，除非有明显的好处来换取额外的复杂性。

## 摘要

> 当乌龟爬出视线时，它标志着许多圆圈中的一个的边缘 - *《看乌龟的十三种方式》，华莱士·D·科瑞萨（Wallace D Coriacea）*

我希望你喜欢这篇文章。我当然很喜欢写它。和往常一样，它的篇幅比我预想的要长得多，所以我希望读这文章的努力对你来说是值得的！

如果你喜欢这种比较方法，并且想要更多，请查看严崔的帖子，他正在自己的博客上做类似的事情。

享受剩余的 F# 降临日历。节日快乐！

这篇文章的源代码可以在 github 上找到。



# 观察乌龟的十三种方法 - 附录

奖励方式：抽象数据龟和基于能力的龟。
07十二月2015 这篇文章已经超过3岁了

https://fsharpforfunandprofit.com/posts/13-ways-of-looking-at-a-turtle-3/

更新：我关于这个话题的演讲幻灯片和视频

在这篇由两部分组成的巨型帖子的第三部分中，我将继续将简单的乌龟图形模型拉伸到断裂点。

在第一篇和第二篇文章中，我描述了 13 种不同的查看海龟图形实现的方法。

不幸的是，在我发表这些文章后，我意识到还有其他一些我忘了提到的方法。因此，在这篇文章中，您将看到两种奖金方式。

- 方式14。抽象数据海龟，其中我们使用抽象数据类型封装海龟实现的细节。
- 方式15。基于能力的 Turtle，我们根据乌龟的当前状态控制客户可以使用哪些乌龟功能。

作为提醒，以下是前十三种方式：

- 方式1。一种基本的面向对象方法，其中我们创建了一个具有可变状态的类。
- 方式2。一种基本的函数方法，其中我们创建了一个具有不可变状态的函数模块。
- 方式3。具有面向对象核心的 API，其中我们创建了一个面向对象的API，该API调用有状态核心类。
- 方式4。一个带有函数式核心的 API，其中我们创建了一个使用无状态核心函数的有状态API。
- 方式5。代理前面的 API，其中我们创建了一个 API，该 API 使用消息队列与代理通信。
- 方式6。使用接口的依赖注入，其中我们使用接口或函数记录将实现与 API 解耦。
- 方式7。使用函数的依赖注入，其中我们通过传递函数参数将实现与 API 解耦。
- 方式8。使用状态单子进行批处理，其中我们创建了一个特殊的“海龟工作流”计算表达式来为我们跟踪状态。
- 方式9。使用命令对象进行批处理，其中我们创建一个类型来表示 turtle 命令，然后一次处理一系列命令。
- 间奏：有意识地用数据类型解耦。关于使用数据与接口进行解耦的几点说明。
- 方式10。事件溯源，其中状态是根据过去的事件列表构建的。
- 方式11。函数式回溯编程（流处理），其中业务逻辑基于对早期事件的反应。
- 第五集：乌龟反击战，乌龟 API 发生变化，一些命令可能会失败。
- 方式12。一元控制流，其中我们根据早期命令的结果在海龟工作流中做出决策。
- 方式13。一个 turtle 解释器，其中我们将 turtle 编程与 turtle 实现完全解耦，几乎遇到了免费 monad。
- 回顾所有使用的技术。

这篇文章的所有源代码都可以在 github 上找到。

## 14：抽象数据海龟

在这个设计中，我们使用抽象数据类型的概念来封装海龟上的操作。

也就是说，“turtle”被定义为不透明类型以及相应的一组操作，就像定义 `List`、`Set` 和 `Map` 等标准 F# 类型一样。

也就是说，我们有许多函数在类型上工作，但我们不允许看到类型本身的“内部”。

从某种意义上说，您可以将其视为方式 1 中 OO 方法和方式 2 中函数方法的第三种选择。

- 在 OO 实现中，内部细节被很好地封装，并且只能通过方法进行访问。OO 类的缺点是它是可变的。
- 在 FP 实现中，`TurtleState` 是不可变的，但缺点是状态的内部是公共的，一些客户端可能已经访问了这些字段，所以如果我们改变了 `TurtleState` 的设计，这些客户端可能会崩溃。

抽象数据类型实现结合了两者的优点：乌龟状态是不可变的，就像原始的 FP 方式一样，但没有客户端可以访问它，就像 OO 方式一样。

此（以及任何抽象类型）的设计如下：

- 乌龟状态类型本身是公共的，但它的构造函数和字段是私有的。
- 相关 `Turtle` 模块中的函数可以看到 turtle 状态类型内部（因此与 FP 设计相同）。
- 因为 turtle 状态构造函数是私有的，所以我们需要在 `Turtle` 模块中有一个构造函数。
- 客户端看不到 turtle 状态类型内部，因此必须完全依赖 `Turtle` 模块函数。

这就是全部。我们只需要在早期的 FP 版本中添加一些隐私修饰符，就完成了！

### 实现

首先，我们将把 turtle 状态类型和 `Turtle` 模块放在一个名为 `AdtTurtle` 的公共模块中。这使得 `AdtTurtle.Turtle` 模块中的功能可以访问乌龟状态，但在 `AdtTurtle` 外部无法访问。

接下来，海龟状态类型现在将被称为 `Turtle`，而不是 `TurtleState`，因为我们几乎将其视为一个对象。

最后，相关模块 `Turtle`（包含函数）将具有一些特殊属性：

- `RequireQualifiedAccess` 意味着在访问函数时必须使用模块名称（就像 `List` 模块一样）
- 需要 `ModuleSuffix`，以便该模块可以与状态类型同名。对于泛型类型，这不是必需的（例如，如果我们有 `Turtle<'a>`）。

```F#
module AdtTurtle =

    /// A private structure representing the turtle
    type Turtle = private {
        position : Position
        angle : float<Degrees>
        color : PenColor
        penState : PenState
    }

    /// Functions for manipulating a turtle
    /// "RequireQualifiedAccess" means the module name *must*
    ///    be used (just like List module)
    /// "ModuleSuffix" is needed so the that module can
    ///    have the same name as the state type
    [<RequireQualifiedAccess>]
    [<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
    module Turtle =
```

避免冲突的另一种方法是让状态类型具有不同的大小写，或者具有小写别名的不同名称，如下所示：

```F#
type TurtleState = { ... }
type turtle = TurtleState

module Turtle =
    let something (t:turtle) = t
```

无论如何命名，我们都需要一种方法来构造一只新的 `Turtle`。

如果构造函数没有参数，并且状态是不可变的，那么我们只需要一个初始值，而不是一个函数（比如 `Set.empty`）。

否则，我们可以定义一个名为 `make`（或 `create` 或类似）的函数：

```F#
[<RequireQualifiedAccess>]
[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module Turtle =

    /// return a new turtle with the specified color
    let make(initialColor) = {
        position = initialPosition
        angle = 0.0<Degrees>
        color = initialColor
        penState = initialPenState
    }
```

海龟模块的其余函数与方式 2 中的实现保持不变。

### ADT 客户端

现在让我们看看客户。

首先，让我们检查一下状态是否真的是私有的。如果我们尝试显式创建一个状态，如下所示，我们会得到一个编译器错误：

```F#
let initialTurtle = {
    position = initialPosition
    angle = 0.0<Degrees>
    color = initialColor
    penState = initialPenState
}
// Compiler error FS1093:
//    The union cases or fields of the type 'Turtle'
//    are not accessible from this code location
```

如果我们使用构造函数，然后尝试直接访问一个字段（如 `position`），我们会再次收到编译器错误：

```F#
let turtle = Turtle.make(Red)
printfn "%A" turtle.position
// Compiler error FS1093:
//    The union cases or fields of the type 'Turtle'
//    are not accessible from this code location
```

但是，如果我们坚持使用 `Turtle` 模块中的函数，我们可以安全地创建一个状态值，然后像以前一样调用它的函数：

```F#
// versions with log baked in (via partial application)
let move = Turtle.move log
let turn = Turtle.turn log
// etc

let drawTriangle() =
    Turtle.make(Red)
    |> move 100.0
    |> turn 120.0<Degrees>
    |> move 100.0
    |> turn 120.0<Degrees>
    |> move 100.0
    |> turn 120.0<Degrees>
```

### ADT 的优缺点

*优势*

- 所有代码都是无状态的，因此易于测试。
- 状态的封装意味着焦点始终完全放在类型的行为和属性上。
- 客户端永远不能依赖于特定的实现，这意味着可以安全地更改实现。
- 您甚至可以交换实现（例如通过阴影或链接到不同的程序集）以进行测试、性能等。

*缺点*

- 客户必须管理当前的乌龟状态。
- 客户端无法控制实现（例如通过使用依赖注入）。

有关 F# 中 ADT 的更多信息，请参阅 Bryan Edds 的演讲和帖子。

此版本的源代码可在此处获得。

## 15：基于能力的海龟

在“一元控制流”方法（方法 12）中，我们处理了乌龟告诉我们它碰到了障碍的反应。

但即使我们遇到了障碍，也没有什么能阻止我们一遍又一遍地呼叫 `move` 行动！

现在想象一下，一旦我们遇到障碍，`move` 操作就不再对我们可用。我们不能滥用它，因为它将不再存在！

为了实现这一点，我们不应该提供 API，而是在每次调用后返回一个函数列表，客户端可以调用这些函数来执行下一步。这些功能通常包括 `move`、`turn`、`penUp` 等常见的嫌疑人，但当我们遇到障碍时，移动将从该列表中删除。简单但有效。

这种技术与一种称为基于能力的安全的授权和安全技术密切相关。如果你有兴趣了解更多，我有一系列专门的帖子。

### 设计基于能力的海龟

第一件事是定义每次调用后将返回的函数记录：

```F#
type MoveResponse =
    | MoveOk
    | HitABarrier

type SetColorResponse =
    | ColorOk
    | OutOfInk

type TurtleFunctions = {
    move     : MoveFn option
    turn     : TurnFn
    penUp    : PenUpDownFn
    penDown  : PenUpDownFn
    setBlack : SetColorFn  option
    setBlue  : SetColorFn  option
    setRed   : SetColorFn  option
    }
and MoveFn =      Distance -> (MoveResponse * TurtleFunctions)
and TurnFn =      Angle    -> TurtleFunctions
and PenUpDownFn = unit     -> TurtleFunctions
and SetColorFn =  unit     -> (SetColorResponse * TurtleFunctions)
```

让我们详细看看这些声明。

首先，任何地方都没有 `TurtleState`。已发布的 turtle 函数将为我们封装状态。同样，也没有 `log` 函数。

接下来，函数的记录 `TurtleFunctions` 为API中的每个函数定义了一个字段（`move`、`turn` 等）：

- `move` 函数是可选的，这意味着它可能不可用。
- `turn`、`penUp` 和 `penDown` 函数始终可用。
- `setColor` 操作分为三个单独的函数，每种颜色一个，因为您可能无法使用红色墨水，但仍然可以使用蓝色墨水。为了表示这些功能可能不可用，再次使用 `option`。

我们还为每个函数声明了类型别名，以使其更容易工作。编写 `MoveFn` 比在任何地方编写 `Distance -> (MoveResponse * TurtleFunctions)` 更容易！请注意，由于这些定义是相互递归的，我不得不使用 `and` 关键字。

最后，请注意本设计中 `MoveFn` 的签名与方式 12 的早期设计中 `move` 的签名之间的区别。

早期版本：

```F#
val move :
    Log -> Distance -> TurtleState -> (MoveResponse * TurtleState)
```

新版本：

```F#
val move :
    Distance -> (MoveResponse * TurtleFunctions)
```

在输入端，`Log` 和 `TurtleState` 参数已消失，在输出端，`TurtleState` 已被 `TurtleFunctions` 替换。

这意味着，以某种方式，每个 API 函数的输出都必须更改为 `TurtleFunctions` 记录。

### 实现海龟行动

为了决定我们是否真的可以移动或使用特定的颜色，我们首先需要增加 `TurtleState` 类型来跟踪这些因素：

```F#
type Log = string -> unit

type private TurtleState = {
    position : Position
    angle : float<Degrees>
    color : PenColor
    penState : PenState

    canMove : bool                // new!
    availableInk: Set<PenColor>   // new!
    logger : Log                  // new!
}
```

这已经通过以下方式得到了增强

- `canMove`，如果为 false，则表示我们处于障碍状态，不应返回有效的 `move` 函数。
- `availableInk` 包含一组颜色。如果一种颜色不在此集合中，那么我们不应该为该颜色返回有效的 `setColorXXX` 函数。
- 最后，我们将 `log` 函数添加到状态中，这样我们就不必显式地将其传递给每个操作。当乌龟被创造出来时，它将被设置一次。

`TurtleState` 现在有点丑陋，但没关系，因为它是私人的！客户甚至永远不会看到它。

有了这个增强的状态，我们可以改变 `move`。首先，我们将其设置为私有，其次，在返回新状态之前，我们将设置 `canMove` 标志（使用 `moveResult <> HitABarrier`）：

```F#
/// Function is private! Only accessible to the client via the TurtleFunctions record
let private move log distance state =

    log (sprintf "Move %0.1f" distance)
    // calculate new position
    let newPosition = calcNewPosition distance state.angle state.position
    // adjust the new position if out of bounds
    let moveResult, newPosition = checkPosition newPosition
    // draw line if needed
    if state.penState = Down then
        dummyDrawLine log state.position newPosition state.color

    // return the new state and the Move result
    let newState = {
        state with
         position = newPosition
         canMove = (moveResult <> HitABarrier)   // NEW!
        }
    (moveResult,newState)
```

我们需要一些改变 `canMove` 回到 true 的方法！所以，让我们假设如果你转身，你可以再次移动。

让我们将该逻辑添加到 `turn` 函数中：

```F#
let private turn log angle state =
    log (sprintf "Turn %0.1f" angle)
    // calculate new angle
    let newAngle = (state.angle + angle) % 360.0<Degrees>
    // NEW!! assume you can always move after turning
    let canMove = true
    // update the state
    {state with angle = newAngle; canMove = canMove}
```

`penUp` 和 `penDown` 函数保持不变，只是被设置为私有。

对于最后一个操作 `setColor`，只要墨水只使用一次，我们就会从可用性集中删除它！

```F#
let private setColor log color state =
    let colorResult =
        if color = Red then OutOfInk else ColorOk
    log (sprintf "SetColor %A" color)

    // NEW! remove color ink from available inks
    let newAvailableInk = state.availableInk |> Set.remove color

    // return the new state and the SetColor result
    let newState = {state with color = color; availableInk = newAvailableInk}
    (colorResult,newState)
```

最后，我们需要一个函数，可以从 `TurtleState` 创建 `TurtleFunctions` 记录。我称之为 `createTurtleFunctions`。

这是完整的代码，我将在下面详细讨论：

```F#
/// Create the TurtleFunctions structure associated with a TurtleState
let rec private createTurtleFunctions state =
    let ctf = createTurtleFunctions  // alias

    // create the move function,
    // if the turtle can't move, return None
    let move =
        // the inner function
        let f dist =
            let resp, newState = move state.logger dist state
            (resp, ctf newState)

        // return Some of the inner function
        // if the turtle can move, or None
        if state.canMove then
            Some f
        else
            None

    // create the turn function
    let turn angle =
        let newState = turn state.logger angle state
        ctf newState

    // create the pen state functions
    let penDown() =
        let newState = penDown state.logger state
        ctf newState

    let penUp() =
        let newState = penUp state.logger state
        ctf newState

    // create the set color functions
    let setColor color =
        // the inner function
        let f() =
            let resp, newState = setColor state.logger color state
            (resp, ctf newState)

        // return Some of the inner function
        // if that color is available, or None
        if state.availableInk |> Set.contains color then
            Some f
        else
            None

    let setBlack = setColor Black
    let setBlue = setColor Blue
    let setRed = setColor Red

    // return the structure
    {
    move     = move
    turn     = turn
    penUp    = penUp
    penDown  = penDown
    setBlack = setBlack
    setBlue  = setBlue
    setRed   = setRed
    }
```

让我们看看这是如何工作的。

首先，请注意，此函数需要附加 `rec` 关键字，因为它引用自身。我也为它添加了一个较短的别名（`ctf`）。

接下来，将创建每个 API 函数的新版本。例如，一个新的 `turn` 函数定义如下：

```F#
let turn angle =
    let newState = turn state.logger angle state
    ctf newState
```

这将使用 logger 和 state 调用原始的 `turn` 函数，然后使用递归调用（`ctf`）将新状态转换为函数记录。

对于像 `move` 这样的可选功能，它有点复杂。使用原始 `move` 定义一个内部函数 `f`，然后根据 `state.canMove` 标志是否设置，`f` 返回 `Some` 或 `None`：

```F#
// create the move function,
// if the turtle can't move, return None
let move =
    // the inner function
    let f dist =
        let resp, newState = move state.logger dist state
        (resp, ctf newState)

    // return Some of the inner function
    // if the turtle can move, or None
    if state.canMove then
        Some f
    else
        None
```

同样，对于 `setColor`，定义了一个内部函数 `f`，然后根据颜色参数是否在 `state.availableInk` 集合中返回：

```F#
let setColor color =
    // the inner function
    let f() =
        let resp, newState = setColor state.logger color state
        (resp, ctf newState)

    // return Some of the inner function
    // if that color is available, or None
    if state.availableInk |> Set.contains color then
        Some f
    else
        None
```

最后，所有这些函数都添加到记录中：

```F#
// return the structure
{
move     = move
turn     = turn
penUp    = penUp
penDown  = penDown
setBlack = setBlack
setBlue  = setBlue
setRed   = setRed
}
```

这就是你建立 `TurtleFunctions` 记录的方法！

我们还需要一件事：一个构造函数来创建 `TurtleFunctions` 的一些初始值，因为我们不再可以直接访问 API。这现在是客户唯一可用的公共功能！

```F#
/// Return the initial turtle.
/// This is the ONLY public function!
let make(initialColor, log) =
    let state = {
        position = initialPosition
        angle = 0.0<Degrees>
        color = initialColor
        penState = initialPenState
        canMove = true
        availableInk = [Black; Blue; Red] |> Set.ofList
        logger = log
    }
    createTurtleFunctions state
```

此函数在 `log` 函数中烘焙，创建新状态，然后调用 `createTurtleFunctions` 返回 `TurtleFunction` 记录供客户端使用。

### 实现基于能力的海龟客户端

让我们现在尝试使用它。首先，让我们尝试 `move 60`，然后再 `move 60`。第二步应该把我们带到边界（100），因此在那一点上，`move` 函数应该不再可用。

首先，我们使用 `Turtle.make` 创建 `TurtleFunctions` 记录。然后我们不能立即移动，我们必须先测试一下 `move` 函数是否可用：

```F#
let testBoundary() =
    let turtleFns = Turtle.make(Red,log)
    match turtleFns.move with
    | None ->
        log "Error: Can't do move 1"
    | Some moveFn ->
        ...
```

在最后一种情况下，`moveFn` 可用，因此我们可以在 60 的距离内调用它。

函数的输出是一对：一个 `MoveResponse` 类型和一个新的 `TurtleFunctions` 记录。

我们将忽略 `MoveResponse` 并再次检查 `TurtleFunctions` 记录，看看我们是否可以执行下一步操作：

```F#
let testBoundary() =
    let turtleFns = Turtle.make(Red,log)
    match turtleFns.move with
    | None ->
        log "Error: Can't do move 1"
    | Some moveFn ->
        let (moveResp,turtleFns) = moveFn 60.0
        match turtleFns.move with
        | None ->
            log "Error: Can't do move 2"
        | Some moveFn ->
            ...
```

最后，再来一次：

```F#
let testBoundary() =
    let turtleFns = Turtle.make(Red,log)
    match turtleFns.move with
    | None ->
        log "Error: Can't do move 1"
    | Some moveFn ->
        let (moveResp,turtleFns) = moveFn 60.0
        match turtleFns.move with
        | None ->
            log "Error: Can't do move 2"
        | Some moveFn ->
            let (moveResp,turtleFns) = moveFn 60.0
            match turtleFns.move with
            | None ->
                log "Error: Can't do move 3"
            | Some moveFn ->
                log "Success"
```

如果我们运行这个，我们得到输出：

```
Move 60.0
...Draw line from (0.0,0.0) to (60.0,0.0) using Red
Move 60.0
...Draw line from (60.0,0.0) to (100.0,0.0) using Red
Error: Can't do move 3
```

这确实表明，这个概念正在发挥作用！

嵌套选项匹配真的很难看，所以让我们快速制定一个 `maybe` 工作流程，让它看起来更好：

```F#
type MaybeBuilder() =
    member this.Return(x) = Some x
    member this.Bind(x,f) = Option.bind f x
    member this.Zero() = Some()
let maybe = MaybeBuilder()
```

以及我们可以在工作流中使用的日志功能：

```F#
/// A function that logs and returns Some(),
/// for use in the "maybe" workflow
let logO message =
    printfn "%s" message
    Some ()
```

现在，我们可以尝试使用 `maybe` 工作流设置一些颜色：

```F#
let testInk() =
    maybe {
    // create a turtle
    let turtleFns = Turtle.make(Black,log)

    // attempt to get the "setRed" function
    let! setRedFn = turtleFns.setRed

    // if so, use it
    let (resp,turtleFns) = setRedFn()

    // attempt to get the "move" function
    let! moveFn = turtleFns.move

    // if so, move a distance of 60 with the red ink
    let (resp,turtleFns) = moveFn 60.0

    // check if the "setRed" function is still available
    do! match turtleFns.setRed with
        | None ->
            logO "Error: Can no longer use Red ink"
        | Some _ ->
            logO "Success: Can still use Red ink"

    // check if the "setBlue" function is still available
    do! match turtleFns.setBlue with
        | None ->
            logO "Error: Can no longer use Blue ink"
        | Some _ ->
            logO "Success: Can still use Blue ink"

    } |> ignore
```

其输出为：

```
SetColor Red
Move 60.0
...Draw line from (0.0,0.0) to (60.0,0.0) using Red
Error: Can no longer use Red ink
Success: Can still use Blue ink
```

实际上，使用 `maybe` 工作流并不是一个好主意，因为第一个失败会退出工作流！你会想为真正的代码想出更好的东西，但我希望你能理解。

### 基于能力的方法的优缺点

*优势*

- 防止客户端滥用 API。
- 允许 API 在不影响客户端的情况下发展（和下放）。例如，我可以通过在函数记录中为每个颜色函数硬编码 `None` 来转换为仅单色的乌龟，之后我可以安全地删除 `setColor` 实现。在这个过程中，没有客户会崩溃！这类似于 RESTful web 服务的 HATEAOS 方法。
- 客户端与特定的实现是解耦的，因为函数的记录充当了接口。

缺点

- 实施起来很复杂。
- 客户端的逻辑要复杂得多，因为它永远无法确定某个函数是否可用！每次都要检查。
- API 不像一些面向数据的 API 那样易于序列化。

有关基于能力的安全性的更多信息，请参阅我的帖子或观看我的“企业 Tic Tac Toe”视频。

此版本的源代码可在此处获得。

## 摘要

> 我有三个想法，就像一棵手指树，里面有三只不变的乌龟 - *《看乌龟的十三种方式》，华莱士·D·科瑞萨（Wallace D Coriacea）*

我现在感觉好多了，因为我有了这两种额外的方法！感谢您的阅读！

这篇文章的源代码可以在 github 上找到。



# 理解解析器组合子

构建解析器组合子库，然后从头开始编写JSON解析器

https://fsharpforfunandprofit.com/parser/

本页包含我演讲“理解解析器组合子”中的幻灯片和代码的链接。

以下是演讲的简介：

> 传统上，编写解析器一直很困难，涉及 Lex 和 Yacc 等晦涩难懂的工具。另一种方法是用你最喜欢的编程语言编写一个解析器，使用“解析器组合子”库和不比正则表达式复杂的概念。
>
> 在本次演讲中，我们将深入探讨解析器组合子。我们将使用函数式编程技术在 F# 中从头开始构建一个解析器组合子库，然后使用它来实现一个功能齐全的 JSON 解析器。

这次演讲是基于我关于这个话题的博客文章：

- 理解解析器组合子

## 视频

奥斯陆NDC视频，2016年6月9日（点击图片查看视频）

奥斯陆NDC视频，2016年6月9日

## 幻灯片

2016年6月9日，奥斯陆NDC幻灯片

**[Understanding parser combinators](https://www.slideshare.net/ScottWlaschin/understanding-parser-combinators)** from **[my slides on Slideshare](http://www.slideshare.net/ScottWlaschin)**



# “理解解析器组合器”系列

https://fsharpforfunandprofit.com/series/understanding-parser-combinators/

在本系列中，我们将看看所谓的“应用解析器”是如何工作的。为了理解一些东西，没有什么比自己构建它更好的了，所以我们将从头开始创建一个基本的解析器库，然后创建一些有用的“解析器组合子”，最后构建一个完整的 JSON 解析器。

1. 理解解析器组合器
   从头开始构建解析器组合器库
2. 构建一组有用的解析器组合子
   15个左右的组合子，可以组合起来解析几乎任何东西
3. 改进解析器库
   添加更多信息错误
4. 从头开始编写JSON解析器
   250行代码



# 不使用静态类型函数式编程语言的十个理由

对我不明白的事情咆哮
2013年4月12日 这篇文章已有3年多的历史了

https://fsharpforfunandprofit.com/posts/ten-reasons-not-to-use-a-functional-programming-language/

你受够了关于函数式编程的所有炒作吗？我也是！我想我会咆哮一些理由，为什么像我们这样明智的人应该远离它。

需要明确的是，当我说“静态类型函数式编程语言”时，我指的是还包括类型推理、默认不变性等内容的语言。在实践中，这意味着Haskell和ML家族（包括 OCaml 和 F#）。

## 原因1：我不想追随最新的时尚

和大多数程序员一样，我天生保守，不喜欢学习新东西。这就是我选择IT职业的原因。

我不会仅仅因为所有“酷孩子”都在做这件事就加入最新的潮流——我会等到事情成熟，我才能得到一些看法。

对我来说，函数式编程还没有出现足够长的时间来让我相信它会一直存在。

是的，我想一些学究会声称 ML（从1973年开始）和Haskell（从1990年开始）的存在时间几乎和 Java 和 PHP 等老热门一样长，但我最近才听说 Haskell，所以这个论点对我来说并不成立。

看看这群孩子，F#。看在皮特的份上，它才七岁！当然，对于地质学家来说，这可能是一段很长的时间，但在互联网时代，七年只是一眨眼的功夫。

所以，总的来说，我肯定会采取谨慎的态度，等待几十年，看看这个函数式编程的东西是否会继续存在，或者它是否只是昙花一现。

## 原因2：我按行计酬

我不知道你是怎么想的，但我写的代码行数越多，我就觉得效率越高。如果我能在一天内写出500行代码，那是一项很好的工作。我的承诺很大，我的老板可以看出我一直很忙。

但是，当我将用函数式语言编写的代码与一种很好的旧式 C 语言进行比较时，代码要少得多，这让我感到害怕。

我的意思是，看看用熟悉的语言编写的代码：

```c#
public static class SumOfSquaresHelper
{
   public static int Square(int i)
   {
      return i * i;
   }

   public static int SumOfSquares(int n)
   {
      int sum = 0;
      for (int i = 1; i <= n; i++)
      {
         sum += Square(i);
      }
      return sum;
   }
}
```

并将其与此进行比较：

```F#
let square x = x * x
let sumOfSquares n = [1..n] |> List.map square |> List.sum
```

这是17行，而只有2行。想象一下，这种差异在整个项目中成倍增加！

如果我真的使用这种方法，我的生产力会急剧下降。对不起，我就是买不起。

## 原因3：我喜欢一些花括号

这是另一回事。这些去掉花括号的语言是怎么回事。他们怎么能称自己为真正的编程语言呢？

我来告诉你我的意思。这是一个带有熟悉花括号的代码示例。

```C#
public class Squarer
{
    public int Square(int input)
    {
        var result = input * input;
        return result;
    }

    public void PrintSquare(int input)
    {
        var result = this.Square(input);
        Console.WriteLine("Input={0}. Result={1}", input, result);
    }
}
```

这里有一些类似的代码，但没有花括号。

```F#
module Squarer =

    let square input =
        let result = input * input
        result

    let printSquare input =
        let result = square input
        printfn "Input=%i. Result=%i" input result
```

看看区别！我不知道你怎么想，但我觉得第二个例子有点令人不安，好像缺少了什么重要的东西。

说实话，没有花括号给我的指导，我感到有点失落。

## 原因4：我喜欢看到显式类型

函数式语言的支持者声称，类型推理使代码更清晰，因为你不必一直用类型声明来弄乱代码。

好吧，碰巧的是，我喜欢看到类型声明。如果我不知道每个参数的确切类型，我会感到不舒服。这就是为什么 Java 是我最喜欢的语言。

这是一些 ML 代码的函数签名。不需要类型声明，所有类型都是自动推断的。

```F#
let groupBy source keySelector =
    ...
```

这是 C# 中类似代码的函数签名，带有显式类型声明。

```F#
public IEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(
    IEnumerable<TSource> source,
    Func<TSource, TKey> keySelector
    )
    ...
```

我可能是这里的少数派，但我更喜欢第二个版本。对我来说，重要的是要知道返回的类型是`IEnumerable<IGrouping<TKey，TSource>>`。

当然，编译器会为您进行类型检查，并在类型不匹配时发出警告。但是，当你的大脑可以做这项工作时，为什么让编译器来做呢？

好吧，我承认，如果你确实使用泛型、lambda、返回函数的函数以及所有其他新奇的东西，那么是的，你的类型声明可能会变得非常复杂。而且很难正确地输入它们。

但我对此有一个简单的解决方案——不要使用泛型，也不要传递函数。你的签名会简单得多。

## 原因5：我喜欢修复bug

对我来说，没有什么比狩猎的刺激更令人兴奋的了——找到并杀死一只讨厌的虫子。如果bug出现在生产系统中，那就更好了，因为我也会成为英雄。

但我读到，在静态类型函数式语言中，引入bug要困难得多。

真是令人沮丧。

## 原因6：我住在调试器中

说到bug修复，我一天中的大部分时间都在调试器中度过，逐步完成代码。是的，我知道我应该使用单元测试，但说起来容易做起来难，好吗？

不管怎样，显然对于这些静态类型的函数式语言，如果你的代码编译成功，它通常会正常工作。

我听说你必须花很多时间提前匹配类型，但一旦完成并成功编译，就没有什么可调试的了。这有什么好玩的？

这让我想到…

## 原因7：我不想考虑每一个细节

所有这些匹配类型并确保一切都是完美的，对我来说听起来很累。

事实上，我听说你不得不考虑所有可能的边缘情况、所有可能的错误条件以及其他可能出错的事情。你必须从一开始就这样做——你不能懒惰，把它推迟到以后。

我宁愿让一切（大多）朝着幸福的方向发展，然后在出现错误时进行修复。

## 原因8：我喜欢检查空值

我非常认真地检查每个方法上的空值。得知我的代码因此完全防弹，我感到非常满意。

```c#
void someMethod(SomeClass x)
{
    if (x == null) { throw new NullArgumentException(); }

    x.doSomething();
}
```

哈哈！开玩笑的！当然，我不会费心把空检查代码放在任何地方。我永远不会完成任何真正的工作。

我从来没有遇到过由null引起的严重问题。好吧，一个。但在停电期间，该公司并没有损失太多钱。我知道大多数员工都很感激这次意外的休息日。所以我不知道为什么这是一件大事。

## 原因9：我喜欢在任何地方使用设计模式

我第一次在《设计模式》一书中读到关于设计模式的内容（出于某种原因，它被称为《四人帮》一书，但我不知道为什么，因为它的前面有一个女孩），从那以后，我一直在努力使用它们来解决所有问题。这当然让我的代码看起来严肃而“企业化”，给我的老板留下了深刻的印象。

但我没有看到函数式设计中提到任何模式。没有 Strategy、AbstractFactory、Decorator、Proxy 等，你如何完成有用的事情？

也许函数式程序员没有意识到它们？

## 理由10：太数学化了

这里还有一些计算平方和的代码。这太难理解了，因为里面有很多奇怪的符号。

```
ss=: +/ @: *:
```

哎呀，对不起！我的错。这是 J 代码。

但我确实听说函数式程序使用奇怪的符号，如 `<*>` 和 `>>=`，以及被称为“monads”和“functor”的模糊概念。

我不知道为什么函数式的人不能坚持我已经知道的东西——像 `++` 和 `!=` 这样的明显符号以及“继承”和“多态性”等简单概念。

## 总结：我不明白

你知道吗。我不明白。我不明白为什么函数式编程是有用的。

我真正希望的是，有人能在一页纸上向我展示一些真正的好处，而不是给我太多的信息。

更新：所以现在我已经阅读了“在一页上你需要知道的一切”页面。但对我来说，它太短了，太简单了。

我真的在寻找一些更深入的东西——一些我可以深入其中的东西。

不，不要说我应该阅读教程，玩例子，写自己的代码。我只是想在不做所有这些工作的情况下摸索一下。

我不想为了学习一种新的范式而改变我的思维方式。

# 为什么我不写monad教程

2013年5月14日 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/why-i-wont-be-writing-a-monad-tutorial/

“在Haskell中，‘新手’是指还没有实现编译器的人。他们只写了一个monad教程”-Pseudonymn

让我们从一个故事开始…

## 爱丽丝学数数

小爱丽丝和她的父亲（碰巧是一位数学家）正在参观一个宠物动物园…

爱丽丝：看那些小猫。

爸爸：它们真可爱。有两个。

艾丽斯：看那些小狗。

爸爸：没错。你会数数吗？有两只小狗。

爱丽丝：看那些小马。

爸爸：是的，亲爱的。你知道小猫、小狗和小马有什么共同点吗？

艾丽斯：没有。没有共同点！

爸爸：嗯，其实他们确实有一些共同点。你能看到它是什么吗？

艾丽斯：不！小狗不是小猫。荷西不是小猫。

爸爸：我给你解释一下怎么样？首先，让我们考虑一个集合S，它在集合成员方面是严格有序的，其中S的每个元素也是S的子集。这能给你一个线索吗？

爱丽丝：（泪如雨下）

## 如何不赢得朋友和影响别人

没有（明智的）父母会试图从序数的正式定义开始解释如何计数。

那么，为什么许多人觉得有必要通过强调单子的形式定义来解释单子这样的概念呢？

对于大学水平的数学课来说，这可能很好，但对于那些只想创造有用东西的普通程序员来说，这显然行不通。

然而，这种方法的一个不幸结果是，现在围绕单子的概念有一个完整的神秘感。它已经成为你通往真正启蒙之路必须跨越的桥梁。当然，有大量的monad教程可以帮助你跨越它。

事实是：你不需要理解单子来编写有用的函数式代码。与 Haskell 相比，F# 尤其如此。

单子不是金锤。他们不会让你更有效率。它们不会减少你的代码的错误。

所以，真的不用担心他们。

## 为什么我不写monad教程

所以这就是为什么我不会写单子教程。我认为它不会帮助人们学习函数式编程。如果有的话，它只会造成混乱和焦虑。

是的，我会在许多不同的帖子中使用 monad 的例子，但是，除了这里，我会尽量避免在这个网站的任何地方使用“monad”这个词。事实上，它在我的禁用词列表中占有一席之地！

## 为什么你应该写一个 monad 教程

另一方面，我确实认为你应该写一个单子教程。当你试图向别人解释某事时，你自己最终会更好地理解它。

以下是我认为你应该遵循的过程：

1. 首先，编写大量涉及列表、序列、选项、异步工作流、计算表达式等的实用代码。
2. 当你变得更有经验时，你会开始使用更多的抽象概念，专注于事物的形状而不是细节。
3. 在某个时刻，你会哈哈大笑！瞬间——突然意识到所有抽象概念都有共同点。
4. 答对 了！是时候写你的monad教程了！

关键是你必须按这个顺序做——你不能直接跳到最后一步，然后倒退。正是通过细节来理解抽象的行为，使你能够在看到它时理解它。

祝你的教程顺利——我要去吃墨西哥卷饼了。