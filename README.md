# 实现目标

## 放置战略游戏

让玩法从简单基础慢慢长出来（由简入深，逐步细化）

1. 就是最基础的人数地图涂色，占领地块，放置看海。
   1. 考虑实现：环形地图？海上、空中战斗和经营如何设计？

## 整个项目

作为一个自己的实验项目，目标上线 Steam，尽量跑通整个流程。暂时完全不考虑经济盈利目标 ，按 AGPL-3.0 协议开源全部代码。

计划是把各种小原型、测试 Demo 都可以丢这个项目里面，设计一个主界面提供各个小部分的访问入口，然后保持整个项目运行稳定即可。

开发过程中尽量把相关**国际化、存档、MOD、版本更新、联机**等技术点开发 Demo 测试一下

## 时间安排

1. 开发规划（计划要开发的内容，对要开发的内容进行 DDD 建模）
2. 技术预研（对于开发过程中的难点进行技术预研）
3. 实际开发（代码、美术等）



# 分析

- 初始化
  - 地图：根据 TileMapLayer 内容初始化 Tile
    - 地块默认无主状态

  - 玩家：每个玩家占领一块地
    - 临时初始化策略：随机选一块地

- 人口循环：
  - 每 X 秒玩家地块增加人口

- 出兵循环
  - 游戏开始 Y 秒后，所有玩家出兵
    - 出兵策略：**出发地块为玩家领土随机**、目的地随机**邻接地块**、人数为**出发地人口下随机**。
      - 玩家查询所属地块
      - 查询地块周围联通地块

  - 部队
    - **行动速度**和人口成反比
    - 部队从所属地块出发，扣减地块人口
    - 部队抵达目的地块，同玩家控制则增加地块人口，无主地块或敌方少人口地块则被占领，敌方人口多地块则扣减敌方人口。

  - 玩家部队抵达目的地后，触发下一次出兵。




目前最头疼的两个问题（现均已解决）：

- **函数式编程游戏状态的保存**
  - 是否需要引入 State Monad？
  - 存储库在函数式编程中如何设计比较好？
    - 和领域层的关系？
      - 新对象工厂的问题
        - 半成品实体怎么处理？尤其 id 得从存储库拿，这里逻辑有点别扭
      - 这个也涉及到事件抛出的时机，和内存中存储库或数据库事务可见性的关系。
        - *之前已经出过 bug，暂时在事件里加了需要提前暴露的数据（查询查不到）绕过了设计问题*
    - 是否需要考虑未来对数据库逻辑的兼容？
- **领域事件**
  - **使用 ReactiveX 响应式编程**
    - 本身应该也算是副作用
    - 后续异步可能好改？
  - 直接返回中传出
    - 全程同步的纯函数写法
    - 貌似没想到在 F# 后端进行每 X 秒操作的实现方式
      - 除非是入参里面传回调函数
      - 否则只能依赖于 Godot Timer 来从前端调进来了



# 待办

## C# 基础

- [ ] **【2024-10-21 10:04】**：为啥 Rider psvm 生成 Main(string[] args) 就运行不了，Main() 就可以？
- [ ] **【2024-10-21 10:04】**：为啥 Main 函数里面 nullable foreach 会提示处理空，但自己 F# 互操作函数里就不提示？F# 生成的返回用元组匹配为啥第一个都会变成 nullable？越来越看不懂了属于是

## F#

- [ ] GameState 放哪里？全局节点？Reactive 管道里？

## 响应式编程

- [ ] **【2024-10-27 10:08】**比如先占领地块扔出的事件，如何同步在写入游戏状态后再被订阅者收到？（类似事务，得同步控制，看书上感觉可以用 `ConnectableObservable` 做？但这样频繁开关会不会有问题）
- [ ] **【2024-10-27 15:37】**`Delay` 实现发兵以后抵达的通知



# 灵感

## 游戏开发

