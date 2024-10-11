# [返回主 Markdown](./FSharpForFunAndProfit翻译.md)



# 1 用类型设计：简介

*Part of the "Designing with types" series (*[link](https://fsharpforfunandprofit.com/posts/designing-with-types-intro/#series-toc)*)*

使设计更加透明，提高正确性
2013年1月12日 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/designing-with-types-intro/

在本系列中，我们将探讨在设计过程中使用类型的一些方法。特别是，周到地使用类型可以使设计更加透明，同时提高正确性。

本系列将聚焦于设计的“微观层面”。也就是说，在单个类型和函数的最低级别上工作。更高层次的设计方法，以及使用函数式或面向对象风格的相关决策，将在另一个系列中讨论。

许多建议在 C# 或 Java 中也是可行的，但 F# 类型的轻量级特性意味着我们更有可能进行这种重构。

## 一个基本的例子

为了演示类型的各种用途，我将使用一个非常简单的示例，即 `Contact` 类型，如下所示。

```F#
type Contact =
    {
    FirstName: string;
    MiddleInitial: string;
    LastName: string;

    EmailAddress: string;
    //true if ownership of email address is confirmed
    IsEmailVerified: bool;

    Address1: string;
    Address2: string;
    City: string;
    State: string;
    Zip: string;
    //true if validated against address service
    IsAddressValid: bool;
    }

```

这似乎很明显——我相信我们都见过很多次这样的事情。那么，我们能用它做什么呢？我们如何重构它以充分利用类型系统？

## 创建“原子”类型

首先要做的是查看数据访问和更新的使用模式。例如，`Zip` 是否有可能在不同时更新 `Address1` 的情况下进行更新？另一方面，事务更新 `EmailAddress` 而不更新 `FirstName` 的情况可能很常见。

这就引出了第一条准则：

- *指导原则：使用记录或元组将需要一致的数据（即“原子”）分组在一起，但不要不必要地将不相关的数据分组在一起。*

在这种情况下，很明显，三个名称值是一个集合，地址值是一组，电子邮件也是一组。

我们这里还有一些额外的标志，比如 `IsAddressValid` 和 `IsEmailVerified`。这些是否应该成为相关集合的一部分？当然，目前是的，因为标志取决于相关值。

例如，如果 `EmailAddress` 更改，则 `IsEmailVerified` 可能需要同时重置为 false。

对于 `PostalAddress`，很明显，核心“地址”部分是一个有用的通用类型，没有 `IsAddressValid` 标志。另一方面，`IsAddressValid` 与地址相关联，并在地址更改时进行更新。

因此，我们似乎应该创建两种类型。一个是通用的 `PostalAddress`，另一个是联系人上下文中的地址，我们可以称之为 `PostalContactInfo`。

```F#
type PostalAddress =
    {
    Address1: string;
    Address2: string;
    City: string;
    State: string;
    Zip: string;
    }

type PostalContactInfo =
    {
    Address: PostalAddress;
    IsAddressValid: bool;
    }
```

最后，我们可以使用选项类型来表示某些值（如 `MiddleInitial`）确实是可选的。

```F#
type PersonalName =
    {
    FirstName: string;
    // use "option" to signal optionality
    MiddleInitial: string option;
    LastName: string;
    }
```

## 摘要

经过所有这些更改，我们现在有以下代码：

```F#
type PersonalName =
    {
    FirstName: string;
    // use "option" to signal optionality
    MiddleInitial: string option;
    LastName: string;
    }

type EmailContactInfo =
    {
    EmailAddress: string;
    IsEmailVerified: bool;
    }

type PostalAddress =
    {
    Address1: string;
    Address2: string;
    City: string;
    State: string;
    Zip: string;
    }

type PostalContactInfo =
    {
    Address: PostalAddress;
    IsAddressValid: bool;
    }

type Contact =
    {
    Name: PersonalName;
    EmailContactInfo: EmailContactInfo;
    PostalContactInfo: PostalContactInfo;
    }

```

我们还没有编写一个函数，但代码已经更好地代表了域。然而，这只是我们所能做的事情的开始。

接下来，使用单案例联合为原始类型添加语义意义。



# 2 用类型设计：单案例联合类型

*Part of the "Designing with types" series (*[link](https://fsharpforfunandprofit.com/posts/designing-with-types-single-case-dus/#series-toc)*)*

为基本类型添加意义
2013年1月13日这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/designing-with-types-single-case-dus/

在上一篇文章的末尾，我们有电子邮件地址、邮政编码等的值，定义如下：

```F#
EmailAddress: string;
State: string;
Zip: string;
```

这些都被定义为简单的字符串。但实际上，它们只是字符串吗？电子邮件地址是否可以与邮政编码或州缩写互换？

在领域驱动的设计中，它们确实是不同的东西，而不仅仅是字符串。因此，理想情况下，我们希望为它们提供许多单独的类型，这样它们就不会意外混淆。

长期以来，这一直被认为是一种良好的做法，但在 C# 和 Java 等语言中，创建数百个这样的小类型可能会很痛苦，从而导致所谓的“原始痴迷”代码气味。

但是F#没有任何借口！创建简单的包装器类型是轻而易举的。

## 包装基本类型

创建单独类型的最简单方法是将底层字符串类型包装在另一个类型中。

我们可以使用单案例联合类型来实现，如下所示：

```F#
type EmailAddress = EmailAddress of string
type ZipCode = ZipCode of string
type StateCode = StateCode of string
```

或者，我们可以在一个字段中使用记录类型，如下所示：

```F#
type EmailAddress = { EmailAddress: string }
type ZipCode = { ZipCode: string }
type StateCode = { StateCode: string}
```

这两种方法都可以用来围绕字符串或其他基本类型创建包装器类型，那么哪种方法更好呢？

答案通常是单一案例可区分联合。“包装”和“解包”要容易得多，因为“联合案例”本身就是一个适当的构造函数。展开可以使用内联模式匹配来完成。

以下是一些如何构造和解构 `EmailAddress` 类型的示例：

```F#
type EmailAddress = EmailAddress of string

// using the constructor as a function
"a" |> EmailAddress
["a"; "b"; "c"] |> List.map EmailAddress

// inline deconstruction
let a' = "a" |> EmailAddress
let (EmailAddress a'') = a'

let addresses =
    ["a"; "b"; "c"]
    |> List.map EmailAddress

let addresses' =
    addresses
    |> List.map (fun (EmailAddress e) -> e)
```

使用记录类型无法轻松做到这一点。

因此，让我们再次重构代码以使用这些联合类型。现在看起来是这样的：

```F#
type PersonalName =
    {
    FirstName: string;
    MiddleInitial: string option;
    LastName: string;
    }

type EmailAddress = EmailAddress of string

type EmailContactInfo =
    {
    EmailAddress: EmailAddress;
    IsEmailVerified: bool;
    }

type ZipCode = ZipCode of string
type StateCode = StateCode of string

type PostalAddress =
    {
    Address1: string;
    Address2: string;
    City: string;
    State: StateCode;
    Zip: ZipCode;
    }

type PostalContactInfo =
    {
    Address: PostalAddress;
    IsAddressValid: bool;
    }

type Contact =
    {
    Name: PersonalName;
    EmailContactInfo: EmailContactInfo;
    PostalContactInfo: PostalContactInfo;
    }
```

联合类型的另一个优点是，实现可以用模块签名封装，我们将在下面讨论。

## 命名单个案例联合的“案例”

在上面的例子中，我们对案例使用了与类型相同的名称：

```F#
type EmailAddress = EmailAddress of string
type ZipCode = ZipCode of string
type StateCode = StateCode of string
```

这最初可能看起来很混乱，但实际上它们在不同的范围内，所以没有命名冲突。一个是类型，一个是同名的构造函数。

所以，如果你看到这样的函数签名：

```F#
val f: string -> EmailAddress
```

这指的是类型世界中的事物，因此 `EmailAddress` 指的是该类型。

另一方面，如果你看到这样的代码：

```F#
let x = EmailAddress y
```

这指的是值世界中的事物，因此 `EmailAddress` 指的是构造函数。

## 构建单一案例联合

对于具有特殊含义的值，如电子邮件地址和邮政编码，通常只允许使用某些值。并非每个字符串都是可接受的电子邮件或邮政编码。

这意味着我们需要在某个时候进行验证，还有什么比在构建时更好的时候呢？毕竟，一旦构建了值，它就是不可变的，所以不用担心以后有人会修改它。

以下是我们如何使用一些构造函数扩展上述模块：

```F#
... types as above ...

let CreateEmailAddress (s:string) =
    if System.Text.RegularExpressions.Regex.IsMatch(s,@"^\S+@\S+\.\S+$")
        then Some (EmailAddress s)
        else None

let CreateStateCode (s:string) =
    let s' = s.ToUpper()
    let stateCodes = ["AZ";"CA";"NY"] //etc
    if stateCodes |> List.exists ((=) s')
        then Some (StateCode s')
        else None
```

我们现在可以测试构造函数：

```F#
CreateStateCode "CA"
CreateStateCode "XX"

CreateEmailAddress "a@example.com"
CreateEmailAddress "example.com"
```

## 处理构造函数中的无效输入

对于这些类型的构造函数，一个迫在眉睫的挑战是如何处理无效输入的问题。例如，如果我将“abc”传递给电子邮件地址构造函数，会发生什么？

有很多方法可以处理它。

首先，你可以抛出一个异常。我觉得这个丑陋而缺乏想象力，所以我立即拒绝了这个！

接下来，您可以返回一个选项类型，其中 `None` 表示输入无效。这就是上面的构造函数所做的。

这通常是最简单的方法。它的优点是，当值无效时，调用者必须显式处理这种情况。

例如，上面示例的调用者代码可能如下：

```F#
match (CreateEmailAddress "a@example.com") with
| Some email -> ... do something with email
| None -> ... ignore?
```

缺点是，对于复杂的验证，可能不清楚出了什么问题。电子邮件太长，或者缺少“@”符号，或者域名无效？我们无法判断。

如果您确实需要更多详细信息，您可能希望返回一个在错误情况下包含更详细解释的类型。

以下示例使用 `CreationResult` 类型来指示失败案例中的错误。

```F#
type EmailAddress = EmailAddress of string
type CreationResult<'T> = Success of 'T | Error of string

let CreateEmailAddress2 (s:string) =
    if System.Text.RegularExpressions.Regex.IsMatch(s,@"^\S+@\S+\.\S+$")
        then Success (EmailAddress s)
        else Error "Email address must contain an @ sign"

// test
CreateEmailAddress2 "example.com"
```

最后，最通用的方法是使用延续（continuations）。也就是说，您传入两个函数，一个用于成功案例（将新构造的电子邮件作为参数），另一个用于失败案例（将错误字符串作为参数）。

```F#
type EmailAddress = EmailAddress of string

let CreateEmailAddressWithContinuations success failure (s:string) =
    if System.Text.RegularExpressions.Regex.IsMatch(s,@"^\S+@\S+\.\S+$")
        then success (EmailAddress s)
        else failure "Email address must contain an @ sign"
```

success 函数接受电子邮件作为参数，error 函数接受字符串。这两个函数必须返回相同的类型，但类型由您决定。

这里有一个简单的例子——这两个函数都执行 printf，不返回任何值（即 unit）。

```F#
let success (EmailAddress s) = printfn "success creating email %s" s
let failure  msg = printfn "error creating email: %s" msg
CreateEmailAddressWithContinuations success failure "example.com"
CreateEmailAddressWithContinuations success failure "x@example.com"
```

使用 continuation，您可以轻松地复制任何其他方法。例如，以下是创建选项的方法。在这种情况下，这两个函数都返回 `EmailAddress option`。

```F#
let success e = Some e
let failure _  = None
CreateEmailAddressWithContinuations success failure "example.com"
CreateEmailAddressWithContinuations success failure "x@example.com"
```

以下是在错误情况下抛出异常的方法：

```F#
let success e = e
let failure _  = failwith "bad email address"
CreateEmailAddressWithContinuations success failure "example.com"
CreateEmailAddressWithContinuations success failure "x@example.com"
```

这段代码看起来很麻烦，但在实践中，您可能会创建一个局部部分应用的函数，而不是冗长的函数。

```F#
// setup a partially applied function
let success e = Some e
let failure _  = None
let createEmail = CreateEmailAddressWithContinuations success failure

// use the partially applied function
createEmail "x@example.com"
createEmail "example.com"
```

> 如果你觉得这篇文章很有趣，看看我的《领域建模函数化》一书！这是对领域驱动设计、类型建模和函数式编程的一个很好的介绍。

## 为包装器类型创建模块

由于我们添加了验证，这些简单的包装器类型开始变得更加复杂，我们可能会发现我们想与该类型关联的其他函数。

因此，为每种包装器类型创建一个模块，并将该类型及其相关函数放在那里，这可能是一个好主意。

```F#
module EmailAddress =

    type T = EmailAddress of string

    // wrap
    let create (s:string) =
        if System.Text.RegularExpressions.Regex.IsMatch(s,@"^\S+@\S+\.\S+$")
            then Some (EmailAddress s)
            else None

    // unwrap
    let value (EmailAddress e) = e
```

然后，该类型的用户将使用模块函数创建和解包该类型。例如：

```F#
// create email addresses
let address1 = EmailAddress.create "x@example.com"
let address2 = EmailAddress.create "example.com"

// unwrap an email address
match address1 with
| Some e -> EmailAddress.value e |> printfn "the value is %s"
| None -> ()
```

## 强制使用构造函数

一个问题是，您不能强制调用者使用构造函数。有人可以绕过验证直接创建类型。

在实践中，这往往不是问题。一个简单的技术是使用命名约定来指示“private”类型，并提供“wrap”和“unwrap”函数，这样客户端就不需要直接与该类型交互。

这里有一个例子：

```F#
module EmailAddress =

    // private type
    type _T = EmailAddress of string

    // wrap
    let create (s:string) =
        if System.Text.RegularExpressions.Regex.IsMatch(s,@"^\S+@\S+\.\S+$")
            then Some (EmailAddress s)
            else None

    // unwrap
    let value (EmailAddress e) = e
```

当然，在这种情况下，类型并不是真正私有的，但您鼓励调用者始终使用“published”函数。

如果你真的想封装类型的内部并强制调用者使用构造函数，你可以使用模块签名。

以下是电子邮件地址示例的签名文件：

```F#
// FILE: EmailAddress.fsi

module EmailAddress

// encapsulated type
type T

// wrap
val create : string -> T option

// unwrap
val value : T -> string
```

（请注意，模块签名仅在编译后的项目中有效，在交互式脚本中无效，因此要测试这一点，您需要在 F# 项目中创建三个文件，文件名如下所示。）

以下是实现文件：

```F#
// FILE: EmailAddress.fs

module EmailAddress

// encapsulated type
type T = EmailAddress of string

// wrap
let create (s:string) =
    if System.Text.RegularExpressions.Regex.IsMatch(s,@"^\S+@\S+\.\S+$")
        then Some (EmailAddress s)
        else None

// unwrap
let value (EmailAddress e) = e
```

这是一个客户：

```F#
// FILE: EmailAddressClient.fs

module EmailAddressClient

open EmailAddress

// code works when using the published functions
let address1 = EmailAddress.create "x@example.com"
let address2 = EmailAddress.create "example.com"

// code that uses the internals of the type fails to compile
let address3 = T.EmailAddress "bad email"

```

模块签名导出的类型 `EmailAddress.T` 是不透明的，因此客户端无法访问内部。

如您所见，这种方法强制使用构造函数。试图直接创建类型（`T.EmailAddress "bad email"`）会导致编译错误。

## 何时“包装”单案例联合

现在我们有了包装器类型，我们应该什么时候构造它们？

通常，您只需要在服务边界处（例如，六边形架构中的边界）

在这种方法中，包装是在 UI 层中完成的，或者在从持久层加载时完成的，一旦创建了包装类型，它就会被传递到域层并作为不透明类型进行“整体”操作。令人惊讶的是，在域本身中工作时，您实际上直接需要包装内容的情况并不常见。

作为构造的一部分，调用者使用提供的构造函数而不是执行自己的验证逻辑至关重要。这确保了“坏”值永远不会进入域。

例如，这里有一些代码显示 UI 正在进行自己的验证：

```F#
let processFormSubmit () =
    let s = uiTextBox.Text
    if (s.Length < 50)
        then // set email on domain object
        else // show validation error message
```

更好的方法是让构造函数来做，如前所示。

```F#
let processFormSubmit () =
    let emailOpt = uiTextBox.Text |> EmailAddress.create
    match emailOpt with
    | Some email -> // set email on domain object
    | None -> // show validation error message
```

## 何时“拆封”单案例联合

什么时候需要拆开包装？同样，通常只在服务边界。例如，当您将电子邮件持久化到数据库，或绑定到 UI 元素或视图模型时。

避免显式展开的一个技巧是再次使用连续方法，传递一个将应用于包裹值的函数。

也就是说，与其显式调用“unwrap”函数：

```F#
address |> EmailAddress.value |> printfn "the value is %s"
```

你可以传入一个应用于内部值的函数，如下所示：

```F#
address |> EmailAddress.apply (printfn "the value is %s")
```

综上所述，我们现在有了完整的 `EmailAddress` 模块。

```F#
module EmailAddress =

    type _T = EmailAddress of string

    // create with continuation
    let createWithCont success failure (s:string) =
        if System.Text.RegularExpressions.Regex.IsMatch(s,@"^\S+@\S+\.\S+$")
            then success (EmailAddress s)
            else failure "Email address must contain an @ sign"

    // create directly
    let create s =
        let success e = Some e
        let failure _  = None
        createWithCont success failure s

    // unwrap with continuation
    let apply f (EmailAddress e) = f e

    // unwrap directly
    let value e = apply id e
```

`create` 和 `value` 函数不是严格必需的，但添加它们是为了方便调用者。

## 到目前为止的代码

现在让我们重构 `Contact` 代码，添加新的包装器类型和模块。

```F#
module EmailAddress =

    type T = EmailAddress of string

    // create with continuation
    let createWithCont success failure (s:string) =
        if System.Text.RegularExpressions.Regex.IsMatch(s,@"^\S+@\S+\.\S+$")
            then success (EmailAddress s)
            else failure "Email address must contain an @ sign"

    // create directly
    let create s =
        let success e = Some e
        let failure _  = None
        createWithCont success failure s

    // unwrap with continuation
    let apply f (EmailAddress e) = f e

    // unwrap directly
    let value e = apply id e

module ZipCode =

    type T = ZipCode of string

    // create with continuation
    let createWithCont success failure  (s:string) =
        if System.Text.RegularExpressions.Regex.IsMatch(s,@"^\d{5}$")
            then success (ZipCode s)
            else failure "Zip code must be 5 digits"

    // create directly
    let create s =
        let success e = Some e
        let failure _  = None
        createWithCont success failure s

    // unwrap with continuation
    let apply f (ZipCode e) = f e

    // unwrap directly
    let value e = apply id e

module StateCode =

    type T = StateCode of string

    // create with continuation
    let createWithCont success failure  (s:string) =
        let s' = s.ToUpper()
        let stateCodes = ["AZ";"CA";"NY"] //etc
        if stateCodes |> List.exists ((=) s')
            then success (StateCode s')
            else failure "State is not in list"

    // create directly
    let create s =
        let success e = Some e
        let failure _  = None
        createWithCont success failure s

    // unwrap with continuation
    let apply f (StateCode e) = f e

    // unwrap directly
    let value e = apply id e

type PersonalName =
    {
    FirstName: string;
    MiddleInitial: string option;
    LastName: string;
    }

type EmailContactInfo =
    {
    EmailAddress: EmailAddress.T;
    IsEmailVerified: bool;
    }

type PostalAddress =
    {
    Address1: string;
    Address2: string;
    City: string;
    State: StateCode.T;
    Zip: ZipCode.T;
    }

type PostalContactInfo =
    {
    Address: PostalAddress;
    IsAddressValid: bool;
    }

type Contact =
    {
    Name: PersonalName;
    EmailContactInfo: EmailContactInfo;
    PostalContactInfo: PostalContactInfo;
    }

```

顺便说一句，请注意，我们现在在三个包装器类型模块中有很多重复代码。什么是摆脱它的好方法，或者至少让它更干净？

## 摘要

总结可区分联合的使用情况，以下是一些指导方针：

- 请使用区分大小写的联合来创建准确表示域的类型。
- 如果包装的值需要验证，那么提供执行验证的构造函数并强制使用它们。
- 明确验证失败时会发生什么。在简单情况下，返回选项类型。在更复杂的情况下，让调用者传入成功和失败的处理程序。
- 如果包装值有许多相关函数，请考虑将其移动到自己的模块中。
- 如果需要强制封装，请使用签名文件。

我们还没有完成重构。我们可以改变类型的设计，在编译时强制执行业务规则，使非法状态无法表示。

## 更新

许多人要求提供更多信息，了解如何确保仅通过执行验证的特殊构造函数创建 `EmailAddress` 等受约束类型。所以我在这里创建了一个要点，其中有一些其他方法的详细示例。

# 3 用类型设计：使非法状态无法表示

*Part of the "Designing with types" series (*[link](https://fsharpforfunandprofit.com/posts/designing-with-types-making-illegal-states-unrepresentable/#series-toc)*)*

以类型编码业务逻辑
2013年1月14日 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/designing-with-types-making-illegal-states-unrepresentable/

在这篇文章中，我们将探讨 F# 的一个关键优势，即使用类型系统“使非法状态不可表示”（这是从Yaron Minsky借来的短语）。

让我们看看我们的联系人类型。由于之前的重构，它非常简单：

```F#
type Contact =
    {
    Name: Name;
    EmailContactInfo: EmailContactInfo;
    PostalContactInfo: PostalContactInfo;
    }
```

现在，假设我们有以下简单的业务规则：“联系人必须有电子邮件或邮政地址”。我们的类型符合这个规则吗？

答案是否定的。业务规则意味着联系人可能有一个电子邮件地址，但没有邮政地址，反之亦然。但就目前而言，我们的类型要求联系人必须始终拥有这两条信息。

答案似乎很明显——让地址可选，如下所示：

```F#
type Contact =
    {
    Name: PersonalName;
    EmailContactInfo: EmailContactInfo option;
    PostalContactInfo: PostalContactInfo option;
    }
```

但现在我们已经走得太远了。在这种设计中，联系人可能根本没有任何类型的地址。但商业规则规定，必须至少提供一条信息。

解决方案是什么？

## 使非法状态无法表示

如果我们仔细考虑业务规则，我们会意识到有三种可能性：

- 联系人只有一个电子邮件地址
- 联系人只有一个邮政地址
- 联系人同时拥有电子邮件地址和邮政地址

一旦这样放置，解决方案就变得显而易见了——使用一个联合类型，并为每种可能性添加一个案例。

```F#
type ContactInfo =
    | EmailOnly of EmailContactInfo
    | PostOnly of PostalContactInfo
    | EmailAndPost of EmailContactInfo * PostalContactInfo

type Contact =
    {
    Name: Name;
    ContactInfo: ContactInfo;
    }
```

这种设计完全符合要求。所有三种情况都明确表示，第四种可能的情况（根本没有电子邮件或邮政地址）是不允许的。

请注意，对于“电子邮件和帖子”的情况，我现在只使用了元组类型。它完全满足我们的需求。

## 构建 ContactInfo

现在让我们看看如何在实践中使用它。我们将首先创建一个新联系人：

```F#
let contactFromEmail name emailStr =
    let emailOpt = EmailAddress.create emailStr
    // handle cases when email is valid or invalid
    match emailOpt with
    | Some email ->
        let emailContactInfo =
            {EmailAddress=email; IsEmailVerified=false}
        let contactInfo = EmailOnly emailContactInfo
        Some {Name=name; ContactInfo=contactInfo}
    | None -> None

let name = {FirstName = "A"; MiddleInitial=None; LastName="Smith"}
let contactOpt = contactFromEmail name "abc@example.com"
```

在这段代码中，我们创建了一个简单的辅助函数 `contactFromEmail`，通过传入姓名和电子邮件来创建新联系人。但是，电子邮件可能无效，因此该函数必须处理这两种情况，它通过返回 `Contact option` 而不是 `Concat` 来实现

## 更新联系人信息

现在，如果我们需要向现有的 `ContactInfo` 添加邮政地址，我们别无选择，只能处理所有三种可能的情况：

- 如果联系人以前只有一个电子邮件地址，那么现在它既有电子邮件地址又有邮政地址，因此请使用 `EmailAndPost` 案例返回联系人。
- 如果联系人以前只有邮政地址，请使用 `PostOnly` 大小写返回联系人，替换现有地址。
- 如果联系人以前既有电子邮件地址又有邮政地址，请使用 `EmailAndPost` 案例返回联系人，替换现有地址。

这里有一个更新邮政地址的辅助方法。您可以看到它是如何显式处理每个案例的。

```F#
let updatePostalAddress contact newPostalAddress =
    let {Name=name; ContactInfo=contactInfo} = contact
    let newContactInfo =
        match contactInfo with
        | EmailOnly email ->
            EmailAndPost (email,newPostalAddress)
        | PostOnly _ -> // ignore existing address
            PostOnly newPostalAddress
        | EmailAndPost (email,_) -> // ignore existing address
            EmailAndPost (email,newPostalAddress)
    // make a new contact
    {Name=name; ContactInfo=newContactInfo}
```

以下是正在使用的代码：

```F#
let contact = contactOpt.Value   // see warning about option.Value below
let newPostalAddress =
    let state = StateCode.create "CA"
    let zip = ZipCode.create "97210"
    {
        Address =
            {
            Address1= "123 Main";
            Address2="";
            City="Beverly Hills";
            State=state.Value; // see warning about option.Value below
            Zip=zip.Value;     // see warning about option.Value below
            };
        IsAddressValid=false
    }
let newContact = updatePostalAddress contact newPostalAddress
```

*警告：我正在使用 *`option.Value`* 用于提取此代码中选项内容。这在交互式玩闹中是可以的，但在生产代码中是极其糟糕的做法！您应该始终使用匹配来处理选项的两种情况。*

## 为什么要费心制作这些复杂的类型？

在这一点上，你可能会说我们把事情变得不必要地复杂了。我想用以下几点来回答：

首先，商业逻辑很复杂。没有简单的方法可以避免它。如果你的代码没有这么复杂，你就没有正确处理所有情况。

其次，如果逻辑由类型表示，则它会自动自文档化。您可以查看下面的联合案例，立即了解业务规则是什么。您不必花任何时间试图分析任何其他代码。

```F#
type ContactInfo =
    | EmailOnly of EmailContactInfo
    | PostOnly of PostalContactInfo
    | EmailAndPost of EmailContactInfo * PostalContactInfo
```

最后，如果逻辑由类型表示，则对业务规则的任何更改都会立即产生突破性的更改，这通常是一件好事。

在下一篇文章中，我们将深入探讨最后一点。当您尝试使用类型表示业务逻辑时，您可能会突然发现这可以获得对该领域的全新见解。

> 如果你对领域建模和设计的功能方法感兴趣，你可能会喜欢我的《领域建模函数化》一书！这是一个初学者友好的介绍，涵盖了领域驱动设计、类型建模和函数式编程。

# 4 用类型设计：发现新概念

*Part of the "Designing with types" series (*[link](https://fsharpforfunandprofit.com/posts/designing-with-types-discovering-the-domain/#series-toc)*)*

深入了解该领域
2013年1月15日 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/designing-with-types-discovering-the-domain/

在上一篇文章中，我们研究了如何使用类型表示业务规则。

规则是：“联系人必须有电子邮件或邮政地址”。

我们设计的类型是：

```F#
type ContactInfo =
    | EmailOnly of EmailContactInfo
    | PostOnly of PostalContactInfo
    | EmailAndPost of EmailContactInfo * PostalContactInfo
```

现在，假设企业决定也需要支持电话号码。新的业务规则是：“*联系人必须至少有以下之一：电子邮件、邮政地址、家庭电话或工作电话*”。

我们现在该如何表达这一点？

稍加思考，就会发现这四种接触方法有15种可能的组合。我们当然不想创建一个有15个选择的联合案例吗？有更好的办法吗？

让我们坚持这个想法，看看一个不同但相关的问题。

## 当需求发生变化时，强制进行突破性的更改

这就是问题所在。假设你有一个联系人结构，其中包含一个电子邮件地址列表和一个邮政地址列表，如下所示：

```F#
type ContactInformation =
    {
    EmailAddresses : EmailContactInfo list;
    PostalAddresses : PostalContactInfo list
    }
```

此外，假设您创建了一个 `printReport` 函数，该函数循环遍历信息并在报告中打印出来：

```F#
// mock code
let printEmail emailAddress =
    printfn "Email Address is %s" emailAddress

// mock code
let printPostalAddress postalAddress =
    printfn "Postal Address is %s" postalAddress

let printReport contactInfo =
    let {
        EmailAddresses = emailAddresses;
        PostalAddresses = postalAddresses;
        } = contactInfo
    for email in emailAddresses do
         printEmail email
    for postalAddress in postalAddresses do
         printPostalAddress postalAddress
```

粗鲁，但简单易懂。

现在，如果新的业务规则生效，我们可能会决定改变结构，为电话号码添加一些新的列表。更新后的结构现在看起来像这样：

```F#
type PhoneContactInfo = string // dummy for now

type ContactInformation =
    {
    EmailAddresses : EmailContactInfo list;
    PostalAddresses : PostalContactInfo list;
    HomePhones : PhoneContactInfo list;
    WorkPhones : PhoneContactInfo list;
    }
```

如果您进行了此更改，您还需要确保处理联系人信息的所有功能都已更新，以处理新的手机案例。

当然，您将被迫修复任何中断的模式匹配。但在许多情况下，你不会被迫处理新案例。

例如，这里的 `printReport` 已更新以使用新列表：

```F#
let printReport contactInfo =
    let {
        EmailAddresses = emailAddresses;
        PostalAddresses = postalAddresses;
        } = contactInfo
    for email in emailAddresses do
         printEmail email
    for postalAddress in postalAddresses do
         printPostalAddress postalAddress
```

你能看出故意的错误吗？是的，我忘了更改函数来处理手机。记录中的新字段根本没有导致代码中断。不能保证你会记得处理新案件。这太容易忘记了。

我们再次面临挑战：我们能否设计类型，使这些情况不容易发生？

## 更深入地了解该领域

如果你更深入地思考这个例子，你会意识到我们只见树木，不见森林。

我们最初的概念是：“*要联系客户，将有一个可能的电子邮件列表和一个可能地址列表等*”。

但实际上，这一切都是错误的。一个更好的概念是：“*要联系客户，会有一个联系方式列表。每种联系方式都可以是电子邮件、邮政地址或电话号码*”。

这是对该领域应如何建模的关键见解。它创建了一种全新的类型，即“ContactMethod”，它一举解决了我们的问题。

我们可以立即重构类型以使用这个新概念：

```F#
type ContactMethod =
    | Email of EmailContactInfo
    | PostalAddress of PostalContactInfo
    | HomePhone of PhoneContactInfo
    | WorkPhone of PhoneContactInfo

type ContactInformation =
    {
    ContactMethods  : ContactMethod list;
    }
```

现在必须更改报告代码以处理新类型：

```F#
// mock code
let printContactMethod cm =
    match cm with
    | Email emailAddress ->
        printfn "Email Address is %s" emailAddress
    | PostalAddress postalAddress ->
         printfn "Postal Address is %s" postalAddress
    | HomePhone phoneNumber ->
        printfn "Home Phone is %s" phoneNumber
    | WorkPhone phoneNumber ->
        printfn "Work Phone is %s" phoneNumber

let printReport contactInfo =
    let {
        ContactMethods=methods;
        } = contactInfo
    methods
    |> List.iter printContactMethod
```

这些变化有很多好处。

首先，从建模的角度来看，新类型更好地代表了领域，并且更能适应不断变化的需求。

从开发的角度来看，将类型更改为联合意味着我们添加（或删除）的任何新案例都会以一种非常明显的方式破坏代码，并且更难意外忘记处理所有案例。

> 如果你觉得这篇文章很有趣，看看我的《领域建模函数化》一书！这是对领域驱动设计、类型建模和函数式编程的一个很好的介绍。

## 回到具有15种可能组合的业务规则

现在回到最初的例子。我们离开时认为，为了对业务规则进行编码，我们可能必须创建各种联系方式的 15 种可能组合。

但是，报告问题的新见解也影响了我们对业务规则的理解。

有了“联系方式”的概念，我们可以将要求重新表述为：“*客户必须至少有一种联系方式。联系方式可以是电子邮件、邮政地址或电话号码*”。

因此，让我们重新设计 `Contact` 类型，使其具有联系人方法列表：

```F#
type Contact =
    {
    Name: PersonalName;
    ContactMethods: ContactMethod list;
    }
```

但这仍然不太正确。列表可能为空。我们如何执行必须至少有一种联系方式的规则？

最简单的方法是创建一个必需的新字段，如下所示：

```F#
type Contact =
    {
    Name: PersonalName;
    PrimaryContactMethod: ContactMethod;
    SecondaryContactMethods: ContactMethod list;
    }
```

在此设计中，`PrimaryContactMethod` 是必需的，次要联系方式是可选的，这正是业务规则所要求的！

这种重构也给了我们一些见解。“主要”和“次要”联系方法的概念可能反过来又会澄清其他领域的代码，从而产生洞察力和重构的级联变化。

## 摘要

在这篇文章中，我们看到了如何使用类型对业务规则进行建模，实际上可以帮助您更深入地理解该领域。

在《领域驱动设计》一书中，Eric Evans 用了整整一节，特别是两章（第 8 章和第 9 章）来讨论重构对更深入洞察的重要性。相比之下，这篇文章中的例子很简单，但我希望它能表明这样的见解如何有助于提高模型和代码的正确性。

在下一篇文章中，我们将看到类型如何帮助表示细粒度状态。

# 5 用类型设计：使状态显式

*Part of the "Designing with types" series (*[link](https://fsharpforfunandprofit.com/posts/designing-with-types-representing-states/#series-toc)*)*

使用状态机确保正确性
2013年1月16日 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/designing-with-types-representing-states/

在这篇文章中，我们将研究如何使用状态机使隐式状态显式化，然后用联合类型对这些状态机进行建模。

## 背景

在本系列的早期文章中，我们将单案例联合视为电子邮件地址等类型的包装器。

```F#
module EmailAddress =

    type T = EmailAddress of string

    let create (s:string) =
        if System.Text.RegularExpressions.Regex.IsMatch(s,@"^\S+@\S+\.\S+$")
            then Some (EmailAddress s)
            else None
```

此代码假定地址有效或无效。如果不是，我们完全拒绝它，并返回 `None` 而不是有效值。

但也有不同程度的有效性。例如，如果我们想保留一个无效的电子邮件地址，而不仅仅是拒绝它，会发生什么？在这种情况下，像往常一样，我们希望使用类型系统来确保我们不会得到一个有效的地址与一个无效的地址混淆。

最明显的方法是使用联合类型：

```F#
module EmailAddress =

    type T =
        | ValidEmailAddress of string
        | InvalidEmailAddress of string

    let create (s:string) =
        if System.Text.RegularExpressions.Regex.IsMatch(s,@"^\S+@\S+\.\S+$")
            then ValidEmailAddress s    // change result type
            else InvalidEmailAddress s  // change result type

    // test
    let valid = create "abc@example.com"
    let invalid = create "example.com"
```

通过这些类型，我们可以确保只发送有效的电子邮件：

```F#
let sendMessageTo t =
    match t with
    | ValidEmailAddress email ->
         // send email
    | InvalidEmailAddress _ ->
         // ignore
```

到目前为止，一切顺利。这种设计现在应该对你来说很明显了。

但这种方法的适用范围比你想象的要广。在许多情况下，有类似的“状态”没有明确表示，而是用代码中的标志、枚举或条件逻辑来处理。

## 状态机

在上面的例子中，“有效”和“无效”的情况是相互不兼容的。也就是说，有效的电子邮件永远不会失效，反之亦然。

但在许多情况下，可能会因某种事件而从一个案例转到另一个案例。在这一点上，我们有一个“状态机”，其中每个案例都代表一个“状态”，从一个状态移动到另一个状态是一个“过渡”。

一些例子：

- 电子邮件地址可能有“未验证”和“已验证”状态，您可以通过要求用户单击确认电子邮件中的链接从“未验证的”状态转换为“已验证的”。

  状态转换图：已验证电子邮件

- 购物车可能有“空”、“活动”和“已支付”的状态，在这些状态下，您可以通过向购物车中添加商品从“空”状态转换为“活动”状态，并通过支付转换为“已付费”状态。

  状态转换图：购物车

- 像国际象棋这样的游戏可能有“WhiteToPlay”、“BlackToPlay”和“GameOver”状态，在这些状态下，你可以通过White做出非游戏结束的动作从“WhiteToPlay”状态转换到“BlackToGame”状态，或者通过玩对弈动作转换到“GameOver”状态。

  状态转换图：国际象棋游戏

在每种情况下，我们都有一组状态、一组转换和可以触发转换的事件。状态机通常用一个表来表示，比如购物车的这个表：

| 当前状态 | 事件-> | 添加物品           | 移除物品                                                     | 付款             |
| :------- | :----- | :----------------- | :----------------------------------------------------------- | :--------------- |
| Empty    |        | new state = Active | n/a                                                          | n/a              |
| Active   |        | new state = Active | new state = Active or Empty, depending on the number of items | new state = Paid |
| Paid     |        | n/a                | n/a                                                          | n/a              |

使用这样的表，您可以快速准确地看到系统处于给定状态时每个事件应该发生什么。

## 为什么要使用状态机？

在这些情况下使用状态机有很多好处：

**每个状态可以有不同的允许行为。**

在经过验证的电子邮件示例中，可能有一条业务规则规定，您只能将密码重置发送到已验证的电子邮件地址，而不能发送到未验证的地址。在购物车示例中，只有活动购物车可以付款，付费购物车不能添加。

**所有状态都有明确的记录。**

拥有隐含但从未记录的重要状态太容易了。

例如，“空购物车”与“活动购物车”的行为不同，但很少在代码中明确记录这一点。

**它是一种设计工具，迫使你思考可能发生的每一种可能性。**

错误的一个常见原因是某些边缘情况没有得到处理，但状态机会强制考虑所有情况。

例如，如果我们试图验证一封已经验证过的电子邮件，会发生什么？如果我们试图从空购物车中删除商品，会发生什么？当状态为“BlackToPlay”时，如果白方试图玩游戏，会发生什么？等等。

## 如何在 F# 中实现简单的状态机

您可能熟悉复杂的状态机，例如语言解析器和正则表达式中使用的状态机。这些状态机是由规则集或语法生成的，非常复杂。

我所说的状态机要简单得多。最多只有少数情况，转换次数很少，所以我们不需要使用复杂的生成器。

那么，实现这些简单状态机的最佳方式是什么？

通常，每个状态都有自己的类型，用于存储与该状态相关的数据（如果有的话），然后整个状态集将由一个联合类表示。

下面是一个使用购物车状态机的示例：

```F#
type ActiveCartData = { UnpaidItems: string list }
type PaidCartData = { PaidItems: string list; Payment: float }

type ShoppingCart =
    | EmptyCart  // no data
    | ActiveCart of ActiveCartData
    | PaidCart of PaidCartData
```

请注意，`EmptyCart` 状态没有数据，因此不需要特殊类型。

然后，每个事件都由一个函数表示，该函数接受整个状态机（联合类型）并返回状态机的新版本（同样是联合类型）。

以下是一个使用两个购物车事件的示例：

```F#
let addItem cart item =
    match cart with
    | EmptyCart ->
        // create a new active cart with one item
        ActiveCart {UnpaidItems=[item]}
    | ActiveCart {UnpaidItems=existingItems} ->
        // create a new ActiveCart with the item added
        ActiveCart {UnpaidItems = item :: existingItems}
    | PaidCart _ ->
        // ignore
        cart

let makePayment cart payment =
    match cart with
    | EmptyCart ->
        // ignore
        cart
    | ActiveCart {UnpaidItems=existingItems} ->
        // create a new PaidCart with the payment
        PaidCart {PaidItems = existingItems; Payment=payment}
    | PaidCart _ ->
        // ignore
        cart
```

您可以看到，从调用者的角度来看，对于一般操作（`ShoppingCart` 类型），状态集被视为“一件事”，但在内部处理事件时，每个状态都是单独处理的。

### 设计事件处理功能

*指南：事件处理函数应始终接受并返回整个状态机*

您可能会问：为什么我们必须将整个购物车传递给事件处理功能？例如，`makePayment` 事件仅在购物车处于活动状态时才具有相关性，那么为什么不直接将 ActiveCart 类型显式传递给它呢，如下所示：

```F#
let makePayment2 activeCart payment =
    let {UnpaidItems=existingItems} = activeCart
    {PaidItems = existingItems; Payment=payment}
```

让我们比较一下函数签名：

```F#
// the original function
val makePayment : ShoppingCart -> float -> ShoppingCart

// the new more specific function
val makePayment2 :  ActiveCartData -> float -> PaidCartData
```

您将看到原始的 `makePayment` 函数接受购物车并产生购物车，而新函数接受 `ActiveCartData` 并产生 `PaidCartData`，这似乎更相关。

但是，如果你这样做，当购物车处于不同状态（如空或已支付）时，你会如何处理同一事件？必须有人在某个地方处理所有三种可能状态的事件，将此业务逻辑封装在函数中比任由调用者摆布要好得多。

### 与“原始”状态合作

有时，你确实需要将其中一个状态视为一个独立的实体，并独立使用它。因为每个状态也是一种类型，所以这通常很简单。

例如，如果我需要报告所有付费购物车，我可以向它传递一个 `PaidCartData` 列表。

```F#
let paymentReport paidCarts =
    let printOneLine {Payment=payment} =
        printfn "Paid %f for items" payment
    paidCarts |> List.iter printOneLine
```

通过使用 `PaidCartData` 列表而不是 `ShoppingCart` 本身作为参数，我确保不会意外报告未付款的购物车。

如果你这样做，它应该在事件处理程序的支持函数中，而不是事件处理程序本身。

## 使用显式状态替换布尔标志

现在让我们看看如何将这种方法应用于一个真实的例子。

在早期帖子中的 `Contact` 示例中，我们有一个标志，用于指示客户是否已验证其电子邮件地址。类型看起来像这样：

```F#
type EmailContactInfo =
    {
    EmailAddress: EmailAddress.T;
    IsEmailVerified: bool;
    }
```

每当你看到这样的旗帜，你很可能是在和状态打交道。在这种情况下，布尔值用于表示我们有两个状态：“未验证”和“已验证”。

如上所述，可能会有各种与每个状态允许的内容相关的业务规则。例如，这里有两个：

- 业务规则：“*验证电子邮件只能发送给拥有未验证电子邮件地址的客户*”
- 业务规则：“*密码重置电子邮件应仅发送给已验证电子邮件地址的客户*”

和以前一样，我们可以使用类型来确保代码符合这些规则。

让我们使用状态机重写 `EmailContactInfo` 类型。我们也将把它放在一个模块中。

我们将首先定义这两个状态。

- 对于“未验证”状态，我们需要保留的唯一数据是电子邮件地址。
- 对于“已验证”状态，除了电子邮件地址外，我们可能还希望保留一些额外的数据，例如验证日期、最近密码重置次数等。这些数据与“未验证”状态无关（甚至不应该可见）。

```F#
module EmailContactInfo =
    open System

    // placeholder
    type EmailAddress = string

    // UnverifiedData = just the email
    type UnverifiedData = EmailAddress

    // VerifiedData = email plus the time it was verified
    type VerifiedData = EmailAddress * DateTime

    // set of states
    type T =
        | UnverifiedState of UnverifiedData
        | VerifiedState of VerifiedData

```

请注意，对于 `UnverifiedData` 类型，我只使用了一个类型别名。现在不需要任何更复杂的东西，但使用类型别名可以明确目的，并有助于重构。

现在，让我们处理新状态机的构建，然后处理事件。

- 构造*总是*导致未经验证的电子邮件，所以这很容易。
- 只有一个事件可以从一种状态转换到另一种状态：“已验证”事件。

```F#
module EmailContactInfo =

    // types as above

    let create email =
        // unverified on creation
        UnverifiedState email

    // handle the "verified" event
    let verified emailContactInfo dateVerified =
        match emailContactInfo with
        | UnverifiedState email ->
            // construct a new info in the verified state
            VerifiedState (email, dateVerified)
        | VerifiedState _ ->
            // ignore
            emailContactInfo
```

请注意，正如这里所讨论的，匹配的每个分支都必须返回相同的类型，因此在忽略已验证的状态时，我们仍然必须返回一些东西，比如传入的对象。

最后，我们可以编写两个实用函数 `sendVerificationEmail` 和 `sendPasswordReset`。

```F#
module EmailContactInfo =

    // types and functions as above

    let sendVerificationEmail emailContactInfo =
        match emailContactInfo with
        | UnverifiedState email ->
            // send email
            printfn "sending email"
        | VerifiedState _ ->
            // do nothing
            ()

    let sendPasswordReset emailContactInfo =
        match emailContactInfo with
        | UnverifiedState email ->
            // ignore
            ()
        | VerifiedState _ ->
            // ignore
            printfn "sending password reset"
```

> 如果你觉得这篇文章很有趣，看看我的《领域建模函数化》一书！这是对领域驱动设计、类型建模和函数式编程的一个很好的介绍。

## 使用显式案例替换 case/switch 语句

有时，它不仅仅是一个用于指示状态的简单布尔标志。在 C# 和 Java 中，通常使用 `int` 或 `enum` 来表示一组状态。

例如，这是一个交付系统的包裹状态的简单状态图，其中包裹有三种可能的状态：

状态转换图：包裹交付

从这个图中可以看出一些明显的业务规则：

- 规则：“*如果一个包裹已经准备好送货，你就不能把它放在卡车上*”
- 规则：“*您不能签收已送达的包裹*”

等等。

现在，在不使用联合类型的情况下，我们可以通过使用枚举来表示状态来表示这种设计，如下所示：

```F#
open System

type PackageStatus =
    | Undelivered
    | OutForDelivery
    | Delivered

type Package =
    {
    PackageId: int;
    PackageStatus: PackageStatus;
    DeliveryDate: DateTime;
    DeliverySignature: string;
    }
```

然后处理“putOnTruck”和“signedFor”事件的代码可能如下：

```F#
let putOnTruck package =
    {package with PackageStatus=OutForDelivery}

let signedFor package signature =
    let {PackageStatus=packageStatus} = package
    if (packageStatus = Undelivered)
    then
        failwith "package not out for delivery"
    else if (packageStatus = OutForDelivery)
    then
        {package with
            PackageStatus=OutForDelivery;
            DeliveryDate = DateTime.UtcNow;
            DeliverySignature=signature;
            }
    else
        failwith "package already delivered"
```

这段代码中有一些微妙的错误。

- 在处理“putOnTruck”事件时，如果状态已经为 `OutForDelivery` 或 `Delivered`，应该发生什么。代码对此并不明确。
- 在处理“signedFor”事件时，我们确实会处理其他状态，但最后一个else分支假设我们只有三个状态，因此不会费心明确测试它。如果我们添加了新的状态，这段代码将是不正确的。
- 最后，由于 `DeliveryDate` 和 `DeliverySignature` 处于基本结构中，即使状态未 `Delivered`，也可能意外设置它们。

但像往常一样，惯用的、更类型安全的 F# 方法是使用整体联合类型，而不是在数据结构中嵌入状态值。

```F#
open System

type UndeliveredData =
    {
    PackageId: int;
    }

type OutForDeliveryData =
    {
    PackageId: int;
    }

type DeliveredData =
    {
    PackageId: int;
    DeliveryDate: DateTime;
    DeliverySignature: string;
    }

type Package =
    | Undelivered of UndeliveredData
    | OutForDelivery of OutForDeliveryData
    | Delivered of DeliveredData
```

然后，事件处理程序必须处理每种情况。

```F#
let putOnTruck package =
    match package with
    | Undelivered {PackageId=id} ->
        OutForDelivery {PackageId=id}
    | OutForDelivery _ ->
        failwith "package already out"
    | Delivered _ ->
        failwith "package already delivered"

let signedFor package signature =
    match package with
    | Undelivered _ ->
        failwith "package not out"
    | OutForDelivery {PackageId=id} ->
        Delivered {
            PackageId=id;
            DeliveryDate = DateTime.UtcNow;
            DeliverySignature=signature;
            }
    | Delivered _ ->
        failwith "package already delivered"
```

*注意：我使用 `failWith` 来处理错误。在生产系统中，此代码应替换为客户端驱动的错误处理程序。有关一些想法，请参阅关于单案例 DU 的帖子中关于处理构造函数错误的讨论。*

## 使用显式案例替换隐式条件代码

最后，在某些情况下，系统有状态，但它们隐含在条件代码中。

例如，这里有一个表示订单的类型。

```F#
open System

type Order =
    {
    OrderId: int;
    PlacedDate: DateTime;
    PaidDate: DateTime option;
    PaidAmount: float option;
    ShippedDate: DateTime option;
    ShippingMethod: string option;
    ReturnedDate: DateTime option;
    ReturnedReason: string option;
    }
```

您可以猜测订单可以是“新”、“已支付”、“发货”或“退回”，并且每个转换都有时间戳和额外信息，但这在结构中没有明确说明。

选项类型表明这种类型试图做得太多。至少 F# 强制你使用选项——在 C# 或 Java 中，这些选项可能只是空值，你从类型定义中不知道它们是否是必需的。

现在，让我们看看可能测试这些选项类型以查看顺序状态的丑陋代码。

同样，有一些重要的业务逻辑取决于订单的状态，但没有明确记录各种状态和转换是什么。

```F#
let makePayment order payment =
    if (order.PaidDate.IsSome)
    then failwith "order is already paid"
    //return an updated order with payment info
    {order with
        PaidDate=Some DateTime.UtcNow
        PaidAmount=Some payment
        }

let shipOrder order shippingMethod =
    if (order.ShippedDate.IsSome)
    then failwith "order is already shipped"
    //return an updated order with shipping info
    {order with
        ShippedDate=Some DateTime.UtcNow
        ShippingMethod=Some shippingMethod
        }
```

*注意：我添加了 `IsSome` 来测试选项值是否作为 C# 程序测试 `null` 的直接端口存在。但 `IsSome` 既丑陋又危险。不要用它！*

这里有一种更好的方法，使用使状态显式的类型。

```F#
open System

type InitialOrderData =
    {
    OrderId: int;
    PlacedDate: DateTime;
    }
type PaidOrderData =
    {
    Date: DateTime;
    Amount: float;
    }
type ShippedOrderData =
    {
    Date: DateTime;
    Method: string;
    }
type ReturnedOrderData =
    {
    Date: DateTime;
    Reason: string;
    }

type Order =
    | Unpaid of InitialOrderData
    | Paid of InitialOrderData * PaidOrderData
    | Shipped of InitialOrderData * PaidOrderData * ShippedOrderData
    | Returned of InitialOrderData * PaidOrderData * ShippedOrderData * ReturnedOrderData
```

以下是事件处理方法：

```F#
let makePayment order payment =
    match order with
    | Unpaid i ->
        let p = {Date=DateTime.UtcNow; Amount=payment}
        // return the Paid order
        Paid (i,p)
    | _ ->
        printfn "order is already paid"
        order

let shipOrder order shippingMethod =
    match order with
    | Paid (i,p) ->
        let s = {Date=DateTime.UtcNow; Method=shippingMethod}
        // return the Shipped order
        Shipped (i,p,s)
    | Unpaid _ ->
        printfn "order is not paid for"
        order
    | _ ->
        printfn "order is already shipped"
        order
```

*注意：这里我使用 `printfn` 来处理错误。在生产系统中，请使用不同的方法。*

## 何时不使用此方法

就像我们学习的任何技术一样，我们必须小心，不要把它当作金锤。

这种方法确实增加了复杂性，因此在开始使用它之前，请确保收益大于成本。

总而言之，以下是使用简单状态机可能有益的条件：

- 你有一组互斥的状态，它们之间有转换。
- 这些转变是由外部事件触发的。
- 各状态是详尽无遗的。也就是说，没有其他选择，你必须始终处理所有案例。
- 当系统处于另一个状态时，每个状态都可能有不可访问的关联数据。
- 存在适用于各状态的静态业务规则。

让我们看看这些指导方针不适用的一些例子。

### 状态在这一领域并不重要。

考虑一个博客创作应用程序。通常，每篇博客文章都可以处于“草稿”、“已发布”等状态。这些状态之间显然存在由事件驱动的转换（例如单击“发布”按钮）。

但值得为此创建一个状态机吗？一般来说，我会说不是。

是的，有状态转换，但真的会因此而改变逻辑吗？从创作的角度来看，大多数博客应用程序都没有基于状态的任何限制。您可以以与撰写已发布帖子完全相同的方式撰写草稿帖子。

系统中唯一关心状态的部分是显示引擎，它在草稿到达域之前过滤掉数据库层中的草稿。

由于没有关心状态的特殊域逻辑，因此可能没有必要。

### 状态转换发生在应用程序外部

在客户管理应用程序中，通常将客户分为“潜在客户”、“活跃”、“非活跃”等。

状态转换图：客户状态

在应用程序中，这些状态具有业务意义，应该由类型系统（如联合类型）表示。但状态转换通常不会在应用程序本身内发生。例如，如果客户已经6个月没有订购任何东西，我们可能会将其归类为非活跃客户。然后，此规则可能会通过夜间批处理作业应用于数据库中的客户记录，或者在从数据库加载客户记录时应用。但从应用程序的角度来看，转换不会在应用程序内发生，因此我们不需要创建特殊的状态机。

### 动态业务规则

上面列表中的最后一个要点是指“静态”业务规则。我的意思是，规则变化得足够慢，应该嵌入到代码本身中。

另一方面，如果规则是动态的并且经常变化，那么创建静态类型的麻烦可能就不值得了。

在这些情况下，您应该考虑使用主动模式，甚至使用适当的规则引擎。

## 摘要

在这篇文章中，我们看到，如果你有带有显式标志（“IsVerified”）或状态字段（“OrderStatus”）或隐式状态（由过多的可空或选项类型组成）的数据结构，那么值得考虑使用简单的状态机来建模域对象。在大多数情况下，额外的复杂性可以通过明确记录状态和消除因未处理所有可能情况而导致的错误来补偿。

# 6 用类型设计：受约束的字符串

*Part of the "Designing with types" series (*[link](https://fsharpforfunandprofit.com/posts/designing-with-types-more-semantic-types/#series-toc)*)*

向基元类型添加更多语义信息
2013年1月17日 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/designing-with-types-more-semantic-types/

在上一篇文章中，我谈到了避免对电子邮件地址、邮政编码、州等使用纯基元字符串。通过将它们包装在一个单案例联合中，我们可以（a）强制类型不同，（b）添加验证规则。

在这篇文章中，我们将探讨是否可以将这一概念扩展到更细粒度的级别。

## 什么时候字符串不是字符串？

让我们来看一个简单的 `PersonalName` 类型。

```F#
type PersonalName =
    {
    FirstName: string;
    LastName: string;
    }
```

类型说名字是一个 `string`。但实际上，就这些吗？我们可能还需要添加其他限制吗？

好吧，它不能为空。但这在 F# 中是假设的。

绳子的长度是多少？64K 字符长的名字可以接受吗？如果没有，那么是否有允许的最大长度？

名称可以包含换行符或制表符吗？它可以以空格开头或结尾吗？

一旦你这样说，即使是“通用”字符串也有很多约束。以下是一些明显的约束：

- 它的最大长度是多少？
- 它能跨越多行吗？
- 它可以有前导或尾随空格吗？
- 它可以包含非打印字符吗？

## 这些约束是否应该成为领域模型的一部分？

因此，我们可能会承认存在一些约束，但它们真的应该是领域模型的一部分吗（以及从中派生出的相应类型）？例如，姓氏限制为100个字符的约束——这肯定是特定于特定实现的，根本不是域的一部分。

我会回答说，逻辑模型和物理模型之间是有区别的。在逻辑模型中，这些约束中的一些可能不相关，但在物理模型中，它们肯定是相关的。当我们编写代码时，我们总是在处理物理模型。

将约束合并到模型中的另一个原因是，模型通常在许多单独的应用程序中共享。例如，可以在电子商务应用程序中创建个人姓名，该应用程序将其写入数据库表，然后将其放入消息队列中，由 CRM 应用程序提取，CRM 应用程序进而调用电子邮件模板服务，等等。

重要的是，所有这些应用程序和服务都对个人姓名有相同的理解，包括长度和其他限制。如果模型没有明确约束，那么在跨服务边界移动时很容易出现不匹配。

例如，您是否编写过在将字符串写入数据库之前检查其长度的代码？

```c#
void SaveToDatabase(PersonalName personalName)
{
   var first = personalName.First;
   if (first.Length > 50)
   {
        // ensure string is not too long
        first = first.Substring(0,50);
   }

   //save to database
}
```

如果此时字符串太长，你应该怎么做？默默地截断它？抛出异常？

如果可能的话，更好的答案是完全避免这个问题。当字符串到达数据库层时，已经太晚了——数据库层不应该做出这类决定。

这个问题应该在字符串首次创建时处理，而不是在使用时处理。换句话说，它应该是字符串验证的一部分。

但是，我们怎么能相信所有可能的路径都已正确完成验证呢？我想你能猜出答案…

## 使用类型对受约束字符串进行建模

当然，答案是创建具有内置约束的包装器类型。

所以，让我们使用我们之前使用的单案例联合技术来快速构建一个原型。

```F#
module String100 =
    type T = String100 of string
    let create (s:string) =
        if s <> null && s.Length <= 100
        then Some (String100 s)
        else None
    let apply f (String100 s) = f s
    let value s = apply id s

module String50 =
    type T = String50 of string
    let create (s:string) =
        if s <> null && s.Length <= 50
        then Some (String50 s)
        else None
    let apply f (String50 s) = f s
    let value s = apply id s

module String2 =
    type T = String2 of string
    let create (s:string) =
        if s <> null && s.Length <= 2
        then Some (String2 s)
        else None
    let apply f (String2 s) = f s
    let value s = apply id s
```

请注意，当验证失败时，我们必须立即使用选项类型作为结果来处理这种情况。它使创造更加痛苦，但如果我们以后想要好处，就无法避免。

例如，这里有一个好字符串和一个长度为2的坏字符串。

```F#
let s2good = String2.create "CA"
let s2bad = String2.create "California"

match s2bad with
| Some s2 -> // update domain object
| None -> // handle error
```

为了使用 `String2` 值，我们必须在创建时检查它是 `Some` 还是 `None`。

### 此设计的问题

一个问题是我们有很多重复的代码。在实践中，一个典型的域只有几十种字符串类型，因此不会浪费那么多代码。但是，我们可能会做得更好。

另一个更严重的问题是，比较变得更加困难。`String50` 与 `String100` 的类型不同，因此无法直接进行比较。

```F#
let s50 = String50.create "John"
let s100 = String100.create "Smith"

let s50' = s50.Value
let s100' = s100.Value

let areEqual = (s50' = s100')  // compiler error
```

这种事情会让使用字典和列表变得更加困难。

### 重构

此时，我们可以利用 F# 对接口的支持，创建一个所有包装字符串都必须支持的通用接口，以及一些标准函数：

```F#
module WrappedString =

    /// An interface that all wrapped strings support
    type IWrappedString =
        abstract Value : string

    /// Create a wrapped value option
    /// 1) canonicalize the input first
    /// 2) If the validation succeeds, return Some of the given constructor
    /// 3) If the validation fails, return None
    /// Null values are never valid.
    let create canonicalize isValid ctor (s:string) =
        if s = null
        then None
        else
            let s' = canonicalize s
            if isValid s'
            then Some (ctor s')
            else None

    /// Apply the given function to the wrapped value
    let apply f (s:IWrappedString) =
        s.Value |> f

    /// Get the wrapped value
    let value s = apply id s

    /// Equality test
    let equals left right =
        (value left) = (value right)

    /// Comparison
    let compareTo left right =
        (value left).CompareTo (value right)
```

关键函数是 `create`，它接受一个构造函数，并仅在验证通过时使用它创建新值。

有了这个，定义新类型就容易多了：

```F#
module WrappedString =

    // ... code from above ...

    /// Canonicalizes a string before construction
    /// * converts all whitespace to a space char
    /// * trims both ends
    let singleLineTrimmed s =
        System.Text.RegularExpressions.Regex.Replace(s,"\s"," ").Trim()

    /// A validation function based on length
    let lengthValidator len (s:string) =
        s.Length <= len

    /// A string of length 100
    type String100 = String100 of string with
        interface IWrappedString with
            member this.Value = let (String100 s) = this in s

    /// A constructor for strings of length 100
    let string100 = create singleLineTrimmed (lengthValidator 100) String100

    /// Converts a wrapped string to a string of length 100
    let convertTo100 s = apply string100 s

    /// A string of length 50
    type String50 = String50 of string with
        interface IWrappedString with
            member this.Value = let (String50 s) = this in s

    /// A constructor for strings of length 50
    let string50 = create singleLineTrimmed (lengthValidator 50)  String50

    /// Converts a wrapped string to a string of length 50
    let convertTo50 s = apply string50 s
```

现在，对于每种类型的字符串，我们只需要：

- 创建一个类型（例如 `String100`）
- 该类型的 `IWrappedString` 的实现
- 以及该类型的公共构造函数（例如 `string100`）。

（在上面的示例中，我还抛出了一个有用的 `convertTo`，用于从一种类型转换为另一种类型。）

正如我们之前看到的，该类型是一种简单的包裹类型。

IWrappedString 的 `Value` 方法的实现可以使用多行编写，如下所示：

```F#
member this.Value =
    let (String100 s) = this
    s
```

但我选择使用单行快捷写法：

```F#
member this.Value = let (String100 s) = this in s
```

构造函数也很简单。规范化函数是 `singleLineTrimmed`，验证器函数检查长度，构造函数是 `String100` 函数（与单个案例相关的函数，不要与同名类型混淆）。

```F#
let string100 = create singleLineTrimmed (lengthValidator 100) String100
```

如果你想有其他具有不同约束的类型，你可以很容易地添加它们。例如，您可能希望有一个支持多行和嵌入式选项卡且不修剪的 `Text1000` 类型。

```F#
module WrappedString =

    // ... code from above ...

    /// A multiline text of length 1000
    type Text1000 = Text1000 of string with
        interface IWrappedString with
            member this.Value = let (Text1000 s) = this in s

    /// A constructor for multiline strings of length 1000
    let text1000 = create id (lengthValidator 1000) Text1000
```

### 使用 WrappedString 模块

我们现在可以交互式地玩这个模块，看看它是如何工作的：

```F#
let s50 = WrappedString.string50 "abc" |> Option.get
printfn "s50 is %A" s50
let bad = WrappedString.string50 null
printfn "bad is %A" bad
let s100 = WrappedString.string100 "abc" |> Option.get
printfn "s100 is %A" s100

// equality using module function is true
printfn "s50 is equal to s100 using module equals? %b" (WrappedString.equals s50 s100)

// equality using Object method is false
printfn "s50 is equal to s100 using Object.Equals? %b" (s50.Equals s100)

// direct equality does not compile
printfn "s50 is equal to s100? %b" (s50 = s100) // compiler error
```

当我们需要与使用原始字符串的映射等类型交互时，很容易编写新的辅助函数。

例如，这里有一些使用映射（maps）的助手：

```F#
module WrappedString =

    // ... code from above ...

    /// map helpers
    let mapAdd k v map =
        Map.add (value k) v map

    let mapContainsKey k map =
        Map.containsKey (value k) map

    let mapTryFind k map =
        Map.tryFind (value k) map
```

以下是这些助手在实践中的使用方法：

```F#
let abc = WrappedString.string50 "abc" |> Option.get
let def = WrappedString.string100 "def" |> Option.get
let map =
    Map.empty
    |> WrappedString.mapAdd abc "value for abc"
    |> WrappedString.mapAdd def "value for def"

printfn "Found abc in map? %A" (WrappedString.mapTryFind abc map)

let xyz = WrappedString.string100 "xyz" |> Option.get
printfn "Found xyz in map? %A" (WrappedString.mapTryFind xyz map)
```

所以总的来说，这个“WrappedString”模块允许我们创建类型良好的字符串，而不会干扰太多。现在让我们在实际情况中使用它。

## 在域中使用新的字符串类型

现在我们有了类型，我们可以更改 `PersonalName` 类型的定义来使用它们。

```F#
module PersonalName =
    open WrappedString

    type T =
        {
        FirstName: String50;
        LastName: String100;
        }

    /// create a new value
    let create first last =
        match (string50 first),(string100 last) with
        | Some f, Some l ->
            Some {
                FirstName = f;
                LastName = l;
                }
        | _ ->
            None
```

我们为该类型创建了一个模块，并添加了一个创建函数，该函数将一对字符串转换为 `PersonalName`。

请注意，如果任一输入字符串无效，我们必须决定如何处理。同样，我们不能把这个问题推迟到以后，我们必须在施工时处理。

在这种情况下，我们使用使用None创建选项类型的简单方法来表示失败。

它在这里使用：

```F#
let name = PersonalName.create "John" "Smith"
```

我们还可以在模块中提供额外的辅助函数。

例如，假设我们想创建一个 `fullname` 函数，该函数将返回连接在一起的名字和姓氏。

再次，需要做出更多的决定。

- 我们应该返回原始字符串还是包裹字符串？后者的优点是调用者确切地知道字符串的长度，并且它将与其他类似类型兼容。
- 如果我们确实返回一个包装字符串（比如 `String100`），那么当组合长度太长时，我们该如何处理这种情况？（根据名字和姓氏类型的长度，最多可达 151 个字符。）。我们可以返回一个选项，或者如果组合长度太长，则强制截断。

这是演示所有三个选项的代码。

```F#
module PersonalName =

    // ... code from above ...

    /// concat the first and last names together
    /// and return a raw string
    let fullNameRaw personalName =
        let f = personalName.FirstName |> value
        let l = personalName.LastName |> value
        f + " " + l

    /// concat the first and last names together
    /// and return None if too long
    let fullNameOption personalName =
        personalName |> fullNameRaw |> string100

    /// concat the first and last names together
    /// and truncate if too long
    let fullNameTruncated personalName =
        // helper function
        let left n (s:string) =
            if (s.Length > n)
            then s.Substring(0,n)
            else s

        personalName
        |> fullNameRaw  // concat
        |> left 100     // truncate
        |> string100    // wrap
        |> Option.get   // this will always be ok
```

实现 `fullName` 的具体方法取决于您。但它展示了这种面向类型的设计风格的一个关键点：在创建代码时，必须预先做出这些决定。你不能把它们推迟到以后。

这有时会很烦人，但总的来说，我认为这是一件好事。

## 重新查看电子邮件地址和邮政编码类型

我们可以使用此 WrappedString 模块重新实现 `EmailAddress` 和 `ZipCode` 类型。

```F#
module EmailAddress =

    type T = EmailAddress of string with
        interface WrappedString.IWrappedString with
            member this.Value = let (EmailAddress s) = this in s

    let create =
        let canonicalize = WrappedString.singleLineTrimmed
        let isValid s =
            (WrappedString.lengthValidator 100 s) &&
            System.Text.RegularExpressions.Regex.IsMatch(s,@"^\S+@\S+\.\S+$")
        WrappedString.create canonicalize isValid EmailAddress

    /// Converts any wrapped string to an EmailAddress
    let convert s = WrappedString.apply create s

module ZipCode =

    type T = ZipCode of string with
        interface WrappedString.IWrappedString with
            member this.Value = let (ZipCode s) = this in s

    let create =
        let canonicalize = WrappedString.singleLineTrimmed
        let isValid s =
            System.Text.RegularExpressions.Regex.IsMatch(s,@"^\d{5}$")
        WrappedString.create canonicalize isValid ZipCode

    /// Converts any wrapped string to a ZipCode
    let convert s = WrappedString.apply create s
```

## 包装字符串的其他用途

这种包装字符串的方法也可用于其他不想意外将字符串类型混合在一起的场景。

一个跃入脑海的例子是确保在 web 应用程序中安全引用和取消引用字符串。

例如，假设您想将字符串输出到 HTML。字符串是否应该被转义？如果它已经逃脱了，你想让它一个人呆着，但如果它没有，你确实想逃脱它。

这可能是一个棘手的问题。Joel Spolsky 在这里讨论了使用命名约定，但当然，在 F# 中，我们想要一个基于类型的解决方案。

基于类型的解决方案可能会将一个类型用于“安全”（已经转义的）HTML 字符串（比如 `HtmlString`），一个用于安全的 Javascript 字符串（`JsString`），一种用于安全的 SQL 字符串（`SqlString`）等。然后，这些字符串可以安全地混合和匹配，而不会意外导致安全问题。

我不会在这里创建一个解决方案（而且你可能会使用类似 Razor 的东西），但如果你感兴趣，你可以在这里阅读 Haskell 方法以及将其移植到 F# 的方法。

## 更新

许多人要求提供更多信息，了解如何确保仅通过执行验证的特殊构造函数创建 `EmailAddress` 等受约束类型。所以我在这里创建了一个要点，其中有一些其他方法的详细示例。

> 如果你对领域建模和设计的功能方法感兴趣，你可能会喜欢我的《领域建模函数化》一书！这是一个初学者友好的介绍，涵盖了领域驱动设计、类型建模和函数式编程。

# 7 使用类型进行设计：非字符串类型

*Part of the "Designing with types" series (*[link](https://fsharpforfunandprofit.com/posts/designing-with-types-non-strings/#series-toc)*)*

安全地处理整数和日期
2013年1月18日 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/designing-with-types-non-strings/

在本系列中，我们看到了很多使用区分大小写的联合来包装字符串的方法。

没有理由不能将此技术用于其他基本类型，如数字和日期。让我们看几个例子。

## 单一案例联合

在许多情况下，我们希望避免意外混淆不同类型的整数。两个域对象可能具有相同的表示（使用整数），但不应混淆。

例如，您可能有一个 `OrderId` 和一个 `CustomerId`，两者都存储为整数。但它们并不是真正的整数。例如，您无法将 42 添加到 `CustomerId`。并且 `CustomerId(42)`不等于 `OrderId(42)`。事实上，他们根本不应该被允许进行比较。

当然，类型来帮忙了。

```F#
type CustomerId = CustomerId of int
type OrderId = OrderId of int

let custId = CustomerId 42
let orderId = OrderId 42

// compiler error
printfn "cust is equal to order? %b" (custId = orderId)
```

同样，您可能希望通过将语义上不同的日期值包装在一个类型中来避免混淆它们。（`DateTimeKind` 是一种尝试，但并不总是可靠的。）

```F#
type LocalDttm = LocalDttm of System.DateTime
type UtcDttm = UtcDttm of System.DateTime
```

使用这些类型，我们可以确保始终传递正确类型的日期时间作为参数。此外，它还充当文档。

```F#
let SetOrderDate (d:LocalDttm) =
    () // do something

let SetAuditTimestamp (d:UtcDttm) =
    () // do something
```

## 整数约束

正如我们对 `String50` 和 `ZipCode` 等类型进行了验证和约束一样，当我们需要对整数进行约束时，我们也可以使用相同的方法。

例如，库存管理系统或购物车可能要求某些类型的数字始终为正数。您可以通过创建 `NonNegativeInt` 类型来确保这一点。

```F#
module NonNegativeInt =
    type T = NonNegativeInt of int

    let create i =
        if (i >= 0 )
        then Some (NonNegativeInt i)
        else None

module InventoryManager =

    // example of NonNegativeInt in use
    let SetStockQuantity (i:NonNegativeInt.T) =
        //set stock
        ()
```

## 在类型中嵌入业务规则

正如我们之前想知道名字是否可以长达 64K 个字符一样，你真的能在购物车中添加 999999 件商品吗？

状态转换图：包裹交付

是否值得通过使用约束类型来避免这个问题？让我们看看一些真实的代码。

这是一个非常简单的购物车管理器，使用标准的 `int` 类型表示数量。点击相关按钮时，数量会增加或减少。你能找到明显的bug吗？

```F#
module ShoppingCartWithBug =

    let mutable itemQty = 1  // don't do this at home!

    let incrementClicked() =
        itemQty <- itemQty + 1

    let decrementClicked() =
        itemQty <- itemQty - 1
```

如果你不能很快找到这个 bug，也许你应该考虑让任何约束更加明确。

这是一个使用键入数量的简单购物车管理器。你现在能找到 bug 吗？（提示：将代码粘贴到 F# 脚本文件中并运行）

```F#
module ShoppingCartQty =

    type T = ShoppingCartQty of int

    let initialValue = ShoppingCartQty 1

    let create i =
        if (i > 0 && i < 100)
        then Some (ShoppingCartQty i)
        else None

    let increment t = create (t + 1)
    let decrement t = create (t - 1)

module ShoppingCartWithTypedQty =

    let mutable itemQty = ShoppingCartQty.initialValue

    let incrementClicked() =
        itemQty <- ShoppingCartQty.increment itemQty

    let decrementClicked() =
        itemQty <- ShoppingCartQty.decrement itemQty
```

你可能会认为，对于这样一个微不足道的问题来说，这太过分了。但如果你想避免 DailyWTF，这可能值得考虑。

> 如果你觉得这篇文章很有趣，看看我的《领域建模函数化》一书！这是对领域驱动设计、类型建模和函数式编程的一个很好的介绍。

## 日期限制

并非所有系统都能处理所有可能的日期。有些系统只能存储可追溯到1980年1月1日的日期，有些系统只能保存到2038年之前的未来日期（我喜欢使用2038年1月一日作为最大日期，以避免美国/英国的月/日订单问题）。

与整数一样，在类型中内置对有效日期的约束可能是有用的，这样任何越界问题都可以在构造时而不是以后处理。

```F#
type SafeDate = SafeDate of System.DateTime

let create dttm =
    let min = new System.DateTime(1980,1,1)
    let max = new System.DateTime(2038,1,1)
    if dttm < min || dttm > max
    then None
    else Some (SafeDate dttm)
```

## 联合类型 vs 计量单位

你可能会问：计量单位呢？难道它们不应该用于这个目的吗？

是和否。度量单位确实可以用来避免混淆不同类型的数值，并且比我们一直使用的单案例联合强大得多。

另一方面，度量单位没有封装，不能有约束。任何人都可以创建一个以 `<kg>` 为单位的 int，并且没有最小值或最大值。

在许多情况下，这两种方法都能很好地工作。例如，有很多部分 .NET 库使用超时，但有时超时以秒为单位设置，有时以毫秒为单位设置。我经常记不清哪个是哪个。我绝对不想意外地使用 1000 秒的超时，而我真正的意思是 1000 毫秒的超时。

为了避免这种情况，我经常喜欢为秒和毫秒创建单独的类型。

这是一种使用单案例联合的基于类型的方法：

```F#
type TimeoutSecs = TimeoutSecs of int
type TimeoutMs = TimeoutMs of int

let toMs (TimeoutSecs secs)  =
    TimeoutMs (secs * 1000)

let toSecs (TimeoutMs ms) =
    TimeoutSecs (ms / 1000)

/// sleep for a certain number of milliseconds
let sleep (TimeoutMs ms) =
    System.Threading.Thread.Sleep ms

/// timeout after a certain number of seconds
let commandTimeout (TimeoutSecs s) (cmd:System.Data.IDbCommand) =
    cmd.CommandTimeout <- s
```

使用度量单位也是一样的：

```F#
[<Measure>] type sec
[<Measure>] type ms

let toMs (secs:int<sec>) =
    secs * 1000<ms/sec>

let toSecs (ms:int<ms>) =
    ms / 1000<ms/sec>

/// sleep for a certain number of milliseconds
let sleep (ms:int<ms>) =
    System.Threading.Thread.Sleep (ms * 1<_>)

/// timeout after a certain number of seconds
let commandTimeout (s:int<sec>) (cmd:System.Data.IDbCommand) =
    cmd.CommandTimeout <- (s * 1<_>)
```

哪种方法更好？

如果你正在对它们进行大量的算术运算（加法、乘法等），那么度量单位方法要方便得多，但除此之外，它们之间没有太多选择。

# 8 用类型设计：结论

*Part of the "Designing with types" series (*[link](https://fsharpforfunandprofit.com/posts/designing-with-types-conclusion/#series-toc)*)*

前后对比
2013年1月19日 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/designing-with-types-conclusion/

在本系列中，我们探讨了在设计过程中使用类型的一些方法，包括：

- 将大型结构分解为小型“原子”组件。
- 使用单案例联合为 `EmailAddress` 和 `ZipCode` 等关键域类型添加语义含义和验证。
- 确保类型系统只能表示有效数据（“使非法状态不可表示”）。
- 使用类型作为分析工具来发现隐藏的需求
- 用简单的状态机替换标志和枚举
- 用保证各种约束的类型替换原始字符串

在最后一篇文章中，让我们看看它们都一起应用。

## “之前的”代码

这是我们在本系列第一篇文章中开始的原始示例：

```F#
type Contact =
    {
    FirstName: string;
    MiddleInitial: string;
    LastName: string;

    EmailAddress: string;
    //true if ownership of email address is confirmed
    IsEmailVerified: bool;

    Address1: string;
    Address2: string;
    City: string;
    State: string;
    Zip: string;
    //true if validated against address service
    IsAddressValid: bool;
    }
```

这与应用上述所有技术后的最终结果相比如何？

## “之后的”代码

首先，让我们从非特定于应用程序的类型开始。这些类型可能可以在许多应用程序中重用。

```F#
// ========================================
// WrappedString
// ========================================

/// Common code for wrapped strings
module WrappedString =

    /// An interface that all wrapped strings support
    type IWrappedString =
        abstract Value : string

    /// Create a wrapped value option
    /// 1) canonicalize the input first
    /// 2) If the validation succeeds, return Some of the given constructor
    /// 3) If the validation fails, return None
    /// Null values are never valid.
    let create canonicalize isValid ctor (s:string) =
        if s = null
        then None
        else
            let s' = canonicalize s
            if isValid s'
            then Some (ctor s')
            else None

    /// Apply the given function to the wrapped value
    let apply f (s:IWrappedString) =
        s.Value |> f

    /// Get the wrapped value
    let value s = apply id s

    /// Equality
    let equals left right =
        (value left) = (value right)

    /// Comparison
    let compareTo left right =
        (value left).CompareTo (value right)

    /// Canonicalizes a string before construction
    /// * converts all whitespace to a space char
    /// * trims both ends
    let singleLineTrimmed s =
        System.Text.RegularExpressions.Regex.Replace(s,"\s"," ").Trim()

    /// A validation function based on length
    let lengthValidator len (s:string) =
        s.Length <= len

    /// A string of length 100
    type String100 = String100 of string with
        interface IWrappedString with
            member this.Value = let (String100 s) = this in s

    /// A constructor for strings of length 100
    let string100 = create singleLineTrimmed (lengthValidator 100) String100

    /// Converts a wrapped string to a string of length 100
    let convertTo100 s = apply string100 s

    /// A string of length 50
    type String50 = String50 of string with
        interface IWrappedString with
            member this.Value = let (String50 s) = this in s

    /// A constructor for strings of length 50
    let string50 = create singleLineTrimmed (lengthValidator 50)  String50

    /// Converts a wrapped string to a string of length 50
    let convertTo50 s = apply string50 s

    /// map helpers
    let mapAdd k v map =
        Map.add (value k) v map

    let mapContainsKey k map =
        Map.containsKey (value k) map

    let mapTryFind k map =
        Map.tryFind (value k) map

// ========================================
// Email address (not application specific)
// ========================================

module EmailAddress =

    type T = EmailAddress of string with
        interface WrappedString.IWrappedString with
            member this.Value = let (EmailAddress s) = this in s

    let create =
        let canonicalize = WrappedString.singleLineTrimmed
        let isValid s =
            (WrappedString.lengthValidator 100 s) &&
            System.Text.RegularExpressions.Regex.IsMatch(s,@"^\S+@\S+\.\S+$")
        WrappedString.create canonicalize isValid EmailAddress

    /// Converts any wrapped string to an EmailAddress
    let convert s = WrappedString.apply create s

// ========================================
// ZipCode (not application specific)
// ========================================

module ZipCode =

    type T = ZipCode of string with
        interface WrappedString.IWrappedString with
            member this.Value = let (ZipCode s) = this in s

    let create =
        let canonicalize = WrappedString.singleLineTrimmed
        let isValid s =
            System.Text.RegularExpressions.Regex.IsMatch(s,@"^\d{5}$")
        WrappedString.create canonicalize isValid ZipCode

    /// Converts any wrapped string to a ZipCode
    let convert s = WrappedString.apply create s

// ========================================
// StateCode (not application specific)
// ========================================

module StateCode =

    type T = StateCode  of string with
        interface WrappedString.IWrappedString with
            member this.Value = let (StateCode  s) = this in s

    let create =
        let canonicalize = WrappedString.singleLineTrimmed
        let stateCodes = ["AZ";"CA";"NY"] //etc
        let isValid s =
            stateCodes |> List.exists ((=) s)

        WrappedString.create canonicalize isValid StateCode

    /// Converts any wrapped string to a StateCode
    let convert s = WrappedString.apply create s

// ========================================
// PostalAddress (not application specific)
// ========================================

module PostalAddress =

    type USPostalAddress =
        {
        Address1: WrappedString.String50;
        Address2: WrappedString.String50;
        City: WrappedString.String50;
        State: StateCode.T;
        Zip: ZipCode.T;
        }

    type UKPostalAddress =
        {
        Address1: WrappedString.String50;
        Address2: WrappedString.String50;
        Town: WrappedString.String50;
        PostCode: WrappedString.String50;   // todo
        }

    type GenericPostalAddress =
        {
        Address1: WrappedString.String50;
        Address2: WrappedString.String50;
        Address3: WrappedString.String50;
        Address4: WrappedString.String50;
        Address5: WrappedString.String50;
        }

    type T =
        | USPostalAddress of USPostalAddress
        | UKPostalAddress of UKPostalAddress
        | GenericPostalAddress of GenericPostalAddress

// ========================================
// PersonalName (not application specific)
// ========================================

module PersonalName =
    open WrappedString

    type T =
        {
        FirstName: String50;
        MiddleName: String50 option;
        LastName: String100;
        }

    /// create a new value
    let create first middle last =
        match (string50 first),(string100 last) with
        | Some f, Some l ->
            Some {
                FirstName = f;
                MiddleName = (string50 middle)
                LastName = l;
                }
        | _ ->
            None

    /// concat the names together
    /// and return a raw string
    let fullNameRaw personalName =
        let f = personalName.FirstName |> value
        let l = personalName.LastName |> value
        let names =
            match personalName.MiddleName with
            | None -> [| f; l |]
            | Some middle -> [| f; (value middle); l |]
        System.String.Join(" ", names)

    /// concat the names together
    /// and return None if too long
    let fullNameOption personalName =
        personalName |> fullNameRaw |> string100

    /// concat the names together
    /// and truncate if too long
    let fullNameTruncated personalName =
        // helper function
        let left n (s:string) =
            if (s.Length > n)
            then s.Substring(0,n)
            else s

        personalName
        |> fullNameRaw  // concat
        |> left 100     // truncate
        |> string100    // wrap
        |> Option.get   // this will always be ok
```

现在是特定于应用程序的类型。

```F#
// ========================================
// EmailContactInfo -- state machine
// ========================================

module EmailContactInfo =
    open System

    // UnverifiedData = just the EmailAddress
    type UnverifiedData = EmailAddress.T

    // VerifiedData = EmailAddress plus the time it was verified
    type VerifiedData = EmailAddress.T * DateTime

    // set of states
    type T =
        | UnverifiedState of UnverifiedData
        | VerifiedState of VerifiedData

    let create email =
        // unverified on creation
        UnverifiedState email

    // handle the "verified" event
    let verified emailContactInfo dateVerified =
        match emailContactInfo with
        | UnverifiedState email ->
            // construct a new info in the verified state
            VerifiedState (email, dateVerified)
        | VerifiedState _ ->
            // ignore
            emailContactInfo

    let sendVerificationEmail emailContactInfo =
        match emailContactInfo with
        | UnverifiedState email ->
            // send email
            printfn "sending email"
        | VerifiedState _ ->
            // do nothing
            ()

    let sendPasswordReset emailContactInfo =
        match emailContactInfo with
        | UnverifiedState email ->
            // ignore
            ()
        | VerifiedState _ ->
            // ignore
            printfn "sending password reset"

// ========================================
// PostalContactInfo -- state machine
// ========================================

module PostalContactInfo =
    open System

    // InvalidData = just the PostalAddress
    type InvalidData = PostalAddress.T

    // ValidData = PostalAddress plus the time it was verified
    type ValidData = PostalAddress.T * DateTime

    // set of states
    type T =
        | InvalidState of InvalidData
        | ValidState of ValidData

    let create address =
        // invalid on creation
        InvalidState address

    // handle the "validated" event
    let validated postalContactInfo dateValidated =
        match postalContactInfo with
        | InvalidState address ->
            // construct a new info in the valid state
            ValidState (address, dateValidated)
        | ValidState _ ->
            // ignore
            postalContactInfo

    let contactValidationService postalContactInfo =
        let dateIsTooLongAgo (d:DateTime) =
            d < DateTime.Today.AddYears(-1)

        match postalContactInfo with
        | InvalidState address ->
            printfn "contacting the address validation service"
        | ValidState (address,date) when date |> dateIsTooLongAgo  ->
            printfn "last checked a long time ago."
            printfn "contacting the address validation service again"
        | ValidState  _ ->
            printfn "recently checked. Doing nothing."

// ========================================
// ContactMethod and Contact
// ========================================

type ContactMethod =
    | Email of EmailContactInfo.T
    | PostalAddress of PostalContactInfo.T

type Contact =
    {
    Name: PersonalName.T;
    PrimaryContactMethod: ContactMethod;
    SecondaryContactMethods: ContactMethod list;
    }

```

> 如果你对领域建模和设计的功能方法感兴趣，你可能会喜欢我的《领域建模函数化》一书！这是一个初学者友好的介绍，涵盖了领域驱动设计、类型建模和函数式编程。

## 结论

呼！新代码比原始代码长得多。当然，它有很多原始版本中不需要的支持功能，但即便如此，它似乎还是有很多额外的工作要做。那么值得吗？

我认为答案是肯定的。以下是一些原因：

### 新代码更加明确

如果我们看看最初的例子，字段之间没有原子性，没有验证规则，没有长度约束，没有什么可以阻止你以错误的顺序更新标志，等等。

数据结构是“愚蠢的”，所有的业务规则都隐含在应用程序代码中。应用程序可能会有很多微妙的错误，甚至可能不会在单元测试中出现。（*您确定应用程序在电子邮件地址更新的每个地方都将 `IsEmailVerified` 标记重置为 false 吗？*）

另一方面，新代码对每一个细节都非常明确。如果我去掉除类型本身之外的所有内容，您将非常清楚业务规则和域约束是什么。

### 新代码不会让你推迟错误处理

编写适用于新类型的代码意味着您必须处理所有可能出错的事情，从处理过长的名称到无法提供联系方法。而且你必须在施工时提前做这件事。你不能推迟到以后。

编写这样的错误处理代码可能会很烦人和乏味，但另一方面，它几乎是自己编写的。实际上只有一种方法可以编写真正与这些类型编译的代码。

### 新代码更有可能是正确的

新代码的巨大好处是它可能没有 bug。即使不编写任何单元测试，我也可以很有信心，当写入数据库中的 `varchar(50)` 时，名字永远不会被截断，而且我永远不会意外地发送两次验证电子邮件。

就代码本身而言，作为开发人员，你必须记住要处理（或忘记处理）的许多事情都完全不存在。没有空检查，没有强制转换，不用担心 `switch` 语句中的默认值应该是什么。如果你喜欢使用圈复杂度作为代码质量指标，你可能会注意到，在整个 350 多行中只有三个 `if` 语句。

### 一句警告…

最后，小心！适应这种基于类型的设计风格会对你产生阴险的影响。每当你看到输入不够严格的代码时，你就会开始产生偏执狂。（*确切地说，电子邮件地址应该有多长？*）你将无法编写最简单的 python 脚本而不感到焦虑。当这种情况发生时，你将完全融入邪教。欢迎光临！

*如果你喜欢这个系列，这里有一个幻灯片，涵盖了许多相同的主题。还有一个视频（这里）*

**[Domain Driven Design with the F# type System -- F#unctional Londoners 2014](https://www.slideshare.net/ScottWlaschin/domain-driven-design-with-the-f-type-system-functional-londoners-2014)** from **[my slides on Slideshare](http://www.slideshare.net/ScottWlaschin)**