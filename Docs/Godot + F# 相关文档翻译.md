# ----------[HAMY 博客]----------



# Godot 4：使用 F# 编写脚本

https://hamy.xyz/labs/2023-04-godot-4-script-fsharp

Date: 2023-04-30 | [create](https://hamy.xyz/labs/tags/create) | [ctech](https://hamy.xyz/labs/tags/ctech) | [fsharp](https://hamy.xyz/labs/tags/fsharp) | [godot](https://hamy.xyz/labs/tags/godot) |

*[如果您正在运行 Godot 3，请参阅 Godot 3 指南](https://hamy.xyz/labs/2022-11-godot-script-with-fsharp).*

## 概述

F# 仍然是我最喜欢的编程语言。现在我全职专注于成为一名技术专家/小企业家（Tinypreneur），我一直在研究 F# 和创造性编码——这意味着我要重新回到 Godot。

在这篇文章中，我们将演示如何设置一个可以在 C# 和 F# 中运行脚本的 Godot 4 项目。

[YouTube 视频](https://www.youtube.com/embed/kJK_lBk4qOk)

## 要求

- **Godot 4（C#/.NET 版本）**- 此版本允许我们构建/运行 C#/.NET。我们将利用 .NET 支持运行 F#。
  - 要安装，请访问 [Godot 下载页面](https://godotengine.org/download/windows/)。
- **.NET SDK** - 因为我们将构建/运行 .NET，我们需要 .NET SDK 已安装。这也将使我们能够访问 `dotnet` 命令，这将在整个过程中为我们提供帮助。
  - 要安装，请前往 [.NET 下载页面](https://dotnet.microsoft.com/en-us/download)。*在撰写本文时，Godot 的目标是 .NET 6.0*。

## 创建 Godot 项目

[在 GitHub 上访问完整的 Godot 4 + F# 示例项目](https://github.com/HAMY-LABS/hamy-labs-code-examples/tree/main/fsharp/2023-04-godot-4-fsharp-example)，可供 [HAMINIONs 支持者](https://hamy.xyz/labs/haminions)使用。

我们要做的第一件事是创建一个兼容 .NET 的新 Godot 项目。

- 打开 Godot
- 创建新的 Godot 项目
- 将 C# 脚本添加到项目中
  - 为此，请执行以下操作：检查器 > 文件图标（创建新资源…）> CSharpScript（`Inspector > File Icon (Create a new Resource...) > CSharpScript`）

一旦我们添加了 C# 脚本，Godot 应该进行一些处理，并将 `.csproj` 添加到项目中。要验证这一点，请在文件资源管理器中打开项目文件夹（`.csproj` 将不会显示在 Godot 编辑器中）。您应该看到一个 `.csproj` 文件。

现在我们有了 `.csproj`，我们的项目应该有 C# 脚本支持。我们可以通过在项目中创建一个空节点并附加一个简单的脚本来验证这一点，该脚本将在我们构建/运行游戏时打印到 Godot 控制台。

*SimplePrintCs.cs*

```c#
using Godot;
using System;

public partial class SimplePrintCs : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("SimplePrintCs: C# Running...");
	}
}
```

*注意：在 Godot 4 中，所有连接到 Godot 的 C# 类都必须是分部的（`partial`），因此这与 Godot 3 不同。*

要将 C# 脚本连接到 Godot：

- 创建场景（如果您还没有）
- 创建节点（查找节点或 Node2D）
- 创建 C# 脚本
- 将新的 C# 脚本拖动到新节点上以将其附加

现在，当我们运行 Godot 项目时，我们应该能够看到我们的输出：

```
SimplePrintCs: C# Running...
```

## 在 Godot 中启用 F#

[在 GitHub 上访问完整的 Godot 4 + F# 示例项目](https://github.com/HAMY-LABS/hamy-labs-code-examples/tree/main/fsharp/2023-04-godot-4-fsharp-example)，可供 [HAMINIONs 支持者](https://hamy.xyz/labs/haminions)使用。

接下来，我们需要创建一个 F# 项目并将其连接到我们的 C# 项目。

首先创建一个 F# 库项目：

- 在项目中创建一个新文件夹（如 `ScriptsFs`）
- 导航到该目录（如 `cd ScriptsFs`）
- 使用 `dotnet` CLI 创建一个新的 F# 库项目：
  - `dotnet new classlib -lang "F#"`

这应该输出一个包含 `.fsproj` 和 `.fs` 脚本的最小 F# 项目。

现在我们有一个 C# 项目和一个 F# 项目，我们需要确保它们彼此兼容，并且与 Godot 引擎兼容。

同时打开 `.csproj` 和 `.fsproj`，并修改 `.fproj`，使其与 `.csproj` 兼容。以下是需要检查的常见/重要字段：

- `Project Sdk` - 这些必须针对相同的 Godot Sdk。在撰写本文时，我的 `.csproj` SDK 是 `Godot.NET.Sdk/4.0.2`，但使用 `.csproj` 中的任何内容
- `TargetFramework` - 用于配置目标 .NET 版本进行构建，因此这些版本必须匹配。在撰写本文时，我的 `.csproj` 表示 `net6.0`，但请使用 `.csproj` 中的任何内容

我们的 C# 和 F# 项目现在应该是兼容的，但我们需要一种方法来连接它们，以便它们可以相互通信。我们可以通过在 `.csproj` 中添加对 `.fsproj` 的引用来改变这一点。

这将允许我们的 C# 代码调用我们的 F# 代码。

将 C# 项目中的引用添加到 F# 项目中：

- 导航到 Godot 项目的根目录（`.csproj` 所在的位置）
- 运行 `dotnet add ./CSPROJ_NAME.csproj reference ./ScriptsFs/FSPROJ_NAME.fsproj`
  - 注意：将 `CSPROJ_NAME` 和 `FSPROJ_NAME` 替换为实际文件名。

这应该修改我们的 `.csproj`，以包含对 `.fsproj` 的引用。我们现在应该能够从 C# 代码调用 F# 代码。

修改我的 `.csproj`/`.fsproj` 后，它们看起来是这样的：

*ScriptsFs.fsproj*

```xml
<Project Sdk="Godot.NET.Sdk/4.0.2">

  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
  </PropertyGroup>

  <ItemGroup>
    <Compile Include="Library.fs" />
    <Compile Include="SimplePrintFs.fs" />
  </ItemGroup>

</Project>
```

*Godot4Fsharp.csproj*

```xml
<Project Sdk="Godot.NET.Sdk/4.0.2">
  <ItemGroup>
    <ProjectReference Include="ScriptsFs\ScriptsFs.fsproj" />
  </ItemGroup>
  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <EnableDynamicLoading>true</EnableDynamicLoading>
  </PropertyGroup>
</Project>
```

## Godot 中的 F# 脚本

[在 GitHub 上访问完整的 Godot 4 + F# 示例项目](https://github.com/HAMY-LABS/hamy-labs-code-examples/tree/main/fsharp/2023-04-godot-4-fsharp-example)，可供 [HAMINIONs 支持者](https://hamy.xyz/labs/haminions)使用。

目前，Godot 已经了解了我们的 C# 项目，而 C# 项目也已经了解了 F# 项目。

```
Godot -> C# -> F#
```

但是，Godot 不知道我们的 F# 项目，我们的 F# 也不了解我们的 C# 项目！目前，这是我们必须接受的限制。

那么，我们实际上是如何用它来构建东西的呢？

答案是——它有点笨重。基本上，我们需要一个附加到 Godot 的 C# 脚本，然后调用我们的 F# 项目使其运行。

范式看起来像这样：

- Godot 场景 - 包含带有附加 C# 脚本的节点，以便与游戏一起运行
- C# 脚本：FSharpCaller.cs -> 引用 F# 项目并直接调用它
- F# 脚本：要运行的实际代码

我找到了几种方法来连接这些，所以我们将逐一介绍并讨论它们各自的优缺点。对于每种方法，我还将提供一些示例代码，这些代码只是打印到 Godot 的控制台，以便我们可以看到如何调用。

Godot 4 的突破性变化：在之前的 Godot 3 中，我们能够通过简单地创建一个继承自 F# 类的占位符 C# 类来执行 F#。Godot 4 引入了一个突破性的更改，其中所有 C# 类都必须是分部类，这打破了这种集成。为了在 Godot 4 中执行 F#，我们现在必须直接从 C# 调用 F# 代码。

### 选项 1：假继承

*SimplePrintFsInheritanceHolder.cs*

```c#
using ScriptsFs; 

public partial class SimplePrintFsHolder : SimplePrintFs
{
	public override void _Ready()
	{
		GD.Print("SimplePrintFsHolder: C# Running...");
    // Ham: Without this base call, F# never gets run!
		base._Ready();
	}
}
```

*SimplePrintFsInheritance.fs*

```F#
namespace ScriptsFs

open Godot

type SimplePrintFsInheritance() =
    inherit Node()

    override this._Ready() =
        GD.Print($"{nameof SimplePrintFsInheritance}: F# Running...")
```

**选项1：伪继承** - 与 Godot 3 最相似。我们继承了 C# 持有者中的 F# 类，以便连接 Godot 和 F#。问题是，这将不再通过继承调用 F# 生命周期函数（如 `_Ready()`）！相反，我们必须直接调用F#基类才能执行它。

这将一些有点笨拙的东西（Godot - C# - F#）降级为真正笨拙的东西。这主要是因为我们仍然坚持继承模式，即使它不再适用于我们。

这就把我们带到了选项 2，考虑到 Godot 4 中的新约束，选项 2 试图更加务实。

### [推荐] 选项 2：F# 库

*SimplePrintFsLibraryHolder.cs*

```c#
using Godot;

using ScriptsFs; 

public partial class SimplePrintFsLibraryHolder : Node
{
	public override void _Ready()
	{
		GD.Print($"{nameof(SimplePrintFsLibraryHolder)}: C# Running...");

		SimplePrintFsLibrary.logRunning();
	}
}
```

*SimplePrintFsLibrary.fs*

```F#
namespace ScriptsFs

open Godot

module SimplePrintFsLibrary =
    // Ham: There is no way to cleanly get current module name, so make hard-coded configuration
    let currentModuleName = "SimplePrintFsLibrary"

    let logRunning = fun() ->
        GD.Print($"{currentModuleName}: F# Running...") 
```

在**选项 2：F# 库**中，我们将从 F# 中删除 C# 的继承，而是将我们的 F# 代码更像一个库。在许多方面，这是一个比 1 更好的选择，因为它更简单、更透明地了解这里的实际情况。

如果你眯着眼睛看一看，这看起来很像 DDD/Clean Code，但在 Godot 游戏环境中。我们将 C# 代码视为表示层。那么我们的 F# 就是核心 App/Domain 逻辑。

我们翻译来自 C# -> F# 的 Godot 游戏信号，并根据 F# -> C# 的返回更新我们的 Godot 游戏状态。

这是我将使用的选项，因为我认为在我们的 Godot 项目中，有一条更干净的方法可以使用惯用的 F# 来获得乐趣和利润。

## 下一步

Godot 3 没有提供最愉快的 F# 体验，Godot 4 打破了这个变通方法，让我不再推荐它。然而，这些新的约束可能为 Godot 到 F# 的互操作解锁了一个更清晰、更好的范式，我很高兴看到它的发展方向。

在接下来的几个月里，我将尝试不同的范式，所以一定要关注/订阅更新。

### 想要更多这样的吗？

支持我工作的最佳/最简单的方法是订阅未来的更新并与您的网络共享。

- [订阅电子邮件更新](https://hamniverse.substack.com/)
- [关注 HAMY LABS Socials](https://hamy.xyz/)
- [成为 HAMINIONs 会员](https://hamy.xyz/labs/haminions)
- [赞助商 HAMY LABS](https://hamy.xyz/labs/sponsors)



# [系列]代码的本质 - Godot 4 + F#

https://hamy.xyz/labs/2023-04-series-the-nature-of-code-godot-4-fsharp

Date: 2023-04-15 | [series](https://hamy.xyz/labs/tags/series) | [create](https://hamy.xyz/labs/tags/create) | [ctech](https://hamy.xyz/labs/tags/ctech) | [godot](https://hamy.xyz/labs/tags/godot) | [fsharp](https://hamy.xyz/labs/tags/fsharp) | [the-nature-of-code](https://hamy.xyz/labs/tags/the-nature-of-code) |

自从[我辞去全职软件工程师的工作](https://hamy.xyz/blog/2023-03-i-quit-my-job-again)以来，我一直在重新投入[创意编码](https://www.instagram.com/hamy.art/)。Godot 长期以来一直是我的首选，因为它具有轻量级、高性能的属性，我认为随着时间的推移，这些属性将使我能够构建更高规模的视觉效果。

过去，我曾将[《代码的本质》](https://amzn.to/40ayNwv)作为 JS、Python 和 C# 开启创意技术之旅的指南和参考。我计划再次使用它来帮助重新熟悉图形编程，这次是用 F#。

当我使用 Godot 4 + F# 完成《代码的本质》中的练习时，此页面将作为链接我的实验的中心。

如果您想开始使用 Godot 4 和 F#，请查看我的设置指南：[Godot 4: 使用 F# 编写脚本](https://hamy.xyz/labs/2023-04-godot-4-script-fsharp)。

## 练习

我将包括我在这里完成的每个练习，包括核心源代码和它如何工作的一般演练。如果您是 [HAMINION 的支持者](https://hamy.xyz/labs/haminions)，您还可以访问[完整的项目文件](https://github.com/HAMY-LABS/hamy-labs-code-examples/tree/main/fsharp/2023-04-the-nature-of-code-godot-4-fsharp)进行克隆并在本地运行。

WIP：练习即将到来！

### 介绍

[I.1-传统随机漫步](https://hamy.xyz/labs/2023-04-the-nature-of-code-godot-4-fsharp-introduction-1)

### 想要更多这样的吗？

支持我工作的最佳/最简单的方法是订阅未来的更新并与您的网络共享。

- [订阅电子邮件更新](https://hamniverse.substack.com/)
- [关注 HAMY LABS Socials](https://hamy.xyz/)
- [成为 HAMINIONs 会员](https://hamy.xyz/labs/haminions)
- [赞助商 HAMY LABS](https://hamy.xyz/labs/sponsors)



# 代码的本质：示例 I.1 - 传统随机游走（Godot 4 + F#）

https://hamy.xyz/labs/2023-04-the-nature-of-code-godot-4-fsharp-introduction-1

Date: 2023-05-03 | [create](https://hamy.xyz/labs/tags/create) | [ctech](https://hamy.xyz/labs/tags/ctech) | [godot](https://hamy.xyz/labs/tags/godot) | [fsharp](https://hamy.xyz/labs/tags/fsharp) | [the-nature-of-code](https://hamy.xyz/labs/tags/the-nature-of-code) |

这是[代码的本质（Godot 4 + F#）系列](https://hamy.xyz/labs/2023-04-series-the-nature-of-code-godot-4-fsharp)的一部分。

## 概述

在这篇文章中，我将详细介绍我对[《代码的本质》](https://amzn.to/40ayNwv)中练习 I.1（传统随机游走）的解决方案。

[YouTube 视频](https://www.youtube.com/embed/L29Rw_j9SQ8)

## 规则

任务基本上是创建一个可以在四个方向（上、下、左、右）之一移动的随机步行者。

其功能要求概述：

- 传统随机步行者
  - Position (x, y)
  - Init - 从屏幕中间开始
  - Step - 沿随机方向（上、下、左、右）迈出一步
  - Display - 显示自身的能力

## 解决方案概述

对于这个解决方案，我有两个主要目标：

- 尽可能简单（我们会在学习过程中了解更多）
- 大规模实验 Godot <> F# 互操作范式

在尝试将 Godot 与 F# 一起使用时，我们有一些独特的限制，即互操作性不好。这基本上强制了 Godot 代码和 F# 代码之间的严格划分，因为我们需要一个中间翻译层（C#）。

*我详细讨论了 Godot 4 + F# 互操作策略：[Godot 4: 使用 F# 编写脚本](https://hamy.xyz/labs/2023-04-godot-4-script-fsharp)。*

这迫使我们放弃某些模式（主要是具有共享状态的面向对象学习），转向其他可以分离的模式。幸运的是，带有纯函数的 FP 风格代码在这些约束下实际上工作得很好，所以我将尝试以更 FP 的风格编写这些东西。

![Godot 4 + F#: Architecture](https://storage.googleapis.com/iamhamy-static/labs/posts/2023/2023-04_series-the-nature-of-code-godot-4-fsharp/2023-04_godot-fsharp-architecture.jpg)

*Godot 4 + F#：架构*

总体架构将是“干净代码”式的：

- 外部：Godot
- 展示（Presentation）/外部端口 - C#
- 应用程序/领域逻辑 - F#

要更深入地了解此 Godot - C# - F# 架构，请参阅：[Godot 4: 使用 F# 编写脚本](https://hamy.xyz/labs/2023-04-godot-4-script-fsharp)

这可能看起来过于复杂（也许是这样，我仍在尝试这种范式），但我认为当我们查看实际代码时，它会变得更加清晰。

## 解决方案代码

基本上，我们只有两个文件：

- RandomWalkerHolder.cs - 一个从 Godot 转换为 F# 域逻辑（生命周期事件、状态等）的 C# 文件
- RandomWalker.fs - 一个 F# 文件，用于处理我们的 Walker（实际上是我们的整个场景/游戏功能）

我们将把 C# 脚本添加到场景中的 3D 节点中，该节点将更新其自身的位置。

*注意：在这个脚本的未来版本中，我们可能会将场景逻辑与 Walker 逻辑分开，但我现在想保持简单。*

*I_1_TraditionalRandomWalkerHolder.cs*

```c#
using Godot;
using System;

using ScriptsFs;

public partial class I_1_TraditionalRandomWalkerHolder : Node3D
{
	private I_1_TraditionalRandomWalker.SceneState SceneState { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.SceneState = I_1_TraditionalRandomWalker.Create();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		SceneState = I_1_TraditionalRandomWalker.Update(
			this.SceneState,
			new I_1_TraditionalRandomWalker.ProcessEvent (
				DateTimeOffset.Now.ToUnixTimeMilliseconds()
			)
		);

		Position = new Vector3(
			SceneState.Walker.Position.X,
			SceneState.Walker.Position.Y,
			0f
		);
	}
}
```

此文件是附加到 Godot 场景中 `Node3D` 的 `Node3D`。本质上，它所做的就是将 Godot 连接到我们的 F# 游戏逻辑。

- 在 `_Ready` - 调用 F# 获取场景的初始状态并将其保存到对象中
- 在 `_Process` - 使用更新的数据调用 F# 以获取场景的新状态，并使用返回值更新 Godot

*I_1_TraditionalRandomWalker.fs*

```F#
namespace ScriptsFs 

open Godot
open Microsoft.FSharp.Reflection
open System

module I_1_TraditionalRandomWalker = 
    let walkStepMagnitude = 1f

    let random = new Random()

    type StepDirections =
        | Up
        | Down
        | Left
        | Right 

    type TraditionalRandomWalker = {
        Position : Vector2
    }

    type ProcessEvent = {
        ElapsedTimeMs : int64
    }

    type SceneState = {
        Walker : TraditionalRandomWalker
    }

    let CreateWalker (xPosition : float32) (yPosition : float32) : TraditionalRandomWalker = 
        {
            Position = Vector2(xPosition, yPosition)
        }

    let getNextWalkerPosition : TraditionalRandomWalker -> TraditionalRandomWalker =
        // Ham: This is ugly, but only way to enumerate DU cases
        // Source: https://stackoverflow.com/questions/6997083/how-to-enumerate-a-discriminated-union-in-f
        let allDirections : StepDirections list = 
            FSharpType.GetUnionCases typeof<StepDirections>
            |> Seq.map (fun(caseInfo) ->
                FSharpValue.MakeUnion(caseInfo, [||])
                :?> StepDirections)
            |> Seq.toList

        fun (walker : TraditionalRandomWalker) -> 
            let index = random.Next(0, allDirections.Length) 
            let locationDelta = 
                match allDirections[index] with 
                | Up -> (0f, 1f)
                | Down -> (0f, -1f)
                | Left -> (-1f, 0f)
                | Right -> (1f, 0f)
            let newPositionTuple = ((
                walker.Position.X + (fst locationDelta),
                walker.Position.Y + (snd locationDelta)
            ))

            {
                walker with 
                    Position = (
                        new Vector2(
                            (fst newPositionTuple),
                            (snd newPositionTuple)
                        )
                    )
            }

    let TimeBasedDebounce<'a> (debounceRateMs : int64) : (unit -> 'a) -> 'a -> int64 -> 'a =
        let mutable lastExecutedTimeMs = 0L

        fun (fn : unit -> 'a) (currentReturn : 'a) (currentTimeMs : int64) ->
            match currentTimeMs with
            | t when (currentTimeMs > (lastExecutedTimeMs + debounceRateMs)) ->
                    lastExecutedTimeMs <- currentTimeMs
                    fn()
            | _ -> currentReturn

    let UpdateWalker : ProcessEvent -> TraditionalRandomWalker -> TraditionalRandomWalker =
        let updateWalkerFn = TimeBasedDebounce<TraditionalRandomWalker> 200
        
        fun (processEvent : ProcessEvent) (walker : TraditionalRandomWalker) ->
            updateWalkerFn (fun() -> (getNextWalkerPosition walker)) walker processEvent.ElapsedTimeMs

    let Update (sceneState : SceneState) (processEvent : ProcessEvent) : SceneState =
        let updateWalker = UpdateWalker processEvent sceneState.Walker 

        {
            Walker = updateWalker
        }

    let Create : unit -> SceneState = 
        fun() ->
            {
                Walker = CreateWalker 0f 0f
            }
```

此脚本处理整个场景，包括如何创建和更新场景。这种方法的美妙之处在于，我们与任何一个实现的耦合都很小——很容易看出我们如何将这段代码移植到任何地方使用——它与 Godot 无关。缺点是我们通过抽象增加了一些额外的复杂性。

- 在文件的顶部，我们声明了所有类型。这在 F# 中很常见，我认为在大多数程序中都是一种很好的做法。
- 然后，我们定义场景运行所需的函数，包括 `Create` 和 `Update`，以及每个较小实体 `CreateWalker` 和 `UpdateWalker` 的一些辅助函数。
- 这段代码的大部分是纯代码 - 这意味着我们在输入和输出之间有一个确定性的联系。主要的例外是 `Random` 的创建，但如果我们愿意，我们也可以通过它。

## 下一步

这就是这次代码潜水。如果您对改进此代码有任何疑问或建议，请告诉我。我仍在研究如何在这种环境中最好地定义/分离代码，所以我认为随着我读完这本书，这些想法中的许多都会不断发展。

### 想要更多这样的吗？

支持我工作的最佳/最简单的方法是订阅未来的更新并与您的网络共享。

- [订阅电子邮件更新](https://hamniverse.substack.com/)
- [关注 HAMY LABS Socials](https://hamy.xyz/)
- [成为 HAMINIONs 会员](https://hamy.xyz/labs/haminions)
- [赞助商 HAMY LABS](https://hamy.xyz/labs/sponsors)



# 我（再次）辞去了软件工程师的工作

Date: 2023-03-26 | [create](https://hamy.xyz/blog/tags/create) | [career](https://hamy.xyz/blog/tags/career) | [hamy-labs](https://hamy.xyz/blog/tags/hamy-labs) |

我辞职了。。。再一次。

11 月，[我辞去了 Instagram 软件工程师的工作](https://hamy.xyz/blog/2022-11-instagram-badge-post)。我一直在寻找自由/经验来作为一名技术专家/小企业家（Tinypreneur）（在员工和投资组合层面）构建我的无尽游戏（Endless Game）。

但后来我收到了一份在 [Reddit 担任高级软件工程师的理想工作](https://hamy.xyz/blog/2022-Review#career)——一家创建我使用和喜爱的产品的初创公司。我认为这将是在一家真正做这件事的公司从内部了解初创公司/创业精神的好方法，而且风险要小得多，有稳定的薪水/医疗保险等。

但几个月后，我意识到 Reddit 并不是一家真正的初创公司——它是一个旨在进一步全球化和现代化的扩展。这对公司来说是有意义的，因为它的目标是下一阶段的增长和资本化，但对我来说并没有什么意义。

当我预测在 Reddit 再工作 12 个月会获得的经验和教训时，我意识到我不会更接近我的无尽游戏——我想在余生中每天都在度过的生活/职业生涯。一直以来，时间都在流逝，所以这 12 个月是另一个 12 个月，没有朝着我的无限游戏（机会成本）努力。

我对 2023 年的愿景是更接近我的无尽游戏。我觉得继续走我所走的路会错过那些目标，而这些目标反过来又会让我离我的生活/职业计划更远。最终，我决定是时候做出另一个改变了。

[YouTube 视频](https://www.youtube.com/embed/6Owe8XfDreA)

## 接下来是什么

事实上，我还在想。但指导原则与我 11 月的指导原则相似，现在又增加了新的 #知识。我有一个方向性的策略来指导我即将到来的[创作周期](https://hamy.xyz/labs/2022-12-the-creation-cycle)。

第一个是一个标签，可以有效地向大家解释我今年追求的目标。这既是对我希望实现的目标的诚实、描述性的审视，也是对使用多年寿命和 1 万美元几乎没有金钱回报的可识别和可接受的解释——IRLMBA/CS：

- 在（In）
- 真实（Real）
- 生活（Life）
- 做（Make）
- 商业（Business）
- 资产（Assets）
- /
- 创建（Create）
- 东西（Stuff）

我追求 IRLMBA/CS 的主要目标是接近拉面盈利（Ramen Profitability）能力，这将解锁我的无尽游戏。拉面盈利能力对于任何无休止的游戏来说都是至关重要的，因为没有它，你就破产了，不能再玩了。#wasted

*拉面盈利能力 = 利润 > 最低生活费*

从专业角度来看，我将把我的 [Linkedin 头衔](https://www.linkedin.com/in/hamiltongreene/)更新为 HAMY LABS 的创始人兼首席技术官。这似乎比我的真实头衔“技术专家/Tinypreneur”在专业上更容易接受/理解，[我的简历需要尽可能多的帮助](https://hamy.xyz/blog/2023-03-i-quit-my-job-again#faq)。

## 三个任务

在接下来的一年里，我给自己三个任务来彻底探索：

- **建设**——企业和 CTECH。唯一可行的方法是，如果我真的尝试过我的无尽游戏。我的无尽游戏是以建造东西为基础的，所以我必须建造。
- **学习**——深入研究我感兴趣的研究领域/领域，我觉得这将加强我的资产组合。我将学习几门课程，并进行大量自主研究。我的目标/希望是，通过简单地将我的许多核心技能应用于我目标发挥的领域内的许多不同周期，我可以在许多核心技能上实现 20-50% 的改进。
- **探索**——弄清楚我接下来想做什么。我越来越清楚，到目前为止，我在专业 Create 领域寻找的经验/价值观已不再适用。我希望测试不同的领域/假设，以帮助弄清楚我的新价值观是什么，以及我接下来可能想做什么：
  - Q： 我的无尽游戏真的是我余生想做的吗？
  - Q： 如果这失败了，我如何以一种仍然可以解锁我的无尽游戏的方式养活自己？
  - Q： 在接下来的 5 到 10 年里，我会喜欢什么？

## 支持/关注

如果你想支持/关注 - 首先我很感激你！我不知道自己在做什么，所以我真的需要所有能得到的帮助/指导。这是一段旅程，老实说，我不确定 8 个月后我会去哪里。

支持我的旅程的最佳/最简单的方法是简单地关注更新：

- [推特：@SIRHAMY](https://twitter.com/SIRHAMY)
- [YouTube：HAMY LABS](https://www.youtube.com/hamylabs)
- [Instagram：@hamy.labs](https://www.instagram.com/hamy.labs/)

如果您想进一步支持：

- 对我的项目提供反馈：[过去 10 多年的所有项目](https://hamy.xyz/projects)
- [成为 HAMINION](https://hamy.xyz/labs/haminions)：获得独家折扣、代码访问权限，帮助我成功=）））

## 常见问题解答

我想我会收到很多关于这个决定的问题。在本节中，我将探讨其中的一些问题，既可以有效地分发答案，也可以反思并确保我真正思考过整件事。

### 为什么是现在？

我在 Reddit 的时间比我通常想要的要短得多。一般来说，我认为你应该尝试一家公司至少 6-12 个月，以真正了解它（晋升通常需要 6 个月），并保持你的简历没有危险信号（<12 个月通常被认为是一个令人遗憾的雇佣，因为公司可能没有从聘用中获利）。但有几个因素共同作用，使早期转换的好处远远超过了短期任期的潜在缺点。

- **关闭零依赖责任窗口**。我[今年 30 岁结婚了](https://hamy.xyz/blog/2022-Review#engaged)。这向我发出信号，我基本上没有依赖责任的时间即将结束。因此，这可能是我余生冒险的最佳时机。
- **缺乏激情**。在过去的 1.5 年里，我一直在构建我职业生涯中最好的软件，但背后几乎没有激情。与前几年相比，我们可以[在 2022 年发布的项目](https://hamy.xyz/projects)的风格/质量/数量中清楚地看到这一点。我相信充实的生活遵循[对冲猪原则（Hedge Hog principle）](https://twitter.com/SIRHAMY/status/1638553126276849664)（赚钱，做你擅长的事情，以及你热爱的事情）。只要我错过了激情部分，我就错过了过上最好的生活和打造最好的创作。
- **没有合适的时机，只有比别的更好的时机**。事实上，做事没有合适的时机。等待下一个季度/半年/一年可能是有意义的，这总是有原因的。这种想法的问题在于，它会很快导致拖延/对下一件事的饥饿——在那里你永远不会真正跳槽。我不知道现在是否是最好的时机。但我确实知道，我所做的事情似乎并不正确，老实说，现在去做几乎没有什么坏处。因此，在没有确凿证据表明更好的未来即将到来的情况下，我必须根据我掌握的有限数据，假设现在和任何即将到来的时间一样好。

### 风险

我不会撒谎，独自外出有很多风险。所以在这里，我列出了已知的风险以及我计划如何处理它们。

- **钱**。今年我可能会损失一大笔钱，因为我在没有任何收入的情况下为自己的生活买单。
  - 但我很年轻，有很高的收入潜力（假设人工智能不会承担所有的工作），在我职业生涯的大部分时间里，我[对自己的财务都非常谨慎](https://hamy.xyz/blog/tags/finance)。这让我非常有信心，即使在最坏的情况下，我在未来几年的财务状况也会很好。
  - 最坏的情况是，我将把这个实验限制在 2023 年底。如果我没有接近拉面的盈利能力，那么我会去某个地方寻求资金/工作。下行空间有限。
- **职业生涯**。在一家公司工作不到 6 个月就离职，这看起来并不好。作为一名软件工程师，在顶级公司工作的时间越少，在无名初创公司（HAMY LABS）工作的时间越多，并不一定能提高我的收入潜力。
  - 但我认为[我过去的公司经验](https://www.linkedin.com/in/hamiltongreene/)和推理（在像这样的股票中）可以抚平很多这些皱纹。此外，我希望 HAMY LABS 能够成功，或者至少让我更接近我的无尽游戏，这对我的生活/职业生涯比攀登公司阶梯更重要。
  - 最坏的情况是，我重新加入公司，从原来的地方开始，但一般情况是，如果失败了，我可能会成为初创公司/小公司更具吸引力的候选人，我假设我可能想去那里（众所周知，大型科技工程公司在小公司表现不佳）

### 想要更多这样的吗？

支持我工作的最佳/最简单的方法是订阅未来的更新并与您的网络共享。

- [订阅电子邮件更新](https://hamniverse.substack.com/)
- [关注 HAMY LABS Socials](https://hamy.xyz/)
- [成为 HAMINIONs 会员](https://hamy.xyz/labs/haminions)
- [赞助商 HAMY LABS](https://hamy.xyz/labs/sponsors)



# Godot 3：使用 F# 编写脚本

https://hamy.xyz/labs/2022-11-godot-script-with-fsharp

Date: 2022-11-09 | [godot](https://hamy.xyz/labs/tags/godot) | [fsharp](https://hamy.xyz/labs/tags/fsharp) | [csharp](https://hamy.xyz/labs/tags/csharp) |

*[如果您正在运行 Godot 4，请参阅 Godot 4 指南。](https://hamy.xyz/labs/2023-04-godot-4-script-fsharp)*

## 概述

F# 很快成为我最喜欢的编程语言/生态系统。然而，它在许多领域（包括游戏/3D开发）的直接支持仍然落后，这意味着需要进行一些修补才能启动和运行。

在这篇文章中，我们将演示如何设置一个可以在 C# 和 F# 中运行脚本的 Godot 项目。

[YouTube 视频](https://www.youtube.com/embed/LwTdcNYGXM0)

## 要求

- **Godot 3（Mono 版本）**- 我们需要 Godot Mono 版本，以便有 C# 支持。这隐含地给了我们 .NET 支持，我们将利用它来运行 F# 代码。
  - 要安装，请前往 [Godot 下载页面](https://godotengine.org/download)
- **.NET SDK** - 我们需要已安装 .NET SDK 在我们的机器上，因此我们可以使用 `dotnet` 命令创建和修改 `.csproj` 和 `.fsproj` 文件
  - 要安装，请前往 [.NET 下载页面](https://dotnet.microsoft.com/en-us/download)

## 创建 Godot 项目

*注意：您可以在 GitHub 上获取[完整的 Godot 和 F# 项目源代码](https://github.com/SIRHAMY/godot-fsharp-example)。*

我们要做的第一件事是创建一个兼容 C# 和 F# 的新 Godot 项目。

- 打开 Godot
- 创建新的 Godot 项目
- 将 C# 脚本添加到项目中 - 这将强制项目创建 `.csproj`，并为我们的项目启用 C# 脚本支持
  - 您可以通过在场景资源管理器中右键单击并浏览选项来创建新的 C# 脚本来完成此操作

我们的项目现在应该有 C# 支持。我们可以通过在项目中创建一个空节点并附加一个将打印到控制台的简单脚本来检查这一点。当我们运行它时，我们应该在控制台中看到输出。

*SimplePrintCs.cs*

```c#
using Godot;
using System;

public class SimplePrintCs : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("SimplePrintCs: C# Running...");
	}
}
```

当我们运行 Godot 项目时，我们希望它输出：

```
SimplePrintCs: C# Running...
```

### 在 Godot 中启用 F#

*注意：您可以在 GitHub 上获取[完整的 Godot 和 F# 项目源代码](https://github.com/SIRHAMY/godot-fsharp-example)。*

现在我们的 Godot 项目中有一个 `.csproj`，我们可以开始修改它以启用 F#。

首先，我们将创建一个 F# 库项目。

- 在项目中创建一个新文件夹（如 `ScriptsFs`）
- 导航到该目录（如 `cd ScriptsFs`）
- 创建新的 F# 库项目：
  - `dotnet new classlib -lang "F#"`

这应该输出一个包含 `.fsproj` 和示例 `.fs` 脚本的最小 F# 项目。

现在我们有了这个最小的 F# 项目，我们需要在 `.fsproj` 中修改它的构建依赖关系，使其与我们的 Godot 项目和脚本兼容。

打开 `.fsproj` 并进行更改以匹配 `.csproj`：

- `Sdk` - 在撰写本文时，当前的 Godot LTS 是 `Godot.NET.Sdk/3.3.0`，但这可能会改变，重要的是 `.csproj` 和 `.fsproj` 匹配。通过直接引用 Godot SDK，我们可以让 F# 代码访问 Godot API
- `PropertyGroup > TargetFramework` - 在撰写本文时，Godot 的目标是 `net472`，但重要的是您的 `.csproj` 和 `.fsproj` 在目标上达成一致。

目前，我们有一个 C# 项目和一个 F# 项目了解 Godot，但他们还不了解彼此。我们可以通过将 `.csproj` 中的项目引用添加到 `.fsproj` 中来改变这一点，这将允许我们的 C# 代码调用我们的 F# 代码。

添加一个从 C# 到 F# 的引用：

- 导航到 Godot 项目的根目录（`.csproj` 所在的位置）
- 运行 `dotnet add ./CSPROJ_NAME.csproj reference ./ScriptsFs/ScriptsFs.fsproj`
  - （将 `CSPROJ_NAME` 替换为您的 `.csproj` 的实际名称）

如果幸运的话，我们现在应该能够从 C# 调用我们的 F# 代码。

## Godot 中的 F# 脚本

Godot 知道我们的 C# 项目，C# 项目知道我们的 F# 项目，但目前 Godot 不知道 F#，所以它不能直接调用它。网络上有一些解决方法，但我发现它们有点笨拙和手动，所以我们不会启用它，而是从基 C# 文件调用所有 F#。

当你想在 Godot 中引用 F# 脚本时，我们会这样做：

- C# 文件：FSharpHoldr.cs -> 引用实际代码
- F# 文件：实际代码 -> 实际逻辑就在这里

所以，假设我们想要一个打印到控制台的脚本。我们可以用 F# 这样写：

*SimplePrintFs.fs*

```F#
namespace ScriptsFs

open Godot

type SimplePrintFs() =
    inherit Node()

    override this._Ready() =
        GD.Print("SimplePrintFs: F# Running...")
```

*注意：确保你更新了你的 `.fsproj`，这样它就知道要编译这个新脚本了！*

现在，我们创建一个占位符 C# 文件，将其附加到 Godot 场景中，以方便 Godot -> F# 连接。占位符 C# 文件直接从 F# 文件继承，允许它直接运行。

*SimplePrintFsHolder.cs*

```c#
using ScriptsFs;

public class SimplePrintFsHolder : SimplePrintFs
{}
```

现在，当我们运行这个程序时，我们希望得到 C# 和 F# 的打印结果！

```
SimplePrintCs: C# Running...
SimplePrintFs: F# Running...
```

## 下一步

希望这能让你开始 Godot x F# 之旅！

- [完整 Godot F# 源代码](https://github.com/SIRHAMY/godot-fsharp-example)可在 GitHub 上获得
- 如果您遇到任何问题/对 Godot 或 F# 有任何疑问，请告诉我！

### 想要更多这样的吗？

支持我工作的最佳/最简单的方法是订阅未来的更新并与您的网络共享。

- [订阅电子邮件更新](https://hamniverse.substack.com/)
- [关注 HAMY LABS Socials](https://hamy.xyz/)
- [成为 HAMINIONs 会员](https://hamy.xyz/labs/haminions)
- [赞助商 HAMY LABS](https://hamy.xyz/labs/sponsors)



# ----------[bencope.land 博客]----------



# Godot 4 和 F#

https://blog.bencope.land/godot-4-and-f/

**[Ben Copeland](https://blog.bencope.land/author/ben/)**

Sep 21, 2022 • 2 min read

**更新：**在开始制作游戏后，结果并不像这篇文章所暗示的那样直截了当。主要是因为 Godot 4 要求 C# 类是扩展相应 Godot 实体的 `partial` 类。据我所知，F# 没有直接翻译分部类。虽然按照这篇文章中的说明会让 F# 进入项目，但到目前为止，我不得不将 F# 作为实用程序（utilities）和逻辑（logic），通过 C# 代码消费它们；而不是在 F# 中进行所有的提升（如果你可以直接从 F# 类扩展 Godot 实体，你就可以这样做）。如果我找到更好的方法，我一定会再次更新。

**原始帖子：**Godot 4 终于进入测试阶段，.NET 6 版本可供下载。以前我写过一篇关于将 F# 添加到 Godot 3 项目的复杂过程的文章，但现在我很高兴地说，添加 F# 非常简单。唯一能让它更简单的是，如果你能从 Godot UI 正式初始化 F# 脚本。

以下是使用 F# 脚本开始的步骤：

- 在此处下载 Godot 4 测试版
- 打开它并创建新项目
- 在场景中添加一个节点（在我的例子中，我添加了一个 node2d）
- 右键单击节点并选择“附加脚本”
- 选择 C# 作为语言，并将其他选项保留为默认值
- 添加脚本将创建一个 .NET 解决方案文件。打开 Rider 或您选择的 IDE 并打开该解决方案文件
- 右键单击左侧面板中的解决方案，然后选择“添加项目”
- 创建一个空白的 F# 类库。我通常将我的项目命名为“FsharpScripting”、“[GameName]FS” 或 “[Scene]FS”。
- 右键单击左侧面板中的 C# 项目，然后选择“添加引用”。添加您刚才创建的 F# 库。
  打开 `.fsproj` 文件，将父级 `<Project>` 元素更改为 `<Project Sdk="Godot.NET.Sdk/4.0.0-beta1">`。这将允许您将Godot模块导入F#模块。

现在你们都准备好了。你可以用 F# 而不是 C# 编写代码。如果你回到 Godot 生成的 .cs 文件，它看起来像这样：

```c#
using Godot;
using System;

public partial class node_2d : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
```

此类扩展了 Godot 的 `Node2D` 类。我们可以在 F# 中做同样的事情。在 F# 库中创建一个名为 `Node2D.fs` 的新文件，并填充以下内容：

```F#
namespace FSharpScripting

open Godot

type MyNode () =
    inherit Node2D()
    
    override this._Ready() =
        ()
        
    override this._Process(delta) =
        ()
```

然后回到 C# 文件并对其进行更改，使其仅调用 F# 代码：

```c#
using FsharpScripting;

public partial class node_2d : MyNode { }
```

完成！现在，您可以返回 Godot 并播放场景，它将构建并播放而不会出错，尽管由于还没有逻辑，所以不会发生任何事情。从这里开始，你的 MO 将把 C# 脚本附加到节点上，这些 C# 脚本将简单地扩展和调用具有真正逻辑的 F# 文件。



# 在 Godot 4 Alpha 中的 .NET 6

https://blog.bencope.land/net-6-in-godot-alpha/

**[Ben Copeland](https://blog.bencope.land/author/ben/)**

Jun 8, 2022

根据[最近的消息](https://godotengine.org/article/dev-snapshot-godot-4-0-alpha-9?ref=blog.bencope.land)，看起来 Godot 4 alpha 版即将获得 .NET 6。这太令人兴奋了。我期待着尝试一下，最终可能会做一份关于在 Godot 4 中设置 F# 的指南。我特别想看看苹果硅芯片（Apple silicon）上的一切工作得如何。



# Godot 和 F#

https://blog.bencope.land/godot-and-f/

**[Ben Copeland](https://blog.bencope.land/author/ben/)**

May 31, 2020

周末，我用 Godot Mono 和 F# 开始了一场游戏。它运行得很好，尽管它依赖于使用 C# 来扩展 F# 中定义的类型。从一开始，我就采用了一种惯例，即定义一个名称空间只是为了定义一个可以由 C# 扩展的类型（类），然后将该名称空间内的所有逻辑抽象到其他 F# 模块中，在那里我尽最大努力坚持函数式原则。



# ----------[Double-oxygeN 日语博客]----------



# 继续 F# 中的 Godot 教程

https://zenn.dev/double_oxygen/articles/8a6a51fc677ba9

**F#でGodotのチュートリアルを進める**

2024/01/21に公開

[Double-oxygeN](https://zenn.dev/double_oxygen)

## 目标

在这篇文章中，目标是在 [Godot Engine](https://godotengine.org/) 4 + F# 中完成 [2D 游戏的教程](https://docs.godotengine.org/en/stable/getting_started/first_2d_game/index.html)。介绍与在 C# 中实现时的不同之处，以及实现时容易沉迷的地方等。实现案例在 GitHub Gist 中公开。

[Godot 4 + F#: 2D game official tutorial](https://gist.github.com/Double-oxygeN/0529a3b30348aad6f30d4f5912e021ad)

## 环境

- Godot Engine (.NET): 4.2.1
- .NET SDK: 6.0.126
  - 查看 Godot Engine 的相应 SDK 版本（GitHub）

编辑器推荐 Visual Studio Code。

## C# 与 F# 的协作

制作 Godot 项目，制作想写脚本的场景后，首先用 C# 制作脚本，然后制作 F# 的项目。

```shell
dotnet new classlib --language 'F#' --name FS --output lib --framework net6.0
```

编辑 F# 的项目文件（这里是 `FS.fsproj`），Project 的 SDK 属性更改为 `Godot.NET.Sdk`：

FS.fsproj

```xml
<Project Sdk="Godot.NET.Sdk/4.2.1">
```

编辑 C# 的项目文件（`*.csproj`），参考 F# 的项目：

Tutorial2D.csproj
```xml
 <!-- Add below in <Project></Project> -->
  <ItemGroup>
    <ProjectReference Include="lib/FS.fsproj" />
  </ItemGroup>
```

到此为止完成后，回到 Godot Engine，进行一次项目的构建。

现在已经准备好用 F# 写脚本了🎉

## F# 的写法

Godot 的脚本用 F# 写的时候，大致分为两种方法，试着比较一下各自的写法。

### 实现为抽象类，由 C# 继承

和在 C# 中写脚本一样，在 F# 中制作继承 Godot 类的类。通过在 C# 中再次继承该类，可以直接使用 F# 中写的实现。另外，请注意，由于 C# 中必须作为部分类实现的关系，需要明确调用父类的实现。

SomeNode.fs
```F#
namespace FS
open Godot

[<AbstractClass>]
type SomeNode () =
    inherit Node ()

    override self._Ready () =
        // Implement something
        ()

    override self._Process (delta: float) =
        // Implement something
        ()
```

SomeNode.cs
```c#
using Godot;

public partial class SomeNode : FS.SomeNode
{
    public override void _Ready()
    {
        base._Ready(); // Must explicitly call the base implementation
    }

    public override void _Process(double delta)
    {
        base._Process(delta); // Must explicitly call the base implementation
    }
}
```

- 优点
  - 大部分实现都可以用 F# 写
  - 可以用与 C# 相同的写法实现
- 缺点
  - 需要在C#中显式调用父类实现
  - 自定义信号的定义等需要用 C# 来写

### 作为函数组实现，在 C# 的实现内调用

用 F# 定义状态的类型，用函数实现状态迁移。状态本身用 C# 管理，通过调用 F# 的函数使状态变化，在 F# 中可以保持函数的纯粹性。

SomeNode.fs

```F#
namespace FS

module SomeNode =
    type State = {
        foo: int
        bar: string
    }

    let Init () = {
        foo = 0
        bar = ""
    }

    let CountUp (state: State) =
        { state with foo = state.foo + 1 }
```

SomeNode.cs
```c#
using Godot;

public partial class SomeNode : Node
{
    private FS.SomeNode.State state;

    public override void _Ready()
    {
        state = FS.SomeNode.Init();
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("count_up"))
        {
            state = FS.SomeNode.CountUp(state);
        }
    }
}
```

- 优点
  - 可以保持 F# 的函数的纯粹性，容易追踪状态变化
  - 与继承类的实现不同，不需要显式调用父类的实现那样的工夫
- 缺点
  - 为了保持 F# 的纯粹性，必须用 C# 管理输入输出等 F# 中未定义的许多状态

在这里，用与 C# 的实现差异少的前者的方法进行。后者的方法容易活用 F# 的函数型编程的特征，另一方面，C# 侧的实现容易变得复杂。

## 教程

让我们按照 [2D 游戏的教程](https://docs.godotengine.org/en/stable/getting_started/first_2d_game/index.html)来进行实现吧，从这里开始，我们来列举在进行教程时应该注意的地方。

### Export

可在编辑器中设置值的导出属性必须在 C# 的类内赋予。在 F# 内使用时，首先将变量定义为抽象成员：

Player.fs
```F#
[<AbstractClass>]
type Player () =
    inherit Area2D ()

    // [<Export>]
    abstract Speed: int with get, set
```

接下来，在继承对象 C# 的类中，一边赋予 `Export` 属性一边实现：

Player.cs
```c#
public partial class Player : FS.Player
{
    [Export]
    public override int Speed { get; set; } = 400;
}
```

由此，在 F# 内也同样可以利用被 export 的变量。

### 自定义信号

对于自制信号也一样，需要在 C# 的类内赋予 `Signal` 属性。这种情况下，在 F# 中不能像 `SignalName.Hit` 那样直接参照自制信号的名字。也可以用字符串直接指定，但作为抽象成员，从 F# 也可以参照自制信号的名字

Player.fs
```F#
[<AbstractClass>]
type Player () =
    inherit Area2D ()

    // [<Signal>]
    abstract HitSignal: StringName with get
```

Player.cs
```F#
public partial class Player : FS.Player
{
    [Signal]
    public delegate void HitEventHandler();
    public override StringName HitSignal { get => SignalName.Hit; }
}
```

### 信号处理程序

信号处理程序也需要在 C# 的类内重新定义。

因此，作为抽象方法的默认实现，如下所示：

Player.fs
```F#
[<AbstractClass>]
type Player () =
    inherit Area2D ()

    abstract OnBodyEntered: Node2D -> unit
    default self.OnBodyEntered _ =
        self.Hide()
        self.EmitSignal(self.HitSignal) |> ignore
        self.GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true)
```

Player.cs
```c#
public partial class Player : FS.Player
{
    public override void OnBodyEntered(Node2D body)
    {
        base.OnBodyEntered(body);
    }
}
```

### 异步处理

在 C# 中，可以利用 async/await 实现异步处理。在 F# 中，代替使用 [task 式](https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/task-expressions)。

在这里，必须说明 C# 和 F# 中的 await 可能式的想法的不同。在 [C# 的语言规格](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#12982-awaitable-expressions)中，await 可能式定义如下：

- Await 可能的表达式具有 `GetAwaiter()` 方法，其返回值的类型满足以下条件（**awaiter 类型**）：
  - 安装 `System.Runtime.CompilerServices.INotifyCompletion` 接口
  - 具有布尔型 `IsCompleted` 属性
  - 具有 `GetResult()` 方法

另一方面，F# 中 await 可能式的[准任务值](https://github.com/fsharp/fslang-design/blob/main/FSharp-6.0/FS-1097-task-builder.md#binding-to-task-like-values)定义如下：

```F#
 member Bind...
     when  ^TaskLike: (member GetAwaiter:  unit ->  ^Awaiter)
     and ^Awaiter :> ICriticalNotifyCompletion
     and ^Awaiter: (member get_IsCompleted:  unit -> bool)
     and ^Awaiter: (member GetResult:  unit ->  ^TResult1)
```

值得注意的是，awaiter 型应该实现的接口不同，C# 中是 `INotifyCompletion`，F# 中是 `ICriticalNotifyCompletion`。Godot 中定义了相当于 awaiter 型的 `IAwaiter` 接口，但这只实现了 `INotifyCompletion`，**F# 中不能进行 await**。

> **[godotengine/godot/modules/mono/glue/GodotSharp/GodotSharp/Core/Interfaces/IAwaiter.cs](https://github.com/godotengine/godot/blob/4.2/modules/mono/glue/GodotSharp/GodotSharp/Core/Interfaces/IAwaiter.cs)**
>
> Lines 1 to 25 in 4.2

```F#
using System.Runtime.CompilerServices;

namespace Godot
{
    /// <summary>
    /// An interface that requires a boolean for completion status and a method that gets the result of completion.
    /// </summary>
    public interface IAwaiter : INotifyCompletion
    {
        bool IsCompleted { get; }

        void GetResult();
    }

    /// <summary>
    /// A templated interface that requires a boolean for completion status and a method that gets the result of completion and returns it.
    /// </summary>
    /// <typeparam name="TResult">A reference to the result to be passed out.</typeparam>
    public interface IAwaiter<out TResult> : INotifyCompletion
    {
        bool IsCompleted { get; }

        TResult GetResult();
    }
}
```

因此，首先要定义一个辅助类，以便在 awaiter 中实现 `ICriticalNotifyCompletion`：

TaskUtils.fs
```F#
namespace FS
open Godot
open System.Runtime.CompilerServices

module TaskUtils =
    type CriticalAwaiter<'a, 'b when 'a :> IAwaitable<'b> and 'a :> IAwaiter<'b>> (awaiter: 'a) =
        interface ICriticalNotifyCompletion with
            member self.OnCompleted continuation = awaiter.OnCompleted continuation
            member self.UnsafeOnCompleted continuation = awaiter.OnCompleted continuation

        member self.IsCompleted
            with get () = awaiter.IsCompleted

        member self.GetResult () =
            awaiter.GetResult ()

        member self.GetAwaiter () =
            self
```

在此基础上，可以实现以下异步处理：

HUD.fs
```F#
type HUD () =
    inherit CanvasLayer ()

    member self.ShowGameOver () =
        task {
            self.ShowMessage "Game Over"

            let messageTimer = self.GetNode<Timer>("MessageTimer")
            let! _ = self.ToSignal(messageTimer, Timer.SignalName.Timeout) |> TaskUtils.CriticalAwaiter

            let message = self.GetNode<Label>("Message")
            message.Text <- "Dodge the Creeps!"
            message.Show()

            let! _ = self.ToSignal(self.GetTree().CreateTimer(1.0), SceneTreeTimer.SignalName.Timeout) |> TaskUtils.CriticalAwaiter
            self.GetNode<Button>("StartButton").Show()
        }
```

注意以上内容，可以完成 2D 游戏的教程。

## 总结

介绍了用 F# 进行 Godot Engine 4 的 2D 游戏的教程的方法。F# 也可以和 C# 一样写脚本，但是为了和 C# 合作，有几个需要考虑的地方。



# ----------[FirstJohn 俄语博客]----------



# C# 下的入侵. 把 F# 拖到 Godot

https://habr.com/ru/companies/first/articles/806145/

**[FirstJohn](https://habr.com/ru/users/FirstJohn/)**
2024年4月9日11:23

朴素，18分钟，阅读 2.6K
[FirstVDS 公司博客（Блог компании）](https://habr.com/ru/companies/first/articles/)，[.NET*](https://habr.com/ru/hubs/net/)，[游戏开发（Разработка игр）*](https://habr.com/ru/hubs/gamedev/)，[F#*](https://habr.com/ru/hubs/fsharp/)，[Godot*](https://habr.com/ru/hubs/godot/)



> - 你去过战斗任务吗？
> - 你什么意思？
> - 一次水下入侵，目的是占领一支精英部队占领的要塞，这支部队拥有 15 枚 VX 毒气导弹。

`Godot` 是一个游戏引擎，支持 `dotnet`。不幸的是，这种支持被限制在 C# 下，以至于 F# 从侧面出来。几乎所有的问题都是可以解决的，但由于缺乏经验，它们会滚到地牢入口附近的一个大橡皮毛辊中，有时会导致过早和毫无意义的死亡。为了避免这种情况，我将在本文中给出一个程序，它将允许您在 Godot 中生存，但不会最大限度地挤出它。这并不意味着 `F# + Godot` 没有自己的面包屑。我只是想先把所有的苍蝇集中在一个地方吃，然后再以更自由的方式做肉饼。我还假设 F# 新手和 Godot 新手都会遇到这篇文章，所以我会在一些地方重复基本教程。

![img](https://habrastorage.org/r/w1560/webt/fk/w6/ke/fkw6kekyr_76wrtvmqxlqzdvmvs.png)

在业余游戏玩家领域，我厌倦了无聊，试图通过将玩家角色转变为游戏开发者来娱乐自己。这是一个没有远大目标的爱好，我还没有计划自主进入游戏开发者。

![img](https://habrastorage.org/r/w1560/webt/mx/hn/h8/mxhnh8zalvyyfm98g4ogdhqerni.png)

我喜欢有时模拟桌面游戏中的机制，因为它在描述商业逻辑时很有用。如果我们谈论的是一些 filler，有足够的内置 F# REPL 来管理游戏的状态。在更复杂的情况下，以 Avalonia/WPF 的形式使用 UI。我对 Unity 很熟悉，对 Monogame 也很熟悉，但在使用它们时，我从来没有觉得有能力完成 PET 项目。我对 Godot 的期望是类似的，但事实证明，这个引擎有一套我需要的现成组件，具有非常高的渗透性。是的，F# 集成有问题，但这些是常数，随着代码库的增长，我越来越不担心。因此，Godot 在 REPL 和 Avalonia 之间的某个地方变得更加复杂，在表现力上超过了任何 UI 框架。

## 在 Godot 中支持 .Net

Godot 以前使用的 `Mono` 比 WinXP 下的 UI 项目更受欢迎。从版本 4.0 开始，Godot 已经**正式**升级到 `.NET 6+`，但一些工具的惯性名称继续谈论 `Mono`。在 .Net Godot 其他事项内置为 SDK，可以从 Nuget 网站下载，无论是完整的还是包的。`Godot.NET.SDK` 取代了违约的 `Microsoft.NET.Sdk` 允许您访问 Godot 的所有功能。它类似于早期版本的 WPF 连接（也适用于 `.NET Core`）。但是，与 WPF 不同的是，您不能只使用 `dotnet` 项目编写 Godot 应用程序。

Godot 有自己的编程语言 `GDScript`，它被认为是主要的编程语言，而 `dotnet`（尤其是 C#）仅作为可选插件提供。我还没有看到 `GDScript` API和 `dotnet` API 之间有什么实质性的区别，也没有看到它们的意识形态背景。我们拥有与母语相同的访问权限，但无论是在那里还是在那里，我们都无法完全控制外部执行环境。我们可以自由地运行文件系统，使用 `Hopac`，`Garnet`，`Hedgehog` 和任何东西，只要轴允许。但是，引擎不会通过条件应用程序 `App.run()` 对于最终产品来说不是问题，但我们再次选择了无缝 REPL。从技术上讲，我们可以在交互中连接所有必要的包，但大多数对 Godot 对象的访问都会导致访问错误和其他 `nativeptr`。

在 4.0 版本中，移动设备上的 `.NET` 应用程序临时丢失，但在 4.2 版本中，它以实验星号返回。这不是一个平庸的再保险，因为我已经在 Android 上遇到了一些网络问题，导致我完全转向 `Godot.HttpClient`. 开发人员[承诺](https://github.com/godotengine/godot/issues/84559)稍后会修复它，但目前的 Workaround 已经足够了。我们无法访问应用程序的 web 版本，根据[引擎开发人员的说法](https://godotengine.org/article/platform-state-in-csharp-for-godot-4-2/#web)，这一点不太可能改变，除非 `dotnet` 内部学会将 WASM 作为非核心模块。

## 项目部署和编辑器配置

Godot [在这里](https://godotengine.org/download/windows/)可用。默认情况下，它不包括 `dotnet` 支持，并且以后可能无法下载，因此最好立即下载 `Godot 引擎 - .NET`. Godot 没有安装程序，可以直接从文件夹中运行下载的 150 MB。在我所有的公司中，我都以**相同的**方式运行 Godot 项目引擎和存储库（原因如下）。

启动编辑器后，您将看到一个标准面板，其中包含以前打开的项目等。页：1 与 VS Godot 不同，Godot 不会为新项目创建单独的文件夹，因此最好在发布共享目录之前自行创建。内置 `gitignore` 支持对 `dotnet` 或 Visual Studio 的特殊性一无所知，因此更容易取消选中它的勾选，使用标准的 `dotnet new gitignore` 并添加 `.godot/*`。

打开项目后，可以调整编辑器的常规设置。例如，通过将`编辑器 -> Editor Settings -> Dotnet -> Editor -> External Editor` 设置为所需状态，可以立即将 C# 脚本编辑切换到 Visual Studio/Rider。事实上，在一半的情况下，Godot 将继续使用内部编辑器，因此并行编辑源代码的问题仍然存在，并询问如何`保存` VS `重新启动`。如果您计划在 Android 上构建一个项目，您可以将 Godot 与已安装的 SDK 和 `"debug.keystore"` 友好地 `导出到 -> Android`。例如，如果您的计算机上已经安装了带有移动开发扩展的 Visual Studio，则会出现以下情况：

```properties
export/android/android_sdk_path="C:/Program Files (x86)/Android/android-sdk"
export/android/debug_keystore="C:/Users/<UserName>/AppData/Local/Xamarin/Mono for Android/debug.keystore"
```

Godot 有自己的格式来描述项目（`project.godot`）、资源（`.tres`）、场景（`.tscn`）、编辑器本身的设置，也许还有一些我还不知道的东西。这种格式是人为的，如果需要，可以手动修改这些文件。我喜欢它，因为它可以有意义地阅读评论。但是，如果我理解正确的话，Godot 并没有为这种格式提供现成的 API。我也找不到清晰的公共软件包。

项目的条件主文件是 `project.godot`，主要存储全局项目设置、屏幕分辨率、主场景、`dotnet` 设置等。页：1 它没有列出正在使用的文件，这意味着项目文件夹中的所有内容都属于它。Godot 只有在至少添加了一个 `.cs` 文件或直接给出 `项目 -> Tools -> C# -> Create C# Solution` 命令后才会添加 `.csproj`。`.csproj` 和 `.sln` 文件将位于 `project.godot` 旁边的根文件夹中。但是，在此之前，我强烈建议更正 `dotnet` 项目的名称（该名称用于盐水）。您可以通过“`项目 -> 项目设置 -> Dotnet -> 项目 -> Assembly Name`”菜单或 `.godot` 源代码中的直接说明来完成此操作：

```properties
[dotnet]

project/assembly_name="SomeName.WithoutSpaces"
```

如果不这样做，默认情况下 `Godot` 会在单词之间划出空格，您将获得带有空格名称和碎片 `camelCase` 的 `.sln` 和 `.csproj` 文件。同时，项目中的名称空间将没有空格，DevOps 阶段的名称差异将需要手动解决。

## 连接 F#

接下来，您需要在 IDE 中打开 `.sln` 并将 F# 类库添加到解决方案中（我更喜欢将其命名为 `<ProjectName>.Core`）。新项目需要更改 SDK，只需从 Godot 打开 `.csproj` 并将 C# 项目的标题复制到 F# 即可：

```xml
<Project Sdk="Godot.NET.Sdk/4.2.1">
	<PropertyGroup>
		<TargetFramework>net6.0</TargetFramework>
		<TargetFramework Condition=" '$(GodotTargetPlatform)' == 'android' ">net7.0</TargetFramework>
		<TargetFramework Condition=" '$(GodotTargetPlatform)' == 'ios' ">net8.0</TargetFramework>
```

然后将 C# 项目引用到 `<ProjectName>.Core`.

```xml
<ItemGroup>
	<ProjectReference Include="ProjectName.Core\ProjectName.Core.fsproj" />
</ItemGroup>
```

在这里，你应该立即记住，Godot以一种非标准的方式增加依赖性。通常，`dotnet` 在没有任何人帮助的情况下隐式地包含依赖项目包，但 Godot 在这个过程中崩溃了。添加到 F# 项目中的所有包都必须显式添加到 C# 项目中，否则构建将失败。我没有检查 `C# -> C#` 粘合剂的这种特殊性，但它确实与我们有关。

## 调试和配置启动

我通常很少使用 Debag，但 Godot 在 REPL 中不可用，而且对我来说是一个新的环境，所以调试可能是有用的。Godot 编辑器连接到正在运行的场景，但 F# 无法加载。但是，它允许您查看当前场景树（通过“`场景”面板 -> 远程选项卡`），我强烈建议您注意这一点。它是为 Godot 设计的。Node（及其继承者）只覆盖导出的属性（在编辑器中可见）。但是，如果你愿意，你可以磨练技术，形成你自己的自定义节点，并将必要的信息存储在它们中。

F# 必须使用基本 IDE 进行调试。首先在 Godot 中运行场景，然后从 IDE 连接到正在运行的进程。特别是在 VS 中，此项目隐藏在 `调试 -> 加入进程中...` 这不是最方便的方法，但它是有用的，如果你突然使一个组件处于非致命的状态，你需要不停地检查它的内部数据。

如果您知道要卸载，您可以提前准备连接信息。如果您通过调试进入 VS `错误 -> <ProjectName>项目的调试属性`，将打开一个窗口，您可以在其中添加新配置文件并启动外部可执行文件等。D.其他事项当我们翻译成俄语时，我们可以配置控制台调用某个 `.exe` 文件而不是标准运行（这对库不起作用），然后按 `F5` 将启动场景，而不是错误消息。我们需要指定可执行文件路径、调用参数和工作目录。所有这些 VS 设置都存储在 `Properties/launchsettings.json` 文件中，您可以自己填写。

如果调用 `--help` 参数的编辑器（即 `Godot_v4.2.1-stable_mono_win64.exe --help`），您可以获得大量的帮助，并确保 Godot 在 DevOps 中完全可用。我们更关心发射设置。例如，通过 `--editor`，您可以在编辑器中打开项目，并通过 `Relative/Path/To/Scene.tscn` 启动特定场景。有很多选项，只有在有一个特定的任务时才能沉浸在其中。我只限于一个小[脚本](https://github.com/StrigoEnTurbano/Article6/blob/main/GodotFSharp.Core/PrepareLaunchSettings.fsx)，它生成：

- 运行 Godot 编辑器的配置文件，
- 违约场景，
- 以及项目中的每一个场景。

```F#
[
    Profile.RunEditor
    Profile.RunGame

    let scenes =
        System.IO.Directory.EnumerateFiles(
            godotProjectDirectory
            , "*.tscn"
            , System.IO.SearchOption.AllDirectories
        )
    for fullPath in scenes do
        System.IO.Path.GetRelativePath(godotProjectDirectory, fullPath)
        |> Profile.RunScene
]
```

配置文件解释如下：

```F#
[<RequireQualifiedAccess>]
type Profile =
    | RunEditor
    | RunGame
    | RunScene of ScenePath : string

    with
    member this.CommandLineArgs = [
        "--path"
        "."
        match this with
        | Profile.RunEditor ->
            "--editor"
            //"--rendering-engine"
            //"opengl3"
        | Profile.RunGame -> ()
        | Profile.RunScene path ->
            path
        "--verbose"
    ]
    member this.Name =
        match this with
        | Profile.RunEditor -> "Godot Editor"
        | Profile.RunGame -> "Godot Game"
        | Profile.RunScene path -> $"Godot {path}"
```

这个代码是元素的，我认为如果你想显示导航区域/路径等，它很容易修改。D.其他事项其余的代码更为例行公事，最好直接从源代码中查看。

该脚本有一个缺陷，因为它生成的配置文件取决于特定机器上 `Godot_v4.2.1-stable_mono_win64.exe` 的位置。我使用相同的项目布局和引擎布局，但配置文件有绝对的路径，不允许在公司之间无痛地搜索 `Properties/Launchsettings.json`，从而使后者进入 `.gitignore`。如果你愿意的话，这种手工艺可以通过在 `Environment.Path` 中注册 Godot 来治愈。

```yaml
Godot:
- Engines: // Папка с различными версиями godot.
  - Godot_v{godotVersion}-stable_mono_win64
    - Godot_v{godotVersion}-stable_mono_win64.exe
- Projects: // Папка с проектами.
 - ProjectName:
   - ProjectName.csproj
   - ProjectName.sln
   - ProjectName.Core: 
     - ProjectName.Core.fsproj
     - PrepareLaunchSettings.fsx
   - Properties: // Изначально может отсутствовать.
     - launchSettings.json
```

因此，我的项目开幕如下：

0. 下载存储库；
1. 在 VS 中打开盐水室；
2. 运行 `PrepareLaunchSettings.fsx` （如果配置文件自上次运行以来没有过期，则跳过）；
3. 选择 `Godot Editor` 作为启动配置文件。
4. 启动项目**无需调试**！

因此，我同时运行现成的 VS 和 Godot。请注意，默认情况下，场景只在一个编辑器中调试，而不是在两个编辑器中调试。如果场景是从 VS 运行的，那么它在 Godot 中无法调试，反之亦然，在 Godot 中运行的场景在 VS 中无法调试（除了上面的屏幕对给出的附加案例）。从 VS 和 Godot 场景连续调试 Godot 编辑器不会产生任何效果，因为 VS 不会听孙子的话。此脚本仅用于测试 Godot 图标。

![img](https://habrastorage.org/r/w1560/webt/qv/tj/38/qvtj38wg0lwmuchcmadajbfqmhg.png)

## 第一个场景脚本和运行机制

在 Godot 分类设备中，脚本是任何带有 `GDScript`/C# 场景/节点类代码的文件。这是一个常见的 `.fs` 类似物，而不是 `.fsx`。由于 `.fsx` 脚本将不再被提及，所以我决定保留 Godot 术语。

默认情况下，Godot 中的场景没有脚本（“代码背后”）。您可以通过单击相应的按钮连接它们。这个脚本不附在舞台上，而是附在舞台上的任何节点上。如果每个节点最多有一个脚本，则场景中可能有许多脚本。我更喜欢一个附在根节点上的整个场景脚本，这使得这个过程类似于经典的基于 XAML 的应用程序，但从 Godot 的角度来看，这种方法不是教规。

此外，附加脚本可以使用任何支持的语言（基本上是 GDScript 和 C#，但也有一些 `GDExtensions`）。也就是说，在一个场景中，可以有多个脚本附加到不同的节点上，并用不同的语言编写。这需要 Godot 的作者做出一些努力，但也允许用户随着项目的发展而克服复杂性。我们必须明白，F# 的整合目前是如此的繁琐，以至于大部分行李都会丢失，需要自己补偿。我对这一事实并不感到紧张，甚至高兴，因为 F# 比竞争对手更好地将信息从一个地方拖到另一个地方。

我的算法（强调主观性而不是版权）创建场景并连接 F#：

1. 让我们创造一个舞台。
2. 在场景中，我们创建了一个必要类型的根节点，称为 `MySceneName`。
3. 我们将它以相同的名称保存在同一文件夹（`MySceneName/MySceneName.tscn`）中。
4. 连接到 C#（`MySceneName.cs`）上的根节点，拒绝模板，保存在同一文件夹中。
5. 打开 VS，在 F# 项目中创建 `MySceneName.fs` 文件。
6. 在文件中声明顶级模块 `ProjectName.Core.MySceneName`
7. 打开 `Godot`
8. 声明从项目 2 继承的 `Main` 类型。
9. 用塞子覆盖 `_Ready` 方法（或 `Hello world!`）。
10. 我们构建一个项目，以便在C#中运行新创建的模块、类型和方法。
11. 打开 VS `MySceneName.cs`，用 `ProjectName.Core.MySceneName.Main` 替换祖先类型。
12. 用调用祖先方法的违约实现覆盖 `_Ready` 方法。
13. 准备好了

结果代码 F#：

```F#
module ProjectName.Core.MySceneName

open Godot

type Main () =
    inherit Node2D()

    override this._Ready () =
        GD.Print "Hello world!"
```

C# 代码：

```c#
using Godot;
using System;

public partial class MySceneName : ProjectName.Core.MySceneName.Main
{
	public override void _Ready() => base._Ready();
}
```

C# 类型标记为 `partial`。这是 Godot 的一个强制性要求，没有它，项目就不会编译。这是因为 `Godot.SourceGenerators` （与 `Godot.NET.Sdk` 一起使用）必须创建更多的文件，其中指定的类型将通过 `partial` 由实例交互方法（例如 `HasMember`/`CallMember`）扩展。

Godot 根据其对世界的投射来决定挑战。如果这个投影没有提到 `_Ready`, `_Process` 等方法。D.从引擎的角度来看，它们是不存在的。这并不意味着您在 dotnet 代码中调用 `_Ready()` 时会遇到任何 `MissingMemberException`。这样的挑战不会出错。这意味着，当引擎将给定的节点放入树中时，它不会从树中看到 `_Ready` 方法，并将通过忽略其调用来节省纳秒。如果从 `GDScript` 调用 `_Ready` 方法，也应该发生同样的情况。在 `HasGodotClassMethod` 和 `InvokeGodotClassMethod` 内部的某个地方，呼叫将失败。

Godot 对 F# 一无所知。生成器不知道如何编写 `.fs` 文件，也没有人调用它们。在 F# 结果中，类型没有相互作用的方法。Runtime 引擎不识别节点中的方法，也不需要访问 `dotnet`。

因此，`C# -> F#` 链接不是因为我们不能绑定到 F# 脚本节点（尽管这也是事实），而是因为我们不能在 F# 类型上使用内置生成器。我试着自己复制生成器，但事实证明，F# 中的一些低级功能根本无法在语言级别启动。这一点不会改变，因为官方的立场 F#（由我们的赛姆在 Ishuev 系列中表达的 Svetoch）是：“这是吗？去吧。».

生成结果可以通过 `ProjectName -> Dependence -> Analytics -> Godot.SourceGenerators -> _ -><type name>_<生成器后缀>generated.cs` 查看。代码是相当愚蠢的，如果一切正常，那么系统地查看它是没有意义的。所有这些都归结为在另一种表示法中描述类型，就像我们禁用类型并在 `json` 方案中表达它们一样。我没有注意到任何点，你可以抓住和堆积任何东西。这比依赖性更容易。

此外，即使我们能够将生成重写为 F#，也会出现缺少 partial 的问题。我不想在一个文件中巧妙地将手写源代码与机器源代码结合起来，因为引入额外限制的风险太高。我也不想生成一个后代（或一个祖先，基于“后记”），因为在这种情况下，我们仍然有一个额外的类型失去了 C# 继承人，因为在 C# 上生成一个封装类型更容易，然后官方的 Godot 生成器会自行配置所有必要的绑定。

在所有这些之后，当 Godot 将 C# 类型循环时，它将根据其祖先的描述给出一个相互作用的描述。问题是，如果在 F# 中定义了祖先，那么对祖先的描述将是不相关的。可以说，继承链中的所有 F# 类型都将被省略，直到我们遇到下一个 C# 类型。到目前为止，绕过这一限制的最简单方法是重新定义 C# 中的“F# 成员”。它看起来不美观，我个人很害怕，但它解决了我的任务。

总之，目前最好使用 C# 作为类固醇上的 DSL。我们不在 `.cs` 文件中编写任何逻辑，只指定现有成员。一种尽可能机械化的方法，将来可以被我们的代码生成器所取代。

## 互操作的一般原则

Godot 承认，如果 GDScript 或场景（例如，通过订阅信号）需要，它有时会调用我们类型的常规不可探索的方法和属性。为了更快，呼叫不是通过反射进行的，而是通过预先准备好的合同进行的。合同的一部分通过祖先合同的金额收取。其余部分由生成器根据类型中定义的属性和方法形成。生成器将为遇到的每个成员创建包装，如果他们认为签名可以在引擎可用的类别中描述。这个列表比 `GodotObject` 的原语和继承者的集合要宽一些，因为它可以处理抽象的 `enum`，可能还有其他东西。

它似乎只是因为它们存在，而且它们是 `Godot 友好的`，而不是因为它在等待它们。无论如何，我看不出重新定义的方法的合同和同名方法的重叠有什么区别。生成器还忽略 `private` 属性，因此封装失败。Godot 不允许出口。

有趣的是，在我们的例子中，我们从 C# 垫片的存在得到更多的控制，而不是从没有它得到更多的控制。如果我们不复制方法，它将不会出现在合同中，因此引擎将不可用：

```F#
member this.VisibleMethod (input : int) = ()
member this.InvisibleMethod (input : int) = ()
```

```c#
// 请注意“new”这个词。
public new void VisibleMethod(int input) => base.VisibleMethod(input);
```

GDScript 积极使用指纹。很有可能，我们的 Node 的一种方法会从中调用，而不确定她是否真的有这种方法。为此，生成器准备实例的多个互操作方法，其本质如下：

```F#
match methodName, args.Count with
| "SomeMethod", 1 -> 
    Vartiants.toInt32 args.[0]
    |> this.SomeMethod
    |> Variants.fromUnit
// | .. -> ..
| _ -> 
    base.InvokeGodotClassMethod(method, args)
```

在原版中，所有这些都是在 `if`-ах 上描述的，但这两种设计都没有太多的消耗。然而，在一个有数百种服务方法和数千个调用的类中，在某些退化的情况下，匹配可能会影响性能。除此之外，出于这个原因，我倾向于只显示实际使用的成员类型。

区分系统调用和野生调用很重要。`_Process`、`_Draw` 等页：1方法是直接调用的，但某些定时器每次通过 `InvokeGodotClassMethod` 拖动 `_on_enemy_spawn_timer_timeout`。您可以通过 `.generated.cs` 文件中的Debugpoint 验证这一点。

## 导出的 Godot 属性（свойства）和特性（атрибуты）

Godot 允许您将某些类型属性标记为编辑器可用的属性。为此，属性必须用 `Export` 属性标记。将属性挂在F#中是无用的，生成器是盲目的，因此工作方案是宣布具有相同属性的新同名属性。首先在源代码中声明：

```F#
member val Value = 42 with get, set
```

然后在投影中覆盖它：

```c#
[Export]
public new int Value
{
	get => base.Value;
	set => base.Value = value;
}
```

F# 中多余的 `Export` 不会影响结果，所以你可以自己写还是不写。考虑到一个假设的生成器，我创建了一个单独的 `MustBeExportAttribute`，它将 F# 中的属性放在一起。

您还应该知道，`Export` 不会修改源代码，因此没有人插入任何其他 `NotifyPropertyChanged`。我们的附加逻辑也不会被任何人触及，因此可以从那里触发事件/信号。

另一方面，Interop 不允许任何自定义，因此属性的重复允许使用 `DU`、`option` 和 `list` 的转换稍微萨满。这使我们摆脱了一些垃圾，但使 C# 中的逻辑复杂化。在这种情况下，我更喜欢使用带有单独描述模型的适配器。这一点尤其重要，因为编辑器中有可选的可见性自定义选项和属性分组选项，这些选项目前在属性和覆盖之间[扩展](https://docs.godotengine.org/en/stable/classes/class_object.html#class-object-private-method-get-property-list)。

## 充满脚本的场景

在根节点场景的情况下，实例是在 Godot 侧创建的（或者通过 Packedscene 创建的），这几乎自动取代了赤裸的 C# 后代上的野生草原 F# 祖先。在这种情况下，我们不必担心设计师或工厂。然而，在我的实践中，带有 `.tscn` 文件的类型更像是例外，而不是正常。我将重构大量仅以脚本形式存在的类型。所以在 `module ProjectName.Core.MySceneName` 和 `type Main` 之间，可以定义十几种或两种较小的 Godot 类型。其中大约五分之一可能包含重新定义的方法。

不能直接创建它们，因为对于引擎来说，这些实例将是空的。我们再次需要通过生成器磨坊的后代。在 C# 中，DI 将被拧紧，但我不太可能在其他地方使用，所以就我个人而言，我已经足够了，通常的 ~~`IVar`~~ 变异场与工厂直接类型：

```F#
type Minor () =
    inherit Node2D()

    static member val Factory =
        // Func только из-за того, что C# слишком вербозно описывает FSharpFunc и Unit.
        System.Func<Minor>(fun () -> failwith "Factory is empty!")
        with get, set

    static member create () = Minor.Factory.Invoke ()

    override this._Ready () =
        GD.print "Hello from Minor!"
```

假设将使用 `create` 方法代替 `Minor` 构造函数：

```F#
this.AddChild ^ Minor.create()
```

接下来，您必须定义子工厂的初始化：

```c#
public partial class Minor : GodotFSharp.Core.Main.Minor
{
    public static void Initialize()
    {
        GodotFSharp.Core.Main.Minor.Factory = () => new Minor();
    }

    public override void _Ready() => base._Ready();
}
```

最后，在 `Program.Main` 调用 `Minor.Initialize`. 问题是没有 `Program.Main` 不存在，因为 Godot 根本没有给我们通常的输入方法。相反，Godot 有一个`Autoload` 机制，旨在初始化全局 `singleton`。在引擎上下文中，这些节点独立于打开的场景而存在，但始终可以通过 `this.getNode "root/SingletonName"` 访问。它们用于存储全局状态、游戏设置类型、玩家配置文件等。D.其他事项这就解释了为什么我们的任务有一定的缺陷。

我们需要创建另一种节点类型，但现在只在 C# 侧。描述**整个项目**通用的初始化过程，并在 `_Ready()` 方法中调用它：

```c#
public partial class EntryPoint : Godot.Node
{
    private bool Initialized = false;

    private static void Initialize()
    {
        // 提到所有类型。
        Minor.Initialize();
        // 或者用经典的方式来启动 DI。
    }

    public override void _Ready()
    {
        if (!Initialized)
        {
            Initialized = true;
            Initialize();
        }
    }
}
```

在godot编辑器中打开项目->项目设置…选择“启动”选项卡，打开entrypoint.cs文件并将其添加到全局变量列表中。现在，无论您选择的场景如何，都将创建EntryPoint，它将自定义Minor和其他类型。

## 代替信号

Godot 中的信号起着事件的作用，但它们的实现和使用与我们通常使用的 `IEvent<_,_>` 非常不同。而不是在选定的对象（如 F#）中表达信号，创作者将它们涂抹在一个实例上，作为一个古老的结构。因此，F# 不能用作原始数据源，必须涂上抽象方法并在后代中实现。一切看起来都很可怕，试图美化它变成了库代码，这是我在本文中试图避免的。

我将在以下文章中回到这个话题，现在我只能说：

- 可以而且必须使用现有类型中已经存在的信号。
- 只有当考虑到 GDScript 交互时，才有必要确定信号。
- 在所有其他情况下，最好使用常规事件、`Hopac`、`ECS` 等。D.其他事项

## 结论

我已经强调了 F# 与 Godot 连接的亮点。它是 C# 导向的，但它应该足以让学生们在不发明自行车的情况下通过。基于这些数据的项目的一个小例子在[这里](https://github.com/StrigoEnTurbano/Article6)。

![img](https://habrastorage.org/r/w1560/webt/1d/xr/fz/1dxrfzsrmxsj6msbi1dnv9hx13s.png)

接下来，我将讨论 F# 特定的方法和功能。我不知道它会多快发生，所以我会给你一个提示：

- Godot 充满了 OOP 风格的设计。由于 F# 的多范式性质，它们可以而且应该在盈利的时候使用，并且在业务逻辑的数量或复杂性超过舒适的数量时，必要时丢弃。
- Godot 在语法结构中保留了自己的契约。这是 C#-Way，这是慢性的，不能治疗。幸运的是，我们在 F# 和 C# 之间有一个连接位，可以让这种缺陷保持在边缘。如果你觉得合同的一部分（不是逻辑）开始移动到一个单独的设施，不要抗拒。这可能是因为事物的自然顺序。
- Godot 提倡这首歌，但无论是官方的还是球迷的指导方针，这条线几乎都没有发展。如果你是一个新手，不太了解我们在谈论什么，我建议你首先把这个概念简化为一个实用的格言：**创建比你习惯的更多类型**。这足以让一个平均的场景开始分解成 5-6 个大部件，它们之间有一组连接，它们一起操作一堆较小的类型。我要强调的是，这并不是要打破 Godot 的场景。所有这些组件都位于 `module` 和 `type Main` 之间，只有在必要时才能完全独立。
- 如果你能形成一个合适的 DSL，Godot 编辑器只需要在空间中直观地定位物体，而不是所有的。其他一切都可以在不离开 F# 的情况下完成。

文章作者 [@Kleidemos](https://habr.com/users/kleidemos)

---

UFO 飞到这里，给我们博客的读者留下了一个促销代码：
[-15% 订购任何 VDS](https://firstvds.ru/?utm_source=habr&utm_medium=article&utm_campaign=product&utm_content=vds15exeptprogrev)（加热费除外）- HABRFIRSTVDS

标签：[godot](https://habr.com/ru/search/?target_type=posts&order=relevance&q=[godot])，[f#](https://habr.com/ru/search/?target_type=posts&order=relevance&q=[f%23])，[.net](https://habr.com/ru/search/?target_type=posts&order=relevance&q=[.net])，[游戏引擎（игровой движок）](https://habr.com/ru/search/?target_type=posts&order=relevance&q=[игровой+движок])，[游戏开发（разработка игр）](https://habr.com/ru/search/?target_type=posts&order=relevance&q=[разработка+игр])

hub：[公司博客（Блог компании） FirstVDS](https://habr.com/ru/companies/first/articles/)，[.NET](https://habr.com/ru/hubs/net/)，[游戏开发（Разработка игр）](https://habr.com/ru/hubs/gamedev/)，[F#](https://habr.com/ru/hubs/fsharp/)，[Godot](https://habr.com/ru/hubs/godot/)



# 一个 60 岁的囚犯和实验室老鼠。F# 在 Godot 上。第1部分 会见框架

https://habr.com/ru/companies/first/articles/846224/

**[FirstJohn](https://habr.com/ru/users/FirstJohn/)**
2024年9月26日12:16

朴素，16 min，2.8K 阅读

[公司博客（Блог компании） FirstVDS](https://habr.com/ru/companies/first/articles/)，[.NET*](https://habr.com/ru/hubs/net/)，[游戏开发（Разработка игр）*](https://habr.com/ru/hubs/gamedev/)，[F#*](https://habr.com/ru/hubs/fsharp/)，[Godot*](https://habr.com/ru/hubs/godot/)



![img](https://habrastorage.org/r/w1560/webt/vy/ta/19/vyta19tblpvyxgfl4gn2dworjbo.jpeg)

[上一次](https://habr.com/ru/companies/first/articles/806145/)，我主要谈到了在尝试将 F# 和 Godot 结合起来时遇到的困难。这是一个被迫的措施，因为我们首先关心的是“标准”的行为，以防非标准和方便的方式失败。可以说，在我们学会投掷和疼痛技巧之前，我们已经学会了在没有严重后果的情况下摔倒。如果我们不想让大多数人在几节课上致残，这是一个正确的步骤，但这不是我们来这里的原因。现在是时候把自己变成例行公事，然后采用更具侵略性的技术了。

## 叙事背景

几周前，就在夏天的烈火中，我又一次染上新冠病毒，感染了我周围的人，在我出去的时候（脸朝地上躺了四天后），我的主动性和工作能力突然发作。在这个过程中，我看到了这一系列的文章：

- [Godot – 没有规则的绘画](https://habr.com/ru/articles/553742/)
- [矩形瓦片（тайловые）世界](https://habr.com/ru/articles/554960/)
- [六角形瓦片世界](https://habr.com/ru/articles/557496/)
- [没有疼痛的瓦片](https://habr.com/ru/articles/569186/) - 不完全在循环中，但接近。

文章使用 `GDScript`，但我用 F# 重写了所有内容，去掉了正确的部分，清理了它，简化了它，并用它作为例子。好的材料是好的，有很多，在 F# 的情况下，它产生了足够多的有趣的案例，同时你想引起注意。另一方面，生成的对象区域很少与用户交互，很少从引擎中提取，并且具有不需要清理的状态。这相当于一个教学控制台项目，几乎没有魔法。

在标题循环的第一部分，简要地解释了通过 `RenderingServer`（当时称为 `VisualServer`）绘制的机制。同样的机制很可能用于绘制标准节点。无论如何，`RenderingServer` 的分类设备与一般的 Godot-вский 非常相似，因此大脑很容易将一个模型投射到另一个模型上。问题是，这种类型的 API 是如此老式和面向机器的，以至于它需要一个替代的抽象集，类似于现有的抽象集。需要一个很好的理由来选择 `RenderingServer` 而不是标准节点。然而，这是我们的基础，对于那些需要绘制大量、更薄、更有效的标准节点的人来说，这是一个很好的切入点。

下一篇文章是用给定的机制绘制的正方形网格。虽然我对六边形和点对点地图更感兴趣，但我认为第二部分是最有意义的，所以所有计划的叙述都将涉及它或更原始的例子。剩下的六边形和照明要么没有带来全新的例子，要么太远了，我无法在出版的这一阶段达到它们。

由于空间限制，我不会重复 [@Goerging](https://habr.com/users/Goerging) 文本，也不会深入研究算法的技术细节，除非它们与语言或引擎无关。我把所有的劳拉都从另一个作者那里拿走了，这样我就可以专注于我感兴趣的细节，事实证明，这些细节太多了。

我在文章中看到的项目越多，我想使用的技巧就越多。但通常情况下，它们要么必须被拒绝，要么必须以某种方式评论，否则只有极少数人能够理解因果关系。然而，在 Godot 中，我看到了一个非常吸引人的 F# 利基，所以这次我试图从 F# 的位置构建整个循环，F# 是一个拥有未知引擎水平的新手。这是一个缓慢的过程，特别是在第二和第三部分，但我想把所有的业余爱好者从索哈（сохи）到核弹在没有重大冲击的情况下。

## 实验室实验

在互联网的某个地方，有人说 F# 是数学家解决数学问题的语言。我不知道这个想法是从哪里来的，但社区每年都会遇到这个概念的辩护者，然后在个人资料聊天中分享他们收到的珍珠。不能说我们没有高额头的数学家，但他们和其他工业语言一样难以获得。另一方面，游戏中的数学可能比普通学科领域要多得多，所以你应该注意现有的库存。

如前所述，Runtime Godot 的特性剥夺了我们的 `REPL` - A（`read-eval-print loop`）。我失去了它，但事实证明，它不是。Godot 中的大多数矩阵类型，如向量、矩阵等。D.它不需要与引擎内核接触，因此它们仍然可以通过 REPL 工作。连接正确的数据包，您可以检查假设：

```F#
#r "nuget: Godot.CSharp"
#r "nuget: Hedgehog"

open Godot
open Hedgehog

let inline (^) f x = f x

Property.check ^ property {
    let! point, offsetBefore, offsetAfter =
        [-100f..100f]
        |> Gen.item
        |> Gen.tuple
        |> Gen.map ^ fun (x, y) -> Vector2(x, y)
        |> Gen.tuple3
    let! scale = 
        Range.constant 0.01f 100f
        |> Gen.single
        |> Gen.tuple
        |> Gen.map ^ fun (x, y) -> Vector2(x, y)
    
    let transform = 
        Transform2D
            .Identity
            .Translated(offsetBefore)
            .Scaled(scale)
            .Translated(offsetAfter)
    
    let projected = transform * point
    let reversed = transform.AffineInverse() * projected
    counterexample $"{point} -> {projected} -> {reversed}"
    let delta = (reversed - point).Abs()
    return delta.X <= 0.01f && delta.Y <= 0.01f
}
```

图形和 UI 无法验证，因此我们需要在运行的引擎中运行 REPL。

## 类型扩展

F# `type extensions` 是一种很酷的机制，可以用化妆品技术和特性来补充现有的类型。我把它们称为化妆品，因为它们只能做普通外在功能所能做的事情。没有人会允许存储原始类型中不包含的数据，因为无法添加字段、接口实现或构造函数。您也无法访问受保护的成员。无论如何，这都需要继承。

化妆品会员是一个快速溶解的帮手，将所有内部物流隐藏在舒适的立面后面。F# 中的扩展语法比 C# 中的`[<extension>]` 要简单得多（尽管后者也可以使用）。例如，抛出向量的一个组件的操作可以如下所示：

```F#
[<Auto>] // 打开父模块或空间时自动打开。
module Auto // 必须在模块中

type Vector2 with // 类型扩展语法
    member this.X_ = Vector2(this.X, 0f) // 或者 `this * Vector2.Right`
    member this._Y = Vector2(0f, this.Y) // `* Vector2.Down`
```

您可以在扩展中嵌入许多内容，但更常见的是，它们要么是与某种类型相关的小功能，要么是悬浮在空中的功能，您必须为此启动一个名不明显的模块：

```F#
type RenderingServer with
    static member canvasItemCreateIn parent =
        let res = RenderingServer.CanvasItemCreate()
        RenderingServer.CanvasItemSetParent(res, parent)
        res
```

扩展是与大型外部框架（如 WPF、AvaloniaUI 或 Godot）碰撞时非常重要的特征。我们不需要在类别中讨论它，`我们不会使用 VS`。你一定会的（除非你的任务是在同一家公司找到一个无休止抱怨的理由）。

![img](https://habrastorage.org/r/w1560/webt/fd/3u/s7/fd3us7oeye1chrvvqke3kdzgtxc.png)

您最好小心公约，因为这些扩展将如何放置在项目中。条件实用程序不包括任何东西，也不应该低估定义在尽可能窄的范围内的力量。有时我真的很希望扩展可以在函数内部编写。

## 属性初始化

默认情况下，在语法级别上，F# 中绝大多数声明都意味着不变性（иммутабельность）。这是影响整个系统的一个重要因素，所以有时我们需要证明变异性。

在微观层面（框架不适用）感受到最大的差异。在 F# 中，可突变变量需要一个单独的关键字：

```F#
let mutable a = 42
```

进一步的更改是通过单独的赋值“运算符” `<-` 进行的，该“运算符”专门与比较不同：

```F#
// 赋值 Присваивание
// : unit
a <- 32

// 等值检验 Проверка на эквивалентность
// : bool
a = 32
```

在 F# 中，没有直接对应的 `i++`, `++i`, `+=` 等。D.在 ChatGPT 发明之前，阻碍了算法从其他语言的机械移植。然而，在本地生成的算法中，缺少算子的影响很小，因为它们的利基被更高层次的机制（механиками）占据了（我们将在下面的文章中部分讨论）。

在宏观层面上，F# 中的数据变异几乎没有区别。使用箭头指定属性和字段：

```F#
let camera = new Camera2D()

camera.PositionSmoothingEnabled <- true
camera.PositionSmoothingSpeed <- 4f
```

我们可能有一些问题与任何 `+=`，但 Godot 充满了 `Node2D.Translate` 方法。原则上，通过技术进行的突变越多，语言之间的差异就越小。

有一种特殊的情况，即赋权是通过等号实现的：

```F#
let camera = new Camera2D(
    PositionSmoothingEnabled = true
    , PositionSmoothingSpeed = 4f
)
```

由于 C# 的原因，这个案例与构造函数的初始化错误地联系在一起，但事实上，如果方法返回被变异的对象，则可以在每次使用方法时指定属性（作为未定义函数的值）。传递方法参数即可，然后可以在同一位置指定**返回对象**的属性：

```F#
match List.ofSeq cell with
| [:? Syntax.ParagraphBlock as singleParagraph]->
    singleParagraph.Convert(
        styleConfig
        , SizeFlagsStretchRatio = table.ColumnDefinitions.[index].WidthAsRatio
    )
| unexpected -> ...
```

这个功能与前一个功能结合使用，可以产生有趣的效果。例如，在我们的代码中可以找到这样的方法：

```F#
type Vector2 with
    member this.With () = this
```

从普通人的角度来看，这个代码是荒谬的，因为我们没有做任何编辑，而是返回相同的对象。引用的是 `Vector2` 是一个结构，当结构通过方法的磨坊时，一个新的实例会出现，但数据相同。因此，您可以将上一段的代码重写为：

```F#
type Vector2 with
    member this.X_ = this.With(Y = 0f)
    member this._Y = this.With(X = 0f)
```

我们不需要定义 `With` 方法的参数，因为该语言提供了一个很好的替代方案。这里只有一个危险。你可能不得不向一个不小心上当的人解释这个黑客的装置。在我的实践中，有几次它意外地导致了最残酷的多步愚蠢，所以我不建议在你的每一个结构上铆接 `With()`。此外，对于作为静态构造函数运行的结构，使用同名方法复制静态属性更合理：

```F#
type Transform2D with
    static member NewIdentity () =
        Transform2D.Identity

let shifted = Transform2D.NewIdentity(Origin = 100f * Vector2.One)
```

## 静态解析类型参数（Statically Resolved Type Parameters）

[静态解析类型参数](https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/generics/statically-resolved-type-parameters)（或简称 `SRTP`）-在当前主题的上下文中，可以理解为在编译阶段进行的dactiping（дактайпинг）。它的语法在某些情况下非常复杂，所以最好分开学习。每次使用运算符 `(+)`, `(-)`, `(*)`等时，您都会隐式地遇到它。D.所以你可以看看他们的签名。使用 SRTP，您可以编写更实用的东西。例如，由于 SRTP，我们不需要任何 `System.Math` 具有数千个 `Abs` 方法过载，因为 `abs` 函数在全局范围内可用：

```F#
abs -42 // 42
abs -42f // 42f
abs -42I // 42I
```

`abs` 通过 SRTP 运行。该函数期望一个具有静态 `Abs` 方法的 `'a` 类对象，其签名形式为 `: 'a -> 'a`。所有有符号的数字原语都有这种方法。函数定义如下：

```F#
let inline abs (a : 'a when 'a : (static member Abs : 'a -> 'a)) =
    'a.Abs a
```

在实际意义上，这意味着在 `abs` 中，**任何**满足条件的 `someA` 都可以传递，而不仅仅是一个数字。为了说明这一点，下面给出了一个完全不正确语义的语法正确类型的例子：

```F#
type AbsFriendly = {
    Value : string
}
    with
    static member Abs absFriendly = {
        Value = $"Абузим abs: {absFriendly.Value}"
    }
// type AbsFriendly =
//   { Value: string }
//   static member Abs: absFriendly: AbsFriendly -> AbsFriendly

let before = { AbsFriendly.Value = "Проверка" }
// val before: AbsFriendly = { Value = "Проверка" }

let after = abs before
// val after: AbsFriendly = { Value = "Абузим abs: Проверка" }
```

这当然是卡通。在现实中，只有数字类型被传递到这些函数中，很少有数字类型的自定义组合。例如，在桌面游戏建模中，使用非平凡的资源模型（代币等）可能会响应 `sign`、`abs`、`ceil`、`truncate` 或 `round` 可能会很方便，因为它允许在不相关的类型上简单地定义复杂的 `inline` 函数。在游戏力学方面，计算机游戏远远落后于桌上游戏，但在线性代数方面却远远优于它们。不幸的是，F# 和 Godot 遇到了这个问题。

Godot 有自己的“标准”类型集来表示向量和矩阵，以及部分支持向量行为的伴随类型（例如颜色）。运算符和 fluent 语法被附加到它们上，但不知何故忘记了静态方法。SRTP 看到运算符，它们在整个 `dotnet` 上都是标准的，但它不能基于实例方法来考虑静态方法。使用类型扩展无法解决此问题，因为 SRTP 只关注类型中最初定义的成员。在未来的 F# 版本中，这一点将被修复，但您现在必须与其他 Godot 开发人员一起坐船，或者在向量下定义函数：

```F#
let inline abs' (vec : 'vec when 'vec : (member Abs : unit -> 'vec)) =
    vec.Abs()

abs' -Vector2.One // (1f, 1f)
abs' Vector2I.Up // (0f, 1f)
```

最后，向量有足够的运算，而不是与数字运算相冲突：

```F#
let inline distance (a : 'vec when 'vec : (member DistanceTo : 'vec -> float32)) (b : 'vec) =
    a.DistanceTo b

distance Vector2.One Vector2.Zero // 1.414213538f
distance Vector3.One Vector3.Zero // 1.732050776f
distance Vector4.One Vector4.Zero // 2.0f

// Универсальность типов наследуется новыми функциями за счёт инлайна.
let inline nearestTo me vecs =
    vecs
    |> Seq.minBy ^ distance me
```

## 标记数

[Units of Measure](https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/units-of-measure)（或简称 Measures）最初，这是一个非常实用的简单想法，允许开发人员用测量单位标记所使用的数字：

```F#
type [<Measure>] m
type [<Measure>] s
type [<Measure>] g

// 127_000f<g>
let weight = (105f + 10f + 12f) * 1_000f<g>

// 0.694444418f<m/s>
let speed = 25_000f<m> / 36_000f<s>

// 88194.4375f: float32<g*m/s>
let momentum = weight * speed
```

这些数字可以自然地操作，单位标记的行为方式是预期的。它们还将阻止明显不正确的计算，如米与秒的加法或平方米与立方的加法。限制将涉及函数和属性，直到标记的数字不能与常规插槽匹配，反之亦然：

```F#
let f (x : float32) = ...

// Ошибка компиляции:
f momentum

// Корректные варианты:
f (momentum / 1f<g * m / s>)
f (momentum * 1f<s / g / m>)

let g (x : float32<m>) = ...

// Ошибка компиляции:
g 10

// Корректный вариант:
g 10<m>
```

所有这些魔法只存在于编译阶段，因此单位不会影响性能。问题是，它不可能从一个缺氧（забоксированного）（换算为（приведённого к） `object`）数中找出度量单位，因为从兰特（рантайма）的角度来看，`float32` 和 `float32<m>` 是相同的。这也是 `Option.None` 的问题，它在 Runtime 中等于 null。由于这种不可逆的退化，无论是单位还是 `Option` 都不能完全用于传输 `obj` 的通道。WPF 中的 `DataTemplate` 对 `None` 没有反应，`ECS` 中的系统忽略消息，`DI` 容器记录的值等。D.其他事项在这样的环境中，必须进行再保险，并且仅在具有明确标识的类型中使用这些值。

![img](https://habrastorage.org/r/w1560/webt/vm/6-/rx/vm6-rxsif5wkabr-whc1fxfl19c.png)

游戏是不同的，但具体来说，我的国际 CI（СИ）还不需要。我在短时间内使用商业逻辑中的度量来标记我不想不小心混淆的数字指标。这是一个罕见的情况，通常是一个真正令人讨厌的脱节计算。它们通常处于不同的上下文中，所以我要么根本没有 `<m / s>` 或 `<g * m>` 类型的关联，要么在极少数情况下，它们被绑定在与普通三角洲不匹配的任何 `<localTick>` 上。但是，如果我们谈论图形矩阵，我不介意在向量和矩阵的应用领域上标记它们，比如 `<globalPixel>`、`<localPixel>`、`<tile>` 等。D.其他事项不幸的是，目前唯一一个没有跳舞的度量单位是用数字表示的度量单位。这意味着它们可以很容易地在 CI（弧度或度）和应用区域（场上的角度或屏幕上的角度）中进行属性。你可以用支持单位来编写自己的类型，这样你就可以从长远来看依赖 Godot 向量的犹太复制品，这些复制品将支持上述所有特征（包括 SRTP）。但是，不能将单位绑定到其他类型。

好吧，准确地说，这是可能的，但失去了所有的领带从操作符，函数等。页：1该方法可以在 `FSharp.UMX` 中查看，通过不安全黑客，学会了将单位附加到字符串、GUID、日期等上。D.其他事项问题是，从语言的角度来看，标记为 `<userId>` 的字符串不再是字符串。要知道它的长度，或者与另一个字符串匹配，必须将它转换回正常的字符串。在 UMX 的情况下，这是一个可以接受的牺牲，因为数据包的任务是隔离不同表和实体的标识符，而不是进行密集的数学运算。在我们的例子中，黑匣子的内容和按类型隔离黑匣子一样重要，因此 UMX 脚本不适合我们。

## 单案例标记组合（Однокейсовые размеченные объединения）

既然我们谈到了 `UMX`，那么我们也应该谈谈 `SCDU`（`Single Case Discriminated Union`）的概念。这是另一种建筑学方法，用于将一个原始区域与另一个原始区域隔离开来。它比度量更重，但更常见，因为它可以应用于非数字类型，通常倾向于分离与基本类型行为显著不同的逻辑。

例如，你可能有一个系统，在准备阶段用相对的路径工作，在最后用绝对的路径工作。为了避免意外混合它们的风险，您可以创建两种类型：`RelativePath` 和 `AbsolutePath`，它们将包含相同的字符串，但不能将它们放入为不同类型设计的函数中：

```F#
type RelativePath =
    | RelativePath of string

let handleRelativePath (RelativePath path) =
    // path : string
    ...
```

DU 是非常不同的，因此它们在记忆中的表现具有最大的变异性。猜测是毫无意义的，最好是实地核实。例如，编译器将结构单播 DU 优化到接近其内容的状态。即使在拳击之后，这种类型的实例也不会退化为 `string`：

```F#
[<Struct>]
type FastRelativePath =
    | FastRelativePath of string
```

`SCDU` 可以帮助我们避免不正确的 ID 传输。要做到这一点，只需在业务逻辑级别使用 `SCDU` 而不是裸露的 `Guid`、`int` 和 `string` 即可：

```F#
type MyEntityId =
    | MyEntityId of System.Guid
```

在这一点上，有一个问题是广播不同的 query，这类事情通常会崩溃。为了防止这种情况发生，上述 `FSharp.UMX` 是专门针对标识符编写的。我个人没有拿到这个包裹，但我不能不指出它。我通常不会写域类型的query，所以对于标识符，我更喜欢使用常规记录。它没有带来显著的区别，所以惯性，这件事在集体中也被称为SCDU，最大SCDU-记录。

`RenderingServer` 通过其`资源 ID`（`Rid`）处理对象。有明显不相交的对象类型，其标识符可以按以下方式划分：

```F#
type [<RequireQualifiedAccess>] CanvasItemId = { AsRid : Rid }
type [<RequireQualifiedAccess>] CameraId = { AsRid : Rid }
type [<RequireQualifiedAccess>] ViewportId = { AsRid : Rid }
```

理想情况下，您可以生成 `RenderingServer` 投影，该投影将与自定义标识符一起工作。然而，这些只是跳棋，主要的是将域类型和函数转换为它们。我们的 `DrawMap`、`FillCell` 等D.其他事项接受标识符记录，并在 `RenderingServer` 中传输常规 `Rid`。由于大多数标识符错误发生在远程传输时，因此必须将保护放在通信中，而不是本地功能上。

如果 SCDU 中的值必须严格符合某些定律，则可以将其“构造函数”与内容锁定在一起。F#不承认单独的锁，所以您可以自己卸载数据：

```F#
module DU =
    type Main = 
        private
        | Case of string

        with
        member this.AsString = 
            match this with
            | Case str -> str

    let tryCreate str =
        if (str : string).StartsWith "TILE:"
        then Some ^ Case str
        else None

let (|DU|) (du : DU.Main) = du.AsString

module Record =
    type Main = 
        private { raw : string }
        with
        member this.AsString = this.raw

    let tryCreate str =
        if (str : string).StartsWith "TILE:"
        then Ok ^ Case str
        else Error """Must start with "TILE:"."""

let (|Record|) (record : Record.Main) = record.AsString
```

通过 `option`、`Result` 或 exception 定义工厂是一种开发选择，但它仍然必须考虑到使用范围。

在保护形式中，SCDU 开始与类别和亚类型的概念相结合。在某些人的头脑中，这些术语是完全相同的，在我看来，这是一个严重的错误。从数字到受 SCDU 保护的选项范围是离散的，但选项的频率足够高，可以找到真正需要的选项。在这个地方你可以做的最糟糕的事情是截断所有的流形到两三个模式。

Godot 对小类型的概念并不陌生。最常见的是 `NodePath`，如果它是用 F# 写的，它很可能是用 SCDU 写的。类型-包含少量域功能的字符串上方的包装。这种类型还允许 Godot 编辑器将字段识别为节点地址：

```F#
type NodePath = {
    AsString : string
}
    with
    member this.Names : string list = ...
    member this.SubNames : string list = ...
    member this.IsEmpty : bool = ...
    ...
```

在使用 SCDU 时，您必须充分了解特定类型的传播范围。SCDU 可以作为外部和本地上下文之间的边界上的显式标记，这意味着它们被突出。但要想突出，必须在某处，而不是某处。此外，即使在一种情况下，SCDU 的适用范围也可能有限。例如，读取模型可以是完全开放的，并以常规类型表示，而写入模型（通常是 `Command`）需要受保护的 SCDU。

在这种情况下，至少在抽象的情况下，没有明确的标准。但你越接近具体，你就越没有足够的发言空间。

## 重载是个问题。

Godot-A API 是以 C# 为目标编写的，这使得一些实体的形式与它们的内容无关。例如，在 C# 中，重载非常常见，而在 F# 中，重载非常冷酷（прохладно）。由于当前类型输出的水平，柯里化（каррированных）函数中根本没有这些。方法允许重载，但方法本身并不总是可用的。因此，通常遵循“一个名字一个签名”的规则。

![img](https://habrastorage.org/r/w1560/webt/qa/_c/j_/qa_cj_v7qprr-oggxqeiinugmzk.jpeg)

不喜欢的原因很简单：重载对输出类型没有帮助。典型的情况是，重载密集型模块比项目的其他部分包含更多显式类型指示，并且从未检测到所谓的可读性改进。考虑到这一点，我更喜欢在现有的 API 之上构建自己的 API，而性能考虑不起作用，因为如果需要的话，`inline` 将使这十几个函数完全免费。

在 DSL 类型的情况下，方法过载是合理的。有时，他们会阻碍工作。例如，`Node` 有两种 `GetNode` 方法：

```F#
GetNode<'a> : NodePath -> 'a
GetNode : NodePath -> Node
```

在 F# 中，我们很少明确地将仿制品类型化，编译器为我们这样做。因此，这些方法的视觉应用必须相同。一个广义的版本显然被简化为未公开的版本，因此后者可以被丢弃。但实际上，未公开的版本将具有优先级，因此以下代码将停止编译：

```F#
let draw (drawer : Node2D) = ()
draw ^ this.GetNode pathToDrawer
```

从编译器的角度来看，我们正在尝试将 `Node` 而不是 `Node2D` 嵌入，这肯定是一个错误。为了让编译器理解我们，至少应该这样写：

```F#
draw ^ this.GetNode<_> pathToDrawer
```

这种设计仅仅是因为方法有重载。如果您自己定义以下扩展，则调用不需要任何角括号：

```F#
type Node with
    member this.getNode path = this.GetNode<_>(path)

draw ^ this.getNode pathToDrawer
```

新功能可以进一步修复 Godot 的明显缺陷。出于某种原因，原始 `GetNode<'a>` 并不认为 `'a` 是 `Node` 的继承者，尽管 `AddChild` 和未公开的 `GetNode` 设备表明情况并非如此。我们不太可能专门尝试通过 `GetNode` 获取其他内容，更可能在重构过程中发生这种调用，因为类型通常只从上下文中推断出来。如果编译器知道相关限制，则可以指定错误的查询：

```F#
type Node with
    member this.getNode path : #Node = this.GetNode<_>(path)
```

与 C# 不同的是，我们有 `op_Implicit` 处理脚本。我们通常欢迎这一点，但特别是在 `NodePath` 的情况下，这一点往往会受到阻碍。因此，放弃 `NodePath` 作为默认参数并用字符串替换它是有意义的。路径的版本最好以不同的名称保留：

```F#
type Node with
    member this.getNodeByPath path : #Node = this.GetNode<_>(path)
    member this.getNode path : #Node = 
        use path = new NodePath(path)
        this.GetNode<_>(path)
```

因此，对于已知的路径，我们将使用字符串。在需要多步传输（基础设施代码）的更复杂的情况下，我们将转向 `NodePath`。

## 临时结论

为了方便工作，您应该忽略 Godot 的标准 API 并积极复制它。问题不在于特定的引擎，而在于 C# 的一般意识形态，所以这个结论适用于大多数框架。与初学者的意图相反，这种与 dotnet 社区其他部分的原始联系不会给你带来任何实际好处。

进一步的实际步骤，使引擎压缩，需要处理一些“基本”的 F# 物质，我们将在[下一部分](https://habr.com/ru/companies/first/articles/850980/)讨论。

文章作者 [@Kleidemos](https://habr.com/users/kleidemos)

---

UFO 飞到这里，给我们博客的读者留下了一个促销代码：
[-15% 订购任何 VDS](https://firstvds.ru/?utm_source=habr&utm_medium=article&utm_campaign=product&utm_content=vds15exeptprogrev)（加热费除外）- HABRFIRSTVDS

标签：[godot](https://habr.com/ru/search/?target_type=posts&order=relevance&q=[godot])，[f#](https://habr.com/ru/search/?target_type=posts&order=relevance&q=[f%23])，[.net](https://habr.com/ru/search/?target_type=posts&order=relevance&q=[.net])，[srtp](https://habr.com/ru/search/?target_type=posts&order=relevance&q=[srtp])

Hub：[公司博客（Блог компании） FirstVDS](https://habr.com/ru/companies/first/articles/)，[.NET](https://habr.com/ru/hubs/net/)，[游戏开发（Разработка игр）](https://habr.com/ru/hubs/gamedev/)，[F#](https://habr.com/ru/hubs/fsharp/)，[Godot](https://habr.com/ru/hubs/godot/)



# 一个 60 岁的囚犯和实验室老鼠。F# 在 Godot 上。第2部分 表达

https://habr.com/ru/companies/first/articles/850980/

**[FirstJohn](https://habr.com/ru/users/FirstJohn/)**
2024年10月16日上午11:20

朴素，16 min，1.8K 阅读

[公司博客（Блог компании） FirstVDS](https://habr.com/ru/companies/first/articles/)，[Программирование*](https://habr.com/ru/hubs/programming/)，[.NET*](https://habr.com/ru/hubs/net/)，[Godot*](https://habr.com/ru/hubs/godot/)，[F#*](https://habr.com/ru/hubs/fsharp/)



![img](https://habrastorage.org/r/w1560/webt/3x/zb/_a/3xzb_akfcdu-zpuwuewdnn-lxju.jpeg)

在上一篇文章中，我谈到了 Godot API 对 F# 的适应。接下来的计划是研究应用程序的整体结构，但我不得不填补公共文本库中的一个严重空白。因此，在这一部分和随后的部分中，我将解释一些奇怪的事情——一个正常的函数是如何通过进化在 Godot 上生成一个运行程序的。

在我看来，大多数 F# 新手在战术和战略层面上都来自不同的宇宙。就像在这里，我们有一个 FP 在本地空间，而在全球突然只有 OOP 拖着。我们可以将两种范式结合起来，这当然是件好事，但在我看来，在FP活动领域的边界上，这道不可逾越的墙并不是那么不可逾越的。它的存在不是由于客观原因，而是由于缺乏经验。

像许多人一样，我从学校的计算机科学课开始学习编程。在那里，我们得到了 `Pascal`，但后来我们几乎擅自爬上了 `Delphi` 和 `C++`。根据我个人的经验，大多数人都是这样的：

- 首先，我们提取函数并提取数组；
- 然后“发明”记录，以避免在多个数组之间同步数据；
- 我们再创造纪录，使纪录更短；
- 继承和重新定义行为，以免与旗帜作对；
- 发明接口以避免继承；
- 学习模式，DI 等。D.其他事项

也就是说，平均发展向量是从过程程序设计到面向对象。在中欧和东欧，从农场的发明（存储单元）到 ОБЧР（`“插入你最喜欢的 OOP 技术”`）。一切都是可以理解和预期的，短期变化是可能的，但新技术迟早需要填补空白。不幸的是，当人们进入 FP 时，他们基本上接受了类固醇（стероидах ）程序编程的过程，而没有转向真正的长寿命应用程序。这就是为什么后者是完全通过 OOP 准备的。

我们不会放弃 OOP，特别是考虑到它在框架层面的普遍性。但我想展示它是如何以及在哪里自然出现在 FP 和 F#，特别是。我将首先分析一个小的抽象函数，将它放大到它在自己的重量下崩溃的状态，用一个新的机制将它分解并重建，然后再进行扩展。这个过程将在几章中重复多次，并且在开始时不会与 Godot 关联（示例除外）。也许我应该把几章放在一个单独的周期里，但我不敢花时间进行如此彻底的重新设计。对于那些已经站稳脚跟，来到这里适应 Godot 的人，我建议他们要么耐心等待，要么向前推两章。而那些刚刚开始他们的旅程的人可能会很高兴，因为在这一章中，正如我的测试人员所说，我坦率地说，我在努力调整你对代码的理解。

## 一切都是表达式

在许多介绍中，F# 提到绝大多数代码都是表达式。但我不知道需要什么样的经验才能立即理解这些单词的含义，因为在大多数情况下，一切都以 C# 中三元算子的类比开始和结束。

这个主题要广泛得多，但我也要从三元表达式（тернарки）开始，因为在“习惯”的形式上，我们确实没有。它可以自己施法，但这没有任何意义，因为通常的 `if` 完全可以完成任务：

```F#
let color =
    if block.Position = cellUnderMouse
    then Colors.OrangeRed
    else Colors.White
```

有趣的是，在 F# 结构中，`if ... then ... else` 是表达式，表达式总是返回某个值。即使它“不返回任何东西”，它也实际上返回了 `Unit` 类型的实例。

`Unit`（或 `unit`）是我自由解释的一种类型，表示可实现的空虚。它由数学和一个漂亮的大型编译器黑客组成，但一般来说，它可以被认为是一个singleton，可以通过 `let instance = ()`。没有数据。它的所有实例/实例都是平等的。因此，从逻辑上讲，如果您知道 `Unit` 在某个地方被放置（或只会被放置），则您事先知道该点上的值：

```F#
// 完全确定的匹配模式，
// 因为 `let () = ()`，
// 编译器不会抱怨它。
let () = GD.print "此函数实际返回 `unit`."
```

因此，在大多数情况下，`unit` 不感兴趣，可以忽略：

```F#
let a =
    () // 忽略。
    () // 忽略。
    () // 忽略。
    GD.print "会有事的 Что-то будет."
    42

// val a: int = 42
```

其他类型的异常：

```F#
let a =
    () // 忽略。
    () // 忽略。
    42 // 它发出警告，因为它很可能是错误的。
    ()

// ... warning FS0020: 此表达式的结果类型为“int”，隐式忽略。使用“ignore”显式拒绝此值，例如“expr |> ignore”，或使用“let”将结果绑定到名称，例如“let result = expr”。
// 
// val a: unit = ()
```

根据同样的逻辑， `if ... then` （没有`else`）。如果 `then` 分支返回 `unit`，编译器可以自己“完成” `else ()` 来对选项进行排序：

```F#
if pointUnderMouse <> under.Point then
    pointUnderMouse <- under.Point
    dirtyAngle <- true
// else ()
```

分支平等是一件很酷的事情，但它与其他几个项目一起，是我们没有早期 `return` 的原因。我们不能写：

```F#
if not ^ canStand obstacles mapSize goal then None
// 我们继续计算其余的情况。
```

我们必须写：

```F#
if not ^ canStand obstacles mapSize goal then None else
// 继续计算。
```

如果在连续计算中出现，则这种 `if` 并不可怕。但更多的时候，他是在比赛，循环等的深度。P.这就是为什么我们必须重新设计整个算法。在这个地方，新手会受到伤害，因为他们没有一套合适的技术（`railway`，`rec` 等）。然而，随着经验的推移，你会发现早期的回归是重要的语义锚，你想在系统的某些节点上收集它们。这并不总是可能的，但只要有可能，每个人都会从中受益。

方法调用是一个表达式，因此它必须返回一些东西，因此 F# 假设 C# 中的所有 `void` 方法都返回 `unit`。我们还需要 `obj.ToString()` 和 `obj.toString` 都是表达式，因此在前一种情况下，括号不被解释为一种特殊的语言结构，而是将 `unit` 实例传递给 `obj.Tostring`。在可能的情况下，转换是反向的，C# 中的 `unit` 将根据上下文解释为 `void` 或缺少参数。由于这种通用性，我们不区分 `Action` 和 `Func`，因为两者都可以返回 `unit`：

```F#
System.Action = System.Func<unit> = unit -> unit
System.Action<int> = System.Func<int, unit> = int -> unit
// 意义上的平等不适用于类型/实例。类型，也就是说，标本，是不同的。
```

确切地说，我们不需要 `Action`，因为它是 `Func` 的子集，可以用普通别名来表示。类似于“异步功能”。`'a Async`（= `Async<'a>`），和 `'a Hopac.Job` 及其许多继承人都不需要“空”版本，就像 `'a Task` 一样，任务需要一个额外的层来表示 `unit Task`。这种通用性进一步扩展到用户代码中，因此当从其他语言迁移某些算法和库时，可能会发现方法和类型的数量有时会减少一半。

`unit` 仍然是一种有形的类型，可以逐件存储，也可以存储在集合中：

```F#
let a = [
    ()
    ()
    ()
    ()
]

// val a: unit list = [(); (); (); ()]
```

当然，这是一个退化的情况，但假设不是列表，而是参与者之间的消息通道。即使您编写了一个简单的触发器，并且不打算传输任何数据，您仍然有一个通用的接口来进行交换。只有一个限制，`unit` 无法从 `obj` 中识别，因为 `box ()` 等于 `null`。这类似于 `Measure` 和 `Option.None`，当您向 ECS 投掷时，请重新检查并仅在具有明确标识的类型中使用值。

## Ignore

表达式有一个新的非 C# 问题。如果调用返回结果并产生侧效应的方法，则很可能只需要侧效应。问题是，你无论如何都必须处理结果。

例如，通过单击鼠标，我们希望删除或添加字段中的障碍块：

```F#
if set.Contains block then
    set.Remove block
else
    set.Add block
```

`set.Remove`，和  `set.Add` 如果操作成功，将返回 `true`。显然，在 `if` 条件下，两个分支都将返回 `true`，因此 `bool` 作为 `unit` 是无用的，只需将其组合在一起即可。任何结果**都可以**插入一个未命名的变量：

```F#
let _ =
    if set.Contains block then
        set.Remove block
    else
        set.Add block
```

这是一种多功能的方法，当您使用不必要的 `IDisposable` 时值得记住：

```F#
use _ = new DisposableSideEffect()
// ...
```

或与建设者（CE）：

```F#
job {
    // ...
    let! _ = // MySystem
        MySubsystem.init // MySubsystem Promise
    // ...
}
```

但在大多数情况下，`let` 是一种过度的措施。在全局 scope 中有一个函数 `ignore: 'a -> unit`。正如标题所示，它只是忽略了它的论点，因此可以将示例重写为：

```F#
if set.Contains block then
    set.Remove block
    |> ignore // : unit
else
    set.Add block
    |> ignore // : unit
```

我们不能高估 `ignore`，并将其视为通用关键字。这只是一个对建设者一无所知的函数，所以它不会将 `'a Job'` 变成 `'unit Job`：

```F#
job {
    // 错误，因为在 builder 中没有特殊的“unit”处理。
    do! client.UploadFile file
        |> ignore

    // 没有错误，也没有上传文件，
	// 因为我们只是扔掉了“FileId Job”，而不是启动它。
    do client.UploadFile file
        |> ignore

    // 它仍然有效，但可能不适合美学原因。
    let! _ = client.UploadFile file

    // ...
}
```

为此目的，核建设者类型通常具有类似性质的功能，例如：

- `Async.Ignore : 'a Async -> unit Async`
- `Job.Ignore : 'a Job -> unit Job`
- `Alt.Ignore : 'a Alt -> unit Alt`
- `Stream.mapIgnore : 'a Stream -> unit Stream`
- 等等。D.其他事项

让你这样写代码：

```F#
job {
    do! client.UploadFile file // : FileId Job
        |> Job.Ignore // : unit Job
}
```

在实践中， `let! _` 和 `do! ... |> Job.Ignore` 没有什么区别。这种一致性必须记住，因为它也意味着两个选项都将等待 `Job` 的完成。`Job.ignore` 没有“Fire and Forget”功能，因为它有不同的类型功能（`queue`、`start` 及其变体）。与 C# 进行类比：

- `let!` 和 `!do` – `await` 的类似物；
- `Job.Ignore` 是一种忽略**工作结果，而不是工作本身**的方法。
- `queue`、`start`、`Job.start` 等D.-忽略工作的方法（“Fire and Forget”）。

C# 中的最后两个点是可选的，但在 F# 中，它们是强制性的，除非选择了第一个点。也就是说，你要么明确地将（`await`-ить） `Job` 整合到建设者中，要么明确地将工作与 `Job.Ignore` 结合起来。此义务来自表达式，因此这些规则适用于所有建设者（`async`、`task` 等）。这是一个非常有用的属性。它花费了几秒钟的工作时间，作为回报，它完全消除了一些常见的错误。

如果 `let!` 负责 `Job` “有结果”，`do!` 对于“没有结果”的 `Job`，我们有两种不同的语法结构，这意味着至少在建设者中，F# 返回到“习惯”的 `void` vs `所有其他二分法`。这很容易在实践中验证，因为 F# 中的建设者可以自己编写（[这里](https://habr.com/ru/articles/804631/)详细介绍了如何编写）。期望每个关键字有一个单独的代码是合乎逻辑的。但是，如果我们考虑到建筑合同，我们不会发现这样的事情。他似乎根本不知道他必须处理 `do!`。这是因为 F# 认为 `do!` 只是 `let! () =` 糖：

```F#
job {
    let! () = someUnitJob
    do! someUnitJob
}
```

我在“寻找扭曲的故事”圈子里走来走去，并不是为了让你注意 `unit` 的概念如何降低基础设施成本。如果没有它，建设者中所需方法的数量将增加 4 倍（输入 x2，输出 x2）。我怀疑这是 C# 放弃建设者转而使用 `async/await` 关键字的原因之一。在 F# 中没有这样的问题，这使得我们更容易过渡到单独的基础设施，并且很少生成共享的基础设施。

![img](https://habrastorage.org/r/w1560/webt/md/zi/vm/mdzivmvrzib9vic_1avjme0apl8.png)

人们试图把这种行为归因于心理学的差异，但我认为这只是物质环境的结果。我们真的可以，所以我们不需要别人，C#-电子设备不能，这就是为什么他们如此依附于微软。总有一天，我们会讨论 `Godot.Callable`，这一刻会再次浮出水面。

值得一提的是，F# 对 `do!` 更宽容并允许它成为 builder 中的最后一句话。`let! () =` - 此特权已被剥夺，并且在此之后至少需要一些活动：

```F#
async {
    let! () = someUnitAsync
    ()
}
```

我没有尝试过，但很可能，如果你有足够的努力，你可以写代码根本没有 `do!`。这是没有意义的，因为 `do!` 简单地说，在这种严格的“线性”定义下，它不可能是一种威胁。如果有人被称为“在小屋里找老鼠”，我会首先关注的是不小心的 `ignore` 扫描功能。

![img](https://habrastorage.org/r/w1560/webt/zk/ka/0b/zkka0bwimvzgfzefzrzbxcix8fk.png)

这东西真的可以把腿射击。

## Scope 和侧效应隔离

F# 中有 `do` 关键字（无感叹号）。巧合的是，它的意思是 `let () =` （没有感叹号）。有些地方需要写 `do`，但在大多数情况下，它被省略，就像显式处理 `()`。这很简单，甚至太多了，所以在一定的情况下，它的存在可能不知道足够长的时间，以至于你不再被认为是新手（有些情况下）。

`do` 的力量在特殊情况下被揭示。首先，它作为编译器的显式类型器工作。这里 `f` 函数 `int -> string`：

```F#
let g f = [
    f 42
    "Item"
]
```

**（后面的懒得翻了，不知道作者写这些和 Godot 有什么关系…… 感觉除了第一篇《C# 下的入侵》那个，其他这个系列文章都不怎么搭边，本身又是俄语，看起来累得慌）**



# 一个 60 岁的囚犯和实验室老鼠。F# 在 Godot 上。第3部分 移植算法

https://habr.com/ru/companies/first/articles/856406/

**[FirstJohn](https://habr.com/ru/users/FirstJohn/)**
2024年11月6日15:15

朴素，18 min，1.1K 阅读

[公司博客（Блог компании） FirstVDS](https://habr.com/ru/companies/first/articles/)，[F#*](https://habr.com/ru/hubs/fsharp/)，[Godot*](https://habr.com/ru/hubs/godot/)，[.NET*](https://habr.com/ru/hubs/net/)，[函数程序设计（Функциональное программирование）*](https://habr.com/ru/hubs/funcprog/)



![img](https://habrastorage.org/r/w1560/webt/zl/jv/wg/zljvwgl8u84rushycfwi7nh0xfi.jpeg)

在[上一章](https://habr.com/ru/companies/first/articles/850980/)中，我们研究了表达式的性质及其对函数结构的影响。从某种意义上说，这是一种自下而上的函数观。现在我们需要从算法的角度从上往下看。我们感兴趣的是算法如何存在于函数中，它们位于何处，以及如何转换周围空间。这是一个广泛的主题，但它围绕着过程的生命周期和数据。

## 先天性和后天性

在基于 XAML 的平台世界中，有一个 `Freezable` 对象的概念。每个人都有一个不可逆的 `Freeze` 方法，在调用该方法后，任何更改尝试都会导致异常。如果对象积极参与某个进程，则可以阻止它进入冻结（`CanFreeze = false`）。无法解冻对象，但可以通过 `Clone` 复制对象，并将副本变异为新的雕带。我们总共得到三个状态和四个过渡。我们只能间接控制两个过渡，直接控制两个过渡，但其中一个过渡导致一个新的物体。

![img](https://habrastorage.org/r/w1560/webt/kz/9p/fw/kz9pfwfkxfkgtoiogmnlse3ilhw.png)

**（后面的懒得翻了，不知道作者写这些和 Godot 有什么关系…… 感觉除了第一篇《C# 下的入侵》那个，其他这个系列文章都不怎么搭边，本身又是俄语，看起来累得慌）**



# ----------[Dragoș-Andrei Ilieș 博客]----------



# 用 F# 开发游戏有可能吗？

Dragos 探索了用 F# 开发游戏的可能性

https://www.compositional-it.com/news-blog/game-development-with-f-is-it-possible/

By [Dragoș-Andrei Ilieș](https://www.compositional-it.com/news-blog/author/dragos/)

Posted: December 2, 2022

Categorised: [Technology](https://www.compositional-it.com/news-blog/category/technology/)

- [.net](https://www.compositional-it.com/news-blog/tag/net/)
- [fsharp](https://www.compositional-it.com/news-blog/tag/fsharp/)
- [gamedevelopment](https://www.compositional-it.com/news-blog/tag/gamedevelopment/)



## 介绍

我喜欢 F# 和函数式编程（FP），但我也喜欢电子游戏和制作它们，这当然引出了一个自然的问题：F# 可以用于游戏开发吗？

作为一个涉足游戏开发、目前正在深入研究函数式编程的人，我觉得这有点矛盾。游戏开发遵循面向对象范式（OOP），每个致力于游戏开发的主要框架或引擎都使用 OOP 语言（例如 Unity、Godot 和虚幻引擎），在开发游戏时，我们考虑的是玩家、敌人和世界，这些都是具有属性和动作的东西，因此感觉更适合 OOP。FP 往往更抽象，如果我们发现自己需要处理数据或抓取网站数据，OOP 就不再有意义了，FP 似乎更适合。

然而，探索解决方案可能是值得的，这些解决方案使我们能够利用在功能范式中工作的好处，同时以富有成效和易于理解的方式开发游戏。F# 是一种 .NET 生态系统里的语言，允许更容易的与 .NET 中其他语言之间的通信，类似于编程语言 C#。这种语言可以在许多流行的游戏引擎中找到，比如 Unity 和 Godot，因为这两种语言在同一个生态系统中，所以有工具/方法允许使用 F#。我们将探索这些，并试图弄清楚用 F# 开发游戏是否可行。

## 游戏引擎

### Unity

Unity 使用脚本（C# 文件）为其对象创建行为。Unity 中像玩家角色一样的对象将附加一个脚本，允许它移动、射击和碰撞，或执行 C# 脚本编程的任何其他行为。

在互联网上可以找到几种方法。杰克逊·邓斯坦（Jackson Dunstan）在他的[博客](https://www.jacksondunstan.com/articles/5058)中提供了一种利用 `FSharp.Core.dll` 的方法。最近，频道 7sharp9 开始了一个 [youtube 系列](https://www.youtube.com/watch?v=sK6BUkQE5U4)，他们在 Unity 中添加了 F#。他们的方法依赖于将 F# 编译成程序集，Unity 加载程序集，这往往比加载 C# 脚本慢。

另一个有趣的发现是[四名高年级本科生](https://www.reddit.com/r/fsharp/comments/gma9sk/using_f_with_unity_part_6_conclusions_and_farewell/)为他们的高年级项目开发 Unity 和 F# 游戏的过程。虽然有点老，但它提供了一个试图构建完整游戏的团队的概述。他们得出结论，在与 Unity 合作一年后，他们不建议使用 F#，而且这充其量是一种新奇的东西。Unity 设计的命令式和可变状态导致从头开始重新创建了许多 Unity 行为，他们从 Unity 中使用的东西被证明是不值得的。

### Godot

Godot 的行为与 Unity 类似，它还使用脚本添加了行为，并且可以理解 C#。我们使用 F# 编写行为的方式是使用 C# 作为管道，用 F# 编写的所有内容都可以在 C# 文件中使用，而 C# 文件将由 Godot 使用。[Lars Kokemohr 的博客](http://www.lkokemohr.de/fsharp_godot.html)中可以找到这种实现的一个例子。在 [HAMY LABS 的](https://www.youtube.com/watch?v=LwTdcNYGXM0)频道上可以找到类似方法的视频解释，其中包含更多解释。

就我个人而言，我希望这些指南能显示更多的示例，所以我尝试了一下。这就是我们如何编写 Godot 用于每秒（或60帧）运行的“Process”函数，以及一些额外的代码，使对象根据按键移动：

```F#
	override this._Process(delta : float32) =
        let mutable (speed : float32) = 3.0f
        if Input.IsKeyPressed (int KeyList.W)
        then this.Position <- this.Position + new Vector2(0.0f, (- speed))
        if Input.IsKeyPressed (int KeyList.S)
        then this.Position <- this.Position + new Vector2(0.0f, speed)
        if Input.IsKeyPressed (int KeyList.A)
        then this.Position <- this.Position + new Vector2((- speed), 0.0f)
        if Input.IsKeyPressed (int KeyList.D)
        then this.Position <- this.Position + new Vector2(speed, 0.0f)
        ()
```

编写 F# 代码以与 Godot 配合使用似乎与 Unity 中发生的问题类似。我们仍然使用可变状态和继承，它们来自 F# 的纯度。然而，使用 Godot，我发现应用我的 F# 代码要容易得多，尽管这似乎仍然有点费力，我们创建的每个 F# 文件都必须有一个等效的 C# 文件。

## 结论

我们查看了游戏行业中一些流行的游戏引擎。结论是什么？用 F# 开发游戏是一个艰难的过程，流行的游戏引擎不可靠。我希望你喜欢这篇关于 F# 游戏开发的简短概述🙂



# ----------[Lars Kokemohr 博客]----------



# 在 Godot 3 中使用 F#

https://www.lkokemohr.de/fsharp_godot.html



![img](https://www.lkokemohr.de/img/articles/fsharp_godot_header.png)

**Godot 3 增加了对 C# 的支持。本文解释了如何利用它，以便您可以用 F# 编写游戏代码。**

以下文章解释了工作流。如果要查看结果，请查看[项目文件](https://www.lkokemohr.de/Godot/fsharp_godot.zip)。

注意：目前，本文仅涉及使用 Windows 和 Visual Studio 的工作流。我打算稍后用相应的 linux 工作流更新它。

## 为什么是 F#？

C# 最大的优点是它允许你混合搭配许多不同的风格。但这是有代价的，因为根据我的经验，一个非常有经验和洞察力的程序员需要了解不同的编程范式，坚持你选择的范式，并根据你编写的代码类型选择正确的范式。因此，C# 和 F# 之间最大的区别可能是 F# 是一种更加固执己见的语言。在 C# 中你不能做但在 F# 中你能做的事情真的并不多，但软件开发不是关于你能做什么或不能做什么，而是关于你最终会做什么。通过使遵守既定规则比违反规则更容易，F# 创造了所谓的“成功坑”（更多细节请参阅 [Mark Seemann的演讲](https://www.youtube.com/watch?v=US8QG9I1XW0)）。

## 步骤1：使用 C# 脚本创建 Godot 项目

首先在 Godot 3 中创建一个新项目（使用支持 Mono 的版本）。完成此操作后，创建一个名为 Player.tscn 的新场景，并**添加一个称为 Player.cs 的 C# 脚本**。

## 步骤2：将 F# 项目添加到解决方案中

1. 添加 C# 脚本不仅会创建一个 .cs 文件，还会生成一个解决方案文件。导航到项目文件夹并使用 Visual Studio **打开解决方案文件**。
2. 接下来，向解决方案中**添加一个新的 F# 项目**。从“Visual F#”部分选择“库”作为模板。
3. 编辑新项目的属性，并**将目标框架设置**为“.NET framework 4.5”。
4. 现在右键单击引用并**添加“GodotSharp”和“System.Runtime”**。您必须浏览它们 - GodotSharp.dll 可以在项目的“.mono/passemblys”子文件夹中找到。你计算机上可能会有不止一个 System.Runtime.dll。无论你选择哪一个，Visual Studio 只会使用你提供的路径作为提示，并自行定位正确的文件。
5. 最后，**将对新 F# 项目的引用添加到** Godot 生成的 **C# 项目**中。

## 步骤3：用 F# 编写一个类，并在 C# 中引用它

在 Godot 中使用 F# 的关键技巧非常简单：只需在 F# 中编写一个类，并在 C# 类中**继承它**。

因此，转到 Godot 生成的 C# 类。从文件中删除所有内容，并替换为

```c#
using GodotDemoFs;
 
public class Player : PlayerFs
{}
```

![img](https://www.lkokemohr.de/img/articles/fsharp_godot_2.png)

现在转到 F# 项目中的 .fs 文件并添加以下代码：

```F#
namespace GodotDemoFs
 
open Godot
 
type PlayerFs() =
    inherit Node()
 
    override this._Ready() = 
        GD.Print("Hello from F#!")
```

![img](https://www.lkokemohr.de/img/articles/fsharp_godot_3.png)

## 步骤4：保存和编译

最后但同样重要的是，保存所有更改，返回 Godot 编辑器，打开屏幕底部的“Mono”面板，然后单击“构建项目”。

![img](https://www.lkokemohr.de/img/articles/fsharp_godot_4.png)



# 在 Godot 3 中使用 F# - 第 2 部分

https://www.lkokemohr.de/fsharp_godot_part2.html



![img](https://www.lkokemohr.de/img/articles/fsharp_godot_part2_header.png)

**本文展示了如何在 F# 中实现 Godot 脚本的常见元素。**

正如我在[第一部分](https://www.lkokemohr.de/fsharp_godot.html)中提到的，F# 是一种相当固执己见的语言。在其他语言中很容易做到的几件事是故意尴尬的，甚至是根本不允许的。

这意味着，如果你打算在 Godot 中编写 F# 代码，你应该知道一些注意事项。

本文只介绍了对 C# 代码的面向对象结构的轻微更改。本系列的下一部分将介绍如何编写更紧密地遵循函数式范式的游戏代码。

如果你想测试代码，你可以下载[项目文件](https://www.lkokemohr.de/Godot/fsharp_godot_part2.zip)。

## 一个典型的 C# 脚本

让我们来看看 C# 中一个非常常见的脚本，然后将其翻译成 F#。

这个脚本应该做两件事：移动玩家，并在玩家移动时保持动画运行。

> 免责声明：这个脚本是围绕我想展示的东西构建的，这不是编写玩家脚本的最佳实践。

```c#
using Godot;

public class Player : Sprite
{
    [Export]
    float speed = 200.0f;
    
    AnimationPlayer anim;
    
    public override void _Ready()
    {
        anim = GetNode(new NodePath("AnimationPlayer"))
            as AnimationPlayer;
    }
    
    public override void _Process(float delta)
    {
        var movement = new Vector2();

        if (Input.IsActionPressed("ui_right"))
        {
            movement.x += 1.0f;
        }

        if (Input.IsActionPressed("ui_left"))
        {
            movement.x -= 1.0f;
        }

        if (Input.IsActionPressed("ui_down"))
        {
            movement.y += 1.0f;
        }

        if (Input.IsActionPressed("ui_up"))
        {
            movement.y -= 1.0f;
        }
        
        Translate(movement.Normalized() * speed * delta);
        
        if (movement != new Vector2())
        {
            if (!anim.IsPlaying())
            {
                anim.Play("walk");
            }
        }
        else
        {
            anim.Stop();
        }
    }
}
```

![img](https://www.lkokemohr.de/img/articles/fsharp_godot_part2_1.png)

### export

该脚本包含一个名为 Player 的类。这个类有**一个名为“speed”的成员**，初始值为 200，可以从检查器中编辑。

![img](https://www.lkokemohr.de/img/articles/fsharp_godot_part2_1_1.png)

### onready

它还有**一个名为“anim”的成员**，用于存储对场景中动画播放器的引用。由于 C# 中没有“onready”关键字，因此**在 _Ready() 方法中填充了引用**。

![img](https://www.lkokemohr.de/img/articles/fsharp_godot_part2_1_2.png)

### 运动

在 _Process() 方法中，会**创建一个新的运动向量**，并根据按下的按键**进行调整**。ui_right 和 ui_left 分别与 x 坐标相加、相减，从 y 坐标加减 ui_up 和 ui_down。然后，通过归一化 movement 乘以 speed 乘以上一帧中经过的时间来**平移**角色。

![img](https://www.lkokemohr.de/img/articles/fsharp_godot_part2_1_3.png)

### 动画

第 39-49 行检查移动向量是否不为零（即玩家正在移动）。如果玩家正在移动，**动画将保持运行**。如果不是，动画将停止。

![img](https://www.lkokemohr.de/img/articles/fsharp_godot_part2_1_4.png)

## F# 中的相同脚本

现在让我们来看看脚本的 F# 实现。如果你正在使用项目文件，你可以在子文件夹 GodotFsBasicFs 的 Library1.fs 中找到代码。（我喜欢用项目名称+Fs来命名我的F#项目，这就是为什么在GodotFsBasic项目中，我最终得到了一个包含两次“Fs”的文件夹名称。）

```F#
namespace GodotFsBasicFs

open Godot

type PlayerFs() as this =
	inherit Sprite()
	
	[<Export>]
	let speed = 200.0f
	
	let anim =
		lazy(
			this.GetNode(new NodePath("AnimationPlayer"))
			:?> AnimationPlayer
			)

	override this._Process(delta) =
		let check key =
			if Input.IsActionPressed(key) then 1.0f else 0.0f

		let movement =
			new Vector2(check "ui_right" - check "ui_left",
						check "ui_down" - check "ui_up")
		
		this.Translate(movement.Normalized() * speed * delta)
		
		let isMoving = movement <> new Vector2()
		let enableAnim name =
			if not (anim.Value.IsPlaying())
			then anim.Value.Play(name)
		if isMoving then enableAnim "walk" else anim.Value.Stop()
```

![img](https://www.lkokemohr.de/img/articles/fsharp_godot_part2_2.png)

### 命名空间

第一个区别是第 1 行：每个类都必须是 F# 中命名空间或模块的一部分，因此此文件中的**命名空间**是必需的。

```F#
namespace GodotFsBasicFs
```

![img](https://www.lkokemohr.de/img/articles/fsharp_godot_part2_2_1.png)

### 构造器

下一个区别是在第 5 行：Player 类的类型声明包含单词**“as this”**。

在 F# 中，类是通过编写构造函数来声明的。第 9-15 行不仅仅是成员声明，它们是**类构造函数**中初始化成员的命令行。因此，首先将速度设置为 200，然后填充动画。这与 C# 等语言中的声明性代码不同，在 C# 中，声明之间没有明确的顺序。

![img](https://www.lkokemohr.de/img/articles/fsharp_godot_part2_2_2.png)

那么，这与“as this”这个词有什么关系呢？

由于第 9-15 行属于构造函数，因此**不允许访问“this”**，因为对象尚未完全初始化。因此，如果我们需要访问“this”，我们需要通过添加公共成员和方法来结束构造函数，或者我们可以通过在类声明的第一行添加单词“as this”来允许从构造函数访问“this”。

（顺便说一句，这也允许通过使用“as self”或“as currentPlayer”之类的东西来重命名“this”。）

![img](https://www.lkokemohr.de/img/articles/fsharp_godot_part2_2_3.png)

### 类型推断

由于 F# 是一种**类型推断**语言，我们不需要指定“速度”是一个浮点数，它是从初始值推断出来的。但与 GDscript 不同的是，**F# 是静态类型的**，这意味着如果我们试图将速度用作浮点数以外的东西，编译器会抱怨。

![img](https://www.lkokemohr.de/img/articles/fsharp_godot_part2_2_4.png)

### 惰性

第 11-15 行初始化 anim 成员。同样，没有 onready 关键字，但 F# 有 **lazy 关键字**，在这种情况下，它的工作原理非常相似。通过将某些内容包装在“lazy（…）”中，我们可以定义这个值在第一次需要之前不应该被确定。一旦计算完毕，它就会被存储，不再计算。缺点是我们需要使用“anim.value”来访问值，但有两个好处：

1. 我们不需要编写 _Ready 方法来填充单个成员变量。
2. 从创建玩家到填充动漫成员之间没有无效的阶段。在 C# 中，你可能会遇到 bug，因为一段代码在填充之前就访问了anim。这就是为什么 **F# 不允许我们声明未初始化的成员**。通过使成员的内容变为惰性，我们可以解决需要立即填充的问题，即使稍后才能找到 AnimationPlayer。

![img](https://www.lkokemohr.de/img/articles/fsharp_godot_part2_2_5.png)

### 向下转型

如果你是 F# 的新手，你可能会想知道第 14 行中的字符是什么意思——“**`:?>`**”只是 C# “as” 关键字的 F# 符号。

如果我们删除第 14 行，我们将得到一个节点，第 14 行将该节点向下转型到 AnimationPlayer。

![img](https://www.lkokemohr.de/img/articles/fsharp_godot_part2_2_6.png)

## F# 中的 _Process 方法

让我们来看看 _Process 方法。以“member this”或“override this”开头的第一行标志着构造函数的结束。任何 let（或 do）绑定都必须出现在第一个成员之前。

### 检查

首先，您会注意到 C# 脚本中的 **if-else 模式在 F# 版本中只出现一次**。第 18 行定义了一个函数，根据当前是否按下操作，将输入操作的名称转换为 1 或 0。

正如我在第一篇文章的前言中所说：我们本可以在 C# 中做同样的事情，只是创建一个新函数需要太多的开销，这可能会让人感到困惑而不是有帮助。在 F# 中，新函数添加的开销很小，并且它是 _Process 方法的本地函数，因此阅读代码的人会立即知道它与类的其他部分无关。

![img](https://www.lkokemohr.de/img/articles/fsharp_godot_part2_3_1.png)

### 运动

21 行计算运动矢量。得益于检查功能，这段代码最终变得更加简洁和抗 bug（没有遇到偶尔出现的 x 和 y 或 + 和 - 混淆的复制粘贴错误）。

![img](https://www.lkokemohr.de/img/articles/fsharp_godot_part2_3_2.png)

### 翻译

关于第 25 行没什么好说的，除了 F# 要求我们**在“this”上显式调用 Transform**，有些人会用可选的“do”-关键字（如“do this.Translate（…）”）开始这一行，以表明这一行是由于其副作用而执行的，并且没有返回值。

![img](https://www.lkokemohr.de/img/articles/fsharp_godot_part2_3_3.png)

### 更新动画

同样，第 27-31 行基本上与 C# 脚本中的第 39-49 行做了相同的事情，但 F# 的轻量级本地函数会拆分功能并编写更多**自文档的代码**。

![img](https://www.lkokemohr.de/img/articles/fsharp_godot_part2_3_4.png)



# 在 Godot 3 中使用 F# - 第 3 部分

https://www.lkokemohr.de/fsharp_godot_part3.html



![img](https://www.lkokemohr.de/img/articles/fsharp_godot_part3_header.png)

**本文展示了如何使用 F# 的一些语言特性来清理第 2 部分的代码。**

在本系列的这一部分中，我们将查看[上一部分](https://www.lkokemohr.de/fsharp_godot_part2.html)的 F# 脚本，并了解如何使用 F# 的一些语言特性来提高可读性。

如果你想测试代码，你可以下载[项目文件](https://www.lkokemohr.de/Godot/fsharp_godot_part3.zip)。

## 让我们看看……

让我们看看当前状态下的代码。

```F#
namespace GodotFsBasicFs

open Godot

type PlayerFs() as this =
	inherit Sprite()
	
	[<Export>]
	let speed = 200.0f
	
	let anim =
		lazy(
			this.GetNode(new NodePath("AnimationPlayer"))
			:?> AnimationPlayer
			)

	override this._Process(delta) =
		let check key =
			if Input.IsActionPressed(key) then 1.0f else 0.0f

		let movement =
			new Vector2(check "ui_right" - check "ui_left",
						check "ui_down" - check "ui_up")
		
		this.Translate(movement.Normalized() * speed * delta)
		
		let isMoving = movement <> new Vector2()
		let enableAnim name =
			if not (anim.Value.IsPlaying())
			then anim.Value.Play(name)
		if isMoving then enableAnim "walk" else anim.Value.Stop()
```

![img](https://www.lkokemohr.de/img/articles/fsharp_godot_part3_1.png)

### 输入和移动

我在 C# 和 F# 版本之间所做的更改缩短了脚本，并删除了第 18 至 23 行中的一些代码重复。

我本可以在 C# 中做类似的事情，但因为这意味着要在类中添加一个新方法，所以似乎不值得这样做。毕竟，这意味着阅读代码的人必须在 Player 类的代码中**来回跳转**。

另一方面，F# 允许我们在**函数中声明函数**。这样，检查函数的声明就在使用它的行旁边，这**提高了可读性**。在 `_Process` 方法中编写函数的另一个效果是，现在非常清楚的是，**除了** `_Process` 方法**之外，永远不会使用** check 函数，这有助于进一步清理混乱。

但现在我们已经将代码分为检查函数和计算运动向量的代码，虽然计算运动向量在某种程度上是**特定于项目的**（因为它包括要检查的动作的名称），但检查函数实际上不是。它只是将动作转换为一个非常有用的因子，但开发人员绝不需要阅读它来理解 Player 类在做什么。

此外，它**不访问 Player 类的任何成员**，因此将代码放在这里而不是其他地方既没有帮助也没有必要。

![img](https://www.lkokemohr.de/img/articles/fsharp_godot_part3_1_1.png)

### 模块

所以，让我们移动它。如果你来自面向对象语言，你的第一个想法可能是将其转换为**静态类函数**（就像IsActionPressed() 函数是 Input 类中的静态函数一样）。F# 有一个非常简洁的语法来做到这一点。

现在，让我们在 PlayerFs 类的正上方添加一个新模块。

模块允许我们在类之外声明函数。好吧，从技术上讲，模块被编译成具有静态函数的类，因此我们实际上是在做与 C# 中相同的事情，但简洁的语法大大**降低了门槛**。

![img](https://www.lkokemohr.de/img/articles/fsharp_godot_part3_1_2.png)

## 游戏代码到库代码

让我们进一步遵循这一思路——创建运动矢量的方式并不特别。其他游戏肯定会使用不同的动作标识符，但使用两个用于左右，两个用于上下并不是什么突破性的新闻。

```F#
namespace GodotFsBasicFs

module GdInput =
	open Godot
	
	/// <summary>
	/// Returns 1.0f if given action is currently pressed,
	/// otherwise returns 0.0f
	/// </summary>
	let actionToFactor (key : string) =
		if Input.IsActionPressed(key) then 1.0f else 0.0f

	let actionsToFactor pos neg =
		actionToFactor pos - actionToFactor neg

	let actionsToDirection posX negX posY negY =
		new Vector2(actionsToFactor posX negX,
					actionsToFactor posY negY)

open Godot
open GdInput

type PlayerFs as this =
	inherit Sprite()

	[<Export>]
	let speed = 200.0f
```

![img](https://www.lkokemohr.de/img/articles/fsharp_godot_part3_2.png)

### 命名

既然我们已经有一个函数可以将一个动作转换为 1 或 0，那么让我们编写另一个函数，将两个动作转换成 -1、0 或 1。

使用这个新函数，我们可以声明一个将四个动作转换为 Vector2 的函数。只需编写“module GdInput =”并移动几行，我们就将非常特定于 Player 类的代码转换为可以**在项目中的任何地方重用**的代码。

这也会影响函数的编写方式。“check”这个名字**对于本地函数来说很好**——它很短，作为助记符就足够了，但它真的**不是不言自明的**。因此，让我们将“check”重命名为“actionToFactor”，将“checkTwo”重命名为”actionsToFactor”，并将“checkFour”重命名为：“actionsToDirection”。

![img](https://www.lkokemohr.de/img/articles/fsharp_godot_part3_2_1.png)

### 类型

游戏代码和库代码之间应该改变的另一件事是我们如何处理类型。**Player 类中的游戏代码应尽可能简洁**，以便您一眼就能理解代码应该做什么。此时类型信息并不重要，因为我们并不真正关心“actionsToDirection”返回什么样的向量，然后传递给“this.Translate”。如果返回的类型与预期的类型不匹配，编译器将发出投诉，由于两行直接相邻，因此投诉的内容将很明显。

转移到 GdInput 模块的功能不再需要那么简洁，因为没有人真正需要阅读整个 GdInput 模块并立即理解它。一旦从 GdInput 模块的某个函数中得到的结果与预期不符，人们就会查看 GdInput 模块，因此**模块中的类型信息应该是明确的**。

与大多数具有类型推理的语言一样，F# 允许我们通过在变量或参数后面加上“: type”来指定其数据类型。

![img](https://www.lkokemohr.de/img/articles/fsharp_godot_part3_2_2.png)

## 完成模块

```F#
module GdInput

open Godot

/// <summary>
/// Returns 1.0f if given action is currently pressed,
/// otherwise returns 0.0f
/// </summary>
let actionToFactor (key : string) =
	if Input.IsActionPressed(key) then 1.0f else 0.0f

/// <summary>
/// Returns 1.0f if the pos-action is pressed,
/// -1.0f if the neg-action is pressed
/// and 0.0f if both are or none is pressed
/// </summary>
let actionsToFactor pos neg =
	actionToFactor pos - actionToFactor neg

/// <summary>
/// Treats the four given actions as a virtual joystick
/// If only posX is pressed (1,0) is returned
/// If only negY is pressed (0,-1) etc.
/// </summary>
let actionsToDirection posX negX posY negY =
	new Vector2(actionsToFactor posX negX,
				actionsToFactor posY negY)
```

![img](https://www.lkokemohr.de/img/articles/fsharp_godot_part3_3.png)

最后但同样重要的是，让我们为函数**添加一些文档**，这样当我们在其他地方使用函数时，我们就会得到一个有用的工具提示。

**请注意编码风格的变化如何影响我们将来处理代码的方式**：对更显式的命名、显式类型和注释的需求为每次代码更改增加了大量工作，这就是为什么我们没有将这种风格应用于 Player 类中的代码，因为这样的游戏特定代码应该很容易修改，这样你就可以快速原型化新想法，而不会冒着过时注释的风险，也不会因为忘记调整数据类型而不断遇到编译器错误。

库代码（即旨在重用的代码）不需要那么容易修改，事实上更改它是有风险的，因为更改它很有可能会破坏使用它的许多地方之一。

这也是为什么**注释对于这类代码如此重要**的原因，因为使用库函数的程序员应该依赖于预期的功能，而不是实际的功能。如果我们的函数没有做它应该做的事情，并且有人故意使用了这种破坏行为，那么修复我们的函数会破坏代码，而不是修复它。

因此，作为最后一步，让我们**将模块代码移动到它自己的文件中**。小心：创建新的 .fs 文件时，请确保它出现在 Library1.fs 文件上方，因为 F# 文件是按照它们在项目文件列表中的显示顺序进行计算的。

## 玩家代码到游戏代码

现在，生成输入方向的函数已移出 Player 类，但 actionsToDirection 的调用仍然存在，它列出了用于控制播放器的所有操作。

```F#
namespace GodotFsBasicFs

open Godot
open GdInput

module GameInput =
	let getMovement () =
		actionsToDirection "ui_right" "ui_left" "ui_down" "ui_up"

open GameInput

type PlayerFs() as this =
	inherit Sprite()
	
	[<Export>]
	let speed = 200.0f
	
	let anim =
		lazy(
			this.GetNode(new NodePath("AnimationPlayer"))
			:?> AnimationPlayer
			)

	override this._Process(delta) =
		let movement = getMovement ()
		this.Translate(movement.Normalized() * speed * delta)
		
		let isMoving = movement <> new Vector2()
		let enableAnim name = if not (anim.Value.IsPlaying()) then anim.Value.Play(name)
		if isMoving then enableAnim "walk" else anim.Value.Stop()
```

![img](https://www.lkokemohr.de/img/articles/fsharp_godot_part3_4.png)

### 游戏输入

同样，这并不是 Player 类**所独有的**。如果我们想让菜单对输入做出反应，我们就必须再次重复同一行，并得到**代码重复**的常见风险和缺点。因此，让我们也将该代码移出 Player 类，但由于该代码是特定于游戏的，所以让我们将其保留在文件中。

![img](https://www.lkokemohr.de/img/articles/fsharp_godot_part3_4_1.png)


现在我们可以在游戏中的任何地方使用 GameInput.getMovement()，而不必再次指定操作。

## 动画模块

现在，让我们来处理与动画相关的代码。这里的代码应该是不言自明的。

```F#
module Animations

open Godot

/// <summary>
/// Starts an animation unless it's already playing.
/// <summary>
let enableAnim (anim : Lazy<AnimationPlayer>) name =
	if
		not (anim.Value.IsPlaying())
		|| not (anim.Value.CurrentAnimation = name)
	then
		anim.Value.Play(name)

/// <summary>
/// Makes sure that the animation plays if the condition is true
/// and stops if it isn't.
/// <summary>
/// <param name="anim">lazy reference to Godot AnimationPlayer node</param>
/// <param name="condition">condition that decides whether
/// the animation should play</param>
/// <param name="name">name of the animation to play</param>
let visualize (anim : Lazy<AnimationPlayer>) condition name =
	if condition then (enableAnim anim name) else anim.Value.Stop()
```

![img](https://www.lkokemohr.de/img/articles/fsharp_godot_part3_5.png)

## 节点扩展方法

最后，让我们移动填充 anim 节点的代码。由于该代码需要引用“this”，因此我们不能将其移动到模块中，除非我们想更改签名，以便将其作为参数传递。

幸运的是，F# 确实有一个特性，允许我们向现有类添加方法：**扩展方法**（如同从 C# 所知的）。
为了向现有类添加方法，我们以这样开始

`type ABC with`

然后像在类声明中一样列出公共成员。

```F#
module GodotUtils

open Godot

type Node with
	/// <summary>
	/// Gets a lazy reference to a node
	/// </summary>
	/// <param name="path">Path to the node to be retrieved</param>
	member this.getNode<'a when 'a :> Node> (path:string) =
		lazy((this.GetNode(new NodePath(path))) :?> 'a)
```

![img](https://www.lkokemohr.de/img/articles/fsharp_godot_part3_6.png)

### 具有约束的泛型

由于我们希望该方法返回一个类型化节点（在这种情况下是AnimationPlayer），我们需要使用泛型，但由于GetNode的结果永远不能是任何不从node继承的东西，我们需要向类型参数添加一个约束。
这就是为什么getNode有一个类型参数“'a when'a:>Node”，其内容为“某个名为'a的类型，它满足了'a必须从Node继承的约束”。

![img](https://www.lkokemohr.de/img/articles/fsharp_godot_part3_6_1.png)

### 库代码 vs. 游戏代码

请再次注意，扩展方法中的库代码要冗长得多，因为该代码提供了有关所涉及类型的大量信息，而调用新 getNode 方法的 Player 类中的游戏代码现在可以完全依赖类型推理，从而生成简洁的代码。

![img](https://www.lkokemohr.de/img/articles/fsharp_godot_part3_6_2.png)

## 后记

通过将游戏代码与库代码分离，我们能够将 Player 类从 25 行缩短到 12 行。更重要的是，现在 Player 类几乎没有不包含**特定于该 Player 类的信息**的行，这意味着如果你想了解该类的功能，或者想找到需要编辑的地方来测试不同的机制或新的平衡公式，你就不必搜索通用代码。

```F#
namespace GodotFsBasicFs

open Godot
open GodotUtils
open Animations

module MyInput =
	let getInputDir () =
		Input.actionsToDir "ui_right" "ui_left" "ui_down" "ui_up"

type PlayerFs() as this =
	inherit Sprite()
	
	[<Export>]
	let speed = 200.0f
	
	let anim = this.getNode "AnimationPlayer"

	override this._Process(delta) =
		let dir = MyInput.getInputDir ()
		this.Translate(dir * speed * delta)
		
		visualize anim (dir <> new Vector2()) "walk"
```

![img](https://www.lkokemohr.de/img/articles/fsharp_godot_part3_7.png)



# ----------[Lenscas 博客]----------



# F# 和 Godot 第 1 部分

https://lenscas.github.io/posts/fsharp_godot_part1/



在这一部分中，我将谈谈为什么我选择 F# 而不是我所知道的许多其他语言，这些语言本可以与 godot 一起使用。此外，它还介绍了如何设置它。

第 2 部分将介绍如何在 Godot 中使用 F# 做一些基本的事情，比如如何发出信号。

在第 3 部分中，我们将更深入地了解 F#，并使用它的一些功能来扩展 Godot 的一个库，使其更易于使用。这将涉及本部分不涉及的功能和语法。

## 为什么是 F#？

简单地说，在现有的选项中，这是我最感兴趣的，也是最有可能让我愉快写作的。

## 为什么不是 GDScript 或 Rust。

我已经知道 GDScript 和我相处不好，只是因为我不喜欢动态类型，我真的很喜欢用 lambdas。

Rust 不是一个选择，因为你不能用 GDNative 编译成 wasm。即使可以，至少看起来与 godot 一起工作的 crates（rust 用于库的名称）仍然很年轻，不断变化，可能需要“未保存”的代码才能工作，或者依赖于太多的“未保存的”代码，如果保存抽象还不完善，我也不会感到惊讶。

## C# vs F#

让我们从为什么我不是 C# 的狂热粉丝开始，然后让我们深入了解我喜欢它的一些方面，最后谈谈 F# 是如何做得更好的，或者至少对我来说更好。

首先，C# 具有隐式 null。这意味着我无法从类型签名中看到一个方法是否可能不返回正确的值。这听起来可能没什么大不了的，但如果你曾经使用过一种具有良好 `Option<T>` 类型的语言，你很快就会在不提供它的语言中错过它。还有一个原因是“null”通常被称为数十亿美元的错误。

C# 也不擅长推断你的类型。这是有原因的，但在花了大量时间学习一门真正擅长它的语言之后，每当我需要告诉 C# 我制作并直接分配给 `List<int>` 类型的属性的列表确实是一个 int 列表时，它就变成了一件苦差事，值得叹气。来吧！

它还严重依赖异常，这在函数的类型签名中是不可见的，因此我并不喜欢。

我喜欢 C# 的哪些方面？好吧，Linq 和扩展方法很好。有了 Linq，使用任何类型的集合都变得超级容易，而不是陷入丑陋的 foreach 循环。扩展方法很好地帮助了链式方法，尽管 Rust 在 trait 系统方面做得更好。

C# 也有很好的 lambda 支持，async/await 比 F# 稍微简单一点，尤其是当 godot 参与其中时。

最后，让我们来看看 F#。首先，它使用 `Option<T>` 而不是隐式空值，并且有许多函数可以让使用选项变得很好 https://fsharp.github.io/fsharp-core-docs/reference/fsharp-core-optionmodule.html. F# 它也更善于推断你的类型（有时可以说它太好了……），而且它对异常的依赖要小得多，尽管我知道你并不总是能逃脱它们。

至于 Linq，F# 既可以访问 Linq，也有自己版本的 Linq 提供的方法，这些方法实际上具有正确/标准的名称，而不是 Linq 的“sql 启发”命名方案。但好处并不止于此，因为它有一种神奇的方法来链接函数（`|>`）、`Lazy<T>`、模式匹配、尾部调用优化和 if 是表达式而不是语句，从而消除了对 C# `C ? A:B` 语法的需求。

当然，它并不完美，尤其是与 Godot 结合使用时。这里有一些你会遇到的痛点

首先，语法相当不同。有多不同？下面是一个简单的 add 函数以及如何调用它

```F#
let add a b =
    a + b
    
add 10 20
```

这也意味着方法更难链接，因为你不能简单地执行 `a.b(param).c()`，而是将其写成 `(a.b param).c()`。幸运的是，F# 有另一种方法来链接函数，但它要求你停止编写方法，而是使用将其作为参数的函数。F# 中链接函数的方法是使用 `|>` 完成的，这会导致类似的代码

```F#
let x =  
    param1 
    |> function1 
    |> function2 thatGetsAnotherFirstParameter 
    |> function3
```

这段代码大致翻译为 C#

```c#
var x = function3(
    function2(
        thatGetsAnotherFirstParameter,
        function1(
            Param1
        )
    )
);
```

另一个缺点是，很容易意外地将非空值返回给 godot。这将导致运行时错误，信号和 F# 类型也不能很好地混合。最后，async/await 在 F# 中的工作方式与 C# 完全不同。因此，虽然在 C# 中，你可以简单地使用 async/await 而不是 GDscript 的“yield”，但你需要依赖 F# 中的“ply”，甚至为你通常可以等待的 godot 值制作一个包装器。幸运的是，基于任务的 async 仍然工作正常（只需要使用 async.AwaitTask），ply 也有助于基于任务的异步。

不过，我个人并没有深入了解 async。对于我的游戏来说，WASM 构建是必须的，在编写本文时，C# 和 F# 的异步系统都被 WASM 破坏了https://github.com/godotengine/godot/issues/34506

## 如何设置它

你需要了解的第一件事是，godot 是硬编码的，只能附加 C# 脚本。这使得通过 mono 使用其他语言有点烦人，但有一个[插件](https://github.com/willnationsdev/godot-fsharp-tools)可以帮助你。

你只需要一个新文件夹，它是一个 F# 项目，并且已经加载了 godot。库也应该由 godot 创建的 C# 项目加载。插件可以为你做到这一点。只需安装它，然后转到 项目->工具->设置 F# 项目。专业提示：不要选择 godot 项目的根文件夹。如果你这样做，它将陷入无限循环。只需创建一个子目录。

完成后，您现在应该启用“自动生成 F# 脚本”。只需进入项目 -> 项目设置 -> mono -> F# 工具，填写默认命名空间和默认路径，然后启用它。

现在，下次您编写 C# 脚本时，将创建一个新的 F# 脚本，C# 脚本将自动扩展所述 F# 脚本中的类型。因此，几乎自动化了整个工作流程。

有一件事：可能是你的编辑器很愚蠢，当你打开一个 C# 文件时，不想认识到 F# 库的存在。我还没有找到解决这个问题的方法，只需确保 `dotnet build` 构建没有错误和/或 godot 能够运行项目即可。除非你想同时使用 C# 和 F#（这也是一个很好的策略），否则你的 C# 文件无论如何都是空的，所以在编辑器中破坏它们并不重要。

## 结束部分1。

下一部分将介绍如何在 F# 中做一些基本的事情。如果你等不及了，还有[这个概述](http://www.lkokemohr.de/fsharp_godot_part2.html)。它并没有涵盖我计划做的所有事情，但它仍然是一个良好的开端，也是我最初使用的。



# F# 和 Godot 第 2 部分

第 1 部分我讲述了为什么选择 F# 以及如何使用 godot 进行设置。

第 2 部分，我将介绍如何在 F# 中执行一些基本的 godot 操作，比如发出自定义信号。

第 3 部分将深入探讨 F#，并使用它使 Godot 中的库更易于使用。

## 通过 this._Ready 设置值

F# 的核心是函数式编程语言。Null 不被视为惯用语，更糟糕的是，如果你创建了一个不以某种值开头的属性，F# 会试图与你对抗。

幸运的是，F# 也有一个解决方案。而且，如果你问我，这比 `this._Ready` 好多了。

所以，这里有一段从我的纸牌游戏中提取的代码，说明了如何获取一个子节点并将其存储以供以后使用。

```F#
type LoginScreenFs() as this =
    inherit Control()

    let userNameNode =
        lazy (this.GetNode<LineEdit>(new NodePath("UserName")))
```

好吧，这很奇怪，所以让我们把它分解一下：

```F#
type LoginScreenFs() as this =
```

这只是声明了我们要创建的类型。`as this` 部分使 `let` 绑定可以使用 `this`。`inherit Control()` 用于告诉 F# 要扩展什么。在这种情况下，我们扩展了 Control 节点。

现在，有趣的部分来了

```F#
let userNameNode =
    lazy (this.GetNode<LineEdit>(new NodePath("UserName")))
```

`let userNameNode` 简单地创建一个私有字段。在这种情况下，值为 `lazy (this.GetNode<LineEdit>(new NodePath("UserName")))`。我们不必为 `userNameNode` 设置类型，因为 F# 能够将其推断为 `Lazy<LineEdit>`。

至于它是如何工作的，很简单：`lazy()` 接受一个表达式，并在第一次需要值时运行它。之后，它会记住该值，因此下次需要该值时，它可以简单地返回存储的值，而不必再次运行代码。

所以，换言之，不要在 `this._Ready` 设置 `userNameNode` 的值，我们还没有给它一个可用值，第一次需要值时，我们会搜索节点并记住它以备下次使用。除了我们不必检查自己之外，一切都已经由 `userNameNode.Value` 处理了。

为什么它更好？简单：不可能忘记在**构造函数**或 **`this.Ready`** 中为我们的字段赋值。忘记的越少，你能制造的错误就越少。你能制造的 bug 越少越好。

如果你真的需要一个没有值的字段，请使用选项。

```F#
type LoginScreenFs() as this =
    inherit Control()

    let  mutable userNameNode : Option<LineEdit>= None

    override this._Ready () =
        userNameNode <- Some(this.GetNode<LineEdit>(new NodePath("UserName")))
```

请记住，在使用此代码时，F# 会强制您处理 None（Null）情况。

## 异步任何东西

C# 和 F# 中的 async 有很多不同之处。让我们先来看一些代码示例，然后再将其分解。

C# 中的一个基本异步函数

```c#
public async Task<int> AnAsyncFunction(int a, int b)
{
    var newVal = await DoStuffWithAnInt(a);
    return a + b;
}
```

F# 中的相同函数

```F#
let AnAsyncFunction (a:int) (b:int) : Async<int> = 
    async {
        let! newVal = DoStuffWithAnInt a
        return a + b
    }
```

正如你所看到的，一切都不一样。这就是从 C# 切换到 F# 时的主题。让我们把它分解一下：

你首先注意到的是，F# 没有**异步函数**的概念，至少不像 C# 那样。相反，它使用**异步块**。

接下来，你会看到 F# 不使用 `await`，如果你注意了，你也可能会在块中看  `let! newVal = DoStuffWithAnInt a` 时看到 `let` 后面的 `!` 。

这个 `!` 是非常重要的，并且告诉 F# 您不需要函数的文字值（在本例中为 `Async<int>`），而只关心内部值（在该例中为 `int`）。换句话说，它的工作原理与 C# 中的 `await` 相同。

幕后还有更多的事情要做，因为 F# 允许你为其他类型编写类似的块。如果你想了解更多关于它是如何工作的，可以搜索计算表达式。在第 3 部分中，我们将使用一个来使一些语法更好一些。现在，你需要知道的就是 `let!` 在 C# 中做你想做的事情。

另一个区别是：F# 使用自己的类型 `Async<T>`，而不是使用 `Task<T>`。不过，您仍然可以在 F# 中使用 `Tasks`。只需要使用 `Async.AwaitTask` 转换它们。像这样

```F#
let! a = parameter1 |> someTaskFunction |> Async.AwaitTask
```

还有像 Ply 这样的库可以给你一个 `task` 块。这允许您编写与 `async` 块相同的代码，但可以使用 `Task`。

你必须记住的最后一个区别是：在 F# 中，你必须自己启动一个异步进程，使用 `Async.Start` 或启动许多其他功能之一。它们的行为都有点不同，但都有很好的记录。

## 房间里的大象：等待信号。

如前所述，使用 `ToSignal()`。在 C# 中，你可以使用 `await` 并结束它。在 F# 中，情况并非如此。这是因为 F# 和 C# 异步执行的方式存在差异，godot 的异步执行方式与标准 C# 方式略有不同。

Godot 不为 `ToSignal` 使用 `Task`，因此使用 `Async.AwaitTask` 对我们没有帮助。如果你想使用 `ToSignal`，你必须使用 `ply` 及其任务块。此外，由于一些奇怪的原因，尽管实现了所有功能，但 Godot 使用的类与 `ply` 不兼容。因此，您还需要将 `ToSignal` 的输出打包到自己的类中，使其兼容。

如果我没有因为 WASM 的问题而放弃 Async，我会使用这段包装器代码。

```F#
open System.Runtime.CompilerServices
open Godot

type SignalAwaiter2(wrapped: SignalAwaiter) =
    interface ICriticalNotifyCompletion with
        member this.OnCompleted(cont) = wrapped.OnCompleted(cont)
        member this.UnsafeOnCompleted(cont) = wrapped.OnCompleted(cont)

    member this.IsCompleted = wrapped.IsCompleted

    member this.GetResult() = wrapped.GetResult()

    member this.GetAwaiter() = this
```

使用它很简单（尽管不可否认，并不理想）：

```F#
let someFunc = 
    task {
        let! x = SignalAwaiter2(this.ToSignal(this.GetTree(), "idle_frame"))
    }
```

您可以编写一个扩展方法来自动包装它。ToSignal 到 SignalWaiter，但在我走到这一步之前，我已经停止使用这个系统了。

## 发射和收听（自定义）信号。

与 C# 相比没有太大变化。不过，有两件事你需要考虑。

首先，Godot 希望对通过 Signals 传递的值进行 `marshal`。这严重限制了你可以通过它们发送的内容。任何 F# 特定类型的类型，如 struct 记录，都不起作用。据我所知，Option/Result 等 F# 特定的东西也不起作用。如果你想通过信号发送它们，就像你用 C# 代码一样，请使用扩展 `Godot.Object` 的类。

此外，在 F# 中无法在类中定义子类型。因此，我还没有找到一种在 F# 中定义自定义信号的方法。然而，它有一个简单的解决方法：只要在你已经需要的 C# 文件中定义它们。由于 F# 代码作为库加载，您可以访问任何要发送的 F# 类型。这确实意味着在发出信号时，您不能在 F# 中使用 `nameof` 来获取信号的名称。

除了这两件事，它的工作原理完全相同。然而，如果这些限制是不允许的，那么 F# 和 C# 都有可以扮演相同角色的事件。您只需记住，每当删除节点时，都要手动取消订阅这些事件。

## 下一部分

下一部分将深入探讨 F# 及其一些特性，如**可区分联合**、**`match`** 和**计算表达式**。这些将用于对 Godot 的一个库进行包装，使其更易于使用。