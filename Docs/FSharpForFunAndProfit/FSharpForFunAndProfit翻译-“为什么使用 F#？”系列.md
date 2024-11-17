# [返回主 markdown](./FSharpForFunAndProfit翻译.md)

# 1 “为什么使用F#”系列介绍

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/why-use-fsharp-intro/#series-toc)*)*

F# 的好处概述
01 Apr 2012 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/why-use-fsharp-intro/

本系列文章将为您介绍F#的主要功能，然后向您展示F#如何帮助您进行日常开发。

## F# 与 C# 相比的主要优势

如果你已经熟悉C#或Java，你可能会想知道为什么学习另一种语言是值得的。F#有一些主要优点，我将其分为以下主题：

- **简洁**。F#不会被花括号、分号等编码“噪音”所困扰。得益于强大的类型推理系统，您几乎不必指定对象的类型。解决同样的问题通常需要更少的代码行。
- **方便**。许多常见的编程任务在F#中要简单得多。这包括创建和使用复杂类型定义、执行列表处理、比较和相等、状态机等等。由于函数是一级对象，因此通过创建具有其他函数作为参数的函数，或者组合现有函数以创建新功能，可以很容易地创建功能强大且可重用的代码。
- **正确**。F#有一个非常强大的类型系统，可以防止许多常见错误，如空引用异常。此外，您通常可以使用类型系统本身对业务逻辑进行编码，因此实际上不可能编写错误的代码，因为它在编译时被捕获为类型错误。
- **并发性**。F#有许多内置工具和库，可以在一次发生多件事时帮助编程系统。异步编程是直接支持的，并行性也是如此。F#还有一个消息队列系统，对事件处理和响应式编程有很好的支持。由于数据结构在默认情况下是不可变的，因此共享状态和避免锁要容易得多。
- **完整性**。虽然F#本质上是一种函数式语言，但它确实支持其他并非100%纯的风格，这使得与网站、数据库、其他应用程序等非纯世界的交互变得更加容易。特别是，F#被设计为混合函数式/OO语言，因此它几乎可以做C#能做的一切。当然，F#与无缝集成。NET生态系统，让您可以访问所有第三方。NET库和工具。最后，它是Visual Studio的一部分，这意味着您可以获得一个具有IntelliSense支持的好编辑器、一个调试器和许多用于单元测试、源代码控制和其他开发任务的插件。

在本系列文章的其余部分中，我将尝试使用独立的F#代码片段（通常与C#代码进行比较）来演示F#的每一个好处。我将简要介绍F#的所有主要特性，包括模式匹配、函数组合和并发编程。当你完成这个系列时，我希望你会对F#的力量和优雅印象深刻，并鼓励你在下一个项目中使用它！

## 如何阅读和使用示例代码

这些帖子中的所有代码片段都被设计为交互式运行。我强烈建议你在阅读每篇文章时评估这些片段。任何大型代码文件的源代码都将从帖子链接到。

本系列不是教程，所以我不会过多地介绍代码的工作原理。如果你不能理解一些细节，不要担心；本系列的目的只是向您介绍F#，并激发您更深入地学习它的欲望。

如果你有C#和Java等语言的经验，你可能会发现，即使你不熟悉关键字或库，你也可以很好地理解用其他类似语言编写的源代码。你可能会问“如何分配变量？”或“如何进行循环？”，有了这些答案，你就可以很快地完成一些基本的编程。

这种方法不适用于 F#，因为在纯形式中没有变量、循环和对象。不要沮丧，这最终会有意义的！如果你想更深入地学习 F#，“学习F#”页面上有一些有用的提示。

# 2 F#语法60秒

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/fsharp-in-60-seconds/#series-toc)*)*

关于如何阅读 F# 代码的快速概述
02 Apr 2012 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/fsharp-in-60-seconds/

下面是一个关于如何为不熟悉语法的新手阅读F#代码的快速概述。

它显然不是很详细，但应该足够了，这样你就可以阅读并理解本系列即将到来的示例的要点。如果你不理解所有内容，不要担心，因为当我们看到实际的代码示例时，我会给出更详细的解释。

F#语法和标准类C语法之间的两个主要区别是：

- 大括号不用于分隔代码块。相反，使用缩进（Python 与此类似）。
- 空格用于分隔参数，而不是逗号。

有些人觉得F#语法令人反感。如果你是其中之一，请考虑这句话：

> “优化你的符号，让人们在看到它的前10分钟不会感到困惑，但从那以后就会阻碍可读性，这是一个非常糟糕的错误。”（David MacIver，通过一篇关于Scala语法的文章）。

就我个人而言，我认为当你习惯了 F# 语法时，它非常清晰明了。在很多方面，它比 C# 语法更简单，关键字和特殊情况更少。

下面的示例代码是一个简单的F#脚本，它演示了您经常需要的大多数概念。

我鼓励你以交互方式测试这段代码，并尝试一下！要么：

- 将其键入 F# 脚本文件（扩展名为 .fsx）并将其发送到交互式窗口。有关详细信息，请参阅“安装和使用F#”页面。
- 或者，尝试在交互式窗口中运行此代码。记住要总在最后使用 `;;` 告诉解释器你已经完成输入并准备进行评估。

```F#
// single line comments use a double slash
(* multi line comments use (* . . . *) pair

-end of multi line comment- *)

// ======== "Variables" (but not really) ==========
// The "let" keyword defines an (immutable) value
let myInt = 5
let myFloat = 3.14
let myString = "hello"	//note that no types needed

// ======== Lists ============
let twoToFive = [2;3;4;5]        // Square brackets create a list with
                                 // semicolon delimiters.
let oneToFive = 1 :: twoToFive   // :: creates list with new 1st element
// The result is [1;2;3;4;5]
let zeroToFive = [0;1] @ twoToFive   // @ concats two lists

// IMPORTANT: commas are never used as delimiters, only semicolons!

// ======== Functions ========
// The "let" keyword also defines a named function.
let square x = x * x          // Note that no parens are used.
square 3                      // Now run the function. Again, no parens.

let add x y = x + y           // don't use add (x,y)! It means something
                              // completely different.
add 2 3                       // Now run the function.

// to define a multiline function, just use indents. No semicolons needed.
let evens list =
   let isEven x = x%2 = 0     // Define "isEven" as an inner ("nested") function
   List.filter isEven list    // List.filter is a library function
                              // with two parameters: a boolean function
                              // and a list to work on

evens oneToFive               // Now run the function

// You can use parens to clarify precedence. In this example,
// do "map" first, with two args, then do "sum" on the result.
// Without the parens, "List.map" would be passed as an arg to List.sum
let sumOfSquaresTo100 =
   List.sum ( List.map square [1..100] )

// You can pipe the output of one operation to the next using "|>"
// Here is the same sumOfSquares function written using pipes
let sumOfSquaresTo100piped =
   [1..100] |> List.map square |> List.sum  // "square" was defined earlier

// you can define lambdas (anonymous functions) using the "fun" keyword
let sumOfSquaresTo100withFun =
   [1..100] |> List.map (fun x->x*x) |> List.sum

// In F# returns are implicit -- no "return" needed. A function always
// returns the value of the last expression used.

// ======== Pattern Matching ========
// Match..with.. is a supercharged case/switch statement.
let simplePatternMatch =
   let x = "a"
   match x with
    | "a" -> printfn "x is a"
    | "b" -> printfn "x is b"
    | _ -> printfn "x is something else"   // underscore matches anything

// Some(..) and None are roughly analogous to Nullable wrappers
let validValue = Some(99)
let invalidValue = None

// In this example, match..with matches the "Some" and the "None",
// and also unpacks the value in the "Some" at the same time.
let optionPatternMatch input =
   match input with
    | Some i -> printfn "input is an int=%d" i
    | None -> printfn "input is missing"

optionPatternMatch validValue
optionPatternMatch invalidValue

// ========= Complex Data Types =========

// Tuple types are pairs, triples, etc. Tuples use commas.
let twoTuple = 1,2
let threeTuple = "a",2,true

// Record types have named fields. Semicolons are separators.
type Person = {First:string; Last:string}
let person1 = {First="john"; Last="Doe"}

// Union types have choices. Vertical bars are separators.
type Temp =
  | DegreesC of float
  | DegreesF of float
let temp = DegreesF 98.6

// Types can be combined recursively in complex ways.
// E.g. here is a union type that contains a list of the same type:
type Employee =
  | Worker of Person
  | Manager of Employee list
let jdoe = {First="John";Last="Doe"}
let worker = Worker jdoe

// ========= Printing =========
// The printf/printfn functions are similar to the
// Console.Write/WriteLine functions in C#.
printfn "Printing an int %i, a float %f, a bool %b" 1 2.0 true
printfn "A string %s, and something generic %A" "hello" [1;2;3;4]

// all complex types have pretty printing built in
printfn "twoTuple=%A,\nPerson=%A,\nTemp=%A,\nEmployee=%A"
         twoTuple person1 temp worker

// There are also sprintf/sprintfn functions for formatting data
// into a string, similar to String.Format.
```

然后，让我们从比较一些简单的F#代码和等效的C#代码开始。

# 3 F#与C#的比较：一个简单的和

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/fvsc-sum-of-squares/#series-toc)*)*

其中，我们试图在不使用循环的情况下对1到N的平方进行求和
03 Apr 2012 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/fvsc-sum-of-squares/

为了了解一些真正的F#代码是什么样子的，让我们从一个简单的问题开始：“将1到N的平方和”。

我们将比较F#实现和C#实现。首先，F#代码：

```F#
// define the square function
let square x = x * x

// define the sumOfSquares function
let sumOfSquares n =
   [1..n] |> List.map square |> List.sum

// try it
sumOfSquares 100
```

神秘的外观 `|>` 被称为管道操作员。它只是将一个表达式的输出传输到下一个表达式。所以 `sumOfSquares` 的代码如下：

1. 创建一个1到n的列表（方括号构成一个列表）。
2. 将列表导入名为list.map的库函数，使用我们刚才定义的“square”函数将输入列表转换为输出列表。
3. 将得到的方块列表通过管道传输到名为 `List.sum` 的库函数中。你能猜到它是干什么的吗？
4. 没有明确的“return”语句。`List.sum` 的输出是函数的总体结果。

接下来，这是一个使用基于C语言的经典（非函数式）风格的C#实现。（稍后将讨论使用LINQ的功能更强大的版本。）

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

有什么区别？

- F# 代码更紧凑
- F# 代码没有任何类型声明
- F# 可以交互式开发

让我们逐一进行。

## 更少的代码

最明显的区别是 C# 代码要多得多。13 行 C# 代码，而 3 行 F# 代码（忽略注释）。C# 代码有很多“噪音”，比如花括号、分号等。在C#中，函数不能独立存在，需要添加到某个类（“SumOfSquaresHelper”）中。F# 使用空格而不是括号，不需要行终止符，函数可以独立存在。

在F#中，整个函数通常写在一行上，就像“square”函数一样。`sumOfSquares` 函数也可以写在一行都上。在 C#中，这通常被认为是不好的做法。

当一个函数确实有多行时，F# 使用缩进来表示一段代码，从而消除了对大括号的需要。（如果你曾经使用过Python，这是同样的想法）。所以 `sumOfSquares` 函数也可以这样编写：

```F#
let sumOfSquares n =
   [1..n]
   |> List.map square
   |> List.sum
```

唯一的缺点是你必须仔细缩进代码。就我个人而言，我认为这是值得的。

## 无类型声明

下一个区别是 C# 代码必须显式声明所使用的所有类型。例如，`int i` 参数和 `int SumOfSquares` 返回类型。是的，C# 确实允许您在许多地方使用“var”关键字，但不适用于函数的参数和返回类型。

在 F# 代码中，我们根本没有声明任何类型。这一点很重要：F# 看起来像一种非类型化语言，但实际上它和 C# 一样是类型安全的，事实上，甚至更是如此！F# 使用一种称为“类型推断”的技术，从上下文中推断出您正在使用的类型。它在大多数情况下工作得非常好，极大地降低了代码的复杂性。

在这种情况下，类型推理算法注意到我们从整数列表开始。这反过来意味着平方函数和求和函数也必须接受整数，并且最终值必须是整数。您可以通过查看交互式窗口中的编译结果来查看推断的类型。您将看到类似以下内容：

```F#
val square : int -> int
```

这意味着“square”函数接受一个int并返回一个int。

如果原始列表使用了浮点数，类型推理系统就会推断出平方函数使用了浮点。试试看：

```F#
// define the square function
let squareF x = x * x

// define the sumOfSquares function
let sumOfSquaresF n =
   [1.0 .. n] |> List.map squareF |> List.sum  // "1.0" is a float

sumOfSquaresF 100.0
```

类型检查非常严格！如果您在原始 `sumOfSquares` 示例中尝试使用浮点数列表（`[1.0..n]`），或在 `sumOfSquaresF` 示例中使用整数列表（`[1..n]`），你会从编译器获得一个类型错误。

## 互动式开发

最后，F# 有一个交互式窗口，您可以在其中立即测试代码并使用它。在 C# 中，没有简单的方法可以做到这一点。

例如，我可以编写我的平方函数并立即对其进行测试：

```F#
// define the square function
let square x = x * x

// test
let s2 = square 2
let s3 = square 3
let s4 = square 4
```

当我确信它有效时，我可以继续下一段代码。

这种交互性鼓励了一种渐进式的编码方法，这种方法可能会让人上瘾！

此外，许多人声称，交互式设计代码会强制执行良好的设计实践，如解耦和显式依赖关系，因此，适合交互式评估的代码也将是易于测试的代码。相反，不能交互式测试的代码可能也很难测试。

## 重新审视 C# 代码

我最初的示例是使用“旧式”C# 编写的。C# 包含了许多功能特性，可以使用 LINQ 扩展以更紧凑的方式重写示例。

这是另一个 C# 版本——F# 代码的逐行翻译。

```c#
public static class FunctionalSumOfSquaresHelper
{
   public static int SumOfSquares(int n)
   {
      return Enumerable.Range(1, n)
         .Select(i => i * i)
         .Sum();
   }
}
```

然而，除了花括号、句点和分号的噪音外，C# 版本还需要声明参数和返回类型，这与 F# 版本不同。

许多 C# 开发人员可能会发现这是一个微不足道的例子，但当逻辑变得更加复杂时，他们仍然会求助于循环。然而，在 F# 中，你几乎永远不会看到这样的显式循环。例如，请参阅这篇关于从更复杂的循环中消除样板的文章。

# 4 F#与C#的比较：排序

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/fvsc-quicksort/#series-toc)*)*

其中我们看到F#比C#更具声明性，我们介绍了模式匹配。
04 Apr 2012 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/fvsc-quicksort/

在下一个例子中，我们将实现一个类似快速排序的算法来对列表进行排序，并将F#实现与C#实现进行比较。

以下是一个简化的快速排序算法的逻辑：

```
如果列表为空，则无需执行任何操作。
否则：
    1.取列表的第一个元素
    2.查找列表其余部分中的小于第一个元素的所有元素，并对其进行排序。
    3.查找列表其余部分中的大于等于第一个元素的所有元素，并对其进行排序
    4.将三个部分结合在一起，得到最终结果：
    （排序较小的元素+firstElement+排序较大的元素）
```

请注意，这是一个简化的算法，没有经过优化（也没有像真正的快速排序那样就地排序）；我们现在想专注于清晰度。

以下是F#中的代码：

```F#
let rec quicksort list =
   match list with
   | [] ->                            // If the list is empty
        []                            // return an empty list
   | firstElem::otherElements ->      // If the list is not empty
        let smallerElements =         // extract the smaller ones
            otherElements
            |> List.filter (fun e -> e < firstElem)
            |> quicksort              // and sort them
        let largerElements =          // extract the large ones
            otherElements
            |> List.filter (fun e -> e >= firstElem)
            |> quicksort              // and sort them
        // Combine the 3 parts into a new list and return it
        List.concat [smallerElements; [firstElem]; largerElements]

//test
printfn "%A" (quicksort [1;5;23;18;9;1;3])
```

再次注意，这不是一个优化的实现，而是为了紧密地反映算法而设计的。

让我们来看看这段代码：

- 任何地方都没有类型声明。此函数适用于任何具有可比项的列表（几乎所有 F# 类型，因为它们自动具有默认比较函数）。
- 整个函数是递归的——这是使用“`let rec quicksort list=`”中的 `rec` 关键字向编译器发出的信号。
- `match..with` 有点像 switch/case 语句。每个要测试的分支都用一个垂直条发出信号，如下所示：

```F#
match x with
| caseA -> something
| caseB -> somethingElse
```

- 带有 `[]` 的 “`match`” 匹配空列表，并返回空列表。
- 带有 `firstElem::otherElements` 的“`match`”有两个作用。
  - 首先，它只匹配非空列表。
  - 其次，它会自动创建两个新值。一个用于第一个元素，称为“`firstElem`”，另一个用于列表的其余部分，称为 `otherElements`。用 C# 的术语来说，这就像有一个“switch”语句，它不仅分支，而且同时进行变量声明和赋值。
- `->` 有点像 C# 中的 lambda（`=>`）。等效的 C# lambda 看起来像 `(firstElem，otherElements) => do something`。
- “`smallerElements`”部分获取列表的其余部分，使用带有“`<`”运算符的内联 lambda 表达式对第一个元素进行过滤，然后将结果递归地传输到快速排序函数中。
- “`largerElements`”行执行相同的操作，除了使用“`>=`”运算符
- 最后，使用列表连接函数“`List.concat`”构建结果列表。为此，需要将第一个元素放入列表中，这就是方括号的作用。
- 再次注意，没有“return”关键字；将返回最后一个值。在“`[]`”分支中，返回值是空列表，在main分支中，它是新构造的列表。

这里有一个老式的C#实现（不使用LINQ）进行比较。

```c#
public class QuickSortHelper
{
   public static List<T> QuickSort<T>(List<T> values)
      where T : IComparable
   {
      if (values.Count == 0)
      {
         return new List<T>();
      }

      //get the first element
      T firstElement = values[0];

      //get the smaller and larger elements
      var smallerElements = new List<T>();
      var largerElements = new List<T>();
      for (int i = 1; i < values.Count; i++)  // i starts at 1
      {                                       // not 0!
         var elem = values[i];
         if (elem.CompareTo(firstElement) < 0)
         {
            smallerElements.Add(elem);
         }
         else
         {
            largerElements.Add(elem);
         }
      }

      //return the result
      var result = new List<T>();
      result.AddRange(QuickSort(smallerElements.ToList()));
      result.Add(firstElement);
      result.AddRange(QuickSort(largerElements.ToList()));
      return result;
   }
}
```

比较这两组代码，我们可以再次看到F#代码更紧凑，噪音更少，不需要类型声明。

此外，F#代码的读取几乎与实际算法完全相同，与C#代码不同。这是F#的另一个关键优势——与C#相比，代码通常更具声明性（“做什么”），命令性（“如何做”）更低，因此更具自文档性。

## C# 中的函数式实现

下面是一个使用LINQ和扩展方法的更现代的“函数式”实现：

```c#
public static class QuickSortExtension
{
    /// <summary>
    /// Implement as an extension method for IEnumerable
    /// </summary>
    public static IEnumerable<T> QuickSort<T>(
        this IEnumerable<T> values) where T : IComparable
    {
        if (values == null || !values.Any())
        {
            return new List<T>();
        }

        //split the list into the first element and the rest
        var firstElement = values.First();
        var rest = values.Skip(1);

        //get the smaller and larger elements
        var smallerElements = rest
                .Where(i => i.CompareTo(firstElement) < 0)
                .QuickSort();

        var largerElements = rest
                .Where(i => i.CompareTo(firstElement) >= 0)
                .QuickSort();

        //return the result
        return smallerElements
            .Concat(new List<T>{firstElement})
            .Concat(largerElements);
    }
}
```

这要干净得多，读起来几乎与F#版本相同。但不幸的是，无法避免函数签名中的额外噪声。

## 正确性

最后，这种紧凑性的一个有益的副作用是 F# 代码通常在第一次工作时就可以了，而 C# 代码可能需要更多的调试。

事实上，在对这些示例进行编码时，旧式的 C# 代码最初是不正确的，需要一些调试才能使其正确。特别棘手的领域是 `for` 循环（从 1 开始，不是零）和 `CompareTo` 比较（我绕错了方向），而且很容易意外修改入站列表。第二个 C# 示例中的函数式风格不仅更简洁，而且更容易正确编码。

但与 F# 版本相比，即使是函数式 C# 版本也有缺点。例如，由于 F# 使用模式匹配，因此无法使用空列表分支到“非空列表”情况。另一方面，在 C# 代码中，如果我们忘记了测试：

```F#
if (values == null || !values.Any()) ...
```

然后提取第一元素：

```c#
var firstElement = values.First();
```

将失败，但有一个例外。编译器无法为您强制执行此操作。在您自己的代码中，您使用 `FirstOrDefault` 而不是 `First` 的频率是多少，因为您正在编写“防御性”代码。下面是一个在C#中很常见但在F#中很少见的代码模式示例：

```c#
var item = values.FirstOrDefault();  // instead of .First()
if (item != null)
{
   // do something if item is valid
}
```

F#中的一步式“模式匹配和分支”允许您在许多情况下避免这种情况。

## 后记

按照 F# 标准，上面 F# 中的示例实现实际上非常冗长！

为了好玩，以下是一个更典型的简洁版本：

```F#
let rec quicksort2 = function
   | [] -> []
   | first::rest ->
        let smaller,larger = List.partition ((>=) first) rest
        List.concat [quicksort2 smaller; [first]; quicksort2 larger]

// test code
printfn "%A" (quicksort2 [1;5;23;18;9;1;3])
```

对于4行代码来说还不错，当你习惯了语法后，仍然非常可读。

# 5 F#与C#的比较：下载网页

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/fvsc-download/#series-toc)*)*

其中我们看到F#擅长回调，我们被介绍给了“use”关键字
05 Apr 2012 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/fvsc-download/

在这个例子中，我们将比较下载网页的F#和C#代码，以及处理文本流的回调。

我们将从一个简单的F#实现开始。

```F#
// "open" brings a .NET namespace into visibility
open System.Net
open System
open System.IO

// Fetch the contents of a web page
let fetchUrl callback url =
    let req = WebRequest.Create(Uri(url))
    use resp = req.GetResponse()
    use stream = resp.GetResponseStream()
    use reader = new IO.StreamReader(stream)
    callback reader url
```

让我们来看看这段代码：

- 在顶部使用“open”允许我们编写“WebRequest”而不是“System.Net.Webequest”。它类似于 C# 中的“`using System.Net`”标头。
- 接下来，我们定义 `fetchUrl` 函数，它接受两个参数，一个用于处理流的回调和一个用于获取的 url。
- 接下来，我们将 url 字符串包装在 Uri中。F# 具有严格的类型检查，因此如果我们编写了：`let req=WebRequest.Create(url)` 编译器会抱怨它不知道 `WebRequest.Create` 的版本以供使用。
- 在声明 `response`、`stream` 和 `reader` 值时，使用“`use`”关键字而不是“`let`”。这只能与实现 `IDisposable` 的类结合使用。它告诉编译器在资源超出范围时自动处理它。这相当于 C# 的“`using`”关键字。
- 最后一行使用 StreamReader 和 url 作为参数调用回调函数。请注意，回调的类型不必在任何地方指定。

