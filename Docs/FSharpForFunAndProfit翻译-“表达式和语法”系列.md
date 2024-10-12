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