# [返回主 Markdown](./FSharpForFunAndProfit翻译.md)



# 1 企业开发者从地狱归来

*Part of the "The Return of the EDFH" series (*[link](https://fsharpforfunandprofit.com/posts/return-of-the-edfh/#series-toc)*)*

更多的恶意合规性，更多的基于属性的测试
2021年2月14日

https://fsharpforfunandprofit.com/posts/return-of-the-edfh/

在[之前的一系列帖子](https://fsharpforfunandprofit.com/pbt)中，我向您介绍了一位精疲力竭、懒惰的程序员，他被称为*地狱企业开发人员*（*Enterprise Developer From Hell*），简称 EDFH。正如我们所见，EDFH 喜欢[恶意遵守规定](https://www.reddit.com/r/MaliciousCompliance/top/?sort=top&t=all)。

最近，EDFH 对一个[采访问题的病毒式回答](https://twitter.com/allenholub/status/1357115515672555520)明显产生了影响。

> 编写一个将输入转换为输出的函数。
>
> 输入：“aaaabbbcca”
> 输出：[（a’，4），（b’，3），（c’，2），（a’’，1）]

当然，EDFH 的答案很简单：

```F#
let func inputStr =
  // hard code the answer
  [('a',4); ('b',3); ('c',2); ('a',1)]
```

因为这是我们得到的唯一规范，所以这是一个完美的实现！

不过，这很有趣，因为面试官显然要求更复杂的东西。

但这引发了两个非常重要的问题：面试官到底在问什么？他们怎么知道他们是否得到了它？

只有一个输入/输出对，有很多潜在的规范可以工作。然而，twitter 的共识是，这是一个行程编码（run-length encoding, RLE）问题。😀

所以现在我们有两个具体的挑战：

- RLE 的规格应该是什么？我们如何明确地定义它？
- 我们如何检查特定的 RLE 实现是否符合该规范？

那么 RLE 的规格到底是什么样子的呢？有趣的是，当我在互联网上搜索时，我没有找到太多。维基百科有一个 [RLE 页面](https://en.wikipedia.org/wiki/Run-length_encoding)，上面有一个示例，但没有规范。Rosetta Stone 有一个带有非正式规范的 [RLE 页面](https://rosettacode.org/wiki/Run-length_encoding)。

## 在 EDFH 存在的情况下进行测试

让我们把规格搁置一分钟，把注意力转向测试。我们如何检查 RLE 实现是否有效？

一种方法是进行基于示例的测试：

- 预期 `rle("")` 的输出为 `[]`
- 预期 `rle("a")` 的输出为 `[(a,1)]`
- 预期 `rle("aab")` 的输出为 `[(a,2); (b,1)]`

等等。

但是，如果我们回顾一下我们之前在 EDFH 方面的经验，他们肯定会发现一个通过所有测试的实现，但仍然是错误的。例如，EDFH 对上述示例的实现可能如下：

```F#
let rle inputStr =
  match inputStr with
  | "" ->
    []
  | "a" ->
    [('a',1)]
  | "aab" ->
    [('a',2); ('b',1)]
  | "aaaabbbcca" ->
    [('a',4); ('b',3); ('c',2); ('a',1)]
  // everything else
  | _ -> []
```

如果我们检查一下这个实现，它看起来相当不错！

```F#
rle "a"           //=> [('a',1);]
rle "aab"         //=> [('a',2); ('b',1)]
rle "aaaabbbcca"  //=> [('a',4); ('b',3); ('c',2); ('a',1)]
```

击败 EFDH 的最佳方法是使用随机输入，特别是基于属性的测试。

基于属性的测试的一个好处是，通过这样做，您通常可以发现规范。在[上一篇文章](https://fsharpforfunandprofit.com/posts/property-based-testing)中，我讨论了如何测试加法的实现。最终，我们发现了交换性、结合性和恒等性的性质。这些不仅定义了我们需要的测试，还几乎定义了“添加”的实际含义。

让我们看看我们是否可以为 RLE 做同样的事情。

## 使用 EDFH 实现帮助我们思考属性

请记住，在基于属性的测试中，我们不允许重新实现逻辑，而是必须提出适用于所有输入的通用属性。

但这是最难的部分——思考属性。但是，我们可以使用 EDFH 来指导我们！对于 EDFH 创建的每个实现，我们找出错误的原因，然后创建一个属性来捕获该需求。

例如，EDFH 可能会将 RLE 函数实现为空列表，而不管输入是什么：

```F#
let rle_empty (inputStr:string) : (char*int) list =
  []
```

为什么这是错的？因为输出必须与输入有某种连接。事实上，它应该包含输入中的每个字符。

那么，EDFH 将通过返回每个字符并计数1来进行报复。

```F#
let rle_allChars inputStr =
  inputStr
  |> Seq.toList
  |> List.map (fun ch -> (ch,1))
```

如果我们运行这个，我们得到

```F#
rle_allChars ""      //=> []
rle_allChars "a"     //=> [('a',1)]
rle_allChars "abc"   //=> [('a',1); ('b',1); ('c',1)]
rle_allChars "aab"   //=> [('a',1); ('a',1); ('b',1)]
```

这些输出确实包含了相应输入中的每个字符。

为什么这是错的？好吧，我们想收集“分数”，这意味着我们不应该在一起有两个 a。输出列表中的每个字符必须与相邻字符不同。

这对 EFDH 来说是一个简单的修复，只需在管道中添加 `distinct`！

```F#
let rle_distinct inputStr =
  inputStr
  |> Seq.distinct // added
  |> Seq.toList
  |> List.map (fun ch -> (ch,1))
```

现在输出满足“runs”属性——重复项已经消失。

```F#
rle_distinct "a"     //=> [('a',1)]
rle_distinct "aab"   //=> [('a',1); ('b',1))]
rle_distinct "aaabb" //=> [('a',1); ('b',1))]
```

为什么这是错的？那么这些数字呢？他们都只是 1。它们应该是什么？

如果不重新实现算法，我们不知道各个计数应该是多少，但我们知道它们应该加起来是多少：字符串中的字符数。如果源字符串中有 5 个字符，则游程长度的总和也应该是 5。

不幸的是，EDFH 对此也有答案。它们的实现只能使用 `groupBy` 或 `countBy` 来获取计数。

```F#
let rle_groupedCount inputStr =
  inputStr
  |> Seq.countBy id
  |> Seq.toList
```

乍一看，输出效果不错

```F#
rle_groupedCount "aab"         //=> [('a',2); ('b',1))]
rle_groupedCount "aaabb"       //=> [('a',3); ('b',3))]
rle_groupedCount "aaaabbbcca"  //=> [('a',5); ('b',3); ('c',2))]
```

但有一个微妙的问题。在第三个示例中，`'a'` 有两个不同的运行，但 `rle_groupedCount` 实现将它们合并在一起。

我们想要的：

```F#
[('a',4); ('b',3); ('c',2); ('a',1)]
```

我们得到了什么：

```F#
[('a',5); ('b',3); ('c',2)]
//    ^ wrong number      ^ another entry needed here
```

`groupedCount` 方法的问题在于它没有考虑字符的顺序。我们能想出什么样的属性来捕捉这种情况？

检查排序的最简单方法就是反转某些内容！在这种情况下，我们可以有一个属性：“反向输入应该给出反向输出”。`rle_groupedCount` 实现会失败——这正是我们想要的。

因此，只需几分钟的思考（以及 EDFH 的一些帮助），我们就可以使用一些属性来检查 RLE 实现：

- 输出必须包含输入中的所有字符
- 输出中没有两个相邻的字符可以相同
- 输出中的行程长度之和必须等于输入的总长度
- 如果输入被反转，输出也必须被反转

> 这足以正确检查 RLE 实现吗？你能想到任何满足这些属性但错误的恶意 EDFH 实现吗？我们将在[稍后的帖子](https://fsharpforfunandprofit.com/posts/return-of-the-edfh-3)中重新讨论这个问题。	

## 属性检查实践

让我们把这些概念付诸实践。我们将使用 F# 库 `FsCheck` 来测试这些属性，包括好的和坏的实现。

从 F# 5 开始，很容易将 FsCheck 加载到交互式工作区中。你可以像这样直接引用它：

```F#
#r "nuget:FsCheck"
```

*注意：有关这些示例中使用的代码，请参阅本文底部的链接*

现在我们可以编写第一个属性：“结果必须包含输入中的所有字符”

```F#
// An RLE implementation has this signature
type RleImpl = string -> (char*int) list

let propUsesAllCharacters (impl:RleImpl) inputStr =
  let output = impl inputStr
  let expected =
    inputStr
    |> Seq.distinct
    |> Seq.toList
  let actual =
    output
    |> Seq.map fst
    |> Seq.distinct
    |> Seq.toList
  expected = actual
```

通常，属性的唯一参数是被测试的参数，但在这种情况下，我们还将传递一个实现参数，以便我们可以使用EDFH实现以及（希望）正确的实现进行测试

### 检查 rle_empty 实现

让我们用第一个 EDFH 实现来尝试一下，这个实现总是返回空列表：

```F#
let impl = rle_empty
let prop = propUsesAllCharacters impl
FsCheck.Check.Quick prop
```

FsCheck 的回应是：

```
Falsifiable, after 1 test (1 shrink) (StdGen (777291017, 296855223)):
Original:
"#"
Shrunk:
"a"
```

换句话说，简单地使用最小字符串“a”作为输入将破坏该属性。

### 检查 rle_allChars 的实现

如果我们尝试使用 `rle_allChars` 实现…

```F#
let impl = rle_allChars
let prop = propUsesAllCharacters impl
FsCheck.Check.Quick prop
```

…我们立即得到 `ArgumentNullException`，因为我们完全忘记了在实现中处理 null 输入！谢谢你，基于属性的测试！

让我们修复实现以处理 null…

```F#
let rle_allChars inputStr =
  // add null check
  if System.String.IsNullOrEmpty inputStr then
    []
  else
    inputStr
    |> Seq.toList
    |> List.map (fun ch -> (ch,1))
```

…然后再试一次——哎呀——我们又遇到了一个空问题，这次是在我们的属性中。我们也来解决这个问题。

```F#
let propUsesAllCharacters (impl:RleImpl) inputStr =
  let output = impl inputStr
  let expected =
    if System.String.IsNullOrEmpty inputStr then
      []
    else
      inputStr
      |> Seq.distinct
      |> Seq.toList
  let actual =
    output
    |> Seq.map fst
    |> Seq.distinct
    |> Seq.toList
  expected = actual
```

现在，如果我们再试一次，属性就通过了。

`好的，通过了 100 次测试。`

因此，正如我们所料，不正确的 `rle_allChars` 实现确实通过了。这就是为什么我们需要下一个属性：“输出中的相邻字符不能相同”

## “相邻字符不相同”属性

为了定义此属性，我们将首先定义一个辅助函数 `removeDupAdjacentChars`，用于删除重复项。

```F#
/// Given a list of elements, remove elements that have the
/// same char as the preceding element.
/// Example:
///   removeDupAdjacentChars ['a';'a';'b';'b';'a'] => ['a'; 'b'; 'a']
let removeDupAdjacentChars charList =
  let folder stack element =
    match stack with
    | [] ->
      // First time? Create the stack
      [element]
    | top::_ ->
      // New element? add it to the stack
      if top <> element then
        element::stack
      // else leave stack alone
      else
        stack

  // Loop over the input, generating a list of non-dup items.
  // These are in reverse order. so reverse the result
  charList |> List.fold folder [] |> List.rev
```

有了这个，我们的属性将从输出中获取字符，然后删除重复字符。如果实现正确，删除重复项应该没有任何效果。

```F#
/// Property: "Adjacent characters in the output cannot be the same"
let propAdjacentCharactersAreNotSame (impl:RleImpl) inputStr =
  let output = impl inputStr
  let actual =
    output
    |> Seq.map fst
    |> Seq.toList
  let expected =
    actual
    |> removeDupAdjacentChars // should have no effect
  expected = actual // should be the same
```

现在，让我们对照 EDFH 的 `rle_allChars` 实现来检查这个新属性：

```F#
let impl = rle_allChars
let prop = propAdjacentCharactersAreNotSame impl
FsCheck.Check.Quick prop
```

还有…

`好的，通过了100次测试。`

这是出乎意料的。也许我们只是运气不好？让我们将默认配置更改为 10000 次运行，而不是 100 次。

```F#
let impl = rle_allChars
let prop = propAdjacentCharactersAreNotSame impl
let config = {FsCheck.Config.Default with MaxTest = 10000}
FsCheck.Check.One(config,prop)
```

还有…

`好的，通过了10000次测试。`

…它仍然会过去。这可不好。

嗯，让我们添加一个快速 `printf` 来打印 FsCheck 生成的字符串，这样我们就可以看到发生了什么。

```F#
let propAdjacentCharactersAreNotSame (impl:RleImpl) inputStr =
  let output = impl inputStr
  printfn "%s" inputStr
  // etc
```

以下是 FsCheck 生成的输入字符串的样子：

```
v$D
%q6,NDUwm9~ 8I?a-ruc(@6Gi_+pT;1SdZ|H
E`Vxc(1daN
t/vLH$".5m8RjMrlCUb1J1'
Y[Q?zh^#ELn:0u
```

我们可以看到，字符串是非常随机的，几乎从来没有一系列重复的字符。从测试 RLE 算法的角度来看，这些输入是完全无用的！

> 这个故事的寓意是，就像常规 TDD 一样，确保你从失败的测试开始。只有这样，你才能确保你的正确实现是出于正确的原因。

因此，我们现在需要做的是生成*有趣的*输入，而不是随机字符串。

我们如何做到这一点？那么，我们如何在不进行粗略 `print` 调试的情况下监控输入内容呢？

这将是[下一期](https://fsharpforfunandprofit.com/posts/return-of-the-edfh-2)的主题！

> 本文中使用的源代码可以在[这里找到](https://github.com/swlaschin/fsharpforfunandprofit.com_code/tree/master/posts/return-of-the-edfh)。



# 2 为基于属性的测试生成有趣的输入

*Part of the "The Return of the EDFH" series (*[link](https://fsharpforfunandprofit.com/posts/return-of-the-edfh-2/#series-toc)*)*

以及如何对它们进行分类
2021年2月15日

https://fsharpforfunandprofit.com/posts/return-of-the-edfh-2/

在[上一篇文章](https://fsharpforfunandprofit.com/posts/return-of-the-edfh)中，我们试图为行程编码（RLE）实现定义一些属性，但由于 FsCheck 生成的随机值不是很有用而陷入困境。

在这篇文章中，我们将探讨生成“有趣”输入的几种方法，以及如何观察它们，以便我们可以确保它们确实有趣。

## 观察生成的数据

我们应该做的第一件事是添加某种监控，看看有多少输入是有趣的。

那么，什么是“有趣”的输入呢？对于这种情况，它是一个有一些游程的字符串。这意味着一个由像这样的随机字符组成的字符串…

`%q6，NDUwm9~ 8I?a-ruc(@6Gi_+pT;1SdZ|H`

…作为 RLE 实现的输入不是很有趣。

在不尝试重新实现 RLE 逻辑的情况下，确定是否有游程的一种方法是查看不同字符的数量是否远小于字符串的长度。如果这是真的，那么根据[鸽子洞原理](https://en.wikipedia.org/wiki/Pigeonhole_principle)，一定有一些字符的重复。这并不能确保有运行，但如果我们把差异做得足够大，大多数“有趣”的输入都会有运行。

以下是 `isInterestingString` 函数的定义：

```F#
let isInterestingString inputStr =
  if System.String.IsNullOrEmpty inputStr then
    false
  else
    let distinctChars =
      inputStr
      |> Seq.countBy id
      |> Seq.length
    distinctChars <= (inputStr.Length / 2)
```

如果我们测试它，我们可以看到它工作得很好。

```F#
isInterestingString ""        //=> false
isInterestingString "aa"      //=> true
isInterestingString "abc"     //=> false
isInterestingString "aabbccc" //=> true
isInterestingString "aabaaac" //=> true
isInterestingString "abcabc"  //=> true (but no runs)
```

为了监视输入是否有趣，我们将使用 FsCheck 函数 `Prop.classify`。

> `Prop.classify` 只是处理属性的众多函数之一。有关属性的更多信息，[请参阅 FsCheck 文档](https://fscheck.github.io/FsCheck//Properties.html)。或者查看[完整的 API](https://fscheck.github.io/FsCheck/reference/fscheck-prop.html)。

为了测试所有这些，让我们创建一个虚拟属性 `propIsInterestingString`，我们可以使用它来监视 FsCheck 生成的输入。实际的属性测试本身应该总是成功的，所以我们只使用 `true`。代码如下：

```F#
let propIsInterestingString input =
  let isInterestingInput = isInterestingString input

  true // we don't care about the actual test
  |> Prop.classify (not isInterestingInput) "not interesting"
  |> Prop.classify isInterestingInput "interesting"
```

现在让我们检查一下：

```F#
FsCheck.Check.Quick propIsInterestingString
// Ok, passed 100 tests (100% not interesting).
```

我们发现 100% 的输入都不有趣。所以我们需要更好的投入！

## 生成有趣的字符串，第 1 部分

一种方法是使用过滤器删除所有不感兴趣的字符串。但这将是非常低效的，因为有趣的字符串非常罕见。

相反，让我们生成有趣的字符串。对于我们的第一次尝试，我们将从非常简单的事情开始：我们将生成一个 `'a'` 字符列表和一个 `'b'` 字符列表，然后将这两个列表连接起来，给我们一些不错的运行。

为了做到这一点，我们将构建自己的生成器（见前面对生成器和收缩器的讨论）。FsCheck 提供了一组有用的函数来制作生成器，例如 `Gen.constant` 用于生成常量，`Gen.choose` 用于从区间中选取随机数，`Gen.elements` 用于从列表中选取随机元素。一旦你有了一个基本的生成器，你就可以 `map` 和 `filter` 它的输出，还可以用 `map2`、`oneOf` 等组合多个生成器。

> 有关使用生成器的更多信息，请参阅 FsCheck 文档。
>
> - [生成器使用概述](https://fscheck.github.io/FsCheck//TestData.html)
> - [生成器 API](https://fscheck.github.io/FsCheck/reference/fscheck-gen.html)

下面是我们使用生成器的代码：

```F#
let arbTwoCharString =
  // helper function to create strings from a list of chars
  let listToString chars =
    chars |> List.toArray |> System.String

  // random lists of 'a's and 'b's
  let genListA = Gen.constant 'a' |> Gen.listOf
  let genListB  = Gen.constant 'b' |> Gen.listOf

  (genListA,genListB)
  ||> Gen.map2 (fun listA listB -> listA @ listB )
  |> Gen.map listToString
  |> Arb.fromGen
```

我们生成一个 `'a'` 字符列表和一个 `'b'` 字符列表，然后使用 `map2` 将它们连接起来，然后将结果列表转换为字符串。作为最后一步，我们从生成器中构建一个 `Arbitrary`，这是我们在测试阶段所需的。我们现在不提供定制收缩器。

接下来，让我们从新生成器中采样一些随机字符串，看看它们是什么样子的：

```F#
arbTwoCharString.Generator |> Gen.sample 10 10
(*
[ "aaabbbbbbb"; "aaaaaaaaabb"; "b"; "abbbbbbbbbb";
  "aaabbbb"; "bbbbbb"; "aaaaaaaabbbbbbb";
  "a"; "aabbbb"; "aaaaabbbbbbbbb"]
*)
```

看起来不错。大多数字符串都有运行，正如我们所希望的那样。

现在，我们可以将此生成器应用于我们之前创建的 `propIsInterestingString` 属性。我们将使用 `Prop.forAll` 使用自定义生成器构造新属性，然后使用 `Check.Quick` 以通常的方式测试新属性。

```F#
// make a new property from the old one, with input from our generator
let prop = Prop.forAll arbTwoCharString propIsInterestingString
// check it
Check.Quick prop

(*
Ok, passed 100 tests.
97% interesting.
3% not interesting.
*)
```

而且这个输出要好得多！几乎所有的输入都很有趣。

## 生成有趣的字符串，第 2 部分

我们生成的字符串最多有两个游程，这并不能很好地代表我们想要游程长度编码（run-length encode）的真实字符串。我们可以增强生成器以包含多个字符列表，但它有点复杂，所以让我们从完全不同的方向来处理这个问题。

行程编码（run-length encoding）最常见的用途之一是压缩图像。我们可以将单色图像视为 0 和 1 的数组，其中 1 表示黑色像素。现在让我们考虑一个只有几个黑色像素的图像，这反过来意味着大量的白色像素，非常适合作为我们测试的输入。

我们如何生成这样的“图像”？从一组白色像素开始，随机将其中一些翻转为黑色，怎么样？

首先，我们需要一个辅助函数来随机翻转字符串中的“位”：

```F#
let flipRandomBits (str:string) = gen {

  // convert input to a mutable array
  let arr = str |> Seq.toArray

  // get a random subset of pixels
  let max = str.Length - 1
  let! indices = Gen.subListOf [0..max]

  // flip them
  for i in indices do arr.[i] <- '1'

  // convert back to a string
  return (System.String arr)
  }
```

然后我们可以构造一个生成器：

```F#
let arbPixels =
  gen {
    // randomly choose a length up to 50,
    // and set all pixels to 0
    let! pixelCount = Gen.choose(1,50)
    let image1 = String.replicate pixelCount "0"

    // then flip some pixels
    let! image2 = flipRandomBits image1

    return image2
    }
  |> Arb.fromGen // create a new Arb from the generator
```

现在让我们对生成器进行采样：

```F#
arbPixels.Generator |> Gen.sample 10 10
(*
"0001001000000000010010010000000";
"00000000000000000000000000000000000000000000100";
"0001111011111011110000011111";
"0101101101111111011010";
"10000010001011000001000001000001101000100100100000";
"0000000000001000";
"00010100000101000001010000100100001010000010100";
"00000000000000000000000000000000000000000";
"0000110101001010010";
"11100000001100011000000000000000001"
*)
```

看起来不错——只有一个字符串中没有 run。我们总是希望在我们的示例中有一些空字符串和没有 run 的字符串，以检查边缘情况。

我们现在可以使用这个新生成器尝试 `propIsInterestingString` 属性。

```F#
// make a new property from the old one, with input from our generator
let prop = Prop.forAll arbPixels propIsInterestingString
// check it
Check.Quick prop

(*
Ok, passed 100 tests.
94% interesting.
6% not interesting.
*)
```

同样，我们得到了 94% 有趣字符串的有用结果。

## 是时候测试 EDFH 的性能了

现在我们有了一种可靠的生成字符串的方法，我们可以重新访问上一篇文章中的属性，看看 EDFH 的实现是否通过。

作为提醒，为了正确实现 RLE，以下是我们提出的属性：

- 输出必须包含输入中的所有字符
- 输出中的两个相邻字符不能相同
- 输出中的行程长度之和必须等于输入的总长度
- 如果输入被反转，输出也必须被反转

以下是每个的代码：

### Prop #1：输出必须包含输入中的所有字符

```F#
// A RLE implementation has this signature
type RleImpl = string -> (char*int) list
let propUsesAllCharacters (impl:RleImpl) inputStr =
  let output = impl inputStr
  let expected =
    if System.String.IsNullOrEmpty inputStr then
      []
    else
      inputStr
      |> Seq.distinct
      |> Seq.toList
  let actual =
    output
    |> Seq.map fst
    |> Seq.distinct
    |> Seq.toList
  expected = actual
```

注意：实现时，此属性实际上比“包含输入中的所有字符”*更强*。如果我们想要这样做，我们应该在比较之前将 `expected` 和 `actual` 转换为无序集。但是，由于我们将它们作为列表保留，因此实现的属性实际上“包含输入中的所有字符，*并且顺序相同*”。

### Prop #2：输出中的两个相邻字符不能相同

```F#
let propAdjacentCharactersAreNotSame (impl:RleImpl) inputStr =
  let output = impl inputStr
  let actual =
    output
    |> Seq.map fst
    |> Seq.toList
  let expected =
    actual
    |> removeDupAdjacentChars // should have no effect
  expected = actual // should be the same
```

提醒：此代码中的 `removeDupAdjacentChars` 函数是在[上一篇文章](https://fsharpforfunandprofit.com/posts/return-of-the-edfh/#the-adjacent-characters-are-not-the-same-property)中定义的。

### Prop #3：输出中的游程长度之和必须等于输入的长度

```F#
let propRunLengthSum_eq_inputLength (impl:RleImpl) inputStr =
  let output = impl inputStr
  let expected = inputStr.Length
  let actual = output |> List.sumBy snd
  expected = actual // should be the same
```

在这里，我们只是对每个 `(char,run-length)` 元组的第二部分求和。

### Prop #4：如果输入被反转，输出也必须被反转

```F#
/// Helper to reverse strings
let strRev (str:string) =
  str
  |> Seq.rev
  |> Seq.toArray
  |> System.String

let propInputReversed_implies_outputReversed (impl:RleImpl) inputStr =
  // original
  let output1 =
    inputStr |> impl

  // reversed
  let output2 =
    inputStr |> strRev |> impl

  List.rev output1 = output2 // should be the same
```

### 组合属性

最后，我们可以将所有四个属性组合成一个复合属性。四个子属性中的每一个都有一个带有 `@|` 的标签，这样当复合属性失败时，我们就知道是哪个子属性导致了失败。

```F#
let propRle (impl:RleImpl) inputStr =
  let prop1 =
    propUsesAllCharacters impl inputStr
    |@ "propUsesAllCharacters"
  let prop2 =
    propAdjacentCharactersAreNotSame impl inputStr
    |@ "propAdjacentCharactersAreNotSame"
  let prop3 =
    propRunLengthSum_eq_inputLength impl inputStr
    |@ "propRunLengthSum_eq_inputLength"
  let prop4 =
    propInputReversed_implies_outputReversed impl inputStr
    |@ "propInputReversed_implies_outputReversed"

  // combine them
  prop1 .&. prop2 .&. prop3 .&. prop4
```

## 测试 EDFH 实现

现在，最后，我们可以根据复合属性测试 EDFH 实现。

第一个 EDFH 实现只是返回了一个空列表。

```F#
/// Return an empty list
let rle_empty (inputStr:string) : (char*int) list =
  []
```

我们希望它在第一个属性上失败：“输出必须包含输入中的所有字符”。

```F#
let prop = Prop.forAll arbPixels (propRle rle_empty)
// -- expect to fail on propUsesAllCharacters

// check it
Check.Quick prop
(*
Falsifiable, after 1 test (0 shrinks)
Label of failing property: propUsesAllCharacters
*)
```

确实如此。

### EDFH 实现 #2

下一个 EDFH 实现只是将每个 char 作为自己的 run 返回，run 长度为 1。

```F#
/// Return each char with count 1
let rle_allChars inputStr =
  // add null check
  if System.String.IsNullOrEmpty inputStr then
    []
  else
    inputStr
    |> Seq.toList
    |> List.map (fun ch -> (ch,1))
```

我们希望它在第二个属性上失败：“输出中没有两个相邻的字符可以相同”。

```F#
let prop = Prop.forAll arbPixels (propRle rle_allChars)
// -- expect to fail on propAdjacentCharactersAreNotSame

// check it
Check.Quick prop
(*
Falsifiable, after 1 test (0 shrinks)
Label of failing property: propAdjacentCharactersAreNotSame
*)
```

确实如此。

### EDFH 实现 #3

第三个EDFH实现通过执行不同的第一个来避免重复字符问题。

```F#
let rle_distinct inputStr =
  // add null check
  if System.String.IsNullOrEmpty inputStr then
    []
  else
    inputStr
    |> Seq.distinct
    |> Seq.toList
    |> List.map (fun ch -> (ch,1))
```

它将传递第二个属性：“输出中没有两个相邻的字符可以相同”，但我们预计它在第三个属性上会失败：“输出的游程长度之和必须等于输入的总长度”。

```F#
let prop = Prop.forAll arbPixels (propRle rle_distinct)
// -- expect to fail on propRunLengthSum_eq_inputLength

// check it
Check.Quick prop
(*
Falsifiable, after 1 test (0 shrinks)
Label of failing property: propRunLengthSum_eq_inputLength
*)
```

确实如此！

### EDFH 实现 #4

最后一个 EDFH 实现避免了重复字符的问题，并通过执行 `groupBy` 操作获得了正确的整体游程长度。

```F#
let rle_countBy inputStr =
  if System.String.IsNullOrEmpty inputStr then
    []
  else
    inputStr
    |> Seq.countBy id
    |> Seq.toList
```

这就是为什么我们添加了第四个属性来捕捉这一点：“如果输入被反转，输出也必须被反转”。

```F#
let prop = Prop.forAll arbPixels (propRle rle_countBy)
// -- expect to fail on propInputReversed_implies_outputReversed

// check it
Check.Quick prop
(*
Falsifiable, after 1 test (0 shrinks)
Label of failing property: propInputReversed_implies_outputReversed
*)
```

它如预期般失败了。

## 测试正确的实现

在所有这些糟糕的实现之后，让我们看看一些正确的实现。我们可以使用我们的四个属性来确信特定的实现是正确的。

### 正确实现 #1

我们的第一个实现将使用递归。它将去掉第一个字符，留下一个较小的列表。然后，它将对较小的列表应用相同的逻辑。

```F#
let rle_recursive inputStr =

  // inner recursive function
  let rec loop input =
    match input with
    | [] -> []
    | head::_ ->
      [
      // get a run
      let runLength = List.length (List.takeWhile ((=) head) input)
      // return it
      yield head,runLength
      // skip the run and repeat
      yield! loop (List.skip runLength input)
      ]

  // main
  inputStr |> Seq.toList |> loop
```

如果我们测试一下，它似乎按预期工作。

```F#
rle_recursive "aaaabbbcca"
// [('a', 4); ('b', 3); ('c', 2); ('a', 1)]
```

但真的吗？让我们通过属性检查器来确认：

```F#
let prop = Prop.forAll arbPixels (propRle rle_recursive)
// -- expect it to not fail

// check it
Check.Quick prop
(*
Ok, passed 100 tests.
*)
```

是的，没有属性失败！

### 正确实现 #2

上面的递归实现可能对非常大的输入字符串有一些问题。首先，内部循环不是尾部递归的，因此堆栈可能会溢出。此外，通过不断创建子列表，它会产生大量垃圾，从而影响性能。

另一种方法是使用 `Seq.fold` 迭代输入一次。以下是一个基本实现：

```F#
let rle_fold inputStr =
  // This implementation iterates over the list
  // using the 'folder' function and accumulates
  // into 'acc'

  // helper
  let folder (currChar,currCount,acc) inputChar =
    if currChar <> inputChar then
      // push old run onto accumulator
      let acc' = (currChar,currCount) :: acc
      // start new run
      (inputChar,1,acc')
    else
      // same run, so increment count
      (currChar,currCount+1,acc)

  // helper
  let toFinalList (currChar,currCount,acc) =
    // push final run onto acc
    (currChar,currCount) :: acc
    |> List.rev

  // main
  if System.String.IsNullOrEmpty inputStr then
    []
  else
    let head = inputStr.[0]
    let tail = inputStr.[1..inputStr.Length-1]
    let initialState = (head,1,[])
    tail
    |> Seq.fold folder initialState
    |> toFinalList
```

我们可以通过使用可变累加器、使用数组而不是列表等来进一步优化这一点。但这足以演示原理。

以下是一些交互式测试，以确保其按预期工作：

```F#
rle_fold ""    //=> []
rle_fold "a"   //=> [('a',1)]
rle_fold "aa"  //=> [('a',2)]
rle_fold "ab"  //=> [('a',1); ('b',1)]
rle_fold "aab" //=> [('a',2); ('b',1)]
rle_fold "abb" //=> [('a',1); ('b',2)]
rle_fold "aaaabbbcca"
  //=> [('a',4); ('b',3); ('c',2); ('a',1)]
```

当然，使用属性检查器是一种更好的方法来确保：

```F#
let prop = Prop.forAll arbPixels (propRle rle_fold)
// -- expect it to not fail

// check it
Check.Quick prop
(*
Ok, passed 100 tests.
*)
```

它确实通过了所有的测试。

因此，逻辑是正确的，但如上所述，在考虑生产就绪之前，我们还应该对大型输入和优化进行一些性能测试。那完全是另一个话题了！

优化有时会引入错误，但现在我们有了这些属性，我们可以以同样的方式测试优化后的代码，并确信任何错误都会立即被检测到。

## 摘要

在这篇文章中，我们首先找到了一种生成“有趣”输入的方法，然后使用这些输入，对 EDFH 实现运行上次的属性。他们都失败了！然后我们定义了两个满足所有属性的正确实现。

那么，我们现在结束了吗？不是的。事实证明，EDFH 仍然可以创建一个满足所有属性的实现！为了最终击败EDFH，我们需要做得更好。

这将是[下一期](https://fsharpforfunandprofit.com/posts/return-of-the-edfh-3)的主题。

> 本文中使用的源代码可以在[这里找到](https://github.com/swlaschin/fsharpforfunandprofit.com_code/tree/master/posts/return-of-the-edfh-2)。

# 3 EDFH 再次被击败

*Part of the "The Return of the EDFH" series (*[link](https://fsharpforfunandprofit.com/posts/return-of-the-edfh-3/#series-toc)*)*

2021年2月16日

https://fsharpforfunandprofit.com/posts/return-of-the-edfh-3/

在[本系列的第一篇文章](https://fsharpforfunandprofit.com/posts/return-of-the-edfh)中，我们提出了一些可用于测试行程编码实现的属性：

- 输出必须以相同的顺序包含输入中的所有字符
- 输出中的两个相邻字符不能相同
- 输出中的行程长度之和必须等于输入的总长度
- 如果输入被反转，输出也必须被反转

在[上一篇文章](https://fsharpforfunandprofit.com/posts/return-of-the-edfh-2)中，我们测试了由[地狱企业开发人员](https://fsharpforfunandprofit.com/pbt)创建的各种RLE实现，并很高兴它们都失败了。

但是，这四个属性足以正确检查 RLE 实现吗？EDFH 能否创建一个满足这些属性但又错误的实现？

答案是肯定的！EDFH 可以获取正确实现的输出，然后在列表的开头和结尾添加一些额外的字符来破坏答案。确切地说，要添加的内容受到上述属性的约束：

- “两个相邻字符”属性意味着新前缀必须与第一个字符不同。
- 但是“所有字符顺序相同”属性意味着 EDFH 不能只添加不同的字符，因为这会破坏“顺序相同”。解决方法是 EDFH 添加前两个字符的副本！
- “游程长度之和”属性意味着新前缀的游程长度必须从后续游程中窃取计数。如果我们没有窃取计数，并使用0作为这些新元素的游程长度，那么这实际上是一个可接受的 RLE——完全没有损坏！
- 最后，“reversed”属性意味着列表的前面和后面都必须以相同的方式进行修改。为了避免两次损坏相同的元素，我们要求列表至少有四个元素。

将所有这些要求放在一起，我们可以提出这个实现，其中 `rle_recursive` 是[上一篇文章](https://fsharpforfunandprofit.com/posts/return-of-the-edfh-2#correct1)中正确的 RLE 实现。

```F#
/// An incorrect implementation that satisfies all the properties
let rle_corrupted (inputStr:string) : (char*int) list =

  // helper
  let duplicateFirstTwoChars list =
    match list with
    | (ch1,n)::(ch2,m)::e3::e4::tail when n > 1 && m > 1 ->
      (ch1,1)::(ch2,1)::(ch1,n-1)::(ch2,m-1)::e3::e4::tail
    | _ ->
      list

  // start with correct output...
  let output = rle_recursive inputStr

  // ...and then corrupt it by
  // adding extra chars front and back
  output
  |> duplicateFirstTwoChars
  |> List.rev
  |> duplicateFirstTwoChars
  |> List.rev
```

请注意，我们仅在以下情况下损坏列表：

- 前两次的行程长度大于 1，所以我们可以偷 1。
- 列表中至少有四个元素，这样我们就可以反转并重新损坏另一端。如果删除 `::e3::e4::` 上的匹配项，则实现将使“reversed”属性失败。

那么，当我们运行检查器时，我们希望增加测试的数量，因为只有少数输入符合损坏的要求，我们希望确保捕获到它们。

好的，让我们对照上次定义的复合属性 `propRle` 来检查这个新的 EDFH 实现。如前所述，我们将使用自定义生成器 `arbPixels` 来生成包含大量运行的字符串。

```F#
let prop = Prop.forAll arbPixels (propRle rle_corrupted)

// check it thoroughly
let config = { Config.Default with MaxTest=10000}
Check.One(config,prop)
// Ok, passed 10000 tests.
```

它通过了。哦，天哪！我们现在该如何击败 EDFH？

在之前的一篇文章中，我描述了一些可用于生成属性的方法。我们已经使用其中一个（“有些东西永远不会改变”）来要求一个不变量，即源字符串中的每个字符也出现在 RLE 中。我们将在这篇文章中使用其中的两个：

- “不同的路径，相同的目的地”
- “来回”

## 结构保持性能测试

对于我们的第一种方法，我们将使用“交换图”方法的变体。

![img](https://fsharpforfunandprofit.com/posts/return-of-the-edfh-3/property_commutative.png)

在这种情况下，我们将利用游程编码是一种“结构保持”操作的事实。这意味着“字符串世界”中的操作在转换为“RLE 世界”后得以保留。

对字符串的定义操作是串联（因为字符串是[幺半群](https://fsharpforfunandprofit.com/posts/monoids-without-tears/)），因此我们要求对字符串的结构保持操作将字符串世界中的串联映射到目标世界中的级联

`OP(str1 + str2) = OP(str1) + OP(str2)`

`strLen` 是一个对字符串进行简单结构保留操作的示例。它不仅仅是字符串到整数的随机映射，因为它保留了连接操作。

`strLen(str1 + str2) = strLen(str1) + strLen(str2)`

值得注意的是，“结构保留”并不意味着它保留了字符串的内容，只是保留了字符串之间的关系。上面的 `strLen` 函数不保留字符串的内容，你甚至可以有一个 `empty` 函数，将所有字符串映射到一个空列表上。它不会保留内容，但会保留结构，因为：

`empty(str1 + str2) = empty(str1) + empty(str2)`

在我们的例子中，我们希望 `rle` 函数也能保留字符串的结构，这意味着我们需要：

`rle(str1 + str2) = rle(str1) + rle(str2)`

所以现在我们需要的是一种“添加”两个 `Rle` 结构的方法。即使它们是列表，我们也不能直接将它们连接起来，因为我们最终可能会得到相邻的运行。相反，我们希望合并相同字符的运行：

```F#
// wrong
['a',1] + ['a',1]  //=> [('a',1); ('a',1)]
// correct
['a',1] + ['a',1]  //=> [('a',2)]
```

下面是这样一个函数的实现。所有特殊情况都有点棘手。

```F#
// A Rle is a list of chars and run-lengths
type Rle = (char*int) list

let rec rleConcat (rle1:Rle) (rle2:Rle) =
  match rle1 with
  // 0 elements, so return rle2
  | [] -> rle2

  // 1 element left, so compare with
  // first element of rle2 and merge if equal
  | [ (x,xCount) ] ->
    match rle2 with
    | [] ->
      rle1
    | (y,yCount)::tail ->
      if x = y then
        // merge
        (x,(xCount+yCount)) :: tail
      else
        (x,xCount)::(y,yCount)::tail

  // longer than 1, so recurse
  | head::tail ->
    head :: (rleConcat tail rle2)
```

一些交互式测试，以确保它看起来不错：

```F#
rleConcat ['a',1] ['a',1]  //=> [('a',2)]
rleConcat ['a',1] ['b',1]  //=> [('a',1); ('b',1)]

let rle1 = rle_recursive "aaabb"
let rle2 = rle_recursive "bccc"
let rle3 = rle_recursive ("aaabb" + "bccc")
rle3 = rleConcat rle1 rle2   //=> true
```

我们现在有了 RLE 的“concat”函数，所以我们可以定义一个属性来检查 RLE 实现是否保留了字符串连接。

```F#
let propConcat (impl:RleImpl) (str1,str2) =
  let ( <+> ) = rleConcat

  let rle1 = impl str1
  let rle2 = impl str2
  let actual = rle1 <+> rle2
  let expected = impl (str1 + str2)
  actual = expected
```

此属性需要一对字符串，而不仅仅是一个，因此我们需要创建一个新的生成器：

```F#
let arbPixelsPair =
  arbPixels.Generator
  |> Gen.two
  |> Arb.fromGen
```

最后，我们可以根据属性检查 EDFH 的实现：

```F#
let prop = Prop.forAll arbPixelsPair (propConcat rle_corrupted)

// check it thoroughly
let config = { Config.Default with MaxTest=10000}
Check.One(config,prop)
// Falsifiable, after 2 tests
```

失败了！但正确的实现仍然成功：

```F#
let prop = Prop.forAll arbPixelsPair (propConcat rle_recursive)

// check it thoroughly
let config = { Config.Default with MaxTest=10000}
Check.One(config,prop)
// Ok, passed 10000 tests.
```

让我们复习一下。我们可以用更通用的“concat 保持”属性替换过度约束的“reverse”属性，这样我们的行程编码实现的属性如下：

- **内容不变性**：输出必须以相同的顺序包含输入中的所有字符。
- **游程不同**：输出中的两个相邻字符不能相同。
- **总长度相同**：输出中的游程长度之和必须等于输入的总长度。
- **结构保持**：必须保持如上所述的连接。

我们有四个单独的属性，每个属性都必须单独发现和实现。有更简单的方法吗？是的，有！

## 使用逆函数进行测试

如果我们回到行程编码的目的，它应该以压缩但无损的方式表示字符串。“无损”是关键。这意味着我们有一个逆函数——一个可以从 RLE 数据结构中重建原始字符串的函数。

由于我们有一个逆，我们可以进行“前后”测试。编码和解码应该让我们回到起点。

![img](https://fsharpforfunandprofit.com/posts/return-of-the-edfh-3/property_inverse.png)

在我们开始处理解码器之前，让我们停止并为 RLE 编码定义一个适当的类型，以便我们可以对其进行一些封装。这将在以后证明是有用的。

```F#
type Rle = Rle of (char*int) list
```

现在，我们应该返回并更改之前的“编码”实现，以立即返回 `Rle`。我把它当作练习。

实现一个接受 `Rle` 并返回 `string` 的解码器很简单。有很多方法可以做到这一点。我选择使用可变 `StringBuilder` 和嵌套循环来提高性能。

```F#
let decode (Rle rle) : string =
  let sb = System.Text.StringBuilder()
  for (ch,count) in rle do
    for _ in [1..count] do
      sb.Append(ch) |> ignore
  sb.ToString()
```

让我们以交互方式快速测试一下：

```F#
rle_recursive "111000011"
|> Rle     // wrap in Rle type
|> decode  //=> "111000011"
```

好的，这似乎有效。我们可以创建一系列属性，分别用于测试 `decode` 和编码，但现在我们只将它们作为一对逆进行测试。

有了 `decode` 函数，我们可以编写“there and back again”属性：

```F#
let propEncodeDecode (encode:RleImpl) inputStr =
  let actual =
    inputStr
    |> encode
    |> Rle  // wrap in Rle type
    |> decode

  actual = inputStr
```

让我们对照 EDFH 的错误实现来检查这个属性，它失败了。杰出的！

```F#
let prop = Prop.forAll arbPixels (propEncodeDecode rle_corrupted)

// check it thoroughly
let config = { Config.Default with MaxTest=10000}
Check.One(config,prop)
// Falsifiable, after 2 tests
```

如果我们根据良好的实现检查这个属性，它就会通过。

```F#
let prop = Prop.forAll arbPixels (propEncodeDecode rle_recursive)

// check it thoroughly
let config = { Config.Default with MaxTest=10000}
Check.One(config,prop)
// Ok, passed 10000 tests.
```

因此，我们最终创建了一个属性，可以击败“损坏”的 EDFH 函数。

然而，我们还没有完成，因为之前的 EDFH 实现之一确实满足了这个属性，即最简单的 `rle_allChars`。

```F#
/// a very simple RLE implementation
let rle_allChars inputStr =
  inputStr
  |> Seq.toList
  |> List.map (fun ch -> (ch,1))

// make a property 
let prop = Prop.forAll arbPixels (propEncodeDecode rle_allChars)

// and check it thoroughly
let config = { Config.Default with MaxTest=10000}
Check.One(config,prop)
// Ok, passed 10000 tests.
```

这是因为它是一个正确的游程编码，而不是最大游程编码！

## 定义 RLE 规范

在第一篇文章中，我提到我无法快速找到 RLE 实现的程序员友好规范。我认为我们现在已经受够了。

首先，RLE 是无损的，所以我们可以说也必须有一个逆函数。即使没有确切定义逆函数是什么，我们也可以说“往返性质”成立。

其次，我们需要消除琐碎的编码，例如每个游程长度为 1 的编码。我们可以通过要求游程为最大值来实现这一点，这意味着相邻的游程不共享相同的字符。

我认为这就是我们所需要的。其他属性是隐含的。例如，由于往返属性，“包含输入中的所有字符”是隐含的。出于同样的原因，也隐含了“行程长度之和”属性。

所以，这是规格：

> RLE实现是一对函数 `encode : string->Rle` 和 `decode : Rle->string`，这样：
>
> - **往返**。由 `decode` 组成的 `encode` 与恒等函数相同。
> - **最大游程次数**。Rle 结构中没有相邻的游程共享相同的字符，并且所有游程长度都大于 0。

你能想出一种方法让 EDFH 打破这个规范吗？请在评论中告诉我。

## 奖励：走向另一个方向

我们可以就此止步，但让我们进一步探索 FsCheck。

编码和解码是彼此的逆，所以我们同样可以定义一个从解码开始的属性，然后对结果进行编码，如下所示：

```F#
let propDecodeEncode (encode:RleImpl) rle =
  let actual =
    rle
    |> decode
    |> encode
    |> Rle

  actual = rle
```

如果我们针对 EDFH 损坏的编码器进行测试，它将失败：

```F#
let prop = propDecodeEncode rle_corrupted
Check.Quick(prop)
// Falsifiable, after 4 tests
// Rle [('a', 0)]
```

但它也无法使用我们正确的 `rle_recursive` 编码器。

```F#
let prop = propDecodeEncode rle_recursive
Check.Quick(prop)
// Falsifiable, after 4 tests
// Rle [('a', 0)]
```

为什么？我们可以立即看到 FsCheck 正在生成一个 0 长度的游程，当解码和编码时，它将返回一个空列表。为了解决这个问题，我们必须再次创建自己的生成器。

### 观察有趣的 RLE

不过，在我们创建新的生成器之前，让我们进行一些监控，以便我们可以判断它是否真的在工作。

我们将遵循与以前相同的方法。首先，我们将定义“有趣”的样子，然后我们将创建一个虚拟属性来监视输入。

首先，我们会说一个“有趣”的 RLE 是一个长度非平凡且包含一些非平凡游程的 RLE。

```F#
let isInterestingRle (Rle rle) =
  let isLongList = rle.Length > 2
  let noOfLongRuns =
    rle
    |> List.filter (fun (_,run) -> run > 2)
    |> List.length
  isLongList && (noOfLongRuns > 2)
```

然后让我们用它来对属性的输入进行分类：

```F#
let propIsInterestingRle input =
  let isInterestingInput = isInterestingRle input

  true // we don't care about the actual test
  |> Prop.classify (not isInterestingInput) "not interesting"
  |> Prop.classify isInterestingInput "interesting"
```

结果很明显——FsCheck 自动生成的大多数输入都是无趣的。

```F#
Check.Quick propIsInterestingRle
// Ok, passed 100 tests.
// 99% not interesting.
// 1% interesting.
```

### 生成有趣的 RLE

那么，让我们建造一个生成器。我们将选取一个随机字符和一个随机游程长度，并将它们组合成一对，如下所示：

```F#
let arbRle =
  let genChar = Gen.elements ['a'..'z']
  let genRunLength = Gen.choose(1,10)
  Gen.zip genChar genRunLength
  |> Gen.listOf
  |> Gen.map Rle
  |> Arb.fromGen
```

如果我们使用这个新的生成器检查属性，现在的结果要好得多：

```F#
let prop = Prop.forAll arbRle propIsInterestingRle
Check.Quick prop
// Ok, passed 100 tests.
// 86% interesting.
// 14% not interesting.
```

让我们用这个新的生成器重新测试我们的正确实现。

```F#
let prop = Prop.forAll arbRle (propDecodeEncode rle_recursive)

// check it thoroughly
let config = { Config.Default with MaxTest=10000}
Check.One(config,prop)
// Falsifiable, after 82 tests
// Rle [('e', 7); ('e', 6); ('z', 10)]
```

哎呀，我们又做到了。它仍然失败了。

幸运的是，反例向我们展示了原因。两个相邻的字符是相同的，这意味着重新编码将与原始字符不匹配。对此的修复是过滤掉生成器逻辑中的这些共享字符运行。

以下是删除相邻运行的代码：

```F#
let removeAdjacentRuns runList =
  let folder prevRuns run =
    match prevRuns with
    | [] -> [run]
    | head::_ ->
      if fst head <> fst run then
        // add
        run::prevRuns
      else
        // duplicate -- ignore
        prevRuns
  runList
  |> List.fold folder []
  |> List.rev
```

这是更新的生成器：

```F#
let arbRle =
  let genChar = Gen.elements ['a'..'z']
  let genRunLength = Gen.choose(1,10)
  Gen.zip genChar genRunLength
  |> Gen.listOf
  |> Gen.map removeAdjacentRuns
  |> Gen.map Rle
  |> Arb.fromGen
```

现在，如果我们再测试一次，一切正常。

```F#
let prop = Prop.forAll arbRle (propDecodeEncode rle_recursive)

// check it thoroughly
let config = { Config.Default with MaxTest=10000}
Check.One(config,prop)
// Ok, passed 10000 tests.
```

## 为 RLE 注册生成器

FsCheck 为所有常见类型（`string`、`int` 等）定义了默认生成器，也可以通过反射为复合类型（记录、判别联合）生成数据，但正如我们所见，我们通常需要更多的控制。

到目前为止，我们一直在使用 `Prop.forAll` 将 `arbRle` 实例显式传递到每个测试中。FsCheck 支持为类型注册 `Arbitrary`，这样您就不必每次都传递它。对于一种会被大量重用的常见类型，这非常方便。

FsCheck 提供了许多有用的内置类型和自定义生成器，如 [PositiveInt](https://fscheck.github.io/FsCheck/reference/fscheck-positiveint.html)、[NonWhiteSpaceString](https://fscheck.github.io/FsCheck/reference/fscheck-nonwhitespacestring.html) 等（详见 [FsCheck 命名空间](https://fscheck.github.io/FsCheck/reference/fscheck.html)）。我们如何将自定义类型添加到此列表中？

[FsCheck 文档解释了如何操作](https://fscheck.github.io/FsCheck//TestData.html#Default-Generators-and-Shrinkers-based-on-type)。您首先为要注册的每个 `Arbitrary` 定义一个具有静态方法的类：

```F#
type MyGenerators =
  static member Rle() = arbRle

  // static member MyCustomType() = arbMyCustomType
```

然后向 FsCheck 注册该类：

```F#
Arb.register<MyGenerators>()
```

注册后，您可以获得样品：

```F#
Arb.generate<Rle> |> Gen.sample 5 4
// [Rle [('c', 2); ('m', 8)];
//  Rle [];
//  Rle [('e', 7); ('c', 2); ('s', 1); ('m', 8)];
//  Rle [('t', 3); ('e', 7); ('c', 2)]]
```

无需 `Prop.forAll` 即可检查属性。

```F#
let prop = propDecodeEncode rle_recursive

Check.Quick(prop)
// Ok, passed 100 tests.
```

## 结论

本系列到此结束。我们从一条关于如何回答面试问题的推文开始，这是一种愚蠢的方式，最终我们绕道进入 FsCheck，确保我们有“有趣”的输入，构建自己的生成器，并尝试使用属性的不同方式来获得对实现的信心。

我希望这给了你一些想法，可以用于你自己的基于属性的测试。玩得高兴！

> 本文中使用的源代码可以在[这里找到](https://github.com/swlaschin/fsharpforfunandprofit.com_code/tree/master/posts/return-of-the-edfh-3)。