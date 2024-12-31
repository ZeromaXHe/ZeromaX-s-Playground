

# [返回主 Markdown](./CatlikeCoding网站翻译.md)

# 哈希：小 xxHash

发布于 2021-03-24 更新于 2021-05-23

https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing/

*创建哈希可视化网格。*
*将二维坐标转换为伪随机值。*
*实现一个小版本的xxHash。*
*使用哈希值对立方体进行着色和偏移。*

这是关于[伪随机噪声](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/)的系列教程中的第一篇。它是在[基础](https://catlikecoding.com/unity/tutorials/basics/)系列之后推出的。它引入了一种通过哈希函数生成明显随机值的方法，特别是 xxHash 的较小版本。

本教程使用 Unity 2020.3.6f1 编写。

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing/tutorial-image.jpg)

*使用哈希函数来实现。*

## 1 可视化

随机性是使事物变得不可预测、多样和自然的必要条件。感知到的现象是否真的是随机的，或者只是由于缺乏信息或对观察者的理解而出现的，这并不重要。所以我们可以做一些完全确定的事情，而不是随机的，只要这不是显而易见的。这很好，因为软件天生具有确定性。设计不佳的多线程代码可能会导致竞争条件，从而导致不可预测的结果，但这并不是一个可靠的随机性来源。真正可靠的随机性只能从外部来源获得，比如采样大气噪声的硬件，而这些通常是不可用的。

真正的随机性通常是不需要的。它产生的任何东西都是一次性事件，无法复制。每次的结果都不一样。理想情况下，我们有一个过程，对于任何特定的输入，都会产生一个唯一且固定的随机输出。这就是哈希函数的作用。

在本教程中，我们将创建一个由小立方体组成的二维网格，并使用它来可视化哈希函数。按照“基础知识”系列中的描述，从一个新项目开始。我们将使用 jobs 系统，因此导入 Burst 包。我还将使用 URP，因此导入*通用 RP* 并为其创建资产，然后配置 Unity 以使用它。

### 1.1 哈希作业

我们将使用作业为网格中的所有多维数据集创建哈希值。按照 [Basic](https://catlikecoding.com/unity/tutorials/basics/) 系列中介绍的相同方法，创建一个包含此类作业的 `HashVisualization` 组件类型。该作业将用哈希值填充 `NativeArray`。哈希值本质上是没有内在含义的比特集合。我们将为它们使用 `uint` 类型，它最接近 32 位（4 个字节）的通用数据包。最初，我们将直接使用作业的执行索引作为哈希值。

```c#
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

using static Unity.Mathematics.math;

public class HashVisualization : MonoBehaviour {

	[BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
	struct HashJob : IJobFor {

		[WriteOnly]
		public NativeArray<uint> hashes;
        
		public void Execute(int i) {
			hashes[i] = (uint)i;
		}
	}
}
```

> **为什么不使用 `int` 作为哈希类型？**
>
> `int` 类型表示一个有符号的整数，它有一个特殊的符号位。`uint` 类型是无符号的，因此没有特殊的符号位数。`uint` 值的所有位都以相同的方式处理。

### 1.2 初始化和渲染

与“[基础知识](https://catlikecoding.com/unity/tutorials/basics/)”系列一样，我们将向 `HashVisualization` 添加实例网格和材质的配置选项，以及分辨率滑块和所需的 `NativeArray`、`ComputeBuffer` 和 `MaterialPropertyBlock`。我们将使用 *_Hashes* 作为缓冲区的着色器标识符，并添加 *_Config* 着色器属性以进行其他配置。

```c#
	static int
		hashesId = Shader.PropertyToID("_Hashes"),
		configId = Shader.PropertyToID("_Config");

	[SerializeField]
	Mesh instanceMesh;

	[SerializeField]
	Material material;

	[SerializeField, Range(1, 512)]
	int resolution = 16;

	NativeArray<uint> hashes;

	ComputeBuffer hashesBuffer;

	MaterialPropertyBlock propertyBlock;
```

在 `OnEnable` 中初始化所有内容。因为我们不打算为哈希设置动画，所以我们可以立即在这里运行作业，并配置一次属性块，而不是每次更新都这样做。

我们需要在着色器中乘以分辨率并除以分辨率，因此将分辨率及其倒数存储在配置向量的前两个分量中。

```c#
	void OnEnable () {
		int length = resolution * resolution;
		hashes = new NativeArray<uint>(length, Allocator.Persistent);
		hashesBuffer = new ComputeBuffer(length, 4);

		new HashJob {
			hashes = hashes
		}.ScheduleParallel(hashes.Length, resolution, default).Complete();

		hashesBuffer.SetData(hashes);

		propertyBlock ??= new MaterialPropertyBlock();
		propertyBlock.SetBuffer(hashesId, hashesBuffer);
		propertyBlock.SetVector(configId, new Vector4(resolution, 1f / resolution));
	}
```

清理 `OnDisable` 中的哈希和缓冲区，并再次使用 `OnValidate` 中重置所有内容的方法，这样在播放模式下更改配置将刷新网格。

```c#
	void OnDisable () {
		hashes.Dispose();
		hashesBuffer.Release();
		hashesBuffer = null;
	}

	void OnValidate () {
		if (hashesBuffer != null && enabled) {
			OnDisable();
			OnEnable();
		}
	}
```

这一次，我们在 `Update` 中唯一要做的就是发出 draw 命令。我们将把网格保持在原点的单位立方体内。

```c#
	void Update () {
		Graphics.DrawMeshInstancedProcedural(
			instanceMesh, 0, material, new Bounds(Vector3.zero, Vector3.one),
			hashes.Length, propertyBlock
		);
	}
```

### 1.3 着色器

使用程序配置功能创建 HLSL 包含文件。与 [Basic](https://catlikecoding.com/unity/tutorials/basics/) 系列早期版本的不同之处在于，我们将直接从实例的标识符中推导出实例的位置。

通过将 1D 线切割成相等长度的线段并将其彼此相邻放置，在第二个维度上偏移，可以将其转换为 2D 网格。我们通过将标识符除以分辨率作为整数除法来实现这一点。GPU 没有整数除法，所以我们将通过 `floor` 函数丢弃除法的小数部分。这为我们提供了第二维的坐标，我们将其命名为 V。然后，通过从标识符中减去 V 乘以分辨率来找到 U 坐标。

然后，我们使用 UV 坐标将实例放置在 XZ 平面上，进行偏移和缩放，使其保持在原点处的单位立方体内。

```glsl
#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
	StructuredBuffer<uint> _Hashes;
#endif

float4 _Config;

void ConfigureProcedural () {
	#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
		float v = floor(_Config.y * unity_InstanceID);
		float u = unity_InstanceID - _Config.x * v;
		
		unity_ObjectToWorld = 0.0;
		unity_ObjectToWorld._m03_m13_m23_m33 = float4(
			_Config.y * (u + 0.5) - 0.5,
			0.0,
			_Config.y * (v + 0.5) - 0.5,
			1.0
		);
		unity_ObjectToWorld._m00_m11_m22 = _Config.y;
	#endif
}
```

还引入了一个函数，该函数检索哈希值并使用它来生成 RGB 颜色。最初将其设置为灰度值，将哈希除以分辨率平方，从而根据哈希索引从黑色变为白色。

```glsl
float3 GetHashColor () {
	#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
		uint hash = _Hashes[unity_InstanceID];
		return _Config.y * _Config.y * hash;
	#else
		return 1.0;
	#endif
}
```

接下来使用着色器图函数，我们将使用该函数传递位置并输出颜色。

```glsl
void ShaderGraphFunction_float (float3 In, out float3 Out, out float3 Color) {
	Out = In;
	Color = GetHashColor();
}

void ShaderGraphFunction_half (half3 In, out half3 Out, out half3 Color) {
	Out = In;
	Color = GetHashColor();
}
```

然后创建一个着色器图，就像我们在“[基础](https://catlikecoding.com/unity/tutorials/basics/)”系列中所做的那样，只是我们使用了新的 HLSL 文件和函数，并将我们的颜色直接连接到着色器的基色。我还使用默认的 0.5 值来获得平滑度，而不是使其可配置。

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing/visualization/shader-graph.png)

*着色器图。*

以下是用于 *InjectPragmas* 自定义函数节点的代码文本：

```glsl
#pragma instancing_options assumeuniformscaling procedural:ConfigureProcedural
#pragma editor_sync_compilation

Out = In;
```

> **如果我不想使用 URP 怎么办？**
>
> 您还可以使用 HDRP，或创建一个包含默认 RP 的 HLSL 文件的曲面着色器，如“[基础知识](https://catlikecoding.com/unity/tutorials/basics/)”系列中所述。

现在，我们可以创建一个使用着色器的材质，以及一个使用该材质和立方体作为其实例的 `HashVisualization` 组件的游戏对象。

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing/visualization/game-object.png)

*哈希游戏对象。*

此时，游戏模式下应该会出现一个网格。

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing/visualization/grid.png)

*网格，自上而下的正交场景视图。*

在播放模式下通过检查器调整分辨率会导致网格重新创建。它似乎在大多数时候都能正常工作，但对于某些分辨率，网格是不对齐的。

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing/visualization/misaligned-resolution-41.png)

*对分辨率 41 的不一致之处。*

此错误是由浮点精度限制引起的。在某些情况下，在应用 `floor` 之前，我们最终得到的值比整数小一点，这会导致实例错位。在我们的例子中，我们可以通过在丢弃分数部分之前添加 0.00001 的正偏差来解决这个问题。

```glsl
		float v = floor(_Config.y * unity_InstanceID + 0.00001);
```

### 1.4 模式

在我们继续实现真正的哈希函数之前，让我们简要考虑一下简单的数学函数。作为第一步，我们将使当前的灰度梯度每 256 点重复一次。我们只考虑 `GetHashColor` 中哈希的八个最低有效位。这是通过按位 AND 运算符将哈希与二进制 11111111（十进制 255）组合来实现的。这会掩盖该值，因此只保留其八个最低有效位，将其限制在 0~255 范围内。然后，通过除以 255，该范围可以缩小到 0~1。

```glsl
		uint hash = _Hashes[unity_InstanceID];
		return (1.0 / 255.0) * (hash & 255);
```

结果模式取决于分辨率。在分辨率为32时，我们得到一个沿Z方向重复四次的梯度，但当分辨率稍有变化时，例如变为41，这种模式就会失准。

![32](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing/visualization/lowest-eight-bits-32.png) ![41](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing/visualization/lowest-eight-bits-41.png)

*分辨率为 32 和 41 时的最低 8 位。*

让我们用 Weyl 序列替换明显的梯度，我们最初在[《有机品种》](https://catlikecoding.com/unity/tutorials/basics/organic-variety/)教程中也使用了 Weyl 序列对分形进行着色。根据HashJob中的索引执行此操作。在转换为 `uint` 之前，执行并将结果乘以 256，这样我们就得到了介于 0 和 255 之间的哈希值。

```glsl
			hashes[i] = (uint)(frac(i * 0.381f) * 256f);
```

![32](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing/visualization/sequence-32.png) ![41](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing/visualization/sequence-41.png)

*分辨率为 32 和 41 的 0.381 序列。*

我们总是得到一个明显的重复梯度，其方向取决于分辨率。为了使其独立于分辨率，我们必须将函数基于点的 UV 坐标，而不是它们的索引。我们可以在作业中找到坐标，就像在着色器中一样。然后使用 U 和 V 的乘积作为序列的基础。这要求我们为决议及其对等内容添加字段。

```c#
		public int resolution;

		public float invResolution;
        
		public void Execute(int i) {
			float v = floor(invResolution * i + 0.00001f);
			float u = i - resolution * v;
			hashes[i] = (uint)(frac(u * v * 0.381f) * 255f);
		}
```

> **我们不能用整数除法代替 `floor` 吗？**
>
> 是的，但这不是一个好主意，因为整数除法不能矢量化，这使得我们的工作效率大大降低。您可以通过研究 *Burst* 生成的代码来验证这一点。
>
> 请注意，SSE2 指令集不包括矢量化的 floor 操作，因此当仅限于该指令集时，你会得到四个对 floor 函数的非矢量化调用，这是次优的。因为在这个特定的情况下，我们只处理正值，你也可以将其转换为整数，这会用 SSE2 进行矢量化。但为了保持一致，我忽略了这一点。

将所需数据传递给 `OnEnable` 中的作业。

```c#
		new HashJob {
			hashes = hashes,
			resolution = resolution,
			invResolution = 1f / resolution
		}.ScheduleParallel(hashes.Length, resolution, default).Complete();
```

![32](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing/visualization/uv-32.png) ![41](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing/visualization/uv-41.png)

*分辨率 32 和 41 中的基于坐标的顺序。*

我们现在得到一个更有趣的模式，它看起来比以前更随意，但它有非常明显的重复。为了获得更好的结果，我们需要一个好的哈希函数。

## 2 小 xxHash

有许多已知的哈希函数。我们不关心可用于保护数据和连接的加密哈希函数，我们正在寻找一种既快速又能产生良好视觉效果的函数。Yann Collet 设计的 [xxHash 快速摘要算法](http://xxhash.com/)是一个很好的候选者。因为我们处理的是非常小的输入数据——只有两个整数——我们将创建一个 XXH32 的变体，跳过[算法](https://github.com/Cyan4973/xxHash/blob/dev/doc/xxhash_spec.md)的第 2、3 和 4 步。我把它命名为 `SmallXXHash`。

### 2.1 哈希结构

在单独的 C# 文件中为 `SmallXXHash` 创建一个结构类型。在其中定义五个 `uint` 常量，如下所示。这是五个二进制素数，名为 A 到 E，用于操纵比特。这些值是由 Yann Collet 根据经验选择的。

```c#
public struct SmallXXHash {

	const uint primeA = 0b10011110001101110111100110110001;
	const uint primeB = 0b10000101111010111100101001110111;
	const uint primeC = 0b11000010101100101010111000111101;
	const uint primeD = 0b00100111110101001110101100101111;
	const uint primeE = 0b00010110010101100110011110110001;
}
```

该算法的工作原理是将哈希位存储在累加器中，为此我们需要一个 `uint` 字段。该值用一个种子树初始化，并加上素数 E。这是创建哈希的第一步，因此我们通过带有种子参数的公共构造函数方法来实现。我们将把种子视为 `uint`，但有符号整数通常用于代码中，因此 `int` 参数更方便。

```c#
	uint accumulator;

	public SmallXXHash (int seed) {
		accumulator = (uint)seed + primeE;
	}
```

> **构造函数是如何定义的？**
>
> 它被声明为一个常规方法，返回它构造的类型，只是它没有名称。它也不会显式返回任何内容，因为该方法始终用于初始化新的对象实例或结构值。

这允许我们创建一个种子 `SmallXXHash` 值。为了得到最终的 `uint` 哈希值，我们可以引入一个公共的 `ToUint` 方法，该方法只返回累加器。

```c#
	public uint ToUint () => accumulator;
```

但我们可以将转换为 `uint` 隐式。首先重写方法，使其变为静态，并对给定的 `SmallXXHash` 值进行操作。

```c#
	public static uint ToUint (SmallXXHash hash) => hash.accumulator;
```

然后，通过将方法名称替换为运算符 `uint`，将静态方法转换为强制转换为 `operator uint`。

```c#
	public static operator uint (SmallXXHash hash) => hash.accumulator;
```

类型转换必须是隐式的或显式的。让我们通过在 `operator` 前面写 `implicit` 关键字来实现隐式。这使得我们可以直接将 `SmallXXHash` 值分配给 `uint`，转换将隐式发生，而无需在它前面写（`uint`）。

```c#
	public static implicit operator uint (SmallXXHash hash) => hash.accumulator;
```

现在，我们可以在作业中创建一个新的 `SmallXXHash` 值，最初将其种子设置为零，然后直接将其用作最终哈希值。

```c#
		public void Execute(int i) {
			float v = floor(invResolution * i + 0.00001f);
			float u = i - resolution * v;

			var hash = new SmallXXHash(0);
			hashes[i] = hash;
		}
```

> **使用单独的 `SmallXXHash` 类型并转换为 `uint` 不是很慢吗？**
>
> `int` 和 `uint` 之间没有实际的转换。这些类型仅控制如何解释值。类型指示在执行整数运算时是否应考虑符号。因此，根据经验，始终使用 `int`，除非你真的不想区别对待符号位，`SmallXXHash` 就是这种情况。
>
> 除此之外，如果可能的话，*Burst* 将摆脱所有方法调用。我们的 `SmallXXHash` 类型实际上是 `uint` 的装饰性别名，对性能没有影响。最终结果与我们使用 `uint` 变量直接在 `Execute` 内部用 `SmallXXHash` 编写所有代码的结果相同。因此，它也将被矢量化。
>
> 在常规 C# 代码的情况下，它可能效率稍低，但我们正在编写专门用于 *Burst* 的方便代码。

### 2.2 吃数据

XXHash32 的工作原理是以 32 位的部分消耗其输入，可能是并行的。我们的小版本只关心单独吃一份，为此我们将添加一个 `SmallXXHash.Eat` 方法有一个 `int` 参数，但不返回任何值。我们将再次将输入数据视为 `uint`，将其与素数 C 相乘，然后将其添加到累加器中。这将导致整数溢出，但这很好，因为我们不关心数据的数值解释。因此，所有操作实际上都是模 2^32^。

```c#
	public void Eat (int data) {
		accumulator += (uint)data * primeC;
	}
```

调整 `HashJob.Execute` 使 U 和 V 为整数，然后在将 U 和 V 用于结果之前将其输入哈希。

```c#
		public void Execute(int i) {
			int v = (int)floor(invResolution * i + 0.00001f);
			int u = i - resolution * v;

			var hash = new SmallXXHash(0);
			hash.Eat(u);
			hash.Eat(v);
			hashes[i] = hash;
		}
```

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing/short-xxhash/diagonal-pattern.png)

*对角线图案，分辨率 32。*

这只是进食过程的第一步。添加值 `Eat` 后，必须将累加器的位向左旋转。让我们为此添加一个私有静态方法，按给定的步骤移动一些数据。首先，使用 `<<` 运算符将所有位向左移动。

```c#
	static uint RotateLeft (uint data, int steps) => data << steps;
```

> **移位是如何工作的？**
>
> 向左移动的位随着所指示的步数而变得更加重要。左侧的位丢失，右侧用零填充。例如，`0b11111111_00000000_11111111_00000001 << 3` 产生 `0b11111000_ 00000111_11111000_00001000`。

旋转和移位之间的区别在于，移位会丢失的比特会通过旋转重新插入到另一侧。对于 32 位数据，也可以通过在另一个方向上移动 32 减去指示的步长，然后用 `|` 二进制 OR 运算符合并两次移动的结果来实现。

```c#
	static uint RotateLeft (uint data, int steps) =>
		(data << steps) | (data >> 32 - steps);
```

> **不是有左旋转 CPU 指令吗？**
>
> 是的，*Burst* 能够识别此代码并使用适当的 ROL 指令。然而，没有矢量化的 ROL 指令，因此当矢量化可能时，它将通过两次移位和一次位 OR 来完成。

现在在 `Eat` 中将累加器向左旋转 17 位。Burst还将内联此方法调用，并直接使用 15 进行右移，从而消除了常量减法。

```c#
	public void Eat (int data) {
		accumulator = RotateLeft(accumulator + (uint)data * primeC, 17);
	}
```

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing/short-xxhash/bit-rotation.png)

*带字节旋转。*

进食过程的最后一步是将累加器与素数 D 相乘。

```c#
	public void Eat (int data) {
		accumulator = RotateLeft(accumulator + (uint)data * primeC, 17) * primeD;
	}
```

![32](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing/short-xxhash/extra-multiplication.png)

*额外的乘法。*

虽然结果看起来还不好，但 `Eat` 方法已经完成。虽然我们在本教程中不会使用它，但让我们添加一个接受单个 `byte` 的变体 `Eat` 方法，因为 XXHash32 对数据大小的处理略有不同：它向左旋转 11 步而不是 17 步，并与素数 E 和 A 相乘，而不是素数 C 和 D。

```c#
	public void Eat (byte data) {
		accumulator = RotateLeft(accumulator + data * primeE, 11) * primeA;
	}
```

### 2.3 雪崩

XXHash 算法的最后一步是混合累加器的比特，以分散所有输入比特的影响。这被称为雪崩效应。这发生在所有数据都被吃掉并且需要最终的哈希值之后，所以我们将在转换为 `uint` 时这样做。

雪崩值开始等于累加器。它向右移动了 15 步，然后通过 `^` 位 XOR 运算符与原始值组合。之后，它与素数 B 相乘。这个过程再次完成，向右移动 13 步，XOR，并与素数 C 相乘，然后再乘以 16 步，但没有进一步的乘法。

```c#
	public static implicit operator uint (SmallXXHash hash) {
		uint avalanche = hash.accumulator;
		avalanche ^= avalanche >> 15;
		avalanche *= primeB;
		avalanche ^= avalanche >> 13;
		avalanche *= primeC;
		avalanche ^= avalanche >> 16;
		return avalanche;
	}
```

> **按位 XOR 的作用是什么？**
>
> 它是异或运算符。当第一个或第二个操作数的相同位设置为 1 时，每个位变为 1。当两个或两个操作数位都不是 1 时，该位变为 0。例如，`0b0011100 ^ 0b000001111` 得到 `0b00110010011`。

![32](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing/short-xxhash/avalanche-32.png) ![364](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing/short-xxhash/avalanche-64.png)

*雪崩，分辨率 32 和 64。*

### 2.4 负坐标

为了证明我们的哈希函数也适用于负坐标，在 `HashJob.Execute` 中从 U 和 V 中减去一半的分辨率。

```c#
			int v = (int)floor(invResolution * i + 0.00001f);
			int u = i - resolution * v - resolution / 2;
			v -= resolution / 2;
```

![32](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing/short-xxhash/centered-32.png) ![364](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing/short-xxhash/centered-64.png)

*中心坐标，分辨率 32 和 64。*

调整分辨率时，哈希模式现在将保持居中，尽管在偶数和奇数分辨率之间切换时会抖动一步。

### 2.5 方法链

虽然 `SmallXXHash` 已经功能齐全，但通过添加对方法链的支持，我们可以使其更便于使用。这意味着我们更改了两个 `Eat` 方法，使它们返回哈希值本身，这可以通过 `this` 关键字完成。

```c#
	public SmallXXHash Eat (int data) {
		accumulator = RotateLeft(accumulator + (uint)data * primeC, 17) * primeD;
		return this;
	}

	public SmallXXHash Eat (byte data) {
		accumulator = RotateLeft(accumulator + data * primeE, 11) * primeA;
		return this;
	}
```

`HashJob.Execute` 中的当前代码仍然有效，忽略返回的哈希值。但现在我们可以直接对构造函数调用的结果调用 `Eat`，也可以直接对 `Eat` 本身的结果调用，从而将我们的代码减少到一行。

```c#
			//var hash = new SmallXXHash(0).Eat(u).Eat(v);
			//hash.Eat(u);
			//hash.Eat(v);
			hashes[i] = new SmallXXHash(0).Eat(u).Eat(v);
```

### 2.6 不可变性

我们可以更进一步，使其成为 `SmallXXHash.Eat` 方法不会调整它们被调用的值的累加器。这使得保留一个中间哈希值并在以后重用它成为可能，这是我们将来会使用的东西。因此，我们使 `SmallXXHash` 成为一个不可变的结构，其行为与 `uint` 值完全相同。

将 `readonly` 修饰符添加到 `SmallXXHash` 中，以指示并强制它是不可变的。

```c#
public readonly struct SmallXXHash { … }
```

我们必须以同样的方式标记它的累加器字段。

```c#
	readonly uint accumulator;
```

从现在开始，获取不同累加器值的唯一方法是将其作为参数传递给构造函数方法，因为只有构造函数才允许修改 `readonly` 字段。调整现有构造函数，使其直接设置累加器，而不是应用种子。

```c#
	public SmallXXHash (uint accumulator) {
		this.accumulator = accumulator;
	}
```

此时，我们有一个从 `uint` 到 `SmallXXHash` 的直接转换。让我们为它创建一个方便的隐式强制转换方法。

```c#
	public static implicit operator SmallXXHash (uint accumulator) =>
		new SmallXXHash(accumulator);
```

`Eat` 方法现在可以直接将新的累加器值作为 `SmallXXHash` 返回。

```c#
	public SmallXXHash Eat (int data) =>
		RotateLeft(accumulator + (uint)data * primeC, 17) * primeD;

	public SmallXXHash Eat (byte data) =>
		RotateLeft(accumulator + data * primeE, 11) * primeA;
```

最后，为了再次使用种子进行初始化，引入一个公共静态 `Seed` 方法，该方法创建一个具有适当累加器值的 `SmallXXHash`。

```c#
	public static SmallXXHash Seed (int seed) => (uint)seed + primeE;
```

使用此新方法初始化 `HashJob.Execute` 中的哈希，因此我们不再显式调用构造函数，而是依赖于一系列常规方法调用，其中第一个是静态的。

```c#
			hashes[i] = SmallXXHash.Seed(0).Eat(u).Eat(v);
```

请注意，这些更改纯粹是为了风格。*Burst* 仍然会生成相同的指令。

## 3 显示更多哈希值

`SmallXXHash` 完成后，让我们把注意力集中在结果的可视化上。

### 3.1 使用不同的比特

到目前为止，我们只查看了生成的哈希值的最低有效字节。改变这一点的一个快速方法是移动 `GetHashColor` 中的哈希位。例如，通过将哈希向右移动 8 位，我们最终会看到第二个字节的重要性提高了一步。

```c#
		return (1.0 / 255.0) * ((hash >> 8) & 255);
```

![first](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing/short-xxhash/centered-32.png) ![second](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing/showing-more-of-the-hash/second-byte.png)

*第一个和第二个字节。*

通过这种方式，我们可以使用相同的哈希创建四个完全独立的 8 位可视化，尽管也可以使用不同的比特计数和移位。

### 3.2 颜色

我们可以通过将每个字节用于最终颜色的不同 RGB 通道来组合三个字节的可视化。让我们用最低字节表示红色，第二低字节表示绿色，第三低字节表示蓝色。因此，我们必须向右移动 0、8 和 16 位。

```c#
		uint hash = _Hashes[unity_InstanceID];
		return (1.0 / 255.0) * float3(
			hash & 255,
			(hash >> 8) & 255,
			(hash >> 16) & 255
		);
```

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing/showing-more-of-the-hash/rgb-colors.png)

