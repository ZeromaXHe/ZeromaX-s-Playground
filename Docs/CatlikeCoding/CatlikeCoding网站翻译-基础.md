# [返回主 Markdown](./CatlikeCoding网站翻译.md)

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

*将位置存储在计算缓冲区中。*
*让GPU完成大部分工作。*
*按程序画许多立方体。*
*将整个函数库复制到GPU。*

这是关于学习使用 Unity [基础](https://catlikecoding.com/unity/tutorials/basics/)知识的系列教程中的第五个。这次我们将使用计算着色器来显著提高图形的分辨率。

本教程使用 Unity 2020.3.6f1 编写。

![img](https://catlikecoding.com/unity/tutorials/basics/compute-shaders/tutorial-image.jpg)

*一百万个移动的立方体。*

## 1 将工作转移到 GPU

图形的分辨率越高，CPU 和 GPU 需要做的工作就越多，计算位置和渲染立方体。点数等于分辨率的平方，因此分辨率加倍会显著增加工作量。在分辨率为 100 的情况下，我们可能能够达到 60FPS，但我们能把它推多远？如果我们遇到瓶颈，我们能用不同的方法来克服它吗？

### 1.1 分辨率 200

让我们首先将 `Graph` 的最大分辨率从 100 倍提高到 200 倍，看看我们能得到什么性能。

```c#
	[SerializeField, Range(10, 200)]
	int resolution = 10;
```

![inspector](https://catlikecoding.com/unity/tutorials/basics/compute-shaders/moving-work-to-the-gpu/resolution-200-inspector.png)
![game](https://catlikecoding.com/unity/tutorials/basics/compute-shaders/moving-work-to-the-gpu/resolution-200-game.png)

*分辨率设置为 200 的图形。*

我们现在渲染 40000 点。在我的例子中，BRP 构建的平均帧速率降至 10FPS，URP 构建降至 15FPS。这对于流畅的体验来说太低了。

![BRP](https://catlikecoding.com/unity/tutorials/basics/compute-shaders/moving-work-to-the-gpu/resolution-200-profiler-drp.png) ![URP](https://catlikecoding.com/unity/tutorials/basics/compute-shaders/moving-work-to-the-gpu/resolution-200-profiler-urp.png)

*分析分辨率为 200 的构建，不包括 VSync、BRP 和 URP。*

对构建进行分析表明，所有事情都需要大约四倍的时间，这是有道理的。

### 1.2 GPU 图形

排序、批处理，然后将 40000 个点的变换矩阵发送到 GPU 需要花费大量时间。一个矩阵由 16 个浮点数组成，每个浮点数为 4 个字节，每个矩阵总共 64 B。对于 40000 个点，即 256 万字节，大约 2.44MiB，每次绘制点时都必须复制到 GPU。URP 必须每帧执行两次此操作，一次用于阴影，另一次用于常规几何体。BRP 必须至少做三次，因为它只有额外的深度通过，除了主方向光之外，每个光都要再做一次。

> **什么是 MiB？**
>
> 因为计算机硬件使用二进制数来寻址内存，所以它是按 2 的幂而不是 10 的幂进行分区的。MiB 是 mebibyte的后缀，即 2^20^=1024^2^=1048576 字节。这最初被称为兆字节，用 MB 表示，但现在应该表示 10^6^ 字节，与官方定义的 100 万字节相匹配。然而，MB、GB 等仍然经常被使用，而不是 MiB、GiB 等。

一般来说，最好尽量减少 CPU 和 GPU 之间的通信和数据传输量。由于我们只需要点的位置来显示它们，如果这些数据只存在于 GPU 侧，那将是理想的。这将消除大量的数据传输。但随后 CPU 无法再计算位置，GPU 必须取而代之。幸运的是，它非常适合这项任务。

让 GPU 计算位置需要不同的方法。我们将保持当前图形不变以进行比较，并创建一个新的图形。复制 `Graph` C# 资源文件并将其重命名为 `GPUGraph`。从新类中删除 `pointPrefab` 和 `points` 字段。然后还删除其 `Awake`、`UpdateFunction` 和 `UpdateFunctionTransition` 方法。我只标记了新类的已删除代码，而不是将所有内容标记为新代码。

```c#
using UnityEngine;

public class GPUGraph : MonoBehaviour {

	//[SerializeField]
	//Transform pointPrefab;

	[SerializeField, Range(10, 200)]
	int resolution = 10;

	[SerializeField]
	FunctionLibrary.FunctionName function;

	public enum TransitionMode { Cycle, Random }

	[SerializeField]
	TransitionMode transitionMode = TransitionMode.Cycle;

	[SerializeField, Min(0f)]
	float functionDuration = 1f, transitionDuration = 1f;

	//Transform[] points;

	float duration;

	bool transitioning;

	FunctionLibrary.FunctionName transitionFunction;

	//void Awake () { … }

	void Update () { … }

	void PickNextFunction () { … }

	//void UpdateFunction () { … }

	//void UpdateFunctionTransition () { … }
}
```

然后在 `Update` 结束时删除调用现在缺失的方法的代码。

```c#
	void Update () {
		…

		//if (transitioning) {
		//	UpdateFunctionTransition();
		//}
		//else {
		//	UpdateFunction();
		//}
	}
```

我们新的 `GPUGraph` 组件是 `Graph` 的一个被删除的版本，它公开了相同的配置选项，但去掉了前缀。它包含了从一个函数转换到另一个函数的逻辑，但除此之外没有做任何事情。使用此组件创建一个游戏对象，分辨率为 200，设置为瞬时过渡循环。停用原始图形对象，以便只有 GPU 版本保持活动状态。

![img](https://catlikecoding.com/unity/tutorials/basics/compute-shaders/moving-work-to-the-gpu/gpu-graph-component.png)

GPU 图形组件，设置为瞬时过渡。

### 1.3 计算缓冲区

为了在 GPU 上存储位置，我们需要为它们分配空间。我们通过创建 `ComputeBuffer` 对象来实现这一点。在 `GPUGraph` 中为位置缓冲区添加一个字段，并通过调用 `new ComputeBuffer()`（也称为构造函数）在新的 `Awake` 方法中创建对象。它的工作原理类似于分配一个新数组，但适用于对象或结构。

```c#
	ComputeBuffer positionsBuffer;

	void Awake () {
		positionsBuffer = new ComputeBuffer();
	}
```

我们需要将缓冲区的元素数量作为参数传递，即分辨率的平方，就像 `Graph` 的 positions 数组一样。

```c#
		positionsBuffer = new ComputeBuffer(resolution * resolution);
```

计算缓冲区包含任意非类型化数据。我们必须通过第二个参数指定每个元素的确切大小（以字节为单位）。我们需要存储由三个浮点数组成的 3D 位置向量，因此元素大小是四字节的三倍。因此，40000 个位置需要 0.48MB 或大约 0.46MiB 的 GPU 内存。

```c#
		positionsBuffer = new ComputeBuffer(resolution * resolution, 3 * 4);
```

这为我们提供了一个计算缓冲区，但这些对象在热重新加载后无法生存，这意味着如果我们在播放模式下更改代码，它将消失。我们可以通过将 `Awake` 方法替换为 `OnEnable` 方法来处理这个问题，每次启用组件时都会调用该方法。这发生在它醒来后——除非它被禁用——以及热重新加载完成后。

```c#
	void OnEnable () {
		positionsBuffer = new ComputeBuffer(resolution * resolution, 3 * 4);
	}
```

除此之外，我们还应该添加一个配套的 `OnDisable` 方法，当组件被禁用时，该方法会被调用，如果图被破坏，在热重新加载之前也会发生这种情况。通过调用其 `Release` 方法释放缓冲区。这表示缓冲区占用的 GPU 内存可以立即释放。

```c#
	void OnDisable () {
		positionsBuffer.Release();
	}
```

由于在此之后我们将不再使用此特定对象实例，因此最好将字段显式设置为引用 `null`。如果我们的图在播放模式下被禁用或销毁，Unity 的内存垃圾回收过程可以在下次运行时回收该对象。

```c#
	void OnDisable () {
		positionsBuffer.Release();
		positionsBuffer = null;
	}
```

> **如果我们不显式释放缓冲区，会发生什么？**
>
> 如果没有任何对象的引用，当垃圾收集器回收它时，它最终会被释放。但这种情况发生的时间是任意的。最好尽快明确地释放它，以避免堵塞内存。

### 1.4 计算着色器

为了计算 GPU 上的位置，我们必须为它编写一个脚本，特别是一个计算着色器。通过资源/创建/着色器/计算着色器创建一个。它将成为我们 `FunctionLibrary` 类的 GPU 等价物，因此也将其命名为 *FunctionLibrary*。虽然它被称为着色器并使用 HLSL 语法，但它作为一个通用程序运行，而不是用于渲染事物的常规着色器。因此，我将资源放置在 *Scripts* 文件夹中。

![img](https://catlikecoding.com/unity/tutorials/basics/compute-shaders/moving-work-to-the-gpu/compute-shader-asset.png)

*函数库计算着色器资源。*

打开资产文件并删除其默认内容。计算着色器需要包含一个称为内核的主函数，通过 `#pragma kernel` 指令后跟一个名称表示，如我们的表面着色器的 `#pragma surface`。使用 `FunctionKernel` 名称将此指令添加为第一行，也是当前唯一的一行。

```glsl
#pragma kernel FunctionKernel
```

在指令下方定义函数。这是一个 `void` 函数，最初没有参数。

```glsl
#pragma kernel FunctionKernel

void FunctionKernel () {}
```

### 1.5 计算线程

当 GPU 被指示执行计算着色器函数时，它会将其工作划分为组，然后安排它们独立并行运行。每个组又由多个线程组成，这些线程执行相同的计算，但输入不同。我们必须通过在内核函数中添加 `numthreads` 属性来指定每个组应该有多少线程。它需要三个整数参数。最简单的选择是对所有三个参数使用 1，这使得每个组只运行一个线程。

```glsl
[numthreads(1, 1, 1)]
void FunctionKernel () {}
```

GPU 硬件包含始终以锁步方式运行特定固定数量线程的计算单元。这些被称为扭曲或波阵面。如果一个组的线程数量小于扭曲大小，一些线程将闲置，浪费时间。如果线程数量超过了大小，那么 GPU 将为每组使用更多的扭曲。一般来说，64 个线程是一个很好的默认值，因为这与 AMD GPU 的扭曲大小相匹配，而 NVidia GPU 的扭曲尺寸为 32，因此后者每组将使用两个扭曲。实际上，硬件更复杂，可以用线程组做更多的事情，但这与我们的简单图无关。

`numthreads` 的三个参数可用于在一维、二维或三维中组织线程。例如，（64,1,1）在一个维度上为我们提供了 64 个线程，而（8,8,1）提供了相同的数量，但表示为 2D 8×8 方形网格。当我们根据 2D UV 坐标定义点时，让我们使用后一种选项。

```glsl
[numthreads(8, 8, 1)]
```

每个线程都由一个由三个无符号整数组成的向量标识，我们可以通过在函数中添加 `uint3` 参数来访问该向量。

```glsl
void FunctionKernel (uint3 id) {}
```

> **什么是无符号整数？**
>
> 它是一个整数，没有数字符号的指示符，因此它是无符号的。无符号整数为零或正。因为无符号整数不需要使用位来指示符号，所以它们可以存储更大的值，但这通常并不重要。

我们必须明确指出此参数用于线程标识符。我们通过在参数名称后写一个冒号，后跟 `SV_DispatchThreadID` 着色器语义关键字来实现这一点。

```glsl
void FunctionKernel (uint3 id: SV_DispatchThreadID) {}
```

### 1.6 UV 坐标

如果我们知道图形的步长，我们可以将线程标识符转换为 UV 坐标。为它添加一个名为 *_Step* 的计算机着色器属性，就像我们向曲面着色器添加 *_Smoothness* 一样。

```glsl
float _Step;

[numthreads(8, 8, 1)]
void FunctionKernel (uint3 id: SV_DispatchThreadID) {}
```

然后创建一个 `GetUV` 函数，该函数将线程标识符作为参数，并将 UV 坐标作为 `float2` 返回。我们可以使用在 `Graph` 中循环点时应用的相同逻辑。取标识符的 XY 分量，加 0.5，乘以步长，然后减 1。

```glsl
float _Step;

float2 GetUV (uint3 id) {
	return (id.xy + 0.5) * _Step - 1.0;
}
```

### 1.7 设置位置

要存储位置，我们需要访问位置缓冲区。在 HLSL 中，计算缓冲区被称为结构化缓冲区。因为我们必须写入它，所以我们需要支持读写的版本，即 `RWStructuredBuffer`。为名为 *_Positions* 的对象添加着色器属性。

```glsl
RWStructuredBuffer _Positions;

float _Step;
```

在这种情况下，我们必须指定缓冲区的元素类型。位置是 `float3` 值，我们直接在尖括号之间的 `RWStructuredBuffer` 后面写入。

```glsl
RWStructuredBuffer<float3> _Positions;
```

为了存储点的位置，我们需要根据线程标识符为其分配索引。我们需要知道这个图的分辨率。因此，添加一个 *_Resolution* 着色器属性，其 `uint` 类型与标识符的类型相匹配。

```glsl
RWStructuredBuffer<float3> _Positions;

uint _Resolution;

float _Step;
```

然后创建一个 `SetPosition` 函数，在给定标识符和要设置的位置的情况下设置位置。对于索引，我们将使用标识符的 X 分量加上它的 Y 分量乘以图形分辨率。通过这种方式，我们将 2D 数据顺序存储在 1D 数组中。

```glsl
float2 GetUV (uint3 id) {
	return (id.xy + 0.5) * _Step - 1.0;
}

void SetPosition (uint3 id, float3 position) {
	_Positions[id.x + id.y * _Resolution] = position;
}
```

![img](https://catlikecoding.com/unity/tutorials/basics/compute-shaders/moving-work-to-the-gpu/3x3-grid-indices.png)

*3×3 网格的位置索引。*

我们必须意识到的一件事是，我们每个小组都计算一个 8×8 点的网格。如果图的分辨率不是 8 的倍数，那么我们最终将得到一行一列的组，这些组将计算出一些超出界限的点。这些点的索引要么落在缓冲区之外，要么与有效索引冲突，这会破坏我们的数据。

![img](https://catlikecoding.com/unity/tutorials/basics/compute-shaders/moving-work-to-the-gpu/3x3-grid-indices-out-of-bounds.png)

*走出界限。*

只有当 X 和 Y 标识符分量都小于分辨率时，才能通过存储无效位置来避免。

```glsl
void SetPosition (uint3 id, float3 position) {
	if (id.x < _Resolution && id.y < _Resolution) {
		_Positions[id.x + id.y * _Resolution] = position;
	}
}
```

### 1.8 波浪函数

现在，我们可以在 `FunctionKernel` 中获取 UV 坐标，并使用我们创建的函数设置位置。首先使用零作为位置。

```glsl
[numthreads(8, 8, 1)]
void FunctionKernel (uint3 id: SV_DispatchThreadID) {
	float2 uv = GetUV(id);
	SetPosition(id, 0.0);
}
```

我们最初只支持 *Wave* 函数，这是我们库中最简单的函数。为了使其具有动画效果，我们需要知道时间，因此添加一个 *_Time* 属性。

```glsl
float _Step, _Time;
```

然后从 `FunctionLibrary` 类复制 `Wave` 方法，将其插入 `FunctionKernel` 上方。要将其转换为 HLSL 函数，请删除 `public static` 限定符，将 `Vector3` 替换为 `float3`，将 `Sin` 替换为 `sin`。

```glsl
float3 Wave (float u, float v, float t) {
	float3 p;
	p.x = u;
	p.y = sin(PI * (u + v + t));
	p.z = v;
	return p;
}
```

唯一缺少的是 `PI` 的定义。我们将通过为它定义一个宏来添加它。这是通过编写 `#define PI` 后跟数字来完成的，为此我们将使用 `3.14159265358979323846`。这比 `float` 值所能表示的要精确得多，但我们让着色器编译器使用适当的近似值。

```glsl
#define PI 3.14159265358979323846

float3 Wave (float u, float v, float t) { … }
```

现在使用 `Wave` 函数计算 `FunctionKernel` 中的位置，而不是使用零。

```glsl
void FunctionKernel (uint3 id: SV_DispatchThreadID) {
	float2 uv = GetUV(id);
	SetPosition(id, Wave(uv.x, uv.y, _Time));
}
```

### 1.9 分派计算着色器内核

我们有一个核函数，用于计算和存储图中点的位置。下一步是在 GPU 上运行它。`GPUGraph` 需要访问计算着色器才能做到这一点，因此向其中添加一个可序列化的 `ComputeShader` 字段，然后将我们的资源连接到组件。

```c#
	[SerializeField]
	ComputeShader computeShader;
```

![img](https://catlikecoding.com/unity/tutorials/basics/compute-shaders/moving-work-to-the-gpu/compute-shader-assigned.png)

*计算指定的着色器。*

我们需要设置计算着色器的一些属性。为此，我们需要知道 Unity 为它们使用的标识符。这些是可以通过调用带有名称字符串的 `Shader.PropertyToID` 检索的整数。这些标识符是按需声明的，在应用程序或编辑器运行时保持不变，因此我们可以直接将标识符存储在静态字段中。从 *_Positions* 属性开始。

```c#
	static int positionsId = Shader.PropertyToID("_Positions");
```

我们永远不会改变这些字段，我们可以通过向它们添加 `readonly` 限定符来表示。除了明确字段的意图外，这还会指示编译器在我们将某些内容分配给其他地方时产生错误。

```c#
	static readonly int positionsId = Shader.PropertyToID("_Positions");
```

> **我们难道不应该将 `FunctionLibrary.functions` 也标记为 `readonly` 吗？**
>
> 虽然这是有道理的，但 `readonly` 对于引用类型来说效果不佳，因为它只强制字段值本身不被更改。对象（在本例中为数组）本身仍然可以修改。因此，它将阻止分配一个完全不同的数组，但不会阻止更改其元素。我更喜欢只对 `int` 等原始类型使用 `readonly`。

还存储 *_Resolution*、*_Step* 和 *_Time* 的标识符。

```c#
	static readonly int
		positionsId = Shader.PropertyToID("_Positions"),
		resolutionId = Shader.PropertyToID("_Resolution"),
		stepId = Shader.PropertyToID("_Step"),
		timeId = Shader.PropertyToID("_Time");
```

接下来，创建一个 `UpdateFunctionOnGPU` 方法，该方法计算步长并设置计算着色器的分辨率、步长和时间属性。这是通过调用 `SetInt` 进行解析，调用 `SetFloat` 进行其他两个属性的解析，并将标识符和值作为参数来实现的。

```c#
	void UpdateFunctionOnGPU () {
		float step = 2f / resolution;
		computeShader.SetInt(resolutionId, resolution);
		computeShader.SetFloat(stepId, step);
		computeShader.SetFloat(timeId, Time.time);
	}
```

> **着色器的分辨率属性不是 `uint` 吗？**
>
> 是的，但只有一种方法可以设置正则整数，而不是无符号整数。这很好，因为正的 `int` 值相当于 `uint` 值。

我们还必须设置位置缓冲区，它不复制任何数据，而是将缓冲区链接到内核。这是通过调用 `SetBuffer` 来实现的，它的工作原理与其他方法类似，除了它需要一个额外的参数。它的第一个参数是内核函数的索引，因为计算着色器可以包含多个内核，缓冲区可以链接到特定的内核。我们可以通过在计算着色器上调用 `FindKernel` 来获取内核索引，但我们的单个内核的索引始终为零，因此我们可以直接使用该值。

```c#
		computeShader.SetFloat(timeId, Time.time);
		
		computeShader.SetBuffer(0, positionsId, positionsBuffer);
```

设置缓冲区后，我们可以通过使用四个整数参数在计算着色器上调用 `Dispatch` 来运行内核。第一个是内核索引，另外三个是要运行的组的数量，也是按维度划分的。对所有维度使用 1 意味着只计算第一组 8×8 的位置。

```c#
		computeShader.SetBuffer(0, positionsId, positionsBuffer);
		
		computeShader.Dispatch(0, 1, 1, 1);
```

由于我们固定的 8×8 组大小，我们在 X 和 Y 维度上需要的组数量等于分辨率除以 8，四舍五入。我们可以通过执行 `float` 除法并将结果传递给 `Mathf.CeilToInt` 来实现这一点。

```c#
		int groups = Mathf.CeilToInt(resolution / 8f);
		computeShader.Dispatch(0, groups, groups, 1);
```

要最终运行我们的内核，请在 `Update` 结束时调用 `UpdateFunctionOnGPU`。

```c#
	void Update () {
		…

		UpdateFunctionOnGPU();
	}
```

现在，我们在播放模式下每帧计算所有图形的位置，即使我们没有注意到这一点，也没有对数据做任何处理。

## 2 程序化绘制

利用 GPU 上可用的位置，下一步是绘制点，而无需将任何变换矩阵从 CPU 发送到 GPU。因此，着色器必须从缓冲区中检索正确的位置，而不是依赖于标准矩阵。

### 2.1 绘制许多网格

因为 GPU 上已经存在这些位置，所以我们不需要在 CPU 端跟踪它们。我们甚至不需要他们的游戏对象。相反，我们将通过一个命令指示 GPU 多次使用特定材质绘制特定网格。要配置要绘制的内容，请在 `GPUGraph` 中添加可序列化的 `Material` 和 `Mesh` 字段。我们最初将使用我们现有的*点曲面*材质，这些材质已经用于使用BRP绘制点。对于网格，我们将使用默认立方体。

```c#
	[SerializeField]
	Material material;

	[SerializeField]
	Mesh mesh;
```

![img](https://catlikecoding.com/unity/tutorials/basics/compute-shaders/procedural-drawing/material-mesh.png)

*材质和网格配置。*

程序绘图是通过调用 `Graphics.DrawMeshInstancedProcedural` 完成的，以网格、子网格索引和材质作为参数。子网格索引适用于网格由多个部分组成的情况，但对我们来说并非如此，因此我们使用索引零。在 `UpdateFunctionOnGPU` 结束时执行此操作。

```c#
	void UpdateFunctionOnGPU () {
		…

		Graphics.DrawMeshInstancedProcedural(mesh, 0, material);
	}
```

> **我们不应该使用 `DrawMeshInstancedIndirect` 吗？**
>
> 当您不知道在 CPU 端绘制多少个实例，而是通过缓冲区用计算着色器提供这些信息时，`DrawMeshInstancedIndirect` 方法非常有用。

因为这种绘图方式不使用游戏对象，Unity 不知道绘图发生在场景中的何处。我们必须通过提供一个边界框作为额外的参数来表明这一点。这是一个轴对齐的框，指示我们正在绘制的任何内容的空间边界。Unity 使用此功能来确定是否可以跳过绘图，因为它最终会出现在相机的视野之外。这被称为截锥剔除。因此，现在不是计算每个点的边界，而是一次计算整个图的边界。这对我们的图表来说很好，因为我们的想法是从整体上看待它。

我们的图位于原点，点应保持在大小为 2 的立方体内。我们可以通过调用 `Bounds` 构造函数方法，将 `Vector3.zero` 和 `Vector3.one` 按 2 缩放作为参数，为其创建一个边界值。

```c#
		var bounds = new Bounds(Vector3.zero, Vector3.one * 2f);
		Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds);
```

但点也有大小，其中一半可能会在所有方向上超出界限。因此，我们也应该相应地提高界限。

```c#
		var bounds = new Bounds(Vector3.zero, Vector3.one * (2f + 2f / resolution));
```

我们必须向 `DrawMeshInstancedProcedural` 提供的最后一个参数是应该绘制多少个实例。这应该与位置缓冲区中的元素数量相匹配，我们可以通过其 `count` 属性检索。

```c#
		Graphics.DrawMeshInstancedProcedural(
			mesh, 0, material, bounds, positionsBuffer.count
		);
```

![img](https://catlikecoding.com/unity/tutorials/basics/compute-shaders/procedural-drawing/unit-cubes.png)

*重叠的单位立方体。*

> **为什么现在进入游戏模式会完全冻结 Unity 或不显示立方体？**
>
> 此时的行为取决于 Unity 版本。继续，一旦一切设置正确，着色器使用了位置，它应该会工作。

当进入游戏模式时，我们现在会看到一个单色的立方体位于原点。每个点渲染一次相同的立方体，但使用单位变换矩阵，因此它们都重叠。性能比以前好得多，因为几乎不需要将数据复制到 GPU，所有点都是通过一次绘图调用绘制的。此外，Unity 不必对每个点进行任何筛选。它也不会根据点的视图空间深度对点进行排序，通常会先绘制离相机最近的点。深度排序使不透明几何体的渲染更加高效，因为它避免了过度绘制，但我们的程序化绘制命令只是一个接一个地渲染点。然而，消除的 CPU 工作和数据传输，加上 GPU 全速渲染所有立方体的能力，远远弥补了这一点。

### 2.2 检索位置

要检索我们存储在 GPU 上的点位置，我们必须创建一个新的着色器，最初用于 BRP。复制*点曲面*着色器并将其重命名为*点曲面 GPU*。调整其着色器菜单标签以匹配。此外，由于我们现在依赖于由计算着色器填充的结构化缓冲区，因此将着色器的目标级别提高到 4.5。这不是严格需要的，但表明我们需要计算着色器支持。

```glsl
Shader "Graph/Point Surface GPU" {

	Properties {
		_Smoothness ("Smoothness", Range(0,1)) = 0.5
	}
	
	SubShader {
		CGPROGRAM
		#pragma surface ConfigureSurface Standard fullforwardshadows
		#pragma target 4.5

		…
		ENDCG
	}

	FallBack "Diffuse"
}
```

> **目标水平 4.5 是什么意思？**
>
> [这表明我们至少需要 OpenGL ES 3.1 的功能](https://docs.unity3d.com/Manual/SL-ShaderCompileTargets.html)。它不适用于 DX11 之前的旧 GPU，也不适用于 OpenGL ES 2.0 或 3.0。这也不包括 WebGL。WebGL 2.0 有一些实验性的计算着色器支持，但 Unity 目前不支持它。
>
> 当支持不足时运行 GPU 图最多只会导致所有点重叠，就像现在发生的那样。因此，如果你针对这样的平台，你就必须坚持旧的方法，或者将两者都包括在内，并在需要时以更低的分辨率退回 CPU 图。

过程化渲染的工作方式类似于 GPU 实例化，但我们需要指定一个额外的选项，通过添加 `#pragma instancing_options` 指令来指示。在这种情况下，我们必须遵循 `procedural:ConfigureProcedural` 选项。

```glsl
		#pragma surface ConfigureSurface Standard fullforwardshadows
		#pragma instancing_options procedural:ConfigureProcedural
```

这表示曲面着色器需要为每个顶点调用 `ConfigureProcedural` 函数。这是一个没有任何参数的 `void` 函数。将其添加到我们的着色器中。

```glsl
		void ConfigureProcedural () {}

		void ConfigureSurface (Input input, inout SurfaceOutputStandard surface) {
			surface.Albedo = saturate(input.worldPos * 0.5 + 0.5);
			surface.Smoothness = _Smoothness;
		}
```

默认情况下，此函数仅在常规绘制过程中被调用。要在渲染阴影时应用它，我们必须通过在 `#pragma surface` 指令中添加 `addshadow` 来指示我们需要一个自定义阴影过程。

```glsl
		#pragma surface ConfigureSurface Standard fullforwardshadows addshadow
```

现在添加我们在计算着色器中声明的相同位置缓冲区字段。这次我们只从中读取，因此给它 `StructuredBuffer` 类型，而不是 `RWStructuredBBuffer`。

```glsl
		StructuredBuffer<float3> _Positions;

		void ConfigureProcedural () {}
```

但我们应该只对专门为程序绘图编译的着色器变体进行此操作。定义 `UNITY_PROCEDURAL_INSTANCING_ENABLED` 宏标签时就是这种情况。我们可以通过写 `#if define(UNITY_PROCEDURAL_INSTANCING_ENABLED)` 来检查这一点。这是一个预处理器指令，指示编译器在定义了标签的情况下仅包含以下行中的代码。这适用于只包含 `#endif` 指令的行。它的工作方式类似于 C# 中的条件块，除了在编译过程中包含或省略代码。最终代码中不存在分支。

```glsl
		#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
			StructuredBuffer<float3> _Positions;
		#endif
```

我们必须对将放入 `ConfigureProcedural` 函数中的代码执行相同的操作。

```glsl
		void ConfigureProcedural () {
			#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
			#endif
		}
```

现在，我们可以通过用当前正在绘制的实例的标识符对位置缓冲区进行索引来检索点的位置。我们可以通过全局可访问的 `unity_InstanceID` 访问其标识符。

```glsl
		void ConfigureProcedural () {
			#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
				float3 position = _Positions[unity_InstanceID];
			#endif
		}
```

### 2.3 创建转换矩阵

一旦我们有了位置，下一步就是为该点创建一个对象到世界的转换矩阵。为了使事情尽可能简单，我们将图固定在世界原点，不进行任何旋转或缩放。调整 *GPU Graph* 游戏对象的 `Transform` 组件不会产生任何效果，因为我们不会将其用于任何用途。

我们只需要应用点的位置和规模。位置存储在 4×4 变换矩阵的最后一列，而比例存储在矩阵对角线中。矩阵的最后一个分量始终设置为 1。所有其他组件对我们来说都是零。

![img](https://catlikecoding.com/unity/tutorials/basics/compute-shaders/procedural-drawing/transformation-matrix.png)

*带位置和比例的变换矩阵。*

变换矩阵用于将顶点从对象空间转换到世界空间。它通过 `unity_ObjectToWorld` 在全球范围内提供。因为我们是按程序绘制的，所以它是一个单位矩阵，所以我们必须替换它。首先将整个矩阵设置为零。

```glsl
				float3 position = _Positions[unity_InstanceID];

				unity_ObjectToWorld = 0.0;
```

我们可以通过 `float4(position, 1.0)` 为位置偏移量构造一个列向量。我们可以通过将其分配给 `unity_ObjectToWorld._m03_m13_m23_m33` 将其设置为第四列。

```glsl
				unity_ObjectToWorld = 0.0;
				unity_ObjectToWorld._m03_m13_m23_m33 = float4(position, 1.0);
```

然后将 `float _Step` 着色器属性添加到我们的着色器中，并将其分配给 `unity_ObjectToWorld._m00_m11_m22`。这正确地表达了我们的观点。

```glsl
		float _Step;

		void ConfigureProcedural () {
			#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
				float3 position = _Positions[unity_InstanceID];

				unity_ObjectToWorld = 0.0;
				unity_ObjectToWorld._m03_m13_m23_m33 = float4(position, 1.0);
				unity_ObjectToWorld._m00_m11_m22 = _Step;
			#endif
		}
```

还有一个 `unity_WorldToObject` 矩阵，其中包含用于变换法向量的逆变换。当施加非均匀变形时，需要正确地变换方向矢量。但是，由于这不适用于我们的图，我们可以忽略它。不过，我们应该通过在实例化选项 pragma 中添加 `assumeuniformscaling` 来告诉着色器。

```glsl
		#pragma instancing_options assumeuniformscaling procedural:ConfigureProcedural
```

现在创建一个使用此着色器的新材质，启用 GPU 实例化，并将其分配给我们的 GPU 图。

![img](https://catlikecoding.com/unity/tutorials/basics/compute-shaders/procedural-drawing/using-point-surface-gpu-material.png)

*使用 GPU 材质。*

为了使其正确工作，我们必须像之前设置计算着色器一样设置材质的属性。在绘图之前，在 `UpdateFunctionOnGPU` 中对材质调用 `SetBuffer` 和 `SetFloat`。在这种情况下，我们不必为缓冲区提供内核索引。

```c#
		material.SetBuffer(positionsId, positionsBuffer);
		material.SetFloat(stepId, step);
		var bounds = new Bounds(Vector3.zero, Vector3.one * (2f + 2f / resolution));
		Graphics.DrawMeshInstancedProcedural(
			mesh, 0, material, bounds, positionsBuffer.count
		);
```

*40000 个阴影立方体，用 BRP 绘制。*

当我们进入游戏模式时，我们再次看到了我们的图表，但现在它的 40000 点以稳定的 60FPS 渲染。如果我关闭编辑器游戏窗口的 VSync，它的帧率会高达 245FPS。我们的程序方法显然比每点使用一个游戏对象快得多。

![img](https://catlikecoding.com/unity/tutorials/basics/compute-shaders/procedural-drawing/profiler-drp-vsync.png)

*使用 VSync 分析 BRP 构建。*

评测构建表明，我们的 `GPUGraph` 组件几乎没有任何作用。它只指示 GPU 运行计算着色器内核，然后告诉 Unity 按程序绘制大量点。这不会立即发生。计算着色器已安排，一旦 GPU 空闲，它将立即运行。程序绘制命令稍后由 BRP 发送到 GPU。该命令发送三次，一次用于仅深度通道，一次为阴影，一次是最终绘制。GPU 将首先运行计算着色器，只有在完成后才能绘制场景，之后才能运行计算着色器的下一次调用。Unity 可以轻松获得 40000 点。

### 2.4 追求一百万

由于它可以很好地处理 40000 个点，让我们看看我们的 GPU 图是否可以处理一百万个点。但在此之前，我们必须了解异步着色器编译。这是 Unity 编辑器的一个功能，而不是构建。编辑器只在需要时编译着色器，而不是提前编译。这可以在编辑着色器时节省大量编译时间，但意味着着色器并不总是立即可用。当这种情况发生时，会临时使用统一的青色虚拟着色器，直到着色器编译过程完成，该过程并行运行。这通常很好，但虚拟着色器不适用于程序绘制。这将大大减缓绘图过程。如果在尝试渲染一百万个点时发生这种情况，它很可能会冻结，然后导致 Unity 崩溃，甚至可能导致整个机器崩溃。

我们可以通过项目设置关闭异步着色器编译，但这对我们的*点表面 GPU* 着色器来说只是一个问题。幸运的是，我们可以通过向 Unity 添加 `#pragma editor_sync_compilation` 指令来告诉 Unity 对特定着色器使用同步编译。这将迫使 Unity 在第一次使用着色器之前立即停止并编译着色器，避免使用虚拟着色器。

```glsl
		#pragma surface ConfigureSurface Standard fullforwardshadows addshadow
		#pragma instancing_options assumeuniformscaling procedural:ConfigureProcedural
		#pragma editor_sync_compilation
		#pragma target 4.5
```

现在，将 `GPUGraph` 的分辨率限制提高到 1000 是安全的。

```c#
	[SerializeField, Range(10, 1000)]
	int resolution = 10;
```

让我们试试最大分辨率。

![inspector](https://catlikecoding.com/unity/tutorials/basics/compute-shaders/procedural-drawing/resolution-1000-inspector.png)
![scene](https://catlikecoding.com/unity/tutorials/basics/compute-shaders/procedural-drawing/resolution-1000-scene.png)

*分辨率设置为 1000。*

当在小窗口中查看时，它看起来并不漂亮——莫尔图案会出现，因为点太小了——但它会运行。对我来说，一百万个动画点以 24FPS 的速度渲染。编辑器和构建中的性能是相同的。此时编辑器开销微不足道，GPU 是瓶颈。此外，在我的情况下，是否启用 VSync 并没有明显的区别。

![img](https://catlikecoding.com/unity/tutorials/basics/compute-shaders/procedural-drawing/profiler-drp-million-no-vsync.png)

*分析一个构建渲染一百万点，没有 VSync。*

当 VSync 被禁用时，很明显播放器循环的大部分时间都花在等待 GPU 完成上。GPU 确实是瓶颈。我们可以在不影响性能的情况下向 CPU 添加相当多的工作负载。

请注意，我们正在渲染一百万个带有阴影的点，这要求 BRP 每帧绘制三次。禁用阴影会使我的平均帧速率在没有 VSync 的情况下提高到 65FPS 左右。

当然，如果你发现帧率不够，你不需要把分辨率一直提高到 1000。将其减少到 700 可能已经使其在启用阴影的情况下以 60FPS 运行，并且看起来基本相同。但从现在开始，我将始终如一地使用分辨率 1000。

### 2.5 URP

为了了解 URP 的性能，我们还需要复制我们的 *Point URP* 着色器图，将其重命名为 *Point URP GPU*。着色器图不直接支持程序化绘图，但我们可以使用一些自定义代码使其工作。为了简化这一过程并重用一些代码，我们将创建一个 HLSL 包含文件资产。Unity 没有此菜单选项，因此只需复制一个表面着色器资源并将其重命名为 *PointGPU*。然后使用系统的文件浏览器将资源的文件扩展名从 *shader* 更改为 *hlsl*。

![img](https://catlikecoding.com/unity/tutorials/basics/compute-shaders/procedural-drawing/point-gpu-hlsl-asset.png)

*PointGPU HLSL 脚本资源。*

清除文件内容，然后将位置缓冲区、比例和 `ConfigureProcedural` 函数的代码从 *Points Surface GPU* 复制到文件中。

```glsl
#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
	StructuredBuffer<float3> _Positions;
#endif

float _Step;

void ConfigureProcedural () {
	#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
		float3 position = _Positions[unity_InstanceID];

		unity_ObjectToWorld = 0.0;
		unity_ObjectToWorld._m03_m13_m23_m33 = float4(position, 1.0);
		unity_ObjectToWorld._m00_m11_m22 = _Step;
	#endif
}
```

我们现在可以通过 `#include "PointGPU.hlsl"` 指令将此文件包含在*点表面 GPU* 着色器中，之后可以从中删除原始代码。

```glsl
		#include "PointGPU.hlsl"

		struct Input {
			float3 worldPos;
		};

		float _Smoothness;

		//#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
		//	StructuredBuffer<float3> _Positions;
		//#endif

		//float _Step;

		//void ConfigureProcedural () { … }
		
		void ConfigureSurface (Input input, inout SurfaceOutputStandard surface) { … }
```

> **我们可以在 `CGPROGRAM` 着色器中包含 HLSL 文件吗？**
>
> 对。`CGPROGRAM` 块和 `HLSLPROGRAM` 块之间的唯一区别是，前者默认包含一些文件。这种差异与我们无关。

我们将使用*自定义函数*节点将 HLSL 文件包含在着色器图中。这个想法是节点从文件中调用一个函数。虽然我们不需要这个功能，但除非我们将其连接到我们的图上，否则代码不会被包含在内。因此，我们将向 *PointGPU* 添加一个格式正确的虚拟函数，该函数只需传递 `float3` 值而不进行更改。

使用两个名为 `In` 和 `Out` 的 `float3` 参数向 *PointGPU* 添加一个 `void ShaderGraphFunction_float` 函数。该函数只是将输入分配给输出。参数名称按照惯例大写，因为它们将对应于着色器图中使用的输入和输出标签。

```glsl
void ShaderGraphFunction_float (float3 In, float3 Out) {
	Out = In;
}
```

这假设 `Out` 参数是一个输出参数，我们必须在它前面写 `out` 声明它。

```glsl
void ShaderGraphFunction_float (float3 In, out float3 Out) {
	Out = In;
}
```

函数名的 `_float` 后缀是必需的，因为它表示函数的精度。着色器图提供两种精度模式，`float` 或 `half`。后者的大小是前者的一半，因此是两个字节而不是四个字节。节点使用的精度可以显式选择或设置为继承，这是默认值。为了确保我们的图适用于两种精度模式，还添加了一个使用半精度的变体函数。

```glsl
void ShaderGraphFunction_float (float3 In, out float3 Out) {
	Out = In;
}

void ShaderGraphFunction_half (half3 In, out half3 Out) {
	Out = In;
}
```

现在将*自定义函数*节点添加到我们的*点 URP GPU*图中。默认情况下，其类型设置为“*文件*”。将 *PointGPU* 指定给其 *Source* 属性。使用 *ShaderGraphFunction* 作为其名称，不带精度后缀。然后将 *In* 添加到 *Inputs* 列表，将 *Out* 添加到 *Outputs* 列表，两者都作为 *Vector3*。

![img](https://catlikecoding.com/unity/tutorials/basics/compute-shaders/procedural-drawing/custom-function-file.png)

*通过文件自定义功能。*

为了将我们的代码集成到图中，我们必须将节点连接到它。因为顶点阶段需要将其输出连接到*顶点*节点的*位置*。然后将“*位置*”节点集添加到对象空间，并将其连接到自定义节点的输入。

![img](https://catlikecoding.com/unity/tutorials/basics/compute-shaders/procedural-drawing/vertex-position.png)

*通过我们的函数传递的对象空间顶点位置。*

现在，对象空间顶点位置通过我们的虚拟函数传递，我们的代码被包含在生成的着色器中。但是，为了启用过程渲染，我们还必须包含 `#pragma instancing_options` 和 `#pragma editor_sync_copilation` 编译器指令。这些必须直接注入生成的着色器源代码中，不能通过单独的文件包含。因此，添加另一个*自定义函数*节点，其输入和输出与之前相同，但这次其*类型*设置为*字符串*。将其名称设置为适当的值，如 *InjectPragmas*，然后将指令放入*正文*文本块中。主体充当函数的代码块，因此我们还必须在这里将输入分配给输出。

![img](https://catlikecoding.com/unity/tutorials/basics/compute-shaders/procedural-drawing/custom-function-string.png)

*通过字符串注入语法自定义函数。*

为清楚起见，这是主体的代码：

```glsl
#pragma instancing_options assumeuniformscaling procedural:ConfigureProcedural
#pragma editor_sync_compilation

Out = In;
```

也可以在其他自定义函数节点之前或之后通过此节点传递顶点位置。

![img](https://catlikecoding.com/unity/tutorials/basics/compute-shaders/procedural-drawing/with-pragmas.png)

*带 pragmas 的着色器图。*

创建一个启用实例化的材质，该材质使用 *Point URP GPU* 着色器，将其分配给我们的图形，然后进入播放模式。我现在在编辑器和构建中都获得了 36FPS 的帧率，并启用了阴影。这比 BRP 快 50%。

![img](https://catlikecoding.com/unity/tutorials/basics/compute-shaders/procedural-drawing/profiler-urp-million-no-vsync.png)

*分析 URP 构建。*

同样，VSync 对平均帧速率没有影响。禁用阴影会将其增加到 69FPS，这与 BRP 大致相同，玩家循环只需要更少的时间。

### 2.6 可变分辨率

因为我们目前总是为缓冲区中的每个位置绘制一个点，在播放模式下降低分辨率会固定一些点。这是因为计算着色器只更新适合图形的点。

![img](https://catlikecoding.com/unity/tutorials/basics/compute-shaders/procedural-drawing/stuck-points.png)

*降低分辨率后卡住点。*

无法调整计算缓冲区的大小。每次更改分辨率时，我们都可以创建一个新的缓冲区，但另一种更简单的方法是始终为最大分辨率分配一个缓冲区。这将使在游戏模式下轻松更改分辨率。

首先将最大分辨率定义为常数，然后在 `resolution` 字段的 `Range` 属性中使用它。

```c#
	const int maxResolution = 1000;

	…

	[SerializeField, Range(10, maxResolution)]
	int resolution = 10;
```

接下来，始终使用最大分辨率的平方作为缓冲区元素的数量。这意味着，无论图形分辨率如何，我们都会要求 12MB（大约 11.44MiB）的 GPU 内存。

```c#
	void OnEnable () {
		positionsBuffer = new ComputeBuffer(maxResolution * maxResolution, 3 * 4);
	}
```

最后，绘图时使用当前分辨率的平方，而不是缓冲区元素计数。

```c#
	void UpdateFunctionOnGPU () {
		…
		Graphics.DrawMeshInstancedProcedural(
			mesh, 0, material, bounds, resolution * resolution
		);
	}
```

*分辨率在 10 到 1000 之间变化。*

## 3 GPU 函数库

现在我们基于 GPU 的方法功能齐全，让我们将整个函数库移植到我们的计算着色器。

### 3.1 所有函数

我们可以复制其他函数，就像我们复制和调整 `Wave` 一样。第二个是 `MultiWave`。与 `Wave` 的唯一显著区别是它包含 `float` 值。HLSL 中不存在 f 后缀，因此应从所有数字中删除。为了表明它们都是浮点值，我为它们显式地添加了一个点，例如 `2f` 变为 `2.0`。

```glsl
float3 MultiWave (float u, float v, float t) {
	float3 p;
	p.x = u;
	p.y = sin(PI * (u + 0.5 * t));
	p.y += 0.5 * sin(2.0 * PI * (v + t));
	p.y += sin(PI * (u + v + 0.25 * t));
	p.y *= 1.0 / 2.5;
	p.z = v;
	return p;
}
```

对其余功能执行相同的操作。`Sqrt` 变为 `sqrt`，`Cos` 变为 `cos`。

```glsl
float3 Ripple (float u, float v, float t) {
	float d = sqrt(u * u + v * v);
	float3 p;
	p.x = u;
	p.y = sin(PI * (4.0 * d - t));
	p.y /= 1.0 + 10.0 * d;
	p.z = v;
	return p;
}

float3 Sphere (float u, float v, float t) {
	float r = 0.9 + 0.1 * sin(PI * (6.0 * u + 4.0 * v + t));
	float s = r * cos(0.5 * PI * v);
	float3 p;
	p.x = s * sin(PI * u);
	p.y = r * sin(0.5 * PI * v);
	p.z = s * cos(PI * u);
	return p;
}

float3 Torus (float u, float v, float t) {
	float r1 = 0.7 + 0.1 * sin(PI * (6.0 * u + 0.5 * t));
	float r2 = 0.15 + 0.05 * sin(PI * (8.0 * u + 4.0 * v + 2.0 * t));
	float s = r2 * cos(PI * v) + r1;
	float3 p;
	p.x = s * sin(PI * u);
	p.y = r2 * sin(PI * v);
	p.z = s * cos(PI * u);
	return p;
}
```

### 3.2 宏

我们现在必须为每个图函数创建一个单独的内核函数，但这需要大量的重复代码。我们可以通过创建着色器宏来避免这种情况，就像我们之前定义的 `PI` 一样。首先在 `FunctionKernel` 函数上方的行上写 `#define KERNEL_FUNCTION`。

```glsl
#define KERNEL_FUNCTION
	[numthreads(8, 8, 1)]
	void FunctionKernel (uint3 id: SV_DispatchThreadID) { … }
```

这些定义通常只适用于同一行后面写的任何内容，但我们可以通过在除最后一行之外的每一行末尾添加一个 `\` 反斜杠将其扩展到多行。

```glsl
#define KERNEL_FUNCTION \
	[numthreads(8, 8, 1)] \
	void FunctionKernel (uint3 id: SV_DispatchThreadID) { \
		float2 uv = GetUV(id); \
		SetPosition(id, Wave(uv.x, uv.y, _Time)); \
	}
```

现在，当我们编写 `KERNEL_FUNCTION` 时，编译器将用 `FunctionKernel` 函数的代码替换它。为了使其适用于任意函数，我们在宏中添加了一个参数。这类似于函数的参数列表，但没有类型，并且必须将左括号附加到宏名称上。给它一个 `function` 参数，并使用它来代替 `Wave` 的显式调用。

```glsl
#define KERNEL_FUNCTION(function) \
	[numthreads(8, 8, 1)] \
	void FunctionKernel (uint3 id: SV_DispatchThreadID) { \
		float2 uv = GetUV(id); \
		SetPosition(id, function(uv.x, uv.y, _Time)); \
	}
```

我们还必须更改内核函数的名称。我们将使用 `function` 参数作为前缀，后面跟着 `Kernel`。不过，我们必须将 `function` 标签分开，否则它将不会被识别为着色器参数。要组合这两个单词，请使用 `##` 宏连接运算符将它们连接起来。

```glsl
	void function##Kernel (uint3 id: SV_DispatchThreadID) { \
```

现在，通过编写带有适当参数的 `KERNEL_FUNCTION`，可以定义所有五个内核函数。

```glsl
#define KERNEL_FUNCTION(function) \
	…

KERNEL_FUNCTION(Wave)
KERNEL_FUNCTION(MultiWave)
KERNEL_FUNCTION(Ripple)
KERNEL_FUNCTION(Sphere)
KERNEL_FUNCTION(Torus)
```

我们还必须按照与 `FunctionLibrary.FunctionName` 匹配的顺序，为每个函数替换一个内核指令。

```glsl
#pragma kernel WaveKernel
#pragma kernel MultiWaveKernel
#pragma kernel RippleKernel
#pragma kernel SphereKernel
#pragma kernel TorusKernel
```

最后一步是使用当前函数作为 `GPUGraph.UpdateFunctionOnGPU` 中的内核索引，而不是总是使用零。

```c#
		var kernelIndex = (int)function;
		computeShader.SetBuffer(kernelIndex, positionsId, positionsBuffer);
		
		int groups = Mathf.CeilToInt(resolution / 8f);
		computeShader.Dispatch(kernelIndex, groups, groups, 1);
```

*所有功能分辨率为 1000，平面显示阴影。*

计算着色器运行得如此之快，以至于显示哪个函数都无关紧要，所有函数的帧速率都是相同的。

### 3.3 变形函数

支持从一个函数到另一个函数的变形有点复杂，因为我们需要为每个独特的转换提供一个单独的内核。首先，将过渡进度的属性添加到计算着色器中，我们将使用它来混合函数。

```glsl
float _Step, _Time, _TransitionProgress;
```

然后复制内核宏，将其重命名为 `KERNEL_MORPH_FUNCTION`，并为其提供两个参数：`functionA` 和 `functionB`。将函数的名称更改为 `functionA##to##functionB##Kernel`，并使用 `lerp` 在它们根据进度计算的位置之间进行线性插值。我们也可以在这里使用 `smoothstep`，但我们只会在 CPU 上每帧计算一次。

```glsl
#define KERNEL_MORPH_FUNCTION(functionA, functionB) \
	[numthreads(8, 8, 1)] \
	void functionA##To##functionB##Kernel (uint3 id: SV_DispatchThreadID) { \
		float2 uv = GetUV(id); \
		float3 position = lerp( \
			functionA(uv.x, uv.y, _Time), functionB(uv.x, uv.y, _Time), \
			_TransitionProgress \
		); \
		SetPosition(id, position); \
	}
```

每个函数都可以转换为所有其他函数，因此每个函数有四个转换。为所有这些添加内核函数。

```glsl
KERNEL_FUNCTION(Wave)
KERNEL_FUNCTION(MultiWave)
KERNEL_FUNCTION(Ripple)
KERNEL_FUNCTION(Sphere)
KERNEL_FUNCTION(Torus)

KERNEL_MORPH_FUNCTION(Wave, MultiWave);
KERNEL_MORPH_FUNCTION(Wave, Ripple);
KERNEL_MORPH_FUNCTION(Wave, Sphere);
KERNEL_MORPH_FUNCTION(Wave, Torus);

KERNEL_MORPH_FUNCTION(MultiWave, Wave);
KERNEL_MORPH_FUNCTION(MultiWave, Ripple);
KERNEL_MORPH_FUNCTION(MultiWave, Sphere);
KERNEL_MORPH_FUNCTION(MultiWave, Torus);

KERNEL_MORPH_FUNCTION(Ripple, Wave);
KERNEL_MORPH_FUNCTION(Ripple, MultiWave);
KERNEL_MORPH_FUNCTION(Ripple, Sphere);
KERNEL_MORPH_FUNCTION(Ripple, Torus);

KERNEL_MORPH_FUNCTION(Sphere, Wave);
KERNEL_MORPH_FUNCTION(Sphere, MultiWave);
KERNEL_MORPH_FUNCTION(Sphere, Ripple);
KERNEL_MORPH_FUNCTION(Sphere, Torus);

KERNEL_MORPH_FUNCTION(Torus, Wave);
KERNEL_MORPH_FUNCTION(Torus, MultiWave);
KERNEL_MORPH_FUNCTION(Torus, Ripple);
KERNEL_MORPH_FUNCTION(Torus, Sphere);
```

我们将定义内核，使其索引等于 `functionB + functionA * 5`，将不转换的内核视为在同一函数之间转换。因此，第一个内核是 *Wave*，接下来是从 *Wave* 转换到其他函数的四个内核。之后是从 *MultiWave* 开始的函数，其中第二个是非转换内核，以此类推。

```glsl
#pragma kernel WaveKernel
#pragma kernel WaveToMultiWaveKernel
#pragma kernel WaveToRippleKernel
#pragma kernel WaveToSphereKernel
#pragma kernel WaveToTorusKernel

#pragma kernel MultiWaveToWaveKernel
#pragma kernel MultiWaveKernel
#pragma kernel MultiWaveToRippleKernel
#pragma kernel MultiWaveToSphereKernel
#pragma kernel MultiWaveToTorusKernel

#pragma kernel RippleToWaveKernel
#pragma kernel RippleToMultiWaveKernel
#pragma kernel RippleKernel
#pragma kernel RippleToSphereKernel
#pragma kernel RippleToTorusKernel

#pragma kernel SphereToWaveKernel
#pragma kernel SphereToMultiWaveKernel
#pragma kernel SphereToRippleKernel
#pragma kernel SphereKernel
#pragma kernel SphereToTorusKernel

#pragma kernel TorusToWaveKernel
#pragma kernel TorusToMultiWaveKernel
#pragma kernel TorusToRippleKernel
#pragma kernel TorusToSphereKernel
#pragma kernel TorusKernel
```

回到 `GPUGraph`，为过渡进度着色器属性添加标识符。

```c#
	static readonly int
		…
		timeId = Shader.PropertyToID("_Time"),
		transitionProgressId = Shader.PropertyToID("_TransitionProgress");
```

如果我们正在转换，请在 `UpdateFunctionOnGPU` 中设置它，否则不用麻烦。我们在这里应用了 smoothstep 函数，所以我们不必对 GPU 上的每个点都这样做。这是一个小的优化，但它是免费的，避免了大量的工作。

```c#
		computeShader.SetFloat(timeId, Time.time);
		if (transitioning) {
			computeShader.SetFloat(
				transitionProgressId,
				Mathf.SmoothStep(0f, 1f, duration / transitionDuration)
			);
		}
```

要选择正确的内核索引，请向其中添加五倍的转换函数，或者如果我们不转换，则添加五倍相同的函数。

```c#
		var kernelIndex =
			(int)function + (int)(transitioning ? transitionFunction : function) * 5;
```

*连续随机变形。*

对我来说，添加的过渡仍然不会影响帧率。很明显，渲染才是瓶颈，而不是位置的计算。

### 3.4 函数计数属性

为了计算内核索引，`GPUGraph` 需要知道有多少个函数。我们可以在返回它的 `FunctionLibrary` 中添加一个 `GetFunctionCount` 方法，而不是在 `GPUGraph` 中对其进行硬编码。这样做的好处是，如果我们要添加或删除一个函数，我们只需要更改两个 *FunctionLibrary* 文件——类和计算着色器。

```c#
	public static int GetFunctionCount () {
		return 5;
	}
```

我们甚至可以删除常量值并返回 `functions` 数组的长度，从而进一步减少我们以后必须更改的代码。

```c#
	public static int GetFunctionCount () {
		return functions.Length;
	}
```

函数计数是转换为属性的一个很好的候选者。要自己创建一个，请从 `GetFunctionCount` 中删除 `Get` 前缀，并删除其空参数列表。然后将 return 语句包装在嵌套的 `get` 代码块中。

```c#
	public static int FunctionCount {
		get {
			return functions.Length;
		}
	}
```

这定义了一个 getter 属性。由于它只返回一个值，我们可以通过将 `get` 块简化为表达式体来简化它，这是通过用 `get => functions.Length;` 替换它来完成的。

```c#
	public static int FunctionCount {
		get => functions.Length;
	}
```

因为没有 `set` 块，我们可以通过省略 `get` 来进一步简化属性。这将属性简化为一条线。

```c#
	public static int FunctionCount => functions.Length;
```

这也适用于适用的方法，在本例中为 `GetFunction` 和 `GetNextFunctionName`。

```c#
	public static Function GetFunction (FunctionName name) => functions[(int)name];

	public static FunctionName GetNextFunctionName (FunctionName name) =>
		(int)name < functions.Length - 1 ? name + 1 : 0;
```

在 `GPUGraph.UpdateFunctionOnGPU` 中使用新属性而不是常数值。

```c#
		var kernelIndex =
			(int)function +
			(int)(transitioning ? transitionFunction : function) *
			FunctionLibrary.FunctionCount;
```

### 3.5 更多详情

总之，由于分辨率的提高，我们的函数可以变得更加详细。例如，我们可以将*球体*扭曲的频率加倍。

```glsl
float3 Sphere (float u, float v, float t) {
	float r = 0.9 + 0.1 * sin(PI * (12.0 * u + 8.0 * v + t));
	…
}
```

![img](https://catlikecoding.com/unity/tutorials/basics/compute-shaders/procedural-drawing/detailed-sphere.png)

*更详细的范围。*

星形图案和*圆环*的扭曲也是如此。这将使扭曲看起来相对于主图案移动得更慢，因此也会稍微增加它们的时间因素。

```glsl
float3 Torus (float u, float v, float t) {
	float r1 = 0.7 + 0.1 * sin(PI * (8.0 * u + 0.5 * t));
	float r2 = 0.15 + 0.05 * sin(PI * (16.0 * u + 8.0 * v + 3.0 * t));
	…
}
```

![img](https://catlikecoding.com/unity/tutorials/basics/compute-shaders/procedural-drawing/detailed-torus.png)

*更详细的圆环。*

为了保持两个函数库的同步，还需要调整 `FunctionLibrary` 类中的函数。这允许在基于游戏对象 CPU 和基于程序 GPU 的方法之间进行更诚实的比较。

下一个教程是[工作](https://catlikecoding.com/unity/tutorials/basics/jobs/)。

[license](https://catlikecoding.com/unity/tutorials/license/)

[repository](https://bitbucket.org/catlikecodingunitytutorials/basics-05-compute-shaders/)

[PDF](https://catlikecoding.com/unity/tutorials/basics/compute-shaders/Compute-Shaders.pdf)



# 工作：动画分形

发布于 2020-12-15 更新于 2021-05-18

https://catlikecoding.com/unity/tutorials/basics/jobs/

*使用对象层次结构构建分形。*
*理顺等级制度。*
*摆脱游戏对象并按程序绘制。*
*使用作业更新分形。*
*并行更新分形的部分内容。*

这是关于学习使用 Unity [基础](https://catlikecoding.com/unity/tutorials/basics/)知识的系列教程中的第六个。这次我们将创建一个动画分形。我们从常规的游戏对象层次结构开始，然后慢慢过渡到作业系统，在此过程中衡量性能。

本教程使用 Unity 2020.3.6f1 编写。

![img](https://catlikecoding.com/unity/tutorials/basics/jobs/tutorial-image.jpg)

*由 97656 个球体组成的分形。*

## 1 分形

一般来说，分形是一种具有自相似性的东西，简单来说，这意味着它的较小部分看起来与较大的部分相似。例如海岸线和许多植物。例如，一棵树的树枝可以看起来像树干，只是更小。同样，它的树枝也可以看起来像树枝的较小版本。还有数学和几何分形。一些例子是 Mandelbrot 和 Julia 集合、Koch 雪花、Menger 海绵和 Sierpiński 三角形。

几何分形可以通过从初始形状开始，然后将其较小的副本附加到自身上来构建，这也会产生其较小的版本，以此类推。理论上，这可以永远持续下去，创造出无限数量的形状，但这些形状仍占据有限的空间。我们可以在 Unity 中创建这样的东西，但在性能下降太多之前，只能创建几个级别。

我们将在与[上一教程](https://catlikecoding.com/unity/tutorials/basics/compute-shaders/)相同的项目中创建分形，只是没有图形。

### 1.1 创建分形

首先创建一个 `Fractal` 组件类型来表示我们的分形。给它一个可配置的深度整数来控制分形的最大深度。最小深度为 1，仅由初始形状组成。我们将使用 8 作为最大值，这相当高，但不应该太高，以免意外地使您的机器没有响应。深度为 4 是合理的默认值。

```c#
using UnityEngine;

public class Fractal : MonoBehaviour {

	[SerializeField, Range(1, 8)]
	int depth = 4;
}
```

我们将使用球体作为初始形状，可以通过 *GameObject/3D Object/Sphere* 创建。将它定位在世界原点，将我们的分形分量附加到它上面，并给它一个简单的材质。我最初使用 URP 将其变为黄色。从中删除 `SphereCollider` 组件，以使游戏对象尽可能简单。

![img](https://catlikecoding.com/unity/tutorials/basics/jobs/fractal/fractal-inspector.png)

*分形检查器。*

要将球体变成分形，我们必须生成它的克隆。这可以通过调用 `Instantiate` 方法并将其自身作为参数来实现。为了传递对 `Fractal` 实例本身的引用，我们可以使用 `this` 关键字，因此我们将调用 `Instantiate(this)`，我们将在进入播放模式后执行此操作以生成分形。

我们可以在 `Awake` 方法中克隆分形，但克隆的 `Awake` 方法也会立即被调用，这会立即创建另一个实例，以此类推。这会一直持续到 Unity 崩溃，因为它递归调用了太多的方法，这会很快发生。

为了避免立即递归，我们可以添加一个 `Start` 方法并在其中调用 `Instantite`。`Start` 是另一个 Unity 事件方法，它与 `Awake` 一样，在创建组件后也会被调用一次。不同之处在于，`Start` 不会立即被调用，而是在组件上第一次调用 `Update` 方法之前，无论它是否有一个 Update 方法。此时创建的新组件将在下一帧中首次更新。这意味着每帧只会发生一次实例化。

```c#
	void Start () {
		Instantiate(this);
	}
```

如果你现在进入播放模式，你会看到每一帧都会创建一个新的克隆。首先克隆原始分形，然后克隆第一个克隆，然后克隆第二个克隆，以此类推。此过程只会在您的机器内存不足时停止，因此您应该在此之前退出播放模式。

![img](https://catlikecoding.com/unity/tutorials/basics/jobs/fractal/infinite-clones.png)

*创建无限的克隆。*

为了强制执行最大深度，一旦达到最大深度，我们就必须中止实例化。最直接的方法是减小生成的子分形的配置深度。

```c#
	void Start () {
		Fractal child = Instantiate(this);
		child.depth = depth - 1;
	}
```

然后，我们可以在 `Start` 开始时检查深度是否为 1 或更小。如果是这样，我们就不应该再深入下去，放弃这种方法。我们可以通过从它返回来实现这一点，单独使用 `return` 语句，因为这是一个 `void` 方法，它什么也不返回。

```c#
	void Start () {
		if (depth <= 1) {
			return;
		}

		Fractal child = Instantiate(this);
		child.depth = depth - 1;
	}
```

为了更容易看到配置的深度确实随着每个新的子分形而减小，让我们将它们的 `name` 属性设置为 Fractal，然后是空间和深度。整数可以通过加法运算符附加到文本字符串中。这将生成数字的文本表示，然后生成一个新的连接字符串。

```c#
	void Start () {
		name = "Fractal " + depth;

		…
	}
```

![img](https://catlikecoding.com/unity/tutorials/basics/jobs/fractal/four-levels.png)

*四个分形层次，深度逐渐减小。*

深度确实会随着级别的增加而减少，一旦我们创建了正确数量的克隆，这个过程就会停止。为了使新的分形成为其直接父分形的真正子分形，我们必须将其父分形设置为生成它们的父分形。

```c#
		Fractal child = Instantiate(this);
		child.depth = depth - 1;
		child.transform.SetParent(transform, false);
```

![img](https://catlikecoding.com/unity/tutorials/basics/jobs/fractal/fractal-hierarchy.png)

*分形层次。*

这为我们提供了一个简单的游戏对象层次结构，但它看起来仍然像一个球体，因为它们都重叠了。若要更改此设置，请将子对象变换的局部位置设置为 `Vector3.right`。这将它定位在其父对象的右侧一个单位处，因此我们所有的球体最终都会沿着 X 轴排成一行。

```c#
		child.transform.SetParent(transform, false);
		child.transform.localPosition = Vector3.right;
```

![img](https://catlikecoding.com/unity/tutorials/basics/jobs/fractal/spheres-in-a-row.png)

*球体排成一排。*

自相似性的概念是，较小的部分看起来像较大的部分，因此每个孩子都应该比父母小。我们只需将其大小减半，将其局部比例设置为统一的 0.5。由于规模也适用于儿童，这意味着在层次结构中，每一步的大小都是一半。

```c#
		child.transform.localPosition = Vector3.right;
		child.transform.localScale = 0.5f * Vector3.one;
```

![img](https://catlikecoding.com/unity/tutorials/basics/jobs/fractal/spheres-decreasing-size.png)

*球体尺寸逐渐减小。*

为了使球体再次接触，我们必须减小它们的偏移。父对象和子对象的局部半径过去都是 0.5，因此偏移 1 会使它们接触。由于孩子的大小减半，其局部半径现在为 0.25，因此偏移量应减小到 0.75。

```c#
		child.transform.localPosition = 0.75f * Vector3.right;
		child.transform.localScale = 0.5f * Vector3.one;
```

![img](https://catlikecoding.com/unity/tutorials/basics/jobs/fractal/spheres-touching.png)

*触摸球体。*

### 1.2 多个孩子

每个级别只生育一个孩子会产生一条大小逐渐减小的球体线，这不是一个有趣的分形。因此，让我们在每一步添加第二个子项，通过复制创建子项的代码，重用 `child` 变量。唯一的区别是，我们将使用 `Vector3.up` 作为额外的子对象，这将使它的子对象位于父对象之上，而不是右侧。

```c#
		Fractal child = Instantiate(this);
		child.depth = depth - 1;
		child.transform.SetParent(transform, false);
		child.transform.localPosition = 0.75f * Vector3.right;
		child.transform.localScale = 0.5f * Vector3.one;

		child = Instantiate(this);
		child.depth = depth - 1;
		child.transform.SetParent(transform, false);
		child.transform.localPosition = 0.75f * Vector3.up;
		child.transform.localScale = 0.5f* Vector3.one;
```

预计每个分形部分现在都将有两个孩子，深度可达四级。

![img](https://catlikecoding.com/unity/tutorials/basics/jobs/fractal/spheres-multiple-children-incorrect.png)

*有多个孩子的球体，不正确*

事实似乎并非如此。我们最终在分形的顶部有太多的层次。这是因为当我们克隆分形以创建其第二个子对象时，我们已经给了它第一个子对象。这个孩子现在也被克隆了，因为 `Instantiate` 复制了传递给它的整个游戏对象层次结构。

解决方案是只有在创建了两个孩子之后才能建立亲子关系。为了使这更容易，让我们将子创建代码移动到一个单独的 `CreateChild` 方法中，该方法返回子分形。它做的一切都一样，除了它不设置父对象，偏移方向成为一个参数。

```c#
	Fractal CreateChild (Vector3 direction) {
		Fractal child = Instantiate(this);
		child.depth = depth - 1;
		child.transform.localPosition = 0.75f * direction;
		child.transform.localScale = 0.5f * Vector3.one;
		return child;
	}
```

从 `Start` 中删除子创建代码，改为使用向上和向右向量作为参数调用 `CreateChild` 两次。通过变量跟踪孩子，然后用这些变量设置他们的父母。

```c#
	void Start () {
		name = "Fractal " + depth;

		if (depth <= 1) {
			return;
		}

		Fractal childA = CreateChild(Vector3.up);
		Fractal childB = CreateChild(Vector3.right);
		
		childA.transform.SetParent(transform, false);
		childB.transform.SetParent(transform, false);
	}
```

![img](https://catlikecoding.com/unity/tutorials/basics/jobs/fractal/spheres-multiple-children-correct.png)

*有多个孩子的球体，正确*

### 1.3 重新定位（Reorientation）

我们现在得到一个分形，每个部分只有两个子部分，除了最大深度处没有子部分的最小部分。这些孩子的位置总是一样的：一个在上面，一个在右边。然而，分形儿童依附于他们的父母，可以被认为是脱离了父母。因此，他们的取向也与父母有关，这是有道理的。对于子对象来说，其父对象是地面，这使得其偏移方向等于其局部上轴。

我们可以通过添加 `Quaterion` 来支持每个零件的不同方向，我们将其分配给子零件的局部旋转，使其相对于其父零件的方向。

```c#
	Fractal CreateChild (Vector3 direction, Quaternion rotation) {
		Fractal child = Instantiate(this);
		child.depth = depth - 1;
		child.transform.localPosition = 0.75f * direction;
		child.transform.localRotation = rotation;
		child.transform.localScale = 0.5f * Vector3.one;
		return child;
	}
```

在 `Start` 中，第一个子对象位于其父对象上方，因此其方向不会改变。我们可以用 `Quaternion.identity` 来表示这一点，这是表示没有旋转的恒等式四元数。第二个孩子在右边，所以我们必须绕 Z 轴顺时针旋转 90°。我们可以通过静态 `Quaternion.Euler` 方法来实现这一点，在给定 Euler 角度的情况下沿 X、Y 和 Z 轴创建旋转。前两个轴为零，Z 为 -90°。

```c#
		Fractal childA = CreateChild(Vector3.up, Quaternion.identity);
		Fractal childB = CreateChild(Vector3.right, Quaternion.Euler(0f, 0f, -90f));
```

![img](https://catlikecoding.com/unity/tutorials/basics/jobs/fractal/spheres-reoriented.png)

*重新定向分形儿童。*

### 1.4 完成分形

让我们通过添加第三个子对象来继续生长分形，这次向左偏移，围绕 Z 轴旋转 90°。这就完成了我们在 XY 平面上的分形。

```c#
		Fractal childA = CreateChild(Vector3.up, Quaternion.identity);
		Fractal childB = CreateChild(Vector3.right, Quaternion.Euler(0f, 0f, -90f));
		Fractal childC = CreateChild(Vector3.left, Quaternion.Euler(0f, 0f, 90f));

		childA.transform.SetParent(transform, false);
		childB.transform.SetParent(transform, false);
		childC.transform.SetParent(transform, false);
```

![img](https://catlikecoding.com/unity/tutorials/basics/jobs/fractal/fractal-2d.png)

*2D 分形。*

> **我们还可以添加一个向下偏移的子项吗？**
>
> 是的，但这只对根分形部分有意义，因为在所有其他情况下，孩子最终会隐藏在父母的父母体内。为了简单起见，我不会只给根一个额外的孩子。

我们通过添加另外两个子项，将分形带入第三维，这两个子项具有前后偏移，以及围绕 X 轴的 90° 和 -90° 旋转。

```c#
		Fractal childA = CreateChild(Vector3.up, Quaternion.identity);
		Fractal childB = CreateChild(Vector3.right, Quaternion.Euler(0f, 0f, -90f));
		Fractal childC = CreateChild(Vector3.left, Quaternion.Euler(0f, 0f, 90f));
		Fractal childD = CreateChild(Vector3.forward, Quaternion.Euler(90f, 0f, 0f));
		Fractal childE = CreateChild(Vector3.back, Quaternion.Euler(-90f, 0f, 0f));

		childA.transform.SetParent(transform, false);
		childB.transform.SetParent(transform, false);
		childC.transform.SetParent(transform, false);
		childD.transform.SetParent(transform, false);
		childE.transform.SetParent(transform, false);
```

![img](https://catlikecoding.com/unity/tutorials/basics/jobs/fractal/fractal-3d.png)

*3D 分形。*

一旦你确定分形是正确的，你可以尝试配置更大的深度，例如六个。

![img](https://catlikecoding.com/unity/tutorials/basics/jobs/fractal/fractal-depth-6.png)

*深度 6  分形。*

在这个深度，你可以注意到分形描述的金字塔的侧面显示了谢尔宾斯基（Sierpiński）三角形的图案。这在使用其他程序投影时最容易看到。

![img](https://catlikecoding.com/unity/tutorials/basics/jobs/fractal/sierpinski-triangle-pattern.png)

*谢尔宾斯基三角形图案。*

### 1.5 动画

我们可以通过为分形设置动画来赋予它生命。创建无休止运动的最简单方法是在新的更新方法中沿其局部向上轴旋转每个部分。这可以通过在分形的 `Transform` 组件上调用 `Rotate` 来完成。这将随着时间的推移进行累积旋转，从而使其旋转。如果我们对第二个参数使用 `Time.deltaTime`，对另外两个参数使用零，那么我们最终的旋转速度为每秒一度。让我们将其缩放到每秒 22.5°，以便在 16 秒内实现完整的 360° 旋转。由于分形的四边对称性，该动画将每四秒钟循环一次。

```c#
	void Update () {
		transform.Rotate(0f, 22.5f * Time.deltaTime, 0f);
	}
```

*为分形设置动画。*

分形的每个部分都以完全相同的方式旋转，但由于整个分形的递归性质，这会产生运动，分形越深，运动就越复杂。

### 1.6 性能

我们的动画分形可能是一个好主意，但它也应该运行得足够快。深度小于 6 的裂缝应该没有问题，但高于 6 的裂缝可能会出现问题。所以我介绍了一些建筑。

![img](https://catlikecoding.com/unity/tutorials/basics/jobs/fractal/profiler-build-urp-depth-6.png)

*使用 URP 和分形深度 6 分析构建。*

我分析了深度为 6、7 和 8 的分形的单独构建。我粗略估计了每帧调用 `Update` 方法的平均时间（以毫秒为单位），以及 URP 和 BRP 的平均每秒帧数。我关闭了 VSync，以便最好地了解它在我的机器上可能运行的速度。我还使用 *IL2CPP* 而不是 *Mono* 作为*脚本后端*，可以在播放器项目设置的*其他设置/配置*下找到。

> **Mono 和 IL2CPP 有什么区别？**
>
> 后者通常生成比 Mono 运行时环境所能提供的更优化的本机代码。

| 深度 | MS   | URP  | BRP  |
| ---- | ---- | ---- | ---- |
| 6    | 2    | 125  | 80   |
| 7    | 8    | 24   | 15   |
| 8    | 10   | 4    | 3    |

事实证明，深度 6 没有问题，但我的机器在深度 7 上挣扎，而深度 8 是一场灾难。调用 `Update` 方法花费了太多时间。仅此一项就将帧速率限制在最多 25FPS，但 URP 的帧速率最终会低得多，为 4，BRP 为 3。

Unity 的默认球体有很多顶点，因此尝试同样的实验是有意义的，但用立方体替换分形的网格，这样渲染成本要低得多。这样做之后，我得到了同样的结果，这表明 CPU 是瓶颈，而不是 GPU。

![img](https://catlikecoding.com/unity/tutorials/basics/jobs/fractal/fractal-depth-6-cubes.png)

*深度 6 分形，使用立方体而不是球体。*

请注意，使用立方体时，分形自相交，因为立方体比球体突出得多。深度 4 处的一些部分最终会接触到 1 级根部。因此，这些部分的向上子部分最终会穿透根部，而该级别的其他一些子部分会接触到 2 级部分，以此类推。

## 2 平面层次结构

我们分形的递归层次结构及其所有独立运动的部分是 Unity 努力解决的问题。它必须单独更新零件，计算它们的对象到世界的转换矩阵，然后剔除它们，最后用 GPU 实例化或 SRP 批处理器渲染它们。由于我们确切地知道分形是如何工作的，我们可以使用比 Unity 的通用方法更有效的策略。我们或许可以通过简化层次结构，摆脱其递归性质来提高性能。

### 2.1 清理

重构分形层次结构的第一步是删除当前的方法。删除 `Start`、`CreateChild` 和 `Update` 方法。

```c#
	//void Start () { … }

	//Fractal CreateChild (Vector3 direction, Quaternion rotation) { … }

	//void Update () { … }
```

我们将使用它作为所有分形部分的根容器，而不是复制根游戏对象。因此，从我们的分形游戏对象中删除 `MeshFilter` 和 `MeshRenderer` 组件。然后将网格和材质的配置字段添加到 `Fractal` 中。通过检查器将它们设置为我们之前使用的球体和材料。

```c#
	[SerializeField]
	Mesh mesh;

	[SerializeField]
	Material material;
```

![img](https://catlikecoding.com/unity/tutorials/basics/jobs/flat-hierarchy/adjusted-fractal.png)

*调整后的分形游戏对象。*

我们将对分形部分使用相同的方向和旋转。这次我们将把这些存储在静态数组中，以便以后轻松访问。

```c#
	static Vector3[] directions = {
		Vector3.up, Vector3.right, Vector3.left, Vector3.forward, Vector3.back
	};

	static Quaternion[] rotations = {
		Quaternion.identity,
		Quaternion.Euler(0f, 0f, -90f), Quaternion.Euler(0f, 0f, 90f),
		Quaternion.Euler(90f, 0f, 0f), Quaternion.Euler(-90f, 0f, 0f)
	};
```

### 2.2 创建零件

现在，我们将重新审视如何创建零件。为此添加一个新的 `CreatePart` 方法，最初是一个没有参数的 void 方法。

```c#
	void CreatePart () {}
```

在 `Awake` 方法中调用它。这次我们不需要防止无限递归，所以不需要等到 `Start`。

```c#
	void Awake () {
		CreatePart();
	}
```

我们将在 `CreatePart` 中手动构造一个新的游戏对象。这是通过调用 `GameObject` 构造函数方法完成的。通过提供该字符串作为参数，为其命名为 *Fractal Part*。用变量跟踪它，然后将分形根作为其父级。

```c#
	void CreatePart () {
		var go = new GameObject("Fractal Part");
		go.transform.SetParent(transform, false);
	}
```

![img](https://catlikecoding.com/unity/tutorials/basics/jobs/flat-hierarchy/first-fractal-part.png)

*分形与第一部分。*

这为我们提供了一个只有 `Transform` 组件而没有其他组件的游戏对象。为了使其可见，我们必须通过在游戏对象上调用 `AddComponent` 来添加更多组件。做一次。

```c#
		var go = new GameObject("Fractal Part");
		go.transform.SetParent(transform, false);
		go.AddComponent();
```

`AddComponent` 是一种泛型方法，可以添加任何类型的组件。它就像一个方法模板，每个所需的组件类型都有一个特定的版本。我们通过在尖括号内将其附加到方法名称上来指定所需的类型。对 `MeshFilter` 执行此操作。

```c#
		go.AddComponent<MeshFilter>();
```

这将向游戏对象添加一个 `MeshFilter`，该对象也将返回。我们需要将我们的网格分配给它的 `mesh` 属性，这可以直接在方法调用的结果上完成。

```c#
		go.AddComponent<MeshFilter>().mesh = mesh;
```

对 `MeshRenderer` 组件执行相同的操作，设置其材质。

```c#
		go.AddComponent<MeshFilter>().mesh = mesh;
		go.AddComponent<MeshRenderer>().material = material;
```

我们的分形部分现在被渲染，因此在进入游戏模式后会出现一个球体。

### 2.3 存储信息

与其让每个部分自行更新，我们不如从具有 `Fractal` 分量的单个根对象控制整个分形。这对 Unity 来说要容易得多，因为它只需要管理一个更新的游戏对象，而不是可能的数千个。但要做到这一点，我们需要跟踪单个 `Fractal` 组件中所有部分的数据。

至少我们需要知道零件的方向和旋转。我们可以通过将它们存储在数组中来跟踪它们。但是，我们不会对向量和四元数使用单独的数组，而是通过创建一个新的 `FractalPart` 结构类型将它们组合在一起。这类似于定义一个类，但使用 `struct` 关键字而不是 `class`。因为我们只需要在 `Fractal` 中定义这种类型，所以在该类中定义它以及它的字段。出于同样的原因，不要公开。

```c#
public class Fractal : MonoBehaviour {
	
	struct FractalPart {
		Vector3 direction;
		Quaternion rotation;
	}
	
	…
}
```

这种类型将作为一个简单的数据容器，这些数据被捆绑在一起，并被视为一个值，而不是一个对象。为了使 `Fractal` 中的其他代码能够访问此嵌套类型中的字段，需要将其公开。请注意，这只会暴露 `Fractal` 中的字段，因为 `Fractal` 内部的结构本身是私有的。

```c#
	struct FractalPart {
		public Vector3 direction;
		public Quaternion rotation;
	}
```

为了正确地定位、旋转和缩放分形部分，我们需要访问它的 `Transform` 组件，因此也需要在结构中添加一个字段作为对它的引用。

```c#
	struct FractalPart {
		public Vector3 direction;
		public Quaternion rotation;
		public Transform transform;
	}
```

现在我们可以在 `Fractal` 中为分形部分数组定义一个字段。

```c#
	FractalPart[] parts;
```

虽然可以将所有部分放在一个大数组中，但让我们给同一级别的所有部分自己的数组。这使得以后使用层次结构更容易。我们通过将 `parts` 字段转换为数组来跟踪所有这些数组。这样一个数组的元素类型是 `FractalPart[]`，因此它自己的类型被定义为后面跟着一对空方括号，就像任何其他数组一样。

```c#
	FractalPart[][] parts;
```

在 `Awake` 开始时创建这个新的顶级数组，其大小等于分形深度。在这种情况下，大小在第一对方括号内声明，第二对方括号应为空。

```c#
	void Awake () {
		parts = new FractalPart[depth][];

		CreatePart();
	}
```

每个级别都有自己的数组，也是只有一个部分的分形的根级别。因此，首先为单个元素创建一个新的 `FractalPart` 数组，并将其分配给第一级。

```c#
		parts = new FractalPart[depth][];
		parts[0] = new FractalPart[1];
```

之后，我们必须为其他级别创建一个数组。每一级都是上一级的五倍，因为我们给五个孩子。将级别数组创建变成一个循环，跟踪数组长度，并在每次迭代结束时将其乘以 5。

```c#
		parts = new FractalPart[depth][];
		int length = 1;
		for (int i = 0; i < parts.Length; i++) {
			parts[i] = new FractalPart[length];
			length *= 5;
		}
```

因为长度是一个整数，我们只在循环中使用它，所以我们可以将它合并到 `for` 语句中，将初始化器和调整部分转换为逗号分隔的列表。

```c#
		parts = new FractalPart[depth][];
		//int length = 1;
		for (int i = 0, length = 1; i < parts.Length; i++, length *= 5) {
			parts[i] = new FractalPart[length];
			//length *= 5;
		}
```

| Level 等级 | Parts 部分 | Cumulative 累积 |
| ---------- | ---------- | --------------- |
| 1          | 1          | 1               |
| 2          | 5          | 6               |
| 3          | 25         | 31              |
| 4          | 125        | 156             |
| 5          | 625        | 781             |
| 6          | 3125       | 3906            |
| 7          | 15625      | 19531           |
| 8          | 78125      | 97656           |

### 2.4 创建所有零件

要检查我们是否正确创建了零件，请将级别索引的参数添加到 `CreatePart` 中，并将其附加到零件的名称中。请注意，级别指数从零开始并增加，而我们在之前的方法中减小了子级的配置深度。

```c#
	void CreatePart (int levelIndex) {
		var go = new GameObject("Fractal Part " + levelIndex);
		…
	}
```

第一部分的水平指数为零。然后在所有级别上进行循环，从索引 1 开始，因为我们首先明确地完成了顶层的单个部分。我们将嵌套循环，因此为级别迭代器变量使用更具体的名称，如 `li`。

```c#
	void Awake () {
		…

		CreatePart(0);
		for (int li = 1; li < parts.Length; li++) {}
	}
```

每次级别迭代，首先存储对该级别零件数组的引用。然后循环该级别的所有部分并创建它们，这次使用 `fpi` 这样的名称作为分形部分迭代器变量。

```c#
		for (int li = 1; li < parts.Length; li++) {
			FractalPart[] levelParts = parts[li];
			for (int fpi = 0; fpi < levelParts.Length; fpi++) {
				CreatePart(li);
			}
		}
```

![img](https://catlikecoding.com/unity/tutorials/basics/jobs/flat-hierarchy/all-fractal-parts.png)

*所有分形部分，按级别创建。*

由于孩子们有不同的方向和旋转，我们需要区分他们。我们通过向 `CreatePart` 添加一个子索引来实现这一点，我们也可以将其添加到游戏对象的名称中。

```c#
	void CreatePart (int levelIndex, int childIndex) {
		var go = new GameObject("Fractal Part L" + levelIndex + " C" + childIndex);
		…
	}
```

根部分不是另一部分的子部分，因此我们使用索引零，因为它可以被视为地面的向上子部分。

```c#
		CreatePart(0, 0);
```

在每个级别的循环中，我们必须循环遍历五个子索引。我们可以通过在每次迭代中递增子索引，并在适当的时候将其重置为零来实现这一点。或者，我们可以在另一个嵌套循环中显式创建五个子循环。这要求我们每次迭代都将分形部分索引增加 5，而不仅仅是增加它。

```c#
		for (int li = 1; li < parts.Length; li++) {
			FractalPart[] levelParts = parts[li];
			for (int fpi = 0; fpi < levelParts.Length; fpi += 5) {
				for (int ci = 0; ci < 5; ci++) {
					CreatePart(li, ci);
				}
			}
		}
```

![img](https://catlikecoding.com/unity/tutorials/basics/jobs/flat-hierarchy/parts-level-child-index.png)

*显示了级别和儿童索引。*

我们还必须确保零件的尺寸正确。同一级别的所有部分具有相同的比例，这不会改变。因此，在创建每个零件时，我们只需要设置一次。将其参数添加到 `CreatePart` 中，并使用它来设置统一比例。

```c#
	void CreatePart (int levelIndex, int childIndex, float scale) {
		var go = new GameObject("Fractal Part L" + levelIndex + " C" + childIndex);
		go.transform.localScale = scale * Vector3.one;
		…
	}
```

根部分的比例为 1。之后，每个级别的刻度减半。

```c#
		float scale = 1f;
		CreatePart(0, 0, scale);
		for (int li = 1; li < parts.Length; li++) {
			scale *= 0.5f;
			FractalPart[] levelParts = parts[li];
			for (int fpi = 0; fpi < levelParts.Length; fpi += 5) {
				for (int ci = 0; ci < 5; ci++) {
					CreatePart(li, ci, scale);
				}
			}
		}
```

### 2.5 重建分形

为了重建分形的结构，我们必须直接定位所有部分，这次是在世界空间中。由于我们没有使用变换层次结构，因此位置将随着分形的动画而变化，因此我们将在 `Update` 而不是 `Awake` 中不断设置它们。但首先我们需要存储零件的数据。

首先更改 `CreatePart`，使其返回新的 `FractalPart` 结构值。

```c#
	FractalPart CreatePart (int levelIndex, int childIndex, float scale) {
		…

		return new FractalPart();
	}
```

然后使用其子索引和静态数组，以及对其游戏对象的 `Transform` 组件的引用，设置该部分的方向和旋转。我们可以通过将新部分存储在变量中，设置其字段，然后返回它来实现这一点。另一种方法是使用对象或结构初始化器。这是一个花括号内的列表，位于构造函数调用的参数列表之后。

```c#
		return new FractalPart() {};
```

我们可以将任务分配给其中创建的任何字段或属性，作为逗号分隔的列表。

```c#
		return new FractalPart() {
			direction = directions[childIndex],
			rotation = rotations[childIndex],
			transform = go.transform
		};
```

如果构造函数方法调用没有参数，如果我们包含一个初始化器，我们可以跳过空参数列表。

```c#
		//return new FractalPart() {
		return new FractalPart {
			…
		};
```

将返回的部分复制到 `Awake` 中的正确数组元素中。这是根部分第一个数组的第一个元素。对于其他部分，它是当前级别数组的元素，其索引等于分形部分索引。当我们以五步为单位增加该指数时，也必须添加子指数。

```c#
		parts[0][0] = CreatePart(0, 0, scale);
		for (int li = 1; li < parts.Length; li++) {
			scale *= 0.5f;
			FractalPart[] levelParts = parts[li];
			for (int fpi = 0; fpi < levelParts.Length; fpi += 5) {
				for (int ci = 0; ci < 5; ci++) {
					levelParts[fpi + ci] = CreatePart(li, ci, scale);
				}
			}
		}
```

接下来，创建一个新的 `Update` 方法，迭代所有级别及其所有部分，将相关的分形部分数据存储在变量中。我们再次从第二层开始循环，因为根部分不移动，始终位于原点。

```c#
	void Update () {
		for (int li = 1; li < parts.Length; li++) {
			FractalPart[] levelParts = parts[li];
			for (int fpi = 0; fpi < levelParts.Length; fpi++) {
				FractalPart part = levelParts[fpi];
			}
		}
	}
```

要相对于其父级定位零件，我们还需要访问父级的 `Transform` 组件。为此，还要跟踪父零件阵列。父元素是该数组中的元素，其索引等于当前部分的索引除以 5。这是有效的，因为我们执行整数除法，所以没有余数。因此，索引为 0-4 的部分得到父索引 0，索引为 5-9 的部分得到母索引 1，以此类推。

```c#
		for (int li = 1; li < parts.Length; li++) {
			FractalPart[] parentParts = parts[li - 1];
			FractalPart[] levelParts = parts[li];
			for (int fpi = 0; fpi < levelParts.Length; fpi++) {
				Transform parentTransform = parentParts[fpi / 5].transform;
				FractalPart part = levelParts[fpi];
			}
		}
```

现在，我们可以设置零件相对于其指定父级的位置。首先，使其局部位置等于其父级位置，再加上零件的方向乘以其局部比例。由于比例是均匀的，我们可以用比例的 X 分量来满足。

```c#
				Transform parentTransform = parentParts[fpi / 5].transform;
				FractalPart part = levelParts[fpi];
				part.transform.localPosition =
					parentTransform.localPosition +
					part.transform.localScale.x * part.direction;
```

![img](https://catlikecoding.com/unity/tutorials/basics/jobs/flat-hierarchy/parts-distance-incorrect.png)

*零件彼此靠得太近。*

这会使零件离其父级太近，因为我们正在按零件自己的比例缩放距离。由于每个级别的规模减半，我们必须将最终偏移量增加到 150%。

```c#
				part.transform.localPosition =
					parentTransform.localPosition +
					1.5f * part.transform.localScale.x * part.direction;
```

![img](https://catlikecoding.com/unity/tutorials/basics/jobs/flat-hierarchy/parts-distance-correct.png)

*零件之间的距离正确。*

我们还必须应用零件的旋转。这是通过将其指定给对象的局部旋转来实现的。在确定其位置之前，让我们先这样做。

```c#
				part.transform.localRotation = part.rotation;		
				part.transform.localPosition =
					parentTransform.localPosition +
					1.5f * part.transform.localScale.x * part.direction;
```

但我们也必须传播父对象的旋转。旋转可以通过四元数的乘法来堆叠。与常规的数字乘法不同，在这种情况下，顺序很重要。得到的四元数表示通过执行第二个四元数的旋转，然后应用第一个四元数来获得的旋转。因此，在变换层次结构中，首先执行子对象的旋转，然后执行父对象的旋转。因此，正确的四元数乘法顺序是父子。

```c#
				part.transform.localRotation =
					parentTransform.localRotation * part.rotation;
```

最后，父对象的旋转也应影响其偏移的方向。我们可以通过执行四元数-向量乘法将四元数旋转应用于向量。

```c#
				part.transform.localPosition =
					parentTransform.localPosition +
					parentTransform.localRotation *
						(1.5f * part.transform.localScale.x * part.direction);
```

![img](https://catlikecoding.com/unity/tutorials/basics/jobs/flat-hierarchy/fractal-restored.png)

*恢复分形。*

### 2.6 再次制作动画

为了使分形再次动画化，我们必须重新引入另一个旋转。这次我们将创建一个四元数来表示当前增量时间的旋转，其角速度与以前相同。在 `Update` 开始时执行此操作。

```c#
	void Update () {
		Quaternion deltaRotation = Quaternion.Euler(0f, 22.5f * Time.deltaTime, 0f);
		
		…
	}
```

让我们从根部分开始。在循环之前检索它，并将其旋转与增量旋转相乘。

```c#
		Quaternion deltaRotation = Quaternion.Euler(0f, 22.5f * Time.deltaTime, 0f);
		
		FractalPart rootPart = parts[0][0];
		rootPart.rotation *= deltaRotation;
```

`FractalPart` 是一个结构体，它是一种值类型，因此更改它的局部变量不会更改其他任何内容。我们必须将它复制回它的数组元素——替换旧数据——以记住它的旋转已经改变。

```c#
		FractalPart rootPart = parts[0][0];
		rootPart.rotation *= deltaRotation;
		parts[0][0] = rootPart;
```

我们还必须调整根的 `Transform` 组件的旋转。这将使分形再次旋转，但仅围绕其根部旋转。

```c#
		FractalPart rootPart = parts[0][0];
		rootPart.rotation *= deltaRotation;
		rootPart.transform.localRotation = rootPart.rotation;
		parts[0][0] = rootPart;
```

为了旋转所有其他部分，我们还必须将相同的增量旋转纳入它们的旋转中。当所有事物都围绕其局部上轴旋转时，增量旋转是最右侧的操作数。在应用角色游戏对象的最终旋转之前执行此操作。同时将调整后的零件数据复制回末尾的数组。

```c#
			for (int fpi = 0; fpi < levelParts.Length; fpi++) {
				Transform parentTransform = parentParts[fpi / 5].transform;
				FractalPart part = levelParts[fpi];
				part.rotation *= deltaRotation;
				part.transform.localRotation =
					parentTransform.localRotation * part.rotation;
				part.transform.localPosition =
					parentTransform.localPosition +
					parentTransform.localRotation *
						(1.5f * part.transform.localScale.x * part.direction);
				levelParts[fpi] = part;
			}
```

### 2.7 再次性能

此时，我们的分形显示和动画与以前完全一样，但有一个新的平面对象层次结构和一个负责更新整个事物的组件。让我们再次分析一下，使用相同的构建设置，这种新方法是否表现更好。

![img](https://catlikecoding.com/unity/tutorials/basics/jobs/flat-hierarchy/profiler-build-urp-depth-6.png)

*使用 URP 和分形深度 6 分析构建。*

| 深度 | MS   | URP  | BRP  |
| ---- | ---- | ---- | ---- |
| 6    | 2    | 150  | 95   |
| 7    | 8    | 32   | 17   |
| 8    | 43   | 5    | 3    |

与递归方法相比，平均帧速率到处都在增加。使用 URP 的深度 7 现在对我来说达到了 30FPS。更新期间花费的时间没有明显减少，甚至在深度 8 时似乎有所增加，但这在渲染过程中被更简单的层次结构所补偿。使用多维数据集可以获得大致相同的性能。

我们可以得出结论，我们的新方法无疑是一种改进，但仅凭其本身仍不足以支持深度为 7 或 8 的分形。

## 3 程序图

因为我们的分形目前具有平坦的对象层次结构，所以它具有与我们之前教程中的图形相同的结构设计：一个具有许多几乎相同子对象的单个对象。通过按程序渲染图形的点，而不是为每个点使用单独的游戏对象，我们设法显著提高了其性能。这表明我们可以将同样的方法应用于分形。

即使对象层次是平的，分形部分仍然具有递归层次关系。这使得它与具有独立点的图有着根本的不同。这种层次依赖性使其不适合迁移到计算着色器。但仍然可以通过一个程序命令绘制同一级别的所有部分，避免了数千个游戏对象的开销。

> **是否可以使用计算着色器更新分形？**
>
> 是的，但这很不方便，因为父部件必须在孩子之前更新。这种依赖性要求将工作分为多个连续的阶段，就像我们一次迭代一个级别一样。由于大多数级别没有很多部分——从 GPU 的角度来看——它的并行处理能力无法得到有效利用。
>
> 可以应用混合方法：除最后一个级别外，所有级别都使用 CPU，然后最后一个使用 GPU。但本教程主要关注 CPU，最后我们会发现 GPU 将是瓶颈，而不是 CPU。

### 3.1 删除游戏对象

我们首先移除游戏对象。这也意味着我们不再有 `Transform` 组件来存储世界位置和旋转。相反，我们将把这些存储在 `FractalPart` 的其他字段中。

```c#
	struct FractalPart { 
		public Vector3 direction, worldPosition;
		public Quaternion rotation, worldRotation;
		//public Transform transform;
	}
```

从 `CreatePart` 中删除所有游戏对象代码。我们只需要保留其子索引参数，因为其他参数仅在创建游戏对象时使用。

```c#
	FractalPart CreatePart (int childIndex) {
		//var go = new GameObject("Fractal Part L" + levelIndex + " C" + childIndex);
		//go.transform.localScale = scale * Vector3.one;
		//go.transform.SetParent(transform, false);
		//go.AddComponent<MeshFilter>().mesh = mesh;
		//go.AddComponent<MeshRenderer>().material = material;

		return new FractalPart {
			direction = directions[childIndex],
			rotation = rotations[childIndex] //,
			//transform = go.transform
		};
	}
```

我们现在还可以将该方法简化为一个表达式。

```c#
	FractalPart CreatePart (int childIndex) => new FractalPart {
		direction = directions[childIndex],
		rotation = rotations[childIndex]
	};
```

相应地调整 `Awake` 中的代码。从现在开始，我们不再处理规模问题。

```c#
		//float scale = 1f;
		parts[0][0] = CreatePart(0);
		for (int li = 1; li < parts.Length; li++) {
			//scale *= 0.5f;
			FractalPart[] levelParts = parts[li];
			for (int fpi = 0; fpi < levelParts.Length; fpi += 5) {
				for (int ci = 0; ci < 5; ci++) {
					levelParts[fpi + ci] = CreatePart(ci);
				}
			}
		}
```

在 `Update` 中，我们现在必须将根的旋转指定给其世界旋转场，而不是 `Transform` 组件旋转。

```c#
		FractalPart rootPart = parts[0][0];
		rootPart.rotation *= deltaRotation;
		rootPart.worldRotation = rootPart.rotation;
		parts[0][0] = rootPart;
```

所有其他零件的旋转和位置都需要进行相同的调整。我们在这里还重新介绍了递减的规模。

```c#
		float scale = 1f;
		for (int li = 1; li < parts.Length; li++) {
			scale *= 0.5f;
			FractalPart[] parentParts = parts[li - 1];
			FractalPart[] levelParts = parts[li];
			for (int fpi = 0; fpi < levelParts.Length; fpi++) {
				//Transform parentTransform = parentParts[fpi / 5].transform;
				FractalPart parent = parentParts[fpi / 5];
				FractalPart part = levelParts[fpi];
				part.rotation *= deltaRotation;
				part.worldRotation = parent.worldRotation * part.rotation;
				part.worldPosition =
					parent.worldPosition +
					parent.worldRotation * (1.5f * scale * part.direction);
				levelParts[fpi] = part;
			}
		}
```

### 3.2 变换矩阵

`Transform` 组件提供用于渲染的变换矩阵。由于我们的零件不再有这些组件，我们需要自己创建矩阵。我们将按级别将它们存储在数组中，就像我们存储零件一样。为此添加一个 `Matrix4x4[][]` 字段，并在 `Awake` 中创建其所有数组以及其他数组。

```c#
	FractalPart[][] parts;

	Matrix4x4[][] matrices;

	void Awake () {
		parts = new FractalPart[depth][];
		matrices = new Matrix4x4[depth][];
		for (int i = 0, length = 1; i < parts.Length; i++, length *= 5) {
			parts[i] = new FractalPart[length];
			matrices[i] = new Matrix4x4[length];
		}

		…
	}
```

创建变换矩阵的最简单方法是调用静态 `Matrix4x4.TRS` 方法，并将位置、旋转和缩放作为参数。它返回一个 `Matrix4x4` 结构，我们可以将其复制到数组中。第一个是 `Update` 中的根矩阵，根据其世界位置、世界旋转和1的比例创建。

```c#
		parts[0][0] = rootPart;
		matrices[0][0] = Matrix4x4.TRS(
			rootPart.worldPosition, rootPart.worldRotation, Vector3.one
		);
```

> **TRS 是什么意思？**
>
> 它代表平移旋转比例。在这种情况下，翻译意味着重新定位或偏移。

在循环中以相同的方式创建所有其他矩阵，这次使用可变比例。

```c#
			scale *= 0.5f;
			FractalPart[] parentParts = parts[li - 1];
			FractalPart[] levelParts = parts[li];
			Matrix4x4[] levelMatrices = matrices[li];
			for (int fpi = 0; fpi < levelParts.Length; fpi++) {
				…
				
				levelMatrices[fpi] = Matrix4x4.TRS(
					part.worldPosition, part.worldRotation, scale * Vector3.one
				);
			}
```

此时进入游戏模式并不会向我们显示分形，因为我们还没有将零件可视化。但我们确实计算了它们的变换矩阵。如果我们让播放模式运行一段时间，分形深度为 6 或更大，Unity 将在某个时候开始记录错误。这些错误告诉 use 四元数到矩阵的转换失败，因为输入的四元数无效。

由于浮点精度限制，转换失败。当我们不断地将四元数相乘时，连续的微小误差会加剧，直到结果不再被识别为有效的旋转。它是由我们每次更新累积的许多非常小的旋转引起的。

解决方案是每次更新都从新的四元数开始。我们可以通过将自旋角作为单独的浮场存储在 `FractalPart` 中来实现这一点，而不是调整其局部旋转。

```c#
	struct FractalPart { 
		public Vector3 direction, worldPosition;
		public Quaternion rotation, worldRotation;
		public float spinAngle;
	}
```

在 `Update` 中，我们恢复到使用自旋增量角的旧方法，然后将其添加到根的自旋角中。根的世界旋转等于其配置的旋转，该旋转应用于围绕 Y 轴的新旋转之上，该新旋转等于其当前的旋转角度。

```c#
		//Quaternion deltaRotation = Quaternion.Euler(0f, 22.5f * Time.deltaTime, 0f);
		float spinAngleDelta = 22.5f * Time.deltaTime;
		FractalPart rootPart = parts[0][0];
		//rootPart.rotation *= deltaRotation;
		rootPart.spinAngle += spinAngleDelta;
		rootPart.worldRotation =
			rootPart.rotation * Quaternion.Euler(0f, rootPart.spinAngle, 0f);
		parts[0][0] = rootPart;
```

所有其他部分也是如此，它们的父级世界旋转应用在顶部。

```c#
				//part.rotation *= deltaRotation;
				part.spinAngle += spinAngleDelta;
				part.worldRotation =
					parent.worldRotation *
					(part.rotation * Quaternion.Euler(0f, part.spinAngle, 0f));
```

### 3.3 计算缓冲区

为了渲染这些部分，我们需要将它们的矩阵发送到 GPU。我们将为此使用计算缓冲区，就像我们为图所做的那样。不同之处在于，这次 CPU 将填充缓冲区，而不是 GPU，我们每层使用一个单独的缓冲区。为缓冲区数组添加一个字段，并在 `Awake` 中创建它们。4×4 矩阵有 16 个浮点值，因此缓冲区的步长是 16 乘以 4 字节。

```c#
	ComputeBuffer[] matricesBuffers;

	void Awake () {
		parts = new FractalPart[depth][];
		matrices = new Matrix4x4[depth][];
		matricesBuffers = new ComputeBuffer[depth];
		int stride = 16 * 4;
		for (int i = 0, length = 1; i < parts.Length; i++, length *= 5) {
			parts[i] = new FractalPart[length];
			matrices[i] = new Matrix4x4[length];
			matricesBuffers[i] = new ComputeBuffer(length, stride);
		}

		…
	}
```

我们还必须在新的 `OnDisable` 方法中释放缓冲区。要使此功能适用于热重新加载，请将 `Awake` 更改为 `OnEnable`。

```c#
	void OnEnable () {
		parts = new FractalPart[depth][];
		matrices = new Matrix4x4[depth][];
		matricesBuffers = new ComputeBuffer[depth];
		…
	}

	void OnDisable () {
		for (int i = 0; i < matricesBuffers.Length; i++) {
			matricesBuffers[i].Release();
		}
	}
```

为了保持整洁，还需要删除 `OnDisable` 末尾的所有数组引用。无论如何，我们都会在 `OnEnable` 中创建新的。

```c#
	void OnDisable () {
		for (int i = 0; i < matricesBuffers.Length; i++) {
			matricesBuffers[i].Release();
		}
		parts = null;
		matrices = null;
		matricesBuffers = null;
	}
```

这也使得在播放模式下通过检查器轻松支持更改分形深度成为可能，方法是添加一个 `OnValidate` 方法，该方法只需依次调用 `OnDisable` 和 `OnEnable`，重置分形。`OnValidate` 方法在通过检查器或撤消/重做操作对组件进行更改后被调用。

```c#
	void OnValidate () {
		OnDisable();
		OnEnable();
	}
```

然而，这只有在我们处于游戏模式并且分形当前处于活动状态时才能工作。我们可以通过 `!=` 不等运算符检查其中一个数组是否为 `null` 来验证这一点。

```c#
	void OnValidate () {
		if (parts != null) {
			OnDisable();
			OnEnable();
		}
	}
```

此外，如果我们通过检查器禁用组件，`OnValidate` 也会被调用。这将触发分形重置，然后再次禁用。我们还可以通过其 `enabled` 属性检查 `Fractal` 组件是否启用来避免这种情况。只有当这两个条件都为真时，我们才能重置分形。我们将这些检查组合在一起，使用布尔 `&&` AND 运算符形成一个条件表达式。

```c#
		if (parts != null && enabled) {
			OnDisable();
			OnEnable();
		}
```

最后，要将矩阵上传到 GPU，请在 `Update` 结束时调用所有缓冲区上的 `SetData`，并将相应的矩阵数组作为参数。

```c#
	void Update () {
		…

		for (int i = 0; i < matricesBuffers.Length; i++) {
			matricesBuffers[i].SetData(matrices[i]);
		}
	}
```

> **我们难道不应该避免向 GPU 发送数据吗？**
>
> 尽可能多，是的。但在这种情况下，我们别无选择，我们必须以某种方式将矩阵发送到 GPU，这是最有效的方法。

### 3.4 着色器

我们现在必须再次创建一个支持程序绘制的着色器。为了将对象设置为世界矩阵，我们可以从图的 *PointGPU.hlsl* 中获取代码，将其复制到新的 *FractalGPU.hlsl* 文件中，并使其适应我们的分形。这意味着它使用 `float4x4` 矩阵缓冲区，而不是 `float3` 位置缓冲区。我们可以直接复制矩阵，而不必在着色器中构造它。

```glsl
#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
	StructuredBuffer<float4x4> _Matrices;
#endif

//float _Step;

void ConfigureProcedural () {
	#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
		unity_ObjectToWorld = _Matrices[unity_InstanceID];
	#endif
}

void ShaderGraphFunction_float (float3 In, out float3 Out) {
	Out = In;
}

void ShaderGraphFunction_half (half3 In, out half3 Out) {
	Out = In;
}
```

我们分形的 URP 着色器图也是 *Point URP GPU* 着色器图的简化副本。顶点位置节点完全相同，除了我们现在必须依赖 *FractalGPU* HLSL 文件。而不是基于世界位置着色，一个单一的*基础颜色*属性就足够了。

![img](https://catlikecoding.com/unity/tutorials/basics/jobs/procedural-drawing/shader-graph.png)

*分形着色器图。*

BRP 曲面着色器也比其图形等效着色器更简单。它需要一个不同的名称，包括正确的文件，以及反照率的 *BaseColor* 颜色属性。颜色属性的工作原理类似于平滑度，除了使用 `Color` 而不是范围和四分量默认值。即使不再需要，我也在 `Input` 结构中保留了世界位置，因为空结构无法编译。

```glsl
Shader "Fractal/Fractal Surface GPU" {

	Properties {
		_BaseColor ("Base Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_Smoothness ("Smoothness", Range(0,1)) = 0.5
	}
	
	SubShader {
		CGPROGRAM
		#pragma surface ConfigureSurface Standard fullforwardshadows addshadow
		#pragma instancing_options assumeuniformscaling procedural:ConfigureProcedural
		#pragma editor_sync_compilation

		#pragma target 4.5
		
		#include "FractalGPU.hlsl"

		struct Input {
			float3 worldPos;
		};

		float4 _BaseColor;
		float _Smoothness;

		void ConfigureSurface (Input input, inout SurfaceOutputStandard surface) {
			surface.Albedo = _BaseColor.rgb;
			surface.Smoothness = _Smoothness;
		}
		ENDCG
	}

	FallBack "Diffuse"
}
```

### 3.5 绘图

最后，为了再次绘制分形，我们必须跟踪 `Fractal` 中矩阵缓冲区的标识符。

```c#
	static readonly int matricesId = Shader.PropertyToID("_Matrices");
```

然后在 `Update` 结束时调用 `Graphics.DrawMeshInstancedProcedural`，每个级别一次，使用正确的缓冲区。我们只需对所有级别使用相同的边界：边长为 3 的立方体。

```c#
		var bounds = new Bounds(Vector3.zero, 3f * Vector3.one);
		for (int i = 0; i < matricesBuffers.Length; i++) {
			ComputeBuffer buffer = matricesBuffers[i];
			buffer.SetData(matrices[i]);
			material.SetBuffer(matricesId, buffer);
			Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, buffer.count);
		}
```

> **为什么使用 3 作为边界大小？**
>
> 根的直径为 1。下一层的直径为 0.5，向各个方向延伸。因此，前两层的最大直径为 1+0.5+0.5=2。第三级在两侧增加 0.25，因此总直径变为 2.5，以此类推。因此，具有较大深度的分形的直径等于 $1+1+\frac{1}{2}+\frac 1 4+\frac 1 8+...=2+\sum\limits^n_{i=1}\frac 1 {2^i}$
>
> 对于具有无限深度的理论分形，总和会永远持续下去，但这是一个众所周知的收敛无穷序列：$\lim\limits_{n\rightarrow\infin}\sum\limits^n_{i=1}\frac 1{2^i}=1$。这很直观，因为从 0 到 1 的每一步都是你仍然需要走的距离的一半，所以你每一步的距离都会越来越近，但在有限的步数内永远不会达到 1。因此，我们的分形保证适合三个单位宽的边界框。

![img](https://catlikecoding.com/unity/tutorials/basics/jobs/procedural-drawing/only-deepest-level.png)

*只有最深的层次。*

我们的分形再次出现，但看起来只有最深的层次被渲染。帧调试器将显示所有级别都会被渲染，但它们都错误地使用了上一级别的矩阵。这是因为绘图命令会排队等待稍后执行。因此，我们最后设置的缓冲区是所有缓冲区都会使用的缓冲区。

解决方案是将每个缓冲区链接到特定的绘图命令。我们可以通过 `MaterialPropertyBlock` 对象来实现这一点。为它添加一个静态字段，并在 `OnEnable` 中创建一个新的实例（如果还不存在）。

```c#
	static MaterialPropertyBlock propertyBlock;

	…
	
	void OnEnable () {
		…

		if (propertyBlock == null) {
			propertyBlock = new MaterialPropertyBlock();
		}
	}
```

仅当当前值为空时分配某物可以通过使用 `??=` 简化为单个表达式空合并分配。

```c#
		//if (propertyBlock == null) {
		propertyBlock ??= new MaterialPropertyBlock();
		//}
```

在 `Update` 中，将缓冲区设置在特性块上，而不是直接设置在材质上。然后将该块作为附加参数传递给 `Graphics.DrawMeshInstancedProcedural`。这将使 Unity 复制块当时的配置，并将其用于特定的绘图命令，推翻为材质设置的配置。

```c#
			ComputeBuffer buffer = matricesBuffers[i];
			buffer.SetData(matrices[i]);
			propertyBlock.SetBuffer(matricesId, buffer);
			Graphics.DrawMeshInstancedProcedural(
				mesh, 0, material, bounds, buffer.count, propertyBlock
			);
		}
```

> **为什么场景窗口中的分形会闪烁？**
>
> 这是一个编辑器定时错误，可能发生在场景窗口中，但不会发生在游戏窗口或构建中。打开游戏窗口的 VSync 可以使它变得更好或更坏，具体取决于您的编辑器布局。

### 3.6 性能

现在我们的分形再次完成，让我们再次测量它的性能，最初是在渲染球体时。

![img](https://catlikecoding.com/unity/tutorials/basics/jobs/procedural-drawing/profiler-build-urp-depth-6.png)

*使用 URP 和分形深度 6 分析构建。*

| 深度 | MS   | URP  | BRP  |
| ---- | ---- | ---- | ---- |
| 6    | 0.7  | 160  | 96   |
| 7    | 3    | 54   | 35   |
| 8    | 13   | 11   | 7    |

现在 `Update` 需要更少的时间。在 FPS 方面，深度 6 和 8 略有改善，而深度 7 的渲染速度几乎是以前的两倍。URP 接近 60FPS。当我们尝试立方体时，我们看到了显著的改进。

| 深度 | MS   | URP  | BRP  |
| ---- | ---- | ---- | ---- |
| 6    | 0.4  | 470  | 175  |
| 7    | 2    | 365  | 155  |
| 8    | 11   | 86   | 85   |

帧率大幅提高，这清楚地表明 GPU 现在是瓶颈。更新时间也减少了一点。这可能是因为在渲染球体时，设置缓冲区数据更加停滞，因为 CPU 被迫等待 GPU 完成从缓冲区的读取。

### 3.7 移动游戏对象

创建我们自己的变换矩阵的一个副作用是，我们的分形现在忽略了其游戏对象的变换。我们可以通过在 `Update` 中将游戏对象的旋转和位置合并到根对象矩阵中来解决这个问题。

```c#
		rootPart.worldRotation =
			transform.rotation *
			(rootPart.rotation * Quaternion.Euler(0f, rootPart.spinAngle, 0f));
		rootPart.worldPosition = transform.position;
```

我们还可以应用游戏对象的比例。然而，如果游戏对象是包含非均匀比例和旋转的复杂层次结构的一部分，它可能会受到非仿射变换的影响，从而导致剪切。在这种情况下，它没有明确的规模。因此，`Transform` 组件没有简单的世界空间比例属性。它们具有 `lossyScale` 属性，以表明它可能不是一个精确的仿射尺度。我们将简单地使用该比例的 X 分量，忽略任何不均匀的比例。

```c#
		float objectScale = transform.lossyScale.x;
		matrices[0][0] = Matrix4x4.TRS(
			rootPart.worldPosition, rootPart.worldRotation, objectScale * Vector3.one
		);

		float scale = objectScale;
```

还将调整后的世界位置和比例应用于边界。

```c#
		var bounds = new Bounds(rootPart.worldPosition, 3f * objectScale * Vector3.one);
```

## 4 工作系统

目前，我们的 C# 代码已经快到了极限，但我们可以利用 Unity 的作业系统切换到另一种方法。这应该能够进一步减少我们的 `Update` 时间，提高性能或为更多代码的运行腾出空间，而不会减慢速度。

作业系统的想法是尽可能有效地利用 CPU 的并行处理能力，利用其多核和特殊的 SIMD 指令，即单指令多数据。这是通过将工作定义为单独的作业来实现的。这些作业的编写方式类似于常规 C# 代码，但随后使用 Unity 的 Burst 编译器进行编译，该编译器通过强制执行常规 C# 没有的一些结构约束来执行积极的优化和并行化。

### 4.1 Burst 包

Burst 作为一个单独的包存在，因此请通过包管理器安装 Unity 版本的最新版本。就我而言，这是 Burst 1.4.8 版本。它依赖于数学包版本 1.2.1，因此该包也将被安装或升级到版本 1.2.1。

要为 `Fractal` 创建作业，我们必须使用  `Unity.Burst`, `Unity.Collections` 和 `Unity.Jobs` 中的代码。

```c#
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
```

### 4.2 本机数组

作业不能处理对象，只允许使用简单值和结构类型。仍然可以使用数组，但我们必须将它们转换为通用的 `NativeArray` 类型。这是一个包含指向本机内存的指针的结构体，本机内存存在于 C# 代码使用的常规托管内存堆之外。因此，它避开了默认的内存管理开销。

要创建分形部分的原生数组，我们需要使用 `NativeArray<FractalPart>` 类型。当我们使用多个这样的数组时，我们真正需要的是一个数组。对于多个矩阵数组也是如此。

```c#
	NativeArray<FractalPart>[] parts;

	NativeArray<Matrix4x4>[] matrices;
```

我们现在必须在 `OnEnable` 开始时创建新的本机数组数组。

```c#
		parts = new NativeArray<FractalPart>[depth];
		matrices = new NativeArray<Matrix4x4>[depth];
```

并使用相应 `NativeArray` 类型的构造函数方法为每个级别创建新的本机数组，该方法需要两个参数。第一个参数是数组的大小。第二个参数指示本机数组预计存在多长时间。由于我们每帧都使用相同的数组，因此必须使用 `Allocator.Persistent`。

```c#
		for (int i = 0, length = 1; i < parts.Length; i++, length *= 5) {
			parts[i] = new NativeArray<FractalPart>(length, Allocator.Persistent);
			matrices[i] = new NativeArray<Matrix4x4>(length, Allocator.Persistent);
			matricesBuffers[i] = new ComputeBuffer(length, stride);
		}
```

我们还必须更改零件创建循环中的变量类型以匹配。

```c#
		parts[0][0] = CreatePart(0);
		for (int li = 1; li < parts.Length; li++) {
			NativeArray<FractalPart> levelParts = parts[li];
			…
		}
```

在 `Update` 内部的循环中也是如此。

```c#
			NativeArray<FractalPart> parentParts = parts[li - 1];
			NativeArray<FractalPart> levelParts = parts[li];
			NativeArray<Matrix4x4> levelMatrices = matrices[li];
```

最后，就像计算缓冲区一样，我们必须在 `OnDisable` 中明确地释放它们的内存。我们通过在本机数组上调用 `Dispose` 来实现这一点。

```c#
		for (int i = 0; i < matricesBuffers.Length; i++) {
			matricesBuffers[i].Release();
			parts[i].Dispose();
			matrices[i].Dispose();
		}
```

在这一点上，分形仍然是一样的。唯一的区别是，我们现在使用的是本机数组，而不是托管 C# 数组。这可能会表现得更糟，因为从托管 C# 代码访问本机数组会有一点额外的开销。一旦我们使用 Burst 编译的作业，这种开销就不会存在。

### 4.3 工作结构

要定义作业，我们必须创建一个实现作业接口的结构类型。实现一个接口就像扩展一个类，除了接口要求你自己包含特定的功能，而不是继承现有的功能。我们将在 `Fractal` 中创建一个 `UpdateFractalLevelJob` 结构，该结构实现了 `IJobFor`，这是最灵活的作业接口类型。

```c#
public class Fractal : MonoBehaviour {
	
	struct UpdateFractalLevelJob : IJobFor {}
	
	…
}
```

> **为什么接口命名为 `IJobFor`？**
>
> 惯例是在所有接口类型前加上代表接口的 *I*，因此该接口名为 *JobFor*，前缀为 *I*。这是一个作业界面，特别是用于 `for` 循环内部运行的功能。

`IJobFor` 接口要求我们添加一个 `Execute` 方法，该方法有一个整数参数并且不返回任何值。该参数表示 `for` 循环的迭代器变量。接口强制执行的所有内容都必须是公共的，因此此方法必须是公开的。

```c#
	struct UpdateFractalLevelJob : IJobFor {

		public void Execute (int i) {}
	}
```

这个想法是 `Execute` 方法替换了 `Update` 方法最内层循环的代码。为了实现这一点，必须将该代码所需的所有变量作为字段添加到 `UpdateFractalLevelJob` 中。将其公开，以便我们稍后设置。

```c#
	struct UpdateFractalLevelJob : IJobFor {

		public float spinAngleDelta;
		public float scale;

		public NativeArray<FractalPart> parents;
		public NativeArray<FractalPart> parts;

		public NativeArray<Matrix4x4> matrices;

		public void Execute (int i) {}
	}
```

我们可以更进一步，使用 `ReadOnly` 和 `WriteOnly` 属性来指示我们只需要部分访问一些本机数组。最里面的循环只从父数组读取，只写入矩阵数组。它既可以从 parts 数组读取数据，也可以向 parts 数组写入数据，这是默认假设，因此没有相应的属性。

```c#
		[ReadOnly]
		public NativeArray<FractalPart> parents;

		public NativeArray<FractalPart> parts;

		[WriteOnly]
		public NativeArray<Matrix4x4> matrices;
```

如果多个进程并行修改相同的数据，那么谁先做什么就变得任意了。如果两个进程设置了相同的数组元素，则最后一个进程获胜。如果一个进程得到的元素与另一个进程设置的元素相同，它要么得到旧值，要么得到新值。最终结果取决于我们无法控制的确切时间，这可能会导致难以检测和修复的不一致行为。这些现象被称为竞态条件。`ReadOnly` 属性表示此数据在作业执行期间保持不变，这意味着进程可以安全地并行读取，因为结果始终相同。

编译器强制作业不写入 `ReadOnly` 数据，也不从 `WriteOnly` 数据读取。如果我们无意中这样做，编译器会让我们知道我们犯了语义错误。

### 4.4 正在执行作业

`Execute` 方法将替换 `Update` 方法的最内层循环。将相关代码复制到方法中，并在需要时对其进行调整，使其使用作业的字段和参数。

```c#
		public void Execute (int i) {
			FractalPart parent = parents[i / 5];
			FractalPart part = parts[i];
			part.spinAngle += spinAngleDelta;
			part.worldRotation =
				parent.worldRotation *
				(part.rotation * Quaternion.Euler(0f, part.spinAngle, 0f));
			part.worldPosition =
				parent.worldPosition +
				parent.worldRotation * (1.5f * scale * part.direction);
			parts[i] = part;

			matrices[i] = Matrix4x4.TRS(
				part.worldPosition, part.worldRotation, scale * Vector3.one
			);
		}
```

更改 `Update`，以便我们创建一个新的 `UpdateFractalLevelJob` 值，并在级别循环中设置其所有字段。然后更改最内层的循环，使其调用作业的 `Execute` 方法。这样，我们保持了完全相同的功能，但代码迁移到了作业中。

```c#
		for (int li = 1; li < parts.Length; li++) {
			scale *= 0.5f;
			var job = new UpdateFractalLevelJob {
				spinAngleDelta = spinAngleDelta,
				scale = scale,
				parents = parts[li - 1],
				parts = parts[li],
				matrices = matrices[li]
			};
			//NativeArray<FractalPart> parentParts = parts[li - 1];
			//NativeArray<FractalPart> levelParts = parts[li];
			//NativeArray<Matrix4x4> levelMatrices = matrices[li];
			for (int fpi = 0; fpi < parts[li].Length; fpi++) {
				job.Execute(fpi);
			}
		}
```

但我们不必为每次迭代都显式调用 `Execute` 方法。这个想法是，我们安排作业，让它自己执行循环。这是通过调用带有两个参数的 `Schedule` 来实现的。第一个是我们想要多少次迭代，这等于我们正在处理的零件数组的长度。第二个是 `JobHandle` 结构值，用于强制作业之间的顺序依赖关系。我们最初将使用此结构的默认值——通过 `default` 关键字——它不强制任何约束。

```c#
			var job = new UpdateFractalLevelJob {
				…
			};
			job.Schedule(parts[li].Length, default);
			//for (int fpi = 0; fpi < parts[li].Length; fpi++) {
			//	job.Execute(fpi);
			//}
```

`Schedule` 不会立即运行作业，它只会为以后的处理调度作业。它返回一个 `JobHandle` 值，可用于跟踪作业的进度。我们可以通过在句柄上调用 `Complete` 来延迟代码的进一步执行，直到作业完成。

```c#
			job.Schedule(parts[li].Length, default).Complete();
```

### 4.5 行程安排

此时，我们正在安排并立即等待每个级别的作业完成。结果是，即使我们已经切换到使用作业，我们的分形仍然像以前一样以顺序的方式更新。我们可以通过将完成时间推迟到我们安排好所有作业之后来稍微放松一下。我们通过使作业相互依赖来实现这一点，在安排时将最后一个作业句柄传递给下一个作业。然后，我们在完成循环后调用 `Complete`，这会触发整个作业序列的执行。

```c#
		JobHandle jobHandle = default;
		for (int li = 1; li < parts.Length; li++) {
			scale *= 0.5f;
			var job = new UpdateFractalLevelJob {
				…
			};
			//job.Schedule(parts[li].Length, default).Complete();
			jobHandle = job.Schedule(parts[li].Length, jobHandle);
		}
		jobHandle.Complete();
```

此时，我们不再需要将单个作业存储在变量中，只需要跟踪最后一个句柄。

```c#
			jobHandle = new UpdateFractalLevelJob {
				…
			}.Schedule(parts[li].Length, jobHandle);
			//jobHandle = job.Schedule(parts[li].Length, jobHandle);
```

分析器将向我们显示作业最终可以在工作线程而不是主线程上运行。但它们也可能在主线程上运行，因为主线程无论如何都需要等待作业完成，所以此时作业运行的位置没有什么不同。计划作业也可能最终在不同的线程上运行，尽管它们仍然具有顺序依赖关系。

![img](https://catlikecoding.com/unity/tutorials/basics/jobs/job-system/scheduled-on-worker-thread.png)

*使用 URP 和分形深度 8 分析构建；主线程等待工作线程完成。*

将所有作业捆绑在一起运行，只等待最后一个作业完成的好处是，这可以延迟等待完成。一个常见的例子是在 `Update` 中安排所有作业，做一些其他事情，并通过在 `LateUpdate` 方法中这样做来延迟调用  `Complete`，`LateUpdate` 方法在所有常规 `Update` 方法完成后调用。也可以将完成时间推迟到下一帧，甚至更晚。但我们不会这样做，因为我们需要完成每一帧的任务，除了之后将矩阵上传到 GPU 外，没有其他事情可做。

### 4.6 Burst 编译

在所有这些变化之后，我们还没有看到任何性能改进。这是因为我们目前没有使用Burst编译器。我们必须通过将 `BurstCompile` 属性附加到 Unity 来明确指示 Unity 使用 Burst 编译我们的作业结构体。

```c#
	[BurstCompile]
	struct UpdateFractalLevelJob : IJobFor { … }
```

![img](https://catlikecoding.com/unity/tutorials/basics/jobs/job-system/burst-compiled.png)

*用 Burst 编译。*

帧调试器现在指示我们正在使用作业的 Burst 编译版本。由于 Burst 应用了优化，它们运行得更快一些，但目前还没有太大收益。

您可能会注意到，在编辑器中进入播放模式后，性能要差得多。这是因为 Burst 编译是在编辑器中按需进行的，就像着色器编译一样。当作业首次运行时，它将由 Burst 编译，同时使用常规 C# 编译版本运行作业。Burst 编译完成后，编辑器将切换到运行 Burst 版本。我们可以通过将 `BurstCompile` 属性的 `CompileSynchronously` 属性设置为 `true`，强制编辑器在需要时立即编译作业的 Burst 版本——暂停 Unity 直到编译完成。属性的属性可以通过将其赋值包含在参数列表中来设置。

```c#
	[BurstCompile(CompileSynchronously = true)]
```

就像着色器编译一样，这不会影响构建，因为所有内容都是在构建过程中编译的。

### 4.7 Burst 检视栏

您可以通过 *Jobs/Burst/Open Inspector...* 打开的 *Burst 检查器*窗口检查 Burst 生成的汇编代码。这向您展示了 Burst 为项目中的所有作业生成的低级指令。我们的工作将作为 *Fractal.UpdateFractalLevelJob - (IForJob)* 被列入*编译目标*列表。

我不会详细分析生成的代码，性能改进必须不言而喻。但切换到最右侧的显示模式是有用的，即 *.LVM IR 优化诊断*，了解 Burst 的功能。目前，它包含了我的以下评论：

```
Remark: IJobFor.cs:43:0: loop not vectorized: loop control flow is not understood by vectorizer
Remark: NativeArray.cs:162:0: Stores SLP vectorized with cost -3 and with tree size 2
```

第一句话意味着 Burst 无法使用 SIMD 指令重写代码以合并多个迭代。最简单的例子是一个类似 `data[i] = 2f * data[i]` 的作业。使用 SIMD 指令，Burst 可以改变这一点，因此可以一次对多个索引执行此操作，在理想情况下最多可以同时对八个索引执行。以这种方式合并操作被称为矢量化，因为单个值上的指令被矢量上的指令所取代。

当 Burst 指示不理解控制流时，这意味着存在复杂的条件块。我们没有这些，但默认情况下启用了 Burst 安全检查，该检查强制执行读/写属性并检测作业之间的其他依赖性问题，例如尝试并行运行两个写入同一数组的作业。这些检查用于开发，并从构建中删除。我们还可以通过禁用*安全检查*开关来停用它们，以便 Burst 检查器查看最终结果。您还可以通过*作业/突发/安全检查*菜单为每个作业或整个项目禁用它们。您通常会在编辑器中启用安全检查，并在构建中测试性能，除非您想最大限度地提高编辑器性能。

```
Remark: IJobFor.cs:43:0: loop not vectorized: call instruction cannot be vectorized
Remark: NativeArray.cs:148:0: Stores SLP vectorized with cost -3 and with tree size 2
```

如果没有安全检查，Burst 仍然无法对循环进行矢量化，这一次是因为调用指令会妨碍。这意味着有一个Burst无法优化的方法调用，它永远无法被矢量化。

第二句话表明，Burst 找到了一种将多个独立操作矢量化为单个 SIMD 指令的方法。例如，多个独立值的加法被合并为一个向量加法。-3 的成本表明，这有效地消除了三条指令。

> **SLP 是什么意思？**
>
> 这是超词级并行性（superword-level parallelism）的简写。

### 4.8 数学库

我们目前使用的代码没有针对 Burst 进行优化。Burst 无法优化的调用指令对应于我们调用的静态 `Quaternion` 方法。Burst 专门针对 Unity 的数学库进行了优化，该库在设计时考虑了矢量化。

数学库代码包含在 `Unity.Mathematics` 命名空间中。

```c#
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
```

该库的设计类似于着色器数学代码。这个想法是静态使用 `Unity.Mathematics.math` 类型，就像我们静态使用函数库中的 `UnityEngine.Mathf` 用于我们的图。

```c#
using Unity.Mathematics;
using UnityEngine;

using static Unity.Mathematics.math;
```

然而，当尝试调用 `float4x4` 和 `quaternion` 类型上的某些方法时，这将导致冲突，因为 `math` 的方法与这些类型的名称完全相同。这将使编译器抱怨我们试图调用方法上的方法，这是不可能的。为了避免这种情况，添加 `using` 语句来指示当我们编写这些单词时，默认情况下它们应该被解释为类型。这是通过 `using` 后跟标签、赋值和完全限定类型来编写的。我们只是使用标签的类型名称，尽管也可以使用不同的标签。

```c#
using static Unity.Mathematics.math;
using float4x4 = Unity.Mathematics.float4x4;
using quaternion = Unity.Mathematics.quaternion;
```

现在，将 `Vector3` 的所有用法替换为 `float3`，除了我们在 `Update` 中用于缩放边界的向量。我不会列出所有这些更改，我们稍后会修复错误。然后用四元数替换所有四元数的用法。请注意，唯一的区别是数学类型没有大写。之后，将 `Matrix4x4` 的所有用法替换为 `float4x4`。

完成后，用 `math` 中的相应方法替换 `directions` 数组的矢量方向属性。

```c#
	static float3[] directions = {
		up(), right(), left(), forward(), back()
	};
```

我们还必须调整 `rotations` 数组的初始化。数学库使用弧度而不是度数，因此将 `90f` 的所有实例更改为 `0.5f * PI`。此外，四元数有单独的方法来创建围绕 X、Y或 Z 轴的旋转，这些方法比通用的 `Euler` 方法更有效。

```c#
	static quaternion[] rotations = {
		quaternion.identity,
		quaternion.RotateZ(-0.5f * PI), quaternion.RotateZ(0.5f * PI),
		quaternion.RotateX(0.5f * PI), quaternion.RotateX(-0.5f * PI)
	};
```

我们还必须将更新中的自旋角增量转换为弧度。

```c#
		float spinAngleDelta = 0.125f * PI * Time.deltaTime;
```

下一步是调整 `UpdateFractalLevelJob.Execute`。首先，用更快的 `RotateY` 变体替换 `Euler` 方法调用。然后用 `mul` 方法的调用替换所有涉及四元数的乘法。最后，我们可以通过调用 `math.float3` 方法，将 scale 作为单个参数来创建一个统一的 scale 向量。

```c#
			part.worldRotation = mul(parent.worldRotation,
				mul(part.rotation, quaternion.RotateY(part.spinAngle))
			);
			part.worldPosition =
				parent.worldPosition +
				mul(parent.worldRotation, 1.5f * scale * part.direction);
			parts[i] = part;

			matrices[i] = float4x4.TRS(
				part.worldPosition, part.worldRotation, float3(scale)
			);
```

以相同的方式调整 `Update` i 中根部分的更新代码。

```c#
		rootPart.worldRotation = mul(transform.rotation,
			mul(rootPart.rotation, quaternion.RotateY(rootPart.spinAngle))
		);
		rootPart.worldPosition = transform.position;
		parts[0][0] = rootPart;
		float objectScale = transform.lossyScale.x;
		matrices[0][0] = float4x4.TRS(
			rootPart.worldPosition, rootPart.worldRotation, float3(objectScale)
		);
```

> **`Transform` 位置和旋转的类型是否错误？**
>
> 确实如此，但 `Vector3` 和 `float3` 类型之间以及 `Quaternion` 和 `quaternion` 类型之间存在隐式转换。

此时，Burst 检查员将不再抱怨调用指令。它仍然无法对循环进行矢量化，因为返回类型不能被矢量化。这是因为我们的数据太大，无法对循环的多次迭代进行矢量化。这很好，Burst 仍然可以对单个迭代的许多操作进行矢量化，因为我们使用了 Mathematics 库，尽管 Burst 检查器不会提及这一点。

```
Remark: quaternion.cs:330:0: loop not vectorized: instruction return type cannot be vectorized
Remark: NativeArray.cs:162:0: Stores SLP vectorized with cost -6 and with tree size 2
```

此时，对于深度为 8 的分形，更新在构建中平均只需要 5.5 毫秒。因此，与非作业方法相比，我们的更新速度大约翻了一番。通过向 `BurstCompile` 构造函数方法传递两个参数，启用更多的 Burst 优化，我们可以走得更快。这些是常规构造函数参数，必须在属性赋值之前编写。

我们将使用 `FloatPrecision.Standard` 作为第一个参数和 `FloatMode.Fast` 作为第二个参数。快速模式允许 Burst 对数学运算进行重新排序，例如将 `a + b * c` 重写为 `b * c + a`。这可以提高性能，因为有 madd ——乘法-加法——指令，比单独的加法指令和乘法更快。默认情况下，着色器编译器会执行此操作。通常，重新排序操作在逻辑上没有区别，但由于浮点限制，更改顺序会产生略有不同的结果。你可以假设这些差异并不重要，所以除非你有充分的理由不这样做，否则一定要启用这种优化。

```c#
	[BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
```

结果是更新持续时间进一步缩短，平均降至约 4.5ms。

> **`FloatPrecision` 怎么样？**
>
> `FloatPrecision` 参数控制 `sin` 和 `cos` 方法的精度。我们不直接使用它们，但在创建四元数时会使用它们。降低三角精度可以加快速度，但对我来说并没有明显的区别。

### 4.9 发送更少的数据

变换矩阵的最后一行总是包含相同的向量：（0,0,0,1）。由于它总是相同的，我们可以丢弃它，将矩阵数据的大小减少 25%。这意味着内存使用量减少，从 CPU 到 GPU 的数据传输也减少。

首先，将 `float4x4` 的所有用法替换为 `float3x4`，它表示一个有三行四列的矩阵。然后在 `OnEnable` 中将计算缓冲区的步幅从 16 个浮点数减小到 12 个浮点数。

```c#
		int stride = 12 * 4;
```

`float3x4` 没有 `TRS` 方法，我们必须在 `Execute` 中自己组装矩阵。这是通过首先创建一个用于旋转和缩放的 3×3 矩阵，通过调用带有旋转的 `float3x3`，然后将缩放因子分解到其中来实现的。最后一个矩阵是通过调用带有四列向量的 `float3x4` 来创建的，这四列向量是 3×3 矩阵的三列，存储在其 `c0`、`c1` 和 `c2` 字段中，后面是零件的位置。

```c#
			float3x3 r = float3x3(part.worldRotation) * scale;
			matrices[i] = float3x4(r.c0, r.c1, r.c2, part.worldPosition);
```

在 `Update` 中对根部分执行相同的操作。

```c#
		float3x3 r = float3x3(rootPart.worldRotation) * objectScale;
		matrices[0][0] = float3x4(r.c0, r.c1, r.c2, rootPart.worldPosition);
```

由于我们没有在 `float3x4` 类型上调用方法，因此与 `math.float3x4` 方法没有冲突，因此我们不需要为它或 `float4x4` 使用 `using` 语句。

```c#
//using float3x4 = Unity.Mathematics.float3x4;
```

最后，调整 `ConfigureProcedural`，使我们逐行复制矩阵，并添加缺少的矩阵。

```glsl
#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
	StructuredBuffer<float3x4> _Matrices;
#endif

void ConfigureProcedural () {
	#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
		float3x4 m = _Matrices[unity_InstanceID];
		unity_ObjectToWorld._m00_m01_m02_m03 = m._m00_m01_m02_m03;
		unity_ObjectToWorld._m10_m11_m12_m13 = m._m10_m11_m12_m13;
		unity_ObjectToWorld._m20_m21_m22_m23 = m._m20_m21_m22_m23;
		unity_ObjectToWorld._m30_m31_m32_m33 = float4(0.0, 0.0, 0.0, 1.0);
	#endif
}
```

更改后，我的平均 `Update` 持续时间缩短到 4ms。因此，我只存储和传输了最少量的数据，就获得了半毫秒的时间。

### 4.10 使用多核

我们已经达到了单个 CPU 核心的优化终点，但我们还可以更进一步。更新图时，所有父部分都必须在更新子部分之前进行更新，因此我们无法摆脱作业之间的顺序依赖关系。但同一级别的所有部分都是独立的，可以按任何顺序更新，甚至可以并行更新。这意味着我们可以将单个作业的工作分散到多个 CPU 核上。这是通过在作业上调用 `ScheduleParallel` 而不是 `Schedule` 来实现的。此方法需要一个新的第二个参数，用于指示批计数。让我们先将其设置为 1，看看会发生什么。

```c#
			jobHandle = new UpdateFractalLevelJob {
				…
			}.ScheduleParallel(parts[li].Length, 1, jobHandle);
```

![img](https://catlikecoding.com/unity/tutorials/basics/jobs/job-system/multiple-threads.png)

*在多个线程上运行。*

我们的作业现在被分解并在多个 CPU 核上运行，这些 CPU 核并行更新我们的分形部分。在我的例子中，这将总更新时间平均缩短到 1.9ms。减少多少取决于有多少 CPU 核可用，这受到硬件的限制，以及有多少其他进程占用了一个线程。

批计数控制迭代如何分配给线程。每个线程循环一批，执行一点簿记，然后循环另一批，直到工作完成。经验法则是，当 `Execute` 做的工作很少时，你应该尝试大批量计数，而当 `Execute` 做了很多工作时，你可以尝试小批量计数。在我们的例子中，`Execute` 做了很多工作，因此批处理计数 1 是一个合理的默认值。但是，当我们给每个部分 5 个孩子时，让我们试试批处理计数为 5。

```c#
			jobHandle = new UpdateFractalLevelJob {
				…
			}.ScheduleParallel(parts[li].Length, 5, jobHandle);
```

这进一步将我的平均 `Update` 时间缩短到 1.7ms。使用更大的批次计数并没有进一步改善情况，甚至让它有点慢，所以我把它保持在 5。

### 4.11 最终性能

如果我们现在评估我们完全 Burst 优化的分形的性能，我们会发现更新持续时间变得微不足道。GPU 始终是瓶颈。渲染球体时，我们不会得到比以前更高的帧率。但是，在渲染立方体时，我们现在使用深度为 8 的分形的两个 RP 都超过了 100FPS。

| 深度 | MS   | URP  | BRP  |
| ---- | ---- | ---- | ---- |
| 6    | 0.1  | 480  | 180  |
| 7    | 0.26 | 360  | 160  |
| 8    | 1.7  | 160  | 110  |

下一个教程是[有机多样性](https://catlikecoding.com/unity/tutorials/basics/organic-variety/)。

[license](https://catlikecoding.com/unity/tutorials/license/)

[repository](https://bitbucket.org/catlikecodingunitytutorials/basics-06-jobs/)

[PDF](https://catlikecoding.com/unity/tutorials/basics/jobs/Jobs.pdf)



# 有机多样性：使人工变得自然

发布于 2021-01-20 更新于 2021-05-18

https://catlikecoding.com/unity/tutorials/basics/organic-variety/

*根据深度为分形着色。*
*应用基于随机序列的品种。*
*介绍看起来不同的叶子。*
*使分形像重力一样下垂。*
*增加旋转的多样性，有时会反转。*

这是关于学习使用 Unity [基础](https://catlikecoding.com/unity/tutorials/basics/)知识的系列教程中的第七篇。在文中，我们将调整分形，使其看起来更自然，而不是数学。

本教程使用 Unity 2020.3.6f1 编写。

![img](https://catlikecoding.com/unity/tutorials/basics/organic-variety/tutorial-image.jpg)

*一种经过修改的分形，看起来很有机。*

## 1 颜色渐变

我们在[上一教程](https://catlikecoding.com/unity/tutorials/basics/jobs/)中创建的分形显然是应用数学的结果。它看起来僵硬、精确、正式和统一。它看起来不是有机的，也不是活的。然而，在一定程度上，通过一些改变，我们可以使数学看起来有机。我们通过引入多样性和明显的随机性，以及模拟某种有机行为来实现这一点。

使分形更加多样化的最直接方法是用一系列颜色替换其均匀颜色，最简单的方法是基于每个绘制实例的级别。

### 1.1 重写颜色

我们之前为 BRP 表面着色器和 URP 都提供了一个*基础颜色*属性，我们目前通过调整材质来配置该属性，但我们可以通过代码覆盖它。为此，请在 `Fractal` 中跟踪其标识符。

```c#
	static readonly int
		baseColorId = Shader.PropertyToID("_BaseColor"),
		matricesId = Shader.PropertyToID("_Matrices");
```

然后在 `Update` 内部的绘图循环中的属性块上调用 `SetColor`。我们首先将颜色设置为白色，乘以当前循环迭代器值除以缓冲区长度减 1。这将使第一级为黑色，最后一级为白色。

```c#
		for (int i = 0; i < matricesBuffers.Length; i++) {
			ComputeBuffer buffer = matricesBuffers[i];
			buffer.SetData(matrices[i]);
			propertyBlock.SetColor(
				baseColorId, Color.white * (i / (matricesBuffers.Length - 1))
			);
			propertyBlock.SetBuffer(matricesId, buffer);
			Graphics.DrawMeshInstancedProcedural(
				mesh, 0, material, bounds, buffer.count, propertyBlock
			);
		}
```

为了给所有中间级别一个灰色阴影，这必须是浮点除法，而不是没有小数部分的整数除法。我们可以通过将除数中的 1 相减作为浮点减法来确保这一点。然后，计算的其余部分也变为浮点运算。

```c#
			propertyBlock.SetColor(
				baseColorId, Color.white * (i / (matricesBuffers.Length - 1f))
			);
```

如果使用 URP，请确保颜色的参考设置为 *_BaseColor*。

![img](https://catlikecoding.com/unity/tutorials/basics/organic-variety/color-gradient/shader-graph-property-reference.png)

*带有 _BaseColor 的着色器图。*

结果是一个灰度分形，在 BRP 和 URP 中，从根实例的黑色到叶实例的白色。

![img](https://catlikecoding.com/unity/tutorials/basics/organic-variety/color-gradient/gradient-fractal-grayscale.png)

*灰度梯度分形。*

请注意，需要在除数中减去 1 才能达到最深层的白色。但是，如果分形的深度设置为 1，这将导致除以零，从而产生无效的颜色。为了避免这种情况，我们应该将最小深度增加到 2。

```c#
	[SerializeField, Range(2, 8)]
	int depth = 4;
```

### 1.2 颜色之间的插值

我们不仅限于灰度或单色渐变。我们可以通过我们之前用作插值器的因子调用静态 `Color.Lerp` 在任何两种颜色之间进行插值。这样我们就可以在 `Update` 中创建任何双色渐变，例如从黄色到红色。

```c#
			propertyBlock.SetColor(
				baseColorId, Color.Lerp(
					Color.yellow, Color.red, i / (matricesBuffers.Length - 1f)
				)
			);
```

![img](https://catlikecoding.com/unity/tutorials/basics/organic-variety/color-gradient/gradient-fractal-yellow-green.png)

*黄红色梯度分形。*

### 1.3 可配置梯度

我们可以更进一步，支持任意渐变，这些渐变可以有两种以上的配置颜色，也可以有不均匀的分布。这是通过依赖 Unity 的 `Gradient` 类型来实现的。使用它为 `Fractal` 添加可配置的渐变。

```c#
	[SerializeField]
	Material material;

	[SerializeField]
	Gradient gradient;
```

![img](https://catlikecoding.com/unity/tutorials/basics/organic-variety/color-gradient/gradient-property.png)

*渐变属性，设置为白-红-黑。*

要使用渐变，请替换对 `Color.Lerp` 的调用。在 `Update` 中调用 `Evaluate` 对梯度进行更新，同样使用相同的插值器值。

```c#
			propertyBlock.SetColor(
				baseColorId, gradient.Evaluate(i / (matricesBuffers.Length - 1f))
			);
```

![img](https://catlikecoding.com/unity/tutorials/basics/organic-variety/color-gradient/gradient-fractal-configurable.png)

*白-红-黑可配置梯度分形。*

## 2 任意颜色

用渐变着色的分形看起来比用均匀颜色着色的分形更有趣，但它的着色显然是公式化的。有机物的颜色通常是随机的，或者看起来是随机的。对于我们的分形，这意味着单个网格实例应该表现出各种颜色。

### 2.1 颜色着色器功能

为了同时为我们的表面着色器和着色器图做工作，我们将通过 *FractalGPU* HLSL 文件提供实例颜色。首先在其中声明 *_BaseColor* 属性字段，然后是一个只返回该字段的 `GetFractalColor` 函数。将其放置在着色器图函数上方。

```glsl
float4 _BaseColor;

float4 GetFractalColor () {
	return _BaseColor;
}

void ShaderGraphFunction_float (float3 In, out float3 Out) {
	Out = In;
}
```

然后从曲面着色器中删除现在冗余的属性，并在 `ConfigureSurface` 中调用 `GetFractalColor`，而不是直接访问该字段。

```glsl
		//float4 _BaseColor;
		float _Smoothness;

		void ConfigureSurface (Input input, inout SurfaceOutputStandard surface) {
			surface.Albedo = GetFractalColor().rgb;
			surface.Smoothness = _Smoothness;
		}
```

然后从曲面着色器中删除现在冗余的属性，并在 `ConfigureSurface` 中调用 `GetFractalColor`，而不是直接访问该字段。

```glsl
		//float4 _BaseColor;
		float _Smoothness;

		void ConfigureSurface (Input input, inout SurfaceOutputStandard surface) {
			surface.Albedo = GetFractalColor().rgb;
			surface.Smoothness = _Smoothness;
		}
```

由于我们不再依赖材质检查器来配置颜色，我们也可以将其从 `Properties` 块中删除。

```glsl
	Properties {
		//_BaseColor ("Albedo", Base Color) = (1.0, 1.0, 1.0, 1.0)
		_Smoothness ("Smoothness", Range(0,1)) = 0.5
	}
```

我们将通过向为其创建的着色器图函数添加输出参数，将分形颜色暴露给着色器图。

```glsl
void ShaderGraphFunction_float (float3 In, out float3 Out, out float4 FractalColor) {
	Out = In;
	FractalColor = GetFractalColor();
}

void ShaderGraphFunction_half (half3 In, out half3 Out, out half4 FractalColor) {
	Out = In;
	FractalColor = GetFractalColor();
}
```

在着色器图本身中，我们首先必须删除“*基础颜色*”属性。它可以通过右键单击黑板上的标签打开的上下文菜单删除。

![img](https://catlikecoding.com/unity/tutorials/basics/organic-variety/arbitrary-colors/shader-graph-only-smoothness.png)

*仅平滑特性。*

然后将输出添加到我们的自定义函数节点。

![img](https://catlikecoding.com/unity/tutorials/basics/organic-variety/arbitrary-colors/shader-graph-fractalcolor-output.png)

*自定义函数的额外 FractalColor 输出。*

最后将新输出连接到 *Fragment* 节点的 *Base Color*。

![img](https://catlikecoding.com/unity/tutorials/basics/organic-variety/arbitrary-colors/shader-graph-fractal-color.png)

*使用 FractalColor 为片段着色。*

### 2.2 基于实例标识符的颜色

为了为每个实例引入多样性，我们必须以某种方式使 `GetFractalColor` 依赖于所绘制内容的实例标识符。由于这是一个从零开始递增的整数，最简单的测试是返回按三个数量级缩小的实例标识符，从而产生灰度梯度。

```glsl
float4 GetFractalColor () {
	return unity_InstanceID * 0.001;
}
```

但现在我们还必须确保仅对启用了过程实例化的着色器变体访问实例标识符，就像我们在 `ConfigureProcedural` 中所做的那样。

```glsl
float4 GetFractalColor () {
	#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
		return unity_InstanceID * 0.001;
	#endif
}
```

在这种情况下，不同之处在于我们总是必须返回一些东西，即使这可能没有多大意义。因此，我们将简单地返回非实例化着色器变体的配置颜色。这是通过在 `#endif` 之前插入 `#else` 指令并返回其间的颜色来实现的。

```glsl
	#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
		return unity_InstanceID * 0.001;
	#else
		return _BaseColor;
	#endif
```

![img](https://catlikecoding.com/unity/tutorials/basics/organic-variety/arbitrary-colors/colored-by-instance-id.png)

*按实例标识符着色。*

这表明这种方法是有效的，尽管它看起来很糟糕。例如，我们可以通过每五个实例重复一次来使梯度变得合理。为此，我们将通过 `%` 运算符使用实例标识符模 5。这将标识符序列转换为重复序列 0、1、2、3、4、0、1，2，3，4 等。然后我们将其缩小到四分之一，使范围从 0-4 变为 0-1。

```c#
		return (unity_InstanceID % 5.0) / 4.0;
```

![img](https://catlikecoding.com/unity/tutorials/basics/organic-variety/arbitrary-colors/colored-modulo-5.png)

*彩色模 5。*

即使梯度有规律地循环，但第一次偶然检查时，结果的颜色已经显得任意，因为它与分形的几何结构并不完全匹配。唯一明显的模式是，中心列始终是黑色的，因为它由每个级别的第一个实例组成。当序列与几何体对齐时，这种现象也会在更深层次上表现出来。

我们可以通过调整序列的长度来改变模式，例如将其增加到 10。这增加了更多的颜色变化，使黑色列出现的频率降低，尽管这确实使它们更加突出。

```c#
		return (unity_InstanceID % 10.0) / 9.0;
```

![img](https://catlikecoding.com/unity/tutorials/basics/organic-variety/arbitrary-colors/colored-modulo-10.png)

*有色模 10。*

### 2.3 Weyl 序列

创建重复渐变的一种稍微不同的方法是使用 Weyl 序列。简单地说，这些是 0X 模1、1X 模1、2X 模 1 和 3X 模 1 等形式的序列。因此，我们只得到 0-1 范围内的分数值，不包括 1。如果 X 是一个无理数，那么这个序列将在该范围内均匀分布。

我们真的不需要完美的分布，只需要足够的多样性。对于 X，0-1 范围内的随机值就足够了。例如，让我们考虑 0.381：

0.000, 0.381, 0.762, 0.143, 0.524, 0.905, 0.286, 0.667, 0.048, 0.429, 0.810, 0.191, 0.572, 0.953, 0.334, 0.715, 0.096, 0.477, 0.858, 0.239, 0.620, 0.001, 0.382, 0.763, 0.144, 0.525.

我们得到的是重复的三步但有时是两步的递增梯度，这些梯度都有点不同。该模式在 21 步后重复，但偏移了 0.001。其他值将产生不同的模式，具有不同的梯度，可以更长、更短和相反。

在着色器中，我们可以通过一次乘法创建这个序列，并将结果馈送到 `frac` 函数。

```c#
		return frac(unity_InstanceID * 0.381);
```

![img](https://catlikecoding.com/unity/tutorials/basics/organic-variety/arbitrary-colors/colored-sequence-381.png)

*基于 0.381 的彩色序列。*

### 2.4 随机因子和偏移

使用分数序列的结果看起来是可以接受的，除了我们仍然得到那些黑色的列。我们可以通过在序列中为每个级别添加不同的偏移量来消除这些问题，甚至可以为每个级别使用不同的序列。为了支持这一点，为两个序列号添加一个着色器属性向量，第一个是乘数，第二个是偏移量，然后在 `GetFractalColor` 中使用它们。在隔离值的小数部分之前，必须添加偏移量，以便对序列应用包裹移位。

```glsl
float2 _SequenceNumbers;

float4 GetFractalColor () {
	#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
		return frac(unity_InstanceID * _SequenceNumbers.x + _SequenceNumbers.y);
	#else
		return _BaseColor;
	#endif
}
```

在 `Fractal` 中跟踪着色器属性的标识符。

```c#
	static readonly int
		baseColorId = Shader.PropertyToID("_BaseColor"),
		matricesId = Shader.PropertyToID("_Matrices"),
		sequenceNumbersId = Shader.PropertyToID("_SequenceNumbers");
```

然后为每个级别添加一个序列号数组，最初设置为等于我们当前的配置，即 0.381 和 0。我们使用 `Vector4` 类型，因为即使我们使用更少的组件，也只能向 GPU 发送四个组件向量。

```c#
	Vector4[] sequenceNumbers;

	void OnEnable () {
		…
		sequenceNumbers = new Vector4[depth];
		int stride = 12 * 4;
		for (int i = 0, length = 1; i < parts.Length; i++, length *= 5) {
			…
			sequenceNumbers[i] = new Vector4(0.381f, 0f);
		}

		…
	}

	void OnDisable () {
		…
		sequenceNumbers = null;
	}
```

通过在属性块上调用 `SetVector`，在 `Update` 中为每个级别设置绘制循环中的序列号。

```c#
			propertyBlock.SetBuffer(matricesId, buffer);
			propertyBlock.SetVector(sequenceNumbersId, sequenceNumbers[i]);
```

最后，为了使序列任意且每级不同，我们用随机值替换固定配置的序列号。我们将使用 `UnityEngine.Random`，但这种类型与 `Unity.Mathematics.Random` 冲突，因此我们将显式使用适当的类型。

```c#
using quaternion = Unity.Mathematics.quaternion;
using Random = UnityEngine.Random;
```

然后，要获得随机值，只需将这两个常数替换为 `Random.value`，这将产生一个 0-1 范围内的值。

```c#
			sequenceNumbers[i] = new Vector4(Random.value, Random.value);
```

![img](https://catlikecoding.com/unity/tutorials/basics/organic-variety/arbitrary-colors/colored-sequence-random.png)

*带有随机因子和偏移的彩色序列。*

### 2.5 两个梯度

为了将随机序列与我们现有的渐变相结合，我们将引入第二个渐变，并将两者的颜色发送到 GPU。因此，用 A 和 B 颜色的属性替换单色属性。

```c#
	static readonly int
		//baseColorId = Shader.PropertyToID("_BaseColor"),
		colorAId = Shader.PropertyToID("_ColorA"),
		colorBId = Shader.PropertyToID("_ColorB"),
		matricesId = Shader.PropertyToID("_Matrices"),
		sequenceNumbersId = Shader.PropertyToID("_SequenceNumbers");
```

还将单个可配置梯度替换为 A 和 B 梯度。

```c#
	[SerializeField]
	//Gradient gradient;
	Gradient gradientA, gradientB;
```

然后在 `Update` 的绘制循环中计算两个渐变并设置它们的颜色。

```c#
			float gradientInterpolator = i / (matricesBuffers.Length - 1f);
			propertyBlock.SetColor(colorAId, gradientA.Evaluate(gradientInterpolator));
			propertyBlock.SetColor(colorBId, gradientB.Evaluate(gradientInterpolator));
```

![img](https://catlikecoding.com/unity/tutorials/basics/organic-variety/arbitrary-colors/two-gradient-properties.png)

*两个渐变属性。*

还将 *FractalGPU* 中的单色属性替换为两个。

```glsl
//float4 _BaseColor;
float4 _ColorA, _ColorB;
```

并使用 `lerp` 在 `GetFractalColor` 中使用序列结果作为插值器在它们之间进行插值。

```glsl
		return lerp(
			_ColorA, _ColorB,
			frac(unity_InstanceID * _SequenceNumbers.x + _SequenceNumbers.y)
		);
```

最后，对于 `#else` 的情况，只需返回 A 颜色。

```glsl
	#else
		return _ColorA;
	#endif
```

![img](https://catlikecoding.com/unity/tutorials/basics/organic-variety/arbitrary-colors/colored-two-gradients.png)

*用两个渐变着色。*

请注意，结果不是每个实例的两种颜色之间的二元选择，而是混合。

## 3 叶子

植物的一个共同特征是它们的四肢是特化的。例如叶子、花和水果。我们可以通过使最深的层次不同于其他层次，将这一特征添加到分形中。从现在开始，我们将把它视为叶子级别，即使它可能不代表实际的叶子。

### 3.1 叶子颜色

为了使分形的叶子实例与众不同，我们会给它们一个不同的颜色。虽然我们可以简单地通过渐变来实现这一点，但单独配置叶子颜色更方便，将渐变专用于树干、树枝和细枝。因此，在 `Fractal` 中添加两种叶子颜色的配置选项。

```c#
	[SerializeField]
	Gradient gradientA, gradientB;

	[SerializeField]
	Color leafColorA, leafColorB;
```

![img](https://catlikecoding.com/unity/tutorials/basics/organic-variety/leaves/leaf-color-properties.png)

*叶色属性。*

在 `Update` 中，在绘制循环之前确定叶子索引，该索引等于最后一个索引。

```c#
		int leafIndex = matricesBuffers.Length - 1;
		for (int i = 0; i < matricesBuffers.Length; i++) { … }
```

然后，在循环内部，直接使用叶级别的配置颜色，同时仍然评估所有其他级别的渐变。此外，由于我们现在提前一步结束梯度，因此在计算插值器时，我们必须从缓冲区长度中减去 2 而不是 1。

```c#
			Color colorA, colorB;
			if (i == leafIndex) {
				colorA = leafColorA;
				colorB = leafColorB;
			}
			else {
				float gradientInterpolator = i / (matricesBuffers.Length - 2f);
				colorA = gradientA.Evaluate(gradientInterpolator);
				colorB = gradientB.Evaluate(gradientInterpolator);
			}
			propertyBlock.SetColor(colorAId, colorA);
			propertyBlock.SetColor(colorBId, colorB);
```

![img](https://catlikecoding.com/unity/tutorials/basics/organic-variety/leaves/colored-leaves.png)

*具有明显叶子颜色的分形。*

请注意，这一变化迫使我们再次增加最小分形深度。

```c#
	[SerializeField, Range(3, 8)]
	int depth = 4;
```

### 3.2 叶网格

现在我们以不同的方式处理最低级别，我们还可以使用不同的网格来绘制它。为它添加一个配置字段。这使得可以对叶子使用立方体，而对其他所有东西使用球体。

```c#
	[SerializeField]
	Mesh mesh, leafMesh;
```

![img](https://catlikecoding.com/unity/tutorials/basics/organic-variety/leaves/leaf-mesh-property.png)

*叶网格属性，设置为立方体。*

在 `Update` 中调用 `Graphics.DrawMeshInstancedProcedural` 时使用适当的网格。

```c#
			Mesh instanceMesh;
			if (i == leafIndex) {
				colorA = leafColorA;
				colorB = leafColorB;
				instanceMesh = leafMesh;
			}
			else {
				float gradientInterpolator = i / (matricesBuffers.Length - 2f);
				colorA = gradientA.Evaluate(gradientInterpolator);
				colorB = gradientB.Evaluate(gradientInterpolator);
				instanceMesh = mesh;
			}
			…
			Graphics.DrawMeshInstancedProcedural(
				instanceMesh, 0, material, bounds, buffer.count, propertyBlock
			);
```

![img](https://catlikecoding.com/unity/tutorials/basics/organic-variety/leaves/cube-leaves.png)

*叶子的立方体。*

除了看起来更有趣之外，使用立方体作为叶子还可以显著提高性能，因为现在大多数实例都是立方体。我们最终得到的帧速率介于只绘制球体和只绘制立方体之间。

| 深度 | URP  | BRP  |
| ---- | ---- | ---- |
| 6    | 360  | 150  |
| 7    | 125  | 90   |
| 8    | 48   | 31   |

### 3.3 平滑度

除了不同的颜色，我们还可以给叶子不同的光滑度。事实上，我们可以根据第二个序列改变平滑度，就像改变颜色一样。要配置第二个序列，我们需要做的就是在 `OnEnable` 中用随机值填充序列号向量的其他两个分量。

```c#
			sequenceNumbers[i] =
				new Vector4(Random.value, Random.value, Random.value, Random.value);
```

然后，我们将在 `GetFractalColor` 中分别插值 RGB 和 A 通道，使用 A 通道的另外两个配置数字。

```glsl
float4 _SequenceNumbers;

float4 GetFractalColor () {
	#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
			float4 color;
		color.rgb = lerp(
			_ColorA.rgb, _ColorB.rgb,
			frac(unity_InstanceID * _SequenceNumbers.x + _SequenceNumbers.y)
		);
		color.a = lerp(
			_ColorA.a, _ColorB.a,
			frac(unity_InstanceID * _SequenceNumbers.z + _SequenceNumbers.w)
		);
		return color;
	#else
		return _ColorA;
	#endif
}
```

我们这样做是因为从现在开始，我们将使用颜色的 A 通道来设置平滑度，这是可能的，因为我们不将其用于透明度。这意味着在我们的着色器图中，我们将使用 *Split* 节点从 *FractalColor* 中提取 alpha 通道，并将其链接到 *Fragment* 的*平滑度*输入。然后从黑板上删除平滑度属性。

![img](https://catlikecoding.com/unity/tutorials/basics/organic-variety/leaves/shader-graph-smoothness.png)

*衍生平滑度。*

请注意，当您将光标悬停在节点上时，可以通过右上角出现的箭头按钮最小化着色器图节点以隐藏未使用的输入和输出。

![img](https://catlikecoding.com/unity/tutorials/basics/organic-variety/leaves/shader-graph-minimized-node.png)

*最小化拆分节点。*

我们在曲面着色器中以相同的方式设置平滑度。

```glsl
		void ConfigureSurface (Input input, inout SurfaceOutputStandard surface) {
			surface.Albedo = GetFractalColor().rgb;
			surface.Smoothness = GetFractalColor().a;
		}
```

> **我们不应该重用 `GetFractalColor` 的一次调用的结果吗？**
>
> 是的，但我们已经在这么做了。着色器编译器识别并优化掉重复的工作。请注意，这总是发生在着色器上，但通常不会发生在常规 C# 代码上。

我们还可以从曲面着色器中删除整个 `Properties` 块。

```glsl
	//Properties {
		//_Smoothness ("Smoothness", Range(0,1)) = 0.5
	//}
```

当我们使用颜色的阿尔法通道来控制平滑度时，我们现在必须调整颜色以考虑到这一点。例如，我将叶子平滑度设置为 50% 和 90%。请注意，平滑度的选择与颜色无关，即使它们是通过相同的属性配置在一起的。我们只是利用了迄今为止尚未使用的现有渠道。

![inspector](https://catlikecoding.com/unity/tutorials/basics/organic-variety/leaves/black-leaves-smoothness-inspector.png)

*黑叶，光滑度不一。*

我们还必须对默认设置为 100% alpha 的渐变进行此操作。我将它们设置为255中的80-90和140-160。我还调整了颜色，使分形更像树。

![inspector](https://catlikecoding.com/unity/tutorials/basics/organic-variety/leaves/color-configuration.png)
![fractal](https://catlikecoding.com/unity/tutorials/basics/organic-variety/leaves/coloration-depth-6.png)

*颜色看起来像植物。*

当分形深度设置为最大值时，效果最令人信服。

![img](https://catlikecoding.com/unity/tutorials/basics/organic-variety/leaves/coloration-depth-8.png)

*颜色相同，深度为 8。*

## 4 下垂

虽然我们的分形已经看起来更有机了，但这只适用于它的颜色。它的结构仍然坚固而完美。这是从侧面最容易看到的，场景窗口处于正交模式，旋转在 `Update` 中暂时设置为零。

```c#
		float spinAngleDelta = 0.125f * PI * Time.deltaTime * 0f;
```

![img](https://catlikecoding.com/unity/tutorials/basics/organic-variety/sagging/rigid.png)

*完全刚性的结构。*

有机结构并非如此完美。除了生长过程中增加的不规则性外，植物最明显的品质是它们受到重力的影响。由于自身重量，所有东西都会至少有点下垂。我们的分形没有经历这一点，但我们可以通过调整每个部分的旋转来近似这一现象。

### 4.1 下垂旋转轴

我们可以通过旋转所有东西来模拟下垂，使其指向下方一点。因此，我们必须围绕某个轴旋转每个实例，使其局部向上轴看起来像是被向下拉。第一步是确定零件在世界空间中的上轴。这是指向远离其父轴的轴。我们通过将上矢量旋转零件的初始世界旋转来找到它。这必须不考虑零件之前的下垂，否则它会积聚，一切都会很快直线下降。因此，在调整零件的世界旋转之前，我们在执行开始时根据零件的固定局部旋转和其父级的世界空间旋转进行旋转。

```c#
		public void Execute (int i) {
			FractalPart parent = parents[i / 5];
			FractalPart part = parts[i];
			part.spinAngle += spinAngleDelta;

			float3 upAxis = mul(mul(parent.worldRotation, part.rotation), up());

			part.worldRotation = mul(parent.worldRotation,
				mul(part.rotation, quaternion.RotateY(part.spinAngle))
			);
			…
		}
```

如果一个零件没有指向正上方，那么它自己的向上轴将与世界向上轴不同。通过围绕另一个轴旋转，可以从世界向上轴旋转到零件的向上轴。这个轴——我们将其命名为凹陷轴——是通过 `cross` 方法求出两个轴的叉积而得到的。

```c#
			float3 upAxis = mul(mul(parent.worldRotation, part.rotation), up());
			float3 sagAxis = cross(up(), upAxis);
```

叉积的结果是一个垂直于其两个参数的向量。矢量的长度取决于原始矢量的相对方向和长度。因为我们使用的是单位长度向量，所以凹陷轴的长度等于操作数之间角度的正弦。因此，为了得到一个合适的单位长度轴，我们必须将其调整为单位长度，为此我们可以使用 `normalize` 方法。

```c#
			float3 sagAxis = cross(up(), upAxis);
			sagAxis = normalize(sagAxis);
```

### 4.2 应用下垂

现在我们有了凹陷轴，我们可以通过用轴和角度调用 `quaternion.AxisAngle` 来构造凹陷旋转，单位为弧度。让我们创建一个 45° 的旋转，即四分之一 π 弧度。

```c#
			sagAxis = normalize(sagAxis);

			quaternion sagRotation = quaternion.AxisAngle(sagAxis, PI * 0.25f);
```

要应用下垂，我们必须使零件的世界旋转不再直接基于其父级。相反，我们通过将下垂旋转应用于父级的世界旋转来引入新的基础旋转。

```c#
			quaternion sagRotation = quaternion.AxisAngle(sagAxis, PI * 0.25f);
			quaternion baseRotation = mul(sagRotation, parent.worldRotation);

			part.worldRotation = mul(baseRotation,
				mul(part.rotation, quaternion.RotateY(part.spinAngle))
			);
```

![img](https://catlikecoding.com/unity/tutorials/basics/organic-variety/sagging/top-missing.png)

*顶部不见了。*

这造成了明显的差异，这显然是不正确的。最极端的错误是分形的顶部似乎缺失了。这是因为当一个部分指向正上方时，它与世界上轴线之间的角度为零。叉积的结果是一个长度为零的向量，归一化失败。我们通过检查凹陷向量的大小（其长度）是否大于零来解决这个问题。如果是这样，我们应用下垂，否则我们不应用下垂，直接使用父对象的旋转。这在物理上是有意义的，因为如果一个部分指向正上方，它就处于平衡状态，不会下垂。

向量的长度（也称为大小）可以通过长度法找到。之后，如果需要，可以将向量除以其大小，使其成为单位长度，这也是归一化的作用。

```c#
			//sagAxis = normalize(sagAxis);

			float sagMagnitude = length(sagAxis);
			quaternion baseRotation;
			if (sagMagnitude > 0f) {
				sagAxis /= sagMagnitude;
				quaternion sagRotation = quaternion.AxisAngle(sagAxis, PI * 0.25f);
				baseRotation = mul(sagRotation, parent.worldRotation);
			}
			else {
				baseRotation = parent.worldRotation;
			}
		
			part.worldRotation = mul(baseRotation,
				mul(part.rotation, quaternion.RotateY(part.spinAngle))
			);
```

![img](https://catlikecoding.com/unity/tutorials/basics/organic-variety/sagging/with-top-malformed.png)

*有顶部，但畸形。*

分形仍然是畸形的，因为我们现在有效地将每个部分的方向应用了两次。首先是下垂时，然后是向特定方向偏移时。我们通过始终沿零件的局部上轴偏移来解决这个问题。

```c#
			part.worldPosition =
				parent.worldPosition +
				//mul(parent.worldRotation, (1.5f * scale * part.direction));
				mul(part.worldRotation, float3(0f, 1.5f * scale, 0f));
```

![img](https://catlikecoding.com/unity/tutorials/basics/organic-variety/sagging/uniform-sagging.png)

*均匀下垂 45°。*

请注意，这意味着我们不再需要跟踪每个部分的方向向量，并且可以删除与之相关的所有代码。

```c#
	struct FractalPart {
		//public float3 direction, worldPosition;
		public float3 worldPosition;
		public quaternion rotation, worldRotation;
		public float spinAngle;
	}
	
	…

	//static float3[] directions = {
	//	up(), right(), left(), forward(), back()
	//};

	…

	FractalPart CreatePart (int childIndex) => new FractalPart {
		//direction = directions[childIndex],
		rotation = rotations[childIndex]
	};
```

### 4.3 调制下垂

凹陷似乎有效，但在分形运动时观察它很重要，所以让它再次旋转。

```c#
		float spinAngleDelta = 0.125f * PI * Time.deltaTime; // * 0f;
```

*持续 45° 下垂。*
它大多有效。无论零件的方向如何，它似乎都会被向下拉动。但方向突然发生了变化。当下垂的方向改变时，就会发生这种情况。因为我们使用固定的下垂角度，所以唯一的选择是沿正向或负向下垂，或者根本不下垂。这也意味着，对于几乎笔直向下指向的零件，下垂旋转最终会过冲，而不是向上拉动它们。

解决方案是让下垂量取决于世界上轴和零件上轴之间的角度。如果零件几乎笔直地向上或向下指向，则几乎不应出现下垂，而如果零件完全侧向指向，则下垂应达到最大值，以 90° 的角度伸出。下垂量与角度之间的关系不必是线性的。事实上，使用角度的正弦曲线会产生很好的结果。这就是我们已经拥有的交叉乘积的大小。因此，使用它来调节下垂旋转角度。

```c#
				quaternion sagRotation =
					quaternion.AxisAngle(sagAxis, PI * 0.25f * sagMagnitude);
```

*调节性下垂。*

由于下垂是在世界空间中计算的，因此整个分形的方向会影响它。因此，通过稍微旋转分形游戏对象，我们也可以使其顶部下垂。

*分形围绕 Z 轴旋转了 20°。*

### 4.4 最大凹陷角度

既然下垂有效，让我们配置最大下垂角度，通过暴露两个值来定义一个范围，再次增加多样性。我们使用度来配置这些角度，因为这比使用弧度更容易，最大值为 90°，默认值为 15° 和 25°。

```c#
	[SerializeField]
	Color leafColorA, leafColorB;

	[SerializeField, Range(0f, 90f)]
	float maxSagAngleA = 15f, maxSagAngleB = 25f;
```

![img](https://catlikecoding.com/unity/tutorials/basics/organic-variety/sagging/max-sag-angles.png)

*最大下垂角度。*

在 `FractalPart` 中添加最大凹陷角度，并在 `CreatePart` 中通过以两个配置的角度作为参数的范围调用 `Random.Range` 对其进行初始化。结果可以通过 `radians` 方法转换为弧度。

```c#
	struct FractalPart {
		public float3 worldPosition;
		public quaternion rotation, worldRotation;
		public float maxSagAngle, spinAngle;
	}

	…

	FractalPart CreatePart (int childIndex) => new FractalPart {
		maxSagAngle = radians(Random.Range(maxSagAngleA, maxSagAngleB)),
		rotation = rotations[childIndex]
	};
```

> **A 角必须小于 B 角吗？**
>
> 虽然这是明智的，但并不需要。`Random.Range` 方法只是使用一个随机值在其两个参数之间进行插值。

然后使用零件的最大下垂角度，而不是执行中的恒定 45°。

```c#
				quaternion sagRotation =
					quaternion.AxisAngle(sagAxis, part.maxSagAngle * sagMagnitude)
```

![img](https://catlikecoding.com/unity/tutorials/basics/organic-variety/sagging/variable-max-sage-angle.png)

*最大凹陷角度可变 15-25°，深度 8。*

## 5 旋转

在这一点上，我们已经调整了分形，使其看起来至少有点有机。我们将进行的最后一项增强是为其旋转行为增加多样性。

### 5.1 变速

就像我们为最大下垂角度所做的那样，引入旋转速度范围的配置选项，单位为度每秒。这些速度应为零或更大。

```c#
	[SerializeField, Range(0f, 90f)]
	float maxSagAngleA = 15f, maxSagAngleB = 25f;

	[SerializeField, Range(0f, 90f)]
	float spinVelocityA = 20f, spinVelocityB = 25f;
```

![img](https://catlikecoding.com/unity/tutorials/basics/organic-variety/spin/spin-velocities.png)

*旋转速度。*

在  `FractalPart` 中添加一个自旋速度场，并在 `CreatePart` 中随机初始化它。

```c#
	struct FractalPart {
		public float3 worldPosition;
		public quaternion rotation, worldRotation;
		public float maxSagAngle, spinAngle, spinVelocity;
	}

	…

	FractalPart CreatePart (int childIndex) => new FractalPart {
		maxSagAngle = radians(Random.Range(maxSagAngleA, maxSagAngleB)),
		rotation = rotations[childIndex],
		spinVelocity = radians(Random.Range(spinVelocityA, spinVelocityB))
	};
```

接下来，在 `UpdateFractalLevelJob` 中删除均匀自旋角增量字段，将其替换为增量时间字段。然后在 `Execute` 中应用零件自身的旋转速度。

```c#
		//public float spinAngleDelta;
		public float scale;
		public float deltaTime;

		…
		
		public void Execute (int i) {
			FractalPart parent = parents[i / 5];
			FractalPart part = parts[i];
			part.spinAngle += part.spinVelocity * deltaTime;

			…
		}
```

之后，调整 `Update`，使其不再使用均匀的旋转角度增量，而是将时间增量传递给作业。

```c#
		//float spinAngleDelta = 0.125f * PI * Time.deltaTime;
		float deltaTime = Time.deltaTime;
		FractalPart rootPart = parts[0][0];
		rootPart.spinAngle += rootPart.spinVelocity * deltaTime;
		…
		for (int li = 1; li < parts.Length; li++) {
			scale *= 0.5f;
			jobHandle = new UpdateFractalLevelJob {
				//spinAngleDelta = spinAngleDelta,
				deltaTime = deltaTime,
				…
			}.ScheduleParallel(parts[li].Length, 5, jobHandle);
		}
```

*0 到 90 之间的可变自旋速度。*

### 5.2 反向旋转

我们可以做的另一件事是反转某些部件的旋转方向。这可以通过允许负自旋速度的配置来实现。然而，如果我们想混合正速度和负速度，那么我们的两个配置值必须具有不同的符号。因此，该范围穿过零，低速不可避免。我们无法配置分形，使其速度在 20-25 的范围内，但要么是正的，要么是负的。

解决方案是分别配置速度和方向。首先将速度重命名为速度，以表示它们没有方向。然后为反向旋转机会添加另一个配置选项，表示为概率，即 0-1 范围内的值。

```c#
	[SerializeField, Range(0f, 90f)]
	float spinSpeedA = 20f, spinSpeedB = 25f;

	[SerializeField, Range(0f, 1f)]
	float reverseSpinChance = 0.25f;
	
	…

	FractalPart CreatePart (int childIndex) => return new FractalPart {
		maxSagAngle = radians(Random.Range(maxSagAngleA, maxSagAngleB)),
		rotation = rotations[childIndex],
		spinVelocity = radians(Random.Range(spinSpeedA, spinSpeedB))
	};
```

![img](https://catlikecoding.com/unity/tutorials/basics/organic-variety/spin/reverse-spin-chance.png)

*速度和反向旋转机会。*

我们可以通过检查随机值是否小于反向旋转机会来选择 `CreatePart` 中的旋转方向。如果是这样，我们将速度乘以 -1，否则乘以 1。

```c#
		spinVelocity =
			(Random.value < reverseSpinChance ? -1f : 1f) *
			radians(Random.Range(spinSpeedA, spinSpeedB))
```

*不同的旋转方向，速度总是 45°。*

请注意，现在分形的某些部分可能看起来相对静止。当相反的自旋速度相互抵消时，就会发生这种情况。

### 5.3 性能

在完成了自上一教程以来所做的所有调整后，我们再次审视了性能。事实证明，更新时间增加了，深度 6 和 7 大约翻了一番，而深度 8 增加了 40%。与上次测量相比，这并没有对帧率产生负面影响，因为它的速度太快了。

| 深度 | MS   | URP  | BRP  |
| ---- | ---- | ---- | ---- |
| 6    | 0.20 | 365  | 145  |
| 7    | 0.45 | 130  | 91   |
| 8    | 2.40 | 48   | 31   |

想知道下一个教程什么时候发布吗？关注我的 [Patreon](https://www.patreon.com/catlikecoding) 页面！

[license](https://catlikecoding.com/unity/tutorials/license/)

[repository](https://bitbucket.org/catlikecodingunitytutorials/basics-07-organic-variety/)

[PDF](https://catlikecoding.com/unity/tutorials/basics/organic-variety/Organic-Variety.pdf)