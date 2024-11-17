# [返回主 Mardown](./FSharpForFunAndProfit翻译.md)



# 1 了解F#类型：简介

*Part of the "Understanding F# types" series (*[link](https://fsharpforfunandprofit.com/posts/types-intro/#series-toc)*)*

类型的新世界
01六月2012 这篇文章是超过3年

https://fsharpforfunandprofit.com/posts/types-intro/

*注意：在阅读本系列之前，我建议你先阅读“函数式思维”和“表达式和语法”系列。*

F# 不仅仅是函数；强大的字体系统是另一个关键因素。就像函数一样，理解类型系统对于流利和舒适地使用语言至关重要。

现在，到目前为止，我们已经看到了一些可以用作函数输入和输出的基本类型：

- 原始类型，如 `int`、`float`、`string` 和 `bool`
- 简单函数类型，如 `int->int`
- `unit` 类型
- 泛型类型。

这些类型都不应该陌生。C# 和其他命令式语言中也有类似的功能。

但在本系列中，我们将介绍一些在函数式语言中非常常见但在命令式语言中不常见的新类型。

我们将在本系列中探讨的扩展类型包括：

- 元组
- 记录
- 联合
- 选项类型
- 枚举类型

对于所有这些类型，我们将讨论抽象原则和如何在实践中使用它们的细节。

列表和其他递归数据类型也是非常重要的类型，但关于它们有很多要说的，所以它们需要自己的序列！

# 2 F# 中的类型概述

*Part of the "Understanding F# types" series (*[link](https://fsharpforfunandprofit.com/posts/overview-of-types-in-fsharp/#series-toc)*)*

纵观全局
02六月2012 这篇文章是超过3年

https://fsharpforfunandprofit.com/posts/overview-of-types-in-fsharp/

在我们深入探讨所有具体类型之前，让我们先看看大局。

## 什么是类型？

如果你来自面向对象的设计背景，那么“函数式思维”所涉及的范式转变之一就是改变你对类型的思考方式。

一个设计良好的面向对象程序将非常关注行为而不是数据，因此它将使用大量的多态性，无论是使用“鸭子类型”还是显式接口，并试图避免对传递的实际具体类有明确的了解。

另一方面，一个设计良好的函数式程序将非常关注数据类型而不是行为。F# 比 C# 等命令式语言更强调正确设计类型，本系列和后续系列中的许多示例将侧重于创建和细化类型定义。

那么，什么是类型？类型很难定义。一本著名教科书中的一个定义是：

> “类型系统是一种易于处理的句法方法，通过根据短语计算的值的种类对短语进行分类来证明某些程序行为的缺失”（*Benjamin Pierce，Types and Programming Languages*）

好吧，这个定义有点技术性。那么，让我们回过头来看看——在实践中我们用类型做什么？在 F# 的上下文中，你可以认为类型有两种主要的使用方式：

- 首先，作为一个值的注释，允许进行某些检查，特别是在编译时。换句话说，类型允许您进行“编译时单元测试”。
- 第二，作为函数作用的域。也就是说，类型是一种数据建模工具，允许您在代码中表示现实世界的域。

这两个定义相互作用。类型定义越能反映真实世界的领域，它们对业务规则的静态编码就越好。它们对业务规则的静态编码越好，“编译时单元测试”的工作就越好。在理想情况下，如果你的程序编译成功，那么它真的是正确的！

## 有哪些类型？

F# 是一种混合语言，因此它有多种类型：一些来自函数式背景，一些来自面向对象背景。

一般来说，F# 中的类型可以分为以下几类：

- **普通 .NET 类型**。这些类型符合 .NET 公共语言基础架构（CLI），并且易于移植到每个 .NET 语言。
- **F# 特定类型**。这些类型是 F# 语言的一部分，专为纯函数式编程而设计。

如果你熟悉 C#，你会知道所有的 CLI 类型。其中包括：

- 内置值类型（int、bool等）。
- 内置引用类型（字符串等）。
- 用户定义的值类型（枚举和结构）。
- 类和接口
- 委托
- 数组

F# 的具体类型包括：

- 函数类型（与委托或 C# lambda 不同）
- unit 类型
- 元组
- 记录
- 可区分联合
- 选项类型
- 列表（与 .NET List 类不同）

我强烈建议在创建新类型时，坚持使用 F# 特定的类型，而不是使用类。与 CLI 类型相比，它们具有许多优势，例如：

- 它们是不可变的
- 它们不能为空
- 它们具有内在的结构平等性和可比性
- 它们内置了漂亮的打印功能

## 总和和乘积类型

理解 F# 中类型的力量的关键是，大多数新类型都是使用两个基本操作从其他类型构建的：**求和**和**乘积**。

也就是说，在 F# 中，你可以定义新类型，就像你在做代数一样：

```
define typeZ = typeX "plus" typeY
define typeW = typeX "times" typeZ
```

在本系列后面详细讨论元组（乘积）和可区分联合（总和）类型之前，我将暂缓解释**总和**和**乘积**在实践中的含义。

关键在于，通过以各种方式使用这些“乘积”和“求和”方法将现有类型组合在一起，可以产生无限数量的新类型。这些统称为“代数数据类型”或 ADT（不要与抽象数据类型混淆，也称为 ADT）。代数数据类型可用于对任何东西进行建模，包括列表、树和其他递归类型。

特别是求和或“联合”类型非常有价值，一旦你习惯了，你会发现它们是不可或缺的！

## 类型是如何定义的

尽管具体细节可能有所不同，但每个类型定义都是相似的。所有类型定义都以“`type`”关键字开头，后跟类型的标识符，后跟任何泛型类型参数，后跟定义。例如，以下是各种类型的一些类型定义：

```F#
type A = int * int
type B = {FirstName:string; LastName:string}
type C = Circle of int | Rectangle of int * int
type D = Day | Month | Year
type E<'a> = Choice1 of 'a | Choice2 of 'a * 'a

type MyClass(initX:int) =
   let x = initX
   member this.Method() = printf "x=%i" x
```

正如我们在上一篇文章中所说，有一种特殊的语法用于定义不同于普通表达式语法的新类型。所以一定要意识到这种差异。

类型只能在命名空间或模块中声明。但这并不意味着你总是必须在顶层创建它们——如果你需要隐藏它们，你可以在嵌套模块中创建类型。

```F#
module sub =
    // type declared in a module
    type A = int * int

    module private helper =
        // type declared in a submodule
        type B = B of string list

        //internal access is allowed
        let b = B ["a";"b"]

//outside access not allowed
let b = sub.helper.B ["a";"b"]
```

类型不能在函数内部声明。

```F#
let f x =
    type A = int * int  //unexpected keyword "type"
    x * x
```

## 构建和解构类型

定义类型后，使用“构造函数”表达式创建类型的实例，该表达式通常看起来与类型定义本身非常相似。

```F#
let a = (1,1)
let b = { FirstName="Bob"; LastName="Smith" }
let c = Circle 99
let c' = Rectangle (2,1)
let d = Month
let e = Choice1 "a"
let myVal = MyClass 99
myVal.Method()
```

有趣的是，在进行模式匹配时，也使用相同的“构造函数”语法来“解构”类型：

```F#
let a = (1,1)                                  // "construct"
let (a1,a2) = a                                // "deconstruct"

let b = { FirstName="Bob"; LastName="Smith" }  // "construct"
let { FirstName = b1 } = b                     // "deconstruct"

let c = Circle 99                              // "construct"
match c with
| Circle c1 -> printf "circle of radius %i" c1 // "deconstruct"
| Rectangle (c2,c3) -> printf "%i %i" c2 c3    // "deconstruct"

let c' = Rectangle (2,1)                       // "construct"
match c' with
| Circle c1 -> printf "circle of radius %i" c1 // "deconstruct"
| Rectangle (c2,c3) -> printf "%i %i" c2 c3    // "deconstruct"
```

在阅读本系列文章时，请注意构造函数是如何以两种方式使用的。

## “type”关键字的字段指南

相同的“type”关键字用于定义所有 F# 类型，因此如果您是 F# 新手，它们看起来都非常相似。以下是这些类型的快速列表以及如何区分它们。

| 类型            | 例子                                                         | 区分特点                                                     |
| :-------------- | :----------------------------------------------------------- | :----------------------------------------------------------- |
| **缩略 (别名)** | `type ProductCode = string`<br />`type transform<'a> = 'a -> 'a ` | 仅使用等号。                                                 |
| **元组**        | `//not explicitly defined with type keyword `<br />`//usage`<br />`let t = 1,2`<br />`let s = (3,4) ` | 始终可用，并且没有用“type”关键字明确定义。用法用逗号表示（带可选括号）。 |
| **记录**        | `type Product = {code:ProductCode; price:float }`<br />`type Message<'a> = {id:int; body:'a}`<br />`//usage`<br />`let p = {code="X123"; price=9.99}`<br />`let m = {id=1; body="hello"} ` | 大括号。使用分号分隔字段。                                   |
| **可区分联合**  | `type MeasurementUnit = Cm | Inch | Mile`<br />`type Name =`<br />`    | Nickname of string`<br />`    | FirstLast of string * string`<br />`type Tree<'a> =`<br />`    | E`<br />`    | T of Tree<'a> * 'a * Tree<'a>`<br />`//usage`<br />`let u = Inch`<br />`let name = Nickname("John")`<br />`let t = T(E,"John",E) ` | 竖线字符。使用“of”表示类型。                                 |
| **枚举**        | `type Gender = | Male = 1 | Female = 2`<br />`//usage`<br />`let g = Gender.Male ` | 类似于 Union，但使用 equals 和 int 值                        |
| **类**          | `type Product (code:string, price:float) =`<br />`   let isFree = price=0.0`<br />`   new (code) = Product(code,0.0)`<br />`   member this.Code = code`<br />`   member this.IsFree = isFree`<br />`//usage`<br />`let p = Product("X123",9.99)`<br />`let p2 = Product("X123") ` | 名称后有函数样式参数列表，用作构造函数。有“member”关键字。为二级构造函数提供“new”关键字。 |
| **接口**        | `type IPrintable =`<br />`   abstract member Print : unit -> unit ` | 与类相同，但所有成员都是抽象的。抽象成员具有冒号和类型签名，而不是具体的实现。 |
| **结构**        | `type Product=`<br />`   struct`<br />`      val code:string`<br />`      val price:float`<br />`      new(code) = { code = code; price = 0.0 }`<br />`   end`<br />`//usage`<br />`let p = Product()`<br />`let p2 = Product("X123") ` | 有“struct”关键字。使用“val”定义字段。可以有构造函数。        |

# 3 类型缩写

*Part of the "Understanding F# types" series (*[link](https://fsharpforfunandprofit.com/posts/type-abbreviations/#series-toc)*)*

也称为别名
03 六月 2012 这篇文章是超过 3 年

https://fsharpforfunandprofit.com/posts/type-abbreviations/

让我们从最简单的类型定义开始，一个类型缩写或别名。

其形式如下：

```F#
type [typename] = [existingType]
```

其中“现有类型”可以是任何类型：我们已经看到的基本类型之一，或者我们很快就会看到的扩展类型之一。

一些例子：

```F#
type RealNumber = float
type ComplexNumber = float * float
type ProductCode = string
type CustomerId = int
type AdditionFunction = int->int->int
type ComplexAdditionFunction =
       ComplexNumber-> ComplexNumber -> ComplexNumber
```

等等——非常简单。

类型缩写对于提供文档和避免重复书写签名很有用。在上面的例子中，`ComplexNumber` 和 `AdditionFunction` 演示了这一点。

另一个用途是在类型的使用和类型的实际实现之间提供一定程度的解耦。在上面的示例中，`ProductCode` 和 `CustomerId` 演示了这一点。我可以很容易地将 `CustomerId` 更改为字符串，而无需更改（大部分）代码。

然而，需要注意的是，这实际上只是一个别名或缩写；你实际上并没有创建一个新类型。因此，如果我定义一个函数，我明确地说它是 `AdditionFunction`：

```F#
type AdditionFunction = int->int->int
let f:AdditionFunction = fun a b -> a + b
```

编译器将擦除别名，并返回一个普通的 `int->int->int` 作为函数签名。

特别是，没有真正的封装。我可以在任何使用 `CustomerId` 的地方使用显式 `int`，编译器不会抱怨。如果我试图创建实体 id 的安全版本，比如这样：

```F#
type CustomerId = int
type OrderId = int
```

那么我会失望的。没有什么能阻止我使用 `OrderId` 代替 `CustomerId`，反之亦然。为了得到像这样的真正的封装类型，我们需要使用单案例联合类型，如稍后的文章所述。

# 4 元组

*Part of the "Understanding F# types" series (*[link](https://fsharpforfunandprofit.com/posts/tuples/#series-toc)*)*

将类型相乘
04六月2012 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/tuples/

我们已经为第一个扩展类型——元组做好了准备。

让我们再次退后一步，看看像“int”这样的类型。正如我们之前所暗示的，与其将“int”视为抽象的东西，不如将其视为所有可能值的具体集合，即集合 {…，-3，-2，-1，0，2，3，…}。

接下来，想象一下这个“int”集合的两个副本。我们可以通过将它们的笛卡尔积“相乘”在一起；也就是说，通过选取两个“int”列表的所有可能组合来创建一个新的对象列表，如下所示：



正如我们已经看到的，这些对在 F# 中被称为元组。现在你可以看到为什么他们有这样的类型签名了。在这个例子中，“int 乘以 int”类型被称为“`int * int`”，星号当然意味着“乘法”！这种新类型的有效实例是所有对：（-2,2）、（-1,0）、（2,2）等等。

让我们看看它们在实践中是如何使用的：

```F#
let t1 = (2,3)
let t2 = (-2,7)
```

现在，如果你评估上面的代码，你会看到 t1 和 t2 的类型是预期的 `int * int`。

```F#
val t1 : int * int = (2, 3)
val t2 : int * int = (-2, 7)
```

这种“乘积”方法可用于从任何类型的混合中生成元组。这里有一个“int 乘以 bool”。

int * bool 元组

这是 F# 的用法。上面的元组类型具有签名“`int*bool`”。

```F#
let t3 = (2,true)
let t4 = (7,false)

// the signatures are:
val t3 : int * bool = (2, true)
val t4 : int * bool = (7, false)
```

当然，字符串也可以使用。所有可能的字符串的宇宙都非常大，但从概念上讲，它是一样的。下面的元组类型具有签名“`string*int`”。

string*int 元组

测试用法和签名：

```F#
let t5 = ("hello",42)
let t6 = ("goodbye",99)

// the signatures are:
val t5 : string * int = ("hello", 42)
val t6 : string * int = ("goodbye", 99)
```

而且没有理由只将两种类型相乘。为什么不是三个？还是四个？例如，这是 `int * bool * string`类型。

int * bool * 字符串元组

测试用法和签名：

```F#
let t7 = (42,true,"hello")

// the signature is:
val t7 : int * bool * string = (42, true, "hello")
```

## 泛型元组

泛型也可以在元组中使用。

'a*'b 元组

使用通常与函数相关：

```F#
let genericTupleFn aTuple =
   let (x,y) = aTuple
   printfn "x is %A and y is %A" x y
```

函数签名为：

```F#
val genericTupleFn : 'a * 'b -> unit
```

这意味着“`genericTupleFn`”接受一个泛型元组 `('a*'b)` 并返回一个单位

## 复杂类型的元组

元组中可以使用任何类型：其他元组、类、函数类型等。以下是一些示例：

```F#
// define some types
type Person = {First:string; Last:string}
type Complex = float * float
type ComplexComparisonFunction = Complex -> Complex -> int

// define some tuples using them
type PersonAndBirthday = Person * System.DateTime
type ComplexPair = Complex * Complex
type ComplexListAndSortFunction = Complex list * ComplexComparisonFunction
type PairOfIntFunctions = (int->int) * (int->int)
```

## 关于元组的关键点

关于元组，需要了解的一些关键点是：

- 元组类型的特定实例是单个对象，类似于 C# 中的两元素数组。当将它们与函数一起使用时，它们被计为单个参数。
- 元组类型不能被赋予显式名称。元组类型的“名称”由相乘的类型组合决定。
- 乘法的顺序很重要。因此，`int*string` 与 `string*int` 的元组类型不同。
- 逗号是定义元组的关键符号，而不是括号。您可以定义不带括号的元组，尽管有时会令人困惑。在 F# 中，如果你看到逗号，它可能是元组的一部分。

这些要点非常重要——如果你不理解它们，你很快就会感到困惑！

值得重申之前文章中提出的观点：不要将元组误认为函数中的多个参数。

```F#
// a function that takes a single tuple parameter
// but looks like it takes two ints
let addConfusingTuple (x,y) = x + y
```

## 创建和匹配元组

F# 中的元组类型比其他扩展类型更原始。正如你所看到的，你不需要明确地定义它们，它们也没有名字。

创建元组很容易——只需使用逗号！

```F#
let x = (1,2)
let y = 1,2        // it's the comma you need, not the parentheses!
let z = 1,true,"hello",3.14   // create arbitrary tuples as needed
```

正如我们所见，要“解构”元组，请使用相同的语法：

```F#
let z = 1,true,"hello",3.14   // "construct"
let z1,z2,z3,z4 = z           // "deconstruct"
```

当像这样进行模式匹配时，你必须有相同数量的元素，否则你会得到一个错误：

```F#
let z1,z2 = z     // error FS0001: Type mismatch.
                  // The tuples have differing lengths
```

如果你不需要一些值，你可以使用“不在乎”符号（下划线）作为占位符。

```F#
let _,z5,_,z6 = z     // ignore 1st and 3rd elements
```

正如你可能猜到的，一个二元元组通常被称为“对”，一个三元元组被称为”三元组“，以此类推。在成对的特殊情况下，有函数 `fst` 和 `snd` 可以提取第一个和第二个元素。

```F#
let x = 1,2
fst x
snd x
```

他们只成对工作。尝试在三元组上使用 `fst` 会出错。

```F#
let x = 1,2,3
fst x              // error FS0001: Type mismatch.
                   // The tuples have differing lengths of 2 and 3
```

## 在实践中使用元组

与其他更复杂的类型相比，元组具有许多优势。它们可以在运行中使用，因为它们始终可用，无需定义，因此非常适合小型、临时、轻质结构。

### 使用元组返回多个值

一种常见的情况是，您希望从函数中返回两个值，而不仅仅是一个值。例如，在 `TryParse` 风格的函数中，您希望返回（a）值是否被解析，以及（b）如果被解析，解析的值是什么。

以下是 `TryParse` 的整数实现（当然，假设它还不存在）：

```F#
let tryParse intStr =
   try
      let i = System.Int32.Parse intStr
      (true,i)
   with _ -> (false,0)  // any exception

//test it
tryParse "99"
tryParse "abc"
```

这是另一个返回一对数字的简单示例：

```F#
// return word count and letter count in a tuple
let wordAndLetterCount (s:string) =
   let words = s.Split [|' '|]
   let letterCount = words |> Array.sumBy (fun word -> word.Length )
   (words.Length, letterCount)

//test
wordAndLetterCount "to be or not to be"
```

### 从其他元组创建元组

与大多数 F# 值一样，元组是不可变的，其中的元素不能被赋值。那么，如何更改元组呢？简短的回答是，你不能——你必须总是创造一个新的。

假设你需要编写一个函数，在给定一个元组的情况下，为每个元素添加一个元组。下面是一个明显的实现：

```F#
let addOneToTuple aTuple =
   let (x,y,z) = aTuple
   (x+1,y+1,z+1)   // create a new one

// try it
addOneToTuple (1,2,3)
```

这似乎有点冗长——有更紧凑的方法吗？是的，因为你可以直接在函数的参数中解构一个元组，这样函数就变成了一个单行：

```F#
let addOneToTuple (x,y,z) = (x+1,y+1,z+1)

// try it
addOneToTuple (1,2,3)
```

### 相等性

元组具有自动定义的相等操作：如果两个元组具有相同的长度并且每个槽中的值相等，则这两个元组相等。

```F#
(1,2) = (1,2)                      // true
(1,2,3,"hello") = (1,2,3,"bye")    // false
(1,(2,3),4) = (1,(2,3),4)          // true
```

试图比较不同长度的元组是一个类型错误：

```F#
(1,2) = (1,2,3)                    // error FS0001: Type mismatch
```

每个插槽中的类型也必须相同：

```F#
(1,2,3) = (1,2,"hello")   // element 3 was expected to have type
                          // int but here has type string
(1,(2,3),4) = (1,2,(3,4)) // elements 2 & 3 have different types
```

元组还具有基于元组中的值自动定义的哈希值，因此元组可以毫无问题地用作字典键。

```F#
(1,2,3).GetHashCode()
```

### 元组表示法

如前一篇文章所述，元组有一个很好的默认字符串表示，可以很容易地序列化。

```F#
(1,2,3).ToString()
```

# 5 记录

*Part of the "Understanding F# types" series (*[link](https://fsharpforfunandprofit.com/posts/records/#series-toc)*)*

用标签扩展元组
05六月2012 这篇文章是超过3年

https://fsharpforfunandprofit.com/posts/records/

正如我们在上一篇文章中提到的，在许多情况下，纯元组是有用的。但它们也有一些缺点。因为所有元组类型都是预定义的，所以你无法区分用于地理坐标的一对浮点数与用于复数的类似元组。当元组包含多个元素时，很容易混淆哪个元素在哪个位置。

在这些情况下，您要做的是标记元组中的每个槽，这将记录每个元素的用途，并强制区分由相同类型组成的元组。

输入“记录”类型。记录类型就是这样，一个每个元素都被标记的元组。

```F#
type ComplexNumber = { Real: float; Imaginary: float }
type GeoCoord = { Lat: float; Long: float }
```

记录类型具有标准的前导码：`type[typename] =` 后跟花括号。花括号内是一个 `label: type` 对列表，用分号分隔（记住，F# 中的所有列表都使用分号分隔符——逗号用于元组）。

让我们将记录类型的“类型语法”与元组类型进行比较：

```F#
type ComplexNumberRecord = { Real: float; Imaginary: float }
type ComplexNumberTuple = float * float
```

在记录类型中，没有“乘法”，只有一系列标记的类型。

> 关系数据库理论使用类似的“记录类型”概念。在关系模型中，关系是一个（可能是空的）有限元组集，所有元组都具有相同的有限属性集。这组属性通常被称为列名集。

## 制作和匹配记录

要创建记录值，请使用与类型定义类似的格式，但在标签后使用等号。这被称为“记录表达式”

```F#
type ComplexNumberRecord = { Real: float; Imaginary: float }
let myComplexNumber = { Real = 1.1; Imaginary = 2.2 } // use equals!

type GeoCoord = { Lat: float; Long: float } // use colon in type
let myGeoCoord = { Lat = 1.1; Long = 2.2 }  // use equals in let
```

要“解构”记录，请使用相同的语法：

```F#
let myGeoCoord = { Lat = 1.1; Long = 2.2 }   // "construct"
let { Lat=myLat; Long=myLong } = myGeoCoord  // "deconstruct"
```

与往常一样，如果您不需要某些值，可以使用下划线作为占位符；或者更干净地说，完全去掉不需要的标签。

```F#
let { Lat=_; Long=myLong2 } = myGeoCoord  // "deconstruct"
let { Long=myLong3 } = myGeoCoord         // "deconstruct"
```

如果你只需要一个属性，你可以使用点表示法，而不是模式匹配。

```F#
let x = myGeoCoord.Lat
let y = myGeoCoord.Long
```

请注意，在解构时可以省略标签，但在构建时则不能：

```F#
let myGeoCoord = { Lat = 1.1; }  // error FS0764: No assignment
  // given for field 'Long'
```

> 记录类型最显著的特征之一是使用花括号。与 C 风格的语言不同，花括号在 F# 中很少使用——只用于记录、序列、计算表达式（其中序列是一个特例）和对象表达式（动态创建接口的实现）。稍后将讨论这些其他用途。

### 标签顺序

与元组不同，标签的顺序并不重要。因此，以下两个值是相同的：

```F#
let myGeoCoordA = { Lat = 1.1; Long = 2.2 }
let myGeoCoordB = { Long = 2.2; Lat = 1.1 }   // same as above
```

### 命名冲突

在上面的例子中，我们可以只使用标签名“`lat`”和“`long`”来构造记录。神奇的是，编译器知道要创建什么记录类型。（好吧，事实上，这并没有那么神奇，因为只有一种记录类型有这些确切的标签。）

但是，如果有两种具有相同标签的记录类型，会发生什么？编译器怎么知道你指的是哪一个？答案是它不能——它将使用最近定义的类型，在某些情况下还会发出警告。尝试评估以下内容：

```F#
type Person1 = {First:string; Last:string}
type Person2 = {First:string; Last:string}
let p = {First="Alice"; Last="Jones"} //
```

`p` 是什么类型的？答案：`Person2`，这是用这些标签定义的最后一个类型。

如果你试图解构，你会得到一个关于模糊字段标签的警告。

```F#
let {First=f; Last=l} = p
// warning FS0667: The labels of this record do not
//   uniquely determine a corresponding record type
```

你怎么能解决这个问题？只需将类型名称作为限定符添加到至少一个标签中即可。

```F#
let p = {Person1.First="Alice"; Last="Jones"}
//  ^Person1
```

如果需要，您甚至可以添加一个完全限定的名称（带命名空间）。这是一个使用模块的示例。

```F#
module Module1 =
  type Person = {First:string; Last:string}

module Module2 =
  type Person = {First:string; Last:string}

let p =
    {Module1.Person.First="Alice"; Module1.Person.Last="Jones"}
```

或者，您可以添加一个显式的类型注释，以便编译器知道记录是什么类型：

```F#
let p : Module1.Person =
  {First="Alice"; Last="Jones"}
```

当然，如果你能确保本地命名空间中只有一个版本，你就可以完全避免这样做。

```F#
module Module3 =
  open Module1  // bring only one definition into scope
  let p = {First="Alice"; Last="Jones"} // will be Module1.Person
```

这个故事的寓意是，在定义记录类型时，如果可能的话，你应该尝试使用唯一的标签，否则充其量只会得到丑陋的代码，最坏的情况是会出现意外的行为。

请注意，在 F# 中，与其他一些函数式语言不同，具有完全相同结构定义的两个类型不是同一类型。这被称为“名义”类型系统，其中两个类型只有在名称相同的情况下才相等，而“结构”类型系统中，具有相同结构的定义将是相同的类型，而不管它们被称为什么。

> 如果你觉得这篇文章很有趣，看看我的《领域建模使函数化》一书！这是对领域驱动设计、类型建模和函数式编程的一个很好的介绍。

## 在实践中使用记录

我们如何使用记录？让我们数一数方式…

### 使用记录作为函数结果

就像元组一样，记录对于从函数传递多个值很有用。让我们重新审视前面描述的元组示例，重写为使用记录：

```F#
// the tuple version of TryParse
let tryParseTuple intStr =
  try
    let i = System.Int32.Parse intStr
    (true,i)
  with _ -> (false,0)  // any exception

// for the record version, create a type to hold the return result
type TryParseResult = {Success:bool; Value:int}

// the record version of TryParse
let tryParseRecord intStr =
  try
    let i = System.Int32.Parse intStr
    {Success=true;Value=i}
  with _ -> {Success=false;Value=0}

//test it
tryParseTuple "99"
tryParseRecord "99"
tryParseTuple "abc"
tryParseRecord "abc"
```

您可以看到，在返回值中使用显式标签会使其更容易理解（当然，在实践中，我们可能会使用 `Option` 类型，这将在后面的文章中讨论）。

这是使用记录而不是元组的单词和字母计数示例：

```F#
//define return type
type WordAndLetterCountResult = {WordCount:int; LetterCount:int}

let wordAndLetterCount (s:string) =
  let words = s.Split [|' '|]
  let letterCount = words |> Array.sumBy (fun word -> word.Length )
  {WordCount=words.Length; LetterCount=letterCount}

//test
wordAndLetterCount "to be or not to be"
```

### 从其他记录创建记录

同样，与大多数 F# 值一样，记录是不可变的，其中的元素不能被赋值。那么，如何更改记录呢？同样，答案是你不能——你必须总是创建一个新的。

假设你需要编写一个函数，在给定 `GeoCoord` 记录的情况下，为每个元素添加一个。这是：

```F#
let addOneToGeoCoord aGeoCoord =
  let {Lat=x; Long=y} = aGeoCoord
  {Lat = x + 1.0; Long = y + 1.0}   // create a new one

// try it
addOneToGeoCoord {Lat=1.1; Long=2.2}
```

但是，你可以通过直接解构函数的参数来简化，这样函数就变成了一行：

```F#
let addOneToGeoCoord {Lat=x; Long=y} = {Lat=x+1.0; Long=y+1.0}

// try it
addOneToGeoCoord {Lat=1.0; Long=2.0}
```

或者根据你的口味，你也可以使用点符号来获得属性：

```F#
let addOneToGeoCoord aGeoCoord =
  {Lat=aGeoCoord.Lat + 1.0; Long= aGeoCoord.Long + 1.0}
```

在许多情况下，您只需调整一两个字段，而其他字段则保持不变。为了使生活更轻松，对于这种常见情况，有一种特殊的语法，即“`with`”关键字。您从原始值开始，后跟“with”，然后是要更改的字段。以下是一些示例：

```F#
let g1 = {Lat=1.1; Long=2.2}
let g2 = {g1 with Lat=99.9}   // create a new one

let p1 = {First="Alice"; Last="Jones"}
let p2 = {p1 with Last="Smith"}
```

“with”的技术术语是复制和更新记录表达式。

### 记录相等性

与元组一样，记录具有自动定义的相等操作：如果两个记录具有相同的类型并且每个槽中的值相等，则这两个记录是相等的。

```F#
let p1 = {First="Alice"; Last="Jones"}
let p2 = {First="Alice"; Last="Jones"}
printfn "p1=p2 is %b" (p1=p2)  // p1=p2 is true
```

记录还具有基于记录中的值自动定义的哈希值，因此记录可以在哈希集合中使用而不会出现任何问题。

```F#
let h1 = {First="Alice"; Last="Jones"}.GetHashCode()
let h2 = {First="Alice"; Last="Jones"}.GetHashCode()
printfn "h1=h2 is %b" (h1=h2)  // h1=h2 is true
```

### 记录表示

如前一篇文章所述，记录有一个很好的默认字符串表示，可以很容易地序列化。默认的 `ToString()` 实现使用相同的表示。

```F#
let p = {First="Alice"; Last="Jones"}
printfn "%A" p
// output:
//   { First = "Alice"
//     Last = "Jones" }
printfn "%O" p   // same as above
```

## 侧边栏：打印格式字符串中的 `%A` 与 `%O`

我们刚刚看到打印格式说明符 %A 和 %O 产生了相同的结果。那么，为什么会有这种差异呢？

`%A` 使用与交互式输出相同的漂亮打印机打印值。但是 `%O` 使用 `ToString()`，这意味着如果 `ToString` 方法没有被重写，`%O` 将给出默认（有时没有帮助）输出。因此，一般来说，对于用户定义的类型，您应该尝试使用 `%A` 而不是 `%O`，除非您想重写 ToString()。

```F#
type Person = {First:string; Last:string}
  with
  override this.ToString() = sprintf "%s %s" this.First this.Last

printfn "%A" {First="Alice"; Last="Jones"}
// output:
//   { First = "Alice"
//     Last = "Jones" }
printfn "%O" {First="Alice"; Last="Jones"}
// output:
//   "Alice Jones"
```

但请注意，F# “类”类型没有标准的漂亮打印格式，因此 `%A` 和 `%O` 同样没有帮助，除非你重写 `ToString()`。

# 6 可区分联合

*Part of the "Understanding F# types" series (*[link](https://fsharpforfunandprofit.com/posts/discriminated-unions/#series-toc)*)*

将类型添加在一起
06六月2012 这篇文章是超过3年

https://fsharpforfunandprofit.com/posts/discriminated-unions/

元组和记录是通过将现有类型“相乘”在一起来创建新类型的示例。在系列文章的开头，我提到创建新类型的另一种方法是“求和”现有类型。这是什么意思？

好吧，假设我们想定义一个处理整数或布尔值的函数，也许可以将它们转换为字符串。但我们希望严格，不接受任何其他类型（如浮点数或字符串）。这是一个函数的示意图：

来自int联合bool的函数

我们如何表示此函数的域？

我们需要的是一个表示所有可能整数加上所有可能布尔值的类型。

int联合布尔值

换句话说，是一种“求和”类型。在这种情况下，新类型是整数类型加布尔类型的“和”。

在 F# 中，求和类型被称为“判别联合”类型。每个组件类型（称为联合案例）都必须用标签（称为案例标识符或标签）标记，以便将其区分开来（“区分”）。标签可以是您喜欢的任何标识符，但必须以大写字母开头。

以下是我们如何定义上面的类型：

```F#
type IntOrBool =
  | I of int
  | B of bool
```

“I”和“B”只是任意的标签；我们本可以使用任何其他有意义的标签。

对于小类型，我们可以把定义放在一行上：

```F#
type IntOrBool = I of int | B of bool
```

组件类型可以是您喜欢的任何其他类型，包括元组、记录、其他联合类型等。

```F#
type Person = {first:string; last:string}  // define a record type
type IntOrBool = I of int | B of bool

type MixedType =
  | Tup of int * int  // a tuple
  | P of Person       // use the record type defined above
  | L of int list     // a list of ints
  | U of IntOrBool    // use the union type defined above
```

你甚至可以有递归的类型，也就是说，它们引用自己。这通常是树结构的定义方式。递归类型将在稍后进行更详细的讨论。

### 求和类型 vs C++ 联合和 VB 变体

乍一看，求和类型可能与 C++ 中的联合类型或 Visual Basic 中的变体类型相似，但有一个关键的区别。C++ 中的联合类型不是类型安全的，可以使用任何可能的标记访问存储在该类型中的数据。F# 可区分联合类型是安全的，数据只能以一种方式访问。将其视为两种类型的总和（如图所示），而不仅仅是数据的叠加，这确实很有帮助。

## 关于联合类型的关键点

关于联合类型，需要了解的一些关键事项是：

- 垂直条在第一个组件之前是可选的，因此以下定义都是等效的，正如您通过检查交互式窗口的输出所看到的：

  ```F#
  type IntOrBool = I of int | B of bool     // without initial bar
  type IntOrBool = | I of int | B of bool   // with initial bar
  type IntOrBool =
     | I of int
     | B of bool      // with initial bar on separate lines
  ```

- 标签必须以大写字母开头。因此，以下将给出错误：

  ```F#
  type IntOrBool = int of int| bool of bool
  //  error FS0053: Discriminated union cases
  //                must be uppercase identifiers
  ```

- 其他命名类型（如 `Person` 或 `IntOrBool`）必须在联合类型之外预定义。你不能把它们定义为“内联”，然后写这样的东西：

  ```F#
  type MixedType =
    | P of  {first:string; last:string}  // error
  ```

  或

  ```F#
  type MixedType =
    | U of (I of int | B of bool)  // error
  ```

- 标签可以是任何标识符，包括组件类型本身的名称，如果您不期望它，这可能会非常令人困惑。例如，如果使用 `Int32` 和 `Boolean` 类型（来自 `System` 命名空间），并且标签的名称相同，我们将有一个完全有效的定义：

  ```F#
  open System
  type IntOrBool = Int32 of Int32 | Boolean of Boolean
  ```

这种“重复命名”风格实际上很常见，因为它准确地记录了组件类型是什么。

## 构造联合类型的值

要创建联合类型的值，您可以使用仅引用一种可能的联合情况的“构造函数”。然后，构造函数遵循定义的形式，使用case标签，就像它是一个函数一样。在 `IntOrBool` 的例子中，你会写：

```F#
type IntOrBool = I of int | B of bool

let i  = I 99    // use the "I" constructor
// val i : IntOrBool = I 99

let b  = B true  // use the "B" constructor
// val b : IntOrBool = B true
```

结果值与标签以及组件类型一起打印出来：

```F#
val [value name] : [type]    = [label] [print of component type]
val i            : IntOrBool = I       99
val b            : IntOrBool = B       true
```

如果 case 构造函数有多个“参数”，则以调用函数的方式构造它：

```F#
type Person = {first:string; last:string}

type MixedType =
  | Tup of int * int
  | P of Person

let myTup  = Tup (2,99)    // use the "Tup" constructor
// val myTup : MixedType = Tup (2,99)

let myP  = P {first="Al"; last="Jones"} // use the "P" constructor
// val myP : MixedType = P {first = "Al";last = "Jones";}
```

联合类型的 case 构造函数是普通函数，因此您可以在任何需要函数的地方使用它们。例如，在 `List.map` 中：

```F#
type C = Circle of int | Rectangle of int * int

[1..10]
|> List.map Circle

[1..10]
|> List.zip [21..30]
|> List.map Rectangle
```

### 命名冲突

如果一个特定案例有一个唯一的名称，那么要构造的类型将是明确的。

但是，如果你有两种类型的箱子有相同的标签，会发生什么？

```F#
type IntOrBool1 = I of int | B of bool
type IntOrBool2 = I of int | B of bool
```

在这种情况下，通常使用最后一个定义：

```F#
let x = I 99                // val x : IntOrBool2 = I 99
```

但最好明确限定类型，如图所示：

```F#
let x1 = IntOrBool1.I 99    // val x1 : IntOrBool1 = I 99
let x2 = IntOrBool2.B true  // val x2 : IntOrBool2 = B true
```

如果类型来自不同的模块，您也可以使用模块名称：

```F#
module Module1 =
  type IntOrBool = I of int | B of bool

module Module2 =
  type IntOrBool = I of int | B of bool

module Module3 =
  let x = Module1.IntOrBool.I 99 // val x : Module1.IntOrBool = I 99
```

### 联合类型匹配

对于元组和记录，我们已经看到“解构”一个值使用与构造它相同的模型。对于联合类型也是如此，但我们有一个复杂的问题：我们应该解构哪种情况？

这正是“匹配”表达式的设计目的。正如您现在应该意识到的，匹配表达式语法与联合类型的定义方式有相似之处。

```F#
// definition of union type
type MixedType =
  | Tup of int * int
  | P of Person

// "deconstruction" of union type
let matcher x =
  match x with
  | Tup (x,y) ->
        printfn "Tuple matched with %i %i" x y
  | P {first=f; last=l} ->
        printfn "Person matched with %s %s" f l

let myTup = Tup (2,99)                 // use the "Tup" constructor
matcher myTup

let myP = P {first="Al"; last="Jones"} // use the "P" constructor
matcher myP
```

让我们分析一下这里发生了什么：

- 整个匹配表达式的每个“分支”都是一个模式表达式，旨在匹配联合类型的相应情况。
- 模式从特定情况的标签开始，然后模式的其余部分以通常的方式解构该情况的类型。
- 模式后面是箭头“->”，然后是要执行的代码。

## 空箱

联合案例的标签后面不必有任何类型。以下都是有效的联合类型：

```F#
type Directory =
  | Root                   // no need to name the root
  | Subdirectory of string // other directories need to be named

type Result =
  | Success                // no string needed for success state
  | ErrorMessage of string // error message needed
```

如果所有的 case 都是空的，那么我们有一个“枚举样式”联合：

```F#
type Size = Small | Medium | Large
type Answer = Yes | No | Maybe
```

请注意，这种“枚举样式”的联合与后面讨论的真正的 C# 枚举类型不同。

要创建一个空案例，只需将标签用作不带任何参数的构造函数：

```F#
let myDir1 = Root
let myDir2 = Subdirectory "bin"

let myResult1 = Success
let myResult2 = ErrorMessage "not found"

let mySize1 = Small
let mySize2 = Medium
```

## 单一案例

有时，只使用一种情况创建联合类型是有用的。这可能看起来没用，因为你似乎没有增加值。但事实上，这是一种非常有用的做法，可以加强类型安全*。

*在未来的系列文章中，我们将看到，结合模块签名，单案例联合还可以帮助实现数据隐藏和基于能力的安全性。
例如，假设我们有客户 id 和订单 id，它们都由整数表示，但它们永远不应该相互分配。

正如我们之前看到的，类型别名方法不起作用，因为别名只是同义词，不会创建不同的类型。以下是如何使用别名：

```F#
type CustomerId = int   // define a type alias
type OrderId = int      // define another type alias

let printOrderId (orderId:OrderId) =
   printfn "The orderId is %i" orderId

//try it
let custId = 1          // create a customer id
printOrderId custId   // Uh-oh!
```

但是，即使我明确地将 `orderId` 参数注释为 `OrderId` 类型，我也无法确保客户 id 不会意外传入。

另一方面，如果我们创建简单的联合类型，我们可以很容易地执行类型区分。

```F#
type CustomerId = CustomerId of int   // define a union type
type OrderId = OrderId of int         // define another union type

let printOrderId (OrderId orderId) =  // deconstruct in the param
   printfn "The orderId is %i" orderId

//try it
let custId = CustomerId 1             // create a customer id
printOrderId custId                   // Good! A compiler error now.
```

这种方法在 C# 和 Java 中也是可行的，但由于为每种类型创建和管理特殊类的开销，很少使用。在 F# 中，这种方法是轻量级的，因此非常常见。

关于单案例联合类型的一个方便之处是，您可以直接对值进行模式匹配，而不必使用与表达式的完全匹配。

```F#
// deconstruct in the param
let printCustomerId (CustomerId customerIdInt) =
   printfn "The CustomerId is %i" customerIdInt

// or deconstruct explicitly through let statement
let printCustomerId2 custId =
   let (CustomerId customerIdInt) = custId  // deconstruct here
   printfn "The CustomerId is %i" customerIdInt

// try it
let custId = CustomerId 1             // create a customer id
printCustomerId custId
printCustomerId2 custId
```

但一个常见的“陷阱”是，在某些情况下，模式匹配必须有括号，否则编译器会认为你在定义一个函数！

```F#
let custId = CustomerId 1
let (CustomerId customerIdInt) = custId  // Correct pattern matching
let CustomerId customerIdInt = custId    // Wrong! New function?
```

同样，如果你确实需要创建一个具有单个 case 的枚举样式联合类型，你必须在类型定义中以竖线开头；否则编译器会认为您正在创建别名。

```F#
type TypeAlias = A     // type alias!
type SingleCase = | A   // single case union type
```

## 联合相等性

与其他核心 F# 类型一样，联合类型具有自动定义的相等操作：如果两个联合具有相同的类型和相同的大小写，并且该大小写的值相等，则这两个联合是相等的。

```F#
type Contact = Email of string | Phone of int

let email1 = Email "bob@example.com"
let email2 = Email "bob@example.com"

let areEqual = (email1=email2)
```

## 联合表示

联合类型有一个很好的默认字符串表示，可以很容易地序列化。但与元组不同，ToString() 表示法没有帮助。

```F#
type Contact = Email of string | Phone of int
let email = Email "bob@example.com"
printfn "%A" email    // nice
printfn "%O" email    // ugly!
```

# 7 选项类型

*Part of the "Understanding F# types" series (*[link](https://fsharpforfunandprofit.com/posts/the-option-type/#series-toc)*)*

为什么它不是 null 或 nullable
07六月2012 这篇文章是超过3年

https://fsharpforfunandprofit.com/posts/the-option-type/

现在让我们看看一个特定的联合类型，Option 类型。它是如此普遍和有用，以至于它实际上已经融入了语言中。

您已经看到了前面讨论的选项类型，但让我们回到基础，了解它如何融入类型系统。

一种非常常见的情况是，你想表示缺失或无效的值。使用图表，域看起来像这样：

int选项

显然，这需要某种形式的联合！

在 F# 中，它被称为 `Option` 类型，并被定义为有两种情况的联合类型：`Some` 和 `None`。类似的类型在函数式语言中很常见：OCaml 和 Scala 也称之为 `Option`，而 Haskell 称之为 `Maybe`。

以下是一个定义：

```F#
type Option<'a> =       // use a generic definition
   | Some of 'a           // valid value
   | None                 // missing
```

> 重要提示：如果您在交互式窗口中评估此问题，请务必在之后重置会话，以便恢复内置类型。

通过指定两种情况之一（`Some` 情况或 `None` 情况），选项类型的使用方式与构造中的任何联合类型相同：

```F#
let validInt = Some 1
let invalidInt = None
```

在模式匹配时，与任何联合类型一样，您必须始终匹配所有情况：

```F#
match validInt with
| Some x -> printfn "the valid value is %A" x
| None -> printfn "the value is None"
```

定义引用 Option 类型的类型时，必须指定要使用的泛型类型。您可以使用尖括号以显式方式执行此操作，也可以使用类型后面的内置“`option`”关键字。以下示例完全相同：

```F#
type SearchResult1 = Option<string>  // Explicit C#-style generics
type SearchResult2 = string option   // built-in postfix keyword
```

## 使用 Option 类型

选项类型在 F# 库中广泛用于可能缺失或无效的值。

例如，`List.tryFind` 函数返回一个选项，使用 `None` 表示没有任何内容与搜索谓词匹配。

```F#
[1;2;3;4]  |> List.tryFind (fun x-> x = 3)  // Some 3
[1;2;3;4]  |> List.tryFind (fun x-> x = 10) // None
```

让我们重新审视我们用于元组和记录的相同示例，看看如何使用选项：

```F#
// the tuple version of TryParse
let tryParseTuple intStr =
   try
      let i = System.Int32.Parse intStr
      (true,i)
   with _ -> (false,0)  // any exception

// for the record version, create a type to hold the return result
type TryParseResult = {success:bool; value:int}

// the record version of TryParse
let tryParseRecord intStr =
   try
      let i = System.Int32.Parse intStr
      {success=true;value=i}
   with _ -> {success=false;value=0}

// the option version of TryParse
let tryParseOption intStr =
   try
      let i = System.Int32.Parse intStr
      Some i
   with _ -> None

//test it
tryParseTuple "99"
tryParseRecord "99"
tryParseOption "99"
tryParseTuple "abc"
tryParseRecord "abc"
tryParseOption "abc"
```

在这三种方法中，“选项”版本通常是首选；不需要定义新类型，对于简单的情况，`None` 的含义从上下文中显而易见。

*注意：`tryParseOption` 代码只是一个示例。.NET 核心库内置了一个类似的函数 `tryParse`，应该使用。*

### Option 相等性

与其他联合类型一样，选项类型具有自动定义的相等操作

```F#
let o1 = Some 42
let o2 = Some 42

let areEqual = (o1=o2)
```

### Option 表示

选项类型有一个很好的默认字符串表示，与其他联合类型不同，`ToString()` 表示也很好。

```F#
let o = Some 42
printfn "%A" o   // nice
printfn "%O" o   // nice
```

### Option 不仅适用于基本类型

F# 选项是一个真正的第一类类型（毕竟它只是一个普通的联合类型）。你可以用它与任何类型。例如，你可以有一个像 Person 这样的复杂类型的选项，或者像 `int*int` 这样的元组类型，或者像 `int->bool` 这样的函数类型，甚至是一个选项类型的选项。

```F#
type OptionalString = string option
type OptionalPerson = Person option       // optional complex type
type OptionalTuple = (int*int) option
type OptionalFunc = (int -> bool) option  // optional function
type NestedOptionalString = OptionalString option //nested options!
type StrangeOption = string option option option
```

## 如何不使用 Option 类型

选项类型具有IsSome、IsNone和Value等函数，这些函数允许您在不进行模式匹配的情况下访问“包装”值。不要用它们！它不仅不地道，而且很危险，可能会导致异常。

以下是如何避免这样做：

```F#
let x = Some 99

// testing using IsSome
if x.IsSome then printfn "x is %i" x.Value   // ugly!!

// no matching at all
printfn "x is %i" x.Value   // ugly and dangerous!!
```

以下是正确操作的方法：

```F#
let x = Some 99
match x with
| Some i -> printfn "x is %i" i
| None -> () // what to do here?
```

模式匹配方法还迫使您思考并记录在 `None` 情况下会发生什么，在使用 `IsSome` 时，您可能很容易忽略这一点。

## Option 模块

如果您正在对选项进行大量的模式匹配，请查看 `Option` 模块，因为它有一些有用的辅助函数，如 `map`、`bind`、`iter` 等。

例如，假设我想将一个有效选项的值乘以 2。以下是模式匹配方式：

```F#
let x = Some 99
let result = match x with
| Some i -> Some(i * 2)
| None -> None
```

这里有一个使用 `Option.map` 编写的更紧凑的版本：

```F#
let x = Some 99
x |> Option.map (fun v -> v * 2)
```

或者，如果一个选项有效，我想将其值乘以 2，但如果它是 `None`，则返回 0。以下是模式匹配方式：

```F#
let x = Some 99
let result = match x with
| Some i -> i * 2
| None -> 0
```

这与使用 `Option.fold` 的单行程序是一样的：

```F#
let x = Some 99
x |> Option.fold (fun _ v -> v * 2) 0
```

在上述简单情况下，也可以使用 `defaultArg` 函数。

```F#
let x = Some 99
defaultArg x 0
```

## 选项 vs. 空 vs. 可空

选项类型经常给习惯于处理 C# 和其他语言中的 null 和 nullable 值的人带来困惑。本节将试图澄清差异。

### Option vs. null 的类型安全性

在 C# 或 Java 等语言中，“null”表示对不存在的对象的引用或指针。“null”的类型与对象完全相同，因此您无法从类型系统中判断出您有 null。

例如，在下面的 C# 代码中，我们创建了两个字符串变量，一个具有有效字符串，另一个具有空字符串。

```c#
string s1 = "abc";
var len1 = s1.Length;

string s2 = null;
var len2 = s2.Length;
```

当然，这很完美。编译器无法分辨这两个变量之间的区别。`null` 与有效字符串的类型完全相同，因此所有 `System.String` 方法和属性都是如此可以在其上使用，包括 `Length` 属性。

现在，我们知道，只要看一下这段代码就会失败，但编译器帮不了我们。相反，正如我们所知，你必须不断地测试 null。

现在让我们看看上面 C# 示例的最接近的 F# 等价物。在 F# 中，要表示缺少数据，可以使用选项类型并将其设置为 `None`。（在这个人为的例子中，我们必须使用一个丑陋的显式类型 `None`——通常这是不必要的。）

```F#
let s1 = "abc"
var len1 = s1.Length

// create a string option with value None
let s2 = Option<string>.None
let len2 = s2.Length
```

在 F# 版本中，我们立即收到编译时错误。`None` 不是字符串，它是一个完全不同的类型，所以你不能直接调用 `Length`。需要明确的是，`Some [string]` 也不是 `string` 的类型，所以你也不能对它调用 `Length`！

因此，如果 `Option<string>` 不是字符串，但你想对它（可能）包含的字符串做点什么，你就必须对它进行模式匹配（假设你没有像前面描述的那样做坏事）。

```F#
let s2 = Option<string>.None

//which one is it?
let len2 = match s2 with
| Some s -> s.Length
| None -> 0
```

你总是必须进行模式匹配，因为给定一个 `Option<string>` 类型的值，你无法分辨它是 Some 还是 None。

同样地，`Option<int>` 与 `int` 的类型不同，`Option<bool>` 与 `bool` 的类型也不同，以此类推。

总结关键点：

- “`string option`”类型与“`string`”类型完全不同。您不能从 `string option` 转换为 `string`——它们没有相同的属性。使用 `string` 的函数不适用于 `string option`，反之亦然。因此，类型系统将防止任何错误。
- 另一方面，C# 中的“空字符串”与“字符串”的类型完全相同。您无法在编译时区分它们，只能在运行时区分。“空字符串”似乎具有与有效字符串相同的所有属性和功能，只是当您尝试使用它时，您的代码会崩溃！

### 空值 vs. 缺失数据

C# 中使用的“null”与“缺失”数据的概念完全不同，后者是用任何语言对任何系统建模的有效部分。

在真正的函数式语言中，可能会有缺失数据的概念，但不可能有“null”这样的东西，因为在函数式思维中不存在“指针”或“未初始化变量”的概念。

例如，考虑一个绑定到表达式结果的值，如下所示：

```F#
let x = "hello world"
```

这个值怎么可能被取消初始化，或者变成 null，甚至变成任何其他值？

不幸的是，由于在某些情况下 API 设计者也使用了 null 来表示“丢失”数据的概念，因此造成了额外的混乱！例如 .NET 库方法 `StreamReader.ReadLine` 返回 null 表示文件中没有更多数据。

### F# 和 null

F# 不是一种纯粹的函数式语言，必须和确实有 null 的概念的 .NET 语言进行交互。因此，F# 在其设计中确实包含了一个 null 关键字，但这使得它很难使用，并将其视为异常值。

一般来说，空值永远不会在“纯” F# 中创建，而只能通过和 .NET 库或其他外部系统交互来创建。

以下是一些示例：

```F#
// pure F# type is not allowed to be null (in general)
type Person = {first:string; last:string}
let p : Person = null                      // error!

// type defined in CLR, so is allowed to be null
let s : string = null                      // no error!
let line = streamReader.ReadLine()         // no error if null
```

在这些情况下，最好立即检查 null 并将其转换为选项类型！

```F#
// streamReader example
let line = match streamReader.ReadLine()  with
           | null -> None
           | line -> Some line

// environment example
let GetEnvVar var =
    match System.Environment.GetEnvironmentVariable(var) with
    | null -> None
    | value -> Some value

// try it
GetEnvVar "PATH"
GetEnvVar "TEST"
```

有时，您可能需要将 null 传递给外部库。您也可以使用 `null` 关键字来完成此操作。

### 选项 vs. 可空

除了 null 之外，C# 还具有 Nullable 类型的概念，例如 `Nullable<int>`，这似乎与选项类型相似。那么，有什么区别呢？

基本思想是一样的，但 Nullable 要弱得多。它只适用于 `Int` 和 `DateTime` 等值类型，不适用于字符串、类或函数等引用类型。你不能嵌套 Nullables，它们也没有太多特殊的行为。

另一方面，F# 选项是一个真正的第一类类型，可以以相同的方式在所有类型中一致使用。（请参阅“选项不仅仅适用于基本类型”一节中的上述示例。）

# 8 枚举类型

*Part of the "Understanding F# types" series (*[link](https://fsharpforfunandprofit.com/posts/enum-types/#series-toc)*)*

与联合类型不同
09七月2012 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/enum-types/

F# 的枚举类型与 C# 的枚举类型相同。它的定义表面上就像联合类型，但有许多不明显的差异需要注意。

## 定义枚举

要定义枚举，您可以使用与具有空大小写的联合类型完全相同的语法，除了您必须为每个大小写指定一个常量值，并且常量必须都是相同的类型。

```F#
type SizeUnion = Small | Medium | Large         // union
type ColorEnum = Red=0 | Yellow=1 | Blue=2      // enum
```

不允许使用字符串，只允许使用整数或兼容类型，如字节和字符：

```F#
type MyEnum = Yes = "Y" | No ="N"  // Error. Strings not allowed.
type MyEnum = Yes = 'Y' | No ='N'  // Ok because char was used.
```

联合类型要求其大小写以大写字母开头。枚举不需要此项。

```F#
type SizeUnion = Small | Medium | large      // Error - "large" is invalid.
type ColorEnum = Red=0 | Yellow=1 | blue=2      // Ok
```

与 C# 一样，您可以将 FlagsAttribute 用于位标志：

```F#
[<System.FlagsAttribute>]
type PermissionFlags = Read = 1 | Write = 2 | Execute = 4
let permission = PermissionFlags.Read ||| PermissionFlags.Write
```

## 构建枚举

与联合类型不同，要构造枚举，必须始终使用限定名：

```F#
let red = Red            // Error. Enums must be qualified
let red = ColorEnum.Red  // Ok
let small = Small        // Ok.  Unions do not need to be qualified
```

您还可以转换到底层 int 类型或从底层 int 类型转换：

```F#
let redInt = int ColorEnum.Red
let redAgain:ColorEnum = enum redInt // cast to a specified enum type
let yellowAgain = enum<ColorEnum>(1) // or create directly
```

您甚至可以创建根本不在枚举列表上的值。

```F#
let unknownColor = enum<ColorEnum>(99)   // valid
```

而且，与联合不同，您可以使用 BCL 枚举函数来枚举和解析值，就像使用 C# 一样。例如：

```F#
let values = System.Enum.GetValues(typeof<ColorEnum>)
let redFromString =
    System.Enum.Parse(typeof<ColorEnum>,"Red")
    :?> ColorEnum  // downcast needed
```

## 匹配枚举

要匹配枚举，您必须始终使用限定名：

```F#
let unqualifiedMatch x =
    match x with
    | Red -> printfn "red"             // warning FS0049
    | _ -> printfn "something else"

let qualifiedMatch x =
    match x with
    | ColorEnum.Red -> printfn "red"   //OK. qualified name used.
    | _ -> printfn "something else"
```

如果在模式匹配时没有涵盖所有已知情况，联合和枚举都会发出警告：

```F#
let matchUnionIncomplete x =
    match x with
    | Small -> printfn "small"
    | Medium -> printfn "medium"
    // Warning: Incomplete pattern matches

let matchEnumIncomplete x =
    match x with
    | ColorEnum.Red -> printfn "red"
    | ColorEnum.Yellow -> printfn "yellow"
    // Warning: Incomplete pattern matches
```

联合和枚举之间的一个重要区别是，通过列出所有联合类型，你能让编译器对穷举模式匹配感到满意吗。

对于枚举来说不是这样。可以创建一个不在预定义列表上的枚举，并尝试与之匹配，然后得到一个运行时异常，因此即使您显式列出了所有已知的枚举，编译器也会警告您：

```F#
// the compiler is still not happy
let matchEnumIncomplete2 x =
    match x with
    | ColorEnum.Red -> printfn "red"
    | ColorEnum.Yellow -> printfn "yellow"
    | ColorEnum.Blue -> printfn "blue"
    // the value '3' may indicate a case not covered by the pattern(s).
```

解决此问题的唯一方法是在案例底部添加通配符，以处理预定义范围之外的枚举。

```F#
// the compiler is finally happy
let matchEnumComplete x =
    match x with
    | ColorEnum.Red -> printfn "red"
    | ColorEnum.Yellow -> printfn "yellow"
    | ColorEnum.Blue -> printfn "blue"
    | _ -> printfn "something else"

// test with unknown case
let unknownColor = enum<ColorEnum>(99)   // valid
matchEnumComplete unknownColor
```

## 摘要

一般来说，你应该更喜欢有区别的联合类型而不是枚举，除非你真的需要与它们关联一个 int 值，或者你正在编写需要暴露给其他 .NET 语言的类型。

# 9 内置 .NET 类型

*Part of the "Understanding F# types" series (*[link](https://fsharpforfunandprofit.com/posts/cli-types/#series-toc)*)*

Int、string、bool 等
2012年7月10日 这篇文章已有3年多的历史了

https://fsharpforfunandprofit.com/posts/cli-types/

在这篇文章中，我们将快速了解 F# 如何处理 .NET 内置的标准类型。

## 直接常量

F# 对文字使用与 C# 相同的语法，但有一些例外。

我将把内置类型分为以下几组：

- 其他类型（`bool`、`char`等）
- 字符串类型
- 整数类型（`int`、`uint` 和 `byte` 等）
- 浮点数类型（`float`、`decimal` 等）
- 指针类型（`IntPtr` 等）

下表列出了基本类型及其 F# 关键字、后缀（如果有的话）、示例和 .NET CLR 类型相应内容。

### 其他类型

|           | 对象          | Unit            | Bool       | Char (Unicode) | Char (Ascii) |
| :-------- | :------------ | :-------------- | :--------- | :------------- | :----------- |
| 关键词    | obj           | unit            | bool       | char           | byte         |
| 后缀      |               |                 |            |                | B            |
| 例子      | let o = obj() | let u = ()      | true false | 'a'            | 'a'B         |
| .NET 类型 | Object        | (no equivalent) | Boolean    | Char           | Byte         |

对象和单位并不是真的 .NET 基本类型，但为了完整起见，我已经包含了它们。

### 字符串类型

|           | String (Unicode)     | 逐字 string (Unicode) | 三引号 string (Unicode)            | String (Ascii) |
| :-------- | :------------------- | :-------------------- | :--------------------------------- | :------------- |
| 关键词    | string               | string                | string                             | byte[]         |
| 后缀      |                      |                       |                                    |                |
| 例子      | "first\nsecond line" | @"C:\name"            | """can "contain"" special chars""" | "aaa"B         |
| .NET 类型 | String               | String                | String                             | Byte[]         |

普通字符串中可以使用常见的特殊字符，如 `\n`、`\t`、`\\` 等。引号必须用反斜杠转义：`\'` 和 `\"`。

在逐字字符串中，反斜杠被忽略（适用于 Windows 文件名和正则表达式模式）。但引号需要是双引号。

三引号字符串在 VS2012 中是新的。它们很有用，因为特殊字符根本不需要转义，所以它们可以很好地处理嵌入的引号（非常适合 XML）。

### 整数类型

|           | 8 bit (Signed) | 8 bit (Unsigned) | 16 bit (Signed) | 16 bit (Unsigned) | 32 bit (Signed) | 32 bit (Unsigned) | 64 bit (Signed) | 64 bit (Unsigned) | Unlimited precision |
| :-------- | :------------- | :--------------- | :-------------- | :---------------- | :-------------- | :---------------- | :-------------- | :---------------- | :------------------ |
| 关键词    | sbyte          | byte             | int16           | uint16            | int             | uint32            | int64           | uint64            | bigint              |
| 后缀      | y              | uy               | s               | us                |                 | u                 | L               | UL                | I                   |
| 例子      | 99y            | 99uy             | 99s             | 99us              | 99              | 99u               | 99L             | 99UL              | 99I                 |
| .NET 类型 | SByte          | Byte             | Int16           | UInt16            | Int32           | UInt32            | Int64           | UInt64            | BigInteger          |

`BigInteger` 在所有版本的 F# 中都可用。从 .NET 4 中它就作为 .NET 基础库的部分被包含。

整数类型也可以用十六进制和八进制表示。

- 十六进制前缀为 `0x`。所以 `0xFF` 是 255 的十六进制。
- 八进制前缀为 `0o`。所以 `0o377` 是 255 的八进制数。

### 浮点类型

|           | 32 bit 浮点数   | 64 bit (默认) 浮点数 | 高精度浮点数 |
| :-------- | :-------------- | :------------------- | :----------- |
| 关键词    | float32, single | float, double        | decimal      |
| 后缀      | f               |                      | m            |
| 例子      | 123.456f        | 123.456              | 123.456m     |
| .NET 类型 | Single          | Double               | Decimal      |

请注意，F# 本机使用 `float` 而不是 `double`，但两者都可以使用。

### 指针类型

|           | 指针/句柄 (signed) | 指针/句柄 (unsigned) |
| :-------- | :----------------- | :------------------- |
| 关键词    | nativeint          | unativeint           |
| 后缀      | n                  | un                   |
| 例子      | 0xFFFFFFFFn        | 0xFFFFFFFFun         |
| .NET 类型 | IntPtr             | UIntPtr              |

## 在内置基本类型之间进行转换

*注意：本节仅介绍基本类型的强制转换。关于类之间的转换，请参阅面向对象编程系列。*

F# 中没有直接的“强制转换”语法，但有辅助函数可以在类型之间强制转换。这些辅助函数与类型同名（您可以在 `Microsoft.FSharp.Core` 命名空间中看到它们）。

例如，在 C# 中，你可以写：

```c#
var x = (int)1.23
var y = (double)1
```

在 F# 中，等价物为：

```F#
let x = int 1.23
let y = float 1
```

在 F# 中，只有数值类型的转换函数。特别是，bool 没有强制转换，您必须使用 `Convert` 或类似方法。

```F#
let x = bool 1  //error
let y = System.Convert.ToBoolean(1)  // ok
```

### 装箱和拆箱

就像 C# 和其他 .NET 语言中一样，基本的 int 和 float 类型是值对象，而不是类。虽然这通常是透明的，但在某些情况下可能会成为问题。

首先，让我们看看透明的情况。在下面的示例中，我们定义了一个函数，该函数接受 `Object` 类型的参数，并简单地返回它。如果我们传入一个 `int`，它会被静默地打包成一个对象，从测试代码中可以看出，它返回的是一个 `object` 而不是 `int`。

```F#
// create a function with parameter of type Object
let objFunction (o:obj) = o

// test: call with an integer
let result = objFunction 1

// result is
// val result : obj = 1
```

如果你不小心，`result` 是一个对象而不是 int 的事实可能会导致类型错误。例如，结果不能直接与原始值进行比较：

```F#
let resultIsOne = (result = 1)
// error FS0001: This expression was expected to have type obj
// but here has type int
```

要处理这种情况和其他类似情况，您可以使用 `box` 关键字直接将基元类型转换为对象：

```F#
let o = box 1

// retest the comparison example above, but with boxing
let result = objFunction 1
let resultIsOne = (result = box 1)  // OK
```

要将对象转换回基元类型，请使用 `unbox` 关键字，但与 `box` 不同，您必须提供一个特定的类型进行 unbox，或者确保编译器有足够的信息进行准确的类型推断。

```F#
// box an int
let o = box 1

// type known for target value
let i:int = unbox o  // OK

// explicit type given in unbox
let j = unbox<int> o  // OK

// type inference, so no type annotation needed
let k = 1 + unbox o  // OK
```

因此，上面的比较示例也可以用 `unbox` 来完成。不需要显式类型注释，因为它正在与 int 进行比较。

```F#
let result = objFunction 1
let resultIsOne = (unbox result = 1)  // OK
```

如果您没有指定足够的类型信息，就会出现一个常见的问题——您将遇到臭名昭著的“值限制”错误，如下所示：

```F#
let o = box 1

// no type specified
let i = unbox o  // FS0030: Value restriction error
```

解决方案是重新排序代码以帮助类型推理，或者在所有其他方法都失败时，添加显式的类型注释。有关更多提示，请参阅关于类型推理的帖子。

### 装箱与类型检测相结合

假设你想有一个基于参数类型匹配的函数，使用 `:?` 操作符：

```F#
let detectType v =
    match v with
        | :? int -> printfn "this is an int"
        | _ -> printfn "something else"
```

不幸的是，此代码将无法编译，并出现以下错误：

```F#
// error FS0008: This runtime coercion or type test from type 'a to int
// involves an indeterminate type based on information prior to this program point.
// Runtime type tests are not allowed on some types. Further type annotations are needed.
```

消息告诉您问题：“不允许对某些类型进行运行时类型测试”。

答案是将强制其进入引用类型的值“装箱”起来，然后您可以对其进行类型检查：

```F#
let detectTypeBoxed v =
    match box v with      // used "box v"
        | :? int -> printfn "this is an int"
        | _ -> printfn "something else"

//test
detectTypeBoxed 1
detectTypeBoxed 3.14
```

# 10 计量单位

*Part of the "Understanding F# types" series (*[link](https://fsharpforfunandprofit.com/posts/units-of-measure/#series-toc)*)*

数字的类型安全
2012年7月10日 这篇文章已有3年多的历史了

https://fsharpforfunandprofit.com/posts/units-of-measure/

正如我们在前面的“为什么使用 F#？”系列中提到的，F# 有一个非常酷的功能，它允许您将额外的度量单位信息作为元数据添加到数值类型中。

F#编译器将确保只有具有相同度量单位的数字才能组合。这对于阻止意外不匹配和使代码更安全非常有用。

## 定义度量单位

度量单位定义由特性 `[<measure>]`、`type` 关键字和名称组成。例如：

```F#
[<Measure>]
type cm

[<Measure>]
type inch
```

通常你会看到整个定义写在一行上：

```F#
[<Measure>] type cm
[<Measure>] type inch
```

一旦你有了定义，你就可以使用尖括号将度量值类型与数字类型相关联，括号内有度量值名称：

```F#
let x = 1<cm>    // int
let y = 1.0<cm>  // float
let z = 1.0m<cm> // decimal
```

您甚至可以在尖括号内组合度量值以创建复合度量值：

```F#
[<Measure>] type m
[<Measure>] type sec
[<Measure>] type kg

let distance = 1.0<m>
let time = 2.0<sec>
let speed = 2.0<m/sec>
let acceleration = 2.0<m/sec^2>
let force = 5.0<kg m/sec^2>
```

### 衍生计量单位

如果你经常使用某些单位组合，你可以定义一个派生度量并使用它。

```F#
[<Measure>] type N = kg m/sec^2

let force1 = 5.0<kg m/sec^2>
let force2 = 5.0<N>

force1 = force2 // true
```

### 国际单位制和常数

如果你在物理或其他科学应用中使用测量单位，你肯定会想要使用国际单位制和相关常数。你不需要自己定义所有这些！这些是为您预定义的，如下所示：

- 在 F# 3.0 及更高版本（随 Visual Studio 2012 附带）中，这些已内置到核心 F# 库中 `Microsoft.FSharp.Data.UnitSystems.SI` 命名空间（请参阅 MSDN 页面）。
- 在 F# 2.0（随 Visual Studio 2010 一起提供）中，您必须安装 F# powerpack 才能获得它们。（F# 电源包在 Codeplex 上 http://fsharppowerpack.codeplex.com).

## 类型检查和类型推断

计量单位就像适当的类型；您将获得静态检查和类型推理。

```F#
[<Measure>] type foot
[<Measure>] type inch

let distance = 3.0<foot>

// type inference for result
let distance2 = distance * 2.0

// type inference for input and output
let addThreeFeet ft =
    ft + 3.0<foot>
```

当然，在使用它们时，类型检查是严格的：

```F#
addThreeFeet 1.0        //error
addThreeFeet 1.0<inch>  //error
addThreeFeet 1.0<foot>  //OK
```

### 类型注解

如果您想明确指定度量单位类型的注释，可以按照通常的方式进行。数字类型必须带有带度量单位的尖括号。

```F#
let untypedTimesThree (ft:float) =
    ft * 3.0

let footTimesThree (ft:float<foot>) =
    ft * 3.0
```

### 将度量单位与乘法和除法相结合

编译器理解当单个值被相乘或除法时，度量单位是如何转换的。例如，在下文中，`speed` 值被自动赋予 `<m/sec>` 的度量值。

```F#
[<Measure>] type m
[<Measure>] type sec
[<Measure>] type kg

let distance = 1.0<m>
let time = 2.0<sec>
let speed = distance/time
let acceleration = speed/time
let mass = 5.0<kg>
let force = mass * speed/time
```

看看上面的 `acceleration` 和 `force` 值的类型，看看这是如何工作的其他例子。

### 无量纲值

没有任何特定度量单位的数值称为无量纲。如果你想明确一个值是无量纲的，你可以使用名为 `1` 的度量。

```F#
// dimensionless
let x = 42

// also dimensionless
let x = 42<1>
```

### 混合无量纲值的计量单位

请注意，您不能将无量纲值添加到具有度量单位的值中，但可以乘以或除以无量纲值。

```F#
// test addition
3.0<foot> + 2.0<foot>  // OK
3.0<foot> + 2.0        // error

// test multiplication
3.0<foot> * 2.0        // OK
```

但是，请参阅下面关于“泛型”的部分，了解另一种方法。

## 单位之间的转换

如果你需要在单位之间转换怎么办？

这很简单。您首先需要定义一个使用这两个单位的转换值，然后将源值乘以转换因子。

这里有一个英尺和英寸的例子：

```F#
[<Measure>] type foot
[<Measure>] type inch

//conversion factor
let inchesPerFoot = 12.0<inch/foot>

// test
let distanceInFeet = 3.0<foot>
let distanceInInches = distanceInFeet * inchesPerFoot
```

这里有一个温度的例子：

```F#
[<Measure>] type degC
[<Measure>] type degF

let convertDegCToF c =
    c * 1.8<degF/degC> + 32.0<degF>

// test
let f = convertDegCToF 0.0<degC>
```

编译器正确推断出转换函数的签名。

```F#
val convertDegCToF : float<degC> -> float<degF>
```

请注意，常数 `32.0<degF>` 已明确标注为 `degF`，因此结果也将以 `degF` 为单位。如果省略此注释，结果将是一个普通的浮点数，函数签名将更改为更奇怪的东西！试试看：

```F#
let badConvertDegCToF c =
    c * 1.8<degF/degC> + 32.0
```

### 无量纲值和测量单位值之间的转换

要将无量纲数值转换为具有度量类型的值，只需将其乘以 1，但其中一个值用适当的单位注释。

```F#
[<Measure>] type foot

let ten = 10.0   // normal

//converting from non-measure to measure
let tenFeet = ten * 1.0<foot>  // with measure
```

要转换为另一种方式，要么除以1，要么乘以逆单位。

```F#
//converting from measure to non-measure
let tenAgain = tenFeet / 1.0<foot>  // without measure
let tenAnotherWay = tenFeet * 1.0<1/foot>  // without measure
```

上述方法是类型安全的，如果您尝试转换错误的类型，将导致错误。

如果你不关心类型检查，你可以用标准的强制转换函数来进行转换：

```F#
let tenFeet = 10.0<foot>  // with measure
let tenDimensionless = float tenFeet // without measure
```

## 通用计量单位

通常，我们希望编写能够处理任何值的函数，无论与之关联的度量单位是什么。

例如，这里是我们的老朋友 `square`。但是当我们试图将其与度量单位一起使用时，我们会得到一个错误。

```F#
let square x = x * x

// test
square 10<foot>   // error
```

我们能做什么？我们不想指定特定的度量单位，但另一方面，我们必须指定一些东西，因为上面的简单定义不起作用。

答案是使用通用度量单位，在度量名称通常所在的位置用下划线表示。

```F#
let square (x:int<_>) = x * x

// test
square 10<foot>   // OK
square 10<sec>   // OK
```

现在，`square` 函数按预期工作，您可以看到函数签名使用了字母 `'u` 来表示通用度量单位。还要注意，编译器已经推断出返回值的类型为“unit 平方”。

```F#
val square : int<'u> -> int<'u ^ 2>
```

事实上，如果你愿意，你也可以使用字母指定泛型类型：

```F#
// with underscores
let square (x:int<_>) = x * x

// with letters
let square (x:int<'u>) = x * x

// with underscores
let speed (distance:float<_>) (time:float<_>) =
    distance / time

// with letters
let speed (distance:float<'u>) (time:float<'v>) =
    distance / time
```

有时您可能需要使用字母来明确表示单位是相同的：

```F#
let ratio (distance1:float<'u>) (distance2:float<'u>) =
    distance1 / distance2
```

### 使用带有列表的通用度量

你不能总是直接使用度量。例如，您不能直接定义脚列表：

```F#
//error
[1.0<foot>..10.0<foot>]
```

相反，你必须使用上面提到的“乘一”技巧：

```F#
//converting using map -- OK
[1.0..10.0] |> List.map (fun i-> i * 1.0<foot>)

//using a generator -- OK
[ for i in [1.0..10.0] -> i * 1.0<foot> ]
```

### 对常数使用通用度量

常数乘法是可以的（如上所述），但如果你尝试加法，你会得到一个错误。

```F#
let x = 10<foot> + 1  // error
```

修复方法是向常量添加泛型类型，如下所示：

```F#
let x = 10<foot> + 1<_>  // ok
```

当将常数传递给更高阶函数（如 `fold`）时，也会出现类似的情况。

```F#
let feet = [ for i in [1.0..10.0] -> i * 1.0<foot> ]

// OK
feet |> List.sum

// Error
feet |> List.fold (+) 0.0

// Fixed with generic 0
feet |> List.fold (+) 0.0<_>
```

### 函数的泛型度量的问题

在某些情况下，类型推理会失败。例如，让我们尝试创建一个使用单位的简单 `add1` 函数。

```F#
// try to define a generic function
let add1 n = n + 1.0<_>
// warning FS0064: This construct causes code to be less generic than
// indicated by the type annotations. The unit-of-measure variable 'u
// has been constrained to be measure '1'.

// test
add1 10.0<foot>
// error FS0001: This expression was expected to have type float
// but here has type float<foot>
```

警告信息有线索。输入参数 `n` 没有度量值，因此 `1<_>` 的度量值将始终被忽略。`add1` 函数没有度量单位，所以当你试图用一个有度量的值调用它时，你会得到一个错误。

因此，也许解决方案是显式地注释度量值类型，如下所示：

```F#
// define a function with explicit type annotation
let add1 (n:float<'u>) : float<'u> =  n + 1.0<_>
```

但是没有，你会再次收到同样的警告 FS0064。

也许我们可以用更明确的东西替换下划线，比如 `1.0<'u>`？

```F#
let add1 (n:float<'u>) : float<'u> = n + 1.0<'u>
// error FS0634: Non-zero constants cannot have generic units.
```

但这次我们遇到了编译器错误！

答案是使用 LanguagePrimitives 模块中一个有用的实用函数：`FloatWithMeasure`、`Int32WithMeasure` 等。

```F#
// define the function
let add1 n  =
    n + (LanguagePrimitives.FloatWithMeasure 1.0)

// test
add1 10.0<foot>   // Yes!
```

对于泛型 int，您可以使用相同的方法：

```F#
open LanguagePrimitives

let add2Int n  =
    n + (Int32WithMeasure 2)

add2Int 10<foot>   // OK
```

### 使用具有类型定义的泛型度量

这照顾到了功能。当我们需要在类型定义中使用度量单位时呢？

假设我们想定义一个使用度量单位的通用坐标记录。让我们从一个天真的方法开始：

```F#
type Coord =
    { X: float<'u>; Y: float<'u>; }
// error FS0039: The type parameter 'u' is not defined
```

这不起作用，那么将度量值添加为类型参数呢：

```F#
type Coord<'u> =
    { X: float<'u>; Y: float<'u>; }
// error FS0702: Expected unit-of-measure parameter, not type parameter.
// Explicit unit-of-measure parameters must be marked with the [<Measure>] attribute.
```

这也不起作用，但错误消息告诉我们该怎么做。下面是使用 `Measure` 属性的最终正确版本：

```F#
type Coord<[<Measure>] 'u> =
    { X: float<'u>; Y: float<'u>; }

// Test
let coord = {X=10.0<foot>; Y=2.0<foot>}
```

在某些情况下，您可能需要定义多个度量值。在以下示例中，货币汇率被定义为两种货币的比率，因此需要定义两个通用度量。

```F#
type CurrencyRate<[<Measure>]'u, [<Measure>]'v> =
    { Rate: float<'u/'v>; Date: System.DateTime}

// test
[<Measure>] type EUR
[<Measure>] type USD
[<Measure>] type GBP

let mar1 = System.DateTime(2012,3,1)
let eurToUsdOnMar1 = {Rate= 1.2<USD/EUR>; Date=mar1 }
let eurToGbpOnMar1 = {Rate= 0.8<GBP/EUR>; Date=mar1 }

let tenEur = 10.0<EUR>
let tenEurInUsd = eurToUsdOnMar1.Rate * tenEur
```

当然，您可以将常规泛型类型与度量单位类型混合使用。

例如，产品价格可能由通用产品类型和货币价格组成：

```F#
type ProductPrice<'product, [<Measure>] 'currency> =
    { Product: 'product; Price: float<'currency>; }
```

### 运行时的计量单位

您可能会遇到的一个问题是，度量单位不是 .NET 类型系统的一部分。

F# 确实在程序集中存储了关于它们的额外元数据，但只有 F# 才能理解这些元数据。

这意味着在运行时没有（简单）的方法来确定一个值具有什么度量单位，也没有任何方法在运行时动态分配度量单位。

这也意味着没有办法将度量单位作为公共 API 的一部分公开给另一个 .NET 语言（其他 F# 程序集除外）。

# 11 理解类型推理

*Part of the "Understanding F# types" series (*[link](https://fsharpforfunandprofit.com/posts/type-inference/#series-toc)*)*

魔法帘子后面
09七月2012 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/type-inference/

在我们结束类型之前，让我们重新审视类型推理：让 F# 编译器推断出使用了哪些类型以及在哪里使用的魔法。到目前为止，我们已经通过所有的例子看到了这一点，但它是如何工作的，如果出了问题，你能做什么？

## 类型推理是如何工作的？

这似乎很神奇，但规则大多很简单。基本逻辑基于一种算法，通常称为“Hindley-Milner”或“HM”（更准确地说，它应该被称为“Damas-Milner 算法 W”）。如果你想知道细节，去谷歌吧。

我建议你花点时间了解这个算法，这样你就可以“像编译器一样思考”，并在需要时有效地进行故障排除。

以下是确定简单值和函数值类型的一些规则：

- 看看文字
- 看看与之交互的函数和其他值
- 查看任何显式类型约束
- 如果任何地方都没有约束，则自动推广到泛型类型

让我们依次看看这些。

### 看看字面量

字面量为编译器提供了上下文的线索。正如我们所看到的，类型检查非常严格；int 和 float 不会自动转换为另一个。这样做的好处是编译器可以通过查看字面值来推断类型。如果字面量是一个 `int`，并且你向它添加了“x”，那么“x”也必须是 int。但是，如果字面量是一个 `float`，并且你向它添加了“x”，那么“x”也必须是浮点数。

这里有一些例子。运行它们并在交互窗口中查看它们的签名：

```F#
let inferInt x = x + 1
let inferFloat x = x + 1.0
let inferDecimal x = x + 1m     // m suffix means decimal
let inferSByte x = x + 1y       // y suffix means signed byte
let inferChar x = x + 'a'       // a char
let inferString x = x + "my string"
```

### 查看它与之交互的函数和其他值

如果任何地方都没有文字，编译器会尝试通过分析函数和它们交互的其他值来计算类型。在下面的情况下，“`indirect`”函数调用了一个我们知道其类型的函数，这为我们提供了推断“`indirect`”功能本身类型的信息。

```F#
let inferInt x = x + 1
let inferIndirectInt x = inferInt x       //deduce that x is an int

let inferFloat x = x + 1.0
let inferIndirectFloat x = inferFloat x   //deduce that x is a float
```

当然，赋值也是一种互动。如果 x 是某个类型，并且 y 被绑定（分配）到 x，那么 y 必须与 x 是同一类型。

```F#
let x = 1
let y = x     //deduce that y is also an int
```

其他交互可能是控制结构或外部库

```F#
// if..else implies a bool
let inferBool x = if x then false else true
// for..do implies a sequence
let inferStringList x = for y in x do printfn "%s" y
// :: implies a list
let inferIntList x = 99::x
// .NET library method is strongly typed
let inferStringAndBool x = System.String.IsNullOrEmpty(x)
```

### 查看任何显式类型约束或注释

如果指定了任何显式类型约束或注释，则编译器将使用它们。在下面的例子中，我们明确地告诉编译器“`inferent2`”接受一个 `int` 参数。然后，它可以推断出“`infectInt2`”的返回值也是一个 `int`，这反过来又意味着“`infectIndirectInt2`”是 int->int 类型。

```F#
let inferInt2 (x:int) = x
let inferIndirectInt2 x = inferInt2 x

let inferFloat2 (x:float) = x
let inferIndirectFloat2 x = inferFloat2 x
```

请注意，`printf` 语句中的格式化代码也算作显式类型约束！

```F#
let inferIntPrint x = printf "x is %i" x
let inferFloatPrint x = printf "x is %f" x
let inferGenericPrint x = printf "x is %A" x
```

### 自动泛化

如果经过这一切，没有找到约束，编译器只会使类型为泛型。

```F#
let inferGeneric x = x
let inferIndirectGeneric x = inferGeneric x
let inferIndirectGenericAgain x = (inferIndirectGeneric x).ToString()
```

### 它在各个方向都有效！

类型推理自上而下、自下而上、前后、后前、内外，任何有类型信息的地方都会被使用。

考虑以下示例。内部函数有一个文字，所以我们知道它返回一个 `int`。外部函数被明确地告知它返回 `string`。但是中间传递的“`action`”函数的类型是什么？

```F#
let outerFn action : string =
   let innerFn x = x + 1 // define a sub fn that returns an int
   action (innerFn 2)    // result of applying action to innerFn
```

类型推断的工作原理如下：

- `1` 是一个 `int`
- 因此 `x+1` 必须是 `int`，因此 `x` 必须是 `int`
- 因此 `innerFn` 必须是 `int->int`
- 接下来，`(innerFn 2)` 返回一个 `int`，因此“`action`”将 `int` 作为输入。
- `action` 的输出是 `outerFn` 的返回值，因此 `action` 的输出类型与 `outerFn` 相同。
- `outerFn` 的输出类型已明确约束为字符串，因此动作的输出类型也是字符串。
  综上所述，我们现在知道 `action` 函数的签名是 `int->string`
  因此，最后，编译器推断 `outerFn` 的类型为：

```F#
val outerFn: (int -> string) -> string
```

### 基本演绎法，我亲爱的华生！

编译器可以做出与夏洛克·福尔摩斯相当的推论。这里有一个棘手的例子，可以测试到目前为止你对所有事情的理解程度。

假设我们有一个 `doItTwice` 函数，它接受任何输入函数（称为“`f`”）并生成一个新函数，该函数只需连续执行原始函数两次。以下是它的代码：

```F#
let doItTwice f  = (f >> f)
```

正如你所看到的，它自己组成 `f`。换言之，它的意思是：“执行 f”，然后根据结果执行 f。

现在，编译器可能推断出 `doItTwice` 的签名是什么？

好吧，让我们先看看“`f`”的签名。第一次调用“`f`”的输出也是第二次调用“f”的输入。因此，“`f`”的输出和输入必须是同一类型。因此，`f` 的签名必须是 `'a->'a`。该类型是泛型的（写成 'a），因为我们没有关于它的其他信息。

所以回到 `doItTwice` 本身，我们现在知道它需要一个函数参数 `'a->'a`。但它返回什么？好吧，这是我们如何一步一步地推断出来的：

- 首先，请注意 `doItTwice` 会生成一个函数，因此必须返回一个函数类型。
- 生成函数的输入与第一次调用“`f`”的输入类型相同
- 生成的函数的输出与第二次调用“`f`”的输出类型相同
- 因此，生成的函数也必须具有类型 `'a->'a`
- 综上所述，`doItTwice` 的域为 `'a->'a`，范围为 `'a->'a`，因此它的签名必须是 `('a->'a) -> ('a->'a)`。

你头晕了吗？你可能想再读一遍，直到它沉入脑海（sinks in）。

一行代码的推导相当复杂。幸运的是，编译器为我们完成了所有这些。但如果你有问题，你需要了解这类事情，并且必须确定编译器在做什么。

让我们来测试一下！实际上，在实践中比在理论上更容易理解。

```F#
let doItTwice f  = (f >> f)

let add3 x = x + 3
let add6 = doItTwice add3
// test
add6 5             // result = 11

let square x = x * x
let fourthPower = doItTwice square
// test
fourthPower 3      // result = 81

let chittyBang x = "Chitty " + x + " Bang"
let chittyChittyBangBang = doItTwice chittyBang
// test
chittyChittyBangBang "&"      // result = "Chitty Chitty & Bang Bang"
```

希望现在这更有意义。

## 类型推理可能出错的地方

唉，类型推理并不完美。有时编译器根本不知道该怎么做。同样，理解正在发生的事情真的会帮助你保持冷静，而不是想杀死编译器。以下是类型错误的一些主要原因：

- 声明不符合规定
- 信息不足
- 重载方法
- 泛型数值函数的问题

### 声明不符合规定

一个基本规则是，在使用函数之前必须声明它们。

此代码失败：

```F#
let square2 x = square x   // fails: square not defined
let square x = x * x
```

但这没关系：

```F#
let square x = x * x
let square2 x = square x   // square already defined earlier
```

与 C# 不同，在 F# 中，文件编译的顺序很重要，因此请确保文件以正确的顺序编译。（在 Visual Studio 中，可以从上下文菜单更改顺序）。

### 递归或同时声明

“乱序”问题的一个变体出现在必须相互引用的递归函数或定义中。在这种情况下，再多的重新排序也无济于事——我们需要使用额外的关键字来帮助编译器。

在编译函数时，函数标识符对主体不可用。因此，如果你定义了一个简单的递归函数，你会得到一个编译器错误。修复方法是在函数定义中添加“rec”关键字。例如：

```F#
// the compiler does not know what "fib" means
let fib n =
   if n <= 2 then 1
   else fib (n - 1) + fib (n - 2)
   // error FS0039: The value or constructor 'fib' is not defined
```

这是添加了“rec-fib”的固定版本，表示它是递归的：

```F#
let rec fib n =              // LET REC rather than LET
   if n <= 2 then 1
   else fib (n - 1) + fib (n - 2)
```

类似的“`let rec ... and`”语法用于相互引用的两个函数。这是一个非常人为的例子，如果你没有“`rec`”关键字，它就会失败。

```F#
let rec showPositiveNumber x =               // LET REC rather than LET
   match x with
   | x when x >= 0 -> printfn "%i is positive" x
   | _ -> showNegativeNumber x

and showNegativeNumber x =                   // AND rather than LET

   match x with
   | x when x < 0 -> printfn "%i is negative" x
   | _ -> showPositiveNumber x
```

关键字“`and`”也可以用类似的方式声明并发类型。

```F#
type A = None | AUsesB of B
   // error FS0039: The type 'B' is not defined
type B = None | BUsesA of A
```

固定版本：

```F#
type A = None | AUsesB of B
and B = None | BUsesA of A    // use AND instead of TYPE
```

### 信息不足

有时，编译器没有足够的信息来确定类型。在下面的例子中，编译器不知道 `Length` 方法应该处理什么类型。但它也不能使其通用，所以它会抱怨。

```F#
let stringLength s = s.Length
  // error FS0072: Lookup on object of indeterminate type
  // based on information prior to this program point.
  // A type annotation may be needed ...
```

这些类型的错误可以通过显式注释来修复。

```F#
let stringLength (s:string) = s.Length
```

偶尔会出现足够的信息，但编译器似乎仍然无法识别它。例如，对于人类来说，`List.map` 函数（如下）显然被应用于字符串列表，那么为什么 `x.Length` 会导致错误呢？

```F#
List.map (fun x -> x.Length) ["hello"; "world"]       //not ok
```

原因是 F# 编译器目前是一个单程编译器，因此如果程序中的后续信息尚未解析，则会被忽略。（F# 团队表示，可以使编译器更复杂，但它在 Intellisense 上的工作效果较差，可能会产生更不友好和模糊的错误消息。因此，目前我们将不得不接受这一限制。）

因此，在这种情况下，您始终可以明确地注释：

```F#
List.map (fun (x:string) -> x.Length) ["hello"; "world"]       // ok
```

但另一种更优雅的方法通常可以解决这个问题，那就是重新排列事物，使已知类型排在第一位，编译器可以在进入下一个子句之前对其进行消化。

```F#
["hello"; "world"] |> List.map (fun s -> s.Length)   //ok
```

函数式程序员努力避免显式类型注释，所以这让他们更快乐！

这种技术也可以更广泛地用于其他领域；经验法则是，尽量把“已知类型”的东西放在“未知类型”的前面。

### 重载方法

在 .NET 中调用外部类或方法时，你经常会因为重载而出错。

在许多情况下，例如下面的 concat 示例，您必须显式地注释外部函数的参数，以便编译器知道要调用哪个重载方法。

```F#
let concat x = System.String.Concat(x)           //fails
let concat (x:string) = System.String.Concat(x)  //works
let concat x = System.String.Concat(x:string)    //works
```

有时重载方法具有不同的参数名称，在这种情况下，您还可以通过命名参数给编译器提供线索。下面是 `StreamReader` 构造函数的示例。

```F#
let makeStreamReader x = new System.IO.StreamReader(x)        //fails
let makeStreamReader x = new System.IO.StreamReader(path=x)   //works
```

### 泛型数值函数的问题

数字函数可能会有点令人困惑。通常会出现泛型，但一旦它们绑定到特定的数字类型，它们就会被修复，将它们与不同的数字类型一起使用会导致错误。以下示例演示了这一点：

```F#
let myNumericFn x = x * x
myNumericFn 10
myNumericFn 10.0             //fails
  // error FS0001: This expression was expected to have
  // type int but has type float

let myNumericFn2 x = x * x
myNumericFn2 10.0
myNumericFn2 10               //fails
  // error FS0001: This expression was expected to have
  // type float but has type int
```

对于使用“内联”关键字和“静态类型参数”的数值类型，有一种方法可以绕过这个问题。我不会在这里讨论这些概念，但你可以在MSDN的F#参考中查找它们。

## “信息不足”故障排除摘要

总之，如果编译器抱怨缺少类型或信息不足，您可以做的事情是：

- 在使用之前定义东西（这包括确保文件以正确的顺序编译）
- 把“已知类型”的东西放在“未知类型”的前面。特别是，您可以重新排序管道和类似的链式函数，以便类型化对象排在第一位。
- 根据需要进行注释。一个常见的技巧是添加注释，直到一切正常，然后逐一删除注释，直到达到所需的最小值。如果可能的话，尽量避免注释。它不仅不美观，而且使代码更加脆弱。如果没有明确的依赖关系，更改类型会容易得多。

## 调试类型推断问题

一旦你对所有内容进行了排序和注释，你可能仍然会遇到类型错误，或者发现函数不如预期的泛型 。根据你迄今为止所学到的知识，你应该有工具来确定为什么会发生这种情况（尽管这仍然可能很痛苦）。

例如：

```F#
let myBottomLevelFn x = x

let myMidLevelFn x =
   let y = myBottomLevelFn x
   // some stuff
   let z= y
   // some stuff
   printf "%s" z         // this will kill your generic types!
   // some more stuff
   x

let myTopLevelFn x =
   // some stuff
   myMidLevelFn x
   // some more stuff
   x
```

在这个例子中，我们有一个函数链。底层函数肯定是泛型的，但顶层函数呢？通常，我们可能会认为它是泛型的，但事实并非如此。在这种情况下，我们有：

```F#
val myTopLevelFn : string -> string
```

出了什么问题？答案在中级函数中。z 上的 `%s` 强制它是一个字符串，这也强制 y 和 x 也是字符串。

这是一个非常明显的例子，但有数千行代码，可能会隐藏一行代码，从而导致问题。有一件事可以帮助你查看所有的签名；在这种情况下，签名是：

```F#
val myBottomLevelFn : 'a -> 'a       // generic as expected
val myMidLevelFn : string -> string  // here's the clue! Should be generic
val myTopLevelFn : string -> string
```

当你发现一个意想不到的签名时，你就知道这是有罪的一方。然后，您可以深入了解并重复该过程，直到找到问题。