下面是等效的 C# 实现。

```c#
class WebPageDownloader
{
    public TResult FetchUrl<TResult>(
        string url,
        Func<string, StreamReader, TResult> callback)
    {
        var req = WebRequest.Create(url);
        using (var resp = req.GetResponse())
        {
            using (var stream = resp.GetResponseStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    return callback(url, reader);
                }
            }
        }
    }
}
```

像往常一样，C#版本有更多的“噪音”。

- 花括号有10行，嵌套有5个层次的视觉复杂性*
- 所有参数类型都必须显式声明，泛型 `TResult` 类型必须重复三次。

*确实，在这个特定的例子中，当所有 `using` 语句都相邻时，可以删除额外的大括号和缩进，但在更一般的情况下，它们是必需的。

## 测试代码

回到F#世界，我们现在可以交互式地测试代码：

```F#
let myCallback (reader:IO.StreamReader) url =
    let html = reader.ReadToEnd()
    let html1000 = html.Substring(0,1000)
    printfn "Downloaded %s. First 1000 is %s" url html1000
    html      // return all the html

//test
let google = fetchUrl myCallback "http://google.com"
```

最后，我们必须为 reader 参数（`reader:IO.StreamReader`）使用类型声明。这是必需的，因为 F# 编译器无法自动确定“reader”参数的类型。

F# 的一个非常有用的特性是，你可以在函数中“烘焙”参数，这样就不必每次都传入它们。这就是为什么 `url` 参数放在最后而不是第一个的原因，就像 C# 版本一样。回调可以设置一次，而 url 因调用而异。

```F#
// build a function with the callback "baked in"
let fetchUrl2 = fetchUrl myCallback

// test
let google = fetchUrl2 "http://www.google.com"
let bbc    = fetchUrl2 "http://news.bbc.co.uk"

// test with a list of sites
let sites = ["http://www.bing.com";
             "http://www.google.com";
             "http://www.yahoo.com"]

// process each site in the list
sites |> List.map fetchUrl2
```

最后一行（使用 `List.map`）显示了如何将新函数与列表处理函数结合使用，以便一次下载整个列表。

以下是等效的 C# 测试代码：

```c#
[Test]
public void TestFetchUrlWithCallback()
{
    Func<string, StreamReader, string> myCallback = (url, reader) =>
    {
        var html = reader.ReadToEnd();
        var html1000 = html.Substring(0, 1000);
        Console.WriteLine(
            "Downloaded {0}. First 1000 is {1}", url,
            html1000);
        return html;
    };

    var downloader = new WebPageDownloader();
    var google = downloader.FetchUrl("http://www.google.com",
                                      myCallback);

    // test with a list of sites
    var sites = new List<string> {
        "http://www.bing.com",
        "http://www.google.com",
        "http://www.yahoo.com"};

    // process each site in the list
    sites.ForEach(site => downloader.FetchUrl(site, myCallback));
}
```

同样，代码比 F# 代码有点嘈杂，有很多显式类型引用。更重要的是，C# 代码不容易让你在函数中烘焙一些参数，所以每次都必须显式引用回调。



# 6 四个关键概念

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/key-concepts/#series-toc)*)*

区分F#与标准命令式语言的概念
06 Apr 2012 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/key-concepts/

在接下来的几篇文章中，我们将继续演示本系列的主题：简洁性、便利性、正确性、并发性和完整性。

但在此之前，让我们来看看 F# 中的一些关键概念，我们将一次又一次地遇到这些概念。F# 在很多方面与 C# 等标准命令式语言不同，但有一些主要的区别尤其需要理解：

- **面向函数**而非面向对象
- **表达式**而非语句（statements）
- 用于创建域模型的**代数类型**
- 控制流的**模式匹配**

在以后的文章中，我们将更深入地讨论这些问题——这只是一个品酒师，可以帮助你理解本系列的其余部分。

四个关键概念

## 面向函数而非面向对象

正如您对“函数式编程”一词的期望，函数在F#中无处不在。

当然，函数是第一类实体，可以像任何其他值一样传递：

```F#
let square x = x * x

// functions as values
let squareclone = square
let result = [1..10] |> List.map squareclone

// functions taking other functions as parameters
let execFunction aFunc aParam = aFunc aParam
let result2 = execFunction square 12
```

但是C#也有一级函数，那么函数式编程有什么特别之处呢？

简而言之，F# 的面向函数特性渗透到语言和类型系统的每一个部分，这是 C# 所没有的，因此 C# 中笨重或笨拙的东西在 F# 中非常优雅。

很难用几段话来解释这一点，但以下是我们将在这一系列帖子中看到的一些好处：

- **通过组合构建**。组合是一种“粘合剂”，它使我们能够从较小的系统构建更大的系统。这不是一种可选技术，而是功能风格的核心。几乎每一行代码都是一个可组合的表达式（见下文）。组合用于构建基本函数，然后构建使用这些函数的函数，以此类推。组合原理不仅适用于函数，也适用于类型（下文讨论的乘积和求和类型）。
- **分解和重构**。将问题分解为各个部分的能力取决于这些部分粘合在一起的难易程度。在命令式语言中看似不可分割的方法和类，在函数式设计中往往可以分解成令人惊讶的小块。这些细粒度组件通常由（a）一些非常通用的函数组成，这些函数将其他函数作为参数，以及（b）其他辅助函数，这些辅助函数专门用于特定数据结构或应用程序的一般情况。一旦被分解，广义函数允许非常容易地编程许多额外的操作，而无需编写新代码。在关于从循环中提取重复代码的文章中，您可以看到这样一个通用函数（fold 函数）的一个很好的例子。
- **好设计**。许多良好设计的原则，如“关注点分离”、“单一责任原则”、“程序到接口，而不是实现”，都是函数式方法的自然结果。函数式代码通常具有高级和声明性。

本系列的以下文章将提供函数如何使代码更简洁方便的示例，然后为了更深入地理解，有一个关于函数思维的完整系列。

## 表达式而非语句

在函数式语言中，没有语句，只有表达式。也就是说，每个代码块总是返回一个值，较大的代码块是通过使用组合而不是序列化语句列表组合较小的代码块而创建的。

如果你使用过 LINQ 或 SQL，那么你已经熟悉了基于表达式的语言。例如，在纯 SQL 中，不能有赋值。相反，您必须在较大的查询中包含子查询。

```sql
SELECT EmployeeName
FROM Employees
WHERE EmployeeID IN
	(SELECT DISTINCT ManagerID FROM Employees)  -- subquery
```

F# 的工作方式是一样的——每个函数定义都是一个表达式，而不是一组语句。

这可能并不明显，但从表达式构建的代码比使用语句更安全、更简洁。为了了解这一点，让我们将 C# 中一些基于语句的代码与等效的基于表达式的代码进行比较。

首先，基于语句的代码。语句不返回值，因此您必须使用从语句体中分配的临时变量。

```c#
// statement-based code in C#
int result;
if (aBool)
{
  result = 42;
}
Console.WriteLine("result={0}", result);
```

因为 `if-then` 块是一个语句，所以 `result` 变量必须在语句外部定义，但从语句内部分配，这会导致以下问题：

- `result` 应该设置为什么初始值？
- 如果我忘记给 `result` 变量赋值怎么办？
- 在“else”情况下，`result` 变量的值是什么？

为了进行比较，这是以面向表达式的风格重写的相同代码：

```F#
// expression-based code in C#
int result = (aBool) ? 42 : 0;
Console.WriteLine("result={0}", result);
```

在面向表达式的版本中，这些问题都不适用：

- `result` 变量在赋值的同时被声明。不必在表达式“外部”设置任何变量，也不必担心它们应该设置为什么初始值。
- “else”被明确处理。不可能忘记在其中一个分支机构做作业。
- 不可能忘记赋值 `result`，因为这样变量就不存在了！

在 F# 中，面向表达式的风格不是一种选择，当来自命令式背景时，这是需要改变方法的事情之一。

## 代数类型

F# 中的类型系统基于**代数类型**的概念。也就是说，通过以两种不同的方式组合现有类型来构建新的复合类型：

- 首先，值的组合，每个值都是从一组类型中挑选出来的。这些被称为“乘积”类型。
- 或者，作为一个不相交的联合，代表一组类型之间的选择。这些被称为“求和”类型。

例如，给定现有类型 `int` 和 `bool`，我们可以创建一个新的产品类型，该类型必须各有一个：

```F#
//declare it
type IntAndBool = {intPart: int; boolPart: bool}

//use it
let x = {intPart=1; boolPart=false}
```

或者，我们可以创建一个新的并集/求和类型，可以在每种类型之间进行选择：

```F#
//declare it
type IntOrBool =
  | IntChoice of int
  | BoolChoice of bool

//use it
let y = IntChoice 42
let z = BoolChoice true
```

这些“选择”类型在 C# 中不可用，但对于建模许多现实世界的情况非常有用，例如状态机中的状态（这在许多领域是一个令人惊讶的常见主题）。

通过以这种方式组合“乘积”和“总和”类型，可以很容易地创建一组丰富的类型，准确地对任何业务领域进行建模。有关此操作的示例，请参阅有关低开销类型定义和使用类型系统确保正确代码的帖子。

## 控制流的模式匹配

大多数命令式语言都为分支和循环提供了各种控制流语句：

- `if-then-else`（以及三元版本 `bool ? if-true : if-false`）
- `case` 或 `switch` 语句
- `for` 和 `foreach` 循环，带有 `break` 和 `continue`
- `while` 和 `until` 循环
- 甚至可怕的 `goto`

F# 确实支持其中一些，但 F# 也支持最通用的条件表达式形式，即**模式匹配**。

替换 `if-then-else` 的典型匹配表达式如下：

```F#
match booleanExpression with
| true -> // true branch
| false -> // false branch
```

`switch` 的替换可能看起来像这样：

```F#
match aDigit with
| 1 -> // Case when digit=1
| 2 -> // Case when digit=2
| _ -> // Case otherwise
```

最后，循环通常使用递归完成，通常看起来像这样：

```F#
match aList with
| [] ->
     // Empty case
| first::rest ->
     // 至少有一个元素的案例。
     // 处理第一个元素，然后调用
     // 与列表的其余部分递归
```

虽然匹配表达式一开始看起来不必要地复杂，但在实践中，你会发现它既优雅又强大。

有关模式匹配的好处，请参阅关于穷举模式匹配的文章，有关大量使用模式匹配的工作示例，请参阅罗马数字示例。

## 与联合类型的模式匹配

我们上面提到过F#支持“union”或“choice”类型。这是用来代替继承来处理底层类型的不同变体的。模式匹配与这些类型无缝协作，为每个选择创建控制流。

在下面的示例中，我们创建了一个表示四种不同形状的 `Shape` 类型，然后为每种形状定义了一个具有不同行为的 `draw` 函数。这类似于面向对象语言中的多态性，但基于函数。

```F#
type Shape =        // define a "union" of alternative structures
    | Circle of radius:int
    | Rectangle of height:int * width:int
    | Point of x:int * y:int
    | Polygon of pointList:(int * int) list

let draw shape =    // define a function "draw" with a shape param
  match shape with
  | Circle radius ->
      printfn "The circle has a radius of %d" radius
  | Rectangle (height,width) ->
      printfn "The rectangle is %d high by %d wide" height width
  | Polygon points ->
      printfn "The polygon is made of these points %A" points
  | _ -> printfn "I don't recognize this shape"

let circle = Circle(10)
let rect = Rectangle(4,5)
let point = Point(2,3)
let polygon = Polygon( [(1,1); (2,2); (3,3)])

[circle; rect; polygon; point] |> List.iter draw
```

有几件事需要注意：

- 像往常一样，我们不必指定任何类型。编译器正确地确定了“draw”函数的形状参数的类型为 `Shape`。
- `Polygon` 案例定义中的 `int*int` 是一个元组，一对 int。如果你想知道为什么类型是“乘法”的，请参阅这篇关于元组的文章。
- 你可以看到这 `match..with` 逻辑不仅与形状的内部结构相匹配，而且根据适合形状的内容分配值。
- 下划线类似于 switch 语句中的“默认”分支，除了在 F# 中是必需的——必须始终处理所有可能的情况。如果你注释掉这句话

```F#
 | _ -> printfn "I don't recognize this shape"
```

看看编译时会发生什么！

在 C# 中，可以通过使用子类或接口来模拟这些类型的选择，但 C# 类型系统中没有内置对这种穷举匹配和错误检查的支持。

## 行为导向设计与数据导向设计

你可能想知道这种模式匹配是不是一个好主意？在面向对象的设计中，检查特定类是一种反模式，因为你应该只关心行为，而不是实现它的类。

但在纯功能设计中，没有对象也没有行为。有函数，也有“哑（dumb）”的数据类型。数据类型没有任何与之相关的行为，函数也不包含数据——它们只是将数据类型转换为其他数据类型。

在这种情况下，`Circle` 和 `Rectangle` 实际上不是类型。唯一的类型是 `Shape`——一种选择，一种可区分联合——这些都是这种类型的各种情况。（更多关于可区分联合在这里）。

为了使用 `Shape` 类型，函数需要处理 `Shape` 的每种情况，这是通过模式匹配完成的。



# 7 简洁

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/conciseness-intro/#series-toc)*)*

为什么简洁很重要？
07 Apr 2012 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/conciseness-intro/

在看过一些简单的代码后，我们现在将继续演示主要主题（简洁性、便利性、正确性、并发性和完整性），并通过类型、函数和模式匹配的概念进行筛选。

在接下来的几篇文章中，我们将研究 F# 的简洁性和可读性。

大多数主流编程语言的一个重要目标是可读性和简洁性的良好平衡。过多的简洁可能会导致代码难以理解或混淆（APL 有人吗？），而过多的冗长则很容易淹没其潜在含义。理想情况下，我们希望有一个高信噪比，代码中的每个单词和字符都有助于代码的含义，并且样板最少。

为什么简洁很重要？以下是几个原因：

- **简洁的语言往往更具声明性**，说明代码应该做什么，而不是如何做。也就是说，声明性代码更关注高级逻辑，而不是实现的具体细节。
- 如果要推理的代码行数更少，就**更容易推理正确性**！
- 当然，**您一次可以在屏幕上看到更多代码**。这可能看起来微不足道，但你看到的越多，你掌握的也就越多。

如您所见，与C#相比，F#通常更简洁。这是由于以下功能：

- **类型推断**和**低开销类型定义**。F# 简洁易读的主要原因之一是它的类型系统。F# 使得根据需要创建新类型变得非常容易。它们在定义或使用中都不会造成视觉混乱，类型推理系统意味着您可以自由使用它们，而不会被复杂的类型语法分心。
- **使用函数提取样板代码**。DRY 原则（“不要重复自己”）是函数式语言和面向对象语言中良好设计的核心原则。在F#中，将重复代码提取到常见的实用函数中非常容易，这使您可以专注于重要的事情。
- **从简单函数编写复杂代码**并**创建迷你语言**。函数式方法使创建一组基本操作变得容易，然后以各种方式组合这些构建块以构建更复杂的行为。这样，即使是最复杂的代码也仍然非常简洁易读。
- **图案匹配**。我们已经将模式匹配视为一种美化的开关语句，但事实上它更通用，因为它可以以多种方式比较表达式，在值、条件和类型上进行匹配，然后同时分配或提取值。

# 8 类型推断

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/conciseness-type-inference/#series-toc)*)*

如何避免被复杂的类型语法分心
08 Apr 2012 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/conciseness-type-inference/

正如您已经看到的，F# 使用了一种称为“类型推理”的技术，大大减少了需要在正常代码中显式指定的类型注释的数量。即使确实需要指定类型，与 C# 相比，语法也不那么冗长。

为了理解这一点，这里有一些 C# 方法，它们封装了两个标准的 LINQ 函数。实现很简单，但方法签名非常复杂：

```c#
public IEnumerable<TSource> Where<TSource>(
    IEnumerable<TSource> source,
    Func<TSource, bool> predicate
    )
{
    //use the standard LINQ implementation
    return source.Where(predicate);
}

public IEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(
    IEnumerable<TSource> source,
    Func<TSource, TKey> keySelector
    )
{
    //use the standard LINQ implementation
    return source.GroupBy(keySelector);
}
```

这里是 F# 的确切等价物，表明根本不需要类型注释！

```F#
let Where source predicate =
    //use the standard F# implementation
    Seq.filter predicate source

let GroupBy source keySelector =
    //use the standard F# implementation
    Seq.groupBy keySelector source
```

> 您可能会注意到，“filter”和“groupBy”的标准 F# 实现的参数顺序与 C# 中使用的 LINQ 实现完全相反。“source”参数放在最后，而不是第一个。这是有原因的，这将在函数式思维系列中解释。

类型推理算法在从多个来源收集信息以确定类型方面表现出色。在下面的示例中，它正确地推断出 `list` 值是字符串列表。

```F#
let i = 1
let s = "hello"
let tuple = s,i       // pack into tuple
let s2,i2 = tuple     // unpack
let list = [s2]       // type is string list
```

在这个例子中，它正确地推断出 `sumLengths` 函数接受一个字符串列表并返回一个 int。

```F#
let sumLengths strList =
    strList |> List.map String.length |> List.sum

// function type is: string list -> int
```

# 9 低开销类型定义

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/conciseness-type-definitions/#series-toc)*)*

制造新类型不受处罚
09 Apr 2012 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/conciseness-type-definitions/

在 C# 中，创建新类型有一个抑制因素——缺乏类型推理意味着你需要在大多数地方显式指定类型，从而导致脆弱性和更多的视觉混乱。因此，人们总是倾向于创建巨大单个（monolithic）类，而不是将其模块化。

在 F# 中，创建新类型没有惩罚，因此拥有数百甚至数千个新类型是很常见的。每次需要定义结构时，都可以创建一个特殊类型，而不是重用（和重载）现有类型，如字符串和列表。

这意味着你的程序将更加类型安全、更加自文档化、更易于维护（因为当类型发生变化时，你会立即得到编译时错误，而不是运行时错误）。

以下是 F# 中单行类型的一些示例：

```F#
open System

// some "record" types
type Person = {FirstName:string; LastName:string; Dob:DateTime}
type Coord = {Lat:float; Long:float}

// some "union" (choice) types
type TimePeriod = Hour | Day | Week | Year
type Temperature = C of int | F of int
type Appointment =
  OneTime of DateTime | Recurring of DateTime list
```

## F#类型和领域驱动设计

F#中类型系统的简洁性在进行域驱动设计（DDD）时特别有用。在 DDD 中，对于每个真实世界的实体和值对象，理想情况下都希望有一个相应的类型。这可能意味着创建数百个“小”类型，这在 C# 中可能很乏味。

此外，DDD 中的“值”对象应该具有结构相等性，这意味着包含相同数据的两个对象应该始终相等。在 C# 中，这可能意味着重写 `IEquatable<T>` 会更加乏味，但在 F# 中，默认情况下你可以免费获得它。

为了展示在 F# 中创建 DDD 类型有多容易，这里有一些可能为简单的“客户”域创建的示例类型。

```F#
type PersonalName =
  {FirstName:string; LastName:string}

// Addresses
type StreetAddress = {
  Line1:string;
  Line2:string;
  Line3:string
  }

type ZipCode = ZipCode of string
type StateAbbrev = StateAbbrev of string
type ZipAndState =
  {State:StateAbbrev; Zip:ZipCode }
type USAddress =
  {Street:StreetAddress; Region:ZipAndState}

type UKPostCode = PostCode of string
type UKAddress =
  {Street:StreetAddress; Region:UKPostCode}

type InternationalAddress = {
  Street:StreetAddress;
  Region:string;
  CountryName:string
  }

// choice type -- must be one of these three specific types
type Address =
  | USAddress of USAddress
  | UKAddress of UKAddress
  | InternationalAddress of InternationalAddress

// Email
type Email = Email of string

// Phone
type CountryPrefix = Prefix of int
type Phone =
  {CountryPrefix:CountryPrefix; LocalNumber:string}

type Contact =
  {
  PersonalName: PersonalName;
  // "option" means it might be missing
  Address: Address option;
  Email: Email option;
  Phone: Phone option;
  }

// Put it all together into a CustomerAccount type
type CustomerAccountId = AccountId of string
type CustomerType  = Prospect | Active | Inactive

// override equality and deny comparison
[<CustomEquality; NoComparison>]
type CustomerAccount =
  {
  CustomerAccountId: CustomerAccountId;
  CustomerType: CustomerType;
  ContactInfo: Contact;
  }

  override this.Equals(other) =
    match other with
    | :? CustomerAccount as otherCust ->
      (this.CustomerAccountId = otherCust.CustomerAccountId)
    | _ -> false

  override this.GetHashCode() = hash this.CustomerAccountId
```

这个代码片段在短短几行中包含了 17 个类型定义，但复杂性很低。做同样的事情需要多少行 C# 代码？

显然，这是一个仅包含基本类型的简化版本——在真实系统中，会添加约束和其他方法。但请注意，创建大量DDD值对象是多么容易，尤其是字符串的包装类型，如“`ZipCode`”和“`Email`”。通过使用这些包装器类型，我们可以在创建时强制执行某些约束，并确保这些类型不会与正常代码中的无约束字符串混淆。唯一的“实体”类型是 `CustomerAccount`，它被明确表示为具有平等和比较的特殊待遇。

有关更深入的讨论，请参阅名为“F#中的领域驱动设计”的系列文章。

# 10 使用函数提取样板代码

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/conciseness-extracting-boilerplate/#series-toc)*)*

DRY 原理的功能方法
2012年4月10日 这篇文章已有3年多的历史了

https://fsharpforfunandprofit.com/posts/conciseness-extracting-boilerplate/

在本系列的第一个示例中，我们看到了一个计算平方和的简单函数，该函数在F#和C#中都实现了。现在，假设我们想要一些类似的新函数，例如：

- 计算N以下所有数字的乘积
- 将奇数之和计数到N
- 数字的交替和，直到N

显然，所有这些要求都是相似的，但您将如何提取任何共同的功能？

让我们先从C#中的一些简单实现开始：

```c#
public static int Product(int n)
{
    int product = 1;
    for (int i = 1; i <= n; i++)
    {
        product *= i;
    }
    return product;
}

public static int SumOfOdds(int n)
{
    int sum = 0;
    for (int i = 1; i <= n; i++)
    {
        if (i % 2 != 0) { sum += i; }
    }
    return sum;
}

public static int AlternatingSum(int n)
{
    int sum = 0;
    bool isNeg = true;
    for (int i = 1; i <= n; i++)
    {
        if (isNeg)
        {
            sum -= i;
            isNeg = false;
        }
        else
        {
            sum += i;
            isNeg = true;
        }
    }
    return sum;
}
```

所有这些实现有什么共同点？循环逻辑！作为程序员，我们被告知要记住DRY原则（“不要重复自己”），但在这里，我们每次都重复了几乎完全相同的循环逻辑。让我们看看能否提取出这三种方法之间的差异：

