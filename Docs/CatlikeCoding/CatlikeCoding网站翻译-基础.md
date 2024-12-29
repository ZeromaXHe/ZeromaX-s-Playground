# [返回主 Markdown](./CatlikeCoding网站翻译-基础.md)

# 游戏对象和脚本：创建一个钟表

发布于 2017-09-08 更新于 2021-05-18

https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/

*用简单的物体制作一个时钟。*
*写一个 C# 脚本。*
*旋转时钟的指针以显示时间。*
*为钟臂制作动画。*

这是关于学习使用 Unity [基础](https://catlikecoding.com/unity/tutorials/basics/)知识的系列教程中的第一篇。在其中，我们将创建一个简单的时钟，并对一个组件进行编程，使其显示当前时间。您还不需要对 Unity 编辑器有任何经验，但一般来说，您应该对多窗口编辑器应用程序有一些经验。

在我最近所有教程的底部，你会找到教程许可证的链接、包含已完成教程项目的存储库以及教程页面的 PDF 版本。

本教程使用 Unity 2020.3.6f1 编写。

![img](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/tutorial-image.jpg)

*是时候制作一个时钟了。*

## 1 创建项目

在开始使用 Unity 编辑器之前，我们必须先创建一个项目。

### 1.1 新建项目

当您打开 Unity 时，您将看到 Unity Hub。这是一个启动器和安装程序应用程序，您可以在其中创建或打开项目，安装 Unity 版本，并执行其他一些操作。如果您没有安装 Unity 2020.3 或更高版本，请立即添加。

> **哪些 Unity 版本是合适的？**
>
> Unity 每年发布多个新版本。有两个平行的发布时间表。最稳定、最安全的是 LTS 版本。LTS 代表长期支持，在 Unity 的情况下为两年。我坚持使用 LTS 版本的教程。本教程具体使用 2020.3.6。版本号的第三部分表示补丁发布。补丁版本包含错误修复，很少有新功能。另一个 f1 后缀表示官方最终版本。任何 2020.3 版本都适用于本教程。
>
> Unity 的最高版本是开发分支，它引入了新功能，并可能删除了旧功能。这些版本不如 LTS 版本可靠，每个版本只支持几个月。

偶尔，我的教程会包含一些小问题及其答案，总是放在一个灰色的盒子里，就像上面的那个。在网页上，默认情况下答案是隐藏的。这可以通过单击或点击问题来切换。

当你创建一个新项目时，你可以选择它的 Unity 版本和模板。我们将使用标准的 3D 模板。创建后，它将被添加到项目列表中，并在适当版本的 Unity 编辑器中打开。

> **我可以使用不同的渲染管道创建项目吗？**
>
> 是的，唯一的区别是项目的默认场景中会有更多的东西，你的材质看起来也会不同。您的项目还将包含相应的包。

### 1.2 编辑器布局

如果你还没有自定义编辑器，你最终会得到它的默认窗口布局。

![img](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/creating-a-project/default-layout.png)

*默认编辑器布局。*

默认布局包含我们需要的所有窗口，但您可以通过重新排序和分组窗口来自定义它。您还可以打开和关闭窗口，如资产存储中的窗口。每个窗口都有自己的配置选项，可以通过右上角的三点按钮访问。除此之外，大多数还具有更多选项的工具栏。如果你的窗口看起来与教程中的不同——例如场景窗口的背景是统一的，而不是天空框——那么它的一个选项就不同了。

您可以通过 Unity 编辑器右上角的下拉菜单切换到预配置的布局。您还可以将当前布局保存在那里，以便以后可以恢复到它。

### 1.3 包

Unity 的功能是模块化的。除了核心功能外，还有额外的包可以下载并包含在您的项目中。默认情况下，默认三维项目当前包含几个包，您可以在项目窗口的“*包（Packages）*”下看到这些包。

![img](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/creating-a-project/default-packages.png)

*默认包。*

这些包可以通过切换项目窗口右上角的按钮来隐藏，该按钮看起来像一只眼睛，中间有一个破折号。这纯粹是为了减少编辑器中的视觉混乱，这些包仍然是项目的一部分。该按钮还显示有多少个这样的包。

您可以通过包管理器控制项目中包含哪些包，包管理器可以通过*窗口/包管理器*菜单项打开。

![img](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/creating-a-project/package-manager.png)

*包管理器，仅显示项目中的包。*

这些软件包为 Unity 添加了额外的功能。例如，*Visual Studio Editor* 为用于编写代码的 Visual Studio 编辑器添加了集成。本教程不使用所包含包的功能，因此我将其全部删除。唯一的例外是 *Visual Studio Editor*，因为这是我用来编写代码的编辑器。如果您使用其他编辑器，则希望包含其集成包（如果存在）。

> **你不也需要 *Visual Studio Code Editor* 包吗？**
>
> 尽管名称相似，但 *Visual Studio* 和 *Visual Studio Code* 是两种不同的编辑器。您只需要其中一个包，具体取决于您使用的编辑器。

删除包的最简单方法是首先使用工具栏将包列表限制为*仅在项目中*。然后一次选择一个包，并使用窗口右下角的“*删除*”按钮。Unity 将在每次删除后重新编译，因此该过程需要几秒钟才能完成。

删除除 *Visual Studio Editor* 之外的所有内容后，我在项目窗口中只剩下三个可见的包：*Custom NUnit*, *Test Framework*, 和 *Visual Studio Editor*。另外两个仍然存在，因为 *Visual Studio Editor* 依赖于它们。

您可以通过项目设置窗口在包管理器中显示依赖关系和隐式导入的包，该窗口通过*编辑/项目设置...* 打开。选择其“*包管理器*”类别，然后在“*高级设置*”下启用“*显示依赖关系*”。

![img](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/creating-a-project/package-manager-settings.png)

*包管理器项目设置；显示已启用的依赖关系。*

### 1.4 色空间

如今，渲染通常在线性颜色空间中完成，但 Unity 默认情况下仍配置为使用 gamma 颜色空间。为了获得最佳的视觉效果，请选择项目设置窗口的“播放器”类别，打开“其他设置”面板，然后向下滚动到其“渲染”部分。确保“颜色空间”设置为“线性”。Unity 将显示警告，这可能需要很长时间，但对于一个几乎空白的项目来说，情况并非如此。确认切换。

![img](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/creating-a-project/color-space.png)

*颜色空间设置为线性。*

> **有理由使用伽玛颜色空间吗？**
>
> 仅当您针对旧硬件或旧图形 API 时。OpenGL ES 2.0 和 WebGL 1.0 不支持线性空间，而且在旧的移动设备上，gamma 可能比线性更快。

### 1.5 示例场景

新项目包含一个名为 *SampleScene* 的示例场景，默认情况下会打开该场景。您可以在项目窗口的“*资源/场景*”下找到其资源。

![img](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/creating-a-project/two-column-layout.png)

*项目窗口中的示例场景。*

默认情况下，项目窗口使用两列布局。您可以通过其三点配置菜单选项切换到单列布局。

![img](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/creating-a-project/one-column-layout.png)

*单栏布局。*

示例场景包含一个主摄影机和一个平行光。这些是游戏对象。它们列在场景下的层次窗口中。

![img](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/creating-a-project/sample-scene-hierarchy.png)

*场景中的对象层次结构。*

您可以通过层次窗口或场景窗口选择游戏对象。相机有一个看起来像老式胶片相机的场景图标，而平行光的图标看起来像太阳。

![img](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/creating-a-project/scene-icons.png)

*场景窗口中的图标。*

> **如何导航场景窗口？**
>
> 您可以将 alt 或选项键与光标结合使用来旋转视图。您还可以使用箭头键移动视点，并通过滚动进行缩放。此外，按 F 键将视图聚焦在当前选定的游戏对象上。还有更多的可能性，但这些足以让你在场景中找到方向。

当一个对象被选中时，它的详细信息将显示在检查器窗口中，但我们会在需要时覆盖这些信息。我们不需要修改相机或灯光，因此我们可以通过在层次窗口中单击它们左侧的眼睛图标将它们隐藏在场景中。默认情况下，此图标不可见，但当我们将光标悬停在那里时会出现。这纯粹是为了减少场景窗口中的视觉混乱。

![img](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/creating-a-project/hidden-objects.png)

*隐藏对象。*

> **眼睛旁边的手状图标是做什么的？**
>
> 在包含眼睛图标的列旁边是另一个包含手状图标的列。默认情况下，这些图标也是不可见的。当游戏对象的手形图标处于活动状态时，无法通过场景窗口选择对象。这样，您可以通过场景窗口控制哪些对象对选择做出响应。

## 2 制作一个简单的时钟

现在我们的项目设置正确，我们可以开始创建时钟了。

### 2.1 创建游戏对象

我们需要一个游戏对象来代表时钟。我们将从最简单的游戏对象开始，它是一个空的。它可以通过 *游戏对象/创建空* 菜单选项创建。或者，您可以在层次结构窗口的上下文菜单中使用“*创建空*”选项，您可以通过另一种单击方式打开该窗口，通常是右键单击或用两根手指轻击。这将把游戏对象添加到场景中。它在 *SampleScene* 下的层次窗口中可见并立即被选中，该窗口现在用星号标记，表示它有未保存的更改。您也可以立即更改其名称或稍后再更改。

![img](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/building-a-simple-clock/new-game-object.png)

*已选择新游戏对象的层次结构。*

只要选择了游戏对象，检查器窗口就会显示其详细信息。它的顶部是一个标题，上面有对象的名称和一些配置选项。默认情况下，该对象处于启用状态，不是静态的，没有标记，并且位于默认层上。除了名称之外，这些设置都很好。将其重命名为 *Clock*。

![img](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/building-a-simple-clock/inspector.png)

*选中时钟的检查器窗口。*

标题下方是游戏对象所有组件的列表。该列表的顶部始终有一个 `Transform` 组件，这是我们时钟当前的全部内容。它控制游戏对象的位置、旋转和缩放。确保所有时钟的位置和旋转值都设置为 0。其刻度应统一为 1。

> **那么 2D 对象呢？**
> 在 2D 而不是 3D 中工作时，可以忽略三个维度中的一个。专门用于 2D 的对象（如 UI 元素）通常有一个 `RectTransform`，这是一个专门的 `Transform` 组件。

因为游戏对象是空的，所以在场景窗口本身中不可见。然而，在游戏对象位于世界中心的位置，可以看到一个操纵工具。

![img](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/building-a-simple-clock/move-tool.png)

*使用移动工具选择。*

> **为什么在选择时钟后我看不到操纵工具？**
>
> 操纵工具存在于场景窗口中。请确保您正在查看场景窗口，而不是游戏窗口。

可以通过编辑器工具栏左上角的按钮控制哪个操作工具处于活动状态。这些模式也可以通过Q、W、E、R、T和Y键激活。组中最右侧的按钮用于启用我们没有的自定义编辑器工具。默认情况下，移动工具处于活动状态。

![img](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/building-a-simple-clock/manipulation-toolbar.png)

*操纵模式工具栏。*

模式按钮旁边还有三个按钮，用于控制操纵工具的放置、方向和捕捉。

### 2.2 创造时钟的面

虽然我们有一个时钟对象，但我们还没有看到任何东西。我们必须向其中添加 3D 模型，以便渲染某些内容。Unity 包含一些可以用来构建简单时钟的原始对象。让我们首先通过 *GameObject/3D Object/Clinder* 向场景中添加一个圆柱体。确保它与我们的时钟具有相同的 `Transform` 值。

![scene](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/building-a-simple-clock/cylinder-scene.png) ![inspector](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/building-a-simple-clock/cylinder-inspector.png)

*代表圆柱体的游戏对象。*

新对象比空游戏对象多了三个组件。首先，它有一个 `MeshFilter`，其中包含对内置圆柱体网格的引用。

![img](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/building-a-simple-clock/mesh-filter.png)

*`MeshFilter` 组件，设置为圆柱体。*

第二个是 `MeshRenderer`。此组件的目的是确保对象的网格得到渲染。它还决定了用于渲染的材质，即默认材质。该材料也显示在组件列表下方的检查员中。

![img](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/building-a-simple-clock/mesh-renderer.png)

*`MeshRenderer` 组件，设置为默认材质。*

第三个是 `CapsuleCollider`，用于 3D 物理。该对象表示一个圆柱体，但它有一个胶囊碰撞体，因为 Unity 没有基本圆柱体对撞机。我们不需要它，所以我们可以删除这个组件。如果你想在时钟中使用物理学，最好使用 `MeshCollider` 组件。可以通过右上角的三点下拉菜单删除组件。

![img](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/building-a-simple-clock/cylinder-without-collider.png)

*无碰撞体的圆柱体。*

我们将通过使圆柱体变平来将其变成钟面。这是通过减小其刻度的 Y 分量来实现的。将其减少到 0.2。由于圆柱体网格的高度为两个单位，其有效高度变为 0.4 个单位。让我们制作一个大钟，将其刻度的 X 和 Z 分量增加到10。

![scene](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/building-a-simple-clock/cylinder-scaled-scene.png)
![inspector](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/building-a-simple-clock/cylinder-scaled-inspector.png)

*缩放圆柱体。*

我们的钟应该站在或挂在墙上，但它的表面目前是平的。我们可以通过将圆柱体旋转四分之一圈来解决这个问题。在 Unity 中，X 轴指向右侧，Y 轴指向上方，Z 轴指向前方。所以，让我们在设计时钟时考虑到相同的方向，这意味着我们在沿着 Z 轴看它的同时看到它的正面。将圆柱体的 X 旋转设置为 90，并调整场景视图，使时钟的前部可见，从而移动工具的蓝色 Z 箭头指向远离您的屏幕。

![scene](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/building-a-simple-clock/cylinder-rotated-scene.png)
![inspector](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/building-a-simple-clock/cylinder-rotated-inspector.png)

*旋转圆柱体。*

将圆柱体对象的名称更改为 *Face*，因为它代表时钟的面。它只是时钟的一部分，所以我们将其作为 *Clock* 对象的子对象。我们通过将面部拖动到层次结构窗口中的时钟上来实现这一点。

![img](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/building-a-simple-clock/child-object.png)

*Face 子对象。*

子对象受制于其父对象的转换。这意味着当 *Clock* 改变位置时，*Face* 也会改变位置。就好像他们是一个单一的实体。旋转和缩放也是如此。您可以使用它来创建复杂的对象层次结构。

### 2.3 创建时钟外围设备

时钟表面的外圈通常有标记，有助于指示它显示的时间。这被称为时钟外围。让我们用方块来表示 12 小时制的小时数。

通过 *GameObject/3D object/cube* 将立方体对象添加到场景中，将其命名为 *Hour Indicator 12*，并使其成为 *Clock* 的子对象。子对象在层次结构中的顺序并不重要，您可以将其放置在面的上方或下方。

![img](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/building-a-simple-clock/hour-indicator-child.png)

*小时指示器子对象。*

将其 X 比例设置为 0.5，Y 比例设置为 1，Z 比例设置为 0.1，使其成为一个狭窄的扁平长块。然后将其 X 位置设置为 0，Y 位置设置为 4，Z 位置设置为 -0.25。这把它放在脸上，表示 12 点。同时删除其 `BoxCollider` 组件。

![scene](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/building-a-simple-clock/indicator-scene.png) ![inspector](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/building-a-simple-clock/indicator-inspector.png)

*12 小时指示器。*

指示器很难看到，因为它与面颜色相同。让我们通过*资产/创建/材质*，或通过项目窗口的加号按钮或上下文菜单为其创建一个单独的材质。这为我们提供了一个与默认材料重复的材料资产。将其名称更改为*小时指示器*。

![one-column](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/building-a-simple-clock/material-project-one-column.png)
![two-column](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/building-a-simple-clock/material-project-two-column.png)

*项目窗口中的小时指示器，一列和两列布局。*

选择材质，然后通过单击其颜色字段将其*反照率(Albedo)*更改为其他值。这将打开一个颜色弹出窗口，提供各种选择颜色的方法。我选择了深灰色，对应于十六进制 494949，这与 RGB 0-255 模式的均匀 73 相同。我们不使用 alpha 通道，所以它的值无关紧要。我们也可以保持所有其他材料属性不变。

![color popup](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/building-a-simple-clock/hour-indicator-material.png)

*深灰色反照率。*

> **什么是反照率？**
>
> Albedo 是一个拉丁词，意思是白色。它是某物在白光照射下的颜色。

使用这种材料制作小时指示器。您可以通过将材质拖动到场景或层次窗口中的对象上来实现此操作。您还可以在选择指示器游戏对象时将其拖动到检查器窗口的底部，或更改其 `MeshRenderer` 的“*材质*”数组的“*元素 0*”。

![scene](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/building-a-simple-clock/dark-hour-indicator-scene.png) ![inspector](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/building-a-simple-clock/dark-hour-indicator-inspector.png)

*黑暗时间指示器。*

### 2.4 十二小时指示器

我们可以用一个指标来衡量第 12 小时，但让我们每小时都包括一个指标。首先，调整场景视图摄影机的方向，使我们沿 Z 轴直视。您可以通过单击场景视图右上角的视图摄影机小控件的轴锥来完成此操作。您还可以通过网格工具栏按钮将场景网格的轴更改为 Z。

![img](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/building-a-simple-clock/looking-along-z-axis.png)

*沿着 Z 轴直视时钟。*

复制小时指示器 12 游戏对象。您可以通过“*编辑/复制*”、指定的键盘快捷键或层次窗口中的上下文菜单来执行此操作。重复项将出现在层次结构窗口中的原始项下方，也是 *Clock* 的子项。其名称设置为*小时指示器12（1）*。将其重命名为*小时指示器 6*，并取消其位置的 Y 分量，使其指示小时 6。

![hierarchy](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/building-a-simple-clock/hour-indicator-6-hierarchy.png)
![scene](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/building-a-simple-clock/hour-indicator-6-scene.png)

*6 小时和 12 小时的指示器。*

以相同的方式为第 3 小时和第 9 小时创建指标。在这种情况下，它们的 X 位置应该是 4 和 -4，而它们的 Y 位置应该是零。此外，将它们的 Z 旋转设置为 90，这样它们就转了四分之一圈。

![img](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/building-a-simple-clock/four-hour-indicators.png)

*四个小时指示器。*

然后创建另一个小时指示器 12 的副本，这次是小时 1。将其 X 位置设置为 2，Y 位置设置为 3.464，Z 旋转设置为 -30。然后在 2 小时内复制一个，交换它的 X 和 Y 位置，并将它的 Z 旋转加倍到 -60。

![img](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/building-a-simple-clock/hour-indicators-1-2.png)

*小时 1 和 2 的指示器。*

> **这些数字是从哪里来的？**
>
> 每小时沿 Z 轴顺时针旋转 30°。在这种情况下，我们使用负旋转，因为 Unity 的旋转是逆时针的。我们可以通过三角几何法找到 1 小时的位置。30° 的正弦为 ½，余弦为 $\frac{\sqrt 3}{2}$。我们根据小时指示器与中心的距离进行缩放，即 4。所以我们最终得到了 X 2 和 Y $2\sqrt 3 \approx 3.4.64$。对于第 2 小时，旋转角度为 60°，我们可以简单地交换正弦和余弦。

复制这两个指示器，并取负它们的 Y 位置和旋转，以创建 4 小时和 5 小时的指示器。然后在小时 1、2、4 和 5 上使用相同的技巧来创建其余的指示器，这次是否定它们的 X 位置，再次否定它们的旋转。

![img](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/building-a-simple-clock/all-hour-indicators.png)

*所有小时指示器。*

### 2.5 制造钟臂

下一步是制作时钟的臂。我们从时针开始。再次复制 *小时指示器 12*，并将其命名为*小时臂*。然后创建一个 *Clock Arm* 材质并让臂使用它。在这种情况下，我将其设置为纯黑色，十六进制为 000000。将臂的 X 比例减小到 0.3，将其 Y 比例增大到 2.5。然后将其 Y 位置更改为 0.75，使其指向第 12 小时，但也指向相反的方向。这使得手臂在旋转时看起来好像有一点配重。

![scene](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/building-a-simple-clock/hours-arm-scene.png) ![inspector](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/building-a-simple-clock/hours-arm-inspector.png)

*小时臂。*

臂必须围绕时钟的中心旋转，但改变其 Z 轴旋转会使其围绕自己的中心旋转。

*时钟臂围绕其中心旋转。*

这是因为旋转相对于游戏对象的局部位置。为了创建适当的旋转，我们必须引入一个枢轴对象并旋转该对象。因此，创建一个新的空游戏对象，并将其作为 *Clock* 的子对象。您可以通过层次结构窗口中时钟的上下文菜单直接创建对象来完成此操作。将其命名为 *Hours Arm Pivot*，并确保其位置和旋转为零，刻度统一为 1。然后将 *Hours Arm* 设置为枢轴的子节点。

![img](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/building-a-simple-clock/hours-arm-hierarchy.png)

*带枢轴的手臂。*

现在尝试旋转枢轴。如果通过场景视图执行此操作，请确保将工具手柄位置模式设置为“*枢轴*”而不是“*中心*”。

*时钟臂围绕枢轴旋转。*

将小时臂枢轴复制两次，以创建 *分钟臂枢轴* 和 *秒臂枢轴*。相应地重命名它们，包括复制的臂子对象。

![img](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/building-a-simple-clock/all-arms-hierarchy.png)

*所有臂层次结构。*

分钟臂应比小时臂更窄、更长，因此将其 X 刻度设置为 0.2，Y 刻度设置为 4，然后将其 Y 位置增加到 1。还将其 Z 位置更改为 -0.35，使其位于时针臂的顶部。请注意，这适用于臂，而不是其枢轴。

![img](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/building-a-simple-clock/minutes-arm-transform.png)

*分钟臂的变换。*

同时调整*秒臂*。这次 XY 刻度使用 0.1 和 5，YZ 位置使用 1.25 和 -0.45。

![img](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/building-a-simple-clock/seconds-arm-transform.png)

*秒臂的变换。*

让我们通过为它创建一个单独的材质来突出秒针。我给它一个深红色，十六进制 B30000。此外，当我们完成时钟的构建时，我关闭了场景窗口中的网格。

![img](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/building-a-simple-clock/all-arms-scene.png)

*三臂钟。*

如果您还没有这样做，现在是通过*文件/保存*或指定的键盘快捷键保存场景的好时机。

保持项目资产的有序性也是一个好主意。由于我们有三种材质，让我们将它们放在通过 *Assets/Create/Folder* 或项目窗口创建的材质文件夹中。然后，您可以将材质拖动到那里。

![one-column](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/building-a-simple-clock/material-folder-one-column.png)
![two-column](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/building-a-simple-clock/material-folder-two-column.png)

*项目窗口中的材质文件夹，一列和两列布局。*

## 3 时钟动画

我们的钟现在不报时，总是卡在十二点钟。为了给它设置动画，我们必须向它添加一个自定义行为。我们通过创建一个自定义组件类型来实现这一点，该类型是通过脚本定义的。

### 3.1 C# 脚本资源

通过 *Assets/Create/C# Script* 将新的脚本资源添加到项目中，并将其命名为 *Clock*。C# 是用于 Unity 脚本的编程语言，发音为 *C-sharp*。让我们也立即把它放在一个新的 *Scripts* 文件夹中，以保持项目的整洁。

![one-column](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/animating-the-clock/script-asset-one-column.png)
![two-column](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/animating-the-clock/script-asset-two-column.png)

*脚本文件夹，带时钟脚本，一列和两列布局。*

选择脚本后，检查器将显示其内容。但是要编辑代码，我们必须使用代码编辑器。您可以按“*打开…*”打开脚本进行编辑。在检查器中单击按钮，或在层次结构窗口中双击它。打开哪个程序可以通过 Unity 的首选项进行配置。

![img](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/animating-the-clock/clock-inspector.png)

*C# `Clock` 资产检查器。*

### 3.2 定义组件类型

在代码编辑器中加载脚本后，首先删除标准模板代码，因为我们将从头开始创建组件类型。

空文件不定义任何内容。它必须包含我们时钟组件的定义。我们要定义的不是组件的单个实例。相反，我们定义了称为 `Clock` 的通用类或类型。一旦建立，我们就可以在 Unity 中创建多个这样的组件，尽管在本教程中我们将把自己限制在一个时钟上。

在 C# 中，我们定义 `Clock` 类型，首先声明我们正在定义一个类，然后是它的名称。在下面的代码片段中，更改后的代码背景为黄色，如果您使用深色网页主题查看本教程，则背景为深红色。当我们从一个空文件开始时，它的内容应该变成 `class Clock`，而不是其他，不过你可以根据需要在单词之间添加空格和换行符。

```c#
class Clock
```

> **从技术上讲，什么是类？**
>
> 您可以将类视为一个蓝图，用于创建驻留在计算机内存中的对象。蓝图定义了这些对象包含哪些数据以及它们具有哪些功能。
>
> 类还可以定义不属于对象实例但属于类本身的数据和功能。这通常用于提供全局可用的功能。我们将使用其中的一些，但 `Clock` 没有。

因为我们不想限制哪些代码可以访问我们的 `Clock` 类型，所以最好在它前面加上 `public` 访问修饰符。

```c#
public class Clock
```

> **类的默认访问修饰符是什么？**
>
> 如果没有访问修饰符，就好像我们编写了 `internal class Clock` 一样。这将限制对同一程序集中代码的访问，当您使用打包在单独程序集中的代码时，这将变得相关。为了确保它始终有效，默认情况下将类设置为公共。

目前，我们还没有有效的 C# 语法。如果要保存文件并返回 Unity 编辑器，则编译错误将记录在其控制台窗口中。

我们表示我们正在定义一个类型，所以我们必须实际定义它是什么样子的。这是通过声明后的一段代码完成的。代码块的边界用花括号表示。我们暂时把它留空，所以只需写 `{}`。

```c#
public class Clock {}
```

我们的代码现在有效。保存文件并切换回 Unity。Unity 编辑器将检测到脚本资源已更改并触发重新编译。完成后，选择我们的脚本。检查员将通知我们，该资产不包含 `MonoBehaviour` 脚本。

![img](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/animating-the-clock/non-component.png)

*非组件脚本。*

这意味着我们不能使用此脚本在 Unity 中创建组件。此时，我们的 `Clock` 定义了一个基本的 C# 对象类型。我们的自定义组件类型必须扩展 Unity 的 `MonoBehaviour` 类型，继承其数据和功能。

> **mono-behavior 是什么意思？**
>
> 我们的想法是，我们可以对自己的组件进行编程，为游戏对象添加自定义行为。这就是行为部分所指的。它只是碰巧使用了英国拼写，这很奇怪。mono 部分是指 Unity 中添加对自定义代码的支持的方式。它使用了 Mono 项目，这是一个多平台的 .NET 框架实现。因此，`MonoBehaviour`。由于向后兼容性，我们一直沿用这个旧名字。

要将 `Clock` 转换为 `MonoBehaviour` 的一个子类型，我们必须更改我们的类型声明，使其扩展该类型，这是通过在我们的类型名称后加一个冒号，后跟它扩展的内容来完成的。这使得 `Clock` 继承了 `MonoBehaviour` 类类型的所有内容。

```c#
public class Clock : MonoBehaviour {}
```

但是，这将导致编译后出现错误。编译器抱怨找不到 `MonoBehaviour` 类型。这是因为该类型包含在名称空间中，即 `UnityEngine` 中。要访问它，我们必须使用它的完全限定名 `UnityEngine.MonoBehaviour`。

```c#
public class Clock : UnityEngine.MonoBehaviour {}
```

> **什么是命名空间？**
>
> 命名空间类似于网站域，但用于代码。就像域可以有子域一样，命名空间也可以有子命名空间。最大的区别在于，它是反过来写的。所以不是 forum.unity.com，而是 com.unity.forum。命名空间用于组织代码并防止名称冲突。
>
> Unity 附带了包含 `UnityEngine` 代码的程序集，您不必联机单独获取它。如果导入了相应的编辑器集成包，则应自动设置代码编辑器使用的项目文件以识别它。

在访问 Unity 类型时，总是必须包含 `UnityEngine` 前缀是不方便的。幸运的是，我们可以声明应该自动搜索名称空间以完成 C# 文件中的类型名称。这是通过在文件的顶部添加 `using UnityEngine;` 来完成的。分号是标记语句结束所必需的。

```c#
using UnityEngine;

public class Clock : MonoBehaviour {}
```

现在，我们可以将自定义组件添加到 Unity 中的 *Clock* 游戏对象中。这可以通过将脚本资源拖动到对象上或通过对象检查器底部的“*添加组件*”按钮来完成。

![img](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/animating-the-clock/clock-with-component.png)

*时钟游戏对象与我们的 `Clock` 组件。*

请注意，我的教程中的大多数代码类型都链接到在线文档。例如，[`MonoBehaviour`](http://docs.unity3d.com/Documentation/ScriptReference/MonoBehaviour.html) 是一个链接，可以将您带到该类型的 Unity 的在线脚本 API 页面。

### 3.3 抓住一只臂

要旋转臂，`Clock` 对象需要了解它们。让我们从时针开始。与所有游戏对象一样，可以通过调整其 `Transfrom` 组件来旋转它。因此，我们必须将臂枢轴的 `Transform` 组件的知识添加到 `Clock` 中。这可以通过在代码块中添加一个数据字段来实现，该字段定义为名称后跟分号。

名称 `hours pivot` 将适用于该字段。然而，名字必须是单个单词。惯例是将字段名的第一个单词小写，并将所有其他单词大写，然后将它们粘在一起。所以我们把它命名为 `hoursPivot`。

```c#
public class Clock : MonoBehaviour {

	hoursPivot;
}
```

> **使用声明去哪里了？**
>
> 它仍然存在，我只是没有显示它。代码片段将包含足够的现有代码，以便您了解更改的上下文。

我们还必须声明字段的类型，在本例中为 `UnityEngine.Transform`。它必须写在字段名称的前面。

```c#
	Transform hoursPivot;
```

我们的类现在定义了一个字段，可以保存对另一个对象的引用，该对象的类型必须是 `Transform`。我们必须确保它包含对时针轴 `Transform` 组件的引用。

默认情况下，字段是私有的，这意味着它们只能由属于 `Clock` 的代码访问。但是类不知道我们的 Unity 场景，所以没有直接的方法将字段与正确的对象相关联。我们可以通过将字段声明为可序列化来改变这一点。这意味着，当 Unity 保存场景时，它应该包含在场景的数据中，这是通过将所有数据按顺序排列（序列化）并将其写入文件来实现的。

将字段标记为可序列化是通过向其附加属性来完成的，在本例中为 `SerializeField`。它写在方括号之间的字段声明前面，通常在它上面的一行，但也可以放在同一行。

```c#
	[SerializeField]
	Transform hoursPivot;
```

> **我们不能 `public` 吗？**
>
> 是的，但让类字段公开访问通常是不好的。经验法则是，只有当其他类型的 C# 代码需要访问类内容时，才将类内容公开，然后更喜欢方法或属性而不是字段。不易访问的东西越容易维护，因为可以直接依赖它的代码越少。在本教程中，我们唯一的 C# 代码是 Clock，所以没有理由公开其内容。

一旦字段可序列化，Unity 将检测到这一点，并将其显示在 *Clock* 游戏对象的 `Clock` 组件的检查器窗口中。

![img](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/animating-the-clock/hours-pivot-field.png)

*小时枢轴字段。*

要进行正确的连接，请将 *Hours Arm Pivot* 从层次结构拖动到 *Hours Pivot* 字段上。或者，使用字段右侧的圆形按钮，在弹出的列表中搜索枢轴。在这两种情况下，Unity 编辑器都会抓取 *Hours Arm Pivot* 的 `Transform` 组件，并在我们的字段中引用它。

![img](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/animating-the-clock/hours-pivot-connected.png)

*小时枢轴已连接。*

### 3.4 了解所有三只臂

我们必须对分钟和秒臂枢轴做同样的事情。因此，在 `Clock` 中添加两个具有适当名称的可序列化 `Transform` 字段。

```c#
	[SerializeField]
	Transform hoursPivot;

	[SerializeField]
	Transform minutesPivot;

	[SerializeField]
	Transform secondsPivot;
```

可以使这些字段声明更简洁，因为它们共享相同的属性、访问修饰符和类型。它们可以合并为一个逗号分隔的字段名列表，位于属性和类型声明之后。

```c#
	[SerializeField]
	Transform hoursPivot, minutesPivot, secondsPivot;

	//[SerializeField]
	//Transform minutesPivot;

	//[SerializeField]
	//Transform secondsPivot;
```

> **`//` 做什么？**
>
> 双斜线表示注释。编译器会忽略它们之后直到行尾的所有文本。如果需要，它用于添加文本以澄清代码。我还用它来表示已删除的代码。除此之外，删除的代码中还有一条线穿过。

把编辑器的另外两只臂也挂起来。

![img](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/animating-the-clock/all-pivots-connected.png)

所有三个枢轴都连接在一起。

### 3.5 苏醒

现在我们可以使用臂枢轴了，下一步是旋转它们。为此，我们需要告诉 Clock 执行一些代码。这是通过向类中添加一个代码块（称为方法）来实现的。该块必须以一个名称作为前缀，按照惯例，该名称应大写。我们将其命名为 `Awake`，建议在组件唤醒时执行代码。

```c#
public class Clock : MonoBehaviour {

	[SerializeField]
	Transform hoursPivot, minutesPivot, secondsPivot;

	Awake {}
}
```

例如，方法有点像数学函数 $f(x) = 2x + 3$。该函数接受一个数字，由变量参数表示 $x$ ——将其加倍，然后加三。它对一个数字进行运算，其结果也是一个数字。就方法而言，它更像 $f(p)=c$ 其中 $p$ 表示输入参数和 $c$ 表示它执行的任何代码。

就像数学函数一样，方法可以产生结果，但这不是必需的。我们必须声明结果的类型——就像它是一个字段一样——或者写 `void` 来表示没有结果。在我们的例子中，我们只想执行一些代码而不提供结果值，所以我们使用 `void`。

```c#
	void Awake {}
```

我们也不需要任何输入数据。但是，我们仍然必须将方法的参数定义为圆括号之间的逗号分隔列表。在我们的情况下，这只是一个空列表。

```c#
	void Awake () {}
```

我们现在有一个有效的方法，尽管它还没有做任何事情。就像 Unity 检测到我们的字段一样，它也检测到这种 `Awake` 方法。当组件具有 `Awake` 方法时，Unity 将在组件唤醒时调用该方法。这是在游戏模式下创建或加载后发生的。我们目前处于编辑模式，所以这还没有发生。

> **`Awake` 不一定要 `public` 吗？**
>
> `Awake` 和一系列其他方法被认为是特殊的 Unity 事件方法。Unity 引擎将找到它们并在适当的时候调用它们，无论我们如何声明它们。这发生在管理层 .NET 环境之外。

请注意，[`Awake`](http://docs.unity3d.com/Documentation/ScriptReference/MonoBehaviour.Awake.html) 和其他特殊的 Unity 事件方法在我的教程中有粗体文本，并链接到他们的在线 Unity 脚本 API 页面。

### 3.6 通过代码旋转

要旋转臂，我们必须创建一个新的旋转。我们可以通过为变换的 `localRotation` 属性指定一个新的变换来更改变换的旋转。

> **什么是属性？**
>
> 属性是一种假装为字段的方法。它可能是只读的或只写的。C# 的约定是将属性大写，但 Unity 的代码不这样做。

尽管在检查器中，`Transform` 组件的旋转是用每个轴的欧拉角（度）定义的，但在代码中，我们必须用四元数来实现。

> **四元数是什么？**
>
> 四元数基于复数，用于表示 3D 旋转。虽然比单独的 X、Y 和 Z 旋转角度的组合更难理解，但它们有一些有用的特征。例如，它们不会受到万向节锁的影响。

我们可以通过调用四元数来基于欧拉角创建 `Quaternion.Euler` 方法。通过在 `Awake` 中写入它，然后用分号结束语句来完成此操作。

```c#
	void Awake () {
		Quaternion.Euler;
	}
```

该方法具有用于描述所需旋转的参数。在这种情况下，我们将在方法名称后提供一个逗号分隔的包含三个参数的列表，所有参数都在圆括号之间。我们为 X、Y 和 Z 旋转提供了三个数字。前两个使用零，Z 旋转使用 -30。

```c#
	Quaternion.Euler(0, 0, -30);
```

此调用的结果是一个包含围绕 Z 轴顺时针旋转 30° 的四元数结构值，与我们时钟上的小时 1 相匹配。

> **什么是 struct？**
> struct（structure的缩写）是一个蓝图，就像一个类一样。不同之处在于，它创建的任何内容都被视为一个简单的值，如整数或颜色，而不是对象。它没有身份感。定义自己的结构与定义类的工作原理相同，只是您编写的是 `struct` 而不是 `class`。

要将此旋转应用于时针，请使用 `=` 赋值语句将 `Quaternion.Euler` 的结果赋值给 `hoursPivots.localRotation`。

```c#
		hoursPivot.localRotation = Quaternion.Euler(0, 0, -30);
```

> **`localRotation` 和 `rotation` 有什么区别？**
>
> `localRotation` 属性表示由 `Transform` 组件单独描述的旋转，因此它是相对于其父级的旋转。这是你在它的检查器中看到的旋转。相比之下，旋转属性表示世界空间中的最终旋转，同时考虑到整个对象层次。如果我们整体旋转时钟，设置该属性会产生奇怪的结果，因为当该属性补偿时钟的旋转时，臂会忽略这一点。

> **难道不应该有一个警告说 `hoursPivot` 从未初始化过吗？**
>
> 编译器可以检测到没有代码为字段分配任何内容，并且确实可以发出这样的警告，因为它不知道我们是通过 Unity 的检查器设置的。但是，默认情况下会抑制此警告。抑制可以通过项目设置进行控制。在*播放器/其他设置/脚本编译*下有一个*抑制常见警告开关*。它会抑制有关未初始化和未使用的私有字段的警告。

现在在编辑器中进入播放模式。您可以通过“*编辑/播放*”、指定的键盘快捷键或按下编辑器窗口顶部中心的播放按钮来完成此操作。Unity 会将焦点切换到游戏窗口，该窗口会渲染场景中*主摄影机*看到的内容。时钟组件将唤醒，时钟将设置为 1 点钟。

![img](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/animating-the-clock/one-o-clock.png)

*游戏模式总是一点钟。*

如果相机没有聚焦在时钟上，您可以移动它，使时钟可见，但请记住，退出播放模式时场景会重置，因此您在播放模式下对场景所做的任何更改都不会持续。然而，对于资产来说，情况并非如此，对它们的更改总是持续存在的。您还可以在游戏模式下打开场景窗口，甚至多个场景和游戏窗口。在继续之前退出播放模式。

### 3.7 获取当前时间

下一步是计算出我们醒来的当前时间。我们可以使用 `DateTime` 结构来访问我们正在运行的设备的系统时间。`DateTime` 不是 Unity 类型，它位于 `System` 命名空间中。它是 .NET 框架的核心功能的一部分，Unity 使用它来支持脚本编写。

`DateTime` 具有 `Now` 属性，该属性生成包含当前系统日期和时间的 `DateTime` 值。为了检查它是否正确，我们将在 `Awake` 开始时将其记录到控制台。我们可以通过将其传递给 `Debug.Log` 方法来实现这一点。

```c#
using System;
using UnityEngine;

public class Clock : MonoBehaviour {

	[SerializeField]
	Transform hoursPivot, minutesPivot, secondsPivot;

	void Awake () {
		Debug.Log(DateTime.Now);
		hoursPivot.localRotation = Quaternion.Euler(0, 0, -30);
	}
}
```

现在，每次进入播放模式时，我们都会记录一个时间戳。您可以在控制台窗口和编辑器窗口底部的状态栏中看到它。

### 3.8 旋转臂

我们快到一个工作时钟了。让我们再次从小时开始。`DateTime` 有一个 `Hour` 属性，它为我们提供 `DateTime` 值的小时部分。在当前时间戳上调用它将为我们提供一天中的小时数。

```c#
		Debug.Log(DateTime.Now.Hour);
```

因此，要让小时臂显示当前小时，我们必须将 -30° 旋转乘以当前小时。乘法是用星号 * 字符完成的。我们也不再需要记录当前时间，这样就可以摆脱这种说法。

```c#
		//Debug.Log(DateTime.Now.Hour);
		hoursPivot.localRotation = Quaternion.Euler(0, 0, -30 * DateTime.Now.Hour);
```

![img](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/animating-the-clock/four-o-clock.png)

*目前四点钟处于播放模式。*

为了清楚地表明我们正在从小时转换为度数，我们可以定义一个包含转换因子的 `hoursToDegrees` 字段。`Quaternion.Euler` 的角度被定义为浮点值，因此我们将使用 `float` 类型。因为我们已经知道这个数字，所以我们可以立即将其作为字段声明的一部分进行分配。然后乘以字段，而不是 `Awake` 中的 `-30`。

```c#
	float hoursToDegrees = -30;

	[SerializeField]
	Transform hoursPivot, minutesPivot, secondsPivot;

	void Awake () {
		hoursPivot.localRotation =
			Quaternion.Euler(0, 0, hoursToDegrees * DateTime.Now.Hour);
	}
```

> **什么是 `float`？**
>
> 计算机不能存储所有数字，它们必须在由 0 或 1 位组成的二进制存储器中表示。这使得不可能在有限的内存大小内精确存储许多数字，就像我们不能用十进制表示法精确地写出 ⅓ 这个数字一样。我们能做的最好的事情就是写 0.3333333，然后在某个时候停止。
>
> 假设我们决定在点后最多写三位数字，在点前只写一位数字。那么 ⅓ 近似为 0.333。如果我们将 ⅓ 除以 100，那么我们将被迫写出 0.003，这意味着我们失去了两位数的精度。为了提高小值的精度，让我们添加一个单独的指数来指示我们数字的数量级。那么 $0.333 \times10^{-2}$ 可以表示 ⅓ 除以 100，而不会丢失有意义的数字。我们可以使用 $0.333 \times 10^2$ 也表示乘以 100，同时在点前只保留一个数字。因此，这个点可以被认为是浮动的，因为它没有指定固定的数量级。这允许我们只使用几个数字来表示大量的数字。
>
> 浮点数对计算机的工作方式相同，除了它们使用二进制而不是十进制数字，还必须表示特殊值，如无穷大和非数字。`float` 是一个存储在四个字节中的值，这意味着它有 32 位。

如果我们声明一个没有后缀的整数，则假定它是一个整数，这是一种不同的值类型。虽然编译器会自动转换它们，但让我们通过向它们添加 f 后缀来明确所有数字都是 `float` 类型。

```c#
	float hoursToDegrees = -30f;

	[SerializeField]
	Transform hoursPivot, minutesPivot, secondsPivot;

	void Awake () {
		hoursPivot.localRotation =
			Quaternion.Euler(0f, 0f, hoursToDegrees * DateTime.Now.Hour);
	}
```

每小时的度数总是相同的。我们可以通过在 `hoursToDegrees` 的声明中添加 `const` 前缀来强制执行这一点。这将其转换为常量而不是字段。

```c#
	const float hoursToDegrees = -30f;
```

> **`const` 值有什么特别之处？**
>
> `const` 关键字表示值永远不会改变，也不需要是字段。相反，它的值将在编译期间计算，并替换为常量的所有使用。这仅适用于数字等原始类型。

让我们使用 `DateTime` 的适当属性对其他两个分支进行相同的处理。一分钟和一秒都由负六度的旋转表示。

```c#
	const float hoursToDegrees = -30f, minutesToDegrees = -6f, secondsToDegrees = -6f;

	[SerializeField]
	Transform hoursPivot, minutesPivot, secondsPivot;

	void Awake () {
		hoursPivot.localRotation =
			Quaternion.Euler(0f, 0f, hoursToDegrees * DateTime.Now.Hour);
		minutesPivot.localRotation =
			Quaternion.Euler(0f, 0f, minutesToDegrees * DateTime.Now.Minute);
		secondsPivot.localRotation =
			Quaternion.Euler(0f, 0f, secondsToDegrees * DateTime.Now.Second);
	}
```

![img](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/animating-the-clock/time-5-16-31.png)

*目前为 5:16:31。*

我们正在使用 `DateTime.Now` 三次，检索小时、分钟和秒。每次我们再次遍历该属性时，都需要一些工作，这在理论上可能会导致不同的时间值。为了确保这种情况不会发生，我们应该只检索一次时间。我们可以通过在方法内声明一个变量并为其分配时间，然后在之后使用此值来实现这一点。让我们把它命名为 `time`。

> **什么是变量？**
>
> 变量的作用类似于字段，除了它只在方法执行时存在。它属于方法，而不是类。

```c#
	void Awake () {
		DateTime time = DateTime.Now;
		hoursPivot.localRotation =
			Quaternion.Euler(0f, 0f, hoursToDegrees * time.Hour);
		minutesPivot.localRotation =
			Quaternion.Euler(0f, 0f, minutesToDegrees * time.Minute);
		secondsPivot.localRotation =
			Quaternion.Euler(0f, 0f, secondsToDegrees * time.Second);
	}
```

对于变量，可以省略类型声明，用 `var` 关键字替换它。这可以缩短代码，但只有当变量的类型可以从声明时分配给它的内容中推断出来时，才有可能。此外，我更喜欢只在语句中明确提到类型时才这样做，这里就是这种情况。

```c#
		var time = DateTime.Now;
```

### 3.9 臂动画

我们在进入游戏模式时获得当前时间，但之后时钟保持静止。为了使时钟与当前时间保持同步，请将 `Awake` 方法的名称更改为 `Update`。这是另一种特殊的事件方法，只要我们处于播放模式，Unity 就会每帧调用一次，而不仅仅是一次。

```c#
	void Update () {
		var time = DateTime.Now;
		hoursPivot.localRotation =
			Quaternion.Euler(0f, 0f, hoursToDegrees * time.Hour);
		minutesPivot.localRotation =
			Quaternion.Euler(0f, 0f, minutesToDegrees * time.Minute);
		secondsPivot.localRotation =
			Quaternion.Euler(0f, 0f, secondsToDegrees * time.Second);
	}
```

*正在更新时钟。*

> **什么是相框？**
>
> 在播放模式下，Unity 会从主摄像头的角度连续渲染场景。渲染完成后，结果将显示在显示器上。然后，显示器将显示该帧，直到它显示下一帧。在渲染新帧之前，所有内容都会更新。因此，Unity 会经历一系列的更新、渲染、更新、渲染等过程。一个更新步骤，然后渲染一次场景，通常被认为是一帧，尽管实际上时间更复杂。

请注意，我们的 `Clock` 组件在检查器中的名称前有一个切换。这允许我们禁用它，从而阻止 Unity 调用其 `Update` 方法。

![img](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/animating-the-clock/enabled-toggle.png)

*可以禁用的 `Clock` 组件。*

### 3.10 连续旋转

我们时钟的指针精确地指示当前的小时、分钟或秒。它的行为就像一个数字时钟，离散但有臂。通常，时钟具有缓慢旋转的臂，提供时间的模拟表示。让我们改变我们的方法，让我们的时钟变成模拟的。

`DateTime` 不包含小数数据。幸运的是，它确实有 `TimeOfDay` 属性。这为我们提供了一个 `TimeSpan` 值，该值通过其 `TotalHours`、`TotalMinutes` 和 `TotalSeconds` 属性以我们需要的格式包含数据。

首先从 `DateTime.Now` 获取 `TimeOfDay` 结构体值，将其存储在变量中。由于此语句中没有提到 `TimeSpan` 类型，我将使变量的类型显式。然后调整我们用来旋转臂的属性。

```c#
	void Update () {
		TimeSpan time = DateTime.Now.TimeOfDay;
		hoursPivot.localRotation =
			Quaternion.Euler(0f, 0f, hoursToDegrees * time.TotalHours);
		minutesPivot.localRotation =
			Quaternion.Euler(0f, 0f, minutesToDegrees * time.TotalMinutes);
		secondsPivot.localRotation =
			Quaternion.Euler(0f, 0f, secondsToDegrees * time.TotalSeconds);
	}
```

这将导致编译器错误，抱怨我们无法从 `double` 转换为 `float`。这是因为 `TimeSpan` 属性生成具有双精度浮点类型（称为 `double`）的值。这些值提供了比浮点值更高的精度，但 Unity 的代码仅适用于单精度浮点值。

> **单精度是否足够？**
>
> 对于大多数游戏来说，是的。当处理非常大的距离或比例差异时，这会成为一个问题。然后，您必须应用传送或相机相对渲染等技巧，以保持活动区域靠近世界原点。虽然使用双精度可以解决这个问题，但它也会使所涉及数字的内存大小加倍，从而导致其他性能问题。游戏引擎通常使用单精度浮点值，GPU 也是如此。

我们可以通过显式地从 `double` 转换为 `float` 来解决这个问题。这个过程被称为类型转换（casting），是通过在要转换的值前面的圆括号内写入新类型来完成的。

```c#
		hoursPivot.localRotation =
			Quaternion.Euler(0f, 0f, hoursToDegrees * (float)time.TotalHours);
		minutesPivot.localRotation =
			Quaternion.Euler(0f, 0f, minutesToDegrees * (float)time.TotalMinutes);
		secondsPivot.localRotation =
			Quaternion.Euler(0f, 0f, secondsToDegrees * (float)time.TotalSeconds);
```

*模拟时钟。*

现在，您已经了解了在 Unity 中创建对象和编写代码的基本原理。下一个教程是[构建图](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/)。

[license](https://catlikecoding.com/unity/tutorials/license/)

[repository](https://bitbucket.org/catlikecodingunitytutorials/basics-01-game-objects-and-scripts/)

[PDF](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/Game-Objects-and-Scripts.pdf)



# 构建图：可视化数学

发布于 2017-11-13 更新于 2021-05-18

https://catlikecoding.com/unity/tutorials/basics/building-a-graph/

*创建预制件。*
*实例化多个立方体。*
*展示一个数学函数。*
*创建曲面着色器和着色器图。*
*为图形设置动画。*

这是关于学习使用 Unity [基础](https://catlikecoding.com/unity/tutorials/basics/)知识的系列教程中的第二篇。这次我们将使用游戏对象来构建一个图，这样我们就可以显示数学公式。我们还将使函数随时间变化，创建一个动画图。

本教程使用 Unity 2020.3.6f1 编写。

![img](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/tutorial-image.jpg)

*使用立方体显示正弦波。*

## 1 创建一行立方体

在编程时，对数学有很好的理解是必不可少的。在最基本的层面上，数学是对表示数字的符号的操纵。求解一个方程归结为重写一组符号，使其成为另一组通常更短的符号。数学规则规定了如何进行这种重写。

例如，我们有函数 `f(x)=x+1`。我们可以用一个数字代替它的 `x` 参数，比如 3。这导致 `f(3)=3+1=4`。我们提供了 3 作为输入参数，最终得到 4 作为输出。我们可以说函数映射 3 到 4。写这个的一种较短的方法是作为输入-输出对，如 (3, 4)。我们可以创建许多形式为 `(x, f(x))` 的对，例如 (5, 6) 和 (8, 9) 以及 (1, 2) 和 (6, 7)。但是，当我们按输入数字对配对进行排序时，更容易理解函数。(1, 2) 和 (2, 3) 和 (3, 4) 等等。

函数 `f(x)=x+1` 很容易理解 `f(x)=(x-1)^4+5x^3-8x^2+3x` 更硬。我们可以写下一些输入输出对，但这可能无法让我们很好地掌握它所代表的映射。我们需要很多分数，靠得很近。这最终将是一个难以解析的数字海洋。相反，我们可以将这些对解释为形式为 `[x]，[f(x)]]` 的二维坐标。这是一个二维向量，其中顶部数字表示 X 轴上的水平坐标，底部数字表示 Y 轴上的垂直坐标。换句话说，`y=f(x)`。我们可以在曲面上绘制这些点。如果我们使用足够多的非常接近的点，我们最终会得到一条线。结果是一个图表。

![img](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/creating-a-line-of-cubes/graph.png)

*用 [Desmos](https://www.desmos.com/calculator/di84egsf7a) 绘制的 `x` 在 −2 和 2 之间的图形。*

查看图表可以快速让我们了解函数的行为。这是一个方便的工具，所以让我们在 Unity 中创建一个。我们将从一个新项目开始，如[前一教程](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/)的第一部分所述。

### 1.1 预制件

通过在适当的坐标处放置点来创建图形。为此，我们需要一个点的 3D 可视化。我们将简单地使用 Unity 的默认立方体游戏对象。在场景中添加一个，并将其命名为 *Point*。删除其 `BoxCollider` 组件，因为我们不会使用物理。

> **立方体是可视化图形的最佳方式吗？**
>
> 您也可以使用粒子系统或线段，但使用单个立方体最简单。

我们将使用自定义组件创建此多维数据集的许多实例并正确定位它们。为了做到这一点，我们将把立方体变成游戏对象模板。将立方体从层次结构窗口拖动到项目窗口中。这将创建一个新的资产，称为预制件。它是一个预先制作的游戏对象，存在于项目中，而不是场景中。

![one column](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/creating-a-line-of-cubes/prefab-project-one-column.png)
![two column](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/creating-a-line-of-cubes/prefab-project-two-column.png)

*点预制资产，单列和双列布局。*

我们用来创建预制件的游戏对象仍然存在于场景中，但现在是预制件实例。它在层次结构窗口中有一个蓝色图标，右侧有一个箭头。其检查器的标题还表明它是一个预制件，并显示了更多控件。位置和旋转现在以粗体文本显示，这表示实例的值覆盖了前言。您对实例所做的任何其他更改也将以这种方式显示。

![hierarchy](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/creating-a-line-of-cubes/prefab-instance-hierarchy.png)
![inspector](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/creating-a-line-of-cubes/prefab-instance-inspector.png)

*点预制实例。*

选择预制资产时，其检查器将显示其根游戏对象和一个打开预制资产的大按钮。

![img](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/creating-a-line-of-cubes/prefab-inspector.png)

*预制资产检视栏。*

单击“*打开预制*”按钮将使场景窗口显示一个只包含预制对象层次的场景。您还可以通过实例的“*打开*”按钮、层次结构窗口中实例旁边的向右箭头或双击项目窗口中的资源来到达那里。当预制件具有复杂的层次结构时，这很有用，但对于我们简单的点预制件来说情况并非如此。

![img](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/creating-a-line-of-cubes/prefab-hierarchy.png)

*我们预制件的层次结构窗口。*

您可以通过层次窗口中预制件名称左侧的箭头退出预制件的场景。

> **为什么预制场景的背景是均匀的深蓝色？**
>
> 如果打开作为场景一部分的预制实例，则场景窗口将根据窗口顶部显示的上下文设置显示其周围环境。如果打开预制资源，则没有上下文。对于资源，默认情况下，预制场景中的天空盒以及其他一些东西都是禁用的。您可以通过场景窗口的工具栏进行配置，就像您可以对常规场景窗口进行配置一样。天空框可以通过下拉菜单切换，下拉菜单看起来像一个顶部有一颗星星的堆栈。请注意，当您进入和退出预制资源模式时，场景工具栏设置是如何变化的。

预制件是配置游戏对象的一种方便的方法。如果更改预制资源，则任何场景中的所有预制资源实例都将以相同的方式更改。例如，更改预制件的比例也会更改场景中仍存在的立方体的比例。但是，每个实例都使用自己的位置和旋转。此外，可以修改游戏对象实例，从而覆盖预制件的值。请注意，在播放模式下，预制件和实例之间的关系被破坏。

我们将使用脚本创建预制件的实例，这意味着我们不再需要场景中当前的预制件实例。因此，通过“*编辑/删除*”、指定的键盘快捷键或层次结构窗口中的上下文菜单删除它。

### 1.2 图形组件

我们需要一个 C# 脚本来生成一个带有点预制体的图。创建一个并将其命名为 `Graph`。

![img](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/creating-a-line-of-cubes/graph-asset.png)

*在 Scripts 文件夹中绘制 C# 资源。*

我们从一个扩展 `MonoBehaviour` 的简单类开始，这样它就可以用作游戏对象的组件。给它一个可序列化的字段来保存对用于实例化点的预制件的引用，命名为 `pointPrefab`。我们需要访问 `Transform` 组件来定位点，因此将其设置为字段的类型。

```c#
using UnityEngine;

public class Graph : MonoBehaviour {

	[SerializeField]
	Transform pointPrefab;
}
```

将一个空游戏对象添加到场景中，并将其命名为 *Graph*。确保其位置和旋转为零，刻度为 1。将 `Graph` 组件添加到此对象中。然后将我们的预制资源拖动到图形的“*点预制体*”字段中。现在，它包含对预制件 `Transform` 组件的引用。

![img](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/creating-a-line-of-cubes/graph-with-prefab.png)

*带有预制体引用的图形游戏对象。*

### 1.3 实例化预制件

实例化游戏对象是通过 `Object.Instantiate` 方法完成的。这是 Unity `Object` 类型的公开方法，`Graph` 通过扩展 `MonoBehaviour` 间接继承了它。`Instantiate` 方法克隆作为参数传递给它的任何Unity对象。对于预制件，它将导致一个实例被添加到当前场景中。让我们在 `Graph` 组件唤醒时执行此操作。

```c#
public class Graph : MonoBehaviour {

	[SerializeField]
	Transform pointPrefab;
	
	void Awake () {
		Instantiate(pointPrefab);
	}
}
```

> **`MonoBaviour` 的完整继承链是什么？**
>
> `MonoBehaviour` 扩展了 `Behaviour`，`Behaviour` 又扩展了 `Component`， `Component` 又扩展了 `Object`。

如果我们现在进入播放模式，将在世界原点生成一个 *Point* 预制件的实例。它的名称与前言相同，后面附加了（*Clone*）。

![img](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/creating-a-line-of-cubes/instantiated-prefab.png)

*实例化预制件，在场景窗口中沿 Z 轴向下看。*

> **在播放模式下可以打开场景窗口吗？**
>
> 是的，但 Unity 在进入游戏模式时总是将游戏窗口强制显示在前台。如果游戏窗口与场景窗口共享一个面板，则该场景窗口将被隐藏。但您可以在仍处于播放模式时切换回场景窗口。此外，您可以配置编辑器布局，使一个或多个游戏和场景窗口同时可见。请记住，Unity 必须渲染所有这些窗口，因此打开的窗口越多，速度就越慢。

要将该点放置在其他位置，我们需要调整实例的位置。`Instantiate` 方法为我们提供了对它所创建内容的引用。因为我们给了它一个 `Transform` 组件的引用，这就是我们得到的回报。让我们用一个变量来跟踪它。

```c#
	void Awake () {
		Transform point = Instantiate(pointPrefab);
	}
```

在[前面的教程](https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/)中，我们通过将四元数指定给枢轴变换的 `localRotation` 属性来旋转时钟臂。更改位置的方式与此相同，只是我们必须为 `localPosition` 属性分配一个 3D 向量。

3D 向量是使用 `Vector3` 结构类型创建的。例如，让我们将点的 X 坐标设置为 1，使其 Y 和 Z 坐标为零。`Vector3` 具有 `right` 属性，为我们提供了这样一个向量。使用它来设置点的位置。

```c#
		Transform point = Instantiate(pointPrefab);
		point.localPosition = Vector3.right;
```

![img](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/creating-a-line-of-cubes/one-unit-to-the-right.png)

*将立方体放在右边一个单位。*

现在进入游戏模式时，我们仍然会得到一个立方体，只是位置略有不同。让我们实例化第二个，并将其放置在右侧的额外步骤中。这可以通过将右向量乘以 2 来实现。重复实例化和定位，然后将乘法添加到新代码中。

```c#
	void Awake () {
		Transform point = Instantiate(pointPrefab);
		point.localPosition = Vector3.right;

		Transform point = Instantiate(pointPrefab);
		point.localPosition = Vector3.right * 2f;
	}
```

> **我们能把结构和数字相乘吗？**
>
> 通常你不能，但可以定义这样的功能。这是通过创建一个具有特殊语法的方法来实现的，因此可以像乘法一样调用它。在这种情况下，看似简单的乘法实际上是一种方法调用，类似于 `Vector3.Multiply(Vector3.right, 2f)`。结果是一个等于右向量的向量，其所有分量都加倍。
>
> 能够将方法当作简单的操作来使用，使编写代码更快、更容易阅读。它不是必需的，但很好，就像能够隐式使用名称空间一样。这种方便的语法被称为句法糖。
>
> 话虽如此，只有当方法严格符合运算符的原始含义时，才应将其用作运算符。在向量的情况下，一些数学运算符是定义良好的，因此对这些运算符来说是可以的。

这段代码将产生编译器错误，因为我们试图定义 `point` 变量两次。如果我们想使用另一个变量，我们必须给它一个不同的名称。或者，我们重用已有的变量。一旦我们完成了第一个点的引用，我们就不需要保留它，所以将新点分配给同一个变量。

```c#
		Transform point = Instantiate(pointPrefab);
		point.localPosition = Vector3.right;

//		Transform point = Instantiate(pointPrefab);
		point = Instantiate(pointPrefab);
		point.localPosition = Vector3.right * 2f;
```

![img](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/creating-a-line-of-cubes/two-instances.png)

*两个实例，X 坐标为 1 和 2。*

### 1.4 代码循环

让我们创造更多的点，直到我们有十个。我们可以重复同样的代码八次，但这将是非常低效的编程。理想情况下，我们只编写一点代码，并指示程序多次执行，略有变化。

`while` 语句可用于使代码块重复。将其应用于我们方法的前两个语句，并删除其他语句。

```c#
	void Awake () {
		while {
			Transform point = Instantiate(pointPrefab);
			point.localPosition = Vector3.right;
		}
//		point = Instantiate(pointPrefab);
//		point.localPosition = Vector3.right * 2f;
	}
```

`while` 关键字后面必须有一个圆括号内的表达式。`while` 后面的代码块只有在表达式计算结果为 true 时才会执行。之后，程序将循环回到 `while` 语句。如果此时表达式再次计算为 true，则代码块将再次执行。重复此操作，直到表达式的计算结果为 false。然后，程序跳过 `while` 语句后面的代码块，并在其下方继续。

因此，我们必须在 `while` 之后添加一个表达式。我们必须小心确保循环不会永远重复。无限循环会导致程序卡住，需要用户手动终止。编译最安全的表达式就是 false。

```c#
		while (false) {
			Transform point = Instantiate(pointPrefab);
			point.localPosition = Vector3.right;
		}
```

> **我们能定义循环中的 `point` 吗？**
>
> 对。虽然代码会重复，但我们只定义了一次变量。它在循环的每次迭代中都会被重用，就像我们之前手动做的那样。
>
> 您还可以在循环之前定义 `point`。这也允许您在循环外使用变量。否则，它的作用域仅限于 `while` 循环的块。

通过记录我们重复代码的次数，可以限制循环。我们可以使用整数变量来跟踪这一点。它的类型是 `int`。它将包含循环的迭代次数，所以我们把它命名为 `i`。它的初始值是零。为了能够在 `while` 表达式中使用它，必须在它上面定义它。

```c#
		int i = 0;
		while (false) {
			Transform point = Instantiate(pointPrefab);
			point.localPosition = Vector3.right;
		}
```

每次迭代，通过将其设置为自身加 1，将数字增加 1。

```c#
		int i = 0;
		while (false) {
			i = i + 1;
			Transform point = Instantiate(pointPrefab);
			point.localPosition = Vector3.right;
		}
```

现在，`i` 在第一次迭代开始时变为 1，在第二次迭代开始时变为 2，以此类推。但是 `while` 表达式在每次迭代之前都会被求值。所以在第一次迭代之前，`i` 为零，在第二次迭代之前为 1，以此类推。所以在第十次迭代之后，`i` 为 10。此时，我们想停止循环，所以它的表达式应该计算为 false。换句话说，只要 `i` 不到十，我们就应该继续。从数学上讲，这表示为 `i < 10`。它在代码中也是这样写的，使用了 < less than 运算符。

```c#
		int i = 0;
		while (i < 10) {
			i = i + 1;
			Transform point = Instantiate(pointPrefab);
			point.localPosition = Vector3.right;
		}
```

现在，进入游戏模式后，我们将获得十个立方体。但他们最终都处于同一位置。要将它们沿X轴排成一行，请将 `right` 向量乘以 `i`。

```c#
			point.localPosition = Vector3.right * i;
```

![img](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/creating-a-line-of-cubes/ten-cubes.png)

*沿 X 轴排列十个立方体。*

请注意，目前第一个立方体的 X 坐标为 1，最后一个立方体的坐标为 10。让我们改变这一点，从零开始，将第一个立方体定位在原点。我们可以通过 `right` 乘以 `(i-1)` 而不是 `i`，将所有点向左移动一个单位。但是，我们可以通过在块的末尾、乘法之后而不是开头增加 `i` 来跳过额外的减法。

```c#
		while (i < 10) {
//			i = i + 1;
			Transform point = Instantiate(pointPrefab);
			point.localPosition = Vector3.right * i;
			i = i + 1;
		}
```

### 1.5 简明语法

因为循环一定次数是很常见的，所以保持循环代码简洁是很方便的。一些句法糖可以帮助我们做到这一点。

首先，让我们考虑增加迭代次数。当执行形式为 `x = x * y`的操作时，可以将其缩短为 `x *= y`。这适用于作用于两个操作数的所有运算符。

```c#
//			i = i + 1;
			i += 1;
```

更进一步，当将一个数字递增或递减 1 时，可以将其缩短为 `++x` 或 `--x`。

```c#
//			i += 1;
			++i;
```

赋值语句的一个特性是它们也可以用作表达式。这意味着你可以写类似于 `y = (x += 3)`的东西。这将使 `x` 增加3，并将其结果也分配给 `y`。这表明我们可以在 `while` 表达式中递增 `i`，从而缩短代码块。

```c#
		while (++i < 10) {
			Transform point = Instantiate(pointPrefab);
			point.localPosition = Vector3.right * i;
//			++i;
		}
```

然而，现在我们在比较之前而不是之后递增 `i`，这将导致少一次迭代。具体来说，对于这种情况，递增和递减运算符也可以放在变量之后，而不是之前。该表达式的结果是更改之前的原始值。

```c#
//		while (++i < 10) {
		while (i++ < 10) {
			Transform point = Instantiate(pointPrefab);
			point.localPosition = Vector3.right * i;
		}
```

虽然 `while` 语句适用于所有类型的循环，但还有一种特别适合在范围内迭代的替代语法。这是 `for` 循环。它的工作方式类似于 `while`，只是迭代器变量声明及其比较都包含在圆括号内，用分号分隔。

```c#
//		int i = 0;
//		while (i++ < 10) {
		for (int i = 0; i++ < 10) {
			Transform point = Instantiate(pointPrefab);
			point.localPosition = Vector3.right * i;
		}
```

这将产生编译器错误，因为在另一个分号之后还有第三部分用于递增迭代器，使其与比较分开。这部分在每次迭代结束时执行。

```c#
//		for (int i = 0; i++ < 10) {
		for (int i = 0; i < 10; i++) {
			Transform point = Instantiate(pointPrefab);
			point.localPosition = Vector3.right * i;
		}
```

> **为什么在 `for` 循环中使用 `i++` 而不是 `++i`？**
>
> 由于增量表达式不用于其他任何用途，因此我们使用哪个版本并不重要。我们也可以使用 `i += 1` 或 `i = i + 1`。
>
> 经典的 for 循环具有 `for (int i = 0; i < someLimit; i++)` 的形式。您将在许多程序和脚本中遇到该代码片段。

### 1.6 更改域名

目前，我们的点被赋予 X 坐标 0 到 9。使用函数时，这不是一个方便的范围。通常，X 的取值范围为 0 ~ 1。或者，当处理以零为中心的函数时，取值范围为 -1 ~ 1。让我们相应地重新定位我们的观点。

沿两个单位长的线段放置十个立方体会导致它们重叠。为了防止这种情况，我们将减少他们的规模。默认情况下，每个立方体在每个维度上的大小为 1，因此为了使它们适合，我们必须将它们的比例缩小到 `2/10 = 1/5`。我们可以通过将每个点的局部比例设置为 `Vector3.one` 属性除以 5 来实现这一点。除法是用 `/` slash 运算符完成的。

```c#
		for (int i = 0; i < 10; i++) {
			Transform point = Instantiate(pointPrefab);
			point.localPosition = Vector3.right * i;
			point.localScale = Vector3.one / 5f;
		}
```

通过将场景窗口切换到正交投影（忽略透视），可以更好地查看立方体的相对位置。单击场景窗口右上角的轴小部件下的标签，可在正交模式和透视模式之间切换。如果通过场景窗口工具栏关闭天空盒，白色立方体也更容易看到。

![img](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/creating-a-line-of-cubes/small-cubes.png)

*在没有天空盒的正交场景窗口中看到的小立方体。*

要将立方体重新组合在一起，也要将它们的位置除以 5。

```c#
			point.localPosition = Vector3.right * i / 5f;
```

这使得它们覆盖了 0~2 的范围。要将其转换为 -1~1 范围，请在缩放向量之前减去 1。使用圆括号表示数学表达式的运算顺序。

```c#
			point.localPosition = Vector3.right * (i / 5f - 1f);
```

![img](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/creating-a-line-of-cubes/from-1-to-08.png)

*从 -1 到 0.8。*

现在第一个立方体的 X 坐标为 -1，而最后一个立方体的 X 坐标为 0.8。但是，立方体大小为 0.2。当立方体以其位置为中心时，第一个立方体的左侧为 -1.1，而最后一个立方体的右侧为 0.9。为了用我们的立方体整齐地填充 -1~1 范围，我们必须将它们向右移动半个立方体。这可以通过在除以 `i` 之前将其加 0.5 来实现。

```c#
			point.localPosition = Vector3.right * ((i + 0.5f) / 5f - 1f);
```

![img](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/creating-a-line-of-cubes/from-1-to-1.png)

*填充 -1~1 范围。*

### 1.7 将矢量从循环中提升出来

尽管所有立方体具有相同的比例，但我们在循环的每次迭代中都会再次计算它。我们不必这样做，规模是不变的。相反，我们可以在循环之前计算一次，将其存储在一个 `scale` 变量中，并在循环中使用。

```c#
	void Awake () {
		var scale = Vector3.one / 5f;
		for (int i = 0; i < 10; i++) {
			Transform point = Instantiate(pointPrefab);
			point.localPosition = Vector3.right * ((i + 0.5f) / 5f - 1f);
			point.localScale = scale;
		}
	}
```

我们还可以在循环之前为位置定义一个变量。当我们沿着 X 轴创建一条线时，我们只需要调整循环内位置的 X 坐标。所以我们不再需要乘以 `Vector3.right`。

```c#
		Vector3 position;
		var scale = Vector3.one / 5f;
		for (int i = 0; i < 10; i++) {
			Transform point = Instantiate(pointPrefab);
			//point.localPosition = Vector3.right * ((i + 0.5f) / 5f - 1f);
			position.x = (i + 0.5f) / 5f - 1f;
			point.localPosition = position;
			point.localScale = scale;
		}
```

> **我们可以单独更改向量的分量吗？**
>
> `Vector3` 结构体有三个浮点字段：x、y 和 z。这些字段是公共的，所以我们可以更改它们。
>
> 因为结构的行为就像简单的值，所以它们应该是不可变的。一旦建成，它们就不应该改变。如果你想使用不同的值，给字段或变量分配一个新的结构，就像我们处理数字一样。如果我们说“x=3”，然后说“x=5”，我们给“x”分配了一个不同的数字。我们没有将数字 3 本身修改为 5。然而，Unity 的向量类型是可变的。这既是为了方便，也是为了性能，因为单个矢量分量通常是独立操纵的。
>
> 为了了解如何使用可变向量，您可以考虑使用 `Vector3` 来方便地替代使用三个单独的 `float` 值。您可以独立访问它们，也可以将它们复制并分配为一个组。

这将导致编译器错误，抱怨使用了未赋值的变量。这是因为我们正在为某物分配 `position`，而我们还没有设置它的 Y 和 Z 坐标。我们可以通过将 `position` 初始设置为零向量，并为其分配 `Vector3.zero` 来解决这个问题。

```c#
		//Vector3 position;
		var position = Vector3.zero;
		var scale = Vector3.one / 5f;
```

### 1.8 用 X 定义 Y

这个想法是，我们的立方体的位置被定义为 `[[x],[f(x)],[0]]`，所以我们可以用它们来显示一个函数。此时，Y坐标始终为零，这表示平凡函数 `f(x) = 0`。为了展示一个不同的函数，我们必须确定循环内的 Y 坐标，而不是循环前的 Y 坐标。让我们从使 Y 等于 X 开始，表示函数 `f(x)=x`。

```c#
		for (int i = 0; i < 10; i++) {
			Transform point = Instantiate(pointPrefab);
			position.x = (i + 0.5f) / 5f - 1f;
			position.y = position.x;
			point.localPosition = position;
			point.localScale = scale;
		}
```

![img](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/creating-a-line-of-cubes/y-equals-x.png)

*Y 等于 X。*

一个稍微不那么明显的函数是 `f(x)=x^2`，它定义了一个最小值为零的抛物线。

```c#
			position.y = position.x * position.x;
```

![img](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/creating-a-line-of-cubes/y-equals-x-squared.png)

*Y 等于 X 的平方。*

## 2 创建更多立方体

虽然我们现在有一个函数图，但它很难看。因为我们只使用了十个立方体，所以建议的线条看起来非常块状和离散。如果我们使用更多更小的立方体，效果会更好。

### 2.1 可变分辨率

我们可以使其可配置，而不是使用固定数量的立方体。为了实现这一点，请在 `Graph` 中为分辨率添加一个可序列化的整数字段。默认值为 10，这就是我们现在使用的。

```c#
	[SerializeField]
	Transform pointPrefab;

	[SerializeField]
	int resolution = 10;
```

![img](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/creating-more-cubes/configurable-resolution.png)

*可配置分辨率。*

现在，我们可以通过检查器更改图形的分辨率来调整它。然而，并非所有整数都是有效的分辨率。至少他们必须是积极的。我们可以指示检查员为我们的决议设定一个范围。这是通过将 `Range` 属性附加到它来实现的。我们可以将 `resolution` 的两个属性放在它们自己的方括号之间，也可以将它们组合在一个逗号分隔的属性列表中。让我们做后者。

```c#
	[SerializeField, Range]
	int resolution = 10;
```

检查器检查字段是否附加了 `Range` 属性。如果是这样，它将约束该值并显示滑块。然而，要做到这一点，它需要知道允许的范围。因此，`Range` 需要两个参数——就像一个方法一样——用于最小值和最大值。让我们用 10 和 100。

```c#
	[SerializeField, Range(10, 100)]
	int resolution = 10;
```

![img](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/creating-more-cubes/resolution-slider.png)

*分辨率滑块设置为 50。*

> **这能保证 `resolution` 限制在 10 到 100 之间吗？**
>
> `Range` 属性所做的只是指示检查器使用具有该范围的滑块。它不会以任何其他方式影响 `resolution`。因此，我们可以编写代码为其分配一个超出范围的值，但我们不会这样做。

### 2.2 变量实例化

为了使用配置的分辨率，我们必须更改实例化的立方体数量。在 `Awake` 中，迭代次数现在受到 `resolution` 的限制，而不是总是 `10` 次，而不是固定的循环次数。因此，如果分辨率设置为 50，我们将在进入播放模式后获得 50 个立方体。

```c#
		for (int i = 0; i < resolution; i++) {
			…
		}
```

> **`...` 什么意思？**
>
> 这表明我省略了一些没有改变的代码。

我们还必须调整立方体的比例和位置，使其保持在 -1~1 域内。现在，我们每次迭代必须完成的每个步骤的大小是 2 除以分辨率。将此值存储在变量中，并使用它来计算立方体的比例及其 X 坐标。

```c#
		float step = 2f / resolution;
		var position = Vector3.zero;
		var scale = Vector3.one * step;
		for (int i = 0; i < resolution; i++) {
			Transform point = Instantiate(pointPrefab);
			position.x = (i + 0.5f) * step - 1f;
			…
		}
```

![img](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/creating-more-cubes/resolution-50.png)

*使用分辨率 50。*

### 2.3 设置父母

在进入分辨率为 50 的播放模式后，许多实例化的立方体出现在场景中，因此也出现在项目窗口中。

![img](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/creating-more-cubes/many-root-objects.png)

*点是根对象。*

这些点目前是根对象，但它们作为图对象的子对象是有意义的。我们可以在实例化一个点后，通过调用其 `Transform` 组件的 `SetParent` 方法，将所需的父 `Transform` 传递给它，来建立这种关系。我们可以通过 `Graph` 继承自 `Component` 的 `transform` 属性来获取图对象的 `Transform` 组件。在循环块的末尾执行此操作。

```c#
		for (int i = 0; i < resolution; i++) {
			…
			point.SetParent(transform);
		}
```

![img](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/creating-more-cubes/child-objects.png)

*点是图的子项。*

当设置新的父对象时，Unity 将尝试将对象保持在其原始世界位置、旋转和缩放。在我们的情况下，我们不需要这个。我们可以通过将 `false` 作为第二个参数传递给 `SetParent` 来表示这一点。

```c#
			point.SetParent(transform, false);
```

## 3 为图形着色

白色的图表看起来并不美观。我们可以使用另一种纯色，但这也不是很有趣。使用点的位置来确定其颜色更有趣。

调整每个立方体颜色的一种简单方法是设置其材质的颜色属性。我们可以在循环中做到这一点。由于每个立方体将获得不同的颜色，这意味着我们最终将为每个对象提供一个唯一的材质实例。当我们稍后为图形设置动画时，我们也必须一直调整这些材质。虽然这有效，但效率不高。如果我们能使用一种直接使用位置作为颜色的单一材料，那就更好了。不幸的是，Unity 没有这样的材料。所以，让我们做自己的。

### 3.1 创建曲面着色器

GPU 运行着色器程序来渲染 3D 对象。Unity 的材质资源决定了使用哪个着色器，并允许配置其属性。我们需要创建一个自定义着色器来获得我们想要的功能。通过*“资源”/“创建”/“着色器”/“标准曲面着色器”*创建一个，并将其命名为*“点曲面”*。

![one column](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/coloring-the-graph/point-surface-one-column.png)
![two column](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/coloring-the-graph/point-surface-two-column.png)

*在 Point 文件夹中，着色器与预制件分组，采用单列和双列布局。*

我们现在有一个着色器资源，您可以像脚本一样打开它。我们的着色器文件包含定义曲面着色器的代码，该着色器使用与 C# 不同的语法。它包含一个表面着色器模板，但我们将删除所有内容，从头开始创建一个最小的着色器。

> **曲面着色器是如何工作的？**
>
> Unity 提供了一个框架，可以快速生成执行默认照明计算的着色器，您可以通过调整某些值来影响这些着色器。这种着色器被称为曲面着色器。遗憾的是，它们仅适用于默认的渲染管道。稍后我们将介绍通用渲染管道。

Unity 有自己的着色器资源语法，总体上大致类似于 C#，但它是不同语言的混合体。它以 `Shader` 关键字开头，后跟一个定义着色器菜单项的字符串。字符串写在双引号内。我们将使用*图形/点曲面*。之后是着色器内容的代码块。

```glsl
Shader "Graph/Point Surface" {}
```

着色器可以有多个子着色器，每个子着色器由 `SubShader` 关键字和一个代码块定义。我们只需要一个。

```glsl
Shader "Graph/Point Surface" {

	SubShader {}
}
```

在子着色器下方，我们还想通过编写 `Fallback "diffuse"` 来向标准漫反射着色器添加回退。

```c#
Shader "Graph/Point Surface" {
	
	SubShader {}
	
	FallBack "Diffuse"
}
```

曲面着色器的子着色器需要一个用 CG 和 HLSL 这两种着色器语言混合编写的代码段。此代码必须用 `CGPROGRAM` 和 `ENDCG` 关键字括起来。

```glsl
	SubShader {
		CGPROGRAM
		ENDCG
	}
```

第一个需要的语句是编译器指令，称为 pragma。它被写成 `#pragma`，后面跟着一个指令。在这种情况下，我们需要 `#pragma surface ConfigureSurface Standard fullforwardshadows`，它指示着色器编译器生成一个具有标准照明和完全支持阴影的表面着色器。`ConfigureSurface` 是指用于配置着色器的方法，我们必须创建着色器。

```glsl
		CGPROGRAM
		#pragma surface ConfigureSurface Standard fullforwardshadows
		ENDCG
```

> **pragma 是什么意思？**
>
> pragma 这个词来自希腊语，指的是一个行动或需要做的事情。它在许多编程语言中用于发出特殊的编译器指令。

我们遵循 `#pragma target 3.0` 指令，该指令为着色器的目标级别和质量设置了最小值。

```glsl
		CGPROGRAM
		#pragma surface ConfigureSurface Standard fullforwardshadows
		#pragma target 3.0
		ENDCG
```

我们将根据他们的世界地位来为我们的观点着色。为了在曲面着色器中实现这一点，我们必须为配置函数定义输入结构。它必须写成 `struct Input`，后跟一个代码块，然后是分号。在块内，我们声明一个 struct 字段，特别是 `float3 worldPos`。它将包含渲染内容的世界位置。`float3` 类型是与 `Vector3` 结构体等效的着色器。

```glsl
		CGPROGRAM
		#pragma surface ConfigureSurface Standard fullforwardshadows
		#pragma target 3.0

		struct Input {
			float3 worldPos;
		};
		ENDCG
```

> **这是否意味着移动图形会影响其颜色？**
>
> 对。使用这种方法，只有当我们将 *Graph* 对象留在原地时，着色才是正确的：在世界原点，没有旋转，比例为 1。
>
> 还要注意，此位置是按顶点确定的。在我们的例子中，这是立方体的每个角。颜色将在立方体的各个面上进行插值。立方体越大，这种颜色过渡就越明显。

下面我们定义了 `ConfigureSurface` 方法，尽管在着色器的情况下，它总是被称为函数，而不是方法。它是一个有两个参数的 `void` 函数。第一个是具有我们刚才定义的 `Input` 类型的输入参数。第二个参数是曲面配置数据，类型为 `SurfaceOutputStandard`。

```glsl
		struct Input {
			float3 worldPos;
		};

		void ConfigureSurface (Input input, SurfaceOutputStandard surface) {}
```

第二个参数必须在其类型前面写入 `inout` 关键字，这表示它既传递给函数又用于函数的结果。

```glsl
		void ConfigureSurface (Input input, inout SurfaceOutputStandard surface) {}
```

现在我们有了一个功能正常的着色器，为它创建一个名为“*点曲面*”的材质。通过其检查器标题中的“*着色器*”下拉列表选择“*图形/点曲面*”，将其设置为使用我们的着色器。

![img](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/coloring-the-graph/point-surface-material.png)

*点表面材质。*

材质目前为纯哑光黑色。通过在我们的配置函数中将 `surface.Smoothness` 设置为 0.5，我们可以使其看起来更像默认材质。。编写着色器代码时，我们不必在 `float` 值后添加 f 后缀。

```glsl
		void ConfigureSurface (Input input, inout SurfaceOutputStandard surface) {
			surface.Smoothness = 0.5;
		}
```

现在材质不再是完美的哑光。您可以在检查器标题的小材质预览中看到这一点，也可以在其底部的可调整大小的预览中看到。

![img](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/coloring-the-graph/material-preview.png)

*具有平均平滑度的材质预览。*

我们还可以配置平滑度，就像为它添加一个字段并在函数中使用它一样。默认样式是在着色器配置选项前加下划线，并将下一个字母大写，因此我们将使用 `_Smoothness`。

```glsl
		float _Smoothness;

		void ConfigureSurface (Input input, inout SurfaceOutputStandard surface) {
			surface.Smoothness = _Smoothness;
		}
```

要使此配置选项显示在编辑器中，我们必须在着色器顶部的子着色器上方添加一个 `Properties` 块。在其中写入 `_Smoothness`，后跟 `("Smoothness", Range(0,1)) = 0.5`。这为其提供了“*平滑度*”标签，将其显示为 0~1 范围的滑块，并将其默认值设置为 0.5。

```glsl
Shader "Graph/Point Surface" {

	Properties {
		_Smoothness ("Smoothness", Range(0,1)) = 0.5
	}
	
	SubShader {
		…
	}
}
```

![img](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/coloring-the-graph/configurable-smoothness.png)

*可配置的平滑度。*

使我们的*立方体*预制资产使用此材质而不是默认材质。这将使问题变得更加严重。

![img](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/coloring-the-graph/black-points.png)

*黑点。*

### 3.2 基于世界位置的着色

为了调整点的颜色，我们必须修改 `surface.Albedo`。由于反照率和世界位置都有三个组成部分，我们可以直接使用位置来计算反照率。

```glsl
		void ConfigureSurface (Input input, inout SurfaceOutputStandard surface) {
			surface.Albedo = input.worldPos;
			surface.Smoothness = _Smoothness;
		}
```

![img](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/coloring-the-graph/colored-points.png)

*彩色点。*

> **反照率是什么意思？**
>
> Albedo 在拉丁语中意为白色。这是衡量表面漫反射的光线量。如果反照率不是完全白色的，那么部分光能会被吸收而不是反射。

现在，世界 X 位置控制点的红色分量，Y 位置控制绿色分量，Z 控制蓝色分量。但是我们的图的 X 域是 -1~1，负颜色分量没有意义。因此，我们必须将位置减半，然后加 ½，使颜色适合域。我们可以同时对所有三维空间进行此操作。

```glsl
			surface.Albedo = input.worldPos * 0.5 + 0.5;
```

为了更好地了解颜色是否正确，让我们更改 `Graph.Awake` 后，我们显示函数 `f(x)=x^3`，它使 Y 也从 -1 变为 1。

```glsl
			position.y = position.x * position.x * position.x;
```

![img](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/coloring-the-graph/x-cubed.png)

*X 立方，略带蓝色。*

结果是蓝色的，因为所有立方体面的 Z 坐标都接近零，这将它们的蓝色分量设置为接近 0.5。在设置反照率时，我们可以通过只包括红色和绿色通道来消除蓝色。这可以在着色器中通过仅指定 `surface.Albedo.rg` 和仅使用 `input.worldPos.xy` 来完成。这样蓝色分量就保持为零。

```glsl
		surface.Albedo.rg = input.worldPos.xy * 0.5 + 0.5;
```

由于红色加绿色会导致黄色，这将使点从左下角的黑色附近开始，当 Y 最初比 X 增加得更快时变为绿色，当 X 赶上时变为黄色，当 X 增加得更快时变为轻微的橙色，最后在右上角的亮黄色附近结束。

![img](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/coloring-the-graph/x-cubed-green-yellow.png)

X 立方，从绿色到黄色。

### 3.3 通用渲染管道

除了默认的渲染管道外，Unity 还具有通用和高清晰度渲染管道，简称 URP 和 HDRP。两个渲染管道都有不同的功能和限制。当前的默认渲染管道仍然可用，但其功能集已冻结。几年后，URP 可能会成为默认值。所以，让我们的图也适用于 URP。

如果您还没有使用 URP，请转到包管理器并安装针对 Unity 版本验证的最新通用 RP 包。就我而言，这是 10.4.0。

![img](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/coloring-the-graph/urp-installed.png)

*URP 包已安装。*

> **我在哪里可以在包管理器中找到 URP？**
>
> 确保您已将包筛选器设置为 *Unity 注册*，而不是*项目中*。然后搜索通用或向下滚动列表，直到找到它。

这不会自动使 Unity 使用 URP。我们首先必须通过*资产/创建/渲染/通用渲染管道/管道资产（正向渲染器）*为其创建资产。我把它命名为 URP。这也将自动为渲染器创建另一个资源，在我的例子中名为 *URP_renderer*。

![one column](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/coloring-the-graph/urp-assets-one-column.png)
![two column](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/coloring-the-graph/urp-assets-two-column.png)

*URP 资产在单独的文件夹中，一列和两列布局。*

接下来，转到项目设置的“图形”部分，并将 URP 资源指定给“*可脚本渲染器管道设置*”字段。

![img](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/coloring-the-graph/urp-used.png)

*使用 URP。*

要稍后切换回默认渲染管道，只需将“*可脚本渲染器管道设置*”设置为“*无*”。这只能在编辑器中完成，渲染管道不能在构建的独立应用程序中更改。

> **HDRP 怎么样？**
> HDRP 是一个复杂得多的渲染管道。我不会在教程中介绍它。

### 3.4 创建着色器图

我们当前的材质仅适用于默认渲染管道，不适用于 URP。因此，当使用 URP 时，它会被 Unity 的错误材料所取代，即纯品红色。

![img](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/coloring-the-graph/error-material.png)

*立方体已经变成了洋红色。*

我们必须为 URP 创建一个单独的着色器。我们可以自己编写一个，但目前很难，升级到较新的 URP 版本时可能会中断。最好的方法是使用 Unity 的着色器图包来直观地设计着色器。URP 依赖于此软件包，因此它与 URP 软件包一起自动安装。

通过*“资源”/“创建”/“着色器”/“通用渲染管道”/“亮着色器图”*创建新的着色器图，并将其命名为“*点 URP*”。

![one column](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/coloring-the-graph/point-urp-one-column.png)
![two column](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/coloring-the-graph/point-urp-two-column.png)

*点 URP 着色器图形资源，一列和两列布局。*

通过在项目窗口中双击其资源或按其检查器中的“*打开着色器编辑器*”按钮，可以打开该图。这将为其打开一个着色器图窗口，该窗口可能会被多个节点和面板弄乱。这些是黑板、图形检查器和主预览面板，它们可以调整大小，也可以通过工具栏按钮隐藏。还有两个链接节点：*顶点*节点和*片段*节点。这两个用于配置着色器图的输出。

![img](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/coloring-the-graph/default-lit-shader-graph.png)

*默认照明着色器图，所有内容都可见。*

着色器图由表示数据或操作的节点组成。目前，*“片段”*节点的*“平滑度”*值设置为 0.5。要使其成为可配置的着色器属性，请按“*点 URP*”背板面板上的加号按钮，选择“Float”，并将新条目命名为“*平滑度*”。这将在黑板上添加一个圆形按钮来表示属性。选择它并将图形检查器切换到其“*节点设置*”选项卡，以查看此属性的配置。

![img](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/coloring-the-graph/smoothness-property-default.png)

*具有默认设置的平滑度属性。*

*引用*是内部已知属性的名称。这对应于我们在曲面着色器代码中如何命名属性字段 *_Smoothness*，所以让我们在这里也使用相同的内部名称。然后将其下方的默认值设置为0.5。确保其“*暴露*”切换选项已启用，因为这将控制材质是否为其获取着色器属性。最后，要使其显示为滑块，请将其“*模式*”更改为“*滑块*”。

![img](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/coloring-the-graph/smoothness-property-configured.png)

*已配置平滑度属性。*

接下来，将圆形的“*平滑度*”按钮从黑板拖动到图形中的空白处。这将为图形添加一个平滑度节点。通过从一个点拖动到另一个点，将其连接到 *PRB 主*节点的*平滑度*输入。这在它们之间建立了联系。

![img](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/coloring-the-graph/smoothness-connected.png)

*平滑连接。*

现在，您可以通过“*保存资源*”工具栏按钮保存图形，并创建一个名为 *Point URP* 的材质来使用它。着色器的菜单项是“*着色器图形/点 URP*”。然后使*点*预制件使用该材质而不是*点曲面*。

![img](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/coloring-the-graph/point-urp-material.png)

*使用我们的着色器图为 URP 材质。*

### 3.5 使用节点编程

要给点着色，我们必须从位置节点开始。通过在图形的空白部分打开上下文菜单并从中选择“*新建节点*”来创建一个。选择“*输入/几何图形/位置*”或只搜索“*位置*”。

![img](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/coloring-the-graph/world-position-node.png)

*世界位置节点。*

我们现在有一个位置节点，默认设置为世界空间。通过将光标悬停在其上时出现的向上箭头，可以折叠其预览可视化效果。

使用相同的方法创建*乘法*和*加法*节点。使用这些将位置的 XY 分量缩放 0.5，然后添加 0.5，同时将 Z 设置为零。这些节点根据其连接的内容调整其输入类型。因此，首先连接节点，然后填写其常量输入。然后将结果连接到 *Fragment* 的*基色*输入。

![img](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/coloring-the-graph/colored-shader-graph.png)

*彩色着色器图。*

如果将鼠标悬停在“*乘法*”和“*加法*”节点上，则可以通过按其右上角出现的箭头来压缩节点的视觉大小。这隐藏了所有未连接到其他节点的输入和输出。这消除了很多杂乱。您还可以通过“*顶点*”和“*片段*”节点的上下文菜单删除它们的组件。通过这种方式，您可以隐藏所有保持默认值的内容。

![img](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/coloring-the-graph/compacted-shader-graph.png)

*压缩着色器图。*

保存着色器资源后，我们现在在播放模式下获得与使用默认渲染管道时相同的色点。除此之外，在播放模式下，调试更新程序出现在单独的 *DontDestroyOnLoad* 场景中。这是用于调试 URP 的，可以忽略。

![img](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/coloring-the-graph/urp-debug-updater.png)

*URP 调试更新程序处于播放模式。*

从这一点开始，您可以使用默认渲染管道或 URP。从一个切换到另一个后，您还必须更改 *Point* 预制件的材质，否则它将是洋红色的。如果你对从图中生成的着色器代码很好奇，你可以通过图检查器的“*视图生成的着色器*”按钮来访问它。

## 4 为图形设置动画

显示静态图很有用，但移动图更有趣。因此，让我们添加对动画功能的支持。这是通过将时间作为一个额外的函数参数来实现的，使用形式为 `f(x, t)` 的函数，而不仅仅是 `f(x)`，其中 `t` 是时间。

### 4.1 跟踪积分

为了使图形动画化，我们必须随着时间的推移调整它的点。我们可以通过删除所有点并在每次更新时创建新点来实现这一点，但这是一种低效的方法。最好继续使用相同的点，每次更新都调整它们的位置。为了实现这一点，我们将使用一个字段来保持对我们的点的引用。在 `Transform` 类型的 `Graph` 中添加一个 `points` 字段。

```c#
	[SerializeField, Range(10, 100)]
	int resolution = 10;
	
	Transform points;
```

这个字段允许我们引用一个点，但我们需要访问所有这些点。我们可以通过在字段的类型后面放置空方括号来将字段转换为数组。

```c#
	Transform[] points;
```

`points` 字段现在是对数组的引用，数组的元素类型为 `Transform`。数组是对象，而不是简单的值。我们必须显式地创建这样一个对象，并使我们的字段引用它。这是通过在数组类型后面写 `new` 来完成的，因此在我们的例子中是 `new Transform[]`。在 `Awake` 中，在循环之前创建数组，并将其分配给点。

```c#
		points = new Transform[];
		for (int i = 0; i < resolution; i++) {
			…
		}
```

创建数组时，我们必须指定其长度。这定义了它有多少个元素，创建后不能更改。构造数组时，长度写在方括号内。使其等于图形的分辨率。

```c#
		points = new Transform[resolution];
```

现在我们可以用指向我们的点的引用填充数组。访问数组元素是通过在数组引用后面的方括号中写入其索引来完成的。数组索引从第一个元素的零开始，就像我们循环的迭代计数器一样。因此，我们可以使用它来分配给适当的数组元素。

```c#
		points = new Transform[resolution];
		for (int i = 0; i < resolution; i++) {
			Transform point = Instantiate(pointPrefab);
			points[i] = point;
			…
		}
```

如果我们连续多次分配同一件事，我们可以将这些分配链接在一起，因为分配表达式的结果就是分配的结果，如前一教程所述。

```c#
			Transform point = points[i] = Instantiate(pointPrefab);
			//points[i] = point;
```

我们现在正在遍历我们的点数组。因为数组的长度与分辨率相同，所以我们也可以用它来约束我们的循环。为此，每个数组都有一个 `Length` 属性，所以让我们使用它。

```c#
		points = new Transform[resolution];
		for (int i = 0; i < points.Length; i++) {
			…
		}
```

### 4.2 更新积分

为了在每一帧调整图形，我们需要在 `Update` 方法中设置点的Y坐标。所以我们不再需要在 `Awake` 中计算它们。我们仍然可以在这里设置 X 坐标，因为我们不会更改它们。

```c#
		for (int i = 0; i < points.Length; i++) {
			Transform point = points[i] = Instantiate(pointPrefab);
			position.x = (i + 0.5f) * step - 1f;
//			position.y = position.x * position.x * position.x;
			…
		}
```

添加一个带有 `for` 循环的 `Update` 方法，就像 `Awake` 一样，但其块中还没有任何代码。

```c#
	void Awake () {
		…
	}
	
	void Update () {
		for (int i = 0; i < points.Length; i++) {}
	}
```

我们将通过获取对当前数组元素的引用并将其存储在变量中来开始循环的每次迭代。

```c#
		for (int i = 0; i < points.Length; i++) {
			Transform point = points[i];
		}
```

之后，我们检索点的局部位置并将其存储在变量中。

```c#
		for (int i = 0; i < points.Length; i++) {
			Transform point = points[i];
			Vector3 position = point.localPosition;
		}
```

现在，我们可以像之前一样，基于 X 设置位置的 Y 坐标。

```c#
		for (int i = 0; i < points.Length; i++) {
			Transform point = points[i];
			Vector3 position = point.localPosition;
			position.y = position.x * position.x * position.x;
		}
```

因为 position 是一个结构体，所以我们只调整了局部变量的值。为了把它应用到这一点上，我们必须重新确定它的位置。

```c#
		for (int i = 0; i < points.Length; i++) {
			Transform point = points[i];
			Vector3 position = point.localPosition;
			position.y = position.x * position.x * position.x;
			point.localPosition = position;
		}
```

> **我们不能直接设置 `point.localPosition.y` 吗？**
>
> 如果 `localPosition` 是一个公共字段，那么我们可以直接设置点位置的 Y 坐标。但是，`localPosition` 是一个属性。它将向量值的副本传递给我们，或者复制我们分配给它的值。因此，我们最终会调整一个局部向量值，这根本不会影响点的位置。由于我们没有先将其显式存储在变量中，因此该操作将毫无意义，并将产生编译器错误。

### 4.3 显示正弦波

从现在开始，在播放模式下，我们的图的点在每一帧都会被定位。我们还没有注意到这一点，因为它们总是处于相同的位置。我们必须将时间纳入函数中，才能使其发生变化。然而，简单地添加时间会导致函数上升并迅速消失在视线之外。为了防止这种情况发生，我们必须使用一个变化但保持在固定范围内的函数。正弦函数对此非常理想，因此我们将使用 `f(x)=sin(x)`。我们可以使用 `Mathf.Sin` 方法来计算它。

```c#
			position.y = Mathf.Sin(position.x);
```

![img](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/animating-the-graph/sin-x.png)

*X 的正弦，从 -1 到 1。*

> **什么是 `Mathf`？**
>
> 它是 `UnityEngine` 命名空间中的一个结构体，包含一组数学函数和常量。由于它使用浮点数，其类型名称被赋予了 f 后缀。

正弦波在 -1 和 1 之间振荡。它每 2π ——发音为两个 pie——单位重复一次，这意味着它的周期大约为 6.28。由于我们的图的 X 坐标在 -1 和 1 之间，我们目前看到的重复模式不到三分之一。要看到它的整体尺度 X 乘以 π，所以我们最终得到 `f(X)=sin(pix)`。我们可以使用 `Mathf.PI` 常数作为 π 的近似值。

```c#
			position.y = Mathf.Sin(Mathf.PI * position.x);
```

![img](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/animating-the-graph/sin-pix.png)

*πX 的正弦。*

> **什么是正弦波和 π？**
>
> 正弦是一个三角函数，作用于一个角度。在我们的例子中，最有用的例子是半径为 1 的圆，即单位圆。圆上的每个点都有一个与之相关的角度 `θ`，以及一个二维位置。定义这些位置坐标的一种方法是 `[[sin(theta)],[sin(theta+pi/2)]]`。这表示从圆的顶部开始，沿顺时针方向绕着它走。除了 `sin(theta+pi/2)` 之外，您还可以使用余弦，从而得到 `[[sin(theta)],[cos(theta)]]`。
>
> ![img](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/animating-the-graph/sin-cos-pix.png)
>
> *πX 的正弦和余弦。*
>
> 角度“θ”以弧度表示，对应于沿单位圆圆周行进的距离。在中途，行进距离等于 π，大约为 3.14。因此，整个圆周的长度为 2π。换句话说，π 是圆的周长和直径之间的比值。

要设置此函数的动画，请在计算正弦函数之前将当前游戏时间添加到 X。它是通过 `Time.time` 找到的。如果我们也按 π 缩放时间，则函数将每两秒重复一次。因此，使用 `f(x,t)=sin(pi(x+t))`，其中 `t` 是经过的游戏时间。这将随着时间的推移使正弦波前进，使其在负 X 方向上移动。

```c#
			position.y = Mathf.Sin(Mathf.PI * (position.x + Time.time));
```

*动画正弦波。*

因为 `Time.time` 的值对于循环的每次迭代都是相同的，所以我们可以将属性调用提升到循环之外。

```c#
		float time = Time.time;
		for (int i = 0; i < points.Length; i++) {
			Transform point = points[i];
			Vector3 position = point.localPosition;
			position.y = Mathf.Sin(Mathf.PI * (position.x + time));
			point.localPosition = position;
		}
```

### 4.4 夹紧颜色

正弦波的振幅为 1，这意味着我们的点达到的最低和最高位置分别为 -1 和 1。然而，由于这些点是具有特定大小的立方体，它们稍微超出了这个范围。因此，我们可以得到绿色成分为负或大于 1 的颜色。虽然这并不明显，但让我们正确地夹紧颜色，以确保它们保持在 0-1 的范围内。

我们可以通过将生成的颜色传递给 `saturate` 函数来为我们的表面着色器执行此操作。这是一个将所有组件夹紧到 0-1 的特殊函数。这是着色器中一种常见的操作，称为饱和度，因此得名。

```glsl
			surface.Albedo.rg = saturate(input.worldPos.xy * 0.5 + 0.5);
```

在着色器图中使用*饱和*节点也可以完成同样的操作。

![img](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/animating-the-graph/saturated-shader-graph.png)

*着色器图中的饱和颜色。*

下一个教程是[数学曲面](https://catlikecoding.com/unity/tutorials/basics/mathematical-surfaces/)。

[license](https://catlikecoding.com/unity/tutorials/license/)

[repository](https://bitbucket.org/catlikecodingunitytutorials/basics-02-building-a-graph/)

[PDF](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/Building-a-Graph.pdf)



# 数学曲面：用数字雕刻

发布于 2018-01-10 更新于 2021-05-18

https://catlikecoding.com/unity/tutorials/basics/mathematical-surfaces/

*创建函数库。*
*使用委托和枚举类型。*
*用网格显示二维函数。*
*在三维空间中定义曲面。*

这是关于学习使用 Unity [基础](https://catlikecoding.com/unity/tutorials/basics/)知识的系列教程中的第三个。这是[构建图形](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/)教程的延续，所以我们不会启动新项目。这一次，我们将能够显示多个更复杂的功能。

本教程使用 Unity 2020.3.6f1 编写。

![img](https://catlikecoding.com/unity/tutorials/basics/mathematical-surfaces/tutorial-image.jpg)

*组合一些波以创建复杂的曲面。*

## 1 函数库

完成[上一教程](https://catlikecoding.com/unity/tutorials/basics/building-a-graph/)后，我们有一个点图，显示了在播放模式下的动画正弦波。也可以显示其他数学函数。你可以更改代码，函数也会随之更改。你甚至可以在 Unity 编辑器处于播放模式时这样做。执行将暂停，保存当前游戏状态，然后再次编译脚本，最后重新加载游戏状态并恢复游戏。这被称为热重新加载。并非所有东西都能经受住热重载，但我们的图表却能。它将切换到为新功能设置动画，而不会意识到有什么变化。

虽然在播放模式下更改代码很方便，但在多个功能之间来回切换并不是一种方便的方法。如果我们可以通过图的配置选项更改函数，那会好得多。

### 1.1 库类

我们可以在 `Graph` 中声明多个数学函数，但让我们把这个类专门用于显示一个函数，让它不知道确切的数学方程。这是专业化和关注点分离的一个例子。

创建一个新的 `FunctionLibrary` C# 脚本，并将其放在 `Graph` 旁边的 *Scripts* 文件夹中。您可以使用菜单选项创建新资源，也可以复制并重命名图形。在任何一种情况下，清除文件内容，并从使用 `UnityEngine` 开始，声明一个不扩展任何内容的空 `FunctionLibrary` 类。

```c#
using UnityEngine;

public class FunctionLibrary {}
```

此类不会是组件类型。我们也不会创建它的对象实例。相反，我们将使用它来提供一组可公开访问的方法，这些方法表示数学函数，类似于 Unity 的 `Mathf`。

为了表示这个类不会被用作对象模板，请在 `class` 之前写入 `static` 关键字，将其标记为静态。

```c#
public static class FunctionLibrary {}
```

### 1.2 函数方法

我们的第一个函数将是 `Graph` 当前显示的正弦波。我们需要为它创建一个方法。这与创建 `Awake` 或 `Update` 方法的工作原理相同，只是我们将其命名为 `Wave`。

```c#
public static class FunctionLibrary {

	void Wave () {}
}
```

默认情况下，方法是实例方法，这意味着必须在对象实例上调用它们。为了使它们直接在类级别工作，我们必须将其标记为静态，就像 `FunctionLibrary` 本身一样。

```c#
	static void Wave () {}
```

为了使其可公开访问，还需要为其添加 `public` 访问修饰符。

```c#
	public static void Wave () {}
```

这种方法将表示我们的数学函数 $f(x,t)=\sin(\pi(x+t))$。这意味着它必须产生一个结果，该结果必须是浮点数。因此，函数的返回类型需要是 `float`，而不是 `void`。

```c#
	public static float Wave () {}
```

接下来，我们必须将这两个参数添加到方法的参数列表中，就像数学函数一样。唯一的区别是，我们必须在每个参数前面写入类型，即 `float`。

```c#
	public static float Wave (float x, float t) {}
```

现在，我们可以将计算正弦波的代码放入该方法中，使用其 `x` 和 `t` 参数。

```c#
	public static float Wave (float x, float t) {
		Mathf.Sin(Mathf.PI * (x + t));
	}
```

最后一步是明确指出该方法的结果。由于这是一个 `float` 方法，因此它必须在完成时返回一个 `float` 值。我们通过在 `return` 后面写上结果应该是什么来表明这一点，这是我们的数学计算。

```c#
	public static float Wave (float x, float t) {
		return Mathf.Sin(Mathf.PI * (x + t));
	}
```

现在可以在 `Graph.Update` 中调用此方法，使用 `position.x` 和 `time` 作为其参数的参数。其结果可用于设置点的 Y 坐标，而不是显式的数学方程。

```c#
	void Update () {
		float time = Time.time;
		for (int i = 0; i < points.Length; i++) {
			Transform point = points[i];
			Vector3 position = point.localPosition;
			position.y = FunctionLibrary.Wave(position.x, time);
			point.localPosition = position;
		}
	}
```

### 1.3 隐式使用 Type

我们将使用 `Mathf.PI`，`Mathf.Sin` 和 `FunctionLibrary` 中 `Mathf` 的其他方法。如果我们可以写这些，而不必一直明确地提到类型，那就太好了。我们可以通过在 `FunctionLibrary` 文件的顶部添加另一个 `using` 语句来实现这一点，该语句后面有额外的 `static` 关键字和显式的 `UnityEngine.Mathf` 类型。这使得该类型的所有常量和静态成员都可以使用，而无需明确提及该类型本身。

```c#
using UnityEngine;

using static UnityEngine.Mathf;

public static class FunctionLibrary { … }
```

现在我们可以通过省略 `Mathf` 来缩短 `Wave` 中的代码。

```c#
	public static float Wave (float x, float z, float t) {
		return Sin(PI * (x + t));
	}
```

### 1.4 第二个函数

让我们添加另一个函数方法。这次我们将使用多个正弦波制作一个稍微复杂的函数。首先复制 `Wave` 方法并将其重命名为 `MultiWave`。

```c#
	public static float Wave (float x, float t) {
		return Sin(PI * (x + t));
	}

	public static float MultiWave (float x, float t) {
		return Sin(PI * (x + t));
	}
```

我们将保留已有的正弦函数，但会添加一些额外的东西。为了方便起见，在返回之前，将当前结果赋值给 `y` 变量。

```c#
	public static float MultiWave (float x, float t) {
		float y = Sin(PI * (x + t));
		return y;
	}
```

增加正弦波复杂性的最简单方法是添加另一个频率加倍的正弦波。这意味着它的变化速度是原来的两倍，这是通过将正弦函数的自变量乘以 2 来实现的。同时，我们将把这个函数的结果减半。这使得新正弦波的形状与旧正弦波相同，但大小减半。

```c#
		float y = Sin(PI * (x + t));
		y += Sin(2f * PI * (x + t)) / 2f;
		return y;
```

这给了我们数学函数 $f(x,t)=sin(\pi(x+t))+\frac{\sin(2\pi(x + t))}{2}$ 由于正弦函数的正负极端都是 1 和 -1，因此这个新函数的最大值和最小值可能是 1.5 和 -1.5。为了保证我们保持在 -1~1 的范围内，我们应该将总和除以 1.5。

```c#
		return y / 1.5f;
```

除法比乘法需要更多的工作，所以根据经验，乘法比除法更受欢迎。然而，常数表达式如 `1f / 2f` 和 `2f * Mathf.PI` 已经被编译器简化为一个数字。因此，我们可以重写代码，使其在运行时仅使用乘法。我们必须确保使用操作顺序和括号首先减少常量部分。

```c#
		y += Sin(2f * PI * (x + t)) * (1f / 2f);
		return y * (2f / 3f);
```

我们也可以直接写 `0.5f` 而不是 `1f / 2f`，但 1.5 的倒数不能精确地用十进制表示法写，所以我们将继续使用 `2f / 3f`，编译器将其简化为具有最大精度的浮点表示。

```c#
		y += 0.5f * Sin(2f * PI * (x + t));
```

现在使用此函数代替 `Graph.Update` 中的 `Wave` 并查看其外观。

```c#
			position.y = FunctionLibrary.MultiWave(position.x, time);
```

*两个正弦波之和。*

你可以说，一个较小的正弦波现在跟随着一个较大的正弦波。我们还可以使较小的一个沿着较大的一个滑动，例如将较大波浪的时间减半。结果将是一个函数，它不仅会随着时间的推移而滑动，还会改变其形状。现在，该模式需要四秒钟才能重复。

```c#
		float y = Sin(PI * (x + 0.5f * t));
		y += 0.5f * Sin(2f * PI * (x + t));
```

*变形波。*

### 1.5 在编辑器中选择函数

接下来我们可以做的是添加一些代码，以控制 `Graph` 使用哪个方法。我们可以用滑块来实现这一点，就像图形的分辨率一样。由于我们有两个函数可供选择，我们需要一个范围为 0-1 的可序列化整数字段。将其命名为 `function`，以便明确其控制的内容。

```c#
	[SerializeField, Range(10, 100)]
	int resolution = 10;
	
	[SerializeField, Range(0, 1)]
	int function;
```

![img](https://catlikecoding.com/unity/tutorials/basics/mathematical-surfaces/function-library/function-slider.png)

*函数滑块。*

现在我们可以检查 `Update` 循环中的 `function`。如果它为零，则图形应显示 `Wave`。为了做出这个选择，我们将使用 `if` 语句，后跟一个表达式和一个代码块。这就像 `while` 一样工作，只是它不循环，所以该块要么被执行，要么被跳过。在这种情况下，测试的是 `function` 是否等于零，这可以用 == 相等运算符来完成。

```c#
	void Update () {
		float time = Time.time;
		for (int i = 0; i < points.Length; i++) {
			Transform point = points[i];
			Vector3 position = point.localPosition;
			if (function == 0) {
				position.y = FunctionLibrary.Wave(position.x, time);
			}
			point.localPosition = position;
		}
	}
```

我们可以在 if 块后面加上 `else` 和另一个块，如果测试失败，则执行该块。在这种情况下，图形应显示 `MultiWave`。

```c#
			if (function == 0) {
				position.y = FunctionLibrary.Wave(position.x, time);
			}
			else {
				position.y = FunctionLibrary.MultiWave(position.x, time);
			}
```

这使得在我们处于播放模式时，也可以通过图的检查器来控制函数。

> **在播放模式下更改分辨率滑块是否有任何效果？**
>
> 这将导致图形的分辨率值发生变化，但 `Graph.Update` 不依赖于它，因此没有可见的效果。在游戏模式下更改点数需要删除和实例化点数，但我们在本教程中不支持。

### 1.6 波纹函数

让我们在库中添加第三个函数，它会产生类似涟漪的效果。我们通过使正弦波远离原点来创建它，而不是始终沿同一方向传播。我们可以通过基于与中心的距离来实现这一点，这是 X 的绝对值。在 `Mathf.Abs` 的帮助下，从计算开始，在一个新的 `FunctionLibrary.Ripple` 中。将距离存储在 `d` 变量中，然后返回。

```c#
	public static float Ripple (float x, float t) {
		float d = Abs(x);
		return d;
	}
```

要显示它，请将 `Graph.function` 的范围增加到 2，并在 `Update` 中为 `Wave` 方法添加另一个块。我们可以通过在 `else` 之后直接写入另一个 `if` 来链接多个条件块，这样它就变成了当 `function` 等于 1 时应该执行的 else-if 块。然后为波纹添加一个新的 `else` 块。

```c#
	[SerializeField, Range(0, 2)]
	…

	void Update () {
		float time = Time.time;
		for (int i = 0; i < points.Length; i++) {
			Transform point = points[i];
			Vector3 position = point.localPosition;
			if (function == 0) {
				position.y = FunctionLibrary.Wave(position.x, time);
			}
			else if (function == 1) {
				position.y = FunctionLibrary.MultiWave(position.x, time);
			}
			else {
				position.y = FunctionLibrary.Ripple(position.x, time);
			}
			point.localPosition = position;
		}
	}
```

![img](https://catlikecoding.com/unity/tutorials/basics/mathematical-surfaces/function-library/absolute-x.png)

*绝对 X。*

返回 `FunctionLibrary.Ripple`，我们使用距离作为正弦函数的输入，并将其作为结果。具体来说，我们将使用 $y = \sin(4\pi d)$ 与  $d=|x|$。因此，波纹在图的域中会多次上下波动。

```c#
	public static float Ripple (float x, float t) {
		float d = Abs(x);
		float y = Sin(4f * PI * d);
		return y;
	}
```

![img](https://catlikecoding.com/unity/tutorials/basics/mathematical-surfaces/function-library/sin-d.png)

*正弦距离。*

结果在视觉上很难解释，因为 Y 变化太大。我们可以通过减小波的振幅来减少这种情况。但波纹没有固定的振幅，它会随着距离的增加而减小。那么，让我们将函数转化为 $y=\frac{\sin(4\pi d)}{1+10d}$

```c#
		float y = Sin(4f * PI * d);
		return y / (1f + 10f * d);
```

最后一点是让涟漪变得生动起来。为了使其向外流动，我们必须从传递给正弦函数的值中减去时间。让我们使用 $\pi t$。因此，最终函数变为 $y=\frac{\sin(\pi(4d-t))}{1+10d}$

```c#
		float y = Sin(PI * (4f * d - t));
		return y / (1f + 10f * d);
```

*动画涟漪。*

## 2 管理方法

一系列条件块适用于两到三个函数，但当试图支持更多函数时，它会很快变得笨拙。如果我们可以向我们的库请求对基于某些标准的方法的引用，然后重复调用它，那会方便得多。

### 2.1 委托

可以通过使用委托来获取方法的引用。委托是一种特殊类型，它定义了某物可以引用的方法类型。我们的数学函数方法没有标准的委托类型，但我们可以自己定义它。因为它是一种类型，我们可以在它自己的文件中创建它，但由于它专门用于我们库的方法，我们将在 `FunctionLibrary` 类中定义它，使其成为内部或嵌套类型。

要创建委托类型，请复制 `Wave` 函数，将其重命名为 `Function`，并用分号替换其代码块。这定义了一个没有实现的方法签名。然后，我们通过将 `static` 关键字替换为 `delegate` 将其转换为委托类型。

```c#
public static class FunctionLibrary {
	
	public delegate float Function (float x, float t);
	
	…
}
```

现在，我们可以引入一个 `GetFunction` 方法，该方法在给定索引参数的情况下返回一个 `Function`，使用我们在循环中使用的 if-else 逻辑，除了在每个块中我们返回适当的方法而不是调用它。

```c#
	public delegate float Function (float x, float t);
	
	public static Function GetFunction (int index) {
		if (index == 0) {
			return Wave;
		}
		else if (index == 1) {
			return MultiWave;
		}
		else {
			return Ripple;
		}
	}
```

接下来，我们使用此方法在 `Graph.Update` 的开头获取一个函数委托。根据 `function` 进行更新，并将其存储在变量中。因为此代码不在 `FunctionLibrary` 中，所以我们必须将嵌套委托类型称为 `FunctionLibrary.Function`。

```c#
	void Update () {
		FunctionLibrary.Function f = FunctionLibrary.GetFunction(function);
		…
	}
```

然后调用委托变量，而不是循环中的显式方法。

```c#
		for (int i = 0; i < points.Length; i++) {
			Transform point = points[i];
			Vector3 position = point.localPosition;
			//if (function == 0) {
			//	position.y = FunctionLibrary.Wave(position.x, time);
			//}
			//else if (function == 1) {
			//	position.y = FunctionLibrary.MultiWave(position.x, time);
			//}
			//else {
			//	position.y = FunctionLibrary.Ripple(position.x, time);
			//}
			position.y = f(position.x, time);
			point.localPosition = position;
		}
```

### 2.2 委托数组

我们简化了 `Graph.Update` 很多，但我们只将 if-else 代码移动到了 `FunctionLibrary.GetFunction`。我们可以通过用数组索引替换它来完全摆脱这段代码。首先，将 `functions` 数组的静态字段添加到 `FunctionLibrary` 中。此数组仅供内部使用，因此不要将其公开。

```c#
	public delegate float Function (float x, float t);

	static Function[] functions;

	public static Function GetFunction (int index) { … }
```

我们总是将相同的元素放在这个数组中，这样我们就可以在它的声明中显式地定义它的内容。这是通过在花括号之间分配一个逗号分隔的数组元素序列来实现的。最简单的是一个空列表。

```c#
	static Function[] functions = {};
```

这意味着我们立即得到一个数组实例，但它是空的。更改此项，使其包含我们方法的委托，顺序与以前相同。

```c#
	static Function[] functions = { Wave, MultiWave, Ripple };
```

`GetFunction` 方法现在可以简单地索引数组以返回相应的委托。

```c#
	public static Function GetFunction (int index) {
		return functions[index];
	}
```

> **我们为什么不让数组 public 呢？**
>
> 这将使任何代码都可以更改数组。通过对图书馆保密，我们保证它永远不会改变。

### 2.3 枚举

整数滑块是有效的，但 0 表示波函数等并不明显。如果我们有一个包含函数名称的下拉列表，会更清楚。我们可以使用枚举来实现这一点。

枚举可以通过定义 `enum` 类型来创建。我们将在 `FunctionLibrary` 内部再次执行此操作，这次将其命名为 `FunctionName`。在这种情况下，类型名称后面是花括号内的标签列表。我们可以使用数组元素列表的副本，但不使用分号。请注意，这些是简单的标签，它们不引用任何东西，尽管它们遵循与类型名称相同的规则。我们有责任使这两份清单保持一致。

```c#
	public delegate float Function (float x, float t);

	public enum FunctionName { Wave, MultiWave, Ripple }

	static Function[] functions = { Wave, MultiWave, Ripple };
```

现在将 `GetFunction` 的索引参数替换为 `FunctionName` 类型的 name 参数。这表示参数必须是有效的函数名。

```c#
	public static Function GetFunction (FunctionName name) {
		return functions[name];
	}
```

枚举可以被认为是语法糖。默认情况下，枚举的每个标签代表一个整数。第一个标签对应 0，第二个标签对应 1，以此类推。因此，我们可以使用名称对数组进行索引。但是，编译器会抱怨枚举不能隐式转换为整数。我们必须明确地执行这个角色扮演。

```c#
		return functions[(int)name];
```

最后一步是将 `Graph.function` 字段的类型更改为 `FunctionLibrary.FunctionName` 并删除其 `Range` 属性。

```c#
	//[SerializeField, Range(0, 2)]
	[SerializeField]
	FunctionLibrary.FunctionName function;
```

`Graph` 的检查器现在显示了一个包含函数名称的下拉列表，在大写单词之间添加了空格。

![img](https://catlikecoding.com/unity/tutorials/basics/mathematical-surfaces/managing-methods/function-name.png)

*函数下拉列表。*

## 3 添加另一个维度

到目前为止，我们的图只包含一行点。我们将一维值映射到其他一维值，但如果你考虑时间，它实际上是将二维值映射到一维值。因此，我们已经将高维输入映射到 1D 值。就像我们添加了时间一样，我们也可以添加额外的空间维度。

目前，我们使用 X 维度作为函数的空间输入。Y 维度用于显示输出。这使得 Z 成为用于输入的第二个空间维度。添加 Z 作为输入将我们的线条升级为方形网格。

### 3.1 3D 颜色

在 Z 不再恒定的情况下，通过从赋值中删除 `.rg` 和 `.xy` 代码，更改我们的点曲面着色器，使其也修改蓝色反照率分量。

```glsl
			surface.Albedo = saturate(input.worldPos * 0.5 + 0.5);
```

并调整我们的*点 URP* 着色器图，使 Z 与 X 和 Y 被同等对待。

![img](https://catlikecoding.com/unity/tutorials/basics/mathematical-surfaces/adding-another-dimension/shader-graph.png)

*调整了乘法和加法节点输入。*

### 3.2 升级函数

为了支持我们的函数的第二个非时间输入，请在 `FunctionLibrary.Function` 委托类型的 `x` 参数后添加一个 `z` 参数。

```c#
	public delegate float Function (float x, float z, float t);
```

这要求我们还将参数添加到我们的三个函数方法中。

```c#
	public static float Wave (float x, float z, float t) { … }
	
	public static float MultiWave (float x, float z, float t) { … }
	
	public static float Ripple (float x, float z, float t) { … }
```

在 `Graph.Update` 中调用函数时，还添加 `position.z` 作为参数。

```c#
			position.y = f(position.x, position.z, time);
```

### 3.3 创建点网格

为了显示 Z 维度，我们必须将点线变成点网格。我们可以通过创建多条线来实现这一点，每条线沿 Z 偏移一步。我们将使用与 X 相同的 Z 范围，因此我们将创建与当前点一样多的线。这意味着我们必须将分数平方。调整 `Awake` 中 `points` 数组的创建，使其足够大以包含所有点。

```c#
		points = new Transform[resolution * resolution];
```

当我们根据分辨率在 `Awake` 中每次迭代循环时增加X坐标时，简单地创建更多点将导致一条长线。我们必须调整初始化循环，以考虑第二个维度。

![img](https://catlikecoding.com/unity/tutorials/basics/mathematical-surfaces/adding-another-dimension/long-line.png)

*2500 点的长线。*

首先，让我们明确地跟踪 X 坐标。通过在 `for` 循环中声明并递增 `x` 变量以及 `i` 迭代器变量来实现这一点。为此，`for` 语句的第三部分可以转换为逗号分隔的列表。

```c#
		points = new Transform[resolution * resolution];
		for (int i = 0, x = 0; i < points.Length; i++, x++) {
			…
		}
```

每次我们完成一行，我们都必须将 `x` 重置为零。当 `x` 等于分辨率时，一行就完成了，所以我们可以在循环顶部使用 `if` 块来处理这个问题。然后使用 `x` 而不是 `i` 来计算 X 坐标。

```c#
		for (int i = 0, x = 0; i < points.Length; i++, x++) {
			if (x == resolution) {
				x = 0;
			}
			Transform point = points[i] = Instantiate(pointPrefab);
			position.x = (x + 0.5f) * step - 1f;
			…
		}
```

接下来，每一行都必须沿 Z 维度偏移。这也可以通过在 `for` 循环中添加一个 `z` 变量来实现。此变量不得在每次迭代中递增。相反，它只在我们移动到下一行时递增，因为我们已经有一个 `if` 块了。然后，使用 `z` 而不是 `x`，将位置的 Z 坐标设置为与 X 坐标相同。

```c#
		for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++) {
			if (x == resolution) {
				x = 0;
				z += 1;
			}
			Transform point = points[i] = Instantiate(pointPrefab);
			position.x = (x + 0.5f) * step - 1f;
			position.z = (z + 0.5f) * step - 1f;
			…
		}
```

我们现在创建一个由点组成的方形网格，而不是一条线。因为我们的函数仍然只使用X维度，所以看起来原始点已经被拉伸成直线。

![img](https://catlikecoding.com/unity/tutorials/basics/mathematical-surfaces/adding-another-dimension/grid.png)

*图形网格。*

### 3.4 更好的视觉效果

因为我们的图形现在是 3D 的，所以从现在开始，我将使用游戏窗口从透视图的角度来看它。要快速选择一个好的相机位置，您可以在游戏模式下在场景窗口中找到一个不错的视点，退出游戏模式，然后使游戏的相机与视点匹配。您可以通过*“游戏对象”/“与视图对齐”*来完成此操作，选择*主摄影机*并显示场景窗口。我让它在XZ对角线上大致向下看。然后，我将*平行光*的 Y 旋转从 -30 改为 30，以改善该视角的照明。

除此之外，我们可以稍微调整阴影质量。使用默认渲染管道时，阴影可能看起来已经可以接受，但它们被配置为在我们近距离查看图形时看得很远。

通过转到“*质量*”项目设置并选择一个预配置的级别，可以为默认渲染管道选择视觉质量级别。默认下拉菜单控制独立应用程序默认使用哪个级别。

![img](https://catlikecoding.com/unity/tutorials/basics/mathematical-surfaces/adding-another-dimension/quality-levels.png)

*质量水平。*

我们可以进一步调整阴影的性能和精度，方法是转到下面的“*阴影*”部分，将“*阴影距离*”减小到 10，并将“*阴影级联*”设置为“*无级联*”。默认设置渲染阴影四次，这对我们来说太过分了。

![quality](https://catlikecoding.com/unity/tutorials/basics/mathematical-surfaces/adding-another-dimension/shadow-settings-default-quality.png)
![game](https://catlikecoding.com/unity/tutorials/basics/mathematical-surfaces/adding-another-dimension/shadow-settings-default-game.png)

*默认渲染管道的阴影设置。*

> **阴影级联和距离控制是什么？**
>
> Unity 和大多数游戏引擎将阴影投射器渲染成纹理，然后对其进行采样以创建阴影。这些阴影贴图具有固定的分辨率。如果它们必须覆盖大面积，则单个像素也会变大，从而产生块状阴影。
>
> 通过减少最大阴影距离，我们减少了阴影贴图必须覆盖的区域，从而提高了阴影质量，但代价是在距离上丢失了阴影。
>
> 阴影级联通过使用基于距离的多个贴图，使平行光更进一步，因此附近的阴影最终比远处的阴影具有更高的分辨率。这两个渲染管道最多支持四个级联。

URP 不使用这些设置，而是通过 *URP* 资产的检查器配置其阴影。默认情况下，它已经只渲染一次方向阴影，但*阴影/最大距离*可以减少到 10。此外，为了与默认渲染管道的标准超质量相匹配，请启用“*阴影/软阴影*”，并将“*照明*”下的“*照明/主光/阴影分辨率*”提高到 4096。

![inspector](https://catlikecoding.com/unity/tutorials/basics/mathematical-surfaces/adding-another-dimension/shadow-settings-urp-inspector.png)
![game](https://catlikecoding.com/unity/tutorials/basics/mathematical-surfaces/adding-another-dimension/shadow-settings-urp-game.png)

*URP 的阴影设置。*

最后，在播放模式下，您可能会注意到视觉撕裂。通过游戏窗口工具栏左侧的第二个下拉菜单启用 *VSync（仅游戏视图)*，可以防止在游戏窗口中发生这种情况。启用后，新帧的呈现与显示刷新率同步。只有在没有同时可见的场景窗口的情况下，这才能可靠地工作。VSync 是通过质量设置的“*其他*”部分为独立应用程序配置的。

![img](https://catlikecoding.com/unity/tutorials/basics/mathematical-surfaces/adding-another-dimension/game-window-vsync.png)

*VSnyc 已启用游戏窗口。*

> **为什么帧率下降了？**
>
> 与之前的单线相比，网格包含更多的点。在分辨率 50 时，它有 2500 个点。在分辨率 100 时，它有 10000 个点。为了获得最佳性能，始终只有一个场景或游戏窗口可见。此外，请确保 `Graph` 的对象层次结构在层次结构窗口中折叠，这样就不必列出任何点。
>
> 我们将在下一教程中更仔细地了解性能。

### 3.5 合并 Z

在 `Wave` 函数中使用 Z 的最简单方法是使用 X 和 Z 的和，而不仅仅是 X。这将创建一个对角线波。

```c#
	public static float Wave (float x, float z, float t) {
		return Sin(PI * (x + z + t));
	}
```

*对角线波浪。*

`MultiWave` 最直接的变化是让每个波使用一个单独的维度。让我们用 Z 来做较小的那个。

```c#
	public static float MultiWave (float x, float z, float t) {
		float y = Sin(PI * (x + 0.5f * t));
		y += 0.5f * Sin(2f * PI * (z + t));
		return y * (2f / 3f);
	}
```

我们还可以添加沿 XZ 对角线传播的第三个波。让我们使用与 `Wave` 相同的波浪，只是时间减慢到四分之一。然后将结果除以 2.5，使其保持在 -1~1 范围内。

```c#
		float y = Sin(PI * (x + 0.5f * t));
		y += 0.5f * Sin(2f * PI * (z + t));
		y += Sin(PI * (x + z + 0.25f * t));
		return y * (1f / 2.5f);
```

请注意，第一波和第三波将以规则的间隔相互抵消。

*三波。*

最后，为了使波纹在 XZ 平面上向各个方向扩散，我们必须计算两个维度上的距离。在 `Mathf.Sqrt` 的帮助下，我们可以使用毕达哥拉斯定理来解决这个问题。

> **毕达哥拉斯定理是什么？**
>
> 毕达哥拉斯定理指出 $a^2 + b^2 = c^2$，此处 $c$ 是直角三角形斜边的长度。$a$ 以及 $b$ 是它另外两条边的长度。
>
> ![img](https://catlikecoding.com/unity/tutorials/basics/mathematical-surfaces/adding-another-dimension/pythagorean-theorem.png)
>
> *使用毕达哥拉斯定理。*
>
> 在 XZ 平面中的 2D 点的情况下，这种三角形的斜边对应于原点和该点之间的线，其 X 和 Z 坐标为其他两条边的长度。因此，我们每个点与原点之间的距离为 $\sqrt{x^2 + z^2}$

```c#
	public static float Ripple (float x, float z, float t) {
		float d = Sqrt(x * x + z * z);
		float y = Sin(PI * (4f * d - t));
		return y / (1f + 10f * d);
	}
```

*XZ 平面上出现波纹。*

## 4 离开网格

通过使用 X 和 Z 来定义 Y，我们能够创建描述各种曲面的函数，但它们总是链接到 XZ 平面。没有两个点可以具有相同的 X 和 Z 坐标，同时具有不同的 Y 坐标。这意味着我们曲面的曲率是有限的。它们的斜坡不能垂直，也不能向后折叠。为了实现这一点，我们的函数不仅要输出 Y，还要输出 X 和 Z。

### 4.1 三维函数

如果我们的函数输出 3D 位置而不是 1D 值，我们可以使用它们来创建任意曲面。例如，函数 $f(x,z)=\left[\begin{array}{c}x\\0\\z\end{array}\right]$ 描述 XZ 平面，而函数 $f(x,z)=\left[\begin{array}{c}x\\z\\0\end{array}\right]$ 描述了 XY 平面。

因为这些函数的输入参数不再需要对应于最终的 X 和 Z 坐标，所以不再适合命名它们 $x$ 以及 $z$。相反，它们用于创建参数化曲面，通常被命名为 $u$ 以及 $v$。因此，我们得到的函数如下 $f(u,v)=\left[\begin{array}{c}u\\ \sin(\pi(u+v))\\z\end{array}\right]$

调整我们的 `Function` 委托类型以支持这种新方法。唯一需要的更改是将其 `float` 返回类型替换为 `Vector3`，但让我们也重命名它的参数。

```c#
	public delegate Vector3 Function (float u, float v, float t);
```

我们还必须相应地调整我们的函数方法。我们只需将 U 和 V 直接用于 X 和 Z。不需要调整参数名称——只需要它们的类型与委托相匹配——但让我们这样做以保持一致。如果你的代码编辑器支持它，你可以快速重构重命名参数和其他东西，这样它就可以通过菜单或上下文菜单选项一次重命名。

从 `Wave` 开始。让它首先声明一个 `Vector3` 变量，然后设置它的组件，然后返回它。我们不必给向量一个初始值，因为我们在返回它之前设置了它的所有字段。

```c#
	public static Vector3 Wave (float u, float v, float t) {
		Vector3 p;
		p.x = u;
		p.y = Sin(PI * (u + v + t));
		p.z = v;
		return p;
	}
```

然后给予 `MultiWave` 和 `Ripple` 相同的治疗。

```c#
	public static Vector3 MultiWave (float u, float v, float t) {
		Vector3 p;
		p.x = u;
		p.y = Sin(PI * (u + 0.5f * t));
		p.y += 0.5f * Sin(2f * PI * (v + t));
		p.y += Sin(PI * (u + v + 0.25f * t));
		p.y *= 1f / 2.5f;
		p.z = v;
		return p;
	}

	public static Vector3 Ripple (float u, float v, float t) {
		float d = Sqrt(u * u + v * v);
		Vector3 p;
		p.x = u;
		p.y = Sin(PI * (4f * d - t));
		p.y /= 1f + 10f * d;
		p.z = v;
		return p;
	}
```

由于点的 X 和 Z 坐标不再恒定，我们也不能再依赖 `Graph.Update` 中的初始值。我们可以通过将 `Update` 中的循环替换为 `Awake` 中使用的循环来解决这个问题，除了我们现在可以直接将函数结果分配给点的位置。

```c#
	void Update () {
		FunctionLibrary.Function f = FunctionLibrary.GetFunction(function);
		float time = Time.time;
		float step = 2f / resolution;
		for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++) {
			if (x == resolution) {
				x = 0;
				z += 1;
			}
			float u = (x + 0.5f) * step - 1f;
			float v = (z + 0.5f) * step - 1f;
			points[i].localPosition = f(u, v, time);
		}
	}
```

请注意，我们只需要在 `z` 变化时重新计算 `v`。这确实要求我们在循环开始之前设置其初始值。

```c#
		float v = 0.5f * step - 1f;
		for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++) {
			if (x == resolution) {
				x = 0;
				z += 1;
				v = (z + 0.5f) * step - 1f;
			}
			float u = (x + 0.5f) * step - 1f;
			//float v = (z + 0.5f) * step - 1f;
			points[i].localPosition = f(u, v, time);
		}
```

另请注意，由于 `Update` 现在使用分辨率，在播放模式下更改分辨率将使图形变形，将网格拉伸或挤压成矩形。

> **为什么不使用嵌套双循环？**
>
> 这也是可能的，也是在二维上循环时的常用方法。然而，这种方法主要是在点而不是维度上循环。即使在播放模式下分辨率发生变化，它也会始终更新所有点。

我们不再需要在 `Awake` 中初始化位置，因此我们可以使该方法更简单。我们只需设置点的比例和父级就足够了。

```c#
	void Awake () {
		float step = 2f / resolution;
		var scale = Vector3.one * step;
		//var position = Vector3.zero;
		points = new Transform[resolution * resolution];
		for (int i = 0; i < points.Length; i++) {
			//if (x == resolution) {
			//	x = 0;
			//	z += 1;
			//}
			Transform point = points[i] = Instantiate(pointPrefab);
			//position.x = (x + 0.5f) * step - 1f;
			//position.z = (z + 0.5f) * step - 1f;
			//point.localPosition = position;
			point.localScale = scale;
			point.SetParent(transform, false);
		}
	}
```

### 4.2 创建球体

为了证明我们确实不再局限于每个 (X, Z) 坐标对一个点，让我们创建一个定义球体的函数。为此，向 `FunctionLibrary` 添加一个 `Sphere` 方法。还将其条目添加到 `FunctionName` 枚举和 `functions` 数组中。首先，始终在原点返回一个点。

```c#
	public enum FunctionName { Wave, MultiWave, Ripple, Sphere }

	static Function[] functions = { Wave, MultiWave, Ripple, Sphere };

	…

	public static Vector3 Sphere (float u, float v, float t) {
		Vector3 p;
		p.x = 0f;
		p.y = 0f;
		p.z = 0f;
		return p;
	}
```

创建球体的第一步是描述一个平放在 XZ 平面上的圆。我们可以使用 $\left[\begin{array}{c}\sin(\pi u)\\0\\ \cos(\pi u)\end{array}\right]$。为此，仅依靠 $u$

```c#
		p.x = Sin(PI * u);
		p.y = 0f;
		p.z = Cos(PI * u);
```

![img](https://catlikecoding.com/unity/tutorials/basics/mathematical-surfaces/leaving-the-grid/circle.png)

*一个圆圈。*

我们现在有多个完美重叠的圆。我们可以根据以下公式沿 Y 方向挤出它们 $v$，这给了我们一个无盖的圆柱体。

```c#
		p.x = Sin(PI * u);
		p.y = v;
		p.z = Cos(PI * u);
```

![img](https://catlikecoding.com/unity/tutorials/basics/mathematical-surfaces/leaving-the-grid/cylinder.png)

*一个圆柱体。*

我们可以通过按某个值缩放 X 和 Z 来调整圆柱体的半径 $r$。如果我们使用 $r = \cos(\frac{\pi}2 v)$ 然后圆柱体的顶部和底部折叠成单点。

```c#
		float r = Cos(0.5f * PI * v);
		Vector3 p;
		p.x = r * Sin(PI * u);
		p.y = v;
		p.z = r * Cos(PI * u);
```

![img](https://catlikecoding.com/unity/tutorials/basics/mathematical-surfaces/leaving-the-grid/cylinder-collapsing.png)

*具有收缩半径的圆柱体。*

这使我们接近球体，但圆柱体半径的减小还不是圆形的。这是因为一个圆是由正弦和余弦组成的，我们现在只使用余弦作为它的半径。方程的另一部分是 Y，目前仍等于 $v$。为了完成所有圆圈，我们必须使用 $y=\sin(\frac{\pi}2 v)$

```c#
		p.y = Sin(PI * 0.5f * v);
```

![img](https://catlikecoding.com/unity/tutorials/basics/mathematical-surfaces/leaving-the-grid/sphere.png)

*一个球体。*

结果是一个球体，使用通常称为 UV 球体的图案创建。虽然这种方法创建了一个正确的球体，但请注意，点的分布并不均匀，因为球体是通过堆叠不同半径的圆创建的。或者，我们可以认为它由围绕 Y 轴旋转的多个半圆组成。

### 4.3 扰动球体

让我们调整球体的表面，使其更有趣。要做到这一点，我们必须稍微调整一下我们的公式。我们将使用 $f(u,v)=\left[\begin{array}{c}s\sin(\pi u)\\r\sin(\frac\pi 2 v)\\ s\cos(\pi u)\end{array}\right]$ 此处 $s=r\cos(\frac\pi 2 v)$ 且 $r$ 是半径。这使得可以设置半径的动画。例如，我们可以通过使用 $r=\frac{1+\sin(\pi t)} 2$ 来根据时间缩放它。

```c#
		float r = 0.5f + 0.5f * Sin(PI * t);
		float s = r * Cos(0.5f * PI * v);
		Vector3 p;
		p.x = s * Sin(PI * u);
		p.y = r * Sin(0.5f * PI * v);
		p.z = s * Cos(PI * u);
```

*缩放球体。*

我们不必使用统一的半径。我们可以根据 $u$，比如 $r = \frac{9+\sin(8\pi u)}{10}$

```c#
		float r = 0.9f + 0.1f * Sin(8f * PI * u);
```

![img](https://catlikecoding.com/unity/tutorials/basics/mathematical-surfaces/leaving-the-grid/sphere-vertical-bands.png)

*带有垂直带的球体；分辨率 100。*

这使球体看起来具有垂直条带。我们可以通过以下方式切换到水平带 $v$ 而不是 $u$

```c#
		float r = 0.9f + 0.1f * Sin(8f * PI * v);
```

![img](https://catlikecoding.com/unity/tutorials/basics/mathematical-surfaces/leaving-the-grid/sphere-horizontal-bands.png)

*带有水平条带的球体。*

通过使用这两种方法，我们得到了扭曲的带子。让我们也增加时间让它们旋转，最后 $r = \frac{9 + \sin(\pi(6u+4v+t))}{10}$

```c#
		float r = 0.9f + 0.1f * Sin(PI * (6f * u + 4f * v + t));
```

*旋转扭曲球体。*

### 4.4 创建一个 Torus

最后，让我们向 `FunctionLibrary` 添加一个环面。复制 `Sphere`，将其重命名为 `Torus`，并将其半径设置为 1。同时更新名称和函数数组。

```c#
	public enum FunctionName { Wave, MultiWave, Ripple, Sphere, Torus }

	static Function[] functions = { Wave, MultiWave, Ripple, Sphere, Torus };

	…

	public static Vector3 Torus (float u, float v, float t) {
		float r = 1f;
		float s = r * Cos(0.5f * PI * v);
		Vector3 p;
		p.x = s * Sin(PI * u);
		p.y = r * Sin(0.5f * PI * v);
		p.z = s * Cos(PI * u);
		return p;
	}
```

我们可以通过将球体的垂直半圆相互拉开并将其变成完整的圆，将球体变成圆环。让我们从切换到 $s=\frac 1 2 + r\cos(\frac\pi 2 v)$

```c#
		float s = 0.5f + r * Cos(0.5f * PI * v);
```

![img](https://catlikecoding.com/unity/tutorials/basics/mathematical-surfaces/leaving-the-grid/sphere-pulled-apart.png)

*球体被撕裂*。

这给了我们半个圆环，只考虑了圆环的外部。为了完成环面，我们必须使用 $v$ 描述一个完整的圆而不是半个圆。这可以通过在 $s$ 和 $y$ 中使用 $\pi v$ 而不是 $\frac\pi 2 v$

```c#
		float s = 0.5f + r * Cos(PI * v);
		Vector3 p;
		p.x = s * Sin(PI * u);
		p.y = r * Sin(PI * v);
		p.z = s * Cos(PI * u);
```

![img](https://catlikecoding.com/unity/tutorials/basics/mathematical-surfaces/leaving-the-grid/torus-spindle.png)

*自相交的纺锤环。*

因为我们把球体拉开了半个单位，所以产生了一个自相交的形状，称为纺锤环（spindle torus）。如果我们把它拆开一个单位，我们就会得到一个不自相交的环面，但也没有孔，这被称为角环面（horn torus）。因此，我们将球体拉开的距离会影响圆环的形状。具体来说，它定义了环面的主半径。另一个半径是小半径，它决定了环的厚度。让我们将大半径定义为 $r_1$ 并将另一个重命名为 $r_2$ 所以 $s = r_2\cos(\pi v)+r_1$。然后使用 0.75 作为主要半径，0.25 作为次要半径，以将点保持在 -1~1 范围内。

```c#
		//float r = 1f;
		float r1 = 0.75f;
		float r2 = 0.25f;
		float s = r1 + r2 * Cos(PI * v);
		Vector3 p;
		p.x = s * Sin(PI * u);
		p.y = r2 * Sin(PI * v);
		p.z = s * Cos(PI * u);
```

![img](https://catlikecoding.com/unity/tutorials/basics/mathematical-surfaces/leaving-the-grid/torus-ring.png)

*圆环。*

现在我们有两个半径可以用来制作一个更有趣的圆环。例如，我们可以通过使用 $r_1=\frac{7+\sin(\pi(6u+\frac{t}{2}))}{10}$ 将它转为一个旋转星形，同时使用 $r_2=\frac{3+\sin(\pi(8u+4v+2t))}{20}$ 扭曲环

```c#
		float r1 = 0.7f + 0.1f * Sin(PI * (6f * u + 0.5f * t));
		float r2 = 0.15f + 0.05f * Sin(PI * (8f * u + 4f * v + 2f * t));
```

*扭曲的圆环。*

现在，您已经有了一些使用描述曲面的非平凡函数的经验，以及如何将它们可视化。你可以尝试使用自己的函数来更好地掌握它的工作原理。有许多看似复杂的参数曲面可以用几个正弦波创建。

下一个教程是[测量性能](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/)。

[license](https://catlikecoding.com/unity/tutorials/license/)

[repository](https://bitbucket.org/catlikecodingunitytutorials/basics-03-mathematical-surfaces/)

[PDF](https://catlikecoding.com/unity/tutorials/basics/mathematical-surfaces/Mathematical-Surfaces.pdf)

# 测量性能：MS 和 FPS

发布于 2020-10-09 更新于 2021-05-18

https://catlikecoding.com/unity/tutorials/basics/measuring-performance/

*使用游戏窗口统计数据、帧调试器和分析器。*
*比较动态批处理、GPU实例化和SRP批处理。*
*显示帧率计数器。*
*自动循环功能。*
*功能之间平稳过渡。*

这是关于学习使用 Unity 的[基础](https://catlikecoding.com/unity/tutorials/basics/)知识的系列教程中的第四个。这是对衡量性能的介绍。我们还将在函数库中添加变形函数的功能。

本教程使用 Unity 2020.3.6f1 编写。

![img](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/tutorial-image.jpg)

*在波浪和球体之间的某个地方。*

## 1 分析 Unity

Unity 不断渲染新帧。为了使任何移动的东西看起来都是流畅的，它必须足够快，这样我们才能将图像序列感知为连续的运动。通常，每秒 30 帧（简称 FPS）是最低目标，60FPS 是理想的。这些数字经常出现，因为许多设备的显示刷新率为 60 赫兹。如果不关闭 VSync，您无法绘制比这更快的帧，这将导致图像撕裂。如果无法实现一致的 60FPS，那么下一个最佳速率是 30FPS，即每两次显示刷新一次。低一步将是 15FPS，这对于流体运动来说是不够的。

> **其他常见的显示器刷新率是什么？**
>
> 75Hz、85Hz 和 144Hz 也常见于台式显示器。对于竞技游戏场景，刷新率甚至更高。因此，如果你的应用程序能够可靠地达到 85FPS，那么它在所有显示器上打开 VSync 时都会表现良好。如果它只能达到 60FPS，那么 75Hz 的显示器将下降到 37.5FPS 的一半速率，85Hz 将减半到 42.5FPS，144Hz 将下降到 48FPS 的三分之一速率。不过，这假设性能保持不变。实际上，帧速率可能在刷新率的倍数之间波动。

能否达到目标帧率取决于处理单个帧需要多长时间。为了达到 60FPS，我们必须在 16.67 毫秒内更新和渲染每一帧。30FPS 的时间预算是其两倍，即每帧 33.33ms。

当我们的图运行时，我们可以通过简单的观察来了解它的运动有多平稳，但这是一种非常不精确的衡量其性能的方法。如果运动看起来很平滑，那么它可能会超过 30FPS，如果它看起来很结巴，那么可能会低于 30FPS。由于性能不一致，它也可能前一刻很流畅，下一刻就会结巴。这可能是由于我们应用程序的变化造成的，也可能是由于在同一设备上运行的其他应用程序造成的。Unity 编辑器的性能也可能不一致，具体取决于它正在做什么。如果我们勉强达到 60FPS，那么我们最终可能会在 30FPS 和 60FPS 之间快速来回切换，尽管平均 FPS 很高，但这会让人感到紧张。因此，为了更好地了解正在发生的事情，我们必须更精确地衡量绩效。Unity 有一些工具可以帮助我们做到这一点。

### 1.1 游戏窗口统计

游戏窗口有一个*统计*覆盖面板，可以通过其*统计*工具栏按钮激活。它显示对最后一个渲染帧进行的测量。它并没有告诉我们太多，但它是我们可以用来指示发生了什么的最简单的工具。在编辑模式下，游戏窗口通常只会在发生更改后偶尔更新。它在播放模式下刷新每一帧。

以下统计数据适用于我们的图，其中环面函数和分辨率为 100，使用默认的内置渲染管道，从现在开始我将称之为 BRP。我为游戏窗口打开了 VSync，因此刷新与我的 60 Hz 显示器同步。

![img](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/profiling-unity/stats-default.png)

*BRP 统计数据。*

统计数据显示，CPU 主线程耗时 31.7ms，渲染线程耗时 29.2ms。根据您的硬件和游戏窗口屏幕大小，您可能会得到不同的结果。在我的例子中，它表明整个帧的渲染时间为 60.9ms，但统计面板报告的帧速率为 31.5FPS，与 CPU 时间相匹配。FPS 指示器似乎花费了最长的时间，并假设与帧速率匹配。这是一种过于简化的做法，只考虑了 CPU 方面，忽略了 GPU 和显示器。实际帧速率可能较低。

> **什么是线程？**
>
> 线程是一个子进程，在这种情况下是 Unity 应用程序。可以同时并行运行多个线程。统计数据显示 Unity 的主线程和渲染线程在上一帧中运行了多长时间。

除了持续时间和 FPS 指示外，统计面板还显示了有关渲染内容的各种详细信息。共有 30.003 批，批处理显然节省了零。这些是发送到 GPU 的绘图命令。我们的图形包含 10000 个点，因此每个点似乎都被渲染了三次。这一次用于深度传递，一次用于阴影投射器（也单独列出），还有一次用于渲染每个点的最终立方体。其他三批用于额外的工作，如天空盒和阴影处理，这些工作与我们的图无关。还有六个 set pass 调用，这可以被认为是 GPU 被重新配置为以不同的方式渲染，比如使用不同的材质。

![img](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/profiling-unity/stats-urp.png)

*URP 的统计数据。*

如果我们使用 URP，统计数据就会不同。它渲染得更快。很容易看出原因：只有 20.002 批，比 BRP 少 10.001 批。这是因为 URP 没有为定向阴影使用单独的深度通道。它确实有更多的设置传递调用，但这似乎不是问题。

虽然通过批处理保存报告没有批处理，但 URP 默认使用 SRP 批处理器，但统计面板不理解它。SRP 批管理器不会消除单个绘图命令，但可以使它们更高效。为了说明这一点，请选择我们的 URP 资产，并在其检查器底部的“*高级*”部分下禁用 *SRP Batcher*。确保*动态批处理*也已禁用。

![img](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/profiling-unity/urp-advanced.png)

*URP 高级设置。*

在 SRP 批处理器禁用的情况下，URP 性能要差得多。

![img](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/profiling-unity/stats-urp-no-batcher.png)

*不带 SRP 批处理器的 URP 统计数据。*

### 1.2 动态批处理

除了 SRP 批处理器，URP 还有另一个用于动态批处理的开关。这是一种旧技术，它将小网格动态组合成一个较大的网格，然后进行渲染。为 URP 启用它会将批次减少到 10.024，统计面板显示消除了 9.978 次抽奖。

![img](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/profiling-unity/stats-urp-dynamic-batching.png)

*动态批处理 URP 的统计数据。*

在我的例子中，SRP 批处理器和动态批处理具有相当的性能，因为我们的图点的立方体网格是动态批处理的理想候选者。

SRP 批处理不适用于 BRP，但我们可以为其启用动态批处理。在这种情况下，我们可以在 Player 项目设置的“其他设置”部分找到切换，就在我们将颜色空间设置为线性的位置下方。仅当未使用可脚本化的渲染管道设置时，它才可见。

![img](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/profiling-unity/stats-default-dynamic-batching.png)

*动态批处理 BRP 的统计数据。*

动态批处理对 BRP 来说效率更高，消除了  29.964 批，将其减少到只有 39 批，但似乎没有多大帮助。

### 1.3 GPU 实例化

另一种提高渲染性能的方法是启用 GPU 实例化。这使得可以使用单个绘制命令来告诉 GPU 绘制具有相同材质的一个网格的多个实例，从而提供变换矩阵数组和可选的其他实例数据。在这种情况下，我们必须按材料启用它。我们有一个*启用 GPU 实例化*的开关。

![img](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/profiling-unity/material-gpu-instancing-enabled.png)

*启用 GPU 实例化的材质。*

URP 更喜欢 SRP 批处理器而不是 GPU 实例化，因此为了使其适用于我们的点，必须禁用 SRP 批处理器。然后我们可以看到，批处理的数量减少到只有 46 个，比动态批处理好得多。我们稍后会发现这种差异的原因。

![img](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/profiling-unity/stats-urp-gpu-instancing.png)

*带有 GPU 实例化的 URP 统计数据。*

从这些数据中我们可以得出结论，URP GPU 实例化是最好的，其次是动态批处理，然后是 SRP 批处理。但差异很小，所以它们在我们的图中似乎是等效的。唯一明确的结论是，应该使用 GPU 实例化或 SRP 批处理器。

对于 BRP GPU 实例化，批处理结果比动态批处理多，但帧速率略高。

![img](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/profiling-unity/stats-default-gpu-instancing.png)

*带 GPU 实例化的 BRP 统计数据。*

### 1.4 帧调试器

统计面板可以告诉我们，使用动态批处理与使用 GPU 实例化不同，但没有告诉我们为什么。为了更好地了解正在发生的事情，我们可以使用通过 *Window/Analysis/Frame Debugger* 打开的帧调试器。当通过其工具栏按钮启用时，它会显示一个列表，列出在游戏窗口的最后一帧发送到 GPU 的所有绘图命令，这些命令按分析样本分组。此列表显示在其左侧。其右侧显示了特定选定绘图命令的详细信息。此外，游戏窗口显示渐进式绘制状态，直到直接在所选命令之后。

> **为什么我的电脑突然变热了？**
>
> Unity 使用了一种技巧，需要一遍又一遍地渲染同一帧，以显示绘制帧的中间状态。只要帧调试器处于活动状态，它就会这样做。确保在不需要时禁用帧调试器。

在我们的例子中，我们必须处于播放模式，因为这是我们绘制图表的时候。启用帧调试器将暂停播放模式，这允许我们检查绘图命令层次结构。让我们首先对 BRP 执行此操作，而不使用动态批处理或 GPU 实例化。

![img](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/profiling-unity/frame-debugger-default.png)

*BRP 的帧调试器。*

我们总共看到 30.007 个 draw 调用，比统计面板报告的要多，因为还有一些命令不算作批处理，比如清除目标缓冲区。我们点的 30000 次绘制分别列为 *DepthPass.Job* 下的 *Draw Mesh Point(Clone)*，*Shadows.RenderDirJob* 和 *RenderForward.RenderLoopJob*。

如果我们再次尝试启用动态批处理，命令结构将保持不变，只是每组 10000 次绘制将减少到 12 次 *Draw Dynamic* 调用。就 CPU-GPU 通信开销而言，这是一个显著的改进。

![img](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/profiling-unity/frame-debugger-default-dynamic-batching.png)

*具有动态批处理的 BRP。*

如果我们使用 GPU 实例化，那么每个组将减少到 20 个*绘制网格（实例化）点（克隆）*调用。这是一个很大的改进，但方法不同。

![img](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/profiling-unity/frame-debugger-default-gpu-instancing.png)

*带 GPU 实例化的 BRP。*

我们可以看到 URP 也会发生同样的情况，但命令层次结构不同。在这种情况下，点会绘制两次，第一次是在阴影下。在 `RenderLoop.Draw` 下绘制并再次绘制。一个显著的区别是，动态批处理似乎不适用于阴影贴图，这解释了为什么它对 URP 的效果较差。我们最终也得到了 22 个批次，而不是只有 12 个批次，这表明 URP 材质比标准 BRP 材质依赖更多的网格顶点数据，因此单个批次中适合的点更少。与动态批处理不同，GPU 实例化确实适用于阴影，因此在这种情况下它更优越。

![nothing](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/profiling-unity/frame-debugger-urp.png)
![dynamic batching](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/profiling-unity/frame-debugger-urp-dynamic-batching.png)
![GPU instancing](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/profiling-unity/frame-debugger-urp-gpu-instancing.png)

*URP 无任何东西、动态批处理和 GPU 实例化。*

最后，在启用 SRP 批处理器的情况下，10000 个点被列为 11 个 *SRP 批*命令，但请记住，这些仍然是单独的绘制调用，只是非常高效的调用。

![img](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/profiling-unity/frame-debugger-urp-srp-batcher.png)

*URP 与 SRP 批处理器。*

### 1.5 额外的光

到目前为止，我们得到的结果是针对我们的图形，使用单向光和我们使用的其他项目设置。让我们看看当我们向场景中添加第二盏灯时会发生什么，特别是通过*游戏对象/灯光/点光源*添加点光源。将其位置设置为零，并确保它不会投射阴影，这是它的默认行为。BRP 支持点光源的阴影，但 URP 仍然不支持。

![img](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/profiling-unity/point-light.png)

*原点处没有阴影的点光源。*

有了额外的光线，BRP 现在可以额外绘制所有点。帧调试器向我们展示了 `RenderForward.RenderLoopJob` 的渲染量是以前的两倍。更糟糕的是，动态批处理现在只适用于深度和阴影过程，而不适用于正向过程。

![nothing](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/profiling-unity/point-light-nothing.png) ![dynamic batching](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/profiling-unity/point-light-dynamic-batching.png)

*BRP 无任何东西和动态批处理。*

这是因为 BRP 在每盏灯上绘制每个对象一次。它有一个与单向光一起工作的主通道，然后在其上渲染其他通道。这是因为它是一个老式的前向加性渲染管道。动态批处理无法处理这些不同的过程，因此不会被使用。

GPU 实例化也是如此，除了它仍然适用于主通道。只有额外的光通道没有从中受益。

![img](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/profiling-unity/point-light-gpu-instancing.png)

*带 GPU 实例化的 BRP。*

第二盏灯似乎对 URP 没有影响，因为它是一个现代的前向渲染器，可以在一次渲染中应用所有照明。因此，即使GPU需要在每次绘制时执行更多的照明计算，命令列表也保持不变。

这些结论适用于影响所有点的单个额外光线。如果添加更多灯光并移动它们，使不同的点受到不同灯光的影响，那么使用 GPU 实例化时，事情会变得更加复杂，批次可能会被拆分。对于一个简单的场景来说是真的，对于一个复杂的场景来说可能不是真的。

> **那么延迟渲染呢？**
>
> BRP 和 HDRP 都有正向和延迟渲染模式，而 URP 目前没有。延迟渲染的想法是对象只绘制一次，将其可见表面属性存储在 GPU 缓冲区中。之后，一个或多个照明过程仅将照明应用于可见的部分。与正向渲染相比，它有优点也有缺点，但我们不会在本系列教程中介绍它。

### 1.6 分析器

为了更好地了解 CPU 端发生了什么，我们可以打开分析器窗口。关闭点光源，然后通过*窗口/分析/分析器*打开窗口。它将在播放模式下记录性能数据，并将其存储以供以后检查。

分析器分为两部分。它的顶部包含一个显示各种性能图的模块列表。顶部是 *CPU 使用率*，这是我们将关注的重点。选择该模块后，窗口的底部显示了我们可以在图中选择的帧的详细分解。

![default](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/profiling-unity/profiler-default.png) ![urp](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/profiling-unity/profiler-urp.png)

*分析器显示 BRP 和 URP 的 CPU 使用时间线。*

CPU 使用率的默认底部视图是时间线。它可视化了在一个帧中花了多少时间。它显示每一帧都以 *PlayerLoop* 开始，*PlayerLoop* 大部分时间都在调用 *RunBehaviourUpdate*。再往下走两步，我们看到这主要是对 `Graph.Update` 方法的调用。您可以选择一个时间线块来查看其全名和持续时间（以毫秒为单位）。

在最初的播放器循环段之后是一个简短的 *EditorLoop* 部分，之后是帧的渲染部分的另一个播放器段，CPU 告诉 GPU 该做什么。工作在主线程、渲染线程和几个作业工作者线程之间分开，但 BRP 和 URP 的具体方法不同。这些线程并行运行，但当一个线程必须等待另一个线程的结果时，它们也有同步点。

在渲染部分之后——如果使用 URP，渲染线程仍然繁忙——会出现另一个编辑器段，之后下一帧开始。线程也可能看起来跨越帧边界。这是因为 Unity 可以在渲染线程完成之前在主线程上启动下一帧的更新循环，利用并行性。我们将在下一节稍后再讨论这个问题。

如果你对线程的确切时间不感兴趣，那么你可以通过左侧的下拉列表用*层次结构*视图替换*时间线*视图。层次结构在单个可排序列表中显示相同的数据。此视图使您更容易看到所需时间最长的部分以及内存分配发生的位置。

![img](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/profiling-unity/profiler-hierarchy.png)

*分析器显示层次结构。*

### 1.7 分析构建

分析器很明显，编辑器给我们的应用程序增加了很多开销。因此，当我们的应用程序独立运行时，对其进行分析会更有用。为此，我们必须构建我们的应用程序，专门用于调试。我们可以在通过*文件/构建设置...*打开的构建设置窗口中配置我们的应用程序的构建方式。如果尚未配置，则“*构建中的场景*”部分为空。这很好，因为默认情况下将使用当前打开的场景。

您可以选择您当前机器最方便的目标平台。然后启用“*开发构建*”和“*自动连接探查器*”选项。

![img](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/profiling-unity/development-build.png)

*用于分析的开发构建。*

要最终创建独立应用程序（通常称为构建），请在构建过程完成后按“*构建*”按钮或“*构建并运行*”立即打开应用程序。您还可以通过*文件/构建并运行*或指定的快捷方式触发另一个构建。

> **构建过程需要多长时间？**
>
> 第一次构建耗时最长，使用 URP 时可能会忙上几分钟。之后，如果可能的话，Unity 会重用之前生成的构建数据，大大加快了过程。除此之外，项目越大，所需时间越长。

一旦构建自行运行，过一段时间后退出并切换回 Unity。分析器现在应该包含有关其执行情况的信息。这并不总是发生在第一次构建之后，如果是这样，请重试。还要记住，即使启用了“*播放时清除*”，分析器在连接到构建时也不会清除旧数据，因此如果你只运行了几秒钟的应用程序，请确保你正在查看相关帧。

![default](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/profiling-unity/profiler-default-build.png) ![urp](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/profiling-unity/profiler-urp-build.png)

*分析构建、BRP 和 URP*

因为没有编辑器开销，所以构建应该比 Unity 编辑器中的播放模式表现更好。分析器确实将不再显示编辑器循环部分。

## 2 显示帧率

我们并不总是需要详细的分析信息，帧速率的粗略指示通常就足够了。此外，我们或其他人可能在没有 Unity 编辑器的地方运行我们的应用程序。对于这些情况，我们可以做的是在应用程序本身的一个小覆盖面板中测量和显示框架。默认情况下，此类功能不可用，因此我们将自己创建它。

### 2.1 UI 面板

Unity 的游戏内 UI 可以创建一个小的覆盖面板。我们还将使用 *TextMeshPro* 创建文本以显示帧率。*TextMeshPro* 是一个独立的软件包，包含高级文本显示功能，优于默认的 UI 文本组件。如果您尚未安装其软件包，请通过软件包管理器添加它。这也会自动安装 *Unity UI* 包，因为 *TextMeshPro* 依赖于它。

> **为什么不使用 *UI 工具包（Toolkit）*？**
>
> *UI 工具包*目前仅适用于编辑器 UI。有一个运行时使用的包，但它仍在预览中，所以我不会使用它。

一旦 UI 包成为项目的一部分，就可以通过GameObject/UI/panel创建一个面板。这将创建一个覆盖整个 UI 画布的半透明面板。画布与游戏窗口大小相匹配，但在场景窗口中要大得多。通过场景窗口工具栏启用 2D 模式，然后缩小，最容易看到它。

![img](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/showing-the-frame-rate/ui-panel.png)

*覆盖整个画布的面板。*

每个 UI 都有一个画布根对象，当我们添加一个面板时，它会自动创建。该面板是画布的子对象。还创建了一个 *EventSystem* 游戏对象，负责处理 UI 输入事件。我们不会使用这些，因此可以忽略甚至删除它。

![img](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/showing-the-frame-rate/ui-hierarchy.png)

*UI 游戏对象层次结构。*

画布有一个缩放器组件，可用于配置 UI 的缩放比例。默认设置假定像素大小恒定。如果你使用的是高分辨率或视网膜显示器，那么你必须增加比例因子，否则 UI 会太小。您还可以尝试其他缩放模式。

![img](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/showing-the-frame-rate/canvas-inspector.png)

*UI 画布游戏对象。*

UI 游戏对象有一个专门的  `RectTransform` 组件，可以替换通常的 `Transform` 组件。除了通常的位置、旋转和缩放之外，它还基于锚点暴露了额外的属性。锚点控制对象相对于其父对象的相对位置和大小调整行为。更改它的最简单方法是通过单击方形锚点图像打开的弹出窗口。

![img](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/showing-the-frame-rate/panel-inspector.png)

*UI 面板。*

我们将把帧率计数器面板放在窗口的右上角，因此将面板的锚点设置为右上角并将枢轴 XY 设置为 1。然后将宽度设置为 38，高度设置为 70，位置设置为零。之后，将图像组件的颜色设置为黑色，保持其 alpha 不变。

![img](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/showing-the-frame-rate/panel-top-right-corner.png)

*右上角的深色面板。*

### 2.2 文本

要在面板中放置文本，请通过 *GameObject/UI/Text - TextMeshPro* 创建 *TextMeshPro* UI 文本元素。如果这是您第一次创建 *TextMeshPro* 对象，将显示 *Import TMP Essentials* 弹出窗口。按照建议导入必需品。这将创建一个 *TextMesh Pro* 资产文件夹，其中包含一些资产，我们不需要直接处理。

创建文本游戏对象后，将其设置为面板的子对象，将其锚点设置为二维拉伸模式。使其与整个面板重叠，这可以通过将左、上、右和下设置为零来实现。也给它一个描述性的名称，比如*帧率文本*。

![img](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/showing-the-frame-rate/tmp-inspector.png)

*UI 文本。*

接下来，对 *TextMeshPro - Text (UI)*组件进行一些调整。将“*字体大小*”设置为 14，将“*对齐*”设置为居中。然后用占位符文本填充*文本输入*区域，特别是 *FPS*，后面是三行，每行三个零。

![img](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/showing-the-frame-rate/tmp-settings.png)

*文本设置。*

我们现在可以看到我们的帧率计数器是什么样子的。这三行或零是我们即将显示的统计数据的占位符。

![img](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/showing-the-frame-rate/frame-rate-text.png)

*帧率文本。*

### 2.3 更新显示器

要更新计数器，我们需要一个自定义组件。为 `FrameRateCounter` 组件创建新的 C# 脚本资源。给它一个可序列化的 `TMPro.TextMeshProUGUI` 字段，用于保存对用于显示其数据的文本组件的引用。

```c#
using UnityEngine;
using TMPro;

public class FrameRateCounter : MonoBehaviour {

	[SerializeField]
	TextMeshProUGUI display;
}
```

将此组件添加到文本对象并连接显示器。

![img](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/showing-the-frame-rate/frame-rate-counter-component.png)

*帧率计数器组件。*

为了显示帧率，我们需要知道前一帧和当前帧之间经过了多少时间。此信息可通过 `Time.deltaTime` 获得。但是，该值受可用于慢动作、快进或完全停止时间的时间尺度的影响。我们需要使用 `Time.unscaledDeltaTime`。在 `FrameRateCounter` 中的新 `Update` 方法开始时检索它。

```c#
	void Update () {
		float frameDuration = Time.unscaledDeltaTime;
	}
```

下一步是调整显示的文本。我们可以通过使用文本字符串参数调用其 `SetText` 方法来实现这一点。让我们从提供我们已经拥有的相同占位符文本开始。字符串写在双引号之间，换行符用特殊的 `\n` 字符序列写。

```c#
		float frameDuration = Time.unscaledDeltaTime;
		display.SetText("FPS\n000\n000\n000");
```

`TextMeshProUGUI` 具有接受其他浮点参数的变体 `SetText` 方法。添加帧持续时间作为第二个参数，然后将字符串的第一行三重零替换为花括号内的单个零。这指示了 `float` 参数应插入字符串的位置。

```c#
		display.SetText("FPS\n{0}\n000\n000", frameDuration);
```

帧持续时间告诉我们经过了多少时间。为了显示以每秒帧数表示的帧速率，我们必须显示其倒数，即除以帧持续时间。

```c#
		display.SetText("FPS\n{0}\n000\n000", 1f / frameDuration);
```

这将显示一个有意义的值，但它会有很多数字，比如 59.823424。我们可以通过在零后面加一个冒号和所需的数字，指示文本四舍五入到小数点后的特定数字。我们将四舍五入到一个整数，所以加零。

```c#
		display.SetText("FPS\n{0:0}\n000\n000", 1f / frameDuration);
```

*显示最后一帧的帧速率。*

### 2.4 平均帧速率

显示的帧速率最终会迅速变化，因为连续帧之间的时间几乎永远不会完全相同。我们可以通过显示帧速率平均值而不是仅显示最后一帧的速率来使其不那么不稳定。我们通过跟踪渲染的帧数和总持续时间来实现这一点，然后显示帧数除以它们的组合持续时间。

```c#
	int frames;

	float duration;
	
	void Update () {
		float frameDuration = Time.unscaledDeltaTime;
		frames += 1;
		duration += frameDuration;
		display.SetText("FPS\n{0:0}\n000\n000", frames / duration);
	}
```

这将使我们的反向趋势在运行时间越长的情况下趋于稳定的平均值，但这个平均值是我们应用程序的整个运行时间。由于我们需要最新的信息，我们必须经常重置并重新开始，采样一个新的平均值。我们可以通过添加一个可序列化的采样持续时间字段来进行配置，默认设置为 1 秒。给它一个合理的范围，比如 0.1-2。持续时间越短，我们得到的结果就越精确，但随着变化速度的加快，它将更难阅读。

```c#
	[SerializeField]
	TextMeshProUGUI display;

	[SerializeField, Range(0.1f, 2f)]
	float sampleDuration = 1f;
```

![img](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/showing-the-frame-rate/sample-duration.png)

*采样持续时间设置为 1 秒。*

从现在开始，我们只会在累计持续时间等于或超过配置的采样持续时间时调整显示。我们可以用 >= 更大或相等运算符来检查这一点。更新显示后，将累积帧和持续时间设置回零。

```c#
	void Update () {
		float frameDuration = Time.unscaledDeltaTime;
		frames += 1;
		duration += frameDuration;

		if (duration >= sampleDuration) {
			display.SetText("FPS\n{0:0}\n000\n000", frames / duration);
			frames = 0;
			duration = 0f;
		}
	}
```

*一秒钟内的平均帧速率。*

### 2.5 最佳和最差

平均帧率波动是因为我们的应用程序的性能不是恒定的。它有时会变慢，要么是因为它暂时有更多的工作要做，要么是由于在同一台机器上运行的其他进程造成了阻碍。为了了解这些波动有多大，我们还将记录并显示采样期间发生的最佳和最差帧持续时间。将最佳持续时间设置默认情况下为 `float.MaxValue`，这是最差的最佳持续时间。

```c#
	float duration, bestDuration = float.MaxValue, worstDuration;
```

每次更新都会检查当前帧持续时间是否小于迄今为止的最佳持续时间。如果是这样，请将其设置为新的最佳持续时间。还要检查当前帧持续时间是否大于迄今为止最差的持续时间。如果是这样，则将其设置为新的最差持续时间。

```c#
	void Update () {
		float frameDuration = Time.unscaledDeltaTime;
		frames += 1;
		duration += frameDuration;

		if (frameDuration < bestDuration) {
			bestDuration = frameDuration;
		}
		if (frameDuration > worstDuration) {
			worstDuration = frameDuration;
		}
		
		…
	}
```

现在，我们将把最佳帧率放在第一行，将平均帧率放在第二行，将最差帧率放在最后一行。我们可以通过向 `SetText` 添加两个参数并向字符串添加更多占位符来实现这一点。它们是索引，因此第一个数字用 0 表示，第二个用 1 表示，第三个用 2 表示。之后，还重置最佳和最差持续时间。

```c#
		if (duration >= sampleDuration) {
			display.SetText(
				"FPS\n{0:0}\n{1:0}\n{2:0}",
				1f / bestDuration,
				frames / duration,
				1f / worstDuration
			);
			frames = 0;
			duration = 0f;
			bestDuration = float.MaxValue;
			worstDuration = 0f;
		}
```

*最佳、平均和最差帧率。*

请注意，即使启用了 VSync，最佳帧速率也可能超过显示刷新率。同样，最差的帧速率也不一定是显示器刷新率的倍数。这是可能的，因为我们没有测量显示帧之间的持续时间。我们正在测量 Unity 帧之间的持续时间，这是其更新循环的迭代。Unity 的更新循环与显示器没有完全同步。当分析器显示下一帧的播放器循环开始时，当前帧的渲染线程仍然繁忙，我们已经看到了这一点。渲染线程完成后，GPU 仍有一些工作要做，之后显示器刷新仍需要一些时间。因此，我们显示的 FPS 并不是真正的帧速率，而是 Unity 告诉我们的。理想情况下，这些都是一样的，但要做到这一点很复杂。有一篇 Unity 博客文章介绍了 Unity 在这方面的改进，但这甚至没有说明全部情况。

### 2.6 帧持续时间

每秒帧数是衡量感知性能的一个很好的单位，但当试图达到目标帧率时，显示帧持续时间更有用。例如，当试图在移动设备上实现稳定的 60FPS 时，每毫秒都很重要。因此，让我们在帧率计数器中添加一个显示模式配置选项。

在 `FrameRateCounter` 中为 FPS 和 MS 定义 `DisplayMode` 枚举，然后添加该类型的可序列化字段，默认设置为 FPS。

```c#
	[SerializeField]
	TextMeshProUGUI display;

	public enum DisplayMode { FPS, MS }

	[SerializeField]
	DisplayMode displayMode = DisplayMode.FPS;
```

![img](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/showing-the-frame-rate/display-mode.png)

*可配置的显示模式。*

然后，当我们在更新中刷新显示时，检查模式是否设置为 FPS。如果是这样，那就做我们已经在做的事情。否则，将 FPS 标头替换为 MS 并使用逆参数。将它们乘以 1000，从秒转换为毫秒。

```c#
		if (duration >= sampleDuration) {
			if (displayMode == DisplayMode.FPS) {
				display.SetText(
					"FPS\n{0:0}\n{1:0}\n{2:0}",
					1f / bestDuration,
					frames / duration,
					1f / worstDuration
				);
			}
			else {
				display.SetText(
					"MS\n{0:0}\n{1:0}\n{2:0}",
					1000f * bestDuration,
					1000f * duration / frames,
					1000f * worstDuration
				);
			}
			frames = 0;
			duration = 0f;
			bestDuration = float.MaxValue;
			worstDuration = 0f;
		}
```

帧持续时间通常以十分之一毫秒为单位进行测量。通过将数字四舍五入从零增加到 1，我们可以一步提高显示的精度。

```c#
				display.SetText(
					"MS\n{0:1}\n{1:1}\n{2:1}",
					1000f * bestDuration,
					1000f * duration / frames,
					1000f * worstDuration
				);
```

![img](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/showing-the-frame-rate/precise-frame-duration.png)

*帧持续时间。*

### 2.7 内存分配

我们的帧率计数器已经完成，但在继续之前，让我们检查一下它对性能的影响有多大。显示UI需要每帧更多的绘制调用，但这并没有什么区别。在播放模式下使用分析器，然后搜索我们更新文本的帧。事实证明，这并不需要太多时间，但它确实分配了内存。这最容易通过层次结构视图检测到，按 *GC Alloc* 列排序。

![img](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/showing-the-frame-rate/profiler-hierarchy-allocations.png)

*分析器层次结构中显示的分配。*

文本字符串是对象。当我们通过 `SetText` 创建一个新的字符串对象时，它会生成一个新字符串对象，负责分配 106 个字节。Unity 的 UI 刷新将其增加到 4.5 千字节。虽然这并不多，但它会累积起来，在某个时候触发内存垃圾收集过程，这将导致不希望的帧持续时间尖峰。

重要的是要注意临时对象的内存分配，并尽可能消除重复出现的对象。幸运的是，`SetText` 和 Unity 的 UI 更新仅在编辑器中执行这些内存分配，原因有很多，比如更新文本输入字段。如果我们分析一个构建，那么我们将找到一些初始分配，但不会再找到更多。因此，对构建进行概要分析至关重要。分析编辑器播放模式只会给人留下第一印象。

## 3 自动功能切换

现在我们知道了如何分析我们的应用程序，我们可以比较它在显示不同功能时的性能。如果一个函数需要更多的计算，CPU 必须做更多的工作，这样会降低帧率。不过，点的计算方式对 GPU 没有影响。如果分辨率相同，GPU 将必须执行相同的工作量。

最大的区别在于波函数和环面函数之间。我们可以通过分析器比较他们的 CPU 使用情况。我们可以比较两个配置了不同功能的单独运行，也可以在播放模式下进行配置并在播放过程中进行切换。

![img](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/automatic-function-switching/spike-during-switch.png)

*从环面切换到波浪时发出尖刺。*

CPU 图显示，从圆环切换到波浪后，负载确实降低了。当切换发生时，也会出现巨大的帧持续时间峰值。这是因为通过编辑器进行更改时，播放模式会暂时暂停。由于取消选举和编辑焦点的变化，后来也出现了一些较小的峰值。

这些尖刺属于*其他*类别。CPU 图可以通过切换左侧的类别标签进行过滤，这样我们只能看到相关数据。禁用“*其他*”类别后，计算量的变化更加明显。

![img](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/automatic-function-switching/others-not-shown.png)

*其他类别未显示。*

> **剩下的小尖刺是什么？**
>
> 这就是刚刚激活的垃圾收集器。

由于停顿，通过检查器切换功能对于分析来说很尴尬。更糟糕的是，我们必须进行新的构建来分析不同的功能。我们可以通过在图中添加切换函数的能力来改进这一点，除了通过其检查器，还可以自动或通过用户输入。我们将在本教程中选择第一个选项。

### 3.1 函数循环

我们将自动循环所有函数。每个功能将在固定的持续时间内显示，之后将显示下一个功能。要使函数持续时间可配置，请在 `Graph` 中为其添加一个可序列化字段，默认值为 1 秒。同时，通过赋予其 `Min` 属性，将其最小值设置为零。持续时间为零将导致每帧切换到不同的函数。

```c#
	[SerializeField]
	FunctionLibrary.FunctionName function;

	[SerializeField, Min(0f)]
	float functionDuration = 1f;
```

![img](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/automatic-function-switching/function-duration.png)

*函数持续时间。*

从现在开始，我们需要跟踪当前函数的活动时间，并在需要时切换到下一个函数。这将使我们的 `Update` 方法复杂化。其当前代码仅处理更新当前函数，因此让我们将其移动到单独的 `UpdateFunction` 方法中，并让 `Update` 调用它。这使我们的代码保持有序。

```c#
	void Update () {
		UpdateFunction();
	}

	void UpdateFunction () {
		FunctionLibrary.Function f = FunctionLibrary.GetFunction(function);
		float time = Time.time;
		float step = 2f / resolution;
		float v = 0.5f * step - 1f;
		for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++) { … }
	}
```

现在添加一个持续时间字段，并在 `Update` 开始时将其增加（可能按比例）增量时间。然后，如果持续时间等于或超过配置的持续时间，则将其重置为零。之后是调用 `UpdateFunction`。

```c#
	Transform[] points;
	
	float duration;

	…
	
	void Update () {
		duration += Time.deltaTime;
		if (duration >= functionDuration) {
			duration = 0f;
		}
		UpdateFunction();
	}
```

我们很可能永远不会完全达到函数持续时间，我们会稍微超过它。我们可以忽略这一点，但为了与功能切换的预期时间保持合理的同步，我们应该从下一个功能的持续时间中扣除额外的时间。我们通过从当前持续时间中减去所需的持续时间来实现这一点，而不是将其设置为零。

```c#
		if (duration >= functionDuration) {
			duration -= functionDuration;
		}
```

为了遍历函数，我们将在 `FunctionLibrary` 中添加一个 `GetNextFunctionName` 方法，该方法接受一个函数名并返回下一个。由于枚举是整数，我们只需将其参数加一并返回即可。

```c#
	public static FunctionName GetNextFunctionName (FunctionName name) {
		return name + 1;
	}
```

但我们也必须循环回到第一个函数，而不是跳过最后一个函数，否则我们最终会得到一个无效的名称。因此，只有当提供的名称小于环面时，我们才能增加它。否则，我们返回第一个函数，即波。我们可以用if-else块来实现这一点，每个块都返回相应的结果。

```c#
		if (name < FunctionName.Torus) {
			return name + 1;
		}
		else {
			return FunctionName.Wave;
		}
```

我们可以通过将名称（作为 int）与函数数组的长度减 1 进行比较，使此方法函数名不可知，该长度与最后一个函数的索引相匹配。如果我们在最后，我们也可以返回零，这是第一个索引。这种方法的优点是，如果稍后更改函数名称，我们就不必调整方法。

```c#
		if ((int)name < functions.Length - 1) {
			return name + 1;
		}
		else {
			return 0;
		}
```

也可以通过使用 `?:` 三元条件运算符将方法体简化为单个表达式。这是一个 if-then-else 表达式 `?` 以及 `:` 分离其各个部分。两个备选方案都必须产生相同类型的值。

```c#
	public static FunctionName GetNextFunctionName (FunctionName name) {
		return (int)name < functions.Length - 1 ? name + 1 : 0;
	}
```

使用 `Graph.Update` 中的新方法以在适当的时候切换到下一个函数。

```c#
		if (duration >= functionDuration) {
			duration -= functionDuration;
			function = FunctionLibrary.GetNextFunctionName(function);
		}
```

*循环函数。*

现在，我们可以通过分析构建来按顺序查看所有函数的性能。

![img](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/automatic-function-switching/profiler-build-looping-functions.png)

*分析循环函数的构建。*

在我的例子中，所有函数的帧速率都是一样的，因为它从未降至 60FPS 以下。通过等待 VSync 来消除差异。使用 VSync 功能分析构建会使差异更加明显。或者，只在分析器中显示脚本。

![img](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/automatic-function-switching/profiler-build-looping-functions-only-scripts.png)

*仅显示脚本。*

事实证明，*Wave* 是最快的，其次是 *Ripple*，然后是 *Multi Wave*，在那个之后是 *Sphere*，*Torus* 是最慢的。这在知道代码的前提下符合我们的期望。

> **VSync 不应该在分析器中以黄色单独显示吗？**
>
> 这是应该的，但这在 Unity 2020 中不会发生在我身上。相反，它与渲染相结合。

### 3.2 随机函数

让我们通过添加一个选项在函数之间随机切换，而不是在固定序列中循环，使我们的图更有趣一些。将 `GetRandomFunctionName` 方法添加到 `FunctionLibrary` 中以支持此操作。它可以通过 0 调用 `Range.Random` 来选择随机索引，函数数组长度为参数。所选索引有效，因为这是该方法的整数版本，提供的范围是包容性的。

```c#
	public static FunctionName GetRandomFunctionName () {
		var choice = (FunctionName)Random.Range(0, functions.Length);
		return choice;
	}
```

我们可以更进一步，确保我们永远不会连续两次得到相同的函数。通过将我们的新方法重命名为 `GetRandomFunctionNameOtherThan` 并添加函数名参数来实现这一点。增加 `Random.Range` 的第一个参数为 1，因此索引零永远不会随机选择。然后检查选项是否等于要避免的名称。如果是，返回名字，否则返回所选名字。因此，我们用零代替不允许的索引，而不会引入选择偏差。

```c#
	public static FunctionName GetRandomFunctionNameOtherThan (FunctionName name) {
		var choice = (FunctionName)Random.Range(1, functions.Length);
		return choice == name ? 0 : choice;
	}
```

回到 `Graph`，为过渡模式添加一个配置选项，可以是循环或随机。再次使用自定义枚举字段执行此操作。

```c#
	[SerializeField]
	FunctionLibrary.FunctionName function;
	
	public enum TransitionMode { Cycle, Random }

	[SerializeField]
	TransitionMode transitionMode;
```

选择下一个函数时，检查转换模式是否设置为循环。如果是这样，请调用 `GetNextFunctionName`，否则调用 `GetRandomFunctionName`。由于这使选择下一个函数变得复杂，让我们也把这段代码放在一个单独的方法中，保持 `Update` 的简单性。

```c#
	void Update () {
		duration += Time.deltaTime;
		if (duration >= functionDuration) {
			duration -= functionDuration;
			//function = FunctionLibrary.GetNextFunctionName(function);
			PickNextFunction();
		}
		UpdateFunction();
	}
	
	void PickNextFunction () {
		function = transitionMode == TransitionMode.Cycle ?
			FunctionLibrary.GetNextFunctionName(function) :
			FunctionLibrary.GetRandomFunctionNameOtherThan(function);
	}
```

![inspector](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/automatic-function-switching/transition-mode-random.png)

*选取随机函数。*

### 3.3 插值函数

我们通过使函数之间的转换更有趣来结束本教程。我们不会突然切换到另一个函数，而是将图形平滑地变形为下一个函数。这对于性能分析也很有趣，因为它需要在转换过程中同时计算两个函数。

首先，在 `FunctionLibrary` 中添加一个 `Morph` 函数来处理转换。为其提供与函数方法相同的参数，再加上两个 `Function` 参数和一个  `float` 参数来控制变形进度。

```c#
	public static Vector3 Morph (
		float u, float v, float t, Function from, Function to, float progress
	) {}
```

我们使用 `Function` 参数而不是 `FunctionName` 参数，因为这样 `Graph` 可以在每次更新时按名称检索一次函数，所以我们不必在每个点访问函数数组两次。

> **为什么每次检索函数都会更新图中的 `Graph`？**
>
> 我们也可以将函数存储在 `Graph` 的字段中，而不是每次更新都获取它们。我们不这样做是因为 `Function` 类型的字段值不能在热重新加载中生存，而 `FunctionName` 字段可以。此外，每次更新检索一两个函数不会对性能产生有意义的影响。但是，每次更新每个点都要做很多不必要的额外工作。
>
> Unity 的 `UnityEvent` 类型是可序列化的，因此我们可以使用它们，但它们增加了我们不需要的更多开销和功能。它们通常用于将方法与 UI 事件挂钩。

进度是一个 0-1 的值，我们将使用它从第一个提供的函数插值到第二个函数。我们可以使用 `Vector3.Lerp` 函数，将这两个函数的结果和进度值传递给它。

```c#
	public static Vector3 Morph (
		float u, float v, float t, Function from, Function to, float progress
	) {
		return Vector3.Lerp(from(u, v, t), to(u, v, t), progress);
	}
```

*Lerp* 是线性插值的简写。它将在功能之间产生直线恒速过渡。我们可以通过在开始和结束时放慢进度来使它看起来更平滑。这是通过以零、一和进展（progress）作为参数调用 `Mathf.Smoothstep` 替换原始进度来实现的，走得更平稳。它适用于 $3x^2-2x^3$ 函数，通常称为 smoothstep。`Smoothstep` 的前两个参数是此函数的偏移量和比例，我们不需要使用 0 和 1。

![img](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/automatic-function-switching/smoothstep.png)

*0–1 平滑且线性。*

```c#
		return Vector3.Lerp(from(u, v, t), to(u, v, t), SmoothStep(0f, 1f, progress));
```

`Lerp` 方法对其第三个参数进行钳制，使其落在 0-1 的范围内。`Smoothstep` 方法也能做到这一点。我们将后者配置为输出 0-1 值，因此不需要 `Lerp` 的额外箝位。对于这样的情况，有一种替代的 `LerpUnclamped` 方法，所以让我们使用它。

```c#
		return Vector3.LerpUnclamped(
			from(u, v, t), to(u, v, t), SmoothStep(0f, 1f, progress)
		);
```

### 3.4 过渡

函数之间的过渡期需要一个持续时间，因此在 `Graph` 中为其添加一个配置选项，其最小值和默认值与函数持续时间相同。

```c#
	[SerializeField, Min(0f)]
	float functionDuration = 1f, transitionDuration = 1f;
```

![img](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/automatic-function-switching/transition-duration.png)

*过渡持续时间。*

我们的图现在可以处于两种模式，要么过渡，要么不过渡。我们将使用 `bool` 类型的布尔字段来跟踪这一点。我们还需要跟踪从中转换的函数的名称。

```c#
	float duration;

	bool transitioning;

	FunctionLibrary.FunctionName transitionFunction;
```

`UpdateFunction` 方法用于显示单个函数。复制它并将新的重命名为 `UpdateFunctionTransition`。更改它，使其同时获得两个函数并计算进度，即当前持续时间除以转换持续时间。然后让它调用 `Morph`，而不是其循环中的单个函数。

```c#
	void UpdateFunctionTransition () {
		FunctionLibrary.Function
			from = FunctionLibrary.GetFunction(transitionFunction),
			to = FunctionLibrary.GetFunction(function);
		float progress = duration / transitionDuration;
		float time = Time.time;
		float step = 2f / resolution;
		float v = 0.5f * step - 1f;
		for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++) {
			…
			points[i].localPosition = FunctionLibrary.Morph(
				u, v, time, from, to, progress
			);
		}
	}
```

在更新结束时，检查我们是否正在转换。如果是这样，请调用 `UpdateFunctionTransition`，否则调用 `UpdateFuction`。

```c#
	void Update () {
		duration += Time.deltaTime;
		if (duration >= functionDuration) {
			duration -= functionDuration;
			PickNextFunction();
		}

		if (transitioning) {
			UpdateFunctionTransition();
		}
		else {
			UpdateFunction();
		}
	}
```

一旦持续时间超过函数持续时间，我们就继续下一个。在选择下一个函数之前，请表明我们正在转换，并使转换函数等于当前函数。

```c#
		if (duration >= functionDuration) {
			duration -= functionDuration;
			transitioning = true;
			transitionFunction = function;
			PickNextFunction();
		}
```

但如果我们已经在转型，我们就必须做点别的。所以，首先检查我们是否正在过渡。只有当情况不是这样时，我们才必须检查是否超过了函数持续时间。

```c#
		duration += Time.deltaTime;
		if (transitioning) {}
		else if (duration >= functionDuration) {
			duration -= functionDuration;
			transitioning = true;
			transitionFunction = function;
			PickNextFunction();
		}
```

如果我们正在过渡，那么我们必须检查我们是否超过了过渡持续时间。如果是这样，从当前持续时间中扣除过渡持续时间，然后切换回单函数模式。

```c#
		if (transitioning) {
			if (duration >= transitionDuration) {
				duration -= transitionDuration;
				transitioning = false;
			}
		}
		else if (duration >= functionDuration) { … }
```

*函数之间的转换。*

如果我们现在进行剖析，我们确实可以在转换 `Grpah.Update` 中看到需要更长的时间。具体需要多少时间取决于它混合了哪些功能。

![img](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/automatic-function-switching/profiler-build-showing-transitions.png)

*分析器构建显示了过渡的额外脚本工作。*

值得重申的是，你得到的分析结果取决于你的硬件，可能与我在本教程中展示的示例大不相同。在开发自己的应用程序时，确定您支持的最低硬件规格，并使用这些规格进行测试。您的开发机器仅用于初步测试。如果你的目标是多个平台或硬件规格，那么你需要多个测试设备。

下一个教程是[计算着色器](https://catlikecoding.com/unity/tutorials/basics/compute-shaders/)。

[license](https://catlikecoding.com/unity/tutorials/license/)

[repository](https://bitbucket.org/catlikecodingunitytutorials/basics-04-measuring-performance/)

[PDF](https://catlikecoding.com/unity/tutorials/basics/measuring-performance/Measuring-Performance.pdf)



# 计算着色器：渲染一百万个立方体

发布于 2020-11-06 更新于 2021-05-18

https://catlikecoding.com/unity/tutorials/basics/compute-shaders/

