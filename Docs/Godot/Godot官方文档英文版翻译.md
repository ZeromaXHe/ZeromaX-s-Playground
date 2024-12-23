# 插件

## 安装插件

https://docs.godotengine.org/en/stable/tutorials/plugins/editor/installing_plugins.html#installing-plugins

Godot 具有一个编辑器插件系统，其中包含社区开发的众多插件。插件可以通过新节点、附加停靠点、便利功能等扩展编辑器的功能。

### 查找插件

查找 Godot 插件的首选方法是使用[资源库](https://godotengine.org/asset-library/)。虽然可以在线浏览，但直接从编辑器中使用它更方便。为此，请单击编辑器顶部的 **AssetLib** 选项卡：

![../../../_images/installing_plugins_assetlib_tab.webp](https://docs.godotengine.org/en/stable/_images/installing_plugins_assetlib_tab.webp)

您还可以在 GitHub 等代码托管网站上找到资产。

> **注：**
>
> 一些存储库将自己描述为“插件”，但实际上可能不是编辑器插件。对于打算在运行中的项目中使用的脚本来说尤其如此。您不需要启用这些插件来使用它们。下载它们并解压缩项目文件夹中的文件。
>
> 区分编辑器插件和非编辑器插件的一种方法是在托管插件的存储库中查找 `plugin.cfg` 文件。如果存储库在 `addons/` 文件夹中的文件夹中包含 `plugin.cfg` 文件，则它是一个编辑器插件。

### 安装插件

要安装插件，请将其作为 ZIP 存档下载。在资产库上，可以使用下载按钮从编辑器或使用 Web 界面完成此操作。

在 GitHub 上，如果插件声明了*标签*（版本），请转到“**发布**”选项卡下载稳定版本。这可确保您下载的版本由其作者声明为稳定版本。

在 GitHub 上，如果插件没有声明任何标签，请使用**下载 ZIP** 按钮下载最新版本的 ZIP：

![../../../_images/installing_plugins_github_download_zip.png](https://docs.godotengine.org/en/stable/_images/installing_plugins_github_download_zip.png)

解压缩 ZIP 存档，并将其中包含的 `addons/` 文件夹移动到您的项目文件夹中。如果您的项目已经包含 `addons/` 文件夹，请将插件的 `addons/` 文件夹移动到您的项目文件夹中，以将新文件夹内容与现有文件夹内容合并。您的文件管理员可能会询问您是否要写入该文件夹；回答**是**。在此过程中不会覆盖任何文件。

### 启用插件

要启用新安装的插件，请打开编辑器顶部的**项目 > 项目设置**，然后转到**插件**选项卡。如果插件打包正确，您应该在插件列表中看到它。单击**启用**复选框启用插件。

![../../../_images/installing_plugins_project_settings.webp](https://docs.godotengine.org/en/stable/_images/installing_plugins_project_settings.webp)

启用插件后，您可以立即使用它；不需要重新启动编辑器。同样，禁用插件也可以在不重新启动编辑器的情况下完成。



## 制作插件

https://docs.godotengine.org/en/stable/tutorials/plugins/editor/making_plugins.html#making-plugins

### 关于插件

插件是用有用的工具扩展编辑器的好方法。它可以完全使用 GDScript 和标准场景制作，甚至无需重新加载编辑器。与模块不同，您不需要创建 C++ 代码，也不需要重新编译引擎。虽然这会降低插件的功能，但你仍然可以用它们做很多事情。请注意，插件类似于您已经可以制作的任何场景，除了它是使用脚本添加编辑器功能创建的。

本教程将指导您创建两个插件，以便您了解它们的工作原理，并能够开发自己的插件。第一个是可以添加到项目中任何场景的自定义节点，另一个是添加到编辑器中的自定义停靠点。

### 创建插件

在开始之前，无论你想在哪里创建一个新的空项目。这将作为开发和测试插件的基础。

编辑器识别新插件需要做的第一件事是创建两个文件：一个用于配置的 `plugin.cfg` 和一个具有该功能的工具脚本。插件在项目文件夹中有一个标准路径，如 `addons/plugin_name`。Godot 提供了一个对话框，用于生成这些文件并将其放置在需要的位置。

在主工具栏中，单击“`项目`”下拉列表。然后单击“`项目设置…`”。转到 `插件` 选项卡，然后单击右上角的`创建新插件`按钮。

您将看到对话框出现，如下所示：

![../../../_images/making_plugins-create_plugin_dialog.webp](https://docs.godotengine.org/en/stable/_images/making_plugins-create_plugin_dialog.webp)

每个字段中的占位符文本描述了它如何影响插件的文件创建和配置文件的值。

要继续示例，请使用以下值：

```gdscript
Plugin Name: My Custom Node
Subfolder: my_custom_node
Description: A custom node made to extend the Godot Engine.
Author: Your Name Here
Version: 1.0.0
Language: GDScript
Script Name: custom_node.gd
Activate now: No
```

```c#
Plugin Name: My Custom Node
Subfolder: MyCustomNode
Description: A custom node made to extend the Godot Engine.
Author: Your Name Here
Version: 1.0.0
Language: C#
Script Name: CustomNode.cs
Activate now: No
```

> **警告：**
>
> 取消选中“`立即激活？`” C# 中的选项始终是必需的，因为与其他 C# 脚本一样，需要编译 EditorPlugin 脚本，这需要构建项目。构建项目后，可以在 `项目设置` 的`插件`选项卡中启用插件。

你应该得到这样的目录结构：

![../../../_images/making_plugins-my_custom_mode_folder.webp](https://docs.godotengine.org/en/stable/_images/making_plugins-my_custom_mode_folder.webp)

`plugin.cfg` 是一个 INI 文件，其中包含有关插件的元数据。名称和描述有助于人们理解它的功能。你的名字有助于你的工作得到适当的认可。版本号有助于其他人知道他们是否有过时的版本；如果您不确定如何计算版本号，请查看[语义版本控制](https://semver.org/)。主脚本文件将指示Godot，一旦插件处于活动状态，它将在编辑器中执行什么操作。

#### 脚本文件

创建插件后，对话框将自动为您打开 EditorPlugin 脚本。该脚本有两个不能更改的要求：它必须是 `@tool` 脚本，否则将无法在编辑器中正确加载，并且必须从 [EditorPlugin](https://docs.godotengine.org/en/stable/classes/class_editorplugin.html#class-editorplugin) 继承。

> **警告：**
>
> 除了 EditorPlugin 脚本外，您的插件使用的任何其他 GDScript *也*必须是一个工具。编辑器使用的任何没有 `@tool` 的 GDScript 都将像一个空文件一样！

处理资源的初始化和清理非常重要。一个好的做法是使用虚拟函数 [_enter_tree()](https://docs.godotengine.org/en/stable/classes/class_node.html#class-node-private-method-enter-tree) 来初始化你的插件，并使用 [_exit_tree()](https://docs.godotengine.org/en/stable/classes/class_node.html#class-node-private-method-exit-tree) 来清理它。幸运的是，对话框为您生成了这些回调。你的脚本应该看起来像这样：

```gdscript
@tool
extends EditorPlugin


func _enter_tree():
	# Initialization of the plugin goes here.
	pass


func _exit_tree():
	# Clean-up of the plugin goes here.
	pass
```

```c#
#if TOOLS
using Godot;

[Tool]
public partial class CustomNode : EditorPlugin
{
    public override void _EnterTree()
    {
        // Initialization of the plugin goes here.
    }

    public override void _ExitTree()
    {
        // Clean-up of the plugin goes here.
    }
}
#endif
```

这是创建新插件时使用的好模板。

### 自定义节点

有时，您希望在许多节点中具有某种行为，例如可以重用的自定义场景或控件。实例化在很多情况下都很有用，但有时它可能很麻烦，特别是如果你在许多项目中使用它。一个好的解决方案是制作一个插件，添加一个具有自定义行为的节点。

> **警告：**
>
> 通过 EditorPlugin 添加的节点是“CustomType”节点。虽然它们可以使用任何脚本语言，但它们的功能比[脚本类系统](https://docs.godotengine.org/en/stable/tutorials/scripting/gdscript/gdscript_basics.html#doc-gdscript-basics-class-name)少。如果您正在编写 GDScript 或 NativeScript，我们建议您改用脚本类。

要创建新的节点类型，可以使用 [EditorPlugin](https://docs.godotengine.org/en/stable/classes/class_editorplugin.html#class-editorplugin) 类中的函数 [add_custom_type()](https://docs.godotengine.org/en/stable/classes/class_editorplugin.html#class-editorplugin-method-add-custom-type)。此函数可以向编辑器添加新类型（节点或资源）。但是，在创建类型之前，您需要一个充当类型逻辑的脚本。虽然该脚本不必使用 `@tool` 注释，但可以添加它，以便脚本在编辑器中运行。

在本教程中，我们将创建一个按钮，在单击时打印一条消息。为此，我们需要一个从 [Button](https://docs.godotengine.org/en/stable/classes/class_button.html#class-button) 扩展的脚本。如果你愿意，它也可以扩展 [BaseButton](https://docs.godotengine.org/en/stable/classes/class_basebutton.html#class-basebutton)：

```gdscript
@tool
extends Button


func _enter_tree():
	pressed.connect(clicked)


func clicked():
	print("You clicked me!")
```

```c#
using Godot;

[Tool]
public partial class MyButton : Button
{
    public override void _EnterTree()
    {
        Pressed += Clicked;
    }

    public void Clicked()
    {
        GD.Print("You clicked me!");
    }
}
```

这就是我们的基本按钮。您可以将其另存为插件文件夹中的 `my_button.gd`。您还需要一个 16×16 的图标才能在场景树中显示。如果没有，可以从引擎中获取默认徽标，并将其保存在 *addons/my_custom_node* 文件夹中作为 *icon.png*，或者使用默认的 Godot 徽标（*preload("res://icon.svg")*).

> **小贴士：**
>
> 用作自定义节点图标的 SVG 图像应启用**“编辑器”>“随编辑器缩放”**和**“编辑器>随编辑器主题转换颜色”**导入选项。如果图标与 Godot 自己的图标使用相同的调色板设计，这允许图标遵循编辑器的比例和主题设置。

![../../../_images/making_plugins-custom_node_icon.png](https://docs.godotengine.org/en/stable/_images/making_plugins-custom_node_icon.png)

现在，我们需要将其添加为自定义类型，以便在“**创建新节点**”对话框中显示。为此，将 `custom_node.gd` 脚本更改为以下内容：

```gdscript
@tool
extends EditorPlugin


func _enter_tree():
	# Initialization of the plugin goes here.
	# Add the new type with a name, a parent type, a script and an icon.
	add_custom_type("MyButton", "Button", preload("my_button.gd"), preload("icon.png"))


func _exit_tree():
	# Clean-up of the plugin goes here.
	# Always remember to remove it from the engine when deactivated.
	remove_custom_type("MyButton")
```

```c#
#if TOOLS
using Godot;

[Tool]
public partial class CustomNode : EditorPlugin
{
    public override void _EnterTree()
    {
        // Initialization of the plugin goes here.
        // Add the new type with a name, a parent type, a script and an icon.
        var script = GD.Load<Script>("res://addons/MyCustomNode/MyButton.cs");
        var texture = GD.Load<Texture2D>("res://addons/MyCustomNode/Icon.png");
        AddCustomType("MyButton", "Button", script, texture);
    }

    public override void _ExitTree()
    {
        // Clean-up of the plugin goes here.
        // Always remember to remove it from the engine when deactivated.
        RemoveCustomType("MyButton");
    }
}
#endif
```

完成此操作后，该插件应该已经在**项目设置**中的插件列表中可用，因此请按照[检查结果](https://docs.godotengine.org/en/stable/tutorials/plugins/editor/making_plugins.html#checking-the-results)中的说明激活它。

然后通过添加新节点来尝试：

![../../../_images/making_plugins-custom_node_create.webp](https://docs.godotengine.org/en/stable/_images/making_plugins-custom_node_create.webp)

添加节点时，您可以看到它已经附加了您创建的脚本。为按钮设置文本，保存并运行场景。当您单击按钮时，您可以在控制台中看到一些文本：

![../../../_images/making_plugins-custom_node_console.webp](https://docs.godotengine.org/en/stable/_images/making_plugins-custom_node_console.webp)

#### 定制停靠栏

有时，您需要扩展编辑器并添加始终可用的工具。一个简单的方法是添加一个带有插件的新 dock。Docks 只是基于 Control 的场景，因此它们的创建方式类似于通常的 GUI 场景。

创建自定义停靠点就像创建自定义节点一样。在 `addons/my_custom_dock` 文件夹中创建一个新的 `plugin.cfg` 文件，然后向其中添加以下内容：

```gdscript
[plugin]

name="My Custom Dock"
description="A custom dock made so I can learn how to make plugins."
author="Your Name Here"
version="1.0"
script="custom_dock.gd"
```

```c#
[plugin]

name="My Custom Dock"
description="A custom dock made so I can learn how to make plugins."
author="Your Name Here"
version="1.0"
script="CustomDock.cs"
```

然后在同一文件夹中创建脚本 `custom_dock.gd`。用我们[之前看到的模板](https://docs.godotengine.org/en/stable/tutorials/plugins/editor/making_plugins.html#doc-making-plugins-template-code)填充它，以获得一个良好的开端。

由于我们正试图添加一个新的自定义停靠站，因此需要创建停靠站的内容。这只不过是一个标准的Godot场景：只需在编辑器中创建一个新场景，然后对其进行编辑。

对于编辑器停靠，根节点**必须**是 [Control](https://docs.godotengine.org/en/stable/classes/class_control.html#class-control) 或其子类之一。在本教程中，您可以创建一个按钮。根节点的名称也将是dock选项卡上显示的名称，因此请确保为其提供一个简短的描述性名称。此外，别忘了在按钮上添加一些文本。

![../../../_images/making_plugins-my_custom_dock_scene.webp](https://docs.godotengine.org/en/stable/_images/making_plugins-my_custom_dock_scene.webp)

将此场景另存为 `my_dock.tscn`。现在，我们需要抓取我们创建的场景，然后将其作为dock添加到编辑器中。为此，您可以依赖 [EditorPlugin](https://docs.godotengine.org/en/stable/classes/class_editorplugin.html#class-editorplugin) 类中的函数 [add_control_to_dock()](https://docs.godotengine.org/en/stable/classes/class_editorplugin.html#class-editorplugin-method-add-control-to-dock)。

您需要选择一个停靠位置并定义要添加的控件（即您刚才创建的场景）。当插件被停用时，不要忘记**移除 dock**。脚本可能看起来像这样：

```gdscript
@tool
extends EditorPlugin


# A class member to hold the dock during the plugin life cycle.
var dock


func _enter_tree():
	# Initialization of the plugin goes here.
	# Load the dock scene and instantiate it.
	dock = preload("res://addons/my_custom_dock/my_dock.tscn").instantiate()

	# Add the loaded scene to the docks.
	add_control_to_dock(DOCK_SLOT_LEFT_UL, dock)
	# Note that LEFT_UL means the left of the editor, upper-left dock.


func _exit_tree():
	# Clean-up of the plugin goes here.
	# Remove the dock.
	remove_control_from_docks(dock)
	# Erase the control from the memory.
	dock.free()
```

```c#
#if TOOLS
using Godot;

[Tool]
public partial class CustomDock : EditorPlugin
{
    private Control _dock;

    public override void _EnterTree()
    {
        _dock = GD.Load<PackedScene>("res://addons/MyCustomDock/MyDock.tscn").Instantiate<Control>();
        AddControlToDock(DockSlot.LeftUl, _dock);
    }

    public override void _ExitTree()
    {
        // Clean-up of the plugin goes here.
        // Remove the dock.
        RemoveControlFromDocks(_dock);
        // Erase the control from the memory.
        _dock.Free();
    }
}
#endif
```

请注意，虽然停靠站最初会出现在指定位置，但用户可以自由更改其位置并保存最终布局。

#### 检查结果

现在是时候检查你的工作结果了。打开**项目设置**，单击**插件**选项卡。你的插件应该是列表中唯一的一个。

![../../../_images/making_plugins-project_settings.webp](https://docs.godotengine.org/en/stable/_images/making_plugins-project_settings.webp)

您可以看到该插件未启用。单击**启用**复选框以激活插件。在您关闭设置窗口之前，停靠栏应该已经可见。您现在应该有一个自定义停靠栏：

![../../../_images/making_plugins-custom_dock.webp](https://docs.godotengine.org/en/stable/_images/making_plugins-custom_dock.webp)

### 超越

现在您已经学习了如何制作基本插件，您可以通过多种方式扩展编辑器。GDScript 可以为编辑器添加许多功能；这是一种创建专用编辑器的强大方法，而无需深入研究 C++ 模块。

你可以制作自己的插件来帮助自己，并在[资产库](https://godotengine.org/asset-library/)中共享它们，这样人们就可以从你的工作中受益。

### 在插件中注册自动加载/单例加载

启用插件后，编辑器插件可以自动注册[自动加载](https://docs.godotengine.org/en/stable/tutorials/scripting/singletons_autoload.html#doc-singletons-autoload)。这也包括在插件被禁用时注销自动加载。

这使得用户设置插件更快，因为如果你的编辑器插件需要使用自动加载，他们就不必再手动将自动加载添加到项目设置中。

使用以下代码从编辑器插件注册单例：

```gdscript
@tool
extends EditorPlugin

# Replace this value with a PascalCase autoload name, as per the GDScript style guide.
const AUTOLOAD_NAME = "SomeAutoload"


func _enable_plugin():
	# The autoload can be a scene or script file.
	add_autoload_singleton(AUTOLOAD_NAME, "res://addons/my_addon/some_autoload.tscn")


func _disable_plugin():
	remove_autoload_singleton(AUTOLOAD_NAME)
```

```c#
#if TOOLS
using Godot;

[Tool]
public partial class MyEditorPlugin : EditorPlugin
{
    // Replace this value with a PascalCase autoload name.
    private const string AutoloadName = "SomeAutoload";

    public override void _EnablePlugin()
    {
        // The autoload can be a scene or script file.
        AddAutoloadSingleton(AutoloadName, "res://addons/MyAddon/SomeAutoload.tscn");
    }

    public override void _DisablePlugin()
    {
        RemoveAutoloadSingleton(AutoloadName);
    }
}
#endif
```

### 用户贡献的笔记

在提交评论之前，请阅读[用户贡献笔记政策](https://github.com/godotengine/godot-docs-user-notes/discussions/1)。

**1 个评论** · 1 个回复



[Rivers47](https://github.com/Rivers47) [Sep 13, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/137#discussioncomment-10629857)

为什么 C# 版本的 MyButton 没有附带 #if TOOLS #endif？

​	[paulloz](https://github.com/paulloz) [Sep 16, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/137#discussioncomment-10654243) Member

​	你好。如果该类被 `#If TOOLS` 包装，则导出游戏时不会编译节点类型。你可以在[这里](https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/c_sharp_features.html#full-list-of-defines)找到更多信息



## 制作主屏幕插件

https://docs.godotengine.org/en/stable/tutorials/plugins/editor/making_main_screen_plugins.html#making-main-screen-plugins

### 本教程涵盖的内容

主屏幕插件允许您在编辑器的中心部分创建新的 UI，这些UI显示在“2D”、“3D”、“脚本”和“AssetLib”按钮旁边。此类编辑器插件称为“主屏幕插件”。

本教程将引导您创建一个基本的主屏幕插件。为了简单起见，我们的主屏幕插件将包含一个将文本打印到控制台的按钮。

### 初始化插件

首先从插件菜单创建一个新插件。在本教程中，我们将把它放在一个名为 `main_screen` 的文件夹中，但您可以使用任何您喜欢的名称。

插件脚本将附带 `_enter_tree()` 和 `_exit_tree()` 方法，但对于主屏幕插件，我们需要添加一些额外的方法。添加五个额外的方法，使脚本看起来像这样：

```gdscript
@tool
extends EditorPlugin


func _enter_tree():
	pass


func _exit_tree():
	pass


func _has_main_screen():
	return true


func _make_visible(visible):
	pass


func _get_plugin_name():
	return "Main Screen Plugin"


func _get_plugin_icon():
	return EditorInterface.get_editor_theme().get_icon("Node", "EditorIcons")
```

```c#
#if TOOLS
using Godot;

[Tool]
public partial class MainScreenPlugin : EditorPlugin
{
    public override void _EnterTree()
    {

    }

    public override void _ExitTree()
    {

    }

    public override bool _HasMainScreen()
    {
        return true;
    }

    public override void _MakeVisible(bool visible)
    {

    }

    public override string _GetPluginName()
    {
        return "Main Screen Plugin";
    }

    public override Texture2D _GetPluginIcon()
    {
        return EditorInterface.GetEditorTheme().GetIcon("Node", "EditorIcons");
    }
}
#endif
```

此脚本中的重要部分是 `_has_main_screen()` 函数，该函数被重载，因此返回 `true`。此函数在插件激活时由编辑器自动调用，告诉它此插件向编辑器添加了一个新的中心视图。现在，我们将保持这个脚本不变，稍后再回来讨论。

### 主屏幕场景

使用从 `Control` 派生的根节点创建一个新场景（对于这个示例插件，我们将把根节点设置为 `CenterContainer`）。选择此根节点，在视口中，单击“`布局`”菜单并选择“`完全矩形`”。您还需要在检查器中启用“`展开`垂直尺寸”标志。该面板现在使用主视口中的所有可用空间。

接下来，让我们在示例主屏幕插件中添加一个按钮。添加一个 `Button` 节点，并将文本设置为“Print Hello”或类似。向按钮添加一个脚本，如下所示：

```gdscript
@tool
extends Button


func _on_print_hello_pressed():
	print("Hello from the main screen plugin!")
```

```c#
using Godot;

[Tool]
public partial class PrintHello : Button
{
    private void OnPrintHelloPressed()
    {
        GD.Print("Hello from the main screen plugin!");
    }
}
```

然后将“按下”信号连接到自身。如果您需要信号方面的帮助，请参阅[使用信号](https://docs.godotengine.org/en/stable/getting_started/step_by_step/signals.html#doc-signals)文章。

我们已经完成了主屏幕面板。将场景另存为 `main_panel.tscn`。

### 更新插件脚本

我们需要更新 `main_screen_plugin.gd` 脚本，以便插件实例化我们的主面板场景并将其放置在需要的位置。以下是完整的插件脚本：

```gdscript
@tool
extends EditorPlugin


const MainPanel = preload("res://addons/main_screen/main_panel.tscn")

var main_panel_instance


func _enter_tree():
	main_panel_instance = MainPanel.instantiate()
	# Add the main panel to the editor's main viewport.
	EditorInterface.get_editor_main_screen().add_child(main_panel_instance)
	# Hide the main panel. Very much required.
	_make_visible(false)


func _exit_tree():
	if main_panel_instance:
		main_panel_instance.queue_free()


func _has_main_screen():
	return true


func _make_visible(visible):
	if main_panel_instance:
		main_panel_instance.visible = visible


func _get_plugin_name():
	return "Main Screen Plugin"


func _get_plugin_icon():
	# Must return some kind of Texture for the icon.
	return EditorInterface.get_editor_theme().get_icon("Node", "EditorIcons")
```

```c#
#if TOOLS
using Godot;

[Tool]
public partial class MainScreenPlugin : EditorPlugin
{
    PackedScene MainPanel = ResourceLoader.Load<PackedScene>("res://addons/main_screen/main_panel.tscn");
    Control MainPanelInstance;

    public override void _EnterTree()
    {
        MainPanelInstance = (Control)MainPanel.Instantiate();
        // Add the main panel to the editor's main viewport.
        EditorInterface.GetEditorMainScreen().AddChild(MainPanelInstance);
        // Hide the main panel. Very much required.
        _MakeVisible(false);
    }

    public override void _ExitTree()
    {
        if (MainPanelInstance != null)
        {
            MainPanelInstance.QueueFree();
        }
    }

    public override bool _HasMainScreen()
    {
        return true;
    }

    public override void _MakeVisible(bool visible)
    {
        if (MainPanelInstance != null)
        {
            MainPanelInstance.Visible = visible;
        }
    }

    public override string _GetPluginName()
    {
        return "Main Screen Plugin";
    }

    public override Texture2D _GetPluginIcon()
    {
        // Must return some kind of Texture for the icon.
        return EditorInterface.GetEditorTheme().GetIcon("Node", "EditorIcons");
    }
}
#endif
```

增加了几行具体内容。`MainPanel` 是一个常量，它包含对场景的引用，我们将其实例化到 *main_panel_instance* 中。

`_enter_tree()` 函数在 `_ready()` 之前被调用。这是我们实例化主面板场景的地方，并将它们添加为编辑器特定部分的子级。我们使用 `EditorInterface.get_editor_main_screen()` 来获取主编辑器屏幕，并将我们的主面板实例作为子面板添加到其中。我们调用 `_make_visible(false)` 函数来隐藏主面板，这样它在首次激活插件时就不会争用空间。

当插件被停用时，会调用 `_exit_tree()` 函数。如果主屏幕仍然存在，我们调用 `queue_free()` 来释放实例并将其从内存中删除。

`_make_visible()` 函数被重写，以根据需要隐藏或显示主面板。当用户单击编辑器顶部的主视口按钮时，编辑器会自动调用此函数。

`_get_plugin_name()` 和 `_get_plugin_icon()` 函数控制插件主视口按钮的显示名称和图标。

您可以添加的另一个函数是 `handles()` 函数，它允许您处理节点类型，在选择类型时自动聚焦主屏幕。这类似于单击 3D 节点将自动切换到 3D 视口。

### 尝试插件

在项目设置中激活插件。您将在主视口上方的 2D、3D、脚本旁边看到一个新按钮。点击它将带你进入新的主屏幕插件，中间的按钮将打印文本。

如果你想尝试这个插件的完整版本，请查看这里的插件演示：https://github.com/godotengine/godot-demo-projects/tree/master/plugins

如果您想查看主屏幕插件功能的更完整示例，请查看此处的 2.5D 演示项目：https://github.com/godotengine/godot-demo-projects/tree/master/misc/2.5d



## 导入插件

https://docs.godotengine.org/en/stable/tutorials/plugins/editor/import_plugins.html#import-plugins

> **注：**
>
> 本教程假设您已经知道如何制作通用插件。如有疑问，请参阅[制作插件](https://docs.godotengine.org/en/stable/tutorials/plugins/editor/making_plugins.html#doc-making-plugins)页面。这也假设您熟悉 Godot 的导入系统。

### 引言

导入插件是一种特殊类型的编辑器工具，它允许 Godot 导入自定义资源并将其视为一级资源。编辑器本身附带了许多导入插件，用于处理 PNG 图像、Collada 和 glTF 模型、Ogg Vorbis 声音等常见资源。

本教程展示了如何创建导入插件，以将自定义文本文件作为素材资源加载。此文本文件将包含三个用逗号分隔的数值，表示一种颜色的三个通道，生成的颜色将用作导入材质的反照率（主颜色）。在这个例子中，它包含纯蓝色（零红色、零绿色和全蓝色）：

```
0,0,255
```

### 配置

首先，我们需要一个通用插件来处理导入插件的初始化和销毁。让我们先添加 `plugin.cfg` 文件：

```properties
[plugin]

name="Silly Material Importer"
description="Imports a 3D Material from an external text file."
author="Yours Truly"
version="1.0"
script="material_import.gd"
```

然后，我们需要 `material_inmport.gd` 文件在需要时添加和删除导入插件：

```gdscript
# material_import.gd
@tool
extends EditorPlugin


var import_plugin


func _enter_tree():
	import_plugin = preload("import_plugin.gd").new()
	add_import_plugin(import_plugin)


func _exit_tree():
	remove_import_plugin(import_plugin)
	import_plugin = null
```

激活此插件后，它将创建导入插件的新实例（我们很快就会创建），并使用 [add_import_plugin()](https://docs.godotengine.org/en/stable/classes/class_editorplugin.html#class-editorplugin-method-add-import-plugin) 方法将其添加到编辑器中。我们将对它的引用存储在类成员 `import_plugin` 中，以便以后删除它时可以引用它。当插件被停用时，会调用 [remove_import_plugin()](https://docs.godotengine.org/en/stable/classes/class_editorplugin.html#class-editorplugin-method-remove-import-plugin) 方法来清理内存，并让编辑器知道导入插件不再可用。

请注意，导入插件是一个引用类型，因此不需要使用 `free()` 函数从内存中显式释放它。当它超出范围时，发动机会自动释放。

### EditorImportPlugin 类

该节目的主角是 [EditorImportPlugin 类](https://docs.godotengine.org/en/stable/classes/class_editorimportplugin.html#class-editorimportplugin)。它负责实现 Godot 在需要知道如何处理文件时调用的方法。

让我们开始编写我们的插件，一次一个方法：

```gdscript
# import_plugin.gd
@tool
extends EditorImportPlugin


func _get_importer_name():
	return "demos.sillymaterial"
```

第一种方法是 [_get_inmporter_name()](https://docs.godotengine.org/en/stable/classes/class_editorimportplugin.html#class-editorimportplugin-private-method-get-importer-name)。这是您的插件的唯一名称，Godot 使用它来知道在某个文件中使用了哪个导入。当需要重新导入文件时，编辑器将知道要调用哪个插件。

```gdscript
func _get_visible_name():
	return "Silly Material"
```

[_get_visible_name()](https://docs.godotengine.org/en/stable/classes/class_editorimportplugin.html#class-editorimportplugin-private-method-get-visible-name)方法负责返回它导入的类型的名称，并将在 Import dock 中显示给用户。

您应该选择此名称作为“导入为”的延续，例如“*导入为愚蠢材料*”。你可以随心所欲地命名它，但我们建议为你的插件取一个描述性的名称。

```gdscript
func _get_recognized_extensions():
	return ["mtxt"]
```

Godot 的导入系统通过扩展名检测文件类型。在 [_get_recognized_extensions()](https://docs.godotengine.org/en/stable/classes/class_editorimportplugin.html#class-editorimportplugin-private-method-get-recognized-extensions) 方法中，您返回一个字符串数组来表示此插件可以理解的每个扩展。如果一个扩展名被多个插件识别，用户可以在导入文件时选择使用哪个插件。

> **小贴士：**
>
> 许多插件可能会使用 `.json` 和 `.txt` 等常见扩展名。此外，项目中可能有一些文件只是游戏的数据，不应该导入。在导入以验证数据时必须小心。永远不要指望文件格式良好。

```gdscript
func _get_save_extension():
	return "material"
```

导入的文件保存在项目根目录下的 `.import` 文件夹中。它们的扩展应该与您要导入的资源类型相匹配，但由于 Godot 无法告诉您将使用什么（因为同一资源可能有多个有效的扩展），您需要声明将在导入中使用什么。

由于我们正在导入一个材质，因此我们将使用此类资源类型的特殊扩展名。如果要导入场景，可以使用 `scn`。通用资源可以使用 `res` 扩展名。然而，引擎并没有以任何方式强制执行。

```gdscript
func _get_resource_type():
	return "StandardMaterial3D"
```

导入的资源具有特定类型，因此编辑器可以知道它属于哪个属性槽。这允许从 FileSystem dock 拖放到 Inspector 中的属性。

在我们的例子中，它是一个 [StandardMaterial3D](https://docs.godotengine.org/en/stable/classes/class_standardmaterial3d.html#class-standardmaterial3d)，可以应用于 3D 对象。

> **注：**
>
> 如果你需要从同一个扩展中导入不同的类型，你必须创建多个导入插件。您可以在另一个文件上抽象导入代码，以避免这方面的重复。

### 选项和预设

您的插件可以提供不同的选项，允许用户控制如何导入资源。如果一组选定的选项是通用的，您还可以创建不同的预设，以方便用户使用。下图显示了选项在编辑器中的显示方式：

![../../../_images/import_plugin_options.png](https://docs.godotengine.org/en/stable/_images/import_plugin_options.png)

由于可能有许多预设，并且它们是用数字标识的，因此使用枚举是一种很好的做法，这样你就可以使用名称来引用它们。

```gdscript
@tool
extends EditorImportPlugin


enum Presets { DEFAULT }


...
```

现在枚举已经定义好了，让我们继续研究导入插件的方法：

```gdscript
func _get_preset_count():
	return Presets.size()
```

[_get_preset_count()](https://docs.godotengine.org/en/stable/classes/class_editorimportplugin.html#class-editorimportplugin-private-method-get-preset-count) 方法返回此插件定义的预设数量。我们现在只有一个预设，但我们可以通过返回 `Presets` 枚举的大小来使此方法不受未来影响。

```gdscript
func _get_preset_name(preset_index):
	match preset_index:
		Presets.DEFAULT:
			return "Default"
		_:
			return "Unknown"
```

这里我们有 [_get_preset_name()](https://docs.godotengine.org/en/stable/classes/class_editorimportplugin.html#class-editorimportplugin-private-method-get-preset-name) 方法，它为将呈现给用户的预设命名，所以一定要使用简短明了的名称。

我们可以在这里使用 `match` 语句来使代码更有条理。这样，将来很容易添加新的预设。我们也使用包罗万象的模式来返回一些东西。尽管 Godot 不会要求超出您定义的预设计数的预设，但最好是安全的。

如果你只有一个预设，你可以直接返回它的名称，但如果你这样做，在添加更多预设时必须小心。

```gdscript
func _get_import_options(path, preset_index):
	match preset_index:
		Presets.DEFAULT:
			return [{
					   "name": "use_red_anyway",
					   "default_value": false
					}]
		_:
			return []
```

这是定义可用选项的方法 [_get_import_options()](https://docs.godotengine.org/en/stable/classes/class_editorimportplugin.html#class-editorimportplugin-private-method-get-import-options) 返回一个字典数组，每个字典都包含一些键，这些键被选中以自定义选项，使其显示给用户。下表显示了可能的键：

| 键              | 类型    | 描述                                                         |
| --------------- | ------- | ------------------------------------------------------------ |
| `name`          | String  | 选项的名称。显示时，下划线变为空格，首字母大写。             |
| `default_value` | 任何    | 此预设选项的默认值。                                         |
| `property_hint` | Enum 值 | 其中一个 [PropertyHint](https://docs.godotengine.org/en/stable/classes/class_%40globalscope.html#enum-globalscope-propertyhint) 值用作提示。 |
| `hint_string`   | String  | 属性的提示文本。与您在 GDScript 中添加的 `export` 语句相同。 |
| `usage`         | Enum 值 | 其中一个 [PropertyUsageFlags](https://docs.godotengine.org/en/stable/classes/class_%40globalscope.html#enum-globalscope-propertyusageflags) 值来定义用法。 |

`name` 和 `default_value` 键是**必需的**，其余的是可选的。

请注意，`_get_import_options` 方法接收预设编号，因此您可以为每个不同的预设（尤其是默认值）配置选项。在这个例子中，我们使用 `match` 语句，但如果你有很多选项，而预设只更改了值，你可能想先创建选项数组，然后根据预设进行更改。

> **警告：**
>
> 即使您没有定义预设（通过使 `_get_preet_count` 返回零），也会调用 `_get_inmport_options` 方法。你必须返回一个数组，即使它是空的，否则你可能会得到错误。

```gdscript
func _get_option_visibility(path, option_name, options):
	return true
```

对于 [_get_option_visibility()](https://docs.godotengine.org/en/stable/classes/class_editorimportplugin.html#class-editorimportplugin-private-method-get-option-visibility) 方法，我们只需返回 `true`，因为我们所有的选项（即我们定义的单个选项）始终可见。

如果您需要使某些选项仅在另一个选项设置为特定值时可见，则可以在此方法中添加逻辑。

### `import` 方法

该过程中负责将文件转换为资源的繁重部分由 [_import()](https://docs.godotengine.org/en/stable/classes/class_editorimportplugin.html#class-editorimportplugin-private-method-import) 方法完成。我们的示例代码有点长，所以让我们分成几个部分：

```gdscript
func _import(source_file, save_path, options, r_platform_variants, r_gen_files):
	var file = FileAccess.open(source_file, FileAccess.READ)
	if file == null:
		return FileAccess.get_open_error()

	var line = file.get_line()
```

导入方法的第一部分打开并读取源文件。我们使用 [FileAccess](https://docs.godotengine.org/en/stable/classes/class_fileaccess.html#class-fileaccess) 类来实现这一点，传递编辑器提供的 `source_file` 参数。

如果打开文件时出错，我们会返回它，让编辑器知道导入不成功。

```gdscript
var channels = line.split(",")
if channels.size() != 3:
	return ERR_PARSE_ERROR

var color
if options.use_red_anyway:
	color = Color8(255, 0, 0)
else:
	color = Color8(int(channels[0]), int(channels[1]), int(channels[2]))
```

这段代码获取它之前读取的文件的行，并将其拆分为用逗号分隔的片段。如果多于或少于这三个值，则认为文件无效并报告错误。

然后，它创建一个新的 [Color](https://docs.godotengine.org/en/stable/classes/class_color.html#class-color) 变量，并根据输入文件设置其值。如果启用了 `use_red_anyway` 选项，则将颜色设置为纯红色。

```gdscript
var material = StandardMaterial3D.new()
material.albedo_color = color
```

这部分创建了一个新的 [StandardMaterial3D](https://docs.godotengine.org/en/stable/classes/class_standardmaterial3d.html#class-standardmaterial3d)，即导入的资源。我们创建一个新的实例，然后将其反照率颜色设置为之前的值。

```gdscript
return ResourceSaver.save(material, "%s.%s" % [save_path, _get_save_extension()])
```

这是最后一部分，也是非常重要的一部分，因为在这里我们将生成的资源保存到磁盘上。编辑器通过 `save_path` 参数生成并通知保存文件的路径。请注意，这**没有**扩展名，因此我们使用[字符串格式](https://docs.godotengine.org/en/stable/tutorials/scripting/gdscript/gdscript_format_string.html#doc-gdscript-printf)添加它。为此，我们调用前面定义的 `_get_save_extension` 方法，这样我们就可以确保它们不会不同步。

我们还返回 [ResourceSaver.save()](https://docs.godotengine.org/en/stable/classes/class_resourcesaver.html#class-resourcesaver-method-save) 方法的结果，因此如果此步骤中出现错误，编辑器将知道。

### 平台变体和生成的文件

您可能已经注意到，我们的插件忽略了 `import` 方法的两个参数。这些是返回参数（因此名称开头的 `r`），这意味着编辑器在调用导入方法后将从中读取。它们都是可以填充信息的数组。

如果需要根据目标平台以不同方式导入资源，则使用 `r_platform_variants` 参数。虽然它被称为*平台*变体，但它是基于[功能标签](https://docs.godotengine.org/en/stable/tutorials/export/feature_tags.html#doc-feature-tags)的存在，因此即使是同一个平台也可以根据设置有多个变体。

要导入平台变体，您需要在扩展之前将其与功能标记一起保存，然后将标记推送到 `r_platform_variants` 数组中，以便编辑器知道您已经导入了。

例如，假设我们为移动平台保存了不同的材料。我们需要做以下事情：

```gdscript
r_platform_variants.push_back("mobile")
return ResourceSaver.save(mobile_material, "%s.%s.%s" % [save_path, "mobile", _get_save_extension()])
```

`r_gen_files` 参数用于导入过程中生成的需要保留的额外文件。编辑器将查看它以了解依赖关系，并确保额外的文件不会被无意中删除。

这也是一个数组，应该填充您保存的文件的完整路径。例如，让我们为下一遍创建另一种材质并将其保存在其他文件中：

```gdscript
var next_pass = StandardMaterial3D.new()
next_pass.albedo_color = color.inverted()
var next_pass_path = "%s.next_pass.%s" % [save_path, _get_save_extension()]

err = ResourceSaver.save(next_pass, next_pass_path)
if err != OK:
	return err
r_gen_files.push_back(next_pass_path)
```

### 尝试插件

这是理论上的，但现在导入插件已经完成，让我们测试一下。确保您创建了示例文件（包含介绍部分中描述的内容）并将其另存为 `test.mtxt`。然后在项目设置中激活插件。

如果一切顺利，则将导入插件添加到编辑器中，并扫描文件系统，使自定义资源显示在 FileSystem dock 上。如果选择它并聚焦“导入”停靠栏，则可以在那里看到唯一的选择选项。

在场景中创建 MeshInstance3D 节点，并为其 Mesh 属性设置新的 SphereMesh。在检查器中展开“材质”部分，然后将文件从文件系统停靠栏拖动到材质属性。对象将在视口中更新为导入材质的蓝色。

![../../../_images/import_plugin_trying.png](https://docs.godotengine.org/en/stable/_images/import_plugin_trying.png)

转到导入坞，启用“无论如何使用红色”选项，然后单击“重新导入”。这将更新导入的材质，并应自动更新显示红色的视图。

就是这样！您的第一个导入插件已完成！现在，发挥创造力，为你喜欢的格式制作插件。这对于以自定义格式编写数据，然后在 Godot 中使用它非常有用，就像它们是本机资源一样。这表明导入系统是多么强大和可扩展。

### 用户贡献的笔记

在提交评论之前，请阅读[用户贡献笔记政策](https://github.com/godotengine/godot-docs-user-notes/discussions/1)。

**1 个评论** · 2 个回复



[horseyhorsey](https://github.com/horseyhorsey) [Sep 17, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/147#discussioncomment-10666037)

```
Unimplemented _get_priority in add-on.`
`BUG: File queued for import, but can't be imported, importer for type '' not found.
```

添加：

```gdscript
func _get_priority():
	return 2
```

​	[scriptsengineer](https://github.com/scriptsengineer) [Sep 27, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/147#discussioncomment-10769872)

​	为什么是 2？

​	[Jeremi360](https://github.com/Jeremi360) [Nov 30, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/147#discussioncomment-11421292)

​	我尝试以 JSON 格式导入一些文件。

​	我添加：

```gdscript
func _get_priority():
	return 2

# after that I also need to add this
func _get_import_order() -> int:
	return ResourceImporter.ImportOrder.IMPORT_ORDER_DEFAULT
```

​	但现在我收到了以下错误：

```
  editor/import/editor_import_plugin.cpp:123 - Condition "!d.has_all(needed)" is true.
  core/io/file_access.cpp:812 - Condition "f.is_null()" is true. Continuing.
```



## 3D 小工具插件

https://docs.godotengine.org/en/stable/tutorials/plugins/editor/3d_gizmos.html#d-gizmo-plugins

### 引言

编辑器和自定义插件使用 3D 小控件插件来定义附加到任何类型的 Node3D 节点的小控件。

本教程展示了定义自己的自定义小控件的两种主要方法。第一个选项适用于简单的小控件，并在插件结构中减少混乱，第二个选项将允许您存储一些每个小控件的数据。

> **注：**
>
> 本教程假设您已经知道如何制作通用插件。如有疑问，请参阅[制作插件](https://docs.godotengine.org/en/stable/tutorials/plugins/editor/making_plugins.html#doc-making-plugins)页面。

### EditorNode3DGizmo 插件

无论我们选择哪种方法，我们都需要创建一个新的 [EditorNode3DGizmoPlugin](https://docs.godotengine.org/en/stable/classes/class_editornode3dgizmoplugin.html#class-editornode3dgizmoplugin)。这将允许我们为新的小控件类型设置一个名称，并定义其他行为，例如小控件是否可以隐藏。

这将是一个基本设置：

```gdscript
# my_custom_gizmo_plugin.gd
extends EditorNode3DGizmoPlugin


func _get_gizmo_name():
	return "CustomNode"
```

```gdscript
# MyCustomEditorPlugin.gd
@tool
extends EditorPlugin


const MyCustomGizmoPlugin = preload("res://addons/my-addon/my_custom_gizmo_plugin.gd")

var gizmo_plugin = MyCustomGizmoPlugin.new()


func _enter_tree():
	add_node_3d_gizmo_plugin(gizmo_plugin)


func _exit_tree():
	remove_node_3d_gizmo_plugin(gizmo_plugin)
```

对于简单的小控件，继承 [EditorNode3DGizmoPlugin](https://docs.godotengine.org/en/stable/classes/class_editornode3dgizmoplugin.html#class-editornode3dgizmoplugin) 就足够了。如果你想存储一些每个小控件的数据，或者你正在将 Godot 3.0 小控件移植到 3.1+，你应该采用第二种方法。

### 简单的方法

第一步是在我们的自定义 gizmo 插件中重写 [_has_gizmo()](https://docs.godotengine.org/en/stable/classes/class_editornode3dgizmoplugin.html#class-editornode3dgizmoplugin-private-method-has-gizmo) 方法，以便在节点参数为我们的目标类型时返回 `true`。

```gdscript
# ...


func _has_gizmo(node):
	return node is MyCustomNode3D


# ...
```

然后我们可以重写像 [_redraw()](https://docs.godotengine.org/en/stable/classes/class_editornode3dgizmoplugin.html#class-editornode3dgizmoplugin-private-method-redraw) 这样的方法或所有与句柄相关的方法。

```gdscript
# ...


func _init():
	create_material("main", Color(1, 0, 0))
	create_handle_material("handles")


func _redraw(gizmo):
	gizmo.clear()

	var node3d = gizmo.get_node_3d()

	var lines = PackedVector3Array()

	lines.push_back(Vector3(0, 1, 0))
	lines.push_back(Vector3(0, node3d.my_custom_value, 0))

	var handles = PackedVector3Array()

	handles.push_back(Vector3(0, 1, 0))
	handles.push_back(Vector3(0, node3d.my_custom_value, 0))

	gizmo.add_lines(lines, get_material("main", gizmo), false)
	gizmo.add_handles(handles, get_material("handles", gizmo), [])


# ...
```

请注意，我们在 *_init* 方法中创建了一个材质，并使用 [get_material()](https://docs.godotengine.org/en/stable/classes/class_editornode3dgizmoplugin.html#class-editornode3dgizmoplugin-method-get-material) 在 *_redraw* 方法中检索它。此方法根据小控件的状态（选定和/或可编辑）检索材质的变体之一。

所以最后的插件看起来有点像这样：

```gdscript
extends EditorNode3DGizmoPlugin


const MyCustomNode3D = preload("res://addons/my-addon/my_custom_node_3d.gd")


func _init():
	create_material("main", Color(1,0,0))
	create_handle_material("handles")


func _has_gizmo(node):
	return node is MyCustomNode3D


func _redraw(gizmo):
	gizmo.clear()

	var node3d = gizmo.get_node_3d()

	var lines = PackedVector3Array()

	lines.push_back(Vector3(0, 1, 0))
	lines.push_back(Vector3(0, node3d.my_custom_value, 0))

	var handles = PackedVector3Array()

	handles.push_back(Vector3(0, 1, 0))
	handles.push_back(Vector3(0, node3d.my_custom_value, 0))

	gizmo.add_lines(lines, get_material("main", gizmo), false)
	gizmo.add_handles(handles, get_material("handles", gizmo), [])


# You should implement the rest of handle-related callbacks
# (_get_handle_name(), _get_handle_value(), _commit_handle(), ...).
```

请注意，我们只是在 *_redraw* 方法中添加了一些句柄，但我们仍然需要在 [EditorNode3DGizmoPlugin](https://docs.godotengine.org/en/stable/classes/class_editornode3dgizmoplugin.html#class-editornode3dgizmoplugin) 中实现其余与句柄相关的回调，以获得正常工作的句柄。

### 替代方法

在某些情况下，我们想提供我们自己的 [EditorNode3DGizmo](https://docs.godotengine.org/en/stable/classes/class_editornode3dgizmo.html#class-editornode3dgizmo) 实现，也许是因为我们想在每个小控件中存储一些状态，或者因为我们正在移植一个旧的小控件插件，我们不想经历重写过程。

在这些情况下，我们需要做的就是在我们的新 gizmo 插件中重写 [_create_gizmo()](https://docs.godotengine.org/en/stable/classes/class_editornode3dgizmoplugin.html#class-editornode3dgizmoplugin-private-method-create-gizmo)，这样它就会为我们想要定位的 Node3D 节点返回我们的自定义 gizmo 实现。

```gdscript
# my_custom_gizmo_plugin.gd
extends EditorNode3DGizmoPlugin


const MyCustomNode3D = preload("res://addons/my-addon/my_custom_node_3d.gd")
const MyCustomGizmo = preload("res://addons/my-addon/my_custom_gizmo.gd")


func _init():
	create_material("main", Color(1, 0, 0))
	create_handle_material("handles")


func _create_gizmo(node):
	if node is MyCustomNode3D:
		return MyCustomGizmo.new()
	else:
		return null
```

这样，所有的 gizmo 逻辑和绘图方法都可以在一个扩展 [EditorNode3DGizmo](https://docs.godotengine.org/en/stable/classes/class_editornode3dgizmo.html#class-editornode3dgizmo) 的新类中实现，如下所示：

```gdscript
# my_custom_gizmo.gd
extends EditorNode3DGizmo


# You can store data in the gizmo itself (more useful when working with handles).
var gizmo_size = 3.0


func _redraw():
	clear()

	var node3d = get_node_3d()

	var lines = PackedVector3Array()

	lines.push_back(Vector3(0, 1, 0))
	lines.push_back(Vector3(gizmo_size, node3d.my_custom_value, 0))

	var handles = PackedVector3Array()

	handles.push_back(Vector3(0, 1, 0))
	handles.push_back(Vector3(gizmo_size, node3d.my_custom_value, 0))

	var material = get_plugin().get_material("main", self)
	add_lines(lines, material, false)

	var handles_material = get_plugin().get_material("handles", self)
	add_handles(handles, handles_material, [])


# You should implement the rest of handle-related callbacks
# (_get_handle_name(), _get_handle_value(), _commit_handle(), ...).
```

请注意，我们只是在 *_redraw* 方法中添加了一些句柄，但我们仍然需要在 [EditorNode3DGizmo](https://docs.godotengine.org/en/stable/classes/class_editornode3dgizmo.html#class-editornode3dgizmo) 中实现其余与句柄相关的回调，以获得正常工作的句柄。

## 检查器插件

https://docs.godotengine.org/en/stable/tutorials/plugins/editor/inspector_plugins.html#inspector-plugins

检查器 dock 允许您创建自定义小部件，通过插件编辑属性。在使用自定义数据类型和资源时，这可能是有益的，尽管您可以使用该功能更改内置类型的检查器小部件。您可以为特定属性、整个对象，甚至与特定数据类型关联的单独控件设计自定义控件。

本指南解释了如何使用 [EditorInspectorPlugin](https://docs.godotengine.org/en/stable/classes/class_editorinspectorplugin.html#class-editorinspectorplugin) 和 [EditorProperty](https://docs.godotengine.org/en/stable/classes/class_editorproperty.html#class-editorproperty) 类为整数创建自定义接口，用一个生成 0 到 99 之间随机值的按钮替换默认行为。

![../../../_images/inspector_plugin_example.png](https://docs.godotengine.org/en/stable/_images/inspector_plugin_example.png)

*左侧为默认行为，右侧为最终结果。*

### 设置插件

创建一个新的空插件以开始。

> **另请参见：**
>
> 请参阅[制作插件](https://docs.godotengine.org/en/stable/tutorials/plugins/editor/making_plugins.html#doc-making-plugins)指南以设置新插件。

假设您已将插件文件夹命名为 `my_inspector_plugin`。如果是这样，您应该会得到一个新的 `addons/my_inspector_plugin` 文件夹，其中包含两个文件：`plugin.cfg` 和 `plugin.gd`。

如前所述，`plugin.gd` 是一个扩展 [EditorPlugin](https://docs.godotengine.org/en/stable/classes/class_editorplugin.html#class-editorplugin) 的脚本，您需要为其 `_enter_tree` 和 `_exit_tree` 方法引入新代码。要设置检查器插件，您必须加载其脚本，然后通过调用 `add_inspector_plugin()` 创建和添加实例。如果插件被禁用，您应该通过调用 `remove_inspector_plugin()` 来删除您添加的实例。

> **注：**
>
> 在这里，您正在加载一个脚本，而不是一个打包的场景。因此，您应该使用 `new()` 而不是 `instantiate()`。

```gdscript
# plugin.gd
@tool
extends EditorPlugin

var plugin


func _enter_tree():
	plugin = preload("res://addons/my_inspector_plugin/my_inspector_plugin.gd").new()
	add_inspector_plugin(plugin)


func _exit_tree():
	remove_inspector_plugin(plugin)
```

```c#
// Plugin.cs
#if TOOLS
using Godot;

[Tool]
public partial class Plugin : EditorPlugin
{
    private MyInspectorPlugin _plugin;

    public override void _EnterTree()
    {
        _plugin = new MyInspectorPlugin();
        AddInspectorPlugin(_plugin);
    }

    public override void _ExitTree()
    {
        RemoveInspectorPlugin(_plugin);
    }
}
#endif
```

### 与检查器互动

要与检查器 dock 交互，`my_inspector_plugin.gd` 脚本必须扩展 [EditorInspectorPlugin](https://docs.godotengine.org/en/stable/classes/class_editorinspectorplugin.html#class-editorinspectorplugin) 类。此类提供了几个影响检查器处理属性方式的虚拟方法。

为了产生任何效果，脚本必须实现 `_can_handle()` 方法。每个编辑过的 [Object](https://docs.godotengine.org/en/stable/classes/class_object.html#class-object) 都会调用此函数，如果此插件应处理该对象或其属性，则必须返回 `true`。

> **注：**
>
> 这包括附加到对象的任何 [Resource](https://docs.godotengine.org/en/stable/classes/class_resource.html#class-resource)。

您可以实现其他四种方法，在特定位置向检查器添加控件。`_parse_begin()` 和 `_parse_end()` 方法分别在每个对象的解析开始和结束时只调用一次。他们可以通过调用 `add_custom_control()` 在检查器布局的顶部或底部添加控件。

当编辑器解析对象时，它调用 `_parse_category()` 和 `_parse_property()` 方法。在那里，除了 `add_custom_control()` 之外，您还可以调用 `add_property_editor()` 和 `add_property_editor_for_multiple_properties()`。使用后两种方法专门添加基于 [EditorProperty](https://docs.godotengine.org/en/stable/classes/class_editorproperty.html#class-editorproperty) 的控件。

```gdscript
# my_inspector_plugin.gd
extends EditorInspectorPlugin

var RandomIntEditor = preload("res://addons/my_inspector_plugin/random_int_editor.gd")


func _can_handle(object):
	# We support all objects in this example.
	return true


func _parse_property(object, type, name, hint_type, hint_string, usage_flags, wide):
	# We handle properties of type integer.
	if type == TYPE_INT:
		# Create an instance of the custom property editor and register
		# it to a specific property path.
		add_property_editor(name, RandomIntEditor.new())
		# Inform the editor to remove the default property editor for
		# this property type.
		return true
	else:
		return false
```

```c#
// MyInspectorPlugin.cs
#if TOOLS
using Godot;

public partial class MyInspectorPlugin : EditorInspectorPlugin
{
    public override bool _CanHandle(GodotObject @object)
    {
        // We support all objects in this example.
        return true;
    }

    public override bool _ParseProperty(GodotObject @object, Variant.Type type,
        string name, PropertyHint hintType, string hintString,
        PropertyUsageFlags usageFlags, bool wide)
    {
        // We handle properties of type integer.
        if (type == Variant.Type.Int)
        {
            // Create an instance of the custom property editor and register
            // it to a specific property path.
            AddPropertyEditor(name, new RandomIntEditor());
            // Inform the editor to remove the default property editor for
            // this property type.
            return true;
        }

        return false;
    }
}
#endif
```

### 添加编辑属性的界面

[EditorProperty](https://docs.godotengine.org/en/stable/classes/class_editorproperty.html#class-editorproperty) 类是一种特殊类型的 [Control](https://docs.godotengine.org/en/stable/classes/class_control.html#class-control)，可以与检查器 dock 的编辑对象进行交互。它不显示任何内容，但可以容纳任何其他控制节点，包括复杂的场景。

扩展 [EditorProperty](https://docs.godotengine.org/en/stable/classes/class_editorproperty.html#class-editorproperty) 的脚本有三个基本部分：

1. 您必须定义 `_init()` 方法来设置控制节点的结构。
2. 您应该实现 `_update_property()` 来处理来自外部的数据更改。
3. 必须在某个时刻发出信号，通知检查器控件已使用 `emit_changed` 更改了属性。

您可以通过两种方式显示自定义小部件。只使用默认的 `add_child()` 方法将其显示在属性名称的右侧，并使用 `add_child()` 和 `set_bottom_editor()` 将其放置在名称下方。

```gdscript
# random_int_editor.gd
extends EditorProperty


# The main control for editing the property.
var property_control = Button.new()
# An internal value of the property.
var current_value = 0
# A guard against internal changes when the property is updated.
var updating = false


func _init():
	# Add the control as a direct child of EditorProperty node.
	add_child(property_control)
	# Make sure the control is able to retain the focus.
	add_focusable(property_control)
	# Setup the initial state and connect to the signal to track changes.
	refresh_control_text()
	property_control.pressed.connect(_on_button_pressed)


func _on_button_pressed():
	# Ignore the signal if the property is currently being updated.
	if (updating):
		return

	# Generate a new random integer between 0 and 99.
	current_value = randi() % 100
	refresh_control_text()
	emit_changed(get_edited_property(), current_value)


func _update_property():
	# Read the current value from the property.
	var new_value = get_edited_object()[get_edited_property()]
	if (new_value == current_value):
		return

	# Update the control with the new value.
	updating = true
	current_value = new_value
	refresh_control_text()
	updating = false

func refresh_control_text():
	property_control.text = "Value: " + str(current_value)
```

```c#
// RandomIntEditor.cs
#if TOOLS
using Godot;

public partial class RandomIntEditor : EditorProperty
{
    // The main control for editing the property.
    private Button _propertyControl = new Button();
    // An internal value of the property.
    private int _currentValue = 0;
    // A guard against internal changes when the property is updated.
    private bool _updating = false;

    public RandomIntEditor()
    {
        // Add the control as a direct child of EditorProperty node.
        AddChild(_propertyControl);
        // Make sure the control is able to retain the focus.
        AddFocusable(_propertyControl);
        // Setup the initial state and connect to the signal to track changes.
        RefreshControlText();
        _propertyControl.Pressed += OnButtonPressed;
    }

    private void OnButtonPressed()
    {
        // Ignore the signal if the property is currently being updated.
        if (_updating)
        {
            return;
        }

        // Generate a new random integer between 0 and 99.
        _currentValue = (int)GD.Randi() % 100;
        RefreshControlText();
        EmitChanged(GetEditedProperty(), _currentValue);
    }

    public override void _UpdateProperty()
    {
        // Read the current value from the property.
        var newValue = (int)GetEditedObject().Get(GetEditedProperty());
        if (newValue == _currentValue)
        {
            return;
        }

        // Update the control with the new value.
        _updating = true;
        _currentValue = newValue;
        RefreshControlText();
        _updating = false;
    }

    private void RefreshControlText()
    {
        _propertyControl.Text = $"Value: {_currentValue}";
    }
}
#endif
```

使用上面的示例代码，您应该能够制作一个自定义小部件，将整数的默认 [SpinBox](https://docs.godotengine.org/en/stable/classes/class_spinbox.html#class-spinbox) 控件替换为生成随机值的 [Button](https://docs.godotengine.org/en/stable/classes/class_button.html#class-button)。



## Visual Shader 插件

https://docs.godotengine.org/en/stable/tutorials/plugins/editor/visual_shader_plugins.html#visual-shader-plugins

Visual Shader 插件用于在 GDScript 中创建自定义 [VisualShader](https://docs.godotengine.org/en/stable/classes/class_visualshader.html#class-visualshader) 节点。

创建过程不同于通常的编辑器插件。您不需要创建 `plugin.cfg` 文件来注册它；相反，创建并保存一个脚本文件，只要自定义节点已用 `class_name` 注册，它就可以使用了。

本简短教程将解释如何制作 Perlin-3D 噪波节点（来自此 [GPU 噪波着色器插件](https://github.com/curly-brace/Godot-3.0-Noise-Shaders/blob/master/assets/gpu_noise_shaders/classic_perlin3d.tres)的原始代码）。

创建 Sprite2D 并将 [ShaderMaterial](https://docs.godotengine.org/en/stable/classes/class_shadermaterial.html#class-shadermaterial) 指定给其材质窗：

![../../../_images/visual_shader_plugins_start.png](https://docs.godotengine.org/en/stable/_images/visual_shader_plugins_start.png)

将 [VisualShader](https://docs.godotengine.org/en/stable/classes/class_visualshader.html#class-visualshader) 指定给材质的着色器窗：

![../../../_images/visual_shader_plugins_start2.png](https://docs.godotengine.org/en/stable/_images/visual_shader_plugins_start2.png)

不要忘记将其模式更改为“CanvasItem”（如果您使用的是 Sprite2D)：

![../../../_images/visual_shader_plugins_start3.png](https://docs.godotengine.org/en/stable/_images/visual_shader_plugins_start3.png)

创建一个从 [VisualShaderNodeCustom](https://docs.godotengine.org/en/stable/classes/class_visualshadernodecustom.html#class-visualshadernodecustom) 派生的脚本。这就是初始化插件所需的全部内容。

```gdscript
# perlin_noise_3d.gd
@tool
extends VisualShaderNodeCustom
class_name VisualShaderNodePerlinNoise3D


func _get_name():
	return "PerlinNoise3D"


func _get_category():
	return "MyShaderNodes"


func _get_description():
	return "Classic Perlin-Noise-3D function (by Curly-Brace)"


func _init():
	set_input_port_default_value(2, 0.0)


func _get_return_icon_type():
	return VisualShaderNode.PORT_TYPE_SCALAR


func _get_input_port_count():
	return 4


func _get_input_port_name(port):
	match port:
		0:
			return "uv"
		1:
			return "offset"
		2:
			return "scale"
		3:
			return "time"


func _get_input_port_type(port):
	match port:
		0:
			return VisualShaderNode.PORT_TYPE_VECTOR_3D
		1:
			return VisualShaderNode.PORT_TYPE_VECTOR_3D
		2:
			return VisualShaderNode.PORT_TYPE_SCALAR
		3:
			return VisualShaderNode.PORT_TYPE_SCALAR


func _get_output_port_count():
	return 1


func _get_output_port_name(port):
	return "result"


func _get_output_port_type(port):
	return VisualShaderNode.PORT_TYPE_SCALAR


func _get_global_code(mode):
	return """
        vec3 mod289_3(vec3 x) {
            return x - floor(x * (1.0 / 289.0)) * 289.0;
        }

        vec4 mod289_4(vec4 x) {
            return x - floor(x * (1.0 / 289.0)) * 289.0;
        }

        vec4 permute(vec4 x) {
            return mod289_4(((x * 34.0) + 1.0) * x);
        }

        vec4 taylorInvSqrt(vec4 r) {
            return 1.79284291400159 - 0.85373472095314 * r;
        }

        vec3 fade(vec3 t) {
            return t * t * t * (t * (t * 6.0 - 15.0) + 10.0);
        }

        // Classic Perlin noise.
        float cnoise(vec3 P) {
            vec3 Pi0 = floor(P); // Integer part for indexing.
            vec3 Pi1 = Pi0 + vec3(1.0); // Integer part + 1.
            Pi0 = mod289_3(Pi0);
            Pi1 = mod289_3(Pi1);
            vec3 Pf0 = fract(P); // Fractional part for interpolation.
            vec3 Pf1 = Pf0 - vec3(1.0); // Fractional part - 1.0.
            vec4 ix = vec4(Pi0.x, Pi1.x, Pi0.x, Pi1.x);
            vec4 iy = vec4(Pi0.yy, Pi1.yy);
            vec4 iz0 = vec4(Pi0.z);
            vec4 iz1 = vec4(Pi1.z);

            vec4 ixy = permute(permute(ix) + iy);
            vec4 ixy0 = permute(ixy + iz0);
            vec4 ixy1 = permute(ixy + iz1);

            vec4 gx0 = ixy0 * (1.0 / 7.0);
            vec4 gy0 = fract(floor(gx0) * (1.0 / 7.0)) - 0.5;
            gx0 = fract(gx0);
            vec4 gz0 = vec4(0.5) - abs(gx0) - abs(gy0);
            vec4 sz0 = step(gz0, vec4(0.0));
            gx0 -= sz0 * (step(0.0, gx0) - 0.5);
            gy0 -= sz0 * (step(0.0, gy0) - 0.5);

            vec4 gx1 = ixy1 * (1.0 / 7.0);
            vec4 gy1 = fract(floor(gx1) * (1.0 / 7.0)) - 0.5;
            gx1 = fract(gx1);
            vec4 gz1 = vec4(0.5) - abs(gx1) - abs(gy1);
            vec4 sz1 = step(gz1, vec4(0.0));
            gx1 -= sz1 * (step(0.0, gx1) - 0.5);
            gy1 -= sz1 * (step(0.0, gy1) - 0.5);

            vec3 g000 = vec3(gx0.x, gy0.x, gz0.x);
            vec3 g100 = vec3(gx0.y, gy0.y, gz0.y);
            vec3 g010 = vec3(gx0.z, gy0.z, gz0.z);
            vec3 g110 = vec3(gx0.w, gy0.w, gz0.w);
            vec3 g001 = vec3(gx1.x, gy1.x, gz1.x);
            vec3 g101 = vec3(gx1.y, gy1.y, gz1.y);
            vec3 g011 = vec3(gx1.z, gy1.z, gz1.z);
            vec3 g111 = vec3(gx1.w, gy1.w, gz1.w);

            vec4 norm0 = taylorInvSqrt(vec4(dot(g000, g000), dot(g010, g010), dot(g100, g100), dot(g110, g110)));
            g000 *= norm0.x;
            g010 *= norm0.y;
            g100 *= norm0.z;
            g110 *= norm0.w;
            vec4 norm1 = taylorInvSqrt(vec4(dot(g001, g001), dot(g011, g011), dot(g101, g101), dot(g111, g111)));
            g001 *= norm1.x;
            g011 *= norm1.y;
            g101 *= norm1.z;
            g111 *= norm1.w;

            float n000 = dot(g000, Pf0);
            float n100 = dot(g100, vec3(Pf1.x, Pf0.yz));
            float n010 = dot(g010, vec3(Pf0.x, Pf1.y, Pf0.z));
            float n110 = dot(g110, vec3(Pf1.xy, Pf0.z));
            float n001 = dot(g001, vec3(Pf0.xy, Pf1.z));
            float n101 = dot(g101, vec3(Pf1.x, Pf0.y, Pf1.z));
            float n011 = dot(g011, vec3(Pf0.x, Pf1.yz));
            float n111 = dot(g111, Pf1);

            vec3 fade_xyz = fade(Pf0);
            vec4 n_z = mix(vec4(n000, n100, n010, n110), vec4(n001, n101, n011, n111), fade_xyz.z);
            vec2 n_yz = mix(n_z.xy, n_z.zw, fade_xyz.y);
            float n_xyz = mix(n_yz.x, n_yz.y, fade_xyz.x);
            return 2.2 * n_xyz;
        }
    """


func _get_code(input_vars, output_vars, mode, type):
	return output_vars[0] + " = cnoise(vec3((%s.xy + %s.xy) * %s, %s)) * 0.5 + 0.5;" % [input_vars[0], input_vars[1], input_vars[2], input_vars[3]]
```

保存它并打开 Visual Shader。您应该在 Addons 类别下的成员对话框中看到新的节点类型（如果看不到新节点，请尝试重新启动编辑器）：

![../../../_images/visual_shader_plugins_result1.png](https://docs.godotengine.org/en/stable/_images/visual_shader_plugins_result1.png)

将其放在图表上并连接所需的端口：

![../../../_images/visual_shader_plugins_result2.png](https://docs.godotengine.org/en/stable/_images/visual_shader_plugins_result2.png)

这就是你需要做的一切，正如你所看到的，创建自己的自定义 VisualShader 节点很容易！



## 在编辑器中运行代码

https://docs.godotengine.org/en/stable/tutorials/plugins/running_code_in_the_editor.html#running-code-in-the-editor

### 什么是 `@tool`？

`@tool` 是一行功能强大的代码，当添加到脚本顶部时，它将在编辑器中执行。您还可以决定脚本的哪些部分在编辑器中执行，哪些在游戏中执行，以及哪些在两者中执行。

你可以用它做很多事情，但它在关卡设计中最有用，可以直观地呈现我们自己难以预测的东西。以下是一些用例：

- 如果你有一门可以发射受物理（重力）影响的炮弹的大炮，你可以在编辑器中绘制炮弹的轨迹，使关卡设计变得容易得多。
- 如果你有不同跳跃高度的跳台，你可以画出玩家跳上跳台时能达到的最大跳跃高度，这也使关卡设计更容易。
- 如果你的玩家不使用角色，而是使用代码绘制自己，你可以在编辑器中执行绘制代码来查看你的玩家。

> **危险：**
>
> `@tool` 脚本在编辑器内运行，并允许您访问当前编辑场景的场景树。这是一个强大的功能，但也有警告，因为编辑器不包括对 `@tool` 脚本潜在滥用的保护。在操作场景树时，尤其是通过 [Node.queue_free](https://docs.godotengine.org/en/stable/classes/class_node.html#class-node-method-queue-free)，要格外小心，因为如果在编辑器运行涉及节点的逻辑时释放节点，可能会导致崩溃。

### 如何使用 `@tool`

要将脚本转换为工具，请在代码顶部添加 `@tool` 注释。

要检查您当前是否在编辑器中，请使用：`Engine.is_editor_hint()`。

例如，如果您只想在编辑器中执行某些代码，请使用：

```gdscript
if Engine.is_editor_hint():
	# Code to execute when in editor.
```

```c#
if (Engine.IsEditorHint())
{
    // Code to execute when in editor.
}
```

另一方面，如果你只想在游戏中执行代码，只需否定相同的语句：

```gdscript
if not Engine.is_editor_hint():
	# Code to execute when in game.
```

```c#
if (!Engine.IsEditorHint())
{
    // Code to execute when in game.
}
```

不符合上述两个条件的代码将在编辑器和游戏中运行。

以下是 `_process()` 函数可能会如何查找您：

```gdscript
func _process(delta):
	if Engine.is_editor_hint():
		# Code to execute in editor.

	if not Engine.is_editor_hint():
		# Code to execute in game.

	# Code to execute both in editor and in game.
```

```c#
public override void _Process(double delta)
{
    if (Engine.IsEditorHint())
    {
        // Code to execute in editor.
    }

    if (!Engine.IsEditorHint())
    {
        // Code to execute in game.
    }

    // Code to execute both in editor and in game.
}
```

### 重要信息

您的工具脚本使用的任何其他 GDScript *也*必须是工具。编辑器使用的任何没有 `@tool` 的 GDScript 都将像一个空文件一样！

扩展 `@tool` 脚本不会自动使扩展脚本成为 `@tool`。从扩展脚本中省略 `@tool` 将禁用超类中的工具行为。因此，扩展脚本还应指定 `@tool` 注解。

编辑器中的修改是永久性的。例如，在下一节中，当我们删除脚本时，节点将保持其旋转。小心避免进行不必要的修改。

### 试试 `@tool`

将 `Sprite2D` 节点添加到场景中，并将纹理设置为 Godot 图标。附加并打开一个脚本，然后将其更改为：

```gdscript
@tool
extends Sprite2D

func _process(delta):
	rotation += PI * delta
```

```c#
using Godot;

[Tool]
public partial class MySprite : Sprite2D
{
    public override void _Process(double delta)
    {
        Rotation += Mathf.Pi * (float)delta;
    }
}
```

保存脚本并返回编辑器。现在，您应该看到您的对象正在旋转。如果你运行游戏，它也会旋转。

![../../_images/rotating_in_editor.gif](https://docs.godotengine.org/en/stable/_images/rotating_in_editor.gif)

> **注：**
>
> 如果看不到更改，请重新加载场景（关闭并再次打开）。

现在让我们选择什么时候运行哪些代码。修改 `_process()` 函数，使其看起来像这样：

```gdscript
func _process(delta):
	if Engine.is_editor_hint():
		rotation += PI * delta
	else:
		rotation -= PI * delta
```

```c#
public override void _Process(double delta)
{
    if (Engine.IsEditorHint())
    {
        Rotation += Mathf.Pi * (float)delta;
    }
    else
    {
        Rotation -= Mathf.Pi * (float)delta;
    }
}
```

保存脚本。现在，对象将在编辑器中顺时针旋转，但如果你运行游戏，它将逆时针旋转。

### 编辑变量

将变速添加并导出到脚本中。要更新速度并重置旋转角度，请添加一个设置器 `set(new_speed)` ，该设置器集根据检查器的输入执行。修改 `_process()` 以包含旋转速度。

```gdscript
@tool
extends Sprite2D


@export var speed = 1:
	# Update speed and reset the rotation.
	set(new_speed):
		speed = new_speed
		rotation = 0


func _process(delta):
	rotation += PI * delta * speed
```

```c#
using Godot;

[Tool]
public partial class MySprite : Sprite2D
{
    private float _speed = 1;

    [Export]
    public float Speed
    {
        get => _speed;
        set
        {
            // Update speed and reset the rotation.
            _speed = value;
            Rotation = 0;
        }
    }

    public override void _Process(double delta)
    {
        Rotation += Mathf.Pi * (float)delta * speed;
    }
}
```

> **注：**
>
> 来自其他节点的代码不会在编辑器中运行。您对其他节点的访问受到限制。您可以访问树和节点及其默认属性，但无法访问用户变量。如果你想这样做，其他节点也必须在编辑器中运行。在编辑器中根本无法访问自动加载节点。

### 当资源发生变化时收到通知

有时你希望你的工具使用资源。但是，当您在编辑器中更改该资源的属性时，将不会调用工具的 `set()` 方法。

```gdscript
@tool
class_name MyTool
extends Node

@export var resource: MyResource:
	set(new_resource):
		resource = new_resource
		_on_resource_set()

# This will only be called when you create, delete, or paste a resource.
# You will not get an update when tweaking properties of it.
func _on_resource_set():
	print("My resource was set!")
```

```c#
using Godot;

[Tool]
public partial class MyTool : Node
{
    private MyResource _resource;

    [Export]
    public MyResource Resource
    {
        get => _resource;
        set
        {
            _resource = value;
            OnResourceSet();
        }
    }
}

// This will only be called when you create, delete, or paste a resource.
// You will not get an update when tweaking properties of it.
private void OnResourceSet()
{
    GD.Print("My resource was set!");
}
```

要解决这个问题，您首先必须将资源设置为工具，并使其在设置属性时发出 `changed` 的信号：

```gdscript
# Make Your Resource a tool.
@tool
class_name MyResource
extends Resource

@export var property = 1:
	set(new_setting):
		property = new_setting
		# Emit a signal when the property is changed.
		changed.emit()
```

```c#
using Godot;

[Tool]
public partial class MyResource : Resource
{
    private float _property = 1;

    [Export]
    public float Property
    {
        get => _property;
        set
        {
            _property = value;
            // Emit a signal when the property is changed.
            EmitChanged();
        }
    }
}
```

然后，您希望在设置新资源时连接信号：

```gdscript
@tool
class_name MyTool
extends Node

@export var resource: MyResource:
	set(new_resource):
		resource = new_resource
		# Connect the changed signal as soon as a new resource is being added.
		resource.changed.connect(_on_resource_changed)

func _on_resource_changed():
	print("My resource just changed!")
```

```c#
using Godot;

[Tool]
public partial class MyTool : Node
{
    private MyResource _resource;

    [Export]
    public MyResource Resource
    {
        get => _resource;
        set
        {
            _resource = value;
            // Connect the changed signal as soon as a new resource is being added.
            _resource.Changed += OnResourceChanged;
        }
    }
}

private void OnResourceChanged()
{
    GD.Print("My resource just changed!");
}
```

最后，记得断开信号，因为在其他地方使用和更改的旧资源会导致不必要的更新。

```gdscript
@export var resource: MyResource:
	set(new_resource):
		# Disconnect the signal if the previous resource was not null.
		if resource != null:
			resource.changed.disconnect(_on_resource_changed)
		resource = new_resource
		resource.changed.connect(_on_resource_changed)
```

```c#
[Export]
public MyResource Resource
{
    get => _resource;
    set
    {
        // Disconnect the signal if the previous resource was not null.
        if (_resource != null)
        {
            _resource.Changed -= OnResourceChanged;
        }
        _resource = value;
        _resource.Changed += OnResourceChanged;
    }
}
```

### 报告节点配置警告

Godot 使用*节点配置警告*系统来警告用户节点配置不正确。当节点配置不正确时，“场景”停靠栏中节点名称旁边会显示一个黄色警告标志。当您悬停或单击图标时，会弹出一条警告消息。您可以在脚本中使用此功能，以帮助您和您的团队在设置场景时避免错误。

当使用节点配置警告时，当任何应该影响或删除警告的值发生变化时，您需要调用 [update_configuration_warnings](https://docs.godotengine.org/en/stable/classes/class_node.html#class-node-method-update-configuration-warnings)。默认情况下，警告仅在关闭和重新打开场景时更新。

```gdscript
# Use setters to update the configuration warning automatically.
@export var title = "":
	set(p_title):
		if p_title != title:
			title = p_title
			update_configuration_warnings()

@export var description = "":
	set(p_description):
		if p_description != description:
			description = p_description
			update_configuration_warnings()


func _get_configuration_warnings():
	var warnings = []

	if title == "":
		warnings.append("Please set `title` to a non-empty value.")

	if description.length() >= 100:
		warnings.append("`description` should be less than 100 characters long.")

	# Returning an empty array means "no warning".
	return warnings
```

### 使用 EditorScript 运行一次性脚本

有时，您只需要运行一次代码，就可以自动执行编辑器中不可用的特定任务。一些例子可能是：

- 用作 GDScript 或 C# 脚本的游乐场，而无需运行项目。`print()` 输出显示在编辑器输出面板中。
- 缩放当前编辑场景中的所有灯光节点，因为您注意到在将灯光放置在所需位置后，您的关卡看起来太暗或太亮。
- 将复制粘贴的节点替换为场景实例，以便以后更容易修改。

在 Godot 中，这可以通过在脚本中扩展 [EditorScript](https://docs.godotengine.org/en/stable/classes/class_editorscript.html#class-editorscript) 来实现。这提供了一种在编辑器中运行单个脚本的方法，而无需创建编辑器插件。

要创建 EditorScript，请右键单击文件系统停靠栏中的文件夹或空白区域，然后选择**新建 > 脚本...**。在脚本创建对话框中，单击树图标选择要从中扩展的对象（或直接在左侧的字段中输入 `EditorScript`，但请注意这是区分大小写的）：

![Creating an editor script in the script editor creation dialog](https://docs.godotengine.org/en/stable/_images/running_code_in_the_editor_creating_editor_script.webp)

*在脚本编辑器创建对话框中创建编辑器脚本*

这将自动选择一个适合 EditorScripts 的脚本模板，其中已插入 `_run()` 方法：

```gdscript
@tool
extends EditorScript

# Called when the script is executed (using File -> Run in Script Editor).
func _run():
	pass
```

当 EditorScript 是脚本编辑器中当前打开的脚本时，当您使用**“文件” > “运行”**或键盘快捷键 `Ctrl + Shift + X` 时，会执行此 `_run()` 方法。此键盘快捷键仅在当前专注于脚本编辑器时有效。

扩展 EditorScript 的脚本必须是 `@tool` 脚本才能运行。

> **警告：**
>
> EditorScripts 没有撤消/重做功能，因此如果脚本旨在修改任何数据，请**确保在运行场景之前保存场景**。

要访问当前编辑场景中的节点，请使用 [EditorScript.get_scene](https://docs.godotengine.org/en/stable/classes/class_editorscript.html#class-editorscript-method-get-scene) 方法，该方法返回当前编辑场景的根节点。以下是一个递归获取当前编辑场景中所有节点并将所有 OmniLight3D 节点的范围加倍的示例：

```gdscript
@tool
extends EditorScript

func _run():
	for node in get_all_children(get_scene()):
		if node is OmniLight3D:
			# Don't operate on instanced subscene children, as changes are lost
			# when reloading the scene.
			# See the "Instancing scenes" section below for a description of `owner`.
			var is_instanced_subscene_child = node != get_scene() and node.owner != get_scene()
			if not is_instanced_subscene_child:
				node.omni_range *= 2.0

# This function is recursive: it calls itself to get lower levels of child nodes as needed.
# `children_acc` is the accumulator parameter that allows this function to work.
# It should be left to its default value when you call this function directly.
func get_all_children(in_node, children_acc = []):
	children_acc.push_back(in_node)
	for child in in_node.get_children():
		children_acc = get_all_children(child, children_acc)

	return children_acc
```

> **小贴士：**
>
> 即使在“脚本”视图打开时，也可以在编辑器顶部更改当前编辑的场景。这将影响 [EditorScript.get_scene](https://docs.godotengine.org/en/stable/classes/class_editorscript.html#class-editorscript-method-get-scene) 的返回值，因此请确保在运行脚本之前已选择要迭代的场景。

### 实例化场景

您可以正常实例化打包的场景，并将其添加到编辑器中当前打开的场景中。默认情况下，使用 [Node.add_child(Node)](https://docs.godotengine.org/en/stable/classes/class_node.html#class-node-method-add-child) 添加的节点或场景在场景树停靠栏中**不**可见，也**不会**持久化到磁盘。如果希望节点或场景在场景树停靠栏中可见，并在保存场景时持久化到磁盘，则需要将子节点的 [owner](https://docs.godotengine.org/en/stable/classes/class_node.html#class-node-property-owner) 属性设置为当前编辑的场景根。

如果您正在使用 `@tool`：

```gdscript
func _ready():
	var node = Node3D.new()
	add_child(node) # Parent could be any node in the scene

	# The line below is required to make the node visible in the Scene tree dock
	# and persist changes made by the tool script to the saved scene file.
	node.owner = get_tree().edited_scene_root
```

```c#
public override void _Ready()
{
    var node = new Node3D();
    AddChild(node); // Parent could be any node in the scene

    // The line below is required to make the node visible in the Scene tree dock
    // and persist changes made by the tool script to the saved scene file.
    node.Owner = GetTree().EditedSceneRoot;
}
```

如果您使用的是 [EditorScript](https://docs.godotengine.org/en/stable/classes/class_editorscript.html#class-editorscript)：

```gdscript
func _run():
	# `parent` could be any node in the scene.
	var parent = get_scene().get_node("Parent")
	var node = Node3D.new()
	parent.add_child(node)

	# The line below is required to make the node visible in the Scene tree dock
	# and persist changes made by the tool script to the saved scene file.
	node.owner = get_scene()
```

```c#
public override void _Run()
{
    // `parent` could be any node in the scene.
    var parent = GetScene().GetNode("Parent");
    var node = new Node3D();
    parent.AddChild(node);

    // The line below is required to make the node visible in the Scene tree dock
    // and persist changes made by the tool script to the saved scene file.
    node.Owner = GetScene();
}
```

> **警告：**
>
> 不正确地使用 `@tool` 会产生许多错误。建议先按照您想要的方式编写代码，然后再将 `@tool` 注解添加到顶部。此外，请确保将编辑器中运行的代码与游戏中运行的程序代码分开。这样，你可以更容易地发现 bug。



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

### 视口容器

如果 [SubViewport](https://docs.godotengine.org/en/stable/classes/class_viewport.html#class-viewport) 是 [SubViewportContainer](https://docs.godotengine.org/en/stable/classes/class_subviewportcontainer.html#class-subviewportcontainer)的子级，则它将处于活动状态并显示其中的任何内容。布局如下：

![../../_images/container.webp](https://docs.godotengine.org/en/stable/_images/container.webp)

如果在 SubViewportContainer 中将 [Stretch](https://docs.godotengine.org/en/stable/classes/class_subviewportcontainer.html#class-subviewportcontainer-property-stretch) 设置为 `true`，则 [SubViewport](https://docs.godotengine.org/en/stable/classes/class_subviewport.html#class-subviewport) 将完全覆盖其父 [SubViewportContainer](https://docs.godotengine.org/en/stable/classes/class_subviewportcontainer.html#class-subviewportcontainer) 的区域。

> **注：**
>
> [SubViewportContainer](https://docs.godotengine.org/en/stable/classes/class_subviewportcontainer.html#class-subviewportcontainer) 的大小不能小于 [SubViewport](https://docs.godotengine.org/en/stable/classes/class_subviewport.html#class-subviewport) 的大小。

### 渲染

由于 [Viewport](https://docs.godotengine.org/en/stable/classes/class_viewport.html#class-viewport) 是进入另一个渲染曲面的入口，它暴露了一些可能与项目设置不同的渲染属性。您可以选择为每个视口使用不同级别的 [MSAA](https://docs.godotengine.org/en/stable/classes/class_viewport.html#class-viewport-property-msaa-2d)。默认行为为 `Disabled`。

如果知道 [Viewport](https://docs.godotengine.org/en/stable/classes/class_viewport.html#class-viewport) 仅用于二维，则可以[禁用三维](https://docs.godotengine.org/en/stable/classes/class_viewport.html#class-viewport-property-disable-3d)。然后，Godot 将限制 Viewport 的绘制方式。与启用 3D 相比，禁用 3D 稍微快一些，占用的内存更少。如果您的视口没有渲染任何 3D 内容，最好禁用 3D。

> **注：**
>
> 如果需要在视口中渲染 3D 阴影，请确保将视口的 [positional_shadow_atlas_size](https://docs.godotengine.org/en/stable/classes/class_viewport.html#class-viewport-property-positional-shadow-atlas-size) 属性设置为大于 `0` 的值。否则，将不会渲染阴影。默认情况下，在桌面平台上，等效项目设置设置为 `4096`，在移动平台上设置为 `2048`。

Godot 还提供了一种使用 [Debug Draw](https://docs.godotengine.org/en/stable/classes/class_viewport.html#class-viewport-property-debug-draw) 自定义如何在 [Viewports](https://docs.godotengine.org/en/stable/classes/class_viewport.html#class-viewport) 内绘制所有内容的方法。Debug Draw允许您指定一种模式，该模式决定了视口如何显示其内部绘制的内容。默认情况下，Debug Draw 处于 `Disabled` 状态。其他一些选项包括 `Unshaded`、`Overdraw` 和 `Wireframe`。有关完整列表，请参阅[视口文档](https://docs.godotengine.org/en/stable/classes/class_viewport.html#class-viewport-property-debug-draw)。

- **调试绘制 = 禁用**（默认）：场景正常绘制。

  ![../../_images/default_scene.webp](https://docs.godotengine.org/en/stable/_images/default_scene.webp)

- **Debug Draw = Unshaded**：Unshaded 在不使用照明信息的情况下绘制场景，这样所有对象都会以其反照率颜色显示为单色。

  ![../../_images/unshaded.webp](https://docs.godotengine.org/en/stable/_images/unshaded.webp)

- **Debug Draw = Overdraw**：Overdraw 使用添加混合绘制半透明网格，以便您可以看到网格是如何重叠的。

  ![../../_images/overdraw.webp](https://docs.godotengine.org/en/stable/_images/overdraw.webp)

- **调试绘制 = 线框**：线框仅使用网格中三角形的边绘制场景。

  ![../../_images/wireframe.webp](https://docs.godotengine.org/en/stable/_images/wireframe.webp)

> **注：**
>
> 使用兼容性呈现方法时，当前**不**支持调试绘图模式。它们将显示为常规绘图模式。

### 渲染目标

渲染到 [SubViewport](https://docs.godotengine.org/en/stable/classes/class_subviewport.html#class-subviewport) 时，场景编辑器中不会显示其中的任何内容。要显示内容，您必须在某处绘制子视口的 [ViewportTexture](https://docs.godotengine.org/en/stable/classes/class_viewporttexture.html#class-viewporttexture)。这可以通过代码请求（例如）：

```gdscript
# This gives us the ViewportTexture.
var tex = viewport.get_texture()
sprite.texture = tex
```

```c#
// This gives us the ViewportTexture.
var tex = viewport.GetTexture();
sprite.Texture = tex;
```

或者可以在编辑器中选择“新建 ViewportTexture”进行分配

![../../_images/texturemenu.webp](https://docs.godotengine.org/en/stable/_images/texturemenu.webp)

然后选择要使用的视口。

![../../_images/texturepath.webp](https://docs.godotengine.org/en/stable/_images/texturepath.webp)

每一帧，[Viewport](https://docs.godotengine.org/en/stable/classes/class_viewport.html#class-viewport) 的纹理都会用默认的透明颜色清除（如果“[透明 BG](https://docs.godotengine.org/en/stable/classes/class_viewport.html#class-viewport-property-transparent-bg)”设置为 `true`，则使用透明颜色)。这可以通过将“[清除模式](https://docs.godotengine.org/en/stable/classes/class_subviewport.html#class-subviewport-property-render-target-clear-mode)”设置为 `Never` 或 `NextFrame` 来更改。顾名思义，Never 意味着纹理永远不会被清除，而下一帧将清除下一帧上的纹理，然后将其设置为 Never。

默认情况下，当子视口的 [ViewportTexture](https://docs.godotengine.org/en/stable/classes/class_viewporttexture.html#class-viewporttexture) 在帧中绘制时，会重新渲染 [SubViewport](https://docs.godotengine.org/en/stable/classes/class_subviewport.html#class-subviewport)。如果可见，它将被渲染，否则将不会。通过将“[更新模式](https://docs.godotengine.org/en/stable/classes/class_subviewport.html#class-subviewport-property-render-target-update-mode)”设置为 `Never`、`Once`、`Always` 或 `When Parent Visible`，可以更改此行为。Never 和 Always 永远不会或总是分别重新渲染。Once 将重新渲染下一帧，然后更改为 Never。这可用于手动更新视口。这种灵活性允许用户渲染一次图像，然后使用纹理，而不会产生渲染每一帧的成本。

> **注：**
>
> 请务必查看视口演示。它们位于演示存档的视口文件夹中，或位于 https://github.com/godotengine/godot-demo-projects/tree/master/viewport.



## 多分辨率

https://docs.godotengine.org/en/stable/tutorials/rendering/multiple_resolutions.html#multiple-resolutions

### 多分辨率的问题

开发人员经常很难理解如何在他们的游戏中最好地支持多种分辨率。对于桌面和主机游戏，这或多或少是简单的，因为大多数屏幕宽高比是 16:9，分辨率是标准的（720p、1080p、1440p、4K 等）。

对于手机游戏来说，起初很容易。多年来，iPhone 和 iPad 使用相同的分辨率。当 *Retina* 被实现时，他们只是将像素密度提高了一倍；大多数开发商不得不以默认和双重分辨率提供资产。

如今，情况已不再如此，因为有很多不同的屏幕尺寸、密度和纵横比。非传统尺寸也越来越受欢迎，例如超宽显示器。

对于 3D 游戏，不需要支持多种分辨率（从美学角度来看）。3D 几何体将仅根据视场填充屏幕，而忽略纵横比。在这种情况下，人们可能希望支持这一点的主要原因是*性能*原因（以较低的分辨率运行以增加每秒帧数）。

对于 2D 和游戏 UI 来说，这是另一回事，因为艺术需要在 Photoshop、GIMP 或 Krita 等软件中使用特定的像素大小来创建。

由于布局、纵横比、分辨率和像素密度可能会发生很大变化，因此不再可能为每个特定屏幕设计 UI。必须使用另一种方法。

### 一个尺寸适配所有

最常见的方法是使用单个基础分辨率，然后将其与其他所有内容相匹配。这个分辨率是大多数玩家玩游戏的方式（考虑到他们的硬件）。对于移动设备，谷歌在网上有有用的[统计数据](https://developer.android.com/about/dashboards)，对于台式机，Steam [也有](https://store.steampowered.com/hwsurvey/)。

例如，Steam 显示最常见的*主显示器分辨率*是 1920×1080，因此一个明智的方法是开发一款适用于此分辨率的游戏，然后处理不同尺寸和纵横比的缩放。

Godot 提供了几个有用的工具来轻松实现这一点。

> **另请参见：**
>
> 您可以使用[多分辨率和纵横比演示项目](https://github.com/godotengine/godot-demo-projects/tree/master/gui/multiple_resolutions)看到 Godot 对多分辨率的支持是如何工作的。

### 基础尺寸

可以在**“显示”→“窗口”**下的“项目设置”中指定窗口的基本大小。

![../../_images/screenres.webp](https://docs.godotengine.org/en/stable/_images/screenres.webp)

然而，它的作用并不完全明显；引擎将不会尝试将监视器切换到此分辨率。相反，将此设置视为“设计大小”，即您在编辑器中使用的区域的大小。此设置直接对应于 2D 编辑器中蓝色矩形的大小。

通常需要支持屏幕和窗口尺寸与此基本尺寸不同的设备。Godot 提供了许多方法来控制视口如何调整大小和拉伸到不同的屏幕大小。

> **注：**
>
> 在此页面上，*窗口*是指系统分配给游戏的屏幕区域，而*视口*是指游戏控制以填充此屏幕区域的根对象（可从 `get_tree().root` 访问）。此视口是 [Window](https://docs.godotengine.org/en/stable/classes/class_window.html#class-window) 实例。回想一下，在[介绍](https://docs.godotengine.org/en/stable/tutorials/rendering/viewports.html#doc-viewports)中，*所有*窗口对象都是视口。

要在运行时从脚本配置拉伸基大小，请使用 `get_tree().root.content_scale_size` 属性（请参阅 [Window.content_scale_size](https://docs.godotengine.org/en/stable/classes/class_window.html#class-window-property-content-scale-size)）。更改此值可以间接更改二维元素的大小。但是，为了提供用户可访问的缩放选项，建议使用“[拉伸比例](https://docs.godotengine.org/en/stable/tutorials/rendering/multiple_resolutions.html#doc-multiple-resolutions-stretch-scale)”，因为它更容易调整。

> **注：**
>
> Godot 采用现代方法处理多种分辨率。引擎永远不会自行改变显示器的分辨率。虽然更改显示器的分辨率是最有效的方法，但它也是最不可靠的方法，因为如果游戏崩溃，它可能会使显示器停留在低分辨率上。这在 macOS 或 Linux 上尤其常见，因为它们不能像 Windows 那样处理分辨率更改。
>
> 更改显示器的分辨率也会消除游戏开发人员对过滤和纵横比拉伸的任何控制，这对于确保像素艺术游戏的正确显示非常重要。
>
> 除此之外，更改显示器的分辨率会使 alt-tab 在游戏中的切换速度变慢，因为每次切换时显示器都必须更改分辨率。

### 调整大小

有几种类型的设备，几种类型的屏幕，它们又具有不同的像素密度和分辨率。处理所有这些可能是一项艰巨的工作，因此 Godot 试图让开发人员的生活更轻松一些。[Viewport](https://docs.godotengine.org/en/stable/classes/class_viewport.html#class-viewport) 节点有几个函数来处理大小调整，场景树的根节点始终是一个视口（加载的场景被实例化为它的子节点，并且始终可以通过调用 `get_tree().root` 或 `get_node("/root")` 来访问它）。

无论如何，虽然更改根视口参数可能是处理问题的最灵活的方法，但这可能需要大量的工作、代码和猜测，因此 Godot 在项目设置中提供了一组参数来处理多种分辨率。

### 拉伸设置

拉伸设置位于项目设置中，提供了几个选项：

![../../_images/stretchsettings.webp](https://docs.godotengine.org/en/stable/_images/stretchsettings.webp)

#### 拉伸模式

“**拉伸模式**”设置定义了如何拉伸基础尺寸以适应窗口或屏幕的分辨率。下面的动画使用仅 16×9 像素的“基本尺寸”来演示不同拉伸模式的效果。一个同样大小为 16×9 像素的精灵图覆盖了整个视口，并在其顶部添加了一条对角线 [Line2D](https://docs.godotengine.org/en/stable/classes/class_line2d.html#class-line2d)：

![../../_images/stretch_demo_scene.png](https://docs.godotengine.org/en/stable/_images/stretch_demo_scene.png)

- **拉伸模式 = 禁用**（默认)：不进行拉伸。场景中的一个单位对应于屏幕上的一个像素。在此模式下，“**拉伸纵横比**”设置无效。

  ![../../_images/stretch_disabled_expand.gif](https://docs.godotengine.org/en/stable/_images/stretch_disabled_expand.gif)

- **拉伸模式 = 画布项目**：在此模式下，在项目设置中以宽度和高度指定的基础尺寸被拉伸以覆盖整个屏幕（考虑到“**拉伸纵横比**”设置）。这意味着所有内容都直接以目标分辨率呈现。3D 不受影响，而在 2D 中，子画面像素和屏幕像素之间不再有 1:1 的对应关系，这可能会导致缩放伪影。

  ![../../_images/stretch_2d_expand.gif](https://docs.godotengine.org/en/stable/_images/stretch_2d_expand.gif)

- **拉伸模式 = 视口**：视口缩放意味着根 [Viewport](https://docs.godotengine.org/en/stable/classes/class_viewport.html#class-viewport) 的大小被精确地设置为在“项目设置”的“**显示**”部分中指定的基准大小。场景首先渲染到此视口。最后，缩放此视口以适应屏幕（考虑到“**拉伸纵横比**”设置）。

  ![../../_images/stretch_viewport_expand.gif](https://docs.godotengine.org/en/stable/_images/stretch_viewport_expand.gif)

要在运行时从脚本配置拉伸模式，请使用 `get_tree().root.content_scale_mode` 属性（请参阅 [Window.content_scale_mode](https://docs.godotengine.org/en/stable/classes/class_window.html#class-window-property-content-scale-mode) 和 [ContentScaleMode](https://docs.godotengine.org/en/stable/classes/class_window.html#enum-window-contentscalemode) 枚举）。

#### 拉伸比例

第二个设置是拉伸比例。请注意，这仅在“**拉伸模式**”设置为“**禁用**”以外的其他值时生效。

在下面的动画中，您会注意到灰色和黑色区域。黑色区域是由引擎添加的，不能被绘制。灰色区域是场景的一部分，可以绘制到。灰色区域对应于您在 2D 编辑器中看到的蓝色框外的区域。

- **拉伸纵横比 = 忽略**：拉伸屏幕时忽略纵横比。这意味着原始分辨率将被拉伸以完全填充屏幕，即使它更宽或更窄。这可能会导致拉伸不均匀：物体看起来比设计的更宽或更高。

  ![../../_images/stretch_viewport_ignore.gif](https://docs.godotengine.org/en/stable/_images/stretch_viewport_ignore.gif)

- **拉伸纵横比 = 保持**：拉伸屏幕时保持纵横比。这意味着，无论屏幕分辨率如何，视口都将保持其原始大小，并且黑色条将添加到屏幕的顶部/底部（“letterboxing”）或侧面（“pillarboxing”）。

  如果你提前知道目标设备的宽高比，或者你不想处理不同的宽高比的话，这是一个不错的选择。

  ![../../_images/stretch_viewport_keep.gif](https://docs.godotengine.org/en/stable/_images/stretch_viewport_keep.gif)

- **拉伸纵横比 = 保持宽度**：拉伸屏幕时保持纵横比。如果屏幕比基本尺寸宽，则会在左右两侧添加黑色条（pillarboxing）。但是，如果屏幕高于基本分辨率，视口将在垂直方向上生长（底部将看到更多内容）。你也可以把这看作是“垂直扩展”。

  这通常是创建可缩放的 GUI 或 HUD 的最佳选择，因此一些控件可以锚定在底部（[大小和锚定](https://docs.godotengine.org/en/stable/tutorials/ui/size_and_anchors.html#doc-size-and-anchors)）。

  ![../../_images/stretch_viewport_keep_width.gif](https://docs.godotengine.org/en/stable/_images/stretch_viewport_keep_width.gif)

- **拉伸纵横比 = 保持高度**：拉伸屏幕时保持纵横比。如果屏幕高于基本尺寸，则在顶部和底部添加黑色条（letterboxing）。但是，如果屏幕比基本分辨率宽，视口将在水平方向上增长（右侧将看到更多内容）。你也可以把这看作是“横向扩展”。

  这通常是水平滚动的2D游戏（如跑步者或平台游戏）的最佳选择。

  ![../../_images/stretch_viewport_keep_height.gif](https://docs.godotengine.org/en/stable/_images/stretch_viewport_keep_height.gif)

- **拉伸纵横比 = 扩展**：拉伸屏幕时保持纵横比，但既不保持底部宽度也不保持高度。根据屏幕纵横比，视口将在水平方向上（如果屏幕比基本尺寸宽）或垂直方向（如果屏幕高于原始尺寸）更大。

  ![../../_images/stretch_viewport_expand.gif](https://docs.godotengine.org/en/stable/_images/stretch_viewport_expand.gif)

> **小贴士：**
>
> 要使用类似的自动确定的比例因子支持纵向和横向模式，请将项目的基本分辨率设置为正方形（1:1 纵横比）而不是矩形。例如，如果您希望将 1280×720 设计为基本分辨率，但希望同时支持纵向和横向模式，请在“项目设置”中使用 720×720 作为项目的基本窗口大小。
>
> 要允许用户在运行时选择他们喜欢的屏幕方向，请记住将**“显示”>“窗口”>“手持设备”>“方向”**设置为 `sensor`。

要在运行时从脚本配置拉伸特性，请使用 `get_tree().root.content_scale_aspect` 属性（请参阅 [Window.content_scale_aspect](https://docs.godotengine.org/en/stable/classes/class_window.html#class-window-property-content-scale-aspect) 和 [ContentScaleAspect](https://docs.godotengine.org/en/stable/classes/class_window.html#enum-window-contentscaleaspect) 枚举）。

#### 拉伸缩放

**“缩放（Scale）”**设置允许您在上述“**拉伸**”选项已经提供的基础上添加额外的缩放因子。默认值 `1.0` 表示不会发生额外的缩放。

例如，如果将“**缩放**”设置为 `2.0`，并将“**拉伸模式**”设置为“**禁用**”，则场景中的每个单位将对应于屏幕上的 2×2 像素。这是为非游戏应用程序提供缩放选项的好方法。

如果“**拉伸模式**”设置为 **canvas_items**，则二维元素将相对于基本窗口大小进行缩放，然后乘以“**缩放**”设置。这可以暴露给玩家，让他们根据自己的喜好调整自动确定的比例，以获得更好的可访问性。

如果“**拉伸模式**”设置为**视口**，则视口的分辨率除以“**缩放**”。这会使像素看起来更大，并降低渲染分辨率（在给定的窗口大小下），从而提高性能。

要在运行时从脚本配置拉伸比例，请使用 `get_tree().root.content_scale_factor` 属性（请参阅 [Window.content_scale_factor](https://docs.godotengine.org/en/stable/classes/class_window.html#class-window-property-content-scale-factor)）。

#### 拉伸缩放模式

从 Godot 4.2 开始，**拉伸缩放模式**设置允许您将自动确定的缩放因子（以及手动指定的**拉伸缩放**设置）约束为整数值。默认情况下，此设置设置为 `fractional`，允许应用任何缩放因子（包括 `2.5` 等分数值）。当设置为 `integer` 时，该值将四舍五入到最接近的整数。例如，它将四舍五入到 `2.0`，而不是使用 `2.5` 的缩放因子。这有助于在显示像素艺术时防止失真。

将使用 `viewport` 拉伸模式显示的此像素艺术与设置为 `fractional` 的拉伸比例模式进行比较：

![Fractional scaling example (incorrect pixel art appearance)](https://docs.godotengine.org/en/stable/_images/multiple_resolutions_pixel_art_fractional_scaling.webp)

*棋盘看起来并不“均匀”。徽标和文本中的线宽差异很大。*

此像素艺术也以 `viewport` 拉伸模式显示，但这次拉伸比例模式设置为 `integer`：

![Integer scaling example (correct pixel art appearance)](https://docs.godotengine.org/en/stable/_images/multiple_resolutions_pixel_art_integer_scaling.webp)

*棋盘看起来非常均匀。线宽一致。*

例如，如果视口基准大小为 640×360，窗口大小为 1366×768：

- 使用 `fractional` 时，视口以 1366×768 的分辨率显示（缩放因子约为 2.133×）。整个窗口空间都被使用了。视口中的每个像素对应于显示区域中的 2.133×2.133 像素。然而，由于显示器只能显示“整个”像素，这将导致像素缩放不均匀，从而导致像素艺术的外观不正确。
- 使用 `integer` 时，视口以 1280×720 的分辨率显示（缩放因子为 2×）。剩余的空间在所有四个侧面都填充了黑色条，因此视口中的每个像素对应于显示区域中的 2×2 像素。

此设置对任何拉伸模式都有效。但是，当使用 `disabled` 的拉伸模式时，它只会通过将其四舍五入到最接近的整数值来影响“**拉伸缩放**”设置。这可用于具有像素艺术 UI 的 3D 游戏，这样 3D 视口中的可见区域就不会减小大小（当使用 `canvas_items` 或具有 `integer` 缩放模式的 `viewport` 拉伸模式时会发生这种情况）。

> **小贴士：**
>
> 游戏应使用**独占全屏**窗口模式，而不是**全屏**模式，全屏模式旨在防止 Windows 自动将窗口视为独占全屏。
>
> **全屏**是指希望使用每像素透明度而不会被操作系统禁用的 GUI 应用程序使用的。它通过在屏幕底部留下一条 1 像素的线来实现这一点。相比之下，**Exclusive Fullscreen** 使用实际屏幕大小，并允许 Windows 减少全屏游戏的抖动和输入延迟。
>
> 当使用整数缩放时，这一点尤为重要，因为**全屏**模式下的 1 像素高度降低可能会导致整数缩放使用比预期更小的缩放因子。

### 常见用例场景

建议使用以下设置来很好地支持多种分辨率和宽高比。

#### 桌面游戏

**非像素艺术**：

- 将基本窗口宽度设置为 `1920`，窗口高度设置为 `1080`。如果显示尺寸小于 1920×1080，请将“**窗口宽度替代**”和“**窗口高度替代**”设置为较低的值，以便在项目开始时使窗口变小。
- 或者，如果您主要针对高端设备，请将基本窗口宽度设置为 `3840`，窗口高度设置为 `2160`。这允许您提供更高分辨率的 2D 资源，以更高的内存使用率和文件大小为代价，实现更清晰的视觉效果。请注意，这将使低分辨率设备上的非 mipmap 纹理具有颗粒感，因此请确保遵循“[减少下采样时的混叠](https://docs.godotengine.org/en/stable/tutorials/rendering/multiple_resolutions.html#doc-multiple-resolutions-reducing-aliasing-on-downsampling)”中描述的说明。
- 将拉伸模式设置为 `canvas_items`。
- 将拉伸方面设置为 `expand`。这允许支持多种宽高比，并更好地利用高智能手机显示屏（如 18:9 或 19:9 宽高比）。
- 使用**布局**菜单配置控制节点的锚点以捕捉到正确的角。

**像素艺术**：

- 将基本窗口大小设置为要使用的视口大小。大多数像素艺术游戏使用 256×224 到 640×480 之间的视口大小。640×360 是一个很好的基线，因为使用整数缩放时，它可以缩放到 1280×720、1920×1080、2560×1440 和 3840×2160，没有任何黑条。更高的视口尺寸将需要使用更高分辨率的艺术品，除非您打算在给定时间显示更多的游戏世界。
- 将拉伸模式设置为 `viewport`。
- 将拉伸纵横比设置为 `keep`，以强制执行单一纵横比（带黑条）。作为替代方案，您可以将拉伸纵横比设置为 `expand` 以支持多种纵横比。
- 如果使用 `expand` 拉伸比例，请使用“**布局**”菜单配置控制节点的锚点以捕捉到正确的角。
- 将拉伸比例模式设置为 `integer`。这可以防止出现不均匀的像素缩放，从而使像素艺术无法按预期显示。

> **注：**
>
> `viewport` 拉伸模式提供低分辨率渲染，然后将其拉伸到最终窗口大小。如果您同意精灵图能够在“亚像素”位置移动或旋转，或者希望拥有高分辨率的 3D 视口，则应使用 `canvas_items` 拉伸模式而不是 `viewport` 拉伸模式。

#### 横向模式下的手机游戏

Godot 默认配置为使用横向模式。这意味着您不需要更改显示方向项目设置。

- 将基本窗口宽度设置为 `1280`，窗口高度设置为 `720`。
- 或者，如果您主要针对高端设备，请将基本窗口宽度设置为 `1920`，窗口高度设置为 `1080`。这允许您提供更高分辨率的 2D 资源，以更高的内存使用率和文件大小为代价，实现更清晰的视觉效果。许多设备具有更高分辨率的显示器（1440p），但考虑到智能手机显示器的尺寸很小，与 1080p 的差异几乎看不见。请注意，这将使低分辨率设备上的非 mipmap 纹理具有颗粒感，因此请确保遵循“[减少下采样时的混叠](https://docs.godotengine.org/en/stable/tutorials/rendering/multiple_resolutions.html#doc-multiple-resolutions-reducing-aliasing-on-downsampling)”中描述的说明。
- 将拉伸模式设置为 `canvas_items`。
- 将拉伸方面设置为 `expand`。这允许支持多种宽高比，并更好地利用高智能手机显示屏（如 18:9 或 19:9 宽高比）。
- 使用**布局**菜单配置控制节点的锚点以捕捉到正确的角。

> **小贴士：**
>
> 为了更好地支持平板电脑和可折叠手机（其显示器的宽高比通常接近 4:3），请考虑使用宽高比为 4:3 的基本分辨率，同时遵循此处的其余说明。例如，您可以将基本窗口宽度设置为 `1280`，将基本窗口高度设置为 `960`。

#### 竖屏模式下的手机游戏

- 将基础窗口宽度设置为 `720`，窗口高度设置为 `1280`。
- 或者，如果您主要针对高端设备，请将基本窗口宽度设置为 `1080`，窗口高度设置为 `1920`。这允许您提供更高分辨率的 2D 资源，以更高的内存使用率和文件大小为代价，实现更清晰的视觉效果。许多设备具有更高分辨率的显示器（1440p），但考虑到智能手机显示器的尺寸很小，与 1080p 的差异几乎看不见。请注意，这将使低分辨率设备上的非 mipmap 纹理具有颗粒感，因此请确保遵循“[减少下采样时的混叠](https://docs.godotengine.org/en/stable/tutorials/rendering/multiple_resolutions.html#doc-multiple-resolutions-reducing-aliasing-on-downsampling)”中描述的说明。
- 将“**显示 > 窗口 > 手持设备 > 方向**”设置为 `portrait`。
- 将拉伸模式设置为 `canvas_items`。
- 将拉伸方面设置为 `expand`。这允许支持多种宽高比，并更好地利用高智能手机显示屏（如 18:9 或 19:9 宽高比）。
- 使用**布局**菜单配置控制节点的锚点以捕捉到正确的角。

> **小贴士：**
>
> 为了更好地支持平板电脑和可折叠手机（其显示器的宽高比通常接近 4:3），请考虑使用宽高比为 3:4 的基本分辨率，同时遵循此处的其余说明。例如，您可以将基本窗口宽度设置为 `960`，将基本窗口高度设置为 `1280`。

#### 非游戏应用程序

- 将基础窗口的宽度和高度设置为您想要的最小窗口大小。这不是必需的，但这可以确保您在设计 UI 时考虑到小窗口大小。
- 将拉伸模式保持为默认值，`disabled`。
- 将拉伸方面保持为默认值，`ignore`（由于拉伸模式 `disabled`，因此不会使用其值）。
- 您可以通过在脚本的 `_ready()` 函数中调用 `get_window().set_min_size()` 来定义最小窗口大小。这可以防止用户将应用程序的大小调整到低于某个大小，这可能会破坏 UI 布局。

> **注：**
>
> Godot 还不支持手动覆盖 2D 比例因子，因此在非游戏应用程序中不可能有 hiDPI 支持。因此，建议在非游戏应用程序中禁用 **Allow Hidpi**，以允许操作系统使用其低DPI回退。

### hiDPI 支持

默认情况下，Godot 项目被操作系统视为支持 DPI。这由**“显示”>“窗口”>“Dpi”>“允许Hidpi”**项目设置控制，应尽可能保持启用状态。禁用 DPI 感知可能会破坏 Windows 上的全屏行为。

由于 Godot 项目是 DPI 感知的，因此在高 DPI 显示器上启动时，它们可能会以非常小的窗口大小显示（与屏幕分辨率成比例）。对于游戏来说，解决这个问题的最常见方法是默认设置为全屏。或者，您可以根据屏幕大小在[自动加载](https://docs.godotengine.org/en/stable/tutorials/scripting/singletons_autoload.html#doc-singletons-autoload)的 `_ready()` 函数中设置窗口大小。

为确保 2D 元素在 hiDPI 显示器上不会显得太小：

- 对于游戏，使用 `canvas_items` 或 `viewport` 拉伸模式，以便根据当前窗口大小自动调整 2D 元素的大小。
- 对于非游戏应用程序，使用 `disabled` 的拉伸模式，并在[自动加载](https://docs.godotengine.org/en/stable/tutorials/scripting/singletons_autoload.html#doc-singletons-autoload)的 `_ready()` 函数中将拉伸比例设置为与显示比例因子对应的值。显示比例因子在操作系统的设置中设置，可以使用 [screen_get_scale](https://docs.godotengine.org/en/stable/classes/class_displayserver.html#class-displayserver-method-screen-get-scale) 查询。此方法目前仅在 macOS 上实现。在其他操作系统上，您需要实现一种方法，根据屏幕分辨率猜测显示比例因子（如果需要，可以设置为让用户覆盖）。这是 Godot 编辑器目前使用的方法。

“**允许 Hidpi**”设置仅在 Windows 和 macOS 上有效。它在所有其他平台上都被忽略了。

> **注：**
>
> Godot 编辑器本身始终标记为 DPI 感知。只有在“项目设置”中启用了“**允许 Hidpi**”时，从编辑器运行项目才会感知 DPI。

### 减少下采样时的混叠

如果游戏具有非常高的基本分辨率（例如 3840×2160），则在降采样到相当低的值（如 1280×720）时可能会出现混叠。

为了解决这个问题，您可以在所有 2D 纹理上[启用 mipmaps](https://docs.godotengine.org/en/stable/tutorials/assets_pipeline/importing_images.html#doc-importing-images-mipmaps)。然而，启用 mipmaps 将增加内存使用，这在低端移动设备上可能是一个问题。

### 处理纵横比

一旦考虑了不同分辨率的缩放，请确保您的用户界面也能针对不同的宽高比进行缩放。这可以使用[锚](https://docs.godotengine.org/en/stable/tutorials/ui/size_and_anchors.html#doc-size-and-anchors)和/或[容器](https://docs.godotengine.org/en/stable/tutorials/ui/gui_containers.html#doc-gui-containers)来完成。

### 视场缩放

“3D 摄影机”节点的“**保持纵横比**”属性默认为“**保持高度**”缩放模式（也称为 *Hor+*）。这通常是横向模式下桌面游戏和移动游戏的最佳值，因为宽屏显示器会自动使用更宽的视野。

但是，如果您的 3D 游戏打算在纵向模式下玩，那么使用“**保持宽度**”（也称为 *Vert-*）可能更有意义。这样，纵横比高于 16:9（例如 19:9）的智能手机将使用更高的视野，这在这里更合乎逻辑。

### 使用视口以不同方式缩放 2D 和 3D 元素

使用多个视口节点，可以为各种元素设置不同的比例。例如，您可以使用此功能以低分辨率渲染 3D 世界，同时将 2D 元素保持在本机分辨率。这可以显著提高性能，同时保持 HUD 和其他 2D 元素的清晰。

这是通过仅对二维元素使用根 Viewport 节点，然后创建一个 Viewport 节点来显示三维世界，并使用 SubViewportContainer 或 TextureRect 节点进行显示来实现的。在最终项目中，实际上将有两个视口。在 SubViewportContainer 上使用 TextureRect 的一个好处是它允许启用线性过滤。这在许多情况下使缩放的 3D 视口看起来更好。

有关示例，请参阅 [3D 视口缩放演示](https://github.com/godotengine/godot-demo-projects/tree/master/viewport/3d_scaling)。

### 用户贡献的笔记

在提交评论之前，请阅读[用户贡献笔记政策](https://github.com/godotengine/godot-docs-user-notes/discussions/1)。

**1 个评论** · 1 个回复



[troynall](https://github.com/troynall) [Dec 7, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/275#discussioncomment-11491911)

但是，它的作用并不完全明显；引擎不会尝试将显示器切换到此分辨率。相反，可以将此设置视为“设计大小”，即您在编辑器中使用的区域的大小。此设置直接对应于 2D 编辑器中蓝色矩形的大小

上面的文字很冗长。我认为我的版本（以下文本）更简洁。

此设置定义了“设计尺寸”，即您在编辑器中看到的工作区，由二维编辑器中的蓝色矩形表示。它不会切换显示器分辨率，但会为您的设计建立尺寸

​	[mechalynx](https://github.com/mechalynx) [Dec 8, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/275#discussioncomment-11495853)

​	说实话，我认为长度没有问题。也许它可以被视为冗长的，因为解释某事所需的时间比严格必要的要长，但它符合文本其余部分的流程。

​	我认为它很好地确保了用户不会期望指示的分辨率是全屏分辨率，并且还暗示默认分辨率旨在与编辑器预览相匹配。读者可能会将“设计尺寸”解释解释为编辑器自己会做的事情（例如，当它只是一个初始化值时，编辑器会将分辨率设置为与预览相同的分辨率），但刚才读过之后，我没有得到这样的印象，尽管我已经知道它是如何工作的。因此，从这个意义上讲，这可能是一个有用的默认值，而不是与预览分辨率相对应的动态设置值，这可能会更清楚。

​	另一方面，我觉得你提出的简洁版本更有可能让用户感到困惑，因为它可以被解释为定义设计尺寸，而事实并非如此。我自己读过它，我想我会得到这样的印象，即改变这个高度和宽度实际上会在编辑器内部产生影响，而不是在运行时产生影响（尽管本文的其余部分会清楚地说明它所指的是什么）。

​	我想，如果需要改进，那就是“设计尺寸”应该被描述为一个有用的默认值，没有歧义，并且由于“窗口化”默认值，分辨率不会将显示器切换到该分辨率。这将对 2D 和 3D 渲染产生不同的效果，但我觉得其余的文本可以很好地解释这种差异，因为它提到了 2D 中的像素对齐，而且无论如何场景都会适合 3D 中的分辨率。



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

## 粒子着色器

https://docs.godotengine.org/en/stable/tutorials/shaders/shader_reference/particle_shader.html#particle-shaders

粒子着色器是一种特殊类型的着色器，在绘制对象之前运行。它们用于计算材质属性，如颜色、位置和旋转。它们是使用 CanvasItem 或 Spatial 的任何常规材质绘制的，具体取决于它们是 2D 还是 3D。

粒子着色器是唯一的，因为它们不用于绘制对象本身；它们用于计算粒子属性，然后由 CanvasItem 或 Spatial 着色器使用。它们包含两个处理器函数：`start()` 和 `process()`。

与其他着色器类型不同，粒子着色器保留上一帧输出的数据。因此，粒子着色器可用于在多个帧上发生的复杂效果。

> **注：**
>
> 粒子着色器仅适用于基于 GPU 的粒子节点（[GPUParticles2D](https://docs.godotengine.org/en/stable/classes/class_gpuparticles2d.html#class-gpuparticles2d) 和 [GPUParticles3D](https://docs.godotengine.org/en/stable/classes/class_gpuparticles3d.html#class-gpuparticles3d)）。
>
> 基于 CPU 的粒子节点（[CPUarticles2D](https://docs.godotengine.org/en/stable/classes/class_cpuparticles2d.html#class-cpuparticles2d) 和 [CPUarticles3D](https://docs.godotengine.org/en/stable/classes/class_cpuparticles3d.html#class-cpuparticles3d)）在 GPU 上*渲染*（这意味着它们可以使用自定义 CanvasItem 或 Spatial 着色器），但它们的运动是在 CPU 上*模拟*的。

### 渲染模式

| 渲染模式                | 描述                           |
| ----------------------- | ------------------------------ |
| **keep_data**           | 重新启动时不要清除以前的数据。 |
| **disable_force**       | 禁用吸引子力。                 |
| **disable_velocity**    | 忽略 **VELOCITY** 值。         |
| **collision_use_scale** | 缩放粒子的大小以进行碰撞。     |

### 内置

标记为“in”的值是只读的。标记为“out”的值用于可选写入，不一定包含合理的值。标记为“inout”的值提供了一个合理的默认值，并且可以选择写入。采样器不是写入的对象，也没有标记。

### 全局内置

全局内置在各处可用，包括自定义函数。

| 内置              | 描述                                                         |
| ----------------- | ------------------------------------------------------------ |
| in float **TIME** | 引擎启动后的全局时间（秒）。它每 3600 秒重复一次（可以通过[翻转](https://docs.godotengine.org/en/stable/classes/class_projectsettings.html#class-projectsettings-property-rendering-limits-time-time-rollover-secs)设置进行更改）。它不受 [time_scale](https://docs.godotengine.org/en/stable/classes/class_engine.html#class-engine-property-time-scale) 的影响或暂停。如果你需要一个可以缩放或暂停的 `TIME` 变量，请添加你自己的[全局着色器 uniform](https://docs.godotengine.org/en/stable/tutorials/shaders/shader_reference/shading_language.html#doc-shading-language-global-uniforms)，并在每一帧更新它。 |
| in float **PI**   | 一个 `PI` 常数（`3.141592`）。圆的周长与直径的比值，以及半圈的弧度数。 |
| in float **TAU**  | 一个 `TAU` 常数（`6.283185`）。相当于 `PI * 2` 和整圈弧度。  |
| in float **E**    | 一个 `E` 常数（`2.718281`）。欧拉数和自然对数的底数。        |

### Start 和 Process 内置

这些属性可以从 `start()` 和 `process()` 函数访问。

| 函数                            | 描述                                                         |
| ------------------------------- | ------------------------------------------------------------ |
| in float **LIFETIME**           | 粒子寿命。                                                   |
| in float **DELTA**              | 增量处理时间。                                               |
| in uint **NUMBER**              | 自排放开始以来的唯一编号。                                   |
| in uint **INDEX**               | 颗粒索引（来自总颗粒）。                                     |
| in mat4 **EMISSION_TRANSFORM**  | 发射器变换（用于非局部系统）。                               |
| in uint **RANDOM_SEED**         | 随机种子用作随机的基础。                                     |
| inout bool **ACTIVE**           | 当粒子处于活动状态时为 `true` ，可以设置为 `false`。         |
| inout vec4 **COLOR**            | 粒子颜色，可以写入网格的顶点函数并在其中访问。               |
| inout vec3 **VELOCITY**         | 粒子速度可以修改。                                           |
| inout mat4 **TRANSFORM**        | 粒子变换。                                                   |
| inout vec4 **CUSTOM**           | 自定义粒子数据。可从网格着色器中访问 **INSTANCE_CUSTOM**。   |
| inout float **MASS**            | 粒子质量，用于吸引子。默认情况下等于 `1.0`。                 |
| in vec4 **USERDATAX**           | 可将补充的用户定义数据集成到粒子过程着色器中的矢量。`USERDATAX` 是由数字标识的六个内置组件，`X` 可以是 1 到 6 之间的数字。 |
| in uint **FLAG_EMIT_POSITION**  | 用于在 `emit_subparticle` 函数的最后一个参数上使用的标志，用于为新粒子的变换分配位置。 |
| in uint **FLAG_EMIT_ROT_SCALE** | 用于在 `emit_subparticle` 函数的最后一个参数上使用的标志，用于将旋转和缩放分配给新粒子的变换。 |
| in uint **FLAG_EMIT_VELOCITY**  | 用于在 `emit_subparticle` 函数的最后一个参数上使用的标志，用于为新粒子分配速度。 |
| in uint **FLAG_EMIT_COLOR**     | 用于在 `emit_subparticle` 函数的最后一个参数上使用的标志，为新粒子分配颜色。 |
| in uint **FLAG_EMIT_CUSTOM**    | 用于在 `emit_subparticle` 函数的最后一个参数上使用的标志，用于将自定义数据向量分配给新粒子。 |
| in vec3 **EMITTER_VELOCITY**    | Particles 节点的速度。                                       |
| in float **INTERPOLATE_TO_END** | 粒子节点的 `interp_to_end` 属性的值。                        |
| in uint **AMOUNT_RATIO**        | 粒子节点的 `amount_ratio` 属性的值。                         |

> **注：**
>
> 为了在 StandardMaterial3D 中使用 `COLOR` 变量，请将 `vertex_color_use_as_albedo` 设置为 `true`。在 ShaderMaterial 中，使用 `COLOR` 变量访问它。

### Start 内置

| 内置                          | 描述                                                         |
| ----------------------------- | ------------------------------------------------------------ |
| in bool **RESTART_POSITION**  | 如果粒子重新启动，或者在没有自定义位置的情况下发射（即该粒子是由`emit_subparticle()` 创建的，没有 `FLAG_EMIT_POSITION` 标志），则返回 `true`。 |
| in bool **RESTART_ROT_SCALE** | 如果粒子重新启动，或者在没有自定义旋转或缩放的情况下发射（即该粒子是由 `emit_subparticle()` 创建的，没有 `FLAG_EMIT_ROT_SCALE` 标志），则为 `true`。 |
| in bool **RESTART_VELOCITY**  | 如果粒子重新启动，或者在没有自定义速度的情况下发射（即该粒子是由 `emit_subparticle()` 创建的，没有 `FLAG_EMIT_VELOCITY` 标志），则为 `true`。 |
| in bool **RESTART_COLOR**     | 如果粒子重新启动或发射时没有自定义颜色（即该粒子是由 `emit_subparticle()` 创建的，没有 `FLAG_EMIT_COLOR` 标志），则为 `true`。 |
| in bool **RESTART_CUSTOM**    | 如果粒子重新启动，或者在没有自定义属性的情况下发射（即该粒子是由 `emit_subparticle()` 创建的，没有 `FLAG_EMIT_CUSTOM` 标志），则为 `true`。 |

### Process 内置

| 内置                         | 描述                                                         |
| ---------------------------- | ------------------------------------------------------------ |
| in bool **RESTART**          | 如果当前进程帧是粒子的第一个进程帧，则为 `true`。            |
| in bool **COLLIDED**         | 当粒子与粒子碰撞体碰撞时为 `true`。                          |
| in vec3 **COLLISION_NORMAL** | 上次碰撞的正常现象。如果没有检测到碰撞，则等于 `vec3(0.0)`。 |
| in float **COLLISION_DEPTH** | 上次碰撞的法线长度。如果没有检测到碰撞，则等于 `0.0`。       |
| in vec3 **ATTRACTOR_FORCE**  | 此刻吸引子对该粒子的合力。                                   |

### Process 函数

`emit_subparticle` 是粒子着色器目前唯一支持的自定义函数。它允许用户从子发射器添加具有指定参数的新粒子。新创建的粒子将仅使用与 `flags` 参数匹配的属性。例如，以下代码将发射具有指定位置、速度和颜色但未指定旋转、比例和自定义值的粒子：

```glsl
mat4 custom_transform = mat4(1.0);
custom_transform[3].xyz = vec3(10.5, 0.0, 4.0);
emit_subparticle(custom_transform, vec3(1.0, 0.5, 1.0), vec4(1.0, 0.0, 0.0, 1.0), vec4(1.0), FLAG_EMIT_POSITION | FLAG_EMIT_VELOCITY | FLAG_EMIT_COLOR);
```

| 函数                                                         | 描述                 |
| ------------------------------------------------------------ | -------------------- |
| bool **emit_subparticle** (mat4 xform, vec3 velocity, vec4 color, vec4 custom, uint flags) | 从子发射器发射粒子。 |



## 天空着色器

https://docs.godotengine.org/en/stable/tutorials/shaders/shader_reference/sky_shader.html#sky-shaders

天空着色器是一种特殊类型的着色器，用于绘制天空背景和更新用于基于图像的照明（IBL）的辐射立方体图。天空着色器只有一个处理函数，即 `sky()` 函数。

有三个地方使用了天空着色器。

- 首先，当您选择在场景中使用天空作为背景时，天空着色器用于绘制天空。
- 其次，当使用天空作为环境色或反射时，天空着色器用于更新辐射立方体贴图。
- 第三，天空着色器用于绘制低分辨率子通道，这些子通道可用于高分辨率背景或立方体贴图通道。

总的来说，这意味着天空着色器每帧最多可以运行六次，但在实践中，它会比这少得多，因为辐射立方体贴图不需要每帧都更新，也不会使用所有子路径。通过检查 `AT_*_PASS` 布尔值，可以根据调用着色器的位置更改着色器的行为。例如：

```glsl
shader_type sky;

void sky() {
    if (AT_CUBEMAP_PASS) {
        // Sets the radiance cubemap to a nice shade of blue instead of doing
        // expensive sky calculations
        COLOR = vec3(0.2, 0.6, 1.0);
    } else {
        // Do expensive sky calculations for background sky only
        COLOR = get_sky_color(EYEDIR);
    }
}
```

使用天空着色器绘制背景时，将对屏幕上的所有非遮挡片段调用着色器。但是，对于背景的子通道，将对子通道的每个像素调用着色器。

当使用天空着色器更新辐射立方体贴图时，立方体贴图中的每个像素都将调用天空着色器。另一方面，只有在需要更新辐射立方体贴图时才会调用着色器。当任何着色器参数更新时，都需要更新辐射立方体贴图。例如，如果着色器中使用了 `TIME`，则辐射立方体贴图将更新每一帧。以下更改列表强制更新辐射立方体贴图：

- `TIME` 被使用。
- 使用 `POSITION`，相机位置会发生变化。
- 如果使用了任何 `LIGHTX_*` 属性，并且任何 [DirectionalLight3D](https://docs.godotengine.org/en/stable/classes/class_directionallight3d.html#class-directionallight3d) 发生了变化。
- 如果着色器中更改了任何均匀性。
- 如果调整了屏幕大小并且使用了其中任何一个子页面。

尽量避免不必要地更新辐射立方体贴图。如果您确实需要在每一帧更新辐射立方体贴图，请确保您的[天空处理模式](https://docs.godotengine.org/en/stable/classes/class_sky.html#class-sky-property-process-mode)设置为[实时](https://docs.godotengine.org/en/stable/classes/class_sky.html#class-sky-constant-process-mode-realtime)。

请注意，[处理模式](https://docs.godotengine.org/en/stable/classes/class_sky.html#class-sky-property-process-mode)仅影响辐射立方体贴图的渲染。始终通过为每个像素调用片段着色器来渲染可见天空。对于复杂的片段着色器，这可能会导致较高的渲染开销。如果天空是静态的（满足上述条件）或变化缓慢，则不需要每帧都运行完整的片段着色器。通过将整个天空渲染到辐射立方体贴图中，并在渲染可见天空时从该立方体贴图中读取，可以避免这种情况。对于完全静态的天空，这意味着只需要渲染一次。

以下代码将整个天空渲染到辐射立方体贴图中，并从该立方体贴图中读取以显示可见天空：

```glsl
shader_type sky;

void sky() {
    if (AT_CUBEMAP_PASS) {
        vec3 dir = EYEDIR;

        vec4 col = vec4(0.0);

        // Complex color calculation

        COLOR = col.xyz;
        ALPHA = 1.0;
    } else {
        COLOR = texture(RADIANCE, EYEDIR).rgb;
    }
}
```

这样，复杂的计算只发生在立方体贴图过程中，可以通过设置天空的处理模式和辐射大小来优化立方体贴图过程，以在性能和视觉保真度之间达到所需的平衡。

### 渲染模式

子通道允许您以较低的分辨率进行更昂贵的计算，以加快着色器的速度。例如，以下代码以比天空其他部分更低的分辨率渲染云：

```glsl
shader_type sky;
render_mode use_half_res_pass;

void sky() {
    if (AT_HALF_RES_PASS) {
        // Run cloud calculation for 1/4 of the pixels
        vec4 color = generate_clouds(EYEDIR);
        COLOR = color.rgb;
        ALPHA = color.a;
    } else {
        // At full resolution pass, blend sky and clouds together
        vec3 color = generate_sky(EYEDIR);
        COLOR = color + HALF_RES_COLOR.rgb * HALF_RES_COLOR.a;
    }
}
```

| 渲染模式                 | 描述                                     |
| ------------------------ | ---------------------------------------- |
| **use_half_res_pass**    | 允许着色器写入和访问半分辨率过程。       |
| **use_quarter_res_pass** | 允许着色器写入和访问四分之一分辨率通道。 |
| **disable_fog**          | 如果使用，雾不会影响天空。               |

### 内置

标记为“in”的值是只读的。标记为“out”的值用于可选写入，不一定包含合理的值。采样器无法写入，因此没有标记。

### 全局内置

全局内置在所有地方可用，包括在自定义函数中。

有 4 个 `LIGHTX` 光，分别为 `LIGHT0`、`LIGHT1`、`LIGHT2` 和 `LIGHT3`。

| 内置                            | 描述                                                         |
| ------------------------------- | ------------------------------------------------------------ |
| in float **TIME**               | 引擎启动后的全局时间（秒）。它每 3600 秒重复一次（可以通过[翻转](https://docs.godotengine.org/en/stable/classes/class_projectsettings.html#class-projectsettings-property-rendering-limits-time-time-rollover-secs)设置进行更改）。它不受 [time_scale](https://docs.godotengine.org/en/stable/classes/class_engine.html#class-engine-property-time-scale) 的影响或暂停。如果你需要一个可以缩放或暂停的 `TIME` 变量，请添加你自己的[全局着色器 uniform](https://docs.godotengine.org/en/stable/tutorials/shaders/shader_reference/shading_language.html#doc-shading-language-global-uniforms)，并在每一帧更新它。 |
| in vec3 **POSITION**            | 相机在世界空间中的位置                                       |
| samplerCube **RADIANCE**        | 辐射立方体图。只能在后台通过时读取。检查 `!AT_CUBEMAP_PASS` 使用前。 |
| in bool **AT_HALF_RES_PASS**    | 当前渲染为半分辨率通过。                                     |
| in bool **AT_QUARTER_RES_PASS** | 当前渲染为四分之一分辨率。                                   |
| in bool **AT_CUBEMAP_PASS**     | 当前正在渲染为辐射立方体贴图。                               |
| in bool **LIGHTX_ENABLED**      | `LightX` 在场景中可见。如果为 `false`，则其他灯光属性可能是垃圾。 |
| in float **LIGHTX_ENERGY**      | `LIGHTX` 的能量倍增器。                                      |
| in vec3 **LIGHTX_DIRECTION**    | `LIGHTX` 所面向的方向。                                      |
| in vec3 **LIGHTX_COLOR**        | `LIGHTX` 的颜色。                                            |
| in float **LIGHTX_SIZE**        | 天空中 `LIGHTX` 的角直径。以弧度表示。作为参考，太阳离地球约为 .0087 弧度（0.5 度）。 |
| in float **PI**                 | 一个 `PI` 常数（`3.141592`）。圆的周长与直径的比值，以及半圈的弧度数。 |
| in float **TAU**                | 一个 `TAU` 常数（`6.283185`）。相当于 `PI * 2` 和整圈弧度。  |
| in float **E**                  | 一个 `E` 常数（`2.718281`）。欧拉数和自然对数的底数。        |

### 天空内置

| 内置                          | 描述                                                   |
| ----------------------------- | ------------------------------------------------------ |
| in vec3 **EYEDIR**            | 当前像素的归一化方向。将此作为程序效果的基本方向。     |
| in vec2 **SCREEN_UV**         | 当前像素的屏幕 UV 坐标。用于将纹理映射到全屏。         |
| in vec2 **SKY_COORDS**        | 球体 UV。用于将全景纹理映射到天空。                    |
| in vec4 **HALF_RES_COLOR**    | 半分辨率通过时对应像素的颜色值。使用线性滤波器。       |
| in vec4 **QUARTER_RES_COLOR** | 四分之一分辨率通过时对应像素的颜色值。使用线性滤波器。 |
| out vec3 **COLOR**            | 输出颜色。                                             |
| out float **ALPHA**           | 输出 alpha 值，只能在子页面中使用。                    |
| out vec4 **FOG**              |                                                        |



## 雾着色器

https://docs.godotengine.org/en/stable/tutorials/shaders/shader_reference/fog_shader.html#fog-shaders

雾着色器用于定义如何在给定区域的场景中添加（或减少）雾。雾着色器始终与 [FogVolumes](https://docs.godotengine.org/en/stable/classes/class_fogvolume.html#class-fogvolume) 和体积雾一起使用。雾着色器只有一个处理函数，即 `fog()` 函数。

雾着色器的分辨率取决于体积雾冻结网格的分辨率。因此，雾着色器可以添加的细节级别取决于 [FogVolume](https://docs.godotengine.org/en/stable/classes/class_fogvolume.html#class-fogvolume) 与摄影机的距离。

雾着色器是一种特殊形式的计算着色器，对于关联 [FogVolume](https://docs.godotengine.org/en/stable/classes/class_fogvolume.html#class-fogvolume) 的轴对齐边界框所接触的每个 froxel，都会调用一次。这意味着几乎不会接触到给定 [FogVolume](https://docs.godotengine.org/en/stable/classes/class_fogvolume.html#class-fogvolume) 的 froxels 仍将被使用。

### 内置

标记为“in”的值是只读的。标记为“out”的值用于可选写入，不一定包含合理的值。采样器无法写入，因此没有标记。

### 全局内置

全局内置功能无处不在，包括在自定义功能中。

| 内置              | 描述                                                         |
| ----------------- | ------------------------------------------------------------ |
| in float **TIME** | 引擎启动后的全球时间（秒）。它每 3600 秒重复一次（可以通过[翻转](https://docs.godotengine.org/en/stable/classes/class_projectsettings.html#class-projectsettings-property-rendering-limits-time-time-rollover-secs)设置进行更改）。它不受 [time_scale](https://docs.godotengine.org/en/stable/classes/class_engine.html#class-engine-property-time-scale) 的影响或暂停。如果你需要一个可以缩放或暂停的 `TIME` 变量，请添加你自己的[全局着色器 uniform](https://docs.godotengine.org/en/stable/tutorials/shaders/shader_reference/shading_language.html#doc-shading-language-global-uniforms)，并在每一帧更新它。 |
| in float **PI**   | 一个 `PI` 常数（`3.141592`）。圆的周长与直径的比值，以及半圈的弧度数。 |
| in float **TAU**  | 一个 `TAU` 常数（`6.283185`）。相当于 `PI * 2` 和整圈弧度。  |
| in float **E**    | 一个 `E` 常数（`2.718281`）。欧拉数和自然对数的底数。        |

### 雾内置

雾体积的所有输出值都相互重叠。这允许有效地渲染 [FogVolumes](https://docs.godotengine.org/en/stable/classes/class_fogvolume.html#class-fogvolume)，因为它们可以一次绘制。

| 内置                        | 描述                                                         |
| --------------------------- | ------------------------------------------------------------ |
| in vec3 **WORLD_POSITION**  | 当前 froxel 细胞在世界空间中的位置。                         |
| in vec3 **OBJECT_POSITION** | 当前在世界空间中 [FogVolume](https://docs.godotengine.org/en/stable/classes/class_fogvolume.html#class-fogvolume) 的中心位置。 |
| in vec3 **UVW**             | 三维 uv，用于将 3D 纹理映射到当前 [FogVolume](https://docs.godotengine.org/en/stable/classes/class_fogvolume.html#class-fogvolume)。 |
| in vec3 **SIZE**            | 当前 [FogVolume](https://docs.godotengine.org/en/stable/classes/class_fogvolume.html#class-fogvolume) 的 [形状](https://docs.godotengine.org/en/stable/classes/class_fogvolume.html#class-fogvolume-property-shape) 具有大小时的大小。 |
| in vec3 **SDF**             | [FogVolume](https://docs.godotengine.org/en/stable/classes/class_fogvolume.html#class-fogvolume) 表面的符号距离场。如果内部体积为负，则为正。 |
| out vec3 **ALBEDO**         | 输出基色值，与光相互作用以产生最终颜色。仅在使用时写入雾量。 |
| out float **DENSITY**       | 输出密度值。可以为负数，以允许从另一个卷中减去一个卷。雾着色器必须使用“密度”才能写入任何内容。 |
| out vec3 **EMISSION**       | 输出发射颜色值，在光通过过程中添加到颜色中以产生最终颜色。仅在使用时写入雾量。 |



## 你的第一个 2D 着色器

https://docs.godotengine.org/en/stable/tutorials/shaders/your_first_shader/your_first_2d_shader.html#your-first-2d-shader

### 引言

着色器是在 GPU 上执行的特殊程序，用于渲染图形。所有现代渲染都是使用着色器完成的。有关着色器的更详细描述，请参阅[着色器是什么](https://docs.godotengine.org/en/stable/tutorials/shaders/introduction_to_shaders.html#doc-introduction-to-shaders)。

本教程将通过指导您编写具有顶点和片段函数的着色器的过程，重点介绍编写着色器程序的实际方面。本教程面向着色器的绝对初学者。

> **注：**
>
> 如果您有编写着色器的经验，并且只是想了解着色器在 Godot 中的工作原理，请参阅[《着色参考》](https://docs.godotengine.org/en/stable/tutorials/shaders/shader_reference/index.html#toc-shading-reference)。

### 设置

[CanvasItem 着色器](https://docs.godotengine.org/en/stable/tutorials/shaders/shader_reference/canvas_item_shader.html#doc-canvas-item-shader)用于绘制 Godot 中的所有 2D 对象，而 [Spatial](https://docs.godotengine.org/en/stable/tutorials/shaders/shader_reference/spatial_shader.html#doc-spatial-shader) 着色器用于绘制所有 3D 对象。

为了使用着色器，它必须附着在必须附着到对象的 [Material](https://docs.godotengine.org/en/stable/classes/class_material.html#class-material) 内。Material 是一种 [Resource](https://docs.godotengine.org/en/stable/tutorials/scripting/resources.html#doc-resources)。要使用相同的材质绘制多个对象，必须将材质附着到每个对象上。

从 [CanvasItem](https://docs.godotengine.org/en/stable/classes/class_canvasitem.html#class-canvasitem) 派生的所有对象都具有材质属性。这包括所有 [GUI 元素](https://docs.godotengine.org/en/stable/classes/class_control.html#class-control)、[Sprite2Ds](https://docs.godotengine.org/en/stable/classes/class_sprite2d.html#class-sprite2d)、[TileMapLayers](https://docs.godotengine.org/en/stable/classes/class_tilemaplayer.html#class-tilemaplayer)、[MeshInstance2Ds](https://docs.godotengine.org/en/stable/classes/class_meshinstance2d.html#class-meshinstance2d) 等。他们还可以选择继承父母的材料。如果您有大量要使用相同材质的节点，这可能很有用。

首先，创建一个 Sprite2D 节点。[您可以使用任何 CanvasItem](https://docs.godotengine.org/en/stable/tutorials/2d/custom_drawing_in_2d.html#doc-custom-drawing-in-2d)，只要它是在画布上绘制的，因此在本教程中，我们将使用 Sprite2D，因为它是最容易开始绘制的 CanvasItem。

在检查器中，单击“纹理”旁边的“[空]”，选择“加载”，然后选择“icon.svg”。对于新项目，这是 Godot 图标。现在，您应该可以在视口中看到该图标。

接下来，在检查器的 CanvasItem 部分下向下看，单击“材质”旁边的，然后选择“新建着色器材质”。这将创建一个新的材质资源。单击出现的球体。Godot 目前不知道您是在编写 CanvasItem 着色器还是空间着色器，它会预览空间着色器的输出。因此，您看到的是默认空间着色器的输出。

单击“着色器”旁边的，然后选择“新建着色器”。最后，单击刚刚创建的着色器，着色器编辑器将打开。现在，您已准备好开始编写第一个着色器。

### 您的第一个 CanvasItem 着色器

在 Godot 中，所有着色器都以一行开头，指定它们是哪种类型的着色器。它使用以下格式：

```glsl
shader_type canvas_item;
```

因为我们正在编写 CanvasItem 着色器，所以我们在第一行中指定 `canvas_item`。我们所有的代码都将位于此声明之下。

这一行告诉引擎要为您提供哪些内置变量和功能。

在 Godot 中，您可以覆盖三个函数来控制着色器的操作方式；`vertex`、`fragment` 和 `light`。本教程将指导您编写具有顶点和片段函数的着色器。光函数比顶点和片段函数复杂得多，因此这里将不涉及。

### 您的第一个片段函数

片段函数对 Sprite2D 中的每个像素运行，并确定该像素应该是什么颜色。

它们仅限于 Sprite2D 覆盖的像素，这意味着您不能使用它们来创建 Sprite2D 周围的轮廓。

最基本的片段函数除了为每个像素分配一种颜色外，什么也不做。

我们通过将 `vec4` 写入内置变量 `COLOR` 来实现。`vec4` 是构造具有 4 个数字的向量的简写。有关向量的更多信息，请参阅[向量数学教程](https://docs.godotengine.org/en/stable/tutorials/math/vector_math.html#doc-vector-math)。`COLOR` 既是片段函数的输入变量，也是其最终输出。

```glsl
void fragment(){
  COLOR = vec4(0.4, 0.6, 0.9, 1.0);
}
```

![../../../_images/blue-box.png](https://docs.godotengine.org/en/stable/_images/blue-box.png)

祝贺！你完成了。您已经在Godot中成功编写了第一个着色器。

现在让我们把事情弄得更复杂。

您可以使用片段函数的许多输入来计算 `COLOR`。`UV` 就是其中之一。UV 坐标在 Sprite2D 中指定（您不知道！），它们告诉着色器从网格每个部分的纹理中读取的位置。

在片段函数中，您只能从 `UV` 读取，但您可以在其他函数中使用它或直接为 `COLOR` 赋值。

`UV` 从左到右和从上到下在 0-1 之间变化。

![../../../_images/iconuv.png](https://docs.godotengine.org/en/stable/_images/iconuv.png)

```glsl
void fragment() {
  COLOR = vec4(UV, 0.5, 1.0);
}
```

![../../../_images/UV.png](https://docs.godotengine.org/en/stable/_images/UV.png)

#### 使用内置的 `TEXTURE`

默认片段函数从 Sprite2D 纹理集读取并显示它。

当你想在 Sprite2D 中调整颜色时，你可以像下面的代码一样手动调整纹理的颜色。

```glsl
void fragment(){
  // This shader will result in a blue-tinted icon
  COLOR.b = 1.0;
}
```

某些节点，如 Sprite2Ds，有一个专用的纹理变量，可以在着色器中使用 `TEXTURE` 访问。如果要使用 Sprite2D 纹理与其他颜色组合，可以使用 `UV` 和 `texture` 函数来访问此变量。使用它们用纹理重绘 Sprite2D。

```glsl
void fragment(){
  COLOR = texture(TEXTURE, UV); // Read from texture again.
  COLOR.b = 1.0; //set blue channel to 1.0
}
```

![../../../_images/blue-tex.png](https://docs.godotengine.org/en/stable/_images/blue-tex.png)

#### uniform 输入

uniform 输入用于将数据传递到着色器中，该着色器在整个着色器中都是相同的。

您可以通过在着色器顶部定义 uniform 来使用，如下所示：

```glsl
uniform float size;
```

有关用法的更多信息，请参阅[着色语言文档](https://docs.godotengine.org/en/stable/tutorials/shaders/shader_reference/shading_language.html#doc-shading-language)。

添加一个 uniform 以更改 Sprite2D 中的蓝色量。

```glsl
uniform float blue = 1.0; // you can assign a default value to uniforms

void fragment(){
  COLOR = texture(TEXTURE, UV); // Read from texture
  COLOR.b = blue;
}
```

现在，您可以从编辑器中更改 Sprite2D 中的蓝色量。回头看看创建着色器的检查器。您应该看到一个名为“着色器参数”的部分。展开该部分，您将看到您刚才声明的 uniform。如果在编辑器中更改该值，它将覆盖在着色器中提供的默认值。

#### 从代码中与着色器交互

您可以使用在节点的材料资源上调用的函数 `set_shader_parameter()` 从代码中更改 uniform。使用 Sprite2D 节点，可以使用以下代码设置 `blue` uniform。

```gdscript
var blue_value = 1.0
material.set_shader_parameter("blue", blue_value)
```

```c#
var blueValue = 1.0;
((ShaderMaterial)Material).SetShaderParameter("blue", blueValue);
```

请注意，uniform 的名称是一个字符串。该字符串必须与着色器中的书写方式完全匹配，包括拼写和大小写。

### 你的第一个顶点函数

现在我们有了片段函数，让我们编写一个顶点函数。

使用顶点函数计算每个顶点在屏幕上的最终位置。

顶点函数中最重要的变量是 `VERTEX`。最初，它指定模型中的顶点坐标，但您也可以写入它以确定实际绘制这些顶点的位置。`VERTEX` 是一个最初在局部空间中呈现的 `vec2`（即不相对于摄影机、视口或父节点）。

您可以通过直接添加到 `VERTEX` 来偏移顶点。

```glsl
void vertex() {
  VERTEX += vec2(10.0, 0.0);
}
```

结合 `TIME` 内置变量，这可用于基本动画。

```glsl
void vertex() {
  // Animate Sprite2D moving in big circle around its location
  VERTEX += vec2(cos(TIME)*100.0, sin(TIME)*100.0);
}
```

### 结论

着色器的核心是执行您到目前为止看到的操作，即计算 `VERTEX` 和 `COLOR`。你可以想出更复杂的数学策略来给这些变量赋值。

为了获得灵感，请查看一些更高级的着色器教程，并查看其他网站，如 [Shadertoy](https://www.shadertoy.com/results?query=&sort=popular&from=10&num=4) 和 [The Book of Shaders](https://thebookofshaders.com/)。

### 用户贡献的笔记

在提交评论之前，请阅读[用户贡献笔记政策](https://github.com/godotengine/godot-docs-user-notes/discussions/1)。

**1 个评论**



[vanBerloDevelopments](https://github.com/vanBerloDevelopments) [Oct 8, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/188#discussioncomment-10880147)

以下是我在学习着色器以及如何接近它们时遇到的一些困难。希望它能帮助别人！

1. 着色器的外部边界

   顶点着色器控制可以使用顶点操纵的区域。顶点之外的所有内容都不会在屏幕上绘制。

2. `texture(TEXTURE, UV)` 功能不会“创建”纹理。
   这是开始时常见的误解。纹理函数从您在第一个参数中指定的纹理中采样纹理数据，在本例中为 `TEXTURE`（即其原始纹理）。然后将颜色数据和阿尔法通道作为向量 4 数据类型返回。

   例子：

   ```glsl
   vec4 fragment_color_data = texture(TEXTURE, UV).rgba;
   ```

3. 使用 UV 贴图缩放纹理：

   - 要“放大”，请将 UV 坐标乘以小于 1 的值。
   - 要“缩小”比例，请将 UV 坐标乘以大于 1 的值。

   例子：

   ```glsl
   vec4 scaled_up_sample = texture(TEXTURE, vec2(UV.x * 0.9, UV.y));
   ```

   有关视觉示例，请参阅[此视频教程](https://www.youtube.com/watch?v=tbcgEIvJlrI&t=109s)。

4. 使用 UV 贴图移动（平移）纹理：

   - 向左平移：将正值添加到 UV.x
   - 要向右平移：从 UV.x 中减去正值
   - 向上转换：将正值添加到 UV.y
   - 向下平移：从 UV.y 中减去正值

   例子：

   ```glsl
   vec4 sampled_to_left = texture(TEXTURE, vec2(UV.x + 0.1, UV.y));
   ```

   有关视觉示例，请参阅[此视频教程](https://www.youtube.com/watch?v=tbcgEIvJlrI&t=109s)。

快乐着色！
亲切问候 Sam，
godot2dshaders.com 的作者



## 您的第一个 3D 着色器

https://docs.godotengine.org/en/stable/tutorials/shaders/your_first_shader/your_first_3d_shader.html#your-first-3d-shader

您已决定开始编写自己的自定义空间着色器。也许你在网上看到了一个很酷的着色器技巧，或者你发现 [StandardMaterial3D](https://docs.godotengine.org/en/stable/classes/class_standardmaterial3d.html#class-standardmaterial3d) 并不能完全满足你的需求。不管怎样，你已经决定写自己的，现在你需要弄清楚从哪里开始。

本教程将解释如何编写空间着色器，并将涵盖比 [CanvasItem](https://docs.godotengine.org/en/stable/tutorials/shaders/your_first_shader/your_first_2d_shader.html#doc-your-first-canvasitem-shader) 教程更多的主题。

空间着色器比 CanvasItem 着色器具有更多的内置功能。对空间着色器的期望是，Godot 已经为常见用例提供了功能，用户在着色器中需要做的就是设置适当的参数。对于 PBR（基于物理的渲染）工作流来说尤其如此。

这是一个由两部分组成的教程。在第一部分中，我们将使用顶点函数中高度图的顶点位移来创建地形。在[第二部分](https://docs.godotengine.org/en/stable/tutorials/shaders/your_first_shader/your_second_3d_shader.html#doc-your-second-spatial-shader)中，我们将学习本教程中的概念，并通过编写海水着色器在片段着色器中设置自定义材质。

> **注：**
>
> 本教程假设您具备一些基本的着色器知识，如类型（`vec2`、`float`、`sampler2D`）和函数。如果你对这些概念感到不舒服，最好在完成本教程之前从[《着色器之书》](https://thebookofshaders.com/)中得到一个温和的介绍。

### 在哪里分配我的材质

在三维中，对象是使用 [Meshes](https://docs.godotengine.org/en/stable/classes/class_mesh.html#class-mesh) 绘制的。网格是一种资源类型，它以称为“曲面”的单位存储几何体（对象的形状）和材质（颜色以及对象对光的反应）。网格可以有多个曲面，也可以只有一个曲面。通常，您会从另一个程序（例如 Blender）导入网格。但是 Godot 也有一些 [PrimitiveMeshes](https://docs.godotengine.org/en/stable/classes/class_primitivemesh.html#class-primitivemesh)，允许您在不导入网格的情况下向场景添加基本几何体。

有多种节点类型可用于绘制网格。主要的是 [MeshInstance3D](https://docs.godotengine.org/en/stable/classes/class_meshinstance3d.html#class-meshinstance3d)，但您也可以使用 [GPUParticles3D](https://docs.godotengine.org/en/stable/classes/class_gpuparticles3d.html#class-gpuparticles3d)、[MultiMeshes](https://docs.godotengine.org/en/stable/classes/class_multimesh.html#class-multimesh)（使用 [MultiMeshInstance3D](https://docs.godotengine.org/en/stable/classes/class_multimeshinstance3d.html#class-multimeshinstance3d)）或其他。

通常，材质与网格中的给定曲面相关联，但某些节点（如 MeshInstance3D）允许您覆盖特定曲面或所有曲面的材质。

如果在曲面或网格本身上设置材质，则共享该网格的所有 MeshInstance3D 都将共享该材质。但是，如果你想在多个网格实例中重用相同的网格，但每个实例都有不同的材质，那么你应该在 MeshInstance3D 上设置材质。

在本教程中，我们将在网格本身上设置材质，而不是利用 MeshInstance3D 覆盖材质的能力。

### 设置

将新的 [MeshInstance3D](https://docs.godotengine.org/en/stable/classes/class_meshinstance3d.html#class-meshinstance3d) 节点添加到场景中。

在“网格”旁边的检查器选项卡中，单击“[空]”，然后选择“新建平面网格”。然后单击出现的平面图像。

这将为我们的场景添加一个 [PlaneMesh](https://docs.godotengine.org/en/stable/classes/class_planemesh.html#class-planemesh)。

然后，在视口中，单击左上角的“透视”按钮。将出现一个菜单。菜单中间是用于显示场景的选项。选择“显示线框”。

这将使您能够看到构成平面的三角形。

![../../../_images/plane.png](https://docs.godotengine.org/en/stable/_images/plane.png)

现在将平面网格的“`Subdivide Width`”和“`Subdivide Depth`”设置为 `32`。

![../../../_images/plane-sub-set.webp](https://docs.godotengine.org/en/stable/_images/plane-sub-set.webp)

您可以看到 [MeshInstance3D](https://docs.godotengine.org/en/stable/classes/class_meshinstance3d.html#class-meshinstance3d) 中现在有更多的三角形。这将为我们提供更多的顶点，从而允许我们添加更多的细节。

![../../../_images/plane-sub.png](https://docs.godotengine.org/en/stable/_images/plane-sub.png)

与 PlaneMesh 一样，[PrimitiveMeshes](https://docs.godotengine.org/en/stable/classes/class_primitivemesh.html#class-primitivemesh) 只有一个表面，因此只有一个曲面，而不是一系列材质。单击“材质”旁边的“[空]”，然后选择“新建着色器材质”。然后单击出现的球体。

现在单击“着色器”旁边的“[空]”，然后选择“新建着色器”。

着色器编辑器现在应该弹出，您已经准备好开始编写第一个Spatial着色器了！

### 着色魔法

![../../../_images/shader-editor.webp](https://docs.godotengine.org/en/stable/_images/shader-editor.webp)

新着色器已经使用 `shader_type` 变量和 `fragment()` 函数生成。Godot 着色器需要的第一件事是声明它们是什么类型的着色器。在这种情况下，`shader_type` 被设置为 `spatial`，因为这是一个空间着色器。

```glsl
shader_type spatial;
```

现在忽略 `fragment()` 函数并定义 `vertex()` 函数。`vertex()` 函数确定 [MeshInstance3D](https://docs.godotengine.org/en/stable/classes/class_meshinstance3d.html#class-meshinstance3d) 的顶点在最终场景中的显示位置。我们将使用它来偏移每个顶点的高度，使我们的平面看起来像一个小地形。

我们这样定义顶点着色器：

```glsl
void vertex() {

}
```

在 `vertex()` 函数中没有任何内容的情况下，Godot 将使用其默认顶点着色器。我们可以通过添加一行轻松地开始进行更改：

```glsl
void vertex() {
  VERTEX.y += cos(VERTEX.x) * sin(VERTEX.z);
}
```

添加这一行，你应该会得到一个像下面这样的图像。

![../../../_images/cos.png](https://docs.godotengine.org/en/stable/_images/cos.png)

好吧，让我们打开这个。`VERTEX` 的 `y` 值正在增加。我们将 `VERTEX` 的 `x` 和 `z` 分量作为参数传递给 `cos` 和 `sin`；这使我们在 `x` 和 `z` 轴上呈现出波浪状的外观。

我们想要实现的是小山的外观；毕竟。因为 `cos` 和 `sin` 看起来已经有点像山了。我们通过将输入缩放到 `cos` 和 `sin` 函数来实现。

```glsl
void vertex() {
  VERTEX.y += cos(VERTEX.x * 4.0) * sin(VERTEX.z * 4.0);
}
```

![../../../_images/cos4.png](https://docs.godotengine.org/en/stable/_images/cos4.png)

这看起来更好，但它仍然太尖锐和重复，让我们让它更有趣一点。

### 噪声高度图

噪波是一种非常流行的伪造地形外观的工具。把它想象成类似于余弦函数，其中你有重复的山丘，除了有噪音，每个山丘都有不同的高度。

Godot 提供 [NoiseTexture2D](https://docs.godotengine.org/en/stable/classes/class_noisetexture2d.html#class-noisetexture2d) 资源，用于生成可以从着色器访问的噪波纹理。

要访问着色器中的纹理，请在着色器顶部附近的 `vertex()` 函数外添加以下代码。

```glsl
uniform sampler2D noise;
```

这将允许您将噪波纹理发送到着色器。现在看看你材质下面的检视栏。您应该看到一个名为“着色器参数”的部分。如果你打开它，你会看到一个名为“noise”的部分。

单击它旁边的“[空]”，然后选择“新建 NoiseTexture2D”。然后在 [NoiseTexture2D](https://docs.godotengine.org/en/stable/classes/class_noisetexture2d.html#class-noisetexture2d) 中，单击“Noise”旁边的位置，然后选择“新建 FastNoiseSite”。

> **注：**
>
> NoiseTexture2D 使用 [FastNoiseSite](https://docs.godotengine.org/en/stable/classes/class_fastnoiselite.html#class-fastnoiselite) 生成高度图。

一旦你设置好了，它应该看起来像这样。

![../../../_images/noise-set.webp](https://docs.godotengine.org/en/stable/_images/noise-set.webp)

现在，使用 `texture()` 函数访问噪波纹理。`texture()` 将纹理作为第一个参数，将纹理上位置的 `vec2` 作为第二个参数。我们使用 `VERTEX` 的 `x` 和 `z` 通道来确定在纹理上查找的位置。请注意，PlaneMesh 坐标在 [-1,1] 范围内（大小为 2），而纹理坐标在 [0,1] 范围内，因此为了归一化，我们将 PlaneMesh 的大小除以 2.0，再加上 0.5。`texture()` 返回该位置 `r, g, b, a` 通道的 `vec4`。由于噪波纹理是灰度的，所有值都是相同的，因此我们可以使用任何一个通道作为高度。在这种情况下，我们将使用 `r` 或 `x` 通道。

```glsl
void vertex() {
  float height = texture(noise, VERTEX.xz / 2.0 + 0.5).x;
  VERTEX.y += height;
}
```

注意：`xyzw` 与 GLSL 中的 `rgba` 相同，因此我们可以使用 `texture().r` 来代替上面的 `texture().x`。有关更多详细信息，请参阅 [OpenGL 文档](https://www.khronos.org/opengl/wiki/Data_Type_(GLSL)#Vectors)。

使用此代码，您可以看到纹理创建了随机外观的山丘。

![../../../_images/noise.png](https://docs.godotengine.org/en/stable/_images/noise.png)

现在它太尖了，我们想稍微软化一下山丘。为此，我们将使用 uniform。您已经使用了上面的 uniform 来传递噪波纹理，现在让我们学习它们是如何工作的。

### uniforms

uniform 变量允许您将游戏中的数据传递到着色器中。它们对于控制着色器效果非常有用。uniform 几乎可以是着色器中可以使用的任何数据类型。要使用 uniform，您可以在[着色器](https://docs.godotengine.org/en/stable/classes/class_shader.html#class-shader)中使用关键字 `uniform` 声明它。

让我们制作一个可以改变地形高度的 uniform。

```glsl
uniform float height_scale = 0.5;
```

Godot 允许您使用值初始化 uniform；这里，`height_scale` 设置为 `0.5`。您可以通过在与着色器对应的材质上调用函数 `set_shader_parameter()`，从 GDScript 中设置 uniform。从 GDScript 传递的值优先于用于在着色器中初始化它的值。

```gdscript
# called from the MeshInstance3D
mesh.material.set_shader_parameter("height_scale", 0.5)
```

> **注：**
>
> 在基于 Spatial 的节点中更改 uniform 与基于 CanvasItem 的节点不同。在这里，我们在 PlaneMesh 资源中设置材质。在其他网格资源中，您可能需要首先通过调用 `surface_get_material()` 来访问材质。在 MeshInstance3D 中，您可以使用 `get_surface_material()` 或 `material_override` 访问材质。

请记住，传递给 `set_shader_parameter()` 的字符串必须与[着色器](https://docs.godotengine.org/en/stable/classes/class_shader.html#class-shader)中 uniform 变量的名称匹配。您可以在[着色器](https://docs.godotengine.org/en/stable/classes/class_shader.html#class-shader)内的任何位置使用 uniform 变量。在这里，我们将使用它来设置高度值，而不是任意乘以 `0.5`。

```glsl
VERTEX.y += height * height_scale;
```

现在看起来好多了。

![../../../_images/noise-low.png](https://docs.godotengine.org/en/stable/_images/noise-low.png)

使用 uniforms，我们甚至可以更改每一帧的值，以设置地形高度的动画。结合 [Tweens](https://docs.godotengine.org/en/stable/classes/class_tween.html#class-tween)，这对动画特别有用。

### 与光相互作用

首先，关闭线框。为此，再次单击视口左上角的“透视”，然后选择“显示法线”。此外，在3D场景工具栏中，关闭预览日光。

![../../../_images/normal.png](https://docs.godotengine.org/en/stable/_images/normal.png)

请注意网格颜色是如何变平的。这是因为它的照明是平的。让我们加一盏灯！

首先，我们将在场景中添加 [OmniLight3D](https://docs.godotengine.org/en/stable/classes/class_omnilight3d.html#class-omnilight3d)。

![../../../_images/light.png](https://docs.godotengine.org/en/stable/_images/light.png)

你可以看到光线影响地形，但看起来很奇怪。问题是光线对地形的影响就像是一个平面。这是因为光着色器使用 [Mesh](https://docs.godotengine.org/en/stable/classes/class_mesh.html#class-mesh) 中的法线来计算光。

法线存储在网格中，但我们正在着色器中更改网格的形状，因此法线不再正确。为了解决这个问题，我们可以在着色器中重新计算法线，或者使用与噪声相对应的法线纹理。戈多让我们两者都变得容易。

您可以在顶点函数中手动计算新的法线，然后只需设置 `NORMAL`。使用 `NORMAL` 设置，Godot将为我们完成所有困难的照明计算。我们将在本教程的下一部分介绍这种方法，现在我们将从纹理中读取法线。

相反，我们将再次依赖 NoiseTexture 来为我们计算法线。我们通过传递第二个噪波纹理来实现这一点。

```glsl
uniform sampler2D normalmap;
```

使用另一个 [FastNoiseLite](https://docs.godotengine.org/en/stable/classes/class_fastnoiselite.html#class-fastnoiselite) 将第二个均匀纹理设置为另一个 [NoiseTexture2D](https://docs.godotengine.org/en/stable/classes/class_noisetexture2d.html#class-noisetexture2d)。但这一次，请勾选 **As Normalmap**。

![../../../_images/normal-set.webp](https://docs.godotengine.org/en/stable/_images/normal-set.webp)

现在，因为这是一个法线贴图，而不是逐顶点法线，我们将在 `fragment()` 函数中分配它。本教程的下一部分将更详细地解释 `fragment()` 函数。

```glsl
void fragment() {
}
```

当我们有与特定顶点对应的法线时，我们会设置 `NORMAL`，但如果你有一个来自纹理的法线贴图，请使用 `NORMAL_MAP` 设置法线。这样，Godot 将自动处理网格周围的纹理包裹。

最后，为了确保我们从噪波纹理和法线贴图纹理的相同位置读取，我们将把 `vertex()` 函数的 `VERTEX.xz` 位置传递给 `fragment()` 函数。我们用变量来做这件事。

在 `vertex()` 之上定义一个名为 `tex_position` 的 `vec2`。在 `vertex()` 函数中，将 `VERTEX.xz` 赋值给 `tex_position`。

```glsl
varying vec2 tex_position;

void vertex() {
  ...
  tex_position = VERTEX.xz / 2.0 + 0.5;
  float height = texture(noise, tex_position).x;
  ...
}
```

现在我们可以从 `fragment()` 函数访问 `tex_position`。

```glsl
void fragment() {
  NORMAL_MAP = texture(normalmap, tex_position).xyz;
}
```

法线就位后，灯光现在会动态地对网格的高度做出反应。

![../../../_images/normalmap.png](https://docs.godotengine.org/en/stable/_images/normalmap.png)

我们甚至可以拖动灯光，灯光会自动更新。

![../../../_images/normalmap2.png](https://docs.godotengine.org/en/stable/_images/normalmap2.png)

这是本教程的完整代码。你可以看到，由于 Godot 为你处理了大部分困难的事情，所以并不长。

```glsl
shader_type spatial;

uniform float height_scale = 0.5;
uniform sampler2D noise;
uniform sampler2D normalmap;

varying vec2 tex_position;

void vertex() {
  tex_position = VERTEX.xz / 2.0 + 0.5;
  float height = texture(noise, tex_position).x;
  VERTEX.y += height * height_scale;
}

void fragment() {
  NORMAL_MAP = texture(normalmap, tex_position).xyz;
}
```

这就是这部分的全部内容。希望您现在了解了 Godot 中顶点着色器的基础知识。在本教程的下一部分中，我们将编写一个片段函数来配合这个顶点函数，我们将介绍一种更高级的技术，将这个地形变成一个移动的波浪海洋。

### 用户贡献的笔记

在提交评论之前，请阅读[用户贡献笔记政策](https://github.com/godotengine/godot-docs-user-notes/discussions/1)。

**1 个评论** · 2 个回复



[mrlem](https://github.com/mrlem) [Sep 7, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/120#discussioncomment-10571877)

我不是数学/着色器专家，但正常的计算不应该也基于高度标度吗？这似乎是合乎逻辑的，因为这个变量会影响三角形的方向。除非 NORMAL_MAP 在引擎盖下处理？

​	[Sleepybean2](https://github.com/Sleepybean2) [Sep 10, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/120#discussioncomment-10594911)

​	这实际上根本不会根据几何体计算法线。它使用纹理中的 uv 位置和颜色值来计算法线。该法线是准确的，但不是直接从几何图形计算出来的。因此，乘以 tex_position 将缩放纹理的 uv 位置（和颜色值，但它是归一化的），这意味着你的法线实际上将用于几何体上的另一个位置。如果按 0.5 缩放，则将 uv 位置乘以 2，从而得到一个法线，该法线适用于相距一半的顶点。

​	[Sleepybean2](https://github.com/Sleepybean2) [Sep 10, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/120#discussioncomment-10594922)

​	对不起，是 1/2，不是 2



## 你的第二个 3D 着色器

https://docs.godotengine.org/en/stable/tutorials/shaders/your_first_shader/your_second_3d_shader.html#your-second-3d-shader

从高层来看，Godot 所做的是为用户提供一系列可以选择设置的参数（`AO`、`SSS_Strength`、`RIM` 等）。这些参数对应于不同的复杂效果（环境光遮挡、亚表面散射、边缘照明等）。如果不写入，代码在编译之前就会被抛出，因此着色器不会产生额外功能的成本。这使得用户可以轻松地进行复杂的 PBR-正确着色，而无需编写复杂的着色器。当然，Godot 还允许您忽略所有这些参数，并编写一个完全自定义的着色器。

有关这些参数的完整列表，请参阅[空间着色器](https://docs.godotengine.org/en/stable/tutorials/shaders/shader_reference/spatial_shader.html#doc-spatial-shader)参考文档。

顶点函数和片段函数之间的区别在于，顶点函数按顶点运行并设置 `VERTEX`（位置）和 `NORMAL` 等属性，而片段着色器按像素运行，最重要的是设置 [MeshInstance3D](https://docs.godotengine.org/en/stable/classes/class_meshinstance3d.html#class-meshinstance3d) 的 `ALBEDO` 颜色。

### 您的第一个空间片段函数

正如本教程前一部分所述。Godot 中片段函数的标准用途是设置不同的材质属性，并让 Godot 处理其余的属性。为了提供更大的灵活性，Godot 还提供了称为渲染模式的功能。渲染模式设置在着色器的顶部，直接在 `shader_type` 下方，它们指定了您希望着色器的内置方面具有什么样的功能。

例如，如果不希望灯光影响对象，请将渲染模式设置为 `unshaded`：

```glsl
render_mode unshaded;
```

您还可以将多个渲染模式堆叠在一起。例如，如果要使用卡通着色而不是更逼真的 PBR 着色，请将漫反射模式和镜面反射模式设置为卡通：

```glsl
render_mode diffuse_toon, specular_toon;
```

这种内置功能模型允许您通过仅更改几个参数来编写复杂的自定义着色器。

有关渲染模式的完整列表，请参见[空间着色器参考](https://docs.godotengine.org/en/stable/tutorials/shaders/shader_reference/spatial_shader.html#doc-spatial-shader)。

在本教程的这一部分中，我们将介绍如何将上一部分的崎岖地形变成海洋。

首先，我们来设定水的颜色。我们通过设置 `ALBEDO` 来实现这一点。

`ALBEDO` 是一个包含对象颜色的 `vec3`。

让我们把它调成一种漂亮的蓝色。

```glsl
void fragment() {
  ALBEDO = vec3(0.1, 0.3, 0.5);
}
```

![../../../_images/albedo.png](https://docs.godotengine.org/en/stable/_images/albedo.png)

我们将其设置为非常深的蓝色，因为水的大部分蓝色将来自天空的反射。

Godot 使用的 PBR 模型依赖于两个主要参数：`METALLIC` 和 `ROUGHNESS`。

`ROUGHNESS` 指定材料表面的光滑/粗糙程度。低 `ROUGHNESS` 会使材料看起来像闪亮的塑料，而高粗糙度会让材料看起来颜色更纯色。

`METALLIC` 指定对象与金属的相似度。最好将其设置为接近 `0` 或 `1`。将 `METALLIC` 视为改变反射和 `ALBEDO` 颜色之间的平衡。高 `METALLIC` 几乎完全忽略了 `ALBEDO`，看起来就像天空的镜子。而低 `METALLIC` 更能代表天空颜色和 `ALBEDO` 颜色。

`ROUGHNESS` 从左到右从 `0` 增加到 `1`，而 `METALLIC` 从上到下从 `0` 增加至 `1`。

![../../../_images/PBR.png](https://docs.godotengine.org/en/stable/_images/PBR.png)

> **注：**
>
> `METALLIC` 应接近 `0` 或 `1`，以获得适当的 PBR 阴影。仅在它们之间设置它，用于混合材料。

水不是金属，因此我们将其 `METALLIC` 属性设置为 `0.0`。水也具有很高的反射性，因此我们也会将其 `ROUGHNESS` 属性设置得相当低。

```glsl
void fragment() {
  METALLIC = 0.0;
  ROUGHNESS = 0.01;
  ALBEDO = vec3(0.1, 0.3, 0.5);
}
```

![../../../_images/plastic.png](https://docs.godotengine.org/en/stable/_images/plastic.png)

现在我们有了一个光滑的塑料表面。是时候考虑我们想要模拟的水的一些特殊性质了。有两个主要的方法可以将它从一个奇怪的塑料表面变成一个漂亮的风格化的水。第一种是镜面反射。镜面反射是指你从太阳直接反射到眼睛的地方看到的亮点。第二个是菲涅耳反射率。菲涅耳反射率是物体在浅角度下变得更具反射性的特性。这就是为什么你可以看到下面的水，但更远的地方却能看到天空。

为了增加镜面反射，我们将做两件事。首先，我们将把镜面反射的渲染模式更改为 toon，因为 toon 渲染模式具有更大的镜面反射高光。

```glsl
render_mode specular_toon;
```

![../../../_images/specular-toon.png](https://docs.godotengine.org/en/stable/_images/specular-toon.png)

其次，我们将添加边缘照明。边缘照明增强了光线在掠角处的效果。通常，它用于模拟光线穿过物体边缘织物的方式，但我们将在这里使用它来帮助实现良好的水性效果。

```glsl
void fragment() {
  RIM = 0.2;
  METALLIC = 0.0;
  ROUGHNESS = 0.01;
  ALBEDO = vec3(0.1, 0.3, 0.5);
}
```

![../../../_images/rim.png](https://docs.godotengine.org/en/stable/_images/rim.png)

为了添加菲涅耳反射率，我们将在片段着色器中计算菲涅耳项。在这里，出于性能原因，我们不打算使用真正的菲涅耳项。相反，我们将使用 `NORMAL` 和 `VIEW` 向量的点积来近似它。`NORMAL` 矢量指向远离网格曲面的方向，而 `VIEW` 矢量是眼睛和曲面上该点之间的方向。它们之间的点积是一种方便的方法，可以判断你是正面看表面还是从某个角度看表面。

```glsl
float fresnel = sqrt(1.0 - dot(NORMAL, VIEW));
```

并将其混合到 `ROUGHNESS` 和 `ALBEDO` 中。这是 ShaderMaterials 优于 StandardMaterial3D 的优点。使用 StandardMaterial3D，我们可以用纹理或平面数字设置这些属性。但是有了着色器，我们可以根据我们能想到的任何数学函数来设置它们。

```glsl
void fragment() {
  float fresnel = sqrt(1.0 - dot(NORMAL, VIEW));
  RIM = 0.2;
  METALLIC = 0.0;
  ROUGHNESS = 0.01 * (1.0 - fresnel);
  ALBEDO = vec3(0.1, 0.3, 0.5) + (0.1 * fresnel);
}
```

![../../../_images/fresnel.png](https://docs.godotengine.org/en/stable/_images/fresnel.png)

现在，只需 5 行代码，你就可以得到外观复杂的水。现在我们有了照明，这水看起来太亮了。让我们把它变暗。这很容易通过减小我们传递给 `ALBEDO` 的 `vec3` 的值来实现。让我们将它们设置为 `vec3(0.01, 0.03, 0.05)`。

![../../../_images/dark-water.png](https://docs.godotengine.org/en/stable/_images/dark-water.png)

### 用 `TIME` 制作动画

回到顶点函数，我们可以使用内置变量 `TIME` 为波浪设置动画。

`TIME` 是一个内置变量，可以从顶点和片段函数访问。

在上一个教程中，我们通过阅读高度图来计算高度。对于本教程，我们将做同样的事情。将 heightmap 代码放入一个名为 `height()` 的函数中。

```glsl
float height(vec2 position) {
  return texture(noise, position / 10.0).x; // Scaling factor is based on mesh size (this PlaneMesh is 10×10).
}
```

为了在 `height()` 函数中使用 `TIME`，我们需要传入它。

```glsl
float height(vec2 position, float time) {
}
```

并确保在顶点函数内正确传递它。

```glsl
void vertex() {
  vec2 pos = VERTEX.xz;
  float k = height(pos, TIME);
  VERTEX.y = k;
}
```

而不是使用法线贴图来计算法线。我们将在 `vertex()` 函数中手动计算它们。为此，请使用以下代码行。

```glsl
NORMAL = normalize(vec3(k - height(pos + vec2(0.1, 0.0), TIME), 0.1, k - height(pos + vec2(0.0, 0.1), TIME)));
```

我们需要手动计算 `NORMAL`，因为在下一节中，我们将使用数学来创建复杂的外观波。

现在，我们将通过用 `TIME` 的余弦偏移 `position` 来使 `height()` 函数稍微复杂一些。

```glsl
float height(vec2 position, float time) {
  vec2 offset = 0.01 * cos(position + time);
  return texture(noise, (position / 10.0) - offset).x;
}
```

这导致波浪移动缓慢，但不是以非常自然的方式。下一节将深入探讨使用着色器通过添加更多数学函数来创建更复杂的效果，在这种情况下是逼真的波浪。

### 高级效果：波浪

着色器之所以如此强大，是因为你可以通过使用数学来实现复杂的效果。为了说明这一点，我们将通过修改 `height()` 函数并引入一个名为 `wave()` 的新函数，将我们的 wave 提升到一个新的层次。

`wave()` 有一个参数 `position`，它与 `height()` 中的参数相同。

我们将在 `height()` 中多次调用 `wave()`，以伪造波浪的外观。

```glsl
float wave(vec2 position){
  position += texture(noise, position / 10.0).x * 2.0 - 1.0;
  vec2 wv = 1.0 - abs(sin(position));
  return pow(1.0 - pow(wv.x * wv.y, 0.65), 4.0);
}
```

起初，这看起来很复杂。所以，让我们一行一行地浏览一下。

```glsl
position += texture(noise, position / 10.0).x * 2.0 - 1.0;
```

通过 `noise` 纹理偏移位置。这将使波浪弯曲，因此它们不会是与网格完全对齐的直线。

```glsl
vec2 wv = 1.0 - abs(sin(position));
```

使用 `sin()` 和 `position` 定义一个波浪形函数。通常 `sin()` 波是非常圆的。我们使用 `abs()` 进行绝对化，以给它们一个尖锐的脊，并将它们约束在 0-1 的范围内。然后我们从 `1.0` 中减去它，把峰值放在顶部。

```glsl
return pow(1.0 - pow(wv.x * wv.y, 0.65), 4.0);
```

将 x 方向波乘以 y 方向波，并将其提高到某个功率以锐化峰值。然后从 `1.0` 中减去该值，使脊变为峰值，并将其提高到使脊变尖的幂。

我们现在可以用 `wave()` 替换 `height()` 函数的内容。

```glsl
float height(vec2 position, float time) {
  float h = wave(position);
  return h;
}
```

使用此方法，您可以获得：

![../../../_images/wave1.png](https://docs.godotengine.org/en/stable/_images/wave1.png)

正弦波的形状太明显了。所以，让我们把波浪分散一点。我们通过缩放 `position` 来做到这一点。

```glsl
float height(vec2 position, float time) {
  float h = wave(position * 0.4);
  return h;
}
```

现在看起来好多了。

![../../../_images/wave2.png](https://docs.godotengine.org/en/stable/_images/wave2.png)

如果我们以不同的频率和振幅将多个波叠加在一起，我们可以做得更好。这意味着我们将缩放每个波的位置，使波变薄或变宽（频率）。我们将把波的输出相乘，使其更短或更高（振幅)。

以下是一个示例，说明如何对四个波进行分层，以获得更美观的波。

```glsl
float height(vec2 position, float time) {
  float d = wave((position + time) * 0.4) * 0.3;
  d += wave((position - time) * 0.3) * 0.3;
  d += wave((position + time) * 0.5) * 0.2;
  d += wave((position - time) * 0.6) * 0.2;
  return d;
}
```

请注意，我们将时间加到两个上，然后从另外两个中减去。这使得波浪向不同的方向移动，从而产生复杂的效果。还要注意，振幅（结果乘以的数字）加起来都是 `1.0`。这将使波保持在 0-1 范围内。

有了这段代码，你最终会得到看起来更复杂的波浪，你所要做的就是添加一些数学运算！

![../../../_images/wave3.png](https://docs.godotengine.org/en/stable/_images/wave3.png)

有关空间着色器的更多信息，请阅读“[着色语言](https://docs.godotengine.org/en/stable/tutorials/shaders/shader_reference/shading_language.html#doc-shading-language)”文档和“[空间着色器](https://docs.godotengine.org/en/stable/tutorials/shaders/shader_reference/spatial_shader.html#doc-spatial-shader)”文档。还要查看[着色部分](https://docs.godotengine.org/en/stable/tutorials/shaders/index.html#toc-learn-features-shading)和[三维](https://docs.godotengine.org/en/stable/tutorials/3d/index.html#toc-learn-features-3d)部分中的更高级教程。

### 用户贡献的笔记

在提交评论之前，请阅读[用户贡献笔记政策](https://github.com/godotengine/godot-docs-user-notes/discussions/1)。

**4 个评论** · 8 个回复



[ChrisDWhitehead](https://github.com/ChrisDWhitehead) [Aug 29, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/95#discussioncomment-10480523)

我一直在尝试遵循本教程，但我得到的结果与图片所示完全不同。如果最后有一个完整的代码列表，就像 2D Shader 教程中那样，那将非常有帮助。我怀疑正在运行的解释中可能缺少代码。

​	[Catley94](https://github.com/Catley94) [Aug 29, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/95#discussioncomment-10488861)

​	我在下面发布了我的代码，它是有效的，我不得不稍微调整一下，我用水的“平静”进行了修改，不过欢迎你忽略这一点，用 10.0 这个神奇的数字替换变量。

​	[mrlem](https://github.com/mrlem) [Sep 7, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/95#discussioncomment-10576138)

​	我发现这两个教程之间缺少的东西：

- 您应该将平面从 2x2（第一个教程）调整为 10x10（第二个教程中假设）
- 您应该使噪波纹理无缝，以避免在纹理边缘出现周期性的“波峰”

​	[Catley94](https://github.com/Catley94) [Sep 7, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/95#discussioncomment-10576152)

​	对！虽然不知道第二个，但稍后会看看。谢谢！



[Catley94](https://github.com/Catley94) [Aug 29, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/95#discussioncomment-10488237)

我也在努力解决这个问题——到目前为止，我注意到了两件事，当我从第 1 部分=>第 2 部分开始时，我的峰值比第 2 部分的第一张图片更清晰，尽管我忽略了这一点。

但我真正陷入困境的是 TIME，你第一次提到顶点函数是：

```glsl
void vertex() {
  vec2 pos = VERTEX.xz;
  float k = height(pos, TIME);
  VERTEX.y = k;
}
```

然而，在我做出任何改变之前，我的样子是：

```glsl
void vertex() {
	tex_position = VERTEX.xz / 2.0 + 0.5;
	float height = texture(noise, tex_position).x;
	VERTEX.y += height * height_scale;
}
```

我现在正在弄清楚这一点，但我觉得过去和现在之间存在脱节。（尽管这也可能是我目前陷得太深了）。
将继续并在最后发布我的代码，看看其他人是否也得到了同样的代码：-）

​	[Calinou](https://github.com/Calinou) [Aug 31, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/95#discussioncomment-10504759)

​	PS：代码块应该使用这样的三重回溯（带有可选的语言名称用于语法突出显示）：

````markdown
```glsl
code here
```
````

​	我相应地编辑了你的帖子，但以后记得这样做🙂

​	[Catley94](https://github.com/Catley94) [Aug 31, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/95#discussioncomment-10506645)

​	谢谢你！😊



[Catley94](https://github.com/Catley94) [Aug 29, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/95#discussioncomment-10488853)

这是我的最后一段代码，尽管我对其进行了轻微的修改以“驯服”水域，因为我在教程之后的实现非常不稳定和快速。

冷静变量从 1 到 100，但我会坚持在 20 到 60 之间（这个数字取决于你用 `d += wave((position - time) * 0.6) * 0.2; //Or similar` 创建的波的数量）。

下面的代码很好地产生了一段相对平静的水，多层水向不同方向移动。

```glsl
shader_type spatial;
render_mode specular_toon;

uniform float height_scale = 0.5;
uniform sampler2D noise;
uniform sampler2D normalmap;
uniform float calmness = 50.0; // 1 - 100
uniform int num_of_additional_waves_in_x_direction = 2; // May be labeled as the wrong direction
uniform int num_of_additional_waves_in_z_direction = 2; // May be labeled as the wrong direction

varying vec2 tex_position;

float wave(vec2 position) {
	position += texture(noise, position / calmness).x * 2.0 - 1.0;
	vec2 wv = 1.0 - abs(sin(position));
	return pow(1.0 - pow(wv.x * wv.y, 0.65), 4.0);
}

float height(vec2 position, float time) {
	// Change your MeshInstance3D PlaneMesh to 10 x 10

	float d = wave((position + time) * 0.4) * 0.3;
	
	// Give the user control over how many waves and in what direction
	
	//for(int i = 0; i < num_of_additional_waves_in_x_direction; i++) {
		////Randomise magic number 0.3 and 0.3 to a range beteen 0.1 and 1
		//d += wave((position + time) * 0.3) * 0.3;
	//}
	//
	//for(int i = 0; i < num_of_additional_waves_in_z_direction; i++) {
		////Randomise magic number 0.3 and 0.3 to a range beteen 0.1 and 1
		//d += wave((position - time) * 0.3) * 0.3;
	//}
	
	// Statically create the waves
	
	d += wave((position - time) * 0.3) * 0.3;
	d += wave((position + time) * 0.5) * 0.2;
	d += wave((position - time) * 0.6) * 0.2;
	return d;
}


void vertex() {
	// Called for every vertex the material is visible on.	

	//Do I change the below to height(tex_position, TIME); ?
	// Version - 1
	//tex_position = VERTEX.xz / 5.0 + 0.5;
	//float _height = texture(noise, tex_position).x;
	
	//VERTEX.y += _height * height_scale;
	
	
	vec2 pos = VERTEX.xz;
	float k = height(pos, TIME);

	// Version - 2
  	VERTEX.y = k;
	
	NORMAL = normalize(vec3(k - height(pos + vec2(0.1, 0.0), TIME), 0.1, k - height(pos + vec2(0.0, 0.1), TIME)));
	
	
}

void fragment() {
	// Called for every pixel the material is visible on.
	
	float fresnel = sqrt(1.0 - dot(NORMAL, VIEW));
	NORMAL_MAP = texture(normalmap, tex_position).xyz;
	
	
	RIM = 0.2;
	METALLIC = 0.0;
	ROUGHNESS = 0.01 * (1.0 - fresnel);
	ALBEDO = vec3(0.01, 0.03, 0.05) +  + (0.1 * fresnel);
	
}

//void light() {
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}```
```

​	[mrlem](https://github.com/mrlem) [Sep 7, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/95#discussioncomment-10576149)

​	我想说，在你的代码中，你应该删除 `fragment()` 中的 `NORMAL_MAP`，因为法线现在是在 `vertex()` 中计算的

​	[Catley94](https://github.com/Catley94) [Sep 7, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/95#discussioncomment-10576160)

​	谢谢你，我还在学习着色器，还以为它们是不同的东西哈哈。目前我正在通过 TheBookofShaders 工作，希望这能有所帮助：）



[Vosburgh](https://github.com/Vosburgh) [Nov 20, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/95#discussioncomment-11322878)

本教程需要重新复习，着色器已经足够难了。为什么教程要跳过步骤，把猜测留给作者做什么？

至少给我们留下一些可以相互参照的东西。

​	[Calinou](https://github.com/Calinou) [Nov 23, 2024](https://github.com/godotengine/godot-docs-user-notes/discussions/95#discussioncomment-11354040)

​	这在 [godotengine/godot-docs#8077](https://github.com/godotengine/godot-docs/issues/8077) 中进行了跟踪。



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