| 函数           | 初始值                        | 循环内逻辑                                                   |
| :------------- | :---------------------------- | :----------------------------------------------------------- |
| Product        | product=1                     | 将第i个值乘以运行总数                                        |
| SumOfOdds      | sum=0                         | 如果不是偶数，则将第i个值添加到运行总数中                    |
| AlternatingSum | int sum = 0 bool isNeg = true | 使用 isNeg 标志来决定是加还是减，并在下一次传递时翻转该标志。 |

有没有办法去掉重复的代码，只关注设置和内部循环逻辑？是的，有。以下是F#中相同的三个函数：

```F#
let product n =
    let initialValue = 1
    let action productSoFar x = productSoFar * x
    [1..n] |> List.fold action initialValue

//test
product 10

let sumOfOdds n =
    let initialValue = 0
    let action sumSoFar x = if x%2=0 then sumSoFar else sumSoFar+x
    [1..n] |> List.fold action initialValue

//test
sumOfOdds 10

let alternatingSum n =
    let initialValue = (true,0)
    let action (isNeg,sumSoFar) x = if isNeg then (false,sumSoFar-x)
                                             else (true ,sumSoFar+x)
    [1..n] |> List.fold action initialValue |> snd

//test
alternatingSum 100
```

这三个函数具有相同的模式：

- 设置初始值
- 设置一个动作函数，该函数将在循环内的每个元素上执行。
- 调用库函数 `List.fold`。这是一个功能强大的通用函数，它从初始值开始，然后依次为列表中的每个元素运行动作函数。

action 函数总是有两个参数：一个运行的 total（或state）和要操作的 list 元素（在上面的示例中称为“x”）。

在最后一个函数 `alternatingSum` 中，您会注意到它使用了一个元组（一对值）作为初始值和操作结果。这是因为运行总计和 `isNeg` 标志都必须传递给循环的下一次迭代——没有可以使用的“全局”值。折叠的最终结果也是一个元组，因此我们必须使用“snd”（second）函数来提取我们想要的最终总和。

通过使用 `List.fold` 并完全避免任何循环逻辑，F# 代码获得了许多好处：

- **强调并明确了关键程序逻辑**。功能之间的重要差异变得非常明显，而共性则被推到了后台。
- **样板循环代码已被消除**，因此代码比 C# 版本更简洁（4-5 行 F# 代码比至少 9 行 C# 代码）
- **循环逻辑中永远不会有错误**（例如超出一个（off-by-one）），因为该逻辑没有暴露给我们。

顺便说一句，平方和的例子也可以用 `fold` 来写：

```F#
let sumOfSquaresWithFold n =
    let initialValue = 0
    let action sumSoFar x = sumSoFar + (x*x)
    [1..n] |> List.fold action initialValue

//test
sumOfSquaresWithFold 100
```

## C# 中的“折叠”

你能在 C# 中使用“折叠”方法吗？对。LINQ 确实有一个相当于 `fold` 的东西，称为 `Aggregate`。以下是为使用它而重写的 C# 代码：

```c#
public static int ProductWithAggregate(int n)
{
    var initialValue = 1;
    Func<int, int, int> action = (productSoFar, x) =>
        productSoFar * x;
    return Enumerable.Range(1, n)
            .Aggregate(initialValue, action);
}

public static int SumOfOddsWithAggregate(int n)
{
    var initialValue = 0;
    Func<int, int, int> action = (sumSoFar, x) =>
        (x % 2 == 0) ? sumSoFar : sumSoFar + x;
    return Enumerable.Range(1, n)
        .Aggregate(initialValue, action);
}

public static int AlternatingSumsWithAggregate(int n)
{
    var initialValue = Tuple.Create(true, 0);
    Func<Tuple<bool, int>, int, Tuple<bool, int>> action =
        (t, x) => t.Item1
            ? Tuple.Create(false, t.Item2 - x)
            : Tuple.Create(true, t.Item2 + x);
    return Enumerable.Range(1, n)
        .Aggregate(initialValue, action)
        .Item2;
}
```

好吧，从某种意义上说，这些实现比原始的 C# 版本更简单、更安全，但泛型类型的所有额外噪声使这种方法比 F# 中的等效代码要优雅得多。你可以看到为什么大多数 C# 程序员更喜欢坚持使用显式循环。

## 一个更相关的例子

在现实世界中经常出现的一个稍微相关的例子是，当元素是类或结构时，如何获得列表的“最大”元素。LINQ 方法“max”只返回最大值，而不是包含最大值的整个元素。

这是一个使用显式循环的解决方案：

```c#
public class NameAndSize
{
    public string Name;
    public int Size;
}

public static NameAndSize MaxNameAndSize(IList<NameAndSize> list)
{
    if (list.Count() == 0)
    {
        return default(NameAndSize);
    }

    var maxSoFar = list[0];
    foreach (var item in list)
    {
        if (item.Size > maxSoFar.Size)
        {
            maxSoFar = item;
        }
    }
    return maxSoFar;
}
```

在 LINQ 中这样做似乎很难高效地完成（即一次完成），并且已经成为 Stack Overflow 问题。Jon Skeet 甚至为此写了一篇文章。

再次，fold 挺身而出！

以下是使用 `Aggregate` 的 C# 代码：

```c#
public class NameAndSize
{
    public string Name;
    public int Size;
}

public static NameAndSize MaxNameAndSize(IList<NameAndSize> list)
{
    if (!list.Any())
    {
        return default(NameAndSize);
    }

    var initialValue = list[0];
    Func<NameAndSize, NameAndSize, NameAndSize> action =
        (maxSoFar, x) => x.Size > maxSoFar.Size ? x : maxSoFar;
    return list.Aggregate(initialValue, action);
}
```

请注意，此 C# 版本为空列表返回 null。这似乎很危险，那么应该怎么办呢？抛出异常？这似乎也不对。

以下是使用 fold 的 F# 代码：

```F#
type NameAndSize= {Name:string;Size:int}

let maxNameAndSize list =

    let innerMaxNameAndSize initialValue rest =
        let action maxSoFar x = if maxSoFar.Size < x.Size then x else maxSoFar
        rest |> List.fold action initialValue

    // handle empty lists
    match list with
    | [] ->
        None
    | first::rest ->
        let max = innerMaxNameAndSize first rest
        Some max
```

F# 代码有两部分：

- `innerMaxNameAndSize` 函数与我们之前看到的类似。
- 第二个位 `match list with` 根据列表是否为空进行分支。对于空列表，它返回 `None`，在非空的情况下，它返回 `Some`。这样做可以保证函数的调用者必须处理这两种情况。

还有一个测试：

```F#
//test
let list = [
    {Name="Alice"; Size=10}
    {Name="Bob"; Size=1}
    {Name="Carol"; Size=12}
    {Name="David"; Size=5}
    ]
maxNameAndSize list
maxNameAndSize []
```

实际上，我根本不需要写这个，因为 F# 已经有一个 `maxBy` 函数了！

```F#
// use the built in function
list |> List.maxBy (fun item -> item.Size)
[] |> List.maxBy (fun item -> item.Size)
```

但正如你所看到的，它不能很好地处理空列表。这是一个安全包装 `maxBy` 的版本。

```F#
let maxNameAndSize list =
    match list with
    | [] ->
        None
    | _ ->
        let max = list |> List.maxBy (fun item -> item.Size)
        Some max
```

# 11 将函数用作构建块

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/conciseness-functions-as-building-blocks/#series-toc)*)*

函数组合和迷你语言使代码更具可读性
2012年4月11日 这篇文章已有3年多的历史了

https://fsharpforfunandprofit.com/posts/conciseness-functions-as-building-blocks/

一个众所周知的好设计原则是创建一组基本操作，然后以各种方式组合这些构建块，以构建更复杂的行为。在面向对象语言中，这一目标产生了许多实现方法，如“流畅接口”、“策略模式”、“装饰器模式”等。在F#中，它们都是通过函数组合以相同的方式完成的。

让我们从一个使用整数的简单示例开始。假设我们创建了一些基本函数来进行算术运算：

```F#
// building blocks
let add2 x = x + 2
let mult3 x = x * 3
let square x = x * x

// test
[1..10] |> List.map add2 |> printfn "%A"
[1..10] |> List.map mult3 |> printfn "%A"
[1..10] |> List.map square |> printfn "%A"
```

现在我们想创建基于这些的新函数：

```F#
// new composed functions
let add2ThenMult3 = add2 >> mult3
let mult3ThenSquare = mult3 >> square
```

“`>>`”运算符是组合运算符。它的意思是：执行第一个函数，然后执行第二个函数。

注意这种组合函数的方式是多么简洁。没有参数、类型或其他无关的噪音。

可以肯定的是，这些例子也可以写得不那么简洁，更明确：

```F#
let add2ThenMult3 x = mult3 (add2 x)
let mult3ThenSquare x = square (mult3 x)
```

但这种更明确的风格也有点混乱：

- 在显式样式中，必须添加x参数和括号，即使它们不会增加代码的含义。
- 在显式风格中，函数是按照应用顺序从前向后写的。在我的 `add2nMult3` 示例中，我想先加 2，然后相乘。`add2 >> mult3` 语法使其在视觉上比 `mult3(add2 x)`更清晰。

现在让我们测试这些组合：

```F#
// test
add2ThenMult3 5
mult3ThenSquare 5
[1..10] |> List.map add2ThenMult3 |> printfn "%A"
[1..10] |> List.map mult3ThenSquare |> printfn "%A"
```

## 扩展现有功能

现在假设我们想用一些日志行为来装饰这些现有的函数。我们也可以组合这些，以创建一个内置日志的新函数。

```F#
// helper functions;
let logMsg msg x = printf "%s%i" msg x; x     //without linefeed
let logMsgN msg x = printfn "%s%i" msg x; x   //with linefeed

// new composed function with new improved logging!
let mult3ThenSquareLogged =
   logMsg "before="
   >> mult3
   >> logMsg " after mult3="
   >> square
   >> logMsgN " result="

// test
mult3ThenSquareLogged 5
[1..10] |> List.map mult3ThenSquareLogged //apply to a whole list
```

我们的新函数 `mult3ThenSquareLogged` 的名字很难看，但它易于使用，很好地隐藏了其中函数的复杂性。你可以看到，如果你很好地定义了构建块函数，这种函数组合可以成为获得新功能的强大方式。

但是等等，还有更多！函数是 F# 中的第一类实体，任何其他F#代码都可以对其进行操作。下面是一个使用组合运算符将函数列表折叠为单个操作的示例。

```F#
let listOfFunctions = [
   mult3;
   square;
   add2;
   logMsgN "result=";
   ]

// compose all functions in the list into a single one
let allFunctions = List.reduce (>>) listOfFunctions

//test
allFunctions 5
```

## 迷你语言

领域特定语言（DSL）被公认为是一种创建更可读、更简洁代码的技术。功能方法非常适合这一点。

如果需要，你可以选择拥有一个完整的“外部”DSL，它有自己的词法分析器、解析器等，F#有各种工具集，使这变得非常简单。

但在许多情况下，更容易保持F#的语法，只需设计一组“动词”和“名词”来封装我们想要的行为。

简洁地创建新类型并与之匹配的能力使得快速设置流畅的界面变得非常容易。例如，这里有一个使用简单词汇表计算日期的小函数。请注意，仅为此函数定义了两种新的枚举样式类型。

```F#
// set up the vocabulary
type DateScale = Hour | Hours | Day | Days | Week | Weeks
type DateDirection = Ago | Hence

// define a function that matches on the vocabulary
let getDate interval scale direction =
    let absHours = match scale with
                   | Hour | Hours -> 1 * interval
                   | Day | Days -> 24 * interval
                   | Week | Weeks -> 24 * 7 * interval
    let signedHours = match direction with
                      | Ago -> -1 * absHours
                      | Hence ->  absHours
    System.DateTime.Now.AddHours(float signedHours)

// test some examples
let example1 = getDate 5 Days Ago
let example2 = getDate 1 Hour Hence

// the C# equivalent would probably be more like this:
// getDate().Interval(5).Days().Ago()
// getDate().Interval(1).Hour().Hence()
```

上面的例子只有一个“动词”，对“名词”使用了很多类型。

以下示例演示了如何构建具有许多“动词”的流畅界面的功能等效物。

假设我们正在创建一个具有各种形状的绘图程序。每个形状都有一个颜色、大小、标签和点击时要执行的操作，我们希望有一个流畅的界面来配置每个形状。

下面是一个C#中流畅接口的简单方法链的示例：

```c#
FluentShape.Default
   .SetColor("red")
   .SetLabel("box")
   .OnClick( s => Console.Write("clicked") );
```

现在，“流畅接口”和“方法链”的概念实际上只适用于面向对象的设计。在F#这样的函数式语言中，最接近的等价物是使用管道运算符将一组函数链接在一起。

让我们从基础Shape类型开始：

```F#
// create an underlying type
type FluentShape = {
    label : string;
    color : string;
    onClick : FluentShape->FluentShape // a function type
    }
```

我们将添加一些基本函数：

```F#
let defaultShape =
    {label=""; color=""; onClick=fun shape->shape}

let click shape =
    shape.onClick shape

let display shape =
    printfn "My label=%s and my color=%s" shape.label shape.color
    shape   //return same shape
```

为了使“方法链”工作，每个函数都应该返回一个可以在链中下一步使用的对象。因此，您将看到“`display`”函数返回形状，而不是什么都不返回。

接下来，我们创建一些辅助函数，将其作为“迷你语言”公开，并将被该语言的用户用作构建块。

```F#
let setLabel label shape =
   {shape with FluentShape.label = label}

let setColor color shape =
   {shape with FluentShape.color = color}

//add a click action to what is already there
let appendClickAction action shape =
   {shape with FluentShape.onClick = shape.onClick >> action}
```

请注意，`appendClickAction` 将函数作为参数，并将其与现有的单击操作组合在一起。当你开始深入了解重用的函数方法时，你会开始看到更多像这样的“高阶函数”，即作用于其他函数的函数。组合这样的函数是理解函数式编程的关键之一。

现在，作为这种“迷你语言”的用户，我可以将基本辅助函数组合成我自己的更复杂的函数，创建我自己的函数库。（在 C# 中，这种事情可以使用扩展方法来完成。）

```F#
// Compose two "base" functions to make a compound function.
let setRedBox = setColor "red" >> setLabel "box"

// Create another function by composing with previous function.
// It overrides the color value but leaves the label alone.
let setBlueBox = setRedBox >> setColor "blue"

// Make a special case of appendClickAction
let changeColorOnClick color = appendClickAction (setColor color)
```

然后，我可以将这些函数组合在一起，创建具有所需行为的对象。

```F#
//setup some test values
let redBox = defaultShape |> setRedBox
let blueBox = defaultShape |> setBlueBox

// create a shape that changes color when clicked
redBox
    |> display
    |> changeColorOnClick "green"
    |> click
    |> display  // new version after the click

// create a shape that changes label and color when clicked
blueBox
    |> display
    |> appendClickAction (setLabel "box2" >> setColor "green")
    |> click
    |> display  // new version after the click
```

在第二种情况下，我实际上将两个函数传递给 `appendClickAction`，但我首先将它们组合成一个。对于一个结构良好的函数库来说，这类事情是微不足道的，但在 C# 中，如果没有 lambdas 中的 lambdas，就很难做到这一点。

这里有一个更复杂的例子。我们将创建一个函数“`showRainbow`”，为彩虹中的每种颜色设置颜色并显示形状。

```F#
let rainbow =
    ["red";"orange";"yellow";"green";"blue";"indigo";"violet"]

let showRainbow =
    let setColorAndDisplay color = setColor color >> display
    rainbow
    |> List.map setColorAndDisplay
    |> List.reduce (>>)

// test the showRainbow function
defaultShape |> showRainbow
```

请注意，函数变得越来越复杂，但代码量仍然很小。其中一个原因是，在进行函数组合时，函数参数通常可以忽略，这减少了视觉混乱。例如，“`showRainbow`”函数确实将形状作为参数，但没有明确显示！这种参数的省略被称为“无点”风格，将在“函数式思维”系列中进一步讨论。



# 12 简洁的模式匹配

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/conciseness-pattern-matching/#series-toc)*)*

模式匹配可以在一个步骤中进行匹配和绑定
2012年4月12日 这篇文章已有3年多的历史了

https://fsharpforfunandprofit.com/posts/conciseness-pattern-matching/

到目前为止，我们已经看到了匹配中的模式匹配逻辑。。对于表达式，它似乎只是一个switch/case语句。但事实上，模式匹配更为通用——它可以通过多种方式比较表达式，在值、条件和类型上进行匹配，然后同时分配或提取值。

模式匹配将在后面的文章中进行深入讨论，但首先，这里有一个帮助简洁的方法。我们将研究模式匹配用于将值绑定到表达式的方式（相当于赋值给变量的功能）。

在以下示例中，我们直接绑定到元组和列表的内部成员：

```F#
//matching tuples directly
let firstPart, secondPart, _ =  (1,2,3)  // underscore means ignore

//matching lists directly
let elem1::elem2::rest = [1..10]       // ignore the warning for now

//matching lists inside a match..with
let listMatcher aList =
    match aList with
    | [] -> printfn "the list is empty"
    | [firstElement] -> printfn "the list has one element %A " firstElement
    | [first; second] -> printfn "list is %A and %A" first second
    | _ -> printfn "the list has more than two elements"

listMatcher [1;2;3;4]
listMatcher [1;2]
listMatcher [1]
listMatcher []
```

您还可以将值绑定到记录等复杂结构的内部。在下面的示例中，我们将创建一个“`Address`”类型，然后创建一个包含地址的“`Customer`”类型。接下来，我们将创建一个客户价值，然后将各种属性与之匹配。

```F#
// create some types
type Address = { Street: string; City: string; }
type Customer = { ID: int; Name: string; Address: Address}

// create a customer
let customer1 = { ID = 1; Name = "Bob";
      Address = {Street="123 Main"; City="NY" } }

// extract name only
let { Name=name1 } =  customer1
printfn "The customer is called %s" name1

// extract name and id
let { ID=id2; Name=name2; } =  customer1
printfn "The customer called %s has id %i" name2 id2

// extract name and address
let { Name=name3;  Address={Street=street3}  } =  customer1
printfn "The customer is called %s and lives on %s" name3 street3
```

在最后一个示例中，请注意我们如何直接进入 `Address` 子结构，并提取街道和客户名称。

这种在一个步骤中处理嵌套结构、仅提取所需字段并将其赋值的能力非常有用。它消除了相当多的编码苦差事，是典型 F# 代码简洁性的另一个因素。

# 13 便利性

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/convenience-intro/#series-toc)*)*

减少编程繁琐和样板代码的功能
2012年4月13日 这篇文章已有3年多的历史了

https://fsharpforfunandprofit.com/posts/convenience-intro/

在下一组帖子中，我们将探讨我在“便利性”主题下分组的 F# 的更多功能。这些特性不一定能产生更简洁的代码，但它们确实消除了 C# 中所需的许多繁琐和样板代码。

- **对类型有用的“开箱即用”行为**。您创建的大多数类型都会立即具有一些有用的行为，例如不变性和内置相等性——这些功能必须在C#中显式编码。
- **所有函数都是“接口”**，这意味着接口在面向对象设计中扮演的许多角色都隐含在函数的工作方式中。同样，许多面向对象的设计模式在函数范式中是不必要的或微不足道的。
- **部分应用**。具有许多参数的复杂函数可以固定或“内置”一些参数，但保留其他参数。
- **活跃的模式**（Active patterns）。主动模式（Active patterns）是一种特殊的模式，可以动态地而不是静态地匹配或检测模式。它们非常适合简化常用的解析和分组行为。

# 14 类型的开箱即用行为

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/convenience-types/#series-toc)*)*

不可变和内置相等性，无需编码
14 Apr 2012 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/convenience-types/

F# 的一个优点是，大多数类型立即具有一些有用的“开箱即用”行为，如不变性和内置相等性，这些功能通常必须在 C# 中显式编码。

我所说的“大多数” F# 类型，是指核心的“结构”类型，如元组、记录、联合、选项、列表等。添加了类和其他一些类型来给 .NET 集成提供帮助，但失去了结构类型的一些功能。

这些核心类型的内置功能包括：

- 不可变性
- 调试时打印效果不错
- 相等
- 比较

下文将逐一介绍。

## F# 类型具有内置的不变性

在 C# 和 Java 中，尽可能创建不可变类已经成为一种很好的做法。在 F# 中，你可以免费获得这个。

这是 F# 中的一个不可变类型：

```F#
type PersonalName = {FirstName:string; LastName:string}
```

以下是 C# 中相同类型的典型编码方式：

```c#
class ImmutablePersonalName
{
    public ImmutablePersonalName(string firstName, string lastName)
    {
        this.FirstName = firstName;
        this.LastName = lastName;
    }

    public string FirstName { get; private set; }
    public string LastName { get; private set; }
}
```

这是 10 行代码，与 F# 的 1 行代码做同样的事情。

## 大多数 F# 类型都内置了漂亮的打印功能

在 F# 中，你不必为大多数类型重写 `ToString()`——你可以免费获得漂亮的打印效果！

在运行前面的示例时，您可能已经看到了这一点。下面是另一个简单的例子：

```F#
type USAddress =
   {Street:string; City:string; State:string; Zip:string}
type UKAddress =
   {Street:string; Town:string; PostCode:string}
type Address =
   | US of USAddress
   | UK of UKAddress
type Person =
   {Name:string; Address:Address}

let alice = {
   Name="Alice";
   Address=US {Street="123 Main";City="LA";State="CA";Zip="91201"}}
let bob = {
   Name="Bob";
   Address=UK {Street="221b Baker St";Town="London";PostCode="NW1 6XE"}}

printfn "Alice is %A" alice
printfn "Bob is %A" bob
```

输出为：

```F#
Alice is {Name = "Alice";
 Address = US {Street = "123 Main";
               City = "LA";
               State = "CA";
               Zip = "91201" };}

Bob is {Name = "Bob";
 Address = UK {Street = "221b Baker St";
               Town = "London";
               PostCode = "NW1 6XE";};}
```

## 大多数 F# 类型都有内置的结构相等性

在 C# 中，您通常必须实现 `IEquatable` 接口，以便测试对象之间的相等性。例如，当使用对象作为字典键时，这是必要的。

在 F# 中，大多数 F# 类型都可以免费获得此功能。例如，使用上面的 `PersonalName` 类型，我们可以直接比较两个名字。

```F#
type PersonalName = {FirstName:string; LastName:string}
let alice1 = {FirstName="Alice"; LastName="Adams"}
let alice2 = {FirstName="Alice"; LastName="Adams"}
let bob1 = {FirstName="Bob"; LastName="Bishop"}

//test
printfn "alice1=alice2 is %A" (alice1=alice2)
printfn "alice1=bob1 is %A" (alice1=bob1)
```

## 大多数 F# 类型都是自动可比较的

在 C# 中，您通常必须实现 `IComparable` 接口，以便对对象进行排序。

同样，在 F# 中，大多数 F# 类型都可以免费获得此功能。例如，这是一副扑克牌的简单定义。

```F#
type Suit = Club | Diamond | Spade | Heart
type Rank = Two | Three | Four | Five | Six | Seven | Eight
            | Nine | Ten | Jack | Queen | King | Ace
```

我们可以编写一个函数来测试比较逻辑：

```F#
let compareCard card1 card2 =
    if card1 < card2
    then printfn "%A is greater than %A" card2 card1
    else printfn "%A is greater than %A" card1 card2
```

