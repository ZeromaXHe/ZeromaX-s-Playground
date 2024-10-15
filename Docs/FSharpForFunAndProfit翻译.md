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



# 安装和使用 F#

关于在 Visual Studio、SharpDevelop 和 MonoDevelop 中下载、安装和使用 F# 的说明

https://fsharpforfunandprofit.com/installing-and-using/

F# 编译器是一个免费的开源工具，可用于 Windows、Mac 和 Linux（通过 Mono）。了解更多关于 F# 以及如何在 F# 基金会安装它的信息。

您可以将其与 IDE（Visual Studio、MonoDevelop）或您最喜欢的编辑器（VS Code 和 Atom 使用 Ionide 对 F# 有特别好的支持）一起使用，也可以简单地将其用作独立的命令行编译器。

如果您不想安装任何东西，可以尝试 .NET Fiddle 站点，这是一个交互式环境，您可以在 web 浏览器中探索 F#。你应该能够在那里运行这个网站上的大部分代码。

## 使用代码示例

安装并运行 F# 后，您可以按照代码示例进行操作。

在本网站上运行代码示例的最佳方式是将代码键入 `.FSX` 脚本文件，然后您可以将其发送到 F# 交互式窗口进行评估。或者，您可以直接在 F# 交互式控制台窗口中键入示例。除了一两行之外，我建议使用脚本文件方法。

对于较长的示例，可以从本网站下载代码——链接将在帖子中。

最后，我鼓励您尝试并修改这些示例。如果您随后遇到编译器错误，请务必查看“F# 故障排除”页面，该页面解释了最常见的问题以及如何修复它们。

### 目录

- 安装 F#
- 使用 F# 和各种工具
  - 在 Visual Studio 中使用 F#
  - 在 Linux 和 Mac 上使用 F# 和 Mono
  - 直接在浏览器中尝试 F#
  - 在 FSI 交互式 shell 中使用 F#
  - 在 SharpDevelop 中使用 F#
- 编译错误
- 项目和解决方案
- 使用 F# 编写 shell 脚本

## 安装 F#

您可以在此处获取多个平台的F#。下载并安装 F# 后，您还可以考虑安装 F# 电源组（power pack），它提供了许多不错的附加功能，其中一些将在本网站中提及。

## 在Visual Studio中使用F#

如果您使用的是 Windows 平台，强烈建议使用 Visual Studio 编写 F#，因为 F# 与 IDE、调试器等具有很好的集成。

- 如果你有 Visual Studio 2010 或更高版本，F# 已经包含在内。
- 如果你有 Visual Studio 2008，你可以从 MSDN 下载 F# 的安装程序。
- 如果你两者都没有，你可以安装“Visual Studio Integrated Shell”，然后在其中安装F#。

安装 F# 后，您应该创建一个 F# 项目。

新项目

然后创建一个脚本（FSX）文件来运行其中的示例。

新 FSX 脚本

接下来，确保 F# 交互窗口处于活动状态（通常通过“视图”>“其他窗口”>“F# 交互”）。

使用脚本文件是尝试 F# 的最简单方法；只需在脚本窗口中键入一些代码，并对其进行求值，即可在下面的交互式窗口中看到输出。要评估突出显示的代码，您可以：

- 右键单击以获取上下文菜单，然后执行“发送到交互式”。请注意，如果 F# 交互式窗口不可见，则“发送到交互式”菜单选项将不会出现。
- 使用 `Alt+Enter` 组合键（但请参阅下面关于键盘映射的注释）。

发送到互动

> **Resharper 警报**
>
> 如果您安装了 Resharper 或其他插件，则可以使用 `Alt+Enter` 组合键。在这种情况下，许多人将命令重新映射为 `Alt+Semicolon`。
>
> 您可以从 Visual Studio 菜单“工具”>“选项”>“键盘”重新映射命令，“发送到交互式”命令称为 `EditorContextMenus.CodeWindow.SendToInteractive`。

您也可以直接在交互式窗口中工作。但在这种情况下，您必须始终用双分号终止代码块。

互动

## 在 Linux 和 Mac 上使用 F# 和 Mono

从 Mono 3.0.2 版本开始，F# 就包含在 Mono 中。在此处下载 Mono。

安装Mono后，您可以使用MonoDevelopIDE或Emacs等编辑器。

- MonoDevelop 集成了对 F# 的支持。请参阅 http://addins.monodevelop.com/Project/Index/48.
- Emacs 有一个 F# 模式，它通过 F# 的语法突出显示对其进行了扩展。请参阅 Codeplex 上的“F# 跨平台包和示例”页面。

### 在浏览器中尝试 F#

如果你不想下载任何东西，你可以直接从浏览器尝试 F#。该网站位于 try.fsharp.org。请注意，它确实需要 Silverlight 才能运行。

互动

## 在交互式 shell 中使用F#

F# 有一个名为 FSI.exe 的简单交互式控制台，也可用于在中运行代码。就像 Visual Studio 中的交互式窗口一样，您必须用双分号终止一段代码。

FSI

## 在 SharpDevelop 中使用 F#

SharpDevelop 对 F# 有一些支持。您可以创建一个 F# 项目，然后在其中创建一个 FSX 脚本文件。然后在脚本窗口中键入一些代码，并使用上下文菜单将代码发送到交互式窗口（如下所示）。

发送到互动

## 编译错误？

如果您在编译自己的代码时遇到问题，“F# 疑难解答”页面可能会有所帮助。

## 项目和解决方案

F# 使用与 C# 完全相同的“项目”和“解决方案”模型，所以如果你熟悉这一点，你应该能够很容易地创建一个 F# 可执行文件。

要制作一个将作为项目一部分编译的文件，而不是脚本文件，请使用 `.fs` 扩展名。`.fsx` 文件将不会被编译。

然而，F# 项目确实与 C# 有一些主要区别：

- F# 文件是线性组织的，而不是文件夹和子文件夹的层次结构。事实上，F# 项目中没有“添加新文件夹”选项！这通常不是问题，因为与 C# 不同，F# 文件包含多个类。在 C# 中，一个完整的类文件夹可能很容易在 F# 中变成一个文件。
- 项目中文件的顺序非常重要：“后”F#文件可以使用“前”F#文件中定义的公共类型，但反之则不行。因此，文件之间不能有任何循环依赖关系。
- 您可以通过右键单击并执行“上移”或“下移”来更改文件的顺序。同样，在创建新文件时，您可以选择“添加到上方”或“添加到下方”现有文件。

## F# 中的 Shell 脚本

您还可以将 F# 用作脚本语言，而不必将代码编译成 EXE。这是通过使用 FSI 程序完成的，FSI 程序不仅是一个控制台，还可以像使用 Python 或 Powershell 一样用于运行脚本。

当您想快速创建一些代码而不将其编译成完整的应用程序时，这非常方便。F# 构建自动化系统“FAKE”就是一个很好的例子。

为了了解如何自己完成此操作，这里有一个将网页下载到本地文件的小示例脚本。首先创建一个 FSX 脚本文件——称之为“`ShellScriptExample.fsx`”——并粘贴以下代码。

```F#
// ================================
// Description:
//    downloads the given url and stores it as a file with a timestamp
//
// Example command line:
//    fsi ShellScriptExample.fsx http://google.com google
// ================================

// "open" brings a .NET namespace into visibility
open System.Net
open System

// download the contents of a web page
let downloadUriToFile url targetfile =
    let req = WebRequest.Create(Uri(url))
    use resp = req.GetResponse()
    use stream = resp.GetResponseStream()
    use reader = new IO.StreamReader(stream)
    let timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH-mm")
    let path = sprintf "%s.%s.html" targetfile timestamp
    use writer = new IO.StreamWriter(path)
    writer.Write(reader.ReadToEnd())
    printfn "finished downloading %s to %s" url path

// Running from FSI, the script name is first, and other args after
match fsi.CommandLineArgs with
    | [| scriptName; url; targetfile |] ->
        printfn "running script: %s" scriptName
        downloadUriToFile url targetfile
    | _ ->
        printfn "USAGE: [url] [targetfile]"
```

现在不要担心代码是如何工作的。无论如何，它都很粗糙，一个更好的例子会添加错误处理等。

要运行此脚本，请在同一目录中打开一个命令窗口，然后键入：

```shell
fsi ShellScriptExample.fsx http://google.com google_homepage
```

当你在这个网站上玩代码时，你可能想尝试同时创建一些脚本。



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



# 在工作中使用 F# 的 26 种低风险方法

