# 1 如何设计和编写一个完整的程序

*Part of the "A recipe for a functional app" series (*[link](https://fsharpforfunandprofit.com/posts/recipe-part1/#series-toc)*)*

功能应用程序的配方，第1部分
2013年5月10日 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/recipe-part1

**“我想我在微观层面上理解函数式编程，我写过玩具程序，但我如何真正编写一个完整的应用程序，有真实的数据、真实的错误处理等等？”**

这是一个非常常见的问题，所以我想在这一系列文章中，我会描述一个完全做到这一点的方法，包括设计、验证、错误处理、持久性、依赖关系管理、代码组织等。

首先是一些评论和注意事项：

- 我将只关注一个用例，而不是整个应用程序。我希望如何根据需要扩展代码是显而易见的。
- 这将是一个非常简单的面向数据流的配方，没有特殊的技巧或先进的技术。但如果你刚刚开始，我认为有一些简单的步骤可以遵循以获得可预测的结果是有用的。我并不认为这是唯一真正的方法。不同的场景需要不同的食谱，当然，随着你越来越专业，你可能会发现这个食谱过于简单和有限。
- 为了帮助简化从面向对象设计的过渡，我将尝试使用熟悉的概念，如“模式”、“服务”、“依赖注入”等，并解释它们如何映射到函数式概念。
- 这个食谱也故意有点命令式（imperative），也就是说，它使用了一个明确的循序渐进的工作流程。我希望这种方法能够缓解从 OO 到 FP 的过渡。
- 为了保持简单（并且可以从简单的 F# 脚本中使用），我将模拟整个基础设施，直接避免 UI。

## 概述

以下是我计划在本系列中介绍的内容的概述：

- **将用例转换为函数**。在第一篇文章中，我们将研究一个简单的用例，并看看如何使用函数式方法来实现它。
- **将较小的功能连接在一起**。在下一篇文章中，我们将讨论一个简单的比喻，将较小的函数组合成较大的函数。
- **类型驱动的设计和故障类型**。在第三篇文章中，我们将构建用例所需的类型，并讨论故障路径中特殊错误类型的使用。
- **配置和依赖关系管理**。在这篇文章中，我们将讨论如何连接所有功能。
- **验证**。在这篇文章中，我们将讨论实现验证的各种方法，以及从不安全的外部世界转换为类型安全的温暖模糊世界。
- **基础设施**。在这篇文章中，我们将讨论各种基础设施组件，如日志记录、使用外部代码等。
- **域层**。在这篇文章中，我们将讨论领域驱动设计如何在函数式环境中工作。
- **表示层**。在这篇文章中，我们将讨论如何将结果和错误传达回 UI。
- **应对不断变化的需求**。在这篇文章中，我们将讨论如何处理不断变化的需求以及这对代码的影响。

## 开始使用

让我们选择一个非常简单的用例，即通过 web 服务更新一些客户信息。

以下是基本要求：

- 用户提交一些数据（用户 ID、姓名和电子邮件）。
- 我们会检查姓名和电子邮件是否有效。
- 数据库中的相应用户记录将使用新名称和电子邮件进行更新。
- 如果电子邮件已更改，请向该地址发送一封验证电子邮件。
- 向用户显示操作结果。

这是一个典型的以数据为中心的用例。有某种请求触发用例，然后请求数据在系统中“流动”，由每个步骤依次处理。这种场景在企业软件中很常见，这就是为什么我把它作为一个例子。

以下是各种组件的示意图：

![Recipe Happy Path](https://fsharpforfunandprofit.com/posts/recipe-part1/Recipe_HappyPath.png)

但这仅描述了“幸福之路”。现实从未如此简单！如果在数据库中找不到用户 ID，或者电子邮件地址无效，或者数据库有错误，会发生什么？

让我们更新图表，显示所有可能出错的地方。

![Recipe Error Path](https://fsharpforfunandprofit.com/posts/recipe-part1/Recipe_ErrorPath.png)

在用例的每个步骤中，各种事情都可能导致错误，如图所示。解释如何以优雅的方式处理这些错误将是本系列的目标之一。

## 函数式思维

那么，现在我们已经了解了用例中的步骤，我们如何使用函数式方法设计解决方案呢？

首先，我们必须解决原始用例和函数式思维之间的不匹配问题。

在用例中，我们通常考虑请求/响应模型。请求已发送，响应已返回。如果出现问题，流程会短路，响应会“提前”返回。

这是一个基于用例简化版本的图表，显示了我的意思：

![A imperative data flow](https://fsharpforfunandprofit.com/posts/recipe-part1/Recipe_ResponseBack.png)

但在函数模型中，函数是一个有输入和输出的黑匣子，如下所示：

![A function with one output](https://fsharpforfunandprofit.com/posts/recipe-part1/Recipe_Function1.png)

我们如何调整用例以适应这种模型？

### 仅正向流动

首先，您必须认识到功能数据流只是向前的。你不能掉头或早点回来。

在我们的例子中，这意味着所有错误都必须被发送到最后，作为通往幸福之路的替代路径。

![A functional data flow](https://fsharpforfunandprofit.com/posts/recipe-part1/Recipe_ResponseForward.png)

一旦我们做到了这一点，我们就可以将整个流程转换为一个“黑匣子”函数，如下所示：

![A function with many outputs](https://fsharpforfunandprofit.com/posts/recipe-part1/Recipe_FunctionMany.png)

当然，如果你看看大函数的内部，它是由（用函数的话说是“由”组成的）较小的函数组成的，每个步骤一个，连接在一个管道中。

![A function with many outputs](https://fsharpforfunandprofit.com/posts/recipe-part1/Recipe_FunctionMany2.png)

### 错误处理

在最后一个图中，有一个成功输出和三个错误输出。这是一个问题，因为函数只能有一个输出，而不是四个！

我们该如何应对？

答案是使用联合类型，用一个案例来表示每个不同的可能输出。然后，整个函数实际上只有一个输出。

以下是输出的可能类型定义示例：

```F#
type UseCaseResult =
    | Success
    | ValidationError
    | UpdateError
    | SmtpError
```

这是重新绘制的图表，显示了一个包含四个不同案例的单一输出：

![A function with a 4 case union output](https://fsharpforfunandprofit.com/posts/recipe-part1/Recipe_Function_Union4.png)

### 简化错误处理

这确实解决了问题，但流程中的每个步骤都有一个错误案例是脆弱的，不太可重用。我们能做得更好吗？

对！我们真正需要的是两个案例。一个用于快乐路径，一个用于所有其他错误路径，如下所示：

```F#
type UseCaseResult =
    | Success
    | Failure
```

![A function with a 2 case union output](https://fsharpforfunandprofit.com/posts/recipe-part1/Recipe_Function_Union2.png)

这种类型非常通用，适用于任何工作流！事实上，您很快就会看到，我们可以创建一个很好的有用函数库，这些函数可以与这种类型一起使用，并且可以在各种场景中重用。

不过还有一件事——就目前而言，结果中根本没有数据，只有成功/失败的状态。我们需要稍微调整一下，以便它可以包含一个实际的成功或失败对象。我们将使用泛型（也称为类型参数）指定成功类型和失败类型。

这是最终的、完全通用和可重用的版本：

```F#
type Result<'TSuccess,'TFailure> =
    | Success of 'TSuccess
    | Failure of 'TFailure
```

事实上，F# 库中已经定义了一个几乎与此完全相同的类型。这叫做 Choice。不过，为了清楚起见，我将在本篇和下一篇文章中继续使用上面定义的 `Result` 类型。当我们谈到更严肃的编码时，我们会重新审视这个问题。

所以，现在，再次显示各个步骤，我们可以看到，我们必须将每个步骤的错误合并到一个“失败”路径上。

![A function with two outputs](https://fsharpforfunandprofit.com/posts/recipe-part1/Recipe_Function_ErrorTrack.png)

如何做到这一点将是下一篇文章的主题。

## 总结和指导方针

到目前为止，我们有以下食谱指南：

指导方针

- 每个用例都相当于一个函数
- 用例函数将返回一个包含两种情况的联合类型：成功和失败。
- 用例函数将由一系列较小的函数构建，每个函数代表数据流中的一个步骤。
- 每个步骤的错误将合并到一个错误路径中。

# 2 面向铁路的编程

*Part of the "A recipe for a functional app" series (*[link](https://fsharpforfunandprofit.com/posts/recipe-part2/#series-toc)*)*

功能应用程序的配方，第2部分
2013年5月11日 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/recipe-part2/

更新：这里有更全面的演示文稿中的幻灯片和视频（如果你理解《非此即彼》，请先阅读这篇文章！）。

更新2：这是我最受欢迎的帖子之一，这是一种有用的错误处理方法，但请不要过度使用这个想法！请参阅我在“反对面向铁路的编程”上的帖子。

在上一篇文章中，我们看到了如何将用例分解为步骤，并将所有错误分流到单独的错误轨道上，如下所示：

![A function with two outputs](https://fsharpforfunandprofit.com/posts/recipe-part2/Recipe_Function_ErrorTrack.png)

在这篇文章中，我们将探讨将这些步骤函数连接到单个单元中的各种方法。这些功能的详细内部设计将在稍后的帖子中描述。

## 设计一个代表步骤的函数

让我们仔细看看这些步骤。例如，考虑验证功能。它将如何工作？有些数据进来了，但结果如何？

好吧，有两种可能的情况：要么数据有效（快乐路径），要么出了问题，在这种情况下，我们走上失败路径，绕过其余步骤，如下所示：

![The validation function with a two outputs](https://fsharpforfunandprofit.com/posts/recipe-part2/Recipe_Validation_Paths.png)

但和以前一样，这不是一个有效的函数。一个函数只能有一个输出，因此我们必须使用上次定义的 `Result` 类型：

```F#
type Result<'TSuccess,'TFailure> =
    | Success of 'TSuccess
    | Failure of 'TFailure
```

现在的图表看起来像这样：

![The validation function with a success/failure output](https://fsharpforfunandprofit.com/posts/recipe-part2/Recipe_Validation_Union2.png)

为了向您展示这在实践中是如何工作的，这里有一个实际验证函数的示例：

```F#
type Request = {name:string; email:string}

let validateInput input =
   if input.name = "" then Failure "Name must not be blank"
   else if input.email = "" then Failure "Email must not be blank"
   else Success input  // happy path
```

如果你看看函数的类型，编译器推断出它接受一个 `Request` 并输出一个 `Result` 作为输出，其中 `Request` 表示成功，字符串表示失败：

```F#
validateInput : Request -> Result<Request,string>
```

我们可以用同样的方法分析流程中的其他步骤。我们会发现每个都有相同的“形状”——某种输入，然后是成功/失败的输出。

先发制人的道歉：刚刚说过一个函数不能有两个输出，我可能会偶尔在下文中将其称为“两个输出”函数！当然，我的意思是函数输出的形状有两种情况。

## 面向铁路的编程

所以我们有很多这样的“一个输入->成功/失败输出”函数——我们如何将它们连接在一起？

我们想做的是将一个的 `Success` 输出连接到下一个的输入，但在 `Failure` 输出的情况下，以某种方式绕过第二个函数。此图给出了总体思路：

![Connecting validation function with update function](https://fsharpforfunandprofit.com/posts/recipe-part2/Recipe_Validation_Update.png)

这样做有一个很好的类比——你可能已经熟悉了。铁路！

铁路有道岔（英国的“道岔”），用于将列车引导到不同的轨道上。我们可以将这些“成功/失败”功能视为铁路道岔，如下所示：

![A railway switch](https://fsharpforfunandprofit.com/posts/recipe-part2/Recipe_RailwaySwitch.png)

这里我们有两个连续。

![2 railway switches disconnected](https://fsharpforfunandprofit.com/posts/recipe-part2/Recipe_RailwaySwitch1.png)

我们如何将它们结合起来，使两条故障轨道相互连接？很明显，就像这样！

![2 railway switches connected](https://fsharpforfunandprofit.com/posts/recipe-part2/Recipe_RailwaySwitch2.png)

如果我们有一系列的开关，我们最终会得到一个双轨系统，看起来像这样：

![3 railway switches connected](https://fsharpforfunandprofit.com/posts/recipe-part2/Recipe_RailwaySwitch3.png)

最上面的轨道是快乐的道路，最下面的轨道是失败的道路。

现在回过头来看看大局，我们可以看到，我们将有一系列黑匣子函数，它们似乎跨越了一条双轨铁路，每个函数都处理数据并将其传递给下一个函数：

![Opaque functions](https://fsharpforfunandprofit.com/posts/recipe-part2/Recipe_Railway_Opaque.png)

但如果我们看看函数内部，我们可以看到每个函数内部实际上都有一个开关，用于将坏数据分流到故障轨道上：

![Transparent functions](https://fsharpforfunandprofit.com/posts/recipe-part2/Recipe_Railway_Transparent.png)

请注意，一旦我们走上了失败的道路，我们就永远不会（通常）回到快乐的道路上。我们只是绕过其余的函数，直到到达末尾。

## 基本组成

在我们讨论如何将步骤功能“粘合”在一起之前，让我们先回顾一下组合是如何工作的。

想象一下，一个标准功能是一个坐在单轨铁路上的黑匣子（比如隧道）。它有一个输入和一个输出。

如果我们想连接一系列这些单轨函数，我们可以使用从左到右的组合运算符，符号为 `>>`。

![Composition of one-track functions](https://fsharpforfunandprofit.com/posts/recipe-part2/Recipe_Railway_Compose1.png)

同样的合成操作也适用于双轨函数：

![Composition of two-track functions](https://fsharpforfunandprofit.com/posts/recipe-part2/Recipe_Railway_Compose2.png)

组合的唯一约束是左侧函数的输出类型必须与右侧函数的输入类型匹配。

在我们的铁路类比中，这意味着你可以将一个轨道输出连接到一个轨道输入，或将两个轨道输出链接到两个轨道输入。

![Composition of two-track functions](https://fsharpforfunandprofit.com/posts/recipe-part2/Recipe_Railway_Compose3.png)

## 将开关转换为双轨输入

所以现在我们遇到了一个问题。

每个步骤的函数都是一个开关，有一个输入轨道。但整个流程需要一个双轨系统，每个功能都跨越两条轨道，这意味着每个函数都必须有一个双轨输入（前一个函数的结果输出），而不仅仅是一个简单的单轨输入（请求）。

我们如何将道岔插入双轨制？

答案很简单。我们可以创建一个“适配器”函数，它有一个用于开关函数的“孔”或“槽”，并将其转换为适当的双轨函数。以下是一个示例：

![Bind adapter](https://fsharpforfunandprofit.com/posts/recipe-part2/Recipe_Railway_BindAdapter.png)

这是实际代码的样子。我将把适配器函数命名为 `bind`，这是它的标准名称。

```F#
let bind switchFunction =
    fun twoTrackInput ->
        match twoTrackInput with
        | Success s -> switchFunction s
        | Failure f -> Failure f
```

bind 函数接受 switch 函数作为参数，并返回一个新函数。新函数接受双轨输入（类型为 `Result`），然后检查每种情况。如果输入为 `Success`，则调用 `switchFunction` 并输入值。但如果输入为 `Failure`，则开关功能将被绕过。

编译它，然后查看函数签名：

```F#
val bind : ('a -> Result<'b,'c>) -> Result<'a,'c> -> Result<'b,'c>
```

解释此签名的一种方法是 `bind` 函数有一个参数，一个开关函数（`'a -> Result<..>`），它返回一个完全双轨函数（`Result<..> -> Result<..>`）作为输出。

更具体地说：

- bind 的参数（`switchFunction`）采用某种类型 `'a`，并发出类型为 `'b`（用于成功跟踪）和“c”（用于失败跟踪）的结果
- 返回的函数本身有一个参数（`twoTrackInput`），它是类型为 `'a`（表示成功）和 `'c`（表示失败）的 `Result`。类型 `'a` 必须与 `switchFunction` 在其一个轨道上所期望的类型相同。
- 返回函数的输出是另一个 `Result`，这次类型为 `'b`（表示成功）和 `'c`（表示失败），与开关函数输出的类型相同。

如果你仔细想想，这种类型签名正是我们所期望的。

请注意，此函数是完全通用的——它适用于任何开关函数和任何类型。它只关心 `switchFunction` 的“形状”，而不是所涉及的实际类型。

### 编写 bind 函数的其他方法

顺便说一句，还有其他一些编写函数的方法。

一种方法是为 `twoTrackInput` 使用显式的第二个参数，而不是定义一个内部函数，如下所示：

```F#
let bind switchFunction twoTrackInput =
    match twoTrackInput with
    | Success s -> switchFunction s
    | Failure f -> Failure f
```

这与第一个定义完全相同。如果你想知道双参数函数如何与单参数函数完全相同，你需要阅读关于 currying 的文章！

另一种写作方式是替换 `match..with` 语法和更简洁的 `function` 关键字，如下所示：

```F#
let bind switchFunction =
    function
    | Success s -> switchFunction s
    | Failure f -> Failure f
```

您可能会在其他代码中看到这三种样式，但我个人更喜欢使用第二种样式（`let bind switchFunction twoTrackInput =`），因为我认为具有显式参数会使代码对非专家更具可读性。

> 如果你喜欢我用图片解释事物的方式，看看我的《领域建模使函数化》一书！这是对领域驱动设计、类型建模和函数式编程的友好介绍。

## 示例：组合一些验证函数

现在让我们写一点代码来测试这些概念。

让我们从我们已经定义的内容开始。`Request`、`Result` 和 `bind`：

```F#
type Result<'TSuccess,'TFailure> =
    | Success of 'TSuccess
    | Failure of 'TFailure

type Request = {name:string; email:string}

let bind switchFunction twoTrackInput =
    match twoTrackInput with
    | Success s -> switchFunction s
    | Failure f -> Failure f
```

接下来，我们将创建三个验证函数，每个函数都是一个“开关”函数，目标是将它们组合成一个更大的函数：

```F#
let validate1 input =
   if input.name = "" then Failure "Name must not be blank"
   else Success input

let validate2 input =
   if input.name.Length > 50 then Failure "Name must not be longer than 50 chars"
   else Success input

let validate3 input =
   if input.email = "" then Failure "Email must not be blank"
   else Success input
```

现在，为了将它们组合起来，我们将 `bind` 应用于每个验证函数，以创建一个新的双轨替代函数。

然后，我们可以使用标准函数组合来连接这两个被跟踪的函数，如下所示：

```F#
/// glue the three validation functions together
let combinedValidation =
    // convert from switch to two-track input
    let validate2' = bind validate2
    let validate3' = bind validate3
    // connect the two-tracks together
    validate1 >> validate2' >> validate3'
```

函数 `validate2’` 和 `validate3’` 是接受双轨输入的新函数。如果你看他们的签名，你会看到他们取一个 `Result` 并返回一个 `Result`。但请注意，`validate1` 不需要转换为双轨输入。根据构图的需要，它的输入保留为一个轨道，输出已经是两个轨道。

这是一个显示 `Validate1` 开关（未绑定）、`Validate2` 和 `Validate3` 开关以及 `Validate2'` 和 `Validate3'` 适配器的图。

![Validate2 and Validate3 connected](https://fsharpforfunandprofit.com/posts/recipe-part2/Recipe_Railway_Validator2and3.png)

我们也可以“内联” `bind`，如下所示：

```F#
let combinedValidation =
    // connect the two-tracks together
    validate1
    >> bind validate2
    >> bind validate3
```

让我们用两个坏输入和一个好输入来测试它：

```F#
// test 1
let input1 = {name=""; email=""}
combinedValidation input1
|> printfn "Result1=%A"

// ==> Result1=Failure "Name must not be blank"

// test 2
let input2 = {name="Alice"; email=""}
combinedValidation input2
|> printfn "Result2=%A"

// ==> Result2=Failure "Email must not be blank"

// test 3
let input3 = {name="Alice"; email="good"}
combinedValidation input3
|> printfn "Result3=%A"

// ==> Result3=Success {name = "Alice"; email = "good";}
```

我鼓励您自己尝试一下，并尝试验证功能和测试输入。

*您可能想知道是否有办法并行运行所有三个验证，而不是串行运行，这样您就可以一次返回所有验证错误。是的，有一种方法，我将在本文稍后解释。*

### 绑定为管道操作

当我们讨论 `bind` 函数时，它有一个共同的符号 `>>=`，用于将值管道传输到开关函数中。

这是定义，它围绕这两个参数进行切换，使它们更容易链接在一起：

```F#
/// create an infix operator
let (>>=) twoTrackInput switchFunction =
    bind switchFunction twoTrackInput
```

*记住该符号的一种方法是将其视为组成符号 `>>`，然后是双轨铁路符号 `=`。*

当这样使用时，`>>=` 运算符有点像管道（`|>`），但用于开关函数。

在普通管道中，左侧是一个单轨值，右侧是一个普通函数。但在“绑定管道”操作中，左侧是一个双轨值，右侧是一个开关函数。

这里使用它来创建 `combinedValidation` 函数的另一个实现。

```F#
let combinedValidation x =
    x
    |> validate1   // normal pipe because validate1 has a one-track input
                   // but validate1 results in a two track output...
    >>= validate2  // ... so use "bind pipe". Again the result is a two track output
    >>= validate3   // ... so use "bind pipe" again.
```

此实现与前一个实现的区别在于，此定义是面向数据的，而不是面向函数的。它有一个显式的初始数据值参数，即`x`。`x` 被传递给第一个函数，然后其输出被传递给第二个函数，依此类推。

在之前的实现中（下面重复），根本没有数据参数！重点是函数本身，而不是流经它们的数据。

```F#
let combinedValidation =
    validate1
    >> bind validate2
    >> bind validate3
```

## 绑定的替代方案

组合开关的另一种方法不是使它们适应双轨输入，而是简单地将它们直接连接在一起，形成一个新的、更大的开关。

换言之，这：

![2 railway switches disconnected](https://fsharpforfunandprofit.com/posts/recipe-part2/Recipe_RailwaySwitch1.png)

变成这样：

![2 railway switches connected](https://fsharpforfunandprofit.com/posts/recipe-part2/Recipe_RailwaySwitch2.png)

但如果你仔细想想，这条组合赛道实际上只是另一个转折点！如果你盖住中间的位，你可以看到这一点。有一个输入和两个输出：

![2 railway switches connected](https://fsharpforfunandprofit.com/posts/recipe-part2/Recipe_RailwaySwitch2a.png)

所以我们真正做的是一种开关的组合形式，就像这样：

![switches composition](https://fsharpforfunandprofit.com/posts/recipe-part2/Recipe_Railway_MComp.png)

因为每个组合只会产生另一个开关，所以我们总是可以再次添加另一个切换，从而产生一个更大的东西，它仍然是一个开关等等。

这是开关组合的代码。使用的标准符号是 `>=>`，有点像普通的合成符号，但在角度之间有一条铁路轨道。

```F#
let (>=>) switch1 switch2 x =
    match switch1 x with
    | Success s -> switch2 s
    | Failure f -> Failure f
```

同样，实际实施非常简单。将单轨输入 `x` 通过第一个开关。成功后，将结果传递给第二个开关，否则完全绕过第二个交换机。

现在我们可以重写 `combinedValidation` 函数，使用开关组合而不是绑定：

```F#
let combinedValidation =
    validate1
    >=> validate2
    >=> validate3
```

我认为这是最简单的。当然，它很容易扩展，如果我们有第四个验证函数，我们可以把它附加到末尾。

### 绑定 vs. 切换组合

我们有两个不同的概念，乍一看似乎非常相似。有什么区别？

总结一下：

- **Bind** 有一个开关函数参数。它是一个适配器，可以将开关功能转换为完全双轨功能（具有双轨输入和双轨输出）。
- **开关组合**有两个开关函数参数。它将它们串联在一起，以实现另一个开关函数。

那么，为什么要使用绑定而不是切换组合呢？这取决于上下文。如果您有一个现有的双轨系统，并且需要插入一个开关，那么您必须使用 bind 作为适配器将开关转换为需要双轨输入的东西。

![switches composition](https://fsharpforfunandprofit.com/posts/recipe-part2/Recipe_Railway_WhyBind.png)

另一方面，如果您的整个数据流由一系列开关组成，那么开关组合可以更简单。

![switches composition](https://fsharpforfunandprofit.com/posts/recipe-part2/Recipe_Railway_WhyCompose.png)

### 从绑定角度切换组合

碰巧的是，开关组合可以用绑定来编写。如果你将第一个开关与一个绑定适配的第二个开关连接起来，你会得到与开关组合相同的结果：

这里有两个单独的开关：

![2 railway switches disconnected](https://fsharpforfunandprofit.com/posts/recipe-part2/Recipe_RailwaySwitch1.png)

然后，以下是组合在一起的开关，以形成一个新的更大的开关：

![2 railway switches disconnected](https://fsharpforfunandprofit.com/posts/recipe-part2/Recipe_RailwaySwitch2.png)

在第二个开关上使用 `bind` 也可以完成同样的事情：

![bind as switch composition](https://fsharpforfunandprofit.com/posts/recipe-part2/Recipe_Railway_BindIsCompose.png)

以下是使用这种思维方式重写的开关组合运算符：

```F#
let (>=>) switch1 switch2 =
    switch1 >> (bind switch2)
```

这种开关组合的实现比第一种简单得多，但也更抽象。对于初学者来说，这是否更容易理解是另一回事！我发现，如果你把函数本身看作是事物，而不仅仅是数据的管道，这种方法就更容易理解。

## 将简单函数转换为面向铁路的编程模型

一旦你掌握了窍门，你就可以将各种其他东西放入这个模型中。

例如，假设我们有一个不是开关的函数，只是一个常规函数。并说我们想把它插入到我们的流程中。

这是一个真实的例子——假设我们想在验证完成后修剪并小写电子邮件地址。以下是一些代码：

```F#
let canonicalizeEmail input =
   { input with email = input.email.Trim().ToLower() }
```

此代码接受一个（单轨）`Request` 并返回一个（双轨）`Request`。

我们如何在验证步骤之后但在更新步骤之前插入此内容？

好吧，如果我们能把这个简单的函数变成一个开关函数，那么我们就可以使用上面提到的开关组合。

换句话说，我们需要一个适配器块。它与我们用于 `bind` 的概念相同，只是这次我们的适配器块将有一个用于一个轨道功能的插槽，适配器块的整体“形状”是一个开关。

![lifting a simple function](https://fsharpforfunandprofit.com/posts/recipe-part2/Recipe_Railway_SwitchAdapter.png)

执行此操作的代码很简单。我们需要做的就是将单轨函数的输出转化为双轨结果。在这种情况下，结果总是成功。

```F#
// convert a normal function into a switch
let switch f x =
    f x |> Success
```

在铁路方面，我们增加了一些故障轨道。总的来说，它看起来像一个开关功能（一个轨道输入，两个轨道输出），但当然，故障轨道只是一个虚拟的，开关从未被实际使用过。

![lifting a simple function](https://fsharpforfunandprofit.com/posts/recipe-part2/Recipe_Railway_SwitchAdapter2.png)

一旦 `switch` 可用，我们就可以很容易地将 `canonicalizeEmail` 函数附加到链的末尾。既然我们开始扩展它，让我们将函数重命名为 `usecase`。

```F#
let usecase =
    validate1
    >=> validate2
    >=> validate3
    >=> switch canonicalizeEmail
```

试着测试一下，看看会发生什么：

```F#
let goodInput = {name="Alice"; email="UPPERCASE   "}
usecase goodInput
|> printfn "Canonicalize Good Result = %A"

//Canonicalize Good Result = Success {name = "Alice"; email = "uppercase";}

let badInput = {name=""; email="UPPERCASE   "}
usecase badInput
|> printfn "Canonicalize Bad Result = %A"

//Canonicalize Bad Result = Failure "Name must not be blank"
```

## 从一个轨迹函数创建两个轨迹函数

在前面的示例中，我们采用了一个单轨函数并从中创建了一个开关。这使我们能够使用开关组合。

不过，有时您想直接使用双轨模型，在这种情况下，您想直接将单轨函数转换为双轨函数。

![mapping a simple function](https://fsharpforfunandprofit.com/posts/recipe-part2/Recipe_Railway_MapAdapter2.png)

同样，我们只需要一个带有插槽的适配器块来实现简单的功能。我们通常称之为适配器 `map`。

![mapping a simple function](https://fsharpforfunandprofit.com/posts/recipe-part2/Recipe_Railway_MapAdapter.png)

而且，实际实施非常简单。如果双轨输入是 `Success`，则调用该函数，并将其输出转换为 Success。另一方面，如果双轨输入为 `Failure`，则完全绕过该功能。

代码如下：

```F#
// convert a normal function into a two-track function
let map oneTrackFunction twoTrackInput =
    match twoTrackInput with
    | Success s -> Success (oneTrackFunction s)
    | Failure f -> Failure f
```

这里它与 `canonicalizationEmail` 一起使用：

```F#
let usecase =
    validate1
    >=> validate2
    >=> validate3
    >> map canonicalizeEmail  // normal composition
```

请注意，现在使用正常的组合，因为 `map canonicalizeEmail` 是一个完全双轨的函数，可以直接连接到 `validate3` 开关的输出。

换句话说，对于单轨迹函数，`>=> switch` 与 `>> map` 完全相同。你的选择。

## 将死端函数转换为双轨函数

我们经常想使用的另一个函数是“死胡同”函数——一个接受输入但没有有用输出的函数。

例如，考虑一个更新数据库记录的函数。它只对其副作用有用——它通常不会返回任何东西。

我们如何将这种功能整合到流程中？

我们需要做的是：

- 保存输入的副本。
- 调用函数并忽略其输出（如果有的话）。
- 返回原始输入，以便传递给链中的下一个函数。

从铁路的角度来看，这相当于创建一个死胡同，就像这样。

![tee for a dead end function](https://fsharpforfunandprofit.com/posts/recipe-part2/Recipe_Railway_Tee.png)

为了实现这一点，我们需要另一个适配器函数，比如 `switch`，除了这次它有一个用于单轨道死端函数的插槽，并将其转换为具有单轨道输出的单轨道直通函数。

![tee for a dead end function](https://fsharpforfunandprofit.com/posts/recipe-part2/Recipe_Railway_TeeAdapter.png)

以下是遵循 UNIX tee 命令之后我将称之为 `tee` 的代码：

```F#
let tee f x =
    f x |> ignore
    x
```

一旦我们将死端函数转换为简单的单轨传递函数，我们就可以在数据流中使用它，如上所述，通过使用 `switch` 或 `map` 进行转换。

以下是使用“开关组合”样式的代码：

```F#
// a dead-end function
let updateDatabase input =
   ()   // dummy dead-end function for now

let usecase =
    validate1
    >=> validate2
    >=> validate3
    >=> switch canonicalizeEmail
    >=> switch (tee updateDatabase)
```

或者，我们可以使用 `map` 并用 `>>` 连接，而不是使用 `switch` 然后用 `>=>` 连接。

这是一个变体实现，它完全相同，但使用“双轨”风格和正常组合

```F#
let usecase =
    validate1
    >> bind validate2
    >> bind validate3
    >> map canonicalizeEmail
    >> map (tee updateDatabase)
```

## 异常处理

我们的死胡同数据库更新可能不会返回任何东西，但这并不意味着它可能不会抛出异常。我们希望捕获该异常并将其转化为失败，而不是崩溃。

该代码类似于 `switch` 函数，除了它捕获异常。我称之为 `tryCatch`：

```F#
let tryCatch f x =
    try
        f x |> Success
    with
    | ex -> Failure ex.Message
```

这是数据流的修改版本，使用 `tryCatch` 而不是 `switch` 来更新数据库代码。

```F#
let usecase =
    validate1
    >=> validate2
    >=> validate3
    >=> switch canonicalizeEmail
    >=> tryCatch (tee updateDatabase)
```

## 具有双轨输入的函数

到目前为止，我们看到的所有函数都只有一个输入，因为它们总是只处理沿着快乐路径传输的数据。

不过，有时您确实需要一个处理两条轨迹的函数。例如，一个记录错误和成功的日志功能。

正如我们之前所做的那样，我们将创建一个适配器块，但这次它将有两个单独的单轨函数的插槽。

![double map adapter](https://fsharpforfunandprofit.com/posts/recipe-part2/Recipe_Railway_DoubleMapAdapter.png)

代码如下：

```F#
let doubleMap successFunc failureFunc twoTrackInput =
    match twoTrackInput with
    | Success s -> Success (successFunc s)
    | Failure f -> Failure (failureFunc f)
```

另外，我们可以使用此函数创建一个更简单的 `map` 版本，使用 `id` 作为 failure 函数：

```F#
let map successFunc =
    doubleMap successFunc id
```

让我们使用 `doubleMap` 在数据流中插入一些日志：

```F#
let log twoTrackInput =
    let success x = printfn "DEBUG. Success so far: %A" x; x
    let failure x = printfn "ERROR. %A" x; x
    doubleMap success failure twoTrackInput

let usecase =
    validate1
    >=> validate2
    >=> validate3
    >=> switch canonicalizeEmail
    >=> tryCatch (tee updateDatabase)
    >> log
```

以下是一些测试代码及其结果：

```F#
let goodInput = {name="Alice"; email="good"}
usecase goodInput
|> printfn "Good Result = %A"

// DEBUG. Success so far: {name = "Alice"; email = "good";}
// Good Result = Success {name = "Alice"; email = "good";}

let badInput = {name=""; email=""}
usecase badInput
|> printfn "Bad Result = %A"

// ERROR. "Name must not be blank"
// Bad Result = Failure "Name must not be blank"
```

## 将单个值转换为双轨值

为了完整起见，我们还应该创建简单的函数，将单个简单值转换为双轨值，无论是成功还是失败。

```F#
let succeed x =
    Success x

let fail x =
    Failure x
```

现在这些都是微不足道的，只是调用Result类型的构造函数，但当我们进行适当的编码时，我们会看到，通过直接使用这些而不是联合案例构造函数，我们可以将自己与幕后的更改隔离开来。

## 并行组合函数

到目前为止，我们已经将功能串联在一起。但是，对于验证之类的事情，我们可能希望并行运行多个开关，并组合结果，如下所示：

![switches in parallel](https://fsharpforfunandprofit.com/posts/recipe-part2/Recipe_Railway_Parallel.png)

为了使这更容易，我们可以重用我们为切换组合所做的相同技巧。与其一次做很多，如果我们只关注一对，并“添加”它们以进行新的切换，那么我们可以很容易地将“添加”链接在一起，这样我们就可以添加任意数量的。换句话说，我们只需要实现这一点：

![add two switches in parallel](https://fsharpforfunandprofit.com/posts/recipe-part2/Recipe_Railway_MPlus.png)

那么，并行添加两个开关的逻辑是什么？

- 首先，获取输入并将其应用于每个开关。
- 接下来查看两个开关的输出，如果两个开关都成功，则总体结果为 `Success`。
- 如果任一输出失败，则总体结果也是 `Failure`。

这是我将调用 `plus` 的函数：

```F#
let plus switch1 switch2 x =
    match (switch1 x),(switch2 x) with
    | Success s1,Success s2 -> Success (s1 + s2)
    | Failure f1,Success _  -> Failure f1
    | Success _ ,Failure f2 -> Failure f2
    | Failure f1,Failure f2 -> Failure (f1 + f2)
```

但我们现在面临一个新问题。我们如何处理两次成功或两次失败？我们如何将内在价值观结合起来？

我在上面的例子中使用了 `s1 + s2` 和 `f1 + f2`，但这意味着我们可以使用某种 `+` 运算符。对于字符串和整数来说，这可能是正确的，但一般来说并不正确。

组合值的方法可能会在不同的上下文中发生变化，因此，与其试图一劳永逸地解决它，不如让调用者传入所需的函数来进行下注。

这是一个重写版本：

```F#
let plus addSuccess addFailure switch1 switch2 x =
    match (switch1 x),(switch2 x) with
    | Success s1,Success s2 -> Success (addSuccess s1 s2)
    | Failure f1,Success _  -> Failure f1
    | Success _ ,Failure f2 -> Failure f2
    | Failure f1,Failure f2 -> Failure (addFailure f1 f2)
```

我把这些新函数放在参数列表的第一位，以帮助部分应用。

### 并行验证的实现

现在，让我们为验证函数创建一个“plus”的实现。

- 当两个函数都成功时，它们将原封不动地返回请求，因此 `addSuccess` 函数可以返回任一参数。
- 当两个函数都失败时，它们将返回不同的字符串，因此 `addFailure` 函数应该将它们连接起来。

为了验证，我们想要的“plus”操作就像一个“AND”函数。只有当这两个部分都是“真”时，结果才是“真”。

这自然会导致想要使用 `&&` 作为运算符符号。很遗憾，`&&` 是保留的，但我们可以使用 `&&&`，如下所示：

```F#
// create a "plus" function for validation functions
let (&&&) v1 v2 =
    let addSuccess r1 r2 = r1 // return first
    let addFailure s1 s2 = s1 + "; " + s2  // concat
    plus addSuccess addFailure v1 v2
```

现在使用 `&&&`，我们可以创建一个结合了三个较小验证的单一验证函数：

```F#
let combinedValidation =
    validate1
    &&& validate2
    &&& validate3
```

现在，让我们用之前的测试来尝试一下：

```F#
// test 1
let input1 = {name=""; email=""}
combinedValidation input1
|> printfn "Result1=%A"
// ==>  Result1=Failure "Name must not be blank; Email must not be blank"

// test 2
let input2 = {name="Alice"; email=""}
combinedValidation input2
|> printfn "Result2=%A"
// ==>  Result2=Failure "Email must not be blank"

// test 3
let input3 = {name="Alice"; email="good"}
combinedValidation input3
|> printfn "Result3=%A"
// ==>  Result3=Success {name = "Alice"; email = "good";}
```

正如我们所希望的那样，第一个测试现在将两个验证错误组合成一个字符串。

接下来，我们可以使用现在的 `usecase` 函数来整理主数据流函数，而不是之前的三个单独的验证函数：

```F#
let usecase =
    combinedValidation
    >=> switch canonicalizeEmail
    >=> tryCatch (tee updateDatabase)
```

如果我们现在测试一下，我们可以看到成功一直持续到最后，电子邮件的大小被降低和修剪：

```F#
// test 4
let input4 = {name="Alice"; email="UPPERCASE   "}
usecase input4
|> printfn "Result4=%A"
// ==>  Result4=Success {name = "Alice"; email = "uppercase";}
```

*你可能会问，我们也可以创建一种操作验证函数的方法吗？也就是说，如果任何一部分都有效，那么总体结果就是有效的？当然，答案是肯定的。试试看！我建议你用符号 `|||` 来表示。*

## 函数的动态注入

我们可能想做的另一件事是根据配置设置甚至数据内容动态地向流中添加或删除函数。

最简单的方法是创建一个双轨函数注入到流中，如果不需要，用 `id` 函数替换它。

想法如下：

```F#
let injectableFunction =
    if config.debug then debugLogger else id
```

让我们用一些真实的代码来尝试一下：

```F#
type Config = {debug:bool}

let debugLogger twoTrackInput =
    let success x = printfn "DEBUG. Success so far: %A" x; x
    let failure = id // don't log here
    doubleMap success failure twoTrackInput

let injectableLogger config =
    if config.debug then debugLogger else id

let usecase config =
    combinedValidation
    >> map canonicalizeEmail
    >> injectableLogger config
```

以下是它的使用情况：

```F#
let input = {name="Alice"; email="good"}

let releaseConfig = {debug=false}
input
|> usecase releaseConfig
|> ignore

// no output

let debugConfig = {debug=true}
input
|> usecase debugConfig
|> ignore

// debug output
// DEBUG. Success so far: {name = "Alice"; email = "good";}
```

## 铁路轨道函数：工具包

让我们退一步，回顾一下我们迄今为止所做的工作。

以铁路轨道为比喻，我们创建了许多有用的构建块，这些构建块可以与任何数据流风格的应用程序一起使用。

我们可以大致这样对函数进行分类：

- “**构造函数**”用于创建新轨迹。
- “**适配器**”将一种轨道转换为另一种轨道。
- “**组合器**”将轨道的各个部分连接在一起，形成更大的轨道。

这些函数形成了可以松散地称为*组合子库*的东西，也就是说，一组旨在与一种类型（这里由铁路轨道表示）一起工作的函数，其设计目标是通过调整和组合较小的部分来构建较大的部分。

`bind`、`map`、`plus` 等函数出现在各种函数式编程场景中，因此您可以将它们视为函数式模式——类似于但不相同于 OO 模式，如“访问者”、“单例”、“外观”等。

他们都在一起：

| 概念        | 描述                                                         |
| :---------- | :----------------------------------------------------------- |
| `succeed`   | 一个构造函数，在 Success 分支上取一个单轨值并创建一个双轨值。在其他情况下，这也可能被称为 `return` 或 `pure`。 |
| `fail`      | 一个构造函数，它在 Failure 分支上取一个单轨值并创建一个双轨值。 |
| `bind`      | 一个适配器，它接受开关函数并创建一个新函数，该函数接受两个轨道值作为输入。 |
| `>>=`       | 一种中缀版本的 bind，用于将两个轨迹值管道传输到开关函数中。  |
| `>>`        | 正常组成。一种组合器，它接受两个正常函数，并通过串联连接它们来创建一个新函数。 |
| `>=>`       | 切换组合。一种组合器，它接受两个开关函数，并通过串联连接它们来创建新的开关函数。 |
| `switch`    | 一种适配器，它具有正常的单轨功能，并将其转换为开关功能。（在某些情况下也称为“提升”。） |
| `map`       | 一种适配器，它具有正常的单轨功能，并将其转换为双轨功能。（在某些情况下也称为“提升”。） |
| `tee`       | 一种适配器，它采用死端函数并将其转换为可在数据流中使用的单轨函数。（也称为 `tap`。） |
| `tryCatch`  | 一种适配器，它接受正常的单轨功能并将其转换为开关功能，但也会捕获异常。 |
| `doubleMap` | 一种适配器，它接受两个单轨功能并将其转换为一个双轨功能。（也称为 `bimap`。） |
| `plus`      | 一个组合器，它接受两个开关函数，并通过“并行”连接它们并“添加”结果来创建一个新的开关函数。（在其他上下文中也称为 `++` 和 `<+>`。） |
| `&&&`       | “plus”组合器专门针对验证函数进行了调整，以二进制 AND 为模型。 |

### 铁路轨道函数：完整代码

以下是所有函数的完整代码。

我对上面给出的原始代码进行了一些细微的调整：

- 现在，大多数函数都是根据一个名为 `either` 的核心函数来定义的。
- `tryCatch` 为异常处理程序提供了一个额外的参数。

```F#
// the two-track type
type Result<'TSuccess,'TFailure> =
    | Success of 'TSuccess
    | Failure of 'TFailure

// convert a single value into a two-track result
let succeed x =
    Success x

// convert a single value into a two-track result
let fail x =
    Failure x

// apply either a success function or failure function
let either successFunc failureFunc twoTrackInput =
    match twoTrackInput with
    | Success s -> successFunc s
    | Failure f -> failureFunc f

// convert a switch function into a two-track function
let bind f =
    either f fail

// pipe a two-track value into a switch function
let (>>=) x f =
    bind f x

// compose two switches into another switch
let (>=>) s1 s2 =
    s1 >> bind s2

// convert a one-track function into a switch
let switch f =
    f >> succeed

// convert a one-track function into a two-track function
let map f =
    either (f >> succeed) fail

// convert a dead-end function into a one-track function
let tee f x =
    f x; x

// convert a one-track function into a switch with exception handling
let tryCatch f exnHandler x =
    try
        f x |> succeed
    with
    | ex -> exnHandler ex |> fail

// convert two one-track functions into a two-track function
let doubleMap successFunc failureFunc =
    either (successFunc >> succeed) (failureFunc >> fail)

// add two switches in parallel
let plus addSuccess addFailure switch1 switch2 x =
    match (switch1 x),(switch2 x) with
    | Success s1,Success s2 -> Success (addSuccess s1 s2)
    | Failure f1,Success _  -> Failure f1
    | Success _ ,Failure f2 -> Failure f2
    | Failure f1,Failure f2 -> Failure (addFailure f1 f2)
```

## 类型 vs. 形状

到目前为止，我们完全专注于轨道的形状，而不是火车上的货物。

这是一条神奇的铁路，运输的货物可以在每条轨道上变化。

例如，一批菠萝在通过名为 `function1` 的隧道时会神奇地变成苹果。

![pineapples to apples](https://fsharpforfunandprofit.com/posts/recipe-part2/Recipe_Railway_Cargo1.png)

当一批苹果通过名为 `function2` 的隧道时，它会变成香蕉。

![apples to bananas](https://fsharpforfunandprofit.com/posts/recipe-part2/Recipe_Railway_Cargo2.png)

这条神奇的铁路有一个重要的规则，即你只能连接运载同种货物的轨道。在这种情况下，我们可以将 `function1` 连接到 `function2` ，因为从 `function1` 出来的货物（苹果）与进入 `function2` 的货物（也是苹果）相同。

![connecting functions](https://fsharpforfunandprofit.com/posts/recipe-part2/Recipe_Railway_Cargo3.png)

当然，轨道运载的货物并不总是相同的，货物种类的不匹配会导致错误。

但你会注意到，在这次讨论中，到目前为止，我们一次也没有提到过货物！相反，我们花了所有的时间来讨论单轨道与双轨功能。

当然，货物必须匹配，这是不言而喻的。但我希望你能看到，真正重要的是轨道的形状，而不是所运载的货物。

### 泛型类型功能强大

为什么我们不担心货物的类型？因为所有的“适配器”和“组合器”功能都是完全通用的！`bind`、`map`、`switch` 和 `plus` 功能不关心货物的类型，只关心轨道的形状。

具有极其通用的功能有两个好处。第一种方法是显而易见的：函数越通用，它的可重用性就越高。`bind` 的实现将适用于任何类型（只要形状正确）。

但泛型函数还有另一个更微妙的方面值得指出。因为我们通常对所涉及的类型一无所知，所以我们在能做什么和不能做什么方面受到很大限制。因此，我们不能引入 bug！

为了理解我的意思，让我们看看 `map` 的签名：

```F#
val map : ('a -> 'b) -> (Result<'a,'c> -> Result<'b,'c>)
```

它接受一个函数参数 `'a->'b` 和一个值 `Result<'a, 'c>`，并返回一个值 `Result<'b, 'c>`。

我们对类型 `'a`、`'b` 和 `'c` 一无所知。我们只知道：

- 在函数参数和第一个 `Result` 的 `Success` 案例中都显示了相同的类型 `'a`。
- 在函数参数和第二个 `Result` 的 `Success` 案例中都显示了相同的类型 `'b`。
- 相同的类型 `'c` 出现在第一个和第二个 `Result`s 的 `Failure` 案例中，但根本没有出现在函数参数中。

我们能从中推断出什么？

返回值中有一个类型 `'b`。但是它从哪里来？我们不知道 `'b` 型是什么，所以我们不知道如何制作一个。但是函数参数知道如何创建一个！给它一个 `'a`，它就会给我们一个 `'b`。

但是我们从哪里可以得到 `'a` 呢？我们也不知道 `'a` 是什么类型，所以我们也不清楚如何制作。但是第一个 result 参数有一个我们可以使用的 `'a`，所以你可以看到我们被迫从 `Result<'a, 'c>` 参数中获取 `Success` 值并将其传递给函数参数。然后，必须根据函数的结果构造 `Result<'b, 'c>` 返回值的 `Success` 情况。

最后，同样的逻辑也适用于 `'c`。我们被迫从 `Result<'a, 'c>` 输入参数中获取 `Failure` 值，并使用它来构造 `Result<'a, 'c>` 返回值的 `Failure` 情况。

换句话说，基本上只有一种方法可以实现 `map` 功能！类型签名是如此泛型，我们别无选择。

另一方面，想象一下 `map` 函数对它需要的类型非常具体，就像这样：

```F#
val map : (int -> int) -> (Result<int,int> -> Result<int,int>)
```

在这种情况下，我们可以提出大量不同的实现。举几个例子：

- 我们本可以互换成功和失败的轨迹。
- 我们本可以在成功轨迹中添加一个随机数。
- 我们本可以完全忽略函数参数，在成功和失败的轨道上都返回零。

所有这些实现都是“有缺陷的”，因为它们没有达到我们的期望。但它们都是可能的，因为我们事先知道类型是 `int`，因此我们可以以不应该的方式操纵值。我们对类型了解得越少，出错的可能性就越小。

### 故障类型

在我们的大多数功能中，转换仅适用于成功轨道。故障跟踪被单独保留（`map`），或与传入的故障合并（`bind`）。

这意味着故障轨迹必须始终保持相同的类型。在这篇文章中，我们只使用了 `string`，但在下一篇文章中我们将把失败类型更改为更有用的类型。

## 总结和指导方针

在这个系列的开始，我答应给你一个简单的食谱，你可以遵循。

但你现在可能有点不知所措。我没有让事情变得更简单，反而让事情变得更加复杂。我已经向你展示了很多不同的方法来做同样的事情！绑定 vs. 组合。映射 vs. 开关。你应该使用哪种方法？哪种方式最好？

当然，从来没有一种“正确的方法”适用于所有情况，但正如所承诺的那样，这里有一些指导方针，可以作为可靠和可重复配方的基础。

指导方针

- 使用双轨铁路作为数据流情况的底层模型。
- 为用例中的每个步骤创建一个函数。每个步骤的函数可以由较小的函数（例如验证函数）构建。
- 使用标准组合（`>>`）连接功能。
- 如果需要在流中插入开关，请使用 `bind`。
- 如果需要在流程中插入单轨函数，请使用 `map`。
- 如果需要在流中插入其他类型的函数，请创建一个适当的适配器块并使用它。

这些指导方针可能会导致代码不够简洁或优雅，但另一方面，您将使用一致的模型，并且在需要维护时，其他人应该可以理解。

因此，根据这些指导方针，以下是到目前为止实现的主要部分。请特别注意在最终 `usecase` 函数中的所有地方都使用 `>>`。

```F#
open RailwayCombinatorModule

let (&&&) v1 v2 =
    let addSuccess r1 r2 = r1 // return first
    let addFailure s1 s2 = s1 + "; " + s2  // concat
    plus addSuccess addFailure v1 v2

let combinedValidation =
    validate1
    &&& validate2
    &&& validate3

let canonicalizeEmail input =
   { input with email = input.email.Trim().ToLower() }

let updateDatabase input =
   ()   // dummy dead-end function for now

// new function to handle exceptions
let updateDatebaseStep =
    tryCatch (tee updateDatabase) (fun ex -> ex.Message)

let usecase =
    combinedValidation
    >> map canonicalizeEmail
    >> bind updateDatebaseStep
    >> log
```

最后一个建议。如果你和一个非专家团队一起工作，不熟悉的运算符符号会让人反感。所以这里有一些关于运算符的额外指南：

- 除了 `>>` 和 `|>` 之外，不要使用任何“奇怪”的运算符。
- 特别是，这意味着你不应该使用像 `>>=` 或 `>=>` 这样的运算符，除非每个人都知道它们。
- 如果在使用它的模块或函数的顶部定义运算符，则可能会出现异常。例如，可以在验证模块的顶部定义 `&&&` 运算符，然后在同一模块中稍后使用。

## 进一步阅读

- 如果你喜欢这种“面向铁路”的方法，你也可以看到它应用于 FizzBuzz。
- 我还有一些幻灯片和视频，展示了如何进一步采用这种方法。（在某个时候，我会把这些变成一篇合适的博客文章）

我在 2014 年奥斯陆 NDC 上就这一主题发表了演讲（点击图片观看视频）

[![Video from NDC Oslo 2014](https://fsharpforfunandprofit.com/posts/recipe-part2/rop-ndcoslo.jpg)](http://vimeo.com/97344498)

以下是我使用的幻灯片：

从我在 Slideshare 上的幻灯片看面向铁路的编程

> 如果你对领域建模和设计的功能方法感兴趣，你可能会喜欢我的《领域建模函数化》一书！这是一个初学者友好的介绍，涵盖了领域驱动设计、类型建模和函数式编程。



# 3 在项目中组织模块

*Part of the "A recipe for a functional app" series (*[link](https://fsharpforfunandprofit.com/posts/recipe-part3/#series-toc)*)*

功能应用程序的配方，第3部分
2013年5月26日 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/recipe-part3/

在我们继续进行配方中的任何编码之前，让我们先看看F#项目的整体结构。特别是：（a）哪些代码应该包含在哪些模块中，以及（b）如何在项目中组织这些模块。

## 怎么不做

F# 的新手可能会像 C# 一样在类中组织代码。每个文件一个类，按字母顺序排列。毕竟，F# 支持与 C# 相同的面向对象特性，对吗？那么，F# 代码可以像 C# 代码一样组织吗？

一段时间后，人们通常会发现 F# 要求文件（以及文件中的代码）按依赖顺序排列。也就是说，您不能对编译器尚未看到的代码使用正向引用**。

接下来是普遍的恼怒和咒骂。F# 怎么会这么愚蠢？当然，写任何大型项目都是不可能的！

在这篇文章中，我们将介绍一种简单的方法来组织你的代码，这样就不会发生这种情况。

**在某些情况下，可以使用 `and` 关键字来允许相互递归，但不鼓励使用。

## 分层设计的功能方法

思考代码的一种标准方式是将其分为多个层：域层、表示层等，如下所示：

![Design layers](https://fsharpforfunandprofit.com/posts/recipe-part3/Recipe_DesignLayers1.png)

每一层只包含与该层相关的代码。

但在实践中，事情并没有那么简单，因为每一层之间都有依赖关系。领域层取决于基础设施，表示层取决于领域。

最重要的是，领域层不应该依赖于持久层。也就是说，它应该是“与持久性无关的”。

因此，我们需要调整层图，使其看起来更像这样（其中每个箭头代表一个依赖关系）：

![Design layers](https://fsharpforfunandprofit.com/posts/recipe-part3/Recipe_DesignLayers1a.png)

理想情况下，这种重组将更加细粒度，有一个单独的“服务层”，包含应用程序服务、域服务等。当我们完成时，核心域类是“纯”的，不依赖于域外的任何其他东西。这通常被称为“六边形建筑”或“洋葱式建筑”。但这篇文章不是关于 OO 设计的微妙之处，所以现在，让我们只使用更简单的模型。

## 将行为与类型分离

*“在一个数据结构上运行 100 个函数比在 10 个数据结构中运行 10 个函数要好”——Alan Perlis*

在功能设计中，将行为与数据分离非常重要。数据类型简单而“愚蠢”。然后，分别地，您有许多函数对这些数据类型进行操作。

这与面向对象的设计完全相反，在面向对象设计中，行为和数据是要结合在一起的。毕竟，这正是类的本质。事实上，在真正的面向对象设计中，你应该只有行为——数据是私有的，只能通过方法访问。

事实上，在面向对象设计中，围绕数据类型没有足够的行为被认为是一件坏事，甚至有一个名字：“贫血领域模型”。

然而，在函数式设计中，拥有透明的“哑数据”是首选。数据在不被封装的情况下被暴露通常是可以的。数据是不可变的，因此它不会被行为异常的函数“损坏”。事实证明，对透明数据的关注允许更多更灵活、更通用的代码。

如果你还没有看过，我强烈推荐 Rich Hickey 关于“值的价值观”的精彩演讲，该演讲解释了这种方法的好处。

### 类型层和行为层

那么，这如何适用于我们上面的分层设计呢？

首先，我们必须将每一层分为两个不同的部分：

- **数据类型**。该层使用的数据结构。
- **逻辑**。在该层中实现的功能。

一旦我们将这两个元素分开，我们的图表将如下所示：

![Design layers](https://fsharpforfunandprofit.com/posts/recipe-part3/Recipe_DesignLayers2.png)

不过请注意，我们可能有一些反向引用（如红色箭头所示）。例如，域层中的函数可能依赖于持久性相关的类型，如 `IRepository`。

在面向对象的设计中，我们会添加更多的层（例如应用程序服务）来处理这个问题。但在功能设计中，我们不需要这样做——我们可以将与持久性相关的类型移动到层次结构中域函数下方的不同位置，如下所示：

![Design layers](https://fsharpforfunandprofit.com/posts/recipe-part3/Recipe_DesignLayers2a.png)

在这个设计中，我们现在消除了层之间的所有循环引用。所有的箭头都指向下方。

这不需要创建任何额外的层或开销。

最后，我们可以通过将其倒置来将这种分层设计转换为 F# 文件。

- 项目中的第一个文件应包含没有依赖关系的代码。这表示层图底部的功能。它通常是一组类型，如基础设施或域类型。
- 下一个文件仅依赖于第一个文件。它将代表下一层的功能。
- 以此类推。每个文件只依赖于前一个文件。

因此，如果我们回顾第 1 部分中讨论的用例示例：

![Recipe Happy Path](https://fsharpforfunandprofit.com/posts/recipe-part1/Recipe_HappyPath.png)

那么 F# 项目中的相应代码可能看起来像这样：

![Design layers](https://fsharpforfunandprofit.com/posts/recipe-part3/Recipe_DesignLayers_CodeLayout.png)

列表的最底部是主文件，称为“main”或“program”，其中包含程序的入口点。

上面是应用程序中用例的代码。此文件中的代码将所有其他模块的所有函数“粘合在一起”，形成一个表示特定用例或服务请求的单一函数。（在面向对象设计中，与之最接近的等价物是“应用程序服务”，它们的作用大致相同。）

然后就在上面是“UI 层”，然后是“DB 层”，以此类推，直到你到达顶部。

这种方法的好处是，如果你是代码库的新手，你总是知道从哪里开始。前几个文件将始终是应用程序的“底层”，后几个文件将永远是“顶层”。

## 将代码放在模块中，而不是类中

F# 新手的一个常见问题是“如果我不使用类，我应该如何组织我的代码？”

答案是：模块。如你所知，在面向对象的程序中，数据结构和作用于它的函数将组合在一个类中。然而，在函数式 F# 中，数据结构和作用于它的函数包含在模块中。

将类型和功能混合在一起有三种常见模式：

- 具有与函数在同一模块中声明的类型。
- 具有与函数分开声明但在同一文件中的类型。
- 将类型与函数分开声明，并保存在不同的文件中，通常只包含类型定义。

在第一种方法中，类型及其相关函数在模块内定义。如果只有一个主类型，通常会给它一个简单的名称，如“T”或模块的名称。

这里有一个例子：

```F#
namespace Example

// declare a module
module Person =

    type T = {First:string; Last:string}

    // constructor
    let create first last =
        {First=first; Last=last}

    // method that works on the type
    let fullName {First=first; Last=last} =
        first + " " + last
```

因此，这些函数的名称是 `Person.create` 和 `Person.fullName`，而类型本身的名称是 `Person.T`

在第二种方法中，类型在同一个文件中声明，但在任何模块之外：

```F#
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
```

在这种情况下，使用相同的名称（`Person.create` 和 `Person.fullName`）访问函数，而使用 `PersonType` 等名称访问类型本身。

最后，这是第三种方法。该类型在一个特殊的“仅类型”模块中声明（通常在不同的文件中）：

```F#
// =========================
// File: DomainTypes.fs
// =========================
namespace Example

// "types-only" module
[<AutoOpen>]
module DomainTypes =

    type Person = {First:string; Last:string}

    type OtherDomainType = ...

    type ThirdDomainType = ...

```

在这种特殊情况下，`AutoOpen` 特性已用于使此模块中的类型自动对项目中的所有其他模块可见，使其“全局”。

然后，一个不同的模块包含所有处理 `Person` 类型的函数。

```F#
// =========================
// File: Person.fs
// =========================
namespace Example

// declare a module for functions that work on the type
module Person =

    // constructor
    let create first last =
        {First=first; Last=last}

    // method that works on the type
    let fullName {First=first; Last=last} =
        first + " " + last
```

请注意，在这个例子中，类型和模块都被称为 `Person`。这在实践中通常不是问题，因为编译器通常可以弄清楚你想要什么。

所以，如果你写这个：

```F#
let f (p:Person) = p.First
```

然后编译器将理解您所指的是 `Person` 类型。

另一方面，如果你写下：

```F#
let g () = Person.create "Alice" "Smith"
```

然后编译器将理解您所指的是 `Person` 模块。

有关模块的更多信息，请参阅关于组织功能的帖子。

## 模块的组织

对于我们的食谱，我们将使用多种方法，并遵循以下指导方针：

### 模块指南

*如果一个类型在多个模块之间共享，那么把它放在一个特殊的纯类型模块中。*

- 例如，如果一个类型是全局使用的（或者确切地说，在 DDD 术语中的“有界领域”内），我会把它放在一个名为 `DomainTypes` 或 `DomainModel` 的模块中，该模块在编译顺序的早期出现。
- 如果一个类型仅在子系统中使用，例如由多个 UI 模块共享的类型，那么我会把它放在一个名为 `UITypes` 的模块中，该模块在编译顺序上会排在其他 UI 模块之前。

*如果一个类型是一个（或两个）模块的私有类型，则将其与其相关函数放在同一个模块中。*

- 例如，仅用于验证的类型将被放入 `Validation` 模块中。仅用于数据库访问的类型将放在 `Database` 模块中，依此类推。

当然，组织类型的方法有很多，但这些指导方针是一个很好的默认起点。

### 伙计，文件夹呢？

F# 工具（如 Visual Studio 中的工具）支持文件夹，用于处理更复杂的项目。然而，如果你来自 C#，它们并不是你所期望的那种文件夹。

F# 工具中的文件夹必须符合文件排序语义，就像文件一样。对于某些项目，自上而下的排序并不总是像“每个文件都依赖于它上面的文件”这样简单。实际上，可能存在位于依赖顺序的同一“级别”的文件组，这些文件可能与整体功能有关。这就是文件夹发挥作用的地方。

一个具体的例子是 Visual Studio 本身的 F# 工具。在这个项目中，尽管有适量的文件，但自上而下的依赖堆栈实际上相当小。“完成”文件夹包含多个关于在 Visual Studio 中提供完成（IntelliSense）的文件。Visual Studio 中有多种完成方式，其中一些逻辑可以共享，但这种共享逻辑不适用于其他任何东西。

要了解更多关于使用 F# 排序及其对代码库的影响的信息，请参阅关于循环和模块化的文章。

### 求助，我的类型之间相互依赖

如果你来自面向对象设计，你可能会遇到类型之间的相互依赖关系，比如这个例子，它不会编译：

```F#
type Location = {name: string; workers: Employee list}

type Employee = {name: string; worksAt: Location}
```

如何修复此问题以使 F# 编译器满意？

这并不难，但确实需要更多的解释，所以我又专门写了一整篇文章来处理循环依赖关系。

## 示例代码

让我们重新审视到目前为止的代码，但这次是按模块组织的。

下面的每个模块通常会成为一个单独的文件。

请注意，这仍然是一个骨架。部分模块缺失，部分模块几乎为空。

对于一个小项目来说，这种组织方式可能有点过头了，但还会有更多的代码出现！

```F#
/// ===========================================
/// Common types and functions shared across multiple projects
/// ===========================================
module CommonLibrary =

    // the two-track type
    type Result<'TSuccess,'TFailure> =
        | Success of 'TSuccess
        | Failure of 'TFailure

    // convert a single value into a two-track result
    let succeed x =
        Success x

    // convert a single value into a two-track result
    let fail x =
        Failure x

    // apply either a success function or failure function
    let either successFunc failureFunc twoTrackInput =
        match twoTrackInput with
        | Success s -> successFunc s
        | Failure f -> failureFunc f


    // convert a switch function into a two-track function
    let bind f =
        either f fail

    // pipe a two-track value into a switch function
    let (>>=) x f =
        bind f x

    // compose two switches into another switch
    let (>=>) s1 s2 =
        s1 >> bind s2

    // convert a one-track function into a switch
    let switch f =
        f >> succeed

    // convert a one-track function into a two-track function
    let map f =
        either (f >> succeed) fail

    // convert a dead-end function into a one-track function
    let tee f x =
        f x; x

    // convert a one-track function into a switch with exception handling
    let tryCatch f exnHandler x =
        try
            f x |> succeed
        with
        | ex -> exnHandler ex |> fail

    // convert two one-track functions into a two-track function
    let doubleMap successFunc failureFunc =
        either (successFunc >> succeed) (failureFunc >> fail)

    // add two switches in parallel
    let plus addSuccess addFailure switch1 switch2 x =
        match (switch1 x),(switch2 x) with
        | Success s1,Success s2 -> Success (addSuccess s1 s2)
        | Failure f1,Success _  -> Failure f1
        | Success _ ,Failure f2 -> Failure f2
        | Failure f1,Failure f2 -> Failure (addFailure f1 f2)


/// ===========================================
/// Global types for this project
/// ===========================================
module DomainTypes =

    open CommonLibrary

    /// The DTO for the request
    type Request = {name:string; email:string}

    // Many more types coming soon!

/// ===========================================
/// Logging functions
/// ===========================================
module Logger =

    open CommonLibrary
    open DomainTypes

    let log twoTrackInput =
        let success x = printfn "DEBUG. Success so far: %A" x; x
        let failure x = printfn "ERROR. %A" x; x
        doubleMap success failure twoTrackInput

/// ===========================================
/// Validation functions
/// ===========================================
module Validation =

    open CommonLibrary
    open DomainTypes

    let validate1 input =
       if input.name = "" then Failure "Name must not be blank"
       else Success input

    let validate2 input =
       if input.name.Length > 50 then Failure "Name must not be longer than 50 chars"
       else Success input

    let validate3 input =
       if input.email = "" then Failure "Email must not be blank"
       else Success input

    // create a "plus" function for validation functions
    let (&&&) v1 v2 =
        let addSuccess r1 r2 = r1 // return first
        let addFailure s1 s2 = s1 + "; " + s2  // concat
        plus addSuccess addFailure v1 v2

    let combinedValidation =
        validate1
        &&& validate2
        &&& validate3

    let canonicalizeEmail input =
       { input with email = input.email.Trim().ToLower() }

/// ===========================================
/// Database functions
/// ===========================================
module CustomerRepository =

    open CommonLibrary
    open DomainTypes

    let updateDatabase input =
       ()   // dummy dead-end function for now

    // new function to handle exceptions
    let updateDatebaseStep =
        tryCatch (tee updateDatabase) (fun ex -> ex.Message)

/// ===========================================
/// All the use cases or services in one place
/// ===========================================
module UseCases =

    open CommonLibrary
    open DomainTypes

    let handleUpdateRequest =
        Validation.combinedValidation
        >> map Validation.canonicalizeEmail
        >> bind CustomerRepository.updateDatebaseStep
        >> Logger.log

```

## 摘要

在这篇文章中，我们研究了如何将代码组织成模块。在本系列的下一篇文章中，我们终于开始做一些真正的编码了！

同时，您可以在后续帖子中阅读更多关于循环依赖关系的内容：

- 循环依赖是邪恶的。
- 重构以删除循环依赖关系。
- 循环和模块化，比较了 C# 和 F# 项目的一些现实指标。