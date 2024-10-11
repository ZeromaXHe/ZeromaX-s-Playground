# [返回主 Markdown](./FSharpForFunAndProfit翻译.md)



# 1 函数式思维：引言

*Part of the "Thinking functionally" series (*[link](https://fsharpforfunandprofit.com/posts/thinking-functionally-intro/#series-toc)*)*

函数式编程的基础知识
01五月2012 这篇文章是超过3年

https://fsharpforfunandprofit.com/posts/thinking-functionally-intro/

现在你已经在“为什么使用F#”系列中看到了F#的一些力量，我们将回过头来看看函数式编程的基本原理——“函数式编程”的真正含义是什么，以及这种方法与面向对象或命令式编程有何不同。

## 改变你的思维方式

重要的是要理解函数式编程不仅仅是风格上的差异；这是一种完全不同的编程思维方式，就像真正的面向对象编程（在 Smalltalk 中）也是一种与 C 等传统命令式语言不同的思维方式一样。

F# 确实允许非函数式风格，并且很容易保留你已经熟悉的习惯。你可能只是以一种非函数式的方式使用F#，而不会真正改变你的思维方式，也不会意识到你错过了什么。为了充分利用 F#，并熟练掌握函数式编程，你必须从函数式而非命令式的角度思考。这就是本系列的目标：帮助您深入理解函数式编程，并帮助您改变思维方式。

这将是一个相当抽象的系列，尽管我将使用大量简短的代码示例来演示这些观点。我们将探讨以下几点：

- **数学函数**。第一篇文章介绍了函数式语言背后的数学思想，以及这种方法带来的好处。
- **函数和值**。下一篇文章将介绍函数和值，展示“值”与变量的不同之处，以及为什么函数和简单值之间存在相似之处。
- **类型**。然后我们继续讨论与函数一起工作的基本类型：string和int等基本类型；单元类型、函数类型和泛型类型。
- **具有多个参数的函数**。接下来，我将解释“currying”和“partial application”的概念。如果你来自一个迫切的背景，这就是你的大脑开始受伤的地方！
- **定义函数**。然后，一些帖子专门介绍了定义和组合功能的许多不同方法。
  函数签名。然后是一篇关于函数签名这一关键主题的重要文章：它们的含义以及如何使用它们来帮助理解。
- **组织函数**。一旦你知道如何创建函数，你如何组织它们，使它们可用于其他代码？

# 2 数学函数

*Part of the "Thinking functionally" series (*[link](https://fsharpforfunandprofit.com/posts/mathematical-functions/#series-toc)*)*

函数式编程背后的动力
02五月2012 这篇文章是超过3年

https://fsharpforfunandprofit.com/posts/mathematical-functions/

函数式编程背后的动力来自数学。数学函数具有许多非常好的特性，函数式语言试图在现实世界中模拟这些特性。

首先，让我们从一个数学函数开始，该函数将一个数字加1。

`Add1(x) = x+1`

这到底意味着什么？嗯，这似乎很简单。这意味着有一个操作以一个数字开始，然后加一。

让我们介绍一些术语：

- 可以用作函数输入的值集称为域。在这种情况下，它可能是实数集，但为了让现在的生活更简单，让我们把它限制在整数上。
- 函数的一组可能输出值称为范围（技术上称为共域上的图像）。在这种情况下，它也是整数集。
- 据说该函数将域映射到范围。

以下是 F# 中的定义

```F#
let add1 x = x + 1
```

如果你在 F# 交互窗口中键入它（不要忘记双分号），你会看到结果（函数的“签名”）：

```F#
val add1 : int -> int
```

让我们详细看看这个输出：

- 总体含义是“函数 `add1` 将整数（域）映射到整数（范围）上”。
- “`add1`”被定义为“val”，是“value”的缩写。嗯……那是什么意思？我们稍后将讨论值。
- 箭头符号“->”用于显示域和范围。在这种情况下，域是int类型，范围也是int类型。

还要注意，没有指定类型，但 F# 编译器猜测该函数正在处理 int。（这个可以调整吗？是的，我们稍后会看到）。

## 数学函数的关键性质

数学函数具有一些性质，这些性质与你在过程编程中习惯的函数类型非常不同。

- 对于给定的输入值，函数总是给出相同的输出值
- 函数没有副作用。

这些属性提供了一些非常强大的好处，因此函数式编程语言也试图在设计中强制执行这些属性。让我们依次看看它们中的每一个。

### 对于给定的输入，数学函数总是给出相同的输出

在命令式编程中，我们认为函数“做”某事或“计算”某事。数学函数不做任何计算——它纯粹是从输入到输出的映射。事实上，定义函数的另一种方式就是将其视为所有映射的集合。例如，以一种非常粗略的方式，我们可以将“`add1`”函数（在C#中）定义为

```C#
int add1(int input)
{
   switch (input)
   {
   case 0: return 1;
   case 1: return 2;
   case 2: return 3;
   case 3: return 4;
   etc ad infinitum
   }
}
```

显然，我们不能为每个可能的整数都有一个案例，但原理是一样的。你可以看到，根本没有进行任何计算，只是进行了查找。

### 数学函数没有副作用

在数学函数中，输入和输出在逻辑上是两个不同的东西，两者都是预定义的。该函数不会改变输入或输出，它只是将域中预先存在的输入值映射到范围内预先存在的输出值。

换句话说，评估函数不可能对输入或其他任何东西产生任何影响。记住，评估函数实际上并不是计算或操纵任何东西；这只是一次美化的查找。

这些值的“不变性”是微妙的，但非常重要。如果我在做数学，我不希望当我加上数字时，下面的数字会改变！例如，如果我有：

```
x = 5
y = x+1
```

我不希望x因加1而改变。我希望得到一个不同的数字（y），x保持不变。在数学领域，整数已经作为一个不可改变的集合存在，而“add1”函数只是定义了它们之间的关系。

### 纯函数的力量

具有可重复结果且没有副作用的函数称为“纯函数”，你可以用它们做一些有趣的事情：

- 它们是可并行化的。我可以取 1 到 1000 之间的所有整数，假设有1000个不同的CPU，我可以让每个CPU同时执行相应整数的“`add1`”函数，因为它们之间不需要任何交互。不需要锁、互斥量（mutexes）、信号量（semaphores）等。
- 我可以惰性地使用函数，只在需要输出时对其进行求值。我可以肯定，无论我现在还是以后求值，答案都是一样的。
- 我只需要对某个输入计算一次函数，然后就可以缓存结果，因为我知道相同的输入总是给出相同的输出。
- 如果我有一些纯函数，我可以按照我喜欢的任何顺序对它们进行求值。同样，这对最终结果没有任何影响。

所以你可以看到，如果我们能用编程语言创建纯函数，我们会立即获得很多强大的技术。事实上，你可以在F#中完成所有这些事情：

- 您已经在“为什么使用F#？”系列中看到了并行性的一个例子。
- 惰性地求值函数将在“优化”系列中讨论。
- 缓存函数的结果称为“记忆化”，也将在“优化”系列中讨论。
- 不关心求值顺序会使并发编程更容易，并且在重新排序或重构函数时不会引入错误。

## 数学函数的“无用”性质

数学函数也有一些在编程中使用时似乎没有多大帮助的特性。

- 输入和输出值是不可变的
- 函数总是只有一个输入和一个输出

这些特性也反映在函数式编程语言的设计中。让我们依次看看这些。

### 输入和输出值是不可变的

从理论上讲，不可变值似乎是一个好主意，但如果你不能以传统方式为变量赋值，你怎么能真正完成任何工作呢？

我可以向你保证，这并不像你想象的那么严重。当您完成本系列时，您将看到这在实践中是如何工作的。

### 数学函数总是只有一个输入和一个输出

正如你从图表中看到的，一个数学函数总是只有一个输入和一个输出。函数式编程语言也是如此，尽管当你第一次使用它们时可能并不明显。

这似乎很烦人。如果没有具有两个（或更多）参数的函数，你如何做有用的事情？

好吧，它有一种方法可以做到这一点，而且，在F#中，它对你来说是完全透明的。它被称为“currying”，它值得拥有自己的职位，这个职位很快就会出现。

事实上，正如您稍后将发现的那样，这两个“无益”的属性将变得非常有用，并且是函数式编程如此强大的关键部分。

# 3 函数值和简单值

*Part of the "Thinking functionally" series (*[link](https://fsharpforfunandprofit.com/posts/function-values-and-simple-values/#series-toc)*)*

绑定不赋值
03五月2012这篇文章是超过3年

https://fsharpforfunandprofit.com/posts/function-values-and-simple-values/

让我们再看看这个简单的函数

这里的“x”是什么意思？这意味着：

1. 从输入域中接受一些值。
2. 使用名称“x”表示该值，以便我们以后可以引用它。

使用名称表示值的过程称为“绑定”。名称“x”被“绑定”到输入值。

所以，如果我们用输入5来评估函数，比如说，在原始定义中看到“x”的地方，我们用“5”替换它，有点像文字处理器中的搜索和替换。

```F#
let add1 x = x + 1
add1 5
// replace "x" with "5"
// add1 5 = 5 + 1 = 6
// result is 6
```

重要的是要明白这不是任务。“x”不是分配给该值的“插槽”或变量，以后可以分配给另一个值。它是名称“x”与该值的一次性关联。该值是预定义的整数之一，不能更改。因此，一旦绑定，x也不能改变；一旦与某个值相关联，就始终与该值相关联。

这个概念是函数式思维的关键部分：没有“变量”，只有值。

## 函数值

如果你再仔细想想，你会发现“`add1`”这个名字本身只是“向其输入添加一个值的函数”的绑定。函数本身与其绑定的名称无关。

当你键入 `let add1 x=x+1` 时，你是在告诉 F# 编译器“每次看到名称”`add1`“时，用向其输入添加1的函数替换它”。“`add1`”被称为**函数值**。

要查看函数是否独立于其名称，请尝试：

```F#
let add1 x = x + 1
let plus1 = add1
add1 5
plus1 5
```

您可以看到“`add1`”和“`plus1`”是两个引用（“绑定到”）同一函数的名称。

你总是可以识别一个函数值，因为它的签名具有标准形式的 `域->范围`。这是一个通用的函数值签名：

```F#
val functionName : domain -> range
```

## 简单值

想象一下，一个操作总是返回整数5，并且没有任何输入。



这将是一个“常量”的操作。

我们如何用F#写这个？我们想告诉F#编译器“每次你看到名称c时，用5替换它”。方法如下：

```F#
let c = 5
```

其在求值时返回：

```F#
val c : int = 5
```

这次没有映射箭头，只有一个整数。新的是一个等号，后面打印了实际值。F# 编译器知道这个绑定有一个已知的值，它将始终返回，即值 5。

换句话说，我们刚刚定义了一个常量，或者用 F# 术语来说，一个简单的值。

你总是可以区分简单值和函数值，因为所有简单值都有一个签名，看起来像：

```F#
val aName: type = constant     // Note that there is no arrow
```

## 简单值与函数值

重要的是要理解，在 F# 中，与 C# 等语言不同，简单值和函数值之间几乎没有区别。它们都是可以绑定到名称（使用相同的关键字let）然后传递的值。事实上，函数思维的一个关键方面就是：函数是可以作为输入传递给其他函数的值，我们很快就会看到。

请注意，简单值和函数值之间存在细微差异。函数总是有一个域和范围，必须“应用”到参数才能得到结果。绑定后不需要对简单值进行求值。使用上面的例子，如果我们想定义一个返回 5 的“常量函数”，我们必须使用

```F#
let c = fun()->5
// or
let c() = 5
```

这些函数的签名是：

```F#
val c : unit -> int
```

而不是：

```F#
val c : int = 5
```

稍后将详细介绍单元、函数语法和匿名函数。

## “值”与“对象”

在F#这样的函数式编程语言中，大多数东西都被称为“值”。在C#这样的面向对象语言中，大多数东西都被称为“对象”。那么，“值”和“对象”之间有什么区别呢？

如上所述，值只是域的一个成员。int的域、字符串的域、将int映射到字符串的函数的域等等。原则上，值是不可变的。值没有任何行为。

在标准定义中，对象是数据结构及其相关行为（方法）的封装。一般来说，对象应该具有状态（即可变），所有改变内部状态的操作都必须由对象本身提供（通过“点”符号）。

在F#中，即使是原始值也有一些类似对象的行为。例如，您可以点入字符串以获取其长度：

```F#
"abc".Length
```

但是，一般来说，我们将避免在 F# 中为标准值使用“object”，将其保留为引用真类的实例或其他公开成员方法的值。

## 命名值

标准命名规则用于值和函数名称，基本上是任何字母数字字符串，包括下划线。有几个额外的：

您可以在名称中的任何位置添加撇号，但第一个字符除外。所以：

```F#
A'b'c     begin'  // valid names
```

最后一个刻度通常用于表示值的某种“变体”版本：

```F#
let f = x
let f' = derivative f
let f'' = derivative f'
```

或定义现有关键字的变体

```F#
let if' b t f = if b then t else f
```

您还可以在任何字符串周围加上双引号，以形成有效的标识符。

```F#
``this is a name``  ``123``    //valid names
```

有时你可能想使用双引号技巧：

- 当您想使用与关键字相同的标识符时

  ```F#
  let ``begin`` = "begin"
  ```

- 当试图将自然语言用于业务规则、单元测试或BDD风格的可执行规范时，就像Cucumber一样。

  ```F#
  let ``is first time customer?`` = true
  let ``add gift to order`` = ()
  if ``is first time customer?`` then ``add gift to order``
  
  // Unit test
  let [<Test>] ``When input is 2 then expect square is 4``=
     // code here
  
  // BDD clause
  let [<Given>] ``I have (.*) N products in my cart`` (n:int) =
     // code here
  ```

与C#不同，F#的命名约定是函数和值以小写字母而不是大写字母开头（camelCase 而不是 PascalCase），除非是为暴露给其他 .NET语言而设计的。但是，类型和模块使用大写字母。

# 4 类型如何与函数协同工作

*Part of the "Thinking functionally" series (*[link](https://fsharpforfunandprofit.com/posts/how-types-work-with-functions/#series-toc)*)*

理解类型符号
04五月2012 这篇文章是超过3年

https://fsharpforfunandprofit.com/posts/how-types-work-with-functions/

现在我们对函数有了一些了解，我们将看看类型是如何与函数一起工作的，无论是作为域还是范围。这只是一个概述；“理解F#类型”系列将详细介绍类型。

首先，我们需要更多地了解类型表示法。我们已经看到箭头符号“->”用于显示域和范围。所以函数签名总是看起来像：

```F#
val functionName : domain -> range
```

以下是一些示例函数：

```F#
let intToString x = sprintf "x is %i" x  // format int to string
let stringToInt x = System.Int32.Parse(x)
```

如果你在F#交互窗口中进行评估，你会看到签名：

```F#
val intToString : int -> string
val stringToInt : string -> int
```

这意味着：

- `intToString` 有一个 int 域，它映射到范围字符串上。
- `stringToInt` 有一个字符串域，它映射到 int 范围。

## 原始类型

可能的原始类型是您所期望的：string、int、float、bool、char、byte等，以及从派生的更多类型。NET类型系统。

以下是使用基元类型的函数的更多示例：

```F#
let intToFloat x = float x // "float" fn. converts ints to floats
let intToBool x = (x = 2)  // true if x equals 2
let stringToString x = x + " world"
```

他们的签名是：

```F#
val intToFloat : int -> float
val intToBool : int -> bool
val stringToString : string -> string
```

## 类型注解

在前面的示例中，F# 编译器正确地确定了参数和结果的类型。但情况并非总是如此。如果你尝试以下代码，你会得到一个编译器错误：

```F#
let stringLength x = x.Length
   => error FS0072: Lookup on object of indeterminate type
```

编译器不知道“x”是什么类型，因此不知道“Length”是否是有效的方法。在大多数情况下，这可以通过给 F# 编译器一个“类型注释”来解决，这样它就知道要使用哪种类型。在下面的更正版本中，我们指出“x”的类型是字符串。

```F#
let stringLength (x:string) = x.Length
```

`x:string` 参数周围的括号很重要。如果它们缺失，编译器会认为返回值是一个字符串！也就是说，“open”冒号用于指示返回值的类型，如下面的示例所示。

```F#
let stringLengthAsInt (x:string) :int = x.Length
```

我们表示x参数是一个字符串，返回值是一个int。

## 函数类型作为参数

将其他函数作为参数或返回函数的函数称为**高阶函数**（有时缩写为 HOF）。它们被用作抽象出常见行为的一种方式。这类函数在 F# 中非常常见；大多数标准库都使用它们。

考虑一个函数 `evalWith5ThenAdd2`，它将函数作为参数，然后用值5计算函数，并将结果加2：

```F#
let evalWith5ThenAdd2 fn = fn 5 + 2     // same as fn(5) + 2
```

此函数的签名如下：

```F#
val evalWith5ThenAdd2 : (int -> int) -> int
```

你可以看到域是（`int->int`），范围是 `int`。这是什么意思？这意味着输入参数不是一个简单的值，而是一个函数，而且仅限于将 `int` 映射到 `int` 的函数。输出不是函数，只是一个int。

让我们试试：

```F#
let add1 x = x + 1      // define a function of type (int -> int)
evalWith5ThenAdd2 add1  // test it
```

得到：

```F#
val add1 : int -> int
val it : int = 8
```

“`add1`”是一个将int映射到int的函数，我们可以从它的签名中看到。因此，它是 `evalWith5TenAdd2` 函数的有效参数。结果是8。

顺便说一句，特殊的单词“`it`”用于最后一个被评估的东西；在这种情况下，我们想要的结果。这不是一个关键字，只是一个惯例。

这是另一个：

```F#
let times3 x = x * 3      // a function of type (int -> int)
evalWith5ThenAdd2 times3  // test it
```

得到：

```F#
val times3 : int -> int
val it : int = 17
```

“`times3`”也是一个将int映射到int的函数，我们可以从它的签名中看到。因此，它也是 `evalWith5TenAdd2` 函数的有效参数。结果是 17。

请注意，输入对类型很敏感。如果我们的输入函数使用浮点数而不是整数，它将无法工作。例如，如果我们有：

```F#
let times3float x = x * 3.0  // a function of type (float->float)
evalWith5ThenAdd2 times3float
```

对此进行评估将产生错误：

```F#
error FS0001: Type mismatch. Expecting a int -> int but
              given a float -> float
```

这意味着输入函数应该是 `int->int` 函数。

### 作为输出函数

函数值也可以是函数的输出。例如，以下函数将生成一个使用输入值进行加法的“加法器”函数。

```F#
let adderGenerator numberToAdd = (+) numberToAdd
```

签名为：

```F#
val adderGenerator : int -> (int -> int)
```

这意味着生成器接受一个 `int`，并创建一个将 `int` 映射到 `int` 的函数（“加法器”）。让我们看看它是如何工作的：

```F#
let add1 = adderGenerator 1
let add2 = adderGenerator 2
```

这将创建两个加法器函数。第一个生成的函数将其输入加1，第二个函数将其输出加2。请注意，签名正如我们所期望的那样。

```F#
val add1 : (int -> int)
val add2 : (int -> int)
```

我们现在可以以正常方式使用这些生成的函数。它们与明确定义的函数无法区分

```F#
add1 5    // val it : int = 6
add2 5    // val it : int = 7
```

### 使用类型注释约束函数类型

在第一个例子中，我们有一个函数：

```F#
let evalWith5ThenAdd2 fn = fn 5 +2
    => val evalWith5ThenAdd2 : (int -> int) -> int
```

在这种情况下，F#可以推断出“`fn`”将 `int` 映射为 `int`，因此它的签名将是 `int->int`

但是，在以下情况下，“fn”的签名是什么？

```F#
let evalWith5 fn = fn 5
```

显然，“`fn`”是一种接受 int 的函数，但它返回什么？编译器无法判断。如果确实要指定函数的类型，可以按照与基本类型相同的方式为函数参数添加类型注释。

```F#
let evalWith5AsInt (fn:int->int) = fn 5
let evalWith5AsFloat (fn:int->float) = fn 5
```

或者，您也可以指定返回类型。

```F#
let evalWith5AsString fn :string = fn 5
```

因为main函数返回字符串，所以“`fn`”函数也被约束为返回字符串，因此不需要对“fn“进行显式键入。

## “unit”类型

在编程时，我们有时希望函数在不返回值的情况下执行某些操作。考虑下面定义的函数“printInt”。该函数实际上没有返回任何东西。它只是在控制台上打印一个字符串作为副作用。

```F#
let printInt x = printf "x is %i" x        // print to console
```

那么这个函数的签名是什么？

```F#
val printInt : int -> unit
```

这个“`unit`”是什么？

好吧，即使一个函数没有返回输出，它仍然需要一个范围。数学世界中没有“空”函数。每个函数都必须有一些输出，因为函数是一个映射，映射必须有一些东西可以映射！



因此，在F#中，像这样的函数返回一个称为“`unit`”的特殊范围。此范围中只有一个值，称为“`()`”。你可以把`unit` 和 `()` 看作是C#中的“void”（类型）和“null”（值）。但与void/null不同，`unit` 是实数类型，（）是实数值。要看到这一点，请评估：

```F#
let whatIsThis = ()
```

您将看到签名：

```F#
val whatIsThis : unit = ()
```

这意味着值“`whatIsThis`”的类型为 `unit`，并且已绑定到值 `()`

所以，回到“`printInt`”的签名，我们现在可以理解它：

```F#
val printInt : int -> unit
```

这个签名说：`printInt` 有一个 `int` 域，它映射到我们不关心的任何东西上。

### 无参数函数

现在我们了解了这个单位，我们能预测它在其他环境中的出现吗？例如，让我们尝试创建一个可重用的“hello world”函数。由于没有输入也没有输出，我们希望它有一个签名 `unit->unit`。让我们看看：

```F#
let printHello = printf "hello world"        // print to console
```

结果是：

```F#
hello world
val printHello : unit = ()
```

与我们预期的不太一样。“Hello world”会立即打印出来，结果不是函数，而是一个简单的类型单位值。正如我们之前看到的，我们可以看出这是一个简单的值，因为它具有以下形式的签名：

```F#
val aName: type = constant
```

因此，在这种情况下，我们看到 `printHello` 实际上是一个具有值 `()` 的简单值。这不是一个我们可以再次调用的函数。

为什么 `printInt` 和 `printHello` 有区别？在 `printInt` 的情况下，在我们知道 x 参数的值之前，无法确定值，因此定义是一个函数。在 `printHello` 的情况下，没有参数，因此可以立即确定右侧。它是什么，返回 `()` 值，并附带向控制台打印的副作用。

我们可以通过强制定义有一个单元参数来创建一个真正的无参数可重用函数，如下所示：

```F#
let printHelloFn () = printf "hello world"    // print to console
```

现在签名是：

```F#
val printHelloFn : unit -> unit
```

要调用它，我们必须将 `()` 值作为参数传递，如下所示：

```F#
printHelloFn ()
```

### 使用忽略功能强制单位类型

在某些情况下，编译器需要一个单元类型，并会发出抱怨。例如，以下两种情况都是编译器错误：

```F#
do 1+1     // => FS0020: This expression should have type 'unit'

let something =
  2+2      // => FS0020: This expression should have type 'unit'
  "hello"
```

为了在这些情况下提供帮助，有一个特殊的函数 `ignore`，它接受任何东西并返回单位类型。此代码的正确版本为：

```F#
do (1+1 |> ignore)  // ok

let something =
  2+2 |> ignore     // ok
  "hello"
```

## 泛型类型

在许多情况下，函数参数的类型可以是任何类型，因此我们需要一种方法来指示这一点。F#使用 .NET 泛型类型系统用于这种情况。

例如，以下函数将参数转换为字符串并附加一些文本：

```F#
let onAStick x = x.ToString() + " on a stick"
```

参数是什么类型并不重要，因为所有对象都理解 `ToString()`。

签名为：

```F#
val onAStick : 'a -> string
```

这种类型被称为“`'a`”是什么？这是 F# 表示编译时未知的泛型类型的方式。“a”前面的撇号表示类型是泛型的。C#对应的签名如下：

```C#
string onAStick<a>();

//or more idiomatically
string OnAStick<TObject>();   // F#'s use of 'a is like
                              // C#'s "TObject" convention
```

请注意，F# 函数仍然是具有泛型类型的强类型函数。它不接受 `Object` 类型的参数。这种强类型是可取的，这样当函数组合在一起时，仍然可以保持类型安全。

这是一个用于int、float和字符串的相同函数

```F#
onAStick 22
onAStick 3.14159
onAStick "hello"
```

如果有两个泛型参数，编译器会给它们不同的名称：`'a` 代表第一个泛型，`'b` 代表第二个泛型，以此类推。以下是一个示例：

```F#
let concatString x y = x.ToString() + y.ToString()
```

这个的类型签名有两个泛型：`'a` 和 `'b`：

```F#
val concatString : 'a -> 'b -> string
```

另一方面，编译器将识别何时只需要一个泛型类型。在以下示例中，x 和 y 参数必须为同一类型：

```F#
let isEqual x y = (x=y)
```

因此，这两个函数的签名具有相同的泛型类型：

```F#
val isEqual : 'a -> 'a -> bool
```

当涉及到列表和更抽象的结构时，泛型参数也非常重要，我们将在接下来的示例中看到很多。

## 其他类型

到目前为止讨论的类型只是基本类型。这些类型可以以各种方式组合，以制作更复杂的类型。对这些类型的全面讨论将不得不等待下一系列文章，但与此同时，这里有一个简短的介绍，以便您在函数签名中识别它们。

- **“tuple”类型**。这些是其他类型的成对、三元组等。例如 `("hello"，1)` 是一个由字符串和 int 组成的元组。逗号是元组的区别特征——如果你在 F# 中看到逗号，它几乎肯定是元组的一部分！

  在函数签名中，元组被写为所涉及的两种类型的“乘法”。因此，在这种情况下，元组的类型为：

  ```F#
  string * int      // ("hello", 1)
  ```

- **集合类型**。其中最常见的是列表、序列和数组。列表和数组的大小是固定的，而序列可能是无限的（在幕后，序列与 `IEnumerable` 相同）。在函数签名中，它们有自己的关键字：“`list`”、“`seq`”和数组的“`[]`”。

  ```F#
  int list          // List type  e.g. [1;2;3]
  string list       // List type  e.g. ["a";"b";"c"]
  seq<int>          // Seq type   e.g. seq{1..10}
  int []            // Array type e.g. [|1;2;3|]
  ```

- **选项类型**。这是一个简单的包装器，用于包装可能缺失的对象。有两种情况：`Some` 和 `None`。在函数签名中，它们有自己的“`option`”关键字：

  ```F#
  int option        // Some(1)
  ```

- **可区分联合类型**。这些是基于一组其他类型的选项构建的。我们在“为什么使用F#？”系列中看到了一些这样的例子。在函数签名中，它们由类型的名称引用，因此没有特殊的关键字。

- **记录类型**。这些类似于结构或数据库行，是命名槽的列表。我们在“为什么使用F#？”系列中也看到了一些这样的例子。在函数签名中，它们是通过类型的名称引用的，所以也没有特殊的关键字。

## 测试你对类型的理解

你对这些类型了解多少？这里有一些表达方式——看看你能不能猜出它们的签名。要查看您是否正确，只需在交互式窗口中运行它们！

```F#
let testA   = float 2
let testB x = float 2
let testC x = float 2 + x
let testD x = x.ToString().Length
let testE (x:float) = x.ToString().Length
let testF x = printfn "%s" x
let testG x = printfn "%f" x
let testH   = 2 * 2 |> ignore
let testI x = 2 * 2 |> ignore
let testJ (x:int) = 2 * 2 |> ignore
let testK   = "hello"
let testL() = "hello"
let testM x = x=x
let testN x = x 1          // hint: what kind of thing is x?
let testO x:string = x 1   // hint: what does :string modify?
```



# 5 柯里化

*Part of the "Thinking functionally" series (*[link](https://fsharpforfunandprofit.com/posts/currying/#series-toc)*)*

将多参数函数分解为更小的单参数函数
05五月2012 这篇文章是超过3年

https://fsharpforfunandprofit.com/posts/currying/

在对基本类型稍作偏离之后，我们可以再次回到函数，特别是我们之前提到的难题：如果一个数学函数只能有一个参数，那么F#函数怎么可能有多个参数呢？

答案很简单：一个具有多个参数的函数被重写为一系列新函数，每个函数只有一个参数。这是编译器自动为您完成的。它被称为“currying”，以数学家Haskell Curry的名字命名，他对函数式编程的发展产生了重要影响。

为了了解这在实践中是如何工作的，让我们使用一个非常基本的例子来打印两个数字：

```F#
//normal version
let printTwoParameters x y =
   printfn "x=%i y=%i" x y
```

在内部，编译器将其重写为更像：

```F#
//explicitly curried version
let printTwoParameters x  =    // only one parameter!
   let subFunction y =
      printfn "x=%i y=%i" x y  // new function with one param
   subFunction                 // return the subfunction
```

让我们更详细地研究一下：

1. 构造名为“`printTwoParameters`”的函数，但只有一个参数：“x”
2. 在里面，构造一个只有一个参数的子函数：“y”。请注意，此内部函数使用“x”参数，但x没有作为参数显式传递给它。“x”参数在作用域内，因此内部函数可以看到它并使用它，而不需要传入它。
3. 最后，返回新创建的子函数。
4. 随后，该返回函数将用于处理“y”。“x”参数被烘焙到其中，因此返回的函数只需要y参数来完成函数逻辑。

通过以这种方式重写它，编译器确保了每个函数只需要一个参数。因此，当您使用“`printTwoParameters`”时，您可能会认为您使用的是一个双参数函数，但实际上它只是一个单参数函数！你可以通过只传递一个论点而不是两个论点来亲眼看到：

```F#
// eval with one argument
printTwoParameters 1

// get back a function!
val it : (int -> unit) = <fun:printTwoParameters@286-3>
```

如果你用一个参数来计算它，你不会得到错误，你会得到一个函数。

因此，当您使用两个参数调用 `printTwoParameters` 时，您真正要做的是：

- 使用第一个参数（x）调用 `printTwoParameters`
- `printTwoParameters` 返回一个包含“x”的新函数。
- 然后，您使用第二个参数（y）调用新函数

这是一个逐步版本的示例，然后是正常版本。

```F#
// step by step version
let x = 6
let y = 99
let intermediateFn = printTwoParameters x  // return fn with
                                           // x "baked in"
let result  = intermediateFn y

// inline version of above
let result  = (printTwoParameters x) y

// normal version
let result  = printTwoParameters x y
```

下面是另一个例子：

```F#
//normal version
let addTwoParameters x y =
   x + y

//explicitly curried version
let addTwoParameters x  =      // only one parameter!
   let subFunction y =
      x + y                    // new function with one param
   subFunction                 // return the subfunction

// now use it step by step
let x = 6
let y = 99
let intermediateFn = addTwoParameters x  // return fn with
                                         // x "baked in"
let result  = intermediateFn y

// normal version
let result  = addTwoParameters x y
```

同样，“双参数函数”实际上是一个返回中间函数的单参数函数。

但是等一下——“`+`”操作本身呢？这是一个必须有两个参数的二元运算，对吧？不，它像其他函数一样被curried。有一个名为“`+`”的函数，它接受一个参数并返回一个新的中间函数，就像上面的 `addTwoParameters` 一样。

当我们编写语句 `x+y` 时，编译器会重新排序代码以删除中缀，并将其转换为 `(+) x y`，这是一个名为 `+` 的函数，用两个参数调用。请注意，名为“+”的函数需要用括号括起来，以表明它被用作普通函数名，而不是中缀运算符。
最后，名为 `+` 的双参数函数被视为任何其他双参数函数。

```F#
// using plus as a single value function
let x = 6
let y = 99
let intermediateFn = (+) x     // return add with x baked in
let result  = intermediateFn y

// using plus as a function with two parameters
let result  = (+) x y

// normal version of plus as infix operator
let result  = x + y
```

是的，这适用于所有其他运算符和内置函数，如 printf。

```F#
// normal version of multiply
let result  = 3 * 5

// multiply as a one parameter function
let intermediateFn = (*) 3   // return multiply with "3" baked in
let result  = intermediateFn 5

// normal version of printfn
let result  = printfn "x=%i y=%i" 3 5

// printfn as a one parameter function
let intermediateFn = printfn "x=%i y=%i" 3  // "3" is baked in
let result  = intermediateFn 5
```

## curried函数的签名

既然我们知道了curried函数是如何工作的，那么我们应该期望它们的签名是什么样子的呢？

回到第一个例子“`printTwoParameters`”，我们看到它接受一个参数并返回一个中间函数。中间函数也接受了一个参数，但没有返回任何值（即unit）。因此，中间函数的类型为 `int->unit`。换句话说，`printTwoParameters` 的域是 `int`，范围是 `int->unit`。综上所述，我们可以看到最终的签名是：

```F#
val printTwoParameters : int -> (int -> unit)
```

如果你评估显式柯里化的实现，你会看到签名中的括号，如上所述，但如果你评估隐式柯里化的正常实现，括号会被省略，如下所示：

```F#
val printTwoParameters : int -> int -> unit
```

括号是可选的。如果你想理解函数签名，在脑海中重新添加它们可能会有所帮助。

此时，您可能想知道，返回中间函数的函数和常规双参数函数之间有什么区别？

这是一个返回函数的单参数函数：

```F#
let add1Param x = (+) x
// signature is = int -> (int -> int)
```

这是一个返回简单值的双参数函数：

```F#
let add2Params x y = (+) x y
// signature is = int -> int -> int
```

签名略有不同，但实际上没有区别，只是第二个函数会自动为您 curried。

## 具有两个以上参数的函数

currying 如何处理具有两个以上参数的函数？完全相同的方式：对于除最后一个参数之外的每个参数，该函数返回一个中间函数，其中包含前面的参数。

考虑一下这个人为的例子。我已经明确指定了参数的类型，但函数本身什么也不做。

```F#
let multiParamFn (p1:int)(p2:bool)(p3:string)(p4:float)=
   ()   //do nothing

let intermediateFn1 = multiParamFn 42
   // intermediateFn1 takes a bool
   // and returns a new function (string -> float -> unit)
let intermediateFn2 = intermediateFn1 false
   // intermediateFn2 takes a string
   // and returns a new function (float -> unit)
let intermediateFn3 = intermediateFn2 "hello"
   // intermediateFn3 takes a float
   // and returns a simple value (unit)
let finalResult = intermediateFn3 3.141
```

整体函数的签名是：

```F#
val multiParamFn : int -> bool -> string -> float -> unit
```

中间函数的签名为：

```F#
val intermediateFn1 : (bool -> string -> float -> unit)
val intermediateFn2 : (string -> float -> unit)
val intermediateFn3 : (float -> unit)
val finalResult : unit = ()
```

函数签名可以告诉你函数需要多少参数：只需计算括号外的箭头数量。如果函数接受或返回其他函数参数，括号中会有其他箭头，但可以忽略这些箭头。以下是一些示例：

```F#
int->int->int      // two int parameters and returns an int

string->bool->int  // first param is a string, second is a bool,
                   // returns an int

int->string->bool->unit // three params (int,string,bool)
                        // returns nothing (unit)

(int->string)->int      // has only one parameter, a function
                        // value (from int to string)
                        // and returns a int

(int->string)->(int->bool) // takes a function (int to string)
                           // returns a function (int to bool)
```

## 多参数问题

柯里化背后的逻辑可能会产生一些意想不到的结果，直到你理解它。记住，如果你计算一个参数比预期少的函数，你就不会出错。相反，您将返回一个部分应用的函数。如果你在期望值的上下文中继续使用这个部分应用的函数，你会从编译器那里得到模糊的错误消息。

这是一个看似无害的函数：

```F#
// create a function
let printHello() = printfn "hello"
```

当我们称之为如下所示时，您会期望发生什么？它会向控制台打印“你好”吗？在评估之前，请尝试猜测，这里有一个提示：一定要查看函数签名。

```F#
// call it
printHello
```

它不会像预期的那样被调用。原始函数需要一个未提供的 unit 参数，因此您得到的是一个部分应用的函数（在本例中没有参数）。

这个怎么样？它会编译吗？

```F#
let addXY x y =
    printfn "x=%i y=%i" x
    x + y
```

如果你对它进行评估，你会看到编译器对printfn行有抱怨。

```F#
printfn "x=%i y=%i" x
//^^^^^^^^^^^^^^^^^^^^^
//warning FS0193: This expression is a function value, i.e. is missing
//arguments. Its type is  ^a -> unit.
```

如果你不理解currying，这条消息会非常神秘！所有像这样独立计算的表达式（即不用作返回值或用“let”绑定到某物）都必须计算为单位值。在这种情况下，它不是计算单位值，而是计算函数。这是一种冗长的说法，说 `printfn` 缺少一个论点。

此类错误的一个常见情况是在与 .NET 库进行接口连接时。例如，`TextReader` 的 `ReadLine` 方法必须接受一个单位参数。通常很容易忘记这一点并省略括号，在这种情况下，你不会立即得到编译器错误，而只有当你试图将结果视为字符串时才会出现。

```F#
let reader = new System.IO.StringReader("hello");

let line1 = reader.ReadLine        // wrong but compiler doesn't
                                   // complain
printfn "The line is %s" line1     //compiler error here!
// ==> error FS0001: This expression was expected to have
// type string but here has type unit -> string

let line2 = reader.ReadLine()      //correct
printfn "The line is %s" line2     //no compiler error
```

在上面的代码中，`line1` 只是 `Readline` 方法的指针或委托，而不是我们期望的字符串。在 `reader.ReadLine()` 中使用 `()` 实际执行该函数。

## 参数太多

当你有太多的参数时，你也会得到类似的神秘消息。以下是一些向printf传递过多参数的示例。

```F#
printfn "hello" 42
// ==> error FS0001: This expression was expected to have
//                   type 'a -> 'b but here has type unit

printfn "hello %i" 42 43
// ==> Error FS0001: Type mismatch. Expecting a 'a -> 'b -> 'c
//                   but given a 'a -> unit

printfn "hello %i %i" 42 43 44
// ==> Error FS0001: Type mismatch. Expecting a 'a->'b->'c->'d
//                   but given a 'a -> 'b -> unit
```

例如，在最后一种情况下，编译器表示它期望格式参数有三个参数（签名 `'a->'b->'c->'d` 有三个变量），但只给出了两个参数（标记 `'a->'b->unit` 有两个参数）。

在不使用 `printf` 的情况下，传递太多的参数通常意味着你最终会得到一个简单的值，然后试图传递一个参数。编译器会抱怨这个简单的值不是函数。

```F#
let add1 x = x + 1
let x = add1 2 3
// ==>   error FS0003: This value is not a function
//                     and cannot be applied
```

如果你像前面一样将调用分解为一系列显式的中间函数，你就可以准确地看到出了什么问题。

```F#
let add1 x = x + 1
let intermediateFn = add1 2   //returns a simple value
let x = intermediateFn 3      //intermediateFn is not a function!
// ==>   error FS0003: This value is not a function
//                     and cannot be applied
```

# 6 部分应用

*Part of the "Thinking functionally" series (*[link](https://fsharpforfunandprofit.com/posts/partial-application/#series-toc)*)*

烘焙函数的某些参数
06五月2012 这篇文章是超过3年

https://fsharpforfunandprofit.com/posts/partial-application/

在上一篇关于currying的文章中，我们研究了如何将多个参数函数分解为更小的单参数函数。这是数学上正确的方法，但这不是唯一的原因——它还导致了一种非常强大的技术，称为**偏函数应用**。这是函数式编程中一种非常广泛使用的风格，理解它很重要。

部分应用的想法是，如果你固定了函数的前 N 个参数，你就得到了其余参数的函数。从关于柯里化的讨论中，你可能会看到这是如何自然发生的。

以下是一些简单的例子来证明这一点：

```F#
// create an "adder" by partial application of add
let add42 = (+) 42    // partial application
add42 1
add42 3

// create a new list by applying the add42 function
// to each element
[1;2;3] |> List.map add42

// create a "tester" by partial application of "less than"
let twoIsLessThan = (<) 2   // partial application
twoIsLessThan 1
twoIsLessThan 3

// filter each element with the twoIsLessThan function
[1;2;3] |> List.filter twoIsLessThan

// create a "printer" by partial application of printfn
let printer = printfn "printing param=%i"

// loop over each element and call the printer function
[1;2;3] |> List.iter printer
```

在每种情况下，我们创建一个部分应用的函数，然后可以在多个上下文中重用。

当然，部分应用程序也可以很容易地涉及固定函数参数。以下是一些示例：

```F#
// an example using List.map
let add1 = (+) 1
let add1ToEach = List.map add1   // fix the "add1" function

// test
add1ToEach [1;2;3;4]

// an example using List.filter
let filterEvens =
  List.filter (fun i -> i%2 = 0) // fix the filter function

// test
filterEvens [1;2;3;4]
```

以下更复杂的示例显示了如何使用相同的方法来创建透明的“插件”行为。

- 我们创建了一个将两个数字相加的函数，但除此之外，还需要一个记录函数来记录这两个数字和结果。
- 日志函数有两个参数：（string）“name”和（generic）“value”，因此它有签名 `string->'a->unit`。
- 然后，我们创建日志功能的各种实现，例如控制台记录器或彩色记录器。
- 最后，我们部分应用 main 函数来创建新函数，这些函数中包含一个特定的记录器。

```F#
// create an adder that supports a pluggable logging function
let adderWithPluggableLogger logger x y =
  logger "x" x
  logger "y" y
  let result = x + y
  logger "x+y"  result
  result

// create a logging function that writes to the console
let consoleLogger argName argValue =
  printfn "%s=%A" argName argValue

//create an adder with the console logger partially applied
let addWithConsoleLogger = adderWithPluggableLogger consoleLogger
addWithConsoleLogger 1 2
addWithConsoleLogger 42 99

// create a logging function that uses red text
let redLogger argName argValue =
  let message = sprintf "%s=%A" argName argValue
  System.Console.ForegroundColor <- System.ConsoleColor.Red
  System.Console.WriteLine("{0}",message)
  System.Console.ResetColor()

//create an adder with the popup logger partially applied
let addWithRedLogger = adderWithPluggableLogger redLogger
addWithRedLogger 1 2
addWithRedLogger 42 99
```

这些内置记录器的功能可以像其他功能一样使用。例如，我们可以创建一个部分应用程序来添加42，然后将其传递给列表函数，就像我们对简单的“`add42`”函数所做的那样。

```F#
// create a another adder with 42 baked in
let add42WithConsoleLogger = addWithConsoleLogger 42
[1;2;3] |> List.map add42WithConsoleLogger
[1;2;3] |> List.map add42               //compare without logger
```

这些部分应用的函数是一个非常有用的工具。我们可以创建灵活（但复杂）的库函数，同时使创建可重用的默认值变得容易，这样调用者就不必一直暴露在复杂性中。

## 为部分应用程序设计函数

您可以看到，参数的顺序对部分应用程序的易用性有很大影响。例如，`List` 库中的大多数函数，如 `List.map` 和 `List.filter`，都有类似的形式，即：

`List-function [function parameter(s)] [list]`

列表始终是最后一个参数。以下是完整表格的一些示例：

```F#
List.map    (fun i -> i+1) [0;1;2;3]
List.filter (fun i -> i>1) [0;1;2;3]
List.sortBy (fun i -> -i ) [0;1;2;3]
```

使用部分应用程序的相同示例：

```F#
let eachAdd1 = List.map (fun i -> i+1)
eachAdd1 [0;1;2;3]

let excludeOneOrLess = List.filter (fun i -> i>1)
excludeOneOrLess [0;1;2;3]

let sortDesc = List.sortBy (fun i -> -i)
sortDesc [0;1;2;3]
```

如果库函数以不同的顺序编写参数，那么在部分应用程序中使用它们会更加不便。

当你编写自己的多参数函数时，你可能会想知道最佳的参数顺序是什么。与所有设计问题一样，这个问题没有“正确”的答案，但这里有一些普遍接受的准则：

1. 放在前面：参数更有可能是静态的
2. 放在最后：数据结构或集合（或变化最大的参数）
3. 对于众所周知的操作，如“减法”，按预期顺序排列

准则1直截了当。最有可能在部分应用中“固定”的参数应该是第一个。我们在前面的记录器示例中看到了这一点。

准则2使将结构或集合从一个功能传输到另一个功能变得更加容易。我们已经在列表函数中多次看到了这一点。

```F#
// piping using list functions
let result =
  [1..10]
  |> List.map (fun i -> i+1)
  |> List.filter (fun i -> i>5)
// output => [6; 7; 8; 9; 10; 11]
```

同样，部分应用的列表函数很容易组合，因为列表参数本身很容易省略：

```F#
let f1 = List.map (fun i -> i+1)
let f2 = List.filter (fun i -> i>5)
let compositeOp = f1 >> f2 // compose
let result = compositeOp [1..10]
// output => [6; 7; 8; 9; 10; 11]
```

### 包装BCL功能以供部分应用

这个 .NET 基类库函数在 F# 中很容易访问，但并不是真正为与 F# 等函数式语言一起使用而设计的。例如，大多数函数首先有data参数，而对于 F#，正如我们所看到的，data参数通常应该排在最后。

然而，为它们创建更符合习惯的包装器很容易。例如，在下面的代码片段中，.NET 字符串函数被重写，使字符串目标成为最后一个参数，而不是第一个参数：

```F#
// create wrappers for .NET string functions
let replace oldStr newStr (s:string) =
  s.Replace(oldValue=oldStr, newValue=newStr)

let startsWith (lookFor:string) (s:string) =
  s.StartsWith(lookFor)
```

一旦字符串成为最后一个参数，我们就可以以预期的方式将它们与管道一起使用：

```F#
let result =
  "hello"
  |> replace "h" "j"
  |> startsWith "j"

["the"; "quick"; "brown"; "fox"]
  |> List.filter (startsWith "f")
```

或具有函数组成：

```F#
let compositeOp = replace "h" "j" >> startsWith "j"
let result = compositeOp "hello"
```

### 理解“管道”功能

现在您已经了解了部分应用程序的工作原理，您应该能够理解“管道”函数的工作原理。

管道功能定义为：

```F#
let (|>) x f = f x
```

它所做的只是允许您将函数参数放在函数前面，而不是后面。仅此而已。

```F#
let doSomething x y z = x+y+z
doSomething 1 2 3       // all parameters after function
```

如果函数有多个参数，那么输入似乎是最后一个参数。实际上，函数被部分应用，返回一个只有一个参数的函数：输入

下面是重写为使用部分应用程序的相同示例

```F#
let doSomething x y  =
  let intermediateFn z = x+y+z
  intermediateFn        // return intermediateFn

let doSomethingPartial = doSomething 1 2
doSomethingPartial 3     // only one parameter after function now
3 |> doSomethingPartial  // same as above - last parameter piped in
```

正如您已经看到的，管道操作符在F#中非常常见，并且一直用于保持自然流动。以下是您可能会看到的一些用法：

```F#
"12" |> int               // parses string "12" to an int
1 |> (+) 2 |> (*) 3       // chain of arithmetic
```

### 反向管道功能

您可能偶尔会看到使用反向管道函数“<|”。

```F#
let (<|) f x = f x
```

这个函数似乎并没有做任何与正常函数不同的事情，那么它为什么存在呢？

原因是，当在中缀样式中用作二进制运算符时，它减少了对括号的需求，可以使代码更清晰。

```F#
printf "%i" 1+2          // error
printf "%i" (1+2)        // using parens
printf "%i" <| 1+2       // using reverse pipe
```

您还可以同时在两个方向上使用管道来获得伪中缀符号。

```F#
let add x y = x + y
(1+2) add (3+4)          // error
1+2 |> add <| 3+4        // pseudo infix
```

# 7 函数关联性和组合

*Part of the "Thinking functionally" series (*[link](https://fsharpforfunandprofit.com/posts/function-composition/#series-toc)*)*

从现有功能构建新功能
07五月2012 这篇文章是超过3年

## 函数关联性

如果我们有一排函数链，它们是如何组合的？

例如，这意味着什么？

```F#
let F x y z = x y z
```

这是否意味着将函数y应用于参数z，然后获取结果并将其用作x的参数？在这种情况下，它与以下内容相同：

```F#
let F x y z = x (y z)
```

或者，这是否意味着将函数x应用于参数y，然后获取结果函数并用参数z对其进行求值？在这种情况下，它与以下内容相同：

```F#
let F x y z = (x y) z
```

答案是后者。函数应用程序是左关联的。也就是说，评估 `x y z`与评估 `(x y) z` 是相同的。评估 `w x y z` 也与评估 `((w x) y) z` 相同。这并不奇怪。我们已经看到，这就是部分应用程序的工作原理。如果你把 x 看作一个双参数函数，那么 `(x y) z` 是第一个参数部分应用的结果，然后将 z 参数传递给中间函数。

如果您确实想进行正确的关联，可以使用显式括号，也可以使用管道。以下三种形式是等效的。

```F#
let F x y z = x (y z)
let F x y z = y z |> x    // using forward pipe
let F x y z = x <| y z    // using backward pipe
```

作为练习，在不实际评估的情况下计算出这些函数的签名！

## 函数组合

我们现在已经多次提到函数组合，但它实际上是什么意思？起初看起来很吓人，但实际上很简单。

假设你有一个从类型“T1”映射到类型“T2”的函数“f”，并且假设你还有一个从“T2”映射到“T3”类型的函数“g”。然后，您可以将“f”的输出连接到“g”的输入，创建一个从类型“T1”映射到类型“T3”的新函数。

这里是例子

```F#
let f (x:int) = float x * 3.0  // f is int->float
let g (x:float) = x > 4.0      // g is float->bool
```

我们可以创建一个新函数h，它接受“f”的输出并将其用作“g”的输入。

```F#
let h (x:int) =
    let y = f(x)
    g(y)                   // return output of g
```

一种更紧凑的方式是：

```F#
let h (x:int) = g ( f(x) ) // h is int->bool

//test
h 1
h 2
```

到目前为止，一切都很简单。有趣的是，我们可以定义一个名为“compose”的新函数，给定函数“f”和“g”，它以这种方式组合它们，甚至不知道它们的签名。

```F#
let compose f g x = g ( f(x) )
```

如果你对此进行评估，你会看到编译器已经正确地推断出，如果“`f`”是一个从泛型类型 `'a` 到泛型类型 `'b` 的函数，那么“`g`”被约束为具有泛型类型 `'b` 作为输入。总体签名为：

```F#
val compose : ('a -> 'b) -> ('b -> 'c) -> 'a -> 'c
```

（请注意，这种通用组合操作之所以可能，是因为每个函数都有一个输入和一个输出。这种方法在非函数式语言中是不可能的。）

正如我们所看到的，compose 的实际定义使用了“`>>`”符号。

```F#
let (>>) f g x = g ( f(x) )
```

根据这个定义，我们现在可以使用组合从现有函数构建新函数。

```F#
let add1 x = x + 1
let times2 x = x * 2
let add1Times2 x = (>>) add1 times2 x

//test
add1Times2 3
```

这种露骨的风格相当杂乱。我们可以做一些事情来让它更容易使用和理解。

首先，我们可以省略x参数，以便组合运算符返回部分应用程序。

```F#
let add1Times2 = (>>) add1 times2
```

现在我们有一个二元运算，所以我们可以把运算符在中间。

```F#
let add1Times2 = add1 >> times2
```

这就是了。使用组合运算符可以使代码更清晰、更直接。

```F#
let add1 x = x + 1
let times2 x = x * 2

//old style
let add1Times2 x = times2(add1 x)

//new style
let add1Times2 = add1 >> times2
```

## 在实践中使用组合运算符

组合运算符（与所有中缀运算符一样）的优先级低于普通函数应用程序。这意味着组合中使用的函数可以有参数，而不需要使用括号。

例如，如果“add”和“times”函数有一个额外的参数，则可以在组合过程中传递该参数。

```F#
let add n x = x + n
let times n x = x * n
let add1Times2 = add 1 >> times 2
let add5Times3 = add 5 >> times 3

//test
add5Times3 1
```

只要输入和输出匹配，所涉及的函数就可以使用任何类型的值。例如，考虑以下情况，它执行一个函数两次：

```F#
let twice f = f >> f    //signature is ('a -> 'a) -> ('a -> 'a)
```

请注意，编译器已经推断出函数f必须对输入和输出使用相同的类型。

现在考虑一个像“`+`”这样的函数。正如我们之前看到的，输入是一个 `int`，但输出实际上是一个部分应用的函数（`int->int`）。因此，“`+`”的输出可以用作“`twice`”的输入。所以我们可以写这样的东西：

```F#
let add1 = (+) 1           // signature is (int -> int)
let add1Twice = twice add1 // signature is also (int -> int)

//test
add1Twice 9
```

另一方面，我们不能写这样的东西：

```F#
let addThenMultiply = (+) >> (*)
```

因为“*”的输入必须是 `int` 值，而不是 `int->int` 函数（这是加法的输出）。

但如果我们调整它，使第一个函数的输出仅为 `int`，那么它确实有效：

```F#
let add1ThenMultiply = (+) 1 >> (*)
// (+) 1 has signature (int -> int) and output is an 'int'

//test
add1ThenMultiply 2 7
```

如果需要，也可以使用“`<<`”运算符反向组合。

```F#
let times2Add1 = add 1 << times 2
times2Add1 3
```

反向组合主要用于使代码更像英语。例如，这里有一个简单的例子：

```F#
let myList = []
myList |> List.isEmpty |> not    // straight pipeline

myList |> (not << List.isEmpty)  // using reverse composition
```

## 组合 vs 管道

此时，您可能想知道组合运算符和管道运算符之间的区别是什么，因为它们看起来非常相似。

首先，让我们再次看看管道操作符的定义：

```F#
let (|>) x f = f x
```

它所做的只是允许您将函数参数放在函数前面，而不是后面。仅此而已。如果函数有多个参数，那么输入将是最后一个参数。这是前面使用的示例。

```F#
let doSomething x y z = x+y+z
doSomething 1 2 3       // all parameters after function
3 |> doSomething 1 2    // last parameter piped in
```

组合不是一回事，不能代替管道。在以下情况下，数字3甚至不是一个函数，因此它的“输出”不能馈入`doSomething`：

```F#
3 >> doSomething 1 2     // not allowed
// f >> g is the same as  g(f(x)) so rewriting it we have:
doSomething 1 2 ( 3(x) ) // implies 3 should be a function!
// error FS0001: This expression was expected to have type 'a->'b
//               but here has type int
```

编译器抱怨“3”应该是某种函数 `'a->'b`。

与此相比，组合的定义需要 3 个参数，其中前两个必须是函数。

```F#
let (>>) f g x = g ( f(x) )

let add n x = x + n
let times n x = x * n
let add1Times2 = add 1 >> times 2
```

尝试使用管道是行不通的。在下面的例子中，“`add 1`”是一个 `int->int` 类型的（部分）函数，不能用作“`times 2`”的第二个参数。

```F#
let add1Times2 = add 1 |> times 2   // not allowed
// x |> f is the same as  f(x) so rewriting it we have:
let add1Times2 = times 2 (add 1)    // add1 should be an int
// error FS0001: Type mismatch. 'int -> int' does not match 'int'
```

编译器抱怨“`times 2`”应该接受一个 `int->int` 参数，即类型为 `(int->int) ->'a`。

# 8 定义函数

*Part of the "Thinking functionally" series (*[link](https://fsharpforfunandprofit.com/posts/defining-functions/#series-toc)*)*

Lambdas和更多
08五月2012 这篇文章是超过3年

https://fsharpforfunandprofit.com/posts/defining-functions/

我们已经看到了如何使用“let”语法创建典型函数，如下所示：

```F#
let add x y = x + y
```

在本节中，我们将介绍创建函数的其他方法以及定义函数的技巧。

## 匿名函数（又名lambdas）

如果你熟悉其他语言中的 lambdas，这对你来说并不陌生。匿名函数（或“lambda 表达式”）使用以下形式定义：

```F#
fun parameter1 parameter2 etc -> expression
```

如果你习惯 C# 中的 lambdas，有几个不同之处：

- lambda 必须有特殊的关键字 `fun`，这在 C# 版本中是不需要的
- 箭头符号是一个单箭头 `->`，而不是 C# 中的双箭头（`=>`）。

下面是一个定义加法的 lambda：

```F#
let add = fun x y -> x + y
```

这与更传统的函数定义完全相同：

```F#
let add x y = x + y
```

当你有一个简短的表达式，并且不想只为该表达式定义一个函数时，通常会使用 Lambdas。正如我们已经看到的，这在列表操作中尤其常见。

```F#
// with separately defined function
let add1 i = i + 1
[1..10] |> List.map add1

// inlined without separately defined function
[1..10] |> List.map (fun i -> i + 1)
```

请注意，必须在 lambda 周围使用括号。

当你想清楚地表明你是从另一个函数返回一个函数时，也会使用 Lambdas。例如，我们之前讨论的“`adderGenerator`”函数可以用lambda重写。

```F#
// original definition
let adderGenerator x = (+) x

// definition using lambda
let adderGenerator x = fun y -> x + y
```

lambda 版本稍长，但明确表示返回的是一个中间函数。

你也可以嵌套 lambda。这是 `adderGenerator` 的另一个定义，这次只使用 lambdas。

```F#
let adderGenerator = fun x -> (fun y -> x + y)
```

你能看出以下三个定义都是一样的吗？

```F#
let adderGenerator1 x y = x + y
let adderGenerator2 x   = fun y -> x + y
let adderGenerator3     = fun x -> (fun y -> x + y)
```

如果你看不到，那么一定要重读这篇关于柯里化的文章。这是需要了解的重要内容！

## 参数模式匹配

在定义函数时，您可以传递一个显式参数，正如我们所看到的，但您也可以在参数部分直接进行模式匹配。换句话说，参数部分可以包含模式，而不仅仅是标识符！

以下示例演示了如何在函数定义中使用模式：

```F#
type Name = {first:string; last:string} // define a new type
let bob = {first="bob"; last="smith"}   // define a value

// single parameter style
let f1 name =                       // pass in single parameter
   let {first=f; last=l} = name     // extract in body of function
   printfn "first=%s; last=%s" f l

// match in the parameter itself
let f2 {first=f; last=l} =          // direct pattern matching
   printfn "first=%s; last=%s" f l

// test
f1 bob
f2 bob
```

这种匹配只有在匹配总是可能的情况下才能发生。例如，您无法以这种方式在联合类型或列表上进行匹配，因为某些情况可能不匹配。

```F#
let f3 (x::xs) =            // use pattern matching on a list
   printfn "first element is=%A" x
```

您将收到关于不完整模式匹配的警告。

## 一个常见的错误：元组与多个参数

如果你来自类C语言，用作单个函数参数的元组可能看起来非常像多个参数。它们根本不是一回事！正如我之前提到的，如果你看到一个逗号，它可能是元组的一部分。参数之间用空格分隔。

下面是一个混淆的例子：

```F#
// a function that takes two distinct parameters
let addTwoParams x y = x + y

// a function that takes a single tuple parameter
let addTuple aTuple =
   let (x,y) = aTuple
   x + y

// another function that takes a single tuple parameter
// but looks like it takes two ints
let addConfusingTuple (x,y) = x + y
```

- 第一个定义“`addTwoParams`”接受两个参数，用空格分隔。
- 第二个定义“`addTuple`”只接受一个参数。然后，它将“x”和“y”绑定到元组内部并进行加法。
- 第三个定义“`addConfusingTuple`”与“`addTuple`”一样只接受一个参数，但棘手的是，元组是使用模式匹配作为参数定义的一部分进行解包和绑定的。在幕后，它与“`addTuple`”完全相同。

让我们看看签名（如果你不确定，最好看看签名）

```F#
val addTwoParams : int -> int -> int        // two params
val addTuple : int * int -> int             // tuple->int
val addConfusingTuple : int * int -> int    // tuple->int
```

现在让我们使用它们：

```F#
//test
addTwoParams 1 2      // ok - uses spaces to separate args
addTwoParams (1,2)    // error trying to pass a single tuple
//   => error FS0001: This expression was expected to have type
//                    int but here has type 'a * 'b
```

在这里，我们可以看到在上述第二种情况下发生了错误。

首先，编译器将 `(1,2)` 视为 `('a * 'b)` 类型的泛型元组，它试图将其作为第一个参数传递给“`addTwoParams`”。然后它抱怨 `addTwoParams` 的第一个参数是一个 `int`，我们试图传递一个元组。

要创建元组，请使用逗号！以下是正确操作的方法：

```F#
addTuple (1,2)           // ok
addConfusingTuple (1,2)  // ok

let x = (1,2)
addTuple x               // ok

let y = 1,2              // it's the comma you need,
                         // not the parentheses!
addTuple y               // ok
addConfusingTuple y      // ok
```

相反，如果你试图向一个需要元组的函数传递多个参数，你也会得到一个模糊的错误。

```F#
addConfusingTuple 1 2    // error trying to pass two args
// => error FS0003: This value is not a function and
//                  cannot be applied
```

在这种情况下，编译器认为，由于您传递了两个参数，`addConfucingTuple` 必须是可 curryable 的。因此，“`addConfucingTuple 1`”将是一个返回另一个中间函数的部分应用程序。试图用“2”应用该中间函数会出错，因为没有中间函数！我们在关于currying的帖子中看到了完全相同的错误，当时我们讨论了参数过多可能导致的问题。

### 为什么不使用元组作为参数？

上面对元组问题的讨论表明，还有另一种方法可以定义具有多个参数的函数：与其单独传递它们，不如将所有参数组合成一个复合数据结构。在下面的示例中，该函数接受一个参数，即一个包含三个项的元组。

```F#
let f (x,y,z) = x + y * z
// type is int * int * int -> int

// test
f (1,2,3)
```

请注意，函数签名不同于真正的三参数函数。只有一个箭头，所以只有一个参数，星号表示这是一个 `(int * int * int)` 的元组。

我们什么时候想使用元组参数而不是单个参数？

- 当元组本身有意义时。例如，如果我们使用三维坐标，三元组可能比三个单独的维度更方便。
- 元组偶尔用于将数据捆绑在一个应该保存在一起的单一结构中。例如，.NET 库中的 `TryParse` 函数返回结果和一个布尔值作为元组。但是，如果您有大量数据作为捆绑包保存在一起，那么您可能需要定义一个记录或类类型来存储它。

### 一个特例：元组和 .NET 库函数

逗号经常出现的一个领域是调用 .NET库函数时！

这些都采用类似元组的参数，因此这些调用看起来与 C# 中的调用完全相同：

```F#
// correct
System.String.Compare("a","b")

// incorrect
System.String.Compare "a" "b"
```

原因是 .NET 库函数不是 curried 的，不能部分应用。所有参数都必须始终传入，使用类似元组的方法是实现这一点的明显方法。

但请注意，尽管这些调用看起来像元组，但它们实际上是一个特例。不能使用真的元组，因此以下代码无效：

```F#
let tuple = ("a","b")
System.String.Compare tuple   // error

System.String.Compare "a","b" // error
```

如果你想部分应用 .NET 库函数，为它们编写包装器函数通常很简单，正如我们前面看到的，如下所示：

```F#
// create a wrapper function
let strCompare x y = System.String.Compare(x,y)

// partially apply it
let strCompareWithB = strCompare "B"

// use it with a higher order function
["A";"B";"C"]
|> List.map strCompareWithB
```

## 单独参数与分组参数的指南

关于元组的讨论将我们引向一个更一般的话题：函数参数何时应该分开，何时应该分组？

请注意，F# 在这方面与 C# 不同。在 C# 中，所有的参数都是提供的，所以这个问题甚至不会出现！在 F# 中，由于部分应用，可能只提供了一些参数，因此您需要区分需要组合在一起的参数和独立的参数。

以下是在设计自己的函数时如何构造参数的一些一般准则。

- 一般来说，最好使用单独的参数，而不是将它们作为单个结构（如元组或记录）传递。这允许更灵活的行为，如部分应用。
- 但是，当一组参数必须同时设置时，请使用某种分组机制。

换句话说，在设计函数时，问问自己“我可以单独提供这个参数吗？”如果答案是否定的，那么应该对参数进行分组。

让我们来看一些例子：

```F#
// Pass in two numbers for addition.
// The numbers are independent, so use two parameters
let add x y = x + y

// Pass in two numbers as a geographical co-ordinate.
// The numbers are dependent, so group them into a tuple or record
let locateOnMap (xCoord,yCoord) = // do something

// Set first and last name for a customer.
// The values are dependent, so group them into a record.
type CustomerName = {First:string; Last:string}
let setCustomerName aCustomerName = // good
let setCustomerName first last = // not recommended

// Set first and last name and and pass the
// authorizing credentials as well.
// The name and credentials are independent, keep them separate
let setCustomerName myCredentials aName = //good
```

最后，一定要对参数进行适当的排序，以帮助部分应用（请参阅前面文章中的指导方针）。例如，在上面的最后一个函数中，为什么我把 `myCredentials` 参数放在 `aName` 参数之前？

## 无参数功能

有时我们可能希望函数根本不接受任何参数。例如，我们可能需要一个可以重复调用的“hello world”函数。正如我们在上一节中看到的，幼稚的定义是行不通的。

```F#
let sayHello = printfn "Hello World!"     // not what we want
```

修复方法是在函数中添加一个单元参数，或使用 lambda。

```F#
let sayHello() = printfn "Hello World!"           // good
let sayHello = fun () -> printfn "Hello World!"   // good
```

然后，必须始终使用 unit 参数调用函数：

```F#
// call it
sayHello()
```

这在 .NET 库以下情况中尤为常见。一些例子是：

```F#
Console.ReadLine()
System.Environment.GetCommandLineArgs()
System.IO.Directory.GetCurrentDirectory()
```

记得用 unit 参数调用它们！

## 定义新操作符

您可以定义使用一个或多个运算符符号命名的函数（有关可以使用的确切符号列表，请参阅F#文档）：

```F#
// define
let (.*%) x y = x + y + 1
```

定义符号时，必须在符号周围使用括号。

请注意，对于以 `*` 开头的自定义运算符，需要空格；否则 `(*` 被解释为评论的开头：

```F#
let ( *+* ) x y = x + y + 1
```

定义后，新函数可以以正常方式使用，符号周围也有括号：

```F#
let result = (.*%) 2 3
```

如果函数恰好有两个参数，则可以将其用作不带括号的中缀运算符。

```F#
let result = 2 .*% 3
```

您还可以定义以 `!` 或 `~` 开头的前缀运算符（有一些限制——请参阅F#关于运算符重载的文档）

```F#
let (~%%) (s:string) = s.ToCharArray()

//use
let result = %% "hello"
```

在 F# 中，创建自己的运算符是很常见的，许多库都会导出名为 `>=>` 和 `<*>` 的运算符。

## 无点风格

我们已经看到了许多省略函数最后一个参数以减少混乱的例子。这种风格被称为**无点风格**（point-free style）或**隐性编程**（tacit programming）。

以下是一些示例：

```F#
let add x y = x + y   // explicit
let add x = (+) x     // point free

let add1Times2 x = (x + 1) * 2    // explicit
let add1Times2 = (+) 1 >> (*) 2   // point free

let sum list = List.reduce (fun sum e -> sum+e) list // explicit
let sum = List.reduce (+)                            // point free
```

这种风格有利有弊。

从好的方面来看，它将注意力集中在高级功能组合上，而不是低级对象上。例如，“`(+) 1 >> (*) 2`”显然是一个加法运算，然后是乘法。“`List.reduce (+)`”明确表示加号操作是关键，而不需要知道它实际应用于的列表。

无点有助于阐明底层算法并揭示代码之间的共性——上面使用的“`reduce`”函数就是一个很好的例子——它将在计划中的列表处理系列中进行讨论。

另一方面，过多的无点风格会导致代码混乱。显式参数可以作为一种文档形式，它们的名称（如“list”）清楚地表明了函数的作用。

与编程中的任何事情一样，最好的指导方针是使用最清晰的方法。

## 组合子

“**组合子**”一词用于描述其结果仅取决于其参数的函数。这意味着不依赖于外部世界，特别是根本无法访问其他函数或全局值。

在实践中，这意味着组合子函数仅限于以各种方式组合其参数。

我们已经看到了一些组合子：“管道”运算符和“组合”运算符。如果你看看它们的定义，很明显，它们所做的只是以各种方式重新排序参数

```F#
let (|>) x f = f x             // forward pipe
let (<|) f x = f x             // reverse pipe
let (>>) f g x = g (f x)       // forward composition
let (<<) g f x = g (f x)       // reverse composition
```

另一方面，像“printf”这样的函数虽然很原始，但不是组合子，因为它依赖于外部世界（I/O）。

### 组合鸟

组合子是逻辑的一个完整分支（自然称为“组合逻辑”）的基础，该分支比计算机和编程语言早很多年被发明。组合逻辑对函数式编程产生了很大的影响。

要了解更多关于组合子和组合逻辑的知识，我推荐雷蒙德·斯穆里安的《嘲笑一只知更鸟》一书。在书中，他描述了许多其他组合子，并异想天开地给它们起了鸟的名字。以下是一些标准组合子及其鸟名的示例：

```F#
let I x = x                // identity function, or the Idiot bird
let K x y = x              // the Kestrel
let M x = x >> x           // the Mockingbird
let T x y = y x            // the Thrush (this looks familiar!)
let Q x y z = y (x z)      // the Queer bird (also familiar!)
let S x y z = x z (y z)    // The Starling
// and the infamous...
let rec Y f x = f (Y f) x  // Y-combinator, or Sage bird
```

字母名称非常标准，所以如果你提到“K组合子”，每个人都会熟悉这个术语。

事实证明，许多常见的编程模式都可以用这些标准组合子来表示。例如，Kestrel是流畅界面中的一种常见模式，在这种模式下，你做了一些事情，但随后返回了原始对象。Thrush是管道操作，Queer bird是正向组合，Y组合子被用来使函数递归。

事实上，有一个众所周知的定理指出，任何可计算函数都可以仅由两个基本组合子Kestrel和Starling构建。

### Combinator库

组合子库是一个代码库，它导出一组旨在协同工作的组合子函数。然后，库的用户可以轻松地将简单的功能组合在一起，制作更大、更复杂的功能，比如用乐高积木建造。

一个设计良好的组合子库允许您专注于高级操作，并将低级“噪声”推到后台。我们在“为什么使用F#”系列的例子中已经看到了一些这种能力的例子，`List` 模块中充满了这些例子——如果你仔细想想，“`fold`”和“`map`”函数也是组合子。

组合子的另一个优点是它们是最安全的函数类型。由于他们不依赖外部世界，因此如果全局环境发生变化，他们也无法改变。如果上下文不同，读取全局值或使用库函数的函数可以在调用之间中断或更改。组合子永远不会发生这种情况。

在 F# 中，组合子库可用于解析（FParsec 库）、HTML 构造、测试框架等。我们将在后面的系列文章中进一步讨论和使用组合子。

## 递归函数

通常，一个函数需要在其主体中引用自己。经典的例子是斐波那契函数：

```F#
let fib i =
   match i with
   | 1 -> 1
   | 2 -> 1
   | n -> fib(n-1) + fib(n-2)
```

递归函数和数据结构在函数式编程中非常常见，我希望在以后的系列文章中专门讨论这个主题。

# 9 函数签名

*Part of the "Thinking functionally" series (*[link](https://fsharpforfunandprofit.com/posts/function-signatures/#series-toc)*)*

函数签名可以让你对它的作用有所了解
09五月2012 这篇文章是超过3年

https://fsharpforfunandprofit.com/posts/function-signatures/

这可能并不明显，但 F# 实际上有两种语法——一种用于普通（值）表达式，一种用于类型定义。例如：

```F#
[1;2;3]      // a normal expression
int list     // a type expression

Some 1       // a normal expression
int option   // a type expression

(1,"a")      // a normal expression
int * string // a type expression
```

类型表达式具有与普通表达式中使用的语法不同的特殊语法。当您使用交互式会话时，您已经看到了许多这样的例子，因为每个表达式的类型及其求值都已打印出来。

如您所知，F# 使用类型推断来推断类型，因此您通常不需要在代码中显式指定类型，特别是对于函数。但是，为了在 F# 中有效地工作，您确实需要了解类型语法，以便构建自己的类型、调试类型错误和理解函数签名。在这篇文章中，我们将重点介绍它在函数签名中的使用。

以下是一些使用类型语法的示例函数签名：

```F#
// expression syntax          // type syntax
let add1 x = x + 1            // int -> int
let add x y = x + y           // int -> int -> int
let print x = printf "%A" x   // 'a -> unit
System.Console.ReadLine       // unit -> string
List.sum                      // 'a list -> 'a
List.filter                   // ('a -> bool) -> 'a list -> 'a list
List.map                      // ('a -> 'b) -> 'a list -> 'b list
```

## 通过签名理解函数

仅仅通过检查函数的签名，你通常就可以对它的功能有所了解。让我们看一些例子，然后依次分析它们。

```F#
// function signature 1
int -> int -> int
```

此函数接受两个 `int` 参数并返回另一个，因此推测它是某种数学函数，如加法、减法、乘法或求幂。

```F#
// function signature 2
int -> unit
```

此函数接受一个 `int` 并返回一个 `unit`，这意味着该函数正在做一些重要的副作用。由于没有有用的返回值，副作用可能与写入IO有关，例如日志记录、写入文件或数据库，或类似的事情。

```F#
// function signature 3
unit -> string
```

此函数不接受任何输入，但返回一个 `string`，这意味着该函数正在凭空构造一个字符串！由于没有显式输入，该函数可能与读取（比如从文件中）或生成（比如随机字符串）有关。

```F#
// function signature 4
int -> (unit -> string)
```

此函数接受一个 `int`  输入并返回一个函数，该函数在调用时返回字符串。同样，该函数可能与读取或生成有关。输入可能会以某种方式初始化返回的函数。例如，输入可以是文件句柄，返回的函数类似于 `readline()`。或者，输入可以是随机字符串生成器的种子。我们不能确切地说，但我们可以做出一些有根据的猜测。

```F#
// function signature 5
'a list -> 'a
```

此函数接受某种类型的列表，但只返回该类型中的一个，这意味着该函数正在合并或从列表中选择元素。具有此签名的函数示例包括 `List.sum`、`List.max`、`List.head` 等。

```F#
// function signature 6
('a -> bool) -> 'a list -> 'a list
```

此函数有两个参数：第一个是将某物映射到 bool（谓词）的函数，第二个是列表。返回值是相同类型的列表。谓词用于确定一个值是否满足某种标准，因此看起来该函数是根据谓词是否为真从列表中选择元素，然后返回原始列表的一个子集。具有此签名的典型函数是 `List.filter`。

```F#
// function signature 7
('a -> 'b) -> 'a list -> 'b list
```

此函数有两个参数：第一个将类型 `'a` 映射到类型 `'b`，第二个是 `'a` 的列表。返回值是一个不同类型 `'b` 的列表。一个合理的猜测是，该函数将列表中的每个 `'a`，使用作为第一个参数传入的函数将它们映射到 `'b` 并返回新的 `'b` 列表。事实上，具有此签名的原型函数是 `List.map`。

### 使用函数签名查找库方法

函数签名是搜索库函数的重要组成部分。F# 库中有数百个函数，最初可能会让人不知所措。与面向对象语言不同，你不能简单地“点入”一个对象来找到所有合适的方法。然而，如果你知道你正在寻找的函数的签名，你通常可以快速缩小候选列表的范围。

例如，假设你有两个列表，你正在寻找一个函数将它们组合成一个。这个函数的签名是什么？它将接受两个列表参数，并返回第三个，所有参数类型相同，并给出签名：

```F#
'a list -> 'a list -> 'a list
```

现在转到 F# List 模块的 MSDN 文档，向下扫描函数列表，查找匹配的内容。碰巧的是，只有一个函数具有该签名：

```F#
append : 'T list -> 'T list -> 'T list
```

这正是我们想要的！

## 为函数签名定义自己的类型

有时，您可能希望创建自己的类型来匹配所需的函数签名。您可以使用“type”关键字来实现这一点，并以与编写签名相同的方式定义类型：

```F#
type Adder = int -> int
type AdderGenerator = int -> Adder
```

然后，您可以使用这些类型来约束函数值和参数。

例如，由于类型约束，下面的第二个定义将失败。如果删除类型约束（如第三个定义中所示），则不会有任何问题。

```F#
let a:AdderGenerator = fun x -> (fun y -> x + y)
let b:AdderGenerator = fun (x:float) -> (fun y -> x + y)
let c                = fun (x:float) -> (fun y -> x + y)
```

## 测试你对函数签名的理解

你对函数签名的理解程度如何？看看是否可以创建具有这些签名的简单函数。避免使用显式类型注释！

```F#
val testA = int -> int
val testB = int -> int -> int
val testC = int -> (int -> int)
val testD = (int -> int) -> int
val testE = int -> int -> int -> int
val testF = (int -> int) -> (int -> int)
val testG = int -> (int -> int) -> int
val testH = (int -> int -> int) -> int
```



# 10 组织函数

*Part of the "Thinking functionally" series (*[link](https://fsharpforfunandprofit.com/posts/organizing-functions/#series-toc)*)*

嵌套函数和模块
2012年5月10日 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/organizing-functions/

既然你知道如何定义函数，那么你如何组织它们呢？

在 F# 中，有三个选项：

- 函数可以嵌套在其他函数中。
- 在应用程序级别，顶层功能被分组为“模块”。
- 或者，您也可以使用面向对象的方法，将函数作为方法附加到类型上。

我们将在本文中介绍前两个选项，在下一篇文章中介绍第三个选项。

## 嵌套的函数

在F#中，您可以在其他函数内定义函数。这是封装主函数所需但不应暴露在外部的“辅助”函数的好方法。

在下面的示例中，`add` 嵌套在 `addThreeNumbers` 中：

```F#
let addThreeNumbers x y z  =

    //create a nested helper function
    let add n =
       fun x -> x + n

    // use the helper function
    x |> add y |> add z

// test
addThreeNumbers 2 3 4
```

嵌套函数可以直接访问其父函数参数，因为它们在作用域内。因此，在下面的示例中，`printError` 嵌套函数不需要有自己的任何参数——它可以直接访问 `n` 和 `max` 值。

```F#
let validateSize max n  =

    //create a nested helper function with no params
    let printError() =
        printfn "Oops: '%i' is bigger than max: '%i'" n max

    // use the helper function
    if n > max then printError()

// test
validateSize 10 9
validateSize 10 11
```

一种非常常见的模式是，main 函数定义了一个嵌套的递归辅助函数，然后用适当的初始值调用它。下面的代码就是一个例子：

```F#
let sumNumbersUpTo max =

    // recursive helper function with accumulator
    let rec recursiveSum n sumSoFar =
        match n with
        | 0 -> sumSoFar
        | _ -> recursiveSum (n-1) (n+sumSoFar)

    // call helper function with initial values
    recursiveSum max 0

// test
sumNumbersUpTo 10
```

嵌套函数时，请尽量避免嵌套非常深的函数，特别是如果嵌套函数直接访问其父作用域中的变量，而不是将参数传递给它们。一个嵌套不好的函数会像最糟糕的深度嵌套命令式分支一样令人困惑。

以下是如何避免这样做：

```F#
// wtf does this function do?
let f x =
    let f2 y =
        let f3 z =
            x * z
        let f4 z =
            let f5 z =
                y * z
            let f6 () =
                y * x
            f6()
        f4 y
    x * f2 x
```

## 模块

模块只是一组组合在一起的函数，通常是因为它们处理相同的数据类型。

模块定义看起来很像函数定义。它以 `module` 关键字开头，然后是一个 `=` 符号，然后列出模块的内容。模块的内容必须缩进，就像函数定义中的表达式必须缩进一样。

这是一个包含两个函数的模块：

```F#
module MathStuff =

    let add x y  = x + y
    let subtract x y  = x - y
```

现在，如果您在 Visual Studio 中尝试此操作，并将鼠标悬停在 `add` 函数上，您将看到 `add` 函数的全名实际上是`MathStuff.add`，就像 `MathStuff` 是一个类，`add` 是一个方法一样。

实际上，这正是正在发生的事情。在幕后，F#编译器创建了一个具有静态方法的静态类。所以C#的等价物是：

```C#
static class MathStuff
{
    static public int add(int x, int y)
    {
        return x + y;
    }

    static public int subtract(int x, int y)
    {
        return x - y;
    }
}
```

如果你意识到模块只是静态类，函数是静态方法，那么你在理解模块在 F# 中的工作方式方面已经有了一个良好的开端，因为适用于静态类的大多数规则也适用于模块。

而且，就像在 C# 中每个独立函数都必须是类的一部分一样，在 F# 中每个独立功能都必须是模块的一部分。

### 跨模块边界访问功能

如果你想访问另一个模块中的函数，你可以通过它的限定名来引用它。

```F#
module MathStuff =

    let add x y  = x + y
    let subtract x y  = x - y

module OtherStuff =

    // use a function from the MathStuff module
    let add1 x = MathStuff.add x 1
```

您还可以使用 open 指令导入另一个模块中的所有函数，之后您可以使用短名称，而不必指定限定名称。

```F#
module OtherStuff =
    open MathStuff  // make all functions accessible

    let add1 x = add x 1
```

使用限定名的规则与您所期望的完全一致。也就是说，您始终可以使用完全限定名来访问函数，并且可以根据范围内的其他模块使用相对名或非限定名。

### 嵌套模块

就像静态类一样，模块可以包含嵌套在其中的子模块，如下所示：

```F#
module MathStuff =

    let add x y  = x + y
    let subtract x y  = x - y

    // nested module
    module FloatLib =

        let add x y :float = x + y
        let subtract x y :float  = x - y
```

其他模块可以根据需要使用全名或相对名称引用嵌套模块中的函数：

```F#
module OtherStuff =
    open MathStuff

    let add1 x = add x 1

    // fully qualified
    let add1Float x = MathStuff.FloatLib.add x 1.0

    //with a relative path
    let sub1Float x = FloatLib.subtract x 1.0
```

### 顶级模块

因此，如果可以有嵌套的子模块，这意味着，在链的上游，必须始终有一些顶级父模块。这确实是真的。

顶层模块的定义与我们迄今为止看到的模块略有不同。

- `module MyModuleName` 行必须是文件中的第一个声明
- 没有 `=` 符号
- 模块的内容没有缩进

一般来说，每个 `.FS` 源文件中都必须有一个顶级模块声明。也有一些例外，但无论如何，这都是很好的做法。模块名称不必与文件名称相同，但两个文件不能共享相同的模块名称。

为了 `.FSX` 脚本文件，不需要模块声明，在这种情况下，模块名称会自动设置为脚本的文件名。

下面是一个声明为顶级模块的 `MathStuff` 示例：

```F#
// top level module
module MathStuff

let add x y  = x + y
let subtract x y  = x - y

// nested module
module FloatLib =

    let add x y :float = x + y
    let subtract x y :float  = x - y
```

请注意，顶层代码（`module MathStuff` 的内容）没有缩进，但像 `FloatLib` 这样的嵌套模块的内容仍然需要缩进。

### 其他模块内容

一个模块可以包含其他声明和函数，包括类型声明、简单值和初始化代码（如静态构造函数）

```F#
module MathStuff =

    // functions
    let add x y  = x + y
    let subtract x y  = x - y

    // type definitions
    type Complex = {r:float; i:float}
    type IntegerFunction = int -> int -> int
    type DegreesOrRadians = Deg | Rad

    // "constant"
    let PI = 3.141

    // "variable"
    let mutable TrigType = Deg

    // initialization / static constructor
    do printfn "module initialized"
```

> 顺便说一句，如果你在交互式窗口中玩这些示例，你可能想每隔一段时间右键单击并执行“重置会话”，这样代码就新鲜了，不会被之前的评估所污染

### 阴影

这是我们的示例模块。请注意，`MathStuff` 具有 `add` 函数，`FloatLib` 也具有 `add` 函数。

```F#
module MathStuff =

    let add x y  = x + y
    let subtract x y  = x - y

    // nested module
    module FloatLib =

        let add x y :float = x + y
        let subtract x y :float  = x - y
```

现在，如果我将两者都纳入范围，然后使用 `add`，会发生什么？

```F#
open  MathStuff
open  MathStuff.FloatLib

let result = add 1 2  // Compiler error: This expression was expected to
                      // have type float but here has type int
```

发生的事情就是 `MathStuff.FloatLib` 模块屏蔽或覆盖了原始的 `MathStuff` 模块，该模块已被 `FloatLib` “屏蔽”。

因此，您现在会得到一个 FS0001 编译器错误，因为第一个参数 `1` 应该是浮点数。您必须将 `1` 更改为 `1.0` 才能修复此问题。

不幸的是，这是看不见的，很容易被忽视。有时你可以用它做一些很酷的技巧，几乎像子类化，但更常见的是，如果你有同名函数（比如非常常见的 `map`），这可能会很烦人。

如果你不想发生这种情况，有一种方法可以通过使用 `RequireQualifiedAccess` 属性来阻止它。这是一个同样的例子，两个模块都用它来装饰。

```F#
[<RequireQualifiedAccess>]
module MathStuff =

    let add x y  = x + y
    let subtract x y  = x - y

    // nested module
    [<RequireQualifiedAccess>]
    module FloatLib =

        let add x y :float = x + y
        let subtract x y :float  = x - y
```

现在不允许 `open`：

```F#
open  MathStuff   // error
open  MathStuff.FloatLib // error
```

但我们仍然可以通过它们的限定名访问这些函数（没有任何歧义）：

```F#
let result = MathStuff.add 1 2
let result = MathStuff.FloatLib.add 1.0 2.0
```

### 访问控制

F# 支持使用标准 .NET 访问控制关键字，如 `public`、`private` 和 `internal`。MSDN 文档提供了完整的详细信息。

- 这些访问说明符可以放在模块中的顶级（“let bound”）函数、值、类型和其他声明上。也可以为模块本身指定它们（例如，您可能需要一个私有嵌套模块）。
- 默认情况下，所有内容都是公开的（除了少数例外），因此如果你想保护它们，你需要使用 `private` 或 `internal`。

这些访问说明符只是 F# 中进行访问控制的一种方式。另一种完全不同的方法是使用模块“签名”文件，它有点像 C 头文件。它们以抽象的方式描述模块的内容。签名对于进行严肃的封装非常有用，但这种讨论将不得不等待计划中的封装和基于功能的安全系列。

## 命名空间

F#中的命名空间类似于C#中的命名空间。它们可用于组织模块和类型，以避免名称冲突。

使用 `namespace` 关键字声明命名空间，如下所示。

```F#
namespace Utilities

module MathStuff =

    // functions
    let add x y  = x + y
    let subtract x y  = x - y
```

由于这个命名空间，`MathStuff` 模块的完全限定名现在变成了 `Utilities.MathStuff` 和 `add` 函数的完全限定名现在变成了 `Utilities.MathStuff.add`。

对于命名空间，缩进规则适用，因此上面定义的模块必须缩进其内容，就像它是一个嵌套模块一样。

您还可以通过在模块名称中添加点来隐式声明命名空间。也就是说，上面的代码也可以写成：

```F#
module Utilities.MathStuff

// functions
let add x y  = x + y
let subtract x y  = x - y
```

`MathStuff` 模块的完全限定名仍然是 `Utilities.MathStuff`，但在这种情况下，该模块是一个顶级模块，内容不需要缩进。

使用名称空间时需要注意的一些其他事项：

- 命名空间对于模块是可选的。与C#不同，F#项目没有默认的命名空间，因此没有命名空间的顶级模块将处于全局级别。如果您计划创建可重用的库，请务必添加某种名称空间，以避免与其他库中的代码发生命名冲突。
- 命名空间可以直接包含类型声明，但不能包含函数声明。如前所述，所有函数和值声明都必须是模块的一部分。
- 最后，请注意，名称空间在脚本中不能很好地工作。例如，如果您试图将命名空间声明（如下面的命名空间实用程序）发送到交互式窗口，您将收到错误。

### 命名空间层次结构

您可以通过简单地用句点分隔名称来创建命名空间层次结构：

```F#
namespace Core.Utilities

module MathStuff =
    let add x y  = x + y
```

如果你想把两个命名空间放在同一个文件中，你可以。请注意，所有命名空间都必须完全限定——没有嵌套。

```F#
namespace Core.Utilities

module MathStuff =
    let add x y  = x + y

namespace Core.Extra

module MoreMathStuff =
    let add x y  = x + y
```

你不能做的一件事是在命名空间和模块之间发生命名冲突。

```F#
namespace Core.Utilities

module MathStuff =
    let add x y  = x + y

namespace Core

// fully qualified name of module
// is Core.Utilities
// Collision with namespace above!
module Utilities =
    let add x y  = x + y
```

## 在模块中混合类型和功能

我们已经看到，一个模块通常由一组作用于数据类型的相关函数组成。

在面向对象程序中，数据结构和作用于它的函数将组合在一个类中。然而，在函数式F#中，数据结构和作用于它的函数被组合在一个模块中。

将类型和函数混合在一起有两种常见模式：

- 将类型与函数分开声明
- 在与函数相同的模块中声明类型

在第一种方法中，类型在任何模块外部（但在名称空间中）声明，然后将处理该类型的函数放入一个名称相似的模块中。

```F#
// top-level module
namespace Example

// declare the type outside the module
type PersonType = {First:string; Last:string}

// declare a module for functions that work on the type
module Person =

    // constructor
    let create first last =
        {First=first; Last=last}

    // method that works on the type
    let fullName {First=first; Last=last} =
        first + " " + last

// test
let person = Person.create "john" "doe"
Person.fullName person |> printfn "Fullname=%s"
```

在另一种方法中，类型在模块内声明，并给出一个简单的名称，如“`T`”或模块的名称。因此，这些函数是用 `MyModule.Func1` 和 `MyModule.Func2` 这样的名称访问的。而类型本身是用 `MyModule.T` 这样的名称访问的。举个例子：

```F#
module Customer =

    // Customer.T is the primary type for this module
    type T = {AccountId:int; Name:string}

    // constructor
    let create id name =
        {T.AccountId=id; T.Name=name}

    // method that works on the type
    let isValid {T.AccountId=id; } =
        id > 0

// test
let customer = Customer.create 42 "bob"
Customer.isValid customer |> printfn "Is valid?=%b"
```

请注意，在这两种情况下，您都应该有一个构造函数来创建该类型的新实例（如果您愿意，可以使用工厂方法）。这样做意味着您很少需要在客户端代码中显式命名该类型，因此，您不应该关心它是否存在于模块中！

那么，你应该选择哪种方法呢？

- 前一种方法更像 .NET，如果你想与其他非 F# 代码共享你的库，那就更好了，因为导出的类名就是你所期望的。
- 对于那些使用其他函数式语言的人来说，后一种方法更为常见。模块内的类型编译成嵌套类，这对互操作来说不是很好。

对你自己来说，你可能想尝试两者。在团队编程的情况下，你应该选择一种风格并保持一致。

### 仅包含类型的模块

如果你有一组类型需要声明，而不需要任何相关函数，那么就不用费心使用模块。您可以直接在命名空间中声明类型，避免嵌套类。

例如，以下是您可能会想到的方法：

```F#
// top-level module
module Example

// declare the type inside a module
type PersonType = {First:string; Last:string}

// no functions in the module, just types...
```

这里有一种替代方法。`module` 关键字已被 `namespace` 替换。

```F#
// use a namespace
namespace Example

// declare the type outside any module
type PersonType = {First:string; Last:string}
```

在这两种情况下，`PersonType` 将具有相同的完全限定名。

请注意，这仅适用于类型。函数必须始终位于模块中。

# 11 将函数附加到类型

*Part of the "Thinking functionally" series (*[link](https://fsharpforfunandprofit.com/posts/type-extensions/#series-toc)*)*

以F#方式创建方法
2012年5月11日 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/type-extensions/

尽管到目前为止我们一直关注纯函数式风格，但有时切换到面向对象风格是方便的。OO 风格的一个关键特征是能够将函数附加到类中，并“点入”类以获得所需的行为。

在 F# 中，这是使用一个名为“类型扩展”的功能完成的。任何 F# 类型，而不仅仅是类，都可以附加函数。

下面是一个将函数附加到记录类型的示例。

```F#
module Person =
    type T = {First:string; Last:string} with
        // member defined with type declaration
        member this.FullName =
            this.First + " " + this.Last

    // constructor
    let create first last =
        {First=first; Last=last}

// test
let person = Person.create "John" "Doe"
let fullname = person.FullName
```

需要注意的关键事项是：

- `with` 关键字表示成员列表的开始
- `member` 关键字表示这是一个成员函数（即方法）
- 单词 `this` 是被点入的对象的占位符（称为“自我标识符”）。占位符作为函数名称的前缀，然后函数体在需要引用当前实例时使用相同的占位符。没有要求使用特定的单词，只要它是一致的。你可以用 `this` 或 `self` 或 `me`，或者任何其他通常表示自我参照的词。

您不必在声明类型的同时添加成员，您始终可以稍后在同一模块中添加它：

```F#
module Person =
    type T = {First:string; Last:string} with
       // member defined with type declaration
        member this.FullName =
            this.First + " " + this.Last

    // constructor
    let create first last =
        {First=first; Last=last}

    // another member added later
    type T with
        member this.SortableName =
            this.Last + ", " + this.First
// test
let person = Person.create "John" "Doe"
let fullname = person.FullName
let sortableName = person.SortableName
```

这些例子展示了所谓的“内在扩展”。它们被编译成类型本身，并且在使用该类型时始终可用。当你使用反射时，它们也会出现。

使用内部扩展，甚至可以有一个跨多个文件划分的类型定义，只要所有组件使用相同的命名空间并全部编译到相同的程序集中。就像C#中的分部类一样，这对于将生成的代码与编写的代码分开非常有用。

## 可选扩展

另一种选择是，您可以从完全不同的模块中添加额外的成员。这些被称为“可选扩展”。它们不会被编译成类型本身，并且需要一些其他模块在范围内才能工作（这种行为就像C#扩展方法一样）。

例如，假设我们定义了 `Person` 类型：

```F#
module Person =
    type T = {First:string; Last:string} with
       // member defined with type declaration
        member this.FullName =
            this.First + " " + this.Last

    // constructor
    let create first last =
        {First=first; Last=last}

    // another member added later
    type T with
        member this.SortableName =
            this.Last + ", " + this.First
```

下面的示例演示了如何在不同的模块中向其添加 `UppercaseName` 扩展：

```F#
// in a different module
module PersonExtensions =

    type Person.T with
    member this.UppercaseName =
        this.FullName.ToUpper()
```

现在让我们测试一下这个扩展：

```F#
let person = Person.create "John" "Doe"
let uppercaseName = person.UppercaseName
```

哦，我们出了点差错。问题是 `PersonExtensions` 不在作用域内。就像 C# 一样，任何扩展都必须进入范围才能使用。

一旦我们做到了，一切都会好起来的：

```F#
// bring the extension into scope first!
open PersonExtensions

let person = Person.create "John" "Doe"
let uppercaseName = person.UppercaseName
```

## 扩展系统类型

您可以扩展中的类型 .NET 库也是如此。但请注意，在扩展类型时，必须使用实际的类型名称，而不是类型缩写。

例如，如果你试图扩展 `int`，你会失败，因为 `int` 不是该类型的真实名称：

```F#
type int with
    member this.IsEven = this % 2 = 0
```

您必须改用 `System.Int32`：

```F#
type System.Int32 with
    member this.IsEven = this % 2 = 0

//test
let i = 20
if i.IsEven then printfn "'%i' is even" i
```

## 静态成员

您可以通过以下方式使成员函数静态：

- 添加关键字 `static`
- 删除 `this` 占位符

```F#
module Person =
    type T = {First:string; Last:string} with
        // member defined with type declaration
        member this.FullName =
            this.First + " " + this.Last

        // static constructor
        static member Create first last =
            {First=first; Last=last}

// test
let person = Person.T.Create "John" "Doe"
let fullname = person.FullName
```

您也可以为系统类型创建静态成员：

```F#
type System.Int32 with
    static member IsOdd x = x % 2 = 1

type System.Double with
    static member Pi = 3.141

//test
let result = System.Int32.IsOdd 20
let pi = System.Double.Pi
```

## 附加现有功能

一种非常常见的模式是将预先存在的独立函数附加到类型上。这有几个好处：

- 在开发过程中，您可以创建引用其他独立函数的独立函数。这使得编程更容易，因为类型推理在函数式代码中比在OO风格（“点入”）代码中工作得更好。
- 但对于某些关键函数，您也可以将它们附加到类型上。这让客户可以选择是使用函数式风格还是面向对象风格。

F# 库中的一个例子是计算列表长度的函数。它可以作为 `List` 模块中的独立函数使用，也可以作为列表实例上的方法使用。

```F#
let list = [1..10]

// functional style
let len1 = List.length list

// OO style
let len2 = list.Length
```

在下面的示例中，我们从一个最初没有成员的类型开始，然后定义一些函数，最后将 `fullName` 函数附加到该类型。

```F#
module Person =
    // type with no members initially
    type T = {First:string; Last:string}

    // constructor
    let create first last =
        {First=first; Last=last}

    // standalone function
    let fullName {First=first; Last=last} =
        first + " " + last

    // attach preexisting function as a member
    type T with
        member this.FullName = fullName this

// test
let person = Person.create "John" "Doe"
let fullname = Person.fullName person  // functional style
let fullname2 = person.FullName        // OO style
```

独立的 `fullName` 函数有一个参数，即 person。在附加的成员中，参数来自此自引用。

### 附加具有多个参数的现有函数

一件好事是，当之前定义的函数有多个参数时，只要 `this` 参数是第一个，在执行附件时就不必重新指定所有参数。

在下面的示例中，`hasSameFirstAndLastName` 函数有三个参数。然而，当我们附上它时，我们只需要指定一个！

```F#
module Person =
    // type with no members initially
    type T = {First:string; Last:string}

    // constructor
    let create first last =
        {First=first; Last=last}

    // standalone function
    let hasSameFirstAndLastName (person:T) otherFirst otherLast =
        person.First = otherFirst && person.Last = otherLast

    // attach preexisting function as a member
    type T with
        member this.HasSameFirstAndLastName = hasSameFirstAndLastName this

// test
let person = Person.create "John" "Doe"
let result1 = Person.hasSameFirstAndLastName person "bob" "smith" // functional style
let result2 = person.HasSameFirstAndLastName "bob" "smith" // OO style
```

为什么这行得通？提示：考虑currying和部分应用！

## 元组形式方法

当我们开始使用具有多个参数的方法时，我们必须做出决定：

- 我们可以使用标准（curried）形式，其中参数用空格分隔，并支持部分应用程序。
- 我们可以在一个元组中一次性传递所有参数，以逗号分隔。

“curried”形式更具功能性，“tuple”形式更面向对象。

元组形式也是 F# 与标准 .NET 库交互的方式，所以让我们更详细地研究一下这种方法。

作为测试平台，这里有一个带有两个方法的产品类型，每个方法都使用其中一种方法实现。`CurriedTotal` 和 `TupleTotal` 方法都做同样的事情：计算给定数量和折扣的总价。

```F#
type Product = {SKU:string; Price: float} with

    // curried style
    member this.CurriedTotal qty discount =
        (this.Price * float qty) - discount

    // tuple style
    member this.TupleTotal(qty,discount) =
        (this.Price * float qty) - discount
```

以下是一些测试代码：

```F#
let product = {SKU="ABC"; Price=2.0}
let total1 = product.CurriedTotal 10 1.0
let total2 = product.TupleTotal(10,1.0)
```

到目前为止没有区别。

我们知道curried版本可以部分应用：

```F#
let totalFor10 = product.CurriedTotal 10
let discounts = [1.0..5.0]
let totalForDifferentDiscounts
    = discounts |> List.map totalFor10
```

但是元组方法可以做一些curried方法做不到的事情，即：

- 命名参数
- 可选参数
- 重载

### 带元组样式参数的命名参数

元组样式方法支持命名参数：

```F#
let product = {SKU="ABC"; Price=2.0}
let total3 = product.TupleTotal(qty=10,discount=1.0)
let total4 = product.TupleTotal(discount=1.0, qty=10)
```

如您所见，当使用名称时，可以更改参数顺序。

注意：如果某些参数有名称，而另一些没有，则命名的参数必须始终是最后一个。

### 带元组样式参数的可选参数

对于元组风格的方法，您可以通过在参数名称前加上问号来指定可选参数。

- 如果设置了参数，它将显示为 `Some value`
- 如果未设置参数，则显示为 `None`

这里有一个例子：

```F#
type Product = {SKU:string; Price: float} with

    // optional discount
    member this.TupleTotal2(qty,?discount) =
        let extPrice = this.Price * float qty
        match discount with
        | None -> extPrice
        | Some discount -> extPrice - discount
```

这里有一个测试：

```F#
let product = {SKU="ABC"; Price=2.0}

// discount not specified
let total1 = product.TupleTotal2(10)

// discount specified
let total2 = product.TupleTotal2(10,1.0)
```

`None` 和 `Some` 的显式匹配可能很乏味，处理可选参数有一个稍微优雅的解决方案。

有一个函数 `defaultArg`，它将参数作为第一个参数，将默认值作为第二个参数。如果设置了参数，则返回值。如果没有，则返回默认值。

让我们看看重写相同的代码以使用 `defaultArg`

```F#
type Product = {SKU:string; Price: float} with

    // optional discount
    member this.TupleTotal2(qty,?discount) =
        let extPrice = this.Price * float qty
        let discount = defaultArg discount 0.0
        //return
        extPrice - discount
```

### 函数重载

在 C# 中，你可以有多个同名方法，它们的不同之处仅在于函数签名（例如不同的参数类型和/或参数数量）

在纯函数模型中，这是没有意义的——函数使用特定的域类型和特定的范围类型。同一函数不能用于不同的域和范围。

然而，F# 确实支持方法重载，但仅适用于方法（即附加到类型的函数），其中仅适用于使用元组样式参数传递的方法。

这里有一个例子，它是 `TupleTotal` 方法的另一个变体！

```F#
type Product = {SKU:string; Price: float} with

    // no discount
    member this.TupleTotal3(qty) =
        printfn "using non-discount method"
        this.Price * float qty

    // with discount
    member this.TupleTotal3(qty, discount) =
        printfn "using discount method"
        (this.Price * float qty) - discount
```

通常，F# 编译器会抱怨有两个同名方法，但在这种情况下，因为它们是基于元组的，而且它们的签名不同，所以这是可以接受的。（为了清楚地说明调用的是哪一个，我添加了一条小调试消息。）

这里有一个测试：

```F#
let product = {SKU="ABC"; Price=2.0}

// discount not specified
let total1 = product.TupleTotal3(10)

// discount specified
let total2 = product.TupleTotal3(10,1.0)
```

## 嘿！没那么快……使用方法的缺点

如果你来自面向对象的背景，你可能会想在任何地方都使用方法，因为这是你所熟悉的。但请注意，使用方法也有一些主要的缺点：

- 方法不能很好地进行类型推理
- 方法不能很好地处理高阶函数

事实上，过度使用方法会不必要地绕过F#编程中最强大和最有用的方面。

让我们看看我的意思。

### 方法不能很好地进行类型推理

让我们再次回到 Person 示例，该示例将相同的逻辑实现为独立函数和方法：

```F#
module Person =
    // type with no members initially
    type T = {First:string; Last:string}

    // constructor
    let create first last =
        {First=first; Last=last}

    // standalone function
    let fullName {First=first; Last=last} =
        first + " " + last

    // function as a member
    type T with
        member this.FullName = fullName this
```

现在让我们看看每种方法在类型推理方面的效果如何。假设我想打印一个人的全名，那么我将定义一个函数 `printFullName`，它将一个人作为参数。

这是使用模块级独立函数的代码。

```F#
open Person

// using standalone function
let printFullName person =
    printfn "Name is %s" (fullName person)

// type inference worked:
//    val printFullName : Person.T -> unit
```

编译时没有问题，类型推断正确地推断出参数是人

现在，让我们尝试“点（dotted）”版本：

```F#
open Person

// using method with "dotting into"
let printFullName2 person =
    printfn "Name is %s" (person.FullName)
```

这根本无法编译，因为类型推断没有足够的信息来推断参数。任何对象都可以实现 `.FullName`——这还不够。

是的，我们可以用参数类型来注释函数，但这违背了类型推理的全部目的。

### 方法不能很好地处理高阶函数

高阶函数也会出现类似的问题。例如，假设给定一个人员列表，我们想得到他们的全名。

使用独立函数，这很简单：

```F#
open Person

let list = [
    Person.create "Andy" "Anderson";
    Person.create "John" "Johnson";
    Person.create "Jack" "Jackson"]

//get all the full names at once
list |> List.map fullName
```

使用对象方法，我们必须在任何地方创建特殊的 lambdas：

```F#
open Person

let list = [
    Person.create "Andy" "Anderson";
    Person.create "John" "Johnson";
    Person.create "Jack" "Jackson"]

//get all the full names at once
list |> List.map (fun p -> p.FullName)
```

这只是一个简单的例子。对象方法组合不好，很难管道化等等。

所以，我呼吁你们这些刚接触函数式编程的人。如果可以的话，不要使用任何方法，尤其是在学习的时候。它们是阻碍你从函数式编程中充分受益的拐杖。

# 12 示例：基于堆栈的计算器

*Part of the "Thinking functionally" series (*[link](https://fsharpforfunandprofit.com/posts/stack-based-calculator/#series-toc)*)*

使用组合子构建功能
07七月2012 这篇文章是超过3年

https://fsharpforfunandprofit.com/posts/stack-based-calculator/

在这篇文章中，我们将实现一个简单的基于堆栈的计算器（也称为“逆波兰式”风格）。该实现几乎完全由函数完成，只有一种特殊类型，根本没有模式匹配，因此它是本系列介绍的概念的一个很好的测试场。

如果你不熟悉基于堆栈的计算器，它的工作原理如下：数字被推送到堆栈上，加法和乘法等操作会从堆栈中弹出数字并将结果推回到堆栈上。

以下是使用堆栈进行简单计算的示意图：

基于堆栈的计算器图

设计这样一个系统的第一步是考虑如何使用它。遵循类似 Forth 的语法，我们将给每个动作一个标签，这样上面的例子可能会写成这样：

`EMPTY ONE THREE ADD TWO MUL SHOW`

我们可能无法得到这个确切的语法，但让我们看看我们能有多接近。

## Stack 数据类型

首先，我们需要定义堆栈的数据结构。为了简单起见，我们只使用浮点数列表。

```F#
type Stack = float list
```

但是，等等，让我们把它包装成一个单案例联合类型，使其更具描述性，如下所示：

```F#
type Stack = StackContents of float list
```

有关为什么这更好的更多细节，请阅读本文中关于单案例联合类型的讨论。

现在，要创建一个新的堆栈，我们使用 `StackContents` 作为构造函数：

```F#
let newStack = StackContents [1.0;2.0;3.0]
```

为了提取现有 Stack 的内容，我们使用 `StackContents` 进行模式匹配：

```F#
let (StackContents contents) = newStack

// "contents" value set to
// float list = [1.0; 2.0; 3.0]
```

## Push 函数

接下来，我们需要一种方法将数字推送到堆栈上。这将只是使用“`::`”运算符在列表前面添加新值。

以下是我们的推送功能：

```F#
let push x aStack =
    let (StackContents contents) = aStack
    let newContents = x::contents
    StackContents newContents
```

这个基本功能有很多值得讨论的地方。

首先，请注意列表结构是不可变的，因此函数必须接受一个现有的堆栈并返回一个新的堆栈。它不能只是改变现有的堆栈。事实上，本例中的所有函数都具有类似的格式，如下所示：

`输入：堆栈加上其他参数`
`输出：一个新的堆栈`

接下来，参数的顺序应该是什么？堆栈参数应该排在第一位还是最后一位？如果你还记得为部分应用程序设计函数的讨论，你会记得最易变的东西应该放在最后。你很快就会看到这条指导方针即将出台。

最后，通过在函数参数本身中使用模式匹配，而不是在函数体中使用 `let`，可以使函数更加简洁。

以下是重写版本：

```F#
let push x (StackContents contents) =
    StackContents (x::contents)
```

好多了！

顺便说一句，看看它的漂亮签名：

```F#
val push : float -> Stack -> Stack
```

正如我们在上一篇文章中所知道的，签名告诉了你很多关于函数的信息。在这种情况下，即使不知道函数的名称是“push”，我也可以仅从签名中猜测它做了什么。这就是为什么使用显式类型名是一个好主意的原因之一。如果堆栈类型只是一个浮点数列表，它就不会像自文档一样。

不管怎样，现在让我们来测试一下：

```F#
let emptyStack = StackContents []
let stackWith1 = push 1.0 emptyStack
let stackWith2 = push 2.0 stackWith1
```

工作得很好！

## 建立在“push”之上

有了这个简单的函数，我们可以很容易地定义一个将特定数字推送到堆栈上的操作。

```F#
let ONE stack = push 1.0 stack
let TWO stack = push 2.0 stack
```

但是等一下！你能看到 `stack` 参数在两侧都有使用吗？事实上，我们根本不需要提及它。相反，我们可以跳过 `stack` 参数，使用部分应用编写函数，如下所示：

```F#
let ONE = push 1.0
let TWO = push 2.0
let THREE = push 3.0
let FOUR = push 4.0
let FIVE = push 5.0
```

现在你可以看到，如果 `push` 的参数顺序不同，我们就无法做到这一点。

当我们开始时，让我们定义一个函数来创建一个空堆栈：

```F#
let EMPTY = StackContents []
```

现在让我们测试所有这些：

```F#
let stackWith1 = ONE EMPTY
let stackWith2 = TWO stackWith1
let stackWith3  = THREE stackWith2
```

这些中间堆栈很烦人——我们能摆脱它们吗？对！请注意，这些函数ONE、TWO、THREE都有相同的签名：

```F#
Stack -> Stack
```

这意味着它们可以很好地连接在一起！一个的输出可以馈入下一个的输入，如下所示：

```F#
let result123 = EMPTY |> ONE |> TWO |> THREE
let result312 = EMPTY |> THREE |> ONE |> TWO
```

## 弹出堆栈

这样就可以推送到堆栈上了——接下来 `pop` 函数呢？

当我们弹出堆栈时，我们显然会返回堆栈的顶部，但仅此而已吗？

在面向对象的风格中，答案是肯定的。在 OO 方法中，我们会在幕后修改堆栈本身，以便删除顶部元素。

但在函数式风格中，堆栈是不可变的。删除顶部元素的唯一方法是创建一个删除了该元素的新堆栈。为了使调用者能够访问这个新的缩减堆栈，它需要与顶部元素本身一起返回。

换句话说，`pop` 函数必须返回两个值，顶部加上新堆栈。在 F# 中，最简单的方法就是使用元组。

下面是实现：

```F#
/// Pop a value from the stack and return it
/// and the new stack as a tuple
let pop (StackContents contents) =
    match contents with
    | top::rest ->
        let newStack = StackContents rest
        (top,newStack)
```

这个功能也很简单。

如前所述，我们直接在参数中提取 `contents`。

然后我们用 `match..with` 表达式来测试内容。

接下来，我们将顶部元素与其余元素分离，从其余元素创建一个新的堆栈，最后将该对作为元组返回。

试试上面的代码，看看会发生什么。你会得到一个编译器错误！编译器发现了一个我们忽略的情况——如果堆栈为空，会发生什么？

所以现在我们必须决定如何处理这个问题。

- 选项1：返回一个特殊的“成功”或“错误”状态，就像我们在“为什么使用F#？”系列文章中所做的那样。
- 选项2：抛出异常。

一般来说，我更喜欢使用错误案例，但在这种情况下，我们将使用异常。以下是为处理空箱而更改的 `pop` 代码：

```F#
/// Pop a value from the stack and return it
/// and the new stack as a tuple
let pop (StackContents contents) =
    match contents with
    | top::rest ->
        let newStack = StackContents rest
        (top,newStack)
    | [] ->
        failwith "Stack underflow"
```

现在让我们测试一下：

```F#
let initialStack = EMPTY  |> ONE |> TWO
let popped1, poppedStack = pop initialStack
let popped2, poppedStack2 = pop poppedStack
```

并测试下溢：

```F#
let _ = pop EMPTY
```

## 编写数学函数

现在，有了推送和弹出功能，我们可以处理“添加”和“乘法”函数：

```F#
let ADD stack =
   let x,s = pop stack  //pop the top of the stack
   let y,s2 = pop s     //pop the result stack
   let result = x + y   //do the math
   push result s2       //push back on the doubly-popped stack

let MUL stack =
   let x,s = pop stack  //pop the top of the stack
   let y,s2 = pop s     //pop the result stack
   let result = x * y   //do the math
   push result s2       //push back on the doubly-popped stack
```

以交互方式测试这些：

```F#
let add1and2 = EMPTY |> ONE |> TWO |> ADD
let add2and3 = EMPTY |> TWO |> THREE |> ADD
let mult2and3 = EMPTY |> TWO |> THREE |> MUL
```

它奏效了！

## 是时候进行重构了…

很明显，这两个函数之间存在大量重复代码。我们如何重构？

这两个函数都从堆栈中弹出两个值，应用某种二进制函数，然后将结果推回堆栈。这导致我们将通用代码重构为一个“二进制”函数，该函数接受一个双参数数学函数作为参数：

```F#
let binary mathFn stack =
    // pop the top of the stack
    let y,stack' = pop stack
    // pop the top of the stack again
    let x,stack'' = pop stack'
    // do the math
    let z = mathFn x y
    // push the result value back on the doubly-popped stack
    push z stack''
```

*请注意，在这个实现中，我已经切换到使用记号来表示“同一”对象的更改状态，而不是数字后缀。数字后缀很容易让人感到困惑。*

问题：为什么参数是按上面顺序排列的，而不是 `mathFn` 在 `stack` 之后？

现在我们有了 `binary`，我们可以更简单地定义 ADD 和朋友：

这是使用新的 `binary` 助手首次尝试 ADD：

```F#
let ADD aStack = binary (fun x y -> x + y) aStack
```

但是我们可以删除 lambda，因为它正是内置 `+` 函数的定义！这给了我们：

```F#
let ADD aStack = binary (+) aStack
```

同样，我们可以使用部分应用程序来隐藏堆栈参数。以下是最终的定义：

```F#
let ADD = binary (+)
```

以下是其他一些数学函数的定义：

```F#
let SUB = binary (-)
let MUL = binary (*)
let DIV = binary (/)
```

让我们再次以交互方式进行测试。

```F#
let threeDivTwo = EMPTY |> THREE |> TWO |> DIV   // Answer: 1.5
let twoSubtractFive = EMPTY |> TWO |> FIVE |> SUB  // Answer: -3.0
let oneAddTwoSubThree = EMPTY |> ONE |> TWO |> ADD |> THREE |> SUB // Answer: 0.0
```

以类似的方式，我们可以为一元函数创建一个辅助函数

```F#
let unary f stack =
    let x,stack' = pop stack  //pop the top of the stack
    push (f x) stack'         //push the function value on the stack
```

然后定义一些一元函数：

```F#
let NEG = unary (fun x -> -x)
let SQUARE = unary (fun x -> x * x)
```

再次交互式测试：

```F#
let neg3 = EMPTY |> THREE |> NEG
let square2 = EMPTY |> TWO |> SQUARE
```

## 把它们放在一起

在最初的需求中，我们提到我们希望能够显示结果，所以让我们定义一个 SHOW 函数。

```F#
let SHOW stack =
    let x,_ = pop stack
    printfn "The answer is %f" x
    stack  // keep going with same stack
```

请注意，在这种情况下，我们弹出原始堆栈，但忽略缩减版本。函数的最终结果是原始堆栈，就好像它从未被弹出过一样。

现在，我们终于可以根据原始需求编写代码示例了

```F#
EMPTY |> ONE |> THREE |> ADD |> TWO |> MUL |> SHOW // (1+3)*2 = 8
```

### 更进一步

这很有趣——我们还能做什么？

那么，我们可以定义更多的核心辅助函数：

```F#
/// Duplicate the top value on the stack
let DUP stack =
    // get the top of the stack
    let x,_ = pop stack
    // push it onto the stack again
    push x stack

/// Swap the top two values
let SWAP stack =
    let x,s = pop stack
    let y,s' = pop s
    push y (push x s')

/// Make an obvious starting point
let START  = EMPTY
```

有了这些附加功能，我们可以写一些很好的例子：

```F#
START
    |> ONE |> TWO |> SHOW

START
    |> ONE |> TWO |> ADD |> SHOW
    |> THREE |> ADD |> SHOW

START
    |> THREE |> DUP |> DUP |> MUL |> MUL // 27

START
    |> ONE |> TWO |> ADD |> SHOW  // 3
    |> THREE |> MUL |> SHOW       // 9
    |> TWO |> DIV |> SHOW         // 9 div 2 = 4.5
```

## 使用组合而不是管道

但这还不是全部。事实上，还有另一种非常有趣的方式来思考这些功能。

正如我之前指出的，它们都有相同的签名：

```F#
Stack -> Stack
```

因此，由于输入和输出类型相同，这些函数可以使用组合运算符 `>>` 组合，而不仅仅是用管道链接在一起。

以下是一些示例：

```F#
// define a new function
let ONE_TWO_ADD =
    ONE >> TWO >> ADD

// test it
START |> ONE_TWO_ADD |> SHOW

// define a new function
let SQUARE =
    DUP >> MUL

// test it
START |> TWO |> SQUARE |> SHOW

// define a new function
let CUBE =
    DUP >> DUP >> MUL >> MUL

// test it
START |> THREE |> CUBE |> SHOW

// define a new function
let SUM_NUMBERS_UPTO =
    DUP      // n, n           2 items on stack
    >> ONE   // n, n, 1        3 items on stack
    >> ADD   // n, (n+1)       2 items on stack
    >> MUL   // n(n+1)         1 item on stack
    >> TWO   // n(n+1), 2      2 items on stack
    >> DIV   // n(n+1)/2       1 item on stack

// test it with sum of numbers up to 9
START |> THREE |> SQUARE |> SUM_NUMBERS_UPTO |> SHOW  // 45
```

在每种情况下，通过将其他函数组合在一起以创建新函数来定义新函数。这是构建功能的“组合子”方法的一个很好的例子。

## 管道 vs 组合

我们现在已经看到了使用这种基于堆栈的模型的两种不同方式；通过管道或组合。那么，有什么区别呢？为什么我们更喜欢一种方式而不是另一种方式？

不同之处在于，从某种意义上说，管道是一种“实时转换”操作。当你使用管道时，你实际上是在做操作，传递一个特定的堆栈。

另一方面，组合是一种你想做什么的“计划”，从一组部分构建一个整体功能，但还没有实际运行它。

例如，我可以创建一个“计划”，通过组合较小的操作来平方一个数字：

```F#
let COMPOSED_SQUARE = DUP >> MUL
```

我无法用管道方法进行等效操作。

```F#
let PIPED_SQUARE = DUP |> MUL
```

这会导致编译错误。我必须有某种具体的堆栈实例才能使其工作：

```F#
let stackWith2 = EMPTY |> TWO
let twoSquared = stackWith2 |> DUP |> MUL
```

即便如此，我也只得到了这个特定输入的答案，而不是像 COMPOSED_SQUARE 示例中那样得到所有可能输入的计划。

创建“计划”的另一种方法是显式地将lambda传递给更原始的函数，正如我们在开头看到的那样：

```F#
let LAMBDA_SQUARE = unary (fun x -> x * x)
```

这要明确得多（而且可能更快），但失去了组合方法的所有好处和清晰度。

所以，一般来说，如果可以的话，就选择组合法吧！

## 完整的代码

这是到目前为止所有示例的完整代码。

```F#
// ==============================================
// Types
// ==============================================

type Stack = StackContents of float list

// ==============================================
// Stack primitives
// ==============================================

/// Push a value on the stack
let push x (StackContents contents) =
    StackContents (x::contents)

/// Pop a value from the stack and return it
/// and the new stack as a tuple
let pop (StackContents contents) =
    match contents with
    | top::rest ->
        let newStack = StackContents rest
        (top,newStack)
    | [] ->
        failwith "Stack underflow"

// ==============================================
// Operator core
// ==============================================

// pop the top two elements
// do a binary operation on them
// push the result
let binary mathFn stack =
    let y,stack' = pop stack
    let x,stack'' = pop stack'
    let z = mathFn x y
    push z stack''

// pop the top element
// do a unary operation on it
// push the result
let unary f stack =
    let x,stack' = pop stack
    push (f x) stack'

// ==============================================
// Other core
// ==============================================

/// Pop and show the top value on the stack
let SHOW stack =
    let x,_ = pop stack
    printfn "The answer is %f" x
    stack  // keep going with same stack

/// Duplicate the top value on the stack
let DUP stack =
    let x,s = pop stack
    push x (push x s)

/// Swap the top two values
let SWAP stack =
    let x,s = pop stack
    let y,s' = pop s
    push y (push x s')

/// Drop the top value on the stack
let DROP stack =
    let _,s = pop stack  //pop the top of the stack
    s                    //return the rest

// ==============================================
// Words based on primitives
// ==============================================

// Constants
// -------------------------------
let EMPTY = StackContents []
let START  = EMPTY


// Numbers
// -------------------------------
let ONE = push 1.0
let TWO = push 2.0
let THREE = push 3.0
let FOUR = push 4.0
let FIVE = push 5.0

// Math functions
// -------------------------------
let ADD = binary (+)
let SUB = binary (-)
let MUL = binary (*)
let DIV = binary (/)

let NEG = unary (fun x -> -x)


// ==============================================
// Words based on composition
// ==============================================

let SQUARE =
    DUP >> MUL

let CUBE =
    DUP >> DUP >> MUL >> MUL

let SUM_NUMBERS_UPTO =
    DUP      // n, n           2 items on stack
    >> ONE   // n, n, 1        3 items on stack
    >> ADD   // n, (n+1)       2 items on stack
    >> MUL   // n(n+1)         1 item on stack
    >> TWO   // n(n+1), 2      2 items on stack
    >> DIV   // n(n+1)/2       1 item on stack

```

## 摘要

所以我们有了它，一个简单的基于堆栈的计算器。我们已经看到了如何从一些基本操作（`push`、`pop`、`binary`、`unary`）开始，并从中构建一个易于实现和使用的整个领域特定语言。

正如你所猜测的，这个例子在很大程度上基于Forth语言。我强烈推荐免费的书《Thinking Forth》，它不仅介绍Forth语言，还介绍同样适用于函数式编程的（非面向对象！）问题分解技术。

我写这篇文章的灵感来自 Ashley Feniello 的一个很棒的博客。如果你想更深入地模拟 F# 中基于堆栈的语言，那就从那里开始。玩得高兴！

