# [回到主 Markdown](./FSharpForFunAndProfit翻译.md)



# 观察乌龟的十三种方法

API、依赖项注入、状态 monad 等示例！
05十二月2015 这篇文章已经超过3岁了

https://fsharpforfunandprofit.com/posts/13-ways-of-looking-at-a-turtle/

*更新：[我关于这个话题的演讲幻灯片和视频](https://fsharpforfunandprofit.com/turtle/)*

> 这篇文章是 2015 年英语 F# 降临日历项目的一部分。查看那里的所有其他精彩帖子！特别感谢 Sergey Tihon 组织这次活动。

不久前，我在讨论如何实现一个简单的乌龟图形系统，我突然想到，由于乌龟的要求非常简单且众所周知，这将为演示一系列不同的技术奠定坚实的基础。

因此，在这篇由两部分组成的巨型文章中，我将把乌龟模型扩展到极限，同时演示以下内容：部分应用、成功/失败结果的验证、“提升”的概念、带有消息队列的代理、依赖注入、状态单子、事件源、流处理，最后是自定义解释器！

闲话少说，我在此介绍 13 种实现乌龟的不同方法：

- 方式1。一种基本的面向对象方法，其中我们创建了一个具有可变状态的类。
- 方式2。一种基本的函数方法，其中我们创建了一个具有不可变状态的函数模块。
- 方式3。具有面向对象核心的 API，其中我们创建了一个面向对象的 API，该 API 调用有状态核心类。
- 方式4。一个带有函数式核心的 API，其中我们创建了一个使用无状态核心函数的有状态 API。
- 方式5。代理前面的 API，其中我们创建了一个 API，该 API 使用消息队列与代理通信。
- 方式6。使用接口的依赖注入，其中我们使用接口或函数记录将实现与 API 解耦。
- 方式7。使用函数的依赖注入，其中我们通过传递函数参数将实现与 API 解耦。
- 方式8。使用状态单子进行批处理，其中我们创建了一个特殊的“海龟工作流”计算表达式来为我们跟踪状态。
- 方式9。使用命令对象进行批处理，其中我们创建一个类型来表示 turtle 命令，然后一次处理一系列命令。
- 间奏：有意识地使用数据类型解耦。关于使用数据与接口进行解耦的几点说明。
- 方式10。事件溯源，其中状态是根据过去的事件列表构建的。
- 方式11。函数式回溯编程（流处理），其中业务逻辑基于对早期事件的反应。
- 第五集：乌龟反击战，乌龟 API 发生变化，一些命令可能会失败。
- 方式12。一元控制流，其中我们根据早期命令的结果在海龟工作流中做出决策。
- 方式13。一个 turtle 解释器，其中我们将 turtle 编程与 turtle 实现完全解耦，几乎遇到了 free monad。
- 回顾所有使用的技术。

扩展版有 2 种奖励方式：

- 方式14。抽象数据海龟，其中我们使用抽象数据类型封装海龟实现的细节。
- 方式15。基于能力的 Turtle，我们根据乌龟的当前状态控制客户可以使用哪些乌龟功能。

这篇文章的所有源代码都可以在 github 上找到。

## 海龟的要求

乌龟支持四条指令：

- 沿当前方向移动一段距离。
- 顺时针或逆时针转动一定角度。
- 把笔放下或举起。当笔放下时，移动乌龟会画一条线。
- 设置钢笔颜色（黑色、蓝色或红色之一）。

这些要求自然会导致某种“海龟接口”，如下所示：

- `Move aDistance`
- `Turn anAngle`
- `PenUp`
- `PenDown`
- `SetColor aColor`

以下所有实现都将基于此接口或其变体。

请注意，乌龟必须将这些指令转换为在画布或其他图形上下文中绘制线条。因此，实现可能需要以某种方式跟踪乌龟的位置和当前状态。

## 通用代码

在开始实现之前，让我们先处理一些常见代码。

首先，我们需要一些类型来表示距离、角度、笔状态和笔颜色。

```F#
/// An alias for a float
type Distance = float

/// Use a unit of measure to make it clear that the angle is in degrees, not radians
type [<Measure>] Degrees

/// An alias for a float of Degrees
type Angle  = float<Degrees>

/// Enumeration of available pen states
type PenState = Up | Down

/// Enumeration of available pen colors
type PenColor = Black | Red | Blue
```

我们还需要一个类型来表示乌龟的位置：

```F#
/// A structure to store the (x,y) coordinates
type Position = {x:float; y:float}
```

我们还需要一个辅助函数，根据以特定角度移动特定距离来计算新位置：

```F#
// round a float to two places to make it easier to read
let round2 (flt:float) = Math.Round(flt,2)

/// calculate a new position from the current position given an angle and a distance
let calcNewPosition (distance:Distance) (angle:Angle) currentPos =
    // Convert degrees to radians with 180.0 degrees = 1 pi radian
    let angleInRads = angle * (Math.PI/180.0) * 1.0<1/Degrees>
    // current pos
    let x0 = currentPos.x
    let y0 = currentPos.y
    // new pos
    let x1 = x0 + (distance * cos angleInRads)
    let y1 = y0 + (distance * sin angleInRads)
    // return a new Position
    {x=round2 x1; y=round2 y1}
```

让我们定义一只乌龟的初始状态：

```F#
/// Default initial state
let initialPosition,initialColor,initialPenState =
    {x=0.0; y=0.0}, Black, Down
```

还有一个假装在画布上画线的助手：

```F#
let dummyDrawLine log oldPos newPos color =
    // for now just log it
    log (sprintf "...Draw line from (%0.1f,%0.1f) to (%0.1f,%0.1f) using %A" oldPos.x oldPos.y newPos.x newPos.y color)
```

现在，我们已经为首次实现做好了准备！

## 1：基本 OO——一个具有可变状态的类

在第一个设计中，我们将使用面向对象的方法，用一个简单的类来表示乌龟。

- 状态将存储在可变的本地字段（`currentPosition`、`currentAngle` 等）中。
- 我们将注入一个日志函数 `log`，以便我们可以监视发生了什么。

这是完整的代码，应该是不言自明的：

```F#
type Turtle(log) =

    let mutable currentPosition = initialPosition
    let mutable currentAngle = 0.0<Degrees>
    let mutable currentColor = initialColor
    let mutable currentPenState = initialPenState

    member this.Move(distance) =
        log (sprintf "Move %0.1f" distance)
        // calculate new position
        let newPosition = calcNewPosition distance currentAngle currentPosition
        // draw line if needed
        if currentPenState = Down then
            dummyDrawLine log currentPosition newPosition currentColor
        // update the state
        currentPosition <- newPosition

    member this.Turn(angle) =
        log (sprintf "Turn %0.1f" angle)
        // calculate new angle
        let newAngle = (currentAngle + angle) % 360.0<Degrees>
        // update the state
        currentAngle <- newAngle

    member this.PenUp() =
        log "Pen up"
        currentPenState <- Up

    member this.PenDown() =
        log "Pen down"
        currentPenState <- Down

    member this.SetColor(color) =
        log (sprintf "SetColor %A" color)
        currentColor <- color
```

### 调用乌龟对象

客户端代码实例化乌龟并直接与它对话：

```F#
/// Function to log a message
let log message =
    printfn "%s" message

let drawTriangle() =
    let turtle = Turtle(log)
    turtle.Move 100.0
    turtle.Turn 120.0<Degrees>
    turtle.Move 100.0
    turtle.Turn 120.0<Degrees>
    turtle.Move 100.0
    turtle.Turn 120.0<Degrees>
    // back home at (0,0) with angle 0
```

`drawTriangle()` 的记录输出为：

```
Move 100.0
...Draw line from (0.0,0.0) to (100.0,0.0) using Black
Turn 120.0
Move 100.0
...Draw line from (100.0,0.0) to (50.0,86.6) using Black
Turn 120.0
Move 100.0
...Draw line from (50.0,86.6) to (0.0,0.0) using Black
Turn 120.0
```

同样，以下是绘制多边形的代码：

```F#
let drawPolygon n =
    let angle = 180.0 - (360.0/float n)
    let angleDegrees = angle * 1.0<Degrees>
    let turtle = Turtle(log)

    // define a function that draws one side
    let drawOneSide() =
        turtle.Move 100.0
        turtle.Turn angleDegrees

    // repeat for all sides
    for i in [1..n] do
        drawOneSide()
```

请注意，`drawOneSide()` 不返回任何东西——所有的代码都是命令式和有状态的。将其与下一个示例中的代码进行比较，该示例采用纯函数方法。

### 优点和缺点

那么，这种简单方法的优缺点是什么？

*优势*

- 它很容易实现和理解。

*缺点*

- 有状态代码更难测试。在测试之前，我们必须将对象置于已知状态，在这种情况下这很简单，但对于更复杂的对象来说可能会冗长且容易出错。
- 客户端耦合到特定的实现。这里没有接口！我们稍后将探讨如何使用接口。

此版本的源代码可在此处（turtle 类）和此处（客户端）获得。

## 2：Basic FP - 一个具有不可变状态的函数模块

下一个设计将使用纯粹的功能方法。定义了一个不可变的 `TurtleState`，然后各种 turtle 函数接受一个状态作为输入，并返回一个新的状态作为输出。

在这种方法中，客户端负责跟踪当前状态并将其传递给下一个函数调用。



以下是TurtleState的定义和初始状态的值：

```F#
module Turtle =

    type TurtleState = {
        position : Position
        angle : float<Degrees>
        color : PenColor
        penState : PenState
    }

    let initialTurtleState = {
        position = initialPosition
        angle = 0.0<Degrees>
        color = initialColor
        penState = initialPenState
    }
```

以下是“api”函数，所有这些函数都接受一个状态参数并返回一个新的状态：

```F#
module Turtle =

    // [state type snipped]

    let move log distance state =
        log (sprintf "Move %0.1f" distance)
        // calculate new position
        let newPosition = calcNewPosition distance state.angle state.position
        // draw line if needed
        if state.penState = Down then
            dummyDrawLine log state.position newPosition state.color
        // update the state
        {state with position = newPosition}

    let turn log angle state =
        log (sprintf "Turn %0.1f" angle)
        // calculate new angle
        let newAngle = (state.angle + angle) % 360.0<Degrees>
        // update the state
        {state with angle = newAngle}

    let penUp log state =
        log "Pen up"
        {state with penState = Up}

    let penDown log state =
        log "Pen down"
        {state with penState = Down}

    let setColor log color state =
        log (sprintf "SetColor %A" color)
        {state with color = color}
```

请注意，`state` 始终是最后一个参数——这使得使用“管道”习惯用法更容易。

### 使用乌龟函数

客户端现在每次都必须将日志函数和状态传递给每个函数！

我们可以通过使用部分应用程序创建带有内置记录器的函数的新版本来消除传递日志函数的需要：

```F#
/// Function to log a message
let log message =
    printfn "%s" message

// versions with log baked in (via partial application)
let move = Turtle.move log
let turn = Turtle.turn log
let penDown = Turtle.penDown log
let penUp = Turtle.penUp log
let setColor = Turtle.setColor log
```

使用这些更简单的版本，客户端可以以自然的方式传输状态：

```F#
let drawTriangle() =
    Turtle.initialTurtleState
    |> move 100.0
    |> turn 120.0<Degrees>
    |> move 100.0
    |> turn 120.0<Degrees>
    |> move 100.0
    |> turn 120.0<Degrees>
    // back home at (0,0) with angle 0
```

当涉及到绘制多边形时，它有点复杂，因为我们必须通过每边的重复来“折叠”状态：

```F#
let drawPolygon n =
    let angle = 180.0 - (360.0/float n)
    let angleDegrees = angle * 1.0<Degrees>

    // define a function that draws one side
    let oneSide state sideNumber =
        state
        |> move 100.0
        |> turn angleDegrees

    // repeat for all sides
    [1..n]
    |> List.fold oneSide Turtle.initialTurtleState
```

### 优点和缺点

这种纯功能方法的优缺点是什么？

*优势*

- 同样，它很容易实现和理解。
- 无状态函数更容易测试。我们总是提供当前状态作为输入，因此不需要设置来使对象进入已知状态。
- 因为没有全局状态，所以这些函数是模块化的，可以在其他环境中重用（正如我们将在本文稍后看到的）。

*缺点*

- 如前所述，客户端与特定的实现相耦合。
- 客户端必须跟踪状态（但本文稍后将介绍一些使这更容易的解决方案）。

此版本的源代码可在此处（turtle函数）和此处（客户端）获得。

## 3：具有面向对象核心的 API

让我们使用 API 对实现隐藏客户端！

在这种情况下，API 将是基于字符串的，带有文本命令，如“`move 100`”或“`turn 90`”。API 必须验证这些命令，并将它们转换为对 turtle 的方法调用（我们将再次使用有状态 `Turtle` 类的 OO 方法）。



如果命令无效，API 必须将其指示给客户端。由于我们使用的是 OO 方法，我们将通过抛出包含字符串的 `TurtleApiException` 来实现这一点，如下所示。

```F#
exception TurtleApiException of string
```

接下来，我们需要一些函数来验证命令文本：

```F#
// convert the distance parameter to a float, or throw an exception
let validateDistance distanceStr =
    try
        float distanceStr
    with
    | ex ->
        let msg = sprintf "Invalid distance '%s' [%s]" distanceStr  ex.Message
        raise (TurtleApiException msg)

// convert the angle parameter to a float<Degrees>, or throw an exception
let validateAngle angleStr =
    try
        (float angleStr) * 1.0<Degrees>
    with
    | ex ->
        let msg = sprintf "Invalid angle '%s' [%s]" angleStr ex.Message
        raise (TurtleApiException msg)

// convert the color parameter to a PenColor, or throw an exception
let validateColor colorStr =
    match colorStr with
    | "Black" -> Black
    | "Blue" -> Blue
    | "Red" -> Red
    | _ ->
        let msg = sprintf "Color '%s' is not recognized" colorStr
        raise (TurtleApiException msg)
```

有了这些，我们就可以创建 API。

解析命令文本的逻辑是将命令文本拆分为标记，然后将第一个标记与“`move`”、“`turn`”等进行匹配。

代码如下：

```F#
type TurtleApi() =

    let turtle = Turtle(log)

    member this.Exec (commandStr:string) =
        let tokens = commandStr.Split(' ') |> List.ofArray |> List.map trimString
        match tokens with
        | [ "Move"; distanceStr ] ->
            let distance = validateDistance distanceStr
            turtle.Move distance
        | [ "Turn"; angleStr ] ->
            let angle = validateAngle angleStr
            turtle.Turn angle
        | [ "Pen"; "Up" ] ->
            turtle.PenUp()
        | [ "Pen"; "Down" ] ->
            turtle.PenDown()
        | [ "SetColor"; colorStr ] ->
            let color = validateColor colorStr
            turtle.SetColor color
        | _ ->
            let msg = sprintf "Instruction '%s' is not recognized" commandStr
            raise (TurtleApiException msg)
```

### 使用 API

以下是使用 `TurtleApi` 类实现 `drawPolygon` 的方法：

```F#
let drawPolygon n =
    let angle = 180.0 - (360.0/float n)
    let api = TurtleApi()

    // define a function that draws one side
    let drawOneSide() =
        api.Exec "Move 100.0"
        api.Exec (sprintf "Turn %f" angle)

    // repeat for all sides
    for i in [1..n] do
        drawOneSide()
```

您可以看到，代码与早期的 OO 版本非常相似，使用的是直接调用 `turtle.Move 100.0`，替换为间接 API 调用 `api.Exec "Move 100.0"`。

现在，如果我们使用错误的命令如  `api.Exec "Move bad"`，如下所示：

```F#
let triggerError() =
    let api = TurtleApi()
    api.Exec "Move bad"
```

则抛出预期的异常：

```
Exception of type 'TurtleApiException' was thrown.
```

### 优点和缺点

像这样的API层有哪些优点和缺点？

- 海龟实现现在对客户端隐藏。
- 服务边界上的 API 支持验证，并可以扩展到支持监控、内部路由、负载平衡等。

*缺点*

- API 耦合到特定的实现，即使客户端不是。
- 该系统非常有状态。尽管客户端不知道 API 背后的实现，但客户端仍然通过共享状态间接耦合到内核，这反过来会使测试更加困难。

此版本的源代码可在此处获得。

## 4：具有函数式核心的 API

这种情况的另一种方法是使用混合设计，其中应用程序的核心由纯函数组成，而边界是命令式和有状态的。

Gary Bernhardt 将这种方法命名为“函数式核心/命令式外壳”。

应用到我们的 API 示例中，API 层仅使用纯乌龟函数，但 API 层通过存储可变乌龟状态来管理状态（而不是客户端）。

此外，为了更函数式，如果命令文本无效，API 将不会抛出异常，而是返回具有 `Success` 和 `Failure` 情况的 `Result` 值，其中 `Failure` 情况用于任何错误。（有关此技术的更深入讨论，请参阅我关于错误处理的函数方法的演讲）。



让我们从实现 API 类开始。这一次，它包含了一个 `mutable` 乌龟状态：

```F#
type TurtleApi() =

    let mutable state = initialTurtleState

    /// Update the mutable state value
    let updateState newState =
        state <- newState
```

验证函数不再抛出异常，而是返回 `Success` 或 `Failure`：

```F#
let validateDistance distanceStr =
    try
        Success (float distanceStr)
    with
    | ex ->
        Failure (InvalidDistance distanceStr)
```

错误案例以自己的类型记录：

```F#
type ErrorMessage =
    | InvalidDistance of string
    | InvalidAngle of string
    | InvalidColor of string
    | InvalidCommand of string
```

现在，由于验证函数现在返回 `Result<Distance>` 而不是“原始”距离，因此需要将 `move` 函数提升到 `Results` 世界，就像当前状态一样。

在处理 `Result`s 时，我们将使用三个函数：`returnR`、`mapR` 和 `lift2R`。

- `returnR` 将“正常”值转换为 Results 世界中的值：
- `mapR` 将 Results 世界中的“正常”单参数函数转换为单参数函数：
- `lift2R` 将“正常”双参数函数转换为 Results 世界中的双参数函数：


例如，使用这些辅助函数，我们可以将正常的 `move` 函数转换为 Results 世界中的函数：

- 距离参数已在 `Result` 世界中
- 使用 `returnR` 将状态参数提升到 `Result` 世界中
- 使用 `lift2R` 将 `move` 函数提升到 `Result` 世界

```F#
// lift current state to Result
let stateR = returnR state

// get the distance as a Result
let distanceR = validateDistance distanceStr

// call "move" lifted to the world of Results
lift2R move distanceR stateR
```

*（有关将功能提升到 `Result` world 的更多详细信息，请参阅关于“提升”的帖子）*

以下是 `Exec` 的完整代码：

```F#
/// Execute the command string, and return a Result
/// Exec : commandStr:string -> Result<unit,ErrorMessage>
member this.Exec (commandStr:string) =
    let tokens = commandStr.Split(' ') |> List.ofArray |> List.map trimString

    // lift current state to Result
    let stateR = returnR state

    // calculate the new state
    let newStateR =
        match tokens with
        | [ "Move"; distanceStr ] ->
            // get the distance as a Result
            let distanceR = validateDistance distanceStr

            // call "move" lifted to the world of Results
            lift2R move distanceR stateR

        | [ "Turn"; angleStr ] ->
            let angleR = validateAngle angleStr
            lift2R turn angleR stateR

        | [ "Pen"; "Up" ] ->
            returnR (penUp state)

        | [ "Pen"; "Down" ] ->
            returnR (penDown state)

        | [ "SetColor"; colorStr ] ->
            let colorR = validateColor colorStr
            lift2R setColor colorR stateR

        | _ ->
            Failure (InvalidCommand commandStr)

    // Lift `updateState` into the world of Results and
    // call it with the new state.
    mapR updateState newStateR

    // Return the final result (output of updateState)
```

### 使用 API

API 返回一个 `Result`，因此客户端不能再按顺序调用每个函数，因为我们需要处理来自调用的任何错误并放弃其余步骤。

为了让我们的生活更轻松，我们将使用 `result` 计算表达式（或工作流）来链接调用，并保留 OO 版本的命令式“感觉”。

```F#
let drawTriangle() =
    let api = TurtleApi()
    result {
        do! api.Exec "Move 100"
        do! api.Exec "Turn 120"
        do! api.Exec "Move 100"
        do! api.Exec "Turn 120"
        do! api.Exec "Move 100"
        do! api.Exec "Turn 120"
        }
```

*`result` 计算表达式的源代码可以在[这里](https://github.com/swlaschin/13-ways-of-looking-at-a-turtle/blob/master/Common.fsx#L70)找到。*

同样，对于 `drawPolygon` 代码，我们可以创建一个助手来绘制一侧，然后在 `result` 表达式中调用它 `n` 次。

```F#
let drawPolygon n =
    let angle = 180.0 - (360.0/float n)
    let api = TurtleApi()

    // define a function that draws one side
    let drawOneSide() = result {
        do! api.Exec "Move 100.0"
        do! api.Exec (sprintf "Turn %f" angle)
        }

    // repeat for all sides
    result {
        for i in [1..n] do
            do! drawOneSide()
    }
```

该代码看起来是命令式的，但实际上纯粹是函数式的，因为返回的 `Result` 值由 `result` 工作流透明地处理。

### 优点和缺点

*优势*

- 与 API 的 OO 版本相同——乌龟实现对客户端隐藏，可以进行验证等。
- 系统中唯一有状态的部分位于边界处。核心是无状态的，这使得测试更容易。

*缺点*

- API 仍然耦合到特定的实现。

此版本的源代码在这里（api 助手函数）和这里（api 和客户端）都可用。

## 5：代理面前的API

在该设计中，API 层通过消息队列与 `TurtleAgent` 通信，客户端与 API 层进行通信，与以前一样。



API（或任何地方）中没有可变项。`TurtleAgent` 通过将当前状态作为参数存储在递归消息处理循环中来管理状态。

现在，由于 `TurtleAgent` 有一个类型化的消息队列，其中所有消息都是相同的类型，我们必须将所有可能的命令组合成一个可区分的联合类型（`TurtleCommand`）。

```F#
type TurtleCommand =
    | Move of Distance
    | Turn of Angle
    | PenUp
    | PenDown
    | SetColor of PenColor
```

代理实现类似于前面的实现，但我们现在不对传入的命令进行模式匹配，以决定调用哪个函数，而不是直接公开 turtle 函数：

```F#
type TurtleAgent() =

    /// Function to log a message
    let log message =
        printfn "%s" message

    // logged versions
    let move = Turtle.move log
    let turn = Turtle.turn log
    let penDown = Turtle.penDown log
    let penUp = Turtle.penUp log
    let setColor = Turtle.setColor log

    let mailboxProc = MailboxProcessor.Start(fun inbox ->
        let rec loop turtleState = async {
            // read a command message from the queue
            let! command = inbox.Receive()
            // create a new state from handling the message
            let newState =
                match command with
                | Move distance ->
                    move distance turtleState
                | Turn angle ->
                    turn angle turtleState
                | PenUp ->
                    penUp turtleState
                | PenDown ->
                    penDown turtleState
                | SetColor color ->
                    setColor color turtleState
            return! loop newState
            }
        loop Turtle.initialTurtleState )

    // expose the queue externally
    member this.Post(command) =
        mailboxProc.Post command
```

### 向代理发送命令

API 通过构造 `TurtleCommand` 并将其发布到代理的队列来调用代理。

这一次，而不是使用之前的“提升” `move` 命令的方法：

```F#
let stateR = returnR state
let distanceR = validateDistance distanceStr
lift2R move distanceR stateR
```

我们将使用 `result` 计算表达式，因此上面的代码看起来像这样：

```F#
result {
    let! distance = validateDistance distanceStr
    move distance state
    }
```

在代理实现中，我们没有调用 `move` 命令，而是创建了 `command` 类型的 `Move` 案例，因此代码如下：

```F#
result {
    let! distance = validateDistance distanceStr
    let command = Move distance
    turtleAgent.Post command
    }
```

以下是完整的代码：

```F#
member this.Exec (commandStr:string) =
    let tokens = commandStr.Split(' ') |> List.ofArray |> List.map trimString

    // calculate the new state
    let result =
        match tokens with
        | [ "Move"; distanceStr ] -> result {
            let! distance = validateDistance distanceStr
            let command = Move distance
            turtleAgent.Post command
            }

        | [ "Turn"; angleStr ] -> result {
            let! angle = validateAngle angleStr
            let command = Turn angle
            turtleAgent.Post command
            }

        | [ "Pen"; "Up" ] -> result {
            let command = PenUp
            turtleAgent.Post command
            }

        | [ "Pen"; "Down" ] -> result {
            let command = PenDown
            turtleAgent.Post command
            }

        | [ "SetColor"; colorStr ] -> result {
            let! color = validateColor colorStr
            let command = SetColor color
            turtleAgent.Post command
            }

        | _ ->
            Failure (InvalidCommand commandStr)

    // return any errors
    result
```

### Agent 方法的优缺点

*优势*

- 一种不用锁保护可变状态的好方法。
- API 通过消息队列与特定实现解耦。`TurtleCommand` 充当一种将队列两端解耦的协议。
- 海龟代理自然是异步的。
- 代理可以很容易地水平缩放。

缺点

- 代理是有状态的，与有状态对象有同样的问题：
  - 更难对代码进行推理。
  - 测试更难。
  - 在参与者之间创建一个复杂的依赖关系网太容易了。
- 代理的稳健实现可能会变得相当复杂，因为您可能需要对监视者、心跳、背压等的支持。

此版本的源代码可在此处获得。

## 6：使用接口进行依赖注入

到目前为止，所有的实现都与乌龟函数的特定实现绑定在一起，但代理版本除外，在代理版本中，API 通过队列间接通信。

因此，让我们来看看将 API 与实现脱钩的一些方法。

### 设计面向对象风格的接口

我们将从经典的 OO 解耦实现方式开始：使用接口。

将这种方法应用于乌龟域，我们可以看到我们的 API 层将需要与 `ITurtle` 接口通信，而不是与特定的乌龟实现通信。客户端稍后通过 API 的构造函数注入乌龟实现。

以下是接口定义：

```F#
type ITurtle =
    abstract Move : Distance -> unit
    abstract Turn : Angle -> unit
    abstract PenUp : unit -> unit
    abstract PenDown : unit -> unit
    abstract SetColor : PenColor -> unit
```

请注意，这些函数中有很多 `unit`。函数签名中的 `unit` 意味着副作用，事实上，`TurtleState` 并没有在任何地方使用，因为这是一种基于 OO 的方法，可变状态被封装在对象中。

接下来，我们需要通过在 `TurtleApi` 的构造函数中注入接口来更改 API 层以使用该接口。除此之外，API 代码的其余部分保持不变，如下面的代码片段所示：

```F#
type TurtleApi(turtle: ITurtle) =

    // other code

    member this.Exec (commandStr:string) =
        let tokens = commandStr.Split(' ') |> List.ofArray |> List.map trimString
        match tokens with
        | [ "Move"; distanceStr ] ->
            let distance = validateDistance distanceStr
            turtle.Move distance
        | [ "Turn"; angleStr ] ->
            let angle = validateAngle angleStr
            turtle.Turn angle
        // etc
```

### 创建 OO 接口的一些实现

现在，让我们创建并测试一些实现。

第一个实现将被称为 `normalSize`，并且将是原始实现。第二个将被称为 `halfSize`，并将所有距离减半。

对于 `normalSize`，我们可以返回并修改原始的 `Turtle` 类以支持 `ITurtle` 接口。但我讨厌改变工作代码！相反，我们可以在原始的 `Turtle` 类周围创建一个“代理”包装器，代理在其中实现新的接口。

在某些语言中，创建代理包装器可能很冗长，但在 F# 中，您可以使用 object 表达式快速实现接口：

```F#
let normalSize() =
    let log = printfn "%s"
    let turtle = Turtle(log)

    // return an interface wrapped around the Turtle
    {new ITurtle with
        member this.Move dist = turtle.Move dist
        member this.Turn angle = turtle.Turn angle
        member this.PenUp() = turtle.PenUp()
        member this.PenDown() = turtle.PenDown()
        member this.SetColor color = turtle.SetColor color
    }
```

为了创建 `halfSize` 版本，我们做了同样的事情，但拦截了 `Move` 和将距离参数减半的调用：

```F#
let halfSize() =
    let normalSize = normalSize()

    // return a decorated interface
    {new ITurtle with
        member this.Move dist = normalSize.Move (dist/2.0)   // halved!!
        member this.Turn angle = normalSize.Turn angle
        member this.PenUp() = normalSize.PenUp()
        member this.PenDown() = normalSize.PenDown()
        member this.SetColor color = normalSize.SetColor color
    }
```

这实际上是工作中的“装饰器”模式：我们将 `normalSize` 包装在一个具有相同接口的代理中，然后更改其中一些方法的行为，同时传递其他方法，尽管它们没有受到影响。

### 注入依赖关系，OO 风格

现在让我们看看将依赖项注入 API 的客户端代码。

首先，一些绘制三角形的代码，其中传递了 `TurtleApi`：

```F#
let drawTriangle(api:TurtleApi) =
    api.Exec "Move 100"
    api.Exec "Turn 120"
    api.Exec "Move 100"
    api.Exec "Turn 120"
    api.Exec "Move 100"
    api.Exec "Turn 120"
```

现在让我们尝试通过用普通接口实例化 API 对象来绘制三角形：

```F#
let iTurtle = normalSize()   // an ITurtle type
let api = TurtleApi(iTurtle)
drawTriangle(api)
```

显然，在真实的系统中，依赖注入将在调用站点之外发生，使用 IoC 容器或类似容器。

如果我们运行它，`drawTriangle` 的输出与之前一样：

```
Move 100.0
...Draw line from (0.0,0.0) to (100.0,0.0) using Black
Turn 120.0
Move 100.0
...Draw line from (100.0,0.0) to (50.0,86.6) using Black
Turn 120.0
Move 100.0
...Draw line from (50.0,86.6) to (0.0,0.0) using Black
Turn 120.0
```

现在有了半尺寸的接口。。

```F#
let iTurtle = halfSize()
let api = TurtleApi(iTurtle)
drawTriangle(api)
```

…正如我们所希望的那样，尺寸是原来的一半！

```
Move 50.0
...Draw line from (0.0,0.0) to (50.0,0.0) using Black
Turn 120.0
Move 50.0
...Draw line from (50.0,0.0) to (25.0,43.3) using Black
Turn 120.0
Move 50.0
...Draw line from (25.0,43.3) to (0.0,0.0) using Black
Turn 120.0
```

### 设计接口，函数式风格

在纯 FP 世界中，OO 风格的接口是不存在的。但是，您可以通过使用包含函数的记录来模拟它们，接口中的每个方法对应一个函数。

因此，让我们创建一个依赖项注入的替代版本，这一次API层将使用函数记录，而不是接口。

函数记录是普通记录，但字段的类型是函数类型。以下是我们将使用的定义：

```F#
type TurtleFunctions = {
    move : Distance -> TurtleState -> TurtleState
    turn : Angle -> TurtleState -> TurtleState
    penUp : TurtleState -> TurtleState
    penDown : TurtleState -> TurtleState
    setColor : PenColor -> TurtleState -> TurtleState
    }
```

请注意，与 OO 版本不同，这些函数签名中没有 `unit`。相反，`TurtleState` 被明确地传入并返回。

还要注意，也没有日志记录。创建记录时，日志记录方法将内置到函数中。

`TurtleApi` 构造函数现在采用 `TurtleFunctions` 记录，而不是 `ITurtle`，但由于这些函数是纯函数，API 需要使用 `mutable` 字段再次管理状态。

```F#
type TurtleApi(turtleFunctions: TurtleFunctions) =

    let mutable state = initialTurtleState
```

主 `Exec` 方法的实现与我们之前看到的非常相似，但有以下区别：

- 函数从记录中提取（例如 `turtleFunctions.move`）。
- 所有活动都发生在 `result` 计算表达式中，因此可以使用验证的结果。

代码如下：

```F#
member this.Exec (commandStr:string) =
    let tokens = commandStr.Split(' ') |> List.ofArray |> List.map trimString

    // return Success of unit, or Failure
    match tokens with
    | [ "Move"; distanceStr ] -> result {
        let! distance = validateDistance distanceStr
        let newState = turtleFunctions.move distance state
        updateState newState
        }
    | [ "Turn"; angleStr ] -> result {
        let! angle = validateAngle angleStr
        let newState = turtleFunctions.turn angle state
        updateState newState
        }
    // etc
```

### 创建“函数记录”的一些实现

现在让我们创建一些实现。

同样，我们将有一个 `normalSize` 实现和一个 `halfSize` 实现。

对于 `normalSize`，我们只需要使用原始 `Turtle` 模块中的函数，并使用部分应用程序内置日志：

```F#
let normalSize() =
    let log = printfn "%s"
    // return a record of functions
    {
        move = Turtle.move log
        turn = Turtle.turn log
        penUp = Turtle.penUp log
        penDown = Turtle.penDown log
        setColor = Turtle.setColor log
    }
```

为了创建 `halfSize` 版本，我们克隆记录，只更改 `move` 函数：

```F#
let halfSize() =
    let normalSize = normalSize()
    // return a reduced turtle
    { normalSize with
        move = fun dist -> normalSize.move (dist/2.0)
    }
```

克隆记录而不是代理接口的好处是，我们不必重新实现记录中的每个功能，只需要重新实现我们关心的功能。

### 再次注入依赖项

将依赖项注入 API 的客户端代码实现得与您期望的一样。API 是一个带有构造函数的类，因此函数的记录可以以与 `ITurtle` 接口完全相同的方式传递到构造函数中：

```F#
let turtleFns = normalSize()  // a TurtleFunctions type
let api = TurtleApi(turtleFns)
drawTriangle(api)
```

如您所见，`ITurtle` 版本和 `TurtleFunctions` 版本中的客户端代码看起来完全相同！如果不是因为不同的类型，你就无法区分它们。

### 使用接口的优缺点

OO 风格的接口和 FP 风格的“函数记录”非常相似，尽管与 OO 接口不同，FP 函数是无状态的。

*优势*

- API 通过接口与特定实现解耦。
- 对于 FP “函数记录”方法（与 OO 接口相比）：
  - 函数的记录比接口更容易克隆。
  - 函数是无状态的

缺点

- 接口比单个函数更为单一，很容易包含太多不相关的方法，如果不小心，就会破坏接口隔离原则。
- 接口是不可组合的（与单个函数不同）。
- 有关此方法的更多问题，请参阅 Mark Seemann 的 Stack Overflow答案。
- 特别是对于 OO 接口方法：
  - 在重构接口时，您可能需要修改现有的类。
- 对于 FP “函数记录”方法：
  - 与 OO 接口相比，工具支持较少，互操作性较差。

这些版本的源代码可以在这里（接口）和这里（函数记录）找到。

## 7：使用函数进行依赖注入

“接口”方法的两个主要缺点是接口是不可组合的，它们打破了“只传递你需要的依赖关系”的规则，这是函数式设计的关键部分。

在真正的函数式方法中，我们会传递函数。也就是说，API 层通过作为参数传递给 API 调用的一个或多个函数进行通信。这些函数通常被部分应用，以便调用站点与“注入”解耦。

没有接口传递给构造函数，因为通常没有构造函数！（这里我只使用一个 API 类来包装可变的乌龟状态。）

在本节的方法中，我将展示两种使用函数传递注入依赖关系的替代方案：

- 在第一种方法中，每个依赖项（turtle 函数）都是单独传递的。
- 在第二种方法中，只传入一个函数。因此，为了确定使用哪个特定的 turtle 函数，定义了一个判别联合类型。

### 方法 1 - 将每个依赖关系作为单独的函数传递

管理依赖关系的最简单方法总是将所有依赖关系作为参数传递给需要它们的函数。

在我们的例子中，`Exec` 方法是唯一需要控制 turtle 的函数，因此我们可以直接将它们传递给它：

```F#
member this.Exec move turn penUp penDown setColor (commandStr:string) =
    ...
```

再次强调这一点：在这种方法中，依赖关系总是“及时”传递给需要它们的函数。构造函数中不使用依赖项，以后再使用。

以下是使用这些函数的 `Exec` 方法的更大片段：

```F#
member this.Exec move turn penUp penDown setColor (commandStr:string) =
    ...

    // return Success of unit, or Failure
    match tokens with
    | [ "Move"; distanceStr ] -> result {
        let! distance = validateDistance distanceStr
        let newState = move distance state   // use `move` function that was passed in
        updateState newState
        }
    | [ "Turn"; angleStr ] -> result {
        let! angle = validateAngle angleStr
        let newState = turn angle state   // use `turn` function that was passed in
        updateState newState
        }
    ...
```

### 在实现中使用部分应用程序进行烘焙

要创建 `Exec` 的常规或半尺寸版本，我们只需传入不同的函数：

```F#
let log = printfn "%s"
let move = Turtle.move log
let turn = Turtle.turn log
let penUp = Turtle.penUp log
let penDown = Turtle.penDown log
let setColor = Turtle.setColor log

let normalSize() =
    let api = TurtleApi()
    // partially apply the functions
    api.Exec move turn penUp penDown setColor
    // the return value is a function:
    //     string -> Result<unit,ErrorMessage>

let halfSize() =
    let moveHalf dist = move (dist/2.0)
    let api = TurtleApi()
    // partially apply the functions
    api.Exec moveHalf turn penUp penDown setColor
    // the return value is a function:
    //     string -> Result<unit,ErrorMessage>
```

在这两种情况下，我们都返回一个 `string -> Result<unit，ErrorMessage>` 类型的函数。

### 使用纯函数式的 API

所以现在，当我们想绘制一些东西时，我们只需要传入任何 `string -> Result<unit，ErrorMessage>` 类型的函数。`TurtleApi` 不再被需要或提及！

```F#
// the API type is just a function
type ApiFunction = string -> Result<unit,ErrorMessage>

let drawTriangle(api:ApiFunction) =
    result {
        do! api "Move 100"
        do! api "Turn 120"
        do! api "Move 100"
        do! api "Turn 120"
        do! api "Move 100"
        do! api "Turn 120"
        }
```

以下是 API 的使用方法：

```F#
let apiFn = normalSize()  // string -> Result<unit,ErrorMessage>
drawTriangle(apiFn)

let apiFn = halfSize()
drawTriangle(apiFn)
```

因此，尽管我们在 `TurtleApi` 中确实有可变状态，但最终的“已发布”api是一个隐藏这一事实的函数。

这种将 api 作为单个函数的方法使得模拟测试变得非常容易！

```F#
let mockApi s =
    printfn "[MockAPI] %s" s
    Success ()

drawTriangle(mockApi)
```

### 方法 2 - 传递一个处理所有命令的函数

在上面的版本中，我们传递了 5 个单独的函数！

一般来说，当你传递三到四个以上的参数时，这意味着你的设计需要调整。如果这些函数是真正独立的，你不应该真的需要那么多。

但在我们的例子中，这五个函数不是独立的——它们是一个集合——那么我们如何在不使用“函数记录”方法的情况下将它们一起传递呢？

诀窍是只传入一个函数！但是一个函数如何处理五个不同的动作呢？简单-通过使用有区别的联合来表示可能的命令。

我们之前在代理示例中已经看到过这样做，所以让我们再次回顾一下这种类型：

```F#
type TurtleCommand =
    | Move of Distance
    | Turn of Angle
    | PenUp
    | PenDown
    | SetColor of PenColor
```

我们现在需要的是一个处理这种类型的每个案例的函数。

不过，在我们这样做之前，让我们看看 `Exec` 方法实现的更改：

```F#
member this.Exec turtleFn (commandStr:string) =
    ...

    // return Success of unit, or Failure
    match tokens with
    | [ "Move"; distanceStr ] -> result {
        let! distance = validateDistance distanceStr
        let command =  Move distance      // create a Command object
        let newState = turtleFn command state
        updateState newState
        }
    | [ "Turn"; angleStr ] -> result {
        let! angle = validateAngle angleStr
        let command =  Turn angle      // create a Command object
        let newState = turtleFn command state
        updateState newState
        }
    ...
```

请注意，正在创建一个 `command` 对象，然后使用它调用 `turtleFn` 参数。

顺便说一句，这段代码与使用 `turtleAgent.Post command` 而不是 `newState = turtleFn command` 的代理实现非常相似：

### 在实现中使用部分应用程序进行烘焙

让我们使用这种方法创建两个实现：

```F#
let log = printfn "%s"
let move = Turtle.move log
let turn = Turtle.turn log
let penUp = Turtle.penUp log
let penDown = Turtle.penDown log
let setColor = Turtle.setColor log

let normalSize() =
    let turtleFn = function
        | Move dist -> move dist
        | Turn angle -> turn angle
        | PenUp -> penUp
        | PenDown -> penDown
        | SetColor color -> setColor color

    // partially apply the function to the API
    let api = TurtleApi()
    api.Exec turtleFn
    // the return value is a function:
    //     string -> Result<unit,ErrorMessage>

let halfSize() =
    let turtleFn = function
        | Move dist -> move (dist/2.0)
        | Turn angle -> turn angle
        | PenUp -> penUp
        | PenDown -> penDown
        | SetColor color -> setColor color

    // partially apply the function to the API
    let api = TurtleApi()
    api.Exec turtleFn
    // the return value is a function:
    //     string -> Result<unit,ErrorMessage>
```

如前所述，在这两种情况下，我们都返回一个 `string -> Result<unit, ErrorMessage>` 类型的函数，。我们可以将其传递给前面定义的 `drawTriangle` 函数：

```F#
let api = normalSize()
drawTriangle(api)

let api = halfSize()
drawTriangle(api)
```

### 使用函数的优缺点

*优势*

- API 通过参数化与特定实现解耦。
- 因为依赖关系是在使用点（“当着你的面”）传递的，而不是在构造函数（“看不见”）中传递的，所以依赖关系倍增的趋势大大降低了。
- 任何功能参数都是自动的“一种方法接口”，因此不需要改装。
- 常规的部分应用程序可用于烘焙“依赖注入”的参数。不需要特殊的模式或 IoC 容器。

*缺点*

- 如果依赖函数的数量太大（比如超过四个），将它们全部作为单独的参数传递可能会变得很尴尬（因此，第二种方法）。
- 区分的联合类型可能比接口更难处理。

这些版本的源代码可以在这里（五个函数参数）和这里（一个函数参数”）找到。

## 8：使用状态单子进行批处理

在接下来的两节中，我们将从“交互”模式切换到“批处理”模式，在“交互式”模式下，指令一次处理一个，在“批处理模式下，一系列指令被分组在一起，然后作为一个单元运行。

在第一个设计中，我们将回到客户端直接使用 Turtle 函数的模型。

和以前一样，客户端必须跟踪当前状态并将其传递给下一个函数调用，但这次我们将通过使用所谓的“状态单子”将状态线程化到各种指令中，从而使状态不可见。因此，任何地方都没有可变性！

这不是一个通用的状态单子，而是一个仅用于本演示的简化单子。我称之为 `turtle` 工作流程。

（有关状态 monad 的更多信息，请参阅我关于解析器组合子的“monaster”演讲和帖子）

### 定义 `turtle` 工作流程

我们在一开始定义的核心乌龟函数遵循与许多其他状态转换函数相同的“形状”，即输入加乌龟状态，输出加乌龟状态。



*（确实，到目前为止，我们还没有海龟函数的任何可用输出，但在后面的示例中，我们将看到这个输出被用于做出决策。）*

有一种标准的方法来处理这类函数——“状态单子”。

让我们看看它是如何构建的。

首先，请注意，由于 currying，我们可以将这种形状的函数重新定义为两个单独的单参数函数：处理输入会生成另一个函数，该函数反过来将状态作为参数：



然后，我们可以将海龟函数视为接受输入并返回新函数的东西，如下所示：



在我们的例子中，使用 `TurtleState` 作为状态，返回的函数如下：

```F#
TurtleState -> 'a * TurtleState
```

最后，为了更容易使用，我们可以将返回的函数视为一个独立的东西，给它起一个名字，比如 `TurtleStateComputation`：



在实现中，我们通常会用一个区分大小写的联合来包装函数，如下所示：

```F#
type TurtleStateComputation<'a> =
    TurtleStateComputation of (Turtle.TurtleState -> 'a * Turtle.TurtleState)
```

这就是“状态单子”背后的基本思想。然而，重要的是要意识到，状态单子不仅仅由这种类型组成——你还需要一些遵守一些合理规律的函数（“return”和“bind”）。

我不会在这里定义 `returnT` 和 `bindT` 函数，但您可以在完整的源代码中看到它们的定义。

我们还需要一些额外的辅助函数。（我将为所有函数添加一个 `T` 作为 Turtle 后缀）。

特别是，我们需要一种方法将一些状态输入到 `TurtleStateComputation` 中以“运行”它：

```F#
let runT turtle state =
    // pattern match against the turtle
    // to extract the inner function
    let (TurtleStateComputation innerFn) = turtle
    // run the inner function with the passed in state
    innerFn state
```

最后，我们可以创建一个 `turtle` 工作流，这是一个计算表达式，可以更容易地使用 `TurtleStateComputation` 类型：

```F#
// define a computation expression builder
type TurtleBuilder() =
    member this.Return(x) = returnT x
    member this.Bind(x,f) = bindT f x

// create an instance of the computation expression builder
let turtle = TurtleBuilder()
```

### 使用 Turtle 工作流

要使用 `turtle` 工作流，我们首先需要创建 turtle 函数的“lified”或“monadic”版本：

```F#
let move dist =
    toUnitComputation (Turtle.move log dist)
// val move : Distance -> TurtleStateComputation<unit>

let turn angle =
    toUnitComputation (Turtle.turn log angle)
// val turn : Angle -> TurtleStateComputation<unit>

let penDown =
    toUnitComputation (Turtle.penDown log)
// val penDown : TurtleStateComputation<unit>

let penUp =
    toUnitComputation (Turtle.penUp log)
// val penUp : TurtleStateComputation<unit>

let setColor color =
    toUnitComputation (Turtle.setColor log color)
// val setColor : PenColor -> TurtleStateComputation<unit>
```

`toUnitComputation` 辅助函数执行提升操作。不用担心它是如何工作的，但效果是 `move` 函数的原始版本（`Distance -> TurtleState -> TurtleState`）重生为返回 `TurtleStateComputation` 的函数（`Distance -> TurtleStateCalculation<unit>`）

一旦我们有了这些“一元(monadic)”版本，我们就可以在 `turtle` 工作流中使用它们，如下所示：

```F#
let drawTriangle() =
    // define a set of instructions
    let t = turtle {
        do! move 100.0
        do! turn 120.0<Degrees>
        do! move 100.0
        do! turn 120.0<Degrees>
        do! move 100.0
        do! turn 120.0<Degrees>
        }

    // finally, run them using the initial state as input
    runT t initialTurtleState
```

`drawTriangle` 链的第一部分有六条指令，但重要的是，*不*运行它们。只有在最后使用 `runT` 函数时，指令才会实际执行。

`drawPolygon` 示例稍微复杂一些。首先，我们定义一个绘制一侧的工作流程：

```F#
let oneSide = turtle {
    do! move 100.0
    do! turn angleDegrees
    }
```

但是，我们需要一种将所有方面结合到一个工作流程中的方法。有几种方法可以做到这一点。我将创建一个成对组合器 `chain`，然后使用 `reduce` 将所有边组合成一个操作。

```F#
// chain two turtle operations in sequence
let chain f g  = turtle {
    do! f
    do! g
    }

// create a list of operations, one for each side
let sides = List.replicate n oneSide

// chain all the sides into one operation
let all = sides |> List.reduce chain
```

以下是 `drawPolygon` 的完整代码：

```F#
let drawPolygon n =
    let angle = 180.0 - (360.0/float n)
    let angleDegrees = angle * 1.0<Degrees>

    // define a function that draws one side
    let oneSide = turtle {
        do! move 100.0
        do! turn angleDegrees
        }

    // chain two turtle operations in sequence
    let chain f g  = turtle {
        do! f
        do! g
        }

    // create a list of operations, one for each side
    let sides = List.replicate n oneSide

    // chain all the sides into one operation
    let all = sides |> List.reduce chain

    // finally, run them using the initial state
    runT all initialTurtleState
```

### `turtle` 工作流程的优点和缺点

*优势*

- 客户端代码类似于命令式代码，但保留了不变性。
- 工作流是可组合的——您可以定义两个工作流，然后组合它们以创建另一个工作流。

*缺点*

- 耦合乌龟功能的特定实现。
- 比明确跟踪状态更复杂。
- 一堆嵌套的单子/工作流很难使用。

作为最后一点的一个例子，假设我们有一个包含 `result` 工作流的 `seq`，其中包含一个 `turtle` 工作流，我们想反转它们，使 `turtle` 工作流位于外部。你会怎么做？这并不明显！

此版本的源代码可在此处获得。

## 9：使用命令对象进行批处理

另一种面向批处理的方法是以新的方式重用 `TurtleCommand` 类型。客户端创建了一个将作为一个组运行的命令列表，而不是立即调用函数。

当你“运行”命令列表时，你可以使用标准的 Turtle 库函数依次执行每个命令，使用 `fold` 将状态贯穿序列。



由于所有命令都是同时运行的，这种方法意味着客户端在调用之间不需要持久化任何状态。

以下是 `TurtleCommand` 的定义：

```F#
type TurtleCommand =
    | Move of Distance
    | Turn of Angle
    | PenUp
    | PenDown
    | SetColor of PenColor
```

为了处理一系列命令，我们需要折叠它们，将状态线程化，因此我们需要一个函数，将单个命令应用于一个状态并返回一个新的状态：

```F#
/// Apply a command to the turtle state and return the new state
let applyCommand state command =
    match command with
    | Move distance ->
        move distance state
    | Turn angle ->
        turn angle state
    | PenUp ->
        penUp state
    | PenDown ->
        penDown state
    | SetColor color ->
        setColor color state
```

然后，要运行所有命令，我们只需使用 `fold`：

```F#
/// Run list of commands in one go
let run aListOfCommands =
    aListOfCommands
    |> List.fold applyCommand Turtle.initialTurtleState
```

### 运行一批命令

例如，要绘制一个三角形，我们只需创建一个命令列表，然后运行它们：

```F#
let drawTriangle() =
    // create the list of commands
    let commands = [
        Move 100.0
        Turn 120.0<Degrees>
        Move 100.0
        Turn 120.0<Degrees>
        Move 100.0
        Turn 120.0<Degrees>
        ]
    // run them
    run commands
```

现在，由于命令只是一个集合，我们可以很容易地从较小的集合构建更大的集合。

这是一个 `drawPolygon` 的示例，其中 `drawOneSide` 返回一组命令，并且该集合在每一侧都是重复的：

```F#
let drawPolygon n =
    let angle = 180.0 - (360.0/float n)
    let angleDegrees = angle * 1.0<Degrees>

    // define a function that draws one side
    let drawOneSide sideNumber = [
        Move 100.0
        Turn angleDegrees
        ]

    // repeat for all sides
    let commands =
        [1..n] |> List.collect drawOneSide

    // run the commands
    run commands
```

### 批处理命令的优缺点

*优势*

- 比工作流或单子更容易构建和使用。
- 只有一个函数耦合到特定的实现。客户端的其余部分是解耦的。

缺点

- 仅面向批处理。
- 仅适用于控制流不基于先前命令的响应的情况。如果您确实需要对每个命令的结果做出响应，请考虑使用稍后讨论的“解释器”方法。

此版本的源代码可在此处获得。

## 间奏：有意识地使用数据类型解耦

在迄今为止的三个示例（代理、函数依赖项注入和批处理）中，我们使用了 `Command` 类型——一个有区别联合，每个 API 调用都包含一个事例。在下一篇文章中，我们还将看到类似的东西用于事件溯源和解释器方法。

这不是意外。面向对象设计和函数式设计之间的区别之一是，面向对象设计侧重于行为，而函数式设计侧重于数据转换。

因此，他们解耦的方法也不同。OO 设计更喜欢通过共享封装行为包（“接口”）来提供解耦，而函数式设计则更喜欢通过商定一种通用数据类型（有时称为“协议（protocol）”）来实现解耦（尽管我更喜欢用这个词来描述消息交换模式）。

一旦就通用数据类型达成一致，任何发出该类型的函数都可以使用常规函数组合连接到使用该类型的任何函数。

您还可以将这两种方法视为类似于 web 服务中 RPC 或面向消息的 API 之间的选择，正如基于消息的设计比 RPC 有许多优势一样，基于数据的解耦也比基于行为的解耦有相似的优势。

使用数据解耦的一些优点包括：

- 使用共享数据类型意味着组合是微不足道的。编写基于行为的接口更难。
- 可以说，每个函数都已经“解耦”了，因此在重构时不需要改造现有的函数。最坏的情况下，你可能需要将一种数据类型转换为另一种，但使用…moar函数和 moar 函数组合可以很容易地完成！
- 如果需要将代码拆分为物理上独立的服务，数据结构很容易序列化为远程服务。
- 数据结构易于安全地演化。例如，如果我添加了第六个 turtle 动作，或删除了一个动作，或更改了动作的参数，则区分的联合类型将发生变化，共享类型的所有客户端将无法编译，直到第六个 turtle 动作被考虑在内，等等。另一方面，如果你不希望现有代码中断，你可以使用版本友好的数据序列化格式，如 protobuf。当使用接口时，这两个选项都不那么容易。

## 摘要

> 模因正在传播。乌龟一定在划水 - 《看乌龟的十三种方式》，华莱士·D·科瑞萨

你好？还有人吗？谢谢你走这么远！

所以，是时候休息了！在下一篇文章中，我们将介绍观察乌龟的其余四种方法。

这篇文章的源代码可以在 github 上找到。



# 观察乌龟的十三种方式（part 2）

继续介绍事件源、FRP、一元控制流和解释器的示例。
2015年12月6日 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/13-ways-of-looking-at-a-turtle-2/#series-toc

更新：[我关于这个话题的演讲幻灯片和视频](https://fsharpforfunandprofit.com/turtle/)

这篇文章是 2015 年英语 F# 降临日历项目的一部分。查看那里的所有其他精彩帖子！特别感谢 Sergey Tihon 组织这次活动。

在这篇由两部分组成的大型帖子中，我将把简单的乌龟图形模型扩展到极限，同时演示了部分应用、验证、“提升”的概念、带有消息队列的代理、依赖注入、状态单子、事件溯源、流处理和解释器！

在上一篇文章中，我们介绍了观察乌龟的前九种方法。在这篇文章中，我们将看看剩下的四个。

作为提醒，这里有十三种方法：

- 方式1。一种基本的面向对象方法，其中我们创建了一个具有可变状态的类。
- 方式2。一种基本的函数方法，其中我们创建了一个具有不可变状态的函数模块。
- 方式3。具有面向对象核心的 API，其中我们创建了一个面向对象的 API，该 API 调用有状态核心类。
- 方式4。一个带有函数式核心的 API，其中我们创建了一个使用无状态核心函数的有状态 API。
- 方式5。代理前面的 API，其中我们创建了一个 API，该 API 使用消息队列与代理通信。
- 方式6。使用接口的依赖注入，其中我们使用接口或函数记录将实现与 API 解耦。
- 方式7。使用函数的依赖注入，其中我们通过传递函数参数将实现与 API 解耦。
- 方式8。使用状态单子进行批处理，其中我们创建了一个特殊的“海龟工作流”计算表达式来为我们跟踪状态。
- 方式9。使用命令对象进行批处理，其中我们创建一个类型来表示 turtle 命令，然后一次处理一系列命令。
- 间奏：有意识地使用数据类型解耦。关于使用数据与接口进行解耦的几点说明。
- 方式10。事件溯源，其中状态是根据过去的事件列表构建的。
- 方式11。函数式回溯编程（流处理），其中业务逻辑基于对早期事件的反应。
- 第五集：乌龟反击战，乌龟 API 发生变化，一些命令可能会失败。
- 方式12。一元控制流，其中我们根据早期命令的结果在海龟工作流中做出决策。
- 方式13。一个 turtle 解释器，其中我们将 turtle 编程与 turtle 实现完全解耦，几乎遇到了 free monad。
- 回顾所有使用的技术。

扩展版有 2 种奖励方式：

- 方式14。抽象数据海龟，其中我们使用抽象数据类型封装海龟实现的细节。
- 方式15。基于能力的 Turtle，我们根据乌龟的当前状态控制客户可以使用哪些乌龟功能。

一路下来都是乌龟！

这篇文章的所有源代码都可以在 github 上找到。

## 10：事件溯源——根据过去的事件列表建立状态

在这个设计中，我们基于 Agent（方式 5）和 Batch（方式 9）方法中使用的“命令”概念，但将“命令”替换为“事件”作为更新状态的方法。

它的工作方式是：

- 客户端向 `CommandHandler` 发送 `Command`。
- 在处理 `Command` 之前，`CommandHandler` 首先使用与特定乌龟相关的过去事件从头开始重建当前状态。
- 然后，`CommandHandler` 验证命令，并根据当前（重建）状态决定要做什么。它生成一个（可能为空）事件列表。
- 生成的事件存储在 `EventStore` 中，供下一个命令使用。


这样，客户端和命令处理程序都不需要跟踪状态。只有 `EventStore` 是可变的。

### 命令和事件类型

我们将首先定义与我们的事件源系统相关的类型。首先，与命令相关的类型：

```F#
type TurtleId = System.Guid

/// A desired action on a turtle
type TurtleCommandAction =
    | Move of Distance
    | Turn of Angle
    | PenUp
    | PenDown
    | SetColor of PenColor

/// A command representing a desired action addressed to a specific turtle
type TurtleCommand = {
    turtleId : TurtleId
    action : TurtleCommandAction
    }
```

请注意，该命令是使用 `TurtleId` 发送给特定海龟的。

接下来，我们将定义可以从命令生成的两种事件：

- `StateChangedEvent`，表示状态中发生了什么变化
- 一个 `MovedEvent`，表示乌龟运动的开始和结束位置。

```F#
/// An event representing a state change that happened
type StateChangedEvent =
    | Moved of Distance
    | Turned of Angle
    | PenWentUp
    | PenWentDown
    | ColorChanged of PenColor

/// An event representing a move that happened
/// This can be easily translated into a line-drawing activity on a canvas
type MovedEvent = {
    startPos : Position
    endPos : Position
    penColor : PenColor option
    }

/// A union of all possible events
type TurtleEvent =
    | StateChangedEvent of StateChangedEvent
    | MovedEvent of MovedEvent
```

事件来源的一个重要部分是，所有事件都以过去时标记：`Moved` 和 `Turned`，而不是 `Move` 和 `Turn`。这件事是事实——它们发生在过去。

### 命令处理程序

下一步是定义将命令转换为事件的函数。

我们需要：

- 一个（私有）`applyEvent` 函数，用于更新前一个事件的状态。
- 一个（私有）`eventsFromCommand` 函数，根据命令和状态确定要生成哪些事件。
- 一个公共的 `commandHandler` 函数，用于处理命令、从事件存储中读取事件并调用其他两个函数。

这是 `applyEvent`。您可以看到，它与我们在前面的批处理示例中看到的 `applyCommand` 函数非常相似。

```F#
/// Apply an event to the current state and return the new state of the turtle
let applyEvent log oldState event =
    match event with
    | Moved distance ->
        Turtle.move log distance oldState
    | Turned angle ->
        Turtle.turn log angle oldState
    | PenWentUp ->
        Turtle.penUp log oldState
    | PenWentDown ->
        Turtle.penDown log oldState
    | ColorChanged color ->
        Turtle.setColor log color oldState
```

`eventsFromCommand` 函数包含用于验证命令和创建事件的关键逻辑。

- 在这种特殊的设计中，命令始终有效，因此至少返回一个事件。
- `StateChangedEvent` 是由 `TurtleCommand` 在案例的直接一对一映射中创建的。
- 只有当海龟改变了位置时，才会从 `TurtleCommand` 中创建 `MovedEvent`。

```F#
// Determine what events to generate, based on the command and the state.
let eventsFromCommand log command stateBeforeCommand =

    // --------------------------
    // create the StateChangedEvent from the TurtleCommand
    let stateChangedEvent =
        match command.action with
        | Move dist -> Moved dist
        | Turn angle -> Turned angle
        | PenUp -> PenWentUp
        | PenDown -> PenWentDown
        | SetColor color -> ColorChanged color

    // --------------------------
    // calculate the current state from the new event
    let stateAfterCommand =
        applyEvent log stateBeforeCommand stateChangedEvent

    // --------------------------
    // create the MovedEvent
    let startPos = stateBeforeCommand.position
    let endPos = stateAfterCommand.position
    let penColor =
        if stateBeforeCommand.penState=Down then
            Some stateBeforeCommand.color
        else
            None

    let movedEvent = {
        startPos = startPos
        endPos = endPos
        penColor = penColor
        }

    // --------------------------
    // return the list of events
    if startPos <> endPos then
        // if the turtle has moved, return both the stateChangedEvent and the movedEvent
        // lifted into the common TurtleEvent type
        [ StateChangedEvent stateChangedEvent; MovedEvent movedEvent]
    else
        // if the turtle has not moved, return just the stateChangedEvent
        [ StateChangedEvent stateChangedEvent]
```

最后，`commandHandler` 是公共接口。它在某些依赖关系中作为参数传递：一个日志记录函数，一个从事件存储中检索历史事件的函数，以及一个将新生成的事件保存到事件存储中的函数。

```F#
/// The type representing a function that gets the StateChangedEvents for a turtle id
/// The oldest events are first
type GetStateChangedEventsForId =
     TurtleId -> StateChangedEvent list

/// The type representing a function that saves a TurtleEvent
type SaveTurtleEvent =
    TurtleId -> TurtleEvent -> unit

/// main function : process a command
let commandHandler
    (log:string -> unit)
    (getEvents:GetStateChangedEventsForId)
    (saveEvent:SaveTurtleEvent)
    (command:TurtleCommand) =

    /// First load all the events from the event store
    let eventHistory =
        getEvents command.turtleId

    /// Then, recreate the state before the command
    let stateBeforeCommand =
        let nolog = ignore // no logging when recreating state
        eventHistory
        |> List.fold (applyEvent nolog) Turtle.initialTurtleState

    /// Construct the events from the command and the stateBeforeCommand
    /// Do use the supplied logger for this bit
    let events = eventsFromCommand log command stateBeforeCommand

    // store the events in the event store
    events |> List.iter (saveEvent command.turtleId)
```

### 调用命令处理程序

现在，我们已经准备好将事件发送到命令处理程序。

首先，我们需要一些创建命令的辅助函数：

```F#
// Command versions of standard actions
let turtleId = System.Guid.NewGuid()
let move dist = {turtleId=turtleId; action=Move dist}
let turn angle = {turtleId=turtleId; action=Turn angle}
let penDown = {turtleId=turtleId; action=PenDown}
let penUp = {turtleId=turtleId; action=PenUp}
let setColor color = {turtleId=turtleId; action=SetColor color}
```

然后我们可以通过向命令处理程序发送各种命令来绘制一个图形：

```F#
let drawTriangle() =
    let handler = makeCommandHandler()
    handler (move 100.0)
    handler (turn 120.0<Degrees>)
    handler (move 100.0)
    handler (turn 120.0<Degrees>)
    handler (move 100.0)
    handler (turn 120.0<Degrees>)
```

注意：我没有展示如何创建命令处理程序或事件存储，有关完整详细信息，请参阅代码。

### 事件溯源的优缺点

*优势*

- 所有代码都是无状态的，因此易于测试。
- 支持事件回放。

*缺点*

- 实现起来可能比 CRUD 方法更复杂（或者至少工具和库的支持更少）。
- 如果不小心，命令处理程序可能会变得过于复杂，并演变为实现过多的业务逻辑。

此版本的源代码可在此处获得。

## 11：函数式回溯编程（流处理）

在上面的事件源示例中，所有域逻辑（在我们的例子中，只是跟踪状态）都嵌入在命令处理程序中。这样做的一个缺点是，随着应用程序的发展，命令处理程序中的逻辑可能会变得非常复杂。

避免这种情况的一种方法是将“函数式反应式编程”与事件源相结合，通过监听事件存储发出的事件（“信号”），创建一种在“读取侧”执行域逻辑的设计。

在这种方法中，“写端”遵循与事件源示例相同的模式。客户端向 `commandHandler` 发送 `Command`，`commandHandler` 将其转换为事件列表并将其存储在 `EventStore` 中。

然而，`commandHandler` 只做最少量的工作，比如更新状态，不做任何复杂的域逻辑。复杂的逻辑由订阅事件流的一个或多个下游“处理器”（有时也称为“聚合器”）执行。



您甚至可以将这些事件视为对处理器的“命令”，当然，处理器可以生成新的事件供另一个处理器使用，因此这种方法可以扩展到一种架构风格，其中应用程序由一组由事件存储链接的命令处理程序组成。

这种技术通常被称为“流处理”。然而，Jessica Kerr 曾将这种方法称为“函数式回溯编程（Functional Retroactive Programming）”——我喜欢这样，所以我要盗用这个名字！



### 实现设计

对于此实现，`commandHandler` 函数与事件源示例中的函数相同，只是根本不做任何工作（只是记录！）。命令处理程序仅重建状态并生成事件。如何将事件用于业务逻辑不再在其范围内。

新的东西出现在制造处理器上。

然而，在我们创建处理器之前，我们需要一些辅助函数来过滤事件存储提要，使其仅包含特定于乌龟的事件，其中只有 `StateChangedEvent`s 或 `MovedEvent`s。

```F#
// filter to choose only TurtleEvents
let turtleFilter ev =
    match box ev with
    | :? TurtleEvent as tev -> Some tev
    | _ -> None

// filter to choose only MovedEvents from TurtleEvents
let moveFilter = function
    | MovedEvent ev -> Some ev
    | _ -> None

// filter to choose only StateChangedEvent from TurtleEvents
let stateChangedEventFilter = function
    | StateChangedEvent ev -> Some ev
    | _ -> None
```

现在，让我们创建一个处理器，用于监听移动事件，并在虚拟乌龟移动时移动物理乌龟。

我们将使处理器的输入成为 `IObservable`（事件流），这样它就不会耦合到任何特定的源，如 `EventStore`。配置应用程序时，我们将把 `EventStore` “保存”事件连接到此处理器。

```F#
/// Physically move the turtle
let physicalTurtleProcessor (eventStream:IObservable<Guid*obj>) =

    // the function that handles the input from the observable
    let subscriberFn (ev:MovedEvent) =
        let colorText =
            match ev.penColor with
            | Some color -> sprintf "line of color %A" color
            | None -> "no line"
        printfn "[turtle  ]: Moved from (%0.2f,%0.2f) to (%0.2f,%0.2f) with %s"
            ev.startPos.x ev.startPos.y ev.endPos.x ev.endPos.y colorText

    // start with all events
    eventStream
    // filter the stream on just TurtleEvents
    |> Observable.choose (function (id,ev) -> turtleFilter ev)
    // filter on just MovedEvents
    |> Observable.choose moveFilter
    // handle these
    |> Observable.subscribe subscriberFn
```

在这种情况下，我们只是在打印运动——我将把一只真正的乐高头脑风暴乌龟的建造留给读者作为练习！

让我们还创建一个在图形显示器上绘制线条的处理器：

```F#
/// Draw lines on a graphics device
let graphicsProcessor (eventStream:IObservable<Guid*obj>) =

    // the function that handles the input from the observable
    let subscriberFn (ev:MovedEvent) =
        match ev.penColor with
        | Some color ->
            printfn "[graphics]: Draw line from (%0.2f,%0.2f) to (%0.2f,%0.2f) with color %A"
                ev.startPos.x ev.startPos.y ev.endPos.x ev.endPos.y color
        | None ->
            ()  // do nothing

    // start with all events
    eventStream
    // filter the stream on just TurtleEvents
    |> Observable.choose (function (id,ev) -> turtleFilter ev)
    // filter on just MovedEvents
    |> Observable.choose moveFilter
    // handle these
    |> Observable.subscribe subscriberFn
```

最后，让我们创建一个处理器来累积移动的总距离，这样我们就可以跟踪使用了多少墨水。

```F#
/// Listen for "moved" events and aggregate them to keep
/// track of the total ink used
let inkUsedProcessor (eventStream:IObservable<Guid*obj>) =

    // Accumulate the total distance moved so far when a new event happens
    let accumulate distanceSoFar (ev:StateChangedEvent) =
        match ev with
        | Moved dist ->
            distanceSoFar + dist
        | _ ->
            distanceSoFar

    // the function that handles the input from the observable
    let subscriberFn distanceSoFar  =
        printfn "[ink used]: %0.2f" distanceSoFar

    // start with all events
    eventStream
    // filter the stream on just TurtleEvents
    |> Observable.choose (function (id,ev) -> turtleFilter ev)
    // filter on just StateChangedEvent
    |> Observable.choose stateChangedEventFilter
    // accumulate total distance
    |> Observable.scan accumulate 0.0
    // handle these
    |> Observable.subscribe subscriberFn
```

该处理器使用 `Observable.scan` 将事件累积为一个值——总行驶距离。

### 实践中的处理器

让我们试试这些！

例如，这里是 `drawTriangle`：

```F#
let drawTriangle() =
    // clear older events
    eventStore.Clear turtleId

    // create an event stream from an IEvent
    let eventStream = eventStore.SaveEvent :> IObservable<Guid*obj>

    // register the processors
    use physicalTurtleProcessor = EventProcessors.physicalTurtleProcessor eventStream
    use graphicsProcessor = EventProcessors.graphicsProcessor eventStream
    use inkUsedProcessor = EventProcessors.inkUsedProcessor eventStream

    let handler = makeCommandHandler
    handler (move 100.0)
    handler (turn 120.0<Degrees>)
    handler (move 100.0)
    handler (turn 120.0<Degrees>)
    handler (move 100.0)
    handler (turn 120.0<Degrees>)
```

请注意 `eventStore.SaveEvent` 在作为参数传递给处理器之前，会被转换为 `IObservable<Guid*obj>`（即事件流）。

`drawTriangle` 生成以下输出：

```
[ink used]: 100.00
[turtle  ]: Moved from (0.00,0.00) to (100.00,0.00) with line of color Black
[graphics]: Draw line from (0.00,0.00) to (100.00,0.00) with color Black
[ink used]: 100.00
[ink used]: 200.00
[turtle  ]: Moved from (100.00,0.00) to (50.00,86.60) with line of color Black
[graphics]: Draw line from (100.00,0.00) to (50.00,86.60) with color Black
[ink used]: 200.00
[ink used]: 300.00
[turtle  ]: Moved from (50.00,86.60) to (0.00,0.00) with line of color Black
[graphics]: Draw line from (50.00,86.60) to (0.00,0.00) with color Black
[ink used]: 300.00
```

您可以看到所有处理器都在成功处理事件。

乌龟在移动，图形处理器在画线，使用墨水的处理器正确计算出移动的总距离为 300 个单位。

不过，请注意，使用墨水的处理器在每次状态变化（如转动）时都会发出输出，而不仅仅是在实际移动时。

我们可以通过在流中放入一对 `(previousDistance, currentDistance)`，然后过滤掉值相同的事件来解决这个问题。

这是新的 `inkUsedProcessor` 代码，有以下更改：

- `accumulate` 函数现在发出一对。
- 新过滤器 `changedDistanceOnly`。

```F#
/// Listen for "moved" events and aggregate them to keep
/// track of the total distance moved
/// NEW! No duplicate events!
let inkUsedProcessor (eventStream:IObservable<Guid*obj>) =

    // Accumulate the total distance moved so far when a new event happens
    let accumulate (prevDist,currDist) (ev:StateChangedEvent) =
        let newDist =
            match ev with
            | Moved dist ->
                currDist + dist
            | _ ->
                currDist
        (currDist, newDist)

    // convert unchanged events to None so they can be filtered out with "choose"
    let changedDistanceOnly (currDist, newDist) =
        if currDist <> newDist then
            Some newDist
        else
            None

    // the function that handles the input from the observable
    let subscriberFn distanceSoFar  =
        printfn "[ink used]: %0.2f" distanceSoFar

    // start with all events
    eventStream
    // filter the stream on just TurtleEvents
    |> Observable.choose (function (id,ev) -> turtleFilter ev)
    // filter on just StateChangedEvent
    |> Observable.choose stateChangedEventFilter
    // NEW! accumulate total distance as pairs
    |> Observable.scan accumulate (0.0,0.0)
    // NEW! filter out when distance has not changed
    |> Observable.choose changedDistanceOnly
    // handle these
    |> Observable.subscribe subscriberFn
```

经过这些更改，`drawTriangle` 的输出如下：

```
[ink used]: 100.00
[turtle  ]: Moved from (0.00,0.00) to (100.00,0.00) with line of color Black
[graphics]: Draw line from (0.00,0.00) to (100.00,0.00) with color Black
[ink used]: 200.00
[turtle  ]: Moved from (100.00,0.00) to (50.00,86.60) with line of color Black
[graphics]: Draw line from (100.00,0.00) to (50.00,86.60) with color Black
[ink used]: 300.00
[turtle  ]: Moved from (50.00,86.60) to (0.00,0.00) with line of color Black
[graphics]: Draw line from (50.00,86.60) to (0.00,0.00) with color Black
```

并且不再有来自 `inkUsedProcessor` 的任何重复消息。

### 流处理的优缺点

*优势*

- 与事件溯源具有相同的优势。
- 将有状态逻辑与其他非内在逻辑解耦。
- 易于添加和删除域逻辑，而不会影响核心命令处理程序。

*缺点*

- 实施起来更复杂。

此版本的源代码可在此处获得。

## 第五集：海龟反击

到目前为止，我们还没有根据海龟的状态做出决定。因此，对于最后两种方法，我们将更改乌龟 API，这样一些命令可能会失败。

例如，我们可以说乌龟必须在有限的竞技场内移动，`move` 指令可能会导致乌龟撞上障碍物。在这种情况下，`move` 指令可以返回 `MovedOk` 或 `HitBarrier` 选项。

或者说，彩色墨水的数量是有限的。在这种情况下，尝试设置颜色可能会返回“墨水不足”的响应。

那么，让我们用这些案例更新 turtle 函数。首先是 `move` 和 `setColor` 的新响应类型：

```F#
type MoveResponse =
    | MoveOk
    | HitABarrier

type SetColorResponse =
    | ColorOk
    | OutOfInk
```

我们需要一个边界检查器来查看乌龟是否在竞技场内。假设如果位置试图超出正方形（0,0,100,100），则响应为 `HitABarrier`：

```F#
// if the position is outside the square (0,0,100,100)
// then constrain the position and return HitABarrier
let checkPosition position =
    let isOutOfBounds p =
        p > 100.0 || p < 0.0
    let bringInsideBounds p =
        max (min p 100.0) 0.0

    if isOutOfBounds position.x || isOutOfBounds position.y then
        let newPos = {
            x = bringInsideBounds position.x
            y = bringInsideBounds position.y }
        HitABarrier,newPos
    else
        MoveOk,position
```

最后，`move` 函数需要一个额外的行来检查新的位置：

```F#
let move log distance state =
    let newPosition = ...

    // adjust the new position if out of bounds
    let moveResult, newPosition = checkPosition newPosition

    ...
```

以下是完整的 `move` 功能：

```F#
let move log distance state =
    log (sprintf "Move %0.1f" distance)
    // calculate new position
    let newPosition = calcNewPosition distance state.angle state.position
    // adjust the new position if out of bounds
    let moveResult, newPosition = checkPosition newPosition
    // draw line if needed
    if state.penState = Down then
        dummyDrawLine log state.position newPosition state.color
    // return the new state and the Move result
    let newState = {state with position = newPosition}
    (moveResult,newState)
```

我们也将对 `setColor` 函数进行类似的更改，如果我们试图将颜色设置为 `Red`，则返回 `OutOfInk`。

```F#
let setColor log color state =
    let colorResult =
        if color = Red then OutOfInk else ColorOk
    log (sprintf "SetColor %A" color)
    // return the new state and the SetColor result
    let newState = {state with color = color}
    (colorResult,newState)
```

有了新版本的 turtle 函数，我们必须创建能够响应错误情况的实现。这将在接下来的两个例子中完成。

*新的 turtle 函数的源代码可以在这里找到。*

## 12：Monadic 控制流

在这种方法中，我们将重用方式 8 中的 turtle 工作流。不过，这一次，我们将根据上一个命令的结果为下一个命令做出决定。

不过，在我们这样做之前，让我们看看 `move` 的更改会对我们的代码产生什么影响。假设我们想使用 `move 40.0` 向前移动几次。

如果我们与之前一样使用 `do!` 编写代码，我们遇到了一个严重的编译器错误：

```F#
let drawShape() =
    // define a set of instructions
    let t = turtle {
        do! move 60.0
        // error FS0001:
        // This expression was expected to have type
        //    Turtle.MoveResponse
        // but here has type
        //     unit
        do! move 60.0
        }
    // etc
```

相反，我们需要使用 `let!` 并将响应分配给某个对象。

在下面的代码中，我们将响应赋给一个值，然后忽略它！

```F#
let drawShapeWithoutResponding() =
    // define a set of instructions
    let t = turtle {
        let! response = move 60.0
        let! response = move 60.0
        let! response = move 60.0
        return ()
        }

    // finally, run the monad using the initial state
    runT t initialTurtleState
```

代码确实可以编译和工作，但如果我们运行它，输出显示，在第三次调用时，我们正在把乌龟撞到墙上（100,0），没有移动到任何地方。

```
Move 60.0
...Draw line from (0.0,0.0) to (60.0,0.0) using Black
Move 60.0
...Draw line from (60.0,0.0) to (100.0,0.0) using Black
Move 60.0
...Draw line from (100.0,0.0) to (100.0,0.0) using Black
```

### 根据响应做出决策

假设我们对返回 `HitABarrier` 的 `move` 的反应是旋转 90 度并等待下一个命令。不是最聪明的算法，但它可以用于演示目的！

让我们设计一个函数来实现这一点。输入将是 `MoveResponse`，但输出将是什么？我们想以某种方式对 `turn` 动作进行编码，但原始 `turn` 函数需要我们没有的状态输入。因此，让我们返回一个 `turn` 工作流，当状态可用时（在 `run` 命令中），它表示我们想要执行的指令。

以下是代码：

```F#
let handleMoveResponse moveResponse = turtle {
    match moveResponse with
    | Turtle.MoveOk ->
        () // do nothing
    | Turtle.HitABarrier ->
        // turn 90 before trying again
        printfn "Oops -- hit a barrier -- turning"
        do! turn 90.0<Degrees>
    }
```

类型签名看起来像这样：

```F#
val handleMoveResponse : MoveResponse -> TurtleStateComputation<unit>
```

这意味着它是一个一元（或“对角线”）函数——一个从正常世界开始到 `TurtleStateComputation` 世界结束的函数。

这些正是我们可以在计算表达式 `let!` 或 `do!` 中使用“bind”的函数。

现在，我们可以在 turtle 工作流中 `move` 后添加此 `handleMoveResponse` 步骤：

```F#
let drawShape() =
    // define a set of instructions
    let t = turtle {
        let! response = move 60.0
        do! handleMoveResponse response

        let! response = move 60.0
        do! handleMoveResponse response

        let! response = move 60.0
        do! handleMoveResponse response
        }

    // finally, run the monad using the initial state
    runT t initialTurtleState
```

运行它的结果是：

```
Move 60.0
...Draw line from (0.0,0.0) to (60.0,0.0) using Black
Move 60.0
...Draw line from (60.0,0.0) to (100.0,0.0) using Black
Oops -- hit a barrier -- turning
Turn 90.0
Move 60.0
...Draw line from (100.0,0.0) to (100.0,60.0) using Black
```

你可以看到移动响应是有效的。当乌龟在（100,0）处碰到边缘时，它转了90度，下一步成功了（从（100,0）到（100,60））。

好了！此代码演示了在幕后传递状态时，如何在 `turtle` 工作流中做出决策。

### 优点和缺点

*优势*

- 计算表达式允许代码专注于逻辑，同时处理“管道”——在本例中是乌龟状态。

*缺点*

- 仍然与海龟功能的特定实现相结合。
- 计算表达式的实现可能很复杂，对于初学者来说，它们的工作原理并不明显。

此版本的源代码可在此处获得。

## 13：海龟解释器

对于我们的最后一种方法，我们将研究一种将海龟的编程与其解释完全解耦的方法。

这类似于使用命令对象的批处理方法，但经过增强以支持对命令输出的响应。

### 设计一个解释器

我们将采取的方法是为一组乌龟命令设计一个“解释器”，客户端向乌龟提供命令，并对乌龟的输出做出响应，但实际的乌龟功能稍后由特定的实现提供。

换句话说，我们有一系列交错的命令和乌龟函数，看起来像这样：



那么，我们如何在代码中对这种设计进行建模呢？

对于第一次尝试，让我们将链建模为请求/响应对的序列。我们向乌龟发送一个命令，它会以 `MoveResponse` 或其他方式做出适当的响应，如下所示：

```F#
// we send this to the turtle...
type TurtleCommand =
    | Move of Distance
    | Turn of Angle
    | PenUp
    | PenDown
    | SetColor of PenColor

// ... and the turtle replies with one of these
type TurtleResponse =
    | Moved of MoveResponse
    | Turned
    | PenWentUp
    | PenWentDown
    | ColorSet of SetColorResponse
```

问题是，我们无法确定响应是否与命令正确匹配。例如，如果我发送了一个 `Move` 命令，我希望得到一个 `MoveResponse`，而不是一个 `SetColorResponse`。但这个实现并没有强制执行！

我们想让非法状态无法表达——我们怎么能做到呢？

诀窍是将请求和响应成对组合。也就是说，对于 `Move` 命令，有一个关联的函数，该函数以 `MoveResponse` 作为输入，对于其他组合也是如此。目前，没有响应的命令可以被视为返回 `unit`。

```F#
Move command => pair of (Move command parameters), (function MoveResponse -> something)
Turn command => pair of (Turn command parameters), (function unit -> something)
etc
```

其工作原理如下：

- 客户端创建一个命令，比如 `Move 100`，并提供处理响应的附加功能。
- Move 命令的 turtle 实现（在解释器内）处理输入（`Distance`），然后生成 `MoveResponse`。
- 然后，解释器接收此 `MoveResponse` 并调用客户端提供的配对中的相关函数。

通过以这种方式将 `Move` 命令与函数相关联，我们可以保证内部 turtle 实现必须接受距离并返回 `MoveResponse`，正如我们所希望的那样。

下一个问题是：输出 `something` 是什么？它是客户端处理响应后的输出，即另一个命令/响应链！

因此，我们可以将整个成对链建模为递归结构：



或者在代码中：

```F#
type TurtleProgram =
    //         (input params)  (response)
    | Move     of Distance   * (MoveResponse -> TurtleProgram)
    | Turn     of Angle      * (unit -> TurtleProgram)
    | PenUp    of (* none *)   (unit -> TurtleProgram)
    | PenDown  of (* none *)   (unit -> TurtleProgram)
    | SetColor of PenColor   * (SetColorResponse -> TurtleProgram)
```

我将类型从 `TurtleCommand` 重命名为 `TurtleProgram`，因为它不再只是一个命令，而是一个完整的命令链和相关的响应处理程序。

不过有个问题！每一步都需要另一个 `TurtleProgram` 来跟进——那么它什么时候会停止呢？我们需要某种方式来表明没有下一个命令。

为了解决这个问题，我们将在程序类型中添加一个特殊的 `Stop` 案例：

```F#
type TurtleProgram =
    //         (input params)  (response)
    | Stop
    | Move     of Distance   * (MoveResponse -> TurtleProgram)
    | Turn     of Angle      * (unit -> TurtleProgram)
    | PenUp    of (* none *)   (unit -> TurtleProgram)
    | PenDown  of (* none *)   (unit -> TurtleProgram)
    | SetColor of PenColor   * (SetColorResponse -> TurtleProgram)
```

请注意，此结构中没有提到 `TurtleState`。海龟状态的管理方式是解释器内部的，可以说不是“指令集”的一部分。

`TurtleProgram` 是抽象语法树（AST）的一个例子，AST 是一种表示要解释（或编译）的程序的结构。

### 测试解释器

让我们使用这个模型创建一个小程序。这是我们的老朋友 `drawTriangle`：

```F#
let drawTriangle =
    Move (100.0, fun response ->
    Turn (120.0<Degrees>, fun () ->
    Move (100.0, fun response ->
    Turn (120.0<Degrees>, fun () ->
    Move (100.0, fun response ->
    Turn (120.0<Degrees>, fun () ->
    Stop))))))
```

这个程序是一个只包含客户端命令和响应的数据结构，其中任何地方都没有实际的乌龟函数！是的，现在真的很难看，但我们很快就会解决这个问题。

现在下一步是解释这个数据结构。

让我们创建一个调用真正的 turtle 函数的解释器。比如说，我们将如何实施 `Move` 案例？

正如前文所述：

- 从 `Move` 案例中获取距离和相关功能
- 使用距离和当前乌龟状态调用真实的乌龟函数，以获得 `MoveResult` 和新的乌龟状态。
- 通过将 `MoveResult` 传递给相关函数来获取程序的下一步
- 最后，使用新程序和新的 turtle 状态再次（递归）调用解释器。

```F#
let rec interpretAsTurtle state program =
    ...
    match program  with
    | Move (dist,next) ->
        let result,newState = Turtle.move log dist state
        let nextProgram = next result  // compute the next step
        interpretAsTurtle newState nextProgram
    ...
```

您可以看到，更新后的 turtle 状态作为参数传递给下一个递归调用，因此不需要可变字段。

以下是 `interpretAsTurtle` 的完整代码：

```F#
let rec interpretAsTurtle state program =
    let log = printfn "%s"

    match program  with
    | Stop ->
        state
    | Move (dist,next) ->
        let result,newState = Turtle.move log dist state
        let nextProgram = next result  // compute the next step
        interpretAsTurtle newState nextProgram
    | Turn (angle,next) ->
        let newState = Turtle.turn log angle state
        let nextProgram = next()       // compute the next step
        interpretAsTurtle newState nextProgram
    | PenUp next ->
        let newState = Turtle.penUp log state
        let nextProgram = next()
        interpretAsTurtle newState nextProgram
    | PenDown next ->
        let newState = Turtle.penDown log state
        let nextProgram = next()
        interpretAsTurtle newState nextProgram
    | SetColor (color,next) ->
        let result,newState = Turtle.setColor log color state
        let nextProgram = next result
        interpretAsTurtle newState nextProgram
```

让我们运行它：

```F#
let program = drawTriangle
let interpret = interpretAsTurtle   // choose an interpreter
let initialState = Turtle.initialTurtleState
interpret initialState program |> ignore
```

输出正是我们之前看到的：

```
Move 100.0
...Draw line from (0.0,0.0) to (100.0,0.0) using Black
Turn 120.0
Move 100.0
...Draw line from (100.0,0.0) to (50.0,86.6) using Black
Turn 120.0
Move 100.0
...Draw line from (50.0,86.6) to (0.0,0.0) using Black
Turn 120.0
```

但与之前的所有方法不同，我们可以采用完全相同的程序并以新的方式对其进行解释。我们不需要设置任何类型的依赖注入，我们只需要使用不同的解释器。

因此，让我们创建另一个解释器来聚合旅行的距离，而不关心乌龟的状态：

```F#
let rec interpretAsDistance distanceSoFar program =
    let recurse = interpretAsDistance
    let log = printfn "%s"

    match program with
    | Stop ->
        distanceSoFar
    | Move (dist,next) ->
        let newDistanceSoFar = distanceSoFar + dist
        let result = Turtle.MoveOk   // hard-code result
        let nextProgram = next result
        recurse newDistanceSoFar nextProgram
    | Turn (angle,next) ->
        // no change in distanceSoFar
        let nextProgram = next()
        recurse distanceSoFar nextProgram
    | PenUp next ->
        // no change in distanceSoFar
        let nextProgram = next()
        recurse distanceSoFar nextProgram
    | PenDown next ->
        // no change in distanceSoFar
        let nextProgram = next()
        recurse distanceSoFar nextProgram
    | SetColor (color,next) ->
        // no change in distanceSoFar
        let result = Turtle.ColorOk   // hard-code result
        let nextProgram = next result
        recurse distanceSoFar nextProgram
```

在这种情况下，我将 `interpretAsDistance` 设置本地别名为 `recurse`，以明确发生了什么样的递归。

让我们用这个新的解释器运行同样的程序：

```F#
let program = drawTriangle           // same program
let interpret = interpretAsDistance  // choose an interpreter
let initialState = 0.0
interpret initialState program |> printfn "Total distance moved is %0.1f"
```

输出再次完全符合我们的预期：

```
Total distance moved is 300.0
```

### 创建“海龟程序”工作流程

创建要解释的程序的代码非常丑陋！我们可以创建一个计算表达式来让它看起来更好吗？

为了创建计算表达式，我们需要 `return` 和 `bind` 函数，这些函数要求 `TurtleProgram` 类型是泛型的。

没问题！那么，让我们将 `TurtleProgram` 设为泛型：

```F#
type TurtleProgram<'a> =
    | Stop     of 'a
    | Move     of Distance * (MoveResponse -> TurtleProgram<'a>)
    | Turn     of Angle    * (unit -> TurtleProgram<'a>)
    | PenUp    of            (unit -> TurtleProgram<'a>)
    | PenDown  of            (unit -> TurtleProgram<'a>)
    | SetColor of PenColor * (SetColorResponse -> TurtleProgram<'a>)
```

请注意，`Stop` 案例现在有一个与之关联的 `'a` 类型的值。这是我们正确实现 `return` 所必需的：

```F#
let returnT x =
    Stop x
```

`bind` 函数的实现更为复杂。现在不用担心它是如何工作的——重要的是类型匹配并且编译！

```F#
let rec bindT f inst  =
    match inst with
    | Stop x ->
        f x
    | Move(dist,next) ->
        (*
        Move(dist,fun moveResponse -> (bindT f)(next moveResponse))
        *)
        // "next >> bindT f" is a shorter version of function response
        Move(dist,next >> bindT f)
    | Turn(angle,next) ->
        Turn(angle,next >> bindT f)
    | PenUp(next) ->
        PenUp(next >> bindT f)
    | PenDown(next) ->
        PenDown(next >> bindT f)
    | SetColor(color,next) ->
        SetColor(color,next >> bindT f)
```

有了 `bind` 和 `return`，我们可以创建一个计算表达式：

```F#
// define a computation expression builder
type TurtleProgramBuilder() =
    member this.Return(x) = returnT x
    member this.Bind(x,f) = bindT f x
    member this.Zero(x) = returnT ()

// create an instance of the computation expression builder
let turtleProgram = TurtleProgramBuilder()
```

我们现在可以创建一个处理 `MoveResponse`s 的工作流，就像前面的一元控制流示例（方式 12）一样。

```F#
// helper functions
let stop = fun x -> Stop x
let move dist  = Move (dist, stop)
let turn angle  = Turn (angle, stop)
let penUp  = PenUp stop
let penDown  = PenDown stop
let setColor color = SetColor (color,stop)

let handleMoveResponse log moveResponse = turtleProgram {
    match moveResponse with
    | Turtle.MoveOk ->
        ()
    | Turtle.HitABarrier ->
        // turn 90 before trying again
        log "Oops -- hit a barrier -- turning"
        let! x = turn 90.0<Degrees>
        ()
    }

// example
let drawTwoLines log = turtleProgram {
    let! response = move 60.0
    do! handleMoveResponse log response
    let! response = move 60.0
    do! handleMoveResponse log response
    }
```

让我们使用真实的 turtle 函数来解释这一点（假设 `interpretAsTurtle` 函数已被修改以处理新的泛型结构）：

```F#
let log = printfn "%s"
let program = drawTwoLines log
let interpret = interpretAsTurtle
let initialState = Turtle.initialTurtleState
interpret initialState program |> ignore
```

输出显示，当遇到障碍时，`MoveResponse` 确实得到了正确处理：

```
Move 60.0
...Draw line from (0.0,0.0) to (60.0,0.0) using Black
Move 60.0
...Draw line from (60.0,0.0) to (100.0,0.0) using Black
Oops -- hit a barrier -- turning
Turn 90.0
```

### 重构 `TurtleProgram` 分为两部分

这种方法工作得很好，但让我感到困扰的是，`TurtleProgram` 类型中有一个特殊的 `Stop` 案例。如果我们能以某种方式专注于五只乌龟的行为而忽略它，那就太好了。

事实证明，有一种方法可以做到这一点。在 Haskell 和 Scalaz 中，它被称为“free monad”，但由于 F# 不支持类型类，我将称之为“free monad 模式”，你可以用它来解决这个问题。你必须写一些样板，但不多。

诀窍是将 api 案例和“stop”/“keep going”逻辑分为两种不同的类型，如下所示：

```F#
/// Create a type to represent each instruction
type TurtleInstruction<'next> =
    | Move     of Distance * (MoveResponse -> 'next)
    | Turn     of Angle    * 'next
    | PenUp    of            'next
    | PenDown  of            'next
    | SetColor of PenColor * (SetColorResponse -> 'next)

/// Create a type to represent the Turtle Program
type TurtleProgram<'a> =
    | Stop of 'a
    | KeepGoing of TurtleInstruction<TurtleProgram<'a>>
```

请注意，我还将 `Turn`、`PenUp` 和 `PenDown` 的响应更改为单个值，而不是 unit 函数。`Move` 和 `SetColor` 仍然是函数。

在这种新的“自由 monad”方法中，我们需要编写的唯一自定义代码是 api 类型的简单映射函数，在本例中为 `TurtleInstruction`：

```F#
let mapInstr f inst  =
    match inst with
    | Move(dist,next) ->      Move(dist,next >> f)
    | Turn(angle,next) ->     Turn(angle,f next)
    | PenUp(next) ->          PenUp(f next)
    | PenDown(next) ->        PenDown(f next)
    | SetColor(color,next) -> SetColor(color,next >> f)
```

其余的代码（`return`、`bind` 和计算表达式）总是以完全相同的方式实现，而不管特定的 api 是什么。也就是说，需要更多的样板，但需要更少的思考！

解释器需要更换，以处理新的案例。以下是新版 `interpretAsTurtle` 的一个片段：

```F#
let rec interpretAsTurtle log state program =
    let recurse = interpretAsTurtle log

    match program with
    | Stop a ->
        state
    | KeepGoing (Move (dist,next)) ->
        let result,newState = Turtle.move log dist state
        let nextProgram = next result // compute next program
        recurse newState nextProgram
    | KeepGoing (Turn (angle,next)) ->
        let newState = Turtle.turn log angle state
        let nextProgram = next        // use next program directly
        recurse newState nextProgram
```

在创建工作流时，我们还需要调整辅助函数。您可以在下面看到，我们现在有稍微复杂的代码，如 `KeepGoing(Move(dist, Stop))`，而不是原始解释器中的简单代码。

```F#
// helper functions
let stop = Stop()
let move dist  = KeepGoing (Move (dist, Stop))    // "Stop" is a function
let turn angle  = KeepGoing (Turn (angle, stop))  // "stop" is a value
let penUp  = KeepGoing (PenUp stop)
let penDown  = KeepGoing (PenDown stop)
let setColor color = KeepGoing (SetColor (color,Stop))

let handleMoveResponse log moveResponse = turtleProgram {
    ... // as before

// example
let drawTwoLines log = turtleProgram {
    let! response = move 60.0
    do! handleMoveResponse log response
    let! response = move 60.0
    do! handleMoveResponse log response
    }
```

但有了这些更改，我们就完成了，代码和以前一样工作。

### 解释器模式的优缺点

*优势*

- *解耦*。抽象语法树将程序流与实现完全解耦，并提供了很大的灵活性。
- *优化*。抽象语法树可以在运行之前进行操作和更改，以便进行优化或其他转换。例如，对于 turtle 程序，我们可以处理树并将 `Turn` 的所有连续序列折叠为单个 `Turn` 操作。这是一个简单的优化，可以节省我们与实体乌龟交流的次数。推特的 Stitch 库做了这样的事情，但显然是以一种更复杂的方式。这段视频有很好的解释。
- *低功耗代码*。创建抽象语法树的“free monad”方法允许您专注于 API 而忽略 Stop / KeepGoing 逻辑，这也意味着只需要定制最少量的代码。有关免费单子（free monad）的更多信息，请从这个优秀的视频开始，然后查看这篇文章和这篇文章。

*缺点*

- 理解起来很复杂。
- 只有在执行的操作有限的情况下才能很好地工作。
- 如果AST变得太大，可能会效率低下。

此版本的源代码可在此处（原始版本）和此处（“免费 monad”版本）获得。

## 对所用技术的回顾

在这篇文章中，我们研究了使用多种不同技术实现海龟 API 的十三种不同方法。让我们快速总结一下所使用的所有技术：

- **纯无状态函数**。如所有面向 FP 的示例所示。所有这些都很容易测试和模拟。
- **部分应用**。如最简单的 FP 示例（方式 2）所示，当海龟函数应用了日志功能，使主流可以使用管道时，它被广泛使用，特别是在“使用函数进行依赖注入的方法”（方式 7）中。
- **对象表达式**，用于在不创建类的情况下实现接口，如方式 6 所示。
- **结果类型**（也称为“Either 单子”）。在所有函数式 API 示例中使用（例如方式 4），以返回错误而不是抛出异常。
- **应用“提升”**（如 `lift2`）将正常功能提升到结果世界，同样以方式4和其他方式。
- **管理状态的多种方式**：
  - 可变字段（方式 1）
  - 明确管理状态并通过一系列函数将其管道化（方式 2）
  - 仅在边缘具有状态（方式 4 中的函数式核心/命令外壳）
  - 在代理中隐藏状态（方式 5）
  - 在 state monad 中线程化幕后状态（方式 8 和 12 中的 `turtle`工作流）
  - 通过使用批量命令（方式 9）或批量事件（方式 10）或解释器（方式 13）来完全避免状态
- **将函数包装在类型中**。在方式 8 中用于管理状态（状态单子），在方式 13 中用于存储响应。
- **计算表达式**，很多！我们创建并使用了三个：
  - 处理错误的 `result`
  - `turtle` 管理乌龟状态
  - `turtleProgram` 用于在解释器方法中构建 AST（方法 13）。
- 在 `result` 和 `turtle` 工作流中**链接一元函数**。底层函数是一元函数（monadic，“对角线（diagonal）”），通常不会正确组合，但在工作流中，它们可以轻松透明地排序。
- 在“函数依赖注入”示例中（方式 7），**将行为表示为数据结构**，以便可以传入单个函数而不是整个接口。
- **使用以数据为中心的协议进行解耦**，如代理、批处理命令、事件源和解释器示例所示。
- **使用代理进行无锁异步处理**（方式 5）。
- **“构建”计算 vs “运行”计算的分离**，如 `turtle` 工作流（方式 8 和 12）和 `turtleProgram` 工作流（方式 13：解释器）所示。
- **使用事件溯源从头开始重建状态**，而不是在内存中维护可变状态，如事件溯源（方式 10）和 FRP（方式 11）示例所示。
- **使用事件流**和 FRP（方式 11）将业务逻辑分解为小型、独立和解耦的处理器，而不是具有单片对象。

我希望很明显，研究这十三种方法只是一个有趣的练习，我并不是建议你立即将所有代码转换为使用流处理器和解释器！而且，特别是如果你和函数式编程的新手一起工作，我倾向于坚持使用早期（和更简单）的方法，除非有明显的好处来换取额外的复杂性。

## 摘要

> 当乌龟爬出视线时，它标志着许多圆圈中的一个的边缘 - *《看乌龟的十三种方式》，华莱士·D·科瑞萨（Wallace D Coriacea）*

我希望你喜欢这篇文章。我当然很喜欢写它。和往常一样，它的篇幅比我预想的要长得多，所以我希望读这文章的努力对你来说是值得的！

如果你喜欢这种比较方法，并且想要更多，请查看严崔的帖子，他正在自己的博客上做类似的事情。

享受剩余的 F# 降临日历。节日快乐！

这篇文章的源代码可以在 github 上找到。



# 观察乌龟的十三种方法 - 附录

奖励方式：抽象数据龟和基于能力的龟。
07十二月2015 这篇文章已经超过3岁了

https://fsharpforfunandprofit.com/posts/13-ways-of-looking-at-a-turtle-3/

更新：我关于这个话题的演讲幻灯片和视频

在这篇由两部分组成的巨型帖子的第三部分中，我将继续将简单的乌龟图形模型拉伸到断裂点。

在第一篇和第二篇文章中，我描述了 13 种不同的查看海龟图形实现的方法。

不幸的是，在我发表这些文章后，我意识到还有其他一些我忘了提到的方法。因此，在这篇文章中，您将看到两种奖金方式。

- 方式14。抽象数据海龟，其中我们使用抽象数据类型封装海龟实现的细节。
- 方式15。基于能力的 Turtle，我们根据乌龟的当前状态控制客户可以使用哪些乌龟功能。

作为提醒，以下是前十三种方式：

- 方式1。一种基本的面向对象方法，其中我们创建了一个具有可变状态的类。
- 方式2。一种基本的函数方法，其中我们创建了一个具有不可变状态的函数模块。
- 方式3。具有面向对象核心的 API，其中我们创建了一个面向对象的API，该API调用有状态核心类。
- 方式4。一个带有函数式核心的 API，其中我们创建了一个使用无状态核心函数的有状态API。
- 方式5。代理前面的 API，其中我们创建了一个 API，该 API 使用消息队列与代理通信。
- 方式6。使用接口的依赖注入，其中我们使用接口或函数记录将实现与 API 解耦。
- 方式7。使用函数的依赖注入，其中我们通过传递函数参数将实现与 API 解耦。
- 方式8。使用状态单子进行批处理，其中我们创建了一个特殊的“海龟工作流”计算表达式来为我们跟踪状态。
- 方式9。使用命令对象进行批处理，其中我们创建一个类型来表示 turtle 命令，然后一次处理一系列命令。
- 间奏：有意识地用数据类型解耦。关于使用数据与接口进行解耦的几点说明。
- 方式10。事件溯源，其中状态是根据过去的事件列表构建的。
- 方式11。函数式回溯编程（流处理），其中业务逻辑基于对早期事件的反应。
- 第五集：乌龟反击战，乌龟 API 发生变化，一些命令可能会失败。
- 方式12。一元控制流，其中我们根据早期命令的结果在海龟工作流中做出决策。
- 方式13。一个 turtle 解释器，其中我们将 turtle 编程与 turtle 实现完全解耦，几乎遇到了免费 monad。
- 回顾所有使用的技术。

这篇文章的所有源代码都可以在 github 上找到。

## 14：抽象数据海龟

在这个设计中，我们使用抽象数据类型的概念来封装海龟上的操作。

也就是说，“turtle”被定义为不透明类型以及相应的一组操作，就像定义 `List`、`Set` 和 `Map` 等标准 F# 类型一样。

也就是说，我们有许多函数在类型上工作，但我们不允许看到类型本身的“内部”。

从某种意义上说，您可以将其视为方式 1 中 OO 方法和方式 2 中函数方法的第三种选择。

- 在 OO 实现中，内部细节被很好地封装，并且只能通过方法进行访问。OO 类的缺点是它是可变的。
- 在 FP 实现中，`TurtleState` 是不可变的，但缺点是状态的内部是公共的，一些客户端可能已经访问了这些字段，所以如果我们改变了 `TurtleState` 的设计，这些客户端可能会崩溃。

抽象数据类型实现结合了两者的优点：乌龟状态是不可变的，就像原始的 FP 方式一样，但没有客户端可以访问它，就像 OO 方式一样。

此（以及任何抽象类型）的设计如下：

- 乌龟状态类型本身是公共的，但它的构造函数和字段是私有的。
- 相关 `Turtle` 模块中的函数可以看到 turtle 状态类型内部（因此与 FP 设计相同）。
- 因为 turtle 状态构造函数是私有的，所以我们需要在 `Turtle` 模块中有一个构造函数。
- 客户端看不到 turtle 状态类型内部，因此必须完全依赖 `Turtle` 模块函数。

这就是全部。我们只需要在早期的 FP 版本中添加一些隐私修饰符，就完成了！

### 实现

首先，我们将把 turtle 状态类型和 `Turtle` 模块放在一个名为 `AdtTurtle` 的公共模块中。这使得 `AdtTurtle.Turtle` 模块中的功能可以访问乌龟状态，但在 `AdtTurtle` 外部无法访问。

接下来，海龟状态类型现在将被称为 `Turtle`，而不是 `TurtleState`，因为我们几乎将其视为一个对象。

最后，相关模块 `Turtle`（包含函数）将具有一些特殊属性：

- `RequireQualifiedAccess` 意味着在访问函数时必须使用模块名称（就像 `List` 模块一样）
- 需要 `ModuleSuffix`，以便该模块可以与状态类型同名。对于泛型类型，这不是必需的（例如，如果我们有 `Turtle<'a>`）。

```F#
module AdtTurtle =

    /// A private structure representing the turtle
    type Turtle = private {
        position : Position
        angle : float<Degrees>
        color : PenColor
        penState : PenState
    }

    /// Functions for manipulating a turtle
    /// "RequireQualifiedAccess" means the module name *must*
    ///    be used (just like List module)
    /// "ModuleSuffix" is needed so the that module can
    ///    have the same name as the state type
    [<RequireQualifiedAccess>]
    [<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
    module Turtle =
```

避免冲突的另一种方法是让状态类型具有不同的大小写，或者具有小写别名的不同名称，如下所示：

```F#
type TurtleState = { ... }
type turtle = TurtleState

module Turtle =
    let something (t:turtle) = t
```

无论如何命名，我们都需要一种方法来构造一只新的 `Turtle`。

如果构造函数没有参数，并且状态是不可变的，那么我们只需要一个初始值，而不是一个函数（比如 `Set.empty`）。

否则，我们可以定义一个名为 `make`（或 `create` 或类似）的函数：

```F#
[<RequireQualifiedAccess>]
[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module Turtle =

    /// return a new turtle with the specified color
    let make(initialColor) = {
        position = initialPosition
        angle = 0.0<Degrees>
        color = initialColor
        penState = initialPenState
    }
```

海龟模块的其余函数与方式 2 中的实现保持不变。

### ADT 客户端

现在让我们看看客户。

首先，让我们检查一下状态是否真的是私有的。如果我们尝试显式创建一个状态，如下所示，我们会得到一个编译器错误：

```F#
let initialTurtle = {
    position = initialPosition
    angle = 0.0<Degrees>
    color = initialColor
    penState = initialPenState
}
// Compiler error FS1093:
//    The union cases or fields of the type 'Turtle'
//    are not accessible from this code location
```

如果我们使用构造函数，然后尝试直接访问一个字段（如 `position`），我们会再次收到编译器错误：

```F#
let turtle = Turtle.make(Red)
printfn "%A" turtle.position
// Compiler error FS1093:
//    The union cases or fields of the type 'Turtle'
//    are not accessible from this code location
```

但是，如果我们坚持使用 `Turtle` 模块中的函数，我们可以安全地创建一个状态值，然后像以前一样调用它的函数：

```F#
// versions with log baked in (via partial application)
let move = Turtle.move log
let turn = Turtle.turn log
// etc

let drawTriangle() =
    Turtle.make(Red)
    |> move 100.0
    |> turn 120.0<Degrees>
    |> move 100.0
    |> turn 120.0<Degrees>
    |> move 100.0
    |> turn 120.0<Degrees>
```

### ADT 的优缺点

*优势*

- 所有代码都是无状态的，因此易于测试。
- 状态的封装意味着焦点始终完全放在类型的行为和属性上。
- 客户端永远不能依赖于特定的实现，这意味着可以安全地更改实现。
- 您甚至可以交换实现（例如通过阴影或链接到不同的程序集）以进行测试、性能等。

*缺点*

- 客户必须管理当前的乌龟状态。
- 客户端无法控制实现（例如通过使用依赖注入）。

有关 F# 中 ADT 的更多信息，请参阅 Bryan Edds 的演讲和帖子。

此版本的源代码可在此处获得。

## 15：基于能力的海龟

在“一元控制流”方法（方法 12）中，我们处理了乌龟告诉我们它碰到了障碍的反应。

但即使我们遇到了障碍，也没有什么能阻止我们一遍又一遍地呼叫 `move` 行动！

现在想象一下，一旦我们遇到障碍，`move` 操作就不再对我们可用。我们不能滥用它，因为它将不再存在！

为了实现这一点，我们不应该提供 API，而是在每次调用后返回一个函数列表，客户端可以调用这些函数来执行下一步。这些功能通常包括 `move`、`turn`、`penUp` 等常见的嫌疑人，但当我们遇到障碍时，移动将从该列表中删除。简单但有效。

这种技术与一种称为基于能力的安全的授权和安全技术密切相关。如果你有兴趣了解更多，我有一系列专门的帖子。

### 设计基于能力的海龟

第一件事是定义每次调用后将返回的函数记录：

```F#
type MoveResponse =
    | MoveOk
    | HitABarrier

type SetColorResponse =
    | ColorOk
    | OutOfInk

type TurtleFunctions = {
    move     : MoveFn option
    turn     : TurnFn
    penUp    : PenUpDownFn
    penDown  : PenUpDownFn
    setBlack : SetColorFn  option
    setBlue  : SetColorFn  option
    setRed   : SetColorFn  option
    }
and MoveFn =      Distance -> (MoveResponse * TurtleFunctions)
and TurnFn =      Angle    -> TurtleFunctions
and PenUpDownFn = unit     -> TurtleFunctions
and SetColorFn =  unit     -> (SetColorResponse * TurtleFunctions)
```

让我们详细看看这些声明。

首先，任何地方都没有 `TurtleState`。已发布的 turtle 函数将为我们封装状态。同样，也没有 `log` 函数。

接下来，函数的记录 `TurtleFunctions` 为API中的每个函数定义了一个字段（`move`、`turn` 等）：

- `move` 函数是可选的，这意味着它可能不可用。
- `turn`、`penUp` 和 `penDown` 函数始终可用。
- `setColor` 操作分为三个单独的函数，每种颜色一个，因为您可能无法使用红色墨水，但仍然可以使用蓝色墨水。为了表示这些功能可能不可用，再次使用 `option`。

我们还为每个函数声明了类型别名，以使其更容易工作。编写 `MoveFn` 比在任何地方编写 `Distance -> (MoveResponse * TurtleFunctions)` 更容易！请注意，由于这些定义是相互递归的，我不得不使用 `and` 关键字。

最后，请注意本设计中 `MoveFn` 的签名与方式 12 的早期设计中 `move` 的签名之间的区别。

早期版本：

```F#
val move :
    Log -> Distance -> TurtleState -> (MoveResponse * TurtleState)
```

新版本：

```F#
val move :
    Distance -> (MoveResponse * TurtleFunctions)
```

在输入端，`Log` 和 `TurtleState` 参数已消失，在输出端，`TurtleState` 已被 `TurtleFunctions` 替换。

这意味着，以某种方式，每个 API 函数的输出都必须更改为 `TurtleFunctions` 记录。

### 实现海龟行动

为了决定我们是否真的可以移动或使用特定的颜色，我们首先需要增加 `TurtleState` 类型来跟踪这些因素：

```F#
type Log = string -> unit

type private TurtleState = {
    position : Position
    angle : float<Degrees>
    color : PenColor
    penState : PenState

    canMove : bool                // new!
    availableInk: Set<PenColor>   // new!
    logger : Log                  // new!
}
```

这已经通过以下方式得到了增强

- `canMove`，如果为 false，则表示我们处于障碍状态，不应返回有效的 `move` 函数。
- `availableInk` 包含一组颜色。如果一种颜色不在此集合中，那么我们不应该为该颜色返回有效的 `setColorXXX` 函数。
- 最后，我们将 `log` 函数添加到状态中，这样我们就不必显式地将其传递给每个操作。当乌龟被创造出来时，它将被设置一次。

`TurtleState` 现在有点丑陋，但没关系，因为它是私人的！客户甚至永远不会看到它。

有了这个增强的状态，我们可以改变 `move`。首先，我们将其设置为私有，其次，在返回新状态之前，我们将设置 `canMove` 标志（使用 `moveResult <> HitABarrier`）：

```F#
/// Function is private! Only accessible to the client via the TurtleFunctions record
let private move log distance state =

    log (sprintf "Move %0.1f" distance)
    // calculate new position
    let newPosition = calcNewPosition distance state.angle state.position
    // adjust the new position if out of bounds
    let moveResult, newPosition = checkPosition newPosition
    // draw line if needed
    if state.penState = Down then
        dummyDrawLine log state.position newPosition state.color

    // return the new state and the Move result
    let newState = {
        state with
         position = newPosition
         canMove = (moveResult <> HitABarrier)   // NEW!
        }
    (moveResult,newState)
```

我们需要一些改变 `canMove` 回到 true 的方法！所以，让我们假设如果你转身，你可以再次移动。

让我们将该逻辑添加到 `turn` 函数中：

```F#
let private turn log angle state =
    log (sprintf "Turn %0.1f" angle)
    // calculate new angle
    let newAngle = (state.angle + angle) % 360.0<Degrees>
    // NEW!! assume you can always move after turning
    let canMove = true
    // update the state
    {state with angle = newAngle; canMove = canMove}
```

`penUp` 和 `penDown` 函数保持不变，只是被设置为私有。

对于最后一个操作 `setColor`，只要墨水只使用一次，我们就会从可用性集中删除它！

```F#
let private setColor log color state =
    let colorResult =
        if color = Red then OutOfInk else ColorOk
    log (sprintf "SetColor %A" color)

    // NEW! remove color ink from available inks
    let newAvailableInk = state.availableInk |> Set.remove color

    // return the new state and the SetColor result
    let newState = {state with color = color; availableInk = newAvailableInk}
    (colorResult,newState)
```

最后，我们需要一个函数，可以从 `TurtleState` 创建 `TurtleFunctions` 记录。我称之为 `createTurtleFunctions`。

这是完整的代码，我将在下面详细讨论：

```F#
/// Create the TurtleFunctions structure associated with a TurtleState
let rec private createTurtleFunctions state =
    let ctf = createTurtleFunctions  // alias

    // create the move function,
    // if the turtle can't move, return None
    let move =
        // the inner function
        let f dist =
            let resp, newState = move state.logger dist state
            (resp, ctf newState)

        // return Some of the inner function
        // if the turtle can move, or None
        if state.canMove then
            Some f
        else
            None

    // create the turn function
    let turn angle =
        let newState = turn state.logger angle state
        ctf newState

    // create the pen state functions
    let penDown() =
        let newState = penDown state.logger state
        ctf newState

    let penUp() =
        let newState = penUp state.logger state
        ctf newState

    // create the set color functions
    let setColor color =
        // the inner function
        let f() =
            let resp, newState = setColor state.logger color state
            (resp, ctf newState)

        // return Some of the inner function
        // if that color is available, or None
        if state.availableInk |> Set.contains color then
            Some f
        else
            None

    let setBlack = setColor Black
    let setBlue = setColor Blue
    let setRed = setColor Red

    // return the structure
    {
    move     = move
    turn     = turn
    penUp    = penUp
    penDown  = penDown
    setBlack = setBlack
    setBlue  = setBlue
    setRed   = setRed
    }
```

让我们看看这是如何工作的。

首先，请注意，此函数需要附加 `rec` 关键字，因为它引用自身。我也为它添加了一个较短的别名（`ctf`）。

接下来，将创建每个 API 函数的新版本。例如，一个新的 `turn` 函数定义如下：

```F#
let turn angle =
    let newState = turn state.logger angle state
    ctf newState
```

这将使用 logger 和 state 调用原始的 `turn` 函数，然后使用递归调用（`ctf`）将新状态转换为函数记录。

对于像 `move` 这样的可选功能，它有点复杂。使用原始 `move` 定义一个内部函数 `f`，然后根据 `state.canMove` 标志是否设置，`f` 返回 `Some` 或 `None`：

```F#
// create the move function,
// if the turtle can't move, return None
let move =
    // the inner function
    let f dist =
        let resp, newState = move state.logger dist state
        (resp, ctf newState)

    // return Some of the inner function
    // if the turtle can move, or None
    if state.canMove then
        Some f
    else
        None
```

同样，对于 `setColor`，定义了一个内部函数 `f`，然后根据颜色参数是否在 `state.availableInk` 集合中返回：

```F#
let setColor color =
    // the inner function
    let f() =
        let resp, newState = setColor state.logger color state
        (resp, ctf newState)

    // return Some of the inner function
    // if that color is available, or None
    if state.availableInk |> Set.contains color then
        Some f
    else
        None
```

最后，所有这些函数都添加到记录中：

```F#
// return the structure
{
move     = move
turn     = turn
penUp    = penUp
penDown  = penDown
setBlack = setBlack
setBlue  = setBlue
setRed   = setRed
}
```

这就是你建立 `TurtleFunctions` 记录的方法！

我们还需要一件事：一个构造函数来创建 `TurtleFunctions` 的一些初始值，因为我们不再可以直接访问 API。这现在是客户唯一可用的公共功能！

```F#
/// Return the initial turtle.
/// This is the ONLY public function!
let make(initialColor, log) =
    let state = {
        position = initialPosition
        angle = 0.0<Degrees>
        color = initialColor
        penState = initialPenState
        canMove = true
        availableInk = [Black; Blue; Red] |> Set.ofList
        logger = log
    }
    createTurtleFunctions state
```

此函数在 `log` 函数中烘焙，创建新状态，然后调用 `createTurtleFunctions` 返回 `TurtleFunction` 记录供客户端使用。

### 实现基于能力的海龟客户端

让我们现在尝试使用它。首先，让我们尝试 `move 60`，然后再 `move 60`。第二步应该把我们带到边界（100），因此在那一点上，`move` 函数应该不再可用。

首先，我们使用 `Turtle.make` 创建 `TurtleFunctions` 记录。然后我们不能立即移动，我们必须先测试一下 `move` 函数是否可用：

```F#
let testBoundary() =
    let turtleFns = Turtle.make(Red,log)
    match turtleFns.move with
    | None ->
        log "Error: Can't do move 1"
    | Some moveFn ->
        ...
```

在最后一种情况下，`moveFn` 可用，因此我们可以在 60 的距离内调用它。

函数的输出是一对：一个 `MoveResponse` 类型和一个新的 `TurtleFunctions` 记录。

我们将忽略 `MoveResponse` 并再次检查 `TurtleFunctions` 记录，看看我们是否可以执行下一步操作：

```F#
let testBoundary() =
    let turtleFns = Turtle.make(Red,log)
    match turtleFns.move with
    | None ->
        log "Error: Can't do move 1"
    | Some moveFn ->
        let (moveResp,turtleFns) = moveFn 60.0
        match turtleFns.move with
        | None ->
            log "Error: Can't do move 2"
        | Some moveFn ->
            ...
```

最后，再来一次：

```F#
let testBoundary() =
    let turtleFns = Turtle.make(Red,log)
    match turtleFns.move with
    | None ->
        log "Error: Can't do move 1"
    | Some moveFn ->
        let (moveResp,turtleFns) = moveFn 60.0
        match turtleFns.move with
        | None ->
            log "Error: Can't do move 2"
        | Some moveFn ->
            let (moveResp,turtleFns) = moveFn 60.0
            match turtleFns.move with
            | None ->
                log "Error: Can't do move 3"
            | Some moveFn ->
                log "Success"
```

如果我们运行这个，我们得到输出：

```
Move 60.0
...Draw line from (0.0,0.0) to (60.0,0.0) using Red
Move 60.0
...Draw line from (60.0,0.0) to (100.0,0.0) using Red
Error: Can't do move 3
```

这确实表明，这个概念正在发挥作用！

嵌套选项匹配真的很难看，所以让我们快速制定一个 `maybe` 工作流程，让它看起来更好：

```F#
type MaybeBuilder() =
    member this.Return(x) = Some x
    member this.Bind(x,f) = Option.bind f x
    member this.Zero() = Some()
let maybe = MaybeBuilder()
```

以及我们可以在工作流中使用的日志功能：

```F#
/// A function that logs and returns Some(),
/// for use in the "maybe" workflow
let logO message =
    printfn "%s" message
    Some ()
```

现在，我们可以尝试使用 `maybe` 工作流设置一些颜色：

```F#
let testInk() =
    maybe {
    // create a turtle
    let turtleFns = Turtle.make(Black,log)

    // attempt to get the "setRed" function
    let! setRedFn = turtleFns.setRed

    // if so, use it
    let (resp,turtleFns) = setRedFn()

    // attempt to get the "move" function
    let! moveFn = turtleFns.move

    // if so, move a distance of 60 with the red ink
    let (resp,turtleFns) = moveFn 60.0

    // check if the "setRed" function is still available
    do! match turtleFns.setRed with
        | None ->
            logO "Error: Can no longer use Red ink"
        | Some _ ->
            logO "Success: Can still use Red ink"

    // check if the "setBlue" function is still available
    do! match turtleFns.setBlue with
        | None ->
            logO "Error: Can no longer use Blue ink"
        | Some _ ->
            logO "Success: Can still use Blue ink"

    } |> ignore
```

其输出为：

```
SetColor Red
Move 60.0
...Draw line from (0.0,0.0) to (60.0,0.0) using Red
Error: Can no longer use Red ink
Success: Can still use Blue ink
```

实际上，使用 `maybe` 工作流并不是一个好主意，因为第一个失败会退出工作流！你会想为真正的代码想出更好的东西，但我希望你能理解。

### 基于能力的方法的优缺点

*优势*

- 防止客户端滥用 API。
- 允许 API 在不影响客户端的情况下发展（和下放）。例如，我可以通过在函数记录中为每个颜色函数硬编码 `None` 来转换为仅单色的乌龟，之后我可以安全地删除 `setColor` 实现。在这个过程中，没有客户会崩溃！这类似于 RESTful web 服务的 HATEAOS 方法。
- 客户端与特定的实现是解耦的，因为函数的记录充当了接口。

缺点

- 实施起来很复杂。
- 客户端的逻辑要复杂得多，因为它永远无法确定某个函数是否可用！每次都要检查。
- API 不像一些面向数据的 API 那样易于序列化。

有关基于能力的安全性的更多信息，请参阅我的帖子或观看我的“企业 Tic Tac Toe”视频。

此版本的源代码可在此处获得。

## 摘要

> 我有三个想法，就像一棵手指树，里面有三只不变的乌龟 - *《看乌龟的十三种方式》，华莱士·D·科瑞萨（Wallace D Coriacea）*

我现在感觉好多了，因为我有了这两种额外的方法！感谢您的阅读！

这篇文章的源代码可以在 github 上找到。

