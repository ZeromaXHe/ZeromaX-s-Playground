# [返回主 Markdown](./FSharpForFunAndProfit翻译.md)



# 1 了解映射和应用

*Part of the "Map and Bind and Apply, Oh my!" series (*[link](https://fsharpforfunandprofit.com/posts/elevated-world/#series-toc)*)*

用于处理提升世界的工具集
2015年8月2日 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/elevated-world/

在本系列文章中，我将尝试描述处理泛型数据类型（如 `Option` 和 `List`）的一些核心函数。这是我关于函数式模式演讲的后续帖子。

是的，我知道我答应过不做这种事情，但对于这篇文章，我想我会采取与大多数人不同的方法。与其谈论类型类等抽象概念，我认为关注核心函数本身以及它们在实践中的使用方式可能是有用的。

换句话说，这是一种用于 `map`, `return`, `apply`, 和 `bind` 的“手册页”。

因此，每个函数都有一个部分，描述它们的名称（和常见别名）、常见运算符、类型签名，然后详细描述为什么需要它们以及如何使用它们，以及一些视觉效果（我总是觉得很有帮助）。

Haskellers 和范畴理论家现在可能想把目光移开！不会有数学，也会有很多挥手。我将避免使用行话和 Haskell 特有的概念，如类型类，并尽可能地关注大局。这里的概念应该适用于任何语言的任何类型的函数式编程。

我知道有些人可能不喜欢这种方法。那很好。网络上不乏更多的学术解释。从这个和这个开始。

最后，与本网站上的大多数帖子一样，我写这篇文章也是为了自己的利益，也是我自己学习过程的一部分。我根本不自称是专家，所以如果我犯了任何错误，请告诉我。

## 背景

首先，让我介绍一下背景和一些术语。

想象一下，我们可以在两个世界中编程：一个是“正常”的日常世界，另一个是我称之为“提升（elevated）世界”的世界（原因我稍后会解释）。

提升世界与正常世界非常相似。事实上，正常世界中的每一件事在提升世界中都有相应的东西。

例如，在正常世界中，我们有一组名为 `Int` 的值，在提升的世界中，有一组并行的值，比如 `E<Int>`。同样，在普通世界中，我们有一组名为 `String` 的值，在高级世界中，有一组并行的值，称为 `E<String>`。

![img](https://fsharpforfunandprofit.com/posts/elevated-world/vgfp_e_values.png)

此外，正如在正常世界中 `Int` 和 `String` 之间有函数一样，在提升世界中 `E<Int>`s 和 `E<String>`s之间也有函数。

![img](https://fsharpforfunandprofit.com/posts/elevated-world/vgfp_e_functions.png)

请注意，我故意使用“世界”而不是“类型”一词来强调世界中值之间的关系与底层数据类型同样重要。

### 究竟什么是提升（elevated）世界？

我无法准确定义什么是提升世界，因为有太多不同类型的提升世界，它们没有任何共同点。

其中一些表示数据结构（`Option<T>`），一些表示工作流（`State<T>`），其中一些表示信号（`Observable<T>`），或异步值（`Async<T>`），或其他概念。

但是，尽管各种不同的提升世界没有任何共同点，但它们的使用方式也有共同点。我们发现，某些问题在不同的高层次世界中反复出现，我们可以使用标准工具和模式来处理这些问题。

本系列的其余部分将尝试记录这些工具和模式。

## 系列内容

本系列开发如下：

- 首先，我将讨论我们将正常事物提升到提升世界的工具。这包括 `map`、`return`、`apply` 和 `bind` 等函数。
- 接下来，我将讨论如何根据值是独立的还是依赖的，以不同的方式组合提升的值。
- 接下来，我们将探讨一些将列表与其他提升值混合的方法。
- 最后，我们将看看两个使用所有这些技术的真实世界的例子，我们会发现自己意外地发明了 Reader monad。

以下是各种函数的快捷方式列表：

- **第 1 部分：提升到更高的世界**
  - `map` 函数
  - `return` 函数
  - `apply` 函数
  - `liftN` 系列函数
  - `zip` 函数和 ZipList 世界
- **第 2 部分：如何构建跨世界函数**
  - `bind` 函数
  - List 不是单子。Option 不是单子。
- **第 3 部分：在实践中使用核心函数**
  - 独立和依赖数据
  - 示例：使用应用函子风格和单子风格进行验证
  - 迈向一致的世界
  - Kleisli 世界
- **第 4 部分：混合列表和提升值**
  - 混合列表和提升值
  - `traverse` / `MapM` 函数
  - `sequence` 函数
  - “序列”作为ad-hoc实现的配方
  - 可读性与性能
  - 老兄，我的 `filter` 在哪里？
- **第 5 部分：使用所有技术的真实世界示例**
  - 示例：下载和处理网站列表
  - 将两个世界视为一体
- **第 6 部分：设计你自己的提升世界**
  - 设计你自己的提升世界
  - 过滤掉故障
  - Reader monad
- **第 7 部分：总结**
  - 提到的操作符名单
  - 进一步阅读

## 第 1 部分：提升到更高的世界

第一个挑战是：我们如何从正常世界进入更高的世界？

首先，我们将假设，对于任何特定的高阶世界：

- 正常世界中的每一种类型在提升世界中都有相应的类型。
- 正常世界中的每一个值在提升世界中都有相应的值。
- 正常世界中的每个函数在提升世界中都有相应的函数。

将某物从正常世界转移到提升世界的概念被称为“提升（lifting）”（这就是我最初使用“提升（elevated）世界”一词的原因）。

我们将把这些相应的东西称为“提升类型（lifted type）”、“提升值（lifted values）”和“提升函数（lifted function）”。

现在因为每个提升的世界都是不同的，所以没有通用的提升实现，但我们可以给各种“提升”模式命名，比如 `map` 和 `return`。

*注意：这些提升类型没有标准名称。我见过它们被称为“包装器类型”、“增强类型”或“单子类型”。我对这些名字都不太满意，所以我发明了一个新的！此外，我试图避免任何假设，所以我不想暗示提升的类型在某种程度上更好或包含额外的信息。我希望通过在这篇文章中使用“提升（elevated）”一词，我可以专注于提升过程，而不是类型本身。*

*至于使用“monadic”这个词，这是不准确的，因为没有要求这些类型是 monad 的一部分。*

## `map` 函数

**常用名称**：`map`、`fmap`、`lift`、`Select`

**常用运算符**：`<$>` `<!>`

**它的作用**：将函数提升到更高的世界

**签名**：`(a->b) -> E<a> -> E<b>`。或者，参数颠倒：`E<a> -> (a->b) -> E<b>`

### 说明

“map”是在正常世界中接受一个函数并将其转换为提升世界中相应函数的东西的通用名称。

![img](https://fsharpforfunandprofit.com/posts/elevated-world/vgfp_map.png)

每个高架世界都有自己的地图实现。

### 替代解释

对 `map` 的另一种解释是，它是一个双参数函数，接受一个提升值（`E<a>`）和一个正常函数（`a->b`），并返回一个新的提升值（`E<b>`），该值是通过将函数 `a->b` 应用于 `E<a>` 的内部元素而生成的。

![img](https://fsharpforfunandprofit.com/posts/elevated-world/vgfp_map2.png)

在默认情况下对函数进行 curried 的语言中，如 F#，这两种解释是相同的。在其他语言中，您可能需要 curry/uncurry 才能在两种用途之间切换。

请注意，双参数版本通常具有签名 `E<a> -> (a->b) -> E<b>`，首先是提升值，其次是正常函数。从抽象的角度来看，它们之间没有区别——映射概念是相同的——但很明显，参数顺序会影响你在实践中如何使用映射函数。

### 实现示例

这里有两个例子，说明如何在 F# 中为选项和列表定义映射。

```F#
/// map for Options
let mapOption f opt =
    match opt with
    | None ->
        None
    | Some x ->
        Some (f x)
// has type : ('a -> 'b) -> 'a option -> 'b option

/// map for Lists
let rec mapList f list =
    match list with
    | [] ->
        []
    | head::tail ->
        // new head + new tail
        (f head) :: (mapList f tail)
// has type : ('a -> 'b) -> 'a list -> 'b list
```

当然，这些函数是内置的，所以我们不需要定义它们，我这样做只是为了展示它们可能寻找的一些常见类型。

### 使用示例

以下是在 F# 中如何使用 map 的一些示例：

```F#
// Define a function in the normal world
let add1 x = x + 1
// has type : int -> int

// A function lifted to the world of Options
let add1IfSomething = Option.map add1
// has type : int option -> int option

// A function lifted to the world of Lists
let add1ToEachElement = List.map add1
// has type : int list -> int list
```

有了这些映射版本，你可以编写这样的代码：

```F#
Some 2 |> add1IfSomething    // Some 3
[1;2;3] |> add1ToEachElement // [2; 3; 4]
```

在许多情况下，我们不会费心创建中间函数，而是使用部分应用程序：

```F#
Some 2 |> Option.map add1    // Some 3
[1;2;3] |> List.map add1     // [2; 3; 4]
```

### 正确映射实现的属性

我之前说过，从某种意义上说，提升世界是正常世界的一面镜子。正常世界中的每个函数在提升世界中都有相应的函数，以此类推。我们希望 `map` 以一种合理的方式返回这个相应的提升函数。

例如，`add` 的 `map` 不应该（错误地）返回 `multiplied` 的提升版本，`lowercase` 的 `map` 不应返回 `uppercase` 的提升版本！但是，我们如何确保 map 的特定实现确实返回了正确的对应函数呢？

在我关于基于属性的测试的文章中，我展示了如何使用通用属性而不是具体示例来定义和测试函数的正确实现。

`map` 也是如此。实现将因特定的提升世界而异，但在所有情况下，实现都应该满足某些属性以避免奇怪的行为。

首先，如果你在正常世界中使用 `id` 函数，并用 `map` 将其提升到提升世界中，则新函数必须与提升世界中的 `id` 函数相同。

![img](https://fsharpforfunandprofit.com/posts/elevated-world/vgfp_functor_law_id.png)

接下来，如果你在正常世界中取两个函数 `f` 和 `g`，并将它们组合（比如说，组合成 `h`），然后使用 `map` 提升结果函数，则结果函数应该与你先将 `f` 和 `g` 提升到提升世界中，然后再在那里组合它们的情况相同。

![img](https://fsharpforfunandprofit.com/posts/elevated-world/vgfp_functor_law_compose.png)

这两个属性就是所谓的“函子（Functor）定律”，一个**函子（Functor）**（在编程意义上）被定义为一个泛型数据类型——在我们的例子中是 `E<T>` ——加上一个遵循 Functor 定律的 `map` 函数。

*注意：“Functor”是一个令人困惑的词。范畴论意义上有“functor”，编程意义上也有“functor”（如上所述）。还有一些在库中定义的叫做“functor”的东西，比如 Haskell 中的 Functor 类型类和 Scalaz 中的 Functor trait，更不用说 SML 和 OCaml（以及 C++）中的 functor 了，它们又是不同的！*

*因此，我更喜欢谈论“可映射”的世界。在实际编程中，你几乎永远不会遇到一个不支持以某种方式映射的高级世界。*

### map 变体

map 有一些常见的变体：

- **常量映射（Const map）**。const 或 “replace-by” 映射将所有值替换为常量，而不是函数的输出。在某些情况下，像这样的专门函数可以实现更高效的实现。
- 适用于跨世界功能的映射。映射函数 `a -> b` 完全存在于正常世界中。但是，如果你想要映射的函数在正常世界中没有返回值，而是在另一个不同的增强世界中返回值，该怎么办？我们将在稍后的文章中看到如何应对这一挑战。

## `return` 函数

**常用名称**：`return`，`pure`，`unit`，`yield`，`point`

**常用运算符**：无

**它的作用**：将单一值提升到更高的世界

**签名**：`a -> E<a>`

### 说明

“return”（也称为“unit”或“pure”）只是从正常值创建一个提升值。

![img](https://fsharpforfunandprofit.com/posts/elevated-world/vgfp_return.png)

这个函数有很多名字，但我会保持一致，并称之为 `return`，因为这是 F# 中常用的术语，也是计算表达式中使用的术语。

*注意：我忽略了 `pure` 和 `return` 之间的区别，因为类型类不是本文的重点。*

### 实现示例

以下是 F# 中 `return` 实现的两个示例：

```F#
// A value lifted to the world of Options
let returnOption x = Some x
// has type : 'a -> 'a option

// A value lifted to the world of Lists
let returnList x  = [x]
// has type : 'a -> 'a list
```

显然，我们不需要为选项和列表定义这样的特殊函数。再次，我只是为了展示一些常见类型的 `return`。

## `apply` 函数

**常用名称**：`apply`，`ap`

**常用运算符**：`<*>`

**它的作用**：将包装在提升值内的函数解包为提升函数 `E<a> -> E<b>`

**签名**：`E<(a->b)> -> E<a> -> E<b>`

### 说明

“apply”将封装在提升值（`E<(a->b)>`）内的函数解包为提升函数 `E<a> -> E<b>`

![img](https://fsharpforfunandprofit.com/posts/elevated-world/vgfp_apply.png)

这可能看起来不重要，但实际上非常有价值，因为它允许您将正常世界中的多参数函数提升为提升世界中的一个多参数函数，我们很快就会看到。

### 替代解释

对 `apply` 的另一种解释是，它是一个双参数函数，接受一个提升值（`E<a>`）和一个提升函数（`E<(a->b)>`），并返回一个新的提升值（`E<b>`），该值是通过将函数 `a->b` 应用于 `E<a>` 的内部元素而生成的。

例如，如果你有一个单参数函数（`E<(a->b)>`），你可以将它应用于一个提升的参数，以获得另一个提升值的输出。

![img](https://fsharpforfunandprofit.com/posts/elevated-world/vgfp_apply2.png)

如果你有一个双参数函数（`E<(a->b->c)>`），你可以连续两次使用两个提升的参数来获得提升的输出。

![img](https://fsharpforfunandprofit.com/posts/elevated-world/vgfp_apply3.png)

您可以继续使用此技术来处理任意多的参数。

### 实现示例

以下是在 F# 中为两种不同类型定义 `apply` 的一些示例：

```F#
module Option =

    // The apply function for Options
    let apply fOpt xOpt =
        match fOpt,xOpt with
        | Some f, Some x -> Some (f x)
        | _ -> None

module List =

    // The apply function for lists
    // [f;g] apply [x;y] becomes [f x; f y; g x; g y]
    let apply (fList: ('a->'b) list) (xList: 'a list)  =
        [ for f in fList do
          for x in xList do
              yield f x ]
```

在这种情况下，我没有给函数起像 `applyOption` 和 `applyList` 这样的名字，而是给了它们相同的名字，但把它们放在了一个按类型的模块中。

请注意，在 `List.apply` 实现中，第一个列表中的每个函数都应用于第二个列表的每个值，从而产生“叉积”样式的结果。也就是说，应用于值列表 `[x; y]` 的函数列表 `[f; g]` 成为四元素列表 `[f x; f y; g x; g y]`。我们很快就会看到，这不是唯一的方法。

当然，我也在作弊，因为我把这个实现建立在 `for..in..do` 循环上——已经存在的函数！

我这样做是为了清楚地展示应用程序是如何工作的。创建一个“从头开始”的递归实现很容易（尽管创建一个正确的尾递归实现并不容易！），但我现在想关注的是概念，而不是实现。

### apply 的 Infix 版本

按原样使用 `apply` 函数可能会很尴尬，因此通常会创建一个中缀版本，通常称为 `<*>`。有了这个，你可以编写这样的代码：

```F#
let resultOption =
    let (<*>) = Option.apply
    (Some add) <*> (Some 2) <*> (Some 3)
// resultOption = Some 5

let resultList =
    let (<*>) = List.apply
    [add] <*> [1;2] <*> [10;20]
// resultList = [11; 21; 12; 22]
```

### Apply vs. Map

`apply` 和 `return` 的组合被认为比 `map`“更强大”，因为如果你有 `apply` 和 `return`，你可以从它们构建 `map`，但反之则不然。

它的工作原理如下：要从普通函数构造一个提升函数，只需在普通函数上使用 `return`，然后 `apply`。这会给你带来和你一开始只是做 `map` 一样的结果。



这个技巧也意味着我们的中缀符号可以简化一点。初始 `return` 然后 `apply` 可以用 `map` 替换，因此我们通常也会为 `map` 创建一个中缀运算符，例如 `<!>` 在 F# 中。

```F#
let resultOption2 =
    let (<!>) = Option.map
    let (<*>) = Option.apply

    add <!> (Some 2) <*> (Some 3)
// resultOption2 = Some 5

let resultList2 =
    let (<!>) = List.map
    let (<*>) = List.apply

    add <!> [1;2] <*> [10;20]
// resultList2 = [11; 21; 12; 22]
```

这使得代码看起来更像是正常使用该函数。也就是说，我们可以使用外观相似的 `add <!> x <*> y` 来代替普通的 `add x y`，但现在 `x` 和 `y` 可以是高位值而不是正常值。有些人甚至称这种风格为“重载空格”！

这里还有一个好玩的：

```F#
let batman =
    let (<!>) = List.map
    let (<*>) = List.apply

    // string concatenation using +
    (+) <!> ["bam"; "kapow"; "zap"] <*> ["!"; "!!"]

// result =
// ["bam!"; "bam!!"; "kapow!"; "kapow!!"; "zap!"; "zap!!"]
```

### 正确应用/返回实现的属性

与 `map` 一样，`apply`/`return` 对的正确实现应该具有一些真实的属性，无论我们使用的是什么高级世界。

有四种所谓的“应用函子定律（Applicative Laws）”，**应用函子（Applicative Functor）**（在编程意义上）被定义为一个泛型数据类型构造函数——`E<T>`——加上一对遵守应用函子定律的函数（`apply` 和 `return`）。

就像 `map` 的定律一样，这些定律是相当合理的。我将展示其中两个。

第一定律说，如果你在正常世界中取 `id` 函数，并用 `return` 将其提升到提升世界中，然后你确实 `apply` 了，那么类型为 `E<a> -> E<a>` 的新函数应该与提升世界中的 `id` 函数相同。

![img](https://fsharpforfunandprofit.com/posts/elevated-world/vgfp_apply_law_id.png)

第二定律说，如果你在正常世界中取一个函数 `f` 和一个值 `x`，然后将 `f` 应用于 `x` 得到一个结果（比如 `y`），然后使用 `return` 提升结果，那么结果值应该与你先将 `f` 和 `x` 提升到提升世界，然后再将它们应用于提升世界的情况相同。

![img](https://fsharpforfunandprofit.com/posts/elevated-world/vgfp_apply_law_homomorphism.png)

另外两条定律不太容易绘制出来，所以我不会在这里记录它们，但这些定律共同确保了任何实现都是明智的。

## `liftN` 函数家族

**常用名称**：`lift2`、`lift3`、`lift4` 等

**常用运算符**：无

**它的作用**：使用指定函数组合两个（或三个，或四个）提升值

**签名**：lift2: `(a->b->c) -> E<a> -> E<b> -> E<c>`，lift3:`(a->b->c->d) -> E<a> -> E<b> -> E<c> -> E<d>`，以此类推。

### 说明

`apply` 和 `return` 函数可用于定义一系列辅助函数 `liftN`（`lift2`、`lift3`、`lift4` 等），这些函数接受具有 N 个参数的正常函数（其中 N=2,3,4 等），并将其转换为相应的提升函数。

请注意，`lift1` 只是 `map`，因此它通常不被定义为单独的函数。

以下是实现可能的样子：

```F#
module Option =
    let (<*>) = apply
    let (<!>) = Option.map

    let lift2 f x y =
        f <!> x <*> y

    let lift3 f x y z =
        f <!> x <*> y <*> z

    let lift4 f x y z w =
        f <!> x <*> y <*> z <*> w
```

以下是 `lift2` 的视觉表示：

![img](https://fsharpforfunandprofit.com/posts/elevated-world/vgfp_lift2.png)

`lift` 系列函数可用于使代码更具可读性，因为通过使用预先制作的 `lift` 函数之一，我们可以避免使用 `<*>` 语法。

首先，这里有一个提升双参数函数的示例：

```F#
// define a two-parameter function to test with
let addPair x y = x + y

// lift a two-param function
let addPairOpt = Option.lift2 addPair

// call as normal
addPairOpt (Some 1) (Some 2)
// result => Some 3
```

下面是一个提升三参数函数的示例：

```F#
// define a three-parameter function to test with
let addTriple x y z = x + y + z

// lift a three-param function
let addTripleOpt = Option.lift3 addTriple

// call as normal
addTripleOpt (Some 1) (Some 2) (Some 3)
// result => Some 6
```

### 将“lift2”解释为“组合器”

`apply` 有另一种解释，即作为提升值的“组合器”，而不是作为函数应用。

例如，使用 `lift2` 时，第一个参数是指定如何组合值的参数。

这里有一个例子，相同的值以两种不同的方式组合：首先是加法，然后是乘法。

```F#
Option.lift2 (+) (Some 2) (Some 3)   // Some 5
Option.lift2 (*) (Some 2) (Some 3)   // Some 6
```

更进一步地说，我们能否消除对第一个函数参数的需求，并采用一种泛型的方法来组合这些值？

为什么，是的，我们可以！我们可以使用元组构造函数来组合值。当我们这样做时，我们正在组合这些值，而还没有决定如何使用它们。

这是它在图表中的样子：

![img](https://fsharpforfunandprofit.com/posts/elevated-world/vgfp_apply_combine.png)

以下是如何为选项和列表实现它：

```F#
// define a tuple creation function
let tuple x y = x,y

// create a generic combiner of options
// with the tuple constructor baked in
let combineOpt x y = Option.lift2 tuple x y

// create a generic combiner of lists
// with the tuple constructor baked in
let combineList x y = List.lift2 tuple x y
```

让我们看看当我们使用组合器时会发生什么：

```F#
combineOpt (Some 1) (Some 2)
// Result => Some (1, 2)

combineList [1;2] [100;200]
// Result => [(1, 100); (1, 200); (2, 100); (2, 200)]
```

现在我们有了一个提升的元组，我们可以以任何我们想要的方式使用它，我们只需要使用 `map` 来进行实际的组合。

想要对值做加法吗？只需在 `map` 函数中使用 `+`：

```F#
combineOpt (Some 2) (Some 3)
|> Option.map (fun (x,y) -> x + y)
// Result => // Some 5

combineList [1;2] [100;200]
|> List.map (fun (x,y) -> x + y)
// Result => [101; 201; 102; 202]
```

想要将这些值相乘吗？只需在 `map` 函数中使用 `*`：

```F#
combineOpt (Some 2) (Some 3)
|> Option.map (fun (x,y) -> x * y)
// Result => Some 6

combineList [1;2] [100;200]
|> List.map (fun (x,y) -> x * y)
// Result => [100; 200; 200; 400]
```

等等。显然，现实世界的使用会更有趣。

### 用 `lift2` 的术语定义 `apply`

有趣的是，上述 `lift2` 函数实际上可以用作定义 `apply` 的替代基础。

也就是说，我们可以通过将组合函数设置为函数应用来定义 `lift2` 函数的 `apply`。

以下是 `Option` 的工作原理演示：

```F#
module Option =

    /// define lift2 from scratch
    let lift2 f xOpt yOpt =
        match xOpt,yOpt with
        | Some x,Some y -> Some (f x y)
        | _ -> None

    /// define apply in terms of lift2
    let apply fOpt xOpt =
        lift2 (fun f x -> f x) fOpt xOpt
```

这种替代方法值得了解，因为对于某些类型，定义 `lift2` 比 `apply` 更容易。

### 合并缺失或错误的数据

请注意，在我们研究的所有组合器中，如果其中一个升高的值不知何故“缺失”或“糟糕”，那么整体结果也很糟糕。

例如，使用 `combineList` 时，如果其中一个参数是空列表，则结果也是空列表；使用 `combineOpt` 时，如果一个参数为 `None`，则结果也为 `None`。

```F#
combineOpt (Some 2) None
|> Option.map (fun (x,y) -> x + y)
// Result => None

combineList [1;2] []
|> List.map (fun (x,y) -> x * y)
// Result => Empty list
```

可以创建一种替代类型的组合器，忽略缺失或错误的值，就像忽略向数字添加“0”一样。有关更多信息，请参阅我在“没有眼泪的 Monoid”上的帖子。

### 单侧组合器 `<*` 和 `*>`

在某些情况下，您可能有两个升高的值，并希望从一侧或另一侧丢弃该值。

以下是一个列表示例：

```F#
let ( <* ) x y =
    List.lift2 (fun left right -> left) x y

let ( *> ) x y =
    List.lift2 (fun left right -> right) x y
```

然后，我们可以组合一个 2 元素列表和一个 3 元素列表，得到一个 6 元素列表，但内容只来自一侧或另一侧。

```F#
[1;2] <* [3;4;5]   // [1; 1; 1; 2; 2; 2]
[1;2] *> [3;4;5]   // [3; 4; 5; 3; 4; 5]
```

我们可以把它变成一个函数！我们可以通过将一个值与 `[1..N]` 交叉来复制 N 次。

```F#
let repeat n pattern =
    [1..n] *> pattern

let replicate n x =
    [1..n] *> [x]

repeat 3 ["a";"b"]
// ["a"; "b"; "a"; "b"; "a"; "b"]

replicate 5 "A"
// ["A"; "A"; "A"; "A"; "A"]
```

当然，这绝不是复制值的有效方法，但它确实表明，从两个函数 `apply` 和 `return` 开始，您可以构建一些相当复杂的行为。

然而，从更实际的角度来看，为什么这种“丢弃数据”可能有用？在许多情况下，我们可能不想要这些值，但我们确实想要效果。

例如，在解析器中，您可能会看到这样的代码：

```F#
let readQuotedString =
   readQuoteChar *> readNonQuoteChars <* readQuoteChar
```

在这段代码中，`readQuoteChar` 表示“从输入流中匹配并读取引号字符”，`readNonQuoteChars` 表示“从输出流中读取一系列非引号字符”。

当我们解析引号字符串时，我们希望确保包含引号字符的输入流被读取，但我们不关心引号字符本身，只关心内部内容。

因此，使用 `*>` 忽略前导引号，使用 `<*` 忽略尾随引号。

## `zip` 函数和 ZipList 世界

**常用名称**：`zip`、`zipWith`、`map2`

**常见运算符**：`<*>`（在 ZipList 世界中）

**它的作用**：使用指定的函数组合两个列表（或其他可枚举项）

**签名**：`E<(a->b->c)> -> E<a> -> E<b> -> E<c>`，其中 `E` 是一个列表或其他可枚举类型，或者 `E<a> -> E<b> -> E<a,b>` 表示元组组合版本。

### 说明

某些数据类型可能有多个有效的 `apply` 实现。例如，还有另一种可能的 `apply` 列表实现，通常称为 `ZipList` 或其变体。

在这个实现中，同时处理每个列表中的相应元素，然后移动两个列表以获得下一个元素。也就是说，应用于值列表 `[x; y]` 的函数列表 `[f; g]` 变为两元素列表 `[f x; g y]`

```F#
// alternate "zip" implementation
// [f;g] apply [x;y] becomes [f x; g y]
let rec zipList fList xList  =
    match fList,xList with
    | [],_
    | _,[] ->
        // either side empty, then done
        []
    | (f::fTail),(x::xTail) ->
        // new head + new tail
        (f x) :: (zipList fTail xTail)
// has type : ('a -> 'b) -> 'a list -> 'b list
```

*警告：此实现仅用于演示。它不是尾部递归的，所以不要将其用于大型列表！*

如果列表的长度不同，一些实现会抛出异常（如 F# 库函数 `List.map2` 和 `List.zip` 所做的那样），而另一些实现则会默默地忽略额外的数据（如上面的实现所做的）。

好的，让我们看看它的使用情况：

```F#
let add10 x = x + 10
let add20 x = x + 20
let add30 x = x + 30

let result =
    let (<*>) = zipList
    [add10; add20; add30] <*> [1; 2; 3]
// result => [11; 22; 33]
```

请注意，结果是 `[11; 22; 33]` ——只有三个元素。如果我们使用标准的 `List.apply`，就会有九个元素。

### 将“zip”解释为“组合器”

我们在上面看到，`List.apply`，或者更确切地说，`List.lift2`，可以被解释为组合器。同样，`zipList` 也是如此。

```F#
let add x y = x + y

let resultAdd =
    let (<*>) = zipList
    [add;add] <*> [1;2] <*> [10;20]
// resultAdd = [11; 22]
// [ (add 1 10); (add 2 20) ]
```

请注意，我们不能在第一个列表中只有一个 `add` 函数——我们必须为第二个和第三个列表中的每个元素都有一个 `add`！

这可能会很烦人，所以经常使用 `zip` 的“元组”版本，在这种版本中，您根本不需要指定组合函数，只需要返回一个元组列表，然后可以稍后使用 `map` 进行处理。这与上面讨论的 `combine` 函数中使用的方法相同，但用于 `zipList`。

### ZipList 世界

在标准的 List 世界中，有一个 `apply` 和一个 `return`。但是，通过我们不同版本的 `apply`，我们可以创建一个不同版本的列表世界，称为 ZipList 世界。

ZipList 世界与标准 List 世界截然不同。

在 ZipList 世界中，`apply` 函数如上所述实现。但更有趣的是，与标准列表世界相比，ZipList 世界的 `return` 实现完全不同。在标准的 List 世界中，`return` 只是一个包含单个元素的列表，但对于 ZipList 世界，它必须是一个无限重复的值！

在 F# 这样的非惰性语言中，我们不能这样做，但如果我们用 `Seq`（又名 `IEnumerable`）替换 `List`，那么我们可以创建一个无限重复的值，如下所示：

```F#
module ZipSeq =

    // define "return" for ZipSeqWorld
    let retn x = Seq.initInfinite (fun _ -> x)

    // define "apply" for ZipSeqWorld
    // (where we can define apply in terms of "lift2", aka "map2")
    let apply fSeq xSeq  =
        Seq.map2 (fun f x -> f x)  fSeq xSeq
    // has type : ('a -> 'b) seq -> 'a seq -> 'b seq

    // define a sequence that is a combination of two others
    let triangularNumbers =
        let (<*>) = apply

        let addAndDivideByTwo x y = (x + y) / 2
        let numbers = Seq.initInfinite id
        let squareNumbers = Seq.initInfinite (fun i -> i * i)
        (retn addAndDivideByTwo) <*> numbers <*> squareNumbers

    // evaluate first 10 elements
    // and display result
    triangularNumbers |> Seq.take 10 |> List.ofSeq |> printfn "%A"
    // Result =>
    // [0; 1; 3; 6; 10; 15; 21; 28; 36; 45]
```

这个例子表明，一个提升的世界不仅仅是一种数据类型（如列表类型），而是由数据类型和与之配合的函数组成。在这个特定的情况下，“列表世界”和“ZipList 世界”共享相同的数据类型，但具有完全不同的环境。

## 哪些类型支持 `map` 和 `apply` 和 `return`?

到目前为止，我们已经以抽象的方式定义了所有这些有用的函数。但是，找到具有这些实现的真实类型（包括所有各种法律）有多容易呢？

答案是：非常简单！事实上，几乎所有类型都支持这些函数集。你很难找到一种有用的类型。

这意味着 `map`、`apply` 和 `return` 可用于（或可以很容易地实现）`Option`、`List`、`Seq`、`Async` 等标准类型，以及您可能自己定义的任何类型。

## 摘要

在这篇文章中，我描述了将简单的“正常”值提升到提升世界的三个核心函数：`map`、`return` 和 `apply`，以及一些派生函数，如 `liftN` 和 `zip`。

然而，在实践中，事情并没有那么简单。我们经常需要处理跨世界的函数。他们的投入在正常世界，但他们的产出在高水平世界。

在下一篇文章中，我们将展示如何将这些跨越世界的功能提升到更高的世界。

# 2 理解绑定

*Part of the "Map and Bind and Apply, Oh my!" series (*[link](https://fsharpforfunandprofit.com/posts/elevated-world-2/#series-toc)*)*

或者，如何构建跨越世界的函数
03八月2015 这篇文章是超过3年

https://fsharpforfunandprofit.com/posts/elevated-world-2/

这篇文章是系列文章中的第二篇。在上一篇文章中，我描述了将价值从正常世界提升到更高世界的一些核心功能。

在这篇文章中，我们将介绍“跨世界”函数，以及如何使用 `bind` 函数来驯服它们。

## 系列内容

以下是本系列中提到的各种函数的快捷方式列表：

- **第 1 部分：提升到更高的世界**
  - `map` 函数
  - `return` 函数
  - `apply` 函数
  - `liftN` 系列函数
  - `zip` 函数和 ZipList 世界
- **第 2 部分：如何构建跨世界函数**
  - `bind` 函数
  - List 不是单子。Option 不是单子。
- **第 3 部分：在实践中使用核心函数**
  - 独立和依赖数据
  - 示例：使用应用函子风格和单子风格进行验证
  - 迈向一致的世界
  - Kleisli 世界
- **第 4 部分：混合列表和提升值**
  - 混合列表和提升值
  - `traverse` / `MapM` 函数
  - `sequence` 函数
  - “序列”作为ad-hoc实现的配方
  - 可读性与性能
  - 老兄，我的 `filter` 在哪里？
- **第 5 部分：使用所有技术的真实世界示例**
  - 示例：下载和处理网站列表
  - 将两个世界视为一体
- **第 6 部分：设计你自己的提升世界**
  - 设计你自己的提升世界
  - 过滤掉故障
  - Reader monad
- **第 7 部分：总结**
  - 提到的操作符名单
  - 进一步阅读

## 第 2 部分：如何构建跨世界函数

## `bind` 函数

**常用名称**： `bind`, `flatMap`, `andThen`, `collect`, `SelectMany`

**常用运算符**：`>>=`（从左到右），`=<<`（从右到左）

**它的作用**：允许您编写跨世界（“monadic”）函数

**签名**：`(a->E<b>) -> E<a> -> E<b>`。或者，参数颠倒： `E<a> -> (a->E<b>) -> E<b>`

### 说明

我们经常需要处理介于正常世界和提升世界之间的函数。

例如：一个将 `string` 解析为 `int` 的函数可能会返回 `Option<int>` 而不是普通的 `int`，一个从文件中读取行的函数可能返回 `IEnumerable<string>`，一个获取网页的函数可能回报 `Async<string>`，等等。

这些类型的“跨世界”函数可以通过它们的签名 `a -> E<b>` 来识别；他们的投入在正常世界，但他们的产出在高端世界。不幸的是，这意味着这些函数不能使用标准组合链接在一起。

![img](https://fsharpforfunandprofit.com/posts/elevated-world-2/vgfp_bind_noncomposition.png)

“bind”的作用是将一个世界交叉函数（通常称为“一元函数”）转换为提升函数 `E<a> -> E<b>`。

![img](https://fsharpforfunandprofit.com/posts/elevated-world-2/vgfp_bind.png)

这样做的好处是，由此产生的提升功能纯粹存在于提升的世界中，因此可以通过组合轻松组合。

例如，类型为 `a -> E<b>` 的函数不能直接与类型为 `b -> E<c>` 的函数组合，但在使用 `bind` 后，第二个函数变为类型为 `E<b> -> E<c>`，可以组合。

![img](https://fsharpforfunandprofit.com/posts/elevated-world-2/vgfp_bind_composition.png)

通过这种方式，`bind` 允许我们将任意数量的一元函数链接在一起。

### 替代解释

`bind` 的另一种解释是，它是一个双参数函数，它接受一个提升值（`E<a>`）和一个“单子函数”（`a -> E<b>`），并返回一个新的提升值（`E<b>`，这是通过“展开”输入中的值并对其运行函数 `a -> E<b>` 生成的。当然，“展开”隐喻并不适用于每个提升的世界，但以这种方式思考它通常是有用的。

![img](https://fsharpforfunandprofit.com/posts/elevated-world-2/vgfp_bind2.png)

### 实现示例

以下是在 F# 中为两种不同类型定义 `bind` 的一些示例：

```F#
module Option =

    // The bind function for Options
    let bind f xOpt =
        match xOpt with
        | Some x -> f x
        | _ -> None
    // has type : ('a -> 'b option) -> 'a option -> 'b option

module List =

    // The bind function for lists
    let bindList (f: 'a->'b list) (xList: 'a list)  =
        [ for x in xList do
          for y in f x do
              yield y ]
    // has type : ('a -> 'b list) -> 'a list -> 'b list
```

笔记：

- 当然，在这两种特定情况下，函数已经存在于 F# 中，称为 `Option.bind` 和 `List.collect`。
- 对于 `List.bind`，我再次作弊并使用 `for..in..do`，但我认为这个特定的实现清楚地展示了 bind 如何与列表一起工作。有一个更纯粹的递归实现，但我不会在这里展示。

### 使用示例

正如本节开头所解释的，`bind` 可用于组合跨世界函数。让我们通过一个简单的例子来看看这在实践中是如何工作的。

首先，假设我们有一个函数，可以将某些 `string` 解析为 `int`。下面是一个非常简单的实现：

```F#
let parseInt str =
    match str with
    | "-1" -> Some -1
    | "0" -> Some 0
    | "1" -> Some 1
    | "2" -> Some 2
    // etc
    | _ -> None

// signature is string -> int option
```

有时返回 int，有时不返回。所以签名是 `string -> int option`——一个跨世界函数。

假设我们有另一个函数，它接受 `int` 作为输入并返回 `OrderQty` 类型：

```F#
type OrderQty = OrderQty of int

let toOrderQty qty =
    if qty >= 1 then
        Some (OrderQty qty)
    else
        // only positive numbers allowed
        None

// signature is int -> OrderQty option
```

同样，如果输入不是正数，它可能不会返回 `OrderQty`。因此，签名是 `int -> OrderQty option`——另一个跨世界函数。

现在，我们如何创建一个以字符串开头并在一步中返回 `OrderQty` 的函数？

`parseInt` 的输出不能直接输入到 `toOrderQty`中，所以这就是 `bind` 的用武之地！

执行 `Option.bind toOrderQty` 将其提升为 `int option -> OrderQty option` 函数，因此 `parseInt` 的输出可以用作输入，正如我们需要的那样。

```F#
let parseOrderQty str =
    parseInt str
    |> Option.bind toOrderQty
// signature is string -> OrderQty option
```

我们的新 `parseOrderQty` 的签名是 `string -> OrderQty option`，这是另一个跨世界函数。因此，如果我们想对输出的 `OrderQty` 做点什么，我们可能必须在链中的下一个函数上再次使用 `bind`。

### bind 的 Infix 版本

与 `apply` 一样，使用命名的 `bind` 函数可能会很尴尬，因此通常会创建一个中缀版本，通常称为 `>>=`（用于从左到右的数据流）或 `=<<`（用于从右到左的数据流。

有了这个，你可以编写一个 `parseOrderQty` 的替代版本，如下所示：

```F#
let parseOrderQty_alt str =
    str |> parseInt >>= toOrderQty
```

您可以看到，`>>=` 执行与管道（`|>`）相同的角色，除了它用于将“提升”值管道化到跨世界函数中。

### 绑定为“可编程分号”

Bind可用于将任意数量的函数或表达式链接在一起，因此您经常看到类似这样的代码：

```F#
expression1 >>=
expression2 >>=
expression3 >>=
expression4
```

这与用一个 `;` 替换 `>>=` 时命令式程序的外观没有太大区别：

```F#
statement1;
statement2;
statement3;
statement4;
```

因此，`bind` 有时被称为“可编程分号”。

### 绑定/返回的语言支持

大多数函数式编程语言对 `bind` 都有某种语法支持，使您无需编写一系列延续或使用显式绑定。

在 F# 中，它是计算表达式的（一个组件），因此以下是 `bind` 的显式链接：

```F#
initialExpression >>= (fun x ->
expressionUsingX  >>= (fun y ->
expressionUsingY  >>= (fun z ->
x+y+z )))             // return
```

使用 `let!` 语法变得隐式：

```F#
elevated {
    let! x = initialExpression
    let! y = expressionUsingX x
    let! z = expressionUsingY y
    return x+y+z }
```

在 Haskell 中，等价物是“do 符号（notation）”：

```haskell
do
    x <- initialExpression
    y <- expressionUsingX x
    z <- expressionUsingY y
    return x+y+z
```

在 Scala 中，等价物是“for comprehensive”：

```scala
for {
    x <- initialExpression
    y <- expressionUsingX(x)
    z <- expressionUsingY(y)
} yield {
    x+y+z
}
```

重要的是要强调，在使用 bind / rereturn 时不必使用特殊语法。你总是可以像使用其他函数一样使用 `bind` 或 `>>=`。

### 绑定 vs. 应用 vs. 映射

`bind` 和 `return` 的组合被认为比 `apply` 和 `return` 更强大，因为如果你有 `bind` 和 `return`，你可以从它们构建 `map` 和 `apply`，但反之则不然。

以下是如何使用 bind 来模拟 `map`，例如：

- 首先，通过将 `return` 应用于输出，从普通函数构造一个世界交叉函数。
- 接下来，使用 `bind` 将这个世界交叉函数转换为提升函数。这会给你带来与你一开始只是做 `map` 相同的结果。

![img](https://fsharpforfunandprofit.com/posts/elevated-world-2/vgfp_bind_vs_map.png)

同样，`bind` 可以模拟 `apply`。以下是如何使用 F# 中 Options 的 `bind` 和 `return` 定义 `map` 和 `apply`：

```F#
// map defined in terms of bind and return (Some)
let map f =
    Option.bind (f >> Some)

// apply defined in terms of bind and return (Some)
let apply fOpt xOpt =
    fOpt |> Option.bind (fun f ->
        let map = Option.bind (f >> Some)
        map xOpt)
```

此时，人们经常问“当 `bind` 更强大时，为什么我应该使用 `apply` 而不是 `bind`？”

答案是，仅仅因为 `bind` 可以模拟 `apply`，并不意味着它应该如此。例如，有可能以 `bind` 实现无法模拟的方式实现 `apply`。

事实上，使用 `apply`（“应用函子风格”）或 `bind`（“单子风格”）会对程序的工作方式产生深远的影响！我们将在本文的第 3 部分更详细地讨论这两种方法。

### 正确绑定/返回实现的属性

与 `map` 和 `apply`/`return` 一样，`bind`/`return` 对的正确实现应该具有一些真实的属性，无论我们使用的是什么高级世界。

有三个所谓的“Monad 定律”，定义 **Monad** 的一种方式（在编程意义上）是说它由三件事组成：一个泛型类型构造函数 `E<T>` 加上一对遵守 Monad 定律的函数（`bind` 和 `return`）。这不是定义单子的唯一方法，数学家通常使用略有不同的定义，但这个定义对程序员最有用。

正如我们之前看到的函子定律和应用函子定律一样，这些定律是相当合理的。

首先，请注意 `return` 函数本身就是一个跨世界函数：

![img](https://fsharpforfunandprofit.com/posts/elevated-world-2/vgfp_monad_law1_a.png)

这意味着我们可以使用 `bind` 将其提升到提升世界中的一个函数中。这个提升的功能是什么？希望什么都没有！它应该只返回其输入。

所以这正是第一个单子定律：它说这个提升的函数必须与提升世界中的 `id` 函数相同。

![img](https://fsharpforfunandprofit.com/posts/elevated-world-2/vgfp_monad_law1_b.png)

第二条定律与之相似，但 `bind` 和 `return` 是相反的。假设我们有一个正常值 `a` 和一个将 `a` 转化为 `E<b>` 的跨世界函数 `f`。

![img](https://fsharpforfunandprofit.com/posts/elevated-world-2/vgfp_monad_law2_a.png)

让我们把它们都提升到更高的世界，在 `f` 上使用 `bind`，在 `a` 上使用 `return`。

![img](https://fsharpforfunandprofit.com/posts/elevated-world-2/vgfp_monad_law2_b.png)

现在，如果我们将 `f` 的提升版本应用于 `a` 的提升版本，我们会得到一些值 `E<b>`。

![img](https://fsharpforfunandprofit.com/posts/elevated-world-2/vgfp_monad_law2_c.png)

另一方面，如果我们将 `f` 的正常版本应用于 `a` 的正常版本，我们也会得到一些值 `E<b>`。

![img](https://fsharpforfunandprofit.com/posts/elevated-world-2/vgfp_monad_law2_d.png)

第二个单子定律说，这两个升高的值（`E<b>`）应该是相同的。换句话说，所有这些绑定和返回都不应该扭曲数据。

第三条单子定律是关于结合性的。

在正常世界中，函数组合是结合的。例如，我们可以将一个值管道化到函数 `f` 中，然后将该结果管道化到另一个函数 `g` 中。或者，我们可以先将 `f` 和 `g` 组合成一个函数，然后将 `a` 管道化到其中。

```F#
let groupFromTheLeft = (a |> f) |> g
let groupFromTheRight = a |> (f >> g)
```

在正常情况下，我们希望这两种选择都能给出相同的答案。

第三条单子定律说，在使用 `bind` 和 `return` 之后，分组也不重要。以下两个示例与上述示例相对应：

```F#
let groupFromTheLeft = (a >>= f) >>= g
let groupFromTheRight = a >>= (fun x -> f x >>= g)
```

同样，我们希望这两者都能给出相同的答案。

## List 不是单子。Option 不是单子。

如果你看看上面的定义，monad 有一个类型构造函数（也称为“泛型类型”）和两个函数以及一组必须满足的属性。

因此，List 数据类型只是单子的一个组件，Option 数据类型也是如此。List 和 Option 本身并不是单子。

将单子视为一种转换可能会更好，因此“列表单子”是将正常世界转换为提升的“列表世界”的转换，而“选项单子”是从正常世界转换到提升的“选项世界”的转变。

我认为这就是很多混淆的地方。“列表”一词可以有很多不同的含义：

1. 一种具体的类型或数据结构，如 `List<int>`。
2. 类型构造函数（泛型类型）：`List<T>`。
3. 类型构造函数和一些操作，如 `List` 类或模块。
4. 一个类型构造函数和一些操作以及这些操作满足 monad 定律。

只有最后一个是单子！其他含义是有效的，但会造成混淆。

此外，通过查看代码很难区分最后两种情况。不幸的是，在某些情况下，实现不符合单子定律。仅仅因为它是“单子”并不意味着它是单子。

就我个人而言，我尽量避免在这个网站上使用“monad”这个词，而是把重点放在 `bind` 函数上，作为解决问题的函数工具包的一部分，而不是抽象概念。

所以不要问：我有单子吗？

请问：我有有用的绑定和返回函数吗？它们是否得到了正确的实现？

## 摘要

我们现在有一组四个核心函数：`map`、`return`、`apply` 和 `bind`，我希望您清楚每个函数的作用。

但还有一些问题尚未得到解决，比如“为什么我应该选择 `apply` 而不是 `bind`？”，或者“我如何同时处理多个提升的世界？”

在下一篇文章中，我们将解决这些问题，并通过一系列实际示例演示如何使用该工具集。

*更新：修复了 @joseanpg 指出的单子定律中的错误。谢谢！*

# 3 在实践中使用核心函数

*Part of the "Map and Bind and Apply, Oh my!" series (*[link](https://fsharpforfunandprofit.com/posts/elevated-world-3/#series-toc)*)*

处理独立和依赖的数据
04八月2015 这篇文章是超过3年

https://fsharpforfunandprofit.com/posts/elevated-world-3/

这篇文章是系列文章中的第三篇。在前两篇文章中，我描述了处理通用数据类型的一些核心功能： `map`, `apply`, `bind` 等。

在这篇文章中，我将展示如何在实践中使用这些函数，并解释所谓的“应用函子”和“单子”风格之间的区别。

## 系列内容

以下是本系列中提到的各种函数的快捷方式列表：

- **第 1 部分：提升到更高的世界**
  - `map` 函数
  - `return` 函数
  - `apply` 函数
  - `liftN` 系列函数
  - `zip` 函数和 ZipList 世界
- **第 2 部分：如何构建跨世界函数**
  - `bind` 函数
  - List 不是单子。Option 不是单子。
- **第 3 部分：在实践中使用核心函数**
  - 独立和依赖数据
  - 示例：使用应用函子风格和单子风格进行验证
  - 迈向一致的世界
  - Kleisli 世界
- **第 4 部分：混合列表和提升值**
  - 混合列表和提升值
  - `traverse` / `MapM` 函数
  - `sequence` 函数
  - “序列”作为ad-hoc实现的配方
  - 可读性与性能
  - 老兄，我的 `filter` 在哪里？
- **第 5 部分：使用所有技术的真实世界示例**
  - 示例：下载和处理网站列表
  - 将两个世界视为一体
- **第 6 部分：设计你自己的提升世界**
  - 设计你自己的提升世界
  - 过滤掉故障
  - Reader monad
- **第 7 部分：总结**
  - 提到的操作符名单
  - 进一步阅读

## 第 3 部分：在实践中使用核心函数

现在我们已经有了将正常值提升到更高值并使用跨世界函数的基本工具，是时候开始使用它们了！

在本节中，我们将查看一些实际使用这些函数的示例。

## 独立数据与依赖数据

我之前简要提到过，使用 `apply` 和 `bind` 之间有一个重要区别。现在让我们来讨论一下。

使用 `apply` 时，您可以看到每个参数（`E<a>`，`E<b>`）都完全独立于其他参数。`E<b>` 的值不取决于 `E<a>` 是什么。

![img](https://fsharpforfunandprofit.com/posts/elevated-world/vgfp_apply3.png)

另一方面，当使用 `bind` 时，`E<b>` 的值确实取决于 `E<a>` 是什么。

![img](https://fsharpforfunandprofit.com/posts/elevated-world-2/vgfp_bind.png)

使用独立值或依赖值之间的区别导致了两种不同的风格：

- 所谓的“应用函子性（applicative）”风格使用诸如 `apply`、`lift` 和 `combine` 等功能，其中每个提升的值都是独立的。
- 所谓的“单子性（monadic）”风格使用诸如 `bind` 之类的函数，将依赖于先前值的函数链接在一起。

这在实践中意味着什么？好吧，让我们来看一个例子，你可以从这两种方法中进行选择。

假设你必须从三个网站下载数据并将其合并。假设我们有一个操作，比如 `GetURL`，它根据需要从网站获取数据。

现在你有一个选择：

- **您想并行获取所有 URL 吗**？如果是这样，请将 `GetURL` 视为独立数据并使用应用函子性样式。
- **您想一次获取一个 URL，如果前一个失败，跳过下一行吗**？如果是这样，请将 `GetURL` 视为依赖数据并使用单子性样式。这种线性方法总体上比上述“应用函子性”版本慢，但也可以避免不必要的 I/O。
- **下一个网站的 URL 是否取决于您从上一个网站下载的内容**？在这种情况下，您必须使用“monadic”样式，因为每个 `GetURL` 都依赖于前一个 GetURL 的输出。

正如你所看到的，应用函子性风格和单子性风格之间的选择并不明确；这取决于你想做什么。

我们将在本系列的最后一篇文章中查看此示例的实际实现。

### 但是…

重要的是要说，仅仅因为你选择了一种风格，并不意味着它会按照你的期望实现。正如我们所看到的，你可以很容易地使用 `bind` 实现 `apply`，所以即使你在代码中使用了 `<*>`，实现也可能是单子性进行的。

在上面的示例中，实现不必并行运行下载。它可以连续运行它们。通过使用应用函子性风格，你只是说你不关心依赖关系，所以它们可以并行下载。

### 静态 vs. 动态结构

如果你使用应用函子性风格，这意味着你预先定义了所有的操作——可以说是“静态的”。

在下载示例中，应用函子性风格要求您提前指定将访问哪些 URL。因为前面有更多的知识，这意味着我们可以做并行化或其他优化之类的事情。

另一方面，单子性风格意味着只有初始动作是预先知道的。其余的动作是基于先前动作的输出动态确定的。这更灵活，但也限制了我们提前看到大局的能力。

### 求值顺序 vs. 依赖关系

有时，依赖性与求值顺序相混淆。

当然，如果一个值依赖于另一个值，那么第一个值必须在第二个值之前计算。理论上，如果这些值是完全独立的（并且没有副作用），那么它们可以按任何顺序进行评估。

然而，即使这些值完全独立，它们的求值方式仍然可能存在隐式顺序。

例如，即使 `GetURL` 列表是并行完成的，URL 也很可能会从第一个开始，按照列出的顺序开始获取。

在上一篇文章中实现的 `List.apply` 中，我们看到 `[f; g] apply [x; y]` 的结果是 `[f x; f y; g x; g y]`，而不是 `[f x; g x; f y; g y]`。也就是说，所有的 `f` 值都是第一个，然后是所有的 `g` 值。

一般来说，有一种惯例是，即使值是独立的，也会按照从左到右的顺序进行计算。

## 示例：使用应用函子风格和单子风格进行验证

为了了解如何使用应用函子风格和单子风格，让我们来看一个使用验证的示例。

假设我们有一个简单的域，其中包含 `CustomerId`、`EmailAddress` 和 `CustomerInfo`，`CustomerInfo` 是一个包含这两者的记录。

```F#
type CustomerId = CustomerId of int
type EmailAddress = EmailAddress of string
type CustomerInfo = {
    id: CustomerId
    email: EmailAddress
    }
```

假设在创建 `CustomerId` 时需要进行一些验证。例如，内部 `int` 必须为正。当然，创建 `EmailAddress` 也需要一些验证。例如，它必须至少包含一个“@”符号。

我们该怎么做？

首先，我们创建一个类型来表示验证的成功/失败。

```F#
type Result<'a> =
    | Success of 'a
    | Failure of string list
```

请注意，我已经将 `Failure` 案例定义为包含字符串列表，而不仅仅是一个字符串。这将在以后变得重要。

有了 `Result`，我们可以继续根据需要定义两个构造函数/验证函数：

```F#
let createCustomerId id =
    if id > 0 then
        Success (CustomerId id)
    else
        Failure ["CustomerId must be positive"]
// int -> Result<CustomerId>

let createEmailAddress str =
    if System.String.IsNullOrEmpty(str) then
        Failure ["Email must not be empty"]
    elif str.Contains("@") then
        Success (EmailAddress str)
    else
        Failure ["Email must contain @-sign"]
// string -> Result<EmailAddress>
```

请注意，`createCustomerId` 的类型为 `int -> Result<CustomerId>`，`createEmailAddress` 的类型为 `string -> Result<EmailAddress>`。

这意味着这两个验证函数都是世界交叉函数，从正常世界到 `Result<_>` 世界。

### 为 `Result` 定义核心函数

由于我们处理的是跨世界函数，我们知道必须使用 `apply` 和 `bind` 等函数，所以让我们为 `Result` 类型定义它们。

```F#
module Result =

    let map f xResult =
        match xResult with
        | Success x ->
            Success (f x)
        | Failure errs ->
            Failure errs
    // Signature: ('a -> 'b) -> Result<'a> -> Result<'b>

    // "return" is a keyword in F#, so abbreviate it
    let retn x =
        Success x
    // Signature: 'a -> Result<'a>

    let apply fResult xResult =
        match fResult,xResult with
        | Success f, Success x ->
            Success (f x)
        | Failure errs, Success x ->
            Failure errs
        | Success f, Failure errs ->
            Failure errs
        | Failure errs1, Failure errs2 ->
            // concat both lists of errors
            Failure (List.concat [errs1; errs2])
    // Signature: Result<('a -> 'b)> -> Result<'a> -> Result<'b>

    let bind f xResult =
        match xResult with
        | Success x ->
            f x
        | Failure errs ->
            Failure errs
    // Signature: ('a -> Result<'b>) -> Result<'a> -> Result<'b>
```

如果我们检查签名，我们可以看到它们正是我们想要的：

- `map` 有签名： `('a -> 'b) -> Result<'a> -> Result<'b>`
- `retn` 有签名： `'a -> Result<'a>`
- `apply` 有签名： `Result<('a -> 'b)> -> Result<'a> -> Result<'b>`
- `bind` 有签名： `('a -> Result<'b>) -> Result<'a> -> Result<'b>`

我在模块中定义了一个 `retn` 函数以保持一致，但我并不经常使用它。`return` 的概念很重要，但在实践中，我可能会直接使用 `Success` 构造函数。在具有类型类的语言中，如 Haskell，`return` 的使用要多得多。

还要注意，如果两个参数都失败，`apply` 将合并来自每一方的错误消息。这使我们能够收集所有故障，而不会丢弃任何故障。这就是为什么我让 `Failure` 案例有一个字符串列表，而不是一个字符串。

*注意：我在失败案例中使用 `string` 是为了使演示更容易。在更复杂的设计中，我会明确列出可能的失败。有关更多详细信息，请参阅我的功能错误处理演讲。*

### 使用应用函子风格进行验证

现在我们已经有了围绕 `Result` 的域和工具集，让我们尝试使用应用程序样式创建 `CustomerInfo` 记录。

验证的输出已经提升到 `Result`，所以我们知道我们需要使用某种“提升”方法来处理它们。

首先，我们将在普通世界中创建一个函数，该函数在给定普通 `CustomerId` 和普通 `EmailAddress` 的情况下创建`CustomerInfo` 记录：

```F#
let createCustomer customerId email =
    { id=customerId;  email=email }
// CustomerId -> EmailAddress -> CustomerInfo
```

请注意，签名是 `CustomerId -> EmailAddress -> CustomerInfo`。

现在我们可以通过 `<!>` 和 `<*>` 使用提升技术这在上一篇文章中已经解释过：

```F#
let (<!>) = Result.map
let (<*>) = Result.apply

// applicative version
let createCustomerResultA id email =
    let idResult = createCustomerId id
    let emailResult = createEmailAddress email
    createCustomer <!> idResult <*> emailResult
// int -> string -> Result<CustomerInfo>
```

这个签名表明，我们从一个普通的 `int` 和 `string` 开始，返回一个 `Result<CustomerInfo>`

![img](https://fsharpforfunandprofit.com/posts/elevated-world-3/vgfp_applicative_style.png)

让我们用一些好的和坏的数据来尝试一下：

```F#
let goodId = 1
let badId = 0
let goodEmail = "test@example.com"
let badEmail = "example.com"

let goodCustomerA =
    createCustomerResultA goodId goodEmail
// Result<CustomerInfo> =
//   Success {id = CustomerId 1; email = EmailAddress "test@example.com";}

let badCustomerA =
    createCustomerResultA badId badEmail
// Result<CustomerInfo> =
//   Failure ["CustomerId must be positive"; "Email must contain @-sign"]
```

`goodCustomerA` 是 `Success`，包含正确的数据，但 `badCustomerA` 则是 `Failure`，包含两条验证错误消息。杰出的！

### 使用单子风格进行验证

现在让我们做另一个实现，但这次是使用单子风格。在这个版本中，逻辑将是：

- 尝试将 int 转换为 `CustomerId`
- 如果成功，请尝试将字符串转换为 `EmailAddress`
- 如果成功，请根据 customerId 和电子邮件创建 `CustomerInfo`。

代码如下：

```F#
let (>>=) x f = Result.bind f x

// monadic version
let createCustomerResultM id email =
    createCustomerId id >>= (fun customerId ->
    createEmailAddress email >>= (fun emailAddress ->
    let customer = createCustomer customerId emailAddress
    Success customer
    ))
// int -> string -> Result<CustomerInfo>
```

单子样式 `createCustomerResultM` 的签名与应用函子样式 `createCustomerResultA` 完全相同，但在内部它做了一些不同的事情，这将反映在我们得到的不同结果中。

![img](https://fsharpforfunandprofit.com/posts/elevated-world-3/vgfp_monadic_style.png)

```F#
let goodCustomerM =
    createCustomerResultM goodId goodEmail
// Result<CustomerInfo> =
//   Success {id = CustomerId 1; email = EmailAddress "test@example.com";}

let badCustomerM =
    createCustomerResultM badId badEmail
// Result<CustomerInfo> =
//   Failure ["CustomerId must be positive"]
```

在好客户的情况下，最终结果是相同的，但在坏客户的情况中，只返回一个错误，即第一个错误。`CustomerId` 创建失败后，验证的其余部分短路。

### 比较两种风格

我认为，这个例子很好地展示了应用函子风格和单子风格之间的区别。

- 应用函子示例预先进行了所有验证，然后将结果组合在一起。好处是我们没有丢失任何验证错误。不利的一面是，我们做了可能不需要做的工作。

  ![img](https://fsharpforfunandprofit.com/posts/elevated-world-3/vgfp_applicative_style.png)

- 另一方面，monadic 示例一次执行一个验证，并链接在一起。好处是，一旦发生错误，我们就会将链的其余部分短路，从而避免了额外的工作。缺点是我们只得到了第一个错误。

  ![img](https://fsharpforfunandprofit.com/posts/elevated-world-3/vgfp_monadic_style.png)

### 混合两种风格

现在没什么好说的了，我们不能混合搭配应用函子风格和单子风格。

例如，我们可能会使用应用程序样式构建 `CustomerInfo`，这样我们就不会丢失任何错误，但在程序的后期，当验证之后是数据库更新时，我们可能希望使用单子样式，这样如果验证失败，就会跳过数据库更新。

### 使用 F# 计算表达式

最后，让我们为这些 `Result` 类型构建一个计算表达式。

为此，我们只需定义一个包含名为 `Return` 和 `Bind` 的成员的类，然后创建该类的一个实例，称为 `result`，例如：

```F#
module Result =

    type ResultBuilder() =
        member this.Return x = retn x
        member this.Bind(x,f) = bind f x

    let result = new ResultBuilder()
```

然后，我们可以重写 `createCustomerResultM` 函数，使其看起来像这样：

```F#
let createCustomerResultCE id email = result {
    let! customerId = createCustomerId id
    let! emailAddress = createEmailAddress email
    let customer = createCustomer customerId emailAddress
    return customer }
```

这个计算表达式版本看起来几乎像是使用了命令式语言。

请注意，F# 计算表达式总是一元的，Haskell-do 表示法（notation）和 Scala for-推导（comprehensions）也是如此。这通常不是问题，因为如果你需要应用函子风格，在没有任何语言支持的情况下很容易编写。

## 迈向一致的世界

在实践中，我们经常需要将不同类型的值和函数组合在一起。

做到这一点的诀窍是将所有它们转换为相同的类型，之后可以很容易地组合它们。

### 使值保持一致

让我们重新审视前面的验证示例，但让我们更改记录，使其具有一个额外的属性，一个字符串类型的 `name`：

```F#
type CustomerId = CustomerId of int
type EmailAddress = EmailAddress of string

type CustomerInfo = {
    id: CustomerId
    name: string  // New!
    email: EmailAddress
    }
```

如前所述，我们希望在正常世界中创建一个函数，稍后将其提升到 `Result` 世界。

```F#
let createCustomer customerId name email =
    { id=customerId; name=name; email=email }
// CustomerId -> String -> EmailAddress -> CustomerInfo
```

现在，我们准备用额外的参数更新已提升的 `createCustomer`：

```F#
let (<!>) = Result.map
let (<*>) = Result.apply

let createCustomerResultA id name email =
    let idResult = createCustomerId id
    let emailResult = createEmailAddress email
    createCustomer <!> idResult <*> name <*> emailResult
// ERROR                            ~~~~
```

但这不会编译！在参数系列 `idResult <*> name <*> emailResult` 中，其中一个参数与其他参数不同。问题是 `idResult` 和 `emailResult` 都是 Results，但 `name` 仍然是字符串。

修复方法只是通过使用 `return` 将 `name` 提升到结果的世界中（比如 `nameResult`），对于 `Result` 来说，`return` 就是 `Success`。以下是该函数的正确版本：

```F#
let createCustomerResultA id name email =
    let idResult = createCustomerId id
    let emailResult = createEmailAddress email
    let nameResult = Success name  // lift name to Result
    createCustomer <!> idResult <*> nameResult <*> emailResult
```

### 使函数保持一致

同样的技巧也可以用于函数。

例如，假设我们有一个简单的客户更新工作流，包括四个步骤：

- 首先，我们验证输入。其输出与我们上面创建的 `Result` 类型相同。请注意，此验证函数本身可能是使用 `apply` 组合其他较小验证函数的结果。
- 接下来，我们将数据规范化（canonicalize）。例如：降低电子邮件的大小、修剪空格等。这一步永远不会引发错误。
- 接下来，我们从数据库中获取现有记录。例如，获取 `CustomerId` 的客户。此步骤也可能失败并出现错误。
- 最后，我们更新数据库。这一步是一个“死胡同”功能——没有输出。

对于错误处理，我想有两条轨道：成功轨道和失败轨道。在这个模型中，误差生成函数类似于铁路道岔（railway switch）（美国）或道岔（points）（英国）。

![img](https://fsharpforfunandprofit.com/posts/elevated-world-3/vgfp_rop_before.png)

问题是这些函数不能粘在一起；它们都是不同的形状。

解决方案是将所有这些转换为相同的形状，在这种情况下，是双轨模型，成功和失败在不同的轨道上。让我们称之为双轨世界！

### 使用工具集转换函数

那么，每个原始函数都需要提升到双轨世界，我们只知道可以做到这一点的工具！

`Canonicalize`（规范化）函数是一个单轨函数。我们可以使用 `map` 将其转换为双轨函数。

![img](https://fsharpforfunandprofit.com/posts/elevated-world-3/vgfp_rop_map.png)

`DbFetch` 函数是一个跨世界函数。我们可以使用 `bind` 将其转换为完全双轨函数。

![img](https://fsharpforfunandprofit.com/posts/elevated-world-3/vgfp_rop_bind.png)

`DbUpdate` 函数更复杂。我们不喜欢死胡同函数，所以首先我们需要将其转换为数据持续流动的函数。我把这个函数称为 `tee`。`tee` 的输出有一个输入轨迹和一个输出轨迹，因此我们需要再次使用 `map` 将其转换为双轨函数。

![img](https://fsharpforfunandprofit.com/posts/elevated-world-3/vgfp_rop_tee.png)

经过所有这些转换，我们可以重新组装这些函数的新版本。结果如下：

![img](https://fsharpforfunandprofit.com/posts/elevated-world-3/vgfp_rop_after.png)

当然，这些函数现在可以很容易地组合在一起，所以我们最终得到一个这样的函数，只有一个输入和一个成功/失败的输出：

![img](https://fsharpforfunandprofit.com/posts/elevated-world-3/vgfp_rop_after2.png)

这个组合函数是另一个形式为 `a -> Result<b>` 的世界交叉函数，因此它反过来可以用作更大函数的组成部分。

有关这种“将所有内容提升到同一个世界”方法的更多示例，请参阅我关于函数错误处理和线程状态的帖子。

## Kleisli 世界

有一个替代世界可以作为一致性的基础，我称之为“Kleisli”世界，以数学家 Kleisli 教授的名字命名！

在克莱斯利（Kleisli）的世界里，一切都是跨世界的函数！或者，用铁路轨道的类比，一切都是一个开关（或点）。

在 Kleisli 世界中，跨世界函数可以直接组合，使用一个名为 `>=>` 的运算符进行从左到右的组合，或使用 `<=<` 的运算符进行由右到左的组合。

![img](https://fsharpforfunandprofit.com/posts/elevated-world-3/vgfp_kleisli_3.png)

使用与之前相同的示例，我们可以将所有函数提升到 Kleisli 世界。

- `Validate` 和 `DbFetch` 函数已经具有正确的形式，因此不需要更改。

- 只需将输出提升到双轨值，即可将单轨 `Canonicalize` 函数提升到开关。让我们称之为 `toSwitch`。

  ![img](https://fsharpforfunandprofit.com/posts/elevated-world-3/vgfp_kleisli_1.png)

- tee-d `DbUpdate` 功能也可以通过在 tee 后执行 `toSwitch` 来提升到开关。

  ![img](https://fsharpforfunandprofit.com/posts/elevated-world-3/vgfp_kleisli_2.png)


一旦所有功能都提升到 Kleisli 世界，它们就可以用 Kleisli 组合来组合：

![img](https://fsharpforfunandprofit.com/posts/elevated-world-3/vgfp_kleisli_4.png)

Kleisli world 有一些 Two Track world 没有的好特性，但另一方面，我发现很难理解它！所以我通常坚持使用双轨世界作为我做这类事情的基础。

## 摘要

在这篇文章中，我们了解了“应用函子性”与“单子性”风格，以及为什么选择会对执行哪些操作以及返回什么结果产生重要影响。

我们还看到了如何将不同种类的值和函数提升到一个一致的世界，这样我们的函数就可以更容易地组合。

在下一篇文章中，我们将探讨一个常见问题：使用提升值列表。

# 4 理解遍历和序列

*Part of the "Map and Bind and Apply, Oh my!" series (*[link](https://fsharpforfunandprofit.com/posts/elevated-world-4/#series-toc)*)*

混合列表和提升值
05八月2015 这篇文章是超过3年

https://fsharpforfunandprofit.com/posts/elevated-world-4/

这篇文章是一系列文章中的一篇。在前两篇文章中，我描述了处理泛型数据类型的一些核心函数：`map`、`bind` 等。在上一篇文章中我讨论了“应用函子性”与“单子性”风格，以及如何提升值和函数的一致性。

在这篇文章中，我们将研究一个常见的问题：使用提升值列表。

## 系列内容

以下是本系列中提到的各种功能的快捷方式列表：

- **第 1 部分：提升到更高的世界**
  - `map` 函数
  - `return` 函数
  - `apply` 函数
  - `liftN` 系列函数
  - `zip` 函数和 ZipList 世界
- **第 2 部分：如何构建跨世界函数**
  - `bind` 函数
  - List 不是单子。Option 不是单子。
- **第 3 部分：在实践中使用核心函数**
  - 独立和依赖数据
  - 示例：使用应用函子风格和单子风格进行验证
  - 迈向一致的世界
  - Kleisli 世界
- **第 4 部分：混合列表和提升值**
  - 混合列表和提升值
  - `traverse` / `MapM` 函数
  - `sequence` 函数
  - “序列”作为ad-hoc实现的配方
  - 可读性与性能
  - 老兄，我的 `filter` 在哪里？
- **第 5 部分：使用所有技术的真实世界示例**
  - 示例：下载和处理网站列表
  - 将两个世界视为一体
- **第 6 部分：设计你自己的提升世界**
  - 设计你自己的提升世界
  - 过滤掉故障
  - Reader monad
- **第 7 部分：总结**
  - 提到的操作符名单
  - 进一步阅读

## 第 4 部分：混合列表和提升值

一个常见的问题是如何处理列表或其他高值集合。

以下是一些示例：

- **示例 1**：我们有一个 `parseInt`，它具有签名 `string -> int option`，并且我们有一系列字符串。我们想一次解析所有字符串。当然，现在我们可以使用 `map` 将字符串列表转换为选项列表。但我们真正想要的不是一个“选项列表”，而是一个“列表选项”，一个解析的整数列表，在任何失败的情况下都被包裹在一个选项中。
- **示例 2**：我们有一个 `readCustomerFromDb` 函数，其签名为 `CustomerId -> Result<Customer>`，如果可以找到并返回记录，则返回 `Success`，否则返回 `Failure`。假设我们有一个 `CustomerId` 列表，我们想一次读取所有客户。同样，我们可以使用 `map` 将 id 列表转换为结果列表。但我们真正想要的不是一个 `Result<Customer>` 列表，而是一个包含 `Customer list` 的 `Result`，并在出现错误时显示 `Failure` 案例。
- **示例 3**：我们有一个带有签名 `Uri -> Async<string>` 的 `fetchWebPage` 函数，它将返回一个按需下载页面内容的任务。假设我们有一个 `Uris`s 的列表，我们想一次获取所有页面。同样，我们可以使用 `map` 将 `Uri`s 列表转换为 `Async`s 列表。但我们真正想要的不是 `Async` 列表，而是包含字符串列表的 `Async`。

### 映射选项生成函数

让我们首先为第一种情况提出一个解决方案，然后看看我们是否可以将其推广到其他情况。

显而易见的方法是：

- 首先，使用 `map` 将 `string` 列表转换为 `Option<int>` 列表。
- 接下来，创建一个函数，将 `Option<int>` 的列表转换为 `Option<int list>`。

但这需要两次通过列表。我们能一次性完成吗？

对！如果我们考虑一下列表是如何构建的，有一个“cons”函数（F# 中的 `::`）用于将头部连接到尾部。如果我们将其提升到 `Option` 世界，我们可以使用 `Option.apply` 使用 `cons` 的提升版本将 head `Option` 连接到 tail `Option`。

```F#
let (<*>) = Option.apply
let retn = Some

let rec mapOption f list =
    let cons head tail = head :: tail
    match list with
    | [] ->
        retn []
    | head::tail ->
        retn cons <*> (f head) <*> (mapOption f tail)
```

*注意：我明确定义了 `cons`，因为 `::` 不是函数和 `List.Cons` 接受一个元组，因此在此上下文中不可用。*

以下是实现示意图：

![img](https://fsharpforfunandprofit.com/posts/elevated-world-4/vgfp_mapOption.png)

如果你对这是如何工作的感到困惑，请阅读本系列第一篇文章中关于 `apply` 的部分。

还要注意，我明确地定义了 `retn` 并在实现中使用它，而不仅仅是使用 `Some`。您将在下一节中看到原因。

现在让我们来测试一下！

```F#
let parseInt str =
    match (System.Int32.TryParse str) with
    | true,i -> Some i
    | false,_ -> None
// string -> int option

let good = ["1";"2";"3"] |> mapOption parseInt
// Some [1; 2; 3]

let bad = ["1";"x";"y"] |> mapOption parseInt
// None
```

我们首先定义 `string->int option` 类型的 `parseInt`（基于现有的 .NET 库）。

我们使用 `mapOption` 对一系列好值运行它，我们得到 `Some[1; 2; 3]`，列表在选项内，正如我们所希望的那样。

如果我们使用一个列表，其中一些值是坏的，那么整个结果将为 `None`。

### 映射结果生成函数

让我们重复一下，但这次使用前面验证示例中的 `Result` 类型。

这是 `mapResult` 函数：

```F#
let (<*>) = Result.apply
let retn = Success

let rec mapResult f list =
    let cons head tail = head :: tail
    match list with
    | [] ->
        retn []
    | head::tail ->
        retn cons <*> (f head) <*> (mapResult f tail)
```

我再次明确地定义了 `retn`，而不仅仅是使用 `Success`。因此，`mapResult` 和 `mapOption` 的代码体完全相同！

现在让我们更改 `parseInt` 以返回 `Result` 而不是 `Option`：

```F#
let parseInt str =
    match (System.Int32.TryParse str) with
    | true,i -> Success i
    | false,_ -> Failure [str + " is not an int"]
```

然后我们可以再次运行测试，但这次在失败的情况下会出现更多信息错误：

```F#
let good = ["1";"2";"3"] |> mapResult parseInt
// Success [1; 2; 3]

let bad = ["1";"x";"y"] |> mapResult parseInt
// Failure ["x is not an int"; "y is not an int"]
```

### 我们可以创建一个泛型的 mapXXX 函数吗？

`mapOption` 和 `mapResult` 的实现具有完全相同的代码，唯一的区别是 `retn` 和 `<*>` 函数不同（分别来自 Option 和 Result）。

因此，问题自然会出现，与其为每个提升类型设置 `mapResult`、`mapOption` 和其他特定实现，我们能否制作一个适用于所有提升类型的完全通用的 `mapXXX` 版本？

显而易见的是，可以将这两个函数作为额外参数传递，如下所示：

```F#
let rec mapE (retn,ap) f list =
    let cons head tail = head :: tail
    let (<*>) = ap

    match list with
    | [] ->
        retn []
    | head::tail ->
        (retn cons) <*> (f head) <*> (mapE retn ap f tail)
```

不过，这也有一些问题。首先，这段代码不能用 F# 编译！但即使这样，我们也要确保在所有地方传递相同的两个参数。

我们可以尝试创建一个包含两个参数的记录结构，然后为每种类型的提升世界创建一个实例：

```F#
type Applicative<'a,'b> = {
    retn: 'a -> E<'a>
    apply: E<'a->'b> -> E<'a> -> E<'b>
    }

// functions for applying Option
let applOption = {retn = Option.Some; apply=Option.apply}

// functions for applying Result
let applResult = {retn = Result.Success; apply=Result.apply}
```

`Applictive` 记录（比如 `appl`）的实例将是我们通用 `mapE` 函数的一个额外参数，如下所示：

```F#
let rec mapE appl f list =
    let cons head tail = head :: tail
    let (<*>) = appl.apply
    let retn = appl.retn

    match list with
    | [] ->
        retn []
    | head::tail ->
        (retn cons) <*> (f head) <*> (mapE retn ap f tail)
```

在使用中，我们会传入我们想要的特定应用函子实例，如下所示：

```F#
// build an Option specific version...
let mapOption = mapE applOption

// ...and use it
let good = ["1";"2";"3"] |> mapOption parseInt
```

不幸的是，这些都不起作用，至少在 F# 中是这样。定义的 `Applictive` 类型将无法编译。这是因为 F# 不支持“更高级的类型”。也就是说，我们不能用泛型类型参数化 `Applictive` 类型，只能用具体类型。

在 Haskell 和支持“高级类类型（higher-kinded types）”的语言中，我们定义的 `Applicative` 类型类似于“类型类（type class）”。更重要的是，对于类型类，我们不必显式地传递函数——编译器会为我们做这件事。

实际上，在 F# 中，有一种聪明（和黑客（hacky））的方法可以通过使用静态类型约束来获得相同的效果。我不打算在这里讨论它，但你可以在 FSharpx 库中看到它的使用。

所有这些抽象的替代方案是为我们想要使用的每个提升世界创建一个 `mapXXX` 函数：`mapOption`、`mapResult`、`mapAsync` 等。

就我个人而言，我接受这种更粗糙的方法。你经常与之合作的高级世界并不多，即使你失去了抽象，你也会获得明确性，这在混合能力的团队中工作时通常很有用。

让我们来看看这些 `mapXXX` 函数，也称为 `traverse`。

## `traverse`/`mapM` 函数

**常用名称**：`mapM`、`traverse`、`for`

**常用运算符**：无

**它的作用**：将跨世界函数转换为与集合一起工作的跨世界函数

**签名**： `(a->E<b>) -> a list -> E<b list>` （或列表被其他集合类型替换的变体）

### 说明

我们在上面看到，我们可以定义一组 `mapXXX` 函数，其中 XXX 代表一个应用函子世界——一个有 `apply` 和 `return` 的世界。这些 `mapXXX` 函数中的每一个都将跨世界函数转换为与集合一起工作的跨世界函数。

![img](https://fsharpforfunandprofit.com/posts/elevated-world-4/vgfp_traverse.png)

正如我们上面提到的，如果语言支持类型类，我们可以使用一个单独的实现，称为 `mapM` 或 `traverse`。从现在开始，我将把一般概念称为 `traverse`，以明确它与 `map` 不同。

### Map vs. Traverse

理解 `map` 和 `traverse` 之间的区别可能很难，所以让我们看看是否可以用图片来解释它。

首先，让我们用一个“提升”世界坐在“正常”世界之上的类比来介绍一些视觉符号。

其中一些提升的世界（事实上几乎所有的世界！）都有 `apply` 和 `return` 函数。我们称之为“应用函子世界（Applicative worlds）”。示例包括 `Option`、`Result`、`Async` 等。

其中一些提升世界具有 `traverse` 函数。我们将这些称为“可遍历世界（Traversable worlds）”，并以 `List` 为例。

如果遍历世界位于顶部，则会生成一个类型，如 `List<a>`，如果应用世界位于顶部则会生成类型，如 `Result<a>`。

![img](https://fsharpforfunandprofit.com/posts/elevated-world-4/vgfp_mstack_1.png)

*重要提示：为了与 `Result<_>` 等保持一致，我将使用语法 `List<_>` 来表示“List world”。这不意味着与 .NET 列表类相同！在 F# 中，这将通过不可变 `list` 类型来实现。*

但从现在开始，我们将在同一个“堆栈”中处理这两种提升的世界。

遍历世界可以堆叠在应用函子世界的顶部，这会产生一个类型，如 `List<Result<a>>`，或者，应用函子世界也可以堆叠在遍历世界的顶部，产生一个类型，如 `Result<List<a>>`。

![img](https://fsharpforfunandprofit.com/posts/elevated-world-4/vgfp_mstack_2.png)

现在让我们看看使用这种符号的不同类型的函数是什么样子的。

让我们从一个简单的跨世界函数开始，如 `a -> Result<b>`，其中目标世界是一个应用函子世界。在图中，输入是一个正常世界（在左边），输出（在右边）是一个叠加在正常世界之上的应用世界。



现在，如果我们有一个普通的 `a` 值列表，然后我们使用 `map` 使用像 `a -> Result<b>` 这样的函数来转换每个 `a` 值，结果也将是一个列表，但其中的内容是 `Result<b>` values 而不是 `a` 值。



当涉及到 `traverse` 时，效果会大不相同。如果我们使用 `traverse` 来转换使用该函数的 `a` 值列表，则输出将是一个 `Result`，而不是一个列表。`Result` 的内容将是 `List<b>`。

![img](https://fsharpforfunandprofit.com/posts/elevated-world-4/vgfp_traverse_traverse.png)

换言之，使用 `traverse` 时，`List` 将保持附加到正常世界，而应用函子世界（如 `Result`）将添加到顶部。

好吧，我知道这听起来很抽象，但实际上这是一种非常有用的技术。我们将在下面的实践中看到一个例子。

###  `traverse` 的应用函子版本 vs 单子版本

事实证明，`traverse` 可以以应用函子风格或单子风格实现，因此通常有两种单独的实现可供选择。应用程序版本往往以 `A` 结尾，而单子版本以 `M` 结尾，这很有帮助！

让我们看看这是如何与我们可靠的 `Result` 类型一起工作的。

首先，我们将使用应用程序和一元方法来实现 `traverseResult`。

```F#
module List =

    /// Map a Result producing function over a list to get a new Result
    /// using applicative style
    /// ('a -> Result<'b>) -> 'a list -> Result<'b list>
    let rec traverseResultA f list =

        // define the applicative functions
        let (<*>) = Result.apply
        let retn = Result.Success

        // define a "cons" function
        let cons head tail = head :: tail

        // loop through the list
        match list with
        | [] ->
            // if empty, lift [] to a Result
            retn []
        | head::tail ->
            // otherwise lift the head to a Result using f
            // and cons it with the lifted version of the remaining list
            retn cons <*> (f head) <*> (traverseResultA f tail)


    /// Map a Result producing function over a list to get a new Result
    /// using monadic style
    /// ('a -> Result<'b>) -> 'a list -> Result<'b list>
    let rec traverseResultM f list =

        // define the monadic functions
        let (>>=) x f = Result.bind f x
        let retn = Result.Success

        // define a "cons" function
        let cons head tail = head :: tail

        // loop through the list
        match list with
        | [] ->
            // if empty, lift [] to a Result
            retn []
        | head::tail ->
            // otherwise lift the head to a Result using f
            // then lift the tail to a Result using traverse
            // then cons the head and tail and return it
            f head                 >>= (fun h ->
            traverseResultM f tail >>= (fun t ->
            retn (cons h t) ))
```

应用函子版本与我们之前使用的实现相同。

monadic 版本将函数 `f` 应用于第一个元素，然后将其传递给 `bind`。与 monadic 样式一样，如果结果不好，列表的其余部分将被跳过。

另一方面，如果结果良好，则处理列表中的下一个元素，依此类推。然后将结果再次合并在一起。

*注意：这些实现仅用于演示！这两个实现都不是尾部递归的，因此它们在大列表上会失败！*

好吧，让我们测试这两个函数，看看它们有什么不同。首先，我们需要 `parseInt` 函数：

```F#
/// parse an int and return a Result
/// string -> Result<int>
let parseInt str =
    match (System.Int32.TryParse str) with
    | true,i -> Result.Success i
    | false,_ -> Result.Failure [str + " is not an int"]
```

现在，如果我们传入一个好值列表（都是可解析的），两个实现的结果是相同的。

```F#
// pass in strings wrapped in a List
// (applicative version)
let goodA = ["1"; "2"; "3"] |> List.traverseResultA parseInt
// get back a Result containing a list of ints
// Success [1; 2; 3]

// pass in strings wrapped in a List
// (monadic version)
let goodM = ["1"; "2"; "3"] |> List.traverseResultM parseInt
// get back a Result containing a list of ints
// Success [1; 2; 3]
```

但是，如果我们传入一个包含一些错误值的列表，结果就会不同。

```F#
// pass in strings wrapped in a List
// (applicative version)
let badA = ["1"; "x"; "y"] |> List.traverseResultA parseInt
// get back a Result containing a list of ints
// Failure ["x is not an int"; "y is not an int"]

// pass in strings wrapped in a List
// (monadic version)
let badM = ["1"; "x"; "y"] |> List.traverseResultM parseInt
// get back a Result containing a list of ints
// Failure ["x is not an int"]
```

应用函子版本返回所有错误，而单子版本仅返回第一个错误。

### 使用 `fold` 实现 `traverse`

我上面提到过，“从头开始”的实现不是尾部递归的，对于大列表来说会失败。当然，这是可以解决的，但代价是代码变得更加复杂。

另一方面，如果你的集合类型有一个“右折叠”函数，就像 `List` 一样，那么你也可以使用它来使实现更简单、更快、更安全。

事实上，只要有可能，我总是喜欢使用 `fold` 及其同类，这样我就永远不必担心尾部递归是正确的！

因此，以下是使用 `List.foldBack` 对 `traverseResult` 的重新实现。我尽可能地保持代码的相似性，但将列表的循环委托给 fold 函数，而不是创建递归函数。

```F#
/// Map a Result producing function over a list to get a new Result
/// using applicative style
/// ('a -> Result<'b>) -> 'a list -> Result<'b list>
let traverseResultA f list =

    // define the applicative functions
    let (<*>) = Result.apply
    let retn = Result.Success

    // define a "cons" function
    let cons head tail = head :: tail

    // right fold over the list
    let initState = retn []
    let folder head tail =
        retn cons <*> (f head) <*> tail

    List.foldBack folder list initState

/// Map a Result producing function over a list to get a new Result
/// using monadic style
/// ('a -> Result<'b>) -> 'a list -> Result<'b list>
let traverseResultM f list =

    // define the monadic functions
    let (>>=) x f = Result.bind f x
    let retn = Result.Success

    // define a "cons" function
    let cons head tail = head :: tail

    // right fold over the list
    let initState = retn []
    let folder head tail =
        f head >>= (fun h ->
        tail >>= (fun t ->
        retn (cons h t) ))

    List.foldBack folder list initState
```

请注意，这种方法并不适用于所有集合类。某些类型没有正确的折叠，因此必须以不同的方式实现 `traverse`。

### 除了列表之外的其他类型呢？

所有这些示例都使用 `list` 类型作为集合类型。我们也可以为其他类型实现 `traverse` 吗？

对。例如，一个 `Option` 可以被视为一个单元素列表，我们可以使用同样的技巧。

例如，这是一个用于 `Option` 的 `traverseResultA` 的实现

```F#
module Option =

    /// Map a Result producing function over an Option to get a new Result
    /// ('a -> Result<'b>) -> 'a option -> Result<'b option>
    let traverseResultA f opt =

        // define the applicative functions
        let (<*>) = Result.apply
        let retn = Result.Success

        // loop through the option
        match opt with
        | None ->
            // if empty, lift None to an Result
            retn None
        | Some x ->
            // lift value to an Result
            (retn Some) <*> (f x)
```

现在我们可以将字符串包装在 `Option` 中，并对其使用 `parseInt`。我们不是得到 `Option` of `Result`，而是反转堆栈并得到 `Result` of `Option`。

```F#
// pass in an string wrapped in an Option
let good = Some "1" |> Option.traverseResultA parseInt
// get back a Result containing an Option
// Success (Some 1)
```

如果我们传入一个不可解析的字符串，我们会得到失败：

```F#
// pass in an string wrapped in an Option
let bad = Some "x" |> Option.traverseResultA parseInt
// get back a Result containing an Option
// Failure ["x is not an int"]
```

如果我们通过 `None`，我们将获得包含 `None` 的 `Success`！

```F#
// pass in an string wrapped in an Option
let goodNone = None |> Option.traverseResultA parseInt
// get back a Result containing an Option
// Success (None)
```

乍一看，最后一个结果可能令人惊讶，但这样想，解析并没有失败，所以根本没有失败。

### 可遍历的（Traversables）

可以实现 `mapXXX` 或 `traverseXXX` 等函数的类型称为 Traversable。例如，集合类型包括遍历对象和其他一些类型。

如上所述，在具有类型类的语言中，Traversable 类型只需要一个 `traverse` 实现就可以了，但在没有类型类的编程语言中，每个提升的类型都需要一个 Traversable 实现。

还要注意，与我们之前创建的所有泛型函数不同，要实现遍历，（在集合内）作用的类型必须具有适当的 `apply` 和 `return` 函数。也就是说，内部类型必须是应用函子型（Applicative）。

### 正确 `traverse` 实现的属性

与往常一样，`traverse` 的正确实现应该具有一些真实的属性，无论我们使用的是什么提升世界。

这些是“遍历定律”，**可遍历（Traversable）**被定义为一个通用的数据类型构造函数——`E<T>`——加上一组遵守这些定律的函数（`traverse` 或 `traverseXXX`）。

这些定律与之前的定律相似。例如，应正确映射恒等函数，应保留组合等。

## `sequence` 函数

**通用名称**：`sequence`

**常用运算符**：无

**它的作用**：将提升值列表转换为包含列表的提升值

**签名**： `E<a> list -> E<a list>` （或列表被其他集合类型替换的变体）

### 说明

我们在上面看到了当您有一个生成应用函子类型（如 `Result`）的函数时，如何使用 `traverse` 函数来替代 `map`。

但是，如果你只收到一个 `List<Result>`，并且需要将其更改为 `Result<List>`，会发生什么。也就是说，你需要交换堆栈上世界的顺序：

![img](https://fsharpforfunandprofit.com/posts/elevated-world-4/vgfp_sequence_stack.png)

这就是 `sequence` 有用的地方——这正是它的作用！`sequence` 函数“交换层”。

交换顺序是固定的：

- Traversable 世界从更高的地方开始，然后向下交换。
- 应用函子世界从较低的位置开始，然后被交换。

请注意，若您已经实现了 `traverse`，则可以轻松地从中导出 `sequence`。事实上，您可以将序列视为内置 `id` 函数的遍历。

### `sequence` 的应用函子版本 vs 单子版本

就像 `traverse` 一样，`sequence` 也可以有应用函子性和单子性版本：

- `sequenceA` 代表应用函子版。
- `sequenceM`（或简称 `sequence`）表示单子版。

### 一个简单的例子

让我们实现并测试 `Result` 的 `sequence` 实现：

```F#
module List =

    /// Transform a "list<Result>" into a "Result<list>"
    /// and collect the results using apply
    /// Result<'a> list -> Result<'a list>
    let sequenceResultA x = traverseResultA id x

    /// Transform a "list<Result>" into a "Result<list>"
    /// and collect the results using bind.
    /// Result<'a> list -> Result<'a list>
    let sequenceResultM x = traverseResultM id x
```

好吧，这太容易了！现在让我们测试一下，从应用函子版本开始：

```F#
let goodSequenceA =
    ["1"; "2"; "3"]
    |> List.map parseInt
    |> List.sequenceResultA
// Success [1; 2; 3]

let badSequenceA =
    ["1"; "x"; "y"]
    |> List.map parseInt
    |> List.sequenceResultA
// Failure ["x is not an int"; "y is not an int"]
```

然后是单子版本：

```F#
let goodSequenceM =
    ["1"; "2"; "3"]
    |> List.map parseInt
    |> List.sequenceResultM
// Success [1; 2; 3]

let badSequenceM =
    ["1"; "x"; "y"]
    |> List.map parseInt
    |> List.sequenceResultM
// Failure ["x is not an int"]
```

如前所述，我们返回一个 `Result<List>`，如前所示，单子版本在第一个错误时停止，而应用函子版本则累积所有错误。

## “序列”作为 ad-hoc 实现的配方

我们在上面看到，拥有像 Applictive 这样的类型类意味着您只需要实现一次 `traverse` 和 `sequence`。在 F# 和其他没有高级类类型的语言中，您必须为要遍历的每种类型创建一个实现。

这是否意味着 `traverse` 和 `sequence` 的概念无关紧要或过于抽象？我不这么认为。

与其把它们看作库函数，我发现把它们看作配方是有用的——一组你可以机械地遵循以解决特定问题的指令。

在许多情况下，问题是特定上下文特有的，不需要创建库函数——您可以根据需要创建辅助函数。

让我举个例子来演示。假设你得到一个选项列表，其中每个选项都包含一个元组，如下所示：

```F#
let tuples = [Some (1,2); Some (3,4); None; Some (7,8);]
// List<Option<Tuple<int>>>
```

此数据的格式为 `List<Option<Tuple<int>>>`。现在说，出于某种原因，你需要将其转换为两个列表的元组，其中每个列表都包含选项，如下所示：

```F#
let desiredOutput = [Some 1; Some 3; None; Some 7],[Some 2; Some 4; None; Some 8]
// Tuple<List<Option<int>>>
```

所需结果的形式为 `Tuple<List<Option<int>>>`。

那么，你会如何编写一个函数来实现这一点呢？快！

毫无疑问，你可以想出一个，但这可能需要一些思考和测试来确保你做对了。

另一方面，如果你认识到这项任务只是将一堆世界转换为另一堆，你可以机械地创建一个函数，几乎不需要思考。

![img](https://fsharpforfunandprofit.com/posts/elevated-world-4/vgfp_tuple_sequence-1.png)

### 设计解决方案

为了设计解决方案，我们需要注意哪些世界向上移动，哪些世界向下移动。

- 元组世界需要位于顶部，因此必须“向上”交换，这反过来意味着它将扮演“应用函子性”的角色。
- 选项和列表世界需要“向下”交换，这反过来意味着它们都将扮演“可遍历”的角色。

为了进行这种转换，我需要两个辅助函数：

- `optionSequenceTuple` 将向下移动一个选项，向上移动一个元组。
- `listSequenceTuple` 将向下移动列表，向上移动元组。


这些辅助函数需要在库中吗？不，我不太可能再次需要它们，即使我偶尔需要它们，我也更愿意从头开始写，以避免产生依赖性。

另一方面，前面实现的 `List.sequenceResult` 函数将 `List<Result<a>>` 转换为 `Result<List<a>>` 是我经常使用的函数，因此值得集中使用。

### 实施解决方案

一旦我们知道解决方案会是什么样子，我们就可以开始机械地编码。

首先，元组扮演着应用程序的角色，所以我们需要定义 `apply` 和 `return` 函数：

```F#
let listSequenceTuple list =
    // define the applicative functions
    let (<*>) = tupleApply
    let retn = tupleReturn

    // define a "cons" function
    let cons head tail = head :: tail

    // right fold over the list
    let initState = retn []
    let folder head tail = retn cons <*> head <*> tail

    List.foldBack folder list initState
```

这里没有思考。我只是在遵循模板！

我们可以立即进行测试：

```F#
[ (1,2); (3,4)] |> listSequenceTuple
// Result => ([1; 3], [2; 4])
```

正如预期的那样，它给出了一个包含两个列表的元组。

同样，再次使用相同的右折叠模板定义 `optionSequenceTuple`。这次 `Option` 是可遍历的，元组仍然是应用函子性的：

```F#
let optionSequenceTuple opt =
    // define the applicative functions
    let (<*>) = tupleApply
    let retn = tupleReturn

    // right fold over the option
    let initState = retn None
    let folder x _ = (retn Some) <*> x

    Option.foldBack folder opt initState
```

我们也可以测试一下：

```F#
Some (1,2) |> optionSequenceTuple
// Result => (Some 1, Some 2)
```

正如预期的那样，它给出了一个有两个选项的元组。

最后，我们可以把所有零件粘在一起。再一次，不需要思考！

```F#
let convert input =
    input

    // from List<Option<Tuple<int>>> to List<Tuple<Option<int>>>
    |> List.map optionSequenceTuple

    // from List<Tuple<Option<int>>> to Tuple<List<Option<int>>>
    |> listSequenceTuple
```

如果我们使用它，我们就会得到我们想要的：

```F#
let output = convert tuples
// ( [Some 1; Some 3; None; Some 7], [Some 2; Some 4; None; Some 8] )

output = desiredOutput |> printfn "Is output correct? %b"
// Is output correct? true
```

好吧，这个解决方案比拥有一个可重用的函数要费力，但因为它是机械的，所以只需要几分钟的时间来编码，而且仍然比试图想出自己的解决方案更容易！

想要更多吗？有关在现实世界问题中使用 `sequence` 的示例，请阅读这篇文章。

## 可读性 vs. 性能

在这篇文章的开头，我注意到作为函数式程序员，我们倾向于先 `map`，然后再提问。

换句话说，给定一个像 `parseInt` 这样的 `Result` 生成函数，我们将从收集结果开始，然后才弄清楚如何处理它们。我们的代码看起来像这样，然后：

```F#
["1"; "2"; "3"]
|> List.map parseInt
|> List.sequenceResultM
```

当然，这确实涉及对列表的两次遍历，我们看到了 `traverse` 如何在一步中组合 `map` 和 `sequence`，只对列表进行一次遍历，如下所示：

```F#
["1"; "2"; "3"]
|> List.traverseResultM parseInt
```

所以，如果 `traverse` 更紧凑，可能更快，为什么还要使用 `sequence` 呢？

好吧，有时你会得到一个特定的结构，你别无选择，但在其他情况下，我可能仍然更喜欢两步 `map-sequence` 方法，因为它更容易理解。对于大多数人来说，“映射（map）”然后“交换（swap）”的心理模型似乎比一步遍历更容易掌握。

换句话说，我总是追求可读性，除非你能证明性能受到了影响。根据我的经验，许多人仍在学习 FP，过于隐晦是没有帮助的。

## 老兄，我的 `filter` 呢?

我们已经看到像 `map` 和 `sequence` 这样的函数在列表上工作，将它们转换为其他类型，但过滤呢？我如何使用这些方法过滤内容？

答案是——你不能！`map`、`traverse` 和 `sequence` 都是“结构保持”的。如果你从一个 10 件事的列表开始，那么之后你仍然有一个 10 项事的列表，尽管在堆栈的其他地方。或者，如果你从一棵有三根树枝的树开始，你在最后仍然有一棵有三枝的树。

在上面的元组示例中，原始列表有四个元素，经过转换后，元组中的两个新列表也有四个要素。

如果你需要改变一个类型的结构，你需要使用像 `fold` 这样的东西。Fold 允许您从旧结构构建新结构，这意味着您可以使用它创建一个缺少某些元素的新列表（即过滤器）。

fold 的各种用途都值得一个系列，所以我将把过滤的讨论留到另一个时间。

## 摘要

在这篇文章中，我们学习了 `traverse` 和 `sequence` 作为处理提升值列表的一种方式。

在下一篇文章中，我们将通过一个使用所有讨论过的技术的实际例子来结束。

# 5 在实践中使用 map、apply、bind 和 sequence

*Part of the "Map and Bind and Apply, Oh my!" series (*[link](https://fsharpforfunandprofit.com/posts/elevated-world-5/#series-toc)*)*

一个使用所有技术的真实世界的例子
06八月2015 这篇文章是超过3年

https://fsharpforfunandprofit.com/posts/elevated-world-5/

这篇文章是系列文章中的第五篇。在前两篇文章中，我描述了处理泛型数据类型的一些核心函数：`map`、`bind` 等。在第三篇文章里，我讨论了“应用函子性”与“单子性”风格，以及如何提升值和函数之间的一致性。在上一篇文章中，我介绍了 `traverse` 和 `sequence` 作为处理提升值列表的一种方式。

在这篇文章中，我们将通过一个实际的例子来结束，这个例子使用了到目前为止讨论过的所有技术。

## 系列内容

以下是本系列中提到的各种函数的快捷方式列表：

- **第 1 部分：提升到更高的世界**
  - `map` 函数
  - `return` 函数
  - `apply` 函数
  - `liftN` 系列函数
  - `zip` 函数和 ZipList 世界
- **第 2 部分：如何构建跨世界函数**
  - `bind` 函数
  - List 不是单子。Option 不是单子。
- **第 3 部分：在实践中使用核心函数**
  - 独立和依赖数据
  - 示例：使用应用函子风格和单子风格进行验证
  - 迈向一致的世界
  - Kleisli 世界
- **第 4 部分：混合列表和提升值**
  - 混合列表和提升值
  - `traverse` / `MapM` 函数
  - `sequence` 函数
  - “序列”作为ad-hoc实现的配方
  - 可读性与性能
  - 老兄，我的 `filter` 在哪里？
- **第 5 部分：使用所有技术的真实世界示例**
  - 示例：下载和处理网站列表
  - 将两个世界视为一体
- **第 6 部分：设计你自己的提升世界**
  - 设计你自己的提升世界
  - 过滤掉故障
  - Reader monad
- **第 7 部分：总结**
  - 提到的操作符名单
  - 进一步阅读

## 第 5 部分：使用所有技术的真实世界示例

## 示例：下载和处理网站列表

该示例将是第三篇文章开头提到的示例的变体：

- 给定一个网站列表，创建一个找到主页最大的网站的动作。

让我们把它分解成几个步骤：

首先，我们需要将 url 转换为一系列操作，每个操作都会下载页面并获取内容的大小。

然后我们需要找到最大的内容，但为了做到这一点，我们必须将操作列表转换为包含大小列表的单个操作。这就是 `traverse` 或 `sequence` 发挥作用的地方。

让我们开始吧！

### 下载器

首先，我们需要创建一个下载器。我会使用内置 `System.Net.WebClient` 类，但由于某种原因，它不允许覆盖超时。我想为以后对坏 uri 的测试留出一点时间，所以这很重要。

一个技巧是只子类化 `WebClient` 并拦截构建请求的方法。所以它是：

```F#
// define a millisecond Unit of Measure
type [<Measure>] ms

/// Custom implementation of WebClient with settable timeout
type WebClientWithTimeout(timeout:int<ms>) =
    inherit System.Net.WebClient()

    override this.GetWebRequest(address) =
        let result = base.GetWebRequest(address)
        result.Timeout <- int timeout
        result
```

请注意，我使用的是超时值的度量单位。我发现度量单位对于区分秒和毫秒是非常宝贵的。我曾经不小心将超时设置为 2000 秒而不是 2000 毫秒，我不想再犯这个错误！

下一段代码定义了我们的域类型。我们希望在处理 url 和大小时能够将它们保持在一起。我们可以使用元组，但我支持使用类型来建模你的域，即使只是用于文档。

```F#
// The content of a downloaded page
type UriContent =
    UriContent of System.Uri * string

// The content size of a downloaded page
type UriContentSize =
    UriContentSize of System.Uri * int
```

是的，对于这样一个微不足道的例子来说，这可能有点过头了，但在一个更严肃的项目中，我认为这非常值得做。

现在，我们来看一下进行下载的代码：

```F#
/// Get the contents of the page at the given Uri
/// Uri -> Async<Result<UriContent>>
let getUriContent (uri:System.Uri) =
    async {
        use client = new WebClientWithTimeout(1000<ms>) // 1 sec timeout
        try
            printfn "  [%s] Started ..." uri.Host
            let! html = client.AsyncDownloadString(uri)
            printfn "  [%s] ... finished" uri.Host
            let uriContent = UriContent (uri, html)
            return (Result.Success uriContent)
        with
        | ex ->
            printfn "  [%s] ... exception" uri.Host
            let err = sprintf "[%s] %A" uri.Host ex.Message
            return Result.Failure [err ]
        }
```

笔记：

- 这个 .NET 库会抛出各种错误，所以我正在捕捉并将其转化为 `Failure`。
- `use client =` 部分确保客户端将在块的末尾被正确处理。
- 整个操作被包裹在一个 `async` 工作流中，`let! html = client.AsyncDownloadString` 是异步下载发生的地方。
- 我添加了一些 `printfn`s 用于跟踪，仅用于此示例。在真实的代码中，我当然不会这样做！

在继续之前，让我们以交互方式测试这段代码。首先，我们需要一个助手来打印结果：

```F#
let showContentResult result =
    match result with
    | Success (UriContent (uri, html)) ->
        printfn "SUCCESS: [%s] First 100 chars: %s" uri.Host (html.Substring(0,100))
    | Failure errs ->
        printfn "FAILURE: %A" errs
```

然后我们可以在一个好的网站上尝试一下：

```F#
System.Uri ("http://google.com")
|> getUriContent
|> Async.RunSynchronously
|> showContentResult

//  [google.com] Started ...
//  [google.com] ... finished
// SUCCESS: [google.com] First 100 chars: <!doctype html><html itemscope="" itemtype="http://schema.org/WebPage" lang="en-GB"><head><meta cont
```

还有一个不好的：

```F#
System.Uri ("http://example.bad")
|> getUriContent
|> Async.RunSynchronously
|> showContentResult

//  [example.bad] Started ...
//  [example.bad] ... exception
// FAILURE: ["[example.bad] "The remote name could not be resolved: 'example.bad'""]
```

### 用 `map` 和 `apply` 和 `bind` 扩展 Async 类型

在这一点上，我们知道我们将要处理 `Async` 世界，所以在我们进一步讨论之前，让我们确保我们有四个核心功能可用：

```F#
module Async =

    let map f xAsync = async {
        // get the contents of xAsync
        let! x = xAsync
        // apply the function and lift the result
        return f x
        }

    let retn x = async {
        // lift x to an Async
        return x
        }

    let apply fAsync xAsync = async {
        // start the two asyncs in parallel
        let! fChild = Async.StartChild fAsync
        let! xChild = Async.StartChild xAsync

        // wait for the results
        let! f = fChild
        let! x = xChild

        // apply the function to the results
        return f x
        }

    let bind f xAsync = async {
        // get the contents of xAsync
        let! x = xAsync
        // apply the function but don't lift the result
        // as f will return an Async
        return! f x
        }
```

这些实现很简单：

- 我正在使用 `async` 工作流来处理异步值。
- `map` 中 `let!` 的语法从 `Async` 中提取内容（意味着运行它并等待结果）。
- `map`、`retn` 和 `apply` 中的 `return` 语法使用 `return` 将值提升到 `Async`。
- `apply` 函数使用 fork/join 模式并行运行这两个参数。如果我当初写的是 `let! fChild = ...` 接着是 `let! xChild = ...` 那将是单子的和序列的，这不是我想要的。
- `bind` 中的 `return` 语法意味着该值已经被提升，不能对其调用 `return`。

### 获取下载页面的大小

回到正轨，我们可以从下载步骤继续，继续将结果转换为 `UriContentSize` 的过程：

```F#
/// Make a UriContentSize from a UriContent
/// UriContent -> Result<UriContentSize>
let makeContentSize (UriContent (uri, html)) =
    if System.String.IsNullOrEmpty(html) then
        Result.Failure ["empty page"]
    else
        let uriContentSize = UriContentSize (uri, html.Length)
        Result.Success uriContentSize
```

如果输入的 html 为 null 或空，我们将视其为错误，否则我们将返回 `UriContentSize`。

现在我们有两个函数，我们想将它们组合成一个“给定 Uri 获取 UriContentSize”函数。问题是输出和输入不匹配：

- `getUriContent` 是 `Uri -> Async<Result<UriContent>>`
- `makeContentSize` 是 `UriContent -> Result<UriContentSize>`

答案是将 `makeContentSize` 从一个以 `UriContent` 为输入的函数转换为一个以 `Async<Result<UriContent>>` 为输入的功能。我们如何做到这一点？

首先，使用 `Result.bind` 将其从 `a -> Result<b>` 函数转换为 `Result<a> -> Result<b>` 函数。在这种情况下，`UriContent -> Result<UriContentSize>` 变为 `Result<UriContent> -> Results<UriContentSize>`。

接下来，使用 `Async.map` 将其从 `a->b` 函数转换为 `Async<a>->Async<b>` 函数。在这种情况下， `Result<UriContent> -> Result<UriContentSize>` 变为 `Async<Result<UriContent>> -> Async<Result<UriContentSize>>`。

![img](https://fsharpforfunandprofit.com/posts/elevated-world-5/vgfp_urlcontentsize.png)

现在它有了正确的输入类型，所以我们可以用 `getUriContent` 组合它：

```F#
/// Get the size of the contents of the page at the given Uri
/// Uri -> Async<Result<UriContentSize>>
let getUriContentSize uri =
    getUriContent uri
    |> Async.map (Result.bind makeContentSize)
```

那是一种粗糙的字体签名，而且只会变得更糟！在这种时候，我真的很欣赏类型推理。

让我们再测试一次。首先是一个格式化结果的助手：

```F#
let showContentSizeResult result =
    match result with
    | Success (UriContentSize (uri, len)) ->
        printfn "SUCCESS: [%s] Content size is %i" uri.Host len
    | Failure errs ->
        printfn "FAILURE: %A" errs
```

然后我们可以在一个好的网站上尝试一下：

```F#
System.Uri ("http://google.com")
|> getUriContentSize
|> Async.RunSynchronously
|> showContentSizeResult

//  [google.com] Started ...
//  [google.com] ... finished
//SUCCESS: [google.com] Content size is 44293
```

还有一个不好的：

```F#
System.Uri ("http://example.bad")
|> getUriContentSize
|> Async.RunSynchronously
|> showContentSizeResult

//  [example.bad] Started ...
//  [example.bad] ... exception
//FAILURE: ["[example.bad] "The remote name could not be resolved: 'example.bad'""]
```

### 从列表中获取最大大小

该过程的最后一步是找到最大的页面大小。

这很容易。一旦我们有了 `UriContentSize` 的列表，我们就可以使用 `list.maxBy` 轻松找到最大的一个：

```F#
/// Get the largest UriContentSize from a list
/// UriContentSize list -> UriContentSize
let maxContentSize list =

    // extract the len field from a UriContentSize
    let contentSize (UriContentSize (_, len)) = len

    // use maxBy to find the largest
    list |> List.maxBy contentSize
```

### 把它们放在一起

我们现在准备使用以下算法组装所有部件：

- 从URL列表开始

- 将字符串列表转换为Uri列表（`Uri list`）

- 将 `Uri`s 列表转换为操作列表（`Async<Result<UriContentSize>> list`）

- 接下来，我们需要交换堆栈的前两部分。也就是说，将 `List<Async>` 转换为 `Async<List>`。

  ![img](https://fsharpforfunandprofit.com/posts/elevated-world-5/vgfp_download_stack_1.png)

- 接下来，我们需要交换堆栈的底部两部分——将 `List<Result>` 转换为 `Result<List>`。但是堆栈的两个底部部分被包装在 `Async` 中，所以我们需要使用 `Async.map` 来实现这一点。

  ![img](https://fsharpforfunandprofit.com/posts/elevated-world-5/vgfp_download_stack_2.png)

- 最后，我们需要在底部的 `List` 上使用 `List.maxBy` 将其转换为单个值。也就是说，将 `List<UriContentSize>` 转换为 `UriContentSize`。但是堆栈的底部被包装在异步包装的 `Result` 中，所以我们需要使用 `Async.map` 和 `Result.map` 来实现这一点。

  ![img](https://fsharpforfunandprofit.com/posts/elevated-world-5/vgfp_download_stack_3.png)


以下是完整的代码：

```F#
/// Get the largest page size from a list of websites
let largestPageSizeA urls =
    urls
    // turn the list of strings into a list of Uris
    // (In F# v4, we can call System.Uri directly!)
    |> List.map (fun s -> System.Uri(s))

    // turn the list of Uris into a "Async<Result<UriContentSize>> list"
    |> List.map getUriContentSize

    // turn the "Async<Result<UriContentSize>> list"
    //   into an "Async<Result<UriContentSize> list>"
    |> List.sequenceAsyncA

    // turn the "Async<Result<UriContentSize> list>"
    //   into a "Async<Result<UriContentSize list>>"
    |> Async.map List.sequenceResultA

    // find the largest in the inner list to get
    //   a "Async<Result<UriContentSize>>"
    |> Async.map (Result.map maxContentSize)
```

此函数具有签名 `string list -> Async<Result<UriContentSize>>`，这正是我们想要的！

这里涉及两个 `sequence` 函数：`sequenceSyncA` 和 `sequenceResultA`。实现正如您在前面的所有讨论中所期望的那样，但我仍将显示代码：

```F#
module List =

    /// Map a Async producing function over a list to get a new Async
    /// using applicative style
    /// ('a -> Async<'b>) -> 'a list -> Async<'b list>
    let rec traverseAsyncA f list =

        // define the applicative functions
        let (<*>) = Async.apply
        let retn = Async.retn

        // define a "cons" function
        let cons head tail = head :: tail

        // right fold over the list
        let initState = retn []
        let folder head tail =
            retn cons <*> (f head) <*> tail

        List.foldBack folder list initState

    /// Transform a "list<Async>" into a "Async<list>"
    /// and collect the results using apply.
    let sequenceAsyncA x = traverseAsyncA id x

    /// Map a Result producing function over a list to get a new Result
    /// using applicative style
    /// ('a -> Result<'b>) -> 'a list -> Result<'b list>
    let rec traverseResultA f list =

        // define the applicative functions
        let (<*>) = Result.apply
        let retn = Result.Success

        // define a "cons" function
        let cons head tail = head :: tail

        // right fold over the list
        let initState = retn []
        let folder head tail =
            retn cons <*> (f head) <*> tail

        List.foldBack folder list initState

    /// Transform a "list<Result>" into a "Result<list>"
    /// and collect the results using apply.
    let sequenceResultA x = traverseResultA id x
```

### 添加计时器

看看不同场景的下载需要多长时间会很有趣，所以让我们创建一个小计时器，运行一个函数一定次数并取平均值：

```F#
/// Do countN repetitions of the function f and print the time per run
let time countN label f  =

    let stopwatch = System.Diagnostics.Stopwatch()

    // do a full GC at the start but not thereafter
    // allow garbage to collect for each iteration
    System.GC.Collect()

    printfn "======================="
    printfn "%s" label
    printfn "======================="

    let mutable totalMs = 0L

    for iteration in [1..countN] do
        stopwatch.Restart()
        f()
        stopwatch.Stop()
        printfn "#%2i elapsed:%6ims " iteration stopwatch.ElapsedMilliseconds
        totalMs <- totalMs + stopwatch.ElapsedMilliseconds

    let avgTimePerRun = totalMs / int64 countN
    printfn "%s: Average time per run:%6ims " label avgTimePerRun
```

### 终于可以下载了

让我们下载一些真正的网站！

我们将定义两个站点列表：一个是“好”的，其中所有站点都应该是可访问的，另一个是包含无效站点的“坏”的。

```F#
let goodSites = [
    "http://google.com"
    "http://bbc.co.uk"
    "http://fsharp.org"
    "http://microsoft.com"
    ]

let badSites = [
    "http://example.com/nopage"
    "http://bad.example.com"
    "http://verybad.example.com"
    "http://veryverybad.example.com"
    ]
```

让我们从运行 `largestPageSizeA` 10 次良好的网站列表开始：

```F#
let f() =
    largestPageSizeA goodSites
    |> Async.RunSynchronously
    |> showContentSizeResult
time 10 "largestPageSizeA_Good" f
```

输出如下：

```
[google.com] Started ...
[bbc.co.uk] Started ...
[fsharp.org] Started ...
[microsoft.com] Started ...
[bbc.co.uk] ... finished
[fsharp.org] ... finished
[google.com] ... finished
[microsoft.com] ... finished

SUCCESS: [bbc.co.uk] Content size is 108983
largestPageSizeA_Good: Average time per run:   533ms
```

我们可以立即看到下载是并行进行的——它们在第一个下载完成之前就已经开始了。

如果有些网站不好怎么办？

```F#
let f() =
    largestPageSizeA badSites
    |> Async.RunSynchronously
    |> showContentSizeResult
time 10 "largestPageSizeA_Bad" f
```

输出如下：

```
[example.com] Started ...
[bad.example.com] Started ...
[verybad.example.com] Started ...
[veryverybad.example.com] Started ...
[verybad.example.com] ... exception
[veryverybad.example.com] ... exception
[example.com] ... exception
[bad.example.com] ... exception

FAILURE: [
 "[example.com] "The remote server returned an error: (404) Not Found."";
 "[bad.example.com] "The remote name could not be resolved: 'bad.example.com'"";
 "[verybad.example.com] "The remote name could not be resolved: 'verybad.example.com'"";
 "[veryverybad.example.com] "The remote name could not be resolved: 'veryverybad.example.com'""]

largestPageSizeA_Bad: Average time per run:  2252ms
```

同样，所有下载都是并行进行的，并且返回了所有四个失败。

### 优化

`largePageSizeA` 中有一系列映射和序列，这意味着列表被迭代了三次，异步映射了两次。

正如我之前所说的，除非有其他证据，否则我更喜欢清晰度而不是微观优化，所以这并不困扰我。

但是，让我们看看如果你愿意，你可以做些什么。

这是原始版本，删除了评论：

```F#
let largestPageSizeA urls =
    urls
    |> List.map (fun s -> System.Uri(s))
    |> List.map getUriContentSize
    |> List.sequenceAsyncA
    |> Async.map List.sequenceResultA
    |> Async.map (Result.map maxContentSize)
```

前两个 `List.map`s 可以合并：

```F#
let largestPageSizeA urls =
    urls
    |> List.map (fun s -> System.Uri(s) |> getUriContentSize)
    |> List.sequenceAsyncA
    |> Async.map List.sequenceResultA
    |> Async.map (Result.map maxContentSize)
```

`map-sequence` 可以用 `traverse` 代替：

```F#
let largestPageSizeA urls =
    urls
    |> List.traverseAsyncA (fun s -> System.Uri(s) |> getUriContentSize)
    |> Async.map List.sequenceResultA
    |> Async.map (Result.map maxContentSize)
```

最后两个 `Async.map`s 也可以被组合：

```F#
let largestPageSizeA urls =
    urls
    |> List.traverseAsyncA (fun s -> System.Uri(s) |> getUriContentSize)
    |> Async.map (List.sequenceResultA >> Result.map maxContentSize)
```

就我个人而言，我认为我们在这里走得太远了。比起这个，我更喜欢原版！

顺便说一句，两全其美的一种方法是使用一个“流”库，它会自动为您合并映射。在 F# 中，一个很好的例子是Nessos Streams。这是一篇博客文章，展示了流和标准 `seq` 之间的区别。

### 单子方式下载

让我们使用一元风格重新实现下载逻辑，看看它有什么不同。

首先，我们需要一个下载器的一元版本：

```F#
let largestPageSizeM urls =
    urls
    |> List.map (fun s -> System.Uri(s))
    |> List.map getUriContentSize
    |> List.sequenceAsyncM              // <= "M" version
    |> Async.map List.sequenceResultM   // <= "M" version
    |> Async.map (Result.map maxContentSize)
```

这个使用了单子 `sequence` 函数（我不会显示它们——实现正如你所期望的那样）。

让我们用好的站点列表运行 `largestSizeM` 10 次，看看与应用程序版本是否有任何区别：

```F#
let f() =
    largestPageSizeM goodSites
    |> Async.RunSynchronously
    |> showContentSizeResult
time 10 "largestPageSizeM_Good" f
```

输出如下：

```
  [google.com] Started ...
  [google.com] ... finished
  [bbc.co.uk] Started ...
  [bbc.co.uk] ... finished
  [fsharp.org] Started ...
  [fsharp.org] ... finished
  [microsoft.com] Started ...
  [microsoft.com] ... finished

SUCCESS: [bbc.co.uk] Content size is 108695
largestPageSizeM_Good: Average time per run:   955ms
```

现在有一个很大的区别——很明显，下载是按顺序进行的——每次下载都是在前一次下载完成后才开始的。

因此，每次运行的平均时间为 955ms，几乎是应用函子版本的两倍。

如果有些网站不好怎么办？我们应该期待什么？好吧，因为它是单子的，我们应该预料到，在第一个错误之后，其余的站点会被跳过，对吧？让我们看看是否会发生这种情况！

```F#
let f() =
    largestPageSizeM badSites
    |> Async.RunSynchronously
    |> showContentSizeResult
time 10 "largestPageSizeM_Bad" f
```

输出如下：

```
[example.com] Started ...
[example.com] ... exception
[bad.example.com] Started ...
[bad.example.com] ... exception
[verybad.example.com] Started ...
[verybad.example.com] ... exception
[veryverybad.example.com] Started ...
[veryverybad.example.com] ... exception

FAILURE: ["[example.com] "The remote server returned an error: (404) Not Found.""]
largestPageSizeM_Bad: Average time per run:  2371ms
```

这真是出乎意料！所有网站都是按顺序访问的，尽管第一个网站有错误。但在这种情况下，为什么只返回第一个错误，而不是所有错误？

你能看出哪里出了问题吗？

### 解释问题

实现没有按预期工作的原因是 `Async`s 的链接独立于 `Result`s 的链接。

如果你在调试器中逐步执行此操作，你可以看到发生了什么：

- 列表中的第一个 `Async` 已运行，导致失败。
- `Async.bind` 与列表中的下一个 `Async` 一起使用。但是 `Async.bind` 没有错误的概念，因此运行了下一个 `Async`，产生了另一个失败。
- 通过这种方式，所有 `Async`s 都运行了，产生了一个故障列表。
- 然后使用 `Result.bind` 遍历此失败列表。当然，由于绑定，只有第一个被处理，其余的被忽略。
- 最终的结果是，所有 `Async`s 都运行了，但只返回了第一个失败。

## 将两个世界视为一体

根本问题是，我们将 `Async` 列表和 `Result` 列表视为需要遍历的独立对象。但这意味着失败的 `Result` 不会影响是否运行下一个 `Async`。

那么，我们想做的是将它们联系在一起，这样糟糕的结果就决定了下一个 `Async` 是否运行。

为了做到这一点，我们需要将 `Async` 和 `Result` 视为一个单一的类型——让我们富有想象力地称之为 `AsyncResult`。

如果它们是单一类型，那么 `bind` 看起来像这样：

![img](https://fsharpforfunandprofit.com/posts/elevated-world-5/vgfp_asyncresult-1.png)

这意味着前一个值将决定下一个值。

而且，“交换”变得简单得多：

![img](https://fsharpforfunandprofit.com/posts/elevated-world-5/vgfp_asyncresult-2.png)

### 定义 AsyncResult 类型

好的，让我们定义 `AsyncResult` 类型及其关联的 `map`, `return`, `apply` 和 `bind` 函数。

```F#
/// type alias (optional)
type AsyncResult<'a> = Async<Result<'a>>

/// functions for AsyncResult
module AsyncResult =
module AsyncResult =

    let map f =
        f |> Result.map |> Async.map

    let retn x =
        x |> Result.retn |> Async.retn

    let apply fAsyncResult xAsyncResult =
        fAsyncResult |> Async.bind (fun fResult ->
        xAsyncResult |> Async.map (fun xResult ->
        Result.apply fResult xResult))

    let bind f xAsyncResult = async {
        let! xResult = xAsyncResult
        match xResult with
        | Success x -> return! f x
        | Failure err -> return (Failure err)
        }
```

笔记：

- 类型别名是可选的。我们可以在代码中直接使用 `Async<Result<'a>>`，它会很好地工作。关键在于，从概念上讲，`AsyncResult` 是一个单独的类型。
- `bind` 实现是新的。延续函数 `f` 现在跨越了两个世界，并且具有签名 `'a -> Async<Result<'b>>`。
  - 如果内部 `Result` 成功，则使用结果对连续函数 `f` 进行求值。`return!` 语法意味着返回值已经被提升。
  - 如果内部 `Result` 是失败的，我们必须将失败提升到 Async。

### 定义遍历和序列函数

有了 `bind` 和 `return`，我们可以为 `AsyncResult` 创建适当的 `traverse` 和 `sequence` 函数：

```F#
module List =

    /// Map an AsyncResult producing function over a list to get a new AsyncResult
    /// using monadic style
    /// ('a -> AsyncResult<'b>) -> 'a list -> AsyncResult<'b list>
    let rec traverseAsyncResultM f list =

        // define the monadic functions
        let (>>=) x f = AsyncResult.bind f x
        let retn = AsyncResult.retn

        // define a "cons" function
        let cons head tail = head :: tail

        // right fold over the list
        let initState = retn []
        let folder head tail =
            f head >>= (fun h ->
            tail >>= (fun t ->
            retn (cons h t) ))

        List.foldBack folder list initState

    /// Transform a "list<AsyncResult>" into a "AsyncResult<list>"
    /// and collect the results using bind.
    let sequenceAsyncResultM x = traverseAsyncResultM id x
```

### 定义和测试下载功能

最后，`largestPageSize` 函数现在更简单了，只需要一个序列。

```F#
let largestPageSizeM_AR urls =
    urls
    |> List.map (fun s -> System.Uri(s) |> getUriContentSize)
    |> List.sequenceAsyncResultM
    |> AsyncResult.map maxContentSize
```

让我们使用良好的站点列表运行 `largestSizeM_AR` 10次，看看与应用函子版本是否有任何差异：

```F#
let f() =
    largestPageSizeM_AR goodSites
    |> Async.RunSynchronously
    |> showContentSizeResult
time 10 "largestPageSizeM_AR_Good" f
```

输出如下：

```
[google.com] Started ...
[google.com] ... finished
[bbc.co.uk] Started ...
[bbc.co.uk] ... finished
[fsharp.org] Started ...
[fsharp.org] ... finished
[microsoft.com] Started ...
[microsoft.com] ... finished

SUCCESS: [bbc.co.uk] Content size is 108510
largestPageSizeM_AR_Good: Average time per run:  1026ms
```

同样，下载是连续发生的。而且，每次运行的时间几乎是应用函子版本的两倍。

现在，我们一直在等待的时刻！在第一个坏网站之后，它会跳过下载吗？

```F#
let f() =
    largestPageSizeM_AR badSites
    |> Async.RunSynchronously
    |> showContentSizeResult
time 10 "largestPageSizeM_AR_Bad" f
```

输出如下：

```
  [example.com] Started ...
  [example.com] ... exception

FAILURE: ["[example.com] "The remote server returned an error: (404) Not Found.""]
largestPageSizeM_AR_Bad: Average time per run:   117ms
```

成功！第一个坏网站的错误阻止了其余的下载，而短运行时间就是证明。

## 摘要

在这篇文章中，我们通过一个小的实际例子。我希望这个例子表明， `map`, `apply`, `bind`, `traverse`, 和 `sequence` 不仅仅是学术抽象，而是你工具带中必不可少的工具。

在下一篇文章中，我们将讨论另一个实际例子，但这次我们最终将创造我们自己的提升世界。到时候见！

# 6 重塑阅读器 monad

或者，设计你自己的提升世界
07八月2015 这篇文章是超过3年

https://fsharpforfunandprofit.com/posts/elevated-world-6/

这篇文章是系列文章中的第六篇。在前两篇文章中，我描述了处理泛型数据类型的一些核心函数：`map`、`bind` 等。在第三篇文章里，我讨论了“应用函子”与“单子性”风格，以及如何提升值和函数之间的一致性。在第四篇和之前的文章中，我介绍了 `traverse` 和 `sequence` 作为处理提升值列表的一种方式，我们在一个实际示例中看到了这一点：下载一些 URL。

在这篇文章中，我们将通过另一个实际例子来结束，但这次我们将创建自己的“高级世界”，作为处理尴尬代码的一种方式。我们将看到这种方法非常普遍，以至于它有一个名字——“阅读器单子”。

## 系列内容

以下是本系列中提到的各种函数的快捷方式列表：

- **第 1 部分：提升到更高的世界**
  - `map` 函数
  - `return` 函数
  - `apply` 函数
  - `liftN` 系列函数
  - `zip` 函数和 ZipList 世界
- **第 2 部分：如何构建跨世界函数**
  - `bind` 函数
  - List 不是单子。Option 不是单子。
- **第 3 部分：在实践中使用核心函数**
  - 独立和依赖数据
  - 示例：使用应用函子风格和单子风格进行验证
  - 迈向一致的世界
  - Kleisli 世界
- **第 4 部分：混合列表和提升值**
  - 混合列表和提升值
  - `traverse` / `MapM` 函数
  - `sequence` 函数
  - “序列”作为ad-hoc实现的配方
  - 可读性与性能
  - 老兄，我的 `filter` 在哪里？
- **第 5 部分：使用所有技术的真实世界示例**
  - 示例：下载和处理网站列表
  - 将两个世界视为一体
- **第 6 部分：设计你自己的提升世界**
  - 设计你自己的提升世界
  - 过滤掉故障
  - Reader monad
- **第 7 部分：总结**
  - 提到的操作符名单
  - 进一步阅读

## 第 6 部分：设计你自己的高雅世界

我们将在这篇文章中使用的场景是这样的：

*一位客户来到您的网站，想查看他们购买的产品的信息。*

在本例中，我们假设您有一个用于键/值存储（如 Redis 或 NoSql 数据库）的 API，并且您需要的所有信息都存储在那里。

所以我们需要的代码看起来像这样：

```
打开API连接
使用API获取客户id购买的产品id
对于每个产品id：
	使用API获取该id的产品信息
关闭API连接
返回产品信息列表
```

这有多难？

好吧，结果却出乎意料地棘手！幸运的是，我们可以找到一种方法来更容易地使用本系列中的概念。

## 定义领域和虚拟 ApiClient

首先，让我们定义域类型：

- 当然会有 `CustomerId` 和 `ProductId`。
- 对于产品信息，我们只需定义一个带有 `ProductName` 字段的简单 `ProductInfo`。

以下是类型：

```F#
type CustId = CustId of string
type ProductId = ProductId of string
type ProductInfo = {ProductName: string; }
```

为了测试我们的 api，让我们创建一个 `ApiClient` 类，其中包含一些 `Get` 和 `Set` 方法，并由静态可变字典支持。这是基于类似的 API，如 Redis 客户端。

笔记：

- `Get` 和 `Set` 都可以处理对象，所以我添加了一个强制转换机制。
- 如果出现错误，如转换失败或缺少密钥，我将使用我们在本系列中一直使用的 `Result` 类型。因此，`Get` 和 `Set` 都返回 `Result`s 而不是普通对象。
- 为了使它更真实，我还为 `Open`、`Close` 和 `Dispose` 添加了虚拟（dummy）方法。
- 所有方法都将日志跟踪到控制台。

```F#
type ApiClient() =
    // static storage
    static let mutable data = Map.empty<string,obj>

    /// Try casting a value
    /// Return Success of the value or Failure on failure
    member private this.TryCast<'a> key (value:obj) =
        match value with
        | :? 'a as a ->
            Result.Success a
        | _  ->
            let typeName = typeof<'a>.Name
            Result.Failure [sprintf "Can't cast value at %s to %s" key typeName]

    /// Get a value
    member this.Get<'a> (id:obj) =
        let key =  sprintf "%A" id
        printfn "[API] Get %s" key
        match Map.tryFind key data with
        | Some o ->
            this.TryCast<'a> key o
        | None ->
            Result.Failure [sprintf "Key %s not found" key]

    /// Set a value
    member this.Set (id:obj) (value:obj) =
        let key =  sprintf "%A" id
        printfn "[API] Set %s" key
        if key = "bad" then  // for testing failure paths
            Result.Failure [sprintf "Bad Key %s " key]
        else
            data <- Map.add key value data
            Result.Success ()

    member this.Open() =
        printfn "[API] Opening"

    member this.Close() =
        printfn "[API] Closing"

    interface System.IDisposable with
        member this.Dispose() =
            printfn "[API] Disposing"
```

让我们做一些测试：

```F#
do
    use api = new ApiClient()
    api.Get "K1" |> printfn "[K1] %A"

    api.Set "K2" "hello" |> ignore
    api.Get<string> "K2" |> printfn "[K2] %A"

    api.Set "K3" "hello" |> ignore
    api.Get<int> "K3" |> printfn "[K3] %A"
```

结果如下：

```
[API] Get "K1"
[K1] Failure ["Key "K1" not found"]
[API] Set "K2"
[API] Get "K2"
[K2] Success "hello"
[API] Set "K3"
[API] Get "K3"
[K3] Failure ["Can't cast value at "K3" to Int32"]
[API] Disposing
```

## 首次实现尝试

对于我们实现场景的第一次尝试，让我们从上面的伪代码开始：

```F#
let getPurchaseInfo (custId:CustId) : Result<ProductInfo list> =

    // Open api connection
    use api = new ApiClient()
    api.Open()

    // Get product ids purchased by customer id
    let productIdsResult = api.Get<ProductId list> custId

    let productInfosResult = ??

    // Close api connection
    api.Close()

    // Return the list of product infos
    productInfosResult
```

到目前为止一切顺利，但已经有点问题了。

`getPurchaseInfo` 函数接受 `CustId` 作为输入，但它不能只输出 `ProductInfo` 列表，因为可能会出现故障。这意味着返回类型需要是 `Result<ProductInfo list>`。

好的，我们如何创建我们的 `productInfosResult`？

嗯，这应该很容易。如果 `productIdsResult` 为 Success，则循环遍历每个 id 并获取每个 id 的信息。如果 `productIdsResult` 为 Failure，则只需返回失败。

```F#
let getPurchaseInfo (custId:CustId) : Result<ProductInfo list> =

    // Open api connection
    use api = new ApiClient()
    api.Open()

    // Get product ids purchased by customer id
    let productIdsResult = api.Get<ProductId list> custId

    let productInfosResult =
        match productIdsResult with
        | Success productIds ->
            let productInfos = ResizeArray()  // Same as .NET List<T>
            for productId in productIds do
                let productInfo = api.Get<ProductInfo> productId
                productInfos.Add productInfo  // mutation!
            Success productInfos
        | Failure err ->
            Failure err

    // Close api connection
    api.Close()

    // Return the list of product infos
    productInfosResult
```

嗯，它看起来有点难看。我必须使用可变数据结构（`productInfos`）来积累每个产品信息，然后将其包装在`Success` 中。

还有一个更糟糕的问题，我从 `api.Get<productInfo>` 获取的 `productInfo` 根本不是 `ProductInfo`，而是 `Result<ProductInfo>`，因此 `productInfos` 根本不是正确的类型！

让我们添加代码来测试每个 `ProductInfo` 结果。如果成功，则将其添加到产品信息列表中，如果失败，则返回失败。

```F#
let getPurchaseInfo (custId:CustId) : Result<ProductInfo list> =

    // Open api connection
    use api = new ApiClient()
    api.Open()

    // Get product ids purchased by customer id
    let productIdsResult = api.Get<ProductId list> custId

    let productInfosResult =
        match productIdsResult with
        | Success productIds ->
            let productInfos = ResizeArray()  // Same as .NET List<T>
            let mutable anyFailures = false
            for productId in productIds do
                let productInfoResult = api.Get<ProductInfo> productId
                match productInfoResult with
                | Success productInfo ->
                    productInfos.Add productInfo
                | Failure err ->
                    Failure err
            Success productInfos
        | Failure err ->
            Failure err

    // Close api connection
    api.Close()

    // Return the list of product infos
    productInfosResult
```

嗯，不。那根本行不通。上面的代码无法编译。当发生故障时，我们不能在循环中进行“提前返回”。

那么，到目前为止，我们有什么？一些非常丑陋的代码甚至无法编译。

一定有更好的办法。

## 第二次实现尝试

如果我们能隐藏所有这些 `Result`s 的展开和测试，那就太好了。还有——计算表达式可以帮忙。

如果我们为 `Result` 创建一个计算表达式，我们可以编写如下代码：

```F#
/// CustId -> Result<ProductInfo list>
let getPurchaseInfo (custId:CustId) : Result<ProductInfo list> =

    // Open api connection
    use api = new ApiClient()
    api.Open()

    let productInfosResult = Result.result {

        // Get product ids purchased by customer id
        let! productIds = api.Get<ProductId list> custId

        let productInfos = ResizeArray()  // Same as .NET List<T>
        for productId in productIds do
            let! productInfo = api.Get<ProductInfo> productId
            productInfos.Add productInfo
        return productInfos |> List.ofSeq
        }

    // Close api connection
    api.Close()

    // Return the list of product infos
    productInfosResult
```

在 `let productInfosResult = Result.Result { .. }` 代码中，我们创建了一个结果计算表达式，简化了所有的解包（使用 `let!`）和换行（使用 `return`）。

因此，此实现在任何地方都没有显式的 `xxxResult` 值。然而，它仍然必须使用可变集合类来进行累加，因为 `for productId in productIds do` 实际上不是一个真正的 `for` 循环，我们不能用 `List.map` 替换它。

### `result` 计算表达式

这就引出了 `result` 计算表达式的实现。在之前的帖子中，`ResultBuilder` 只有两个方法，`Return` 和 `Bind`，但为了获得 `for..in..do` 功能中，我们还必须实现许多其他方法，最终会变得有点复杂。

```F#
module Result =

    let bind f xResult = ...

    type ResultBuilder() =
        member this.Return x = retn x
        member this.ReturnFrom(m: Result<'T>) = m
        member this.Bind(x,f) = bind f x

        member this.Zero() = Failure []
        member this.Combine (x,f) = bind f x
        member this.Delay(f: unit -> _) = f
        member this.Run(f) = f()

        member this.TryFinally(m, compensation) =
            try this.ReturnFrom(m)
            finally compensation()

        member this.Using(res:#System.IDisposable, body) =
            this.TryFinally(body res, fun () ->
            match res with
            | null -> ()
            | disp -> disp.Dispose())

        member this.While(guard, f) =
            if not (guard()) then
                this.Zero()
            else
                this.Bind(f(), fun _ -> this.While(guard, f))

        member this.For(sequence:seq<_>, body) =
            this.Using(sequence.GetEnumerator(), fun enum ->
                this.While(enum.MoveNext, this.Delay(fun () ->
                    body enum.Current)))

    let result = new ResultBuilder()
```

我有一个关于计算表达式内部的系列，所以我不想在这里解释所有的代码。相反，在本文的其余部分，我们将对 `getPurchaseInfo` 进行重构，到本文结束时，我们将看到我们根本不需要结果计算表达式。

## 重构函数

`getPurchaseInfo` 函数目前的问题是它混合了关注点：它既创建了 `ApiClient`，又对它做了一些工作。

这种方法存在许多问题：

- 如果我们想用API做不同的工作，我们必须重复这个代码的打开/关闭部分。其中一个实现可能会打开API，但忘记关闭它。
- 它不能用模拟API客户端进行测试。

我们可以通过参数化操作将 `ApiClient` 的创建与其使用分开来解决这两个问题，就像这样。

```F#
let executeApiAction apiAction  =

    // Open api connection
    use api = new ApiClient()
    api.Open()

    // do something with it
    let result = apiAction api

    // Close api connection
    api.Close()

    // return result
    result
```

传入的动作函数如下，`ApiClient` 和 `CustId` 都有一个参数：

```F#
/// CustId -> ApiClient -> Result<ProductInfo list>
let getPurchaseInfo (custId:CustId) (api:ApiClient) =

    let productInfosResult = Result.result {
        let! productIds = api.Get<ProductId list> custId

        let productInfos = ResizeArray()  // Same as .NET List<T>
        for productId in productIds do
            let! productInfo = api.Get<ProductInfo> productId
            productInfos.Add productInfo
        return productInfos |> List.ofSeq
        }

    // return result
    productInfosResult
```

请注意，`getPurchaseInfo` 有两个参数，但 `executeApiAction` 需要一个只有一个参数的函数。

没问题！只需使用局部应用程序来烘焙第一个参数：

```F#
let action = getPurchaseInfo (CustId "C1")  // partially apply
executeApiAction action
```

这就是为什么 `ApiClient` 是参数列表中的第二个参数——这样我们就可以进行部分应用。

### 更多重构

我们可能需要出于其他目的获取产品 id，以及 productInfo，所以让我们也将它们重构为单独的函数：

```F#
/// CustId -> ApiClient -> Result<ProductId list>
let getPurchaseIds (custId:CustId) (api:ApiClient) =
    api.Get<ProductId list> custId

/// ProductId -> ApiClient -> Result<ProductInfo>
let getProductInfo (productId:ProductId) (api:ApiClient) =
    api.Get<ProductInfo> productId

/// CustId -> ApiClient -> Result<ProductInfo list>
let getPurchaseInfo (custId:CustId) (api:ApiClient) =

    let result = Result.result {
        let! productIds = getPurchaseIds custId api

        let productInfos = ResizeArray()
        for productId in productIds do
            let! productInfo = getProductInfo productId api
            productInfos.Add productInfo
        return productInfos |> List.ofSeq
        }

    // return result
    result
```

现在，我们有了这些很好的核心函数 `getPurchaseIds` 和 `getProductInfo`，但我很恼火，我必须编写混乱的代码才能将它们粘在 `getPurchaseInfo` 中。

理想情况下，我想做的是将 `getPurchaseIds` 的输出管道到 `getProductInfo` 中，如下所示：

```F#
let getPurchaseInfo (custId:CustId) =
    custId
    |> getPurchaseIds
    |> List.map getProductInfo
```

或者以图表的形式：

![img](https://fsharpforfunandprofit.com/posts/elevated-world-6/vgfp_api_pipe.png)

但我不能，原因有两个：

- 首先，`getProductInfo` 有两个参数。不仅是 `ProductId`，还有 `ApiClient`。
- 其次，即使 `ApiClient` 不存在，`getProductInfo` 的输入也是一个简单的 `ProductId`，但 `getPurchaseIds` 的输出是一个 `Result`。

如果我们能同时解决这两个问题，那岂不是太好了！

## 介绍我们自己的提升世界

让我们解决第一个问题。当额外的 `ApiClient` 参数不断阻碍时，我们如何组合函数？

这是典型的 API 调用函数的样子：

![img](https://fsharpforfunandprofit.com/posts/elevated-world-6/vgfp_api_action1.png)

如果我们看看类型签名，我们会看到这是一个有两个参数的函数：

![img](https://fsharpforfunandprofit.com/posts/elevated-world-6/vgfp_api_action2.png)

但是，另一种解释此函数的方法是将其视为一个具有一个参数的函数，该参数返回另一个函数。返回的函数有一个 `ApiClient` 参数，并返回最终输出。

![img](https://fsharpforfunandprofit.com/posts/elevated-world-6/vgfp_api_action3.png)

你可能会这样想：我现在有一个输入，但稍后我才会有一个真正的 `ApiClient`，所以让我使用输入来创建一个 api 消费函数，我现在可以用各种方式将其粘合在一起，而根本不需要 `ApiClient`。

让我们为这个 api 消费函数命名。我们称之为 `ApiAction`。

![img](https://fsharpforfunandprofit.com/posts/elevated-world-6/vgfp_api_action4.png)

事实上，让我们做得更多——让我们把它变成一种类型！

```F#
type ApiAction<'a> = (ApiClient -> 'a)
```

不幸的是，就目前而言，这只是一个函数的类型别名，而不是一个单独的类型。我们需要将其包装在一个单独的案例联合中，使其成为一个独特的类型。

```F#
type ApiAction<'a> = ApiAction of (ApiClient -> 'a)
```

### 重写以使用 ApiAction

现在我们有了一个真正的类型可以使用，我们可以重写我们的核心域函数来使用它。

首次 `getPurchaseIds`：

```F#
// CustId -> ApiAction<Result<ProductId list>>
let getPurchaseIds (custId:CustId) =

    // create the api-consuming function
    let action (api:ApiClient) =
        api.Get<ProductId list> custId

    // wrap it in the single case
    ApiAction action
```

签名现在是 `CustId -> ApiAction<Result<ProductId-list>>`，您可以将其解释为：“给我一个 CustId，我会给你一个 ApiAction，当给你 api 时，它会生成一个 ProductId 列表”。

同样，可以重写 `getProductInfo` 以返回 `ApiAction`：

```F#
// ProductId -> ApiAction<Result<ProductInfo>>
let getProductInfo (productId:ProductId) =

    // create the api-consuming function
    let action (api:ApiClient) =
        api.Get<ProductInfo> productId

    // wrap it in the single case
    ApiAction action
```

请注意这些签名：

- `CustId -> ApiAction<Result<ProductId list>>`
- `ProductId -> ApiAction<Result<ProductInfo>>`

这开始看起来非常熟悉。我们在上一篇文章中没有看到类似 `Async<Result<_>>`的东西吗？

### ApiAction 作为一个提升的世界

若我们绘制这两个函数中涉及的各种类型的图表，我们可以清楚地看到 `ApiAction` 是一个提升的世界，就像 `List` 和 `Result` 一样。这意味着我们应该能够使用与以前相同的技术： `map`, `bind`, `traverse` 等。

这是 `getPurchaseIds` 的堆栈图。输入是 `CustId`，输出是 `ApiAction<Result<List<ProductId>>>`：

![img](https://fsharpforfunandprofit.com/posts/elevated-world-6/vgfp_api_getpurchaseids.png)

在 `getProductInfo` 中，输入是一个 `ProductId`，输出是一个 `ApiAction<Result<ProductInfo>>`：

![img](https://fsharpforfunandprofit.com/posts/elevated-world-6/vgfp_api_getproductinfo.png)

我们想要的组合函数 `getPurchaseInfo` 应该如下所示：

![img](https://fsharpforfunandprofit.com/posts/elevated-world-6/vgfp_api_getpurchaseinfo.png)

现在，组合这两个函数的问题非常明显：`getPurchaseIds` 的输出不能用作 `getProductInfo` 的输入：

![img](https://fsharpforfunandprofit.com/posts/elevated-world-6/vgfp_api_noncompose.png)

但我想你可以看到我们有一些希望！应该有一些方法来操纵这些层，使它们匹配起来，然后我们就可以很容易地组合它们。

这就是我们下一步要做的。

### ApiActionResult 介绍

在上一篇文章中，我们将 `Async` 和 `Result` 合并为复合类型 `AsyncResult`。我们可以在这里做同样的事情，并创建 `ApiActionResult` 类型。

当我们进行此更改时，我们的两个函数会变得稍微简单一些：

![img](https://fsharpforfunandprofit.com/posts/elevated-world-6/vgfp_api_apiactionresult_functions.png)

足够的图表了——现在让我们写一些代码。

首先，我们需要为 `ApiAction` 定义 `map`、`apply`、`return` 和 `bind`：

```F#
module ApiAction =

    /// Evaluate the action with a given api
    /// ApiClient -> ApiAction<'a> -> 'a
    let run api (ApiAction action) =
        let resultOfAction = action api
        resultOfAction

    /// ('a -> 'b) -> ApiAction<'a> -> ApiAction<'b>
    let map f action =
        let newAction api =
            let x = run api action
            f x
        ApiAction newAction

    /// 'a -> ApiAction<'a>
    let retn x =
        let newAction api =
            x
        ApiAction newAction

    /// ApiAction<('a -> 'b)> -> ApiAction<'a> -> ApiAction<'b>
    let apply fAction xAction =
        let newAction api =
            let f = run api fAction
            let x = run api xAction
            f x
        ApiAction newAction

    /// ('a -> ApiAction<'b>) -> ApiAction<'a> -> ApiAction<'b>
    let bind f xAction =
        let newAction api =
            let x = run api xAction
            run api (f x)
        ApiAction newAction

    /// Create an ApiClient and run the action on it
    /// ApiAction<'a> -> 'a
    let execute action =
        use api = new ApiClient()
        api.Open()
        let result = run api action
        api.Close()
        result
```

请注意，所有函数都使用一个名为 `run` 的辅助函数，该函数解包 `ApiAction` 以获取内部函数，并将其应用于传入的 `api`。结果是封装在 `ApiAction` 中的值。

例如，如果我们有一个 `ApiAction<int>`，那么 `run api myAction` 会得到一个 `int`。

在底部，有一个 `execute` 函数，它创建一个 `ApiClient`，打开连接，运行操作，然后关闭连接。

定义了 `ApiAction` 的核心函数后，我们可以继续定义复合类型 `ApiActionResult` 的函数，就像我们在上一篇文章中对 `AsyncResult` 所做的那样：

```F#
module ApiActionResult =

    let map f  =
        ApiAction.map (Result.map f)

    let retn x =
        ApiAction.retn (Result.retn x)

    let apply fActionResult xActionResult =
        let newAction api =
            let fResult = ApiAction.run api fActionResult
            let xResult = ApiAction.run api xActionResult
            Result.apply fResult xResult
        ApiAction newAction

    let bind f xActionResult =
        let newAction api =
            let xResult = ApiAction.run api xActionResult
            // create a new action based on what xResult is
            let yAction =
                match xResult with
                | Success x ->
                    // Success? Run the function
                    f x
                | Failure err ->
                    // Failure? wrap the error in an ApiAction
                    (Failure err) |> ApiAction.retn
            ApiAction.run api yAction
        ApiAction newAction
```

## 计算变换

现在我们已经有了所有的工具，我们必须决定使用什么转换来更改 `getProductInfo` 的形状，以便输入匹配。

我们应该选择 `map`、`bind` 还是 `traverse`？

让我们在视觉上玩弄堆栈，看看每种转换会发生什么。

在我们开始之前，让我们明确一下我们想要实现的目标：

- 我们有两个函数 `getPurchaseIds` 和 `getProductInfo`，我们想将它们组合成一个函数 `getPurchaseInfo`。
- 我们必须操纵 `getProductInfo` 的左侧（输入），使其与 `getPurchaseIds` 的输出相匹配。
- 我们必须操纵 `getProductInfo` 的右侧（输出），使其与我们理想的 `getPurchaseInfo` 的输出相匹配。

![img](https://fsharpforfunandprofit.com/posts/elevated-world-6/vgfp_api_wanted.png)

### Map

作为提醒，`map` 在两侧添加了一个新的堆栈。因此，如果我们从这样一个通用的世界穿越函数开始：

![img](https://fsharpforfunandprofit.com/posts/elevated-world-6/vgfp_api_generic.png)

然后，在 `List.map` 之后，我们将在每个站点上都有一个新的 `List` 堆栈。

![img](https://fsharpforfunandprofit.com/posts/elevated-world-6/vgfp_api_map_generic.png)

以下是转换前的 `getProductInfo`：

![img](https://fsharpforfunandprofit.com/posts/elevated-world-6/vgfp_api_getproductinfo2.png)

这是使用 `List.map` 后的样子

![img](https://fsharpforfunandprofit.com/posts/elevated-world-6/vgfp_api_map_getproductinfo.png)

这似乎很有希望——我们现在有一个 `ProductId` `List` 作为输入，如果我们能在上面堆叠一个 `ApiActionResult`，我们就会匹配 `getPurchaseId` 的输出。

但输出完全错误。我们希望 `ApiActionResult` 保持在顶部。也就是说，我们不想要 `ApiActionResult` 的 `List`，而是想要 `List` 的 `ApiActionResult`。

### 绑定

好吧，那 `bind` 呢？

如果你还记得，`bind` 通过在左侧添加一个新的堆栈，将一个“对角线”函数变成一个水平函数。因此，例如，无论顶部升高的世界在右边，它都会被添加到左边。

![img](https://fsharpforfunandprofit.com/posts/elevated-world-6/vgfp_api_generic.png)

![img](https://fsharpforfunandprofit.com/posts/elevated-world-6/vgfp_api_bind_generic.png)

这是使用 `ApiActionResult.bind` 后我们的 `getProductInfo` 的样子

![img](https://fsharpforfunandprofit.com/posts/elevated-world-6/vgfp_api_bind_getproductinfo.png)

这对我们没有好处。我们需要一个 `ProductId` `List` 作为输入。

### 遍历

最后，让我们尝试 `traverse`。

`traverse` 将值的对角函数转换为包含值的列表的对角函数。也就是说，`List` 被添加为左侧的顶部堆栈，右侧的顶部堆栈中的第二个堆栈。

![img](https://fsharpforfunandprofit.com/posts/elevated-world-6/vgfp_api_generic.png)

![img](https://fsharpforfunandprofit.com/posts/elevated-world-6/vgfp_api_traverse_generic.png)

如果我们在 `getProductInfo` 上尝试一下，我们会得到一些非常有前景的东西。

![img](https://fsharpforfunandprofit.com/posts/elevated-world-6/vgfp_api_traverse_getproductinfo.png)

输入是所需的列表。输出是完美的。我们想要一个 `ApiAction<Result<List<ProductInfo>>>`，现在我们有了它。

所以我们现在需要做的就是在左侧添加一个 `ApiActionResult`。

我们刚刚看到这个！这是 `bind`。所以如果我们也这样做，我们就完成了。

![img](https://fsharpforfunandprofit.com/posts/elevated-world-6/vgfp_api_complete_getproductinfo.png)

在这里，它被表示为代码：

```F#
let getPurchaseInfo =
    let getProductInfo1 = traverse getProductInfo
    let getProductInfo2 = ApiActionResult.bind getProductInfo1
    getPurchaseIds >> getProductInfo2
```

或者让它稍微不那么丑陋：

```F#
let getPurchaseInfo =
    let getProductInfoLifted =
        getProductInfo
        |> traverse
        |> ApiActionResult.bind
    getPurchaseIds >> getProductInfoLifted
```

让我们将其与早期版本的 `getPurchaseInfo` 进行比较：

```F#
let getPurchaseInfo (custId:CustId) (api:ApiClient) =

    let result = Result.result {
        let! productIds = getPurchaseIds custId api

        let productInfos = ResizeArray()
        for productId in productIds do
            let! productInfo = getProductInfo productId api
            productInfos.Add productInfo
        return productInfos |> List.ofSeq
        }

    // return result
    result
```

让我们在表格中比较这两个版本：

| 早先版本                                                     | 最近函数                         |
| :----------------------------------------------------------- | :------------------------------- |
| 复合函数是不平凡的，需要特殊的代码将两个较小的函数粘合在一起 | 复合功能只是管道和组合           |
| 使用“结果”计算表达式                                         | 无需特殊语法                     |
| 有特殊的代码来循环结果                                       | 使用“遍历”                       |
| 使用中间（可变）List对象来累积产品信息列表                   | 不需要中间值。只是一个数据管道。 |

### 实现遍历

上面的代码使用 `traverse`，但我们还没有实现它。正如我之前提到的，它可以按照模板机械地实现。

这是：

```F#
let traverse f list =
    // define the applicative functions
    let (<*>) = ApiActionResult.apply
    let retn = ApiActionResult.retn

    // define a "cons" function
    let cons head tail = head :: tail

    // right fold over the list
    let initState = retn []
    let folder head tail =
        retn cons <*> f head <*> tail

    List.foldBack folder list initState
```

### 测试实现

让我们来测试一下！

首先，我们需要一个辅助函数来显示结果：

```F#
let showResult result =
    match result with
    | Success (productInfoList) ->
        printfn "SUCCESS: %A" productInfoList
    | Failure errs ->
        printfn "FAILURE: %A" errs
```

接下来，我们需要在 API 中加载一些测试数据：

```F#
let setupTestData (api:ApiClient) =
    //setup purchases
    api.Set (CustId "C1") [ProductId "P1"; ProductId "P2"] |> ignore
    api.Set (CustId "C2") [ProductId "PX"; ProductId "P2"] |> ignore

    //setup product info
    api.Set (ProductId "P1") {ProductName="P1-Name"} |> ignore
    api.Set (ProductId "P2") {ProductName="P2-Name"} |> ignore
    // P3 missing

// setupTestData is an api-consuming function
// so it can be put in an ApiAction
// and then that apiAction can be executed
let setupAction = ApiAction setupTestData
ApiAction.execute setupAction
```

- 客户 C1 购买了两个产品：P1 和 P2。
- 客户 C2 购买了两种产品：PX 和 P2。
- 产品 P1 和 P2 有一些信息。
- 产品 PX 没有任何信息。

让我们看看这对不同的客户 id 是如何起作用的。

我们将从客户 C1 开始。对于该客户，我们希望收到两份产品信息：

```F#
CustId "C1"
|> getPurchaseInfo
|> ApiAction.execute
|> showResult
```

结果如下：

```
[API] Opening
[API] Get CustId "C1"
[API] Get ProductId "P1"
[API] Get ProductId "P2"
[API] Closing
[API] Disposing
SUCCESS: [{ProductName = "P1-Name";}; {ProductName = "P2-Name";}]
```

如果我们使用一个缺失的客户，比如 CX，会发生什么？

```F#
CustId "CX"
|> getPurchaseInfo
|> ApiAction.execute
|> showResult
```

正如预期的那样，我们遇到了一个很好的“未找到密钥”失败，一旦找不到密钥，其余的操作就会被跳过。

```
[API] Opening
[API] Get CustId "CX"
[API] Closing
[API] Disposing
FAILURE: ["Key CustId "CX" not found"]
```

如果其中一个购买的产品没有信息怎么办？例如，客户 C2 购买了 PX 和 P2，但没有 PX 的信息。

```F#
CustId "C2"
|> getPurchaseInfo
|> ApiAction.execute
|> showResult
```

总体结果是失败。任何不良产品都会导致整个操作失败。

```
[API] Opening
[API] Get CustId "C2"
[API] Get ProductId "PX"
[API] Get ProductId "P2"
[API] Closing
[API] Disposing
FAILURE: ["Key ProductId "PX" not found"]
```

但请注意，即使产品 PX 失败，也会获取产品 P2 的数据。为什么？因为我们使用的是 `traverse` 的应用函子版本，所以列表中的每个元素都是“并行”获取的。

如果我们只想在知道 PX 存在后获取 P2，那么我们应该使用单子风格。我们已经看到了如何编写遍历的单子版本，所以我把它留给你做练习！

## 过滤掉故障

在上面的实现中，如果找不到任何产品，`getPurchaseInfo` 函数就会失败。严厉！

一个真正的应用程序可能会更宽容。可能应该发生的是，失败的产品会被记录下来，但所有的成功都会被累积并返回。

我们怎么能这样做？

答案很简单——我们只需要修改 `traverse` 函数来跳过失败。

首先，我们需要为 `ApiActionResult` 创建一个新的辅助函数。它将允许我们传递两个函数，一个用于成功情况，另一个用于错误情况：

```F#
module ApiActionResult =

    let map = ...
    let retn =  ...
    let apply = ...
    let bind = ...

    let either onSuccess onFailure xActionResult =
        let newAction api =
            let xResult = ApiAction.run api xActionResult
            let yAction =
                match xResult with
                | Result.Success x -> onSuccess x
                | Result.Failure err -> onFailure err
            ApiAction.run api yAction
        ApiAction newAction
```

这个辅助函数帮助我们在 `ApiAction` 中匹配这两种情况，而无需进行复杂的解包。我们需要这个来 `traverse` 跳过失败。

顺便说一句，请注意，`ApiActionResult.bind` 可以根据以 `either` 方式定义：

```F#
let bind f =
    either
        // Success? Run the function
        (fun x -> f x)
        // Failure? wrap the error in an ApiAction
        (fun err -> (Failure err) |> ApiAction.retn)
```

现在我们可以定义我们的“遍历并记录失败”函数：

```F#
let traverseWithLog log f list =
    // define the applicative functions
    let (<*>) = ApiActionResult.apply
    let retn = ApiActionResult.retn

    // define a "cons" function
    let cons head tail = head :: tail

    // right fold over the list
    let initState = retn []
    let folder head tail =
        (f head)
        |> ApiActionResult.either
            (fun h -> retn cons <*> retn h <*> tail)
            (fun errs -> log errs; tail)
    List.foldBack folder list initState
```

这个和之前的实现之间的唯一区别是这一点：

```F#
let folder head tail =
    (f head)
    |> ApiActionResult.either
        (fun h -> retn cons <*> retn h <*> tail)
        (fun errs -> log errs; tail)
```

这表明：

- 如果新的第一个元素（`f head`）成功，则提升内部值（`retn h`）并将其与尾部进行对比，以构建一个新的列表。
- 但是，如果新的第一个元素失败，则使用传入的日志函数（`log`）记录内部错误（`errs`），并重用当前的尾部。这样，失败的元素不会添加到列表中，但也不会导致整个函数失败。

让我们创建一个新函数 `getPurchasesInfoWithLog`，并尝试使用客户 C2 和缺少的产品 PX：

```F#
let getPurchasesInfoWithLog =
    let log errs = printfn "SKIPPED %A" errs
    let getProductInfoLifted =
        getProductInfo
        |> traverseWithLog log
        |> ApiActionResult.bind
    getPurchaseIds >> getProductInfoLifted

CustId "C2"
|> getPurchasesInfoWithLog
|> ApiAction.execute
|> showResult
```

结果现在是 Success，但只返回了 P2 的一个 `ProductInfo`。日志显示跳过了 PX。

```
[API] Opening
[API] Get CustId "C2"
[API] Get ProductId "PX"
SKIPPED ["Key ProductId "PX" not found"]
[API] Get ProductId "P2"
[API] Closing
[API] Disposing
SUCCESS: [{ProductName = "P2-Name";}]
```

## 阅读器 monad

如果你仔细查看 `ApiResult` 模块，你会发现 `map`、`bind` 和所有其他函数都不使用任何关于传递的 `api` 的信息。我们可以把它做成任何类型，这些功能仍然可以工作。

那么，本着“参数化所有事物”的精神，为什么不把它变成一个参数呢？

这意味着我们可以将 `ApiAction` 定义如下：

```F#
type ApiAction<'anything,'a> = ApiAction of ('anything -> 'a)
```

但如果它可以是任何东西，为什么还要称之为 `ApiAction` 呢？它可以表示依赖于传递给它们的对象（如 `api`）的任何一组事物。

我们不是第一个发现这一点的人！这种类型通常被称为 `Reader` 类型，其定义如下：

```F#
type Reader<'environment,'a> = Reader of ('environment -> 'a)
```

额外类型的 `'emvironment` 与 `ApiClient` 在 `ApiAction` 的定义中所起的作用相同。有一些环境作为额外的参数传递给所有函数，就像 `api` 实例一样。

事实上，我们可以很容易地用 `Reader` 来定义 `ApiAction`：

```F#
type ApiAction<'a> = Reader<ApiClient,'a>
```

`Reader` 的函数集与 `ApiAction` 完全相同。我刚刚拿到代码，用 `Reader` 替换了 `ApiAction`，用 `environment` 替换了 `api`！

```F#
module Reader =

    /// Evaluate the action with a given environment
    /// 'env -> Reader<'env,'a> -> 'a
    let run environment (Reader action) =
        let resultOfAction = action environment
        resultOfAction

    /// ('a -> 'b) -> Reader<'env,'a> -> Reader<'env,'b>
    let map f action =
        let newAction environment =
            let x = run environment action
            f x
        Reader newAction

    /// 'a -> Reader<'env,'a>
    let retn x =
        let newAction environment =
            x
        Reader newAction

    /// Reader<'env,('a -> 'b)> -> Reader<'env,'a> -> Reader<'env,'b>
    let apply fAction xAction =
        let newAction environment =
            let f = run environment fAction
            let x = run environment xAction
            f x
        Reader newAction

    /// ('a -> Reader<'env,'b>) -> Reader<'env,'a> -> Reader<'env,'b>
    let bind f xAction =
        let newAction environment =
            let x = run environment xAction
            run environment (f x)
        Reader newAction
```

不过，现在类型签名有点难读了！

`Reader` 类型加上 `bind` 和 `return`，再加上 `bind` 和 `return` 实现了 monad 定律，这意味着 `Reader` 通常被称为“Reader monad”。

我不打算在这里深入研究阅读器单子，但我希望你能看到它实际上是一件有用的事情，而不是一些奇怪的象牙塔概念。

### Reader monad vs. 显式类型

现在，如果你愿意，你可以用 `Reader` 代码替换上面的所有 `ApiAction` 代码，它的工作原理也一样。但你应该吗？

就我个人而言，我认为虽然理解 Reader monad 背后的概念很重要也很有用，但我更喜欢我最初定义的 `ApiAction` 的实际实现，一个显式类型，而不是别名 `Reader<ApiClient, 'a>`。

为什么？好吧，F# 没有类型类，F# 没有部分应用类型构造函数，F# 没有“newtype”。基本上，F# 不是 Haskell！我认为，当语言不支持 F# 时，在 Haskell 中运行良好的习语不应该直接转移到 F# 中。

如果你理解了这些概念，你就可以在几行代码中实现所有必要的转换。是的，这是一项额外的工作，但好处是抽象性和依赖性更少。

如果你的团队都是 Haskell 专家，并且每个人都熟悉 Reader monad，我可能会破例。但对于不同能力的团队，我会过于具体而不是过于抽象。

## 摘要

在这篇文章中，我们通过另一个实际例子，创建了我们自己的提升世界，这让事情变得容易得多，在这个过程中，我们意外地重新发明了阅读器 monad。

如果你喜欢这个，你可以看到一个类似的实际例子，这次是在我关于“弗兰肯芬克特博士和莫纳德斯特”的系列文章中，以状态单子为例。

下一篇也是最后一篇文章对该系列进行了快速总结，并提供了一些进一步的阅读。

# 7 映射、绑定和应用，总结

*Part of the "Map and Bind and Apply, Oh my!" series (*[link](https://fsharpforfunandprofit.com/posts/elevated-world-7/#series-toc)*)*

08八月2015 这篇文章是超过3年

https://fsharpforfunandprofit.com/posts/elevated-world-7/

## 系列总结

好吧，这个系列比我最初计划的要长。谢谢你坚持到最后！

我希望这次讨论有助于理解各种函数转换，如 `map` 和 `bind`，并为您提供一些处理跨世界函数的有用技术——甚至可能稍微揭开 m-word 的神秘面纱！

如果你想在自己的代码中开始使用这类函数，我希望你能看到它们是多么容易编写，但你也应该考虑使用一个优秀的 F# 实用程序库，其中包含这些以及更多内容：

- **ExtCore**（源代码，NuGet）。ExtCore 为 F# 核心库（FSharp.Core）提供扩展，旨在帮助您构建工业级 F# 应用程序。这些扩展包括 Array、List、Set 和 Map 等模块的附加功能；不可变的 IntSet、IntMap、LazyList 和 Queue 集合；各种计算表达式（工作流）；以及“工作流集合”——已调整为在工作流内无缝工作的集合模块。
- **FSharpx.Extras**（主页）。FSharpx.Extras 是 FSharpx 系列库的一部分。它实现了几个标准 monad（State、Reader、Writer、Either、Continuation、Distribution）、使用应用函子进行验证、flip 等通用函数和一些异步编程实用程序，以及使 C# - F# 互操作更容易的函数。

例如，我在本文中实现的单子遍历 `List.traverseResultM` 已经在 ExtCore 中可用。

如果你喜欢这个系列，我在我的“弗兰肯芬顿博士与单子”系列文章中有关于状态单子的解释，在我的演讲“面向铁路的编程”中有关于 Either 单子的解释。

正如我一开始所说，写这篇文章对我来说也是一个学习过程。我不是专家，所以如果我犯了任何错误，请告诉我。

谢谢！

## 系列内容

以下是本系列中提到的各种函数的快捷方式列表：

- **第 1 部分：提升到更高的世界**
  - `map` 函数
  - `return` 函数
  - `apply` 函数
  - `liftN` 系列函数
  - `zip` 函数和 ZipList 世界
- **第 2 部分：如何构建跨世界函数**
  - `bind` 函数
  - List 不是单子。Option 不是单子。
- **第 3 部分：在实践中使用核心函数**
  - 独立和依赖数据
  - 示例：使用应用函子风格和单子风格进行验证
  - 迈向一致的世界
  - Kleisli 世界
- **第 4 部分：混合列表和提升值**
  - 混合列表和提升值
  - `traverse` / `MapM` 函数
  - `sequence` 函数
  - “序列”作为ad-hoc实现的配方
  - 可读性与性能
  - 老兄，我的 `filter` 在哪里？
- **第 5 部分：使用所有技术的真实世界示例**
  - 示例：下载和处理网站列表
  - 将两个世界视为一体
- **第 6 部分：设计你自己的提升世界**
  - 设计你自己的提升世界
  - 过滤掉故障
  - Reader monad
- **第 7 部分：总结**
  - 提到的操作符名单
  - 进一步阅读

## 附录：提及的操作符名单

与面向对象语言不同，函数式编程语言以其奇怪的运算符而闻名，因此我认为记录本系列中使用的运算符并提供相关讨论的链接会有所帮助。

| 操作符 | 等效函数              | 讨论                                                         |
| ------ | --------------------- | ------------------------------------------------------------ |
| `>>`   | 从左到右组合          | Not part of this series, but [discussed here](https://fsharpforfunandprofit.com/posts/function-composition/) |
| `<<`   | 从右到左组合          | 同上                                                         |
| `|>`   | 从左到右管道          |                                                              |
| `<|`   | 从右到左管道          |                                                              |
| `<!>`  | `map`                 | [Discussed here](https://fsharpforfunandprofit.com/posts/elevated-world/#map) |
| `<$>`  | `map`                 | 用于 map 的 Haskell 运算符，但在 F# 中不是有效的运算符，所以我使用了`<!>`在这个系列中。 |
| `<*>`  | `apply`               | [Discussed here](https://fsharpforfunandprofit.com/posts/elevated-world/#apply) |
| `<*`   | -                     | One sided combiner. [Discussed here](https://fsharpforfunandprofit.com/posts/elevated-world/#lift) |
| `*>`   | -                     | One sided combiner. [Discussed here](https://fsharpforfunandprofit.com/posts/elevated-world/#lift) |
| `>>=`  | 从左到右 `bind`       | [Discussed here](https://fsharpforfunandprofit.com/posts/elevated-world-2/#bind) |
| `=<<`  | 从右到左 `bind`       | 同上                                                         |
| `>=>`  | 从左到右 Kleisli 组合 | [Discussed here](https://fsharpforfunandprofit.com/posts/elevated-world-3/#kleisli) |
| `<=<`  | 从右到左 Kleisli 组合 | 同上                                                         |

## 附录：进一步阅读

替代教程：

- 你本可以发明单子的！（也许你已经有了）。
- 图片中的函数、应用程序和单子。
- [Kleisli composition à la Up-Goer Five](http://mergeconflict.com/kleisli-composition-a-la-up-goer-five/)。我觉得这个很有趣。
- Eric Lippert 的 C# 单子系列。

对于有学术头脑的人：

- 函数式编程单子（PDF），作者 Philip Wadler。最早的单子论文之一。
- Conor McBride 和 Ross Paterson的《带效果的应用函子设计》（PDF）。
- 《迭代器模式的本质》（PDF），作者：Jeremy Gibbons 和 Bruno Oliveira。

F# 示例：

- F# ExtCore 和 FSharpx.Extras 有很多有用的代码。
- FSharpx.Async 为 Async 提供了 `map`、`apply`、`liftN`（称为“Parallel”）、`bind` 和其他有用的扩展。
- 应用程序非常适合解析，如下文所述：
  - 在 F# 中使用应用函数进行解析。
  - 深入了解解析器组合子：在 Kiln 中使用 F# 和 FParsec 解析搜索查询。