让我们看看它是如何工作的：

```F#
let aceHearts = Heart, Ace
let twoHearts = Heart, Two
let aceSpades = Spade, Ace

compareCard aceHearts twoHearts
compareCard twoHearts aceSpades
```

请注意，Ace of Hearts 会自动大于 Two of Hearts，因为“Ace”排名值在“Two”排名值之后。

但也要注意，Two of Hearts 会自动大于 Ace of Spades，因为首先比较的是 Suit 部分，而“Heart”套装值在“Spade”值之后。

这是一个纸牌手的例子：

```F#
let hand = [ Club,Ace; Heart,Three; Heart,Ace;
             Spade,Jack; Diamond,Two; Diamond,Ace ]

//instant sorting!
List.sort hand |> printfn "sorted hand is (low to high) %A"
```

作为附带福利，您还可以免费获得最小值和最大值！

```F#
List.max hand |> printfn "high card is %A"
List.min hand |> printfn "low card is %A"
```

> 如果你对领域建模和设计的函数式方法感兴趣，你可能会喜欢我的《领域建模函数式》一书！这是一个初学者友好的介绍，涵盖了领域驱动设计、类型建模和函数式编程。

# 15 作为接口的函数式

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

## 通用包装

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

## 策略模式

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

# 16 局部应用

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/convenience-partial-application/#series-toc)*)*

如何固定函数的一些参数
16 Apr 2012 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/convenience-partial-application/

F# 的一个特别方便的特性是，具有许多参数的复杂函数可以将一些参数固定或“内置”，但其他参数保持打开状态。在这篇文章中，我们将快速了解如何在实践中使用它。

让我们从一个非常简单的例子开始，看看它是如何工作的。我们将从一个简单的函数开始：

```F#
// define a adding function
let add x y = x + y

// normal use
let z = add 1 2
```

但我们也可以做一些奇怪的事情——我们可以只用一个参数调用函数！

```F#
let add42 = add 42
```

结果是一个新函数，它内置了“42”，现在只接受一个参数，而不是两个！这种技术被称为“部分应用”，这意味着，对于任何函数，你都可以“固定”一些参数，并留下其他参数供以后填写。

```F#
// use the new function
add42 2
add42 3
```

有了这些，让我们重新审视一下我们之前看到的通用记录器：

```F#
let genericLogger anyFunc input =
   printfn "input is %A" input   //log the input
   let result = anyFunc input    //evaluate the function
   printfn "result is %A" result //log the result
   result                        //return the result
```

不幸的是，我已经硬编码了日志操作。理想情况下，我想让它更通用，这样我就可以选择如何进行日志记录。

当然，F#是一种函数式编程语言，我们将通过传递函数来实现这一点。

在这种情况下，我们会将“before”和“after”回调函数传递给库函数，如下所示：

```F#
let genericLogger before after anyFunc input =
   before input               //callback for custom behavior
   let result = anyFunc input //evaluate the function
   after result               //callback for custom behavior
   result                     //return the result
```

您可以看到日志功能现在有四个参数。“before”和“after”动作作为显式参数以及函数及其输入传递。为了在实践中使用它，我们只需定义函数并将它们与最后的 int 参数一起传递给库函数：

```F#
let add1 input = input + 1

// reuse case 1
genericLogger
    (fun x -> printf "before=%i. " x) // function to call before
    (fun x -> printfn " after=%i." x) // function to call after
    add1                              // main function
    2                                 // parameter

// reuse case 2
genericLogger
    (fun x -> printf "started with=%i " x) // different callback
    (fun x -> printfn " ended with=%i" x)
    add1                              // main function
    2                                 // parameter
```

这要灵活得多。我不必每次想改变行为时都创建一个新函数——我可以动态定义行为。

但你可能会认为这有点丑陋。一个库函数可能会暴露许多回调函数，并且必须一遍又一遍地传递相同的函数会很不方便。

幸运的是，我们知道解决这个问题的办法。我们可以使用部分应用程序来修复一些参数。因此，在这种情况下，让我们定义一个新函数，它固定了 `before` 和 `after` 函数以及 `add1` 函数，但保留了最后一个参数。

```F#
// define a reusable function with the "callback" functions fixed
let add1WithConsoleLogging =
    genericLogger
        (fun x -> printf "input=%i. " x)
        (fun x -> printfn " result=%i" x)
        add1
        // last parameter NOT defined here yet!
```

现在只使用 int 调用新的“包装器”函数，因此代码更简洁。与前面的示例一样，它可以在任何可以使用原始 `add1` 函数而无需任何更改的地方使用。

```F#
add1WithConsoleLogging 2
add1WithConsoleLogging 3
add1WithConsoleLogging 4
[1..5] |> List.map add1WithConsoleLogging
```

## C# 中的函数方法

在经典的面向对象方法中，我们可能会使用继承来做这类事情。例如，我们可能有一个抽象的 `LoggerBase` 类，其中包含“`before`”和“`after`”的虚拟方法以及要执行的函数。然后，为了实现一种特定的行为，我们会创建一个新的子类，并根据需要重写虚拟方法。

但在面向对象的设计中，经典风格的继承现在变得不受欢迎，对象的组合更受欢迎。事实上，在“现代”C#中，我们可能会以与F#相同的方式编写代码，要么使用事件，要么传入函数。

这是转换为C#的F#代码（请注意，我必须为每个Action指定类型）

```c#
public class GenericLoggerHelper<TInput, TResult>
{
    public TResult GenericLogger(
        Action<TInput> before,
        Action<TResult> after,
        Func<TInput, TResult> aFunc,
        TInput input)
    {
        before(input);             //callback for custom behavior
        var result = aFunc(input); //do the function
        after(result);             //callback for custom behavior
        return result;
    }
}
```

它在这里使用：

```c#
[NUnit.Framework.Test]
public void TestGenericLogger()
{
    var sut = new GenericLoggerHelper<int, int>();
    sut.GenericLogger(
        x => Console.Write("input={0}. ", x),
        x => Console.WriteLine(" result={0}", x),
        x => x + 1,
        3);
}
```

在 C# 中，使用 LINQ 库时需要这种编程风格，但许多开发人员还没有完全接受它，以使自己的代码更通用、更具适应性。而且，所需的丑陋的 `Action<>` 和 `Func<>` 类型声明也无济于事。但它肯定可以使代码更具可重用性。

# 17 活动模式（Active patterns）

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/convenience-active-patterns/#series-toc)*)*

动态模式（Dynamic patterns），实现强力匹配
2012年4月17日 这篇文章已有3年多的历史了

https://fsharpforfunandprofit.com/posts/convenience-active-patterns/

F# 有一种特殊类型的模式匹配，称为“活动模式”，可以动态解析或检测模式。与正常模式一样，从调用者的角度来看，匹配和输出被组合成一个步骤。

下面是一个使用活动模式将字符串解析为 int 或 bool 的示例。

```F#
// create an active pattern
let (|Int|_|) str =
   match System.Int32.TryParse(str:string) with
   | (true,int) -> Some(int)
   | _ -> None

// create an active pattern
let (|Bool|_|) str =
   match System.Boolean.TryParse(str:string) with
   | (true,bool) -> Some(bool)
   | _ -> None
```

> 您现在不需要担心用于定义活动模式的复杂语法——这只是一个例子，这样您就可以看到它们是如何使用的。

一旦设置了这些模式，它们就可以用作正常的“`match..with`”表达式的一部分。

```F#
// create a function to call the patterns
let testParse str =
    match str with
    | Int i -> printfn "The value is an int '%i'" i
    | Bool b -> printfn "The value is a bool '%b'" b
    | _ -> printfn "The value '%s' is something else" str

// test
testParse "12"
testParse "true"
testParse "abc"
```

您可以看到，从调用者的角度来看，与 `Int` 或 `Bool` 的匹配是透明的，即使在幕后进行了解析。

一个类似的例子是将活动模式与正则表达式一起使用，以便在正则表达式模式上进行匹配，并在一个步骤中返回匹配的值。

```F#
// create an active pattern
open System.Text.RegularExpressions
let (|FirstRegexGroup|_|) pattern input =
   let m = Regex.Match(input,pattern)
   if (m.Success) then Some m.Groups.[1].Value else None
```

同样，一旦设置了此模式，它就可以透明地用作正常匹配表达式的一部分。

```F#
// create a function to call the pattern
let testRegex str =
    match str with
    | FirstRegexGroup "http://(.*?)/(.*)" host ->
           printfn "The value is a url and the host is %s" host
    | FirstRegexGroup ".*?@(.*)" host ->
           printfn "The value is an email and the host is %s" host
    | _ -> printfn "The value '%s' is something else" str

// test
testRegex "http://google.com/test"
testRegex "alice@hotmail.com"
```

为了好玩，这里还有一个：著名的使用主动模式编写的 FizzBuzz 挑战。

```F#
// setup the active patterns
let (|MultOf3|_|) i = if i % 3 = 0 then Some MultOf3 else None
let (|MultOf5|_|) i = if i % 5 = 0 then Some MultOf5 else None

// the main function
let fizzBuzz i =
  match i with
  | MultOf3 & MultOf5 -> printf "FizzBuzz, "
  | MultOf3 -> printf "Fizz, "
  | MultOf5 -> printf "Buzz, "
  | _ -> printf "%i, " i

// test
[1..20] |> List.iter fizzBuzz
```

# 18 正确性

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/correctness-intro/#series-toc)*)*

如何编写“编译时单元测试”
2012年4月18日 这篇文章已有3年多的历史了

https://fsharpforfunandprofit.com/posts/correctness-intro/

作为一名程序员，你不断地判断你和其他人编写的代码。在一个理想的世界里，你应该能够看到一段代码，并很容易地理解它的确切功能；当然，简洁、清晰和可读是其中的一个主要因素。

但更重要的是，你必须能够说服自己，代码做了它应该做的事情。当你编程时，你不断地推理代码的正确性，你大脑中的小编译器正在检查代码是否有错误和可能的错误。

那么，编程语言如何帮助你做到这一点呢？

像 C# 这样的现代命令式语言提供了许多您已经熟悉的方法：类型检查、作用域和命名规则、访问修饰符等。而且，在最近的版本中，还提供了静态代码分析和代码契约。

所有这些技术意味着编译器可以承担很多检查正确性的负担。如果你犯了错误，编译器会警告你。

但是 F# 有一些额外的功能，可以对确保正确性产生巨大影响。接下来的几个帖子将专门讨论其中的四个：

- **不变性**，使代码的行为更加可预测。
- **详尽的模式匹配**，在编译时捕获了许多常见错误。
- **一个严格的类型系统**，它是你的朋友，而不是你的敌人。您几乎可以将静态类型检查用作即时的“编译时单元测试”。
- **一个富有表现力的类型系统**，可以帮助你“使非法状态变得不可表示”*。我们将看到如何设计一个真实世界的例子来演示这一点。

*感谢简街的亚伦·明斯基（Yaron Minsky at Jane Street）说出这句话。

# 19 不可变性

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/correctness-immutability/#series-toc)*)*

让你的代码可预测
2012年4月19日 这篇文章已有3年多的历史了

https://fsharpforfunandprofit.com/posts/correctness-immutability/

为了了解为什么不变性很重要，让我们从一个小例子开始。

下面是一些处理数字列表的简单C#代码。

```c#
public List<int> MakeList()
{
   return new List<int> {1,2,3,4,5,6,7,8,9,10};
}

public List<int> OddNumbers(List<int> list)
{
   // some code
}

public List<int> EvenNumbers(List<int> list)
{
   // some code
}
```

现在让我来测试一下：

```c#
public void Test()
{
   var odds = OddNumbers(MakeList());
   var evens = EvenNumbers(MakeList());
   // assert odds = 1,3,5,7,9 -- OK!
   // assert evens = 2,4,6,8,10 -- OK!
}
```

一切都很好，测试也通过了，但我注意到我创建了两次列表——我当然应该重构它吗？所以我进行了重构，这是新的改进版本：

```c#
public void RefactoredTest()
{
   var list = MakeList();
   var odds = OddNumbers(list);
   var evens = EvenNumbers(list);
   // assert odds = 1,3,5,7,9 -- OK!
   // assert evens = 2,4,6,8,10 -- FAIL!
}
```

但现在测试突然失败了！为什么重构会破坏测试？你能只看代码就知道吗？

当然，答案是列表是可变的，并且很可能 `OddNumbers` 函数正在对列表进行破坏性更改，作为其过滤逻辑的一部分。当然，为了确定，我们必须检查 `OddNumbers` 函数内的代码。

换句话说，当我调用 `OddNumbers` 函数时，我无意中产生了不良的副作用。

有没有办法确保这种情况不会发生？是–如果函数使用了 `IEnumerable`：

```c#
public IEnumerable<int> MakeList() {}
public List<int> OddNumbers(IEnumerable<int> list) {}
public List<int> EvenNumbers(IEnumerable <int> list) {}
```

在这种情况下，我们可以确信调用 `OddNumbers` 函数不可能对列表产生任何影响，`EvenNumbers` 将正常工作。更重要的是，我们只需查看签名就可以知道这一点，而不必检查函数的内部。如果你试图通过分配给列表来使其中一个函数行为异常，那么在编译时你会立刻得到一个错误。

因此，在这种情况下，`IEnumerable` 可以提供帮助，但如果我使用了 `IEnumerable<Person>` 这样的类型而不是 `IEnumerable<int>` 呢？我还能相信这些功能不会有无意的副作用吗？

## 不变性重要的原因

上面的例子说明了为什么不变性是有帮助的。事实上，这只是冰山一角。不变性很重要的原因有很多：

- 不可变数据使代码可预测
- 不可变数据更易于使用
- 不可变数据迫使您使用“转型”方法

首先，不变性使代码具有**可预测性**。如果数据是不可变的，就不会有副作用。如果没有副作用，就更容易对代码的正确性进行推理。

当你有两个函数处理不可变数据时，你不必担心调用它们的顺序，也不必担心一个函数是否会干扰另一个函数的输入。在传递数据时，您可以放心（例如，您不必担心将对象用作哈希表中的键并更改其哈希码）。

事实上，不变性是一个好主意，因为全局变量是一个坏主意：数据应尽可能保持局部，并应避免副作用。

其次，不变性**更容易使用**。如果数据是不可变的，那么许多常见的任务就会变得容易得多。代码更容易编写，也更容易维护。需要更少的单元测试（你只需要检查一个函数是否独立工作），并且模拟要容易得多。并发性要简单得多，因为您不必担心使用锁来避免更新冲突（因为没有更新）。

最后，默认使用不变性意味着您开始以不同的方式思考编程。你倾向于考虑**转换**数据，而不是在原地对其进行突变。

SQL 查询和 LINQ 查询就是这种“转换”方法的好例子。在这两种情况下，您总是通过各种函数（选择、筛选、排序）转换原始数据，而不是修改原始数据。

当使用转换方法设计程序时，结果往往更优雅、更模块化、更具可扩展性。碰巧的是，转换方法也与面向功能的范式完美契合。

## F# 如何实现不变性

我们之前看到不可变值和类型是F#的默认：

```F#
// immutable list
let list = [1;2;3;4]

type PersonalName = {FirstName:string; LastName:string}
// immutable person
let john = {FirstName="John"; LastName="Doe"}
```

因此，F# 有许多技巧可以让生活更轻松，并优化底层代码。

首先，由于您无法修改数据结构，因此必须在需要更改时复制它。F# 可以轻松复制另一个数据结构，只需进行所需的更改：

```F#
let alice = {john with FirstName="Alice"}
```

复杂的数据结构被实现为链表或类似结构，以便共享结构的公共部分。

```F#
// create an immutable list
let list1 = [1;2;3;4]

// prepend to make a new list
let list2 = 0::list1

// get the last 4 of the second list
let list3 = list2.Tail

// the two lists are the identical object in memory!
System.Object.ReferenceEquals(list1,list3)
```

这种技术可以确保，虽然你的代码中可能有数百个列表副本，但它们在幕后共享相同的内存。

## 可变数据

F# 对不变性并不教条；它确实支持使用 `mutable` 关键字的可变数据。但是启用可变性是一个明确的决定，与默认值不同，通常只在优化、缓存等特殊情况下或处理 .NET 库时才需要。

在实践中，如果一个严肃的应用程序处理用户界面、数据库、网络等混乱的世界，它必然会有一些可变状态。但 F# 鼓励将这种可变状态最小化。通常，您仍然可以将核心业务逻辑设计为使用不可变数据，并获得所有相应的好处。

# 20 穷举的模式匹配

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/correctness-exhaustive-pattern-matching/#series-toc)*)*

一种确保正确性的强大技术
2012年4月20日 这篇文章已有3年多的历史了

https://fsharpforfunandprofit.com/posts/correctness-exhaustive-pattern-matching/

我们之前简要地指出，在模式匹配时，需要匹配所有可能的情况。事实证明，这是一种非常强大的技术，可以确保正确性。

让我们再次比较一下 C# 和 F#。下面是一些使用 switch 语句处理不同类型状态的C#代码。

```c#
enum State { New, Draft, Published, Inactive, Discontinued }
void HandleState(State state)
{
    switch (state)
    {
    case State.Inactive:
        ...
    case State.Draft:
        ...
    case State.New:
        ...
    case State.Discontinued:
        ...
    }
}
```

这段代码可以编译，但有一个明显的错误！编译器看不到它——你能吗？如果你能，而且你已经解决了这个问题，如果我在名单上增加另一个 `State`，它会保持不变吗？

这是F#的等价物：

```F#
type State = New | Draft | Published | Inactive | Discontinued
let handleState state =
   match state with
   | Inactive ->
      ...
   | Draft ->
      ...
   | New ->
      ...
   | Discontinued ->
      ...
```

现在试着运行这段代码。编译器告诉你什么？它会说：

`此表达式上的模式匹配不完整。`
`例如，值“Published”可能表示模式未涵盖的案例`

穷举匹配总是完成的事实意味着编译器会立即检测到某些常见错误：

- 缺失的案例（通常是由于需求更改或重构而添加新选项时造成的）。
- 一个不可能的情况（当现有的选择被删除时）。
- 一个永远无法触及的冗余案例（该案例已被归入之前的案例中——这有时可能是不明显的）。

现在，让我们来看一些真实的例子，说明穷举匹配如何帮助您编写正确的代码。

## 避免 Option 类型为空

我们将从一个非常常见的场景开始，在这个场景中，调用者应该始终检查无效的情况，即测试null。一个典型的C#程序中充斥着这样的代码：

```c#
if (myObject != null)
{
  // do something
}
```

不幸的是，编译器不需要此测试。只需一段代码忘记执行此操作，程序就会崩溃。多年来，大量的编程工作都致力于处理 null——null 的发明甚至被称为十亿美元的错误！

在纯 F# 中，null 不能意外存在。字符串或对象在创建时必须始终分配给某物，此后是不可变的。

然而，在许多情况下，设计意图是区分有效值和无效值，您需要调用者处理这两种情况。

在 C# 中，在某些情况下可以通过使用可以为 null 的值类型（如 `Nullable<int>`）来管理这一点，以使设计决策清晰明了。当遇到可空值时，编译器会强制您注意它。然后，您可以在使用该值之前测试其有效性。但可空值不适用于标准类（即引用类型），并且很容易意外绕过测试，直接调用 `Value`。

在 F# 中，有一个类似但更强大的概念来传达设计意图：名为 `Option` 的通用包装器类型，有两个选择：`Some` 或 `None`。`Some` 选项包装了一个有效值，`None` 表示缺少值。

这里有一个例子，如果文件存在，则返回 `Some`，但缺少文件则返回 `None`。

```F#
let getFileInfo filePath =
   let fi = new System.IO.FileInfo(filePath)
   if fi.Exists then Some(fi) else None

let goodFileName = "good.txt"
let badFileName = "bad.txt"

let goodFileInfo = getFileInfo goodFileName // Some(fileinfo)
let badFileInfo = getFileInfo badFileName   // None
```

如果我们想用这些值做任何事情，我们必须始终处理这两种可能的情况。

```F#
match goodFileInfo with
  | Some fileInfo ->
      printfn "the file %s exists" fileInfo.FullName
  | None ->
      printfn "the file doesn't exist"

match badFileInfo with
  | Some fileInfo ->
      printfn "the file %s exists" fileInfo.FullName
  | None ->
      printfn "the file doesn't exist"
```

我们别无选择。不处理案例是编译时错误，而不是运行时错误。通过避免 null 并以这种方式使用 Option 类型，F# 完全消除了大量 null 引用异常。

警告：F# 确实允许您在不进行测试的情况下访问值，就像 C# 一样，但这被认为是极其糟糕的做法。

## 边缘情况下的穷尽模式匹配

下面是一些C#代码，它通过对输入列表中的数字对求平均值来创建列表：

```c#
public IList<float> MovingAverages(IList<int> list)
{
    var averages = new List<float>();
    for (int i = 0; i < list.Count; i++)
    {
        var avg = (list[i] + list[i+1]) / 2;
        averages.Add(avg);
    }
    return averages;
}
```

它编译正确，但实际上有几个问题。你能很快找到他们吗？如果你幸运的话，你的单元测试会为你找到它们，假设你已经考虑了所有的边缘情况。

现在让我们在 F# 中尝试同样的事情：

```F#
let rec movingAverages list =
    match list with
    // if input is empty, return an empty list
    | [] -> []
    // otherwise process pairs of items from the input
    | x::y::rest ->
        let avg = (x+y)/2.0
        //build the result by recursing the rest of the list
        avg :: movingAverages (y::rest)
```

这段代码也有一个 bug。但与 C# 不同，在我修复之前，这段代码甚至不会编译。当我的列表中只有一个项目时，编译器会告诉我，我还没有处理过这种情况。它不仅发现了一个 bug，还揭示了需求中的一个缺口：当只有一个项目时应该发生什么？

以下是修复版本：

```F#
let rec movingAverages list =
    match list with
    // if input is empty, return an empty list
    | [] -> []
    // otherwise process pairs of items from the input
    | x::y::rest ->
        let avg = (x+y)/2.0
        //build the result by recursing the rest of the list
        avg :: movingAverages (y::rest)
    // for one item, return an empty list
    | [_] -> []

// test
movingAverages [1.0]
movingAverages [1.0; 2.0]
movingAverages [1.0; 2.0; 3.0]
```

作为额外的好处，F# 代码也更加自文档化。它明确地描述了每个案例的后果。在 C# 代码中，如果列表为空或只有一个项目，会发生什么并不明显。你必须仔细阅读代码才能找到答案。

## 穷尽模式匹配作为一种错误处理技术

所有选项都必须匹配的事实也可以作为抛出异常的有用替代方案。例如，考虑以下常见场景：

- 在应用程序的最底层有一个实用函数，它打开一个文件并对其执行任意操作（您将其作为回调函数传递）
- 然后将结果向上传递到最高层。
- 客户端调用顶层代码，处理结果并完成任何错误处理。

在过程式或面向对象语言中，跨代码层传播和处理异常是一个常见问题。顶级函数不容易区分它们应该从中恢复的异常（比如 `FileNotFound`）和它们不需要处理的异常（例如 `OutOfMemory`）。在 Java 中，有人试图用检查异常来实现这一点，但结果喜忧参半。

