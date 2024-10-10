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

## 1 “为什么使用F#”系列介绍

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/why-use-fsharp-intro/#series-toc)*)*

F# 的好处概述
01 Apr 2012 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/why-use-fsharp-intro/

本系列文章将为您介绍F#的主要功能，然后向您展示F#如何帮助您进行日常开发。

### F# 与 C# 相比的主要优势

如果你已经熟悉C#或Java，你可能会想知道为什么学习另一种语言是值得的。F#有一些主要优点，我将其分为以下主题：

- **简洁**。F#不会被花括号、分号等编码“噪音”所困扰。得益于强大的类型推理系统，您几乎不必指定对象的类型。解决同样的问题通常需要更少的代码行。
- **方便**。许多常见的编程任务在F#中要简单得多。这包括创建和使用复杂类型定义、执行列表处理、比较和相等、状态机等等。由于函数是一级对象，因此通过创建具有其他函数作为参数的函数，或者组合现有函数以创建新功能，可以很容易地创建功能强大且可重用的代码。
- **正确**。F#有一个非常强大的类型系统，可以防止许多常见错误，如空引用异常。此外，您通常可以使用类型系统本身对业务逻辑进行编码，因此实际上不可能编写错误的代码，因为它在编译时被捕获为类型错误。
- **并发性**。F#有许多内置工具和库，可以在一次发生多件事时帮助编程系统。异步编程是直接支持的，并行性也是如此。F#还有一个消息队列系统，对事件处理和响应式编程有很好的支持。由于数据结构在默认情况下是不可变的，因此共享状态和避免锁要容易得多。
- **完整性**。虽然F#本质上是一种函数式语言，但它确实支持其他并非100%纯的风格，这使得与网站、数据库、其他应用程序等非纯世界的交互变得更加容易。特别是，F#被设计为混合函数式/OO语言，因此它几乎可以做C#能做的一切。当然，F#与无缝集成。NET生态系统，让您可以访问所有第三方。NET库和工具。最后，它是Visual Studio的一部分，这意味着您可以获得一个具有IntelliSense支持的好编辑器、一个调试器和许多用于单元测试、源代码控制和其他开发任务的插件。

在本系列文章的其余部分中，我将尝试使用独立的F#代码片段（通常与C#代码进行比较）来演示F#的每一个好处。我将简要介绍F#的所有主要特性，包括模式匹配、函数组合和并发编程。当你完成这个系列时，我希望你会对F#的力量和优雅印象深刻，并鼓励你在下一个项目中使用它！

### 如何阅读和使用示例代码

这些帖子中的所有代码片段都被设计为交互式运行。我强烈建议你在阅读每篇文章时评估这些片段。任何大型代码文件的源代码都将从帖子链接到。

本系列不是教程，所以我不会过多地介绍代码的工作原理。如果你不能理解一些细节，不要担心；本系列的目的只是向您介绍F#，并激发您更深入地学习它的欲望。

如果你有C#和Java等语言的经验，你可能会发现，即使你不熟悉关键字或库，你也可以很好地理解用其他类似语言编写的源代码。你可能会问“如何分配变量？”或“如何进行循环？”，有了这些答案，你就可以很快地完成一些基本的编程。

这种方法不适用于 F#，因为在纯形式中没有变量、循环和对象。不要沮丧，这最终会有意义的！如果你想更深入地学习 F#，“学习F#”页面上有一些有用的提示。

## 2 F#语法60秒

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/fsharp-in-60-seconds/#series-toc)*)*

关于如何阅读 F# 代码的快速概述
02 Apr 2012 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/fsharp-in-60-seconds/

下面是一个关于如何为不熟悉语法的新手阅读F#代码的快速概述。

它显然不是很详细，但应该足够了，这样你就可以阅读并理解本系列即将到来的示例的要点。如果你不理解所有内容，不要担心，因为当我们看到实际的代码示例时，我会给出更详细的解释。

F#语法和标准类C语法之间的两个主要区别是：

- 大括号不用于分隔代码块。相反，使用缩进（Python 与此类似）。
- 空格用于分隔参数，而不是逗号。

有些人觉得F#语法令人反感。如果你是其中之一，请考虑这句话：

> “优化你的符号，让人们在看到它的前10分钟不会感到困惑，但从那以后就会阻碍可读性，这是一个非常糟糕的错误。”（David MacIver，通过一篇关于Scala语法的文章）。

就我个人而言，我认为当你习惯了 F# 语法时，它非常清晰明了。在很多方面，它比 C# 语法更简单，关键字和特殊情况更少。

下面的示例代码是一个简单的F#脚本，它演示了您经常需要的大多数概念。

我鼓励你以交互方式测试这段代码，并尝试一下！要么：

- 将其键入 F# 脚本文件（扩展名为 .fsx）并将其发送到交互式窗口。有关详细信息，请参阅“安装和使用F#”页面。
- 或者，尝试在交互式窗口中运行此代码。记住要总在最后使用 `;;` 告诉解释器你已经完成输入并准备进行评估。

```F#
// single line comments use a double slash
(* multi line comments use (* . . . *) pair

-end of multi line comment- *)

// ======== "Variables" (but not really) ==========
// The "let" keyword defines an (immutable) value
let myInt = 5
let myFloat = 3.14
let myString = "hello"	//note that no types needed

// ======== Lists ============
let twoToFive = [2;3;4;5]        // Square brackets create a list with
                                 // semicolon delimiters.
let oneToFive = 1 :: twoToFive   // :: creates list with new 1st element
// The result is [1;2;3;4;5]
let zeroToFive = [0;1] @ twoToFive   // @ concats two lists

// IMPORTANT: commas are never used as delimiters, only semicolons!

// ======== Functions ========
// The "let" keyword also defines a named function.
let square x = x * x          // Note that no parens are used.
square 3                      // Now run the function. Again, no parens.

let add x y = x + y           // don't use add (x,y)! It means something
                              // completely different.
add 2 3                       // Now run the function.

// to define a multiline function, just use indents. No semicolons needed.
let evens list =
   let isEven x = x%2 = 0     // Define "isEven" as an inner ("nested") function
   List.filter isEven list    // List.filter is a library function
                              // with two parameters: a boolean function
                              // and a list to work on

evens oneToFive               // Now run the function

// You can use parens to clarify precedence. In this example,
// do "map" first, with two args, then do "sum" on the result.
// Without the parens, "List.map" would be passed as an arg to List.sum
let sumOfSquaresTo100 =
   List.sum ( List.map square [1..100] )

// You can pipe the output of one operation to the next using "|>"
// Here is the same sumOfSquares function written using pipes
let sumOfSquaresTo100piped =
   [1..100] |> List.map square |> List.sum  // "square" was defined earlier

// you can define lambdas (anonymous functions) using the "fun" keyword
let sumOfSquaresTo100withFun =
   [1..100] |> List.map (fun x->x*x) |> List.sum

// In F# returns are implicit -- no "return" needed. A function always
// returns the value of the last expression used.

// ======== Pattern Matching ========
// Match..with.. is a supercharged case/switch statement.
let simplePatternMatch =
   let x = "a"
   match x with
    | "a" -> printfn "x is a"
    | "b" -> printfn "x is b"
    | _ -> printfn "x is something else"   // underscore matches anything

// Some(..) and None are roughly analogous to Nullable wrappers
let validValue = Some(99)
let invalidValue = None

// In this example, match..with matches the "Some" and the "None",
// and also unpacks the value in the "Some" at the same time.
let optionPatternMatch input =
   match input with
    | Some i -> printfn "input is an int=%d" i
    | None -> printfn "input is missing"

optionPatternMatch validValue
optionPatternMatch invalidValue

// ========= Complex Data Types =========

// Tuple types are pairs, triples, etc. Tuples use commas.
let twoTuple = 1,2
let threeTuple = "a",2,true

// Record types have named fields. Semicolons are separators.
type Person = {First:string; Last:string}
let person1 = {First="john"; Last="Doe"}

// Union types have choices. Vertical bars are separators.
type Temp =
  | DegreesC of float
  | DegreesF of float
let temp = DegreesF 98.6

// Types can be combined recursively in complex ways.
// E.g. here is a union type that contains a list of the same type:
type Employee =
  | Worker of Person
  | Manager of Employee list
let jdoe = {First="John";Last="Doe"}
let worker = Worker jdoe

// ========= Printing =========
// The printf/printfn functions are similar to the
// Console.Write/WriteLine functions in C#.
printfn "Printing an int %i, a float %f, a bool %b" 1 2.0 true
printfn "A string %s, and something generic %A" "hello" [1;2;3;4]

// all complex types have pretty printing built in
printfn "twoTuple=%A,\nPerson=%A,\nTemp=%A,\nEmployee=%A"
         twoTuple person1 temp worker

// There are also sprintf/sprintfn functions for formatting data
// into a string, similar to String.Format.
```

然后，让我们从比较一些简单的F#代码和等效的C#代码开始。

## 3 F#与C#的比较：一个简单的和

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/fvsc-sum-of-squares/#series-toc)*)*

其中，我们试图在不使用循环的情况下对1到N的平方进行求和
03 Apr 2012 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/fvsc-sum-of-squares/

为了了解一些真正的F#代码是什么样子的，让我们从一个简单的问题开始：“将1到N的平方和”。

我们将比较F#实现和C#实现。首先，F#代码：

```F#
// define the square function
let square x = x * x

// define the sumOfSquares function
let sumOfSquares n =
   [1..n] |> List.map square |> List.sum

// try it
sumOfSquares 100
```

神秘的外观 `|>` 被称为管道操作员。它只是将一个表达式的输出传输到下一个表达式。所以 `sumOfSquares` 的代码如下：

1. 创建一个1到n的列表（方括号构成一个列表）。
2. 将列表导入名为list.map的库函数，使用我们刚才定义的“square”函数将输入列表转换为输出列表。
3. 将得到的方块列表通过管道传输到名为 `List.sum` 的库函数中。你能猜到它是干什么的吗？
4. 没有明确的“return”语句。`List.sum` 的输出是函数的总体结果。

接下来，这是一个使用基于C语言的经典（非函数式）风格的C#实现。（稍后将讨论使用LINQ的功能更强大的版本。）

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

有什么区别？

- F# 代码更紧凑
- F# 代码没有任何类型声明
- F# 可以交互式开发

让我们逐一进行。

### 更少的代码

最明显的区别是 C# 代码要多得多。13 行 C# 代码，而 3 行 F# 代码（忽略注释）。C# 代码有很多“噪音”，比如花括号、分号等。在C#中，函数不能独立存在，需要添加到某个类（“SumOfSquaresHelper”）中。F# 使用空格而不是括号，不需要行终止符，函数可以独立存在。

在F#中，整个函数通常写在一行上，就像“square”函数一样。`sumOfSquares` 函数也可以写在一行都上。在 C#中，这通常被认为是不好的做法。

当一个函数确实有多行时，F# 使用缩进来表示一段代码，从而消除了对大括号的需要。（如果你曾经使用过Python，这是同样的想法）。所以 `sumOfSquares` 函数也可以这样编写：

```F#
let sumOfSquares n =
   [1..n]
   |> List.map square
   |> List.sum
```

唯一的缺点是你必须仔细缩进代码。就我个人而言，我认为这是值得的。

### 无类型声明

下一个区别是 C# 代码必须显式声明所使用的所有类型。例如，`int i` 参数和 `int SumOfSquares` 返回类型。是的，C# 确实允许您在许多地方使用“var”关键字，但不适用于函数的参数和返回类型。

在 F# 代码中，我们根本没有声明任何类型。这一点很重要：F# 看起来像一种非类型化语言，但实际上它和 C# 一样是类型安全的，事实上，甚至更是如此！F# 使用一种称为“类型推断”的技术，从上下文中推断出您正在使用的类型。它在大多数情况下工作得非常好，极大地降低了代码的复杂性。

在这种情况下，类型推理算法注意到我们从整数列表开始。这反过来意味着平方函数和求和函数也必须接受整数，并且最终值必须是整数。您可以通过查看交互式窗口中的编译结果来查看推断的类型。您将看到类似以下内容：

```F#
val square : int -> int
```

这意味着“square”函数接受一个int并返回一个int。

如果原始列表使用了浮点数，类型推理系统就会推断出平方函数使用了浮点。试试看：

```F#
// define the square function
let squareF x = x * x

// define the sumOfSquares function
let sumOfSquaresF n =
   [1.0 .. n] |> List.map squareF |> List.sum  // "1.0" is a float

sumOfSquaresF 100.0
```

类型检查非常严格！如果您在原始 `sumOfSquares` 示例中尝试使用浮点数列表（`[1.0..n]`），或在 `sumOfSquaresF` 示例中使用整数列表（`[1..n]`），你会从编译器获得一个类型错误。

### 互动式开发

最后，F# 有一个交互式窗口，您可以在其中立即测试代码并使用它。在 C# 中，没有简单的方法可以做到这一点。

例如，我可以编写我的平方函数并立即对其进行测试：

```F#
// define the square function
let square x = x * x

// test
let s2 = square 2
let s3 = square 3
let s4 = square 4
```

当我确信它有效时，我可以继续下一段代码。

这种交互性鼓励了一种渐进式的编码方法，这种方法可能会让人上瘾！

此外，许多人声称，交互式设计代码会强制执行良好的设计实践，如解耦和显式依赖关系，因此，适合交互式评估的代码也将是易于测试的代码。相反，不能交互式测试的代码可能也很难测试。

### 重新审视 C# 代码

我最初的示例是使用“旧式”C# 编写的。C# 包含了许多功能特性，可以使用 LINQ 扩展以更紧凑的方式重写示例。

这是另一个 C# 版本——F# 代码的逐行翻译。

```c#
public static class FunctionalSumOfSquaresHelper
{
   public static int SumOfSquares(int n)
   {
      return Enumerable.Range(1, n)
         .Select(i => i * i)
         .Sum();
   }
}
```

然而，除了花括号、句点和分号的噪音外，C# 版本还需要声明参数和返回类型，这与 F# 版本不同。

许多 C# 开发人员可能会发现这是一个微不足道的例子，但当逻辑变得更加复杂时，他们仍然会求助于循环。然而，在 F# 中，你几乎永远不会看到这样的显式循环。例如，请参阅这篇关于从更复杂的循环中消除样板的文章。

## 4 F#与C#的比较：排序

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/fvsc-quicksort/#series-toc)*)*

其中我们看到F#比C#更具声明性，我们介绍了模式匹配。
04 Apr 2012 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/fvsc-quicksort/

在下一个例子中，我们将实现一个类似快速排序的算法来对列表进行排序，并将F#实现与C#实现进行比较。

以下是一个简化的快速排序算法的逻辑：

```
如果列表为空，则无需执行任何操作。
否则：
    1.取列表的第一个元素
    2.查找列表其余部分中的小于第一个元素的所有元素，并对其进行排序。
    3.查找列表其余部分中的大于等于第一个元素的所有元素，并对其进行排序
    4.将三个部分结合在一起，得到最终结果：
    （排序较小的元素+firstElement+排序较大的元素）
```

请注意，这是一个简化的算法，没有经过优化（也没有像真正的快速排序那样就地排序）；我们现在想专注于清晰度。

以下是F#中的代码：

```F#
let rec quicksort list =
   match list with
   | [] ->                            // If the list is empty
        []                            // return an empty list
   | firstElem::otherElements ->      // If the list is not empty
        let smallerElements =         // extract the smaller ones
            otherElements
            |> List.filter (fun e -> e < firstElem)
            |> quicksort              // and sort them
        let largerElements =          // extract the large ones
            otherElements
            |> List.filter (fun e -> e >= firstElem)
            |> quicksort              // and sort them
        // Combine the 3 parts into a new list and return it
        List.concat [smallerElements; [firstElem]; largerElements]

//test
printfn "%A" (quicksort [1;5;23;18;9;1;3])
```

再次注意，这不是一个优化的实现，而是为了紧密地反映算法而设计的。

让我们来看看这段代码：

- 任何地方都没有类型声明。此函数适用于任何具有可比项的列表（几乎所有 F# 类型，因为它们自动具有默认比较函数）。
- 整个函数是递归的——这是使用“`let rec quicksort list=`”中的 `rec` 关键字向编译器发出的信号。
- `match..with` 有点像 switch/case 语句。每个要测试的分支都用一个垂直条发出信号，如下所示：

```F#
match x with
| caseA -> something
| caseB -> somethingElse
```

- 带有 `[]` 的 “`match`” 匹配空列表，并返回空列表。
- 带有 `firstElem::otherElements` 的“`match`”有两个作用。
  - 首先，它只匹配非空列表。
  - 其次，它会自动创建两个新值。一个用于第一个元素，称为“`firstElem`”，另一个用于列表的其余部分，称为 `otherElements`。用 C# 的术语来说，这就像有一个“switch”语句，它不仅分支，而且同时进行变量声明和赋值。
- `->` 有点像 C# 中的 lambda（`=>`）。等效的 C# lambda 看起来像 `(firstElem，otherElements) => do something`。
- “`smallerElements`”部分获取列表的其余部分，使用带有“`<`”运算符的内联 lambda 表达式对第一个元素进行过滤，然后将结果递归地传输到快速排序函数中。
- “`largerElements`”行执行相同的操作，除了使用“`>=`”运算符
- 最后，使用列表连接函数“`List.concat`”构建结果列表。为此，需要将第一个元素放入列表中，这就是方括号的作用。
- 再次注意，没有“return”关键字；将返回最后一个值。在“`[]`”分支中，返回值是空列表，在main分支中，它是新构造的列表。

这里有一个老式的C#实现（不使用LINQ）进行比较。

```c#
public class QuickSortHelper
{
   public static List<T> QuickSort<T>(List<T> values)
      where T : IComparable
   {
      if (values.Count == 0)
      {
         return new List<T>();
      }

      //get the first element
      T firstElement = values[0];

      //get the smaller and larger elements
      var smallerElements = new List<T>();
      var largerElements = new List<T>();
      for (int i = 1; i < values.Count; i++)  // i starts at 1
      {                                       // not 0!
         var elem = values[i];
         if (elem.CompareTo(firstElement) < 0)
         {
            smallerElements.Add(elem);
         }
         else
         {
            largerElements.Add(elem);
         }
      }

      //return the result
      var result = new List<T>();
      result.AddRange(QuickSort(smallerElements.ToList()));
      result.Add(firstElement);
      result.AddRange(QuickSort(largerElements.ToList()));
      return result;
   }
}
```

比较这两组代码，我们可以再次看到F#代码更紧凑，噪音更少，不需要类型声明。

此外，F#代码的读取几乎与实际算法完全相同，与C#代码不同。这是F#的另一个关键优势——与C#相比，代码通常更具声明性（“做什么”），命令性（“如何做”）更低，因此更具自文档性。

### C# 中的函数式实现

下面是一个使用LINQ和扩展方法的更现代的“函数式”实现：

```c#
public static class QuickSortExtension
{
    /// <summary>
    /// Implement as an extension method for IEnumerable
    /// </summary>
    public static IEnumerable<T> QuickSort<T>(
        this IEnumerable<T> values) where T : IComparable
    {
        if (values == null || !values.Any())
        {
            return new List<T>();
        }

        //split the list into the first element and the rest
        var firstElement = values.First();
        var rest = values.Skip(1);

        //get the smaller and larger elements
        var smallerElements = rest
                .Where(i => i.CompareTo(firstElement) < 0)
                .QuickSort();

        var largerElements = rest
                .Where(i => i.CompareTo(firstElement) >= 0)
                .QuickSort();

        //return the result
        return smallerElements
            .Concat(new List<T>{firstElement})
            .Concat(largerElements);
    }
}
```

这要干净得多，读起来几乎与F#版本相同。但不幸的是，无法避免函数签名中的额外噪声。

### 正确性

最后，这种紧凑性的一个有益的副作用是 F# 代码通常在第一次工作时就可以了，而 C# 代码可能需要更多的调试。

事实上，在对这些示例进行编码时，旧式的 C# 代码最初是不正确的，需要一些调试才能使其正确。特别棘手的领域是 `for` 循环（从 1 开始，不是零）和 `CompareTo` 比较（我绕错了方向），而且很容易意外修改入站列表。第二个 C# 示例中的函数式风格不仅更简洁，而且更容易正确编码。

但与 F# 版本相比，即使是函数式 C# 版本也有缺点。例如，由于 F# 使用模式匹配，因此无法使用空列表分支到“非空列表”情况。另一方面，在 C# 代码中，如果我们忘记了测试：

```F#
if (values == null || !values.Any()) ...
```

然后提取第一元素：

```c#
var firstElement = values.First();
```

将失败，但有一个例外。编译器无法为您强制执行此操作。在您自己的代码中，您使用 `FirstOrDefault` 而不是 `First` 的频率是多少，因为您正在编写“防御性”代码。下面是一个在C#中很常见但在F#中很少见的代码模式示例：

```c#
var item = values.FirstOrDefault();  // instead of .First()
if (item != null)
{
   // do something if item is valid
}
```

F#中的一步式“模式匹配和分支”允许您在许多情况下避免这种情况。

### 后记

按照 F# 标准，上面 F# 中的示例实现实际上非常冗长！

为了好玩，以下是一个更典型的简洁版本：

```F#
let rec quicksort2 = function
   | [] -> []
   | first::rest ->
        let smaller,larger = List.partition ((>=) first) rest
        List.concat [quicksort2 smaller; [first]; quicksort2 larger]

// test code
printfn "%A" (quicksort2 [1;5;23;18;9;1;3])
```

对于4行代码来说还不错，当你习惯了语法后，仍然非常可读。

## 5 F#与C#的比较：下载网页

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/fvsc-download/#series-toc)*)*

其中我们看到F#擅长回调，我们被介绍给了“use”关键字
05 Apr 2012 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/fvsc-download/

在这个例子中，我们将比较下载网页的F#和C#代码，以及处理文本流的回调。

我们将从一个简单的F#实现开始。

```F#
// "open" brings a .NET namespace into visibility
open System.Net
open System
open System.IO

// Fetch the contents of a web page
let fetchUrl callback url =
    let req = WebRequest.Create(Uri(url))
    use resp = req.GetResponse()
    use stream = resp.GetResponseStream()
    use reader = new IO.StreamReader(stream)
    callback reader url
```

让我们来看看这段代码：

- 在顶部使用“open”允许我们编写“WebRequest”而不是“System.Net.Webequest”。它类似于 C# 中的“`using System.Net`”标头。
- 接下来，我们定义 `fetchUrl` 函数，它接受两个参数，一个用于处理流的回调和一个用于获取的 url。
- 接下来，我们将 url 字符串包装在 Uri中。F# 具有严格的类型检查，因此如果我们编写了：`let req=WebRequest.Create(url)` 编译器会抱怨它不知道 `WebRequest.Create` 的版本以供使用。
- 在声明 `response`、`stream` 和 `reader` 值时，使用“`use`”关键字而不是“`let`”。这只能与实现 `IDisposable` 的类结合使用。它告诉编译器在资源超出范围时自动处理它。这相当于 C# 的“`using`”关键字。
- 最后一行使用 StreamReader 和 url 作为参数调用回调函数。请注意，回调的类型不必在任何地方指定。

下面是等效的 C# 实现。

```c#
class WebPageDownloader
{
    public TResult FetchUrl<TResult>(
        string url,
        Func<string, StreamReader, TResult> callback)
    {
        var req = WebRequest.Create(url);
        using (var resp = req.GetResponse())
        {
            using (var stream = resp.GetResponseStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    return callback(url, reader);
                }
            }
        }
    }
}
```

像往常一样，C#版本有更多的“噪音”。

- 花括号有10行，嵌套有5个层次的视觉复杂性*
- 所有参数类型都必须显式声明，泛型 `TResult` 类型必须重复三次。

*确实，在这个特定的例子中，当所有 `using` 语句都相邻时，可以删除额外的大括号和缩进，但在更一般的情况下，它们是必需的。

### 测试代码

回到F#世界，我们现在可以交互式地测试代码：

```F#
let myCallback (reader:IO.StreamReader) url =
    let html = reader.ReadToEnd()
    let html1000 = html.Substring(0,1000)
    printfn "Downloaded %s. First 1000 is %s" url html1000
    html      // return all the html

//test
let google = fetchUrl myCallback "http://google.com"
```

最后，我们必须为 reader 参数（`reader:IO.StreamReader`）使用类型声明。这是必需的，因为 F# 编译器无法自动确定“reader”参数的类型。

F# 的一个非常有用的特性是，你可以在函数中“烘焙”参数，这样就不必每次都传入它们。这就是为什么 `url` 参数放在最后而不是第一个的原因，就像 C# 版本一样。回调可以设置一次，而 url 因调用而异。

```F#
// build a function with the callback "baked in"
let fetchUrl2 = fetchUrl myCallback

// test
let google = fetchUrl2 "http://www.google.com"
let bbc    = fetchUrl2 "http://news.bbc.co.uk"

// test with a list of sites
let sites = ["http://www.bing.com";
             "http://www.google.com";
             "http://www.yahoo.com"]

// process each site in the list
sites |> List.map fetchUrl2
```

最后一行（使用 `List.map`）显示了如何将新函数与列表处理函数结合使用，以便一次下载整个列表。

以下是等效的 C# 测试代码：

```c#
[Test]
public void TestFetchUrlWithCallback()
{
    Func<string, StreamReader, string> myCallback = (url, reader) =>
    {
        var html = reader.ReadToEnd();
        var html1000 = html.Substring(0, 1000);
        Console.WriteLine(
            "Downloaded {0}. First 1000 is {1}", url,
            html1000);
        return html;
    };

    var downloader = new WebPageDownloader();
    var google = downloader.FetchUrl("http://www.google.com",
                                      myCallback);

    // test with a list of sites
    var sites = new List<string> {
        "http://www.bing.com",
        "http://www.google.com",
        "http://www.yahoo.com"};

    // process each site in the list
    sites.ForEach(site => downloader.FetchUrl(site, myCallback));
}
```

同样，代码比 F# 代码有点嘈杂，有很多显式类型引用。更重要的是，C# 代码不容易让你在函数中烘焙一些参数，所以每次都必须显式引用回调。



## 6 四个关键概念

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/key-concepts/#series-toc)*)*

区分F#与标准命令式语言的概念
06 Apr 2012 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/key-concepts/

在接下来的几篇文章中，我们将继续演示本系列的主题：简洁性、便利性、正确性、并发性和完整性。

但在此之前，让我们来看看 F# 中的一些关键概念，我们将一次又一次地遇到这些概念。F# 在很多方面与 C# 等标准命令式语言不同，但有一些主要的区别尤其需要理解：

- **面向函数**而非面向对象
- **表达式**而非语句（statements）
- 用于创建域模型的**代数类型**
- 控制流的**模式匹配**

在以后的文章中，我们将更深入地讨论这些问题——这只是一个品酒师，可以帮助你理解本系列的其余部分。

四个关键概念

### 面向函数而非面向对象

正如您对“函数式编程”一词的期望，函数在F#中无处不在。

当然，函数是第一类实体，可以像任何其他值一样传递：

```F#
let square x = x * x

// functions as values
let squareclone = square
let result = [1..10] |> List.map squareclone

// functions taking other functions as parameters
let execFunction aFunc aParam = aFunc aParam
let result2 = execFunction square 12
```

但是C#也有一级函数，那么函数式编程有什么特别之处呢？

简而言之，F# 的面向函数特性渗透到语言和类型系统的每一个部分，这是 C# 所没有的，因此 C# 中笨重或笨拙的东西在 F# 中非常优雅。

很难用几段话来解释这一点，但以下是我们将在这一系列帖子中看到的一些好处：

- **通过组合构建**。组合是一种“粘合剂”，它使我们能够从较小的系统构建更大的系统。这不是一种可选技术，而是功能风格的核心。几乎每一行代码都是一个可组合的表达式（见下文）。组合用于构建基本函数，然后构建使用这些函数的函数，以此类推。组合原理不仅适用于函数，也适用于类型（下文讨论的乘积和求和类型）。
- **分解和重构**。将问题分解为各个部分的能力取决于这些部分粘合在一起的难易程度。在命令式语言中看似不可分割的方法和类，在函数式设计中往往可以分解成令人惊讶的小块。这些细粒度组件通常由（a）一些非常通用的函数组成，这些函数将其他函数作为参数，以及（b）其他辅助函数，这些辅助函数专门用于特定数据结构或应用程序的一般情况。一旦被分解，广义函数允许非常容易地编程许多额外的操作，而无需编写新代码。在关于从循环中提取重复代码的文章中，您可以看到这样一个通用函数（fold 函数）的一个很好的例子。
- **好设计**。许多良好设计的原则，如“关注点分离”、“单一责任原则”、“程序到接口，而不是实现”，都是函数式方法的自然结果。函数式代码通常具有高级和声明性。

本系列的以下文章将提供函数如何使代码更简洁方便的示例，然后为了更深入地理解，有一个关于函数思维的完整系列。

### 表达式而非语句

在函数式语言中，没有语句，只有表达式。也就是说，每个代码块总是返回一个值，较大的代码块是通过使用组合而不是序列化语句列表组合较小的代码块而创建的。

如果你使用过 LINQ 或 SQL，那么你已经熟悉了基于表达式的语言。例如，在纯 SQL 中，不能有赋值。相反，您必须在较大的查询中包含子查询。

```sql
SELECT EmployeeName
FROM Employees
WHERE EmployeeID IN
	(SELECT DISTINCT ManagerID FROM Employees)  -- subquery
```

F# 的工作方式是一样的——每个函数定义都是一个表达式，而不是一组语句。

这可能并不明显，但从表达式构建的代码比使用语句更安全、更简洁。为了了解这一点，让我们将 C# 中一些基于语句的代码与等效的基于表达式的代码进行比较。

首先，基于语句的代码。语句不返回值，因此您必须使用从语句体中分配的临时变量。

```c#
// statement-based code in C#
int result;
if (aBool)
{
  result = 42;
}
Console.WriteLine("result={0}", result);
```

因为 `if-then` 块是一个语句，所以 `result` 变量必须在语句外部定义，但从语句内部分配，这会导致以下问题：

- `result` 应该设置为什么初始值？
- 如果我忘记给 `result` 变量赋值怎么办？
- 在“else”情况下，`result` 变量的值是什么？

为了进行比较，这是以面向表达式的风格重写的相同代码：

```F#
// expression-based code in C#
int result = (aBool) ? 42 : 0;
Console.WriteLine("result={0}", result);
```

在面向表达式的版本中，这些问题都不适用：

- `result` 变量在赋值的同时被声明。不必在表达式“外部”设置任何变量，也不必担心它们应该设置为什么初始值。
- “else”被明确处理。不可能忘记在其中一个分支机构做作业。
- 不可能忘记赋值 `result`，因为这样变量就不存在了！

在 F# 中，面向表达式的风格不是一种选择，当来自命令式背景时，这是需要改变方法的事情之一。

### 代数类型

F# 中的类型系统基于**代数类型**的概念。也就是说，通过以两种不同的方式组合现有类型来构建新的复合类型：

- 首先，值的组合，每个值都是从一组类型中挑选出来的。这些被称为“乘积”类型。
- 或者，作为一个不相交的联合，代表一组类型之间的选择。这些被称为“求和”类型。

例如，给定现有类型 `int` 和 `bool`，我们可以创建一个新的产品类型，该类型必须各有一个：

```F#
//declare it
type IntAndBool = {intPart: int; boolPart: bool}

//use it
let x = {intPart=1; boolPart=false}
```

或者，我们可以创建一个新的并集/求和类型，可以在每种类型之间进行选择：

```F#
//declare it
type IntOrBool =
  | IntChoice of int
  | BoolChoice of bool

//use it
let y = IntChoice 42
let z = BoolChoice true
```

这些“选择”类型在 C# 中不可用，但对于建模许多现实世界的情况非常有用，例如状态机中的状态（这在许多领域是一个令人惊讶的常见主题）。

通过以这种方式组合“乘积”和“总和”类型，可以很容易地创建一组丰富的类型，准确地对任何业务领域进行建模。有关此操作的示例，请参阅有关低开销类型定义和使用类型系统确保正确代码的帖子。

### 控制流的模式匹配

大多数命令式语言都为分支和循环提供了各种控制流语句：

- `if-then-else`（以及三元版本 `bool ? if-true : if-false`）
- `case` 或 `switch` 语句
- `for` 和 `foreach` 循环，带有 `break` 和 `continue`
- `while` 和 `until` 循环
- 甚至可怕的 `goto`

F# 确实支持其中一些，但 F# 也支持最通用的条件表达式形式，即**模式匹配**。

替换 `if-then-else` 的典型匹配表达式如下：

```F#
match booleanExpression with
| true -> // true branch
| false -> // false branch
```

`switch` 的替换可能看起来像这样：

```F#
match aDigit with
| 1 -> // Case when digit=1
| 2 -> // Case when digit=2
| _ -> // Case otherwise
```

最后，循环通常使用递归完成，通常看起来像这样：

```F#
match aList with
| [] ->
     // Empty case
| first::rest ->
     // 至少有一个元素的案例。
     // 处理第一个元素，然后调用
     // 与列表的其余部分递归
```

虽然匹配表达式一开始看起来不必要地复杂，但在实践中，你会发现它既优雅又强大。

有关模式匹配的好处，请参阅关于穷举模式匹配的文章，有关大量使用模式匹配的工作示例，请参阅罗马数字示例。

### 与联合类型的模式匹配

我们上面提到过F#支持“union”或“choice”类型。这是用来代替继承来处理底层类型的不同变体的。模式匹配与这些类型无缝协作，为每个选择创建控制流。

在下面的示例中，我们创建了一个表示四种不同形状的 `Shape` 类型，然后为每种形状定义了一个具有不同行为的 `draw` 函数。这类似于面向对象语言中的多态性，但基于函数。

```F#
type Shape =        // define a "union" of alternative structures
    | Circle of radius:int
    | Rectangle of height:int * width:int
    | Point of x:int * y:int
    | Polygon of pointList:(int * int) list

let draw shape =    // define a function "draw" with a shape param
  match shape with
  | Circle radius ->
      printfn "The circle has a radius of %d" radius
  | Rectangle (height,width) ->
      printfn "The rectangle is %d high by %d wide" height width
  | Polygon points ->
      printfn "The polygon is made of these points %A" points
  | _ -> printfn "I don't recognize this shape"

let circle = Circle(10)
let rect = Rectangle(4,5)
let point = Point(2,3)
let polygon = Polygon( [(1,1); (2,2); (3,3)])

[circle; rect; polygon; point] |> List.iter draw
```

有几件事需要注意：

- 像往常一样，我们不必指定任何类型。编译器正确地确定了“draw”函数的形状参数的类型为 `Shape`。
- `Polygon` 案例定义中的 `int*int` 是一个元组，一对 int。如果你想知道为什么类型是“乘法”的，请参阅这篇关于元组的文章。
- 你可以看到这 `match..with` 逻辑不仅与形状的内部结构相匹配，而且根据适合形状的内容分配值。
- 下划线类似于 switch 语句中的“默认”分支，除了在 F# 中是必需的——必须始终处理所有可能的情况。如果你注释掉这句话

```F#
 | _ -> printfn "I don't recognize this shape"
```

看看编译时会发生什么！

在 C# 中，可以通过使用子类或接口来模拟这些类型的选择，但 C# 类型系统中没有内置对这种穷举匹配和错误检查的支持。

### 行为导向设计与数据导向设计

你可能想知道这种模式匹配是不是一个好主意？在面向对象的设计中，检查特定类是一种反模式，因为你应该只关心行为，而不是实现它的类。

但在纯功能设计中，没有对象也没有行为。有函数，也有“哑（dumb）”的数据类型。数据类型没有任何与之相关的行为，函数也不包含数据——它们只是将数据类型转换为其他数据类型。

在这种情况下，`Circle` 和 `Rectangle` 实际上不是类型。唯一的类型是 `Shape`——一种选择，一种可区分联合——这些都是这种类型的各种情况。（更多关于可区分联合在这里）。

为了使用 `Shape` 类型，函数需要处理 `Shape` 的每种情况，这是通过模式匹配完成的。



## 7 简洁

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/conciseness-intro/#series-toc)*)*

为什么简洁很重要？
07 Apr 2012 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/conciseness-intro/

在看过一些简单的代码后，我们现在将继续演示主要主题（简洁性、便利性、正确性、并发性和完整性），并通过类型、函数和模式匹配的概念进行筛选。

在接下来的几篇文章中，我们将研究 F# 的简洁性和可读性。

大多数主流编程语言的一个重要目标是可读性和简洁性的良好平衡。过多的简洁可能会导致代码难以理解或混淆（APL 有人吗？），而过多的冗长则很容易淹没其潜在含义。理想情况下，我们希望有一个高信噪比，代码中的每个单词和字符都有助于代码的含义，并且样板最少。

为什么简洁很重要？以下是几个原因：

- **简洁的语言往往更具声明性**，说明代码应该做什么，而不是如何做。也就是说，声明性代码更关注高级逻辑，而不是实现的具体细节。
- 如果要推理的代码行数更少，就**更容易推理正确性**！
- 当然，**您一次可以在屏幕上看到更多代码**。这可能看起来微不足道，但你看到的越多，你掌握的也就越多。

如您所见，与C#相比，F#通常更简洁。这是由于以下功能：

- **类型推断**和**低开销类型定义**。F# 简洁易读的主要原因之一是它的类型系统。F# 使得根据需要创建新类型变得非常容易。它们在定义或使用中都不会造成视觉混乱，类型推理系统意味着您可以自由使用它们，而不会被复杂的类型语法分心。
- **使用函数提取样板代码**。DRY 原则（“不要重复自己”）是函数式语言和面向对象语言中良好设计的核心原则。在F#中，将重复代码提取到常见的实用函数中非常容易，这使您可以专注于重要的事情。
- **从简单函数编写复杂代码**并**创建迷你语言**。函数式方法使创建一组基本操作变得容易，然后以各种方式组合这些构建块以构建更复杂的行为。这样，即使是最复杂的代码也仍然非常简洁易读。
- **图案匹配**。我们已经将模式匹配视为一种美化的开关语句，但事实上它更通用，因为它可以以多种方式比较表达式，在值、条件和类型上进行匹配，然后同时分配或提取值。

## 8 类型推断

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/conciseness-type-inference/#series-toc)*)*

如何避免被复杂的类型语法分心
08 Apr 2012 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/conciseness-type-inference/

正如您已经看到的，F# 使用了一种称为“类型推理”的技术，大大减少了需要在正常代码中显式指定的类型注释的数量。即使确实需要指定类型，与 C# 相比，语法也不那么冗长。

为了理解这一点，这里有一些 C# 方法，它们封装了两个标准的 LINQ 函数。实现很简单，但方法签名非常复杂：

```c#
public IEnumerable<TSource> Where<TSource>(
    IEnumerable<TSource> source,
    Func<TSource, bool> predicate
    )
{
    //use the standard LINQ implementation
    return source.Where(predicate);
}

public IEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(
    IEnumerable<TSource> source,
    Func<TSource, TKey> keySelector
    )
{
    //use the standard LINQ implementation
    return source.GroupBy(keySelector);
}
```

这里是 F# 的确切等价物，表明根本不需要类型注释！

```F#
let Where source predicate =
    //use the standard F# implementation
    Seq.filter predicate source

let GroupBy source keySelector =
    //use the standard F# implementation
    Seq.groupBy keySelector source
```

> 您可能会注意到，“filter”和“groupBy”的标准 F# 实现的参数顺序与 C# 中使用的 LINQ 实现完全相反。“source”参数放在最后，而不是第一个。这是有原因的，这将在函数式思维系列中解释。

类型推理算法在从多个来源收集信息以确定类型方面表现出色。在下面的示例中，它正确地推断出 `list` 值是字符串列表。

```F#
let i = 1
let s = "hello"
let tuple = s,i       // pack into tuple
let s2,i2 = tuple     // unpack
let list = [s2]       // type is string list
```

在这个例子中，它正确地推断出 `sumLengths` 函数接受一个字符串列表并返回一个 int。

```F#
let sumLengths strList =
    strList |> List.map String.length |> List.sum

// function type is: string list -> int
```

## 9 低开销类型定义

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/conciseness-type-definitions/#series-toc)*)*

制造新类型不受处罚
09 Apr 2012 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/conciseness-type-definitions/

在 C# 中，创建新类型有一个抑制因素——缺乏类型推理意味着你需要在大多数地方显式指定类型，从而导致脆弱性和更多的视觉混乱。因此，人们总是倾向于创建巨大单个（monolithic）类，而不是将其模块化。

在 F# 中，创建新类型没有惩罚，因此拥有数百甚至数千个新类型是很常见的。每次需要定义结构时，都可以创建一个特殊类型，而不是重用（和重载）现有类型，如字符串和列表。

这意味着你的程序将更加类型安全、更加自文档化、更易于维护（因为当类型发生变化时，你会立即得到编译时错误，而不是运行时错误）。

以下是 F# 中单行类型的一些示例：

```F#
open System

// some "record" types
type Person = {FirstName:string; LastName:string; Dob:DateTime}
type Coord = {Lat:float; Long:float}

// some "union" (choice) types
type TimePeriod = Hour | Day | Week | Year
type Temperature = C of int | F of int
type Appointment =
  OneTime of DateTime | Recurring of DateTime list
```

### F#类型和领域驱动设计

F#中类型系统的简洁性在进行域驱动设计（DDD）时特别有用。在 DDD 中，对于每个真实世界的实体和值对象，理想情况下都希望有一个相应的类型。这可能意味着创建数百个“小”类型，这在 C# 中可能很乏味。

此外，DDD 中的“值”对象应该具有结构相等性，这意味着包含相同数据的两个对象应该始终相等。在 C# 中，这可能意味着重写 `IEquatable<T>` 会更加乏味，但在 F# 中，默认情况下你可以免费获得它。

为了展示在 F# 中创建 DDD 类型有多容易，这里有一些可能为简单的“客户”域创建的示例类型。

```F#
type PersonalName =
  {FirstName:string; LastName:string}

// Addresses
type StreetAddress = {
  Line1:string;
  Line2:string;
  Line3:string
  }

type ZipCode = ZipCode of string
type StateAbbrev = StateAbbrev of string
type ZipAndState =
  {State:StateAbbrev; Zip:ZipCode }
type USAddress =
  {Street:StreetAddress; Region:ZipAndState}

type UKPostCode = PostCode of string
type UKAddress =
  {Street:StreetAddress; Region:UKPostCode}

type InternationalAddress = {
  Street:StreetAddress;
  Region:string;
  CountryName:string
  }

// choice type -- must be one of these three specific types
type Address =
  | USAddress of USAddress
  | UKAddress of UKAddress
  | InternationalAddress of InternationalAddress

// Email
type Email = Email of string

// Phone
type CountryPrefix = Prefix of int
type Phone =
  {CountryPrefix:CountryPrefix; LocalNumber:string}

type Contact =
  {
  PersonalName: PersonalName;
  // "option" means it might be missing
  Address: Address option;
  Email: Email option;
  Phone: Phone option;
  }

// Put it all together into a CustomerAccount type
type CustomerAccountId = AccountId of string
type CustomerType  = Prospect | Active | Inactive

// override equality and deny comparison
[<CustomEquality; NoComparison>]
type CustomerAccount =
  {
  CustomerAccountId: CustomerAccountId;
  CustomerType: CustomerType;
  ContactInfo: Contact;
  }

  override this.Equals(other) =
    match other with
    | :? CustomerAccount as otherCust ->
      (this.CustomerAccountId = otherCust.CustomerAccountId)
    | _ -> false

  override this.GetHashCode() = hash this.CustomerAccountId
```

这个代码片段在短短几行中包含了 17 个类型定义，但复杂性很低。做同样的事情需要多少行 C# 代码？

显然，这是一个仅包含基本类型的简化版本——在真实系统中，会添加约束和其他方法。但请注意，创建大量DDD值对象是多么容易，尤其是字符串的包装类型，如“`ZipCode`”和“`Email`”。通过使用这些包装器类型，我们可以在创建时强制执行某些约束，并确保这些类型不会与正常代码中的无约束字符串混淆。唯一的“实体”类型是 `CustomerAccount`，它被明确表示为具有平等和比较的特殊待遇。

有关更深入的讨论，请参阅名为“F#中的领域驱动设计”的系列文章。

## 10 使用函数提取样板代码

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/conciseness-extracting-boilerplate/#series-toc)*)*

DRY 原理的功能方法
2012年4月10日 这篇文章已有3年多的历史了

https://fsharpforfunandprofit.com/posts/conciseness-extracting-boilerplate/

在本系列的第一个示例中，我们看到了一个计算平方和的简单函数，该函数在F#和C#中都实现了。现在，假设我们想要一些类似的新函数，例如：

- 计算N以下所有数字的乘积
- 将奇数之和计数到N
- 数字的交替和，直到N

显然，所有这些要求都是相似的，但您将如何提取任何共同的功能？

让我们先从C#中的一些简单实现开始：

```c#
public static int Product(int n)
{
    int product = 1;
    for (int i = 1; i <= n; i++)
    {
        product *= i;
    }
    return product;
}

public static int SumOfOdds(int n)
{
    int sum = 0;
    for (int i = 1; i <= n; i++)
    {
        if (i % 2 != 0) { sum += i; }
    }
    return sum;
}

public static int AlternatingSum(int n)
{
    int sum = 0;
    bool isNeg = true;
    for (int i = 1; i <= n; i++)
    {
        if (isNeg)
        {
            sum -= i;
            isNeg = false;
        }
        else
        {
            sum += i;
            isNeg = true;
        }
    }
    return sum;
}
```

所有这些实现有什么共同点？循环逻辑！作为程序员，我们被告知要记住DRY原则（“不要重复自己”），但在这里，我们每次都重复了几乎完全相同的循环逻辑。让我们看看能否提取出这三种方法之间的差异：

| 函数           | 初始值                        | 循环内逻辑                                                   |
| :------------- | :---------------------------- | :----------------------------------------------------------- |
| Product        | product=1                     | 将第i个值乘以运行总数                                        |
| SumOfOdds      | sum=0                         | 如果不是偶数，则将第i个值添加到运行总数中                    |
| AlternatingSum | int sum = 0 bool isNeg = true | 使用 isNeg 标志来决定是加还是减，并在下一次传递时翻转该标志。 |

有没有办法去掉重复的代码，只关注设置和内部循环逻辑？是的，有。以下是F#中相同的三个函数：

```F#
let product n =
    let initialValue = 1
    let action productSoFar x = productSoFar * x
    [1..n] |> List.fold action initialValue

//test
product 10

let sumOfOdds n =
    let initialValue = 0
    let action sumSoFar x = if x%2=0 then sumSoFar else sumSoFar+x
    [1..n] |> List.fold action initialValue

//test
sumOfOdds 10

let alternatingSum n =
    let initialValue = (true,0)
    let action (isNeg,sumSoFar) x = if isNeg then (false,sumSoFar-x)
                                             else (true ,sumSoFar+x)
    [1..n] |> List.fold action initialValue |> snd

//test
alternatingSum 100
```

这三个函数具有相同的模式：

- 设置初始值
- 设置一个动作函数，该函数将在循环内的每个元素上执行。
- 调用库函数 `List.fold`。这是一个功能强大的通用函数，它从初始值开始，然后依次为列表中的每个元素运行动作函数。

action 函数总是有两个参数：一个运行的 total（或state）和要操作的 list 元素（在上面的示例中称为“x”）。

在最后一个函数 `alternatingSum` 中，您会注意到它使用了一个元组（一对值）作为初始值和操作结果。这是因为运行总计和 `isNeg` 标志都必须传递给循环的下一次迭代——没有可以使用的“全局”值。折叠的最终结果也是一个元组，因此我们必须使用“snd”（second）函数来提取我们想要的最终总和。

通过使用 `List.fold` 并完全避免任何循环逻辑，F# 代码获得了许多好处：

- **强调并明确了关键程序逻辑**。功能之间的重要差异变得非常明显，而共性则被推到了后台。
- **样板循环代码已被消除**，因此代码比 C# 版本更简洁（4-5 行 F# 代码比至少 9 行 C# 代码）
- **循环逻辑中永远不会有错误**（例如超出一个（off-by-one）），因为该逻辑没有暴露给我们。

顺便说一句，平方和的例子也可以用 `fold` 来写：

```F#
let sumOfSquaresWithFold n =
    let initialValue = 0
    let action sumSoFar x = sumSoFar + (x*x)
    [1..n] |> List.fold action initialValue

//test
sumOfSquaresWithFold 100
```

### C# 中的“折叠”

你能在 C# 中使用“折叠”方法吗？对。LINQ 确实有一个相当于 `fold` 的东西，称为 `Aggregate`。以下是为使用它而重写的 C# 代码：

```c#
public static int ProductWithAggregate(int n)
{
    var initialValue = 1;
    Func<int, int, int> action = (productSoFar, x) =>
        productSoFar * x;
    return Enumerable.Range(1, n)
            .Aggregate(initialValue, action);
}

public static int SumOfOddsWithAggregate(int n)
{
    var initialValue = 0;
    Func<int, int, int> action = (sumSoFar, x) =>
        (x % 2 == 0) ? sumSoFar : sumSoFar + x;
    return Enumerable.Range(1, n)
        .Aggregate(initialValue, action);
}

public static int AlternatingSumsWithAggregate(int n)
{
    var initialValue = Tuple.Create(true, 0);
    Func<Tuple<bool, int>, int, Tuple<bool, int>> action =
        (t, x) => t.Item1
            ? Tuple.Create(false, t.Item2 - x)
            : Tuple.Create(true, t.Item2 + x);
    return Enumerable.Range(1, n)
        .Aggregate(initialValue, action)
        .Item2;
}
```

好吧，从某种意义上说，这些实现比原始的 C# 版本更简单、更安全，但泛型类型的所有额外噪声使这种方法比 F# 中的等效代码要优雅得多。你可以看到为什么大多数 C# 程序员更喜欢坚持使用显式循环。

### 一个更相关的例子

在现实世界中经常出现的一个稍微相关的例子是，当元素是类或结构时，如何获得列表的“最大”元素。LINQ 方法“max”只返回最大值，而不是包含最大值的整个元素。

这是一个使用显式循环的解决方案：

```c#
public class NameAndSize
{
    public string Name;
    public int Size;
}

public static NameAndSize MaxNameAndSize(IList<NameAndSize> list)
{
    if (list.Count() == 0)
    {
        return default(NameAndSize);
    }

    var maxSoFar = list[0];
    foreach (var item in list)
    {
        if (item.Size > maxSoFar.Size)
        {
            maxSoFar = item;
        }
    }
    return maxSoFar;
}
```

在 LINQ 中这样做似乎很难高效地完成（即一次完成），并且已经成为 Stack Overflow 问题。Jon Skeet 甚至为此写了一篇文章。

再次，fold 挺身而出！

以下是使用 `Aggregate` 的 C# 代码：

```c#
public class NameAndSize
{
    public string Name;
    public int Size;
}

public static NameAndSize MaxNameAndSize(IList<NameAndSize> list)
{
    if (!list.Any())
    {
        return default(NameAndSize);
    }

    var initialValue = list[0];
    Func<NameAndSize, NameAndSize, NameAndSize> action =
        (maxSoFar, x) => x.Size > maxSoFar.Size ? x : maxSoFar;
    return list.Aggregate(initialValue, action);
}
```

请注意，此 C# 版本为空列表返回 null。这似乎很危险，那么应该怎么办呢？抛出异常？这似乎也不对。

以下是使用 fold 的 F# 代码：

```F#
type NameAndSize= {Name:string;Size:int}

let maxNameAndSize list =

    let innerMaxNameAndSize initialValue rest =
        let action maxSoFar x = if maxSoFar.Size < x.Size then x else maxSoFar
        rest |> List.fold action initialValue

    // handle empty lists
    match list with
    | [] ->
        None
    | first::rest ->
        let max = innerMaxNameAndSize first rest
        Some max
```

F# 代码有两部分：

- `innerMaxNameAndSize` 函数与我们之前看到的类似。
- 第二个位 `match list with` 根据列表是否为空进行分支。对于空列表，它返回 `None`，在非空的情况下，它返回 `Some`。这样做可以保证函数的调用者必须处理这两种情况。

还有一个测试：

```F#
//test
let list = [
    {Name="Alice"; Size=10}
    {Name="Bob"; Size=1}
    {Name="Carol"; Size=12}
    {Name="David"; Size=5}
    ]
maxNameAndSize list
maxNameAndSize []
```

实际上，我根本不需要写这个，因为 F# 已经有一个 `maxBy` 函数了！

```F#
// use the built in function
list |> List.maxBy (fun item -> item.Size)
[] |> List.maxBy (fun item -> item.Size)
```

但正如你所看到的，它不能很好地处理空列表。这是一个安全包装 `maxBy` 的版本。

```F#
let maxNameAndSize list =
    match list with
    | [] ->
        None
    | _ ->
        let max = list |> List.maxBy (fun item -> item.Size)
        Some max
```

## 11 将函数用作构建块

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/conciseness-functions-as-building-blocks/#series-toc)*)*

函数组合和迷你语言使代码更具可读性
2012年4月11日 这篇文章已有3年多的历史了

https://fsharpforfunandprofit.com/posts/conciseness-functions-as-building-blocks/

一个众所周知的好设计原则是创建一组基本操作，然后以各种方式组合这些构建块，以构建更复杂的行为。在面向对象语言中，这一目标产生了许多实现方法，如“流畅接口”、“策略模式”、“装饰器模式”等。在F#中，它们都是通过函数组合以相同的方式完成的。

让我们从一个使用整数的简单示例开始。假设我们创建了一些基本函数来进行算术运算：

```F#
// building blocks
let add2 x = x + 2
let mult3 x = x * 3
let square x = x * x

// test
[1..10] |> List.map add2 |> printfn "%A"
[1..10] |> List.map mult3 |> printfn "%A"
[1..10] |> List.map square |> printfn "%A"
```

现在我们想创建基于这些的新函数：

```F#
// new composed functions
let add2ThenMult3 = add2 >> mult3
let mult3ThenSquare = mult3 >> square
```

“`>>`”运算符是组合运算符。它的意思是：执行第一个函数，然后执行第二个函数。

注意这种组合函数的方式是多么简洁。没有参数、类型或其他无关的噪音。

可以肯定的是，这些例子也可以写得不那么简洁，更明确：

```F#
let add2ThenMult3 x = mult3 (add2 x)
let mult3ThenSquare x = square (mult3 x)
```

但这种更明确的风格也有点混乱：

- 在显式样式中，必须添加x参数和括号，即使它们不会增加代码的含义。
- 在显式风格中，函数是按照应用顺序从前向后写的。在我的 `add2nMult3` 示例中，我想先加 2，然后相乘。`add2 >> mult3` 语法使其在视觉上比 `mult3(add2 x)`更清晰。

现在让我们测试这些组合：

```F#
// test
add2ThenMult3 5
mult3ThenSquare 5
[1..10] |> List.map add2ThenMult3 |> printfn "%A"
[1..10] |> List.map mult3ThenSquare |> printfn "%A"
```

### 扩展现有功能

现在假设我们想用一些日志行为来装饰这些现有的函数。我们也可以组合这些，以创建一个内置日志的新函数。

```F#
// helper functions;
let logMsg msg x = printf "%s%i" msg x; x     //without linefeed
let logMsgN msg x = printfn "%s%i" msg x; x   //with linefeed

// new composed function with new improved logging!
let mult3ThenSquareLogged =
   logMsg "before="
   >> mult3
   >> logMsg " after mult3="
   >> square
   >> logMsgN " result="

// test
mult3ThenSquareLogged 5
[1..10] |> List.map mult3ThenSquareLogged //apply to a whole list
```

我们的新函数 `mult3ThenSquareLogged` 的名字很难看，但它易于使用，很好地隐藏了其中函数的复杂性。你可以看到，如果你很好地定义了构建块函数，这种函数组合可以成为获得新功能的强大方式。

但是等等，还有更多！函数是 F# 中的第一类实体，任何其他F#代码都可以对其进行操作。下面是一个使用组合运算符将函数列表折叠为单个操作的示例。

```F#
let listOfFunctions = [
   mult3;
   square;
   add2;
   logMsgN "result=";
   ]

// compose all functions in the list into a single one
let allFunctions = List.reduce (>>) listOfFunctions

//test
allFunctions 5
```

### 迷你语言

领域特定语言（DSL）被公认为是一种创建更可读、更简洁代码的技术。功能方法非常适合这一点。

如果需要，你可以选择拥有一个完整的“外部”DSL，它有自己的词法分析器、解析器等，F#有各种工具集，使这变得非常简单。

但在许多情况下，更容易保持F#的语法，只需设计一组“动词”和“名词”来封装我们想要的行为。

简洁地创建新类型并与之匹配的能力使得快速设置流畅的界面变得非常容易。例如，这里有一个使用简单词汇表计算日期的小函数。请注意，仅为此函数定义了两种新的枚举样式类型。

```F#
// set up the vocabulary
type DateScale = Hour | Hours | Day | Days | Week | Weeks
type DateDirection = Ago | Hence

// define a function that matches on the vocabulary
let getDate interval scale direction =
    let absHours = match scale with
                   | Hour | Hours -> 1 * interval
                   | Day | Days -> 24 * interval
                   | Week | Weeks -> 24 * 7 * interval
    let signedHours = match direction with
                      | Ago -> -1 * absHours
                      | Hence ->  absHours
    System.DateTime.Now.AddHours(float signedHours)

// test some examples
let example1 = getDate 5 Days Ago
let example2 = getDate 1 Hour Hence

// the C# equivalent would probably be more like this:
// getDate().Interval(5).Days().Ago()
// getDate().Interval(1).Hour().Hence()
```

上面的例子只有一个“动词”，对“名词”使用了很多类型。

以下示例演示了如何构建具有许多“动词”的流畅界面的功能等效物。

假设我们正在创建一个具有各种形状的绘图程序。每个形状都有一个颜色、大小、标签和点击时要执行的操作，我们希望有一个流畅的界面来配置每个形状。

下面是一个C#中流畅接口的简单方法链的示例：

```c#
FluentShape.Default
   .SetColor("red")
   .SetLabel("box")
   .OnClick( s => Console.Write("clicked") );
```

现在，“流畅接口”和“方法链”的概念实际上只适用于面向对象的设计。在F#这样的函数式语言中，最接近的等价物是使用管道运算符将一组函数链接在一起。

让我们从基础Shape类型开始：

```F#
// create an underlying type
type FluentShape = {
    label : string;
    color : string;
    onClick : FluentShape->FluentShape // a function type
    }
```

我们将添加一些基本函数：

```F#
let defaultShape =
    {label=""; color=""; onClick=fun shape->shape}

let click shape =
    shape.onClick shape

let display shape =
    printfn "My label=%s and my color=%s" shape.label shape.color
    shape   //return same shape
```

为了使“方法链”工作，每个函数都应该返回一个可以在链中下一步使用的对象。因此，您将看到“`display`”函数返回形状，而不是什么都不返回。

接下来，我们创建一些辅助函数，将其作为“迷你语言”公开，并将被该语言的用户用作构建块。

```F#
let setLabel label shape =
   {shape with FluentShape.label = label}

let setColor color shape =
   {shape with FluentShape.color = color}

//add a click action to what is already there
let appendClickAction action shape =
   {shape with FluentShape.onClick = shape.onClick >> action}
```

请注意，`appendClickAction` 将函数作为参数，并将其与现有的单击操作组合在一起。当你开始深入了解重用的函数方法时，你会开始看到更多像这样的“高阶函数”，即作用于其他函数的函数。组合这样的函数是理解函数式编程的关键之一。

现在，作为这种“迷你语言”的用户，我可以将基本辅助函数组合成我自己的更复杂的函数，创建我自己的函数库。（在 C# 中，这种事情可以使用扩展方法来完成。）

```F#
// Compose two "base" functions to make a compound function.
let setRedBox = setColor "red" >> setLabel "box"

// Create another function by composing with previous function.
// It overrides the color value but leaves the label alone.
let setBlueBox = setRedBox >> setColor "blue"

// Make a special case of appendClickAction
let changeColorOnClick color = appendClickAction (setColor color)
```

然后，我可以将这些函数组合在一起，创建具有所需行为的对象。

```F#
//setup some test values
let redBox = defaultShape |> setRedBox
let blueBox = defaultShape |> setBlueBox

// create a shape that changes color when clicked
redBox
    |> display
    |> changeColorOnClick "green"
    |> click
    |> display  // new version after the click

// create a shape that changes label and color when clicked
blueBox
    |> display
    |> appendClickAction (setLabel "box2" >> setColor "green")
    |> click
    |> display  // new version after the click
```

在第二种情况下，我实际上将两个函数传递给 `appendClickAction`，但我首先将它们组合成一个。对于一个结构良好的函数库来说，这类事情是微不足道的，但在 C# 中，如果没有 lambdas 中的 lambdas，就很难做到这一点。

这里有一个更复杂的例子。我们将创建一个函数“`showRainbow`”，为彩虹中的每种颜色设置颜色并显示形状。

```F#
let rainbow =
    ["red";"orange";"yellow";"green";"blue";"indigo";"violet"]

let showRainbow =
    let setColorAndDisplay color = setColor color >> display
    rainbow
    |> List.map setColorAndDisplay
    |> List.reduce (>>)

// test the showRainbow function
defaultShape |> showRainbow
```

请注意，函数变得越来越复杂，但代码量仍然很小。其中一个原因是，在进行函数组合时，函数参数通常可以忽略，这减少了视觉混乱。例如，“`showRainbow`”函数确实将形状作为参数，但没有明确显示！这种参数的省略被称为“无点”风格，将在“函数式思维”系列中进一步讨论。



## 12 简洁的模式匹配

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/conciseness-pattern-matching/#series-toc)*)*

模式匹配可以在一个步骤中进行匹配和绑定
2012年4月12日 这篇文章已有3年多的历史了

https://fsharpforfunandprofit.com/posts/conciseness-pattern-matching/

到目前为止，我们已经看到了匹配中的模式匹配逻辑。。对于表达式，它似乎只是一个switch/case语句。但事实上，模式匹配更为通用——它可以通过多种方式比较表达式，在值、条件和类型上进行匹配，然后同时分配或提取值。

模式匹配将在后面的文章中进行深入讨论，但首先，这里有一个帮助简洁的方法。我们将研究模式匹配用于将值绑定到表达式的方式（相当于赋值给变量的功能）。

在以下示例中，我们直接绑定到元组和列表的内部成员：

```F#
//matching tuples directly
let firstPart, secondPart, _ =  (1,2,3)  // underscore means ignore

//matching lists directly
let elem1::elem2::rest = [1..10]       // ignore the warning for now

//matching lists inside a match..with
let listMatcher aList =
    match aList with
    | [] -> printfn "the list is empty"
    | [firstElement] -> printfn "the list has one element %A " firstElement
    | [first; second] -> printfn "list is %A and %A" first second
    | _ -> printfn "the list has more than two elements"

listMatcher [1;2;3;4]
listMatcher [1;2]
listMatcher [1]
listMatcher []
```

您还可以将值绑定到记录等复杂结构的内部。在下面的示例中，我们将创建一个“`Address`”类型，然后创建一个包含地址的“`Customer`”类型。接下来，我们将创建一个客户价值，然后将各种属性与之匹配。

```F#
// create some types
type Address = { Street: string; City: string; }
type Customer = { ID: int; Name: string; Address: Address}

// create a customer
let customer1 = { ID = 1; Name = "Bob";
      Address = {Street="123 Main"; City="NY" } }

// extract name only
let { Name=name1 } =  customer1
printfn "The customer is called %s" name1

// extract name and id
let { ID=id2; Name=name2; } =  customer1
printfn "The customer called %s has id %i" name2 id2

// extract name and address
let { Name=name3;  Address={Street=street3}  } =  customer1
printfn "The customer is called %s and lives on %s" name3 street3
```

在最后一个示例中，请注意我们如何直接进入 `Address` 子结构，并提取街道和客户名称。

这种在一个步骤中处理嵌套结构、仅提取所需字段并将其赋值的能力非常有用。它消除了相当多的编码苦差事，是典型 F# 代码简洁性的另一个因素。

## 13 便利性

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/convenience-intro/#series-toc)*)*

减少编程繁琐和样板代码的功能
2012年4月13日 这篇文章已有3年多的历史了

https://fsharpforfunandprofit.com/posts/convenience-intro/

在下一组帖子中，我们将探讨我在“便利性”主题下分组的 F# 的更多功能。这些特性不一定能产生更简洁的代码，但它们确实消除了 C# 中所需的许多繁琐和样板代码。

- **对类型有用的“开箱即用”行为**。您创建的大多数类型都会立即具有一些有用的行为，例如不变性和内置相等性——这些功能必须在C#中显式编码。
- **所有函数都是“接口”**，这意味着接口在面向对象设计中扮演的许多角色都隐含在函数的工作方式中。同样，许多面向对象的设计模式在函数范式中是不必要的或微不足道的。
- **部分应用**。具有许多参数的复杂函数可以固定或“内置”一些参数，但保留其他参数。
- **活跃的模式**（Active patterns）。主动模式（Active patterns）是一种特殊的模式，可以动态地而不是静态地匹配或检测模式。它们非常适合简化常用的解析和分组行为。

## 14 类型的开箱即用行为

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/convenience-types/#series-toc)*)*

不可变和内置相等性，无需编码
14 Apr 2012 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/convenience-types/

F# 的一个优点是，大多数类型立即具有一些有用的“开箱即用”行为，如不变性和内置相等性，这些功能通常必须在 C# 中显式编码。

我所说的“大多数” F# 类型，是指核心的“结构”类型，如元组、记录、联合、选项、列表等。添加了类和其他一些类型来给 .NET 集成提供帮助，但失去了结构类型的一些功能。

这些核心类型的内置功能包括：

- 不可变性
- 调试时打印效果不错
- 相等
- 比较

下文将逐一介绍。

### F# 类型具有内置的不变性

在 C# 和 Java 中，尽可能创建不可变类已经成为一种很好的做法。在 F# 中，你可以免费获得这个。

这是 F# 中的一个不可变类型：

```F#
type PersonalName = {FirstName:string; LastName:string}
```

以下是 C# 中相同类型的典型编码方式：

```c#
class ImmutablePersonalName
{
    public ImmutablePersonalName(string firstName, string lastName)
    {
        this.FirstName = firstName;
        this.LastName = lastName;
    }

    public string FirstName { get; private set; }
    public string LastName { get; private set; }
}
```

这是 10 行代码，与 F# 的 1 行代码做同样的事情。

### 大多数 F# 类型都内置了漂亮的打印功能

在 F# 中，你不必为大多数类型重写 `ToString()`——你可以免费获得漂亮的打印效果！

在运行前面的示例时，您可能已经看到了这一点。下面是另一个简单的例子：

```F#
type USAddress =
   {Street:string; City:string; State:string; Zip:string}
type UKAddress =
   {Street:string; Town:string; PostCode:string}
type Address =
   | US of USAddress
   | UK of UKAddress
type Person =
   {Name:string; Address:Address}

let alice = {
   Name="Alice";
   Address=US {Street="123 Main";City="LA";State="CA";Zip="91201"}}
let bob = {
   Name="Bob";
   Address=UK {Street="221b Baker St";Town="London";PostCode="NW1 6XE"}}

printfn "Alice is %A" alice
printfn "Bob is %A" bob
```

输出为：

```F#
Alice is {Name = "Alice";
 Address = US {Street = "123 Main";
               City = "LA";
               State = "CA";
               Zip = "91201" };}

Bob is {Name = "Bob";
 Address = UK {Street = "221b Baker St";
               Town = "London";
               PostCode = "NW1 6XE";};}
```

### 大多数 F# 类型都有内置的结构相等性

在 C# 中，您通常必须实现 `IEquatable` 接口，以便测试对象之间的相等性。例如，当使用对象作为字典键时，这是必要的。

在 F# 中，大多数 F# 类型都可以免费获得此功能。例如，使用上面的 `PersonalName` 类型，我们可以直接比较两个名字。

```F#
type PersonalName = {FirstName:string; LastName:string}
let alice1 = {FirstName="Alice"; LastName="Adams"}
let alice2 = {FirstName="Alice"; LastName="Adams"}
let bob1 = {FirstName="Bob"; LastName="Bishop"}

//test
printfn "alice1=alice2 is %A" (alice1=alice2)
printfn "alice1=bob1 is %A" (alice1=bob1)
```

### 大多数 F# 类型都是自动可比较的

在 C# 中，您通常必须实现 `IComparable` 接口，以便对对象进行排序。

同样，在 F# 中，大多数 F# 类型都可以免费获得此功能。例如，这是一副扑克牌的简单定义。

```F#
type Suit = Club | Diamond | Spade | Heart
type Rank = Two | Three | Four | Five | Six | Seven | Eight
            | Nine | Ten | Jack | Queen | King | Ace
```

我们可以编写一个函数来测试比较逻辑：

```F#
let compareCard card1 card2 =
    if card1 < card2
    then printfn "%A is greater than %A" card2 card1
    else printfn "%A is greater than %A" card1 card2
```

让我们看看它是如何工作的：

```F#
let aceHearts = Heart, Ace
let twoHearts = Heart, Two
let aceSpades = Spade, Ace

compareCard aceHearts twoHearts
compareCard twoHearts aceSpades
```

请注意，Ace of Hearts 会自动大于 Two of Hearts，因为“Ace”排名值在“Two”排名值之后。

但也要注意，Two of Hearts 会自动大于 Ace of Spades，因为首先比较的是 Suit 部分，而“Heart”套装值在“Spade”值之后。

这是一个纸牌手的例子：

```F#
let hand = [ Club,Ace; Heart,Three; Heart,Ace;
             Spade,Jack; Diamond,Two; Diamond,Ace ]

//instant sorting!
List.sort hand |> printfn "sorted hand is (low to high) %A"
```

作为附带福利，您还可以免费获得最小值和最大值！

```F#
List.max hand |> printfn "high card is %A"
List.min hand |> printfn "low card is %A"
```

> 如果你对领域建模和设计的函数式方法感兴趣，你可能会喜欢我的《领域建模函数式》一书！这是一个初学者友好的介绍，涵盖了领域驱动设计、类型建模和函数式编程。

## 15 作为接口的函数式

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/convenience-functions-as-interfaces/#series-toc)*)*

使用函数时，OO 设计模式可能微不足道
2012年4月15日 这篇文章已有3年多的历史了

https://fsharpforfunandprofit.com/posts/convenience-functions-as-interfaces/

函数式编程的一个重要方面是，从某种意义上说，所有函数都是“接口”，这意味着接口在面向对象设计中扮演的许多角色都隐含在函数的工作方式中。

事实上，关键的设计准则之一，“程序到接口，而不是实现”，在 F# 中是免费的。

为了了解这是如何工作的，让我们比较一下 C# 和 F# 中的相同设计模式。例如，在 C# 中，我们可能希望使用“装饰器模式”来增强一些核心代码。

假设我们有一个计算器界面：

```c#
interface ICalculator
{
   int Calculate(int input);
}
```

然后是一个具体的实现：

```c#
class AddingCalculator: ICalculator
{
   public int Calculate(int input) { return input + 1; }
}
```

然后，如果我们想添加日志记录，我们可以将核心计算器实现封装在日志记录包装器中。

```c#
class LoggingCalculator: ICalculator
{
   ICalculator _innerCalculator;

   LoggingCalculator(ICalculator innerCalculator)
   {
      _innerCalculator = innerCalculator;
   }

   public int Calculate(int input)
   {
      Console.WriteLine("input is {0}", input);
      var result  = _innerCalculator.Calculate(input);
      Console.WriteLine("result is {0}", result);
      return result;
   }
}
```

到目前为止，一切都很简单。但请注意，为了使其工作，我们必须为类定义一个接口。如果没有 ICalculator 接口，则有必要对现有代码进行改装。

这就是 F# 闪耀的地方。在 F# 中，你可以做同样的事情，而不必先定义接口。只要签名相同，任何函数都可以透明地替换为任何其他函数。

这是等效的 F# 代码。

```F#
let addingCalculator input = input + 1

let loggingCalculator innerCalculator input =
   printfn "input is %A" input
   let result = innerCalculator input
   printfn "result is %A" result
   result
```

换句话说，函数的签名就是接口。

### 通用包装

更妙的是，默认情况下，F#日志代码可以完全通用，这样它就可以适用于任何函数。以下是一些示例：

```F#
let add1 input = input + 1
let times2 input = input * 2

let genericLogger anyFunc input =
   printfn "input is %A" input   //log the input
   let result = anyFunc input    //evaluate the function
   printfn "result is %A" result //log the result
   result                        //return the result

let add1WithLogging = genericLogger add1
let times2WithLogging = genericLogger times2
```

新的“包装”函数可以在任何可以使用原始函数的地方使用——没有人能分辨出区别！

```F#
// test
add1WithLogging 3
times2WithLogging 3

[1..5] |> List.map add1WithLogging
```

完全相同的通用包装器方法可用于其他事情。例如，这是一个用于为函数计时的通用包装器。

```F#
let genericTimer anyFunc input =
   let stopwatch = System.Diagnostics.Stopwatch()
   stopwatch.Start()
   let result = anyFunc input  //evaluate the function
   printfn "elapsed ms is %A" stopwatch.ElapsedMilliseconds
   result

let add1WithTimer = genericTimer add1WithLogging

// test
add1WithTimer 3
```

进行这种通用包装的能力是面向功能方法的巨大便利之一。你可以接受任何函数，并基于它创建一个类似的函数。只要新函数的输入和输出与原始函数完全相同，新函数就可以在任何地方替换原始函数。更多示例：

- 为慢速函数编写通用缓存包装器很容易，这样值只计算一次。
- 为函数编写一个通用的“惰性”包装器也很容易，这样只有在需要结果时才会调用内部函数

### 策略模式

我们可以将这种方法应用于另一种常见的设计模式，即“策略模式”

让我们使用熟悉的继承示例：一个具有 `Cat` 和 `Dog` 子类的 `Animal` 超类，每个子类都重写 `MakeNoise()` 方法以产生不同的噪声。

在真正的函数式设计中，没有子类，而是 `Animal` 类将有一个 `NoiseMaking` 函数，该函数将与构造函数一起传递。这种方法与面向对象设计中的“策略”模式完全相同。

```F#
type Animal(noiseMakingStrategy) =
   member this.MakeNoise =
      noiseMakingStrategy() |> printfn "Making noise %s"

// now create a cat
let meowing() = "Meow"
let cat = Animal(meowing)
cat.MakeNoise

// .. and a dog
let woofOrBark() = if (System.DateTime.Now.Second % 2 = 0)
                   then "Woof" else "Bark"
let dog = Animal(woofOrBark)
dog.MakeNoise
dog.MakeNoise  //try again a second later
```

再次注意，我们不必先定义任何类型的 `INoiseMakingStrategy` 接口。任何具有正确签名的函数都可以工作。因此，在函数模型中，标准。NET“策略”接口（如 `IComparer`、`IFormatProvider` 和 `IServiceProvider`）变得无关紧要。

许多其他设计模式也可以用同样的方式简化。

## 16 局部应用

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/convenience-partial-application/#series-toc)*)*

如何固定函数的一些参数
16 Apr 2012 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/convenience-partial-application/

F# 的一个特别方便的特性是，具有许多参数的复杂函数可以将一些参数固定或“内置”，但其他参数保持打开状态。在这篇文章中，我们将快速了解如何在实践中使用它。

让我们从一个非常简单的例子开始，看看它是如何工作的。我们将从一个简单的函数开始：

```F#
// define a adding function
let add x y = x + y

// normal use
let z = add 1 2
```

但我们也可以做一些奇怪的事情——我们可以只用一个参数调用函数！

```F#
let add42 = add 42
```

结果是一个新函数，它内置了“42”，现在只接受一个参数，而不是两个！这种技术被称为“部分应用”，这意味着，对于任何函数，你都可以“固定”一些参数，并留下其他参数供以后填写。

```F#
// use the new function
add42 2
add42 3
```

有了这些，让我们重新审视一下我们之前看到的通用记录器：

```F#
let genericLogger anyFunc input =
   printfn "input is %A" input   //log the input
   let result = anyFunc input    //evaluate the function
   printfn "result is %A" result //log the result
   result                        //return the result
```

不幸的是，我已经硬编码了日志操作。理想情况下，我想让它更通用，这样我就可以选择如何进行日志记录。

当然，F#是一种函数式编程语言，我们将通过传递函数来实现这一点。

在这种情况下，我们会将“before”和“after”回调函数传递给库函数，如下所示：

```F#
let genericLogger before after anyFunc input =
   before input               //callback for custom behavior
   let result = anyFunc input //evaluate the function
   after result               //callback for custom behavior
   result                     //return the result
```

您可以看到日志功能现在有四个参数。“before”和“after”动作作为显式参数以及函数及其输入传递。为了在实践中使用它，我们只需定义函数并将它们与最后的 int 参数一起传递给库函数：

```F#
let add1 input = input + 1

// reuse case 1
genericLogger
    (fun x -> printf "before=%i. " x) // function to call before
    (fun x -> printfn " after=%i." x) // function to call after
    add1                              // main function
    2                                 // parameter

// reuse case 2
genericLogger
    (fun x -> printf "started with=%i " x) // different callback
    (fun x -> printfn " ended with=%i" x)
    add1                              // main function
    2                                 // parameter
```

这要灵活得多。我不必每次想改变行为时都创建一个新函数——我可以动态定义行为。

但你可能会认为这有点丑陋。一个库函数可能会暴露许多回调函数，并且必须一遍又一遍地传递相同的函数会很不方便。

幸运的是，我们知道解决这个问题的办法。我们可以使用部分应用程序来修复一些参数。因此，在这种情况下，让我们定义一个新函数，它固定了 `before` 和 `after` 函数以及 `add1` 函数，但保留了最后一个参数。

```F#
// define a reusable function with the "callback" functions fixed
let add1WithConsoleLogging =
    genericLogger
        (fun x -> printf "input=%i. " x)
        (fun x -> printfn " result=%i" x)
        add1
        // last parameter NOT defined here yet!
```

现在只使用 int 调用新的“包装器”函数，因此代码更简洁。与前面的示例一样，它可以在任何可以使用原始 `add1` 函数而无需任何更改的地方使用。

```F#
add1WithConsoleLogging 2
add1WithConsoleLogging 3
add1WithConsoleLogging 4
[1..5] |> List.map add1WithConsoleLogging
```

### C# 中的函数方法

在经典的面向对象方法中，我们可能会使用继承来做这类事情。例如，我们可能有一个抽象的 `LoggerBase` 类，其中包含“`before`”和“`after`”的虚拟方法以及要执行的函数。然后，为了实现一种特定的行为，我们会创建一个新的子类，并根据需要重写虚拟方法。

但在面向对象的设计中，经典风格的继承现在变得不受欢迎，对象的组合更受欢迎。事实上，在“现代”C#中，我们可能会以与F#相同的方式编写代码，要么使用事件，要么传入函数。

这是转换为C#的F#代码（请注意，我必须为每个Action指定类型）

```c#
public class GenericLoggerHelper<TInput, TResult>
{
    public TResult GenericLogger(
        Action<TInput> before,
        Action<TResult> after,
        Func<TInput, TResult> aFunc,
        TInput input)
    {
        before(input);             //callback for custom behavior
        var result = aFunc(input); //do the function
        after(result);             //callback for custom behavior
        return result;
    }
}
```

它在这里使用：

```c#
[NUnit.Framework.Test]
public void TestGenericLogger()
{
    var sut = new GenericLoggerHelper<int, int>();
    sut.GenericLogger(
        x => Console.Write("input={0}. ", x),
        x => Console.WriteLine(" result={0}", x),
        x => x + 1,
        3);
}
```

在 C# 中，使用 LINQ 库时需要这种编程风格，但许多开发人员还没有完全接受它，以使自己的代码更通用、更具适应性。而且，所需的丑陋的 `Action<>` 和 `Func<>` 类型声明也无济于事。但它肯定可以使代码更具可重用性。

## 17 活动模式（Active patterns）

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/convenience-active-patterns/#series-toc)*)*

动态模式（Dynamic patterns），实现强力匹配
2012年4月17日 这篇文章已有3年多的历史了

https://fsharpforfunandprofit.com/posts/convenience-active-patterns/

F# 有一种特殊类型的模式匹配，称为“活动模式”，可以动态解析或检测模式。与正常模式一样，从调用者的角度来看，匹配和输出被组合成一个步骤。

下面是一个使用活动模式将字符串解析为 int 或 bool 的示例。

```F#
// create an active pattern
let (|Int|_|) str =
   match System.Int32.TryParse(str:string) with
   | (true,int) -> Some(int)
   | _ -> None

// create an active pattern
let (|Bool|_|) str =
   match System.Boolean.TryParse(str:string) with
   | (true,bool) -> Some(bool)
   | _ -> None
```

> 您现在不需要担心用于定义活动模式的复杂语法——这只是一个例子，这样您就可以看到它们是如何使用的。

一旦设置了这些模式，它们就可以用作正常的“`match..with`”表达式的一部分。

```F#
// create a function to call the patterns
let testParse str =
    match str with
    | Int i -> printfn "The value is an int '%i'" i
    | Bool b -> printfn "The value is a bool '%b'" b
    | _ -> printfn "The value '%s' is something else" str

// test
testParse "12"
testParse "true"
testParse "abc"
```

您可以看到，从调用者的角度来看，与 `Int` 或 `Bool` 的匹配是透明的，即使在幕后进行了解析。

一个类似的例子是将活动模式与正则表达式一起使用，以便在正则表达式模式上进行匹配，并在一个步骤中返回匹配的值。

```F#
// create an active pattern
open System.Text.RegularExpressions
let (|FirstRegexGroup|_|) pattern input =
   let m = Regex.Match(input,pattern)
   if (m.Success) then Some m.Groups.[1].Value else None
```

同样，一旦设置了此模式，它就可以透明地用作正常匹配表达式的一部分。

```F#
// create a function to call the pattern
let testRegex str =
    match str with
    | FirstRegexGroup "http://(.*?)/(.*)" host ->
           printfn "The value is a url and the host is %s" host
    | FirstRegexGroup ".*?@(.*)" host ->
           printfn "The value is an email and the host is %s" host
    | _ -> printfn "The value '%s' is something else" str

// test
testRegex "http://google.com/test"
testRegex "alice@hotmail.com"
```

为了好玩，这里还有一个：著名的使用主动模式编写的 FizzBuzz 挑战。

```F#
// setup the active patterns
let (|MultOf3|_|) i = if i % 3 = 0 then Some MultOf3 else None
let (|MultOf5|_|) i = if i % 5 = 0 then Some MultOf5 else None

// the main function
let fizzBuzz i =
  match i with
  | MultOf3 & MultOf5 -> printf "FizzBuzz, "
  | MultOf3 -> printf "Fizz, "
  | MultOf5 -> printf "Buzz, "
  | _ -> printf "%i, " i

// test
[1..20] |> List.iter fizzBuzz
```

## 18 正确性

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/correctness-intro/#series-toc)*)*

如何编写“编译时单元测试”
2012年4月18日 这篇文章已有3年多的历史了

https://fsharpforfunandprofit.com/posts/correctness-intro/

作为一名程序员，你不断地判断你和其他人编写的代码。在一个理想的世界里，你应该能够看到一段代码，并很容易地理解它的确切功能；当然，简洁、清晰和可读是其中的一个主要因素。

但更重要的是，你必须能够说服自己，代码做了它应该做的事情。当你编程时，你不断地推理代码的正确性，你大脑中的小编译器正在检查代码是否有错误和可能的错误。

那么，编程语言如何帮助你做到这一点呢？

像 C# 这样的现代命令式语言提供了许多您已经熟悉的方法：类型检查、作用域和命名规则、访问修饰符等。而且，在最近的版本中，还提供了静态代码分析和代码契约。

所有这些技术意味着编译器可以承担很多检查正确性的负担。如果你犯了错误，编译器会警告你。

但是 F# 有一些额外的功能，可以对确保正确性产生巨大影响。接下来的几个帖子将专门讨论其中的四个：

- **不变性**，使代码的行为更加可预测。
- **详尽的模式匹配**，在编译时捕获了许多常见错误。
- **一个严格的类型系统**，它是你的朋友，而不是你的敌人。您几乎可以将静态类型检查用作即时的“编译时单元测试”。
- **一个富有表现力的类型系统**，可以帮助你“使非法状态变得不可表示”*。我们将看到如何设计一个真实世界的例子来演示这一点。

*感谢简街的亚伦·明斯基（Yaron Minsky at Jane Street）说出这句话。

## 19 不可变性

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/correctness-immutability/#series-toc)*)*

让你的代码可预测
2012年4月19日 这篇文章已有3年多的历史了

https://fsharpforfunandprofit.com/posts/correctness-immutability/

为了了解为什么不变性很重要，让我们从一个小例子开始。

下面是一些处理数字列表的简单C#代码。

```c#
public List<int> MakeList()
{
   return new List<int> {1,2,3,4,5,6,7,8,9,10};
}

public List<int> OddNumbers(List<int> list)
{
   // some code
}

public List<int> EvenNumbers(List<int> list)
{
   // some code
}
```

现在让我来测试一下：

```c#
public void Test()
{
   var odds = OddNumbers(MakeList());
   var evens = EvenNumbers(MakeList());
   // assert odds = 1,3,5,7,9 -- OK!
   // assert evens = 2,4,6,8,10 -- OK!
}
```

一切都很好，测试也通过了，但我注意到我创建了两次列表——我当然应该重构它吗？所以我进行了重构，这是新的改进版本：

```c#
public void RefactoredTest()
{
   var list = MakeList();
   var odds = OddNumbers(list);
   var evens = EvenNumbers(list);
   // assert odds = 1,3,5,7,9 -- OK!
   // assert evens = 2,4,6,8,10 -- FAIL!
}
```

但现在测试突然失败了！为什么重构会破坏测试？你能只看代码就知道吗？

当然，答案是列表是可变的，并且很可能 `OddNumbers` 函数正在对列表进行破坏性更改，作为其过滤逻辑的一部分。当然，为了确定，我们必须检查 `OddNumbers` 函数内的代码。

换句话说，当我调用 `OddNumbers` 函数时，我无意中产生了不良的副作用。

有没有办法确保这种情况不会发生？是–如果函数使用了 `IEnumerable`：

```c#
public IEnumerable<int> MakeList() {}
public List<int> OddNumbers(IEnumerable<int> list) {}
public List<int> EvenNumbers(IEnumerable <int> list) {}
```

在这种情况下，我们可以确信调用 `OddNumbers` 函数不可能对列表产生任何影响，`EvenNumbers` 将正常工作。更重要的是，我们只需查看签名就可以知道这一点，而不必检查函数的内部。如果你试图通过分配给列表来使其中一个函数行为异常，那么在编译时你会立刻得到一个错误。

因此，在这种情况下，`IEnumerable` 可以提供帮助，但如果我使用了 `IEnumerable<Person>` 这样的类型而不是 `IEnumerable<int>` 呢？我还能相信这些功能不会有无意的副作用吗？

### 不变性重要的原因

上面的例子说明了为什么不变性是有帮助的。事实上，这只是冰山一角。不变性很重要的原因有很多：

- 不可变数据使代码可预测
- 不可变数据更易于使用
- 不可变数据迫使您使用“转型”方法

首先，不变性使代码具有**可预测性**。如果数据是不可变的，就不会有副作用。如果没有副作用，就更容易对代码的正确性进行推理。

当你有两个函数处理不可变数据时，你不必担心调用它们的顺序，也不必担心一个函数是否会干扰另一个函数的输入。在传递数据时，您可以放心（例如，您不必担心将对象用作哈希表中的键并更改其哈希码）。

事实上，不变性是一个好主意，因为全局变量是一个坏主意：数据应尽可能保持局部，并应避免副作用。

其次，不变性**更容易使用**。如果数据是不可变的，那么许多常见的任务就会变得容易得多。代码更容易编写，也更容易维护。需要更少的单元测试（你只需要检查一个函数是否独立工作），并且模拟要容易得多。并发性要简单得多，因为您不必担心使用锁来避免更新冲突（因为没有更新）。

最后，默认使用不变性意味着您开始以不同的方式思考编程。你倾向于考虑**转换**数据，而不是在原地对其进行突变。

SQL 查询和 LINQ 查询就是这种“转换”方法的好例子。在这两种情况下，您总是通过各种函数（选择、筛选、排序）转换原始数据，而不是修改原始数据。

当使用转换方法设计程序时，结果往往更优雅、更模块化、更具可扩展性。碰巧的是，转换方法也与面向功能的范式完美契合。

### F# 如何实现不变性

我们之前看到不可变值和类型是F#的默认：

```F#
// immutable list
let list = [1;2;3;4]

type PersonalName = {FirstName:string; LastName:string}
// immutable person
let john = {FirstName="John"; LastName="Doe"}
```

因此，F# 有许多技巧可以让生活更轻松，并优化底层代码。

首先，由于您无法修改数据结构，因此必须在需要更改时复制它。F# 可以轻松复制另一个数据结构，只需进行所需的更改：

```F#
let alice = {john with FirstName="Alice"}
```

复杂的数据结构被实现为链表或类似结构，以便共享结构的公共部分。

```F#
// create an immutable list
let list1 = [1;2;3;4]

// prepend to make a new list
let list2 = 0::list1

// get the last 4 of the second list
let list3 = list2.Tail

// the two lists are the identical object in memory!
System.Object.ReferenceEquals(list1,list3)
```

这种技术可以确保，虽然你的代码中可能有数百个列表副本，但它们在幕后共享相同的内存。

### 可变数据

F# 对不变性并不教条；它确实支持使用 `mutable` 关键字的可变数据。但是启用可变性是一个明确的决定，与默认值不同，通常只在优化、缓存等特殊情况下或处理 .NET 库时才需要。

在实践中，如果一个严肃的应用程序处理用户界面、数据库、网络等混乱的世界，它必然会有一些可变状态。但 F# 鼓励将这种可变状态最小化。通常，您仍然可以将核心业务逻辑设计为使用不可变数据，并获得所有相应的好处。

## 20 穷举的模式匹配

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/correctness-exhaustive-pattern-matching/#series-toc)*)*

一种确保正确性的强大技术
2012年4月20日 这篇文章已有3年多的历史了

https://fsharpforfunandprofit.com/posts/correctness-exhaustive-pattern-matching/

我们之前简要地指出，在模式匹配时，需要匹配所有可能的情况。事实证明，这是一种非常强大的技术，可以确保正确性。

让我们再次比较一下 C# 和 F#。下面是一些使用 switch 语句处理不同类型状态的C#代码。

```c#
enum State { New, Draft, Published, Inactive, Discontinued }
void HandleState(State state)
{
    switch (state)
    {
    case State.Inactive:
        ...
    case State.Draft:
        ...
    case State.New:
        ...
    case State.Discontinued:
        ...
    }
}
```

这段代码可以编译，但有一个明显的错误！编译器看不到它——你能吗？如果你能，而且你已经解决了这个问题，如果我在名单上增加另一个 `State`，它会保持不变吗？

这是F#的等价物：

```F#
type State = New | Draft | Published | Inactive | Discontinued
let handleState state =
   match state with
   | Inactive ->
      ...
   | Draft ->
      ...
   | New ->
      ...
   | Discontinued ->
      ...
```

现在试着运行这段代码。编译器告诉你什么？它会说：

`此表达式上的模式匹配不完整。`
`例如，值“Published”可能表示模式未涵盖的案例`

穷举匹配总是完成的事实意味着编译器会立即检测到某些常见错误：

- 缺失的案例（通常是由于需求更改或重构而添加新选项时造成的）。
- 一个不可能的情况（当现有的选择被删除时）。
- 一个永远无法触及的冗余案例（该案例已被归入之前的案例中——这有时可能是不明显的）。

现在，让我们来看一些真实的例子，说明穷举匹配如何帮助您编写正确的代码。

### 避免 Option 类型为空

我们将从一个非常常见的场景开始，在这个场景中，调用者应该始终检查无效的情况，即测试null。一个典型的C#程序中充斥着这样的代码：

```c#
if (myObject != null)
{
  // do something
}
```

不幸的是，编译器不需要此测试。只需一段代码忘记执行此操作，程序就会崩溃。多年来，大量的编程工作都致力于处理 null——null 的发明甚至被称为十亿美元的错误！

在纯 F# 中，null 不能意外存在。字符串或对象在创建时必须始终分配给某物，此后是不可变的。

然而，在许多情况下，设计意图是区分有效值和无效值，您需要调用者处理这两种情况。

在 C# 中，在某些情况下可以通过使用可以为 null 的值类型（如 `Nullable<int>`）来管理这一点，以使设计决策清晰明了。当遇到可空值时，编译器会强制您注意它。然后，您可以在使用该值之前测试其有效性。但可空值不适用于标准类（即引用类型），并且很容易意外绕过测试，直接调用 `Value`。

在 F# 中，有一个类似但更强大的概念来传达设计意图：名为 `Option` 的通用包装器类型，有两个选择：`Some` 或 `None`。`Some` 选项包装了一个有效值，`None` 表示缺少值。

这里有一个例子，如果文件存在，则返回 `Some`，但缺少文件则返回 `None`。

```F#
let getFileInfo filePath =
   let fi = new System.IO.FileInfo(filePath)
   if fi.Exists then Some(fi) else None

let goodFileName = "good.txt"
let badFileName = "bad.txt"

let goodFileInfo = getFileInfo goodFileName // Some(fileinfo)
let badFileInfo = getFileInfo badFileName   // None
```

如果我们想用这些值做任何事情，我们必须始终处理这两种可能的情况。

```F#
match goodFileInfo with
  | Some fileInfo ->
      printfn "the file %s exists" fileInfo.FullName
  | None ->
      printfn "the file doesn't exist"

match badFileInfo with
  | Some fileInfo ->
      printfn "the file %s exists" fileInfo.FullName
  | None ->
      printfn "the file doesn't exist"
```

我们别无选择。不处理案例是编译时错误，而不是运行时错误。通过避免 null 并以这种方式使用 Option 类型，F# 完全消除了大量 null 引用异常。

警告：F# 确实允许您在不进行测试的情况下访问值，就像 C# 一样，但这被认为是极其糟糕的做法。

### 边缘情况下的穷尽模式匹配

下面是一些C#代码，它通过对输入列表中的数字对求平均值来创建列表：

```c#
public IList<float> MovingAverages(IList<int> list)
{
    var averages = new List<float>();
    for (int i = 0; i < list.Count; i++)
    {
        var avg = (list[i] + list[i+1]) / 2;
        averages.Add(avg);
    }
    return averages;
}
```

它编译正确，但实际上有几个问题。你能很快找到他们吗？如果你幸运的话，你的单元测试会为你找到它们，假设你已经考虑了所有的边缘情况。

现在让我们在 F# 中尝试同样的事情：

```F#
let rec movingAverages list =
    match list with
    // if input is empty, return an empty list
    | [] -> []
    // otherwise process pairs of items from the input
    | x::y::rest ->
        let avg = (x+y)/2.0
        //build the result by recursing the rest of the list
        avg :: movingAverages (y::rest)
```

这段代码也有一个 bug。但与 C# 不同，在我修复之前，这段代码甚至不会编译。当我的列表中只有一个项目时，编译器会告诉我，我还没有处理过这种情况。它不仅发现了一个 bug，还揭示了需求中的一个缺口：当只有一个项目时应该发生什么？

以下是修复版本：

```F#
let rec movingAverages list =
    match list with
    // if input is empty, return an empty list
    | [] -> []
    // otherwise process pairs of items from the input
    | x::y::rest ->
        let avg = (x+y)/2.0
        //build the result by recursing the rest of the list
        avg :: movingAverages (y::rest)
    // for one item, return an empty list
    | [_] -> []

// test
movingAverages [1.0]
movingAverages [1.0; 2.0]
movingAverages [1.0; 2.0; 3.0]
```

作为额外的好处，F# 代码也更加自文档化。它明确地描述了每个案例的后果。在 C# 代码中，如果列表为空或只有一个项目，会发生什么并不明显。你必须仔细阅读代码才能找到答案。

### 穷尽模式匹配作为一种错误处理技术

所有选项都必须匹配的事实也可以作为抛出异常的有用替代方案。例如，考虑以下常见场景：

- 在应用程序的最底层有一个实用函数，它打开一个文件并对其执行任意操作（您将其作为回调函数传递）
- 然后将结果向上传递到最高层。
- 客户端调用顶层代码，处理结果并完成任何错误处理。

在过程式或面向对象语言中，跨代码层传播和处理异常是一个常见问题。顶级函数不容易区分它们应该从中恢复的异常（比如 `FileNotFound`）和它们不需要处理的异常（例如 `OutOfMemory`）。在 Java 中，有人试图用检查异常来实现这一点，但结果喜忧参半。

在函数世界中，一种常见的技术是创建一个新的结构来保存好的和坏的可能性，而不是在文件丢失时抛出异常。

```F#
// define a "union" of two different alternatives
type Result<'a, 'b> =
    | Success of 'a  // 'a means generic type. The actual type
                     // will be determined when it is used.
    | Failure of 'b  // generic failure type as well

// define all possible errors
type FileErrorReason =
    | FileNotFound of string
    | UnauthorizedAccess of string * System.Exception

// define a low level function in the bottom layer
let performActionOnFile action filePath =
   try
      //open file, do the action and return the result
      use sr = new System.IO.StreamReader(filePath:string)
      let result = action sr  //do the action to the reader
      Success (result)        // return a Success
   with      // catch some exceptions and convert them to errors
      | :? System.IO.FileNotFoundException as ex
          -> Failure (FileNotFound filePath)
      | :? System.Security.SecurityException as ex
          -> Failure (UnauthorizedAccess (filePath,ex))
      // other exceptions are unhandled
```

该代码演示了 `performActionOnFile` 如何返回一个 `Result` 对象，该对象有两个选项：成功和失败。`Failure` 选项也有两个选项：`FileNotFound` 和 `Unauthorized Access`。

现在，中间层可以相互调用，传递结果类型，而不必担心它的结构是什么，只要它们不访问它：

```F#
// a function in the middle layer
let middleLayerDo action filePath =
    let fileResult = performActionOnFile action filePath
    // do some stuff
    fileResult //return

// a function in the top layer
let topLayerDo action filePath =
    let fileResult = middleLayerDo action filePath
    // do some stuff
    fileResult //return
```

由于类型推断，中间层和顶层不需要指定返回的确切类型。如果下层根本更改了类型定义，则中间层不会受到影响。

显然，在某些时候，顶层的客户端确实希望访问结果。这里是执行匹配所有模式的要求的地方。客户端必须处理 `Failure` 的情况，否则编译器将发出投诉。此外，在处理 `Failure` 分支时，还必须处理可能的原因。换句话说，这种特殊情况处理可以在编译时强制执行，而不是在运行时！此外，通过检查原因类型，明确记录了可能的原因。

以下是一个访问顶层的客户端函数示例：

```F#
/// get the first line of the file
let printFirstLineOfFile filePath =
    let fileResult = topLayerDo (fun fs->fs.ReadLine()) filePath

    match fileResult with
    | Success result ->
        // note type-safe string printing with %s
        printfn "first line is: '%s'" result
    | Failure reason ->
       match reason with  // must match EVERY reason
       | FileNotFound file ->
           printfn "File not found: %s" file
       | UnauthorizedAccess (file,_) ->
           printfn "You do not have access to the file: %s" file
```

您可以看到，此代码必须显式处理成功和失败情况，然后对于失败情况，它显式处理不同的原因。如果你想看看如果它不处理其中一种情况会发生什么，试着注释掉处理未经授权访问的行，看看编译器会说什么。

现在，您不需要始终明确地处理所有可能的情况。在下面的示例中，该函数使用下划线通配符将所有失败原因视为一个。如果我们想从严格中获益，这可以被认为是一种糟糕的做法，但至少这是明确的。

```F#
/// get the length of the text in the file
let printLengthOfFile filePath =
   let fileResult =
     topLayerDo (fun fs->fs.ReadToEnd().Length) filePath

   match fileResult with
   | Success result ->
      // note type-safe int printing with %i
      printfn "length is: %i" result
   | Failure _ ->
      printfn "An error happened but I don't want to be specific"
```

现在，让我们通过一些交互式测试来查看所有这些代码在实践中的工作情况。

首先设置一个好文件和一个坏文件。

```F#
/// write some text to a file
let writeSomeText filePath someText =
    use writer = new System.IO.StreamWriter(filePath:string)
    writer.WriteLine(someText:string)

let goodFileName = "good.txt"
let badFileName = "bad.txt"

writeSomeText goodFileName "hello"
```

现在交互式测试：

```F#
printFirstLineOfFile goodFileName
printLengthOfFile goodFileName

printFirstLineOfFile badFileName
printLengthOfFile badFileName
```

我认为你可以看到这种方法非常有吸引力：

- 函数为每个预期的情况（如 `FileNotFound`）返回错误类型，但处理这些类型不需要使调用代码变得丑陋。
- 函数会继续为意外情况（如 `OutOfMemory`）抛出异常，这些异常通常会在程序的顶层被捕获和记录。

这项技术简单方便。类似的（更通用的）方法是函数式编程的标准方法。

在 C# 中使用这种方法也是可行的，但由于缺乏联合类型和类型推理（我们必须在所有地方指定泛型类型），这种方法通常是不切实际的。

### 作为变更管理工具的穷举模式匹配

最后，穷举模式匹配是一种有价值的工具，可以确保代码在需求变化或重构过程中保持正确。

假设需求发生了变化，我们需要处理第三种错误：“不确定”。要实现这一新要求，请按如下方式更改第一个Result类型，并重新评估所有代码。会发生什么？

```F#
type Result<'a, 'b> =
    | Success of 'a
    | Failure of 'b
    | Indeterminate
```

或者有时需求更改会删除一个可能的选择。要模拟这一点，请更改第一个 `Result` 类型，以消除除一个选项外的所有选项。

```F#
type Result<'a> =
    | Success of 'a
```

现在重新评估代码的其余部分。现在怎么办？

这太强大了！当我们调整选择时，我们立即知道所有需要修复的地方来处理变化。这是静态检查类型错误威力的另一个例子。人们常说，像F#这样的函数式语言“如果编译，它必须是正确的”。

## 21 使用类型系统确保代码正确

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/correctness-type-checking/#series-toc)*)*

在 F# 中，类型系统是你的朋友，而不是你的敌人
21 Apr 2012 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/correctness-type-checking/

您熟悉通过 C# 和 Java 等语言进行静态类型检查。在这些语言中，类型检查很简单，但相当粗糙，与 Python 和 Ruby 等动态语言的自由相比，这可能会被视为一种烦恼。

但在 F# 中，类型系统是你的朋友，而不是你的敌人。您几乎可以将静态类型检查用作即时单元测试，以确保您的代码在编译时是正确的。

在前面的文章中，我们已经看到了一些可以用 F# 中的类型系统做的事情：

- 类型及其相关函数提供了一个抽象来对问题域进行建模。因为创建类型很容易，所以很少有理由避免根据给定问题的需要进行设计，而且与 C# 类不同，很难创建能做所有事情的“厨房水槽”类型。
- 明确的类型有助于维护。由于 F# 使用类型推理，您通常可以轻松重命名或重构类型，而无需使用重构工具。如果以不兼容的方式更改类型，这几乎肯定会产生编译时错误，有助于跟踪任何问题。
- 命名良好的类型提供了有关其在程序中角色的即时文档（此文档永远不会过时）。

在这篇文章和下一篇文章中，我们将重点介绍如何使用类型系统来帮助编写正确的代码。我将演示您可以创建这样的设计，如果您的代码实际编译，它几乎肯定会按设计工作。

### 使用标准类型检查

在 C# 中，您使用编译时检查来验证您的代码，而无需考虑它。例如，您会放弃 `List<string>` 而使用纯 List 吗？或者放弃 `Nullable<int>` 并强制使用带强制转换的 `object`？可能不会。

但是，如果你能有更细粒度的类型呢？您甚至可以进行更好的编译时检查。这正是 F# 所提供的。

F# 类型检查器并不比 C# 类型检查器严格得多。但是，由于创建新类型非常容易，没有混乱，因此可以更好地表示域，并且作为一种有用的副作用，可以避免许多常见错误。

这里有一个简单的例子：

```F#
//define a "safe" email address type
type EmailAddress = EmailAddress of string

//define a function that uses it
let sendEmail (EmailAddress email) =
   printfn "sent an email to %s" email

//try to send one
let aliceEmail = EmailAddress "alice@example.com"
sendEmail aliceEmail

//try to send a plain string
sendEmail "bob@example.com"   //error
```

通过将电子邮件地址包装为特殊类型，我们确保普通字符串不能用作电子邮件特定函数的参数。（在实践中，我们也会隐藏 `EmailAddress` 类型的构造函数，以确保一开始只能创建有效值。）

这里没有什么是 C# 做不到的，但仅仅为了这个目的创建一个新的值类型将是一项相当大的工作，所以在 C# 中，很容易懒惰，只是传递字符串。

### F# 中的其他类型安全功能

在继续讨论“为正确性而设计”这一主要话题之前，让我们看看 F# 是类型安全的其他一些次要但很酷的方法。

#### 使用 printf 进行类型安全格式化

这里有一个小特性，演示了F#比C#更具类型安全性的一种方式，以及F#编译器如何捕获仅在C#运行时检测到的错误。

尝试评估以下内容，并查看生成的错误：

```F#
let printingExample =
   printf "an int %i" 2                        // ok
   printf "an int %i" 2.0                      // wrong type
   printf "an int %i" "hello"                  // wrong type
   printf "an int %i"                          // missing param

   printf "a string %s" "hello"                // ok
   printf "a string %s" 2                      // wrong type
   printf "a string %s"                        // missing param
   printf "a string %s" "he" "lo"              // too many params

   printf "an int %i and string %s" 2 "hello"  // ok
   printf "an int %i and string %s" "hello" 2  // wrong type
   printf "an int %i and string %s" 2          // missing param
```

与 C# 不同，编译器分析格式字符串并确定参数的数量和类型。

这可用于约束参数的类型，而无需明确指定。例如，在下面的代码中，编译器可以自动推断参数的类型。

```F#
let printAString x = printf "%s" x
let printAnInt x = printf "%i" x

// the result is:
// val printAString : string -> unit  //takes a string parameter
// val printAnInt : int -> unit       //takes an int parameter
```

#### 计量单位

F# 能够定义度量单位并将其与浮点数相关联。然后，计量单位作为一种类型“附着”在浮子上，防止不同类型的混合。如果你需要的话，这是另一个非常方便的功能。

```F#
// define some measures
[<Measure>]
type cm

[<Measure>]
type inches

[<Measure>]
type feet =
   // add a conversion function
   static member toInches(feet : float<feet>) : float<inches> =
      feet * 12.0<inches/feet>

// define some values
let meter = 100.0<cm>
let yard = 3.0<feet>

//convert to different measure
let yardInInches = feet.toInches(yard)

// can't mix and match!
yard + meter

// now define some currencies
[<Measure>]
type GBP

[<Measure>]
type USD

let gbp10 = 10.0<GBP>
let usd10 = 10.0<USD>
gbp10 + gbp10             // allowed: same currency
gbp10 + usd10             // not allowed: different currency
gbp10 + 1.0               // not allowed: didn't specify a currency
gbp10 + 1.0<_>            // allowed using wildcard
```

#### 类型安全相等

最后一个例子。在 C# 中，任何类都可以与任何其他类相等（默认情况下使用引用相等）。总的来说，这是个坏主意！例如，你根本不应该能够将字符串与人进行比较。

以下是一些完全有效且编译良好的 C# 代码：

```c#
using System;
var obj = new Object();
var ex = new Exception();
var b = (obj == ex);
```

如果我们用 F# 编写相同的代码，我们会得到一个编译时错误：

```F#
open System
let obj = new Object()
let ex = new Exception()
let b = (obj = ex)
```

很可能，如果你在测试两种不同类型之间的相等性，你做错了什么。

在 F# 中，你甚至可以完全停止一个类型的比较！这并不像看起来那么愚蠢。对于某些类型，可能没有有用的默认值，或者您可能希望强制相等性基于特定字段而不是整个对象。

以下是一个示例：

```F#
// deny comparison
[<NoEquality; NoComparison>]
type CustomerAccount = {CustomerAccountId: int}

let x = {CustomerAccountId = 1}

x = x       // error!
x.CustomerAccountId = x.CustomerAccountId // no error
```

> 如果你对领域建模和设计的函数式方法感兴趣，你可能会喜欢我的《领域建模函数式》一书！这是一个初学者友好的介绍，涵盖了领域驱动设计、类型建模和函数式编程。

## 22 示例：为正确性而设计

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/designing-for-correctness/#series-toc)*)*

如何使非法状态不具代表性
22 Apr 2012 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/designing-for-correctness/

在这篇文章中，我们将看到如何为正确性（或至少为您目前理解的需求）进行设计，我的意思是，设计良好的模型的客户端将无法将系统置于非法状态——一种不符合需求的状态。你实际上不能创建不正确的代码，因为编译器不允许你这样做。

为了实现这一点，我们确实需要花一些时间预先考虑设计，并努力将需求编码到您使用的类型中。如果你只对所有数据结构使用字符串或列表，你将无法从类型检查中获得任何好处。

我们将使用一个简单的例子。假设你正在设计一个有购物车的电子商务网站，你有以下要求。

- 您只能为购物车付款一次。
- 购物车付款后，您将无法更改其中的商品。
- 空推车无法付款。

### C# 中的糟糕设计

在 C# 中，我们可能会认为这很简单，直接进入编码。这是一个直观的 C# 实现，乍一看似乎还可以。

```c#
public class NaiveShoppingCart<TItem>
{
   private List<TItem> items;
   private decimal paidAmount;

   public NaiveShoppingCart()
   {
      this.items = new List<TItem>();
      this.paidAmount = 0;
   }

   /// Is cart paid for?
   public bool IsPaidFor { get { return this.paidAmount > 0; } }

   /// Readonly list of items
   public IEnumerable<TItem> Items { get {return this.items; } }

   /// add item only if not paid for
   public void AddItem(TItem item)
   {
      if (!this.IsPaidFor)
      {
         this.items.Add(item);
      }
   }

   /// remove item only if not paid for
   public void RemoveItem(TItem item)
   {
      if (!this.IsPaidFor)
      {
         this.items.Remove(item);
      }
   }

   /// pay for the cart
   public void Pay(decimal amount)
   {
      if (!this.IsPaidFor)
      {
         this.paidAmount = amount;
      }
   }
}
```

不幸的是，这实际上是一个相当糟糕的设计：

- 其中一个要求甚至没有得到满足。你能看到哪一个吗？
- 它有一个主要的设计缺陷，还有一些次要的缺陷。你能看到它们是什么吗？

这么短的代码中有这么多问题！

如果我们有更复杂的需求，代码长达数千行，会发生什么？例如，到处重复的片段：

```c#
if (!this.IsPaidFor) { do something }
```

如果某些方法的需求发生了变化，而其他方法没有，那么它看起来会非常脆弱。

在阅读下一节之前，请花一分钟思考如何在 C# 中更好地实现上述要求，以及这些附加要求：

- 如果你试图做一些需求中不允许的事情，你会得到一个*编译时错误*，而不是运行时错误。例如，您必须创建一个设计，这样您甚至不能从空购物车中调用 `RemoveItem` 方法。
- 购物车在任何状态下的内容都应该是不可变的。这样做的好处是，如果我正在为购物车付款，即使其他流程同时添加或删除项目，购物车的内容也不会更改。

### F# 中的正确设计

让我们退一步，看看我们是否能想出一个更好的设计。看看这些要求，很明显，我们有一个简单的状态机，有三个状态和一些状态转换：

- 购物车可以是空的（Empty）、活动的（Active）或已付款的（PaidFor）
- 当您将商品添加到空（Empty）购物车时，它将变为活动状态（Active）
- 当您从活动（Active）购物车中删除最后一件商品时，它将变为空（Empty）
- 当您为 Active 购物车付款时，它将变为 PaidFor

现在我们可以将业务规则添加到此模型中：

- 您只能将商品添加到空或活动的购物车中
- 您只能从活动购物车中删除商品
- 您只能为处于活动状态的购物车付款

以下是状态图：



值得注意的是，这些面向状态的模型在业务系统中非常常见。产品开发、客户关系管理、订单处理和其他工作流通常可以用这种方式建模。

现在我们有了设计，我们可以用 F# 复制它：

```F#
type CartItem = string    // placeholder for a more complicated type

type EmptyState = NoItems // don't use empty list! We want to
                          // force clients to handle this as a
                          // separate case. E.g. "you have no
                          // items in your cart"

type ActiveState = { UnpaidItems : CartItem list; }
type PaidForState = { PaidItems : CartItem list;
                      Payment : decimal}

type Cart =
    | Empty of EmptyState
    | Active of ActiveState
    | PaidFor of PaidForState
```

我们为每个状态创建一个类型，`Cart` 类型是任何一个状态的选择。我给所有东西都起了一个不同的名字（例如 `PaidItems` 和 `UnpaidItems`，而不仅仅是 `Items`），因为这有助于推理引擎，并使代码更加自文档化。

> 这是一个比前面的例子长得多的例子！现在不要太担心 F# 语法，但我希望你能理解代码的要点，看看它如何融入整体设计。
>
> 此外，请将片段粘贴到脚本文件中，并在出现时自行评估。

接下来，我们可以为每个状态创建操作。需要注意的是，每次操作都会将其中一个状态作为输入，并返回一个新的购物车。也就是说，你从一个特定的已知状态开始，但你返回了一个 `Cart`，它是三种可能状态的包装。

```F#
// =============================
// operations on empty state
// =============================

let addToEmptyState item =
   // returns a new Active Cart
   Cart.Active {UnpaidItems=[item]}

// =============================
// operations on active state
// =============================

let addToActiveState state itemToAdd =
   let newList = itemToAdd :: state.UnpaidItems
   Cart.Active {state with UnpaidItems=newList }

let removeFromActiveState state itemToRemove =
   let newList = state.UnpaidItems
                 |> List.filter (fun i -> i<>itemToRemove)

   match newList with
   | [] -> Cart.Empty NoItems
   | _ -> Cart.Active {state with UnpaidItems=newList}

let payForActiveState state amount =
   // returns a new PaidFor Cart
   Cart.PaidFor {PaidItems=state.UnpaidItems; Payment=amount}
```

接下来，我们将操作作为方法附加到状态

```F#
type EmptyState with
   member this.Add = addToEmptyState

type ActiveState with
   member this.Add = addToActiveState this
   member this.Remove = removeFromActiveState this
   member this.Pay = payForActiveState this
```

我们也可以创建一些购物车级别的辅助方法。在购物车级别，我们必须通过 `match..with` 表达式来明确处理内部状态的每种可能性。

```F#
let addItemToCart cart item =
   match cart with
   | Empty state -> state.Add item
   | Active state -> state.Add item
   | PaidFor state ->
       printfn "ERROR: The cart is paid for"
       cart

let removeItemFromCart cart item =
   match cart with
   | Empty state ->
      printfn "ERROR: The cart is empty"
      cart   // return the cart
   | Active state ->
      state.Remove item
   | PaidFor state ->
      printfn "ERROR: The cart is paid for"
      cart   // return the cart

let displayCart cart  =
   match cart with
   | Empty state ->
      printfn "The cart is empty"   // can't do state.Items
   | Active state ->
      printfn "The cart contains %A unpaid items"
                                                state.UnpaidItems
   | PaidFor state ->
      printfn "The cart contains %A paid items. Amount paid: %f"
                                    state.PaidItems state.Payment

type Cart with
   static member NewCart = Cart.Empty NoItems
   member this.Add = addItemToCart this
   member this.Remove = removeItemFromCart this
   member this.Display = displayCart this
```

> 如果你觉得这篇文章很有趣，看看我的《领域建模函数式》一书！这是对领域驱动设计、类型建模和函数式编程的一个很好的介绍。

### 测试设计

现在让我们练习这段代码：

```F#
let emptyCart = Cart.NewCart
printf "emptyCart="; emptyCart.Display

let cartA = emptyCart.Add "A"
printf "cartA="; cartA.Display
```

我们现在有一个活动购物车，里面有一个商品。请注意，“`cartA`”是一个与“`emptyCart`”完全不同的对象，并且处于不同的状态。

让我们继续前进：

```F#
let cartAB = cartA.Add "B"
printf "cartAB="; cartAB.Display

let cartB = cartAB.Remove "A"
printf "cartB="; cartB.Display

let emptyCart2 = cartB.Remove "B"
printf "emptyCart2="; emptyCart2.Display
```

到目前为止，一切顺利。所有这些都是处于不同状态的不同对象，

让我们测试一下不能从空购物车中删除商品的要求：

```F#
let emptyCart3 = emptyCart2.Remove "B"    //error
printf "emptyCart3="; emptyCart3.Display
```

一个错误——正是我们想要的！

现在假设我们想为购物车付款。我们没有在 Cart 级别创建此方法，因为我们不想告诉客户如何处理所有情况。此方法仅适用于活动状态，因此客户端必须显式处理每种情况，并且仅在活动状态匹配时调用 `Pay` 方法。

首先，我们将尝试支付 cartA 费用。

```F#
//  try to pay for cartA
let cartAPaid =
    match cartA with
    | Empty _ | PaidFor _ -> cartA
    | Active state -> state.Pay 100m
printf "cartAPaid="; cartAPaid.Display
```

结果是一辆付费购物车。

现在，我们将尝试为 emptyCart 付款。

```F#
//  try to pay for emptyCart
let emptyCartPaid =
    match emptyCart with
    | Empty _ | PaidFor _ -> emptyCart
    | Active state -> state.Pay 100m
printf "emptyCartPaid="; emptyCartPaid.Display
```

什么都没发生。购物车为空，因此不会调用 Active 分支。我们可能想在其他分支中引发错误或记录消息，但无论我们做什么，都不能意外地调用空购物车上的 `Pay` 方法，因为该状态没有可调用的方法！

如果我们不小心试图为已经付款的购物车付款，也会发生同样的事情。

```F#
//  try to pay for cartAB
let cartABPaid =
    match cartAB with
    | Empty _ | PaidFor _ -> cartAB // return the same cart
    | Active state -> state.Pay 100m

//  try to pay for cartAB again
let cartABPaidAgain =
    match cartABPaid with
    | Empty _ | PaidFor _ -> cartABPaid  // return the same cart
    | Active state -> state.Pay 100m
```

你可能会说，上面的客户端代码可能不能代表现实世界中的代码——它表现良好，已经处理了需求。

那么，如果我们有编写糟糕或恶意的客户端代码试图强制付款，会发生什么：

```F#
match cartABPaid with
| Empty state -> state.Pay 100m
| PaidFor state -> state.Pay 100m
| Active state -> state.Pay 100m
```

如果我们试图这样强制它，我们会得到编译错误。客户端不可能创建不符合要求的代码。

### 摘要

我们设计了一个简单的购物车模型，它比 C# 设计有很多好处。

- 它非常清楚地符合要求。这个 API 的客户端不可能调用不符合要求的代码。
- 使用状态意味着可能的代码路径的数量比 C# 版本少得多，因此需要编写的单元测试要少得多。
- 每个函数都很简单，可能第一次就能工作，因为与 C# 版本不同，任何地方都没有条件语句。

> **对原始 C# 代码的分析**
>
> 现在您已经看到了 F# 代码，我们可以用新的眼光重新审视原始的 C# 代码。如果你想知道，以下是我对 C# 购物车示例设计问题的看法。
>
> *未满足要求*：空购物车仍可付款。
>
> *主要设计缺陷*：将支付金额作为 IsPaidFor 的信号，意味着零支付金额永远无法锁定购物车。你确定永远不可能有一辆付费但免费的购物车吗？要求不明确，但如果这后来成为要求呢？需要更改多少代码？
>
> *轻微的设计缺陷*：当试图从空购物车中删除商品时，会发生什么？当试图为已经付款的购物车付款时，应该怎么办？在这些情况下，我们应该抛出异常，还是只是默默地忽略它们？客户应该能够枚举空购物车中的商品，这有意义吗？而且这并不像设计的那样是线程安全的；那么，如果在主线程上进行付款时，辅助线程将商品添加到购物车中，会发生什么？
>
> 这有很多事情要担心。
>
> F# 设计的好处是这些问题都不可能存在。因此，以这种方式设计不仅可以确保正确的代码，还可以真正减少确保设计一开始就是防弹的认知努力。
>
> *编译时检查*：最初的 C# 设计将所有状态和转换混合在一个类中，这使得它非常容易出错。一个更好的方法是创建单独的状态类（比如一个公共基类），这可以降低复杂性，但缺乏内置的“联合”类型意味着你无法静态验证代码的正确性。在 C# 中有很多方法可以实现“联合”类型，但这根本不是惯用的，而在 F# 中则很常见。

### 附录：正确解决方案的 C# 代码

当在 C# 中遇到这些要求时，您可能会立即想到——只需创建一个接口！

但这并不像你想象的那么容易。我写了一篇后续文章来解释原因：C# 中的购物车示例。

如果你有兴趣看看解决方案的 C# 代码是什么样子的，请看下面。此代码满足上述要求，并保证编译时的正确性。

需要注意的关键是，由于 C# 没有联合类型，因此实现使用了一个“fold”函数，该函数有三个函数参数，每个状态一个。要使用购物车，调用者传入一组三个 lambdas，（隐藏）状态决定会发生什么。

```c#
var paidCart = cartA.Do(
    // lambda for Empty state
    state => cartA,
    // lambda for Active state
    state => state.Pay(100),
    // lambda for Paid state
    state => cartA);
```

这种方法意味着调用者永远不能调用“错误”的函数，例如空状态的“Pay”，因为 lambda 的参数不支持它。试试看！

```c#
using System;
using System.Collections.Generic;
using System.Linq;

namespace WhyUseFsharp
{

    public class ShoppingCart<TItem>
    {

        #region ShoppingCart State classes

        /// <summary>
        /// Represents the Empty state
        /// </summary>
        public class EmptyState
        {
            public ShoppingCart<TItem> Add(TItem item)
            {
                var newItems = new[] { item };
                var newState = new ActiveState(newItems);
                return FromState(newState);
            }
        }

        /// <summary>
        /// Represents the Active state
        /// </summary>
        public class ActiveState
        {
            public ActiveState(IEnumerable<TItem> items)
            {
                Items = items;
            }

            public IEnumerable<TItem> Items { get; private set; }

            public ShoppingCart<TItem> Add(TItem item)
            {
                var newItems = new List<TItem>(Items) {item};
                var newState = new ActiveState(newItems);
                return FromState(newState);
            }

            public ShoppingCart<TItem> Remove(TItem item)
            {
                var newItems = new List<TItem>(Items);
                newItems.Remove(item);
                if (newItems.Count > 0)
                {
                    var newState = new ActiveState(newItems);
                    return FromState(newState);
                }
                else
                {
                    var newState = new EmptyState();
                    return FromState(newState);
                }
            }

            public ShoppingCart<TItem> Pay(decimal amount)
            {
                var newState = new PaidForState(Items, amount);
                return FromState(newState);
            }


        }

        /// <summary>
        /// Represents the Paid state
        /// </summary>
        public class PaidForState
        {
            public PaidForState(IEnumerable<TItem> items, decimal amount)
            {
                Items = items.ToList();
                Amount = amount;
            }

            public IEnumerable<TItem> Items { get; private set; }
            public decimal Amount { get; private set; }
        }

        #endregion ShoppingCart State classes

        //====================================
        // Execute of shopping cart proper
        //====================================

        private enum Tag { Empty, Active, PaidFor }
        private readonly Tag _tag = Tag.Empty;
        private readonly object _state;       //has to be a generic object

        /// <summary>
        /// Private ctor. Use FromState instead
        /// </summary>
        private ShoppingCart(Tag tagValue, object state)
        {
            _state = state;
            _tag = tagValue;
        }

        public static ShoppingCart<TItem> FromState(EmptyState state)
        {
            return new ShoppingCart<TItem>(Tag.Empty, state);
        }

        public static ShoppingCart<TItem> FromState(ActiveState state)
        {
            return new ShoppingCart<TItem>(Tag.Active, state);
        }

        public static ShoppingCart<TItem> FromState(PaidForState state)
        {
            return new ShoppingCart<TItem>(Tag.PaidFor, state);
        }

        /// <summary>
        /// Create a new empty cart
        /// </summary>
        public static ShoppingCart<TItem> NewCart()
        {
            var newState = new EmptyState();
            return FromState(newState);
        }

        /// <summary>
        /// Call a function for each case of the state
        /// </summary>
        /// <remarks>
        /// Forcing the caller to pass a function for each possible case means that all cases are handled at all times.
        /// </remarks>
        public TResult Do<TResult>(
            Func<EmptyState, TResult> emptyFn,
            Func<ActiveState, TResult> activeFn,
            Func<PaidForState, TResult> paidForyFn
            )
        {
            switch (_tag)
            {
                case Tag.Empty:
                    return emptyFn(_state as EmptyState);
                case Tag.Active:
                    return activeFn(_state as ActiveState);
                case Tag.PaidFor:
                    return paidForyFn(_state as PaidForState);
                default:
                    throw new InvalidOperationException(string.Format("Tag {0} not recognized", _tag));
            }
        }

        /// <summary>
        /// Do an action without a return value
        /// </summary>
        public void Do(
            Action<EmptyState> emptyFn,
            Action<ActiveState> activeFn,
            Action<PaidForState> paidForyFn
            )
        {
            //convert the Actions into Funcs by returning a dummy value
            Do(
                state => { emptyFn(state); return 0; },
                state => { activeFn(state); return 0; },
                state => { paidForyFn(state); return 0; }
                );
        }



    }

    /// <summary>
    /// Extension methods for my own personal library
    /// </summary>
    public static class ShoppingCartExtension
    {
        /// <summary>
        /// Helper method to Add
        /// </summary>
        public static ShoppingCart<TItem> Add<TItem>(this ShoppingCart<TItem> cart, TItem item)
        {
            return cart.Do(
                state => state.Add(item), //empty case
                state => state.Add(item), //active case
                state => { Console.WriteLine("ERROR: The cart is paid for and items cannot be added"); return cart; } //paid for case
            );
        }

        /// <summary>
        /// Helper method to Remove
        /// </summary>
        public static ShoppingCart<TItem> Remove<TItem>(this ShoppingCart<TItem> cart, TItem item)
        {
            return cart.Do(
                state => { Console.WriteLine("ERROR: The cart is empty and items cannot be removed"); return cart; }, //empty case
                state => state.Remove(item), //active case
                state => { Console.WriteLine("ERROR: The cart is paid for and items cannot be removed"); return cart; } //paid for case
            );
        }

        /// <summary>
        /// Helper method to Display
        /// </summary>
        public static void Display<TItem>(this ShoppingCart<TItem> cart)
        {
            cart.Do(
                state => Console.WriteLine("The cart is empty"),
                state => Console.WriteLine("The active cart contains {0} items", state.Items.Count()),
                state => Console.WriteLine("The paid cart contains {0} items. Amount paid {1}", state.Items.Count(), state.Amount)
            );
        }
    }

    [NUnit.Framework.TestFixture]
    public class CorrectShoppingCartTest
    {
        [NUnit.Framework.Test]
        public void TestCart()
        {
            var emptyCart = ShoppingCart<string>.NewCart();
            emptyCart.Display();

            var cartA = emptyCart.Add("A");  //one item
            cartA.Display();

            var cartAb = cartA.Add("B");  //two items
            cartAb.Display();

            var cartB = cartAb.Remove("A"); //one item
            cartB.Display();

            var emptyCart2 = cartB.Remove("B"); //empty
            emptyCart2.Display();

            Console.WriteLine("Removing from emptyCart");
            emptyCart.Remove("B"); //error


            //  try to pay for cartA
            Console.WriteLine("paying for cartA");
            var paidCart = cartA.Do(
                state => cartA,
                state => state.Pay(100),
                state => cartA);
            paidCart.Display();

            Console.WriteLine("Adding to paidCart");
            paidCart.Add("C");

            //  try to pay for emptyCart
            Console.WriteLine("paying for emptyCart");
            var emptyCartPaid = emptyCart.Do(
                state => emptyCart,
                state => state.Pay(100),
                state => emptyCart);
            emptyCartPaid.Display();
        }
    }
}
```

## 23 并发性

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/concurrency-intro/#series-toc)*)*

我们如何编写软件的下一次重大革命？
2012年4月23日 这篇文章已有3年多的历史了

https://fsharpforfunandprofit.com/posts/concurrency-intro/

如今，我们听到了很多关于并发性的话题，它有多重要，以及它是如何“我们编写软件的下一次重大革命”的。

那么，我们所说的“并发性”到底是什么意思，F# 如何提供帮助？

并发性最简单的定义就是“几件事同时发生，可能还会相互作用”。这似乎是一个微不足道的定义，但关键在于，大多数计算机程序（和语言）都是为串行工作而设计的，一次只处理一件事，并且没有足够的能力处理并发性。

即使计算机程序是为了处理并发性而编写的，也有一个更严重的问题：我们的大脑在考虑并发性时表现不佳。众所周知，编写处理并发性的代码非常困难。或者我应该说，编写正确的并发代码是极其困难的！编写有缺陷的并发代码非常容易；可能存在竞争条件，或者操作可能不是原子性的，或者任务可能被不必要地饥饿或阻塞，通过查看代码或使用调试器很难发现这些问题。

在讨论 F# 的细节之前，让我们尝试对开发人员必须处理的一些常见类型的并发场景进行分类：

- **“并发多任务处理”**。当我们直接控制多个并发任务（如进程或线程）时，我们希望它们能够相互通信并安全地共享数据。
- **“异步”编程**。这是我们与直接控制之外的单独系统发起对话，然后等待它返回给我们的时候。常见的情况是与文件系统、数据库或网络对话。这些情况通常是 I/O 受限的，所以你想在等待的时候做一些其他有用的事情。这些类型的任务通常也是非确定性的，这意味着运行同一程序两次可能会产生不同的结果。
- **“并行”编程**。这是当我们有一个任务，我们想将其拆分为独立的子任务，然后并行运行子任务时，最好使用所有可用的内核或 CPU。这些情况通常是 CPU 受限的。与异步任务不同，并行性通常是确定的，因此运行同一程序两次将得到相同的结果。
- **“反应式”编程**。这是我们自己不主动发起任务，而是专注于倾听事件，然后尽快处理的时候。这种情况发生在设计服务器和使用用户界面时。

当然，这些都是模糊的定义，在实践中是重叠的。不过，一般来说，对于所有这些情况，解决这些场景的实际实现往往使用两种不同的方法：

- 如果有许多不同的任务需要共享状态或资源而无需等待，那么请使用“缓冲异步”设计。
- 如果有很多相同的任务不需要共享状态，那么使用“fork/join”或“分而治之”方法使用并行任务。

### F#并发编程工具

F# 提供了许多不同的并发代码编写方法：

- 对于多任务处理和异步问题，F# 可以直接使用所有常用.NET 怀疑(suspects)的方法，如 `Thread` `AutoResetEvent`, `BackgroundWorker` 和 `IAsyncResult`。但它也为所有类型的异步 IO 和后台任务管理提供了一个更简单的模型，称为“异步工作流”。我们将在下一篇文章中介绍这些。
- 异步问题的另一种方法是使用消息队列和“参与者模型（actor model）”（这是上面提到的“缓冲异步”设计）。F# 有一个名为 `MailboxProcessor` 的 actor 模型的内置实现。我非常支持使用参与者和消息队列进行设计，因为它将各种组件解耦，并允许您连续思考每个组件。
- 为了实现真正的 CPU 并行性，F# 具有基于上述异步工作流构建的方便的库代码，它还可以使用 .NET 任务并行库。
- 最后，事件处理和响应式编程的函数式方法与传统方法截然不同。函数式方法将事件视为“流”，可以像 LINQ 处理集合一样进行过滤、拆分和组合。F# 内置了对该模型以及标准事件驱动模型的支持。

## 24 异步编程

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/concurrency-async-and-parallel/#series-toc)*)*

用Async类封装后台任务
24 Apr 2012这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/concurrency-async-and-parallel/

在这篇文章中，我们将看看用 F# 编写异步代码的几种方法，以及一个非常简短的并行性示例。

### 传统异步编程

如前一篇文章所述，F# 可以直接使用所有常用的 .NET 怀疑，如 `Thread` `AutoResetEvent`, `BackgroundWorker` 和 `IAsyncResult`。

让我们来看一个简单的例子，我们等待计时器事件发生：

```F#
open System

let userTimerWithCallback =
    // create an event to wait on
    let event = new System.Threading.AutoResetEvent(false)

    // create a timer and add an event handler that will signal the event
    let timer = new System.Timers.Timer(2000.0)
    timer.Elapsed.Add (fun _ -> event.Set() |> ignore )

    //start
    printfn "Waiting for timer at %O" DateTime.Now.TimeOfDay
    timer.Start()

    // keep working
    printfn "Doing something useful while waiting for event"

    // block on the timer via the AutoResetEvent
    event.WaitOne() |> ignore

    //done
    printfn "Timer ticked at %O" DateTime.Now.TimeOfDay
```

这显示了 `AutoResetEvent` 作为同步机制的使用。

- lambda 已在通过 `Timer.Elapsed` 事件注册，当事件触发时，会发出自动重置事件的信号。
- 主线程启动计时器，在等待时执行其他操作，然后阻塞，直到事件被触发。
- 最后，大约 2 秒后，主线程继续。

上面的代码相当简单，但确实需要您实例化一个 AutoResetEvent，如果 lambda 定义不正确，可能会出现错误。

### 引入异步工作流

F# 有一个名为“异步工作流”的内置构造，使异步代码更容易编写。这些工作流是封装后台任务的对象，并提供了许多有用的操作来管理它们。

以下是重写为使用一个的前一个示例：

```F#
open System
//open Microsoft.FSharp.Control  // Async.* is in this module.

let userTimerWithAsync =

    // create a timer and associated async event
    let timer = new System.Timers.Timer(2000.0)
    let timerEvent = Async.AwaitEvent (timer.Elapsed) |> Async.Ignore

    // start
    printfn "Waiting for timer at %O" DateTime.Now.TimeOfDay
    timer.Start()

    // keep working
    printfn "Doing something useful while waiting for event"

    // block on the timer event now by waiting for the async to complete
    Async.RunSynchronously timerEvent

    // done
    printfn "Timer ticked at %O" DateTime.Now.TimeOfDay
```

以下是变化：

- `AutoResetEvent` 和 lambda 已消失，并被 `let timerEvent = Control.Async.AwaitEvent (timer.Elapsed)` 替换。它直接从事件创建 `async` 对象，不需要 lambda。添加 `ignore` 以忽略结果。
- `event.WaitOne()` 已被 `Async.RunSynchronously timerEvent` 替换，它会阻塞异步对象，直到它完成。

就是这样。既简单又容易理解。

异步工作流还可以与 `IAsyncResult`，开始/结束对和其他标准一起使用 .NET 方法。

例如，您可以通过包装从 `BeginWrite` 生成的 `IAsyncResult` 来执行异步文件写入。

```F#
let fileWriteWithAsync =

    // create a stream to write to
    use stream = new System.IO.FileStream("test.txt",System.IO.FileMode.Create)

    // start
    printfn "Starting async write"
    let asyncResult = stream.BeginWrite(Array.empty,0,0,null,null)

	// create an async wrapper around an IAsyncResult
    let async = Async.AwaitIAsyncResult(asyncResult) |> Async.Ignore

    // keep working
    printfn "Doing something useful while waiting for write to complete"

    // block on the timer now by waiting for the async to complete
    Async.RunSynchronously async

    // done
    printfn "Async write completed"
```

### 创建和嵌套异步工作流

异步工作流也可以手动创建。使用 `async` 关键字和花括号创建了一个新的工作流。大括号包含一组要在后台执行的表达式。

这个简单的工作流只会休眠 2 秒。

```F#
let sleepWorkflow  = async{
    printfn "Starting sleep workflow at %O" DateTime.Now.TimeOfDay
    do! Async.Sleep 2000
    printfn "Finished sleep workflow at %O" DateTime.Now.TimeOfDay
    }

Async.RunSynchronously sleepWorkflow
```

*注意：代码 `do! Async.Sleep 2000` 类似于 `Thread.Sleep`，但设计用于异步工作流程。*

工作流可以包含嵌套在其中的其他异步工作流。在大括号内，可以使用 `let!` 语法来阻塞嵌套的工作流。

```F#
let nestedWorkflow  = async{

    printfn "Starting parent"
    let! childWorkflow = Async.StartChild sleepWorkflow

    // give the child a chance and then keep working
    do! Async.Sleep 100
    printfn "Doing something useful while waiting "

    // block on the child
    let! result = childWorkflow

    // done
    printfn "Finished parent"
    }

// run the whole workflow
Async.RunSynchronously nestedWorkflow
```

### 取消工作流

异步工作流的一个非常方便的地方是，它们支持内置的取消机制。不需要特殊代码。

考虑一个简单的任务，打印1到100的数字：

```F#
let testLoop = async {
    for i in [1..100] do
        // do something
        printf "%i before.." i

        // sleep a bit
        do! Async.Sleep 10
        printfn "..after"
    }
```

我们可以用通常的方式进行测试：

```F#
Async.RunSynchronously testLoop
```

现在，假设我们想在中途取消此任务。最好的方法是什么？

在 C# 中，我们必须创建标志来传递，然后经常检查它们，但在 F# 中，这种技术是内置的，使用 `CancellationToken` 类。

以下是我们如何取消任务的示例：

```F#
open System
open System.Threading

// create a cancellation source
use cancellationSource = new CancellationTokenSource()

// start the task, but this time pass in a cancellation token
Async.Start (testLoop,cancellationSource.Token)

// wait a bit
Thread.Sleep(200)

// cancel after 200ms
cancellationSource.Cancel()
```

在 F# 中，任何嵌套的异步调用都会自动检查取消令牌！

在这种情况下，这是一行：

```F#
do! Async.Sleep(10)
```

正如您从输出中看到的，此行是取消发生的地方。

### 串行和并行组合工作流

异步工作流的另一个有用之处是，它们可以很容易地以各种方式组合：串行和并行。

让我们再次创建一个只在给定时间内休眠的简单工作流：

```F#
// create a workflow to sleep for a time
let sleepWorkflowMs ms = async {
    printfn "%i ms workflow started" ms
    do! Async.Sleep ms
    printfn "%i ms workflow finished" ms
    }
```

这是一个将其中两个串联在一起的版本：

```F#
let workflowInSeries = async {
    let! sleep1 = sleepWorkflowMs 1000
    printfn "Finished one"
    let! sleep2 = sleepWorkflowMs 2000
    printfn "Finished two"
    }

#time
Async.RunSynchronously workflowInSeries
#time
```

这里有一个将这两个并行结合的版本：

```F#
// Create them
let sleep1 = sleepWorkflowMs 1000
let sleep2 = sleepWorkflowMs 2000

// run them in parallel
#time
[sleep1; sleep2]
    |> Async.Parallel
    |> Async.RunSynchronously
#time
```

> 注意：`#time` 命令可打开和关闭计时器。它仅在交互式窗口中工作，因此必须将此示例发送到交互式窗口才能正常工作。

我们使用 `#time` 选项来显示总运行时间，因为它们是并行运行的，所以总运行时间为 2 秒。如果他们以串联的方式进行，则需要 3 秒。

此外，您可能会看到输出有时会出现乱码，因为两个任务同时写入控制台！

最后一个示例是“fork/join”方法的经典示例，其中生成了许多子任务，然后父任务等待它们全部完成。正如你所看到的，F# 让这一切变得非常容易！

### 示例：异步web下载器

在这个更现实的例子中，我们将看到将一些现有代码从非异步风格转换为异步风格是多么容易，以及可以实现的相应性能提升。

所以这里有一个简单的 URL 下载器，与我们在系列文章开头看到的非常相似：

```F#
open System.Net
open System
open System.IO

let fetchUrl url =
    let req = WebRequest.Create(Uri(url))
    use resp = req.GetResponse()
    use stream = resp.GetResponseStream()
    use reader = new IO.StreamReader(stream)
    let html = reader.ReadToEnd()
    printfn "finished downloading %s" url
```

这里有一些代码来计时：

```F#
// a list of sites to fetch
let sites = ["http://www.bing.com";
             "http://www.google.com";
             "http://www.microsoft.com";
             "http://www.amazon.com";
             "http://www.yahoo.com"]

#time                     // turn interactive timer on
sites                     // start with the list of sites
|> List.map fetchUrl      // loop through each site and download
#time                     // turn timer off
```

记下所花费的时间，让我们看看是否可以改进它！

显然，上面的例子效率低下——一次只访问一个网站。如果我们能同时访问它们，程序会更快。

那么，我们如何将其转换为并发算法呢？逻辑大致如下：

- 为我们正在下载的每个网页创建一个任务，然后对于每个任务，下载逻辑如下：
  - 开始从网站下载页面。在这种情况下，暂停一下，让其他任务轮流进行。
  - 下载完成后，唤醒并继续执行任务
- 最后，启动所有任务，让他们去做！

不幸的是，在标准的类 C 语言中很难做到这一点。例如，在 C# 中，您必须为异步任务完成时创建一个回调。管理这些回调是痛苦的，并且会创建大量额外的支持代码，从而妨碍理解逻辑。对此有一些优雅的解决方案，但总的来说，C# 并发编程的信噪比非常高*。

*截至撰写本文时。C# 的未来版本将有 `await` 关键字，这与 F# 现在的关键字类似。

但正如你所料，F# 让这变得容易。以下是下载器代码的并发 F# 版本：

```F#
open Microsoft.FSharp.Control.CommonExtensions
                                        // adds AsyncGetResponse

// Fetch the contents of a web page asynchronously
let fetchUrlAsync url =
    async {
        let req = WebRequest.Create(Uri(url))
        use! resp = req.AsyncGetResponse()  // new keyword "use!"
        use stream = resp.GetResponseStream()
        use reader = new IO.StreamReader(stream)
        let html = reader.ReadToEnd()
        printfn "finished downloading %s" url
        }
```

请注意，新代码看起来几乎与原始代码完全相同。只有一些小的变化。

- 从“`use resp = `”到“`use! resp =`“的更改正是我们上面讨论的更改——在异步操作进行的同时，让其他任务轮流执行。
- 我们还使用了 `CommonExtensions` 命名空间中定义的 `AsyncGetResponse` 扩展方法。这将返回一个异步工作流，我们可以将其嵌套在主工作流中。
- 此外，整个步骤集都包含在“`async {…}`”包装器中，该包装器将其转换为可以异步运行的块。

这是一个使用异步版本的定时下载。

```F#
// a list of sites to fetch
let sites = ["http://www.bing.com";
             "http://www.google.com";
             "http://www.microsoft.com";
             "http://www.amazon.com";
             "http://www.yahoo.com"]

#time                      // turn interactive timer on
sites
|> List.map fetchUrlAsync  // make a list of async tasks
|> Async.Parallel          // set up the tasks to run in parallel
|> Async.RunSynchronously  // start them off
#time                      // turn timer off
```

其工作原理如下：

- `fetchUrlAsync` 应用于每个站点。它不会立即开始下载，但会返回一个异步工作流供以后运行。
- 为了设置所有任务同时运行，我们使用 `Async.Parallel` 函数
- 最后我们调用 `Async.RunSynchronously` 以启动所有任务，并等待它们全部停止。

如果你自己尝试这段代码，你会发现异步版本比同步版本快得多。对于一些小的代码更改来说还不错！最重要的是，底层逻辑仍然非常清晰，没有被噪音所干扰。

### 示例：并行计算

最后，让我们再次快速浏览一下并行计算。

在我们开始之前，我应该警告你，下面的示例代码只是为了演示基本原理。像这样的并行化“玩具”版本的基准测试没有意义，因为任何一种真正的并发代码都有如此多的依赖关系。

还要注意，并行化很少是加速代码的最佳方式。你的时间几乎总是最好花在改进算法上。我随时都会用我的串行版本的 quicksort 和你的并行版本的 bubblesort 打赌！（有关如何提高性能的更多详细信息，请参阅优化系列）

不管怎样，有了这个警告，让我们创建一个消耗一些 CPU 的小任务。我们将对其进行串行和并行测试。

```F#
let childTask() =
    // chew up some CPU.
    for i in [1..1000] do
        for i in [1..1000] do
            do "Hello".Contains("H") |> ignore
            // we don't care about the answer!

// Test the child task on its own.
// Adjust the upper bounds as needed
// to make this run in about 0.2 sec
#time
childTask()
#time
```

根据需要调整循环的上限，使其在大约 0.2 秒内运行。

现在，让我们将这些组合成一个串行任务（使用组合），并用计时器进行测试：

```F#
let parentTask =
    childTask
    |> List.replicate 20
    |> List.reduce (>>)

//test
#time
parentTask()
#time
```

这大约需要 4 秒。

现在，为了使 `childTask` 可并行化，我们必须将其封装在 `async` 中：

```F#
let asyncChildTask = async { return childTask() }
```

为了将一堆异步组合成一个并行任务，我们使用 `Async.Parallel`。

让我们测试一下并比较一下时间：

```F#
let asyncParentTask =
    asyncChildTask
    |> List.replicate 20
    |> Async.Parallel

//test
#time
asyncParentTask
|> Async.RunSynchronously
#time
```

在双核机器上，并行版本的速度大约快 50%。当然，它会随着内核或 CPU 数量的增加而变得更快，但速度会降低。四个核心会比一个核心快，但不会快四倍。

另一方面，与异步 web 下载示例一样，一些微小的代码更改可以产生很大的影响，同时仍然使代码易于阅读和理解。因此，在并行性真正有帮助的情况下，很高兴知道它很容易安排。

## 25 消息和代理

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/concurrency-actor-model/#series-toc)*)*

更容易思考并发性
25 Apr 2012 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/concurrency-actor-model/

在这篇文章中，我们将研究基于消息（或基于参与者，actor-based）的并发方法。

在这种方法中，当一个任务想要与另一个任务通信时，它会向其发送消息，而不是直接联系它。消息被放入队列，接收任务（称为“参与者（actor）”或“代理（agent）”）一次从队列中提取一条消息进行处理。

这种基于消息的方法已应用于许多情况，从低级网络套接字（基于 TCP/IP 构建）到企业范围的应用程序集成系统（例如 RabbitMQ 或 IBM WebSphere MQ）。

从软件设计的角度来看，基于消息的方法有很多好处：

- 您可以在没有锁的情况下管理共享数据和资源。
- 你可以很容易地遵循“单一责任原则”，因为每个代理都可以被设计为只做一件事。
- 它鼓励一种“管道”编程模型，其中“生产者”向解耦的“消费者”发送消息，这还有其他好处：
  - 队列充当缓冲区，消除了客户端的等待。
  - 根据需要扩大队列的一侧或另一侧以最大限度地提高吞吐量是很简单的。
  - 可以优雅地处理错误，因为解耦意味着可以在不影响客户端的情况下创建和销毁代理。

从实际开发人员的角度来看，我发现基于消息的方法最吸引人的地方是，在为任何给定的参与者编写代码时，你不必考虑并发性来伤害你的大脑。消息队列强制对可能同时发生的操作进行“序列化”。这反过来又使思考（并编写代码）处理消息的逻辑变得更加容易，因为您可以确信您的代码将与可能中断您的流的其他事件隔离开来。

有了这些优势，当爱立信内部的一个团队想要设计一种用于编写高度并发电话应用程序的编程语言时，他们使用基于消息的方法创建了一种语言，即 Erlang，这并不奇怪。Erlang 现在已经成为整个主题的典型代表，并引起了人们对在其他语言中实现相同方法的极大兴趣。

### F# 如何实现基于消息的方法

F# 有一个名为 `MailboxProcessor` 的内置代理类。与线程相比，这些代理非常轻量级——您可以同时实例化数万个代理。

这些类似于 Erlang 中的代理，但与 Erlang 不同，它们不跨进程边界工作，只在同一进程中工作。与 RabbitMQ 等重量级排队系统不同，消息不是持久的。如果你的应用程序崩溃，消息就会丢失。

但这些都是小问题，可以解决。在未来的系列文章中，我将介绍消息队列的替代实现。基本方法在所有情况下都是一样的。

让我们看看 F# 中的一个简单代理实现：

```F#
let printerAgent = MailboxProcessor.Start(fun inbox->

    // the message processing function
    let rec messageLoop() = async{

        // read a message
        let! msg = inbox.Receive()

        // process a message
        printfn "message is: %s" msg

        // loop to top
        return! messageLoop()
        }

    // start the loop
    messageLoop()
    )
```

`MailboxProcessor.Start` 函数接受一个简单的函数参数。该函数永远循环，从队列（或“收件箱”）读取消息并对其进行处理。

以下是使用中的示例：

```F#
// test it
printerAgent.Post "hello"
printerAgent.Post "hello again"
printerAgent.Post "hello a third time"
```

在本文的其余部分，我们将看看两个稍微有用的例子：

- 无锁管理共享状态
- 对共享 IO 的序列化和缓冲访问

在这两种情况下，基于消息的并发方法都是优雅、高效且易于编程的。

### 管理共享状态

让我们先看看共享状态问题。

一种常见的情况是，您有一些状态需要由多个并发任务或线程访问和更改。我们将使用一个非常简单的案例，并说明要求如下：

- 一个共享的“计数器”和“总和”，可以由多个任务同时递增。
- 计数器和总和的更改必须是原子性的——我们必须保证它们将同时更新。

#### 共享状态的锁定方法

对于这些需求，使用锁或互斥是一种常见的解决方案，所以让我们使用锁编写一些代码，看看它是如何执行的。

首先，让我们编写一个静态的 `LockedCounter` 类，用锁保护状态。

```F#
open System
open System.Threading
open System.Diagnostics

// a utility function
type Utility() =
    static let rand = Random()

    static member RandomSleep() =
        let ms = rand.Next(1,10)
        Thread.Sleep ms

// an implementation of a shared counter using locks
type LockedCounter () =

    static let _lock = Object()

    static let mutable count = 0
    static let mutable sum = 0

    static let updateState i =
        // increment the counters and...
        sum <- sum + i
        count <- count + 1
        printfn "Count is: %i. Sum is: %i" count sum

        // ...emulate a short delay
        Utility.RandomSleep()


    // public interface to hide the state
    static member Add i =
        // see how long a client has to wait
        let stopwatch = Stopwatch()
        stopwatch.Start()

        // start lock. Same as C# lock{...}
        lock _lock (fun () ->

            // see how long the wait was
            stopwatch.Stop()
            printfn "Client waited %i" stopwatch.ElapsedMilliseconds

            // do the core logic
            updateState i
            )
        // release lock
```

关于此代码的一些注意事项：

- 这段代码是使用一种非常命令式的方法编写的，其中包含可变变量和锁
- 公共 `Add` 方法具有显式 `Monitor.Enter` 和 `Monitor.Exit` 表达式以获取和释放锁。这与 C# 中的 `lock{…}` 语句相同。
- 我们还添加了一个秒表来测量客户需要等待多长时间才能拿到锁。
- 核心的“业务逻辑”是 `updateState` 方法，它不仅更新状态，还添加了一个小的随机等待，以模拟执行处理所需的时间。

让我们单独测试一下：

```F#
// test in isolation
LockedCounter.Add 4
LockedCounter.Add 5
```

接下来，我们将创建一个尝试访问计数器的任务：

```F#
let makeCountingTask addFunction taskId  = async {
    let name = sprintf "Task%i" taskId
    for i in [1..3] do
        addFunction i
    }

// test in isolation
let task = makeCountingTask LockedCounter.Add 1
Async.RunSynchronously task
```

在这种情况下，当根本没有争用时，等待时间都是 0。

但是，当我们创建 10 个子任务，所有子任务都试图同时访问计数器时会发生什么：

```F#
let lockedExample5 =
    [1..10]
        |> List.map (fun i -> makeCountingTask LockedCounter.Add i)
        |> Async.Parallel
        |> Async.RunSynchronously
        |> ignore
```

哦，天哪！大多数任务现在都要等一段时间。如果两个任务想同时更新状态，一个必须等待另一个的工作完成，然后才能完成自己的工作，这会影响性能。

如果我们添加越来越多的任务，争用就会增加，任务将花费越来越多的时间等待而不是工作。

#### 基于消息的共享状态方法

让我们看看消息队列如何帮助我们。这是基于消息的版本：

```F#
type MessageBasedCounter () =

    static let updateState (count,sum) msg =

        // increment the counters and...
        let newSum = sum + msg
        let newCount = count + 1
        printfn "Count is: %i. Sum is: %i" newCount newSum

        // ...emulate a short delay
        Utility.RandomSleep()

        // return the new state
        (newCount,newSum)

    // create the agent
    static let agent = MailboxProcessor.Start(fun inbox ->

        // the message processing function
        let rec messageLoop oldState = async{

            // read a message
            let! msg = inbox.Receive()

            // do the core logic
            let newState = updateState oldState msg

            // loop to top
            return! messageLoop newState
            }

        // start the loop
        messageLoop (0,0)
        )

    // public interface to hide the implementation
    static member Add i = agent.Post i
```

关于此代码的一些注意事项：

- 核心“业务逻辑”再次位于 `updateState` 方法中，该方法的实现与前面的示例几乎相同，除了状态是不可变的，因此创建了一个新的状态并将其返回给主循环。
- 代理读取消息（在这种情况下是简单的整数），然后调用 `updateState` 方法
- 公共方法 `Add` 向代理发布消息，而不是直接调用 `updateState` 方法
- 这段代码是以更实用的方式编写的；任何地方都没有可变变量和锁。事实上，根本没有处理并发性的代码！代码只需要关注业务逻辑，因此更容易理解。

让我们单独测试一下：

```F#
// test in isolation
MessageBasedCounter.Add 4
MessageBasedCounter.Add 5
```

接下来，我们将重用前面定义的任务，但改为调用 `MessageBasedCounter.Add`：

```F#
let task = makeCountingTask MessageBasedCounter.Add 1
Async.RunSynchronously task
```

最后，让我们创建5个子任务，尝试一次访问计数器。

```F#
let messageExample5 =
    [1..5]
        |> List.map (fun i -> makeCountingTask MessageBasedCounter.Add i)
        |> Async.Parallel
        |> Async.RunSynchronously
        |> ignore
```

我们无法测量客户的等待时间，因为没有！

### 共享 IO

访问共享 IO 资源（如文件）时也会出现类似的并发问题：

- 如果IO速度慢，即使没有锁，客户端也会花费大量时间等待。
- 如果多个线程同时写入资源，则可能会得到损坏的数据。

这两个问题都可以通过使用异步调用结合缓冲来解决——这正是消息队列所做的。

在下一个示例中，我们将考虑许多客户端将同时写入的日志服务的示例。（在这个简单的情况下，我们将直接写入Console。）

我们将首先看一个没有并发控制的实现，然后看一个使用消息队列序列化所有请求的实现。

#### IO 无序列化

为了使损坏非常明显和可重复，让我们首先创建一个“慢速”控制台，在日志消息中写入每个单独的字符，并在每个字符之间暂停一毫秒。在这毫秒内，另一个线程也可能正在写入，导致消息的不必要交织。

```F#
let slowConsoleWrite msg =
    msg |> String.iter (fun ch->
        System.Threading.Thread.Sleep(1)
        System.Console.Write ch
        )

// test in isolation
slowConsoleWrite "abc"
```

接下来，我们将创建一个循环几次的简单任务，每次将其名称写入记录器：

```F#
let makeTask logger taskId = async {
    let name = sprintf "Task%i" taskId
    for i in [1..3] do
        let msg = sprintf "-%s:Loop%i-" name i
        logger msg
    }

// test in isolation
let task = makeTask slowConsoleWrite 1
Async.RunSynchronously task
```

接下来，我们编写一个日志类，封装对慢速控制台的访问。它没有锁定或序列化，基本上不是线程安全的：

```F#
type UnserializedLogger() =
    // interface
    member this.Log msg = slowConsoleWrite msg

// test in isolation
let unserializedLogger = UnserializedLogger()
unserializedLogger.Log "hello"
```

现在让我们把所有这些结合成一个真实的例子。我们将创建五个子任务并并行运行它们，所有子任务都试图写入慢速控制台。

```F#
let unserializedExample =
    let logger = UnserializedLogger()
    [1..5]
        |> List.map (fun i -> makeTask logger.Log i)
        |> Async.Parallel
        |> Async.RunSynchronously
        |> ignore
```

哎哟！输出非常混乱！

#### 带有消息的序列化 IO

那么，当我们用封装消息队列的 `SerializedLogger` 类替换 `UnserializedLogger` 时会发生什么。

`SerializedLogger` 中的代理只是从其输入队列中读取消息并将其写入慢速控制台。同样，没有处理并发性的代码，也没有使用锁。

```F#
type SerializedLogger() =

    // create the mailbox processor
    let agent = MailboxProcessor.Start(fun inbox ->

        // the message processing function
        let rec messageLoop () = async{

            // read a message
            let! msg = inbox.Receive()

            // write it to the log
            slowConsoleWrite msg

            // loop to top
            return! messageLoop ()
            }

        // start the loop
        messageLoop ()
        )

    // public interface
    member this.Log msg = agent.Post msg

// test in isolation
let serializedLogger = SerializedLogger()
serializedLogger.Log "hello"
```

因此，现在我们可以重复前面的未序列化示例，但改用 `SerializedLogger`。同样，我们创建了五个子任务并并行运行它们：

```F#
let serializedExample =
    let logger = SerializedLogger()
    [1..5]
        |> List.map (fun i -> makeTask logger.Log i)
        |> Async.Parallel
        |> Async.RunSynchronously
        |> ignore
```

真是大不一样！这一次的输出是完美的。

### 摘要

关于这种基于消息的方法还有很多要说的。在未来的系列中，我希望深入探讨更多细节，包括讨论以下主题：

- 使用 RabbitMQ 和 TPL Dataflow 的消息队列的替代实现。
- 取消和带外消息。
- 错误处理和重试，以及一般的异常处理。
- 如何通过创建或删除子代理来扩大和缩小规模。
- 避免缓冲区溢出并检测饥饿或不活动。

## 26 函数式反应式编程

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/concurrency-reactive/#series-toc)*)*

将事件转化为流
26 Apr 2012 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/concurrency-reactive/

事件无处不在。几乎每个程序都必须处理事件，无论是用户界面中的按钮点击、服务器中的套接字监听，甚至是系统关闭通知。

事件是最常见的 OO 设计模式之一的基础：“观察者”模式。

但正如我们所知，事件处理，就像一般的并发一样，实现起来可能很棘手。简单的事件逻辑很简单，但像“如果连续发生两个事件，做点什么，但如果只有一个事件发生，做点不同的事情”或“如果两个事件大致同时发生，做些什么”这样的逻辑呢。以其他更复杂的方式组合这些要求有多容易？

即使你能成功实现这些要求，即使有最好的意图，代码也往往是意大利面条般的，很难理解。

是否有一种方法可以使事件处理更容易？

我们在上一篇关于消息队列的文章中看到，这种方法的优点之一是请求被“序列化”，使其在概念上更容易处理。

有一种类似的方法可以用于事件。其想法是将一系列事件转化为“事件流”。然后，事件流变得非常像IEnumerables，因此下一步显然是以与 LINQ 处理集合大致相同的方式处理它们，以便对它们进行过滤、映射、拆分和组合。

F# 内置了对该模型以及更传统方法的支持。

### 一个简单的事件流

让我们从一个简单的例子开始比较这两种方法。我们将首先实现经典的事件处理程序方法。

首先，我们定义一个效用函数，它将：

- 创建计时器
- 为 `Elapsed` 事件注册一个处理程序
- 运行计时器五秒钟，然后停止

代码如下：

```F#
open System
open System.Threading

/// create a timer and register an event handler,
/// then run the timer for five seconds
let createTimer timerInterval eventHandler =
    // setup a timer
    let timer = new System.Timers.Timer(float timerInterval)
    timer.AutoReset <- true

    // add an event handler
    timer.Elapsed.Add eventHandler

    // return an async task
    async {
        // start timer...
        timer.Start()
        // ...run for five seconds...
        do! Async.Sleep 5000
        // ... and stop
        timer.Stop()
        }
```

现在以交互方式测试它：

```F#
// create a handler. The event args are ignored
let basicHandler _ = printfn "tick %A" DateTime.Now

// register the handler
let basicTimer1 = createTimer 1000 basicHandler

// run the task now
Async.RunSynchronously basicTimer1
```

现在，让我们创建一个类似的实用方法来创建计时器，但这次它也将返回一个“可观测”，即事件流。

```F#
let createTimerAndObservable timerInterval =
    // setup a timer
    let timer = new System.Timers.Timer(float timerInterval)
    timer.AutoReset <- true

    // events are automatically IObservable
    let observable = timer.Elapsed

    // return an async task
    let task = async {
        timer.Start()
        do! Async.Sleep 5000
        timer.Stop()
        }

    // return a async task and the observable
    (task,observable)
```

再次以交互方式进行测试：

```F#
// create the timer and the corresponding observable
let basicTimer2 , timerEventStream = createTimerAndObservable 1000

// register that every time something happens on the
// event stream, print the time.
timerEventStream
|> Observable.subscribe (fun _ -> printfn "tick %A" DateTime.Now)

// run the task now
Async.RunSynchronously basicTimer2
```

不同之处在于，我们不是直接向事件注册处理程序，而是“订阅”事件流。微妙的不同，而且很重要。

### 统计事件

在下一个例子中，我们将有一个稍微复杂一些的要求：

`创建一个每500毫秒滴答一次的计时器。`
`在每个刻度处，打印到目前为止的刻度数和当前时间。`

为了以经典的命令式方式实现这一点，我们可能会创建一个具有可变计数器的类，如下所示：

```F#
type ImperativeTimerCount() =

    let mutable count = 0

    // the event handler. The event args are ignored
    member this.handleEvent _ =
      count <- count + 1
      printfn "timer ticked with count %i" count
```

我们可以重用我们之前创建的实用函数来测试它：

```F#
// create a handler class
let handler = new ImperativeTimerCount()

// register the handler method
let timerCount1 = createTimer 500 handler.handleEvent

// run the task now
Async.RunSynchronously timerCount1
```

让我们看看如何以一种函数式的方式做同样的事情：

```F#
// create the timer and the corresponding observable
let timerCount2, timerEventStream = createTimerAndObservable 500

// set up the transformations on the event stream
timerEventStream
|> Observable.scan (fun count _ -> count + 1) 0
|> Observable.subscribe (fun count -> printfn "timer ticked with count %i" count)

// run the task now
Async.RunSynchronously timerCount2
```

在这里，我们看到了如何构建事件转换层，就像在 LINQ 中使用列表转换一样。

第一个转换是 `scan`，它为每个事件累积状态。它大致相当于我们看到的用于列表的 `List.fold` 函数。在这种情况下，累积状态只是一个计数器。

然后，对于每个事件，计数都会打印出来。

请注意，在这种函数式方法中，我们没有任何可变状态，也不需要创建任何特殊的类。

### 合并多个事件流

对于最后一个示例，我们将着眼于合并多个事件流。

让我们根据众所周知的“FizzBuzz”问题提出一个要求：

`创建两个计时器，分别称为“3”和“5”。“3”定时器每300ms滴答一次，“5”定时器每500毫秒滴答一次。`

`按如下方式处理事件：`
`a） 对于所有事件，打印时间和时间的id`
`b） 当一个刻度与前一个刻度同时出现时，打印“FizzBuzz”`
`否则：`
`c） 当“3”定时器自行计时时，打印“Fizz”`
`d） 当“5”定时器自行计时时，打印“Buzz”`

首先，让我们创建一些两种实现都可以使用的代码。

我们需要一个通用的事件类型来捕获计时器id和滴答声的时间。

```F#
type FizzBuzzEvent = {label:int; time: DateTime}
```

然后我们需要一个效用函数来查看两个事件是否同时发生。我们将慷慨地允许高达 50 毫秒的时差。

```F#
let areSimultaneous (earlierEvent,laterEvent) =
    let {label=_;time=t1} = earlierEvent
    let {label=_;time=t2} = laterEvent
    t2.Subtract(t1).Milliseconds < 50
```

在命令式设计中，我们需要跟踪之前的事件，以便进行比较。当前一个事件不存在时，我们第一次需要特殊的案例代码

```F#
type ImperativeFizzBuzzHandler() =

    let mutable previousEvent: FizzBuzzEvent option = None

    let printEvent thisEvent  =
      let {label=id; time=t} = thisEvent
      printf "[%i] %i.%03i " id t.Second t.Millisecond
      let simultaneous = previousEvent.IsSome && areSimultaneous (previousEvent.Value,thisEvent)
      if simultaneous then printfn "FizzBuzz"
      elif id = 3 then printfn "Fizz"
      elif id = 5 then printfn "Buzz"

    member this.handleEvent3 eventArgs =
      let event = {label=3; time=DateTime.Now}
      printEvent event
      previousEvent <- Some event

    member this.handleEvent5 eventArgs =
      let event = {label=5; time=DateTime.Now}
      printEvent event
      previousEvent <- Some event
```

现在代码开始变丑了！我们已经有了可变状态、复杂的条件逻辑和特殊情况，仅仅是为了满足这样一个简单的需求。

让我们来测试一下：

```F#
// create the class
let handler = new ImperativeFizzBuzzHandler()

// create the two timers and register the two handlers
let timer3 = createTimer 300 handler.handleEvent3
let timer5 = createTimer 500 handler.handleEvent5

// run the two timers at the same time
[timer3;timer5]
|> Async.Parallel
|> Async.RunSynchronously
```

它确实有效，但你确定代码没有错误吗？如果你换了东西，你可能会不小心把它弄坏吗？

这个命令式代码的问题是，它有很多噪音，掩盖了需求。

函数式版本能做得更好吗？让我们看看！

首先，我们创建两个事件流，每个计时器一个：

```F#
let timer3, timerEventStream3 = createTimerAndObservable 300
let timer5, timerEventStream5 = createTimerAndObservable 500
```

接下来，我们将“原始”事件流中的每个事件转换为FizzBuzz事件类型：

```F#
// convert the time events into FizzBuzz events with the appropriate id
let eventStream3  =
   timerEventStream3
   |> Observable.map (fun _ -> {label=3; time=DateTime.Now})

let eventStream5  =
   timerEventStream5
   |> Observable.map (fun _ -> {label=5; time=DateTime.Now})
```

现在，为了确定两个事件是否同时发生，我们需要以某种方式比较两个不同流中的事件。

这实际上比听起来更容易，因为我们可以：

- 将这两个流合并为一个流：
- 然后创建成对的连续事件
- 然后测试这些对，看看它们是否是同时的
- 然后根据该测试将输入流拆分为两个新的输出流

以下是执行此操作的实际代码：

```F#
// combine the two streams
let combinedStream =
    Observable.merge eventStream3 eventStream5

// make pairs of events
let pairwiseStream =
   combinedStream |> Observable.pairwise

// split the stream based on whether the pairs are simultaneous
let simultaneousStream, nonSimultaneousStream =
    pairwiseStream |> Observable.partition areSimultaneous
```

最后，我们可以根据事件 id 再次拆分 `nonSimultaneousStream`：

```F#
// split the non-simultaneous stream based on the id
let fizzStream, buzzStream  =
    nonSimultaneousStream
    // convert pair of events to the first event
    |> Observable.map (fun (ev1,_) -> ev1)
    // split on whether the event id is three
    |> Observable.partition (fun {label=id} -> id=3)
```

让我们回顾一下到目前为止。我们从两个原始事件流开始，并从中创建了四个新的事件流：

- `combinedStream` 包含所有事件
- `simultaneousStream` 只包含同时发生的事件
- `fizzStream` 仅包含id为3的非同步事件
- `buzzStream` 仅包含id为5的非同步事件

现在我们需要做的就是将行为附加到每个流上：

```F#
//print events from the combinedStream
combinedStream
|> Observable.subscribe (fun {label=id;time=t} ->
                              printf "[%i] %i.%03i " id t.Second t.Millisecond)

//print events from the simultaneous stream
simultaneousStream
|> Observable.subscribe (fun _ -> printfn "FizzBuzz")

//print events from the nonSimultaneous streams
fizzStream
|> Observable.subscribe (fun _ -> printfn "Fizz")

buzzStream
|> Observable.subscribe (fun _ -> printfn "Buzz")
```

让我们来测试一下：

```F#
// run the two timers at the same time
[timer3;timer5]
|> Async.Parallel
|> Async.RunSynchronously
```

以下是一套完整的代码：

```F#
// create the event streams and raw observables
let timer3, timerEventStream3 = createTimerAndObservable 300
let timer5, timerEventStream5 = createTimerAndObservable 500

// convert the time events into FizzBuzz events with the appropriate id
let eventStream3  = timerEventStream3
                    |> Observable.map (fun _ -> {label=3; time=DateTime.Now})
let eventStream5  = timerEventStream5
                    |> Observable.map (fun _ -> {label=5; time=DateTime.Now})

// combine the two streams
let combinedStream =
   Observable.merge eventStream3 eventStream5

// make pairs of events
let pairwiseStream =
   combinedStream |> Observable.pairwise

// split the stream based on whether the pairs are simultaneous
let simultaneousStream, nonSimultaneousStream =
   pairwiseStream |> Observable.partition areSimultaneous

// split the non-simultaneous stream based on the id
let fizzStream, buzzStream  =
    nonSimultaneousStream
    // convert pair of events to the first event
    |> Observable.map (fun (ev1,_) -> ev1)
    // split on whether the event id is three
    |> Observable.partition (fun {label=id} -> id=3)

//print events from the combinedStream
combinedStream
|> Observable.subscribe (fun {label=id;time=t} ->
                              printf "[%i] %i.%03i " id t.Second t.Millisecond)

//print events from the simultaneous stream
simultaneousStream
|> Observable.subscribe (fun _ -> printfn "FizzBuzz")

//print events from the nonSimultaneous streams
fizzStream
|> Observable.subscribe (fun _ -> printfn "Fizz")

buzzStream
|> Observable.subscribe (fun _ -> printfn "Buzz")

// run the two timers at the same time
[timer3;timer5]
|> Async.Parallel
|> Async.RunSynchronously
```

代码可能看起来有点冗长，但这种渐进式、循序渐进的方法非常清晰，并且具有自文档性。

这种风格的一些好处是：

- 只需查看它，甚至不运行它，我就可以看到它符合要求。命令式版本则不然。

- 从设计的角度来看，每个最终的“输出”流都遵循单一责任原则——它只做一件事——所以很容易将行为与它联系起来。

- 这段代码没有条件句，没有可变状态，没有边缘情况。我希望它很容易维护或更改。

- 它易于调试。例如，我可以很容易地“点击”simultaneousStream的输出，看看它是否包含我认为它包含的内容：

  ```F#
  // debugging code
  //simultaneousStream |> Observable.subscribe (fun e -> printfn "sim %A" e)
  //nonSimultaneousStream |> Observable.subscribe (fun e -> printfn "non-sim %A" e)
  ```

在命令式版本中，这会困难得多。

### 摘要

函数式响应式编程（称为 FRP）是一个大话题，我们在这里才刚刚谈到它。我希望这篇介绍能让你一窥这种做事方式的有用性。

如果您想了解更多信息，请参阅 F# Observable 模块的文档，该模块具有上面使用的基本转换。还有作为 .NET 4 的一部分提供的响应式扩展（Rx）库。这包含了许多其他的转换。



## 27 完整性

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/completeness-intro/#series-toc)*)*

F# 是 .NET 生态系统整体的一部分
27 Apr 2012 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/completeness-intro/

在这最后一组帖子中，我们将在“完整性”的主题下探讨F#的其他方面。

来自学术界的编程语言往往更注重优雅和纯粹，而不是现实世界的实用性，而 C# 和 Java 等更主流的商业语言之所以受到重视，正是因为它们是实用的；它们可以在各种情况下工作，并拥有广泛的工具和库来满足几乎所有的需求。换句话说，为了在企业中发挥作用，一门语言需要是完整的，而不仅仅是设计良好的。

F# 的不同寻常之处在于它成功地连接了两个世界。尽管到目前为止，所有的例子都集中在 F# 作为一种优雅的函数式语言上，但它也支持面向对象的范式，并且可以很容易地与其他 .NET 语言和工具集成。因此，F# 不是一个孤岛，而是从 .NET 生态系统整体中受益。

让 F# “完整”的其他方面是成为一名官方 .NET 语言（及其所需的所有支持和文档），旨在在 Visual Studio（提供具有 IntelliSense 支持的优秀编辑器、调试器等）和 Visual Studio Code 中工作。这些好处应该是显而易见的，这里不会讨论。

因此，在最后一节中，我们将重点介绍两个特定领域：

- **与 .NET 库无缝互操作**。显然，F# 的函数式方法和设计到基础库中的命令式方法之间可能存在不匹配。我们将看看 F# 的一些特性，这些特性使这种集成更容易。
- **完全支持类和其他 C# 风格的代码**。F# 被设计为一种混合函数式/OO 语言，因此它几乎可以做 C# 能做的一切。我们将快速浏览这些其他功能的语法。

## 28 与 .NET 库无缝互操作

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/completeness-seamless-dotnet-interop/#series-toc)*)*

一些便于使用 .NET 库的功能
28 Apr 2012 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/completeness-seamless-dotnet-interop/

我们已经看到了许多使用 .NET库 F# 的例子，例如使用 `System.Net.WebRequest` 和 `System.Text.RegularExpressions`。而且整合确实是无缝的。

对于更复杂的需求，F# 本机支持 .NET 类、接口和结构，因此互操作仍然非常简单。例如，你可以用 C# 编写一个 `ISomething` 接口，并用 F# 实现。

但 F# 不仅可以调用现有的 .NET 代码，它也可以公开几乎任何 .NET API 返回到其他语言。例如，你可以用 F# 编写类和方法，并将其暴露给 C#、VB 或 COM。你甚至可以反向执行上述示例——在 F# 中定义一个 `ISomething` 接口，并用 C# 完成实现！所有这些的好处是，您不必丢弃任何现有的代码库；你可以开始在某些事情上使用 F#，而在其他事情上保留 C# 或 VB，并为这项工作选择最好的工具。

除了紧密的集成之外，F# 中还有许多很好的功能，通常可以使用 .NET 库在某些方面比 C# 更方便。以下是我最喜欢的一些：

- 您可以使用 TryParse 和 TryGetValue，而无需传递“out”参数。
- 您可以通过使用参数名来解决方法重载问题，这也有助于类型推理。
- 您可以使用“活动模式”进行转换。NET API转化为更友好的代码。
- 您可以从IDisposable等接口动态创建对象，而无需创建具体类。
- 您可以将“纯” F# 对象与现有 .NET API 混合和匹配

### TryParse 和 TryGetValue

值和字典的 `TryParse` 和 `TryGetValue` 函数经常用于避免额外的异常处理。但是 C# 语法有点笨拙。从 F# 使用它们更优雅，因为 F# 会自动将函数转换为元组，其中第一个元素是函数返回值，第二个元素是“out”参数。

```F#
//using an Int32
let (i1success,i1) = System.Int32.TryParse("123");
if i1success then printfn "parsed as %i" i1 else printfn "parse failed"

let (i2success,i2) = System.Int32.TryParse("hello");
if i2success then printfn "parsed as %i" i2 else printfn "parse failed"

//using a DateTime
let (d1success,d1) = System.DateTime.TryParse("1/1/1980");
let (d2success,d2) = System.DateTime.TryParse("hello");

//using a dictionary
let dict = new System.Collections.Generic.Dictionary<string,string>();
dict.Add("a","hello")
let (e1success,e1) = dict.TryGetValue("a");
let (e2success,e2) = dict.TryGetValue("b");
```

### 命名参数以帮助类型推理

在C#（以及一般的 .NET）中，您可以重载具有许多不同参数的方法。F# 可能会遇到这个问题。例如，以下是创建 StreamReader 的尝试：

```F#
let createReader fileName = new System.IO.StreamReader(fileName)
// error FS0041: A unique overload for method 'StreamReader'
//               could not be determined
```

问题是 F# 不知道参数应该是字符串还是流。您可以显式指定参数的类型，但这不是 F# 的方式！

相反，一个很好的解决方法是通过在 F# 中调用 .NET库中的方法时，您可以指定命名参数的方式启用。

```F#
let createReader2 fileName = new System.IO.StreamReader(path=fileName)
```

在许多情况下，如上所述，仅使用参数名称就足以解决类型问题。使用明确的参数名称通常有助于使代码更易读。

### .NET 函数的活动模式

在许多情况下，您都希望使用 .NET 类型模式匹配，但本机库不支持此功能。之前，我们简要介绍了 F# 的“活动模式”功能，该功能允许您动态创建匹配选项。这对 .NET 集成非常有用。

一个常见的情况是 .NET 库类有许多互斥的 `isSomething`、`isSomethingElse` 方法，这些方法必须用看起来很可怕的级联 if-else 语句进行测试。主动模式可以隐藏所有丑陋的测试，让代码的其余部分使用更自然的方法。

例如，这是测试 `System.Char` 的各种 `isXXX` 方法的代码。

```F#
let (|Digit|Letter|Whitespace|Other|) ch =
   if System.Char.IsDigit(ch) then Digit
   else if System.Char.IsLetter(ch) then Letter
   else if System.Char.IsWhiteSpace(ch) then Whitespace
   else Other
```

一旦定义了选项，正常的代码就可以很简单了：

```F#
let printChar ch =
  match ch with
  | Digit -> printfn "%c is a Digit" ch
  | Letter -> printfn "%c is a Letter" ch
  | Whitespace -> printfn "%c is a Whitespace" ch
  | _ -> printfn "%c is something else" ch

// print a list
['a';'b';'1';' ';'-';'c'] |> List.iter printChar
```

另一种常见情况是，您必须解析文本或错误代码以确定异常或结果的类型。下面是一个使用活动模式解析与 `SqlExceptions` 相关的错误号的示例，使其更易于接受。

首先，在错误号上设置活动模式匹配：

```F#
open System.Data.SqlClient

let (|ConstraintException|ForeignKeyException|Other|) (ex:SqlException) =
   if ex.Number = 2601 then ConstraintException
   else if ex.Number = 2627 then ConstraintException
   else if ex.Number = 547 then ForeignKeyException
   else Other
```

现在我们可以在处理SQL命令时使用这些模式：

```F#
let executeNonQuery (sqlCommmand:SqlCommand) =
    try
       let result = sqlCommmand.ExecuteNonQuery()
       // handle success
    with
    | :?SqlException as sqlException -> // if a SqlException
        match sqlException with         // nice pattern matching
        | ConstraintException  -> // handle constraint error
        | ForeignKeyException  -> // handle FK error
        | _ -> reraise()          // don't handle any other cases
    // all non SqlExceptions are thrown normally
```

### 直接从接口创建对象

F# 还有另一个有用的特性，称为“对象表达式”。这是一种直接从接口或抽象类创建对象的能力，而无需先定义具体类。

在下面的示例中，我们使用 `makeResource` 辅助函数创建了一些实现 `IDisposable` 的对象。

```F#
// create a new object that implements IDisposable
let makeResource name =
   { new System.IDisposable
     with member this.Dispose() = printfn "%s disposed" name }

let useAndDisposeResources =
    use r1 = makeResource "first resource"
    printfn "using first resource"
    for i in [1..3] do
        let resourceName = sprintf "\tinner resource %d" i
        use temp = makeResource resourceName
        printfn "\tdo something with %s" resourceName
    use r2 = makeResource "second resource"
    printfn "using second resource"
    printfn "done."
```

该示例还演示了当资源超出范围时，“`use`”关键字如何自动处置资源。输出如下：

```
using first resource
	do something with 	inner resource 1
	inner resource 1 disposed
	do something with 	inner resource 2
	inner resource 2 disposed
	do something with 	inner resource 3
	inner resource 3 disposed
using second resource
done.
second resource disposed
first resource disposed
```

### 混合纯 F# 类型的 .NET 接口

动态创建接口实例的能力意味着很容易将现有 API 的接口与纯 F# 类型混合和匹配。

例如，假设您有一个预先存在的 API，它使用 IAnimal 接口，如下所示。

```F#
type IAnimal =
   abstract member MakeNoise : unit -> string

let showTheNoiseAnAnimalMakes (animal:IAnimal) =
   animal.MakeNoise() |> printfn "Making noise %s"
```

但是我们希望拥有模式匹配等的所有好处，所以我们为猫和狗创建了纯 F# 类型，而不是类。

```F#
type Cat = Felix | Socks
type Dog = Butch | Lassie
```

但是使用这种纯 F# 方法意味着我们不能直接将猫和狗传递给 `showTheNoiseAnAnimalMakes` 函数。

然而，我们不必为了实现 `IAnimal` 而创建新的具体类集。相反，我们可以通过扩展纯 F# 类型来动态创建 `IAnimal` 接口。

```F#
// now mixin the interface with the F# types
type Cat with
   member this.AsAnimal =
        { new IAnimal
          with member a.MakeNoise() = "Meow" }

type Dog with
   member this.AsAnimal =
        { new IAnimal
          with member a.MakeNoise() = "Woof" }
```

以下是一些测试代码：

```F#
let dog = Lassie
showTheNoiseAnAnimalMakes (dog.AsAnimal)

let cat = Felix
showTheNoiseAnAnimalMakes (cat.AsAnimal)
```

这种方法让我们两全其美。内部纯 F# 类型，但能够根据需要将其转换为与库交互的接口。

### 使用反射检查 F# 类型

F# 从 .NET 反射系统中受益，这意味着您可以使用语言本身的语法完成各种您无法直接使用的有趣的事情。`Microsoft.FSharp.Reflection` 命名空间有许多专门用于帮助 F# 类型的函数。

例如，这里有一种打印记录类型中的字段和联合类型中的选项的方法。

```F#
open System.Reflection
open Microsoft.FSharp.Reflection

// create a record type...
type Account = {Id: int; Name: string}

// ... and show the fields
let fields =
    FSharpType.GetRecordFields(typeof<Account>)
    |> Array.map (fun propInfo -> propInfo.Name, propInfo.PropertyType.Name)

// create a union type...
type Choices = | A of int | B of string

// ... and show the choices
let choices =
    FSharpType.GetUnionCases(typeof<Choices>)
    |> Array.map (fun choiceInfo -> choiceInfo.Name)
```

## 29 C# 可以做的任何事情……

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/completeness-anything-csharp-can-do/#series-toc)*)*

F# 中面向对象代码的旋风之旅
29 Apr 2012 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/completeness-anything-csharp-can-do/

很明显，在 F# 中，你通常应该尝试更喜欢函数式代码而不是面向对象代码，但在某些情况下，你可能需要一种完全成熟的 OO 语言的所有功能——类、继承、虚拟方法等。

因此，为了结束本节，这里是对这些功能的 F# 版本的快速浏览。

其中一些将在稍后的系列文章中更深入地讨论。NET集成。但我不会介绍一些比较晦涩的，如果你需要的话，可以在MSDN文档中阅读它们。

### 类和接口

首先，这里有一些接口、抽象类和从抽象类继承的具体类的示例。

```F#
// interface
type IEnumerator<'a> =
    abstract member Current : 'a
    abstract MoveNext : unit -> bool

// abstract base class with virtual methods
[<AbstractClass>]
type Shape() =
    //readonly properties
    abstract member Width : int with get
    abstract member Height : int with get
    //non-virtual method
    member this.BoundingArea = this.Height * this.Width
    //virtual method with base implementation
    abstract member Print : unit -> unit
    default this.Print () = printfn "I'm a shape"

// concrete class that inherits from base class and overrides
type Rectangle(x:int, y:int) =
    inherit Shape()
    override this.Width = x
    override this.Height = y
    override this.Print ()  = printfn "I'm a Rectangle"

//test
let r = Rectangle(2,3)
printfn "The width is %i" r.Width
printfn "The area is %i" r.BoundingArea
r.Print()
```

类可以有多个构造函数、可变属性等。

```F#
type Circle(rad:int) =
    inherit Shape()

    //mutable field
    let mutable radius = rad

    //property overrides
    override this.Width = radius * 2
    override this.Height = radius * 2

    //alternate constructor with default radius
    new() = Circle(10)

    //property with get and set
    member this.Radius
         with get() = radius
         and set(value) = radius <- value

// test constructors
let c1 = Circle()   // parameterless ctor
printfn "The width is %i" c1.Width
let c2 = Circle(2)  // main ctor
printfn "The width is %i" c2.Width

// test mutable property
c2.Radius <- 3
printfn "The width is %i" c2.Width
```

### 泛型

F#支持泛型和所有相关的约束。

```F#
// standard generics
type KeyValuePair<'a,'b>(key:'a, value: 'b) =
    member this.Key = key
    member this.Value = value

// generics with constraints
type Container<'a,'b
    when 'a : equality
    and 'b :> System.Collections.ICollection>
    (name:'a, values:'b) =
    member this.Name = name
    member this.Values = values
```

### 结构体

F# 不仅支持类，还支持 .NET 结构类型，在某些情况下可以帮助提高性能。

```F#
type Point2D =
   struct
      val X: float
      val Y: float
      new(x: float, y: float) = { X = x; Y = y }
   end

//test
let p = Point2D()  // zero initialized
let p2 = Point2D(2.0,3.0)  // explicitly initialized
```

### 异常

F# 可以创建异常类，引发它们并捕获它们。

```F#
// create a new Exception class
exception MyError of string

try
    let e = MyError("Oops!")
    raise e
with
    | MyError msg ->
        printfn "The exception error was %s" msg
    | _ ->
        printfn "Some other exception"
```

### 扩展方法

与 C# 一样，F# 可以使用扩展方法扩展现有类。

```F#
type System.String with
    member this.StartsWithA = this.StartsWith "A"

//test
let s = "Alice"
printfn "'%s' starts with an 'A' = %A" s s.StartsWithA

type System.Int32 with
    member this.IsEven = this % 2 = 0

//test
let i = 20
if i.IsEven then printfn "'%i' is even" i
```

### 参数数组

就像 C# 的可变长度“params”关键字一样，这允许将可变长度的参数列表转换为单个数组参数。

```F#
open System
type MyConsole() =
    member this.WriteLine([<ParamArray>] args: Object[]) =
        for arg in args do
            printfn "%A" arg

let cons = new MyConsole()
cons.WriteLine("abc", 42, 3.14, true)
```

### 事件

F# 类可以有事件，这些事件可以被触发和响应。

```F#
type MyButton() =
    let clickEvent = new Event<_>()

    [<CLIEvent>]
    member this.OnClick = clickEvent.Publish

    member this.TestEvent(arg) =
        clickEvent.Trigger(this, arg)

// test
let myButton = new MyButton()
myButton.OnClick.Add(fun (sender, arg) ->
        printfn "Click event with arg=%O" arg)

myButton.TestEvent("Hello World!")
```

### 委托

F# 可以做委托。

```F#
// delegates
type MyDelegate = delegate of int -> int
let f = MyDelegate (fun x -> x * x)
let result = f.Invoke(5)
```

### 枚举类型

F# 支持 CLI 枚举类型，这些枚举类型看起来类似于“union”类型，但实际上在幕后是不同的。

```F#
// enums
type Color = | Red=1 | Green=2 | Blue=3

let color1  = Color.Red    // simple assignment
let color2:Color = enum 2  // cast from int
// created from parsing a string
let color3 = System.Enum.Parse(typeof<Color>,"Green") :?> Color // :?> is a downcast

[<System.Flags>]
type FileAccess = | Read=1 | Write=2 | Execute=4
let fileaccess = FileAccess.Read ||| FileAccess.Write
```

### 使用标准用户界面

最后，F# 可以像 C# 一样使用 WinForms 和 WPF 用户界面库。

这是一个打开表单并处理点击事件的简单示例。

```F#
open System.Windows.Forms

let form = new Form(Width= 400, Height = 300, Visible = true, Text = "Hello World")
form.TopMost <- true
form.Click.Add (fun args-> printfn "the form was clicked")
form.Show()
```

## 30 为什么使用F#：结论

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/why-use-fsharp-conclusion/#series-toc)*)*

2012年4月30日 这篇文章已有3年多的历史了

https://fsharpforfunandprofit.com/posts/why-use-fsharp-conclusion/

至此，F# 函数式编程之旅结束。我希望这些例子能让你对 F# 和函数式编程的力量有所了解。如果您对整个系列有任何意见，请将其留在本页底部。

在后面的系列中，我希望更深入地了解数据结构、模式匹配、列表处理、异步和并行编程等等。

但在此之前，我建议你阅读“函数式思维”系列，这将有助于你更深入地理解函数式编程。



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

## 1 函数式思维：引言

*Part of the "Thinking functionally" series (*[link](https://fsharpforfunandprofit.com/posts/thinking-functionally-intro/#series-toc)*)*

函数式编程的基础知识
01五月2012 这篇文章是超过3年

https://fsharpforfunandprofit.com/posts/thinking-functionally-intro/

现在你已经在“为什么使用F#”系列中看到了F#的一些力量，我们将回过头来看看函数式编程的基本原理——“函数式编程”的真正含义是什么，以及这种方法与面向对象或命令式编程有何不同。

### 改变你的思维方式

重要的是要理解函数式编程不仅仅是风格上的差异；这是一种完全不同的编程思维方式，就像真正的面向对象编程（在 Smalltalk 中）也是一种与 C 等传统命令式语言不同的思维方式一样。

F# 确实允许非函数式风格，并且很容易保留你已经熟悉的习惯。你可能只是以一种非函数式的方式使用F#，而不会真正改变你的思维方式，也不会意识到你错过了什么。为了充分利用 F#，并熟练掌握函数式编程，你必须从函数式而非命令式的角度思考。这就是本系列的目标：帮助您深入理解函数式编程，并帮助您改变思维方式。

这将是一个相当抽象的系列，尽管我将使用大量简短的代码示例来演示这些观点。我们将探讨以下几点：

- **数学函数**。第一篇文章介绍了函数式语言背后的数学思想，以及这种方法带来的好处。
- **函数和值**。下一篇文章将介绍函数和值，展示“值”与变量的不同之处，以及为什么函数和简单值之间存在相似之处。
- **类型**。然后我们继续讨论与函数一起工作的基本类型：string和int等基本类型；单元类型、函数类型和泛型类型。
- **具有多个参数的函数**。接下来，我将解释“currying”和“partial application”的概念。如果你来自一个迫切的背景，这就是你的大脑开始受伤的地方！
- **定义函数**。然后，一些帖子专门介绍了定义和组合功能的许多不同方法。
  函数签名。然后是一篇关于函数签名这一关键主题的重要文章：它们的含义以及如何使用它们来帮助理解。
- **组织函数**。一旦你知道如何创建函数，你如何组织它们，使它们可用于其他代码？

## 2 数学函数

*Part of the "Thinking functionally" series (*[link](https://fsharpforfunandprofit.com/posts/mathematical-functions/#series-toc)*)*

函数式编程背后的动力
02五月2012 这篇文章是超过3年

https://fsharpforfunandprofit.com/posts/mathematical-functions/

函数式编程背后的动力来自数学。数学函数具有许多非常好的特性，函数式语言试图在现实世界中模拟这些特性。

首先，让我们从一个数学函数开始，该函数将一个数字加1。

`Add1(x) = x+1`

这到底意味着什么？嗯，这似乎很简单。这意味着有一个操作以一个数字开始，然后加一。

让我们介绍一些术语：

- 可以用作函数输入的值集称为域。在这种情况下，它可能是实数集，但为了让现在的生活更简单，让我们把它限制在整数上。
- 函数的一组可能输出值称为范围（技术上称为共域上的图像）。在这种情况下，它也是整数集。
- 据说该函数将域映射到范围。

以下是 F# 中的定义

```F#
let add1 x = x + 1
```

如果你在 F# 交互窗口中键入它（不要忘记双分号），你会看到结果（函数的“签名”）：

```F#
val add1 : int -> int
```

让我们详细看看这个输出：

- 总体含义是“函数 `add1` 将整数（域）映射到整数（范围）上”。
- “`add1`”被定义为“val”，是“value”的缩写。嗯……那是什么意思？我们稍后将讨论值。
- 箭头符号“->”用于显示域和范围。在这种情况下，域是int类型，范围也是int类型。

还要注意，没有指定类型，但 F# 编译器猜测该函数正在处理 int。（这个可以调整吗？是的，我们稍后会看到）。

### 数学函数的关键性质

数学函数具有一些性质，这些性质与你在过程编程中习惯的函数类型非常不同。

- 对于给定的输入值，函数总是给出相同的输出值
- 函数没有副作用。

这些属性提供了一些非常强大的好处，因此函数式编程语言也试图在设计中强制执行这些属性。让我们依次看看它们中的每一个。

#### 对于给定的输入，数学函数总是给出相同的输出

在命令式编程中，我们认为函数“做”某事或“计算”某事。数学函数不做任何计算——它纯粹是从输入到输出的映射。事实上，定义函数的另一种方式就是将其视为所有映射的集合。例如，以一种非常粗略的方式，我们可以将“`add1`”函数（在C#中）定义为

```C#
int add1(int input)
{
   switch (input)
   {
   case 0: return 1;
   case 1: return 2;
   case 2: return 3;
   case 3: return 4;
   etc ad infinitum
   }
}
```

显然，我们不能为每个可能的整数都有一个案例，但原理是一样的。你可以看到，根本没有进行任何计算，只是进行了查找。

#### 数学函数没有副作用

在数学函数中，输入和输出在逻辑上是两个不同的东西，两者都是预定义的。该函数不会改变输入或输出，它只是将域中预先存在的输入值映射到范围内预先存在的输出值。

换句话说，评估函数不可能对输入或其他任何东西产生任何影响。记住，评估函数实际上并不是计算或操纵任何东西；这只是一次美化的查找。

这些值的“不变性”是微妙的，但非常重要。如果我在做数学，我不希望当我加上数字时，下面的数字会改变！例如，如果我有：

```
x = 5
y = x+1
```

我不希望x因加1而改变。我希望得到一个不同的数字（y），x保持不变。在数学领域，整数已经作为一个不可改变的集合存在，而“add1”函数只是定义了它们之间的关系。

#### 纯函数的力量

具有可重复结果且没有副作用的函数称为“纯函数”，你可以用它们做一些有趣的事情：

- 它们是可并行化的。我可以取 1 到 1000 之间的所有整数，假设有1000个不同的CPU，我可以让每个CPU同时执行相应整数的“`add1`”函数，因为它们之间不需要任何交互。不需要锁、互斥量（mutexes）、信号量（semaphores）等。
- 我可以惰性地使用函数，只在需要输出时对其进行求值。我可以肯定，无论我现在还是以后求值，答案都是一样的。
- 我只需要对某个输入计算一次函数，然后就可以缓存结果，因为我知道相同的输入总是给出相同的输出。
- 如果我有一些纯函数，我可以按照我喜欢的任何顺序对它们进行求值。同样，这对最终结果没有任何影响。

所以你可以看到，如果我们能用编程语言创建纯函数，我们会立即获得很多强大的技术。事实上，你可以在F#中完成所有这些事情：

- 您已经在“为什么使用F#？”系列中看到了并行性的一个例子。
- 惰性地求值函数将在“优化”系列中讨论。
- 缓存函数的结果称为“记忆化”，也将在“优化”系列中讨论。
- 不关心求值顺序会使并发编程更容易，并且在重新排序或重构函数时不会引入错误。

### 数学函数的“无用”性质

数学函数也有一些在编程中使用时似乎没有多大帮助的特性。

- 输入和输出值是不可变的
- 函数总是只有一个输入和一个输出

这些特性也反映在函数式编程语言的设计中。让我们依次看看这些。

#### 输入和输出值是不可变的

从理论上讲，不可变值似乎是一个好主意，但如果你不能以传统方式为变量赋值，你怎么能真正完成任何工作呢？

我可以向你保证，这并不像你想象的那么严重。当您完成本系列时，您将看到这在实践中是如何工作的。

#### 数学函数总是只有一个输入和一个输出

正如你从图表中看到的，一个数学函数总是只有一个输入和一个输出。函数式编程语言也是如此，尽管当你第一次使用它们时可能并不明显。

这似乎很烦人。如果没有具有两个（或更多）参数的函数，你如何做有用的事情？

好吧，它有一种方法可以做到这一点，而且，在F#中，它对你来说是完全透明的。它被称为“currying”，它值得拥有自己的职位，这个职位很快就会出现。

事实上，正如您稍后将发现的那样，这两个“无益”的属性将变得非常有用，并且是函数式编程如此强大的关键部分。

## 3 函数值和简单值

*Part of the "Thinking functionally" series (*[link](https://fsharpforfunandprofit.com/posts/function-values-and-simple-values/#series-toc)*)*

绑定不赋值
03五月2012这篇文章是超过3年

https://fsharpforfunandprofit.com/posts/function-values-and-simple-values/

让我们再看看这个简单的函数

这里的“x”是什么意思？这意味着：

1. 从输入域中接受一些值。
2. 使用名称“x”表示该值，以便我们以后可以引用它。

使用名称表示值的过程称为“绑定”。名称“x”被“绑定”到输入值。

所以，如果我们用输入5来评估函数，比如说，在原始定义中看到“x”的地方，我们用“5”替换它，有点像文字处理器中的搜索和替换。

```F#
let add1 x = x + 1
add1 5
// replace "x" with "5"
// add1 5 = 5 + 1 = 6
// result is 6
```

重要的是要明白这不是任务。“x”不是分配给该值的“插槽”或变量，以后可以分配给另一个值。它是名称“x”与该值的一次性关联。该值是预定义的整数之一，不能更改。因此，一旦绑定，x也不能改变；一旦与某个值相关联，就始终与该值相关联。

这个概念是函数式思维的关键部分：没有“变量”，只有值。

### 函数值

如果你再仔细想想，你会发现“`add1`”这个名字本身只是“向其输入添加一个值的函数”的绑定。函数本身与其绑定的名称无关。

当你键入 `let add1 x=x+1` 时，你是在告诉 F# 编译器“每次看到名称”`add1`“时，用向其输入添加1的函数替换它”。“`add1`”被称为**函数值**。

要查看函数是否独立于其名称，请尝试：

```F#
let add1 x = x + 1
let plus1 = add1
add1 5
plus1 5
```

您可以看到“`add1`”和“`plus1`”是两个引用（“绑定到”）同一函数的名称。

你总是可以识别一个函数值，因为它的签名具有标准形式的 `域->范围`。这是一个通用的函数值签名：

```F#
val functionName : domain -> range
```

### 简单值

想象一下，一个操作总是返回整数5，并且没有任何输入。



这将是一个“常量”的操作。

我们如何用F#写这个？我们想告诉F#编译器“每次你看到名称c时，用5替换它”。方法如下：

```F#
let c = 5
```

其在求值时返回：

```F#
val c : int = 5
```

这次没有映射箭头，只有一个整数。新的是一个等号，后面打印了实际值。F# 编译器知道这个绑定有一个已知的值，它将始终返回，即值 5。

换句话说，我们刚刚定义了一个常量，或者用 F# 术语来说，一个简单的值。

你总是可以区分简单值和函数值，因为所有简单值都有一个签名，看起来像：

```F#
val aName: type = constant     // Note that there is no arrow
```

### 简单值与函数值

重要的是要理解，在 F# 中，与 C# 等语言不同，简单值和函数值之间几乎没有区别。它们都是可以绑定到名称（使用相同的关键字let）然后传递的值。事实上，函数思维的一个关键方面就是：函数是可以作为输入传递给其他函数的值，我们很快就会看到。

请注意，简单值和函数值之间存在细微差异。函数总是有一个域和范围，必须“应用”到参数才能得到结果。绑定后不需要对简单值进行求值。使用上面的例子，如果我们想定义一个返回 5 的“常量函数”，我们必须使用

```F#
let c = fun()->5
// or
let c() = 5
```

这些函数的签名是：

```F#
val c : unit -> int
```

而不是：

```F#
val c : int = 5
```

稍后将详细介绍单元、函数语法和匿名函数。

### “值”与“对象”

在F#这样的函数式编程语言中，大多数东西都被称为“值”。在C#这样的面向对象语言中，大多数东西都被称为“对象”。那么，“值”和“对象”之间有什么区别呢？

如上所述，值只是域的一个成员。int的域、字符串的域、将int映射到字符串的函数的域等等。原则上，值是不可变的。值没有任何行为。

在标准定义中，对象是数据结构及其相关行为（方法）的封装。一般来说，对象应该具有状态（即可变），所有改变内部状态的操作都必须由对象本身提供（通过“点”符号）。

在F#中，即使是原始值也有一些类似对象的行为。例如，您可以点入字符串以获取其长度：

```F#
"abc".Length
```

但是，一般来说，我们将避免在 F# 中为标准值使用“object”，将其保留为引用真类的实例或其他公开成员方法的值。

### 命名值

标准命名规则用于值和函数名称，基本上是任何字母数字字符串，包括下划线。有几个额外的：

您可以在名称中的任何位置添加撇号，但第一个字符除外。所以：

```F#
A'b'c     begin'  // valid names
```

最后一个刻度通常用于表示值的某种“变体”版本：

```F#
let f = x
let f' = derivative f
let f'' = derivative f'
```

或定义现有关键字的变体

```F#
let if' b t f = if b then t else f
```

您还可以在任何字符串周围加上双引号，以形成有效的标识符。

```F#
``this is a name``  ``123``    //valid names
```

有时你可能想使用双引号技巧：

- 当您想使用与关键字相同的标识符时

  ```F#
  let ``begin`` = "begin"
  ```

- 当试图将自然语言用于业务规则、单元测试或BDD风格的可执行规范时，就像Cucumber一样。

  ```F#
  let ``is first time customer?`` = true
  let ``add gift to order`` = ()
  if ``is first time customer?`` then ``add gift to order``
  
  // Unit test
  let [<Test>] ``When input is 2 then expect square is 4``=
     // code here
  
  // BDD clause
  let [<Given>] ``I have (.*) N products in my cart`` (n:int) =
     // code here
  ```

与C#不同，F#的命名约定是函数和值以小写字母而不是大写字母开头（camelCase 而不是 PascalCase），除非是为暴露给其他 .NET语言而设计的。但是，类型和模块使用大写字母。

## 4 类型如何与函数协同工作

*Part of the "Thinking functionally" series (*[link](https://fsharpforfunandprofit.com/posts/how-types-work-with-functions/#series-toc)*)*

理解类型符号
04五月2012 这篇文章是超过3年

https://fsharpforfunandprofit.com/posts/how-types-work-with-functions/

现在我们对函数有了一些了解，我们将看看类型是如何与函数一起工作的，无论是作为域还是范围。这只是一个概述；“理解F#类型”系列将详细介绍类型。

首先，我们需要更多地了解类型表示法。我们已经看到箭头符号“->”用于显示域和范围。所以函数签名总是看起来像：

```F#
val functionName : domain -> range
```

以下是一些示例函数：

```F#
let intToString x = sprintf "x is %i" x  // format int to string
let stringToInt x = System.Int32.Parse(x)
```

如果你在F#交互窗口中进行评估，你会看到签名：

```F#
val intToString : int -> string
val stringToInt : string -> int
```

这意味着：

- `intToString` 有一个 int 域，它映射到范围字符串上。
- `stringToInt` 有一个字符串域，它映射到 int 范围。

### 原始类型

可能的原始类型是您所期望的：string、int、float、bool、char、byte等，以及从派生的更多类型。NET类型系统。

以下是使用基元类型的函数的更多示例：

```F#
let intToFloat x = float x // "float" fn. converts ints to floats
let intToBool x = (x = 2)  // true if x equals 2
let stringToString x = x + " world"
```

他们的签名是：

```F#
val intToFloat : int -> float
val intToBool : int -> bool
val stringToString : string -> string
```

### 类型注解

在前面的示例中，F# 编译器正确地确定了参数和结果的类型。但情况并非总是如此。如果你尝试以下代码，你会得到一个编译器错误：

```F#
let stringLength x = x.Length
   => error FS0072: Lookup on object of indeterminate type
```

编译器不知道“x”是什么类型，因此不知道“Length”是否是有效的方法。在大多数情况下，这可以通过给 F# 编译器一个“类型注释”来解决，这样它就知道要使用哪种类型。在下面的更正版本中，我们指出“x”的类型是字符串。

```F#
let stringLength (x:string) = x.Length
```

`x:string` 参数周围的括号很重要。如果它们缺失，编译器会认为返回值是一个字符串！也就是说，“open”冒号用于指示返回值的类型，如下面的示例所示。

```F#
let stringLengthAsInt (x:string) :int = x.Length
```

我们表示x参数是一个字符串，返回值是一个int。

### 函数类型作为参数

将其他函数作为参数或返回函数的函数称为**高阶函数**（有时缩写为 HOF）。它们被用作抽象出常见行为的一种方式。这类函数在 F# 中非常常见；大多数标准库都使用它们。

考虑一个函数 `evalWith5ThenAdd2`，它将函数作为参数，然后用值5计算函数，并将结果加2：

```F#
let evalWith5ThenAdd2 fn = fn 5 + 2     // same as fn(5) + 2
```

此函数的签名如下：

```F#
val evalWith5ThenAdd2 : (int -> int) -> int
```

你可以看到域是（`int->int`），范围是 `int`。这是什么意思？这意味着输入参数不是一个简单的值，而是一个函数，而且仅限于将 `int` 映射到 `int` 的函数。输出不是函数，只是一个int。

让我们试试：

```F#
let add1 x = x + 1      // define a function of type (int -> int)
evalWith5ThenAdd2 add1  // test it
```

得到：

```F#
val add1 : int -> int
val it : int = 8
```

“`add1`”是一个将int映射到int的函数，我们可以从它的签名中看到。因此，它是 `evalWith5TenAdd2` 函数的有效参数。结果是8。

顺便说一句，特殊的单词“`it`”用于最后一个被评估的东西；在这种情况下，我们想要的结果。这不是一个关键字，只是一个惯例。

这是另一个：

```F#
let times3 x = x * 3      // a function of type (int -> int)
evalWith5ThenAdd2 times3  // test it
```

得到：

```F#
val times3 : int -> int
val it : int = 17
```

“`times3`”也是一个将int映射到int的函数，我们可以从它的签名中看到。因此，它也是 `evalWith5TenAdd2` 函数的有效参数。结果是 17。

请注意，输入对类型很敏感。如果我们的输入函数使用浮点数而不是整数，它将无法工作。例如，如果我们有：

```F#
let times3float x = x * 3.0  // a function of type (float->float)
evalWith5ThenAdd2 times3float
```

对此进行评估将产生错误：

```F#
error FS0001: Type mismatch. Expecting a int -> int but
              given a float -> float
```

这意味着输入函数应该是 `int->int` 函数。

#### 作为输出函数

函数值也可以是函数的输出。例如，以下函数将生成一个使用输入值进行加法的“加法器”函数。

```F#
let adderGenerator numberToAdd = (+) numberToAdd
```

签名为：

```F#
val adderGenerator : int -> (int -> int)
```

这意味着生成器接受一个 `int`，并创建一个将 `int` 映射到 `int` 的函数（“加法器”）。让我们看看它是如何工作的：

```F#
let add1 = adderGenerator 1
let add2 = adderGenerator 2
```

这将创建两个加法器函数。第一个生成的函数将其输入加1，第二个函数将其输出加2。请注意，签名正如我们所期望的那样。

```F#
val add1 : (int -> int)
val add2 : (int -> int)
```

我们现在可以以正常方式使用这些生成的函数。它们与明确定义的函数无法区分

```F#
add1 5    // val it : int = 6
add2 5    // val it : int = 7
```

#### 使用类型注释约束函数类型

在第一个例子中，我们有一个函数：

```F#
let evalWith5ThenAdd2 fn = fn 5 +2
    => val evalWith5ThenAdd2 : (int -> int) -> int
```

在这种情况下，F#可以推断出“`fn`”将 `int` 映射为 `int`，因此它的签名将是 `int->int`

但是，在以下情况下，“fn”的签名是什么？

```F#
let evalWith5 fn = fn 5
```

显然，“`fn`”是一种接受 int 的函数，但它返回什么？编译器无法判断。如果确实要指定函数的类型，可以按照与基本类型相同的方式为函数参数添加类型注释。

```F#
let evalWith5AsInt (fn:int->int) = fn 5
let evalWith5AsFloat (fn:int->float) = fn 5
```

或者，您也可以指定返回类型。

```F#
let evalWith5AsString fn :string = fn 5
```

因为main函数返回字符串，所以“`fn`”函数也被约束为返回字符串，因此不需要对“fn“进行显式键入。

### “unit”类型

在编程时，我们有时希望函数在不返回值的情况下执行某些操作。考虑下面定义的函数“printInt”。该函数实际上没有返回任何东西。它只是在控制台上打印一个字符串作为副作用。

```F#
let printInt x = printf "x is %i" x        // print to console
```

那么这个函数的签名是什么？

```F#
val printInt : int -> unit
```

这个“`unit`”是什么？

好吧，即使一个函数没有返回输出，它仍然需要一个范围。数学世界中没有“空”函数。每个函数都必须有一些输出，因为函数是一个映射，映射必须有一些东西可以映射！



因此，在F#中，像这样的函数返回一个称为“`unit`”的特殊范围。此范围中只有一个值，称为“`()`”。你可以把`unit` 和 `()` 看作是C#中的“void”（类型）和“null”（值）。但与void/null不同，`unit` 是实数类型，（）是实数值。要看到这一点，请评估：

```F#
let whatIsThis = ()
```

您将看到签名：

```F#
val whatIsThis : unit = ()
```

这意味着值“`whatIsThis`”的类型为 `unit`，并且已绑定到值 `()`

所以，回到“`printInt`”的签名，我们现在可以理解它：

```F#
val printInt : int -> unit
```

这个签名说：`printInt` 有一个 `int` 域，它映射到我们不关心的任何东西上。

#### 无参数函数

现在我们了解了这个单位，我们能预测它在其他环境中的出现吗？例如，让我们尝试创建一个可重用的“hello world”函数。由于没有输入也没有输出，我们希望它有一个签名 `unit->unit`。让我们看看：

```F#
let printHello = printf "hello world"        // print to console
```

结果是：

```F#
hello world
val printHello : unit = ()
```

与我们预期的不太一样。“Hello world”会立即打印出来，结果不是函数，而是一个简单的类型单位值。正如我们之前看到的，我们可以看出这是一个简单的值，因为它具有以下形式的签名：

```F#
val aName: type = constant
```

因此，在这种情况下，我们看到 `printHello` 实际上是一个具有值 `()` 的简单值。这不是一个我们可以再次调用的函数。

为什么 `printInt` 和 `printHello` 有区别？在 `printInt` 的情况下，在我们知道 x 参数的值之前，无法确定值，因此定义是一个函数。在 `printHello` 的情况下，没有参数，因此可以立即确定右侧。它是什么，返回 `()` 值，并附带向控制台打印的副作用。

我们可以通过强制定义有一个单元参数来创建一个真正的无参数可重用函数，如下所示：

```F#
let printHelloFn () = printf "hello world"    // print to console
```

现在签名是：

```F#
val printHelloFn : unit -> unit
```

要调用它，我们必须将 `()` 值作为参数传递，如下所示：

```F#
printHelloFn ()
```

#### 使用忽略功能强制单位类型

在某些情况下，编译器需要一个单元类型，并会发出抱怨。例如，以下两种情况都是编译器错误：

```F#
do 1+1     // => FS0020: This expression should have type 'unit'

let something =
  2+2      // => FS0020: This expression should have type 'unit'
  "hello"
```

为了在这些情况下提供帮助，有一个特殊的函数 `ignore`，它接受任何东西并返回单位类型。此代码的正确版本为：

```F#
do (1+1 |> ignore)  // ok

let something =
  2+2 |> ignore     // ok
  "hello"
```

### 泛型类型

在许多情况下，函数参数的类型可以是任何类型，因此我们需要一种方法来指示这一点。F#使用 .NET 泛型类型系统用于这种情况。

例如，以下函数将参数转换为字符串并附加一些文本：

```F#
let onAStick x = x.ToString() + " on a stick"
```

参数是什么类型并不重要，因为所有对象都理解 `ToString()`。

签名为：

```F#
val onAStick : 'a -> string
```

这种类型被称为“`'a`”是什么？这是 F# 表示编译时未知的泛型类型的方式。“a”前面的撇号表示类型是泛型的。C#对应的签名如下：

```C#
string onAStick<a>();

//or more idiomatically
string OnAStick<TObject>();   // F#'s use of 'a is like
                              // C#'s "TObject" convention
```

请注意，F# 函数仍然是具有泛型类型的强类型函数。它不接受 `Object` 类型的参数。这种强类型是可取的，这样当函数组合在一起时，仍然可以保持类型安全。

这是一个用于int、float和字符串的相同函数

```F#
onAStick 22
onAStick 3.14159
onAStick "hello"
```

如果有两个泛型参数，编译器会给它们不同的名称：`'a` 代表第一个泛型，`'b` 代表第二个泛型，以此类推。以下是一个示例：

```F#
let concatString x y = x.ToString() + y.ToString()
```

这个的类型签名有两个泛型：`'a` 和 `'b`：

```F#
val concatString : 'a -> 'b -> string
```

另一方面，编译器将识别何时只需要一个泛型类型。在以下示例中，x 和 y 参数必须为同一类型：

```F#
let isEqual x y = (x=y)
```

因此，这两个函数的签名具有相同的泛型类型：

```F#
val isEqual : 'a -> 'a -> bool
```

当涉及到列表和更抽象的结构时，泛型参数也非常重要，我们将在接下来的示例中看到很多。

### 其他类型

到目前为止讨论的类型只是基本类型。这些类型可以以各种方式组合，以制作更复杂的类型。对这些类型的全面讨论将不得不等待下一系列文章，但与此同时，这里有一个简短的介绍，以便您在函数签名中识别它们。

- **“tuple”类型**。这些是其他类型的成对、三元组等。例如 `("hello"，1)` 是一个由字符串和 int 组成的元组。逗号是元组的区别特征——如果你在 F# 中看到逗号，它几乎肯定是元组的一部分！

  在函数签名中，元组被写为所涉及的两种类型的“乘法”。因此，在这种情况下，元组的类型为：

  ```F#
  string * int      // ("hello", 1)
  ```

- **集合类型**。其中最常见的是列表、序列和数组。列表和数组的大小是固定的，而序列可能是无限的（在幕后，序列与 `IEnumerable` 相同）。在函数签名中，它们有自己的关键字：“`list`”、“`seq`”和数组的“`[]`”。

  ```F#
  int list          // List type  e.g. [1;2;3]
  string list       // List type  e.g. ["a";"b";"c"]
  seq<int>          // Seq type   e.g. seq{1..10}
  int []            // Array type e.g. [|1;2;3|]
  ```

- **选项类型**。这是一个简单的包装器，用于包装可能缺失的对象。有两种情况：`Some` 和 `None`。在函数签名中，它们有自己的“`option`”关键字：

  ```F#
  int option        // Some(1)
  ```

- **可区分联合类型**。这些是基于一组其他类型的选项构建的。我们在“为什么使用F#？”系列中看到了一些这样的例子。在函数签名中，它们由类型的名称引用，因此没有特殊的关键字。

- **记录类型**。这些类似于结构或数据库行，是命名槽的列表。我们在“为什么使用F#？”系列中也看到了一些这样的例子。在函数签名中，它们是通过类型的名称引用的，所以也没有特殊的关键字。

### 测试你对类型的理解

你对这些类型了解多少？这里有一些表达方式——看看你能不能猜出它们的签名。要查看您是否正确，只需在交互式窗口中运行它们！

```F#
let testA   = float 2
let testB x = float 2
let testC x = float 2 + x
let testD x = x.ToString().Length
let testE (x:float) = x.ToString().Length
let testF x = printfn "%s" x
let testG x = printfn "%f" x
let testH   = 2 * 2 |> ignore
let testI x = 2 * 2 |> ignore
let testJ (x:int) = 2 * 2 |> ignore
let testK   = "hello"
let testL() = "hello"
let testM x = x=x
let testN x = x 1          // hint: what kind of thing is x?
let testO x:string = x 1   // hint: what does :string modify?
```



## 5 柯里化

*Part of the "Thinking functionally" series (*[link](https://fsharpforfunandprofit.com/posts/currying/#series-toc)*)*

将多参数函数分解为更小的单参数函数
05五月2012 这篇文章是超过3年

https://fsharpforfunandprofit.com/posts/currying/

在对基本类型稍作偏离之后，我们可以再次回到函数，特别是我们之前提到的难题：如果一个数学函数只能有一个参数，那么F#函数怎么可能有多个参数呢？

答案很简单：一个具有多个参数的函数被重写为一系列新函数，每个函数只有一个参数。这是编译器自动为您完成的。它被称为“currying”，以数学家Haskell Curry的名字命名，他对函数式编程的发展产生了重要影响。

为了了解这在实践中是如何工作的，让我们使用一个非常基本的例子来打印两个数字：

```F#
//normal version
let printTwoParameters x y =
   printfn "x=%i y=%i" x y
```

在内部，编译器将其重写为更像：

```F#
//explicitly curried version
let printTwoParameters x  =    // only one parameter!
   let subFunction y =
      printfn "x=%i y=%i" x y  // new function with one param
   subFunction                 // return the subfunction
```

让我们更详细地研究一下：

1. 构造名为“`printTwoParameters`”的函数，但只有一个参数：“x”
2. 在里面，构造一个只有一个参数的子函数：“y”。请注意，此内部函数使用“x”参数，但x没有作为参数显式传递给它。“x”参数在作用域内，因此内部函数可以看到它并使用它，而不需要传入它。
3. 最后，返回新创建的子函数。
4. 随后，该返回函数将用于处理“y”。“x”参数被烘焙到其中，因此返回的函数只需要y参数来完成函数逻辑。

通过以这种方式重写它，编译器确保了每个函数只需要一个参数。因此，当您使用“`printTwoParameters`”时，您可能会认为您使用的是一个双参数函数，但实际上它只是一个单参数函数！你可以通过只传递一个论点而不是两个论点来亲眼看到：

```F#
// eval with one argument
printTwoParameters 1

// get back a function!
val it : (int -> unit) = <fun:printTwoParameters@286-3>
```

如果你用一个参数来计算它，你不会得到错误，你会得到一个函数。

因此，当您使用两个参数调用 `printTwoParameters` 时，您真正要做的是：

- 使用第一个参数（x）调用 `printTwoParameters`
- `printTwoParameters` 返回一个包含“x”的新函数。
- 然后，您使用第二个参数（y）调用新函数

这是一个逐步版本的示例，然后是正常版本。

```F#
// step by step version
let x = 6
let y = 99
let intermediateFn = printTwoParameters x  // return fn with
                                           // x "baked in"
let result  = intermediateFn y

// inline version of above
let result  = (printTwoParameters x) y

// normal version
let result  = printTwoParameters x y
```

下面是另一个例子：

```F#
//normal version
let addTwoParameters x y =
   x + y

//explicitly curried version
let addTwoParameters x  =      // only one parameter!
   let subFunction y =
      x + y                    // new function with one param
   subFunction                 // return the subfunction

// now use it step by step
let x = 6
let y = 99
let intermediateFn = addTwoParameters x  // return fn with
                                         // x "baked in"
let result  = intermediateFn y

// normal version
let result  = addTwoParameters x y
```

同样，“双参数函数”实际上是一个返回中间函数的单参数函数。

但是等一下——“`+`”操作本身呢？这是一个必须有两个参数的二元运算，对吧？不，它像其他函数一样被curried。有一个名为“`+`”的函数，它接受一个参数并返回一个新的中间函数，就像上面的 `addTwoParameters` 一样。

当我们编写语句 `x+y` 时，编译器会重新排序代码以删除中缀，并将其转换为 `(+) x y`，这是一个名为 `+` 的函数，用两个参数调用。请注意，名为“+”的函数需要用括号括起来，以表明它被用作普通函数名，而不是中缀运算符。
最后，名为 `+` 的双参数函数被视为任何其他双参数函数。

```F#
// using plus as a single value function
let x = 6
let y = 99
let intermediateFn = (+) x     // return add with x baked in
let result  = intermediateFn y

// using plus as a function with two parameters
let result  = (+) x y

// normal version of plus as infix operator
let result  = x + y
```

是的，这适用于所有其他运算符和内置函数，如 printf。

```F#
// normal version of multiply
let result  = 3 * 5

// multiply as a one parameter function
let intermediateFn = (*) 3   // return multiply with "3" baked in
let result  = intermediateFn 5

// normal version of printfn
let result  = printfn "x=%i y=%i" 3 5

// printfn as a one parameter function
let intermediateFn = printfn "x=%i y=%i" 3  // "3" is baked in
let result  = intermediateFn 5
```

### curried函数的签名

既然我们知道了curried函数是如何工作的，那么我们应该期望它们的签名是什么样子的呢？

回到第一个例子“`printTwoParameters`”，我们看到它接受一个参数并返回一个中间函数。中间函数也接受了一个参数，但没有返回任何值（即unit）。因此，中间函数的类型为 `int->unit`。换句话说，`printTwoParameters` 的域是 `int`，范围是 `int->unit`。综上所述，我们可以看到最终的签名是：

```F#
val printTwoParameters : int -> (int -> unit)
```

如果你评估显式柯里化的实现，你会看到签名中的括号，如上所述，但如果你评估隐式柯里化的正常实现，括号会被省略，如下所示：

```F#
val printTwoParameters : int -> int -> unit
```

括号是可选的。如果你想理解函数签名，在脑海中重新添加它们可能会有所帮助。

此时，您可能想知道，返回中间函数的函数和常规双参数函数之间有什么区别？

这是一个返回函数的单参数函数：

```F#
let add1Param x = (+) x
// signature is = int -> (int -> int)
```

这是一个返回简单值的双参数函数：

```F#
let add2Params x y = (+) x y
// signature is = int -> int -> int
```

签名略有不同，但实际上没有区别，只是第二个函数会自动为您 curried。

### 具有两个以上参数的函数

currying 如何处理具有两个以上参数的函数？完全相同的方式：对于除最后一个参数之外的每个参数，该函数返回一个中间函数，其中包含前面的参数。

考虑一下这个人为的例子。我已经明确指定了参数的类型，但函数本身什么也不做。

```F#
let multiParamFn (p1:int)(p2:bool)(p3:string)(p4:float)=
   ()   //do nothing

let intermediateFn1 = multiParamFn 42
   // intermediateFn1 takes a bool
   // and returns a new function (string -> float -> unit)
let intermediateFn2 = intermediateFn1 false
   // intermediateFn2 takes a string
   // and returns a new function (float -> unit)
let intermediateFn3 = intermediateFn2 "hello"
   // intermediateFn3 takes a float
   // and returns a simple value (unit)
let finalResult = intermediateFn3 3.141
```

整体函数的签名是：

```F#
val multiParamFn : int -> bool -> string -> float -> unit
```

中间函数的签名为：

```F#
val intermediateFn1 : (bool -> string -> float -> unit)
val intermediateFn2 : (string -> float -> unit)
val intermediateFn3 : (float -> unit)
val finalResult : unit = ()
```

函数签名可以告诉你函数需要多少参数：只需计算括号外的箭头数量。如果函数接受或返回其他函数参数，括号中会有其他箭头，但可以忽略这些箭头。以下是一些示例：

```F#
int->int->int      // two int parameters and returns an int

string->bool->int  // first param is a string, second is a bool,
                   // returns an int

int->string->bool->unit // three params (int,string,bool)
                        // returns nothing (unit)

(int->string)->int      // has only one parameter, a function
                        // value (from int to string)
                        // and returns a int

(int->string)->(int->bool) // takes a function (int to string)
                           // returns a function (int to bool)
```

### 多参数问题

柯里化背后的逻辑可能会产生一些意想不到的结果，直到你理解它。记住，如果你计算一个参数比预期少的函数，你就不会出错。相反，您将返回一个部分应用的函数。如果你在期望值的上下文中继续使用这个部分应用的函数，你会从编译器那里得到模糊的错误消息。

这是一个看似无害的函数：

```F#
// create a function
let printHello() = printfn "hello"
```

当我们称之为如下所示时，您会期望发生什么？它会向控制台打印“你好”吗？在评估之前，请尝试猜测，这里有一个提示：一定要查看函数签名。

```F#
// call it
printHello
```

它不会像预期的那样被调用。原始函数需要一个未提供的 unit 参数，因此您得到的是一个部分应用的函数（在本例中没有参数）。

这个怎么样？它会编译吗？

```F#
let addXY x y =
    printfn "x=%i y=%i" x
    x + y
```

如果你对它进行评估，你会看到编译器对printfn行有抱怨。

```F#
printfn "x=%i y=%i" x
//^^^^^^^^^^^^^^^^^^^^^
//warning FS0193: This expression is a function value, i.e. is missing
//arguments. Its type is  ^a -> unit.
```

如果你不理解currying，这条消息会非常神秘！所有像这样独立计算的表达式（即不用作返回值或用“let”绑定到某物）都必须计算为单位值。在这种情况下，它不是计算单位值，而是计算函数。这是一种冗长的说法，说 `printfn` 缺少一个论点。

此类错误的一个常见情况是在与 .NET 库进行接口连接时。例如，`TextReader` 的 `ReadLine` 方法必须接受一个单位参数。通常很容易忘记这一点并省略括号，在这种情况下，你不会立即得到编译器错误，而只有当你试图将结果视为字符串时才会出现。

```F#
let reader = new System.IO.StringReader("hello");

let line1 = reader.ReadLine        // wrong but compiler doesn't
                                   // complain
printfn "The line is %s" line1     //compiler error here!
// ==> error FS0001: This expression was expected to have
// type string but here has type unit -> string

let line2 = reader.ReadLine()      //correct
printfn "The line is %s" line2     //no compiler error
```

在上面的代码中，`line1` 只是 `Readline` 方法的指针或委托，而不是我们期望的字符串。在 `reader.ReadLine()` 中使用 `()` 实际执行该函数。

### 参数太多

当你有太多的参数时，你也会得到类似的神秘消息。以下是一些向printf传递过多参数的示例。

```F#
printfn "hello" 42
// ==> error FS0001: This expression was expected to have
//                   type 'a -> 'b but here has type unit

printfn "hello %i" 42 43
// ==> Error FS0001: Type mismatch. Expecting a 'a -> 'b -> 'c
//                   but given a 'a -> unit

printfn "hello %i %i" 42 43 44
// ==> Error FS0001: Type mismatch. Expecting a 'a->'b->'c->'d
//                   but given a 'a -> 'b -> unit
```

例如，在最后一种情况下，编译器表示它期望格式参数有三个参数（签名 `'a->'b->'c->'d` 有三个变量），但只给出了两个参数（标记 `'a->'b->unit` 有两个参数）。

在不使用 `printf` 的情况下，传递太多的参数通常意味着你最终会得到一个简单的值，然后试图传递一个参数。编译器会抱怨这个简单的值不是函数。

```F#
let add1 x = x + 1
let x = add1 2 3
// ==>   error FS0003: This value is not a function
//                     and cannot be applied
```

如果你像前面一样将调用分解为一系列显式的中间函数，你就可以准确地看到出了什么问题。

```F#
let add1 x = x + 1
let intermediateFn = add1 2   //returns a simple value
let x = intermediateFn 3      //intermediateFn is not a function!
// ==>   error FS0003: This value is not a function
//                     and cannot be applied
```

## 6 部分应用

*Part of the "Thinking functionally" series (*[link](https://fsharpforfunandprofit.com/posts/partial-application/#series-toc)*)*

烘焙函数的某些参数
06五月2012 这篇文章是超过3年

https://fsharpforfunandprofit.com/posts/partial-application/

在上一篇关于currying的文章中，我们研究了如何将多个参数函数分解为更小的单参数函数。这是数学上正确的方法，但这不是唯一的原因——它还导致了一种非常强大的技术，称为**偏函数应用**。这是函数式编程中一种非常广泛使用的风格，理解它很重要。

部分应用的想法是，如果你固定了函数的前 N 个参数，你就得到了其余参数的函数。从关于柯里化的讨论中，你可能会看到这是如何自然发生的。

以下是一些简单的例子来证明这一点：

```F#
// create an "adder" by partial application of add
let add42 = (+) 42    // partial application
add42 1
add42 3

// create a new list by applying the add42 function
// to each element
[1;2;3] |> List.map add42

// create a "tester" by partial application of "less than"
let twoIsLessThan = (<) 2   // partial application
twoIsLessThan 1
twoIsLessThan 3

// filter each element with the twoIsLessThan function
[1;2;3] |> List.filter twoIsLessThan

// create a "printer" by partial application of printfn
let printer = printfn "printing param=%i"

// loop over each element and call the printer function
[1;2;3] |> List.iter printer
```

在每种情况下，我们创建一个部分应用的函数，然后可以在多个上下文中重用。

当然，部分应用程序也可以很容易地涉及固定函数参数。以下是一些示例：

```F#
// an example using List.map
let add1 = (+) 1
let add1ToEach = List.map add1   // fix the "add1" function

// test
add1ToEach [1;2;3;4]

// an example using List.filter
let filterEvens =
  List.filter (fun i -> i%2 = 0) // fix the filter function

// test
filterEvens [1;2;3;4]
```

以下更复杂的示例显示了如何使用相同的方法来创建透明的“插件”行为。

- 我们创建了一个将两个数字相加的函数，但除此之外，还需要一个记录函数来记录这两个数字和结果。
- 日志函数有两个参数：（string）“name”和（generic）“value”，因此它有签名 `string->'a->unit`。
- 然后，我们创建日志功能的各种实现，例如控制台记录器或彩色记录器。
- 最后，我们部分应用 main 函数来创建新函数，这些函数中包含一个特定的记录器。

```F#
// create an adder that supports a pluggable logging function
let adderWithPluggableLogger logger x y =
  logger "x" x
  logger "y" y
  let result = x + y
  logger "x+y"  result
  result

// create a logging function that writes to the console
let consoleLogger argName argValue =
  printfn "%s=%A" argName argValue

//create an adder with the console logger partially applied
let addWithConsoleLogger = adderWithPluggableLogger consoleLogger
addWithConsoleLogger 1 2
addWithConsoleLogger 42 99

// create a logging function that uses red text
let redLogger argName argValue =
  let message = sprintf "%s=%A" argName argValue
  System.Console.ForegroundColor <- System.ConsoleColor.Red
  System.Console.WriteLine("{0}",message)
  System.Console.ResetColor()

//create an adder with the popup logger partially applied
let addWithRedLogger = adderWithPluggableLogger redLogger
addWithRedLogger 1 2
addWithRedLogger 42 99
```

这些内置记录器的功能可以像其他功能一样使用。例如，我们可以创建一个部分应用程序来添加42，然后将其传递给列表函数，就像我们对简单的“`add42`”函数所做的那样。

```F#
// create a another adder with 42 baked in
let add42WithConsoleLogger = addWithConsoleLogger 42
[1;2;3] |> List.map add42WithConsoleLogger
[1;2;3] |> List.map add42               //compare without logger
```

这些部分应用的函数是一个非常有用的工具。我们可以创建灵活（但复杂）的库函数，同时使创建可重用的默认值变得容易，这样调用者就不必一直暴露在复杂性中。

### 为部分应用程序设计函数

您可以看到，参数的顺序对部分应用程序的易用性有很大影响。例如，`List` 库中的大多数函数，如 `List.map` 和 `List.filter`，都有类似的形式，即：

`List-function [function parameter(s)] [list]`

列表始终是最后一个参数。以下是完整表格的一些示例：

```F#
List.map    (fun i -> i+1) [0;1;2;3]
List.filter (fun i -> i>1) [0;1;2;3]
List.sortBy (fun i -> -i ) [0;1;2;3]
```

使用部分应用程序的相同示例：

```F#
let eachAdd1 = List.map (fun i -> i+1)
eachAdd1 [0;1;2;3]

let excludeOneOrLess = List.filter (fun i -> i>1)
excludeOneOrLess [0;1;2;3]

let sortDesc = List.sortBy (fun i -> -i)
sortDesc [0;1;2;3]
```

如果库函数以不同的顺序编写参数，那么在部分应用程序中使用它们会更加不便。

当你编写自己的多参数函数时，你可能会想知道最佳的参数顺序是什么。与所有设计问题一样，这个问题没有“正确”的答案，但这里有一些普遍接受的准则：

1. 放在前面：参数更有可能是静态的
2. 放在最后：数据结构或集合（或变化最大的参数）
3. 对于众所周知的操作，如“减法”，按预期顺序排列

准则1直截了当。最有可能在部分应用中“固定”的参数应该是第一个。我们在前面的记录器示例中看到了这一点。

准则2使将结构或集合从一个功能传输到另一个功能变得更加容易。我们已经在列表函数中多次看到了这一点。

```F#
// piping using list functions
let result =
  [1..10]
  |> List.map (fun i -> i+1)
  |> List.filter (fun i -> i>5)
// output => [6; 7; 8; 9; 10; 11]
```

同样，部分应用的列表函数很容易组合，因为列表参数本身很容易省略：

```F#
let f1 = List.map (fun i -> i+1)
let f2 = List.filter (fun i -> i>5)
let compositeOp = f1 >> f2 // compose
let result = compositeOp [1..10]
// output => [6; 7; 8; 9; 10; 11]
```

#### 包装BCL功能以供部分应用

这个 .NET 基类库函数在 F# 中很容易访问，但并不是真正为与 F# 等函数式语言一起使用而设计的。例如，大多数函数首先有data参数，而对于 F#，正如我们所看到的，data参数通常应该排在最后。

然而，为它们创建更符合习惯的包装器很容易。例如，在下面的代码片段中，.NET 字符串函数被重写，使字符串目标成为最后一个参数，而不是第一个参数：

```F#
// create wrappers for .NET string functions
let replace oldStr newStr (s:string) =
  s.Replace(oldValue=oldStr, newValue=newStr)

let startsWith (lookFor:string) (s:string) =
  s.StartsWith(lookFor)
```

一旦字符串成为最后一个参数，我们就可以以预期的方式将它们与管道一起使用：

```F#
let result =
  "hello"
  |> replace "h" "j"
  |> startsWith "j"

["the"; "quick"; "brown"; "fox"]
  |> List.filter (startsWith "f")
```

或具有函数组成：

```F#
let compositeOp = replace "h" "j" >> startsWith "j"
let result = compositeOp "hello"
```

#### 理解“管道”功能

现在您已经了解了部分应用程序的工作原理，您应该能够理解“管道”函数的工作原理。

管道功能定义为：

```F#
let (|>) x f = f x
```

它所做的只是允许您将函数参数放在函数前面，而不是后面。仅此而已。

```F#
let doSomething x y z = x+y+z
doSomething 1 2 3       // all parameters after function
```

如果函数有多个参数，那么输入似乎是最后一个参数。实际上，函数被部分应用，返回一个只有一个参数的函数：输入

下面是重写为使用部分应用程序的相同示例

```F#
let doSomething x y  =
  let intermediateFn z = x+y+z
  intermediateFn        // return intermediateFn

let doSomethingPartial = doSomething 1 2
doSomethingPartial 3     // only one parameter after function now
3 |> doSomethingPartial  // same as above - last parameter piped in
```

正如您已经看到的，管道操作符在F#中非常常见，并且一直用于保持自然流动。以下是您可能会看到的一些用法：

```F#
"12" |> int               // parses string "12" to an int
1 |> (+) 2 |> (*) 3       // chain of arithmetic
```

#### 反向管道功能

您可能偶尔会看到使用反向管道函数“<|”。

```F#
let (<|) f x = f x
```

这个函数似乎并没有做任何与正常函数不同的事情，那么它为什么存在呢？

原因是，当在中缀样式中用作二进制运算符时，它减少了对括号的需求，可以使代码更清晰。

```F#
printf "%i" 1+2          // error
printf "%i" (1+2)        // using parens
printf "%i" <| 1+2       // using reverse pipe
```

您还可以同时在两个方向上使用管道来获得伪中缀符号。

```F#
let add x y = x + y
(1+2) add (3+4)          // error
1+2 |> add <| 3+4        // pseudo infix
```

## 7 函数关联性和组合

*Part of the "Thinking functionally" series (*[link](https://fsharpforfunandprofit.com/posts/function-composition/#series-toc)*)*

从现有功能构建新功能
07五月2012 这篇文章是超过3年

### 函数关联性

如果我们有一排函数链，它们是如何组合的？

例如，这意味着什么？

```F#
let F x y z = x y z
```

这是否意味着将函数y应用于参数z，然后获取结果并将其用作x的参数？在这种情况下，它与以下内容相同：

```F#
let F x y z = x (y z)
```

或者，这是否意味着将函数x应用于参数y，然后获取结果函数并用参数z对其进行求值？在这种情况下，它与以下内容相同：

```F#
let F x y z = (x y) z
```

答案是后者。函数应用程序是左关联的。也就是说，评估 `x y z`与评估 `(x y) z` 是相同的。评估 `w x y z` 也与评估 `((w x) y) z` 相同。这并不奇怪。我们已经看到，这就是部分应用程序的工作原理。如果你把 x 看作一个双参数函数，那么 `(x y) z` 是第一个参数部分应用的结果，然后将 z 参数传递给中间函数。

如果您确实想进行正确的关联，可以使用显式括号，也可以使用管道。以下三种形式是等效的。

```F#
let F x y z = x (y z)
let F x y z = y z |> x    // using forward pipe
let F x y z = x <| y z    // using backward pipe
```

作为练习，在不实际评估的情况下计算出这些函数的签名！

### 函数组合

我们现在已经多次提到函数组合，但它实际上是什么意思？起初看起来很吓人，但实际上很简单。

假设你有一个从类型“T1”映射到类型“T2”的函数“f”，并且假设你还有一个从“T2”映射到“T3”类型的函数“g”。然后，您可以将“f”的输出连接到“g”的输入，创建一个从类型“T1”映射到类型“T3”的新函数。

这里是例子

```F#
let f (x:int) = float x * 3.0  // f is int->float
let g (x:float) = x > 4.0      // g is float->bool
```

我们可以创建一个新函数h，它接受“f”的输出并将其用作“g”的输入。

```F#
let h (x:int) =
    let y = f(x)
    g(y)                   // return output of g
```

一种更紧凑的方式是：

```F#
let h (x:int) = g ( f(x) ) // h is int->bool

//test
h 1
h 2
```

到目前为止，一切都很简单。有趣的是，我们可以定义一个名为“compose”的新函数，给定函数“f”和“g”，它以这种方式组合它们，甚至不知道它们的签名。

```F#
let compose f g x = g ( f(x) )
```

如果你对此进行评估，你会看到编译器已经正确地推断出，如果“`f`”是一个从泛型类型 `'a` 到泛型类型 `'b` 的函数，那么“`g`”被约束为具有泛型类型 `'b` 作为输入。总体签名为：

```F#
val compose : ('a -> 'b) -> ('b -> 'c) -> 'a -> 'c
```

（请注意，这种通用组合操作之所以可能，是因为每个函数都有一个输入和一个输出。这种方法在非函数式语言中是不可能的。）

正如我们所看到的，compose 的实际定义使用了“`>>`”符号。

```F#
let (>>) f g x = g ( f(x) )
```

根据这个定义，我们现在可以使用组合从现有函数构建新函数。

```F#
let add1 x = x + 1
let times2 x = x * 2
let add1Times2 x = (>>) add1 times2 x

//test
add1Times2 3
```

这种露骨的风格相当杂乱。我们可以做一些事情来让它更容易使用和理解。

首先，我们可以省略x参数，以便组合运算符返回部分应用程序。

```F#
let add1Times2 = (>>) add1 times2
```

现在我们有一个二元运算，所以我们可以把运算符在中间。

```F#
let add1Times2 = add1 >> times2
```

这就是了。使用组合运算符可以使代码更清晰、更直接。

```F#
let add1 x = x + 1
let times2 x = x * 2

//old style
let add1Times2 x = times2(add1 x)

//new style
let add1Times2 = add1 >> times2
```

### 在实践中使用组合运算符

组合运算符（与所有中缀运算符一样）的优先级低于普通函数应用程序。这意味着组合中使用的函数可以有参数，而不需要使用括号。

例如，如果“add”和“times”函数有一个额外的参数，则可以在组合过程中传递该参数。

```F#
let add n x = x + n
let times n x = x * n
let add1Times2 = add 1 >> times 2
let add5Times3 = add 5 >> times 3

//test
add5Times3 1
```

只要输入和输出匹配，所涉及的函数就可以使用任何类型的值。例如，考虑以下情况，它执行一个函数两次：

```F#
let twice f = f >> f    //signature is ('a -> 'a) -> ('a -> 'a)
```

请注意，编译器已经推断出函数f必须对输入和输出使用相同的类型。

现在考虑一个像“`+`”这样的函数。正如我们之前看到的，输入是一个 `int`，但输出实际上是一个部分应用的函数（`int->int`）。因此，“`+`”的输出可以用作“`twice`”的输入。所以我们可以写这样的东西：

```F#
let add1 = (+) 1           // signature is (int -> int)
let add1Twice = twice add1 // signature is also (int -> int)

//test
add1Twice 9
```

另一方面，我们不能写这样的东西：

```F#
let addThenMultiply = (+) >> (*)
```

因为“*”的输入必须是 `int` 值，而不是 `int->int` 函数（这是加法的输出）。

但如果我们调整它，使第一个函数的输出仅为 `int`，那么它确实有效：

```F#
let add1ThenMultiply = (+) 1 >> (*)
// (+) 1 has signature (int -> int) and output is an 'int'

//test
add1ThenMultiply 2 7
```

如果需要，也可以使用“`<<`”运算符反向组合。

```F#
let times2Add1 = add 1 << times 2
times2Add1 3
```

反向组合主要用于使代码更像英语。例如，这里有一个简单的例子：

```F#
let myList = []
myList |> List.isEmpty |> not    // straight pipeline

myList |> (not << List.isEmpty)  // using reverse composition
```

### 组合 vs 管道

此时，您可能想知道组合运算符和管道运算符之间的区别是什么，因为它们看起来非常相似。

首先，让我们再次看看管道操作符的定义：

```F#
let (|>) x f = f x
```

它所做的只是允许您将函数参数放在函数前面，而不是后面。仅此而已。如果函数有多个参数，那么输入将是最后一个参数。这是前面使用的示例。

```F#
let doSomething x y z = x+y+z
doSomething 1 2 3       // all parameters after function
3 |> doSomething 1 2    // last parameter piped in
```

组合不是一回事，不能代替管道。在以下情况下，数字3甚至不是一个函数，因此它的“输出”不能馈入`doSomething`：

```F#
3 >> doSomething 1 2     // not allowed
// f >> g is the same as  g(f(x)) so rewriting it we have:
doSomething 1 2 ( 3(x) ) // implies 3 should be a function!
// error FS0001: This expression was expected to have type 'a->'b
//               but here has type int
```

编译器抱怨“3”应该是某种函数 `'a->'b`。

与此相比，组合的定义需要 3 个参数，其中前两个必须是函数。

```F#
let (>>) f g x = g ( f(x) )

let add n x = x + n
let times n x = x * n
let add1Times2 = add 1 >> times 2
```

尝试使用管道是行不通的。在下面的例子中，“`add 1`”是一个 `int->int` 类型的（部分）函数，不能用作“`times 2`”的第二个参数。

```F#
let add1Times2 = add 1 |> times 2   // not allowed
// x |> f is the same as  f(x) so rewriting it we have:
let add1Times2 = times 2 (add 1)    // add1 should be an int
// error FS0001: Type mismatch. 'int -> int' does not match 'int'
```

编译器抱怨“`times 2`”应该接受一个 `int->int` 参数，即类型为 `(int->int) ->'a`。

## 8 定义函数

*Part of the "Thinking functionally" series (*[link](https://fsharpforfunandprofit.com/posts/defining-functions/#series-toc)*)*

Lambdas和更多
08五月2012 这篇文章是超过3年

https://fsharpforfunandprofit.com/posts/defining-functions/

我们已经看到了如何使用“let”语法创建典型函数，如下所示：

```F#
let add x y = x + y
```

在本节中，我们将介绍创建函数的其他方法以及定义函数的技巧。

### 匿名函数（又名lambdas）

如果你熟悉其他语言中的 lambdas，这对你来说并不陌生。匿名函数（或“lambda 表达式”）使用以下形式定义：

```F#
fun parameter1 parameter2 etc -> expression
```

如果你习惯 C# 中的 lambdas，有几个不同之处：

- lambda 必须有特殊的关键字 `fun`，这在 C# 版本中是不需要的
- 箭头符号是一个单箭头 `->`，而不是 C# 中的双箭头（`=>`）。

下面是一个定义加法的 lambda：

```F#
let add = fun x y -> x + y
```

这与更传统的函数定义完全相同：

```F#
let add x y = x + y
```

当你有一个简短的表达式，并且不想只为该表达式定义一个函数时，通常会使用 Lambdas。正如我们已经看到的，这在列表操作中尤其常见。

```F#
// with separately defined function
let add1 i = i + 1
[1..10] |> List.map add1

// inlined without separately defined function
[1..10] |> List.map (fun i -> i + 1)
```

请注意，必须在 lambda 周围使用括号。

当你想清楚地表明你是从另一个函数返回一个函数时，也会使用 Lambdas。例如，我们之前讨论的“`adderGenerator`”函数可以用lambda重写。

```F#
// original definition
let adderGenerator x = (+) x

// definition using lambda
let adderGenerator x = fun y -> x + y
```

lambda 版本稍长，但明确表示返回的是一个中间函数。

你也可以嵌套 lambda。这是 `adderGenerator` 的另一个定义，这次只使用 lambdas。

```F#
let adderGenerator = fun x -> (fun y -> x + y)
```

你能看出以下三个定义都是一样的吗？

```F#
let adderGenerator1 x y = x + y
let adderGenerator2 x   = fun y -> x + y
let adderGenerator3     = fun x -> (fun y -> x + y)
```

如果你看不到，那么一定要重读这篇关于柯里化的文章。这是需要了解的重要内容！

### 参数模式匹配

在定义函数时，您可以传递一个显式参数，正如我们所看到的，但您也可以在参数部分直接进行模式匹配。换句话说，参数部分可以包含模式，而不仅仅是标识符！

以下示例演示了如何在函数定义中使用模式：

```F#
type Name = {first:string; last:string} // define a new type
let bob = {first="bob"; last="smith"}   // define a value

// single parameter style
let f1 name =                       // pass in single parameter
   let {first=f; last=l} = name     // extract in body of function
   printfn "first=%s; last=%s" f l

// match in the parameter itself
let f2 {first=f; last=l} =          // direct pattern matching
   printfn "first=%s; last=%s" f l

// test
f1 bob
f2 bob
```

这种匹配只有在匹配总是可能的情况下才能发生。例如，您无法以这种方式在联合类型或列表上进行匹配，因为某些情况可能不匹配。

```F#
let f3 (x::xs) =            // use pattern matching on a list
   printfn "first element is=%A" x
```

您将收到关于不完整模式匹配的警告。

### 一个常见的错误：元组与多个参数

如果你来自类C语言，用作单个函数参数的元组可能看起来非常像多个参数。它们根本不是一回事！正如我之前提到的，如果你看到一个逗号，它可能是元组的一部分。参数之间用空格分隔。

下面是一个混淆的例子：

```F#
// a function that takes two distinct parameters
let addTwoParams x y = x + y

// a function that takes a single tuple parameter
let addTuple aTuple =
   let (x,y) = aTuple
   x + y

// another function that takes a single tuple parameter
// but looks like it takes two ints
let addConfusingTuple (x,y) = x + y
```

- 第一个定义“`addTwoParams`”接受两个参数，用空格分隔。
- 第二个定义“`addTuple`”只接受一个参数。然后，它将“x”和“y”绑定到元组内部并进行加法。
- 第三个定义“`addConfusingTuple`”与“`addTuple`”一样只接受一个参数，但棘手的是，元组是使用模式匹配作为参数定义的一部分进行解包和绑定的。在幕后，它与“`addTuple`”完全相同。

让我们看看签名（如果你不确定，最好看看签名）

```F#
val addTwoParams : int -> int -> int        // two params
val addTuple : int * int -> int             // tuple->int
val addConfusingTuple : int * int -> int    // tuple->int
```

现在让我们使用它们：

```F#
//test
addTwoParams 1 2      // ok - uses spaces to separate args
addTwoParams (1,2)    // error trying to pass a single tuple
//   => error FS0001: This expression was expected to have type
//                    int but here has type 'a * 'b
```

在这里，我们可以看到在上述第二种情况下发生了错误。

首先，编译器将 `(1,2)` 视为 `('a * 'b)` 类型的泛型元组，它试图将其作为第一个参数传递给“`addTwoParams`”。然后它抱怨 `addTwoParams` 的第一个参数是一个 `int`，我们试图传递一个元组。

要创建元组，请使用逗号！以下是正确操作的方法：

```F#
addTuple (1,2)           // ok
addConfusingTuple (1,2)  // ok

let x = (1,2)
addTuple x               // ok

let y = 1,2              // it's the comma you need,
                         // not the parentheses!
addTuple y               // ok
addConfusingTuple y      // ok
```

相反，如果你试图向一个需要元组的函数传递多个参数，你也会得到一个模糊的错误。

```F#
addConfusingTuple 1 2    // error trying to pass two args
// => error FS0003: This value is not a function and
//                  cannot be applied
```

在这种情况下，编译器认为，由于您传递了两个参数，`addConfucingTuple` 必须是可 curryable 的。因此，“`addConfucingTuple 1`”将是一个返回另一个中间函数的部分应用程序。试图用“2”应用该中间函数会出错，因为没有中间函数！我们在关于currying的帖子中看到了完全相同的错误，当时我们讨论了参数过多可能导致的问题。

#### 为什么不使用元组作为参数？

上面对元组问题的讨论表明，还有另一种方法可以定义具有多个参数的函数：与其单独传递它们，不如将所有参数组合成一个复合数据结构。在下面的示例中，该函数接受一个参数，即一个包含三个项的元组。

```F#
let f (x,y,z) = x + y * z
// type is int * int * int -> int

// test
f (1,2,3)
```

请注意，函数签名不同于真正的三参数函数。只有一个箭头，所以只有一个参数，星号表示这是一个 `(int * int * int)` 的元组。

我们什么时候想使用元组参数而不是单个参数？

- 当元组本身有意义时。例如，如果我们使用三维坐标，三元组可能比三个单独的维度更方便。
- 元组偶尔用于将数据捆绑在一个应该保存在一起的单一结构中。例如，.NET 库中的 `TryParse` 函数返回结果和一个布尔值作为元组。但是，如果您有大量数据作为捆绑包保存在一起，那么您可能需要定义一个记录或类类型来存储它。

#### 一个特例：元组和 .NET 库函数

逗号经常出现的一个领域是调用 .NET库函数时！

这些都采用类似元组的参数，因此这些调用看起来与 C# 中的调用完全相同：

```F#
// correct
System.String.Compare("a","b")

// incorrect
System.String.Compare "a" "b"
```

原因是 .NET 库函数不是 curried 的，不能部分应用。所有参数都必须始终传入，使用类似元组的方法是实现这一点的明显方法。

但请注意，尽管这些调用看起来像元组，但它们实际上是一个特例。不能使用真的元组，因此以下代码无效：

```F#
let tuple = ("a","b")
System.String.Compare tuple   // error

System.String.Compare "a","b" // error
```

如果你想部分应用 .NET 库函数，为它们编写包装器函数通常很简单，正如我们前面看到的，如下所示：

```F#
// create a wrapper function
let strCompare x y = System.String.Compare(x,y)

// partially apply it
let strCompareWithB = strCompare "B"

// use it with a higher order function
["A";"B";"C"]
|> List.map strCompareWithB
```

### 单独参数与分组参数的指南

关于元组的讨论将我们引向一个更一般的话题：函数参数何时应该分开，何时应该分组？

请注意，F# 在这方面与 C# 不同。在 C# 中，所有的参数都是提供的，所以这个问题甚至不会出现！在 F# 中，由于部分应用，可能只提供了一些参数，因此您需要区分需要组合在一起的参数和独立的参数。

以下是在设计自己的函数时如何构造参数的一些一般准则。

- 一般来说，最好使用单独的参数，而不是将它们作为单个结构（如元组或记录）传递。这允许更灵活的行为，如部分应用。
- 但是，当一组参数必须同时设置时，请使用某种分组机制。

换句话说，在设计函数时，问问自己“我可以单独提供这个参数吗？”如果答案是否定的，那么应该对参数进行分组。

让我们来看一些例子：

```F#
// Pass in two numbers for addition.
// The numbers are independent, so use two parameters
let add x y = x + y

// Pass in two numbers as a geographical co-ordinate.
// The numbers are dependent, so group them into a tuple or record
let locateOnMap (xCoord,yCoord) = // do something

// Set first and last name for a customer.
// The values are dependent, so group them into a record.
type CustomerName = {First:string; Last:string}
let setCustomerName aCustomerName = // good
let setCustomerName first last = // not recommended

// Set first and last name and and pass the
// authorizing credentials as well.
// The name and credentials are independent, keep them separate
let setCustomerName myCredentials aName = //good
```

最后，一定要对参数进行适当的排序，以帮助部分应用（请参阅前面文章中的指导方针）。例如，在上面的最后一个函数中，为什么我把 `myCredentials` 参数放在 `aName` 参数之前？

### 无参数功能

有时我们可能希望函数根本不接受任何参数。例如，我们可能需要一个可以重复调用的“hello world”函数。正如我们在上一节中看到的，幼稚的定义是行不通的。

```F#
let sayHello = printfn "Hello World!"     // not what we want
```

修复方法是在函数中添加一个单元参数，或使用 lambda。

```F#
let sayHello() = printfn "Hello World!"           // good
let sayHello = fun () -> printfn "Hello World!"   // good
```

然后，必须始终使用 unit 参数调用函数：

```F#
// call it
sayHello()
```

这在 .NET 库以下情况中尤为常见。一些例子是：

```F#
Console.ReadLine()
System.Environment.GetCommandLineArgs()
System.IO.Directory.GetCurrentDirectory()
```

记得用 unit 参数调用它们！

### 定义新操作符

您可以定义使用一个或多个运算符符号命名的函数（有关可以使用的确切符号列表，请参阅F#文档）：

```F#
// define
let (.*%) x y = x + y + 1
```

定义符号时，必须在符号周围使用括号。

请注意，对于以 `*` 开头的自定义运算符，需要空格；否则 `(*` 被解释为评论的开头：

```F#
let ( *+* ) x y = x + y + 1
```

定义后，新函数可以以正常方式使用，符号周围也有括号：

```F#
let result = (.*%) 2 3
```

如果函数恰好有两个参数，则可以将其用作不带括号的中缀运算符。

```F#
let result = 2 .*% 3
```

您还可以定义以 `!` 或 `~` 开头的前缀运算符（有一些限制——请参阅F#关于运算符重载的文档）

```F#
let (~%%) (s:string) = s.ToCharArray()

//use
let result = %% "hello"
```

在 F# 中，创建自己的运算符是很常见的，许多库都会导出名为 `>=>` 和 `<*>` 的运算符。

### 无点风格

我们已经看到了许多省略函数最后一个参数以减少混乱的例子。这种风格被称为**无点风格**（point-free style）或**隐性编程**（tacit programming）。

以下是一些示例：

```F#
let add x y = x + y   // explicit
let add x = (+) x     // point free

let add1Times2 x = (x + 1) * 2    // explicit
let add1Times2 = (+) 1 >> (*) 2   // point free

let sum list = List.reduce (fun sum e -> sum+e) list // explicit
let sum = List.reduce (+)                            // point free
```

这种风格有利有弊。

从好的方面来看，它将注意力集中在高级功能组合上，而不是低级对象上。例如，“`(+) 1 >> (*) 2`”显然是一个加法运算，然后是乘法。“`List.reduce (+)`”明确表示加号操作是关键，而不需要知道它实际应用于的列表。

无点有助于阐明底层算法并揭示代码之间的共性——上面使用的“`reduce`”函数就是一个很好的例子——它将在计划中的列表处理系列中进行讨论。

另一方面，过多的无点风格会导致代码混乱。显式参数可以作为一种文档形式，它们的名称（如“list”）清楚地表明了函数的作用。

与编程中的任何事情一样，最好的指导方针是使用最清晰的方法。

### 组合子

“**组合子**”一词用于描述其结果仅取决于其参数的函数。这意味着不依赖于外部世界，特别是根本无法访问其他函数或全局值。

在实践中，这意味着组合子函数仅限于以各种方式组合其参数。

我们已经看到了一些组合子：“管道”运算符和“组合”运算符。如果你看看它们的定义，很明显，它们所做的只是以各种方式重新排序参数

```F#
let (|>) x f = f x             // forward pipe
let (<|) f x = f x             // reverse pipe
let (>>) f g x = g (f x)       // forward composition
let (<<) g f x = g (f x)       // reverse composition
```

另一方面，像“printf”这样的函数虽然很原始，但不是组合子，因为它依赖于外部世界（I/O）。

#### 组合鸟

组合子是逻辑的一个完整分支（自然称为“组合逻辑”）的基础，该分支比计算机和编程语言早很多年被发明。组合逻辑对函数式编程产生了很大的影响。

要了解更多关于组合子和组合逻辑的知识，我推荐雷蒙德·斯穆里安的《嘲笑一只知更鸟》一书。在书中，他描述了许多其他组合子，并异想天开地给它们起了鸟的名字。以下是一些标准组合子及其鸟名的示例：

```F#
let I x = x                // identity function, or the Idiot bird
let K x y = x              // the Kestrel
let M x = x >> x           // the Mockingbird
let T x y = y x            // the Thrush (this looks familiar!)
let Q x y z = y (x z)      // the Queer bird (also familiar!)
let S x y z = x z (y z)    // The Starling
// and the infamous...
let rec Y f x = f (Y f) x  // Y-combinator, or Sage bird
```

字母名称非常标准，所以如果你提到“K组合子”，每个人都会熟悉这个术语。

事实证明，许多常见的编程模式都可以用这些标准组合子来表示。例如，Kestrel是流畅界面中的一种常见模式，在这种模式下，你做了一些事情，但随后返回了原始对象。Thrush是管道操作，Queer bird是正向组合，Y组合子被用来使函数递归。

事实上，有一个众所周知的定理指出，任何可计算函数都可以仅由两个基本组合子Kestrel和Starling构建。

#### Combinator库

组合子库是一个代码库，它导出一组旨在协同工作的组合子函数。然后，库的用户可以轻松地将简单的功能组合在一起，制作更大、更复杂的功能，比如用乐高积木建造。

一个设计良好的组合子库允许您专注于高级操作，并将低级“噪声”推到后台。我们在“为什么使用F#”系列的例子中已经看到了一些这种能力的例子，`List` 模块中充满了这些例子——如果你仔细想想，“`fold`”和“`map`”函数也是组合子。

组合子的另一个优点是它们是最安全的函数类型。由于他们不依赖外部世界，因此如果全局环境发生变化，他们也无法改变。如果上下文不同，读取全局值或使用库函数的函数可以在调用之间中断或更改。组合子永远不会发生这种情况。

在 F# 中，组合子库可用于解析（FParsec 库）、HTML 构造、测试框架等。我们将在后面的系列文章中进一步讨论和使用组合子。

### 递归函数

通常，一个函数需要在其主体中引用自己。经典的例子是斐波那契函数：

```F#
let fib i =
   match i with
   | 1 -> 1
   | 2 -> 1
   | n -> fib(n-1) + fib(n-2)
```

递归函数和数据结构在函数式编程中非常常见，我希望在以后的系列文章中专门讨论这个主题。

## 9 函数签名

*Part of the "Thinking functionally" series (*[link](https://fsharpforfunandprofit.com/posts/function-signatures/#series-toc)*)*

函数签名可以让你对它的作用有所了解
09五月2012 这篇文章是超过3年

https://fsharpforfunandprofit.com/posts/function-signatures/

这可能并不明显，但 F# 实际上有两种语法——一种用于普通（值）表达式，一种用于类型定义。例如：

```F#
[1;2;3]      // a normal expression
int list     // a type expression

Some 1       // a normal expression
int option   // a type expression

(1,"a")      // a normal expression
int * string // a type expression
```

类型表达式具有与普通表达式中使用的语法不同的特殊语法。当您使用交互式会话时，您已经看到了许多这样的例子，因为每个表达式的类型及其求值都已打印出来。

如您所知，F# 使用类型推断来推断类型，因此您通常不需要在代码中显式指定类型，特别是对于函数。但是，为了在 F# 中有效地工作，您确实需要了解类型语法，以便构建自己的类型、调试类型错误和理解函数签名。在这篇文章中，我们将重点介绍它在函数签名中的使用。

以下是一些使用类型语法的示例函数签名：

```F#
// expression syntax          // type syntax
let add1 x = x + 1            // int -> int
let add x y = x + y           // int -> int -> int
let print x = printf "%A" x   // 'a -> unit
System.Console.ReadLine       // unit -> string
List.sum                      // 'a list -> 'a
List.filter                   // ('a -> bool) -> 'a list -> 'a list
List.map                      // ('a -> 'b) -> 'a list -> 'b list
```

### 通过签名理解函数

仅仅通过检查函数的签名，你通常就可以对它的功能有所了解。让我们看一些例子，然后依次分析它们。

```F#
// function signature 1
int -> int -> int
```

此函数接受两个 `int` 参数并返回另一个，因此推测它是某种数学函数，如加法、减法、乘法或求幂。

```F#
// function signature 2
int -> unit
```

此函数接受一个 `int` 并返回一个 `unit`，这意味着该函数正在做一些重要的副作用。由于没有有用的返回值，副作用可能与写入IO有关，例如日志记录、写入文件或数据库，或类似的事情。

```F#
// function signature 3
unit -> string
```

此函数不接受任何输入，但返回一个 `string`，这意味着该函数正在凭空构造一个字符串！由于没有显式输入，该函数可能与读取（比如从文件中）或生成（比如随机字符串）有关。

```F#
// function signature 4
int -> (unit -> string)
```

此函数接受一个 `int`  输入并返回一个函数，该函数在调用时返回字符串。同样，该函数可能与读取或生成有关。输入可能会以某种方式初始化返回的函数。例如，输入可以是文件句柄，返回的函数类似于 `readline()`。或者，输入可以是随机字符串生成器的种子。我们不能确切地说，但我们可以做出一些有根据的猜测。

```F#
// function signature 5
'a list -> 'a
```

此函数接受某种类型的列表，但只返回该类型中的一个，这意味着该函数正在合并或从列表中选择元素。具有此签名的函数示例包括 `List.sum`、`List.max`、`List.head` 等。

```F#
// function signature 6
('a -> bool) -> 'a list -> 'a list
```

此函数有两个参数：第一个是将某物映射到 bool（谓词）的函数，第二个是列表。返回值是相同类型的列表。谓词用于确定一个值是否满足某种标准，因此看起来该函数是根据谓词是否为真从列表中选择元素，然后返回原始列表的一个子集。具有此签名的典型函数是 `List.filter`。

```F#
// function signature 7
('a -> 'b) -> 'a list -> 'b list
```

此函数有两个参数：第一个将类型 `'a` 映射到类型 `'b`，第二个是 `'a` 的列表。返回值是一个不同类型 `'b` 的列表。一个合理的猜测是，该函数将列表中的每个 `'a`，使用作为第一个参数传入的函数将它们映射到 `'b` 并返回新的 `'b` 列表。事实上，具有此签名的原型函数是 `List.map`。

#### 使用函数签名查找库方法

函数签名是搜索库函数的重要组成部分。F# 库中有数百个函数，最初可能会让人不知所措。与面向对象语言不同，你不能简单地“点入”一个对象来找到所有合适的方法。然而，如果你知道你正在寻找的函数的签名，你通常可以快速缩小候选列表的范围。

例如，假设你有两个列表，你正在寻找一个函数将它们组合成一个。这个函数的签名是什么？它将接受两个列表参数，并返回第三个，所有参数类型相同，并给出签名：

```F#
'a list -> 'a list -> 'a list
```

现在转到 F# List 模块的 MSDN 文档，向下扫描函数列表，查找匹配的内容。碰巧的是，只有一个函数具有该签名：

```F#
append : 'T list -> 'T list -> 'T list
```

这正是我们想要的！

### 为函数签名定义自己的类型

有时，您可能希望创建自己的类型来匹配所需的函数签名。您可以使用“type”关键字来实现这一点，并以与编写签名相同的方式定义类型：

```F#
type Adder = int -> int
type AdderGenerator = int -> Adder
```

然后，您可以使用这些类型来约束函数值和参数。

例如，由于类型约束，下面的第二个定义将失败。如果删除类型约束（如第三个定义中所示），则不会有任何问题。

```F#
let a:AdderGenerator = fun x -> (fun y -> x + y)
let b:AdderGenerator = fun (x:float) -> (fun y -> x + y)
let c                = fun (x:float) -> (fun y -> x + y)
```

### 测试你对函数签名的理解

你对函数签名的理解程度如何？看看是否可以创建具有这些签名的简单函数。避免使用显式类型注释！

```F#
val testA = int -> int
val testB = int -> int -> int
val testC = int -> (int -> int)
val testD = (int -> int) -> int
val testE = int -> int -> int -> int
val testF = (int -> int) -> (int -> int)
val testG = int -> (int -> int) -> int
val testH = (int -> int -> int) -> int
```



## 10 组织函数

*Part of the "Thinking functionally" series (*[link](https://fsharpforfunandprofit.com/posts/organizing-functions/#series-toc)*)*

嵌套函数和模块
2012年5月10日 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/organizing-functions/

既然你知道如何定义函数，那么你如何组织它们呢？

在 F# 中，有三个选项：

- 函数可以嵌套在其他函数中。
- 在应用程序级别，顶层功能被分组为“模块”。
- 或者，您也可以使用面向对象的方法，将函数作为方法附加到类型上。

我们将在本文中介绍前两个选项，在下一篇文章中介绍第三个选项。

### 嵌套的函数

在F#中，您可以在其他函数内定义函数。这是封装主函数所需但不应暴露在外部的“辅助”函数的好方法。

在下面的示例中，`add` 嵌套在 `addThreeNumbers` 中：

```F#
let addThreeNumbers x y z  =

    //create a nested helper function
    let add n =
       fun x -> x + n

    // use the helper function
    x |> add y |> add z

// test
addThreeNumbers 2 3 4
```

嵌套函数可以直接访问其父函数参数，因为它们在作用域内。因此，在下面的示例中，`printError` 嵌套函数不需要有自己的任何参数——它可以直接访问 `n` 和 `max` 值。

```F#
let validateSize max n  =

    //create a nested helper function with no params
    let printError() =
        printfn "Oops: '%i' is bigger than max: '%i'" n max

    // use the helper function
    if n > max then printError()

// test
validateSize 10 9
validateSize 10 11
```

一种非常常见的模式是，main 函数定义了一个嵌套的递归辅助函数，然后用适当的初始值调用它。下面的代码就是一个例子：

```F#
let sumNumbersUpTo max =

    // recursive helper function with accumulator
    let rec recursiveSum n sumSoFar =
        match n with
        | 0 -> sumSoFar
        | _ -> recursiveSum (n-1) (n+sumSoFar)

    // call helper function with initial values
    recursiveSum max 0

// test
sumNumbersUpTo 10
```

嵌套函数时，请尽量避免嵌套非常深的函数，特别是如果嵌套函数直接访问其父作用域中的变量，而不是将参数传递给它们。一个嵌套不好的函数会像最糟糕的深度嵌套命令式分支一样令人困惑。

以下是如何避免这样做：

```F#
// wtf does this function do?
let f x =
    let f2 y =
        let f3 z =
            x * z
        let f4 z =
            let f5 z =
                y * z
            let f6 () =
                y * x
            f6()
        f4 y
    x * f2 x
```

### 模块

模块只是一组组合在一起的函数，通常是因为它们处理相同的数据类型。

模块定义看起来很像函数定义。它以 `module` 关键字开头，然后是一个 `=` 符号，然后列出模块的内容。模块的内容必须缩进，就像函数定义中的表达式必须缩进一样。

这是一个包含两个函数的模块：

```F#
module MathStuff =

    let add x y  = x + y
    let subtract x y  = x - y
```

现在，如果您在 Visual Studio 中尝试此操作，并将鼠标悬停在 `add` 函数上，您将看到 `add` 函数的全名实际上是`MathStuff.add`，就像 `MathStuff` 是一个类，`add` 是一个方法一样。

实际上，这正是正在发生的事情。在幕后，F#编译器创建了一个具有静态方法的静态类。所以C#的等价物是：

```C#
static class MathStuff
{
    static public int add(int x, int y)
    {
        return x + y;
    }

    static public int subtract(int x, int y)
    {
        return x - y;
    }
}
```

如果你意识到模块只是静态类，函数是静态方法，那么你在理解模块在 F# 中的工作方式方面已经有了一个良好的开端，因为适用于静态类的大多数规则也适用于模块。

而且，就像在 C# 中每个独立函数都必须是类的一部分一样，在 F# 中每个独立功能都必须是模块的一部分。

#### 跨模块边界访问功能

如果你想访问另一个模块中的函数，你可以通过它的限定名来引用它。

```F#
module MathStuff =

    let add x y  = x + y
    let subtract x y  = x - y

module OtherStuff =

    // use a function from the MathStuff module
    let add1 x = MathStuff.add x 1
```

您还可以使用 open 指令导入另一个模块中的所有函数，之后您可以使用短名称，而不必指定限定名称。

```F#
module OtherStuff =
    open MathStuff  // make all functions accessible

    let add1 x = add x 1
```

使用限定名的规则与您所期望的完全一致。也就是说，您始终可以使用完全限定名来访问函数，并且可以根据范围内的其他模块使用相对名或非限定名。

#### 嵌套模块

就像静态类一样，模块可以包含嵌套在其中的子模块，如下所示：

```F#
module MathStuff =

    let add x y  = x + y
    let subtract x y  = x - y

    // nested module
    module FloatLib =

        let add x y :float = x + y
        let subtract x y :float  = x - y
```

其他模块可以根据需要使用全名或相对名称引用嵌套模块中的函数：

```F#
module OtherStuff =
    open MathStuff

    let add1 x = add x 1

    // fully qualified
    let add1Float x = MathStuff.FloatLib.add x 1.0

    //with a relative path
    let sub1Float x = FloatLib.subtract x 1.0
```

#### 顶级模块

因此，如果可以有嵌套的子模块，这意味着，在链的上游，必须始终有一些顶级父模块。这确实是真的。

顶层模块的定义与我们迄今为止看到的模块略有不同。

- `module MyModuleName` 行必须是文件中的第一个声明
- 没有 `=` 符号
- 模块的内容没有缩进

一般来说，每个 `.FS` 源文件中都必须有一个顶级模块声明。也有一些例外，但无论如何，这都是很好的做法。模块名称不必与文件名称相同，但两个文件不能共享相同的模块名称。

为了 `.FSX` 脚本文件，不需要模块声明，在这种情况下，模块名称会自动设置为脚本的文件名。

下面是一个声明为顶级模块的 `MathStuff` 示例：

```F#
// top level module
module MathStuff

let add x y  = x + y
let subtract x y  = x - y

// nested module
module FloatLib =

    let add x y :float = x + y
    let subtract x y :float  = x - y
```

请注意，顶层代码（`module MathStuff` 的内容）没有缩进，但像 `FloatLib` 这样的嵌套模块的内容仍然需要缩进。

#### 其他模块内容

一个模块可以包含其他声明和函数，包括类型声明、简单值和初始化代码（如静态构造函数）

```F#
module MathStuff =

    // functions
    let add x y  = x + y
    let subtract x y  = x - y

    // type definitions
    type Complex = {r:float; i:float}
    type IntegerFunction = int -> int -> int
    type DegreesOrRadians = Deg | Rad

    // "constant"
    let PI = 3.141

    // "variable"
    let mutable TrigType = Deg

    // initialization / static constructor
    do printfn "module initialized"
```

> 顺便说一句，如果你在交互式窗口中玩这些示例，你可能想每隔一段时间右键单击并执行“重置会话”，这样代码就新鲜了，不会被之前的评估所污染

#### 阴影

这是我们的示例模块。请注意，`MathStuff` 具有 `add` 函数，`FloatLib` 也具有 `add` 函数。

```F#
module MathStuff =

    let add x y  = x + y
    let subtract x y  = x - y

    // nested module
    module FloatLib =

        let add x y :float = x + y
        let subtract x y :float  = x - y
```

现在，如果我将两者都纳入范围，然后使用 `add`，会发生什么？

```F#
open  MathStuff
open  MathStuff.FloatLib

let result = add 1 2  // Compiler error: This expression was expected to
                      // have type float but here has type int
```

发生的事情就是 `MathStuff.FloatLib` 模块屏蔽或覆盖了原始的 `MathStuff` 模块，该模块已被 `FloatLib` “屏蔽”。

因此，您现在会得到一个 FS0001 编译器错误，因为第一个参数 `1` 应该是浮点数。您必须将 `1` 更改为 `1.0` 才能修复此问题。

不幸的是，这是看不见的，很容易被忽视。有时你可以用它做一些很酷的技巧，几乎像子类化，但更常见的是，如果你有同名函数（比如非常常见的 `map`），这可能会很烦人。

如果你不想发生这种情况，有一种方法可以通过使用 `RequireQualifiedAccess` 属性来阻止它。这是一个同样的例子，两个模块都用它来装饰。

```F#
[<RequireQualifiedAccess>]
module MathStuff =

    let add x y  = x + y
    let subtract x y  = x - y

    // nested module
    [<RequireQualifiedAccess>]
    module FloatLib =

        let add x y :float = x + y
        let subtract x y :float  = x - y
```

现在不允许 `open`：

```F#
open  MathStuff   // error
open  MathStuff.FloatLib // error
```

但我们仍然可以通过它们的限定名访问这些函数（没有任何歧义）：

```F#
let result = MathStuff.add 1 2
let result = MathStuff.FloatLib.add 1.0 2.0
```

#### 访问控制

F# 支持使用标准 .NET 访问控制关键字，如 `public`、`private` 和 `internal`。MSDN 文档提供了完整的详细信息。

- 这些访问说明符可以放在模块中的顶级（“let bound”）函数、值、类型和其他声明上。也可以为模块本身指定它们（例如，您可能需要一个私有嵌套模块）。
- 默认情况下，所有内容都是公开的（除了少数例外），因此如果你想保护它们，你需要使用 `private` 或 `internal`。

这些访问说明符只是 F# 中进行访问控制的一种方式。另一种完全不同的方法是使用模块“签名”文件，它有点像 C 头文件。它们以抽象的方式描述模块的内容。签名对于进行严肃的封装非常有用，但这种讨论将不得不等待计划中的封装和基于功能的安全系列。

### 命名空间

F#中的命名空间类似于C#中的命名空间。它们可用于组织模块和类型，以避免名称冲突。

使用 `namespace` 关键字声明命名空间，如下所示。

```F#
namespace Utilities

module MathStuff =

    // functions
    let add x y  = x + y
    let subtract x y  = x - y
```

由于这个命名空间，`MathStuff` 模块的完全限定名现在变成了 `Utilities.MathStuff` 和 `add` 函数的完全限定名现在变成了 `Utilities.MathStuff.add`。

对于命名空间，缩进规则适用，因此上面定义的模块必须缩进其内容，就像它是一个嵌套模块一样。

您还可以通过在模块名称中添加点来隐式声明命名空间。也就是说，上面的代码也可以写成：

```F#
module Utilities.MathStuff

// functions
let add x y  = x + y
let subtract x y  = x - y
```

`MathStuff` 模块的完全限定名仍然是 `Utilities.MathStuff`，但在这种情况下，该模块是一个顶级模块，内容不需要缩进。

使用名称空间时需要注意的一些其他事项：

- 命名空间对于模块是可选的。与C#不同，F#项目没有默认的命名空间，因此没有命名空间的顶级模块将处于全局级别。如果您计划创建可重用的库，请务必添加某种名称空间，以避免与其他库中的代码发生命名冲突。
- 命名空间可以直接包含类型声明，但不能包含函数声明。如前所述，所有函数和值声明都必须是模块的一部分。
- 最后，请注意，名称空间在脚本中不能很好地工作。例如，如果您试图将命名空间声明（如下面的命名空间实用程序）发送到交互式窗口，您将收到错误。

#### 命名空间层次结构

您可以通过简单地用句点分隔名称来创建命名空间层次结构：

```F#
namespace Core.Utilities

module MathStuff =
    let add x y  = x + y
```

如果你想把两个命名空间放在同一个文件中，你可以。请注意，所有命名空间都必须完全限定——没有嵌套。

```F#
namespace Core.Utilities

module MathStuff =
    let add x y  = x + y

namespace Core.Extra

module MoreMathStuff =
    let add x y  = x + y
```

你不能做的一件事是在命名空间和模块之间发生命名冲突。

```F#
namespace Core.Utilities

module MathStuff =
    let add x y  = x + y

namespace Core

// fully qualified name of module
// is Core.Utilities
// Collision with namespace above!
module Utilities =
    let add x y  = x + y
```

### 在模块中混合类型和功能

我们已经看到，一个模块通常由一组作用于数据类型的相关函数组成。

在面向对象程序中，数据结构和作用于它的函数将组合在一个类中。然而，在函数式F#中，数据结构和作用于它的函数被组合在一个模块中。

将类型和函数混合在一起有两种常见模式：

- 将类型与函数分开声明
- 在与函数相同的模块中声明类型

在第一种方法中，类型在任何模块外部（但在名称空间中）声明，然后将处理该类型的函数放入一个名称相似的模块中。

```F#
// top-level module
namespace Example

// declare the type outside the module
type PersonType = {First:string; Last:string}

// declare a module for functions that work on the type
module Person =

    // constructor
    let create first last =
        {First=first; Last=last}

    // method that works on the type
    let fullName {First=first; Last=last} =
        first + " " + last

// test
let person = Person.create "john" "doe"
Person.fullName person |> printfn "Fullname=%s"
```

在另一种方法中，类型在模块内声明，并给出一个简单的名称，如“`T`”或模块的名称。因此，这些函数是用 `MyModule.Func1` 和 `MyModule.Func2` 这样的名称访问的。而类型本身是用 `MyModule.T` 这样的名称访问的。举个例子：

```F#
module Customer =

    // Customer.T is the primary type for this module
    type T = {AccountId:int; Name:string}

    // constructor
    let create id name =
        {T.AccountId=id; T.Name=name}

    // method that works on the type
    let isValid {T.AccountId=id; } =
        id > 0

// test
let customer = Customer.create 42 "bob"
Customer.isValid customer |> printfn "Is valid?=%b"
```

请注意，在这两种情况下，您都应该有一个构造函数来创建该类型的新实例（如果您愿意，可以使用工厂方法）。这样做意味着您很少需要在客户端代码中显式命名该类型，因此，您不应该关心它是否存在于模块中！

那么，你应该选择哪种方法呢？

- 前一种方法更像 .NET，如果你想与其他非 F# 代码共享你的库，那就更好了，因为导出的类名就是你所期望的。
- 对于那些使用其他函数式语言的人来说，后一种方法更为常见。模块内的类型编译成嵌套类，这对互操作来说不是很好。

对你自己来说，你可能想尝试两者。在团队编程的情况下，你应该选择一种风格并保持一致。

#### 仅包含类型的模块

如果你有一组类型需要声明，而不需要任何相关函数，那么就不用费心使用模块。您可以直接在命名空间中声明类型，避免嵌套类。

例如，以下是您可能会想到的方法：

```F#
// top-level module
module Example

// declare the type inside a module
type PersonType = {First:string; Last:string}

// no functions in the module, just types...
```

这里有一种替代方法。`module` 关键字已被 `namespace` 替换。

```F#
// use a namespace
namespace Example

// declare the type outside any module
type PersonType = {First:string; Last:string}
```

在这两种情况下，`PersonType` 将具有相同的完全限定名。

请注意，这仅适用于类型。函数必须始终位于模块中。

## 11 将函数附加到类型

*Part of the "Thinking functionally" series (*[link](https://fsharpforfunandprofit.com/posts/type-extensions/#series-toc)*)*

以F#方式创建方法
2012年5月11日 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/type-extensions/

尽管到目前为止我们一直关注纯函数式风格，但有时切换到面向对象风格是方便的。OO 风格的一个关键特征是能够将函数附加到类中，并“点入”类以获得所需的行为。

在 F# 中，这是使用一个名为“类型扩展”的功能完成的。任何 F# 类型，而不仅仅是类，都可以附加函数。

下面是一个将函数附加到记录类型的示例。

```F#
module Person =
    type T = {First:string; Last:string} with
        // member defined with type declaration
        member this.FullName =
            this.First + " " + this.Last

    // constructor
    let create first last =
        {First=first; Last=last}

// test
let person = Person.create "John" "Doe"
let fullname = person.FullName
```

需要注意的关键事项是：

- `with` 关键字表示成员列表的开始
- `member` 关键字表示这是一个成员函数（即方法）
- 单词 `this` 是被点入的对象的占位符（称为“自我标识符”）。占位符作为函数名称的前缀，然后函数体在需要引用当前实例时使用相同的占位符。没有要求使用特定的单词，只要它是一致的。你可以用 `this` 或 `self` 或 `me`，或者任何其他通常表示自我参照的词。

您不必在声明类型的同时添加成员，您始终可以稍后在同一模块中添加它：

```F#
module Person =
    type T = {First:string; Last:string} with
       // member defined with type declaration
        member this.FullName =
            this.First + " " + this.Last

    // constructor
    let create first last =
        {First=first; Last=last}

    // another member added later
    type T with
        member this.SortableName =
            this.Last + ", " + this.First
// test
let person = Person.create "John" "Doe"
let fullname = person.FullName
let sortableName = person.SortableName
```

这些例子展示了所谓的“内在扩展”。它们被编译成类型本身，并且在使用该类型时始终可用。当你使用反射时，它们也会出现。

使用内部扩展，甚至可以有一个跨多个文件划分的类型定义，只要所有组件使用相同的命名空间并全部编译到相同的程序集中。就像C#中的分部类一样，这对于将生成的代码与编写的代码分开非常有用。

### 可选扩展

另一种选择是，您可以从完全不同的模块中添加额外的成员。这些被称为“可选扩展”。它们不会被编译成类型本身，并且需要一些其他模块在范围内才能工作（这种行为就像C#扩展方法一样）。

例如，假设我们定义了 `Person` 类型：

```F#
module Person =
    type T = {First:string; Last:string} with
       // member defined with type declaration
        member this.FullName =
            this.First + " " + this.Last

    // constructor
    let create first last =
        {First=first; Last=last}

    // another member added later
    type T with
        member this.SortableName =
            this.Last + ", " + this.First
```

下面的示例演示了如何在不同的模块中向其添加 `UppercaseName` 扩展：

```F#
// in a different module
module PersonExtensions =

    type Person.T with
    member this.UppercaseName =
        this.FullName.ToUpper()
```

现在让我们测试一下这个扩展：

```F#
let person = Person.create "John" "Doe"
let uppercaseName = person.UppercaseName
```

哦，我们出了点差错。问题是 `PersonExtensions` 不在作用域内。就像 C# 一样，任何扩展都必须进入范围才能使用。

一旦我们做到了，一切都会好起来的：

```F#
// bring the extension into scope first!
open PersonExtensions

let person = Person.create "John" "Doe"
let uppercaseName = person.UppercaseName
```

### 扩展系统类型

您可以扩展中的类型 .NET 库也是如此。但请注意，在扩展类型时，必须使用实际的类型名称，而不是类型缩写。

例如，如果你试图扩展 `int`，你会失败，因为 `int` 不是该类型的真实名称：

```F#
type int with
    member this.IsEven = this % 2 = 0
```

您必须改用 `System.Int32`：

```F#
type System.Int32 with
    member this.IsEven = this % 2 = 0

//test
let i = 20
if i.IsEven then printfn "'%i' is even" i
```

### 静态成员

您可以通过以下方式使成员函数静态：

- 添加关键字 `static`
- 删除 `this` 占位符

```F#
module Person =
    type T = {First:string; Last:string} with
        // member defined with type declaration
        member this.FullName =
            this.First + " " + this.Last

        // static constructor
        static member Create first last =
            {First=first; Last=last}

// test
let person = Person.T.Create "John" "Doe"
let fullname = person.FullName
```

您也可以为系统类型创建静态成员：

```F#
type System.Int32 with
    static member IsOdd x = x % 2 = 1

type System.Double with
    static member Pi = 3.141

//test
let result = System.Int32.IsOdd 20
let pi = System.Double.Pi
```

### 附加现有功能

一种非常常见的模式是将预先存在的独立函数附加到类型上。这有几个好处：

- 在开发过程中，您可以创建引用其他独立函数的独立函数。这使得编程更容易，因为类型推理在函数式代码中比在OO风格（“点入”）代码中工作得更好。
- 但对于某些关键函数，您也可以将它们附加到类型上。这让客户可以选择是使用函数式风格还是面向对象风格。

F# 库中的一个例子是计算列表长度的函数。它可以作为 `List` 模块中的独立函数使用，也可以作为列表实例上的方法使用。

```F#
let list = [1..10]

// functional style
let len1 = List.length list

// OO style
let len2 = list.Length
```

在下面的示例中，我们从一个最初没有成员的类型开始，然后定义一些函数，最后将 `fullName` 函数附加到该类型。

```F#
module Person =
    // type with no members initially
    type T = {First:string; Last:string}

    // constructor
    let create first last =
        {First=first; Last=last}

    // standalone function
    let fullName {First=first; Last=last} =
        first + " " + last

    // attach preexisting function as a member
    type T with
        member this.FullName = fullName this

// test
let person = Person.create "John" "Doe"
let fullname = Person.fullName person  // functional style
let fullname2 = person.FullName        // OO style
```

独立的 `fullName` 函数有一个参数，即 person。在附加的成员中，参数来自此自引用。

#### 附加具有多个参数的现有函数

一件好事是，当之前定义的函数有多个参数时，只要 `this` 参数是第一个，在执行附件时就不必重新指定所有参数。

在下面的示例中，`hasSameFirstAndLastName` 函数有三个参数。然而，当我们附上它时，我们只需要指定一个！

```F#
module Person =
    // type with no members initially
    type T = {First:string; Last:string}

    // constructor
    let create first last =
        {First=first; Last=last}

    // standalone function
    let hasSameFirstAndLastName (person:T) otherFirst otherLast =
        person.First = otherFirst && person.Last = otherLast

    // attach preexisting function as a member
    type T with
        member this.HasSameFirstAndLastName = hasSameFirstAndLastName this

// test
let person = Person.create "John" "Doe"
let result1 = Person.hasSameFirstAndLastName person "bob" "smith" // functional style
let result2 = person.HasSameFirstAndLastName "bob" "smith" // OO style
```

为什么这行得通？提示：考虑currying和部分应用！

### 元组形式方法

当我们开始使用具有多个参数的方法时，我们必须做出决定：

- 我们可以使用标准（curried）形式，其中参数用空格分隔，并支持部分应用程序。
- 我们可以在一个元组中一次性传递所有参数，以逗号分隔。

“curried”形式更具功能性，“tuple”形式更面向对象。

元组形式也是 F# 与标准 .NET 库交互的方式，所以让我们更详细地研究一下这种方法。

作为测试平台，这里有一个带有两个方法的产品类型，每个方法都使用其中一种方法实现。`CurriedTotal` 和 `TupleTotal` 方法都做同样的事情：计算给定数量和折扣的总价。

```F#
type Product = {SKU:string; Price: float} with

    // curried style
    member this.CurriedTotal qty discount =
        (this.Price * float qty) - discount

    // tuple style
    member this.TupleTotal(qty,discount) =
        (this.Price * float qty) - discount
```

以下是一些测试代码：

```F#
let product = {SKU="ABC"; Price=2.0}
let total1 = product.CurriedTotal 10 1.0
let total2 = product.TupleTotal(10,1.0)
```

到目前为止没有区别。

我们知道curried版本可以部分应用：

```F#
let totalFor10 = product.CurriedTotal 10
let discounts = [1.0..5.0]
let totalForDifferentDiscounts
    = discounts |> List.map totalFor10
```

但是元组方法可以做一些curried方法做不到的事情，即：

- 命名参数
- 可选参数
- 重载

#### 带元组样式参数的命名参数

元组样式方法支持命名参数：

```F#
let product = {SKU="ABC"; Price=2.0}
let total3 = product.TupleTotal(qty=10,discount=1.0)
let total4 = product.TupleTotal(discount=1.0, qty=10)
```

如您所见，当使用名称时，可以更改参数顺序。

注意：如果某些参数有名称，而另一些没有，则命名的参数必须始终是最后一个。

#### 带元组样式参数的可选参数

对于元组风格的方法，您可以通过在参数名称前加上问号来指定可选参数。

- 如果设置了参数，它将显示为 `Some value`
- 如果未设置参数，则显示为 `None`

这里有一个例子：

```F#
type Product = {SKU:string; Price: float} with

    // optional discount
    member this.TupleTotal2(qty,?discount) =
        let extPrice = this.Price * float qty
        match discount with
        | None -> extPrice
        | Some discount -> extPrice - discount
```

这里有一个测试：

```F#
let product = {SKU="ABC"; Price=2.0}

// discount not specified
let total1 = product.TupleTotal2(10)

// discount specified
let total2 = product.TupleTotal2(10,1.0)
```

`None` 和 `Some` 的显式匹配可能很乏味，处理可选参数有一个稍微优雅的解决方案。

有一个函数 `defaultArg`，它将参数作为第一个参数，将默认值作为第二个参数。如果设置了参数，则返回值。如果没有，则返回默认值。

让我们看看重写相同的代码以使用 `defaultArg`

```F#
type Product = {SKU:string; Price: float} with

    // optional discount
    member this.TupleTotal2(qty,?discount) =
        let extPrice = this.Price * float qty
        let discount = defaultArg discount 0.0
        //return
        extPrice - discount
```

#### 函数重载

在 C# 中，你可以有多个同名方法，它们的不同之处仅在于函数签名（例如不同的参数类型和/或参数数量）

在纯函数模型中，这是没有意义的——函数使用特定的域类型和特定的范围类型。同一函数不能用于不同的域和范围。

然而，F# 确实支持方法重载，但仅适用于方法（即附加到类型的函数），其中仅适用于使用元组样式参数传递的方法。

这里有一个例子，它是 `TupleTotal` 方法的另一个变体！

```F#
type Product = {SKU:string; Price: float} with

    // no discount
    member this.TupleTotal3(qty) =
        printfn "using non-discount method"
        this.Price * float qty

    // with discount
    member this.TupleTotal3(qty, discount) =
        printfn "using discount method"
        (this.Price * float qty) - discount
```

通常，F# 编译器会抱怨有两个同名方法，但在这种情况下，因为它们是基于元组的，而且它们的签名不同，所以这是可以接受的。（为了清楚地说明调用的是哪一个，我添加了一条小调试消息。）

这里有一个测试：

```F#
let product = {SKU="ABC"; Price=2.0}

// discount not specified
let total1 = product.TupleTotal3(10)

// discount specified
let total2 = product.TupleTotal3(10,1.0)
```

### 嘿！没那么快……使用方法的缺点

如果你来自面向对象的背景，你可能会想在任何地方都使用方法，因为这是你所熟悉的。但请注意，使用方法也有一些主要的缺点：

- 方法不能很好地进行类型推理
- 方法不能很好地处理高阶函数

事实上，过度使用方法会不必要地绕过F#编程中最强大和最有用的方面。

让我们看看我的意思。

#### 方法不能很好地进行类型推理

让我们再次回到 Person 示例，该示例将相同的逻辑实现为独立函数和方法：

```F#
module Person =
    // type with no members initially
    type T = {First:string; Last:string}

    // constructor
    let create first last =
        {First=first; Last=last}

    // standalone function
    let fullName {First=first; Last=last} =
        first + " " + last

    // function as a member
    type T with
        member this.FullName = fullName this
```

现在让我们看看每种方法在类型推理方面的效果如何。假设我想打印一个人的全名，那么我将定义一个函数 `printFullName`，它将一个人作为参数。

这是使用模块级独立函数的代码。

```F#
open Person

// using standalone function
let printFullName person =
    printfn "Name is %s" (fullName person)

// type inference worked:
//    val printFullName : Person.T -> unit
```

编译时没有问题，类型推断正确地推断出参数是人

现在，让我们尝试“点（dotted）”版本：

```F#
open Person

// using method with "dotting into"
let printFullName2 person =
    printfn "Name is %s" (person.FullName)
```

这根本无法编译，因为类型推断没有足够的信息来推断参数。任何对象都可以实现 `.FullName`——这还不够。

是的，我们可以用参数类型来注释函数，但这违背了类型推理的全部目的。

#### 方法不能很好地处理高阶函数

高阶函数也会出现类似的问题。例如，假设给定一个人员列表，我们想得到他们的全名。

使用独立函数，这很简单：

```F#
open Person

let list = [
    Person.create "Andy" "Anderson";
    Person.create "John" "Johnson";
    Person.create "Jack" "Jackson"]

//get all the full names at once
list |> List.map fullName
```

使用对象方法，我们必须在任何地方创建特殊的 lambdas：

```F#
open Person

let list = [
    Person.create "Andy" "Anderson";
    Person.create "John" "Johnson";
    Person.create "Jack" "Jackson"]

//get all the full names at once
list |> List.map (fun p -> p.FullName)
```

这只是一个简单的例子。对象方法组合不好，很难管道化等等。

所以，我呼吁你们这些刚接触函数式编程的人。如果可以的话，不要使用任何方法，尤其是在学习的时候。它们是阻碍你从函数式编程中充分受益的拐杖。

## 12 示例：基于堆栈的计算器

*Part of the "Thinking functionally" series (*[link](https://fsharpforfunandprofit.com/posts/stack-based-calculator/#series-toc)*)*

使用组合子构建功能
07七月2012 这篇文章是超过3年

https://fsharpforfunandprofit.com/posts/stack-based-calculator/

在这篇文章中，我们将实现一个简单的基于堆栈的计算器（也称为“逆波兰式”风格）。该实现几乎完全由函数完成，只有一种特殊类型，根本没有模式匹配，因此它是本系列介绍的概念的一个很好的测试场。

如果你不熟悉基于堆栈的计算器，它的工作原理如下：数字被推送到堆栈上，加法和乘法等操作会从堆栈中弹出数字并将结果推回到堆栈上。

以下是使用堆栈进行简单计算的示意图：

基于堆栈的计算器图

设计这样一个系统的第一步是考虑如何使用它。遵循类似 Forth 的语法，我们将给每个动作一个标签，这样上面的例子可能会写成这样：

`EMPTY ONE THREE ADD TWO MUL SHOW`

我们可能无法得到这个确切的语法，但让我们看看我们能有多接近。

### Stack 数据类型

首先，我们需要定义堆栈的数据结构。为了简单起见，我们只使用浮点数列表。

```F#
type Stack = float list
```

但是，等等，让我们把它包装成一个单案例联合类型，使其更具描述性，如下所示：

```F#
type Stack = StackContents of float list
```

有关为什么这更好的更多细节，请阅读本文中关于单案例联合类型的讨论。

现在，要创建一个新的堆栈，我们使用 `StackContents` 作为构造函数：

```F#
let newStack = StackContents [1.0;2.0;3.0]
```

为了提取现有 Stack 的内容，我们使用 `StackContents` 进行模式匹配：

```F#
let (StackContents contents) = newStack

// "contents" value set to
// float list = [1.0; 2.0; 3.0]
```

### Push 函数

接下来，我们需要一种方法将数字推送到堆栈上。这将只是使用“`::`”运算符在列表前面添加新值。

以下是我们的推送功能：

```F#
let push x aStack =
    let (StackContents contents) = aStack
    let newContents = x::contents
    StackContents newContents
```

这个基本功能有很多值得讨论的地方。

首先，请注意列表结构是不可变的，因此函数必须接受一个现有的堆栈并返回一个新的堆栈。它不能只是改变现有的堆栈。事实上，本例中的所有函数都具有类似的格式，如下所示：

`输入：堆栈加上其他参数`
`输出：一个新的堆栈`

接下来，参数的顺序应该是什么？堆栈参数应该排在第一位还是最后一位？如果你还记得为部分应用程序设计函数的讨论，你会记得最易变的东西应该放在最后。你很快就会看到这条指导方针即将出台。

最后，通过在函数参数本身中使用模式匹配，而不是在函数体中使用 `let`，可以使函数更加简洁。

以下是重写版本：

```F#
let push x (StackContents contents) =
    StackContents (x::contents)
```

好多了！

顺便说一句，看看它的漂亮签名：

```F#
val push : float -> Stack -> Stack
```

正如我们在上一篇文章中所知道的，签名告诉了你很多关于函数的信息。在这种情况下，即使不知道函数的名称是“push”，我也可以仅从签名中猜测它做了什么。这就是为什么使用显式类型名是一个好主意的原因之一。如果堆栈类型只是一个浮点数列表，它就不会像自文档一样。

不管怎样，现在让我们来测试一下：

```F#
let emptyStack = StackContents []
let stackWith1 = push 1.0 emptyStack
let stackWith2 = push 2.0 stackWith1
```

工作得很好！

### 建立在“push”之上

有了这个简单的函数，我们可以很容易地定义一个将特定数字推送到堆栈上的操作。

```F#
let ONE stack = push 1.0 stack
let TWO stack = push 2.0 stack
```

但是等一下！你能看到 `stack` 参数在两侧都有使用吗？事实上，我们根本不需要提及它。相反，我们可以跳过 `stack` 参数，使用部分应用编写函数，如下所示：

```F#
let ONE = push 1.0
let TWO = push 2.0
let THREE = push 3.0
let FOUR = push 4.0
let FIVE = push 5.0
```

现在你可以看到，如果 `push` 的参数顺序不同，我们就无法做到这一点。

当我们开始时，让我们定义一个函数来创建一个空堆栈：

```F#
let EMPTY = StackContents []
```

现在让我们测试所有这些：

```F#
let stackWith1 = ONE EMPTY
let stackWith2 = TWO stackWith1
let stackWith3  = THREE stackWith2
```

这些中间堆栈很烦人——我们能摆脱它们吗？对！请注意，这些函数ONE、TWO、THREE都有相同的签名：

```F#
Stack -> Stack
```

这意味着它们可以很好地连接在一起！一个的输出可以馈入下一个的输入，如下所示：

```F#
let result123 = EMPTY |> ONE |> TWO |> THREE
let result312 = EMPTY |> THREE |> ONE |> TWO
```

### 弹出堆栈

这样就可以推送到堆栈上了——接下来 `pop` 函数呢？

当我们弹出堆栈时，我们显然会返回堆栈的顶部，但仅此而已吗？

在面向对象的风格中，答案是肯定的。在 OO 方法中，我们会在幕后修改堆栈本身，以便删除顶部元素。

但在函数式风格中，堆栈是不可变的。删除顶部元素的唯一方法是创建一个删除了该元素的新堆栈。为了使调用者能够访问这个新的缩减堆栈，它需要与顶部元素本身一起返回。

换句话说，`pop` 函数必须返回两个值，顶部加上新堆栈。在 F# 中，最简单的方法就是使用元组。

下面是实现：

```F#
/// Pop a value from the stack and return it
/// and the new stack as a tuple
let pop (StackContents contents) =
    match contents with
    | top::rest ->
        let newStack = StackContents rest
        (top,newStack)
```

这个功能也很简单。

如前所述，我们直接在参数中提取 `contents`。

然后我们用 `match..with` 表达式来测试内容。

接下来，我们将顶部元素与其余元素分离，从其余元素创建一个新的堆栈，最后将该对作为元组返回。

试试上面的代码，看看会发生什么。你会得到一个编译器错误！编译器发现了一个我们忽略的情况——如果堆栈为空，会发生什么？

所以现在我们必须决定如何处理这个问题。

- 选项1：返回一个特殊的“成功”或“错误”状态，就像我们在“为什么使用F#？”系列文章中所做的那样。
- 选项2：抛出异常。

一般来说，我更喜欢使用错误案例，但在这种情况下，我们将使用异常。以下是为处理空箱而更改的 `pop` 代码：

```F#
/// Pop a value from the stack and return it
/// and the new stack as a tuple
let pop (StackContents contents) =
    match contents with
    | top::rest ->
        let newStack = StackContents rest
        (top,newStack)
    | [] ->
        failwith "Stack underflow"
```

现在让我们测试一下：

```F#
let initialStack = EMPTY  |> ONE |> TWO
let popped1, poppedStack = pop initialStack
let popped2, poppedStack2 = pop poppedStack
```

并测试下溢：

```F#
let _ = pop EMPTY
```

### 编写数学函数

现在，有了推送和弹出功能，我们可以处理“添加”和“乘法”函数：

```F#
let ADD stack =
   let x,s = pop stack  //pop the top of the stack
   let y,s2 = pop s     //pop the result stack
   let result = x + y   //do the math
   push result s2       //push back on the doubly-popped stack

let MUL stack =
   let x,s = pop stack  //pop the top of the stack
   let y,s2 = pop s     //pop the result stack
   let result = x * y   //do the math
   push result s2       //push back on the doubly-popped stack
```

以交互方式测试这些：

```F#
let add1and2 = EMPTY |> ONE |> TWO |> ADD
let add2and3 = EMPTY |> TWO |> THREE |> ADD
let mult2and3 = EMPTY |> TWO |> THREE |> MUL
```

它奏效了！

### 是时候进行重构了…

很明显，这两个函数之间存在大量重复代码。我们如何重构？

这两个函数都从堆栈中弹出两个值，应用某种二进制函数，然后将结果推回堆栈。这导致我们将通用代码重构为一个“二进制”函数，该函数接受一个双参数数学函数作为参数：

```F#
let binary mathFn stack =
    // pop the top of the stack
    let y,stack' = pop stack
    // pop the top of the stack again
    let x,stack'' = pop stack'
    // do the math
    let z = mathFn x y
    // push the result value back on the doubly-popped stack
    push z stack''
```

*请注意，在这个实现中，我已经切换到使用记号来表示“同一”对象的更改状态，而不是数字后缀。数字后缀很容易让人感到困惑。*

问题：为什么参数是按上面顺序排列的，而不是 `mathFn` 在 `stack` 之后？

现在我们有了 `binary`，我们可以更简单地定义 ADD 和朋友：

这是使用新的 `binary` 助手首次尝试 ADD：

```F#
let ADD aStack = binary (fun x y -> x + y) aStack
```

但是我们可以删除 lambda，因为它正是内置 `+` 函数的定义！这给了我们：

```F#
let ADD aStack = binary (+) aStack
```

同样，我们可以使用部分应用程序来隐藏堆栈参数。以下是最终的定义：

```F#
let ADD = binary (+)
```

以下是其他一些数学函数的定义：

```F#
let SUB = binary (-)
let MUL = binary (*)
let DIV = binary (/)
```

让我们再次以交互方式进行测试。

```F#
let threeDivTwo = EMPTY |> THREE |> TWO |> DIV   // Answer: 1.5
let twoSubtractFive = EMPTY |> TWO |> FIVE |> SUB  // Answer: -3.0
let oneAddTwoSubThree = EMPTY |> ONE |> TWO |> ADD |> THREE |> SUB // Answer: 0.0
```

以类似的方式，我们可以为一元函数创建一个辅助函数

```F#
let unary f stack =
    let x,stack' = pop stack  //pop the top of the stack
    push (f x) stack'         //push the function value on the stack
```

然后定义一些一元函数：

```F#
let NEG = unary (fun x -> -x)
let SQUARE = unary (fun x -> x * x)
```

再次交互式测试：

```F#
let neg3 = EMPTY |> THREE |> NEG
let square2 = EMPTY |> TWO |> SQUARE
```

### 把它们放在一起

在最初的需求中，我们提到我们希望能够显示结果，所以让我们定义一个 SHOW 函数。

```F#
let SHOW stack =
    let x,_ = pop stack
    printfn "The answer is %f" x
    stack  // keep going with same stack
```

请注意，在这种情况下，我们弹出原始堆栈，但忽略缩减版本。函数的最终结果是原始堆栈，就好像它从未被弹出过一样。

现在，我们终于可以根据原始需求编写代码示例了

```F#
EMPTY |> ONE |> THREE |> ADD |> TWO |> MUL |> SHOW // (1+3)*2 = 8
```

#### 更进一步

这很有趣——我们还能做什么？

那么，我们可以定义更多的核心辅助函数：

```F#
/// Duplicate the top value on the stack
let DUP stack =
    // get the top of the stack
    let x,_ = pop stack
    // push it onto the stack again
    push x stack

/// Swap the top two values
let SWAP stack =
    let x,s = pop stack
    let y,s' = pop s
    push y (push x s')

/// Make an obvious starting point
let START  = EMPTY
```

有了这些附加功能，我们可以写一些很好的例子：

```F#
START
    |> ONE |> TWO |> SHOW

START
    |> ONE |> TWO |> ADD |> SHOW
    |> THREE |> ADD |> SHOW

START
    |> THREE |> DUP |> DUP |> MUL |> MUL // 27

START
    |> ONE |> TWO |> ADD |> SHOW  // 3
    |> THREE |> MUL |> SHOW       // 9
    |> TWO |> DIV |> SHOW         // 9 div 2 = 4.5
```

### 使用组合而不是管道

但这还不是全部。事实上，还有另一种非常有趣的方式来思考这些功能。

正如我之前指出的，它们都有相同的签名：

```F#
Stack -> Stack
```

因此，由于输入和输出类型相同，这些函数可以使用组合运算符 `>>` 组合，而不仅仅是用管道链接在一起。

以下是一些示例：

```F#
// define a new function
let ONE_TWO_ADD =
    ONE >> TWO >> ADD

// test it
START |> ONE_TWO_ADD |> SHOW

// define a new function
let SQUARE =
    DUP >> MUL

// test it
START |> TWO |> SQUARE |> SHOW

// define a new function
let CUBE =
    DUP >> DUP >> MUL >> MUL

// test it
START |> THREE |> CUBE |> SHOW

// define a new function
let SUM_NUMBERS_UPTO =
    DUP      // n, n           2 items on stack
    >> ONE   // n, n, 1        3 items on stack
    >> ADD   // n, (n+1)       2 items on stack
    >> MUL   // n(n+1)         1 item on stack
    >> TWO   // n(n+1), 2      2 items on stack
    >> DIV   // n(n+1)/2       1 item on stack

// test it with sum of numbers up to 9
START |> THREE |> SQUARE |> SUM_NUMBERS_UPTO |> SHOW  // 45
```

在每种情况下，通过将其他函数组合在一起以创建新函数来定义新函数。这是构建功能的“组合子”方法的一个很好的例子。

### 管道 vs 组合

我们现在已经看到了使用这种基于堆栈的模型的两种不同方式；通过管道或组合。那么，有什么区别呢？为什么我们更喜欢一种方式而不是另一种方式？

不同之处在于，从某种意义上说，管道是一种“实时转换”操作。当你使用管道时，你实际上是在做操作，传递一个特定的堆栈。

另一方面，组合是一种你想做什么的“计划”，从一组部分构建一个整体功能，但还没有实际运行它。

例如，我可以创建一个“计划”，通过组合较小的操作来平方一个数字：

```F#
let COMPOSED_SQUARE = DUP >> MUL
```

我无法用管道方法进行等效操作。

```F#
let PIPED_SQUARE = DUP |> MUL
```

这会导致编译错误。我必须有某种具体的堆栈实例才能使其工作：

```F#
let stackWith2 = EMPTY |> TWO
let twoSquared = stackWith2 |> DUP |> MUL
```

即便如此，我也只得到了这个特定输入的答案，而不是像 COMPOSED_SQUARE 示例中那样得到所有可能输入的计划。

创建“计划”的另一种方法是显式地将lambda传递给更原始的函数，正如我们在开头看到的那样：

```F#
let LAMBDA_SQUARE = unary (fun x -> x * x)
```

这要明确得多（而且可能更快），但失去了组合方法的所有好处和清晰度。

所以，一般来说，如果可以的话，就选择组合法吧！

### 完整的代码

这是到目前为止所有示例的完整代码。

```F#
// ==============================================
// Types
// ==============================================

type Stack = StackContents of float list

// ==============================================
// Stack primitives
// ==============================================

/// Push a value on the stack
let push x (StackContents contents) =
    StackContents (x::contents)

/// Pop a value from the stack and return it
/// and the new stack as a tuple
let pop (StackContents contents) =
    match contents with
    | top::rest ->
        let newStack = StackContents rest
        (top,newStack)
    | [] ->
        failwith "Stack underflow"

// ==============================================
// Operator core
// ==============================================

// pop the top two elements
// do a binary operation on them
// push the result
let binary mathFn stack =
    let y,stack' = pop stack
    let x,stack'' = pop stack'
    let z = mathFn x y
    push z stack''

// pop the top element
// do a unary operation on it
// push the result
let unary f stack =
    let x,stack' = pop stack
    push (f x) stack'

// ==============================================
// Other core
// ==============================================

/// Pop and show the top value on the stack
let SHOW stack =
    let x,_ = pop stack
    printfn "The answer is %f" x
    stack  // keep going with same stack

/// Duplicate the top value on the stack
let DUP stack =
    let x,s = pop stack
    push x (push x s)

/// Swap the top two values
let SWAP stack =
    let x,s = pop stack
    let y,s' = pop s
    push y (push x s')

/// Drop the top value on the stack
let DROP stack =
    let _,s = pop stack  //pop the top of the stack
    s                    //return the rest

// ==============================================
// Words based on primitives
// ==============================================

// Constants
// -------------------------------
let EMPTY = StackContents []
let START  = EMPTY


// Numbers
// -------------------------------
let ONE = push 1.0
let TWO = push 2.0
let THREE = push 3.0
let FOUR = push 4.0
let FIVE = push 5.0

// Math functions
// -------------------------------
let ADD = binary (+)
let SUB = binary (-)
let MUL = binary (*)
let DIV = binary (/)

let NEG = unary (fun x -> -x)


// ==============================================
// Words based on composition
// ==============================================

let SQUARE =
    DUP >> MUL

let CUBE =
    DUP >> DUP >> MUL >> MUL

let SUM_NUMBERS_UPTO =
    DUP      // n, n           2 items on stack
    >> ONE   // n, n, 1        3 items on stack
    >> ADD   // n, (n+1)       2 items on stack
    >> MUL   // n(n+1)         1 item on stack
    >> TWO   // n(n+1), 2      2 items on stack
    >> DIV   // n(n+1)/2       1 item on stack

```

### 摘要

所以我们有了它，一个简单的基于堆栈的计算器。我们已经看到了如何从一些基本操作（`push`、`pop`、`binary`、`unary`）开始，并从中构建一个易于实现和使用的整个领域特定语言。

正如你所猜测的，这个例子在很大程度上基于Forth语言。我强烈推荐免费的书《Thinking Forth》，它不仅介绍Forth语言，还介绍同样适用于函数式编程的（非面向对象！）问题分解技术。

我写这篇文章的灵感来自 Ashley Feniello 的一个很棒的博客。如果你想更深入地模拟 F# 中基于堆栈的语言，那就从那里开始。玩得高兴！



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

## 1 用类型设计：简介

*Part of the "Designing with types" series (*[link](https://fsharpforfunandprofit.com/posts/designing-with-types-intro/#series-toc)*)*

使设计更加透明，提高正确性
2013年1月12日 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/designing-with-types-intro/

在本系列中，我们将探讨在设计过程中使用类型的一些方法。特别是，周到地使用类型可以使设计更加透明，同时提高正确性。

本系列将聚焦于设计的“微观层面”。也就是说，在单个类型和函数的最低级别上工作。更高层次的设计方法，以及使用函数式或面向对象风格的相关决策，将在另一个系列中讨论。

许多建议在 C# 或 Java 中也是可行的，但 F# 类型的轻量级特性意味着我们更有可能进行这种重构。

### 一个基本的例子

为了演示类型的各种用途，我将使用一个非常简单的示例，即 `Contact` 类型，如下所示。

```F#
type Contact =
    {
    FirstName: string;
    MiddleInitial: string;
    LastName: string;

    EmailAddress: string;
    //true if ownership of email address is confirmed
    IsEmailVerified: bool;

    Address1: string;
    Address2: string;
    City: string;
    State: string;
    Zip: string;
    //true if validated against address service
    IsAddressValid: bool;
    }

```

这似乎很明显——我相信我们都见过很多次这样的事情。那么，我们能用它做什么呢？我们如何重构它以充分利用类型系统？

### 创建“原子”类型

首先要做的是查看数据访问和更新的使用模式。例如，`Zip` 是否有可能在不同时更新 `Address1` 的情况下进行更新？另一方面，事务更新 `EmailAddress` 而不更新 `FirstName` 的情况可能很常见。

这就引出了第一条准则：

- *指导原则：使用记录或元组将需要一致的数据（即“原子”）分组在一起，但不要不必要地将不相关的数据分组在一起。*

在这种情况下，很明显，三个名称值是一个集合，地址值是一组，电子邮件也是一组。

我们这里还有一些额外的标志，比如 `IsAddressValid` 和 `IsEmailVerified`。这些是否应该成为相关集合的一部分？当然，目前是的，因为标志取决于相关值。

例如，如果 `EmailAddress` 更改，则 `IsEmailVerified` 可能需要同时重置为 false。

对于 `PostalAddress`，很明显，核心“地址”部分是一个有用的通用类型，没有 `IsAddressValid` 标志。另一方面，`IsAddressValid` 与地址相关联，并在地址更改时进行更新。

因此，我们似乎应该创建两种类型。一个是通用的 `PostalAddress`，另一个是联系人上下文中的地址，我们可以称之为 `PostalContactInfo`。

```F#
type PostalAddress =
    {
    Address1: string;
    Address2: string;
    City: string;
    State: string;
    Zip: string;
    }

type PostalContactInfo =
    {
    Address: PostalAddress;
    IsAddressValid: bool;
    }
```

最后，我们可以使用选项类型来表示某些值（如 `MiddleInitial`）确实是可选的。

```F#
type PersonalName =
    {
    FirstName: string;
    // use "option" to signal optionality
    MiddleInitial: string option;
    LastName: string;
    }
```

### 摘要

经过所有这些更改，我们现在有以下代码：

```F#
type PersonalName =
    {
    FirstName: string;
    // use "option" to signal optionality
    MiddleInitial: string option;
    LastName: string;
    }

type EmailContactInfo =
    {
    EmailAddress: string;
    IsEmailVerified: bool;
    }

type PostalAddress =
    {
    Address1: string;
    Address2: string;
    City: string;
    State: string;
    Zip: string;
    }

type PostalContactInfo =
    {
    Address: PostalAddress;
    IsAddressValid: bool;
    }

type Contact =
    {
    Name: PersonalName;
    EmailContactInfo: EmailContactInfo;
    PostalContactInfo: PostalContactInfo;
    }

```

我们还没有编写一个函数，但代码已经更好地代表了域。然而，这只是我们所能做的事情的开始。

接下来，使用单案例联合为原始类型添加语义意义。



## 2 用类型设计：单案例联合类型

*Part of the "Designing with types" series (*[link](https://fsharpforfunandprofit.com/posts/designing-with-types-single-case-dus/#series-toc)*)*

为基本类型添加意义
2013年1月13日这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/designing-with-types-single-case-dus/

在上一篇文章的末尾，我们有电子邮件地址、邮政编码等的值，定义如下：

```F#
EmailAddress: string;
State: string;
Zip: string;
```

这些都被定义为简单的字符串。但实际上，它们只是字符串吗？电子邮件地址是否可以与邮政编码或州缩写互换？

在领域驱动的设计中，它们确实是不同的东西，而不仅仅是字符串。因此，理想情况下，我们希望为它们提供许多单独的类型，这样它们就不会意外混淆。

长期以来，这一直被认为是一种良好的做法，但在 C# 和 Java 等语言中，创建数百个这样的小类型可能会很痛苦，从而导致所谓的“原始痴迷”代码气味。

但是F#没有任何借口！创建简单的包装器类型是轻而易举的。

### 包装基本类型

创建单独类型的最简单方法是将底层字符串类型包装在另一个类型中。

我们可以使用单案例联合类型来实现，如下所示：

```F#
type EmailAddress = EmailAddress of string
type ZipCode = ZipCode of string
type StateCode = StateCode of string
```

或者，我们可以在一个字段中使用记录类型，如下所示：

```F#
type EmailAddress = { EmailAddress: string }
type ZipCode = { ZipCode: string }
type StateCode = { StateCode: string}
```

这两种方法都可以用来围绕字符串或其他基本类型创建包装器类型，那么哪种方法更好呢？

答案通常是单一案例可区分联合。“包装”和“解包”要容易得多，因为“联合案例”本身就是一个适当的构造函数。展开可以使用内联模式匹配来完成。

以下是一些如何构造和解构 `EmailAddress` 类型的示例：

```F#
type EmailAddress = EmailAddress of string

// using the constructor as a function
"a" |> EmailAddress
["a"; "b"; "c"] |> List.map EmailAddress

// inline deconstruction
let a' = "a" |> EmailAddress
let (EmailAddress a'') = a'

let addresses =
    ["a"; "b"; "c"]
    |> List.map EmailAddress

let addresses' =
    addresses
    |> List.map (fun (EmailAddress e) -> e)
```

使用记录类型无法轻松做到这一点。

因此，让我们再次重构代码以使用这些联合类型。现在看起来是这样的：

```F#
type PersonalName =
    {
    FirstName: string;
    MiddleInitial: string option;
    LastName: string;
    }

type EmailAddress = EmailAddress of string

type EmailContactInfo =
    {
    EmailAddress: EmailAddress;
    IsEmailVerified: bool;
    }

type ZipCode = ZipCode of string
type StateCode = StateCode of string

type PostalAddress =
    {
    Address1: string;
    Address2: string;
    City: string;
    State: StateCode;
    Zip: ZipCode;
    }

type PostalContactInfo =
    {
    Address: PostalAddress;
    IsAddressValid: bool;
    }

type Contact =
    {
    Name: PersonalName;
    EmailContactInfo: EmailContactInfo;
    PostalContactInfo: PostalContactInfo;
    }
```

联合类型的另一个优点是，实现可以用模块签名封装，我们将在下面讨论。

### 命名单个案例联合的“案例”

在上面的例子中，我们对案例使用了与类型相同的名称：

```F#
type EmailAddress = EmailAddress of string
type ZipCode = ZipCode of string
type StateCode = StateCode of string
```

这最初可能看起来很混乱，但实际上它们在不同的范围内，所以没有命名冲突。一个是类型，一个是同名的构造函数。

所以，如果你看到这样的函数签名：

```F#
val f: string -> EmailAddress
```

这指的是类型世界中的事物，因此 `EmailAddress` 指的是该类型。

另一方面，如果你看到这样的代码：

```F#
let x = EmailAddress y
```

这指的是值世界中的事物，因此 `EmailAddress` 指的是构造函数。

### 构建单一案例联合

对于具有特殊含义的值，如电子邮件地址和邮政编码，通常只允许使用某些值。并非每个字符串都是可接受的电子邮件或邮政编码。

这意味着我们需要在某个时候进行验证，还有什么比在构建时更好的时候呢？毕竟，一旦构建了值，它就是不可变的，所以不用担心以后有人会修改它。

以下是我们如何使用一些构造函数扩展上述模块：

```F#
... types as above ...

let CreateEmailAddress (s:string) =
    if System.Text.RegularExpressions.Regex.IsMatch(s,@"^\S+@\S+\.\S+$")
        then Some (EmailAddress s)
        else None

let CreateStateCode (s:string) =
    let s' = s.ToUpper()
    let stateCodes = ["AZ";"CA";"NY"] //etc
    if stateCodes |> List.exists ((=) s')
        then Some (StateCode s')
        else None
```

我们现在可以测试构造函数：

```F#
CreateStateCode "CA"
CreateStateCode "XX"

CreateEmailAddress "a@example.com"
CreateEmailAddress "example.com"
```

### 处理构造函数中的无效输入

对于这些类型的构造函数，一个迫在眉睫的挑战是如何处理无效输入的问题。例如，如果我将“abc”传递给电子邮件地址构造函数，会发生什么？

有很多方法可以处理它。

首先，你可以抛出一个异常。我觉得这个丑陋而缺乏想象力，所以我立即拒绝了这个！

接下来，您可以返回一个选项类型，其中 `None` 表示输入无效。这就是上面的构造函数所做的。

这通常是最简单的方法。它的优点是，当值无效时，调用者必须显式处理这种情况。

例如，上面示例的调用者代码可能如下：

```F#
match (CreateEmailAddress "a@example.com") with
| Some email -> ... do something with email
| None -> ... ignore?
```

缺点是，对于复杂的验证，可能不清楚出了什么问题。电子邮件太长，或者缺少“@”符号，或者域名无效？我们无法判断。

如果您确实需要更多详细信息，您可能希望返回一个在错误情况下包含更详细解释的类型。

以下示例使用 `CreationResult` 类型来指示失败案例中的错误。

```F#
type EmailAddress = EmailAddress of string
type CreationResult<'T> = Success of 'T | Error of string

let CreateEmailAddress2 (s:string) =
    if System.Text.RegularExpressions.Regex.IsMatch(s,@"^\S+@\S+\.\S+$")
        then Success (EmailAddress s)
        else Error "Email address must contain an @ sign"

// test
CreateEmailAddress2 "example.com"
```

最后，最通用的方法是使用延续（continuations）。也就是说，您传入两个函数，一个用于成功案例（将新构造的电子邮件作为参数），另一个用于失败案例（将错误字符串作为参数）。

```F#
type EmailAddress = EmailAddress of string

let CreateEmailAddressWithContinuations success failure (s:string) =
    if System.Text.RegularExpressions.Regex.IsMatch(s,@"^\S+@\S+\.\S+$")
        then success (EmailAddress s)
        else failure "Email address must contain an @ sign"
```

success 函数接受电子邮件作为参数，error 函数接受字符串。这两个函数必须返回相同的类型，但类型由您决定。

这里有一个简单的例子——这两个函数都执行 printf，不返回任何值（即 unit）。

```F#
let success (EmailAddress s) = printfn "success creating email %s" s
let failure  msg = printfn "error creating email: %s" msg
CreateEmailAddressWithContinuations success failure "example.com"
CreateEmailAddressWithContinuations success failure "x@example.com"
```

使用 continuation，您可以轻松地复制任何其他方法。例如，以下是创建选项的方法。在这种情况下，这两个函数都返回 `EmailAddress option`。

```F#
let success e = Some e
let failure _  = None
CreateEmailAddressWithContinuations success failure "example.com"
CreateEmailAddressWithContinuations success failure "x@example.com"
```

以下是在错误情况下抛出异常的方法：

```F#
let success e = e
let failure _  = failwith "bad email address"
CreateEmailAddressWithContinuations success failure "example.com"
CreateEmailAddressWithContinuations success failure "x@example.com"
```

这段代码看起来很麻烦，但在实践中，您可能会创建一个局部部分应用的函数，而不是冗长的函数。

```F#
// setup a partially applied function
let success e = Some e
let failure _  = None
let createEmail = CreateEmailAddressWithContinuations success failure

// use the partially applied function
createEmail "x@example.com"
createEmail "example.com"
```

> 如果你觉得这篇文章很有趣，看看我的《领域建模函数化》一书！这是对领域驱动设计、类型建模和函数式编程的一个很好的介绍。

### 为包装器类型创建模块

由于我们添加了验证，这些简单的包装器类型开始变得更加复杂，我们可能会发现我们想与该类型关联的其他函数。

因此，为每种包装器类型创建一个模块，并将该类型及其相关函数放在那里，这可能是一个好主意。

```F#
module EmailAddress =

    type T = EmailAddress of string

    // wrap
    let create (s:string) =
        if System.Text.RegularExpressions.Regex.IsMatch(s,@"^\S+@\S+\.\S+$")
            then Some (EmailAddress s)
            else None

    // unwrap
    let value (EmailAddress e) = e
```

然后，该类型的用户将使用模块函数创建和解包该类型。例如：

```F#
// create email addresses
let address1 = EmailAddress.create "x@example.com"
let address2 = EmailAddress.create "example.com"

// unwrap an email address
match address1 with
| Some e -> EmailAddress.value e |> printfn "the value is %s"
| None -> ()
```

### 强制使用构造函数

一个问题是，您不能强制调用者使用构造函数。有人可以绕过验证直接创建类型。

在实践中，这往往不是问题。一个简单的技术是使用命名约定来指示“private”类型，并提供“wrap”和“unwrap”函数，这样客户端就不需要直接与该类型交互。

这里有一个例子：

```F#
module EmailAddress =

    // private type
    type _T = EmailAddress of string

    // wrap
    let create (s:string) =
        if System.Text.RegularExpressions.Regex.IsMatch(s,@"^\S+@\S+\.\S+$")
            then Some (EmailAddress s)
            else None

    // unwrap
    let value (EmailAddress e) = e
```

当然，在这种情况下，类型并不是真正私有的，但您鼓励调用者始终使用“published”函数。

如果你真的想封装类型的内部并强制调用者使用构造函数，你可以使用模块签名。

以下是电子邮件地址示例的签名文件：

```F#
// FILE: EmailAddress.fsi

module EmailAddress

// encapsulated type
type T

// wrap
val create : string -> T option

// unwrap
val value : T -> string
```

（请注意，模块签名仅在编译后的项目中有效，在交互式脚本中无效，因此要测试这一点，您需要在 F# 项目中创建三个文件，文件名如下所示。）

以下是实现文件：

```F#
// FILE: EmailAddress.fs

module EmailAddress

// encapsulated type
type T = EmailAddress of string

// wrap
let create (s:string) =
    if System.Text.RegularExpressions.Regex.IsMatch(s,@"^\S+@\S+\.\S+$")
        then Some (EmailAddress s)
        else None

// unwrap
let value (EmailAddress e) = e
```

这是一个客户：

```F#
// FILE: EmailAddressClient.fs

module EmailAddressClient

open EmailAddress

// code works when using the published functions
let address1 = EmailAddress.create "x@example.com"
let address2 = EmailAddress.create "example.com"

// code that uses the internals of the type fails to compile
let address3 = T.EmailAddress "bad email"

```

模块签名导出的类型 `EmailAddress.T` 是不透明的，因此客户端无法访问内部。

如您所见，这种方法强制使用构造函数。试图直接创建类型（`T.EmailAddress "bad email"`）会导致编译错误。

### 何时“包装”单案例联合

现在我们有了包装器类型，我们应该什么时候构造它们？

通常，您只需要在服务边界处（例如，六边形架构中的边界）

在这种方法中，包装是在 UI 层中完成的，或者在从持久层加载时完成的，一旦创建了包装类型，它就会被传递到域层并作为不透明类型进行“整体”操作。令人惊讶的是，在域本身中工作时，您实际上直接需要包装内容的情况并不常见。

作为构造的一部分，调用者使用提供的构造函数而不是执行自己的验证逻辑至关重要。这确保了“坏”值永远不会进入域。

例如，这里有一些代码显示 UI 正在进行自己的验证：

```F#
let processFormSubmit () =
    let s = uiTextBox.Text
    if (s.Length < 50)
        then // set email on domain object
        else // show validation error message
```

更好的方法是让构造函数来做，如前所示。

```F#
let processFormSubmit () =
    let emailOpt = uiTextBox.Text |> EmailAddress.create
    match emailOpt with
    | Some email -> // set email on domain object
    | None -> // show validation error message
```

### 何时“拆封”单案例联合

什么时候需要拆开包装？同样，通常只在服务边界。例如，当您将电子邮件持久化到数据库，或绑定到 UI 元素或视图模型时。

避免显式展开的一个技巧是再次使用连续方法，传递一个将应用于包裹值的函数。

也就是说，与其显式调用“unwrap”函数：

```F#
address |> EmailAddress.value |> printfn "the value is %s"
```

你可以传入一个应用于内部值的函数，如下所示：

```F#
address |> EmailAddress.apply (printfn "the value is %s")
```

综上所述，我们现在有了完整的 `EmailAddress` 模块。

```F#
module EmailAddress =

    type _T = EmailAddress of string

    // create with continuation
    let createWithCont success failure (s:string) =
        if System.Text.RegularExpressions.Regex.IsMatch(s,@"^\S+@\S+\.\S+$")
            then success (EmailAddress s)
            else failure "Email address must contain an @ sign"

    // create directly
    let create s =
        let success e = Some e
        let failure _  = None
        createWithCont success failure s

    // unwrap with continuation
    let apply f (EmailAddress e) = f e

    // unwrap directly
    let value e = apply id e
```

`create` 和 `value` 函数不是严格必需的，但添加它们是为了方便调用者。

### 到目前为止的代码

现在让我们重构 `Contact` 代码，添加新的包装器类型和模块。

```F#
module EmailAddress =

    type T = EmailAddress of string

    // create with continuation
    let createWithCont success failure (s:string) =
        if System.Text.RegularExpressions.Regex.IsMatch(s,@"^\S+@\S+\.\S+$")
            then success (EmailAddress s)
            else failure "Email address must contain an @ sign"

    // create directly
    let create s =
        let success e = Some e
        let failure _  = None
        createWithCont success failure s

    // unwrap with continuation
    let apply f (EmailAddress e) = f e

    // unwrap directly
    let value e = apply id e

module ZipCode =

    type T = ZipCode of string

    // create with continuation
    let createWithCont success failure  (s:string) =
        if System.Text.RegularExpressions.Regex.IsMatch(s,@"^\d{5}$")
            then success (ZipCode s)
            else failure "Zip code must be 5 digits"

    // create directly
    let create s =
        let success e = Some e
        let failure _  = None
        createWithCont success failure s

    // unwrap with continuation
    let apply f (ZipCode e) = f e

    // unwrap directly
    let value e = apply id e

module StateCode =

    type T = StateCode of string

    // create with continuation
    let createWithCont success failure  (s:string) =
        let s' = s.ToUpper()
        let stateCodes = ["AZ";"CA";"NY"] //etc
        if stateCodes |> List.exists ((=) s')
            then success (StateCode s')
            else failure "State is not in list"

    // create directly
    let create s =
        let success e = Some e
        let failure _  = None
        createWithCont success failure s

    // unwrap with continuation
    let apply f (StateCode e) = f e

    // unwrap directly
    let value e = apply id e

type PersonalName =
    {
    FirstName: string;
    MiddleInitial: string option;
    LastName: string;
    }

type EmailContactInfo =
    {
    EmailAddress: EmailAddress.T;
    IsEmailVerified: bool;
    }

type PostalAddress =
    {
    Address1: string;
    Address2: string;
    City: string;
    State: StateCode.T;
    Zip: ZipCode.T;
    }

type PostalContactInfo =
    {
    Address: PostalAddress;
    IsAddressValid: bool;
    }

type Contact =
    {
    Name: PersonalName;
    EmailContactInfo: EmailContactInfo;
    PostalContactInfo: PostalContactInfo;
    }

```

顺便说一句，请注意，我们现在在三个包装器类型模块中有很多重复代码。什么是摆脱它的好方法，或者至少让它更干净？

### 摘要

总结可区分联合的使用情况，以下是一些指导方针：

- 请使用区分大小写的联合来创建准确表示域的类型。
- 如果包装的值需要验证，那么提供执行验证的构造函数并强制使用它们。
- 明确验证失败时会发生什么。在简单情况下，返回选项类型。在更复杂的情况下，让调用者传入成功和失败的处理程序。
- 如果包装值有许多相关函数，请考虑将其移动到自己的模块中。
- 如果需要强制封装，请使用签名文件。

我们还没有完成重构。我们可以改变类型的设计，在编译时强制执行业务规则，使非法状态无法表示。

### 更新

许多人要求提供更多信息，了解如何确保仅通过执行验证的特殊构造函数创建 `EmailAddress` 等受约束类型。所以我在这里创建了一个要点，其中有一些其他方法的详细示例。



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