*使用 RGB 颜色。*

### 3.3 可配置种子

现在我们有了一个很好的可视化，显示了 75% 的哈希位，让我们对种子进行配置。在 `HashJob` 中为它添加一个字段，并使用它来初始化哈希。

```c#
		public int seed;
        
		public void Execute(int i) {
			…

			hashes[i] = SmallXXHash.Seed(seed).Eat(u).Eat(v);
		}
```

还要将配置字段添加到 `HashVisualization` 中，并将其传递给 `OnEnable` 中的作业。种子是一个不受限制的整数值。

```c#
	[SerializeField]
	int seed;

	…

	void OnEnable () {
		…

		new HashJob {
			hashes = hashes,
			resolution = resolution,
			invResolution = 1f / resolution,
			seed = seed
		}.ScheduleParallel(hashes.Length, resolution, default).Complete();

		…
	}
```

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing/showing-more-of-the-hash/seed-inspector.png)

*可配置种子。*

现在，您可以通过调整种子来彻底改变模式。每颗种子都会产生一种完全不同的模式，它们之间没有明显的关系。

![1](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing/showing-more-of-the-hash/seed-1.png) ![2](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing/showing-more-of-the-hash/seed-2.png)

*种子设置为 1 和 2。*

我们可以更进一步，将哈希的初始化从作业中移除。除了从作业中消除单个加法的轻微优化外，这表明您可以在作业中使用哈希值之前，以任何方式初始化哈希值。

将 `HashJob` 的种子字段替换为 `SmallXXHash` 字段，并在 `Execute` 中直接使用。

```c#
		//public int seed;
		public SmallXXHash hash;
        
		public void Execute(int i) {
			…

			hashes[i] = hash.Eat(u).Eat(v);
		}
```

然后将种子哈希传递给 `OnEnable` 中的作业。

```c#
		new HashJob {
			hashes = hashes,
			resolution = resolution,
			invResolution = 1f / resolution,
			hash = SmallXXHash.Seed(seed)
		}.ScheduleParallel(hashes.Length, resolution, default).Complete();
```

### 3.4 使用最后一个字节

哈希中还有一个字节尚未可视化。我们可以将其用于不透明度，但这会使一切更难看到，并且需要对立方体实例进行适当的基于深度的排序才能正确渲染。因此，让我们使用第四个字节为立方体提供垂直偏移。

使垂直偏移可配置，范围有限，如 -2~2，默认值为 1。我们使此偏移相对于多维数据集实例大小，因此实际偏移将除以分辨率。将此比例作为配置向量的第三个分量传递给 GPU。

```c#
	[SerializeField, Range(-2f, 2f)]
	float verticalOffset = 1f;
	
	…
	
	void OnEnable () {
		…
		propertyBlock.SetVector(configId, new Vector4(
			resolution, 1f / resolution, verticalOffset / resolution
		));
	}
```

在 `ConfigureProcedural` 中应用偏移。最后一个字节是通过向右移动 24 步来找到的，因为此时所有其他位都是零，所以我们不需要在之后屏蔽它。将其缩小到 0~1，然后减去一半，将范围更改为 −½~½。然后应用配置的垂直偏移比例。

```c#
		unity_ObjectToWorld._m03_m13_m23_m33 = float4(
			_Config.y * (u + 0.5) - 0.5,
			_Config.z * ((1.0 / 255.0) * (_Hashes[unity_InstanceID] >> 24) - 0.5),
			_Config.y * (v + 0.5) - 0.5,
			1.0
		);
```

![inspector](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing/showing-more-of-the-hash/offset-inspector.png) ![scene](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing/showing-more-of-the-hash/offset-scene.png)

*具有可配置的垂直偏移。*

下一个教程是[哈希空间](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing-space/)。

[license](https://catlikecoding.com/unity/tutorials/license/)

[repository](https://bitbucket.org/catlikecodingunitytutorials/pseudorandom-noise-01-hashing/)

[PDF](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing/Hashing.pdf)



# 哈希空间：在任意网格中哈希

发布于 2021-04-30 更新于 2021-07-15

https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing-space/

*哈希变换的 3D 空间。*
*具有不同形状的样本空间。*
*手动矢量化作业。*
*创建形状作业模板。*

这是关于[伪随机噪声](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/)的系列教程中的第二篇。在其中，我们将调整哈希，使其适用于任意网格和形状。

本教程使用 Unity 2020.3.6f1 编写。

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing-space/tutorial-image.jpg)

*使用变形环面对旋转哈希空间进行采样。*

## 1 网格改造

在前面的教程中，我们通过对整数 UV 坐标进行散列来对平面上的采样点进行着色和位移，因此每个点都有自己的散列值。在本教程中，我们将改为对空间本身进行哈希运算，然后对其进行采样。我们将根据空间定义的整数网格进行此操作。这将哈希与采样点解耦，使其独立于可视化的形状和分辨率。

### 1.1 更改缩放

为了说明哈希模式和分辨率可以解耦，让我们从将哈希模式的有效分辨率加倍开始。我们可以通过将 `HashJob.Execute` 中的 UV 坐标加倍来实现这一点。先执行，然后再馈送到哈希。

```c#
		public void Execute(int i) {
			int v = (int)floor(invResolution * i + 0.00001f);
			int u = i - resolution * v - resolution / 2;
			v -= resolution / 2;

			u *= 2;
			v *= 2;

			hashes[i] = hash.Eat(u).Eat(v);
		}
```

![normal](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing-space/grid-transformation/scale-normal.png) ![double](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing-space/grid-transformation/scale-double.png)

*正常和双刻度；分辨率 32 和种子 0。*

将紫外线的尺度加倍会产生不同的图案，但看起来并没有根本的不同。我们刚刚扩展了哈希函数的域，在其中移动的速度是以前的两倍。我们也可以缩小比例，例如将 UV 除以 4。

```c#
			u /= 4;
			v /= 4;
```

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing-space/grid-transformation/scale-quarter-misalinged.png)

*四分之一缩放未对齐。*

现在，一组 4×4 的采样点最终会得到相同的哈希值，所以看起来我们降低了可视化的分辨率。然而，我们在零附近会出现错位和重复，因为整数除法有效地向零取整。

这两个问题都可以通过最初将 UV 坐标计算为 -0.5~0.5 范围内的浮点值，而不是整数来解决。然后通过 `floor` 方法舍入，从中推导出整数坐标。

```c#
			float vf = floor(invResolution * i + 0.00001f);
			float uf = invResolution * (i - resolution * vf + 0.5f) - 0.5f;
			vf = invResolution * (vf + 0.5f) - 0.5f;

			int u = (int)floor(uf);
			int v = (int)floor(vf);

			hashes[i] = hash.Eat(u).Eat(v);
		}
```

要将哈希模式转换回之前的比例，在四舍五入之前，将 UV 与我们使用的可视化分辨率（32）除以 4。

```c#
			int u = (int)floor(uf * 32f / 4f);
			int v = (int)floor(vf * 32f / 4f);
```

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing-space/grid-transformation/scale-quarter-aligned.png)

*四分之一缩放，对齐。*

### 1.2 域转换

我们将使其可配置，而不是使用固定的域规模。我们不必局限于扩展领域，我们可以将其视为一个常规的 3D 空间。我们可以对其应用任何平移、旋转和缩放，就像我们可以变换游戏对象一样。但是，我们不能单独使用 `Transform` 组件，因此让我们在一个新文件中定义一个自定义 `SpaceTRS` 结构，其中有三个公共 `float3` 字段用于转换。

```c#
using Unity.Mathematics;

public struct SpaceTRS {

	public float3 translation, rotation, scale;
}
```

将此类型的字段添加到 `HashVisualization` 中，以支持可配置的空间转换。默认情况下，将其比例设置为 8，以匹配我们当前的硬编码可视化。

```c#
	[SerializeField]
	SpaceTRS domain = new SpaceTRS {
		scale = 8f
	};
```

只有当我们的自定义 `SpaceTRS` 可以序列化时，这才有效。我们通过连接 `System.Serializable` 特性来实现这一点。

```c#
[System.Serializable]
public struct SpaceTRS { … }
```

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing-space/grid-transformation/domain-configuration.png)

*域配置。*

通过与 4×4 矩阵和向量相乘，可以对位置或方向应用 3D 空间变换。将公共 `float4x4 Matrix` getter 属性添加到返回此类矩阵的 `SpaceTRS` 中。它可以通过调用 `float4x4.TRS` 并将平移、旋转和缩放向量作为参数来创建。旋转必须是四元数。我们可以通过调用 `Quaternion.EulerZXY` 来使用 Unity 的约定创建旋转，通过 `radians` 方法将转换为弧度的旋转矢量传递给它。

```c#
	public float4x4 Matrix {
		get {
			return float4x4.TRS(
				translation, quaternion.EulerZXY(math.radians(rotation)), scale
			);
		}
	}
```

然而，由于我们仅限于平移-旋转-缩放变换，矩阵的第四行将始终为 0,0,0,1。因此，我们可以省略它来减小数据的大小，而是返回一个 `float3x4` 矩阵。这些矩阵类型之间没有直接转换，因此我们必须以矩阵的四列向量作为参数调用 `math.float3x4` 方法。这些列分别命名为 `c0`、`c1`、`c2` 和 `c3`。我们只需要每列的前三个组件，我们可以通过访问它们的 `xyz` 属性来提取它们。

```c#
	public float3x4 Matrix {
		get {
			float4x4 m = float4x4.TRS(
				translation, quaternion.EulerZXY(math.radians(rotation)), scale
			);
			return math.float3x4(m.c0.xyz, m.c1.xyz, m.c2.xyz, m.c3.xyz);
		}
	}
```

### 1.3 应用矩阵

在 `HashJob.Execute` 中应用矩阵。执行时，我们必须首先有一个 `float3` 位置。使用 -0.5 ~ 0.5 UV 坐标在 XZ 平面上创建点，然后将其 XZ 分量用于整数 UV 坐标。

```c#
		public void Execute(int i) {
			float vf = floor(invResolution * i + 0.00001f);
			float uf = invResolution * (i - resolution * vf + 0.5f) - 0.5f;
			vf = invResolution * (vf + 0.5f) - 0.5f;

			float3 p = float3(uf, 0f, vf);

			int u = (int)floor(p.x);
			int v = (int)floor(p.z);

			hashes[i] = hash.Eat(u).Eat(v);
		}
```

然后在作业中添加一个 `float3x4 domainTRS` 字段，并通过 `mul` 方法将该矩阵与点相乘。此操作需要一个 `float4` 而不是 `float3`，其第四个分量设置为 1。最后一个分量与矩阵的第四列（平移向量）相乘，因此它最终被应用而没有变化。Burst 将优化与 1 的乘法。

```c#
		public float3x4 domainTRS;

		public void Execute(int i) {
			…

			float3 p = mul(domainTRS, float4(uf, 0f, vf, 1f));

			…
		}
```

如果此时你检查 Burst 检查器，你会注意到我们的工作不再被矢量化。这是因为我们现在使用的是向量类型而不是单个值。我们暂时忽略这一限制，但稍后会处理。

要应用域转换，请在调度作业时将域矩阵传递给作业。

```c#
		new HashJob {
			hashes = hashes,
			resolution = resolution,
			invResolution = 1f / resolution,
			hash = SmallXXHash.Seed(seed),
			domainTRS = domain.Matrix
		}.ScheduleParallel(hashes.Length, resolution, default).Complete();
```

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing-space/grid-transformation/domain-rotation-y30.png)

*域围绕 Y 旋转 30°。*

我们现在可以移动、旋转和缩放用于生成哈希的域。我们看到的结果就像我们移动、旋转或缩放了用于采样哈希函数的平面一样。因此，当将域向右移动时，哈希值似乎向左移动，因为平面本身保持静止。同样，旋转似乎是朝着相反的方向进行的，而按比例放大会使图案看起来更小。

