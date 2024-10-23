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

- 玩家查询所属地块
- 查询地块周围联通地块
- 玩家派出一支部队
  - 部队从所属地块出发，扣减地块人口
  - 部队抵达目的地块，增加地块人口，无主地块被占领
- 玩家所属地块人口增长
- 地块存在无主状态



# 待办

## C# 基础

- [x] C# 解决方案（这种设计的作用？和文件路径的关系？）
- [x] C# namespace 命名空间的规范
- [x] 不用 Godot 信号的话，C# 原生的回调怎么写？
- [x] C# 的函数式编程
- [x] 单元测试
- [ ] **【2024-10-21 10:04】**：为啥 Rider psvm 生成 Main(string[] args) 就运行不了，Main() 就可以？
- [ ] **【2024-10-21 10:04】**：为啥 Main 函数里面 nullable foreach 会提示处理空，但自己 F# 互操作函数里就不提示？F# 生成的返回用元组匹配为啥第一个都会变成 nullable？越来越看不懂了属于是

## 架构设计

- [ ] InGameMenu 的代码和 MapBoard 的代码定位应该如何？理解为并行的 Presenter/Controller？还是需要提取一个 GamePresenter/Controller 来统一管理它们？是否应该在架构中理解为和 MarchingLine、TileGUI 一样的组件层级？
- [ ] 估计必须在 Godot 里面创建一个项目全局加载的游戏控制器？场景切换的时候由它来管理？
- [ ] MVC、MVP 或 MVVM 前端和后端的融合边界在哪？ P、C 或者 VM 应该分在哪？
- [ ] Godot 节点创建新节点的逻辑应该怎么写比较好？工厂模式？



# 灵感

## 游戏开发

