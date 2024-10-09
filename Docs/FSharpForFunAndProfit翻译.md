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
- 演讲：《国家宣言》：用弗兰肯芬克特博士和《宣言》的故事介绍如何处理国家。有关相关帖子，请参阅链接页面。
- Reader Monad：重塑阅读者单子。
- 映射（map）、绑定（bind）、应用（apply）、提升（lift）、序列（sequence）和遍历（traverse）：描述处理泛型数据类型的一些核心函数的系列。
- 折叠和递归类型：看看递归类型、同态、尾部递归、左折叠和右折叠之间的区别等等。
- “函数式授权方法”系列。如何使用“能力”处理授权的常见安全挑战。也可作为讲座。

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
20. 详尽的模式匹配
    一种确保正确性的强大技术
21. 使用类型系统确保代码正确
    在 F# 中，类型系统是你的朋友，而不是你的敌人
22. 工作示例：为正确性而设计
    如何使非法国家不具代表性
23. 并发性
    我们如何编写软件的下一次重大革命？
24. 异步编程
    用Async类封装后台任务
25. 消息和代理
    更容易思考并发性
26. 函数式反应式编程
    将事件转化为流
27. 完整性
    F#是整体的一部分。NET生态系统
28. 与 .NET 库无缝互操作
    一些便于使用的函数式 .NET库
29. C#能做的任何事情。。。
    F中面向对象代码的旋风之旅#
30. 为什么使用F#：结论

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