### 1.4 3D 哈希

空间变换可以是 3D 的，例如我们可以沿着 Y 移动，但这不会有什么区别，因为哈希目前只取决于 X 和 Z。同样，如果你绕 X 轴旋转，哈希方块会伸长，直到它们从平面的中心一直延伸到边缘。

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing-space/grid-transformation/domain-rotation-x90.png)

*域围绕 X 旋转 90°。*

要将模式划分为哈希立方体，我们所要做的就是在 `Execute` 中将所有三个坐标都输入到哈希中。

```c#
			int u = (int)floor(p.x);
			int v = (int)floor(p.y);
			int w = (int)floor(p.z);

			hashes[i] = hash.Eat(u).Eat(v).Eat(w);
```

现在，沿 Y 方向移动也会影响哈希，围绕一个轴旋转 90° 将产生一个方形图案，显示哈希体积的不同切片。

## 2 示例形状

鉴于我们有一个 3D 哈希体积，我们不需要限制自己用平面形状对其进行采样。我们可以创建其他作业，用不同的形状对哈希进行采样。

### 2.1 形状作业

我们不会创建不同的 `HashJob` 变体，而是将创建形状和采样哈希卷的作业分开。为此在新文件中创建一个静态 `Shapes` 类，最初包含一个 `Job` 结构，该结构为我们的平面生成 `float3` 位置并将其存储在本机数组中。域转换适用于哈希，因此不是形状作业的一部分。因此，它只需要分辨率和逆分辨率以及输入。

```c#
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

using static Unity.Mathematics.math;

public static class Shapes {

	[BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
	public struct Job : IJobFor {

		[WriteOnly]
		NativeArray<float3> positions;

		public float resolution, invResolution;

		public void Execute (int i) {
			float2 uv;
			uv.y = floor(invResolution * i + 0.00001f);
			uv.x = invResolution * (i - resolution * uv.y + 0.5f) - 0.5f;
			uv.y = invResolution * (uv.y + 0.5f) - 0.5f;

			positions[i] = float3(uv.x, 0f, uv.y);
		}
	}
}
```

还为作业提供一个静态的 `ScheduleParallel` 方法，该方法封装了作业的创建及其调度、获取和返回作业句柄。这种方法不需要将逆分辨率作为参数，因为它可以自己计算逆分辨率。

```c#
	public struct Job : IJobFor {
		
		…
		
		public static JobHandle ScheduleParallel (
			NativeArray<float3> positions, int resolution, JobHandle dependency
		) {
			return new Job {
				positions = positions,
				resolution = resolution,
				invResolution = 1f / resolution
			}.ScheduleParallel(positions.Length, resolution, dependency);
		}
	}
```

### 2.2 生成职位

我们将把这些位置提供给 `HashJob`，并在绘制实例时使用它们。因此，为 `HashVisualization` 的位置添加着色器标识符、本机数组和缓冲区。必要时创建并处理它们。

```c#
	static int
		hashesId = Shader.PropertyToID("_Hashes"),
		positionsId = Shader.PropertyToID("_Positions"),
		configId = Shader.PropertyToID("_Config");
	
	…
	
	NativeArray<uint> hashes;

	NativeArray<float3> positions;

	ComputeBuffer hashesBuffer, positionsBuffer;

	MaterialPropertyBlock propertyBlock;

	void OnEnable () {
		int length = resolution * resolution;
		hashes = new NativeArray<uint>(length, Allocator.Persistent);
		positions = new NativeArray<float3>(length, Allocator.Persistent);
		hashesBuffer = new ComputeBuffer(length, 4);
		positionsBuffer = new ComputeBuffer(length, 3 * 4);

		…
	}

	void OnDisable () {
		hashes.Dispose();
		positions.Dispose();
		hashesBuffer.Release();
		positionsBuffer.Release();
		hashesBuffer = null;
		positionsBuffer = null;
	}
```

在 `OnEnable` 中，将形状作业安排在哈希作业之前，并将句柄传递给后者，因为必须在哈希发生之前生成位置。之后还将位置发送到 GPU。

```c#
		JobHandle handle = Shapes.Job.ScheduleParallel(positions, resolution, default);

		new HashJob {
			…
		}.ScheduleParallel(hashes.Length, resolution, handle).Complete();

		hashesBuffer.SetData(hashes);
		positionsBuffer.SetData(positions);

		propertyBlock ??= new MaterialPropertyBlock();
		propertyBlock.SetBuffer(hashesId, hashesBuffer);
		propertyBlock.SetBuffer(positionsId, positionsBuffer);
```

调整 *HashGPU*，使其使用提供的位置，而不是自己生成平面。在此基础上应用垂直偏移。

```glsl
#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
	StructuredBuffer<uint> _Hashes;
	StructuredBuffer<float3> _Positions;
#endif

float4 _Config;

void ConfigureProcedural () {
	#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
		//float v = floor(_Config.y * unity_InstanceID + 0.00001);
		//float u = unity_InstanceID - _Config.x * v;
		
		unity_ObjectToWorld = 0.0;
		unity_ObjectToWorld._m03_m13_m23_m33 = float4(
			_Positions[unity_InstanceID],
			1.0
		);
		unity_ObjectToWorld._m13 +=
			_Config.z * ((1.0 / 255.0) * (_Hashes[unity_InstanceID] >> 24) - 0.5);
		unity_ObjectToWorld._m00_m11_m22 = _Config.y;
	#endif
}
```

### 2.3 使用位置作为输入

将位置作为输入添加到 `HashJob` 中，然后让它检索预先生成的位置，而不是自己计算位置。然后对其应用域转换。从现在开始，哈希不再依赖于样本分辨率，因此可以删除这些输入。

```c#
		[ReadOnly]
		public NativeArray<float3> positions;
		
		[WriteOnly]
		public NativeArray<uint> hashes;

		//public int resolution;

		//public float invResolution;
		
		public SmallXXHash hash;

		public float3x4 domainTRS;
		
		public void Execute(int i) {
			//float vf = floor(invResolution * i + 0.00001f);
			//float uf = invResolution * (i - resolution * vf + 0.5f) - 0.5f;
			//vf = invResolution * (vf + 0.5f) - 0.5f;

			float3 p = mul(domainTRS, float4(positions[i], 1f));

			…
		}
```

调整 `OnEnable` 中 `HashJob` 的创建以匹配。

```c#
		new HashJob {
			positions = positions,
			hashes = hashes,
			//resolution = resolution,
			//invResolution = 1f / resolution,
			hash = SmallXXHash.Seed(seed),
			domainTRS = domain.Matrix
		}.ScheduleParallel(hashes.Length, resolution, handle).Complete();
```

此时，我们可以像以前一样可视化哈希，但现在有两个作业按顺序运行，而不是一个作业。

### 2.4 形状转换

我们在采样时对域应用变换，也可以在生成形状时应用变换。为 `Shapes.Job` 添加 `float3x4` 位置变换，并使用它来修改 `Execute` 中的最终位置。

```c#
		public float3x4 positionTRS;

		public void Execute (int i) {
			…

			positions[i] = mul(positionTRS, float4(uv.x, 0f, uv.y, 1f));
		}
```

在这种情况下，我们可以直接使用可视化游戏对象的 `Transform` 组件中的矩阵，因此形状将按预期变换，而不是固定在原点。为了简化此操作，请在 `ScheduleParallel` 中添加一个 `float4x4` 参数，并将其相关的 3×4 部分传递给作业。

```c#
		public static JobHandle ScheduleParallel (
			NativeArray<float3> positions, int resolution,
			float4x4 trs, JobHandle dependency
		) {
			return new Job {
				positions = positions,
				resolution = resolution,
				invResolution = 1f / resolution,
				positionTRS = float3x4(trs.c0.xyz, trs.c1.xyz, trs.c2.xyz, trs.c3.xyz)
			}.ScheduleParallel(positions.Length, resolution, dependency);
		}
```

在 `HashVisualization.OnEnable` 中调度作业时，将本地到世界的转换矩阵传递给它。

```c#
		JobHandle handle = Shapes.Job.ScheduleParallel(
			positions, resolution, transform.localToWorldMatrix, default
		);
```

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing-space/sample-shapes/shape-rotated-y30.png)

*形状围绕 Y 旋转 30°。*

现在，形状会适当地响应游戏对象的变换，至少是进入游戏模式时的初始变换。请注意，各个实例保持轴对齐且不缩放。

> **我们还可以旋转和缩放实例网格吗？**
>
> 是的，通过将适当的矩阵传递给 GPU 并让 *HashGPU* 将其合并到最终矩阵中。然而，当使用非均匀尺度时，这可能会导致问题，因为我们假设尺度是均匀的。我保持原样。

### 2.5 更新位置

为了在播放模式下响应转换的更改，我们必须在 `Update` 而不是 `OnEnable` 中运行作业。但我们只需要在事情发生变化后运行它们，而不是一直运行。我们将通过在 `HashVisualization` 中添加 `bool isDirty` 字段来表明这一点。当相关内容发生变化时，我们的可视化被认为是肮脏的，需要刷新。当调用 `OnEnable` 时总是如此，因此在那里将 `isDirty` 设置为 `true`。

```c#
	bool isDirty;

	void OnEnable () {
		isDirty = true;

		…
	}
```

在 `Update` 中绘图之前，检查可视化是否脏。如果是这样，将 `isDirty` 设置为 `false`，运行作业，并用新数据填充缓冲区。

```c#
	void Update () {
		if (isDirty) {
			isDirty = false;

			JobHandle handle = Shapes.Job.ScheduleParallel(
				positions, resolution, transform.localToWorldMatrix, default
			);

			new HashJob {
				positions = positions,
				hashes = hashes,
				hash = SmallXXHash.Seed(seed),
				domainTRS = domain.Matrix
			}.ScheduleParallel(hashes.Length, resolution, handle).Complete();

			hashesBuffer.SetData(hashes);
			positionsBuffer.SetData(positions);
		}

		Graphics.DrawMeshInstancedProcedural(…);
	}
```

这意味着我们不再需要在 `OnEnable` 中执行该工作。

```c#
	void OnEnable () {
		…

		//JobHandle handle = Shapes.Job.ScheduleParallel(
		//	positions, resolution, transform.localToWorldMatrix, default
		//);

		//new HashJob {
		//	…
		//}.ScheduleParallel(hashes.Length, resolution, handle).Complete();

		//hashesBuffer.SetData(hashes);
		//positionsBuffer.SetData(positions);

		propertyBlock ??= new MaterialPropertyBlock();
		…
	}
```

最后，在发生转换更改后，在 `Update` 中刷新可视化。这由 `Transform.hasChanged` 属性表示，该属性在每次更改后设置为 `true`。它不会被设置回 `false`，我们必须在检测到变化后手动设置。

```c#
		if (isDirty || transform.hasChanged) {
			isDirty = false;
			transform.hasChanged = false;

			…
		}
```

从现在开始，在游戏模式下，形状会立即响应变换调整。这使得通过移动形状来探索哈希卷成为可能。

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing-space/sample-shapes/shape-transformed.png)

*旋转形状，缩放不均匀。*

### 2.6 位移（Displacement）

如果平面可以具有任意方向，那么我们基于哈希应用的偏移应该沿着平面的法向量，而不是始终沿着世界 Y 轴，这是有道理的。一般来说，位移的方向取决于形状，形状可能根本不是平面，在这种情况下，每个采样点都有自己的位移方向。为了表明这一点，请将 `verticalOffset` 字段替换为 `displacement` 字段，从现在开始，位移字段将表示为世界空间中的单位，而不是相对于分辨率的单位。

```c#
	//[SerializeField, Range(-2f, 2f)]
	//float verticalOffset = 1f;

	[SerializeField, Range(-0.5f, 0.5f)]
	float displacement = 0.1f;

	…

	void OnEnable () {
		…
		
		propertyBlock ??= new MaterialPropertyBlock();
		propertyBlock.SetVector(configId, new Vector4(
			resolution, 1f / resolution, displacement
		));
	}
```

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing-space/sample-shapes/displacement-inspector.png)

*位移而不是垂直偏移。*

沿着曲面法向量的位移——远离形状的曲面——需要 `Shapes.Job` 还输出法向量。在 XZ 平面的情况下，法向量总是指向正上方，尽管也应该对其应用变换。根据需要调整它和 `ScheduleParallel` 方法。

```c#
	public struct Job : IJobFor {

		[WriteOnly]
		NativeArray<float3> positions, normals;

		public float resolution, invResolution;

		public float3x4 positionTRS;

		public void Execute (int i) {
			…

			positions[i] = mul(positionTRS, float4(uv.x, 0f, uv.y, 1f));
			normals[i] = normalize(mul(positionTRS, float4(0f, 1f, 0f, 1f)));
		}

		public static JobHandle ScheduleParallel (
			NativeArray<float3> positions, NativeArray<float3> normals, int resolution,
			float4x4 trs, JobHandle dependency
		) {
			return new Job {
				positions = positions,
				normals = normals,
				…
			}.ScheduleParallel(positions.Length, resolution, dependency);
		}
	}
```

当在 GPU 上进行位移时，`HashVisualization` 必须将法向量发送到 GPU，就像位置一样。为此添加着色器标识符、数组和缓冲区。

```c#
	static int
		hashesId = Shader.PropertyToID("_Hashes"),
		positionsId = Shader.PropertyToID("_Positions"),
		normalsId = Shader.PropertyToID("_Normals"),
		configId = Shader.PropertyToID("_Config");
	
	…
	
	NativeArray<float3> positions, normals;

	ComputeBuffer hashesBuffer, positionsBuffer, normalsBuffer;

	…

	void OnEnable () {
		isDirty = true;

		int length = resolution * resolution;
		hashes = new NativeArray<uint>(length, Allocator.Persistent);
		positions = new NativeArray<float3>(length, Allocator.Persistent);
		normals = new NativeArray<float3>(length, Allocator.Persistent);
		hashesBuffer = new ComputeBuffer(length, 4);
		positionsBuffer = new ComputeBuffer(length, 3 * 4);
		normalsBuffer = new ComputeBuffer(length, 3 * 4);

		propertyBlock ??= new MaterialPropertyBlock();
		propertyBlock.SetBuffer(hashesId, hashesBuffer);
		propertyBlock.SetBuffer(positionsId, positionsBuffer);
		propertyBlock.SetBuffer(normalsId, normalsBuffer);
		…
	}

	void OnDisable () {
		hashes.Dispose();
		positions.Dispose();
		normals.Dispose();
		hashesBuffer.Release();
		positionsBuffer.Release();
		normalsBuffer.Release();
		hashesBuffer = null;
		positionsBuffer = null;
		normalsBuffer = null;
	}
```

将普通数组传递给作业，并将其复制到 `Update` 中的缓冲区。

```c#
			JobHandle handle = Shapes.Job.ScheduleParallel(
				positions, normals, resolution, transform.localToWorldMatrix, default
			);
			
			new HashJob {
				…
			}.ScheduleParallel(hashes.Length, resolution, handle).Complete();

			hashesBuffer.SetData(hashes);
			positionsBuffer.SetData(positions);
			normalsBuffer.SetData(normals);
```

然后调整 *HashGPU*，使其沿着法线而不是始终垂直移动。

```glsl
#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
	StructuredBuffer<uint> _Hashes;
	StructuredBuffer<float3> _Positions, _Normals;
#endif

float4 _Config;

void ConfigureProcedural () {
	#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
		unity_ObjectToWorld = 0.0;
		unity_ObjectToWorld._m03_m13_m23_m33 = float4(
			_Positions[unity_InstanceID],
			1.0
		);
		unity_ObjectToWorld._m03_m13_m23 +=
			(_Config.z * ((1.0 / 255.0) * (_Hashes[unity_InstanceID] >> 24) - 0.5)) *
			_Normals[unity_InstanceID];
		unity_ObjectToWorld._m00_m11_m22 = _Config.y;
	#endif
}
```

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing-space/sample-shapes/displacement-game.png)

*旋转位移。*

### 2.7 边界

游戏对象的转换也应该影响我们在绘制可视化时指定的边界。我们必须使用变换后的位置作为边界的中心。它的大小有点复杂，因为边界定义了一个轴对齐的框，而我们的形状可以旋转和缩放，如果可视化是另一个游戏对象的子对象，这可能会变得更加复杂。因此，我们将采用变换的 `lossyScale`，通过 `cmax` 方法获取其最大的绝对分量，将其加倍，添加位移，并将其用于所有三维的最终缩放。这不是最合适的，但它很容易计算，对我们的目的来说已经足够好了。

```c#
		Graphics.DrawMeshInstancedProcedural(
			instanceMesh, 0, material,
			new Bounds(
				transform.position,
				float3(2f * cmax(abs(transform.lossyScale)) + displacement)
			),
			hashes.Length, propertyBlock
		);
```

我们不必在每一帧都重新计算边界，只有在可视化不干净时才需要重新计算。因此，我们可以将其存储在字段中并重复使用。

```c#
	Bounds bounds;

	…

	void Update () {
		if (isDirty || transform.hasChanged) {
			…

			bounds = new Bounds(
				transform.position,
				float3(2f * cmax(abs(transform.lossyScale)) + displacement)
			);
		}

		Graphics.DrawMeshInstancedProcedural(
			instanceMesh, 0, material, bounds, hashes.Length, propertyBlock
		);
	}
```

## 3 手动矢量化

如前所述，在引入向量类型后，我们的作业的自动矢量化失败了。典型的自动矢量化是通过将对 `float`、`int` 和其他此类原始值执行的操作替换为对 `float4`、`int4` 等执行的相同操作来实现的。然后，该作业利用 SIMD 指令并行执行四次迭代。不幸的是，这对我们的工作来说已经不可能了，因为我们对位置和法向量使用 `float3` 值。但通过一些工作，手动矢量化是可能的。

### 3.1 矢量化哈希

我们的 `SmallXXHash` 结构在设计时考虑了自动矢量化。为了支持手动矢量化，我们需要创建一个新的 `SmallXXHash4` 变体，它并行处理四个值的向量。复制并重命名结构，将两者保存在同一文件中。从现在开始，我们还需要使用这里的数学库。

```c#
using Unity.Mathematics;

public readonly struct SmallXXHash { … }

public readonly struct SmallXXHash4 { … }
```

确保 `SmallXXHash4` 中对其自身类型的所有引用都指向向量版本，而不是单值版本。此外，由于字节没有向量类型，因此从中删除该方法以及 `primeA` 常量，因为没有其他方法使用该值。

```c#
public readonly struct SmallXXHash4 {

	//const uint primeA = 0b10011110001101110111100110110001;
	const uint primeB = 0b10000101111010111100101001110111;
	const uint primeC = 0b11000010101100101010111000111101;
	const uint primeD = 0b00100111110101001110101100101111;
	const uint primeE = 0b00010110010101100110011110110001;

	readonly uint accumulator;

	public SmallXXHash4 (uint accumulator) {
		this.accumulator = accumulator;
	}

	public static implicit operator SmallXXHash4 (uint accumulator) =>
		new SmallXXHash4(accumulator);

	public static SmallXXHash4 Seed (int seed) => (uint)seed + primeE;

	static uint RotateLeft (uint data, int steps) =>
		(data << steps) | (data >> 32 - steps);

	public SmallXXHash4 Eat (int data) =>
		RotateLeft(accumulator + (uint)data * primeC, 17) * primeD;

	//public SmallXXHash Eat (byte data) =>
	//	RotateLeft(accumulator + data * primeE, 11) * primeA;

	public static implicit operator uint (SmallXXHash4 hash) { … }
}
```