在函数世界中，一种常见的技术是创建一个新的结构来保存好的和坏的可能性，而不是在文件丢失时抛出异常。

```F#
// define a "union" of two different alternatives
type Result<'a, 'b> =
    | Success of 'a  // 'a means generic type. The actual type
                     // will be determined when it is used.
    | Failure of 'b  // generic failure type as well

// define all possible errors
type FileErrorReason =
    | FileNotFound of string
    | UnauthorizedAccess of string * System.Exception

// define a low level function in the bottom layer
let performActionOnFile action filePath =
   try
      //open file, do the action and return the result
      use sr = new System.IO.StreamReader(filePath:string)
      let result = action sr  //do the action to the reader
      Success (result)        // return a Success
   with      // catch some exceptions and convert them to errors
      | :? System.IO.FileNotFoundException as ex
          -> Failure (FileNotFound filePath)
      | :? System.Security.SecurityException as ex
          -> Failure (UnauthorizedAccess (filePath,ex))
      // other exceptions are unhandled
```

该代码演示了 `performActionOnFile` 如何返回一个 `Result` 对象，该对象有两个选项：成功和失败。`Failure` 选项也有两个选项：`FileNotFound` 和 `Unauthorized Access`。

现在，中间层可以相互调用，传递结果类型，而不必担心它的结构是什么，只要它们不访问它：

```F#
// a function in the middle layer
let middleLayerDo action filePath =
    let fileResult = performActionOnFile action filePath
    // do some stuff
    fileResult //return

// a function in the top layer
let topLayerDo action filePath =
    let fileResult = middleLayerDo action filePath
    // do some stuff
    fileResult //return
```

由于类型推断，中间层和顶层不需要指定返回的确切类型。如果下层根本更改了类型定义，则中间层不会受到影响。

显然，在某些时候，顶层的客户端确实希望访问结果。这里是执行匹配所有模式的要求的地方。客户端必须处理 `Failure` 的情况，否则编译器将发出投诉。此外，在处理 `Failure` 分支时，还必须处理可能的原因。换句话说，这种特殊情况处理可以在编译时强制执行，而不是在运行时！此外，通过检查原因类型，明确记录了可能的原因。

以下是一个访问顶层的客户端函数示例：

```F#
/// get the first line of the file
let printFirstLineOfFile filePath =
    let fileResult = topLayerDo (fun fs->fs.ReadLine()) filePath

    match fileResult with
    | Success result ->
        // note type-safe string printing with %s
        printfn "first line is: '%s'" result
    | Failure reason ->
       match reason with  // must match EVERY reason
       | FileNotFound file ->
           printfn "File not found: %s" file
       | UnauthorizedAccess (file,_) ->
           printfn "You do not have access to the file: %s" file
```

您可以看到，此代码必须显式处理成功和失败情况，然后对于失败情况，它显式处理不同的原因。如果你想看看如果它不处理其中一种情况会发生什么，试着注释掉处理未经授权访问的行，看看编译器会说什么。

现在，您不需要始终明确地处理所有可能的情况。在下面的示例中，该函数使用下划线通配符将所有失败原因视为一个。如果我们想从严格中获益，这可以被认为是一种糟糕的做法，但至少这是明确的。

```F#
/// get the length of the text in the file
let printLengthOfFile filePath =
   let fileResult =
     topLayerDo (fun fs->fs.ReadToEnd().Length) filePath

   match fileResult with
   | Success result ->
      // note type-safe int printing with %i
      printfn "length is: %i" result
   | Failure _ ->
      printfn "An error happened but I don't want to be specific"
```

现在，让我们通过一些交互式测试来查看所有这些代码在实践中的工作情况。

首先设置一个好文件和一个坏文件。

```F#
/// write some text to a file
let writeSomeText filePath someText =
    use writer = new System.IO.StreamWriter(filePath:string)
    writer.WriteLine(someText:string)

let goodFileName = "good.txt"
let badFileName = "bad.txt"

writeSomeText goodFileName "hello"
```

现在交互式测试：

```F#
printFirstLineOfFile goodFileName
printLengthOfFile goodFileName

printFirstLineOfFile badFileName
printLengthOfFile badFileName
```

我认为你可以看到这种方法非常有吸引力：

- 函数为每个预期的情况（如 `FileNotFound`）返回错误类型，但处理这些类型不需要使调用代码变得丑陋。
- 函数会继续为意外情况（如 `OutOfMemory`）抛出异常，这些异常通常会在程序的顶层被捕获和记录。

这项技术简单方便。类似的（更通用的）方法是函数式编程的标准方法。

在 C# 中使用这种方法也是可行的，但由于缺乏联合类型和类型推理（我们必须在所有地方指定泛型类型），这种方法通常是不切实际的。

## 作为变更管理工具的穷举模式匹配

最后，穷举模式匹配是一种有价值的工具，可以确保代码在需求变化或重构过程中保持正确。

假设需求发生了变化，我们需要处理第三种错误：“不确定”。要实现这一新要求，请按如下方式更改第一个Result类型，并重新评估所有代码。会发生什么？

```F#
type Result<'a, 'b> =
    | Success of 'a
    | Failure of 'b
    | Indeterminate
```

或者有时需求更改会删除一个可能的选择。要模拟这一点，请更改第一个 `Result` 类型，以消除除一个选项外的所有选项。

```F#
type Result<'a> =
    | Success of 'a
```

现在重新评估代码的其余部分。现在怎么办？

这太强大了！当我们调整选择时，我们立即知道所有需要修复的地方来处理变化。这是静态检查类型错误威力的另一个例子。人们常说，像F#这样的函数式语言“如果编译，它必须是正确的”。

# 21 使用类型系统确保代码正确

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/correctness-type-checking/#series-toc)*)*

在 F# 中，类型系统是你的朋友，而不是你的敌人
21 Apr 2012 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/correctness-type-checking/

您熟悉通过 C# 和 Java 等语言进行静态类型检查。在这些语言中，类型检查很简单，但相当粗糙，与 Python 和 Ruby 等动态语言的自由相比，这可能会被视为一种烦恼。

但在 F# 中，类型系统是你的朋友，而不是你的敌人。您几乎可以将静态类型检查用作即时单元测试，以确保您的代码在编译时是正确的。

在前面的文章中，我们已经看到了一些可以用 F# 中的类型系统做的事情：

- 类型及其相关函数提供了一个抽象来对问题域进行建模。因为创建类型很容易，所以很少有理由避免根据给定问题的需要进行设计，而且与 C# 类不同，很难创建能做所有事情的“厨房水槽”类型。
- 明确的类型有助于维护。由于 F# 使用类型推理，您通常可以轻松重命名或重构类型，而无需使用重构工具。如果以不兼容的方式更改类型，这几乎肯定会产生编译时错误，有助于跟踪任何问题。
- 命名良好的类型提供了有关其在程序中角色的即时文档（此文档永远不会过时）。

在这篇文章和下一篇文章中，我们将重点介绍如何使用类型系统来帮助编写正确的代码。我将演示您可以创建这样的设计，如果您的代码实际编译，它几乎肯定会按设计工作。

## 使用标准类型检查

在 C# 中，您使用编译时检查来验证您的代码，而无需考虑它。例如，您会放弃 `List<string>` 而使用纯 List 吗？或者放弃 `Nullable<int>` 并强制使用带强制转换的 `object`？可能不会。

但是，如果你能有更细粒度的类型呢？您甚至可以进行更好的编译时检查。这正是 F# 所提供的。

F# 类型检查器并不比 C# 类型检查器严格得多。但是，由于创建新类型非常容易，没有混乱，因此可以更好地表示域，并且作为一种有用的副作用，可以避免许多常见错误。

这里有一个简单的例子：

```F#
//define a "safe" email address type
type EmailAddress = EmailAddress of string

//define a function that uses it
let sendEmail (EmailAddress email) =
   printfn "sent an email to %s" email

//try to send one
let aliceEmail = EmailAddress "alice@example.com"
sendEmail aliceEmail

//try to send a plain string
sendEmail "bob@example.com"   //error
```

通过将电子邮件地址包装为特殊类型，我们确保普通字符串不能用作电子邮件特定函数的参数。（在实践中，我们也会隐藏 `EmailAddress` 类型的构造函数，以确保一开始只能创建有效值。）

这里没有什么是 C# 做不到的，但仅仅为了这个目的创建一个新的值类型将是一项相当大的工作，所以在 C# 中，很容易懒惰，只是传递字符串。

## F# 中的其他类型安全功能

在继续讨论“为正确性而设计”这一主要话题之前，让我们看看 F# 是类型安全的其他一些次要但很酷的方法。

### 使用 printf 进行类型安全格式化

这里有一个小特性，演示了F#比C#更具类型安全性的一种方式，以及F#编译器如何捕获仅在C#运行时检测到的错误。

尝试评估以下内容，并查看生成的错误：

```F#
let printingExample =
   printf "an int %i" 2                        // ok
   printf "an int %i" 2.0                      // wrong type
   printf "an int %i" "hello"                  // wrong type
   printf "an int %i"                          // missing param

   printf "a string %s" "hello"                // ok
   printf "a string %s" 2                      // wrong type
   printf "a string %s"                        // missing param
   printf "a string %s" "he" "lo"              // too many params

   printf "an int %i and string %s" 2 "hello"  // ok
   printf "an int %i and string %s" "hello" 2  // wrong type
   printf "an int %i and string %s" 2          // missing param
```

与 C# 不同，编译器分析格式字符串并确定参数的数量和类型。

这可用于约束参数的类型，而无需明确指定。例如，在下面的代码中，编译器可以自动推断参数的类型。

```F#
let printAString x = printf "%s" x
let printAnInt x = printf "%i" x

// the result is:
// val printAString : string -> unit  //takes a string parameter
// val printAnInt : int -> unit       //takes an int parameter
```

### 计量单位

F# 能够定义度量单位并将其与浮点数相关联。然后，计量单位作为一种类型“附着”在浮子上，防止不同类型的混合。如果你需要的话，这是另一个非常方便的功能。

```F#
// define some measures
[<Measure>]
type cm

[<Measure>]
type inches

[<Measure>]
type feet =
   // add a conversion function
   static member toInches(feet : float<feet>) : float<inches> =
      feet * 12.0<inches/feet>

// define some values
let meter = 100.0<cm>
let yard = 3.0<feet>

//convert to different measure
let yardInInches = feet.toInches(yard)

// can't mix and match!
yard + meter

// now define some currencies
[<Measure>]
type GBP

[<Measure>]
type USD

let gbp10 = 10.0<GBP>
let usd10 = 10.0<USD>
gbp10 + gbp10             // allowed: same currency
gbp10 + usd10             // not allowed: different currency
gbp10 + 1.0               // not allowed: didn't specify a currency
gbp10 + 1.0<_>            // allowed using wildcard
```

### 类型安全相等

最后一个例子。在 C# 中，任何类都可以与任何其他类相等（默认情况下使用引用相等）。总的来说，这是个坏主意！例如，你根本不应该能够将字符串与人进行比较。

以下是一些完全有效且编译良好的 C# 代码：

```c#
using System;
var obj = new Object();
var ex = new Exception();
var b = (obj == ex);
```

如果我们用 F# 编写相同的代码，我们会得到一个编译时错误：

```F#
open System
let obj = new Object()
let ex = new Exception()
let b = (obj = ex)
```

很可能，如果你在测试两种不同类型之间的相等性，你做错了什么。

在 F# 中，你甚至可以完全停止一个类型的比较！这并不像看起来那么愚蠢。对于某些类型，可能没有有用的默认值，或者您可能希望强制相等性基于特定字段而不是整个对象。

以下是一个示例：

```F#
// deny comparison
[<NoEquality; NoComparison>]
type CustomerAccount = {CustomerAccountId: int}

let x = {CustomerAccountId = 1}

x = x       // error!
x.CustomerAccountId = x.CustomerAccountId // no error
```

> 如果你对领域建模和设计的函数式方法感兴趣，你可能会喜欢我的《领域建模函数式》一书！这是一个初学者友好的介绍，涵盖了领域驱动设计、类型建模和函数式编程。

# 22 示例：为正确性而设计

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/designing-for-correctness/#series-toc)*)*

如何使非法状态不具代表性
22 Apr 2012 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/designing-for-correctness/

在这篇文章中，我们将看到如何为正确性（或至少为您目前理解的需求）进行设计，我的意思是，设计良好的模型的客户端将无法将系统置于非法状态——一种不符合需求的状态。你实际上不能创建不正确的代码，因为编译器不允许你这样做。

为了实现这一点，我们确实需要花一些时间预先考虑设计，并努力将需求编码到您使用的类型中。如果你只对所有数据结构使用字符串或列表，你将无法从类型检查中获得任何好处。

我们将使用一个简单的例子。假设你正在设计一个有购物车的电子商务网站，你有以下要求。

- 您只能为购物车付款一次。
- 购物车付款后，您将无法更改其中的商品。
- 空推车无法付款。

## C# 中的糟糕设计

在 C# 中，我们可能会认为这很简单，直接进入编码。这是一个直观的 C# 实现，乍一看似乎还可以。

```c#
public class NaiveShoppingCart<TItem>
{
   private List<TItem> items;
   private decimal paidAmount;

   public NaiveShoppingCart()
   {
      this.items = new List<TItem>();
      this.paidAmount = 0;
   }

   /// Is cart paid for?
   public bool IsPaidFor { get { return this.paidAmount > 0; } }

   /// Readonly list of items
   public IEnumerable<TItem> Items { get {return this.items; } }

   /// add item only if not paid for
   public void AddItem(TItem item)
   {
      if (!this.IsPaidFor)
      {
         this.items.Add(item);
      }
   }

   /// remove item only if not paid for
   public void RemoveItem(TItem item)
   {
      if (!this.IsPaidFor)
      {
         this.items.Remove(item);
      }
   }

   /// pay for the cart
   public void Pay(decimal amount)
   {
      if (!this.IsPaidFor)
      {
         this.paidAmount = amount;
      }
   }
}
```

不幸的是，这实际上是一个相当糟糕的设计：

- 其中一个要求甚至没有得到满足。你能看到哪一个吗？
- 它有一个主要的设计缺陷，还有一些次要的缺陷。你能看到它们是什么吗？

这么短的代码中有这么多问题！

如果我们有更复杂的需求，代码长达数千行，会发生什么？例如，到处重复的片段：

```c#
if (!this.IsPaidFor) { do something }
```

如果某些方法的需求发生了变化，而其他方法没有，那么它看起来会非常脆弱。

在阅读下一节之前，请花一分钟思考如何在 C# 中更好地实现上述要求，以及这些附加要求：

- 如果你试图做一些需求中不允许的事情，你会得到一个*编译时错误*，而不是运行时错误。例如，您必须创建一个设计，这样您甚至不能从空购物车中调用 `RemoveItem` 方法。
- 购物车在任何状态下的内容都应该是不可变的。这样做的好处是，如果我正在为购物车付款，即使其他流程同时添加或删除项目，购物车的内容也不会更改。

## F# 中的正确设计

让我们退一步，看看我们是否能想出一个更好的设计。看看这些要求，很明显，我们有一个简单的状态机，有三个状态和一些状态转换：

- 购物车可以是空的（Empty）、活动的（Active）或已付款的（PaidFor）
- 当您将商品添加到空（Empty）购物车时，它将变为活动状态（Active）
- 当您从活动（Active）购物车中删除最后一件商品时，它将变为空（Empty）
- 当您为 Active 购物车付款时，它将变为 PaidFor

现在我们可以将业务规则添加到此模型中：

- 您只能将商品添加到空或活动的购物车中
- 您只能从活动购物车中删除商品
- 您只能为处于活动状态的购物车付款

以下是状态图：



值得注意的是，这些面向状态的模型在业务系统中非常常见。产品开发、客户关系管理、订单处理和其他工作流通常可以用这种方式建模。

现在我们有了设计，我们可以用 F# 复制它：

```F#
type CartItem = string    // placeholder for a more complicated type

type EmptyState = NoItems // don't use empty list! We want to
                          // force clients to handle this as a
                          // separate case. E.g. "you have no
                          // items in your cart"

type ActiveState = { UnpaidItems : CartItem list; }
type PaidForState = { PaidItems : CartItem list;
                      Payment : decimal}

type Cart =
    | Empty of EmptyState
    | Active of ActiveState
    | PaidFor of PaidForState
```

我们为每个状态创建一个类型，`Cart` 类型是任何一个状态的选择。我给所有东西都起了一个不同的名字（例如 `PaidItems` 和 `UnpaidItems`，而不仅仅是 `Items`），因为这有助于推理引擎，并使代码更加自文档化。

> 这是一个比前面的例子长得多的例子！现在不要太担心 F# 语法，但我希望你能理解代码的要点，看看它如何融入整体设计。
>
> 此外，请将片段粘贴到脚本文件中，并在出现时自行评估。

接下来，我们可以为每个状态创建操作。需要注意的是，每次操作都会将其中一个状态作为输入，并返回一个新的购物车。也就是说，你从一个特定的已知状态开始，但你返回了一个 `Cart`，它是三种可能状态的包装。

```F#
// =============================
// operations on empty state
// =============================

let addToEmptyState item =
   // returns a new Active Cart
   Cart.Active {UnpaidItems=[item]}

// =============================
// operations on active state
// =============================

let addToActiveState state itemToAdd =
   let newList = itemToAdd :: state.UnpaidItems
   Cart.Active {state with UnpaidItems=newList }

let removeFromActiveState state itemToRemove =
   let newList = state.UnpaidItems
                 |> List.filter (fun i -> i<>itemToRemove)

   match newList with
   | [] -> Cart.Empty NoItems
   | _ -> Cart.Active {state with UnpaidItems=newList}

let payForActiveState state amount =
   // returns a new PaidFor Cart
   Cart.PaidFor {PaidItems=state.UnpaidItems; Payment=amount}
```

接下来，我们将操作作为方法附加到状态

```F#
type EmptyState with
   member this.Add = addToEmptyState

type ActiveState with
   member this.Add = addToActiveState this
   member this.Remove = removeFromActiveState this
   member this.Pay = payForActiveState this
```

我们也可以创建一些购物车级别的辅助方法。在购物车级别，我们必须通过 `match..with` 表达式来明确处理内部状态的每种可能性。

```F#
let addItemToCart cart item =
   match cart with
   | Empty state -> state.Add item
   | Active state -> state.Add item
   | PaidFor state ->
       printfn "ERROR: The cart is paid for"
       cart

let removeItemFromCart cart item =
   match cart with
   | Empty state ->
      printfn "ERROR: The cart is empty"
      cart   // return the cart
   | Active state ->
      state.Remove item
   | PaidFor state ->
      printfn "ERROR: The cart is paid for"
      cart   // return the cart

let displayCart cart  =
   match cart with
   | Empty state ->
      printfn "The cart is empty"   // can't do state.Items
   | Active state ->
      printfn "The cart contains %A unpaid items"
                                                state.UnpaidItems
   | PaidFor state ->
      printfn "The cart contains %A paid items. Amount paid: %f"
                                    state.PaidItems state.Payment

type Cart with
   static member NewCart = Cart.Empty NoItems
   member this.Add = addItemToCart this
   member this.Remove = removeItemFromCart this
   member this.Display = displayCart this
```

> 如果你觉得这篇文章很有趣，看看我的《领域建模函数式》一书！这是对领域驱动设计、类型建模和函数式编程的一个很好的介绍。

## 测试设计

现在让我们练习这段代码：

```F#
let emptyCart = Cart.NewCart
printf "emptyCart="; emptyCart.Display

let cartA = emptyCart.Add "A"
printf "cartA="; cartA.Display
```

我们现在有一个活动购物车，里面有一个商品。请注意，“`cartA`”是一个与“`emptyCart`”完全不同的对象，并且处于不同的状态。

让我们继续前进：

```F#
let cartAB = cartA.Add "B"
printf "cartAB="; cartAB.Display

let cartB = cartAB.Remove "A"
printf "cartB="; cartB.Display

let emptyCart2 = cartB.Remove "B"
printf "emptyCart2="; emptyCart2.Display
```

到目前为止，一切顺利。所有这些都是处于不同状态的不同对象，

让我们测试一下不能从空购物车中删除商品的要求：

```F#
let emptyCart3 = emptyCart2.Remove "B"    //error
printf "emptyCart3="; emptyCart3.Display
```

一个错误——正是我们想要的！

现在假设我们想为购物车付款。我们没有在 Cart 级别创建此方法，因为我们不想告诉客户如何处理所有情况。此方法仅适用于活动状态，因此客户端必须显式处理每种情况，并且仅在活动状态匹配时调用 `Pay` 方法。

首先，我们将尝试支付 cartA 费用。

```F#
//  try to pay for cartA
let cartAPaid =
    match cartA with
    | Empty _ | PaidFor _ -> cartA
    | Active state -> state.Pay 100m
printf "cartAPaid="; cartAPaid.Display
```

结果是一辆付费购物车。

现在，我们将尝试为 emptyCart 付款。

```F#
//  try to pay for emptyCart
let emptyCartPaid =
    match emptyCart with
    | Empty _ | PaidFor _ -> emptyCart
    | Active state -> state.Pay 100m
printf "emptyCartPaid="; emptyCartPaid.Display
```

什么都没发生。购物车为空，因此不会调用 Active 分支。我们可能想在其他分支中引发错误或记录消息，但无论我们做什么，都不能意外地调用空购物车上的 `Pay` 方法，因为该状态没有可调用的方法！

如果我们不小心试图为已经付款的购物车付款，也会发生同样的事情。

```F#
//  try to pay for cartAB
let cartABPaid =
    match cartAB with
    | Empty _ | PaidFor _ -> cartAB // return the same cart
    | Active state -> state.Pay 100m

//  try to pay for cartAB again
let cartABPaidAgain =
    match cartABPaid with
    | Empty _ | PaidFor _ -> cartABPaid  // return the same cart
    | Active state -> state.Pay 100m
```

你可能会说，上面的客户端代码可能不能代表现实世界中的代码——它表现良好，已经处理了需求。

那么，如果我们有编写糟糕或恶意的客户端代码试图强制付款，会发生什么：

```F#
match cartABPaid with
| Empty state -> state.Pay 100m
| PaidFor state -> state.Pay 100m
| Active state -> state.Pay 100m
```

如果我们试图这样强制它，我们会得到编译错误。客户端不可能创建不符合要求的代码。

## 摘要

我们设计了一个简单的购物车模型，它比 C# 设计有很多好处。

- 它非常清楚地符合要求。这个 API 的客户端不可能调用不符合要求的代码。
- 使用状态意味着可能的代码路径的数量比 C# 版本少得多，因此需要编写的单元测试要少得多。
- 每个函数都很简单，可能第一次就能工作，因为与 C# 版本不同，任何地方都没有条件语句。

