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



# 待办

## C# 基础

- [ ] **【2024-10-21 10:04】**：为啥 Rider psvm 生成 Main(string[] args) 就运行不了，Main() 就可以？
- [x] **【2024-10-21 10:04】**：为啥 Main 函数里面 nullable foreach 会提示处理空，但自己 F# 互操作函数里就不提示？F# 生成的返回用元组匹配为啥第一个都会变成 nullable？越来越看不懂了属于是**（现在已经没有这种互操作了）**

## F#

- [ ] **【2024-11-23 13:57】**：编辑器 Export 变量默认值改不了，得想个解决方案

## FSharpPlus

- [x] **【2024-11-09 17:43】**：突然想到之前两层 Monad 的实现一直调不通，连接地块只会斜着连一格。估计原因是得用 `monad.plus` 计算表达式才能把过程积累返回出来？或者 `monad'` 才能严格执行而不是惰性？不然 `monad` 直接惰性，中间过程是不是就实际没执行
  **（不过现在直接干掉两层 monad 的相关实现代码了，以后类似情况得注意一下四个 `monad` 相关计算表达式的区别）**

## 单元测试

- [ ] **【2024-11-12 19:50】**：思考如何保证随机过程的可测试性（日志、顺序性、测试框架引入面向特性的测试 FsCheck、辅助  Godot 运行时的测试验证界面和工具）

## 游戏内容

- [x] **【2024-11-12 19:50】**：类似 P 社游戏的速度调节、暂停功能
- [ ] **【2024-11-12 19:50】**：环球效果的实现、视角移动
- [ ] **【2024-11-12 19:50】**：游戏内地图编辑器



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



## F#

### 运算符优先级

https://learn.microsoft.com/zh-cn/dotnet/fsharp/language-reference/symbol-and-operator-reference/#operator-precedence

下表显示了 F# 中运算符和其他表达式关键字的优先级顺序，从最低优先级到最高优先级。如果适用，还列出了关联性。

| 运算符                                                       | 结合性                   |
| :----------------------------------------------------------- | :----------------------- |
| `as`                                                         | 右                       |
| `when`                                                       | 右                       |
| `|` (管道)                                                   | 左                       |
| `;`                                                          | 右                       |
| `let`                                                        | 无结合性                 |
| `function`, `fun`, `match`, `try`                            | 无结合性                 |
| `if`                                                         | 无结合性                 |
| `not`                                                        | 右                       |
| `->`                                                         | 右                       |
| `:=`                                                         | 右                       |
| `,`                                                          | 无结合性                 |
| `or`, `||`                                                   | 左                       |
| `&`, `&&`                                                    | 左                       |
| `:>`, `:?>`                                                  | 右                       |
| `<`*操作*, `>`*操作*, `=`, `|`*操作*, `&`*操作*, `&`, `$`  (包括 `<<<`, `>>>`, `|||`, `&&&`) | 左                       |
| `^`*操作*  (包括 `^^^`)                                      | 右                       |
| `::`                                                         | 右                       |
| `:?`                                                         | 不可结合                 |
| `-`*操作*, `+`*操作*                                         | 适用于这些符号的中缀使用 |
| `*`*操作*, `/`*操作*, `%`*操作*                              | 左                       |
| `**`*操作*                                                   | 右                       |
| `f x` (函数应用)  (包括 `lazy x`, `assert x`)                | 左                       |
| `|` (模式匹配)                                               | 右                       |
| 前缀操作符 (`+`*操作*, `-`*操作*, `%`, `%%`, `&`, `&&`, `!`*操作*, `~`*操作*) | 左                       |
| `.`                                                          | 左                       |
| `f(x)`                                                       | 左                       |
| `f<`*类型*`>`                                                | 左                       |

F# 支持自定义运算符重载。这意味着您可以定义自己的运算符。在上一个表中，*操作*（op）可以是任何有效（可能为空）的运算符字符序列，可以是内置的或用户定义的。因此，您可以使用此表来确定自定义运算符使用哪种字符序列来实现所需的优先级。开头的 `.` 字符当编译器确定优先级时将被忽略。



### FSharpPlus

#### Monad 计算表达式

- `monad.fx` 或简称 `monad`：懒惰的单子构建器。当你想使用副作用而不是 monadplus 的加法行为时使用。
- `monad.fx.strict`（或 `monad.fx'`，或简称 `monad.strict` 或 `monad'`）是 `monad` 的严格版本。
- `monad.plus`：懒惰的加法单子构建器。当你期待一个或多个结果时使用。
- `monad.plus'` 是 `monad.plus` 的严格版本

#### traverse 和 sequence

```F#
traverse f = map f |> sequence
sequence = fold monadFolder (monadReturn Seq.empty) // monadFolder 和 monadReturn 是我自己的实现。有的时候 sequence 类型推断不出来，必须得用明确的实现
```



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

### issue

#### Assertion failed: Script path can't be empty

**我的报错**

```
  Assertion failed: Script path can't be empty.
    Details: 
     at Godot.GodotTraceListener.Fail(String message, String detailMessage) in /root/godot/modules/mono/glue/GodotSharp/GodotSharp/Core/GodotTraceListener.cs:line 24
     at System.Diagnostics.TraceInternal.Fail(String message, String detailMessage)
     at System.Diagnostics.Debug.Fail(String message, String detailMessage)
     at Godot.Bridge.ScriptManagerBridge.GetGlobalClassName(godot_string* scriptPath, godot_string* outBaseType, godot_string* outIconPath, godot_string* outClassName) in /root/godot/modules/mono/glue/GodotSharp/GodotSharp/Core/Bridge/ScriptManagerBridge.cs:line 232
```

