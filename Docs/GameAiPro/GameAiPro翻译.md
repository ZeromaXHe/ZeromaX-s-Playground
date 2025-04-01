https://www.gameaipro.com/

# 第 VII 部分 概率与结局（Odds and Ends）

# 45 介绍 AI 的 GPGPU

*Conan Bourke and Tomasz Bednarz*

## 45.1 引言

在过去的十年里，计算机硬件已经取得了长足的进步。一个核心，两个核心，四个核心，现在成百上千个核心！计算机的功能已经从 CPU 转移到 GPU，新的 API 允许程序员控制这些芯片，而不仅仅是图形处理。

随着硬件的每一次进步，游戏系统都获得了越来越多的处理器能力，从而提供了比以前想象的更复杂、更详细的体验。对于人工智能来说，这意味着更现实的代理能够与彼此、玩家和周围环境进行更复杂的交互。

硬件方面最重要的进步之一是 GPU，它从纯粹的渲染处理器转变为能够在一定范围内进行任何计算的通用浮点处理器。AMD 和 NVIDIA 这两家主要的硬件供应商都生产通常具有 512 个以上处理器的 GPU，最新型号在消费类型号中提供了约 3000 个处理器，每个处理器都能够并行处理数据。这是一个很大的处理能力，我们不必将其全部用于高端图形。即使是 Xbox 360 也有一个能够进行基本通用处理技术的 GPU，而索尼的 PS3 的架构类似于能够进行相同处理的 GPU。

这些处理器的一个常见缺点是 CPU 和 GPU 之间的延迟和带宽，但在将 GPU 的线性计算模型与 CPU 的通用处理模型（称为加速处理单元（APU））相结合方面，尤其是 AMD，正在取得进展，这大大降低了这种延迟，并提供了本章稍后将讨论的其他优势。

## 45.2 GPGPU 的历史

GPU 上的通用计算（GPGPU）是指使用 GPU 进行渲染以外的计算[Harris 02]。随着早期着色器模型的引入，程序员能够修改 GPU 处理的方式和内容，不再依赖 OpenGL 或 Direct3D 的固定功能管道，也不必被迫渲染图像以供查看。相反，纹理可以用作数据缓冲区，在像素片段中访问和处理，结果可以绘制到输出纹理中。这种方法的缺点是缓冲区是只读的或只写的。此外，元素独立性的限制以及在图形算法和渲染管道的背景下表示问题的需要使这种技术变得繁琐。尽管如此，着色器已经从简单的汇编语言程序演变为几种新的类 C 语言，硬件供应商开始承认 GPGPU 领域的增长。

已经完成了在渲染管线之外暴露 GPU 功能的工作，因此创建了 API，让程序员可以访问 GPU 的功能，而不必将其视为纯粹的基于图形的设备，这些 API 带来了具有读写修改权限和额外数学功能的缓冲区。

2007年2月，NVIDIA 推出了计算统一设备架构（CUDA）API 和 G80 系列 GPU，这大大加速了 GPGPU 领域，并与 DirectX11 一起发布了 DirectCompute API。它与 CUDA 有许多相似的想法和方法，但具有使用 DirectX 现有资源的优势，允许在 DirectCompute 和 Direct3D 之间轻松共享数据以实现可视化。DirectCompute 也可以在 DirectX10 硬件上使用，但仍然受到 DirectX 其他部分的限制，因为它只能在基于 Windows 的系统上运行。

与之竞争的标准 OpenCL（开放计算语言）于 2008 年首次发布。OpenCL 使开发人员能够轻松地为异构架构（如多核 CPU 和 GPU，甚至索尼的 PS3）编写高效的跨平台应用程序，该应用程序具有基于现代 C 语言的单一编程接口。OpenCL 的规范由 Khronos Group[Khronos] 管理，他们还提供了一组我们在本文中使用的 C++ 绑定。这些绑定极大地简化了主机 API 的设置和代码开发速度。

## 45.3 OpenCL

OpenCL 程序被编写为“内核”，即在单个处理单元（计算单元）上与其他处理单元并行执行的函数，彼此独立工作。内核代码与 C 代码非常相似，并支持许多内置的数学函数。

当宿主程序运行时，我们需要执行以下步骤来使用 OpenCL：