> **对原始 C# 代码的分析**
>
> 现在您已经看到了 F# 代码，我们可以用新的眼光重新审视原始的 C# 代码。如果你想知道，以下是我对 C# 购物车示例设计问题的看法。
>
> *未满足要求*：空购物车仍可付款。
>
> *主要设计缺陷*：将支付金额作为 IsPaidFor 的信号，意味着零支付金额永远无法锁定购物车。你确定永远不可能有一辆付费但免费的购物车吗？要求不明确，但如果这后来成为要求呢？需要更改多少代码？
>
> *轻微的设计缺陷*：当试图从空购物车中删除商品时，会发生什么？当试图为已经付款的购物车付款时，应该怎么办？在这些情况下，我们应该抛出异常，还是只是默默地忽略它们？客户应该能够枚举空购物车中的商品，这有意义吗？而且这并不像设计的那样是线程安全的；那么，如果在主线程上进行付款时，辅助线程将商品添加到购物车中，会发生什么？
>
> 这有很多事情要担心。
>
> F# 设计的好处是这些问题都不可能存在。因此，以这种方式设计不仅可以确保正确的代码，还可以真正减少确保设计一开始就是防弹的认知努力。
>
> *编译时检查*：最初的 C# 设计将所有状态和转换混合在一个类中，这使得它非常容易出错。一个更好的方法是创建单独的状态类（比如一个公共基类），这可以降低复杂性，但缺乏内置的“联合”类型意味着你无法静态验证代码的正确性。在 C# 中有很多方法可以实现“联合”类型，但这根本不是惯用的，而在 F# 中则很常见。

## 附录：正确解决方案的 C# 代码

当在 C# 中遇到这些要求时，您可能会立即想到——只需创建一个接口！

但这并不像你想象的那么容易。我写了一篇后续文章来解释原因：C# 中的购物车示例。

如果你有兴趣看看解决方案的 C# 代码是什么样子的，请看下面。此代码满足上述要求，并保证编译时的正确性。

需要注意的关键是，由于 C# 没有联合类型，因此实现使用了一个“fold”函数，该函数有三个函数参数，每个状态一个。要使用购物车，调用者传入一组三个 lambdas，（隐藏）状态决定会发生什么。

```c#
var paidCart = cartA.Do(
    // lambda for Empty state
    state => cartA,
    // lambda for Active state
    state => state.Pay(100),
    // lambda for Paid state
    state => cartA);
```

这种方法意味着调用者永远不能调用“错误”的函数，例如空状态的“Pay”，因为 lambda 的参数不支持它。试试看！

```c#
using System;
using System.Collections.Generic;
using System.Linq;

namespace WhyUseFsharp
{

    public class ShoppingCart<TItem>
    {

        #region ShoppingCart State classes

        /// <summary>
        /// Represents the Empty state
        /// </summary>
        public class EmptyState
        {
            public ShoppingCart<TItem> Add(TItem item)
            {
                var newItems = new[] { item };
                var newState = new ActiveState(newItems);
                return FromState(newState);
            }
        }

        /// <summary>
        /// Represents the Active state
        /// </summary>
        public class ActiveState
        {
            public ActiveState(IEnumerable<TItem> items)
            {
                Items = items;
            }

            public IEnumerable<TItem> Items { get; private set; }

            public ShoppingCart<TItem> Add(TItem item)
            {
                var newItems = new List<TItem>(Items) {item};
                var newState = new ActiveState(newItems);
                return FromState(newState);
            }

            public ShoppingCart<TItem> Remove(TItem item)
            {
                var newItems = new List<TItem>(Items);
                newItems.Remove(item);
                if (newItems.Count > 0)
                {
                    var newState = new ActiveState(newItems);
                    return FromState(newState);
                }
                else
                {
                    var newState = new EmptyState();
                    return FromState(newState);
                }
            }

            public ShoppingCart<TItem> Pay(decimal amount)
            {
                var newState = new PaidForState(Items, amount);
                return FromState(newState);
            }


        }

        /// <summary>
        /// Represents the Paid state
        /// </summary>
        public class PaidForState
        {
            public PaidForState(IEnumerable<TItem> items, decimal amount)
            {
                Items = items.ToList();
                Amount = amount;
            }

            public IEnumerable<TItem> Items { get; private set; }
            public decimal Amount { get; private set; }
        }

        #endregion ShoppingCart State classes

        //====================================
        // Execute of shopping cart proper
        //====================================

        private enum Tag { Empty, Active, PaidFor }
        private readonly Tag _tag = Tag.Empty;
        private readonly object _state;       //has to be a generic object

        /// <summary>
        /// Private ctor. Use FromState instead
        /// </summary>
        private ShoppingCart(Tag tagValue, object state)
        {
            _state = state;
            _tag = tagValue;
        }

        public static ShoppingCart<TItem> FromState(EmptyState state)
        {
            return new ShoppingCart<TItem>(Tag.Empty, state);
        }

        public static ShoppingCart<TItem> FromState(ActiveState state)
        {
            return new ShoppingCart<TItem>(Tag.Active, state);
        }

        public static ShoppingCart<TItem> FromState(PaidForState state)
        {
            return new ShoppingCart<TItem>(Tag.PaidFor, state);
        }

        /// <summary>
        /// Create a new empty cart
        /// </summary>
        public static ShoppingCart<TItem> NewCart()
        {
            var newState = new EmptyState();
            return FromState(newState);
        }

        /// <summary>
        /// Call a function for each case of the state
        /// </summary>
        /// <remarks>
        /// Forcing the caller to pass a function for each possible case means that all cases are handled at all times.
        /// </remarks>
        public TResult Do<TResult>(
            Func<EmptyState, TResult> emptyFn,
            Func<ActiveState, TResult> activeFn,
            Func<PaidForState, TResult> paidForyFn
            )
        {
            switch (_tag)
            {
                case Tag.Empty:
                    return emptyFn(_state as EmptyState);
                case Tag.Active:
                    return activeFn(_state as ActiveState);
                case Tag.PaidFor:
                    return paidForyFn(_state as PaidForState);
                default:
                    throw new InvalidOperationException(string.Format("Tag {0} not recognized", _tag));
            }
        }

        /// <summary>
        /// Do an action without a return value
        /// </summary>
        public void Do(
            Action<EmptyState> emptyFn,
            Action<ActiveState> activeFn,
            Action<PaidForState> paidForyFn
            )
        {
            //convert the Actions into Funcs by returning a dummy value
            Do(
                state => { emptyFn(state); return 0; },
                state => { activeFn(state); return 0; },
                state => { paidForyFn(state); return 0; }
                );
        }



    }

    /// <summary>
    /// Extension methods for my own personal library
    /// </summary>
    public static class ShoppingCartExtension
    {
        /// <summary>
        /// Helper method to Add
        /// </summary>
        public static ShoppingCart<TItem> Add<TItem>(this ShoppingCart<TItem> cart, TItem item)
        {
            return cart.Do(
                state => state.Add(item), //empty case
                state => state.Add(item), //active case
                state => { Console.WriteLine("ERROR: The cart is paid for and items cannot be added"); return cart; } //paid for case
            );
        }

        /// <summary>
        /// Helper method to Remove
        /// </summary>
        public static ShoppingCart<TItem> Remove<TItem>(this ShoppingCart<TItem> cart, TItem item)
        {
            return cart.Do(
                state => { Console.WriteLine("ERROR: The cart is empty and items cannot be removed"); return cart; }, //empty case
                state => state.Remove(item), //active case
                state => { Console.WriteLine("ERROR: The cart is paid for and items cannot be removed"); return cart; } //paid for case
            );
        }

        /// <summary>
        /// Helper method to Display
        /// </summary>
        public static void Display<TItem>(this ShoppingCart<TItem> cart)
        {
            cart.Do(
                state => Console.WriteLine("The cart is empty"),
                state => Console.WriteLine("The active cart contains {0} items", state.Items.Count()),
                state => Console.WriteLine("The paid cart contains {0} items. Amount paid {1}", state.Items.Count(), state.Amount)
            );
        }
    }

    [NUnit.Framework.TestFixture]
    public class CorrectShoppingCartTest
    {
        [NUnit.Framework.Test]
        public void TestCart()
        {
            var emptyCart = ShoppingCart<string>.NewCart();
            emptyCart.Display();

            var cartA = emptyCart.Add("A");  //one item
            cartA.Display();

            var cartAb = cartA.Add("B");  //two items
            cartAb.Display();

            var cartB = cartAb.Remove("A"); //one item
            cartB.Display();

            var emptyCart2 = cartB.Remove("B"); //empty
            emptyCart2.Display();

            Console.WriteLine("Removing from emptyCart");
            emptyCart.Remove("B"); //error


            //  try to pay for cartA
            Console.WriteLine("paying for cartA");
            var paidCart = cartA.Do(
                state => cartA,
                state => state.Pay(100),
                state => cartA);
            paidCart.Display();

            Console.WriteLine("Adding to paidCart");
            paidCart.Add("C");

            //  try to pay for emptyCart
            Console.WriteLine("paying for emptyCart");
            var emptyCartPaid = emptyCart.Do(
                state => emptyCart,
                state => state.Pay(100),
                state => emptyCart);
            emptyCartPaid.Display();
        }
    }
}
```

# 23 并发性

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/concurrency-intro/#series-toc)*)*

我们如何编写软件的下一次重大革命？
2012年4月23日 这篇文章已有3年多的历史了

https://fsharpforfunandprofit.com/posts/concurrency-intro/

如今，我们听到了很多关于并发性的话题，它有多重要，以及它是如何“我们编写软件的下一次重大革命”的。

那么，我们所说的“并发性”到底是什么意思，F# 如何提供帮助？

并发性最简单的定义就是“几件事同时发生，可能还会相互作用”。这似乎是一个微不足道的定义，但关键在于，大多数计算机程序（和语言）都是为串行工作而设计的，一次只处理一件事，并且没有足够的能力处理并发性。

即使计算机程序是为了处理并发性而编写的，也有一个更严重的问题：我们的大脑在考虑并发性时表现不佳。众所周知，编写处理并发性的代码非常困难。或者我应该说，编写正确的并发代码是极其困难的！编写有缺陷的并发代码非常容易；可能存在竞争条件，或者操作可能不是原子性的，或者任务可能被不必要地饥饿或阻塞，通过查看代码或使用调试器很难发现这些问题。

在讨论 F# 的细节之前，让我们尝试对开发人员必须处理的一些常见类型的并发场景进行分类：

- **“并发多任务处理”**。当我们直接控制多个并发任务（如进程或线程）时，我们希望它们能够相互通信并安全地共享数据。
- **“异步”编程**。这是我们与直接控制之外的单独系统发起对话，然后等待它返回给我们的时候。常见的情况是与文件系统、数据库或网络对话。这些情况通常是 I/O 受限的，所以你想在等待的时候做一些其他有用的事情。这些类型的任务通常也是非确定性的，这意味着运行同一程序两次可能会产生不同的结果。
- **“并行”编程**。这是当我们有一个任务，我们想将其拆分为独立的子任务，然后并行运行子任务时，最好使用所有可用的内核或 CPU。这些情况通常是 CPU 受限的。与异步任务不同，并行性通常是确定的，因此运行同一程序两次将得到相同的结果。
- **“反应式”编程**。这是我们自己不主动发起任务，而是专注于倾听事件，然后尽快处理的时候。这种情况发生在设计服务器和使用用户界面时。

当然，这些都是模糊的定义，在实践中是重叠的。不过，一般来说，对于所有这些情况，解决这些场景的实际实现往往使用两种不同的方法：

- 如果有许多不同的任务需要共享状态或资源而无需等待，那么请使用“缓冲异步”设计。
- 如果有很多相同的任务不需要共享状态，那么使用“fork/join”或“分而治之”方法使用并行任务。

## F#并发编程工具

F# 提供了许多不同的并发代码编写方法：

- 对于多任务处理和异步问题，F# 可以直接使用所有常用.NET 怀疑(suspects)的方法，如 `Thread` `AutoResetEvent`, `BackgroundWorker` 和 `IAsyncResult`。但它也为所有类型的异步 IO 和后台任务管理提供了一个更简单的模型，称为“异步工作流”。我们将在下一篇文章中介绍这些。
- 异步问题的另一种方法是使用消息队列和“参与者模型（actor model）”（这是上面提到的“缓冲异步”设计）。F# 有一个名为 `MailboxProcessor` 的 actor 模型的内置实现。我非常支持使用参与者和消息队列进行设计，因为它将各种组件解耦，并允许您连续思考每个组件。
- 为了实现真正的 CPU 并行性，F# 具有基于上述异步工作流构建的方便的库代码，它还可以使用 .NET 任务并行库。
- 最后，事件处理和响应式编程的函数式方法与传统方法截然不同。函数式方法将事件视为“流”，可以像 LINQ 处理集合一样进行过滤、拆分和组合。F# 内置了对该模型以及标准事件驱动模型的支持。

# 24 异步编程

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/concurrency-async-and-parallel/#series-toc)*)*

用Async类封装后台任务
24 Apr 2012这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/concurrency-async-and-parallel/

在这篇文章中，我们将看看用 F# 编写异步代码的几种方法，以及一个非常简短的并行性示例。

## 传统异步编程

如前一篇文章所述，F# 可以直接使用所有常用的 .NET 怀疑，如 `Thread` `AutoResetEvent`, `BackgroundWorker` 和 `IAsyncResult`。

让我们来看一个简单的例子，我们等待计时器事件发生：

```F#
open System

let userTimerWithCallback =
    // create an event to wait on
    let event = new System.Threading.AutoResetEvent(false)

    // create a timer and add an event handler that will signal the event
    let timer = new System.Timers.Timer(2000.0)
    timer.Elapsed.Add (fun _ -> event.Set() |> ignore )

    //start
    printfn "Waiting for timer at %O" DateTime.Now.TimeOfDay
    timer.Start()

    // keep working
    printfn "Doing something useful while waiting for event"

    // block on the timer via the AutoResetEvent
    event.WaitOne() |> ignore

    //done
    printfn "Timer ticked at %O" DateTime.Now.TimeOfDay
```

这显示了 `AutoResetEvent` 作为同步机制的使用。

- lambda 已在通过 `Timer.Elapsed` 事件注册，当事件触发时，会发出自动重置事件的信号。
- 主线程启动计时器，在等待时执行其他操作，然后阻塞，直到事件被触发。
- 最后，大约 2 秒后，主线程继续。

上面的代码相当简单，但确实需要您实例化一个 AutoResetEvent，如果 lambda 定义不正确，可能会出现错误。

## 引入异步工作流

F# 有一个名为“异步工作流”的内置构造，使异步代码更容易编写。这些工作流是封装后台任务的对象，并提供了许多有用的操作来管理它们。

以下是重写为使用一个的前一个示例：

```F#
open System
//open Microsoft.FSharp.Control  // Async.* is in this module.

let userTimerWithAsync =

    // create a timer and associated async event
    let timer = new System.Timers.Timer(2000.0)
    let timerEvent = Async.AwaitEvent (timer.Elapsed) |> Async.Ignore

    // start
    printfn "Waiting for timer at %O" DateTime.Now.TimeOfDay
    timer.Start()

    // keep working
    printfn "Doing something useful while waiting for event"

    // block on the timer event now by waiting for the async to complete
    Async.RunSynchronously timerEvent

    // done
    printfn "Timer ticked at %O" DateTime.Now.TimeOfDay
```

以下是变化：

- `AutoResetEvent` 和 lambda 已消失，并被 `let timerEvent = Control.Async.AwaitEvent (timer.Elapsed)` 替换。它直接从事件创建 `async` 对象，不需要 lambda。添加 `ignore` 以忽略结果。
- `event.WaitOne()` 已被 `Async.RunSynchronously timerEvent` 替换，它会阻塞异步对象，直到它完成。

就是这样。既简单又容易理解。

异步工作流还可以与 `IAsyncResult`，开始/结束对和其他标准一起使用 .NET 方法。

例如，您可以通过包装从 `BeginWrite` 生成的 `IAsyncResult` 来执行异步文件写入。

```F#
let fileWriteWithAsync =

    // create a stream to write to
    use stream = new System.IO.FileStream("test.txt",System.IO.FileMode.Create)

    // start
    printfn "Starting async write"
    let asyncResult = stream.BeginWrite(Array.empty,0,0,null,null)

	// create an async wrapper around an IAsyncResult
    let async = Async.AwaitIAsyncResult(asyncResult) |> Async.Ignore

    // keep working
    printfn "Doing something useful while waiting for write to complete"

    // block on the timer now by waiting for the async to complete
    Async.RunSynchronously async

    // done
    printfn "Async write completed"
```

## 创建和嵌套异步工作流

异步工作流也可以手动创建。使用 `async` 关键字和花括号创建了一个新的工作流。大括号包含一组要在后台执行的表达式。

这个简单的工作流只会休眠 2 秒。

```F#
let sleepWorkflow  = async{
    printfn "Starting sleep workflow at %O" DateTime.Now.TimeOfDay
    do! Async.Sleep 2000
    printfn "Finished sleep workflow at %O" DateTime.Now.TimeOfDay
    }

Async.RunSynchronously sleepWorkflow
```

*注意：代码 `do! Async.Sleep 2000` 类似于 `Thread.Sleep`，但设计用于异步工作流程。*

工作流可以包含嵌套在其中的其他异步工作流。在大括号内，可以使用 `let!` 语法来阻塞嵌套的工作流。

```F#
let nestedWorkflow  = async{

    printfn "Starting parent"
    let! childWorkflow = Async.StartChild sleepWorkflow

    // give the child a chance and then keep working
    do! Async.Sleep 100
    printfn "Doing something useful while waiting "

    // block on the child
    let! result = childWorkflow

    // done
    printfn "Finished parent"
    }

// run the whole workflow
Async.RunSynchronously nestedWorkflow
```

## 取消工作流

异步工作流的一个非常方便的地方是，它们支持内置的取消机制。不需要特殊代码。

考虑一个简单的任务，打印1到100的数字：

```F#
let testLoop = async {
    for i in [1..100] do
        // do something
        printf "%i before.." i

        // sleep a bit
        do! Async.Sleep 10
        printfn "..after"
    }
```

我们可以用通常的方式进行测试：

```F#
Async.RunSynchronously testLoop
```

现在，假设我们想在中途取消此任务。最好的方法是什么？

在 C# 中，我们必须创建标志来传递，然后经常检查它们，但在 F# 中，这种技术是内置的，使用 `CancellationToken` 类。

以下是我们如何取消任务的示例：

```F#
open System
open System.Threading

// create a cancellation source
use cancellationSource = new CancellationTokenSource()

// start the task, but this time pass in a cancellation token
Async.Start (testLoop,cancellationSource.Token)

// wait a bit
Thread.Sleep(200)

// cancel after 200ms
cancellationSource.Cancel()
```

在 F# 中，任何嵌套的异步调用都会自动检查取消令牌！

在这种情况下，这是一行：

```F#
do! Async.Sleep(10)
```

正如您从输出中看到的，此行是取消发生的地方。

## 串行和并行组合工作流

异步工作流的另一个有用之处是，它们可以很容易地以各种方式组合：串行和并行。

让我们再次创建一个只在给定时间内休眠的简单工作流：

```F#
// create a workflow to sleep for a time
let sleepWorkflowMs ms = async {
    printfn "%i ms workflow started" ms
    do! Async.Sleep ms
    printfn "%i ms workflow finished" ms
    }
```

这是一个将其中两个串联在一起的版本：

```F#
let workflowInSeries = async {
    let! sleep1 = sleepWorkflowMs 1000
    printfn "Finished one"
    let! sleep2 = sleepWorkflowMs 2000
    printfn "Finished two"
    }

#time
Async.RunSynchronously workflowInSeries
#time
```

这里有一个将这两个并行结合的版本：

```F#
// Create them
let sleep1 = sleepWorkflowMs 1000
let sleep2 = sleepWorkflowMs 2000

// run them in parallel
#time
[sleep1; sleep2]
    |> Async.Parallel
    |> Async.RunSynchronously
#time
```

> 注意：`#time` 命令可打开和关闭计时器。它仅在交互式窗口中工作，因此必须将此示例发送到交互式窗口才能正常工作。

我们使用 `#time` 选项来显示总运行时间，因为它们是并行运行的，所以总运行时间为 2 秒。如果他们以串联的方式进行，则需要 3 秒。

此外，您可能会看到输出有时会出现乱码，因为两个任务同时写入控制台！

最后一个示例是“fork/join”方法的经典示例，其中生成了许多子任务，然后父任务等待它们全部完成。正如你所看到的，F# 让这一切变得非常容易！

## 示例：异步web下载器

在这个更现实的例子中，我们将看到将一些现有代码从非异步风格转换为异步风格是多么容易，以及可以实现的相应性能提升。

所以这里有一个简单的 URL 下载器，与我们在系列文章开头看到的非常相似：

```F#
open System.Net
open System
open System.IO

let fetchUrl url =
    let req = WebRequest.Create(Uri(url))
    use resp = req.GetResponse()
    use stream = resp.GetResponseStream()
    use reader = new IO.StreamReader(stream)
    let html = reader.ReadToEnd()
    printfn "finished downloading %s" url
```

这里有一些代码来计时：

```F#
// a list of sites to fetch
let sites = ["http://www.bing.com";
             "http://www.google.com";
             "http://www.microsoft.com";
             "http://www.amazon.com";
             "http://www.yahoo.com"]

#time                     // turn interactive timer on
sites                     // start with the list of sites
|> List.map fetchUrl      // loop through each site and download
#time                     // turn timer off
```

记下所花费的时间，让我们看看是否可以改进它！

显然，上面的例子效率低下——一次只访问一个网站。如果我们能同时访问它们，程序会更快。

那么，我们如何将其转换为并发算法呢？逻辑大致如下：

- 为我们正在下载的每个网页创建一个任务，然后对于每个任务，下载逻辑如下：
  - 开始从网站下载页面。在这种情况下，暂停一下，让其他任务轮流进行。
  - 下载完成后，唤醒并继续执行任务
- 最后，启动所有任务，让他们去做！

不幸的是，在标准的类 C 语言中很难做到这一点。例如，在 C# 中，您必须为异步任务完成时创建一个回调。管理这些回调是痛苦的，并且会创建大量额外的支持代码，从而妨碍理解逻辑。对此有一些优雅的解决方案，但总的来说，C# 并发编程的信噪比非常高*。

*截至撰写本文时。C# 的未来版本将有 `await` 关键字，这与 F# 现在的关键字类似。

但正如你所料，F# 让这变得容易。以下是下载器代码的并发 F# 版本：

```F#
open Microsoft.FSharp.Control.CommonExtensions
                                        // adds AsyncGetResponse

// Fetch the contents of a web page asynchronously
let fetchUrlAsync url =
    async {
        let req = WebRequest.Create(Uri(url))
        use! resp = req.AsyncGetResponse()  // new keyword "use!"
        use stream = resp.GetResponseStream()
        use reader = new IO.StreamReader(stream)
        let html = reader.ReadToEnd()
        printfn "finished downloading %s" url
        }
```

请注意，新代码看起来几乎与原始代码完全相同。只有一些小的变化。

- 从“`use resp = `”到“`use! resp =`“的更改正是我们上面讨论的更改——在异步操作进行的同时，让其他任务轮流执行。
- 我们还使用了 `CommonExtensions` 命名空间中定义的 `AsyncGetResponse` 扩展方法。这将返回一个异步工作流，我们可以将其嵌套在主工作流中。
- 此外，整个步骤集都包含在“`async {…}`”包装器中，该包装器将其转换为可以异步运行的块。

这是一个使用异步版本的定时下载。