接下来，将所有出现的 `int` 和 `uint` 类型替换为 `int4` 和 `uint4`。唯一的例外是 `RotateLeft` 的 `steps` 参数，它必须保持为单个 `int`。

```c#
	readonly uint4 accumulator;

	public SmallXXHash4 (uint4 accumulator) {
		this.accumulator = accumulator;
	}

	public static implicit operator SmallXXHash4 (uint4 accumulator) =>
		new SmallXXHash4(accumulator);

	public static SmallXXHash4 Seed (int4 seed) => (uint4)seed + primeE;
	
	static uint4 RotateLeft (uint4 data, int steps) =>
		(data << steps) | (data >> 32 - steps);

	public SmallXXHash4 Eat (int4 data) =>
		RotateLeft(accumulator + (uint4)data * primeC, 17) * primeD;

	public static implicit operator uint4 (SmallXXHash4 hash) {
		uint4 avalanche = hash.accumulator;
		…
	}
```

这足以对 `SmallXXHash4` 进行矢量化。让我们通过在 `SmallXXHash` 中添加一个运算符，隐式地从单值版本转换为矢量化版本。

```c#
public readonly struct SmallXXHash {

	…

	public static implicit operator SmallXXHash4 (SmallXXHash hash) =>
		new SmallXXHash4(hash.accumulator);
}
```

> **我们也可以向另一个方向进行转换吗？**
>
> 从一个哈希到向量哈希的转换只是复制单个值的问题。但是，当我们采取另一种方式时，我们必须以某种方式将四个独立的哈希值减少到一个哈希值。没有直接的方法可以做到这一点，所以我们不会为它添加转换。

### 3.2 矢量化哈希作业

接下来，我们将矢量化 `HashJob`。首先，将 `hashes` 输出和 `hash` 输入替换为它们的四个组件变体。

```c#
		[WriteOnly]
		public NativeArray<uint4> hashes;

		public SmallXXHash4 hash;
```

我们还需要同时处理四个职位。四个位置的序列可以存储在一个 `float3x4` 矩阵值中，每个列都包含一个位置。

```c#
		[ReadOnly]
		public NativeArray<float3x4> positions;
```

然而，为了将我们的计算矢量化，`Execute` 需要为 x、y 和 z 分量提供单独的向量，而不是每个位置一个向量。我们可以通过转置位置矩阵来实现这一点——通过转置方法——这给了我们一个具有所需布局的 `float4x3` 矩阵。然后我们也可以对 u、v 和 w 提取进行矢量化，暂时省略域变换。

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing-space/manual-vectorization/transposing-matrix.png)


将 3×4 转换为 4×3。

```c#
		public void Execute(int i) {
			float4x3 p = transpose(positions[i]);

			int4 u = (int4)floor(p.c0);
			int4 v = (int4)floor(p.c1);
			int4 w = (int4)floor(p.c2);

			hashes[i] = hash.Eat(u).Eat(v).Eat(w);
		}
```

现在，我们需要将矢量化数组传递给 `Update` 中的作业。我们已经有了所需的数组，我们所要做的就是重新解释它们，就像每个元素都包含四个 `float3` 值一样。`NativeArray` 允许我们通过调用所需类型的泛型 `Reinterpret` 方法来实现这一点，并将原始元素大小作为参数。由于这将作业的有效数组长度减少到四分之一，我们还必须将计划作业长度除以四。

```c#
			new HashJob {
				positions = positions.Reinterpret<float3x4>(3 * 4),
				hashes = hashes.Reinterpret<uint4>(4),
				hash = SmallXXHash.Seed(seed),
				domainTRS = domain.Matrix
			}.ScheduleParallel(hashes.Length / 4, resolution, handle).Complete();
```

请注意，这要求数组长度能被四整除，否则重新解释将失败。因此，请确保分辨率是偶数的。

### 3.3 矢量化变换

没有现有的 `math` 方法可以将 3×4 TRS 矩阵和 4×3 XYZ 列矩阵相乘。将 `TransformPositions` 方法添加到 `HashJob` 中，以上述矩阵为参数，为我们完成此操作，返回一个转换后的 4×3 XYZ 列矩阵。最初，它可以返回未转换的位置。然后在 `Execute` 中使用它。

```c#
		float4x3 TransformPositions (float3x4 trs, float4x3 p) => p;

		public void Execute(int i) {
			float4x3 p = TransformPositions(domainTRS, transpose(positions[i]));

			…
		}
```

转换是一个常规的矩阵乘法，除了第四个 TRS 列（包含转换）只是简单地相加。在前面的矩阵乘法中，最后一部分与常数 1 相乘，但我们可以省略它。

因为位置分量是矢量化的，所以乘法步骤是在位置矩阵的整列上执行的，而不是在单个分量上执行的。

```c#
		float4x3 TransformPositions (float3x4 trs, float4x3 p) => float4x3(
			trs.c0.x * p.c0 + trs.c1.x * p.c1 + trs.c2.x * p.c2 + trs.c3.x,
			trs.c0.y * p.c0 + trs.c1.y * p.c1 + trs.c2.y * p.c2 + trs.c3.y,
			trs.c0.z * p.c0 + trs.c1.z * p.c1 + trs.c2.z * p.c2 + trs.c3.z
		);
```

此时，我们的可视化工作与以前一样，只是 `HashJob` 现在被矢量化了。Burst 检查器的诊断视图仍然表明作业没有矢量化，因为 Burst 不知道我们是手动完成的。检查程序集将显示该作业确实使用 SIMD 指令并并行生成四个哈希值。

### 3.4 矢量化形状作业

最后，我们使用相同的方法将 `Shapes.Job` 矢量化。首先，将 position 和法线数组的元素类型更改为 `float3x4`。

```c#
		[WriteOnly]
		NativeArray<float3x4> positions, normals;
		
		…
		
		public static JobHandle ScheduleParallel (
			NativeArray<float3x4> positions, NativeArray<float3x4> normals,
			int resolution, float4x4 trs, JobHandle dependency
		) { … }
```

在 `Execute` 中，使用 4×2 UV 列矩阵对 UV 坐标的计算进行矢量化。

```c#
			float4x2 uv;
			uv.c1 = floor(invResolution * i + 0.00001f);
			uv.c0 = invResolution * (i - resolution * uv.c1 + 0.5f) - 0.5f;
			uv.c1 = invResolution * (uv.c1 + 0.5f) - 0.5f;
```

我们必须用相应的四个矢量化索引替换单个索引参数，这是通过将原始索引乘以四，然后将零、1、2 和 3 加到各个索引上来完成的。

```c#
			float4 i4 = 4f * i + float4(0f, 1f, 2f, 3f);
			uv.c1 = floor(invResolution * i4 + 0.00001f);
			uv.c0 = invResolution * (i4 - resolution * uv.c1 + 0.5f) - 0.5f;
```

接下来，我们还需要应用 TRS 变换，但现在是针对位置和法向量。要使用单个方法同时支持这两种方法，请将 `TransformPositions` 从 `HashJob` 复制到 `Shapes.Job`，将其重命名为 `TransformVectors`，并添加一个默认设置为 1 的 `float w` 参数。将翻译部分乘以此值。

```c#
		float4x3 TransformVectors (float3x4 trs, float4x3 p, float w = 1f) => float4x3(
			trs.c0.x * p.c0 + trs.c1.x * p.c1 + trs.c2.x * p.c2 + trs.c3.x * w,
			trs.c0.y * p.c0 + trs.c1.y * p.c1 + trs.c2.y * p.c2 + trs.c3.y * w,
			trs.c0.z * p.c0 + trs.c1.z * p.c1 + trs.c2.z * p.c2 + trs.c3.z * w
		);
```

在 `Execute` 中，生成具有平面位置的 4×3 XYZ 列矩阵，应用位置 TRS，然后将其转置，以便将其分配给位置输出元素。

```c#
			positions[i] =
				transpose(TransformVectors(positionTRS, float4x3(uv.c0, 0f, uv.c1)));
```

对法向量执行相同的操作，但这次将零作为 `TransformVectors` 的第三个参数传递，因此忽略转换。Burst将消除任何乘以常数零的东西。然后对存储在 3×4 矩阵列中的向量进行归一化。

```c#
			float3x4 n =
				transpose(TransformVectors(positionTRS, float4x3(0f, 1f, 0f), 0f));
			normals[i] = float3x4(
				normalize(n.c0), normalize(n.c1), normalize(n.c2), normalize(n.c3)
			);
```

### 3.5 矢量化数组

因为这两个作业现在都需要矢量化数组，所以让我们在 `HashVisualization` 中直接定义数组，而不是重新解释它们。

```c#
	NativeArray<uint4> hashes;

	NativeArray<float3x4> positions, normals;
```

在 `OnEnable` 中，在创建数组和缓冲区之前，将计算出的长度除以 4。

```c#
	void OnEnable () {
		isDirty = true;

		int length = resolution * resolution;
		length /= 4;
		hashes = new NativeArray<uint4>(length, Allocator.Persistent);
		positions = new NativeArray<float3x4>(length, Allocator.Persistent);
		normals = new NativeArray<float3x4>(length, Allocator.Persistent);
		…
	}
```

为了保持计算缓冲区的大小不变，我们现在必须将其长度增加四倍。

```c#
		hashesBuffer = new ComputeBuffer(length * 4, 4);
		positionsBuffer = new ComputeBuffer(length * 4, 3 * 4);
		normalsBuffer = new ComputeBuffer(length * 4, 3 * 4);
```

这要求我们在将数据复制到 `Update` 中的缓冲区时，仍然需要重新解释一次。

```c#
			hashesBuffer.SetData(hashes.Reinterpret<uint>(4 * 4));
			positionsBuffer.SetData(positions.Reinterpret<float3>(3 * 4 * 4));
			normalsBuffer.SetData(normals.Reinterpret<float3>(3 * 4 * 4));
```

> **我们不能直接将矢量化数据复制到计算缓冲区吗？**
>
> 尽管 CPU 和 GPU 都以自己的方式解释计算缓冲区数据，但事实证明，在某些情况下可能会发生数据错位。因此，我们必须确保计算缓冲区明确使用未矢量化的数据。

此时，我们也可以再次支持奇怪的决议。例如，如果分辨率为 3，则初始长度为 9，矢量化长度将变为 2，仅支持 8 个元素。我们可以通过在最终长度上加 1 来拟合第九个元素。这意味着我们将添加四个值，其中三个是多余的，但这只是微不足道的开销。我们可以通过将初始长度的最低有效位添加到 `OnEnable` 中的矢量化长度中，使其适用于所有分辨率，因为偶数为零，奇数为 1。

```c#
		int length = resolution * resolution;
		length = length / 4 + (length & 1);
```

我们不再需要在 `Update` 中重新解释 `HashJob` 的数组。在调度时，我们还必须再次使用数组的长度，因为它已经通过矢量化减少了。

```c#
			new HashJob {
				positions = positions,
				hashes = hashes,
				hash = SmallXXHash.Seed(seed),
				domainTRS = domain.Matrix
			}.ScheduleParallel(hashes.Length, resolution, handle).Complete();
```

最后，要绘制正确数量的实例，请将方形分辨率传递给 `Graphics.DrawMeshInstancedProcedural`。

```c#
		Graphics.DrawMeshInstancedProcedural(
			instanceMesh, 0, material, bounds, resolution * resolution, propertyBlock
		);
```

## 4 更多形状

现在我们的两个作业都矢量化了，我们将添加两个备选的示例形状。我们可以通过创造额外的工作来实现这一点，但这些工作的大部分代码都是相同的。因此，我们将转而采用基于模板的方法。

### 4.1 平面结构

为了支持多种形状，我们将提升从 `Shapes.Job` 中生成位置和法线的代码进入 `Shapes` 类。每个形状都可以以自己独特的方式生成，但我们将把它们都建立在 UV 网格上。为了避免代码重复，请从 `Job.Execute` 复制 UV 代码。在 `Shapes` 中执行一个新的静态 `IndexTo4UV` 方法。通过省略 0.5 的减法，调整它，使 UV 范围变为 0~1，而不是 -0.5~0.5。

```c#
public static class Shapes {
	
	public static float4x2 IndexTo4UV (int i, float resolution, float invResolution) {
		float4x2 uv;
		float4 i4 = 4f * i + float4(0f, 1f, 2f, 3f);
		uv.c1 = floor(invResolution * i4 + 0.00001f);
		uv.c0 = invResolution * (i4 - resolution * uv.c1 + 0.5f);
		uv.c1 = invResolution * (uv.c1 + 0.5f);
		return uv;
	}
	
	…
}
```

每个形状作业的唯一独特之处在于它如何设置位置和法线。为了传递这些数据，在 `Shapes` 中引入一个 `Point4` 结构，它是一个用于矢量化位置和法线的简单容器。

```c#
public static class Shapes {

	public struct Point4 {
		public float4x3 positions, normals;
	}

	…
}
```

从 `Job.Execute` 中提取生成平面的代码，将 `Plane` 结构类型添加到包含 `GetPoint4` 方法的 `Shapes` 中，该方法使用索引、分辨率和逆分辨率来生成 `Point4` 值。因为我们改变了 UV 的范围，所以现在必须从这里的 UV 中减去 0.5，以保持平面以原点为中心。

```c#
	public struct Plane {

		public Point4 GetPoint4 (int i, float resolution, float invResolution) {
			float4x2 uv = IndexTo4UV(i, resolution, invResolution);
			return new Point4 {
				positions = float4x3(uv.c0 - 0.5f, 0f, uv.c1 - 0.5f),
				normals = float4x3(0f, 1f, 0f)
			};
		}
	}
```

`Plane` 结构体不包含任何字段，它的唯一目的是提供 `GetPoint4` 方法。我们可以在 `Job.Execute` 中访问此方法。通过 `default(Plane)` 在默认的 `Plane` 值上调用它来执行。这将替换显式平面相关代码。

```c#
		public void Execute (int i) {
			//float4x2 uv;
			//float4 i4 = 4f * i + float4(0f, 1f, 2f, 3f);
			//uv.c1 = floor(invResolution * i4 + 0.00001f);
			//uv.c0 = invResolution * (i4 - resolution * uv.c1 + 0.5f) - 0.5f;
			//uv.c1 = invResolution * (uv.c1 + 0.5f) - 0.5f;
			
			Point4 p = default(Plane).GetPoint4(i, resolution, invResolution);

			positions[i] = transpose(TransformVectors(positionTRS, p.positions));

			float3x4 n = transpose(TransformVectors(positionTRS, p.normals, 0f));
			normals[i] = float3x4(
				normalize(n.c0), normalize(n.c1), normalize(n.c2), normalize(n.c3)
			);
		}
```

### 4.2 形状接口

我们的想法是，我们应该能够用不同的结构类型替换 `Plane` 来生成不同的形状。为了使这适用于泛型形状，我们将引入一个接口类型，它就像类或结构的契约，强制要求它们必须具有哪些公共方法或属性。

接口是用 `interface` 关键字声明的，约定是在其类型名称前加上 I，因此我们将其命名为 `IShape`。在其中定义 GetPoint4 方法签名，因此没有代码体，但以分号终止。接口成员根据定义是公共的，因此它没有显式的访问修饰符。然后，`Plane` 可以通过扩展来实现该接口。

```c#
	public interface IShape {
		Point4 GetPoint4 (int i, float resolution, float invResolution);
	}

	public struct Plane : IShape {

		public Point4 GetPoint4 (int i, float resolution, float invResolution) { … }
	}
```

### 4.3 通用作业

下一步是使我们的作业通用化，将其转化为作业模板。我们通过在 `Job` 的类型声明中附加一个尖括号内的泛型类型参数来实现这一点。按照惯例，类型参数名称是单个字母。由于此参数将表示形状类型，因此将其命名为 `s`。

```c#
	public struct Job<S> : IJobFor { … }
```

我们想通过两种方式限制 `S` 的值。首先，我们希望它是一个结构类型。其次，它必须实现 `IShape` 接口。我们可以通过在 `Job` 的类型声明后附加 `where S: struct`、`IShape` 来声明这一点。

```c#
	public struct Job<S> : IJobFor where S : struct, IShape { … }
```

现在我们可以在 `Execute` 中使用泛型类型，而不是显式类型。

```c#
		public void Execute (int i) {
			Point4 p = default(S).GetPoint4(i, resolution, invResolution);

			…
		}
```

我们还必须指定在 `ScheduleParallel` 方法中创建的作业类型，就像创建 `NativeArray` 或其他泛型类型值一样。它应该是相同的作业类型，因此我们传递 `S` 作为泛型参数。

```c#
			return new Job<S> {
				…
			}.ScheduleParallel(positions.Length, resolution, dependency);
```

最后，为了完成这项工作，我们必须明确我们在 `HashVisualization.Update` 中调度的形状作业。

```c#
			JobHandle handle = Shapes.Job<Shapes.Plane>.ScheduleParallel(
				positions, normals, resolution, transform.localToWorldMatrix, default
			);
```

请注意，我们的平面作业现在在 Burst 检查器中被列为“*Shapes.Job`1[Shapes.Plane]* ”，而不仅仅是 *Shapes.Job*。除此之外，生成的汇编代码与以前相同。

### 4.4 球体和圆环

现在可以轻松添加更多形状作业。我们将再添加两个，从一个球体开始。我们可以复制并调整[数学曲面](https://catlikecoding.com/unity/tutorials/basics/mathematical-surfaces/)教程中的代码。唯一的区别是，由于 UV 范围不同，我们必须将 `sin` 和 `cos` 方法的所有参数加倍，并且必须交换 `sin` 和 `cos` 来计算 `s` 和 `c1`。我们将其半径设置为 0.5，使其适合单位立方体。

因为它是一个球体，我们可以直接使用法向量的位置。这些向量的长度为 0.5，但这不是问题，因为作业在应用空间变换后会对它们进行归一化。

```c#
	public struct Plane : IShape { … }
	
	public struct Sphere : IShape {

		public Point4 GetPoint4 (int i, float resolution, float invResolution) {
			float4x2 uv = IndexTo4UV(i, resolution, invResolution);

			float r = 0.5f;
			float4 s = r * sin(PI * uv.c1);

			Point4 p;
			p.positions.c0 = s * sin(2f * PI * uv.c0);
			p.positions.c1 = r * cos(PI * uv.c1);
			p.positions.c2 = s * cos(2f * PI * uv.c0);
			p.normals = p.positions;
			return p;
		}
	}
```

要使用球体形状，我们所要做的就是更改 `HashVisualization.Update` 中的 `Shapes.Job` 的泛型类型参数。

```c#
			JobHandle handle = Shapes.Job<Shapes.Sphere>.ScheduleParallel(
				positions, normals, resolution, transform.localToWorldMatrix, default
			);
