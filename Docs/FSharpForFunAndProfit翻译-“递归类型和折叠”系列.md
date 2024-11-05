# [返回主 Markdown](./FSharpForFunAndProfit翻译.md)



# 1 递归类型介绍

*Part of the "Recursive types and folds" series (*[link](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds/#series-toc)*)*

不要害怕分解形态。。。
2015年8月20日这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/recursive-types-and-folds/

在本系列中，我们将介绍递归类型以及如何使用它们，在这一过程中，我们还将介绍分解形态（catamorphisms）、尾部递归、左右折叠之间的区别等等。

## 系列内容

以下是本系列的内容：

- **第 1 部分：递归类型和分解形态（catamorphisms）介绍**
  - [一个简单的递归类型](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds/#basic-recursive-type)
  - [对所有事物进行参数化](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds/#parameterize)
  - [介绍分解形态](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds/#catamorphisms)
  - [变形的好处](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds/#benefits)
  - [创建分解形态的规则](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds/#rules)
- **第 2 部分：分解形态示例**
  - [变形示例：文件系统域](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-1b/#file-system)
  - [变形示例：产品域](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-1b/#product)
- **第 3 部分：介绍折叠**
  - [我们的分解形态实现中的一个缺陷](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2/#flaw)
  - [介绍 `fold`](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2/#fold)
  - [折叠问题](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2/#problems)
  - [将函数用作累加器](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2/#functions)
  - [介绍 `foldback`](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2/#foldback)
  - [创建折叠的规则](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2/#rules)
- **第 4 部分：了解折叠**
  - [迭代与递归](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2b/#iteration)
  - [折叠示例：文件系统域](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2b/#file-system)
  - [关于“折叠”的常见问题](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2b/#questions)
- **第 5 部分：泛型递归类型**
  - [LinkedList：一种通用的递归类型](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3/#linkedlist)
  - [使礼品领域泛型](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3/#revisiting-gift)
  - [定义泛型容器类型](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3/#container)
  - [实现礼物领域的第三种方法](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3/#another-gift)
  - [抽象还是具体？比较三种设计](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3/#compare)
- **第 6 部分：现实世界中的树**
  - [定义通用树类型](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#tree)
  - [现实世界中的树类型](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#reuse)
  - [映射树类型](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#map)
  - [示例：创建目录列表](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#listing)
  - [示例：并行 grep](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#grep)
  - [示例：将文件系统存储在数据库中](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#database)
  - [示例：将树序列化为 JSON](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#tojson)
  - [示例：从 JSON 反序列化树](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#fromjson)
  - [示例：从 JSON 反序列化树 - 带错误处理](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#json-with-error-handling)

## 基本递归类型

让我们从一个简单的例子开始——如何为礼物建模。

碰巧，我是个非常懒惰的送礼者。我总是送人一本书或巧克力。我通常会把它们包起来，有时，如果我觉得特别奢侈，我会把它们放在礼品盒里，也会加一张卡片。

让我们看看如何在类型中建模：

```F#
type Book = {title: string; price: decimal}

type ChocolateType = Dark | Milk | SeventyPercent
type Chocolate = {chocType: ChocolateType ; price: decimal}

type WrappingPaperStyle =
    | HappyBirthday
    | HappyHolidays
    | SolidColor

type Gift =
    | Book of Book
    | Chocolate of Chocolate
    | Wrapped of Gift * WrappingPaperStyle
    | Boxed of Gift
    | WithACard of Gift * message:string
```

你可以看到，其中三个箱子是指另一份 `Gift` 的“容器”。`Wrapped` 盒和 `Boxed` 盒以及 `WithACard` 盒都有一些纸和内部礼物。另外两种情况，`Book` 和 `Chocolate`，不是指礼物，可以被视为“叶子”节点或终端。

在这三种情况下，对内部 `Gift` 的引用使 `Gift` 成为递归类型。请注意，与函数不同，定义递归类型不需要 `rec` 关键字。

让我们创建一些示例值：

```F#
// a Book
let wolfHall = {title="Wolf Hall"; price=20m}

// a Chocolate
let yummyChoc = {chocType=SeventyPercent; price=5m}

// A Gift
let birthdayPresent = WithACard (Wrapped (Book wolfHall, HappyBirthday), "Happy Birthday")
//  WithACard (
//    Wrapped (
//      Book {title = "Wolf Hall"; price = 20M},
//      HappyBirthday),
//    "Happy Birthday")

// A Gift
let christmasPresent = Wrapped (Boxed (Chocolate yummyChoc), HappyHolidays)
//  Wrapped (
//    Boxed (
//      Chocolate {chocType = SeventyPercent; price = 5M}),
//    HappyHolidays)
```

在我们开始使用这些价值观之前，有一句建议…

### 准则：避免无限递归类型

我建议，在 F# 中，每个递归类型都应该由递归和非递归情况的混合组成。若并没有非递归元素，比如 `Book`，那个么该类型的所有值都必须是无限递归的。

例如，在下面的 `ImpossibleGift` 类型中，所有情况都是递归的。要构建任何一个案例，你都需要一种内在的礼物，而这种礼物也需要被构建，以此类推。

```F#
type ImpossibleGift =
    | Boxed of ImpossibleGift
    | WithACard of ImpossibleGift * message:string
```

如果你允许惰性、可变或反射，就有可能创建这样的类型。但总的来说，在 F# 这样的非惰性语言中，避免使用此类类型是一个好主意。

### 使用递归类型

公共服务公告结束——让我们开始编码吧！

首先，假设我们想要一份礼物的描述。逻辑是：

- 对于这两种非递归情况，返回一个描述该情况的字符串。
- 对于这三种递归情况，返回一个描述该情况的字符串，但也包括对内部礼物的描述。这意味着 `description` 函数将引用自身，因此必须用 `rec` 关键字标记它。

以下是一个示例实现：

```F#
let rec description gift =
    match gift with
    | Book book ->
        sprintf "'%s'" book.title
    | Chocolate choc ->
        sprintf "%A chocolate" choc.chocType
    | Wrapped (innerGift,style) ->
        sprintf "%s wrapped in %A paper" (description innerGift) style
    | Boxed innerGift ->
        sprintf "%s in a box" (description innerGift)
    | WithACard (innerGift,message) ->
        sprintf "%s with a card saying '%s'" (description innerGift) message
```

请注意 `Boxed` 情况下的递归调用：

```F#
    | Boxed innerGift ->
        sprintf "%s in a box" (description innerGift)
                               ~~~~~~~~~~~ <= recursive call
```

如果我们用示例值尝试一下，让我们看看会得到什么：

```F#
birthdayPresent |> description
// "'Wolf Hall' wrapped in HappyBirthday paper with a card saying 'Happy Birthday'"

christmasPresent |> description
// "SeventyPercent chocolate in a box wrapped in HappyHolidays paper"
```

在我看来，这相当不错。像 `HappyHolidays` 这样的东西没有空格看起来有点滑稽，但足以证明这个想法。

创建另一个函数怎么样？例如，一份礼物的总成本是多少？

对于 `totalCost`，逻辑如下：

- 书籍和巧克力在特定情况下捕获价格数据，所以使用它。
- 包装使成本增加了 `0.5`。
- 一个盒子会增加 `1.0` 的成本。
- 一张卡会增加 `2.0` 的成本。

```F#
let rec totalCost gift =
    match gift with
    | Book book ->
        book.price
    | Chocolate choc ->
        choc.price
    | Wrapped (innerGift,style) ->
        (totalCost innerGift) + 0.5m
    | Boxed innerGift ->
        (totalCost innerGift) + 1.0m
    | WithACard (innerGift,message) ->
        (totalCost innerGift) + 2.0m
```

以下是两个例子的成本：

```F#
birthdayPresent |> totalCost
// 22.5m

christmasPresent |> totalCost
// 6.5m
```

有时，人们会问盒子或包装纸里有什么。`whatsInside` 函数易于实现——只需忽略容器情况，并为非递归情况返回一些值。

```F#
let rec whatsInside gift =
    match gift with
    | Book book ->
        "A book"
    | Chocolate choc ->
        "Some chocolate"
    | Wrapped (innerGift,style) ->
        whatsInside innerGift
    | Boxed innerGift ->
        whatsInside innerGift
    | WithACard (innerGift,message) ->
        whatsInside innerGift
```

结果：

```F#
birthdayPresent |> whatsInside
// "A book"

christmasPresent |> whatsInside
// "Some chocolate"
```

所以这是一个很好的开始——三个函数，都很容易编写。

## 对所有事物进行参数化

但是，这三个函数有一些重复的代码。除了独特的应用程序逻辑外，每个函数都在进行自己的模式匹配和递归访问内部礼物的逻辑。

我们如何将导航逻辑与应用程序逻辑分开？

答案：参数化所有东西！

与往常一样，我们可以通过传递函数来参数化应用程序逻辑。在这种情况下，我们需要五个函数，每种情况一个。

这是新的参数化版本——我稍后会解释为什么我称之为 `cataGift`。

```F#
let rec cataGift fBook fChocolate fWrapped fBox fCard gift =
    match gift with
    | Book book ->
        fBook book
    | Chocolate choc ->
        fChocolate choc
    | Wrapped (innerGift,style) ->
        let innerGiftResult = cataGift fBook fChocolate fWrapped fBox fCard innerGift
        fWrapped (innerGiftResult,style)
    | Boxed innerGift ->
        let innerGiftResult = cataGift fBook fChocolate fWrapped fBox fCard innerGift
        fBox innerGiftResult
    | WithACard (innerGift,message) ->
        let innerGiftResult = cataGift fBook fChocolate fWrapped fBox fCard innerGift
        fCard (innerGiftResult,message)
```

您可以看到此函数是使用纯机械过程创建的：

- 每个函数参数（`fBook`、`fChocolate` 等）对应一个 case。
- 对于这两种非递归情况，函数参数将传递与该情况相关的所有数据。
- 对于三种递归情况，有两个步骤：
  - 首先，在 `innerGift` 上递归调用 `cataGift` 函数以获得 `innerGiftResult`
  - 然后，将与该情况相关的所有数据传递给适当的处理程序，但用 `innerGiftResult` 替换 `innerGift`。

让我们使用通用的 `cataGift` 函数重写总成本。

```F#
let totalCostUsingCata gift =
    let fBook (book:Book) =
        book.price
    let fChocolate (choc:Chocolate) =
        choc.price
    let fWrapped  (innerCost,style) =
        innerCost + 0.5m
    let fBox innerCost =
        innerCost + 1.0m
    let fCard (innerCost,message) =
        innerCost + 2.0m
    // call the catamorphism
    cataGift fBook fChocolate fWrapped fBox fCard gift
```

笔记：

- `innerGiftResult` 现在是内部礼物的总成本，因此我将其重命名为 `innerCost`。
- `totalCostUsingCata` 函数本身不是递归的，因为它使用 `cataGit` 函数，因此不再需要 `rec` 关键字。

此函数给出的结果与之前相同：

```F#
birthdayPresent |> totalCostUsingCata
// 22.5m
```

我们可以使用 `cataGift` 以相同的方式重写 `description` 函数，将 `innerGiftResult` 更改为 `innerText`。

```F#
let descriptionUsingCata gift =
    let fBook (book:Book) =
        sprintf "'%s'" book.title
    let fChocolate (choc:Chocolate) =
        sprintf "%A chocolate" choc.chocType
    let fWrapped (innerText,style) =
        sprintf "%s wrapped in %A paper" innerText style
    let fBox innerText =
        sprintf "%s in a box" innerText
    let fCard (innerText,message) =
        sprintf "%s with a card saying '%s'" innerText message
    // call the catamorphism
    cataGift fBook fChocolate fWrapped fBox fCard gift
```

结果与之前相同：

```F#
birthdayPresent |> descriptionUsingCata
// "'Wolf Hall' wrapped in HappyBirthday paper with a card saying 'Happy Birthday'"

christmasPresent |> descriptionUsingCata
// "SeventyPercent chocolate in a box wrapped in HappyHolidays paper"
```

## 介绍分解形态

我们上面写的 `cataGift` 函数被称为“分解形态（catamorphism）”，来自希腊语成分“down+shape”。在正常使用中，分解形态是一种根据递归类型的结构将其“折叠”为新值的函数。事实上，你可以把分解形态看作是一种“访问者模式”。

分解形态是一个非常强大的概念，因为它是你可以为这样的结构定义的最基本的函数。任何其他功能都可以根据它来定义。

也就是说，如果我们想创建一个签名为 `Gift -> string` 或 `Gift -> int` 的函数，我们可以通过在 `Gift` 结构中为每种情况指定一个函数来使用分解形态来创建它。

我们在上面看到了如何使用分解形态将 `totalCost` 重写为 `totalCostUsingCata`，稍后我们将看到许多其他示例。

### 分解形态和折叠

分解形态通常被称为“折叠”，但折叠的种类不止一种，所以我倾向于用“分解形态”来指代概念，用“折叠”来指一种特定的实现。

我将在后续的文章中详细讨论各种折叠，所以在这篇文章的其余部分，我将只使用“分解形态”。

### 整理执行情况

上面的 `cataGift` 实现故意冗长，这样你就可以看到每一步。一旦你理解了发生了什么，你就可以稍微简化一下。

首先，`cataGift fBook fChocolate fWrapped fBox fCard` 出现三次，每个递归情况一次。让我们给它取一个类似 `recurse` 的名字：

```F#
let rec cataGift fBook fChocolate fWrapped fBox fCard gift =
    let recurse = cataGift fBook fChocolate fWrapped fBox fCard
    match gift with
    | Book book ->
        fBook book
    | Chocolate choc ->
        fChocolate choc
    | Wrapped (innerGift,style) ->
        let innerGiftResult = recurse innerGift
        fWrapped (innerGiftResult,style)
    | Boxed innerGift ->
        let innerGiftResult = recurse innerGift
        fBox innerGiftResult
    | WithACard (innerGift,message) ->
        let innerGiftResult = recurse innerGift
        fCard (innerGiftResult,message)
```

`recurse` 函数具有简单的签名 `Gift -> 'a`，也就是说，它将 `Gift` 转换为我们需要的返回类型，因此我们可以使用它来处理各种 `innerGift` 值。

另一件事是在递归情况下用 `gift` 替换 `innerGift` ——这被称为“阴影”。好处是“外部” `gift` 对案例处理代码不再可见，因此我们不能意外地递归到其中，这将导致无限循环。

一般来说，我会避免阴影，但在这种情况下，它实际上是一种很好的做法，因为它消除了一种特别讨厌的错误。

这是清理后的版本：

```F#
let rec cataGift fBook fChocolate fWrapped fBox fCard gift =
    let recurse = cataGift fBook fChocolate fWrapped fBox fCard
    match gift with
    | Book book ->
        fBook book
    | Chocolate choc ->
        fChocolate choc
    | Wrapped (gift,style) ->
        fWrapped (recurse gift,style)
    | Boxed gift ->
        fBox (recurse gift)
    | WithACard (gift,message) ->
        fCard (recurse gift,message)
```

还有一件事。我将显式地注释返回类型并将其命名为 `'r`。在本系列的后面，我们将处理其他泛型类型，如 `'a` 和 `'b`，因此保持一致并始终为返回类型命名是有帮助的。

```F#
let rec cataGift fBook fChocolate fWrapped fBox fCard gift :'r =
//                                name the return type =>  ~~~~
```

以下是最终版本：

```F#
let rec cataGift fBook fChocolate fWrapped fBox fCard gift :'r =
    let recurse = cataGift fBook fChocolate fWrapped fBox fCard
    match gift with
    | Book book ->
        fBook book
    | Chocolate choc ->
        fChocolate choc
    | Wrapped (gift,style) ->
        fWrapped (recurse gift,style)
    | Boxed gift ->
        fBox (recurse gift)
    | WithACard (gift,message) ->
        fCard (recurse gift,message)
```

它比原始实现简单得多，也展示了像 `Wrapped (gift,style)` 这样的 case 构造函数和相应的处理程序 `fWrapped (recurse gift,style)` 之间的对称性。这让我们很好地…

### 案例构造函数和处理程序之间的关系

这是 `cataGift` 函数的签名。您可以看到，每个案例处理函数（`fBook`、`fBox` 等）都有相同的模式：一个包含该案例所有数据的输入，以及一个通用的输出类型 `'r`。

```F#
val cataGift :
  fBook:(Book -> 'r) ->
  fChocolate:(Chocolate -> 'r) ->
  fWrapped:('r * WrappingPaperStyle -> 'r) ->
  fBox:('r -> 'r) ->
  fCard:('r * string -> 'r) ->
  // input value
  gift:Gift ->
  // return value
  'r
```

另一种思考方式是：在构造函数中有 `Gift` 类型的任何地方，它都被替换为 `'r`。

例如：

- `Gift.Book` 构造器获取一本 `Book` 并返回一份 `Gift`。`fBook` 处理程序接收一个 `Book` 并返回一个 `'r`。
- `Gift.Wrapped` 构造函数接受 `Gift * WrappingPaperStyle` 并返回 `Gift`。`fWrapped` 处理程序接收 `'r * WrappingPaperStyle` 作为输入，并返回 `'r`。

以下是通过类型签名表达的关系：

```F#
// The Gift.Book constructor
Book -> Gift

// The fBook handler
Book -> 'r

// The Gift.Wrapped constructor
Gift * WrappingPaperStyle -> Gift

// The fWrapped handler
'r   * WrappingPaperStyle -> 'r

// The Gift.Boxed constructor
Gift -> Gift

// The fBox handler
'r   -> 'r
```

其余的都是如此。

## 分解形态的好处

分解形态背后有很多理论，但在实践中有什么好处呢？

为什么要费心创建像 `cataGift` 这样的特殊功能？为什么不把原来的功能放在一边呢？

原因有很多，包括：

- **重新使用**。稍后，我们将创建相当复杂的分解形态。如果你只需要把逻辑弄对一次就好了。
- **封装**。通过仅公开函数，可以隐藏数据类型的内部结构。
- **灵活性**。函数比模式匹配更灵活——它们可以组合、部分应用等。
- **映射**。有了分解形态，你可以很容易地创建将各种情况映射到新结构的函数。

诚然，这些好处中的大多数也适用于非递归类型，但递归类型往往更复杂，因此封装、灵活性等的好处相应更强。

在接下来的部分中，我们将更详细地介绍最后三点。

### 使用函数参数隐藏内部结构

第一个好处是，分解形态抽象出了内部设计。通过使用函数，客户端代码在一定程度上与内部结构隔离开来。同样，您可以将访问者模式视为面向对象世界中的类似模式。

例如，如果所有客户端都使用了分解形态函数而不是模式匹配，我可以安全地重命名案例，甚至可以稍微小心地添加或删除案例。

这里有一个例子。假设我之前为 `Gift` 设计了一个没有 `WithACard` 案例的设计。我称之为版本 1：

```F#
type Gift =
    | Book of Book
    | Chocolate of Chocolate
    | Wrapped of Gift * WrappingPaperStyle
    | Boxed of Gift
```

假设我们为该结构构建并发布了一个分解形态函数：

```F#
let rec cataGift fBook fChocolate fWrapped fBox gift :'r =
    let recurse = cataGift fBook fChocolate fWrapped fBox
    match gift with
    | Book book ->
        fBook book
    | Chocolate choc ->
        fChocolate choc
    | Wrapped (gift,style) ->
        fWrapped (recurse gift,style)
    | Boxed gift ->
        fBox (recurse gift)
```

请注意，这只有四个函数参数。

现在假设 `Gift` 的版本 2 出现了，它添加了 `WithACard` 案例：

```F#
type Gift =
    | Book of Book
    | Chocolate of Chocolate
    | Wrapped of Gift * WrappingPaperStyle
    | Boxed of Gift
    | WithACard of Gift * message:string
```

现在有五个案例。

通常，当我们添加一个新案例时，我们想打破所有客户，迫使他们处理新案例。

但有时，我们不会。因此，我们可以通过默默地处理额外的案例来保持与原始 `cataGift` 的兼容性，如下所示：

```F#
/// Uses Gift_V2 but is still backwards compatible with the earlier "cataGift".
let rec cataGift fBook fChocolate fWrapped fBox gift :'r =
    let recurse = cataGift fBook fChocolate fWrapped fBox
    match gift with
    | Book book ->
        fBook book
    | Chocolate choc ->
        fChocolate choc
    | Wrapped (gift,style) ->
        fWrapped (recurse gift,style)
    | Boxed gift ->
        fBox (recurse gift)
    // pass through the new case silently
    | WithACard (gift,message) ->
        recurse gift
```

此函数仍然只有四个函数参数——`WithACard` 情况没有特殊行为。

有许多其他兼容方式，例如返回默认值。关键在于，客户并未意识到这一变化。

### 旁白：使用活动模式隐藏数据

当我们讨论隐藏类型的结构时，我应该提到的是，你也可以使用活动模式来做到这一点。

例如，我们可以为前四个案例创建一个活动模式，忽略 `WithACard` 案例。

```F#
let rec (|Book|Chocolate|Wrapped|Boxed|) gift =
    match gift with
    | Gift.Book book ->
        Book book
    | Gift.Chocolate choc ->
        Chocolate choc
    | Gift.Wrapped (gift,style) ->
        Wrapped (gift,style)
    | Gift.Boxed gift ->
        Boxed gift
    | Gift.WithACard (gift,message) ->
        // ignore the message and recurse into the gift
        (|Book|Chocolate|Wrapped|Boxed|) gift
```

客户端可以在不知道新案例存在的情况下对四种案例进行模式匹配：

```F#
let rec whatsInside gift =
    match gift with
    | Book book ->
        "A book"
    | Chocolate choc ->
        "Some chocolate"
    | Wrapped (gift,style) ->
        whatsInside gift
    | Boxed gift ->
        whatsInside gift
```

### 案例处理功能 vs. 模式匹配

分解形态使用函数参数，如上所述，由于组合、部分应用等工具，函数比模式匹配更灵活。

这里有一个例子，忽略所有“容器”案例，只处理“内容”案例。

```F#
let handleContents fBook fChocolate gift =
    let fWrapped (innerGiftResult,style) =
        innerGiftResult
    let fBox innerGiftResult =
        innerGiftResult
    let fCard (innerGiftResult,message) =
        innerGiftResult

    // call the catamorphism
    cataGift fBook fChocolate fWrapped fBox fCard gift
```

在这里，它正在使用中，剩下的两个案例使用管道“内联”处理：

```F#
birthdayPresent
|> handleContents
    (fun book -> "The book you wanted for your birthday")
    (fun choc -> "Your fave chocolate")
// Result => "The book you wanted for your birthday"

christmasPresent
|> handleContents
    (fun book -> "The book you wanted for Christmas")
    (fun choc -> "Don't eat too much over the holidays!")
// Result => "Don't eat too much over the holidays!"
```

当然，这可以通过模式匹配来实现，但能够直接使用现有的 `cataGift` 函数会使工作更容易。

### 利用分解形态进行映射

我上面说过，分解形态是一个将递归类型“折叠”为新值的函数。例如，在 `totalCost` 中，递归礼物结构被分解为单个成本值。

但是“单个值”不仅仅是一个原始值，它也可以是一个复杂的结构，比如另一个递归结构。

事实上，分解形态非常适合将一种结构映射到另一种结构上，特别是当它们非常相似的时候。

例如，假设我有一个喜欢巧克力的室友，他会悄悄地拿走并吃掉礼物中的任何巧克力，不碰包装，但会丢弃盒子和礼品卡。

最后剩下的是“礼物减去巧克力”，我们可以这样建模：

```F#
type GiftMinusChocolate =
    | Book of Book
    | Apology of string
    | Wrapped of GiftMinusChocolate * WrappingPaperStyle
```

我们可以很容易地从礼物映射到礼品迷你巧克力，因为案例几乎是平行的。

- 一本 `Book` 原封不动地通过。
- `Chocolate` 被吃掉，取而代之的是 `Apology`。
- `Wrapped` 的箱子完好无损地通过了。
- `Box` 和 `WithACard` 案例被忽略。

代码如下：

```F#
let removeChocolate gift =
    let fBook (book:Book) =
        Book book
    let fChocolate (choc:Chocolate) =
        Apology "sorry I ate your chocolate"
    let fWrapped (innerGiftResult,style) =
        Wrapped (innerGiftResult,style)
    let fBox innerGiftResult =
        innerGiftResult
    let fCard (innerGiftResult,message) =
        innerGiftResult
    // call the catamorphism
    cataGift fBook fChocolate fWrapped fBox fCard gift
```

如果我们测试…

```F#
birthdayPresent |> removeChocolate
// GiftMinusChocolate =
//     Wrapped (Book {title = "Wolf Hall"; price = 20M}, HappyBirthday)

christmasPresent |> removeChocolate
// GiftMinusChocolate =
//     Wrapped (Apology "sorry I ate your chocolate", HappyHolidays)
```

### 深度复制

还有一件事。还记得每个案例处理函数都需要与该案例相关的数据吗？这意味着我们可以只使用原始的case构造函数作为函数！

为了理解我的意思，让我们定义一个名为 `deepCopy` 的函数来克隆原始值。每个案例处理程序只是相应的案例构造函数：

```F#
let deepCopy gift =
    let fBook book =
        Book book
    let fChocolate (choc:Chocolate) =
        Chocolate choc
    let fWrapped (innerGiftResult,style) =
        Wrapped (innerGiftResult,style)
    let fBox innerGiftResult =
        Boxed innerGiftResult
    let fCard (innerGiftResult,message) =
        WithACard (innerGiftResult,message)
    // call the catamorphism
    cataGift fBook fChocolate fWrapped fBox fCard gift
```

我们可以通过删除每个处理程序的冗余参数来进一步简化这一点：

```F#
let deepCopy gift =
    let fBook = Book
    let fChocolate = Chocolate
    let fWrapped = Wrapped
    let fBox = Boxed
    let fCard = WithACard
    // call the catamorphism
    cataGift fBook fChocolate fWrapped fBox fCard gift
```

你可以自己测试一下：

```F#
christmasPresent |> deepCopy
// Result =>
//   Wrapped (
//    Boxed (Chocolate {chocType = SeventyPercent; price = 5M;}),
//    HappyHolidays)
```

这就引出了另一种思考分解形态的方式：

- 分解形态是递归类型的一个函数，当你传入该类型的 case 构造函数时，你会得到一个“clone”函数。

### 一次完成映射和转换

`deepCopy` 的一个微小变体允许我们递归遍历对象，并在这样做时更改其部分。

例如，假设我不喜欢牛奶巧克力。好吧，我可以写一个函数，将礼物升级为质量更好的巧克力，并让所有其他盒子保持原样。

```F#
let upgradeChocolate gift =
    let fBook = Book
    let fChocolate (choc:Chocolate) =
        Chocolate {choc with chocType = SeventyPercent}
    let fWrapped = Wrapped
    let fBox = Boxed
    let fCard = WithACard
    // call the catamorphism
    cataGift fBook fChocolate fWrapped fBox fCard gift
```

它在这里使用：

```F#
// create some chocolate I don't like
let cheapChoc = Boxed (Chocolate {chocType=Milk; price=5m})

// upgrade it!
cheapChoc |> upgradeChocolate
// Result =>
//   Boxed (Chocolate {chocType = SeventyPercent; price = 5M})
```

如果你认为这开始闻起来像 `map`，你是对的。我们将在第六篇文章中讨论泛型映射，作为泛型递归类型讨论的一部分。

## 创建分解形态的规则

我们在上面看到，创建分解形态是一个机械过程：

- 创建一个函数参数来处理结构中的每种情况。
- 对于非递归情况，将与该情况相关的所有数据传递给函数参数。
- 对于递归情况，执行两个步骤：
  - 首先，对嵌套值递归调用分解形态。
  - 然后将与该情况相关的所有数据传递给处理程序，但分解的结果将替换原始嵌套值。
- 现在让我们看看我们是否可以应用这些规则在其他域中创建分解形态。

## 摘要

我们在这篇文章中看到了如何定义递归类型，并介绍了分解形态（catamorphisms）。

在下一篇文章中，我们将使用这些规则为其他一些域创建分解形态。

到时候见！

*这篇文章的源代码可以在这里找到。*



# 2 分解形态示例

*Part of the "Recursive types and folds" series (*[link](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-1b/#series-toc)*)*

将规则应用于其他域
2015年8月21日这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-1b/

这篇文章是系列文章中的第二篇。

在上一篇文章中，我介绍了“分解形态（catamorphisms）”，这是一种为递归类型创建函数的方法，并列出了一些可用于机械实现它们的规则。在这篇文章中，我们将使用这些规则为其他一些域实现分解形态。

## 系列内容

以下是本系列的内容：

- **第 1 部分：递归类型和分解形态（catamorphisms）介绍**
  - [一个简单的递归类型](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds/#basic-recursive-type)
  - [对所有事物进行参数化](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds/#parameterize)
  - [介绍分解形态](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds/#catamorphisms)
  - [变形的好处](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds/#benefits)
  - [创建分解形态的规则](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds/#rules)
- **第 2 部分：分解形态示例**
  - [变形示例：文件系统域](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-1b/#file-system)
  - [变形示例：产品域](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-1b/#product)
- **第 3 部分：介绍折叠**
  - [我们的分解形态实现中的一个缺陷](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2/#flaw)
  - [介绍 `fold`](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2/#fold)
  - [折叠问题](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2/#problems)
  - [将函数用作累加器](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2/#functions)
  - [介绍 `foldback`](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2/#foldback)
  - [创建折叠的规则](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2/#rules)
- **第 4 部分：了解折叠**
  - [迭代与递归](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2b/#iteration)
  - [折叠示例：文件系统域](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2b/#file-system)
  - [关于“折叠”的常见问题](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2b/#questions)
- **第 5 部分：泛型递归类型**
  - [LinkedList：一种通用的递归类型](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3/#linkedlist)
  - [使礼品领域泛型](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3/#revisiting-gift)
  - [定义泛型容器类型](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3/#container)
  - [实现礼物领域的第三种方法](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3/#another-gift)
  - [抽象还是具体？比较三种设计](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3/#compare)
- **第 6 部分：现实世界中的树**
  - [定义通用树类型](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#tree)
  - [现实世界中的树类型](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#reuse)
  - [映射树类型](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#map)
  - [示例：创建目录列表](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#listing)
  - [示例：并行 grep](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#grep)
  - [示例：将文件系统存储在数据库中](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#database)
  - [示例：将树序列化为 JSON](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#tojson)
  - [示例：从 JSON 反序列化树](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#fromjson)
  - [示例：从 JSON 反序列化树 - 带错误处理](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#json-with-error-handling)

## 创建分解形态的规则

我们在上一篇文章中看到，创建分解形态是一个机械过程，规则是：

- 创建一个函数参数来处理结构中的每种情况。
- 对于非递归情况，将与该情况相关的所有数据传递给函数参数。
- 对于递归情况，执行两个步骤：
  - 首先，对嵌套值递归调用分解形态。
  - 然后将与该情况相关的所有数据传递给处理程序，但分解的结果将替换原始嵌套值。

现在让我们看看我们是否可以应用这些规则在其他域中创建分解形态。

## 分解形态示例：文件系统域

让我们从一个非常粗糙的文件系统模型开始：

- 每个文件都有一个名称和大小。
- 每个目录都有一个名称、大小和子项列表。

我可以这样建模：

```F#
type FileSystemItem =
    | File of File
    | Directory of Directory
and File = {name:string; fileSize:int}
and Directory = {name:string; dirSize:int; subitems:FileSystemItem list}
```

我承认这是一个相当糟糕的模型，但对于这个例子来说，它已经足够好了！

好的，这里有一些示例文件和目录：

```F#
let readme = File {name="readme.txt"; fileSize=1}
let config = File {name="config.xml"; fileSize=2}
let build  = File {name="build.bat"; fileSize=3}
let src = Directory {name="src"; dirSize=10; subitems=[readme; config; build]}
let bin = Directory {name="bin"; dirSize=10; subitems=[]}
let root = Directory {name="root"; dirSize=5; subitems=[src; bin]}
```

是时候创造分解形态了！

让我们先看看签名，找出我们需要什么。

`File` 构造函数接受一个 `File` 并返回一个 `FileSystemItem`。根据上述指南，`File` 案例的处理程序需要具有签名 `File->'r`。

```F#
// case constructor
File  : File -> FileSystemItem

// function parameter to handle File case
fFile : File -> 'r
```

这很简单。让我们构建一个 `cataFS` 的初步框架，我称之为：

```F#
let rec cataFS fFile fDir item :'r =
    let recurse = cataFS fFile fDir
    match item with
    | File file ->
        fFile file
    | Directory dir ->
        // to do
```

`Directory` 的情况更为复杂。如果我们天真地应用了上述准则，`Directory` 情况的处理程序将具有签名 `Directory -> 'r`，但这是不正确的，因为 `Directory` 记录本身包含一个 `FileSystemItem`，也需要用 `'r` 替换。我们如何做到这一点？

一种方法是将 `Directory` 记录“分解”为 `(string,int,FileSystemItem list)` 的元组，然后在其中也用 `'r` 替换 `FileSystemItem`。

换句话说，我们有这样一系列的转换：

```F#
// case constructor (Directory as record)
Directory : Directory -> FileSystemItem

// case constructor (Directory unpacked as tuple)
Directory : (string, int, FileSystemItem list) -> FileSystemItem
//   replace with 'r ===> ~~~~~~~~~~~~~~          ~~~~~~~~~~~~~~

// function parameter to handle Directory case
fDir :      (string, int, 'r list)             -> 'r
```

另一个问题是，与 Directory 案例相关的数据是 `FileSystemItem`s 列表。我们如何将其转换为 `'r`s 列表？

好吧，`recurse` 助手将 `FileSystemItem` 转换为 `'r`，所以我们可以在 `recurse` 中使用 `List.map` 作为映射函数，这将为我们提供所需的 `'r`s 列表！

综合起来，我们得到了这样的实现：

```F#
let rec cataFS fFile fDir item :'r =
    let recurse = cataFS fFile fDir
    match item with
    | File file ->
        fFile file
    | Directory dir ->
        let listOfRs = dir.subitems |> List.map recurse
        fDir (dir.name,dir.dirSize,listOfRs)
```

如果我们看看类型签名，我们可以看到它正是我们想要的：

```F#
val cataFS :
    fFile : (File -> 'r) ->
    fDir  : (string * int * 'r list -> 'r) ->
    // input value
    FileSystemItem ->
    // return value
    'r
```

所以我们完了。设置起来有点复杂，但一旦构建完成，我们就有了一个很好的可重用函数，可以作为许多其他函数的基础。

### 文件系统域：`totalSize` 例子

好吧，让我们在实践中使用它。

首先，我们可以很容易地定义一个 `totalSize` 函数，它返回一个项目及其所有子项目的总大小：

```F#
let totalSize fileSystemItem =
    let fFile (file:File) =
        file.fileSize
    let fDir (name,size,subsizes) =
        (List.sum subsizes) + size
    cataFS fFile fDir fileSystemItem
```

结果如下：

```F#
readme |> totalSize  // 1
src |> totalSize     // 16 = 10 + (1 + 2 + 3)
root |> totalSize    // 31 = 5 + 16 + 10
```

### 文件系统域：`largestFile` 例子

一个更复杂的函数怎么样，比如“树中最大的文件是什么？”

在我们开始这个之前，让我们想想它应该返回什么。也就是说，`'r` 是什么？

你可能会认为这只是一个 `File`。但是，如果子目录为空并且没有文件怎么办？

因此，让我们将 `'r` 设置为 `File option`。

`File` 案例的函数应该返回 `Some File`，然后：

```F#
let fFile (file:File) =
    Some file
```

`Directory` 案例的功能需要更多思考：

- 如果子文件列表为空，则返回 `None`
- 如果子文件列表非空，则返回最大的子文件

```F#
let fDir (name,size,subfiles) =
    match subfiles with
    | [] ->
        None  // empty directory
    | subfiles ->
        // return largest one
```

但请记住，`'r` 不是 `File`，而是 `File option`。这意味着 `subfiles` 不是文件列表，而是 `File option` 列表。

现在，我们如何找到其中最大的一个？我们可能想使用 `List.maxBy` 并传入大小。但是文件选项的大小是多少？

让我们使用以下逻辑编写一个辅助函数来提供 `File option` 的大小：

- 如果 `File option` 为 `None`，则返回 0
- 否则，返回选项中文件的大小

代码如下：

```F#
// helper to provide a default if missing
let ifNone deflt opt =
    defaultArg opt deflt

// get the file size of an option
let fileSize fileOpt =
    fileOpt
    |> Option.map (fun file -> file.fileSize)
    |> ifNone 0
```

把它们放在一起，我们就有了 `largestFile` 函数：

```F#
let largestFile fileSystemItem =

    // helper to provide a default if missing
    let ifNone deflt opt =
        defaultArg opt deflt

    // helper to get the size of a File option
    let fileSize fileOpt =
        fileOpt
        |> Option.map (fun file -> file.fileSize)
        |> ifNone 0

    // handle File case
    let fFile (file:File) =
        Some file

    // handle Directory case
    let fDir (name,size,subfiles) =
        match subfiles with
        | [] ->
            None  // empty directory
        | subfiles ->
            // find the biggest File option using the helper
            subfiles
            |> List.maxBy fileSize

    // call the catamorphism
    cataFS fFile fDir fileSystemItem
```

如果我们测试它，我们会得到预期的结果：

```F#
readme |> largestFile
// Some {name = "readme.txt"; fileSize = 1}

src |> largestFile
// Some {name = "build.bat"; fileSize = 3}

bin |> largestFile
// None

root |> largestFile
// Some {name = "build.bat"; fileSize = 3}
```

同样，设置起来有点棘手，但只不过是我们必须从头开始编写它而不使用任何分解形态。

## 分解形态示例：产品域

让我们处理一个稍微复杂一些的域。这一次，想象一下我们制造和销售某种产品：

- 有些产品是通过可选供应商购买的。
- 有些产品是在我们的场所制造的，由子组件构建而成，其中子组件是一定数量的另一种产品。

以下是建模为类型的域：

```F#
type Product =
    | Bought of BoughtProduct
    | Made of MadeProduct
and BoughtProduct = {
    name : string
    weight : int
    vendor : string option }
and MadeProduct = {
    name : string
    weight : int
    components:Component list }
and Component = {
    qty : int
    product : Product }
```

请注意，这些类型是可变递归的。`Product` 引用 `MadeProduct`，`MadeProduct` 引用 `Component`，组件又引用 `Product`。

以下是一些示例产品：

```F#
let label =
    Bought {name="label"; weight=1; vendor=Some "ACME"}
let bottle =
    Bought {name="bottle"; weight=2; vendor=Some "ACME"}
let formulation =
    Bought {name="formulation"; weight=3; vendor=None}

let shampoo =
    Made {name="shampoo"; weight=10; components=
    [
    {qty=1; product=formulation}
    {qty=1; product=bottle}
    {qty=2; product=label}
    ]}

let twoPack =
    Made {name="twoPack"; weight=5; components=
    [
    {qty=2; product=shampoo}
    ]}
```

现在要设计分解形态，我们需要做的就是在所有构造函数中将 `Product` 类型替换为 `'r`。

就像前面的例子一样，`Bought` 案例很简单：

```F#
// case constructor
Bought  : BoughtProduct -> Product

// function parameter to handle Bought case
fBought : BoughtProduct -> 'r
```

`Made` 案例更棘手。我们需要将 `MadeProduct` 扩展为元组。但是这个元组包含一个 `Component`，所以我们也需要扩展它。最后我们得到内部的 `Product`，然后我们可以用 `'r` 机械地替换它。

以下是转换的顺序：

```F#
// case constructor
Made  : MadeProduct -> Product

// case constructor (MadeProduct unpacked as tuple)
Made  : (string,int,Component list) -> Product

// case constructor (Component unpacked as tuple)
Made  : (string,int,(int,Product) list) -> Product
//  replace with 'r ===> ~~~~~~~           ~~~~~~~

// function parameter to handle Made case
fMade : (string,int,(int,'r) list)      -> 'r
```

在实现 `cataProduct` 函数时，我们需要像以前一样进行映射，将 `Component` 列表转换为 `(int, 'r)` 列表。

我们需要一个帮手：

```F#
// Converts a Component into a (int * 'r) tuple
let convertComponentToTuple comp =
    (comp.qty,recurse comp.product)
```

您可以看到，这使用 `recurse` 函数将内部产品 (`comp.product`) 转换为 `'r`，然后使元组为 `int * 'r`。

有了 `convertComponentToTuple`，我们可以使用 `List.map` 将所有组件转换为元组：

```F#
let componentTuples =
    made.components
    |> List.map convertComponentToTuple
```

`componentTuples` 是 `(int * 'r)` 的列表，这正是 `fMade` 函数所需要的。

`cataProduct` 的完整实现如下：

```F#
let rec cataProduct fBought fMade product :'r =
    let recurse = cataProduct fBought fMade

    // Converts a Component into a (int * 'r) tuple
    let convertComponentToTuple comp =
        (comp.qty,recurse comp.product)

    match product with
    | Bought bought ->
        fBought bought
    | Made made ->
        let componentTuples =  // (int * 'r) list
            made.components
            |> List.map convertComponentToTuple
        fMade (made.name,made.weight,componentTuples)
```

### 产品领域：`productWeight` 例子

我们现在可以使用 `cataProduct` 来计算重量。

```F#
let productWeight product =

    // handle Bought case
    let fBought (bought:BoughtProduct) =
        bought.weight

    // handle Made case
    let fMade (name,weight,componentTuples) =
        // helper to calculate weight of one component tuple
        let componentWeight (qty,weight) =
            qty * weight
        // add up the weights of all component tuples
        let totalComponentWeight =
            componentTuples
            |> List.sumBy componentWeight
        // and add the weight of the Made case too
        totalComponentWeight + weight

    // call the catamorphism
    cataProduct fBought fMade product
```

让我们以交互方式测试它，以确保它有效：

```F#
label |> productWeight    // 1
shampoo |> productWeight  // 17 = 10 + (2x1 + 1x2 + 1x3)
twoPack |> productWeight  // 39 = 5  + (2x17)
```

正如我们所料。

尝试从头开始实现 `productWeight`，而不使用像 `cataProduct` 这样的辅助函数。同样，这是可行的，但你可能会浪费大量时间来正确理解递归逻辑。

### 产品领域：`mostUsedVendor`例子

现在让我们做一个更复杂的函数。最常用的供应商是什么？

逻辑很简单：每次产品引用供应商时，我们都会给该供应商一分，得分最高的供应商获胜。

让我们再次思考它应该返回什么。也就是说，`'r` 是什么？

你可能会认为这只是某种分数，但我们还需要知道供应商的名称。好吧，那就来一个元组。但是，如果没有供应商呢？

因此，让我们将 `'r` 设置为 `VendorScore option`，在这里我们将创建一个小类型 `VendorScore`，而不是使用元组。

```F#
type VendorScore = {vendor:string; score:int}
```

我们还将定义一些助手来轻松地从 `VendorScore` 获取数据：

```F#
let vendor vs = vs.vendor
let score vs = vs.score
```

现在，在获得整个树的结果之前，您无法确定使用最多的供应商，因此 `Bought` 案例和 `Made` 案例都需要返回一个列表，当我们递归树时可以添加到该列表中。然后，在获得所有分数后，我们将按降序排序，找到得分最高的供应商。

所以我们必须制作一个供应商评分列表，而不仅仅是一个选项！

那么，`Bought` 案例的逻辑是：

- 如果供应商存在，则返回 score = 1的 `VendorScore`，但作为一个元素列表而不是单个项目。
- 如果缺少供应商，则返回一个空列表。

```F#
let fBought (bought:BoughtProduct) =
    // set score = 1 if there is a vendor
    bought.vendor
    |> Option.map (fun vendor -> {vendor = vendor; score = 1} )
    // => a VendorScore option
    |> Option.toList
    // => a VendorScore list
```

`Made` 案例的功能更为复杂。

- 如果子分数列表为空，则返回一个空列表。
- 如果子分数列表非空，我们将按供应商对其求和，然后返回新列表。

但是传递给fMade函数的子结果列表将不是子分数列表，而是元组列表，`qty * 'r`，其中 `'r` 是 `VendorScore list`。复杂！

那么我们需要做的是：

- 将 `qty * 'r` 转换为 `'r`，因为在这种情况下我们不关心数量。我们现在有一个 `VendorScore list`。我们可以使用 `List.map snd` 来实现这一点。
- 但现在我们将有一个 `VendorScore list`。我们可以使用 `list.collect` 将列表列表展平为简单列表。事实上，使用 `List.collect snd` 可以一次性完成这两个步骤。
- 按供应商将此列表分组，以便我们有一个 `key=vendor; values=VendorScore list` 元组。
- 将每个供应商的得分（`values=VendorScore list`）汇总为一个值，这样我们就有了一个 `key=vendor; values=VendorScore` 元组的列表。

此时，`cata` 函数将返回 `VendorScore list`。要获得最高分数，请使用 `List.sortByDescending`，然后使用 `List.tryHead`。请注意，`maxBy` 不起作用，因为列表可能为空。

以下是完整的 `mostUsedVendor` 函数：

```F#
let mostUsedVendor product =

    let fBought (bought:BoughtProduct) =
        // set score = 1 if there is a vendor
        bought.vendor
        |> Option.map (fun vendor -> {vendor = vendor; score = 1} )
        // => a VendorScore option
        |> Option.toList
        // => a VendorScore list

    let fMade (name,weight,subresults) =
        // subresults are a list of (qty * VendorScore list)

        // helper to get sum of scores
        let totalScore (vendor,vendorScores) =
            let totalScore = vendorScores |> List.sumBy score
            {vendor=vendor; score=totalScore}

        subresults
        // => a list of (qty * VendorScore list)
        |> List.collect snd  // ignore qty part of subresult
        // => a list of VendorScore
        |> List.groupBy vendor
        // second item is list of VendorScore, reduce to sum
        |> List.map totalScore
        // => list of VendorScores

    // call the catamorphism
    cataProduct fBought fMade product
    |> List.sortByDescending score  // find highest score
    // return first, or None if list is empty
    |> List.tryHead
```

现在让我们测试一下：

```F#
label |> mostUsedVendor
// Some {vendor = "ACME"; score = 1}

formulation |> mostUsedVendor
// None

shampoo |> mostUsedVendor
// Some {vendor = "ACME"; score = 2}

twoPack |> mostUsedVendor
// Some {vendor = "ACME"; score = 2}
```

当然，这并不是 `fMade` 的唯一可能实现。我本可以使用 `List.fold` 一次性完成整个过程，但这个版本似乎是最明显、最可读的实现。

同样，我本可以完全避免使用 `cataProduct`，并从头开始编写 `mostUsedVendor`。如果性能是一个问题，那么这可能是一种更好的方法，因为泛型分解形态会创建中间值（如 `qty * VendorScore option` 列表），这些值过于笼统，可能会造成浪费。

另一方面，通过使用分解形态，我可以只关注计数逻辑而忽略递归逻辑。

因此，与往常一样，您应该考虑重用与从头开始创建的利弊；一次性编写通用代码并以标准化的方式使用它的好处，与自定义代码的性能但额外的努力（和潜在的麻烦）相比。

## 摘要

我们在这篇文章中看到了如何定义递归类型，并介绍了分解形态。

我们还看到了分解形态的一些用法：

- 任何“折叠”递归类型的函数，如 `Gift->'r`，都可以根据该类型的分解形态来编写。
- 可以使用分解形态来隐藏类型的内部结构。
- 通过调整处理每种情况的函数，可以使用分解形态来创建从一种类型到另一种类型的映射。
- 通过传入类型的case构造函数，可以使用分解形态来创建原始值的克隆。

但在分解形态的土地上，一切都不是完美的。事实上，此页面上的所有分解形态实现都有一个潜在的严重缺陷。

在下一篇文章中，我们将看到它们可能会出现什么问题，如何修复它们，并在此过程中查看各种“折叠”。

到时候见！

*这篇文章的源代码可以在这里找到。*

*更新：修复了 Paul Schnapp 在评论中指出的 `mostUsedVendor` 中的逻辑错误。谢谢你，保罗！*

# 3 介绍折叠

*Part of the "Recursive types and folds" series (*[link](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2/#series-toc)*)*

通过递归数据结构线程化状态
2015年8月22日这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2/

这篇文章是系列文章中的第三篇。

在第一篇文章中，我介绍了“分解形态”，这是一种为递归类型创建函数的方法，在第二篇文章中我们创建了一些分解形态实现。

但在上一篇文章的末尾，我注意到到到目前为止，所有的分解形态实现都有一个潜在的严重缺陷。

在这篇文章中，我们将看看这个缺陷以及如何解决它，并在这个过程中看看折叠、尾部递归以及“左折叠”和“右折叠”之间的区别。

## 系列内容

以下是本系列的内容：

- **第 1 部分：递归类型和分解形态（catamorphisms）介绍**
  - [一个简单的递归类型](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds/#basic-recursive-type)
  - [对所有事物进行参数化](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds/#parameterize)
  - [介绍分解形态](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds/#catamorphisms)
  - [变形的好处](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds/#benefits)
  - [创建分解形态的规则](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds/#rules)
- **第 2 部分：分解形态示例**
  - [变形示例：文件系统域](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-1b/#file-system)
  - [变形示例：产品域](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-1b/#product)
- **第 3 部分：介绍折叠**
  - [我们的分解形态实现中的一个缺陷](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2/#flaw)
  - [介绍 `fold`](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2/#fold)
  - [折叠问题](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2/#problems)
  - [将函数用作累加器](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2/#functions)
  - [介绍 `foldback`](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2/#foldback)
  - [创建折叠的规则](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2/#rules)
- **第 4 部分：了解折叠**
  - [迭代与递归](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2b/#iteration)
  - [折叠示例：文件系统域](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2b/#file-system)
  - [关于“折叠”的常见问题](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2b/#questions)
- **第 5 部分：泛型递归类型**
  - [LinkedList：一种通用的递归类型](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3/#linkedlist)
  - [使礼品领域泛型](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3/#revisiting-gift)
  - [定义泛型容器类型](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3/#container)
  - [实现礼物领域的第三种方法](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3/#another-gift)
  - [抽象还是具体？比较三种设计](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3/#compare)
- **第 6 部分：现实世界中的树**
  - [定义通用树类型](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#tree)
  - [现实世界中的树类型](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#reuse)
  - [映射树类型](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#map)
  - [示例：创建目录列表](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#listing)
  - [示例：并行 grep](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#grep)
  - [示例：将文件系统存储在数据库中](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#database)
  - [示例：将树序列化为 JSON](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#tojson)
  - [示例：从 JSON 反序列化树](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#fromjson)
  - [示例：从 JSON 反序列化树 - 带错误处理](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#json-with-error-handling)

## 我们的分解形态实现中的一个缺陷

在我们研究这个缺陷之前，让我们先回顾一下递归类型 `Gift` 和我们为它创建的相关的分解形态 `cataGift`。

这是领域：

```F#
type Book = {title: string; price: decimal}

type ChocolateType = Dark | Milk | SeventyPercent
type Chocolate = {chocType: ChocolateType ; price: decimal}

type WrappingPaperStyle =
    | HappyBirthday
    | HappyHolidays
    | SolidColor

type Gift =
    | Book of Book
    | Chocolate of Chocolate
    | Wrapped of Gift * WrappingPaperStyle
    | Boxed of Gift
    | WithACard of Gift * message:string
```

以下是我们将在本文中使用的一些示例值：

```F#
// A Book
let wolfHall = {title="Wolf Hall"; price=20m}
// A Chocolate
let yummyChoc = {chocType=SeventyPercent; price=5m}
// A Gift
let birthdayPresent = WithACard (Wrapped (Book wolfHall, HappyBirthday), "Happy Birthday")
// A Gift
let christmasPresent = Wrapped (Boxed (Chocolate yummyChoc), HappyHolidays)
```

这是分解形态（catamorphism）：

```F#
let rec cataGift fBook fChocolate fWrapped fBox fCard gift :'r =
    let recurse = cataGift fBook fChocolate fWrapped fBox fCard
    match gift with
    | Book book ->
        fBook book
    | Chocolate choc ->
        fChocolate choc
    | Wrapped (gift,style) ->
        fWrapped (recurse gift,style)
    | Boxed gift ->
        fBox (recurse gift)
    | WithACard (gift,message) ->
        fCard (recurse gift,message)
```

这是一个使用 `cataGift` 构建的 `totalCostUsingCata` 函数：

```F#
let totalCostUsingCata gift =
    let fBook (book:Book) =
        book.price
    let fChocolate (choc:Chocolate) =
        choc.price
    let fWrapped  (innerCost,style) =
        innerCost + 0.5m
    let fBox innerCost =
        innerCost + 1.0m
    let fCard (innerCost,message) =
        innerCost + 2.0m
    // call the catamorphism
    cataGift fBook fChocolate fWrapped fBox fCard gift
```

### 缺陷是什么？

那么，这种实现方式有什么问题呢？让我们进行压力测试，找出答案！

我们要做的是在一个 `Box` 里的 `Box` 里的很多次创建一个 `Box` ，看看会发生什么。

这里有一个创建嵌套框的小助手函数：

```F#
let deeplyNestedBox depth =
    let rec loop depth boxSoFar =
        match depth with
        | 0 -> boxSoFar
        | n -> loop (n-1) (Boxed boxSoFar)
    loop depth (Book wolfHall)
```

让我们尝试一下，以确保它有效：

```F#
deeplyNestedBox 5
// Boxed (Boxed (Boxed (Boxed (Boxed (Book {title = "Wolf Hall"; price = 20M})))))

deeplyNestedBox 10
//  Boxed(Boxed(Boxed(Boxed(Boxed
//   (Boxed(Boxed(Boxed(Boxed(Boxed(Book {title = "Wolf Hall";price = 20M}))))))))))
```

现在尝试使用这些嵌套很深的框运行 `totalCostUsingCata`：

```F#
deeplyNestedBox 10 |> totalCostUsingCata       // OK     30.0M
deeplyNestedBox 100 |> totalCostUsingCata      // OK    120.0M
deeplyNestedBox 1000 |> totalCostUsingCata     // OK   1020.0M
```

到现在为止，一直都还不错。

但是，如果我们使用更大的数字，我们很快就会遇到堆栈溢出异常：

```F#
deeplyNestedBox 10000 |> totalCostUsingCata  // Stack overflow?
deeplyNestedBox 100000 |> totalCostUsingCata // Stack overflow?
```

导致错误的确切数字取决于环境、可用内存等。但可以肯定的是，当你开始使用较大的数字时，你会遇到它。

为什么会这样？

### 深度递归的问题

回想一下，盒装箱（`fBox`）的成本定义为 `innerCost + 1.0m`。内部成本是什么？这也是另一个盒子，所以我们最终得到了一系列类似这样的计算：

```F#
innerCost + 1.0m where innerCost =
  innerCost2 + 1.0m where innerCost2 =
    innerCost3 + 1.0m where innerCost3 =
      innerCost4 + 1.0m where innerCost4 =
        ...
        innerCost999 + 1.0m where innerCost999 =
          innerCost1000 + 1.0m where innerCost1000 =
            book.price
```

换句话说，在计算 `innerCost999` 之前，必须先计算 `inneCost1000`，在计算顶级 `innerCost` 之前，必须计算 999 其他内部成本。

在计算该级别之前，每个级别都在等待其内部成本的计算。

所有这些未完成的计算都堆积在一起，等待内部计算完成。当你有太多的时候呢？嘣！堆栈溢出！

### 堆栈溢出的解决方案

这个问题的解决方案很简单。每个级别不是等待计算内部成本，而是使用累加器计算到目前为止的成本，并将其传递给下一个内部级别。当我们到达最底层时，我们就有了最终的答案。

```F#
costSoFar = 1.0m; evaluate calcInnerCost with costSoFar:
  costSoFar = costSoFar + 1.0m; evaluate calcInnerCost with costSoFar:
    costSoFar = costSoFar + 1.0m; evaluate calcInnerCost with costSoFar:
      costSoFar = costSoFar + 1.0m; evaluate calcInnerCost with costSoFar:
        ...
        costSoFar = costSoFar + 1.0m; evaluate calcInnerCost with costSoFar:
          costSoFar = costSoFar + 1.0m; evaluate calcInnerCost with costSoFar:
            finalCost = costSoFar + book.price   // final answer
```

这种方法的一大优点是，在调用下一个较低级别之前，特定级别的所有计算都已*完全完成*。这意味着可以从堆栈中安全地丢弃该级别及其相关数据。这意味着没有堆栈溢出！

这样的实现称为*尾部递归*，其中可以安全地丢弃更高的级别。

### 重新使用累加器函数实现 `totalCost`

让我们从头开始重写总成本函数，使用一个名为 `costSoFar` 的累加器：

```F#
let rec totalCostUsingAcc costSoFar gift =
    match gift with
    | Book book ->
        costSoFar + book.price  // final result
    | Chocolate choc ->
        costSoFar + choc.price  // final result
    | Wrapped (innerGift,style) ->
        let newCostSoFar = costSoFar + 0.5m
        totalCostUsingAcc newCostSoFar innerGift
    | Boxed innerGift ->
        let newCostSoFar = costSoFar + 1.0m
        totalCostUsingAcc newCostSoFar innerGift
    | WithACard (innerGift,message) ->
        let newCostSoFar = costSoFar + 2.0m
        totalCostUsingAcc newCostSoFar innerGift
```

有几件事需要注意：

- 新版本的函数有一个额外的参数（`costSoFar`）。当我们在顶层调用它时，我们必须为此提供一个初始值（如零）。
- 非递归案例（`Book` 和 `Chocolate`）是终点。他们把到目前为止的成本加到他们的价格上，然后这就是最终的结果。
- 递归案例根据传入的参数计算新的 `costSoFar`。然后，新的 `costSoFar` 被传递到下一个较低的级别，就像上面的例子一样。

让我们对这个版本进行压力测试：

```F#
deeplyNestedBox 1000 |> totalCostUsingAcc 0.0m     // OK    1020.0M
deeplyNestedBox 10000 |> totalCostUsingAcc 0.0m    // OK   10020.0M
deeplyNestedBox 100000 |> totalCostUsingAcc 0.0m   // OK  100020.0M
deeplyNestedBox 1000000 |> totalCostUsingAcc 0.0m  // OK 1000020.0M
```

杰出的。多达一百万个嵌套级别，没有出现任何问题。

## 介绍“折叠”

现在，让我们将相同的设计原则应用于分解形态（catamorphism）实现。

我们将创建一个新函数 `foldGift`。我们将引入一个累加器 `acc`，我们将遍历每个级别，非递归情况将返回最终的累加器。

```F#
let rec foldGift fBook fChocolate fWrapped fBox fCard acc gift :'r =
    let recurse = foldGift fBook fChocolate fWrapped fBox fCard
    match gift with
    | Book book ->
        let finalAcc = fBook acc book
        finalAcc     // final result
    | Chocolate choc ->
        let finalAcc = fChocolate acc choc
        finalAcc     // final result
    | Wrapped (innerGift,style) ->
        let newAcc = fWrapped acc style
        recurse newAcc innerGift
    | Boxed innerGift ->
        let newAcc = fBox acc
        recurse newAcc innerGift
    | WithACard (innerGift,message) ->
        let newAcc = fCard acc message
        recurse newAcc innerGift
```

如果我们看看类型签名，我们可以看到它有细微的不同。现在到处都在使用 `'a` 型累加器。使用最终返回类型的唯一时间是在两种非递归情况下（`fBook` 和 `fChocolate`）。

```F#
val foldGift :
  fBook:('a -> Book -> 'r) ->
  fChocolate:('a -> Chocolate -> 'r) ->
  fWrapped:('a -> WrappingPaperStyle -> 'a) ->
  fBox:('a -> 'a) ->
  fCard:('a -> string -> 'a) ->
  // accumulator
  acc:'a ->
  // input value
  gift:Gift ->
  // return value
  'r
```

让我们更仔细地看看这一点，并将上一篇文章中原始同态的签名与新 `fold` 函数的签名进行比较。

首先，非递归情况：

```F#
// original catamorphism
fBook:(Book -> 'r)
fChocolate:(Chocolate -> 'r)

// fold
fBook:('a -> Book -> 'r)
fChocolate:('a -> Chocolate -> 'r)
```

正如你所看到的，使用“fold”，非递归情况需要一个额外的参数（累加器）并返回 `'r` 类型。

这一点非常重要：*累加器的类型不需要与返回类型相同*。我们需要尽快利用这一点。

递归案例呢？他们的签名是如何改变的？

```F#
// original catamorphism
fWrapped:('r -> WrappingPaperStyle -> 'r)
fBox:('r -> 'r)

// fold
fWrapped:('a -> WrappingPaperStyle -> 'a)
fBox:('a -> 'a)
```

对于递归情况，结构是相同的，但 `'r` 类型的所有使用都已替换为 `'a` 类型。递归情况根本不使用 `'r` 类型。

### 重新使用 fold 功能实现 `totalCost`

再次，我们可以重新实现总成本函数，但这次使用 `foldGift` 函数：

```F#
let totalCostUsingFold gift =

    let fBook costSoFar (book:Book) =
        costSoFar + book.price
    let fChocolate costSoFar (choc:Chocolate) =
        costSoFar + choc.price
    let fWrapped costSoFar style =
        costSoFar + 0.5m
    let fBox costSoFar =
        costSoFar + 1.0m
    let fCard costSoFar message =
        costSoFar + 2.0m

    // initial accumulator
    let initialAcc = 0m

    // call the fold
    foldGift fBook fChocolate fWrapped fBox fCard initialAcc gift
```

同样，我们可以处理大量嵌套的盒子，而不会发生堆栈溢出：

```F#
deeplyNestedBox 100000 |> totalCostUsingFold  // no problem   100020.0M
deeplyNestedBox 1000000 |> totalCostUsingFold // no problem  1000020.0M
```

## 折叠的问题

所以使用 fold 解决了我们所有的问题，对吧？

不幸的是，没有。

是的，没有更多的堆栈溢出，但我们现在还有另一个问题。

### 重新实现 `description` 函数

为了了解问题所在，让我们重新审视我们在第一篇文章中创建的 `description` 函数。

原始的不是尾部递归的，所以让我们让它更安全，并使用 `foldGift` 重新实现它。

```F#
let descriptionUsingFold gift =
    let fBook descriptionSoFar (book:Book) =
        sprintf "'%s' %s" book.title descriptionSoFar

    let fChocolate descriptionSoFar (choc:Chocolate) =
        sprintf "%A chocolate %s" choc.chocType descriptionSoFar

    let fWrapped descriptionSoFar style =
        sprintf "%s wrapped in %A paper" descriptionSoFar style

    let fBox descriptionSoFar =
        sprintf "%s in a box" descriptionSoFar

    let fCard descriptionSoFar message =
        sprintf "%s with a card saying '%s'" descriptionSoFar message

    // initial accumulator
    let initialAcc = ""

    // main call
    foldGift fBook fChocolate fWrapped fBox fCard initialAcc gift
```

让我们看看输出是什么：

```F#
birthdayPresent |> descriptionUsingFold
// "'Wolf Hall'  with a card saying 'Happy Birthday' wrapped in HappyBirthday paper"

christmasPresent |> descriptionUsingFold
// "SeventyPercent chocolate  wrapped in HappyHolidays paper in a box"
```

这些输出是错误的！装饰的顺序混淆了。

它应该是一本带卡片的包装书，而不是一本书和一张卡片包装在一起。它应该是装在盒子里的巧克力，然后包装好，而不是装在盒子里面的巧克力！

```F#
// OUTPUT: "'Wolf Hall'  with a card saying 'Happy Birthday' wrapped in HappyBirthday paper"
// CORRECT "'Wolf Hall' wrapped in HappyBirthday paper with a card saying 'Happy Birthday'"

// OUTPUT: "SeventyPercent chocolate  wrapped in HappyHolidays paper in a box"
// CORRECT "SeventyPercent chocolate in a box wrapped in HappyHolidays paper"
```

出了什么问题？

答案是，每一层的正确描述取决于下面层的描述。我们不能“预先计算”一个层的描述，并使用 `descriptionSoFar` 累加器将其传递给下一层。

但现在我们面临一个困境：一个层依赖于下一层的信息，但我们想避免堆栈溢出。

## 将函数用作累加器

请记住，累加器类型不必与返回类型相同。我们可以使用任何东西作为累加器，甚至是函数！

所以我们要做的是，与其传递一个 `descriptionSoFar` 作为累加器，不如传递一个函数（指 `descriptionGenerator`），该函数将根据下一层的值构建适当的描述。

以下是非递归情况的实现：

```F#
let fBook descriptionGenerator (book:Book) =
    descriptionGenerator (sprintf "'%s'" book.title)
//  ~~~~~~~~~~~~~~~~~~~~  <= a function as an accumulator!

let fChocolate descriptionGenerator (choc:Chocolate) =
    descriptionGenerator (sprintf "%A chocolate" choc.chocType)
```

递归案例的实现有点复杂：

- 我们得到一个累加器（`descriptionGenerator`）作为参数。
- 我们需要创建一个新的累加器（新的 `descriptionGenerator`），以传递到下一个较低的层。
- 描述生成器的输入将是从较低层累积的所有数据。我们操纵它来创建一个新的描述，然后调用从更高层传入的 `descriptionGenerator`。

谈论比演示更复杂，因此以下是其中两种情况的实现：

```F#
let fWrapped descriptionGenerator style =
    let newDescriptionGenerator innerText =
        let newInnerText = sprintf "%s wrapped in %A paper" innerText style
        descriptionGenerator newInnerText
    newDescriptionGenerator

let fBox descriptionGenerator =
    let newDescriptionGenerator innerText =
        let newInnerText = sprintf "%s in a box" innerText
        descriptionGenerator newInnerText
    newDescriptionGenerator
```

我们可以通过直接使用 lambda 来简化代码：

```F#
let fWrapped descriptionGenerator style =
    fun innerText ->
        let newInnerText = sprintf "%s wrapped in %A paper" innerText style
        descriptionGenerator newInnerText

let fBox descriptionGenerator =
    fun innerText ->
        let newInnerText = sprintf "%s in a box" innerText
        descriptionGenerator newInnerText
```

我们可以继续使用管道和其他东西使它更紧凑，但我认为我们在这里有一个简洁和模糊之间的良好平衡。

以下是整个功能：

```F#
let descriptionUsingFoldWithGenerator gift =

    let fBook descriptionGenerator (book:Book) =
        descriptionGenerator (sprintf "'%s'" book.title)

    let fChocolate descriptionGenerator (choc:Chocolate) =
        descriptionGenerator (sprintf "%A chocolate" choc.chocType)

    let fWrapped descriptionGenerator style =
        fun innerText ->
            let newInnerText = sprintf "%s wrapped in %A paper" innerText style
            descriptionGenerator newInnerText

    let fBox descriptionGenerator =
        fun innerText ->
            let newInnerText = sprintf "%s in a box" innerText
            descriptionGenerator newInnerText

    let fCard descriptionGenerator message =
        fun innerText ->
            let newInnerText = sprintf "%s with a card saying '%s'" innerText message
            descriptionGenerator newInnerText

    // initial DescriptionGenerator
    let initialAcc = fun innerText -> innerText

    // main call
    foldGift fBook fChocolate fWrapped fBox fCard initialAcc gift
```

同样，我使用了过于描述性的中间值来明确发生了什么。

如果我们现在尝试使用 `descriptionUsingFoldWithGenerator`，我们将再次得到正确答案：

```F#
birthdayPresent |> descriptionUsingFoldWithGenerator
// CORRECT "'Wolf Hall' wrapped in HappyBirthday paper with a card saying 'Happy Birthday'"

christmasPresent |> descriptionUsingFoldWithGenerator
// CORRECT "SeventyPercent chocolate in a box wrapped in HappyHolidays paper"
```

## 介绍“foldback”

现在我们知道该做什么了，让我们制作一个通用版本，为我们处理生成器函数逻辑。这个版本我们称之为“foldback”：

*顺便说一句，我将在这里使用术语“生成器”。在其他地方，它通常被称为“continuation”函数，通常缩写为“k”。*

下面是实现：

```F#
let rec foldbackGift fBook fChocolate fWrapped fBox fCard generator gift :'r =
    let recurse = foldbackGift fBook fChocolate fWrapped fBox fCard
    match gift with
    | Book book ->
        generator (fBook book)
    | Chocolate choc ->
        generator (fChocolate choc)
    | Wrapped (innerGift,style) ->
        let newGenerator innerVal =
            let newInnerVal = fWrapped innerVal style
            generator newInnerVal
        recurse newGenerator innerGift
    | Boxed innerGift ->
        let newGenerator innerVal =
            let newInnerVal = fBox innerVal
            generator newInnerVal
        recurse newGenerator innerGift
    | WithACard (innerGift,message) ->
        let newGenerator innerVal =
            let newInnerVal = fCard innerVal message
            generator newInnerVal
        recurse newGenerator innerGift
```

您可以看到，它就像 `descriptionUsingFoldWithGenerator` 实现一样，只是我们使用了通用的 `newInnerVal` 和 `generator` 值。

类型签名类似于原始的分解形态（catamorphism），除了每种情况仅仅现在适用于 `'a`。唯一使用 `'r` 的时间是在生成器函数本身！

```F#
val foldbackGift :
  fBook:(Book -> 'a) ->
  fChocolate:(Chocolate -> 'a) ->
  fWrapped:('a -> WrappingPaperStyle -> 'a) ->
  fBox:('a -> 'a) ->
  fCard:('a -> string -> 'a) ->
  // accumulator
  generator:('a -> 'r) ->
  // input value
  gift:Gift ->
  // return value
  'r
```

上面的 `foldback` 实现是从头开始编写的。如果你想做一个有趣的练习，看看你是否可以用 `fold` 来写 `foldback`。

让我们使用 `foldback` 重写 `description` 函数：

```F#
let descriptionUsingFoldBack gift =
    let fBook (book:Book) =
        sprintf "'%s'" book.title
    let fChocolate (choc:Chocolate) =
        sprintf "%A chocolate" choc.chocType
    let fWrapped innerText style =
        sprintf "%s wrapped in %A paper" innerText style
    let fBox innerText =
        sprintf "%s in a box" innerText
    let fCard innerText message =
        sprintf "%s with a card saying '%s'" innerText message
    // initial DescriptionGenerator
    let initialAcc = fun innerText -> innerText
    // main call
    foldbackGift fBook fChocolate fWrapped fBox fCard initialAcc gift
```

结果仍然正确：

```F#
birthdayPresent |> descriptionUsingFoldBack
// CORRECT "'Wolf Hall' wrapped in HappyBirthday paper with a card saying 'Happy Birthday'"

christmasPresent |> descriptionUsingFoldBack
// CORRECT "SeventyPercent chocolate in a box wrapped in HappyHolidays paper"
```

### 和原始的分解形态比较 `foldback`

`descriptionUsingFoldBack` 的实现几乎与上一篇文章中使用原始分解形态 `cataGift` 的版本相同。

以下是使用 `cataGift` 的版本：

```F#
let descriptionUsingCata gift =
    let fBook (book:Book) =
        sprintf "'%s'" book.title
    let fChocolate (choc:Chocolate) =
        sprintf "%A chocolate" choc.chocType
    let fWrapped (innerText,style) =
        sprintf "%s wrapped in %A paper" innerText style
    let fBox innerText =
        sprintf "%s in a box" innerText
    let fCard (innerText,message) =
        sprintf "%s with a card saying '%s'" innerText message
    // call the catamorphism
    cataGift fBook fChocolate fWrapped fBox fCard gift
```

以下是使用 `foldbackGift` 的版本：

```F#
let descriptionUsingFoldBack gift =
    let fBook (book:Book) =
        sprintf "'%s'" book.title
    let fChocolate (choc:Chocolate) =
        sprintf "%A chocolate" choc.chocType
    let fWrapped innerText style =
        sprintf "%s wrapped in %A paper" innerText style
    let fBox innerText =
        sprintf "%s in a box" innerText
    let fCard innerText message =
        sprintf "%s with a card saying '%s'" innerText message
    // initial DescriptionGenerator
    let initialAcc = fun innerText -> innerText    // could be replaced with id
    // main call
    foldbackGift fBook fChocolate fWrapped fBox fCard initialAcc gift
```

所有处理函数基本相同。唯一的变化是添加了一个初始生成器函数，在本例中仅为 `id`。

然而，尽管这两种情况下的代码看起来相同，但它们的递归安全性不同。`foldbackGift` 版本仍然是尾部递归的，可以处理非常大的嵌套深度，这与 `cataGift` 版本不同。

但这种实现方式也并不完美。嵌套函数链可能会变得非常慢并生成大量垃圾，对于这个特定的例子，有一种更快的方法，我们将在下一篇文章中介绍。

### 为了避免混淆，更改 `foldback` 的类型签名

在 `foldGift` 中，`fWrapped` 的签名是：

```F#
fWrapped:('a -> WrappingPaperStyle -> 'a)
```

但在 `foldbackGift` 中，`fWrapped` 的签名是：

```F#
fWrapped:('a -> WrappingPaperStyle -> 'a)
```

你能看出区别吗？不，我也是。

这两个功能非常相似，但工作方式却截然不同。在 `foldGift` 版本中，第一个参数是来自外部级别的累加器，而在 `foldbackGift` 版本，第一个参数值是来自内部级别的累加器。这是一个非常重要的区别！

因此，通常会更改 `foldBack` 版本的签名，使累加器始终位于最后，而在正常的折叠功能中，累加器始终位于第一。

```F#
let rec foldbackGift fBook fChocolate fWrapped fBox fCard gift generator :'r =
//swapped =>                                              ~~~~~~~~~~~~~~

    let recurse = foldbackGiftWithAccLast fBook fChocolate fWrapped fBox fCard

    match gift with
    | Book book ->
        generator (fBook book)
    | Chocolate choc ->
        generator (fChocolate choc)

    | Wrapped (innerGift,style) ->
        let newGenerator innerVal =
            let newInnerVal = fWrapped style innerVal
//swapped =>                           ~~~~~~~~~~~~~~
            generator newInnerVal
        recurse innerGift newGenerator
//swapped =>    ~~~~~~~~~~~~~~~~~~~~~~

    | Boxed innerGift ->
        let newGenerator innerVal =
            let newInnerVal = fBox innerVal
            generator newInnerVal
        recurse innerGift newGenerator
//swapped =>    ~~~~~~~~~~~~~~~~~~~~~~

    | WithACard (innerGift,message) ->
        let newGenerator innerVal =
            let newInnerVal = fCard message innerVal
//swapped =>                        ~~~~~~~~~~~~~~~~
            generator newInnerVal
        recurse innerGift newGenerator
//swapped =>    ~~~~~~~~~~~~~~~~~~~~~~
```

此更改显示在类型签名中。`Gift` 价值现在位于累加器之前：

```F#
val foldbackGift :
  fBook:(Book -> 'a) ->
  fChocolate:(Chocolate -> 'a) ->
  fWrapped:(WrappingPaperStyle -> 'a -> 'a) ->
  fBox:('a -> 'a) ->
  fCard:(string -> 'a -> 'a) ->
  // input value
  gift:Gift ->
  // accumulator
  generator:('a -> 'r) ->
  // return value
  'r
```

现在我们可以很容易地分辨出这两个版本。

```F#
// fold
fWrapped:('a -> WrappingPaperStyle -> 'a)

// foldback
fWrapped:(WrappingPaperStyle -> 'a -> 'a)
```

## 创建折叠的规则

为了完成这篇文章，让我们总结一下创建折叠的规则。

在第一篇文章中，我们看到创建分解形态是一个[遵循规则](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds/#rules)的机械过程。创建自上而下的迭代折叠也是如此。该过程是：

- 创建一个函数参数来处理结构中的每种情况。
- 添加一个附加参数作为累加器。
- 对于非递归情况，传递函数参数累加器加上与该情况相关的所有数据。
- 对于递归情况，执行两个步骤：
  - 首先，向处理程序传递累加器以及与该情况相关的所有数据（内部递归数据除外）。结果是一个新的累加器值。
  - 然后，使用新的累加器值对嵌套值递归调用 fold。

请注意，每个处理程序只“看到”（a）该情况的数据，以及（b）从外部级别传递给它的累加器。它无法访问内部级别的结果。

## 摘要

我们在这篇文章中看到了如何定义一个称为“fold”的分解态的尾部递归实现，以及反向版本“foldback”。

在[下一篇文章](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2b/)中，我们将退一步，花一些时间了解“fold”的真正含义，以及在 `fold`、`foldback` 和 `cata` 之间进行选择的一些指导方针。

然后，我们将看看是否可以将这些规则应用于另一个域。

这篇文章的源代码可以在[这里 gist](https://gist.github.com/swlaschin/df4427d0043d7146e592) 找到。



# 4 理解折叠

*Part of the "Recursive types and folds" series (*[link](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2b/#series-toc)*)*

递归与迭代
23 八月 2015 这篇文章是超过3 年

https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2b/

这篇文章是系列文章中的第四篇。

在[上一篇文章](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds/)中，我介绍了“folds”，这是一种为递归类型创建自上而下迭代函数的方法。

在这篇文章中，我们将花一些时间更详细地了解折叠。

## 系列内容

以下是本系列的内容：

- **第 1 部分：递归类型和分解形态（catamorphisms）介绍**
  - [一个简单的递归类型](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds/#basic-recursive-type)
  - [对所有事物进行参数化](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds/#parameterize)
  - [介绍分解形态](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds/#catamorphisms)
  - [变形的好处](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds/#benefits)
  - [创建分解形态的规则](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds/#rules)
- **第 2 部分：分解形态示例**
  - [变形示例：文件系统域](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-1b/#file-system)
  - [变形示例：产品域](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-1b/#product)
- **第 3 部分：介绍折叠**
  - [我们的分解形态实现中的一个缺陷](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2/#flaw)
  - [介绍 `fold`](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2/#fold)
  - [折叠问题](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2/#problems)
  - [将函数用作累加器](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2/#functions)
  - [介绍 `foldback`](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2/#foldback)
  - [创建折叠的规则](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2/#rules)
- **第 4 部分：了解折叠**
  - [迭代与递归](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2b/#iteration)
  - [折叠示例：文件系统域](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2b/#file-system)
  - [关于“折叠”的常见问题](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2b/#questions)
- **第 5 部分：泛型递归类型**
  - [LinkedList：一种通用的递归类型](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3/#linkedlist)
  - [使礼品领域泛型](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3/#revisiting-gift)
  - [定义泛型容器类型](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3/#container)
  - [实现礼物领域的第三种方法](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3/#another-gift)
  - [抽象还是具体？比较三种设计](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3/#compare)
- **第 6 部分：现实世界中的树**
  - [定义通用树类型](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#tree)
  - [现实世界中的树类型](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#reuse)
  - [映射树类型](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#map)
  - [示例：创建目录列表](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#listing)
  - [示例：并行 grep](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#grep)
  - [示例：将文件系统存储在数据库中](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#database)
  - [示例：将树序列化为 JSON](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#tojson)
  - [示例：从 JSON 反序列化树](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#fromjson)
  - [示例：从 JSON 反序列化树 - 带错误处理](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#json-with-error-handling)

## 迭代 vs. 递归

我们现在有*三种*不同的函数—— `cata`、`fold` 和 `foldback`。

那么，它们之间到底有什么区别呢？我们已经看到，一些不适用于 `fold` 的东西也适用于 `foldBack`，但有没有一种简单的方法来记住其中的区别？

区分这三种方法的一种方法是记住这一点：

- `fold` 是自上而下的迭代
- `cata` 是自下而上的递归
- `foldBack` 是自下而上的迭代

这是什么意思？

那么，在 `fold` 中，累加器在顶层初始化，并向下传递到每个较低的级别，直到达到最低和最后一个级别。

在代码方面，每个级别都做到了这一点：

```F#
accumulatorFromHigherLevel, combined with
  stuffFromThisLevel
    => stuffToSendDownToNextLowerLevel
```

在命令式语言中，这正是一个“for循环”，其中一个可变变量存储累加器。

```F#
var accumulator = initialValue
foreach level in levels do
{
  accumulator, combined with
    stuffFromThisLevel
      => update accumulator
}
```

因此，这种自上而下的折叠可以被视为迭代（事实上，F# 编译器会将这样的尾部递归函数转换为幕后的迭代）。

另一方面，在 `cata`，累加器从底部开始，向上传递到每个更高的水平，直到达到顶部水平。

在代码方面，每个级别都做到了这一点：

```F#
accumulatorFromLowerLevel, combined with
  stuffFromThisLevel
    => stuffToSendUpToNextHigherLevel
```

这完全是一个递归循环：

```F#
let recurse (head::tail) =
    if atBottomLevel then
       return something
    else    // if not at bottom level
       let accumulatorFromLowerLevel = recurse tail
       return stuffFromThisLevel, combined with
          accumulatorFromLowerLevel
```

最后，`foldback` 可以看作是“反向迭代”。累加器贯穿所有层面，但从底部而不是顶部开始。它具有 `cata` 的优点，因为首先计算内部值并向上传递，但由于它是迭代的，所以不会出现堆栈溢出。

到目前为止，我们讨论的许多概念在用迭代与递归表示时变得清晰起来。例如：

- 迭代版本（`fold` 和 `foldback`）没有堆栈，不会导致堆栈溢出。
- “总成本”函数不需要内部数据，因此自上而下的迭代版本（`fold`）工作正常。
- 不过，“描述”函数需要内部文本才能正确格式化，因此递归版本（`cata`）或自下而上迭代（`foldback`）更合适。

## 折叠示例：文件系统域

在上一篇文章中，我们描述了创建折叠的一些规则。让我们看看是否可以应用这些规则在另一个域中创建折叠，即[本系列第二篇文章](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-1b/#file-system)中的“文件系统”域。

作为提醒，这是该帖子中的粗略“文件系统”域：

```F#
type FileSystemItem =
    | File of File
    | Directory of Directory
and File = {name:string; fileSize:int}
and Directory = {name:string; dirSize:int; subitems:FileSystemItem list}
```

请注意，每个目录都包含一个子项列表，因此这不是像 `Gift` 这样的线性结构，而是一个树状结构。我们在实现 fold 时必须考虑到这一点。

以下是一些示例值：

```F#
let readme = File {name="readme.txt"; fileSize=1}
let config = File {name="config.xml"; fileSize=2}
let build  = File {name="build.bat"; fileSize=3}
let src = Directory {name="src"; dirSize=10; subitems=[readme; config; build]}
let bin = Directory {name="bin"; dirSize=10; subitems=[]}
let root = Directory {name="root"; dirSize=5; subitems=[src; bin]}
```

我们想创建一个 fold，比如 `foldFS`。因此，按照规则，让我们添加一个额外的累加器参数 `acc` 并将其传递给 `File` 案例：

```F#
let rec foldFS fFile fDir acc item :'r =
    let recurse = foldFS fFile fDir
    match item with
    | File file ->
        fFile acc file
    | Directory dir ->
        // to do
```

`Directory` 的情况更棘手。我们不应该知道子项，这意味着我们唯一可以使用的数据是 `name`、`dirSize` 和从更高级别传入的累加器。将这些组合在一起制成一个新的累加器。

```F#
| Directory dir ->
    let newAcc = fDir acc (dir.name,dir.dirSize)
    // to do
```

*注意：出于分组目的，我将 `name` 和 `dirSize` 保留为元组，但当然你可以将它们作为单独的参数传入。*

现在我们需要依次将这个新的累加器传递给每个子项，但每个子项都会返回一个自己的新累加器，因此我们需要使用以下方法：

- 获取新创建的累加器并将其传递给第一个子项。
- 获取该（另一个累加器）的输出，并将其传递给第二个子项。
- 获取该（另一个累加器）的输出，并将其传递给第三个子项。
- 以此类推。最后一个子项的输出是最终结果。

不过，我们已经可以采用这种方法。这正是 `List.fold` 所做的！以下是 Directory 案例的代码：

```F#
| Directory dir ->
    let newAcc = fDir acc (dir.name,dir.dirSize)
    dir.subitems |> List.fold recurse newAcc
```

这是整个 `foldFS` 函数：

```F#
let rec foldFS fFile fDir acc item :'r =
    let recurse = foldFS fFile fDir
    match item with
    | File file ->
        fFile acc file
    | Directory dir ->
        let newAcc = fDir acc (dir.name,dir.dirSize)
        dir.subitems |> List.fold recurse newAcc
```

有了这个，我们可以重写上一篇文章中实现的两个函数。

首先，`totalSize` 函数，它只是对所有大小进行求和：

```F#
let totalSize fileSystemItem =
    let fFile acc (file:File) =
        acc + file.fileSize
    let fDir acc (name,size) =
        acc + size
    foldFS fFile fDir 0 fileSystemItem
```

如果我们测试它，我们会得到与以前相同的结果：

```F#
readme |> totalSize  // 1
src |> totalSize     // 16 = 10 + (1 + 2 + 3)
root |> totalSize    // 31 = 5 + 16 + 10
```

### 文件系统域：`largestFile` 例子

我们还可以重新实现“树中最大的文件是什么？”函数。

和以前一样，它将返回一个 `File option`，因为树可能是空的。这意味着累加器也将是一个 `File option`。

这一次，`File` 案例处理程序很棘手：

- 如果传入的累加器为 `None`，则此当前文件将成为新的累加器。
- 如果传入的累加器是 `Some file`，则将该文件的大小与此文件进行比较。以较大者为准，即为新的蓄能器。

代码如下：

```F#
let fFile (largestSoFarOpt:File option) (file:File) =
    match largestSoFarOpt with
    | None ->
        Some file
    | Some largestSoFar ->
        if largestSoFar.fileSize > file.fileSize then
            Some largestSoFar
        else
            Some file
```

另一方面，`Directory` 处理程序很简单——只需将“迄今为止最大的”累加器传递到下一级

```F#
let fDir largestSoFarOpt (name,size) =
    largestSoFarOpt
```

以下是完整的实现：

```F#
let largestFile fileSystemItem =
    let fFile (largestSoFarOpt:File option) (file:File) =
        match largestSoFarOpt with
        | None ->
            Some file
        | Some largestSoFar ->
            if largestSoFar.fileSize > file.fileSize then
                Some largestSoFar
            else
                Some file

    let fDir largestSoFarOpt (name,size) =
        largestSoFarOpt

    // call the fold
    foldFS fFile fDir None fileSystemItem
```

如果我们测试它，我们会得到与以前相同的结果：

```F#
readme |> largestFile
// Some {name = "readme.txt"; fileSize = 1}

src |> largestFile
// Some {name = "build.bat"; fileSize = 3}

bin |> largestFile
// None

root |> largestFile
// Some {name = "build.bat"; fileSize = 3}
```

将此实现与[第二篇文章中的递归版本](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-1b/#file-system)进行比较很有趣。我认为这个更容易实现，我自己。

### 树遍历类型

到目前为止讨论的各种折叠函数对应于各种各样的树遍历：

- `fold` 函数（如这里实现的）更恰当地称为“预购深度优先”树遍历。
- `foldback` 函数将是“后序深度优先”的树遍历。
- `cata` 函数根本不是“遍历”，因为每个内部节点一次处理所有子结果的列表。

通过调整逻辑，您可以制作其他变体。

有关各种树漫游的描述，请参阅[维基百科](https://en.wikipedia.org/wiki/Tree_traversal)。

### 我们需要 `foldback` 吗?

我们需要为 FileSystem 域实现 `foldback` 功能吗？

我不这么认为。如果我们需要访问内部数据，我们可以使用上一篇文章中原始的“天真”的分解形态（catamorphism）实现。

但是，嘿，等等，我一开始不是说过我们必须注意堆栈溢出吗？

是的，如果递归类型嵌套很深。但是，考虑一个每个目录只有两个子目录的文件系统。如果有 64 个嵌套级别，会有多少个目录？（提示：您可能熟悉类似的问题。与棋盘上的谷物有关）。

我们之前看到，堆栈溢出问题只发生在 1000 多个嵌套级别上，而且这种嵌套级别通常只发生在线性递归类型上，而不是像 FileSystem 域这样的树。

## 关于“折叠”的常见问题

此时，你可能会不知所措！所有这些不同的实现方式都有不同的优缺点。

所以，让我们稍作休息，解决一些常见问题。

### “左折”和“右折”有什么区别

关于折叠的术语经常有很多混淆：“左”与“右”、“前”与“后”等。

- *左折叠或前折叠*就是我刚才所说的 `fold`——自上而下的迭代方法。
- *右折叠或后折叠*是我所说的 `foldBack`——自下而上的迭代方法。

然而，这些术语实际上只适用于像 `Gift` 这样的线性递归结构。当涉及到更复杂的树状结构时，这些区别太简单了，因为有很多方法可以遍历它们：广度优先、深度优先、前置和后置等等。

### 我应该使用哪种折叠功能？

以下是一些指导方针：

- 如果你的递归类型不会嵌套得太深（比如说，深度不到 100 层），那么我们在第一篇文章中描述的朴素 `cata` 分解形态（catamorphism）是可以的。它很容易机械地实现——只需将主递归类型替换为 `'r`。
- 如果你的递归类型将被深度嵌套，并且你想防止堆栈溢出，请使用迭代 `fold`。
- 如果您正在使用迭代折叠，但需要访问内部数据，请将连续函数作为累加器传递。
- 最后，迭代方法通常比递归方法更快，使用更少的内存（但如果你传递了太多的嵌套延续，这种优势就会丧失）。

另一种思考方式是看看你的“组合器”函数。在每一步中，您都要组合来自不同级别的数据：

```
level1 data [combined with] level2 data [combined with] level3 data [combined with] level4 data
```

如果你的组合器函数是“左结合”的，就像这样：

```F#
(((level1 combineWith level2) combineWith level3) combineWith level4)
```

然后使用迭代方法，但如果你的组合器函数是“右结合”的，就像这样：

```F#
(level1 combineWith (level2 combineWith (level3 combineWith level4)))
```

然后使用 `cata` 或 `foldback` 方法。

如果你的组合器函数不在乎（例如加法），请使用更方便的函数。

### 我怎么知道代码是否是尾部递归的？

实现是否是尾部递归并不总是显而易见的。最简单的方法是查看每个案例的最后一个表达式。

如果对“recurse”的调用是最后一个表达式，那么它就是尾部递归的。如果在那之后还有其他工作，那么它就不是尾递归的。

亲自看看我们讨论过的三个实现。

首先，这是原始 `cataGift` 实现中 `WithACard` 案例的代码：

```F#
| WithACard (gift,message) ->
    fCard (recurse gift,message)
//         ~~~~~~~  <= Call to recurse is not last expression.
//                     Tail-recursive? No!
```

`cataGift` 实现*不是*尾部递归的。

以下是 `foldGift` 实现的代码：

```F#
| WithACard (innerGift,message) ->
    let newAcc = fCard acc message
    recurse newAcc innerGift
//  ~~~~~~~  <= Call to recurse is last expression.
//              Tail-recursive? Yes!
```

`foldGift` 实现是尾部递归的。

以下是 `foldbackGift` 实现的代码：

```F#
| WithACard (innerGift,message) ->
    let newGenerator innerVal =
        let newInnerVal = fCard innerVal message
        generator newInnerVal
    recurse newGenerator innerGift
//  ~~~~~~~  <= Call to recurse is last expression.
//              Tail-recursive? Yes!
```

`foldbackGift` 实现也是尾部递归的。

### 如何使折叠短路？

在 C# 这样的语言中，您可以使用 `break` 提前退出迭代循环，如下所示：

```c#
foreach (var elem in collection)
{
    // do something

    if ( x == "error")
    {
        break;
    }
}
```

那么，你如何用折叠做同样的事情呢？

简短的回答是，你不能！折叠的设计是为了依次访问所有元素。访问者模式具有相同的约束。

有三种解决方法。

第一种方法是根本不使用 `fold`，并创建自己的递归函数，该函数在所需条件下终止：

在这个例子中，当总和大于 100 时，循环退出：

```F#
let rec firstSumBiggerThan100 sumSoFar listOfInts =
    match listOfInts with
    | [] ->
        sumSoFar // exhausted all the ints!
    | head::tail ->
        let newSumSoFar = head + sumSoFar
        if newSumSoFar > 100 then
            newSumSoFar
        else
            firstSumBiggerThan100 newSumSoFar tail

// test
[30;40;50;60] |> firstSumBiggerThan100 0  // 120
[1..3..100] |> firstSumBiggerThan100 0  // 117
```

第二种方法是使用 `fold`，但在传递的累加器中添加某种“忽略”标志。一旦设置了此标志，其余迭代就什么都不做。

这是一个计算总和的例子，但累加器实际上是一个除了 `sumSoFar` 之外还带有 `ignoreFlag` 的元组：

```F#
let firstSumBiggerThan100 listOfInts =

    let folder accumulator i =
        let (ignoreFlag,sumSoFar) = accumulator
        if not ignoreFlag then
            let newSumSoFar = i + sumSoFar
            let newIgnoreFlag  = newSumSoFar > 100
            (newIgnoreFlag, newSumSoFar)
        else
            // pass the accumulator along
            accumulator

    let initialAcc = (false,0)

    listOfInts
    |> List.fold folder initialAcc  // use fold
    |> snd // get the sumSoFar

/// test
[30;40;50;60] |> firstSumBiggerThan100  // 120
[1..3..100] |> firstSumBiggerThan100  // 117
```

第三个版本是第二个版本的变体——创建一个特殊的值来表示应该忽略剩余的数据，但将其包装在计算表达式中，使其看起来更自然。

[Tomas Petricek 的博客](http://tomasp.net/blog/imperative-ii-break.aspx/)上记录了这种方法，代码如下：

```F#
let firstSumBiggerThan100 listOfInts =
    let mutable sumSoFar = 0
    imperative {
        for x in listOfInts do
            sumSoFar <- x + sumSoFar
            if sumSoFar > 100 then do! break
    }
    sumSoFar
```

## 摘要

这篇文章的目的是帮助你更好地理解折叠，并展示如何将它们应用于像文件系统这样的树结构。我希望这有帮助！

到目前为止，本系列中的所有示例都非常具体；我们已经为遇到的每个域实现了自定义折叠。我们能不能更通用一些，构建一些可重用的折叠实现？

在[下一篇文章](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3/)中，我们将介绍泛型递归类型，以及如何使用它们。

*这篇文章的源代码可以在[这里 gist](https://gist.github.com/swlaschin/e065b0e99dd68cd35846) 找到。*

# 5 泛型递归类型

*Part of the "Recursive types and folds" series (*[link](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3/#series-toc)*)*

以三种方式实现域
2015年8月24日这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3/

这篇文章是系列文章中的第五篇。

在[上一篇文章](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2b/)中，我们花了一些时间了解特定领域类型的折叠。

在这篇文章中，我们将拓宽视野，看看如何使用泛型递归类型。

## 系列内容

以下是本系列的内容：

- **第 1 部分：递归类型和分解形态（catamorphisms）介绍**
  - [一个简单的递归类型](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds/#basic-recursive-type)
  - [对所有事物进行参数化](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds/#parameterize)
  - [介绍分解形态](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds/#catamorphisms)
  - [变形的好处](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds/#benefits)
  - [创建分解形态的规则](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds/#rules)
- **第 2 部分：分解形态示例**
  - [变形示例：文件系统域](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-1b/#file-system)
  - [变形示例：产品域](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-1b/#product)
- **第 3 部分：介绍折叠**
  - [我们的分解形态实现中的一个缺陷](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2/#flaw)
  - [介绍 `fold`](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2/#fold)
  - [折叠问题](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2/#problems)
  - [将函数用作累加器](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2/#functions)
  - [介绍 `foldback`](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2/#foldback)
  - [创建折叠的规则](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2/#rules)
- **第 4 部分：了解折叠**
  - [迭代与递归](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2b/#iteration)
  - [折叠示例：文件系统域](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2b/#file-system)
  - [关于“折叠”的常见问题](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2b/#questions)
- **第 5 部分：泛型递归类型**
  - [LinkedList：一种通用的递归类型](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3/#linkedlist)
  - [使礼品领域泛型](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3/#revisiting-gift)
  - [定义泛型容器类型](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3/#container)
  - [实现礼物领域的第三种方法](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3/#another-gift)
  - [抽象还是具体？比较三种设计](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3/#compare)
- **第 6 部分：现实世界中的树**
  - [定义通用树类型](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#tree)
  - [现实世界中的树类型](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#reuse)
  - [映射树类型](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#map)
  - [示例：创建目录列表](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#listing)
  - [示例：并行 grep](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#grep)
  - [示例：将文件系统存储在数据库中](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#database)
  - [示例：将树序列化为 JSON](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#tojson)
  - [示例：从 JSON 反序列化树](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#fromjson)
  - [示例：从 JSON 反序列化树 - 带错误处理](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#json-with-error-handling)

## LinkedList：一种通用的递归类型

这里有一个问题：如果你只有代数类型，并且你只能将它们组合成乘积（[元组](https://fsharpforfunandprofit.com/posts/tuples/)、记录）或和（[可区分并集](https://fsharpforfunandprofit.com/posts/discriminated-unions/)），那么你如何仅通过使用这些操作来创建列表类型呢？

答案当然是递归！

让我们从最基本的递归类型开始：列表。

我将称我的版本为 `LinkedList`，但它与 F# 中的 `list` 类型基本相同。

那么，如何以递归方式定义列表呢？

好吧，它要么是空的，要么是由一个元素加上另一个列表组成的。换句话说，我们可以将其定义为一种选择类型（“可区分联合”），如下所示：

```F#
type LinkedList<'a> =
    | Empty
    | Cons of head:'a * tail:LinkedList<'a>
```

`Empty` 例子表示空列表。`Cons` 案例有一个元组：head 元素和 tail，后者是另一个列表。

然后，我们可以定义一个特定的 `LinkedList` 值，如下所示：

```F#
let linkedList = Cons (1, Cons (2, Cons(3, Empty)))
```

使用本机 F# 列表类型，等效定义为：

```F#
let linkedList = 1 :: 2 :: 3 :: []
```

这只是 `[1; 2; 3]`

### LinkedList 的 `cata`

按照[本系列第一篇文章](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds/#rules)中的规则，我们可以通过将 `Empty` 和 `Cons` 替换为 `fEmpty` 和 `fCons` 来机械地创建一个 `cata` 函数：

```F#
module LinkedList =

    let rec cata fCons fEmpty list :'r=
        let recurse = cata fCons fEmpty
        match list with
        | Empty ->
            fEmpty
        | Cons (element,list) ->
            fCons element (recurse list)
```

*注意：我们将把所有与 `LinkedList<'a>` 关联的函数放在一个名为 `LinkedList` 的模块中。使用泛型类型的一个好处是，类型名称不会与类似的模块名称冲突！*

一如既往，案例处理函数的签名与类型构造函数的签名并行，`LinkedList` 被 `'r` 替换。

```F#
val cata :
    fCons:('a -> 'r -> 'r) ->
    fEmpty:'r ->
    list:LinkedList<'a>
    -> 'r
```

### LinkedList 的 `fold`

我们还可以使用[前面文章](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2/#rules)中的规则创建一个自上而下的迭代 `fold` 函数。

```F#
module LinkedList =

    let rec cata ...

    let rec foldWithEmpty fCons fEmpty acc list :'r=
        let recurse = foldWithEmpty fCons fEmpty
        match list with
        | Empty ->
            fEmpty acc
        | Cons (element,list) ->
            let newAcc = fCons acc element
            recurse newAcc list
```

此 `foldWithEmpty` 函数与标准 `List.fold` 函数不太相同，因为它有一个额外的函数参数用于空案例（`fEmpty`）。但是，如果我们消除该参数并只返回累加器，我们就会得到这个变量：

```F#
module LinkedList =

    let rec fold fCons acc list :'r=
        let recurse = fold fCons
        match list with
        | Empty ->
            acc
        | Cons (element,list) ->
            let newAcc = fCons acc element
            recurse newAcc list
```

如果我们将签名与 [List.fold 文档](https://fsharp.github.io/fsharp-core-docs/reference/fsharp-collections-listmodule.html#fold)进行比较，我们可以看到它们是等效的，其中 `'State` 被 `'r` 替换，`'T List` 被 `LinkedList<'a>` 替换：

```F#
LinkedList.fold : ('r     -> 'a -> 'r    ) -> 'r      -> LinkedList<'a> -> 'r
List.fold       : ('State -> 'T -> 'State) -> 'State -> 'T list         -> 'State
```

让我们通过做一个小求和来测试 `fold` 的工作原理：

```F#
let linkedList = Cons (1, Cons (2, Cons(3, Empty)))
linkedList |> LinkedList.fold (+) 0
// Result => 6
```

### LinkedList 的 `foldBack`

最后，我们可以使用上一篇文章中描述的“函数累加器”方法创建一个 `foldBack` 函数：

```F#
module LinkedList =

    let rec cata ...

    let rec fold ...

    let foldBack fCons list acc :'r=
        let fEmpty' generator =
            generator acc
        let fCons' generator element=
            fun innerResult ->
                let newResult = fCons element innerResult
                generator newResult
        let initialGenerator = id
        foldWithEmpty fCons' fEmpty' initialGenerator  list
```

同样，如果我们将签名与 [List.foldBack 文档](https://fsharp.github.io/fsharp-core-docs/reference/fsharp-collections-listmodule.html#foldBack)进行比较，它们也是等价的，其中 `'State` 被 `'r` 替换，`'T List` 被 `LinkedList<'a>` 替换：

```F#
LinkedList.foldBack : ('a -> 'r     -> 'r    ) -> LinkedList<'a> -> 'r     -> 'r
List.foldBack       : ('T -> 'State -> 'State) -> 'T list        -> 'State -> 'State
```

### 使用 `foldBack` 在列表类型之间进行转换

在[第一篇文章](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds/#benefits)中，我们注意到同态可用于在相似结构的类型之间进行转换。

现在，让我们通过创建一些函数来演示这一点，这些函数可以从 `LinkedList` 转换为原生 `list` 类型，然后再转换回来。

要将 `LinkedList` 转换为原生列表，我们只需将 `Cons` 替换为 `::`，将 `Empty` 替换为 `[]`：

```F#
module LinkedList =

    let toList linkedList =
        let fCons head tail = head::tail
        let initialState = []
        foldBack fCons linkedList initialState
```

要转换为另一种方式，我们需要将 `::` 替换为 `Cons`，将 `[]` 替换为 `Empty`：

```F#
module LinkedList =

    let ofList list =
        let fCons head tail = Cons(head,tail)
        let initialState = Empty
        List.foldBack fCons list initialState
```

简单！让我们测试一下 `toList`：

```F#
let linkedList = Cons (1, Cons (2, Cons(3, Empty)))
linkedList |> LinkedList.toList
// Result => [1; 2; 3]
```

和 `ofList`：

```F#
let list = [1;2;3]
list |> LinkedList.ofList
// Result => Cons (1,Cons (2,Cons (3,Empty)))
```

两者都按预期工作。

### 使用 `foldBack` 实现其他函数

我之前说过，catamorphism 函数（对于线性列表，`foldBack`）是递归类型最基本的函数，事实上也是你唯一需要的函数！

让我们通过使用 `foldBack` 实现一些其他常见函数来亲眼看看。

以下是根据 `foldBack` 定义的 `map`：

```F#
module LinkedList =

    /// map a function "f" over all elements
    let map f list =
        // helper function
        let folder head tail =
            Cons(f head,tail)

        foldBack folder list Empty
```

这是测试：

```F#
let linkedList = Cons (1, Cons (2, Cons(3, Empty)))

linkedList |> LinkedList.map (fun i -> i+10)
// Result => Cons (11,Cons (12,Cons (13,Empty)))
```

以下是根据 `foldBack` 定义的 `filter`：

```F#
module LinkedList =

    /// return a new list of elements for which "pred" is true
    let filter pred list =
        // helper function
        let folder head tail =
            if pred head then
                Cons(head,tail)
            else
                tail

        foldBack folder list Empty
```

这是测试：

```F#
let isOdd n = (n%2=1)
let linkedList = Cons (1, Cons (2, Cons(3, Empty)))

linkedList |> LinkedList.filter isOdd
// Result => Cons (1,Cons (3,Empty))
```

最后，这里是根据 `fold` 定义的 `rev`：

```F#
/// reverse the elements of the list
let rev list =
    // helper function
    let folder tail head =
        Cons(head,tail)

    fold folder Empty list
```

这是测试：

```F#
let linkedList = Cons (1, Cons (2, Cons(3, Empty)))
linkedList |> LinkedList.rev
// Result => Cons (3,Cons (2,Cons (1,Empty)))
```

所以，我希望你相信！

### 避免生成器函数

我之前提到过，在不使用生成器或延续的情况下，有一种替代的（有时）更有效的方法来实现 `foldBack`。

正如我们所看到的，`foldBack` 是反向迭代，这意味着它与应用于反向列表的 `fold` 相同！

所以我们可以这样实现它：

```F#
let foldBack_ViaRev fCons list acc :'r=
    let fCons' acc element =
        // just swap the params!
        fCons element acc
    list
    |> rev
    |> fold fCons' acc
```

它涉及制作列表的额外副本，但另一方面，不再有大量未决的延续。如果性能是一个问题，那么在您的环境中比较两个版本的配置文件可能是值得的。

## 使 Gift 领域泛型

在本文的其余部分，我们将研究 `Gift` 类型，看看是否可以使其更通用。

作为提醒，这是原始设计：

```F#
type Gift =
    | Book of Book
    | Chocolate of Chocolate
    | Wrapped of Gift * WrappingPaperStyle
    | Boxed of Gift
    | WithACard of Gift * message:string
```

其中三种情况是递归的，两种是非递归的。

现在，这种特殊设计的重点是对领域进行建模，这就是为什么有这么多单独的案例。

但是，如果我们想专注于*可重用性*而不是领域建模，那么我们应该将设计简化到本质，而所有这些特殊情况现在都成为了障碍。

为了准备好重用，那么，让我们将所有非递归案例合并为一个案例，比如 `GiftContents`，并将所有递归案例合并到另一个案例中，比如 `GiftDecoration`，如下所示：

```F#
// unified data for non-recursive cases
type GiftContents =
    | Book of Book
    | Chocolate of Chocolate

// unified data for recursive cases
type GiftDecoration =
    | Wrapped of WrappingPaperStyle
    | Boxed
    | WithACard of string

type Gift =
    // non-recursive case
    | Contents of GiftContents
    // recursive case
    | Decoration of Gift * GiftDecoration
```

主 `Gift` 类型现在只有两种情况：非递归和递归。

## 定义泛型容器类型

既然类型已经简化，我们可以通过允许任何类型的内容和任何类型的装饰来“泛化”它。

```F#
type Container<'ContentData,'DecorationData> =
    | Contents of 'ContentData
    | Decoration of 'DecorationData * Container<'ContentData,'DecorationData>
```

和以前一样，我们可以使用标准流程机械地为它创建一个 `cata`  和 `fold` 和 `foldBack`：

```F#
module Container =

    let rec cata fContents fDecoration (container:Container<'ContentData,'DecorationData>) :'r =
        let recurse = cata fContents fDecoration
        match container with
        | Contents contentData ->
            fContents contentData
        | Decoration (decorationData,subContainer) ->
            fDecoration decorationData (recurse subContainer)

    (*
    val cata :
        // function parameters
        fContents:('ContentData -> 'r) ->
        fDecoration:('DecorationData -> 'r -> 'r) ->
        // input
        container:Container<'ContentData,'DecorationData> ->
        // return value
        'r
    *)

    let rec fold fContents fDecoration acc (container:Container<'ContentData,'DecorationData>) :'r =
        let recurse = fold fContents fDecoration
        match container with
        | Contents contentData ->
            fContents acc contentData
        | Decoration (decorationData,subContainer) ->
            let newAcc = fDecoration acc decorationData
            recurse newAcc subContainer

    (*
    val fold :
        // function parameters
        fContents:('a -> 'ContentData -> 'r) ->
        fDecoration:('a -> 'DecorationData -> 'a) ->
        // accumulator
        acc:'a ->
        // input
        container:Container<'ContentData,'DecorationData> ->
        // return value
        'r
    *)

    let foldBack fContents fDecoration (container:Container<'ContentData,'DecorationData>) :'r =
        let fContents' generator contentData =
            generator (fContents contentData)
        let fDecoration' generator decorationData =
            let newGenerator innerValue =
                let newInnerValue = fDecoration decorationData innerValue
                generator newInnerValue
            newGenerator
        fold fContents' fDecoration' id container

    (*
    val foldBack :
        // function parameters
        fContents:('ContentData -> 'r) ->
        fDecoration:('DecorationData -> 'r -> 'r) ->
        // input
        container:Container<'ContentData,'DecorationData> ->
        // return value
        'r
    *)
```

### 将礼品域转换为使用容器类型

让我们将礼物类型转换为这种泛型的容器类型：

```F#
type Gift = Container<GiftContents,GiftDecoration>
```

现在我们需要一些辅助方法来构造值，同时隐藏泛型类型的“真实”情况：

```F#
let fromBook book =
    Contents (Book book)

let fromChoc choc =
    Contents (Chocolate choc)

let wrapInPaper paperStyle innerGift =
    let container = Wrapped paperStyle
    Decoration (container, innerGift)

let putInBox innerGift =
    let container = Boxed
    Decoration (container, innerGift)

let withCard message innerGift =
    let container = WithACard message
    Decoration (container, innerGift)
```

最后，我们可以创建一些测试值：

```F#
let wolfHall = {title="Wolf Hall"; price=20m}
let yummyChoc = {chocType=SeventyPercent; price=5m}

let birthdayPresent =
    wolfHall
    |> fromBook
    |> wrapInPaper HappyBirthday
    |> withCard "Happy Birthday"

let christmasPresent =
    yummyChoc
    |> fromChoc
    |> putInBox
    |> wrapInPaper HappyHolidays
```

### 使用容器类型的 `totalCost` 函数

“总成本”函数可以使用 `fold` 编写，因为它不需要任何内部数据。

与早期的实现不同，我们只有两个函数参数 `fColutes` 和 `fDecoration`，因此每个参数都需要一些模式匹配来获取“真实”数据。

代码如下：

```F#
let totalCost gift =

    let fContents costSoFar contentData =
        match contentData with
        | Book book ->
            costSoFar + book.price
        | Chocolate choc ->
            costSoFar + choc.price

    let fDecoration costSoFar decorationInfo =
        match decorationInfo with
        | Wrapped style ->
            costSoFar + 0.5m
        | Boxed ->
            costSoFar + 1.0m
        | WithACard message ->
            costSoFar + 2.0m

    // initial accumulator
    let initialAcc = 0m

    // call the fold
    Container.fold fContents fDecoration initialAcc gift
```

代码按预期工作：

```F#
birthdayPresent |> totalCost
// 22.5m

christmasPresent |> totalCost
// 6.5m
```

### 使用容器类型的 `description` 函数

“description”函数需要使用 `foldBack` 编写，因为它确实需要内部数据。与上面的代码一样，我们需要一些模式匹配来获取每种情况的“真实”数据。

```F#
let description gift =

    let fContents contentData =
        match contentData with
        | Book book ->
            sprintf "'%s'" book.title
        | Chocolate choc ->
            sprintf "%A chocolate" choc.chocType

    let fDecoration decorationInfo innerText =
        match decorationInfo with
        | Wrapped style ->
            sprintf "%s wrapped in %A paper" innerText style
        | Boxed ->
            sprintf "%s in a box" innerText
        | WithACard message ->
            sprintf "%s with a card saying '%s'" innerText message

    // main call
    Container.foldBack fContents fDecoration gift
```

代码再次按照我们的意愿工作：

```F#
birthdayPresent |> description
// CORRECT "'Wolf Hall' wrapped in HappyBirthday paper with a card saying 'Happy Birthday'"

christmasPresent |> description
// CORRECT "SeventyPercent chocolate in a box wrapped in HappyHolidays paper"
```

## 实现礼物域的第三种方法

这一切看起来都很不错，不是吗？

但我必须承认，我一直在隐瞒一些事情。

上面的代码都不是绝对必要的，因为事实证明，还有*另一种*方法可以对 `Gift` 进行建模，而无需创建任何新的泛型类型！

`Gift` 类型基本上是装饰的线性序列，最后一步是一些内容。我们可以将其建模为一对——一个 `Content` 和一个 `Decoration` 列表。或者让它更友好一点，一个有两个字段的记录：一个用于内容，一个用于装饰。

```F#
type Gift = {contents: GiftContents; decorations: GiftDecoration list}
```

就是这样！不需要其他新类型！

### 使用记录类型构建值

如前所述，让我们创建一些助手来使用此类型构造值：

```F#
let fromBook book =
    { contents = (Book book); decorations = [] }

let fromChoc choc =
    { contents = (Chocolate choc); decorations = [] }

let wrapInPaper paperStyle innerGift =
    let decoration = Wrapped paperStyle
    { innerGift with decorations = decoration::innerGift.decorations }

let putInBox innerGift =
    let decoration = Boxed
    { innerGift with decorations = decoration::innerGift.decorations }

let withCard message innerGift =
    let decoration = WithACard message
    { innerGift with decorations = decoration::innerGift.decorations }
```

使用这些辅助函数，值的构造方式与之前的版本相同。这就是为什么隐藏你的原始构造函数是件好事，伙计们！

```F#
let wolfHall = {title="Wolf Hall"; price=20m}
let yummyChoc = {chocType=SeventyPercent; price=5m}

let birthdayPresent =
    wolfHall
    |> fromBook
    |> wrapInPaper HappyBirthday
    |> withCard "Happy Birthday"

let christmasPresent =
    yummyChoc
    |> fromChoc
    |> putInBox
    |> wrapInPaper HappyHolidays
```

### 使用记录类型的 `totalCost` 函数

现在编写 `totalCost` 函数更容易了。

```F#
let totalCost gift =

    let contentCost =
        match gift.contents with
        | Book book ->
            book.price
        | Chocolate choc ->
            choc.price

    let decorationFolder costSoFar decorationInfo =
        match decorationInfo with
        | Wrapped style ->
            costSoFar + 0.5m
        | Boxed ->
            costSoFar + 1.0m
        | WithACard message ->
            costSoFar + 2.0m

    let decorationCost =
        gift.decorations |> List.fold decorationFolder 0m

    // total cost
    contentCost + decorationCost
```

### 使用记录类型的 `description` 函数

同样，`description` 函数也易于编写。

```F#
let description gift =

    let contentDescription =
        match gift.contents with
        | Book book ->
            sprintf "'%s'" book.title
        | Chocolate choc ->
            sprintf "%A chocolate" choc.chocType

    let decorationFolder decorationInfo innerText =
        match decorationInfo with
        | Wrapped style ->
            sprintf "%s wrapped in %A paper" innerText style
        | Boxed ->
            sprintf "%s in a box" innerText
        | WithACard message ->
            sprintf "%s with a card saying '%s'" innerText message

    List.foldBack decorationFolder gift.decorations contentDescription
```

## 抽象还是具体？比较三种设计

如果你对如此多的设计感到困惑，我不怪你！

但事实上，这三个不同的定义实际上是可以互换的：

**原始版本**

```F#
type Gift =
    | Book of Book
    | Chocolate of Chocolate
    | Wrapped of Gift * WrappingPaperStyle
    | Boxed of Gift
    | WithACard of Gift * message:string
```

**泛型容器版本**

```F#
type Container<'ContentData,'DecorationData> =
    | Contents of 'ContentData
    | Decoration of 'DecorationData * Container<'ContentData,'DecorationData>

type GiftContents =
    | Book of Book
    | Chocolate of Chocolate

type GiftDecoration =
    | Wrapped of WrappingPaperStyle
    | Boxed
    | WithACard of string

type Gift = Container<GiftContents,GiftDecoration>
```

**记录版本**

```F#
type GiftContents =
    | Book of Book
    | Chocolate of Chocolate

type GiftDecoration =
    | Wrapped of WrappingPaperStyle
    | Boxed
    | WithACard of string

type Gift = {contents: GiftContents; decorations: GiftDecoration list}
```

如果这不明显，阅读我关于数据类型大小的文章可能会有所帮助。它解释了两种类型如何“等效”，即使乍一看它们似乎完全不同。

### 挑选设计

那么，哪种设计最好呢？一如既往，答案是“这取决于”。

对于建模和记录一个领域，我喜欢第一个有五个明确案例的设计。对我来说，易于他人理解比为了可重用性而引入抽象更重要。

如果我想要一个适用于许多情况的可重用模型，我可能会选择第二种（“容器”）设计。在我看来，这种类型确实代表了一种常见的情况，内容是一种东西，包装是另一种东西。因此，这种抽象可能会得到一些使用。

最终的“配对”模型很好，但通过将两个组件分开，我们过度抽象了这种情况下的设计。在其他情况下，这种设计可能非常适合（例如装饰器模式），但在我看来，在这里不是这样。

还有一个选择，它会给你最好的世界。

正如我上面提到的，所有的设计在逻辑上都是等价的，这意味着它们之间存在“无损”映射。在这种情况下，你的“公共”设计可以是面向领域的，就像第一个一样，但在幕后，你可以将其映射到更高效、更可重用的“私有”类型。

甚至 F# 列表实现本身也能做到这一点。例如，`List` 模块中的一些函数，如 `foldBack` 和 `sort`，将列表转换为数组，执行操作，然后再次将其转换回列表。

## 摘要

在这篇文章中，我们研究了将 `Gift` 建模为泛型类型的一些方法，以及每种方法的优缺点。

在[下一篇文章](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/)中，我们将查看使用泛型递归类型的真实示例。

*这篇文章的源代码可以在[这里 gist](https://gist.github.com/swlaschin/c423a0f78b22496a0aff)找到。*

# 6 现实世界中的树

*Part of the "Recursive types and folds" series (*[link](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#series-toc)*)*

使用数据库、JSON 和错误处理的示例
25八月2015这篇文章是超过3 年

https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/

这篇文章是系列文章中的第六篇。

在[上一篇文章](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3/)中，我们简要介绍了一些泛型类型。

在这篇文章中，我们将更深入地探讨使用树和折叠的一些现实世界的例子。

## 系列内容

以下是本系列的内容：

- **第 1 部分：递归类型和分解形态（catamorphisms）介绍**
  - [一个简单的递归类型](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds/#basic-recursive-type)
  - [对所有事物进行参数化](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds/#parameterize)
  - [介绍分解形态](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds/#catamorphisms)
  - [变形的好处](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds/#benefits)
  - [创建分解形态的规则](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds/#rules)
- **第 2 部分：分解形态示例**
  - [变形示例：文件系统域](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-1b/#file-system)
  - [变形示例：产品域](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-1b/#product)
- **第 3 部分：介绍折叠**
  - [我们的分解形态实现中的一个缺陷](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2/#flaw)
  - [介绍 `fold`](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2/#fold)
  - [折叠问题](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2/#problems)
  - [将函数用作累加器](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2/#functions)
  - [介绍 `foldback`](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2/#foldback)
  - [创建折叠的规则](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2/#rules)
- **第 4 部分：了解折叠**
  - [迭代与递归](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2b/#iteration)
  - [折叠示例：文件系统域](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2b/#file-system)
  - [关于“折叠”的常见问题](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-2b/#questions)
- **第 5 部分：泛型递归类型**
  - [LinkedList：一种通用的递归类型](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3/#linkedlist)
  - [使礼品领域泛型](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3/#revisiting-gift)
  - [定义泛型容器类型](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3/#container)
  - [实现礼物领域的第三种方法](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3/#another-gift)
  - [抽象还是具体？比较三种设计](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3/#compare)
- **第 6 部分：现实世界中的树**
  - [定义通用树类型](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#tree)
  - [现实世界中的树类型](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#reuse)
  - [映射树类型](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#map)
  - [示例：创建目录列表](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#listing)
  - [示例：并行 grep](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#grep)
  - [示例：将文件系统存储在数据库中](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#database)
  - [示例：将树序列化为 JSON](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#tojson)
  - [示例：从 JSON 反序列化树](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#fromjson)
  - [示例：从 JSON 反序列化树 - 带错误处理](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-3b/#json-with-error-handling)

## 定义泛型树类型

在这篇文章中，我们将使用一个受我们之前探索的 `FileSystem` 域启发的泛型 `Tree`。

这是最初的设计：

```F#
type FileSystemItem =
    | File of FileInfo
    | Directory of DirectoryInfo
and FileInfo = {name:string; fileSize:int}
and DirectoryInfo = {name:string; dirSize:int; subitems:FileSystemItem list}
```

我们可以从递归中分离出数据，并创建一个泛型的 `Tree` 类型，如下所示：

```F#
type Tree<'LeafData,'INodeData> =
    | LeafNode of 'LeafData
    | InternalNode of 'INodeData * Tree<'LeafData,'INodeData> seq
```

请注意，我使用 `seq` 来表示子项，而不是 `list`。原因很快就会显现出来。

然后，可以使用 `Tree` 对文件系统域进行建模，方法是将 `FileInfo` 指定为与叶子节点关联的数据，将 `DirectoryInfo` 指定为和内部节点关联的信息：

```F#
type FileInfo = {name:string; fileSize:int}
type DirectoryInfo = {name:string; dirSize:int}

type FileSystemItem = Tree<FileInfo,DirectoryInfo>
```

### Tree 的 `cata` 和 `fold`

我们可以用通常的方式定义 `cata` 和 `fold`：

```F#
module Tree =

    let rec cata fLeaf fNode (tree:Tree<'LeafData,'INodeData>) :'r =
        let recurse = cata fLeaf fNode
        match tree with
        | LeafNode leafInfo ->
            fLeaf leafInfo
        | InternalNode (nodeInfo,subtrees) ->
            fNode nodeInfo (subtrees |> Seq.map recurse)

    let rec fold fLeaf fNode acc (tree:Tree<'LeafData,'INodeData>) :'r =
        let recurse = fold fLeaf fNode
        match tree with
        | LeafNode leafInfo ->
            fLeaf acc leafInfo
        | InternalNode (nodeInfo,subtrees) ->
            // determine the local accumulator at this level
            let localAccum = fNode acc nodeInfo
            // thread the local accumulator through all the subitems using Seq.fold
            let finalAccum = subtrees |> Seq.fold recurse localAccum
            // ... and return it
            finalAccum
```

请注意，我不会为 `Tree` 类型实现 `foldBack`，因为树不太可能太深而导致堆栈溢出。需要内部数据的函数可以使用 `cata`。

### 用树对文件系统域进行建模

让我们用之前使用的相同值进行测试：

```F#
let fromFile (fileInfo:FileInfo) =
    LeafNode fileInfo

let fromDir (dirInfo:DirectoryInfo) subitems =
    InternalNode (dirInfo,subitems)

let readme = fromFile {name="readme.txt"; fileSize=1}
let config = fromFile {name="config.xml"; fileSize=2}
let build  = fromFile {name="build.bat"; fileSize=3}
let src = fromDir {name="src"; dirSize=10} [readme; config; build]
let bin = fromDir {name="bin"; dirSize=10} []
let root = fromDir {name="root"; dirSize=5} [src; bin]
```

`totalSize` 函数与上一篇文章中的函数几乎相同：

```F#
let totalSize fileSystemItem =
    let fFile acc (file:FileInfo) =
        acc + file.fileSize
    let fDir acc (dir:DirectoryInfo)=
        acc + dir.dirSize
    Tree.fold fFile fDir 0 fileSystemItem

readme |> totalSize  // 1
src |> totalSize     // 16 = 10 + (1 + 2 + 3)
root |> totalSize    // 31 = 5 + 16 + 10
```

`largestFile` 函数也是如此：

```F#
let largestFile fileSystemItem =
    let fFile (largestSoFarOpt:FileInfo option) (file:FileInfo) =
        match largestSoFarOpt with
        | None ->
            Some file
        | Some largestSoFar ->
            if largestSoFar.fileSize > file.fileSize then
                Some largestSoFar
            else
                Some file

    let fDir largestSoFarOpt dirInfo =
        largestSoFarOpt

    // call the fold
    Tree.fold fFile fDir None fileSystemItem

readme |> largestFile
// Some {name = "readme.txt"; fileSize = 1}

src |> largestFile
// Some {name = "build.bat"; fileSize = 3}

bin |> largestFile
// None

root |> largestFile
// Some {name = "build.bat"; fileSize = 3}
```

本节的源代码可以在[此处 gist](https://gist.github.com/swlaschin/1ef784481bae91b63a36)找到。

## 现实世界中的树类型

我们也可以使用树来模拟真实的文件系统！为此，只需将叶子节点类型设置为 `System.IO.FileInfo` 和内部节点类型为 `System.IO.DirectoryInfo`。

```F#
open System
open System.IO

type FileSystemTree = Tree<IO.FileInfo,IO.DirectoryInfo>
```

让我们创建一些辅助方法来创建各种节点：

```F#
let fromFile (fileInfo:FileInfo) =
    LeafNode fileInfo

let rec fromDir (dirInfo:DirectoryInfo) =
    let subItems = seq{
        yield! dirInfo.EnumerateFiles() |> Seq.map fromFile
        yield! dirInfo.EnumerateDirectories() |> Seq.map fromDir
        }
    InternalNode (dirInfo,subItems)
```

现在你可以看到我为什么对子项使用 `seq` 而不是 `list`。`seq` 是惰性的，这意味着我们可以在不实际撞击磁盘的情况下创建节点。

这是 `totalSize` 函数，这次使用的是真实的文件信息：

```F#
let totalSize fileSystemItem =
    let fFile acc (file:FileInfo) =
        acc + file.Length
    let fDir acc (dir:DirectoryInfo)=
        acc
    Tree.fold fFile fDir 0L fileSystemItem
```

让我们看看当前目录的大小：

```F#
// set the current directory to the current source directory
Directory.SetCurrentDirectory __SOURCE_DIRECTORY__

// get the current directory as a Tree
let currentDir = fromDir (DirectoryInfo("."))

// get the size of the current directory
currentDir  |> totalSize
```

同样，我们可以得到最大的文件：

```F#
let largestFile fileSystemItem =
    let fFile (largestSoFarOpt:FileInfo option) (file:FileInfo) =
        match largestSoFarOpt with
        | None ->
            Some file
        | Some largestSoFar ->
            if largestSoFar.Length > file.Length then
                Some largestSoFar
            else
                Some file

    let fDir largestSoFarOpt dirInfo =
        largestSoFarOpt

    // call the fold
    Tree.fold fFile fDir None fileSystemItem

currentDir |> largestFile
```

这就是使用泛型递归类型的一大好处。如果我们能将现实世界的层次结构转化为树形结构，我们就可以“免费”获得折叠的所有好处。

## 使用泛型类型进行映射

使用泛型类型的另一个优点是，你可以做像 `map` 这样的事情——在不改变结构的情况下将每个元素转换为新类型。

我们可以在现实世界的文件系统中看到这一点。但首先我们需要为 `Tree` 类型定义 `map`！

`map` 的实现也可以使用以下规则机械地完成：

- 创建一个函数参数来处理结构中的每种情况。
- 对于非递归情况
  - 首先，使用函数参数转换与该情况相关的非递归数据
  - 然后将结果包装在同一个case构造函数中
- 对于递归情况，执行两个步骤：
  - 首先，使用函数参数转换与该情况相关的非递归数据
  - 接下来，递归 `map` 嵌套值。
  - 最后，将结果包装在同一个case构造函数中

以下是通过遵循这些规则创建的 `Tree` 的 `map` 的实现：

```F#
module Tree =

    let rec cata ...

    let rec fold ...

    let rec map fLeaf fNode (tree:Tree<'LeafData,'INodeData>) =
        let recurse = map fLeaf fNode
        match tree with
        | LeafNode leafInfo ->
            let newLeafInfo = fLeaf leafInfo
            LeafNode newLeafInfo
        | InternalNode (nodeInfo,subtrees) ->
            let newNodeInfo = fNode nodeInfo
            let newSubtrees = subtrees |> Seq.map recurse
            InternalNode (newNodeInfo, newSubtrees)
```

如果我们查看 `Tree.map` 的签名，我们可以看到所有叶子数据都被转换为类型 `'a`，所有节点数据都转换为类型 `'b`，最终结果是一棵 `Tree<'a，'b>`。

```F#
val map :
  fLeaf:('LeafData -> 'a) ->
  fNode:('INodeData -> 'b) ->
  tree:Tree<'LeafData,'INodeData> ->
  Tree<'a,'b>
```

我们可以用类似的方式定义 `Tree.iter`：

```F#
module Tree =

    let rec map ...

    let rec iter fLeaf fNode (tree:Tree<'LeafData,'INodeData>) =
        let recurse = iter fLeaf fNode
        match tree with
        | LeafNode leafInfo ->
            fLeaf leafInfo
        | InternalNode (nodeInfo,subtrees) ->
            subtrees |> Seq.iter recurse
            fNode nodeInfo
```

## 示例：创建目录列表

假设我们想使用 `map` 将文件系统转换为目录列表——一个字符串树，其中每个字符串都有关于相应文件或目录的信息。我们可以这样做：

```F#
let dirListing fileSystemItem =
    let printDate (d:DateTime) = d.ToString()
    let mapFile (fi:FileInfo) =
        sprintf "%10i  %s  %-s"  fi.Length (printDate fi.LastWriteTime) fi.Name
    let mapDir (di:DirectoryInfo) =
        di.FullName
    Tree.map mapFile mapDir fileSystemItem
```

然后我们可以像这样打印字符串：

```F#
currentDir
|> dirListing
|> Tree.iter (printfn "%s") (printfn "\n%s")
```

结果看起来像这样：

```
  8315  10/08/2015 23:37:41  Fold.fsx
  3680  11/08/2015 23:59:01  FoldAndRecursiveTypes.fsproj
  1010  11/08/2015 01:19:07  FoldAndRecursiveTypes.sln
  1107  11/08/2015 23:59:01  HtmlDom.fsx
    79  11/08/2015 01:21:54  LinkedList.fsx
```

*这个例子的源代码就在[这里 gist](https://gist.github.com/swlaschin/77fadc19acb8cc850276)。*

## 示例：创建并行 grep

让我们来看一个更复杂的例子。我将演示如何使用 `fold` 创建并行的“grep”风格搜索。

逻辑如下：

- 使用 `fold` 迭代文件。
- 对于每个文件，如果其名称与所需的文件模式不匹配，则返回 `None`。
- 如果要处理文件，则返回一个 async，返回文件中的所有行匹配。
- 接下来，所有这些异步——折叠的输出——被聚合成一个序列。
- 使用返回结果列表的 `Async.Parallel` 将异步序列转换为单个异步序列。

在开始编写主代码之前，我们需要一些辅助函数。

首先，一个异步折叠文件行的泛型函数。这将是模式匹配的基础。

```F#
/// Fold over the lines in a file asynchronously
/// passing in the current line and line number to the folder function.
///
/// Signature:
///   folder:('a -> int -> string -> 'a) ->
///   acc:'a ->
///   fi:FileInfo ->
///   Async<'a>
let foldLinesAsync folder acc (fi:FileInfo) =
    async {
        let mutable acc = acc
        let mutable lineNo = 1
        use sr = new StreamReader(path=fi.FullName)
        while not sr.EndOfStream do
            let! lineText = sr.ReadLineAsync() |> Async.AwaitTask
            acc <- folder acc lineNo lineText
            lineNo <- lineNo + 1
        return acc
    }
```

接下来，一个小助手，允许我们对 `Async` 值 `map`：

```F#
let asyncMap f asyncX = async {
    let! x = asyncX
    return (f x)  }
```

现在谈谈中心逻辑。我们将创建一个函数，给定一个 `textPattern` 和一个 `FileInfo`，该函数将异步返回一个与 textPattern 匹配的行列表：

```F#
/// return the matching lines in a file, as an async<string list>
let matchPattern textPattern (fi:FileInfo) =
    // set up the regex
    let regex = Text.RegularExpressions.Regex(pattern=textPattern)

    // set up the function to use with "fold"
    let folder results lineNo lineText =
        if regex.IsMatch lineText then
            let result = sprintf "%40s:%-5i   %s" fi.Name lineNo lineText
            result :: results
        else
            // pass through
            results

    // main flow
    fi
    |> foldLinesAsync folder []
    // the fold output is in reverse order, so reverse it
    |> asyncMap List.rev
```

现在来看 `grep` 函数本身：

```F#
let grep filePattern textPattern fileSystemItem =
    let regex = Text.RegularExpressions.Regex(pattern=filePattern)

    /// if the file matches the pattern
    /// do the matching and return Some async, else None
    let matchFile (fi:FileInfo) =
        if regex.IsMatch fi.Name then
            Some (matchPattern textPattern fi)
        else
            None

    /// process a file by adding its async to the list
    let fFile asyncs (fi:FileInfo) =
        // add to the list of asyncs
        (matchFile fi) :: asyncs

    // for directories, just pass through the list of asyncs
    let fDir asyncs (di:DirectoryInfo)  =
        asyncs

    fileSystemItem
    |> Tree.fold fFile fDir []    // get the list of asyncs
    |> Seq.choose id              // choose the Somes (where a file was processed)
    |> Async.Parallel             // merge all asyncs into a single async
    |> asyncMap (Array.toList >> List.collect id)  // flatten array of lists into a single list
```

让我们来测试一下！

```F#
currentDir
|> grep "fsx" "LinkedList"
|> Async.RunSynchronously
```

结果看起来像这样：

```
"                  SizeOfTypes.fsx:120     type LinkedList<'a> = ";
"                  SizeOfTypes.fsx:122         | Cell of head:'a * tail:LinkedList<'a>";
"                  SizeOfTypes.fsx:125     let S = size(LinkedList<'a>)";
"      RecursiveTypesAndFold-3.fsx:15      // LinkedList";
"      RecursiveTypesAndFold-3.fsx:18      type LinkedList<'a> = ";
"      RecursiveTypesAndFold-3.fsx:20          | Cons of head:'a * tail:LinkedList<'a>";
"      RecursiveTypesAndFold-3.fsx:26      module LinkedList = ";
"      RecursiveTypesAndFold-3.fsx:39              list:LinkedList<'a> ";
"      RecursiveTypesAndFold-3.fsx:64              list:LinkedList<'a> -> ";
```

对于大约 40 行代码来说，这还不错。这种简洁性是因为我们使用了各种隐藏递归的 `fold` 和 `map`，使我们能够专注于模式匹配逻辑本身。

当然，这根本不是高效或优化的（每行都是异步的！），所以我不会把它当作一个真正的实现，但它确实让你了解了fold的强大功能。

这个例子的源代码就在[这里 gist](https://gist.github.com/swlaschin/137c322b5a46b97cc8be)。

## 示例：将文件系统存储在数据库中

对于下一个示例，让我们看看如何在数据库中存储文件系统树。我真的不知道你为什么要这样做，但这些原则同样适用于存储任何层次结构，所以无论如何我都会演示它！

要对数据库中的文件系统层次结构进行建模，假设我们有四个表：

- `DbDir` 存储有关每个目录的信息。
- `DbFile` 存储有关每个文件的信息。
- `DbDir_File` 存储目录和文件之间的关系。
- `DbDir_Dir` 存储父目录和子目录之间的关系。

以下是数据库表定义：

```sql
CREATE TABLE DbDir (
	DirId int IDENTITY NOT NULL,
	Name nvarchar(50) NOT NULL
)

CREATE TABLE DbFile (
	FileId int IDENTITY NOT NULL,
	Name nvarchar(50) NOT NULL,
	FileSize int NOT NULL
)

CREATE TABLE DbDir_File (
	DirId int NOT NULL,
	FileId int NOT NULL
)

CREATE TABLE DbDir_Dir (
	ParentDirId int NOT NULL,
	ChildDirId int NOT NULL
)
```

这很简单。但是请注意，为了完全保存目录及其与子项的关系，我们首先需要其所有子项的id，每个子目录都需要其子项的 ids，以此类推。

这意味着我们应该使用 `cata` 而不是 `fold`，这样我们就可以从层次结构的较低级别访问数据。

### 实现数据库功能

我们不够明智，没有使用 [SQL Provider](https://fsprojects.github.io/SQLProvider/)，所以我们编写了自己的表插入函数，就像这个虚拟函数一样：

```F#
/// Insert a DbFile record
let insertDbFile name (fileSize:int64) =
    let id = nextIdentity()
    printfn "%10s: inserting id:%i name:%s size:%i" "DbFile" id name fileSize
```

在真实的数据库中，identity 列会自动为您生成，但对于这个例子，我将使用一个小助手函数 `nextIdentity`：

```F#
let nextIdentity =
    let id = ref 0
    fun () ->
        id := !id + 1
        !id

// test
nextIdentity() // 1
nextIdentity() // 2
nextIdentity() // 3
```

现在，为了插入目录，我们需要首先知道目录中文件的所有 id。这意味着 `insertDbFile` 函数应该返回生成的id。

```F#
/// Insert a DbFile record and return the new file id
let insertDbFile name (fileSize:int64) =
    let id = nextIdentity()
    printfn "%10s: inserting id:%i name:%s size:%i" "DbFile" id name fileSize
    id
```

但这种逻辑也适用于目录：

```F#
/// Insert a DbDir record and return the new directory id
let insertDbDir name =
    let id = nextIdentity()
    printfn "%10s: inserting id:%i name:%s" "DbDir" id name
    id
```

但这还不够好。当子 ID 传递给父目录时，需要区分文件和目录，因为它们的关系存储在不同的表中。

没问题，我们只需使用选择类型来区分它们！

```F#
type PrimaryKey =
    | FileId of int
    | DirId of int
```

有了这个，我们就可以完成数据库功能的实现：

```F#
/// Insert a DbFile record and return the new PrimaryKey
let insertDbFile name (fileSize:int64) =
    let id = nextIdentity()
    printfn "%10s: inserting id:%i name:%s size:%i" "DbFile" id name fileSize
    FileId id

/// Insert a DbDir record and return the new PrimaryKey
let insertDbDir name =
    let id = nextIdentity()
    printfn "%10s: inserting id:%i name:%s" "DbDir" id name
    DirId id

/// Insert a DbDir_File record
let insertDbDir_File dirId fileId =
    printfn "%10s: inserting parentDir:%i childFile:%i" "DbDir_File" dirId fileId

/// Insert a DbDir_Dir record
let insertDbDir_Dir parentDirId childDirId =
    printfn "%10s: inserting parentDir:%i childDir:%i" "DbDir_Dir" parentDirId childDirId
```

### 使用分解形态

如上所述，我们需要使用 `cata` 而不是 `fold`，因为我们在每一步都需要内部 id。

处理 `File` 案例的函数很容易——只需插入它并返回 `PrimaryKey`。

```F#
let fFile (fi:FileInfo) =
    insertDbFile fi.Name fi.Length
```

处理 `Directory` 案例的函数将传递 `DirectoryInfo` 和已插入的子项中的 `PrimaryKeys` 序列。

它应该插入主目录记录，然后插入子目录，然后返回下一个更高级别的 `PrimaryKey`：

```F#
let fDir (di:DirectoryInfo) childIds  =
    let dirId = insertDbDir di.Name
    // insert the children
    // return the id to the parent
    dirId
```

插入目录记录并获取其 id 后，对于每个子 id，我们根据 `childId` 的类型将其插入 `DbDir_File` 表或 `DbDir_Dir`。

```F#
let fDir (di:DirectoryInfo) childIds  =
    let dirId = insertDbDir di.Name
    let parentPK = pkToInt dirId
    childIds |> Seq.iter (fun childId ->
        match childId with
        | FileId fileId -> insertDbDir_File parentPK fileId
        | DirId childDirId -> insertDbDir_Dir parentPK childDirId
    )
    // return the id to the parent
    dirId
```

请注意，我还创建了一个小助手函数 `pkToInt`，用于从 `PrimaryKey` 类型中提取整数 id。

以下是一个块中的所有代码：

```F#
open System
open System.IO

let nextIdentity =
    let id = ref 0
    fun () ->
        id := !id + 1
        !id

type PrimaryKey =
    | FileId of int
    | DirId of int

/// Insert a DbFile record and return the new PrimaryKey
let insertDbFile name (fileSize:int64) =
    let id = nextIdentity()
    printfn "%10s: inserting id:%i name:%s size:%i" "DbFile" id name fileSize
    FileId id

/// Insert a DbDir record and return the new PrimaryKey
let insertDbDir name =
    let id = nextIdentity()
    printfn "%10s: inserting id:%i name:%s" "DbDir" id name
    DirId id

/// Insert a DbDir_File record
let insertDbDir_File dirId fileId =
    printfn "%10s: inserting parentDir:%i childFile:%i" "DbDir_File" dirId fileId

/// Insert a DbDir_Dir record
let insertDbDir_Dir parentDirId childDirId =
    printfn "%10s: inserting parentDir:%i childDir:%i" "DbDir_Dir" parentDirId childDirId

let pkToInt primaryKey =
    match primaryKey with
    | FileId fileId -> fileId
    | DirId dirId -> dirId

let insertFileSystemTree fileSystemItem =

    let fFile (fi:FileInfo) =
        insertDbFile fi.Name fi.Length

    let fDir (di:DirectoryInfo) childIds  =
        let dirId = insertDbDir di.Name
        let parentPK = pkToInt dirId
        childIds |> Seq.iter (fun childId ->
            match childId with
            | FileId fileId -> insertDbDir_File parentPK fileId
            | DirId childDirId -> insertDbDir_Dir parentPK childDirId
        )
        // return the id to the parent
        dirId

    fileSystemItem
    |> Tree.cata fFile fDir
```

现在让我们测试一下：

```F#
// get the current directory as a Tree
let currentDir = fromDir (DirectoryInfo("."))

// insert into the database
currentDir
|> insertFileSystemTree
```

输出应该看起来像这样：

```
     DbDir: inserting id:41 name:FoldAndRecursiveTypes
    DbFile: inserting id:42 name:Fold.fsx size:8315
DbDir_File: inserting parentDir:41 childFile:42
    DbFile: inserting id:43 name:FoldAndRecursiveTypes.fsproj size:3680
DbDir_File: inserting parentDir:41 childFile:43
    DbFile: inserting id:44 name:FoldAndRecursiveTypes.sln size:1010
DbDir_File: inserting parentDir:41 childFile:44
...
     DbDir: inserting id:57 name:bin
     DbDir: inserting id:58 name:Debug
 DbDir_Dir: inserting parentDir:57 childDir:58
 DbDir_Dir: inserting parentDir:41 childDir:57
```

您可以看到，在迭代文件时会生成 id，并且每个 `DbFile` 插入后面都有一个 `DbDir_File` 插入。

这个例子的源代码就在[这里 gist](https://gist.github.com/swlaschin/3a416f26d873faa84cde)。

## 示例：将树序列化为 JSON

让我们看看另一个常见的挑战：将树序列化和反序列化为 JSON、XML 或其他格式。

让我们再次使用 Gift 域，但这次我们将把 `Gift` 类型建模为树。这意味着我们可以在一个盒子里放不止一件东西！

### 将礼品领域建模为树

以下是主要类型，但请注意，最终的 `Gift` 类型被定义为树：

```F#
type Book = {title: string; price: decimal}
type ChocolateType = Dark | Milk | SeventyPercent
type Chocolate = {chocType: ChocolateType ; price: decimal}

type WrappingPaperStyle =
    | HappyBirthday
    | HappyHolidays
    | SolidColor

// unified data for non-recursive cases
type GiftContents =
    | Book of Book
    | Chocolate of Chocolate

// unified data for recursive cases
type GiftDecoration =
    | Wrapped of WrappingPaperStyle
    | Boxed
    | WithACard of string

type Gift = Tree<GiftContents,GiftDecoration>
```

像往常一样，我们可以创建一些辅助函数来帮助构造 `Gift`：

```F#
let fromBook book =
    LeafNode (Book book)

let fromChoc choc =
    LeafNode (Chocolate choc)

let wrapInPaper paperStyle innerGift =
    let container = Wrapped paperStyle
    InternalNode (container, [innerGift])

let putInBox innerGift =
    let container = Boxed
    InternalNode (container, [innerGift])

let withCard message innerGift =
    let container = WithACard message
    InternalNode (container, [innerGift])

let putTwoThingsInBox innerGift innerGift2 =
    let container = Boxed
    InternalNode (container, [innerGift;innerGift2])
```

我们可以创建一些示例数据：

```F#
let wolfHall = {title="Wolf Hall"; price=20m}
let yummyChoc = {chocType=SeventyPercent; price=5m}

let birthdayPresent =
    wolfHall
    |> fromBook
    |> wrapInPaper HappyBirthday
    |> withCard "Happy Birthday"

let christmasPresent =
    yummyChoc
    |> fromChoc
    |> putInBox
    |> wrapInPaper HappyHolidays

let twoBirthdayPresents =
    let thing1 = wolfHall |> fromBook
    let thing2 = yummyChoc |> fromChoc
    putTwoThingsInBox thing1 thing2
    |> wrapInPaper HappyBirthday

let twoWrappedPresentsInBox =
    let thing1 = wolfHall |> fromBook |> wrapInPaper HappyHolidays
    let thing2 = yummyChoc |> fromChoc  |> wrapInPaper HappyBirthday
    putTwoThingsInBox thing1 thing2
```

像 `description` 这样的函数现在需要处理一系列内部文本，而不是一个。我们只需使用分隔符 `&` 将字符串连接在一起：

```F#
let description gift =

    let fLeaf leafData =
        match leafData with
        | Book book ->
            sprintf "'%s'" book.title
        | Chocolate choc ->
            sprintf "%A chocolate" choc.chocType

    let fNode nodeData innerTexts =
        let innerText = String.concat " & " innerTexts
        match nodeData with
        | Wrapped style ->
            sprintf "%s wrapped in %A paper" innerText style
        | Boxed ->
            sprintf "%s in a box" innerText
        | WithACard message ->
            sprintf "%s with a card saying '%s'" innerText message

    // main call
    Tree.cata fLeaf fNode gift
```

最后，我们可以检查函数是否仍然像以前一样工作，以及是否正确处理了多个项目：

```F#
birthdayPresent |> description
// "'Wolf Hall' wrapped in HappyBirthday paper with a card saying 'Happy Birthday'"

christmasPresent |> description
// "SeventyPercent chocolate in a box wrapped in HappyHolidays paper"

twoBirthdayPresents |> description
// "'Wolf Hall' & SeventyPercent chocolate in a box
//   wrapped in HappyBirthday paper"

twoWrappedPresentsInBox |> description
// "'Wolf Hall' wrapped in HappyHolidays paper
//   & SeventyPercent chocolate wrapped in HappyBirthday paper
//   in a box"
```

### 第一步：定义 `GiftDto`

我们的 `Gift` 类型由许多可区分联合组成。根据我的经验，这些不能很好地序列化。事实上，大多数复杂类型都不能很好地序列化！

所以我喜欢做的是定义 [DTO](https://en.wikipedia.org/wiki/Data_transfer_object) 类型，这些类型被明确地设计为可以很好地序列化。在实践中，这意味着 DTO 类型受到如下约束：

- 只应使用记录类型。
- 记录字段应仅包含 `int`、`string` 和 `bool` 等原始值。

通过这样做，我们还获得了其他一些优势：

**我们可以控制序列化输出**。大多数序列化器对这类数据类型的处理方式相同，而不同库对诸如联合之类的“奇怪”事物的解释可能不同。

**我们对错误处理有更好的控制**。在处理序列化数据时，我的首要规则是“不信任任何人”。数据结构正确但对域无效是很常见的：假设非空字符串为空，字符串太长，整数超出正确的界限，等等。

通过使用 DTO，我们可以确保反序列化步骤本身会起作用。然后，当我们将 DTO 转换为域类型时，我们可以进行适当的验证。

那么，让我们为域外定义一些 DTO 类型。每个 DTO 类型都对应一个域类型，所以让我们从 `GiftContents` 开始。我们将定义一个名为 `GiftContentsDto` 的相应 DTO 类型，如下所示：

```F#
[<CLIMutableAttribute>]
type GiftContentsDto = {
    discriminator : string // "Book" or "Chocolate"
    // for "Book" case only
    bookTitle: string
    // for "Chocolate" case only
    chocolateType : string // one of "Dark" "Milk" "SeventyPercent"
    // for all cases
    price: decimal
    }
```

显然，这与最初的 `GiftContent` 完全不同，所以让我们来看看它们的区别：

- 首先，它具有 `CLIMutableAttribute`，它允许反序列化器使用反射来构造它们。
- 其次，它有一个 `discriminator`，指示正在使用原始联合类型的哪种情况。显然，这个字符串可以设置为任何值，所以当从 DTO 转换回域类型时，我们必须仔细检查！
- 接下来是一系列字段，每个字段对应需要存储的每个可能的数据项。例如，在 `Book` 案例中，我们需要一个 `bookTitle`，而在 `Chocolate` 案例中，需要巧克力类型。最后是两种类型的 `price` 字段。请注意，巧克力类型也存储为字符串，因此当我们从 DTO 转换为域时也需要特殊处理。

`GiftDecorationDto` 类型以相同的方式创建，使用鉴别器和字符串，而不是联合。

```F#
[<CLIMutableAttribute>]
type GiftDecorationDto = {
    discriminator: string // "Wrapped" or "Boxed" or "WithACard"
    // for "Wrapped" case only
    wrappingPaperStyle: string  // "HappyBirthday" or "HappyHolidays" or "SolidColor"
    // for "WithACard" case only
    message: string
    }
```

最后，我们可以将 `GiftDto` 类型定义为由两种 DTO 类型组成的树：

```F#
type GiftDto = Tree<GiftContentsDto,GiftDecorationDto>
```

### 第二步：转换 `Gift` 为 `GiftDto`

现在我们有了这个 DTO 类型，我们所需要做的就是使用 `Tree.map` 将礼物转换为 `GiftDto`。为了做到这一点，我们需要创建两个函数：一个将 `GiftContents` 转换为 `GiftContentsDto`，另一个将 `GiftDecoration` 转换为 `GiftDecorationDto`。

这是 `giftToDto` 的完整代码，应该是不言自明的：

```F#
let giftToDto (gift:Gift) :GiftDto =

    let fLeaf leafData :GiftContentsDto =
        match leafData with
        | Book book ->
            {discriminator= "Book"; bookTitle=book.title; chocolateType=null; price=book.price}
        | Chocolate choc ->
            let chocolateType = sprintf "%A" choc.chocType
            {discriminator= "Chocolate"; bookTitle=null; chocolateType=chocolateType; price=choc.price}

    let fNode nodeData :GiftDecorationDto =
        match nodeData with
        | Wrapped style ->
            let wrappingPaperStyle = sprintf "%A" style
            {discriminator= "Wrapped"; wrappingPaperStyle=wrappingPaperStyle; message=null}
        | Boxed ->
            {discriminator= "Boxed"; wrappingPaperStyle=null; message=null}
        | WithACard message ->
            {discriminator= "WithACard"; wrappingPaperStyle=null; message=message}

    // main call
    Tree.map fLeaf fNode gift
```

您可以看到，案例（`Book`、`Chocolate` 等）被转换为 `discriminator` 字符串，`chocolateType` 也被转换为字符串，如上所述。

### 第三步：定义 `TreeDto`

我上面说过，一个好的 DTO 应该是记录型的。我们已经转换了树的节点，但树本身是一个联合类型！我们还需要将 `Tree` 类型转换为 `TreeDto` 类型。

我们如何做到这一点？与礼品 DTO 类型一样，我们将创建一个记录类型，其中包含这两种情况的所有数据。我们可以像以前一样使用鉴别器字段，但这次，由于只有两个选择，叶子和内部节点，我将在反序列化时检查值是否为 null。如果叶子值不为 null，则记录必须表示 `LeafNode` 案例，否则记录必须表示 `InternalNode` 案例。

以下是数据类型的定义：

```F#
/// A DTO that represents a Tree
/// The Leaf/Node choice is turned into a record
[<CLIMutableAttribute>]
type TreeDto<'LeafData,'NodeData> = {
    leafData : 'LeafData
    nodeData : 'NodeData
    subtrees : TreeDto<'LeafData,'NodeData>[] }
```

与以前一样，该类型具有 `CLIMutableAttribute`。和以前一样，该类型有字段来存储所有可能选择的数据。`subtrees` 被存储为数组而不是 seq——这让序列化器很高兴！

为了创建 `TreeDto`，我们使用我们的老朋友 `cata` 从常规 `Tree` 中组装记录。

```F#
/// Transform a Tree into a TreeDto
let treeToDto tree : TreeDto<'LeafData,'NodeData> =

    let fLeaf leafData  =
        let nodeData = Unchecked.defaultof<'NodeData>
        let subtrees = [||]
        {leafData=leafData; nodeData=nodeData; subtrees=subtrees}

    let fNode nodeData subtrees =
        let leafData = Unchecked.defaultof<'NodeData>
        let subtrees = subtrees |> Seq.toArray
        {leafData=leafData; nodeData=nodeData; subtrees=subtrees}

    // recurse to build up the TreeDto
    Tree.cata fLeaf fNode tree
```

请注意，在 F# 中，记录不可为 null，因此我使用 `Unchecked.defaultOf<'NodeData>` 而不是 `null` 来表示缺少数据。

还要注意，我假设 `LeafData` 或 `NodeData` 是引用类型。如果 `LeafData` 或 `NodeData` 是 `int` 或 `bool` 这样的值类型，那么这种方法就会失败，因为你无法分辨默认值和缺失值之间的区别。在这种情况下，我会像以前一样切换到鉴别器字段。

或者，我可以使用 `IDictionary`。这将不太方便反序列化，但可以避免空检查的需要。

### 第四步：序列化 TreeDto

最后，我们可以使用 JSON 序列化器对 `TreeDto` 进行序列化。

对于这个例子，我使用了内置的 `DataContractJsonSerializer`，这样我就不需要依赖于 NuGet 包。还有其他 JSON 序列化器可能更适合严肃的项目。

```F#
#r "System.Runtime.Serialization.dll"

open System.Runtime.Serialization
open System.Runtime.Serialization.Json

let toJson (o:'a) =
    let serializer = new DataContractJsonSerializer(typeof<'a>)
    let encoding = System.Text.UTF8Encoding()
    use stream = new System.IO.MemoryStream()
    serializer.WriteObject(stream,o)
    stream.Close()
    encoding.GetString(stream.ToArray())
```

### 第五步：组装管道

所以，把它们放在一起，我们有以下管道：

- 使用 `GiftToDto` 将 `Gift` 转换为 `GiftDto`，
  也就是说，使用 `Tree.map` 从 `Tree<GiftContents, GiftDecoration>` 转到 `Tree<GiftsContentsDto, GiftecorationDto>`
- 使用 `treeToDto` 将 `Tree` 转换为 `TreeDto`，
  也就是说，使用 `Tree.cata` 从 `Tree<GiftContentsDo, GiftDecorationDto>` 转到 `TreeDto<GiftContentDto, GiftecorationDto>`
- 将 `TreeDto` 序列化为 JSON 字符串

以下是一些示例代码：

```F#
let goodJson = christmasPresent |> giftToDto |> treeToDto |> toJson
```

下面是 JSON 输出的样子：

```json
{
  "leafData@": null,
  "nodeData@": {
    "discriminator@": "Wrapped",
    "message@": null,
    "wrappingPaperStyle@": "HappyHolidays"
  },
  "subtrees@": [
    {
      "leafData@": null,
      "nodeData@": {
        "discriminator@": "Boxed",
        "message@": null,
        "wrappingPaperStyle@": null
      },
      "subtrees@": [
        {
          "leafData@": {
            "bookTitle@": null,
            "chocolateType@": "SeventyPercent",
            "discriminator@": "Chocolate",
            "price@": 5
          },
          "nodeData@": null,
          "subtrees@": []
        }
      ]
    }
  ]
}
```

字段名上丑陋的 `@` 符号是序列化 F# 记录类型的产物。这可以通过一点努力来纠正，但我现在不会费心！

*这个例子的源代码可以在[这里 gist](https://gist.github.com/swlaschin/bbe70c768215b209c06c) 找到*

## 示例：从 JSON 反序列化树

既然我们已经创建了 JSON，那么换一种方式将其加载到 `Gift` 中怎么样？

简单！我们只需要反转管道：

- 将 JSON 字符串反序列化为 `TreeDto`。
- 使用 `dtoToTree` 将 `TreeDto` 转换为 `Tree`，
  也就是说，从 `TreeDto<GiftContentsDo, GiftDecorationDto>` 再到 `Tree<GiftContentDto, GiftecorationDto>`。我们不能使用 `cata`——我们必须创建一个小的递归循环。
- 使用 `dtoToGift` 将 `GiftDto` 转换为 `Gift`，
  也就是说，使用 `Tree.map` 从 `Tree<GiftContentsDto, GiftDecorationDto>` 转到 `Tree<GiftsContents, GiftDecoration>`。

### 第一步：反序列化 `TreeDto`

我们可以使用 JSON 序列化器对 `TreeDto` 进行反序列化。

```F#
let fromJson<'a> str =
    let serializer = new DataContractJsonSerializer(typeof<'a>)
    let encoding = System.Text.UTF8Encoding()
    use stream = new System.IO.MemoryStream(encoding.GetBytes(s=str))
    let obj = serializer.ReadObject(stream)
    obj :?> 'a
```

如果反序列化失败了怎么办？现在，我们将忽略任何错误处理，让异常传播。

### 第二步：转换 `TreeDto` 为 `Tree`

为了将 `TreeDto` 转换为 `Tree`，我们递归地循环记录及其子树，根据相应字段是否为 null，将每个子树转换为 `InternalNode` 或 `LeafNode`。

```F#
let rec dtoToTree (treeDto:TreeDto<'Leaf,'Node>) :Tree<'Leaf,'Node> =
    let nullLeaf = Unchecked.defaultof<'Leaf>
    let nullNode = Unchecked.defaultof<'Node>

    // check if there is nodeData present
    if treeDto.nodeData <> nullNode then
        if treeDto.subtrees = null then
            failwith "subtrees must not be null if node data present"
        else
            let subtrees = treeDto.subtrees |> Array.map dtoToTree
            InternalNode (treeDto.nodeData,subtrees)
    // check if there is leafData present
    elif treeDto.leafData <> nullLeaf then
        LeafNode (treeDto.leafData)
    // if both missing then fail
    else
        failwith "expecting leaf or node data"
```

如您所见，许多事情都可能出错：

- 如果 `leafData` 和 `nodeData` 字段都为空怎么办？
- 如果 `nodeData` 字段不为空，但 `subtrees` 字段为空，该怎么办？

同样，我们将忽略任何错误处理，只是抛出异常（目前）。

*问题：我们能否为 `TreeDto` 创建一个 `cata`，使这段代码更简单？值得吗？*

### 第三步：转换 `GiftDto` 为 `Gift`

现在我们有了一个合适的树，我们可以再次使用 `Tree.map` 将每个叶子和内部节点从 DTO 转换为合适的域类型。

这意味着我们需要将 `GiftContentsDto` 映射为 `GiftContents`，将 `GiftDecorationDto` 映射为 `Gift Decoration` 的函数。

这是完整的代码——它比往另一个方向走要复杂得多！

代码可以按如下方式分组：

- 辅助方法（如 `strToChocolateType`）将字符串转换为正确的域类型，并在输入无效时抛出异常。
- 将整个 DTO 转换为 case 的 case 转换器方法（如 `bookFromDto`）。
- 最后是 `dtoToGift` 函数本身。它查看 `discriminator` 字段，看看要调用哪个 case 转换器，如果鉴别器值未被识别，则抛出异常。

```F#
let strToBookTitle str =
    match str with
    | null -> failwith "BookTitle must not be null"
    | _ -> str

let strToChocolateType str =
    match str with
    | "Dark" -> Dark
    | "Milk" -> Milk
    | "SeventyPercent" -> SeventyPercent
    | _ -> failwithf "ChocolateType %s not recognized" str

let strToWrappingPaperStyle str =
    match str with
    | "HappyBirthday" -> HappyBirthday
    | "HappyHolidays" -> HappyHolidays
    | "SolidColor" -> SolidColor
    | _ -> failwithf "WrappingPaperStyle %s not recognized" str

let strToCardMessage str =
    match str with
    | null -> failwith "CardMessage must not be null"
    | _ -> str

let bookFromDto (dto:GiftContentsDto) =
    let bookTitle = strToBookTitle dto.bookTitle
    Book {title=bookTitle; price=dto.price}

let chocolateFromDto (dto:GiftContentsDto) =
    let chocType = strToChocolateType dto.chocolateType
    Chocolate {chocType=chocType; price=dto.price}

let wrappedFromDto (dto:GiftDecorationDto) =
    let wrappingPaperStyle = strToWrappingPaperStyle dto.wrappingPaperStyle
    Wrapped wrappingPaperStyle

let boxedFromDto (dto:GiftDecorationDto) =
    Boxed

let withACardFromDto (dto:GiftDecorationDto) =
    let message = strToCardMessage dto.message
    WithACard message

/// Transform a GiftDto to a Gift
let dtoToGift (giftDto:GiftDto) :Gift=

    let fLeaf (leafDto:GiftContentsDto) =
        match leafDto.discriminator with
        | "Book" -> bookFromDto leafDto
        | "Chocolate" -> chocolateFromDto leafDto
        | _ -> failwithf "Unknown leaf discriminator '%s'" leafDto.discriminator

    let fNode (nodeDto:GiftDecorationDto)  =
        match nodeDto.discriminator with
        | "Wrapped" -> wrappedFromDto nodeDto
        | "Boxed" -> boxedFromDto nodeDto
        | "WithACard" -> withACardFromDto nodeDto
        | _ -> failwithf "Unknown node discriminator '%s'" nodeDto.discriminator

    // map the tree
    Tree.map fLeaf fNode giftDto
```

### 第四步：组装管道

我们现在可以组装一个接受 JSON 字符串并创建 `Gift` 的管道。

```F#
let goodGift = goodJson |> fromJson |> dtoToTree |> dtoToGift

// check that the description is unchanged
goodGift |> description
// "SeventyPercent chocolate in a box wrapped in HappyHolidays paper"
```

这工作得很好，但错误处理很糟糕！

看看如果我们稍微破坏 JSON 会发生什么：

```F#
let badJson1 = goodJson.Replace("leafData","leafDataXX")

let badJson1_result = badJson1 |> fromJson |> dtoToTree |> dtoToGift
// Exception "The data contract type 'TreeDto' cannot be deserialized because the required data member 'leafData@' was not found."
```

我们得到了一个丑陋的例外。

或者，如果鉴别器是错误的呢？

```F#
let badJson2 = goodJson.Replace("Wrapped","Wrapped2")

let badJson2_result = badJson2 |> fromJson |> dtoToTree |> dtoToGift
// Exception "Unknown node discriminator 'Wrapped2'"
```

还是 WrappingPaperStyle DU 的某个值？

```F#
let badJson3 = goodJson.Replace("HappyHolidays","HappyHolidays2")
let badJson3_result = badJson3 |> fromJson |> dtoToTree |> dtoToGift
// Exception "WrappingPaperStyle HappyHolidays2 not recognized"
```

我们会遇到很多异常，作为函数式程序员，我们应该尽可能地删除它们。

我们将在下一节讨论如何做到这一点。

*这个例子的源代码就在[这里 gist](https://gist.github.com/swlaschin/bbe70c768215b209c06c)。*

## 示例：从 JSON 反序列化树 - 带错误处理

为了解决错误处理问题，我们将使用下面显示的 `Result` 类型：

```F#
type Result<'a> =
    | Success of 'a
    | Failure of string list
```

我不打算解释它在这里是如何工作的。如果你不熟悉这种方法，请[阅读我的帖子](https://fsharpforfunandprofit.com/posts/recipe-part2/)或[观看我关于功能错误处理的演讲](https://fsharpforfunandprofit.com/rop/)。

让我们重新审视上一节中的所有步骤，并使用 `Result` 而不是抛出异常。

### 第一步：反序列化 TreeDto

当我们使用 JSON 序列化器反序列化 `TreeDto` 时，我们将捕获异常并将其转换为 `Result`。

```F#
let fromJson<'a> str =
    try
        let serializer = new DataContractJsonSerializer(typeof<'a>)
        let encoding = System.Text.UTF8Encoding()
        use stream = new System.IO.MemoryStream(encoding.GetBytes(s=str))
        let obj = serializer.ReadObject(stream)
        obj :?> 'a
        |> Result.retn
    with
    | ex ->
        Result.failWithMsg ex.Message
```

fromJson的签名现在是字符串->结果<‘a>。

### 第二步：转换 `TreeDto` 为 `Tree`

如前所述，我们通过递归循环记录及其子树，将 `TreeDto` 转换为 `Tree`，将每个子树转换为 `InternalNode` 或 `LeafNode`。不过，这一次，我们使用 `Result` 来处理任何错误。

```F#
let rec dtoToTreeOfResults (treeDto:TreeDto<'Leaf,'Node>) :Tree<Result<'Leaf>,Result<'Node>> =
    let nullLeaf = Unchecked.defaultof<'Leaf>
    let nullNode = Unchecked.defaultof<'Node>

    // check if there is nodeData present
    if treeDto.nodeData <> nullNode then
        if treeDto.subtrees = null then
            LeafNode <| Result.failWithMsg "subtrees must not be null if node data present"
        else
            let subtrees = treeDto.subtrees |> Array.map dtoToTreeOfResults
            InternalNode (Result.retn treeDto.nodeData,subtrees)
    // check if there is leafData present
    elif treeDto.leafData <> nullLeaf then
        LeafNode <| Result.retn (treeDto.leafData)
    // if both missing then fail
    else
        LeafNode <| Result.failWithMsg "expecting leaf or node data"

// val dtoToTreeOfResults :
//   treeDto:TreeDto<'Leaf,'Node> -> Tree<Result<'Leaf>,Result<'Node>>
```

但是，我们现在有一棵 `Tree`，其中每个内部节点和叶子都被包裹在一个 `Result` 中。这是一棵 `Results` 树！真正丑陋的签名是： `Tree<Result<'Leaf>,Result<'Node>>`。

但这种类型目前是无用的——我们真正想要的是将所有错误合并在一起，并返回一个包含 `Tree` 的 `Result`。

我们如何将结果们的树转换为树的结果？

答案是使用一个 `sequence` 函数来“交换”这两种类型。你可以在我关于[提高世界的系列文章](https://fsharpforfunandprofit.com/posts/elevated-world-4/#sequence)中阅读更多关于 `sequence` 的内容。

*请注意，我们也可以使用稍微复杂一些的 *`traverse`* 变体将 *`map`* 和 `sequence` 组合成一个步骤，但为了本演示的目的，如果这些步骤是分开的，则更容易理解。*

我们需要为 Tree/Result 组合创建自己的 `sequence` 函数。幸运的是，序列函数的创建是一个机械过程：

- 对于较低的类型（`Result`），我们需要定义 `apply` 和 `return` 函数。请参阅此处了解申请的更多详细信息。
- 对于更高的类型（`Tree`），我们需要有一个 `cata` 函数，我们做到了。
- 在分解形态中，更高类型的每个构造函数（在这种情况下为 `LeafNode` 和 `InternalNode`）都被“提升”到 `Result` 类型的等价物（例如 `retn LeafNode <*> data`）所替换

这是实际的代码——如果你不能立即理解，不要担心。幸运的是，我们只需要为每种类型组合编写一次，所以对于未来的任何一种 Tree/Result 组合，我们都已经设置好了！

```F#
/// Convert a tree of Results into a Result of tree
let sequenceTreeOfResult tree =
    // from the lower level
    let (<*>) = Result.apply
    let retn = Result.retn

    // from the traversable level
    let fLeaf data =
        retn LeafNode <*> data

    let fNode data subitems =
        let makeNode data items = InternalNode(data,items)
        let subItems = Result.sequenceSeq subitems
        retn makeNode <*> data <*> subItems

    // do the traverse
    Tree.cata fLeaf fNode tree

// val sequenceTreeOfResult :
//    tree:Tree<Result<'a>,Result<'b>> -> Result<Tree<'a,'b>>
```

最后，实际的 `dtoToTree` 函数很简单——只需通过 `dtoToTreeOfResults` 发送 `treeDto`，然后使用 `sequenceTreeOfResult` 将最终结果转换为 `Result<Tree<..>>`，这正是我们所需要的。

```F#
let dtoToTree treeDto =
    treeDto |> dtoToTreeOfResults |> sequenceTreeOfResult

// val dtoToTree : treeDto:TreeDto<'a,'b> -> Result<Tree<'a,'b>>
```

### 第三步：转换 `GiftDto` 为 `Gift`

同样，我们可以使用 `Tree.map` 将每个叶子和内部节点从 DTO 转换为适当的域类型。

但是我们的函数会处理错误，因此它们需要将 `GiftContentsDto` 映射到 `Result<GiftContents>`，并将 `GiftDecorationDto` 映射到 `Result<GiftDecoration>`。这将再次生成结果树，因此我们必须再次使用 `sequenceTreeOfResult` 将其恢复到正确的 `Result<Tree<..>>` 形状。

让我们从将字符串转换为正确域类型的辅助方法（如 `strToChocolateType`）开始。这一次，它们返回一个 `Result`，而不是抛出一个异常。

```F#
let strToBookTitle str =
    match str with
    | null -> Result.failWithMsg "BookTitle must not be null"
    | _ -> Result.retn str

let strToChocolateType str =
    match str with
    | "Dark" -> Result.retn Dark
    | "Milk" -> Result.retn Milk
    | "SeventyPercent" -> Result.retn SeventyPercent
    | _ -> Result.failWithMsg (sprintf "ChocolateType %s not recognized" str)

let strToWrappingPaperStyle str =
    match str with
    | "HappyBirthday" -> Result.retn HappyBirthday
    | "HappyHolidays" -> Result.retn HappyHolidays
    | "SolidColor" -> Result.retn SolidColor
    | _ -> Result.failWithMsg (sprintf "WrappingPaperStyle %s not recognized" str)

let strToCardMessage str =
    match str with
    | null -> Result.failWithMsg "CardMessage must not be null"
    | _ -> Result.retn str
```

case 转换器方法必须从 `Result`s 而不是正常值的参数构建 `Book` 或 `Chocolate`。这就是像 `Result.lift2` 这样的提升功能可以提供帮助的地方。有关其工作原理的详细信息，请参阅这篇[关于提升的文章](https://fsharpforfunandprofit.com/posts/elevated-world/#lift)和[这篇关于应用函子验证的文章](https://fsharpforfunandprofit.com/posts/elevated-world-3/#validation)。

```F#
let bookFromDto (dto:GiftContentsDto) =
    let book bookTitle price =
        Book {title=bookTitle; price=price}

    let bookTitle = strToBookTitle dto.bookTitle
    let price = Result.retn dto.price
    Result.lift2 book bookTitle price

let chocolateFromDto (dto:GiftContentsDto) =
    let choc chocType price =
        Chocolate {chocType=chocType; price=price}

    let chocType = strToChocolateType dto.chocolateType
    let price = Result.retn dto.price
    Result.lift2 choc chocType price

let wrappedFromDto (dto:GiftDecorationDto) =
    let wrappingPaperStyle = strToWrappingPaperStyle dto.wrappingPaperStyle
    Result.map Wrapped wrappingPaperStyle

let boxedFromDto (dto:GiftDecorationDto) =
    Result.retn Boxed

let withACardFromDto (dto:GiftDecorationDto) =
    let message = strToCardMessage dto.message
    Result.map WithACard message
```

最后，如果 `discriminator` 无效，则更改 `dtoToGift` 函数本身以返回 `Result`。

如前所述，此映射创建了一个结果树，因此我们通过 `sequenceTreeOfResult` 管道传输 `Tree.map` 的输出…

```F#
`Tree.map fLeaf fNode giftDto |> sequenceTreeOfResult`
```

…返回树的结果。

以下是 `dtoToGift` 的完整代码：

```F#
open TreeDto_WithErrorHandling

/// Transform a GiftDto to a Result<Gift>
let dtoToGift (giftDto:GiftDto) :Result<Gift>=

    let fLeaf (leafDto:GiftContentsDto) =
        match leafDto.discriminator with
        | "Book" -> bookFromDto leafDto
        | "Chocolate" -> chocolateFromDto leafDto
        | _ -> Result.failWithMsg (sprintf "Unknown leaf discriminator '%s'" leafDto.discriminator)

    let fNode (nodeDto:GiftDecorationDto)  =
        match nodeDto.discriminator with
        | "Wrapped" -> wrappedFromDto nodeDto
        | "Boxed" -> boxedFromDto nodeDto
        | "WithACard" -> withACardFromDto nodeDto
        | _ -> Result.failWithMsg (sprintf "Unknown node discriminator '%s'" nodeDto.discriminator)

    // map the tree
    Tree.map fLeaf fNode giftDto |> sequenceTreeOfResult
```

`dtoToGift` 的类型签名已更改——它现在返回一个 `Result<Gift>`，而不仅仅是一个 `Gift`。

```F#
// val dtoToGift : GiftDto -> Result<GiftUsingTree.Gift>
```

### 第四步：组装管道

我们现在可以重新组装接受 JSON 字符串并创建 `Gift` 的管道。

但是，需要更改才能使用新的错误处理代码：

- `fromJson` 函数返回一个 `Result<TreeDto>`，但管道中的下一个函数（`dtoToTree`）需要一个常规的 `TreeDto`作为输入。
- 类似地，`dtoTree` 返回一个 `Result<Tree>`，但管道中的下一个函数（`dtoToGift`）需要一个常规 `Tree` 作为输入。

在这两种情况下，`Result.bind` 都可以用来解决输出/输入不匹配的问题。请参阅[此处有关 bind 的更详细讨论](https://fsharpforfunandprofit.com/posts/elevated-world-2/#bind)。

好的，让我们尝试对之前创建的 `goodJson` 字符串进行反序列化。

```F#
let goodGift = goodJson |> fromJson |> Result.bind dtoToTree |> Result.bind dtoToGift

// check that the description is unchanged
goodGift |> description
// Success "SeventyPercent chocolate in a box wrapped in HappyHolidays paper"
```

那很好。

让我们看看错误处理现在是否有所改善。我们将再次损坏 JSON：

```F#
let badJson1 = goodJson.Replace("leafData","leafDataXX")

let badJson1_result = badJson1 |> fromJson |> Result.bind dtoToTree |> Result.bind dtoToGift
// Failure ["The data contract type 'TreeDto' cannot be deserialized because the required data member 'leafData@' was not found."]
```

太棒了我们得到了一个很好的 `Failure` 案例。

或者，如果鉴别者是错误的呢？

```F#
let badJson2 = goodJson.Replace("Wrapped","Wrapped2")
let badJson2_result = badJson2 |> fromJson |> Result.bind dtoToTree |> Result.bind dtoToGift
// Failure ["Unknown node discriminator 'Wrapped2'"]
```

还是 WrappingPaperStyle DU 的某个值？

```F#
let badJson3 = goodJson.Replace("HappyHolidays","HappyHolidays2")
let badJson3_result = badJson3 |> fromJson |> Result.bind dtoToTree |> Result.bind dtoToGift
// Failure ["WrappingPaperStyle HappyHolidays2 not recognized"]
```

再次，很好的 `Failure` 案例。

非常好的是（这是异常处理方法无法提供的），如果有多个错误，可以聚合各种错误，这样我们就可以得到所有错误的列表，而不是一次只有一个错误。

让我们通过在 JSON 字符串中引入两个错误来看看这一点：

```F#
// create two errors
let badJson4 = goodJson.Replace("HappyHolidays","HappyHolidays2")
                       .Replace("SeventyPercent","SeventyPercent2")
let badJson4_result = badJson4 |> fromJson |> Result.bind dtoToTree |> Result.bind dtoToGift
// Failure ["WrappingPaperStyle HappyHolidays2 not recognized";
//          "ChocolateType SeventyPercent2 not recognized"]
```

总的来说，我认为这是一个成功！

*这个例子的源代码就在[这里 gist](https://gist.github.com/swlaschin/2b06fe266e3299a656c1)。*

## 摘要

在本系列中，我们已经看到了如何定义分解形态、折叠，特别是在这篇文章中，我们看到了如何使用它们来解决现实世界的问题。我希望这些帖子是有用的，并为您提供了一些可以应用于自己代码的提示和见解。

这个系列比我预想的要长得多，所以谢谢你把它看完！干杯！