1. OpenCL 枚举系统中可用的平台和计算设备，即所有 CPU 和 GPU。在每个设备内都有一个或多个计算单元，在这些处理单元内处理实际计算。单元的数量对应于设备（CPU 或 GPU）可以同时执行的独立指令的数量。因此，具有 8 个单元的 CPU 仍然被视为单个设备，具有 448 个单元的 GPU 也是如此。
2. 选择最强大的设备（通常是 GPU），并设置其上下文以进行进一步操作。
3. 在主机应用程序和 OpenCL 之间配置数据共享。
4. 使用内核代码和 OpenCL 上下文为您的设备构建 OpenCL 程序，然后从编译的内核代码中提取内核对象。
5. OpenCL 利用命令队列来控制内核执行的同步。向内核读取数据和从内核写入数据以及操作内存对象也由命令队列执行。
6. 内核在所有处理单元中被调用和执行。并行线程共享内存并使用屏障进行同步。最终，工作项的输出被读回主机内存以供进一步操作。

## 45.4 简单群集（Simple Flocking）和 OpenCL

我们将简要介绍使用 OpenCL 将经典 AI 算法转换为在 GPU 上运行。Craig Reynolds[Reynolds 87]介绍了用于控制自主移动代理的转向行为的概念，以及用于模拟人群和羊群的群集和 boids 的概念。许多 RTS 游戏都利用了具有美丽效果的群集——Relic Entertainment 的《家园》系列就是一个这样的例子——但这些游戏通常在可用的代理数量上有限。将此算法转换为在 GPU 上运行，我们可以很容易地将代理数量增加到数千。

我们将在 CPU 和 GPU 上实现一种强力群集方法，以证明通过简单地切换到 GPU 而不使用任何分区，利用 GPU 大规模并行架构可以轻松获得收益。利用简单的空间划分方案，使用 PS3 的 Cell Architecture[Renods 06]进行了类似的工作。清单 45.1 给出了一个基本群集算法的伪代码，该算法使用优先加权力之和进行分离、内聚和对齐，还包括一个漫游行为来帮助随机化代理。首要任务是漫游，然后是分离、凝聚力，最后是对齐。所有速度在应用于位置之前必须更新，否则初始试剂会错误地影响后续试剂。

Listing 45.1 群集伪代码

```
for each agent
    for each neighbor within radius
         calculate agent separation force away from neighbor
         calculate center of mass for agent cohesion force
         calculate average heading for agent alignment force
    calculate wander force
    sum prioritized weighted forces and apply to agent velocity
for each agent
	apply velocity to position
```

在 CPU 上实现该算法很简单。通常，一个代理将由一个包含相关代理信息（即位置、速度、漫游目标等）的对象组成。然后，一系列代理将被循环两次；首先更新每个代理的力和速度，然后应用新的速度。