```F#
// a list of sites to fetch
let sites = ["http://www.bing.com";
             "http://www.google.com";
             "http://www.microsoft.com";
             "http://www.amazon.com";
             "http://www.yahoo.com"]

#time                      // turn interactive timer on
sites
|> List.map fetchUrlAsync  // make a list of async tasks
|> Async.Parallel          // set up the tasks to run in parallel
|> Async.RunSynchronously  // start them off
#time                      // turn timer off
```

其工作原理如下：

- `fetchUrlAsync` 应用于每个站点。它不会立即开始下载，但会返回一个异步工作流供以后运行。
- 为了设置所有任务同时运行，我们使用 `Async.Parallel` 函数
- 最后我们调用 `Async.RunSynchronously` 以启动所有任务，并等待它们全部停止。

如果你自己尝试这段代码，你会发现异步版本比同步版本快得多。对于一些小的代码更改来说还不错！最重要的是，底层逻辑仍然非常清晰，没有被噪音所干扰。

## 示例：并行计算

最后，让我们再次快速浏览一下并行计算。

在我们开始之前，我应该警告你，下面的示例代码只是为了演示基本原理。像这样的并行化“玩具”版本的基准测试没有意义，因为任何一种真正的并发代码都有如此多的依赖关系。

还要注意，并行化很少是加速代码的最佳方式。你的时间几乎总是最好花在改进算法上。我随时都会用我的串行版本的 quicksort 和你的并行版本的 bubblesort 打赌！（有关如何提高性能的更多详细信息，请参阅优化系列）

不管怎样，有了这个警告，让我们创建一个消耗一些 CPU 的小任务。我们将对其进行串行和并行测试。

```F#
let childTask() =
    // chew up some CPU.
    for i in [1..1000] do
        for i in [1..1000] do
            do "Hello".Contains("H") |> ignore
            // we don't care about the answer!

// Test the child task on its own.
// Adjust the upper bounds as needed
// to make this run in about 0.2 sec
#time
childTask()
#time
```

根据需要调整循环的上限，使其在大约 0.2 秒内运行。

现在，让我们将这些组合成一个串行任务（使用组合），并用计时器进行测试：

```F#
let parentTask =
    childTask
    |> List.replicate 20
    |> List.reduce (>>)

//test
#time
parentTask()
#time
```

这大约需要 4 秒。

现在，为了使 `childTask` 可并行化，我们必须将其封装在 `async` 中：

```F#
let asyncChildTask = async { return childTask() }
```

为了将一堆异步组合成一个并行任务，我们使用 `Async.Parallel`。

让我们测试一下并比较一下时间：

```F#
let asyncParentTask =
    asyncChildTask
    |> List.replicate 20
    |> Async.Parallel

//test
#time
asyncParentTask
|> Async.RunSynchronously
#time
```

在双核机器上，并行版本的速度大约快 50%。当然，它会随着内核或 CPU 数量的增加而变得更快，但速度会降低。四个核心会比一个核心快，但不会快四倍。

另一方面，与异步 web 下载示例一样，一些微小的代码更改可以产生很大的影响，同时仍然使代码易于阅读和理解。因此，在并行性真正有帮助的情况下，很高兴知道它很容易安排。

# 25 消息和代理

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/concurrency-actor-model/#series-toc)*)*

更容易思考并发性
25 Apr 2012 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/concurrency-actor-model/

在这篇文章中，我们将研究基于消息（或基于参与者，actor-based）的并发方法。

在这种方法中，当一个任务想要与另一个任务通信时，它会向其发送消息，而不是直接联系它。消息被放入队列，接收任务（称为“参与者（actor）”或“代理（agent）”）一次从队列中提取一条消息进行处理。

这种基于消息的方法已应用于许多情况，从低级网络套接字（基于 TCP/IP 构建）到企业范围的应用程序集成系统（例如 RabbitMQ 或 IBM WebSphere MQ）。

从软件设计的角度来看，基于消息的方法有很多好处：

- 您可以在没有锁的情况下管理共享数据和资源。
- 你可以很容易地遵循“单一责任原则”，因为每个代理都可以被设计为只做一件事。
- 它鼓励一种“管道”编程模型，其中“生产者”向解耦的“消费者”发送消息，这还有其他好处：
  - 队列充当缓冲区，消除了客户端的等待。
  - 根据需要扩大队列的一侧或另一侧以最大限度地提高吞吐量是很简单的。
  - 可以优雅地处理错误，因为解耦意味着可以在不影响客户端的情况下创建和销毁代理。

从实际开发人员的角度来看，我发现基于消息的方法最吸引人的地方是，在为任何给定的参与者编写代码时，你不必考虑并发性来伤害你的大脑。消息队列强制对可能同时发生的操作进行“序列化”。这反过来又使思考（并编写代码）处理消息的逻辑变得更加容易，因为您可以确信您的代码将与可能中断您的流的其他事件隔离开来。

有了这些优势，当爱立信内部的一个团队想要设计一种用于编写高度并发电话应用程序的编程语言时，他们使用基于消息的方法创建了一种语言，即 Erlang，这并不奇怪。Erlang 现在已经成为整个主题的典型代表，并引起了人们对在其他语言中实现相同方法的极大兴趣。

## F# 如何实现基于消息的方法

F# 有一个名为 `MailboxProcessor` 的内置代理类。与线程相比，这些代理非常轻量级——您可以同时实例化数万个代理。

这些类似于 Erlang 中的代理，但与 Erlang 不同，它们不跨进程边界工作，只在同一进程中工作。与 RabbitMQ 等重量级排队系统不同，消息不是持久的。如果你的应用程序崩溃，消息就会丢失。

但这些都是小问题，可以解决。在未来的系列文章中，我将介绍消息队列的替代实现。基本方法在所有情况下都是一样的。

让我们看看 F# 中的一个简单代理实现：

```F#
let printerAgent = MailboxProcessor.Start(fun inbox->

    // the message processing function
    let rec messageLoop() = async{

        // read a message
        let! msg = inbox.Receive()

        // process a message
        printfn "message is: %s" msg

        // loop to top
        return! messageLoop()
        }

    // start the loop
    messageLoop()
    )
```

`MailboxProcessor.Start` 函数接受一个简单的函数参数。该函数永远循环，从队列（或“收件箱”）读取消息并对其进行处理。

以下是使用中的示例：

```F#
// test it
printerAgent.Post "hello"
printerAgent.Post "hello again"
printerAgent.Post "hello a third time"
```

在本文的其余部分，我们将看看两个稍微有用的例子：

- 无锁管理共享状态
- 对共享 IO 的序列化和缓冲访问

在这两种情况下，基于消息的并发方法都是优雅、高效且易于编程的。

## 管理共享状态

让我们先看看共享状态问题。

一种常见的情况是，您有一些状态需要由多个并发任务或线程访问和更改。我们将使用一个非常简单的案例，并说明要求如下：

- 一个共享的“计数器”和“总和”，可以由多个任务同时递增。
- 计数器和总和的更改必须是原子性的——我们必须保证它们将同时更新。

### 共享状态的锁定方法

对于这些需求，使用锁或互斥是一种常见的解决方案，所以让我们使用锁编写一些代码，看看它是如何执行的。

首先，让我们编写一个静态的 `LockedCounter` 类，用锁保护状态。

```F#
open System
open System.Threading
open System.Diagnostics

// a utility function
type Utility() =
    static let rand = Random()

    static member RandomSleep() =
        let ms = rand.Next(1,10)
        Thread.Sleep ms

// an implementation of a shared counter using locks
type LockedCounter () =

    static let _lock = Object()

    static let mutable count = 0
    static let mutable sum = 0

    static let updateState i =
        // increment the counters and...
        sum <- sum + i
        count <- count + 1
        printfn "Count is: %i. Sum is: %i" count sum

        // ...emulate a short delay
        Utility.RandomSleep()


    // public interface to hide the state
    static member Add i =
        // see how long a client has to wait
        let stopwatch = Stopwatch()
        stopwatch.Start()

        // start lock. Same as C# lock{...}
        lock _lock (fun () ->

            // see how long the wait was
            stopwatch.Stop()
            printfn "Client waited %i" stopwatch.ElapsedMilliseconds

            // do the core logic
            updateState i
            )
        // release lock
```

关于此代码的一些注意事项：

- 这段代码是使用一种非常命令式的方法编写的，其中包含可变变量和锁
- 公共 `Add` 方法具有显式 `Monitor.Enter` 和 `Monitor.Exit` 表达式以获取和释放锁。这与 C# 中的 `lock{…}` 语句相同。
- 我们还添加了一个秒表来测量客户需要等待多长时间才能拿到锁。
- 核心的“业务逻辑”是 `updateState` 方法，它不仅更新状态，还添加了一个小的随机等待，以模拟执行处理所需的时间。

让我们单独测试一下：

```F#
// test in isolation
LockedCounter.Add 4
LockedCounter.Add 5
```

接下来，我们将创建一个尝试访问计数器的任务：

```F#
let makeCountingTask addFunction taskId  = async {
    let name = sprintf "Task%i" taskId
    for i in [1..3] do
        addFunction i
    }

// test in isolation
let task = makeCountingTask LockedCounter.Add 1
Async.RunSynchronously task
```

在这种情况下，当根本没有争用时，等待时间都是 0。

但是，当我们创建 10 个子任务，所有子任务都试图同时访问计数器时会发生什么：

```F#
let lockedExample5 =
    [1..10]
        |> List.map (fun i -> makeCountingTask LockedCounter.Add i)
        |> Async.Parallel
        |> Async.RunSynchronously
        |> ignore
```

哦，天哪！大多数任务现在都要等一段时间。如果两个任务想同时更新状态，一个必须等待另一个的工作完成，然后才能完成自己的工作，这会影响性能。

如果我们添加越来越多的任务，争用就会增加，任务将花费越来越多的时间等待而不是工作。

### 基于消息的共享状态方法

让我们看看消息队列如何帮助我们。这是基于消息的版本：

```F#
type MessageBasedCounter () =

    static let updateState (count,sum) msg =

        // increment the counters and...
        let newSum = sum + msg
        let newCount = count + 1
        printfn "Count is: %i. Sum is: %i" newCount newSum

        // ...emulate a short delay
        Utility.RandomSleep()

        // return the new state
        (newCount,newSum)

    // create the agent
    static let agent = MailboxProcessor.Start(fun inbox ->

        // the message processing function
        let rec messageLoop oldState = async{

            // read a message
            let! msg = inbox.Receive()

            // do the core logic
            let newState = updateState oldState msg

            // loop to top
            return! messageLoop newState
            }

        // start the loop
        messageLoop (0,0)
        )

    // public interface to hide the implementation
    static member Add i = agent.Post i
```

关于此代码的一些注意事项：

- 核心“业务逻辑”再次位于 `updateState` 方法中，该方法的实现与前面的示例几乎相同，除了状态是不可变的，因此创建了一个新的状态并将其返回给主循环。
- 代理读取消息（在这种情况下是简单的整数），然后调用 `updateState` 方法
- 公共方法 `Add` 向代理发布消息，而不是直接调用 `updateState` 方法
- 这段代码是以更实用的方式编写的；任何地方都没有可变变量和锁。事实上，根本没有处理并发性的代码！代码只需要关注业务逻辑，因此更容易理解。

让我们单独测试一下：

```F#
// test in isolation
MessageBasedCounter.Add 4
MessageBasedCounter.Add 5
```

接下来，我们将重用前面定义的任务，但改为调用 `MessageBasedCounter.Add`：

```F#
let task = makeCountingTask MessageBasedCounter.Add 1
Async.RunSynchronously task
```

最后，让我们创建5个子任务，尝试一次访问计数器。

```F#
let messageExample5 =
    [1..5]
        |> List.map (fun i -> makeCountingTask MessageBasedCounter.Add i)
        |> Async.Parallel
        |> Async.RunSynchronously
        |> ignore
```

我们无法测量客户的等待时间，因为没有！

## 共享 IO

访问共享 IO 资源（如文件）时也会出现类似的并发问题：

- 如果IO速度慢，即使没有锁，客户端也会花费大量时间等待。
- 如果多个线程同时写入资源，则可能会得到损坏的数据。

这两个问题都可以通过使用异步调用结合缓冲来解决——这正是消息队列所做的。

在下一个示例中，我们将考虑许多客户端将同时写入的日志服务的示例。（在这个简单的情况下，我们将直接写入Console。）

我们将首先看一个没有并发控制的实现，然后看一个使用消息队列序列化所有请求的实现。

### IO 无序列化

为了使损坏非常明显和可重复，让我们首先创建一个“慢速”控制台，在日志消息中写入每个单独的字符，并在每个字符之间暂停一毫秒。在这毫秒内，另一个线程也可能正在写入，导致消息的不必要交织。

```F#
let slowConsoleWrite msg =
    msg |> String.iter (fun ch->
        System.Threading.Thread.Sleep(1)
        System.Console.Write ch
        )

// test in isolation
slowConsoleWrite "abc"
```

接下来，我们将创建一个循环几次的简单任务，每次将其名称写入记录器：

```F#
let makeTask logger taskId = async {
    let name = sprintf "Task%i" taskId
    for i in [1..3] do
        let msg = sprintf "-%s:Loop%i-" name i
        logger msg
    }

// test in isolation
let task = makeTask slowConsoleWrite 1
Async.RunSynchronously task
```

接下来，我们编写一个日志类，封装对慢速控制台的访问。它没有锁定或序列化，基本上不是线程安全的：

```F#
type UnserializedLogger() =
    // interface
    member this.Log msg = slowConsoleWrite msg

// test in isolation
let unserializedLogger = UnserializedLogger()
unserializedLogger.Log "hello"
```

现在让我们把所有这些结合成一个真实的例子。我们将创建五个子任务并并行运行它们，所有子任务都试图写入慢速控制台。

```F#
let unserializedExample =
    let logger = UnserializedLogger()
    [1..5]
        |> List.map (fun i -> makeTask logger.Log i)
        |> Async.Parallel
        |> Async.RunSynchronously
        |> ignore
```

哎哟！输出非常混乱！

### 带有消息的序列化 IO

那么，当我们用封装消息队列的 `SerializedLogger` 类替换 `UnserializedLogger` 时会发生什么。

`SerializedLogger` 中的代理只是从其输入队列中读取消息并将其写入慢速控制台。同样，没有处理并发性的代码，也没有使用锁。

```F#
type SerializedLogger() =

    // create the mailbox processor
    let agent = MailboxProcessor.Start(fun inbox ->

        // the message processing function
        let rec messageLoop () = async{

            // read a message
            let! msg = inbox.Receive()

            // write it to the log
            slowConsoleWrite msg

            // loop to top
            return! messageLoop ()
            }

        // start the loop
        messageLoop ()
        )

    // public interface
    member this.Log msg = agent.Post msg

// test in isolation
let serializedLogger = SerializedLogger()
serializedLogger.Log "hello"
```

因此，现在我们可以重复前面的未序列化示例，但改用 `SerializedLogger`。同样，我们创建了五个子任务并并行运行它们：

```F#
let serializedExample =
    let logger = SerializedLogger()
    [1..5]
        |> List.map (fun i -> makeTask logger.Log i)
        |> Async.Parallel
        |> Async.RunSynchronously
        |> ignore
```

真是大不一样！这一次的输出是完美的。

## 摘要

关于这种基于消息的方法还有很多要说的。在未来的系列中，我希望深入探讨更多细节，包括讨论以下主题：

- 使用 RabbitMQ 和 TPL Dataflow 的消息队列的替代实现。
- 取消和带外消息。
- 错误处理和重试，以及一般的异常处理。
- 如何通过创建或删除子代理来扩大和缩小规模。
- 避免缓冲区溢出并检测饥饿或不活动。

# 26 函数式反应式编程

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/concurrency-reactive/#series-toc)*)*

将事件转化为流
26 Apr 2012 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/concurrency-reactive/

事件无处不在。几乎每个程序都必须处理事件，无论是用户界面中的按钮点击、服务器中的套接字监听，甚至是系统关闭通知。

事件是最常见的 OO 设计模式之一的基础：“观察者”模式。

但正如我们所知，事件处理，就像一般的并发一样，实现起来可能很棘手。简单的事件逻辑很简单，但像“如果连续发生两个事件，做点什么，但如果只有一个事件发生，做点不同的事情”或“如果两个事件大致同时发生，做些什么”这样的逻辑呢。以其他更复杂的方式组合这些要求有多容易？

即使你能成功实现这些要求，即使有最好的意图，代码也往往是意大利面条般的，很难理解。

是否有一种方法可以使事件处理更容易？

我们在上一篇关于消息队列的文章中看到，这种方法的优点之一是请求被“序列化”，使其在概念上更容易处理。

有一种类似的方法可以用于事件。其想法是将一系列事件转化为“事件流”。然后，事件流变得非常像IEnumerables，因此下一步显然是以与 LINQ 处理集合大致相同的方式处理它们，以便对它们进行过滤、映射、拆分和组合。

F# 内置了对该模型以及更传统方法的支持。

## 一个简单的事件流

让我们从一个简单的例子开始比较这两种方法。我们将首先实现经典的事件处理程序方法。

首先，我们定义一个效用函数，它将：

- 创建计时器
- 为 `Elapsed` 事件注册一个处理程序
- 运行计时器五秒钟，然后停止

代码如下：

```F#
open System
open System.Threading

/// create a timer and register an event handler,
/// then run the timer for five seconds
let createTimer timerInterval eventHandler =
    // setup a timer
    let timer = new System.Timers.Timer(float timerInterval)
    timer.AutoReset <- true

    // add an event handler
    timer.Elapsed.Add eventHandler

    // return an async task
    async {
        // start timer...
        timer.Start()
        // ...run for five seconds...
        do! Async.Sleep 5000
        // ... and stop
        timer.Stop()
        }
```

现在以交互方式测试它：

```F#
// create a handler. The event args are ignored
let basicHandler _ = printfn "tick %A" DateTime.Now

// register the handler
let basicTimer1 = createTimer 1000 basicHandler

// run the task now
Async.RunSynchronously basicTimer1
```

现在，让我们创建一个类似的实用方法来创建计时器，但这次它也将返回一个“可观测”，即事件流。

```F#
let createTimerAndObservable timerInterval =
    // setup a timer
    let timer = new System.Timers.Timer(float timerInterval)
    timer.AutoReset <- true

    // events are automatically IObservable
    let observable = timer.Elapsed

    // return an async task
    let task = async {
        timer.Start()
        do! Async.Sleep 5000
        timer.Stop()
        }

    // return a async task and the observable
    (task,observable)
```

再次以交互方式进行测试：

```F#
// create the timer and the corresponding observable
let basicTimer2 , timerEventStream = createTimerAndObservable 1000

// register that every time something happens on the
// event stream, print the time.
timerEventStream
|> Observable.subscribe (fun _ -> printfn "tick %A" DateTime.Now)

// run the task now
Async.RunSynchronously basicTimer2
```

不同之处在于，我们不是直接向事件注册处理程序，而是“订阅”事件流。微妙的不同，而且很重要。

## 统计事件

在下一个例子中，我们将有一个稍微复杂一些的要求：

`创建一个每500毫秒滴答一次的计时器。`
`在每个刻度处，打印到目前为止的刻度数和当前时间。`

为了以经典的命令式方式实现这一点，我们可能会创建一个具有可变计数器的类，如下所示：

```F#
type ImperativeTimerCount() =

    let mutable count = 0

    // the event handler. The event args are ignored
    member this.handleEvent _ =
      count <- count + 1
      printfn "timer ticked with count %i" count
```

我们可以重用我们之前创建的实用函数来测试它：

```F#
// create a handler class
let handler = new ImperativeTimerCount()

// register the handler method
let timerCount1 = createTimer 500 handler.handleEvent

// run the task now
Async.RunSynchronously timerCount1
```

让我们看看如何以一种函数式的方式做同样的事情：

```F#
// create the timer and the corresponding observable
let timerCount2, timerEventStream = createTimerAndObservable 500

// set up the transformations on the event stream
timerEventStream
|> Observable.scan (fun count _ -> count + 1) 0
|> Observable.subscribe (fun count -> printfn "timer ticked with count %i" count)

// run the task now
Async.RunSynchronously timerCount2
```

在这里，我们看到了如何构建事件转换层，就像在 LINQ 中使用列表转换一样。

第一个转换是 `scan`，它为每个事件累积状态。它大致相当于我们看到的用于列表的 `List.fold` 函数。在这种情况下，累积状态只是一个计数器。

然后，对于每个事件，计数都会打印出来。

请注意，在这种函数式方法中，我们没有任何可变状态，也不需要创建任何特殊的类。

## 合并多个事件流

对于最后一个示例，我们将着眼于合并多个事件流。

让我们根据众所周知的“FizzBuzz”问题提出一个要求：

`创建两个计时器，分别称为“3”和“5”。“3”定时器每300ms滴答一次，“5”定时器每500毫秒滴答一次。`

`按如下方式处理事件：`
`a） 对于所有事件，打印时间和时间的id`
`b） 当一个刻度与前一个刻度同时出现时，打印“FizzBuzz”`
`否则：`
`c） 当“3”定时器自行计时时，打印“Fizz”`
`d） 当“5”定时器自行计时时，打印“Buzz”`

首先，让我们创建一些两种实现都可以使用的代码。

我们需要一个通用的事件类型来捕获计时器id和滴答声的时间。

```F#
type FizzBuzzEvent = {label:int; time: DateTime}
```

然后我们需要一个效用函数来查看两个事件是否同时发生。我们将慷慨地允许高达 50 毫秒的时差。

```F#
let areSimultaneous (earlierEvent,laterEvent) =
    let {label=_;time=t1} = earlierEvent
    let {label=_;time=t2} = laterEvent
    t2.Subtract(t1).Milliseconds < 50
```

在命令式设计中，我们需要跟踪之前的事件，以便进行比较。当前一个事件不存在时，我们第一次需要特殊的案例代码

```F#
type ImperativeFizzBuzzHandler() =

    let mutable previousEvent: FizzBuzzEvent option = None

    let printEvent thisEvent  =
      let {label=id; time=t} = thisEvent
      printf "[%i] %i.%03i " id t.Second t.Millisecond
      let simultaneous = previousEvent.IsSome && areSimultaneous (previousEvent.Value,thisEvent)
      if simultaneous then printfn "FizzBuzz"
      elif id = 3 then printfn "Fizz"
      elif id = 5 then printfn "Buzz"

    member this.handleEvent3 eventArgs =
      let event = {label=3; time=DateTime.Now}
      printEvent event
      previousEvent <- Some event

    member this.handleEvent5 eventArgs =
      let event = {label=5; time=DateTime.Now}
      printEvent event
      previousEvent <- Some event
```

现在代码开始变丑了！我们已经有了可变状态、复杂的条件逻辑和特殊情况，仅仅是为了满足这样一个简单的需求。

让我们来测试一下：

```F#
// create the class
let handler = new ImperativeFizzBuzzHandler()

// create the two timers and register the two handlers
let timer3 = createTimer 300 handler.handleEvent3
let timer5 = createTimer 500 handler.handleEvent5

// run the two timers at the same time
[timer3;timer5]
|> Async.Parallel
|> Async.RunSynchronously
```

它确实有效，但你确定代码没有错误吗？如果你换了东西，你可能会不小心把它弄坏吗？