对应 [GitHub Issue #97405](https://github.com/godotengine/godot/issues/97405)：**[.Net] Assertion failed when inheriting an external `Node` type**

预计会在 4.4 合并 [Pull Request #97443](https://github.com/godotengine/godot/pull/97443)：**[.Net] Add Reminder for External Node Types**



#### .NET: Failed to unload assemblies

对应 [GitHub Issue #78513](https://github.com/godotengine/godot/issues/78513)：**.NET: Failed to unload assemblies. Please check `<this issue>` for more information.**



**Issue 描述**

程序集重新加载可能会因各种原因失败，通常是因为工具代码中使用的库与程序集卸载不兼容。

卸载失败后，C# 脚本将不可用，直到编辑器重新启动（在极少数情况下，可以在一段时间后通过重新构建程序集来完成卸载）。

如果项目的程序集卸载失败，请查看 [Microsoft 的故障排除说明](https://learn.microsoft.com/en-us/dotnet/standard/assembly/unloadability#troubleshoot-unloadability-issues)，并确保您没有使用已知不兼容的库之一：

- [Json.Net 目前不支持卸载。JamesNK/Newtonsoft.Json#2414](https://github.com/JamesNK/Newtonsoft.Json/issues/2414)
- [System.Text.Json 应该正确支持可卸载的程序集 dotnet/runtime#65323](https://github.com/dotnet/runtime/issues/65323)

如果您知道其他库会导致问题，请发表评论。

如果你的代码没有使用任何库，没有违反任何[准则](https://learn.microsoft.com/en-us/dotnet/standard/assembly/unloadability#troubleshoot-unloadability-issues)，并且你认为卸载被 godot 阻止，请打开一个新问题。已报告的原因有：

- [使用泛型的 C# 脚本注册可能会在 ScriptManagerBridge 中出错 #79519](https://github.com/godotengine/godot/issues/79519)
- [C# 为导出的自定义资源属性分配默认值将导致错误 #80175](https://github.com/godotengine/godot/issues/80175)  [【1】](https://github.com/godotengine/godot/issues/78513#user-content-fn-1-73e9bce364b57c434fbd4d58be6342a3)
- [在 Callable 中捕获变量会阻止程序集卸载 #81903](https://github.com/godotengine/godot/issues/81903)
- [更改 C# 类型重新加载的操作顺序 #90837](https://github.com/godotengine/godot/pull/90837)  [【1】](https://github.com/godotengine/godot/issues/78513#user-content-fn-1-73e9bce364b57c434fbd4d58be6342a3)



**最小复制项目和清理示例**

```c#
using Godot;
using System;

[Tool]
public partial class UnloadingIssuesSample : Node
{
    public override void _Ready()
    {
        // block unloading with a strong handle
        var handle = System.Runtime.InteropServices.GCHandle.Alloc(this);

        // register cleanup code to prevent unloading issues
        System.Runtime.Loader.AssemblyLoadContext.GetLoadContext(System.Reflection.Assembly.GetExecutingAssembly()).Unloading += alc =>
        {
            // handle.Free();
        };
    }
}
```



**我的问题**

**【2024-11-22 16:50】**试了很久，但 C# 继承 F# 的实现方式貌似无法正常被卸载，好像需要尝试用组合而非继承方式实现 Tool

> 参考：https://github.com/godotengine/godot/issues/78513#issuecomment-1937403398

**【2024-11-22 20:55】**事实证明有多方面原因都有嫌疑，但并不需要引入上面的清理示例。

- 首先，响应式编程监听 chunks 的生成会有问题，这个会直接导致卸载不干净，必须改成显式出参…… 不知道和“Callable 中捕获变量会阻止程序集卸载”那个 Issue 是不是相似的？
- 还有一个可能会影响的地方是 F# 每个块的渲染器（`HexChunkRendererFS`）的构造函数传入了 `HexEntry`，持有并保留了它的值？但这个我自己实验的时候是在去掉响应式编程前修改的，改了响应式之后才好，所以不确定这个有没有真正关系。
- 而且出现了 _Ready 中的 GD.Print 打印两遍的神奇现象……这个很奇怪，但是似乎与卸载失败无关。
- 上面说的继承问题也可能是原因之一，做好上面修改后，继承 F# 的实现可以保证第一次不卸载失败，但会报一个 InGame/MainMenuFS 注册脚本重复了所以失败的报错，然后第二次 Build 就会程序集卸载失败了。现在真正完全没问题的办法是 C# `[Tool]` 直接写逻辑，别放在 F# 里再继承。

具体原因很难查，总之尽量避免这些提到的问题，现在应该没问题了。

**【2024-11-23 13:34】**最终还是实现了 **C# 继承 F# 方式**的实现，需要做的就是把程序集（也就是 F# 项目）拆细一点。单独把 F# 编辑器 Tool 相关代码拆分出来，就不会报这个错了。（当然，这次也控制变量法全面测试并证实了：响应式编程订阅逻辑、`MeshInstance3D` 子类 `HexChunkRenderFS` 在构造函数持有外界编辑器工具的变量 `HexEntry` 等做法也必须避免。只不过不太确定如果用 `AssemblyLoadContext` 的 `Unloading` 事件处理逻辑来取消订阅行不行……）



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





