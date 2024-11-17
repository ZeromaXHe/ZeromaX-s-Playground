# [返回主 Markdown](./FSharpForFunAndProfit翻译.md)



# 1 弗兰肯福克托（Frankenfunctor）博士和莫纳德斯特（Monadster）

*Part of the "Handling State" series (*[link](https://fsharpforfunandprofit.com/posts/monadster/#series-toc)*)*

或者，一位19世纪的科学家是如何差点发明了国家单子的
07七月2015这篇文章是超过3岁

https://fsharpforfunandprofit.com/posts/monadster

*更新：我关于这个话题的演讲幻灯片和视频*

*警告！这篇文章包含可怕的话题、紧张的类比、对单子的讨论*

几代人以来，我们一直被弗兰肯芬克特博士的悲剧故事所吸引。对生命力的迷恋，早期对电和镀锌的实验，以及最终的突破，最终使一系列尸体部件——Monadster——复活。

但是，正如我们所知，这种生物逃脱了，自由的 Monadster 在计算机科学会议上横冲直撞，甚至给最有经验的程序员带来了恐惧。

![The horror, The horror](https://fsharpforfunandprofit.com/posts/monadster/monadster_horror.jpg)

*标题：1990 年 ACM LISP 和函数式编程会议上发生的可怕事件。*

我在这里不再重复细节；这个故事太可怕了，记不起来了。

但在这场悲剧的数百万字中，有一个话题从未得到令人满意的解决。

*这个生物是如何组装并复活的？*

我们知道弗兰肯芬克特博士用尸体的部分构建了这个生物，然后在一瞬间用闪电创造了生命力。

但身体的各个部分必须组装成一个整体，生命力必须以适当的方式通过组件传递，所有这一切都在闪电击中的瞬间完成。

我花了很多年的时间研究这个问题，最近，我花了很大的钱，设法获得了弗兰肯芬克特博士的个人实验室笔记本。

最后，我可以向全世界展示弗兰肯芬克特博士的技术。随心所欲地使用它。我不会对它的道德性做出任何判断，毕竟，不仅仅是开发人员可以质疑我们构建的东西对现实世界的影响。

## 背景

首先，你需要了解所涉及的基本过程。

首先，你必须知道，弗兰肯芬克特医生没有全身可用。相反，这种生物是由一系列身体部位——手臂、腿、大脑、心脏——组合而成的，这些部位的来源是模糊的，最好不要说。

弗兰肯医生从尸体的一部分开始，给它注入了一定量的生命力。结果是两件事：一个是现在活着的身体部分，另一个是剩下的、减弱的生命力，因为当然，一些生命力被转移到了活着的部分。

这是一个展示原理的图表：

![The principle](https://fsharpforfunandprofit.com/posts/monadster/monadster1.png)

但这只会产生一个身体部位。我们如何创建多个？这就是弗兰肯芬克特博士面临的挑战。

第一个问题是，我们只有有限数量的生命力。这意味着，当我们需要为第二个身体部位设置动画时，我们只能使用前一步中剩余的生命力。

我们如何将这两个步骤连接在一起，以便将第一步的生命力输入到第二步的输入中？

![Connecting steps together](https://fsharpforfunandprofit.com/posts/monadster/monadster_connect.png)

即使我们正确地链接了步骤，我们也需要将各种活体部分以某种方式组合起来。但在创造的那一刻，我们只能接触到活的身体部位。我们如何在那一瞬间将它们结合起来？

![Combining the outputs of each step](https://fsharpforfunandprofit.com/posts/monadster/monadster_combine.png)

正是弗兰肯芬克特博士的天才，导致了一种解决这两个问题的优雅方法，我现在将向大家介绍这种方法。

## 共同背景

在讨论组装身体部件的细节之前，我们应该花点时间讨论一下其余程序所需的常见功能。

首先，我们需要一个标签类型。Frankenfunctor 博士在标记所使用的每个零件的来源方面都非常自律。

```F#
type Label = string
```

我们将使用简单的记录类型对生命力进行建模：

```F#
type VitalForce = {units:int}
```

由于我们将频繁使用生命力，我们将创建一个函数，提取一个 unit 并返回 unit 和剩余力的元组。

```F#
let getVitalForce vitalForce =
   let oneUnit = {units = 1}
   let remaining = {units = vitalForce.units-1}  // decrement
   oneUnit, remaining  // return both
```

## 左腿

说完公共代码后，我们可以回到实质。

Frankenfunctor 博士的笔记本记录了下肢首先被创造出来。实验室里有一条左腿，这就是起点。

```F#
type DeadLeftLeg = DeadLeftLeg of Label
```

从这条腿上，可以创建一条带有相同标签和一个生命力单位的活腿。

```F#
type LiveLeftLeg = LiveLeftLeg of Label * VitalForce
```

因此，create 函数的类型签名看起来像这样：

```F#
type MakeLiveLeftLeg =
    DeadLeftLeg * VitalForce -> LiveLeftLeg * VitalForce
```

实际实现如下：

```F#
let makeLiveLeftLeg (deadLeftLeg,vitalForce) =
    // get the label from the dead leg using pattern matching
    let (DeadLeftLeg label) = deadLeftLeg
    // get one unit of vital force
    let oneUnit, remainingVitalForce = getVitalForce vitalForce
    // create a live leg from the label and vital force
    let liveLeftLeg = LiveLeftLeg (label,oneUnit)
    // return the leg and the remaining vital force
    liveLeftLeg, remainingVitalForce
```

如您所见，此实现与前面的图表完全匹配。

![Version 1](https://fsharpforfunandprofit.com/posts/monadster/monadster1.png)

在这一点上，弗兰肯芬克特博士有两个重要的见解。

第一个见解是，由于currying，函数可以从接受元组的函数转换为双参数函数，每个参数依次传递。

![Version 2](https://fsharpforfunandprofit.com/posts/monadster/monadster2.png)

代码现在看起来像这样：

```F#
type MakeLiveLeftLeg =
    DeadLeftLeg -> VitalForce -> LiveLeftLeg * VitalForce

let makeLiveLeftLeg deadLeftLeg vitalForce =
    let (DeadLeftLeg label) = deadLeftLeg
    let oneUnit, remainingVitalForce = getVitalForce vitalForce
    let liveLeftLeg = LiveLeftLeg (label,oneUnit)
    liveLeftLeg, remainingVitalForce
```

第二个见解是，同样的代码可以被解释为一个函数，该函数反过来返回一个“becomeAlive”函数。

也就是说，我们手头有死部分，但直到最后一刻我们才会有任何生命力，所以为什么不现在处理死部分，并返回一个在生命力可用时可以使用的函数呢。

换句话说，我们传入一个死部分，并返回一个函数，该函数在给定一些生命力时创建一个活部分。

![Version 3](https://fsharpforfunandprofit.com/posts/monadster/monadster3.png)

这些“活起来”的功能可以被视为“配方中的步骤”，假设我们能找到某种方法将它们结合起来。

代码现在看起来像这样：

```F#
type MakeLiveLeftLeg =
    DeadLeftLeg -> (VitalForce -> LiveLeftLeg * VitalForce)

let makeLiveLeftLeg deadLeftLeg =
    // create an inner intermediate function
    let becomeAlive vitalForce =
        let (DeadLeftLeg label) = deadLeftLeg
        let oneUnit, remainingVitalForce = getVitalForce vitalForce
        let liveLeftLeg = LiveLeftLeg (label,oneUnit)
        liveLeftLeg, remainingVitalForce
    // return it
    becomeAlive
```

这可能并不明显，但这与之前的版本完全相同，只是编写方式略有不同。

这个 curried 函数（有两个参数）可以被解释为一个普通的双参数函数，也可以被解释成一个返回另一个单参数函数的单参数函数。

如果这不清楚，请考虑一个更简单的双参数加法函数示例：

```F#
let add x y =
    x + y
```

因为 F# 默认情况下是 curries 函数，所以该实现与此完全相同：

```F#
let add x =
    fun y -> x + y
```

如果我们定义一个中间函数，它也与这个函数完全相同：

```F#
let add x =
    let addX y = x + y
    addX // return the function
```

### 创建 Monadster 类型

展望未来，我们可以看到，我们可以对创建活体器官的所有功能使用类似的方法。

所有这些函数都将返回一个具有以下签名的函数：`VitalForce -> LiveBodyPart * VitalForce`。

为了让我们的生活更轻松，让我们给这个函数签名一个名字 `M`，它代表“Monadster 部件生成器”，并给它一个泛型类型参数“`'LiveBodyPart`”，这样我们就可以在许多不同的身体部位使用它。

```F#
type M<'LiveBodyPart> =
    VitalForce -> 'LiveBodyPart * VitalForce
```

我们现在可以显式地用 `:M<LiveLeftLeg>` 注释 `makeLiveLeftLeg` 函数的返回类型。

```F#
let makeLiveLeftLeg deadLeftLeg :M<LiveLeftLeg> =
    let becomeAlive vitalForce =
        let (DeadLeftLeg label) = deadLeftLeg
        let oneUnit, remainingVitalForce = getVitalForce vitalForce
        let liveLeftLeg = LiveLeftLeg (label,oneUnit)
        liveLeftLeg, remainingVitalForce
    becomeAlive
```

函数的其余部分保持不变，因为 `becomeAlive` 返回值已经与 `M<LiveLeftLeg>` 兼容。

但我不喜欢一直都要明确地注释。我们把函数包装在一个单案例联合中，称之为“M”，给它自己的不同类型，怎么样？这样地：

```F#
type M<'LiveBodyPart> =
    M of (VitalForce -> 'LiveBodyPart * VitalForce)
```

这样，我们就可以区分“Monadster 部分生成器”和返回元组的普通函数。

要使用这个新定义，我们需要调整代码，以便在返回中间函数时将其包装在单一例子联合 `M` 中，如下所示：

```F#
let makeLiveLeftLegM deadLeftLeg  =
    let becomeAlive vitalForce =
        let (DeadLeftLeg label) = deadLeftLeg
        let oneUnit, remainingVitalForce = getVitalForce vitalForce
        let liveLeftLeg = LiveLeftLeg (label,oneUnit)
        liveLeftLeg, remainingVitalForce
    // changed!
    M becomeAlive // wrap the function in a single case union
```

对于最后一个版本，类型签名将被正确推断，而无需显式指定：一个接受死左腿并返回活腿的“M”的函数：

```F#
val makeLiveLeftLegM : DeadLeftLeg -> M<LiveLeftLeg>
```

请注意，我已经重命名了 `makeLiveLeftLegM` 函数，以明确它返回 `LiveLeftLeg` 的 `M`。

### M 的含义

那么，这个“M”型到底是什么意思呢？我们如何理解它？

一个有用的方法是将 `M<T>` 视为创建 `T` 的配方。你给我一些生命力，我会还给你一个 `T`。

但是 `M<T>` 如何从无中创建 `T` 呢？

这就是 `makeLiveLeftLegM` 等函数至关重要的地方。他们获取一个参数并将其“烘焙”到结果中。因此，您将看到许多具有类似签名的“M-making”函数，它们看起来都像这样：

![img](https://fsharpforfunandprofit.com/posts/monadster/monadster5.png)

或者用代码术语来说：

```F#
DeadPart -> M<LivePart>
```

现在的挑战将是如何以优雅的方式将这些结合起来。

### 测试左腿

好吧，让我们测试一下到目前为止我们所掌握的。

我们将首先创建一个死腿，并在其上使用 `makeLiveLeftLegM` 来获得一个 `M<LiveLeftLeg>`。

```F#
let deadLeftLeg = DeadLeftLeg "Boris"
let leftLegM = makeLiveLeftLegM deadLeftLeg
```

什么是 `leftLegM`？这是一个创造活体左腿的配方，只要有一些生命力。

有用的是，我们可以在闪电袭击之前预先创建这个食谱。

现在，让我们假设风暴已经到来，闪电已经击中，现在有 10 个单位的生命力可用：

```F#
let vf = {units = 10}
```

现在，在 `leftLegM` 内有一个函数，我们可以将其应用于生命力。但首先，我们需要使用模式匹配从包装器中获取函数。

```F#
let (M innerFn) = leftLegM
```

然后我们可以运行内部函数来获得左腿和剩余的生命力：

```F#
let liveLeftLeg, remainingAfterLeftLeg = innerFn vf
```

结果如下：

```F#
val liveLeftLeg : LiveLeftLeg =
   LiveLeftLeg ("Boris",{units = 1;})
val remainingAfterLeftLeg : VitalForce =
   {units = 9;}
```

您可以看到 `LiveLeftLeg` 已成功创建，剩余的生命力现在减少到 9 个单位。

这种模式匹配很尴尬，所以让我们创建一个辅助函数，它一次打开内部函数并调用它。

我们称之为 `runM`，它看起来像这样：

```F#
let runM (M f) vitalForce = f vitalForce
```

因此，上面的测试代码现在可以简化为：

```F#
let liveLeftLeg, remainingAfterLeftLeg = runM leftLegM vf
```

所以现在，我们终于有了一个可以创建活体左腿的函数。

它花了一段时间才开始工作，但我们也建立了一些有用的工具和概念，可以用来向前发展。

## 右脚

既然我们知道我们在做什么，我们现在应该能够对其他身体部位使用相同的技术。

那右腿怎么样？

不幸的是，根据笔记本，弗兰肯芬克特博士在实验室里找不到右腿。这个问题已经通过 hack 解决了……但我们稍后会讲到。

## 左臂

接下来，从左臂开始创建手臂。

但有一个问题。实验室里只有一只左臂骨折。手臂必须先愈合，才能用于最后的身体。

现在，作为一名医生，弗兰肯医生确实知道如何治愈骨折的手臂，但只有在它是活的情况下。试图治愈一条死了的断臂是不可能的。

在代码方面，我们有：

```F#
type DeadLeftBrokenArm = DeadLeftBrokenArm of Label

// A live version of the broken arm.
type LiveLeftBrokenArm = LiveLeftBrokenArm of Label * VitalForce

// A live version of a heathly arm, with no dead version available
type LiveLeftArm = LiveLeftArm of Label * VitalForce

// An operation that can turn a broken left arm into a heathly left arm
type HealBrokenArm = LiveLeftBrokenArm -> LiveLeftArm
```

因此，我们面临的挑战是：如何用手头的材料制作一个活的左臂？

首先，我们必须排除从 `DeadLeftUnbrokenArm` 创建 `LiveLeftArm` 的可能性，因为没有这样的东西。我们也不能直接将 `DeadLeftBrokenArm` 转换为健康的 `LiveLeftArm`。

![Map dead to dead](https://fsharpforfunandprofit.com/posts/monadster/monadster_map1.png)

但我们能做的就是把 `DeadLeftBrokenArm` 变成活的断臂，然后治愈活的断手，对吧？

![Can&rsquo;t create live broken arm directly](https://fsharpforfunandprofit.com/posts/monadster/monadster_map2.png)

不，恐怕这行不通。我们不能直接创建活部件，我们只能在 `M` 配方的上下文中创建活部件。

然后，我们需要做的是创建一个特殊版本的 `healBrokenArm`（称之为 `healBrokekArmM`），将 `M<LiveBrokenArm>` 转换为 `M<LiveArm>`。

![Can&rsquo;t create live broken arm directly](https://fsharpforfunandprofit.com/posts/monadster/monadster_map3.png)

但是我们如何创建这样一个函数呢？我们如何将 `healBrokenArm` 作为其一部分进行重用？

让我们从最简单的实现开始。

首先，由于该函数将返回 `M` something，因此其形式与我们之前看到的 `makeLiveLeftLegM` 函数相同。我们需要创建一个具有 vitalForce 参数的内部函数，然后将其包裹在M中返回。

但与我们之前看到的函数不同，这个函数也有一个 `M` 作为参数（`M<LiveBrokenArm>`）。我们如何从该输入中提取所需的数据？

很简单，只要用一些生命力来运行它。那么，我们从哪里获得生命力呢？从参数到内部函数！

所以我们的最终版本看起来像这样：

```F#
// implementation of HealBrokenArm
let healBrokenArm (LiveLeftBrokenArm (label,vf)) = LiveLeftArm (label,vf)

/// convert a M<LiveLeftBrokenArm> into a M<LiveLeftArm>
let makeHealedLeftArm brokenArmM =

    // create a new inner function that takes a vitalForce parameter
    let healWhileAlive vitalForce =
        // run the incoming brokenArmM with the vitalForce
        // to get a broken arm
        let brokenArm,remainingVitalForce = runM brokenArmM vitalForce

        // heal the broken arm
        let healedArm = healBrokenArm brokenArm

        // return the healed arm and the remaining VitalForce
        healedArm, remainingVitalForce

    // wrap the inner function and return it
    M healWhileAlive
```

如果我们评估这段代码，我们会得到签名：

```F#
val makeHealedLeftArm : M<LiveLeftBrokenArm> -> M<LiveLeftArm>
```

这正是我们想要的！

但不要那么快，我们可以做得更好。

我们已经在其中硬编码了 `healBrokenArm` 转换。如果我们想对身体的其他部位进行其他转换，会发生什么？我们能让这个函数更通用一点吗？

是的，这很容易。我们只需要传入一个函数（叫 “f”）来转换身体部位，如下所示：

```F#
let makeGenericTransform f brokenArmM =

    // create a new inner function that takes a vitalForce parameter
    let healWhileAlive vitalForce =
        let brokenArm,remainingVitalForce = runM brokenArmM vitalForce

        // heal the broken arm using passed in f
        let healedArm = f brokenArm
        healedArm, remainingVitalForce

    M healWhileAlive
```

令人惊奇的是，通过用 `f` 参数参数化一个转换，整个函数就变得通用了！

我们没有做任何其他更改，但 `makeGenericTransform` 的签名不再指手臂。它适用于任何东西！

```F#
val makeGenericTransform : f:('a -> 'b) -> M<'a> -> M<'b>
```

### 介绍 mapM

由于它现在如此通用，所以名字很混乱。让我们重命名它。我称之为 `mapM`。它适用于任何身体部位和任何变形。

这是实现，内部名称也已固定。

```F#
let mapM f bodyPartM =
    let transformWhileAlive vitalForce =
        let bodyPart,remainingVitalForce = runM bodyPartM vitalForce
        let updatedBodyPart = f bodyPart
        updatedBodyPart, remainingVitalForce
    M transformWhileAlive
```

特别是，它与 `healBrokenArm` 函数配合使用，因此要创建一个已提升到与 `M`s 配合使用的“heal”版本，我们可以这样写：

```F#
let healBrokenArmM = mapM healBrokenArm
```

![mapM with heal](https://fsharpforfunandprofit.com/posts/monadster/monadster_map4.png)

### mapM 的重要性

对 `mapM` 的一种思考方式是，它是一个“函数转换器”。给定任何“正常”函数，它都会将其转换为输入和输出为 `M`s的函数。

![mapM](https://fsharpforfunandprofit.com/posts/monadster/monadster_mapm.png)

在许多情况下都会出现类似于 `mapM` 的函数。例如，`Option.map` 将“普通”函数转换为输入和输出都是选项的函数。同样，`List.map` 将一个“普通”函数转换为一个输入和输出都是列表的函数。还有许多其他的例子。

```F#
// map works with options
let healBrokenArmO = Option.map healBrokenArm
// LiveLeftBrokenArm option -> LiveLeftArm option

// map works with lists
let healBrokenArmL = List.map healBrokenArm
// LiveLeftBrokenArm list -> LiveLeftArm list
```

对您来说可能是新的，“包装器”类型 `M` 包含一个函数，而不是像 Option 或 List 这样的简单数据结构。这可能会让你头疼！

此外，上图暗示 `M` 可以包装任何正常（normal）类型，`mapM` 可以映射任何正常函数。

让我们试试看！

```F#
let isEven x = (x%2 = 0)   // int -> bool
// map it
let isEvenM = mapM isEven  // M<int> -> M<bool>

let isEmpty x = (String.length x)=0  // string -> bool
// map it
let isEmptyM = mapM isEmpty          // M<string> -> M<bool>
```

所以，是的，它奏效了！

### 测试左臂

让我们再次测试一下到目前为止我们所拥有的。

我们将首先创建一个断臂，并在其上使用 `makeLiveLeftBrokenArm` 来获得一个 `M<BrokenLeftArm>`。

```F#
let makeLiveLeftBrokenArm deadLeftBrokenArm =
    let (DeadLeftBrokenArm label) = deadLeftBrokenArm
    let becomeAlive vitalForce =
        let oneUnit, remainingVitalForce = getVitalForce vitalForce
        let liveLeftBrokenArm = LiveLeftBrokenArm (label,oneUnit)
        liveLeftBrokenArm, remainingVitalForce
    M becomeAlive

/// create a dead Left Broken Arm
let deadLeftBrokenArm = DeadLeftBrokenArm "Victor"

/// create a M<BrokenLeftArm> from the dead one
let leftBrokenArmM = makeLiveLeftBrokenArm deadLeftBrokenArm
```

现在我们可以使用 `mapM` 和 `healBrokenArm` 将 `M<BrokenLeftAm>` 转换为 `M<LeftArm>`：

```F#
let leftArmM = leftBrokenArmM |> mapM healBrokenArm
```

我们现在在 `leftArmM` 中拥有的是创造一个完整而生动的左臂的配方。我们所要做的就是增加一些生命力。

和以前一样，我们可以在闪电袭击之前提前做所有这些事情。

现在，当风暴来临，闪电击中，生命力可用时，我们可以用生命力运行 `leftArmM`…

```F#
let vf = {units = 10}

let liveLeftArm, remainingAfterLeftArm = runM leftArmM vf
```

…我们得到这个结果：

```F#
val liveLeftArm : LiveLeftArm =
    LiveLeftArm ("Victor",{units = 1;})
val remainingAfterLeftArm :
    VitalForce = {units = 9;}
```

一只活生生的左臂，正如我们所希望的那样。

## 右臂

接下来是右臂。

再次出现了一个问题。Frankenfunctor 博士的笔记本记录说，没有整只手臂可用。然而，有一只下臂和一只上臂…

```F#
type DeadRightLowerArm = DeadRightLowerArm of Label
type DeadRightUpperArm = DeadRightUpperArm of Label
```

…它们可以变成相应的活的：

```F#
type LiveRightLowerArm = LiveRightLowerArm of Label * VitalForce
type LiveRightUpperArm = LiveRightUpperArm of Label * VitalForce
```

弗兰肯医生决定做手术，将两个手臂部分连接成一个完整的手臂。

```F#
// define the whole arm
type LiveRightArm = {
    lowerArm : LiveRightLowerArm
    upperArm : LiveRightUpperArm
    }

// surgery to combine the two arm parts
let armSurgery lowerArm upperArm =
    {lowerArm=lowerArm; upperArm=upperArm}
```

与手臂骨折一样，手术只能用带电部件进行。用死零件做这件事会很恶心。

但是，与手臂骨折一样，我们不能直接访问活动部件，只能在 `M` 包装器的上下文中访问。

换句话说，我们需要将适用于正常活部件的 `armSurgery` 函数转换为适用于 `M`s 的 `armSurgeryM` 函数。

![armsurgeryM](https://fsharpforfunandprofit.com/posts/monadster/monadster_armsurgeryM.png)

我们可以使用与以前相同的方法：

- 创建一个接受 vitalForce 参数的内部函数
- 使用 vitalForce 运行传入参数以提取数据
- 从内部函数返回手术后的新数据
- 将内部函数包裹在“M”中并返回

代码如下：

```F#
/// convert a M<LiveRightLowerArm> and  M<LiveRightUpperArm> into a M<LiveRightArm>
let makeArmSurgeryM_v1 lowerArmM upperArmM =

    // create a new inner function that takes a vitalForce parameter
    let becomeAlive vitalForce =
        // run the incoming lowerArmM with the vitalForce
        // to get the lower arm
        let liveLowerArm,remainingVitalForce = runM lowerArmM vitalForce

        // run the incoming upperArmM with the remainingVitalForce
        // to get the upper arm
        let liveUpperArm,remainingVitalForce2 = runM upperArmM remainingVitalForce

        // do the surgery to create a liveRightArm
        let liveRightArm = armSurgery liveLowerArm liveUpperArm

        // return the whole arm and the SECOND remaining VitalForce
        liveRightArm, remainingVitalForce2

    // wrap the inner function and return it
    M becomeAlive
```

当然，与断臂示例的一个很大区别是，我们有两个参数。当我们运行第二个参数（以获取 `liveUpperArm`）时，我们必须确保在第一步之后传递剩余的生命力，而不是原始的生命力。

然后，当我们从内部函数返回时，我们必须确保返回 `remainingVitalForce2`（第二步后的剩余部分），而不是其他任何一个。

如果我们编译这段代码，我们得到：

```F#
M<LiveRightLowerArm> -> M<LiveRightUpperArm> -> M<LiveRightArm>
```

这正是我们正在寻找的签名。

### 介绍 map2M

但和以前一样，为什么不让它更通用呢？我们不需要硬编码 `armSurgery`——我们可以将其作为参数传递。

我们将调用更通用的函数 `map2M`——就像 `mapM` 一样，但有两个参数。

下面是实现：

```F#
let map2M f m1 m2 =
    let becomeAlive vitalForce =
        let v1,remainingVitalForce = runM m1 vitalForce
        let v2,remainingVitalForce2 = runM m2 remainingVitalForce
        let v3 = f v1 v2
        v3, remainingVitalForce2
    M becomeAlive
```

它有签名：

```F#
f:('a -> 'b -> 'c) -> M<'a> -> M<'b> -> M<'c>
```

正如使用 `mapM` 一样，我们可以将此函数解释为“函数转换器”，将“正常”的双参数函数转换为 `M` 世界中的函数。

![map2M](https://fsharpforfunandprofit.com/posts/monadster/monadster_map2m.png)

### 测试右臂

让我们再次测试一下到目前为止我们所拥有的。

与往常一样，我们需要一些函数将死部分转换为活部分。

```F#
let makeLiveRightLowerArm (DeadRightLowerArm label) =
    let becomeAlive vitalForce =
        let oneUnit, remainingVitalForce = getVitalForce vitalForce
        let liveRightLowerArm = LiveRightLowerArm (label,oneUnit)
        liveRightLowerArm, remainingVitalForce
    M becomeAlive

let makeLiveRightUpperArm (DeadRightUpperArm label) =
    let becomeAlive vitalForce =
        let oneUnit, remainingVitalForce = getVitalForce vitalForce
        let liveRightUpperArm = LiveRightUpperArm (label,oneUnit)
        liveRightUpperArm, remainingVitalForce
    M becomeAlive
```

*顺便问一下，你注意到这些功能中有很多重复吗？我也是！我们稍后将尝试解决这个问题。*

接下来，我们将创建零件：

```F#
let deadRightLowerArm = DeadRightLowerArm "Tom"
let lowerRightArmM = makeLiveRightLowerArm deadRightLowerArm

let deadRightUpperArm = DeadRightUpperArm "Jerry"
let upperRightArmM = makeLiveRightUpperArm deadRightUpperArm
```

然后创建函数来制作一个完整的手臂：

```F#
let armSurgeryM  = map2M armSurgery
let rightArmM = armSurgeryM lowerRightArmM upperRightArmM
```

和往常一样，我们可以在闪电袭击之前提前做所有这些事情，建立一个配方（或者计算，如果你愿意的话），在时机成熟时可以做我们需要的一切。

当生命力可用时，我们可以用生命力运行 `rightArmM`…

```F#
let vf = {units = 10}

let liveRightArm, remainingFromRightArm = runM rightArmM vf
```

…我们得到这个结果：

```F#
val liveRightArm : LiveRightArm =
    {lowerArm = LiveRightLowerArm ("Tom",{units = 1;});
     upperArm = LiveRightUpperArm ("Jerry",{units = 1;});}

val remainingFromRightArm : VitalForce =
    {units = 8;}
```

根据需要，由两个子组件组成的活动右臂。

另请注意，剩余的生命力量已降至 8。我们正确地消耗了两个单位的生命力。

## 摘要

在这篇文章中，我们看到了如何创建一个 `M` 类型，该类型包含一个“变活”功能，该功能只能在雷击时激活。

我们还看到了如何使用 `mapM`（用于断开的手臂）和 `map2M`（用于分为两部分的手臂）来处理和组合各种 M值。

*本文中使用的代码示例可以在 GitHub 上找到。*

## 下次

这个激动人心的故事会给你带来更多的震撼！请继续关注下一期，届时我将揭示头部和身体是如何创建的。

# 2 完成Monadster的主体

*Part of the "Handling State" series (*[link](https://fsharpforfunandprofit.com/posts/monadster-2/#series-toc)*)*

弗兰肯福克托博士与莫纳德斯特，第二部分
08七月2015这篇文章已经超过3岁了

https://fsharpforfunandprofit.com/posts/monadster-2/

*更新：我关于这个话题的演讲幻灯片和视频*

*警告！这篇文章包含可怕的话题、紧张的类比、对单子的讨论*

欢迎来到弗兰肯芬克特博士和莫纳德斯特的扣人心弦的故事！

我们在上一期中看到了弗兰肯芬克特博士是如何使用“Monadster 零件生成器”（简称“M”）从尸体零件中创造生命的，在获得某种生命力后，它会返回一个活体零件。

我们还看到了该生物的腿和手臂是如何创建的，以及如何使用 `mapM`（用于断开的手臂）和 `map2M`（用于分为两部分的手臂）来处理和组合这些 M 值。

在第二部分中，我们将介绍弗兰肯芬克特博士用于创建头部、心脏和完整身体的其他技术。

## 头部

首先，头部。

就像右臂一样，头部由两部分组成，一个大脑和一个头骨。

Frankenfunctor博士首先定义了死亡的大脑和头骨：

```F#
type DeadBrain = DeadBrain of Label
type Skull = Skull of Label
```

与由两部分组成的右臂不同，只有大脑需要活下来。头骨可以按原样使用，在用于活体头部之前不需要变形。

```F#
type LiveBrain = LiveBrain of Label * VitalForce

type LiveHead = {
    brain : LiveBrain
    skull : Skull // not live
    }
```

活体大脑与头骨结合，使用 `headSurgery` 函数制成活体头部，类似于我们之前的 `armSurgery`。

```F#
let headSurgery brain skull =
    {brain=brain; skull=skull}
```

现在，我们已经准备好创建一个活的头部了，但我们应该如何做到呢？

如果我们能重用 `map2M`，那就太好了，但有一个问题——要想让 `map2M` 工作，它需要一个包裹在 `M` 中的头骨。

![head](https://fsharpforfunandprofit.com/posts/monadster-2/monadster_head1.png)

但是头骨不需要变得活着或使用生命力，所以我们需要创建一个特殊的函数，将 `Skull` 转换为 `M<Skull>`。

我们可以使用与以前相同的方法：

- 创建一个接受 vitalForce 参数的内部函数
- 在这种情况下，我们保持生命力不变
- 从内部功能返回原始头骨和未触及的生命力
- 将内部函数包裹在“M”中并返回

代码如下：

```F#
let wrapSkullInM skull =
    let becomeAlive vitalForce =
        skull, vitalForce
    M becomeAlive
```

但是 `wrapSkullInM` 的签名非常有趣。

```F#
val wrapSkullInM : 'a -> M<'a>
```

任何地方都没有提到头骨！

### 介绍 returnM

我们创建了一个完全泛型的函数，可以将任何东西转换为 `M`。所以让我们重命名它。我将称之为 `returnM`，但在其他情况下，它可能被称为 `pure` 或 `unit`。

```F#
let returnM x =
    let becomeAlive vitalForce =
        x, vitalForce
    M becomeAlive
```

### 测试头部

让我们付诸行动。

首先，我们需要定义如何创建一个活的大脑。

```F#
let makeLiveBrain (DeadBrain label) =
    let becomeAlive vitalForce =
        let oneUnit, remainingVitalForce = getVitalForce vitalForce
        let liveBrain = LiveBrain (label,oneUnit)
        liveBrain, remainingVitalForce
    M becomeAlive
```

接下来，我们得到一个死去的大脑和头骨：

```F#
let deadBrain = DeadBrain "Abby Normal"
let skull = Skull "Yorick"
```

*顺便说一句，这个特殊的死脑是如何获得的，这是一个有趣的故事，我现在没有时间讲。*

![abnormal brain](https://fsharpforfunandprofit.com/posts/monadster-2/monadster_brain.jpg)

接下来，我们从死部分构建“M”版本：

```F#
let liveBrainM = makeLiveBrain deadBrain
let skullM = returnM skull
```

并使用 `map2M` 组合这些部分：

```F#
let headSurgeryM = map2M headSurgery
let headM = headSurgeryM liveBrainM skullM
```

再一次，我们可以在闪电袭击之前提前做所有这些事情。

当生命力可用时，我们可以用生命力运行 `headM`…

```F#
let vf = {units = 10}

let liveHead, remainingFromHead = runM headM vf
```

…我们得到这个结果：

```F#
val liveHead : LiveHead =
    {brain = LiveBrain ("Abby normal",{units = 1;});
    skull = Skull "Yorick";}

val remainingFromHead : VitalForce =
    {units = 9;}
```

根据需要，由两个子组件组成的活头。

还要注意，剩余的生命力只有 9，因为头骨没有消耗任何单位。

## 跳动的心

我们还需要一个组件，那就是心脏。

首先，我们有一颗死心和一颗活心，它们以通常的方式定义：

```F#
type DeadHeart = DeadHeart of Label
type LiveHeart = LiveHeart of Label * VitalForce
```

但这种生物需要的不仅仅是一颗活着的心——它需要一颗跳动的心。一颗跳动的心脏是由一颗活的心脏和一些生命力量组成的，就像这样：

```F#
type BeatingHeart = BeatingHeart of LiveHeart * VitalForce
```

创建活体心脏的代码与前面的示例非常相似：

```F#
let makeLiveHeart (DeadHeart label) =
    let becomeAlive vitalForce =
        let oneUnit, remainingVitalForce = getVitalForce vitalForce
        let liveHeart = LiveHeart (label,oneUnit)
        liveHeart, remainingVitalForce
    M becomeAlive
```

创建跳动心脏的代码也非常相似。它以活体心脏为参数，消耗另一个单位的生命力，并返回跳动的心脏和剩余的生命力。

```F#
let makeBeatingHeart liveHeart =

    let becomeAlive vitalForce =
        let oneUnit, remainingVitalForce = getVitalForce vitalForce
        let beatingHeart = BeatingHeart (liveHeart, oneUnit)
        beatingHeart, remainingVitalForce
    M becomeAlive
```

如果我们看看这些函数的签名，我们会发现它们非常相似；这两种形式都是 `Something -> M<Something Else>`。

```F#
val makeLiveHeart : DeadHeart -> M<LiveHeart>
val makeBeatingHeart : LiveHeart -> M<BeatingHeart>
```

### 将返回 M 的函数链接在一起

我们从一颗死去的心脏开始，我们需要一个跳动的热量

![heart1](https://fsharpforfunandprofit.com/posts/monadster-2/monadster_heart1.png)

但我们没有直接做到这一点的工具。

我们有一个函数可以将 `DeadHeart` 转换为 `M<LiveHeart>`，也有一个功能可以将 `LiveHeart` 转换为 `M<BeatingHeart>`。

但是第一个的输出与第二个的输入不兼容，所以我们不能把它们粘在一起。

![heart2](https://fsharpforfunandprofit.com/posts/monadster-2/monadster_heart2.png)

那么，我们想要的是一个函数，给定 `M<LiveHeart>` 作为输入，可以将其转换为 `M<BeatingHeart>`。

此外，我们希望基于已有的 `makeBeatingHeart` 函数来构建它。

![heart2](https://fsharpforfunandprofit.com/posts/monadster-2/monadster_heart3.png)

这是第一次尝试，使用我们以前多次使用的相同模式：

```F#
let makeBeatingHeartFromLiveHeartM liveHeartM =

    let becomeAlive vitalForce =
        // extract the liveHeart from liveHeartM
        let liveHeart, remainingVitalForce = runM liveHeartM vitalForce

        // use the liveHeart to create a beatingHeartM
        let beatingHeartM = makeBeatingHeart liveHeart

        // what goes here?

        // return a beatingHeart and remaining vital force
        beatingHeart, remainingVitalForce

    M becomeAlive
```

但是中间是什么呢？我们如何从 `beatingHeartM` 中获得一颗跳动的心脏？答案是用一些生命力量来运行它（我们碰巧手头有，因为我们正处于 `becomeAlive` 函数的中间）。

什么是生命力？它应该是获得 `liveHeart` 后剩下的生命力。

最终版本看起来像这样：

```F#
let makeBeatingHeartFromLiveHeartM liveHeartM =

    let becomeAlive vitalForce =
        // extract the liveHeart from liveHeartM
        let liveHeart, remainingVitalForce = runM liveHeartM vitalForce

        // use the liveHeart to create a beatingHeartM
        let beatingHeartM = makeBeatingHeart liveHeart

        // run beatingHeartM to get a beatingHeart
        let beatingHeart, remainingVitalForce2 = runM beatingHeartM remainingVitalForce

        // return a beatingHeart and remaining vital force
        beatingHeart, remainingVitalForce2

    // wrap the inner function and return it
    M becomeAlive
```

请注意，我们在最后返回 `remainingVitalForce2`，这是两个步骤运行后的剩余部分。

如果我们看看这个函数的签名，它是：

```F#
M<LiveHeart> -> M<BeatingHeart>
```

这正是我们想要的！

### 介绍 bindM

再次，我们可以通过传递函数参数而不是硬编码 `makeBeatingHeart` 来使此函数通用。

我称之为 `bindM`。代码如下：

```F#
let bindM f bodyPartM =
    let becomeAlive vitalForce =
        let bodyPart, remainingVitalForce = runM bodyPartM vitalForce
        let newBodyPartM = f bodyPart
        let newBodyPart, remainingVitalForce2 = runM newBodyPartM remainingVitalForce
        newBodyPart, remainingVitalForce2
    M becomeAlive
```

签名为：

```F#
f:('a -> M<'b>) -> M<'a> -> M<'b>
```

换句话说，给定任何函数 `Something -> M<SomethingElse>`，我都可以将其转换为一个函数 `M<Something> -> M<SomethingElse>`，该函数具有 `M` 作为输入和输出。

顺便说一句，具有 `Something -> M<SomethingElse>` 签名的函数通常被称为*单子（monadic）*函数。

不管怎样，一旦你了解了 `bindM` 中的情况，就可以实现一个稍微短一些的版本，如下所示：

```F#
let bindM f bodyPartM =
    let becomeAlive vitalForce =
        let bodyPart, remainingVitalForce = runM bodyPartM vitalForce
        runM (f bodyPart) remainingVitalForce
    M becomeAlive
```

所以最后，我们有一种方法来创建一个函数，给定一个 `DeadHeart`，创建一个 `M<BeatingHeart>`。

![heart3](https://fsharpforfunandprofit.com/posts/monadster-2/monadster_heart4.png)

代码如下：

```F#
// create a dead heart
let deadHeart = DeadHeart "Anne"

// create a live heart generator (M<LiveHeart>)
let liveHeartM = makeLiveHeart deadHeart

// create a beating heart generator (M<BeatingHeart>)
// from liveHeartM and the makeBeatingHeart function
let beatingHeartM = bindM makeBeatingHeart liveHeartM
```

其中有很多中间值，可以通过使用管道使其更简单，如下所示：

```F#
let beatingHeartM =
   DeadHeart "Anne"
   |> makeLiveHeart
   |> bindM makeBeatingHeart
```

### bind 的重要性

对 `bindM` 的一种思考方式是，它是另一个“函数转换器”，就像 `mapM` 一样。也就是说，给定任何“M-返回”函数，它都会将其转换为输入和输出都是 `M`s 的函数。

![bindM](https://fsharpforfunandprofit.com/posts/monadster-2/monadster_bindm.png)

就像 `map` 一样，`bind` 也出现在许多其他上下文中。

例如，`Option.bind` 将选项生成函数（`'a->'b option`）转换为输入和输出都是选项的函数。同样，`List.bind` 将列表生成函数（`'a->'b list`）转换为输入和输出都是列表的函数。

在我关于函数错误处理的演讲中，我详细讨论了 bind 的另一个版本。

绑定之所以如此重要，是因为“M-返回”函数经常出现，而且它们不能很容易地链接在一起，因为一步的输出和下一步的输入不匹配。

通过使用 `bindM`，我们可以将每个步骤转换为一个函数，其中输入和输出都是 `M`s，然后可以将它们链接在一起。

![bindM](https://fsharpforfunandprofit.com/posts/monadster-2/monadster_bindm2.png)

### 测试跳动的心脏

一如既往，我们提前构建配方，在这种情况下，是为了制作一颗 `BeatingHeart`。

```F#
let beatingHeartM =
    DeadHeart "Anne"
    |> makeLiveHeart
    |> bindM makeBeatingHeart
```

当生命力可用时，我们可以用生命力运行 `beatingHeartM`…

```F#
let vf = {units = 10}

let beatingHeart, remainingFromHeart = runM beatingHeartM vf
```

…我们得到这个结果：

```F#
val beatingHeart : BeatingHeart =
    BeatingHeart (LiveHeart ("Anne",{units = 1;}),{units = 1;})

val remainingFromHeart : VitalForce =
    {units = 8;}
```

请注意，剩余的生命力是八个单位，因为我们做两步就用了两个单位。

## 全身

最后，我们有了组装一个完整身体所需的所有部件。

以下是 Frankenfunctor 博士对活体的定义：

```F#
type LiveBody = {
    leftLeg: LiveLeftLeg
    rightLeg : LiveLeftLeg
    leftArm : LiveLeftArm
    rightArm : LiveRightArm
    head : LiveHead
    heart : BeatingHeart
    }
```

你可以看到，它使用了我们已经开发的所有子组件。

### 两条左腿

由于没有右腿可用，弗兰肯医生决定抄近路，在体内使用两条左腿，希望没有人会注意到。

其结果是，该生物有两只左脚，这并不总是一种障碍，事实上，该生物不仅克服了这一缺点，而且成为了一名值得信赖的舞者，正如这段罕见的镜头所示：

### 组装子部件

`LiveBody` 类型有六个字段。我们如何从我们拥有的各种 `M<BodyPart>` 构建它？

一种方法是重复我们使用 `mapM` 和 `map2M` 的技术。我们可以创建一个 `map3M` 和 `map4M` 等等。

例如，`map3M` 可以这样定义：

```F#
let map3M f m1 m2 m3 =
    let becomeAlive vitalForce =
        let v1,remainingVitalForce = runM m1 vitalForce
        let v2,remainingVitalForce2 = runM m2 remainingVitalForce
        let v3,remainingVitalForce3 = runM m3 remainingVitalForce2
        let v4 = f v1 v2 v3
        v4, remainingVitalForce3
    M becomeAlive
```

但这很快就会变得乏味。有更好的办法吗？

为什么，是的，有！

要理解它，请记住，像 `LiveBody` 这样的记录类型必须全部构建或完全不构建，但由于 currying 和部分应用程序的魔力，函数可以一步一步地组装。

因此，如果我们有一个创建 `LiveBody` 的六参数函数，如下所示：

```F#
val createBody :
    leftLeg:LiveLeftLeg ->
    rightLeg:LiveLeftLeg ->
    leftArm:LiveLeftArm ->
    rightArm:LiveRightArm ->
    head:LiveHead ->
    beatingHeart:BeatingHeart ->
    LiveBody
```

我们实际上可以将其视为一个单参数函数，它返回一个五参数函数，如下所示：

```F#
val createBody :
    leftLeg:LiveLeftLeg -> (five param function)
```

然后当我们将该函数应用于第一个参数（“left Leg”）时，我们得到了那个五参数函数：

```F#
(six param function) apply (first parameter) returns (five param function)
```

其中五参数函数具有签名：

```F#
    rightLeg:LiveLeftLeg ->
    leftArm:LiveLeftArm ->
    rightArm:LiveRightArm ->
    head:LiveHead ->
    beatingHeart:BeatingHeart ->
    LiveBody
```

这个五参数函数可以看作是一个单参数函数，它返回一个四参数函数：

```F#
    rightLeg:LiveLeftLeg -> (four parameter function)
```

同样，我们可以应用第一个参数（“rightLeg”）并返回四参数函数：

```F#
(five param function) apply (first parameter) returns (four param function)
```

其中四参数函数具有签名：

```F#
    leftArm:LiveLeftArm ->
    rightArm:LiveRightArm ->
    head:LiveHead ->
    beatingHeart:BeatingHeart ->
    LiveBody
```

以此类推，直到最终我们得到一个只有一个参数的函数。该函数将具有 `BeatingHeart -> LiveBody` 的签名。

当我们应用最后一个参数（“beatingHeart”）时，我们就会得到完整的 `LiveBody`。

我们也可以把这个技巧用于 M-things！

我们从包裹在 M 中的六参数函数和 `M<LiveLeftLeg>` 参数开始。

让我们假设有某种方法可以将 M 函数“应用”到 M 参数上。我们应该得到一个包裹在 `M` 中的五参数函数。

```F#
// normal version
(six param function) apply (first parameter) returns (five param function)

// M-world version
M<six param function> applyM M<first parameter> returns M<five param function>
```

然后再次这样做，我们可以应用下一个 M 参数

```F#
// normal version
(five param function) apply (first parameter) returns (four param function)

// M-world version
M<five param function> applyM M<first parameter> returns M<four param function>
```

以此类推，逐一应用参数，直到我们得到最终结果。

### 介绍 applyM

这个 `applyM` 函数将有两个参数，一个包裹在 M 中的函数，一个裹着 M 的参数。输出将是包裹在 M 的函数的结果。

下面是实现：

```F#
let applyM mf mx =
    let becomeAlive vitalForce =
        let f,remainingVitalForce = runM mf vitalForce
        let x,remainingVitalForce2 = runM mx remainingVitalForce
        let y = f x
        y, remainingVitalForce2
    M becomeAlive
```

正如你所看到的，它与 `map2M` 非常相似，除了“f”来自第一个参数本身的解包。

让我们试试吧！

首先，我们需要我们的六参数函数：

```F#
let createBody leftLeg rightLeg leftArm rightArm head beatingHeart =
    {
    leftLeg = leftLeg
    rightLeg = rightLeg
    leftArm = leftArm
    rightArm = rightArm
    head = head
    heart = beatingHeart
    }
```

我们需要克隆左腿，以便将其用于右腿：

```F#
let rightLegM = leftLegM
```

接下来，我们需要将这个 `createBody` 函数包装在 `M` 中。我们如何做到这一点？

当然，我们之前为头骨定义了 `returnM` 函数！

所以把它们放在一起，我们有以下代码：

```F#
// move createBody to M-world -- a six parameter function wrapped in an M
let fSixParamM = returnM createBody

// apply first M-param to get a five parameter function wrapped in an M
let fFiveParamM = applyM fSixParamM leftLegM

// apply second M-param to get a four parameter function wrapped in an M
let fFourParamM = applyM fFiveParamM rightLegM

// etc
let fThreeParamM = applyM fFourParamM leftArmM
let fTwoParamM = applyM fThreeParamM rightArmM
let fOneParamM = applyM fTwoParamM headM

// after last application, the result is a M<LiveBody>
let bodyM = applyM fOneParamM beatingHeartM
```

它奏效了！结果就是我们想要的 `M<LiveBody>`。

但这段代码确实很丑！我们能做些什么让它看起来更好？

一个技巧是将 `applyM` 转换为中缀操作，就像普通函数应用程序一样。用于此操作的运算符通常写为 `<*>`。

```F#
let (<*>) = applyM
```

有了这个，我们可以将上述代码重新连接为：

```F#
let bodyM =
    returnM createBody
    <*> leftLegM
    <*> rightLegM
    <*> leftArmM
    <*> rightArmM
    <*> headM
    <*> beatingHeartM
```

这好多了！

另一个技巧是注意 `returnM` 后面跟着 `applyM` 与 `mapM` 相同。所以，如果我们也为 `mapM` 创建一个中缀运算符…

```F#
let (<!>) = mapM
```

…我们也可以去掉 `returnM`，并编写如下代码：

```F#
let bodyM =
    createBody
    <!> leftLegM
    <*> rightLegM
    <*> leftArmM
    <*> rightArmM
    <*> headM
    <*> beatingHeartM
```

这样做的好处是，它读起来几乎就像你只是在调用原始函数（一旦你习惯了这些符号！）

### 测试全身

一如既往，我们希望提前构建配方。在这种情况下，我们已经创建了 `bodyM`，当生命力到来时，它将给我们一个完整的 `liveBody`。

现在我们所要做的就是等待闪电击中并为产生生命力的机器充电！

![Electricity in the lab](https://fsharpforfunandprofit.com/posts/monadster-2/monadster-lab-electricity.gif)
来源：Misfit Robot Daydream

它来了——生命力是可用的！很快，我们以通常的方式跑完 `bodyM`…

```F#
let vf = {units = 10}

let liveBody, remainingFromBody = runM bodyM vf
```

…我们得到这个结果：

```F#
val liveBody : LiveBody =
  {leftLeg = LiveLeftLeg ("Boris",{units = 1;});
   rightLeg = LiveLeftLeg ("Boris",{units = 1;});
   leftArm = LiveLeftArm ("Victor",{units = 1;});
   rightArm = {lowerArm = LiveRightLowerArm ("Tom",{units = 1;});
               upperArm = LiveRightUpperArm ("Jerry",{units = 1;});};
   head = {brain = LiveBrain ("Abby Normal",{units = 1;});
           skull = Skull "Yorick";};
   heart = BeatingHeart (LiveHeart ("Anne",{units = 1;}),{units = 1;});}

val remainingFromBody : VitalForce = {units = 2;}
```

它活着！我们成功地复制了弗兰肯芬克特博士的作品！

请注意，身体包含所有正确的子组件，并且剩余的生命力已正确地减少到两个单位，因为我们用了八个单位来创建身体。

## 摘要

在这篇文章中，我们扩展了我们的操作技巧，包括：

- `returnM` 取头骨
- `bindM` 代表跳动的心脏
- `applyM` 组装整个身体

*本文中使用的代码示例可以在 GitHub上找到。*

## 下次

在最后一期中，我们将重构代码并回顾所使用的所有技术。

# 3 重构 Monadster

*Part of the "Handling State" series (*[link](https://fsharpforfunandprofit.com/posts/monadster-3/#series-toc)*)*

弗兰肯福克托博士与莫纳德斯特，第三部分
2015年7月9日 这篇文章已经超过3年了

https://fsharpforfunandprofit.com/posts/monadster-3/

*更新：我关于这个话题的演讲幻灯片和视频*

*警告！这篇文章包含可怕的话题、紧张的类比、对单子的讨论*

欢迎来到弗兰肯福克托博士和莫纳德斯特引人入胜的故事的第三部分！

我们在第一期中看到了弗兰肯芬克特博士是如何使用“Monadster 零件发生器”（简称“M”）从尸体零件中创造生命的，当被提供一些生命力时，它会返回一个活体零件。

我们还看到了该生物的腿和手臂是如何创建的，以及如何使用 `mapM` 和 `map2M` 处理和组合这些 M 值。

在第二部分中，我们学习了如何使用其他强大的技术（如 `returnM`、`bindM` 和 `applyM`）构建头部、心脏和身体。

在最后一期中，我们将回顾所有使用的技术，重构代码，并将 Frankenfunctor 博士的技术与现代状态单子进行比较。

完整系列的链接：

- 第1部分-弗兰肯福克托博士和莫纳德斯特
- 第2部分-完成身体
- 第3部分-回顾和重构（本文）

## 对所用技术的回顾

在重构之前，让我们回顾一下我们使用的所有技术。

### `M<BodyPart>` 类型

在生命力可用之前，我们无法创建一个真正的活体部分，但我们想在闪电击中之前操纵它们、组合它们等。我们通过创建一个类型 M 来实现这一点，该类型为每个部分包装了一个“变活”函数。然后，我们可以将 `M<BodyPart>` 视为创建 `BodyPart` 的配方或说明。

`M` 的定义是：

```F#
type M<'a> = M of (VitalForce -> 'a * VitalForce)
```

### mapM

接下来，我们想在不使用任何生命力的情况下转换 `M` 的内容。在我们的特定情况下，我们想将断臂配方（`M<BrokenLeftArm>`）转换为未断臂配方（`M<LeftArm>`）。解决方案是实现一个函数 `mapM`，它接受一个正常的函数 `'a->'b`，并将其转换为 `M<'a> -> M<'b>` 函数。

`mapM` 的签名是：

```F#
val mapM : f:('a -> 'b) -> M<'a> -> M<'b>
```

### map2M

我们还想把两个 M 配方结合起来做一个新的。在那个特定的情况下，它将上臂（`M<UpperRightArm>`）和下臂（`M<LowerRightArm>`）组合成一个完整的手臂（`M<RightArm>`）。解决方案是 `map2M`。

`map2M` 的签名是：

```F#
val map2M : f:('a -> 'b -> 'c) -> M<'a> -> M<'b> -> M<'c>
```

### returnM

另一个挑战是在不使用任何生命力的情况下，直接将一个正常值提升到 M-配方的世界中。在那个特定的例子中，它将一个 `Skull` 变成了一个 `M<Skull>`，这样它就可以与 `map2M` 一起使用来制作一个完整的头部。

 `returnM` 的签名为：

```F#
val returnM : 'a -> M<'a>
```

### 单子函数

我们创建了许多形状相似的函数。它们都将某物作为输入，并返回 M-配方作为输出。换句话说，他们有这样的签名：

```F#
val monadicFunction : 'a -> M<'b>
```

以下是我们使用的实际单子函数的一些示例：

```F#
val makeLiveLeftLeg : DeadLeftLeg -> M<LiveLeftLeg>
val makeLiveRightLowerArm : DeadRightLowerArm -> M<LiveRightLowerArm>
val makeLiveHeart : DeadHeart -> M<LiveHeart>
val makeBeatingHeart : LiveHeart -> M<BeatingHeart>
// and also
val returnM : 'a -> M<'a>
```

### bindM

到目前为止，这些职能不需要动用关键部队。但后来我们发现，我们需要将两个一元函数链接在一起。特别是，我们需要将 `makeLiveHeart` 的输出（签名为 `DeadHeart -> M<LiveHeart>`）链接到 `makeBeatingHeart` 的输入（签名为 `LiveHeart -> M<BeatingHeart>`）。解决方案是 `bindM`，它将形式为 `'a -> M<'b>` 的一元函数转换为M-世界（`M<'a> -> M<'b>`）中的函数，然后可以组合在一起。

`bindM` 的签名是：

```F#
val bindM : f:('a -> M<'b>) -> M<'a> -> M<'b>
```

### applyM

最后，我们需要一种方法来组合大量的 M 参数来制作活体。我们不必创建特殊版本的 map（`map4M`、`map5M`、`map6M` 等），而是实现了一个通用的 `applyM` 函数，可以将 M 函数应用于 M 参数。由此，我们可以逐步使用任何大小的函数，使用部分应用一次应用一个 M 参数。

`applyM` 的签名是：

```F#
val applyM : M<('a -> 'b)> -> M<'a> -> M<'b>
```

### 根据 bind 和 return 定义其他函数

请注意，在所有这些功能中，只有 `bindM` 需要访问生命力。

事实上，正如我们将在下面看到的，函数 `mapM`、`map2M` 和 `applyM` 实际上可以用 `bindM` 和 `returnM` 来定义！

## 重构计算表达式

我们创建的许多函数具有非常相似的形状，导致大量重复。这里有一个例子：

```F#
let makeLiveLeftLegM deadLeftLeg  =
    let becomeAlive vitalForce =
        let (DeadLeftLeg label) = deadLeftLeg
        let oneUnit, remainingVitalForce = getVitalForce vitalForce
        let liveLeftLeg = LiveLeftLeg (label,oneUnit)
        liveLeftLeg, remainingVitalForce
    M becomeAlive  // wrap the function in a single case union
```

特别是，对生命力有很多明确的处理。

在大多数函数式语言中，都有一种方法可以隐藏它，这样代码看起来会更清晰。

在 Haskell 中，开发人员使用“do 符号”，在 Scala 中，人们使用“for-yield”（“for comprehensive”）。在 F# 中，人们使用计算表达式。

要在 F# 中创建一个计算表达式，你只需要从两件事开始，一个“bind”和一个“return”，我们都有。

接下来，您定义一个具有特殊命名方法 `Bind` 和 `Return` 的类：

```F#
type MonsterBuilder()=
    member this.Return(x) = returnM x
    member this.Bind(xM,f) = bindM f xM
```

最后，创建这个类的一个实例：

```F#
let monster = new MonsterBuilder()
```

完成此操作后，我们可以访问特殊的语法 `monster{…}`，就像 `async{…}`、`seq{…}` 等。

- `let! x = xM` 语法要求右侧是 M 类型，比如 `M<X>`。

  `let!` 将 `M<X>` 展开为 `X`，并将其绑定到左侧——在本例中为“x”。

- `return y` 语法要求返回值是“正常”类型，比如 `Y`。
  `return` 将其包装为 `M<Y>`（使用 `returnM`），并将其作为 `monster` 表达式的总值返回。

因此，一些示例代码看起来像这样：

```F#
monster {
    let! x = xM  // unwrap an M<X> into an X and bind to "x"
    return y     // wrap a Y and return an M<Y>
    }
```

如果你想了解更多关于计算表达式的信息，我有一系列关于它们的深入文章。

### 重新定义 mapM 和朋友

有了 `monster` 表达式，让我们重写 `mapM` 和其他函数。

#### mapM

`mapM` 接受一个函数和一个包装的 M 值，并返回应用于内部值的函数。

下面是一个使用 `monster` 的实现：

```F#
let mapM f xM =
    monster {
        let! x = xM  // unwrap the M<X>
        return f x   // return M of (f x)
        }
```

如果我们编译这个实现，我们会得到与前一个实现相同的签名：

```F#
val mapM : f:('a -> 'b) -> M<'a> -> M<'b>
```

#### map2M

`map2M` 接受一个函数和两个包装的 M 值，并返回应用于这两个值的函数。

使用 `monster` 表达式也很容易：

```F#
let map2M f xM yM =
    monster {
        let! x = xM  // unwrap M<X>
        let! y = yM  // unwrap M<Y>
        return f x y // return M of (f x y)
        }
```

如果我们编译这个实现，我们再次得到与前一个实现相同的签名：

```F#
val map2M : f:('a -> 'b -> 'c) -> M<'a> -> M<'b> -> M<'c>
```

#### applyM

`applyM` 接受一个包装函数和一个包装值，并返回应用于该值的函数。

同样，使用 `monster` 表达式写作也很简单：

```F#
let applyM fM xM =
    monster {
        let! f = fM  // unwrap M<F>
        let! x = xM  // unwrap M<X>
        return f x   // return M of (f x)
        }
```

签名与预期相符

```F#
val applyM : M<('a -> 'b)> -> M<'a> -> M<'b>
```

## 在怪物的上下文中操纵生命力

我们也想用 monster 表达式重写我们所有的其他函数，但有一个绊脚石。

我们的许多函数都有一个这样的身体：

```F#
// extract a unit of vital force from the context
let oneUnit, remainingVitalForce = getVitalForce vitalForce

// do something

// return value and remaining vital force
liveBodyPart, remainingVitalForce
```

换句话说，我们正在获得一些生命力，然后将一种新的生命力用于下一步。

我们熟悉面向对象编程中的“getter”和“setter”，所以让我们看看我们是否可以编写在怪物上下文中工作的类似程序。

### 介绍 getM

让我们从 getter 开始。我们应该如何实现它？

好吧，生命力只有在活着的情况下才可用，所以函数必须遵循熟悉的模板：

```F#
let getM =
    let doSomethingWhileLive vitalForce =
        // what here ??
        what to return??, vitalForce
    M doSomethingWhileLive
```

请注意，获取 `vitalForce` 不会耗尽任何资金，因此可以原封不动地返回原始金额。

但是中间应该发生什么呢？应该返回什么作为元组的第一个元素？

答案很简单：只需返回生命力本身！

```F#
let getM =
    let doSomethingWhileLive vitalForce =
        // return the current vital force in the first element of the tuple
        vitalForce, vitalForce
    M doSomethingWhileLive
```

`getM` 是一个 `M<VitalForce>` 值，这意味着我们可以在怪物表达式中展开它，如下所示：

```F#
monster {
    let! vitalForce = getM
    // do something with vital force
    }
```

### 介绍 putM

对于 putter 来说，实现是一个带有新生命力参数的函数。

```F#
let putM newVitalForce  =
    let doSomethingWhileLive vitalForce =
        what here ??
    M doSomethingWhileLive
```

再说一遍，我们应该在中间做些什么？

最重要的是， `newVitalForce` 成为传递到下一步的值。我们必须抛弃原有的生命力！

这反过来意味着 `newVitalForce` 必须用作返回的元组的第二部分。

返回的元组的第一部分应该是什么？没有可返回的合理值，所以我们只使用 `unit`。

以下是最终实现：

```F#
let putM newVitalForce  =
    let doSomethingWhileLive vitalForce =
        // return nothing in the first element of the tuple
        // return the newVitalForce in the second element of the tuple
        (), newVitalForce
    M doSomethingWhileLive
```

有了 `getM` 和 `putM`，我们现在可以创建一个函数

- 从上下文中获取当前的生命力
- 从中提取一个单位
- 用剩余的生命力替换当前的生命力
- 将一个单位的生命力返还给呼叫者

以下是代码：

```F#
let useUpOneUnitM =
    monster {
        let! vitalForce = getM
        let oneUnit, remainingVitalForce = getVitalForce vitalForce
        do! putM remainingVitalForce
        return oneUnit
        }
```

## 使用 monster 表达式重写所有其他函数

使用 `useUpOneUnitM`，我们可以开始重写所有其他函数。

例如，原始函数 `makeLiveLeftLegM` 看起来像这样，对生命力进行了大量的显式处理。

```F#
let makeLiveLeftLegM deadLeftLeg  =
    let becomeAlive vitalForce =
        let (DeadLeftLeg label) = deadLeftLeg
        let oneUnit, remainingVitalForce = getVitalForce vitalForce
        let liveLeftLeg = LiveLeftLeg (label,oneUnit)
        liveLeftLeg, remainingVitalForce
    M becomeAlive  // wrap the function in a single case union
```

新版本使用了怪物表达式，隐含了对生命力的处理，因此看起来更干净了。

```F#
let makeLiveLeftLegM deadLeftLeg =
    monster {
        let (DeadLeftLeg label) = deadLeftLeg
        let! oneUnit = useUpOneUnitM
        return LiveLeftLeg (label,oneUnit)
        }
```

同样，我们可以像这样重写所有手臂手术代码：

```F#
let makeLiveRightLowerArm (DeadRightLowerArm label) =
    monster {
        let! oneUnit = useUpOneUnitM
        return LiveRightLowerArm (label,oneUnit)
        }

let makeLiveRightUpperArm (DeadRightUpperArm label) =
    monster {
        let! oneUnit = useUpOneUnitM
        return LiveRightUpperArm (label,oneUnit)
        }

// create the M-parts
let lowerRightArmM = DeadRightLowerArm "Tom" |> makeLiveRightLowerArm
let upperRightArmM = DeadRightUpperArm "Jerry" |> makeLiveRightUpperArm

// turn armSurgery into an M-function
let armSurgeryM  = map2M armSurgery

// do surgery to combine the two M-parts into a new M-part
let rightArmM = armSurgeryM lowerRightArmM upperRightArmM
```

等等。这个新代码要干净得多。

事实上，我们可以通过消除 `armSurgery` 和 `armSurgeryM` 等中间值，并将所有内容放在一个怪物表达式中，使其更清晰。

```F#
let rightArmM = monster {
    let! lowerArm = DeadRightLowerArm "Tom" |> makeLiveRightLowerArm
    let! upperArm = DeadRightUpperArm "Jerry" |> makeLiveRightUpperArm
    return {lowerArm=lowerArm; upperArm=upperArm}
    }
```

我们也可以将这种方法用于头部。我们不再需要 `headSurgery` 或 `returnM`。

```F#
let headM = monster {
    let! brain = makeLiveBrain deadBrain
    return {brain=brain; skull=skull}
    }
```

最后，我们也可以使用 `monster` 表达式来创建整个身体：

```F#
// a function to create a M-body given all the M-parts
let createBodyM leftLegM rightLegM leftArmM rightArmM headM beatingHeartM =
    monster {
        let! leftLeg = leftLegM
        let! rightLeg = rightLegM
        let! leftArm = leftArmM
        let! rightArm = rightArmM
        let! head = headM
        let! beatingHeart = beatingHeartM

        // create the record
        return {
            leftLeg = leftLeg
            rightLeg = rightLeg
            leftArm = leftArm
            rightArm = rightArm
            head = head
            heart = beatingHeart
            }
        }

// create the M-body
let bodyM = createBodyM leftLegM rightLegM leftArmM rightArmM headM beatingHeartM
```

注意：使用 `monster` 表达式的完整代码可以在 GitHub 上找到。

### Monster 表达式 vs applyM

我们之前使用了另一种方法来创建身体，即使用 `applyM`。

作为参考，以下是使用 `applyM` 的方法：

```F#
let createBody leftLeg rightLeg leftArm rightArm head beatingHeart =
    {
    leftLeg = leftLeg
    rightLeg = rightLeg
    leftArm = leftArm
    rightArm = rightArm
    head = head
    heart = beatingHeart
    }

let bodyM =
    createBody
    <!> leftLegM
    <*> rightLegM
    <*> leftArmM
    <*> rightArmM
    <*> headM
    <*> beatingHeartM
```

那么，有什么区别呢？

在美学上有点不同，但你可以合理地选择其中之一。

然而，`applyM` 方法和 `monster` 表达式方法之间有一个更重要的区别。

`applyM` 方法允许参数独立或并行运行，而 `monster` 表达式方法要求参数按顺序运行，一个参数的输出被输入到下一个参数中。

这与此场景无关，但对于验证或异步等其他情况可能很重要。例如，在验证上下文中，您可能希望一次收集所有验证错误，而不是只返回第一个失败的错误。

## 与状态单子的关系

弗兰肯芬克特博士是她那个时代的先驱，开辟了一条新路，但她并没有将她的发现推广到其他领域。

如今，这种通过一系列函数传递信息的模式非常普遍，我们给它起了一个标准名称：“State Monad”。

现在，要成为一个真正的单子，必须满足各种属性（所谓的单子定律），但我不打算在这里讨论它们，因为这篇文章不是单子教程。

相反，我将只关注如何在实践中定义和使用状态单子。

首先，为了真正可重用，我们需要用其他类型替换 `VitalForce` 类型。因此，我们的函数包装类型（称之为 `S`）必须有两个类型参数，一个用于状态的类型，另一个用于值的类型。

```F#
type S<'State,'Value> =
    S of ('State -> 'Value * 'State)
```

有了这个定义，我们可以创建常见的嫌疑人：`runS`、`returnS` 和 `bindS`。

```F#
// encapsulate the function call that "runs" the state
let runS (S f) state = f state

// lift a value to the S-world
let returnS x =
    let run state =
        x, state
    S run

// lift a monadic function to the S-world
let bindS f xS =
    let run state =
        let x, newState = runS xS state
        runS (f x) newState
    S run
```

就我个人而言，我很高兴我们在使它们完全通用之前，已经了解了这些在 `M` 上下文中是如何工作的。我不知道你的情况，但像这样的签名

```F#
val runS : S<'a,'b> -> 'a -> 'b * 'a
val bindS : f:('a -> S<'b,'c>) -> S<'b,'a> -> S<'b,'c>
```

如果没有任何准备，他们自己真的很难理解。

不管怎样，有了这些基础知识，我们就可以创建一个 `state` 表达式。

```F#
type StateBuilder()=
    member this.Return(x) = returnS x
    member this.ReturnFrom(xS) = xS
    member this.Bind(xS,f) = bindS f xS

let state = new StateBuilder()
```

`getS` 和 `putS` 的定义方式与怪物的 `getM` 和 `putM` 类似。

```F#
let getS =
    let run state =
        // return the current state in the first element of the tuple
        state, state
    S run
// val getS : S<State>

let putS newState =
    let run _ =
        // return nothing in the first element of the tuple
        // return the newState in the second element of the tuple
        (), newState
    S run
// val putS : 'State -> S<unit>
```

### 基于属性的状态表达式测试

在继续之前，我们如何知道我们的 `state` 实现是正确的？正确到底意味着什么？

好吧，与其编写大量基于示例的测试，这是基于属性的测试方法的一个很好的候选者。

我们可能期望满足的属性包括：

- **单子定律**。
- **只有最后一次 put 才算数**。也就是说，先放 X 再放 Y 应该和只放 Y 一样。
- **Get 应该返回最后一个 put**。也就是说，放 X 然后做 get 应该返回相同的 X。

等等。

我现在不会再谈这个了。我建议看一下演讲，进行更深入的讨论。

### 使用状态表达式而不是怪物表达式

我们现在可以像使用怪物表达式一样使用状态表达式。这里有一个例子：

```F#
// combine get and put to extract one unit
let useUpOneUnitS = state {
    let! vitalForce = getS
    let oneUnit, remainingVitalForce = getVitalForce vitalForce
    do! putS remainingVitalForce
    return oneUnit
    }

type DeadLeftLeg = DeadLeftLeg of Label
type LiveLeftLeg = LiveLeftLeg of Label * VitalForce

// new version with implicit handling of vital force
let makeLiveLeftLeg (DeadLeftLeg label) = state {
    let! oneUnit = useUpOneUnitS
    return LiveLeftLeg (label,oneUnit)
    }
```

另一个例子是如何构建一颗 `BeatingHeart`：

```F#
type DeadHeart = DeadHeart of Label
type LiveHeart = LiveHeart of Label * VitalForce
type BeatingHeart = BeatingHeart of LiveHeart * VitalForce

let makeLiveHeart (DeadHeart label) = state {
    let! oneUnit = useUpOneUnitS
    return LiveHeart (label,oneUnit)
    }

let makeBeatingHeart liveHeart = state {
    let! oneUnit = useUpOneUnitS
    return BeatingHeart (liveHeart,oneUnit)
    }

let beatingHeartS = state {
    let! liveHeart = DeadHeart "Anne" |> makeLiveHeart
    return! makeBeatingHeart liveHeart
    }

let beatingHeart, remainingFromHeart = runS beatingHeartS vf
```

如您所见，`state` 表达式自动识别出 `VitalForce` 正被用作状态——我们不需要明确指定它。

所以，如果你有一个可用的 `state` 表达式类型，你根本不需要像 `monster` 一样创建自己的表达式！

有关 F# 中状态单子的更详细和复杂的示例，请查看 FSharpx 库。

*注意：使用 `state` 表达式的完整代码可以在 GitHub 上找到。*

## 使用状态表达式的其他示例

状态计算表达式一旦定义，就可以用于各种事情。例如，我们可以使用 `state` 来对堆栈进行建模。

让我们从定义 `Stack` 类型和相关函数开始：

```F#
// define the type to use as the state
type Stack<'a> = Stack of 'a list

// define pop outside of state expressions
let popStack (Stack contents) =
    match contents with
    | [] -> failwith "Stack underflow"
    | head::tail ->
        head, (Stack tail)

// define push outside of state expressions
let pushStack newTop (Stack contents) =
    Stack (newTop::contents)

// define an empty stack
let emptyStack = Stack []

// get the value of the stack when run
// starting with the empty stack
let getValue stackM =
    runS stackM emptyStack |> fst
```

请注意，这些代码都不知道或使用 `state` 计算表达式。

为了使其与 `state` 一起工作，我们需要定义一个定制的 getter 和 putter，以便在 `state` 上下文中使用：

```F#
let pop() = state {
    let! stack = getS
    let top, remainingStack = popStack stack
    do! putS remainingStack
    return top
    }

let push newTop = state {
    let! stack = getS
    let newStack = pushStack newTop stack
    do! putS newStack
    return ()
    }
```

有了这些，我们就可以开始编码我们的域名了！

### 基于堆栈的 Hello World

这里有一个简单的。我们按“world”，然后按“hello”，然后弹出堆栈并组合结果。

```F#
let helloWorldS = state {
    do! push "world"
    do! push "hello"
    let! top1 = pop()
    let! top2 = pop()
    let combined = top1 + " " + top2
    return combined
    }

let helloWorld = getValue helloWorldS // "hello world"
```

### 基于堆栈的计算器

这是一个简单的基于堆栈的计算器：

```F#
let one = state {do! push 1}
let two = state {do! push 2}

let add = state {
    let! top1 = pop()
    let! top2 = pop()
    do! push (top1 + top2)
    }
```

现在，我们可以将这些基本状态组合起来，构建更复杂的状态：

```F#
let three = state {
    do! one
    do! two
    do! add
    }

let five = state {
    do! two
    do! three
    do! add
    }
```

记住，就像生命力一样，我们现在所拥有的只是构建堆栈的配方。我们仍然需要运行它来执行配方并获得结果。

让我们添加一个助手来运行所有操作并返回堆栈顶部：

```F#
let calculate stackOperations = state {
    do! stackOperations
    let! top = pop()
    return top
    }
```

现在我们可以评估这些操作，如下所示：

```F#
let threeN = calculate three |> getValue // 3

let fiveN = calculate five |> getValue   // 5
```

## 好吧，好吧，一些单子的东西

人们总是想知道 monad，尽管我不希望这些帖子沦为另一个 monad 教程。

以下是他们如何与我们在这些帖子中的工作相契合。

**函子（functor）**（无论如何，在编程意义上）是一种数据结构（如 `Option`、`List` 或 `State`），它有一个与之关联的 `map` 函数。`map` 函数有一些必须满足的属性（“functor 定律”）。

**应用函子（applicative functor）**（在编程意义上）是一种数据结构（如 `Option`、`List` 或 `State`），它有两个相关函数：`apply` 和 `pure`（与 `return` 相同）。这些函数有一些必须满足的性质（“应用函子定律”）。

最后，**单子（monad）**（在编程意义上）是一种数据结构（如 `Option`、`List` 或 `State`），它有两个相关函数：`bind`（通常写成 `>>=`）和 `return`。再者，这些函数具有一些必须满足的性质（“单子定律”）。

在这三个函数中，monad 在某种意义上是最“强大”的，因为 `bind` 函数允许你将产生 M 的函数链接在一起，正如我们所看到的，`map` 和 `apply` 可以用 `bind` 和 `return` 来编写。

因此，您可以看到，我们的原始 `M` 类型和更通用的 `State` 类型，以及它们的支持函数，都是 monad（假设我们的 `bind` 和 `return` 实现满足 monad 定律）。

对于这些定义的视觉版本，有一篇很好的文章叫做“图片中的函子、应用函子和单子”。

## 进一步阅读

当然，网络上有很多关于状态单子的帖子。它们中的大多数都是面向 Haskell 的，但我希望在阅读了这一系列文章后，这些解释会更有意义，所以我只会提到一些后续链接。

- 图片中的状态单子
- “还有一些单子”，摘自《Haskell 趣学指南》
- 关于 Monads 的很多事情。关于 F# 中状态单子的讨论。

对于“bind”的另一个重要用途，您可能会发现我关于函数错误处理的演讲很有用。

如果你想看到其他单子的 F# 实现，只需看看 FSharpx 项目即可。

## 摘要

弗兰肯芬克特博士是一位具有开创性的实验家，我很高兴能够分享她对工作方式的见解。

我们已经看到了她是如何发现一个类似 monad 的原始类型 `M<BodyPart>` 的，以及 `mapM`、`map2M`、`returnM`、`bindM` 和 `applyM` 是如何开发来解决特定问题的。

我们还看到了解决同样问题的需要是如何导致现代状态单子和计算表达式的。

不管怎样，我希望这一系列的帖子能给人启发。我并不那么隐秘的愿望是，单子及其相关组合子现在不会再让你如此震惊了…

![shocking](https://fsharpforfunandprofit.com/posts/monadster-3/monadster_shocking300.gif)

…而且你可以在自己的项目中明智地使用它们。祝你好运！

*注意：本文中使用的代码示例可以在 GitHub 上找到。*