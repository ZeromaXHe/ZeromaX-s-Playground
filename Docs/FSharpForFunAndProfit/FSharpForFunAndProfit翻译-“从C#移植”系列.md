# [返回主 Markdown](./FSharpForFunAndProfit翻译.md)



# 1 从C#到F#的移植：简介

*Part of the "Porting from C#" series (*[link](https://fsharpforfunandprofit.com/posts/porting-to-csharp-intro/#series-toc)*)*

将现有 C# 代码移植到 F# 的三种方法
01八月2012这篇文章是超过3年

https://fsharpforfunandprofit.com/posts/porting-to-csharp-intro/

*注意：在阅读本系列之前，我建议您先阅读以下系列：“函数式思维”、“表达式和语法”以及“理解 F# 类型”。*

对于许多开发人员来说，学习一门新语言后的下一步可能是将一些现有代码移植到它上面，这样他们就可以很好地感受到两种语言之间的差异。

正如我们之前指出的，函数式语言与命令式语言非常不同，因此试图将命令式代码直接移植到函数式语言通常是不可能的，即使粗略的移植成功了，移植的代码也可能无法充分利用函数式模型。

当然，F# 是一种多范式语言，包括对面向对象和命令式技术的支持，但即便如此，直接移植通常不是编写相应 F# 代码的最佳方式。

因此，在本系列中，我们将探讨将现有 C# 代码移植到 F# 的各种方法。

## 移植的复杂程度

如果你回想起之前一篇文章中的图表，有四个关键概念可以区分 F# 和 C#。

- 面向函数而非面向对象
- 表达式而非语句
- 用于创建域模型的代数类型
- 控制流的模式匹配

【四个关键概念】

而且，正如那篇文章及其后续文章所解释的那样，这些方面不仅是学术性的，而且为开发人员提供了具体的好处。

因此，我将移植过程分为三个复杂程度（因为没有更好的术语），它们代表了移植代码利用这些优势的程度。

### 基础级别：直接移植

在第一层，F# 代码是 C# 代码的直接移植（如果可能的话）。使用类和方法代替模块和函数，并且值经常发生变化。

### 中级：函数式代码

在下一个层次上，F# 代码已经被重构为功能齐全。

- 类和方法已被模块和函数所取代，值通常是不可变的。
- 高阶函数用于替换接口和继承。
- 模式匹配广泛用于控制流。
- 循环已被“map”或递归等列表函数所取代。

有两条不同的路径可以让你达到这个水平。

- 第一种方法是对 F# 进行基本的直接移植，然后重构 F# 代码。
- 第二种方法是在保持 C# 的同时将现有的命令式代码转换为函数式代码，然后才将函数式 C# 代码移植到函数式 F# 代码！

第二种选择可能看起来很笨拙，但对于真正的代码来说，它可能会更快、更舒适。更快，因为您可以使用 Resharper 等工具进行重构，更舒适，因为您在 C# 中工作到最终端口。这种方法还清楚地表明，困难的部分不是从 C# 到 F# 的实际端口，而是将命令式代码转换为函数式代码！

### 高级级别：类型代表领域

在这个最终级别，不仅代码功能正常，而且设计本身也发生了变化，以利用代数数据类型（尤其是联合类型）的强大功能。

域将被编码为类型，使得非法状态甚至无法表示，并且在编译时强制执行正确性。要具体演示这种方法的强大功能，请参阅“为什么使用 F#”系列和整个“用类型设计”系列中的购物车示例。

这个级别只能在 F# 中完成，在 C# 中并不实用。

### 移植图

这是一个图表，可以帮助您可视化上述各种移植路径。

【四个关键概念】

## 本系列的方法

为了了解这三个级别在实践中是如何工作的，我们将把它们应用于一些工作示例：

- 第一个例子是一个简单的系统，用于创建和评分一个十针保龄球游戏，基于“叔叔”鲍勃·马丁描述的著名“保龄球游戏卡塔”的代码。最初的 C# 代码只有一个类和大约 70 行代码，但即便如此，它也展示了许多重要的原则。
- 接下来，我们将基于此示例查看一些购物车代码。
- 最后一个例子是表示地铁旋转栅门系统状态的代码，也是基于 Bob Martin 的一个例子。此示例演示了 F# 中的联合类型如何比 OO 方法更容易表示状态转换模型。

但首先，在我们开始详细的示例之前，我们将回到基础，对一些代码片段进行一些简单的移植。这将是下一篇文章的主题。

# 2 直接移植入门

*Part of the "Porting from C#" series (*[link](https://fsharpforfunandprofit.com/posts/porting-to-csharp-getting-started/#series-toc)*)*

F# 相当于 C#
02八月2012 这篇文章是超过3年

https://fsharpforfunandprofit.com/posts/porting-to-csharp-getting-started/

在开始详细的示例之前，我们将回到基础知识，对琐碎的示例进行一些简单的移植。

在这篇文章和下一篇文章中，我们将研究与常见 C# 语句和关键字最接近的 F# 等价物，以指导您进行直接移植。

## 基本语法转换指南

在启动移植之前，您需要了解 F# 语法与 C# 语法的不同之处。本节介绍了从一个转换到另一个的一些一般准则。（有关 F# 语法的整体快速概述，请参阅“60 秒内的 F# 语法”）

### 花括号和缩进

C# 使用花括号来表示代码块的开始和结束。F# 通常只使用缩进。

花括号用于 F#，但不用于代码块。相反，您将看到它们被使用：

- 关于“记录”类型的定义和用法。
- 结合计算表达式，如 seq 和 async。一般来说，您无论如何都不会将这些表达式用于基本端口。

有关缩进规则的详细信息，请参阅这篇文章。

### 分号

与 C# 的分号不同，F# 不需要任何类型的行或语句终止符。

### 逗号

F# 不使用逗号分隔参数或列表元素，所以移植时记住不要使用逗号！

*对于分隔列表元素，请使用分号而不是逗号。*

```F#
// C# example
var list = new int[] { 1,2,3}
// F# example
let list = [1;2;3] // semicolons
```

*要分隔本机 F# 函数的参数，请使用空格。*

```F#
// C# example
int myFunc(int x, int y, int z) { ... function body ...}
// F# example
let myFunc (x:int) (y:int) (z:int) :int = ... function body ...
let myFunc x y z = ... function body ...
```

逗号通常仅用于元组，或在调用 .NET 库函数时用于分隔参数。（有关元组与多个参数的更多信息，请参阅这篇文章）

### 定义变量、函数和类型

在 F# 中，变量和函数的定义都使用以下形式：

```F#
let someName = // the definition
```

所有类型（类、结构、接口等）的定义都使用以下形式：

```F#
type someName = // the definition
```

`=` 符号的使用是 F# 和 C# 之间的一个重要区别。C# 使用花括号，F# 使用 `=`，然后必须缩进以下代码块。

### 可变值

在 F# 中，默认情况下值是不可变的。如果你正在做一个直接命令式端口，你可能需要使用 `mutable` 关键字使一些值可变。然后，要为这些值赋值，请使用 `<-` 运算符，而不是等号。

```F#
// C# example
var variableName = 42
variableName = variableName + 1
// F# example
let mutable variableName = 42
variableName <- variableName + 1
```

### 分配与平等测试

在 C# 中，等号用于赋值，双等号 `==` 用于测试等式。

然而，在 F# 中，等号用于测试相等性，并且在声明时也用于最初将值绑定到其他值，

```F#
let mutable variableName = 42     // Bound to 42 on declaration
variableName <- variableName + 1  // Mutated (reassigned)
variableName = variableName + 1   // Comparison not assignment!
```

要测试不等式，请使用SQL样式 `<>` 而不是 `!=`

```F#
let variableName = 42             // Bound to 42 on declaration
variableName <> 43                // Comparison will return true.
variableName != 43                // Error FS0020.
```

如果你不小心使用 `!=` 您可能会收到错误FS0020。

## 转换示例 #1

有了这些基本准则，让我们看看一些真实的代码示例，并为它们做一个直接移植。

第一个例子有一些非常简单的代码，我们将逐行移植。这是 C# 代码。

```c#
using System;
using System.Collections.Generic;

namespace PortingToFsharp
{
    public class Squarer
    {
        public int Square(int input)
        {
            var result = input * input;
            return result;
        }

        public void PrintSquare(int input)
        {
            var result = this.Square(input);
            Console.WriteLine("Input={0}. Result={1}",
              input, result);
        }
    }
```

### 转换“using”和“namespace”

这些关键字很简单：

- `using` 变成 `open`
- 带花括号的 `namespace` 变成了仅仅是 `namespace`。

与 C# 不同，F# 文件通常不声明名称空间，除非它们需要与其他 .NET 代码文件互操作。文件名本身充当默认命名空间。

请注意，如果使用名称空间，它必须位于其他任何东西之前，例如“open”。这与大多数 C# 代码的顺序相反。

### 转换类

要声明一个简单类，请使用：

```F#
type myClassName() =
   ... code ...
```

请注意，类名后有括号。这些是类定义所必需的。

更复杂的类定义将在下一个示例中显示，您将阅读类的完整讨论。

### 转换函数/方法签名

对于函数/方法签名：

- 参数列表周围不需要括号
- 空格用于分隔参数，而不是逗号
- 等号表示函数体的开始，而不是花括号
- 参数通常不需要类型，但如果你确实需要它们：
  - 类型名称位于值或参数之后
  - 参数名称和类型用冒号分隔
  - 在为参数指定类型时，您可能应该将参数对括在括号中，以避免意外行为。
  - 函数的返回类型作为一个整体以冒号作为前缀，并位于所有其他参数之后

这是一个 C# 函数签名：

```c#
int Square(int input) { ... code ...}
```

下面是具有显式类型的相应 F# 函数签名：

```F#
let Square (input:int) :int =  ... code ...
```

但是，由于 F# 通常可以推断参数和返回类型，因此很少需要显式指定它们。

这是一个更典型的 F# 签名，带有推断类型：

```F#
let Square input =  ... code ...
```

### void

C# 中的 `void` 关键字通常不需要，但如果需要，将转换为 `unit`

C# 代码：

```c#
void PrintSquare(int input) { ... code ...}
```

可以转换为 F# 代码：

```F#
let PrintSquare (input:int) :unit =  ... code ...
```

但同样，很少需要特定的类型，因此 F# 版本只是：

```F#
let PrintSquare input =  ... code ...
```

### 转换函数/方法体

在一个函数体中，你可能有以下组合：

- 变量声明和赋值
- 函数调用
- 控制流报表
- 返回值

我们将依次快速了解移植这些程序，但控制流除外，我们稍后将讨论。

### 转换变量声明

几乎总是，你可以单独使用 `let`，就像 C# 中的 `var` 一样：

```F#
// C# variable declaration
var result = input * input;
// F# value declaration
let result = input * input
```

与 C# 不同，您必须始终将某些内容赋值（“绑定”）给 F# 值，作为其声明的一部分。

```F#
// C# example
int unassignedVariable; //valid
// F# example
let unassignedVariable // not valid
```

如上所述，如果需要在声明后更改值，则必须使用“mutable”关键字。

如果需要为值指定类型，则类型名称在值或参数之后，前面加一个冒号。

```F#
// C# example
int variableName = 42;
// F# example
let variableName:int = 42
```

### 转换函数调用

调用本机 F# 函数时，不需要括号或逗号。换句话说，调用函数的规则与定义函数时的规则相同。

以下是定义函数并调用它的 C# 代码：

```c#
// define a method/function
int Square(int input) { ... code  ...}

// call it
var result = Square(input);
```

然而，由于 F# 通常可以推断参数和返回类型，因此很少需要显式指定它们。因此，以下是定义函数并调用它的典型 F# 代码：

```F#
// define a function
let Square input = ... code ...

// call it
let result = Square input
```

### 返回值

在 C# 中，您可以使用 `return` 关键字。但在 F# 中，块中的最后一个值会自动成为“return”值。

这是返回 `result` 变量的 C# 代码。

```c#
public int Square(int input)
{
    var result = input * input;
    return result;   //explicit "return" keyword
}
```

这是 F# 的等价物。

```F#
let Square input =
    let result = input * input
    result        // implicit "return" value
```

这是因为 F# 是基于表达式的。一切都是一个表达式，块表达式作为一个整体的值就是块中最后一个表达式的值。

有关面向表达式的代码的更多详细信息，请参阅“表达式与语句”。

### 打印到控制台

要用 C# 打印输出，通常使用 `Console.WriteLine` 或类似产品。在 F# 中，你通常使用 `printf` 或类似的，这是类型安全的。（有关使用“printf”家族的更多详细信息）。

### 示例 #1 的完整移植

综上所述，这是示例 #1 到 F# 的完整直接移植。

再次是 C# 代码：

```c#
using System;
using System.Collections.Generic;

namespace PortingToFsharp
{
    public class Squarer
    {
        public int Square(int input)
        {
            var result = input * input;
            return result;
        }

        public void PrintSquare(int input)
        {
            var result = this.Square(input);
            Console.WriteLine("Input={0}. Result={1}",
              input, result);
        }
    }
```

以及等效的 F# 代码：

```F#
namespace PortingToFsharp

open System
open System.Collections.Generic

type Squarer() =

    let Square input =
        let result = input * input
        result

    let PrintSquare input =
        let result = Square input
        printf "Input=%i. Result=%i" input result
```

