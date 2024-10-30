# FSharpPlus 官方文档翻译

https://github.com/fsprojects/FSharpPlus/



# README.md

https://github.com/fsprojects/FSharpPlus/blob/master/README.md

[![Download](https://camo.githubusercontent.com/0049c11e623d52d2cf56c5ae684c35ea90aa754aa1aaf6c23580377397bcb35e/68747470733a2f2f696d672e736869656c64732e696f2f6e756765742f64742f465368617270506c75732e737667)](https://www.nuget.org/packages/FSharpPlus) [![NuGet](https://camo.githubusercontent.com/6cf0bc63c1dd61b0e69937c3c1b456d9d8f45f79672382bd49f3078fb4071172/68747470733a2f2f696d672e736869656c64732e696f2f6e756765742f762f465368617270506c75732e737667)](https://www.nuget.org/packages/FSharpPlus) [![NuGet Preview](https://camo.githubusercontent.com/88925337ef9269c7db7cc6fa428bfe62cc20b0fdfacadb51277fb94dfb611c56/68747470733a2f2f696d672e736869656c64732e696f2f6e756765742f767072652f465368617270506c75732e7376673f6c6162656c3d707265)](https://www.nuget.org/packages/FSharpPlus/absoluteLatest) [![Made with F#](https://camo.githubusercontent.com/f10d0dfcc7498451ee017015107170fcdbc6b8e6086f666a52f93e47b214595e/68747470733a2f2f696d672e736869656c64732e696f2f6769746875622f6c616e6775616765732f746f702f667370726f6a656374732f465368617270506c75733f636f6c6f723d253233623834356663)](https://camo.githubusercontent.com/f10d0dfcc7498451ee017015107170fcdbc6b8e6086f666a52f93e47b214595e/68747470733a2f2f696d672e736869656c64732e696f2f6769746875622f6c616e6775616765732f746f702f667370726f6a656374732f465368617270506c75733f636f6c6f723d253233623834356663) [![License - Apache 2.0](https://camo.githubusercontent.com/e2cf5095744ead4f58df2a554b6eee19f9e8bf718acec37a475af03630b3aac5/68747470733a2f2f696d672e736869656c64732e696f2f6769746875622f6c6963656e73652f667370726f6a656374732f465368617270506c75733f636f6c6f723d253233464639353744)](https://camo.githubusercontent.com/e2cf5095744ead4f58df2a554b6eee19f9e8bf718acec37a475af03630b3aac5/68747470733a2f2f696d672e736869656c64732e696f2f6769746875622f6c6963656e73652f667370726f6a656374732f465368617270506c75733f636f6c6f723d253233464639353744)

F#+ 是一个基础库，旨在将 F# 提升到函数式编程的下一个层次。

如果我们把 F# 想象得比实际更多呢？

F#+ 基于 FSharp 构建，使用通用编程技术来帮助您避免样板代码。然而，通过命名约定和签名，可以看出它尽可能地“增强”而不是“替换”现有模式。

入门很容易，因为你可以从享受一些扩展和通用函数开始，但你会发现F#+的其他部分在你之前展开，并且随着你的深入而变得有用。

有关更多详细信息，请参阅[文档](https://fsprojects.github.io/FSharpPlus)。

## 寻求帮助

我们很乐意帮助您解决任何问题，包括完全初学者！

请加入我们聊天：

- Gitter  [![Join the chat at https://gitter.im/fsprojects/FSharpPlus](https://camo.githubusercontent.com/702edeabc737e5b1a9d3a6783fb65e02e6f3fb083dda1b3088ccc08c3ca90bf7/68747470733a2f2f6261646765732e6769747465722e696d2f667370726f6a656374732f465368617270506c75732e737667)](https://gitter.im/fsprojects/FSharpPlus?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
- 你可以被邀请加入[函数式编程 Slack](https://app.slack.com/client/T0432GV8P/CTT70ER47)，然后加入 [#FSharpPlus](https://functionalprogramming.slack.com/join/shared_invite/zt-svowkzcg-6xzAuVrUtINX7swWuhjHUw#/shared-invite/email)

…或者你可以用标签 `F#+` [在 stack overflow 问一个问题](https://stackoverflow.com/questions/ask?tags=f%23%2b)

### 贡献

该项目托管在 GitHub 上，您可以在那里报告问题、分叉项目和提交 pull 请求。欢迎打开问题进行讨论或提问，请随时填写[新问题](https://github.com/fsprojects/FSharpPlus/blob/master/issues/new)表格！

- [《开发人员指南》](https://github.com/fsprojects/FSharpPlus/blob/master/DEVELOPER_GUIDE.md)详细介绍了实现习惯用法，即类型类概念在实现中的转换方式。
- [《设计指南》](https://github.com/fsprojects/FSharpPlus/blob/master/DESIGN_GUIDELINES.md)详细介绍了在命名方面指导实现的设计选择，以及影响运行时和编译时性能的选择。



# DEVELOPER_GUIDE.md

https://github.com/fsprojects/FSharpPlus/blob/master/DEVELOPER_GUIDE.md

## 总体目标/理念：

- 一致性：这个库的设计应该最大限度地提高一致性，这是一组函数、参数类型、类型和它们所代表的抽象之间的关系。通过这样做，用户可以更容易地猜测期望是什么，而不是一直转发来检查文档和/或源代码。因此，为了实现这一目标，应该仔细考虑命名。
- 非侵入性：此库的所有添加都不应改变 F# 语言和 FSharp.Core 的现有功能。在需要的情况下，必须在强制 `open` 的特定模块/命名空间中完成。
- 非固执己见（opinionated）：F#+ 尽可能不强迫用户以特定的方式解决问题，而是试图从不同的角度提供通用的方法来解决问题。使用此库的具体方法最终应由使用 F#+ 以特定方式解决问题并实现该目标的组织/团队/用户定义。为此，可以在 F#+ 之上添加一小部分函数和类型别名，但库必须尽可能中立。
- 信任用户：F#+ 不应用“删除可能以错误方式使用的功能”的原则，它假设用户有足够的技能来决定特定功能的好用法和坏用法。F#+ 维护者和贡献者的目的不是通过隐藏东西来教育用户，这不是这个库的目标，尽管有很多东西需要学习，但为此有一般的指导方针，通常每个组织/团队/用户也应该有具体的指导方针。这并不意味着文档维护不善。
- 编码风格：没有一套特定的规则，但它应该遵守一般的编码惯例，并有所放松，以便能够在方便的地方对齐代码。例如，当有大量重载时，这使得阅读更容易，但也更容易使用多行编辑器支持编辑代码。最好的建议是尽量复制现有的风格。对于签名，请尝试遵循 [FSharp.Core 设计](https://learn.microsoft.com/en-us/dotnet/fsharp/style-guide/)，即 `'T` 而不是 `'a`。
- 命名 commits：在祈使语气中使用一个非常简短的描述性句子，就像完成句子“如果应用，此 commit 将 …”一样。有关更多详细信息，请[用这个](https://cbea.ms/git-commit)作为参考。
- PR：尽量保持它们的原子性，如果发现 PR 涉及许多不相关的领域，它将被要求拆分为不同的 PR。一个典型的例子是添加了特定的功能，但也修复了文档中的错误或拼写错误的 PR。如果我们最终恢复了该功能，其他更改也将恢复，这可能是不希望的。鼓励起草 PR，并在写作过程中随时寻求建议。使用与提交相同的命名约定。
- 这是 F#：尽管 F#+ 包含一些灵感来自其他语言中使用的库的抽象概念，但这些概念被翻译成 F# 标准，并最终适应 F#。

## 扩展

此库为不同类型定义了许多扩展。其中一些函数在概念上是相互关联的，因为它们代表了一个抽象，有时它们确实有一个函数的泛型版本，不需要将类型作为模块前缀键入。请注意，在这些情况下，通常名称是相同的，但这不是规则，有时名称会不同，以避免与其他函数冲突或混淆，例如：

- `map` 是一个泛型函数，F# 有许多类型实现它，但对于字典来说，这对应于 `mapValues`
- `min` 是一个在集合中运行的非泛型函数，但它的泛型对应函数是 `minimum`，以避免与内置的 `min` 函数冲突（两个值之间的最小值）。

因此，这意味着 F# 不提供基于名称的泛型函数，尽管在许多情况下名称是相同的，但我们需要考虑：

1. F# core 有一些不一致之处，因为它不是一个基于类型类的库。基于类型类的库不一定是实现类型类技巧的东西，而是被设计成好像我们支持它们一样的东西，这意味着捕获一些泛型概念，并在为每个函数选择的名称中明确它们。
2. 但是 F#+ 并没有试图修复 F# 核心，而是以非侵入性的方式构建，并尽可能多地重用 F# 核心中所有一致的概念、习惯用法和事实上的命名约定，而不会增加已经存在的不一致性。请记住，在 F#+ 中，**我们试图扩展 FSharp.Core 功能，但如前所述，我们尽量不干涉**：F#+ 基于 FSharp 构建，使用泛型编程技术来帮助避免样板代码。然而，通过命名约定和签名，可以看出它尽可能地“增强”而不是“替换”现有模式。

因此，解决方案有时需要一些创造力，比如想出新的名字，在不偏离现有命名约定的情况下明确函数的作用。

另一个有趣的例子是与 `zip` 相关的函数：

- 对于像类型这样的集合，在 F# 核心中，找到成对作用的 `zip` / `map2` 函数是很正常的。但另一种可能的实现是跨产品工作的应用子 zip。
- 在这里，我们定义了 2 个泛型函数 `lift2`，它总是对应于应用子实例，因此通常在非集合中，它将对应于非泛型 `map2`，但由于集合已经（或至少预期）有一个成对作用的 `map2`，在这些情况下，只有 F#+ 提供了 `lift2` 扩展，这就是用于其泛型对应物的扩展。
- 另一个提供的泛型函数是这样一个 `zip`：主要适用于类集合类型，尽管它们与非泛型名称匹配，但请注意，行为并不完全相同，因为当元素数量不同时，F# 核心会为列表和数组抛出错误。从这个意义上说，事实上，它在定义时匹配 `.zipShortest`，否则它匹配 `.zip`。

因此，这些与 zip 相关的函数是 F# 如何最好地修复 FSharp 的一个很好的例子。以非侵入性的方式解决核心不一致问题。

## 抽象

此库的 Control 命名空间中表示的抽象是一组实际上表示泛型函数的类型，这些类型称为 Invokables。

这些调用的组织方式类似于其他语言中的 `static interfaces`, `type-classes`, `concepts` 或 `traits`，它们具有类型系统构造，可以实现更高的种类和 ad-hoc 多态性。

在 F#+ 中，由于目标 CIL 的约束，CIL 没有对更高种类的第一类支持，我们不能强制特定的类型参数属于特定的抽象。这样，在形成此库实现习惯用法的各种类型签名中，泛型类型参数通常被赋予抽象的名称。举个例子，泛型 `map` 的签名写为 `('T -'U) -> '``Functor<'T>``-> '``Functor<'U>`` ` ，它将被呈现为 `('T -'U) -> 'Functor<'T> -> 'Functor<'U>`，其中 `Functor` 是抽象的名称，所有与 Functor 对齐的类型都可以去那里。

因此，调用对实现没有任何影响，因为它们只是泛型类型参数的任意标签，对于向库的开发人员和用户传达意图非常有用。尽管F#类型系统通常会在方法级别显示一些约束，但这些约束有时对传达相关抽象没有多大帮助。

另一方面，这些抽象的具体实现至少是通过为每个函数定义一个具体类型（一个可调用的（Invokable））来完成的。

例如，最简单的抽象之一 `Monoid` 的核心实现分为 `Plus` 和 `Zero` 两种类型。

这些调用器包含几种静态方法：

- 具体重载，例如 Monoid 的 `Plus` 将对每一个应该支持 `Monoid` 类型类的 BCL 和 FSharp.Core 类型中都有 `Plus` 方法的重载（命名与定义这些方法的类型相同）。例如，一个用于连接两个字符串，一个用来连接两个列表，一个来连接两个数组，以此类推。
- 一个*可调用的（invokable）*方法，应始终定义为 `inline`，它嵌入了用于选择正确重载的 SRTP 调度程序机制（无论是在 F#+ 中定义还是在其外部定义）
- 一个默认或一组默认实现，由于 F# 编译器实现的细节，可能需要在内部类型扩展中定义这些实现，以便在这种抽象带有以组合多个函数表示的默认实现时，能够正确地提取最终支持抽象的类型，这些函数总是被定义为 `inline`。

### 具体实现

F#+ 应该为基元类型（来自 F#+ 依赖关系的类型，主要是 BCL 类型和 FSharp.Core 类型）添加尽可能多的具体实现，因为最终用户以后将无法添加它们。

### 默认值

默认实现的目标是允许库的用户编写更少的代码，例如 F#+ 希望用户添加 `Bind`、`Return` 到他们特定的 monad 类型，但不强制他们添加 `Join`，尽管如果他们这样做，代码可能会更高效。默认值不适合 F#+ 的开发人员使用，因为 F#+ 应该能够编写更多的代码，以最大限度地提高库的可用性。您可以将其解读为“原则是允许用户编写更少的样板，因此我们必须在 F#+ 中添加一些样板”。

### 调用器

理想情况下，调用器的签名应该与泛型函数相同。这将允许在传递类型比传递函数导致更通用代码的情况下使用调用器来代替函数。

## 发布

请参阅 [BRANCHES](https://github.com/fsprojects/FSharpPlus/blob/master/BRANCHES.md) 中的文档。



# DESIGN_GUIDELINES.md

https://github.com/fsprojects/FSharpPlus/blob/master/DESIGN_GUIDELINES.md

F#+ 是一个用于生产用途的 F# 库，因此该库的设计应按照以下准则进行调整：

## 添加新功能前的注意事项

- 它属于这个 F#+ 还是一个独立的库，是否依赖于 F#+？最有可能的是，如果它是特定于技术的东西，它就不会在这里占有一席之地，而如果它与语言扩展有关，这可能是一个好地方。有些情况会落到在中间。解析就是一个例子，目前我们认为 Json 或其他格式解析不是这个库的一部分，但我们正在处理 F# 值解析。
- 我们应该在这个库中添加实验性的、奇特的代码吗？这是一个狡猾的问题。一方面，这个库针对的是生产代码，但也应该考虑到，我们依赖于用户了解他们正在投入生产的内容。在这里，性能、可读性和可扩展性等代码质量并不是通过隐藏函数来强制实现的。这与“生产代码”的概念在不同场景中有点模糊的事实相结合，例如，在某些生产代码场景中，性能不是目标，也不是可扩展性。

## 命名

- 一般 F# 准则和命名约定适用于此。
- 对于新函数或运算符，请尝试首先匹配 F# 中现有的事实约定（即：`foldBack` 而不是 `foldRight`）。
- 如果无法将该名称与现有的 F# 函数名称相关联，我们可以参考其他语言，最好是 FP 优先语言。
- 泛型函数应与特定于模块的函数命名相同，即：`map`，而不是 `fmap`。

## 新增函数

- 与 F# 核心相反，我们不应用通过隐藏东西来鼓励良好实践的原则，即同时是邪恶的有用函数，即 `curry`。最终用户对函数的良好使用负责。
- 理想情况下，泛型函数应该有一个相应的非泛型对应项，即 `map3` ==> `Option.map3` - `Result.map3`。
- 理想情况下，泛型函数应该与规则相关，而不一定是范畴论。
- 记住，在不同类型中以一致的方式设计相关函数，即使这可能会导致函数效率不高。性能是首要任务，但我们不会通过隐藏东西来保证性能。
- 函数应该尽可能透明，也就是说，文化之类的东西应该是中立的或明确的。

## 模块

- 通用运算符和函数应该进入 `FSharpPlus.Operators` 模块，除非它们彼此冲突或与现有的 F# 功能冲突，在这种情况下，它们应该进入需要显式打开的模块。
- 新的类型/集合应该添加在一个单独的文件中，最好有一个带有 let 绑定函数的模块，因为它通常更符合 F# 习惯。当类型实现抽象时，应该添加一些静态成员，以满足泛型抽象的静态约束，但如果它公开了一个具有相同功能的 let 绑定函数，则可以使用属性将静态成员隐藏在工具中。
- 一元/二元运算符：向自动打开的 FSharpPlus 添加运算符。由于运算符数量有限，并且与另一个库的运算符冲突的机会越来越大，因此应仔细考虑并证明运算符模块的合理性。向手动打开的模块添加运算符是另一种选择。

## 静态成员重载

- 默认重载（回退机制）对库的最终用户来说是并且应该是可用的，但在内部我们应该避免依赖它们。与依赖默认实现相比，最好复制代码或最终排除一些部分。这提供了不太复杂的类型推理（对代码的更多控制）和更快的编译时间。同时，只要我们不忘记任何重载（这将被视为一个错误），这个决定就不会影响最终用户，在大多数情况下，它通过具有专门的功能提供了提高性能的额外好处。
- 使用接口时必须小心，在某些情况下，需要为显式接口提供重载（即：`seq<_>`），而不是为实现该接口的隐式类型提供重载，否则将成为默认重载。例如，一个显式的 `seq<_>` 实例有一个重载 `>>=`，但实现 `seq<_>` 的所有类型都默认为该重载是不好的（并非所有 `IEnumerables<_>` 都是 monad），而对于像 `skip` 这样的方法，将 `seq<_>` 作为默认值是可以的。
- 新的抽象应该考虑使用众所周知的运算符作为静态成员，特别是如果它存在一个通用的全局运算符。例如，`>>=` 作为全局运算符存在，所以最好使用它——也作为一元绑定操作的约定，需要一个 `>>=`（而不是一个命名的 `bind`）静态成员。这有两个优点：它允许在没有此库的情况下将类型上的运算符用作非泛型类型，并且还增加了找到已定义该运算符的第三方类型的机会。



# FSharpPlus

https://fsprojects.github.io/FSharpPlus/

F#+ 是一个基础库，旨在将 F# 提升到函数式编程的下一个层次。

*如果我们把 F# 想象得比实际更多呢？*

F#+ 基于 FSharp 构建，使用泛型编程技术来帮助您避免样板代码。然而，通过命名约定和签名，可以看出它尽可能地“增强”而不是“替换”现有模式。

新增内容可以概括为：

- 核心类型的[扩展](https://fsprojects.github.io/FSharpPlus/extensions.html)，如 [String.toLower](https://fsprojects.github.io/FSharpPlus/reference/fsharpplus-string.html)
- [泛型函数和运算符](https://fsprojects.github.io/FSharpPlus/generic-doc.html)，如 `map`，可以扩展以支持其他类型
- 泛型和可定制的[计算表达式](https://fsprojects.github.io/FSharpPlus/computation-expressions.html)，如 `monad`
- 泛型[数学模块](https://fsprojects.github.io/FSharpPlus/numerics.html)
- 捕获常见 FP 模式的[抽象](https://fsprojects.github.io/FSharpPlus/abstractions.html)，如标准单子 Cont、Reader、Writer、State 及其 Monad Transformers
- 一些与抽象配合良好的新类型，如 NonEmptyList、DList 和 Validation
- 一个多态的 [Lenses/Optics](https://fsprojects.github.io/FSharpPlus/tutorial.html#Lens)，可以轻松读取和更新不可变数据的一部分
- 帮助您进行[解析](https://fsprojects.github.io/FSharpPlus/parsing.html)的通用方法

然而，请注意，F#+ 不会为特定技术解决特定问题，例如 JSON 解析。

有些函数可以作为[扩展方法](https://fsprojects.github.io/FSharpPlus/extension-methods.html)使用，因此可以从 C# 调用。请注意，这并不完整，或者目前被视为高优先级。

入门很容易，因为你可以从享受一些扩展和泛型函数开始，但你会发现 F#+ 的其他部分在你之前展开，并且随着你的深入而变得有用。

## 示例 1

此示例演示了如何使用此库中定义的扩展函数。

```F#
#r @"nuget: FSharpPlus"

open FSharpPlus

let x = String.replace "old" "new" "Good old days"
// val x : string = "Good new days"
```

## 示例 2

此示例演示了如何使用此库中定义的泛型函数。

```F#
map string [|2;3;4;5|]
// val it : string [] = [|"2"; "3"; "4"; "5"|]

map ((+) 9) (Some 3)
// val it : int option = Some 12

open FSharpPlus.Data

map string (NonEmptyList.create 2 [3;4;5])
// val it : NonEmptyList<string> = {Head = "2"; Tail = ["3"; "4"; "5"];}
```

为了更深入地实践 F#+，我们建议遵循教程：

- [教程](https://fsprojects.github.io/FSharpPlus/tutorial.html)包含对此库的进一步解释。

## 参考文档

- [类型](https://fsprojects.github.io/FSharpPlus/types.html)包含有关此库中提供的所有类型的详细信息。
- [API 参考](https://fsprojects.github.io/FSharpPlus/reference/index.html)包含库中所有类型、模块和函数的自动生成文档。这包括使用大多数函数的其他简短示例。

## 样品

此文档是从[内容文件夹](https://github.com/fsprojects/FSharpPlus/tree/master/docsrc/content)中的 `*.fsx` 文件自动生成的。克隆本地副本以供查看可能很有用。

[API 引用](https://fsprojects.github.io/FSharpPlus/reference/index.html)是从库实现中的 Markdown 注释自动生成的。

同样值得注意的是 [Sample 文件夹](https://github.com/fsprojects/FSharpPlus/tree/master/src/FSharpPlus.Docs/Samples)，其中包含显示如何在代码中使用 F#+ 的示例脚本。

## 贡献和版权

该项目托管在 [GitHub](https://github.com/fsprojects/FSharpPlus) 上，您可以在那里[报告问题](https://github.com/fsprojects/FSharpPlus/issues)、分叉项目和提交 pull 请求。

如果您正在添加一个新的公共 API，也请考虑添加[文档](https://github.com/fsprojects/FSharpPlus/tree/master/docsrc/content)。您可能还想阅读[库设计说明](https://github.com/fsprojects/FSharpPlus/blob/master/DESIGN_GUIDELINES.md)以了解其工作原理。

该库在 Apache 许可证 2.0 版下可用，该许可证允许出于商业和非商业目的进行修改和重新分发。有关更多信息，请参阅 GitHub 存储库中的 [License 文件](https://github.com/fsprojects/FSharpPlus/blob/master/LICENSE.txt)。



# 教程 - 介绍 FSharpPlus

https://fsprojects.github.io/FSharpPlus/tutorial.html

- 从 Nuget 下载二进制文件，使用最新的 CI 版本。
- 打开 F# 脚本文件或 F# 交互式文件，引用库并打开命名空间

```F#
#r @"nuget: FSharpPlus"
```

现在，我们将快速概述 F#+ 中提供的功能。

## 泛型函数

打开 FSharpPlus 命名空间时，它们会自动可用

这里有一个 `map` 示例（Haskellers 的 [fmap](https://wiki.haskell.org/Functor)，C-sharpers 的 [Select](http://www.dotnetperls.com/select)）：

```F#
  open FSharpPlus

  map string [|2;3;4;5|]
  // val it : string [] = [|"2"; "3"; "4"; "5"|]

  map ((+) 9) (Some 3)
  // val it : int option = Some 12

  open FSharpPlus.Data

  map string (NonEmptyList.create 2 [3;4;5])
  // val it : NonEmptyList<string> = {Head = "2"; Tail = ["3"; "4"; "5"];}
```

只要它们包含具有预期签名的适当方法，它们也可用于您自己的类型

```F#
type Tree<'t> =
      | Tree of 't * Tree<'t> * Tree<'t>
      | Leaf of 't
      static member Map (x:Tree<'a>, f) = 
          let rec loop f = function
              | Leaf x -> Leaf (f x)
              | Tree (x, t1, t2) -> Tree (f x, loop f t1, loop f t2)
          loop f x
  
  map ((*) 10) (Tree(6, Tree(2, Leaf 1, Leaf 3), Leaf 9))
  // val it : Tree<int> = Tree (60,Tree (20,Leaf 10,Leaf 30),Leaf 90)
```

泛型函数在 F# 中可能被视为一种奇特的东西，它只节省了一些按键（`map` 而不是 `List.map` 或 `Array.map`），但它们仍然允许您使用 ad-hoc 多态性达到更高的抽象级别。

但更有趣的是运算符的使用。你不能在它们前面加上它们所属的模块，嗯你可以但它不再是运算符。例如，许多 F# 库定义了绑定运算符 `(>>=)`，但它不是泛型的，所以如果你使用两种不同的类型，这两种类型都是 monad，你需要给它加前缀，例如 `State.(>>=)` 和 `Reader.(>>=)` 这违背了拥有运算符的目的。

这里有一个现成的泛型绑定运算符：`>>=`

```F#
  let x = ["hello";" ";"world"] >>= (fun x -> Seq.toList x)
  // val x : char list = ['h'; 'e'; 'l'; 'l'; 'o'; ' '; 'w'; 'o'; 'r'; 'l'; 'd']


  let tryParseInt : string -> int option = tryParse
  let tryDivide x n = if n = 0 then None else Some (x / n)

  let y = Some "20" >>= tryParseInt >>= tryDivide 100
  // val y : int option = Some 5
```

您还有 Kleisli 组合（鱼）运算符：`>=>`

在[面向铁路的编程](https://www.google.ch/#q=railway+oriented+programming)教程系列之后，它在 F# 中变得越来越流行

```F#
  let parseAndDivide100By = tryParseInt >=> tryDivide 100

  let parsedAndDivide100By20 = parseAndDivide100By "20"   // Some 5
  let parsedAndDivide100By0' = parseAndDivide100By "zero" // None
  let parsedAndDivide100By0  = parseAndDivide100By "0"    // None

  let parseElement n = List.tryItem n >=> tryParseInt
  let parsedElement  = parseElement 2 ["0"; "1";"2"]
```

但别忘了上面使用的运算符是泛型的，所以我们可以更改函数的类型，并免费获得不同的功能：

测试代码保持不变，但我们得到了一个更有趣的功能

```F#
  let parseAndDivide100By = tryParseInt >=> tryDivide 100

  let parsedAndDivide100By20 = parseAndDivide100By "20"   // Choice1Of2 5
  let parsedAndDivide100By0' = parseAndDivide100By "zero" // Choice2Of2 "Failed to parse zero"
  let parsedAndDivide100By0  = parseAndDivide100By "0"    // Choice2Of2 "Can't divide by zero"
```

同样，在处理组合子时，泛型应用函子（空间入侵者）运算符非常方便：`<*>`

```F#
  let sumAllOptions = Some (+) <*> Some 2 <*> Some 10     // val sumAllOptions : int option = Some 12

  let sumAllElemets = [(+)] <*> [10; 100] <*> [1; 2; 3]   // int list = [11; 12; 13; 101; 102; 103]
```

有关更多详细信息和功能，请参阅[泛型运算符和函数](https://fsprojects.github.io/FSharpPlus/generic-doc.html)

以下是所有[泛型运算符和函数](https://fsprojects.github.io/FSharpPlus/reference/fsharpplus-operators.html)

以下是对 Functor、Applictive 和 Monad 抽象的[简短解释](https://fsprojects.github.io/FSharpPlus/applicative-functors.html)，并附有代码示例。

## 镜头（Lens）

从 https://github.com/ekmett/lens/wiki/Examples 而来

首先，打开 F#+ 镜头

```F#
 open FSharpPlus.Lens
```

现在，您可以从 lens 读取（`_2` 是元组第二个分量的 lens）

```F#
  let r1 = ("hello","world")^._2
  // val it : string = "world"
```

你可以对镜头写入。

```F#
  let r2 = setl _2 42 ("hello","world")
  // val it : string * int = ("hello", 42)
```

编写用于阅读（或写作）的镜头按照命令式程序员所期望的顺序进行，只使用 `(<<)`。

```F#
  let r3 = ("hello",("world","!!!"))^.(_2 << _1)
  // val it : string = "world"

  let r4 = setl (_2 << _1) 42 ("hello",("world","!!!"))
  // val it : string * (int * string) = ("hello", (42, "!!!"))
```

你可以用 `to'` 从纯函数中制作一个 Getter。

```F#
  let r5 = "hello"^.to' length
  // val it : int = 5
```

只需使用 `(<<)`，您就可以轻松地用镜头组成 Getter。不需要明确的强制。

```F#
  let r6 = ("hello",("world","!!!"))^. (_2 << _2 << to' length)
  // val it : int = 3
```

正如我们上面看到的，你可以写入镜头，这些写入可以改变容器的类型。`(.->)` 是 `set` 的中缀别名。

```F#
  let r7 = _1 .-> "hello" <| ((),"world")
  // val it : string * string = ("hello", "world")
```

它可以与 `(|>)` 结合使用，用于熟悉的冯·诺伊曼风格的赋值语法：

```F#
  let r8 = ((), "world") |> _1 .-> "hello"
  // val it : string * string = ("hello", "world")
```

相反，视图可以用作 `(^.)`的前缀别名。

```F#
  let r9 = view _2 (10,20)
  // val it : int = 20
```

更多详情：

这是[镜头和所有其他光学元件](https://fsprojects.github.io/FSharpPlus/lens.html)的完整导览

查看所有[镜头函数](https://fsprojects.github.io/FSharpPlus/reference/fsharpplus-lens.html)



# 函子和应用子

https://fsprojects.github.io/FSharpPlus/applicative-functors.html

```F#
#r @"nuget: FSharpPlus"
open FSharpPlus
```

您可以一步一步地运行此脚本。由于函数和运算符有重新定义，因此必须遵守执行顺序

## 函子（Functors）

直观的定义是，Functor 是你可以映射的东西。

因此，它们都有一个 `map` 操作，这是它们的最小定义。

大多数容器都是函子

```F#
let r01 = List.map   (fun x -> string (x + 10)) [ 1;2;3 ]
let r02 = Array.map  (fun x -> string (x + 10)) [|1;2;3|]
let r03 = Option.map (fun x -> string (x + 10)) (Some 5)
```

您可以将 Option functor 视为 List 的一个特例，它可以是空的，也可以只有一个元素。

我们本可以使用这个库中的泛型函数 `map`，它适用于任何 functor。

```F#
let r01' = map (fun x -> string (x + 10)) [ 1;2;3 ]
let r02' = map (fun x -> string (x + 10)) [|1;2;3|]
let r03' = map (fun x -> string (x + 10)) (Some 5)
```

现在，让我们定义一个简单的类型，并通过添加一个 `Map` 静态方法使其成为一个 functor

```F#
type Id<'t> = Id of 't with
    static member Map (Id x, f) = Id (f x)

let r04 = map (fun x -> string (x + 10)) (Id 5)
```

大多数计算也是函子

下面是一个 Async 函数的示例

```F#
let async5 = async.Return 5
let r05  = map (fun x -> string (x + 10)) async5
let r05' = Async.RunSynchronously r05
```

但即使是普通函数也是函子

```F#
let r06  = map (fun x -> string (x + 10)) ((+) 2)
let r06' = r06 3
```

对于函数 `map` 等价于 `(<<)`，这意味着映射到函数上与使用映射器组合函数相同

List functor 可以看作是一个函数，它接受一个整数索引来返回一个值： `f: Naturals -> 't` 。因此，你可以把List functor上的 `map` 看作是组成一个函数：

*

```F#
let listFunc = function 0 -> 1 | 1 -> 2 | 2 -> 3 // [1;2;3]
let r01'' = map (fun x -> string (x + 10)) listFunc
```

元组呢？*

```F#
module TupleFst = let map f (a,b) = (f a, b)
module TupleSnd = let map f (a,b) = (a, f b)

let r07 = TupleFst.map (fun x -> string (x + 10)) (5, "something else")
let r08 = TupleSnd.map (fun x -> string (x + 10)) ("something else", 5)
```

因此，用元组定义一个 functor 的方法不止一种。这同样适用于两种类型的可区分联合。

```F#
// DUs
module ChoiceFst = let map f = function Choice1Of2 x -> Choice1Of2 (f x) | Choice2Of2 x -> Choice2Of2 x
module ChoiceSnd = let map f = function Choice2Of2 x -> Choice2Of2 (f x) | Choice1Of2 x -> Choice1Of2 x

let choiceValue1:Choice<int,string> = Choice1Of2 5
let choiceValue2:Choice<int,string> = Choice2Of2 "Can't divide by zero."

let r09  = ChoiceFst.map (fun x -> string (x + 10)) choiceValue1
let r09' = ChoiceFst.map (fun x -> string (x + 10)) choiceValue2

let r10  = ChoiceSnd.map (fun x -> "The error was: " + x) choiceValue1
let r10' = ChoiceSnd.map (fun x -> "The error was: " + x) choiceValue2
```

Tree 作为函子

```F#
type Tree<'a> =
    | Tree of 'a * Tree<'a> * Tree<'a>
    | Leaf of 'a

module Tree = let rec map f = function 
                | Leaf x        -> Leaf (f x) 
                | Tree(x,t1,t2) -> Tree(f x, map f t1, map f t2)

let myTree = Tree(6, Tree(2, Leaf 1, Leaf 3), Leaf 9)

let r11 = Tree.map (fun x -> string (x + 10)) myTree
```

问：String 是函子吗？

```F#
let r12 = String.map (fun c -> System.Char.ToUpper(c)) "Hello world"
```

答：有点，但我们不能改变包装类型。如果我们假设 'a=char 和 C<'a> = String，我们就坚持 ('a->'a) -> C<'a> -> C<'a> 

最后是一些定律：

- `map id = id`
- `map (f >> g) = map f >> map g`

限制：

我们可以先定义 `map2`，然后定义 `map3` 然后 .. `mapN`？

```F#
type Option<'T> with
    static member map2 f x y = 
        match x, y with
        | Some x, Some y -> Some (f x y)
        | _              -> None

    static member map3 f x y z = 
        match x, y, z with
        | Some x, Some y, Some z -> Some (f x y z)
        | _                      -> None

let r13 = Option.map2 (+) (Some 2) (Some 3)

let r14 = List.map2 (+) [1;2;3] [10;11;12]

let add3 a b c = a + b + c

let r15 = Option.map3 add3 (Some 2) (Some 2) (Some 1)
```

问：有可能推广到 mapN 吗？

## 应用函子（Applicative Functors）

如果我们把 `map` 分成两步呢？

```F#
// map ('a -> 'b) -> C<'a> -> C<'b>
//     \--------/    \---/    \---/
//         (a)        (b)      (c)
//
// 1)    ('a -> 'b)        ->  C<'a -> 'b>
//       \--------/            \---------/
//           (a)                      
//               
// 2)  C<'a -> 'b> -> C<'a>  ->   C<'b>
//     \---------/    \---/       \---/
//                     (b)         (c)
//
//
// step1   ('a -> 'b)        ->  "C<'a -> 'b>"      Put the function into a context C
// step2 "C<'a -> 'b>" C<'a> ->   C<'b>             Apply the function in a context C to a value in a context C
```

下面是一个选项示例

```F#
let step1 f = Some f
let step2 a b = 
    match a, b with
    | Some f, Some x -> Some (f x)
    | _              -> None

let r16 = step1 (fun x -> string (x + 10))
let r17 = step2 r16 (Some 5)

```

所以现在不要写：

```F#
let r18  = Option.map (fun x -> string (x + 10)) (Some 5)
```

我们这样写：

```F#
let r18' = step2 (step1 (fun x -> string (x + 10))) (Some 5)
```

而不是像这样的 `map2`：

```F#
let r19   = Option.map2 (+) (Some 2) (Some 3)
```

我们这样写

```F#
let r19i  = step2 (step1 (+)) (Some 2)
```

.. 最终

```F#
let r19' = step2 r19i (Some 3)
```

通过再次应用 `step2`。如果结果仍然是容器中的函数，我们可以再次应用 `step2`，就像部分应用程序一样。

让我们为 `step1` 和 `step2` 命名：`pure` 和 `<*>`

```F#
module OptionAsApplicative =
    let pure' x = Some x
    let (<*>) a b = 
        match a, b with
        | Some f, Some x -> Some (f x)
        | _              -> None

open OptionAsApplicative

let r18''  = Option.map (fun x -> string (x + 10)) (Some 5)

let r18''' = Some (fun x -> string (x + 10)) <*> Some 5
// analog to:
let r18'''' =     (fun x -> string (x + 10))          5
```

现在使用 `map3`（以及 mapN）

```F#
let r20 = Option.map3 add3 (Some 2) (Some 2) (Some 1)

let r20'  = Some add3 <*> Some 2 <*> Some 2 <*> Some 1
// analog to:
let r20''  =     add3          2          2          1
```

但即使没有 `add3`，我们也可以写 `1 + 2 + 2`，即 `1 + (2 + 2)`，与以下内容相同：

```F#
let r20'''  = (+) 1 ((+) 2 2)
```

选项变为：

```F#
let r20'''' = Some (+) <*> Some 1 <*> (Some (+) <*> Some 2 <*> Some 2)
```

把它和下面对比

```F#
let r20'''''  =    (+)          1     (     (+)          2          2)
```

我们知道 `apply` 是 F# 中的 `(<|)`

```F#
let r21     =      (+) <|       1 <|  (     (+) <|       2 <|       2)
let r21'    = Some (+) <*> Some 1 <*> (Some (+) <*> Some 2 <*> Some 2)
```

因此，在这一点上，“应用函子”这个名字应该是有意义的

问： 做 `Some ( (+) 1 ((+) 2 2) )` 不是更容易吗？我们最终得到了同样的结果。答：是的，在这种特殊情况下，情况是一样的，但如果我们没有 `Some 1` 而是 `None` 呢

```F#
let r22   = Some (+) <*> None <*> (Some (+) <*> Some 2 <*> Some 2)
```

这是因为我们在上下文中应用函数。

它看起来和在外面应用一样，但事实上，一些效果发生在幕后。

为了更好地理解，让我们退出 Option：

```F#
[<AutoOpen>]
module Async =
    let pure' x = async.Return x
    let (<*>) f x = async.Bind(f, fun x1 -> async.Bind(x, fun x2 -> pure'(x1 x2)))

    
let r23   = async {return (+)} <*> async {return 2} <*> async {return 3}

let r23'  = pure' (+) <*> pure' 2 <*> pure' 3
```

试试 `Async.RunSynchronously r23'`

```F#
let getLine = async { 
        let x = System.Console.ReadLine() 
        return  System.Int32.Parse x
    }

let r24  = pure' (+) <*> getLine <*> getLine
```

试试 `Async.RunSynchronously r24`

```F#
module ListAsApplicative =
    let pure' x = [x]        
    let (<*>)  f x = List.collect (fun x1 -> List.collect (fun x2 -> [x1 x2]) x) f

    (* here are two other possible implementations of (<*>) for List
    let (<*>) f x = f |> List.map (fun f -> x |> List.map (fun x -> f x)) |> List.concat
    let (<*>) f x= 
        seq {
                for f in f do
                for x in x do
                yield f x} |> Seq.toList *)

open ListAsApplicative

let r25 =  List.map (fun x -> string (x + 10)) [1;2;3]

let r25'  =       [fun x -> string (x + 10)] <*> [1..3]
let r25'' = pure' (fun x -> string (x + 10)) <*> [1..3]


let r26 = [string; fun x -> string (x + 10)] <*> [1;2;3]
```

因此，对于列表，`map2` 相当于写：

```F#
let r27 = [(+)] <*> [1;2] <*> [10;20;30]

let r28 = [(+);(-)] <*> [1;2] <*> [10;20;30]


    
module SeqAsApplicative =
    let pure' x = Seq.initInfinite (fun _ -> x)
    let (<*>) f x = Seq.zip f x |> Seq.map (fun (f,x) -> f x)

open SeqAsApplicative


let r29 =  Seq.map (fun x -> string (x + 10))    (seq [1;2;3])          |> Seq.toList
let r29' =   pure' (fun x -> string (x + 10)) <*> seq [1;2;3]           |> Seq.toList
    
let r30 = seq [(+);(-)] <*> seq [1;2] <*> seq [10;20;30]                |> Seq.toList  // compare it with r28
```

一个没有 `pure` 的异国情调的案例。

```F#
module MapAsApplicative = 
    let (<*>) (f:Map<'k,_>) x =
        Map (seq {
            for KeyValue(k, vf) in f do
                match Map.tryFind k x with
                | Some vx -> yield k, vf vx
                | _       -> () })


open MapAsApplicative

let r31 = Map ['a',(+);'b',(-)] <*> Map ['a',1;'b',2] <*> Map ['a',10;'b',20;'c',30] 

let r32 = Map ['c',(+);'b',(-)] <*> Map ['a',1;'b',2] <*> Map ['a',10;'b',20;'c',30] 
```

## 单子（Monads）

```F#
open OptionAsApplicative    
    
let a = Some 3
let b = Some 2
let c = Some 1

let half x = x / 2        

let f a b c =
    let x = a + b
    let y = half c
    x + y

let f' a b c =
    let x = Some (+)  <*> a <*> b
    let y = Some half <*> c
    Some (+) <*> x <*> y
    
let r33 = f' (Some 1) (Some 2) (Some 3)

let r33' = f' None (Some 2) (Some 3)
```

好的，但如果我想使用这样的函数：

```F#
let exactHalf x =
    if x % 2 = 0 then Some (x / 2)
    else None
```

不合适

```F#
// let f'' a b c =
//     let x = Some (+) <*> a <*> b
//     let y = Some exactHalf <*> c   // y will be inferred as option<option<int>>
//     Some (+) <*> x <*> y           // so this will not compile
```

问题是，我们使用的是普通函数。当我们将这些函数提升到 C 中时，我们得到的函数被封装在上下文中。使用Applicatives，我们可以在准备好使用的上下文中使用函数，也可以使用普通函数，我们可以用 `pure` 轻松提升它。

但 `exactHalf` 是另一回事：它的签名是 `int -> Option<int>`。此函数从纯值变为上下文中的值，因此：

1） 我们直接使用它，但我们首先需要从上下文中提取论点。

2） 我们在应用程序中使用它，我们将在另一个上下文中的上下文中获得一个值，因此我们需要将这两个上下文都展平。

Monad 为这两种替代方案提供了解决方案

```F#
// bind : C<'a> -> ('a->C<'b>) -> C<'b>
// join : C<C<'a>> -> C<'a>

module OptionAsMonad =
    let join  x   = Option.bind id x
    let (>>=) x f = Option.bind f x
    // in monads pure' is called return, unit or result, but it's essentially the same function.
    let return' x = Some x
        
open OptionAsMonad        



let f'' a b c =
    let x = Some (+) <*> a <*> b
    let y = Some exactHalf <*> c |> join
    Some (+) <*> x <*> y
    

let f''' a b c =
    let x = Some (+) <*> a <*> b
    let y = c >>= exactHalf
    Some (+) <*> x <*> y
```

所有单子都是自动适用的，记住 `<*>` 对于列表，它是：

`let (<*>) f x = List.collect (fun x1 -> List.collect (fun x2 -> [x1 x2]) x) f`

但 `List.collect` 实际上是 `bind`，`[x1 x2]` 是 `pure (x1 x2)`

```F#
// let (<*>) f x = f >>= (fun x1 -> x >>= (fun x2 -> pure' (x1 x2)))
```

`<*>` 的这个定义适用于所有单子。

问： 但是我们说所有的应用子都是函子，所以单子也应该是函子，对吧？答： 是的，它们是，这是基于 `bind` 和 `result`（也称为返回（return）或纯（pure））的 `map` 的一般定义

```F#
let map f x = x >>= (pure' << f)
```

推荐链接

解释相同，但附有图片  http://adit.io/posts/2013-04-17-functors,_applicatives,_and_monads_in_pictures.html

Haskell类型类 http://www.haskell.org/haskellwiki/Typeclassopedia



# 扩展

https://fsprojects.github.io/FSharpPlus/extensions.html

扩展可能是您所期望的：现有类型的辅助函数。

它们被定义为与在 FSharpPlus 命名空间下操作的类型同名的模块，因此可以通过以下方式访问：

```F#
open FSharpPlus
```

一些函数在可折叠类型中很常见，如对列表、数组和序列 `intercalate`，而另一些函数在包装容器中很常用，如对列表、数组、序列，以及选项和结果  `map`, `bind` 和 `apply`。

## 构造：

`singleton` 函数已经为 Seq、Array 和 List 定义，但 F#+ 为枚举器添加了它：

- Enumerator.singleton - 构造一个包含给定值的容器

要构造 MonadError 实例（Result 或 Choice），可以使用 result/throw：

- Result.result / Choice.result - 使用给定值构造（如 Ok 或 Choice1Of2）
- Result.throw / Choice.throw - 根据给定值构造一个错误（如 Error 或 Choice2/2）

也可以通过包装异常生成函数来构造：

- Option.protect - 异常时返回None
- Result.protect - 在异常时返回错误和异常值
- Choice.protect - 在异常时返回 Choice2Of2 和异常值

```F#
// throws "ArgumentException: The input sequence was empty."
let expectedSingleItem1 : int = List.exactlyOne []

// returns a Result.Error holding the exception as its value:
let expectedSingleItem2 : Result<int,exn> = Result.protect List.exactlyOne []

// ...or like typical try prefixed functions, treat exception as None
let expectedSingleItem3 : Option<int> = Option.protect List.exactlyOne []

// which might look like this:
let inline tryExactlyOne xs = Option.protect List.exactlyOne xs
```

## 解构（展开）：

Result 上的一些扩展旨在表现得像 Option：

- Result.get - 当值为 'ok 时打开包装，否则抛出异常
- Result.defaultValue - 如果存在，则返回 'ok 值，否则返回默认值
- Result.defaultWith - 如果存在，则返回 'ok 值，否则将给定函数应用于 'error 值

要解构 MonadError 实例（Result 或 Choice），请使用：

- Result.ether - 通过适当应用给定的 `ok` 或 `err` 函数来展开结果
- Choice.ether - 通过适当应用给定的 `choice1` 或 `choice2` 函数来展开选择

请注意，还有一个泛型的 `either` 运算符函数，其工作原理与 `Result.either` 完全相同。

此外，请参阅泛型函数 [option](https://fsprojects.github.io/FSharpPlus/reference/fsharpplus-operators.html)，以类似于 `either` 的方式展开 Option。

## 关于可折叠物（Foldables）

可折叠物（Foldables）是一类可以折叠成摘要值的数据结构。大多数集合，特别是“可折叠”实例都实现了以下功能：

- intersperse - 获取一个元素并将该元素“散布”在元素之间

  ```F#
  let a = ["Bob"; "Jane"] |> List.intersperse "and"
  // vat a : string list = ["Bob"; "and"; "Jane"]
  
  let b = "WooHoo" |> String.intersperse '-'
  // val b : string = "W-o-o-H-o-o"
  ```

- intercalate - 在每个元素之间插入一个元素列表并使其变平

  ```F#
  let c = [[1;2]; [3;4]] |> List.intercalate [-1;-2];;
  // val c : int list = [1; 2; -1; -2; 3; 4]
  
  let d = ["Woo"; "Hoo"] |> String.intercalate "--o.o--";;
  // val d : string = "Woo--o.o--Hoo"
  ```

- zip/unzip - 在两个容器内将值元组在一起，或将值非元组化

## 关于单子/函子/应用子

实现这些功能的类型（通常）将定义以下功能：

- map - 对容器内的值应用映射函数
- bind - 获取一个包含的值，并应用一个生成另一个包含值的函数
- apply - 类似 map，但映射函数也在容器内

这些也可以根据[泛型函数和运算符](https://fsprojects.github.io/FSharpPlus/reference/fsharpplus-operators.html)从没有模块前缀的泛型函数中调用。

## 压平（Flatten）：

当一个容器内有另一个容器时，可以使用 Flatten：

- Choice.flatten
- Result.flatten
- Option.flatten（已在 FSharp Core 中定义）

请注意，在 List、Array 和 Seq 等可遍历类型上，FSharp Core 使用更常见的 `concat` 进行展开，因此 Enumerable 继续使用此命名：

- Enumerable.concat

## 分区：

分区可以通过应用产生 Choice 的分隔函数来完成：

- Array.partitionMap
- List.partitionMap

```F#
let isEven x = (x % 2) = 0
let chooseEven x = if isEven x then Choice1Of2 x else Choice2Of2 x

let e = [1; 2; 3; 4] |> List.partitionMap chooseEven
// val e : int list * int list = ([2; 4], [1; 3])
```

## 转换函数：

F#+ 添加了在 Result、Choice 和 Option 类型之间转换的函数。

这些应该是不言自明的，但要注意，有时它们在转换为 Option 时通常是“有损的（lossy）”：

//将 `Result` 转换为 `Option` - 有效地丢弃存在的错误值 // 如果出现，替换为 `None`，

```F#
request |> validateRequest |> Option.ofResult
```

反之亦然，但需要为 None 填写一个值：

```F#
let xs = ["some value"]
let firstElementOption = xs |> List.tryHead

// Convert an `Option` to a `Result` will use unit as the Error:
firstElementOption |> Option.toResult

// ...but you can specify an error value with Option.toResultWith:
firstElementOption |> Option.toResultWith "No Element"
```

在 `Choice` 和 `Result` 之间进行转换通常很有用：

```F#
let asyncChoice = anAsyncValue |> Async.Catch |> Async.map Result.ofChoice
```

## 字符串类型：

- [String](https://fsprojects.github.io/FSharpPlus/reference/fsharpplus-string.html)
  - intercalate, intersperse,
  - split, replace
  - isSubString, startsWith, endsWith, contains
  - toUpper, toLower
  - trimWhiteSpaces
  - normalize
  - removeDiacritics
  - padLeft, padLeftWith, padRight, padRightWith
  - trim, trimStart, trimEnd
  - item, tryItem
  - rev
  - take, skip takeWhile, skipWhile
  - truncate, drop
  - findIndex, tryFindIndex
  - findSliceIndex, tryFindSliceIndex
  - findLastSliceIndex, tryFindLastSliceIndex
  - toArray, ofArray, toList, ofList, toSeq, ofSeq, toCodePoints, ofCodePoints
  - getBytes

## 集合/可遍历类型：

- [Array](https://fsprojects.github.io/FSharpPlus/reference/fsharpplus-array.html)
  - intercalate, intersperse,
  - split, replace,
  - findSliceIndex, trySliceIndex,
  - findLastSliceIndex, tryLastSliceIndex,
  - partitionMap
- [IList](https://fsprojects.github.io/FSharpPlus/reference/fsharpplus-ilist.html)
  - toIReadOnlyList
- [List](https://fsprojects.github.io/FSharpPlus/reference/fsharpplus-list.html)
  - singleton,
  - cons,
  - apply,
  - tails, take, skip, drop,
  - intercalate, intersperse,
  - split, replace,
  - toIReadOnlyList,
  - findSliceIndex, tryFindSliceIndex,
  - findLastSliceIndex, tryLastSliceIndex,
  - partitionMap
  - setAt, removeAt
- [Enumerator](https://fsprojects.github.io/FSharpPlus/reference/fsharpplus-enumerator.html)
  - EmptyEnumerator
    - Empty - 创建一个空枚举器
  - ConcatEnumerator
    - concat
  - MapEnumerators
    - map, mapi, map2, mapi2, map3
  - singleton
  - tryItem, nth
  - choose
  - filter
  - unfold
  - upto
  - zip, zip3
- [Seq](https://fsprojects.github.io/FSharpPlus/reference/fsharpplus-seq.html)
  - bind, apply, foldback
  - chunkBy
  - intersperse, intercalate,
  - split, replace
  - drop
  - replicate
  - toIReadOnlyList
  - findSliceIndex, tryFindSliceIndex
  - findLastSliceIndex, tryLastSliceIndex,
- [IReadOnlyCollection](https://fsprojects.github.io/FSharpPlus/reference/fsharpplus-ireadonlycollection.html)
  - ofArray, ofList, ofSeq
  - map
- [IReadOnlyList](https://fsprojects.github.io/FSharpPlus/reference/fsharpplus-ireadonlylist.html)
  - ofArray, toArray
  - trySetItem, tryItem
- [Map](https://fsprojects.github.io/FSharpPlus/reference/fsharpplus-map.html)
  - keys, values
  - mapValues, mapValues2
  - zip, unzip
  - unionWith, union, intersectWith, intersect
- [Dict](https://fsprojects.github.io/FSharpPlus/reference/fsharpplus-dict.html)
  - toIReadOnlyDictionary
  - tryGetValue
  - containsKey
  - keys, values
  - map, map2
  - zip, unzip
  - unionWith, union, intersectWith, intersect
- [IReadOnlyDictionary](https://fsprojects.github.io/FSharpPlus/reference/fsharpplus-ireadonlydictionary.html)
  - add,
  - tryGetValue, containsKey,
  - keys, values,
  - map, map2,
  - zip, unzip,
  - unionWith, union, intersectWith, intersect

## 异步（Async）、任务（Task）和值任务（ValueTask）：

- [Task](https://fsprojects.github.io/FSharpPlus/reference/fsharpplus-task.html)
  - map, map2, map3
  - apply
  - zip
  - join
  - ignore
- [ValueTask](https://fsprojects.github.io/FSharpPlus/reference/fsharpplus-valueTask.html)
  - map, map2, map3
  - apply
  - zip
  - join
  - ignore
- [Async](https://fsprojects.github.io/FSharpPlus/reference/fsharpplus-async.html)
  - map, map2
  - zip
  - join
  - apply
  - raise

## 选项（Option）、选择（Choice）和结果（Result）类型：

- [Option](https://fsprojects.github.io/FSharpPlus/reference/fsharpplus-option.html)
  - apply,
  - unzip, zip,
  - toResult, toResultWith, ofResult,
  - protect
- [Choice](https://fsprojects.github.io/FSharpPlus/reference/fsharpplus-choice.html)
  - result, throw - 构造一个 Choice
  - bind, apply, flatten,
  - map,
  - catch, - deprecated
  - bindChoice2Of2,
  - either,
  - protect
- [Result](https://fsprojects.github.io/FSharpPlus/reference/fsharpplus-result.html)
  - result, throw - 构造一个 Result
  - apply, (map, bind 已经被定义)
  - flatten,
  - bindError,
  - either,
  - protect,
  - get,
  - defaultValue, defaultWith,
  - toChoice, ofChoice,
  - partition

## 扩展方法（基于现有类型）：

这些可以从 C# 中使用

- [扩展方法](https://fsprojects.github.io/FSharpPlus/extension-methods.html)
  - IEnumerable<'T'>.GetSlice
  - List<'T>.GetSlice
  - Task<'T>.WhenAll
  - Async.Sequence - of seq, list or array
  - Option.Sequence - of seq



# 扩展方法

https://fsprojects.github.io/FSharpPlus/extension-methods.html

一些方法也作为扩展公开。这使得 C# 的一些用法成为可能

以下是一些示例：

```F#
#r @"nuget: FSharpPlus"
open FSharpPlus.Extensions

let opt  = Option.Sequential [Some 1; Some 2]
let asn = Async.Sequential [| async {return 1}; async {return 2} |]
```



# 泛型运算符和函数

https://fsprojects.github.io/FSharpPlus/generic-doc.html

在审查了[扩展函数](https://fsprojects.github.io/FSharpPlus/extensions.html)之后，很自然地想要使用可以跨不同类型工作的泛型函数。

F#+ 实现了泛型函数，这些函数可以有效地调用特定的实现。这将处理现有的 .Net 和 F# 类型，您可以通过实现预期的方法名和签名在自己和第三方类型上使用它们。

了解具体操作符：

- 关于[运算符的文档-常见组合器](https://fsprojects.github.io/FSharpPlus/operators-common.html)
- 每个[抽象](https://fsprojects.github.io/FSharpPlus/abstractions.html)都有其他文档
- [泛型函数和运算符](https://fsprojects.github.io/FSharpPlus/reference/fsharpplus-operators.html)的 API 文档

它们特别有用，因为调用的特定函数将取决于输入参数和返回类型。然而，这意味着如果这些信息不可用，有时需要显式指定类型（实际上，当编译器告诉类型错误时，临时显式添加类型是一种很好的调试技术）。

例如：

```F#
// Convert the number 42 to bytes... 
// ... here the type is known (42 is an int, the return value is byte[])
let a = 42 |> toBytes;;  
//val a : byte [] = [|42uy; 0uy; 0uy; 0uy|]

// However, this can't compile since the return type is not inferrable
// let b = [|42uy; 0uy; 0uy; 0uy|] |> ofBytes;;  

// The error will be something like:
// 
//  let b = [|42uy; 0uy; 0uy; 0uy|] |> ofBytes;;
//  -----------------------------------^^^^^^^
//
// error FS0071: Type constraint mismatch when applying the default type 'obj'
// for a type inference variable. No overloads match for method 'OfBytes'.
// The available overloads are shown below. Consider adding further type constraints

// [followed by many possible implementations...]

// So, in this case, we have to give the return type:
let b :int = [|42uy; 0uy; 0uy; 0uy|] |> ofBytes;;
// val b : int = 42

// ...or, the more usual case, you use in context where type can be inferred,
// like this example:
1 + ([|42uy; 0uy; 0uy; 0uy|] |> ofBytes);;
//val it : int = 43
```

## 泛型函数是如何工作的？

F# 不支持重载函数，但它确实支持类型（类）上的重载方法，包括静态方法。F#+ 通过定义泛型函数来利用这一点，这些泛型函数调用一个内部类（称为“可调用（Invokable）”），在该类中定义了各种重载的静态方法。

编写 Invokable 是为了解决现有的 .Net 和 F# 类型最具体、最优化的重载问题，否则将使用更泛型的实现。

这一切意味着什么？

这意味着要注意使用最优化的实现，如果你实现了所需的方法，你可以实现自己的泛型函数实例。

## 示例

以下是对现有 .NET 和 F# 类型进行泛型 `map` 操作的一些示例：

```F#
map string [|2;3;4;5|]
// val it : string [] = [|"2"; "3"; "4"; "5"|]

map ((+) 9) (Some 3)
// val it : int option = Some 12

let res12 = map ((+) 9) (async {return 3})
// val it : Async<int> = Microsoft.FSharp.Control.FSharpAsync`1[System.Int32]
extract res12
// val it : int = 12
```

以下是此库中定义的类型的一些示例：

```F#
open FSharpPlus.Data

map string (NonEmptyList.create 2 [3;4;5])
// val it : NonEmptyList<string> = {Head = "2"; Tail = ["3"; "4"; "5"];}

let stateFul42 = map string (State (fun x -> (42, x)))
State.run stateFul42 "state"
// val stateFul42 : State<string,string> = State <fun:map@12-9>
// val it : string * string = ("42", "state")
```

现在，让我们用自己的映射定义来定义自己的类型

```F#
type Tree<'t> =
    | Tree of 't * Tree<'t> * Tree<'t>
    | Leaf of 't
    static member Map (x:Tree<'a>, f) = 
        let rec loop f = function
            | Leaf x -> Leaf (f x)
            | Tree (x, t1, t2) -> Tree (f x, loop f t1, loop f t2)
        loop f x

map ((*) 10) (Tree(6, Tree(2, Leaf 1, Leaf 3), Leaf 9))
// val it : Tree<int> = Tree (60,Tree (20,Leaf 10,Leaf 30),Leaf 90)
```

对于在外部库中定义的类型，当它包含与预期名称和签名匹配的静态成员时，它将工作。

以下是针对 MathNet 库中定义的类型的 `fromBigInt` 泛型函数的一个示例

```F#
#r "../../packages/docs/MathNet.Numerics/lib/net40/MathNet.Numerics.dll"
#r "../../packages/docs/MathNet.Numerics.FSharp/lib/net45/MathNet.Numerics.FSharp.dll"

let x : MathNet.Numerics.BigRational = fromBigInt 10I
```



# 运算符-常见组合器

https://fsprojects.github.io/FSharpPlus/operators-common.html

这些通用函数和运算符是常用的，不是任何其他抽象的一部分。

您可以在 API 文档中找到这些：[Operators.fs](https://fsprojects.github.io/FSharpPlus/reference/operators.html)

## flip

使用翻转的前两个参数创建新函数。

## konst

创建一个始终返回给定参数的函数。这被称为“常数”函数。

当需要函数作为灵活性的参数，但在特定实例中不需要时，这通常很有用。

例子：

## curry, uncurry, curryN, uncurryN

柯里化（Currying）是接受一个期望元组的函数，并返回一个参数数量与元组大小相同的函数的过程。

去柯里化（Uncurrying）是相反的过程。

`curry` 和 `uncurry` 分别处理两个参数，而 `curryN` 和 `uncurryN` 处理任何数字。

例子：

```F#
let addThreeNums (x, y, z) = x + y + z;;
// val addThreeNums : x:int * y:int * z:int -> int

let b = curryN addThreeNums 1 2 3;;
// val it : int = 6
```

## 作为操作符 - 的函数

定义了一对运算符 `</` 和 `/>`，允许将任何函数用作运算符。它将翻转函数的参数，以便在第一个参数来自左侧时有意义。

例子：

```F#
let biggerThan a b = a > b;;
// val biggerThan : a:'a -> b:'a -> bool when 'a : comparison

let c = 10 </biggerThan/> 3;;
// val c : bool = true
```

## tap

Tap 执行一个副作用函数，然后返回原始输入值。将此视为“利用”一系列功能。

例子：

```F#
// a pipeline of functions, with a tap in the middle
let names = ["John"; "Smith"]
names |> map String.toUpper |> tap (printfn "%A") |> map String.toLower;;

// prints this:
// ["JOHN"; "SMITH"]

// but returns this:
// val it : string list = ["john"; "smith"]
```

## either

从任一侧提取 Result 中的值，无论是 Ok 还是 Error。

它需要两个功能：

- fOk - 如果源包含 Ok 值，则应用于源的函数
- fError - 如果源包含 Error 值，则应用于源的函数

…以及来源：

- source - 包含 Ok 或 Error 的源值

```F#
let myResult = Ok "I am ok!";;
// val myResult : Result<string,'a>

let myOther = Error -1;;
// val myOther : Result<'a,int>

let d = either id id myResult;;
// val d : string = "I am ok!"

let e = either id id myOther;;
// val e : int = -1
```

不要将 `either` 函数与将值提升为 Functor 的 `result` 混淆，就像在计算表达式中的 `return` 一样。

## option

接受一个函数、一个默认值和一个选项值。如果选项值为 None，则函数返回默认值。否则，它将函数应用于 Some 中的值并返回结果。

```F#
let inline option f n = function Some x -> f x | None -> n
```

## tuple2, tuple3, ... tuple8

生成元组的函数。该数字表示定义的参数数量以及元组的相应大小。

```F#
let inline tuple2 a b             = a,b
let inline tuple3 a b c           = a,b,c
let inline tuple4 a b c d         = a,b,c,d
let inline tuple5 a b c d e       = a,b,c,d,e
let inline tuple6 a b c d e f     = a,b,c,d,e,f
let inline tuple7 a b c d e f g   = a,b,c,d,e,f,g
let inline tuple8 a b c d e f g h = a,b,c,d,e,f,g,h
```

## Explicit

Explicit 允许您对标准类型和实现静态显式类型转换签名的类型使用 `explicit` 泛型方法。

### 最小定义

为了将 `explicit` 泛型方法与类型一起使用，它需要实现以下内容：

```F#
static member op_Explicit (x:'r) :'T
```

或者在 C# 中

```c#
public static explicit operator T(R s)
```

这在处理大量使用显式转换的C#库时非常有用。

## Implicit

Implicit 允许您对标准类型和实现静态隐式类型强制转换签名的类型使用 `implicit` 泛型方法。

### 最小定义

为了将 `implicit` 泛型方法与类型一起使用，它需要实现以下内容：

```F#
static member op_Implicit (x:'r) :'T
```

或在 C# 中

```c#
public static implicit operator T(R s)
```

这在处理大量使用隐式转换的 C# 库时非常有用。



# 类型

https://fsprojects.github.io/FSharpPlus/types.html

点击类型名称查看完整描述：

- [All](https://fsprojects.github.io/FSharpPlus/type-all.html)
- [Any](https://fsprojects.github.io/FSharpPlus/type-any.html)
- [ChoiceT](https://fsprojects.github.io/FSharpPlus/type-choicet.html)
- [Compose](https://fsprojects.github.io/FSharpPlus/type-compose.html)
- [Const](https://fsprojects.github.io/FSharpPlus/type-const.html)
- [Cont](https://fsprojects.github.io/FSharpPlus/type-cont.html)
- [ContT](https://fsprojects.github.io/FSharpPlus/type-contt.html)
- [Coproduct](https://fsprojects.github.io/FSharpPlus/type-coproduct.html)
- [DList](https://fsprojects.github.io/FSharpPlus/type-dlist.html)
- [Dual](https://fsprojects.github.io/FSharpPlus/type-dual.html)
- [Endo](https://fsprojects.github.io/FSharpPlus/type-endo.html)
- [First](https://fsprojects.github.io/FSharpPlus/type-first.html)
- [Free](https://fsprojects.github.io/FSharpPlus/type-free.html)
- [Identity](https://fsprojects.github.io/FSharpPlus/type-identity.html)
- [Kleisli](https://fsprojects.github.io/FSharpPlus/type-kleisli.html)
- [Last](https://fsprojects.github.io/FSharpPlus/type-last.html)
- [ListT](https://fsprojects.github.io/FSharpPlus/type-listt.html)
- [Mult](https://fsprojects.github.io/FSharpPlus/type-mult.html)
- [NonEmptyList](https://fsprojects.github.io/FSharpPlus/type-nonempty.html)
- [OptionT](https://fsprojects.github.io/FSharpPlus/type-optiont.html)
- [ParallelArray](https://fsprojects.github.io/FSharpPlus/type-parallelarray.html)
- [Reader](https://fsprojects.github.io/FSharpPlus/type-reader.html)
- [ReaderT](https://fsprojects.github.io/FSharpPlus/type-readert.html)
- [ResultT](https://fsprojects.github.io/FSharpPlus/type-resultt.html)
- [SeqT](https://fsprojects.github.io/FSharpPlus/type-seqt.html)
- [State](https://fsprojects.github.io/FSharpPlus/type-state.html)
- [StateT](https://fsprojects.github.io/FSharpPlus/type-statet.html)
- [Validation](https://fsprojects.github.io/FSharpPlus/type-validation.html)
- [Writer](https://fsprojects.github.io/FSharpPlus/type-writer.html)
- [WriterT](https://fsprojects.github.io/FSharpPlus/type-writert.html)
- [ZipList](https://fsprojects.github.io/FSharpPlus/type-ziplist.html)

## All

https://fsprojects.github.io/FSharpPlus/type-all.html

这是布尔值的包装类型，具有一组特定的单值运算。只有当两个（所有）操作数都为真时，包含的布尔值才会变为真。

### 相关类型

- [Any](https://fsprojects.github.io/FSharpPlus/type-any.html)：类似的包装器，但使用“Any”标准。

### 抽象

- [Semigroup](https://fsprojects.github.io/FSharpPlus/abstraction-semigroup.html)
- [Monoid](https://fsprojects.github.io/FSharpPlus/abstraction-monoid.html)

### 示例

```F#
#r @"nuget: FSharpPlus"
open FSharpPlus
open FSharpPlus.Data

let res1 = All true ++ zero ++ All false
// val res1 : All = All false

let even x = x % 2 = 0

let res2 = [2;4;6;7;8] |> map (even >> All) |> sum
// val res2 : All = All false
```

## Any

https://fsprojects.github.io/FSharpPlus/type-any.html

这是布尔值的包装类型，具有一组特定的单值运算。只有当（任何）操作数之一为真时，包含的布尔值才会变为真。

### 相关类型

- [All](https://fsprojects.github.io/FSharpPlus/type-all.html)：类似的包装器，但使用“All”标准。

### 抽象

- [Semigroup](https://fsprojects.github.io/FSharpPlus/abstraction-semigroup.html)
- [Monoid](https://fsprojects.github.io/FSharpPlus/abstraction-monoid.html)

### 示例

```F#
#r @"nuget: FSharpPlus"
open FSharpPlus
open FSharpPlus.Data

let res1 = Any true ++ zero ++ Any false
// val res1 : Any = Any true

let even x = x % 2 = 0

let res2 = [2;4;6;7;8] |> map (even >> Any) |> sum
// val res2 : Any = Any true
```

## Compose

https://fsprojects.github.io/FSharpPlus/type-compose.html

允许组合应用子和函子。

值得注意的是：

- 由 2 个 functor 组成的组合是一个 functor
- 2 个应用子的组合是应用子
- 2 个单子的组合并不总是单子

### 示例

```F#
#r @"nuget: FSharpPlus"
open FSharpPlus
open FSharpPlus.Data

// First let's create some values

let (one : Async<Result<int, string>>) = async { return Ok 1 }
let (two : Async<Result<int, string>>) = async { return Ok 2 }

// Now we can combine then

let (Compose three) = Compose (async {return Ok (+)}) <*> Compose one <*> Compose two
// val three : Async<FSharpPlus.Result<int,string>>

// or shorter

let (Compose three') = (+) <!> Compose one <*> Compose two
// val three' : Async<FSharpPlus.Result<int,string>>
```

## Const<'T, 'U>

https://fsprojects.github.io/FSharpPlus/type-const.html

Const 函子，定义为 Const<'T, 'U>，其中 'U 是幻影类型。适用于：镜头 getter。它的应用子实例在镜头中起着基础性的作用。

### 示例

```F#
#r @"nuget: FSharpPlus" 
open FSharpPlus
open FSharpPlus.Lens
open FSharpPlus.Data
// note for instance the definition of view (from the Lens part of F#+):
let view (optic: ('a -> Const<_,'b>) -> _ -> Const<_,'t>) (source: 's) : 'a = Const.run (optic Const source)
```



## Cont<'R,'U>

https://fsprojects.github.io/FSharpPlus/type-cont.html

`Cont` 计算类型表示可以中断和恢复的计算。

其中一些示例改编自 [github 上的 fabriceleal/Continuations](https://github.com/fabriceleal/Continuations/blob/master/Continuations/Program.fs)。

你可以在 [Markh Needhams 的博客文章](http://www.markhneedham.com/blog/2009/06/22/f-continuation-passing-style/)中阅读这种风格，或者阅读 [Tomas Petricek 和 Jon Skeet 合著的《真实世界函数式编程（Real World Functional Programming）》](https://livebook.manning.com/book/real-world-functional-programming/chapter-10/156)。

### 示例

为了了解这种风格，让我们对比一些例子，以及在使用 F#+ 或没有帮助的情况下它们的外观。

```F#
#r @"nuget: FSharpPlus"
open FSharpPlus
open FSharpPlus.Data

let assertEqual expected actual = 
    if expected <> actual then
        failwithf "%A != %A" expected actual
```

示例 g k

```F#
let g n = n + 1
let f n = g(n + 1) + 1

module ``EXAMPLE g k`` =
    let g_k n k = k(n + 1)
    let f_k n k = g_k(n + 1) (fun x -> k(x + 1))
    f_k 1 (fun x -> assertEqual (f 1) x)
    f_k 2 (fun x -> assertEqual (f 2) x)


module ``EXAMPLE g k in FSharpPlus`` =
    let g_k n : Cont<int,int> = monad { return (n + 1) }
    let f_k n = monad {
      let! x= g_k(n + 1) 
      return x+1
    }
    let n = 2
    let res = Cont.run (f_k n) id
    assertEqual (f n) res
```

示例 Max

```F#
// Max, regular-style
let max x y =
    if x > y then x else y

module ``EXAMPLE max`` =

    // Max, CPS-style
    let max_k x y k =
        if x > y then k x else k y
    // More CPS Styl-ish
    max_k 1 2 (fun x -> assertEqual (max 1 2) x)

module ``EXAMPLE max in FSharpPlus`` =
    let max_k x y = monad {
        return if x > y then x else y }
    let x = Cont.run (max_k 1 2) id
    assertEqual (max 1 2) x
```

示例 阶乘

```F#
// regular factorial
let rec factorial n =
    if n = 0 then
        1
    else
        n * factorial (n-1)

module ``EXAMPLE factorial`` =
    let rec factorial_k n k =
        if n = 0 then
            k 1
        else
            factorial_k (n-1) (fun x -> k(x * n))

    let fact_n = 5
    factorial_k fact_n (fun x -> assertEqual (factorial fact_n) x)

module ``EXAMPLE factorial in FSharpPlus`` =
    let rec factorial_k n = monad {
        if n = 0 then
            return 1
        else
            let! x=factorial_k (n-1)
            return x * n
      }
    let fact_n = 5
    let x = Cont.run (factorial_k fact_n) id
    assertEqual (factorial fact_n) x
```

示例 求和

```F#
// sum
let rec sum x =
    if x = 1 then
        1
    else
        sum(x - 1) + x

module ``EXAMPLE sum`` =

    let rec sum_k x k =
        if x = 1 then
            k 1
        else
            sum_k(x - 1) (fun y -> k(x + y))

    let sum_n = 5
    sum_k sum_n (fun t ->  assertEqual (sum sum_n) t)
module ``EXAMPLE sum in FSharpPlus`` =

    let rec sum_k x = monad {
        if x = 1 then
            return 1
        else
            let! y=sum_k(x - 1)
            return x + y
      }

    let sum_n = 5
    let t = Cont.run (sum_k sum_n) id
    assertEqual (sum sum_n) t
```

示例 斐波那契数

```F#
// fibo
let rec fibo n =
    if n = 0 then
        1
    else if n = 1 then
        1
        else
            fibo (n - 1) + fibo (n - 2)

module ``EXAMPLE fibo`` =
    let rec fibo_k n k =
        if n = 0 then
            k 1
        else if n = 1 then 
            k 1
            else
                let k_new1 = (fun x1 -> 
                    let k_new2 = (fun x2 -> k(x1 + x2))
                    fibo_k (n - 2) k_new2
                )
                fibo_k (n - 1) k_new1

    let fibo_n = 9
    fibo_k fibo_n (fun x -> assertEqual (fibo fibo_n) x)
module ``EXAMPLE fibo in FSharpPlus`` =
    let rec fibo_k n =
      monad {
        if n = 0 then
            return 1
        else if n = 1 then 
            return 1
            else
                let! x1 = fibo_k (n - 1)
                let! x2 = fibo_k (n - 2)
                return x1+x2
      }
    let fibo_n = 9
    let x = Cont.run (fibo_k fibo_n) id
    assertEqual (fibo fibo_n) x
```

示例 第 n 大数

```F#
// nth
let rec nth n (ls : 'a list) =
    if ls.IsEmpty then
        None
    else if n = 0 then
        Some(ls.Head)
    else
        nth (n - 1) ls.Tail

module ``EXAMPLE nth`` =

    let rec nth_k n (ls : 'a list) k =
        if ls.IsEmpty then
            k(None)
        else if n = 0 then
            k(Some(ls.Head))
        else
            nth_k (n - 1) ls.Tail k
    let ls, i1, i2 = [1;2;3;4;5;6], 3, 15

    // becomes:
    nth_k i1 ls (fun x->assertEqual (nth i1 ls) x)

    nth_k i2 ls (fun x->assertEqual (nth i2 ls) x)


#nowarn "0064"
module ``EXAMPLE nth in FSharpPlus`` =

    let rec nth_k n (ls : 'a list) = monad {
        if ls.IsEmpty then
            return (None)
        else if n = 0 then
            return (Some(ls.Head))
        else
            let! r=nth_k (n - 1) ls.Tail
            return r
      }
    let ls, i1, i2 = [1;2;3;4;5;6], 3, 15

    // becomes:
    let x = Cont.run (nth_k i1 ls) id
    assertEqual (nth i1 ls) x

    let x2 = Cont.run (nth_k i2 ls) id
    assertEqual (nth i2 ls) x2
```

示例 树节点计数

```F#
type Tree =
    | Node of Tree * Tree
    | Leaf
// node_count
let rec node_count = function
                    | Node(lt, rt) -> 1 + node_count(lt)  + node_count(rt)
                    | Leaf -> 0

module ``EXAMPLE count_nodes`` =
    let rec node_count_k tree k = match tree with
                                    | Node(ltree, rtree) ->
                                        let new_k1 = (fun ltree_count -> 
                                            let new_k2 = (fun rtree_count -> 
                                                k(1 + ltree_count + rtree_count)
                                            )
                                            node_count_k rtree new_k2
                                        )
                                        node_count_k ltree new_k1
                                    | Leaf -> k 0

    let t = Node(Node(Leaf, Leaf), Node(Leaf, Node(Leaf, Node(Leaf, Leaf))))
    node_count_k t (fun count -> assertEqual (node_count t)  count)

module ``EXAMPLE count_nodes in FSharpPlus`` =
    let rec node_count_k tree = 
                                monad {
                                    match tree with
                                    | Node(lt, rt) -> 
                                        let! x_lt=node_count_k(lt)
                                        let! x_rt=node_count_k(rt)
                                        return 1 + x_lt + x_rt
                                    | Leaf -> return 0
                                }
    let t = Node(Node(Leaf, Leaf), Node(Leaf, Node(Leaf, Node(Leaf, Leaf))))
    let count = Cont.run (node_count_k t) id
    assertEqual (node_count t)  count
```

## DList

https://fsprojects.github.io/FSharpPlus/type-dlist.html

DList 是一个有序的线性结构，实现了 List 签名（head、tail、cons）、末尾插入（add）和 O(1) 追加。按插入历史排序。DList 是 [John Hughes 附加列表](http://dl.acm.org/citation.cfm?id=8475)的实现。

### 示例

```F#
#r @"nuget: FSharpPlus"
open FSharpPlus
open FSharpPlus.Data
```

### 构造 DLists

```F#
// you can construct a DList by using ofSeq
let list123 = DList.ofSeq [ 1; 2; 3 ]

let listEmpty = DList.empty
// cons
let list2 = DList.cons 100 list123 
// append two DLists
let list3 = DList.append list2 (DList.singleton 200)
// this can be written as (since list2 is a DList):
let list3' = plus list2 (result 200)
// in order to get back to a regular list you can then use toList:
let list4 = toList list3'
```

### DList 的操作符

```F#
let lengthOfList3 = DList.length list3
let lengthOfList3' = length list3

let headOf3 = DList.head list3 
let headOf3' = head list3 
```

## Free<'Functor<'T>, 'T>

https://fsprojects.github.io/FSharpPlus/type-free.html

这种类型是 [Free Monad](https://www.google.com/search?q=free+monad) 的实现，它对任何 [Functor](https://fsprojects.github.io/FSharpPlus/abstraction-functor.html) 都是通用的。

Free Monad 通常用于从高层描述纯程序，并为其单独编写不同的解释器。

### 相关类型

- [Coproduct](https://fsprojects.github.io/FSharpPlus/type-coproduct.html)：一个与 Free Monad 结合使用的函数器，用于组合不同的指令集。

### 示例

[Mark Seemann 博客](https://blog.ploeh.dk/2017/07/17/a-pure-command-line-wizard)中的 F# 自由 monad 解释器，但使用 Free 编码。

```F#
#r @"nuget: FSharpPlus"
open System
open FSharpPlus
open FSharpPlus.Data


type CommandLineInstruction<'t> =
    | ReadLine  of (string -> 't)
    | WriteLine of  string  * 't
with static member Map (x, f) =
        match x with
        | ReadLine   g     -> ReadLine  (f << g)
        | WriteLine (s, g) -> WriteLine (s, f g)

let readLine    = Free.liftF (ReadLine id)
let writeLine s = Free.liftF (WriteLine (s, ()))


let rec interpretCommandLine = Free.run >> function
    | Pure x -> x
    | Roll (ReadLine      next)  -> Console.ReadLine () |> next |> interpretCommandLine
    | Roll (WriteLine (s, next)) ->
        Console.WriteLine s
        next |> interpretCommandLine

let rec readQuantity = monad {
    do! writeLine "Please enter number of diners:"
    let! l = readLine
    match tryParse l with
    | Some dinerCount -> return dinerCount
    | None ->
        do! writeLine "Not an integer."
        return! readQuantity }

let rec readDate = monad {
    do! writeLine "Please enter your desired date:"
    let! l = readLine
    match DateTimeOffset.TryParse l with
    | true, dt -> return dt
    | _ ->
        do! writeLine "Not a date."
        return! readDate }

let readName = monad {
    do! writeLine "Please enter your name:"
    return! readLine }
 
let readEmail = monad {
    do! writeLine "Please enter your email address:"
    return! readLine }


type Reservation = {
    Date : DateTimeOffset
    Name : string
    Email : string
    Quantity : int }
    with static member Create (Quantity, Date, Name, Email) = { Date = Date; Name = Name; Email = Email; Quantity = Quantity }

let readReservationRequest =
    curryN Reservation.Create
    <!> readQuantity
    <*> readDate
    <*> readName
    <*> readEmail



let mainFunc () =
    readReservationRequest
    >>= (writeLine << (sprintf "%A"))
    |> interpretCommandLine
    0
```

### 推荐阅读

- 强烈推荐 Matt Thornton的博客 [Grokking Free Monads](https://dev.to/choc13/grokking-free-monads-9jd) 和 [Interpreting Free Monads](https://dev.to/choc13/interpreting-free-monads-3l3e)。它包含使用 F#+ 的示例和从头开始的解释。
- Mark Seemann的博客有[一系列文章](https://blog.ploeh.dk/2017/06/27/pure-times/)，最终描述了 Free Monads，尽管他不使用 F#+，因此要么重复样板代码，要么切换到 Haskell。无论如何，这些系列中的一些代码（如上面的片段）可以在[我们的 Free 测试套件](https://github.com/fsprojects/FSharpPlus/blob/master/tests/FSharpPlus.Tests/Free.fs)中找到，该套件使用 Free 和 Coproduct 类型进行了简化。
- Scott Wlaschin 的 [13 种观察乌龟的方法](https://fsharpforfunandprofit.com/posts/13-ways-of-looking-at-a-turtle)也是一个系列，它最终定义了一个 Free Monad，没有使用 F#+，而是使用样板代码。



## NonEmptyList<'T>

https://fsprojects.github.io/FSharpPlus/type-nonempty.html

至少包含一个元素的类型安全列表。

### 示例

```F#
#r @"nuget: FSharpPlus"
open FSharpPlus
open FSharpPlus.Data
```

### 构造 NonEmptyList

```F#
// you can construct a NonEmptyList by using ofSeq
let list123' = NonEmptyList.create 1 [ 2; 3 ]
// or more idiomatically
let list123 = nelist { 1 ; 2; 3 } // will work in F# version 4.7

let listOne = NonEmptyList.singleton 1
// cons
let list2 = NonEmptyList.cons 100 list123
// append two NonEmptyLists
let list3 = plus list2 (NonEmptyList.singleton 200)
// this can be written as (since list2 is a NonEmptyList):
let list3' = plus list2 (result 200)
// in order to get back to a regular list you can then use toList:
let list4 = toList list3'
```

### NonEmptyList 操作符

```F#
let lengthOfList3 = length list3

let headOf3 = list3.Head
let headOf3' = head list3

let tailOf3 = list3.Tail
```



## ParallelArray<'T>

https://fsprojects.github.io/FSharpPlus/type-parallelarray.html

这种类型基本上是数组的包装器，它：

- 有一个类似 ZipList 的应用程序实现。
- 默认情况下具有并行处理语义。

### 示例

```F#
#r @"nuget: FSharpPlus"
open FSharpPlus
open FSharpPlus.Data

let arr1 = [| 1..100000|]
let arr2 = [|10..100000|]

let arr1_plus_arr2  = (+) <!> parray arr1 <*> parray arr2

open FSharpPlus.Math.Applicative

let arr1_plus_arr2' = parray arr1 .+. parray arr2
let arrCombined     = 10 *. parray arr1 .+. parray arr2 .- 5
let asMonoid        = Infinite "Hello " </plus/> parray [|"City"; "World"; "Sun"|]
```



## Reader<'R, 'T>

https://fsprojects.github.io/FSharpPlus/type-reader.html

Reader monad 适用于从共享环境中读取值的计算。

### 相关类型

- [State](https://fsprojects.github.io/FSharpPlus/type-state.html)：类似，但它允许您修改环境。

### 示例

```F#
#r @"nuget: FSharpPlus"
```

Reader monad 的一种用法是替代依赖注入或柯里化，以传递依赖关系。以下代码来自 [F# Online - Josef Starýchfojtů - FSharpPlus - Advanced FP concepts in F#](https://www.youtube.com/watch?v=pxJCHJgG8ws)。你可以在 github 上找到演讲者 [@starychfojtu](https://github.com/starychfojtu)。

你为什么想做这种风格？

- 当你想传递一个环境而不是使用依赖注入时。

你为什么不想用这种风格？

- 这种风格的缺点是，它假设你的环境相对不可变。如果不同的实现类有不同的生命周期，依赖注入框架可能更容易使用。

注：

-  `(env : #IUserRepository)`  中的 # 是一个[灵活的类型注释](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/flexible-types)。

```F#
open System
open FSharpPlus
open FSharpPlus.Data

type IUserRepository =
    abstract GetUser : email : string -> string

type IShoppingListRepository =
    abstract AddToCart : shoppingList : string list -> string

let getUser email =
    Reader(fun (env : #IUserRepository) -> env.GetUser email)

let addToShoppingList shoppingListItems =
    Reader(fun (env : #IShoppingListRepository) -> env.AddToCart shoppingListItems)

let addShoppingListM email = monad {
    let! user = getUser email
    // 
    let shoppingListItems = ["Apple"; "Pear";]
    return! addToShoppingList shoppingListItems
}

type MockDataEnv() = // This is how an environment could be constructed
    interface IUserRepository with
        member this.GetUser email =
                "Sandeep"
    interface IShoppingListRepository with
            member this.AddToCart shoppingListItems =
                sprintf "Added the following items %A to the cart" shoppingListItems

Reader.run (addShoppingListM "sandeep@test.com")  (MockDataEnv())
```

[Haskell Wiki 上的 Reader monad](https://wiki.haskell.org/All_About_Monads#The_Reader_monad) 示例

```F#
open System
open FSharpPlus
open FSharpPlus.Data

/// This the abstract syntax representation of a template
type Template =
    /// Text
    | T of string
    /// Variable
    | V of Template
    /// Quote
    | Q of Template
    /// Include
    | I of Template*(Definition list)
    /// Compound
    | C of Template list
and Definition = | D of Template*Template

/// Our environment consists of an association list of named templates and
/// an association list of named variable values.
type Environment = {templates: Map<string,Template>
                    variables: Map<string,string>}

/// lookup a variable from the environment
let lookupVar (name:string) (env:Environment) : string option = tryItem name env.variables

/// lookup a template from the environment
let lookupTemplate (name:string) (env:Environment) : Template option = tryItem name env.templates

/// add a list of resolved definitions to the environment
let addDefs (defs:(string*string) list) env = { env with variables = plus (Map.ofList defs) env.variables}

/// resolve a template into a string
let rec resolve : Template -> Reader<Environment,string>  = function 
                       | T s -> result s
                       | V t -> monad {
                                   let! varName = resolve t
                                   let! env = ask
                                   let varValue = lookupVar varName env
                                   return option id "" varValue }
                        | Q t -> monad {
                                   let! tmplName = resolve t
                                   let! env = ask
                                   let body = lookupTemplate tmplName env
                                   return option string "" body }
                        | I (t,ds) -> monad {
                                    let! tmplName = resolve t
                                    let! env = ask
                                    let body = lookupTemplate tmplName env
                                    match body with
                                    | Some t' ->
                                                let! defs = List.traverse resolveDef ds
                                                return! local (addDefs defs) (resolve t')
                                    | None -> return ""
                                    }
                        | C ts   -> monad {
                                      let! resolved = List.traverse resolve ts
                                      return String.Concat<string> resolved
                                    }
and
   /// resolve a Definition and produce a (name,value) pair
   resolveDef: Definition -> Reader<Environment,string*string> = 
                                      function 
                                      | D (t,d) -> monad {
                                        let! name = resolve t
                                        let! value = resolve d
                                        return (name,value) }
```

### 推荐阅读

- 强烈推荐 Matt Thornton 的博客 [Grokking the Reader Monad](https://dev.to/choc13/grokking-the-reader-monad-4f45)。它包含使用 F#+ 的示例和从头开始的解释。



## `SeqT<Monad<bool>, 'T>`

https://fsprojects.github.io/FSharpPlus/type-seqt.html

这是 `seq<'T>` 的 Monad Transformer，因此它通过用 `seq<'T>` 组合现有的单子来为它们添加测序。

任何 monad 都可以组合，但一个非常典型的用法是与 `Async` 或 `Task` 结合使用，这会产生所谓的异步序列。

因此，[AsyncSeq](https://github.com/fsprojects/FSharp.Control.AsyncSeq) 库可以被认为是 Async 中此 monad 的专门化。

AsyncSeq 的原始帖子可以在[这里](http://tomasp.net/blog/async-sequences.aspx)找到，我们可以通过调整代码用 `SeqT` 运行这些示例。

为了做到这一点，我们需要意识到这两种实现的设计差异。

| **AsyncSeq**                  | **SeqT**                             | **注意事项**                                                 |
| ----------------------------- | ------------------------------------ | ------------------------------------------------------------ |
| `AsyncSeq<'T>`                | `SeqT<Async<bool>, 'T>`              |                                                              |
| `asyncSeq { .. }`             | `monad.plus { .. }`                  | 在某些时候，它需要被推断为 `SeqT<Async<bool>, 'T>`，或者可以用类型参数指定： `monad<SeqT<Async<bool>, 'T>>.plus` |
| `let! x = y`                  | `let! x = SeqT.lift y`               | 没有自动提升。应显式提升。                                   |
| `do! x`                       | `do! SeqT.lift x`                    | ''                                                           |
| `for x in s`                  | `let! x = s`                         | 除非是 `s: SeqT` 否则 `for` 仍然对普通序列是可以的           |
| `AsyncSeq.[function]`         | `SeqT.[function]`                    | 请参阅下面的函数差异。                                       |
| `AsyncSeq.[function]Async`    | `SeqT.[function]M`                   | ''                                                           |
| `AsyncSeq.skip`               | `SeqT.drop`                          | `.skip` 是可用的，但与 F# 集合一致，当序列没有足够的元素时，它会抛出。 |
| `AsyncSeq.take`               | `SeqT.truncate`                      | `.take` 是可用的，但与 F# 集合一致，当序列没有足够的元素时，它会抛出。 |
| `AsyncSeq.toBlockingSequence` | `SeqT.run >> Async.RunSynchronously` | 虽然不完全相同，但在语义上是等价的。                         |
| `AsyncSeq.toListAsync`        | `SeqT.runAsList`                     |                                                              |
| `AsyncSeq.toArrayAsync`       | `SeqT.runAsArray`                    |                                                              |
| `AsyncSeq.zipWith`            | `SeqT.map2`                          | 与 F# 集合一致。                                             |
| `AsyncSeq.zipWithAsync`       | `SeqT.map2M`                         | ''                                                           |
| `AsyncSeq.ofObservable`       | `Observable.toAsyncSeq`              | `.toTaskSeq` 也可用                                          |
| `AsyncSeq.toObservable`       | `Observable.ofAsyncSeq`              | `.ofTaskSeq` 也可用                                          |

### 示例

```F#
#r "nuget: FSharpPlus,1.3.0-CI02744" // still as pre-release

#r @"../../src/FSharpPlus/bin/Release/netstandard2.0/FSharpPlus.dll"
open System
open System.Net
open FSharpPlus
open FSharpPlus.Data

let urls =
  [ "http://bing.com"; "http://yahoo.com";
    "http://google.com"; "http://msn.com"; ]

// Asynchronous sequence that returns URLs and lengths
// of the downloaded HTML. Web pages from a given list
// are downloaded asynchronously in sequence.
let pages: SeqT<_, _> = monad.plus {
    use wc = new WebClient ()
    for url in urls do
        try
            let! html = wc.AsyncDownloadString (Uri url) |> SeqT.lift
            yield url, html.Length
        with _ ->
            yield url, -1 }


// Print URL of pages that are smaller than 100k
let printPages =
    pages
    |> SeqT.filter (fun (_, len) -> len < 100000)
    |> SeqT.map fst
    |> SeqT.iter (printfn "%s")
 
printPages |> Async.Start
```

上面和下面的这些示例来自[原始 AsyncSeq 帖子](http://tomasp.net/blog/async-sequences.aspx)，它们可以很容易地切换到任务序列（taskSeq），只需在 `wc.AsyncDownloadString (Uri url)` 和 `|> SeqT.lift` 之间添加 `|> Async.StartAsTask` 即可。然后运行除 `printPages |> Async.Start` 之外的所有内容。

```F#
// A simple webcrawler

#r "nuget: FSharpPlus,1.3.0-CI02744"
#r "nuget: HtmlAgilityPack"

open System
open System.Net
open System.Text.RegularExpressions
open HtmlAgilityPack
open FSharp.Control

open FSharpPlus
open FSharpPlus.Data

// ----------------------------------------------------------------------------
// Helper functions for downloading documents, extracting links etc.

/// Asynchronously download the document and parse the HTML
let downloadDocument url = async {
  try let wc = new WebClient ()
      let! html = wc.AsyncDownloadString (Uri url)
      let doc = new HtmlDocument ()
      doc.LoadHtml html
      return Some doc 
  with _ -> return None }

/// Extract all links from the document that start with "http://"
let extractLinks (doc:HtmlDocument) = 
  try
    [ for a in doc.DocumentNode.SelectNodes ("//a") do
        if a.Attributes.Contains "href" then
          let href = a.Attributes.["href"].Value
          if href.StartsWith "https://" then
            let endl = href.IndexOf '?'
            yield if endl > 0 then href.Substring(0, endl) else href ]
  with _ -> []

/// Extract the <title> of the web page
let getTitle (doc: HtmlDocument) =
  let title = doc.DocumentNode.SelectSingleNode "//title"
  if title <> null then title.InnerText.Trim () else "Untitled"

// ----------------------------------------------------------------------------
// Basic crawling - crawl web pages and follow just one link from every page

/// Crawl the internet starting from the specified page
/// From each page follow the first not-yet-visited page
let rec randomCrawl url = 
  let visited = new System.Collections.Generic.HashSet<_> ()

  // Visits page and then recursively visits all referenced pages
  let rec loop url = monad.plus {
    if visited.Add(url) then
      let! doc = downloadDocument url |> SeqT.lift
      match doc with 
      | Some doc ->
          // Yield url and title as the next element
          yield url, getTitle doc
          // For every link, yield all referenced pages too
          for link in extractLinks doc do
            yield! loop link 
      | _ -> () }
  loop url

// Use SeqT combinators to print the titles of the first 10
// web sites that are from other domains than en.wikipedia.org
randomCrawl "https://en.wikipedia.org/wiki/Main_Page"
|> SeqT.filter (fun (url, title) -> url.Contains "en.wikipedia.org" |> not)
|> SeqT.map snd
|> SeqT.take 10
|> SeqT.iter (printfn "%s")
|> Async.Start
```



## State<'S, 'T>

https://fsprojects.github.io/FSharpPlus/type-state.html

使用 State monad 的目的是以纯粹的函数式方式保持状态，而不违反函数的引用透明度。

### 相关类型

- [Reader](https://fsprojects.github.io/FSharpPlus/type-reader.html)：类似但只读。

### 示例

```F#
#r @"nuget: FSharpPlus"
open FSharpPlus
open FSharpPlus.Data
```

来自 [Haskell Wiki 上的 State monad](https://wiki.haskell.org/State_Monad)

```F#
let rec playGame =
    function
    | []-> monad {
            let! (_, score) = State.get
            return score
        }
    | x::xs-> monad {
            let! (on, score) = State.get
            match x with
            | 'a' when on -> do! State.put (on, score + 1)
            | 'b' when on -> do! State.put (on, score - 1)
            | 'c'         -> do! State.put (not on, score)
            | _           -> do! State.put (on, score)
            return! playGame xs
        }

let startState = (false, 0)
let moves = toList "abcaaacbbcabbab"
State.eval (playGame moves) startState
let (score, finalState) = State.run (playGame moves) startState
```



## Validation<'Error, 'T>

https://fsprojects.github.io/FSharpPlus/type-validation.html

这类似于 Result<'T, 'Error>，但具有累积错误语义，而不是短路。

### 示例

```F#
#r @"nuget: FSharpPlus"
open System
open FSharpPlus
open FSharpPlus.Data

module MovieValidations =
    type VError =
        | MustNotBeEmpty
        | MustBeAtLessThanChars of int
        | MustBeADate
        | MustBeOlderThan of int
        | MustBeWithingRange of decimal * decimal

    module String =
        let nonEmpty (x:string) : Validation<VError list, string> =
            if String.IsNullOrEmpty x
            then Failure [MustNotBeEmpty]
            else Success x
        let mustBeLessThan (i: int) (x: string) : Validation<VError list, string> =
            if isNull x || x.Length > i
            then Failure [MustBeAtLessThanChars i]
            else Success x

    module Number =
        let mustBeWithin (from, to') x =
            if from<= x && x <= to'
            then Success x
            else Failure [MustBeWithingRange (from, to')]
    
    module DateTime =
        let classicMovie year (d: DateTime) =
            if d.Year < year
            then Success d
            else Failure [MustBeOlderThan year]
        let date (d: DateTime) =
            if d.Date = d
            then Success d
            else Failure [MustBeADate]
    
    type Genre =
        | Classic
        | PostClassic
        | Modern
        | PostModern
        | Contemporary
    
    type Movie = {
        Id: int
        Title: String
        ReleaseDate: DateTime
        Description: String
        Price: decimal
        Genre: Genre
    } with
        static member Create (id, title, releaseDate, description, price, genre) : Validation<VError list, Movie> =
            fun title releaseDate description price -> { Id = id; Title = title; ReleaseDate = releaseDate; Description = description; Price = price; Genre = genre }
            <!> String.nonEmpty title <* String.mustBeLessThan 100 title
            <*> DateTime.classicMovie 1960 releaseDate <* DateTime.date releaseDate
            <*> String.nonEmpty description <* String.mustBeLessThan 1000 description
            <*> Number.mustBeWithin (0.0m, 999.99m) price

    let newRelease = Movie.Create (1, "Midsommar", DateTime (2019, 6, 24), "Midsommar is a 2019 folk horror film written...", 1m, Classic) //Failure [MustBeOlderThan 1960]
    let oldie = Movie.Create (2, "Modern Times", DateTime (1936, 2, 5), "Modern Times is a 1936 American comedy film...", 1m, Classic) // Success..
    let titleToLong = Movie.Create (3, String.Concat (seq { 1..110 }), DateTime (1950, 1, 1), "11", 1m, Classic) //Failure [MustBeAtLessThanChars 100]


module Person =

    type Name = { unName: String }
    with static member create s = {unName = s }

    type Email = { unEmail: String } 
    with static member create s = { unEmail = s }

    type Age = { unAge : int }
    with static member create i = { unAge = i }

    type Person = {
        name: Name
        email: Email
        age: Age }
    with static member create name email age = { name = name; email = email; age = age }


    type Error = 
        | NameBetween1And50
        | EmailMustContainAtChar
        | AgeBetween0and120

    // Smart constructors
    let mkName s =
        let l = length s
        if (l >= 1 && l <= 50)
        then Success <| Name.create s
        else Failure  [NameBetween1And50]

    let mkEmail s =
        if String.contains '@' s
        then Success <| Email.create s
        else Failure [EmailMustContainAtChar]

    let mkAge a =
        if (a >= 0 && a <= 120)
        then Success <| Age.create a
        else Failure [AgeBetween0and120]

    let mkPerson pName pEmail pAge =
        Person.create
        <!> mkName pName
        <*> mkEmail pEmail
        <*> mkAge pAge

    // Examples

    let validPerson = mkPerson "Bob" "bob@gmail.com" 25
    // Success ({name = {unName = "Bob"}; email = {unEmail = "bob@gmail.com"}; age = {unAge = 25}})

    let badName = mkPerson "" "bob@gmail.com" 25
    // Failure [NameBetween1And50]

    let badEmail = mkPerson "Bob" "bademail" 25
    // Failure [EmailMustContainAtChar]

    let badAge = mkPerson "Bob" "bob@gmail.com" 150
    // Failure [AgeBetween0and120]

    let badEverything = mkPerson "" "bademail" 150
    // Failure [NameBetween1And50;EmailMustContainAtChar;AgeBetween0and120]

    open FSharpPlus.Lens
    let asMaybeGood = validPerson ^? Validation._Success
    // Some ({name = {unName = "Bob"}; email = {unEmail = "bob@gmail.com"}; age = {unAge = 25}})
    let asMaybeBad = badEverything ^? Validation._Success
    // None

    let asResultGood = validPerson ^. Validation.isoValidationResult
    // Ok ({name = {unName = "Bob"}; email = {unEmail = "bob@gmail.com"}; age = {unAge = 25}})

    let asResultBad = badEverything ^. Validation.isoValidationResult
    // Error [NameBetween1And50;EmailMustContainAtChar;AgeBetween0and120]


module Email =

    // ***** Types *****
    type AtString = AtString of string
    type PeriodString = PeriodString of string
    type NonEmptyString = NonEmptyString of string

    type Email = Email of string

    type VError =
        | MustNotBeEmpty
        | MustContainAt
        | MustContainPeriod

    // ***** Base smart constructors *****
    // String must contain an '@' character
    let atString (x: string) : Validation<VError list, AtString> =
        if String.contains '@' x then Success <| AtString x
        else Failure [MustContainAt]

    // String must contain an '.' character
    let periodString (x: string) : Validation<VError list, PeriodString> =
        if String.contains '.' x
        then Success <| PeriodString x
        else Failure [MustContainPeriod]

    // String must not be empty
    let nonEmptyString (x: string) : Validation<VError list, NonEmptyString> =
        if not <| String.IsNullOrEmpty x
        then Success <| NonEmptyString x
        else Failure [MustNotBeEmpty]

    // ***** Combining smart constructors *****
    let email (x: string) : Validation<VError list, Email> =
        result (Email x) <*
        nonEmptyString x <*
        atString       x <*
        periodString   x

    // ***** Example usage *****
    let success = email "bob@gmail.com"

    // Success (Email "bob@gmail.com")

    let failureAt = email "bobgmail.com"
    // Failure [MustContainAt]

    let failurePeriod = email "bob@gmailcom"
    // Failure [MustContainPeriod]


    let failureAll = email ""
    // Failure [MustNotBeEmpty;MustContainAt;MustContainPeriod]
```

### 推荐阅读

- 强烈推荐 Matt Thornton 的博客 [Grokking Appliive Validation](https://dev.to/choc13/grokking-applicative-validation-lh6)。它包含使用 F#+ 的示例和从头开始的解释。



## Writer<'Monoid, 'T>

https://fsprojects.github.io/FSharpPlus/type-writer.html

Writer monad 是引入计算日志的好方法。它为您提供了一种不同的日志记录方式，当您希望能够检查记录的结果时，这种方式非常有用。

### 示例

```F#
#r @"nuget: FSharpPlus"
open FSharpPlus
open FSharpPlus.Data
type LogEntry={msg:string}
with
    static member create x = {msg = x}

let output x =  Writer.tell [LogEntry.create x]

let calc = monad {
  do! output "I'm going to start a heavy computation" // start logging
  let y = sum [1..100_000]
  do! output (string y)
  do! output "The computation finished"
  return y // return the result of the computation
}

let logs = Writer.exec calc
let (y,logs') = Writer.run calc
```

使用常规列表会对性能产生一些影响，这就是为什么在这些场景中应该使用 DList

```F#
let output' x =  Writer.tell <| DList.ofSeq [LogEntry.create x]

let calc' = monad {
  do! output' "I'm going to start a heavy computation" // start logging
  let y = sum [1..100_000]
  do! output' (string y)
  do! output' "The computation finished"
  return y // return the result of the computation
}

let logs2 = Writer.exec calc'
let (y',logs2') = Writer.run calc'
```



## ZipList<'T>

https://fsprojects.github.io/FSharpPlus/type-ziplist.html

这是 seq<'T> 的包装器，它将其应用语义更改为逐点处理。

### 示例

```F#
#r @"nuget: FSharpPlus"
open FSharpPlus
open FSharpPlus.Data

let seq1 = seq { 1..100000}
let seq2 = seq {10..100000}

let seq1_plus_seq2  = (+) <!> ZipList seq1 <*> ZipList seq2

open FSharpPlus.Math.Applicative

let seq1_plus_seq2' = ZipList seq1 .+. ZipList seq2
let arrCombined     = 10 *. ZipList seq1 .+. ZipList seq2 .- 5
let asMonoid        = result "Hello " </plus/> ZipList ["City"; "World"; "Sun"]

// try ZipList.run {the results}
```



# 抽象

https://fsprojects.github.io/FSharpPlus/abstractions.html

下图说明了可以用这个库表示的一些常见 FP 抽象及其之间的关系。

点击抽象名称查看完整描述：

```mermaid
classDiagram
    Functor --|> Bifunctor

    Functor --|> Applicative
    Applicative --|> Alternative
    Applicative ..|> ZipApplicative
    ZipApplicative --|> Alternative
    Applicative ..|> Bitraversable
    Applicative ..|> Traversable
    Applicative --|> Monad

    Semigroup --|> Monoid
	Semigroup: (+) x y
    Monoid ..|> Monad
    Monoid ..|> Alternative
    Monoid ..|> Bifoldable
    Bifoldable --|> Bitraversable
    Monoid ..|> Foldable
    Foldable: toSeq x
    Foldable --|> Traversable

    Functor --|> Comonad
    Functor --|> Profunctor
    Contravariant --|> Profunctor
	Contravariant: contramap f x
    Profunctor --|> Arrow
    Category --|> Arrow

    class Functor {
    	map f x
    	()unzip x
    }
    class Bifunctor {
    	bimap f g x
    	first f x
    	second f x
    }
    class Applicative {
    	return x
    	（<*>） f x
    	()map f x
    	()lift2 f x y
    }
    class Alternative {
    	empty
    	（<|>） f x
    	()mfilter p x
    }
    class ZipApplicative {
		pur x
		（<.>） f x
		()map f x
		()map2 f x y
	}
	class Bitraversable {
		bitraverse f x
		bisequence x
	}
	class Traversable {
		traverse f x
		sequence x
	}
	class Monad {
		return x
		（>>=） x f
		()map f x
		()join x
	}
    class Monoid {
    	zero
    	（+） x y [Appends both monoids]
    	()Seq.sum x
	}
	class Bifoldable {
		bifoldMap f g x
		bifold f g z x
		bifoldBack f g x z
		bisum x
	}
	class Comonad {
		extract x
		（=>>） s g | extend s g
		()duplicate x
	}
    class Profunctor {
    	dimap f g x
    	lmap f x
    	rmap f x
    }
    class Category {
    	catId
    	（<<<） f g
    	(>>>) f g
    }
    class Arrow {
    	arr f
    	arrFirst f g
    	()arrSecond f g
    	(***) f g
    	(&&&) f g
    }
```

[这里](https://fsprojects.github.io/FSharpPlus/abstraction-misc.html)还有一些抽象概念。



## Functor

https://fsprojects.github.io/FSharpPlus/abstraction-functor.html

Functor 抽象用于可以映射的类型。___

### 最小完整定义

- `map f x`  /  `(|>>) x f`  /  `(<<|) f x`  /  `(<!>) f x`

```F#
static member Map (x: 'Functor<'T>, f: 'T -> 'U) : 'Functor<'U>
```

### 其他操作

- `unzip x`

```F#
static member Unzip (x: 'Functor<'T * 'U>) : 'Functor<'T> * 'Functor<'U>
```

### 规则

```F#
map id  =  id
map (f << g) = map f << map g
```

### 相关抽象

- [Applicative](https://fsprojects.github.io/FSharpPlus/abstraction-applicative.html)：应用子是一个函子，其 `map` 操作可以拆分为 `return` 和 `(<*>)` 操作，
- [Monad](https://fsprojects.github.io/FSharpPlus/abstraction-monad.html)：Monad 是一个带有额外 `Join` 操作的 functor，

### 具体实现

来自 F#

- `seq<'T>`
- `list<'T>`
- `array<'T>`
- `'T [,]`
- `'T [,,]`
- `'T [,,,]`
- `option<'T>`
- `voption<'T>`
- `IObservable<'T>`
- `Lazy<'T>`
- `Async<'T>`
- `Result<'T,'U>`
- `Choice<'T,'U>`
- `KeyValuePair<'Key, 'T>`
- `Map<'Key,'T>`
- `'Monoid * 'T`
- `'struct ('Monoid * 'T)`
- `Task<'T>`
- `ValueTask<'T>`
- `'R->'T`
- `Expr<'T>`
- `Dictionary<'Key, 'T>`
- `IDictionary<'Key,' T>`
- `IReadOnlyDictionary<'Key, 'T>`
- `ResizeArray<'T>`

来自 F#+

- [`Cont<'R, 'T>`](https://fsprojects.github.io/FSharpPlus/type-cont.html)
- [`ContT<'R, 'T>`](https://fsprojects.github.io/FSharpPlus/type-contt.html)
- [`Reader<'R, 'T>`](https://fsprojects.github.io/FSharpPlus/type-reader.html)
- [`ReaderT<'R, 'Monad<'T>>`](https://fsprojects.github.io/FSharpPlus/type-readert.html)
- [`Writer<'Monoid, 'T>`](https://fsprojects.github.io/FSharpPlus/type-writer.html)
- [`WriterT<'Monad<'T * 'Monoid>>`](https://fsprojects.github.io/FSharpPlus/type-writert.html)
- [`State<'S, 'T * 'S>`](https://fsprojects.github.io/FSharpPlus/type-state.html)
- [`StateT<'S, 'Monad<'T * 'S>>`](https://fsprojects.github.io/FSharpPlus/type-statet.html)
- [`OptionT<'Monad>`](https://fsprojects.github.io/FSharpPlus/type-optiont.html)
- [`ValueOptionT<'Monad>`](https://fsprojects.github.io/FSharpPlus/type-valueoptiont.html)
- [`SeqT<'Monad>`](https://fsprojects.github.io/FSharpPlus/type-seqt.html)
- [`ListT<'Monad>`](https://fsprojects.github.io/FSharpPlus/type-listt.html)
- [`ResultT<'Monad>`](https://fsprojects.github.io/FSharpPlus/type-resultt.html)
- [`ChoiceT<'Monad>`](https://fsprojects.github.io/FSharpPlus/type-choicet.html)
- [`Free<'Functor<'T>, 'T>`](https://fsprojects.github.io/FSharpPlus/type-free.html)
- [`NonEmptyList<'T>`](https://fsprojects.github.io/FSharpPlus/type-nonempty.html)
- [`NonEmptySet<'T>`](https://fsprojects.github.io/FSharpPlus/type-nonempty-set.html)
- [`NonEmptyMap<'Key, 'T>`](https://fsprojects.github.io/FSharpPlus/type-nonempty-map.html)
- [`Validation<'Error, 'T>`](https://fsprojects.github.io/FSharpPlus/type-validation.html)
- [`ZipList<'T>`](https://fsprojects.github.io/FSharpPlus/type-ziplist.html)
- [`ParallelArray<'T>`](https://fsprojects.github.io/FSharpPlus/type-parallelarray.html)
- [`Const<'C, 'T>`](https://fsprojects.github.io/FSharpPlus/type-const.html)
- [`Compose<'AlternativeF<'AlternativeG<'T>>>`](https://fsprojects.github.io/FSharpPlus/type-compose.html)
- [`DList<'T>`](https://fsprojects.github.io/FSharpPlus/type-dlist.html)
- [`Kleisli<'T, 'Monad<'U>>`](https://fsprojects.github.io/FSharpPlus/type-kleisli.html)
- [`Coproduct<'FunctorL<'T>, 'FunctorR<'T>>`](https://fsprojects.github.io/FSharpPlus/type-coproduct.html)
- [`Vector<'T, 'Dimension>`](https://fsprojects.github.io/FSharpPlus/type-vector.html)
- [`Matrix<'T, 'Rows, 'Columns>`](https://fsprojects.github.io/FSharpPlus/type-matrix.html)

限制： \- `string` - `StringBuilder` - `Set<'T>` - `IEnumerator<'T>` [建议另一个](https://github.com/fsprojects/FSharpPlus/issues/new)具体实现

### 示例

```F#
#r @"nuget: FSharpPlus"
open FSharpPlus
open FSharpPlus.Math.Generic

let getLine    = async { return System.Console.ReadLine () }
let putStrLn x = async { printfn "%s" x}
let print    x = async { printfn "%A" x}

// Test IO
let action = monad {
    do! putStrLn  "What is your first name?"
    let! fn = getLine
    do! putStrLn  ("Thanks, " + fn) 
    do! putStrLn  ("What is your last name?")
    let! ln = getLine
    let  fullname = fn + " " + ln
    do! putStrLn  ("Your full name is: " + fullname)
    return fullname }


// Test Functors
let times2,minus3 = (*) 2, (-)/> 3
let resSome1      = map minus3 (Some 4G)
let noValue       = map minus3 None
let lstTimes2     = map times2 [1;2;3;4]
let fTimes2minus3 = map minus3 times2
let res39         = fTimes2minus3 21G
let getChars      = map (fun (x: string) -> x.ToCharArray () |> Seq.toList) action
let quot7         = map ((+) 2) <@ 5 @>


// try -> runIO getChars ;;

// Define a type Tree
type Tree<'a> =
    | Tree of 'a * Tree<'a> * Tree<'a>
    | Leaf of 'a
    static member map f (t:Tree<'a>  )  =
        match t with
        | Leaf  x          -> Leaf (f x)
        | Tree (x, t1, t2) -> Tree (f x, Tree.map f t1, Tree.map f t2)

// add instance for Functor class
    static member Map (x: Tree<_>, f) = Tree.map f x

let myTree = Tree(6, Tree(2, Leaf 1, Leaf 3), Leaf 9)
let mappedTree = map fTimes2minus3 myTree



// An Applicative is automatically a Functor

type ZipList<'s> = ZipList of 's seq with
    static member Return (x: 'a) = ZipList (Seq.initInfinite (konst x))
    static member (<*>) (ZipList (f :seq<'a->'b>), ZipList x) = ZipList (Seq.zip f x |> Seq.map (fun (f, x) -> f x)) : ZipList<'b>

let mappedZipList = map string (ZipList [1; 2; 3])


// A Monad is automatically a Functor

type MyList<'s> = MyList of 's seq with
    static member Return (x:'a)     = MyList x
    static member (>>=)  (MyList x: MyList<'T>, f) = MyList (Seq.collect (f >> (fun (MyList x) -> x)) x)

let mappedMyList = map string (MyList [1; 2; 3])
```

### 推荐阅读

- 强烈推荐 Matt Thornton 的博客 [Grokking Functors](https://dev.to/choc13/grokking-functors-bla)。它包含使用 F#+ 的示例和从头开始的解释。



## Applicative

https://fsprojects.github.io/FSharpPlus/abstraction-applicative.html

一个带应用的 functor，提供嵌入纯表达式（`return`）的操作，以及对计算进行排序并组合结果（`<*>`）___

### 最小完整定义

- `return x`  /  `result x`
- `(<*>) f x`

```F#
static member Return (x: 'T) : 'Applicative<'T>
static member (<*>) (f: 'Applicative<'T -> 'U>, x: 'Applicative<'T>) : 'Applicative<'U>
```

注意：`return` 不能在计算表达式之外使用，请使用 `result`。

### 其他操作

- `lift2`

```F#
static member Lift2 (f: 'T1 -> 'T2 -> 'T, x1: 'Applicative<'T1>, x2: 'Applicative<'T2>) : 'Applicative<'T>
```

### 规则

```F#
result id <*> v = v
result (<<) <*> u <*> v <*> w = u <*> (v <*> w)
result f <*> result x = result (f x)
u <*> result y = result ((|>) y) <*> u
```

### 相关抽象

- [Functor](https://fsprojects.github.io/FSharpPlus/abstraction-functor.html)：应用子是一个 Functor，其 `map` 操作可以在 `return` 和（`<*>`）操作中拆分，
- [Monad](https://fsprojects.github.io/FSharpPlus/abstraction-monad.html)：Monad 是一个带有额外 `Join` 操作的 functor，

### 具体实现

来自 F#

- `seq<'T>`
- `list<'T>`
- `array<'T>`
- `'T [,]`
- `'T [,,]`
- `'T [,,,]`
- `option<'T>`
- `voption<'T>`
- `IObservable<'T>`
- `Lazy<'T>`
- `Async<'T>`
- `Result<'T, 'U>`
- `Choice<'T, 'U>`
- `KeyValuePair<'Key, 'T>`
- `'Monoid * 'T`
- `ValueTuple<'Monoid, 'T>`
- `Task<'T>`
- `ValueTask<'T>`
- `'R -> 'T`
- `Expr<'T>`
- `ResizeArray<'T>`

来自 F#+

- [`Identity<'T>`](https://fsprojects.github.io/FSharpPlus/type-identity.html)
- [`Cont<'R, 'T>`](https://fsprojects.github.io/FSharpPlus/type-cont.html)
- [`ContT<'R, 'T>`](https://fsprojects.github.io/FSharpPlus/type-contt.html)
- [`Reader<'R, 'T>`](https://fsprojects.github.io/FSharpPlus/type-reader.html)
- [`ReaderT<'R, 'Monad<'T>>`](https://fsprojects.github.io/FSharpPlus/type-readert.html)
- [`Writer<'Monoid, 'T>`](https://fsprojects.github.io/FSharpPlus/type-writer.html)
- [`WriterT<'Monad<'T * 'Monoid>>`](https://fsprojects.github.io/FSharpPlus/type-writert.html)
- [`State<'S, 'T * 'S>`](https://fsprojects.github.io/FSharpPlus/type-state.html)
- [`StateT<'S, 'Monad<'T * 'S>>`](https://fsprojects.github.io/FSharpPlus/type-statet.html)
- [`OptionT<'Monad>`](https://fsprojects.github.io/FSharpPlus/type-optiont.html)
- [`ValueOptionT<'Monad>`](https://fsprojects.github.io/FSharpPlus/type-valueoptiont.html)
- [`SeqT<'Monad>`](https://fsprojects.github.io/FSharpPlus/type-seqt.html)
- [`ListT<'Monad>`](https://fsprojects.github.io/FSharpPlus/type-listt.html)
- [`ResultT<'Monad>`](https://fsprojects.github.io/FSharpPlus/type-resultt.html)
- [`ChoiceT<'Monad>`](https://fsprojects.github.io/FSharpPlus/type-choicet.html)
- [`Free<'Functor<'T>, 'T>`](https://fsprojects.github.io/FSharpPlus/type-free.html)
- [`NonEmptyList<'T>`](https://fsprojects.github.io/FSharpPlus/type-nonempty.html)
- [`Validation<'Error, 'T>`](https://fsprojects.github.io/FSharpPlus/type-validation.html)
- [`ZipList<'T>`](https://fsprojects.github.io/FSharpPlus/type-ziplist.html)
- [`ParallelArray<'T>`](https://fsprojects.github.io/FSharpPlus/type-parallelarray.html)
- [`Const<'C, 'T>`](https://fsprojects.github.io/FSharpPlus/type-const.html)
- [`Compose<'Applicative1<'Applicative2<'T>>>`](https://fsprojects.github.io/FSharpPlus/type-compose.html)
- [`DList<'T>`](https://fsprojects.github.io/FSharpPlus/type-dlist.html)
- [`Vector<'T, 'Dimension>`](https://fsprojects.github.io/FSharpPlus/type-vector.html)
- [`Matrix<'T, 'Rows, 'Columns>`](https://fsprojects.github.io/FSharpPlus/type-matrix.html)

受限： \- `string` - `StringBuilder` - `Set<'T>` - `IEnumerator<'T>`

仅适用于 <*> 操作： \- `Map<'Key, 'T>` - `Dictionary<'Key, 'T>` - `IDictionary<'Key, 'T>` - `IReadOnlyDictionary<'Key, 'T>`

[建议另一个](https://github.com/fsprojects/FSharpPlus/issues/new)具体实现方案

### 示例

```F#
#r @"nuget: FSharpPlus"
open FSharpPlus
open FSharpPlus.Data

// Apply +4 to a list
let lst5n6  = map ((+) 4) [ 1; 2 ]

// Apply +4 to an array
let arr5n6  = map ((+) 4) [|1; 2|]

// I could have written this
let arr5n6' = (+) <!> [|4|] <*> [|1; 2|]

// Add two options
let opt120  = (+) <!> Some 20 <*> tryParse "100"


// Applicatives need Return (result)

// Test return
let resSome22 : option<_> = result 22
let resSing22 : list<_>   = result 22
let resLazy22 : Lazy<_>   = result 22
let (quot5 : Microsoft.FSharp.Quotations.Expr<int>) = result 5

// Example
type Person = { Name: string; Age: int } with static member create n a = { Name = n; Age = a }

let person1 = Person.create <!> tryHead ["gus"] <*> tryParse "42"
let person2 = Person.create <!> tryHead ["gus"] <*> tryParse "fourty two"
let person3 = Person.create <!> tryHead ["gus"] <*> (tryHead ["42"] >>= tryParse)


// Other ways to write applicative expressions


// Function lift2 helps in many cases

let person1' = (tryHead ["gus"], tryParse "42")               ||> lift2 Person.create 
let person2' = (tryHead ["gus"], tryParse "fourty two")       ||> lift2 Person.create 
let person3' = (tryHead ["gus"], tryHead ["42"] >>= tryParse) ||> lift2 Person.create 


// Using Idiom brackets from http://www.haskell.org/haskellwiki/Idiom_brackets

let res3n4   = iI ((+) 2) [1;2] Ii
let res3n4'  = iI (+) (result 2) [1;2] Ii
let res18n24 = iI (+) (ZipList(seq [8;4])) (ZipList(seq [10;20])) Ii

let tryDiv x y = if y = 0 then None else Some (x </div/> y)
let resSome3   = join (iI tryDiv (Some 6) (Some 2) Ii)
let resSome3'  =       iI tryDiv (Some 6) (Some 2) Ji

let tryDivBy y = if y = 0 then None else Some (fun x -> x </div/> y)
let resSome2  = join (result tryDivBy  <*> Some 4) <*> Some 8
let resSome2' = join (   iI tryDivBy (Some 4) Ii) <*> Some 8

let resSome2'' = iI tryDivBy (Some 4) J (Some 8) Ii
let resNone    = iI tryDivBy (Some 0) J (Some 8) Ii
let res16n17   = iI (+) (iI (+) (result 4) [2; 3] Ii) [10] Ii

let opt121  = iI (+) (Some 21) (tryParse "100") Ii
let opt122  = iI tryDiv (tryParse "488") (trySqrt 16) Ji


// Using applicative math operators

open FSharpPlus.Math.Applicative

let opt121'  = Some 21 .+. tryParse "100"
let optTrue  = 30 >. tryParse "29"
let optFalse = tryParse "30" .< 29
let m1m2m3 = -.[1; 2; 3]


// Using applicative computation expression

let getName s = tryHead s
let getAge  s = tryParse s

let person4 = applicative {
    let! name = getName ["gus"]
    and! age  = getAge "42"
    return { Name = name; Age = age } }
```

### 组合应用子

与单子不同，应用子总是可组合的。

日期类型 [`Compose<'Applicative1<'Applicative2<'T>>>`](https://fsprojects.github.io/FSharpPlus/type-compose.html) 可用于组合任何 2 个应用子：

```F#
let res4 = (+) <!> Compose [Some 3] <*> Compose [Some 1]

let getNameAsync s = async { return tryHead s }
let getAgeAsync  s = async { return tryParse s }

let person5 = Person.create <!> Compose (getNameAsync ["gus"]) <*> Compose (getAgeAsync "42")
```

计算表达式 application2 和 application3 也可用于组合应用子：

```F#
let person6 = applicative2 {
    let! name = printfn "aa"; getNameAsync ["gus"]
    and! age  = getAgeAsync "42"
    return { Name = name; Age = age } }




// A Monad is automatically an Applicative

type MyList<'s> = MyList of 's seq with
    static member Return (x: 'a) = MyList (Seq.singleton x)
    static member (>>=)  (MyList x: MyList<'T>, f) = MyList (Seq.collect (f >> (fun (MyList x) -> x)) x)

let mappedMyList : MyList<_> = (MyList [(+) 1; (+) 2; (+) 3]) <*> (MyList [1; 2; 3])
```

### 推荐阅读

- 强烈推荐 Matt Thornton 的博客 [Grokking Applicatives](https://dev.to/choc13/grokking-applicatives-44o1)。它包含使用 F#+ 的示例和从头开始的解释。



## Monad

https://fsprojects.github.io/FSharpPlus/abstraction-monad.html

定义单子上的基本操作，单子是一个来自叫范畴论的数学分支的概念。然而，从 F# 程序员的角度来看，最好将 monad 视为动作的抽象数据类型。F#+ 泛型计算表达式为编写单子性表达式提供了一种方便的语法。

### 最小完整定义

- `return x`  /  `result x`
- `(>>=) x f`

```F#
static member Return (x: 'T) : 'Monad<'T>
static member (>>=) (x: 'Monad<'T>, f: 'T -> 'Monad<'U>) : 'Monad<'U>
```

注意：`return` 不能在计算表达式之外使用，请使用 `result`。

### 其他操作

- `join`

```F#
static member Join (x: 'Monad<'Monad<'T>>) : 'Monad<'T>
```

### 规则

```F#
return a >>= k = k a
m >>= return = m
m >>= (fun x -> k x >>= h) = (m >>= k) >>= h
```

### 相关抽象

- [Functor](https://fsprojects.github.io/FSharpPlus/abstraction-functor.html)：单子自动成为函子。
- [Applicative](https://fsprojects.github.io/FSharpPlus/abstraction-applicative.html)：单子是自动成为应用子。

### 具体实现

来自 F#

- `seq<'T>`
- `list<'T>`
- `array<'T>`
- `option<'T>`
- `voption<'T>`
- `Lazy<'T>`
- `Async<'T>`
- `Result<'T,'U>`
- `Choice<'T,'U>`
- `'Monoid * 'T`
- `struct ('Monoid * 'T)`
- `Task<'T>`
- `ValueTask<'T>`
- `'R->'T`
- `ResizeArray<'T>`

来自 F#+

- [`Identity<'T>`](https://fsprojects.github.io/FSharpPlus/type-identity.html)
- [`Cont<'R,'T>`](https://fsprojects.github.io/FSharpPlus/type-cont.html)
- [`ContT<'R,'T>`](https://fsprojects.github.io/FSharpPlus/type-contt.html)
- [`Reader<'R,'T>`](https://fsprojects.github.io/FSharpPlus/type-reader.html)
- [`ReaderT<'R,'Monad<'T>>`](https://fsprojects.github.io/FSharpPlus/type-readert.html)
- [`Writer<'Monoid,'T>`](https://fsprojects.github.io/FSharpPlus/type-writer.html)
- [`WriterT<'Monad<'T * 'Monoid>>`](https://fsprojects.github.io/FSharpPlus/type-writert.html)
- [`State<'S,'T * 'S>`](https://fsprojects.github.io/FSharpPlus/type-state.html)
- [`StateT<'S,'Monad<'T * 'S>>`](https://fsprojects.github.io/FSharpPlus/type-statet.html)
- [`OptionT<'Monad>`](https://fsprojects.github.io/FSharpPlus/type-optiont.html)
- [`ValueOptionT<'Monad>`](https://fsprojects.github.io/FSharpPlus/type-valueoptiont.html)
- [`SeqT<'Monad>`](https://fsprojects.github.io/FSharpPlus/type-seqt.html)
- [`ListT<'Monad>`](https://fsprojects.github.io/FSharpPlus/type-listt.html)
- [`ResultT<'Monad>`](https://fsprojects.github.io/FSharpPlus/type-resultt.html)
- [`ChoiceT<'Monad>`](https://fsprojects.github.io/FSharpPlus/type-choicet.html)
- [`Free<'Functor<'T>,'T>`](https://fsprojects.github.io/FSharpPlus/type-free.html)
- [`NonEmptyList<'T>`](https://fsprojects.github.io/FSharpPlus/type-nonempty.html)
- [`DList<'T>`](https://fsprojects.github.io/FSharpPlus/type-dlist.html)

[建议另一个](https://github.com/fsprojects/FSharpPlus/issues/new)具体实现

### 示例

```F#

#r @"nuget: FSharpPlus"
open FSharpPlus
open FSharpPlus.Data


// Monads allow us to use our generic computation expressions

// This will return the list [11;21;12;22] which is both lists combined in different ways with the (+) operation
let lst11n21n12n22 =
    monad {
        let! x1 = [1;   2]
        let! x2 = [10; 20]
        return ((+) x1 x2) }

// This is the same example but with a non-empty list
let neLst11n21n12n22 = 
    monad {
        let! x1 = { NonEmptyList.Head =  1; Tail =  [2] }
        let! x2 = { NonEmptyList.Head = 10; Tail = [20] }
        return ((+) x1 x2)}

// And now an example with options
let some14 =
    monad {
        let! x1 = Some 4
        let! x2 = tryParse "10"
        return ((+) x1 x2) }



// MONAD TRANSFORMERS
// ==================
//
// Monads do not compose directly, we need to use Monad Transformers
let fn : ResultT<Reader<int,Result<_,string>>> = 
    monad {
       let! x1 = lift ask
       let! x2 = 
           if x1 > 0 then result 1
           else ResultT (result (Error "Negative value"))
       return x1 + x2
    }

let x = (fn |> ResultT.run |> Reader.run) 10
// Result<int,string> = Ok 11
let y = (fn |> ResultT.run |> Reader.run) -1
// Result<int,string> = Error "Negative value"
// The following example comes from Haskell
// async is used instead of IO

open System

// First let's define some functions we'll use later
let getLine    = async { return Console.ReadLine () }
let putStrLn x = async { printfn "%s" x }
let isValid s =
    String.length s >= 8
        && String.exists Char.IsLetter s
        && String.exists Char.IsNumber s
        && String.exists Char.IsPunctuation s

let decodeError = function
    | -1 -> "Password not valid"
    | _  -> "Unknown"


// Now the following functions compose the Error monad with the Async one.

let getValidPassword : ResultT<_> =
    monad {
        let! s = liftAsync getLine
        if isValid s then return s
        else return! throw -1}
    </catch/>
        (fun s -> throw ("The error was: " + decodeError s))

let askPassword = monad {
    do! lift <| putStrLn "Insert your new password:"
    let! value = getValidPassword
    //do! lift <| putStrLn "Storing in database..."
    return value}

//try -> Async.RunSynchronously (ResultT.run askPassword)


// After getting used to monadic CEs it's natural
// to feel the need to combine monads
// (from https://stackoverflow.com/a/37900264 )

module CombineWriterWithResult =
    
    let divide5By = function
        | 0.0 -> Error "Divide by zero"
        | x   -> Ok (5.0 / x)

    let eitherConv logSuccessF logFailF f v =
        ResultT <|
            match f v with
            | Ok a -> Writer(Ok a, ["Success: " + logSuccessF a])
            | Error b -> Writer(Error b, ["ERROR: "   + logFailF b])

    let ew = monad {
        let! x = eitherConv (sprintf "%f") (sprintf "%s") divide5By 6.0
        let! y = eitherConv (sprintf "%f") (sprintf "%s") divide5By 3.0
        let! z = eitherConv (sprintf "%f") (sprintf "%s") divide5By 0.0
        return (x, y, z) }

    let (_, log) = ew |> ResultT.run |> Writer.run


// You can also stack monad transformers.

// A monad transformer and a monad is itself a monad, so you can pass that into another monad transformer.
// For example, below we are stacking them like:
// type Example = ReaderT<DateTime, ResultT<Writer<string list, Result<string * string * string, string>>>>)

// Catch and throw is generic over all monad transformers in F#+ so catch works in this example
// because there is a Result in the stack. We use it here to consolidate Result's 'TError.

module CombineReaderWithWriterWithResult =

    let divide5By : float -> Result<float, string> = function
        | 0.0 -> Error "Divide by zero"
        | x   -> Ok (5.0 / x)

    let otherDivide5By : float -> Result<float, unit>  = function
        | 0.0 -> Error ()
        | x   -> Ok (5.0 / x)

    let eitherConv f v =
        ReaderT <| fun (now : System.DateTime) ->
        ResultT <|
            match f v with
            | Ok a    -> Writer(Ok a,    [sprintf "Success at %s: %A" (now.ToString "o") a])
            | Error b -> Writer(Error b, [sprintf "ERROR at %s: %A"   (now.ToString "o") b])

    let divide = monad {
        let! w = eitherConv divide5By       6.0
        let! x = eitherConv divide5By       3.0
        let! y = eitherConv divide5By       0.0
        let! z = eitherConv otherDivide5By  0.0 </catch/> (throw << (fun _ -> "Unknown error"))

        return (w, x, y, z) }

    let run expr = ReaderT.run expr >> ResultT.run >> Writer.run

    let (_, log) = run divide DateTime.UtcNow


// Many popular F# libraries are in fact an instantiation of a specific monad combination.
// The following example demonstrate how to code a mini-Suave lib in a few lines

module Suave =
    // setup something that reminds us of what Suave can work with
    // this is an overly simplified model of Suave in order to show how OptionT can be used 
    // in conjunction with generic Kleisli composition (fish) operator
    type WebPart<'a> = 'a -> OptionT<Async<'a option>>
    let inline succeed x = async.Return (Some x)

    module Http =
        type HttpResponse = { status: int; content: string }
        type HttpRequest  = { url: Uri; ``method``: string }
        type HttpContext  = { request: HttpRequest; response: HttpResponse }

    module Successful =
        open Http
        let private withStatusCode statusCode s =
            OptionT << fun ctx -> { ctx with response = { ctx.response with status = statusCode; content = s }} |> succeed
        let OK s = withStatusCode 200 s
        let BAD_REQUEST s = withStatusCode 400 s

    module Filters =
        open Http
        let ``method`` (m: string) =
            OptionT << fun (x: HttpContext) -> async.Return (if (m = x.request.``method``) then Some x else None)
        let GET  (x : HttpContext) = ``method`` "GET" x
        let POST (x : HttpContext) = ``method`` "POST" x
  
        let path s =
            OptionT << fun (x: HttpContext) -> async.Return (if (s = x.request.url.AbsolutePath) then Some x else None)

    // Stub implementations: here you can plug Fleece or another similar Json library
    let toJson o : string  = failwith "Not implemented"
    let ofJson (s: string) = failwith "Not implemented"

    module Request =
        let tryGet _s (_r: Http.HttpRequest) = Ok "FORM VALUE"

    let authenticated (f: Http.HttpContext -> int -> OptionT<Async<'a option>>) =
        // we assume that authenticated executes f only if auth, otherwise returns 401
        // we fake it as:
        fun (ctx: Http.HttpContext) -> f ctx -1

    // Usage:
    open Successful
    open Filters
    type Note = { id: int; text: string }
    type NoteList = { notes: Note list; offset: int; chunk: int; total: int }
    type IDb =
        abstract member getUserNotes: int -> Async<NoteList>
        abstract member addUserNote: int -> string -> Async<Note>
    type OverviewViewModel = { myNotes: Note list }
    let app (db: IDb) =
        let overview =
            GET >=> (authenticated <| fun ctx userId ->
                monad {
                  let! res = lift (db.getUserNotes userId)
                  let ovm = toJson { myNotes = res.notes }
                  return! OK ovm ctx
                })
        let register =
            POST >=> (authenticated <| fun ctx userId ->
                monad {
                  match ctx.request |> Request.tryGet "text" with 
                  | Ok text ->
                      let! newNote = lift (db.addUserNote userId text)
                      let rvm = toJson newNote
                      return! OK rvm ctx
                  | Error msg -> 
                      return! BAD_REQUEST msg ctx
                })
        choice [
            path "/" >=> (OK "/")
            path "/note" >=> register
            path "/notes" >=> overview
        ]
```

### 推荐阅读

- 强烈推荐 Matt Thornton 的博客：
  - [Grokking Monads](https://dev.to/choc13/grokking-monads-in-f-3j7f)
  - [Grokking Monads Imperatively](https://dev.to/choc13/grokking-monads-imperatively-394a)
  - [Grokking Monads Transformers](https://dev.to/choc13/grokking-monad-transformers-3l3)
  - 它包含使用 F#+ 的示例和从头开始的解释。



## Semigroup

https://fsprojects.github.io/FSharpPlus/abstraction-semigroup.html

在数学中，半群是一种代数结构，由集合和结合性二元运算组成。半群推广了一个幺半群，因为可能不存在恒等元。它还（最初）将一个群（一个具有所有逆的幺半群）推广到一个类型，其中每个元素都不必有逆，因此被称为半群。___

### 最小完整定义

- `(+)`  /  `(++)`

```F#
static member (+) (x: 'Semigroup, y: 'Semigroup) : 'Semigroup
```

### 规则

```F#
(x + y) + z = x + (y + z)
```

### 相关抽象

- [Monoid](https://fsprojects.github.io/FSharpPlus/abstraction-monoid.html)：Monoid 是一个带有额外 `zero` 运算的半群
- Alt/MonadPlus：也是半群/幺半群的应用子/单子

### 具体实现

来自 .Net/F#

- `list<'T>`
- `option<'T>`
- `voption<'T>`
- `array<'T>`
- `string`
- `StringBuilder`
- `unit`
- `Set<'T>`
- `Map<'T, 'U>`
- `TimeSpan`
- `Tuple<*>`
- `ValueTuple<*> ( * up to 7 elements)`
- `'T1* ... *'Tn`
- `Task<'T>`
- `ValueTask<'T>`
- `'T -> 'Semigroup`
- `Async<'T>`
- `Expr<'T>`
- `Lazy<'T>`
- `Dictionary<'T, 'U>`
- `IDictionary<'T, 'U>`
- `IReadOnlyDictionary<'T, 'U>`
- `ResizeArray<'T>`
- `seq<'T>`
- `IEnumerator<'T>`

来自 F#+

- [`NonEmptyList<'S>`](https://fsprojects.github.io/FSharpPlus/type-nonempty.html)
- [`NonEmptySet<'T>`](https://fsprojects.github.io/FSharpPlus/type-nonempty-set.html)
- [`NonEmptyMap<'Key, 'T>`](https://fsprojects.github.io/FSharpPlus/type-nonempty-map.html)
- [`ZipList<'S>`](https://fsprojects.github.io/FSharpPlus/type-ziplist.html)
- [`Dual<'T>`](https://fsprojects.github.io/FSharpPlus/type-dual.html)
- [`Endo<'T>`](https://fsprojects.github.io/FSharpPlus/type-endo.html)
- [`All`](https://fsprojects.github.io/FSharpPlus/type-all.html)
- [`Any`](https://fsprojects.github.io/FSharpPlus/type-any.html)
- [`Const<'C, 'T>`](https://fsprojects.github.io/FSharpPlus/type-const.html)
- [`First<'T>`](https://fsprojects.github.io/FSharpPlus/type-first.html)
- [`Last<'T>`](https://fsprojects.github.io/FSharpPlus/type-last.html)
- [`DList<'T>`](https://fsprojects.github.io/FSharpPlus/type-dlist.html)
- [`Vector<'T, 'Dimension>`](https://fsprojects.github.io/FSharpPlus/type-vector.html)
- [`Matrix<'T, 'Rows, 'Columns>`](https://fsprojects.github.io/FSharpPlus/type-matrix.html)

[建议另一个](https://github.com/fsprojects/FSharpPlus/issues/new)具体实现



## Monoid

https://fsprojects.github.io/FSharpPlus/abstraction-monoid.html

具有单位元的结合性二元操作的类型。

### 最小完整定义

- `zero`
- `(+) x y`/`(++) x y`

```F#
static member get_Zero () :'Monoid
static member (+) (x:'Monoid, y:'Monoid) :'Monoid
```

### 其他操作

- `Seq.sum`

```F#
static member Sum (x:Seq<'Monoid>) :'Monoid
```

### 规则

```F#
zero + x = x
x + zero = x
(x + y) + z = x + (y + z)
Seq.sum = Seq.fold (+) zero
sum = fold (+) zero (generic to all foldables)
```

### 相关抽象

- [Semigroup](https://fsprojects.github.io/FSharpPlus/abstraction-semigroup.html)：幺半群是一个带有额外 `zero` 运算的半群
- [Alternative / MonadPlus](https://fsprojects.github.io/FSharpPlus/abstraction-alternative.html)：也是幺半群的应用子/单子。尽管它们的幺半群定义可能不同。

### 具体实现

来自 .Net/F#

- `list<'T>`
- `option<'T>`
- `array<'T>`
- `string`
- `StringBuilder`
- `unit`
- `Set<'T>`
- `Map<'T,'Monoid>`
- `TimeSpan`
- `Tuple<'Monoid1* ... *'MonoidN>`
- `'Monoid1* ... *'MonoidN`
- `Task<'T>`
- `ValueTask<'T>`
- `'T->'Monoid`
- `Async<'T>`
- `Expr<'T>`
- `Lazy<'T>`
- `Dictionary<'T,'Monoid>`
- `IDictionary<'T,'Monoid>`
- `IReadOnlyDictionary<'T,'Monoid>`
- `ResizeArray<'T>`
- `seq<'T>`
- `IEnumerator<'T>`

来自 F#+

- [`ZipList<'T>`](https://fsprojects.github.io/FSharpPlus/type-ziplist.html)
- [`Dual<'T>`](https://fsprojects.github.io/FSharpPlus/type-dual.html)
- [`Endo<'T>`](https://fsprojects.github.io/FSharpPlus/type-endo.html)
- [`All`](https://fsprojects.github.io/FSharpPlus/type-all.html)
- [`Any`](https://fsprojects.github.io/FSharpPlus/type-any.html)
- [`Const<'T,'U>`](https://fsprojects.github.io/FSharpPlus/type-const.html)
- [`First<'T>`](https://fsprojects.github.io/FSharpPlus/type-first.html)
- [`Last<'T>`](https://fsprojects.github.io/FSharpPlus/type-last.html)
- [`DList<'T>`](https://fsprojects.github.io/FSharpPlus/type-dlist.html)
- [`Vector<'T,'Dimension>`](https://fsprojects.github.io/FSharpPlus/type-vector.html)
- [`Matrix<'T,'Rows,'Columns>`](https://fsprojects.github.io/FSharpPlus/type-matrix.html)

[建议另一个](https://github.com/fsprojects/FSharpPlus/issues/new)具体实现

### 示例

```F#
#r @"nuget: FSharpPlus"
open FSharpPlus
open FSharpPlus.Math.Generic
open FSharpPlus.Data


/// A monoid that represents results of comparisons
type Ordering = LT|EQ|GT with
    static member        Zero = EQ
    static member        (+) (x:Ordering, y) = 
        match x, y with
        | LT, _ -> LT
        | EQ, a -> a
        | GT, _ -> GT

let inline compare' x y =
    match compare x y with
    | a when a > 0 -> GT
    | a when a < 0 -> LT
    | _            -> EQ

let resGreater = compare' 7 6

/// A monoid of all numbers from 0 to 4
type Mod5 = Mod5 of uint32 with
    static member inline get_Zero() = Mod5 0u
    static member inline (+) (Mod5 x, Mod5 y) = Mod5 ( (x + y) % 5u)
let Mod5 x = Mod5 (x % 5u)


// Results of Monoid operations
let emptyLst:list<int> = zero
let zeroUint:Mod5   = zero
let res1 = zero ++ Mod5 11u
let res2  = sum <| map Mod5 [4u; 2u; 1u]
let res3  = sum [zero; Mod5 2G; Mod5 6G]
let res8n4 = [zero; [8;4]]
let res15 = Mult 15 ++ zero
let resTrue = sum [zero; Any true]
let resFalse = sum (map All [true;false])
let resHi = zero ++ "Hi"
let resGT = zero ++  GT
let resLT = sum [zero; LT ; EQ ;GT]
let res9823 = sum (map Dual [zero;"3";"2";"8";"9"])
let resBA = Dual "A" ++ Dual "B" 
let resEl00:list<int>*float = zero
let resS3P20     = (1G, Mult 5.0) ++  (2, Mult 4G)
let res230       = (zero,zero) ++ ([2],[3.0])
let res243       = ([2;4],[3]) ++ zero
let res23        = zero ++ ([2],"3")
let resLtDualGt  =  (LT,Dual GT) ++ zero
let res230hiSum2 = (zero, zero, 2) ++ ([2], ([3.0], "hi"), zero)
let res230hiS4P3 = (zero, zero   ) ++ ([2], ([3.0], "hi", 4, Mult (6 % 2)))
let tuple5 :string*(Any*string)*(All*All*All)*int*string = zero
```



# 其他抽象概念

https://fsprojects.github.io/FSharpPlus/abstraction-misc.html

这里有一些其他抽象概念，图中没有。

（参见示例）

## 示例

```F#
#r @"../../src/FSharpPlus/bin/Release/netstandard2.0/FSharpPlus.dll"

open System
open FSharpPlus
open FSharpPlus.Data


// Indexable

let namesWithNdx = mapi (fun k v -> "(" + string k + ")" + v ) (Map.ofSeq ['f',"Fred";'p',"Paul"])
let namesAction = iteri (printfn "(%A)%s") (Map.ofSeq ['f',"Fred";'p',"Paul"])
let res119 = foldi (fun s i t -> t * s - i) 10 [3;4]
let res113 = foldi (fun s i t -> t * s - i) 2 [|3;4;5|]
let resSomeId20 = traversei (fun k t -> Some (10 + t)) (Tuple 10)


// Collection

let a = skip 3 [1..10]
let b = chunkBy fst [1, "a"; 1, "b"; 2, "c"; 1, "d"]


// Reducibles

let c = nelist {1; 2; 3}
let d = reduce (+) c

let resultList = nelist {Error "1"; Error "2"; Ok 3; Ok 4; Error "5"}
let firstOk = choice resultList


// Invariant Functor
type StringConverter<'t> = StringConverter of (string -> 't) * ('t -> string) with
    static member Invmap (StringConverter (f, g), f',g') = StringConverter (f' << f, g << g')

let ofString (StringConverter (f, _)) = f
let toString (StringConverter (_, f)) = f

let floatConv = StringConverter (float<string>, string<float>)

let floatParsed  = ofString floatConv "1.8"
let floatEncoded = toString floatConv 1.5

let intConv = invmap int<float> float<int> floatConv

let oneParsed  = ofString intConv "1"
let tenEncoded = toString intConv 10
```



# 计算表达式

https://fsprojects.github.io/FSharpPlus/computation-expressions.html

这个库允许使用一些常见的计算表达式，而无需编写任何样板代码。

对于[应用子（Applicatives）](https://fsprojects.github.io/FSharpPlus/abstraction-applicative.html)，只有一个计算表达式： `applicative { .. }`。此外，对于组合（也称为分层）应用子，存在 `applicative2 { .. }` 和 `applicative3 { .. }`。

对于 [ZipApplicatives](https://fsprojects.github.io/FSharpPlus/abstraction-zipapplicative.html)，有一组对应的计算表达式： `applicative' { .. }`，`applicative2' { .. }` 和 `applicative3' { .. }`。

对于[单子性的（monadic）](https://fsprojects.github.io/FSharpPlus/abstraction-monad.html)代码，只有一个计算表达式： `monad { .. }` ，但它有4种风格：

- 延迟或严格

  延迟计算要求该类型实现 TryWith、TryFinally 和可选的 Delay 方法。F# 带有 async 和 seq 计算表达式，两者都有延迟。

- 它可能具有嵌入的副作用或充当 monadplus

  一个 monadplus 可以多次返回（或产生），因此例如循环中的所有表达式都可以返回，而在另一个模型中，这些表达式的类型是 unit，因为预期会有副作用。

  异步工作流是副作用计算表达式的示例，seq 表达式是 monadplus 的示例。

  副作用工作流对类型没有任何额外要求（除了 monad 操作），但 monadplus 需要额外的 [get_Empty 和 (<|>)](https://fsprojects.github.io/FSharpPlus/abstraction-alternative.html)方法。

  泛型计算表达式 `monad` 是一个副作用表达式，但可以通过访问 `.plus` 属性将其转换为 monadplus。请注意，`monad.fx` 是 `monad` 的别名：fx 是副作用的缩写。

  默认情况下，这些计算是懒惰的，但可以通过添加 `.strict` 或使用 `'`，即 `monad.plus'` 来使它们变得严格。

换言之：

- `monad.fx` 或简称 `monad`：懒惰的单子构建器。当你想使用副作用而不是 monadplus 的加法行为时使用。
- `monad.fx.strict`（或 `monad.fx'`，或简称 `monad.strict` 或 `monad'`）是 `monad` 的严格版本。
- `monad.plus`：懒惰的加法单子构建器。当你期待一个或多个结果时使用。
- `monad.plus'` 是 `monad.plus` 的严格版本

请注意，一个类型要么是惰性的，要么是严格的，但它可以同时充当 fx 或 plus（见下面的一些示例）。这意味着，当我们在一个类型上使用 CE 时，我们需要注意，如果该类型是懒惰的，但使用严格的 monad，我们会得到严格的语义，这可能没有意义，但如果我们做相反的事情，我们可能会遇到运行时错误，幸运的是，编译时警告（或错误）会阻止我们。

判断一个类型是严格型还是懒惰型的一个简单方法是在 fsi 中执行此操作: `let _ : MyType<'t> = monad { printfn "I'm strict" }`

对于分层单子（单子变换器），一般规则是：单子是严格的，除非它的至少一个组成类型是懒惰的，在这种情况下，整个单子都会变得懒惰。

```F#
let _ : OptionT<list<unit option>> = monad { printfn "I'm strict" }
// will print I'm strict, because OptionT and list are strict

let _ : OptionT<seq<unit option>> = monad { printfn "I'm strict" }
// won't print anything, because seq is lazy
```

## 示例

您可以逐步运行此脚本。

```F#
#r @"nuget: FSharpPlus"
open FSharpPlus

let lazyValue = monad {
    let! a = lazy (printfn "I'm lazy"; 2)
    let! b = lazy (printfn "I'm lazy too"; 10)
    return a + b}

// val lazyValue : System.Lazy<int> = Value is not created.

let res12 = lazyValue.Value


let maybeWithSideFx = monad' { 
    let! a = Some 3
    let b = ref 0
    while !b < 10 do 
        let! n = Some ()
        incr b
    if a = 3 then printfn "got 3"
    else printfn "got something else (will never print this)"
    return a }

// val maybeWithSideFx : int option = Some 3



let lst = [None; None; Some 2; Some 4; Some 10; None]

let maybeManyTimes = monad.plus' {
    let defaultValue = 42
    let mutable i = 0
    return! None
    while i < 5 do
        printfn "looping %i" i
        i <- i + 1
        return! lst.[i]
    printfn "halfway"
    return! None
    printfn "near the end"
    return defaultValue }

// val maybeManyTimes : int option = Some 2


let (asnNumber: Async<_>) = monad.fx {
    let mutable m = ResizeArray ()
    try
        for i = 1 to 10 do
            m.Add i
        return m.[-1]
    with e ->
        return -3 }


let (lstNumber: list<_>) = monad.plus' {
    try
        for i = 1 to 10 do
            return i
    with e ->
        return -3 }


(*
有关计算表达式的更多信息，请阅读以下论文： The F# Computation Expression Zoo
http://tomasp.net/academic/papers/computation-zoo/computation-zoo.pdf
*)
```



# 镜头（Lens）

https://fsprojects.github.io/FSharpPlus/lens.html

Lens 是对函数的抽象，允许读取和更新不可变数据的一部分。

抽象名称来源于对数据结构特定部分的关注。

另一个类比可能是指针，但在这种情况下，数据被视为不可变的，这意味着它不会被修改，而是返回一个新的副本。

在这个[快速教程](https://fsprojects.github.io/FSharpPlus/tutorial.html#Lens)中，您可以找到一些使用镜头操作的基本示例。

为了允许对您的记录类型进行镜头设置，镜头（作为函数）必须为每个字段手工书写。

按照惯例，所有镜头标识符都将以下划线 `_` 开头。

以下是将镜头用于商业对象的示例：

```F#
#r @"nuget: FSharpPlus"
open System
open FSharpPlus
// In order to use the Lens module of F#+ we import the following:
open FSharpPlus.Lens

// From Mauricio Scheffer: https://gist.github.com/mausch/4260932
type Person = 
    { Name: string
      DateOfBirth: DateTime }

module Person =
    let inline _name f p =
        f p.Name <&> fun x -> { p with Name = x }

type Page =
    { Contents: string }

module Page =
    let inline _contents f p =
        f p.Contents <&> fun x -> {p with Contents = x}

type Book = 
    { Title: string
      Author: Person 
      Pages: Page list }

module Book =
    let inline _author f b =
        f b.Author <&> fun a -> { b with Author = a }

    let inline _authorName b = _author << Person._name <| b

    let inline _pages f b =
        f b.Pages <&> fun p -> { b with Pages = p }

    let inline _pageNumber i b =
        _pages << List._item i << _Some <| b

let rayuela =
    { Book.Title = "Rayuela"
      Author = { Person.Name = "Julio Cortázar"
                 DateOfBirth = DateTime(1914, 8, 26) } 
      Pages = [
        { Contents = "Once upon a time" }
        { Contents = "The End"} ] }
    
// read book author name:
let authorName1 = view Book._authorName rayuela
//  you can also write the read operation as:
let authorName2 = rayuela ^. Book._authorName

// write value through a lens
let book1 = setl Book._authorName "William Shakespear" rayuela
// update value
let book2 = over Book._authorName String.toUpper rayuela
```

注：

运算符 `<&>` 在 F#+ v1.0 中不可用，但由于它是翻转映射（flipped map），您可以使用 `</flip-map/>`。

但是，建议升级 F#+，因为使用 `<&>` 可以获得更好的编译时间。

## 棱镜（Prism）

也称为部分镜头（Partial Lens），它们聚焦于可能存在或不存在的数据部分。

请参阅以下使用内置 `_Some` 棱镜的示例。

```F#
type Team   = { Name: string; Victories: int }
let inline _name      f t = f t.Name      <&> fun n -> { t with Name      = n }
let inline _victories f t = f t.Victories <&> fun v -> { t with Victories = v }

type Player = { Team: Team; Score: int }
let inline _team  f p = f p.Team  <&> fun t -> { p with Team  = t }
let inline _score f p = f p.Score <&> fun s -> { p with Score = s }

type Result = { Winner: Player option; Started: bool}
let inline _winner   f r = f r.Winner  <&> fun w -> { r with Winner  = w }
let inline _started  f r = f r.Started <&> fun s -> { r with Started = s }

type Match<'t>  = { Players: 't; Finished: bool }
// For polymorphic updates to be possible, we can't use `with` expression on generic field lens.
let inline _players  f m = f m.Players  <&> fun p -> { Finished = m.Finished; Players  = p }
let inline _finished f m = f m.Finished <&> fun f -> { m with Finished = f }

// Lens composed with Prism -> Prism
let inline _winnerTeam x = (_players << _winner << _Some << _team) x

// initial state
let match0 =
    { Players = 
            { Team = { Name = "The A Team"; Victories = 0 }; Score = 0 },
            { Team = { Name = "The B Team"; Victories = 0 }; Score = 0 }
      Finished = false }


// Team 1 scores
let match1 = over (_players << _1 << _score) ((+) 1) match0

// Team 2 scores
let match2 = over (_players << _2 << _score) ((+) 1) match1

// Produce Match<Result> from Match<Player * Player> 
// This is possible with these Lenses since they support polymorphic updates.
let matchResult0 = setl _players { Winner = None; Started = true } match2

// See if there is a winner by using a prism
let _noWinner = preview _winnerTeam matchResult0

// Team 1 scores
let match3 = over (_players << _1 << _score) ((+) 1) match2

// End of the match
let match4 = setl _finished true match3
let match5 = over (_players << _1 << _team << _victories) ((+) 1) match4
let matchResult1 = over _players (fun (x, _) -> { Winner = Some x; Started = true }) match5

// And the winner is ...
let winner = preview _winnerTeam matchResult1
```

## Traversal

```F#
let t1 = [|"Something"; ""; "Something Else"; ""|] |> setl (_all "") ("Nothing")
// val t1 : string [] = [|"Something"; "Nothing"; "Something Else"; "Nothing"|]

// we can preview it
let t2 = [|"Something"; "Nothing"; "Something Else"; "Nothing"|] |> preview (_all "Something")
// val t2 : string option = Some "Something"

// view all elements in a list
let t3 = [|"Something"; "Nothing"; "Something Else"; "Nothing"|] |> toListOf (_all "Something")
// val t3 : string list = ["Something"]

// also view it, since string is a monoid
let t4 = [|"Something"; "Nothing"; "Something Else"; "Nothing"|] |> view  (_all "Something")
// val t4 : string = "Something"

// Lens composed with a Traversal -> Traversal
let t5 = [((), "Something"); ((),""); ((), "Something Else"); ((),"")] |> preview  (_all ((),"Something") << _2)
// val t5 : Option<string> = Some "Something"
```

## Fold

```F#
open FSharpPlus.Lens
open FSharpPlus // This module contain other functions relevant for the examples (length, traverse)
open FSharpPlus.Data // Mult

let f1 = over both length ("hello","world")
// val f1 : int * int = (5, 5)

let f2 = ("hello","world")^.both
// val f2 : string = "helloworld"

let f3 = anyOf both ((=)'x') ('x','y')
// val f3 : bool = true

let f4 = (1,2)^..both
// val f4 : int list = [1; 2]

let f5 = over items length ["hello";"world"]
// val f5 : int list = [5; 5]

let f6 = ["hello";"world"]^.items
// val f6 : string = "helloworld"

let f7 = anyOf items ((=)'x') ['x';'y']
// val f7 : bool = true

let f8 = [1;2]^..items
// val f8 : int list = [1; 2]

let f9 = foldMapOf (traverse << both << _Some) Mult [(Some 21, Some 21)]
// val f9 : Mult<int> = Mult 441

let f10 = foldOf (traverse << both << _Some) [(Some 21, Some 21)]
// val f10 : int = 42

let f11 = allOf both (fun x-> x >= 3) (4,5)
// val f11 : bool = true
```

## Iso

```F#
let toOption (isSome, v) = if isSome then Some v else None
let fromOption = function Some (x:'t) -> (true, x) | None -> (false, Unchecked.defaultof<'t>)
let inline isoTupleOption x = x |> iso toOption fromOption


let i1 = view isoTupleOption (System.Int32.TryParse "42")
// val i1 : int option = Some 42

let i2 = view (from' isoTupleOption) (Some 42)
// val i2 : bool * int = (true, 42)

// Iso composed with a Lens -> Lens
let i3 = view (_1 << isoTupleOption) (System.Int32.TryParse "42", ())
// val i3 : int option = Some 42
```

## 最大值和最小值

```F#
let fv3 = maximumOf (traverse << both << _Some) [(Some 1, Some 2);(Some 3,Some 4)]
// val fv3 : int option = Some 4

let fv4 = minimumOf (traverse << both << _Some) [(Some 1, Some 2);(Some 3,Some 4)]
// val fv4 : int option = Some 1
```

### 推荐阅读

- 强烈推荐 Matt Thornton 的博客 [Grokking Lenses](https://dev.to/choc13/grokking-lenses-2jgp)。它包含使用 F#+ 的示例和从头开始的解释。



# 解析

https://fsprojects.github.io/FSharpPlus/parsing.html

F#+ 提供了几个辅助方法，以简化构建解析器和类似解析的任务。

## Parse

Parse 允许您对标准类型和实现具有正确签名的静态 Parse 方法的类型使用 `parse` 泛型方法。

### 最小定义

```F#
static member Parse (x:'r) :'T
```

或

```F#
static member Parse (x:'r, c:CultureInfo) :'T
```

## TryParse

TryParse 允许您对标准类型和实现具有正确签名的静态 TryParse 方法的类型使用 `tryParse` 泛型方法。

### 最小定义

为了将 `tryParse` 与类型一起使用，该类型需要实现一个类似 TryParse 的静态方法。

您可以使用 F# 样式的 TryParse：

```F#
static member TryParse(value:'r) : 'T option
```

或 C# 样式的 TryParse：

```F#
static member TryParse (x:'r, [<Out>] result: 'T byref) :bool
```

用 C# 表示将是：

```c#
public static bool TryParse (string x, out T result) 
```

当你有实现上述定义的类型时，一件很好的事情是定义活动模式很简单：

```F#
let (|Port|_|) : _-> UInt16 option = tryParse
let (|IPAddress|_|) :_->System.Net.IPAddress option = tryParse
```

## sscanf，trySscanf 和朋友们

在 F# 中，你有一些很好的实用函数来创建 printf 风格的字符串编写器函数。在 F#+ 中，我们找到了逆：sscanf 和 trySscanf。

例如，如果你想根据已知的 url 格式进行解析：

```F#
let route1 x = trySscanf "/api/resource/%d" x
let parsed : int option = route1 "/api/resource/1"
```



# 数字函数（Numeric functions）

https://fsprojects.github.io/FSharpPlus/numerics.html

这个库附带了一些额外的数值函数和常量。

这些函数适用于许多数字类型

```F#
let qr0  = divRem 7  3  //val qr0 : int * int = (2, 1)
let qr1  = divRem 7I 3I //val qr1 : System.Numerics.BigInteger * System.Numerics.BigInteger = (2, 1)
let qr2  = divRem 7. 3. //val qr2 : float * float = (2.333333333, 0.0) -> using default method.
```

## 数字常量

除了典型的数学常数外，有界类型还带有 `minValue` 和 `maxValue` 常数。

下面是一个示例，说明如何使用它来实现高效的 `findMin` 函数

```F#
let inline findMin (lst: 'a list) =
    let rec loop acc = function
        | [] -> acc
        | x::_ when x = minValue -> x
        | x::xs -> loop (if x < acc then x else acc) xs
    loop maxValue lst
    
let minInt  = findMin [1;0;12;2]
let minUInt = findMin [1u;0u;12u;2u]  // loops only twice
```

## 对数字类型的通用操作

在 F# 中，编写针对不同数字类型的通用代码可能会非常乏味。

使用这个库变得很容易，但了解数字抽象及其局限性很重要。

为了对泛型类型进行合理的类型推理，我们需要严格的操作。

例如，F# 对 `(+)` 的定义可以采用 2 种不同的类型，这使得与一些以非常任意的方式定义了 `(+)` 运算符的 .NET 类型进行交互成为可能。

例如，您可以使用 `(+)` 运算符向 `DateTime` 添加一个 `float`，该 `float` 将被解释为秒。

打开 `FSharpPlus.Math.Generic` 命名空间——这将不再可能，因为这是为了获得体面的类型推断而进行的权衡。

## 泛型数字字面量

带有 G 后缀的数字是泛型。

```F#
open FSharpPlus.Math.Generic

let res5Int  : int    = 5G
let res5UInt : uint32 = 5G
```

在定义泛型函数时，通常需要定义泛型常量。由于在撰写本文时无法在 F# 中定义通用十进制文字，我们可以使用除法：

```F#
let inline areaOfCircle radio =
    let pi = 
        314159265358979323846264338G 
                    / 
        100000000000000000000000000G
    pi * radio * radio

let area1 = areaOfCircle 5.
let area2 = areaOfCircle 5.0f
let area3 = areaOfCircle 5.0M
```

## 定义自定义类型，支持泛型操作

```F#
type Vector2d<'T> = Vector2d of 'T * 'T  with
    static member inline (+) (Vector2d(a:'t, b:'t), Vector2d(c:'t, d:'t)) = Vector2d (((a + c):'t), ((b + d):'t))
    static member inline (-) (Vector2d(a:'t, b:'t), Vector2d(c:'t, d:'t)) = Vector2d (((a - c):'t), ((b - d):'t))
    static member inline (*) (Vector2d(a:'t, b:'t), Vector2d(c:'t, d:'t)) = Vector2d (((a * c):'t), ((b * d):'t))
    static member        Return x                               = Vector2d (x, x)
    static member        Map(Vector2d(x, y), f)                 = Vector2d (f x, f y)
    static member inline FromBigInt x = let y = fromBigInt x in Vector2d (y, y)
```

请注意，我们没有定义将向量添加到数字的重载

为什么？除了乏味之外，它们还会打破数学运算符的严格性

因此，我们将在泛型函数的类型推断方面遇到问题。

好的，但是如何对一个数字加（减、乘）呢？

选项 1，明确“提升”数字。

需要返回和（+、-、*）

```F#
let x1  = Vector2d (32,5) + result 7
let x1' = result 7 + Vector2d (32,5)
```

选项 2，使用泛型数字

需要 `FromBigInt` 和（+、-、*、/）

```F#
open FSharpPlus.Math.Generic
let x2  = Vector2d (32,5) + 7G
let x2' = 7G + Vector2d (32,5)
```

选项 3，使用应用子数学运算符只需要 `Map`

```F#
open FSharpPlus.Math.Applicative
let x3 = Vector2d (32,5) .+ 7
let x3' = 7 +. Vector2d (32,5)
```

## 与第三方库集成

我们可能会使用其他库中定义的类型，假设我们在某个地方定义了这种类型 Ratio。

```F#
type Ratio =
    struct
        val Numerator   : bigint
        val Denominator : bigint
        new (numerator: bigint, denominator: bigint) = {Numerator = numerator; Denominator = denominator}
    end
    override this.ToString() = this.Numerator.ToString() + " % " + this.Denominator.ToString()

let ratio (a:bigint) (b:bigint) :Ratio =
    if b = 0I then failwith "Ratio.%: zero denominator"
    let a, b = if b < 0I then (-a, -b) else (a, b)
    let gcd = gcd a b
    Ratio (a / gcd, b / gcd)

let Ratio (x,y) = x </ratio/> y

type Ratio with
    static member inline (/) (a:Ratio, b:Ratio) = (a.Numerator * b.Denominator) </ratio/> (a.Denominator * b.Numerator)                                              
    static member inline (+) (a:Ratio, b:Ratio) = (a.Numerator * b.Denominator + b.Numerator * a.Denominator) </ratio/> (a.Denominator * b.Denominator)
    static member inline (-) (a:Ratio, b:Ratio) = (a.Numerator * b.Denominator - b.Numerator * a.Denominator) </ratio/> (a.Denominator * b.Denominator)
    static member inline (*) (a:Ratio, b:Ratio) = (a.Numerator * b.Numerator) </ratio/> (a.Denominator * b.Denominator)

    static member inline Abs        (r:Ratio) = (abs    r.Numerator) </ratio/> r.Denominator
    static member inline Signum     (r:Ratio) = (signum r.Numerator) </ratio/> 1I
    static member inline FromBigInt (x:bigint) = fromBigInt x </ratio/> 1I
    static member inline (~-)       (r:Ratio) = -(r.Numerator) </ratio/> r.Denominator
```

由于大多数 Rational 实现都定义了 Numerator 和 Denominator，我们可以在上面使用泛型函数：

```F#
let some3_2 = trySqrt (Ratio(9I, 4I))
```

## 示例：创建多态二次函数

二次函数根据其操作的域而具有不同的结果。

例如，对于实数，它可以有 0 或 2 个解（也可以说 1 是双解）。

但对于复数，它总是有两个解。

```F#
open FSharpPlus.Math.Generic

let inline quadratic a b c =
    let root1 = ( -b + sqrt (  b * b - 4G * a * c) )  / (2G * a)
    let root2 = ( -b - sqrt (  b * b - 4G * a * c) )  / (2G * a)
    (root1,root2)


let noRes  = quadratic 2.0  3G 9G
// val noRes : float * float = (nan, nan)

let res30_15  = quadratic 2.0  -3G -9G
// val res30_15 : float * float = (3.0, -1.5)

let res30_15f = quadratic 2.0f -3G -9G
// val res30_15f : float32 * float32 = (3.0f, -1.5f)

let resCmplx:System.Numerics.Complex * _ = quadratic 2G -3G 9G
// val resCmplx : System.Numerics.Complex * System.Numerics.Complex = ((0.75, -1.98431348329844), (0.75, 1.98431348329844))

let res30_15r:Ratio * _ = quadratic 2G -3G -9G
// val res30_15r : Ratio * Ratio = (3 % 1, -3 % 2)
```