```

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing-space/more-shapes/uv-sphere.png)

*球形，零位移。*

我们添加的第二个形状是环面，也是从[数学曲面](https://catlikecoding.com/unity/tutorials/basics/mathematical-surfaces/)复制的，再次将 `sin` 和 `cos` 方法的参数加倍。`r1` 使用 0.375，`r2` 使用 0.125。

```c#
	public struct Torus : IShape {

		public Point4 GetPoint4 (int i, float resolution, float invResolution) {
			float4x2 uv = IndexTo4UV(i, resolution, invResolution);

			float r1 = 0.375f;
			float r2 = 0.125f;
			float4 s = r1 + r2 * cos(2f * PI * uv.c1);

			Point4 p;
			p.positions.c0 = s * sin(2f * PI * uv.c0);
			p.positions.c1 = r2 * sin(2f * PI * uv.c1);
			p.positions.c2 = s * cos(2f * PI * uv.c0);
			p.normals = p.positions;
			return p;
		}
	}
```

环面的曲面法线比球体的曲面法线复杂一些。它们必须指向远离圆环内环的方向，而不是全部指向远离中心的方向。我们不需要担心在这里避免重复计算，因为 Burst 消除了这些重复计算。

```c#
			p.normals = p.positions;
			p.normals.c0 -= r1 * sin(2f * PI * uv.c0);
			p.normals.c2 -= r1 * cos(2f * PI * uv.c0);
```

现在我们可以切换到环形采样。

```c#
			JobHandle handle = Shapes.Job<Shapes.Torus>.ScheduleParallel(
				positions, normals, resolution, transform.localToWorldMatrix, default
			);
```

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing-space/more-shapes/torus.png)

*环形，零位移。*

### 4.5 选择形状

为了通过检查器切换形状，我们必须添加代码，在正确的作业类型上调用 `ScheduleParallel`。我们将再次基于[数学曲面](https://catlikecoding.com/unity/tutorials/basics/mathematical-surfaces/)，使用选择枚举和静态委托数组。

将 `ScheduleDelegate` 委托类型添加到与 `ScheduleParallel` 签名匹配的形状中。

```c#
	public delegate JobHandle ScheduleDelegate (
		NativeArray<float3x4> positions, NativeArray<float3x4> normals,
		int resolution, float4x4 trs, JobHandle dependency
	);
```

哪些形状可用取决于 `HashVisualization`，因此我们将在那里添加选择枚举和委托数组，以及形状配置字段。

```c#
	public enum Shape { Plane, Sphere, Torus }

	static Shapes.ScheduleDelegate[] shapeJobs = {
		Shapes.Job<Shapes.Plane>.ScheduleParallel,
		Shapes.Job<Shapes.Sphere>.ScheduleParallel,
		Shapes.Job<Shapes.Torus>.ScheduleParallel
	};
	
	…
	
	[SerializeField]
	Shape shape;
```

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing-space/more-shapes/shape-inspector.png)

*形状下拉选择菜单。*

然后调整 `Update`，以生成选定的形状。

```c#
			JobHandle handle = shapeJobs[(int)shape](
				positions, normals, resolution, transform.localToWorldMatrix, default
			);
```

### 4.6 转换法线

此时，除了对球体或圆环使用非均匀比例外，一切似乎都按预期工作。例如，当球体几乎变平时，位移仍然远离其中心，而不是直接远离其隐含表面。结果是位移太平。

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing-space/more-shapes/scaled-normals-incorrect.png)

*Y 刻度为 0.01 和 0.5 位移的球体。*

这是因为非均匀缩放会扰乱法向量。它们必须与不同的变换矩阵相乘。要解决这个问题，请向形状添加单独的法线变换输入。作业并使用它来变换法线。

```c#
		public float3x4 positionTRS, normalTRS;

		…

		public void Execute (int i) {
			…

			float3x4 n = transpose(TransformVectors(normalTRS, p.normals, 0f));
			…
		}
```

为了产生正确的曲面法向量，我们必须用逆 4×4 TRS 矩阵的转置对其进行变换，这可以通过 `ScheduleParallel` 中的 `transpose(inverse(trs))` 得到。

```c#
		public static JobHandle ScheduleParallel (
			NativeArray<float3x4> positions, NativeArray<float3x4> normals,
			int resolution,	float4x4 trs, JobHandle dependency
		) {
			float4x4 tim = transpose(inverse(trs));
			return new Job<S> {
				…
				positionTRS = float3x4(trs.c0.xyz, trs.c1.xyz, trs.c2.xyz, trs.c3.xyz),
				normalTRS = float3x4(tim.c0.xyz, tim.c1.xyz, tim.c2.xyz, tim.c3.xyz)
			}.ScheduleParallel(positions.Length, resolution, dependency);
		}
```

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing-space/more-shapes/scaled-normals-correct.png)

*正确的位移。*

### 4.7 八面体球体

我们当前生成的球体被称为 UV 球体。它由在两极退化为点的环组成。点的分布显然不均匀。在两极附近，点靠得太近，在赤道附近，点相距太远。所以，让我们切换到另一种方法，生成一个八面体球体。

八面体球体是通过首先在原点生成八面体，然后对其所有位置向量进行归一化而制成的。通过几个步骤，可以从 0~1 UV 坐标生成八面体。我们从以原点为中心的 XY 平面开始。

```c#
	public struct Sphere : IShape {

		public Point4 GetPoint4 (int i, float resolution, float invResolution) {
			float4x2 uv = IndexTo4UV(i, resolution, invResolution);

			//float r = 0.5f;
			//float4 s = r * sin(PI * uv.c1);

			Point4 p;
			p.positions.c0 = uv.c0 - 0.5f;
			p.positions.c1 = uv.c1 - 0.5f;
			p.positions.c2 = 0f;
			p.normals = p.positions;
			return p;
		}
	}
```

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing-space/more-shapes/octahedron-plane.png)

*XY 平面，零位移。*

第二步是使 Z 等于 0.5 减去绝对 X 和绝对 Z，沿 Z 方向移动平面以创建刻面。这会产生一个看起来像八面体的东西，在一侧折叠打开。

```c#
			p.positions.c2 = 0.5f - abs(p.positions.c0) - abs(p.positions.c1);
```

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing-space/more-shapes/octahedron-folded-open.png)

*八面体折叠打开。*

在正 Z 侧，八面体已经完成。为了闭合负 Z 侧的八面体，我们需要一个等于负 Z 的偏移量，最小值为零，以使正侧不受影响。

```c#
			p.positions.c2 = 0.5f - abs(p.positions.c0) - abs(p.positions.c1);
			float4 offset = max(-p.positions.c2, 0f);
```

我们必须独立地在 X 和 Y 上加上或减去这个偏移量。如果 X 为负，则加，否则减。Y 也是如此。为了对矢量数据进行选择，我们必须使用 `select` 方法。其签名为 `select(valueIfFalse, valueIfTrue, condition)`。

```c#
			float4 offset = max(-p.positions.c2, 0f);
			p.positions.c0 += select(-offset, offset, p.positions.c0 < 0f);
			p.positions.c1 += select(-offset, offset, p.positions.c1 < 0f);
```

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing-space/more-shapes/octahedron-closed.png)

*八面体闭合。*

最后，要将八面体变成半径为 0.5 的球体，我们必须将 0.5 除以向量长度，为此我们可以使用毕达哥拉斯定理和 `rsqrt` 方法。

```c#
			p.positions.c1 += select(-offset, offset, p.positions.c1 < 0f);

			float4 scale = 0.5f * rsqrt(
				p.positions.c0 * p.positions.c0 +
				p.positions.c1 * p.positions.c1 +
				p.positions.c2 * p.positions.c2
			);
			p.positions.c0 *= scale;
			p.positions.c1 *= scale;
			p.positions.c2 *= scale;
			p.normals = p.positions;
```

![octahedron sphere](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing-space/more-shapes/octahedron-sphere.png) ![uv sphere](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing-space/more-shapes/uv-sphere.png)

*八面体球和 UV 球。*

与 UV 球体相比，八面体球体有六个而不是两个点聚集在一起的区域，但它的点分布更均匀。

### 4.8 实例规模

球体和圆环的采样点比平面的采样点相距更远，这使得由于实例之间的空白空间而更难看到它们的表面。

![sphere](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing-space/more-shapes/instance-scale-1-sphere.png) ![torus](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing-space/more-shapes/instance-scale-1-torus.png)

*分辨率 64，位移为 0.1。*

我们通过在 `HashVisualization` 中添加一个可配置的实例规模来结束本教程，这可用于使可视化更加坚固，甚至更加稀疏。

```c#
	[SerializeField, Range(0.1f, 10f)]
	float instanceScale = 2f;
```

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing-space/more-shapes/instance-scale-inspector.png)

*实例缩放滑块。*

实例比例是通过将其除以 `OnEnable` 中的分辨率并将其发送到 GPU 来应用的，而不是反向分辨率。

```c#
		propertyBlock.SetVector(configId, new Vector4(
			resolution, instanceScale / resolution, displacement
		));
```

![sphere](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing-space/more-shapes/instance-scale-2-sphere.png) ![torus](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing-space/more-shapes/instance-scale-2-torus.png)

*实例规模 2。*

下一个教程是“[值噪波](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/value-noise/)”。

[license](https://catlikecoding.com/unity/tutorials/license/)

[repository](https://bitbucket.org/catlikecodingunitytutorials/pseudorandom-noise-02-hashing-space/)

[PDF](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/hashing-space/Hashing-Space.pdf)



# 值噪声：Lattice 噪声

发布于 2021-05-20 更新于 2021-07-15

https://catlikecoding.com/unity/tutorials/pseudorandom-noise/value-noise/

*创建一个抽象可视化类。*
*为噪音引入一个通用的工作。*
*生成 1D、2D 和 3D 值噪声。*

这是关于[伪随机噪声](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/)的系列教程中的第三篇。它涵盖了从纯哈希到最简单形式的晶格（Lattice）噪声的过渡。

本教程使用 Unity 2020.3.6f1 编写。

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/value-noise/tutorial-image.jpg)

*显示3D值噪波的球体。*

## 1 可重用可视化

我们的哈希可视化显示了哈希函数如何基于整数坐标将空间划分为离散的值块。这个想法是，噪声函数使用这些哈希值来产生一个不那么块状的模式。在本教程中，我们将具体实现值噪声，它平滑了块状哈希模式。因此，噪声函数的输出产生连续模式，产生浮点值而不是离散位模式。这需要一个与我们目前类似但不同的噪声可视化。

我们可以复制 `HashVisualization` 中的代码，并将其重新用于 `NoiseVisualization` 类，但这会引入大量重复代码。我们将通过引入一个抽象的 `Visualization` 类来使用继承来避免这种冗余，该类将作为哈希和噪声可视化的基础。

### 1.1 抽象可视化类

复制 `HashVisualization` C# 资产并将其重命名为 `Visualization`，然后删除作业以及与哈希、哈希种子和哈希域直接相关的所有字段。

```c#
//using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

using static Unity.Mathematics.math;

public class Visualization : MonoBehaviour {

	//[BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
	//struct HashJob : IJobFor { … }

	…

	static int
		//hashesId = Shader.PropertyToID("_Hashes"),
		positionsId = Shader.PropertyToID("_Positions"),
		normalsId = Shader.PropertyToID("_Normals"),
		configId = Shader.PropertyToID("_Config");

	…

	//[SerializeField]
	//int seed;

	//[SerializeField]
	//SpaceTRS domain = new SpaceTRS {
		//scale = 8f
	//};
	
	//NativeArray<uint4> hashes;

	NativeArray<float3x4> positions, normals;

	//ComputeBuffer hashesBuffer, positionsBuffer, normalsBuffer;
	ComputeBuffer positionsBuffer, normalsBuffer;

	…
	
}
```

清理 `OnEnable` 和 `OnDisable` 方法以匹配。

```c#
	void OnEnable () {
		isDirty = true;

		int length = resolution * resolution;
		length = length / 4 + (length & 1);
		//hashes = new NativeArray<uint4>(length, Allocator.Persistent);
		positions = new NativeArray<float3x4>(length, Allocator.Persistent);
		normals = new NativeArray<float3x4>(length, Allocator.Persistent);
		//hashesBuffer = new ComputeBuffer(length * 4, 4);
		positionsBuffer = new ComputeBuffer(length * 4, 3 * 4);
		normalsBuffer = new ComputeBuffer(length * 4, 3 * 4);

		propertyBlock ??= new MaterialPropertyBlock();
		//propertyBlock.SetBuffer(hashesId, hashesBuffer);
		…
	}

	void OnDisable () {
		//hashes.Dispose();
		positions.Dispose();
		normals.Dispose();
		//hashesBuffer.Release();
		positionsBuffer.Release();
		normalsBuffer.Release();
		//hashesBuffer = null;
		positionsBuffer = null;
		normalsBuffer = null;
	}
```

并更改 `OnValidate`，使其检查位置缓冲区而不是哈希缓冲区。

```c#
	void OnValidate () {
		if (positionsBuffer != null && enabled) {
			OnDisable();
			OnEnable();
		}
	}
```

然后从 `Update` 中删除哈希作业的调度和哈希缓冲区的设置。

```c#
	void Update () {
		if (isDirty || transform.hasChanged) {
			…

			//new HashJob {
			//	…
			//}.ScheduleParallel(hashes.Length, resolution, handle).Complete();

			//hashesBuffer.SetData(hashes.Reinterpret<uint>(4 * 4));
			…
		}

		…
	}
```

我们剩下的是一个类，它做可视化所需的一切，除了计算和存储要可视化的数据。因此，它本身是无用的，将这种类型的组件附加到游戏对象上是没有意义的。为了表明这一点，我们将类标记为 `abstract`。

```c#
public abstract class Visualization : MonoBehaviour { … }
```

这使得无法创建 `Visualization` 类型的直接实例。

### 1.2 抽象方法

无论我们使用 `Visualization` 做什么，它都必须支持启用和禁用。它已经有了 `OnEnable` 和 `OnDisable` 方法，但这些方法不会创建或删除本机数组、缓冲区或可视化数据所需的其他任何东西。让我们假设这些工作是在专用的 `EnableVisualization` 和 `DisableVisualization` 方法中完成的，并将它们添加到 `Visualization` 中。由于我们不知道这些方法中的代码是什么，我们只将它们声明为 `abstract` 签名，类似于接口的契约。我们可以这样做，因为类本身也是抽象的，所以我们可以省略它的部分实现。

```c#
	abstract void EnableVisualization ();

	abstract void DisableVisualization ();
```

启用可视化时，需要数据长度和材质属性块，因此将它们作为参数添加到 `EnableVisualization` 中。

```c#
	abstract void EnableVisualization (
		int dataLength, MaterialPropertyBlock propertyBlock
	);
```

一旦我们在 `OnEnable` 中有了属性块，就调用 `EnableVisualization`。在 `OnDisable` 结束时调用 `DisableVisualization`。

```c#
	void OnEnable () {
		…
		
		propertyBlock ??= new MaterialPropertyBlock();
		EnableVisualization(length, propertyBlock);
		…
	}

	void OnDisable () {
		…
		DisableVisualization();
	}
```

在更新可视化时，我们还必须执行一些未知的工作。为此添加一个抽象的 `UpdateVisualization` 方法，其中位置、分辨率和作业句柄的原生数组作为参数。

```c#
	abstract void UpdateVisualization (
		NativeArray<float3x4> positions, int resolution, JobHandle handle
	);
```

在 `Update` 中调用此方法，并将形状作业的句柄传递给它。

```c#
			//JobHandle handle = shapeJobs[(int)shape](
				//positions, normals, resolution, transform.localToWorldMatrix, default
			//);
			UpdateVisualization(
				positions, resolution,
				shapeJobs[(int)shape](
					positions, normals, resolution, transform.localToWorldMatrix, default
				)
			);
```

我们的想法是，我们的具体可视化扩展了 `Visualization`，并提供了三种抽象方法的实现。这些类必须能够访问这些方法，但目前这是不可能的，因为它们是 `Visualization` 的私有类。我们可以让它们 `public`，但这不是必需的，因为它们只由类本身使用。我们将使它们 `protected`，这意味着只有类本身和扩展它的所有类可以访问这些方法。

```c#
	protected abstract void EnableVisualization (
		int dataLength, MaterialPropertyBlock propertyBlock
	);

	protected abstract void DisableVisualization ();

	protected abstract void UpdateVisualization (
		NativeArray<float3x4> positions, int resolution, JobHandle handle
	);
```

### 1.3 扩展抽象类

我们现在将调整 `HashVisualization`，使其直接扩展 `Visualization` 而不是 `MonoBehaviour`，从而继承所有通用的可视化功能。

```c#
public class HashVisualization : Visualization { … }
```

删除嵌套的 `Shape` 类型和 `HashVisualization` 现在从 `Visualization` 继承的所有字段，因为它们现在是重复的。

```c#
	//public enum Shape { Plane, Sphere, Torus }

	//static Shapes.ScheduleDelegate[] shapeJobs = { … };

	static int hashesId = Shader.PropertyToID("_Hashes");
		//positionsId = Shader.PropertyToID("_Positions"),
		//normalsId = Shader.PropertyToID("_Normals"),
		//configId = Shader.PropertyToID("_Config");

	//…

	[SerializeField]
	int seed;

	[SerializeField]
	SpaceTRS domain = new SpaceTRS {
		scale = 8f
	};

	NativeArray<uint4> hashes;

	//NativeArray<float3x4> positions, normals;

	//ComputeBuffer hashesBuffer, positionsBuffer, normalsBuffer;
	ComputeBuffer hashesBuffer;

	//MaterialPropertyBlock propertyBlock;

	//bool isDirty;

	//Bounds bounds;
```

更改 `OnEnable`，使其变为 `EnableVisualization`，仅包含处理哈希数据的代码。

```c#
	void EnableVisualization (int dataLength, MaterialPropertyBlock propertyBlock) {
		//…
		hashes = new NativeArray<uint4>(dataLength, Allocator.Persistent);
		//positions = new NativeArray<float3x4>(length, Allocator.Persistent);
		//normals = new NativeArray<float3x4>(length, Allocator.Persistent);
		hashesBuffer = new ComputeBuffer(dataLength * 4,4);
		//positionsBuffer = new ComputeBuffer(length * 4, 3 * 4);
		//normalsBuffer = new ComputeBuffer(length * 4, 3 * 4);

		//propertyBlock ??= new MaterialPropertyBlock();
		propertyBlock.SetBuffer(hashesId, hashesBuffer);
		//…
	}
```

我们必须指出这个方法重写了它的抽象版本，这是通过在它前面写 `override` 来完成的。我们还必须给它相同的 `protected` 访问级别。

```c#
	protected override void EnableVisualization (
		int dataLength, MaterialPropertyBlock propertyBlock
	) { … }
```

使用相同的方法将 `OnDisable` 转换为 `DisableVisualization`。

```c#
	protected override void DisableVisualization () {
		hashes.Dispose();
		//positions.Dispose();
		//normals.Dispose();
		hashesBuffer.Release();
		//positionsBuffer.Release();
		//normalsBuffer.Release();
		hashesBuffer = null;
		//positionsBuffer = null;
		//normalsBuffer = null;
	}
