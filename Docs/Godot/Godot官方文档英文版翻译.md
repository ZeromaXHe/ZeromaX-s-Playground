# 渲染

## 使用 Viewport

https://docs.godotengine.org/en/stable/tutorials/rendering/viewports.html#using-viewports

### 引言

将 [Viewport](https://docs.godotengine.org/en/stable/classes/class_viewport.html#class-viewport) 视为投影游戏的屏幕。为了观看游戏，我们需要一个可以绘制游戏的曲面。这个曲面就是根视口。

![../../_images/subviewportnode.webp](https://docs.godotengine.org/en/stable/_images/subviewportnode.webp)

[SubViewport](https://docs.godotengine.org/en/stable/classes/class_subviewport.html#class-subviewport) 是一种可以添加到场景中的视口，以便有多个表面可供绘制。当我们绘制子视口时，我们称之为渲染目标。我们可以通过访问渲染目标的相应[纹理](https://docs.godotengine.org/en/stable/classes/class_viewport.html#class-viewport-method-get-texture)来访问其内容。通过使用子视口作为渲染目标，我们可以同时渲染多个场景，也可以渲染到应用于场景中对象的 [ViewportTexture](https://docs.godotengine.org/en/stable/classes/class_viewporttexture.html#class-viewporttexture)，例如动态天空盒。

[SubViewports](https://docs.godotengine.org/en/stable/classes/class_subviewport.html#class-subviewport) 有各种用例，包括：

- 在 2D 游戏中渲染 3D 对象
- 在 3D 游戏中渲染 2D 元素
- 渲染动态纹理
- 在运行时生成程序纹理
- 在同一场景中渲染多个摄影机

所有这些用例的共同点是，您可以将对象绘制到纹理上，就像它是另一个屏幕一样，然后可以选择如何处理生成的纹理。

Godot 中的另一种视口是 [Windows](https://docs.godotengine.org/en/stable/classes/class_window.html#class-window)。它们允许将内容投影到窗口上。虽然根视口是一个窗口，但它们的灵活性较差。如果你想使用视口的纹理，你大部分时间都会使用 [SubViewports](https://docs.godotengine.org/en/stable/classes/class_subviewport.html#class-subviewport)。

### 输入

[Viewports](https://docs.godotengine.org/en/stable/classes/class_viewport.html#class-viewport) 还负责将经过适当调整和缩放的输入事件传递给其子节点。默认情况下，[SubViewports](https://docs.godotengine.org/en/stable/classes/class_subviewport.html#class-subviewport) 不会自动接收输入，除非它们从直接的 [SubViewportContainer](https://docs.godotengine.org/en/stable/classes/class_subviewportcontainer.html#class-subviewportcontainer) 父节点接收输入。在这种情况下，可以使用 [Disable Input](https://docs.godotengine.org/en/stable/classes/class_viewport.html#class-viewport-property-gui-disable-input) 属性禁用输入。

![../../_images/input.webp](https://docs.godotengine.org/en/stable/_images/input.webp)

有关 Godot 如何处理输入的更多信息，请阅读[输入事件教程](https://docs.godotengine.org/en/stable/tutorials/inputs/inputevent.html#doc-inputevent)。

### Listener

Godot 支持 3D 声音（在 2D 和 3D 节点中）。更多信息可以在[音频流教程](https://docs.godotengine.org/en/stable/tutorials/audio/audio_streams.html#doc-audio-streams)中找到。为了使这种声音可听，需要将 [Viewport](https://docs.godotengine.org/en/stable/classes/class_viewport.html#class-viewport) 启用为监听器（用于 2D 或 3D）。如果您使用 [SubViewport](https://docs.godotengine.org/en/stable/classes/class_subviewport.html#class-subviewport) 显示 [World3D](https://docs.godotengine.org/en/stable/classes/class_world3d.html#class-world3d) 或 [World2D](https://docs.godotengine.org/en/stable/classes/class_world2d.html#class-world2d)，别忘了启用此选项！

### 相机（2D 和 3D）

使用 [Camera3D](https://docs.godotengine.org/en/stable/classes/class_camera3d.html#class-camera3d) 或 [Camera2D](https://docs.godotengine.org/en/stable/classes/class_camera2d.html#class-camera2d) 时，它将始终显示在最近的父 [Viewport](https://docs.godotengine.org/en/stable/classes/class_viewport.html#class-viewport) 上（朝向根）。例如，在以下层次结构中：

![../../_images/cameras.webp](https://docs.godotengine.org/en/stable/_images/cameras.webp)

`CameraA` 将显示在根 [Viewport](https://docs.godotengine.org/en/stable/classes/class_viewport.html#class-viewport) 上，并绘制 `MeshA`。`CameraB` 将与 `MeshB` 一起被 [SubViewport](https://docs.godotengine.org/en/stable/classes/class_subviewport.html#class-subviewport) 捕获。即使 `MeshB` 位于场景层次中，它也不会被绘制到根视口。同样，`MeshA` 在子视口中不可见，因为子视口仅捕获层次结构中其下方的节点。

每个 [Viewport](https://docs.godotengine.org/en/stable/classes/class_viewport.html#class-viewport) 只能有一个活动摄影机，因此如果有多个，请确保所需的摄影机具有 [current](https://docs.godotengine.org/en/stable/classes/class_camera3d.html#class-camera3d-property-current) 属性集，或通过调用将其设置为当前摄影机：

```gdscript
camera.make_current()
```

```c#
camera.MakeCurrent();
```

默认情况下，摄影机将渲染其世界中的所有对象。在 3D 中，摄影机可以使用其 [cull_mask](https://docs.godotengine.org/en/stable/classes/class_camera3d.html#class-camera3d-property-cull-mask) 属性和 [VisualInstance3D](https://docs.godotengine.org/en/stable/classes/class_visualinstance3d.html#class-visualinstance3d) 的[图层](https://docs.godotengine.org/en/stable/classes/class_visualinstance3d.html#class-visualinstance3d-property-layers)属性来限制渲染哪些对象。

### 缩放和拉伸

[SubViewport](https://docs.godotengine.org/en/stable/classes/class_subviewport.html#class-subviewport) 具有 [size](https://docs.godotengine.org/en/stable/classes/class_subviewport.html#class-subviewport-property-size) 属性，该属性表示子视口的像素大小。对于作为 [SubViewportContainers](https://docs.godotengine.org/en/stable/classes/class_subviewportcontainer.html#class-subviewportcontainer) 子级的SubViewports，这些值将被覆盖，但对于所有其他值，这将设置它们的分辨率。

还可以通过调用以下命令来缩放二维内容，并使 [SubViewport](https://docs.godotengine.org/en/stable/classes/class_subviewport.html#class-subviewport) 分辨率与指定的大小不同：

```gdscript
sub_viewport.set_size_2d_override(Vector2i(width, height)) # Custom size for 2D.
sub_viewport.set_size_2d_override_stretch(true) # Enable stretch for custom size.
```

```c#
subViewport.Size2DOverride = new Vector2I(width, height); // Custom size for 2D.
subViewport.Size2DOverrideStretch = true; // Enable stretch for custom size.
```

有关使用根视口缩放和拉伸的信息，请访问多分辨率教程

### 世界

对于 3D，[Viewport](https://docs.godotengine.org/en/stable/classes/class_viewport.html#class-viewport) 将包含 [World3D](https://docs.godotengine.org/en/stable/classes/class_world3d.html#class-world3d)。这基本上是将物理和渲染联系在一起的宇宙。基于 Node3D 的节点将使用最近视口的 World3D 进行注册。默认情况下，新创建的视口不包含 World3D，但使用与其父视口相同的视口。根视口始终包含 World3D，默认情况下，这是渲染对象的对象。

可以使用 [World 3D](https://docs.godotengine.org/en/stable/classes/class_viewport.html#class-viewport-property-world-3d) 属性在 [Viewport](https://docs.godotengine.org/en/stable/classes/class_viewport.html#class-viewport) 中设置 [World3D](https://docs.godotengine.org/en/stable/classes/class_world3d.html#class-world3d)，这将分隔此 [Viewport](https://docs.godotengine.org/en/stable/classes/class_viewport.html#class-viewport) 的所有子节点，并阻止它们与父视口的 World3D 交互。这在某些场景中特别有用，例如，您可能希望在游戏中（如《星际争霸》）显示一个单独的 3D 角色。

当您想要创建显示单个对象的 [Viewports](https://docs.godotengine.org/en/stable/classes/class_viewport.html#class-viewport) 而不想创建 [World3D](https://docs.godotengine.org/en/stable/classes/class_world3d.html#class-world3d) 时，视口可以选择使用其[自己的 World3D](https://docs.godotengine.org/en/stable/classes/class_viewport.html#class-viewport-property-own-world-3d)。当您想在 [World2D](https://docs.godotengine.org/en/stable/classes/class_world2d.html#class-world2d) 中实例化 3D 角色或对象时，这很有用。

对于二维，每个[Viewport](https://docs.godotengine.org/en/stable/classes/class_viewport.html#class-viewport) 始终包含自己的 [World2D](https://docs.godotengine.org/en/stable/classes/class_world2d.html#class-world2d)。在大多数情况下，这就足够了，但如果需要共享它们，可以通过代码在 Viewport 上设置 [world_2d](https://docs.godotengine.org/en/stable/classes/class_viewport.html#class-viewport-property-world-2d) 来实现。

有关其工作原理的示例，请分别参阅演示项目 [3D in 2D](https://github.com/godotengine/godot-demo-projects/tree/master/viewport/3d_in_2d) 和 [2D in 3D](https://github.com/godotengine/godot-demo-projects/tree/master/viewport/2d_in_3d)。

### 捕获

可以查询 [Viewport](https://docs.godotengine.org/en/stable/classes/class_viewport.html#class-viewport) 内容的捕获。对于根视口，这实际上是一个屏幕截图。这是通过以下代码完成的：

```gdscript
# Retrieve the captured Image using get_image().
var img = get_viewport().get_texture().get_image()
# Convert Image to ImageTexture.
var tex = ImageTexture.create_from_image(img)
# Set sprite texture.
sprite.texture = tex
```

```c#
// Retrieve the captured Image using get_image().
var img = GetViewport().GetTexture().GetImage();
// Convert Image to ImageTexture.
var tex = ImageTexture.CreateFromImage(img);
// Set sprite texture.
sprite.Texture = tex;
```

但是，如果你在 `_ready()` 中或从 [Viewport's](https://docs.godotengine.org/en/stable/classes/class_viewport.html#class-viewport) 初始化的第一帧开始使用它，你将得到一个空纹理，因为没有什么可以作为纹理获得。您可以使用（例如）来处理它：

```gdscript
# Wait until the frame has finished before getting the texture.
await RenderingServer.frame_post_draw
# You can get the image after this.
```

```c#
// Wait until the frame has finished before getting the texture.
await RenderingServer.Singleton.ToSignal(RenderingServer.SignalName.FramePostDraw);
// You can get the image after this.
```





## 修复抖动、卡顿和输入延迟

https://docs.godotengine.org/en/stable/tutorials/rendering/jitter_stutter.html#fixing-jitter-stutter-and-input-lag

### 什么是抖动、卡顿和输入延迟？

抖动（jitter）和卡顿（stutter）是屏幕上物体可见运动的两种不同变化，即使在全速运行时也可能影响游戏。这些效果在游戏中最为明显，在游戏中，世界以恒定的速度朝着固定的方向移动，比如跑步者或平台玩家。

输入延迟（input lag）与抖动和卡顿无关，但有时会一并讨论。输入延迟是指使用鼠标、键盘、控制器或触摸屏执行操作时，屏幕上可见的延迟。它可能与游戏代码、引擎代码或外部因素（如硬件）有关。输入延迟在使用鼠标瞄准的游戏中最为明显，例如第一人称游戏。输入延迟不能完全消除，但可以通过多种方式减少。

### 区分抖动和卡顿

以正常帧率运行而不显示任何效果的游戏将看起来很流畅：

![../../_images/motion_normal.gif](https://docs.godotengine.org/en/stable/_images/motion_normal.gif)

表现出*抖动*的游戏会以一种非常微妙的方式不断抖动：

![../../_images/motion_jitter.gif](https://docs.godotengine.org/en/stable/_images/motion_jitter.gif)

最后，一个出现卡顿的游戏会看起来很流畅，但每隔几秒钟就会停止或回滚一帧：

![../../_images/motion_stutter.gif](https://docs.godotengine.org/en/stable/_images/motion_stutter.gif)

### 抖动

抖动的原因有很多，最典型的是当游戏*物理频率*（通常为 60 Hz)以与显示器刷新率不同的分辨率运行时。检查您的显示器刷新率是否与 60 Hz 不同。

这通常不是问题，因为大多数监视器都是 60 Hz 的，从 Godot 3.1 开始，引入了一个帧计时器，试图尽可能地与刷新同步。

有时只有一些对象出现抖动（角色或背景）。当它们在不同的时间源中处理时会发生这种情况（一个在物理步骤中处理，另一个在空闲步骤中处理）。Godot 3.1 对此做了一些改进，从允许在常规 `_process()` 循环中为运动体设置动画，到进一步修复帧计时器。

### 卡顿

卡顿可能由于两个不同的原因而发生。第一个，也是最明显的一个，是游戏无法保持全帧率性能。解决这个问题是特定于游戏的，需要优化。

第二个更复杂，因为它通常与引擎或游戏无关，而是与底层操作系统有关。以下是关于不同操作系统上卡顿的一些信息。

在支持禁用 V-Sync 的平台上，通过在项目设置中禁用 V-Sync，可以使口吃不那么明显。然而，这将导致撕裂，特别是在刷新率低的显示器上。如果您的显示器支持它，请考虑在启用 V-Sync 的同时启用可变刷新率（G-Sync / FreeSync）。这避免了在不引入撕裂的情况下减轻某些形式的卡顿。

强制图形卡使用最大性能配置也有助于减少卡顿，但代价是 GPU 功耗增加。

#### Windows

众所周知，Windows 会导致窗口游戏卡顿。这主要取决于安装的硬件、驱动程序版本和并行运行的进程（例如，打开许多浏览器选项卡可能会导致正在运行的游戏卡顿）。为了避免这种情况，从 3.1 开始，Godot 将游戏优先级提高到“高于正常值”。这有很大帮助，但可能无法完全消除卡顿。

完全消除这一点需要给你的游戏完全的权限才能变得“时间紧迫的（time critical）”，这是不建议的。有些游戏可能会这样做，但建议学会接受这个问题，因为这在 Windows 游戏中很常见，大多数用户不会玩窗口游戏（在窗口中玩的游戏，如益智游戏，通常无论如何都不会出现这个问题）。

对于全屏模式，Windows 赋予游戏特殊的优先级，因此卡顿不再可见，而且非常罕见。大多数游戏都是这样玩的。

使用轮询率为 1000 Hz 或更高的鼠标时，请考虑使用完全最新的 Windows 11 安装，该安装附带了与高轮询率鼠标的高 CPU 利用率相关的修复程序。这些修复程序在 Windows 10 和旧版本中不可用。

> **小贴士：**
>
> 游戏应使用**独占全屏（Exclusive Fullscreen）**窗口模式，而不是**全屏**模式，全屏模式旨在防止 Windows 自动将窗口视为独占全屏。
>
> **全屏**是指希望使用每像素透明度而不会被操作系统禁用的 GUI 应用程序使用的。它通过在屏幕底部留下一条 1 像素的线来实现这一点。相比之下，**Exclusive Fullscreen** 使用实际屏幕大小，并允许 Windows 减少全屏游戏的抖动和输入延迟。

#### Linux

卡顿可能在桌面 Linux 上可见，但这通常与不同的视频驱动程序和合成器有关。一些合成器也可能触发此问题（例如 KWin），因此建议尝试使用不同的合成器来排除其原因。一些窗口管理器，如 KWin 和 Xfwm，允许您手动禁用合成，这可以提高性能（以撕裂为代价）。

除了向驱动程序或合成器开发人员报告问题外，没有解决驱动程序或合成程序卡顿的方法。与全屏模式相比，在窗口模式下播放时，即使禁用合成，抖动也可能更明显。

[Feral GameMode](https://github.com/FeralInteractive/gamemode) 可用于在运行特定进程时自动应用优化（例如强制GPU性能配置文件）。

#### macOS

一般来说，macOS 是无卡顿的，尽管最近在全屏运行时报告了一些错误（这是一个 macOS 错误）。如果你有一台机器表现出这种行为，请告诉我们。

#### 安卓系统

一般来说，Android 是不连贯和抖动的，因为运行活动得到了所有的优先级。也就是说，可能会有问题的设备（已知较旧的 Kindle Fire 就是其中之一）。如果你在 Android 上看到这个问题，请告诉我们。

#### iOS

iOS 设备通常没有卡顿，但运行较新版本操作系统的旧设备可能会出现问题。这通常是不可避免的。

### 输入延迟

#### 项目配置

在支持禁用 V-Sync 的平台上，可以通过在项目设置中禁用 V-Sync 来减少输入延迟。然而，这将导致撕裂，特别是在刷新率低的显示器上。建议将 V-Sync 作为玩家切换的选项。

使用 Forward+ 或 Mobile 渲染方法时，启用 V-Sync 时减少视觉延迟的另一种方法是使用双缓冲 V-Sync，而不是默认的三缓冲 V-Sync。从 Godot 4.3 开始，这可以通过将“**显示 > 窗口 > V-Sync > Swapchain 图像计数**”项目设置减少到 `2` 来实现。使用双缓冲的缺点是，如果由于 CPU 或 GPU 瓶颈而无法达到显示刷新率，帧率将不太稳定。例如，在 60 Hz 的显示器上，如果在三重缓冲的游戏过程中帧率通常会降至 55 FPS，那么在双缓冲的情况下，它必须暂时降至 30 FPS（然后在可能的情况下恢复到 60 FPS）。因此，只有当您能够在目标硬件上持续达到显示刷新率时，才建议使用双缓冲 V-Sync。

增加每秒的物理迭代次数也可以减少物理引起的输入延迟。这在使用物理插值时尤其明显（这提高了平滑度，但增加了延迟）。为此，请将 **Physics > Common > Physics Ticks Per Second** 设置为高于默认值 `60` 的值，或在脚本中在运行时设置 `Engine.physics_ticks_per_second`。禁用物理插值时，监视器刷新率倍数（通常为 `60`）的值效果最佳，因为它们可以避免抖动。这意味着 `120`、`180` 和 `240` 等值是很好的起点。作为额外的好处，更高的物理FPGA使隧道和物理不稳定性问题不太可能发生。

增加物理 FPS 的缺点是 CPU 使用率会增加，这可能会导致具有大量物理模拟代码的游戏出现性能瓶颈。这可以通过仅在低延迟至关重要的情况下提高物理 FPS 来缓解，或者让玩家调整物理 FPS 以匹配他们的硬件。然而，即使在游戏逻辑中一直使用 `delta`，不同的物理 FPS 也会导致物理模拟中的不同结果。这可以让某些玩家比其他玩家更有优势。因此，对于竞争性多人游戏，应避免允许玩家自己更改物理 FPS。

最后，您可以通过在脚本中调用 `Input.set_use_accumulated_input(false)` 来禁用每个渲染帧的输入缓冲。这将使脚本中的 `_input()` 和 `_unhanded_input()` 函数在每次输入时都被调用，而不是累积输入并等待渲染帧。禁用输入累积将增加 CPU 使用率，因此应谨慎操作。

> **小贴士：**
>
> 在任何 Godot 项目中，您都可以使用 `--disable-vsync` 命令行参数强制禁用 V-Sync。从 Godot 4.2 开始，`--max-fps <fps>` 也可用于设置 FPS 限制（`0` 表示无限制）。这些论点可以同时使用。

#### 硬件/操作系统特定

如果您的显示器支持它，请考虑在启用 V-Sync 的同时启用可变刷新率（G-Sync / FreeSync），然后根据[本页](https://blurbusters.com/howto-low-lag-vsync-on/)将项目设置中的帧速率限制在略低于显示器最大刷新率的值。例如，在 144 Hz 的显示器上，可以将项目的帧速率上限设置为 `141`。起初，这可能违反直觉，但将 FPS 限制在最大刷新率范围以下可以确保操作系统永远不必等待垂直消隐完成。这会导致类似的输入延迟，因为 V-Sync 在相同的帧速率上限下被禁用（通常大于 1ms），但没有任何撕裂。

这可以通过更改**应用程序 > 运行 > 最大 FPS**项目设置或在脚本中的运行时分配 `Engine.max_fps` 来实现。

在某些平台上，您还可以在图形驱动程序选项（如 Windows 上的 NVIDIA 控制面板）中选择进入低延迟模式。**Ultra** 设置将以略低的平均帧速率为代价，为您提供尽可能低的延迟。强制 GPU 使用最大性能配置文件还可以进一步减少输入延迟，但代价是更高的功耗（以及由此产生的热量/风扇噪音）。

最后，确保您的显示器在操作系统的显示设置中以尽可能高的刷新率运行。

此外，请确保您的鼠标配置为使用其最高轮询率（游戏鼠标通常为 1000 Hz，有时甚至更高）。然而，高 USB 轮询率可能会导致高 CPU 使用率，因此 500 Hz 可能是低端 CPU 上更安全的赌注。如果您的鼠标提供多种 DPI 设置，请考虑[使用尽可能高的设置并降低游戏内的灵敏度，以减少鼠标延迟](https://www.youtube.com/watch?v=6AoRfv9W110)。

在 Linux 上，在允许合成的窗口管理器（如 KWin 或 Xfwm）中禁用合成可以显著减少输入延迟。

### 报告抖动、卡顿或输入延迟问题

如果您报告的卡顿或抖动问题（打开问题）不是由上述任何原因引起的，请非常清楚地说明有关设备、操作系统、驱动程序版本等的所有信息。这可能有助于更好地排除故障。

如果您报告的是输入延迟问题，请包括使用高速相机拍摄的照片（例如手机的慢动作视频模式）。捕获**必须**使屏幕和输入设备都可见，以便可以计算输入和屏幕结果之间的帧数。此外，一定要提到显示器的刷新率和输入设备的轮询率（尤其是鼠标）。

此外，请确保根据所展示的行为使用正确的术语（抖动、卡顿、输入延迟）。这将有助于更快地理解您的问题。提供一个可用于重现问题的项目，如果可能的话，包括一个演示错误的屏幕截图。



## 合成器（The Compositor）

https://docs.godotengine.org/en/stable/tutorials/rendering/compositor.html#the-compositor

合成器是 Godot 4 中的一项新功能，它允许在渲染[视口](https://docs.godotengine.org/en/stable/classes/class_viewport.html#class-viewport)内容时控制渲染管道。

它可以在 [WorldEnvironment](https://docs.godotengine.org/en/stable/classes/class_worldenvironment.html#class-worldenvironment) 节点上配置，应用于所有视口，也可以在 [Camera3D](https://docs.godotengine.org/en/stable/classes/class_camera3d.html#class-camera3d) 上配置，仅应用于使用该相机的视口。

[Compositor](https://docs.godotengine.org/en/stable/classes/class_compositor.html#class-compositor)资源用于配置合成器。要开始，只需在相应的节点上创建一个新的合成器：

![../../_images/new_compositor.webp](https://docs.godotengine.org/en/stable/_images/new_compositor.webp)

> **注：**
>
> 合成器目前是仅由 Mobile 和 Forward+ 渲染器支持的功能。

### 合成器效果

合成器效果允许您在各个阶段将其他逻辑插入到渲染管道中。这是一个高级功能，需要对渲染管道有深入的了解才能发挥其最大的优势。

由于合成器效果的核心逻辑是从渲染管道调用的，因此需要注意的是，该逻辑将在进行渲染的线程内运行。需要小心确保我们不会遇到线程问题。

为了说明如何使用合成器效果，我们将创建一个简单的后处理效果，允许您编写自己的着色器代码，并通过计算着色器应用此全屏。您可以在[此处](https://github.com/godotengine/godot-demo-projects/tree/master/compute/post_shader)找到已完成的演示项目。

我们首先创建一个名为 `post_process_shader.gd` 的新脚本。我们将把它变成一个工具脚本，这样我们就可以在编辑器中看到合成器效果。我们需要从 [CompositorEffect](https://docs.godotengine.org/en/stable/classes/class_compositoreffect.html#class-compositoreffect) 扩展我们的节点。我们还必须给脚本一个类名。

```gdscript
@tool
extends CompositorEffect
class_name PostProcessShader
```

接下来，我们将为着色器模板代码定义一个常数。这是使我们的计算着色器工作的样板代码。

```gdscript
const template_shader : String = "#version 450

// Invocations in the (x, y, z) dimension
layout(local_size_x = 8, local_size_y = 8, local_size_z = 1) in;

layout(rgba16f, set = 0, binding = 0) uniform image2D color_image;

// Our push constant
layout(push_constant, std430) uniform Params {
	vec2 raster_size;
	vec2 reserved;
} params;

// The code we want to execute in each invocation
void main() {
	ivec2 uv = ivec2(gl_GlobalInvocationID.xy);
	ivec2 size = ivec2(params.raster_size);

	if (uv.x >= size.x || uv.y >= size.y) {
		return;
	}

	vec4 color = imageLoad(color_image, uv);

	#COMPUTE_CODE

	imageStore(color_image, uv, color);
}"
```

有关计算着色器如何工作的更多信息，请查看[使用计算着色器](https://docs.godotengine.org/en/stable/tutorials/shaders/compute_shaders.html#doc-compute-shaders)。

这里重要的一点是，对于屏幕上的每个像素，我们的 `main` 函数都会被执行，在这个函数中，我们加载像素的当前颜色值，执行用户代码，并将修改后的颜色写回彩色图像。

`#COMPUTE_CODE` 被我们的用户代码替换。

为了设置我们的用户代码，我们需要一个导出变量。我们还将定义一些我们将使用的脚本变量：

```gdscript
@export_multiline var shader_code : String = "":
	set(value):
		mutex.lock()
		shader_code = value
		shader_is_dirty = true
		mutex.unlock()

var rd : RenderingDevice
var shader : RID
var pipeline : RID

var mutex : Mutex = Mutex.new()
var shader_is_dirty : bool = true
```

请注意我们的代码中使用了 [Mutex](https://docs.godotengine.org/en/stable/classes/class_mutex.html#class-mutex)。我们的大部分实现都是从渲染引擎调用的，因此在我们的渲染线程中运行。

我们需要确保我们设置了新的着色器代码，并将着色器代码标记为脏，而渲染线程不会同时访问这些数据。

接下来，我们初始化我们的效果。

```gdscript
# Called when this resource is constructed.
func _init():
	effect_callback_type = EFFECT_CALLBACK_TYPE_POST_TRANSPARENT
	rd = RenderingServer.get_rendering_device()
```

这里的主要内容是设置 `effect_callback_type`，它告诉渲染引擎在渲染管道的哪个阶段调用我们的代码。

> **注：**
>
> 目前，我们只能访问 3D 渲染管道的各个阶段！

我们还可以参考我们的渲染设备，这将非常方便。

我们还需要在自己之后进行清理，为此，我们对 `NOTIFICATION_PREDELETE` 通知做出反应：

```gdscript
# System notifications, we want to react on the notification that
# alerts us we are about to be destroyed.
func _notification(what):
	if what == NOTIFICATION_PREDELETE:
		if shader.is_valid():
			# Freeing our shader will also free any dependents such as the pipeline!
			rd.free_rid(shader)
```

请注意，即使我们在渲染线程内创建了着色器，我们也不会在这里使用互斥体（mutex）。我们渲染服务器上的方法是线程安全的，`free_rid` 将推迟清理着色器，直到当前正在渲染的任何帧完成。

还要注意，我们并没有释放我们的管道。渲染设备执行依赖跟踪，由于管道依赖于着色器，因此当着色器被破坏时，它将自动释放。

从这一点开始，我们的代码将在渲染线程上运行。

我们的下一步是一个辅助函数，如果用户代码发生更改，它将重新编译着色器。

```gdscript
# Check if our shader has changed and needs to be recompiled.
func _check_shader() -> bool:
	if not rd:
		return false

	var new_shader_code : String = ""

	# Check if our shader is dirty.
	mutex.lock()
	if shader_is_dirty:
		new_shader_code = shader_code
		shader_is_dirty = false
	mutex.unlock()

	# We don't have a (new) shader?
	if new_shader_code.is_empty():
		return pipeline.is_valid()

	# Apply template.
	new_shader_code = template_shader.replace("#COMPUTE_CODE", new_shader_code);

	# Out with the old.
	if shader.is_valid():
		rd.free_rid(shader)
		shader = RID()
		pipeline = RID()

	# In with the new.
	var shader_source : RDShaderSource = RDShaderSource.new()
	shader_source.language = RenderingDevice.SHADER_LANGUAGE_GLSL
	shader_source.source_compute = new_shader_code
	var shader_spirv : RDShaderSPIRV = rd.shader_compile_spirv_from_source(shader_source)

	if shader_spirv.compile_error_compute != "":
		push_error(shader_spirv.compile_error_compute)
		push_error("In: " + new_shader_code)
		return false

	shader = rd.shader_create_from_spirv(shader_spirv)
	if not shader.is_valid():
		return false

	pipeline = rd.compute_pipeline_create(shader)
	return pipeline.is_valid()
```

在这个方法的顶部，我们再次使用互斥体来保护对用户着色器代码的访问，以及我们的脏标志。如果用户着色器代码脏了，我们会制作用户着色器代码的本地副本。

如果我们没有新的代码片段，如果我们已经有一个有效的管道，我们将返回 true。

如果我们确实有一个新的代码片段，我们会将其嵌入到模板代码中，然后进行编译。

> **警告：**
>
> 这里显示的代码在运行时编译了我们的新代码。这对于原型制作非常有用，因为我们可以立即看到更改后的着色器的效果。
>
> 这将阻止预编译和缓存此着色器，这在某些平台（如控制台）上可能是一个问题。请注意，演示项目附带了一个替代示例，其中 `glsl` 文件包含整个计算着色器，并使用了该着色器。Godot 能够使用这种方法预编译和缓存着色器。

最后，我们需要实现我们的效果回调，渲染引擎将在渲染的正确阶段调用它。

```gdscript
# Called by the rendering thread every frame.
func _render_callback(p_effect_callback_type, p_render_data):
	if rd and p_effect_callback_type == EFFECT_CALLBACK_TYPE_POST_TRANSPARENT and _check_shader():
		# Get our render scene buffers object, this gives us access to our render buffers.
		# Note that implementation differs per renderer hence the need for the cast.
		var render_scene_buffers : RenderSceneBuffersRD = p_render_data.get_render_scene_buffers()
		if render_scene_buffers:
			# Get our render size, this is the 3D render resolution!
			var size = render_scene_buffers.get_internal_size()
			if size.x == 0 and size.y == 0:
				return

			# We can use a compute shader here
			var x_groups = (size.x - 1) / 8 + 1
			var y_groups = (size.y - 1) / 8 + 1
			var z_groups = 1

			# Push constant
			var push_constant : PackedFloat32Array = PackedFloat32Array()
			push_constant.push_back(size.x)
			push_constant.push_back(size.y)
			push_constant.push_back(0.0)
			push_constant.push_back(0.0)

			# Loop through views just in case we're doing stereo rendering. No extra cost if this is mono.
			var view_count = render_scene_buffers.get_view_count()
			for view in range(view_count):
				# Get the RID for our color image, we will be reading from and writing to it.
				var input_image = render_scene_buffers.get_color_layer(view)

				# Create a uniform set, this will be cached, the cache will be cleared if our viewports configuration is changed.
				var uniform : RDUniform = RDUniform.new()
				uniform.uniform_type = RenderingDevice.UNIFORM_TYPE_IMAGE
				uniform.binding = 0
				uniform.add_id(input_image)
				var uniform_set = UniformSetCacheRD.get_cache(shader, 0, [ uniform ])

				# Run our compute shader.
				var compute_list := rd.compute_list_begin()
				rd.compute_list_bind_compute_pipeline(compute_list, pipeline)
				rd.compute_list_bind_uniform_set(compute_list, uniform_set, 0)
				rd.compute_list_set_push_constant(compute_list, push_constant.to_byte_array(), push_constant.size() * 4)
				rd.compute_list_dispatch(compute_list, x_groups, y_groups, z_groups)
				rd.compute_list_end()
```

在该方法开始时，我们检查是否有渲染设备，我们的回调类型是否正确，并检查我们是否有着色器。

> **注：**
>
> 对效果类型的检查只是一种安全机制。我们在 `_init` 函数中设置了这一点，但是用户可以在 UI 中更改它。

我们的 `p_render_data` 参数允许我们访问一个对象，该对象包含我们当前正在渲染的帧特定的数据。我们目前只对渲染场景缓冲区感兴趣，这些缓冲区为我们提供了对渲染引擎使用的所有内部缓冲区的访问。请注意，我们将其强制转换为 [RenderSceneBuffersRD](https://docs.godotengine.org/en/stable/classes/class_renderscenebuffersrd.html#class-renderscenebuffersrd)，以将整个 API 公开给此数据。

接下来，我们获得 `internal size`，即 3D 渲染缓冲区在升级之前的分辨率（如果适用），升级发生在我们的后处理运行之后。

根据我们的内部大小，我们计算我们的组大小，在模板着色器中查看我们的局部大小。

我们还填充推送常数，以便着色器知道我们的大小。Godot 在这里**还**不支持结构，所以我们使用 `PackedFloat32Array` 来存储这些数据。请注意，我们必须用 16 字节对齐来填充这个数组。换句话说，数组的长度需要是 4 的倍数。

现在我们循环浏览我们的视图，以防我们使用适用于立体渲染（XR）的多视图渲染。在大多数情况下，我们只有一种观点。

> **注：**
>
> 在这里使用多视图进行后处理没有性能优势，像这样单独处理视图仍然可以使 GPU 在有利的情况下使用并行性。

接下来，我们获取此视图的颜色缓冲区。这是渲染 3D 场景的缓冲区。

然后，我们准备一个 uniform 的集合，以便将颜色缓冲区传递给着色器。

请注意我们的 [UniformSetCacheRD](https://docs.godotengine.org/en/stable/classes/class_uniformsetcacherd.html#class-uniformsetcacherd) 缓存的使用，它确保我们可以在每一帧中检查我们的 uniform 集。由于我们的颜色缓冲区可以逐帧变化，并且当缓冲区被释放时，我们的统一缓存会自动清理统一集，这是确保我们不会泄漏内存或使用过时集的安全方法。

最后，我们通过绑定流水线、绑定 uniform 集、推送我们的推送常量数据和调用我们组的调度来构建计算列表。

完成合成器效果后，我们现在需要将其添加到合成器中。

在合成器上，我们展开合成器效果属性，然后按添加元素。

现在我们可以添加合成器效果：

![../../_images/add_compositor_effect.webp](https://docs.godotengine.org/en/stable/_images/add_compositor_effect.webp)

选择 `PostProcessShader` 后，我们需要设置用户着色器代码：

```gdscript
float gray = color.r * 0.2125 + color.g * 0.7154 + color.b * 0.0721;
color.rgb = vec3(gray);
```

完成所有这些后，我们的输出是灰度的。

![../../_images/post_process_shader.webp](https://docs.godotengine.org/en/stable/_images/post_process_shader.webp)

> **注：**
>
> 有关后期效果的更高级示例，请查看 Bastiaan Olij 创建的[基于径向模糊的天空光线](https://github.com/BastiaanOlij/RERadialSunRays)示例项目。

### 用户贡献的笔记

在提交评论之前，请阅读[用户贡献笔记政策](https://github.com/godotengine/godot-docs-user-notes/discussions/1)。

**1 个评论**



[SephReed](https://github.com/SephReed) [Nov 22, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/261#discussioncomment-11344519)

以下是一个指向 github 讨论的链接，其中包含各种后处理方法和信息：
[godotengine/godot#99491](https://github.com/godotengine/godot/issues/99491)



# 着色器

## 使用计算着色器

https://docs.godotengine.org/en/stable/tutorials/shaders/compute_shaders.html#using-compute-shaders

本教程将引导您完成创建最小计算着色器的过程。但首先，我们来了解一下计算着色器的背景以及它们如何与 Godot 配合使用。

> **注：**
>
> 本教程假设您通常熟悉着色器。如果您是着色器新手，请在继续本教程之前阅读[着色器简介](https://docs.godotengine.org/en/stable/tutorials/shaders/introduction_to_shaders.html#doc-introduction-to-shaders)和[您的第一个着色器](https://docs.godotengine.org/en/stable/tutorials/shaders/your_first_shader/index.html#toc-your-first-shader)。

计算着色器是一种特殊类型的着色器程序，面向通用编程。换句话说，它们比顶点着色器和片段着色器更灵活，因为它们没有固定的目的（即变换顶点或将颜色写入图像）。与片段着色器和顶点着色器不同，计算着色器在幕后几乎没有什么作用。你编写的代码是 GPU 运行的代码，其他很少。这可以使它们成为将繁重计算卸载到 GPU 的非常有用的工具。

现在，让我们开始创建一个简短的计算着色器。

首先，在您选择的**外部**文本编辑器中，在项目文件夹中创建一个名为 `compute_example.glsl` 的新文件。在 Godot 中编写计算着色器时，可以直接在 GLSL 中编写。Godot 着色器语言基于 GLSL。如果您熟悉 Godot 中的普通着色器，下面的语法看起来会有点熟悉。

> **注：**
>
> 计算着色器只能从基于 RenderingDevice 的渲染器（Forward+ 或 Mobile 渲染器）中使用。要按照本教程进行操作，请确保您使用的是 Forward+ 或 Mobile 渲染器。其设置位于编辑器的右上角。

请注意，移动设备上的计算着色器支持通常很差（由于驱动程序错误），即使它们在技术上得到了支持。

让我们来看看这个计算着色器代码：

```glsl
#[compute]
#version 450

// Invocations in the (x, y, z) dimension
layout(local_size_x = 2, local_size_y = 1, local_size_z = 1) in;

// A binding to the buffer we create in our script
layout(set = 0, binding = 0, std430) restrict buffer MyDataBuffer {
    float data[];
}
my_data_buffer;

// The code we want to execute in each invocation
void main() {
    // gl_GlobalInvocationID.x uniquely identifies this invocation across all work groups
    my_data_buffer.data[gl_GlobalInvocationID.x] *= 2.0;
}
```

此代码获取一个浮点数数组，将每个元素乘以 2，并将结果存储回缓冲区数组中。现在让我们逐行看一下。

```glsl
#[compute]
#version 450
```

这两行传达了两件事：

1. 以下代码是一个计算着色器。这是编辑器正确导入着色器文件所需的 Godot 特定提示。
2. 代码使用 GLSL 版本 450。

您不应该为自定义计算着色器更改这两行。

```glsl
// Invocations in the (x, y, z) dimension
layout(local_size_x = 2, local_size_y = 1, local_size_z = 1) in;
```

接下来，我们传达每个工作组中要使用的调用数量。调用是在同一工作组中运行的着色器实例。当我们从 CPU 启动计算着色器时，我们告诉它要运行多少个工作组。工作组彼此并行运行。在运行一个工作组时，您无法访问另一工作组中的信息。但是，同一工作组中的调用可以对其他调用进行一些有限的访问。

将工作组和调用视为一个巨大的嵌套 `for` 循环。

```c#
for (int x = 0; x < workgroup_size_x; x++) {
  for (int y = 0; y < workgroup_size_y; y++) {
     for (int z = 0; z < workgroup_size_z; z++) {
        // Each workgroup runs independently and in parallel.
        for (int local_x = 0; local_x < invocation_size_x; local_x++) {
           for (int local_y = 0; local_y < invocation_size_y; local_y++) {
              for (int local_z = 0; local_z < invocation_size_z; local_z++) {
                 // Compute shader runs here.
              }
           }
        }
     }
  }
}
```

工作组和调用是一个高级主题。现在，请记住，我们将为每个工作组运行两次调用。

```c#
// A binding to the buffer we create in our script
layout(set = 0, binding = 0, std430) restrict buffer MyDataBuffer {
    float data[];
}
my_data_buffer;
```

在这里，我们提供了有关计算着色器可以访问的内存的信息。`layout` 属性允许我们告诉着色器在哪里查找缓冲区，我们稍后需要从 CPU 端匹配这些 `set` 和 `binding` 位置。

`restrict` 关键字告诉着色器只能从此着色器中的一个位置访问此缓冲区。换句话说，我们不会将此缓冲区绑定到另一个 `set` 或 `binding` 索引中。这很重要，因为它允许着色器编译器优化着色器代码。尽可能使用限制。

这是一个*无大小*的缓冲区，这意味着它可以是任何大小。因此，我们需要小心，不要从大于缓冲区大小的索引中读取。

```glsl
// The code we want to execute in each invocation
void main() {
    // gl_GlobalInvocationID.x uniquely identifies this invocation across all work groups
    my_data_buffer.data[gl_GlobalInvocationID.x] *= 2.0;
}
```

最后，我们编写 `main` 函数，这是所有逻辑发生的地方。我们使用内置的 `gl_GlobalInvocationID` 变量访问存储缓冲区中的位置。`gl_GlobalInvocationID` 为当前调用提供全局唯一ID。

要继续，请将上述代码写入新创建的 `compute_example.glsl` 文件。

### 创建本地渲染设备

要与计算着色器交互并执行计算着色器，我们需要一个脚本。用您选择的语言创建一个新脚本，并将其附加到场景中的任何节点。

现在要执行着色器，我们需要一个本地 [RenderingDevice](https://docs.godotengine.org/en/stable/classes/class_renderingdevice.html#class-renderingdevice)，可以使用 [RenderingServer](https://docs.godotengine.org/en/stable/classes/class_renderingserver.html#class-renderingserver) 创建：

```gdscript
# Create a local rendering device.
var rd := RenderingServer.create_local_rendering_device()
```

```c#
// Create a local rendering device.
var rd = RenderingServer.CreateLocalRenderingDevice();
```

之后，我们可以加载新创建的着色器文件 `compute_example.glsl`，并使用以下命令创建它的预编译版本：

```gdscript
# Load GLSL shader
var shader_file := load("res://compute_example.glsl")
var shader_spirv: RDShaderSPIRV = shader_file.get_spirv()
var shader := rd.shader_create_from_spirv(shader_spirv)
```

```c#
// Load GLSL shader
var shaderFile = GD.Load<RDShaderFile>("res://compute_example.glsl");
var shaderBytecode = shaderFile.GetSpirV();
var shader = rd.ShaderCreateFromSpirV(shaderBytecode);
```

> **警告：**
>
> 无法使用 [RenderDoc](https://renderdoc.org/) 等工具调试本地 RenderingDevices。

### 提供输入数据

您可能还记得，我们想将一个输入数组传递给着色器，将每个元素乘以 2，得到结果。

我们需要创建一个缓冲区，将值传递给计算着色器。我们正在处理一个浮点数数组，因此我们将在本例中使用存储缓冲区。存储缓冲区采用字节数组，允许 CPU 与 GPU 之间传输数据。

因此，让我们初始化一个浮点数数组并创建一个存储缓冲区：

```gdscript
# Prepare our data. We use floats in the shader, so we need 32 bit.
var input := PackedFloat32Array([1, 2, 3, 4, 5, 6, 7, 8, 9, 10])
var input_bytes := input.to_byte_array()

# Create a storage buffer that can hold our float values.
# Each float has 4 bytes (32 bit) so 10 x 4 = 40 bytes
var buffer := rd.storage_buffer_create(input_bytes.size(), input_bytes)
```

```c#
// Prepare our data. We use floats in the shader, so we need 32 bit.
var input = new float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
var inputBytes = new byte[input.Length * sizeof(float)];
Buffer.BlockCopy(input, 0, inputBytes, 0, inputBytes.Length);

// Create a storage buffer that can hold our float values.
// Each float has 4 bytes (32 bit) so 10 x 4 = 40 bytes
var buffer = rd.StorageBufferCreate((uint)inputBytes.Length, inputBytes);
```

缓冲区就绪后，我们需要告诉渲染设备使用此缓冲区。为此，我们需要创建一个 uniform（就像在普通着色器中一样），并将其分配给一个 uniform 集，稍后我们可以将其传递给着色器。

```gdscript
# Create a uniform to assign the buffer to the rendering device
var uniform := RDUniform.new()
uniform.uniform_type = RenderingDevice.UNIFORM_TYPE_STORAGE_BUFFER
uniform.binding = 0 # this needs to match the "binding" in our shader file
uniform.add_id(buffer)
var uniform_set := rd.uniform_set_create([uniform], shader, 0) # the last parameter (the 0) needs to match the "set" in our shader file
```

```c#
// Create a uniform to assign the buffer to the rendering device
var uniform = new RDUniform
{
    UniformType = RenderingDevice.UniformType.StorageBuffer,
    Binding = 0
};
uniform.AddId(buffer);
var uniformSet = rd.UniformSetCreate(new Array<RDUniform> { uniform }, shader, 0);
```

### 定义计算管道

下一步是创建一组 GPU 可以执行的指令。为此，我们需要一个管道和一个计算列表。

我们需要做的计算结果的步骤是：

1. 创建新管道。
2. 开始列出 GPU 要执行的指令。
3. 将我们的计算列表绑定到我们的管道
4. 将我们的缓冲 uniform 绑定到我们的管道上
5. 指定要使用的工作组数量
6. 结束指令列表

```gdscript
# Create a compute pipeline
var pipeline := rd.compute_pipeline_create(shader)
var compute_list := rd.compute_list_begin()
rd.compute_list_bind_compute_pipeline(compute_list, pipeline)
rd.compute_list_bind_uniform_set(compute_list, uniform_set, 0)
rd.compute_list_dispatch(compute_list, 5, 1, 1)
rd.compute_list_end()
```

```c#
// Create a compute pipeline
var pipeline = rd.ComputePipelineCreate(shader);
var computeList = rd.ComputeListBegin();
rd.ComputeListBindComputePipeline(computeList, pipeline);
rd.ComputeListBindUniformSet(computeList, uniformSet, 0);
rd.ComputeListDispatch(computeList, xGroups: 5, yGroups: 1, zGroups: 1);
rd.ComputeListEnd();
```

请注意，我们将计算着色器在 X 轴上分配 5 个工作组，在其他轴上分配一个工作组。由于我们在 X 轴上有 2 个本地调用（在着色器中指定），因此总共将启动 10 个计算着色器调用。如果读取或写入缓冲区范围之外的索引，则可能会访问着色器控制之外的内存或其他变量的部分，这可能会导致某些硬件出现问题。

### 执行计算着色器

在这一切之后，我们几乎完成了，但我们仍然需要执行我们的管道。到目前为止，我们只记录了我们希望 GPU 做什么；我们实际上还没有运行着色器程序。

要执行我们的计算着色器，我们需要将管道提交给 GPU 并等待执行完成：

```gdscript
# Submit to GPU and wait for sync
rd.submit()
rd.sync()
```

```c#
// Submit to GPU and wait for sync
rd.Submit();
rd.Sync();
```

理想情况下，您不会立即调用 `sync()` 来同步 RenderingDevice，因为这会导致 CPU 等待 GPU 完成工作。在我们的示例中，我们立即同步，因为我们希望我们的数据可以立即读取。一般来说，您希望在同步之前*至少*等待 2 或 3 帧，以便 GPU 能够与 CPU 并行运行。

> **警告：**
>
> 长时间的计算可能会导致 Windows 图形驱动程序“崩溃”，因为 TDR 是由 Windows 触发的。这是一种在经过一定时间后重新初始化图形驱动程序的机制，而图形驱动程序没有任何活动（通常为 5 到 10 秒）。

根据计算着色器执行的持续时间，您可能需要将其拆分为多个分派，以减少每个分派所需的时间并降低触发 TDR 的机会。鉴于 TDR 是时间相关的，与更快的 GPU 相比，运行给定计算着色器时，较慢的 GPU 可能更容易发生 TDR。

### 检索结果

您可能已经注意到，在示例着色器中，我们修改了存储缓冲区的内容。换句话说，着色器从我们的数组中读取数据，并再次将数据存储在同一个数组中，这样我们的结果就已经存在了。让我们检索数据并将结果打印到控制台。

```gdscript
# Read back the data from the buffer
var output_bytes := rd.buffer_get_data(buffer)
var output := output_bytes.to_float32_array()
print("Input: ", input)
print("Output: ", output)
```

```c#
// Read back the data from the buffers
var outputBytes = rd.BufferGetData(buffer);
var output = new float[input.Length];
Buffer.BlockCopy(outputBytes, 0, output, 0, outputBytes.Length);
GD.Print("Input: ", string.Join(", ", input));
GD.Print("Output: ", string.Join(", ", output));
```

有了这个，你就有了开始使用计算着色器所需的一切。

> **另请参见：**
>
> 演示项目存储库包含一个 [Compute Shader Heightmap 演示](https://github.com/godotengine/godot-demo-projects/tree/master/misc/compute_shader_heightmap)。该项目分别在 CPU 和 GPU 上执行高度贴图图像生成，这使您可以比较如何以两种不同的方式实现类似的算法（在大多数情况下，GPU 实现更快）。

### 用户贡献的笔记

在提交评论之前，请阅读[用户贡献笔记政策](https://github.com/godotengine/godot-docs-user-notes/discussions/1)。

**3 个评论**

[bitmammoth](https://github.com/bitmammoth) [Mar 19, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/17#discussioncomment-8835990)：

对于 c#，您可能需要指定 GodotCollections.Array 命名空间类型，以避免非泛型 system.collections 种类。



[Aman-Anas](https://github.com/Aman-Anas) [Jun 16, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/17#discussioncomment-9784442)

希望这些信息对在 C# 中使用大型数组的计算着色器的人有所帮助。我在网上找不到太多清晰的文档，尤其是 Godot 中的 C#。

虽然您仍然不能（在撰写本文时）将 `BufferGetData()` 调用到预先存在的已分配数组中（它是在内部分配的），但 C# 中有一些有用的方法可以避免在计算着色器工作流的其他部分进行内存分配和复制。这有助于提高性能和内存使用率。

替代使用可以在字节数组和浮点数组（或您自己的数据结构）之间进行转换的 `Buffer.BlockCopy()` 办法，可以考虑使用 `Span`s 和 `MemoryMarshal`。这里有一些例子。

> 小心这个力量！确保你的内存布局正确。BlockCopy 在大多数用例中都很快，只有在需要时才使用，例如您有大型数组。

从“提供输入数据”步骤，我们可以重新解释浮点数组。

```c#
var input = new float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
var inputBytes = MemoryMarshal.AsBytes(input.AsSpan());
```

或者，将数据存储为字节字段，并在添加数据之前对其进行强制转换（适用于某些用例）

```c#
var elements = 5;
var inputBytes = new byte[elements * sizeof(float)];
var input = MemoryMarshal.Cast<byte, float>(inputBytes);
input[0] = 1.0f; // Now inputBytes has its data changed!
```

另一个有用的技巧是将 `MemoryMarshal` 与结构体一起使用。这可以帮助清理 GLSL 代码并改进工作流程。

```c#
using System.Runtime.InteropServices;

// Pack= is how to do the alignment. 1 is the tightest,
// but make sure to check GLSL rules to make it match
// depending on your use case.
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CoolData
{
    //  public fields may be bad practice, but using for simplicity
    public int wheels; 
    public float position;
    public float hoursOfSleep;
}
// as byte array (single struct)
byte[] coolDataBytes = new byte[Marshal.SizeOf<CoolData>()];
ref var myData = ref MemoryMarshal.AsRef<CoolData>(coolDataBytes);
myData.wheels = 4;
// You can also use MemoryMarshal.Cast() if you had many structs in an array.
// (For example, many structs generated on the GPU and returned with rd.BufferGetData())
```



[bkorkmaz](https://github.com/bkorkmaz) [Aug 21, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/17#discussioncomment-10407065)

着色器的一个非常简单的解释 ：）

```glsl
#[compute]
#version 450

// Compute Shader 可以处理三维数据。 
// 这基本上是一种用于 1D、2D 和 3D 数据结构的高效设计。
// 在我们的示例中，由于我们正在处理一个 1D 数组，因此我们只使用 X 轴。
//////////////////////////////////////////////////////////////////////////////////////////////// 
// 定义我们将在 GPU 上使用的轴。这里，只有 X 轴。X 轴上有 2 个线程。
layout(local_size_x = 2, local_size_y = 1, local_size_z = 1) in;

// 将我们的数据连接到 GPU。
// set = 数据类型
// binding = GPU 绑定点
// std430 = GPU 内存结构标准。
// restrict buffer MyDataBuffer = 出于优化目的，限制访问的缓冲区。
layout(set = 0, binding = 0, std430) restrict buffer MyDataBuffer {
    float data[];
}
my_data_buffer;

void main() {
    // 将数组的每个元素乘以 2，并将结果写回同一元素的位置。
    my_data_buffer.data[gl_GlobalInvocationID.x] *= 2.0;
}
```

## 屏幕读取着色器

https://docs.godotengine.org/en/stable/tutorials/shaders/screen-reading_shaders.html#screen-reading-shaders

### 引言

通常希望制作一个着色器，从它正在写入的同一屏幕读取。由于内部硬件的限制，OpenGL 或 DirectX 等 3D API 使这一点变得非常困难。GPU 非常并行，因此读写会导致各种缓存和一致性问题。因此，即使是最先进的硬件也无法正确支持这一点。

解决方法是将屏幕或屏幕的一部分复制到后缓冲区，然后在绘图时从中读取。Godot 提供了一些工具，使这一过程变得容易。

### 屏幕纹理

Godot [着色语言](https://docs.godotengine.org/en/stable/tutorials/shaders/shader_reference/shading_language.html#doc-shading-language)有一种特殊的纹理来访问屏幕上已经渲染的内容。在声明 `sampler2D` uniform 时，通过指定一个提示来使用它：`hint_screen_texture`。可以使用特殊的内置可变 `SCREEN_UV` 来获得当前片段相对于屏幕的 UV。因此，这个 canvas_item 片段着色器会产生一个不可见的对象，因为它只显示后面的内容：

```glsl
shader_type canvas_item;

uniform sampler2D screen_texture : hint_screen_texture, repeat_disable, filter_nearest;

void fragment() {
    COLOR = textureLod(screen_texture, SCREEN_UV, 0.0);
}
```

这里使用 `textureLod` 是因为我们只想从底部 mipmap 读取。如果你想从模糊版本的纹理中读取，你可以将第三个参数增加到 `textureLod`，并将提示 `filter_nearest` 更改为 `filter_nearest_mipmap`（或启用 mipmaps 的任何其他过滤器）。如果使用带有 mipmaps 的过滤器，Godot 将自动为您计算模糊纹理。

> **警告：**
>
> 如果过滤模式未更改为名称中包含 `mipmap` 的过滤模式，则 LOD 参数大于 `0.0` 的 `textureLod` 将具有与 `0.0` LOD 参数相同的外观。

### 屏幕纹理示例

屏幕纹理可以用于许多事情。有一个专门的*屏幕空间着色器*演示，您可以下载观看和学习。一个例子是一个简单的着色器来调整亮度、对比度和饱和度：

```glsl
shader_type canvas_item;

uniform sampler2D screen_texture : hint_screen_texture, repeat_disable, filter_nearest;

uniform float brightness = 1.0;
uniform float contrast = 1.0;
uniform float saturation = 1.0;

void fragment() {
    vec3 c = textureLod(screen_texture, SCREEN_UV, 0.0).rgb;

    c.rgb = mix(vec3(0.0), c.rgb, brightness);
    c.rgb = mix(vec3(0.5), c.rgb, contrast);
    c.rgb = mix(vec3(dot(vec3(1.0), c.rgb) * 0.33333), c.rgb, saturation);

    COLOR.rgb = c;
}
```

### 幕后故事

虽然这看起来很神奇，但事实并非如此。在 2D 中，当在即将绘制的节点中首次发现 `hint_screen_texture` 时，Godot 会将全屏复制到后缓冲区。在着色器中使用它的后续节点将不会为它们复制屏幕，因为这最终会导致效率低下。在 3D 中，屏幕是在不透明几何体通过之后但在透明几何体通过之前复制的，因此透明对象不会被捕获在屏幕纹理中。

因此，在 2D 中，如果使用 `hint_screen_texture` 的着色器重叠，第二个着色器将不会使用第一个着色器的结果，从而产生意想不到的视觉效果：

![../../_images/texscreen_demo1.png](https://docs.godotengine.org/en/stable/_images/texscreen_demo1.png)

在上图中，第二个球体（右上角)使用与下面第一个球体相同的屏幕纹理源，因此第一个球体“消失”或不可见。

在二维中，这可以通过 [BackBufferCopy](https://docs.godotengine.org/en/stable/classes/class_backbuffercopy.html#class-backbuffercopy) 节点进行纠正，该节点可以在两个球体之间实例化。BackBufferCopy 可以通过指定屏幕区域或整个屏幕来工作：

![../../_images/texscreen_bbc.png](https://docs.godotengine.org/en/stable/_images/texscreen_bbc.png)

通过正确的后缓冲区复制，两个球体可以正确混合：

![../../_images/texscreen_demo2.png](https://docs.godotengine.org/en/stable/_images/texscreen_demo2.png)

> **警告：**
>
> 在 3D 中，使用 `hint_screen_texture` 的材质本身被认为是透明的，不会出现在其他材质的最终屏幕纹理中。如果您计划实例化一个使用具有 `hint_screen_texture` 材质的场景，则需要使用 BackBufferCopy 节点。

在 3D 中，解决这个特定问题的灵活性较小，因为屏幕纹理只被捕获一次。在 3D 中使用屏幕纹理时要小心，因为它不会捕获透明对象，并且可能会使用屏幕纹理捕获对象前面的一些不透明对象。

通过在与对象相同的位置创建一个带有相机的[视口](https://docs.godotengine.org/en/stable/classes/class_viewport.html#class-viewport)，然后使用[视口](https://docs.godotengine.org/en/stable/classes/class_viewport.html#class-viewport)的纹理而不是屏幕纹理，可以在 3D 中再现后缓冲区逻辑。

### 后缓冲逻辑

因此，为了更清楚起见，以下是 Godot 中 2D 中的后缓冲区复制逻辑的工作原理：

- 如果一个节点使用 `hint_screen_texture`，则在绘制该节点之前，整个屏幕都会被复制到后缓冲区。这只是第一次；后续节点不会触发此操作。
- 如果在上述情况之前处理了 BackBufferCopy 节点（即使未使用 `hint_screen_texture`），则不会发生上述情况中描述的行为。换句话说，只有在节点中首次使用 `hint_screen_texture` 并且之前没有按树顺序找到 BackBufferCopy 节点（未禁用）的情况下，才会自动复制整个屏幕。
- BackBufferCopy 可以复制整个屏幕或一个区域。如果仅设置为一个区域（而不是整个屏幕），并且着色器使用的像素不在复制的区域中，则读取的结果是未定义的（很可能是前几帧的垃圾）。换句话说，可以使用 BackBufferCopy 复制屏幕的一个区域，然后从不同的区域读取屏幕纹理。避免这种行为！

### 深度纹理

对于 3D 着色器，还可以访问屏幕深度缓冲区。为此，使用 `hint_depth_texture` 提示。这种纹理不是线性的；必须使用逆投影矩阵对其进行转换。

以下代码检索正在绘制的像素下方的 3D 位置：

```glsl
uniform sampler2D depth_texture : hint_depth_texture, repeat_disable, filter_nearest;

void fragment() {
    float depth = textureLod(depth_texture, SCREEN_UV, 0.0).r;
    vec4 upos = INV_PROJECTION_MATRIX * vec4(SCREEN_UV * 2.0 - 1.0, depth, 1.0);
    vec3 pixel_position = upos.xyz / upos.w;
}
```

### 法线-粗糙度纹理

> **注：**
>
> 法线-粗糙度纹理仅在 Forward+ 渲染方法中受支持，不支持移动或兼容性。

同样，法线-粗糙度纹理可用于读取在深度预渲染中渲染的对象的法线和粗糙度。法线存储在 `.xyz` 通道中（映射到 0-1 范围），而粗糙度存储在 `.w` 通道中。

```glsl
uniform sampler2D normal_roughness_texture : hint_normal_roughness_texture, repeat_disable, filter_nearest;

void fragment() {
    float screen_roughness = texture(normal_roughness_texture, SCREEN_UV).w;
    vec3 screen_normal = texture(normal_roughness_texture, SCREEN_UV).xyz;
    screen_normal = screen_normal * 2.0 - 1.0;
```

### 重新定义屏幕纹理

屏幕纹理提示（`hint_screen_texture`、`hint_depth_texture` 和 `hint_normal_roughness_texture`）可以与多个 uniform 一起使用。例如，您可能希望使用不同的重复标志或过滤器标志多次读取纹理。

以下示例显示了一个着色器，该着色器使用线性过滤读取屏幕空间法线，但使用最近邻过滤读取屏幕区域粗糙度。

```glsl
uniform sampler2D normal_roughness_texture : hint_normal_roughness_texture, repeat_disable, filter_nearest;
uniform sampler2D normal_roughness_texture2 : hint_normal_roughness_texture, repeat_enable, filter_linear;

void fragment() {
    float screen_roughness = texture(normal_roughness_texture, SCREEN_UV).w;
    vec3 screen_normal = texture(normal_roughness_texture2, SCREEN_UV).xyz;
    screen_normal = screen_normal * 2.0 - 1.0;
```

### 用户贡献的笔记

在提交评论之前，请阅读[用户贡献笔记政策](https://github.com/godotengine/godot-docs-user-notes/discussions/1)。

**3 个评论** · 1 个回复



[DeveCout](https://github.com/DeveCout) [Sep 19, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/151#discussioncomment-10692291)

我找不到测试镜头效果的测试项目。缺少链接还是怎么了？

​	[Calinou](https://github.com/Calinou) [Sep 20, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/151#discussioncomment-10706855) Member

​	不幸的是，我认为这个测试项目没有在任何地方发布。此时，它必须从头开始重新创建。



[thompsop1sou](https://github.com/thompsop1sou) [Nov 20, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/151#discussioncomment-11318374)

本文档中提到了 3D 透明对象的一个重大限制：

在 3D 中，屏幕是在不透明几何体通过之后但在透明几何体通过之前复制的，因此透明对象不会被捕获在屏幕纹理中。

这不仅会影响屏幕纹理，还会影响深度和法线-粗糙度纹理。

有几种方法可以解决这个问题，这样你就可以对透明对象进行后处理效果：

- 如果只使用屏幕纹理（不是深度或法线-粗糙度），请使用 2D 着色器。当您在 2D 着色器中访问屏幕纹理时，将显示透明对象。（我认为这是因为 3D 渲染已经全部完成。）这不适用于深度或法线-粗糙度纹理，因为这些纹理在 2D 着色器中不可用。
- 使用[合成器效果](https://docs.godotengine.org/en/stable/tutorials/rendering/compositor.html)。这是一个较低级别的解决方案，需要编写 GLSL 并像在计算着色器中一样传递数据。请注意，您需要将透明对象设置为“始终深度绘制”，以便它们显示在深度缓冲区中。目前，还没有办法让透明物体在法线-粗糙度纹理中显示出来。
- 您可以使用具有多个相机和视口的设置来手动捕获和传递纹理。设置起来有点复杂，但它最终会给你比合成器效果更多的控制权。您可以访问颜色、深度、法线或粗糙度数据，也可以传递自己的自定义数据。我在这里有一个示例项目：https://github.com/thompsop1sou/custom-screen-buffers



[SephReed](https://github.com/SephReed) [Nov 20, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/151#discussioncomment-11319986)

有人能举一个使用屏幕空间着色器的 3d 场景的最小可行示例吗？它可能只是一个灰度着色器，但它会有很大的帮助。



## 使用 SubViewport 作为纹理

https://docs.godotengine.org/en/stable/tutorials/shaders/using_viewport_as_texture.html#using-a-subviewport-as-a-texture

### 引言

本教程将向您介绍如何将 [SubViewport](https://docs.godotengine.org/en/stable/classes/class_subviewport.html#class-subviewport) 用作可以应用于 3D 对象的纹理。为了做到这一点，它将引导您完成制作如下程序行星的过程：

![../../_images/planet_example.png](https://docs.godotengine.org/en/stable/_images/planet_example.png)

> **注：**
>
> 本教程不介绍如何编写像这个星球这样的动态大气。

本教程假设您熟悉如何设置基本场景，包括：[Camera3D](https://docs.godotengine.org/en/stable/classes/class_camera3d.html#class-camera3d)、[光源](https://docs.godotengine.org/en/stable/classes/class_omnilight3d.html#class-omnilight3d)、具有[基本网格（Primitive Mesh）](https://docs.godotengine.org/en/stable/classes/class_primitivemesh.html#class-primitivemesh)的 [MeshInstance3D](https://docs.godotengine.org/en/stable/classes/class_meshinstance3d.html#class-meshinstance3d)，以及将 [StandardMaterial3D](https://docs.godotengine.org/en/stable/classes/class_standardmaterial3d.html#class-standardmaterial3d) 应用于网格。重点将放在使用 [SubViewport](https://docs.godotengine.org/en/stable/classes/class_subviewport.html#class-subviewport) 动态创建可以应用于网格的纹理上。

在本教程中，我们将介绍以下主题：

- 如何将 [SubViewport](https://docs.godotengine.org/en/stable/classes/class_subviewport.html#class-subviewport) 用作渲染纹理
- 使用等矩形映射将纹理映射到球体
- 程序行星的片段着色器技术
- 从[视口纹理](https://docs.godotengine.org/en/stable/classes/class_viewporttexture.html#class-viewporttexture)设置粗糙度贴图

### 设置场景

创建一个新场景，并添加以下节点，如下所示。

![../../_images/viewport_texture_node_tree.webp](https://docs.godotengine.org/en/stable/_images/viewport_texture_node_tree.webp)

进入 MeshInstance3D，将网格设置为 SphereMesh

### 设置 SubViewport

单击 [SubViewport](https://docs.godotengine.org/en/stable/classes/class_subviewport.html#class-subviewport) 节点并将其大小设置为 `(1024, 512)`。[SubViewport](https://docs.godotengine.org/en/stable/classes/class_subviewport.html#class-subviewport) 实际上可以是任何大小，只要宽度是高度的两倍。宽度需要是高度的两倍，这样图像才能准确地映射到球体上，因为我们将使用等矩形投影，但稍后会详细介绍。

接下来禁用 3D。我们将使用 [ColorRect](https://docs.godotengine.org/en/stable/classes/class_colorrect.html#class-colorrect) 渲染曲面，因此我们也不需要 3D。

![../../_images/planet_new_viewport.webp](https://docs.godotengine.org/en/stable/_images/planet_new_viewport.webp)

选择 [ColorRect](https://docs.godotengine.org/en/stable/classes/class_colorrect.html#class-colorrect)，然后在检查器中将锚点预设为 `Full Rect`。这将确保 [ColorRect](https://docs.godotengine.org/en/stable/classes/class_colorrect.html#class-colorrect) 占据整个 [SubViewport](https://docs.godotengine.org/en/stable/classes/class_subviewport.html#class-subviewport)。

![../../_images/planet_new_colorrect.webp](https://docs.godotengine.org/en/stable/_images/planet_new_colorrect.webp)

接下来，我们将[着色器材质](https://docs.godotengine.org/en/stable/classes/class_shadermaterial.html#class-shadermaterial)添加到 [ColorRect](https://docs.godotengine.org/en/stable/classes/class_colorrect.html#class-colorrect) 中（ColorRect > CanvasItem > Material > Material > `New ShaderMaterial`)。

> **注：**
>
> 本教程建议您对着色有基本的了解。但是，即使您是着色器的新手，也会提供所有代码，因此您应该可以轻松理解。

单击着色器材质的下拉菜单按钮，然后单击/编辑。从这里转到“着色器”>“`新建着色器`”。给它起个名字，然后单击“创建”。在检查器中单击着色器以打开着色器编辑器。删除默认代码并添加以下内容：

```glsl
shader_type canvas_item;

void fragment() {
    COLOR = vec4(UV.x, UV.y, 0.5, 1.0);
}
```

保存着色器代码，您将在检查器中看到上面的代码渲染了一个类似下面的渐变。

![../../_images/planet_gradient.png](https://docs.godotengine.org/en/stable/_images/planet_gradient.png)

现在我们有了渲染的 [SubViewport](https://docs.godotengine.org/en/stable/classes/class_subviewport.html#class-subviewport) 的基础知识，我们有了一个可以应用于球体的独特图像。

### 应用纹理

现在进入 [MeshInstance3D](https://docs.godotengine.org/en/stable/classes/class_meshinstance3d.html#class-meshinstance3d) 并向其中添加一个 [StandardMaterial3D](https://docs.godotengine.org/en/stable/classes/class_standardmaterial3d.html#class-standardmaterial3d)。不需要特殊的[着色器材质](https://docs.godotengine.org/en/stable/classes/class_shadermaterial.html#class-shadermaterial)（尽管这对于更高级的效果来说是个好主意，比如上面例子中的氛围）。

MeshInstance3D > GeometryInstance > Geometry > Material Override > `New StandardMaterial3D`

然后单击 StandardMaterial3D 的下拉菜单，然后单击“编辑”

转到“资源”部分，选中“`本地到场景`”框。然后，转到“反照率”部分，单击“纹理”属性旁边的以添加反照率纹理。在这里，我们将应用我们制作的纹理。选择“新建视口纹理”

![../../_images/planet_new_viewport_texture.webp](https://docs.godotengine.org/en/stable/_images/planet_new_viewport_texture.webp)

单击您刚刚在检查器中创建的 ViewportTexture，然后单击“分配”。然后，从弹出的菜单中，选择我们之前渲染的 Viewport。

![../../_images/planet_pick_viewport_texture.webp](https://docs.godotengine.org/en/stable/_images/planet_pick_viewport_texture.webp)

现在，您的球体应该使用我们渲染到视口的颜色着色。

![../../_images/planet_seam.webp](https://docs.godotengine.org/en/stable/_images/planet_seam.webp)

注意到纹理环绕处形成的丑陋接缝了吗？这是因为我们根据 UV 坐标选取颜色，而 UV 坐标不会环绕纹理。这是二维地图投影中的一个经典问题。游戏开发人员通常有一个二维地图，他们想投影到球体上，但当它环绕时，它会有很大的接缝。对于这个问题，有一个优雅的解决方法，我们将在下一节中加以说明。

### 制作星球纹理

所以现在，当我们渲染到 [SubViewport](https://docs.godotengine.org/en/stable/classes/class_subviewport.html#class-subviewport) 时，它会神奇地出现在球体上。但是，我们的纹理坐标产生了一个丑陋的接缝。那么，我们如何得到一个以一种很好的方式围绕球体的坐标范围呢？一种解决方案是使用一个在纹理域上重复的函数。`sin` 和 `cos` 是两个这样的函数。让我们将它们应用于纹理，看看会发生什么。用以下内容替换着色器中的现有颜色代码：

```glsl
COLOR.xyz = vec3(sin(UV.x * 3.14159 * 4.0) * cos(UV.y * 3.14159 * 4.0) * 0.5 + 0.5);
```

![../../_images/planet_sincos.webp](https://docs.godotengine.org/en/stable/_images/planet_sincos.webp)

还不错。如果你环顾四周，你可以看到接缝现在已经消失了，但在它的位置上，我们挤压了极地。这种挤压是由于 Godot 在其 [StandardMaterial3D](https://docs.godotengine.org/en/stable/classes/class_standardmaterial3d.html#class-standardmaterial3d) 中将纹理映射到球体的方式造成的。它使用一种称为等矩形投影的投影技术，将球面图转换到二维平面上。

> **注：**
>
> 如果你对这项技术感兴趣，我们将把球坐标转换为笛卡尔坐标。球坐标表示球体的经度和纬度，而笛卡尔坐标实际上是从球体中心到该点的向量。

对于每个像素，我们将计算其在球体上的 3D 位置。由此，我们将使用 3D 噪声来确定颜色值。通过计算三维噪声，我们解决了极点处的夹紧问题。要理解原因，请想象一下在球体表面而不是二维平面上计算的噪声。当你在球体的整个表面进行计算时，你永远不会碰到边缘，因此你永远不会在极点上创建接缝或夹点。以下代码将 `UVs` 转换为笛卡尔坐标。

```glsl
float theta = UV.y * 3.14159;
float phi = UV.x * 3.14159 * 2.0;
vec3 unit = vec3(0.0, 0.0, 0.0);

unit.x = sin(phi) * sin(theta);
unit.y = cos(theta) * -1.0;
unit.z = cos(phi) * sin(theta);
unit = normalize(unit);
```

如果我们使用 `unit` 作为输出 `COLOR` 值，我们得到：

![../../_images/planet_normals.webp](https://docs.godotengine.org/en/stable/_images/planet_normals.webp)

现在我们可以计算球体表面的 3D 位置，我们可以使用 3D 噪波来制作行星。我们将直接从 [Shadertoy](https://www.shadertoy.com/view/Xsl3Dl) 中使用此噪声函数：

```glsl
vec3 hash(vec3 p) {
    p = vec3(dot(p, vec3(127.1, 311.7, 74.7)),
             dot(p, vec3(269.5, 183.3, 246.1)),
             dot(p, vec3(113.5, 271.9, 124.6)));

    return -1.0 + 2.0 * fract(sin(p) * 43758.5453123);
}

float noise(vec3 p) {
  vec3 i = floor(p);
  vec3 f = fract(p);
  vec3 u = f * f * (3.0 - 2.0 * f);

  return mix(mix(mix(dot(hash(i + vec3(0.0, 0.0, 0.0)), f - vec3(0.0, 0.0, 0.0)),
                     dot(hash(i + vec3(1.0, 0.0, 0.0)), f - vec3(1.0, 0.0, 0.0)), u.x),
                 mix(dot(hash(i + vec3(0.0, 1.0, 0.0)), f - vec3(0.0, 1.0, 0.0)),
                     dot(hash(i + vec3(1.0, 1.0, 0.0)), f - vec3(1.0, 1.0, 0.0)), u.x), u.y),
             mix(mix(dot(hash(i + vec3(0.0, 0.0, 1.0)), f - vec3(0.0, 0.0, 1.0)),
                     dot(hash(i + vec3(1.0, 0.0, 1.0)), f - vec3(1.0, 0.0, 1.0)), u.x),
                 mix(dot(hash(i + vec3(0.0, 1.0, 1.0)), f - vec3(0.0, 1.0, 1.0)),
                     dot(hash(i + vec3(1.0, 1.0, 1.0)), f - vec3(1.0, 1.0, 1.0)), u.x), u.y), u.z );
}
```

> **注：**
>
> 这一切都归功于作者 Inigo Quilez。它是在 `MIT` 的许可下出版的。

现在要使用 `noise`，请在 `fragment` 函数中添加以下内容：

```glsl
float n = noise(unit * 5.0);
COLOR.xyz = vec3(n * 0.5 + 0.5);
```

![../../_images/planet_noise.webp](https://docs.godotengine.org/en/stable/_images/planet_noise.webp)

> **注：**
>
> 为了突出显示纹理，我们将材质设置为无阴影。

现在你可以看到，噪音确实无缝地环绕着球体。虽然这看起来一点也不像你被承诺的星球。那么，让我们转向更丰富多彩的东西。

### 为星球着色

现在，让星球变彩色。虽然有很多方法可以做到这一点，但就目前而言，我们将坚持水和陆地之间的渐变。

为了在 GLSL 中制作渐变，我们使用了 `mix` 函数。`mix` 需要两个值进行插值，第三个参数选择在它们之间插值多少；本质上，它将这两种价值观混合在一起。在其他 API 中，此函数通常称为 `lerp`。然而，`lerp` 通常用于将两个浮点数混合在一起；`mix` 可以取任何值，无论是浮点数还是向量类型。

```glsl
COLOR.xyz = mix(vec3(0.05, 0.3, 0.5), vec3(0.9, 0.4, 0.1), n * 0.5 + 0.5);
```

第一种颜色是蓝色，代表海洋。第二种颜色是一种红色（因为所有外星行星都需要红色地形）。最后，它们通过 `n * 0.5 + 0.5` 混合在一起。`n` 在 `-1` 和 `1` 之间平滑变化。因此，我们将其映射到 `mix` 预期的 `0-1` 范围内。现在你可以看到颜色在蓝色和红色之间变化。

![../../_images/planet_noise_color.webp](https://docs.godotengine.org/en/stable/_images/planet_noise_color.webp)

这比我们想要的要模糊一点。行星通常在陆地和海洋之间有相对清晰的分隔。为了做到这一点，我们将把最后一项更改为 `smoothstep(-0.1, 0.0, n)`。因此，整行变成：

```glsl
COLOR.xyz = mix(vec3(0.05, 0.3, 0.5), vec3(0.9, 0.4, 0.1), smoothstep(-0.1, 0.0, n));
```

`smoothstep` 的作用是，如果第三个参数低于第一个参数，则返回 `0`，如果第二个参数大于第三个变量，则返回 `1`，如果第三数在第一个和第二个之间，则在 `0` 和 `1` 之间平滑混合。因此，在这一行中，当 `n` 小于 `-0.1` 时，`smoothstep` 返回 `0`，当 `n` 大于 `0` 时，它返回 `1`。

![../../_images/planet_noise_smooth.webp](https://docs.godotengine.org/en/stable/_images/planet_noise_smooth.webp)

还有一件事，让这里更像星球。土地不应该这么松软；让我们把边缘弄粗糙一点。着色器中经常使用的一种技巧是在不同频率下将不同级别的噪声叠加在一起，以制作带有噪声的粗糙地形。我们使用一层来制作大陆的整体块状结构。然后，另一层将边缘分解一点，然后是另一层，以此类推。我们要做的是用四行着色器代码而不是一行来计算 `n`。`n` 变为：

```glsl
float n = noise(unit * 5.0) * 0.5;
n += noise(unit * 10.0) * 0.25;
n += noise(unit * 20.0) * 0.125;
n += noise(unit * 40.0) * 0.0625;
```

现在星球看起来像：

![../../_images/planet_noise_fbm.webp](https://docs.godotengine.org/en/stable/_images/planet_noise_fbm.webp)

### 创造海洋

最后一件事让它看起来更像一颗行星。海洋和陆地反射光线的方式不同。所以我们希望海洋比陆地更明亮。我们可以通过将第四个值传递到输出 `COLOR` 的 `alpha` 通道并将其用作粗糙度贴图来实现这一点。

```glsl
COLOR.a = 0.3 + 0.7 * smoothstep(-0.1, 0.0, n);
```

这行对于水返回 `0.3`，对于陆地返回 `1.0`。这意味着土地将非常崎岖，而水将非常光滑。

然后，在材质的“金属”部分下，确保“`Metallic`”设置为 `0`，“`Specular`”设置为 `1`。原因是水很好地反射光线，但不是金属的。这些值在物理上并不准确，但对于这个演示来说已经足够了。

接下来，在“粗糙度”部分下，将粗糙度纹理设置为指向我们的行星纹理 [SubViewport](https://docs.godotengine.org/en/stable/classes/class_viewporttexture.html#class-viewporttexture) 的“[视口纹理](https://docs.godotengine.org/en/stable/classes/class_viewporttexture.html#class-viewporttexture)”。最后，将“`纹理通道`”设置为 `Alpha`。这指示渲染器将输出 `COLOR` 的 `alpha` 通道用作`粗糙度`值。

![../../_images/planet_ocean.webp](https://docs.godotengine.org/en/stable/_images/planet_ocean.webp)

你会注意到，除了地球不再反射天空之外，变化很小。这是因为，默认情况下，当使用 alpha 值渲染某物时，它会被绘制为背景上的透明对象。由于 [SubViewport](https://docs.godotengine.org/en/stable/classes/class_subviewport.html#class-subviewport) 的默认背景是不透明的，因此“[视口纹理](https://docs.godotengine.org/en/stable/classes/class_viewporttexture.html#class-viewporttexture)”的 `alpha` 通道为1，导致行星纹理的绘制颜色略微暗淡，各处的“`粗糙度`”值为 `1`。为了纠正这一点，我们进入 [SubViewport](https://docs.godotengine.org/en/stable/classes/class_subviewport.html#class-subviewport) 并启用“Transparent Bg”属性。由于我们现在正在渲染一个透明对象在另一个之上，我们希望启用`blend_premul_alpha`：

```glsl
render_mode blend_premul_alpha;
```

这会将颜色预先乘以 `alpha` 值，然后将它们正确地混合在一起。通常，当在另一种透明颜色上混合一种透明色时，即使背景的 `alpha` 为 `0`（就像在这种情况下一样），你最终也会遇到奇怪的颜色溢出问题。设置 `blend_premul_alpha` 可以解决这个问题。

现在，地球应该看起来像是在反射海洋上的光，而不是陆地上的光。在场景中围绕 [OmniLight3D](https://docs.godotengine.org/en/stable/classes/class_omnilight3d.html#class-omnilight3d) 移动，以便可以看到海洋反射的效果。

![../../_images/planet_ocean_reflect.webp](https://docs.godotengine.org/en/stable/_images/planet_ocean_reflect.webp)

这就是它。一个使用[子视口](https://docs.godotengine.org/en/stable/classes/class_subviewport.html#class-subviewport)生成的程序行星。

### 用户贡献的笔记

在提交评论之前，请阅读[用户贡献笔记政策](https://github.com/godotengine/godot-docs-user-notes/discussions/1)。

**1 个评论** · 1 个回复



[rybern](https://github.com/rybern) [Nov 11, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/246#discussioncomment-11206823)

这是一个非常有用的教程，但我认为我可以看到两件事需要改进。记住，我是个初学者，所以我可能错了。

1. 您可以预先计算 3d 噪波纹理，并将其作为均匀纹理传递，而不是在着色器中计算。
2. 这可以通过空间着色器以相同的方式完成，但您根本不需要设置子视口。也许我错过了好处？

​	[tetrapod00](https://github.com/tetrapod00) [Nov 12, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/246#discussioncomment-11219617)

 	1. 是的！您可以预先计算噪声并将其传入。
 	2. 你说得对，复制这种特定效果不需要视口。它可以在单个空间着色器中完成。本教程是关于使用子视口作为纹理的，并且恰好使用了这个示例。

​	在某些时候，可以用一个使用 ViewportTexture 的实际*需求*示例重写此页面。如果你有任何想法，请随时在[文档仓库](https://github.com/godotengine/godot-docs/issues)中打开一个问题。



## 自定义后期处理

### 引言

Godot 提供了许多开箱即用的后处理效果，包括 Bloom、DOF 和 SSAO，这些效果在[环境和后处理](https://docs.godotengine.org/en/stable/tutorials/3d/environment_and_post_processing.html#doc-environment-and-post-processing)中进行了描述。但是，高级用例可能需要自定义效果。本文解释了如何编写自己的自定义效果。

实现自定义后处理着色器的最简单方法是使用 Godot 的内置功能从屏幕纹理读取。如果你对此不熟悉，你应该先阅读[屏幕读取着色器教程](https://docs.godotengine.org/en/stable/tutorials/shaders/screen-reading_shaders.html#doc-screen-reading-shaders)。

### 单通道后期处理

后处理效果是 Godot 渲染后应用于帧的着色器。要将着色器应用于帧，请创建 [CanvasLayer](https://docs.godotengine.org/en/stable/classes/class_canvaslayer.html#class-canvaslayer)，并为其赋予 [ColorRect](https://docs.godotengine.org/en/stable/classes/class_colorrect.html#class-colorrect)。为新创建的 `ColorRect` 指定一个新的 [ShaderMaterial](https://docs.godotengine.org/en/stable/classes/class_shadermaterial.html#class-shadermaterial)，并将 `ColorRect` 的布局设置为“完全矩形”。

您的场景树看起来像这样：

![../../_images/post_tree1.png](https://docs.godotengine.org/en/stable/_images/post_tree1.png)

> **注：**
>
> 另一种更有效的方法是使用 [BackBufferCopy](https://docs.godotengine.org/en/stable/classes/class_backbuffercopy.html#class-backbuffercopy) 将屏幕的一个区域复制到缓冲区，并通过 `sampler2D` 使用 `hint_screen_texture` 在着色器脚本中访问它。

> **注：**
>
> 在撰写本文时，Godot 不支持同时渲染到多个缓冲区。后处理着色器将无法访问 Godot 未公开的其他渲染过程和缓冲区（如深度或法线/粗糙度）。您只能访问 Godot 作为采样器公开的渲染帧和缓冲区。

在这个演示中，我们将使用这种绵羊 [Sprite](https://docs.godotengine.org/en/stable/classes/class_sprite2d.html#class-sprite2d)。

![../../_images/post_example1.png](https://docs.godotengine.org/en/stable/_images/post_example1.png)

为 `ColorRect` 的`着色器材质`指定一个新的[着色器](https://docs.godotengine.org/en/stable/classes/class_shader.html#class-shader)。您可以使用 `hint_screen_texture` 和内置的 `SCREEN_UV` 制服，使用 `sampler2D` 访问帧的纹理和 UV。

将以下代码复制到着色器中。下面的代码是 [arlez80](https://bitbucket.org/arlez80/hex-mosaic/src/master/) 的十六进制像素化着色器，

```glsl
shader_type canvas_item;

uniform vec2 size = vec2(32.0, 28.0);
// If you intend to read from mipmaps with `textureLod()` LOD values greater than `0.0`,
// use `filter_nearest_mipmap` instead. This shader doesn't require it.
uniform sampler2D screen_texture : hint_screen_texture, repeat_disable, filter_nearest;

void fragment() {
        vec2 norm_size = size * SCREEN_PIXEL_SIZE;
        bool half = mod(SCREEN_UV.y / 2.0, norm_size.y) / norm_size.y < 0.5;
        vec2 uv = SCREEN_UV + vec2(norm_size.x * 0.5 * float(half), 0.0);
        vec2 center_uv = floor(uv / norm_size) * norm_size;
        vec2 norm_uv = mod(uv, norm_size) / norm_size;
        center_uv += mix(vec2(0.0, 0.0),
                         mix(mix(vec2(norm_size.x, -norm_size.y),
                                 vec2(0.0, -norm_size.y),
                                 float(norm_uv.x < 0.5)),
                             mix(vec2(0.0, -norm_size.y),
                                 vec2(-norm_size.x, -norm_size.y),
                                 float(norm_uv.x < 0.5)),
                             float(half)),
                         float(norm_uv.y < 0.3333333) * float(norm_uv.y / 0.3333333 < (abs(norm_uv.x - 0.5) * 2.0)));

        COLOR = textureLod(screen_texture, center_uv, 0.0);
}
```

羊看起来像这样：

![../../_images/post_example2.png](https://docs.godotengine.org/en/stable/_images/post_example2.png)

### 多通道后期处理

一些后处理效果，如模糊，是资源密集型的。如果你把他们打散到多个通道中，你可以让他们运行得更快。在多通道材质中，每个通道都将前一通道的结果作为输入并对其进行处理。

要生成多通道后处理着色器，可以堆叠 `CanvasLayer` 和 `ColorRect` 节点。在上面的示例中，您使用 `CanvasLayer` 对象使用下面层上的帧渲染着色器。除了节点结构之外，步骤与单通道后处理着色器相同。

您的场景树看起来像这样：

![../../_images/post_tree2.png](https://docs.godotengine.org/en/stable/_images/post_tree2.png)

例如，您可以通过将以下代码附加到每个 `ColorRect` 节点来编写全屏高斯模糊效果。应用着色器的顺序取决于 `CanvasLayer` 在场景树中的位置，越高表示越快。对于这个模糊着色器，顺序并不重要。

```glsl
shader_type canvas_item;

uniform sampler2D screen_texture : hint_screen_texture, repeat_disable, filter_nearest;

// Blurs the screen in the X-direction.
void fragment() {
    vec3 col = texture(screen_texture, SCREEN_UV).xyz * 0.16;
    col += texture(screen_texture, SCREEN_UV + vec2(SCREEN_PIXEL_SIZE.x, 0.0)).xyz * 0.15;
    col += texture(screen_texture, SCREEN_UV + vec2(-SCREEN_PIXEL_SIZE.x, 0.0)).xyz * 0.15;
    col += texture(screen_texture, SCREEN_UV + vec2(2.0 * SCREEN_PIXEL_SIZE.x, 0.0)).xyz * 0.12;
    col += texture(screen_texture, SCREEN_UV + vec2(2.0 * -SCREEN_PIXEL_SIZE.x, 0.0)).xyz * 0.12;
    col += texture(screen_texture, SCREEN_UV + vec2(3.0 * SCREEN_PIXEL_SIZE.x, 0.0)).xyz * 0.09;
    col += texture(screen_texture, SCREEN_UV + vec2(3.0 * -SCREEN_PIXEL_SIZE.x, 0.0)).xyz * 0.09;
    col += texture(screen_texture, SCREEN_UV + vec2(4.0 * SCREEN_PIXEL_SIZE.x, 0.0)).xyz * 0.05;
    col += texture(screen_texture, SCREEN_UV + vec2(4.0 * -SCREEN_PIXEL_SIZE.x, 0.0)).xyz * 0.05;
    COLOR.xyz = col;
}
```

```glsl
shader_type canvas_item;

uniform sampler2D screen_texture : hint_screen_texture, repeat_disable, filter_nearest;

// Blurs the screen in the Y-direction.
void fragment() {
    vec3 col = texture(screen_texture, SCREEN_UV).xyz * 0.16;
    col += texture(screen_texture, SCREEN_UV + vec2(0.0, SCREEN_PIXEL_SIZE.y)).xyz * 0.15;
    col += texture(screen_texture, SCREEN_UV + vec2(0.0, -SCREEN_PIXEL_SIZE.y)).xyz * 0.15;
    col += texture(screen_texture, SCREEN_UV + vec2(0.0, 2.0 * SCREEN_PIXEL_SIZE.y)).xyz * 0.12;
    col += texture(screen_texture, SCREEN_UV + vec2(0.0, 2.0 * -SCREEN_PIXEL_SIZE.y)).xyz * 0.12;
    col += texture(screen_texture, SCREEN_UV + vec2(0.0, 3.0 * SCREEN_PIXEL_SIZE.y)).xyz * 0.09;
    col += texture(screen_texture, SCREEN_UV + vec2(0.0, 3.0 * -SCREEN_PIXEL_SIZE.y)).xyz * 0.09;
    col += texture(screen_texture, SCREEN_UV + vec2(0.0, 4.0 * SCREEN_PIXEL_SIZE.y)).xyz * 0.05;
    col += texture(screen_texture, SCREEN_UV + vec2(0.0, 4.0 * -SCREEN_PIXEL_SIZE.y)).xyz * 0.05;
    COLOR.xyz = col;
}
```

使用上述代码，您应该会得到如下全屏模糊效果。

![../../_images/post_example3.png](https://docs.godotengine.org/en/stable/_images/post_example3.png)

### 用户贡献的笔记

在提交评论之前，请阅读[用户贡献笔记政策](https://github.com/godotengine/godot-docs-user-notes/discussions/1)。

**1 个评论**



[SephReed](https://github.com/SephReed) [Nov 22, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/260#discussioncomment-11344504)

以下是对各种后处理方法及其优缺点的讨论：
[godotengine/godot#99491](https://github.com/godotengine/godot/issues/99491)



## 高级后期处理

https://docs.godotengine.org/en/stable/tutorials/shaders/advanced_postprocessing.html#advanced-post-processing

### 引言

本教程介绍了 Godot 中后期处理的高级方法。特别是，它将解释如何编写使用深度缓冲区的后处理着色器。您应该已经熟悉了后处理，特别是[自定义后处理教程](https://docs.godotengine.org/en/stable/tutorials/shaders/custom_postprocessing.html#doc-custom-postprocessing)中概述的方法。

### 全屏四边形

制作自定义后处理效果的一种方法是使用视口。然而，使用视口有两个主要缺点：

1. 无法访问深度缓冲区
2. 后处理着色器的效果在编辑器中不可见

要绕过使用深度缓冲区的限制，请使用具有 [QuadMesh](https://docs.godotengine.org/en/stable/classes/class_quadmesh.html#class-quadmesh) 基本体（primitive）的 [MeshInstance3D](https://docs.godotengine.org/en/stable/classes/class_meshinstance3d.html#class-meshinstance3d)。这允许我们使用着色器并访问场景的深度纹理。接下来，使用顶点着色器使四边形始终覆盖屏幕，以便始终应用后处理效果，包括在编辑器中。

首先，创建一个新的 MeshInstance3D，并将其网格设置为 QuadMesh。这将创建一个以位置 `(0, 0, 0)` 为中心的四边形，宽度和高度为 `1`。将宽度和高度设置为 `2`，并启用“**翻转面**”。现在，四边形在世界空间的原点占据了一个位置。但是，我们希望它与相机一起移动，以便它始终覆盖整个屏幕。为此，我们将绕过通过差分坐标空间（difference coordinate spaces）平移顶点位置的坐标变换，并将顶点视为已经在剪辑空间（clip space）中。

顶点着色器期望在剪辑空间中输出坐标，这些坐标的范围从屏幕左侧和底部的 `-1` 到屏幕顶部和右侧的 `1`。这就是为什么 QuadMesh 的高度和宽度需要为 `2`。Godot 在幕后处理从模型到视图空间再到剪辑空间的转换，因此我们需要取消 Godot 转换的效果。我们通过将内置的 `POSITION` 设置为所需的位置来实现这一点。`POSITION` 绕过了内置的变换，直接在剪辑空间中设置顶点位置。

```glsl
shader_type spatial;
// Prevent the quad from being affected by lighting and fog. This also improves performance.
render_mode unshaded, fog_disabled;

void vertex() {
  POSITION = vec4(VERTEX.xy, 1.0, 1.0);
}
```

> **注：**
>
> 在早于 4.3 的 Godot 版本中，此代码建议使用 `POSITION = vec4(VERTEX, 1.0);` 这隐含地假设平面附近的剪辑空间为 `0.0`。该代码现在不正确，在 4.3+ 版本中无法工作，因为我们现在使用“反向 z”深度缓冲区，其中近平面为 `1.0`。

即使有了这个顶点着色器，四边形也会不断消失。这是由于在 CPU 上完成的截头体剔除（frustum culling）。截头体剔除使用相机矩阵和网格的 AABB 来确定网格在传递给 GPU 之前是否可见。CPU 不知道我们正在对顶点做什么，因此它假设指定的坐标是指世界位置，而不是剪辑空间位置，这导致当我们远离场景中心时，Godot 会剔除四边形。为了防止四边形被淘汰，有几个选择：

1. 将 QuadMesh 作为子对象添加到摄影机中，使摄影机始终指向它
2. 在 QuadMesh 中将几何属性 `extra_cull_margin` 设置得尽可能大

第二个选项确保四边形在编辑器中可见，而第一个选项则保证即使相机移动到剔除边缘之外，四边形仍然可见。您也可以使用这两个选项。

### 深度纹理

要读取深度纹理，我们首先需要使用 `hint_depth_texture` 为深度缓冲区创建一个纹理 uniform 集。

```glsl
uniform sampler2D depth_texture : source_color, hint_depth_texture;
```

定义后，可以使用 `texture()` 函数读取深度纹理。

```glsl
float depth = texture(depth_texture, SCREEN_UV).x;
```

> **注：**
>
> 与访问屏幕纹理类似，只有从当前视口读取时才能访问深度纹理。无法从已渲染的其他视口访问深度纹理。

`depth_texture` 返回的值在 `1.0` 和 `0.0` 之间（由于使用了“reverse-z”深度缓冲区，分别对应于近平面和远平面），并且是非线性的。当直接从 `depth_texture` 显示深度时，除非由于非线性而非常接近，否则一切看起来几乎都是黑色的。为了使深度值与世界或模型坐标对齐，我们需要线性化该值。当我们将投影矩阵应用于顶点位置时，z 值是非线性的，因此为了使其线性化，我们将其乘以投影矩阵的逆，在 Godot 中，可以通过变量 `INV_PROJECTION_MATRIX` 访问该逆。

首先，获取屏幕空间坐标并将其转换为归一化设备坐标（NDC）。使用 Vulkan 后端时，NDC 在 `x` 和 `y` 方向上运行 `-1.0` 到 `1.0`，在 `z` 方向上运行 `0.0` 到 `1.0`。使用 `SCREEN_UV` 对 `x` 和 `y` 轴以及 `z` 的深度值重建 NDC。

```glsl
void fragment() {
  float depth = texture(depth_texture, SCREEN_UV).x;
  vec3 ndc = vec3(SCREEN_UV * 2.0 - 1.0, depth);
}
```

> **注：**
>
> 本教程假设使用 Forward+ 或 Mobile 渲染器，这两种渲染器都使用 Z 范围为 `[0.0, 1.0]` 的 Vulkan NDC。相比之下，兼容性渲染器使用 Z 范围为 `[-1.0, 1.0]` 的 OpenGL NDC。对于兼容性渲染器，请将 NDC 计算替换为以下内容：
>
> ```glsl
> vec3 ndc = vec3(SCREEN_UV, depth) * 2.0 - 1.0;
> ```

通过将 NDC 乘以 `INV_PROJECTION_MATRIX`，将 NDC 转换为视图空间。回想一下，视图空间给出了相对于相机的位置，因此 `z` 值将给出到该点的距离。

```glsl
void fragment() {
  ...
  vec4 view = INV_PROJECTION_MATRIX * vec4(ndc, 1.0);
  view.xyz /= view.w;
  float linear_depth = -view.z;
}
```

因为相机面向负 `z` 方向，所以位置将具有负 `z` 值。为了获得可用的深度值，我们必须否定 `view.z`。

可以使用以下代码从深度缓冲区构造世界位置，使用 `INV_VIEW_MATRIX` 将位置从视图空间转换为世界空间。

```glsl
void fragment() {
  ...
  vec4 world = INV_VIEW_MATRIX * INV_PROJECTION_MATRIX * vec4(ndc, 1.0);
  vec3 world_position = world.xyz / world.w;
}
```

### 着色器示例

一旦我们添加了一行来输出 `ALBEDO`，我们就有了一个完整的着色器，看起来像这样。此着色器允许您可视化线性深度或世界空间坐标，具体取决于注释掉的行。

```glsl
shader_type spatial;
// Prevent the quad from being affected by lighting and fog. This also improves performance.
render_mode unshaded, fog_disabled;

uniform sampler2D depth_texture : source_color, hint_depth_texture;

void vertex() {
  POSITION = vec4(VERTEX.xy, 1.0, 1.0);
}

void fragment() {
  float depth = texture(depth_texture, SCREEN_UV).x;
  vec3 ndc = vec3(SCREEN_UV * 2.0 - 1.0, depth);
  vec4 view = INV_PROJECTION_MATRIX * vec4(ndc, 1.0);
  view.xyz /= view.w;
  float linear_depth = -view.z;

  vec4 world = INV_VIEW_MATRIX * INV_PROJECTION_MATRIX * vec4(ndc, 1.0);
  vec3 world_position = world.xyz / world.w;

  // Visualize linear depth
  ALBEDO.rgb = vec3(fract(linear_depth));

  // Visualize world coordinates
  //ALBEDO.rgb = fract(world_position).xyz;
}
```

### 优化

您可以从使用单个大三角形而不是使用全屏四边形中受益。[这里](https://michaldrobot.com/2014/04/01/gcn-execution-patterns-in-full-screen-passes)解释了原因。但是，好处很小，只有在运行特别复杂的片段着色器时才有好处。

将 MeshInstance3D 中的网格设置为 [ArrayMesh](https://docs.godotengine.org/en/stable/classes/class_arraymesh.html#class-arraymesh)。ArrayMesh 是一种工具，它允许您轻松地从数组中为顶点、法线、颜色等构建网格。

现在，将脚本附加到 MeshInstance3D 并使用以下代码：

```gdscript
extends MeshInstance3D

func _ready():
  # Create a single triangle out of vertices:
  var verts = PackedVector3Array()
  verts.append(Vector3(-1.0, -1.0, 0.0))
  verts.append(Vector3(-1.0, 3.0, 0.0))
  verts.append(Vector3(3.0, -1.0, 0.0))

  # Create an array of arrays.
  # This could contain normals, colors, UVs, etc.
  var mesh_array = []
  mesh_array.resize(Mesh.ARRAY_MAX) #required size for ArrayMesh Array
  mesh_array[Mesh.ARRAY_VERTEX] = verts #position of vertex array in ArrayMesh Array

  # Create mesh from mesh_array:
  mesh.add_surface_from_arrays(Mesh.PRIMITIVE_TRIANGLES, mesh_array)
```

> **注：**
>
> 三角形在归一化设备坐标中指定。回想一下，NDC 在 `x` 和 `y` 方向上都从 `-1.0` 运行到 `1.0`。这使得屏幕宽 `2` 个单位，高 `2` 个单位。为了用一个三角形覆盖整个屏幕，请使用一个 `4` 个单位宽、`4` 个单位高的三角形，将其高度和宽度加倍。

从上面指定相同的顶点着色器，一切看起来都应该完全相同。

使用 ArrayMesh 而不是使用 QuadMesh 的一个缺点是，ArrayMesh 在编辑器中不可见，因为三角形在场景运行之前不会构建。为了解决这个问题，在建模程序中构造一个三角形网格，并在 MeshInstance3D 中使用它。

### 用户贡献的笔记

在提交评论之前，请阅读[用户贡献笔记政策](https://github.com/godotengine/godot-docs-user-notes/discussions/1)。

**3 个评论** · 4 个回复



[tetrapod00](https://github.com/tetrapod00) [Aug 17, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/42#discussioncomment-10362065)

以下是一个示例，您可以将其用作效果的基础。它读取所有三个屏幕纹理（`screen_texture`、`depth_texture` 和 `normal_rough_texture`）。您想要使用的大多数值都是从屏幕纹理中提取的。通过取消注释 `ALBEDO` 行来更改渲染的值。

```glsl
// Godot 4.3, Forward+ or Mobile
shader_type spatial;
render_mode unshaded, fog_disabled;

uniform sampler2D screen_texture : source_color, hint_screen_texture;
uniform sampler2D depth_texture : hint_depth_texture;
uniform sampler2D normal_rough_texture : hint_normal_roughness_texture;

void vertex() {
	POSITION = vec4(VERTEX.xy, 1.0, 1.0);
}

void fragment() {
	vec4 screen = texture(screen_texture, SCREEN_UV);
	
	float depth_raw = texture(depth_texture, SCREEN_UV).x;
	vec3 ndc = vec3(SCREEN_UV * 2.0 - 1.0, depth_raw);
	vec4 position_view = INV_PROJECTION_MATRIX * vec4(ndc, 1.0);
	position_view.xyz /= position_view.w;
	float linear_depth = -position_view.z;
	
	vec4 world = INV_VIEW_MATRIX * INV_PROJECTION_MATRIX * vec4(ndc, 1.0);
	vec3 position_world = world.xyz / world.w;
	
	vec4 normal_rough = texture(normal_rough_texture, SCREEN_UV);
	vec3 normals_view_raw = normal_rough.xyz; // Normals in view space, in [0.0, 1.0] range
	vec3 normals_view_remapped = normals_view_raw.xyz * 2.0 - 1.0;  // Normals in view space, in [-1.0, 1.0] range
	vec3 normals_world = (INV_VIEW_MATRIX * vec4(normals_view_remapped, 0.0)).xyz;
	float roughness = normal_rough.w;
	
	// Visualize the outputs
	// Screen texture
	ALBEDO.rgb = screen.rgb;
	// Raw depth
	//ALBEDO.rgb = vec3(depth_raw);
	// Linear depth
	//ALBEDO.rgb = vec3(fract(linear_depth));
	// World position
	//ALBEDO.rgb = fract(position_world);	
	// Normals from the normal buffer, in view space
	//ALBEDO.rgb = normals_view_raw;
	// Normals in world space, [-1.0,1.0] range
	//ALBEDO.rgb = normals_world;
	// Roughness
	//ALBEDO.rgb = vec3(roughness);
}
```



[thompsop1sou](https://github.com/thompsop1sou) [Nov 20, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/42#discussioncomment-11318393)

我也在 [Screen Reading Shaders](https://docs.godotengine.org/en/stable/tutorials/shaders/screen-reading_shaders.html) 文档中发布了这条评论，但我认为这里也值得注意。该文档提到了 3D 中后处理和透明对象的一个重大限制：

> 在 3D 中，屏幕是在不透明几何体通过之后但在透明几何体通过之前复制的，因此透明对象不会被捕获在屏幕纹理中。

这不仅会影响屏幕纹理，还会影响深度和正常粗糙度纹理。

有几种方法可以解决这个问题，这样你就可以对透明对象进行后处理效果：

1. 如果只使用屏幕纹理（不是深度或法线-粗糙度），请使用 2D 着色器。当您在 2D 着色器中访问屏幕纹理时，将显示透明对象。（我认为这是因为 3D 渲染已经全部完成。）这不适用于深度或法线-粗糙度纹理，因为这些纹理在 2D 着色器中不可用。
2. 使用[合成器效果](https://docs.godotengine.org/en/stable/tutorials/rendering/compositor.html)。这是一个较低级别的解决方案，需要编写 GLSL 并像在计算着色器中一样传递数据。请注意，您需要将透明对象设置为“始终深度绘制”，以便它们显示在深度缓冲区中。目前，还没有办法让透明物体在法线-粗糙度纹理中显示出来。
3. 您可以使用具有多个相机和视口的设置来手动捕获和传递纹理。设置起来有点复杂，但它最终会给你比合成器效果更多的控制权。您可以访问颜色、深度、法线或粗糙度数据，也可以传递自己的自定义数据。我在这里有一个示例项目：https://github.com/thompsop1sou/custom-screen-buffers

​	[SephReed](https://github.com/SephReed) [Nov 22, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/42#discussioncomment-11343543)

​	我爱你！出色的解决方案，清晰的文档。谢谢您



[heart-rotting](https://github.com/heart-rotting) [Nov 26, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/42#discussioncomment-11379642)

不确定我是否只是速度较慢，但即使我只是将教程着色器复制粘贴到我的文件中，我也会得到一种奇怪的振荡/带状效果（类似于此）。通过反复试验，我发现我必须将线性深度除以大约100或更高的值，才能得到振荡效应，使其推得足够远，不再存在。

```glsl
float linear_depth = -(view.xyz / view.w).z / 100.0;
```

如果有任何着色器专家理解为什么会发生这种情况，我很想听听！谢谢，希望这能帮助到别人！✌️

​	[tetrapod00](https://github.com/tetrapod00) [Nov 26, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/42#discussioncomment-11379703)

​	嗨，我相信带状效应是有意的。此页面上的此示例本身并不是一个有用的后处理效果，而只是为了可视化线性深度和世界空间位置的值（如果您取消注释另一行）。您可以使用这些值来实现自己的后处理效果。

​	通过使用 `fract()`，带状效应每 1 单位（米）的线性深度重复一次。如果移动相机，它会随着相机向前或向后移动而出现振荡。

​	当你像代码示例中那样将该值除以 100 时，你会每 100 个单位（米）重复一次带状效果，因此在大多数游戏场景中，带状效果似乎消失了。

​	[heart-rotting](https://github.com/heart-rotting) [Nov 26, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/42#discussioncomment-11379729)

​	哦，我知道了！这让我明白了很多😅. 感谢您的快速回复！

​	[LordMcMutton](https://github.com/LordMcMutton) [Dec 20, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/42#discussioncomment-11624282)

​	我能够用这个获得更好的深度缓冲：

```glsl
float depth = texture(depth_texture, SCREEN_UV).x;
float linear_depth = mix(-10, 10, depth);
```

​	您可以使用页面提供的ALBEDO位看到它的样子：

```glsl
ALBEDO = vec3(fract(linear_depth));
```

​	它给你一个从黑到白的平滑缓冲，就像你所期望的那样。



## 树木的制作

https://docs.godotengine.org/en/stable/tutorials/shaders/making_trees.html#making-trees

这是一个关于如何从头开始制作树木和其他类型植被的简短教程。

我们的目的不是关注建模技术（有很多关于建模技术的教程），而是如何让它们在 Godot 中看起来很好。

![../../_images/tree_sway.gif](https://docs.godotengine.org/en/stable/_images/tree_sway.gif)

### 从树开始

我从 SketchFab 中选取了这棵树：

![../../_images/tree_base.png](https://docs.godotengine.org/en/stable/_images/tree_base.png)

https://sketchfab.com/models/ea5e6ed7f9d6445ba69589d503e8cebf

并在 Blender 中打开它。

### 用顶点颜色绘制

您可能要做的第一件事是使用顶点颜色来绘制有风时树会摆动多少。只需使用您最喜欢的 3D 建模程序的顶点颜色绘制工具，绘制如下内容：

![../../_images/tree_vertex_paint.png](https://docs.godotengine.org/en/stable/_images/tree_vertex_paint.png)

这有点夸张，但这个想法是，颜色表明摇摆对树的每个部分有多大影响。这里的比例更能代表它：

![../../_images/tree_gradient.png](https://docs.godotengine.org/en/stable/_images/tree_gradient.png)

### 为叶子编写自定义着色器

这是一个叶子着色器的示例：

```glsl
shader_type spatial;
render_mode depth_prepass_alpha, cull_disabled, world_vertex_coords;
```

这是一个空间着色器。没有前/后剔除（因此可以从两侧看到树叶），并且使用了阿尔法预处理，因此使用透明度（树叶投射阴影）产生的深度伪影更少。最后，对于摇摆效果，建议使用世界坐标，这样树就可以被复制、移动等，并且它仍然可以与其他树一起工作。

```glsl
uniform sampler2D texture_albedo : source_color;
uniform vec4 transmission : source_color;
```

在这里，读取纹理和透射颜色，透射颜色用于为叶子添加一些背光，模拟次表面散射。

```glsl
uniform float sway_speed = 1.0;
uniform float sway_strength = 0.05;
uniform float sway_phase_len = 8.0;

void vertex() {
    float strength = COLOR.r * sway_strength;
    VERTEX.x += sin(VERTEX.x * sway_phase_len * 1.123 + TIME * sway_speed) * strength;
    VERTEX.y += sin(VERTEX.y * sway_phase_len + TIME * sway_speed * 1.12412) * strength;
    VERTEX.z += sin(VERTEX.z * sway_phase_len * 0.9123 + TIME * sway_speed * 1.3123) * strength;
}
```

这是创建树叶摇摆的代码。这是基本的（只使用正弦波乘以时间和轴位置，但效果很好）。请注意，强度与颜色相乘。每个轴都使用一个不同的接近 1.0 的小乘法因子，因此轴不会同步出现。

最后，剩下的就是片段着色器：

```glsl
void fragment() {
    vec4 albedo_tex = texture(texture_albedo, UV);
    ALBEDO = albedo_tex.rgb;
    ALPHA = albedo_tex.a;
    METALLIC = 0.0;
    ROUGHNESS = 1.0;
    TRANSMISSION = transmission.rgb;
}
```

这几乎就是全部。

主干着色器是类似的，除了它不写入阿尔法通道（因此不需要阿尔法预处理），也不需要传输才能工作。这两个着色器都可以通过添加法线贴图、AO 和其他贴图来改进。

### 改进着色器

关于如何做到这一点，你可以阅读更多的资源。既然您已经了解了基础知识，建议您阅读 GPU Gems3 中关于 Crysis 如何做到这一点的章节（主要关注 sway 代码，因为其中显示的许多其他技术已经过时）：

https://developer.nvidia.com/gpugems/GPUGems3/gpugems3_ch16.html

### 用户贡献的笔记

在提交评论之前，请阅读[用户贡献笔记政策](https://github.com/godotengine/godot-docs-user-notes/discussions/1)。

**1 个评论**



[heinermann](https://github.com/heinermann) [Oct 7, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/185#discussioncomment-10866469)

对于任何使用 TreeIt（它会自动生成上述顶点颜色）创建树的人来说，有人在以下位置创建了一个 Godot 就绪着色器 https://godotshaders.com/shader/treeit-tree-shader/