这个命令式代码的问题是，它有很多噪音，掩盖了需求。

函数式版本能做得更好吗？让我们看看！

首先，我们创建两个事件流，每个计时器一个：

```F#
let timer3, timerEventStream3 = createTimerAndObservable 300
let timer5, timerEventStream5 = createTimerAndObservable 500
```

接下来，我们将“原始”事件流中的每个事件转换为FizzBuzz事件类型：

```F#
// convert the time events into FizzBuzz events with the appropriate id
let eventStream3  =
   timerEventStream3
   |> Observable.map (fun _ -> {label=3; time=DateTime.Now})

let eventStream5  =
   timerEventStream5
   |> Observable.map (fun _ -> {label=5; time=DateTime.Now})
```

现在，为了确定两个事件是否同时发生，我们需要以某种方式比较两个不同流中的事件。

这实际上比听起来更容易，因为我们可以：

- 将这两个流合并为一个流：
- 然后创建成对的连续事件
- 然后测试这些对，看看它们是否是同时的
- 然后根据该测试将输入流拆分为两个新的输出流

以下是执行此操作的实际代码：

```F#
// combine the two streams
let combinedStream =
    Observable.merge eventStream3 eventStream5

// make pairs of events
let pairwiseStream =
   combinedStream |> Observable.pairwise

// split the stream based on whether the pairs are simultaneous
let simultaneousStream, nonSimultaneousStream =
    pairwiseStream |> Observable.partition areSimultaneous
```

最后，我们可以根据事件 id 再次拆分 `nonSimultaneousStream`：

```F#
// split the non-simultaneous stream based on the id
let fizzStream, buzzStream  =
    nonSimultaneousStream
    // convert pair of events to the first event
    |> Observable.map (fun (ev1,_) -> ev1)
    // split on whether the event id is three
    |> Observable.partition (fun {label=id} -> id=3)
```

让我们回顾一下到目前为止。我们从两个原始事件流开始，并从中创建了四个新的事件流：

- `combinedStream` 包含所有事件
- `simultaneousStream` 只包含同时发生的事件
- `fizzStream` 仅包含id为3的非同步事件
- `buzzStream` 仅包含id为5的非同步事件

现在我们需要做的就是将行为附加到每个流上：

```F#
//print events from the combinedStream
combinedStream
|> Observable.subscribe (fun {label=id;time=t} ->
                              printf "[%i] %i.%03i " id t.Second t.Millisecond)

//print events from the simultaneous stream
simultaneousStream
|> Observable.subscribe (fun _ -> printfn "FizzBuzz")

//print events from the nonSimultaneous streams
fizzStream
|> Observable.subscribe (fun _ -> printfn "Fizz")

buzzStream
|> Observable.subscribe (fun _ -> printfn "Buzz")
```

让我们来测试一下：

```F#
// run the two timers at the same time
[timer3;timer5]
|> Async.Parallel
|> Async.RunSynchronously
```

以下是一套完整的代码：

```F#
// create the event streams and raw observables
let timer3, timerEventStream3 = createTimerAndObservable 300
let timer5, timerEventStream5 = createTimerAndObservable 500

// convert the time events into FizzBuzz events with the appropriate id
let eventStream3  = timerEventStream3
                    |> Observable.map (fun _ -> {label=3; time=DateTime.Now})
let eventStream5  = timerEventStream5
                    |> Observable.map (fun _ -> {label=5; time=DateTime.Now})

// combine the two streams
let combinedStream =
   Observable.merge eventStream3 eventStream5

// make pairs of events
let pairwiseStream =
   combinedStream |> Observable.pairwise

// split the stream based on whether the pairs are simultaneous
let simultaneousStream, nonSimultaneousStream =
   pairwiseStream |> Observable.partition areSimultaneous

// split the non-simultaneous stream based on the id
let fizzStream, buzzStream  =
    nonSimultaneousStream
    // convert pair of events to the first event
    |> Observable.map (fun (ev1,_) -> ev1)
    // split on whether the event id is three
    |> Observable.partition (fun {label=id} -> id=3)

//print events from the combinedStream
combinedStream
|> Observable.subscribe (fun {label=id;time=t} ->
                              printf "[%i] %i.%03i " id t.Second t.Millisecond)

//print events from the simultaneous stream
simultaneousStream
|> Observable.subscribe (fun _ -> printfn "FizzBuzz")

//print events from the nonSimultaneous streams
fizzStream
|> Observable.subscribe (fun _ -> printfn "Fizz")

buzzStream
|> Observable.subscribe (fun _ -> printfn "Buzz")

// run the two timers at the same time
[timer3;timer5]
|> Async.Parallel
|> Async.RunSynchronously
```

代码可能看起来有点冗长，但这种渐进式、循序渐进的方法非常清晰，并且具有自文档性。

这种风格的一些好处是：

- 只需查看它，甚至不运行它，我就可以看到它符合要求。命令式版本则不然。

- 从设计的角度来看，每个最终的“输出”流都遵循单一责任原则——它只做一件事——所以很容易将行为与它联系起来。

- 这段代码没有条件句，没有可变状态，没有边缘情况。我希望它很容易维护或更改。

- 它易于调试。例如，我可以很容易地“点击”simultaneousStream的输出，看看它是否包含我认为它包含的内容：

  ```F#
  // debugging code
  //simultaneousStream |> Observable.subscribe (fun e -> printfn "sim %A" e)
  //nonSimultaneousStream |> Observable.subscribe (fun e -> printfn "non-sim %A" e)
  ```

在命令式版本中，这会困难得多。

## 摘要

函数式响应式编程（称为 FRP）是一个大话题，我们在这里才刚刚谈到它。我希望这篇介绍能让你一窥这种做事方式的有用性。

如果您想了解更多信息，请参阅 F# Observable 模块的文档，该模块具有上面使用的基本转换。还有作为 .NET 4 的一部分提供的响应式扩展（Rx）库。这包含了许多其他的转换。



# 27 完整性

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/completeness-intro/#series-toc)*)*

F# 是 .NET 生态系统整体的一部分
27 Apr 2012 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/completeness-intro/

在这最后一组帖子中，我们将在“完整性”的主题下探讨F#的其他方面。

来自学术界的编程语言往往更注重优雅和纯粹，而不是现实世界的实用性，而 C# 和 Java 等更主流的商业语言之所以受到重视，正是因为它们是实用的；它们可以在各种情况下工作，并拥有广泛的工具和库来满足几乎所有的需求。换句话说，为了在企业中发挥作用，一门语言需要是完整的，而不仅仅是设计良好的。

F# 的不同寻常之处在于它成功地连接了两个世界。尽管到目前为止，所有的例子都集中在 F# 作为一种优雅的函数式语言上，但它也支持面向对象的范式，并且可以很容易地与其他 .NET 语言和工具集成。因此，F# 不是一个孤岛，而是从 .NET 生态系统整体中受益。

让 F# “完整”的其他方面是成为一名官方 .NET 语言（及其所需的所有支持和文档），旨在在 Visual Studio（提供具有 IntelliSense 支持的优秀编辑器、调试器等）和 Visual Studio Code 中工作。这些好处应该是显而易见的，这里不会讨论。

因此，在最后一节中，我们将重点介绍两个特定领域：

- **与 .NET 库无缝互操作**。显然，F# 的函数式方法和设计到基础库中的命令式方法之间可能存在不匹配。我们将看看 F# 的一些特性，这些特性使这种集成更容易。
- **完全支持类和其他 C# 风格的代码**。F# 被设计为一种混合函数式/OO 语言，因此它几乎可以做 C# 能做的一切。我们将快速浏览这些其他功能的语法。

# 28 与 .NET 库无缝互操作

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/completeness-seamless-dotnet-interop/#series-toc)*)*

一些便于使用 .NET 库的功能
28 Apr 2012 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/completeness-seamless-dotnet-interop/

我们已经看到了许多使用 .NET库 F# 的例子，例如使用 `System.Net.WebRequest` 和 `System.Text.RegularExpressions`。而且整合确实是无缝的。

对于更复杂的需求，F# 本机支持 .NET 类、接口和结构，因此互操作仍然非常简单。例如，你可以用 C# 编写一个 `ISomething` 接口，并用 F# 实现。

但 F# 不仅可以调用现有的 .NET 代码，它也可以公开几乎任何 .NET API 返回到其他语言。例如，你可以用 F# 编写类和方法，并将其暴露给 C#、VB 或 COM。你甚至可以反向执行上述示例——在 F# 中定义一个 `ISomething` 接口，并用 C# 完成实现！所有这些的好处是，您不必丢弃任何现有的代码库；你可以开始在某些事情上使用 F#，而在其他事情上保留 C# 或 VB，并为这项工作选择最好的工具。

除了紧密的集成之外，F# 中还有许多很好的功能，通常可以使用 .NET 库在某些方面比 C# 更方便。以下是我最喜欢的一些：

- 您可以使用 TryParse 和 TryGetValue，而无需传递“out”参数。
- 您可以通过使用参数名来解决方法重载问题，这也有助于类型推理。
- 您可以使用“活动模式”进行转换。NET API转化为更友好的代码。
- 您可以从IDisposable等接口动态创建对象，而无需创建具体类。
- 您可以将“纯” F# 对象与现有 .NET API 混合和匹配

## TryParse 和 TryGetValue

值和字典的 `TryParse` 和 `TryGetValue` 函数经常用于避免额外的异常处理。但是 C# 语法有点笨拙。从 F# 使用它们更优雅，因为 F# 会自动将函数转换为元组，其中第一个元素是函数返回值，第二个元素是“out”参数。

```F#
//using an Int32
let (i1success,i1) = System.Int32.TryParse("123");
if i1success then printfn "parsed as %i" i1 else printfn "parse failed"

let (i2success,i2) = System.Int32.TryParse("hello");
if i2success then printfn "parsed as %i" i2 else printfn "parse failed"

//using a DateTime
let (d1success,d1) = System.DateTime.TryParse("1/1/1980");
let (d2success,d2) = System.DateTime.TryParse("hello");

//using a dictionary
let dict = new System.Collections.Generic.Dictionary<string,string>();
dict.Add("a","hello")
let (e1success,e1) = dict.TryGetValue("a");
let (e2success,e2) = dict.TryGetValue("b");
```

## 命名参数以帮助类型推理

在C#（以及一般的 .NET）中，您可以重载具有许多不同参数的方法。F# 可能会遇到这个问题。例如，以下是创建 StreamReader 的尝试：

```F#
let createReader fileName = new System.IO.StreamReader(fileName)
// error FS0041: A unique overload for method 'StreamReader'
//               could not be determined
```

问题是 F# 不知道参数应该是字符串还是流。您可以显式指定参数的类型，但这不是 F# 的方式！

相反，一个很好的解决方法是通过在 F# 中调用 .NET库中的方法时，您可以指定命名参数的方式启用。

```F#
let createReader2 fileName = new System.IO.StreamReader(path=fileName)
```

在许多情况下，如上所述，仅使用参数名称就足以解决类型问题。使用明确的参数名称通常有助于使代码更易读。

## .NET 函数的活动模式

在许多情况下，您都希望使用 .NET 类型模式匹配，但本机库不支持此功能。之前，我们简要介绍了 F# 的“活动模式”功能，该功能允许您动态创建匹配选项。这对 .NET 集成非常有用。

一个常见的情况是 .NET 库类有许多互斥的 `isSomething`、`isSomethingElse` 方法，这些方法必须用看起来很可怕的级联 if-else 语句进行测试。主动模式可以隐藏所有丑陋的测试，让代码的其余部分使用更自然的方法。

例如，这是测试 `System.Char` 的各种 `isXXX` 方法的代码。

```F#
let (|Digit|Letter|Whitespace|Other|) ch =
   if System.Char.IsDigit(ch) then Digit
   else if System.Char.IsLetter(ch) then Letter
   else if System.Char.IsWhiteSpace(ch) then Whitespace
   else Other
```

一旦定义了选项，正常的代码就可以很简单了：

```F#
let printChar ch =
  match ch with
  | Digit -> printfn "%c is a Digit" ch
  | Letter -> printfn "%c is a Letter" ch
  | Whitespace -> printfn "%c is a Whitespace" ch
  | _ -> printfn "%c is something else" ch

// print a list
['a';'b';'1';' ';'-';'c'] |> List.iter printChar
```

另一种常见情况是，您必须解析文本或错误代码以确定异常或结果的类型。下面是一个使用活动模式解析与 `SqlExceptions` 相关的错误号的示例，使其更易于接受。

首先，在错误号上设置活动模式匹配：

```F#
open System.Data.SqlClient

let (|ConstraintException|ForeignKeyException|Other|) (ex:SqlException) =
   if ex.Number = 2601 then ConstraintException
   else if ex.Number = 2627 then ConstraintException
   else if ex.Number = 547 then ForeignKeyException
   else Other
```

现在我们可以在处理SQL命令时使用这些模式：

```F#
let executeNonQuery (sqlCommmand:SqlCommand) =
    try
       let result = sqlCommmand.ExecuteNonQuery()
       // handle success
    with
    | :?SqlException as sqlException -> // if a SqlException
        match sqlException with         // nice pattern matching
        | ConstraintException  -> // handle constraint error
        | ForeignKeyException  -> // handle FK error
        | _ -> reraise()          // don't handle any other cases
    // all non SqlExceptions are thrown normally
```

## 直接从接口创建对象

F# 还有另一个有用的特性，称为“对象表达式”。这是一种直接从接口或抽象类创建对象的能力，而无需先定义具体类。

在下面的示例中，我们使用 `makeResource` 辅助函数创建了一些实现 `IDisposable` 的对象。

```F#
// create a new object that implements IDisposable
let makeResource name =
   { new System.IDisposable
     with member this.Dispose() = printfn "%s disposed" name }

let useAndDisposeResources =
    use r1 = makeResource "first resource"
    printfn "using first resource"
    for i in [1..3] do
        let resourceName = sprintf "\tinner resource %d" i
        use temp = makeResource resourceName
        printfn "\tdo something with %s" resourceName
    use r2 = makeResource "second resource"
    printfn "using second resource"
    printfn "done."
```

该示例还演示了当资源超出范围时，“`use`”关键字如何自动处置资源。输出如下：

```
using first resource
	do something with 	inner resource 1
	inner resource 1 disposed
	do something with 	inner resource 2
	inner resource 2 disposed
	do something with 	inner resource 3
	inner resource 3 disposed
using second resource
done.
second resource disposed
first resource disposed
```

## 混合纯 F# 类型的 .NET 接口

动态创建接口实例的能力意味着很容易将现有 API 的接口与纯 F# 类型混合和匹配。

例如，假设您有一个预先存在的 API，它使用 IAnimal 接口，如下所示。

```F#
type IAnimal =
   abstract member MakeNoise : unit -> string

let showTheNoiseAnAnimalMakes (animal:IAnimal) =
   animal.MakeNoise() |> printfn "Making noise %s"
```

但是我们希望拥有模式匹配等的所有好处，所以我们为猫和狗创建了纯 F# 类型，而不是类。

```F#
type Cat = Felix | Socks
type Dog = Butch | Lassie
```

但是使用这种纯 F# 方法意味着我们不能直接将猫和狗传递给 `showTheNoiseAnAnimalMakes` 函数。

然而，我们不必为了实现 `IAnimal` 而创建新的具体类集。相反，我们可以通过扩展纯 F# 类型来动态创建 `IAnimal` 接口。

```F#
// now mixin the interface with the F# types
type Cat with
   member this.AsAnimal =
        { new IAnimal
          with member a.MakeNoise() = "Meow" }

type Dog with
   member this.AsAnimal =
        { new IAnimal
          with member a.MakeNoise() = "Woof" }
```

以下是一些测试代码：

```F#
let dog = Lassie
showTheNoiseAnAnimalMakes (dog.AsAnimal)

let cat = Felix
showTheNoiseAnAnimalMakes (cat.AsAnimal)
```

这种方法让我们两全其美。内部纯 F# 类型，但能够根据需要将其转换为与库交互的接口。

## 使用反射检查 F# 类型

F# 从 .NET 反射系统中受益，这意味着您可以使用语言本身的语法完成各种您无法直接使用的有趣的事情。`Microsoft.FSharp.Reflection` 命名空间有许多专门用于帮助 F# 类型的函数。

例如，这里有一种打印记录类型中的字段和联合类型中的选项的方法。

```F#
open System.Reflection
open Microsoft.FSharp.Reflection

// create a record type...
type Account = {Id: int; Name: string}

// ... and show the fields
let fields =
    FSharpType.GetRecordFields(typeof<Account>)
    |> Array.map (fun propInfo -> propInfo.Name, propInfo.PropertyType.Name)

// create a union type...
type Choices = | A of int | B of string

// ... and show the choices
let choices =
    FSharpType.GetUnionCases(typeof<Choices>)
    |> Array.map (fun choiceInfo -> choiceInfo.Name)
```

# 29 C# 可以做的任何事情……

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/completeness-anything-csharp-can-do/#series-toc)*)*

F# 中面向对象代码的旋风之旅
29 Apr 2012 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/completeness-anything-csharp-can-do/

很明显，在 F# 中，你通常应该尝试更喜欢函数式代码而不是面向对象代码，但在某些情况下，你可能需要一种完全成熟的 OO 语言的所有功能——类、继承、虚拟方法等。

因此，为了结束本节，这里是对这些功能的 F# 版本的快速浏览。

其中一些将在稍后的系列文章中更深入地讨论。NET集成。但我不会介绍一些比较晦涩的，如果你需要的话，可以在MSDN文档中阅读它们。

## 类和接口

首先，这里有一些接口、抽象类和从抽象类继承的具体类的示例。

```F#
// interface
type IEnumerator<'a> =
    abstract member Current : 'a
    abstract MoveNext : unit -> bool

// abstract base class with virtual methods
[<AbstractClass>]
type Shape() =
    //readonly properties
    abstract member Width : int with get
    abstract member Height : int with get
    //non-virtual method
    member this.BoundingArea = this.Height * this.Width
    //virtual method with base implementation
    abstract member Print : unit -> unit
    default this.Print () = printfn "I'm a shape"

// concrete class that inherits from base class and overrides
type Rectangle(x:int, y:int) =
    inherit Shape()
    override this.Width = x
    override this.Height = y
    override this.Print ()  = printfn "I'm a Rectangle"

//test
let r = Rectangle(2,3)
printfn "The width is %i" r.Width
printfn "The area is %i" r.BoundingArea
r.Print()
```

类可以有多个构造函数、可变属性等。

```F#
type Circle(rad:int) =
    inherit Shape()

    //mutable field
    let mutable radius = rad

    //property overrides
    override this.Width = radius * 2
    override this.Height = radius * 2

    //alternate constructor with default radius
    new() = Circle(10)

    //property with get and set
    member this.Radius
         with get() = radius
         and set(value) = radius <- value

// test constructors
let c1 = Circle()   // parameterless ctor
printfn "The width is %i" c1.Width
let c2 = Circle(2)  // main ctor
printfn "The width is %i" c2.Width

// test mutable property
c2.Radius <- 3
printfn "The width is %i" c2.Width
```

## 泛型

F#支持泛型和所有相关的约束。

```F#
// standard generics
type KeyValuePair<'a,'b>(key:'a, value: 'b) =
    member this.Key = key
    member this.Value = value

// generics with constraints
type Container<'a,'b
    when 'a : equality
    and 'b :> System.Collections.ICollection>
    (name:'a, values:'b) =
    member this.Name = name
    member this.Values = values
```

## 结构体

F# 不仅支持类，还支持 .NET 结构类型，在某些情况下可以帮助提高性能。

```F#
type Point2D =
   struct
      val X: float
      val Y: float
      new(x: float, y: float) = { X = x; Y = y }
   end

//test
let p = Point2D()  // zero initialized
let p2 = Point2D(2.0,3.0)  // explicitly initialized
```

## 异常

F# 可以创建异常类，引发它们并捕获它们。

```F#
// create a new Exception class
exception MyError of string

try
    let e = MyError("Oops!")
    raise e
with
    | MyError msg ->
        printfn "The exception error was %s" msg
    | _ ->
        printfn "Some other exception"
```

## 扩展方法

与 C# 一样，F# 可以使用扩展方法扩展现有类。

```F#
type System.String with
    member this.StartsWithA = this.StartsWith "A"

//test
let s = "Alice"
printfn "'%s' starts with an 'A' = %A" s s.StartsWithA

type System.Int32 with
    member this.IsEven = this % 2 = 0

//test
let i = 20
if i.IsEven then printfn "'%i' is even" i
```

## 参数数组

就像 C# 的可变长度“params”关键字一样，这允许将可变长度的参数列表转换为单个数组参数。

```F#
open System
type MyConsole() =
    member this.WriteLine([<ParamArray>] args: Object[]) =
        for arg in args do
            printfn "%A" arg

let cons = new MyConsole()
cons.WriteLine("abc", 42, 3.14, true)
```

## 事件

F# 类可以有事件，这些事件可以被触发和响应。

```F#
type MyButton() =
    let clickEvent = new Event<_>()

    [<CLIEvent>]
    member this.OnClick = clickEvent.Publish

    member this.TestEvent(arg) =
        clickEvent.Trigger(this, arg)

// test
let myButton = new MyButton()
myButton.OnClick.Add(fun (sender, arg) ->
        printfn "Click event with arg=%O" arg)

myButton.TestEvent("Hello World!")
```

## 委托

F# 可以做委托。

```F#
// delegates
type MyDelegate = delegate of int -> int
let f = MyDelegate (fun x -> x * x)
let result = f.Invoke(5)
```

## 枚举类型

F# 支持 CLI 枚举类型，这些枚举类型看起来类似于“union”类型，但实际上在幕后是不同的。

```F#
// enums
type Color = | Red=1 | Green=2 | Blue=3

let color1  = Color.Red    // simple assignment
let color2:Color = enum 2  // cast from int
// created from parsing a string
let color3 = System.Enum.Parse(typeof<Color>,"Green") :?> Color // :?> is a downcast

[<System.Flags>]
type FileAccess = | Read=1 | Write=2 | Execute=4
let fileaccess = FileAccess.Read ||| FileAccess.Write
```

## 使用标准用户界面

最后，F# 可以像 C# 一样使用 WinForms 和 WPF 用户界面库。

这是一个打开表单并处理点击事件的简单示例。

```F#
open System.Windows.Forms

let form = new Form(Width= 400, Height = 300, Visible = true, Text = "Hello World")
form.TopMost <- true
form.Click.Add (fun args-> printfn "the form was clicked")
form.Show()
```

# 30 为什么使用F#：结论

*Part of the "Why use F#?" series (*[link](https://fsharpforfunandprofit.com/posts/why-use-fsharp-conclusion/#series-toc)*)*

2012年4月30日 这篇文章已有3年多的历史了

https://fsharpforfunandprofit.com/posts/why-use-fsharp-conclusion/

至此，F# 函数式编程之旅结束。我希望这些例子能让你对 F# 和函数式编程的力量有所了解。如果您对整个系列有任何意见，请将其留在本页底部。

在后面的系列中，我希望更深入地了解数据结构、模式匹配、列表处理、异步和并行编程等等。

但在此之前，我建议你阅读“函数式思维”系列，这将有助于你更深入地理解函数式编程。