*Part of the "Low-risk ways to use F# at work" series (*[link](https://fsharpforfunandprofit.com/posts/low-risk-ways-to-use-fsharp-at-work/#series-toc)*)*

您现在就可以开始，无需任何许可
2014年4月20日 这篇文章已有3年多的历史了

https://fsharpforfunandprofit.com/posts/low-risk-ways-to-use-fsharp-at-work/#series-toc

所以你们都对函数式编程感到兴奋，你们一直在业余时间学习 F#，你们咆哮着它有多棒，惹恼了同事，你们渴望在工作中把它用于严肃的事情…

但后来你撞到了砖墙。

你的工作场所有“只使用 C#”的政策，不允许你使用 F#。

如果你在一个典型的企业环境中工作，获得一门新语言的批准将是一个漫长的过程，包括说服你的队友、QA 人员、运维人员、你的老板、你老板的老板，以及大厅里你从未交谈过的神秘家伙。我鼓励你开始这个过程（这对你的经理来说是一个有用的链接），但你仍然不耐烦，想“我现在能做什么？”

另一方面，也许你在一个灵活、随和的地方工作，在那里你可以做你喜欢的事情。

但你是认真的，不想成为那些在 APL 中重写一些关键任务系统，然后消失得无影无踪，给你的替代品留下一些令人费解的神秘代码来维护的人。不，你要确保你没有做任何会影响你团队总线因素的事情。

因此，在这两种情况下，你都想在工作中使用 F#，但你不能（或不想）将其用于核心应用程序代码。

你能做什么？

好吧，别担心！本系列文章将建议您以低风险、增量的方式使用 F#，而不会影响任何关键代码。

## 系列内容

这里列出了 26 种方法，这样你就可以直接找到任何你觉得特别有趣的方法。

第 1 部分 - 使用 F# 进行交互式探索和开发

1. 使用 F# 探索 .NET 框架交互式
2. 使用 F# 交互式测试自己的代码
3. 使用 F# 以交互方式使用 Web 服务
4. 使用 F# 以交互方式玩 UI

第 2 部分 - 使用 F# 进行开发和 devops 脚本

5. 使用 FAKE 构建和 CI 脚本
6. 一个 F# 脚本，用于检查网站是否响应
7. 将 RSS 提要转换为 CSV 的 F# 脚本
8. 使用 WMI 检查进程统计信息的 F# 脚本
9. 使用 F# 配置和管理云

第 3 部分 - 使用 F# 进行测试

10. 使用 F# 编写具有可读名称的单元测试
11. 使用 F# 以编程方式运行单元测试
12. 使用 F# 学习以其他方式编写单元测试
13. 使用 FsCheck 编写更好的单元测试
14. 使用 FsCheck 创建随机虚拟数据
15. 使用 F# 创建模拟
16. 使用 F# 进行自动浏览器测试
17. 使用 F# 进行行为驱动开发

第 4 部分 - 使用 F# 执行与数据库相关的任务

18. 使用 F# 替换 LINQpad
19. 使用 F# 对存储过程进行单元测试
20. 使用 FsCheck 生成随机数据库记录
21. 使用 F# 进行简单的 ETL
22. 使用 F# 生成 SQL 代理脚本

第 5 部分-使用 F# 的其他有趣方法

23. 使用 F# 进行解析
24. 使用 F# 绘制图表和可视化
25. 使用 F# 访问基于 web 的数据存储
26. 使用 F# 进行数据科学和机器学习
27. （奖金）平衡英国发电站的发电计划



## [跳转系列独立 Markdown](./FSharpForFunAndProfit翻译-“在工作中使用F#的26种低风险方法”系列.md)



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

我也计划很快上传一些关于这些主题的帖子。同时，请参阅函数式应用程序系列的配方，其中涵盖了类似的内容。

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



# 反对面向铁路的编程

（如果使用不当）
2019年12月20日

https://fsharpforfunandprofit.com/posts/against-railway-oriented-programming/

> 这篇文章是 2019 年 F# 降临日历的一部分。查看那里的所有其他精彩帖子！特别感谢 Sergey Tihon 组织这次活动。

六年半前，我写了一篇文章，并就我所说的“面向铁路的编程”做了一次演讲。这是我向自己和其他人解释如何使用 `Result`/`Either` 将错误生成函数链接在一起的一种方式。

令我惊讶的是，这个愚蠢的铁路类比真的开始了，现在有面向铁路的编程库和各种语言的帖子，包括 Ruby、Java、JavaScript、Kotlin、Python 等。

我仍然认为这是一个很好的类比，但我确实认为它经常被轻率地使用，尤其是如果它是你刚刚添加到工具箱中的一项闪亮的新技术。

所以，在这篇文章中，我将阐述为什么你不应该使用面向铁路的编程！或者更准确地说，为什么不应该在所有地方都使用 `Result` 类型（因为 ROP 只是用于连接 `Result` 返回函数的管道）。微软关于错误管理的页面也有很好的建议，这篇博客文章也是如此。

## #1 – 如果需要诊断，请不要使用 Result

如果您关心错误的位置、堆栈跟踪或其他诊断，请不要使用 `Result`。特别是，不要将 `Result` 用作异常的替代品，而是将堆栈跟踪或整个异常存储在 `Result` 中。这有什么意义？

相反，将 `Result` 视为一个带有额外信息的美化布尔值。它仅适用于预期的控制流，不适用于异常。

## #2 – 不要使用 Result 来重新定义异常

我看到人们不加选择地使用 `Result` 来处理各种错误，包括用异常处理会更好的事情。不要重新发明“try-catch”！

我也看到人们试图完全隐藏异常。这是徒劳的。无论您将多少异常转换为 `Result`s，总会有一些异常泄露出来。您始终需要在系统的最高部分适当地处理异常。

## #3 - 如果你需要快速失败，不要使用 Result

如果确实出了问题，你无法继续，不要返回 `Result`，继续前进。快速失败，出现异常，甚至立即退出应用程序。

## #4 – 如果没有人会看到，则不要使用 Result

如果你正在执行一些复杂的控制流，但逻辑对外界是隐藏的，不要仅仅为了它而使用 `Result`。通常，在本地使用异常会更干净。

例如，假设你正在通过遍历一棵树来收集信息，当出现问题时，你需要提前退出。

在 ROP 方法中，你会让节点处理函数返回一个 `Result`，然后必须使用 `bind` 将其传递给下一个节点处理函数，以此类推。对于复杂的导航，你可以花很多时间来计算逻辑，以便代码能够编译（当然，Haskell 程序员除外）

另一方面，你可以定义一个私有的本地异常（例如，以 Python 的 `StopIteration` 的风格），命令式编写迭代，在需要提前返回时抛出异常，然后在顶层捕获异常。只要代码不太长，并且异常是在本地定义的，这种方法通常可以使代码更清晰。如果没有消费者看到内部，那么就没有伤害，没有犯规。

另一个例子可能是在定义微服务时。如果整个代码只有几百行长，并且对调用者不透明，那么使用异常而不是 `Result` 是完全可以的，只要它们不超出服务边界。

## #5 – 如果没有人关心错误情况，则不要使用 Result

通常，`Result` 被定义为错误情况是所有可能出错的事物的判别联合。

例如，假设你想从文件中读取文本，所以你定义了一个这样的函数：

```F#
type ReadTextFromFile = FileInfo -> Result<string, FileError>
```

其中 `FileError` 的定义如下：

```F#
type FileError =
  | FileNotFound
  | DirectoryNotFound
  | FileNotAccessible
  | PathTooLong
  | OtherIOError of string
```

但是，这个函数的使用者真的关心读取文件时可能出错的每一件事吗？也许他们只是想要文本，他们不在乎为什么它不起作用。在这种情况下，返回一个选项可能更简单，比如这样：

```F#
type ReadTextFromFile = FileInfo -> string option
```

`Result` 是一个用于领域建模的工具，所以如果领域模型不需要它，就不要使用它。

在实现事件溯源时，可以在具有标准签名的命令处理函数中找到类似的示例

```F#
'state -> 'command -> 'event list
```

如果执行命令时出错，这在实践中会如何影响返回值（命令创建的事件列表）？当然，你需要处理错误并记录下来，但你真的需要从函数本身返回一个 `Result` 吗？这将使代码变得更加复杂，但好处不大。

## #6 – 使用 I/O 错误结果时要小心

如果您尝试打开一个文件，但遇到错误，是否应该将其包装在 `Result` 中？这取决于你的领域。如果你正在编写文字处理器，那么无法打开文件是意料之中的，应该妥善处理。另一方面，如果你无法打开你的应用程序所依赖的配置文件，你不应该返回 `Result`，你应该快速失败。

任何有 I/O 的地方都会有很多很多事情出错。试图用一个 `Result` 来模拟所有可能性是很有诱惑力的，但我强烈建议不要这样做。相反，只对域所需的最小值进行建模，让所有其他错误成为例外。

当然，如果你遵循最佳实践，将 I/O 与纯粹的业务逻辑分开，那么你应该很少需要在核心代码中处理异常。

## #7 – 如果你关心性能，不要使用 Result

这与其说是绝对禁止，不如说是“小心”。如果你预先知道你有一段性能敏感的代码，那么在那里使用 `Result` 时要小心。事实上，您可能也要警惕其他内置类型（例如 `List`）。但和往常一样，测量以找到热点，而不是提前猜测，这样你就不会过度优化错误的东西。

## #8 - 如果你关心互操作，不要使用 Result

大多数面向对象语言不理解 `Result` 或其他可区分联合。如果您需要从 API 返回可能的失败，请考虑使用对调用方更惯用的方法。甚至——震惊恐怖——偶尔使用 null。不要强迫调用者成为函数习惯用法方面的专家，这样他们就可以调用 API。

## 不使用结果的原因总结

- **诊断**：如果您关心堆栈跟踪或错误位置，请不要使用 `Result`。
- **重新发明 try/catch**：为什么不使用已经内置的语言工具呢？
- **快速失败**：如果工作流的结束无论如何都会抛出异常，请不要在工作流中使用 `Result`。
- **控制流的本地异常是可以的**：如果控制流复杂且私有，则可以对控制流使用异常。
- **冷漠**：如果没有人关心错误，不要返回 `Result`。
- **I/O**：不要试图用结果来模拟每个可能的 I/O 错误。
- **性能**：如果你关心性能，请谨慎使用 `Result`。
- **互操作**：如果你关心互操作，不要强迫调用者理解 `Result` 是什么以及它是如何工作的。

## 什么时候应该使用 Result？

那么，在所有这些消极因素之后，你应该在什么情况下使用 `Result`？

正如我在《领域建模使函数化》一书中所说，我喜欢将错误分为三类：

- **领域错误**。这些错误是业务流程中预期的一部分，因此必须包含在域的设计中。例如，被账单拒绝的订单，或包含无效产品代码的订单。企业已经有了处理这类事情的程序，因此代码需要反映这些过程。领域错误是领域的一部分，就像其他任何东西一样，因此应该纳入我们的领域建模中，与领域专家讨论，并在可能的情况下记录在类型系统中。请注意，不需要诊断——我们使用 `Result` 作为美化的 `bool`。
- **恐慌**。这些错误会使系统处于未知状态，例如无法处理的系统错误（例如“内存不足”）或程序员疏忽造成的错误（例如，“除以零”、“空引用”）。最好通过放弃工作流并引发异常来处理恐慌，然后在最高适当级别（例如应用程序的 main 函数或等效函数）捕获并记录异常。
- **基础设施错误**。这些错误是架构的一部分，但不属于任何业务流程，也不包含在领域中。例如，网络超时或身份验证失败。有时处理这些问题应该被建模为领域的一部分，有时它们可以被视为恐慌。如有疑问，请咨询领域专家！

因此，使用上述定义：

- `Result` 应仅用作域建模过程的一部分，以记录预期的返回值。然后确保在编译时处理所有可能的预期错误情况。
- 如果合适，微领域（如库）也可以使用 `Result`。

总之，我认为如果使用得当，`Result` 类型和面向铁路的编程是非常有用的，但用例比你想象的要有限，它们不应该仅仅因为很酷和有趣而到处使用。

感谢您的阅读！如果您对更多 F# 帖子感兴趣，请查看 2019 F# 降临日历的其余部分。

> 如果你对领域建模和设计的功能方法感兴趣，你可能会喜欢我的《领域建模函数化》一书！这是一个初学者友好的介绍，涵盖了领域驱动设计、类型建模和函数式编程。



# 面向铁路的编程：碳化版

实现 FizzBuzz 的三种方法
2013年6月22日 这篇文章已有3年多的历史了

https://fsharpforfunandprofit.com/posts/railway-oriented-programming-carbonated/

作为面向铁路编程帖子的后续，我想我会将同样的技术应用于 FizzBuzz 问题，并将其与其他实现进行比较。

这篇文章的很大一部分是直接从 Dave Fayram 在 FizzBuzz 上的帖子中窃取的，还有一些来自拉甘瓦尔德（ [raganwald](http://weblog.raganwald.com/2007/01/dont-overthink-fizzbuzz.html)）的其他想法。

## FizzBuzz：命令式版本

作为提醒，以下是 FizzBuzz 问题的要求：

```
编写一个程序，打印 1 到 100 之间的数字。
* 对于三的倍数，打印“Fizz”而不是数字。
* 对于五的倍数，请打印“Buzz”。
* 对于同时是 3 和 5 的倍数的数字，打印“FizzBuzz”。
```

以下是一个基本的 F# 解决方案：

```F#
module FizzBuzz_Match =

    let fizzBuzz i =
        match i with
        | _ when i % 15 = 0 ->
            printf "FizzBuzz"
        | _ when i % 3 = 0 ->
            printf "Fizz"
        | _ when i % 5 = 0 ->
            printf "Buzz"
        | _ ->
            printf "%i" i

        printf "; "

    // do the fizzbuzz
    [1..100] |> List.iter fizzBuzz
```

我定义了一个函数 `fizzBuzz`，给定一个整数 `i`，它使用 `when` 子句的 `match` 进行各种测试，然后打印出相应的值。

简单明了，适合快速破解，但这种实现存在许多问题。

首先，请注意，我们必须为“十五”提供一个特例。我们不能只是重用“三”和“五”情况下的代码。这意味着，如果我们想添加另一个案例，比如“七”，我们还需要为所有组合添加特殊案例（即“21”、“35”和“105”）。当然，增加更多的数字会导致病例的组合爆炸。

其次，匹配的顺序很重要。如果“十五”个案例在模式列表中排在最后，代码本可以正确运行，但实际上并没有满足要求。同样，如果我们需要添加新案例，我们必须始终记住把最大的案例放在第一位，以确保正确性。这正是导致微妙错误的事情。

让我们尝试另一种实现，在这种实现中，我们重用了“三”和“五”情况下的代码，完全消除了对“十五”情况的需要：

```F#
module FizzBuzz_IfPrime =

    let fizzBuzz i =
        let mutable printed = false

        if i % 3 = 0 then
            printed <- true
            printf "Fizz"

        if i % 5 = 0 then
            printed <- true
            printf "Buzz"

        if not printed then
            printf "%i" i

        printf "; "

    // do the fizzbuzz
    [1..100] |> List.iter fizzBuzz
```

在此实现中，“15”的打印值将是正确的，因为将使用“3”和“5”两种情况。而且我们也不必担心订单——不管怎样，也不必那么担心。

但是，这些分支不再独立，因此我们必须跟踪是否使用了任何分支，以便我们可以处理默认情况。这就产生了可变变量。可变性在 F# 中是一种代码气味，因此这种实现并不理想。

然而，这个版本确实有一个优点，那就是它可以很容易地重构以支持多个因素，而不仅仅是 3 和 5。

下面是一个这样做的版本。我们把一系列“规则”交给 `fizzBuzz`。每个规则都由一个因子和一个要打印的相应标签组成。然后，`fizzBuzz` 函数只是迭代这些规则，依次处理每个规则。

```F#
module FizzBuzz_UsingFactorRules =

    let fizzBuzz rules i  =
        let mutable printed = false

        for factor,label in rules do
            if i % factor = 0 then
                printed <- true
                printf "%s" label

        if not printed then
            printf "%i" i

        printf "; "

    // do the fizzbuzz
    let rules = [ (3,"Fizz"); (5,"Buzz") ]
    [1..100] |> List.iter (fizzBuzz rules)
```

如果我们想处理额外的数字，我们只需将它们添加到规则列表中，如下所示：

```F#
module FizzBuzz_UsingFactorRules =

    // existing code as above

    let rules2 = [ (3,"Fizz"); (5,"Buzz"); (7,"Baz") ]
    [1..105] |> List.iter (fizzBuzz rules2)
```

总之，我们创建了一个非常必要的实现，它在 C# 中几乎是一样的。它很灵活，但可变变量有点代码味。还有别的办法吗？

## FizzBuzz：流水线版本

在下一个版本中，我们将研究使用“管道”模型，在该模型中，数据通过一系列函数馈送以获得最终结果。

在这个设计中，我设想了一个函数管道，一个用于处理“三”类情况，一个处理“五”类情况等等。最后，适当的标签被吐出，准备打印。

以下是一些伪代码来演示这个概念：

```F#
data |> handleThreeCase |> handleFiveCase |> handleAllOtherCases |> printResult
```

作为额外要求，我们希望管道没有副作用。这意味着中间函数不得打印任何内容。相反，他们必须将任何生成的标签沿管道传递到末端，并仅在该点打印结果。

### 设计管道

作为第一步，我们需要定义哪些数据将被馈送到管道中。

让我们从第一个函数开始，在上面的伪代码中称为 `handleThreeCase`。它的输入是什么，输出是什么？

显然，输入是正在处理的整数。但如果幸运的话，输出可能是字符串“Fizz”。或者，如果不是，则使用原始整数。

现在让我们考虑第二个函数 `handleFiveCase` 的输入。它也需要整数。但在“15”的情况下，它也需要字符串“Fizz”，因此它可以向其附加“Buzz”。

最后，`handleAllOtherCases` 函数将 int 转换为字符串，但前提是“Fizz”或“Buzz”尚未生成。

很明显，数据结构需要同时包含正在处理的整数和“迄今为止的标签”。

下一个问题是：我们如何知道上游函数是否创建了标签？`handleAllOtherCases` 需要知道这一点，以便确定它是否需要做任何事情。

一种方法是使用空字符串（或者，可怕的是，使用空字符串），但让我们做好准备，使用 `string option`。

所以，这是我们将使用的最终数据类型：

```F#
type Data = {i:int; label:string option}
```

### 管道版本 1

有了这个数据结构，我们可以定义 `handleThreeCase` 和 `handleFiveCase` 的工作方式。

- 首先，测试输入 int `i`，看看它是否能被因子整除。
- 如果它是可分割的，看看 `label`——如果它是 `None`，那么用 `Some "Fizz"` 或 `Some "Buzz"` 替换它。
- 如果标签已经有值，则向其附加“Buzz”（或其他）。
- 如果输入不能被因子整除，只需原封不动地传递数据。

考虑到这种设计，下面是实现。这是一个通用函数，我将称之为 `carbonate`（以 raganwald 命名），因为它同时适用于“Fizz”和“Buzz”：

```F#
let carbonate factor label data =
    let {i=i; label=labelSoFar} = data
    if i % factor = 0 then
        // pass on a new data record
        let newLabel =
            match labelSoFar with
            | Some s -> s + label
            | None -> label
        {data with label=Some newLabel}
    else
        // pass on the unchanged data
        data
```

`handleAllOtherCases` 函数的设计略有不同：

- 看看标签——如果它不是 `None`，那么前面的函数已经创建了一个标签，所以什么都不做。
- 但如果标签为 `None`，则将其替换为整数的字符串表示形式。

这是代码——我将其称为 `labelOrDefault`：

```F#
let labelOrDefault data =
    let {i=i; label=labelSoFar} = data
    match labelSoFar with
    | Some s -> s
    | None -> sprintf "%i" i
```

现在我们有了组件，我们可以组装管道了：

```F#
let fizzBuzz i =
    {i=i; label=None}
    |> carbonate 3 "Fizz"
    |> carbonate 5 "Buzz"
    |> labelOrDefault     // convert to string
    |> printf "%s; "      // print
```

请注意，我们必须使用 `{i=i; label=None}` 创建一个初始记录，以传递给第一个函数（`carbonate 3 "Fizz"`）。

最后，这是所有代码的组合：

```F#
module FizzBuzz_Pipeline_WithRecord =

    type Data = {i:int; label:string option}

    let carbonate factor label data =
        let {i=i; label=labelSoFar} = data
        if i % factor = 0 then
            // pass on a new data record
            let newLabel =
                match labelSoFar with
                | Some s -> s + label
                | None -> label
            {data with label=Some newLabel}
        else
            // pass on the unchanged data
            data

    let labelOrDefault data =
        let {i=i; label=labelSoFar} = data
        match labelSoFar with
        | Some s -> s
        | None -> sprintf "%i" i

    let fizzBuzz i =
        {i=i; label=None}
        |> carbonate 3 "Fizz"
        |> carbonate 5 "Buzz"
        |> labelOrDefault     // convert to string
        |> printf "%s; "      // print

    [1..100] |> List.iter fizzBuzz
```

### 管道版本 2

创建新的记录类型作为一种文档形式可能很有用，但在这种情况下，只使用元组而不是创建特殊的数据结构可能更符合习惯。

所以这里有一个使用元组的修改实现。

```F#
module FizzBuzz_Pipeline_WithTuple =

    // type Data = int * string option

    let carbonate factor label data =
        let (i,labelSoFar) = data
        if i % factor = 0 then
            // pass on a new data record
            let newLabel =
                labelSoFar
                |> Option.map (fun s -> s + label)
                |> defaultArg <| label
            (i,Some newLabel)
        else
            // pass on the unchanged data
            data

    let labelOrDefault data =
        let (i,labelSoFar) = data
        labelSoFar
        |> defaultArg <| sprintf "%i" i

    let fizzBuzz i =
        (i,None)   // use tuple instead of record
        |> carbonate 3 "Fizz"
        |> carbonate 5 "Buzz"
        |> labelOrDefault     // convert to string
        |> printf "%s; "      // print

    [1..100] |> List.iter fizzBuzz
```

作为练习，试着找到所有必须更改的代码。

### 取消对 Some 和 None 的显式测试

在上面的元组代码中，我还用内置的 Option 函数、`map` 和 `defaultArg` 替换了显式的 Option 匹配代码 `match .. Some .. None`。

以下是 `carbonate` 的变化：

```F#
// before
let newLabel =
    match labelSoFar with
    | Some s -> s + label
    | None -> label

// after
let newLabel =
    labelSoFar
    |> Option.map (fun s -> s + label)
    |> defaultArg <| label
```

在 `labelOrDefault` 中：

```F#
// before
match labelSoFar with
| Some s -> s
| None -> sprintf "%i" i

// after
labelSoFar
|> defaultArg <| sprintf "%i" i
```

您可能想知道外观奇怪的 `|> defaultArg <|` 习语。

我使用它是因为该选项是 `defaultArg` 的第一个参数，而不是第二个参数，因此正常的部分应用程序无法工作。但“双向”管道确实有效，因此代码看起来很奇怪。

我的意思是：

```F#
// OK - normal usage
defaultArg myOption defaultValue

// ERROR: piping doesn't work
myOption |> defaultArg defaultValue

// OK - bi-directional piping does work
myOption |> defaultArg <| defaultValue
```

### 管道版本 3

我们的 `carbonate` 函数对任何因素都是通用的，因此我们可以很容易地扩展代码以支持“规则”，就像早期的命令式版本一样。

但有一个问题似乎是，我们已经将“3”和“5”案例硬编码到管道中，如下所示：

```F#
|> carbonate 3 "Fizz"
|> carbonate 5 "Buzz"
```

我们如何动态地将新函数添加到管道中？

答案很简单。我们为每个规则动态创建一个函数，然后使用组合将所有这些函数组合成一个。

下面是一个片段来演示：

```F#
let allRules =
    rules
    |> List.map (fun (factor,label) -> carbonate factor label)
    |> List.reduce (>>)
```

每个规则都映射到一个函数中。然后使用 `>>` 将函数列表组合成一个函数。

综合起来，我们得到了最终的实现：

```F#
module FizzBuzz_Pipeline_WithRules =

    let carbonate factor label data =
        let (i,labelSoFar) = data
        if i % factor = 0 then
            // pass on a new data record
            let newLabel =
                labelSoFar
                |> Option.map (fun s -> s + label)
                |> defaultArg <| label
            (i,Some newLabel)
        else
            // pass on the unchanged data
            data

    let labelOrDefault data =
        let (i,labelSoFar) = data
        labelSoFar
        |> defaultArg <| sprintf "%i" i

    let fizzBuzz rules i =

        // create a single function from all the rules
        let allRules =
            rules
            |> List.map (fun (factor,label) -> carbonate factor label)
            |> List.reduce (>>)

        (i,None)
        |> allRules
        |> labelOrDefault     // convert to string
        |> printf "%s; "      // print

    // test
    let rules = [ (3,"Fizz"); (5,"Buzz"); (7,"Baz") ]
    [1..105] |> List.iter (fizzBuzz rules)
```

将这个“管道”版本与之前的命令式版本进行比较，设计功能要强大得多。没有可变性，也没有任何副作用（除了最后的 `printf` 语句）。

然而，在使用 `List.reduce` 时存在一个微妙的错误。你能看到它是什么吗？**有关问题和修复的讨论，请参阅本页底部的附言。

**提示：尝试一个空的规则列表。

## FizzBuzz：面向铁路的版本

管道版本是 FizzBuzz 的完美功能实现，但为了好玩，让我们看看我们是否可以使用面向铁路的编程文章中描述的“双轨”设计。

作为一个快速提醒，在“面向铁路的编程”（也称为“Either”单子）中，我们定义了一个有两种情况的联合类型：“成功”和“失败”，每种情况都代表一个不同的“轨道”。然后，我们将一组“双轨”功能连接在一起，形成铁路。

我们实际使用的大多数函数都是我所说的“开关”或“点”函数，有一个单轨输入，但有一个双轨输出，一个用于成功情况，一个适用于失败情况。这些开关函数使用称为“bind”的粘合函数转换为双轨函数。

这是一个包含我们需要的函数定义的模块。

```F#
module RailwayCombinatorModule =

    let (|Success|Failure|) =
        function
        | Choice1Of2 s -> Success s
        | Choice2Of2 f -> Failure f

    /// convert a single value into a two-track result
    let succeed x = Choice1Of2 x

    /// convert a single value into a two-track result
    let fail x = Choice2Of2 x

    // apply either a success function or failure function
    let either successFunc failureFunc twoTrackInput =
        match twoTrackInput with
        | Success s -> successFunc s
        | Failure f -> failureFunc f

    // convert a switch function into a two-track function
    let bind f =
        either f fail
```

我在这里使用的是 `Choice` 类型，它内置于 F# 核心库中。但我创建了一些助手，使其看起来像成功/失败类型：一个活动模式和两个构造函数。

现在，我们将如何使 FizzBuzz 适应这种情况？

让我们从一件显而易见的事情开始：将“carbonation”定义为成功，将一个未匹配的整数定义为失败。

换句话说，成功轨道包含标签，失败轨道包含整数。

因此，我们的 `carbonate` “开关”功能看起来像这样：

```F#
let carbonate factor label i =
    if i % factor = 0 then
        succeed label
    else
        fail i
```

这个实现类似于上面讨论的管道设计中使用的实现，但它更清晰，因为输入只是一个 int，而不是记录或元组。

接下来，我们需要将组件连接在一起。逻辑是：

- 如果 int 已经碳化了，忽略它
- 如果 int 没有碳化，请将其连接到下一个开关函数的输入端

以下是实现：

```F#
let connect f =
    function
    | Success x -> succeed x
    | Failure i -> f i
```

另一种写作方式是使用我们在库模块中定义的 `either` 函数：

```F#
let connect' f =
    either succeed f
```

确保你明白这两个实现做的是完全相同的事情！

接下来，我们可以创建我们的“双轨”管道，如下所示：

```F#
let fizzBuzz =
    carbonate 15 "FizzBuzz"      // need the 15-FizzBuzz rule because of short-circuit
    >> connect (carbonate 3 "Fizz")
    >> connect (carbonate 5 "Buzz")
    >> either (printf "%s; ") (printf "%i; ")
```

这表面上类似于“单轨”管道，但实际上使用了不同的技术。开关通过组合（`>>`）而不是管道（`|>`）连接在一起。

因此，`fizzBuzz` 函数没有 int 参数——我们通过组合其他函数来定义一个函数。任何地方都没有数据。

其他一些事情也发生了变化：

- 我们不得不重新引入对“15”案例的明确测试。这是因为我们只有两条轨道（成功或失败）。没有“半成品轨道”允许“5”案例添加到“3”案例的输出中。
- 前面示例中的 `labelOrDefault` 函数已被替换为 `either`。在成功的情况下，会打印一个字符串。在Failure的情况下，会打印一个 int。

以下是完整的实现：

```F#
module FizzBuzz_RailwayOriented_CarbonationIsSuccess =

    open RailwayCombinatorModule

    // carbonate a value
    let carbonate factor label i =
        if i % factor = 0 then
            succeed label
        else
            fail i

    let connect f =
        function
        | Success x -> succeed x
        | Failure i -> f i

    let connect' f =
        either succeed f

    let fizzBuzz =
        carbonate 15 "FizzBuzz"      // need the 15-FizzBuzz rule because of short-circuit
        >> connect (carbonate 3 "Fizz")
        >> connect (carbonate 5 "Buzz")
        >> either (printf "%s; ") (printf "%i; ")

    // test
    [1..100] |> List.iter fizzBuzz
```

### 碳化是失败吗？

在上面的例子中，我们将碳化定义为“成功”——当然，这似乎是很自然的事情。但如果你还记得，在面向铁路的编程模型中，“成功”意味着数据应该传递给下一个函数，而“失败”意味着绕过所有中间函数，直接走到最后。

对于 FizzBuzz 来说，“绕过所有中间函数”轨道是带有碳化标签的轨道，而“传递给下一个函数”轨道则是带有整数的轨道。

因此，我们应该真正扭转这种趋势：“失败”现在意味着碳化，而“成功”意味着没有碳化。

通过这样做，我们还可以重用预定义的绑定函数，而不必编写自己的连接函数。

以下是轨道切换后的代码：

```F#
module FizzBuzz_RailwayOriented_CarbonationIsFailure =

    open RailwayCombinatorModule

    // carbonate a value
    let carbonate factor label i =
        if i % factor = 0 then
            fail label
        else
            succeed i

    let fizzBuzz =
        carbonate 15 "FizzBuzz"
        >> bind (carbonate 3 "Fizz")
        >> bind (carbonate 5 "Buzz")
        >> either (printf "%i; ") (printf "%s; ")

    // test
    [1..100] |> List.iter fizzBuzz
```

### 这两条轨道到底是什么？

我们可以如此容易地交换轨道的事实意味着设计中可能存在弱点。我们是否试图使用不合适的设计？

为什么一个轨道必须是“成功”，另一个轨道无论如何都必须是“失败”？这似乎没什么区别。

那么，我们为什么不保持双轨制的想法，但去掉“成功”和“失败”的标签呢。

相反，我们可以称一首轨道为“碳化”，另一首为“未碳化”。

为了实现这一点，我们可以定义一个活动模式和构造函数方法，就像我们为“成功/失败”所做的那样。

```F#
let (|Uncarbonated|Carbonated|) =
    function
    | Choice1Of2 u -> Uncarbonated u
    | Choice2Of2 c -> Carbonated c

/// convert a single value into a two-track result
let uncarbonated x = Choice1Of2 x
let carbonated x = Choice2Of2 x
```

如果你正在进行领域驱动的设计，最好使用适当的统一语言（ [Ubiquitous Language](http://martinfowler.com/bliki/UbiquitousLanguage.html) ）而不是不适用的语言编写代码。

在这种情况下，如果 FizzBuzz 是我们的领域，那么我们的函数现在可以使用对领域友好的术语，即碳化和非酸化，而不是“成功”或“失败”。

```F#
let carbonate factor label i =
    if i % factor = 0 then
        carbonated label
    else
        uncarbonated i

let connect f =
    function
    | Uncarbonated i -> f i
    | Carbonated x -> carbonated x
```

请注意，与之前一样，`connect` 函数可以使用 `either` 重写（或者我们可以像以前一样使用预定义的 `bind`）：

```F#
let connect' f =
    either f carbonated
```

以下是一个模块中的所有代码：

```F#
module FizzBuzz_RailwayOriented_UsingCustomChoice =

    open RailwayCombinatorModule

    let (|Uncarbonated|Carbonated|) =
        function
        | Choice1Of2 u -> Uncarbonated u
        | Choice2Of2 c -> Carbonated c

    /// convert a single value into a two-track result
    let uncarbonated x = Choice1Of2 x
    let carbonated x = Choice2Of2 x

    // carbonate a value
    let carbonate factor label i =
        if i % factor = 0 then
            carbonated label
        else
            uncarbonated i

    let connect f =
        function
        | Uncarbonated i -> f i
        | Carbonated x -> carbonated x

    let connect' f =
        either f carbonated

    let fizzBuzz =
        carbonate 15 "FizzBuzz"
        >> connect (carbonate 3 "Fizz")
        >> connect (carbonate 5 "Buzz")
        >> either (printf "%i; ") (printf "%s; ")

    // test
    [1..100] |> List.iter fizzBuzz
```

### 添加规则

到目前为止，我们的版本存在一些问题：

- “15”测试是丑陋的。我们能否摆脱它，重新使用“3”和“5”案例？
- “3”和“5”案例是硬编码的。我们能让它更有动态吗？

碰巧的是，我们可以一箭双雕，同时解决这两个问题。

我们可以并行地将它们“添加”在一起，而不是将所有“开关”功能串联组合在一起。在面向铁路的编程岗位上，我们使用了这种技术来组合验证函数。对于 FizzBuzz，我们将使用它来同时处理所有因素。

诀窍是定义一个“append”或“concat”函数来组合两个函数。一旦我们可以以这种方式添加两个函数，我们就可以继续添加任意数量的函数。

那么，考虑到我们有两个碳化功能，我们如何将它们结合起来呢？

以下是可能的情况：

- 如果它们都有碳化输出，我们将标签连接成一个新的碳化标签。
- 如果一个有碳化输出而另一个没有，那么我们使用碳化输出。
- 如果两者都没有碳化输出，那么我们使用未碳化输出（它们将是相同的）。

代码如下：

```F#
// concat two carbonation functions
let (<+>) switch1 switch2 x =
    match (switch1 x),(switch2 x) with
    | Carbonated s1,Carbonated s2 -> carbonated (s1 + s2)
    | Uncarbonated f1,Carbonated s2  -> carbonated s2
    | Carbonated s1,Uncarbonated f2 -> carbonated s1
    | Uncarbonated f1,Uncarbonated f2 -> uncarbonated f1
```

顺便说一句，请注意，这段代码几乎就像数学，`uncarbonated` 扮演“零”的角色，如下所示：

```
something + something = combined somethings
zero + something = something
something + zero = something
zero + zero = zero
```

这不是巧合！我们会看到这种东西在函数式代码中反复出现。我将在以后的文章中讨论这个问题。

不管怎样，有了这个“concat”函数，我们可以这样重写主 `buzzBuzz`：

```F#
let fizzBuzz =
    let carbonateAll =
        carbonate 3 "Fizz" <+> carbonate 5 "Buzz"

    carbonateAll
    >> either (printf "%i; ") (printf "%s; ")
```

添加这两种 `carbonate` 函数，然后像以前一样传递给其中一种。

以下是完整的代码：

```F#
module FizzBuzz_RailwayOriented_UsingAppend =

    open RailwayCombinatorModule

    let (|Uncarbonated|Carbonated|) =
        function
        | Choice1Of2 u -> Uncarbonated u
        | Choice2Of2 c -> Carbonated c

    /// convert a single value into a two-track result
    let uncarbonated x = Choice1Of2 x
    let carbonated x = Choice2Of2 x

    // concat two carbonation functions
    let (<+>) switch1 switch2 x =
        match (switch1 x),(switch2 x) with
        | Carbonated s1,Carbonated s2 -> carbonated (s1 + s2)
        | Uncarbonated f1,Carbonated s2  -> carbonated s2
        | Carbonated s1,Uncarbonated f2 -> carbonated s1
        | Uncarbonated f1,Uncarbonated f2 -> uncarbonated f1

    // carbonate a value
    let carbonate factor label i =
        if i % factor = 0 then
            carbonated label
        else
            uncarbonated i

    let fizzBuzz =
        let carbonateAll =
            carbonate 3 "Fizz" <+> carbonate 5 "Buzz"

        carbonateAll
        >> either (printf "%i; ") (printf "%s; ")

    // test
    [1..100] |> List.iter fizzBuzz
```

有了这个添加逻辑，我们可以很容易地重构代码以使用规则。就像早期的“管道”实现一样，我们可以使用 `reduce` 一次将所有规则添加在一起。

以下是带有规则的版本：

```F#
module FizzBuzz_RailwayOriented_UsingAddition =

    // code as above

    let fizzBuzzPrimes rules =
        let carbonateAll  =
            rules
            |> List.map (fun (factor,label) -> carbonate factor label)
            |> List.reduce (<+>)

        carbonateAll
        >> either (printf "%i; ") (printf "%s; ")

    // test
    let rules = [ (3,"Fizz"); (5,"Buzz"); (7,"Baz") ]
    [1..105] |> List.iter (fizzBuzzPrimes rules)
```

## 摘要

在这篇文章中，我们看到了三种不同的实现：

- 一种使用可变值和逻辑混合副作用的命令式版本。
- 通过一系列函数传递数据结构的“管道”版本。
- 一个“面向铁路”的版本，有两条单独的轨道，并使用“加法”并行组合功能。

在我看来，命令式版本是最糟糕的设计。尽管它很容易快速讨巧写出（hack out），但它存在许多问题，使其变得脆弱且容易出错。

在这两个函数式版本中，我认为“面向铁路”的版本更清晰，至少在这个问题上是这样。

通过使用 `Choice` 类型而不是元组或特殊记录，我们使代码在整个过程中更加优雅。如果你比较管道版的碳化和铁路版的碳化，你可以看到区别。

当然，在其他情况下，面向铁路的方法可能不起作用，管道方法可能更好。我希望这篇文章对两者都有一些有益的见解。

如果你是 FizzBuzz 的粉丝，请查看函数式响应式编程页面，该页面还有另一个问题变体。

## 后记：使用 List.reduce 时要小心

小心 `List.reduce`——它会在空列表时失败。因此，如果你有一个空的规则集，代码将抛出一个 `System.ArgumentException`。

在管道的情况下，您可以通过将以下代码段添加到模块中来看到这一点：

```F#
module FizzBuzz_Pipeline_WithRules =

    // code as before

    // bug
    let emptyRules = []
    [1..105] |> List.iter (fizzBuzz emptyRules)
```

修复方法是将 `List.reduce` 替换为 `List.fold`。`List.fold` 需要一个额外的参数：初始值（或“零”）。在这种情况下，我们可以使用单位元函数(identity function) `id` 作为初始值。

以下是代码的修复版本：

```F#
let allRules =
    rules
    |> List.map (fun (factor,label) -> carbonate factor label)
    |> List.fold (>>) id
```

同样，在面向铁路的示例中，我们有：

```F#
let allRules =
    rules
    |> List.map (fun (factor,label) -> carbonate factor label)
    |> List.reduce (<+>)
```

应将其修正为：

```F#
let allRules =
    rules
    |> List.map (fun (factor,label) -> carbonate factor label)
    |> List.fold (<+>) zero
```

其中 `zero` 是列表为空时使用的“默认”函数。

作为练习，为这种情况定义零函数。（提示：我们实际上已经用另一个名称定义了它）。



# C# 程序员的 F#

https://fsharpforfunandprofit.com/csharp/

本页包含我演讲“F# for C# 程序员”的幻灯片和视频链接。

以下是演讲的简介：

> 对 F# 感到好奇，想了解它与 C# 有何不同？

在本次演讲中，我们将探讨 F# 编码的基础知识，以及函数式编程与面向对象编程的区别。在此过程中，将有许多示例显示用 C# 和 F# 编写的相同代码，以便您自己看到这两种语言在风格和方法上的差异。

## 视频

奥斯陆 NDC 视频，2017年6月14日（点击图片查看视频）

奥斯陆 NDC 视频，2017年6月14日

## 幻灯片

2017年6月14日，奥斯陆 NDC 幻灯片

我在 Slideshare 上的幻灯片中为 C# 程序员提供的 F#

## 相关讲座

如果你喜欢这个，这些主题在其他一些演讲和帖子中得到了扩展：

- 函数式编程设计模式（讲座）
- F# 类型系统的领域驱动设计（演讲）
- 你的编程语言不合理吗？（关于可预测性的帖子）
- C# Light（幻灯片）



# “表达式和语法”系列

https://fsharpforfunandprofit.com/series/expressions-and-syntax/

在本系列文章中，我们将探讨函数和值是如何组合成表达式的，以及F#中可用的不同类型的表达式。

1. 表达式和语法：引言
   如何用 F# 编码
2. 表达式与语句
   为什么表达式更安全，并成为更好的构建块
3. F# 表达式概述
   控制流、let、dos等
4. 用 let、use 和 do 绑定
   如何使用它们
5. F# 语法：缩进和冗长
   理解越位规则
6. 参数和值命名约定
   a、 f、x和朋友
7. 控制流表达式
   以及如何避免使用它们
8. 异常
   抛出和捕获的语法
9. 匹配表达式
   F# 的主力
10. 使用 printf 格式化文本
    打印和记录的技巧和技术
11. 工作示例：解析命令行参数
    实践中的模式匹配
12. 工作示例：罗马数字
    实践中更多的模式匹配



## [跳转系列独立 Markdown](./FSharpForFunAndProfit翻译-“表达式和语法”系列.md)



# “了解 F# 类型”系列

https://fsharpforfunandprofit.com/series/understanding-fsharp-types/

F# 不仅仅是函数；强大的类型系统是另一个关键因素。就像函数一样，理解类型系统对于流利和舒适地使用语言至关重要。

除了普通 .NET 类型。F# 还有一些其他类型，这些类型在函数式语言中非常常见，但在 C# 或 Java 等命令式语言中不可用。

本系列介绍这些类型以及如何使用它们。

1. 了解 F# 类型：介绍
   类型的新世界
2. F# 中的类型概述
   纵观全局
3. 类型缩写
   也称为别名
4. 元组
   将类型相乘
5. 记录
   用标签扩展元组
6. 可区分联合
   将类型添加在一起
7. 选项类型
   为什么它不是 null 或 nullable
8. 枚举类型
   与联合类型不同
9. 内置 .NET 类型
   Int、string、bool 等
10. 计量单位
    数字的类型安全
11. 理解类型推理
    魔法帘子后面



## [跳转系列独立 Markdown](./FSharpForFunAndProfit翻译-“了解F#类型”系列.md)



# 在集合函数之间进行选择

困惑者的指南
14八月2015 这篇文章是超过3年

https://fsharpforfunandprofit.com/posts/list-module-functions/

学习一门新语言不仅仅是语言本身。为了提高效率，你需要记住标准库的大部分内容，并了解其余的大部分内容。例如，如果你知道 C#，你可以很快掌握 Java 语言，但只有当你也熟悉 Java 类库时，你才能真正掌握它。

同样，在熟悉所有与集合一起工作的 F# 函数之前，你无法真正有效地使用 F#。

在 C# 中，你只需要知道几个 LINQ 方法^1^（`Select`、`Where` 等）。但在 F# 中，List 模块中目前有近 100 个函数（Seq 和 Array 模块中也有类似的计数）。太多了！

1 是的，还有更多，但你只需要几个就可以应付。在 F# 中，了解所有这些更为重要。

如果你从 C# 转到 F#，那么大量的列表函数可能会让人不知所措。

所以我写这篇文章是为了帮助你找到你想要的。为了好玩，我以“选择你自己的冒险”的风格做了这件事！

## 我想要什么集合？

首先，一个包含不同类型标准集合信息的表格。有五种“原生” F#：`list`、`seq`、`array`、`map` 和 `set`，也经常使用 `ResizeArray` 和 `IDictionary`。

|             | 不可变? | 说明                                                         |
| :---------- | :------ | :----------------------------------------------------------- |
| list        | Yes     | **优点：**<br />- 提供模式匹配。<br />- 通过递归可以进行复杂的迭代。<br />- 正向迭代很快。前插入很快。<br />**缺点：**<br />- 索引访问和其他访问方式很慢。 |
| seq         | Yes     | `IEnumerable` 的别名。<br />**优点：**<br />- 延迟计算<br />- 内存效率高（一次只加载一个元素）<br />- 可以表示无限序列。<br />- 与使用 IEnumerable 的 .NET 库互操作。<br />**缺点：**<br />- 没有模式匹配。<br />- 仅向前迭代。<br />- 索引访问和其他访问方式很慢。 |
| array       | No      | 与 BCL 的 `Array` 相同。<br />**优点：**<br />- 快速随机访问<br />- 内存效率和缓存位置，特别是在结构体中。<br />- 与使用 Array 的 .NET 库互操作。<br />- 支持 2D、3D 和 4D 数组<br />**缺点：**<br />- 模式匹配有限。<br />- 不[持久](https://en.wikipedia.org/wiki/Persistent_data_structure). |
| map         | Yes     | 不可变词典。需要 key 实现 `IComparable`。                    |
| set         | Yes     | 不可变集合。要求元素实现 `IComparable`。                     |
| ResizeArray | No      | BCL `List` 的别名。优点和缺点类似于数组，但可以调整大小。    |
| IDictionary | Yes     | 对于不需要元素实现 `IComparable` 的备用字典，可以使用 BCL [IDictionary](https://msdn.microsoft.com/en-us/library/s4ys34ea.aspx). 构造函数是在 F# 中的 [`dict`](https://msdn.microsoft.com/en-us/library/ee353774.aspx)。<br />请注意，存在诸如 `Add` 之类的变异方法，但如果调用，将导致运行时错误。 |

这些是您在 F# 中会遇到的主要集合类型，对于所有常见情况都足够好。

如果你需要其他类型的集合，有很多选择：

- 您可以在中使用 .NET 中的集合类，无论是传统的、可变的还是较新的，如 [System.Collections.Immutable 命名空间](https://msdn.microsoft.com/en-us/library/system.collections.immutable.aspx)中的那些。
- 或者，您可以使用 F# 集合库之一：
  - [**FSharpx.Collections**](https://fsprojects.github.io/FSharpx.Collections/)，FSharpx 系列项目的一部分。
  - [**ExtCore**](https://github.com/jack-pappas/ExtCore/tree/master/ExtCore)。其中一些是 FSharp 中 Map 和 Set 类型的插入式（几乎）替换。在特定场景中提供改进性能的核心（例如 HashMap）。其他一些提供了独特的功能来帮助解决特定的编码任务（例如 LazyList 和 LruCache）。
  - [**Funq**](https://github.com/GregRos/Funq)：.NET 高性能、不可变的数据结构。
  - [**Persistent**](https://persistent.codeplex.com/documentation)：一些高效的持久（不可变）数据结构。

## 关于文档

除非另有说明，否则 F# v4 中的所有函数都可用于 `list`、`seq` 和 `array`。`Map` 和 `Set` 模块也有一些，但我不会在这里讨论 `map` 和 `set`。

对于函数签名，我将使用 `list` 作为标准集合类型。`seq` 和 `array` 版本的签名将是相似的。

其中许多功能尚未在 MSDN 上记录，因此我将直接链接到 GitHub 上的源代码，该代码有最新的注释。单击链接的函数名称。

## 关于可用性的说明

这些函数的可用性可能取决于您使用的 F# 版本。

- 在 F# 版本 3（Visual Studio 2013）中，列表、数组和序列之间存在一定程度的不一致。
- 在 F# 版本 4（Visual Studio 2015）中，这种不一致性已被消除，几乎所有函数都可用于所有三种集合类型。

如果你想知道 F# v3 和 F# v4 之间发生了什么变化，请查看此图表（从这里开始）。该图表显示了 F# v4 中的新 API（绿色）、以前存在的 API（蓝色）和有意留下的空白（白色）。

下面记录的一些功能不在此图表中——这些功能更新了！如果你使用的是旧版本的 F#，你可以简单地使用 GitHub 上的代码自己重新实现它们。

有了这个免责声明，你就可以开始你的冒险了！

## 目录

1. 你们有什么样的集合？
2. 创建新集合
3. 创建新的空集合或单元素集合
4. 创建已知大小的新集合
5. 创建一个已知大小的新集合，每个元素具有相同的值
6. 创建一个已知大小的新集合，每个元素具有不同的值
7. 创建新的无限集合
8. 创建一个不确定大小的新集合
9. 使用一个列表
10. 在已知位置获取元素
11. 通过搜索获取元素
12. 从集合中获取元素子集
13. 分区、组块和分组
14. 汇总或概括一个集合
15. 更改元素顺序
16. 测试集合的元素
17. 将每个元素转化为不同的东西
18. 迭代每个元素
19. 通过迭代线程化状态
20. 使用每个元素的索引
21. 将整个集合转换为不同的集合类型
22. 改变整个集合的行为
23. 与两个集合合作
24. 与三个集合合作
25. 与三个以上的集合合作
26. 合并和取消合并集合
27. 其他仅数组功能
28. 使用一次性用品的序列

## 1.你们有什么样的集合？

你有什么样的集合？

- 如果你没有集合，但想创建一个，请转到第 2 节。
- 如果你已经有一个想要使用的集合，请转到第 9 节。
- 如果你有两个要使用的集合，请转到第 23 节。
- 如果你有三个要使用的集合，请转到第 24 节。
- 如果你想使用三个以上的集合，请转到第 25 节。
- 如果你想合并或取消合并集合，请转到第 26 节。

## 2.创建新集合

所以你想创建一个新的集合。你想如何创建它？

- 如果新集合为空或只有一个元素，请转到第 3 节。
- 如果新集合的大小已知，请转到第 4 节。
- 如果新集合可能是无限的，请转到第 7 节。
- 如果你不知道集合有多大，请参阅第 8 节。

## 3.创建新的空集合或单元素集合

如果要创建新的空集合或单元素集合，请使用以下函数：

- [`empty : 'T list`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L142)。返回给定类型的空列表。
- [`singleton : value:'T -> 'T list`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L635)。返回仅包含一个项目的列表。

如果你提前知道集合的大小，使用不同的函数通常会更有效。见下文第 4 节。

### 使用示例

```F#
let list0 = List.empty
// list0 = []

let list1 = List.singleton "hello"
// list1 = ["hello"]
```

## 4.创建已知大小的新集合

- 如果集合的所有元素都具有相同的值，请转到第 5 节。
- 如果集合的元素可能不同，请转到第 6 节。

## 5.创建一个已知大小的新集合，每个元素具有相同的值

如果你想创建一个已知大小的新集合，每个元素都有相同的值，你想使用 `replicate`：

- [`replicate : count:int -> initial:'T -> 'T list`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L602)。通过复制给定的初始值创建集合。
- （仅数组）[`create : count:int -> value:'T -> 'T[]`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/array.fsi#L125)。创建一个数组，其元素最初都是提供的值。
- （仅数组） [`zeroCreate : count:int -> 'T[]`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/array.fsi#L467)。创建一个数组，其中的条目最初是默认值。

`Array.create` 与 `replicate` 基本相同（尽管实现方式略有不同！），但 `replicate` 仅在 F# v4 中为 Array 实现。

### 使用示例

```F#
let repl = List.replicate 3 "hello"
// val repl : string list = ["hello"; "hello"; "hello"]

let arrCreate = Array.create 3 "hello"
// val arrCreate : string [] = [|"hello"; "hello"; "hello"|]

let intArr0 : int[] = Array.zeroCreate 3
// val intArr0 : int [] = [|0; 0; 0|]

let stringArr0 : string[] = Array.zeroCreate 3
// val stringArr0 : string [] = [|null; null; null|]
```

请注意，对于 `zeroCreate`，编译器必须知道目标类型。

## 6.创建一个已知大小的新集合，每个元素具有不同的值

如果你想创建一个已知大小的新集合，每个元素都有一个可能不同的值，你可以选择以下三种方式之一：

- [`init : length:int -> initializer:(int -> 'T) -> 'T list`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L347)。通过在每个索引上调用给定的生成器来创建集合。
- 对于列表和数组，您还可以使用字面语法，如 `[1; 2; 3]`（列表）和`[|1; 2; 3|]`（数组）。
- 对于列表、数组和 seqs，您可以使用 `for .. in .. do .. yield` 的理解语法。

### 使用示例

```F#
// using list initializer
let listInit1 = List.init 5 (fun i-> i*i)
// val listInit1 : int list = [0; 1; 4; 9; 16]

// using list comprehension
let listInit2 = [for i in [1..5] do yield i*i]
// val listInit2 : int list = [1; 4; 9; 16; 25]

// literal
let listInit3 = [1; 4; 9; 16; 25]
// val listInit3 : int list = [1; 4; 9; 16; 25]

let arrayInit3 = [|1; 4; 9; 16; 25|]
// val arrayInit3 : int [] = [|1; 4; 9; 16; 25|]
```

字面语法也允许增量：

```F#
// literal with +2 increment
let listOdd= [1..2..10]
// val listOdd : int list = [1; 3; 5; 7; 9]
```

理解语法更加灵活，因为你可以多次 `yield`：

```F#
// using list comprehension
let listFunny = [
    for i in [2..3] do
        yield i
        yield i*i
        yield i*i*i
        ]
// val listFunny : int list = [2; 4; 8; 3; 9; 27]
```

它也可以用作快速和脏的内联过滤器：

```F#
let primesUpTo n =
   let rec sieve l  =
      match l with
      | [] -> []
      | p::xs ->
            p :: sieve [for x in xs do if (x % p) > 0 then yield x]
   [2..n] |> sieve

primesUpTo 20
// [2; 3; 5; 7; 11; 13; 17; 19]
```

另外两个技巧：

- 你可以使用 `yield!` 返回列表而不是单个值
- 您还可以使用递归

以下是两种技巧用于数到 10 乘 2 的示例：

```F#
let rec listCounter n = [
    if n <= 10 then
        yield n
        yield! listCounter (n+2)
    ]

listCounter 3
// val it : int list = [3; 5; 7; 9]
listCounter 4
// val it : int list = [4; 6; 8; 10]
```

## 7.创建新的无限集合

如果你想要一个无限的列表，你必须使用 seq 而不是列表或数组。

- [`initInfinite : initializer:(int -> 'T) -> seq<'T>`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/seq.fsi#L599)。生成一个新的序列，当迭代时，它将通过调用给定的函数返回连续的元素。
- 您还可以使用带有递归循环的 seq 理解来生成无限序列。

### 使用示例

```F#
// generator version
let seqOfSquares = Seq.initInfinite (fun i -> i*i)
let firstTenSquares = seqOfSquares |> Seq.take 10

firstTenSquares |> List.ofSeq // [0; 1; 4; 9; 16; 25; 36; 49; 64; 81]

// recursive version
let seqOfSquares_v2 =
    let rec loop n = seq {
        yield n * n
        yield! loop (n+1)
        }
    loop 1
let firstTenSquares_v2 = seqOfSquares_v2 |> Seq.take 10
```

## 8.创建一个不确定大小的新集合

有时你事先不知道集合有多大。在这种情况下，您需要一个函数，该函数将不断添加元素，直到收到停止的信号。`unfold` 是你在这里的朋友，而“停止的信号”是你返回“`None`”（停止）还是“`Some`”（继续）。

- [`unfold : generator:('State -> ('T * 'State) option) -> state:'State -> 'T list`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L846)。返回一个包含给定计算生成的元素的集合。

### 使用示例

此示例在循环中从控制台读取，直到输入空行：

```F#
let getInputFromConsole lineNo =
    let text = System.Console.ReadLine()
    if System.String.IsNullOrEmpty(text) then
        None
    else
        // return value and new threaded state
        // "text" will be in the generated sequence
        Some (text,lineNo+1)

let listUnfold = List.unfold getInputFromConsole 1
```

`unfold` 需要一个贯穿整个生成器的状态。您可以忽略它（如上面的 `ReadLine` 示例），也可以使用它来跟踪您迄今为止所做的工作。例如，您可以使用 `unfold` 创建斐波那契数列生成器：

```F#
let fibonacciUnfolder max (f1,f2)  =
    if f1 > max then
        None
    else
        // return value and new threaded state
        let fNext = f1 + f2
        let newState = (f2,fNext)
        // f1 will be in the generated sequence
        Some (f1,newState)

let fibonacci max = List.unfold (fibonacciUnfolder max) (1,1)
fibonacci 100
// int list = [1; 1; 2; 3; 5; 8; 13; 21; 34; 55; 89]
```

## 9.使用一个列表

如果你正在处理一个列表，并且…

- 如果你想在已知位置得到一个元素，请转到第 10 节
- 如果你想通过搜索得到一个元素，请转到第 11 节
- 如果你想获得集合的一个子集，请转到第 12 节
- 如果你想将一个集合分区、块化或分组为更小的集合，请转到第 13 节
- 如果要将集合聚合或汇总为单个值，请转到第 14 节
- 如果你想改变元素的顺序，请转到第 15 节
- 如果你想测试集合中的元素，请转到第 16 节
- 如果你想将每个元素转换为不同的东西，请转到第 17 节
- 如果你想迭代每个元素，请转到第 18 节
- 如果你想在迭代中线程化状态，请转到第 19 节
- 如果在迭代或映射时需要知道每个元素的索引，请转到第 20 节
- 如果你想将整个集合转换为不同的集合类型，请转到第 21 节
- 如果你想改变整个集合的行为，请转到第 22 节
- 如果你想在原地修改集合，请转到第 27 节
- 如果要将惰性集合与 IDisposable 一起使用，请转到第 28 节

## 10.在已知位置获取元素

以下函数按位置获取集合中的元素：

- [`head : list:'T list -> 'T`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L333)。返回集合的第一个元素。
- [`last : list:'T list -> 'T`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L398)。返回集合的最后一个元素。
- [`item : index:int -> list:'T list -> 'T`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L520)。索引到集合中。第一个元素的索引为 0。
  注意：避免在列表和序列中使用 `nth` 和 `item`。它们不是为随机访问而设计的，因此通常会很慢。
- [`nth : list:'T list -> index:int -> 'T`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L520)。`item` 的旧版本。
  注意：v4 中已弃用-请改用 `item`。
- （仅限数组）[`get : array:'T[] -> index:int -> 'T`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/array.fsi#L220)。这是 `item` 的另一个版本。
- [`exactlyOne : list:'T list -> 'T`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L165)。返回集合中唯一的元素。

但是，如果收藏是空的呢？然后 `head` 和 `last` 将失败，并出现异常（ArgumentException）。

如果在集合中找不到索引呢？然后再次出现另一个异常（列表为 ArgumentException，数组为 IndexOutOfRangeException）。

因此，我建议您一般避免使用这些函数，并使用下面的 `tryXXX` 等效函数：

- [`tryHead : list:'T list -> 'T option`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L775)。返回集合的第一个元素，如果集合为空，则返回 None。
- [`tryLast : list:'T list -> 'T option`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L411)。返回集合的最后一个元素，如果集合为空，则返回 None。
- [`tryItem : index:int -> list:'T list -> 'T option`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L827)。对集合进行索引，如果索引无效，则为 None。

### 使用示例

```F#
let head = [1;2;3] |> List.head
// val head : int = 1

let badHead : int = [] |> List.head
// System.ArgumentException: The input list was empty.

let goodHeadOpt =
    [1;2;3] |> List.tryHead
// val goodHeadOpt : int option = Some 1

let badHeadOpt : int option =
    [] |> List.tryHead
// val badHeadOpt : int option = None

let goodItemOpt =
    [1;2;3] |> List.tryItem 2
// val goodItemOpt : int option = Some 3

let badItemOpt =
    [1;2;3] |> List.tryItem 99
// val badItemOpt : int option = None
```

如前所述，列表应避免使用 `item` 功能。例如，如果你想处理列表中的每个项目，并且你来自命令式背景，你可以编写一个类似这样的循环：

```F#
// Don't do this!
let helloBad =
    let list = ["a";"b";"c"]
    let listSize = List.length list
    [ for i in [0..listSize-1] do
        let element = list |> List.item i
        yield "hello " + element
    ]
// val helloBad : string list = ["hello a"; "hello b"; "hello c"]
```

不要那样做！用 `map` 之类的东西代替。它既简洁又高效：

```F#
let helloGood =
    let list = ["a";"b";"c"]
    list |> List.map (fun element -> "hello " + element)
// val helloGood : string list = ["hello a"; "hello b"; "hello c"]
```

## 11.通过搜索获取元素

您可以使用 `find` 和 `findIndex` 搜索元素或其索引：

- [`find : predicate:('T -> bool) -> list:'T list -> 'T`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L201)。返回给定函数返回 true 的第一个元素。
- [`findIndex : predicate:('T -> bool) -> list:'T list -> int`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L222)。返回给定函数返回 true 的第一个元素的索引。

你也可以向后搜索：

- [`findBack : predicate:('T -> bool) -> list:'T list -> 'T`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L211)。返回给定函数返回 true 的最后一个元素。
- [`findIndexBack : predicate:('T -> bool) -> list:'T list -> int`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L233)。返回给定函数返回 true 的最后一个元素的索引。

但是，如果找不到物品怎么办？然后，这些操作将失败，并出现异常（`KeyNotFoundException`）。

因此，我建议，与 `head` 和 `item` 一样，您通常避免使用这些函数，并使用下面的 `tryXXX` 等效函数：

- [`tryFind : predicate:('T -> bool) -> list:'T list -> 'T option`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L800)。返回给定函数返回 true 的第一个元素，如果不存在此类元素，则返回 None。
- [`tryFindBack : predicate:('T -> bool) -> list:'T list -> 'T option`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L809)。返回给定函数返回 true 的最后一个元素，如果不存在此类元素，则返回 None。
- [`tryFindIndex : predicate:('T -> bool) -> list:'T list -> int option`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L819)。返回给定函数返回 true 的第一个元素的索引，如果不存在此类元素，则返回 None。
- [`tryFindIndexBack : predicate:('T -> bool) -> list:'T list -> int option`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L837)。返回给定函数返回 true 的最后一个元素的索引，如果不存在此类元素，则返回 None。

如果你在 `find` 之前做 `map`，你通常可以使用 `pick`（或更好的是 `tryPick`）将这两个步骤组合成一个步骤。请参阅下面的使用示例。

- [`pick : chooser:('T -> 'U option) -> list:'T list -> 'U`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L561)。将给定的函数应用于连续的元素，返回第一个结果，其中chooser函数返回Some。
- [`tryPick : chooser:('T -> 'U option) -> list:'T list -> 'U option`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L791)。将给定的函数应用于连续的元素，返回第一个结果，其中选择器函数返回 Some，如果不存在这样的元素，则返回 None。

### 使用示例

```F#
let listOfTuples = [ (1,"a"); (2,"b"); (3,"b"); (4,"a"); ]

listOfTuples |> List.find ( fun (x,y) -> y = "b")
// (2, "b")

listOfTuples |> List.findBack ( fun (x,y) -> y = "b")
// (3, "b")

listOfTuples |> List.findIndex ( fun (x,y) -> y = "b")
// 1

listOfTuples |> List.findIndexBack ( fun (x,y) -> y = "b")
// 2

listOfTuples |> List.find ( fun (x,y) -> y = "c")
// KeyNotFoundException
```

使用 `pick`，而不是返回 bool，您返回一个选项：

```F#
listOfTuples |> List.pick ( fun (x,y) -> if y = "b" then Some (x,y) else None)
// (2, "b")
```

### 选择 vs. 查找

这个“pick”函数可能看起来没有必要，但在处理返回选项的函数时很有用。

例如，假设有一个函数 `tryInt` 解析字符串，如果字符串是有效的 int，则返回 `Some int`，否则返回 `None`。

```F#
// string -> int option
let tryInt str =
    match System.Int32.TryParse(str) with
    | true, i -> Some i
    | false, _ -> None
```

现在假设我们想找到列表中的第一个有效 int。粗略的方式是：

- 使用 `tryInt` 映射列表
- 使用 `find` 找到第一个 `Some`
- 使用 `Option.get` 从选项内部获取值

代码可能看起来像这样：

```F#
let firstValidNumber =
    ["a";"2";"three"]
    // map the input
    |> List.map tryInt
    // find the first Some
    |> List.find (fun opt -> opt.IsSome)
    // get the data from the option
    |> Option.get
// val firstValidNumber : int = 2
```

但 `pick` 会一次完成所有这些步骤！因此，代码变得简单得多：

```F#
let firstValidNumber =
    ["a";"2";"three"]
    |> List.pick tryInt
```

如果你想以与 `pick` 相同的方式返回许多元素，可以考虑使用 `choose`（见第 12 节）。

## 12.从集合中获取元素子集

上一节是关于获取一个元素的。如何获得多个元素？你真幸运！有很多功能可供选择。

要从正面提取元素，请使用以下方法之一：

- [`take: count:int -> list:'T list -> 'T list`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L746)。返回集合的前 N 个元素。
- [`takeWhile: predicate:('T -> bool) -> list:'T list -> 'T list`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L756)。返回一个包含原始集合的所有元素的集合，而给定的谓词返回 true，然后不再返回其他元素。
- [`truncate: count:int -> list:'T list -> 'T list`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L782)。在新集合中最多返回 N 个元素。

要从后部提取元素，请使用以下方法之一：

- [`skip: count:int -> list: 'T list -> 'T list`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L644)。删除前 N 个元素后返回集合。
- [`skipWhile: predicate:('T -> bool) -> list:'T list -> 'T list`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L652)。当给定的谓词返回 true 时，绕过集合中的元素，然后返回集合的其余元素。
- [`tail: list:'T list -> 'T list`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L730)。删除第一个元素后返回集合。

要提取其他元素子集，请使用以下方法之一：

- [`filter: predicate:('T -> bool) -> list:'T list -> 'T list`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L241)。返回一个新的集合，该集合仅包含给定函数返回 true 的集合元素。
- [`except: itemsToExclude:seq<'T> -> list:'T list -> 'T list when 'T : equality`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L155)。使用泛型哈希和等式比较来比较值，返回一个新的集合，其中包含输入集合中未出现在itemsToExclude序列中的不同元素。
- [`choose: chooser:('T -> 'U option) -> list:'T list -> 'U list`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L55)。将给定的函数应用于集合的每个元素。返回一个由函数返回Some的元素组成的集合。
- [`where: predicate:('T -> bool) -> list:'T list -> 'T list`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L866)。返回一个新集合，该集合仅包含给定谓词返回true的集合元素。注意：“where”是“filter”的同义词。
- （仅数组） `sub : 'T [] -> int -> int -> 'T []`。创建一个包含所提供子范围的数组，该子范围由起始索引和长度指定。
- 您还可以使用切片语法： `myArray.[2..5]`. 示例见下文。

要将列表缩减为不同的元素，请使用以下方法之一：

- [`distinct: list:'T list -> 'T list when 'T : equality`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L107)。根据对条目的通用哈希和相等性比较，返回一个不包含重复条目的集合。
- [`distinctBy: projection:('T -> 'Key) -> list:'T list -> 'T list when 'Key : equality`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L118)。根据给定键生成函数返回的键的通用哈希和相等性比较，返回一个不包含重复项的集合。

### 使用示例

从前面看元素：

```F#
[1..10] |> List.take 3
// [1; 2; 3]

[1..10] |> List.takeWhile (fun i -> i < 3)
// [1; 2]

[1..10] |> List.truncate 4
// [1; 2; 3; 4]

[1..2] |> List.take 3
// System.InvalidOperationException: The input sequence has an insufficient number of elements.

[1..2] |> List.takeWhile (fun i -> i < 3)
// [1; 2]

[1..2] |> List.truncate 4
// [1; 2]   // no error!
```

从后面取元素：

```F#
[1..10] |> List.skip 3
// [4; 5; 6; 7; 8; 9; 10]

[1..10] |> List.skipWhile (fun i -> i < 3)
// [3; 4; 5; 6; 7; 8; 9; 10]

[1..10] |> List.tail
// [2; 3; 4; 5; 6; 7; 8; 9; 10]

[1..2] |> List.skip 3
// System.ArgumentException: The index is outside the legal range.

[1..2] |> List.skipWhile (fun i -> i < 3)
// []

[1] |> List.tail |> List.tail
// System.ArgumentException: The input list was empty.
```

要提取其他元素子集，请执行以下操作：

```F#
[1..10] |> List.filter (fun i -> i%2 = 0) // even
// [2; 4; 6; 8; 10]

[1..10] |> List.where (fun i -> i%2 = 0) // even
// [2; 4; 6; 8; 10]

[1..10] |> List.except [3;4;5]
// [1; 2; 6; 7; 8; 9; 10]
```

要提取切片，请执行以下操作：

```F#
Array.sub [|1..10|] 3 5
// [|4; 5; 6; 7; 8|]

[1..10].[3..5]
// [4; 5; 6]

[1..10].[3..]
// [4; 5; 6; 7; 8; 9; 10]

[1..10].[..5]
// [1; 2; 3; 4; 5; 6]
```

请注意，对列表进行切片可能很慢，因为它们不是随机访问的。然而，阵列上的切片速度很快。

要提取不同的元素，请执行以下操作：

```F#
[1;1;1;2;3;3] |> List.distinct
// [1; 2; 3]

[ (1,"a"); (1,"b"); (1,"c"); (2,"d")] |> List.distinctBy fst
// [(1, "a"); (2, "d")]
```

### 选择 vs. 筛选

与 `pick` 一样，`choose` 函数可能看起来很笨拙，但在处理返回选项的函数时它很有用。

事实上，`choose` 是 `filter`，就像 `pick` 是 `find` 一样，信号是 `Some` vs. `None`，而不是使用布尔过滤器。

如前所述，假设有一个函数 `tryInt` 解析字符串，如果字符串是有效的 int，则返回 `Some int`，否则返回 `None`。

```F#
// string -> int option
let tryInt str =
    match System.Int32.TryParse(str) with
    | true, i -> Some i
    | false, _ -> None
```

现在假设我们想找到列表中的所有有效整数。粗略的方式是：

- 使用 `tryInt` 映射列表
- 筛选以仅包括 `Some`
- 使用 `Option.get` 从每个选项中获取值

代码可能看起来像这样：

```F#
let allValidNumbers =
    ["a";"2";"three"; "4"]
    // map the input
    |> List.map tryInt
    // include only the "Some"
    |> List.filter (fun opt -> opt.IsSome)
    // get the data from each option
    |> List.map Option.get
// val allValidNumbers : int list = [2; 4]
```

但 `choose` 会一次完成所有这些步骤！因此，代码变得简单得多：

```F#
let allValidNumbers =
    ["a";"2";"three"; "4"]
    |> List.choose tryInt
```

如果你已经有了一个选项列表，你可以通过将 `id` 传递给 `choose` 来过滤并返回“Some”：

```F#
let reduceOptions =
    [None; Some 1; None; Some 2]
    |> List.choose id
// val reduceOptions : int list = [1; 2]
```

如果你想以与 `choose` 相同的方式返回第一个元素，可以考虑使用 `pick`（见第 11 节）。

如果你想对其他包装类型（如成功/失败结果）执行与 `choose` 类似的操作，这里有一个讨论。

## 13.分区、组块和分组

有很多不同的方法来分割集合！请查看使用示例以了解差异：

- [`chunkBySize: chunkSize:int -> list:'T list -> 'T list list`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L63)。将输入集合划分为大小不超过 chunkSize 的块。
- [`groupBy : projection:('T -> 'Key) -> list:'T list -> ('Key * 'T list) list when 'Key : equality`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L325)。将键生成函数应用于集合的每个元素，并生成唯一键的列表。每个唯一键都包含与此键匹配的所有元素的列表。
- [`pairwise: list:'T list -> ('T * 'T) list`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L541)。返回输入集合中每个元素及其前导元素的集合，但第一个元素除外，它仅作为第二个元素的前导元素返回。
- （Seq除外） [`partition: predicate:('T -> bool) -> list:'T list -> ('T list * 'T list)`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L551)。将集合拆分为两个集合，包含给定谓词分别返回true和false的元素。
- （Seq除外） [`splitAt: index:int -> list:'T list -> ('T list * 'T list)`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L688)。在给定的索引处将一个集合拆分为两个集合。
- [`splitInto: count:int -> list:'T list -> 'T list list`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L137)。将输入集合拆分为最多计数块。
- [`windowed : windowSize:int -> list:'T list -> 'T list list`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L875)。返回一个滑动窗口列表，其中包含从输入集合中提取的元素。每个窗口都会作为新收藏返回。与 `pairwise` 不同，窗口是集合，而不是元组。

### 使用示例

```F#
[1..10] |> List.chunkBySize 3
// [[1; 2; 3]; [4; 5; 6]; [7; 8; 9]; [10]]
// note that the last chunk has one element

[1..10] |> List.splitInto 3
// [[1; 2; 3; 4]; [5; 6; 7]; [8; 9; 10]]
// note that the first chunk has four elements

['a'..'i'] |> List.splitAt 3
// (['a'; 'b'; 'c'], ['d'; 'e'; 'f'; 'g'; 'h'; 'i'])

['a'..'e'] |> List.pairwise
// [('a', 'b'); ('b', 'c'); ('c', 'd'); ('d', 'e')]

['a'..'e'] |> List.windowed 3
// [['a'; 'b'; 'c']; ['b'; 'c'; 'd']; ['c'; 'd'; 'e']]

let isEven i = (i%2 = 0)
[1..10] |> List.partition isEven
// ([2; 4; 6; 8; 10], [1; 3; 5; 7; 9])

let firstLetter (str:string) = str.[0]
["apple"; "alice"; "bob"; "carrot"] |> List.groupBy firstLetter
// [('a', ["apple"; "alice"]); ('b', ["bob"]); ('c', ["carrot"])]
```

除 `splitAt` 和 `pairwise` 之外的所有函数都能优雅地处理边缘情况：

```F#
[1] |> List.chunkBySize 3
// [[1]]

[1] |> List.splitInto 3
// [[1]]

['a'; 'b'] |> List.splitAt 3
// InvalidOperationException: The input sequence has an insufficient number of elements.

['a'] |> List.pairwise
// InvalidOperationException: The input sequence has an insufficient number of elements.

['a'] |> List.windowed 3
// []

[1] |> List.partition isEven
// ([], [1])

[] |> List.groupBy firstLetter
//  []
```

## 14.汇总或概括一个集合

聚合集合中元素的最通用方法是使用 `reduce`：

- [`reduce : reduction:('T -> 'T -> 'T) -> list:'T list -> 'T`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L584)。对集合的每个元素应用一个函数，在计算中使用累加器参数。
- [`reduceBack : reduction:('T -> 'T -> 'T) -> list:'T list -> 'T`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L595)。从末尾开始，对集合的每个元素应用一个函数，在计算过程中使用累加器参数。

对于常用的聚合，存在特定版本的 `reduce`：

- [`max : list:'T list -> 'T when 'T : comparison`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L482)。通过 Operators.max 比较，返回集合中所有元素中的最大值。
- [`maxBy : projection:('T -> 'U) -> list:'T list -> 'T when 'U : comparison`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L492)。返回集合中所有元素中的最大值，通过函数结果上的 Operators.max 进行比较。
- [`min : list:'T list -> 'T when 'T : comparison`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L501)。通过 Operators.min 进行比较，返回集合中所有元素中的最低值。
- [`minBy : projection:('T -> 'U) -> list:'T list -> 'T when 'U : comparison `](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L511)。返回集合中所有元素中的最低值，通过函数结果上的 Operators.min 进行比较。
- [`sum : list:'T list -> 'T when 'T has static members (+) and Zero`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L711)。返回集合中元素的总和。
- [`sumBy : projection:('T -> 'U) -> list:'T list -> 'U when 'U has static members (+) and Zero`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L720)。返回通过将函数应用于集合的每个元素而生成的结果的总和。
- [`average : list:'T list -> 'T when 'T has static members (+) and Zero and DivideByInt`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L30)。返回集合中元素的平均值。请注意，整数列表不能求平均值——它们必须转换为浮点数或小数。
- [`averageBy : projection:('T -> 'U) -> list:'T list -> 'U when 'U has static members (+) and Zero and DivideByInt`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L43)。返回通过将函数应用于集合的每个元素而生成的结果的平均值。

最后是一些计数函数：

- [`length: list:'T list -> int`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L404)。返回集合的长度。
- [`countBy : projection:('T -> 'Key) -> list:'T list -> ('Key * int) list when 'Key : equality`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L129)。对每个元素应用键生成函数，并返回一个集合，该集合产生唯一的键及其在原始集合中的出现次数。

### 使用示例

`reduce` 是 `fold` 的一种变体，没有初始状态 & 有关 `fold` 的更多信息，请参阅第 19 节。一种思考方式是在每个元素之间插入一个运算符。

```F#
["a";"b";"c"] |> List.reduce (+)
// "abc"
```

与此相同：

```F#
"a" + "b" + "c"
```

这里是另一个例子：

```F#
[2;3;4] |> List.reduce (*)
// is same as
2 * 3 * 4
// Result is 24
```

组合元素的某些方式取决于组合的顺序，因此“reduce”有两种变体：

- `reduce` 在列表中向前移动。
- `reduceBack` 在列表中向后移动，这并不奇怪。

这里展示了这种差异。首先 `reduce`：

```F#
[1;2;3;4] |> List.reduce (fun state x -> (state)*10 + x)

// built up from                // state at each step
1                               // 1
(1)*10 + 2                      // 12
((1)*10 + 2)*10 + 3             // 123
(((1)*10 + 2)*10 + 3)*10 + 4    // 1234

// Final result is 1234
```

将相同的组合函数与 `reduceBack` 一起使用会产生不同的结果！它看起来像这样：

```F#
[1;2;3;4] |> List.reduceBack (fun x state -> x + 10*(state))

// built up from                // state at each step
4                               // 4
3 + 10*(4)                      // 43
2 + 10*(3 + 10*(4))             // 432
1 + 10*(2 + 10*(3 + 10*(4)))    // 4321

// Final result is 4321
```

同样，有关 `fold` 和 `foldBack` 相关功能的更详细讨论，请参阅第 19 节。

其他聚合函数要简单得多。

```F#
type Suit = Club | Diamond | Spade | Heart
type Rank = Two | Three | King | Ace
let cards = [ (Club,King); (Diamond,Ace); (Spade,Two); (Heart,Three); ]

cards |> List.max        // (Heart, Three)
cards |> List.maxBy snd  // (Diamond, Ace)
cards |> List.min        // (Club, King)
cards |> List.minBy snd  // (Spade, Two)

[1..10] |> List.sum
// 55

[ (1,"a"); (2,"b") ] |> List.sumBy fst
// 3

[1..10] |> List.average
// The type 'int' does not support the operator 'DivideByInt'

[1..10] |> List.averageBy float
// 5.5

[ (1,"a"); (2,"b") ] |> List.averageBy (fst >> float)
// 1.5

[1..10] |> List.length
// 10

[ ("a","A"); ("b","B"); ("a","C") ]  |> List.countBy fst
// [("a", 2); ("b", 1)]

[ ("a","A"); ("b","B"); ("a","C") ]  |> List.countBy snd
// [("A", 1); ("B", 1); ("C", 1)]
```

大多数聚合函数不喜欢空列表！为了安全起见，您可以考虑使用其中一个 `fold` 函数——请参阅第 19 节。

```F#
let emptyListOfInts : int list = []

emptyListOfInts |> List.reduce (+)
// ArgumentException: The input list was empty.

emptyListOfInts |> List.max
// ArgumentException: The input sequence was empty.

emptyListOfInts |> List.min
// ArgumentException: The input sequence was empty.

emptyListOfInts |> List.sum
// 0

emptyListOfInts |> List.averageBy float
// ArgumentException: The input sequence was empty.

let emptyListOfTuples : (int*int) list = []
emptyListOfTuples |> List.countBy fst
// (int * int) list = []
```

## 15.更改元素顺序

您可以使用反转、排序和排列来更改元素的顺序。以下所有项目都会返回新集合：

- [`rev: list:'T list -> 'T list`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L608)。返回一个元素顺序相反的新集合。
- [`sort: list:'T list -> 'T list when 'T : comparison`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L678)。使用 Operators.compare 对给定的集合进行排序。
- [`sortDescending: list:'T list -> 'T list when 'T : comparison`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L705)。使用 Operators.compare 按降序对给定的集合进行排序。
- [`sortBy: projection:('T -> 'Key) -> list:'T list -> 'T list when 'Key : comparison`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L670)。使用给定投影给出的键对给定集合进行排序。使用Operators.compare比较密钥。
- [`sortByDescending: projection:('T -> 'Key) -> list:'T list -> 'T list when 'Key : comparison`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L697)。使用Operators.compare比较密钥。
- [`sortWith: comparer:('T -> 'T -> int) -> list:'T list -> 'T list`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L661)。使用给定的比较函数对给定的集合进行排序。
- [`permute : indexMap:(int -> int) -> list:'T list -> 'T list`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L570)。返回一个集合，其中所有元素都根据指定的排列进行了排列。

还有一些只使用数组的函数可以就地排序：

- （仅限数组） [`sortInPlace: array:'T[] -> unit when 'T : comparison`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/array.fsi#L874)。通过在适当的位置修改数组来对数组的元素进行排序。使用 Operators.compare 对元素进行比较。
- （仅限数组） [`sortInPlaceBy: projection:('T -> 'Key) -> array:'T[] -> unit when 'Key : comparison`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/array.fsi#L858)。使用给定的键投影，通过在适当的位置修改数组来对数组的元素进行排序。使用 Operators.compare 比较 keys。
- （仅限数组） [`sortInPlaceWith: comparer:('T -> 'T -> int) -> array:'T[] -> unit`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/array.fsi#L867)。使用给定的比较函数作为顺序，通过在适当的位置修改数组来对数组的元素进行排序。

### 使用示例

```F#
[1..5] |> List.rev
// [5; 4; 3; 2; 1]

[2;4;1;3;5] |> List.sort
// [1; 2; 3; 4; 5]

[2;4;1;3;5] |> List.sortDescending
// [5; 4; 3; 2; 1]

[ ("b","2"); ("a","3"); ("c","1") ]  |> List.sortBy fst
// [("a", "3"); ("b", "2"); ("c", "1")]

[ ("b","2"); ("a","3"); ("c","1") ]  |> List.sortBy snd
// [("c", "1"); ("b", "2"); ("a", "3")]

// example of a comparer
let tupleComparer tuple1 tuple2  =
    if tuple1 < tuple2 then
        -1
    elif tuple1 > tuple2 then
        1
    else
        0

[ ("b","2"); ("a","3"); ("c","1") ]  |> List.sortWith tupleComparer
// [("a", "3"); ("b", "2"); ("c", "1")]

[1..10] |> List.permute (fun i -> (i + 3) % 10)
// [8; 9; 10; 1; 2; 3; 4; 5; 6; 7]

[1..10] |> List.permute (fun i -> 9 - i)
// [10; 9; 8; 7; 6; 5; 4; 3; 2; 1]
```

## 16.测试集合的元素

这些函数集都返回 true 或 false。

- [`contains: value:'T -> source:'T list -> bool when 'T : equality`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L97)。测试集合是否包含指定的元素。
- [`exists: predicate:('T -> bool) -> list:'T list -> bool`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L176)。测试集合中的任何元素是否满足给定的谓词。
- [`forall: predicate:('T -> bool) -> list:'T list -> bool`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L299)。测试集合的所有元素是否满足给定的谓词。
- [`isEmpty: list:'T list -> bool`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L353)。如果集合不包含元素，则返回 true，否则返回 false。

### 使用示例

```F#
[1..10] |> List.contains 5
// true

[1..10] |> List.contains 42
// false

[1..10] |> List.exists (fun i -> i > 3 && i < 5)
// true

[1..10] |> List.exists (fun i -> i > 5 && i < 3)
// false

[1..10] |> List.forall (fun i -> i > 0)
// true

[1..10] |> List.forall (fun i -> i > 5)
// false

[1..10] |> List.isEmpty
// false
```

## 17.将每个元素转化为不同的东西

我有时喜欢将函数式编程视为“面向转换的编程”，而 `map`（也称为 LINQ 中的 `Select`）是这种方法最基本的组成部分之一。事实上，我在这里花了整整一个系列。

- [`map: mapping:('T -> 'U) -> list:'T list -> 'U list`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L419)。构建一个新的集合，其元素是将给定函数应用于集合中每个元素的结果。

有时每个元素都映射到一个列表，你想把所有列表都展平。在这种情况下，使用 `collect`（也称为 LINQ 中的 `SelectMany`）。

- [`collect: mapping:('T -> 'U list) -> list:'T list -> 'U list`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L70)。对于列表中的每个元素，应用给定的函数。连接所有结果并返回组合列表。

其他转换功能包括：

- 第 12 节中的 `choose` 是地图和选项过滤器的组合。
- （仅限序列） [`cast: source:IEnumerable -> seq<'T>`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/seq.fsi#L599)。包装一个松散类型的 `System.Collections` 序列作为类型化序列。

### 使用示例

以下是一些以传统方式使用 `map` 的示例，作为一个接受列表和映射函数并返回新的转换列表的函数：

```F#
let add1 x = x + 1

// map as a list transformer
[1..5] |> List.map add1
// [2; 3; 4; 5; 6]

// the list being mapped over can contain anything!
let times2 x = x * 2
[ add1; times2] |> List.map (fun f -> f 5)
// [6; 10]
```

你也可以把 `map` 看作一个函数转换器。它将元素到元素函数转换为列表到列表函数。

```F#
let add1ToEachElement = List.map add1
// "add1ToEachElement" transforms lists to lists rather than ints to ints
// val add1ToEachElement : (int list -> int list)

// now use it
[1..5] |> add1ToEachElement
// [2; 3; 4; 5; 6]
```

`collect` 对平整列表起作用。如果你已经有一个列表列表，你可以使用带有 `id` 的 `collect` 来展开它们。

```f#
[2..5] |> List.collect (fun x -> [x; x*x; x*x*x] )
// [2; 4; 8; 3; 9; 27; 4; 16; 64; 5; 25; 125]

// using "id" with collect
let list1 = [1..3]
let list2 = [4..6]
[list1; list2] |> List.collect id
// [1; 2; 3; 4; 5; 6]
```

### Seq.cast

最后，当使用 BCL 中具有专门集合类而不是泛型的旧部分时，`Seq.cast` 非常有用。

例如，Regex 库有这个问题，因此下面的代码无法编译，因为 `MatchCollection` 不是 `IEnumerable<T>`

```F#
open System.Text.RegularExpressions

let matches =
    let pattern = "\d\d\d"
    let matchCollection = Regex.Matches("123 456 789",pattern)
    matchCollection
    |> Seq.map (fun m -> m.Value)     // ERROR
    // ERROR: The type 'MatchCollection' is not compatible with the type 'seq<'a>'
    |> Seq.toList
```

修复方法是将 `MatchCollection` 转换为 `Seq<Match>`，然后代码将很好地工作：

```F#
let matches =
    let pattern = "\d\d\d"
    let matchCollection = Regex.Matches("123 456 789",pattern)
    matchCollection
    |> Seq.cast<Match>
    |> Seq.map (fun m -> m.Value)
    |> Seq.toList
// output = ["123"; "456"; "789"]
```

## 18.迭代每个元素

通常，在处理集合时，我们使用 `map` 将每个元素转换为新值。但有时我们需要用一个不产生有用值的函数（“unit 函数”）来处理所有元素。

- [`iter: action:('T -> unit) -> list:'T list -> unit`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L367)。将给定的函数应用于集合的每个元素。
- 或者，您可以使用 for 循环。for 循环中的表达式必须返回 `unit`。

### 使用示例

unit 函数最常见的例子都是关于副作用的：打印到控制台、更新数据库、将消息放入队列等。对于下面的例子，我将只使用 `printfn` 作为我的单元函数。

```F#
[1..3] |> List.iter (fun i -> printfn "i is %i" i)
(*
i is 1
i is 2
i is 3
*)

// or using partial application
[1..3] |> List.iter (printfn "i is %i")

// or using a for loop
for i = 1 to 3 do
    printfn "i is %i" i

// or using a for-in loop
for i in [1..3] do
    printfn "i is %i" i
```

如上所述，`iter` 或 for 循环中的表达式必须返回 unit。在以下示例中，我们尝试向元素添加 1，但得到编译器错误：

```F#
[1..3] |> List.iter (fun i -> i + 1)
//                               ~~~
// ERROR error FS0001: The type 'unit' does not match the type 'int'

// a for-loop expression *must* return unit
for i in [1..3] do
     i + 1  // ERROR
     // This expression should have type 'unit',
     // but has type 'int'. Use 'ignore' ...
```

如果你确定这不是你代码中的逻辑错误，并且你想摆脱这个错误，你可以将结果管道化为 `ignore`：

```F#
[1..3] |> List.iter (fun i -> i + 1 |> ignore)

for i in [1..3] do
     i + 1 |> ignore
```

## 19.通过迭代线程化状态

`fold` 功能是集合库中最基本、最强大的功能。所有其他函数（除了像 `unfold` 这样的生成器）都可以用它来编写。请参阅下面的示例。

- [`fold<'T,'State> : folder:('State -> 'T -> 'State) -> state:'State -> list:'T list -> 'State`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L254)。将函数应用于集合的每个元素，在计算过程中使用累加器参数。
- [`foldBack<'T,'State> : folder:('T -> 'State -> 'State) -> list:'T list -> state:'State -> 'State`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L276)。从末尾开始，对集合的每个元素应用一个函数，在计算过程中使用累加器参数。警告：注意在无限列表上使用`Seq.foldBack`！运行时会嘲笑你哈哈哈，然后很安静。

`fold` 函数通常被称为“左折叠”，`foldBack` 通常被称之为“右折叠”。

`scan` 函数类似于 `fold`，但返回中间结果，因此可用于跟踪或监视迭代。

- [`scan<'T,'State> : folder:('State -> 'T -> 'State) -> state:'State -> list:'T list -> 'State list`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L619)。与 `fold` 类似，但返回中间结果和最终结果。
- [`scanBack<'T,'State> : folder:('T -> 'State -> 'State) -> list:'T list -> state:'State -> 'State list`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L627)。与 `foldBack` 类似，但返回中间结果和最终结果。

就像双胞胎一样，`scan` 通常被称为“左扫描”，而 `scanBack` 通常被称作“右扫描”。

最后，`mapFold` 将 `map` 和 `fold` 组合成一个令人敬畏的超能力。比单独使用 `map` 和 `fold` 更复杂，但也更高效。

- [`mapFold<'T,'State,'Result> : mapping:('State -> 'T -> 'Result * 'State) -> state:'State -> list:'T list -> 'Result list * 'State`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L447)。结合地图和折叠。构建一个新的集合，其元素是将给定函数应用于输入集合的每个元素的结果。该函数还用于累加最终值。
- [`mapFoldBack<'T,'State,'Result> : mapping:('T -> 'State -> 'Result * 'State) -> list:'T list -> state:'State -> 'Result list * 'State`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L456)。将 map 和 foldBack 组合在一起。构建一个新的集合，其元素是将给定函数应用于输入集合的每个元素的结果。该函数还用于累加最终值。

### `fold` 示例

对 `fold` 的一种思考方式是，它类似于 `reduce`，但初始状态有一个额外的参数：

```F#
["a";"b";"c"] |> List.fold (+) "hello: "
// "hello: abc"
// "hello: " + "a" + "b" + "c"

[1;2;3] |> List.fold (+) 10
// 16
// 10 + 1 + 2 + 3
```

与 `reduce` 一样，`fold` 和 `foldBack` 可以给出非常不同的答案。

```F#
[1;2;3;4] |> List.fold (fun state x -> (state)*10 + x) 0
                                // state at each step
1                               // 1
(1)*10 + 2                      // 12
((1)*10 + 2)*10 + 3             // 123
(((1)*10 + 2)*10 + 3)*10 + 4    // 1234
// Final result is 1234
```

这是 `foldBack` 版本：

```F#
List.foldBack (fun x state -> x + 10*(state)) [1;2;3;4] 0
                                // state at each step
4                               // 4
3 + 10*(4)                      // 43
2 + 10*(3 + 10*(4))             // 432
1 + 10*(2 + 10*(3 + 10*(4)))    // 4321
// Final result is 4321
```

请注意，`foldBack` 具有不同的参数 `fold` 顺序：列表倒数第二，初始状态倒数第三，这意味着管道不太方便。

### 递归与迭代

人们很容易把 `fold` 和 `foldBack` 混淆。我发现将 `fold` 视为迭代是有帮助的，而 `foldBack` 是关于递归的。

假设我们想计算一个列表的总和。迭代方式是使用 for 循环。您从一个（可变）累加器开始，并在每次迭代中对其进行线程处理，并在执行过程中对其加以更新。

```F#
let iterativeSum list =
    let mutable total = 0
    for e in list do
        total <- total + e
    total // return sum
```

另一方面，递归方法说，如果列表有头部和尾部，则首先计算尾部的总和（较小的列表），然后将头部添加到其中。

每次尾巴变得越来越小，直到它变空，这时你就完成了。

```F#
let rec recursiveSum list =
    match list with
    | [] ->
        0
    | head::tail ->
        head + (recursiveSum tail)
```

哪种方法更好？

对于聚合，迭代方式（`fold`）通常最容易理解。但对于构建新列表之类的事情，递归方式（`foldBack`）更容易理解。

例如，如果我们要从头开始创建一个函数，将每个元素转换为相应的字符串，我们可能会写这样的内容：

```F#
let rec mapToString list =
    match list with
    | [] ->
        []
    | head::tail ->
        head.ToString() :: (mapToString tail)

[1..3] |> mapToString
// ["1"; "2"; "3"]
```

使用 `foldBack`，我们可以“原样”传输相同的逻辑：

- 空列表的操作 = `[]`
- 非空列表的操作 = `head.ToString() :: state`

以下是结果函数：

```F#
let foldToString list =
    let folder head state =
        head.ToString() :: state
    List.foldBack folder list []

[1..3] |> foldToString
// ["1"; "2"; "3"]
```

另一方面，`fold` 的一大优势是更容易使用“内联”，因为它与管道配合得更好。

幸运的是，您可以像 `foldBack` 一样使用 `fold`（至少用于列表构造），只要在末尾反转列表即可。

```F#
// inline version of "foldToString"
[1..3]
|> List.fold (fun state head -> head.ToString() :: state) []
|> List.rev
// ["1"; "2"; "3"]
```

### 使用 `fold`实现其他功能

正如我上面提到的，`fold` 是操作列表的核心功能，可以模拟大多数其他功能，尽管可能不如自定义实现有效。

例如，以下是使用 `fold` 实现的 `map`：

```F#
/// map a function "f" over all elements
let myMap f list =
    // helper function
    let folder state head =
        f head :: state

    // main flow
    list
    |> List.fold folder []
    |> List.rev

[1..3] |> myMap (fun x -> x + 2)
// [3; 4; 5]
```

下面是使用 `fold` 实现的 `filter`：

```F#
/// return a new list of elements for which "pred" is true
let myFilter pred list =
    // helper function
    let folder state head =
        if pred head then
            head :: state
        else
            state

    // main flow
    list
    |> List.fold folder []
    |> List.rev

let isOdd n = (n%2=1)
[1..5] |> myFilter isOdd
// [1; 3; 5]
```

当然，您可以以类似的方式模拟其他功能。

### `scan` 示例

前面，我展示了一个 `fold` 中间步骤的示例：

```F#
[1;2;3;4] |> List.fold (fun state x -> (state)*10 + x) 0
                                // state at each step
1                               // 1
(1)*10 + 2                      // 12
((1)*10 + 2)*10 + 3             // 123
(((1)*10 + 2)*10 + 3)*10 + 4    // 1234
// Final result is 1234
```

对于那个例子，我必须手动计算中间状态，

好吧，如果我使用了 `scan`，我就会免费获得这些中间状态！

```F#
[1;2;3;4] |> List.scan (fun state x -> (state)*10 + x) 0
// accumulates from left ===> [0; 1; 12; 123; 1234]
```

`scanBack` 的工作方式相同，但当然是反向的：

```F#
List.scanBack (fun x state -> (state)*10 + x) [1;2;3;4] 0
// [4321; 432; 43; 4; 0]  <=== accumulates from right
```

与 `foldBack` 一样，“向右扫描”的参数顺序与“向左扫描”相反。

### 使用 `scan` 截断字符串

这里有一个 `scan` 很有用的例子。假设你有一个新闻网站，你需要确保标题能容纳 50 个字符。

你可以在 50 处截断字符串，但这看起来很难看。相反，您希望截断端位于单词边界处。

这里有一种使用 `scan` 的方法：

- 把标题分成几个字。
- 使用 `scan` 将单词重新组合在一起，生成一个片段列表，每个片段都添加了一个额外的单词。
- 获取 50 个字符以下的最长片段。

```F#
// start by splitting the text into words
let text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor."
let words = text.Split(' ')
// [|"Lorem"; "ipsum"; "dolor"; "sit"; ... ]

// accumulate a series of fragments
let fragments = words |> Seq.scan (fun frag word -> frag + " " + word) ""
(*
" Lorem"
" Lorem ipsum"
" Lorem ipsum dolor"
" Lorem ipsum dolor sit"
" Lorem ipsum dolor sit amet,"
etc
*)

// get the longest fragment under 50
let longestFragUnder50 =
    fragments
    |> Seq.takeWhile (fun s -> s.Length <= 50)
    |> Seq.last

// trim off the first blank
let longestFragUnder50Trimmed =
    longestFragUnder50 |> (fun s -> s.[1..])

// The result is:
//   "Lorem ipsum dolor sit amet, consectetur"
```

请注意，我使用的是 `Seq.scan` 而不是 `Array.scan`。这是一种延迟扫描，避免了创建不需要的片段。

最后，这是作为效用函数的完整逻辑：

```F#
// the whole thing as a function
let truncText max (text:string) =
    if text.Length <= max then
        text
    else
        text.Split(' ')
        |> Seq.scan (fun frag word -> frag + " " + word) ""
        |> Seq.takeWhile (fun s -> s.Length <= max-3)
        |> Seq.last
        |> (fun s -> s.[1..] + "...")

"a small headline" |> truncText 50
// "a small headline"

text |> truncText 50
// "Lorem ipsum dolor sit amet, consectetur..."
```

是的，我知道有一种比这更有效的实现方式，但我希望这个小例子能展示 `scan` 的力量。

### `mapFold` 示例

`mapFold` 函数可以一步完成映射和折叠，有时会很方便。

下面是一个使用 `mapFold` 在一步中组合加法和求和的示例：

```F#
let add1 x = x + 1

// add1 using map
[1..5] |> List.map (add1)
// Result => [2; 3; 4; 5; 6]

// sum using fold
[1..5] |> List.fold (fun state x -> state + x) 0
// Result => 15

// map and sum using mapFold
[1..5] |> List.mapFold (fun state x -> add1 x, (state + x)) 0
// Result => ([2; 3; 4; 5; 6], 15)
```

## 20.使用每个元素的索引

通常，在迭代时需要元素的索引。你可以使用可变计数器，但为什么不坐下来让库为你做这项工作呢？

- [`mapi: mapping:(int -> 'T -> 'U) -> list:'T list -> 'U list`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L465)。与 `map` 类似，但也将整数索引传递给函数。有关 `map` 的更多信息，请参阅第 17 节。
- [`iteri: action:(int -> 'T -> unit) -> list:'T list -> unit`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L382)。与 `iter` 类似，但也将整数索引传递给函数。有关 `iter` 的更多信息，请参阅第18节。
- [`indexed: list:'T list -> (int * 'T) list`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L340)。返回一个新列表，其元素是输入列表中与每个元素的索引（从 0 开始）配对的对应元素。

### 使用示例

```F#
['a'..'c'] |> List.mapi (fun index ch -> sprintf "the %ith element is '%c'" index ch)
// ["the 0th element is 'a'"; "the 1th element is 'b'"; "the 2th element is 'c'"]

// with partial application
['a'..'c'] |> List.mapi (sprintf "the %ith element is '%c'")
// ["the 0th element is 'a'"; "the 1th element is 'b'"; "the 2th element is 'c'"]

['a'..'c'] |> List.iteri (printfn "the %ith element is '%c'")
(*
the 0th element is 'a'
the 1th element is 'b'
the 2th element is 'c'
*)
```

`indexed` 生成一个带有索引的元组，这是 `mapi` 特定用途的快捷方式：

```F#
['a'..'c'] |> List.mapi (fun index ch -> (index, ch) )
// [(0, 'a'); (1, 'b'); (2, 'c')]

// "indexed" is a shorter version of above
['a'..'c'] |> List.indexed
// [(0, 'a'); (1, 'b'); (2, 'c')]
```

## 21.将整个收藏转换为不同的收藏类型

您经常需要从一种集合转换为另一种集合。这些功能可以做到这一点。

`ofXXX` 函数用于将 `XXX` 转换为模块类型。例如，`List.ofArray` 会将数组转换为列表。

- （数组除外） [`ofArray : array:'T[\] -> 'T list`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L526)。从给定数组构建新集合。
- （Seq 除外） [`ofSeq: source:seq<'T> -> 'T list`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L532)。从给定的枚举对象构建新的集合。
- （列表除外） [`ofList: source:'T list -> seq<'T>`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/seq.fsi#L864)。从给定列表构建新集合。

`toXXX` 用于将模块类型转换为 `XXX` 类型。例如，`List.toArray` 将列表转换为数组。

- （数组除外） [`toArray: list:'T list -> 'T[]`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L762)。从给定的集合构建数组。
- （除了 Seq） [`toSeq: list:'T list -> seq<'T>`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L768)。将给定的集合视为序列。
- （列表除外） [`toList: source:seq<'T> -> 'T list`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/seq.fsi#L1189)。从给定的集合构建列表。

### 使用示例

```F#
[1..5] |> List.toArray      // [|1; 2; 3; 4; 5|]
[1..5] |> Array.ofList      // [|1; 2; 3; 4; 5|]
// etc
```

### 使用 disposables 的序列

这些转换函数的一个重要用途是将懒惰枚举（`seq`）转换为完全求值的集合，如 `list`。当涉及可支配资源时，这一点尤为重要，例如文件句柄或数据库连接。如果序列未转换为列表，则访问元素时可能会遇到错误。更多信息请参见第 28 节。

## 22.改变整个集合的行为

有一些特殊函数（仅适用于 Seq）可以改变整个集合的行为。

- （仅限序列） [`cache: source:seq<'T> -> seq<'T>`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/seq.fsi#L98)。返回与输入序列的缓存版本相对应的序列。此结果序列将具有与输入序列相同的元素。结果可以多次枚举。输入序列最多只能枚举一次，并且仅在必要时枚举。
- （仅限序列） [`readonly : source:seq<'T> -> seq<'T>`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/seq.fsi#L919)。构建一个新的序列对象，该对象委托给给定的序列对象。这确保了原始序列不会被类型转换重新发现和突变。
- （仅限序列） [`delay : generator:(unit -> seq<'T>) -> seq<'T>`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/seq.fsi#L221)。返回根据给定的序列延迟规范构建的序列。

### `cache` 例子

以下是一个正在使用的 `cache` 示例：

```F#
let uncachedSeq = seq {
    for i = 1 to 3 do
        printfn "Calculating %i" i
        yield i
    }

// iterate twice
uncachedSeq |> Seq.iter ignore
uncachedSeq |> Seq.iter ignore
```

对序列进行两次迭代的结果正如您所期望的那样：

```
Calculating 1
Calculating 2
Calculating 3
Calculating 1
Calculating 2
Calculating 3
```

但如果我们缓存序列…

```F#
let cachedSeq = uncachedSeq |> Seq.cache

// iterate twice
cachedSeq |> Seq.iter ignore
cachedSeq |> Seq.iter ignore
```

…则每个项目只打印一次：

```
Calculating 1
Calculating 2
Calculating 3
```

### `readonly` 例子

以下是一个 `readonly` 用于隐藏序列底层类型的示例：

```F#
// print the underlying type of the sequence
let printUnderlyingType (s:seq<_>) =
    let typeName = s.GetType().Name
    printfn "%s" typeName

[|1;2;3|] |> printUnderlyingType
// Int32[]

[|1;2;3|] |> Seq.readonly |> printUnderlyingType
// mkSeq@589   // a temporary type
```

### `delay` 例子

这是一个延迟的例子。

```F#
let makeNumbers max =
    [ for i = 1 to max do
        printfn "Evaluating %d." i
        yield i ]

let eagerList =
    printfn "Started creating eagerList"
    let list = makeNumbers 5
    printfn "Finished creating eagerList"
    list

let delayedSeq =
    printfn "Started creating delayedSeq"
    let list = Seq.delay (fun () -> makeNumbers 5 |> Seq.ofList)
    printfn "Finished creating delayedSeq"
    list
```

如果我们运行上面的代码，我们发现只需创建 `eagerList`，我们就可以打印所有“评估”消息。但是创建 `delayedSeq` 不会触发列表迭代。

```
Started creating eagerList
Evaluating 1.
Evaluating 2.
Evaluating 3.
Evaluating 4.
Evaluating 5.
Finished creating eagerList

Started creating delayedSeq
Finished creating delayedSeq
```

只有当序列被迭代时，列表创建才会发生：

```F#
eagerList |> Seq.take 3  // list already created
delayedSeq |> Seq.take 3 // list creation triggered
```

使用延迟的另一种方法是将列表嵌入到 `seq` 中，如下所示：

```F#
let embeddedList = seq {
    printfn "Started creating embeddedList"
    yield! makeNumbers 5
    printfn "Finished creating embeddedList"
    }
```

与 `delayedSeq` 一样，在序列迭代之前不会调用 `makeNumbers` 函数。

## 23.处理两个列表

如果你有两个列表，那么大多数常见函数（如 map 和 fold）都有类似物。

- [`map2: mapping:('T1 -> 'T2 -> 'U) -> list1:'T1 list -> list2:'T2 list -> 'U list`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L428)。构建一个新的集合，其元素是将给定函数成对应用于两个集合的相应元素的结果。
- [`mapi2: mapping:(int -> 'T1 -> 'T2 -> 'U) -> list1:'T1 list -> list2:'T2 list -> 'U list`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L473)。与 `mapi` 类似，但从两个长度相等的列表中映射相应的元素。
- [`iter2: action:('T1 -> 'T2 -> unit) -> list1:'T1 list -> list2:'T2 list -> unit`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L375)。将给定的函数同时应用于两个集合。集合的大小必须相同。
- [`iteri2: action:(int -> 'T1 -> 'T2 -> unit) -> list1:'T1 list -> list2:'T2 list -> unit`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L391)。与 `iteri` 类似，但从两个长度相等的列表中映射相应的元素。
- [`forall2: predicate:('T1 -> 'T2 -> bool) -> list1:'T1 list -> list2:'T2 list -> bool`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L314)。谓词应用于匹配两个集合中的元素，直到集合的两个长度中的较小者。如果任何应用程序返回false，则总体结果为 false，否则为 true。
- [`exists2: predicate:('T1 -> 'T2 -> bool) -> list1:'T1 list -> list2:'T2 list -> bool`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L191)。谓词应用于匹配两个集合中的元素，直到集合的两个长度中的较小者。如果任何应用程序返回true，则总体结果为true，否则为false。
- [`fold2<'T1,'T2,'State> : folder:('State -> 'T1 -> 'T2 -> 'State) -> state:'State -> list1:'T1 list -> list2:'T2 list -> 'State`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L266)。将函数应用于两个集合的相应元素，在计算过程中使用累加器参数。
- [`foldBack2<'T1,'T2,'State> : folder:('T1 -> 'T2 -> 'State -> 'State) -> list1:'T1 list -> list2:'T2 list -> state:'State -> 'State`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L288)。将函数应用于两个集合的相应元素，在计算过程中使用累加器参数。
- [`compareWith: comparer:('T -> 'T -> int) -> list1:'T list -> list2:'T list -> int`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L84)。使用给定的比较函数逐个元素比较两个集合。返回比较函数的第一个非零结果。如果到达集合的末尾，如果第一个集合较短，则返回 -1，如果第二个集合较短时，则返回 1。
- 另请参阅第 26 节：组合和取消组合集合中的 `append`、`concat` 和 `zip`。

### 使用示例

这些功能使用起来很简单：

```F#
let intList1 = [2;3;4]
let intList2 = [5;6;7]

List.map2 (fun i1 i2 -> i1 + i2) intList1 intList2
//  [7; 9; 11]

// TIP use the ||> operator to pipe a tuple as two arguments
(intList1,intList2) ||> List.map2 (fun i1 i2 -> i1 + i2)
//  [7; 9; 11]

(intList1,intList2) ||> List.mapi2 (fun index i1 i2 -> index,i1 + i2)
 // [(0, 7); (1, 9); (2, 11)]

(intList1,intList2) ||> List.iter2 (printf "i1=%i i2=%i; ")
// i1=2 i2=5; i1=3 i2=6; i1=4 i2=7;

(intList1,intList2) ||> List.iteri2 (printf "index=%i i1=%i i2=%i; ")
// index=0 i1=2 i2=5; index=1 i1=3 i2=6; index=2 i1=4 i2=7;

(intList1,intList2) ||> List.forall2 (fun i1 i2 -> i1 < i2)
// true

(intList1,intList2) ||> List.exists2 (fun i1 i2 -> i1+10 > i2)
// true

(intList1,intList2) ||> List.fold2 (fun state i1 i2 -> (10*state) + i1 + i2) 0
// 801 = 234 + 567

List.foldBack2 (fun i1 i2 state -> i1 + i2 + (10*state)) intList1 intList2 0
// 1197 = 432 + 765

(intList1,intList2) ||> List.compareWith (fun i1 i2 -> i1.CompareTo(i2))
// -1

(intList1,intList2) ||> List.append
// [2; 3; 4; 5; 6; 7]

[intList1;intList2] |> List.concat
// [2; 3; 4; 5; 6; 7]

(intList1,intList2) ||> List.zip
// [(2, 5); (3, 6); (4, 7)]
```

### 需要一个不在这里的函数吗？

通过使用 `fold2` 和 `foldBack2`，您可以轻松创建自己的函数。例如，一些 `filter2` 函数可以这样定义：

```F#
/// Apply a function to each element in a pair
/// If either result passes, include that pair in the result
let filterOr2 filterPredicate list1 list2 =
    let pass e = filterPredicate e
    let folder e1 e2 state =
        if (pass e1) || (pass e2) then
            (e1,e2)::state
        else
            state
    List.foldBack2 folder list1 list2 ([])

/// Apply a function to each element in a pair
/// Only if both results pass, include that pair in the result
let filterAnd2 filterPredicate list1 list2 =
    let pass e = filterPredicate e
    let folder e1 e2 state =
        if (pass e1) && (pass e2) then
            (e1,e2)::state
        else
            state
    List.foldBack2 folder list1 list2 []

// test it
let startsWithA (s:string) = (s.[0] = 'A')
let strList1 = ["A1"; "A3"]
let strList2 = ["A2"; "B1"]

(strList1, strList2) ||> filterOr2 startsWithA
// [("A1", "A2"); ("A3", "B1")]
(strList1, strList2) ||> filterAnd2 startsWithA
// [("A1", "A2")]
```

另见第 25 节。

## 24.处理三个列表

如果您有三个列表，则只有一个内置函数可用。但是，请参阅第 25 节，了解如何构建自己的三个列表函数的示例。

- [`map3: mapping:('T1 -> 'T2 -> 'T3 -> 'U) -> list1:'T1 list -> list2:'T2 list -> list3:'T3 list -> 'U list`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L438)。构建一个新的集合，其元素是将给定函数同时应用于三个集合的相应元素的结果。
- 另请参阅第 26 节：组合和取消组合集合中的 `append`、`concat` 和 `zip3`。

## 25.处理三个以上的列表

如果您正在处理三个以上的列表，则没有内置函数适合您。

如果这种情况不经常发生，那么你可以连续使用 `zip2` 和/或 `zip3` 将列表折叠成一个元组，然后使用 `map` 处理该元组。

或者，您可以使用应用子（applicatives）将您的函数“提升”到“压缩列表”的世界。

```F#
let (<*>) fList xList =
    List.map2 (fun f x -> f x) fList xList

let (<!>) = List.map

let addFourParams x y z w =
    x + y + z + w

// lift "addFourParams" to List world and pass lists as parameters rather than ints
addFourParams <!> [1;2;3] <*> [1;2;3] <*> [1;2;3] <*> [1;2;3]
// Result = [4; 8; 12]
```

如果这看起来很神奇，请参阅本系列文章，了解此代码的作用。

## 26.合并和取消合并集合

最后，有许多函数可以组合和取消组合集合。

- [`append: list1:'T list -> list2:'T list -> 'T list`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L21)。返回一个新的集合，该集合包含第一个集合的元素和第二个集合的后续元素。
- `@` 是 `append` 对列表的中缀版本。
- [`concat: lists:seq<'T list> -> 'T list`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L90)。构建一个新的集合，其元素是将给定函数同时应用于集合中相应元素的结果。
- [`zip: list1:'T1 list -> list2:'T2 list -> ('T1 * 'T2) list`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L882)。将两个集合组合成一对列表。这两个集合的长度必须相等。
- [`zip3: list1:'T1 list -> list2:'T2 list -> list3:'T3 list -> ('T1 * 'T2 * 'T3) list`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L890)。将三个集合组合成一个三元组列表。集合的长度必须相等。
- （Seq除外） [`unzip: list:('T1 * 'T2) list -> ('T1 list * 'T2 list)`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L852)。将成对的集合拆分为两个集合。
- （除序列号外） [`unzip3: list:('T1 * 'T2 * 'T3) list -> ('T1 list * 'T2 list * 'T3 list)`](https://github.com/fsharp/fsharp/blob/4331dca3648598223204eed6bfad2b41096eec8a/src/fsharp/FSharp.Core/list.fsi#L858)。将一个三元组集合拆分为三个集合。

### 使用示例

这些功能使用起来很简单：

```F#
List.append [1;2;3] [4;5;6]
// [1; 2; 3; 4; 5; 6]

[1;2;3] @ [4;5;6]
// [1; 2; 3; 4; 5; 6]

List.concat [ [1]; [2;3]; [4;5;6] ]
// [1; 2; 3; 4; 5; 6]

List.zip [1;2] [10;20]
// [(1, 10); (2, 20)]

List.zip3 [1;2] [10;20] [100;200]
// [(1, 10, 100); (2, 20, 200)]

List.unzip [(1, 10); (2, 20)]
// ([1; 2], [10; 20])

List.unzip3 [(1, 10, 100); (2, 20, 200)]
// ([1; 2], [10; 20], [100; 200])
```

请注意，`zip` 函数要求长度相同。

```F#
List.zip [1;2] [10]
// ArgumentException: The lists had different lengths.
```

## 27.其他仅数组功能

数组是可变的，因此有一些函数不适用于列表和序列。

- 请参阅第15节中的“就地排序”功能
- `Array.blit: source:'T[] -> sourceIndex:int -> target:'T[] -> targetIndex:int -> count:int -> unit`。从第一个数组中读取一系列元素并将其写入第二个数组。
- `Array.copy: array:'T[] -> 'T[]`。构建一个包含给定数组元素的新数组。
- `Array.fill: target:'T[] -> targetIndex:int -> count:int -> value:'T -> unit`。用给定值填充数组的一系列元素。
- `Array.set: array:'T[] -> index:int -> value:'T -> unit`。设置数组的元素。
- 除此之外，所有其他 BCL 数组函数也可用。

我不会举例子。请参阅 MSDN 文档。

## 28.使用 disposables 的序列

`List.ofSeq` 等转换函数的一个重要用途是将懒惰枚举（`seq`）转换为完全求值的集合，如 `list`。当涉及可支配资源（如文件句柄或数据库连接）时，这一点尤为重要。如果在资源可用时未将序列转换为列表，则在资源处置后稍后访问元素时可能会遇到错误。

这将是一个扩展示例，所以让我们从一些模拟数据库和 UI 的辅助函数开始：

```F#
// a disposable database connection
let DbConnection() =
    printfn "Opening connection"
    { new System.IDisposable with
        member this.Dispose() =
            printfn "Disposing connection" }

// read some records from the database
let readNCustomersFromDb dbConnection n =
    let makeCustomer i =
        sprintf "Customer %i" i

    seq {
        for i = 1 to n do
            let customer = makeCustomer i
            printfn "Loading %s from db" customer
            yield customer
        }

// show some records on the screen
let showCustomersinUI customers =
    customers |> Seq.iter (printfn "Showing %s in UI")
```

简单的实现将导致在连接关闭后对序列进行评估：

```F#
let readCustomersFromDb() =
    use dbConnection = DbConnection()
    let results = readNCustomersFromDb dbConnection 2
    results

let customers = readCustomersFromDb()
customers |> showCustomersinUI
```

输出如下。您可以看到连接已关闭，只有到那时才能对序列进行评估。

```
Opening connection
Disposing connection
Loading Customer 1 from db  // error! connection closed!
Showing Customer 1 in UI
Loading Customer 2 from db
Showing Customer 2 in UI
```

更好的实现将在连接打开时将序列转换为列表，从而立即对序列进行评估：

```F#
let readCustomersFromDb() =
    use dbConnection = DbConnection()
    let results = readNCustomersFromDb dbConnection 2
    results |> List.ofSeq
    // Convert to list while connection is open

let customers = readCustomersFromDb()
customers |> showCustomersinUI
```

结果好多了。在处理连接之前加载所有记录：

```
Opening connection
Loading Customer 1 from db
Loading Customer 2 from db
Disposing connection
Showing Customer 1 in UI
Showing Customer 2 in UI
```

第三种选择是将 disposable 嵌入序列本身：

```F#
let readCustomersFromDb() =
    seq {
        // put disposable inside the sequence
        use dbConnection = DbConnection()
        yield! readNCustomersFromDb dbConnection 2
        }

let customers = readCustomersFromDb()
customers |> showCustomersinUI
```

输出显示，现在 UI 显示也在连接打开时完成：

```
Opening connection
Loading Customer 1 from db
Showing Customer 1 in UI
Loading Customer 2 from db
Showing Customer 2 in UI
Disposing connection
```

这可能是坏事（连接保持打开的时间更长），也可能是好事（内存使用最少），具体取决于上下文。

## 29.冒险的结束

你坚持到了最后——做得好！不过，这并不是一次真正的冒险，是吗？没有龙或任何东西。尽管如此，我希望这有帮助。



# “计算表达式”系列

https://fsharpforfunandprofit.com/series/computation-expressions/

在本系列中，您将学习什么是计算表达式、一些常见模式以及如何创建自己的计算表达式。在此过程中，我们还将研究 continuation、bind 函数、包装器类型等。

1. 计算表达式：简介
   解开谜团。。。
2. 理解延续
   “let”如何在幕后运作
3. 引入“绑定”
   迈向创造我们自己的“let!”
4. 计算表达式和包装器类型
   使用类型来辅助工作流程
5. 更多关于包装类型的信息
   我们发现，即使是列表也可以是包装器类型
6. 实现 CE：Zero 和 Yield
   开始使用基本的构建器方法
7. 实现 CE：Combine
   如何一次返回多个值
8. 实现 CE：Delay 和 Run
   控制功能何时执行
9. 实现 CE：重载
   愚蠢的方法技巧
10. 实现 CE：增加惰性
    在外部延迟工作流程
11. 实现 CE：其他标准方法
    实现 While、Using 和异常处理



## [跳转系列独立 Markdown](./FSharpForFunAndProfit翻译-“计算表达式”系列.md)



# “F#面向对象编程”系列

https://fsharpforfunandprofit.com/series/object-oriented-programming-in-fsharp/

正如之前多次强调的那样，F# 本质上是一种函数式语言，但 OO 特性已经很好地集成在一起，没有“附加”的感觉。因此，将 F# 作为面向对象语言，作为 C# 的替代品，是非常可行的。

在本系列中，我们将探讨 F# 如何支持面向对象的类和方法。

1. F# 中的面向对象编程：简介
2. 类
3. 继承和抽象类
4. 接口
5. 对象表达式



## [跳转系列独立 Markdown](./FSharpForFunAndProfit翻译-“F#面向对象编程”系列.md)



# “函数式应用程序的配方”系列

- 如何设计和编写一个完整的程序
  函数式应用程序的配方，第 1 部分
- 面向铁路的编程
  函数式应用程序的配方，第 2 部分
- 在项目中组织模块
  函数式应用程序的配方，第 3 部分



## [跳转系列独立 Markdown](./FSharpForFunAndProfit翻译-“函数式应用程序的配方”系列.md)



# “依赖循环”系列

https://fsharpforfunandprofit.com/series/dependency-cycles/

关于 F# 最常见的抱怨之一是，它要求代码按照依赖顺序排列。也就是说，您不能对编译器尚未看到的代码使用正向引用。

在本系列中，我将讨论依赖循环，为什么它们是坏的，以及如何摆脱它们。

1. 循环依赖是邪恶的
   循环依赖关系：第1部分
2. 重构以消除循环依赖关系
   循环依赖关系：第2部分
3. 野外的循环和模块化
   比较 C# 和 F# 项目的一些实际指标



## [跳转系列独立 Markdown](./FSharpForFunAndProfit翻译-“依赖循环”系列.md)



# “从C#移植”系列

https://fsharpforfunandprofit.com/series/porting-from-csharp/

您想将 C# 代码移植到 F# 吗？在本系列文章中，我们将探讨实现这一目标的各种方法，以及所涉及的设计决策和权衡。

1. 从 C# 到 F# 的移植：引言
   将现有 C# 代码移植到 F# 的三种方法
2. 开始使用直接移植
   F# 相当于 C#



## [跳转系列独立 Markdown](./FSharpForFunAndProfit翻译-“从C#移植”系列.md)



# 领域驱动设计

我演讲的幻灯片和视频“用 F# 类型系统实现领域建模功能”

https://fsharpforfunandprofit.com/ddd/

此页面包含我演讲“领域建模函数化”中的幻灯片、视频和代码的链接。

以下是演讲的简介：

> 像 F# 这样的静态类型函数式编程语言鼓励人们以一种非常不同的方式思考类型。类型系统是你的朋友，而不是烦人的东西，它可以以许多面向对象程序员可能不熟悉的方式使用。
>
> 类型可用于以细粒度、自文档化的方式表示域。在许多情况下，类型甚至可以用来对业务规则进行编码，这样你就不会创建错误的代码。然后，您可以将静态类型检查几乎用作即时单元测试，以确保您的代码在编译时是正确的。
>
> 在本次演讲中，我们将通过 F# 中的一些简单的现实世界示例，探讨将类型用作领域驱动设计过程的一部分的一些方法。没有行话，没有数学，也不需要之前的 F# 经验。

我也计划很快上传一些关于这些主题的帖子。同时，请参阅“用类型设计”系列，该系列涵盖了类似的领域。

您还应该阅读 Tomas Petricek 的“为什么类型优先开发很重要”和 Lev Gorodinski 的一系列文章。

## 视频

（点击图片观看视频）

奥斯陆 NDC 视频，2017 年 6 月

## 幻灯片

F# 型系统的领域驱动设计——从我在 Slideshare 上的幻灯片看 2014 年 F# 函数式伦敦人

幻灯片也可从以下链接获得：

NDC 伦敦 2013 版（Slideshare），（Github）
F# 函数式伦敦人 2014 版（Speakerdeck），增加了关于为什么 OO 而不是 FP 是可怕的部分，以及使用状态和转换进行设计。

## 书

我有一本关于这个主题的书——你可以在书籍页面上找到更多细节。

领域建模函数化

## 获取代码

如果你想跟随代码，那么：

- 如果您在本地安装了 F#，请下载此文件。
- 如果您没有在本地安装 F#，可以在您的web浏览器中运行代码：[tryfsharp.org/create/scottw/ddd.fsx](http://www.tryfsharp.org/create/scottw/ddd.fsx)



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



## [跳转系列独立 Markdown](./FSharpForFunAndProfit翻译-“用类型设计”系列.md)



# 代数类型大小和领域建模

理解基数以获得乐趣和利润
2015年8月13日 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/type-size-and-design/

在这篇文章中，我们将看看如何计算代数类型的“大小”或基数，并看看这些知识如何帮助我们做出设计决策。

## 开始使用

我将通过将类型视为一个集合并计算可能元素的数量来定义类型的“大小”。

例如，有两种可能的 `Boolean`，因此布尔类型的大小为 2。

有尺寸为 1 的型号吗？是 –  `unit` 类型只有一个值：`()`。

有尺寸为零的类型吗？也就是说，是否存在一个完全没有值的类型？在 F# 中没有，但在 Haskell 中有。它被称为 `Void`。

像这样的类型怎么样：

```F#
type ThreeState =
    | Checked
    | Unchecked
    | Unknown
```

它的尺寸是多少？有三个可能的值，因此大小为三。

像这样的类型怎么样：

```F#
type Direction =
    | North
    | East
    | South
    | West
```

显然，四。

我想你明白了！

## 计算复合类型的大小

现在让我们来看看计算复合类型的大小。如果你还记得从理解 F# 类型系列开始，有两种代数类型：“乘积”类型，如元组和记录，以及“求和”类型，在 F# 中称为可区分联合。

例如，假设我们有一个 `Speed` 和一个 `Direction`，我们将它们组合成一个名为 `Velocity` 的记录类型：

```F#
type Speed =
    | Slow
    | Fast

type Velocity = {
    direction: Direction
    speed: Speed
    }
```

`Velocity` 的大小是多少？

以下是所有可能的值：

```F#
{direction=North; speed=Slow}; {direction=North; speed=Fast}
{direction=East;  speed=Slow}; {direction=East;  speed=Fast}
{direction=South; speed=Slow}; {direction=South; speed=Fast}
{direction=West;  speed=Slow}; {direction=West;  speed=Fast}
```

有八个可能的值，每个值对应两个速度值和四个方向值的每个可能组合。

我们可以将其概括为一条规则：

- **规则：产品类型的大小是组件类型大小的乘积。**

也就是说，给定这样的记录类型：

```F#
type RecordType = {
    a : TypeA
    b : TypeB }
```

大小计算如下：

```F#
size(RecordType) = size(TypeA) * size(TypeB)
```

对于元组也是如此：

```F#
type TupleType = TypeA * TypeB
```

尺寸为：

```F#
size(TupleType) = size(TypeA) * size(TypeB)
```

### 总和类型

求和类型也可以用同样的方法进行分析。给定这样定义的 `Movement` 类型：

```F#
type Movement =
    | Moving of Direction
    | NotMoving
```

我们可以写出并计算所有的可能性：

```
Moving North
Moving East
Moving South
Moving West
NotMoving
```

所以，总共五个。恰好是 `size(Direction) + 1`。这是另一个有趣的：

```F#
type ThingsYouCanSay =
    | Yes
    | Stop
    | Goodbye

type ThingsICanSay =
    | No
    | GoGoGo
    | Hello

type HelloGoodbye =
    | YouSay of ThingsYouCanSay
    | ISay of ThingsICanSay
```

同样，我们可以写出并计算所有的可能性：

```
YouSay Yes
ISay No
YouSay Stop
ISay GoGoGo
YouSay Goodbye
ISay Hello
```

YouSay 案例中有三个可能的值，ISay 案例中也有三个可能性的值，总共有六个。

同样，我们可以制定一个一般规则。

- **规则：求和或并集类型的大小是组件类型大小的总和**。

也就是说，给定这样的联合类型：

```F#
type SumType =
    | CaseA of TypeA
    | CaseB of TypeB
```

大小计算如下：

```
size(SumType) = size(TypeA) + size(TypeB)
```

## 使用泛型类型

如果我们将泛型类型加入其中，会发生什么？

例如，这样的类型的大小是多少：

```F#
type Optional<'a> =
    | Something of 'a
    | Nothing
```

首先，`Optional<'a>` 不是一个类型，而是一个类型构造函数。`Option<string>` 是一种类型。`Optional<int>` 是一种类型，但 `Optional<'a>` 不是。

尽管如此，我们仍然可以通过注意 `size(Optional<string>)` 只是 `size(string) + 1`，`size(Optionally<int>)` 就是 `size(int) + 1` 来计算它的大小，以此类推。

所以我们可以说：

```F#
size(Optional<'a>) = size('a) + 1
```

同样，对于具有两个泛型的类型，如下所示：

```F#
type Either<'a,'b> =
    | Left of 'a
    | Right of 'b
```

我们可以说，它的大小可以使用通用组件的大小来计算（使用上面的“求和规则”）：

```
size(Either<'a,'b>) = size('a) + size('b)
```

## 递归类型

递归类型呢？让我们看看最简单的一个，链表。

链表要么是空的，要么它有一个带有元组的单元格：一个头和一个尾。头部是 `'a`，尾部是另一个列表。定义如下：

```F#
type LinkedList<'a> =
    | Empty
    | Node of head:'a * tail:LinkedList<'a>
```

为了计算大小，让我们为各个组件指定一些名称：

```F#
let S = size(LinkedList<'a>)
let N = size('a)
```

现在我们可以写：

```F#
S =
    1         // Size of "Empty" case
    +         // Union type
    N * S     // Size of "Cell" case using tuple size calculation
```

让我们来试试这个公式。我们从以下几点开始：

```
S = 1 + (N * S)
```

让我们用公式替换最后一个 S，得到：

```
S = 1 + (N * (1 + (N * S)))
```

如果我们把它清理干净，我们会得到：

```
S = 1 + N + (N^2 * S)
```

（其中 `N^2` 表示“N的平方”）

让我们再次用公式替换最后一个s：

```
S = 1 + N + (N^2 * (1 + (N * S)))
```

再次清理：

```
S = 1 + N + N^2 + (N^3 * S)
```

你可以看到这是怎么回事！`S` 的公式可以无限扩展为：

```
S = 1 + N + N^2 + N^3 + N^4 + N^5 + ...
```

我们如何解释这一点？那么，我们可以说列表是以下情况的结合：

- 空列表（大小=1）
- 单元素列表（大小=N）
- 双元素列表（大小=N x N）
- 三元素列表（大小=N x N x N）
- 等等。

这个公式已经捕捉到了这一点。

另外，您可以直接使用公式 `S = 1/(1-N)` 计算 `S`，这意味着 `Direction` 列表（大小=4）的大小为“-1/3”。嗯，真奇怪！这让我想起了这个“-1/12”视频。

## 计算函数的大小

那么函数呢？它们可以调整大小吗？

是的，我们需要做的就是写下每一个可能的实现，并对其进行计数。容易的！

例如，假设我们有一个函数 `SuiteColor`，它将卡片 `Suit` 映射到红色或黑色。

```F#
type Suit = Heart | Spade | Diamond | Club
type Color = Red | Black

type SuitColor = Suit -> Color
```

一种实现方式是返回红色，无论提供何种花色：

```F#
(Heart -> Red); (Spade -> Red); (Diamond -> Red); (Club -> Red)
```

另一种实施方式是，除 `Club` 外，所有花色都将返回红色：

```F#
(Heart -> Red); (Spade -> Red); (Diamond -> Red); (Club -> Black)
```

事实上，我们可以写下这个函数的所有16种可能实现：

```F#
(Heart -> Red); (Spade -> Red); (Diamond -> Red); (Club -> Red)
(Heart -> Red); (Spade -> Red); (Diamond -> Red); (Club -> Black)
(Heart -> Red); (Spade -> Red); (Diamond -> Black); (Club -> Red)
(Heart -> Red); (Spade -> Red); (Diamond -> Black); (Club -> Black)

(Heart -> Red); (Spade -> Black); (Diamond -> Red); (Club -> Red)
(Heart -> Red); (Spade -> Black); (Diamond -> Red); (Club -> Black)  // the right one!
(Heart -> Red); (Spade -> Black); (Diamond -> Black); (Club -> Red)
(Heart -> Red); (Spade -> Black); (Diamond -> Black); (Club -> Black)

(Heart -> Black); (Spade -> Red); (Diamond -> Red); (Club -> Red)
(Heart -> Black); (Spade -> Red); (Diamond -> Red); (Club -> Black)
(Heart -> Black); (Spade -> Red); (Diamond -> Black); (Club -> Red)
(Heart -> Black); (Spade -> Red); (Diamond -> Black); (Club -> Black)

(Heart -> Black); (Spade -> Black); (Diamond -> Red); (Club -> Red)
(Heart -> Black); (Spade -> Black); (Diamond -> Red); (Club -> Black)
(Heart -> Black); (Spade -> Black); (Diamond -> Black); (Club -> Red)
(Heart -> Black); (Spade -> Black); (Diamond -> Black); (Club -> Black)
```

另一种思考方式是，我们可以定义一个记录类型，其中每个值代表一个特定的实现：我们为 `Heart` 输入返回哪种颜色，为 `Spade` 输入返回哪一种颜色，等等。

因此，`SuiteColor` 实现的类型定义如下：

```F#
type SuitColorImplementation = {
    Heart : Color
    Spade : Color
    Diamond : Color
    Club : Color }
```

此记录类型的大小是多少？

```
size(SuitColorImplementation) = size(Color) * size(Color) * size(Color) * size(Color)
```

这里有四种 `size(Color)` 。换句话说，每个输入都有一个 `size(Color)` ，所以我们可以这样写：

```F#
size(SuitColorImplementation) = size(Color) to the power of size(Suit)
```

一般来说，给定一个函数类型：

```F#
type Function<'input,'output> = 'input -> 'output
```

函数的大小是 `size(output type)` 的 `size(input type)` 次幂：

```
size(Function) =  size(output) ^ size(input)
```

让我们也把它编纂成一条规则：

- **规则：函数类型的大小是 `size(output type)` 的 `size(input type)` 次幂**。

## 类型之间的转换

好吧，这些都很有趣，但有用吗？

是的，我认为是的。我认为理解这样的类型大小有助于我们设计从一种类型到另一种类型的转换，这是我们经常做的事情！

假设我们有一个联合类型和一个记录类型，两者都表示是/否答案：

```F#
type YesNoUnion =
    | Yes
    | No

type YesNoRecord = {
    isYes: bool }
```

我们如何在它们之间进行映射？

它们的大小都为 2，因此我们应该能够将一种类型中的每个值映射到另一种类型，反之亦然：

```F#
let toUnion yesNoRecord =
    if yesNoRecord.isYes then
        Yes
    else
        No

let toRecord yesNoUnion =
    match yesNoUnion with
    | Yes -> {isYes = true}
    | No ->  {isYes = false}
```

这就是你所说的“无损”转换。如果您往返转换，则可以恢复原始值。数学家称之为同构（源自希腊语“相等形状”）。

另一个例子呢？这是一个有三个案例的类型，yes，no，和 maybe。

```F#
type YesNoMaybe =
    | Yes
    | No
    | Maybe
```

我们能无损地将其转换为这种类型吗？

```F#
type YesNoOption = { maybeIsYes: bool option }
```

那么，`option` 的大小是多少？1 加上内部类型的大小，在本例中为 `bool`。所以 `size(YesNoOption)` 也是三。

以下是转换函数：

```F#
let toYesNoMaybe yesNoOption =
    match yesNoOption.maybeIsYes with
    | None -> Maybe
    | Some b -> if b then Yes else No

let toYesNoOption yesNoMaybe =
    match yesNoMaybe with
    | Yes ->   {maybeIsYes = Some true}
    | No ->    {maybeIsYes = Some false}
    | Maybe -> {maybeIsYes = None}
```

所以我们可以制定一个规则：

- **规则：如果两种类型的大小相同，则可以创建一对无损转换函数**

让我们试试。这里有一个 `Nibble` 类型和一个 `TwoNibbles` 类型：

```F#
type Nibble = {
    bit1: bool
    bit2: bool
    bit3: bool
    bit4: bool }

type TwoNibbles = {
    high: Nibble
    low: Nibble }
```

我们可以将 `TwoNibbles` 转换为 `byte` 并返回吗？

`Nibble` 的大小是 `2 x 2 x 2 x 2` =16（使用乘积大小规则），`TwoNibbles` 的大小是 size(Nibble) x size(Nibble)，或 `16 x 16`，即 256。

所以，是的，我们可以将 `TwoNibbles` 转换为 `byte`，然后再转换回来。

## 有损转化

如果类型大小不同，会发生什么？

如果目标类型比源类型“大”，则始终可以无损失地映射，但如果目标类型“小”于源类型，则会出现问题。

例如，`int` 类型小于 `string` 类型。你可以准确地将 `int` 转换为 `string`，但你不能轻易地将 `string` 转换为 `int`。

如果你确实想将字符串映射到 int，那么一些非整数字符串必须映射到目标类型中的特殊非整数值：



换句话说，我们从大小中知道，目标类型不能只是 `int` 类型，它必须是 `int+1` 类型。换句话说，一个选项类型！

有趣的是，BCL 中的 `Int32.TryParse` 函数返回两个值，一个是成功/失败的 `bool` 值，另一个是解析结果为 `int` 值。换句话说，一个元组 `bool * int`。

该元组的大小为 `2 x int`，实际需要更多的值。选项类型 ftw！

现在，假设我们正在从 `string` 转换为 `Direction`。有些字符串是有效的，但大多数字符串都是无效的。但这一次，与其说有一个无效的情况，不如说我们想区分空输入、太长的输入和其他无效输入。



我们无法再使用Option对目标进行建模，因此让我们设计一个包含所有七种情况的自定义类型：

```F#
type StringToDirection_V1 =
    | North
    | East
    | South
    | West
    | Empty
    | NotValid
    | TooLong
```

但这种设计混淆了成功的转换和失败的转换。为什么不把它们分开呢？

```F#
type Direction =
    | North
    | East
    | South
    | West

type ConversionFailure =
    | Empty
    | NotValid
    | TooLong

type StringToDirection_V2 =
    | Success of Direction
    | Failure of ConversionFailure
```

`StringToDirection_V2` 的大小是多少？

在 `Success` 案例中有4个 `Direction` 选项，在 `Failure` 案例中有3个 `ConversionFailure` 选项，因此总大小为 7，与第一个版本一样。

换句话说，这两种设计是等效的，我们可以使用其中任何一种。

就个人而言，我更喜欢版本 2，但如果我们的遗留代码中有版本 1，好消息是我们可以无损地从版本 1 转换到版本 2，然后再转换回来。这反过来意味着，如果需要，我们可以安全地重构到版本 2。

## 设计核心域

知道不同类型可以无损转换，可以让你根据需要调整你的域名设计。

例如，这种类型：

```F#
type Something_V1 =
    | CaseA1 of TypeX * TypeY
    | CaseA2 of TypeX * TypeZ
```

可以无损地转换为这个：

```F#
type Inner =
    | CaseA1 of TypeY
    | CaseA2 of TypeZ

type Something_V2 =
    TypeX * Inner
```

或者这个：

```F#
type Something_V3 = {
    x: TypeX
    inner: Inner }
```

下面是一个真实的例子：

- 你有一个网站，有些用户注册了，有些没有。
- 对于所有用户，您都有一个会话 id
- 仅限注册用户，您有额外信息

我们可以这样建模该需求：

```F#
module Customer_V1 =

    type UserInfo = {name:string} //etc
    type SessionId = SessionId of int

    type WebsiteUser =
        | RegisteredUser of SessionId * UserInfo
        | GuestUser of SessionId
```

或者，我们可以将公共 `SessionId` 拉到更高的级别，如下所示：

```F#
module Customer_V2 =

    type UserInfo = {name:string} //etc
    type SessionId = SessionId of int

    type WebsiteUserInfo =
        | RegisteredUser of UserInfo
        | GuestUser

    type WebsiteUser = {
        sessionId : SessionId
        info: WebsiteUserInfo }
```

哪个更好？从某种意义上说，它们都是“相同的”，但显然最好的设计取决于使用模式。

- 如果你更关心用户的类型而不是会话 id，那么版本 1 会更好。
- 如果你经常查看会话 id 而不关心用户类型，那么版本 2 会更好。

知道它们是同构的好处是，如果你愿意，你可以定义这两种类型，在不同的上下文中使用它们，并根据需要在它们之间进行无损映射。

## 与外界的联系

我们有所有这些不错的域名类型，如 `Direction` 或 `WebsiteUser`，但在某些时候，我们需要与外界进行交互——将它们存储在数据库中，以 JSON 格式接收它们，等等。

问题是外界没有一个好的类型系统！一切都趋向于原始：字符串、整数和布尔。

从我们的域到外部世界意味着从具有“小”值集的类型到具有“大”值集，我们可以直接做到这一点。但是，从外部世界进入我们的领域意味着从一组“大”值变成一组“小”值，这需要验证和错误案例。

例如，域类型可能看起来像这样：

```F#
type DomainCustomer = {
    Name: String50
    Email: EmailAddress
    Age: PositiveIntegerLessThan130 }
```

这些值受到限制：姓名最多 50 个字符，一封经过验证的电子邮件，年龄在 1 到 129 之间。

另一方面，DTO 类型可能看起来像这样：

```F#
type CustomerDTO = {
    Name: string
    Email: string
    Age: int }
```

这些值不受约束：姓名的任何字符串、未验证的电子邮件、年龄可以是2^32个不同值中的任何一个，包括负数。

这意味着我们无法创建 `CustomerDTO` 到 `DomainCustomer` 的映射。我们必须至少有一个其他值（`DomainCustomer + 1`）来映射无效输入，最好有更多值来记录各种错误。

这自然导致了我在函数式错误处理演讲中描述的 `Success/Failure` 模型，

映射的最终版本将是从 `CustomerDTO` 到 `SuccessFailure<DomainCustomer>` 或类似对象。

这就引出了最后一条规则：

- **规则：不要相信任何人。如果从外部源导入数据，请务必处理无效输入。**

如果我们认真对待这条规则，它会产生一些连锁反应，例如：

- 永远不要尝试直接反序列化到域类型（例如没有 ORM），只反序列化到 DTO 类型。
- 始终验证从数据库或其他“可信”来源读取的每条记录。

你可能会认为把所有东西都包装成 `Success/Failure` 类型会让人恼火，这是真的（！），但有一些方法可以让这更容易。例如，请参阅这篇文章。

## 进一步阅读

代数数据类型的“代数”是众所周知的。克里斯·泰勒最近在《代数数据类型的代数（和微积分！）》和一系列文章中做了很好的总结。

在我写了这篇文章之后，有人向我指出了两篇类似的帖子：

- Tomas Petricek的一篇几乎内容相同！
- Bartosz Milewski在其关于范畴理论的系列文章中。

正如其中一些帖子所提到的，你可以用这些类型公式做奇怪的事情，比如区分它们！

如果你喜欢学术论文，你可以阅读 Conor McBride 2001 年发表的“正则类型的导数是它的单洞上下文类型”（PDF）中关于导数的原始讨论，以及“区分数据结构”（PDF）[Abbott，Altenkirch，Ghani 和 McBride，2005] 中的后续讨论。

## 摘要

这可能不是世界上最令人兴奋的话题，但我发现这种方法既有趣又有用，我想与大家分享。

让我知道你的想法。感谢您的阅读！



# 组合的力量

为什么组合是函数式编程的基本原则

https://fsharpforfunandprofit.com/composition/

本页包含我演讲“组合的力量”中的幻灯片和代码的链接。

以下是演讲的简介：

> 组合是函数式编程的基本原则，但它与面向对象的方法有什么不同，你在实践中如何使用它？
>
> 在本次面向初学者的演讲中，我们将首先介绍函数式编程的基本概念，然后探讨一些不同的方法，即组合可以用来从小事物构建大事物。
>
> 之后，我们将看到如何在实践中使用组合，从一个简单的 FizzBuzz 示例开始，到一个完整的（无对象！）web 应用程序结束。

## 视频

2019年莫斯科 dotNext 视频（点击图片查看视频）

2019年莫斯科 dotNext 视频

## 幻灯片

2018年1月17日伦敦 NDC 幻灯片

从我在 Slideshare 上的幻灯片看构图的力量



# 观察乌龟的十三种方法

https://fsharpforfunandprofit.com/turtle/

本页包含我演讲“观察乌龟的十三种方法”中的幻灯片和代码的链接。

以下是演讲的简介：

> 在这篇快节奏的演讲中，我将从著名的乌龟图形API开始，然后通过创建13个不同的实现将其扩展到临界点，每个实现演示不同的功能编程技术。
>
> 在此过程中，我们将使用部分应用程序、函数依赖注入、成功/失败结果验证、要么单子、状态单子、消息队列代理、事件源、流处理、基于能力的设计和解释器模式（又名自由单子）。呼！

这次演讲是基于我关于这个话题的博客文章：

- 观察乌龟的十三种方法
- 看乌龟的十三种方法，第二部分

## 视频

视频来自NDC伦敦，2017年1月18日。为橙色调道歉！

NDC伦敦，2017年1月18日

## 幻灯片

2017年1月18日伦敦NDC幻灯片


从我在Slideshare上的幻灯片看乌龟的13种方式



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



## [跳转系列独立 Markdown](./FSharpForFunAndProfit翻译-“观察乌龟的十三种方法”系列.md)



# 企业 Tic-Tac-Toe

其中，我荒谬地过度设计了一个简单的游戏，使其“企业就绪”

https://fsharpforfunandprofit.com/ettt/

此页面包含指向我的演讲“Enterprise Tic Tac Toe”中的幻灯片和代码的链接。

以下是演讲的简介：

> 跟随我荒谬地过度设计了一个简单的游戏，以演示如何使用函数式编程来创建一个现实世界的“企业就绪”应用程序。
>
> 所涵盖的主题包括：将业务规则编码为类型、使用参数多态性隐藏数据、使用功能实现基于功能的安全性、使用 HATEAOS 公开 REST API、日志记录的功能方法、参与者、可扩展性等等。

这次演讲是基于我关于这个话题的博客文章：

- 企业Tic Tac Toe
- 企业Tic Tac Toe 第 2 部分，我扔掉了之前的设计

此外，还有我关于基于能力的安全和 DDD 的帖子和演讲：

- 更多关于函数式设计的信息：
- 函数式授权方法
- 基于身份和角色限制能力
- 使用类型作为访问令牌
- 我的“用能力设计”演讲的视频和幻灯片
- 更多关于领域驱动设计的信息：
- 我的“用 F# 类型系统进行领域驱动设计”演讲的视频和幻灯片

## 视频

奥斯陆NDC视频，2015年6月17日（点击图片查看视频）

奥斯陆NDC视频，2015年6月17日

## 幻灯片

2015年6月17日，奥斯陆NDC幻灯片


我在Slideshare上的幻灯片中的企业 Tic Tac Toe



# 用能力设计

设计使用能力和最小权限原则

https://fsharpforfunandprofit.com/cap/

此页面包含指向我演讲“用功能设计”中的幻灯片和代码的链接。

以下是演讲的简介：

> 我们都希望生成易于测试和重构的模块化和健壮的代码，我们有 SOLID 等设计原则可以帮助我们做到这一点。
>
> 在这次演讲中，我将介绍一种使用“能力”和最小权威原则进行设计的非常不同的方法。我将展示如何在整个核心领域（而不仅仅是在 API 边界）使用这些设计技术，从而产生设计良好的模块化代码。
>
> 我将演示如何设计和使用基于功能的方法，如何快速组合和轻松限制功能，以及如何将功能与使用 HATEAOS 的 REST API 自然结合。

这次演讲是基于我关于这个话题的博客文章：

- 函数式授权方法
- 基于身份和角色限制能力
- 使用类型作为访问令牌

此外，还有关于设计Enterprise Tic Tac Toe的演讲和帖子：

- 企业Tic Tac Toe
- 企业Tic Tac Toe第2部分，我扔掉了之前的设计
- 我的“企业Tic Tac Toe”演讲的视频和幻灯片

## 视频

2016年1月15日伦敦NDC视频（点击图片查看视频）

2016年1月15日伦敦NDC视频

## 代码

演示中使用的代码可以在github上找到。

## 聚焦 F# 的幻灯片

2016年1月15日伦敦NDC幻灯片

使用我在Slideshare上的幻灯片中的功能进行设计

## 聚焦 DDD 的幻灯片

这些来自2017年2月3日DDD欧洲的幻灯片更侧重于DDD


我在Slideshare上的幻灯片中展示的“用能力设计”（DDD-EU 2017）



# “了解幺半群”系列

https://fsharpforfunandprofit.com/posts/monoids-without-tears/

1. 没有眼泪的幺半群
   一种常见函数模式的无数学讨论
2. 实践中的幺半群
   没有眼泪的幺半群-第2部分
3. 与非幺半群合作
   没有眼泪的幺半群-第3部分



## [跳转系列独立 Markdown](./FSharpForFunAndProfit翻译-“了解幺半群”系列.md)



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



# 内向和外向的编程语言

语言人格类型的深夜思考
2015年7月18日 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/introvert-vs-extrovert/

> 内向和外向的数学家有什么区别？
> 一个内向的数学家在和你说话的时候看着他的鞋子；一个外向的数学家看着你的鞋子。

长期以来，我一直意识到编程语言向外界展示自己的方式存在差异。

如果编程语言有个性，我会倾向于称其中一些为“内向”，另一些为“外向”。

外向的编程语言完全是关于外部世界的，从来没有比与 IO 和外部数据源聚会更快乐的了。

另一方面，内向的编程语言很乐意独处，如果可能的话，也不愿意与外界打交道。当然，它可以是社交的，在需要的时候与 IO 一起工作，但它觉得这项活动很累，当 IO 回家后，它就放心了，可以回去读一本好书了。

有趣的是，通过查看用户指南或教程中被认为重要的内容，你可以了解很多关于语言个性类型的信息。

例如，经典的《C 编程语言》一书一开始就有 `printf("hello, world\n")`，而大多数其他 C 书也遵循同样的模式。

事实上，C 是一种非常外向的语言。编码示例中充斥着文件和控制台 IO。同样，只需看一眼它们的手册，你就可以看出 PHP、Python 和 Perl 同样外向。

事实上，我想说所有最流行的语言都是外向的，原因很明显。他们散发着自信，他们很容易交到朋友，他们把事情做好。

另一方面，我想说 Haskell 是内向语言的一个很好的例子。

例如，在《Haskell 趣学指南（Learn You A Haskell）》一书中，“hello world”的例子直到第 9 章才出现！在《真实世界 Haskell》中，IO 直到第 7 章才被邀请共进晚餐。

如果你手边没有完整的手册，另一个表明语言内向的线索是，如果它很早就把你介绍给它的亲密朋友，斐波那契函数。内向的语言喜欢递归！

现在，就像在现实世界中一样，内向的语言会被外向的人误解。他们被指责过于傲慢、过于严肃、过于规避风险。

但这并不公平——内向的语言实际上更具反思性和思考性，因此比肤浅、喋喋不休的外向者更有可能有深刻的见解。

## **但是…**

> “所有的概括都是错误的，包括这个”

你可能会认为命令式语言和面向对象语言是外向的，而具有更多声明性范式（函数式、逻辑式）的语言是内向的，但事实并非总是如此。

例如，SQL 是一种声明性语言，但它在生活中的全部目的是数据处理，这使得它在我的书中显得外向。

更接近家庭，F# 是一种函数式第一语言，但很乐意做 IO，事实上，它通过类型提供者和 Deedle 对现实世界的数据处理提供了很好的支持。

正如人不是一个或另一个一样，编程语言也是如此。有一个范围。有些语言非常外向，有些非常内向，有些介于两者之间。

## 很多人说我以自我为中心，但他们已经说得够多了

> “现实只是一种幻觉，尽管是一种非常持久的幻觉。”

有些语言并不完全属于这种性格类型。

以 Smalltalk 为例。

在很多方面，Smalltalk 都非常外向。它对用户界面交互有很多支持，它本身就是首批图形开发环境之一。

但有一个问题。当然，它友好、健谈，非常适合进行激烈的一对一对话，但它也有黑暗的一面——它与他人相处得不好。它只是不情愿地承认操作系统，而不是处理外部库的混乱，更喜欢以自己的完美方式实现东西。

大多数 Lisp 也有同样的失败。理想主义和优雅，但相当孤立。我将这种人格类型称为唯我论。

在他们的辩护中，许多重要的编程概念最初是在唯我主义语言中发展起来的。但遗憾的是，尽管他们是狂热的追随者，但他们从未得到应有的广泛认可。

## 我们从哪里来？我们是什么？我们要去哪里？

这不可避免地把我带到了吞噬一切的黑洞——JavaScript。这在这个愚蠢的计划（scheme）中有什么意义？

显然，它最初的目的是帮助和用户沟通（也用于制作猴子动画），所以乍一看它似乎很外向。

但它在沙箱中运行的事实（直到最近才做过很多种 IO）让我不这么认为。对我来说，最关键的是 node.js。需要服务器语言吗？让我们按照自己的形象创造一个！让我们从头开始写所有的库！

但至少目前看来，这似乎奏效了。所以，唯我论赢了！

## 结束不科学的后记

“大自然让我们轻浮，来安慰我们的痛苦”

在这一点上，我应该总结一下对不同编程社区的一些深刻观察，以及他们的语言偏好对他们的启示。

但实际上，这只是一点异想天开，所以我不会。



# 关于本网站

https://fsharpforfunandprofit.com/about/

这个网站的目的是介绍 .NET 开发人员享受函数式编程的乐趣，尤其是 F#。

使用本网站时，请注意条款和条件。

我希望这个网站能不负众望，证明 F# 不仅编程很有趣，而且对主流商业和企业软件也很有用。F# 不仅仅是一个学术练习，它是有用的。

我的做法是毫无歉意地以 .NET 为中心，非学术性。例如：

- 我经常使用 C# 风格的命名，而不是函数式编程中更隐晦的名称，尤其是在介绍性文章中。
- 我将从面向对象的概念（如设计模式）中进行类比和举例。
- 我将避免许多更复杂的概念（单子（monads）、惰性与尽早求值等），并专注于对 OO 世界的新手最有用的概念：代数类型、模式匹配、高阶函数等。

## 关于我

我是一名与 fsharpWorks 咨询公司合作的开发人员和架构师。我在从高级 UX/HCI 到低级数据库实现的各个领域都有 20 多年的经验。

我用许多语言编写了严肃的代码，我最喜欢的是 Smalltalk、Python，最近还有 F#（因此是这个网站）。

## 常见问题解答

### 大多数代码示例使用函数式风格，而不是面向对象风格。F# 可以做到这两点。为什么强调函数式？你不喜欢 OOP 吗？

我确实喜欢面向对象编程，多年来我一直是一个严肃的 Smalltalker。但对于 F#，我更关注函数式方面而不是面向对象方面，因为对于来自 C# 或 Java 背景的人来说，这就是所有新*概念的所在。像类型推理、函数组合等，在面向对象风格中效果不佳。这些概念正是我认为来 F# 的人应该理解的。

*对于主流开发人员来说，FP 概念是“新的”。FP 概念可以追溯到 20 世纪 20 年代，早在 20 世纪 60 年代就用于编程语言（如果算上 lisp 的话，则更早）。ML 是 F# 的直接祖先，创建于 20 世纪 70 年代初。

### 你为什么写这些长系列而不是短重点帖子？

这不是一个真正的博客，而是一组旨在解释 F# 如何工作的页面。关于 FP 和 F# 有很多优秀的博客，但就我个人而言，当我学习一门语言时，我更喜欢更有条理的方法。F# 中有很多新的*概念，试图零碎地理解它可能会让人感到沮丧。像 Stack Overflow 这样的地方对直接问题有很多好的答案，但其中一些问题表明了对函数式编程的严重误解，而且没有空间解释原因。因此，与其孤立地回答一个问题，我宁愿改变背景，这样这些问题就不会被考虑在内。

### 有些帖子的中间隐藏着一些非常重要的想法。为什么不把这些东西放到自己的帖子里呢？

再一次，答案是上下文。例如，关于为什么 Option.None 和 null 不同的部分在页面底部出现，包含选项类型实际含义（还有一个图表！）我认为，如果你通读整个页面，而不是试图快速得到答案，那么最后一节的内容（以及“null”问题的答案）将是不言而喻的。

我也会在内容页面上突出显示最重要的部分，这样它们就不会完全被埋没！

## 禁忌词

许多无辜的人可能会访问这个网站，所以为了避免冒犯，强烈建议不要使用某些令人讨厌的单词和短语。

这些词包括：“自函子（endofunctor）”、“变形（anamorphism）”、“存在量化（existential quantification）”、“贝塔约化（beta reduction）”、“范畴论（category theory）”、“终结共代数（final coalgebra）”、“克莱斯利箭头（Kleisli arrows）”、“柯里-霍华德同构（Curry–Howard correspondence）”，最糟糕的是，以“m”开头的五个字母的词。

重复使用这些词将导致被禁止。你将被放逐，并被迫与其他不可救药的元素共度时光**

**说真的！正如我在主页上明确指出的那样，这个网站不是针对数学家或 Haskell 程序员的。它针对的是第一次接触函数式编程的大量 C#、VB 和 Python 程序员。对于普通企业程序员来说，F# 是一种非常容易理解的语言，但数学术语会让很多人望而却步（“monad 是自函子范畴中的一个幺半群，有什么问题吗？”），我认为用其原生环境中的概念来解释 F# 要好得多，而不是使用源自其他地方且通常不适用的术语。例如，将 F# 计算表达式视为 Haskell 单体（monads）通常会让事情变得更加混乱。这样做也有助于绕过“如何在语言 X 中完成任何事情？它甚至没有 y”的争论，并专注于 F# 能做什么。

## 反馈意见

请帮我改进这个网站。我很高兴收到任何建设性的批评或意见。

如果您对整个网站有任何意见或建议，请在此页面（下方）上发表评论，或者您可以直接通过 `scottw at fsharpforfunandprofit.com` 与我联系。

如果您想偶尔获得有关该网站的更新，请订阅时事通讯。

## 致谢

首先，非常感谢 Don Syme 和 F# 团队的其他成员，他们创造了一种与 .NET 集成得很好的语言。多亏了他们，一种功能齐全的语言终于有望成为主流。

其次，感谢所有为 StackOverflow 和 hubfs.net（又名 fpish）撰写书籍、博客并做出贡献的 F# 爱好者。我从你们身上学到了很多。

非常感谢那些给我发来更正网站上许多拼写错误的人，特别感谢 André van Meulebrouck。

最后，特别感谢您和所有阅读本网站的人。希望你喜欢。谢谢你的阅读！



# 条款和条件

https://fsharpforfunandprofit.com/about/terms/

这些条款和条件构成您与本网站（以下简称 SITE）和 ScottW（以下简称 ME）的协议。

阅读本网站即表示您同意以下内容：

- 您承认，本网站是由我自己多年来撰写的，许多帖子可能已经过时、不完整或完全错误。你也承认，今天的我对许多旧帖子不满意，但一直懒得更新它们。因此，您承诺始终查看帖子上的时间戳，如果它是古老的，请在阅读时记住这一点。
- 您认识到，本网站不是权威的信息来源。特别是，您同意不使用本网站上的任何内容作为权威的论据。
- 您同意绝不会不加批判地使用本网站上描述的技术。本网站不对损坏、收入损失或您当时认为很酷但后来又讨厌的糟糕代码负责。
- 如果不小心摄入，本网站上的一些技术可能是危险的。如果你担心你的单子（monad）已经免费了，请咨询医生。
- 您承认本网站主要面向初级和中级函数程序员。您应赔偿我因被剥夺“Endofunctor”、“Coyoneda” 和 “Zygohistomorphic premorphism”等词语而给您带来的任何痛苦和折磨。
- 你同意不称我为思想领袖或任何类似的贬义词。你也同意我不是你叔叔。

违反此协议的处罚可能包括但不限于：写 1000 次“I heart Scaled Agile Framework”，在所有社交媒体上公开宣布没有其他框架能像 Enterprise JavaBeans 那样好，或者在函数式编程国际会议上发表题为“为什么 Visual Basic 会摧毁 Haskell”的演讲。

# 许可证

https://fsharpforfunandprofit.com/about/license/

## 文本和图像

所有文字和图片版权所有（c）2012-2021 Scott Wlaschin

## MIT 代码许可证

代码版权所有（c）2012-2021 Scott Wlaschin

本网站上的所有代码均按照免费的 MIT 许可证发布，如下所示：

特此免费授予任何获得本软件和相关文档文件（“软件”）副本的人在不受限制的情况下处理软件的权限，包括但不限于使用、复制、修改、合并、发布、分发、再许可和/或销售软件副本的权利，以及允许获得软件的人这样做，但须符合以下条件：

上述版权声明和本许可声明应包含在软件的所有副本或实质部分中。

软件按“原样”提供，不提供任何明示或暗示的保证，包括但不限于适销性、特定用途适用性和非侵权性的保证。在任何情况下，作者或版权持有人均不对因软件或软件的使用或其他交易而产生或与之相关的任何索赔、损害赔偿或其他责任承担责任，无论是在合同、侵权或其他诉讼中。