1. Steam bitburner 游戏 - 代码编辑器语法颜色效果怎么做？（复习编译原理？）
1. Steam 架空地图模拟器（比较单纯的看海游戏）
1. 领土战争、地图战争、[帝国扩张](https://www.bilibili.com/video/BV1d54y1T7ua)（我自己想到的是小时候玩的小游戏[细菌战争](https://www.bilibili.com/video/BV1Jt411g7BG)）、[Territorial.io](https://www.bilibili.com/video/BV1aP411M7UW)、文明时代1
1. 复刻星球水效果（[[中英双字][Sebastian Lague]编程挑战 地形系统](https://www.bilibili.com/video/BV1kB4y1N7xG)）、复刻大气层效果（[[中英双字][Sebastian Lague]编程挑战 大气层](https://www.bilibili.com/video/BV1K541187dD)）

## C#

1. Java MapStruct 对应的工具库在 C# 有吗？（搜到一个 AutoMapper）



# 知识点

## Git

```shell
# 清除文件更新跟踪（保留本地文件，删除 git 中已经提交的记录）
git rm --cached localization/language.en.translation
# 清除文件更新跟踪（保留本地文件，不删除 git 中已经提交的记录）
git update-index --assume-unchanged localization/language.en.translation
```

删除了 Git 中的 Godot 本地化二进制文件（省的每次都得全量更新），拉取项目不确定是否会自动编译出来，可以修改 csv 触发

## C#

https://learn.microsoft.com/zh-cn/dotnet/csharp/



### 一些和 Java 特殊的点

- 解构函数：Deconstruct()

- C# 嵌套类和 Java 内部类不同，需要注意

- 分部类、分部方法：partial

- C# 默认方法非虚，Java 默认为虚方法。C# override 关键字是必须的，不允许隐式重写

  - C# 支持重写实例方法和属性，但不支持字段和任何静态成员的重写。为进行重写，要求在基类和派生类中都显式执行一个操作。基类必须将允许重写的每个成员都标记为 virtual。如一个 public 或 protected 成员没有包含 virtual 修饰符，就不允许子类重写该成员。
  - new 修饰符：它在基类面前隐藏了派生类重新声明的成员。
  - sealed 修饰符：为类使用sealed修饰符可禁止从该类派生。类似地，虚成员也可密封

- 实现接口的时候分为显式实现和隐式实现

- 值类型和引用类型：struct、匿名类型、元组

- C# 可以重载操作符

  - 转型操作符

    ```c#
    public static implicit operator ToClass(FromClass fromClass) {}
    // implicit 隐式，explicit 显式
    // ToClass、FromClass 可以是一般的类，也可以是 double 这种
    ```

- 匿名类型（引用类型）与元组（值类型）

- C# 特性类似于 Java 注解

- C# 局部变量作用域和 Java 不同。`foreach (var i in ints) {}` 外面就不能再声明 `var i` 了

### 访问修饰符

public、private、protected、internal、protected internal 和 private protected

### 数组

```c#
// languages 是数组
// 反向索引 ^ (System.Index)和区间 .. (System.Range)用法：打印从倒数第 3 个开始到最后一个
System.Console.WriteLine($@"^3..^0: {
	string.Join(", ", languages[^3..^0])
}");
System.Console.WriteLine($@"^3..: {
	string.Join(", ", languages[^3..])
}");

// 数组及其元素均声明为可空
string?[]? segments;
System.Console.WriteLine(segments?[0]?.ToLower() ?? "test");
```

二维数组

```c#
int[,] arr = new int[2,3]; // 二维数组写法
int l0 = arr.GetLength(0); // 2
int l1 = arr.GetLength(1); // 3
int l = arr.Length; // 6
GD.Print($"l0: {l0}, l1: {l1}, l: {l}");
```

交错数组 `int[][]`

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



## 领域驱动设计（DDD）

《领域驱动设计精粹》

- 战略设计
  - 限界上下文
  - 通用语言
  - 六边形架构
    - 端口和适配器
      - 输入适配器（安全、用户界面、展示层）
      - 应用服务（安全、事务、任务协调、用例控制器）
      - 领域模型（实体、业务逻辑、领域事件）
      - 输出适配器（仓库、文档、缓存、消息机制）
  - 子域
    - 核心域
    - 支撑子域
    - 通用子域
  - 上下文映射
    - 基于 SOAP 的 RPC
    - RESTful HTTP
    - 消息机制
- 战术设计
  - 聚合
    - 实体
      - 具有唯一标识符
    - 值对象
  - 领域事件
    - 事件溯源
- 事件风暴



也可以参考一下《微服务架构设计模式》一书，感觉这本书从实践的角度提到 DDD 和相关一些架构方法，重点讲的挺清晰的。

- 六边形架构
- Saga 事务
- 聚合
- 领域事件
  - 事件触发器
    - 用户操作
    - 外部系统
    - 另一个领域事件
    - 时间的流逝
- 事件溯源
  - （不知道能不能通过这种方式实现游戏过程回放等功能？）
  - （是否方便网络状态同步？）
- CQRS

其他架构上考虑的点先暂时放这里记录一下：

- 日志
- 请求分配唯一 ID 方便全局跟踪
- 异常跟踪
  - （尤其 Godot 会吃异常的问题，得研究下怎么设计）



## Godot

### 小知识

1. Engine.has_singleton() / register_singleton() 单例模式相关功能
2. `项目 -> 项目设置` 打开 `常规` 选项卡的 `高级设置` 开关，`.NET -> 项目` 中可以配置解决方案目录；`项目 -> 工具 -> C# -> Create C# Solution ` 可以创建 C# 解决方案

### 信号

C# 的信号需要注意几点：

1. **继承 GodotObject**（Node 天然继承，主要针对自己定义的 C# 类），不然就没有 EmitSignal 方法，也不会自动编译出 SignalName（注意：信号传递的参数也必须继承 GodotObject）
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