```

删除 `OnValidate` 方法，因为它已经在我们扩展的基类中定义。

```c#
	//void OnValidate () { … }
```

最后更改 `Update`，使其变为 `UpdateVisualization`，只调度和完成哈希作业，然后设置哈希缓冲区。

```c#
	protected override void UpdateVisualization (
		NativeArray<float3x4> positions, int resolution, JobHandle handle
	) {
		//…
		new HashJob {
			positions = positions,
			hashes = hashes,
			hash = SmallXXHash.Seed(seed),
			domainTRS = domain.Matrix
		}.ScheduleParallel(hashes.Length, resolution, handle).Complete();

		hashesBuffer.SetData(hashes.Reinterpret<uint>(4 * 4));
		//…
	}
```

此时，我们的哈希可视化仍然像以前一样工作，但所有通用可视化代码都隔离在单独的 `Visualization` 类中。

### 1.4 噪声可视化

我们将创建一个新的 `NoiseVisualization` 组件类型，它也扩展了 `Visualization`。通过复制 `HashVisualization`，删除其哈希作业，并将所有对哈希的引用替换为对噪声的引用来实现这一点。由于噪声将由浮点值组成，因此将本机数组的元素类型更改为 `float4`。最初，`UpdateVisualization` 只完成提供的句柄并设置噪声缓冲区。这将产生到处都是零的噪音。

```c#
//using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

//using static Unity.Mathematics.math;

public class NoiseVisualization : Visualization {
	
	//[BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
	//struct HashJob : IJobFor { … }
	
	static int noiseId = Shader.PropertyToID("_Noise");
	
	[SerializeField]
	int seed;

	[SerializeField]
	SpaceTRS domain = new SpaceTRS {
		scale = 8f
	};

	NativeArray<float4> noise;

	ComputeBuffer noiseBuffer;
	
	protected override void EnableVisualization (
		int dataLength, MaterialPropertyBlock propertyBlock
	) {
		noise = new NativeArray<float4>(dataLength, Allocator.Persistent);
		noiseBuffer = new ComputeBuffer(dataLength * 4, 4);
		propertyBlock.SetBuffer(noiseId, noiseBuffer);
	}

	protected override void DisableVisualization () {
		noise.Dispose();
		noiseBuffer.Release();
		noiseBuffer = null;
	}

	protected override void UpdateVisualization (
		NativeArray<float3x4> positions, int resolution, JobHandle handle
	) {
		//new HashJob {
			//…
		//}.ScheduleParallel(hashes.Length, resolution, handle).Complete();
		
		handle.Complete();
		noiseBuffer.SetData(noise.Reinterpret<float>(4 * 4));
	}
}
```

噪波可视化需要一个稍微不同的着色器。复制 *HashGPU* 并将其重命名为 *NoiseGPU*。将哈希缓冲区替换为噪声缓冲区，并直接使用噪声值来偏移 `ConfigureProcedural` 中的位置。这是有效的，因为我们的噪声值将位于 -1~1 范围内。

```glsl
#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
	StructuredBuffer<float> _Noise;
	StructuredBuffer<float3> _Positions, _Normals;
#endif

float4 _Config;

void ConfigureProcedural () {
	#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
		…
		unity_ObjectToWorld._m03_m13_m23 +=
			_Config.z * _Noise[unity_InstanceID] * _Normals[unity_InstanceID];
		unity_ObjectToWorld._m00_m11_m22 = _Config.y;
	#endif
}
```

然后用 `GetNoiseColor` 函数替换 `GetHashColor`，如果噪波值为正，则直接返回噪波值，从而产生灰度值。如果噪声是负的，那么让我们把它变成红色，这样很容易看出正噪声和负噪声之间的区别。

```glsl
float3 GetNoiseColor () {
	#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
		float noise = _Noise[unity_InstanceID];
		return noise < 0.0 ? float3(-noise, 0.0, 0.0) : noise;
	#else
		return 1.0;
	#endif
}

void ShaderGraphFunction_float (float3 In, out float3 Out, out float3 Color) {
	Out = In;
	Color = GetNoiseColor();
}

void ShaderGraphFunction_half (half3 In, out half3 Out, out half3 Color) {
	Out = In;
	Color = GetNoiseColor();
}
```

通过复制哈希版本并更改其使用的 HLSL 文件，为噪波创建着色器图或曲面着色器。如果使用曲面着色器，则还必须更改调用的颜色函数。然后用它创建一个材质，然后用使用它的 `NoiseVisualization` 组件创建一个游戏对象。

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/value-noise/reusable-visualization/noise-visualization-inspector.png)

*噪声可视化游戏对象。*

我将哈希和噪波可视化放在单独的场景中，但你也可以在同一个场景中同时使用这两种可视化，只启用其中一种。

### 1.5 扩展方法

一旦我们创建了一个计算噪声的作业，我们将有三个地方需要执行矢量化矩阵向量变换。与其引入另一个 `TransformPositions` 或 `TransformVectors` 实例，不如将此代码放在一个可以在任何地方使用的地方。最简单的方法是在自己的 C# 文件中创建一个静态 `MathExtensions` 类，该类包含 `Shapes.Job` 中 `TransformVectors` 方法的公共静态副本。

```c#
using Unity.Mathematics;

using static Unity.Mathematics.math;

public static class MathExtensions {

	public static float4x3 TransformVectors (
		float3x4 trs, float4x3 p, float w = 1f
	) => float4x3(
		trs.c0.x * p.c0 + trs.c1.x * p.c1 + trs.c2.x * p.c2 + trs.c3.x * w,
		trs.c0.y * p.c0 + trs.c1.y * p.c1 + trs.c2.y * p.c2 + trs.c3.y * w,
		trs.c0.z * p.c0 + trs.c1.z * p.c1 + trs.c2.z * p.c2 + trs.c3.z * w
	);
}
```

现在我们可以通过 `MathExtensions.TransformVectors(trs, v)` 在任何地方调用它。借助 `using static MathExtensions`，这可以简化为 `TransformVectors(trs, v)`。然而，另一种方法是将其转换为扩展方法。

扩展方法是一种静态方法，它假装是某个类型的实例方法。它是通过将 `this` 修饰符添加到方法的第一个参数中而创建的。

```c#
	public static float4x3 TransformVectors (
		this float3x4 trs, float4x3 p, float w = 1f
	) => float4x3(…);
```

然后可以在该类型的实例上调用该方法，省略其第一个参数，因此我们最终得到 `trs.TransformVectors(v)`，有效地要求矩阵变换向量。更改 `Shapes.Job` 使用这种方法，消除了自己版本的方法。

```c#
		//float4x3 TransformVectors (float3x4 trs, float4x3 p, float w = 1f) => float4x3(…);

		public void Execute (int i) {
			Point4 p = default(S).GetPoint4(i, resolution, invResolution);

			positions[i] = transpose(positionTRS.TransformVectors(p.positions));

			float3x4 n = transpose(normalTRS.TransformVectors(p.normals, 0f));
			normals[i] = float3x4(
				normalize(n.c0), normalize(n.c1), normalize(n.c2), normalize(n.c3)
			);
		}
```

对 `HashVisualization.HashJob` 做同样的事情。

```c#
		//float4x3 TransformPositions (float3x4 trs, float4x3 p) => float4x3(…);

		public void Execute (int i) {
			float4x3 p = domainTRS.TransformVectors(transpose(positions[i]));

			…
		}
```

让我们在 `MathExtensions` 中添加另一个扩展方法，这次是一个 `Get3x4` 方法，用于提取 `float4x4` 矩阵的 `float3x4` 部分。

```c#
	public static float3x4 Get3x4 (this float4x4 m) =>
		float3x4(m.c0.xyz, m.c1.xyz, m.c2.xyz, m.c3.xyz);
```

使用它来简化 `Shapes.Job.ScheduleParallel`。

```c#
		public static JobHandle ScheduleParallel (
			NativeArray<float3x4> positions, NativeArray<float3x4> normals,
			int resolution,	float4x4 trs, JobHandle dependency
		//) {
		//	float4x4 tim = transpose(inverse(trs));
		) => new Job<S> {
			positions = positions,
			normals = normals,
			resolution = resolution,
			invResolution = 1f / resolution,
			positionTRS = trs.Get3x4(),
			normalTRS = transpose(inverse(trs)).Get3x4()
		}.ScheduleParallel(positions.Length, resolution, dependency);
		//}
```

> **扩展方法是如何工作的？**
>
> 如果你足够深入，就没有物体这样的东西。只有数据，其中一些代表信息，一些代表指令。对象是一种抽象。当在对象上调用方法时，真正发生的是 CPU 在数据堆栈上推送一些数据（参数），然后跳到相关指令。调用该方法的对象只是另一个参数。扩展方法使这一点变得明确。

## 2 晶格噪声

我们将在本教程中创建的噪波类型称为值噪波。它是一种特定类型的晶格噪声，是基于几何晶格（通常是规则网格）的噪声。

### 2.1 通用噪声作业

因为有不同类型和风格的噪波，我们将创建一个专用的静态 `Noise` 类，就像我们为形状创建的一个一样。就像 `Shapes` 一样，它包含一个接口和一个通用的 `Job` 结构类型。在这种情况下，接口是 `INoise`，定义了一个 `GetNoise4` 方法，该方法在给定一组位置和哈希值的情况下，返回一个带有噪声的矢量化 `float4` 值。

```c#
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

using static Unity.Mathematics.math;

public static class Noise {

	public interface INoise {
		float4 GetNoise4 (float4x3 positions, SmallXXHash4 hash);
	}

	[BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
	public struct Job<N> : IJobFor where N : struct, INoise {}
}
```

该作业有输入位置和输出噪声，还需要一个哈希和域转换矩阵。它的 `Execute` 方法调用噪声的 `GetNoise4` 方法，将转换后的位置和哈希传递给它。

```c#
	public struct Job : IJobFor where N : struct, INoise {
		
		[ReadOnly]
		public NativeArray<float3x4> positions;

		[WriteOnly]
		public NativeArray<float4> noise;

		public SmallXXHash4 hash;

		public float3x4 domainTRS;

		public void Execute (int i) {
			noise[i] = default(N).GetNoise4(
				domainTRS.TransformVectors(transpose(positions[i])), hash
			);
		}
	}
```

最后，向作业添加适当的 `ScheduleParallel` 方法，并将相应的委托类型添加到 `Noise`。

```c#
	public struct Job<N> : IJobFor where N : struct, INoise {
		
		…
		
		public static JobHandle ScheduleParallel (
			NativeArray<float3x4> positions, NativeArray<float4> noise,
			int seed, SpaceTRS domainTRS, int resolution, JobHandle dependency
		) => new Job<N> {
			positions = positions,
			noise = noise,
			hash = SmallXXHash.Seed(seed),
			domainTRS = domainTRS.Matrix,
		}.ScheduleParallel(positions.Length, resolution, dependency);
	}
	
	public delegate JobHandle ScheduleDelegate (
		NativeArray<float3x4> positions, NativeArray<float4> noise,
		int seed, SpaceTRS domainTRS, int resolution, JobHandle dependency
	);
```

### 2.2 分部类

下一步是将晶格噪声的代码添加到噪声中，但与其将其全部放在同一个 C# 文件中，不如将晶格特定的代码放在一个单独的文件中，以保持有序。这可以通过将 `Noise` 转换为 `partial` 类来实现。这告诉编译器可以有多个文件包含 `Noise` 的一部分。

```c#
public static partial class Noise { … }
```

现在创建一个新的 C# 资源文件，并将其命名为 `Noise.Lattice`。此命名约定不是强制性的，但明确指出该文件包含 `Noise` 的晶格部分。在它里面，我们再次定义了 partial `Noise` 类，这次引入了一个 `Lattice1D` 结构类型，通过最初始终返回零来实现 `INoise`。为了保持简单，我们从一维晶格噪声开始，因此我们将其命名为 `Lattice1D`。

```c#
using Unity.Mathematics;

using static Unity.Mathematics.math;

public static partial class Noise {

	public struct Lattice1D : INoise {

		public float4 GetNoise4(float4x3 positions, SmallXXHash4 hash) {
			return 0f;
		}
	}
}
```

现在可以在 `NoiseVisualization` 中安排一个作业来创建 1D 晶格噪声。

```c#
…

using static Noise;

public class NoiseVisualization : Visualization {

	…

	protected override void UpdateVisualization (
		NativeArray<float3x4> positions, int resolution, JobHandle handle
	) {
		Job<Lattice1D>.ScheduleParallel(
			positions, noise, seed, domain, resolution, handle
		).Complete();
		noiseBuffer.SetData(noise.Reinterpret<float>(4 * 4));
	}
}
```

### 2.3 1D 噪声

在 `Lattice1D.GetNoise4` 中创建 1D 噪声的第一步，是对整数 X 坐标进行哈希运算，将它们的第一个字节作为浮点数检索，然后将其转换为 -1~1 范围。为此，首先计算坐标，将其输入哈希，将其转换为无符号整数，屏蔽第一个字节，将其转化为浮点值，然后调整范围。

```c#
		public float4 GetNoise4(float4x3 positions, SmallXXHash4 hash) {
			int4 p = (int4)floor(positions.c0);
			float4 v = (uint4)hash.Eat(p) & 255;
			return v * (2f / 255f) - 1f;
		}
```

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/value-noise/lattice-noise/1d-hash-p0.png)

*格点的 1D 哈希值；域尺度 8。*

这为我们提供了整数坐标下恒定的孤立哈希值，忽略了坐标的小数部分。为了使噪声平滑连续，我们必须在整数坐标之间混合这些值。整数坐标定义了晶格结构的点。在这些点之间是空白空间的跨度，我们必须用连续的噪声信号填充。为了实现这一点，我们需要知道跨度两侧的两个点。

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/value-noise/lattice-noise/1d-lattice.png)

*一维格线上的单个跨度。*

我们将当前的点指定为 p0。另一点更进一步，称为点 p1。如果我们将 p1 而不是 p0 可视化，那么我们将得到与以前相同的模式，但偏移了一个晶格步长。

```c#
			int4 p0 = (int4)floor(positions.c0);
			int4 p1 = p0 + 1;
			float4 v = (uint4)hash.Eat(p1) & 255;
```

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/value-noise/lattice-noise/1d-hash-p1.png)

*下一个晶格点的值。*

为了填充格点之间的跨度，我们需要组合这两个值，这意味着我们必须将哈希值转换为浮点值两次。为了简化需要字节或浮点值的代码，让我们在 `SmallXXHash4` 中添加两个属性，一个用于检索第一个矢量化字节（指定为字节 A），另一个用于获取相同的数据，但转换为 0~1 范围内的值。

```c#
	public uint4 BytesA => (uint4)this & 255;
	
	public float4 Floats01A => (float4)BytesA * (1f / 255f);
```

现在我们可以很容易地检索 p0 和 p1 的 0-1 值，将它们相加，然后在 `GetNoise4` 中减去 1。这给了我们 −1-1 范围内两个格点值的平均值。

```c#
			//float4 v = (uint4)hash.Eat(p0) & 255;
			return hash.Eat(p0).Floats01A + hash.Eat(p1).Floats01A - 1f;
```

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/value-noise/lattice-noise/1d-hash-average.png)

*两个点的平均值。*

同样可以通过 p0 和 p1 的线性插值来实现，使用以 0.5 为第三个参数的 `lerp` 函数。在减去 1 之前，其结果必须加倍。

```c#
			return lerp(hash.Eat(p0).Floats01A, hash.Eat(p1).Floats01A, 0.5f) * 2f - 1f;
```

最后，为了创建一个连续的过渡，根据晶格坐标的分数部分从 p0 插值到 p1。通过从坐标中减去 p0 可以找到它。这给了我们插值值，我们称之为 `t`。

```c#
			float4 t = positions.c0 - p0;
			return lerp(hash.Eat(p0).Floats01A, hash.Eat(p1).Floats01A, t) * 2f - 1f;
```

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/value-noise/lattice-noise/1d-hash-interpolated.png)

*晶格点之间的线性插值。*

让我们还将其他三个字节及其 0-1 版本的属性添加到 `SmallXXHash4` 中，这在将来会很方便。

```c#
	public uint4 BytesA => (uint4)this & 255;

	public uint4 BytesB => ((uint4)this >> 8) & 255;

	public uint4 BytesC => ((uint4)this >> 16) & 255;

	public uint4 BytesD => (uint4)this >> 24;

	public float4 Floats01A => (float4)BytesA * (1f / 255f);

	public float4 Floats01B => (float4)BytesB * (1f / 255f);

	public float4 Floats01C => (float4)BytesC * (1f / 255f);

	public float4 Floats01D => (float4)BytesD * (1f / 255f);
```

### 2.4 2D 噪声

此时，我们有连续的 1D 噪声，尽管它还不平滑。在担心平滑度之前，让我们先在噪声处于最简单形式时制作一个 2D 变体。

当只考虑一个维度时，我们需要跟踪两个格点和一个插值器值。让我们为此数据定义一个 `LatticeSpan4` 结构类型。由于它仅用于内部晶格噪波计算，因此在 *Noise.Lattice* 文件的 `Noise` 中保持其私有。

```c#
	struct LatticeSpan4 {
		public int4 p0, p1;
		public float4 t;
	}
```

接下来，添加一个静态 `GetLatticeSpan4` 方法，该方法为我们提供给定 1D 坐标集的数据。

```c#
	static LatticeSpan4 GetLatticeSpan4 (float4 coordinates) {
		float4 points = floor(coordinates);
		LatticeSpan4 span;
		span.p0 = (int4)points;
		span.p1 = span.p0 + 1;
		span.t = coordinates - points;
		return span;
	}
```

这使我们能够简化 `Lattice1D.GetNoise4`。

```c#
		public float4 GetNoise4(float4x3 positions, SmallXXHash4 hash) {
			LatticeSpan4 x = GetLatticeSpan4(positions.c0);
			return lerp(
				hash.Eat(x.p0).Floats01A, hash.Eat(x.p1).Floats01A, x.t
			) * 2f - 1f;
		}
```

介绍 `Lattice2D`，最初是 `Lattice1D` 的复制品。

```c#
	public struct Lattice1D : INoise { … }
	
	public struct Lattice2D : INoise { … }
```

调整 `NoiseVisualization.UpdateVisualization` ，使其使用 2D 版本。

```c#
		Job<Lattice2D>.ScheduleParallel(
			positions, noise, seed, domain, resolution, handle
		).Complete();
```

现在调整 `Lattice2D.GetNoise4`，因此它也会获取 Z 维度的晶格跨度数据，然后使用该数据而不是 X 来计算噪声。

```c#
		public float4 GetNoise4 (float4x3 positions, SmallXXHash4 hash) {
			LatticeSpan4
				x = GetLatticeSpan4(positions.c0), z = GetLatticeSpan4(positions.c2);
			
			return lerp(hash.Eat(z.p0).Floats01A, hash.Eat(z.p1).Floats01A, z.t) * 2f - 1f;
		}
```

这将产生与以前相同的图案，但现在是在 Z 维度而不是 X 维度。我们也可以使用 Y 维度，但除非我们旋转域，否则不会为 XZ 平面产生可见的图案。

![x](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/value-noise/lattice-noise/2d-only-x.png) ![z](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/value-noise/lattice-noise/2d-only-z.png)

*仅插值 X 和 Z。*

为了创建一个同时依赖于 X 和 Z 的模式，我们必须同时考虑这两个维度，最终得到格子正方形四个角的哈希值。

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/value-noise/lattice-noise/2d-lattice.png)

*二维格子正方形。*

让我们首先计算 X 点的哈希值，将它们称为 h0 和 h1。

```c#
			LatticeSpan4
				x = GetLatticeSpan4(positions.c0), z = GetLatticeSpan4(positions.c2);

