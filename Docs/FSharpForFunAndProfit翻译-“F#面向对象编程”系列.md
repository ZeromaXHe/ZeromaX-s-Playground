# [返回主 Markdown](./FSharpForFunAndProfit翻译.md)



# 1 F#中的面向对象编程：简介

*Part of the "Object-oriented programming in F#" series (*[link](https://fsharpforfunandprofit.com/posts/object-oriented-intro/#series-toc)*)*

2012年7月13日 这篇文章已有3年多的历史了

https://fsharpforfunandprofit.com/posts/object-oriented-intro/

在本系列中，我们将探讨 F# 如何支持面向对象的类和方法。

## 你应该使用面向对象的特性吗？

正如之前多次强调的那样，F# 本质上是一种函数式语言，但 OO 特性已经很好地集成在一起，没有“附加”的感觉。因此，将 F# 作为面向对象语言，作为 C# 的替代品，是非常可行的。

当然，是使用 OO 风格还是函数式风格取决于你。以下是一些支持和反对的论点。

支持使用 OO 特性的原因：

- 如果你只想直接从 C# 移植而不需要重构。（有关更多信息，有一个关于如何从 C# 移植到 F# 的完整系列。）
- 如果你想把 F# 主要用作面向对象语言，作为 C# 的替代品。
- 如果你需要与其他 .NET 语言融合

反对使用 OO 特性的原因：

- 如果你是一个命令式语言的初学者，那么类可能会成为阻碍你理解函数式编程的拐杖。
- 类没有“纯”F# 数据类型所具有的方便的“开箱即用”特性，例如内置的等式和比较、漂亮的打印等。
- 类和方法不能很好地与类型推理系统和高阶函数配合使用（见这里的讨论），因此大量使用它们意味着你更难从 F# 最强大的部分中受益。

在大多数情况下，最好的方法是混合方法，主要使用纯 F# 类型和函数从类型推理中受益，但偶尔在需要多态性时使用接口和类。

## 理解 F# 的面向对象特性

如果你决定使用 F# 的面向对象特性，以下一系列文章应该涵盖你需要知道的一切，以便在 F# 中高效地使用类和方法。

首先，如何创建类！



# 2 类

2012年7月14日 这篇文章已有3年多的历史了

https://fsharpforfunandprofit.com/posts/classes/

这篇文章和下一篇文章将介绍在 F# 中创建和使用类和方法的基础知识。

## 定义类

就像 F# 中的所有其他数据类型一样，类定义以 `type` 关键字开头。

它们与其他类型的区别在于，类在创建时总是传递一些参数（构造函数），因此类名后总是有括号。

此外，与其他类型不同，类必须具有作为成员附加到它们的函数。这篇文章将解释如何对类执行此操作，但有关将函数附加到其他类型的一般讨论，请参阅关于类型扩展的文章。

因此，例如，如果我们想有一个名为 `CustomerName` 的类，它需要三个参数来构造它，它可以写成这样：

```F#
type CustomerName(firstName, middleInitial, lastName) =
    member this.FirstName = firstName
    member this.MiddleInitial = middleInitial
    member this.LastName = lastName
```

让我们将其与 C# 等价物进行比较：

```c#
public class CustomerName
{
    public CustomerName(string firstName,
       string middleInitial, string lastName)
    {
        this.FirstName = firstName;
        this.MiddleInitial = middleInitial;
        this.LastName = lastName;
    }

    public string FirstName { get; private set; }
    public string MiddleInitial { get; private set; }
    public string LastName { get; private set; }
}
```

您可以看到，在 F# 版本中，主构造函数嵌入到类声明本身中——它不是一个单独的方法。也就是说，类声明具有与构造函数相同的参数，这些参数自动成为不可变的私有字段，用于存储传入的原始值。

因此，在上面的示例中，因为我们将 CustomerName 类声明为：

```F#
type CustomerName(firstName, middleInitial, lastName)
```

因此，`firstName`、`middleInitial` 和 `lastName` 自动成为不可变的私有字段。

### 在构造函数中指定类型

您可能没有注意到，但与 C# 版本不同，上面定义的 `CustomerName` 类不会将参数约束为字符串。一般来说，使用类型推断可能会强制值为字符串，但如果你确实需要显式指定类型，你可以用冒号后跟类型名称的通常方式来指定。

这是在构造函数中具有显式类型的类的一个版本：

```F#
type CustomerName2(firstName:string,
                   middleInitial:string, lastName:string) =
    member this.FirstName = firstName
    member this.MiddleInitial = middleInitial
    member this.LastName = lastName
```

F# 的一个小怪癖是，如果你需要将元组作为参数传递给构造函数，你必须显式地注释它，因为对构造函数的调用看起来是一样的：

```F#
type NonTupledConstructor(x:int,y: int) =
    do printfn "x=%i y=%i" x y

type TupledConstructor(tuple:int * int) =
    let x,y = tuple
    do printfn "x=%i y=%i" x y

// calls look identical
let myNTC = new NonTupledConstructor(1,2)
let myTC = new TupledConstructor(1,2)
```

### 类成员

上面的示例类有三个只读实例属性。在 F# 中，属性和方法都使用 `member` 关键字。

此外，在上面的示例中，您可以在每个成员名称前面看到单词“`this`”。这是一个“自我标识符”，可用于引用类的当前实例。每个非静态成员都必须有一个自标识符，即使它没有被使用（如上面的属性）。没有要求使用特定的单词，只要它是一致的。你可以使用“这个”、“自我”、“我”或任何其他通常表示自我参照的词。

## 理解类签名

当一个类被编译时（或者当你在编辑器中悬停定义时），你会看到该类的“类签名”。例如，对于类定义：

```F#
type MyClass(intParam:int, strParam:string) =
    member this.Two = 2
    member this.Square x = x * x
```

对应的签名为：

```F#
type MyClass =
  class
    new : intParam:int * strParam:string -> MyClass
    member Square : x:int -> int
    member Two : int
  end
```

类签名包含类中所有构造函数、方法和属性的签名。理解这些签名的含义是值得的，因为就像函数一样，你可以通过查看它们来理解类的作用。这也很重要，因为在创建抽象方法和接口时需要编写这些签名。

### 方法签名

这样的方法签名与独立函数的签名非常相似，除了参数名称是签名本身的一部分。

因此，在这种情况下，方法签名是：

```F#
member Square : x:int -> int
```

为了进行比较，独立函数的相应签名为：

```F#
val Square : int -> int
```

### 构造函数签名

构造函数签名总是被称为 `new`，但除此之外，它们看起来像方法签名。

构造函数签名始终将元组值作为其唯一参数。在这种情况下，元组类型是 `int * string`，正如您所期望的那样。返回类型是类本身，正如您所期望的那样。

同样，我们可以将构造函数签名与类似的独立函数进行比较：

```F#
// class constructor signature
new : intParam:int * strParam:string -> MyClass

// standalone function signature
val new : int * string -> MyClass
```

### 属性签名

最后，`member Two : int` 等属性签名与独立简单值的签名非常相似，除了没有给出显式值。

```F#
// member property
member Two : int

// standalone value
val Two : int = 2
```

## 使用“let”绑定的私有字段和函数

在类声明之后，您可以选择使用一组“let”绑定，通常用于定义私有字段和函数。

以下是一些示例代码来演示这一点：

```F#
type PrivateValueExample(seed) =

    // private immutable value
    let privateValue = seed + 1

    // private mutable value
    let mutable mutableValue = 42

    // private function definition
    let privateAddToSeed input =
        seed + input

    // public wrapper for private function
    member this.AddToSeed x =
        privateAddToSeed x

    // public wrapper for mutable value
    member this.SetMutableValue x =
        mutableValue <- x

// test
let instance = new PrivateValueExample(42)
printf "%i" (instance.AddToSeed 2)
instance.SetMutableValue 43
```

在上面的示例中，有三个 let 绑定：

- `privateValue` 设置为初始种子加 1
- `mutableValue` 设置为 42
- `privateAddToSeed` 函数使用初始种子加上一个参数

因为它们是 let 绑定，所以它们是自动私有的，因此要从外部访问它们，必须有一个公共成员充当包装器。

请注意，传递到构造函数中的 `seed` 值也可以作为私有字段使用，就像 let 绑定值一样。

### 可变构造函数参数

有时，您希望传递给构造函数的参数是可变的。您不能在参数本身中指定此值，因此标准技术是创建一个可变的 let 绑定值并从参数中分配它，如下所示：

```F#
type MutableConstructorParameter(seed) =
    let mutable mutableSeed = seed

    // public wrapper for mutable value
    member this.SetSeed x =
        mutableSeed <- x
```

在这种情况下，很常见的是给可变值赋予与参数本身相同的名称，如下所示：

```F#
type MutableConstructorParameter2(seed) =
    let mutable seed = seed // shadow the parameter

    // public wrapper for mutable value
    member this.SetSeed x =
        seed <- x
```

## 带有“do”块的其他构造函数行为

在前面的 `CustomerName` 示例中，构造函数只允许传入一些值，而不做其他任何事情。然而，在某些情况下，您可能需要执行一些代码作为构造函数的一部分。这是使用 `do` 块完成的。

这里有一个例子：

```F#
type DoExample(seed) =
    let privateValue = seed + 1

    //extra code to be done at construction time
    do printfn "the privateValue is now %i" privateValue

// test
new DoExample(42)
```

“do”代码还可以调用之前定义的任何let绑定函数，如下例所示：

```F#
type DoPrivateFunctionExample(seed) =
    let privateValue = seed + 1

    // some code to be done at construction time
    do printfn "hello world"

    // must come BEFORE the do block that calls it
    let printPrivateValue() =
        do printfn "the privateValue is now %i" privateValue

    // more code to be done at construction time
    do printPrivateValue()

// test
new DoPrivateFunctionExample(42)
```

### 在 do 块中通过“this”访问实例

“do”和“let”绑定之间的区别之一是，“do”绑定可以访问实例，而“let”捆绑则不能。这是因为“let”绑定实际上是在构造函数本身之前计算的（类似于 C# 中的字段初始化器），因此从某种意义上说，实例还不存在。

如果你需要从“do”块调用实例的成员，你需要某种方式来引用实例本身。这也是使用“自我标识符”完成的，但这次它附加到类声明本身。

```F#
type DoPublicFunctionExample(seed) as this =
    // Note the "this" keyword in the declaration

    let privateValue = seed + 1

    // extra code to be done at construction time
    do this.PrintPrivateValue()

    // member
    member this.PrintPrivateValue() =
        do printfn "the privateValue is now %i" privateValue

// test
new DoPublicFunctionExample(42)
```

不过，一般来说，除非必须（例如调用虚拟方法），否则从构造函数调用成员不是最佳做法。最好调用私有的let绑定函数，如有必要，让公共成员调用这些相同的私有函数。

## 方法

方法定义非常类似于函数定义，除了它有 `member` 关键字和自我标识符，而不仅仅是 `let` 关键字。

以下是一些示例：

```F#
type MethodExample() =

    // standalone method
    member this.AddOne x =
        x + 1

    // calls another method
    member this.AddTwo x =
        this.AddOne x |> this.AddOne

    // parameterless method
    member this.Pi() =
        3.14159

// test
let me = new MethodExample()
printfn "%i" <| me.AddOne 42
printfn "%i" <| me.AddTwo 42
printfn "%f" <| me.Pi()
```

你可以看到，就像普通函数一样，方法可以有参数，调用其他方法，并且是无参数的（或者更精确地说，接受一个单位参数）

### 元组形式与柯里化形式

与普通函数不同，具有多个参数的方法可以用两种不同的方式定义：

- 柯里化（curried）形式，其中参数用空格分隔，并支持部分应用程序。（为什么“curried”？请参阅curried的解释。）
- 元组形式，其中所有参数同时传入，逗号分隔，在一个元组中。

curried 方法更具功能性，而 tuple 方法更面向对象。下面是一个示例类，每种方法都有一个方法：

```F#
type TupleAndCurriedMethodExample() =

    // curried form
    member this.CurriedAdd x y =
        x + y

    // tuple form
    member this.TupleAdd(x,y) =
        x + y

// test
let tc = new TupleAndCurriedMethodExample()
printfn "%i" <| tc.CurriedAdd 1 2
printfn "%i" <| tc.TupleAdd(1,2)

// use partial application
let addOne = tc.CurriedAdd 1
printfn "%i" <| addOne 99
```

那么，你应该使用哪种方法呢？

元组形式的优点是：

- 与其他 .NET 代码兼容
- 支持命名参数和可选参数
- 支持方法重载（多个同名方法，仅在函数签名上不同）

另一方面，元组形式的缺点是：

- 不支持部分应用
- 与高阶函数配合不好
- 与类型推理配合不好

有关元组形式与 curried 形式的更详细讨论，请参阅关于类型扩展的文章。

### let 绑定函数与类方法结合使用

一种常见的模式是创建 let 绑定函数来完成所有繁重的工作，然后让公共方法直接调用这些内部函数。这样做的好处是，类型推理在函数式代码中比在方法中工作得更好。

这里有一个例子：

```F#
type LetBoundFunctions() =

    let listReduce reducer list =
        list |> List.reduce reducer

    let reduceWithSum sum elem =
        sum + elem

    let sum list =
        list |> listReduce reduceWithSum

    // finally a public wrapper
    member this.Sum  = sum

// test
let lbf = new LetBoundFunctions()
printfn "Sum is %i" <| lbf.Sum [1..10]
```

有关如何执行此操作的更多详细信息，请参阅此讨论。

### 递归方法

与普通的 let 绑定函数不同，递归方法不需要特殊的 `rec` 关键字。这是一个令人厌烦的熟悉的斐波那契函数方法：

```F#
type MethodExample() =

    // recursive method without "rec" keyword
    member this.Fib x =
        match x with
        | 0 | 1 -> 1
        | _ -> this.Fib (x-1) + this.Fib (x-2)

// test
let me = new MethodExample()
printfn "%i" <| me.Fib 10
```

### 方法的类型注解

与往常一样，编译器通常可以推断出方法参数和返回值的类型，但如果需要指定它们，则可以按照与标准函数相同的方式进行：

```F#
type MethodExample() =
    // explicit type annotation
    member this.AddThree (x:int) :int =
        x + 3
```

## 属性

属性可分为三组：

- 不可变属性，其中有“get”但没有“set”。
- 可变属性，其中有一个“get”和一个（可能是私有的）“set”。
- 只写属性，其中有一个“set”但没有“get”。这些非常不寻常，我不会在这里讨论它们，但MSDN文档描述了语法（如果你需要的话）。

不可变和可变属性的语法略有不同。

对于不可变属性，语法很简单。有一个“get”成员类似于标准的“let”值绑定。绑定右侧的表达式可以是任何标准表达式，通常是构造函数参数、私有let绑定字段和私有函数的组合。

这里有一个例子：

```F#
type PropertyExample(seed) =
    // immutable property
    // using a constructor parameter
    member this.Seed = seed
```

然而，对于可变属性，语法更为复杂。您需要提供两个函数，一个用于获取，另一个用于设置。这是通过使用以下语法完成的：

```F#
with get() = ...
and set(value) = ...
```

这里有一个例子：

```F#
type PropertyExample(seed) =
    // private mutable value
    let mutable myProp = seed

    // mutable property
    // changing a private mutable value
    member this.MyProp
        with get() = myProp
        and set(value) =  myProp <- value
```

要使 set 函数私有，请使用关键字 `private set`。

### 自动属性

从 VS2012 开始，F# 支持自动属性，这消除了为它们创建单独备份存储的要求。

要创建不可变的 auto 属性，请使用以下语法：

```F#
member val MyProp = initialValue
```

要创建可变的 auto 属性，请使用以下语法：

```F#
member val MyProp = initialValue with get,set
```

请注意，在这种语法中，有一个新的关键字 `val`，自我标识符已经消失。

### 完整的属性示例

以下是一个完整的示例，演示了所有属性类型：

```F#
type PropertyExample(seed) =
    // private mutable value
    let mutable myProp = seed

    // private function
    let square x = x * x

    // immutable property
    // using a constructor parameter
    member this.Seed = seed

    // immutable property
    // using a private function
    member this.SeedSquared = square seed

    // mutable property
    // changing a private mutable value
    member this.MyProp
        with get() = myProp
        and set(value) =  myProp <- value

    // mutable property with private set
    member this.MyProp2
        with get() = myProp
        and private set(value) =  myProp <- value

    // automatic immutable property (in VS2012)
    member val ReadOnlyAuto = 1

    // automatic mutable property (in VS2012)
    member val ReadWriteAuto = 1 with get,set

// test
let pe = new PropertyExample(42)
printfn "%i" <| pe.Seed
printfn "%i" <| pe.SeedSquared
printfn "%i" <| pe.MyProp
printfn "%i" <| pe.MyProp2

// try calling set
pe.MyProp <- 43    // Ok
printfn "%i" <| pe.MyProp

// try calling private set
pe.MyProp2 <- 43   // Error
```

### 属性 vs. 无参数方法

此时，您可能会对属性和无参数方法之间的区别感到困惑。乍一看，它们看起来是一样的，但有一个微妙的区别——“无参数”方法并不是真正的无参数方法；它们总是有一个单位参数。

以下是一个定义和用法差异的示例：

```F#
type ParameterlessMethodExample() =
    member this.MyProp = 1    // No parens!
    member this.MyFunc() = 1  // Note the ()

// in use
let x = new ParameterlessMethodExample()
printfn "%i" <| x.MyProp      // No parens!
printfn "%i" <| x.MyFunc()    // Note the ()
```

您还可以通过查看类定义的签名来分辨差异

类定义如下：

```F#
type ParameterlessMethodExample =
  class
    new : unit -> ParameterlessMethodExample
    member MyFunc : unit -> int
    member MyProp : int
  end
```

该方法具有签名 `MyFunc: unit->int`，属性具有签名 `MyProp : int`。

这与在任何类之外单独声明函数和属性时的签名非常相似：

```F#
let MyFunc2() = 1
let MyProp2 = 1
```

这些签名看起来像：

```F#
val MyFunc2 : unit -> int
val MyProp2 : int = 1
```

这几乎完全相同。

如果您不清楚差异以及为什么函数需要单位参数，请阅读无参数方法的讨论。

## 二级构造函数

除了声明中嵌入的主构造函数外，类还可以有其他构造函数。这些由 `new` 关键字表示，必须将主构造函数作为最后一个表达式调用。

```F#
type MultipleConstructors(param1, param2) =
    do printfn "Param1=%i Param12=%i" param1 param2

    // secondary constructor
    new(param1) =
        MultipleConstructors(param1,-1)

    // secondary constructor
    new() =
        printfn "Constructing..."
        MultipleConstructors(13,17)

// test
let mc1 = new MultipleConstructors(1,2)
let mc2 = new MultipleConstructors(42)
let mc3 = new MultipleConstructors()
```

## 静态成员

就像在 C# 中一样，类可以有静态成员，这用 `static` 关键字表示。`static` 修饰符位于 member 关键字之前。

静态成员不能有诸如“this”之类的自我标识符，因为没有可供它们引用的实例。

```F#
type StaticExample() =
    member this.InstanceValue = 1
    static member StaticValue = 2  // no "this"

// test
let instance = new StaticExample()
printf "%i" instance.InstanceValue
printf "%i" StaticExample.StaticValue
```

## 静态构造函数

F# 中没有直接等效的静态构造函数，但您可以创建静态 let 绑定值和静态 do 块，这些值和块在类首次使用时执行。

```F#
type StaticConstructor() =

    // static field
    static let rand = new System.Random()

    // static do
    static do printfn "Class initialization!"

    // instance member accessing static field
    member this.GetRand() = rand.Next()
```

## 成员的可访问性

您可以使用标准 .NET 关键字 `public`、`private` 和 `internal` 控制成员的可访问性。可访问性修饰符位于 `member` 关键字之后和成员名称之前。

与 C# 不同，默认情况下所有类成员都是公共的，而不是私有的。这包括属性和方法。但是，非成员（例如 let 声明）是私有的，不能公开。

这里有一个例子：

```F#
type AccessibilityExample() =
    member this.PublicValue = 1
    member private this.PrivateValue = 2
    member internal this.InternalValue = 3
// test
let a = new AccessibilityExample();
printf "%i" a.PublicValue
printf "%i" a.PrivateValue  // not accessible
```

对于属性，如果 set 和 get 具有不同的可访问性，则可以使用单独的可访问修改器标记每个部分。

```F#
type AccessibilityExample2() =
    let mutable privateValue = 42
    member this.PrivateSetProperty
        with get() =
            privateValue
        and private set(value) =
            privateValue <- value

// test
let a2 = new AccessibilityExample2();
printf "%i" a2.PrivateSetProperty  // ok to read
a2.PrivateSetProperty <- 43        // not ok to write
```

在实践中，C# 中常见的“public get，private set”组合在 F# 中通常不需要，因为如前所述，不可变属性可以更优雅地定义。

## 提示：定义类供其他 .NET 代码使用

如果您正在定义需要与其他 .NET 代码互操作的类，不要在模块内定义它们！在命名空间中定义它们，在任何模块之外。

这样做的原因是 F# 模块被公开为静态类，模块中定义的任何F#类都被定义为静态类中的嵌套类，这可能会扰乱您的互操作。例如，一些单元测试运行者不喜欢静态类。

在模块外部定义的 F# 类作为普通顶级 .NET 类被生成，这可能是你想要的。但请记住（如前一篇文章所述），如果你不具体声明一个命名空间，你的类将被放置在一个自动生成的模块中，并在你不知情的情况下嵌套。

以下是两个 F# 类的示例，一个在模块外部定义，另一个在内部定义：

```F#
// Note: this code will not work in an .FSX script,
// only in an .FS source file.
namespace MyNamespace

type TopLevelClass() =
    let nothing = 0

module MyModule =

    type NestedClass() =
        let nothing = 0
```

下面是同样的代码在 C# 中的样子：

```c#
namespace MyNamespace
{
  public class TopLevelClass
  {
  // code
  }

  public static class MyModule
  {
    public class NestedClass
    {
    // code
    }
  }
}
```

## 构建和使用类

既然我们已经定义了类，我们该如何使用它？

创建类实例的一种方法很简单，就像 C# 一样——使用 `new` 关键字并将参数传递给构造函数。

```F#
type MyClass(intParam:int, strParam:string) =
    member this.Two = 2
    member this.Square x = x * x

let myInstance = new MyClass(1,"hello")
```

然而，在 F# 中，构造函数被认为只是另一个函数，所以你通常可以删除 `new` 并自行调用构造函数，如下所示：

```F#
let myInstance2 = MyClass(1,"hello")
let point = System.Drawing.Point(1,2)   // works with .NET classes too!
```

在创建实现 `IDisposible` 的类时，如果不使用 `new`，将收到编译器警告。

```F#
let sr1 = System.IO.StringReader("")      // Warning
let sr2 = new System.IO.StringReader("")  // OK
```

对于一次性用品（disposables），这可能是一个有用的提醒，即使用 `use` 关键字而不是 `let` 关键字。有关更多信息，请参阅 `use` 帖子。

### 调用方法和属性

一旦你有了一个实例，你就可以“点入”该实例，并以标准方式使用任何方法和属性。

```F#
myInstance.Two
myInstance.Square 2
```

在上面的讨论中，我们看到了许多成员使用的例子，对此没有太多要说的。

请记住，如上所述，元组样式方法和 curried 样式方法可以以不同的方式调用：

```F#
type TupleAndCurriedMethodExample() =
    member this.TupleAdd(x,y) = x + y
    member this.CurriedAdd x y = x + y

let tc = TupleAndCurriedMethodExample()
tc.TupleAdd(1,2)      // called with parens
tc.CurriedAdd 1 2     // called without parens
2 |> tc.CurriedAdd 1  // partial application
```

# 3 继承和抽象类

*Part of the "Object-oriented programming in F#" series (*[link](https://fsharpforfunandprofit.com/posts/inheritance/#series-toc)*)*

2012年7月15日 这篇文章已有3年多的历史了

https://fsharpforfunandprofit.com/posts/inheritance/

这是上一篇关于课程的文章的后续。本文将重点介绍F#中的继承，以及如何定义和使用抽象类和接口。

## 继承

要声明一个类继承自另一个类，请使用以下语法：

```F#
type DerivedClass(param1, param2) =
   inherit BaseClass(param1)
```

`inherit` 关键字表示 `DerivedClass` 从 `BaseClass` 继承。此外，必须同时调用某些基类构造函数。

此时，将 F# 与 C# 进行比较可能会有所帮助。下面是一对非常简单的类的 C# 代码。

```c#
public class MyBaseClass
{
    public MyBaseClass(int param1)
    {
        this.Param1 = param1;
    }
    public int Param1 { get; private set; }
}

public class MyDerivedClass: MyBaseClass
{
    public MyDerivedClass(int param1,int param2): base(param1)
    {
        this.Param2 = param2;
    }
    public int Param2 { get; private set; }
}
```

请注意，继承声明 `class MyDerivedClass: MyBaseClass` 与调用 `base(param1)` 的构造函数不同。

以下是 F# 版本：

```F#
type BaseClass(param1) =
   member this.Param1 = param1

type DerivedClass(param1, param2) =
   inherit BaseClass(param1)
   member this.Param2 = param2

// test
let derived = new DerivedClass(1,2)
printfn "param1=%O" derived.Param1
printfn "param2=%O" derived.Param2
```

与 C# 不同，声明的继承部分 `inherit BaseClass(param1)` 包含要继承的类及其构造函数。

## 抽象和虚拟方法

显然，继承的部分意义在于能够拥有抽象方法、虚拟方法等。

### 在基类中定义抽象方法

在 C# 中，抽象方法由 `abstract` 关键字加上方法签名表示。在 F# 中，这是相同的概念，除了函数签名在 F# 中的编写方式与 C# 完全不同。

```F#
// concrete function definition
let Add x y = x + y

// function signature
// val Add : int -> int -> int
```

因此，为了定义一个抽象方法，我们使用签名语法以及 `abstract member` 关键字：

```F#
type BaseClass() =
   abstract member Add: int -> int -> int
```

请注意，等号已被冒号替换。这是您所期望的，因为等号用于绑定值，而冒号用于类型注释。

现在，如果你试图编译上面的代码，你会得到一个错误！编译器将抱怨该方法没有实现。要解决此问题，您需要：

- 提供该方法的默认实现，或
- 告诉编译器，整个类也是抽象的。

我们很快就会研究这两种替代方案。

### 定义抽象属性

抽象不可变属性以类似的方式定义。签名就像一个简单的值。

```F#
type BaseClass() =
   abstract member Pi : float
```

如果抽象属性是读/写的，则添加 get/set 关键字。

```F#
type BaseClass() =
   abstract Area : float with get,set
```

### 默认实现（但没有虚拟方法）

要在基类中提供抽象方法的默认实现，请使用 `default` 关键字而不是 `member` 关键字：

```F#
// with default implementations
type BaseClass() =
   // abstract method
   abstract member Add: int -> int -> int
   // abstract property
   abstract member Pi : float

   // defaults
   default this.Add x y = x + y
   default this.Pi = 3.14
```

您可以看到默认方法是以通常的方式定义的，除了使用 `default` 而不是 `member`。

F# 和 C# 之间的一个主要区别是，在 C# 中，您可以使用 `virtual` 关键字将抽象定义和默认实现组合到一个方法中。在 F# 中，你不能。您必须分别声明抽象方法和默认实现。`abstract member` 具有签名，`default` 具有实现。

### 抽象类

如果至少有一个抽象方法没有默认实现，则整个类都是抽象的，您必须通过用 `AbstractClass` 属性对其进行注释来表明这一点。

```F#
[<AbstractClass>]
type AbstractBaseClass() =
   // abstract method
   abstract member Add: int -> int -> int

   // abstract immutable property
   abstract member Pi : float

   // abstract read/write property
   abstract member Area : float with get,set
```

如果这样做，那么编译器将不再抱怨缺少实现。

### 子类中的覆盖方法

要覆盖子类中的抽象方法或属性，请使用 `override` 关键字而不是 `member` 关键字。除此之外，重写的方法以通常的方式定义。

```F#
[<AbstractClass>]
type Animal() =
   abstract member MakeNoise: unit -> unit

type Dog() =
   inherit Animal()
   override this.MakeNoise () = printfn "woof"

// test
// let animal = new Animal()  // error creating ABC
let dog = new Dog()
dog.MakeNoise()
```

要调用 base 方法，请使用 `base` 关键字，就像在 C# 中一样。

```F#
type Vehicle() =
   abstract member TopSpeed: unit -> int
   default this.TopSpeed() = 60

type Rocket() =
   inherit Vehicle()
   override this.TopSpeed() = base.TopSpeed() * 10

// test
let vehicle = new Vehicle()
printfn "vehicle.TopSpeed = %i" <| vehicle.TopSpeed()
let rocket = new Rocket()
printfn "rocket.TopSpeed = %i" <| rocket.TopSpeed()
```

### 抽象方法概述

抽象方法基本上很简单，类似于 C#。如果你习惯了 C#，只有两个方面可能会很棘手：

- 您必须了解函数签名的工作原理及其语法！有关详细讨论，请参阅关于函数签名的帖子。
- 没有一体化的虚拟方法。您必须分别定义抽象方法和默认实现。

# 4 接口

2012年7月16日 这篇文章已有3年多的历史了

https://fsharpforfunandprofit.com/posts/interfaces/

接口在 F# 中是可用的，并且完全受支持，但在许多重要方面，它们的使用与 C# 中可能习惯的不同。

## 定义接口

定义接口类似于定义抽象类。事实上，它们如此相似，以至于你很容易把它们混淆。

以下是一个接口定义：

```F#
type MyInterface =
   // abstract method
   abstract member Add: int -> int -> int

   // abstract immutable property
   abstract member Pi : float

   // abstract read/write property
   abstract member Area : float with get,set
```

以下是等效抽象基类的定义：

```F#
[<AbstractClass>]
type AbstractBaseClass() =
   // abstract method
   abstract member Add: int -> int -> int

   // abstract immutable property
   abstract member Pi : float

   // abstract read/write property
   abstract member Area : float with get,set
```

那么，有什么区别呢？与往常一样，所有抽象成员仅由签名定义。唯一的区别似乎是缺少 `[<AbstractClass>]` 属性。

但在之前关于抽象方法的讨论中，我们强调 `[<AbstractClass>]` 属性是必需的；编译器会抱怨这些方法没有其他实现。那么，接口定义是如何逃脱惩罚的呢？

答案很琐碎，但很微妙。该接口没有构造函数。也就是说，它在接口名称后没有任何括号：

```F#
type MyInterface =   // <- no parens!
```

就是这样。删除括号将把类定义转换为接口！

### 显式和隐式接口实现

当需要在类中实现接口时，F# 与 C# 有很大不同。在 C# 中，您可以向类定义中添加一系列接口，并隐式实现这些接口。

在 F# 中并非如此。在 F# 中，所有接口都必须显式实现。

在显式接口实现中，接口成员只能通过接口实例访问（例如，通过将类转换为接口类型）。接口成员不作为类本身的一部分可见。

C# 支持显式和隐式接口实现，但几乎总是使用隐式方法，许多程序员甚至不知道 C# 中的显式接口。

### 在 F# 中实现接口

那么，如何在 F# 中实现接口呢？你不能像抽象基类那样从它“继承”。您必须使用语法 `interface XXX with` 为每个接口成员提供显式实现，如下所示：

```F#
type IAddingService =
    abstract member Add: int -> int -> int

type MyAddingService() =

    interface IAddingService with
        member this.Add x y =
            x + y

    interface System.IDisposable with
        member this.Dispose() =
            printfn "disposed"
```

上面的代码显示了 `MyAddingService` 类如何显式实现 `IAddingService` 和 `IDisposable` 接口。在完成所需的 `interface XXX with` 部分后，成员以正常方式实现。

（顺便说一句，再次注意 `MyAddingService()` 有一个构造函数，而 `IAddingService` 没有。）

### 使用接口

现在让我们尝试使用添加服务接口：

```F#
let mas = new MyAddingService()
mas.Add 1 2    // error
```

我们立刻遇到了一个错误。该实例似乎根本没有实现 `Add` 方法。当然，这实际上意味着我们必须首先使用 `:>` 运算符将其转换为接口：

```F#
// cast to the interface
let mas = new MyAddingService()
let adder = mas :> IAddingService
adder.Add 1 2  // ok
```

这可能看起来非常尴尬，但在实践中这不是问题，因为在大多数情况下，类型转换都是为你隐式完成的。

例如，您通常会将实例传递给指定接口参数的函数。在这种情况下，类型转换是自动完成的：

```F#
// function that requires an interface
let testAddingService (adder:IAddingService) =
    printfn "1+2=%i" <| adder.Add 1 2  // ok

let mas = new MyAddingService()
testAddingService mas // cast automatically
```

在 `IDisposable` 的特殊情况下，`use` 关键字也会根据需要自动转换实例：

```F#
let testDispose =
    use mas = new MyAddingService()
    printfn "testing"
    // Dispose() is called here
```

# 5 对象表达式

*Part of the "Object-oriented programming in F#" series (*[link](https://fsharpforfunandprofit.com/posts/object-expressions/#series-toc)*)*

2012年7月17日 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/object-expressions/

因此，正如我们在上一篇文章中看到的，在 F# 中实现接口比在 C# 中更尴尬。但F#有一个诀窍，叫做“对象表达式”。

使用对象表达式，您可以动态实现接口，而无需创建类。

## 使用对象表达式实现接口

对象表达式最常用于实现接口。为此，您可以使用语法 `new MyInterface with ...`，然后用花括号把整个东西包起来（这是 F# 中为数不多的用法之一！）

下面是一些创建多个对象的示例代码，每个对象都实现 `IDisposable`。

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

如果执行此代码，您将看到下面的输出。你可以看到，当对象超出作用域时，`Dispose()` 确实被调用了。

```
using first resource
    do something with   inner resource 1
    inner resource 1 disposed
    do something with   inner resource 2
    inner resource 2 disposed
    do something with   inner resource 3
    inner resource 3 disposed
using second resource
done.
second resource disposed
first resource disposed
```

我们可以对 `IAddingService` 采取相同的方法，并动态创建一个。

```F#
let makeAdder id =
   { new IAddingService with
     member this.Add x y =
         printfn "Adder%i is adding" id
         let result = x + y
         printfn "%i + %i = %i" x y result
         result
         }

let testAdders =
    for i in [1..3] do
        let adder = makeAdder i
        let result = adder.Add i i
        () //ignore result
```

对象表达式非常方便，如果你与一个接口繁重的库交互，它可以大大减少你需要创建的类的数量。
