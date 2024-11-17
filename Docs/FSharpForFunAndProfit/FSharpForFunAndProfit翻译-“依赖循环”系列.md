

# [返回主 Markdown](./FSharpForFunAndProfit翻译.md)



# 1 循环依赖是邪恶的

*Part of the "Dependency cycles" series (*[link](https://fsharpforfunandprofit.com/posts/cyclic-dependencies/#series-toc)*)*

循环依赖关系：第1部分
27五月2013 这篇文章是超过3年

https://fsharpforfunandprofit.com/posts/cyclic-dependencies/

关于模块组织和循环依赖关系的三篇相关文章之一。

关于 F# 最常见的抱怨之一是，它要求代码按照依赖顺序排列。也就是说，您不能对编译器尚未看到的代码使用正向引用。

下面是一个典型的例子：

> “.fs 文件的顺序使得编译变得困难……我的 F# 应用程序只有 50 多行代码，但即使是最微小的非平凡的应用程序，它也已经比值得编译的工作更多了。有没有办法让 F# 编译器更像 C# 编译器，这样它就不会与文件传递给编译器的顺序紧密耦合？” [[fpish.net\]](http://fpish.net/topic/None/57578)

另一个：

> “在尝试用 F# 构建一个略高于玩具大小的项目后，我得出的结论是，用目前的工具很难维护一个即使是中等复杂性的项目。”[www.ikriv.com]

另一个：

> “F# 编译器太线性了。F# 编译器应该自动处理所有类型解析问题，与声明顺序无关”[www.sturmnet.org]

还有一个：

> “这个论坛上已经讨论了 F# 项目系统恼人（在我看来是不必要的）限制的话题。我说的是编译顺序的控制方式” [[fpish.net\]](http://fpish.net/topic/Some/0/59219)

好吧，这些抱怨是没有根据的。您当然可以使用 F# 构建和维护大型项目。F# 编译器和核心库是两个明显的例子。

事实上，这些问题大多归结为“为什么 F# 不能像 C# 一样”。如果你来自 C#，你习惯于让编译器自动连接一切。必须明确地处理依赖关系是非常烦人的——甚至是老式的和倒退的。

这篇文章的目的是解释（a）为什么依赖管理很重要，以及（b）一些可以帮助你处理它的技术。

## 依赖是坏事…

我们都知道依赖是我们生存的祸根。程序集依赖关系、配置依赖关系、数据库依赖关系、网络依赖关系——总有一些东西。

因此，我们开发人员作为一个职业，倾向于投入大量精力使依赖关系更易于管理。这一目标体现在许多不同的方面：接口隔离原则、控制反转和依赖注入；使用 NuGet 进行包管理；使用木偶/厨师（puppet/chef）进行配置管理；等等。从某种意义上说，所有这些方法都试图减少我们必须注意的事情的数量，以及可能发生故障的事情的数目。

当然，这不是一个新问题。经典著作《大规模 C++ 软件设计》的很大一部分内容都是关于依赖关系管理的。正如作者约翰·拉科斯所说：

> “通过避免组件之间不必要的依赖关系，可以显著降低子系统的维护成本”

这里的关键词是“不必要”。什么是“不必要的”依赖？当然，这取决于。但有一种特殊的依赖几乎总是不必要的——**循环依赖**。

## …循环依赖是邪恶的

为了理解为什么循环依赖是邪恶的，让我们重新审视一下“组件”的含义。

组件是好东西。无论你认为它们是包、程序集、模块、类还是其他什么，它们的主要目的都是将大量代码分解成更小、更易于管理的部分。换句话说，我们正在对软件开发问题采用分而治之的方法。

但是，为了对维护、部署或其他方面有用，组件不应该只是随机的东西集合。它（当然）应该只将相关代码组合在一起。

在一个理想的世界里，每个组件都是完全独立的。但一般来说（当然），一些依赖关系总是必要的。

但是，现在我们有了具有依赖关系的组件，我们需要一种方法来管理这些依赖关系。一种标准的方法是使用“分层”原则。我们可以有“高级”层和“低级”层，关键规则是：每一层都应该只依赖于它下面的层，而不能依赖于它上面的层。

我敢肯定，你对此非常熟悉。以下是一些简单层的示意图：

![img](https://fsharpforfunandprofit.com/posts/cyclic-dependencies/Layering1.png)

但现在，当你像这样从底层向顶层引入依赖关系时，会发生什么？

![img](https://fsharpforfunandprofit.com/posts/cyclic-dependencies/Layering2.png)

通过自下而上的依赖，我们引入了邪恶的“循环依赖”。

为什么它是邪恶的？因为现在任何替代的分层方法都是有效的！

例如，我们可以把底层放在上面，就像这样：

![img](https://fsharpforfunandprofit.com/posts/cyclic-dependencies/Layering3.png)

从逻辑的角度来看，这种替代分层与原始分层完全相同。

或者我们把中间层放在上面怎么样？

![img](https://fsharpforfunandprofit.com/posts/cyclic-dependencies/Layering3b.png)

事情出了大问题！很明显，我们真的把事情搞砸了。

事实上，一旦组件之间存在任何形式的循环依赖，你唯一能做的就是将它们全部放入同一层。

![img](https://fsharpforfunandprofit.com/posts/cyclic-dependencies/Layering4.png)

换句话说，循环依赖完全破坏了我们的“分而治之”方法，这是拥有组件的全部原因。我们现在只有一个“超级组件”，而不是三个组件，它比需要的大三倍，也更复杂。

![img](https://fsharpforfunandprofit.com/posts/cyclic-dependencies/Layering5.png)

这就是为什么循环依赖是邪恶的。

有关此主题的更多信息，请参阅 StackOverflow 的答案和 Patrick Smacchia（NDepend 的）关于分层的文章。

## 现实世界中的循环依赖关系

让我们先看看 .NET 程序集它们之间的循环依赖关系。以下是布莱恩·麦克纳马拉的一些战争故事（我的重点）：

> 这个 ,Net Framework 2.0 明显存在这个问题；System.dll，System.Configuration.dll 和 System.Xml.dll 都无可救药地相互纠缠。这以各种丑陋的方式表现出来。例如，我在 VS 调试器中发现了一个简单的 [bug]，当在尝试加载符号时遇到断点时，由于这些程序集之间的循环依赖关系，它会有效地使被调试对象崩溃。另一个故事：我的一个朋友是 Silverlight 初始版本的开发人员，他的任务是试图精简这三个程序集，第一个艰巨的任务是试图解开循环依赖关系。**“免费相互递归”在小范围内非常方便，但在大范围内会毁了你**。

> VS2008 比计划晚了一周发货，因为 VS2008 依赖于 SQL server，而 SQL server 依赖于 VS，哎呀！最终，他们无法生产出所有产品都具有相同内部版本号的完整产品版本，不得不争先恐后地使其工作。 [[fpish.net\]](http://fpish.net/topic/None/59219#comment-70220)

因此，有大量证据表明程序集之间的循环依赖是不好的。事实上，程序集之间的循环依赖关系被认为已经够糟糕的了，Visual Studio 甚至不允许你创建它们！

你可能会说，“是的，我能理解为什么循环依赖对程序集不好，但为什么要费心处理程序集中的代码呢？”

好吧，原因完全一样！分层允许更好的分区、更容易的测试和更清晰的重构。你可以在我比较 C# 项目和 F# 项目的一篇关于“野外”依赖循环的相关文章中看到我的意思。F# 项目中的依赖关系不像意大利面条那么简单。

另一句引用自Brian（优秀）的评论：

> 我在这里宣扬一个不受欢迎的立场，但我的经验是，当你被迫在系统的各个层面考虑和管理“软件组件之间的依赖顺序”时，世界上的一切都会变得更好。F# 的具体 UI/工具可能还不理想，但我认为原则是正确的。这是你想要的负担。这是更多的工作。“单元测试”也是更多的工作，但我们已经达成共识，工作是“值得的”，因为从长远来看，它可以节省你的时间。我对“排序”也有同样的感觉。系统中的类和方法之间存在依赖关系。忽略这些依赖关系会带来风险。一个迫使你考虑这种依赖关系图（大致上是拓扑类型的组件）的系统可能会引导你开发具有更清晰架构、更好的系统分层和更少不必要依赖关系的软件。

---

如果你喜欢我用图片解释事物的方式，看看我的《领域建模使函数化》一书！这是对领域驱动设计、类型建模和函数式编程的友好介绍。

---

## 检测并消除循环依赖关系

好吧，我们一致认为循环依赖是不好的。那么，我们如何检测它们，然后摆脱它们呢？

让我们从检测开始。有许多工具可以帮助您检测代码中的循环依赖关系。

- 如果你使用的是 C#，你需要一个像 NDepend 这样的工具。
- 如果您使用的是 Java，则有等效的工具，如 JDepend。
- 但如果你正在使用 F#，那你真是幸运！您可以免费获得循环依赖检测！

“非常有趣，”你可能会说，“我已经知道 F# 的循环依赖禁令了——这让我抓狂！我该怎么办才能解决这个问题，让编译器满意？”

为此，你需要阅读下一篇文章…

# 2 重构以消除循环依赖关系

*Part of the "Dependency cycles" series (*[link](https://fsharpforfunandprofit.com/posts/removing-cyclic-dependencies/#series-toc)*)*

循环依赖关系：第2部分
27五月2013 这篇文章是超过3年

https://fsharpforfunandprofit.com/posts/removing-cyclic-dependencies/

在上一篇文章中，我们研究了依赖周期的概念，以及它们为什么不好。

在这篇文章中，我们将介绍一些从代码中消除它们的技术。起初，不得不这样做可能看起来很烦人，但实际上，从长远来看，你会意识到，“这不是一个bug，这是一个功能！”

## 对一些常见的循环依赖关系进行分类

让我们对您可能遇到的依赖关系进行分类。我将介绍三种常见情况，并为每种情况演示一些处理方法。

首先，我称之为“*方法依赖*”。

- 类型A在属性中存储类型B的值
- 类型B在方法签名中引用类型A，但不存储类型A的值

其次，我称之为“*结构性依赖*”。

- 类型A在属性中存储类型B的值
- B类型在属性中存储a类型的值

最后，我将称之为“*继承依赖*”。

- 类型A在属性中存储类型B的值
- 类型B继承自类型A

当然，还有其他变体。但如果你知道如何处理这些问题，你也可以使用同样的技术来处理其他问题。

## 处理 F# 中依赖关系的三个技巧

在我们开始之前，这里有三个有用的技巧，在试图理清依赖关系时通常适用。

### 提示 1：像对待 F# 一样对待 F#。

请注意，F# 并非 C#。如果你愿意使用 F# 的原生习惯用法来使用它，那么通过使用不同风格的代码组织来避免循环依赖通常是非常简单的。

### 提示 2：将类型与行为分开。

由于 F# 中的大多数类型都是不可变的，因此它们甚至可以被“暴露”和“贫血”。因此，在函数式设计中，通常将类型本身与作用于它们的功能分开。这种方法通常有助于清理依赖关系，我们将在下面看到。

### 提示 3：参数化，参数化，再参数化。

依赖关系只能在引用特定类型时发生。如果你使用泛型类型，你就不能有依赖关系！

与其对类型的行为进行硬编码，为什么不通过传递函数来对其进行参数化呢？`List` 模块就是这种方法的一个很好的例子，我也将在下面展示一些例子。

## 处理“方法依赖性”

我们将从最简单的依赖关系开始——我称之为“方法依赖关系”。

这里有一个例子。

```F#
module MethodDependencyExample =

    type Customer(name, observer:CustomerObserver) =
        let mutable name = name
        member this.Name
            with get() = name
            and set(value) =
                name <- value
                observer.OnNameChanged(this)

    and CustomerObserver() =
        member this.OnNameChanged(c:Customer) =
            printfn "Customer name changed to '%s' " c.Name

    // test
    let observer = new CustomerObserver()
    let customer = Customer("Alice",observer)
    customer.Name <- "Bob"
```

`Customer` 类具有 `CustomerObserver` 类型的属性/字段，但 `CustomerObserver` 类具有一个将 `Customer` 作为参数的方法，从而导致相互依赖。

### 使用“and”关键字

让类型编译的一个简单方法是使用 `and` 关键字，就像我上面做的那样。

`and` 关键字正是为这种情况而设计的——它允许您有两个或多个相互引用的类型。

要使用它，只需将第二个 `type` 关键字替换为 `and`。请注意，如下所示的使用 `and type` 是不正确的。只有一个 `and`，这就是你所需要的。

```F#
type Something
and type SomethingElse  // wrong

type Something
and SomethingElse       // correct
```

但是，`and` 它有很多问题，除非作为最后的手段，否则通常不鼓励使用它。

首先，它只适用于在同一模块中声明的类型。您不能跨模块边界使用它。

其次，它应该只用于微小的类型。如果在 `type` 和 `and` 之间有500行代码，那么你做得非常错误。

```F#
type Something
   // 500 lines of code
and SomethingElse
   // 500 more lines of code
```

上面显示的代码片段是如何不这样做的示例。

换句话说，不要把 `and` 当作灵丹妙药。过度使用它是你没有正确重构代码的症状。

### 引入参数化

因此，让我们看看使用参数化可以做什么，而不是使用 `and`，正如第三个技巧中提到的那样。

如果我们考虑示例代码，我们真的需要一个特殊的 `CustomerObserver` 类吗？为什么我们只限于 `Customer`？我们不能有一个更通用的观察者类吗？

那么，我们为什么不创建一个 `INameObserver<'T>` 接口，使用相同的 `OnNameChanged` 方法，但该方法（和接口）参数化为接受任何类呢？

我的意思是：

```F#
module MethodDependency_ParameterizedInterface =

    type INameObserver<'T> =
        abstract OnNameChanged : 'T -> unit

    type Customer(name, observer:INameObserver<Customer>) =
        let mutable name = name
        member this.Name
            with get() = name
            and set(value) =
                name <- value
                observer.OnNameChanged(this)

    type CustomerObserver() =
        interface INameObserver<Customer> with
            member this.OnNameChanged c =
                printfn "Customer name changed to '%s' " c.Name

    // test
    let observer = new CustomerObserver()
    let customer = Customer("Alice", observer)
    customer.Name <- "Bob"
```

在这个修订版本中，依赖关系被打破了！完全不需要 `and`。事实上，现在您甚至可以将这些类型放入不同的项目或程序集中！

该代码与第一个版本几乎相同，除了 `Customer` 构造函数接受一个接口，`CustomerObserver` 现在实现了相同的接口。事实上，我认为引入接口实际上使代码比以前更好。

但我们不必止步于此。现在我们有了一个接口，我们真的需要创建一个完整的类来实现它吗？F# 有一个很好的特性，称为对象表达式，它允许您直接实例化接口。

这是同样的代码，但这次完全删除了 `CustomerObserver` 类，直接创建了 `INameObserver`。

```F#
module MethodDependency_ParameterizedInterface =

    // code as above

    // test
    let observer2 = {
        new INameObserver<Customer> with
            member this.OnNameChanged c =
                printfn "Customer name changed to '%s' " c.Name
        }
    let customer2 = Customer("Alice", observer2)
    customer2.Name <- "Bob"
```

这种技术显然也适用于更复杂的接口，如下图所示，其中有两种方法：

```F#
module MethodDependency_ParameterizedInterface2 =

    type ICustomerObserver<'T> =
        abstract OnNameChanged : 'T -> unit
        abstract OnEmailChanged : 'T -> unit

    type Customer(name, email, observer:ICustomerObserver<Customer>) =

        let mutable name = name
        let mutable email = email

        member this.Name
            with get() = name
            and set(value) =
                name <- value
                observer.OnNameChanged(this)

        member this.Email
            with get() = email
            and set(value) =
                email <- value
                observer.OnEmailChanged(this)

    // test
    let observer2 = {
        new ICustomerObserver<Customer> with
            member this.OnNameChanged c =
                printfn "Customer name changed to '%s' " c.Name
            member this.OnEmailChanged c =
                printfn "Customer email changed to '%s' " c.Email
        }
    let customer2 = Customer("Alice", "x@example.com",observer2)
    customer2.Name <- "Bob"
    customer2.Email <- "y@example.com"
```

### 使用函数而不是参数化

在许多情况下，我们还可以进一步消除接口类。为什么不传入一个简单的函数，当名称更改时调用它，如下所示：

```F#
module MethodDependency_ParameterizedClasses_HOF  =

    type Customer(name, observer) =

        let mutable name = name

        member this.Name
            with get() = name
            and set(value) =
                name <- value
                observer this

    // test
    let observer(c:Customer) =
        printfn "Customer name changed to '%s' " c.Name
    let customer = Customer("Alice", observer)
    customer.Name <- "Bob"
```

我想你会同意这个片段比之前的任何一个版本都“不那么正式”。观察器现在根据需要内联定义，非常简单：

```F#
let observer(c:Customer) =
    printfn "Customer name changed to '%s' " c.Name
```

诚然，它只在被替换的接口很简单时才有效，但即便如此，这种方法的使用频率也可能比你想象的要高。

## 一种更具函数式的方法：将类型与函数分离

正如我上面提到的，一个更“功能性的设计”是将类型本身与作用于这些类型的函数分开。让我们看看在这种情况下如何做到这一点。

这是第一次通过：

```F#
module MethodDependencyExample_SeparateTypes =

    module DomainTypes =
        type Customer = { name:string; observer:NameChangedObserver }
        and  NameChangedObserver = Customer -> unit


    module Customer =
        open DomainTypes

        let changeName customer newName =
            let newCustomer = {customer with name=newName}
            customer.observer newCustomer
            newCustomer     // return the new customer

    module Observer =
        open DomainTypes

        let printNameChanged customer =
            printfn "Customer name changed to '%s' " customer.name

    // test
    module Test =
        open DomainTypes

        let observer = Observer.printNameChanged
        let customer = {name="Alice"; observer=observer}
        Customer.changeName customer "Bob"
```

在上面的例子中，我们现在有三个模块：一个用于类型，另一个用于函数。显然，在实际应用中，`Customer` 模块中与客户相关的函数比这个模块多得多！

不过，在这段代码中，`Customer` 和 `CustomerObserver` 之间仍然存在相互依赖关系。类型定义更紧凑，所以这不是问题，但即便如此，我们能消除 `and` 吗？

是的，当然。我们可以使用与前一种方法相同的技巧，消除观察者类型，直接在 `Customer` 数据结构中嵌入一个函数，如下所示：

```F#
module MethodDependency_SeparateTypes2 =

    module DomainTypes =
        type Customer = { name:string; observer:Customer -> unit}

    module Customer =
        open DomainTypes

        let changeName customer newName =
            let newCustomer = {customer with name=newName}
            customer.observer newCustomer
            newCustomer     // return the new customer

    module Observer =
        open DomainTypes

        let printNameChanged customer =
            printfn "Customer name changed to '%s' " customer.name

    module Test =
        open DomainTypes

        let observer = Observer.printNameChanged
        let customer = {name="Alice"; observer=observer}
        Customer.changeName customer "Bob"
```

### 使类型更愚蠢

`Customer` 类型中仍然嵌入了一些行为。在许多情况下，不需要这样做。一种更实用的方法是只在需要时传递函数。

因此，让我们从客户类型中删除 `observer`，并将其作为额外参数传递给 `changeName` 函数，如下所示：

```F#
let changeName observer customer newName =
    let newCustomer = {customer with name=newName}
    observer newCustomer    // call the observer with the new customer
    newCustomer             // return the new customer
```

以下是完整的代码：

```F#
module MethodDependency_SeparateTypes3 =

    module DomainTypes =
        type Customer = {name:string}

    module Customer =
        open DomainTypes

        let changeName observer customer newName =
            let newCustomer = {customer with name=newName}
            observer newCustomer    // call the observer with the new customer
            newCustomer             // return the new customer

    module Observer =
        open DomainTypes

        let printNameChanged customer =
            printfn "Customer name changed to '%s' " customer.name

    module Test =
        open DomainTypes

        let observer = Observer.printNameChanged
        let customer = {name="Alice"}
        Customer.changeName observer customer "Bob"
```

你可能会认为我现在把事情弄得更复杂了——我必须在代码中调用 `changeName` 的每个地方指定 `observer` 函数。这肯定比以前更糟糕了吧？至少在 OO 版本中，观察者是客户对象的一部分，我不必一直传递它。

啊，但是，你忘了部分应用的魔力！你可以设置一个带有“内置”观察者的函数，然后在任何地方使用该函数，而不需要每次使用时都传入观察者。聪明！

```F#
module MethodDependency_SeparateTypes3 =

    // code as above

    module TestWithPartialApplication =
        open DomainTypes

        let observer = Observer.printNameChanged

        // set up this partial application only once (at the top of your module, say)
        let changeName = Customer.changeName observer

        // then call changeName without needing an observer
        let customer = {name="Alice"}
        changeName customer "Bob"
```

### 但是等等…还有更多！

让我们再次查看 `changeName` 函数：

```F#
let changeName observer customer newName =
    let newCustomer = {customer with name=newName}
    observer newCustomer    // call the observer with the new customer
    newCustomer             // return the new customer
```

它有以下步骤：

1. 做点什么使结果值
2. 用结果值调用观察者
3. 返回结果值

这是完全通用的逻辑，与客户无关。因此，我们可以将其重写为一个完全通用的库函数。我们的新函数将允许任何观察者函数“挂钩”到任何其他函数的结果中，所以现在让我们称之为 `hook`。

```F#
let hook2 observer f param1 param2 =
    let y = f param1 param2 // do something to make a result value
    observer y              // call the observer with the result value
    y                       // return the result value
```

实际上，我之所以称之为 `hook2`，是因为被“钩住”的函数 `f` 有两个参数。我可以为有一个参数的函数制作另一个版本，如下所示：

```F#
let hook observer f param1 =
    let y = f param1 // do something to make a result value
    observer y       // call the observer with the result value
    y                // return the result value
```

如果你读过面向铁路的编程文章，你可能会注意到这与我所说的“死胡同”函数非常相似。我不会在这里详细介绍，但这确实是一种常见的模式。

好的，回到代码——我们如何使用这个泛型 `hook` 函数？

- `Customer.changeName` 是挂接到的函数，它有两个参数，因此我们使用 `hook2`。
- 观察者功能和以前一样

因此，我们再次创建了一个部分应用的 `changeName` 函数，但这次我们通过将观察器和挂钩函数传递给 `hook2` 来创建它，如下所示：

```F#
let observer = Observer.printNameChanged
let changeName = hook2 observer Customer.changeName
```

请注意，生成的 `changeName` 与原始 `Customer.changeName` 函数具有完全相同的签名，因此可以在任何地方与它互换使用。

```F#
let customer = {name="Alice"}
changeName customer "Bob"
```

以下是完整的代码：

```F#
module MethodDependency_SeparateTypes_WithHookFunction =

    [<AutoOpen>]
    module MyFunctionLibrary =

        let hook observer f param1 =
            let y = f param1 // do something to make a result value
            observer y       // call the observer with the result value
            y                // return the result value

        let hook2 observer f param1 param2 =
            let y = f param1 param2 // do something to make a result value
            observer y              // call the observer with the result value
            y                       // return the result value

    module DomainTypes =
        type Customer = { name:string}

    module Customer =
        open DomainTypes

        let changeName customer newName =
            {customer with name=newName}

    module Observer =
        open DomainTypes

        let printNameChanged customer =
            printfn "Customer name changed to '%s' " customer.name

    module TestWithPartialApplication =
        open DomainTypes

        // set up this partial application only once (at the top of your module, say)
        let observer = Observer.printNameChanged
        let changeName = hook2 observer Customer.changeName

        // then call changeName without needing an observer
        let customer = {name="Alice"}
        changeName customer "Bob"
```

创建这样的 `hook` 函数最初可能会增加额外的复杂性，但它从主应用程序中消除了更多的代码，一旦你建立了这样的函数库，你就会发现它们无处不在。

顺便说一句，如果它能帮助您使用 OO 设计术语，您可以将这种方法视为“装饰者”或“代理”模式。

## 处理“结构性依赖”

我们的第二个分类是我所说的“结构依赖”，其中每种类型存储另一种类型的值。

- 类型 A 在属性中存储类型 B 的值
- B 类型在属性中存储 A 类型的值

对于这组示例，考虑在某个 `Location` 工作的 `Employee`。`Employee` 包含他们 `Location` 的地点，`Location` 存储在那里工作的 `Employees` 列表。

瞧——相互依存！

下面是代码中的示例：

```F#
module StructuralDependencyExample =

    type Employee(name, location:Location) =
        member this.Name = name
        member this.Location = location

    and Location(name, employees: Employee list) =
        member this.Name = name
        member this.Employees  = employees
```

在我们开始重构之前，让我们考虑一下这种设计有多尴尬。我们如何在没有 `Location` 值的情况下初始化 `Employee` 值，反之亦然。

这里有一个尝试。我们创建一个包含空员工列表的位置，然后使用该位置创建其他员工：

```F#
module StructuralDependencyExample =

    // code as above

    module Test =
        let location = new Location("CA",[])
        let alice = new Employee("Alice",location)
        let bob = new Employee("Bob",location)

        location.Employees  // empty!
        |> List.iter (fun employee ->
            printfn "employee %s works at %s" employee.Name employee.Location.Name)
```

但这段代码并不像我们想要的那样工作。我们必须将 `location` 的员工列表设置为空，因为我们无法转发引用 `alice` 和 `bob` 值。。

F# 有时也会允许您在这种情况下使用 `and` 关键字，用于递归“let”。与“type”一样，“and”关键字取代了“let”关键字。与“type”不同，第一个“let”必须用 `let rec` 标记为递归。

让我们试试。我们会给 `location` 一个 `alice` 和 `bob` 的列表，即使它们还没有声明。

```F#
module UncompilableTest =
    let rec location = new Location("NY",[alice;bob])
    and alice = new Employee("Alice",location  )
    and bob = new Employee("Bob",location )
```

但是，编译器对我们创建的无限递归并不满意。在某些情况下，`and` 确实适用于 `let` 定义，但这不是其中之一！不管怎样，就像类型一样，必须对“let”定义使用 `and` 是你可能需要重构的一个线索。

所以，实际上，唯一明智的解决方案是使用可变结构，并在创建单个员工后修复位置对象，如下所示：

```F#
module StructuralDependencyExample_Mutable =

    type Employee(name, location:Location) =
        member this.Name = name
        member this.Location = location

    and Location(name, employees: Employee list) =
        let mutable employees = employees

        member this.Name = name
        member this.Employees  = employees
        member this.SetEmployees es =
            employees <- es

    module TestWithMutableData =
        let location = new Location("CA",[])
        let alice = new Employee("Alice",location)
        let bob = new Employee("Bob",location)
        // fixup after creation
        location.SetEmployees [alice;bob]

        location.Employees
        |> List.iter (fun employee ->
            printfn "employee %s works at %s" employee.Name employee.Location.Name)
```

所以，仅仅创造一些值就有很多麻烦。这也是为什么相互依赖是一个坏主意的另一个原因！

### 再次参数化

为了打破依赖关系，我们可以再次使用参数化技巧。我们可以创建一个参数化版本的 `Employee`。

```F#
module StructuralDependencyExample_ParameterizedClasses =

    type ParameterizedEmployee<'Location>(name, location:'Location) =
        member this.Name = name
        member this.Location = location

    type Location(name, employees: ParameterizedEmployee<Location> list) =
        let mutable employees = employees
        member this.Name = name
        member this.Employees  = employees
        member this.SetEmployees es =
            employees <- es

    type Employee = ParameterizedEmployee<Location>

    module Test =
        let location = new Location("CA",[])
        let alice = new Employee("Alice",location)
        let bob = new Employee("Bob",location)
        location.SetEmployees [alice;bob]

        location.Employees  // non-empty!
        |> List.iter (fun employee ->
            printfn "employee %s works at %s" employee.Name employee.Location.Name)
```

请注意，我们为 `Employee` 创建了一个类型别名，如下所示：

```F#
type Employee = ParameterizedEmployee<Location>
```

创建这样的别名的一个好处是，创建员工的原始代码将继续保持不变。

```F#
let alice = new Employee("Alice",location)
```

### 使用行为依赖关系进行参数化

上面的代码假设参数化的特定类并不重要。但是，如果依赖于该类型的特定属性呢？

例如，假设 `Employee` 类需要 `Name` 属性，`Location` 类需要 `Age` 属性，如下所示：

```F#
module StructuralDependency_WithAge =

    type Employee(name, age:float, location:Location) =
        member this.Name = name
        member this.Age = age
        member this.Location = location

        // expects Name property
        member this.LocationName = location.Name

    and Location(name, employees: Employee list) =
        let mutable employees = employees
        member this.Name = name
        member this.Employees  = employees
        member this.SetEmployees es =
            employees <- es

        // expects Age property
        member this.AverageAge =
            employees |> List.averageBy (fun e -> e.Age)

    module Test =
        let location = new Location("CA",[])
        let alice = new Employee("Alice",20.0,location)
        let bob = new Employee("Bob",30.0,location)
        location.SetEmployees [alice;bob]
        printfn "Average age is %g" location.AverageAge
```

我们怎样才能将其参数化？

那么，让我们尝试使用与之前相同的方法：

```F#
module StructuralDependencyWithAge_ParameterizedError =

    type ParameterizedEmployee<'Location>(name, age:float, location:'Location) =
        member this.Name = name
        member this.Age = age
        member this.Location = location
        member this.LocationName = location.Name  // error

    type Location(name, employees: ParameterizedEmployee<Location> list) =
        let mutable employees = employees
        member this.Name = name
        member this.Employees  = employees
        member this.SetEmployees es =
            employees <- es
        member this.AverageAge =
            employees |> List.averageBy (fun e -> e.Age)
```

该地点对 `ParameterizedEmployee.Age` 感到满意，但 `location.Name` 编译失败。显然，因为类型参数过于泛型。

一种方法是通过创建 `ILocation` 和 `IEmployee` 等接口来解决这个问题，这可能是最明智的方法。

但另一种方法是让 Location 参数是泛型的，并传入一个知道如何处理它的附加函数。在这种情况下，是一个 `getLocationName` 函数。

```F#
module StructuralDependencyWithAge_ParameterizedCorrect =

    type ParameterizedEmployee<'Location>(name, age:float, location:'Location, getLocationName) =
        member this.Name = name
        member this.Age = age
        member this.Location = location
        member this.LocationName = getLocationName location  // ok

    type Location(name, employees: ParameterizedEmployee<Location> list) =
        let mutable employees = employees
        member this.Name = name
        member this.Employees  = employees
        member this.SetEmployees es =
            employees <- es
        member this.AverageAge =
            employees |> List.averageBy (fun e -> e.Age)

```

对此的一种思考方式是，我们从外部提供行为，而不是作为类型的一部分。

为了使用它，我们需要传入一个函数和类型参数。这会一直很烦人，所以我们自然会把它封装在一个函数中，如下所示：

```F#
module StructuralDependencyWithAge_ParameterizedCorrect =

    // same code as above

    // create a helper function to construct Employees
    let Employee(name, age, location) =
        let getLocationName (l:Location) = l.Name
        new ParameterizedEmployee<Location>(name, age, location, getLocationName)
```

有了这个，原始测试代码继续工作，几乎没有变化（我们必须将 `new Employee` 更改为 `Employee`）。

```F#
module StructuralDependencyWithAge_ParameterizedCorrect =

    // same code as above

    module Test =
        let location = new Location("CA",[])
        let alice = Employee("Alice",20.0,location)
        let bob = Employee("Bob",30.0,location)
        location.SetEmployees [alice;bob]

        location.Employees  // non-empty!
        |> List.iter (fun employee ->
            printfn "employee %s works at %s" employee.Name employee.LocationName)
```

## 函数方法：再次将类型与函数分离

现在，让我们像以前一样，将函数式设计方法应用于这个问题。

同样，我们将把类型本身与作用于这些类型的函数分开。

```F#
module StructuralDependencyExample_SeparateTypes =

    module DomainTypes =
        type Employee = {name:string; age:float; location:Location}
        and Location = {name:string; mutable employees: Employee list}

    module Employee =
        open DomainTypes

        let Name (employee:Employee) = employee.name
        let Age (employee:Employee) = employee.age
        let Location (employee:Employee) = employee.location
        let LocationName (employee:Employee) = employee.location.name

    module Location =
        open DomainTypes

        let Name (location:Location) = location.name
        let Employees (location:Location) = location.employees
        let AverageAge (location:Location) =
            location.employees |> List.averageBy (fun e -> e.age)

    module Test =
        open DomainTypes

        let location = { name="NY"; employees= [] }
        let alice = {name="Alice"; age=20.0; location=location  }
        let bob = {name="Bob"; age=30.0; location=location }
        location.employees <- [alice;bob]

        Location.Employees location
        |> List.iter (fun e ->
            printfn "employee %s works at %s" (Employee.Name e) (Employee.LocationName e) )
```

在我们继续之前，让我们删除一些不需要的代码。使用记录类型的一个好处是，你不需要定义“getter”，所以在模块中你只需要操纵数据的函数，比如 `AverageAge`。

```F#
module StructuralDependencyExample_SeparateTypes2 =

    module DomainTypes =
        type Employee = {name:string; age:float; location:Location}
        and Location = {name:string; mutable employees: Employee list}

    module Employee =
        open DomainTypes

        let LocationName employee = employee.location.name

    module Location =
        open DomainTypes

        let AverageAge location =
            location.employees |> List.averageBy (fun e -> e.age)
```

### 再次参数化

再次，我们可以通过创建类型的参数化版本来删除依赖关系。

让我们退一步思考一下“位置”的概念。为什么一个位置必须只包含员工？如果我们让它更通用一点，我们可以把一个位置看作是一个“地方”加上“那个地方的一系列东西”。

例如，如果这些东西是产品，那么一个装满产品的地方可能是一个仓库。如果这些东西是书，那么一个满是书的地方可能就是图书馆。

以下是用代码表达的这些概念：

```F#
module LocationOfThings =

    type Location<'Thing> = {name:string; mutable things: 'Thing list}

    type Employee = {name:string; age:float; location:Location<Employee> }
    type WorkLocation = Location<Employee>

    type Product = {SKU:string; price:float }
    type Warehouse = Location<Product>

    type Book = {title:string; author:string}
    type Library = Location<Book>
```

当然，这些位置并不完全相同，但可能有一些共同点可以提取到通用设计中，特别是当它们所包含的东西没有附加行为要求时。

因此，使用“事物的位置”设计，我们的依赖关系被重写为使用参数化类型。

```F#
module StructuralDependencyExample_SeparateTypes_Parameterized =

    module DomainTypes =
        type Location<'Thing> = {name:string; mutable things: 'Thing list}
        type Employee = {name:string; age:float; location:Location<Employee> }

    module Employee =
        open DomainTypes

        let LocationName employee = employee.location.name

    module Test =
        open DomainTypes

        let location = { name="NY"; things = [] }
        let alice = {name="Alice"; age=20.0; location=location  }
        let bob = {name="Bob"; age=30.0; location=location }
        location.things <- [alice;bob]

        let employees = location.things
        employees
        |> List.iter (fun e ->
            printfn "employee %s works at %s" (e.name) (Employee.LocationName e) )

        let averageAge =
            employees
            |> List.averageBy (fun e -> e.age)
```

在这个修改后的设计中，您将看到 `AverageAge` 函数已从 `Location` 模块中完全删除。真的不需要它，因为我们可以很好地“内联”进行这类计算，而不需要特殊函数的开销。

如果你仔细想想，如果我们确实需要预定义这样一个函数，那么把它放在 `Employee` 模块而不是 `Location` 模块中可能更合适。毕竟，功能与员工的工作方式比与地点的工作方式更相关。

我的意思是：

```F#
module Employee =

    let AverageAgeAtLocation location =
        location.things |> List.averageBy (fun e -> e.age)
```

这是模块优于类的一个优点；您可以混合和匹配不同类型的函数，只要它们都与底层用例相关。

### 将关系转化为不同的类型

在迄今为止的示例中，位置中的“事物列表”字段必须是可变的。我们如何在使用不可变类型的同时仍然支持关系？

好吧，一种不这样做的方法是我们看到的那种相互依赖。在这种设计中，同步（或缺乏同步）是一个可怕的问题

例如，我可以在不告知 Alice 指向的位置的情况下更改她的位置，从而导致不一致。但如果我试图更改位置的内容，那么我也需要更新 Bob 的值。以此类推，无止境。一场噩梦，基本上。

使用不可变数据的正确方法是从数据库设计中窃取一片叶子，并将关系提取到一个单独的“表”或类型中。当前关系保存在单个主列表中，因此当进行更改时，不需要同步。

这是一个非常粗糙的例子，使用了一个简单的 `Relationship`s 列表。

```F#
module StructuralDependencyExample_Normalized =

    module DomainTypes =
        type Relationship<'Left,'Right> = 'Left * 'Right

        type Location= {name:string}
        type Employee = {name:string; age:float }

    module Employee =
        open DomainTypes

        let EmployeesAtLocation location relations =
            relations
            |> List.filter (fun (loc,empl) -> loc = location)
            |> List.map (fun (loc,empl) -> empl)

        let AverageAgeAtLocation location relations =
            EmployeesAtLocation location relations
            |> List.averageBy (fun e -> e.age)

    module Test =
        open DomainTypes

        let location = { Location.name="NY"}
        let alice = {name="Alice"; age=20.0; }
        let bob = {name="Bob"; age=30.0; }
        let relations = [
            (location,alice)
            (location,bob)
            ]

        relations
        |> List.iter (fun (loc,empl) ->
            printfn "employee %s works at %s" (empl.name) (loc.name) )
```

当然，更有效的设计会使用字典/映射，或为这类事情设计的特殊内存结构。

## 继承依赖关系

最后，让我们看看“继承依赖”。

- 类型 A 在属性中存储类型 B 的值
- 类型 B 继承自类型 A

我们将考虑一个 UI 控件层次结构，其中每个控件都属于一个顶级“窗体”，窗体本身就是一个控件。

以下是实现的第一步：

```F#
module InheritanceDependencyExample =

    type Control(name, form:Form) =
        member this.Name = name

        abstract Form : Form
        default this.Form = form

    and Form(name) as self =
        inherit Control(name, self)

    // test
    let form = new Form("form")       // NullReferenceException!
    let button = new Control("button",form)
```

这里要注意的是，`form` 将自己作为 Control 构造函数的表单值传入。

此代码将编译，但会在运行时导致 `NullReferenceException` 错误。这种技术在 C# 中有效，但在 F# 中无效，因为类初始化逻辑的执行方式不同。

不管怎样，这是一个糟糕的设计。表单不应该将自己传递给构造函数。

一个更好的设计，也修复了构造函数错误，是将 `Control` 改为抽象类，并区分非表单子类（在构造函数中确实采用表单）和 `Form` 类本身（不采用表单）。

以下是一些示例代码：

```F#
module InheritanceDependencyExample2 =

    [<AbstractClass>]
    type Control(name) =
        member this.Name = name

        abstract Form : Form

    and Form(name) =
        inherit Control(name)

        override this.Form = this

    and Button(name,form) =
        inherit Control(name)

        override this.Form = form

    // test
    let form = new Form("form")
    let button = new Button("button",form)
```

### 我们的老朋友再次参数化

为了消除循环依赖，我们可以按照通常的方式对类进行参数化，如下所示。

```F#
module InheritanceDependencyExample_ParameterizedClasses =

    [<AbstractClass>]
    type Control<'Form>(name) =
        member this.Name = name

        abstract Form : 'Form

    type Form(name) =
        inherit Control<Form>(name)

        override this.Form = this

    type Button(name,form) =
        inherit Control<Form>(name)

        override this.Form = form


    // test
    let form = new Form("form")
    let button = new Button("button",form)
```

### 函数式版本

我会留下一个函数式设计作为练习，让你自己做。

如果我们要进行真正的函数式设计，我们可能根本不会使用继承。相反，我们将结合参数化使用组合。

但这是一个大话题，所以我会留到另一天再谈。

## 摘要

我希望这篇文章能给你一些关于消除依赖循环的有用提示。有了这些不同的方法，模块组织的任何问题都应该能够很容易地得到解决。

在本系列的下一篇文章中，我将通过比较一些现实世界中的 C# 和 F# 项目来研究“野外”的依赖循环。

正如我们所看到的，F# 是一种非常固执己见的语言！它希望我们使用模块而不是类，并禁止依赖循环。这些只是烦人的事情，还是它们真的对代码的组织方式产生了影响？继续阅读并找出答案！

# 3 循环和模块化在野外

比较 C# 和 F# 项目的一些实际指标
28五月2013 这篇文章是超过3年

https://fsharpforfunandprofit.com/posts/cycles-and-modularity-in-the-wild/

（更新于2013年6月15日。请参阅帖子末尾的评论）

（更新于2014年4月12日。后续帖子将同样的分析应用于 Roslyn）

（更新于2015年1月23日。Evelina Gabasova 对这一分析进行了更清晰的阐述。她知道自己在说什么，所以我强烈建议你先读她的帖子！）

这是之前关于模块组织和循环依赖关系的两篇文章的后续文章。

我认为看看用 C# 和 F# 编写的一些实际项目会很有趣，看看它们在模块化和循环依赖数量方面的比较。

## 计划

我的计划是拿大约十个用 C# 编写的项目和十个左右用 F# 编写的项目，并以某种方式进行比较。

我不想在这上面花太多时间，所以与其试图分析源文件，我想我会稍微作弊，使用 [Mono.Cecil](http://www.mono-project.com/Cecil) 库分析编译后的程序集。

这也意味着我可以使用 NuGet 直接获取二进制文件。

我选择的项目是：

*C# 项目*

- [Mono.Cecil](http://nuget.org/packages/Mono.Cecil/),检查ECMA CIL格式的程序和库。
- [NUnit](http://nuget.org/packages/NUnit/)
- [SignalR](http://nuget.org/packages/Microsoft.AspNet.SignalR/) 用于实时 web 函数式。
- [NancyFx](http://nuget.org/packages/Nancy/), 一个 web 框架
- [YamlDotNet](http://nuget.org/packages/YamlDotNet.Core/), 用于解析和发出 YAML。
- [SpecFlow](http://nuget.org/packages/SpecFlow/), 一个  BDD 工具。
- [Json.NET](http://nuget.org/packages/Newtonsoft.Json/)。
- [Entity Framework](http://nuget.org/packages/EntityFramework/5.0.0)。
- [ELMAH](http://nuget.org/packages/elmah/), 一个 ASP.NET 的日志框架。
- [NuGet](http://nuget.org/packages/Nuget.Core/) 本身。
- [Moq](http://nuget.org/packages/Moq/)，一个模拟框架。
- [NDepend](http://ndepend.com/)，一个代码分析工具。
- 而且，为了表明我是公平的，我用 C# 编写了一个业务应用程序。

*F# 项目*

不幸的是，目前还没有各种各样的F#项目可供选择。我选择了以下内容：

- [FSharp.Core](http://nuget.org/packages/FSharp.Core/)，核心 F# 库。
- [FSPowerPack](http://nuget.org/packages/FSPowerPack.Community/)。
- [FsUnit](http://nuget.org/packages/FsUnit/)，NUnit的扩展。
- [Canopy](http://nuget.org/packages/canopy/)，一个围绕Selenium测试自动化工具的包装器。
- [FsSql](http://nuget.org/packages/FsSql/)，一个不错的小 ADO.NET 包装器。
- [WebSharper](http://nuget.org/packages/WebSharper/2.4.85.235)，web 框架。
- [TickSpec](http://nuget.org/packages/TickSpec/)，一个 BDD 工具。
- [FSharpx](http://nuget.org/packages/FSharpx.Core/)，一个 F# 库。
- [FParsec](http://nuget.org/packages/FParsec/), 一个解析器库。
- [FsYaml](http://nuget.org/packages/FsYaml/), 一个基于 FParsec 构建的 YAML 库。
- [Storm](http://storm.codeplex.com/releases/view/18871), 一个用于测试web服务的工具。
- [Foq](http://nuget.org/packages/Foq/), 一个模拟框架。
- 我编写的另一个业务应用程序，这次是用 F# 编写的。

我确实选择了 SpecFlow 和 TickSpec 作为直接可比对象，以及 Moq 和 Foq。

但正如你所看到的，大多数 F# 项目与 C# 项目没有直接可比性。例如，没有与 Nancy 或 Entity Framework 等效的直接 F#。

尽管如此，我还是希望通过比较这些项目来观察某种模式。我是对的。继续阅读结果！

## 使用哪些指标？

我想研究两件事：“模块化”和“循环依赖性”。

首先，“模块化”的单位应该是什么？

从编码的角度来看，我们通常使用文件（Smalltalk 是一个明显的例外），因此将文件视为模块化的单位是有意义的。文件用于将相关项目分组在一起，如果两个代码块在不同的文件中，它们在某种程度上就不像在同一个文件中那样“相关”。

在 C# 中，最佳实践是每个文件有一个类。所以 20 个文件意味着 20 个类。有时类有嵌套类，但除极少数例外情况外，嵌套类与父类位于同一文件中。这意味着我们可以忽略它们，只使用顶级类作为我们的模块化单位，作为文件的代理。

在 F# 中，最佳实践是每个文件有一个模块（有时更多）。所以 20 个文件意味着 20 个模块。在幕后，模块被转化为静态类，模块中定义的任何类都被转化为嵌套类。所以，这意味着我们可以忽略嵌套类，只使用顶级类作为我们的模块化单位。

C# 和 F# 编译器为 LINQ、lambdas 等生成了许多“隐藏”类型。在某些情况下，我想排除这些类型，只包括“编写”的类型，这些类型是为显式编码的。我还将 F# 生成的 case 类排除在“编写”类之外。这意味着具有三个案例的联合类型将被视为一个已编写的类型，而不是四个。

因此，我对顶级类型的定义是：一种非嵌套且非编译器生成的类型。

我为模块化选择的指标是：

- 如上定义的**顶级类型的数量**。
- 如上定义的**已编写类型的数量**。
- **所有类型的数量**。这个数字也包括编译器生成的类型。将这个数字与顶级类型进行比较，可以让我们了解顶级类型的代表性。
- **项目的规模**。显然，在一个更大的项目中会有更多的类型，所以我们需要根据项目的规模进行调整。我选择的大小指标是指令的数量，而不是文件的物理大小。这消除了嵌入式资源等问题。

### 依赖关系

一旦我们有了模块化单元，我们就可以查看模块之间的依赖关系。

对于此分析，我只想包括同一程序集中类型之间的依赖关系。换句话说，对 `String` 或 `List` 等系统类型的依赖关系不算作依赖关系。

假设我们有一个顶级类型 `A` 和另一个顶层类型 `B`。然后我说 `A` 到 `B` 之间存在依赖关系，如果：

- 类型 `A` 或其任何嵌套类型继承（或实现）类型 `B` 或其任嵌套类型。
- 类型 `A` 或其任何嵌套类型都有一个字段、属性或方法，将类型 `B` 或其任何嵌套类型作为参数或返回值引用。这也包括私有成员——毕竟，它仍然是一种依赖关系。
- 类型 `A` 或其任何嵌套类型都有一个方法实现，该方法实现引用类型 `B` 或其任何嵌套类型。

这可能不是一个完美的定义。但这对我来说已经足够好了。

除了所有依赖关系，我认为查看“公共”或“已发布”的依赖关系可能有用。如果满足以下条件，则存在从 `A` 到 `B` 的公共依赖关系：

- 类型 `A` 或其任何嵌套类型继承（或实现）类型 `B` 或其任何嵌套类型。
- 类型 `A` 或其任何嵌套类型都有一个公共字段、属性或方法，将类型 `B` 或其任何嵌套类型作为参数或返回值引用。
- 最后，只有当源类型本身是公共的时，才会计算公共依赖关系。

我为依赖关系选择的指标是：

- **依赖项的总数**。这只是所有类型的所有依赖关系的总和。同样，在更大的项目中会有更多的依赖关系，但我们也会考虑项目的规模。
- **具有超过 X 个依赖项的类型的数量**。这让我们知道有多少类型“太”复杂。

### 循环依赖关系

根据这种依赖关系的定义，当两种不同的顶级类型相互依赖时，就会出现循环依赖关系。

请注意此定义中未包含的内容。如果模块中的嵌套类型依赖于同一模块中的另一个嵌套类型，则这不是循环依赖。

如果存在循环依赖关系，那么就有一组链接在一起的模块。例如，如果 `A` 依赖于 `B`，`B` 依赖于 `C`，然后说 `C` 依赖于 `A`，那么 `A`、`B` 和 `C` 就联系在一起了。在图论中，这被称为强连通分量。

我为循环依赖性选择的指标是：

- **循环次数**。也就是说，其中包含多个模块的强连接组件的数量。
- **最大组件的大小**。这让我们了解了依赖关系有多复杂。

我分析了所有依赖关系以及仅公共依赖关系的循环依赖关系。

## 做实验

首先，我使用 NuGet 下载了每个项目二进制文件。然后我写了一个小 F# 脚本，为每个程序集执行以下步骤：

1. 使用 [Mono.Cecil](http://www.mono-project.com/Cecil) 分析了组件，提取了所有类型，包括嵌套类型
2. 对于每种类型，提取了其他类型的公共和实现引用，分为内部（同一程序集）和外部（不同程序集）。
3. 创建了一个“顶级”类型列表。
4. 根据较低级别的依赖关系，创建了从每个顶级类型到其他顶级类型的依赖关系列表。

然后使用此依赖关系列表提取各种统计数据，如下所示。我还将依赖关系图渲染为 SVG 格式（使用 [graphViz](http://www.graphviz.org/)）。

对于循环检测，我使用 QuickGraph 库提取强连接组件，然后进行更多的处理和渲染。

如果你想了解血腥的细节，这里有一个我使用的脚本的链接，这是原始数据。

重要的是要认识到，这不是一项适当的统计研究，只是一项快速分析。然而，正如我们将要看到的，结果非常有趣。

## 模块化

让我们先看看模块化。

以下是 C# 项目的模块化相关结果：

| Project     | Code size | Top-level types | Authored types | All types | Code/Top | Code/Auth | Code/All | Auth/Top | All/Top |
| :---------- | :-------- | :-------------- | :------------- | :-------- | :------- | :-------- | :------- | :------- | :------ |
| ef          | 269521    | 514             | 565            | 876       | 524      | 477       | 308      | 1.1      | 1.7     |
| jsonDotNet  | 148829    | 215             | 232            | 283       | 692      | 642       | 526      | 1.1      | 1.3     |
| nancy       | 143445    | 339             | 366            | 560       | 423      | 392       | 256      | 1.1      | 1.7     |
| cecil       | 101121    | 240             | 245            | 247       | 421      | 413       | 409      | 1.0      | 1.0     |
| nuget       | 114856    | 216             | 237            | 381       | 532      | 485       | 301      | 1.1      | 1.8     |
| signalR     | 65513     | 192             | 229            | 311       | 341      | 286       | 211      | 1.2      | 1.6     |
| nunit       | 45023     | 173             | 195            | 197       | 260      | 231       | 229      | 1.1      | 1.1     |
| specFlow    | 46065     | 242             | 287            | 331       | 190      | 161       | 139      | 1.2      | 1.4     |
| elmah       | 43855     | 116             | 140            | 141       | 378      | 313       | 311      | 1.2      | 1.2     |
| yamlDotNet  | 23499     | 70              | 73             | 73        | 336      | 322       | 322      | 1.0      | 1.0     |
| fparsecCS   | 57474     | 41              | 92             | 93        | 1402     | 625       | 618      | 2.2      | 2.3     |
| moq         | 133189    | 397             | 420            | 533       | 335      | 317       | 250      | 1.1      | 1.3     |
| ndepend     | 478508    | 734             | 828            | 843       | 652      | 578       | 568      | 1.1      | 1.1     |
| ndependPlat | 151625    | 185             | 205            | 205       | 820      | 740       | 740      | 1.1      | 1.1     |
| personalCS  | 422147    | 195             | 278            | 346       | 2165     | 1519      | 1220     | 1.4      | 1.8     |
| TOTAL       | 2244670   | 3869            | 4392           | 5420      | 580      | 511       | 414      | 1.1      | 1.4     |

以下是 F# 项目的结果：

| Project        | Code size | Top-level types | Authored types | All types | Code/Top | Code/Auth | Code/All | Auth/Top | All/Top |
| :------------- | :-------- | :-------------- | :------------- | :-------- | :------- | :-------- | :------- | :------- | :------ |
| fsxCore        | 339596    | 173             | 328            | 2024      | 1963     | 1035      | 168      | 1.9      | 11.7    |
| fsCore         | 226830    | 154             | 313            | 1186      | 1473     | 725       | 191      | 2.0      | 7.7     |
| fsPowerPack    | 117581    | 93              | 150            | 410       | 1264     | 784       | 287      | 1.6      | 4.4     |
| storm          | 73595     | 67              | 70             | 405       | 1098     | 1051      | 182      | 1.0      | 6.0     |
| fParsec        | 67252     | 8               | 24             | 245       | 8407     | 2802      | 274      | 3.0      | 30.6    |
| websharper     | 47391     | 52              | 128            | 285       | 911      | 370       | 166      | 2.5      | 5.5     |
| tickSpec       | 30797     | 34              | 49             | 170       | 906      | 629       | 181      | 1.4      | 5.0     |
| websharperHtml | 14787     | 18              | 28             | 72        | 822      | 528       | 205      | 1.6      | 4.0     |
| canopy         | 15105     | 6               | 16             | 103       | 2518     | 944       | 147      | 2.7      | 17.2    |
| fsYaml         | 15191     | 7               | 11             | 160       | 2170     | 1381      | 95       | 1.6      | 22.9    |
| fsSql          | 15434     | 13              | 18             | 162       | 1187     | 857       | 95       | 1.4      | 12.5    |
| fsUnit         | 1848      | 2               | 3              | 7         | 924      | 616       | 264      | 1.5      | 3.5     |
| foq            | 26957     | 35              | 48             | 103       | 770      | 562       | 262      | 1.4      | 2.9     |
| personalFS     | 118893    | 30              | 146            | 655       | 3963     | 814       | 182      | 4.9      | 21.8    |
| TOTAL          | 1111257   | 692             | 1332           | 5987      | 1606     | 834       | 186      | 1.9      | 8.7     |

这些列是：

- 正如 Cecil 所报告的那样，**代码大小**是来自所有方法的 CIL 指令的数量。
- **顶级类型**是使用上述定义的程序集中顶级类型的总数。
- **编写类型**是程序集中的类型总数，包括嵌套类型、枚举等，但不包括编译器生成的类型。
- **所有类型**是程序集中类型的总数，包括编译器生成的类型。

我用一些额外的计算列扩展了这些核心指标：

- **Code/Top** 是每个顶级类型/模块的 CIL 指令数。这是衡量与每个模块化单元相关联的代码量的指标。一般来说，越多越好，因为如果你没有多个文件，你不想处理多个文件。另一方面，也有一个折衷方案。文件中的代码行太多，无法读取代码。在 C# 和 F# 中，好的做法是每个文件的代码不超过 500-1000 行，除了少数例外，我查看的源代码似乎就是这种情况。
- **Code/Auth** 是每种编写类型的 CIL 指令数。这是衡量每种创作类型有多“大”的指标。
- **Code/All** 是每种类型的 CIL 指令数。这是衡量每种类型有多“大”的指标。
- **Auth/Top** 是所有已编写类型与顶级类型的比率。这是对每个模块化单元中有多少已编写类型的粗略衡量。
- **All/Top** 是所有类型与顶级类型的比率。这是对每个模块化单元中有多少类型的粗略衡量。

### 分析

我注意到的第一件事是，除了少数例外，C# 项目的代码大小比 F# 项目大。当然，部分原因是我选择了更大的项目。但即使是像 SpecFlow 和 TickSpec 这样有点可比的项目，SpecFlow 的代码大小也更大。当然，SpecFlow 可能比 TickSpec 做得更多，但这也可能是F#中使用更通用代码的结果。目前还没有足够的信息来了解这两种情况——进行真正的并排比较会很有趣。

接下来是顶级类型的数量。我之前说过，这应该与项目中的文件数量相对应。是吗？

我没有得到所有项目的所有来源进行彻底检查，但我做了几次抽查。例如，对于 Nancy 来说，有 339 个顶级类，这意味着应该有大约 339 个文件。事实上，实际上有 322 .cs 文件，所以这是一个不错的估计。

另一方面，对于 SpecFlow，有 242 个顶级类型，但只有 171 .cs 文件，所以有点高估了。对于 Cecil 来说，情况也是一样：240 个顶级类，但只有 128 .cs文件。

对于 FSharpX 项目，有 173 个顶级类，这意味着应该有大约 173 个文件。事实上，实际上只有 78 .fs 文件，所以这是一个超过 2 倍的严重高估。如果我们看看 Storm，有 67 个顶级班级。事实上，实际上只有 35 .fs 文件，所以这再次被高估了 2 倍。

因此，看起来顶级类的数量总是高估了文件的数量，但对于 F# 来说比 C# 要多得多。值得在这方面做一些更详细的分析。

### 代码大小与顶级类型数量的比率

F# 代码的“代码/顶部”比率始终大于 C# 代码。总体而言，C# 中的平均顶级类型转换为 580 条指令。但对于 F# 来说，这个数字是 1606 条指令，大约是它的三倍。

我认为这是因为 F# 代码比 C# 代码更简洁。我猜测，一个模块中 500 行 F# 代码创建的 CIL 指令比一个类中 500 行 C# 代码创建的指令多得多。

如果我们直观地绘制“代码大小”与“顶级类型”的关系图，我们会得到这个图表：

![img](https://fsharpforfunandprofit.com/posts/cycles-and-modularity-in-the-wild/Metrics_CodeSize_TopLevel.png)

令我惊讶的是，在这个图表中，F# 和 C# 项目有多么不同。C# 项目似乎具有每 1000 条指令约 1-2 个顶级类型的一致比率，即使在不同的项目规模中也是如此。F# 项目也是一致的，每 1000 条指令的顶级类型比例约为 0.6。

事实上，F# 项目中顶级类型的数量似乎随着项目的扩大而逐渐减少，而不是像 C# 项目那样线性增加。

我从这个图表中得到的信息是，对于给定规模的项目，F# 实现将具有更少的模块，因此可能复杂性也会降低。

你可能注意到有两个异常。两个 C# 项目不合适——一个在 50K 标记处的是 FParsecCS，一个在 425K 标记处是我的业务应用程序。

我相当肯定这一点，因为这两个实现中都有一些相当大的 C# 类，这有助于提高代码率。对于解析器来说，这可能是一个必然的问题，但就我的业务应用程序而言，我知道这是由于多年来积累的积垢造成的，而且有一些庞大的类应该被重构成更小的类。因此，对于 C# 代码库来说，这样的指标可能是一个坏兆头。

### 代码大小与所有类型数量的比率

另一方面，如果我们比较代码与所有类型的比率，包括编译器生成的代码，我们会得到一个非常不同的结果。

以下是“代码大小”与“所有类型”的对应图表：

![img](https://fsharpforfunandprofit.com/posts/cycles-and-modularity-in-the-wild/Metrics_CodeSize_AllTypes.png)

这对 F# 来说是令人惊讶的线性。类型的总数（包括编译器生成的类型）似乎与项目的大小密切相关。另一方面，C# 的类型数量似乎变化很大。

F# 代码的类型的平均“大小”略小于 C# 代码。C# 中的平均类型被转换为大约 400 条指令。但对于 F# 来说，这个数字大约是 180 条指令。

我不确定为什么会这样。是因为 F# 类型更细粒度，还是因为 F# 编译器生成的小类型比 C# 编译器多得多？如果不做更微妙的分析，我无法判断。

### 顶级类型与已编写类型的比率

将类型计数与代码大小进行比较后，现在让我们将它们相互比较：

![img](https://fsharpforfunandprofit.com/posts/cycles-and-modularity-in-the-wild/Metrics_TopLevel_AuthTypes.png)

再次，存在显著差异。在 C# 中，每个模块化单元平均有 1.1 个编写类型。但在 F# 中，平均值为 1.9，对于一些项目来说，平均值要高得多。

当然，在 F# 中创建嵌套类型很简单，在 C# 中也很少见，所以你可以说这不是一个公平的比较。但是，在尽可能多的 F# 行中创建十几种类型的能力肯定会对设计质量产生一些影响吗？这在 C# 中很难做到，但没有什么能阻止你。那么，这是否意味着 C# 中存在一种诱惑，即不能像你可能的那样细粒度？

比率最高的项目（4.9）是我的 F# 业务应用程序。我认为这是因为这是这个列表中唯一一个围绕特定业务领域设计的 F# 项目，我使用这里描述的概念创建了许多“小”类型来准确地对该领域进行建模。对于使用 DDD 原则创建的其他项目，我希望看到同样的高数字。

## 依赖关系

现在让我们看看顶级类之间的依赖关系。

以下是 C# 项目的结果：

| Project     | Top Level Types | Total Dep. Count | Dep/Top | One or more dep. | Three or more dep. | Five or more dep. | Ten or more dep. | Diagram                                                      |
| :---------- | :-------------- | :--------------- | :------ | :--------------- | :----------------- | :---------------- | :--------------- | :----------------------------------------------------------- |
| ef          | 514             | 2354             | 4.6     | 76%              | 51%                | 32%               | 13%              | [svg](https://fsharpforfunandprofit.com/assets/svg/ef.all.dot.svg) [dotfile](https://fsharpforfunandprofit.com/assets/svg/ef.all.dot) |
| jsonDotNet  | 215             | 913              | 4.2     | 69%              | 42%                | 30%               | 14%              | [svg](https://fsharpforfunandprofit.com/assets/svg/jsonDotNet.all.dot.svg) [dotfile](https://fsharpforfunandprofit.com/assets/svg/jsonDotNet.all.dot) |
| nancy       | 339             | 1132             | 3.3     | 78%              | 41%                | 22%               | 6%               | [svg](https://fsharpforfunandprofit.com/assets/svg/nancy.all.dot.svg) [dotfile](https://fsharpforfunandprofit.com/assets/svg/nancy.all.dot) |
| cecil       | 240             | 1145             | 4.8     | 73%              | 43%                | 23%               | 13%              | [svg](https://fsharpforfunandprofit.com/assets/svg/cecil.all.dot.svg) [dotfile](https://fsharpforfunandprofit.com/assets/svg/cecil.all.dot) |
| nuget       | 216             | 833              | 3.9     | 71%              | 43%                | 26%               | 12%              | [svg](https://fsharpforfunandprofit.com/assets/svg/nuget.all.dot.svg) [dotfile](https://fsharpforfunandprofit.com/assets/svg/nuget.all.dot) |
| signalR     | 192             | 641              | 3.3     | 66%              | 34%                | 19%               | 10%              | [svg](https://fsharpforfunandprofit.com/assets/svg/signalR.all.dot.svg) [dotfile](https://fsharpforfunandprofit.com/assets/svg/signalR.all.dot) |
| nunit       | 173             | 499              | 2.9     | 75%              | 39%                | 13%               | 4%               | [svg](https://fsharpforfunandprofit.com/assets/svg/nunit.all.dot.svg) [dotfile](https://fsharpforfunandprofit.com/assets/svg/nunit.all.dot) |
| specFlow    | 242             | 578              | 2.4     | 64%              | 25%                | 17%               | 5%               | [svg](https://fsharpforfunandprofit.com/assets/svg/specFlow.all.dot.svg) [dotfile](https://fsharpforfunandprofit.com/assets/svg/specFlow.all.dot) |
| elmah       | 116             | 300              | 2.6     | 72%              | 28%                | 22%               | 6%               | [svg](https://fsharpforfunandprofit.com/assets/svg/elmah.all.dot.svg) [dotfile](https://fsharpforfunandprofit.com/assets/svg/elmah.all.dot) |
| yamlDotNet  | 70              | 228              | 3.3     | 83%              | 30%                | 11%               | 4%               | [svg](https://fsharpforfunandprofit.com/assets/svg/yamlDotNet.all.dot.svg) [dotfile](https://fsharpforfunandprofit.com/assets/svg/yamlDotNet.all.dot) |
| fparsecCS   | 41              | 64               | 1.6     | 59%              | 29%                | 5%                | 0%               | [svg](https://fsharpforfunandprofit.com/assets/svg/fparsecCS.all.dot.svg) [dotfile](https://fsharpforfunandprofit.com/assets/svg/fparsecCS.all.dot) |
| moq         | 397             | 1100             | 2.8     | 63%              | 29%                | 17%               | 7%               | [svg](https://fsharpforfunandprofit.com/assets/svg/moq.all.dot.svg) [dotfile](https://fsharpforfunandprofit.com/assets/svg/moq.all.dot) |
| ndepend     | 734             | 2426             | 3.3     | 67%              | 37%                | 25%               | 10%              | [svg](https://fsharpforfunandprofit.com/assets/svg/ndepend.all.dot.svg) [dotfile](https://fsharpforfunandprofit.com/assets/svg/ndepend.all.dot) |
| ndependPlat | 185             | 404              | 2.2     | 67%              | 24%                | 11%               | 4%               | [svg](https://fsharpforfunandprofit.com/assets/svg/ndependPlat.all.dot.svg) [dotfile](https://fsharpforfunandprofit.com/assets/svg/ndependPlat.all.dot) |
| personalCS  | 195             | 532              | 2.7     | 69%              | 29%                | 19%               | 7%               |                                                              |
| TOTAL       | 3869            | 13149            | 3.4     | 70%              | 37%                | 22%               | 9%               |                                                              |

以下是F#项目的结果：

| Project        | Top Level Types | Total Dep. Count | Dep/Top | One or more dep. | Three or more dep. | Five or more dep. | Ten or more dep. | Diagram                                                      |
| :------------- | :-------------- | :--------------- | :------ | :--------------- | :----------------- | :---------------- | :--------------- | :----------------------------------------------------------- |
| fsxCore        | 173             | 76               | 0.4     | 30%              | 4%                 | 1%                | 0%               | [svg](https://fsharpforfunandprofit.com/assets/svg/fsxCore.all.dot.svg) [dotfile](https://fsharpforfunandprofit.com/assets/svg/fsxCore.all.dot) |
| fsCore         | 154             | 287              | 1.9     | 55%              | 26%                | 14%               | 3%               | [svg](https://fsharpforfunandprofit.com/assets/svg/fsCore.all.dot.svg) [dotfile](https://fsharpforfunandprofit.com/assets/svg/fsCore.all.dot) |
| fsPowerPack    | 93              | 68               | 0.7     | 38%              | 13%                | 2%                | 0%               | [svg](https://fsharpforfunandprofit.com/assets/svg/fsPowerPack.all.dot.svg) [dotfile](https://fsharpforfunandprofit.com/assets/svg/fsPowerPack.all.dot) |
| storm          | 67              | 195              | 2.9     | 72%              | 40%                | 18%               | 4%               | [svg](https://fsharpforfunandprofit.com/assets/svg/storm.all.dot.svg) [dotfile](https://fsharpforfunandprofit.com/assets/svg/storm.all.dot) |
| fParsec        | 8               | 9                | 1.1     | 63%              | 25%                | 0%                | 0%               | [svg](https://fsharpforfunandprofit.com/assets/svg/fParsec.all.dot.svg) [dotfile](https://fsharpforfunandprofit.com/assets/svg/fParsec.all.dot) |
| websharper     | 52              | 18               | 0.3     | 31%              | 0%                 | 0%                | 0%               | [svg](https://fsharpforfunandprofit.com/assets/svg/websharper.all.dot.svg) [dotfile](https://fsharpforfunandprofit.com/assets/svg/websharper.all.dot) |
| tickSpec       | 34              | 48               | 1.4     | 50%              | 15%                | 9%                | 3%               | [svg](https://fsharpforfunandprofit.com/assets/svg/tickSpec.all.dot.svg) [dotfile](https://fsharpforfunandprofit.com/assets/svg/tickSpec.all.dot) |
| websharperHtml | 18              | 37               | 2.1     | 78%              | 39%                | 6%                | 0%               | [svg](https://fsharpforfunandprofit.com/assets/svg/websharperHtml.all.dot.svg) [dotfile](https://fsharpforfunandprofit.com/assets/svg/websharperHtml.all.dot) |
| canopy         | 6               | 8                | 1.3     | 50%              | 33%                | 0%                | 0%               | [svg](https://fsharpforfunandprofit.com/assets/svg/canopy.all.dot.svg) [dotfile](https://fsharpforfunandprofit.com/assets/svg/canopy.all.dot) |
| fsYaml         | 7               | 10               | 1.4     | 71%              | 14%                | 0%                | 0%               | [svg](https://fsharpforfunandprofit.com/assets/svg/fsYaml.all.dot.svg) [dotfile](https://fsharpforfunandprofit.com/assets/svg/fsYaml.all.dot) |
| fsSql          | 13              | 14               | 1.1     | 54%              | 8%                 | 8%                | 0%               | [svg](https://fsharpforfunandprofit.com/assets/svg/fsSql.all.dot.svg) [dotfile](https://fsharpforfunandprofit.com/assets/svg/fsSql.all.dot) |
| fsUnit         | 2               | 0                | 0.0     | 0%               | 0%                 | 0%                | 0%               | [svg](https://fsharpforfunandprofit.com/assets/svg/fsUnit.all.dot.svg) [dotfile](https://fsharpforfunandprofit.com/assets/svg/fsUnit.all.dot) |
| foq            | 35              | 66               | 1.9     | 66%              | 29%                | 11%               | 0%               | [svg](https://fsharpforfunandprofit.com/assets/svg/foq.all.dot.svg) [dotfile](https://fsharpforfunandprofit.com/assets/svg/foq.all.dot) |
| personalFS     | 30              | 111              | 3.7     | 93%              | 60%                | 27%               | 7%               |                                                              |
| TOTAL          | 692             | 947              | 1.4     | 49%              | 19%                | 8%                | 1%               |                                                              |

这些列是：

- **顶级类型**是程序集中顶级类型的总数，如前所述。
- **Total dep.count** 是顶级类型之间的依赖关系总数。
- **Dep/Top** 仅是每个顶级类型/模块的依赖项数量。这是衡量平均顶级类型/模块有多少依赖关系的指标。
- **一个或多个dep** 是依赖于一个或更多其他顶级类型的顶级类型的数量。
- **三个或更多dep**。与上述类似，但依赖于三个或多个其他顶级类型。
- **五个或更多依赖**。与上述类似。
- **十个或更多依赖**。与上述类似。具有如此多依赖关系的顶级类型将更难理解和维护。因此，这是衡量项目复杂程度的指标。

**图表**列包含一个指向 SVG 文件的链接，该文件由依赖关系生成，还包含用于生成 SVG 的 DOT 文件。有关这些图表的讨论，请参阅下文。（请注意，我不能公开我的应用程序的内部，所以我只会给出指标）

### 分析

这些结果非常有趣。对于 C#，总依赖项的数量随着项目规模的增加而增加。平均而言，每种顶级类型都依赖于 3-4 种其他类型。

另一方面，F# 项目中的总依赖项数量似乎根本不会随着项目规模的变化而变化太大。平均而言，每个 F# 模块依赖于不超过 1-2 个其他模块。最大的项目（FSharpX）的比率低于许多较小的项目。我的商业应用程序和 Storm 项目是唯一的例外。

下面是代码大小和依赖关系数量之间关系的图表：

![img](https://fsharpforfunandprofit.com/posts/cycles-and-modularity-in-the-wild/Metrics_CodeSize_Dependencies.png)

C# 和 F# 项目之间的差异非常明显。C# 依赖关系似乎随着项目规模的增加而线性增长，而 F# 依赖关系似乎是平稳的。

### 依赖关系的分布

每个顶级类型的平均依赖数很有趣，但它并不能帮助我们理解可变性。是否有许多具有大量依赖关系的模块？还是每个人都只有几个？

这可能会对可维护性产生影响。我假设一个只有一两个依赖关系的模块在应用程序的上下文中更容易理解，而这个模块有几十个依赖关系。

与其做复杂的统计分析，我想我会保持简单，只计算有多少顶级类型有一个或多个依赖关系，有三个或更多依赖关系，等等。

以下是视觉显示的相同结果：

![img](https://fsharpforfunandprofit.com/posts/cycles-and-modularity-in-the-wild/Metrics_CS_DependencyPercent.png)

![img](https://fsharpforfunandprofit.com/posts/cycles-and-modularity-in-the-wild/Metrics_FS_DependencyPercent.png)

那么，我们能从这些数字中推断出什么呢？

- 首先，在 F# 项目中，超过一半的模块根本没有外部依赖关系。这有点令人惊讶，但我认为这是由于与C#项目相比，泛型的使用量很大。
- 其次，F# 项目中的模块始终比 C# 项目中的类具有更少的依赖关系。
- 最后，在 F# 项目中，具有大量依赖关系的模块非常罕见，总体不到 2%。但在 C# 项目中，9% 的类对其他类的依赖超过 10 个。

F# 组中最糟糕的违规者是我自己的 F# 应用程序，就这些指标而言，它甚至比我的 C# 应用程序更糟糕。同样，这可能是由于大量使用领域特定类型形式的非泛型，也可能只是因为代码需要更多的重构！

### 依赖关系图

现在查看依赖关系图可能很有用。这些是 SVG 文件，因此您应该能够在浏览器中查看它们。

请注意，这些图表中的大多数都很大——所以打开它们后，你需要缩小一点才能看到任何东西！

让我们从比较 SpecFlow 和 TickSpec 的图表开始。

以下是 SpecFlow 的示例：

[![img](https://fsharpforfunandprofit.com/posts/cycles-and-modularity-in-the-wild/specflow_svg.png)](https://fsharpforfunandprofit.com/assets/svg/specFlow.all.dot.svg)

以下是 TickSpec 的示例：

[![img](https://fsharpforfunandprofit.com/posts/cycles-and-modularity-in-the-wild/tickspec_svg.png)](https://fsharpforfunandprofit.com/assets/svg/tickSpec.all.dot.svg)

每个图表都列出了项目中发现的所有顶级类型。如果一种类型与另一种类型之间存在依赖关系，则用箭头表示。在可能的情况下，依赖关系从左向右指向，因此任何从右向左的箭头都意味着存在循环依赖关系。

布局是由 graphviz 自动完成的，但一般来说，类型被组织成列或“列”。例如，SpecFlow 图有12个等级，TickSpec 图有 5 个等级。

正如你所看到的，在一个典型的依赖关系图中，通常有很多纠结的线条！图表看起来有多纠结是代码复杂性的一种视觉衡量标准。例如，如果我的任务是维护 SpecFlow 项目，在我理解了类之间的所有关系之前，我不会真正感到舒服。项目越复杂，加快速度所需的时间就越长。

### 揭示 OO vs 函数式设计？

TickSpec 图比 SpecFlow 图简单得多。这是因为 TickSpec 可能不如 SpecFlow 做得多吗？

答案是否定的，我认为这与功能集的大小无关，而是因为代码的组织方式不同。

查看 SpecFlow 类（dotfile），我们可以看到它通过创建接口遵循了良好的 OOD 和 TDD 实践。例如，有一个 `TestRunnerManager` 和一个 `ITestRunnerManager`。OOD 中还经常出现许多其他模式：“监听器”类和接口、“提供者”类和界面、“比较器”类和交互等等。

但是，如果我们查看 TickSpec 模块（dotfile），则根本没有接口。也没有“监听器”、“提供者”或“比较器”。代码中可能需要这些东西，但它们要么没有暴露在模块之外，要么更有可能的是，它们所扮演的角色是由函数而不是类型来实现的。

顺便说一句，我不是在挑剔 SpecFlow 代码。它看起来设计得很好，是一个非常有用的库，但我认为它确实突显了面向对象设计和函数式设计之间的一些区别。

### Moq 与 Foq 的比较

让我们也比较一下 Moq 和 Foq 的图表。这两个项目做的事情大致相同，所以代码应该是可比的。

和以前一样，用 F# 编写的项目有一个小得多的依赖关系图。

查看 Moq 类（dotfile），我们可以看到它包括“Castle”库，我没有从分析中删除它。在 249 个有依赖关系的类中，只有 66 个是特定于 Moq 的。如果我们只考虑 Moq 名称空间中的类，我们可能会有一个更清晰的图。

另一方面，查看 Foq 模块（dotfile），只有 23 个模块具有依赖关系，甚至比单独的 Moq 类还要少。

因此，F# 中的代码组织非常不同。

### FParsec 与 FParsecCS 的比较

FParsec 项目是一个有趣的自然实验。该项目有两个程序集，大小大致相同，但一个用 C# 编写，另一个用 F# 编写。

直接比较它们有点不公平，因为 C# 代码是为快速解析而设计的，而 F# 代码更高级。但是……无论如何，我都会不公平地比较它们！

以下是 F# 程序集“FParsec”和 C# 程序集“FPCarsecCS”的示意图。

它们既美观又清晰。可爱的代码！

从图中不清楚的是，我的方法对 C# 程序集不公平。

例如，C# 图显示运算符、运算符类型、InfixOperator 等之间存在依赖关系。但事实上，从源代码来看，这些类都在同一个物理文件中。在 F# 中，它们都在同一个模块中，它们之间的关系不算作公共依赖关系。因此，C# 代码在某种程度上受到了惩罚。

即便如此，从源代码来看，C# 代码有 20 个源文件，而 F# 有 8 个，因此在复杂性上仍然存在一些差异。

### 什么算作依赖？

不过，为了保护我的方法，唯一能将这些 FParsec C# 类放在同一个文件中的是良好的编码实践；它不是由 C# 编译器强制执行的。另一个维护者可能会出现，并在不知不觉中将它们分成不同的文件，这真的会增加复杂性。在 F# 中，你不可能这么容易做到这一点，当然也不是偶然。

所以这取决于你所说的“模块”和“依赖性”是什么意思。在我看来，一个模块包含真正“连接在一起”的东西，不应该轻易解耦。因此，模块内的依赖关系不计算在内，而模块之间的依赖关系则计算在内。

另一种思考方式是，F# 鼓励某些领域（模块）的高耦合，以换取其他领域的低耦合。在 C# 中，唯一可用的严格耦合是基于类的。任何松散的东西，比如使用名称空间，都必须使用良好实践或 NDepend 等工具来强制执行。

F# 方法是好是坏取决于你的偏好。因此，它确实使某些类型的重构变得更加困难。

## 循环依赖关系

最后，我们可以将注意力转向非常邪恶的循环依赖关系。（如果你想知道他们为什么不好，请阅读这篇文章）。

以下是 C# 项目的循环依赖结果。

| Project     | Top-level types | Cycle count | Partic. | Partic.% | Max comp. size | Cycle count (public) | Partic. (public) | Partic.% (public) | Max comp. size (public) | Diagram                                                      |
| :---------- | :-------------- | :---------- | :------ | :------- | :------------- | :------------------- | :--------------- | :---------------- | :---------------------- | :----------------------------------------------------------- |
| ef          | 514             | 14          | 123     | 24%      | 79             | 1                    | 7                | 1%                | 7                       | [svg](https://fsharpforfunandprofit.com/assets/svg/ef.all.cycles.dot.svg) [dotfile](https://fsharpforfunandprofit.com/assets/svg/ef.all.cycles.dot) |
| jsonDotNet  | 215             | 3           | 88      | 41%      | 83             | 1                    | 11               | 5%                | 11                      | [svg](https://fsharpforfunandprofit.com/assets/svg/jsonDotNet.all.cycles.dot.svg) [dotfile](https://fsharpforfunandprofit.com/assets/svg/jsonDotNet.all.cycles.dot) |
| nancy       | 339             | 6           | 35      | 10%      | 21             | 2                    | 4                | 1%                | 2                       | [svg](https://fsharpforfunandprofit.com/assets/svg/nancy.all.cycles.dot.svg) [dotfile](https://fsharpforfunandprofit.com/assets/svg/nancy.all.cycles.dot) |
| cecil       | 240             | 2           | 125     | 52%      | 123            | 1                    | 50               | 21%               | 50                      | [svg](https://fsharpforfunandprofit.com/assets/svg/cecil.all.cycles.dot.svg) [dotfile](https://fsharpforfunandprofit.com/assets/svg/cecil.all.cycles.dot) |
| nuget       | 216             | 4           | 24      | 11%      | 10             | 0                    | 0                | 0%                | 1                       | [svg](https://fsharpforfunandprofit.com/assets/svg/nuget.all.cycles.dot.svg) [dotfile](https://fsharpforfunandprofit.com/assets/svg/nuget.all.cycles.dot) |
| signalR     | 192             | 3           | 14      | 7%       | 7              | 1                    | 5                | 3%                | 5                       | [svg](https://fsharpforfunandprofit.com/assets/svg/signalR.all.cycles.dot.svg) [dotfile](https://fsharpforfunandprofit.com/assets/svg/signalR.all.cycles.dot) |
| nunit       | 173             | 2           | 80      | 46%      | 78             | 1                    | 48               | 28%               | 48                      | [svg](https://fsharpforfunandprofit.com/assets/svg/nunit.all.cycles.dot.svg) [dotfile](https://fsharpforfunandprofit.com/assets/svg/nunit.all.cycles.dot) |
| specFlow    | 242             | 5           | 11      | 5%       | 3              | 1                    | 2                | 1%                | 2                       | [svg](https://fsharpforfunandprofit.com/assets/svg/specFlow.all.cycles.dot.svg) [dotfile](https://fsharpforfunandprofit.com/assets/svg/specFlow.all.cycles.dot) |
| elmah       | 116             | 2           | 9       | 8%       | 5              | 1                    | 2                | 2%                | 2                       | [svg](https://fsharpforfunandprofit.com/assets/svg/elmah.all.cycles.dot.svg) [dotfile](https://fsharpforfunandprofit.com/assets/svg/elmah.all.cycles.dot) |
| yamlDotNet  | 70              | 0           | 0       | 0%       | 1              | 0                    | 0                | 0%                | 1                       | [svg](https://fsharpforfunandprofit.com/assets/svg/yamlDotNet.all.cycles.dot.svg) [dotfile](https://fsharpforfunandprofit.com/assets/svg/yamlDotNet.all.cycles.dot) |
| fparsecCS   | 41              | 3           | 6       | 15%      | 2              | 1                    | 2                | 5%                | 2                       | [svg](https://fsharpforfunandprofit.com/assets/svg/fparsecCS.all.cycles.dot.svg) [dotfile](https://fsharpforfunandprofit.com/assets/svg/fparsecCS.all.cycles.dot) |
| moq         | 397             | 9           | 50      | 13%      | 15             | 0                    | 0                | 0%                | 1                       | [svg](https://fsharpforfunandprofit.com/assets/svg/moq.all.cycles.dot.svg) [dotfile](https://fsharpforfunandprofit.com/assets/svg/moq.all.cycles.dot) |
| ndepend     | 734             | 12          | 79      | 11%      | 22             | 8                    | 36               | 5%                | 7                       | [svg](https://fsharpforfunandprofit.com/assets/svg/ndepend.all.cycles.dot.svg) [dotfile](https://fsharpforfunandprofit.com/assets/svg/ndepend.all.cycles.dot) |
| ndependPlat | 185             | 2           | 5       | 3%       | 3              | 0                    | 0                | 0%                | 1                       | [svg](https://fsharpforfunandprofit.com/assets/svg/ndependPlat.all.cycles.dot.svg) [dotfile](https://fsharpforfunandprofit.com/assets/svg/ndependPlat.all.cycles.dot) |
| personalCS  | 195             | 11          | 34      | 17%      | 8              | 5                    | 19               | 10%               | 7                       | [svg](https://fsharpforfunandprofit.com/assets/svg/personalCS.all.cycles.dot.svg) [dotfile](https://fsharpforfunandprofit.com/assets/svg/personalCS.all.cycles.dot) |
| TOTAL       | 3869            |             | 683     | 18%      |                |                      | 186              | 5%                |                         | [svg](https://fsharpforfunandprofit.com/assets/svg/TOTAL.all.cycles.dot.svg) [dotfile](https://fsharpforfunandprofit.com/assets/svg/TOTAL.all.cycles.dot) |

以下是 F# 项目的结果：

| Project        | Top-level types | Cycle count | Partic. | Partic.% | Max comp. size | Cycle count (public) | Partic. (public) | Partic.% (public) | Max comp. size (public) | Diagram                                                      |
| :------------- | :-------------- | :---------- | :------ | :------- | :------------- | :------------------- | :--------------- | :---------------- | :---------------------- | :----------------------------------------------------------- |
| fsxCore        | 173             | 0           | 0       | 0%       | 1              | 0                    | 0                | 0%                | 1                       | .                                                            |
| fsCore         | 154             | 2           | 5       | 3%       | 3              | 0                    | 0                | 0%                | 1                       | [svg](https://fsharpforfunandprofit.com/assets/svg/fsCore.all.cycles.dot.svg) [dotfile](https://fsharpforfunandprofit.com/assets/svg/fsCore.all.cycles.dot) |
| fsPowerPack    | 93              | 1           | 2       | 2%       | 2              | 0                    | 0                | 0%                | 1                       | [svg](https://fsharpforfunandprofit.com/assets/svg/fsPowerPack.all.cycles.dot.svg) [dotfile](https://fsharpforfunandprofit.com/assets/svg/fsPowerPack.all.cycles.dot) |
| storm          | 67              | 0           | 0       | 0%       | 1              | 0                    | 0                | 0%                | 1                       | .                                                            |
| fParsec        | 8               | 0           | 0       | 0%       | 1              | 0                    | 0                | 0%                | 1                       | .                                                            |
| websharper     | 52              | 0           | 0       | 0%       | 1              | 0                    | 0                | 0%                | 0                       | .                                                            |
| tickSpec       | 34              | 0           | 0       | 0%       | 1              | 0                    | 0                | 0%                | 1                       | .                                                            |
| websharperHtml | 18              | 0           | 0       | 0%       | 1              | 0                    | 0                | 0%                | 1                       | .                                                            |
| canopy         | 6               | 0           | 0       | 0%       | 1              | 0                    | 0                | 0%                | 1                       | .                                                            |
| fsYaml         | 7               | 0           | 0       | 0%       | 1              | 0                    | 0                | 0%                | 1                       | .                                                            |
| fsSql          | 13              | 0           | 0       | 0%       | 1              | 0                    | 0                | 0%                | 1                       | .                                                            |
| fsUnit         | 2               | 0           | 0       | 0%       | 0              | 0                    | 0                | 0%                | 0                       | .                                                            |
| foq            | 35              | 0           | 0       | 0%       | 1              | 0                    | 0                | 0%                | 1                       | .                                                            |
| personalFS     | 30              | 0           | 0       | 0%       | 1              | 0                    | 0                | 0%                | 1                       | .                                                            |
| TOTAL          | 692             |             | 7       | 1%       |                |                      | 0                | 0%                |                         | .                                                            |

这些列是：

- **顶级类型**是程序集中顶级类型的总数，如前所述。
- **循环计数**是循环的总数。理想情况下，它将为零。但更大并不一定更糟。我认为，10 个小循环比一个大循环好。
- **Partic.**。参与任何循环的顶级类型的数量。
- **Partic.%**。参与任何循环的顶级类型的数量，占所有类型的百分比。
- **Max comp. size** 是最大循环组件中顶级类型的数量。这是衡量循环复杂程度的指标。如果只有两个相互依赖的类型，那么循环的复杂程度远低于 123 个相互依赖类型。
- **… (public)** 列具有相同的定义，但仅使用公共依赖项。我想看看将分析限制在公共依赖关系上会有什么影响会很有趣。
- **图表**列包含一个指向 SVG 文件的链接，该文件仅由循环中的依赖关系生成，还包含用于生成 SVG 的 DOT 文件。分析见下文。

### 分析

如果我们在 F# 代码中寻找循环，我们会非常失望。只有两个 F# 项目有循环，而且这些循环很小。例如，在 FSharp 中。在这里，同一文件中相邻的两种类型之间存在相互依赖关系。

另一方面，几乎所有的 C# 项目都有一个或多个循环。Entity Framework 的循环最多，涉及 24% 的类，Cecil 的参与率最低，超过一半的类参与了一个循环。

即使是 NDepend 也有循环，尽管公平地说，这可能有很好的理由。第一个 NDepend 侧重于删除命名空间之间的循环，而不是类之间的循环；第二个，循环可能是在同一源文件中声明的类型之间的循环。因此，我的方法可能会在一定程度上惩罚组织良好的 C# 代码（如上文 FParsec 与 FParsecCS 讨论中所述）。

![img](https://fsharpforfunandprofit.com/posts/cycles-and-modularity-in-the-wild/Metrics_TopLevel_Participation.png)

为什么 C# 和 F# 有区别？

- 在 C# 中，没有什么能阻止你创建循环——这是意外复杂性的完美例子。事实上，你必须特别努力避免它们。
- 当然，在 F# 中，情况正好相反。你根本不容易创造循环。

## 我的业务应用程序比较

还有一个比较。作为日常工作的一部分，我用 C# 编写了许多业务应用程序，最近又用 F# 编写了。与这里列出的其他项目不同，它们非常专注于满足特定的业务需求，具有大量特定于领域的代码、自定义业务规则、特殊情况等。

这两个项目都是在截止日期前完成的，不断变化的需求和所有常见的现实世界限制都阻碍了你编写理想的代码。像我这样的大多数开发人员一样，我很想有机会整理和重构它们，但它们确实有效，业务很好，我必须转向新的事物。

不管怎样，让我们看看它们是如何相互叠加的。除了指标，我不能透露代码的任何细节，但我认为这应该足够有用。

首先来看 C# 项目：

- 它有 195 个顶级类型，大约每 2K 代码对应 1 个。与其他 C# 项目相比，应该有更多的顶级类型。事实上，我知道这是真的。与许多项目一样（这个项目已经 6 年了），只向现有类添加一个方法而不是重构它的风险较低，尤其是在截止日期之前。保持旧代码的稳定总是比使其美观更重要！其结果是，随着时间的推移，类会变得太大。
- 拥有大型类的另一面是，跨类依赖关系要少得多！它在 C# 项目中有一些更好的分数。因此，这表明依赖性并不是唯一的指标。必须保持平衡。
- 就循环依赖性而言，这对于 C# 项目来说是非常典型的。它们有很多（11 个），但最大的只涉及 8 个类。

现在让我们看看我的 F# 项目：

- 它有 30 个模块，大约每 4K 代码对应 1 个模块。与其他 F# 项目相比，这并不过分，但也许需要进行一些重构。
  - 顺便说一句，在我维护这段代码的经验中，我注意到，与 C# 代码不同，当功能请求进来时，我不觉得我必须向现有模块添加 cruft。相反，我发现在许多情况下，更快、风险更低的更改方法就是创建一个新模块，并将新功能的所有代码都放在其中。因为模块没有状态，所以函数可以存在于任何地方——它不必强制存在于同一个类中。随着时间的推移，这种方法也可能会产生自己的问题（COBOL 有人吗？），但现在，我觉得这是一股新鲜空气。
- 指标显示，每个模块都有异常多的“编写”类型（4.9）。正如我上面提到的，我认为这是细粒度 DDD 风格设计的结果。每个编写类型的代码与其他 F# 项目一致，这意味着它们不是太大也不是太小。
- 此外，正如我之前提到的，模块间的依赖关系是所有 F# 项目中最糟糕的。我知道有一些 API/服务函数依赖于几乎所有其他模块，但这可能是它们可能需要重构的线索。
  - 然而，与 C# 代码不同，我确切地知道在哪里可以找到这些问题模块。我可以相当肯定，所有这些模块都在我的应用程序的顶层，因此将出现在 Visual Studio 的模块列表的底部。我怎么能这么肯定？因为…
- 就循环依赖性而言，这对于 F# 项目来说是非常典型的。没有。

## 摘要

我出于好奇开始了这项分析——C# 和 F# 项目的组织有什么有意义的区别吗？

我很惊讶这种区别如此明显。根据这些指标，您当然可以预测程序集是用哪种语言编写的。

- **项目复杂性**。对于给定数量的指令，C# 项目可能比F#项目有更多的顶级类型（以及文件）——似乎是两倍多。
- **细粒型**。对于给定数量的模块，C# 项目的已编写类型可能比 F# 项目少，这意味着这些类型没有达到应有的细粒度。
- **依赖关系**。在 C# 项目中，类之间的依赖关系数量随着项目的大小呈线性增加。在 F# 项目中，依赖项的数量要小得多，并且保持相对平稳。
- **循环**。在 C# 项目中，循环很容易发生，除非小心避免。在 F# 项目中，循环极为罕见，即使存在，也非常小。

也许这与程序员的能力有关，而不是语言之间的差异？首先，我认为 C# 项目的质量总体上相当好——我当然不会说我能写出更好的代码！而且，特别是在两种情况下，C# 和 F# 项目是由同一个人编写的，差异仍然很明显，所以我认为这种说法站不住脚。

## 今后的工作

这种只使用二进制文件的方法可能已经走到了尽头。为了进行更准确的分析，我们还需要使用源代码中的指标（或者可能是 pdb 文件）。

例如，如果一个高的“每类型指令数”指标对应于小的源文件（简洁的代码），那么它是好的，但如果它对应于大的（臃肿的类），那么就不好了。同样，我对模块化的定义使用了顶级类型而不是源文件，这在一定程度上惩罚了 C# 而不是 F#。

所以，我并不是说这个分析是完美的（我希望分析代码中没有犯下可怕的错误！），但我认为这可能是进一步调查的有用起点。

# 更新 2013-06-15

这篇文章引起了相当大的兴趣。根据反馈，我做了以下更改：

## 已记录的程序集

- 增加了 Foq 和 Moq（应 Phil Trelford 的要求）。
- 添加了 FParsec 的 C# 组件（应 Dave Thomas 等人的要求）。
- 添加了两个 NDepend 程序集。
- 添加了我自己的两个项目，一个 C# 和一个 F#。

如您所见，添加七个新数据点（五个 C# 和两个 F# 项目）并没有改变整体分析。

## 算法更改

- 使“编写”类型的定义更加严格。排除了具有“GeneratedCodeAttribute”和求和类型的子类型作为 F# 类型。这对 F# 项目产生了影响，并在一定程度上降低了“Auth/Top”比率。

## 文本更改

- 改写了一些分析。
- 删除了 YamlDotNet 与 FParsec 的不公平比较。
- 添加了 FParsec 的 C# 组件和 F# 组件的比较。
- 添加了 Moq 和 Foq 的比较。
- 添加了我自己的两个项目的比较。