			SmallXXHash4 h0 = hash.Eat(x.p0), h1 = hash.Eat(x.p1);
```

然后将 Z 点输入到 h0，而不是原始哈希值。

```c#
			return lerp(h0.Eat(z.p0).Floats01A, h0.Eat(z.p1).Floats01A, z.t) * 2f - 1f;
```

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/value-noise/lattice-noise/2d-bands.png)

*沿着 X 的独立条带。*

由于我们现在将噪声基于单个 X 点和两个 Z 点的插值，我们得到了沿 Z 维度的连续带，以及沿 X 维度的不连续模式。为了完成该模式，我们还必须沿 X 在 h0 和 h1 带之间进行插值。因此，我们必须对两个线性插值进行线性插值，这被称为双线性插值。

```c#
			return lerp(
				lerp(h0.Eat(z.p0).Floats01A, h0.Eat(z.p1).Floats01A, z.t),
				lerp(h1.Eat(z.p0).Floats01A, h1.Eat(z.p1).Floats01A, z.t),
				x.t
			) * 2f - 1f;
```

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/value-noise/lattice-noise/2d-interpolated.png)

*双线性插值。*

### 2.5 平滑噪音

虽然我们有一个连续的二维图案，但它还不平滑。晶格点之间跨度的过渡是直的平坦段，因此沿着晶格正方形的边缘方向会突然改变。为了使这一过程平稳，我们需要考虑噪声的变化率。如果我们有一个函数，那么它的一阶导数函数描述了它的变化率。当线性插值产生一条直线时，其导数是一个常数值。还有一个二阶导数函数，它是一阶导数的导数。你可以把它看作是曲率的变化率，或者噪声的加速度。在这种情况下，二阶导数始终为零。

例如，在下图中，1D 噪声显示为实心黑线，其一阶导数为橙色虚线，其二阶导数为紫色虚线。导数被除以 6，以缩小规模，使其更容易看到。请注意，在中间的晶格点处，橙色线中存在不连续性，此时噪波会突然改变方向。

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/value-noise/lattice-noise/linear-graph.png)

*点值 0、1 和 0.5 之间的两个 1d 跨度。*

我们可以通过将 smoothstep 函数应用于 `GetLatticeSpan4` 中的插值器来平滑它。

```c#
		span.t = coordinates - points;
		span.t = smoothstep(0f, 1f, span.t);
```

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/value-noise/lattice-noise/2d-smoothstep.png)

*平滑插值。*

这之所以有效，是因为 smoothstep 函数——$3t^2-2t^3$——对于输入 0 和 1 来说是水平的。这意味着该函数的一阶导数——$6t-6t^2$——两端均为零。众所周知，它是 C1 连续的，而我们的线性插值只有 C0 连续。然而，对于其二阶导数，情况并非如此——$6-12t$——所以它不是 C2 连续的。

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/value-noise/lattice-noise/smoothstep-graph.png)

*平滑步长而不是线性插值。*

> **如何找到函数的导数？**
>
> 我们需要知道的唯一规则是 $ax^b$ 的导数是 $abx^{b-1}$ 其中 $a$ 和 $b$ 是常数。常数的导数为零。这适用于该函数的所有单独部分。例如，$4x^3+5x-2$ 的导数是 $12x^2+5$

### 2.6 二阶连续性

虽然 smoothstep 是 C1 连续的，但该函数的导数不是连续的。这意味着晶格边缘两侧的变化率可能不同。在使用基于点的可视化时，这不是很明显，但当使用噪波定义平滑网格曲面或法线贴图时，这种不连续性可能会表现为可见的折痕，露出晶格。为了避免这种情况，我们必须更进一步，使用 C2 连续函数，为此我们可以使用 $6t^5-15^4+10t^3$ 其一阶导数为 $30t^4-60t^3+30t^2$ 其二阶导数为 $120t^3-180t^2+60t$。对于输入 0 和 1，这两个导数都为零。

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/value-noise/lattice-noise/c2-continuous-graph.png)

*C2 连续而不是平滑步进。*

调整 `GetLatticeSpan4` 以使用此功能。可以重写为 $ttt(t(t6-15)+10)$

```c#
		span.t = coordinates - points;
		span.t = span.t * span.t * span.t * (span.t * (span.t * 6f - 15f) + 10f);
```

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/value-noise/lattice-noise/2d-c2-continuous.png)

*C2 连续插值。*

### 2.7 3D 噪声

最后，我们添加了 3D 版本的值噪波。

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/value-noise/lattice-noise/3d-lattice.png)

*三维格子立方体。*

通过复制 `Lattice2D` 创建 `Lattice3D`，并让它检索 Y 坐标的晶格跨度数据。然后，还要跟踪 XY 晶格正方形四个点的哈希值。

```c#
	public struct Lattice3D : INoise {

		public float4 GetNoise4 (float4x3 positions, SmallXXHash4 hash) {
			LatticeSpan4
				x = GetLatticeSpan4(positions.c0),
				y = GetLatticeSpan4(positions.c1),
				z = GetLatticeSpan4(positions.c2);

			SmallXXHash4
				h0 = hash.Eat(x.p0), h1 = hash.Eat(x.p1),
				h00 = h0.Eat(y.p0), h01 = h0.Eat(y.p1),
				h10 = h1.Eat(y.p0), h11 = h1.Eat(y.p1);

			return lerp(…) * 2f - 1f;
		}
	}
```

调整结果，使其沿 X 在两个 YZ 晶格方格之间插值。

```c#
			return lerp(
				lerp(
					lerp(h00.Eat(z.p0).Floats01A, h00.Eat(z.p1).Floats01A, z.t),
					lerp(h01.Eat(z.p0).Floats01A, h01.Eat(z.p1).Floats01A, z.t),
					y.t
				),
				lerp(
					lerp(h10.Eat(z.p0).Floats01A, h10.Eat(z.p1).Floats01A, z.t),
					lerp(h11.Eat(z.p0).Floats01A, h11.Eat(z.p1).Floats01A, z.t),
					y.t
				),
				x.t
			) * 2f - 1f;
```

最后，在 `NoiseVisualization` 中添加一个噪声模式维度的配置滑块，并使用它通过静态数组选择正确的版本。

```c#
	static ScheduleDelegate[] noiseJobs = {
		Job<Lattice1D>.ScheduleParallel,
		Job<Lattice2D>.ScheduleParallel,
		Job<Lattice3D>.ScheduleParallel
	};

	…

	[SerializeField, Range(1, 3)]
	int dimensions = 3;

	…

	protected override void UpdateVisualization (
		NativeArray<float3x4> positions, int resolution, JobHandle handle
	) {
		noiseJobs[dimensions - 1](
			positions, noise, seed, domain, resolution, handle
		).Complete();
		noiseBuffer.SetData(noise.Reinterpret<float>(4 * 4));
	}
```

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/value-noise/lattice-noise/dimensions-slider.png)

*噪声尺寸滑块。*

我们现在可以很快看到不同版本的噪音之间的差异。当使用球体等 3D 形状时，这一点最为明显。

![3D](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/value-noise/lattice-noise/sphere-3d.png) ![2D](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/value-noise/lattice-noise/sphere-2d.png) ![1D](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/value-noise/lattice-noise/sphere-1d.png)

*具有 3D、2D 和 1D 噪声的球体；域尺度 16。*

下一个教程是 [Perlin 噪波](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/perlin-noise/)。

[license](https://catlikecoding.com/unity/tutorials/license/)

[repository](https://bitbucket.org/catlikecodingunitytutorials/pseudorandom-noise-03-value-noise/)

[PDF](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/value-noise/Value-Noise.pdf)



# 柏林噪声：梯度噪声

发布于 2021-06-18

https://catlikecoding.com/unity/tutorials/pseudorandom-noise/perlin-noise/

*使晶格噪波通用。*
*添加对渐变噪波的支持。*
*生成 1D、2D 和 3D Perlin 噪声。*

这是关于[伪随机噪声](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/)的系列教程中的第四篇。它增强了我们的晶格噪声工作，也支持 Perlin 噪声。

本教程使用 Unity 2020.3.6f1 编写。

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/perlin-noise/tutorial-image.jpg)

*一个显示 3D Perlin 噪声的球体。*

## 1 通用梯度噪声

值噪波是在格点处定义常数值的格噪波。这些值的插值会产生平滑的图案，但晶格仍然非常明显。另一种方法是在函数之间插值，而不是在常数值之间插值。这意味着每个格点都有自己的函数。为了保持简单和统一，所有点都应该得到相同的函数，只是参数化不同。最简单的函数是一个常数值，即值噪声。在此基础上，一个函数将线性依赖于相对于晶格点的样本坐标。最直接的是 $f(x)=x$ 其中 $x$ 是相对坐标。这将产生以晶格点为中心的线性斜坡或渐变。因此，这种类型的噪声被称为梯度噪声。

由于值噪声可以被视为梯度噪声的一种微不足道的情况，让我们调整我们的晶格噪声，使其始终作为梯度噪声运行。

### 1.1 渐变接口

我们首先为 `Noise` 引入一个新的部分类资产，这次命名为 *Noise.Gradient*。在其中，我们声明了 `IGradient` 接口。

```c#
using Unity.Mathematics;

using static Unity.Mathematics.math;

public static partial class Noise {

	public interface IGradient {}
}
```

梯度的目的是用哈希和相对坐标作为参数来计算函数。为此定义一个矢量化的 `Evaluate` 方法签名，最初用于 1D 噪声，这意味着除了哈希之外，它还需要有 X 坐标的参数。

```c#
	public interface IGradient {
		float4 Evaluate (SmallXXHash4 hash, float4 x);
	}
```

现在，我们可以在实现 1D 值噪声 `IGradient` 的同一个分部类中添加一个 `Value` 结构类型。它只是忽略坐标并返回哈希的 A 浮点数，就像我们目前的晶格噪声一样。

```c#
	public struct Value : IGradient {

		public float4 Evaluate (SmallXXHash4 hash, float4 x) => hash.Floats01A;
	}
```

为了支持 2D 和 3D 噪波，请在 `IGradient` 中添加具有两个和三个坐标参数的 `Evaluate` 变体。

```c#
	public interface IGradient {
		float4 Evaluate (SmallXXHash4 hash, float4 x);

		float4 Evaluate (SmallXXHash4 hash, float4 x, float4 y);

		float4 Evaluate (SmallXXHash4 hash, float4 x, float4 y, float4 z);
	}
```

`Value` 也必须实现这些方法，只需返回哈希值并忽略坐标即可。未使用的参数不会影响性能，因为 *Burst* 消除了所有方法调用开销和未使用的值。

```c#
	public struct Value : IGradient {

		public float4 Evaluate (SmallXXHash4 hash, float4 x) => hash.Floats01A;

		public float4 Evaluate (SmallXXHash4 hash, float4 x, float4 y) => hash.Floats01A;

		public float4 Evaluate (SmallXXHash4 hash, float4 x, float4 y, float4 z) =>
			hash.Floats01A;
	}
```

### 1.2 梯度输入

为了提供梯度的相对坐标，我们将在 *Noise.Lattice* 中的 `LatticeSpan4` 中添加一个名为 `g` 的矢量化浮点字段。

```c#
	struct LatticeSpan4 {
		public int4 p0, p1;
		public float4 g;
		public float4 t;
	}
```

我们通过从 `GetLatticeSpan4` 中的坐标减去 `p0` 来找到相对坐标。

```c#
		span.p0 = (int4)points;
		span.p1 = span.p0 + 1;
		span.g = coordinates - span.p0;
```

现在我们可以调整 `Lattice1D.GetNoise4`，因此它调用 `Value` 上的 `Evaluate`，而不是直接从哈希中获取值。将相关哈希值和相对坐标作为参数传递给 `Evaluate`。

```c#
	public struct Lattice1D : INoise {

		public float4 GetNoise4(float4x3 positions, SmallXXHash4 hash) {
			LatticeSpan4 x = GetLatticeSpan4(positions.c0);

			var g = default(Value);
			return lerp(
				g.Evaluate(hash.Eat(x.p0), x.g), g.Evaluate(hash.Eat(x.p1), x.g), x.t
			) * 2f - 1f;
		}
	}
```

此时，对于 1D 值噪声，我们仍然得到与以前相同的结果，并且为作业生成的汇编代码也完全相同。但是，为了了解梯度噪声是如何工作的，我们可以暂时更改 `Value.Evaluate`，使其成为简单的梯度函数 $f(x)=x$

```c#
		public float4 Evaluate (SmallXXHash4 hash, float4 x) => x;
```

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/perlin-noise/generic-gradient-noise/raw-gradient.png)

*原始坡度；平面上的1D噪声；分辨率 128。*

由此产生的模式是一系列线性 1D 斜坡，在晶格点之间从 -1 到 1。没有明显的混合，因为我们在每个跨度的两侧使用了完全相同的梯度，相对于两端的 `p0`。为了将其转化为适当的梯度噪声，第二梯度必须基于相对于 `p1` 的坐标。因此，将 `LatticeSpan4` 中的单个 `g` 字段替换为 `g0`，并添加一个新的 `g1` 字段。

```c#
	struct LatticeSpan4 {
		public int4 p0, p1;
		public float4 g0, g1;
		public float4 t;
	}
```

因为跨度的第二个格点沿着该维度再靠一个单位，所以我们通过从 `g0` 中减去 1 来在 `GetLatticeSpan4` 中找到 `g1`。

```c#
		span.p0 = (int4)points;
		span.p1 = span.p0 + 1;
		span.g0 = coordinates - span.p0;
		span.g1 = span.g0 - 1f;
```

调整 `Lattice1D.GetNoise4`，因此它使用正确的相对坐标：`p0` 和 `g0`，`p1` 和 `g1`。

```c#
			return lerp(
				g.Evaluate(hash.Eat(x.p0), x.g0), g.Evaluate(hash.Eat(x.p1), x.g1), x.t
			) * 2f - 1f;
```

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/perlin-noise/generic-gradient-noise/interpolated-gradients.png)

*插值梯度。*

如果我们使用线性插值，得到的图案将是平坦的，因为梯度会相互抵消。但由于我们使用平滑的 C2 连续插值，每个梯度在其格点附近都占主导地位，我们得到了一个波形。

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/perlin-noise/generic-gradient-noise/gradients-graph.png)

*在两个跨度内插值三个梯度。*

请注意，梯度在其晶格点处为零，并且插值图案在跨度的中间也为零，其中梯度相互抵消。

我们得到的模式是完全负的，因为我们之后应用了范围调整。此调整特定于 Value 噪波，因此让我们恢复噪波并将调整移动到 `Value` 方法。

```c#
	public struct Value : IGradient {

		public float4 Evaluate (SmallXXHash4 hash, float4 x) => hash.Floats01A * 2f - 1f;

		public float4 Evaluate (SmallXXHash4 hash, float4 x, float4 y) =>
			hash.Floats01A * 2f - 1f;

		public float4 Evaluate (SmallXXHash4 hash, float4 x, float4 y, float4 z) =>
			hash.Floats01A * 2f - 1f;
	}
```

然后将其从 `Lattice1D.GetNoise4` 中删除。

```c#
			return lerp(
				g.Evaluate(hash.Eat(x.p0), x.g0), g.Evaluate(hash.Eat(x.p1), x.g1), x.t
			);
			//) * 2f - 1f;
```

> **梯度插值仍然是 C2 连续的吗？**
>
> 因为梯度在晶格点处有斜率，所以那里的一阶导数不再为零，但它在跨度上仍然是连续的，因为两侧的梯度是相同的。二阶导数在格点处确实仍然为零，因此也是连续的。
>
> ![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/perlin-noise/generic-gradient-noise/gradient-derivatives-graph.png)
>
> *用一阶和二阶导数插值梯度，除以 6 进行拟合。*

### 1.3 通用晶格

现在我们有了 Value noise 的梯度版本，我们可以通过给它一个 `IGradient` 结构类型参数来使 `Lattice1D` 通用，就像我们对 `Noise.Job` 所做的那样。

```c#
	public struct Lattice1D<G> : INoise where G : struct, IGradient {

		public float4 GetNoise4(float4x3 positions, SmallXXHash4 hash) {
			LatticeSpan4 x = GetLatticeSpan4(positions.c0);

			var g = default(G);
			return lerp(
				g.Evaluate(hash.Eat(x.p0), x.g0), g.Evaluate(hash.Eat(x.p1), x.g1), x.t
			);
		}
	}
```

以相同的方式调整 `Lattice2D`，这次使用 `IGradient.Evaluate` 的 2D 变体。确保为所有晶格点使用正确的相对坐标。

```c#
	public struct Lattice2D<G> : INoise where G: struct, IGradient {

		public float4 GetNoise4 (float4x3 positions, SmallXXHash4 hash) {
			…

			var g = default(G);
			return lerp(
				lerp(
					g.Evaluate(h0.Eat(z.p0), x.g0, z.g0),
					g.Evaluate(h0.Eat(z.p1), x.g0, z.g1),
					z.t
				),
				lerp(
					g.Evaluate(h1.Eat(z.p0), x.g1, z.g0),
					g.Evaluate(h1.Eat(z.p1), x.g1, z.g1),
					z.t
				),
				x.t
			);
			//) * 2f - 1f;
		}
	}
```

同时更新 `Lattice3D`。

```c#
	public struct Lattice3D<G> : INoise where G : struct, IGradient {

		public float4 GetNoise4 (float4x3 positions, SmallXXHash4 hash) {
			…

			var g = default(G);
			return lerp(
				lerp(
					lerp(
						g.Evaluate(h00.Eat(z.p0), x.g0, y.g0, z.g0),
						g.Evaluate(h00.Eat(z.p1), x.g0, y.g0, z.g1),
						z.t
					),
					lerp(
						g.Evaluate(h01.Eat(z.p0), x.g0, y.g1, z.g0),
						g.Evaluate(h01.Eat(z.p1), x.g0, y.g1, z.g1),
						z.t
					),
					y.t
				),
				lerp(
					lerp(
						g.Evaluate(h10.Eat(z.p0), x.g1, y.g0, z.g0),
						g.Evaluate(h10.Eat(z.p1), x.g1, y.g0, z.g1),
						z.t
					),
					lerp(
						g.Evaluate(h11.Eat(z.p0), x.g1, y.g1, z.g0),
						g.Evaluate(h11.Eat(z.p1), x.g1, y.g1, z.g1),
						z.t
					),
					y.t
				),
				x.t
			);
			//) * 2f - 1f;
		}
	}
```

我们现在必须明确声明，我们正在 `NoiseVisualization` 中使用晶格噪波作业的 Value 噪波版本。

```c#
	static ScheduleDelegate[] noiseJobs = {
		Job<Lattice1D<Value>>.ScheduleParallel,
		Job<Lattice2D<Value>>.ScheduleParallel,
		Job<Lattice3D<Value>>.ScheduleParallel
	};
```

一切都和以前一样，但现在只需要一点额外的代码就可以添加其他梯度噪声类型。

## 2 Perlin 噪声

Ken Perlin 提出了梯度噪声的第一个版本，因此这个经典版本的噪声被称为 Perlin 噪声。与我们目前已知的梯度噪声相比，Perlin 噪声增加了梯度向量可以具有不同方向的想法。这些不同梯度的插值产生的图案比值噪声更多样、更少块状。

> **Perlin 噪声不是基于置换表吗？**
>
> 是的，但置换表只是为格点生成伪随机值的一种方法。它适用于简单的硬件，但域很小，无法播种。它也不会矢量化，因为它需要多个数组查找，而这些查找没有 SIMD 指令。因此，我们将其基于 `SmallXXHash4`。

### 2.1 第二梯度噪声类型

要实现 Perlin 噪波，请向 *Noise.IGradient* 添加实现 `IGradient` 的 `Perlin` 结构类型，就像 `Value` 一样。初始时将所有评估设置为零。

```c#
	public struct Perlin : IGradient {

