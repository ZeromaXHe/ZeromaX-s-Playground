# [返回主 Markdown](./FSharpForFunAndProfit翻译.md)



# 1 递归类型介绍

*Part of the "Recursive types and folds" series (*[link](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds/#series-toc)*)*

不要害怕变形。。。
2015年8月20日这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/recursive-types-and-folds/

在本系列中，我们将介绍递归类型以及如何使用它们，在这一过程中，我们还将介绍变形（catamorphisms）、尾部递归、左右折叠之间的区别等等。

## 系列内容

以下是本系列的内容：

- **第 1 部分：递归类型和变形（catamorphisms）介绍**
  - 一个简单的递归类型
  - 对所有事物进行参数化
  - 介绍变形
  - 变形的好处
  - 创建变形的规则
- **第 2 部分：变形示例**
  - 变形示例：文件系统域
  - 变形示例：产品域
- **第 3 部分：介绍折叠**
  - 我们的变形实现中的一个缺陷
  - 介绍 `fold`
  - 折叠问题
  - 将函数用作累加器
  - 介绍 `foldback`
  - 创建折叠的规则
- **第 4 部分：了解折叠**
  - 迭代与递归
  - 折叠示例：文件系统域
  - 关于“折叠”的常见问题
- **第 5 部分：泛型递归类型**
  - LinkedList：一种通用的递归类型
  - 使礼品领域泛型
  - 定义泛型容器类型
  - 实现礼物领域的第三种方法
  - 抽象还是具体？比较三种设计
- **第 6 部分：现实世界中的树**
  - 定义通用树类型
  - 现实世界中的树类型
  - 映射树类型
  - 示例：创建目录列表
  - 示例：并行 grep
  - 示例：将文件系统存储在数据库中
  - 示例：将树序列化为 JSON
  - 示例：从 JSON 反序列化树
  - 示例：从 JSON 反序列化树 - 带错误处理

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

## 介绍变形

我们上面写的 `cataGift` 函数被称为“变形（catamorphism）”，来自希腊语成分“down+shape”。在正常使用中，变形是一种根据递归类型的结构将其“折叠”为新值的函数。事实上，你可以把变形看作是一种“访问者模式”。

变形是一个非常强大的概念，因为它是你可以为这样的结构定义的最基本的函数。任何其他功能都可以根据它来定义。

也就是说，如果我们想创建一个签名为 `Gift -> string` 或 `Gift -> int` 的函数，我们可以通过在 `Gift` 结构中为每种情况指定一个函数来使用变形来创建它。

我们在上面看到了如何使用变形将 `totalCost` 重写为 `totalCostUsingCata`，稍后我们将看到许多其他示例。

### 变形和折叠

变形通常被称为“折叠”，但折叠的种类不止一种，所以我倾向于用“变形”来指代概念，用“折叠”来指一种特定的实现。

我将在后续的文章中详细讨论各种折叠，所以在这篇文章的其余部分，我将只使用“变形”。

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

## 变形的好处

变形背后有很多理论，但在实践中有什么好处呢？

为什么要费心创建像 `cataGift` 这样的特殊功能？为什么不把原来的功能放在一边呢？

原因有很多，包括：

- **重新使用**。稍后，我们将创建相当复杂的变形。如果你只需要把逻辑弄对一次就好了。
- **封装**。通过仅公开函数，可以隐藏数据类型的内部结构。
- **灵活性**。函数比模式匹配更灵活——它们可以组合、部分应用等。
- **映射**。有了变形，你可以很容易地创建将各种情况映射到新结构的函数。

诚然，这些好处中的大多数也适用于非递归类型，但递归类型往往更复杂，因此封装、灵活性等的好处相应更强。

在接下来的部分中，我们将更详细地介绍最后三点。

### 使用函数参数隐藏内部结构

第一个好处是，变形抽象出了内部设计。通过使用函数，客户端代码在一定程度上与内部结构隔离开来。同样，您可以将访问者模式视为面向对象世界中的类似模式。

例如，如果所有客户端都使用了变形函数而不是模式匹配，我可以安全地重命名案例，甚至可以稍微小心地添加或删除案例。

这里有一个例子。假设我之前为 `Gift` 设计了一个没有 `WithACard` 案例的设计。我称之为版本 1：

```F#
type Gift =
    | Book of Book
    | Chocolate of Chocolate
    | Wrapped of Gift * WrappingPaperStyle
    | Boxed of Gift
```

假设我们为该结构构建并发布了一个变形函数：

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

变形使用函数参数，如上所述，由于组合、部分应用等工具，函数比模式匹配更灵活。

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

### 利用变形进行映射

我上面说过，变形是一个将递归类型“折叠”为新值的函数。例如，在 `totalCost` 中，递归礼物结构被分解为单个成本值。

但是“单个值”不仅仅是一个原始值，它也可以是一个复杂的结构，比如另一个递归结构。

事实上，变形非常适合将一种结构映射到另一种结构上，特别是当它们非常相似的时候。

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

这就引出了另一种思考变形的方式：

- 变形是递归类型的一个函数，当你传入该类型的 case 构造函数时，你会得到一个“clone”函数。

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

## 创建变形的规则

我们在上面看到，创建变形是一个机械过程：

- 创建一个函数参数来处理结构中的每种情况。
- 对于非递归情况，将与该情况相关的所有数据传递给函数参数。
- 对于递归情况，执行两个步骤：
  - 首先，对嵌套值递归调用变形。
  - 然后将与该情况相关的所有数据传递给处理程序，但分解的结果将替换原始嵌套值。
- 现在让我们看看我们是否可以应用这些规则在其他域中创建变形。

## 摘要

我们在这篇文章中看到了如何定义递归类型，并介绍了变形（catamorphisms）。

在下一篇文章中，我们将使用这些规则为其他一些域创建变形。

到时候见！

*这篇文章的源代码可以在这里找到。*



# 2 变形示例

*Part of the "Recursive types and folds" series (*[link](https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-1b/#series-toc)*)*

将规则应用于其他域
2015年8月21日这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/recursive-types-and-folds-1b/

这篇文章是系列文章中的第二篇。

在上一篇文章中，我介绍了“变形（catamorphisms）”，这是一种为递归类型创建函数的方法，并列出了一些可用于机械实现它们的规则。在这篇文章中，我们将使用这些规则为其他一些域实现变形。

## 系列内容

以下是本系列的内容：

- **第 1 部分：递归类型和变形（catamorphisms）介绍**
  - 一个简单的递归类型
  - 对所有事物进行参数化
  - 介绍变形
  - 变形的好处
  - 创建变形的规则
- **第 2 部分：变形示例**
  - 变形示例：文件系统域
  - 变形示例：产品域
- **第 3 部分：介绍折叠**
  - 我们的变形实现中的一个缺陷
  - 介绍 `fold`
  - 折叠问题
  - 将函数用作累加器
  - 介绍 `foldback`
  - 创建折叠的规则
- **第 4 部分：了解折叠**
  - 迭代与递归
  - 折叠示例：文件系统域
  - 关于“折叠”的常见问题
- **第 5 部分：泛型递归类型**
  - LinkedList：一种通用的递归类型
  - 使礼品领域泛型
  - 定义泛型容器类型
  - 实现礼物领域的第三种方法
  - 抽象还是具体？比较三种设计
- **第 6 部分：现实世界中的树**
  - 定义通用树类型
  - 现实世界中的树类型
  - 映射树类型
  - 示例：创建目录列表
  - 示例：并行 grep
  - 示例：将文件系统存储在数据库中
  - 示例：将树序列化为 JSON
  - 示例：从 JSON 反序列化树
  - 示例：从 JSON 反序列化树 - 带错误处理

## 创建变形的规则

我们在上一篇文章中看到，创建变形是一个机械过程，规则是：

- 创建一个函数参数来处理结构中的每种情况。
- 对于非递归情况，将与该情况相关的所有数据传递给函数参数。
- 对于递归情况，执行两个步骤：
  - 首先，对嵌套值递归调用变形。
  - 然后将与该情况相关的所有数据传递给处理程序，但分解的结果将替换原始嵌套值。

现在让我们看看我们是否可以应用这些规则在其他域中创建变形。

## 变形示例：文件系统域

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

是时候创造变形了！

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

同样，设置起来有点棘手，但只不过是我们必须从头开始编写它而不使用任何变形。

## 变形示例：产品域

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

现在要设计变形，我们需要做的就是在所有构造函数中将 `Product` 类型替换为 `'r`。

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

同样，我本可以完全避免使用 `cataProduct`，并从头开始编写 `mostUsedVendor`。如果性能是一个问题，那么这可能是一种更好的方法，因为泛型变形会创建中间值（如 `qty * VendorScore option` 列表），这些值过于笼统，可能会造成浪费。

另一方面，通过使用变形，我可以只关注计数逻辑而忽略递归逻辑。

因此，与往常一样，您应该考虑重用与从头开始创建的利弊；一次性编写通用代码并以标准化的方式使用它的好处，与自定义代码的性能但额外的努力（和潜在的麻烦）相比。

## 摘要

我们在这篇文章中看到了如何定义递归类型，并介绍了变形。

我们还看到了变形的一些用法：

- 任何“折叠”递归类型的函数，如 `Gift->'r`，都可以根据该类型的变形来编写。
- 可以使用变形来隐藏类型的内部结构。
- 通过调整处理每种情况的函数，可以使用变形来创建从一种类型到另一种类型的映射。
- 通过传入类型的case构造函数，可以使用变形来创建原始值的克隆。

但在变形的土地上，一切都不是完美的。事实上，此页面上的所有变形实现都有一个潜在的严重缺陷。

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

在第一篇文章中，我介绍了“变形”，这是一种为递归类型创建函数的方法，在第二篇文章中我们创建了一些变形实现。

但在上一篇文章的末尾，我注意到到到目前为止，所有的变形实现都有一个潜在的严重缺陷。

在这篇文章中，我们将看看这个缺陷以及如何解决它，并在这个过程中看看折叠、尾部递归以及“左折叠”和“右折叠”之间的区别。