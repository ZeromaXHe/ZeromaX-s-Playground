# [返回主 Markdown](./FSharpForFunAndProfit翻译.md)



# 1 来自地狱的企业开发者

*Part of the "Property Based Testing" series (*[link](https://fsharpforfunandprofit.com/posts/property-based-testing/#series-toc)*)*

发现恶意遵守基于属性的测试
01十二月2014这篇文章已经超过3岁了

https://fsharpforfunandprofit.com/posts/property-based-testing/

> 这篇文章是 [2014 年英语 F# 降临日历](https://sergeytihon.wordpress.com/2014/11/24/f-advent-calendar-in-english-2014/)项目的一部分。查看那里的所有其他精彩帖子！特别感谢Sergey Tihon组织这次活动。

*更新：我根据这些帖子做了一个关于基于属性的测试的演讲。[幻灯片和视频在这里](https://fsharpforfunandprofit.com/pbt/)。*

*此外，现在有一篇关于[如何为基于属性的测试选择属性](https://fsharpforfunandprofit.com/posts/property-based-testing-2/)的帖子*

让我们从我可能曾经讨论过的一个话题开始（话题改为保护罪犯）：

> 我对同事说：“我们需要一个将两个数字相加的函数，你介意实现它吗？”
>
> （不久之后）
>
> 同事：“我刚刚完成了‘add’函数的实现”
>
> 我：“太好了，你为它写过单元测试吗？”
>
> 同事：“你也想做检查吗？”（翻白眼）“好的。”
>
> （不久之后）
>
> 同事：“我刚刚写了一个测试。看！'给定 1 + 2，我预计输出为 3'。”
>
> 同事：“那么，我们现在可以称之为完成了吗？”
>
> 我：“好吧，这只是一个测试。你怎么知道其他输入不会失败？”
>
> 同事：“好的，让我再做一个。”
>
> （不久之后）
>
> 同事：“我刚刚写了另一个很棒的测试。'给定 2 + 2，我预计输出为 4'”
>
> 我：“是的，但你仍然只测试特殊情况。你怎么知道它不会对你没有想到的其他输入失败？”
>
> 同事：“你还想做更多的测试吗？”（低声咕哝着“奴隶司机”，然后走开）

但说真的，我想象中的同事的抱怨是有道理的：*多少次测试就足够了？*

所以现在想象一下，你不是一名开发人员，而是一名测试工程师，负责测试“添加”功能是否正确实现。

不幸的是，这个实现是由一个精疲力竭、总是懒惰且经常恶意的程序员编写的，我称他为“地狱企业开发人员”，或“EDFH”。（EDFH 有一个[你可能听说过的堂兄](https://en.wikipedia.org/wiki/Bastard_Operator_From_Hell)）。

您正在练习企业风格的测试驱动开发，这意味着您编写了一个测试，然后EDFH实现了通过测试的代码。不幸的是，正如我们将看到的那样，EDFH 喜欢[恶意遵守规定](https://www.reddit.com/r/MaliciousCompliance/top/?sort=top&t=all)。

所以你从这样的测试开始（使用vanilla NUnit风格）：

```F#
[<Test>]
let ``When I add 1 + 2, I expect 3``() =
  let result = add 1 2
  Assert.AreEqual(3,result)
```

EDFH 然后实现如下 `add` 函数：

```F#
let add x y =
  if x=1 && y=2 then
    3
  else
    0
```

你的测试通过了！

当你向 EDFH 投诉时，他们说他们正在正确地进行 TDD，并且只编写了使测试通过的最小代码。

很公平。所以你写了另一个测试：

```F#
[<Test>]
let ``When I add 2 + 2, I expect 4``() =
  let result = add 2 2
  Assert.AreEqual(4,result)
```

当你再次向 EDFH 投诉时，他们指出这种方法实际上是一种最佳实践。显然，它被称为[“转型优先前提”](http://blog.8thlight.com/uncle-bob/2013/05/27/TheTransformationPriorityPremise.html)。

此时，你开始认为 EDFH 是恶意的，这种来回可能会永远持续下去！

## 击败恶意程序员

所以问题是，你可以编写什么样的测试，这样恶意程序员就不会创建错误的实现，即使他们想这样做？

好吧，你可以从一个更大的已知结果列表开始，把它们混合在一起。

```F#
[<Test>]
let ``Add two numbers, expect their sum``() =
  let testData = [ (1,2,3); (2,2,4); (3,5,8); (27,15,42) ]
  for (x,y,expected) in testData do
    let actual = add x y
    Assert.AreEqual(expected,actual)
```

但 EDFH 不知疲倦，并将更新实施情况，以包括所有这些案件。

```F#
let add x y =
  match x,y with
  | 1,2 -> 3
  | 2,2 -> 4
  | 3,5 -> 8
  | 27,15 -> 42
  | _ -> 0 // all other cases
```

一种更好的方法是生成随机数并将其用于输入，这样恶意程序员就不可能提前知道该做什么。

```F#
let rand = System.Random()
let randInt() = rand.Next()

[<Test>]
let ``Add two random numbers, expect their sum``() =
  let x = randInt()
  let y = randInt()
  let expected = x + y
  let actual = add x y
  Assert.AreEqual(expected,actual)
```

如果测试看起来像这样，那么 EDFH 将被迫正确执行添加功能！

最后一个改进是，EDFH 可能只是幸运地选择了偶然有效的数字，所以让我们重复随机数测试几次，比如 100 次。

```F#
[<Test>]
let ``Add two random numbers 100 times, expect their sum``() =
  for _ in [1..100] do
    let x = randInt()
    let y = randInt()
    let expected = x + y
    let actual = add x y
    Assert.AreEqual(expected,actual)
```

现在我们完了！

还是我们？

## 基于属性的测试

只有一个问题。为了测试 `add` 函数，您正在使用 `+` 函数。换句话说，您正在使用一个实现来测试另一个实现。

在某些情况下，这是可以接受的（请参阅以下帖子中“测试神谕”的使用），但总的来说，让你的测试复制你正在测试的代码是一个坏主意！这是浪费时间和精力，现在你有两个实现需要构建和更新。

那么，如果你不能使用 `+` 进行测试，你怎么能测试呢？

答案是创建专注于函数属性（“需求”）的测试。这些属性对于任何正确的实现都应该是正确的。

那么，让我们考虑一下 `add` 函数的属性是什么。

一种开始的方法是考虑 `add` 与其他类似函数的不同之处。

例如，`add` 和 `subtract` 有什么区别？好吧，对于 `substract`，参数的顺序会有所不同，而对于 `add`，则不会。

所以有一个好的属性可以开始。它不依赖于加法本身，但它确实消除了一整类不正确的实现。

```F#
[<Test>]
let addDoesNotDependOnParameterOrder() =
  for _ in [1..100] do
    let x = randInt()
    let y = randInt()
    let result1 = add x y
    let result2 = add y x // reversed params
    Assert.AreEqual(result1,result2)
```

这是一个良好的开端，但这并不能阻止 EDFH。EDFH 仍然可以使用 `x * y` 实现 `add`，并且此测试将通过！

那么，现在 `add` 和 `multiply` 之间的区别是什么呢？加法的真正含义是什么？

我们可以从这样的测试开始，它说 `x + x` 应该与 `x * 2` 相同：

```F#
let result1 = add x x
let result2 = x * 2
Assert.AreEqual(result1,result2)
```

但现在我们假设乘法的存在！我们可以定义一个只依赖于 `add` 本身的属性吗？

一种非常有用的方法是查看当函数重复多次时会发生什么。也就是说，如果你 `add`，然后 `add` 到结果中呢？

这导致了两个 `add 1` 等于一个 `add 2` 的想法。测试如下：

```F#
[<Test>]
let addOneTwiceIsSameAsAddTwo() =
  for _ in [1..100] do
    let x = randInt()
    let y = randInt()
    let result1 = x |> add 1 |> add 1
    let result2 = x |> add 2
    Assert.AreEqual(result1,result2)
```

太棒了！`add` 与此测试完美配合，而 `multiply` 则不然。

但是，请注意，EDFH 仍然可以使用减法实现 `add`，并且此测试将通过！

幸运的是，我们也有上面的“参数顺序”测试。将“参数顺序”和“两次加 1”测试结合起来，应该会缩小范围，这样就只有一个正确的实现，对吧？

提交此测试套件后，我们发现 EDFH 编写了一个通过这两个测试的实现。让我们来看看：

```F#
let add x y = 0  // malicious implementation
```

啊！怎么搞的？我们的方法哪里出了问题？

好吧，我们忘了强制实现实际使用我们生成的随机数！

因此，我们需要确保实现确实对传递给它的参数做了一些事情。我们必须检查结果是否以特定的方式与输入相关联。

在不重新实现我们自己的版本的情况下，`add` 是否有一个微不足道的属性，我们知道答案？

对！

当你把零加到一个数字上时会发生什么？你总是会得到同样的号码。

```F#
[<Test>]
let addZeroIsSameAsDoingNothing() =
  for _ in [1..100] do
    let x = randInt()
    let result1 = x |> add 0
    let result2 = x
    Assert.AreEqual(result1,result2)
```

因此，现在我们有一组属性可用于测试 `add` 的任何实现，并迫使 EDFH 创建正确的实现：

## 重构通用代码

这三个测试中有相当多的重复代码。让我们做一些重构。

首先，我们将编写一个名为 `propertyCheck` 的函数，它的工作是生成 100 对随机整数。

`propertyCheck` 还需要一个参数来测试属性。在这个例子中，`property` 参数将是一个接受两个 int 并返回一个 bool 的函数：

```F#
let propertyCheck property =
  // property has type: int -> int -> bool
  for _ in [1..100] do
    let x = randInt()
    let y = randInt()
    let result = property x y
    Assert.IsTrue(result)
```

有了这个，我们可以通过将属性拉到一个单独的函数中来重新定义其中一个测试，如下所示：

```F#
let commutativeProperty x y =
  let result1 = add x y
  let result2 = add y x // reversed params
  result1 = result2

[<Test>]
let addDoesNotDependOnParameterOrder() =
  propertyCheck commutativeProperty
```

我们也可以对其他两个属性做同样的事情。

重构后，完整的代码看起来像这样：

```F#
let rand = System.Random()
let randInt() = rand.Next()

let add x y = x + y  // correct implementation

let propertyCheck property =
  // property has type: int -> int -> bool
  for _ in [1..100] do
    let x = randInt()
    let y = randInt()
    let result = property x y
    Assert.IsTrue(result)

let commutativeProperty x y =
  let result1 = add x y
  let result2 = add y x // reversed params
  result1 = result2

[<Test>]
let addDoesNotDependOnParameterOrder() =
  propertyCheck commutativeProperty

let add1TwiceIsAdd2Property x _ =
  let result1 = x |> add 1 |> add 1
  let result2 = x |> add 2
  result1 = result2

[<Test>]
let addOneTwiceIsSameAsAddTwo() =
  propertyCheck add1TwiceIsAdd2Property

let identityProperty x _ =
  let result1 = x |> add 0
  result1 = x

[<Test>]
let addZeroIsSameAsDoingNothing() =
  propertyCheck identityProperty
```

## 回顾我们迄今为止所做的工作

我们定义了一组属性，任何 `add` 的实现都应该满足这些属性：

- 参数顺序无关紧要（“可交换性”属性）
- 用 1 `add` 两次和用 2 `add` 一次是一样的
- 添加零不起任何作用（“identity”属性）

这些属性的优点在于，它们适用于所有输入，而不仅仅是特殊的幻数。但更重要的是，它们向我们展示了加法的核心本质。

事实上，您可以采用这种方法得出逻辑结论，并将加法定义为具有这些属性的任何东西。

这正是数学家所做的。如果你[在维基百科上查找加法](https://en.wikipedia.org/wiki/Addition#Properties)，你会发现它完全是根据交换性、结合性、恒等性来定义的。

你会注意到，在我们的实验中，我们没有定义“结合性”，而是创建了一个较弱的属性（`x+1+1 = x+2`）。稍后我们将看到 EDFH 确实可以编写满足此属性的恶意实现，并且结合性更好。

唉，第一次尝试就很难让属性完美，但即便如此，通过使用我们提出的三个属性，我们对实现的正确性有了更高的信心，事实上，我们也学到了一些东西——我们更深入地理解了需求。

## 按特性规范

这样的属性集合可以被视为一种*规范（specification）*。

从历史上看，单元测试以及功能测试也[被用作一种规范](https://en.wikipedia.org/wiki/Unit_testing#Documentation)。但是，使用属性而不是使用“魔法”数据进行测试来规范的方法是一种替代方案，我认为这种方法通常更短，也不那么模糊。

您可能会认为只有数学类型的函数才能以这种方式指定，但在未来的文章中，我们将看到这种方法如何用于测试 web 服务和数据库。

当然，并不是每个业务需求都可以表示为这样的属性，我们不能忽视软件开发的社会成分。在与非技术客户合作时，[示例规范](https://en.wikipedia.org/wiki/Specification_by_example)和领域驱动设计可以发挥宝贵作用。

你也可能认为设计所有这些属性是一项艰巨的工作——你是对的！这是最难的部分。在后续文章中，我将介绍一些建议，以提出可能会在一定程度上减少工作量的属性。

但即使前期投入了额外的努力（顺便说一句，这项活动的技术术语称为“思考问题”），通过自动化测试和明确的规范节省的总体时间也将超过后期的前期成本。

事实上，用于宣传单元测试好处的论点同样可以很好地应用于基于属性的测试！因此，如果一个 TDD 粉丝告诉你，他们没有时间提出基于属性的测试，那么他们可能不会着眼于大局。

## 介绍 FsCheck

我们已经实施了自己的属性检查系统，但存在不少问题：

- 它只适用于整数函数。如果我们能对具有字符串参数的函数或实际上任何类型的参数（包括我们自己定义的参数）使用相同的方法，那就太好了。
- 它只适用于两个参数函数（对于 `adding1TwiceIAdding2OnceProperty` 和 `identity` 属性，我们不得不忽略其中一个）。如果我们能对具有任意数量参数的函数使用相同的方法，那就太好了。
- 当该属性有反例时，我们不知道它是什么！测试失败时没有多大帮助！
- 我们生成的随机数没有日志记录，也无法设置种子，这意味着我们无法轻松调试和复制错误。
- 它是不可配置的。例如，我们不能轻易地将循环数从 100 更改为其他值。

如果有一个框架能为我们完成所有这些，那就太好了！

幸好有！[“QuickCheck”](https://en.wikipedia.org/wiki/QuickCheck)库最初由 Koen Claessen 和 John Hughes 为 Haskell 开发，并已移植到许多其他语言。

F#（以及 C#）中使用的 QuickCheck 版本是 Kurt Schelfthout 创建的优秀的[“FsCheck”](https://fscheck.github.io/FsCheck/)库。虽然基于 Haskell QuickCheck，但它还有一些不错的附加功能，包括与 NUnit 和 xUnit 等测试框架的集成。

那么，让我们看看 FsCheck 将如何做与我们自制的属性测试系统相同的事情。

## 使用 FsCheck 测试加法属性

首先，您需要安装 FsCheck 并加载 DLL

如果你使用的是 F# 5 或更高版本，你可以直接在脚本中引用该包，如下所示：

```F#
#r "nuget:NUnit"
open FsCheck
```

对于旧版本的 F#，您应该手动下载 nuget 包，然后在脚本中引用 DLL：

```F#
// 1) use "nuget install FsCheck" or similar to download
// 2) include your nuget path here
#I "/Users/%USER%/.nuget/packages/fscheck/2.14.4/lib/netstandard2.0"
// 3) reference the DLL
#r "FsCheck.dll"
open FsCheck
```

加载 FsCheck 后，您可以使用 `Check.Quick` 传入任何“属性”函数。现在，我们只说“属性”函数是返回布尔值的任何函数（具有任何参数）。

下面是一个使用名为 `commutativeProperty` 的属性函数的示例

```F#
let add x y = x + y  // correct implementation

let commutativeProperty (x,y) =
  let result1 = add x y
  let result2 = add y x // reversed params
  result1 = result2

// check the property interactively
Check.Quick commutativeProperty
```

下面是一个名为 `adding1TwiceIAddg2OnceProperty` 的属性函数的检查

```F#
let add1TwiceIsAdd2Property x =
  let result1 = x |> add 1 |> add 1
  let result2 = x |> add 2
  result1 = result2

// check the property interactively
Check.Quick add1TwiceIsAdd2Property
```

以及单位元（identity）属性

```F#
let identityProperty x =
  let result1 = x |> add 0
  result1 = x

// check the property interactively
Check.Quick identityProperty
```

如果你以交互方式检查其中一个属性，比如用  `Check.Quick commutativeProperty`，您将看到以下消息：

```
Ok, passed 100 tests.
```

## 使用 FsCheck 查找不满意的属性

让我们看看当我们恶意实现 `add` 时会发生什么。在下面的代码中，EDFH 将加法实现为乘法！

该实现将满足交换属性，但 `adding1TwiceIAdding2OnceProperty` 呢？

```F#
let add x y =
  x * y // malicious implementation

let add1TwiceIsAdd2Property x =
  let result1 = x |> add 1 |> add 1
  let result2 = x |> add 2
  result1 = result2

// check the property interactively
Check.Quick add1TwiceIsAdd2Property
```

FsCheck 的结果是：

```
Falsifiable, after 1 test (1 shrink) (StdGen (1657127138,295941511)):
1
```

这意味着使用 1 作为 `adding1TwiceIAdding2OnceProperty` 的输入将导致 `false`，您可以很容易地看到它确实如此。

## 恶意 EDFH 的回归

通过使用随机测试，我们增加了恶意实现者的难度。他们现在必须改变战术！

EDFH 指出，我们在 `adding1TwiceIAdding2OnceProperty` 中仍在使用一些神奇的数字，即 1 和 2，并决定创建一个利用这一点的实现。他们将对低输入值使用正确的实现，对高输入值使用不正确的实现：

```F#
let add x y =
  if (x < 10) || (y < 10) then
    x + y  // correct for low values
  else
    x * y  // incorrect for high values
```

哦，不！如果我们重新测试所有属性，它们现在都通过了！

这将教会我们在测试中使用魔法数！

还有别的选择吗？好吧，让我们借鉴数学家的经验，创建一个关联属性测试。

```F#
let associativeProperty x y z =
  let result1 = add x (add y z)    // x + (y + z)
  let result2 = add (add x y) z    // (x + y) + z
  result1 = result2

// check the property interactively
Check.Quick associativeProperty
```

啊哈！现在我们得到一个伪造：

```
Falsifiable, after 38 tests (4 shrinks) (StdGen (127898154,295941554)):
8
2
10
```

这意味着使用 `(8+2)+10` 与 `8+(2+10)` 不同。

请注意，FsCheck不仅发现了一些破坏属性的输入，而且找到了一个最低的例子。它知道输入 `8,2,9` 通过，但向上一个（`8,2,10`）失败。太好了！

## 摘要

在这篇文章中，我向您介绍了基于属性的测试的基础知识，以及它与熟悉的基于示例的测试有何不同。

我还介绍了 EDFH 的概念，一个邪恶的恶意程序员。你可能会认为这样一个恶意的程序员是不切实际的，而且是过分的。

但在许多情况下，你的行为就像一个无意中恶意的程序员。你愉快地创建了一个适用于某些特殊情况的实现，但并不适用于更普遍的情况，不是出于恶意，而是出于无知和盲目。

就像鱼不知道水一样，我们往往不知道我们所做的假设。基于属性的测试可以迫使我们意识到它们。

但是，像 FsCheck 这样的基于属性的测试库实际上是如何详细工作的呢？这是[下一篇文章](https://fsharpforfunandprofit.com/posts/property-based-testing-1/)的主题。

> 本文中使用的源代码可以在[这里找到](https://github.com/swlaschin/fsharpforfunandprofit.com_code/tree/master/posts/property-based-testing)。



# 2 了解 FsCheck

生成器、收缩器等
2014年12月2日这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/property-based-testing-1/

*更新：我根据这些帖子做了一个关于基于属性的测试的演讲。[幻灯片和视频在这里](https://fsharpforfunandprofit.com/pbt/)。*

在[上一篇文章](https://fsharpforfunandprofit.com/posts/property-based-testing/)中，我介绍了基于属性的测试的基础知识，并展示了如何通过生成随机测试来节省大量时间。

但它实际上是如何详细工作的？这就是这篇文章的主题。

## 了解 FsCheck：生成器

FsCheck 做的第一件事是为您生成随机输入。这被称为“生成”，每种类型都有一个相关的“生成器”。

```F#
// get the generator for ints
let intGenerator = Arb.generate<int>
```

`Arb` 是“任意（arbitrary）”的缩写，`Arb.generator<T>` 将返回任何类型 `T` 的生成器。

为了从生成器中获取一些样本数据，我们可以使用 `Gen.sample` 函数。您需要传入一个生成器以及两个参数：列表中的元素数量和“大小”。

“大小”的确切含义取决于生成的类型和上下文。“size”的例子有：int 的最大值；列表的长度；树的深度；等。

```F#
// generate three ints with a maximum size of 1
Gen.sample 1 3 intGenerator    // e.g. [0; 0; -1]

// generate three ints with a maximum size of 10
Gen.sample 10 3 intGenerator   // e.g. [-4; 8; 5]

// generate three ints with a maximum size of 100
Gen.sample 100 3 intGenerator  // e.g. [-37; 24; -62]
```

在这个例子中，整数不是均匀生成的，而是聚集在零附近。您可以通过一小段代码亲自看到这一点：

```F#
// see how the values are clustered around the center point
intGenerator
|> Gen.sample 10 1000
|> Seq.groupBy id  // use the generated number as key
|> Seq.map (fun (k,v) -> (k,Seq.length v)) // count the occurences
|> Seq.sortBy fst  // sort by key
|> Seq.toList
```

结果是这样的：

```F#
// the (key, count) pairs
// see how the values are clustered around the center point of 0
[(-10, 3); (-9, 14); (-8, 18); (-7, 10); (-6, 27);
  (-5, 42); (-4, 49); (-3, 56); (-2, 76); (-1, 119);
  (0, 181); (1, 104); (2, 77); (3, 62); (4, 47); (5, 44);
  (6, 26); (7, 16); (8, 14); (9, 12); (10, 3)]
```

您可以看到，大多数值都在中心（0 生成 181 次，1 生成 104 次），而异常值很少（10 仅生成 3 次）。

你也可以用更大的样本重复。这个生成范围为 [-30,30] 的 10000 个元素

```F#
intGenerator
|> Gen.sample 30 10000
|> Seq.groupBy id
|> Seq.map (fun (k,v) -> (k,Seq.length v))
|> Seq.sortBy (fun (k,v) -> k)
|> Seq.toList
```

同样，大多数数字将在零左右。

除了 `Gen.sample` 之外，还有许多其他生成器函数可用（[更多文档请点击此处](https://fscheck.github.io/FsCheck//TestData.html)）。

## 理解 FsCheck：自动生成各种类型

生成器逻辑的优点是它也会自动生成复合值。

例如，这里有一个三个整数元组的生成器：

```F#
let tupleGenerator = Arb.generate<int*int*int>

// generate 3 tuples with a maximum size of 1
Gen.sample 1 3 tupleGenerator
// result: [(0, 0, 0); (0, 0, 0); (0, 1, -1)]

// generate 3 tuples with a maximum size of 10
Gen.sample 10 3 tupleGenerator
// result: [(-6, -4, 1); (2, -2, 8); (1, -4, 5)]

// generate 3 tuples with a maximum size of 100
Gen.sample 100 3 tupleGenerator
// result: [(-2, -36, -51); (-5, 33, 29); (13, 22, -16)]
```

一旦你有了基本类型的生成器，`option` 和 `list` 生成器就会随之而来。这是一个 `int option`s 生成器：

```F#
let intOptionGenerator = Arb.generate<int option>
// generate 10 int options with a maximum size of 5
Gen.sample 5 10 intOptionGenerator
// result:  [Some 0; Some -1; Some 2; Some 0; Some 0;
//           Some -4; null; Some 2; Some -2; Some 0]
```

这里有一个 `int list`s 生成器：

```F#
let intListGenerator = Arb.generate<int list>
// generate 10 int lists with a maximum size of 5
Gen.sample 5 10 intListGenerator
// result:  [ []; []; [-4]; [0; 3; -1; 2]; [1];
//            [1]; []; [0; 1; -2]; []; [-1; -2]]
```

当然，你可以生成随机字符串。

```F#
let stringGenerator = Arb.generate<string>

// generate 3 strings with a maximum size of 1
Gen.sample 1 3 stringGenerator
// result: [""; "!"; "I"]

// generate 3 strings with a maximum size of 10
Gen.sample 10 3 stringGenerator
// result: [""; "eiX$a^"; "U%0Ika&r"]
```

您也可以从用户定义的类型生成随机值，如下所示：

```F#
type Color = Red | Green of int | Blue of bool

let colorGenerator = Arb.generate<Color>

// generate 10 colors with a maximum size of 50
Gen.sample 50 10 colorGenerator

// result:  [Green -47; Red; Red; Red; Blue true;
//           Green 2; Blue false; Red; Blue true; Green -12]
```

这里有一个为用户定义的记录类型生成随机值的方法，该记录类型包含另一个用户定义的类型。

```F#
type Point = {x:int; y:int; color: Color}

let pointGenerator = Arb.generate<Point>

// generate 10 points with a maximum size of 50
Gen.sample 50 10 pointGenerator

(* result
[{x = -8; y = 12; color = Green -4;};
  {x = 28; y = -31; color = Green -6;};
  {x = 11; y = 27; color = Red;};
  {x = -2; y = -13; color = Red;};
  {x = 6; y = 12; color = Red;};
  // etc
*)
```

有一些方法可以对类型的生成方式进行更细粒度的控制，但这必须等待另一篇文章。

## 理解 FsCheck：收缩

像 FsCheck 这样的基于属性的测试工具的一个很酷的地方是，它将尝试为属性创建最小的反例——这被称为“收缩”。

那么收缩是如何工作的呢？

FsCheck 使用的流程有两个部分：

首先，它生成一系列随机输入，从小开始，逐渐变大。这是如上所述的“生成器”阶段。

如果任何输入导致属性失败，它将开始“收缩”第一个参数以找到较小的数字。缩小的确切过程因类型而异（你也可以覆盖它），但假设对于数字，它们会以一种合理的方式变小。

例如，假设你有一个愚蠢的属性 `isSmallerThan80`：

```F#
let isSmallerThan80 x = x < 80
```

您生成了随机数，发现 100 的属性失败，您想尝试一个较小的数字。`Arb.shrink` 将生成一个整数序列，所有整数都小于 100。每种方法都会依次对属性进行尝试，直到属性再次失败。

```F#
isSmallerThan80 100 // false, so start shrinking

Arb.shrink 100 |> Seq.toList
//  [0; 50; 75; 88; 94; 97; 99]
```

对于列表中的每个元素，测试其属性，直到发现另一个失败：

```F#
isSmallerThan80 0 // true
isSmallerThan80 50 // true
isSmallerThan80 75 // true
isSmallerThan80 88 // false, so shrink again
```

该属性以 `88` 失败，因此以 88 为起点再次收缩：

```F#
Arb.shrink 88 |> Seq.toList
//  [0; 44; 66; 77; 83; 86; 87]
isSmallerThan80 0 // true
isSmallerThan80 44 // true
isSmallerThan80 66 // true
isSmallerThan80 77 // true
isSmallerThan80 83 // false, so shrink again
```

该属性现在以 `83` 失败，因此再次以 83 为起点收缩：

```F#
Arb.shrink 83 |> Seq.toList
//  [0; 42; 63; 73; 78; 81; 82]
// smallest failure is 81, so shrink again
```

该属性以 `81` 失败，因此以 81 为起点再次收缩：

```F#
Arb.shrink 81 |> Seq.toList
//  [0; 41; 61; 71; 76; 79; 80]
// smallest failure is 80
```

在这一点之后，缩小到 80 不起作用——找不到更小的值。

在这种情况下，FsCheck将报告 `80` 是伪造属性的最小值，需要 4 次收缩。

与生成器一样，FsCheck 几乎可以为任何类型生成收缩序列：

```F#
Arb.shrink (1,2,3) |> Seq.toList
//  [(0, 2, 3); (1, 0, 3); (1, 1, 3);
//   (1, 2, 0); (1, 2, 2)]

Arb.shrink "abcd" |> Seq.toList
//  ["bcd"; "acd"; "abd"; "abc"; "abca";
//   "abcb"; "abcc"; "abad"; "abbd"; "aacd"]

Arb.shrink [1;2;3] |> Seq.toList
//  [[2; 3]; [1; 3]; [1; 2]; [1; 2; 0]; [1; 2; 2];
//  [1; 0; 3]; [1; 1; 3]; [0; 2; 3]]
```

而且，与生成器一样，如果需要，可以定制收缩的工作方式。

## 配置 FsCheck：更改测试数量

我在上面提到一个愚蠢的属性 `isSmallerThan80`。让我们实际尝试一下，看看 FsCheck 是如何使用它的。

```F#
// silly property to test
let isSmallerThan80 x = x < 80

Check.Quick isSmallerThan80
// result: Ok, passed 100 tests.
```

哦，天哪！FsCheck 未找到反例！我们知道属性应该失败，但我们也知道大多数整数将在零附近生成。也许我们应该告诉FsCheck生成更多的数字？

我们通过更改默认（“Quick”）配置来实现这一点。我们可以设置一个名为 `MaxTest` 的字段。默认值为 100，因此让我们将其增加到 1000。

要使用特定的配置，我们需要使用 `Check.One(config,property)`，而不仅仅是 `Check.Quick(property)`。

```F#
let config = {
  Config.Quick with
    MaxTest = 1000
  }
Check.One(config,isSmallerThan80 )
// result: Ok, passed 1000 tests.
```

哎呀！FsCheck 也没有找到包含 1000 个测试的反例！让我们再次尝试 10000 个测试：

```F#
let config = {
  Config.Quick with
    MaxTest = 10000
  }
Check.One(config,isSmallerThan80 )
// result: Falsifiable, after 8660 tests (1 shrink):
//         80
```

好的，我们终于成功了。但为什么要做这么多测试呢？

答案在于其他一些配置设置：`StartSize` 和 `EndSize`。

记住，生成器从小数字开始，然后逐渐增加。这由 `StartSize` 和 `EndSize` 设置控制。默认情况下，`StartSize` 为 1，`EndSize` 为 100。因此，在测试结束时，生成器的“大小”参数将为 100。

但是，正如我们所看到的，即使大小为 100，在极端情况下也很少生成数字。在这种情况下，这意味着不太可能生成大于 80 的数字。

所以，让我们将 `EndSize` 更改为更大的值，看看会发生什么！

```F#
let config = {
  Config.Quick with
    EndSize = 1000
  }
Check.One(config,isSmallerThan80 )
// result: Falsifiable, after 21 tests (4 shrinks):
//         80
```

更像是这样！现在只需要 21 次测试，而不是 8660 次测试！

这个故事的寓意是：了解你的属性的领域，并适当地配置生成器，否则你可能永远不会生成相关的输入。

## 配置 FsCheck：详细模式和日志记录

我提到过 FsCheck 比原产（home-grown）解决方案的一个好处是日志记录和可重复性，所以让我们来看看。

假设 EDFH 已经实现了“边界”为 `25` 的 `add` 函数。在此限制内，`add` 将正常工作，但在此限制之外，`add` 将具有恶意实现。

让我们看看 FsCheck 是如何通过日志检测这个边界的。

```F#
let add x y =
  if (x < 25) || (y < 25) then
    x + y  // correct for low values
  else
    x * y  // incorrect for high values

let associativeProperty x y z =
  let result1 = add x (add y z)    // x + (y + z)
  let result2 = add (add x y) z    // (x + y) + z
  result1 = result2

// check the property interactively
Check.Quick associativeProperty
```

结果是：

```
Falsifiable, after 66 tests (12 shrinks):
1
24
25
```

FsCheck 再次发现输入 `1`、`24` 和 `25` 失败。它很快发现 `25` 是精确的边界点。但它是怎么做到的？

首先，查看 FsCheck 正在做什么的最简单方法是使用“详细”模式。也就是说，使用 `Check.Verbose` 而不是 `Check.Quick`：

```F#
// check the property interactively
Check.Quick associativeProperty

// with tracing/logging
Check.Verbose associativeProperty
```

执行此操作时，您将看到如下所示的输出。我添加了所有的评论来解释各种元素。

```F#
0:    // test #0
-1    // generated parameter #1 ("x")
-1    // generated parameter #2 ("y")
0     // generated parameter #3 ("z")
//       associativeProperty(-1,-1,0) => true, keep going
1:    // test #1
0
0
0     // associativeProperty 0 0 0  => true, keep going
2:    // test #2
-2
0
-3    // associativeProperty -2 0 -3  => true, keep going
3:    // test #3
1
2
0     // associativeProperty 1 2 0  => true, keep going
// etc
49:   // test #49
46
-4
50    // associativeProperty 46 -4 50  => false, start shrinking
// etc
shrink:
35
-4
50    // associativeProperty 35 -4 50  => false, keep shrinking
shrink:
27
-4
50    // associativeProperty 27 -4 50  => false, keep shrinking
// etc
shrink:
25
1
29    // associativeProperty 25 1 29  => false, keep shrinking
shrink:
25
1
26    // associativeProperty 25 1 26  => false, keep shrinking
// next shrink fails
Falsifiable, after 50 tests (10 shrinks) (StdGen (995282583,295941602)):
25
1
26
```

这是一次不同的运行，因此最终答案与之前不同——25,1,26——但它仍然检测到 25 处的边界。

这个显示器占用了很多空间！我们能把它做得更紧凑吗？

是的，您可以通过编写自己的自定义函数，并通过其 `Config` 结构告诉 FsCheck 使用它们来控制每个测试和收缩的显示方式。

这些函数是通用的，参数列表由未知长度列表（`obj list`）表示。但是，由于我知道我正在测试一个三参数属性，我可以硬编码一个三元素列表参数，并将它们全部打印在一行上。

```F#
// create a function for displaying a test
let printTest testNum [x;y;z] =
  sprintf "#%-3i %3O %3O %3O\n" testNum x y z

// create a function for displaying a shrink
let printShrink [x;y;z] =
  sprintf "shrink %3O %3O %3O\n" x y z
```

该配置还有一个名为 `Replay` 的插槽，通常为 `None`，这意味着每次运行都是不同的。但是，如果将 `Replay` 设置为 `Some seed`，则测试将以完全相同的方式重播。种子看起来像 `StdGen (someInt,someInt)`，每次运行时都会打印出来，所以如果你想保留一次运行，你需要做的就是将种子粘贴到配置中。

同样，要使用特定的配置，您需要使用 `Check.One(config,property)`，而不仅仅是 `Check.Quick(property)`。

这是更改了默认跟踪函数并显式设置了重播种子的代码。

```F#
// create a new FsCheck configuration
let config = {
  Config.Quick with
    Replay = Random.StdGen (995282583,295941602) |> Some
    Every = printTest
    EveryShrink = printShrink
  }

// check the given property with the new configuration
Check.One(config,associativeProperty)
```

输出现在更加紧凑，看起来像这样：

```F#
#0    -1  -1   0
#1     0   0   0
#2    -2   0  -3
#3     1   2   0
#4    -4   2  -3
#5     3   0  -3
#6    -1  -1  -1
// etc
#46  -21 -25  29
#47  -10  -7 -13
#48   -4 -19  23
#49   46  -4  50
// start shrinking first parameter
shrink  35  -4  50
shrink  27  -4  50
shrink  26  -4  50
shrink  25  -4  50
// start shrinking second parameter
shrink  25   4  50
shrink  25   2  50
shrink  25   1  50
// start shrinking third parameter
shrink  25   1  38
shrink  25   1  29
shrink  25   1  26
Falsifiable, after 50 tests (10 shrinks) (StdGen (995282583,295941602)):
25
1
26
```

好了，如果需要，可以很容易地自定义 FsCheck 日志记录。

### 一个真实世界的收缩例子

在前面的例子中，我们看到了一个现实世界中的收缩例子。那么，让我们详细看看收缩是如何完成的。

测试 #49 的最后一组输入（`46,-4,50`）为假，因此触发了收缩开始。我们首先缩小第一个数字 `46`。

```F#
// The last set of inputs (46,-4,50) was false, so shrinking started
associativeProperty 46 -4 50  // false, so shrink

// list of possible shrinks starting at 46
Arb.shrink 46 |> Seq.toList
// result [0; 23; 35; 41; 44; 45]
```

我们将遍历列表 `[0; 23; 35; 41; 44; 45]` ，在导致属性失败的第一个元素处停止：

```F#
// find the next test that fails when shrinking the x parameter
let x,y,z = (46,-4,50)
Arb.shrink x
|> Seq.tryPick (fun x ->
  if associativeProperty x y z then None else Some (x,y,z) )
// answer (35, -4, 50)
```

收缩列表中导致失败的第一个元素是 `x=35`，作为输入的一部分 `(35, -4, 50)`。

所以现在我们从 35 岁开始缩小：

```F#
// find the next test that fails when shrinking the x parameter
let x,y,z = (35,-4,50)
Arb.shrink x
|> Seq.tryPick (fun x ->
  if associativeProperty x y z then None else Some (x,y,z) )
// answer (27, -4, 50)
```

导致失败的第一个元素现在是 `x=27`，作为输入的一部分 `(27, -4, 50)`。

所以现在我们从 27 岁开始，继续前进：

```F#
// find the next test that fails when shrinking the x parameter
let x,y,z = (27,-4,50)
Arb.shrink x
|> Seq.tryPick (fun x ->
  if associativeProperty x y z then None else Some (x,y,z) )
// answer (26, -4, 50)

// find the next test that fails when shrinking the x parameter
let x,y,z = (26,-4,50)
Arb.shrink x
|> Seq.tryPick (fun x ->
  if associativeProperty x y z then None else Some (x,y,z) )
// answer (25, -4, 50)

// find the next test that fails when shrinking the x parameter
let x,y,z = (25,-4,50)
Arb.shrink x
|> Seq.tryPick (fun x ->
  if associativeProperty x y z then None else Some (x,y,z) )
// answer None
```

此时，`x=25` 是你能达到的最低值。其收缩序列均未导致失败。所以我们完成了 `x` 参数！

现在，我们只需使用 `y` 参数以相同的方式重复此过程，从 `-4` 开始。

```F#
// find the next test that fails when shrinking the y parameter
let x,y,z = (25,-4,50)
Arb.shrink y
|> Seq.tryPick (fun y ->
  if associativeProperty x y z then None else Some (x,y,z) )
// answer (25, 4, 50)

// find the next test that fails when shrinking the y parameter
let x,y,z = (25,4,50)
Arb.shrink y
|> Seq.tryPick (fun y ->
  if associativeProperty x y z then None else Some (x,y,z) )
// answer (25, 2, 50)

// find the next test that fails when shrinking the y parameter
let x,y,z = (25,2,50)
Arb.shrink y
|> Seq.tryPick (fun y ->
  if associativeProperty x y z then None else Some (x,y,z) )
// answer (25, 1, 50)

// find the next test that fails when shrinking the y parameter
let x,y,z = (25,1,50)
Arb.shrink y
|> Seq.tryPick (fun y ->
  if associativeProperty x y z then None else Some (x,y,z) )
// answer None
```

此时，`y=1` 是你能达到的最低值。其收缩序列均未导致失败。所以我们完成了 `y` 参数！

最后，我们用 `z` 参数重复这个过程。

```F#
// find the next test that fails when shrinking the z parameter
let x,y,z = (25,1,50)
Arb.shrink z
|> Seq.tryPick (fun z ->
  if associativeProperty x y z then None else Some (x,y,z) )
// answer (25, 1, 38)

// find the next test that fails when shrinking the z parameter
let x,y,z = (25,1,38)
Arb.shrink z
|> Seq.tryPick (fun z ->
  if associativeProperty x y z then None else Some (x,y,z) )
// answer (25, 1, 29)

// find the next test that fails when shrinking the z parameter
let x,y,z = (25,1,29)
Arb.shrink z
|> Seq.tryPick (fun z ->
  if associativeProperty x y z then None else Some (x,y,z) )
// answer (25, 1, 26)

// find the next test that fails when shrinking the z parameter
let x,y,z = (25,1,26)
Arb.shrink z
|> Seq.tryPick (fun z ->
  if associativeProperty x y z then None else Some (x,y,z) )
// answer None
```

现在我们已经完成了所有的参数！

收缩后的最后一个反例是 `(25,1,26)`。

## 添加先决条件

假设我们有一个新的想法来检查一个属性。我们将创建一个名为 `addition is not multiplication`（“加法不是乘法”）的属性，这将有助于阻止实现中的任何恶意（甚至意外）混淆。

这是我们的第一次尝试：

```F#
let additionIsNotMultiplication x y =
  x + y <> x * y
```

但是当我们运行这个测试时，我们失败了！

```F#
Check.Quick additionIsNotMultiplication
// Falsifiable, after 3 tests (0 shrinks):
// 0
// 0
```

当然，`0+0` 和 `0*0` 是相等的。但是，我们如何告诉 FsCheck 忽略这些输入，而忽略所有其他输入呢？

这是通过一个“条件”或过滤器表达式完成的，该表达式使用 `==>`（FsCheck 定义的运算符）作为属性函数的前缀。

这里有一个例子：

```F#
let additionIsNotMultiplication x y =
  x + y <> x * y

let preCondition x y =
  (x,y) <> (0,0)

let additionIsNotMultiplication_withPreCondition x y =
  preCondition x y ==> additionIsNotMultiplication x y
```

新属性是 `additionIsNotmultiplexionwithPreCondition`，可以就像任何其他属性一样传递给  `Check.Quick`。

```F#
Check.Quick additionIsNotMultiplication_withPreCondition
// Falsifiable, after 38 tests (0 shrinks):
// 2
// 2
```

哎呀！我们又忘了一个案例！`2+2` 等于 `2*2`。让我们再次修正我们的前提条件：

```F#
let preCondition x y =
  (x,y) <> (0,0)
  && (x,y) <> (2,2)

let additionIsNotMultiplication_withPreCondition x y =
  preCondition x y ==> additionIsNotMultiplication x y
```

现在，这奏效了。

```F#
Check.Quick additionIsNotMultiplication_withPreCondition
// Ok, passed 100 tests.
```

只有当你想过滤掉少数情况时，才应该使用这种先决条件。

如果大多数输入无效，那么这种过滤将是昂贵的。在这种情况下，有一种更好的方法，这将在以后的文章中讨论。

FsCheck 文档详细介绍了如何[调整属性，在此处](https://fscheck.github.io/FsCheck//Properties.html)。

## 属性的命名约定

这些属性函数的用途与“普通”函数不同，那么我们应该如何命名它们呢？

在 Haskell 和 Erlang 世界中，属性按照惯例被赋予 `prop_` 前缀。在 .NET 世界中，更常见的是使用像 `AbcProperty` 这样的后缀。

此外，在 F# 中，我们有命名空间、模块和属性（如 `[<Test>]`），可以用来组织属性并将其与其他函数区分开来。

## 组合多个属性

一旦你有了一组属性，你就可以通过将它们添加为类类型的静态成员，将它们组合成一个组（甚至，天哪，一个规范！）。

然后，您可以进行 `Check.QuickAll` 并传入类的名称。

例如，以下是我们的三个附加属性：

```F#
let add x y = x + y // good implementation

let commutativeProperty x y =
  add x y = add y x

let associativeProperty x y z =
  add x (add y z) = add (add x y) z

let leftIdentityProperty x =
  add x 0 = x

let rightIdentityProperty x =
  add 0 x = x
```

这是与 `Check.QuickAll` 一起使用的相应静态类：

```F#
type AdditionSpecification =
  static member ``Commutative`` x y =
    commutativeProperty x y
  static member ``Associative`` x y z =
    associativeProperty x y z
  static member ``Left Identity`` x =
    leftIdentityProperty x
  static member ``Right Identity`` x =
    rightIdentityProperty x

Check.QuickAll<AdditionSpecification>()
```

运行 `QuickAll<AdditionSpecification>` 的结果是：

```
--- Checking AdditionSpecification ---
AdditionSpecification.Commutative-Ok, passed 100 tests.
AdditionSpecification.Associative-Ok, passed 100 tests.
AdditionSpecification.Left Identity-Ok, passed 100 tests.
AdditionSpecification.Right Identity-Ok, passed 100 tests.
```

正如您所看到的，所有测试都通过了。请尝试更改 `add` 的实现并重新运行测试！

## 将基于属性的测试与基于示例的测试相结合

在上一篇文章中，我们表明基于示例的测试有一个弱点，即它们只测试了输入空间的一小部分，并且可能被恶意 EDFH 绕过，或者更常见的是，通过忽略异常输入。

然而，我确实认为基于示例的测试可以补充基于属性的测试。

基于示例的测试通常更容易理解，因为它不那么抽象，因此提供了一个很好的切入点和与属性相关的文档。

以下是在同一代码块中混合属性和基于示例的测试的示例：

```F#
type AdditionSpecification =

  // some properties
  static member ``Commutative`` x y =
    commutativeProperty x y
  static member ``Associative`` x y z =
    associativeProperty x y z
  static member ``Left Identity`` x =
    leftIdentityProperty x
  static member ``Right Identity`` x =
    rightIdentityProperty x

  // some example-based tests as well
  static member ``1 + 2 = 3``() =
    add 1 2 = 3

  static member ``1 + 2 = 2 + 1``() =
    add 1 2 = add 2 1

  static member ``42 + 0 = 0 + 42``() =
    add 42 0 = add 0 42
```

## 使用 NUnit 的 FsCheck

您可以使用 NUnit 和其他测试框架中的 FsCheck，并使用额外的插件（例如 NUnit 的 `FsCheck.NUnit`）。

```F#
#r "nuget:FsCheck.NUnit"
open FsCheck.NUnit
```

您可以使用 `Property` 属性，而不是用 `Test` 或 `Fact` 标记测试。与常规测试不同，这些测试可以有参数！

以下是为在 NUnit 中工作而编写的一些测试的示例：

```F#
open NUnit.Framework
open FsCheck
open FsCheck.NUnit

[<Property(QuietOnSuccess = true)>]
let ``Commutative`` x y =
  commutativeProperty x y

[<Property(Verbose= true)>]
let ``Associative`` x y z =
  associativeProperty x y z

[<Property(EndSize=300)>]
let ``Left Identity`` x =
  leftIdentityProperty x
```

如您所见，您可以通过注释的属性更改每个测试的配置（如 `Verbose` 和 `EndSize`）。

`QuietOnSuccess` 标志可用于使 FsCheck 与标准测试框架兼容，这些框架在成功时保持沉默，只在出现问题时显示消息。

## 摘要

在这篇文章中，我向您介绍了基于属性的检查的基础知识。

然而，还有更多内容需要涵盖！在以后的文章中，我将介绍以下主题：

- [**如何提出适用于代码的属性**](https://fsharpforfunandprofit.com/posts/property-based-testing-2)。属性不必是数学的。我们将研究更通用的属性，如逆（用于测试序列化/反序列化）、幂等（用于安全处理多个更新或重复消息），并研究测试预言机（test oracles）。
- **如何创建自己的生成器和收缩器**。我们已经看到FsCheck可以很好地生成随机值。但是，对于带有正数、有效电子邮件地址或电话号码等约束的值呢。FsCheck为您提供了构建自己的工具。
- **如何进行基于模型的测试**，特别是如何测试并发问题。

下次再见——测试愉快！

> 本文中使用的源代码可以在[这里找到](https://github.com/swlaschin/fsharpforfunandprofit.com_code/tree/master/posts/property-based-testing-1)。



# 3 为基于属性的测试选择属性

或者，我想使用PBT，但我永远想不出任何性能可以使用
2014年12月12日这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/property-based-testing-2/

*更新：我根据这些帖子做了一个关于基于属性的测试的演讲。[幻灯片和视频在这里](https://fsharpforfunandprofit.com/pbt/)。*

在[前两篇文章](https://fsharpforfunandprofit.com/posts/property-based-testing/)中，我介绍了基于属性的测试的基础知识，并展示了如何通过生成随机测试来节省大量时间。

但这里有一个共同的问题。每个看到 FsCheck 或 QuickCheck 等基于属性的测试工具的人都认为这很神奇……但当开始创建自己的属性时，普遍的抱怨是：“我应该使用哪些属性？我想不出任何属性！”

本文的目的是展示一些常见的模式，帮助您发现适用于代码的属性。

## 属性类别

根据我的经验，使用下面列出的七种方法之一可以发现许多属性。

- [“不同的路径，相同的目的地”](https://fsharpforfunandprofit.com/posts/property-based-testing-2/#different-paths)
- [“来回”](https://fsharpforfunandprofit.com/posts/property-based-testing-2/#there-and-back)
- [“有些事情永远不会改变”](https://fsharpforfunandprofit.com/posts/property-based-testing-2/#some-things-never-change)
- [“事物变化越多，就越保持不变”](https://fsharpforfunandprofit.com/posts/property-based-testing-2/#idempotence)
- [“先解决一个小问题”](https://fsharpforfunandprofit.com/posts/property-based-testing-2/#structural-induction)
- [“难以证明，易于验证”](https://fsharpforfunandprofit.com/posts/property-based-testing-2/#hard-to-prove-easy-to-verify)
- [“测试预言机”](https://fsharpforfunandprofit.com/posts/property-based-testing-2/#test-oracle)

这绝不是一个全面的列表，只是对我最有用的列表。从另一个角度来看，请查看微软 PEX 团队编制的[模式列表](http://research.microsoft.com/en-us/projects/pex/patterns.pdf)。

### “不同的路径，相同的目的地”

这些类型的属性基于以不同顺序组合操作，但得到相同的结果。例如，在下图中，先做 `X`，后做 `Y`，得到的结果与先做 `Y`，后做 `X` 的结果相同。

![Commutative property](https://fsharpforfunandprofit.com/posts/property-based-testing-2/property_commutative.png)

在范畴论中，这被称为*交换图（commutative diagram）*。

加法就是这种模式的一个明显例子。例如，先 `add 1` 后 `add 2` 的结果与先 `add 2` 后 `add 1` 的结果相同。

这种模式可以产生广泛的有用属性。我们将在本文稍后看到这种模式的更多用法。

### “来回”

这些类型的属性基于将操作与其逆操作相结合，最终得到与您开始时相同的值。

在下图中，执行 `X` 将 `ABC` 序列化为某种二进制格式，`X` 的逆是某种反序列化，它再次返回相同的 `ABC` 值。

![Inverse](https://fsharpforfunandprofit.com/posts/property-based-testing-2/property_inverse.png)

除了序列化/反序列化之外，还可以通过这种方式检查其他对操作： `addition`/`subtraction`, `write`/`read`, `setProperty`/`getProperty` 等。

其他函数对也符合这种模式，即使它们不是严格的逆函数对，如 `insert`/`contains`, `create`/`exists` 等。

### “有些事情永远不会改变”

这些类型的属性基于在某些变换后保留的不变量。

在下图中，转换改变了项目的顺序，但之后仍然存在相同的四个项目。

![Invariant](https://fsharpforfunandprofit.com/posts/property-based-testing-2/property_invariant.png)

常见的不变量包括集合的大小（例如 `map`）、集合的内容（例如 `sort`）、与大小成比例的某物的高度或深度（例如平衡树）。

### “事物变化越多，就越保持不变”

这些类型的属性基于“幂等性（idempotence）”，也就是说，执行两次操作与执行一次操作相同。

在下图中，使用 `distinct` 过滤集合会返回两个项目，但执行两次 `distinct` 会再次返回相同的集合。

![Idempotence](https://fsharpforfunandprofit.com/posts/property-based-testing-2/property_idempotence.png)

Idempotence 属性非常有用，可以扩展到数据库更新和消息处理等领域。

### “先解决一个小问题”

这些性质是基于“结构归纳”的——也就是说，如果一个大的东西可以分解成更小的部分，并且一些性质对这些更小的部分是正确的，那么你通常可以证明这个性质对一个大东西也是正确的。

在下图中，我们可以看到，四项列表可以划分为一项加三项列表，而三项列表又可以划分为两项加一项列表。如果我们能证明该属性适用于两项列表，那么我们就可以推断出它适用于三项列表，也适用于四项列表。

![Induction](https://fsharpforfunandprofit.com/posts/property-based-testing-2/property_induction.png)

归纳属性通常自然适用于递归结构，如列表和树。

### “难以证明，易于验证”

通常，找到结果的算法可能很复杂，但验证答案很容易。

在下图中，我们可以看到，在迷宫中找到一条路线很难，但检查它是否有效却很简单！

![Hard to find, easy to verify](https://fsharpforfunandprofit.com/posts/property-based-testing-2/property_easy_verification.png)

许多著名的问题都是这类的，比如素数因式分解。但这种方法甚至可以用于简单的问题。

例如，您可以通过再次连接所有标记来检查字符串标记器是否正常工作。生成的字符串应该与您开始时的字符串相同。

### “测试预言机”

在许多情况下，你通常有一个算法或过程的替代版本（“测试预言机”），可以用来检查你的结果。

![Test Oracle](https://fsharpforfunandprofit.com/posts/property-based-testing-2/property_test_oracle.png)

例如，您可能有一个高性能的算法，其中包含您想要测试的优化调整。在这种情况下，您可以将其与暴力算法进行比较，暴力算法要慢得多，但也更容易正确编写。

同样，您可以将并行或并发算法的结果与线性单线程版本的结果进行比较。

### “基于模型”的测试

“基于模型”的测试是测试预言机的一种变体，我们将在稍后的文章中更详细地讨论。

它的工作方式是，与测试中的（复杂）系统并行，创建一个简化的模型。

然后，当你对被测系统做一些事情时，你也会对你的模型做同样的（但简化了）事情。

最后，将模型的状态与被测系统的状态进行比较。如果它们是一样的，你就完了。若非如此，要么是你的SUT存在缺陷，要么你的模型有误，你必须重新开始！

### 摘要

因此，这涵盖了一些思考属性的常见方法。

以下是七种方法，以及一个更正式的术语（如果有的话）。

- “不同的路径，相同的目的地”——可交换图（a diagram that commutes）
- “There and back again”——一个可逆函数（an invertible function）
- “有些东西永远不会改变”——变换中的不变量（an invariant under transformation）
- “事物变化越多，它们就越保持不变”——幂等性（idempotence）
- “先解决小问题”——结构归纳（structural induction）
- “难以证明，易于验证”
- “测试预言机”

这就是理论。我们如何在实践中应用它们？[在下一篇文章中，我们将介绍一些简单的任务](https://fsharpforfunandprofit.com/posts/property-based-testing-3)，如“对列表排序”、“反转列表”等，并了解如何使用这些不同的方法测试这些实现。

# 4 在实践中选择属性，第 1 部分

*Part of the "Property Based Testing" series (*[link](https://fsharpforfunandprofit.com/posts/property-based-testing-3/#series-toc)*)*

列表函数的属性
2014年12月13日这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/property-based-testing-3/

在[上一篇文章](https://fsharpforfunandprofit.com/posts/property-based-testing-2/)中，我们研究了查找属性的一些常见模式。在这篇文章中，我们将应用这些方法，看看我们是否可以为一些简单的函数（如“对列表排序”和“反转列表”）提供属性。在我们工作的过程中，我们一直在思考来自地狱的企业开发人员（[见前面的帖子](https://fsharpforfunandprofit.com/posts/property-based-testing/)）以及 EDFH 如何欺骗我们的测试通过。

- [“不同的路径，相同的目的地”](https://fsharpforfunandprofit.com/posts/property-based-testing-3/#path_sort)适用于对列表进行排序
- [“不同的路径，相同的目的地”](https://fsharpforfunandprofit.com/posts/property-based-testing-3/#path_rev)适用于颠倒列表
- [“There and back again”](https://fsharpforfunandprofit.com/posts/property-based-testing-3/#inverseRev)用于颠倒列表
- [“难以证明，易于验证”](https://fsharpforfunandprofit.com/posts/property-based-testing-3/#hardSplit)适用于拆分字符串
- [“难以证明，易于验证”](https://fsharpforfunandprofit.com/posts/property-based-testing-3/#hardList)适用于对列表进行排序
- [“有些事情永远不会改变”](https://fsharpforfunandprofit.com/posts/property-based-testing-3/#invariantSort)。应用于列表排序的不变量
- [“解决一个较小的问题”](https://fsharpforfunandprofit.com/posts/property-based-testing-3/#recurse)应用于列表排序
- [“变化越多，它们就越保持不变”](https://fsharpforfunandprofit.com/posts/property-based-testing-3/#idempotent)适用于对列表进行排序
- [“两个脑袋总比一个好”](https://fsharpforfunandprofit.com/posts/property-based-testing-3/#oracle)适用于排序列表

## “不同的路径，相同的目的地”

让我们从*“不同路径，相同目的地”*的方法开始，或者更正式地说，是“交换图（diagram that commutes）”，并将其应用于“排序”函数。

![List sort?](https://fsharpforfunandprofit.com/posts/property-based-testing-3/property_list_sort.png)

我们能想出任何方法来组合 `sort` 前的一个操作和 `sort` 后的另一个操作，以便最终得到相同的结果吗？也就是说，“先上升后越过顶部”与“先越过底部后上升”是相同的。

这个怎么样？

- **路径 1**：我们向列表的每个元素添加一，然后排序。
- **路径 2**：我们排序，然后向列表的每个元素添加一。
- 两个列表应该相等。

![List sort with +1](https://fsharpforfunandprofit.com/posts/property-based-testing-3/property_list_sort1.png)

这里有一些实现该属性的代码。我添加了一个额外的参数 `sortFn`，这样我就可以传入排序实现的不同实现来测试它们。

```F#
let addThenSort_eq_sortThenAdd sortFn aList =
  let add1 x = x + 1

  let result1 = aList |> sortFn |> List.map add1
  let result2 = aList |> List.map add1 |> sortFn
  result1 = result2
```

现在，让我们使用 FsCheck 检查此属性，并使用 `sort` 函数的良好实现

```F#
let goodSort = List.sort
Check.Quick (addThenSort_eq_sortThenAdd goodSort)
// Ok, passed 100 tests.
```

好吧，这是可行的，但它也适用于许多其他转换。例如，如果 EDFH 仅将 `List.sort` 实现为标识，那么这个属性也会得到很好的满足！你可以自己测试一下：

```F#
let edfhSort1 aList = aList  // return the list unsorted!
Check.Quick (addThenSort_eq_sortThenAdd edfhSort1)
// Ok, passed 100 tests.
```

这个属性的问题在于，它没有利用任何“排序”。我们知道排序可能会对列表进行重新排序，当然，最小的元素应该排在第一位。

### 版本 2

添加一个我们知道排序后会出现在列表前面的项目怎么样？

- **路径 1**：我们将 `Int32.MinValue` 附加到列表末尾，然后进行排序。
- **路径 2**：我们排序，然后在列表前面添加 `Int32.MinValue`。
- 两个列表应该相等。

![List sort with minValue](https://fsharpforfunandprofit.com/posts/property-based-testing-3/property_list_sort2.png)

代码如下：

```F#
let minValueThenSort_eq_sortThenMinValue sortFn aList =
  let minValue = Int32.MinValue

  let appendThenSort =
    (aList @ [minValue]) |> sortFn
  let sortThenPrepend =
    minValue :: (aList |> sortFn)
  appendThenSort = sortThenPrepend

// test
Check.Quick (minValueThenSort_eq_sortThenMinValue goodSort)
// Ok, passed 100 tests.
```

EDFH 的实现现在失败了！

```F#
Check.Quick (minValueThenSort_eq_sortThenMinValue edfhSort1)
// Falsifiable, after 1 test (2 shrinks)
// [0]
```

换句话说，`[0; minValue]` 的坏排序与 `[minValue; 0]` 不同。

那太好了！

但是…我们有一些硬编码的东西，EDFH 可以利用！EDFH 会毫不犹豫地利用这样一个事实，即我们总是使用 `Int32.MinValue`，并且我们总是将其添加到测试列表的前面或后面。

换言之，EDFH 可以识别我们所处的路径，并为每一条路径提供特殊情况：

```F#
// The Enterprise Developer From Hell strikes again
let edfhSort2 aList =
  match aList with
  | [] -> []
  | _ ->
    let head, tail = List.splitAt (aList.Length-1) aList
    let lastElem = tail.[0]
    // if the last element is Int32.MinValue,
    // then move it to front
    if (lastElem = Int32.MinValue) then
      lastElem :: head
    else
      // otherwise, do not sort the list!
      aList
```

当我们检查它时…

```F#
// Oh dear, the bad implementation passes!
Check.Quick (minValueThenSort_eq_sortThenMinValue edfhSort2)
// Ok, passed 100 tests.
```

### 版本 3

我们可以通过以下方式解决这个问题：（a）选择一个比列表中任何数字都小的随机数，（b）将其插入随机位置，而不是总是附加它。但与其过于复杂，让我们停下来重新考虑。

另一种利用“排序性”的方法是首先否定所有值，然后在排序后否定的路径上，也添加一个额外的反向值。

![List sort with negate](https://fsharpforfunandprofit.com/posts/property-based-testing-3/property_list_sort3.png)

```F#
let negateThenSort_eq_sortThenNegateThenReverse sortFn aList =
  let negate x = x * -1

  let negateThenSort =
    aList |> List.map negate |> sortFn
  let sortThenNegateAndReverse =
    aList |> sortFn |> List.map negate |> List.rev
  negateThenSort = sortThenNegateAndReverse
```

EDFH 更难击败此属性，因为没有神奇的数字来帮助识别您所处的路径：

```F#
// test the good implementation
Check.Quick (negateThenSort_eq_sortThenNegateThenReverse goodSort)
// Ok, passed 100 tests.

// test the first EDFH sort
Check.Quick (negateThenSort_eq_sortThenNegateThenReverse edfhSort1)
// Falsifiable, after 1 test (1 shrinks)
// [1; 0]

// test the second EDFH sort
Check.Quick (negateThenSort_eq_sortThenNegateThenReverse edfhSort2)
// Falsifiable, after 5 tests (3 shrinks)
// [1; 0]
```

你可能会说，我们只是在测试整数列表的排序。但是 `List.sort` 函数是泛型的，对整数本身一无所知，所以我很有信心这个属性确实测试了核心排序逻辑。

如果你一直在关注，那么 EDFH 可以使用一个简单的实现来击败这个属性：简单地返回空列表！我们稍后将看看如何处理这个问题。

## 将“不同路径，相同目的地”应用于列表反转功能

好了，足够的 `List.sort` 了。把同样的想法应用于列表反转函数怎么样？

我们可以执行相同的 append/prepend 技巧：

![List reverse](https://fsharpforfunandprofit.com/posts/property-based-testing-3/property_list_rev.png)

这是该属性的代码。再次，我添加了一个额外的参数 `revFn`，这样我们就可以在不同的实现中传递。

```F#
let appendThenReverse_eq_reverseThenPrepend revFn anyValue aList =
  let appendThenReverse =
    (aList @ [anyValue]) |> revFn
  let reverseThenPrepend =
    anyValue :: (aList |> revFn)
  appendThenReverse = reverseThenPrepend
```

以下是正确功能和 EDFH 两次尝试实现错误功能的测试结果：

```F#
// Correct implementation suceeeds
let goodReverse = List.rev
Check.Quick (appendThenReverse_eq_reverseThenPrepend goodReverse)
// Ok, passed 100 tests.

// EDFH attempt #1 fails
let edfhReverse1 aList = []    // incorrect implementation
Check.Quick (appendThenReverse_eq_reverseThenPrepend edfhReverse1)
// Falsifiable, after 1 test (2 shrinks)
// true, []

// EDFH attempt #2 fails
let edfhReverse2 aList = aList  // incorrect implementation
Check.Quick (appendThenReverse_eq_reverseThenPrepend edfhReverse2)
// Falsifiable, after 1 test (1 shrinks)
// true, [false]
```

你可能会注意到这里有一些有趣的东西。我从未指定列表的类型。该属性适用于任何列表。

在这种情况下，FsCheck 将生成布尔、字符串、整数等的随机列表。

在这两种失败的情况下，`anyValue` 都是 bool。因此，FsCheck 首先使用的是胸部列表。

这里有一个练习：这个属性够好吗？EDFH 是否有办法创建一个可以通过的实现？

## “来回”

有时多路径样式属性不可用或太复杂，所以让我们转向另一种方法：反转。

让我们再次使用列表排序。排序有反序吗？嗯，不是真的。所以我们暂时跳过排序。

那么列表反转呢？好吧，碰巧的是，反转本身就是反转！

![List reverse with inverse](https://fsharpforfunandprofit.com/posts/property-based-testing-3/property_list_rev_inverse.png)

让我们把它变成一个属性：

```F#
let reverseThenReverse_eq_original revFn aList =
  let reverseThenReverse = aList |> revFn |> revFn
  reverseThenReverse = aList
```

它通过了：

```F#
let goodReverse = List.rev  // correct implementation
Check.Quick (reverseThenReverse_eq_original goodReverse)
// Ok, passed 100 tests.
```

不幸的是，一个糟糕的实现也满足了该属性！

```F#
let edfhReverse aList = aList  // incorrect implementation
Check.Quick (reverseThenReverse_eq_original edfhReverse)
// Ok, passed 100 tests.
```

然而，使用涉及逆的属性可能非常有用，最常见的是验证成对函数（如序列化/反序列化）是否正确实现。

我们将在下一篇文章中看到一些使用它的真实例子。

## “难以证明，易于验证”

到目前为止，我们一直在测试属性，而实际上并不关心操作的最终结果。

当然，在实践中，我们确实关心最终的结果！

现在，如果不复制被测函数，我们通常无法证明实现的内部是正确的。但通常我们可以很容易地确定输出是正确的。在下面的迷宫图中，我们不必知道路径查找算法是如何实现的——我们可以很容易地检查路径是否有效。

![img](https://fsharpforfunandprofit.com/posts/property-based-testing-3/property_easy_verification.png)

如果我们正在寻找最短路径，我们可能无法检查它，但至少我们知道我们有一些有效的路径。

这一原则可以相当普遍地应用。

例如，假设我们想检查 `stringSplit` 函数是否正常工作。

```F#
let stringSplit (str:string) : string list = ...
```

我们不必编写标记器——我们所要做的就是确保作为 `stringSplit` 输出的标记在连接时返回原始字符串。

![String split property](https://fsharpforfunandprofit.com/posts/property-based-testing-3/property_string_split.png)

以下是该属性的核心代码：

```F#
let tokens = stringSplit inputString

// build a string from the tokens
let recombinedString = tokens |> String.concat ","

// compare with the original
inputString = recombinedString
```

但是我们如何创建一个原始字符串呢？FsCheck 生成的随机字符串不太可能包含许多逗号！

有一些方法可以精确控制 FsCheck 生成随机数据的方式，我们稍后会介绍。

不过，就目前而言，我们将使用一个技巧。诀窍是让 FsCheck 生成一个随机字符串列表，然后我们将它们组合在一起，从中构建一个 `originalString`。

以下是该属性的完整代码：

```F#
let concatElementsOfSplitString_eq_originalString (strings:string list) =
  // make a string
  let inputString = strings |> String.concat ","

  // use the 'split' function on the inputString
  let tokens = stringSplit inputString

  // build a string from the tokens
  let recombinedString = tokens |> String.concat ","

  // compare the result with the original
  inputString = recombinedString
```

当我们测试这一点时，我们很高兴：

```F#
Check.Quick concatElementsOfSplitString_eq_originalString
// Ok, passed 100 tests.
```

## 列表排序的“难以证明，易于验证”

那么，我们如何将这一原则应用于排序列表呢？哪些属性易于验证？

我首先想到的是，对于列表中的每一对元素，第一个元素都会小于第二个元素。

![Pairwise property](https://fsharpforfunandprofit.com/posts/property-based-testing-3/property_list_sort_pairwise.png)

所以，让我们把它变成一个属性：

```F#
let adjacentPairsAreOrdered sortFn aList =
  let pairs = aList |> sortFn |> Seq.pairwise
  pairs |> Seq.forall (fun (x,y) -> x <= y )
```

现在让我们检查一下：

```F#
let goodSort = List.sort
Check.Quick (adjacentPairsAreOrdered goodSort)
```

但当我们执行上面的代码时，会发生一些有趣的事情。我们出错了！

```
System.Exception: The type System.IComparable is not
  handled automatically by FsCheck
```

异常意味着 FsCheck 正在尝试生成随机列表，但它只知道元素必须是 `IComparable`。现在 `IComparable<int>` 或 `IComparable<string>` 是具体类型，但 `IComparable` 本身不是，因此 FsCheck 抛出错误。

我们如何防止这种情况发生？解决方案是为属性指定一个特定的具体类型，例如 `int list`，如下所示：

```F#
let adjacentPairsAreOrdered sortFn (aList:int list) =
  let pairs = aList |> sortFn |> Seq.pairwise
  pairs |> Seq.forall (fun (x,y) -> x <= y )
```

这段代码现在可以工作了。

```F#
let goodSort = List.sort
Check.Quick (adjacentPairsAreOrdered goodSort)
// Ok, passed 100 tests.
```

请注意，即使该属性受到约束，它仍然是一个非常通用的属性。例如，我们本可以使用 `string list`，它的工作原理也一样。

```F#
let adjacentPairsAreOrdered sortFn (aList:string list) =
  let pairs = aList |> sortFn |> Seq.pairwise
  pairs |> Seq.forall (fun (x,y) -> x <= y )

Check.Quick (adjacentPairsAreOrdered goodSort)
// Ok, passed 100 tests.
```

> **提示**：如果 FsCheck 抛出“类型未处理”，请在属性中添加显式类型约束

我们现在结束了吗？不！此属性的一个问题是它无法捕获 EDFH 的恶意实现。

```F#
// EDFH implementation passes :(
let edfhSort aList = []

Check.Quick (adjacentPairsAreOrdered edfhSort )
// Ok, passed 100 tests.
```

一个愚蠢的实现也能奏效，这让你感到惊讶吗？

嗯，这告诉我们，除了成对排序之外，还必须有一些属性与我们忽略的排序相关联。我们在这里错过了什么？

这是一个很好的例子，说明如何进行基于属性的测试可以带来对设计的见解。我们以为我们知道排序意味着什么，但我们被迫在定义上更加严格。

碰巧的是，我们将使用下一个原则来解决这个特殊的问题！

## “有些事情永远不会改变”

一种有用的属性基于在某些转换后保留的不变量，例如保留长度或内容。这些属性本身通常不足以确保正确的实现，但它们确实经常作为对更一般属性的反检查。

例如，在[前面的一篇文章](https://fsharpforfunandprofit.com/posts/property-based-testing/)中，我们为加法创建了交换和结合属性，但后来注意到，简单地让一个返回零的实现也能满足它们！只有当我们添加 `x + 0 = x` 作为属性时，我们才能消除特定的恶意实现。

在上面的“列表排序”示例中，我们可以用一个只返回空列表的函数来满足“成对排序”属性！我们怎样才能解决这个问题？

我们的第一次尝试可能是检查排序列表的长度。如果长度不同，那么排序函数显然作弊了！

```F#
let sortedHasSameLengthAsOriginal sortFn (aList:int list) =
  let sorted = aList |> sortFn
  List.length sorted = List.length aList
```

我们检查它，它工作：

```F#
let goodSort = List.sort
Check.Quick (sortedHasSameLengthAsOriginal goodSort )
// Ok, passed 100 tests.
```

是的，EDFH 的实现失败了：

```F#
let edfhSort aList = []
Check.Quick (sortedHasSameLengthAsOriginal edfhSort)
// Falsifiable, after 1 test (1 shrink)
// [0]
```

不幸的是，EDFH 并没有失败，可以提出另一个合规的实现方案！只需重复第一个元素 N 次！

```F#
// EDFH implementation has same length as original
let edfhSort2 aList =
  match aList with
  | [] ->
    []
  | head::_ ->
    List.replicate (List.length aList) head

// for example
// edfhSort2 [1;2;3]  // => [1;1;1]
```

现在，当我们测试这个时，它通过了：

```F#
Check.Quick (sortedHasSameLengthAsOriginal edfhSort2 )
// Ok, passed 100 tests.
```

此外，它还满足成对排序属性！

```F#
Check.Quick (adjacentPairsAreOrdered edfhSort2)
// Ok, passed 100 tests.
```

我们必须再试一次！

### 排序不变性 - 第二次尝试

为了帮助我们得出一个有用的属性，让我们比较一下真实结果 `[1;2;3]` 和 EDFH 的假结果 `[1;1;1]`？

我们可以看到，虚假的结果正在丢弃数据。实际结果总是包含与原始列表相同的内容，但顺序不同。

![Permutation property](https://fsharpforfunandprofit.com/posts/property-based-testing-3/property_list_sort_permutation.png)

这就引出了一个新的属性：排序列表总是原始列表的排列。啊哈！现在让我们用排列来写这个属性：

```F#
let sortedListIsPermutationOfOriginal sortFn (aList:int list) =
  let sorted = aList |> sortFn
  let permutationsOfOriginalList = permute aList

  // the sorted list must be in the set of permutations
  permutationsOfOriginalList
  |> Seq.exists (fun permutation -> permutation = sorted)
```

太好了，现在我们只需要一个排列（permutation）函数。

让我们前往 StackOverflow，并~~窃取~~[借用一个实现](https://stackoverflow.com/a/3129136/1136133)。这是：

```F#
/// given aList and anElement to insert,
/// generate all possible lists with anElement
/// inserted into aList
let rec distribute e list =
  match list with
  | [] -> [[e]]
  | x::xs' as xs -> (e::xs)::[for xs in distribute e xs' -> x::xs]

/// Given a list, return all permutations of it
let rec permute list =
  match list with
  | [] -> [[]]
  | e::xs -> List.collect (distribute e) (permute xs)
```

一些快速的交互式测试证实它按预期工作：

```F#
permute [1;2;3] |> Seq.toList
//  [[1; 2; 3]; [2; 1; 3]; [2; 3; 1];
//   [1; 3; 2]; [3; 1; 2]; [3; 2; 1]]

permute [1;2;3;4] |> Seq.toList
// [[1; 2; 3; 4]; [2; 1; 3; 4]; [2; 3; 1; 4]; [2; 3; 4; 1]; [1; 3; 2; 4];
//  [3; 1; 2; 4]; [3; 2; 1; 4]; [3; 2; 4; 1]; [1; 3; 4; 2]; [3; 1; 4; 2];
//  [3; 4; 1; 2]; [3; 4; 2; 1]; [1; 2; 4; 3]; [2; 1; 4; 3]; [2; 4; 1; 3];
//  [2; 4; 3; 1]; [1; 4; 2; 3]; [4; 1; 2; 3]; [4; 2; 1; 3]; [4; 2; 3; 1];
//  [1; 4; 3; 2]; [4; 1; 3; 2]; [4; 3; 1; 2]; [4; 3; 2; 1]]

permute [3;3] |> Seq.toList
//  [[3; 3]; [3; 3]]
```

杰出的！现在让我们运行 FsCheck：

```F#
Check.Quick (sortedListIsPermutationOfOriginal goodSort)
```

嗯。真好笑，似乎什么都没发生。由于某种原因，我的 CPU 正在耗尽。怎么回事？

现在的情况是，你将在那里坐很长时间！如果你在家里跟随，我建议你现在右键单击并取消互动会话。

对于任何正常大小的列表来说，看起来天真无邪的 `permute` *真的*很慢。例如，一个只有 10 个项目的列表有 3628800 个排列。如果有 20 个项目，你会得到天文数字。

> **提示 #1**：编码前要三思而后行！要非常小心任何涉及排列的事情！

我们已经看到，即使在最好的情况下，FsCheck 也会对该属性进行 100 次评估。如果需要收缩，甚至更多。因此，请确保您的测试运行速度很快！

> **提示 #2**：确保你的属性检查非常快。你会运行他们很多！

但是，如果你处理的是数据库、网络或其他缓慢依赖关系等真实系统，会发生什么？

在他（强烈推荐）的[关于使用 QuickCheck 的视频](http://vimeo.com/68383317)中，John Hughes 告诉了他的团队何时试图检测分布式数据存储中的缺陷，这些缺陷可能是由网络分区和节点故障引起的。

当然，杀死真实节点数千次太慢了，所以他们将核心逻辑提取到虚拟模型中，并进行了测试。因此，后来对代码*进行了重构*，使这种测试更容易。换句话说，基于属性的测试影响了代码的设计，就像 TDD 一样。

### 排序不变性 - 第三次尝试

好吧，所以我们不能只通过循环来使用排列。因此，让我们使用相同的想法，但编写一个特定于这种情况的函数，即 `isPermutationOf` 函数。

```F#
let sortedListIsPermutationOfOriginal sortFn (aList:int list) =
  let sorted = aList |> sortFn
  isPermutationOf aList sorted
```

检查两个列表是否是彼此的排列的一个简单方法是对它们进行排序，然后进行比较。但是——哎呀！——我们正在尝试实现排序！所以我们不能用它。

相反，我们需要编写一个自定义的 `isPermutationOf` 函数（如下所示）。它更复杂，但它不依赖于现有的 `sort` 实现。

```F#
/// Given an element and a list, and other elements previously skipped,
/// return a new list without the specified element.
/// If not found, return None

/// Given an element and a list, return a new list
/// without the first instance of the specified element.
/// If element is not found, return None
let withoutElement x aList =
  let folder (acc,found) elem =
    if elem = x && not found then
      acc, true  // start skipping
    else
      (elem::acc), found // accumulate

  let (filteredList,found) =
    aList |> List.fold folder ([],false)
  if found then
    filteredList |> List.rev |> Some
  else
    None


/// Given two lists, return true if they have the same contents
/// regardless of order
let rec isPermutationOf list1 list2 =
  match list1 with
  | [] -> List.isEmpty list2 // if both empty, true
  | h1::t1 ->
    match withoutElement h1 list2 with
    | None -> false
    | Some t2 -> isPermutationOf t1 t2
```

让我们再试一次。是的，这一次它在宇宙热寂之前完成。

```F#
Check.Quick (sortedListIsPermutationOfOriginal goodSort)
// Ok, passed 100 tests.
```

同样重要的是，恶意 EDFH 实现无法满足此属性！

```F#
Check.Quick (sortedListIsPermutationOfOriginal edfhSort)
// Falsifiable, after 2 tests (4 shrinks)
// [0]

Check.Quick (sortedListIsPermutationOfOriginal edfhSort2)
// Falsifiable, after 2 tests (5 shrinks)
// [1; 0]
```

事实上，这两个属性，“列表中的相邻对应该排序”和“排序列表是原始列表的置换”，确实应该确保 `sort` 的任何实现都是正确的。

## 侧栏：组合属性

就在上面，我们注意到需要两个属性来定义“已排序”属性。如果我们能将它们组合成一个属性 `is sorted`，以便我们可以进行一次测试，那就太好了。

当然，我们总是可以将两组代码合并到一个函数中，但最好保持函数尽可能小。此外，像“is prevention of”这样的属性也可以在其他上下文中重用。

那么，我们想要的是一个与 `AND` 和 `OR` 等价的东西，它被设计用来处理属性。

FsCheck 来救援！有内置的运算符来组合属性：`.&.` 对于 `AND` 和 `.|.` 对于 `OR`。

以下是一个使用中的示例：

```F#
let listIsSorted sortFn (aList:int list) =
  let prop1 = adjacentPairsAreOrdered sortFn aList
  let prop2 = sortedListIsPermutationOfOriginal sortFn aList
  prop1 .&. prop2
```

当我们用一个好的 `sort` 实现测试组合属性时，一切都按预期工作。

```F#
let goodSort = List.sort
Check.Quick (listIsSorted goodSort )
```

如果我们测试一个糟糕的实现，组合属性也会失败。

```F#
let badSort aList = []
Check.Quick (listIsSorted badSort )
// Falsifiable, after 1 test (0 shrinks)
// [0]
```

但现在有个问题。这两个属性中哪一个失败了？

我们想做的是给每个属性添加一个“标签”，这样我们就可以把它们区分开来。在 FsCheck 中，这是通过 `|@` 运算符完成的：

```F#
let listIsSorted_labelled sortFn (aList:int list) =
  let prop1 =
    adjacentPairsAreOrdered sortFn aList
    |@ "adjacent pairs from a list are ordered"
  let prop2 =
    sortedListIsPermutationOfOriginal sortFn aList
    |@ "sorted list is a permutation of original list"
  prop1 .&. prop2
```

现在，当我们使用坏排序进行测试时，我们得到一条消息 `Label of failing property: a sorted list has same contents as the original list`：

```F#
Check.Quick (listIsSorted_labelled badSort )
//  Falsifiable, after 1 test (2 shrinks)
//  Label of failing property:
//     sorted list is a permutation of original list
//  [0]
```

有关这些运算符的更多信息，[请参阅“And、Or 和 Labels”下的 FsCheck 文档](https://fscheck.github.io/FsCheck/Properties.html#And-Or-and-Labels)。

现在，回到属性设计策略。

## “解决一个较小的问题”

有时，您会遇到递归数据结构或递归问题。在这些情况下，您通常可以找到一个适用于较小部分的属性。

例如，对于排序，我们可以说：

```
如果满足以下条件，则对列表进行排序：
* 第一个元素小于（或等于）第二个元素。
* 第一个元素之后的其余元素也被排序。
```

以下是代码中表达的逻辑：

```F#
let rec firstLessThanSecond_andTailIsSorted sortFn (aList:int list) =
  let sortedList = aList |> sortFn
  match sortedList with
  | [] -> true
  | [first] -> true
  | [first;second] -> first <= second
  | first::second::rest->
    first <= second &&
    let tail = second::rest
    // check that tail is sorted
    firstLessThanSecond_andTailIsSorted sortFn tail
```

真的排序函数满足此属性：

```F#
let goodSort = List.sort
Check.Quick (firstLessThanSecond_andTailIsSorted goodSort )
// Ok, passed 100 tests.
```

但不幸的是，就像前面的例子一样，EDFH 的恶意实现也会通过。

```F#
let efdhSort aList = []
Check.Quick (firstLessThanSecond_andTailIsSorted efdhSort)
// Ok, passed 100 tests.

let efdhSort2  aList =
  match aList with
  | [] -> []
  | head::_ -> List.replicate (List.length aList) head

Check.Quick (firstLessThanSecond_andTailIsSorted efdhSort2)
// Ok, passed 100 tests.
```

因此，与之前一样，我们需要另一个属性（例如“is permutation of”不变量）来确保代码的正确性。

如果你确实有递归数据结构，那么试着寻找递归属性。当你掌握了窍门时，它们非常明显，而且很容易上手。

## “事物变化越多，就越保持不变”

我们的下一类属性是“幂等性”。Idempotence 只是意味着做两次和做一次是一样的。如果我告诉你“坐下”，然后又告诉你“坐下来”，那么第二个命令没有效果。

Idempotence [对于可靠的系统至关重要](https://queue.acm.org/detail.cfm?id=2187821)，[是面向服务](http://soapatterns.org/design_patterns/idempotent_capability)和基于消息的架构的关键方面。

如果你正在设计这类真实世界的系统，那么确保你的请求和流程是幂等的是非常值得的。

我现在不会深入探讨这个问题，但让我们来看两个简单的例子。

首先，我们的老朋友 `sort` 是幂等的（忽略稳定性），而 `reverse` 显然不是。

```F#
let sortTwice_eq_sortOnce sortFn (aList:int list) =
  let sortedOnce = aList |> sortFn
  let sortedTwice = aList |> sortFn |> sortFn
  sortedOnce = sortedTwice

// test
let goodSort = List.sort
Check.Quick (sortTwice_eq_sortOnce goodSort )
// Ok, passed 100 tests.
```

一般来说，任何类型的查询都应该是幂等的，或者换句话说：[“问问题不应该改变答案”](https://en.wikipedia.org/wiki/Command%E2%80%93query_separation)。

在现实世界中，情况可能并非如此。在不同时间运行的数据存储上的简单查询可能会给出不同的结果。

这里有一个快速演示。

这是一个 `IdempotentService`，它总是为每个查询提供相同的结果。

```F#
type IdempotentService() =
  let data = 0
  member this.Get() =
    data
```

这里有一个 `NonIdempotentService`，它改变了每个查询的一些内部状态。也就是说，如果我们问同一个问题，每次都会得到不同的答案。

```F#
type NonIdempotentService() =
  let mutable data = 0
  member this.Get() =
    data <- data + 1
    data
```

用于检查函数是否幂等的属性可能如下：

```F#
let idempotentServiceGivesSameResult get =
  // first GET
  let get1 = get()

  // second GET
  let get2 = get()
  get1 = get2
```

如果我们用第一个幂等服务进行测试，它会通过：

```F#
let service = IdempotentService()
let get() = service.Get()

Check.Quick (idempotentServiceGivesSameResult get)
// Ok, passed 100 tests.
```

但如果我们用非幂等服务进行测试，它就会失败

```F#
let service = NonIdempotentService()
let get() = service.Get()

Check.Quick (idempotentServiceGivesSameResult get)
// Falsifiable, after 1 test
```

这个非幂等服务微不足道。在一个更现实的例子中，内部状态可能不会因查询而改变，而是因其他操作而改变。但是可变状态和并发性不能很好地结合在一起，在这种情况下，在对同一查询的调用之间，状态很容易发生变化（因为其他进程会改变它）。

有很多方法可以解决这个问题。如果你正在构建一个REST GET处理程序或数据库查询服务，并且你想要幂等性，你应该考虑使用etags、“截至”时间、日期范围等技术。使这更容易的设计技术包括事件源、时态数据库等。如果你需要如何做到这一点的技巧，搜索[幂等性模式](http://blog.jonathanoliver.com/idempotency-patterns/)会得到一些好的结果。

## “三个臭皮匠胜过一个诸葛亮”

最后，最后但同样重要的是，我们来到“测试神谕”。测试预言机只是一种提供正确答案的替代实现，您可以对照它检查结果。

通常，测试预言机实现不适合生产环境——它太慢了，或者它没有并行化，或者它[太诗意了](https://xkcd.com/1026/)，等等，但这并不能阻止它对测试非常有用。

因此，对于“列表排序”，有许多简单但缓慢的实现。例如，这里有一个插入排序的快速实现：

```F#
module InsertionSort =

  // Insert a new element into a list by looping over the list.
  // As soon as you find a larger element, insert in front of it
  let rec insert newElem list =
    match list with
    | head::tail when newElem > head ->
      head :: insert newElem tail
    | other -> // including empty list
      newElem :: other

  // Sorts a list by inserting the head into the rest of the list
  // after the rest have been sorted
  let rec sort list =
    match list with
    | []   -> []
    | head::tail ->
      insert head (sort tail)

  // test
  // sort  [5;3;2;1;1]
```

有了这个，我们可以编写一个属性来测试我们的自定义排序函数是否符合插入排序。

```F#
let customSort_eq_insertionSort sortFn (aList:int list) =
  let sorted1 = aList |> sortFn
  let sorted2 = aList |> InsertionSort.sort
  sorted1 = sorted2
```

当我们测试好的排序实现时，它是有效的。好！

```F#
let goodSort = List.sort
Check.Quick (customSort_eq_insertionSort goodSort)
// Ok, passed 100 tests.
```

当我们测试一个糟糕的排序实现时，它不会。更好！

```F#
let edfhSort aList = aList
Check.Quick (customSort_eq_insertionSort edfhSort)
// Falsifiable, after 4 tests (6 shrinks)
// [1; 0]
```

## EDFH 真的是个问题吗？

在这篇文章中，我注意到，琐碎但错误的实现往往既能满足属性，也能满足好的实现。

但我们*真的*应该花时间担心这件事吗？我的意思是，如果我们真的发布了一个只复制第一个元素的排序算法，那么它会立即变得显而易见，对吧？

所以，是的，真正的恶意实现不太可能成为问题。另一方面，你不应该把基于属性的测试看作一个测试过程，而应该把它看作一个设计过程——一种帮助你阐明系统真正想要做什么的技术。如果你的设计的一个关键方面仅仅满足于一个简单的实现，那么也许你忽略了一些东西——当你发现它时，它会让你的设计更清晰、更稳健。

## 摘要

在这篇文章中，我们研究了如何使用这些属性来测试各种列表函数。但是，为其他问题寻找属性呢？在[下一篇文章](https://fsharpforfunandprofit.com/posts/property-based-testing-4)中，我们将继续使用相同的技术来测试不同的领域。

> 本文中使用的源代码可以在[这里找到](https://github.com/swlaschin/fsharpforfunandprofit.com_code/tree/master/posts/property-based-testing-3)。



# 5 在实践中选择属性，第 2 部分

*Part of the "Property Based Testing" series (*[link](https://fsharpforfunandprofit.com/posts/property-based-testing-4/#series-toc)*)*

罗马数字转换的性质
2014年12月14日这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/property-based-testing-4/

在[上一篇文章](https://fsharpforfunandprofit.com/posts/property-based-testing-3/)中，我们使用各种属性测试了一些列表函数。让我们继续以同样的方式测试更多的代码。在这篇文章中，我们的挑战将是测试罗马数字转换逻辑。

- [“两个头总比一个头好”](https://fsharpforfunandprofit.com/posts/property-based-testing-4/#oracle)适用于两种不同的实现
- [“There and back again”](https://fsharpforfunandprofit.com/posts/property-based-testing-4/#inverse)用于罗马数字的编码和解码
- [“解决一个小问题”](https://fsharpforfunandprofit.com/posts/property-based-testing-4/#recurse)在罗马数字解码中的应用
- [“有些事情永远不会改变”](https://fsharpforfunandprofit.com/posts/property-based-testing-4/#invariant)。不变量在罗马数字编码中的应用
- [“不同的路径，相同的目的地”](https://fsharpforfunandprofit.com/posts/property-based-testing-4/#commutative)适用于罗马数字的转换

## 以两种不同的方式生成罗马数字

在我的帖子[“对‘罗马数字 Kata 的评论与评论’”](https://fsharpforfunandprofit.com/posts/roman-numeral-kata/)中，我提出了两种完全不同的生成罗马数字的算法。

第一种算法是基于对罗马数字是基于计数的理解

![img](https://fsharpforfunandprofit.com/posts/property-based-testing-4/200px-Tally_marks.svg.png)

换言之，将五个笔划替换为“V”，将两个Vs替换为X，以此类推，从而得到这个简单的实现：

```F#
module TallyImpl =
  let arabicToRoman arabic =
    (String.replicate arabic "I")
      .Replace("IIIII","V")
      .Replace("VV","X")
      .Replace("XXXXX","L")
      .Replace("LL","C")
      .Replace("CCCCC","D")
      .Replace("DD","M")
      // optional substitutions
      .Replace("IIII","IV")
      .Replace("VIV","IX")
      .Replace("XXXX","XL")
      .Replace("LXL","XC")
      .Replace("CCCC","CD")
      .Replace("DCD","CM")
```

如果我们以交互方式进行测试，我们会得到看似正确的行为。

```F#
TallyImpl.arabicToRoman 1    //=> "I"
TallyImpl.arabicToRoman 9    //=> "IX"
TallyImpl.arabicToRoman 24   //=> "XXIV"
TallyImpl.arabicToRoman 999  //=> "CMXCIX"
TallyImpl.arabicToRoman 1493 //=> "MCDXCIII"
```

### 双五元实现

另一种思考罗马数字的方法是想象一个算盘。每根钢丝有四个“单位”珠和一个“五”珠。

![img](https://fsharpforfunandprofit.com/posts/property-based-testing-4/RomanAbacusRecon.jpg)

这导致了所谓的“双五元”方法：

```F#
module BiQuinaryImpl =
  let biQuinaryDigits place (unit,five,ten) arabic =
    let digit =  arabic % (10*place) / place
    match digit with
    | 0 -> ""
    | 1 -> unit
    | 2 -> unit + unit
    | 3 -> unit + unit + unit
    | 4 -> unit + five // changed to be one less than five
    | 5 -> five
    | 6 -> five + unit
    | 7 -> five + unit + unit
    | 8 -> five + unit + unit + unit
    | 9 -> unit + ten  // changed to be one less than ten
    | _ -> failwith "Expected 0-9 only"

  let arabicToRoman arabic =
    let units = biQuinaryDigits 1 ("I","V","X") arabic
    let tens = biQuinaryDigits 10 ("X","L","C") arabic
    let hundreds = biQuinaryDigits 100 ("C","D","M") arabic
    let thousands = biQuinaryDigits 1000 ("M","?","?") arabic
    thousands + hundreds + tens + units
```

同样，如果我们以交互方式进行测试，结果看起来不错。

```F#
BiQuinaryImpl.arabicToRoman 1    //=> "I"
BiQuinaryImpl.arabicToRoman 9    //=> "IX"
BiQuinaryImpl.arabicToRoman 24   //=> "XXIV"
BiQuinaryImpl.arabicToRoman 999  //=> "CMXCIX"
BiQuinaryImpl.arabicToRoman 1493 //=> "MCDXCIII"
```

但是，我们如何确保这些实现对所有数字都是正确的，而不仅仅是我们测试的数字？

## 测试预言机

获得信心的一种方法是使用测试预言机方法——将它们相互比较。当你不确定两个不同的实现是否正确时，这是一种交叉检查两个不同实现的好方法！

```F#
let biquinary_eq_tally number =
  let tallyResult = TallyImpl.arabicToRoman number
  let biquinaryResult = BiQuinaryImpl.arabicToRoman number
  tallyResult = biquinaryResult
```

但是，如果我们尝试运行此代码，由于调用  `String.replicate` ，我们会得到 `ArgumentException: The input must be non-negative` 。

```F#
Check.Quick biquinary_eq_tally
// ArgumentException: The input must be non-negative.
```

因此，我们只需要包括积极的输入。我们还需要排除大于4000的数字，比如说，因为算法也会在那里崩溃。

我们如何实现这个过滤器？

我们在[之前的一篇文章](https://fsharpforfunandprofit.com/posts/property-based-testing-1/#adding-pre-conditions)中看到，我们可以使用先决条件。但对于这个例子，我们将尝试一些不同的方法并更改生成器。

首先，我们将定义一个名为 `arabicNumber` 的整数生成器（“任意”），它可以根据需要进行过滤（如果你还记得，“任意”是生成器算法和收缩器算法的组合，如[前](https://fsharpforfunandprofit.com/posts/property-based-testing-1/#understanding-fscheck-generators)所述）。我们只包括 1 到 4000 之间的数字。

```F#
let arabicNumber =
  Arb.Default.Int32()
  |> Arb.filter (fun i -> i > 0 && i <= 4000)
```

接下来，我们使用 `Prop.forAll` 辅助程序创建一个新属性，该属性仅限于使用 `arabicNumber` 生成器。

```F#
let biquinary_eq_tally_withinRange =
  Prop.forAll arabicNumber biquinary_eq_tally
```

现在，我们可以再次进行交叉检查测试：

```F#
Check.Quick biquinary_eq_tally_withinRange
// Ok, passed 100 tests.
```

我们很好！这两种算法似乎都能正常工作。

### 检查整个域

我们总共有多少个罗马数字？4000 我们说。那么，为什么不把它们都测试一下呢？

让我们在我们的属性中运行所有 4000 个数字，过滤出成功的数字，只留下失败的数字，如下所示：

```F#
[1..4000] |> List.choose (fun i ->
  if biquinary_eq_tally i then None else Some i
  )
// output => [4000]
```

我们希望没有失败的数字，输出列表为空，但实际上列表中只有一个数字：4000！

如果我们检查 4000 的两个转换，我们可以看到它们有什么不同。双五进制实现不知道如何处理它。计数实现不那么脆弱，可以在不中断的情况下处理更高的数字。

```F#
TallyImpl.arabicToRoman 4000     //=> "MMMM"
BiQuinaryImpl.arabicToRoman 4000 //=> "M?"
```

这是我们关心的事情吗？也许不是。不过，我们可能希望将实现的输入限制在 4000 以下。例如，我们可以更改计数实现以返回 `Some` 或 `None`，如下所示：

```F#
let arabicToRoman arabic =
  if (arabic <= 0 || arabic >= 4000) then
    None
  else
    (String.replicate arabic "I")
      .Replace("IIIII","V")
      .Replace("VV","X")
      // etc
    |> Some
```

> 提示：如果你的域足够小，为什么不检查其中的所有值呢？
>
> 对于来自不同领域的示例，请参阅帖子[“只有 40 亿个浮点数——所以全部测试！”](https://randomascii.wordpress.com/2014/01/27/theres-only-four-billion-floatsso-test-them-all/)。它使用“测试预言机”方法来检查所有 40 亿个浮点数的实现。

### 别忘了检查边界！

这个小问题提醒我们，基于属性的检查不是金锤。它生成随机数据，但不一定是探测边界域的最佳方式。如果你确实有众所周知的边界，最好为它们创建一些特定的测试，要么使用 PBT 工具的自定义生成器，要么简单地对边缘情况进行一些明确的基于示例的测试，如下所示：

```F#
for i in [3999..4001] do
  if not (biquinary_eq_tally i) then
    failwithf "test failed for %i" i
```

## 来回

测试这些罗马数字实现的另一种方法是创建一个逆函数，这样应用原始函数，然后应用逆函数，就可以回到原始状态。

如果我们把罗马数字转换看作一种“编码”，那么我们就需要编写一个相应的“解码器”。这是一个非常简单的方法，它使用了我们最初使用的基于“计数”的方法：

```F#
let romanToArabic (str:string) =
  str
    .Replace("CM","DCD")
    .Replace("CD","CCCC")
    .Replace("XC","LXL")
    .Replace("XL","XXXX")
    .Replace("IX","VIV")
    .Replace("IV","IIII")
    .Replace("M","DD")
    .Replace("D","CCCCC")
    .Replace("C","LL")
    .Replace("L","XXXXX")
    .Replace("X","VV")
    .Replace("V","IIIII")
    .Length
```

这里有一些临时测试

```F#
TallyDecode.romanToArabic "I"       //=> 1
TallyDecode.romanToArabic "IX"      //=> 9
TallyDecode.romanToArabic "XXIV"    //=> 24
TallyDecode.romanToArabic "CMXCIX"  //=> 999
TallyDecode.romanToArabic "MCDXCIII"//=> 1493
```

现在有了逆函数，我们可以编写一个基于属性的测试。请注意，我们使用相同的 `arabicNumber` 生成器来限制输入：

```F#
/// encoding then decoding should return
/// the original number
let encodeThenDecode_eq_original =

  // define an inner property
  let innerProp arabic1 =
    let arabic2 =
      arabic1
      |> TallyImpl.arabicToRoman // encode
      |> TallyDecode.romanToArabic // decode
    // should be same
    arabic1 = arabic2

  Prop.forAll arabicNumber innerProp
```

如果我们运行测试，它就会通过。

```F#
Check.Quick encodeThenDecode_eq_original
// Ok, passed 100 tests.
```

我们还可以将双五元编码与基于计数的解码进行比较

```F#
/// encoding then decoding should return
/// the original number
let encodeThenDecode_eq_original2 =

  // define an inner property
  let innerProp arabic1 =
    let arabic2 =
      arabic1
      |> BiQuinaryImpl.arabicToRoman // encode
      |> TallyDecode.romanToArabic // decode
    // should be same
    arabic1 = arabic2

  Prop.forAll arabicNumber innerProp
```

如果我们运行测试，它会再次通过。

```F#
Check.Quick encodeThenDecode_eq_original2
// Ok, passed 100 tests.
```

## 解决一个较小的问题

检查实现是否正确的另一种方法是将其分解为较小的组件，并检查较小的组件是否正确。

例如，如果我们将阿拉伯数字分解为 1000s、100s、10s和单位，然后分别对它们进行编码，那么这些分量的连接应该与直接编码的原始阿拉伯数字相同。

```F#
let recursive_prop =

  // define an inner property
  let innerProp arabic =
    let thousands =
      (arabic / 1000 % 10) * 1000
      |> BiQuinaryImpl.arabicToRoman
    let hundreds =
      (arabic / 100 % 10) * 100
      |> BiQuinaryImpl.arabicToRoman
    let tens =
      (arabic / 10 % 10) * 10
      |> BiQuinaryImpl.arabicToRoman
    let units =
      arabic % 10
      |> BiQuinaryImpl.arabicToRoman

    let direct =
      arabic
      |> BiQuinaryImpl.arabicToRoman

    // should be same
    direct = thousands+hundreds+tens+units

  Prop.forAll arabicNumber innerProp
```

而且，它工作得很好！

```F#
Check.Quick recursive_prop
// Ok, passed 100 tests.
```

## 有些事情永远不会改变

正如我们上次提到的，不变量通常是击败 EDFH 的好方法。它们本身无法证明实现是正确的，但与其他属性结合使用，它们是一个非常强大的工具。

那么，罗马数字编码有哪些不变量呢？

好吧，在编码的输入和输出之间没有明显的东西被保留，比如字符串的长度或集合的内容。

然而，当使用算术从另一个罗马数字创建罗马数字时，会保留一些属性。例如，永远不会有超过三个“I”或一个“V”等。我们可以很容易地测试这些不变量。

首先，我们需要一个函数来计算模式的出现次数：

```F#
let matchesFor pattern input =
  System.Text.RegularExpressions.Regex.Matches(input,pattern).Count

(*
"MMMCXCVIII" |> matchesFor "I"   //=> 3
"MMMCXCVIII" |> matchesFor "XC"  //=> 1
"MMMCXCVIII" |> matchesFor "C"   //=> 2
"MMMCXCVIII" |> matchesFor "M"   //=> 3
*)
```

然后我们可以使用这些不变量定义我们的属性：

```F#
let invariant_prop =

  let maxMatchesFor pattern n input =
    (matchesFor pattern input) <= n

  // define an inner property
  let innerProp arabic =
    let roman = arabic |> TallyImpl.arabicToRoman
    (roman |> maxMatchesFor "I" 3)
    && (roman |> maxMatchesFor "V" 1)
    && (roman |> maxMatchesFor "X" 4)
    && (roman |> maxMatchesFor "L" 1)
    && (roman |> maxMatchesFor "C" 4)
    // etc

  Prop.forAll arabicNumber innerProp
```

如果我们测试它，它会按预期工作。

```F#
Check.Quick invariant_prop
// Ok, passed 100 tests.
```

## 不同的路径，相同的目的地

我们能用这些模式走多远？我们还没有使用交换图方法——我们能把它也应用于罗马数字编码吗？

![img](https://fsharpforfunandprofit.com/posts/property-based-testing-4/property_commutative.png)

是的，我们可以。这并不是测试这个的最合适的方法，但这是一个有趣的游戏，看看你能想出什么！

这两条路怎么样：

- 路径 1
  - 给定一个小于 400 的数字
  - 先编码
  - 然后用“M”替换“C”，用“C”替换“X”，以此类推，创建一个新的罗马数字。
- 路径 2
  - 给定相同的数字
  - 先把它乘以 10
  - 然后对其进行编码

这两条路径应该给出相同的结果。

```F#
/// Encoding a number less than 400 and then replacing
/// all the characters with the corresponding 10x higher one
/// should be the same as encoding the 10x number directly.
let commutative_prop1 =

  // define an inner property
  let innerProp arabic =
    // take the part < 1000
    let arabic = arabic % 1000
    // encode it
    let result1 =
      (TallyImpl.arabicToRoman arabic)
        .Replace("C","M")
        .Replace("L","D")
        .Replace("X","C")
        .Replace("V","L")
        .Replace("I","X")
    // encode the 10x number
    let result2 =
      TallyImpl.arabicToRoman (arabic * 10)

    // should be same
    result1 = result2

  Prop.forAll arabicNumber innerProp
```

而且，令人惊讶的是，它奏效了！

```F#
Check.Quick commutative_prop1
// Ok, passed 100 tests.
```

反过来呢：

- 路径1
  - 给定一个小于 4000 的数字
  - 先编码
  - 然后用“X”替换“C”，用“C”替换“M”，以此类推，创建一个新的罗马数字。
- 路径2
  - 给定相同的数字
  - 先除以 10
  - 然后对其进行编码

```F#
/// Encoding a number and then replacing all the characters with
/// the corresponding 10x lower one should be the same as
/// encoding the 10x lower number directly.
let commutative_prop2 =

  // define an inner property
  let innerProp arabic =
    // encode it
    let result1 =
      (TallyImpl.arabicToRoman arabic)
        .Replace("I","")
        .Replace("V","")
        .Replace("X","I")
        .Replace("L","V")
        .Replace("C","X")
        .Replace("D","L")
        .Replace("M","C")
    // encode the 10x lower number
    let result2 =
      TallyImpl.arabicToRoman (arabic / 10)

    // should be same
    result1 = result2

  Prop.forAll arabicNumber innerProp
```

这一次，它失败了，以“9”作为反例。这是为什么？

```F#
Check.Quick commutative_prop2
// Falsifiable, after 9 tests
// 9
```

这对读者来说是一种锻炼！

## 到目前为止的总结

我希望你现在明白了。通过尝试各种方法（神谕、逆、不变量、交换图等），我们几乎总能为我们的设计提供有用的属性。

## 插曲：一款基于寻找房产的游戏

至此，我们已经结束了各种属性类别。我们将在一分钟内再复习一遍，但首先是一段插曲。

如果你有时觉得寻找属性是一种精神挑战，那么你并不孤单。假装这是一场游戏会有帮助吗？

碰巧，有一个基于寻找属性的游戏。

它被称为 [Zendo](http://boardgamegeek.com/boardgame/6830/zendo)，它涉及将一组对象（如塑料金字塔）放置在桌子上，这样每个布局都符合一个模式——一个规则——或者我们现在所说的一个属性！。

然后，其他玩家必须根据他们看到的内容猜测规则（属性）是什么。

这是一个正在进行的 Zendo 游戏的图片：

![Zendo](https://fsharpforfunandprofit.com/posts/property-based-testing-4/zendo1.png)

白色的石头意味着属性已经满意，而黑色的石头则意味着失败。你能猜出这里的规则吗？我猜测这类似于“一组必须有一个不接触地面的黄色金字塔”。

好吧，我想 Zendo 并没有真正受到基于属性的测试的启发，但它是一款有趣的游戏，甚至在[编程会议](http://blog.fogus.me/2014/10/23/games-of-interest-zendo/)上也有出现。

如果你想了解更多关于 [Zendo](http://www.looneylabs.com/rules/zendo) 的信息，规则就在这里。

## 待续

在[下一篇文章](https://fsharpforfunandprofit.com/posts/property-based-testing-5)中，我们将继续使用相同的技术来测试一个经典的TDD示例。

> 本文中使用的源代码可以在[这里找到](https://github.com/swlaschin/fsharpforfunandprofit.com_code/tree/master/posts/property-based-testing-4)。



# 6 在实践中选择属性，第3部分

*Part of the "Property Based Testing" series (*[link](https://fsharpforfunandprofit.com/posts/property-based-testing-5/#series-toc)*)*

美元对象的属性
2014年12月15日这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/property-based-testing-5/

在[前两篇文章](https://fsharpforfunandprofit.com/posts/property-based-testing-4/)中，我们研究了如何将基于属性的测试应用于列表和罗马数字。现在让我们再看一个示例问题，看看我们是否能找到它的属性。

此示例基于 Kent Beck 的《TDD By example》一书中描述的著名 `Dollar` 示例。我们不会试图批评设计本身，并使其更加以类型为导向——[其他人已经这样做了](http://spin.atomicobject.com/2014/12/10/typed-language-tdd-part2/)。相反，我们将按照给定的设计，看看我们能想出什么属性。

那么，我们有什么？

- 存储 `Amount` 的 `Dollar` 类。
- 两种方法 `Add` 和 `Times` 以明显的方式转换金额。

```F#
// OO style class with members
type Dollar(amount:int) =
  /// factory method
  static member Create amount  =
    Dollar amount
  /// field
  member val Amount =
    amount with get, set
  /// Add to the amount
  member this.Add add =
    this.Amount <- this.Amount + add
  /// Multiply the amount
  member this.Times multiplier  =
    this.Amount <- this.Amount * multiplier
```

因此，首先让我们以交互方式进行尝试，以确保它按预期工作：

```F#
let d = Dollar.Create 2
d.Amount  // 2
d.Times 3
d.Amount  // 6
d.Add 1
d.Amount  // 7
```

## 测试美元实现

但这只是玩游戏，不是真正的测试。那么，我们能想到什么样的属性呢？

让我们再次浏览它们：

- 通往同一结果的不同路径
- 反转
- 不变式
- 意识
- 结构归纳
- 易于验证
- 测试预言机

让我们暂时跳过“不同的路径”。逆呢？我们可以使用任何反转吗？

是的，setter 和 getter 构成了一个逆函数，我们可以从中创建一个属性：

```F#
let setThenGetShouldGiveSameResult value =
  let obj = Dollar.Create 0
  obj.Amount <- value
  let newValue = obj.Amount
  value = newValue

Check.Quick setThenGetShouldGiveSameResult
// Ok, passed 100 tests.
```

幂等性也很重要。例如，连续做两组应该和只做一组一样。这里有一个属性：

```F#
let setIsIdempotent value =
  let obj = Dollar.Create 0
  obj.Amount <- value
  let afterFirstSet = obj.Amount
  obj.Amount <- value
  let afterSecondSet = obj.Amount
  afterFirstSet = afterSecondSet

Check.Quick setIsIdempotent
// Ok, passed 100 tests.
```

任何“结构归纳”特性？不，与本案无关。

是否有“易于验证”的属性？没什么明显的。

最后，是否有测试预言机？不。同样不相关，尽管如果我们真的在设计一个复杂的货币管理系统，与第三方系统交叉检查我们的结果可能非常有用。

## 不可变美元的属性

忏悔！我在上面的代码中作弊了一点，创建了一个可变类，这就是大多数 OO 对象的设计方式。但是在“TDD by Example”中，Kent 很快意识到了其中的问题，并将其更改为不可变类，所以让我也这样做。

以下是不可变版本：

```F#
type Dollar(amount:int) =
  static member Create amount  =
    Dollar amount
  member val Amount =
    amount
  member this.Add add =
    Dollar (amount + add)
  member this.Times multiplier  =
    Dollar (amount * multiplier)
```

以及一些互动探索：

```F#
let d1 = Dollar.Create 2
d1.Amount  // 2
let d2 = d1.Times 3
d2.Amount  // 6
let d3 = d2.Add 1
d3.Amount  // 7
```

不可变代码的好处在于，我们可以消除对 setter 进行测试的需要，因此我们刚才创建的两个属性现在变得无关紧要了！

说实话，反正它们都很琐碎，所以损失不大。

那么，我们现在可以设计出哪些新的属性呢？

让我们看看 `Times` 的方法。我们如何对此进行测试？我们可以使用哪种策略？

我认为“通往同一结果的不同路径”非常适用。我们可以做对[排序列表](https://fsharpforfunandprofit.com/posts/property-based-testing-3)做过同样的事情，在“内部”和“外部”都做一次乘法运算，看看它们是否给出相同的结果。

![Dollar times](https://fsharpforfunandprofit.com/posts/property-based-testing-5/property_dollar_times.png)

这是用代码表示的属性：

```F#
let createThenTimes_eq_timesThenCreate start multiplier =
  let d1 = Dollar.Create(start).Times(multiplier)
  let d2 = Dollar.Create(start * multiplier)
  d1 = d2
```

太棒了让我们看看它是否有效！

```F#
Check.Quick createThenTimes_eq_timesThenCreate
// Falsifiable, after 1 test
```

哎呀，它不起作用！

为什么不呢？因为我们忘记了 `Dollar` 是一个引用类型，默认情况下不会比较相等！

由于这个错误，我们发现了一个我们可能忽略的属性！让我们在忘记之前对其进行编码。

```F#
let dollarsWithSameAmountAreEqual amount =
  let d1 = Dollar.Create amount
  let d2 = Dollar.Create amount
  d1 = d2

Check.Quick dollarsWithSameAmountAreEqual
// Falsifiable, after 1 test
```

所以现在我们需要通过添加对IEquatable等的支持来解决这个问题。

如果你愿意，你可以这样做——我要切换到F#记录类型，并免费获得相等！

## 美元熟悉 – 版本 3

以下是使用 F# 记录类型重写的 `Dollar`，并添加了相同的方法：

```F#
type Dollar = {amount:int }
  with
  static member Create amount  =
    {amount = amount}
  member this.Add add =
    {amount = this.amount + add }
  member this.Times multiplier  =
    {amount = this.amount * multiplier }
```

现在我们的两个属性都满足了：

```F#
Check.Quick dollarsWithSameAmountAreEqual
// Ok, passed 100 tests.

Check.Quick createThenTimes_eq_timesThenCreate
// Ok, passed 100 tests.
```

我们可以将这种方法扩展到不同的路径。例如，我们可以提取金额并直接进行比较，如下所示：

![Dollar times](https://fsharpforfunandprofit.com/posts/property-based-testing-5/property_dollar_times2.png)

代码看起来像这样：

```F#
let createThenTimesThenGet_eq_times start multiplier =
  let d1 = Dollar.Create(start).Times(multiplier)
  let a1 = d1.amount
  let a2 = start * multiplier
  a1 = a2

Check.Quick createThenTimesThenGet_eq_times
// Ok, passed 100 tests.
```

我们也可以在组合中加入 `Add`。

例如，我们可以通过两条不同的路径执行 `Times` 和 `Add`，如下所示：

![Dollar times](https://fsharpforfunandprofit.com/posts/property-based-testing-5/property_dollar_times3.png)

以下是代码：

```F#
let createThenTimesThenAdd_eq_timesThenAddThenCreate start multiplier adder =
  let d1 = Dollar.Create(start).Times(multiplier).Add(adder)
  let directAmount = (start * multiplier) + adder
  let d2 = Dollar.Create directAmount
  d1 = d2

Check.Quick createThenTimesThenAdd_eq_timesThenAddThenCreate
// Ok, passed 100 tests.
```

因此，这种“不同的路径，相同的结果”的方法非常富有成效，我们可以通过这种方式生成许多路径。

## 美元属性 – 版本 4

那我们就结束吧？我会说不是！

我们开始闻到一股代码的味道。所有这些 `(start * multiplier) + adder` 代码似乎都有点重复逻辑，最终可能会变得脆弱。

我们能否从所有这些案例中提炼出一些共性？

如果我们仔细想想，我们的逻辑实际上是这样的：

- 以某种方式转换类“内部”的数量。
- 以相同的方式转换类“外部”的金额。
- 确保结果相同。

但为了测试这一点，`Dollar` 类必须支持任意转换。按照惯例，这应该被称为 `Map`。

现在，我们所有的测试都可以简化为一个图表：

![Dollar map](https://fsharpforfunandprofit.com/posts/property-based-testing-5/property_dollar_map.png)

让我们为 `Dollar` 添加一个 `Map` 方法。我们还可以根据 `Map` 重写 `Times` 和 `Add`：

```F#
type Dollar = {amount:int }
  with
  static member Create amount  =
    {amount = amount}
  member this.Map f =
    {amount = f this.amount}
  member this.Times multiplier =
    this.Map (fun a -> a * multiplier)
  member this.Add adder =
    this.Map (fun a -> a + adder)
```

现在，我们属性的代码如下：

```F#
let createThenMap_eq_mapThenCreate start f =
  let d1 = Dollar.Create(start).Map f
  let d2 = Dollar.Create(f start)
  d1 = d2
```

但是我们现在怎么测试呢？我们应该传递哪些函数？

别担心！FsCheck 已覆盖您！在这种情况下，FsCheck 实际上也会为您生成随机函数！

试试看——它真的有效！

```F#
Check.Quick createThenMap_eq_mapThenCreate
// Ok, passed 100 tests.
```

我们新的“map”属性比使用“times”的原始属性更通用，因此我们可以安全地消除后者。

### 记录函数参数

目前的属性有点小问题。如果你想知道 FsCheck 正在生成什么函数，那么详细模式没有帮助。

```F#
Check.Verbose createThenMap_eq_mapThenCreate
```

使用 `Check.Verbose` 如上所述，详细给出了输出：

```
0:
18
<fun:Invoke@3000>
1:
7
<fun:Invoke@3000>
-- etc
98:
47
<fun:Invoke@3000>
99:
36
<fun:Invoke@3000>
Ok, passed 100 tests.
```

我们无法说出函数值实际上是什么。

但是，您可以告诉 FsCheck 通过将函数包装在特殊的 `F` 案例中来显示更多有用的信息，如下所示：

```F#
let createThenMap_eq_mapThenCreate_v2 start (F (_,f)) =
  let d1 = Dollar.Create(start).Map f
  let d2 = Dollar.Create(f start)
  d1 = d2
```

现在，当你使用详细模式时…

```F#
Check.Verbose createThenMap_eq_mapThenCreate_v2
```

…您将获得所使用的每个函数的详细日志：

```
0:
0
{ 0->1 }
1:
0
{ 0->0 }
2:
2
{ 2->-2 }
-- etc
98:
-5
{ -5->-52 }
99:
10
{ 10->28 }
Ok, passed 100 tests.
```

每个 `{ 2->-2 }`、`{ 10->28 }` 等表示用于该迭代的函数。

## TDD vs. 基于属性的测试

基于属性的测试（PBT）如何适应 TDD？这是一个常见的问题，所以让我快速地给你我的看法。

首先，TDD 适用于*具体示例（specific examples）*，而 PBT 适用于*通用特性（universal properties）*。

正如我在本系列的[第一篇文章](https://fsharpforfunandprofit.com/posts/property-based-testing)中所说，我认为示例作为设计的一种方式是有用的，并且可以作为一种文档形式。但在我看来，只依赖基于示例的测试是错误的。

与基于示例的测试相比，基于属性的方法具有许多优势：

- 基于属性的测试更为通用，因此其脆性较小。
- 基于属性的测试提供了比一堆示例更好、更简洁的需求描述。
- 因此，一个基于属性的测试可以取代许多基于示例的测试。
- 通过生成随机输入，基于属性的测试通常会揭示您忽略的问题，例如处理空值、丢失数据、除以零、负数等。
- 基于属性的测试迫使你思考。
- 基于属性的测试迫使你有一个干净的设计。

最后两点对我来说是最重要的。编程不是写代码行的问题，而是创建一个满足要求的设计。

因此，任何有助于你深入思考需求和可能出错的东西都应该是你个人工具箱中的关键工具！

例如，在前面的罗马数字帖子中，我们看到接受int是一个坏主意（代码坏了！）。我们有一个快速的解决方案，但实际上我们应该在我们的域中对 `PositiveInteger` 的概念进行建模，然后将我们的代码更改为使用该类型，而不仅仅是 `int`。这展示了使用PBT实际上可以改进你的域模型，而不仅仅只是发现错误。

类似地，在 Dollar 场景中引入 `Map` 方法不仅使测试更容易，而且实际上总体上提高了 Dollar API 的实用性。

不过，回过头来看大局，TDD 和基于属性的测试根本没有冲突。它们都有构建正确程序的共同目标，而且两者都更多地是关于设计而不是编码（想想“测试驱动设计”而不是“测试驱动开发”）。

## 终于结束了

至此，我们结束了一系列关于基于属性的测试！在未来，我们将研究如何创建与您的域匹配的自定义生成器。但就目前而言，是时候停下来了。

我希望您已经发现了一些有用的方法，可以将其应用于您自己的代码。

> 本文中使用的源代码可以在[这里找到](https://github.com/swlaschin/fsharpforfunandprofit.com_code/tree/master/posts/property-based-testing-5)。