在将此算法转换为 GPU 时，必须考虑一些因素，但将 CPU 代码转换为 OpenCL 代码很简单。如上面的伪代码所示，为每个代理计算所有邻居，每个代理的复杂度为 O(n^2）。在 CPU 上，这是通过所有代理的双循环来实现的。在 GPU 上，我们能够并行化外循环，并顺序执行每个工作项（每个代理）的内环交互，从而大大缩短了处理时间。

可以实现空间分区技术来提高性能，但必须注意的是，GPU 以非常线性的方式工作，非常适合处理数据阵列，这通常是在图形处理的顶点阵列的情况下完成的。在复杂的空间分区方案（如八叉树）的情况下，GPU 在尝试访问非线性内存时会抖动。Craig Reynolds 对 PS3 的解决方案是使用一个简单的三维桶网格来存储相邻的代理[Reynolds 06]。这允许对桶进行线性处理，代理只需对与其直接相邻的桶具有读取权限。然而，在本文中，我们演示了一个从 CPU 到 GPU 的简单转换，而没有进行这种优化，以展示转换为 GPGPU 处理的即时收益。

转换为 GPGPU 的第一步是将数据分解为连续的数组。GPU最多可以处理三维数组，但在我们的示例中，我们将把代理分解为代理中每个元素的一维数组，即位置、速度等。

同样值得注意的是，在 OpenCL 术语中有两种类型的内存：局部和全局。区别在于，全局内存可以由任何内核访问，而本地内存是进程独有的，因此访问速度要快得多。把它想象成 RAM 和 CPU 的缓存。

## 45.5 OpenCL 设置

使用C++主机绑定可以直接初始化计算设备。首先，必须枚举主机平台才能访问底层计算设备。然后，从平台创建上下文（请注意，在这个例子中，我们使用 CLD_TYPE_GPU 初始化上下文以专门使用 GPU）以及命令队列，以便通过上下文执行计算内核和排队内存传输。有关详细信息，请参阅清单 45.2。

Listing 45.2 OpenCL host setup.

```c++
cl::Platform::get(&m_oclPlatforms);
cl_context_properties aoProperties[] = {
    CL_CONTEXT_PLATFORM,
    (cl_context_properties)(m_oclPlatforms[0])(),
    0};
m_oclContext = cl::Context(CL_DEVICE_TYPE_GPU, aoProperties);
m_oclDevices = m_oclContext.getInfo<CL_CONTEXT_DEVICES>();
std::cout << “OpenCL device count: “ << m_oclDevices.size();
m_oclQueue = cl::CommandQueue(m_oclContext, m_oclDevices[0]);
```

OpenCL 有两种类型的内存对象：缓冲区和图像。缓冲区包含使用单指令多数据（SIMD）处理模型的标准 4D 浮点向量，而图像是根据纹理像素定义的。为了本文的目的，缓冲区被选择为更适合表示彼此相邻的代理。

缓冲区可以初始化为只读、只写或读写，如清单 45.3 所示。创建缓冲区是为了容纳模拟将使用的最大数量的代理，尽管如果我们愿意，我们可以处理更少的代理。除了代理数据，我们还向内核发送群集算法的参数，以及一个时间值，该时间值指定了自上一帧以来一致速度的经过时间。

Listing 45.3 OpenCL buffer setup

```c++
typedef struct Params
{
    float fNeighborRadiusSqr;
    float fMaxSteeringForce;
    float fMaxBoidSpeed;
    float fWanderRadius;
    float fWanderJitter;
    float fWanderDistance;
    float fSeparationWeight;
    float fCohesionWeight;
    float fAlignmentWeight;
    float fDeltaTime;
} Params;
cl::Buffer m_clVPosition;
cl::Buffer m_clVVelocity;
cl::Buffer m_clVParams;
...
m_clVPosition = cl::Buffer(m_oclContext, CL_MEM_READ_WRITE, 
uiMaxAgentCount * 4 * sizeof(float));
m_clVParams = cl::Buffer(m_oclContext, CL_MEM_READ_ONLY, 
sizeof(Params));
```

为了创建计算内核，我们需要将内核代码编译成 CL 程序，然后提取计算内核。在我们的示例中，内核代码位于一个单独的文件 program.cl 中，加载该文件以创建程序，如清单 45.4 所示。

Listing 45.4 Building the OpenCL program

```c++
//read source file
std::ifstream sFile(“program.cl”);
std::string sCode(std::istreambuf_iterator<char>(sFile),
	(std::istreambuf_iterator<char>()));
cl::Program::Sources oSource(1,
	std::make_pair(sCode.c_str(), sCode.length() + 1));
//build the program for the specified devices
m_oclProgram = cl::Program(m_oclContext, oSource);
m_oclProgram.build(m_oclDevices);
m_clKernel = cl::Kernel(m_oclProgram, “Flocking”);
```

清单 45.5 显示了我们示例内核的一部分，省略了主体，因为它与 CPU 实现几乎相同。然而，值得注意的是内核中关于屏障的最后一部分。在 CPU 上，我们循环两次，在计算完所有代理后对其施加力。我们可以在内核中通过设置一个屏障来实现这一点，这会导致所有执行的线程在此时等待，直到所有线程都赶上。在内核中，我们可以根据缓冲区的大小，使用 0、1 或 2 调用 `get_global_id(0)` 来访问输入缓冲区的当前索引。

Listing 45.5 The OpenCL kernel

```c++
__kernel void Flocking(
     __global float4* vPosition,
     ...
     __constant struct Params* pp)
{
    //get_global_id(0) accesses the current element index
    unsigned int i = get_global_id(0);
    ...
    barrier(CLK_LOCAL_MEM_FENCE | CLK_GLOBAL_MEM_FENCE);
    vPosition[i] + = vVelocity[i] * pp->fDeltaTime;
    barrier(CLK_LOCAL_MEM_FENCE | CLK_GLOBAL_MEM_FENCE);
}
```

一旦构建了内核，内核参数就会显式传递给 OpenCL，如清单 45.6 所示。在执行内核时，必须将参数预加载到相应的参数索引中，而不是将参数传递给内核。

Listing 45.6 Specifying kernel arguments

```c++
m_clKernel.setArg(0, sizeof(cl_mem), &m_clVPosition);
m_clKernel.setArg(1, sizeof(cl_mem), &m_clVVelocity);
```

一旦所有内容都初始化并构建完成，我们就可以将内核排队进行计算。内核不会立即执行，而是排队等待处理。启动内核时，全局工作大小必须等于要处理的元素数量。我们还可以在数组范围内指定偏移量，但我们可以指定一个从数组前面开始的 NullRange。请参考清单 45.7。

Listing 45.7 Executing the kernel program

```c++
m_oclQueue.enqueueNDRangeKernel(
    m_clKernel, cl::NullRange,
    cl::NDRange(uiMaxAgentCount),
    cl::NullRange,
    nullptr, nullptr);
```

## 45.6 系统间共享GPU处理

开发人员在使用 GPGPU 时，特别是对于游戏开发人员来说，最初的担忧可能是图形处理时间被占用了。当今的许多高端游戏都使用 GPGPU 进行图形预处理和后处理，也使用 NVidia 的 PhysX 等 API 进行物理模拟。将 AI 添加到组合中将减少这些其他系统可用的处理时间。这是一个无法避免的问题。然而，GPU 处理能力已经有了巨大的飞跃，从英伟达 500 系列的数百个内核，到 600 系列的数千个内核。随着时间的推移，更多的系统将拥有更多的处理能力，开发人员将开始发现这种能力除了图形、物理和人工智能之外的其他有趣用途。

与此同时，至少对于 OpenCL，存在互操作性 API，允许在 OpenGL 和 Direct3D 之间共享 OpenCL 缓冲区，从而减少了不断将信息传输到 GPU 并返回 CPU 的需要。AI 代理的位置缓冲区既可以用于 GPU 上的群集计算，也可以用于渲染代理的硬件实例化的渲染缓冲区，而不需要将数据返回给 CPU，只需将其传输回 GPU 即可。

## 45.7 结果

图 45.1 显示了使用我们示例群集算法的暴力实现处理代理的性能（以毫秒为单位）（越低越好）。如图所示，GPU 通过更高的代理数量提供了巨大的性能提升，将算法转换为 OpenCL 所需的工作量最小。然而，由于缓冲区转换，在较低的代理计数下，GPU 的运行速度比 CPU 慢，如图 45.2 所示。还测试了用于计算和研究的 GPU，以显示此类设备中优化的带宽和延迟，从而比消费者级 GPU 更快地进行计算，同时也可以洞察未来消费者级的性能。

图45.1

使用各种 GPU 和 CPU 处理 512、1024、4096、8192 和 16384 个代理计数的性能（以毫秒为单位）。

图45.2

代理计数较低时的性能（毫秒）。消费者 GPU 保持不变，因为代理的处理几乎不需要任何时间，但 GPU 之间的缓冲区传输没有得到优化。然而，C2050 的带宽和延迟要高得多

## 45.8 结论

正如我们的例子所示，我们可以很容易地将 GPGPU 计算用于使用极高代理数的游戏，例如使用数千个实体的 RTS游戏，而不是大多数当前 RTS 游戏中的标准数十到数百个实体。我们仍然很难将代理的决策转移到 GPU 上，但运动甚至避障等元素可以从 CPU 上转移出来。占用其他系统（如图形）的处理时间将是另一个问题，但可以通过各种互操作性 API 在一定程度上缓解。

人工智能 GPGPU 的其他例子也存在，神经网络和寻路小组已经完成了工作，经典的康威生命游戏可以很容易地在 GPU 上实现[Rumpf 10]。GPGPU 中可用的 AI 处理类型的主要限制是大多数 AI 决策的分支性质。

APU 可以让我们将决策技术与 GPGPU 技术紧密结合，但消费者对此类设备的接受程度将决定这种 AI 风格是否会更多地出现在游戏中。

目前，GPGPU 是大规模模拟的一个可行选择，根据 Valve 的硬件调查，在撰写本文时，目前最常见的消费级 GPU 是装有 336 个处理器的英伟达 GTX 560。这足以满足我们的人工智能需求。