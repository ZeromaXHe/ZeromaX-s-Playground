# [返回主 Markdown](./FSharpForFunAndProfit翻译.md)



# 在工作中使用 F# 的 26 种低风险方法

*Part of the "Low-risk ways to use F# at work" series (*[link](https://fsharpforfunandprofit.com/posts/low-risk-ways-to-use-fsharp-at-work/#series-toc)*)*

您现在就可以开始，无需任何许可
2014年4月20日 这篇文章已有3年多的历史了

https://fsharpforfunandprofit.com/posts/low-risk-ways-to-use-fsharp-at-work/#series-toc

所以你们都对函数式编程感到兴奋，你们一直在业余时间学习 F#，你们咆哮着它有多棒，惹恼了同事，你们渴望在工作中把它用于严肃的事情…

但后来你撞到了砖墙。

你的工作场所有“只使用 C#”的政策，不允许你使用 F#。

如果你在一个典型的企业环境中工作，获得一门新语言的批准将是一个漫长的过程，包括说服你的队友、QA 人员、运维人员、你的老板、你老板的老板，以及大厅里你从未交谈过的神秘家伙。我鼓励你开始这个过程（这对你的经理来说是一个有用的链接），但你仍然不耐烦，想“我现在能做什么？”

另一方面，也许你在一个灵活、随和的地方工作，在那里你可以做你喜欢的事情。

但你是认真的，不想成为那些在 APL 中重写一些关键任务系统，然后消失得无影无踪，给你的替代品留下一些令人费解的神秘代码来维护的人。不，你要确保你没有做任何会影响你团队总线因素的事情。

因此，在这两种情况下，你都想在工作中使用 F#，但你不能（或不想）将其用于核心应用程序代码。

你能做什么？

好吧，别担心！本系列文章将建议您以低风险、增量的方式使用 F#，而不会影响任何关键代码。

## 系列内容

这里列出了 26 种方法，这样你就可以直接找到任何你觉得特别有趣的方法。

### 第 1 部分 - 使用 F# 进行交互式探索和开发

1. 使用 F# 探索 .NET 框架交互式
2. 使用 F# 交互式测试自己的代码
3. 使用 F# 以交互方式使用 Web 服务
4. 使用 F# 以交互方式玩 UI

### 第 2 部分 - 使用 F# 进行开发和 devops 脚本

5. 使用 FAKE 构建和 CI 脚本
6. 一个 F# 脚本，用于检查网站是否响应
7. 将 RSS 提要转换为 CSV 的 F# 脚本
8. 使用 WMI 检查进程统计信息的 F# 脚本
9. 使用 F# 配置和管理云

### 第 3 部分 - 使用 F# 进行测试

10. 使用 F# 编写具有可读名称的单元测试
11. 使用 F# 以编程方式运行单元测试
12. 使用 F# 学习以其他方式编写单元测试
13. 使用 FsCheck 编写更好的单元测试
14. 使用 FsCheck 创建随机虚拟数据
15. 使用 F# 创建模拟
16. 使用 F# 进行自动浏览器测试
17. 使用 F# 进行行为驱动开发

### 第 4 部分 - 使用 F# 执行与数据库相关的任务

18. 使用 F# 替换 LINQpad
19. 使用 F# 对存储过程进行单元测试
20. 使用 FsCheck 生成随机数据库记录
21. 使用 F# 进行简单的 ETL
22. 使用 F# 生成 SQL 代理脚本

### 第 5 部分-使用 F# 的其他有趣方法

23. 使用 F# 进行解析
24. 使用 F# 绘制图表和可视化
25. 使用 F# 访问基于 web 的数据存储
26. 使用 F# 进行数据科学和机器学习
27. （奖金）平衡英国发电站的发电计划

## 开始使用

如果你使用的是 Visual Studio，那么你已经安装了 F#，所以你已经准备好了！无需征得任何人的许可。

如果你在 Mac 或 Linux 上，你将不得不做一些工作，唉（Mac 和 Linux 的说明）。

有两种方法可以交互使用 F#：（1）直接在 F# 交互窗口中键入，或（2）创建 F# 脚本文件（.FSX），然后评估代码片段。

要在 Visual Studio 中使用 F# 交互式窗口，请执行以下操作：

1. 使用 `“功能表” > “视图” > “其他窗口” > “F# 交互”` 显示窗口
2. 键入一个表达式，并使用双分号（；；）告诉解释器您已完成。

例如：

```F#
let x = 1
let y = 2
x + y;;
```

就我个人而言，我更喜欢创建一个脚本文件（`文件 > 新建 > 文件`，然后选择“F#脚本”）并在那里键入代码，因为您可以自动完成和智能感知。

要运行一段代码，只需突出显示并右键单击，或者简单地执行Alt+Enter。



## 使用外部库和 NuGet

大多数代码示例引用了预期位于脚本目录下的外部库。

您可以显式下载或编译这些 DLL，但我认为从命令行使用 NuGet 更简单。

1. 首先，您需要安装 Chocolately（来自 chocolatey.org）
2. 接下来，使用 `cinst nuget.commandline` 安装 NuGet 命令行
3. 最后，转到脚本目录，从命令行安装 NuGet 包。
   例如， `nuget install FSharp.Data -o Packages -ExcludeVersion`
   正如您所看到的，在从脚本中使用 Nuget 包时，我更喜欢从 Nuget 包中排除版本，这样我以后就可以在不破坏现有代码的情况下进行更新。

## 第1部分：使用F#进行交互式探索和开发

F# 有价值的第一个领域是作为一种交互式探索 .NET 库的工具。

之前，为了做到这一点，您可能已经创建了单元测试，然后使用调试器逐步完成它们，以了解发生了什么。但是使用 F#，你不需要这样做，你可以直接运行代码。

让我们来看一些例子。

## 1.使用 F# 交互式探索 .NET 框架

本节的代码可以在 github 上找到。

当我编码时，我经常对 .NET 库如何工作有一些小问题。

例如，以下是我最近用 F# 交互式回答的一些问题：

- 我的自定义 DateTime 格式字符串是否正确？
- XML 序列化如何处理本地 DateTimes 与 UTC DateTimes？
- `GetEnvironmentVariable` 区分大小写吗？

当然，所有这些问题都可以在 MSDN 文档中找到，但也可以通过运行一些简单的 F# 代码片段在几秒钟内得到答案，如下所示。

### 我的自定义DateTime格式字符串是否正确？

我想以自定义格式使用24小时制。我知道它是“h”，但它是大写还是小写的“h”？

```F#
open System
DateTime.Now.ToString("yyyy-MM-dd hh:mm")  // "2014-04-18 01:08"
DateTime.Now.ToString("yyyy-MM-dd HH:mm")  // "2014-04-18 13:09"
```

### XML 序列化如何处理本地 DateTimes 与 UTC DateTimes？

XML 序列化是如何处理日期的？让我们来看看！

```F#
// TIP: sets the current directory to be same as the script directory
System.IO.Directory.SetCurrentDirectory (__SOURCE_DIRECTORY__)

open System

[<CLIMutable>]
type DateSerTest = {Local:DateTime;Utc:DateTime}

let ser = new System.Xml.Serialization.XmlSerializer(typeof<DateSerTest>)

let testSerialization (dt:DateSerTest) =
    let filename = "serialization.xml"
    use fs = new IO.FileStream(filename , IO.FileMode.Create)
    ser.Serialize(fs, o=dt)
    fs.Close()
    IO.File.ReadAllText(filename) |> printfn "%s"

let d = {
    Local = DateTime.SpecifyKind(new DateTime(2014,7,4), DateTimeKind.Local)
    Utc = DateTime.SpecifyKind(new DateTime(2014,7,4), DateTimeKind.Utc)
    }

testSerialization d
```

输出为：

```F#
<DateSerTest xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <Local>2014-07-04T00:00:00+01:00</Local>
  <Utc>2014-07-04T00:00:00Z</Utc>
</DateSerTest>
```

所以我可以看到它使用“Z”表示 UTC 时间。

### GetEnvironmentVariable 区分大小写吗？

这可以用一个简单的片段来回答：

```F#
Environment.GetEnvironmentVariable "ProgramFiles" =
    Environment.GetEnvironmentVariable "PROGRAMFILES"
// answer => true
```

因此，答案是“不区分大小写”。

## 2.使用 F# 交互式测试自己的代码

本节的代码可以在github上找到。

你们不限于玩 .NET 库，当然。有时，测试自己的代码可能非常有用。

为此，只需引用 DLL，然后打开命名空间，如下所示。

```F#
// set the current directory to be same as the script directory
System.IO.Directory.SetCurrentDirectory (__SOURCE_DIRECTORY__)

// pass in the relative path to the DLL
#r @"bin\debug\myapp.dll"

// open the namespace
open MyApp

// do something
MyApp.DoSomething()
```

警告：在旧版本的 F# 中，打开对 DLL 的引用会将其锁定，因此您无法编译它！在这种情况下，在重新编译之前，请务必重置交互式会话以释放锁。在较新版本的 F# 中，DLL 是卷影复制的，并且没有锁。

## 3.使用 F# 以交互方式使用 Web 服务

本节的代码可以在 github 上找到。

如果你想使用 WebAPI 和 Owin 库，你不需要创建可执行文件——你可以单独通过脚本来完成！

这涉及到一点设置，因为您需要许多库 DLL 才能使其工作。

因此，假设您已经设置了 NuGet 命令行（见上文），请转到您的脚本目录，并通过 `nuget install Microsoft.AspNet.WebApi.OwinSelfHost -o Packages -ExcludeVersion` 安装自托管库。

一旦这些库就绪，您就可以使用下面的代码作为简单 WebAPI 应用程序的骨架。

```F#
// sets the current directory to be same as the script directory
System.IO.Directory.SetCurrentDirectory (__SOURCE_DIRECTORY__)

// assumes nuget install Microsoft.AspNet.WebApi.OwinSelfHost has been run
// so that assemblies are available under the current directory
#r @"Packages\Owin\lib\net40\Owin.dll"
#r @"Packages\Microsoft.Owin\lib\net40\Microsoft.Owin.dll"
#r @"Packages\Microsoft.Owin.Host.HttpListener\lib\net40\Microsoft.Owin.Host.HttpListener.dll"
#r @"Packages\Microsoft.Owin.Hosting\lib\net40\Microsoft.Owin.Hosting.dll"
#r @"Packages\Microsoft.AspNet.WebApi.Owin\lib\net45\System.Web.Http.Owin.dll"
#r @"Packages\Microsoft.AspNet.WebApi.Core\lib\net45\System.Web.Http.dll"
#r @"Packages\Microsoft.AspNet.WebApi.Client\lib\net45\System.Net.Http.Formatting.dll"
#r @"Packages\Newtonsoft.Json\lib\net40\Newtonsoft.Json.dll"
#r "System.Net.Http.dll"

open System
open Owin
open Microsoft.Owin
open System.Web.Http
open System.Web.Http.Dispatcher
open System.Net.Http.Formatting

module OwinSelfhostSample =

    /// a record to return
    [<CLIMutable>]
    type Greeting = { Text : string }

    /// A simple Controller
    type GreetingController() =
        inherit ApiController()

        // GET api/greeting
        member this.Get()  =
            {Text="Hello!"}

    /// Another Controller that parses URIs
    type ValuesController() =
        inherit ApiController()

        // GET api/values
        member this.Get()  =
            ["value1";"value2"]

        // GET api/values/5
        member this.Get id =
            sprintf "id is %i" id

        // POST api/values
        member this.Post ([<FromBody>]value:string) =
            ()

        // PUT api/values/5
        member this.Put(id:int, [<FromBody>]value:string) =
            ()

        // DELETE api/values/5
        member this.Delete(id:int) =
            ()

    /// A helper class to store routes, etc.
    type ApiRoute = { id : RouteParameter }

    /// IMPORTANT: When running interactively, the controllers will not be found with error:
    /// "No type was found that matches the controller named 'XXX'."
    /// The fix is to override the ControllerResolver to use the current assembly
    type ControllerResolver() =
        inherit DefaultHttpControllerTypeResolver()

        override this.GetControllerTypes (assembliesResolver:IAssembliesResolver) =
            let t = typeof<System.Web.Http.Controllers.IHttpController>
            System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
            |> Array.filter t.IsAssignableFrom
            :> Collections.Generic.ICollection<Type>

    /// A class to manage the configuration
    type MyHttpConfiguration() as this =
        inherit HttpConfiguration()

        let configureRoutes() =
            this.Routes.MapHttpRoute(
                name= "DefaultApi",
                routeTemplate= "api/{controller}/{id}",
                defaults= { id = RouteParameter.Optional }
                ) |> ignore

        let configureJsonSerialization() =
            let jsonSettings = this.Formatters.JsonFormatter.SerializerSettings
            jsonSettings.Formatting <- Newtonsoft.Json.Formatting.Indented
            jsonSettings.ContractResolver <-
                Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()

        // Here is where the controllers are resolved
        let configureServices() =
            this.Services.Replace(
                typeof<IHttpControllerTypeResolver>,
                new ControllerResolver())

        do configureRoutes()
        do configureJsonSerialization()
        do configureServices()

    /// Create a startup class using the configuration
    type Startup() =

        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        member this.Configuration (appBuilder:IAppBuilder) =
            // Configure Web API for self-host.
            let config = new MyHttpConfiguration()
            appBuilder.UseWebApi(config) |> ignore


// Start OWIN host
do
    // Create server
    let baseAddress = "http://localhost:9000/"
    use app = Microsoft.Owin.Hosting.WebApp.Start<OwinSelfhostSample.Startup>(url=baseAddress)

    // Create client and make some requests to the api
    use client = new System.Net.Http.HttpClient()

    let showResponse query =
        let response = client.GetAsync(baseAddress + query).Result
        Console.WriteLine(response)
        Console.WriteLine(response.Content.ReadAsStringAsync().Result)

    showResponse "api/greeting"
    showResponse "api/values"
    showResponse "api/values/42"

    // for standalone scripts, pause so that you can test via your browser as well
    Console.ReadLine() |> ignore

```

输出如下：

```
StatusCode: 200, ReasonPhrase: 'OK', Version: 1.1, Content: System.Net.Http.StreamContent, Headers:
{
  Date: Fri, 18 Apr 2014 22:29:04 GMT
  Server: Microsoft-HTTPAPI/2.0
  Content-Length: 24
  Content-Type: application/json; charset=utf-8
}
{
  "text": "Hello!"
}
StatusCode: 200, ReasonPhrase: 'OK', Version: 1.1, Content: System.Net.Http.StreamContent, Headers:
{
  Date: Fri, 18 Apr 2014 22:29:04 GMT
  Server: Microsoft-HTTPAPI/2.0
  Content-Length: 29
  Content-Type: application/json; charset=utf-8
}
[
  "value1",
  "value2"
]
StatusCode: 200, ReasonPhrase: 'OK', Version: 1.1, Content: System.Net.Http.StreamContent, Headers:
{
  Date: Fri, 18 Apr 2014 22:29:04 GMT
  Server: Microsoft-HTTPAPI/2.0
  Content-Length: 10
  Content-Type: application/json; charset=utf-8
}
"id is 42"
```

这个例子只是为了证明你可以“开箱即用”地使用 OWIN 和 WebApi 库。

要获得更友好的 F# web 框架，请查看 Suave 或 WebSharper。在 fsharp.org 上有更多的网络内容。

## 4.使用 F# 以交互方式玩 UI

本节的代码可以在 github 上找到。

F# 交互式的另一个用途是在 UI 运行时玩 UI——实时！

这是一个交互式开发 WinForms 屏幕的示例。

```F#
open System.Windows.Forms
open System.Drawing

let form = new Form(Width= 400, Height = 300, Visible = true, Text = "Hello World")
form.TopMost <- true
form.Click.Add (fun _ ->
    form.Text <- sprintf "form clicked at %i" DateTime.Now.Ticks)
form.Show()
```

这是窗口：



这是点击后的窗口，标题栏发生了变化：



现在，让我们添加一个 FlowLayoutPanel 和一个按钮。

```F#
let panel = new FlowLayoutPanel()
form.Controls.Add(panel)
panel.Dock = DockStyle.Fill
panel.WrapContents <- false

let greenButton = new Button()
greenButton.Text <- "Make the background color green"
greenButton.Click.Add (fun _-> form.BackColor <- Color.LightGreen)
panel.Controls.Add(greenButton)
```

现在是窗口：



但是按钮太小了——我们需要将 `AutoSize` 设置为true。

```F#
greenButton.AutoSize <- true
```

好多了！



我们也添加一个黄色按钮：

```F#
let yellowButton = new Button()
yellowButton.Text <- "Make me yellow"
yellowButton.AutoSize <- true
yellowButton.Click.Add (fun _-> form.BackColor <- Color.Yellow)
panel.Controls.Add(yellowButton)
```

但是按钮被切断了，所以让我们改变流向：

```F#
panel.FlowDirection <- FlowDirection.TopDown
```

但现在黄色按钮的宽度与绿色按钮不同，我们可以用 `Dock` 修复：

```F#
yellowButton.Dock <- DockStyle.Fill
```

正如你所看到的，以这种方式交互式地使用布局真的很容易。一旦你对布局逻辑感到满意，你就可以将代码转换回 C#，以用于你的实际应用程序。

这个例子是 WinForms 特有的。当然，对于其他 UI 框架，逻辑会有所不同。

这就是前四个建议。我们还没结束呢！下一篇文章将介绍如何使用 F# 进行开发和 devops 脚本。

# 使用 F# 进行开发和 devops 脚本

*Part of the "Low-risk ways to use F# at work" series (*[link](https://fsharpforfunandprofit.com/posts/low-risk-ways-to-use-fsharp-at-work-2/#series-toc)*)*

在工作中使用F#的26种低风险方法（第 2 部分）
2014年4月21日 这篇文章已有3年多的历史了

https://fsharpforfunandprofit.com/posts/low-risk-ways-to-use-fsharp-at-work-2/

这篇文章是关于在工作中使用 F# 的低风险方法的系列文章的延续。我一直在建议一些方法，你可以在不影响任何关键任务代码的情况下，以低风险、渐进的方式掌握 F#。

在这篇文章中，我们将讨论使用 F# 进行构建和其他开发和 devops 脚本。

如果你是 F# 的新手，你可能想阅读上一篇文章中关于开始使用和使用 NuGet 的部分。

## 系列内容

以下是 26 种方法的快捷方式列表：

### 第 1 部分 - 使用 F# 进行交互式探索和开发

1. 使用 F# 探索 .NET 框架交互式
2. 使用 F# 交互式测试自己的代码
3. 使用 F# 以交互方式使用 Web 服务
4. 使用 F# 以交互方式玩 UI

### 第 2 部分 - 使用 F# 进行开发和 devops 脚本

5. 使用 FAKE 构建和 CI 脚本
6. 一个 F# 脚本，用于检查网站是否响应
7. 将 RSS 提要转换为 CSV 的 F# 脚本
8. 使用 WMI 检查进程统计信息的 F# 脚本
9. 使用 F# 配置和管理云

### 第 3 部分 - 使用 F# 进行测试

10. 使用 F# 编写具有可读名称的单元测试
11. 使用 F# 以编程方式运行单元测试
12. 使用 F# 学习以其他方式编写单元测试
13. 使用 FsCheck 编写更好的单元测试
14. 使用 FsCheck 创建随机虚拟数据
15. 使用 F# 创建模拟
16. 使用 F# 进行自动浏览器测试
17. 使用 F# 进行行为驱动开发

### 第 4 部分 - 使用 F# 执行与数据库相关的任务

18. 使用 F# 替换 LINQpad
19. 使用 F# 对存储过程进行单元测试
20. 使用 FsCheck 生成随机数据库记录
21. 使用 F# 进行简单的 ETL
22. 使用 F# 生成 SQL 代理脚本

### 第 5 部分-使用 F# 的其他有趣方法

23. 使用 F# 进行解析
24. 使用 F# 绘制图表和可视化
25. 使用 F# 访问基于 web 的数据存储
26. 使用 F# 进行数据科学和机器学习
27. （奖金）平衡英国发电站的发电计划

## 第 2 部分：使用 F# 进行开发和 devops 脚本

下一组建议涉及将 F# 用于围绕开发活动的各种脚本：构建、持续集成、部署等。

对于这类小任务，您需要一种带有 REPL 的好脚本语言。你可以使用 PowerShell、ScriptCS，甚至 Python。但为什么不试试 F# 呢？

- F# 感觉像 Python 一样轻量级（很少或没有类型声明）。
- F# 可以访问 .NET 库，包括核心库和通过 NuGet 下载的库。
- F# 具有类型提供程序（比 PowerShell 和 ScriptCS 有很大的优势），可以让您轻松访问各种数据源。
- 所有这些都以简洁、类型安全的方式进行，也具有智能感！

以这种方式使用 F# 将允许您和您的其他开发人员使用 F# 代码来解决实际问题。经理们不应该对这种低风险的方法有任何抵制——在最糟糕的情况下，你可以很容易地转向使用不同的工具。

当然，一个隐藏的议程是，一旦你的开发人员有机会玩 F#，他们就会上瘾，你就离端到端使用 F# 又近了一步！

### 你能用 F# 脚本做什么？

在接下来的几节中，我们将看到 F# 脚本的三个示例：

- 一个 F# 脚本，用于检查网站是否响应
- 将 RSS 提要转换为 CSV 的 F# 脚本
- 使用 WMI 检查进程统计信息的 F# 脚本

当然，您可以将 F# 脚本与几乎任何脚本集成 .NET 库。以下是可以编写脚本的实用程序的其他建议：

- 简单的文件复制、目录遍历和归档（例如日志文件）。如果你正在使用 .NET 4.5，你可以使用新的 [System.IO.Compression.ZipArchive](http://msdn.microsoft.com/en-us/library/vstudio/hh485720.aspx) 类可以在不需要第三方库的情况下进行压缩和解压缩。
- 使用 JSON 进行操作，无论是使用已知格式（使用 JSON 类型提供程序）还是未知格式（使用 JSON 解析器）。
- 使用 Octokit 与 GitHub 交互。
- 从 Excel 中提取数据或在 Excel 中操作数据。F# 支持 COM 进行 Office 自动化，或者您可以使用其中一个类型提供程序或库。
- 用 Math.NET 做数学。
- 网络爬行、链接检查和屏幕抓取。内置的异步工作流和代理使这种“多线程”代码非常易于编写。
- 用 Quartz.NET 安排事情。

如果这些建议激起了你的兴趣，并且你想使用更多的 F#，那么请查看 F# 社区项目页面。这是为 F# 编写的有用库的一个很好的来源，其中大多数都可以很好地与 F# 脚本配合使用。

### 调试 F# 脚本

使用 F# 脚本的一个好处是，您不需要创建整个项目，也不需要启动 Visual Studio。

但是，如果你需要调试一个脚本，而你不在 Visual Studio 中，你能做什么？以下是一些提示：

- 首先，您可以使用printfn对控制台进行久经考验的打印。我通常将其封装在一个简单的日志函数中，这样我就可以用一个标志打开或关闭日志记录。
- 您可以使用FsEye工具在交互式会话中检查和监视变量。
- 最后，您仍然可以使用Visual Studio调试器。诀窍是将调试器附加到fsi.exe进程，然后可以使用调试器。在某一点停下。

## 5.使用 FAKE 构建和 CI 脚本

本节的代码可以在 github 上找到。

让我们从 FAKE 开始，这是一个用 F# 编写的跨平台构建自动化工具，类似于 Ruby 的 Rake。

FAKE 内置了对 git、NuGet、单元测试、Octopus Deploy、Xamarin 等的支持，使开发具有依赖关系的复杂脚本变得容易。

您甚至可以将其与 TFS 一起使用，以避免使用 XAML。

使用 FAKE 而不是像 Rake 这样的东西的一个原因是，你可以标准化 .NET 代码贯穿整个工具链。理论上，您可以使用 NAnt，但在实践中，不用了，谢谢，因为 XML。PSake 也是一种可能性，但我认为它比 FAKE 更复杂。

您还可以使用 FAKE 删除对特定构建服务器的依赖关系。例如，与其使用 TeamCity 的集成来运行测试和其他任务，您可以考虑在 FAKE 中执行它们，这意味着您可以在不安装 TeamCity 的情况下运行完整版本。

这是一个非常简单的 FAKE 脚本示例，取自 FAKE 网站上的一个更详细的示例。

```F#
// Include Fake lib
// Assumes NuGet has been used to fetch the FAKE libraries
#r "packages/FAKE/tools/FakeLib.dll"
open Fake

// Properties
let buildDir = "./build/"

// Targets
Target "Clean" (fun _ ->
    CleanDir buildDir
)

Target "Default" (fun _ ->
    trace "Hello World from FAKE"
)

// Dependencies
"Clean"
  ==> "Default"

// start build
RunTargetOrDefault "Default"
```

语法需要一点时间来适应，但这种努力是值得的。

关于 FAKE 的进一步阅读：

- 迁移到 FAKE。
- Hanselman谈 FAKE。许多评论来自积极使用 FAKE 的人。
- 一个新手用户尝试 FAKE。

## 6.一个 F# 脚本，用于检查网站是否响应

本节的代码可以在 github 上找到。

此脚本检查网站是否以 200 作为响应。例如，这可能是部署后烟雾测试的基础。

```F#
// Requires FSharp.Data under script directory
//    nuget install FSharp.Data -o Packages -ExcludeVersion
#r @"Packages\FSharp.Data\lib\net40\FSharp.Data.dll"
open FSharp.Data

let queryServer uri queryParams =
    try
        let response = Http.Request(uri, query=queryParams, silentHttpErrors = true)
        Some response
    with
    | :? System.Net.WebException as ex -> None

let sendAlert uri message =
    // send alert via email, say
    printfn "Error for %s. Message=%O" uri message

let checkServer (uri,queryParams) =
    match queryServer uri queryParams with
    | Some response ->
        printfn "Response for %s is %O" uri response.StatusCode
        if (response.StatusCode <> 200) then
            sendAlert uri response.StatusCode
    | None ->
        sendAlert uri "No response"

// test the sites
let google = "http://google.com", ["q","fsharp"]
let bad = "http://example.bad", []

[google;bad]
|> List.iter checkServer
```

结果是：

```
Response for http://google.com is 200
Error for http://example.bad. Message=No response
```

请注意，我使用的是 `Fsharp.Data` 中的 Http 实用程序代码。它为 `HttpClient` 提供了一个很好的包装。更多关于 HttpUtilities 的信息，请点击此处。

## 7.将 RSS 提要转换为 CSV 的 F# 脚本

本节的代码可以在 github 上找到。

这里有一个小脚本，它使用 Xml 类型提供程序来解析 RSS 提要（在本例中为 StackOverflow 上的 F# 问题），并将其转换为 CSV 文件以供以后分析。

请注意，RSS解析代码只是一行代码！大部分代码都与编写CSV有关。是的，我本可以使用CSV库（NuGet上有很多），但我想我会保持原样，向你展示它有多简单。

```F#
// sets the current directory to be same as the script directory
System.IO.Directory.SetCurrentDirectory (__SOURCE_DIRECTORY__)

// Requires FSharp.Data under script directory
//    nuget install FSharp.Data -o Packages -ExcludeVersion
#r @"Packages\FSharp.Data\lib\net40\FSharp.Data.dll"
#r "System.Xml.Linq.dll"
open FSharp.Data

type Rss = XmlProvider<"http://stackoverflow.com/feeds/tag/f%23">

// prepare a string for writing to CSV
let prepareStr obj =
    obj.ToString()
     .Replace("\"","\"\"") // replace single with double quotes
     |> sprintf "\"%s\""   // surround with quotes

// convert a list of strings to a CSV
let listToCsv list =
    let combine s1 s2 = s1 + "," + s2
    list
    |> Seq.map prepareStr
    |> Seq.reduce combine

// extract fields from Entry
let extractFields (entry:Rss.Entry) =
    [entry.Title.Value;
     entry.Author.Name;
     entry.Published.ToShortDateString()]

// write the lines to a file
do
    use writer = new System.IO.StreamWriter("fsharp-questions.csv")
    let feed = Rss.GetSample()
    feed.Entries
    |> Seq.map (extractFields >> listToCsv)
    |> Seq.iter writer.WriteLine
    // writer will be closed automatically at the end of this scope
```

请注意，类型提供程序生成智能感知（如下所示），以根据提要的实际内容显示可用属性。太酷了。



结果是这样的：

```
"Optimising F# answer for Euler #4","DropTheTable","18/04/2014"
"How to execute a function, that creates a lot of objects, in parallel?","Lawrence Woodman","01/04/2014"
"How to invoke a user defined function using R Type Provider","Dave","19/04/2014"
"Two types that use themselves","trn","19/04/2014"
"How does function [x] -> ... work","egerhard","19/04/2014"
```

有关 XML 类型提供程序的更多信息，请参阅 FSharp.Data 页面。

## 8.使用 WMI 检查进程统计信息的 F# 脚本

本节的代码可以在 github 上找到。

如果你使用 Windows，能够访问 WMI 非常有用。幸运的是，WMI 有一个 F# 类型提供程序，使用起来很容易。

在这个例子中，我们将获得系统时间，并检查进程的一些统计数据。例如，这在负载测试期间和之后可能很有用。

```F#
// sets the current directory to be same as the script directory
System.IO.Directory.SetCurrentDirectory (__SOURCE_DIRECTORY__)

// Requires FSharp.Management under script directory
//    nuget install FSharp.Management -o Packages -ExcludeVersion
#r @"System.Management.dll"
#r @"Packages\FSharp.Management\lib\net40\FSharp.Management.dll"
#r @"Packages\FSharp.Management\lib\net40\FSharp.Management.WMI.dll"

open FSharp.Management

// get data for the local machine
type Local = WmiProvider<"localhost">
let data = Local.GetDataContext()

// get the time and timezone on the machine
let time = data.Win32_UTCTime |> Seq.head
let tz = data.Win32_TimeZone |> Seq.head
printfn "Time=%O-%O-%O %O:%O:%O" time.Year time.Month time.Day time.Hour time.Minute time.Second
printfn "Timezone=%O" tz.StandardName

// find the "explorer" process
let explorerProc =
    data.Win32_PerfFormattedData_PerfProc_Process
    |> Seq.find (fun proc -> proc.Name.Contains("explorer") )

// get stats about it
printfn "ElapsedTime=%O" explorerProc.ElapsedTime
printfn "ThreadCount=%O" explorerProc.ThreadCount
printfn "HandleCount=%O" explorerProc.HandleCount
printfn "WorkingSetPeak=%O" explorerProc.WorkingSetPeak
printfn "PageFileBytesPeak=%O" explorerProc.PageFileBytesPeak
```

输出如下：

```
Time=2014-4-20 14:2:35
Timezone=GMT Standard Time
ElapsedTime=2761906
ThreadCount=67
HandleCount=3700
WorkingSetPeak=168607744
PageFileBytesPeak=312565760
```

同样，使用类型提供者意味着您可以获得智能感知（如下所示）。对于数百个 WMI 选项非常有用。



有关 WMI 类型提供程序的更多信息，请单击此处。

## 9.使用 F# 配置和管理云

一个值得特别提及的领域是使用F#配置和管理云服务。fsharp.org 上的云页面有许多有用的链接。

对于简单的脚本编写，Fog 是 Azure 的一个很好的包装器。

例如，要上传 blob，代码就这么简单：

```F#
UploadBlob "testcontainer" "testblob" "This is a test" |> ignore
```

或添加和接收消息：

```F#
AddMessage "testqueue" "This is a test message" |> ignore

let result = GetMessages "testqueue" 20 5
for m in result do
    DeleteMessage "testqueue" m
```

使用 F# 的特别之处在于，你可以在微脚本中完成它——你不需要任何繁重的工具。

## 摘要

我希望你觉得这些建议有用。如果你在实践中应用它们，请在评论中告诉我。

接下来：使用 F# 进行测试。

# 使用 F# 进行测试

*Part of the "Low-risk ways to use F# at work" series (*[link](https://fsharpforfunandprofit.com/posts/low-risk-ways-to-use-fsharp-at-work-3/#series-toc)*)*

在工作中使用 F# 的 26 种低风险方法（第 3 部分）
2014年4月22日 这篇文章已有3年多的历史了

https://fsharpforfunandprofit.com/posts/low-risk-ways-to-use-fsharp-at-work-3/

这篇文章是之前关于在工作中使用 F# 的低风险和增量方法的系列文章的延续——如何在不影响任何关键任务代码的情况下，以低风险、增量的方式使用 F#？

在这篇文章中，我们将讨论使用 F# 进行测试。

## 系列内容

在继续讨论帖子内容之前，以下是 26 种方法的完整列表：

### 第 1 部分 - 使用 F# 进行交互式探索和开发

1. 使用 F# 探索 .NET 框架交互式
2. 使用 F# 交互式测试自己的代码
3. 使用 F# 以交互方式使用 Web 服务
4. 使用 F# 以交互方式玩 UI

### 第 2 部分 - 使用 F# 进行开发和 devops 脚本

5. 使用 FAKE 构建和 CI 脚本
6. 一个 F# 脚本，用于检查网站是否响应
7. 将 RSS 提要转换为 CSV 的 F# 脚本
8. 使用 WMI 检查进程统计信息的 F# 脚本
9. 使用 F# 配置和管理云

### 第 3 部分 - 使用 F# 进行测试

10. 使用 F# 编写具有可读名称的单元测试
11. 使用 F# 以编程方式运行单元测试
12. 使用 F# 学习以其他方式编写单元测试
13. 使用 FsCheck 编写更好的单元测试
14. 使用 FsCheck 创建随机虚拟数据
15. 使用 F# 创建模拟
16. 使用 F# 进行自动浏览器测试
17. 使用 F# 进行行为驱动开发

### 第 4 部分 - 使用 F# 执行与数据库相关的任务

18. 使用 F# 替换 LINQpad
19. 使用 F# 对存储过程进行单元测试
20. 使用 FsCheck 生成随机数据库记录
21. 使用 F# 进行简单的 ETL
22. 使用 F# 生成 SQL 代理脚本

### 第 5 部分-使用 F# 的其他有趣方法

23. 使用 F# 进行解析
24. 使用 F# 绘制图表和可视化
25. 使用 F# 访问基于 web 的数据存储
26. 使用 F# 进行数据科学和机器学习
27. （奖金）平衡英国发电站的发电计划

## 第 3 部分-使用 F# 进行测试

如果你想在不接触核心代码的情况下开始用F#编写有用的代码，编写测试是一个很好的开始。

F# 不仅具有更紧凑的语法，还具有许多不错的特性，例如“双反引号”语法，使测试名称更具可读性。

与本系列中的所有建议一样，我认为这是一个低风险的选择。测试方法往往很短，所以几乎任何人都可以在不深入理解 F# 的情况下阅读它们。在最坏的情况下，你可以很容易地将它们移植回 C#。

## 10.使用 F# 编写具有可读名称的单元测试

本节的代码可以在 github 上找到。

与 C# 一样，F# 也可以使用 NUnit、MsUnit、xUnit 等标准框架编写标准单元测试。

这是一个为 NUnit 编写的测试类示例。

```F#
[<TestFixture>]
type TestClass() =

    [<Test>]
    member this.When2IsAddedTo2Expect4() =
        Assert.AreEqual(4, 2+2)
```

如您所见，有一个类具有 `TestFixture` 属性，还有一个公共 void 方法具有 `Test` 属性。一切都很标准。

但是当你使用 F# 而不是 C# 时，你会得到一些不错的额外功能。首先，您可以使用双反引号语法来创建更易读的名称，其次，您可以在模块而不是类中使用 `let` 绑定函数，这简化了代码。

```F#
[<Test>]
let ``When 2 is added to 2 expect 4``() =
    Assert.AreEqual(4, 2+2)
```

双回溯语法使测试结果更容易阅读。以下是具有标准类名的测试输出：

```
TestClass.When2IsAddedTo2Expect4
Result: Success
```

与使用更友好名称的输出相比：

```
MyUnitTests.When 2 is added to 2 expect 4
Result: Success
```

因此，如果你想编写非程序员可以访问的测试名称，那就试试 F# 吧！

## 11.使用 F# 以编程方式运行单元测试

通常，您可能希望以编程方式运行单元测试。这可能是由于各种原因，例如使用自定义过滤器，或进行自定义日志记录，或不想在测试机器上安装 NUnit。

一种简单的方法是使用 Fuchu 库，该库允许您直接组织测试，特别是参数化测试，而无需任何复杂的测试属性。

这里有一个例子：

```F#
let add1 x = x + 1

// a simple test using any assertion framework:
// Fuchu's own, Nunit, FsUnit, etc
let ``Assert that add1 is x+1`` x _notUsed =
   NUnit.Framework.Assert.AreEqual(x+1, add1 x)

// a single test case with one value
let simpleTest =
   testCase "Test with 42" <|
     ``Assert that add1 is x+1`` 42

// a parameterized test case with one param
let parameterizedTest i =
   testCase (sprintf "Test with %i" i) <|
     ``Assert that add1 is x+1`` i
```

您可以使用以下代码在 F# 交互式中直接运行这些测试： `run simpleTest`。

您还可以将这些测试合并到一个或多个列表中，或分层列表中：

```F#
// create a hierarchy of tests
// mark it as the start point with the "Tests" attribute
[<Fuchu.Tests>]
let tests =
   testList "Test group A" [
      simpleTest
      testList "Parameterized 1..10" ([1..10] |> List.map parameterizedTest)
      testList "Parameterized 11..20" ([11..20] |> List.map parameterizedTest)
   ]
```

上面的代码可以在 github 上找到。

最后，有了 Fuchu，测试组件就变成了自己的测试运行器。只需将程序集设置为控制台应用程序而不是库，并将以下代码添加到 `program.fs` 文件中：

```F#
[<EntryPoint>]
let main args =
    let exitCode = defaultMainThisAssembly args

    Console.WriteLine("Press any key")
    Console.ReadLine() |> ignore

    // return the exit code
    exitCode
```

更多关于 Fuchu 的信息，请点击此处。

### 使用 NUnit 测试运行器

如果你确实需要使用现有的测试运行器（如 NUnit），那么编写一个简单的脚本来实现这一点非常简单。

下面我用 `Nunit.Runners` 包做了一个小例子。

好吧，这可能不是 F# 最令人兴奋的用法，但它确实展示了 F# 创建 `NUnit.Core.EventListener` 接口的“对象表达式”语法，所以我想把它留作演示。

```F#
// sets the current directory to be same as the script directory
System.IO.Directory.SetCurrentDirectory (__SOURCE_DIRECTORY__)

// Requires Nunit.Runners under script directory
//    nuget install NUnit.Runners -o Packages -ExcludeVersion

#r @"Packages\NUnit.Runners\tools\lib\nunit.core.dll"
#r @"Packages\NUnit.Runners\tools\lib\nunit.core.interfaces.dll"

open System
open NUnit.Core

module Setup =
    open System.Reflection
    open NUnit.Core
    open System.Diagnostics.Tracing

    let configureTestRunner path (runner:TestRunner) =
        let package = TestPackage("MyPackage")
        package.Assemblies.Add(path) |> ignore
        runner.Load(package) |> ignore

    let createListener logger =

        let replaceNewline (s:string) =
            s.Replace(Environment.NewLine, "")

        // This is an example of F#'s "object expression" syntax.
        // You don't need to create a class to implement an interface
        {new NUnit.Core.EventListener
            with

            member this.RunStarted(name:string, testCount:int) =
                logger "Run started "

            member this.RunFinished(result:TestResult ) =
                logger ""
                logger "-------------------------------"
                result.ResultState
                |> sprintf "Overall result: %O"
                |> logger

            member this.RunFinished(ex:Exception) =
                ex.StackTrace
                |> replaceNewline
                |> sprintf "Exception occurred: %s"
                |> logger

            member this.SuiteFinished(result:TestResult) = ()
            member this.SuiteStarted(testName:TestName) = ()

            member this.TestFinished(result:TestResult)=
                result.ResultState
                |> sprintf "Result: %O"
                |> logger

            member this.TestOutput(testOutput:TestOutput) =
                testOutput.Text
                |> replaceNewline
                |> logger

            member this.TestStarted(testName:TestName) =
                logger ""

                testName.FullName
                |> replaceNewline
                |> logger

            member this.UnhandledException(ex:Exception) =
                ex.StackTrace
                |> replaceNewline
                |> sprintf "Unhandled exception occurred: %s"
                |> logger
            }


// run all the tests in the DLL
do
    let dllPath = @".\bin\MyUnitTests.dll"

    CoreExtensions.Host.InitializeService();

    use runner = new NUnit.Core.SimpleTestRunner()
    Setup.configureTestRunner dllPath runner
    let logger = printfn "%s"
    let listener = Setup.createListener logger
    let result = runner.Run(listener, TestFilter.Empty, true, LoggingThreshold.All)

    // if running from the command line, wait for user input
    Console.ReadLine() |> ignore

    // if running from the interactive session, reset session before recompiling MyUnitTests.dll
```

上面的代码可以在 github 上找到。

## 12.使用 F# 学习以其他方式编写单元测试

上面的单元测试代码对我们所有人来说都很熟悉，但还有其他编写测试的方法。学习不同风格的代码是为你的技能库添加一些新技术并扩展你的思维的好方法，所以让我们快速看看其中的一些。

首先是 FsUnit，它用更流畅、更地道的方法（自然语言和管道）取代了 `Assert`。

以下是一个片段：

```F#
open NUnit.Framework
open FsUnit

let inline add x y = x + y

[<Test>]
let ``When 2 is added to 2 expect 4``() =
    add 2 2 |> should equal 4

[<Test>]
let ``When 2.0 is added to 2.0 expect 4.01``() =
    add 2.0 2.0 |> should (equalWithin 0.1) 4.01

[<Test>]
let ``When ToLower(), expect lowercase letters``() =
    "FSHARP".ToLower() |> should startWith "fs"
```

上面的代码可以在 github 上找到。

Unquote 使用了一种非常不同的方法。Unquote 方法是将任何 F# 表达式封装在 F# 引号中，然后对其进行求值。如果测试表达式抛出异常，测试将失败，不仅会打印异常，还会打印到异常点的每一步。这些信息可能会让你更深入地了解断言失败的原因。

这里有一个非常简单的例子：

```F#
open Swensen.Unquote

[<Test>]
let ``When 2 is added to 2 expect 4``() =
    test <@ 2 + 2 = 4 @>
```

还有一些快捷运算符，如 `=!` 和 `>!` 这让你可以更简单地编写测试——任何地方都没有断言！

```F#
open Swensen.Unquote

[<Test>]
let ``2 + 2 is 4``() =
   let result = 2 + 2
   result =! 4

[<Test>]
let ``2 + 2 is bigger than 5``() =
   let result = 2 + 2
   result >! 5
```

上面的代码可以在 github 上找到。

## 13.使用 FsCheck 编写更好的单元测试

本节的代码可以在 github 上找到。

假设我们编写了一个将数字转换为罗马数字的函数，我们想为它创建一些测试用例。

我们可以开始编写这样的测试：

```F#
[<Test>]
let ``Test that 497 is CDXCVII``() =
    arabicToRoman 497 |> should equal "CDXCVII"
```

但这种方法的问题在于，它只测试了一个非常具体的例子。可能还有一些我们没有想到的边缘情况。

一个更好的方法是找到对所有情况都必须正确的东西。然后，我们可以创建一个测试，检查这个东西（“属性”）在所有情况下都是真的，或者至少是一个大的随机子集。

例如，在罗马数字的例子中，我们可以说一个属性是“所有罗马数字最多有一个‘V’字符”或“所有罗马数码最多有三个‘X’字符”。然后，我们可以构建测试来检查此属性是否真实。

这就是 FsCheck 可以提供帮助的地方。FsCheck 是一个专门为这种基于属性的测试而设计的框架。它是用F#编写的，但同样适用于测试 C# 代码。

那么，让我们看看如何将 FsCheck 用于罗马数字。

首先，我们定义了一些我们希望适用于所有罗马数字的属性。

```F#
let maxRepetitionProperty ch count (input:string) =
    let find = String.replicate (count+1) ch
    input.Contains find |> not

// a property that holds for all roman numerals
let ``has max rep of one V`` roman =
    maxRepetitionProperty "V" 1 roman

// a property that holds for all roman numerals
let ``has max rep of three Xs`` roman =
    maxRepetitionProperty "X" 3 roman
```

有了这个，我们创建了以下测试：

1. 创建一个适合传递给 FsCheck 的属性检查器函数。
2. 使用 `Check.Quick` 生成数百个随机测试用例并将其发送到属性检查器的功能。

```F#
open FsCheck

[<Test>]
let ``Test that roman numerals have no more than one V``() =
    let property num =
        // convert the number to roman and check the property
        arabicToRoman num |> ``has max rep of one V``

    Check.QuickThrowOnFailure property

[<Test>]
let ``Test that roman numerals have no more than three Xs``() =
    let property num =
        // convert the number to roman and check the property
        arabicToRoman num |> ``has max rep of three Xs``

    Check.QuickThrowOnFailure property
```

以下是测试结果。你可以看到，已经测试了100个随机数，而不仅仅是一个。

```
Test that roman numerals have no more than one V
   Ok, passed 100 tests.

Test that roman numerals have no more than three Xs
   Ok, passed 100 tests.
```

如果我们将测试更改为“测试罗马数字不超过两个 X”，则测试结果为 false，如下所示：

```
Falsifiable, after 33 tests

30
```

换句话说，在生成 33 个不同的输入后，FsCheck 正确地找到了一个不符合所需属性的数字（30）。很不错的！

### 在实践中使用 FsCheck

并非所有情况都有可以用这种方式测试的属性，但你可能会发现它比你想象的更常见。

例如，基于属性的测试对于“算法”代码特别有用。这里有几个例子：

- 如果你反转一个列表，然后再次反转，你会得到原始列表。
- 如果你对一个整数进行因式分解，然后将这些因子相乘，你就得到了原始数字。

但即使在Boring Line Of Business Applications™中，您也可能会发现基于属性的测试也有一席之地。例如，以下是一些可以表示为属性的东西：

- **往返**。例如，如果将记录保存到数据库中，然后重新加载，则记录的字段应保持不变。同样，如果你序列化然后反序列化某些东西，你应该得到原始的东西。
- **不变量**。如果将产品添加到销售订单中，则单行的总和应与订单总额相同。或者，每页的字数总和应该是整本书的字数总和。更一般地说，如果你通过两条不同的路径计算事物，你应该得到相同的答案（monoid 同态（homomorphisms）！）
- **舍入**。如果你在食谱中添加配料，配料百分比的总和（精度为 2 位）应始终为 100%。大多数分区逻辑都需要类似的规则，如份额、税务计算等（例如 DDD 书中的“份额派”示例）。确保在这种情况下正确进行四舍五入是 FsCheck 的亮点。

请参阅此 SO 问题以了解其他想法。

FsCheck 对于重构也非常有用，因为一旦你相信测试非常彻底，你就可以自信地进行调整和优化。

更多关于 FsCheck 的链接：

- 我写了一篇关于基于属性的测试的介绍，以及一篇关于选择基于属性测试的属性的后续文章。
- FsCheck 文档。
- 一篇关于在实践中使用 FsCheck 的文章。
- 我在罗马数字 kata 上的帖子提到了 FsCheck。

有关基于属性的测试的更多信息，请查看有关 QuickCheck 的文章和视频。

- John Hughes 的 QuickCheck简介（PDF）
- 关于使用 QuickCheck 在 Riak 中查找错误的精彩演讲（另一个版本）（视频）

## 14.使用 FsCheck 创建随机虚拟数据

本节的代码可以在 github 上找到。

除了进行测试外，FsCheck 还可以用于创建随机虚拟数据。

例如，下面是生成随机客户的完整代码。

当您将其与 SQL 类型提供程序（稍后讨论）或 CSV 编写器结合使用时，您可以在数据库或 CSV 文件中轻松生成数千行随机客户。或者，您可以将其与 JSON 类型提供程序一起使用，以调用 web 服务来测试验证逻辑或负载测试。

（不要担心不理解代码——这个示例只是为了向你展示它有多容易！）

```F#
// domain objects
type EmailAddress = EmailAddress of string
type PhoneNumber = PhoneNumber of string
type Customer = {
    name: string
    email: EmailAddress
    phone: PhoneNumber
    birthdate: DateTime
    }

// a list of names to sample
let possibleNames = [
    "Georgianne Stephan"
    "Sharolyn Galban"
    "Beatriz Applewhite"
    "Merissa Cornwall"
    "Kenneth Abdulla"
    "Zora Feliz"
    "Janeen Strunk"
    "Oren Curlee"
    ]

// generate a random name by picking from the list at random
let generateName() =
    FsCheck.Gen.elements possibleNames

// generate a random EmailAddress by combining random users and domains
let generateEmail() =
    let userGen = FsCheck.Gen.elements ["a"; "b"; "c"; "d"; "e"; "f"]
    let domainGen = FsCheck.Gen.elements ["gmail.com"; "example.com"; "outlook.com"]
    let makeEmail u d = sprintf "%s@%s" u d |> EmailAddress
    FsCheck.Gen.map2 makeEmail userGen domainGen

// generate a random PhoneNumber
let generatePhone() =
    let areaGen = FsCheck.Gen.choose(100,999)
    let n1Gen = FsCheck.Gen.choose(1,999)
    let n2Gen = FsCheck.Gen.choose(1,9999)
    let makeNumber area n1 n2 = sprintf "(%03i)%03i-%04i" area n1 n2 |> PhoneNumber
    FsCheck.Gen.map3 makeNumber areaGen n1Gen n2Gen

// generate a random birthdate
let generateDate() =
    let minDate = DateTime(1920,1,1).ToOADate() |> int
    let maxDate = DateTime(2014,1,1).ToOADate() |> int
    let oaDateGen = FsCheck.Gen.choose(minDate,maxDate)
    let makeDate oaDate = float oaDate |> DateTime.FromOADate
    FsCheck.Gen.map makeDate oaDateGen

// a function to create a customer
let createCustomer name email phone birthdate =
    {name=name; email=email; phone=phone; birthdate=birthdate}

// use applicatives to create a customer generator
let generateCustomer =
    createCustomer
    <!> generateName()
    <*> generateEmail()
    <*> generatePhone()
    <*> generateDate()

[<Test>]
let printRandomCustomers() =
    let size = 0
    let count = 10
    let data = FsCheck.Gen.sample size count generateCustomer

    // print it
    data |> List.iter (printfn "%A")
```

以下是结果示例：

```
{name = "Georgianne Stephan";
 email = EmailAddress "d@outlook.com";
 phone = PhoneNumber "(420)330-2080";
 birthdate = 11/02/1976 00:00:00;}

{name = "Sharolyn Galban";
 email = EmailAddress "e@outlook.com";
 phone = PhoneNumber "(579)781-9435";
 birthdate = 01/04/2011 00:00:00;}

{name = "Janeen Strunk";
 email = EmailAddress "b@gmail.com";
 phone = PhoneNumber "(265)405-6619";
 birthdate = 21/07/1955 00:00:00;}
```

## 15.使用 F# 创建模拟

如果你使用 F# 为用 C# 编写的代码编写测试用例，你可能想为接口创建模拟和存根。

在 C# 中，您可能会使用 Moq 或 NSubstitute。在 F# 中，您可以使用对象表达式直接创建接口，也可以使用 Foq库。

两者都很容易做到，而且在某种程度上与 Moq 相似。

以下是 C# 中的一些 Moq 代码：

```c#
// Moq Method
var mock = new Mock<IFoo>();
mock.Setup(foo => foo.DoSomething("ping")).Returns(true);
var instance = mock.Object;

// Moq Matching Arguments:
mock.Setup(foo => foo.DoSomething(It.IsAny<string>())).Returns(true);

// Moq Property
mock.Setup(foo => foo.Name ).Returns("bar");
```

以下是 F# 中的等效 Foq 代码：

```F#
// Foq Method
let mock =
    Mock<IFoo>()
        .Setup(fun foo -> <@ foo.DoSomething("ping") @>).Returns(true)
        .Create()

// Foq Matching Arguments
mock.Setup(fun foo -> <@ foo.DoSomething(any()) @>).Returns(true)

// Foq Property
mock.Setup(fun foo -> <@ foo.Name @>).Returns("bar")
```

有关 F# 中 mocking 的更多信息，请参阅：

- F# 作为单元测试语言
- 与 Foq 一起 mocking
- 用 F# 测试和模拟你的 C# 代码

你需要模拟外部服务，如 SMTP，有一个有趣的工具叫做 mountebank，它很容易在 F# 中交互。

## 16.使用 F# 进行自动浏览器测试

除了单元测试，您还应该进行某种自动化的 web 测试，使用 Selenium 或 WatiN 驱动浏览器。

但是你应该用什么语言来写自动化呢？Ruby？python？C#？我想你知道答案！

为了让你的生活更轻松，可以尝试使用 Canopy，这是一个基于 Selenium 构建并用 F# 编写的 web 测试框架。他们的网站声称“快速学习。即使你从未做过 UI 自动化，也不知道 F#。”我倾向于相信他们。

以下是 Canopy 网站上的一个片段。正如你所看到的，代码简单易懂。

此外，FAKE 与 Canopy 集成，因此您可以在 CI 构建中运行自动浏览器测试。

```F#
//start an instance of the firefox browser
start firefox

//this is how you define a test
"taking canopy for a spin" &&& fun _ ->
    //go to url
    url "http://lefthandedgoat.github.io/canopy/testpages/"

    //assert that the element with an id of 'welcome' has
    //the text 'Welcome'
    "#welcome" == "Welcome"

    //assert that the element with an id of 'firstName' has the value 'John'
    "#firstName" == "John"

    //change the value of element with
    //an id of 'firstName' to 'Something Else'
    "#firstName" << "Something Else"

    //verify another element's value, click a button,
    //verify the element is updated
    "#button_clicked" == "button not clicked"
    click "#button"
    "#button_clicked" == "button clicked"

//run all tests
run()
```

## 17. 使用 F# 进行行为驱动开发

本节的代码可以在 github 上找到。

如果你不熟悉行为驱动开发（BDD），那么你的想法是，你以一种人类可读和可执行的方式表达需求。

编写这些测试的标准格式（Gherkin）使用 Given/When/Then 语法——这里有一个例子：

```
Feature: Refunded or replaced items should be returned to stock

Scenario 1: Refunded items should be returned to stock
	Given a customer buys a black jumper
	And I have 3 black jumpers left in stock
	When they return the jumper for a refund
	Then I should have 4 black jumpers in stock
```

如果您已经在 .NET 使用 BDD，您可能正在使用 SpecFlow 或类似的软件。

您应该考虑使用 TickSpec，因为与所有 F# 一样，语法要轻得多。

例如，这是上面场景的完整实现。

```F#
type StockItem = { Count : int }

let mutable stockItem = { Count = 0 }

let [<Given>] ``a customer buys a black jumper`` () =
    ()

let [<Given>] ``I have (.*) black jumpers left in stock`` (n:int) =
    stockItem <- { stockItem with Count = n }

let [<When>] ``they return the jumper for a refund`` () =
    stockItem <- { stockItem with Count = stockItem.Count + 1 }

let [<Then>] ``I should have (.*) black jumpers in stock`` (n:int) =
    let passed = (stockItem.Count = n)
    Assert.True(passed)
```

C# 等价物有更多的混乱，缺乏双反引号语法真的很伤人：

```c#
[Given(@"a customer buys a black jumper")]
public void GivenACustomerBuysABlackJumper()
{
   // code
}

[Given(@"I have (.*) black jumpers left in stock")]
public void GivenIHaveNBlackJumpersLeftInStock(int n)
{
   // code
}
```

示例取自 TickSpec 网站。

## F# 测试总结

当然，您可以将我们迄今为止看到的所有测试技术结合起来（如本幻灯片所示）：

- 单元测试（FsUnit，Unquote）和基于属性的测试（FsCheck）。
- 由浏览器自动化（Canopy）驱动的 BDD（TickSpec）编写的自动验收测试（或至少是烟雾测试）。
- 这两种类型的测试都在每个构建上运行（使用 FAKE）。

有很多关于测试自动化的建议，你会发现很容易将其他语言的概念移植到这些 F# 工具中。玩得高兴！

# 使用 F# 执行与数据库相关的任务

*Part of the "Low-risk ways to use F# at work" series (*[link](https://fsharpforfunandprofit.com/posts/low-risk-ways-to-use-fsharp-at-work-4/#series-toc)*)*

在工作中使用 F# 的 26 种低风险方法（第 4 部分）
2014年4月23日 这篇文章已有3年多的历史了

https://fsharpforfunandprofit.com/posts/low-risk-ways-to-use-fsharp-at-work-4/

这篇文章是之前关于在工作中使用 F# 的低风险和增量方法的系列文章的延续。

在这篇文章中，我们将看到F#在与数据库相关的任务中是如何出乎意料地有用的。

## 系列内容

在继续讨论帖子内容之前，以下是 26 种方法的完整列表：

### 第 1 部分 - 使用 F# 进行交互式探索和开发

1. 使用 F# 探索 .NET 框架交互式
2. 使用 F# 交互式测试自己的代码
3. 使用 F# 以交互方式使用 Web 服务
4. 使用 F# 以交互方式玩 UI

### 第 2 部分 - 使用 F# 进行开发和 devops 脚本

5. 使用 FAKE 构建和 CI 脚本
6. 一个 F# 脚本，用于检查网站是否响应
7. 将 RSS 提要转换为 CSV 的 F# 脚本
8. 使用 WMI 检查进程统计信息的 F# 脚本
9. 使用 F# 配置和管理云

### 第 3 部分 - 使用 F# 进行测试

10. 使用 F# 编写具有可读名称的单元测试
11. 使用 F# 以编程方式运行单元测试
12. 使用 F# 学习以其他方式编写单元测试
13. 使用 FsCheck 编写更好的单元测试
14. 使用 FsCheck 创建随机虚拟数据
15. 使用 F# 创建模拟
16. 使用 F# 进行自动浏览器测试
17. 使用 F# 进行行为驱动开发

### 第 4 部分 - 使用 F# 执行与数据库相关的任务

18. 使用 F# 替换 LINQpad
19. 使用 F# 对存储过程进行单元测试
20. 使用 FsCheck 生成随机数据库记录
21. 使用 F# 进行简单的 ETL
22. 使用 F# 生成 SQL 代理脚本

### 第 5 部分-使用 F# 的其他有趣方法

23. 使用 F# 进行解析
24. 使用 F# 绘制图表和可视化
25. 使用 F# 访问基于 web 的数据存储
26. 使用 F# 进行数据科学和机器学习
27. （奖金）平衡英国发电站的发电计划

## 第 4 部分 - 使用 F# 执行与数据库相关的任务

下一组建议都是关于使用数据库，特别是 MS SQL Server。

关系数据库是大多数应用程序的关键部分，但大多数团队对这些数据库的管理方式与其他开发任务不同。

例如，你知道有多少团队对他们的存储过程进行单元测试？

或者他们的 ETL 工作？

或者使用存储在源代码管理中的非 SQL 脚本语言生成 T-SQL 管理脚本和其他样板？

这就是 F# 可以超越其他脚本语言，甚至超越 T-SQL 本身的地方。

- F# 中的数据库类型提供程序使您能够为测试和管理创建简单、简短的脚本，额外的好处是…
- 这些脚本经过类型检查，如果数据库模式发生变化，它们将在编译时失败，这意味着…
- 整个过程在构建和持续集成过程中非常有效，这反过来意味着…
- 您对数据库相关代码非常有信心！

我们将看几个例子来演示我在说什么：

- 单元测试存储程序
- 使用 FsCheck 生成随机记录
- 用 F# 做简单的 ETL
- 正在生成 SQL 代理脚本

### 正在设置

本节的代码可以在 github 上找到。在那里，有一些 SQL 脚本来创建我将在这些示例中使用的示例数据库、表和存储过程。

那么，要运行这些示例，您需要在本地或可访问的地方运行 SQL Express 或 SQL Server，并运行相关的安装脚本。

### 哪种类型的供应商？

F# 有许多 SQL 类型提供程序，请参阅 fsharp.org 数据访问页面。对于这些示例，我将使用作为 `FSharp.Data.TypeProviders` DLL 一部分的  [`SqlDataConnection` 类型提供程序](http://msdn.microsoft.com/en-us/library/hh361033.aspx)。它在幕后使用 SqlMetal，因此仅适用于 SQL Server 数据库。

SQLProvider 项目是另一个不错的选择——它支持 MySql、SQLite 和其他非微软数据库。

## 18.使用 F# 替换 LINQPad

本节的代码可以在 github 上找到。

LINQPad 是一个对数据库进行查询的好工具，也是 C#、VB、F# 代码的通用草稿栏。

你可以使用 F# interactive 来做许多相同的事情——你可以得到查询、自动补全等，就像 LINQPad 一样。

例如，这里有一个统计具有特定电子邮件域的客户的方法。

```F#
[<Literal>]
let connectionString = "Data Source=localhost; Initial Catalog=SqlInFsharp; Integrated Security=True;"

type Sql = SqlDataConnection<connectionString>
let db = Sql.GetDataContext()

// find the number of customers with a gmail domain
query {
    for c in db.Customer do
    where (c.Email.EndsWith("gmail.com"))
    select c
    count
    }
```

如果你想看看生成了什么 SQL 代码，当然可以打开日志：

```F#
// optional, turn logging on
db.DataContext.Log <- Console.Out
```

此查询的记录输出为：

```
SELECT COUNT(*) AS [value]
FROM [dbo].[Customer] AS [t0]
WHERE [t0].[Email] LIKE @p0
-- @p0: Input VarChar (Size = 8000; Prec = 0; Scale = 0) [%gmail.com]
```

您还可以执行更复杂的操作，例如使用子查询。以下是 MSDN 上的一个示例：

请注意，作为一种函数式方法，查询是很好的和可组合的。

```F#
// Find students who have signed up at least one course.
query {
    for student in db.Student do
    where (query { for courseSelection in db.CourseSelection do
                   exists (courseSelection.StudentID = student.StudentID) })
    select student
}
```

如果 SQL 引擎不支持某些函数，如正则表达式，并且假设数据大小不太大，您可以将数据流式传输出去，并在 F# 中进行处理。

```F#
// find the most popular domain for people born in each decade
let getDomain email =
    Regex.Match(email,".*@(.*)").Groups.[1].Value

let getDecade (birthdate:Nullable<DateTime>) =
    if birthdate.HasValue then
        birthdate.Value.Year / 10  * 10 |> Some
    else
        None

let topDomain list =
    list
    |> Seq.distinct
    |> Seq.head
    |> snd

db.Customer
|> Seq.map (fun c -> getDecade c.Birthdate, getDomain c.Email)
|> Seq.groupBy fst
|> Seq.sortBy fst
|> Seq.map (fun (decade, group) -> (decade,topDomain group))
|> Seq.iter (printfn "%A")
```

正如您从上面的代码中看到的，在F#中进行处理的好处是，您可以单独定义辅助函数并轻松地将它们连接在一起。

## 19.使用 F# 对存储过程进行单元测试

本节的代码可以在 github 上找到。

现在，让我们看看如何使用类型提供程序使为存储过程创建单元测试变得非常容易。

首先，我创建了一个助手模块（我将称之为 `DbLib`）来设置连接并提供共享实用程序函数，如 `resetDatabase`，它将在每次测试之前调用。

```F#
module DbLib

[<Literal>]
let connectionString = "Data Source=localhost; Initial Catalog=SqlInFsharp;Integrated Security=True;"
type Sql = SqlDataConnection<connectionString>

let removeExistingData (db:DbContext) =
    let truncateTable name =
        sprintf "TRUNCATE TABLE %s" name
        |> db.DataContext.ExecuteCommand
        |> ignore

    ["Customer"; "CustomerImport"]
    |> List.iter truncateTable

let insertReferenceData (db:DbContext) =
    [ "US","United States";
      "GB","United Kingdom" ]
    |> List.iter (fun (code,name) ->
        let c = new Sql.ServiceTypes.Country()
        c.IsoCode <- code;  c.CountryName <- name
        db.Country.InsertOnSubmit c
        )
    db.DataContext.SubmitChanges()

// removes all data and restores db to known starting point
let resetDatabase() =
    use db = Sql.GetDataContext()
    removeExistingData db
    insertReferenceData db
```

现在，我可以使用 NUnit say 编写一个单元测试，就像任何其他单元测试一样。

假设我们有 `Customer` 表，以及一个名为 `up_Customer_Upsert` 的 sproc，该 sproc 根据传入的客户 id 是否为 null，插入新客户或更新现有客户。

以下是测试的样子：

```F#
[<Test>]
let ``When upsert customer called with null id, expect customer created with new id``() =
    DbLib.resetDatabase()
    use db = DbLib.Sql.GetDataContext()

    // create customer
    let newId = db.Up_Customer_Upsert(Nullable(),"Alice","x@example.com",Nullable())

    // check new id
    Assert.Greater(newId,0)

    // check one customer exists
    let customerCount = db.Customer |> Seq.length
    Assert.AreEqual(1,customerCount)
```

请注意，由于设置成本高昂，我在测试中执行了多个断言。如果你觉得这太难看，可以重构它！

以下是一个测试更新工作的方法：

```F#
[<Test>]
let ``When upsert customer called with existing id, expect customer updated``() =
    DbLib.resetDatabase()
    use db = DbLib.Sql.GetDataContext()

    // create customer
    let custId = db.Up_Customer_Upsert(Nullable(),"Alice","x@example.com",Nullable())

    // update customer
    let newId = db.Up_Customer_Upsert(Nullable custId,"Bob","y@example.com",Nullable())

    // check id hasn't changed
    Assert.AreEqual(custId,newId)

    // check still only one customer
    let customerCount = db.Customer |> Seq.length
    Assert.AreEqual(1,customerCount)

    // check customer columns are updated
    let customer = db.Customer |> Seq.head
    Assert.AreEqual("Bob",customer.Name)
```

还有一个，用于检查异常情况：

```F#
[<Test>]
let ``When upsert customer called with blank name, expect validation error``() =
    DbLib.resetDatabase()
    use db = DbLib.Sql.GetDataContext()

    try
        // try to create customer will a blank name
        db.Up_Customer_Upsert(Nullable(),"","x@example.com",Nullable()) |> ignore
        Assert.Fail("expecting a SqlException")
    with
    | :? System.Data.SqlClient.SqlException as ex ->
        Assert.That(ex.Message,Is.StringContaining("@Name"))
        Assert.That(ex.Message,Is.StringContaining("blank"))
```

正如你所看到的，整个过程非常简单。

这些测试可以作为持续集成脚本的一部分进行编译和运行。最棒的是，如果数据库模式与代码不同步，那么测试甚至无法编译！

## 20.使用 FsCheck 生成随机数据库记录

本节的代码可以在 github 上找到。

正如我在前面的示例中所示，您可以使用 FsCheck 生成随机数据。在这种情况下，我们将使用它在数据库中生成随机记录。

假设我们有一个 `CustomerImport` 表，定义如下。（我们将在下一节 ETL 中使用此表）

```sql
CREATE TABLE dbo.CustomerImport (
	CustomerId int NOT NULL IDENTITY(1,1)
	,FirstName varchar(50) NOT NULL
	,LastName varchar(50) NOT NULL
	,EmailAddress varchar(50) NOT NULL
	,Age int NULL

	CONSTRAINT PK_CustomerImport PRIMARY KEY CLUSTERED (CustomerId)
	)
```

使用与之前相同的代码，我们可以生成 `CustomerImport` 的随机实例。

```F#
[<Literal>]
let connectionString = "Data Source=localhost; Initial Catalog=SqlInFsharp; Integrated Security=True;"

type Sql = SqlDataConnection<connectionString>

// a list of names to sample
let possibleFirstNames =
    ["Merissa";"Kenneth";"Zora";"Oren"]
let possibleLastNames =
    ["Applewhite";"Feliz";"Abdulla";"Strunk"]

// generate a random name by picking from the list at random
let generateFirstName() =
    FsCheck.Gen.elements possibleFirstNames

let generateLastName() =
    FsCheck.Gen.elements possibleLastNames

// generate a random email address by combining random users and domains
let generateEmail() =
    let userGen = FsCheck.Gen.elements ["a"; "b"; "c"; "d"; "e"; "f"]
    let domainGen = FsCheck.Gen.elements ["gmail.com"; "example.com"; "outlook.com"]
    let makeEmail u d = sprintf "%s@%s" u d
    FsCheck.Gen.map2 makeEmail userGen domainGen
```

到现在为止，一直都还不错。

现在我们来看看 `age` 列，它可以为空。这意味着我们不能生成随机 `int`s，而是必须生成随机的 `Nullable<int>`s。这就是类型检查真正有用的地方——编译器迫使我们考虑到这一点。因此，为了确保我们覆盖了所有基础，我们将在二十次中生成一次空值。

```F#
// Generate a random nullable age.
// Note that because age is nullable,
// the compiler forces us to take that into account
let generateAge() =
    let nonNullAgeGenerator =
        FsCheck.Gen.choose(1,99)
        |> FsCheck.Gen.map (fun age -> Nullable age)
    let nullAgeGenerator =
        FsCheck.Gen.constant (Nullable())

    // 19 out of 20 times choose a non null age
    FsCheck.Gen.frequency [
        (19,nonNullAgeGenerator)
        (1,nullAgeGenerator)
        ]
```

总而言之…

```F#
// a function to create a customer
let createCustomerImport first last email age =
    let c = new Sql.ServiceTypes.CustomerImport()
    c.FirstName <- first
    c.LastName <- last
    c.EmailAddress <- email
    c.Age <- age
    c //return new record

// use applicatives to create a customer generator
let generateCustomerImport =
    createCustomerImport
    <!> generateFirstName()
    <*> generateLastName()
    <*> generateEmail()
    <*> generateAge()
```

一旦我们有了随机生成器，我们就可以获取任意数量的记录，并使用类型提供程序插入它们。

在下面的代码中，我们将生成 10000 条记录，以 1000 条记录的批量访问数据库。

```F#
let insertAll() =
    use db = Sql.GetDataContext()

    // optional, turn logging on or off
    // db.DataContext.Log <- Console.Out
    // db.DataContext.Log <- null

    let insertOne counter customer =
        db.CustomerImport.InsertOnSubmit customer
        // do in batches of 1000
        if counter % 1000 = 0 then
            db.DataContext.SubmitChanges()

    // generate the records
    let count = 10000
    let generator = FsCheck.Gen.sample 0 count generateCustomerImport

    // insert the records
    generator |> List.iteri insertOne
    db.DataContext.SubmitChanges() // commit any remaining
```

最后，让我们行动起来，定下时间。

```
#time
insertAll()
#time
```

它不如使用 BCP 快，但足以进行测试。例如，创建上述 10000 条记录只需要几秒钟。

我想强调的是，这是一个独立的脚本，而不是一个繁重的二进制文件，所以很容易根据需要进行调整和运行。

当然，您可以获得脚本化方法的所有好处，例如能够将其存储在源代码管理中、跟踪更改等。

## 21.使用 F# 进行简单的 ETL

本节的代码可以在 github 上找到。

假设您需要将数据从一个表传输到另一个表，但这并不是一个完全简单的复制，因为您需要进行一些映射和转换。

这是一种典型的 ETL（提取/转换/加载）情况，大多数人会选择 SSIS。

但对于某些情况，例如一次性导入，并且数量不大，您可以使用 F#。我们来看看。

假设我们正在将数据导入一个主表，如下所示：

```SQL
CREATE TABLE dbo.Customer (
	CustomerId int NOT NULL IDENTITY(1,1)
	,Name varchar(50) NOT NULL
	,Email varchar(50) NOT NULL
	,Birthdate datetime NULL
	)
```

但是我们从中导入的系统具有不同的格式，如下所示：

```sql
CREATE TABLE dbo.CustomerImport (
	CustomerId int NOT NULL IDENTITY(1,1)
	,FirstName varchar(50) NOT NULL
	,LastName varchar(50) NOT NULL
	,EmailAddress varchar(50) NOT NULL
	,Age int NULL
	)
```

作为此次导入的一部分，我们将不得不：

- 将 `FirstName` 和 `LastName` 列连接到一个 `Name` 列中
- 将 `EmailAddress` 列映射到 `Email` 列
- 根据 `Age` 计算 `Birthdate`
- 现在我将跳过 `CustomerId`——希望我们在实践中没有使用 IDENTITY 列。

第一步是定义一个将源记录映射到目标记录的函数。在这种情况下，我们称之为 `makeTargetCustomer`。

这里有一些代码：

```F#
[<Literal>]
let sourceConnectionString =
    "Data Source=localhost; Initial Catalog=SqlInFsharp; Integrated Security=True;"

[<Literal>]
let targetConnectionString =
    "Data Source=localhost; Initial Catalog=SqlInFsharp; Integrated Security=True;"

type SourceSql = SqlDataConnection<sourceConnectionString>
type TargetSql = SqlDataConnection<targetConnectionString>

let makeName first last =
    sprintf "%s %s" first last

let makeBirthdate (age:Nullable<int>) =
    if age.HasValue then
        Nullable (DateTime.Today.AddYears(-age.Value))
    else
        Nullable()

let makeTargetCustomer (sourceCustomer:SourceSql.ServiceTypes.CustomerImport) =
    let targetCustomer = new TargetSql.ServiceTypes.Customer()
    targetCustomer.Name <- makeName sourceCustomer.FirstName sourceCustomer.LastName
    targetCustomer.Email <- sourceCustomer.EmailAddress
    targetCustomer.Birthdate <- makeBirthdate sourceCustomer.Age
    targetCustomer // return it
```

有了这个转换，剩下的代码就很容易了，我们只需从源代码读取并写入目标代码。

```F#
let transferAll() =
    use sourceDb = SourceSql.GetDataContext()
    use targetDb = TargetSql.GetDataContext()

    let insertOne counter customer =
        targetDb.Customer.InsertOnSubmit customer
        // do in batches of 1000
        if counter % 1000 = 0 then
            targetDb.DataContext.SubmitChanges()
            printfn "...%i records transferred" counter

    // get the sequence of source records
    sourceDb.CustomerImport
    // transform to a target record
    |>  Seq.map makeTargetCustomer
    // and insert
    |>  Seq.iteri insertOne

    targetDb.DataContext.SubmitChanges() // commit any remaining
    printfn "Done"
```

因为这些是序列操作，一次只有一条记录在内存中（LINQ提交缓冲区除外），所以即使是大型数据集也可以处理。

要查看它的使用情况，首先使用刚才讨论的虚拟数据脚本插入多条记录，然后按如下方式运行传输：

```F#
#time
transferAll()
#time
```

同样，传输 10000 条记录只需要几秒钟。

再说一次，这是一个独立的脚本——它是创建简单 ETL 作业的一种非常轻量级的方法。

## 22.使用 F# 生成 SQL 代理脚本

对于最后一个与数据库相关的建议，让我建议从代码生成 SQL 代理脚本的想法。

在任何规模可观的商店里，你都可能有数百或数千个 SQL 代理作业。在我看来，这些都应该存储为脚本文件，并在配置/构建系统时加载到数据库中。

唉，开发、测试和生产环境之间往往存在微妙的差异：连接字符串、授权、警报、日志配置等。

这自然会导致试图保留一个脚本的三个不同副本的问题，这反过来又会让你思考：为什么不有一个脚本并为环境参数化它呢？

但现在你正在处理大量丑陋的 SQL 代码！创建 SQL 代理作业的脚本通常有数百行长，并不是真正为手工维护而设计的。

F# 来救援！

在 F# 中，创建一些简单的记录类型来存储生成和配置作业所需的所有数据非常容易。

例如，在下面的脚本中：

- 我创建了一个名为 `Step` 的联合类型，可以存储 `Package`、`Executable`、`Powershell` 等。
- 这些步骤类型中的每一个都有自己的特定属性，因此一个 `Package` 都有一个名称和变量，等等。
- `JobInfo` 由名称和 `Step` 列表组成。
- 代理脚本由 `JobInfo` 和一组与环境相关的全局属性（如数据库、共享文件夹位置等）生成。

```F#
let thisDir = __SOURCE_DIRECTORY__
System.IO.Directory.SetCurrentDirectory (thisDir)

#load @"..\..\SqlAgentLibrary.Lib.fsx"

module MySqlAgentJob =

    open SqlAgentLibrary.Lib.SqlAgentLibrary

    let PackageFolder = @"\shared\etl\MyJob"

    let step1 = Package {
        Name = "An SSIS package"
        Package = "AnSsisPackage.dtsx"
        Variables =
            [
            "EtlServer", "EtlServer"
            "EtlDatabase", "EtlDatabase"
            "SsisLogServer", "SsisLogServer"
            "SsisLogDatabase", "SsisLogDatabase"
            ]
        }

    let step2 = Package {
        Name = "Another SSIS package"
        Package = "AnotherSsisPackage.dtsx"
        Variables =
            [
            "EtlServer", "EtlServer2"
            "EtlDatabase", "EtlDatabase2"
            "SsisLogServer", "SsisLogServer2"
            "SsisLogDatabase", "SsisLogDatabase2"
            ]
        }

    let jobInfo = {
        JobName = "My SqlAgent Job"
        JobDescription = "Copy data from one place to another"
        JobCategory = "ETL"
        Steps =
            [
            step1
            step2
            ]
        StepsThatContinueOnFailure = []
        JobSchedule = None
        JobAlert = None
        JobNotification = None
        }

    let generate globals =
        writeAgentScript globals jobInfo

module DevEnvironment =

    let globals =
        [
        // global
        "Environment", "DEV"
        "PackageFolder", @"\shared\etl\MyJob"
        "JobServer", "(local)"

        // General variables
        "JobName", "Some packages"
        "SetStartFlag", "2"
        "SetEndFlag", "0"

        // databases
        "Database", "mydatabase"
        "Server",  "localhost"
        "EtlServer", "localhost"
        "EtlDatabase", "etl_config"

        "SsisLogServer", "localhost"
        "SsisLogDatabase", "etl_config"
        ] |> Map.ofList


    let generateJob() =
        MySqlAgentJob.generate globals

DevEnvironment.generateJob()
```

我不能分享实际的 F# 代码，但我想你明白了。创建起来很简单。

一旦我们有了这些 .FSX 文件，我们可以批量生成真正的 SQL 代理脚本，然后将它们部署到适当的服务器上。

下面是一个可能从 .FSX 文件自动生成的 SQL 代理脚本示例。

正如您所看到的，它是一个布局和格式都很好的 T-SQL 脚本。这个想法是，DBA 可以审查它，并确信没有魔法发生，因此愿意接受它作为输入。

另一方面，维护这样的脚本是有风险的。直接编辑 SQL 代码可能存在风险。最好使用类型检查（更简洁）的F#代码，而不是非类型化 T-SQL！

```sql
USE [msdb]
GO

-- =====================================================
-- Script that deletes and recreates the SQL Agent job 'My SqlAgent Job'
--
-- The job steps are:
-- 1) An SSIS package
     -- {Continue on error=false}
-- 2) Another SSIS package
     -- {Continue on error=false}

-- =====================================================


-- =====================================================
-- Environment is DEV
--
-- The other global variables are:
-- Database = mydatabase
-- EtlDatabase = etl_config
-- EtlServer = localhost
-- JobName = My SqlAgent Job
-- JobServer = (local)
-- PackageFolder = \\shared\etl\MyJob\
-- Server = localhost
-- SetEndFlag = 0
-- SetStartFlag = 2
-- SsisLogDatabase = etl_config
-- SsisLogServer = localhost

-- =====================================================


-- =====================================================
-- Create job
-- =====================================================

-- ---------------------------------------------
-- Delete Job if it exists
-- ---------------------------------------------
IF  EXISTS (SELECT job_id FROM msdb.dbo.sysjobs_view WHERE name = 'My SqlAgent Job')
BEGIN
    PRINT 'Deleting job "My SqlAgent Job"'
    EXEC msdb.dbo.sp_delete_job @job_name='My SqlAgent Job', @delete_unused_schedule=0
END

-- ---------------------------------------------
-- Create Job
-- ---------------------------------------------

BEGIN TRANSACTION
DECLARE @ReturnCode INT
SELECT @ReturnCode = 0

-- ---------------------------------------------
-- Create Category if needed
-- ---------------------------------------------
IF NOT EXISTS (SELECT name FROM msdb.dbo.syscategories WHERE name='ETL' AND category_class=1)
BEGIN
    PRINT 'Creating category "ETL"'
    EXEC @ReturnCode = msdb.dbo.sp_add_category @class=N'JOB', @type=N'LOCAL', @name='ETL'
    IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
END

-- ---------------------------------------------
-- Create Job
-- ---------------------------------------------

DECLARE @jobId BINARY(16)
PRINT 'Creating job "My SqlAgent Job"'
EXEC @ReturnCode =  msdb.dbo.sp_add_job @job_name='My SqlAgent Job',
        @enabled=1,
        @category_name='ETL',
        @owner_login_name=N'sa',
        @description='Copy data from one place to another',
        @job_id = @jobId OUTPUT

IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback


PRINT '-- ---------------------------------------------'
PRINT 'Create step 1: "An SSIS package"'
PRINT '-- ---------------------------------------------'
DECLARE @Step1_Name nvarchar(50) = 'An SSIS package'
DECLARE @Step1_Package nvarchar(170) = 'AnSsisPackage.dtsx'
DECLARE @Step1_Command nvarchar(1700) =
    '/FILE "\\shared\etl\MyJob\AnSsisPackage.dtsx"' +
    ' /CHECKPOINTING OFF' +
    ' /SET "\Package.Variables[User::SetFlag].Value";"2"' +
    ' /SET "\Package.Variables[User::JobName].Value";""' +
    ' /SET "\Package.Variables[User::SourceServer].Value";"localhost"' +
    ' /SET "\Package.Variables[User::SourceDatabaseName].Value";"etl_config"' +

    ' /REPORTING E'

EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=@Step1_Name,
        @step_id=1,
        @on_success_action=3,
        @on_fail_action=2,
        @subsystem=N'SSIS',
        @command=@Step1_Command

        IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback


PRINT '-- ---------------------------------------------'
PRINT 'Create step 2: "Another SSIS Package"'
PRINT '-- ---------------------------------------------'
DECLARE @Step2_Name nvarchar(50) = 'Another SSIS Package'
DECLARE @Step2_Package nvarchar(170) = 'AnotherSsisPackage.dtsx'
DECLARE @Step2_Command nvarchar(1700) =
    '/FILE "\\shared\etl\MyJob\AnotherSsisPackage.dtsx.dtsx"' +
    ' /CHECKPOINTING OFF' +
    ' /SET "\Package.Variables[User::EtlServer].Value";"localhost"' +
    ' /SET "\Package.Variables[User::EtlDatabase].Value";"etl_config"' +
    ' /SET "\Package.Variables[User::SsisLogServer].Value";"localhost"' +
    ' /SET "\Package.Variables[User::SsisLogDatabase].Value";"etl_config"' +

    ' /REPORTING E'

EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=@Step2_Name,
        @step_id=2,
        @on_success_action=3,
        @on_fail_action=2,
        @subsystem=N'SSIS',
        @command=@Step2_Command

        IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback

    -- ---------------------------------------------
-- Job Schedule
-- ---------------------------------------------


-- ----------------------------------------------
-- Job Alert
-- ----------------------------------------------


-- ---------------------------------------------
-- Set start step
-- ---------------------------------------------

EXEC @ReturnCode = msdb.dbo.sp_update_job @job_id = @jobId, @start_step_id = 1
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback

-- ---------------------------------------------
-- Set server
-- ---------------------------------------------


EXEC @ReturnCode = msdb.dbo.sp_add_jobserver @job_id = @jobId, @server_name = '(local)'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback


PRINT 'Done!'

COMMIT TRANSACTION
GOTO EndSave
QuitWithRollback:
    IF (@@TRANCOUNT > 0) ROLLBACK TRANSACTION
EndSave:
GO
```

## 摘要

我希望这组建议能为 F# 的用途带来新的启示。

在我看来，简洁的语法、轻量级脚本（无二进制文件）和 SQL 类型提供程序的结合使 F# 在数据库相关任务中非常有用。

请发表评论，让我知道你的想法。

# 在工作中使用 F# 的其他有趣方法

*Part of the "Low-risk ways to use F# at work" series (*[link](https://fsharpforfunandprofit.com/posts/low-risk-ways-to-use-fsharp-at-work-5/#series-toc)*)*

在工作中使用 F# 的 26 种低风险方法（第 5 部分）
2014年4月24日 这篇文章已有3年多的历史了

https://fsharpforfunandprofit.com/posts/low-risk-ways-to-use-fsharp-at-work-5/

这篇文章是关于在工作中使用 F# 的低风险和增量方法的系列文章的结论。

最后，我们将看看 F# 可以帮助您完成各种边缘开发任务的更多方法，而不会影响任何核心或关键任务代码。

## 系列内容

在继续讨论帖子内容之前，以下是 26 种方法的完整列表：

### 第 1 部分 - 使用 F# 进行交互式探索和开发

1. 使用 F# 探索 .NET 框架交互式
2. 使用 F# 交互式测试自己的代码
3. 使用 F# 以交互方式使用 Web 服务
4. 使用 F# 以交互方式玩 UI

### 第 2 部分 - 使用 F# 进行开发和 devops 脚本

5. 使用 FAKE 构建和 CI 脚本
6. 一个 F# 脚本，用于检查网站是否响应
7. 将 RSS 提要转换为 CSV 的 F# 脚本
8. 使用 WMI 检查进程统计信息的 F# 脚本
9. 使用 F# 配置和管理云

### 第 3 部分 - 使用 F# 进行测试

10. 使用 F# 编写具有可读名称的单元测试
11. 使用 F# 以编程方式运行单元测试
12. 使用 F# 学习以其他方式编写单元测试
13. 使用 FsCheck 编写更好的单元测试
14. 使用 FsCheck 创建随机虚拟数据
15. 使用 F# 创建模拟
16. 使用 F# 进行自动浏览器测试
17. 使用 F# 进行行为驱动开发

### 第 4 部分 - 使用 F# 执行与数据库相关的任务

18. 使用 F# 替换 LINQpad
19. 使用 F# 对存储过程进行单元测试
20. 使用 FsCheck 生成随机数据库记录
21. 使用 F# 进行简单的 ETL
22. 使用 F# 生成 SQL 代理脚本

### 第 5 部分-使用 F# 的其他有趣方法

23. 使用 F# 进行解析
24. 使用 F# 绘制图表和可视化
25. 使用 F# 访问基于 web 的数据存储
26. 使用 F# 进行数据科学和机器学习
27. （奖金）平衡英国发电站的发电计划

## 第 5 部分：在核心外使用 F# 的其他方法

恐怕最后一组建议有点杂乱无章。这些内容不适合早期的帖子，主要涉及使用 F# 进行分析和数据处理。

## 23.使用 F# 进行解析

令人惊讶的是，在常规开发过程中，您需要解析某些内容的频率有多高：在空格处拆分字符串、读取 CSV 文件、在模板中进行替换、为网络爬虫查找 HTML 链接、解析 URI 中的查询字符串等等。

F# 是一种 ML 派生语言，非常适合解析各种任务，从简单的正则表达式到成熟的解析器。

当然，有许多现成的库用于常见任务，但有时你需要自己编写。一个很好的例子是 TickSpec，我们之前看到的 BDD 框架。

TickSpec 需要解析 Given/When/Then 的所谓“Gherkin”格式。与其创建对另一个库的依赖，我想 Phil 用几百行编写自己的解析器会更容易（也更有趣）。您可以在此处查看部分源代码。

另一种可能值得编写自己的解析器的情况是，当你有一些复杂的系统时，比如规则引擎，它的 XML 配置格式很糟糕。与其手动编辑配置，您可以创建一种非常简单的领域特定语言（DSL），对其进行解析，然后转换为复杂的 XML。

Martin Fowler 在他关于 DSL 的书中给出了一个例子，一个被解析以创建状态机的 DSL。这是该 DSL 的 F# 实现。

对于更复杂的解析任务，我强烈建议使用 FParsec，它非常适合这类任务。例如，它已被用于解析 FogCreek、CSV 文件、国际象棋符号和负载测试场景的自定义 DSL 的搜索查询。

## 24.使用 F# 绘制图表和可视化

一旦你解析或分析了一些东西，如果你能直观地显示结果，而不是作为充满数据的表格，那就太好了。

例如，在上一篇文章中，我使用 F# 结合 GraphViz 创建了依赖关系图。您可以在下面看到一个示例：



生成图表本身的代码很短，只有大约 60 行，你可以在这里看到。

作为 GraphViz 的替代方案，您还可以考虑使用 FSGraph。

对于更多的数学或以数据为中心的可视化，有许多好的库：

- FSharp。与 F# 脚本很好地集成的桌面可视化图表。
- FsPlot  用于 HTML 中的交互式可视化。
- VegaHub，一个用于使用 Vega 的 F# 库
- F# 用于可视化

最后，还有 800 磅的大猩猩——Excel。

如果可用，使用 Excel 的内置功能是很好的。F#脚本在Excel中运行良好。

您可以在 Excel 中绘制图表，在 Excel 中打印函数，为了获得更强大的功能和集成，您可以使用 FCell 和 Excel DNA 项目。

## 25.使用 F# 访问基于 web 的数据存储

网络上有很多公共数据，只是等着被拉下来并被爱。借助类型提供者的魔力，F#是将这些web规模的数据存储直接集成到您的工作流程中的好选择。

现在，我们将看看两个数据存储：Freebase 和世界银行。更多信息将很快提供——有关最新信息，请参阅 fsharp.org 数据访问页面。

## Freebase

本节的代码可以在 github 上找到。

Freebase 是一个大型的协作知识库，是从许多来源收集的结构化数据的在线集合。

要开始，只需像我们之前看到的那样在类型提供程序 DLL 中链接即可。

网站被节流，所以如果你经常使用 API 密钥，你可能需要它（这里有 API 详细信息）

```F#
// sets the current directory to be same as the script directory
System.IO.Directory.SetCurrentDirectory (__SOURCE_DIRECTORY__)

// Requires FSharp.Data under script directory
//    nuget install FSharp.Data -o Packages -ExcludeVersion
#r @"Packages\FSharp.Data\lib\net40\FSharp.Data.dll"
open FSharp.Data

// without a key
let data = FreebaseData.GetDataContext()

// with a key
(*
[<Literal>]
let FreebaseApiKey = "<enter your freebase-enabled google API key here>"
type FreebaseDataWithKey = FreebaseDataProvider<Key=FreebaseApiKey>
let data = FreebaseDataWithKey.GetDataContext()
*)
```

加载类型提供程序后，您可以开始提问，例如…

“美国总统是谁？”

```F#
data.Society.Government.``US Presidents``
|> Seq.map (fun p ->  p.``President number`` |> Seq.head, p.Name)
|> Seq.sortBy fst
|> Seq.iter (fun (n,name) -> printfn "%s was number %i" name n )
```

结果：

```
George Washington was number 1
John Adams was number 2
Thomas Jefferson was number 3
James Madison was number 4
James Monroe was number 5
John Quincy Adams was number 6
...
Ronald Reagan was number 40
George H. W. Bush was number 41
Bill Clinton was number 42
George W. Bush was number 43
Barack Obama was number 44
```

只需四行代码就足够了！

“卡萨布兰卡获得了哪些奖项？”

```F#
data.``Arts and Entertainment``.Film.Films.IndividualsAZ.C.Casablanca.``Awards Won``
|> Seq.map (fun award -> award.Year, award.``Award category``.Name)
|> Seq.sortBy fst
|> Seq.iter (fun (year,name) -> printfn "%s -- %s" year name)
```

结果是：

```
1943 -- Academy Award for Best Director
1943 -- Academy Award for Best Picture
1943 -- Academy Award for Best Screenplay
```

这就是 Freebase。有很多好的信息，既有用又无聊。

更多关于如何使用 Freebase 类型提供程序的信息。

## 使用 Freebase 生成真实的测试数据

我们已经了解了如何使用 FsCheck 生成测试数据。好吧，你也可以通过从 Freebase 获取数据来获得同样的效果，这使得数据更加真实。

Kit Eason 在推特上展示了如何做到这一点，下面是一个基于他的代码的示例：

```F#
let randomElement =
    let random = new System.Random()
    fun (arr:string array) -> arr.[random.Next(arr.Length)]

let surnames =
    FreebaseData.GetDataContext().Society.People.``Family names``
    |> Seq.truncate 1000
    |> Seq.map (fun name -> name.Name)
    |> Array.ofSeq

let firstnames =
    FreebaseData.GetDataContext().Society.Celebrities.Celebrities
    |> Seq.truncate 1000
    |> Seq.map (fun celeb -> celeb.Name.Split([|' '|]).[0])
    |> Array.ofSeq

// generate ten random people and print
type Person = {Forename:string; Surname:string}
Seq.init 10 ( fun _ ->
    {Forename = (randomElement firstnames);
     Surname = (randomElement surnames) }
     )
|> Seq.iter (printfn "%A")
```

结果是：

```
{Forename = "Kelly"; Surname = "Deasy";}
{Forename = "Bam"; Surname = "Brézé";}
{Forename = "Claire"; Surname = "Sludden";}
{Forename = "Kenneth"; Surname = "Klütz";}
{Forename = "Étienne"; Surname = "Defendi";}
{Forename = "Billy"; Surname = "Paleti";}
{Forename = "Alix"; Surname = "Nuin";}
{Forename = "Katherine"; Surname = "Desporte";}
{Forename = "Jasmine";  Surname = "Belousov";}
{Forename = "Josh";  Surname = "Kramarsic";}
```

## 世界银行

本节的代码可以在 github 上找到。

Freebase 的另一个极端是世界银行开放数据，它拥有来自世界各地的大量详细的经济和社会信息。

设置与 Freebase 相同，但不需要 API 密钥。

```F#
// sets the current directory to be same as the script directory
System.IO.Directory.SetCurrentDirectory (__SOURCE_DIRECTORY__)

// Requires FSharp.Data under script directory
//    nuget install FSharp.Data -o Packages -ExcludeVersion
#r @"Packages\FSharp.Data\lib\net40\FSharp.Data.dll"
open FSharp.Data

let data = WorldBankData.GetDataContext()
```

设置好类型提供程序后，我们可以进行严肃的查询，例如：

“低收入国家和高收入国家的营养不良率如何比较？”

```F#
// Create a list of countries to process
let groups =
 [| data.Countries.``Low income``
    data.Countries.``High income``
    |]

// get data from an indicator for particular year
let getYearValue (year:int) (ind:Runtime.WorldBank.Indicator) =
    ind.Name,year,ind.Item year

// get data
[ for c in groups ->
    c.Name,
    c.Indicators.``Malnutrition prevalence, weight for age (% of children under 5)`` |> getYearValue 2010
]
// print the data
|> Seq.iter (
    fun (group,(indName, indYear, indValue)) ->
       printfn "%s -- %s %i %0.2f%% " group indName indYear indValue)
```

结果是：

```
Low income -- Malnutrition prevalence, weight for age (% of children under 5) 2010 23.19%
High income -- Malnutrition prevalence, weight for age (% of children under 5) 2010 1.36%
```

同样，以下是比较孕产妇死亡率的代码：

```F#
// Create a list of countries to process
let countries =
 [| data.Countries.``European Union``
    data.Countries.``United Kingdom``
    data.Countries.``United States`` |]

/ get data
[ for c in countries  ->
    c.Name,
    c.Indicators.``Maternal mortality ratio (modeled estimate, per 100,000 live births)`` |> getYearValue 2010
]
// print the data
|> Seq.iter (
    fun (group,(indName, indYear, indValue)) ->
       printfn "%s -- %s %i %0.1f" group indName indYear indValue
```

结果是：

```
European Union -- Maternal mortality ratio (modeled estimate, per 100,000 live births) 2010 9.0
United Kingdom -- Maternal mortality ratio (modeled estimate, per 100,000 live births) 2010 12.0
United States -- Maternal mortality ratio (modeled estimate, per 100,000 live births) 2010 21.0
```

更多关于如何使用世界银行类型的提供者。

## 26.使用 F# 进行数据科学和机器学习

所以，你正在将所有这些建议付诸实践。您正在使用 FParsec 解析 web 日志，使用 SQL 类型提供程序从内部数据库中提取统计数据，并从 web 服务中提取外部数据。你有这些数据，你能用它做什么？

最后，让我们快速了解一下将 F# 用于数据科学和机器学习。

正如我们所看到的，F# 非常适合探索性编程——它有一个具有智能感知的 REPL。但与 Python 和 R 不同，你的代码是经过类型检查的，所以你知道你的代码不会在两个小时的处理工作中途出现异常而失败！

如果你熟悉 Python 中的 Pandas 库或 R 中的“tseries”包，那么你应该认真看看 Deedle，这是一个易于使用、高质量的数据和时间序列操作包。Deedle 设计用于使用 REPL 进行探索性编程，但也可以用于高效编译 .NET 代码。

如果你经常使用 R，当然有一个 R 类型的提供者。这意味着您可以像 .NET 库一样使用 R 包。太棒了！

还有很多其他 F# 友好的软件包。你可以在 fsharp.org 上找到所有关于它们的信息。

- 数据科学
- 数学
- 机器学习

## 系列总结

呼！这是一长串示例和大量代码。如果你已经完成了，恭喜你！

我希望这能让你对 F# 的价值有一些新的认识。这不仅仅是一门数学或金融语言，也是一门实用的语言。它可以帮助您处理开发、测试和数据管理工作流中的各种事情。

最后，正如我在本系列文章中强调的那样，所有这些用途都是安全、低风险和增量的。可能发生的最糟糕的情况是什么？

所以，继续，说服你的队友和老板给 F# 一个尝试，让我知道进展如何。

## 后记

在我发布这篇文章后，Simon Cousins 在推特上说我错过了一个——我忍不住要加上它。

> @ScottWlaschin 27：平衡英国发电站的发电计划。说真的，替代 #fsharp 的方法风险太大了
>
> --西蒙·考辛斯（@simontcouncins）2014年4月25日

你可以在Simon的博客上阅读更多关于他使用 F#（用于发电）的真实世界。在 fsharp.org 上有更多关于 F# 的推荐信。