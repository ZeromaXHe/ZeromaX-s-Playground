# [返回主 Markdown](./FSharpForFunAndProfit翻译.md)



# 1 没有眼泪的幺半群（Monoids）

*Part of the "Understanding monoids" series (*[link](https://fsharpforfunandprofit.com/posts/monoids-without-tears/#series-toc)*)*

一种常见函数模式的无数学讨论
2013年10月23日这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/monoids-without-tears/

如果你有面向对象的背景，学习函数式编程的一个更具挑战性的方面是缺乏明显的设计模式。有很多习语，如部分应用程序和错误处理技术，但没有 GoF 意义上的明显模式。

在这篇文章中，我们将研究一种非常常见的“模式”，即*幺半群（monoid）*。幺半群并不是一种真正的设计模式；更多的是一种以共同方式处理许多不同类型值的方法。事实上，一旦你了解了幺半群，你就会开始在任何地方看到它们！

不幸的是，“monoid”这个词本身有点令人反感。它最初来自数学，但应用于编程的概念很容易掌握，根本不需要任何数学，正如我希望演示的那样。事实上，如果我们今天在编程环境中命名这个概念，我们可能会称之为 `ICombinable`，这并不那么可怕。

最后，你可能想知道“幺半群”是否与“单子（monad）”有任何联系。是的，它们之间存在数学联系，但在编程方面，尽管名称相似，但它们是非常不同的东西。

## 啊哦…一些方程式

在这个网站上，我通常不使用任何数学，但在这种情况下，我将打破我自己强加的规则，向你展示一些方程式。

准备好了吗？这是第一个：

```
1 + 2 = 3
```

你能应付吗？再来一个怎么样？

```
1 + (2 + 3) = (1 + 2) + 3
```

最后还有一个…

```
1 + 0 = 1 and 0 + 1 = 1
```

好啊！我们结束了！如果你能理解这些方程式，那么你就有了理解幺半群所需的所有数学知识。

## 像数学家一样思考

> “数学家就像画家或诗人一样，是模式的创造者。如果他的模式比他们的模式更持久，那是因为它们是用思想创造的”——G H Hardy

大多数人认为数学家处理数字，做复杂的算术和微积分。

这是一种误解。例如，如果你看看典型的高级数学讨论，你会看到很多奇怪的单词，很多字母和符号，但没有很多算术。

数学家们所做的一件事就是试图在事物中找到模式。“这些东西有什么共同点？”和“我们如何推广这些概念？”是典型的数学问题。

让我们用数学家的眼光来看待这三个方程。

### 推广第一个方程式

数学家会看 `1+2=3`，然后想：

- 我们有很多东西（在这种情况下是整数）
- 我们有办法将两者结合起来（在这种情况下是加法）
- 结果是另一个（也就是说，我们得到另一个整数）

然后，数学家可能会尝试看看这种模式是否可以推广到其他类型的事物和操作。

让我们从将整数作为“事物”开始。还有其他组合整数的方法吗？它们符合模式吗？

让我们试试乘法，这符合这个模式吗？

答案是肯定的，乘法确实符合这种模式，因为将任何两个整数相乘都会得到另一个整数。

那除法呢？这符合模式吗？答案是否定的，因为在大多数情况下，对两个整数进行除法运算会得到一个分数，而这个分数不是整数（我忽略了整数除法）。

`max` 函数呢？这符合模式吗？它组合了两个整数并返回其中一个，所以答案是肯定的。

`equals` 函数呢？它组合了两个整数，但返回的是布尔值，而不是整数，所以答案是否定的。

整数够了！我们还能想到其他什么？

浮点数类似于整数，但与整数不同，使用浮点数除法确实会产生另一个浮点数，因此除法操作符合这种模式。

布尔值怎么样？它们可以使用 AND、OR 等运算符组合。`aBool AND aBool` 会产生另一个 bool吗？对！OR 也符合这种模式。

接下来是字符串。如何将它们结合起来？一种方法是字符串连接，它返回另一个字符串，这就是我们想要的。但是像相等操作这样的东西不适合，因为它返回一个布尔值。

最后，让我们考虑一下列表。对于字符串，组合它们的明显方法是列表连接，它返回另一个列表并符合模式。

我们可以继续这样处理各种对象和组合操作，但现在你应该看看它是如何工作的。

你可能会问：为什么操作返回另一个相同类型的东西如此重要？答案是，**您可以使用该操作将多个对象链接在一起**。

例如，因为 `1 + 2` 是另一个整数，你可以给它加 3。然后，因为 `1 + 2 + 3` 也是整数，你也可以继续加 4。换句话说，正是因为整数加法符合模式，你才能写出这样的加法序列：`1 + 2 + 3 + 4`。你不能用同样的方式写 `1 = 2 = 3 = 4`，因为整数相等不符合这种模式。

当然，组合项目的链可以是我们喜欢的长度。换句话说，这种模式允许我们将成对操作扩展为在列表上工作的操作。

数学家将“结果是这些事情中的另一个”的要求称为*闭包*要求。

### 推广第二个方程式

好的，那么下一个方程式，`1 + (2 + 3) = (1 + 2) + 3` 呢？为什么这很重要？

好吧，如果你考虑第一种模式，它说我们可以建立一个操作链，比如 `1 + 2 + 3`。但我们只进行了配对手术。那么，我们应该按什么顺序进行合并呢？我们应该先将 1 和 2 合并，然后将结果与 3 合并吗？或者我们应该先将 2 和 3 相加，然后将 1 与该结果相加吗？这有区别吗？

这就是第二个方程式有用的地方。它说，对于加法，组合的顺序并不重要。无论哪种方式，你都会得到同样的结果。

因此，对于像这样的四个项目的链：`1 + 2 + 3 + 4`，我们可以从左侧开始工作： `((1+2) + 3) + 4` ，或者从右侧开始工作：`1 + (2 + (3+4))`，甚至可以分为两部分，然后将它们组合起来： `(1+2) + (3+4)`。

让我们看看这个模式是否适用于我们已经看过的例子。

再次，让我们从组合整数的其他方式开始。

我们将再次从乘法开始。 `1 * (2 * 3)` 和 `(1 * 2) * 3` 的结果相同吗？对。就像添加一样，顺序并不重要。

现在让我们试试减法。 `1 - (2 - 3)` 和 `(1 - 2) - 3` 的结果相同吗？不是的。对于减法，顺序确实很重要。

那除法呢？ `12 / (2 / 3)` 与 `(12 / 2) / 3` 的结果相同吗？不是。至于除法，顺序也很重要。

但是 `max` 函数确实有效。`max( max(12,2), 3)` 给出的结果与 `max(12, max(2,3))` 相同。

字符串和列表呢？连接是否符合要求？你怎么认为？

这里有一个问题……我们能为依赖顺序的字符串设计一个操作吗？

那么，像“subtractChars”这样的函数怎么样？它从左字符串中删除右字符串中的所有字符。所以 `subtractChars("abc","ab")` 就是 `"c"`。 `subtractChars` 确实是顺序相关的，正如你可以从一个简单的例子中看到的那样： `subtractChars("abc", subtractChars("abc","abc"))` 与 `subtractChars(subtractChars("abc","abc"),"abc")` 不是同一个字符串。

数学家将“顺序无关紧要”的要求称为*结合性*要求。

**重要提示**：当我说“组合顺序”时，我指的是你执行成对组合步骤的顺序——组合一对，然后将结果与下一项组合。

但至关重要的是，项目的整体顺序保持不变。这是因为对于某些操作，如果更改项目的顺序，则会得到完全不同的结果！`1 - 2` 并不意味着与 `2 - 1` 相同，`2 / 3` 也不意味着与 `3 / 2` 相同。

当然，在许多常见情况下，顺序并不重要。毕竟，`1+2` 等于 `2+1`。在这种情况下，该操作被称为可交换的。

### 第三个方程式

现在让我们来看第三个方程，`1 + 0 = 1`。

数学家会说：这很有趣——有一种特殊的东西（“零”），当你将它与某物结合时，它只会给你原始的东西，就好像什么都没发生过一样。

所以，让我们再次回顾我们的例子，看看我们是否可以将这个“零”概念扩展到其他操作和其他事情。

让我们再次从乘法开始。是否有某种值，当你将一个数字与它相乘时，你会得到原始数字？

当然可以！第一。因此，对于乘法，数字 `1` 是“零”。

`max` 呢？这有“零”吗？对于 32 位整数，是的。组合 `System.Int32.MinValue` 与任何其他使用 `max` 的 32 位整数将返回另一个整数。这完全符合“零”的定义。

使用 AND 组合布尔值怎么样？这个有零吗？对。这就是值 `True`。为什么？因为 `True AND False` 是 `False`, 并且 `True AND True` 也是 `True`。在这两种情况下，另一个值都会原封不动地返回。

使用 `OR` 组合布尔值怎么样？这个也有零吗？我会让你决定的。

接下来，字符串连接怎么样？这个有“零”吗？是的，确实如此——它只是空字符串。

```F#
"" + "hello" = "hello"
"hello" + "" = "hello"
```

最后，对于列表连接，“零”只是空列表。

```F#
[] @ [1;2;3] = [1;2;3]
[1;2;3] @ [] = [1;2;3]
```

你可以看到，“零”值在很大程度上取决于操作，而不仅仅是一组东西。整数加法的零不同于整数乘法的“零”，而整数乘法的零又不同于 `Max` 的“零”。

数学家称“零”为*单位元素*。

### 重新审视方程式

现在，让我们重新审视这些方程，并牢记我们的新概括。

以前，我们有：

```F#
1 + 2 = 3
1 + (2 + 3) = (1 + 2) + 3
1 + 0 = 1 and 0 + 1 = 1
```

但现在我们有了更抽象的东西，一组可以应用于各种事物的通用要求：

- 你从一堆东西开始，一次把它们结合起来。
- **规则1（闭包 Closure）**：将两件事结合起来的结果总是另一件事。
- **规则2（结合性 Associativity）**：当组合两件以上的事情时，你首先进行哪种成对组合并不重要。
- **规则3（单位元素 Identity element）**：有一种特殊的东西叫做“零”，当你把任何东西和“零”组合在一起时，你会得到原始的东西。

有了这些规则，我们可以回到幺半群的定义上来。“幺半群”只是一个遵守所有三条规则的系统。简单！

正如我在开始时所说的，不要让数学背景让你分心。如果程序员给这个模式命名，它可能会被称为“可组合模式”，而不是“幺半群”。但这就是生活。这个术语已经很成熟了，所以我们必须使用它。

请注意，幺半群的定义有两个部分——事物加上相关的操作。monoid 不仅仅是“一堆东西”，而是“一堆事情”和“将它们结合起来的某种方式”。因此，例如，“整数”不是幺半群，但“加法下的整数”是幺半群。

### 半群

在某些情况下，您的系统只遵循前两条规则，并且没有“零”值的候选者。

例如，如果你的域只由严格的正数组成，那么在加法下，它们是封闭的和可结合的，但没有可以为“零”的正数。

另一个例子可能是有限列表的交集。它是封闭和可结合的，但没有（有限）列表在与任何其他有限列表相交时保持不变。

这种系统仍然非常有用，数学家称之为“半群”，而不是幺半群。幸运的是，有一个技巧可以将任何半群转换为幺半群（我稍后会描述）。

### 分类表

让我们把所有的例子放在一个表中，这样你就可以把它们放在一起看。

| 事物             | 操作            | 封闭?      | 可结合?   | 单位元?         | 类型      |
| :--------------- | :-------------- | :--------- | :-------- | :-------------- | :-------- |
| Int32            | 加法            | Yes        | Yes       | 0               | Monoid    |
| Int32            | 乘法            | Yes        | Yes       | 1               | Monoid    |
| Int32            | 减法            | Yes        | No        | 0               | Other     |
| Int32            | Max             | Yes        | Yes       | Int32.MinValue  | Monoid    |
| Int32            | 判等            | No         |           |                 | Other     |
| Int32            | 小于            | No         |           |                 | Other     |
|                  |                 |            |           |                 |           |
| Float            | 乘法            | Yes        | No (注 1) | 1               | Other     |
| Float            | 除法            | Yes (注 2) | No        | 1               | Other     |
|                  |                 |            |           |                 |           |
| Positive Numbers | 加法            | Yes        | Yes       | No identity     | Semigroup |
| Positive Numbers | 乘法            | Yes        | Yes       | 1               | Monoid    |
|                  |                 |            |           |                 |           |
| Boolean          | AND             | Yes        | Yes       | true            | Monoid    |
| Boolean          | OR              | Yes        | Yes       | false           | Monoid    |
|                  |                 |            |           |                 |           |
| String           | 连接            | Yes        | Yes       | Empty string "" | Monoid    |
| String           | 判等            | No         |           |                 | Other     |
| String           | "subtractChars" | Yes        | No        | Empty string "" | Other     |
|                  |                 |            |           |                 |           |
| List             | 连接            | Yes        | Yes       | Empty list []   | Monoid    |
| List             | 交集            | Yes        | Yes       | No identity     | Semigroup |

你还可以在这个列表中添加许多其他类型的东西；多项式、矩阵、概率分布等等。这篇文章不会讨论它们，但一旦你了解了单体的概念，你就会发现这个概念可以应用于各种各样的事情。

[注1]正如 Doug 在评论中指出的那样，浮点数不是结合的。将“float”替换为“实数”以获得结合性。

[注2]数学实数在除法下不是闭的，因为你不能除以零得到另一个实数。然而，使用 IEEE 浮点数，你可以除以零得到一个有效值。所以，在分裂的情况下，浮子确实是关闭的！下面是一个演示：

```F#
let x = 1.0/0.0 // infinity
let y = x * 2.0 // two times infinity
let z = 2.0 / x // two divided by infinity
```

## 幺半群对程序员有什么用？

到目前为止，我们已经描述了一些抽象概念，但它们对现实世界的编程问题有什么好处呢？

### 闭包的好处

正如我们所看到的，闭包规则的好处是，您可以将成对操作转换为对列表或序列有效的操作。

换句话说，如果我们能定义一个成对（pairwise）操作，我们就可以“免费”将其扩展到列表操作。

执行此操作的函数通常称为“reduce”。以下是一些示例：

| Explicit                | 使用 reduce                                 |
| :---------------------- | :------------------------------------------ |
| `1 + 2 + 3 + 4`         | `[ 1; 2; 3; 4 ] |> List.reduce (+)`         |
| `1 * 2 * 3 * 4`         | `[ 1; 2; 3; 4 ] |> List.reduce (*)`         |
| `"a" + "b" + "c" + "d"` | `[ "a"; "b"; "c"; "d" ] |> List.reduce (+)` |
| `[1] @ [2] @ [3] @ [4]` | `[ [1]; [2]; [3]; [4] ] |> List.reduce (@)` |

您可以看到，`reduce` 可以被认为是在列表的每个元素之间插入指定的操作。

请注意，在最后一个示例中，`reduce` 的输入是一个列表列表，输出是一个单一的列表。确保你理解这是为什么。

### 结合性的好处

如果成对组合可以按任何顺序完成，那么就开辟了一些有趣的实现技术，例如：

- 分而治之算法
- 并行化
- 递增主义

这些都是深奥的话题，但让我们快速浏览一下！

#### 分而治之算法

考虑对前 8 个整数求和的任务；我们如何实现这一点？

一种方法是粗略的逐步求和，如下所示：

```F#
let sumUpTo2 = 1 + 2
let sumUpTo3 = sumUpTo2 + 3
let sumUpTo4 = sumUpTo3 + 4
// etc
let result = sumUpTo7 + 8
```

但是，由于总和可以按任何顺序完成，我们也可以通过将总和分成两半来实现这一要求，如下所示

```F#
let sum1To4 = 1 + 2 + 3 + 4
let sum5To8 = 5 + 6 + 7 + 8
let result = sum1To4 + sum5To8
```

然后我们可以以相同的方式递归地将和拆分为子和，直到我们进行基本的成对操作：

```F#
let sum1To2 = 1 + 2
let sum3To4 = 3 + 4
let sum1To4 = sum1To2 + sum3To4
```

对于简单的求和，这种“分而治之”的方法可能看起来有些矫枉过正，但我们将在未来的文章中看到，结合 `map`，它是一些众所周知的聚合算法的基础。

#### 并行化

一旦我们有了分而治之的策略，它就可以很容易地转换为并行算法。

例如，要在四核 CPU 上对前 8 个整数求和，我们可以这样做：

|        | Core 1                    | Core 2                    | Core 3          | Core 4          |
| :----- | :------------------------ | :------------------------ | :-------------- | :-------------- |
| Step 1 | `sum12 = 1 + 2`           | `sum34 = 3 + 4`           | `sum56 = 5 + 6` | `sum78 = 7 + 8` |
| Step 2 | `sum1234 = sum12 + sum34` | `sum5678 = sum56 + sum78` | (idle)          | (idle)          |
| Step 3 | `sum1234 + sum5678`       | (idle)                    | (idle)          | (idle)          |

还有七项计算需要完成，但由于我们是并行进行的，我们可以分三步完成。

同样，这似乎是一个微不足道的例子，但 Hadoop 等大数据系统都是关于聚合大量数据的，如果聚合操作是一个幺半群，那么理论上，你可以通过使用多台机器轻松扩展这些聚合*。

*当然，在实践中，魔鬼在于细节，现实世界的系统并不完全是这样工作的。

#### 递增主义

即使你不需要并行性，幺半群的一个很好的特性是它们支持增量计算。

例如，假设你让我计算 1 到 5 的和。那么，我当然会给你十五分的答案。

但现在你说你改变主意了，你想要 1 到 6 的和。我必须从头开始把所有的数字加起来吗？不，我可以使用前面的总和，然后逐步加 6。这是可能的，因为整数加法是一个单体。

也就是说，当面对像 `1 + 2 + 3 + 4 + 5 + 6` 这样的和时，我可以按照我喜欢的任何方式对数字进行分组。特别是，我可以这样做一个增量和：`(1 + 2 + 3 + 4 + 5) + 6`，然后减少到 `15 + 6`。

在这种情况下，从头开始重新计算整个总数可能不是什么大问题，但考虑一个现实世界的例子，比如网络分析，计算过去 30 天的访问者数量。一个简单的实现可能是通过解析过去 30 天数据的日志来计算数字。一种更有效的方法是认识到之前的 29 天没有变化，只处理一天的增量变化。因此，解析工作量大大减少。

同样，如果你有一本 100 页的书的字数，并且你添加了另一页，你不需要再次解析所有 101 页。你只需要数最后一页的单词，并将其添加到上一页的总数中*

*从技术上讲，这些听起来很可怕的幺半群同态（monoid homomorphisms）。我将在下一篇文章中解释这是什么。

### 单位元的好处

具有单位元素并不总是必需的。有一个封闭的结合运算（即半群）就足以做许多有用的事情。

但在某些情况下，这还不够。例如，以下是一些可能出现的情况：

- 如何在空列表上使用 `reduce`？
- 如果我正在设计一个分治算法，如果其中一个“分治”步骤中什么都没有，我该怎么办？
- 当使用增量算法时，当我没有数据时，我应该从哪个值开始？

在所有情况下，我们都需要一个“零”值。例如，这允许我们说空列表的总和为 `0`。

关于上面的第一点，如果我们担心列表可能是空的，那么我们必须用 `fold` 替换 `reduce`，这允许传入初始值。（当然，`fold` 可以用于更多的事情，而不仅仅是 monoid 操作。）

以下是 `reduce` 和 `fold` 的作用：

```F#
// ok
[1..10] |> List.reduce (+)

// error
[] |> List.reduce (+)

// ok with explicit zero
[1..10] |> List.fold (+) 0

// ok with explicit zero
[] |> List.fold (+) 0
```

使用“零”有时会导致违反直觉的结果。例如，一个空整数列表的乘积是多少？

答案是 `1`，而不是你所期望的 `0`！以下是证明这一点的代码：

```F#
[1..4] |> List.fold (*) 1  // result is 24
[] |> List.fold (*) 1      // result is 1
```

### 好处概述

总之，幺半群基本上是一种描述聚合模式的方式——我们有一个事物列表，我们有一些组合它们的方法，最后我们得到一个聚合的对象。

或者用 F# 术语来说：

```F#
Monoid Aggregation : 'T list -> 'T
```

因此，当你在设计代码时，你开始使用“求和”、“乘积”、“组合”或“连接”等术语，这些都是你在处理单体的线索。

## 下一步

现在我们了解了什么是幺半群，让我们看看如何在实践中使用它们。

在本系列的下一篇文章中，我们将探讨如何编写实现 monoid“模式”的真实代码。

# 2 实践中的幺半群

*Part of the "Understanding monoids" series (*[link](https://fsharpforfunandprofit.com/posts/monoids-part2/#series-toc)*)*

没有眼泪的幺半群-第 2 部分
2013年10月24日 这篇文章已有3年多的历史了

https://fsharpforfunandprofit.com/posts/monoids-part2/

在上一篇文章中，我们研究了幺半群的定义。在这篇文章中，我们将看到如何实现一些幺半群。

首先，让我们重新审视一下定义：

- 你从一堆东西开始，一次把它们结合起来。
- **规则1（闭包 Closure）**：将两件事结合起来的结果总是另一件事。
- **规则2（结合性 Associativity）**：当组合两件以上的事情时，你首先进行哪种成对组合并不重要。
- **规则3（单位元素 Identity element）**：有一种特殊的东西叫做“零”，当你把任何东西和“零”组合在一起时，你会得到原始的东西。

例如，如果字符串是事物，字符串连接是操作，那么我们就有一个幺半群。以下是一些代码来演示这一点：

```F#
let s1 = "hello"
let s2 = " world!"

// closure
let sum = s1 + s2  // sum is a string

// associativity
let s3 = "x"
let s4a = (s1+s2) + s3
let s4b = s1 + (s2+s3)
assert (s4a = s4b)

// an empty string is the identity
assert (s1 + "" = s1)
assert ("" + s1 = s1)
```

但现在让我们尝试将其应用于更复杂的对象。

假设我们有一个 `OrderLine`，一个代表销售订单中一行的小结构。

```F#
type OrderLine = {
    ProductCode: string
    Qty: int
    Total: float
    }
```

然后，也许我们想找到订单的总计，也就是说，我们想对行列表的 `Total` 字段求和。

标准的命令式方法是创建一个局部 `total` 变量，然后循环遍历这些行，边做边求和，如下所示：

```F#
let calculateOrderTotal lines =
    let mutable total = 0.0
    for line in lines do
        total <- total + line.Total
    total
```

让我们试试：

```F#
module OrdersUsingImperativeLoop =

    type OrderLine = {
        ProductCode: string
        Qty: int
        Total: float
        }

    let calculateOrderTotal lines =
        let mutable total = 0.0
        for line in lines do
            total <- total + line.Total
        total

    let orderLines = [
        {ProductCode="AAA"; Qty=2; Total=19.98}
        {ProductCode="BBB"; Qty=1; Total=1.99}
        {ProductCode="CCC"; Qty=3; Total=3.99}
        ]

    orderLines
    |> calculateOrderTotal
    |> printfn "Total is %g"
```

当然，作为一名经验丰富的函数式程序员，你会对此嗤之以鼻，并在 `calculateOrderTotal` 中使用 `fold`，如下所示：

```F#
module OrdersUsingFold =

    type OrderLine = {
        ProductCode: string
        Qty: int
        Total: float
        }

    let calculateOrderTotal lines =
        let accumulateTotal total line =
            total + line.Total
        lines
        |> List.fold accumulateTotal 0.0

    let orderLines = [
        {ProductCode="AAA"; Qty=2; Total=19.98}
        {ProductCode="BBB"; Qty=1; Total=1.99}
        {ProductCode="CCC"; Qty=3; Total=3.99}
        ]

    orderLines
    |> calculateOrderTotal
    |> printfn "Total is %g"
```

到目前为止，一切顺利。现在让我们来看一个使用幺半群方法的解决方案。

对于幺半群，我们需要定义某种加法或组合运算。这样怎么样？

```F#
let addLine orderLine1 orderLine2 =
    orderLine1.Total + orderLine2.Total
```

但这并不好，因为我们忘记了幺半群的一个关键方面。加法必须返回相同类型的值！

如果我们查看 `addLine` 函数的签名…

```F#
addLine : OrderLine -> OrderLine -> float
```

…我们可以看到返回类型是 `float` 而不是 `OrderLine`。

我们需要做的是返回整个其他 `OrderLine`。以下是一个正确的实现：

```F#
let addLine orderLine1 orderLine2 =
    {
    ProductCode = "TOTAL"
    Qty = orderLine1.Qty + orderLine2.Qty
    Total = orderLine1.Total + orderLine2.Total
    }
```

现在签名是正确的：`addLine : OrderLine -> OrderLine -> OrderLine`。

请注意，因为我们必须返回整个结构，所以我们还必须为 `ProductCode` 和 `Qty` 指定一些内容，而不仅仅是总数。数量很容易，我们只需求和即可。对于 `ProductCode`，我决定使用字符串“TOTAL”，因为我们没有可以使用的真实产品代码。

让我们做一个小测试：

```F#
// utility method to print an OrderLine
let printLine {ProductCode=p; Qty=q;Total=t} =
    printfn "%-10s %5i %6g" p q t

let orderLine1 = {ProductCode="AAA"; Qty=2; Total=19.98}
let orderLine2 = {ProductCode="BBB"; Qty=1; Total=1.99}

//add two lines to make a third
let orderLine3 = addLine orderLine1 orderLine2
orderLine3 |> printLine // and print it
```

我们应该得到这样的结果：

```
TOTAL          3  21.97
```

*注意：有关使用的 printf 格式选项的更多信息，请参阅此处关于 printf 的帖子。*

现在，让我们使用 `reduce` 将其应用于列表：

```F#
let orderLines = [
    {ProductCode="AAA"; Qty=2; Total=19.98}
    {ProductCode="BBB"; Qty=1; Total=1.99}
    {ProductCode="CCC"; Qty=3; Total=3.99}
    ]

orderLines
|> List.reduce addLine
|> printLine
```

结果：

```
TOTAL          6  25.96
```

起初，这可能看起来像是额外的工作，只是为了加起来。但请注意，我们现在拥有的信息不仅仅是总数；我们也有数量的总和。

例如，我们可以很容易地重用 `printLine` 函数来制作一个简单的收据打印函数，其中包括总计，如下所示：

```F#
let printReceipt lines =
    lines
    |> List.iter printLine

    printfn "-----------------------"

    lines
    |> List.reduce addLine
    |> printLine

orderLines
|> printReceipt
```

输出如下：

```
AAA            2  19.98
BBB            1   1.99
CCC            3   3.99
-----------------------
TOTAL          6  25.96
```

更重要的是，我们现在可以使用幺半群的增量特性来保持一个正在运行的小计（subtotal），每次添加新行时我们都会更新它。

这里有一个例子：

```F#
let subtotal = orderLines |> List.reduce addLine
let newLine = {ProductCode="DDD"; Qty=1; Total=29.98}
let newSubtotal = subtotal |> addLine newLine
newSubtotal |> printLine
```

我们甚至可以定义一个自定义运算符，如 `++`，这样我们就可以自然地将行加在一起，因为它们是数字：

```F#
let (++) a b = addLine a b  // custom operator

let newSubtotal = subtotal ++ newLine
```

你可以看到，使用幺半群模式开辟了一种全新的思维方式。您可以将这种“添加”方法应用于几乎任何类型的对象。

例如，一个产品“加”一个产品会是什么样子？或者一个客户“加”一个客户？让你的想象力驰骋吧！

## 我们到了吗？

你可能已经注意到我们还没有完全完成。我们还没有讨论过幺半群的第三个要求——零或单位元素。

在这种情况下，该要求意味着我们需要某种 `OrderLine`，这样将其添加到另一个订单行时，原始订单行将保持不变。我们有这样的东西吗？

现在没有，因为加法操作总是将产品代码更改为“TOTAL”。我们现在拥有的实际上是半群，而不是幺半群。

正如你所看到的，半群是完全可用的。但是，如果我们有一个空的行列表，我们想把它们加起来，就会出现问题。结果应该是什么？

一种解决方法是更改 `addLine` 函数以忽略空产品代码。然后我们可以使用一个空代码的订单行作为零元素。

我的意思是：

```F#
let addLine orderLine1 orderLine2 =
    match orderLine1.ProductCode, orderLine2.ProductCode with
    // is one of them zero? If so, return the other one
    | "", _ -> orderLine2
    | _, "" -> orderLine1
    // anything else is as before
    | _ ->
        {
        ProductCode = "TOTAL"
        Qty = orderLine1.Qty + orderLine2.Qty
        Total = orderLine1.Total + orderLine2.Total
        }

let zero = {ProductCode=""; Qty=0; Total=0.0}
let orderLine1 = {ProductCode="AAA"; Qty=2; Total=19.98}
```

然后，我们可以测试单位元（identity）是否按预期工作：

```F#
assert (orderLine1 = addLine orderLine1 zero)
assert (orderLine1 = addLine zero orderLine1)
```

这似乎有点老套，所以我一般不会推荐这种技术。还有另一种获得单位元的方法，我们稍后会讨论。

## 引入一种特殊的总类型

在上面的示例中，`OrderLine` 类型非常简单，很容易重载总计字段。

但是，如果 `OrderLine` 类型更复杂，会发生什么？例如，如果它也有一个 `Price` 字段，如下所示：

```F#
type OrderLine = {
    ProductCode: string
    Qty: int
    Price: float
    Total: float
    }
```

现在我们引入了一个复杂因素。当我们将两条线合并时，我们应该将 `Price` 设置为多少？平均价格？没有价格？

```F#
let addLine orderLine1 orderLine2 =
    {
    ProductCode = "TOTAL"
    Qty = orderLine1.Qty + orderLine2.Qty
    Price = 0 // or use average price?
    Total = orderLine1.Total + orderLine2.Total
    }
```

两者似乎都不太令人满意。

我们不知道该怎么办的事实可能意味着我们的设计是错误的。

真的，我们只需要总数据的一个子集，而不是全部。我们如何表示这一点？

当然是一个可区分联合！一个案例可用于产品行，另一个案例仅可用于总计。

我的意思是：

```F#
type ProductLine = {
    ProductCode: string
    Qty: int
    Price: float
    LineTotal: float
    }

type TotalLine = {
    Qty: int
    OrderTotal: float
    }

type OrderLine =
    | Product of ProductLine
    | Total of TotalLine
```

这个设计好多了。我们现在有一个专门用于总计的特殊结构，我们不必使用扭曲来拟合多余的数据。我们甚至可以删除虚拟的“TOTAL”产品代码。

*请注意，我在每条记录中对“总计”字段的命名不同。像这样具有唯一的字段名意味着您不必总是显式指定类型。*

不幸的是，加法逻辑现在更复杂了，因为我们必须处理每种情况的组合：

```F#
let addLine orderLine1 orderLine2 =
    let totalLine =
        match orderLine1,orderLine2 with
        | Product p1, Product p2 ->
            {Qty = p1.Qty + p2.Qty;
            OrderTotal = p1.LineTotal + p2.LineTotal}
        | Product p, Total t ->
            {Qty = p.Qty + t.Qty;
            OrderTotal = p.LineTotal + t.OrderTotal}
        | Total t, Product p ->
            {Qty = p.Qty + t.Qty;
            OrderTotal = p.LineTotal + t.OrderTotal}
        | Total t1, Total t2 ->
            {Qty = t1.Qty + t2.Qty;
            OrderTotal = t1.OrderTotal + t2.OrderTotal}
    Total totalLine // wrap totalLine to make OrderLine
```

请注意，我们不能只返回 `TotalLine` 值。我们必须使用 `Total` 案例来制作一个合适的 `OrderLine`。如果我们不这样做，那么我们的 `addLine` 将具有签名 `OrderLine -> OrderLine -> TotalLine`，这是不正确的。我们必须有签名 `OrderLine -> OrderLine -> OrderLine`——其他什么都不行！

现在我们有两种情况，我们需要在 `printLine` 函数中处理这两种情况：

```F#
let printLine =  function
    | Product {ProductCode=p; Qty=q; Price=pr; LineTotal=t} ->
        printfn "%-10s %5i @%4g each %6g" p q pr t
    | Total {Qty=q; OrderTotal=t} ->
        printfn "%-10s %5i            %6g" "TOTAL" q t
```

但是一旦我们做到了这一点，我们现在可以像以前一样使用加法：

```F#
let orderLine1 = Product {ProductCode="AAA"; Qty=2; Price=9.99; LineTotal=19.98}
let orderLine2 = Product {ProductCode="BBB"; Qty=1; Price=1.99; LineTotal=1.99}
let orderLine3 = addLine orderLine1 orderLine2

orderLine1 |> printLine
orderLine2 |> printLine
orderLine3 |> printLine
```

### 再次单位元

同样，我们还没有处理单位元要求。我们可以尝试使用与以前相同的技巧，使用空白产品代码，但这仅适用于 `Product` 案例。

为了获得一个正确的身份，我们真的需要在联合类型中引入第三种情况，比如 `EmptyOrder`：

```F#
type ProductLine = {
    ProductCode: string
    Qty: int
    Price: float
    LineTotal: float
    }

type TotalLine = {
    Qty: int
    OrderTotal: float
    }

type OrderLine =
    | Product of ProductLine
    | Total of TotalLine
    | EmptyOrder
```

有了这个额外的案例，我们重写了 `addLine` 函数来处理它：

```F#
let addLine orderLine1 orderLine2 =
    match orderLine1,orderLine2 with
    // is one of them zero? If so, return the other one
    | EmptyOrder, _ -> orderLine2
    | _, EmptyOrder -> orderLine1
    // otherwise as before
    | Product p1, Product p2 ->
        Total { Qty = p1.Qty + p2.Qty;
        OrderTotal = p1.LineTotal + p2.LineTotal}
    | Product p, Total t ->
        Total {Qty = p.Qty + t.Qty;
        OrderTotal = p.LineTotal + t.OrderTotal}
    | Total t, Product p ->
        Total {Qty = p.Qty + t.Qty;
        OrderTotal = p.LineTotal + t.OrderTotal}
    | Total t1, Total t2 ->
        Total {Qty = t1.Qty + t2.Qty;
        OrderTotal = t1.OrderTotal + t2.OrderTotal}
```

现在我们可以测试它：

```F#
let zero = EmptyOrder

// test identity
let productLine = Product {ProductCode="AAA"; Qty=2; Price=9.99; LineTotal=19.98}
assert (productLine = addLine productLine zero)
assert (productLine = addLine zero productLine)

let totalLine = Total {Qty=2; OrderTotal=19.98}
assert (totalLine = addLine totalLine zero)
assert (totalLine = addLine zero totalLine)
```

## 使用内置的 List.sum 函数

原来 `List.sum` 函数知道幺半群！如果你告诉它加法操作是什么，零是什么，那么你可以直接使用 `List.sum` 而不是 `List.fold`。

你这样做的方式是将两个静态成员 `+` 和 `Zero` 附加到你的类型上，如下所示：

```F#
type OrderLine with
    static member (+) (x,y) = addLine x y
    static member Zero = EmptyOrder   // a property
```

完成此操作后，您可以使用 `List.sum`，它将按预期工作。

```F#
let lines1 = [productLine]
// using fold with explicit op and zero
lines1 |> List.fold addLine zero |> printfn "%A"
// using sum with implicit op and zero
lines1 |> List.sum |> printfn "%A"

let emptyList: OrderLine list = []
// using fold with explicit op and zero
emptyList |> List.fold addLine zero |> printfn "%A"
// using sum with implicit op and zero
emptyList |> List.sum |> printfn "%A"
```

请注意，要使其工作，您必须还没有一个名为 `Zero` 的方法或案例。如果我在第三种情况下使用 `Zero` 而不是 `EmptyOrder`，它就不会起作用。

虽然这是一个巧妙的技巧，但在实践中，我认为这不是一个好主意，除非你定义了一个适当的数学相关类型，如 `ComplexNumber` 或 `Vector`。这对我来说有点太聪明，也不太明显。

如果你真的想使用这个技巧，你的 `Zero` 成员不能是扩展方法——它必须用类型定义。

例如，在下面的代码中，我试图将空字符串定义为字符串的“零”。

`List.fold` 有效，因为 `String.Zero` 在这里作为扩展方法可见，但 `List.sum` 失败，因为扩展方法对它不可见。

```F#
module StringMonoid =

    // define extension method
    type System.String with
        static member Zero = ""

    // OK.
    ["a";"b";"c"]
    |> List.reduce (+)
    |> printfn "Using reduce: %s"

    // OK. String.Zero is visible as an extension method
    ["a";"b";"c"]
    |> List.fold (+) System.String.Zero
    |> printfn "Using fold: %s"

    // Error. String.Zero is NOT visible to List.sum
    ["a";"b";"c"]
    |> List.sum
    |> printfn "Using sum: %s"
```

## 映射到不同的结构

在订单行情况下，在联合中有两个不同的情况可能是可以接受的，但在许多现实世界的情况下，这种方法太复杂或令人困惑。

考虑这样的客户记录：

```F#
open System

type Customer = {
    Name:string // and many more string fields!
    LastActive:DateTime
    TotalSpend:float }
```

我们如何“添加”其中两个客户？

一个有用的提示是要意识到聚合实际上只适用于数字和类似类型。字符串真的不容易聚合。

因此，与其尝试聚合 `Customer`，不如定义一个单独的类 `CustomerStats`，其中包含所有可聚合的信息：

```F#
// create a type to track customer statistics
type CustomerStats = {
    // number of customers contributing to these stats
    Count:int
    // total number of days since last activity
    TotalInactiveDays:int
    // total amount of money spent
    TotalSpend:float }
```

`CustomerStats` 中的所有字段都是数字，因此很明显我们可以将两个统计数据添加在一起：

```F#
let add stat1 stat2 = {
    Count = stat1.Count + stat2.Count;
    TotalInactiveDays = stat1.TotalInactiveDays + stat2.TotalInactiveDays
    TotalSpend = stat1.TotalSpend + stat2.TotalSpend
    }

// define an infix version as well
let (++) a b = add a b
```

与往常一样，`add` 函数的输入和输出必须是相同的类型。我们必须有 `CustomerStats -> CustomerStats -> CustomerStats`，而不是 `Customer -> Customer -> CustomerStats` 或任何其他变体。

好的，到目前为止一切顺利。

现在，假设我们有一组客户，我们想获得他们的汇总统计数据，我们应该如何做到这一点？

我们不能直接添加客户，所以我们需要做的是首先将每个客户转换为 `CustomerStats`，然后使用 monoid 操作添加统计数据。

这里有一个例子：

```F#
// convert a customer to a stat
let toStats cust =
    let inactiveDays= DateTime.Now.Subtract(cust.LastActive).Days;
    {Count=1; TotalInactiveDays=inactiveDays; TotalSpend=cust.TotalSpend}

// create a list of customers
let c1 = {Name="Alice"; LastActive=DateTime(2005,1,1); TotalSpend=100.0}
let c2 = {Name="Bob"; LastActive=DateTime(2010,2,2); TotalSpend=45.0}
let c3 = {Name="Charlie"; LastActive=DateTime(2011,3,3); TotalSpend=42.0}
let customers = [c1;c2;c3]

// aggregate the stats
customers
|> List.map toStats
|> List.reduce add
|> printfn "result = %A"
```

首先要注意的是，`toStats` 只为一个客户创建统计数据。我们把计数设为 1。这可能看起来有点奇怪，但确实有道理，因为如果列表中只有一个客户，那么汇总统计数据就是这样。

第二点要注意的是最终聚合是如何完成的。首先，我们使用 `map` 将源类型转换为幺半群类型，然后我们使用 `reduce` 聚合所有统计数据。

嗯…。`map` 之后是 `reduce`。你听起来熟悉吗？

确实，谷歌著名的 MapReduce 算法受到了这一概念的启发（尽管细节有所不同）。

在我们继续之前，这里有一些简单的练习来检查你的理解。

- `CustomerStats` 的“零”是什么？在空列表上使用 `List.fold` 测试您的代码。
- 编写一个简单的 `OrderStats` 类，并使用它来聚合我们在本文开头介绍的 `OrderLine` 类型。

## 幺半群同态

我们现在已经拥有了理解一种称为*幺半群同态*的东西所需的所有工具。

我知道你在想什么……啊！不仅仅是一个，而是两个奇怪的数学单词！

但我希望“monoid”这个词现在不再那么吓人了。“同态（homomorphism）”是另一个比听起来更简单的数学单词。这只是希腊语中“相同形状”的意思，它描述了一种保持“形状”不变的映射或函数。

这在实践中意味着什么？

好吧，我们已经看到所有的单体都有某种共同的结构。也就是说，尽管底层对象可能非常不同（整数、字符串、列表、`CustomerStats` 等），但它们的“幺半群性（monoidness）”是相同的。正如乔治·W·布什曾经说过的那样，一旦你看到一个幺半群，你就看到了所有的。

因此，即使“之前”和“之后”的对象完全不同，幺半群同态也是一种保持基本“幺半群性”的变换。

在本节中，我们将介绍一个简单的幺半群同态。这是“hello world”，“fibonacci 级数”，是幺半群同态的一种——字数统计。

### 文档作为幺半群

假设我们有一个表示文本块的类型，类似于这样：

```F#
type Text = Text of string
```

当然，我们可以添加两个较小的文本块来制作一个较大的文本块：

```F#
let addText (Text s1) (Text s2) =
    Text (s1 + s2)
```

下面是一个添加工作原理的示例：

```F#
let t1 = Text "Hello"
let t2 = Text " World"
let t3 = addText t1 t2
```

既然你现在是专家，你很快就会认出这是一个幺半群，零显然是 `Text ""`。

现在，假设我们正在写一本书（比如这本书），我们想要一个字数统计来显示我们写了多少。

这是一个非常粗糙的实现，还有一个测试：

```F#
let wordCount (Text s) =
    s.Split(' ').Length

// test
Text "Hello world"
|> wordCount
|> printfn "The word count is %i"
```

所以我们正在写作，现在我们已经完成了三页的文本。我们如何计算完整文档的字数？

一种方法是将单独的页面添加在一起，形成一个完整的文本块，然后将 `wordCount` 函数应用于该文本块。这是一个图表：

【通过添加页面进行字数统计】

但每次我们完成一个新页面时，我们都必须将所有文本加在一起，并重新进行字数统计。

毫无疑问，你可以看到有更好的方法。与其将所有文本加在一起然后计数，不如分别计算每页的字数，然后将这些计数加起来，如下所示：

【通过添加计数进行字数统计】

第二种方法依赖于这样一个事实，即整数（计数）本身就是一个单体，您可以将它们相加以获得所需的结果。

因此，`wordCount` 函数将“页面”的聚合转换为“计数”的聚合。

现在最大的问题是：`wordCount` 是一个幺半群同态吗？

好吧，页面（文本）和计数（整数）都是幺半群，所以它肯定会将一个幺半群转换为另一个。

但更微妙的条件是：它能保持“形状”吗？也就是说，添加计数是否与添加页面给出了相同的答案？

在这种情况下，答案是肯定的。所以 `wordCount` 是一个幺半群同态！

你可能会认为这是显而易见的，所有这样的映射都必须是幺半群同态，但我们稍后会看到一个例子，其中这是不正确的。

### 组块性的好处

幺半群同态方法的优点是它是“可分块的（chunkable）”。

每个地图和字数统计都是独立的，所以我们可以分别进行，然后将答案加起来。对于许多算法来说，处理小块数据比处理大块数据要高效得多，所以如果可以的话，我们应该尽可能地利用这一点。

作为这种可分块性的直接结果，我们得到了上一篇文章中提到的一些好处。

首先，它是渐进的（incremental）。也就是说，当我们在最后一页添加文本时，我们不必重新计算所有前一页的字数，这可能会节省一些时间。

其次，它是可并行的（parallelizable）。每个块的工作可以在不同的内核或机器上独立完成。请注意，在实践中，并行性被高估了。可分块性对性能的影响比并行性本身大得多。

### 比较字数实现

我们现在准备创建一些代码来演示这两种不同的技术。

让我们从上面的基本定义开始，除了我将把字数改为使用正则表达式而不是 `split`。

```F#
module WordCountTest =
    open System

    type Text = Text of string

    let addText (Text s1) (Text s2) =
        Text (s1 + s2)

    let wordCount (Text s) =
        System.Text.RegularExpressions.Regex.Matches(s,@"\S+").Count
```

接下来，我们将创建一个 1000 字的页面和一个 1000 页的文档。

```F#
module WordCountTest =

    // code as above

    let page() =
        List.replicate 1000 "hello "
        |> List.reduce (+)
        |> Text

    let document() =
        page() |> List.replicate 1000
```

我们想对代码进行计时，看看实现之间是否有任何差异。这是一个小助手函数。

```F#
module WordCountTest =

    // code as above

    let time f msg =
        let stopwatch = Diagnostics.Stopwatch()
        stopwatch.Start()
        f()
        stopwatch.Stop()
        printfn "Time taken for %s was %ims" msg stopwatch.ElapsedMilliseconds
```

好的，让我们实现第一种方法。我们将使用 `addText` 将所有页面添加在一起，然后对整个百万字文档进行字数统计。

```F#
module WordCountTest =

    // code as above

    let wordCountViaAddText() =
        document()
        |> List.reduce addText
        |> wordCount
        |> printfn "The word count is %i"

    time wordCountViaAddText "reduce then count"
```

对于第二种方法，我们将首先在每个页面上执行 `wordCount`，然后将所有结果加在一起（当然使用 `reduce`）。

```F#
module WordCountTest =

    // code as above

    let wordCountViaMap() =
        document()
        |> List.map wordCount
        |> List.reduce (+)
        |> printfn "The word count is %i"

    time wordCountViaMap "map then reduce"
```

请注意，我们只更改了两行代码！

在 `wordCountViaAddText` 中，我们有：

```F#
|> List.reduce addText
|> wordCount
```

在 `wordCountViaMap` 中，我们基本上交换了这些行。我们现在先执行 `wordCount`，然后再执行 `reduce`，如下所示：

```F#
|> List.map wordCount
|> List.reduce (+)
```

最后，让我们看看并行性有什么不同。我们将使用内置的 `Array.Parallel.map` 而不是 `List.map`，这意味着我们需要先将列表转换为数组。

```F#
module WordCountTest =

    // code as above

    let wordCountViaParallelAddCounts() =
        document()
        |> List.toArray
        |> Array.Parallel.map wordCount
        |> Array.reduce (+)
        |> printfn "The word count is %i"

    time wordCountViaParallelAddCounts "parallel map then reduce"
```

我希望您正在关注实现，并了解正在发生的事情。

### 分析结果

以下是在我的 4 核机器上运行的不同实现的结果：

```
Time taken for reduce then count was 7955ms
Time taken for map then reduce was 698ms
Time taken for parallel map then reduce was 603ms
```

我们必须认识到，这些是粗略的结果，而不是适当的绩效表现。但即便如此，很明显，map/reduce 版本的速度大约是 `ViaAddText` 版本的 10 倍。

这就是为什么幺半群同态很重要的关键——它们实现了一种强大且易于实现的“分而治之”策略。

是的，你可以说所使用的算法非常低效。字符串 concat 是一种积累大型文本块的糟糕方式，但有更好的方法来进行字数统计。但即使有这些警告，基本观点仍然有效：通过交换两行代码，我们的性能得到了巨大的提高。

通过一点哈希和缓存，我们还可以获得增量聚合的好处——只需在页面更改时重新计算所需的最小值。

请注意，在这种情况下，并行映射并没有太大区别，尽管它确实使用了所有四个核心。是的，我们确实在 `toArray` 上增加了一些小费用，但即使在最好的情况下，您在多核机器上也可能只会得到很小的速度提升。重申一下，真正产生最大影响的是 map/reduce 方法中固有的分而治之策略。

## 非幺半群同态

我之前提到过，并非所有映射都一定是幺半群同态。在本节中，我们将看一个不是的例子。

对于这个例子，我们将返回文本块中最常见的单词，而不是使用计数单词。

这是基本代码。

```F#
module FrequentWordTest =

    open System
    open System.Text.RegularExpressions

    type Text = Text of string

    let addText (Text s1) (Text s2) =
        Text (s1 + s2)

    let mostFrequentWord (Text s) =
        Regex.Matches(s,@"\S+")
        |> Seq.cast<Match>
        |> Seq.map (fun m -> m.ToString())
        |> Seq.groupBy id
        |> Seq.map (fun (k,v) -> k,Seq.length v)
        |> Seq.sortBy (fun (_,v) -> -v)
        |> Seq.head
        |> fst
```

`mostFrequentWord` 函数比前面的 `wordCount` 函数复杂一些，所以我会一步一步地教你。

首先，我们使用正则表达式来匹配所有非空格。这样做的结果是一个 `MatchCollection`，而不是一个 `Match` 列表，因此我们必须显式地将其转换为一个序列（C# 中的 `IEnumerable<Match>`）。

接下来，我们使用 `ToString()` 将每个 `Match` 转换为匹配的单词。然后我们按单词本身分组，这给了我们一个配对列表，其中每对都是一个 `(word,list of words)`。然后，我们将这些对转换为 `(word,list count)`，然后降序排序（使用否定的单词计数）。

最后，我们取第一对，并返回该对的第一部分。这是最常见的词。

好的，让我们继续，像以前一样创建一些页面和文档。这次我们对性能不感兴趣，所以我们只需要几页。但我们确实想创建不同的页面。我们将创建一个只包含“hello world”的，另一个只包括“再见world”的和第三个包含“foobar”的。（依我看，这不是一本很有趣的书！）

```F#
module FrequentWordTest =

    // code as above

    let page1() =
        List.replicate 1000 "hello world "
        |> List.reduce (+)
        |> Text

    let page2() =
        List.replicate 1000 "goodbye world "
        |> List.reduce (+)
        |> Text

    let page3() =
        List.replicate 1000 "foobar "
        |> List.reduce (+)
        |> Text

    let document() =
        [page1(); page2(); page3()]
```

很明显，就整个文件而言，“世界”是最常见的单词。

让我们像以前一样比较这两种方法。第一种方法将合并所有页面，然后应用 `mostFrequentWord`，如下所示。

【通过添加页面获取最常用单词】

第二种方法将在每个页面上分别执行 `mostFrequentWord`，然后组合结果，如下所示：

【mostFrequentWord 通过添加计数】

代码如下：

```F#
module FrequentWordTest =

    // code as above

    document()
    |> List.reduce addText
    |> mostFrequentWord
    |> printfn "Using add first, the most frequent word is %s"

    document()
    |> List.map mostFrequentWord
    |> List.reduce (+)
    |> printfn "Using map reduce, the most frequent word is %s"
```

你能看到发生了什么事吗？第一种方法是正确的。但第二种方法给出了一个完全错误的答案！

```
Using add first, the most frequent word is world
Using map reduce, the most frequent word is hellogoodbyefoobar
```

第二种方法只是将每页中最常见的单词连接起来。结果是一个不在任何页面上的新字符串。彻底失败！

出了什么问题？

好吧，字符串在连接下是一个幺半群，因此映射将一个幺半群（文本）转换为另一个幺半群（字符串）。

但映射并没有保留“形状”。大块文本中最常见的单词不能从小块文本中最频繁的单词中派生出来。换言之，它不是一个适当的幺半群同态。

### 幺半群同态的定义

让我们再次看看这两个不同的例子，以了解它们之间的区别。

在字数统计示例中，无论是先添加块然后进行字数统计，还是先进行字数统计然后将它们加在一起，我们都得到了相同的最终结果。这是一个图表：

【双向字数统计】

但对于最常见的单词示例，我们没有从两种不同的方法中得到相同的答案。

【双向最常见的单词】

换句话说，对于 `wordCount`，我们有

```
wordCount(page1) + wordCount(page2) EQUALS wordCount(page1 + page)
```

但对于 `mostFrequentWord`，我们有：

```
mostFrequentWord(page1) + mostFrequentWord(page2) NOT EQUAL TO mostFrequentWord(page1 + page)
```

因此，这给我们带来了一个稍微更精确的幺半群同态定义：

```
Given a function that maps from one monoid to another (like 'wordCount' or 'mostFrequentWord')

Then to be a monoid homomorphism, the function must meet the requirement that:

function(chunk1) + function(chunk2) MUST EQUAL function(chunk1 + chunk2)
```

唉，那么，`mostFrequentWord` 并不是幺半群同态。

这意味着，如果我们想在大量文本文件上计算最常用的单词，我们很遗憾地被迫先将所有文本加在一起，我们无法从分而治之的策略中受益。

…或者我们可以吗？有没有一种方法可以将 `mostFrequentWord` 转换为适当的单倍体同态？敬请期待！

## 下一步

到目前为止，我们只处理了适当的幺半群。但是，如果你想与之合作的对象不是幺半群呢？然后呢？

在本系列的下一篇文章中，我将为您提供一些将几乎任何东西转换为幺半群的技巧。

我们还将修复 `mostFrequentWord` 示例，使其成为一个适当的幺半群同态，我们将用一种优雅的方法重新审视零的棘手问题。

到时候见！

## 进一步阅读

如果你对使用幺半群进行数据聚合感兴趣，以下链接中有很多很好的讨论：

- Twitter 的 Algebird 库
- 大多数概率数据结构都是幺半群。
- 高斯分布形成一个幺半群。
- 谷歌的 MapReduce 编程模型（PDF）。
- 幺半群化！Monoid 作为高效 MapReduce 算法的设计原则（PDF）。
- LinkedIn 的 Hadoop 沙漏库
- 来自Stack Exchange：群、幺半群和环在数据库计算中有什么用途？

如果你想了解更多技术，这里有一个关于幺半群和半群的详细研究，使用图形图作为领域：

- 幺半群：主题和变体（PDF）。

# 3 与非幺半群合作

*Part of the "Understanding monoids" series (*[link](https://fsharpforfunandprofit.com/posts/monoids-part3/#series-toc)*)*

没有眼泪的幺半群-第 3 部分
2013年10月25日 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/monoids-part3/

在本系列的前几篇文章中，我们只处理了正确的幺半群。

但是，如果你想与之合作的对象不是幺半群呢？然后呢？好吧，在这篇文章中，我会给你一些关于将几乎任何东西转化为幺半群的提示。

在此过程中，我们将介绍一些重要和常见的函数式设计习惯用法，例如更喜欢列表而不是单例，以及在任何机会都使用选项类型。

## 获得闭包

如果你还记得，对于一个适当的幺半群，我们需要三件事是真的：闭包、结合性和单位元。每个要求都可能带来挑战，因此我们将依次讨论每个要求。

我们将从闭包开始。

在某些情况下，您可能希望将值添加在一起，但组合值的类型与原始值的类型不同。你怎么能处理这个？

一种方法是将原始类型映射到已关闭的新类型。我们在上一篇文章的 `Customer` 和 `CustomerStats` 示例中看到了这种方法。在许多情况下，这是最简单的方法，因为您不必弄乱原始类型的设计。

另一方面，有时你真的不想使用 `map`，而是想从头开始设计你的类型，使其满足闭包要求。

无论是设计新类型还是重新设计现有类型，都可以使用类似的技术来获得闭包。

### 组合封闭类型以创建新的复合类型

显然，我们已经看到，在加法和乘法等一些基本的数学运算下，数值类型是封闭的。我们还看到，一些非数字类型，如字符串和列表，在连接下是封闭的。

考虑到这一点，很明显，这些类型的任何组合也将被封闭。我们只需定义“add”函数，即可对组件类型执行适当的“add”操作。

这里有一个例子：

```F#
type MyType = {count:int; items:int list}

let addMyType t1 t2 =
    {count = t1.count + t2.count;
     items = t1.items @ t2.items}
```

`addMyType` 函数在 `int` 字段上使用整数加法，在 `list` 字段上使用列表连接。因此，`MyType` 是使用函数 `addMyType` 封闭的——事实上，它不仅是封闭的，也是一个幺半群。所以，在这种情况下，我们结束了！

这正是我们在上一篇文章中对 `CustomerStats` 采取的方法。

以下是我的第一条建议：

- **设计提示：要轻松创建幺半群类型，请确保该类型的每个字段也是幺半群。**

思考问题：当你这样做时，新复合类型的“零”是什么？

### 处理非数字类型

上述方法在创建复合类型时有效。但是，对于没有明显数字等价物的非数字类型呢？

这是一个非常简单的案例。假设你有一些要加在一起的字符，如下所示：

```F#
'a' + 'b' -> what?
```

但是，一个 char 加上一个 char 并不是另一个 char。如果有的话，那就是一根绳子。

```F#
'a' + 'b' -> "ab" // Closure fail!
```

但这是非常无益的，因为它不符合关闭要求。

解决这个问题的一种方法是将字符强制转换为字符串，这确实有效：

```F#
"a" + "b" -> "ab"
```

但这是针对字符的特定解决方案——是否有更通用的解决方案适用于其他类型？

好吧，想一想字符串与字符的关系是什么。`string` 可以看作是 `char` 的列表或数组。

换句话说，我们本可以使用字符列表，如下所示：

```F#
['a'] @ ['b'] -> ['a'; 'b'] // Lists FTW!
```

这也符合闭包要求。

更重要的是，这实际上是任何此类问题的通用解决方案，因为任何东西都可以放入列表中，而列表（通过连接）总是幺半群。

以下是我的下一个建议：

- **设计提示：要为非数字类型启用闭包，请用列表替换单个项。**

在某些情况下，您可能需要在设置 monoid 时转换为列表，然后在完成后转换为另一种类型。

例如，在 `Char` 的情况下，您将对字符列表进行所有操作，然后只在末尾转换为字符串。

那么，让我们尝试创建一个“monoidal char”模块。

```F#
module MonoidalChar =
    open System

    /// "monoidal char"
    type MChar = MChar of Char list

    /// convert a char into a "monoidal char"
    let toMChar ch = MChar [ch]

    /// add two monoidal chars
    let addChar (MChar l1) (MChar l2) =
        MChar (l1 @ l2)

    // infix version
    let (++) = addChar

    /// convert to a string
    let toString (MChar cs) =
        new System.String(List.toArray cs)
```

您可以看到，`MChar` 是一个围绕字符列表的包装器，而不是一个字符。

现在让我们测试一下：

```F#
open MonoidalChar

// add two chars and convert to string
let a = 'a' |> toMChar
let b = 'b' |> toMChar
let c = a ++ b
c |> toString |> printfn "a + b = %s"
// result: "a + b = ab"
```

如果我们想变得花哨，我们可以使用 map/reduce 来处理一组字符，如下所示：

```F#
[' '..'z']   // get a lot of chars
|> List.filter System.Char.IsPunctuation
|> List.map toMChar
|> List.reduce addChar
|> toString
|> printfn "punctuation chars are %s"
// result: "punctuation chars are !"#%&'()*,-./:;?@[\]_"
```

### Monoid 表示错误

`MonoidalChar` 示例很简单，也许可以用其他方式实现，但总的来说，这是一种非常有用的技术。

例如，这里有一个用于进行验证的简单模块。有两个选项，`Success` 和 `Failure`，`Failure` 案例也有一个与之相关的错误字符串。

```F#
module Validation =

    type ValidationResult =
        | Success
        | Failure of string

    let validateBadWord badWord (name:string) =
        if name.Contains(badWord) then
            Failure ("string contains a bad word: " + badWord)
        else
            Success

    let validateLength maxLength name =
        if String.length name > maxLength then
            Failure "string is too long"
        else
            Success
```

在实践中，我们可能会对一个字符串执行多次验证，我们希望一次返回所有结果，并以某种方式将它们加在一起。

这要求成为一个幺半群！如果我们可以成对添加两个结果，那么我们可以扩展操作，添加任意数量的结果！

那么问题是，我们如何将两个验证结果结合起来？

```F#
let result1 = Failure "string is null or empty"
let result2 = Failure "string is too long"

result1 + result2 = ????
```

一种简单的方法是连接字符串，但如果我们使用格式字符串或具有本地化的资源 id 等，这将不起作用。

不，更好的方法是将 `Failure` 案例转换为使用字符串列表而不是单个字符串。这将使组合结果变得简单。

这是与上面相同的代码，其中 `Failure` 案例被重新定义为使用列表：

```F#
module MonoidalValidation =

    type ValidationResult =
        | Success
        | Failure of string list

    // helper to convert a single string into the failure case
    let fail str =
        Failure [str]

    let validateBadWord badWord (name:string) =
        if name.Contains(badWord) then
            fail ("string contains a bad word: " + badWord)
        else
            Success

    let validateLength maxLength name =
        if String.length name > maxLength then
            fail "string is too long"
        else
            Success
```

您可以看到，单个验证调用会因单个字符串而 `fail`，但在幕后，它被存储为字符串列表，这些字符串可以连接在一起。

有了这个，我们现在可以创建 `add` 函数了。

逻辑是：

- 如果两个结果都是 `Success`，那么组合结果就是 `Success`
- 如果一个结果是 `Failure`，那么组合结果就是失败。
- 如果两个结果都是 `Failure`，那么组合结果就是 `Failure`，两个错误列表连接在一起。

代码如下：

```F#
module MonoidalValidation =

    // as above

    /// add two results
    let add r1 r2 =
        match r1,r2 with
        | Success,    Success -> Success
        | Failure f1, Success -> Failure f1
        | Success,    Failure f2 -> Failure f2
        | Failure f1, Failure f2 -> Failure (f1 @ f2)
```

以下是一些测试来检查逻辑：

```F#
open MonoidalValidation

let test1 =
    let result1 = Success
    let result2 = Success
    add result1 result2
    |> printfn "Result is %A"
    // "Result is Success"

let test2 =
    let result1 = Success
    let result2 = fail "string is too long"
    add result1 result2
    |> printfn "Result is %A"
    // "Result is Failure ["string is too long"]"

let test3 =
    let result1 = fail "string is null or empty"
    let result2 = fail "string is too long"
    add result1 result2
    |> printfn "Result is %A"

    // Result is Failure
    //   [ "string is null or empty";
    //     "string is too long"]
```

这里有一个更现实的例子，我们有一个要应用的验证函数列表：

```F#
let test4 =
    let validationResults str =
        [
        validateLength 10
        validateBadWord "monad"
        validateBadWord "cobol"
        ]
        |> List.map (fun validate -> validate str)

    "cobol has native support for monads"
    |> validationResults
    |> List.reduce add
    |> printfn "Result is %A"
```

输出为 `Failure`，并显示三条错误消息。

```
Result is Failure
  ["string is too long"; "string contains a bad word: monad";
   "string contains a bad word: cobol"]
```

还需要做一件事来结束这个怪物。我们也需要一个“零”。应该是什么？

根据定义，当与另一个结果结合时，它会让另一个不受影响。

我希望你能明白，根据这个定义，“零”就是 `Success`。

```F#
module MonoidalValidation =

    // as above

    // identity
    let zero = Success
```

如您所知，如果要减少的列表为空，我们需要使用零。所以这里有一个例子，我们根本不应用任何验证函数，给了我们一个空的 `ValidationResult` 列表。

```F#
let test5 =
    let validationResults str =
        []
        |> List.map (fun validate -> validate str)

    "cobol has native support for monads"
    |> validationResults
    |> List.fold add zero
    |> printfn "Result is %A"

    // Result is Success
```

请注意，我们也需要将 `reduce` 更改为 `fold`，否则我们将收到运行时错误。

### 性能列表

这里还有一个使用列表的好处的例子。与其他组合方法相比，列表连接在计算和内存使用方面都相对便宜，因为指向的对象不必更改或重新分配。

例如，在上一篇文章中，我们定义了一个包裹字符串的 `Text` 块，并使用字符串连接来添加其内容。

```F#
type Text = Text of string

let addText (Text s1) (Text s2) =
    Text (s1 + s2)
```

但对于大字符串，这种连续的连接可能很昂贵。

考虑一个不同的实现，其中 `Text` 块包含一个字符串列表。

```F#
type Text = Text of string list

let addText (Text s1) (Text s2) =
    Text (s1 @ s2)
```

实现几乎没有变化，但性能可能会大大提高。

您可以对字符串列表进行所有操作，只需在处理序列的最后转换为普通字符串。

如果列表的性能不够好，你可以很容易地扩展这种方法，使用经典的数据结构，如树、堆等，或可变类型，如 ResizeArray。（关于这一点的更多讨论，请参阅本文底部的绩效附录）

### 行话（Jargon）警告

在数学中，将一系列对象用作幺半群的概念很常见，它被称为“自由幺半群（free monoid）”。在计算机科学中，它也被称为“Kleene 星”，如 `A*`。如果你不允许空列表，那么你就没有零元素。这种变体被称为“自由半群”或“Kleene plus”，如 `A+`。

如果你曾经使用过正则表达式，你肯定会熟悉这个“星号”和“加号”符号*

*你可能不知道正则表达式和幺半群之间有联系！还有一些更深层次的关系。

## 结合性

现在我们已经处理了闭包，让我们来谈谈结合性。

我们在第一篇文章中看到了一些非结合操作，包括减法和除法。

我们可以看到 `5 - (3 - 2)` 不等于 `(5 - 3) - 2`。这表明减法不是结合性的， `12 / (3 / 2)` 也不等于 `(12 / 3) / 2`，这表明除法不是结合性的。

在这些情况下，没有单一的正确答案，因为根据你是从左向右还是从右向左工作，你可能会真正关心不同的答案。

事实上，F# 标准库有两个版本的 `fold` 和 `reduce`，以满足每种偏好。正常的 `fold` 和 `reduce` 工作从左到右，如下所示：

```F#
//same as (12 - 3) - 2
[12;3;2] |> List.reduce (-)  // => 7

//same as ((12 - 3) - 2) - 1
[12;3;2;1] |> List.reduce (-)  // => 6
```

但也有 `foldBack` 和 `reduceBack` 从右向左工作，如下所示：

```F#
//same as 12 - (3 - 2)
[12;3;2] |> List.reduceBack (-) // => 11

//same as 12 - (3 - (2 - 1))
[12;3;2;1] |> List.reduceBack (-) // => 10
```

从某种意义上说，结合性要求只是一种说法，即无论你使用 `fold` 还是 `foldBack`，你都应该得到相同的答案。

### 将操作符移动到元素中

但是，假设你确实想要一个一致的幺半群方法，在许多情况下，诀窍是将操作符移动到每个元素的属性（property）中。**将操作符设置为名词，而不是动词**。

例如，`3 - 2` 可以被认为是 `3 + (-2)`。我们用“负 2”作为名词，而不是用“减法”作为动词。

在这种情况下，上述示例变为 `5 + (-3) + (-2)`。由于我们现在使用加法作为运算符，因此我们确实具有结合性，`5 + (-3 + -2)` 确实与 `(5 + -3) + -2` 相同。

类似的方法适用于除法。`12 / 3 / 2`可以转换为 `12 * (1/3) * (1/2)`，现在我们回到乘法作为运算符，这是结合性的。

这种将运算符转换为元素属性的方法可以很好地推广。

所以这里有一个提示：

- **设计提示：要获得操作的关联性，请尝试将操作移动到对象中。**

我们可以重新审视一个早期的例子来了解它是如何工作的。如果你还记得，在第一篇文章中，我们试图为字符串提出一个非结合操作，并最终确定了 `subtractChars`。

下面是一个 `subactCharacters` 的简单实现

```F#
let subtractChars (s1:string) (s2:string) =
    let isIncluded (ch:char) = s2.IndexOf(ch) = -1
    let chars = s1.ToCharArray() |> Array.filter isIncluded
    System.String(chars)

// infix version
let (--) = subtractChars
```

通过此实现，我们可以进行一些交互式测试：

```F#
"abcdef" -- "abd"   //  "cef"
"abcdef" -- ""      //  "abcdef"
```

我们可以亲眼看到，结合性要求被违反了：

```F#
("abc" -- "abc") -- "abc"  // ""
"abc" -- ("abc" -- "abc")  // "abc"
```

我们如何使这种结合性？

诀窍是将运算符中的“减法”移动到对象中，就像我们之前处理数字一样。

我的意思是，我们用“减法”或“字符删除”数据结构替换普通字符串，以捕获我们想要删除的内容，如下所示：

```F#
let removalAction = (subtract "abd") // a data structure
```

然后我们将数据结构“应用”到字符串上：

```F#
let removalAction = (subtract "abd")
removalAction |> applyTo "abcdef"  // "Result is cef"
```

一旦我们使用了这种方法，我们就可以修改上面的非结合示例，使其看起来像这样：

```F#
let removalAction = (subtract "abc") + (subtract "abc") + (subtract "abc")
removalAction |> applyTo "abc"    // "Result is "
```

是的，它与原始代码并不完全相同，但您可能会发现，在许多情况下，这实际上更适合。

执行情况如下。我们定义了一个 `CharsToRemove` 来包含一组字符，其他函数实现以一种简单的方式从中派生出来。

```F#
/// store a list of chars to remove
type CharsToRemove = CharsToRemove of Set<char>

/// construct a new CharsToRemove
let subtract (s:string) =
    s.ToCharArray() |> Set.ofArray |>  CharsToRemove

/// apply a CharsToRemove to a string
let applyTo (s:string) (CharsToRemove chs) =
    let isIncluded ch = Set.exists ((=) ch) chs |> not
    let chars = s.ToCharArray() |> Array.filter isIncluded
    System.String(chars)

// combine two CharsToRemove to get a new one
let (++) (CharsToRemove c1) (CharsToRemove c2) =
    CharsToRemove (Set.union c1 c2)
```

让我们测试一下！

```F#
let test1 =
    let removalAction = (subtract "abd")
    removalAction |> applyTo "abcdef" |> printfn "Result is %s"
    // "Result is cef"

let test2 =
    let removalAction = (subtract "abc") ++ (subtract "abc") ++ (subtract "abc")
    removalAction |> applyTo "abcdef" |> printfn "Result is %s"
    // "Result is "
```

思考这种方法的方式是，从某种意义上说，我们正在对行动而不是数据进行建模。我们有一个 `CharsToRemove` 操作列表，然后将它们组合成一个“大” `CharsToRemide` 操作，然后在完成中间操作后，在最后执行该操作。

我们很快就会看到另一个例子，但此时你可能会想：“这听起来有点像函数，不是吗？”对此我会说“是的，它确实如此！”

事实上，我们本可以部分应用原始的 `subactChars` 函数，而不是创建这个 `CharToRemove` 数据结构，如下所示：

（请注意，我们反转参数以使部分应用更容易）

```F#
// reverse for partial application
let subtract str charsToSubtract =
    subtractChars charsToSubtract str

let removalAction = subtract "abd"
"abcdef" |> removalAction |> printfn "Result is %s"
// "Result is cef"
```

现在我们甚至不需要一个特殊的 `applyTo` 函数。

但是，当我们有多个这样的减法函数时，我们该怎么办？这些部分应用的函数中的每一个都有签名 `string -> string`，那么我们如何将它们“添加”在一起呢？

```F#
(subtract "abc") + (subtract "abc") + (subtract "abc")  = ?
```

当然，答案是函数式组合！

```F#
let removalAction2 = (subtract "abc") >> (subtract "abc") >> (subtract "abc")
removalAction2 "abcdef" |> printfn "Result is %s"
// "Result is def"
```

这在函数式上相当于创建 `CharsToRemove` 数据结构。

“数据结构即动作”和函数方法并不完全相同——例如，`CharsToRemove` 方法可能更有效，因为它使用一个集合，并且只应用于末尾的字符串——但它们都实现了相同的目标。哪一个更好取决于你正在处理的特定问题。

在下一篇文章中，我将更多地谈论函数和幺半群。

## 单位元

现在，我们来看一个幺半群的最后一个要求：单位元。

正如我们所见，单位元并不总是需要的，但如果你可能正在处理空列表，那么拥有单位元是件好事。

对于数值，找到操作的单位元通常很容易，无论是 `0`（加法）、`1`（乘法）还是 `Int32.MinValue`（max）。

这也适用于仅包含数值的结构——只需将所有值设置为适当的标识即可。上一篇文章中的 `CustomerStats` 类型很好地证明了这一点。

但是，如果你有不是数字的对象呢？如果没有自然候选元素，你如何创建“零”或身份元素？

答案是：*你只需编造一个*。

说真的！

我们在上一篇文章中已经看到了一个例子，当我们向 `OrderLine` 类型添加一个 `EmptyOrder` 案例时：

```F#
type OrderLine =
    | Product of ProductLine
    | Total of TotalLine
    | EmptyOrder
```

让我们更仔细地看看这个。我们执行了两个步骤：

- 首先，我们创建了一个新案例，并将其添加到 `OrderLine` 的备选方案列表中（如上所示）。
- 其次，我们调整了 `addLine` 函数以将其考虑在内（如下所示）。

```F#
let addLine orderLine1 orderLine2 =
    match orderLine1,orderLine2 with
    // is one of them zero? If so, return the other one
    | EmptyOrder, _ -> orderLine2
    | _, EmptyOrder -> orderLine1
    // logic for other cases ...
```

这就是全部。

新的增强类型由旧的订单行案例和新的 `EmptyOrder` 案例组成，因此它可以重用旧案例的大部分行为。

特别是，你能看到新的增广类型遵循所有的幺半群规则吗？

- 可以添加一对新类型的值，以获得新类型的另一个值（闭包）
- 如果组合顺序对旧类型不重要，那么对新类型（结合性）也不重要
- 最后，这个额外的案例现在为我们提供了新类型的身份。

### 将正数转化为幺半群

我们可以对我们看到的其他半群做同样的事情。

例如，我们之前注意到，严格正数（在加法下）没有恒等式；它们只是一个半群。如果我们想使用“通过额外例子增广”技术（而不仅仅是使用 `0`！）创建一个零，我们将首先定义一个特殊的 `Zero` 例子（不是整数），然后创建一个可以处理它的 `addPositve` 函数，如下所示：

```F#
type PositiveNumberOrIdentity =
    | Positive of int
    | Zero

let addPositive i1 i2 =
    match i1,i2 with
    | Zero, _ -> i2
    | _, Zero -> i1
    | Positive p1, Positive p2 -> Positive (p1 + p2)
```

诚然，`PositiveNumberOrIdentity` 是一个人为的例子，但你可以看到，在任何情况下，当你有“正常”值和一个特殊的、单独的零值时，这种方法都是有效的。

### 泛型解决方案

这有几个缺点：

- 我们现在必须处理两种情况：正常情况和零情况。
- 我们必须创建自定义类型和自定义添加函数

不幸的是，对于第一个问题，你无能为力。如果你有一个没有自然零的系统，并且你创建了一个人工零，那么你确实总是要处理两种情况。

但对于第二个问题，你可以做点什么！与其一遍又一遍地创建新的自定义类型，也许我们可以创建一个有两种情况的泛型类型：一种用于所有正常值，另一种用于人为零，如下所示：

```F#
type NormalOrIdentity<'T> =
    | Normal of 'T
    | Zero
```

这种类型看起来熟悉吗？这只是伪装的**选项类型**！

换句话说，任何时候我们需要一个超出正常值集的身份，我们都可以使用 `Option.None` 代表它。然后是 `Option.Some` 用于所有其他“正常”值。

使用 `Option` 的另一个好处是，我们还可以编写一个完全通用的“add”函数。这是第一次尝试：

```F#
let optionAdd o1 o2 =
    match o1, o2 with
    | None, _ -> o2
    | _, None -> o1
    | Some s1, Some s2 -> Some (s1 + s2)
```

逻辑很简单。如果任一选项为 `None`，则返回另一个选项。如果两者都是 `Some`，那么它们会被拆开，加在一起，然后再次被包裹在 `Some` 中。

但是最后一行中的 `+` 对我们添加的类型做出了假设。最好显式传递加法函数，如下所示：

```F#
let optionAdd f o1 o2 =
    match o1, o2 with
    | None, _ -> o2
    | _, None -> o1
    | Some s1, Some s2 -> Some (f s1 s2)
```

在实践中，这将与部分应用一起用于添加函数中的烘焙。

现在我们有另一个重要提示：

- **设计提示：要获取操作的标识，请在可区分联合中创建一个特例，或者更简单地说，只需使用 Option。**

### 重新查看正数

这里再次是正数示例，现在使用 `Option` 类型。

```F#
type PositiveNumberOrIdentity = int option
let addPositive = optionAdd (+)
```

简单多了！

请注意，我们将“实”加法函数作为参数传递给 `optionAdd`，以便将其嵌入。在其他情况下，您可以对与半群相关的相关聚合函数执行相同的操作。

作为这个部分应用的结果，`addPositive` 具有签名：`int option -> int option -> int option`，这正是我们对幺半群加法函数的期望。

换句话说，`optionAdd` 将任何函数 `'a -> 'a -> 'a` 转换为相同的函数，但“提升”为选项类型，即具有签名 `'a option -> 'a option -> 'a option`。

那么，让我们来测试一下！一些测试代码可能看起来像这样：

```F#
// create some values
let p1 = Some 1
let p2 = Some 2
let zero = None

// test addition
addPositive p1 p2
addPositive p1 zero
addPositive zero p2
addPositive zero zero
```

您可以看到，不幸的是，我们必须将正常值包装在 `Some` 中，才能获得 `None` 作为标识。

这听起来很乏味，但在实践中，这很容易。下面的代码显示了在求和列表时如何处理这两种不同的情况。首先是如何对非空列表求和，然后是如何对空列表求总和。

```F#
[1..10]
|> List.map Some
|> List.fold addPositive zero

[]
|> List.map Some
|> List.fold addPositive zero
```

### 重新查看 ValidationResult

在讨论这个问题的同时，让我们再次讨论一下我们之前在讨论使用列表获取闭包时描述的 `ValidationResult` 类型。又来了：

```F#
type ValidationResult =
    | Success
    | Failure of string list
```

现在我们已经对正整数的情况有了一些了解，让我们从不同的角度来看待这种类型。

该类型有两个案例。一个案例包含我们关心的数据，另一个案例没有数据。但我们真正关心的是错误信息，而不是成功。正如列夫·托尔斯泰所说：“所有的验证成功都是一样的；每一次验证失败都是一次失败。”

因此，与其将其视为“结果”，不如将类型视为存储故障，并将其重写为这样，首先是故障情况：

```F#
type ValidationFailure =
    | Failure of string list
    | Success
```

这种类型现在看起来熟悉吗？

对！又是选项类型了！我们能永远摆脱这该死的东西吗？

使用选项类型，我们可以将 `ValidationFailure` 类型的设计简化为：

```F#
type ValidationFailure = string list option
```

将字符串转换为失败案例的助手现在只是带有列表的 `Some`：

```F#
let fail str =
    Some [str]
```

“add”函数可以重用 `optionAdd`，但这次使用列表连接作为底层操作：

```F#
let addFailure f1 f2 = optionAdd (@) f1 f2
```

最后，原始设计中 `Success` 案例的“零”在新设计中变成了 `None`。

这是所有的代码，还有测试

```F#
module MonoidalValidationOption =

    type ValidationFailure = string list option

    // helper to convert a string into the failure case
    let fail str =
        Some [str]

    let validateBadWord badWord (name:string) =
        if name.Contains(badWord) then
            fail ("string contains a bad word: " + badWord)
        else
            None

    let validateLength maxLength name =
        if String.length name > maxLength then
            fail "string is too long"
        else
            None

    let optionAdd f o1 o2 =
        match o1, o2 with
        | None, _ -> o2
        | _, None -> o1
        | Some s1, Some s2 -> Some (f s1 s2)

    /// add two results using optionAdd
    let addFailure f1 f2 = optionAdd (@) f1 f2

    // define the Zero
    let Success = None

module MonoidalValidationOptionTest =
    open MonoidalValidationOption

    let test1 =
        let result1 = Success
        let result2 = Success
        addFailure result1 result2
        |> printfn "Result is %A"

        // Result is <null>

    let test2 =
        let result1 = Success
        let result2 = fail "string is too long"
        addFailure result1 result2
        |> printfn "Result is %A"
        // Result is Some ["string is too long"]

    let test3 =
        let result1 = fail "string is null or empty"
        let result2 = fail "string is too long"
        addFailure result1 result2
        |> printfn "Result is %A"
        // Result is Some ["string is null or empty"; "string is too long"]

    let test4 =
        let validationResults str =
            [
            validateLength 10
            validateBadWord "monad"
            validateBadWord "cobol"
            ]
            |> List.map (fun validate -> validate str)

        "cobol has native support for monads"
        |> validationResults
        |> List.reduce addFailure
        |> printfn "Result is %A"
        // Result is Some
        //   ["string is too long"; "string contains a bad word: monad";
        //    "string contains a bad word: cobol"]

    let test5 =
        let validationResults str =
            []
            |> List.map (fun validate -> validate str)

        "cobol has native support for monads"
        |> validationResults
        |> List.fold addFailure Success
        |> printfn "Result is %A"
        // Result is <null>
```

## 设计技巧总结

让我们暂停一下，看看到目前为止我们已经涵盖了什么。

以下是所有的设计技巧：

- 要轻松创建幺半群类型，请确保该类型的每个字段也是幺半群。
- 要为非数字类型启用闭包，请用列表（或类似的数据结构）替换单个项。
- 要获取操作的结合性，请尝试将操作移动到对象中。
- 要获取操作的单位元，请在可区分联合中创建一个特例，或者更简单地说，只需使用 Option。

在接下来的两节中，我们将把这些技巧应用于我们在之前的帖子中看到的两个非幺半群：“平均”和“最常见的单词”。

## 案例研究：平均

所以现在我们有了工具包，可以处理棘手的平均值问题。

这是一个简单的成对（pairwise）平均函数的实现：

```F#
let avg i1 i2 =
    float (i1 + i2) / 2.0

// test
avg 4 5 |> printfn "Average is %g"
// Average is 4.5
```

正如我们在第一篇文章中简要提到的，`avg` 在所有三个 monoid 要求上都失败了！

首先，它没有关闭。使用 `avg` 组合在一起的两个 int 不会产生另一个 int。

其次，即使它是关闭的，`avg` 也不是结合的，正如我们通过定义一个类似的浮点函数 `avgf` 所看到的：

```F#
let avgf i1 i2 =
    (i1 + i2) / 2.0

// test
avgf (avgf 1.0 3.0) 5.0  |> printfn "Average from left is %g"
avgf 1.0 (avgf 3.0 5.0)  |> printfn "Average from right is %g"

// Average from left is 3.5
// Average from right is 2.5
```

最后，没有单位元。

当与任何其他数字求平均值时，哪个数字返回原始值？答案：没有！

### 应用设计技巧

因此，让我们应用设计技巧，看看它们是否能帮助我们提出解决方案。

- *要轻松创建幺半群类型，请确保该类型的每个字段也是幺半群。*

好吧，“平均值”是一个数学运算，所以我们可以预期，一个幺半群等价物也将基于数字。

- *要为非数字类型启用闭包，请用列表替换单个项目。*

乍一看，这似乎无关紧要，所以我们现在跳过这个。

- *要获取操作的结合性，请尝试将操作移动到对象中。*

关键就在这里！我们如何将“平均值”从动词（操作）转换为名词（数据结构）？

答案是，我们创建了一个结构，它实际上不是一个平均值，而是一个“延迟平均值”——按需进行平均所需的一切。

也就是说，我们需要一个由两个部分组成的数据结构：总计和计数。根据这两个数字，我们可以根据需要计算平均值。

```F#
// store all the info needed for an average
type Avg = {total:int; count:int}

// add two Avgs together
let addAvg avg1 avg2 =
    {total = avg1.total + avg2.total;
     count = avg1.count + avg2.count}
```

这样做的好处是，结构存储的是 `ints`，而不是 `floats`，所以我们不需要担心浮点数的精度或结合性的损失。

最后一个提示是：

- *要获取操作的标识，请在可区分联合中创建一个特例，或者更简单地说，只需使用 Option。*

在这种情况下，不需要提示，因为我们可以通过将两个分量设置为零来轻松创建零：

```F#
let zero = {total=0; count=0}
```

我们也可以用 `None` 来表示零，但在这种情况下，这似乎有些矫枉过正。如果列表为空，则 `Avg` 结果有效，即使我们无法进行除法。

一旦我们了解了数据结构，接下来的实现就很容易了。以下是所有代码，以及一些测试：

```F#
module Average =

    // store all the info needed for an average
    type Avg = {total:int; count:int}

    // add two Avgs together
    let addAvg avg1 avg2 =
        {total = avg1.total + avg2.total;
         count = avg1.count + avg2.count}

    // inline version of add
    let (++) = addAvg

    // construct an average from a single number
    let avg n = {total=n; count=1}

    // calculate the average from the data.
    // return 0 for empty lists
    let calcAvg avg =
        if avg.count = 0
        then 0.0
        else float avg.total / float avg.count

    // alternative - return None for empty lists
    let calcAvg2 avg =
        if avg.count = 0
        then None
        else Some (float avg.total / float avg.count)

    // the identity
    let zero = {total=0; count=0}

    // test
    addAvg (avg 4) (avg 5)
    |> calcAvg
    |> printfn "Average is %g"
    // Average is 4.5

    (avg 4) ++ (avg 5) ++ (avg 6)
    |> calcAvg
    |> printfn "Average is %g"
    // Average is 5

    // test
    [1..10]
    |> List.map avg
    |> List.reduce addAvg
    |> calcAvg
    |> printfn "Average is %g"
    // Average is 5.5
```

在上面的代码中，您可以看到我创建了一个 `calcAvg` 函数，该函数使用 `Avg` 结构来计算（浮点）平均值。这种方法的一个好处是，我们可以推迟决定如何处理零除数。我们可以直接返回 `0`，或者选择 `None`，或者我们可以无限期地推迟计算，只在最后一刻根据需要生成平均值！

当然，这种“平均”的实现有能力进行增量平均。我们免费得到这个，因为它是一个幺半群。

也就是说，如果我已经计算了一百万个数字的平均值，我想再加一个，我不必重新计算所有的数字，我可以把新的数字加到到目前为止的总数上。

## 对指标的轻微偏离

如果你曾经负责管理任何服务器或服务，你会意识到记录和监控指标的重要性，如 CPU、I/O 等。

你经常面临的问题之一是如何设计你的指标。您想要每秒千字节，还是只需要服务器启动后的总千字节数。每小时的访客数，还是总访客数？

如果你在创建指标时查看一些指导方针，你会看到经常建议只跟踪计数器指标，而不是比率（rates）。

计数器的优点是（a）丢失的数据不会影响大局，并且（b）它们可以在事后以多种方式聚合——按分钟、按小时、与其他数据的比率等等。

现在你已经完成了这个系列，你可以看到这个建议真的可以重新表述为**指标应该是幺半群**。

我们在上面的代码中将“平均值”转换为两个组成部分“总计”和“计数”的工作，正是你想要做的，才能得到一个好的指标。

平均值和其他比率不是幺半群，但“总数”和“计数”是，然后可以在你空闲时根据它们计算出“平均值”。

## 案例研究：将“最频繁单词”转化为幺半群同态

在上一篇文章中，我们实现了一个“最频繁单词”函数，但发现它不是幺半群同态。也就是说，

```
mostFrequentWord(text1) + mostFrequentWord(text2)
```

没有给出与以下结果相同的结果：

```F#
mostFrequentWord( text1 + text2 )
```

同样，我们可以使用设计技巧来解决这个问题，使其正常工作。

这里的见解是再次将计算推迟到最后一分钟，就像我们在“平均”示例中所做的那样。

我们创建了一个数据结构，存储了以后计算最频繁单词所需的所有信息，而不是预先计算最频繁的单词。

```F#
module FrequentWordMonoid =

    open System
    open System.Text.RegularExpressions

    type Text = Text of string

    let addText (Text s1) (Text s2) =
        Text (s1 + s2)

    // return a word frequency map
    let wordFreq (Text s) =
        Regex.Matches(s,@"\S+")
        |> Seq.cast<Match>
        |> Seq.map (fun m -> m.ToString())
        |> Seq.groupBy id
        |> Seq.map (fun (k,v) -> k,Seq.length v)
        |> Map.ofSeq
```

在上面的代码中，我们有一个新函数 `wordFreq`，它返回一个 `Map<string, int>`，而不仅仅是一个单词。也就是说，我们现在正在使用字典，其中每个槽都有一个单词及其相关频率。

以下是其工作原理的演示：

```F#
module FrequentWordMonoid =

    // code from above

    let page1() =
        List.replicate 1000 "hello world "
        |> List.reduce (+)
        |> Text

    let page2() =
        List.replicate 1000 "goodbye world "
        |> List.reduce (+)
        |> Text

    let page3() =
        List.replicate 1000 "foobar "
        |> List.reduce (+)
        |> Text

    let document() =
        [page1(); page2(); page3()]

    // show some word frequency maps
    page1() |> wordFreq |> printfn "The frequency map for page1 is %A"
    page2() |> wordFreq |> printfn "The frequency map for page2 is %A"

    //The frequency map for page1 is map [("hello", 1000); ("world", 1000)]
    //The frequency map for page2 is map [("goodbye", 1000); ("world", 1000)]

    document()
    |> List.reduce addText
    |> wordFreq
    |> printfn "The frequency map for the document is %A"

    //The frequency map for the document is map [
    //      ("foobar", 1000); ("goodbye", 1000);
    //      ("hello", 1000); ("world", 2000)]
```

有了这个映射结构，我们可以创建一个函数 `addMap` 来添加两个映射。它只是合并了两张映射中单词的频率计数。

```F#
module FrequentWordMonoid =

    // code from above

    // define addition for the maps
    let addMap map1 map2 =
        let increment mapSoFar word count =
            match mapSoFar |> Map.tryFind word with
            | Some count' -> mapSoFar |> Map.add word (count + count')
            | None -> mapSoFar |> Map.add word count

        map2 |> Map.fold increment map1
```

当我们将所有映射组合在一起时，我们可以通过循环映射并找到频率最高的单词来计算最频繁的单词。

```F#
module FrequentWordMonoid =

    // code from above

    // as the last step,
    // get the most frequent word in a map
    let mostFrequentWord map =
        let max (candidateWord,maxCountSoFar) word count =
            if count > maxCountSoFar
            then (word,count)
            else (candidateWord,maxCountSoFar)

        map |> Map.fold max ("None",0)
```

因此，以下是使用新方法重新审视的两种情况。

第一种情况是将所有页面组合成一个文本，然后应用 `wordFreq` 来获得频率图，并应用 `mostFrequentWord` 来获得最频繁的单词。

第二种情况是将 `wordFreq` 分别应用于每个页面，以获得每个页面的映射。然后将这些地图与 `addMap` 组合，得到一个全局映射。然后，如前所述，`mostFrequentWord` 将作为最后一步应用。

```F#
module FrequentWordMonoid =

    // code from above

    document()
    |> List.reduce addText
    |> wordFreq
    // get the most frequent word from the big map
    |> mostFrequentWord
    |> printfn "Using add first, the most frequent word and count is %A"

    //Using add first, the most frequent word and count is ("world", 2000)

    document()
    |> List.map wordFreq
    |> List.reduce addMap
    // get the most frequent word from the merged smaller maps
    |> mostFrequentWord
    |> printfn "Using map reduce, the most frequent and count is %A"

    //Using map reduce, the most frequent and count is ("world", 2000)
```

如果你运行这段代码，你会看到你现在得到了相同的答案。

这意味着 `wordFreq` 确实是一个幺半群同态，适合并行或递增运行。

## 下次

我们在这篇文章中看到了很多代码，但都集中在数据结构上。

然而，在单体的定义中没有任何东西说要组合的东西必须是数据结构——它们可以是任何东西。

在下一篇文章中，我们将研究应用于其他对象的幺半群，如类型、函数等。

## 附录：关于性能

在上面的例子中，我经常使用 `@` 来“添加”两个列表，就像 `+` 添加两个数字一样。我这样做是为了强调与其他单倍体运算（如数字加法和字符串连接）的类比。

我希望很明显，上面的代码示例是教学示例，不一定是生产环境中所需的真实世界、久经沙场、太丑陋的代码的好模型。

一些人指出，通常应该避免使用 List append（`@`）。这是因为需要复制整个第一个列表，这不是很有效。

到目前为止，向列表中添加内容的最佳方式是使用所谓的“cons”机制将其添加到前面，在 F# 中，该机制就是 `::`。F# 列表被实现为链表，因此添加到前面非常便宜。

使用这种方法的问题在于它不是对称的——它没有将两个列表加在一起，只是一个列表和一个元素。这意味着它不能用作幺半群中的“add”操作。

如果你不需要幺半群的好处，比如分而治之，那么这是一个完全有效的设计决策。没有必要为了一个你不会从中受益的模式而牺牲性能。

使用 `@` 的另一种选择是从一开始就不使用列表！

### 列表的替代方案

在 `ValidationResult` 设计中，我使用了一个列表来保存错误结果，这样我们就可以轻松地累积结果。但我只选择了列表类型，因为它实际上是 F# 中的默认集合类型。我同样可以选择序列、数组或集合。几乎任何其他类型的集合都能很好地完成这项工作。

但并非所有类型都有相同的性能。例如，组合两个序列是一种懒惰的操作。你不必复制所有的数据；你只需枚举一个序列，然后是另一个。这样也许会更快？

我没有猜测，而是写了一个小测试脚本来衡量各种列表大小、各种集合类型的性能。

我选择了一个非常简单的模型：我们有一个对象列表，每个对象都是一个包含一个项目的集合。然后，我们使用适当的 monoid 操作将这个集合列表缩减为一个巨大的集合。最后，我们迭代一次这个庞大的集合。

这与 `ValidationResult` 设计非常相似，我们将所有结果组合成一个结果列表，然后（大概）迭代它们以显示错误。

它也类似于上面的“最频繁单词”设计，我们将所有单独的频率图组合成一个频率图，然后迭代它以找到最频繁的单词。当然，在这种情况下，我们使用的是 `map` 而不是 `list`，但步骤集是相同的。

### 性能实验

好的，这是代码：

```F#
module Performance =

    let printHeader() =
        printfn "Label,ListSize,ReduceAndIterMs"

    // time the reduce and iter steps for a given list size and print the results
    let time label reduce iter listSize =
        System.GC.Collect() //clean up before starting
        let stopwatch = System.Diagnostics.Stopwatch()
        stopwatch.Start()
        reduce() |> iter
        stopwatch.Stop()
        printfn "%s,%iK,%i" label (listSize/1000) stopwatch.ElapsedMilliseconds

    let testListPerformance listSize =
        let lists = List.init listSize (fun i -> [i.ToString()])
        let reduce() = lists |> List.reduce (@)
        let iter = List.iter ignore
        time "List.@" reduce iter listSize

    let testSeqPerformance_Append listSize =
        let seqs = List.init listSize (fun i -> seq {yield i.ToString()})
        let reduce() = seqs |> List.reduce Seq.append
        let iter = Seq.iter ignore
        time "Seq.append" reduce iter listSize

    let testSeqPerformance_Yield listSize =
        let seqs = List.init listSize (fun i -> seq {yield i.ToString()})
        let reduce() = seqs |> List.reduce (fun x y -> seq {yield! x; yield! y})
        let iter = Seq.iter ignore
        time "seq(yield!)" reduce iter listSize

    let testArrayPerformance listSize =
        let arrays = List.init listSize (fun i -> [| i.ToString() |])
        let reduce() = arrays |> List.reduce Array.append
        let iter = Array.iter ignore
        time "Array.append" reduce iter listSize

    let testResizeArrayPerformance listSize  =
        let resizeArrays = List.init listSize (fun i -> new ResizeArray<string>( [i.ToString()] ) )
        let append (x:ResizeArray<_>) y = x.AddRange(y); x
        let reduce() = resizeArrays |> List.reduce append
        let iter = Seq.iter ignore
        time "ResizeArray.append" reduce iter listSize
```

让我们快速浏览一下代码：

- `time` 函数对归约和迭代步骤进行计时。它故意不测试创建集合需要多长时间。在开始之前，我确实会执行  GC，但实际上，特定类型或算法造成的内存压力是决定是否使用它的重要部分。理解 GC 的工作原理是获得高性能代码的重要组成部分。
- `testListPerformance` 函数设置集合列表（在本例中为列表）以及 `reduce` 和 `iter` 函数。然后，它在 `reduce` 和 `iter` 上运行计时器。
- 其他函数也做同样的事情，但使用序列、数组和 ResizeArrays（标准 .NET 列表）。出于好奇，我想测试两种合并序列的方法，一种使用标准库函数 `Seq.append`，另一种使用两个 `yield!`s排成一排。
- `testResizeArrayPerformance` 使用 ResizeArrays 并将右侧列表添加到左侧列表中。左边的那个会根据需要变异并变大，使用一种保持插入高效的生长策略。

现在，让我们编写代码来检查各种大小的列表的性能。我选择从 2000 开始计数，然后以 4000 递增到 50000。

```F#
open Performance

printHeader()

[2000..4000..50000]
|> List.iter testArrayPerformance

[2000..4000..50000]
|> List.iter testResizeArrayPerformance

[2000..4000..50000]
|> List.iter testListPerformance

[2000..4000..50000]
|> List.iter testSeqPerformance_Append

[2000..4000..50000]
|> List.iter testSeqPerformance_Yield
```

我不会列出所有详细的输出——你可以自己运行代码——但这里有一个结果图表。

【幺半群性能】

有几件事需要注意：

- 这两个基于序列的示例因堆栈溢出而崩溃。`yield!` 比 `Seq.append` 快约 30%，但也更快地用完堆栈。
- List.append 没有用完堆栈，但随着列表变大，运行速度变慢了。
- Array.append 很快，并且随着列表的大小而增加得更慢
- ResizeArray 是所有列表中最快的，即使使用大列表也不会费力。

对于三种没有崩溃的集合类型，我还为它们计时了 10 万个项目的列表。结果如下：

- 列表 = 150730 毫秒
- 数组 = 26062 毫秒
- ResizeArray = 33 毫秒

那么，这是一个明显的赢家。

### 分析结果

我们可以从这个小实验中得出什么结论？

首先，您可能会有各种各样的问题，例如：您是在调试模式还是发布模式下运行的？您是否启用了优化？使用并行性来提高性能怎么样？毫无疑问，会有评论说“你为什么使用技术 X，技术 Y 要好得多”。

但以下是我想得出的结论：

- **你不能从这些结果中得出任何结论！**

每种情况都不同，需要不同的方法：

- 如果你使用的是小数据集，你可能根本不关心性能。在这种情况下，我会坚持使用列表——除非迫不得已，否则我宁愿不牺牲模式匹配和不变性。
- 性能瓶颈可能不在列表添加代码中。如果你实际上把所有的时间都花在磁盘 I/O 或网络延迟上，那么优化列表添加就没有意义了。单词频率示例的真实版本实际上可能会花费大部分时间从磁盘读取或解析，而不是添加列表。
- 如果你在谷歌、推特或脸书这样的规模工作，你真的需要去雇佣一些算法专家。

我们可以从任何关于优化和性能的讨论中得出的唯一原则是：

- **一个问题必须根据其自身情况来处理**。正在处理的数据大小、硬件类型、内存量等。所有这些都会对您的性能产生影响。对我有效的东西可能对你无效，这就是为什么…
- **你应该经常测量，而不是猜测**。不要对你的代码在哪里花费时间做出假设——学会使用分析器！这里和这里有一些使用分析器的好例子。
- **警惕微观优化**。即使你的分析器显示你的排序例程将所有时间都花在比较字符串上，这并不一定意味着你需要改进你的字符串比较功能。你最好改进你的算法，这样你一开始就不需要做那么多的比较。过早的优化和所有这些。