1. Steam bitburner 游戏 - 代码编辑器语法颜色效果怎么做？（复习编译原理？）
1. Steam 架空地图模拟器（比较单纯的看海游戏）
1. 领土战争、地图战争、[帝国扩张](https://www.bilibili.com/video/BV1d54y1T7ua)（我自己想到的是小时候玩的小游戏[细菌战争](https://www.bilibili.com/video/BV1Jt411g7BG)）、[Territorial.io](https://www.bilibili.com/video/BV1aP411M7UW)、文明时代1

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



# 已完成

## 游戏开发

- [x] 地块使用 kenney cartography-pack 绘制细节
  **【2024/9/9】**

## 架构设计

- [x] Godot 类、C# 主逻辑以及数据层怎么组织架构设计会比较好？
  **【2024/9/14】**：模仿 DDD 思想，参考事件回调思想，加上简化之前仿文明项目里 GDScript 模拟数据库；后续再细化
  - [x] Godot 场景之间的强依赖
    **【2024/9/14】**：有点忘记自己说的啥意思了，应该是指单元测试啥的不好测，这个后续有情况再看
  - [x] 数据层有没有方便内嵌的内存数据库？用 SQLite 高频操作时效率如何？性能评估怎么做？
    **【2024/9/14】**：直接放弃外界数据库方案，直接通过领域静态字典实现类似连表查询操作
  - [x] 代码逻辑得从 Godot 节点获取信息时，数据的流向很别扭（比如去 TileMapLayer 拿地块相邻信息）
    **【2024/9/14】**：目前使用 EventBus 类的信号回调来实现代码反向调用 Godot 强相关显示层的代码

- [x] 思考：应该以继承 Godot 节点的代码为中心设计（比如利用节点树唯一名称、组等功能实现类似依赖注入）；还是以独立的 C# 代码为后端，Godot 强相关代码为显示层前端的思想设计？现在感觉 TileMapLayer 这种每个瓦片都不能独立用代码控制就总是很别扭；还有就是像玩家、地块这种概念需要实现成 Godot 上真正的节点吗？
  感觉最核心的问题就在于：
  1）用独立的 C# 后端的话，在 Godot 编辑器中不可视
  2）部分 Godot 可视的内容却在代码中不公开 API
  就导致如果是 Godot 引擎天然支持的功能还好，要自定义代码逻辑的时候使用就很割裂

  **【2024/9/20 21:52】**：应该以独立 C# 后端为中心，游戏逻辑更为重要，GUI 与画面显示应该作为具体实现依赖于游戏逻辑。TileMapLayer 查询邻接地块的逻辑，也应该可以使用接口封装一下，相当于 TileMapLayer 节点同时实现画面显示和邻接地块查询的两个接口

  - [x] Godot 节点的设计，之前碰到的问题就是父子节点一旦很深的时候，共有父节点层级高很多层级的埋藏很深的两个子节点之间，想要调用函数或者绑定信号会大量透传，一旦发生修改，整个链路上的代码就全得修改。所以需要有种方便全局传递信号或者拿到任意深度节点的方式

    **【2024/9/15 18:01】**：现在采用 MVP 模式，主逻辑都由 Presenter（也就是 MapBoard 类）控制，V、M 之间禁止直接调用，都通过信号回调到 P 来处理。全局调用时目前直接通过静态方法调用暴露的方法或单例，后续可以考虑依赖注入，但现在感觉还没必要复杂化。

  - [x] 分前后端的设计，现在有个问题就是有些逻辑如果在 Godot 节点上，需要后端反向调用的时候怎么处理比较好？按道理这种设计下，逻辑就不能放在前端，但是有些就是节点自带的逻辑拆不了就很尴尬。

    **【2024/9/15】**：好像所有后端对前端的访问都用信号回调就可以了，不过就是迫使 domain 层的类也得继承 GodotObject 有点被污染的感觉。不知道 C# 原生的回调怎么写，有空研究下。

    **【2024/9/15 15:03】**：现在采用 MVP 模式就不存在 domain 被迫继承 GodotObject 的问题了，统一走 EventBus 通知 Presenter。

  - [x] 分前后端的设计 ，还有一个问题就是请求起点应该是什么？Godot 节点各自在自己的游戏循环中 _ready 和 _process 请求吗？

    **【2024/9/20 21:58】**：参考 DDD 的事件触发器来源有：用户操作、外部系统、另一个领域事件、时间的流逝。那么在目前把 C# 后端代码为中心的设计下，Godot 节点的游戏循环可以作为外部系统调用领域服务方法触发，也可以处理成领域事件。估计从统一的游戏控制器/展示器调用会比较好，后续慢慢斟酌。

    - [x] 现在还没引入用户界面，当用户开始通过鼠标点击、键盘输入事件控制时，这个请求起点应该如何管理？

      **【2024/9/20 22:00】**：同上用户操作部分，在 Godot 节点代码中转化

    - [x] 电脑的玩家的 AI 判断似乎适合放到 _process 循环中判断，是需要将 Player 提取成节点呢？还是在主节点中的 _process 中轮询 Player？泛化后的问题是后端的定时任务怎么做？

      **【2024/9/20 22:03】**：游戏逻辑不应该依赖于画面表现层（Godot 节点）。_process 仅仅触发事件，具体逻辑交给游戏逻辑处理。

  - [x] 目前的设计是否对应 MVC 模型？View 对应 Godot 的显示组件，Controller 对应 Godot 节点脚本，Model 对应 domain 里面的类？接口是必须的吗？如果按上面的理解，Godot 节点脚本之间的调用如何组织？

    **【2024/9/15】**：现在存在 M 直接信号传递到 V 的情况，所以应该符合 MVC。但好像还有种 MVP 的模型和我现在代码改造方向类似，即 P = Presenter，M 的回调统一调到 P，再由 P 管理 V。此外还有 MVVM 模型，但 ViewModel 到 View 的双向绑定不知道 Godot 中实现起来是否复杂，目前 MVP 应该够用。但严格来说目前我的实现还是有 V 直接调用到 M 的情况，主要集中于 id 查对应数据，后续看看需不需要统一走 P 查找。
    （MVC、MVP、MVVM 的区别参考：https://www.zhihu.com/question/51798750/answer/2396493957）

- [x] 统计查询类的数据怎么设计？比如玩家统计所有领土、人口数？感觉每次重新查询一遍会不会有问题？通过信号更新的话是否会有异步问题？还得支持排序等功能，需要缓存吗？怎么设计？

  **【2024/9/20 21:57】**: 计划参考 CQRS 模式，把统计查询单独维护数据库表，利用事件更新。排序、缓存等功能这样就可以在查询类的方法中实现

# 暂时搁置

## C# 相关

**【2024/9/14】**：暂时不需要搞的这么复杂。引入的越多，挖的坑越多，越麻烦

- [ ] NuGet（如何拆分项目，分解成引用工具？）
- [ ] Chickensoft 框架学习（Demo 以及各个工具类）
- [ ] EntityFramework Core 的 DbContext 架构设计的最佳实践？工作单元到底是啥？

## 游戏开发

### 过大的战略放置游戏策划

**【2024/9/12】**：刚开始没必要想太多，想这么多做不完也没用……

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

#### 基础资源

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

### 六边形球体

**【2024/9/9】**

最基础游戏界面是六边形球体世界地图，周围是 GUI

- [ ] 3D 六边形球体
  - [x] **@Tool 编辑器在 C# 修改编译后会疯狂打报错日志并卡死，必须重新生成星球才行**
  - [ ] 复刻星球水效果（[[中英双字][Sebastian Lague]编程挑战 地形系统](https://www.bilibili.com/video/BV1kB4y1N7xG)）
  - [ ] 复刻大气层效果（[[中英双字][Sebastian Lague]编程挑战 大气层](https://www.bilibili.com/video/BV1K541187dD)）
  - [ ] 代码重构
  - [ ] 实现类似 TileMap 的材质填涂功能
  - [ ] 寻路功能
  - [ ] 地形编辑器（修改高度等）