		public float4 Evaluate (SmallXXHash4 hash, float4 x) => 0f;

		public float4 Evaluate (SmallXXHash4 hash, float4 x, float4 y) => 0f;

		public float4 Evaluate (SmallXXHash4 hash, float4 x, float4 y, float4 z) => 0f;
	}
```

然后将 `NoiseVisualization` 中的噪波作业数组转换为二维数组。此类数组的元素有两个索引组件，因此数组类型从 `ScheduleDelegate[]` 更改为 `ScheduleDelegate[,]`。然后将现有的初始化包裹在第二组花括号中，并在 Value 噪波集之前为 Perlin 噪波作业插入一个新的集。

```c#
	static ScheduleDelegate[,] noiseJobs = {
		{
			Job<Lattice1D<Perlin>>.ScheduleParallel,
			Job<Lattice2D<Perlin>>.ScheduleParallel,
			Job<Lattice3D<Perlin>>.ScheduleParallel
		},
		{
			Job<Lattice1D<Value>>.ScheduleParallel,
			Job<Lattice2D<Value>>.ScheduleParallel,
			Job<Lattice3D<Value>>.ScheduleParallel
		}
	};
```

为了在噪声类型之间进行切换，请为 Perlin 和 Value 噪声添加一个枚举配置字段，并将其用作 `UpdateVisualization` 中的第一个数组索引参数。

```c#
	public enum NoiseType { Perlin, Value }

	[SerializeField]
	NoiseType type;
	
	…
	
	protected override void UpdateVisualization (
		NativeArray<float3x4> positions, int resolution, JobHandle handle
	) {
		noiseJobs[(int)type, dimensions - 1](
			positions, noise, seed, domain, resolution, handle
		).Complete();
		noiseBuffer.SetData(noise);
	}
```

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/perlin-noise/perlin-noise/noise-type-option.png)

*噪声类型配置选项。*

### 2.2 1D 梯度

Ken Perlin 从未制作过一个合适的 1D 噪声变体，因为它不是很有用，但我们这样做是因为它使理解更高维度变得更加容易。我们之前已经使用固定梯度函数 $f(x) = x$ 测试了 1D 梯度噪声。Perlin 噪声的思想是晶格点的梯度可以不同。在 1D 的情况下，最明显的其他梯度很简单，就是我们已经使用的负版本：$f(x)=-x$ 我们可以使用哈希的第一位来确定我们是选择正版本还是负版本。

```c#
	public struct Perlin : IGradient {
		
		public float4 Evaluate (SmallXXHash4 hash, float4 x) =>
			select(-x, x, ((uint4)hash & 1) == 0);
		
		…
	}
```

这导致每个晶格跨度有四种可能的梯度插值：正-正、负-负、正-负和负-负。由于正负相互镜像，只有两种独特的情况：相同和不同的梯度。

> **当使用不同的梯度时，噪声是否仍然是 C2 连续的？**
>
> 对。二阶导数在格点处始终为零，一阶导数是连续的，因为在格点的两侧仍然使用相同的梯度。
>
> ![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/perlin-noise/perlin-noise/n-p-n-graph.png)
>
> 负-正-负梯度，导数除以 8 进行拟合。
>
> ![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/perlin-noise/perlin-noise/p-p-n-graph.png)
>
> 正负梯度，导数除以 8 进行拟合。

噪声在具有相反梯度的格点之间的中途达到最大振幅 0.5。这是因为在这一点上两者都是 0.5，我们最终得到了它们的平均值。理想情况下，噪声的最大振幅为 1，我们可以通过加倍梯度来实现。

```c#
		public float4 Evaluate (SmallXXHash4 hash, float4 x) =>
			2f * select(-x, x, ((uint4)hash & 1) == 0);
```

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/perlin-noise/perlin-noise/1d-binary.png)

*一维二元 Perlin 噪声；域尺度 16；分辨率 256。*

### 2.3 可变梯度

我将此时得到的噪声命名为二元 Perlin 噪声，因为它的梯度只能有两个状态。因此，噪声由一系列梯度组成，这些梯度都指向同一方向，除非有符号翻转。翻转显示为最大振幅波，而图案的其余部分由相同的小波浪组成。这看起来很僵硬，所以让我们尝试一种不同的方法。

使用哈希值将相对坐标按 -1~1 范围内的因子缩放，而不是执行二进制选择。我们仍然需要加倍才能达到最大振幅 1。

```c#
		public float4 Evaluate (SmallXXHash4 hash, float4 x) =>
			2f * (hash.Floats01A * 2f - 1f) * x;
```

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/perlin-noise/perlin-noise/1d-variable.png)

*可变坡度。*

这看起来更有趣，因为我们得到了各种不同振幅的梯度，既有正的，也有负的。这确实使得达到最大振幅的可能性大大降低，因此噪声的平均振幅也降低了。

我们还可以将变量和二进制方法结合起来，使用第一位选择符号并按浮点数 A 进行缩放。

```c#
		public float4 Evaluate (SmallXXHash4 hash, float4 x) =>
			2f * hash.Floats01A * select(-x, x, ((uint4)hash & 1) == 0);
```

但这将包括两个选项中的第一位，它引入了一种依赖关系。为了保持选择的独立性，请使用第九位来确定符号。

```c#
			2f * hash.Floats01A * select(-x, x, ((uint4)hash & 1 << 8) == 0);
```

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/perlin-noise/perlin-noise/1d-variable-different.png)

*不同的可变梯度。*

这种方法的优点是，无论梯度方向如何，我们都可以引入最小振幅。通过这种方式，我们可以防止出现退化区域，即多个连续晶格点的尺度最终接近零，从而产生平坦区域。最简单的方法是将最小振幅设置为 1，并将哈希浮点数添加到该值上。结果可以被认为是二进制和变量方法的混合，保证了最小的波幅，但在此基础上增加了一些变化。

```c#
		public float4 Evaluate (SmallXXHash4 hash, float4 x) =>
			(1f + hash.Floats01A) * select(-x, x, ((uint4)hash & 1 << 8) == 0);
```

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/perlin-noise/perlin-noise/1d-mix.png)

*二进制变量混合。*

### 2.4 2D 梯度

接下来是 2D Perlin 噪声，我们再次从基于第一个哈希位的一维二进制梯度开始，看看它是什么样子的。

```c#
		public float4 Evaluate (SmallXXHash4 hash, float4 x, float4 y) =>
			select(-x, x, ((uint4)hash & 1) == 0);
```

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/perlin-noise/perlin-noise/2d-binary.png)

*二维二元 Perlin 噪声平面；域尺度 8；自上而下的视角。*

结果是在一个维度上沿另一个维度插值不同的二进制 1D 噪声带。让我们再次用一种基于浮点 A 的方法来替换它，使梯度更加多样化。

```c#
		public float4 Evaluate (SmallXXHash4 hash, float4 x, float4 y) =>
			(hash.Floats01A * 2f - 1f) * x;
```

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/perlin-noise/perlin-noise/2d-variable.png)

*沿 X 方向的可变梯度。*

在 2D 噪声的情况下，我们不限于轴对齐的梯度，梯度向量可以旋转 360°。为了生成这样的向量，我们可以使用类似于生成八面体球体形状的方法，但将其简化为二维。

我们从沿着 X 的一条线开始，从 -1 到 1。然后，我们使 Y 等于 0.5 减去 X 的绝对值，就像我们在创建八面体的前半部分时定义的 Z 一样，但现在少了一个维度。这创建了一个可以被视为开放正方形的楔形。然后，我们从 X 中减去 X 加 0.5 的底数，以移动正方形的负部分，使其闭合。

![y](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/perlin-noise/perlin-noise/2d-gradient-y-graph.png) ![x and y](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/perlin-noise/perlin-noise/2d-gradient-xy-graph.png) ![square](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/perlin-noise/perlin-noise/2d-gradient-square-graph.png)

*从直线创建正方形的过程。*

结果是一个正方形，其角在 X 和 Y 轴上，距离原点 0.5。因此，梯度向量从原点指向该正方形边缘的某个地方。

为了评估 2D 中的梯度，我们将其 X 和 Y 分量相加。最后，我们将结果加倍，这样轴对齐的向量的长度最终为 1。

```c#
		public float4 Evaluate (SmallXXHash4 hash, float4 x, float4 y) {
			float4 gx = hash.Floats01A * 2f - 1f;
			float4 gy = 0.5f - abs(gx);
			gx -= floor(gx + 0.5f);
			return (gx * x + gy * y) * 2f;
		}
```

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/perlin-noise/perlin-noise/2d-square-based.png)

*基于正方形的渐变。*

如果我们想把平方分布变成一个合适的圆，我们可以通过除以长度来归一化梯度向量。请注意，这不会产生均匀的圆形梯度分布，因为它们是沿着正方形均匀分布的。它们更集中在正方形的角附近，就像八面体球体上的点更集中在八面体的角附近一样。然而，这不是问题，因为分布是对称的，变化足够大。

```c#
			return (gx * x + gy * y) * rsqrt(gx * gx + gy * gy);
```

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/perlin-noise/perlin-noise/2d-normalized.png)

*归一化梯度。*

然而，这并没有太大区别，所以我们可以省略这部分，使噪声代码运行得更快。归一化确实会使对角线梯度更强，但噪声本身仍需要归一化，这也会解决这个问题。

```c#
			return (gx * x + gy * y) * 2f;
```

### 2.5 归一化二维噪声

为了对噪声进行归一化，我们需要确定其当前的最大振幅。如果一个晶格正方形有四个梯度都指向它的中心，那么它的最大值将在在中间达到。由于这将是四个相等梯度的平均值，我们只需要计算该点上单个梯度的值。完全对角梯度是 $f(x,y)=\frac{x+y}2$，因此，中间的振幅将是 $f(0.5,0.5)=\frac 1 2$。然而，这不是整个噪声的最大幅度。还有一种梯度配置具有更大的振幅。

单个维度中的最大值为 0.5，位于晶格跨度的中间，此时两个渐变沿该轴指向彼此。这是非标准化的 1D 二进制 Perlin 噪声。如果晶格正方形另一跨度的两个梯度都指向相反跨度的直线，那么在沿第二维插值的过程中，我们可能会在某个地方得到一个超过 0.5 的值。

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/perlin-noise/perlin-noise/2d-max-gradients.png)

*最大振幅的梯度。*

最大值位于第一维度的中点和第二维度插值的某处。因此，这实际上是轴对齐梯度和 0.5 之间的平滑插值。因此，我们必须找到函数 $xs(1-x)+0.5sx$ 的最大值，带有 $s(t)=6t^5-15t^4+10t^3$ 其中 $x$ 的范围 0–1

该函数的展开形式为 $6x(1-x)^5-15x(1-x)^4+10x(1-x)^3+3x^5-7.5x^4+5x^3$，简化为 $-6x^6+18x^5-17.5x^4+5x^3+x$。

为了找到最大值，我们必须取该函数的导数并求解零，因为当变化率为零时，函数已经达到最大值或最小值。求解这个方程是可能的，但并非易事。为了了解函数的外观，我们可以使用 [Desmos](https://www.desmos.com/calculator/l4otmowqbe) 对其进行可视化。通过选择图线，我们还可以立即了解到在 X 坐标 0.6509 处的最大值为 0.5353。所以它确实高于 0.5。

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/perlin-noise/perlin-noise/2d-max-graph.png)

*2D 最大值图。*

通过让 [Wolfram|Alpha](https://www.wolframalpha.com/input/?i=Maximize%5B%7B-6x%5E6%2B18x%5E5-17.5x%5E4%2B5x%5E3%2Bx%2C0%3C%3Dx%3C%3D1%7D%2C%7Bx%7D%5D) 最大化函数，我们可以得到更精确的答案。它甚至可以给我们一个确切的答案，但它非常复杂，所以我们只能用近似值来解决。0.53528 就足够了，比 Desmos 显示的精确一位数。将梯度除以该值，得到归一化的 2D Perlin 噪声。

```c#
			return (gx * x + gy * y) * (2f / 0.53528f);
```

![top](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/perlin-noise/perlin-noise/2d-normalized-top.png)
![perspective](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/perlin-noise/perlin-noise/2d-normalized-perspective.png)

*归一化的 2D Perlin 噪声。*

> **圆形梯度的最大振幅是多少？**
>
> 完美对角线梯度的圆形版本是 $f(x,y)=\frac{x+y}{\sqrt 2}$。因此，在所有梯度都指向其中心的格子正方形中心发现的最大振幅为 $f(0.5,0.5)=\frac{1}{\sqrt 2}\approx0.7071$。因此，为了归一化噪声，我们必须除以它，这与乘以  $\sqrt2\approx1.4142$ 相同

### 2.6 3D 梯度

为了创建 3D Perlin 噪声，我们必须做与 2D 版本相同的事情，扩展到包括第三维。为此，我们可以使用与生成八面体球体相同的基于八面体的方法。在这种情况下，我们需要 −1~1 范围内的两个随机值。我们将使用浮点数 A 和 D。D 优于 B 或 C，因为单个比特移位比移位和掩码操作都快。

```c#
		public float4 Evaluate (SmallXXHash4 hash, float4 x, float4 y, float4 z) {
			float4 gx = hash.Floats01A * 2f - 1f, gy = hash.Floats01D * 2f - 1f;
			float4 gz = 1f - abs(gx) - abs(gy);
			float4 offset = max(-gz, 0f);
			gx += select(-offset, offset, gx < 0f);
			gy += select(-offset, offset, gy < 0f);
			return (gx * x + gy * y + gz * z);
		}
```

> **3D Perlin 噪声不是只使用 12 个梯度向量吗？**
>
> Perlin 的参考实现确实从一组十二个不同方向的二维对角线梯度向量中挑选出来——指向立方体边缘的中间——经过一些重复，总共有十六个选项。这样，四个比特可以转换为梯度。虽然这种方法对于常规代码或专用硬件可能很有效，但嵌套的二进制分支不适合 SIMD 代码。我们基于八面体的方法既更快——如果我们不归一化梯度——又提供了更多的多样性。

找到最大振幅的工作原理与 2D 相同。在沿第三维插值时，发现沿该轴指向已达到最大值的 2D 晶格矩形的梯度为 0.53528。因此，我们必须像以前一样最大化相同的函数，除了将常数 0.5 替换为 0.53528：$xs(1-x)+0.53528s(x)$，可以扩展为 $6x(1-x)^5-15x(1-x)^4+10x(1-x)^3+0.53528(6x^5-15x^4+10x^3)$。将其输入 [Desmos](https://www.desmos.com/calculator/vbypftfk0x)，在 X 坐标 0.6732 处得到 0.5629。

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/perlin-noise/perlin-noise/3d-max-graph.png)

*3D 最大值图。*

[Wolfram|Alpha](https://www.wolframalpha.com/input/?i=Maximize%5B%7B6x%281-x%29%5E5-15x%281-x%29%5E4%2B10x%281-x%29%5E3%2B0.53528%286x%5E5-15x%5E4%2B10x%5E3%29%2C0%3C%3Dx%3C%3D1%7D%2C%7Bx%7D%5D) 告诉我们，它是 0.56290，在 0.67321。

```c#
return (gx * x + gy * y + gz * z) * (1f / 0.56290f);
```

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/perlin-noise/perlin-noise/3d-sphere.png)

*基于八面体的 3D Perlin 噪声。*

> **球形 3D 梯度的最大振幅是多少？**
>
> 对于基于球体而非八面体的梯度向量，直接指向格子立方体中心的梯度向量为 $f(x,y,z)=\frac{x+y+z}{\sqrt 3}$。因此，在所有梯度都指向其中心的晶格立方体的中间发现的最大幅度是 $f(0.5,0.5,0.5)=\frac{0.5+0.5+0.5}{\sqrt 3}=0.8660$。因此，为了使噪声标准化，我们必须除以它。

下一个教程是“[噪波变体](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/noise-variants/)”。

[license](https://catlikecoding.com/unity/tutorials/license/)

[repository](https://bitbucket.org/catlikecodingunitytutorials/pseudorandom-noise-04-perlin-noise/)

[PDF](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/perlin-noise/Perlin-Noise.pdf)



# 噪声变体：分形和平铺

发布于 2021-07-07 更新于 2022-03-10

https://catlikecoding.com/unity/tutorials/pseudorandom-noise/noise-variants/

*组合多个八度音阶的噪声以创建分形图案。*
*介绍 Perlin 的湍流版本并评估噪声。*
*添加创建平铺噪波的选项。*

这是关于[伪随机噪声](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/)的系列教程中的第五篇。它添加了分形噪声、湍流和平铺。

![img](https://catlikecoding.com/unity/tutorials/pseudorandom-noise/noise-variants/tutorial-image.jpg)

本教程使用 Unity 2020.3.12f1 编写。

一个显示六个八度音阶的间隙 3 分形 3D Perlin 噪声的圆环。

## 1 分形噪声

到目前为止，我们只使用了 Perlin 的单个样本或每个点的值噪声。结果看起来是随机的，但图案的所有特征都有相同的大小。有很多种，但它是基于一个统一的晶格。所有变化都存在于一个单一的尺度上，由域变换决定。噪音在更大和更小的尺度上缺乏多样性，这暴露了它的人工性质。

我们可以通过以不同的尺度再次采样噪声来引入第二频率的变化。最简单的方法是在基本尺度上对其进行采样，也可以在两倍的尺度上进行采样。两个样本的总和产生了一种具有大规模和小规模变化的噪声。我们可以乘以这个倍数，在更大的域尺度上，每个额外的样本都会添加更小的特征。由于较小的特征与较大的特征相似，因此产生的模式将表现出自相似性，因此这种结果被称为分形噪声。

### 1.1 噪音设置

为了支持分形噪声，我们必须添加更多的配置选项来控制它。为了便于将配置传递给噪声，我们将首先创建一个公共 `Noise.Settings` 结构体，最初只包含一个种子整数字段。由于此结构纯粹是为了方便地对配置选项进行分组，我们将字段设置为公共字段，并将该结构用 `System.Serializable` 特性标记为可序列化。这样 Unity 就可以保存配置，并且很容易访问其内容。