

# 实现目标

## 放置战略游戏

让玩法从简单基础慢慢长出来（由简入深，逐步细化）

1. 就是最基础的人数地图涂色，占领地块，放置看海。
   1. 考虑实现：环形地图？海上、空中战斗和经营如何设计？

## 整个项目

作为一个自己的实验项目，目标上线 Steam，尽量跑通整个流程。暂时完全不考虑经济盈利目标 ，按 AGPL-3.0 协议开源全部代码。

计划是把各种小原型、测试 Demo 都可以丢这个项目里面，设计一个主界面提供各个小部分的访问入口，然后保持整个项目运行稳定即可。

开发过程中尽量把相关**国际化、存档、MOD、版本更新、联机**等技术点开发 Demo 测试一下



# 待办

## C# 基础

- [ ] C# 解决方案（这种设计的作用？）、NuGet（如何拆分项目，分解成引用工具？）
- [ ] C# namespace 命名空间的规范
- [ ] Chickensoft 框架学习（Demo 以及各个工具类）

## 游戏开发

- [x] 地块使用 kenney cartography-pack 绘制细节



# 灵感

1. Steam bitburner 游戏 - 代码编辑器语法颜色效果怎么做？（复习编译原理？）
1. Steam 架空地图模拟器（比较单纯的看海游戏）
1. 领土战争、地图战争、[帝国扩张](https://www.bilibili.com/video/BV1d54y1T7ua)（我自己想到的是小时候玩的小游戏[细菌战争](https://www.bilibili.com/video/BV1Jt411g7BG)）、[Territorial.io](https://www.bilibili.com/video/BV1aP411M7UW)、文明时代1



# 知识点

## C#

### 二维数组

```c#
int[,] arr = new int[2,3]; // 二维数组写法
int l0 = arr.GetLength(0); // 2
int l1 = arr.GetLength(1); // 3
int l = arr.Length; // 6
GD.Print($"l0: {l0}, l1: {l1}, l: {l}");
```

### SQLite

参考文档

- Microsoft.Data.Sqlite 概述：https://learn.microsoft.com/zh-cn/dotnet/standard/data/sqlite/?tabs=netcore-cli
- SQLite EF Core 数据库提供程序：https://learn.microsoft.com/zh-cn/ef/core/providers/sqlite/?tabs=dotnet-core-cli
- EF Core 入门：https://learn.microsoft.com/zh-cn/ef/core/get-started/overview/first-app?tabs=netcore-cli



因为 Godot 是 .NET 6.0 和 C# 10 实现的，所以好像引入的 Microsoft.EntityFrameworkCore.Sqlite 版本必须是 6.0.x（最一开始试着导入 8.0.8 的时候报错了）：

```xml
<Project Sdk="Godot.NET.Sdk/4.3.0">
  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <TargetFramework Condition=" '$(GodotTargetPlatform)' == 'android' ">net7.0</TargetFramework>
    <TargetFramework Condition=" '$(GodotTargetPlatform)' == 'ios' ">net8.0</TargetFramework>
    <EnableDynamicLoading>true</EnableDynamicLoading>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="6.0.33" />
  </ItemGroup>
</Project>
```





## Godot

### 信号

C# 的信号需要注意几点：

1. **继承 GodotObject**（Node 天然继承，主要针对自己定义的 C# 类），不然就没有 EmitSignal 方法，也不会自动编译出 SignalName
2. 使用 **EmitSignal() 少了参数**是不会编译报错的，需要特别注意一下，不然信号发送不出去也没有任何提示。



### 文件路径

参考文档：

- 手册 - 文件与数据 I/O - Godot 项目中的文件路径 https://docs.godotengine.org/zh-cn/4.x/tutorials/io/data_paths.html

#### 访问持久化用户数据（`user://`）[¶](https://docs.godotengine.org/zh-cn/4.x/tutorials/io/data_paths.html#accessing-persistent-user-data-user)

要存储持久化数据文件，比如玩家的存档、设置等，你会想要使用 `user://` 作为路径前缀，而不是 `res://`。这是因为游戏运行时，项目的文件系统很可能是只读的。

`user://` 前缀指向的是用户设备上的其他目录。与 `res://` 不同，即便在导出后的项目中，`user://` 指向的这个目录也会自动创建并且*保证*可写。

`user://` 文件夹的位置由“项目设置”中的配置决定：

- 默认情况下，`user://` 文件夹是在[编辑器数据路径](https://docs.godotengine.org/zh-cn/4.x/tutorials/io/data_paths.html#doc-data-paths-editor-data-paths)中创建的 `app_userdata/[项目名称]` 文件夹。使用这一默认值的目的是让原型和测试项目能够在 Godot 的数据文件夹中达到自包含。
- 如果“项目设置”中启用了 [application/config/use_custom_user_dir](https://docs.godotengine.org/zh-cn/4.x/classes/class_projectsettings.html#class-projectsettings-property-application-config-use-custom-user-dir)，`user://` 文件夹会与 Godot 编辑器的数据路径*同级*，即程序数据的标准位置。
  - 默认情况下，文件夹名称是从项目名称推导出来的，但可以使用 [application/config/custom_user_dir_name](https://docs.godotengine.org/zh-cn/4.x/classes/class_projectsettings.html#class-projectsettings-property-application-config-custom-user-dir-name) 进行进一步的自定义。这个路径可以包含路径分隔符，那么比如你就可以把给定工作室的项目都分组到 `工作室名称/游戏名称` 这样的目录结构之下。

在桌面平台上，`user://` 的实际目录路径为：

| 类型             | 位置                                                         |
| ---------------- | ------------------------------------------------------------ |
| 默认             | Windows：`%APPDATA%\Godot\app_userdata\[项目名称]`<br />macOS：`~/Library/Application Support/Godot/app_userdata/[项目名称]`<br />Linux：`~/.local/share/godot/app_userdata/[项目名称]` |
| 自定义目录       | Windows：`%APPDATA%\[项目名称]`<br />macOS：`~/Library/Application Support/Godot/[项目名称]`<br />Linux：`~/.local/share/godot/[项目名称]` |
| 自定义目录及名称 | Windows：`%APPDATA%\[自定义目录名称]`<br />macOS：`~/Library/Application Support/[自定义目录名称]`<br />Linux：`~/.local/share/[自定义目录名称]` |

`[项目名称]` 基于的是项目设置中定义的应用名称，不过你可以使用[特性标签](https://docs.godotengine.org/zh-cn/4.x/tutorials/export/feature_tags.html#doc-feature-tags)来为不同平台单独进行覆盖。

在移动平台上，这个路径是与项目相关的，每个项目都不一样，并且出于安全原因无法被其他应用程序访问。

在 HTML5 导出中，`user://` 会指向保存在设备的虚拟文件系统，这个文件系统使用 IndexedDB 实现。（仍然可以通过 [JavaScriptBridge](https://docs.godotengine.org/zh-cn/4.x/classes/class_javascriptbridge.html#class-javascriptbridge) 与主文件系统交互。）

#### 将路径转换为绝对路径或“本地”路径[¶](https://docs.godotengine.org/zh-cn/4.x/tutorials/io/data_paths.html#converting-paths-to-absolute-paths-or-local-paths)

你可以使用 [ProjectSettings.globalize_path()](https://docs.godotengine.org/zh-cn/4.x/classes/class_projectsettings.html#class-projectsettings-method-globalize-path) 将类似 `res://path/to/file.txt` 的本地路径转换为操作系统的绝对路径。例如，可以使用 [ProjectSettings.globalize_path()](https://docs.godotengine.org/zh-cn/4.x/classes/class_projectsettings.html#class-projectsettings-method-globalize-path) 在操作系统的文件管理器中通过 [OS.shell_open()](https://docs.godotengine.org/zh-cn/4.x/classes/class_os.html#class-os-method-shell-open) 打开“本地”路径，因为这个函数只接受原生操作系统路径。

要将操作系统绝对路径转换为以 `res://` 或 `user://` 开头的“本地”路径，请使用 [ProjectSettings.localize_path()](https://docs.godotengine.org/zh-cn/4.x/classes/class_projectsettings.html#class-projectsettings-method-localize-path)。只对指向项目根目录或者 `user://` 文件夹中的文件或文件夹有效。



# 暂时搁置

## 六边形球体

最基础游戏界面是六边形球体世界地图，周围是 GUI

- [ ] 3D 六边形球体
  - [ ] **@Tool 编辑器在 C# 修改编译后会疯狂打报错日志并卡死，必须重新生成星球才行**
  - [ ] 复刻星球水效果（[[中英双字][Sebastian Lague]编程挑战 地形系统](https://www.bilibili.com/video/BV1kB4y1N7xG)）
  - [ ] 复刻大气层效果（[[中英双字][Sebastian Lague]编程挑战 大气层](https://www.bilibili.com/video/BV1K541187dD)）
  - [ ] 代码重构
  - [ ] 实现类似 TileMap 的材质填涂功能
  - [ ] 寻路功能
  - [ ] 地形编辑器（修改高度等）

## 过大的战略放置游戏策划

刚开始没必要想太多，想这么多做不完也没用……

1. 实现一个基础的 AI 自动互相征伐的战略游戏框架
   1. 电子斗蛐蛐
   2. 自动发展、扩张
   3. 自动外交、战争
2. 玩家通过放置玩法积累点数，消耗点数干预 AI 国家发展
   1. 自然条件修改
      1. 增加粮食资源等等
      2. 增加特定人物（模仿抽卡/roguelike 三选一机制？）
      3. 作战天气
      4. 干预人物决策概率
   2. 国家战略干预
      1. 获取信息（默认可以屏蔽一些信息，通过点数解锁或者查看）
      2. 确定攻击方向
      3. 触发事件（引导舆论、类似P社彗星）
   3. 等等……

### 基础资源

- 人口
  - 职业分布
  - 年龄分布
  - 财产分布
  - 权力网络
- 食物
  - 农田
  - 捕猎（特殊毛皮、象牙之类的）
  - 渔获
- 水源
  - 河流
  - 湖泊
  - 井
- 木材
  - 水土流失
- 矿物
  - 金银铜铁