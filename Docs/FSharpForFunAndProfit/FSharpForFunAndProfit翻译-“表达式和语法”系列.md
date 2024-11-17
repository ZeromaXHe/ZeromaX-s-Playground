# [返回主 Markdown](./FSharpForFunAndProfit翻译.md)



# 1 表达式和语法：简介

*Part of the "Expressions and syntax" series (*[link](https://fsharpforfunandprofit.com/posts/expressions-intro/#series-toc)*)*

如何用 F# 编码
2012年5月15日 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/expressions-intro/

注意：在阅读本系列之前，我建议你先阅读“功能性思维”系列。

在本系列中，我们将探讨函数和值是如何组合成表达式的，以及 F# 中可用的不同类型的表达式。

我们还将探讨其他一些基本主题，如 `let` 绑定、F# 语法、模式匹配以及使用 `printf` 输出文本。

本系列并非详尽无遗或具有决定性。F# 的大部分语法和用法应该从示例中显而易见，如果你需要的话，MSDN 文档中有所有的细节。相反，我们将专注于解释一些可能令人困惑的基本领域。

因此，我们将从一些一般提示开始，讨论 `let` 绑定是如何工作的，并解释缩进规则。

之后，接下来的几篇文章将介绍 `match..with` 表达式、命令式控制流表达式和异常表达式。计算表达式和面向对象的表达式将留待后续系列讨论。

最后，我们将以一些使用模式匹配作为其设计组成部分的工作示例结束。

# 2 表达式与语句

*Part of the "Expressions and syntax" series (*[link](https://fsharpforfunandprofit.com/posts/expressions-vs-statements/#series-toc)*)*

为什么表达式更安全，并成为更好的构建块
2012年5月16日 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/expressions-vs-statements/

在编程语言术语中，“表达式”是值和函数的组合，由编译器组合和解释以创建新值，而“语句”只是一个独立的执行单元，不返回任何东西。一种思考方式是，表达式的目的是创建一个值（带有一些可能的副作用），而语句的唯一目的是产生副作用。

C# 和大多数命令式语言对表达式和语句进行了区分，并对每种表达式和语句的使用位置都有规则。但显而易见的是，真正纯粹的函数式语言根本无法支持语句，因为在真正纯粹的语言中，不会有任何副作用。

尽管 F# 不是纯的，但它遵循同样的原则。在 F# 中，一切都是表达式。不仅是值和函数，还有控制流（如 if-then-else 和循环）、模式匹配等。

使用表达式而不是语句有一些微妙的好处。首先，与语句不同，较小的表达式可以组合（或“组合”）成较大的表达式。所以，如果一切都是表达式，那么一切都是可组合的。

其次，一系列语句总是意味着一个特定的求值顺序，这意味着如果不查看前面的语句，就无法理解语句。但是对于纯表达式，子表达式没有任何隐含的执行顺序或依赖关系。

因此，在表达式 a+b 中，如果“a”和“b”部分都是纯的，那么“a”部分可以独立地被分离、理解、测试和评估，“b”也可以。表达式的这种“隔离性”是函数式编程的另一个有益方面。

> 请注意，F# 交互窗口也依赖于所有内容都是表达式。使用 C# 交互式窗口要困难得多。

## 表达式更安全、更简洁

一致地使用表达式可以使代码更安全、更紧凑。让我们看看我是什么意思。

首先，让我们看看基于语句的方法。语句不返回值，因此您必须使用从语句体中分配的临时变量。以下是一些使用类 C 语言（OK，C#）而不是 F# 的示例：

```c#
public void IfThenElseStatement(bool aBool)
{
   int result;     //what is the value of result before it is used?
   if (aBool)
   {
      result = 42; //what is the result in the 'else' case?
   }
   Console.WriteLine("result={0}", result);
}
```

因为“if then”是一个语句，所以 `result` 变量必须在语句外部定义，但分配给语句内部，这会导致一些问题：

- `result` 变量必须在语句本身之外设置。它应该设置为什么初始值？
- 如果我忘记在 `if` 语句中赋值给 `result` 变量怎么办？“if”语句的目的纯粹是产生副作用（对变量的赋值）。这意味着这些语句可能有缺陷，因为很容易忘记在一个分支中进行赋值。而且因为赋值只是一个副作用，编译器无法提供任何警告。由于 `result` 变量已经在作用域中定义，我可以很容易地使用它，而不知道它是无效的。
- 在“else”情况下，`result` 变量的值是什么？在这种情况下，我没有指定值。我忘了吗？这是一个潜在的 bug 吗？
- 最后，依赖副作用来完成任务意味着语句不容易在另一个上下文中使用（例如，提取用于重构或并行化），因为它们依赖于一个不属于语句本身的变量。

注意：上面的代码不会在 C# 中编译，因为如果你使用这样的未赋值局部变量，编译器会发出抱怨。但是，在使用之前必须为结果定义一些默认值仍然是一个问题。

为了进行比较，这是以面向表达式的风格重写的相同代码：

```c#
public void IfThenElseExpression(bool aBool)
{
    int result = aBool ? 42 : 0;
    Console.WriteLine("result={0}", result);
}
```

在面向表达式的版本中，早期的问题甚至都不适用！

- `result` 变量在赋值的同时被声明。不必在表达式“外部”设置任何变量，也不必担心它们应该设置为什么初始值。
- “else”被明确处理。不可能忘记在其中一个分支机构做作业。
- 我不可能忘记赋值给 `result`，因为那样变量就根本不存在了！

在 F# 中，这两个例子可以写成：

```F#
let IfThenElseStatement aBool =
   let mutable result = 0       // mutable keyword required
   if (aBool) then result <- 42
   printfn "result=%i" result
```

“`mutable`” 关键字在 F# 中被认为是一种代码气味，除非在某些特殊情况下，否则不建议使用。在学习过程中，应该不惜一切代价避免！

在基于表达式的版本中，可变变量已被删除，任何地方都没有重新分配。

```F#
let IfThenElseExpression aBool =
   let result = if aBool then 42 else 0
                // note that the else case must be specified
   printfn "result=%i" result
```

一旦我们将 `if` 语句转换为表达式，现在重构它并将整个子表达式移动到不同的上下文而不引入错误就变得轻而易举了。

以下是 C# 中的重构版本：

```c#
public int StandaloneSubexpression(bool aBool)
{
    return aBool ? 42 : 0;
}

public void IfThenElseExpressionRefactored(bool aBool)
{
    int result = StandaloneSubexpression(aBool);
    Console.WriteLine("result={0}", result);
}
```

在 F# 中：

```F#
let StandaloneSubexpression aBool =
   if aBool then 42 else 0

let IfThenElseExpressionRefactored aBool =
   let result = StandaloneSubexpression aBool
   printfn "result=%i" result
```

## 循环的语句 vs 表达式

再次回到 C#，这里有一个使用循环语句的语句与表达式的类似示例

```c#
public void LoopStatement()
{
    int i;    //what is the value of i before it is used?
    int length;
    var array = new int[] { 1, 2, 3 };
    int sum;  //what is the value of sum if the array is empty?

    length = array.Length;   //what if I forget to assign to length?
    for (i = 0; i < length; i++)
    {
        sum += array[i];
    }

    Console.WriteLine("sum={0}", sum);
}
```

我使用了一种旧式的“for”语句，其中索引变量在循环外声明。前面讨论的许多问题都适用于循环索引“`i`”和最大值“`length`”，例如：它们可以在循环外使用吗？如果他们没有被分配，会发生什么？

更现代的for循环版本通过在“for”循环本身中声明和分配循环变量，并要求初始化“`sum`”变量来解决这些问题：

```c#
public void LoopStatementBetter()
{
    var array = new int[] { 1, 2, 3 };
    int sum = 0;        // initialization is required

    for (var i = 0; i < array.Length; i++)
    {
        sum += array[i];
    }

    Console.WriteLine("sum={0}", sum);
}
```

这个更现代的版本遵循将局部变量的声明与其第一个赋值相结合的一般原则。

当然，我们可以通过使用 `foreach` 循环而不是 `for` 循环来不断改进：

```c#
public void LoopStatementForEach()
{
    var array = new int[] { 1, 2, 3 };
    int sum = 0;        // initialization is required

    foreach (var i in array)
    {
        sum += i;
    }

    Console.WriteLine("sum={0}", sum);
}
```

每次，我们不仅压缩了代码，而且降低了出错的可能性。

但是，将这一原则归结为合乎逻辑的结论，会导致一种完全基于表达的方法！以下是如何使用 LINQ 完成它：

```c#
public void LoopExpression()
{
    var array = new int[] { 1, 2, 3 };

    var sum = array.Aggregate(0, (sumSoFar, i) => sumSoFar + i);

    Console.WriteLine("sum={0}", sum);
}
```

请注意，我本可以使用 LINQ 的内置“sum”函数，但我使用了 `Aggregate` 来展示如何将语句中嵌入的 sum 逻辑转换为 lambda 并用作表达式的一部分。

在下一篇文章中，我们将介绍 F# 中的各种表达式。

# 3 F#表达式概述

*Part of the "Expressions and syntax" series (*[link](https://fsharpforfunandprofit.com/posts/understanding-fsharp-expressions/#series-toc)*)*

控制流、let、dos 等
2012年5月17日 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/understanding-fsharp-expressions/

在这篇文章中，我们将介绍F#中可用的不同类型的表达式以及使用它们的一些一般提示。

## 一切真的是一种表达式吗？

你可能想知道“一切都是一种表达”在实践中是如何运作的。

让我们从一些应该熟悉的基本表达式示例开始：

```c#
1                            // literal
[1;2;3]                      // list expression
-2                           // prefix operator
2 + 2                        // infix operator
"string".Length              // dot lookup
printf "hello"               // function application
```

没有问题。这些显然是表达式。

但这里有一些更复杂的东西，它们也是表达式。也就是说，每个值都返回一个可用于其他用途的值。

```F#
fun () -> 1                  // lambda expression

match 1 with                 // match expression
    | 1 -> "a"
    | _ -> "b"

if true then "a" else "b"    // if-then-else

for i in [1..10]             // for loop
  do printf "%i" i

try                          // exception handling
  let result = 1 / 0
  printfn "%i" result
with
  | e ->
     printfn "%s" e.Message


let n=1 in n+2               // let expression
```

在其他语言中，这些可能是语句，但在 F# 中，它们确实会返回值，正如您通过将值绑定到结果中所看到的那样：

```F#
let x1 = fun () -> 1

let x2 = match 1 with
         | 1 -> "a"
         | _ -> "b"

let x3 = if true then "a" else "b"

let x4 = for i in [1..10]
          do printf "%i" i

let x5 = try
            let result = 1 / 0
            printfn "%i" result
         with
            | e ->
                printfn "%s" e.Message


let x6 = let n=1 in n+2
```

## 有哪些表达式？

F# 中有很多不同类型的表达式，目前大约有 50 种。它们中的大多数都是琐碎而明显的，比如文字、运算符、函数应用程序、“点入”等等。

更有趣和更高级的可以分为以下几类：

- Lambda 表达式
- “控制流”表达式，包括：
  - match表达式（使用 `match..with` 语法）
  - 与命令式控制流相关的表达式，如 if-then-else 循环
  - 异常相关表达式
- “let”和“use”表达式
- 计算表达式，如 `async {..}`
- 与面向对象代码相关的表达式，包括转换、接口等

我们已经在“函数式思维”系列中讨论了 lambdas，如前所述，计算表达式和面向对象表达式将留给后面的系列讨论。

因此，在本系列的后续文章中，我们将重点介绍“控制流”表达式和“let”表达式。

### “控制流”表达式

在命令式语言中，if-then-else、for-in-do 和 match-with 等控制流表达式通常被实现为具有副作用的语句，而在 F# 中，它们都被实现为另一种类型的表达式。

事实上，用函数式语言来思考“控制流”甚至没有帮助；这个概念并不存在。最好把程序看作是一个包含子表达式的巨大表达式，其中一些子表达式被求值，而另一些则没有。如果你能理解这种思维方式，那么你在函数式思维方面就有了一个良好的开端。

接下来会有一些关于这些不同类型的控制流表达式的帖子：

- 匹配表达式
- 强制控制流：if-then-else 和 for 循环
- 异常

### “let”绑定作为表达式

`let x=something` 怎么样？在上面的例子中，我们看到：

```F#
let x6 = let n=1 in n+2
```

“let”怎么能成为一种表达式？原因将在下一篇关于“let”、“use”和“do”的文章中讨论。

## 使用表达式的一般提示

但在我们详细介绍重要的表达式类型之前，这里有一些使用一般表达式的技巧。

### 一行中有多个表达式

通常，每个表达式都放在新行上。但是，如果需要，您可以使用分号来分隔一行中的表达式。除了用作列表和记录元素的分隔符外，这是 F# 中使用分号的少数几次之一。

```F#
let f x =                           // one expression per line
      printfn "x=%i" x
      x + 1

let f x = printfn "x=%i" x; x + 1   // all on same line with ";"
```

当然，在最后一个表达式之前要求单位值的规则仍然适用：

```F#
let x = 1;2              // error: "1;" should be a unit expression
let x = ignore 1;2       // ok
let x = printf "hello";2 // ok
```

### 理解表达式求值顺序

在 F# 中，表达式是从“内到外”计算的——也就是说，一旦“看到”一个完整的子表达式，它就会被计算。

看看下面的代码，试着猜测会发生什么，然后评估代码并查看。

```F#
// create a clone of if-then-else
let test b t f = if b then t else f

// call it with two different choices
test true (printfn "true") (printfn "false")
```

实际情况是，“true”和“false”都会被打印出来，即使测试函数永远不会实际求值“else”分支。为什么？因为 `(printfn "false")` 表达式会立即求值，而不管测试函数将如何使用它。

这种求值方式被称为“及早（eager）”。它的优点是易于理解，但这确实意味着它有时可能效率低下。

另一种计算方式称为“惰性”，即表达式仅在需要时才被计算。Haskell 语言遵循这种方法，因此 Haskell 中的类似示例只会打印“true”。

在 F# 中，有许多技术可以强制不立即计算表达式。最简单的方法是将其封装在一个只按需计算的函数中：

```F#
// create a clone of if-then-else that accepts functions rather than simple values
let test b t f = if b then t() else f()

// call it with two different functions
test true (fun () -> printfn "true") (fun () -> printfn "false")
```

这样做的问题是，现在“真”函数可能会被错误地计算两次，而我们只想计算一次！

因此，不立即计算表达式的首选方法是使用 `Lazy<>` 包装器。

```F#
// create a clone of if-then-else with no restrictions...
let test b t f = if b then t else f

// ...but call it with lazy values
let f = test true (lazy (printfn "true")) (lazy (printfn "false"))
```

最终结果值 `f` 也是一个惰性值，可以在不进行计算的情况下传递，直到你最终准备好得到结果。

```F#
f.Force()     // use Force() to force the evaluation of a lazy value
```

如果你从不需要结果，也从不调用 `Force()`，那么包装后的值将永远不会被计算。

在即将到来的表演系列中，将有更多关于懒惰的内容。

# 4 使用 let、use 和 do 进行绑定

*Part of the "Expressions and syntax" series (*[link](https://fsharpforfunandprofit.com/posts/let-use-do/#series-toc)*)*

如何使用它们
2012年5月17日 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/let-use-do/

正如我们已经看到的，F# 中没有“变量”。相反，有价值观。

我们还看到，let、use 和 do 等关键字充当绑定——将标识符与值或函数表达式相关联。

在这篇文章中，我们将更详细地研究这些绑定。

## “let”绑定

`let` 绑定很简单，它有一般的形式：

```F#
let aName = someExpression
```

但是 `let` 有两种用法，它们有细微的不同。一种是在模块*的顶层定义一个命名表达式，另一种是定义在某个表达式的上下文中使用的本地名称。这有点类似于 C# 中“顶级”方法名和“局部”变量名之间的区别。

*在后面的系列中，当我们讨论 OO 特性时，类也可以具有顶级 let 绑定。

以下是这两种类型的示例：

```F#
module MyModule =

    let topLevelName =
        let nestedName1 = someExpression
        let nestedName2 = someOtherExpression
        finalExpression
```

顶层名称是一个*定义*，它是模块的一部分，您可以使用完全限定的名称（如 `MyModule.topLevelName`）访问它。从某种意义上说，它相当于一个类方法。

但是嵌套名称对任何人来说都是完全不可访问的——它们只在顶级名称绑定的上下文中有效。

### “let”绑定中的模式

我们已经看到了绑定如何直接使用模式的示例

```F#
let a,b = 1,2

type Person = {First:string; Last:string}
let alice = {First="Alice"; Last="Doe"}
let {First=first} = alice
```

在函数定义中，绑定也包括参数：

```F#
// pattern match the parameters
let add (x,y) = x + y

// test
let aTuple = (1,2)
add aTuple
```

各种模式绑定的细节取决于所绑定的类型，将在稍后关于模式匹配的文章中进一步讨论。

### 嵌套的“let”绑定作为表达式

我们强调了表达式是由较小的表达式组成的。但是嵌套 `let` 呢？

```F#
let nestedName = someExpression
```

“`let`”怎么能成为一种表达？它会返回什么？

答案是嵌套的“let”永远不能单独使用——它必须始终是更大代码块的一部分，这样它就可以被解释为：

```F#
let nestedName = [some expression] in [some other expression involving nestedName]
```

也就是说，每次在第二个表达式（称为 *body 表达式*）中看到符号“nestedName”时，用第一个表达式替换它。

例如，表达式：

```F#
// standard syntax
let f () =
  let x = 1
  let y = 2
  x + y          // the result
```

实际上意味着：

```F#
// syntax using "in" keyword
let f () =
  let x = 1 in   // the "in" keyword is available in F#
    let y = 2 in
      x + y      // the result
```

执行替换后，最后一行变为：

```F#
(definition of x) + (definition of y)
// or
(1) + (2)
```

从某种意义上说，嵌套名称只是编译表达式时消失的“宏”或“占位符”。因此，您应该能够看到嵌套的 `let` 对整个表达式没有影响。因此，例如，包含嵌套 `let` 的表达式的类型就是最终主体表达式的类型。

如果你了解嵌套 `let` 绑定是如何工作的，那么某些错误就变得可以理解了。例如，如果嵌套的“let”没有“in”，则整个表达式不完整。在下面的示例中，let 行后面没有任何内容，这是一个错误：

```F#
let f () =
  let x = 1
// error FS0588: Block following this 'let' is unfinished.
//               Expect an expression.
```

而且你不能有多个表达式结果，因为你不能有多重身体表达式。在最终主体表达式之前计算的任何内容都必须是“`do`”表达式（见下文），并返回 `unit`。

```F#
let f () =
  2 + 2      // warning FS0020: This expression should
             // have type 'unit'
  let x = 1
  x + 1      // this is the final result
```

在这种情况下，您必须将结果导入“忽略”。

```F#
let f () =
  2 + 2 |> ignore
  let x = 1
  x + 1      // this is the final result
```

## “use”绑定

`use` 关键字的作用与 `let` 相同——它将表达式的结果绑定到一个命名值。

关键的区别在于，当值超出范围时，它也会*自动处理（automatically disposes）*该值。

显然，这意味着 `use` 只适用于嵌套的情况。您不能进行顶级 `use`，如果您尝试，编译器将警告您。

```F#
module A =
    use f () =  // Error
      let x = 1
      x + 1
```

要了解正确 `use` 绑定的工作原理，首先让我们创建一个助手函数，该函数动态创建 `IDisposable`。

```F#
// create a new object that implements IDisposable
let makeResource name =
   { new System.IDisposable
     with member this.Dispose() = printfn "%s disposed" name }
```

现在让我们用嵌套的 `use` 绑定来测试它：

```F#
let exampleUseBinding name =
    use myResource = makeResource name
    printfn "done"

//test
exampleUseBinding "hello"
```

我们可以看到“done”被打印出来，然后紧接着，`myResource` 就超出了作用域，它的 `Dispose` 被调用，并且“hello disposed”也被打印出来。

另一方面，如果我们使用常规的 `let` 绑定进行测试，我们不会得到相同的效果。

```F#
let exampleLetBinding name =
    let myResource = makeResource name
    printfn "done"

//test
exampleLetBinding "hello"
```

在这种情况下，我们看到“done”被打印出来，但从未调用 `Dispose`。

### “use”仅适用于 IDisposables

请注意，“use”绑定仅适用于实现 `IDisposable` 的类型，否则编译器将发出投诉：

```F#
let exampleUseBinding2 name =
    use s = "hello"  // Error: The type 'string' is
                     // not compatible with the type 'IDisposable'
    printfn "done"
```

### 不要返回“use'd”值

重要的是要意识到，一旦值超出声明它的表达式的范围，它就会被处理。如果您试图返回该值以供其他函数使用，则返回值将无效。

以下示例显示了如何不这样做：

```F#
let returnInvalidResource name =
    use myResource = makeResource name
    myResource // don't do this!

// test
let resource = returnInvalidResource  "hello"
```

如果你需要在创建 disposable 的函数“外部”使用它，最好的方法可能是使用回调。

然后，该功能将按如下方式工作：

- 创造 disposable。
- 用 disposable 求值回调
- 调用 disposable 的 `Dispose`

这里有一个例子：

```F#
let usingResource name callback =
    use myResource = makeResource name
    callback myResource
    printfn "done"

let callback aResource = printfn "Resource is %A" aResource
do usingResource "hello" callback
```

这种方法保证了创建 disposable 的相同功能也会处理它，并且没有泄漏的机会。

另一种可能的方法是在创建时不使用 `use` 绑定，而是使用 `let` 绑定，并让调用者负责处理。

这里有一个例子：

```F#
let returnValidResource name =
    // "let" binding here instead of "use"
    let myResource = makeResource name
    myResource // still valid

let testValidResource =
    // "use" binding here instead of "let"
    use resource = returnValidResource  "hello"
    printfn "done"
```

就我个人而言，我不喜欢这种方法，因为它是不对称的，将创建与处置（dispose）分开，这可能会导致资源泄漏。

### “using” 函数

如上所示，共享 disposable 的首选方法是使用回调函数。

有一个内置的 `using` 函数，其工作方式与此相同。它需要两个参数：

- 第一个是创建资源的表达式
- 第二个是使用资源的函数，将其作为参数

这是我们之前用 `using` 函数重写的示例：

```F#
let callback aResource = printfn "Resource is %A" aResource
using (makeResource "hello") callback
```

在实践中，`using` 函数并不经常使用，因为正如我们之前看到的，制作自己的自定义版本非常容易。

### 滥用“use”

F# 中的一个技巧是使用 `use` 关键字自动执行任何类型的“停止”或“恢复”功能。

方法是：

- 为某些类型创建扩展方法
- 在该方法中，启动所需的行为，然后返回一个停止该行为的 `IDisposable`。

例如，这里有一个扩展方法，它启动一个计时器，然后返回一个停止计时器的 `IDisposable`。

```F#
module TimerExtensions =

    type System.Timers.Timer with
        static member StartWithDisposable interval handler =
            // create the timer
            let timer = new System.Timers.Timer(interval)

            // add the handler and start it
            do timer.Elapsed.Add handler
            timer.Start()

            // return an IDisposable that calls "Stop"
            { new System.IDisposable with
                member disp.Dispose() =
                    do timer.Stop()
                    do printfn "Timer stopped"
                }
```

因此，现在在调用代码中，我们创建计时器并将其与 `use` 绑定。当计时器值超出范围时，它将自动停止！

```F#
open TimerExtensions
let testTimerWithDisposable =
    let handler = (fun _ -> printfn "elapsed")
    use timer = System.Timers.Timer.StartWithDisposable 100.0 handler
    System.Threading.Thread.Sleep 500
```

同样的方法可用于其他常见的操作对，例如：

- 打开/连接然后关闭/断开资源（无论如何，`IDisposable` 都应该用于此目的，但您的目标类型可能没有实现它）
- 注册然后注销事件处理程序（而不是使用 `WeakReference`）
- 在 UI 中，在代码块的开头显示启动画面，然后在代码块末尾自动关闭它

我一般不会推荐这种方法，因为它确实隐藏了正在发生的事情，但有时它可能非常有用。

## “do”绑定

有时我们可能希望独立于函数或值定义执行代码。这在模块初始化、类初始化等方面很有用。

也就是说，与其说“`let x = do something`”，不如说我们只是自己“`do something`”。这类似于命令式语言中的语句。

您可以通过在代码前加上“`do`”来实现这一点：

```F#
do printf "logging"
```

在许多情况下，`do` 关键字可以省略：

```F#
printf "logging"
```

但在这两种情况下，表达式都必须返回 unit。如果没有，您将收到编译器错误。

```F#
do 1 + 1    // warning: This expression is a function
```

与往常一样，您可以通过将结果管道化为“`ignore`”来强制丢弃非单位结果。

```F#
do ( 1+1 |> ignore )
```

您还将看到“`do`”关键字以相同的方式在循环中使用。

请注意，尽管您有时可以省略它，但始终有一个明确的“`do`”被认为是一种好做法，因为它充当了您不想要结果，只想要副作用的文档。

### “do”用于模块初始化

就像 `let` 一样，`do` 既可以在嵌套上下文中使用，也可以在模块或类的顶层使用。

当在模块级别使用时，`do` 表达式仅在模块首次加载时计算一次。

```F#
module A =

    module B =
        do printfn "Module B initialized"

    module C =
        do printfn "Module C initialized"

    do printfn "Module A initialized"
```

这有点类似于 C# 中的静态类构造函数，除了如果有多个模块，初始化的顺序是固定的，它们是按照声明的顺序初始化的。

## let! 和 use! 和 do!

当你看到 `let!`, `use!` 和 `do!` （即带有感叹号）并且它们是花括号 `{..}` 块的一部分，则它们被用作“计算表达式”的一部分。`let!`, `use!` 和 `do!` 的确切含义在这种情况下，取决于计算表达式本身。一般来说，理解计算表达式必须等待稍后的系列文章。

您将遇到的最常见的计算表达式类型是异步工作流，由 `async{..}` 块表示。在这种情况下，这意味着它们被用来等待异步操作完成，然后才绑定到结果值。

以下是我们之前在“为什么使用 F#？”系列文章中看到的一些例子：

```F#
//This simple workflow just sleeps for 2 seconds.
open System
let sleepWorkflow  = async{
    printfn "Starting sleep workflow at %O" DateTime.Now.TimeOfDay

    // do! means to wait as well
    do! Async.Sleep 2000
    printfn "Finished sleep workflow at %O" DateTime.Now.TimeOfDay
    }

//test
Async.RunSynchronously sleepWorkflow


// Workflows with other async workflows nested inside them.
/// Within the braces, the nested workflows can be blocked on by using the let! or use! syntax.
let nestedWorkflow  = async{

    printfn "Starting parent"

    // let! means wait and then bind to the childWorkflow value
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

## let 和 do 绑定的特性

如果它们位于模块的顶层，`let` 和 `do` 绑定可以具有特性。F# 属性使用语法 `[<MyAttribute>]`。

以下是 C# 中的一些示例，然后是 F# 中的相同代码：

```c#
class AttributeTest
{
    [Obsolete]
    public static int MyObsoleteFunction(int x, int y)
    {
        return x + y;
    }

    [CLSCompliant(false)]
    public static void NonCompliant()
    {
    }
}
```

```F#
module AttributeTest =
    [<Obsolete>]
    let myObsoleteFunction x y = x + y

    [<CLSCompliant(false)>]
    let nonCompliant () = ()
```

让我们简要看看三个特性示例：

- EntryPoint 特性用于指示“main”函数。
- 各种 AssemblyInfo 特性。
- 用于与非托管代码交互的 DllImport 特性。

### EntryPoint 特性

特殊的 `EntryPoint` 特性用于标记独立应用程序的入口点，就像 C# 中的 `static void Main` 方法一样。

下面是熟悉的 C# 版本：

```F#
class Program
{
    static int Main(string[] args)
    {
        foreach (var arg in args)
        {
            Console.WriteLine(arg);
        }

        //same as Environment.Exit(code)
        return 0;
    }
}
```

这是 F# 的等价物：

```F#
module Program

[<EntryPoint>]
let main args =
    args |> Array.iter printfn "%A"

    0  // return is required!
```

就像在 C# 中一样，args 是一个字符串数组。但与 C# 不同，C# 的静态 `Main` 方法可以是 `void`，F# 函数必须返回一个 int。

此外，一个大问题是，具有此特性的函数必须是项目中最后一个文件中的最后一个函数！否则会出现以下错误：

`error FS0191: A function labelled with the 'EntryPointAttribute' attribute must be the last declaration in the last file in the compilation sequence`

为什么 F# 编译器如此挑剔？在 C# 中，类可以去任何地方。

一个可能有帮助的类比是：从某种意义上说，整个应用程序是一个绑定到 `main` 的巨大表达式，其中 `main` 是一个包含子表达式的表达式，而子表达式又包含其他子表达式。

```F#
[<EntryPoint>]
let main args =
    // the entire application as a set of subexpressions
```

现在在 F# 项目中，不允许向前引用。也就是说，引用其他表达式的表达式必须在它们之后声明。因此，从逻辑上讲，它们中最高、最顶级的函数 `main` 必须排在最后。

### AssemblyInfo 特性

在 C# 项目中，有一个 `AssemblyInfo.cs` 文件，其中包含所有程序集级特性。

在 F# 中，等效的方法是使用一个虚拟模块，其中包含一个用这些特性注释的 `do` 表达式。

```F#
open System.Reflection

module AssemblyInfo =
    [<assembly: AssemblyTitle("MyAssembly")>]
    [<assembly: AssemblyVersion("1.2.0.0")>]
    [<assembly: AssemblyFileVersion("1.2.3.4152")>]
    do ()   // do nothing -- just a placeholder for the attribute
```

### DllImport 特性

另一个偶尔有用的特性是 `DllImport` 特性。这是一个 C# 示例。

```F#
using System.Runtime.InteropServices;

[TestFixture]
public class TestDllImport
{
    [DllImport("shlwapi", CharSet = CharSet.Auto, EntryPoint = "PathCanonicalize", SetLastError = true)]
    private static extern bool PathCanonicalize(StringBuilder lpszDst, string lpszSrc);

    [Test]
    public void TestPathCanonicalize()
    {
        var input = @"A:\name_1\.\name_2\..\name_3";
        var expected = @"A:\name_1\name_3";

        var builder = new StringBuilder(260);
        PathCanonicalize(builder, input);
        var actual = builder.ToString();

        Assert.AreEqual(expected,actual);
    }
}
```

它在 F# 和 C# 中的工作方式相同。有一件事需要注意 `extern declaration ...` 将类型放在参数之前，C 风格。

```F#
open System.Runtime.InteropServices
open System.Text

[<DllImport("shlwapi", CharSet = CharSet.Ansi, EntryPoint = "PathCanonicalize", SetLastError = true)>]
extern bool PathCanonicalize(StringBuilder lpszDst, string lpszSrc)

let TestPathCanonicalize() =
    let input = @"A:\name_1\.\name_2\..\name_3"
    let expected = @"A:\name_1\name_3"

    let builder = new StringBuilder(260)
    let success = PathCanonicalize(builder, input)
    let actual = builder.ToString()

    printfn "actual=%s success=%b" actual (expected = actual)

// test
TestPathCanonicalize()
```

与非托管代码的互操作是一个大主题，需要自己的系列。

# 5 F# 语法：缩进和冗长

Part of the "Expressions and syntax" series ([link](https://fsharpforfunandprofit.com/posts/fsharp-syntax/#series-toc))

理解越位（offside）规则
2012年5月18日 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/fsharp-syntax/

F# 的语法基本上很简单。但是，如果你想避免常见的缩进错误，你应该了解一些规则。如果你熟悉像 Python 这样对空格敏感的语言，请注意 F# 中的缩进规则略有不同。

## 缩进和“越位”规则

在足球比赛中，越位规则规定，在某些情况下，球员不能“领先”于球，而应该落后于球或与球平齐。“越位线”是指球员不得越过的线。F# 使用相同的术语来描述缩进必须开始的行。与足球一样，避免点球的诀窍是知道底线在哪里，不要领先。

通常，一旦设置了越位线，所有表达式都必须与该线对齐。

```F#
//character columns
//3456789
let f =
  let x=1     // offside line is at column 3
  let y=1     // this line must start at column 3
  x+y         // this line must start at column 3

let f =
  let x=1     // offside line is at column 3
   x+1        // oops! don't start at column 4
              // error FS0010: Unexpected identifier in binding

let f =
  let x=1    // offside line is at column 3
 x+1         // offside! You are ahead of the ball!
             // error FS0588: Block following this
             // 'let' is unfinished
```

各种词元（token）可以触发创建新的越位线。例如，当 F# 看到 let 表达式中使用的“`=`”时，会在遇到的下一个符号或单词的位置创建一条新的越位线。

```F#
//character columns
//34567890123456789
let f =   let x=1  // line is now at column 11 (start of "let x=")
          x+1      // must start at column 11 from now on

//        |        // offside line at col 11
let f =   let x=1  // line is now at column 11 (start of "let x=")
         x+1       // offside!


// |        // offside line at col 4
let f =
   let x=1  // first word after = sign defines the line
            // offside line is now at column 4
   x+1      // must start at column 4 from now on
```

其他标记（tokens）具有相同的行为，包括括号、“`then`”、“`else`”、“`try`”、”`finally`”和“`do`”，以及 match 子句中的“`->`”。

```F#
//character columns
//34567890123456789
let f =
   let g = (
    1+2)             // first char after "(" defines
                     // a new line at col 5
   g

let f =
   if true then
    1+2             // first char after "then" defines
                    // a new line at col 5

let f =
   match 1 with
   | 1 ->
       1+2          // first char after match "->" defines
                    // a new line at col 8
```

越位行可以嵌套，并按照您的期望推送和弹出：

```F#
//character columns
//34567890123456789
let f =
   let g = let x = 1 // first word after "let g ="
                     // defines a new offside line at col 12
           x + 1     // "x" must align at col 12
                     // pop the offside line stack now
   g + 1             // back to previous line. "g" must align
                     // at col 4
```

新的越位线永远不会比堆栈上的前一条线前进得更远：

```F#
let f =
   let g = (         // let defines a new line at col 4
  1+2)               // oops! Can't define new line less than 4
   g
```

## 特殊情况

创建了许多特殊情况，使代码格式更加灵活。其中许多看起来很自然，例如对齐 `if-then-else` 表达式或 `try-catch` 表达式的每个部分的开头。然而，也有一些不明显的。

Infix 运算符（如“+”、“|>”和“»”）允许超出行的长度加一个空格：

```F#
//character columns
//34567890123456789
let x =  1   // defines a new line at col 10
       + 2   // "+" allowed to be outside the line
       + 3

let f g h =   g   // defines a new line at col 15
           >> h   // ">>" allowed to be outside the line
```

如果中缀运算符开始一条线，则该线不必严格对齐：

```F#
let x =  1   // defines a new line at col 10
        + 2   // infix operators that start a line don't count
             * 3  // starts with "*" so doesn't need to align
         - 4  // starts with "-" so doesn't need to align
```

如果一个“`fun`”关键字开始一个表达式，则“fun“不会开始一个新的越位行：

```F#
//character columns
//34567890123456789
let f = fun x ->  // "fun" should define a new line at col 9
   let y = 1      // but doesn't. The real line starts here.
   x + y
```

### 了解更多

关于缩进是如何工作的，还有更多的细节，但上面的例子应该涵盖大多数常见情况。如果你想了解更多，微软提供了 F# 的完整语言规范，可下载 PDF，非常值得一读。

## “详细（Verbose）”语法

默认情况下，F# 使用缩进来表示块结构——这被称为“轻”语法。还有一种不使用缩进的替代语法；它被称为“详细”语法。使用冗长的语法，您不需要使用缩进，空格也不重要，但缺点是您需要使用更多的关键字，包括以下内容：

- 每次“let”和“do”绑定后的“`in`”关键字
- 代码块的“`begin`”/“`end`”关键字，如 if-then-else
- 循环末尾的“`done`”关键字
- 类型定义开头和结尾的关键字

这是一个冗长的语法示例，带有古怪的缩进，否则是不可接受的：

```F#
#indent "off"

      let f =
    let x = 1 in
  if x=2 then
begin "a" end else begin
"b"
end

#indent "on"
```

详细语法始终可用，即使在“轻”模式下，偶尔也有用。例如，当你想将“let”嵌入到一行表达式中时：

```F#
let x = let y = 1 in let z = 2 in y + z
```

其他可能需要使用详细语法的情况包括：

- 输出生成的代码时
- 与 OCaml 兼容
- 如果您有视力障碍或失明，请使用屏幕阅读器
- 或者只是为了深入了解 F# 解析器使用的抽象语法树

除了这些情况，在实践中很少使用冗长的语法。

# 6 参数和值命名约定

*Part of the "Expressions and syntax" series (*[link](https://fsharpforfunandprofit.com/posts/naming-conventions/#series-toc)*)*

a、 f、x 和朋友
2012年5月19日 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/naming-conventions/

若你们是从 C# 这样的命令式语言来学习 F# 的，那个么你们可能会发现很多名字比你们习惯的更短、更神秘。

在 C# 和 Java 中，最好的做法是使用长的描述性标识符。在函数式语言中，函数名本身可以是描述性的，但函数内的局部标识符往往很短，并且大量使用管道和组合来将所有内容都放在最少的行上。

例如，这是一个素数筛的粗略实现，对局部值有非常描述性的名称。

```F#
let primesUpTo n =
    // create a recursive intermediate function
    let rec sieve listOfNumbers  =
        match listOfNumbers with
        | [] -> []
        | primeP::sievedNumbersBiggerThanP->
            let sievedNumbersNotDivisibleByP =
                sievedNumbersBiggerThanP
                |> List.filter (fun i-> i % primeP > 0)
            //recursive part
            let newPrimes = sieve sievedNumbersNotDivisibleByP
            primeP :: newPrimes
    // use the sieve
    let listOfNumbers = [2..n]
    sieve listOfNumbers     // return

//test
primesUpTo 100
```

以下是相同的实现，具有更简洁、惯用的名称和更紧凑的代码：

```F#
let primesUpTo n =
   let rec sieve l  =
      match l with
      | [] -> []
      | p::xs ->
            p :: sieve [for x in xs do if (x % p) > 0 then yield x]
   [2..n] |> sieve
```

当然，神秘的名称并不总是更好，但如果函数保持在几行以内，并且使用的操作是标准的，那么这是一个相当常见的习惯用法。

常见的命名约定如下：

- “a”、“b”、“c”等是类型
- “f”、“g”、“h”等是函数
- “x”、“y”、“z”等是函数的参数
- 列表是通过添加“s”后缀来表示的，因此“`xs`”是 `x` 的列表，“`fs`”是函数的列表，以此类推。非常常见的是，“`x::xs`”表示列表的头部（第一个元素）和尾部（其余元素）。
- “`_`”用于不关心值的时候。所以“`x::_`”意味着你不关心列表的其余部分，而“`let f _ = something`”意味着您不关心 `f` 的参数。

名字短的另一个原因是，它们通常不能被赋予任何有意义的东西。例如，管道操作符的定义为：

```F#
let (|>) x f = f x
```

我们不知道 `f` 和 `x` 是什么，`f` 可以是任何函数，`x` 可以是任何值。明确这一点并不能使代码更容易理解。

```F#
let (|>) aValue aFunction = aFunction aValue // any better?
```

## 本网站使用的样式

在这个网站上，我将使用这两种风格。对于介绍性系列，当大多数概念都是新概念时，我将使用非常描述性的风格，中间值和长名称。但在更高级的系列中，风格会变得更简洁。

# 7 控制流表达式

*Part of the "Expressions and syntax" series (*[link](https://fsharpforfunandprofit.com/posts/control-flow-expressions/#series-toc)*)*

以及如何避免使用它们
2012年5月20日 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/control-flow-expressions/

在这篇文章中，我们将研究控制流表达式，即：

- if-then-else
- for x in 集合（与 C# 中的 foreach 相同）
- for x = 开始 to 结束
- while-do

毫无疑问，这些控制流表达式对您来说非常熟悉。但它们是非常“必要的”，而不是函数式的。

因此，我强烈建议你尽可能不要使用它们，尤其是当你正在学习函数式思维时。如果你真的把它们当作拐杖，你会发现很难摆脱命令式思维。

为了帮助你做到这一点，我将在每一节开头举一些例子，说明如何通过使用更习惯的构造来避免使用它们。如果你确实需要使用它们，你需要注意一些“陷阱”。

## If-then-else

### 如何避免使用 if-then-else

避免 `if-then-else` 的最佳方法是使用“match”。您可以在布尔值上进行匹配，这类似于经典的 then/else 分支。但更好的是避免相等性测试，并实际匹配事物本身，如下面最后一个实现所示。

```F#
// bad
let f x =
    if x = 1
    then "a"
    else "b"

// not much better
let f x =
    match x=1 with
    | true -> "a"
    | false -> "b"

// best
let f x =
    match x with
    | 1 -> "a"
    | _ -> "b"
```

直接匹配更好的部分原因是，相等性测试丢弃了您经常需要再次检索的有用信息。

这在下一个场景中得到了证明，我们希望获取列表的第一个元素以便打印它。显然，我们必须小心，不要对空列表尝试这样做。

第一个实现执行空测试，然后执行第二个操作以获取第一个元素。更好的方法是在一个步骤中匹配和提取元素，如第二个实现所示。

```F#
// bad
let f list =
    if List.isEmpty list
    then printfn "is empty"
    else printfn "first element is %s" (List.head list)

// much better
let f list =
    match list with
    | [] -> printfn "is empty"
    | x::_ -> printfn "first element is %s" x
```

第二种实现方式不仅更容易理解，而且更高效。

如果布尔测试很复杂，仍然可以通过使用额外的“`when`”子句（称为“guards”）使用 match 来完成。比较下面的第一个和第二个实现，看看区别。

```F#
// bad
let f list =
    if List.isEmpty list
        then printfn "is empty"
        elif (List.head list) > 0
            then printfn "first element is > 0"
            else printfn "first element is <= 0"

// much better
let f list =
    match list with
    | [] -> printfn "is empty"
    | x::_ when x > 0 -> printfn "first element is > 0"
    | x::_ -> printfn "first element is <= 0"
```

同样，第二种实现方式更容易理解，也更高效。

这个故事的寓意是：如果你发现自己使用 if-then-else 或布尔值匹配，可以考虑重构你的代码。

### 如何使用 if-then-else

如果你确实需要使用 if-then-else，请注意，即使语法看起来很熟悉，但你必须注意一个问题：“if-then-else”是一个表达式，而不是一个语句，与 F# 中的每个表达式一样，它必须返回一个特定类型的值。

这里有两个返回类型为字符串的示例。

```F#
let v = if true then "a" else "b"    // value : string
let f x = if x then "a" else "b"     // function : bool->string
```

但因此，两个分支必须返回相同的类型！如果这不是真的，那么表达式作为一个整体无法返回一致的类型，编译器将发出抱怨。

以下是每个分支中不同类型的示例：

```F#
let v = if true then "a" else 2
  // error FS0001: This expression was expected to have
  //               type string but here has type int
```

else 子句是可选的，但如果它不存在，则假定 else 子句返回 unit，这意味着 then 子句也必须返回 unit。如果你犯了这个错误，你会收到编译器的投诉。

```F#
let v = if true then "a"
  // error FS0001: This expression was expected to have type unit
  //               but here has type string
```

如果“then”子句返回 unit，那么编译器会很高兴。

```F#
let v2 = if true then printfn "a"   // OK as printfn returns unit
```

请注意，在分支中无法提前返回。返回值是整个表达式。换句话说，if-then-else 表达式与 C# 三元 if 运算符（`<if expr>?<then expr>：<else expr>`）的关系比与 C# if-then-else 语句的关系更密切。

### 单行程序员的 if-then-else

if-then-else真正有用的地方之一是创建简单的单行程序，用于传递给其他函数。

```F#
let posNeg x = if x > 0 then "+" elif x < 0 then "-" else "0"
[-5..5] |> List.map posNeg
```

### 返回函数

别忘了 if-then-else 表达式可以返回任何值，包括函数值。例如：

```F#
let greetings =
    if (System.DateTime.Now.Hour < 12)
    then (fun name -> "good morning, " + name)
    else (fun name -> "good day, " + name)

//test
greetings "Alice"
```

当然，这两个函数必须具有相同的类型，也就是说，它们必须有相同的函数签名。

## 循环

### 如何避免使用循环

避免循环的最佳方法是使用内置的列表和序列函数。几乎任何你想做的事情都可以在不使用显式循环的情况下完成。通常，作为附带好处，您也可以避免可变值。以下是一些示例，有关更多详细信息，请阅读即将推出的列表和序列操作系列。

示例：打印 10 次：

```F#
// bad
for i = 1 to 10 do
   printf "%i" i

// much better
[1..10] |> List.iter (printf "%i")
```

示例：对列表求和：

```F#
// bad
let sum list =
    let mutable total = 0    // uh-oh -- mutable value
    for e in list do
        total <- total + e   // update the mutable value
    total                    // return the total

// much better
let sum list = List.reduce (+) list

//test
sum [1..10]
```

示例：生成并打印随机数序列：

```F#
// bad
let printRandomNumbersUntilMatched matchValue maxValue =
  let mutable continueLooping = true  // another mutable value
  let randomNumberGenerator = new System.Random()
  while continueLooping do
    // Generate a random number between 1 and maxValue.
    let rand = randomNumberGenerator.Next(maxValue)
    printf "%d " rand
    if rand = matchValue then
       printfn "\nFound a %d!" matchValue
       continueLooping <- false

// much better
let printRandomNumbersUntilMatched matchValue maxValue =
  let randomNumberGenerator = new System.Random()
  let sequenceGenerator _ = randomNumberGenerator.Next(maxValue)
  let isNotMatch = (<>) matchValue

  //create and process the sequence of rands
  Seq.initInfinite sequenceGenerator
    |> Seq.takeWhile isNotMatch
    |> Seq.iter (printf "%d ")

  // done
  printfn "\nFound a %d!" matchValue

//test
printRandomNumbersUntilMatched 10 20
```

与 if-then-else 一样，这是一种道德；如果你发现自己在使用循环和变量，请考虑重构代码以避免它们。

### 三种类型的循环

如果你想使用循环，那么有三种类型的循环表达式可供选择，它们与 C# 中的循环表达式相似。

- `for-in-do`。这有 `for x in enumerable do something` 的形式。它与 C# 中的 `foreach` 循环相同，是 F# 中最常见的形式。
- `for-to-do`。这有 `for x = start to finish do something` 的形式。它与 C# 中 `for(i=start; i<end; i++)` 循环的标准相同。
- `while-do`。这是 `while test do something` 的形式。它与 C# 中的 `while` 循环相同。请注意，F# 中没有 `do-while` 等效项。

我不会再详细介绍了，因为用法很简单。如果您遇到问题，请查看MSDN文档。

### 如何使用循环

与 if-then-else 表达式一样，循环表达式看起来很熟悉，但也有一些陷阱。

- 所有循环表达式总是返回整个表达式的单位，因此无法从循环内部返回值。
- 与所有“do”绑定一样，循环内的表达式也必须返回unit。
- 没有“break”和“continue”的等价物（无论如何，使用序列通常可以做得更好）

这是一个单位约束的示例。循环中的表达式应该是 unit，而不是 int，所以编译器会抱怨。

```F#
let f =
  for i in [1..10] do
    i + i  // warning: This expression should have type 'unit'

// version 2
let f =
  for i in [1..10] do
    i + i |> ignore   // fixed
```

### 单行程序员的循环

在实践中使用循环的地方之一是列表和序列生成器。

```F#
let myList = [for x in 0..100 do if x*x < 100 then yield x ]
```

## 摘要

我将重复我在文章顶部所说的话：当你学习函数式思维时，一定要避免使用命令式控制流。并理解证明规则的例外情况；使用单行程序是可接受的。

# 8 异常

*Part of the "Expressions and syntax" series (*[link](https://fsharpforfunandprofit.com/posts/exceptions/#series-toc)*)*

抛出和捕获的语法
2012年5月21日 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/exceptions/

就像其他 .NET 语言一样，F# 支持抛出和捕获异常。与控制流表达式一样，语法会让人感到熟悉，但也有一些需要注意的地方。

## 定义自己的异常

在引发/抛出异常时，您可以使用标准系统异常，如 InvalidOperationException，或者您可以使用下面显示的简单语法定义自己的异常类型，其中异常的“内容”是任何 F# 类型：

```F#
exception MyFSharpError1 of string
exception MyFSharpError2 of string * int
```

就是这样！定义新的异常类比 C# 容易得多！

## 抛出异常

抛出异常有三种基本方法

- 使用内置函数之一，如“invalidArg”
- 使用其中一个标准 .NET 异常类
- 使用您自己的自定义异常类型

### 抛出异常，方法1：使用内置函数之一

F# 内置了四个有用的异常关键字：

- `failwith` 抛出一个泛型 `System.Exception`
- `invalidArg` 抛出 `ArgumentException`
- `nullArg` 抛出 `NullArgumentException` 异常
- `invalidOp` 抛出 `InvalidOperationException`

这四个可能涵盖了你经常抛出的大多数异常。以下是它们的使用方法：

```F#
// throws a generic System.Exception
let f x =
   if x then "ok"
   else failwith "message"

// throws an ArgumentException
let f x =
   if x then "ok"
   else invalidArg "paramName" "message"

// throws a NullArgumentException
let f x =
   if x then "ok"
   else nullArg "paramName" "message"

// throws an InvalidOperationException
let f x =
   if x then "ok"
   else invalidOp "message"
```

顺便说一句，`failwith` 有一个非常有用的变体，叫做 `failwithf`，它包含 `printf` 样式的格式，这样你就可以轻松地制作自定义消息：

```F#
open System
let f x =
    if x = "bad" then
        failwithf "Operation '%s' failed at time %O" x DateTime.Now
    else
        printfn "Operation '%s' succeeded at time %O" x DateTime.Now

// test
f "good"
f "bad"
```

### 抛出异常，方法2：使用标准 .NET 异常类之一

你可以 `raise` 任何 .NET 显式异常：

```F#
// you control the exception type
let f x =
   if x then "ok"
   else raise (new InvalidOperationException("message"))
```

### 抛出异常，方法3：使用自己的 F# 异常类型

最后，您可以使用前面定义的自己的类型。

```F#
// using your own F# exception types
let f x =
   if x then "ok"
   else raise (MyFSharpError1 "message")
```

这几乎就是抛出异常的原因。

### 引发异常对函数类型有什么影响？

我们之前说过，if-then-else 表达式的两个分支必须返回相同的类型。但是，如何在这种约束下引发异常呢？

答案是，为了确定表达式类型，任何引发异常的代码都会被忽略。这意味着函数签名将仅基于正常情况，而不是异常情况。

例如，在下面的代码中，异常被忽略，整个函数的签名为 `bool -> int`，正如您所期望的那样。

```F#
let f x =
   if x then 42
   elif true then failwith "message"
   else invalidArg "paramName" "message"
```

问题：如果两个分支都引发异常，你认为函数签名会是什么？

```F#
let f x =
   if x then failwith "error in true branch"
   else failwith "error in false branch"
```

试试看！

## 捕捉异常

与其他语言一样，使用 try-catch 块捕获异常。F# 将其称为 `try-with`，并使用标准模式匹配语法对每种类型的异常进行测试。

```F#
try
    failwith "fail"
with
    | Failure msg -> "caught: " + msg
    | MyFSharpError1 msg -> " MyFSharpError1: " + msg
    | :? System.InvalidOperationException as ex -> "unexpected"
```

如果要捕获的异常是以 `failwith`（例如 System.Exception）或自定义 F# 异常抛出的，则可以使用上面显示的简单标记方法进行匹配。

另一方面，抓住一个特定的 .NET 异常类，您必须使用更复杂的语法进行匹配：

```F#
:? (exception class) as ex
```

同样，与 if-then-else 和循环一样，try-with 块是一个返回值的表达式。这意味着 `try-with` 表达式的所有分支都必须返回相同的类型。

考虑这个例子：

```F#
let divide x y=
    try
        (x+1) / y                      // error here -- see below
    with
    | :? System.DivideByZeroException as ex ->
          printfn "%s" ex.Message
```

当我们试图评估它时，我们会得到一个错误：

`错误FS0043:类型“unit”与类型“int”不匹配`

原因是“`with`”分支是 `unit` 类型，而“`try`”分支是 `int` 类型。因此，这两个分支是不兼容的类型。

为了解决这个问题，我们需要使“`with`”分支也返回 `int` 类型。我们可以使用分号技巧在一行上链接表达式来轻松做到这一点。

```F#
let divide x y=
    try
        (x+1) / y
    with
    | :? System.DivideByZeroException as ex ->
          printfn "%s" ex.Message; 0            // added 0 here!

//test
divide 1 1
divide 1 0
```

既然 `try-with` 表达式有一个定义的类型，那么整个函数就可以被分配一个类型，即 `int->int->int`，正如预期的那样。

和以前一样，如果任何分支抛出异常，在确定类型时都不计算在内。

### 重新考虑异常

如果需要，您可以在 catch 处理程序中调用“`reraise()`”函数，以在调用链上传播相同的异常。这与 C# 的 `throw` 关键字相同。

```F#
let divide x y=
    try
        (x+1) / y
    with
    | :? System.DivideByZeroException as ex ->
          printfn "%s" ex.Message
          reraise()

//test
divide 1 1
divide 1 0
```

## Try-finally

另一个熟悉的表达是 `try-finally`。正如您所料，无论如何都会调用“finally”子句。

```F#
let f x =
    try
        if x then "ok" else failwith "fail"
    finally
        printf "this will always be printed"
```

try-finally 表达式的返回类型作为一个整体始终与“try”子句本身的返回类型相同。“finally”子句对整个表达式的类型没有影响。因此，在上述示例中，整个表达式的类型为 `string`。

“finally”子句必须始终返回单位，因此任何非单位值都将被编译器标记。

```F#
let f x =
    try
        if x then "ok" else failwith "fail"
    finally
        1+1  // This expression should have type 'unit
```

## try-with 与 try-finally 相结合

try-with 和 try-finally 表达式是不同的，不能直接组合成一个表达式。相反，你必须根据情况需要将它们嵌套起来。

```F#
let divide x y=
   try
      try
         (x+1) / y
      finally
         printf "this will always be printed"
   with
   | :? System.DivideByZeroException as ex ->
           printfn "%s" ex.Message; 0
```

## 函数应该抛出异常还是返回错误结构？

当你设计一个函数时，你应该抛出异常，还是返回编码错误的结构？本节将讨论两种不同的方法。

### 双函数方法

一种方法是提供两个函数：一个假设一切正常，否则抛出异常；另一个“tryXXX”函数在出现问题时返回缺失的值。

例如，我们可能想为除法设计两个不同的库函数，一个不处理异常，另一个处理异常：

```F#
// library function that doesn't handle exceptions
let divideExn x y = x / y

// library function that converts exceptions to None
let tryDivide x y =
   try
       Some (x / y)
   with
   | :? System.DivideByZeroException -> None // return missing
```

请注意，在 `tryDivide` 代码中使用 `Some` 和 `None` Option 类型向客户端发出该值是否有效的信号。

对于第一个函数，客户端代码必须显式处理异常。

```F#
// client code must handle exceptions explicitly
try
    let n = divideExn 1 0
    printfn "result is %i" n
with
| :? System.DivideByZeroException as ex -> printfn "divide by zero"
```

请注意，没有强制客户端执行此操作的约束，因此这种方法可能是错误的来源。

使用第二个函数，客户端代码更简单，客户端被约束为处理正常情况和错误情况。

```F#
// client code must test both cases
match tryDivide 1 0 with
| Some n -> printfn "result is %i" n
| None -> printfn "divide by zero"
```

这种“正常 vs 尝试”的方法在 .NET BCL 中很常见，也在少数情况下出现在 F# 库中。例如，在列表模块中：

- 如果找不到 key，`List.find` 将抛出 `KeyNotFoundException`
- 但是 `List.tryFind` 将返回一个 Option 类型，如果找不到键，则返回 `None`

如果你打算使用这种方法，一定要有一个命名约定。例如：

- “doSomethingExn”用于期望客户端捕获异常的函数。
- “tryDoSomething”用于为您处理正常异常的函数。

请注意，我更喜欢在“doSomething”上有一个“Exn”后缀，而不是根本没有后缀。它清楚地表明，即使在正常情况下，您也希望客户端捕获异常。

这种方法的总体问题是，您必须做额外的工作来创建成对的函数，并且如果客户端使用不安全版本的函数，则依赖客户端捕获异常，从而降低了系统的安全性。

### 基于错误代码的方法

> “编写好的基于错误代码的代码很难，但编写好的基于异常的代码真的很难。” Raymond Chen

在函数世界中，返回错误代码（或者更确切地说是错误类型）通常比抛出异常更可取，因此一种标准的混合方法是将常见情况（您希望用户关心的情况）编码为错误类型，但不处理非常不寻常的异常。

通常，最简单的方法就是使用选项类型：`Some` 表示成功，`None` 表示错误。如果错误情况很明显，比如在`tryDivide` 或 `tryParse` 中，就不需要用更详细的错误情况来明确说明。

但有时可能存在多个错误，每个错误都应该以不同的方式处理。在这种情况下，每个错误都有一个 case 的联合类型是有用的。

在以下示例中，我们希望执行 SqlCommand。三种非常常见的错误情况是登录错误、约束错误和外键错误，因此我们将它们构建到结果结构中。所有其他错误都作为例外情况提出。

```F#
open System.Data.SqlClient

type NonQueryResult =
    | Success of int
    | LoginError of SqlException
    | ConstraintError of SqlException
    | ForeignKeyError of SqlException

let executeNonQuery (sqlCommmand:SqlCommand) =
    try
       use sqlConnection = new SqlConnection("myconnection")
       sqlCommmand.Connection <- sqlConnection
       let result = sqlCommmand.ExecuteNonQuery()
       Success result
    with
    | :?SqlException as ex ->     // if a SqlException
        match ex.Number with
        | 18456 ->                // login Failed
            LoginError ex
        | 2601 | 2627 ->          // handle constraint error
            ConstraintError ex
        | 547 ->                  // handle FK error
            ForeignKeyError ex
        | _ ->                    // don't handle any other cases
            reraise()
       // all non SqlExceptions are thrown normally
```

然后，客户端被迫处理常见情况，而不常见的异常将被调用链上更高级别的处理程序捕获。

```F#
let myCmd = new SqlCommand("DELETE Product WHERE ProductId=1")
let result =  executeNonQuery myCmd
match result with
| Success n -> printfn "success"
| LoginError ex -> printfn "LoginError: %s" ex.Message
| ConstraintError ex -> printfn "ConstraintError: %s" ex.Message
| ForeignKeyError ex -> printfn "ForeignKeyError: %s" ex.Message
```

与传统的错误代码方法不同，函数的调用者不必立即处理任何错误，只需将结构传递给知道如何处理它的人，如下所示：

```F#
let lowLevelFunction commandString =
  let myCmd = new SqlCommand(commandString)
  executeNonQuery myCmd          //returns result

let deleteProduct id =
  let commandString = sprintf "DELETE Product WHERE ProductId=%i" id
  lowLevelFunction commandString  //returns without handling errors

let presentationLayerFunction =
  let result = deleteProduct 1
  match result with
  | Success n -> printfn "success"
  | errorCase -> printfn "error %A" errorCase
```

另一方面，与 C# 不同，表达式的结果不能被意外丢弃。因此，如果一个函数返回错误结果，调用者必须处理它（除非它真的想表现得很糟糕并将其发送 `ignore`）

```F#
let presentationLayerFunction =
  do deleteProduct 1    // error: throwing away a result code!
```

# 9 匹配表达式

*Part of the "Expressions and syntax" series (*[link](https://fsharpforfunandprofit.com/posts/match-expression/#series-toc)*)*

F# 的主力
28六月2012 这篇文章是超过3年

https://fsharpforfunandprofit.com/posts/match-expression/

模式匹配在 F# 中无处不在。它用于使用 `let` 将值绑定到表达式，在函数参数中，以及使用 `match..with` 语法进行分支。

我们在“为什么使用 F#？”系列的一篇文章中简要介绍了将值绑定到表达式，在研究类型时，我们将多次介绍。

所以在这篇文章中，我们将介绍 `match..with` 语法及其在控制流中的应用。

## 什么是匹配表达式？

我们已经多次看过 `match..with` 表达式。我们知道它有以下形式：

```F#
match [something] with
| pattern1 -> expression1
| pattern2 -> expression2
| pattern3 -> expression3
```

如果你仔细看，它看起来有点像一系列 lambda 表达式：

```F#
match [something] with
| lambda-expression-1
| lambda-expression-2
| lambda-expression-3
```

其中每个 lambda 表达式只有一个参数：

`param -> expression`

所以，关于 `match..with` 的一种思考方式是它是在一组 lambda 表达式之间进行选择。但如何做出选择呢？

这就是模式发挥作用的地方。选择是基于“match with”值是否可以与 lambda 表达式的参数匹配。第一个参数可以与输入值匹配的 lambda “获胜”！

例如，如果参数是通配符 `_`，它将始终匹配，如果是第一个，则始终获胜。

`_ -> expression`

### 顺序很重要！

请看以下示例：

```F#
let x =
    match 1 with
    | 1 -> "a"
    | 2 -> "b"
    | _ -> "z"
```

我们可以看到有三个 lambda 表达式要匹配，按此顺序排列：

```F#
fun 1 -> "a"
fun 2 -> "b"
fun _ -> "z"
```

因此，首先尝试 `1` 模式，然后尝试 `2` 模式，最后尝试 `_` 模式。

另一方面，如果我们改变顺序将通配符放在第一位，它将首先尝试，并且总是立即获胜：

```F#
let x =
    match 1 with
    | _ -> "z"
    | 1 -> "a"
    | 2 -> "b"
```

在这种情况下，F# 编译器会提醒我们，其他规则永远不会匹配。

因此，这是“`switch`”或“`case`”语句与 `match..with` 语句之间的一个主要区别。在 `match..with` 中，顺序很重要。

## 格式化匹配表达式

由于 F# 对缩进很敏感，您可能想知道如何最好地格式化此表达式，因为其中有相当多的移动部分。

关于 F# 语法的文章概述了对齐的工作原理，但对于 `match..with` 表达式，这里有一些具体的指导方针。

### 准则1：`| 表达式`子句的对齐应直接在 `match` 下面

这一指导方针很简单。

```F#
let f x =   match x with
            // aligned
            | 1 -> "pattern 1"
            // aligned
            | 2 -> "pattern 2"
            // aligned
            | _ -> "anything"
```

### 准则2：`match..with` 应该在新行上

`match..with` 可以在同一行或新行上，但使用新行可以保持缩进的一致性，与名称的长度无关：

```F#
                                              // ugly alignment!
let myVeryLongNameForAFunction myParameter =  match myParameter with
                                              | 1 -> "something"
                                              | _ -> "anything"

// much better
let myVeryLongNameForAFunction myParameter =
    match myParameter with
    | 1 -> "something"
    | _ -> "anything"
```

### 准则3：箭头 `->` 后的表达式应位于新行上

同样，结果表达式可以与箭头在同一条线上，但再次使用新行可以保持缩进的一致性，并有助于将匹配模式与结果表达式分开。

```F#
let f x =
    match x with
    | "a very long pattern that breaks up the flow" -> "something"
    | _ -> "anything"

let f x =
    match x with
    | "a very long pattern that breaks up the flow" ->
        "something"
    | _ ->
        "anything"
```

当然，当所有模式都非常紧凑时，可以做出一个常识性的例外：

```F#
let f list =
    match list with
    | [] -> "something"
    | x::xs -> "something else"
```

## match..with 是一个表达式

认识到 `match..with` 并不是真正的“控制流”构造很重要。“控制”并不是沿着分支“流动”，而是整个东西是一个在某个时候被求值的表达式，就像任何其他表达式一样。实践中的最终结果可能是相同的，但这是一个重要的概念差异。

它是一个表达式的一个后果是，所有分支的计算结果必须是相同的类型——我们已经在 if-then-else 表达式和 for 循环中看到了这种相同的行为。

```F#
let x =
    match 1 with
    | 1 -> 42
    | 2 -> true  // error wrong type
    | _ -> "hello" // error wrong type
```

您不能混合和匹配表达式中的类型。

### 您可以在任何地方使用匹配表达式

由于它们是正则表达式，匹配表达式可以出现在任何可以使用表达式的地方。

例如，这是一个嵌套的匹配表达式：

```F#
// nested match..withs are ok
let f aValue =
    match aValue with
    | x ->
        match x with
        | _ -> "something"
```

这是嵌入在 lambda 中的匹配表达式：

```F#
[2..10]
|> List.map (fun i ->
        match i with
        | 2 | 3 | 5 | 7 -> sprintf "%i is prime" i
        | _ -> sprintf "%i is not prime" i
        )
```

## 穷举的匹配

作为表达式的另一个后果是，必须总是有一些匹配的分支。表达式作为一个整体必须计算出某个值！

也就是说，“穷举匹配”的宝贵概念来自 F# 的“一切都是一种表达式”的本质。在面向语句的语言中，不需要发生这种情况。

以下是一个不完全匹配的示例：

```F#
let x =
    match 42 with
    | 1 -> "a"
    | 2 -> "b"
```

如果编译器认为缺少分支，它会警告您。如果你故意忽略警告，那么当所有模式都不匹配时，你会得到一个严重的运行时错误（`MatchFailureException`）。

### 穷尽匹配并不完美

用于检查是否列出了所有可能匹配的算法是好的，但并不总是完美的。有时它会抱怨你没有匹配所有可能的情况，而你知道你已经匹配了。在这种情况下，您可能需要添加一个额外的案例来让编译器满意。

### 使用（并避免）通配符匹配

保证始终匹配所有情况的一种方法是将通配符参数作为最后一个匹配项：

```F#
let x =
    match 42 with
    | 1 -> "a"
    | 2 -> "b"
    | _ -> "z"
```

你经常看到这种模式，我在这些例子中使用了很多。这相当于在 switch 语句中有一个包罗万象的 `default` 值。

但是，如果你想获得穷举模式匹配的全部好处，我建议你不要使用通配符，如果可能的话，尽量明确地匹配所有情况。如果您在联合类型的情况下进行匹配，则尤其如此：

```F#
type Choices = A | B | C
let x =
    match A with
    | A -> "a"
    | B -> "b"
    | C -> "c"
    //NO default match
```

通过始终以这种方式显式，您可以捕获因向联合添加新案例而导致的任何错误。如果你有一个通配符匹配，你永远不会知道。

如果你不能让每个案例都明确，你可以尝试尽可能多地记录你的边界条件，并为通配符案例断言运行时错误。

```F#
let x =
    match -1 with
    | 1 -> "a"
    | 2 -> "b"
    | i when i >= 0 && i<=100 -> "ok"
    // the last case will always match
    | x -> failwithf "%i is out of range" x
```

## 模式类型

匹配模式的方法有很多种，我们接下来会介绍。

有关各种模式的更多详细信息，请参阅 MSDN 文档。

### 与值绑定

最基本的模式是绑定到一个值作为匹配的一部分：

```F#
let y =
    match (1,0) with
    // binding to a named value
    | (1,x) -> printfn "x=%A" x
```

*顺便说一句，我故意让这个模式（以及本文中的其他模式）不完整。作为练习，在不使用通配符的情况下完成它们。*

值得注意的是，绑定的值对于每种模式都必须是不同的。所以你不能这样做：

```F#
let elementsAreEqual aTuple =
    match aTuple with
    | (x,x) ->
        printfn "both parts are the same"
    | (_,_) ->
        printfn "both parts are different"
```

相反，你必须做这样的事情：

```F#
let elementsAreEqual aTuple =
    match aTuple with
    | (x,y) ->
        if (x=y) then printfn "both parts are the same"
        else printfn "both parts are different"
```

第二个选项也可以使用“guards”（`when` 子句）重写。警卫将很快讨论。

### AND 和 OR

您可以在一行中组合多个模式，使用OR逻辑和and逻辑：

```F#
let y =
    match (1,0) with
    // OR  -- same as multiple cases on one line
    | (2,x) | (3,x) | (4,x) -> printfn "x=%A" x

    // AND  -- must match both patterns at once
	// Note only a single "&" is used
    | (2,x) & (_,1) -> printfn "x=%A" x
```

在匹配大量并集情况时，OR 逻辑尤其常见：

```F#
type Choices = A | B | C | D
let x =
    match A with
    | A | B | C -> "a or b or c"
    | D -> "d"
```

### 在列表上匹配

列表可以显式匹配为 `[x;y;z]` 形式或“cons”形式 `head::tail`:

```F#
let y =
    match [1;2;3] with
    // binding to explicit positions
    // square brackets used!
    | [1;x;y] -> printfn "x=%A y=%A" x y

    // binding to head::tail.
    // no square brackets used!
    | 1::tail -> printfn "tail=%A" tail

    // empty list
    | [] -> printfn "empty"
```

类似的语法可用于精确匹配数组 `[|x;y;z|]`。

重要的是要明白，序列（也称为 `IEnumerables`）不能直接以这种方式匹配，因为它们是“懒惰的”，意味着一次只能访问一个元素。另一方面，列表和数组完全可供匹配。

在这些模式中，最常见的是“cons”模式，通常与递归结合使用，以循环遍历列表中的元素。

以下是一些使用递归循环列表的示例：

```F#
// loop through a list and print the values
let rec loopAndPrint aList =
    match aList with
    // empty list means we're done.
    | [] ->
        printfn "empty"

    // binding to head::tail.
    | x::xs ->
        printfn "element=%A," x
        // do all over again with the
        // rest of the list
        loopAndPrint xs

//test
loopAndPrint [1..5]

// ------------------------
// loop through a list and sum the values
let rec loopAndSum aList sumSoFar =
    match aList with
    // empty list means we're done.
    | [] ->
        sumSoFar

    // binding to head::tail.
    | x::xs ->
        let newSumSoFar = sumSoFar + x
        // do all over again with the
        // rest of the list and the new sum
        loopAndSum xs newSumSoFar

//test
loopAndSum [1..5] 0
```

第二个例子展示了如何使用特殊的“累加器”参数（在本例中称为 `sumSoFar`）将状态从循环的一次迭代转移到下一次迭代。这是一种非常常见的模式。

### 元组、记录和联合的匹配

模式匹配适用于所有内置的 F# 类型。本系列中关于类型的更多详细信息。

```F#
// -----------------------
// Tuple pattern matching
let aTuple = (1,2)
match aTuple with
| (1,_) -> printfn "first part is 1"
| (_,2) -> printfn "second part is 2"


// -----------------------
// Record pattern matching
type Person = {First:string; Last:string}
let person = {First="john"; Last="doe"}
match person with
| {First="john"}  -> printfn "Matched John"
| _  -> printfn "Not John"

// -----------------------
// Union pattern matching
type IntOrBool= I of int | B of bool
let intOrBool = I 42
match intOrBool with
| I i  -> printfn "Int=%i" i
| B b  -> printfn "Bool=%b" b
```

### 将整体和部分与“as”关键字匹配

有时，您希望匹配价值的各个组成部分以及整个事物。您可以为此使用as关键字。

```F#
let y =
    match (1,0) with
    // binding to three values
    | (x,y) as t ->
        printfn "x=%A and y=%A" x y
        printfn "The whole tuple is %A" t
```

### 子类型匹配

您可以使用以下命令匹配子类型：？操作符，它为您提供了一个粗略的多态性：

```F#
let x = new Object()
let y =
    match x with
    | :? System.Int32 ->
        printfn "matched an int"
    | :? System.DateTime ->
        printfn "matched a datetime"
    | _ ->
        printfn "another type"
```

这只适用于查找父类（在本例中为 Object）的子类。表达式的整体类型以父类作为输入。

请注意，在某些情况下，您可能需要“装箱”该值。

```F#
let detectType v =
    match v with
        | :? int -> printfn "this is an int"
        | _ -> printfn "something else"
// error FS0008: This runtime coercion or type test from type 'a to int
// involves an indeterminate type based on information prior to this program point.
// Runtime type tests are not allowed on some types. Further type annotations are needed.
```

消息告诉您问题：“不允许对某些类型进行运行时类型测试”。答案是将强制其进入引用类型的值“框”起来，然后您可以对其进行类型检查：

```F#
let detectTypeBoxed v =
    match box v with      // used "box v"
        | :? int -> printfn "this is an int"
        | _ -> printfn "something else"

//test
detectTypeBoxed 1
detectTypeBoxed 3.14
```

在我看来，类型的匹配和调度是一种代码气味，就像面向对象编程一样。偶尔有必要，但不小心使用表明设计不佳。

在一个好的面向对象设计中，正确的方法是使用多态性来替换子类型测试，以及双分派等技术。因此，如果你在F#中做这种OO，你可能应该使用同样的技术。

## 匹配多个值

到目前为止，我们研究的所有模式都是对单个值进行模式匹配的。你怎么能为两个或更多做这件事？

简短的回答是：你不能。只允许对单个值进行匹配。

但是等一下，我们能不能把两个值组合成一个元组，然后进行匹配？是的，我们可以！

```F#
let matchOnTwoParameters x y =
    match (x,y) with
    | (1,y) ->
        printfn "x=1 and y=%A" y
    | (x,1) ->
        printfn "x=%A and y=1" x
```

事实上，每当你想匹配一组值时，这个技巧都会奏效——只需将它们全部组合成一个元组。

```F#
let matchOnTwoTuples x y =
    match (x,y) with
    | (1,_),(1,_) -> "both start with 1"
    | (_,2),(_,2) -> "both end with 2"
    | _ -> "something else"

// test
matchOnTwoTuples (1,3) (1,2)
matchOnTwoTuples (3,2) (1,2)
```

## 守卫，或“when”子句

有时模式匹配是不够的，正如我们在这个例子中看到的：

```F#
let elementsAreEqual aTuple =
    match aTuple with
    | (x,y) ->
        if (x=y) then printfn "both parts are the same"
        else printfn "both parts are different"
```

模式匹配仅基于模式——它不能使用函数或其他类型的条件测试。

但是，作为模式匹配的一部分，有一种方法可以进行相等性测试——使用函数箭头左侧的额外 `when` 子句。这些条款被称为“守卫(guards)”。

以下是使用守卫（guard）编写的相同逻辑：

```F#
let elementsAreEqual aTuple =
    match aTuple with
    | (x,y) when x=y ->
        printfn "both parts are the same"
    | _ ->
        printfn "both parts are different"
```

这更好，因为我们已经将测试集成到模式本身中，而不是在匹配完成后使用测试。

守卫可以用于纯模式无法用于的各种事情，例如：

- 比较边界值
- 测试对象属性
- 执行其他类型的匹配，例如正则表达式
- 由函数派生的条件句

让我们来看一些例子：

```F#
// --------------------------------
// comparing values in a when clause
let makeOrdered aTuple =
    match aTuple with
    // swap if x is bigger than y
    | (x,y) when x > y -> (y,x)

    // otherwise leave alone
    | _ -> aTuple

//test
makeOrdered (1,2)
makeOrdered (2,1)

// --------------------------------
// testing properties in a when clause
let isAM aDate =
    match aDate:System.DateTime with
    | x when x.Hour <= 12->
        printfn "AM"

    // otherwise leave alone
    | _ ->
        printfn "PM"

//test
isAM System.DateTime.Now

// --------------------------------
// pattern matching using regular expressions
open System.Text.RegularExpressions

let classifyString aString =
    match aString with
    | x when Regex.Match(x,@".+@.+").Success->
        printfn "%s is an email" aString

    // otherwise leave alone
    | _ ->
        printfn "%s is something else" aString


//test
classifyString "alice@example.com"
classifyString "google.com"

// --------------------------------
// pattern matching using arbitrary conditionals
let fizzBuzz x =
    match x with
    | i when i % 15 = 0 ->
        printfn "fizzbuzz"
    | i when i % 3 = 0 ->
        printfn "fizz"
    | i when i % 5 = 0 ->
        printfn "buzz"
    | i  ->
        printfn "%i" i

//test
[1..30] |> List.iter fizzBuzz
```

### 使用主动模式而不是守卫

守卫非常适合一次性匹配。但如果你反复使用某些守卫，可以考虑使用主动模式。

例如，上面的电子邮件示例可以重写如下：

```F#
open System.Text.RegularExpressions

// create an active pattern to match an email address
let (|EmailAddress|_|) input =
   let m = Regex.Match(input,@".+@.+")
   if (m.Success) then Some input else None

// use the active pattern in the match
let classifyString aString =
    match aString with
    | EmailAddress x ->
        printfn "%s is an email" x

    // otherwise leave alone
    | _ ->
        printfn "%s is something else" aString

//test
classifyString "alice@example.com"
classifyString "google.com"
```

您可以在上一篇文章中看到其他活动模式的示例。

## “function”关键字

在迄今为止的例子中，我们看到了很多这样的情况：

```F#
let f aValue =
    match aValue with
    | _ -> "something"
```

在函数定义的特殊情况下，我们可以通过使用 `function` 关键字来大大简化这一点。

```F#
let f =
    function
    | _ -> "something"
```

如您所见，`aValue` 参数与 `match..with` 一起完全消失了。

这个关键字与标准 lambdas 的 `fun` 关键字不同，它结合了 `fun` 和 `match..with`，只需一步。

`function` 关键字适用于任何可以使用函数定义或 lambda 的地方，例如嵌套匹配：

```F#
// using match..with
let f aValue =
    match aValue with
    | x ->
        match x with
        | _ -> "something"

// using function keyword
let f =
    function
    | x ->
        function
        | _ -> "something"
```

或者将 lambdas 传递给更高阶函数：

```F#
// using match..with
[2..10] |> List.map (fun i ->
        match i with
        | 2 | 3 | 5 | 7 -> sprintf "%i is prime" i
        | _ -> sprintf "%i is not prime" i
        )

// using function keyword
[2..10] |> List.map (function
        | 2 | 3 | 5 | 7 -> sprintf "prime"
        | _ -> sprintf "not prime"
        )
```

与 `match..with` 相比，`function` 上有一个小缺点就是，您无法看到原始输入值，必须依赖模式中的值绑定。

## 使用 try..with 处理异常

在上一篇文章中，我们研究了如何使用 `try..with` 表达式捕获异常

```F#
try
    failwith "fail"
with
    | Failure msg -> "caught: " + msg
    | :? System.InvalidOperationException as ex -> "unexpected"
```

`try..with` 表达式以与 `match..with` 相同的方式实现模式匹配。

因此，在上面的示例中，我们看到了在自定义模式上使用匹配

- `| Failure msg` 是对活动模式（看起来像什么）进行匹配的一个例子
- `| :? System.InvalidOperationException as ex` 是子类型匹配的一个示例（也使用 `as`）。

因为 `try..with` 表达式实现了完整的模式匹配，如果需要添加额外的条件逻辑，我们也可以使用守卫：

```F#
let debugMode = false
try
    failwith "fail"
with
    | Failure msg when debugMode  ->
        reraise()
    | Failure msg when not debugMode ->
        printfn "silently logged in production: %s" msg
```

## 用函数包装匹配表达式

匹配表达式非常有用，但如果不小心使用，可能会导致代码复杂。

主要问题是匹配表达式的组合不太好。也就是说，很难进行链式 `match..with` 表达式，将简单的表达式构建成复杂的表达式。

避免这种情况的最好办法是把 `match..with` 表达式包起来转换为函数，然后可以很好地组合。

这里有一个简单的例子。带有 `match x with 42` 被包裹在 `isAnswerToEverything` 函数中。

```F#
let times6 x = x * 6

let isAnswerToEverything x =
    match x with
    | 42 -> (x,true)
    | _ -> (x,false)

// the function can be used for chaining or composition
[1..10] |> List.map (times6 >> isAnswerToEverything)
```

### 替换显式匹配的库函数

大多数内置的 F# 类型已经具有此类功能。

例如，您应该尝试使用 `List` 模块中的函数，而不是使用递归来遍历列表，这些函数几乎可以完成您需要的一切。

特别是我们之前写的函数：

```F#
let rec loopAndSum aList sumSoFar =
    match aList with
    | [] ->
        sumSoFar
    | x::xs ->
        let newSumSoFar = sumSoFar + x
        loopAndSum xs newSumSoFar
```

可以使用 `List` 模块以至少三种不同的方式重写！

```F#
// simplest
let loopAndSum1 aList = List.sum aList
[1..10] |> loopAndSum1

// reduce is very powerful
let loopAndSum2 aList = List.reduce (+) aList
[1..10] |> loopAndSum2

// fold is most powerful of all
let loopAndSum3 aList = List.fold (fun sum i -> sum+i) 0 aList
[1..10] |> loopAndSum3
```

同样，Option 类型（本文将详细讨论）有一个关联的 `Option` 模块，其中包含许多有用的功能。

例如，在 `Some` vs `None` 上进行匹配的函数可以用 `Option.map` 替换：

```F#
// unnecessary to implement this explicitly
let addOneIfValid optionalInt =
    match optionalInt with
    | Some i -> Some (i + 1)
    | None -> None

Some 42 |> addOneIfValid

// much easier to use the built in function
let addOneIfValid2 optionalInt =
    optionalInt |> Option.map (fun i->i+1)

Some 42 |> addOneIfValid2
```

### 创建“折叠”函数以隐藏匹配逻辑

最后，如果你创建了自己的需要频繁匹配的类型，最好创建一个相应的通用“fold”函数来很好地封装它。

例如，这里有一个定义温度的类型。

```F#
type TemperatureType  = F of float | C of float
```

很有可能，我们会大量匹配这些情况，所以让我们创建一个通用函数来为我们进行匹配。

```F#
module Temperature =
    let fold fahrenheitFunction celsiusFunction aTemp =
        match aTemp with
        | F f -> fahrenheitFunction f
        | C c -> celsiusFunction c
```

所有 `fold` 函数都遵循相同的一般模式：

- 在联合结构（或匹配模式中的子句）中，每种情况都有一个函数
- 最后，要匹配的实际值排在最后。（为什么？请参阅“为部分应用设计功能”的帖子）

现在我们有了 fold 函数，我们可以在不同的环境中使用它。

让我们先测试一下发烧。我们需要一个函数来测试发烧的华氏度，另一个函数用于测试发烧的摄氏度。

然后我们使用 fold 函数将它们结合起来。

```F#
let fFever tempF =
    if tempF > 100.0 then "Fever!" else "OK"

let cFever tempC =
    if tempC > 38.0 then "Fever!" else "OK"

// combine using the fold
let isFever aTemp = Temperature.fold fFever cFever aTemp
```

现在我们可以测试了。

```F#
let normalTemp = C 37.0
let result1 = isFever normalTemp

let highTemp = F 103.1
let result2 = isFever highTemp
```

对于完全不同的用途，让我们编写一个温度转换实用程序。

我们再次从为每种情况编写函数开始，然后将它们组合起来。

```F#
let fConversion tempF =
    let convertedValue = (tempF - 32.0) / 1.8
    TemperatureType.C convertedValue    //wrapped in type

let cConversion tempC =
    let convertedValue = (tempC * 1.8) + 32.0
    TemperatureType.F convertedValue    //wrapped in type

// combine using the fold
let convert aTemp = Temperature.fold fConversion cConversion aTemp
```

请注意，转换函数将转换后的值包装在新的 `TemperatureType` 中，因此 `convert` 函数具有签名：

```F#
val convert : TemperatureType -> TemperatureType
```

现在我们可以测试了。

```F#
let c20 = C 20.0
let resultInF = convert c20

let f75 = F 75.0
let resultInC = convert f75
```

我们甚至可以连续两次调用 convert，我们应该能恢复到最初的温度！

```F#
let resultInC = C 20.0 |> convert |> convert
```

在即将到来的递归和递归类型系列中，将有更多关于折叠的讨论。

# 10 使用 printf 格式化文本

*Part of the "Expressions and syntax" series (*[link](https://fsharpforfunandprofit.com/posts/printf/#series-toc)*)*

打印和记录的技巧和技术
2012年7月12日 这篇文章已有3年多的历史了

https://fsharpforfunandprofit.com/posts/printf/

在这篇文章中，我们将绕道而行，看看如何创建格式化文本。打印和格式化功能在技术上是库功能，但在实践中，它们被当作核心语言的一部分来使用。

F# 支持两种不同的文本格式样式：

- 标准 .NET 的“复合格式化”技术，如 `String.Format`，`Console.WriteLine` 和其他地方所示。
- 使用 `printf` 和相关函数家族（如 `printfn`、`sprintf` 等）的 C 风格技术。

## String.Format vs printf

复合格式技术在所有 .NET 语言情况下都是可用的，你可能从 C# 就熟悉它了。

```F#
Console.WriteLine("A string: {0}. An int: {1}. A float: {2}. A bool: {3}","hello",42,3.14,true)
```

另一方面，printf 技术基于 C 风格的格式字符串：

```F#
printfn "A string: %s. An int: %i. A float: %f. A bool: %b" "hello" 42 3.14 true
```

如您所见，`printf` 技术在 F# 中很常见，而 `String.Format`、`Console.Write` 等则很少使用。

为什么 `printf` 是 F# 的首选和习惯用法？原因如下：

- 它是静态类型检查的。
- 它是一个行为良好的 F# 函数，因此支持部分应用等。
- 它支持原生 F# 类型。

### printf 是静态类型检查的

与 `String.Format` 不同，`printf` 对参数的类型和数字都进行了静态类型检查。

例如，这里有两个使用 `printf` 的代码片段将无法编译：

```F#
// wrong parameter type
printfn "A string: %s" 42

// wrong number of parameters
printfn "A string: %s" "Hello" 42
```

使用复合格式的等效代码可以很好地编译，但要么无法正常工作，要么在运行时出现错误：

```F#
// wrong parameter type
Console.WriteLine("A string: {0}", 42)   //works!

// wrong number of parameters
Console.WriteLine("A string: {0}","Hello",42)  //works!
Console.WriteLine("A string: {0}. An int: {1}","Hello") //FormatException
```

### printf 支持部分应用

这个 .NET 格式化函数要求同时传入所有参数。

但是 `printf` 是一个标准的、行为良好的 F# 函数，因此支持部分应用。

以下是一些示例：

```F#
// partial application - explicit parameters
let printStringAndInt s i =  printfn "A string: %s. An int: %i" s i
let printHelloAndInt i = printStringAndInt "Hello" i
do printHelloAndInt 42

// partial application - point free style
let printInt =  printfn "An int: %i"
do printInt 42
```

当然，`printf` 可以用于任何可以使用标准函数的函数参数。

```F#
let doSomething printerFn x y =
    let result = x + y
    printerFn "result is" result

let callback = printfn "%s %i"
do doSomething callback 3 4
```

这也包括列表等的高阶函数：

```F#
[1..5] |> List.map (sprintf "i=%i")
```

### printf 支持原生 F# 类型

对于非基本类型， .NET 格式化函数仅支持使用 `ToString()`，但 `printf` 支持使用 `%A` 说明符的本机 F# 类型：

```F#
// tuple printing
let t = (1,2)
Console.WriteLine("A tuple: {0}", t)
printfn "A tuple: %A" t

// record printing
type Person = {First:string; Last:string}
let johnDoe = {First="John"; Last="Doe"}
Console.WriteLine("A record: {0}", johnDoe )
printfn "A record: %A" johnDoe

// union types printing
type Temperature = F of int | C of int
let freezing = F 32
Console.WriteLine("A union: {0}", freezing )
printfn "A union: %A" freezing
```

正如您所看到的，元组类型有一个很好的 `ToString()`，但其他用户定义的类型并没有，所以如果您想和 .NET 格式化函数它们一起使用的话，您必须显式重写 `ToString()` 方法。

## printf 有问题

使用 `printf` 时需要注意几个“陷阱”。

首先，如果参数太少，而不是太多，编译器不会立即抱怨，但可能会在以后给出隐晦的错误。

```F#
// too few parameters
printfn "A string: %s An int: %i" "Hello"
```

当然，原因是这根本不是一个错误；`printf` 只是部分应用！如果你不清楚为什么会发生这种情况，请参阅部分应用的讨论。

另一个问题是，“格式字符串”实际上不是字符串。

在 .NET 格式化模型，格式化字符串是普通字符串，因此您可以传递它们，将它们存储在资源文件中，等等。这意味着以下代码工作正常：

```F#
let netFormatString = "A string: {0}"
Console.WriteLine(netFormatString, "hello")
```

另一方面，作为 `printf` 的第一个参数的“格式字符串”根本不是真正的字符串，而是一种称为 `TextWriterFormat` 的东西。这意味着以下代码**不**起作用：

```F#
let fsharpFormatString = "A string: %s"
printfn fsharpFormatString  "Hello"
```

编译器在幕后做了一些魔术，将字符串常量“`A string: %s`”转换为适当的 TextWriterFormat。TextWriterFormat 是“知道”格式字符串类型的关键组件，例如 `string->unit` 或 `string->int->unit`，这反过来又允许 `printf` 是类型安全的。

如果要模拟编译器，可以使用 `Microsoft.FSharp.Core.Printf` 模块的 `Printf.TextWriteFormat` 类型从字符串创建自己的值。

如果格式字符串是“内联”的，编译器可以在绑定过程中为您推断类型：

```F#
let format:Printf.TextWriterFormat<_> = "A string: %s"
printfn format "Hello"
```

但是，如果格式字符串是真正动态的（例如存储在资源中或动态创建），编译器无法为您推断类型，您必须显式地为其提供构造函数。

在下面的示例中，我的第一个格式字符串有一个字符串参数并返回一个单位，因此我必须指定 `string->unit` 作为格式类型。在第二种情况下，我必须指定 `string->int->unit` 作为格式类型。

```F#
let formatAString = "A string: %s"
let formatAStringAndInt = "A string: %s. An int: %i"

//convert to TextWriterFormat
let twFormat1  = Printf.TextWriterFormat<string->unit>(formatAString)
printfn twFormat1 "Hello"
let twFormat2  = Printf.TextWriterFormat<string->int->unit>(formatAStringAndInt)
printfn twFormat2  "Hello" 42
```

我现在不会详细介绍 `printf` 和 `TextWriterFormat` 是如何协同工作的——请注意，这不仅仅是传递简单格式字符串的问题。

最后，值得注意的是，`printf` 和家族不是线程安全的，而 `Console.Write` 和家族则是。

## 如何指定格式

“%”格式规范与C中使用的格式规范非常相似，但对 F# 进行了一些特殊的自定义。

与C一样，紧随 `%` 之后的字符具有特定含义，如下所示。

```
%[flags][width][.precision]specifier
```

我们将在下面更详细地讨论这些属性中的每一个。

### 为傻瓜格式化

最常用的格式说明符是：

- `%s` 代表字符串
- `%b` 代表布尔值
- `%i` 代表整数
- `%f` 代表浮点数
- `%A` 用于漂亮地打印元组、记录和联合类型
- `%O` 对于其他对象，使用 `ToString()`

这六个可能会满足你的大部分基本需求。

### 转义 %

`%` 字符本身会导致错误。要摆脱它，只需加倍：

```F#
printfn "unescaped: %" // error
printfn "escape: %%"
```

### 控制宽度和对齐

在格式化固定宽度的列和表时，您需要控制对齐和宽度。

您可以使用“宽度”和“标志”选项来实现这一点。

- `%5s`，`%5i`。一个数字设置值的宽度
- `%*s`、 `%*i`。星号动态设置值的宽度（从参数前的额外参数到格式化）
- `%-s`、 `%-i`。左连字符表示该值。

以下是一些使用中的示例：

```F#
let rows = [ (1,"a"); (-22,"bb"); (333,"ccc"); (-4444,"dddd") ]

// no alignment
for (i,s) in rows do
    printfn "|%i|%s|" i s

// with alignment
for (i,s) in rows do
    printfn "|%5i|%5s|" i s

// with left alignment for column 2
for (i,s) in rows do
    printfn "|%5i|%-5s|" i s

// with dynamic column width=20 for column 1
for (i,s) in rows do
    printfn "|%*i|%-5s|" 20 i s

// with dynamic column width for column 1 and column 2
for (i,s) in rows do
    printfn "|%*i|%-*s|" 20 i 10 s
```

### 格式化整数

基本整数类型有一些特殊选项：

- `%i` 或 `%d` 表示带符号整数
- `%u` 表示无符号整数
- `%x` 和 `%X` 表示小写和大写十六进制
- `%o` 代表八进制

以下是一些示例：

```F#
printfn "signed8: %i unsigned8: %u" -1y -1y
printfn "signed16: %i unsigned16: %u" -1s -1s
printfn "signed32: %i unsigned32: %u" -1 -1
printfn "signed64: %i unsigned64: %u" -1L -1L
printfn "uppercase hex: %X lowercase hex: %x octal: %o" 255 255 255
printfn "byte: %i " 'A'B
```

说明符在整数类型中不强制任何类型安全。正如您从上面的示例中看到的，您可以毫无问题地将带符号的int传递给无符号的说明符。不同的是它的格式。unsigned 说明符将 int 视为 unsigned，无论其实际类型如何。

请注意，`BigInteger` 不是基本整数类型，因此必须使用 `%A` 或 `%O` 对其进行格式化。

```F#
printfn "bigInt: %i " 123456789I  // Error
printfn "bigInt: %A " 123456789I  // OK
```

您可以使用标志控制符号和零填充的格式：

- `%0i` 用零填充
- `%+i` 显示了一个加号
- `% i` 用空格代替加号

以下是一些示例：

```F#
let rows = [ (1,"a"); (-22,"bb"); (333,"ccc"); (-4444,"dddd") ]

// with alignment
for (i,s) in rows do
    printfn "|%5i|%5s|" i s

// with plus signs
for (i,s) in rows do
    printfn "|%+5i|%5s|" i s

// with zero pad
for (i,s) in rows do
    printfn "|%0+5i|%5s|" i s

// with left align
for (i,s) in rows do
    printfn "|%-5i|%5s|" i s

// with left align and plus
for (i,s) in rows do
    printfn "|%+-5i|%5s|" i s

// with left align and space instead of plus
for (i,s) in rows do
    printfn "|% -5i|%5s|" i s
```

### 格式化浮点数和小数

对于浮点类型，还有一些特殊选项：

- `%f` 代表标准格式
- `%e` 或 `%E` 表示指数格式
- `%g` 或 `%G` 表示 `f` 和 `e` 更紧凑。
- `%M` 代表小数

以下是一些示例：

```F#
let pi = 3.14
printfn "float: %f exponent: %e compact: %g" pi pi pi

let petabyte = pown 2.0 50
printfn "float: %f exponent: %e compact: %g" petabyte petabyte petabyte
```

decimal 类型可以与浮点说明符一起使用，但可能会失去一些精度。`%M` 说明符可用于确保不丢失精度。你可以看到这个例子的区别：

```F#
let largeM = 123456789.123456789M  // a decimal
printfn "float: %f decimal: %M" largeM largeM
```

您可以使用精度规范（如 `%.2f` 和 `%.4f`）控制浮点数的精度。对于 `%f` 和 `%e` 说明符，精度会影响小数点后的位数，而对于 `%g`，精度是总位数。这里有一个例子：

```F#
printfn "2 digits precision: %.2f. 4 digits precision: %.4f." 123.456789 123.456789
// output => 2 digits precision: 123.46. 4 digits precision: 123.4568.
printfn "2 digits precision: %.2e. 4 digits precision: %.4e." 123.456789 123.456789
// output => 2 digits precision: 1.23e+002. 4 digits precision: 1.2346e+002.
printfn "2 digits precision: %.2g. 4 digits precision: %.4g." 123.456789 123.456789
// output => 2 digits precision: 1.2e+02. 4 digits precision: 123.5.
```

对齐和宽度标志也适用于浮点数和小数。

```F#
printfn "|%f|" pi     // normal
printfn "|%10f|" pi   // width
printfn "|%010f|" pi  // zero-pad
printfn "|%-10f|" pi  // left aligned
printfn "|%0-10f|" pi // left zero-pad
```

### 自定义格式化功能

有两个特殊的格式说明符允许您传入函数，而不仅仅是一个简单的值。

- `%t` 期望一个函数输出一些没有输入的文本
- `%a` 期望一个函数从给定的输入中输出一些文本

以下是使用 `%t` 的示例：

```F#
open System.IO

//define the function
let printHello (tw:TextWriter) = tw.Write("hello")

//test it
printfn "custom function: %t" printHello
```

显然，由于回调函数不接受任何参数，它可能是一个引用其他值的闭包。以下是一个打印随机数的示例：

```F#
open System
open System.IO

//define the function using a closure
let printRand =
    let rand = new Random()
    // return the actual printing function
    fun (tw:TextWriter) -> tw.Write(rand.Next(1,100))

//test it
for i in [1..5] do
    printfn "rand = %t" printRand
```

对于 `%a` 说明符，回调函数需要一个额外的参数。也就是说，使用 `%a` 说明符时，必须同时传入函数和值以进行格式化。

下面是一个自定义格式化元组的示例：

```F#
open System
open System.IO

//define the callback function
//note that the data parameter comes after the TextWriter
let printLatLong (tw:TextWriter) (lat,long) =
    tw.Write("lat:{0} long:{1}", lat, long)

// test it
let latLongs = [ (1,2); (3,4); (5,6)]
for latLong  in latLongs  do
    // function and value both passed in to printfn
    printfn "latLong = %a" printLatLong latLong
```

### 日期格式

F# 中的日期没有特殊的格式说明符。

如果你想格式化日期，你有几个选项：

- 使用 `ToString` 将日期转换为字符串，然后使用 `%s` 说明符
- 如上所述，使用带有 `%a` 说明符的自定义回调函数

以下是正在使用的两种方法：

```F#
// function to format a date
let yymmdd1 (date:DateTime) = date.ToString("yy.MM.dd")

// function to format a date onto a TextWriter
let yymmdd2 (tw:TextWriter) (date:DateTime) = tw.Write("{0:yy.MM.dd}", date)

// test it
for i in [1..5] do
    let date = DateTime.Now.AddDays(float i)

    // using %s
    printfn "using ToString = %s" (yymmdd1 date)

    // using %a
    printfn "using a callback = %a" yymmdd2 date
```

哪种方法更好？

使用 `%s` 的 `ToString` 更容易测试和使用，但它的效率不如直接写入 TextWriter。

## printf 函数家族

`printf` 函数有许多变体。以下是一个快速指南：

| F# 函数                                         | C# 等效物                                          | 注释                                     |
| ----------------------------------------------- | -------------------------------------------------- | ---------------------------------------- |
| `printf` 和 `printfn`                           | `Console.Write` 和 `Console.WriteLine`             | 以“print”开头的函数写入标准输出。        |
| `eprintf` 和 `eprintfn`                         | `Console.Error.Write` 和 `Console.Error.WriteLine` | 以“eprint”开头的函数写入标准错误。       |
| `fprintf` 和 `fprintfn`                         | `TextWriter.Write` 和 `TextWriter.WriteLine`       | 以“fprint”开头的函数会写入TextWriter。   |
| `sprintf`                                       | `String.Format`                                    | 以“sprint”开头的函数返回一个字符串。     |
| `bprintf`                                       | `StringBuilder.AppendFormat`                       | 以“bprint”开头的函数写入StringBuilder。  |
| `kprintf`, `kfprintf`, `ksprintf` 和 `kbprintf` | 无等效物                                           | 接受延续的函数。有关讨论，请参阅下一节。 |

除了 `bprintf` 和 `kXXX` 系列之外，所有这些都是自动可用的（通过 Microsoft.FSharp.Core.ExtraTopLevelOperators）。但是，如果您需要使用模块访问它们，它们位于 `Printf` 模块中。

这些的用法应该是显而易见的（`kXXX` 家族除外，下文将详细介绍）。

一种特别有用的技术是使用部分应用程序“烘焙” TextWriter 或 StringBuilder。

下面是一个使用 StringBuilder 的示例：

```F#
let printToSb s i =
    let sb = new System.Text.StringBuilder()

    // use partial application to fix the StringBuilder
    let myPrint format = Printf.bprintf sb format

    do myPrint "A string: %s. " s
    do myPrint "An int: %i" i

    //get the result
    sb.ToString()

// test
printToSb "hello" 42
```

以下是一个使用 TextWriter 的示例：

```F#
open System
open System.IO

let printToFile filename s i =
    let myDocsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
    let fullPath = Path.Combine(myDocsPath, filename)
    use sw = new StreamWriter(path=fullPath)

    // use partial application to fix the TextWriter
    let myPrint format = fprintf sw format

    do myPrint "A string: %s. " s
    do myPrint "An int: %i" i

    //get the result
    sw.Close()

// test
printToFile "myfile.txt" "hello" 42
```

### 更多关于部分应用 printf

请注意，在上述两种情况下，我们在创建部分应用程序时都必须传递一个格式参数。

也就是说，我们必须这样做：

```F#
let myPrint format = fprintf sw format
```

而不是无点版本：

```F#
let myPrint  = fprintf sw
```

这会阻止编译器抱怨类型不正确。原因并不明显。我们简要地提到了上面的 `TextWriterFormat` 作为 `printf` 的第一个参数。事实证明，`printf` 实际上并不是一个特定的函数，就像 `String.Format` 一样，而是一个泛型函数，必须用 TextWriterFormat（或类似的 StringFormat）参数化才能成为“实数”。

因此，为了安全起见，最好总是将 `printf` 与格式参数配对，而不是对部分应用程序过于激进。

## kprintf 函数

这四个 `kXXX` 函数与它们的表亲相似，除了它们接受一个额外的参数——一个延续。也就是说，在格式化完成后立即调用的函数。

下面是一个简单的片段：

```F#
let doAfter s =
    printfn "Done"
    // return the result
    s

let result = Printf.ksprintf doAfter "%s" "Hello"
```

你为什么想要这个？原因有很多：

- 您可以将结果传递给另一个执行有用操作的函数，例如日志框架
- 您可以执行诸如刷新 TextWriter 之类的操作
- 你可以发起一个活动

让我们来看一个使用外部日志框架和自定义事件的示例。

首先，让我们按照 log4net 或 System.Diagnostics.Trace 的思路创建一个简单的日志类。在实践中，这将被真正的第三方库所取代。

```F#
open System
open System.IO

// a logging library such as log4net
// or System.Diagnostics.Trace
type Logger(name) =

    let currentTime (tw:TextWriter) =
        tw.Write("{0:s}",DateTime.Now)

    let logEvent level msg =
        printfn "%t %s [%s] %s" currentTime level name msg

    member this.LogInfo msg =
        logEvent "INFO" msg

    member this.LogError msg =
        logEvent "ERROR" msg

    static member CreateLogger name =
        new Logger(name)
```

接下来，在我的应用程序代码中，我执行以下操作：

- 创建日志框架的实例。我在这里硬编码了工厂方法，但你也可以使用IoC容器。
- 创建调用日志框架的名为 `logInfo` 和 `logError` 的辅助函数，在 `logError` 的情况下，也显示弹出消息。

```F#
// my application code
module MyApplication =

    let logger = Logger.CreateLogger("MyApp")

    // create a logInfo using the Logger class
    let logInfo format =
        let doAfter s =
            logger.LogInfo(s)
        Printf.ksprintf doAfter format

    // create a logError using the Logger class
    let logError format =
        let doAfter s =
            logger.LogError(s)
            System.Windows.Forms.MessageBox.Show(s) |> ignore
        Printf.ksprintf doAfter format

    // function to exercise the logging
    let test() =
        do logInfo "Message #%i" 1
        do logInfo "Message #%i" 2
        do logError "Oops! an error occurred in my app"
```

最后，当我们运行 `test` 函数时，我们应该将消息写入控制台，并看到弹出消息：

```F#
MyApplication.test()
```

您还可以通过在日志库周围创建一个“FormattingLogger”包装类来创建面向对象版本的辅助方法，如下所示。

```F#
type FormattingLogger(name) =

    let logger = Logger.CreateLogger(name)

    // create a logInfo using the Logger class
    member this.logInfo format =
        let doAfter s =
            logger.LogInfo(s)
        Printf.ksprintf doAfter format

    // create a logError using the Logger class
    member this.logError format =
        let doAfter s =
            logger.LogError(s)
            System.Windows.Forms.MessageBox.Show(s) |> ignore
        Printf.ksprintf doAfter format

    static member createLogger name =
        new FormattingLogger(name)

// my application code
module MyApplication2 =

    let logger = FormattingLogger.createLogger("MyApp2")

    let test() =
        do logger.logInfo "Message #%i" 1
        do logger.logInfo "Message #%i" 2
        do logger.logError "Oops! an error occurred in app 2"

// test
MyApplication2.test()
```

面向对象的方法虽然更熟悉，但并不一定会更好！这里讨论了 OO 方法与纯函数的优缺点。

# 11 工作示例：解析命令行参数

*Part of the "Expressions and syntax" series (*[link](https://fsharpforfunandprofit.com/posts/pattern-matching-command-line/#series-toc)*)*

实践中的模式匹配
29六月2012 这篇文章是超过3年

https://fsharpforfunandprofit.com/posts/pattern-matching-command-line/

现在我们已经了解了 match 表达式的工作原理，让我们来看看实践中的一些示例。但首先，谈谈设计方法。

## F# 中的应用程序设计

我们已经看到泛型函数接收输入并发出输出。但从某种意义上说，这种方法适用于任何级别的功能代码，即使是顶层。

事实上，我们可以说一个函数式应用程序接收输入，对其进行转换，并发出输出：



现在理想情况下，转换在我们创建的纯类型安全世界中工作，以对域进行建模，但不幸的是，现实世界是无类型的！也就是说，输入可能是简单的字符串或字节，输出也是如此。

我们如何处理这个问题？显而易见的解决方案是有一个单独的阶段将输入转换为我们的纯内部模型，然后有一个独立的阶段将内部模型转换为输出。



通过这种方式，我们可以将现实世界的混乱隐藏在应用程序的核心之外。这种“保持模型纯粹”的方法类似于大型的“六边形架构”概念，或小型的 MVC 模式。

在这篇文章和下一篇文章中，我们将看到一些简单的例子。

## 示例：解析命令行

我们在上一篇文章中讨论了 match 表达式的一般情况，所以让我们来看一个它有用的真实示例，即解析命令行。

我们将设计和实现两个略有不同的版本，一个具有基本的内部模型，另一个具有一些改进。

### 要求

假设我们有三个命令行选项：“verbose”、“subdirectories”和“orderby”。“详细（Verbose）”和“子目录（subdirectories）”是标志，而“orderby”有两个选择：“按大小（by size）”和“按名称（by name）”。

所以命令行参数看起来像

```
MYAPP [/V] [/S] [/O order]
/V    verbose
/S    include subdirectories
/O    order by. Parameter is one of
        N - order by name.
        S - order by size
```

## 第一版

根据上述设计规则，我们可以看到：

- 输入将是一个字符串数组（或列表），每个参数对应一个字符串。
- 内部模型将是一组对（微小）领域进行建模的类型。
- 在这个例子中，输出超出了范围。

因此，我们将首先创建参数的内部模型，然后研究如何将输入解析为内部模型中使用的类型。

以下是对模型的第一次尝试：

```F#
// constants used later
let OrderByName = "N"
let OrderBySize = "S"

// set up a type to represent the options
type CommandLineOptions = {
    verbose: bool;
    subdirectories: bool;
    orderby: string;
    }
```

好的，看起来不错。现在让我们解析这些参数。

解析逻辑与上一篇文章中的 `loopAndSum` 示例非常相似。

- 我们在参数列表上创建一个递归循环。
- 每次通过循环，我们都会解析一个参数。
- 到目前为止解析的选项作为参数（“累加器”模式）传递到每个循环中。

```F#
let rec parseCommandLine args optionsSoFar =
    match args with
    // empty list means we're done.
    | [] ->
        optionsSoFar

    // match verbose flag
    | "/v"::xs ->
        let newOptionsSoFar = { optionsSoFar with verbose=true}
        parseCommandLine xs newOptionsSoFar

    // match subdirectories flag
    | "/s"::xs ->
        let newOptionsSoFar = { optionsSoFar with subdirectories=true}
        parseCommandLine xs newOptionsSoFar

    // match orderBy by flag
    | "/o"::xs ->
        //start a submatch on the next arg
        match xs with
        | "S"::xss ->
            let newOptionsSoFar = { optionsSoFar with orderby=OrderBySize}
            parseCommandLine xss newOptionsSoFar

        | "N"::xss ->
            let newOptionsSoFar = { optionsSoFar with orderby=OrderByName}
            parseCommandLine xss newOptionsSoFar

        // handle unrecognized option and keep looping
        | _ ->
            eprintfn "OrderBy needs a second argument"
            parseCommandLine xs optionsSoFar

    // handle unrecognized option and keep looping
    | x::xs ->
        eprintfn "Option '%s' is unrecognized" x
        parseCommandLine xs optionsSoFar
```

我希望这段代码很简单。

每个匹配都包含一个 `option::restOfList` 模式。如果选项匹配，则创建一个新的 `optionsSoFar` 值，循环将与剩余列表重复，直到列表变为空，此时我们可以退出循环并返回 `optionsSoFar` 值作为最终结果。

有两种特殊情况：

- 匹配“orderBy”选项会创建一个子匹配模式，该模式会查看列表其余部分中的第一个项目，如果找不到，则会报告缺少第二个参数。
- 主 `match..with` 的最后一个匹配不是通配符，而是“绑定到值”。就像通配符一样，这总是会成功的，但因为我们绑定了值，所以它允许我们打印出有问题的不匹配参数。
- 请注意，对于打印错误，我们使用 `eprintf` 而不是 `printf`。这将写入 STDERR 而不是 STDOUT。

现在让我们测试一下：

```F#
parseCommandLine ["/v"; "/s"]
```

哎呀！这不起作用——我们需要传递一个初始 `optionSoFar` 参数！让我们再试一次：

```F#
// define the defaults to pass in
let defaultOptions = {
    verbose = false;
    subdirectories = false;
    orderby = ByName
    }

// test it
parseCommandLine ["/v"] defaultOptions
parseCommandLine ["/v"; "/s"] defaultOptions
parseCommandLine ["/o"; "S"] defaultOptions
```

检查输出是否符合您的期望。

我们还应该检查错误情况：

```F#
parseCommandLine ["/v"; "xyz"] defaultOptions
parseCommandLine ["/o"; "xyz"] defaultOptions
```

现在，您应该看到这些情况下的错误消息。

在我们完成这个实现之前，让我们先解决一些烦人的问题。我们每次都会传递这些默认选项——我们能摆脱它们吗？

这是一种非常常见的情况：你有一个递归函数，它接受一个“累加器”参数，但你不想一直传递初始值。

答案很简单：只需创建另一个函数，使用默认值调用递归函数。

通常，第二个是“公共”的，递归的是隐藏的，所以我们将按如下方式重写代码：

- 将 `parseCommandLine` 重命名为 `parseCommandLineRec`。您还可以使用其他命名约定，例如带勾号的 `parseCommandLine'` 或 `innerParseCommandLine`。
- 创建一个新的 `parseCommandLine`，使用默认值调用 `parseCommandLineRec`

```F#
// create the "helper" recursive function
let rec parseCommandLineRec args optionsSoFar =
	// implementation as above

// create the "public" parse function
let parseCommandLine args =
    // create the defaults
    let defaultOptions = {
        verbose = false;
        subdirectories = false;
        orderby = OrderByName
        }

    // call the recursive one with the initial options
    parseCommandLineRec args defaultOptions
```

在这种情况下，辅助函数可以独立存在。但如果你真的想隐藏它，你可以把它作为嵌套子函数放在 `parseCommandLine` 本身的定义中。

```F#
// create the "public" parse function
let parseCommandLine args =
    // create the defaults
    let defaultOptions =
		// implementation as above

	// inner recursive function
	let rec parseCommandLineRec args optionsSoFar =
		// implementation as above

    // call the recursive one with the initial options
    parseCommandLineRec args defaultOptions
```

在这种情况下，我认为这只会让事情变得更加复杂，所以我把它们分开了。

所以，这是一次打包在一个模块中的所有代码：

```F#
module CommandLineV1 =

    // constants used later
    let OrderByName = "N"
    let OrderBySize = "S"

    // set up a type to represent the options
    type CommandLineOptions = {
        verbose: bool;
        subdirectories: bool;
        orderby: string;
        }

    // create the "helper" recursive function
    let rec parseCommandLineRec args optionsSoFar =
        match args with
        // empty list means we're done.
        | [] ->
            optionsSoFar

        // match verbose flag
        | "/v"::xs ->
            let newOptionsSoFar = { optionsSoFar with verbose=true}
            parseCommandLineRec xs newOptionsSoFar

        // match subdirectories flag
        | "/s"::xs ->
            let newOptionsSoFar = { optionsSoFar with subdirectories=true}
            parseCommandLineRec xs newOptionsSoFar

        // match orderBy by flag
        | "/o"::xs ->
            //start a submatch on the next arg
            match xs with
            | "S"::xss ->
                let newOptionsSoFar = { optionsSoFar with orderby=OrderBySize}
                parseCommandLineRec xss newOptionsSoFar

            | "N"::xss ->
                let newOptionsSoFar = { optionsSoFar with orderby=OrderByName}
                parseCommandLineRec xss newOptionsSoFar

            // handle unrecognized option and keep looping
            | _ ->
                eprintfn "OrderBy needs a second argument"
                parseCommandLineRec xs optionsSoFar

        // handle unrecognized option and keep looping
        | x::xs ->
            eprintfn "Option '%s' is unrecognized" x
            parseCommandLineRec xs optionsSoFar

    // create the "public" parse function
    let parseCommandLine args =
        // create the defaults
        let defaultOptions = {
            verbose = false;
            subdirectories = false;
            orderby = OrderByName
            }

        // call the recursive one with the initial options
        parseCommandLineRec args defaultOptions


// happy path
CommandLineV1.parseCommandLine ["/v"]
CommandLineV1.parseCommandLine  ["/v"; "/s"]
CommandLineV1.parseCommandLine  ["/o"; "S"]

// error handling
CommandLineV1.parseCommandLine ["/v"; "xyz"]
CommandLineV1.parseCommandLine ["/o"; "xyz"]
```

## 第二版

在我们的初始模型中，我们使用 bool 和 string 来表示可能的值。

```F#
type CommandLineOptions = {
    verbose: bool;
    subdirectories: bool;
    orderby: string;
    }
```

这有两个问题：

- **它并不真正代表域名**。例如，`orderby` 真的可以是任何字符串吗？如果我将其设置为“ABC”，我的代码会中断吗？
- **这些值不是自我记录的**。例如，verbose 值是一个 bool。我们只知道 bool 表示“verbose”选项，因为它所在的上下文（名为 `verbose` 的字段）。如果我们传递该 bool，并将其从上下文中删除，我们就不知道它表示什么。我敢肯定，我们都见过 C# 函数有很多这样的布尔参数：

```F#
myObject.SetUpComplicatedOptions(true,false,true,false,false);
```

因为 bool 不代表域级别的任何东西，所以很容易出错。

这两个问题的解决方案是在定义域时尽可能具体，通常是通过创建许多非常具体的类型。

这里有一个新版本的 `CommandLineOptions`：

```F#
type OrderByOption = OrderBySize | OrderByName
type SubdirectoryOption = IncludeSubdirectories | ExcludeSubdirectories
type VerboseOption = VerboseOutput | TerseOutput

type CommandLineOptions = {
    verbose: VerboseOption;
    subdirectories: SubdirectoryOption;
    orderby: OrderByOption
    }
```

有几件事需要注意：

- 任何地方都没有 bool 或 string。
- 名字很明确。当一个值被单独取值时，这就像文档一样，但也意味着该名称是唯一的，这有助于类型推理，从
- 而帮助您避免显式的类型注释。

一旦我们对域进行了更改，就很容易修复解析逻辑。

因此，以下是所有修订后的代码，包装在“v2”模块中：

```F#
module CommandLineV2 =

    type OrderByOption = OrderBySize | OrderByName
    type SubdirectoryOption = IncludeSubdirectories | ExcludeSubdirectories
    type VerboseOption = VerboseOutput | TerseOutput

    type CommandLineOptions = {
        verbose: VerboseOption;
        subdirectories: SubdirectoryOption;
        orderby: OrderByOption
        }

    // create the "helper" recursive function
    let rec parseCommandLineRec args optionsSoFar =
        match args with
        // empty list means we're done.
        | [] ->
            optionsSoFar

        // match verbose flag
        | "/v"::xs ->
            let newOptionsSoFar = { optionsSoFar with verbose=VerboseOutput}
            parseCommandLineRec xs newOptionsSoFar

        // match subdirectories flag
        | "/s"::xs ->
            let newOptionsSoFar = { optionsSoFar with subdirectories=IncludeSubdirectories}
            parseCommandLineRec xs newOptionsSoFar

        // match sort order flag
        | "/o"::xs ->
            //start a submatch on the next arg
            match xs with
            | "S"::xss ->
                let newOptionsSoFar = { optionsSoFar with orderby=OrderBySize}
                parseCommandLineRec xss newOptionsSoFar
            | "N"::xss ->
                let newOptionsSoFar = { optionsSoFar with orderby=OrderByName}
                parseCommandLineRec xss newOptionsSoFar
            // handle unrecognized option and keep looping
            | _ ->
                printfn "OrderBy needs a second argument"
                parseCommandLineRec xs optionsSoFar

        // handle unrecognized option and keep looping
        | x::xs ->
            printfn "Option '%s' is unrecognized" x
            parseCommandLineRec xs optionsSoFar

    // create the "public" parse function
    let parseCommandLine args =
        // create the defaults
        let defaultOptions = {
            verbose = TerseOutput;
            subdirectories = ExcludeSubdirectories;
            orderby = OrderByName
            }

        // call the recursive one with the initial options
        parseCommandLineRec args defaultOptions

// ==============================
// tests

// happy path
CommandLineV2.parseCommandLine ["/v"]
CommandLineV2.parseCommandLine ["/v"; "/s"]
CommandLineV2.parseCommandLine ["/o"; "S"]

// error handling
CommandLineV2.parseCommandLine ["/v"; "xyz"]
CommandLineV2.parseCommandLine ["/o"; "xyz"]
```

## 使用 fold 而不是递归？

我们在上一篇文章中说过，最好尽可能避免递归，并使用 `List` 模块中的内置函数，如 `map` 和 `fold`。

那么，我们可以在这里接受这个建议，并修改这段代码来做到这一点吗？

遗憾的是，这并不容易。问题是，列表函数通常一次处理一个元素，而“orderby”选项也需要一个“前瞻”参数。

为了使这与 `fold` 类似，我们需要创建一个“解析模式”标志来指示我们是否处于前瞻模式。这是可能的，但我认为与上述简单的递归版本相比，它只会增加额外的复杂性。

在现实世界中，任何比这更复杂的事情都是一个信号，你需要切换到一个合适的解析系统，比如 FParsec。

但是，为了向您展示它可以用 `fold` 完成：

```F#
module CommandLineV3 =

    type OrderByOption = OrderBySize | OrderByName
    type SubdirectoryOption = IncludeSubdirectories | ExcludeSubdirectories
    type VerboseOption = VerboseOutput | TerseOutput

    type CommandLineOptions = {
        verbose: VerboseOption;
        subdirectories: SubdirectoryOption;
        orderby: OrderByOption
        }

    type ParseMode = TopLevel | OrderBy

    type FoldState = {
        options: CommandLineOptions ;
        parseMode: ParseMode;
        }

    // parse the top-level arguments
    // return a new FoldState
    let parseTopLevel arg optionsSoFar =
        match arg with

        // match verbose flag
        | "/v" ->
            let newOptionsSoFar = {optionsSoFar with verbose=VerboseOutput}
            {options=newOptionsSoFar; parseMode=TopLevel}

        // match subdirectories flag
        | "/s"->
            let newOptionsSoFar = { optionsSoFar with subdirectories=IncludeSubdirectories}
            {options=newOptionsSoFar; parseMode=TopLevel}

        // match sort order flag
        | "/o" ->
            {options=optionsSoFar; parseMode=OrderBy}

        // handle unrecognized option and keep looping
        | x ->
            printfn "Option '%s' is unrecognized" x
            {options=optionsSoFar; parseMode=TopLevel}

    // parse the orderBy arguments
    // return a new FoldState
    let parseOrderBy arg optionsSoFar =
        match arg with
        | "S" ->
            let newOptionsSoFar = { optionsSoFar with orderby=OrderBySize}
            {options=newOptionsSoFar; parseMode=TopLevel}
        | "N" ->
            let newOptionsSoFar = { optionsSoFar with orderby=OrderByName}
            {options=newOptionsSoFar; parseMode=TopLevel}
        // handle unrecognized option and keep looping
        | _ ->
            printfn "OrderBy needs a second argument"
            {options=optionsSoFar; parseMode=TopLevel}

    // create a helper fold function
    let foldFunction state element  =
        match state with
        | {options=optionsSoFar; parseMode=TopLevel} ->
            // return new state
            parseTopLevel element optionsSoFar

        | {options=optionsSoFar; parseMode=OrderBy} ->
            // return new state
            parseOrderBy element optionsSoFar

    // create the "public" parse function
    let parseCommandLine args =

        let defaultOptions = {
            verbose = TerseOutput;
            subdirectories = ExcludeSubdirectories;
            orderby = OrderByName
            }

        let initialFoldState =
            {options=defaultOptions; parseMode=TopLevel}

        // call fold with the initial state
        args |> List.fold foldFunction initialFoldState

// ==============================
// tests

// happy path
CommandLineV3.parseCommandLine ["/v"]
CommandLineV3.parseCommandLine ["/v"; "/s"]
CommandLineV3.parseCommandLine ["/o"; "S"]

// error handling
CommandLineV3.parseCommandLine ["/v"; "xyz"]
CommandLineV3.parseCommandLine ["/o"; "xyz"]
```

顺便问一下，你能看到这个版本中行为的微妙变化吗？

在之前的版本中，如果“orderBy”选项没有参数，递归循环下次仍将解析它。但在“fold”版本中，这个词元被吞下并丢失了。

要了解这一点，请比较两种实现：

```F#
// verbose set
CommandLineV2.parseCommandLine ["/o"; "/v"]

// verbose not set!
CommandLineV3.parseCommandLine ["/o"; "/v"]
```

解决这个问题需要更多的工作。这再次表明，第二种实现是最容易调试和维护的。

## 摘要

在这篇文章中，我们看到了如何将模式匹配应用于现实世界的例子。

更重要的是，我们已经看到为最小的领域创建一个设计合理的内部模型是多么容易。而且，与使用 string 和 bool等基本类型相比，这种内部模型提供了更多的类型安全性和文档。

在下一个示例中，我们将进行更多的模式匹配！

# 12 工作示例：罗马数字

*Part of the "Expressions and syntax" series (*[link](https://fsharpforfunandprofit.com/posts/roman-numerals/#series-toc)*)*

实践中更多的模式匹配
30六月2012 这篇文章是超过3年

https://fsharpforfunandprofit.com/posts/roman-numerals/

上次我们看了解析命令行。这次我们将看另一个模式匹配示例，这次使用罗马数字。

如前所述，我们将尝试使用一个“纯”内部模型，该模型具有单独的阶段将输入转换为内部模型，然后再使用另一个单独的阶段从内部模型转换为输出。



## 要求

让我们从要求开始：

```
1） 接受一个字符串，如“MMMXCLXXIV”，并将其转换为整数。
转换为：I=1；V=5；X=10；L=50；C=100；D=500；M=1000；

如果较低的字母出现在较高的字母之前，则较高的字母的值会相应减小，因此
IV=4；IX＝9；XC=90；等等。

2） 作为额外的步骤，验证字母串，看看它是否是一个有效的数字。例如：“IIVVMM”是一个无效的罗马数字。

```

## 第一版

与之前一样，我们将首先创建内部模型，然后看看如何将输入解析到内部模型中。

这是对模型的第一次尝试。我们将把 `RomanDigital` 视为 `RomanDigits` 列表。

```F#
type RomanDigit = int
type RomanNumeral = RomanDigit list
```

不，站住！`RomanDigit` 不是任何数字，它必须从有限的集合中提取。

此外，`RomanNumeral` 不应该只是数字列表的类型别名。如果它也是自己的特殊类型，那就更好了。我们可以通过创建单个案例联合类型来实现这一点。

这里有一个更好的版本：

```F#
type RomanDigit = I | V | X | L | C | D | M
type RomanNumeral = RomanNumeral of RomanDigit list
```

### 输出：将数字转换为整数

现在让我们执行输出逻辑，将罗马数字转换为整数。

数字转换很容易：

```F#
/// Converts a single RomanDigit to an integer
let digitToInt =
    function
    | I -> 1
    | V -> 5
    | X -> 10
    | L -> 50
    | C -> 100
    | D -> 500
    | M -> 1000

// tests
I  |> digitToInt
V  |> digitToInt
M  |> digitToInt
```

请注意，我们使用的是 `function` 关键字而不是 `match..with` 表达式。

为了转换数字列表，我们将再次使用递归循环。有一种特殊情况，我们需要向前看下一个数字，如果它大于当前数字，那么就使用它们的差值。

```F#
let rec digitsToInt =
    function

    // empty is 0
    | [] -> 0

    // special case when a smaller comes before larger
    // convert both digits and add the difference to the sum
	// Example: "IV" and "CM"
    | smaller::larger::ns when smaller < larger ->
        (digitToInt larger - digitToInt smaller)  + digitsToInt ns

    // otherwise convert the digit and add to the sum
    | digit::ns ->
        digitToInt digit + digitsToInt ns

// tests
[I;I;I;I]  |> digitsToInt
[I;V]  |> digitsToInt
[V;I]  |> digitsToInt
[I;X]  |> digitsToInt
[M;C;M;L;X;X;I;X]  |> digitsToInt // 1979
[M;C;M;X;L;I;V] |> digitsToInt // 1944
```

请注意，不必定义“小于”操作。类型会自动按其声明顺序排序。

最后，我们可以通过将内容解包为列表并调用 `digitsToInt` 来转换 `RomanNumeral` 类型本身。

```F#
/// converts a RomanNumeral to an integer
let toInt (RomanNumeral digits) = digitsToInt digits

// test
let x = RomanNumeral [I;I;I;I]
x |> toInt

let x = RomanNumeral [M;C;M;L;X;X;I;X]
x |> toInt
```

这就解决了输出问题。

### 输入：将字符串转换为罗马数字

现在让我们执行输入逻辑，将字符串转换为我们的内部模型。

首先，让我们处理一个字符转换。这似乎很简单。

```F#
let charToRomanDigit =
    function
    | 'I' -> I
    | 'V' -> V
    | 'X' -> X
    | 'L' -> L
    | 'C' -> C
    | 'D' -> D
    | 'M' -> M
```

编译器不喜欢这样！如果我们得到其他角色会怎么样？

这是一个很好的例子，说明详尽的模式匹配如何迫使您思考缺失的需求。

那么，对于糟糕的输入应该怎么办。打印一条错误消息怎么样？

让我们再试一次，添加一个案例来处理所有其他字符：

```F#
let charToRomanDigit =
    function
    | 'I' -> I
    | 'V' -> V
    | 'X' -> X
    | 'L' -> L
    | 'C' -> C
    | 'D' -> D
    | 'M' -> M
	| ch -> eprintf "%c is not a valid character" ch
```

编译器也不喜欢这样！正常情况下返回有效的 `RomanDigit`，但错误情况下返回单位。正如我们在前面的文章中看到的，每个分支都必须返回相同的类型。

我们如何解决这个问题？我们可以抛出一个例外，但这似乎有点过分。如果我们再仔细想想，`charToRomanDigit` 不可能总是返回一个有效的 `RomanDigit`。有时可以，有时不能。换句话说，我们需要在这里使用类似于选项类型的东西。

但进一步考虑后，我们可能还希望调用者知道坏字符是什么。因此，我们需要在选项类型上创建自己的小变体来容纳这两种情况。

以下是修复版本：

```F#
type ParsedChar =
    | Digit of RomanDigit
    | BadChar of char

let charToRomanDigit =
    function
    | 'I' -> Digit I
    | 'V' -> Digit V
    | 'X' -> Digit X
    | 'L' -> Digit L
    | 'C' -> Digit C
    | 'D' -> Digit D
    | 'M' -> Digit M
    | ch -> BadChar ch
```

请注意，我已经删除了错误消息。由于返回了坏字符，调用者可以为 `BadChar` 情况打印自己的消息。

接下来，我们应该检查函数签名，以确保它是我们所期望的：

```F#
charToRomanDigit : char -> ParsedChar
```

看起来不错。

现在，我们如何将字符串转换为这些数字？我们将字符串转换为 char 数组，将其转换为列表，然后使用 `charToRomanDigit` 进行最终转换。

```F#
let toRomanDigitList s =
    s.ToCharArray() // error FS0072
    |> List.ofArray
    |> List.map charToRomanDigit
```

但编译器再次抱怨“FS0072:查找不确定类型的对象”，

当你使用方法而不是函数时，通常会发生这种情况。任何对象都可以实现 `.ToCharArray()`，因此类型推理无法判断类型的含义。

在这种情况下，解决方案就是在参数上使用显式类型注释——这是我们迄今为止的第一个！

```F#
let toRomanDigitList (s:string) =
    s.ToCharArray()
    |> List.ofArray
    |> List.map charToRomanDigit
```

但看看签名：

```F#
toRomanDigitList : string -> ParsedChar list
```

它仍然有讨厌的 `ParsedChar`，而不是 `RomanDigits`。我们希望如何进行？回答，让我们再次推卸责任，让别人来处理吧！

在这种情况下，“推卸责任（Passing the buck）”实际上是一个很好的设计原则。此函数不知道其客户端可能想做什么——有些客户端可能想忽略错误，而另一些客户端可能想快速失败。所以，只要把信息传回去，让他们决定。

在这种情况下，客户端是创建 `RomanNumeral` 类型的顶级函数。这是我们的第一次尝试：

```F#
// convert a string to a RomanNumeral
let toRomanNumeral s =
    toRomanDigitList s
    |> RomanNumeral
```

编译器并不满意—— `RomanNumeral` 构造函数需要一个 `RomanDigits` 列表，但 `toRomanDigitList` 却给了我们一个 `ParsedChars` 列表。

现在，我们终于必须承诺一个错误处理策略。让我们选择忽略坏字符，但在出现错误时打印出来。为此，我们将使用 `List.choose` 函数。它类似于 `List.map`，但还内置了一个过滤器。返回有效的元素（`Some something`），但过滤掉 `None` 的元素。

因此，我们的 choose 函数应该执行以下操作：

- 对于有效数字，返回 `Some digit`
- 对于无效的 `BadChars`，打印错误消息并返回 `None`。

如果我们这样做，`List.choose` 的输出将是一个 `RomanDigits` 列表，与 `RomanNumeral` 构造函数的输入完全一样。

以下是所有内容的组合：

```F#
/// Convert a string to a RomanNumeral
/// Does not validate the input.E.g. "IVIV" would be valid
let toRomanNumeral s =
    toRomanDigitList s
    |> List.choose (
        function
        | Digit digit ->
            Some digit
        | BadChar ch ->
            eprintfn "%c is not a valid character" ch
            None
        )
    |> RomanNumeral
```

让我们测试一下！

```F#
// test good cases

"IIII"  |> toRomanNumeral
"IV"  |> toRomanNumeral
"VI"  |> toRomanNumeral
"IX"  |> toRomanNumeral
"MCMLXXIX"  |> toRomanNumeral
"MCMXLIV" |> toRomanNumeral
"" |> toRomanNumeral

// error cases
"MC?I" |> toRomanNumeral
"abc" |> toRomanNumeral
```

好的，到目前为止一切都很好。让我们继续进行验证。

### 验证规则

需求中没有列出验证规则，所以让我们根据对罗马数字的了解来做出最好的猜测：

- 不允许连续五位数字
- 某些数字最多允许为 4。它们是I、X、C 和 M。其他（V、L、D）只能单独出现。
- 一些较低的数字可以出现在较高的数字之前，但前提是它们单独出现。例如，“IX”可以，但“IIIX”不行。
- 但这仅适用于成对的数字。一行中的三个升序数字无效。例如，“IX”可以，但“IXC”不行。
- 始终允许使用没有游程的个位数

我们可以将这些要求转换为模式匹配函数，如下所示：

```F#
let runsAllowed =
    function
    | I | X | C | M -> true
    | V | L | D -> false

let noRunsAllowed  = runsAllowed >> not

// check for validity
let rec isValidDigitList digitList =
    match digitList with

    // empty list is valid
    | [] -> true

    // A run of 5 or more anything is invalid
    // Example:  XXXXX
    | d1::d2::d3::d4::d5::_
        when d1=d2 && d1=d3 && d1=d4 && d1=d5 ->
            false

    // 2 or more non-runnable digits is invalid
    // Example:  VV
    | d1::d2::_
        when d1=d2 && noRunsAllowed d1 ->
            false

    // runs of 2,3,4 in the middle are invalid if next digit is higher
    // Example:  IIIX
    | d1::d2::d3::d4::higher::ds
        when d1=d2 && d1=d3 && d1=d4
        && runsAllowed d1 // not really needed because of the order of matching
        && higher > d1 ->
            false

    | d1::d2::d3::higher::ds
        when d1=d2 && d1=d3
        && runsAllowed d1
        && higher > d1 ->
            false

    | d1::d2::higher::ds
        when d1=d2
        && runsAllowed d1
        && higher > d1 ->
            false

    // three ascending numbers in a row is invalid
    // Example:  IVX
    | d1::d2::d3::_  when d1<d2 && d2<= d3 ->
        false

    // A single digit with no runs is always allowed
    | _::ds ->
        // check the remainder of the list
        isValidDigitList ds
```

同样，请注意，“相等”和“小于”不需要定义。

让我们测试一下验证：

```F#
// test valid
let validList = [
    [I;I;I;I]
    [I;V]
    [I;X]
    [I;X;V]
    [V;X]
    [X;I;V]
    [X;I;X]
    [X;X;I;I]
    ]

let testValid = validList |> List.map isValidDigitList

let invalidList = [
    // Five in a row of any digit is not allowed
    [I;I;I;I;I]
    // Two in a row for V,L, D is not allowed
    [V;V]
    [L;L]
    [D;D]
    // runs of 2,3,4 in the middle are invalid if next digit is higher
    [I;I;V]
    [X;X;X;M]
    [C;C;C;C;D]
    // three ascending numbers in a row is invalid
    [I;V;X]
    [X;L;D]
    ]
let testInvalid = invalidList |> List.map isValidDigitList
```

最后，我们添加了一个顶层函数来测试 `RomanNumeral` 类型本身的有效性。

```F#
// top level check for validity
let isValid (RomanNumeral digitList) =
    isValidDigitList digitList


// test good cases
"IIII"  |> toRomanNumeral |> isValid
"IV"  |> toRomanNumeral |> isValid
"" |> toRomanNumeral |> isValid

// error cases
"IIXX" |> toRomanNumeral |> isValid
"VV" |> toRomanNumeral |> isValid

// grand finale
[ "IIII"; "XIV"; "MMDXC";
"IIXX"; "VV"; ]
|> List.map toRomanNumeral
|> List.iter (function
    | n when isValid n ->
        printfn "%A is valid and its integer value is %i" n (toInt n)
    | n ->
        printfn "%A is not valid" n
    )
```

## 第一版的完整代码

以下是一个模块中的所有代码：

```F#
module RomanNumeralsV1 =

    // ==========================================
    // Types
    // ==========================================

    type RomanDigit = I | V | X | L | C | D | M
    type RomanNumeral = RomanNumeral of RomanDigit list

    // ==========================================
    // Output logic
    // ==========================================

    /// Converts a single RomanDigit to an integer
    let digitToInt =
        function
        | I -> 1
        | V -> 5
        | X -> 10
        | L -> 50
        | C -> 100
        | D -> 500
        | M -> 1000

    /// converts a list of digits to an integer
    let rec digitsToInt =
        function

        // empty is 0
        | [] -> 0

        // special case when a smaller comes before larger
        // convert both digits and add the difference to the sum
        // Example: "IV" and "CM"
        | smaller::larger::ns when smaller < larger ->
            (digitToInt larger - digitToInt smaller)  + digitsToInt ns

        // otherwise convert the digit and add to the sum
        | digit::ns ->
            digitToInt digit + digitsToInt ns

    /// converts a RomanNumeral to an integer
    let toInt (RomanNumeral digits) = digitsToInt digits

    // ==========================================
    // Input logic
    // ==========================================

    type ParsedChar =
        | Digit of RomanDigit
        | BadChar of char

    let charToRomanDigit =
        function
        | 'I' -> Digit I
        | 'V' -> Digit V
        | 'X' -> Digit X
        | 'L' -> Digit L
        | 'C' -> Digit C
        | 'D' -> Digit D
        | 'M' -> Digit M
        | ch -> BadChar ch

    let toRomanDigitList (s:string) =
        s.ToCharArray()
        |> List.ofArray
        |> List.map charToRomanDigit

    /// Convert a string to a RomanNumeral
    /// Does not validate the input.E.g. "IVIV" would be valid
    let toRomanNumeral s =
        toRomanDigitList s
        |> List.choose (
            function
            | Digit digit ->
                Some digit
            | BadChar ch ->
                eprintfn "%c is not a valid character" ch
                None
            )
        |> RomanNumeral

    // ==========================================
    // Validation logic
    // ==========================================

    let runsAllowed =
        function
        | I | X | C | M -> true
        | V | L | D -> false

    let noRunsAllowed  = runsAllowed >> not

    // check for validity
    let rec isValidDigitList digitList =
        match digitList with

        // empty list is valid
        | [] -> true

        // A run of 5 or more anything is invalid
        // Example:  XXXXX
        | d1::d2::d3::d4::d5::_
            when d1=d2 && d1=d3 && d1=d4 && d1=d5 ->
                false

        // 2 or more non-runnable digits is invalid
        // Example:  VV
        | d1::d2::_
            when d1=d2 && noRunsAllowed d1 ->
                false

        // runs of 2,3,4 in the middle are invalid if next digit is higher
        // Example:  IIIX
        | d1::d2::d3::d4::higher::ds
            when d1=d2 && d1=d3 && d1=d4
            && runsAllowed d1 // not really needed because of the order of matching
            && higher > d1 ->
                false

        | d1::d2::d3::higher::ds
            when d1=d2 && d1=d3
            && runsAllowed d1
            && higher > d1 ->
                false

        | d1::d2::higher::ds
            when d1=d2
            && runsAllowed d1
            && higher > d1 ->
                false

        // three ascending numbers in a row is invalid
        // Example:  IVX
        | d1::d2::d3::_  when d1<d2 && d2<= d3 ->
            false

        // A single digit with no runs is always allowed
        | _::ds ->
            // check the remainder of the list
            isValidDigitList ds

    // top level check for validity
    let isValid (RomanNumeral digitList) =
        isValidDigitList digitList
```

## 第二版

代码可以工作，但有一点让我很困扰。验证逻辑似乎非常复杂。难道罗马人不必考虑这一切吗？

此外，我还可以想到一些验证失败但通过的例子，比如“VIV”：

```F#
"VIV" |> toRomanNumeral |> isValid
```

我们可以尝试收紧我们的验证规则，但让我们尝试另一种策略。复杂的逻辑往往表明你没有完全正确地理解这个领域。

换句话说，我们能否改变内部模型，使一切变得更简单？

如果我们不再试图将字母映射到数字，而是创建一个映射罗马人思维方式的域，那会怎么样？在这个模型中，“I”、“II”、“III”、“IV”等等都是一个单独的数字。

让我们试试看会发生什么。

这是该域的新类型。我现在为每个可能的数字都有一个数字类型。`RomanNumeral` 类型保持不变。

```F#
type RomanDigit =
    | I | II | III | IIII
    | IV | V
    | IX | X | XX | XXX | XXXX
    | XL | L
    | XC | C | CC | CCC | CCCC
    | CD | D
    | CM | M | MM | MMM | MMMM
type RomanNumeral = RomanNumeral of RomanDigit list
```

### 输出：第二版

接下来，将单个 `RomanDigit` 转换为整数与之前相同，但情况更多：

```F#
/// Converts a single RomanDigit to an integer
let digitToInt =
    function
    | I -> 1 | II -> 2 | III -> 3 | IIII -> 4
    | IV -> 4 | V -> 5
    | IX -> 9 | X -> 10 | XX -> 20 | XXX -> 30 | XXXX -> 40
    | XL -> 40 | L -> 50
    | XC -> 90 | C -> 100 | CC -> 200 | CCC -> 300 | CCCC -> 400
    | CD -> 400 | D -> 500
    | CM -> 900 | M -> 1000 | MM -> 2000 | MMM -> 3000 | MMMM -> 4000

// tests
I  |> digitToInt
III  |> digitToInt
V  |> digitToInt
CM  |> digitToInt
```

计算数字之和现在很简单。无需特殊情况：

```F#
/// converts a list of digits to an integer
let digitsToInt list =
    list |> List.sumBy digitToInt

// tests
[IIII]  |> digitsToInt
[IV]  |> digitsToInt
[V;I]  |> digitsToInt
[IX]  |> digitsToInt
[M;CM;L;X;X;IX]  |> digitsToInt // 1979
[M;CM;XL;IV] |> digitsToInt // 1944
```

最后，顶层函数是相同的：

```F#
/// converts a RomanNumeral to an integer
let toInt (RomanNumeral digits) = digitsToInt digits

// test
let x = RomanNumeral [M;CM;LX;X;IX]
x |> toInt
```

### 输入：第二版

对于输入解析，我们将保留 `ParsedChar` 类型。但这次，我们必须一次匹配 1、2、3 或 4个字符。这意味着我们不能像第一个版本那样只扮演一个角色——我们必须在主循环中进行匹配。这意味着循环现在必须是递归的。

此外，我们希望将 IIII 转换为单个 `IIII` 数字，而不是4个单独的 `I` 数字，因此我们将最长的匹配放在前面。

```F#
type ParsedChar =
    | Digit of RomanDigit
    | BadChar of char

let rec toRomanDigitListRec charList =
    match charList with
    // match the longest patterns first

    // 4 letter matches
    | 'I'::'I'::'I'::'I'::ns ->
        Digit IIII :: (toRomanDigitListRec ns)
    | 'X'::'X'::'X'::'X'::ns ->
        Digit XXXX :: (toRomanDigitListRec ns)
    | 'C'::'C'::'C'::'C'::ns ->
        Digit CCCC :: (toRomanDigitListRec ns)
    | 'M'::'M'::'M'::'M'::ns ->
        Digit MMMM :: (toRomanDigitListRec ns)

    // 3 letter matches
    | 'I'::'I'::'I'::ns ->
        Digit III :: (toRomanDigitListRec ns)
    | 'X'::'X'::'X'::ns ->
        Digit XXX :: (toRomanDigitListRec ns)
    | 'C'::'C'::'C'::ns ->
        Digit CCC :: (toRomanDigitListRec ns)
    | 'M'::'M'::'M'::ns ->
        Digit MMM :: (toRomanDigitListRec ns)

    // 2 letter matches
    | 'I'::'I'::ns ->
        Digit II :: (toRomanDigitListRec ns)
    | 'X'::'X'::ns ->
        Digit XX :: (toRomanDigitListRec ns)
    | 'C'::'C'::ns ->
        Digit CC :: (toRomanDigitListRec ns)
    | 'M'::'M'::ns ->
        Digit MM :: (toRomanDigitListRec ns)

    | 'I'::'V'::ns ->
        Digit IV :: (toRomanDigitListRec ns)
    | 'I'::'X'::ns ->
        Digit IX :: (toRomanDigitListRec ns)
    | 'X'::'L'::ns ->
        Digit XL :: (toRomanDigitListRec ns)
    | 'X'::'C'::ns ->
        Digit XC :: (toRomanDigitListRec ns)
    | 'C'::'D'::ns ->
        Digit CD :: (toRomanDigitListRec ns)
    | 'C'::'M'::ns ->
        Digit CM :: (toRomanDigitListRec ns)

    // 1 letter matches
    | 'I'::ns ->
        Digit I :: (toRomanDigitListRec ns)
    | 'V'::ns ->
        Digit V :: (toRomanDigitListRec ns)
    | 'X'::ns ->
        Digit X :: (toRomanDigitListRec ns)
    | 'L'::ns ->
        Digit L :: (toRomanDigitListRec ns)
    | 'C'::ns ->
        Digit C :: (toRomanDigitListRec ns)
    | 'D'::ns ->
        Digit D :: (toRomanDigitListRec ns)
    | 'M'::ns ->
        Digit M :: (toRomanDigitListRec ns)

    // bad letter matches
    | badChar::ns ->
        BadChar badChar :: (toRomanDigitListRec ns)

    // 0 letter matches
    | [] ->
        []

```

嗯，这比第一个版本长得多，但其他方面基本相同。

顶层功能不变。

```F#
let toRomanDigitList (s:string) =
    s.ToCharArray()
    |> List.ofArray
    |> toRomanDigitListRec

/// Convert a string to a RomanNumeral
let toRomanNumeral s =
    toRomanDigitList s
    |> List.choose (
        function
        | Digit digit ->
            Some digit
        | BadChar ch ->
            eprintfn "%c is not a valid character" ch
            None
        )
    |> RomanNumeral

// test good cases
"IIII"  |> toRomanNumeral
"IV"  |> toRomanNumeral
"VI"  |> toRomanNumeral
"IX"  |> toRomanNumeral
"MCMLXXIX"  |> toRomanNumeral
"MCMXLIV" |> toRomanNumeral
"" |> toRomanNumeral

// error cases
"MC?I" |> toRomanNumeral
"abc" |> toRomanNumeral
```

### 验证：第二版

最后，让我们看看新的域模型如何影响验证规则。现在，规则要简单得多。事实上，只有一个。

- 每个数字必须小于前一个数字

```F#
// check for validity
let rec isValidDigitList digitList =
    match digitList with

    // empty list is valid
    | [] -> true

    // a following digit that is equal or larger is an error
    | d1::d2::_
        when d1 <= d2  ->
            false

    // A single digit is always allowed
    | _::ds ->
        // check the remainder of the list
        isValidDigitList ds

// top level check for validity
let isValid (RomanNumeral digitList) =
    isValidDigitList digitList

// test good cases
"IIII"  |> toRomanNumeral |> isValid
"IV"  |> toRomanNumeral |> isValid
"" |> toRomanNumeral |> isValid

// error cases
"IIXX" |> toRomanNumeral |> isValid
"VV" |> toRomanNumeral |> isValid

```

唉，尽管如此，我们仍然没有解决引发重写的坏情况！

```F#
"VIV" |> toRomanNumeral |> isValid
```

对此有一个不太复杂的解决方案，但我认为现在是时候别管它了！

## 第二版的完整代码

以下是第二个版本的一个模块中的所有代码：

```F#
module RomanNumeralsV2 =

    // ==========================================
    // Types
    // ==========================================

    type RomanDigit =
        | I | II | III | IIII
        | IV | V
        | IX | X | XX | XXX | XXXX
        | XL | L
        | XC | C | CC | CCC | CCCC
        | CD | D
        | CM | M | MM | MMM | MMMM
    type RomanNumeral = RomanNumeral of RomanDigit list

    // ==========================================
    // Output logic
    // ==========================================

    /// Converts a single RomanDigit to an integer
    let digitToInt =
        function
        | I -> 1 | II -> 2 | III -> 3 | IIII -> 4
        | IV -> 4 | V -> 5
        | IX -> 9 | X -> 10 | XX -> 20 | XXX -> 30 | XXXX -> 40
        | XL -> 40 | L -> 50
        | XC -> 90 | C -> 100 | CC -> 200 | CCC -> 300 | CCCC -> 400
        | CD -> 400 | D -> 500
        | CM -> 900 | M -> 1000 | MM -> 2000 | MMM -> 3000 | MMMM -> 4000

    /// converts a RomanNumeral to an integer
    let toInt (RomanNumeral digits) = digitsToInt digits

    // ==========================================
    // Input logic
    // ==========================================

    type ParsedChar =
        | Digit of RomanDigit
        | BadChar of char

    let rec toRomanDigitListRec charList =
        match charList with
        // match the longest patterns first

        // 4 letter matches
        | 'I'::'I'::'I'::'I'::ns ->
            Digit IIII :: (toRomanDigitListRec ns)
        | 'X'::'X'::'X'::'X'::ns ->
            Digit XXXX :: (toRomanDigitListRec ns)
        | 'C'::'C'::'C'::'C'::ns ->
            Digit CCCC :: (toRomanDigitListRec ns)
        | 'M'::'M'::'M'::'M'::ns ->
            Digit MMMM :: (toRomanDigitListRec ns)

        // 3 letter matches
        | 'I'::'I'::'I'::ns ->
            Digit III :: (toRomanDigitListRec ns)
        | 'X'::'X'::'X'::ns ->
            Digit XXX :: (toRomanDigitListRec ns)
        | 'C'::'C'::'C'::ns ->
            Digit CCC :: (toRomanDigitListRec ns)
        | 'M'::'M'::'M'::ns ->
            Digit MMM :: (toRomanDigitListRec ns)

        // 2 letter matches
        | 'I'::'I'::ns ->
            Digit II :: (toRomanDigitListRec ns)
        | 'X'::'X'::ns ->
            Digit XX :: (toRomanDigitListRec ns)
        | 'C'::'C'::ns ->
            Digit CC :: (toRomanDigitListRec ns)
        | 'M'::'M'::ns ->
            Digit MM :: (toRomanDigitListRec ns)

        | 'I'::'V'::ns ->
            Digit IV :: (toRomanDigitListRec ns)
        | 'I'::'X'::ns ->
            Digit IX :: (toRomanDigitListRec ns)
        | 'X'::'L'::ns ->
            Digit XL :: (toRomanDigitListRec ns)
        | 'X'::'C'::ns ->
            Digit XC :: (toRomanDigitListRec ns)
        | 'C'::'D'::ns ->
            Digit CD :: (toRomanDigitListRec ns)
        | 'C'::'M'::ns ->
            Digit CM :: (toRomanDigitListRec ns)

        // 1 letter matches
        | 'I'::ns ->
            Digit I :: (toRomanDigitListRec ns)
        | 'V'::ns ->
            Digit V :: (toRomanDigitListRec ns)
        | 'X'::ns ->
            Digit X :: (toRomanDigitListRec ns)
        | 'L'::ns ->
            Digit L :: (toRomanDigitListRec ns)
        | 'C'::ns ->
            Digit C :: (toRomanDigitListRec ns)
        | 'D'::ns ->
            Digit D :: (toRomanDigitListRec ns)
        | 'M'::ns ->
            Digit M :: (toRomanDigitListRec ns)

        // bad letter matches
        | badChar::ns ->
            BadChar badChar :: (toRomanDigitListRec ns)

        // 0 letter matches
        | [] ->
            []

    let toRomanDigitList (s:string) =
        s.ToCharArray()
        |> List.ofArray
        |> toRomanDigitListRec

    /// Convert a string to a RomanNumeral
    /// Does not validate the input.E.g. "IVIV" would be valid
    let toRomanNumeral s =
        toRomanDigitList s
        |> List.choose (
            function
            | Digit digit ->
                Some digit
            | BadChar ch ->
                eprintfn "%c is not a valid character" ch
                None
            )
        |> RomanNumeral

    // ==========================================
    // Validation logic
    // ==========================================

    // check for validity
    let rec isValidDigitList digitList =
        match digitList with

        // empty list is valid
        | [] -> true

        // a following digit that is equal or larger is an error
        | d1::d2::_
            when d1 <= d2  ->
                false

        // A single digit is always allowed
        | _::ds ->
            // check the remainder of the list
            isValidDigitList ds

    // top level check for validity
    let isValid (RomanNumeral digitList) =
        isValidDigitList digitList
```

## 比较两个版本

你更喜欢哪个版本？第二种更冗长，因为它有更多的案例，但另一方面，实际逻辑在所有领域都是相同的或更简单的，没有特殊情况。因此，两个版本的代码行总数大致相同。

总的来说，由于缺乏特殊情况，我更喜欢第二种实施方式。

作为一个有趣的实验，试着用 C# 或你最喜欢的命令式语言编写相同的代码！

## 使其面向对象

最后，让我们看看如何使这个面向对象。我们不关心辅助函数，所以我们可能只需要三个方法：

- 静态构造函数
- 一种转换为 int 的方法
- 一种转换为字符串的方法

它们在这里：

```F#
type RomanNumeral with

    static member FromString s =
        toRomanNumeral s

    member this.ToInt() =
        toInt this

    override this.ToString() =
        sprintf "%A" this
```

注意：您可以忽略编译器关于已弃用覆盖的警告。

现在让我们以面向对象的方式使用它：

```F#
let r = RomanNumeral.FromString "XXIV"
let s = r.ToString()
let i = r.ToInt()
```

## 摘要

在这篇文章中，我们看到了很多很多的模式匹配！

但是，与上一篇文章一样，同样重要的是，我们已经看到为非常琐碎的领域创建一个设计合理的内部模型是多么容易。同样，我们的内部模型没有使用原始类型——没有理由不创建大量的小类型来更好地表示域。例如，`ParsedChar` 类型——你会费心在C#中创建它吗？

应该清楚的是，内部模型的选择会对设计的复杂性产生很大的影响。但是，如果我们进行重构，编译器几乎总是会警告我们是否忘记了什么。