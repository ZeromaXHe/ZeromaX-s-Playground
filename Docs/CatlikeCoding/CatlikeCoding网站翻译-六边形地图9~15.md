# [返回主 Markdown](./CatlikeCoding网站翻译.md)



# Hex Map 9：地形特征

发布于 2016-09-23

https://catlikecoding.com/unity/tutorials/hex-map/part-9/

*将细节对象添加到地形中。*
*支持特征密度级别。*
*每个级别使用各种对象。*
*混合三种不同的特征类型。*

本教程是关于[六边形地图](https://catlikecoding.com/unity/tutorials/hex-map/)系列的第九部分。本期内容是关于为地形添加细节。建筑物和树木等特征。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-9/tutorial-image.jpg)

*树木、农田和城市化之间的冲突。*

## 1 添加对特征的支持

虽然我们的地形形状有变化，但没有太多变化。这是一个没有生命的地方。为了让它活起来，我们需要添加树木和建筑等东西。这些特征不是地形网格的一部分。它们是独立的物体。但这并不能阻止我们在对地形进行三角剖分时添加它们。

`HexGridChunk` 不关心网格是如何工作的。它只需命令其 `HexMesh` 子对象之一添加三角形或四边形。同样，它可以有一个孩子来照顾它的特征放置。

### 1.1 特征管理器

让我们创建一个 `HexFeatureManager` 组件，负责单个块的功能。使用与 `HexMesh` 相同的设计，我们将为其提供 `Clear`、`Apply` 和 `AddFeature` 方法。由于特征必须放置在某个地方，`AddFeature` 方法会获得一个位置参数。

我们从一个实际上什么都不做的存根（stub）实现开始。

```c#
using UnityEngine;

public class HexFeatureManager : MonoBehaviour {

	public void Clear () {}

	public void Apply () {}

	public void AddFeature (Vector3 position) {}
}
```

我们现在可以向 `HexGridChunk` 添加对此类组件的引用。然后我们可以像所有 `HexMesh` 子对象一样，将其包含在三角剖分过程中。

```c#
	public HexFeatureManager features;

	public void Triangulate () {
		terrain.Clear();
		rivers.Clear();
		roads.Clear();
		water.Clear();
		waterShore.Clear();
		estuaries.Clear();
		features.Clear();
		for (int i = 0; i < cells.Length; i++) {
			Triangulate(cells[i]);
		}
		terrain.Apply();
		rivers.Apply();
		roads.Apply();
		water.Apply();
		waterShore.Apply();
		estuaries.Apply();
		features.Apply();
	}
```

让我们从在每个单元格的中心放置一个特征开始。

```c#
	void Triangulate (HexCell cell) {
		for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
			Triangulate(d, cell);
		}
		features.AddFeature(cell.Position);
	}
```

现在我们需要实际的功能管理器。将另一个子对象添加到 *Hex Grid Chunk* 预制件中，并为其提供 `HexFeatureManager` 组件。然后我们可以将块连接到它。

![hierarchy](https://catlikecoding.com/unity/tutorials/hex-map/part-9/adding-support-for-features/hierarchy.png) ![feature object](https://catlikecoding.com/unity/tutorials/hex-map/part-9/adding-support-for-features/features-child-object.png) ![chunk prefab](https://catlikecoding.com/unity/tutorials/hex-map/part-9/adding-support-for-features/chunk-prefab.png)

特征管理器已添加到块预制件中。

### 1.2 特征预制

我们应该做什么样的功能？对于我们的第一个测试，一个立方体就可以了。创建一个相当大的立方体，比如缩放到（3,3,3），然后把它变成一个预制件。也为它创建一个材质。我使用了默认的红色材质。拆下它的碰撞体，因为我们不需要它。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-9/adding-support-for-features/feature-prefab.png)

*特征立方体预制件。*

我们的特征管理器需要一个对这个前言的引用，所以在 `HexFeatureManager` 中添加一个，然后将它们连接起来。因为放置需要访问变换组件，所以将其用作引用类型。

```c#
	public Transform featurePrefab;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-9/adding-support-for-features/manager-with-prefab.png)

*带预制件的特征管理器。*

### 1.3 实例化特征

我们的设置完成了，我们可以开始添加特征了！这就像在 `HexFeatureManager.AddFeature` 中实例化预制件一样简单并设置位置。

```c#
	public void AddFeature (Vector3 position) {
		Transform instance = Instantiate(featurePrefab);
		instance.localPosition = position;
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-9/adding-support-for-features/instances.png)

*特征实例。*

从现在开始，地形将充满立方体。至少，立方体的上半部分。因为 Unity 立方体网格的局部原点位于立方体的中心，所以下半部分被淹没在地形表面之下。要将立方体放置在地形顶部，我们必须将它们向上移动一半的高度。

```c#
	public void AddFeature (Vector3 position) {
		Transform instance = Instantiate(featurePrefab);
		position.y += instance.localScale.y * 0.5f;
		instance.localPosition = position;
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-9/adding-support-for-features/instances-moved.png)

*坐在地面上。*

> **如果我们使用不同的网格呢？**
>
> 此方法专门用于默认多维数据集。如果您使用的是自定义网格，最好将其设计为使其本地原点位于底部。那么你根本不需要调整位置。

当然，我们的单元格会受到干扰，所以我们也应该干扰我们特征的位置。这消除了网格的完美规则性。

```c#
		instance.localPosition = HexMetrics.Perturb(position);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-9/adding-support-for-features/instances-perturbed.png)

*受干扰的特征。*

### 1.4 破坏特征

每次刷新块时，我们都会创建新特征。这意味着我们目前在相同的位置上不断创建越来越多的功能。为了防止重复，我们必须在清除块时删除旧功能。

一种快速的方法是创建一个容器游戏对象，并使所有特征都成为该对象的子对象。然后，当调用 `Clear` 时，我们销毁这个容器并创建一个新的容器。容器本身将是其管理器的子级。

```c#
	Transform container;

	public void Clear () {
		if (container) {
			Destroy(container.gameObject);
		}
		container = new GameObject("Features Container").transform;
		container.SetParent(transform, false);
	}
	
	…
	
	public void AddFeature (Vector3 position) {
		Transform instance = Instantiate(featurePrefab);
		position.y += instance.localScale.y * 0.5f;
		instance.localPosition = HexMetrics.Perturb(position);
		instance.SetParent(container, false);
	}
```

> **一直创建和销毁功能不是效率低下吗？**
>
> 感觉确实如此。但我们现在不应该担心。首先，我们要正确放置特征。一旦我们解决了这个问题，而事实证明这是一个瓶颈，那么我们就可以明智地提高效率。这时我们可能会使用HexFeatureManager。也应用方法。但这是未来的教程。幸运的是，它真的没那么糟糕，因为我们已经把地形分成了块。

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-9/adding-support-for-features/adding-support-for-features.unitypackage)

## 2 特征摆放

我们目前正在每个单元格的中心放置一个功能。这对于其他空单元格来说是可以的。但对于含有河流和道路的单元格，或者水下的单元格来说，这看起来并不好。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-9/feature-placement/everywhere.png)

*特征无处不在。*

因此，在 `HexGridChunk.Triangulate` 中向单元格添加特征之前，让我们确保单元格是清晰的。

```c#
		if (!cell.IsUnderwater && !cell.HasRiver && !cell.HasRoads) {
			features.AddFeature(cell.Position);
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-9/feature-placement/limited.png)

*受限的放置。*

### 2.1 每个方向一个特征

每个单元格只有一个特征并不多。有足够的空间容纳更多。让我们在单元格的六个三角形的中心添加一个附加特征。所以每个方向一个。

当我们知道没有河流时，我们在另一种 `Triangulate` 方法中这样做。我们仍然需要检查我们是否在水下，或者是否有道路。但在这种情况下，我们只关心沿当前方向行驶的道路。

```c#
	void Triangulate (HexDirection direction, HexCell cell) {
		…

		if (cell.HasRiver) {
			…
		}
		else {
			TriangulateWithoutRiver(direction, cell, center, e);

			if (!cell.IsUnderwater && !cell.HasRoadThroughEdge(direction)) {
				features.AddFeature((center + e.v1 + e.v5) * (1f / 3f));
			}
		}

		…
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-9/feature-placement/many-features.png)

*许多特征，但不靠近河流。*

这产生了更多的特征！它们出现在公路旁，但仍然避开河流。为了获取河流沿岸的特征，我们还可以在 `TriangulateAdjacentToRiver` 中添加它们。但再一次，只有在不在水下，也不在公路上的时候。

```c#
	void TriangulateAdjacentToRiver (
		HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e
	) {
		…

		if (!cell.IsUnderwater && !cell.HasRoadThroughEdge(direction)) {
			features.AddFeature((center + e.v1 + e.v5) * (1f / 3f));
		}
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-9/feature-placement/features-adjacent-to-river.png)

*也毗邻河流。*

> **我们能渲染那么多对象吗？**
>
> 许多功能会产生许多绘图调用，但Unity的动态批处理对我们有所帮助。由于特征很小，它们的网格应该只有很少的顶点。这使得它们中的许多可以组合成一批。但如果事实证明这是一个瓶颈，我们稍后会处理。也可以使用实例化，这与使用许多小网格时的动态批处理相当。

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-9/feature-placement/feature-placement.unitypackage)

## 3 特征多样性

我们所有的特征对象都有完全相同的方向，这看起来一点也不自然。所以，让我们给每一个随机旋转。

```c#
	public void AddFeature (Vector3 position) {
		Transform instance = Instantiate(featurePrefab);
		position.y += instance.localScale.y * 0.5f;
		instance.localPosition = HexMetrics.Perturb(position);
		instance.localRotation = Quaternion.Euler(0f, 360f * Random.value, 0f);
		instance.SetParent(container, false);
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-9/feature-variety/rotated.png)

*随机旋转。*

这产生了更加多样化的结果。不幸的是，每次刷新一个块时，它的特征最终都会出现新的随机旋转。编辑一些东西不应该让附近的特征痉挛，所以我们需要一种不同的方法。

我们有一个噪波纹理，它总是一样的。然而，该纹理包含 Perlin 梯度噪声，这是局部相干的。这正是我们在扰动顶点单元位置时想要的。但我们不需要连贯的旋转。所有旋转都应该是同样可能的，并且是混合的。所以我们需要的是一个具有非梯度随机值的纹理，并在没有双线性滤波的情况下对其进行采样。这实际上是一个哈希网格（hash grid），它构成了梯度噪声（gradient noise）的基础。

### 3.1 创建哈希网格

我们可以用浮点数数组创建一个哈希网格，并用随机值填充一次。这样我们根本不需要纹理。让我们把它添加到 `HexMetrics` 中。256 乘 256 应该提供足够的多样性。

```c#
	public const int hashGridSize = 256;

	static float[] hashGrid;

	public static void InitializeHashGrid () {
		hashGrid = new float[hashGridSize * hashGridSize];
		for (int i = 0; i < hashGrid.Length; i++) {
			hashGrid[i] = Random.value;
		}
	}
```

随机值由总是产生相同结果的数学公式生成。您得到的序列取决于种子号，种子号默认为当前时间值。这就是为什么每次游玩 session 都会得到不同的结果。

为了重新创建完全相同的特征，我们必须在初始化方法中添加一个种子参数。

```c#
	public static void InitializeHashGrid (int seed) {
		hashGrid = new float[hashGridSize * hashGridSize];
		Random.InitState(seed);
		for (int i = 0; i < hashGrid.Length; i++) {
			hashGrid[i] = Random.value;
		}
	}
```

现在我们已经初始化了随机数流，我们将始终从中得到相同的序列。因此，生成地图后发生的所有所谓的随机事件也将始终相同。我们可以通过在初始化随机数生成器之前保存其状态来防止这种情况。完成后，我们将其设置回旧状态。

```c#
		Random.State currentState = Random.state;
		Random.InitState(seed);
		for (int i = 0; i < hashGrid.Length; i++) {
			hashGrid[i] = Random.value;
		}
		Random.state = currentState;
```

哈希网格的初始化由 `HexGrid` 完成，同时它分配噪声纹理。这就是 `HexGrid.Start` 和 `HexGrid.Awake`。确保我们生成它的频率不超过必要的频率。

```c#
	public int seed;

	void Awake () {
		HexMetrics.noiseSource = noiseSource;
		HexMetrics.InitializeHashGrid(seed);

		…
	}

	void OnEnable () {
		if (!HexMetrics.noiseSource) {
			HexMetrics.noiseSource = noiseSource;
			HexMetrics.InitializeHashGrid(seed);
		}
	}
```

公共种子允许我们为地图选择种子值。任何值都可以。我选择了 1234。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-9/feature-variety/seed.png)

*选择一颗种子。*

### 3.2 使用哈希网格

要使用哈希网格，请向 `HexMetrics` 添加采样方法。与 `SampleNoise` 类似，它使用位置的 XZ 坐标来检索值。哈希索引是通过将坐标夹紧为整数值，然后将整数除以网格大小的余数来找到的。

```c#
	public static float SampleHashGrid (Vector3 position) {
		int x = (int)position.x % hashGridSize;
		int z = (int)position.z % hashGridSize;
		return hashGrid[x + z * hashGridSize];
	}
```

> **% 做什么？**
>
> 这是模运算符。它计算剩余的除法，在我们的例子中是整数除法。例如，序列 −4，−3，−2，−1，0，1，2，3，4 模 3 变为 −1, 0, −2, −1, 0, 1, 2, 0, 1。

这适用于正坐标，但不适用于负坐标，因为对于这些数字，其余部分将为负。我们可以通过将网格大小添加到负面结果中来解决这个问题。

```c#
		int x = (int)position.x % hashGridSize;
		if (x < 0) {
			x += hashGridSize;
		}
		int z = (int)position.z % hashGridSize;
		if (z < 0) {
			z += hashGridSize;
		}
```

现在，我们为每个平方单位生成不同的值。我们其实不需要这么密集的网格。这些特征远不止于此。在计算索引之前，我们可以通过缩小位置来拉伸网格。每个 4 乘 4 平方的唯一值就足够了。

```c#
	public const float hashGridScale = 0.25f;

	public static float SampleHashGrid (Vector3 position) {
		int x = (int)(position.x * hashGridScale) % hashGridSize;
		if (x < 0) {
			x += hashGridSize;
		}
		int z = (int)(position.z * hashGridScale) % hashGridSize;
		if (z < 0) {
			z += hashGridSize;
		}
		return hashGrid[x + z * hashGridSize];
	}
```

返回 `HexFeatureManager.AddFeature` 并使用我们新的哈希网格来获取值。一旦我们使用它来设置旋转，当我们编辑地形时，我们的特征将保持静止。

```c#
	public void AddFeature (Vector3 position) {
		float hash = HexMetrics.SampleHashGrid(position);
		Transform instance = Instantiate(featurePrefab);
		position.y += instance.localScale.y * 0.5f;
		instance.localPosition = HexMetrics.Perturb(position);
		instance.localRotation = Quaternion.Euler(0f, 360f * hash, 0f);
		instance.SetParent(container, false);
	}
```

### 3.3 放置门槛

虽然特征具有不同的旋转，但它们的放置仍然有明显的模式。每个单元都有七个特征挤满了它。我们可以通过任意省略一些特征来给这个设置引入混乱。我们如何决定是否添加特征？通过参考另一个随机值！

所以现在我们需要两个哈希值，而不是一个。我们通过使用 `Vector2` 而不是 `float` 作为哈希网格数组类型来支持这一点。但是向量操作对我们的哈希值没有意义，所以让我们为此创建一个特殊的结构。它只需要两个 float。让我们添加一个静态方法来创建随机值对。

```c#
using UnityEngine;

public struct HexHash {

	public float a, b;

	public static HexHash Create () {
		HexHash hash;
		hash.a = Random.value;
		hash.b = Random.value;
		return hash;
	}
}
```

> **它不需要序列化吗？**
>
> 我们只将这些结构存储在哈希网格中，哈希网格是静态的，因此在重新编译期间不会被 Unity 序列化。因此，它不需要是可序列化的。

调整 `HexMetrics`，使其使用此新结构。

```c#
	static HexHash[] hashGrid;

	public static void InitializeHashGrid (int seed) {
		hashGrid = new HexHash[hashGridSize * hashGridSize];
		Random.State currentState = Random.state;
		Random.InitState(seed);
		for (int i = 0; i < hashGrid.Length; i++) {
			hashGrid[i] = HexHash.Create();
		}
		Random.state = currentState;
	}

	public static HexHash SampleHashGrid (Vector3 position) {
		…
	}
```

现在是 `HexFeatureManager.AddFeature` 可以访问两个哈希值。让我们使用第一个来决定我们是否真的添加了一个特征，或者跳过它。如果该值为 0.5 或更大，我们退出。这将消除大约一半的特征。我们像往常一样使用第二个值来确定旋转。

```c#
	public void AddFeature (Vector3 position) {
		HexHash hash = HexMetrics.SampleHashGrid(position);
		if (hash.a >= 0.5f) {
			return;
		}
		Transform instance = Instantiate(featurePrefab);
		position.y += instance.localScale.y * 0.5f;
		instance.localPosition = HexMetrics.Perturb(position);
		instance.localRotation = Quaternion.Euler(0f, 360f * hash.b, 0f);
		instance.SetParent(container, false);
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-9/feature-variety/threshold.png)

*特征密度降低到 50%。*

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-9/feature-variety/feature-variety.unitypackage)

## 4 绘画特点

与其到处放置特征，不如让它们可编辑。但我们不会绘制单个特征。相反，我们将为每个单元格添加一个特征级别。此级别控制特征出现在单元格中的可能性。默认值为零，这保证不存在任何特征。

由于我们的红色立方体看起来不像地形的自然特征，所以假设它们是建筑物。它们代表着城市的发展。因此，让我们为 `HexCell` 添加一个城市级别。

```c#
	public int UrbanLevel {
		get {
			return urbanLevel;
		}
		set {
			if (urbanLevel != value) {
				urbanLevel = value;
				RefreshSelfOnly();
			}
		}
	}

	int urbanLevel;
```

我们可以确保水下单元格的城市水平为零，但这不是必需的。我们已经在水下省略了特征。也许我们会在某个时候添加城市水景，比如码头或水下结构。

### 4.1 密度滑块

要编辑城市级别，请在 `HexMapEditor` 中添加对另一个滑块的支持。

```c#
	int activeUrbanLevel;
	
	…
	
	bool applyUrbanLevel;
	
	…
	
	public void SetApplyUrbanLevel (bool toggle) {
		applyUrbanLevel = toggle;
	}
	
	public void SetUrbanLevel (float level) {
		activeUrbanLevel = (int)level;
	}

	void EditCell (HexCell cell) {
		if (cell) {
			…
			if (applyWaterLevel) {
				cell.WaterLevel = activeWaterLevel;
			}
			if (applyUrbanLevel) {
				cell.UrbanLevel = activeUrbanLevel;
			}
			if (riverMode == OptionalToggle.No) {
				cell.RemoveRiver();
			}
			…
		}
	}
```

在 UI 中添加另一张幻灯片，并将其连接到相应的方法。我把它放在屏幕右侧的一个新面板上，以防止左侧面板过度拥挤。

我们需要多少个级别？让我们坚持四个，代表零、低、中、高密度开发。

![scene](https://catlikecoding.com/unity/tutorials/hex-map/part-9/painting-features/slider.png) ![inspector](https://catlikecoding.com/unity/tutorials/hex-map/part-9/painting-features/slider-inspector.png)
*城市滑块。*

### 4.2 调整阈值

现在我们有了城市等级，我们必须用它来确定我们是否放置了特征。为此，我们必须将城市级别作为额外参数添加到 `HexFeatureManager.AddFeature` 中。让我们更进一步，直接传递单元格本身。稍后会更方便。

利用城市等级的一个快速方法是将其乘以 0.25，并将其作为保释（bail）的新门槛。这样，每个级别出现特征的概率增加25%。

```c#
	public void AddFeature (HexCell cell, Vector3 position) {
		HexHash hash = HexMetrics.SampleHashGrid(position);
		if (hash.a >= cell.UrbanLevel * 0.25f) {
			return;
		}
		…
	}
```

要使其工作，请传递 `HexGridChunk` 中的单元格。

```c#
	void Triangulate (HexCell cell) {
		…
		if (!cell.IsUnderwater && !cell.HasRiver && !cell.HasRoads) {
			features.AddFeature(cell,  cell.Position);
		}
	}

	void Triangulate (HexDirection direction, HexCell cell) {
		…
			if (!cell.IsUnderwater && !cell.HasRoadThroughEdge(direction)) {
				features.AddFeature(cell, (center + e.v1 + e.v5) * (1f / 3f));
			}
		…
	}
	
	…
	
	void TriangulateAdjacentToRiver (
		HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e
	) {
		…

		if (!cell.IsUnderwater && !cell.HasRoadThroughEdge(direction)) {
			features.AddFeature(cell, (center + e.v1 + e.v5) * (1f / 3f));
		}
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-9/painting-features/urban-levels.png)

*绘制城市密度水平。*

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-9/painting-features/painting-features.unitypackage)

## 5 多功能预制件

特征概率的差异不足以明确区分较低和较高的城市水平。一些单元格最终会比预期的更少或更多的建筑物。我们可以通过为每个级别使用不同的预制体来使差异更加明显。

删除 `HexFeatureManager` 中的 `featurePrefab` 字段，并将其替换为城市预制件的数组。使用城市标高减一作为索引来检索相应的预制件。

```c#
//	public Transform featurePrefab;
	public Transform[] urbanPrefabs;
	
	public void AddFeature (HexCell cell, Vector3 position) {
		…
		Transform instance = Instantiate(urbanPrefabs[cell.UrbanLevel - 1]);
		…
	}
```

创建特征预制件的两个副本，重命名并调整它们以表示三个不同的城市标高。第一层是低密度，所以我用一个单位大小的立方体来代表一个棚屋。我将 2 层预制结构的比例设置为（1.5，2，1.5），以建议建造一座更大的两层建筑。对于第三层，我用（2,5,2）表示高层建筑。

![inspector](https://catlikecoding.com/unity/tutorials/hex-map/part-9/multiple-feature-prefabs/multiple-prefabs-inspector.png) ![scene](https://catlikecoding.com/unity/tutorials/hex-map/part-9/multiple-feature-prefabs/multiple-prefabs.png)
*为每个城市级别使用不同的预制件。*

### 5.1 混合预制件

我们不需要把自己限制在建筑类型的严格隔离上。我们可以把它们混合在一起，就像在现实世界中一样。与其在每层使用单个阈值，不如在每层中使用三个阈值，每种建筑类型使用一个阈值。

对于第一级，让我们使用 40% 的机会建造一座小屋。另一栋楼根本不会出现。这需要阈值三元组（0.4，0，0）。

对于第二级，让我们用更大的建筑取代棚屋，并增加 20% 的机会建造更多的棚屋。仍然没有高层建筑。这表明阈值三元组（0.2，0.4，0）。

对于第三层，让我们将中型建筑升级为高层建筑，再次更换棚屋，并为更多的棚屋增加 20% 的改动。阈值为（0.2，0.2，0.4）。

因此，我们的想法是，随着城市水平的提高，我们升级现有的建筑，并在空地上增加新的建筑。要替换现有的建筑，我们必须使用相同的哈希值范围。如果 0 到 0.4 之间的哈希值是 1 级的棚屋，那么相同的范围应该会在 3 级产生较高的涨幅。具体来说，在 3 层，高层建筑的哈希值应在 0-0.4 范围内，两层楼的房子在 0.4-0.6 范围内，小屋在 0.6-0.8 范围内。如果我们从最高到最低检查它们，我们可以用阈值三元组（0.4、0.6、0.8）来做到这一点。然后，2 级阈值变为（0，0.4，0.6），1 级阈值变为了（0，0，0.4）。

让我们将这些阈值作为数组的集合存储在 `HexMetrics` 中，并使用一种方法来获取特定级别的阈值。由于我们只关注具有特征的级别，因此忽略了级别 0。

```c#
	static float[][] featureThresholds = {
		new float[] {0.0f, 0.0f, 0.4f},
		new float[] {0.0f, 0.4f, 0.6f},
		new float[] {0.4f, 0.6f, 0.8f}
	};
						
	public static float[] GetFeatureThresholds (int level) {
		return featureThresholds[level];
	}
```

接下来，我们在 `HexFeatureManager` 中添加一个方法，该方法使用级别和哈希值来选择一个预制件。如果级别大于零，我们将使用降低一的级别检索阈值。然后我们循环遍历阈值，直到一个阈值超过哈希值。这意味着我们找到了一个预制件。如果没有，则返回 null。

```c#
	Transform PickPrefab (int level, float hash) {
		if (level > 0) {
			float[] thresholds = HexMetrics.GetFeatureThresholds(level - 1);
			for (int i = 0; i < thresholds.Length; i++) {
				if (hash < thresholds[i]) {
					return urbanPrefabs[i];
				}
			}
		}
		return null;
	}
```

这种方法要求我们重新排序预制引用，使其从高密度变为低密度。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-9/multiple-feature-prefabs/reversed-prefabs.png)

*反向预制件。*

在 `AddFeature` 中使用此新方法来拾取预制件。如果我们最终没有任一个，保释（bail）。否则，实例化它并像以前一样继续。

```c#
	public void AddFeature (HexCell cell, Vector3 position) {
		HexHash hash = HexMetrics.SampleHashGrid(position);
//		if (hash.a >= cell.UrbanLevel * 0.25f) {
//			return;
//		}
//		Transform instance = Instantiate(urbanPrefabs[cell.UrbanLevel - 1]);
		Transform prefab = PickPrefab(cell.UrbanLevel, hash.a);
		if (!prefab) {
			return;
		}
		Transform instance = Instantiate(prefab);
		position.y += instance.localScale.y * 0.5f;
		instance.localPosition = HexMetrics.Perturb(position);
		instance.localRotation = Quaternion.Euler(0f, 360f * hash.b, 0f);
		instance.SetParent(container, false);
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-9/multiple-feature-prefabs/mixing-prefabs.png)

*混合预制件。*

### 5.2 每级变化

到目前为止，我们已经有了很好的建筑组合，但仍然只有三个不同的建筑。通过将预制件的集合与每个城市密度水平相关联，我们可以进一步增加多样性。然后我们随机选择其中一个。这需要一个新的随机值，因此在HexHash中添加第三个。

```c#
	public float a, b, c;

	public static HexHash Create () {
		HexHash hash;
		hash.a = Random.value;
		hash.b = Random.value;
		hash.c = Random.value;
		return hash;
	}
```

将 `HexFeatureManager.urbanPrefabs` 转换为数组，并将 `choice` 参数添加到 `PickPrefab` 方法中。使用它通过将嵌套数组与该数组的长度相乘并转换为整数来对嵌套数组进行索引。

```c#
	public Transform[][] urbanPrefabs;
	
	…

	Transform PickPrefab (int level, float hash, float choice) {
		if (level > 0) {
			float[] thresholds = HexMetrics.GetFeatureThresholds(level - 1);
			for (int i = 0; i < thresholds.Length; i++) {
				if (hash < thresholds[i]) {
					return urbanPrefabs[i][(int)(choice * urbanPrefabs[i].Length)];
				}
			}
		}
		return null;
	}
```

让我们根据第二个哈希值 B 进行选择。这要求旋转从 B 变为 C。

```c#
	public void AddFeature (HexCell cell, Vector3 position) {
		HexHash hash = HexMetrics.SampleHashGrid(position);
		Transform prefab = PickPrefab(cell.UrbanLevel, hash.a, hash.b);
		if (!prefab) {
			return;
		}
		Transform instance = Instantiate(prefab);
		position.y += instance.localScale.y * 0.5f;
		instance.localPosition = HexMetrics.Perturb(position);
		instance.localRotation = Quaternion.Euler(0f, 360f * hash.c, 0f);
		instance.SetParent(container, false);
	}
```

在继续之前，我们必须知道 `Random.value` 可以产生值 1。这将导致我们的数组索引超出界限。为了确保不会发生这种情况，请将哈希值缩小一点。只需将它们全部缩小，这样我们就不必担心使用哪一个。

```c#
	public static HexHash Create () {
		HexHash hash;
		hash.a = Random.value * 0.999f;
		hash.b = Random.value * 0.999f;
		hash.c = Random.value * 0.999f;
		return hash;
	}
```

不幸的是，检查器没有显示数组数组。因此，我们无法对其进行配置。为了解决这个问题，创建一个封装嵌套数组的可序列化结构。给它一个方法来处理从选项到数组索引的转换，并返回前缀。

```c#
using UnityEngine;

[System.Serializable]
public struct HexFeatureCollection {

	public Transform[] prefabs;

	public Transform Pick (float choice) {
		return prefabs[(int)(choice * prefabs.Length)];
	}
}
```

在 `HexFeatureManager` 中使用这些集合的数组，而不是嵌套数组。

```c#
//	public Transform[][] urbanPrefabs;
	public HexFeatureCollection[] urbanCollections;

	…

	Transform PickPrefab (int level, float hash, float choice) {
		if (level > 0) {
			float[] thresholds = HexMetrics.GetFeatureThresholds(level - 1);
			for (int i = 0; i < thresholds.Length; i++) {
				if (hash < thresholds[i]) {
					return urbanCollections[i].Pick(choice);
				}
			}
		}
		return null;
	}
```

现在，您可以为每个密度级别定义多个建筑。由于它们是独立的，您不需要在每个级别使用相同的金额。我只是在每个级别上使用了两个变体，并为每个级别添加了一个较长的较低变体。我将它们的比例设置为（3.5，3，2），（2.75，1.5，1.5）和（1.75，1，1）。

![inspector](https://catlikecoding.com/unity/tutorials/hex-map/part-9/multiple-feature-prefabs/collections-inspector.png) ![scene](https://catlikecoding.com/unity/tutorials/hex-map/part-9/multiple-feature-prefabs/collections.png)

*每个密度级别有两种建筑类型。*

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-9/multiple-feature-prefabs/multiple-feature-prefabs.unitypackage)

## 6 多种特征类型

通过我们目前的设置，我们可以创建体面的城市环境。但地形可以包含的不仅仅是建筑物。农场呢？植物呢？让我们也为 `HexCell` 添加这些级别。他们不是排他性的，他们可以混合。

```c#
	public int FarmLevel {
		get {
			return farmLevel;
		}
		set {
			if (farmLevel != value) {
				farmLevel = value;
				RefreshSelfOnly();
			}
		}
	}

	public int PlantLevel {
		get {
			return plantLevel;
		}
		set {
			if (plantLevel != value) {
				plantLevel = value;
				RefreshSelfOnly();
			}
		}
	}

	int urbanLevel, farmLevel, plantLevel;
```

当然，这需要在 `HexMapEditor` 中支持两个附加滑动条。

```c#
	int activeUrbanLevel, activeFarmLevel, activePlantLevel;

	bool applyUrbanLevel, applyFarmLevel, applyPlantLevel;

	…

	public void SetApplyFarmLevel (bool toggle) {
		applyFarmLevel = toggle;
	}

	public void SetFarmLevel (float level) {
		activeFarmLevel = (int)level;
	}

	public void SetApplyPlantLevel (bool toggle) {
		applyPlantLevel = toggle;
	}

	public void SetPlantLevel (float level) {
		activePlantLevel = (int)level;
	}

	…

	void EditCell (HexCell cell) {
		if (cell) {
			…
			if (applyUrbanLevel) {
				cell.UrbanLevel = activeUrbanLevel;
			}
			if (applyFarmLevel) {
				cell.FarmLevel = activeFarmLevel;
			}
			if (applyPlantLevel) {
				cell.PlantLevel = activePlantLevel;
			}
			…
		}
	}
```

按预期将它们添加到 UI 中。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-9/multiple-feature-types/sliders.png)

*三个滑块。*

`HexFeatureManager` 也需要额外的集合。

```c#
	public HexFeatureCollection[]
		urbanCollections, farmCollections, plantCollections;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-9/multiple-feature-types/three-sets-of-collections.png)

*三个特色系列。*

我给农场和工厂每个密度水平提供了两个预制件，就像城市收藏一样。我给它们都用了立方体。农场有浅绿色的材料，而植物有深绿色的材料。

我把农场立方体做成 0.1 个单位高，代表矩形农田。高密度标度为（2.5，0.1，2.5）和（3.5，0.1，2）。中等地块为 1.75 平方和 2.5 乘 1.25。低密度得到 1 平方和 1.5 乘 0.75。

植物预制件代表高大的树木和大型灌木。密度最大的是（1.25，4.5，1.25）和（1.5，3，1.5）。中等尺度是（0.75，3，0.75）和（1，1.5，1）。最小的植物有大小（0.5，1.5，0.5）和（0.75，1，0.75）。

### 6.1 特征选择

每种特征类型都应该有自己的哈希值，所以它们有不同的生成模式。这使得将它们混合在一起成为可能。因此，向 `HexHash` 添加两个附加值。

```c#
	public float a, b, c, d, e;

	public static HexHash Create () {
		HexHash hash;
		hash.a = Random.value * 0.999f;
		hash.b = Random.value * 0.999f;
		hash.c = Random.value * 0.999f;
		hash.d = Random.value * 0.999f;
		hash.e = Random.value * 0.999f;
		return hash;
	}
```

`HexFeatureManager.PickPrefab` 现在必须处理不同的集合。为此添加一个参数。此外，将用于预制变量选择的哈希值更改为 D，将用于旋转的哈希值修改为 E。

```c#
	Transform PickPrefab (
		HexFeatureCollection[] collection,
		int level, float hash, float choice
	) {
		if (level > 0) {
			float[] thresholds = HexMetrics.GetFeatureThresholds(level - 1);
			for (int i = 0; i < thresholds.Length; i++) {
				if (hash < thresholds[i]) {
					return collection[i].Pick(choice);
				}
			}
		}
		return null;
	}

	public void AddFeature (HexCell cell, Vector3 position) {
		HexHash hash = HexMetrics.SampleHashGrid(position);
		Transform prefab = PickPrefab(
			urbanCollections, cell.UrbanLevel, hash.a, hash.d
		);
		…
		instance.localRotation = Quaternion.Euler(0f, 360f * hash.e, 0f);
		instance.SetParent(container, false);
	}
```

目前，`AddFeature` 选择了一个城市预制件。这很好，但现在我们有了更多的选择。所以，让我们从农场里再挑一个预制件。我们将使用 B 作为它的哈希值。变体选择可以再次依赖于 D。

```c#
		Transform prefab = PickPrefab(
			urbanCollections, cell.UrbanLevel, hash.a, hash.d
		);
		Transform otherPrefab = PickPrefab(
			farmCollections, cell.FarmLevel, hash.b, hash.d
		);
		if (!prefab) {
			return;
		}
```

我们最终要实例化哪个前言？如果其中一个最终为空，那么选择就很明确了。但当两者都存在时，我们必须做出决定。让我们只使用哈希值最低的那个。

```c#
		Transform otherPrefab = PickPrefab(
			farmCollections, cell.FarmLevel, hash.b, hash.d
		);
		if (prefab) {
			if (otherPrefab && hash.b < hash.a) {
				prefab = otherPrefab;
			}
		}
		else if (otherPrefab) {
			prefab = otherPrefab;
		}
		else {
			return;
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-9/multiple-feature-types/urban-farm.png)

*城市和农场的混合特征。*

接下来，我们使用 C 哈希值对植物进行同样的操作。

```c#
		if (prefab) {
			if (otherPrefab && hash.b < hash.a) {
				prefab = otherPrefab;
			}
		}
		else if (otherPrefab) {
			prefab = otherPrefab;
		}
		otherPrefab = PickPrefab(
			plantCollections, cell.PlantLevel, hash.c, hash.d
		);
		if (prefab) {
			if (otherPrefab && hash.c < hash.a) {
				prefab = otherPrefab;
			}
		}
		else if (otherPrefab) {
			prefab = otherPrefab;
		}
		else {
			return;
		}
```

然而，我们不能就这样复制代码。当我们最终选择农场而不是城市时，我们应该将植物哈希值与农场哈希值进行比较。不是城市哈希。因此，我们必须跟踪我们决定使用的哈希值，并与该哈希值进行比较。

```c#
		float usedHash = hash.a;
		if (prefab) {
			if (otherPrefab && hash.b < hash.a) {
				prefab = otherPrefab;
				usedHash = hash.b;
			}
		}
		else if (otherPrefab) {
			prefab = otherPrefab;
			usedHash = hash.b;
		}
		otherPrefab = PickPrefab(
			plantCollections, cell.PlantLevel, hash.c, hash.d
		);
		if (prefab) {
			if (otherPrefab && hash.c < usedHash) {
				prefab = otherPrefab;
			}
		}
		else if (otherPrefab) {
			prefab = otherPrefab;
		}
		else {
			return;
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-9/multiple-feature-types/urban-farm-plant.png)

*城市、农场和植物特色混合。*

下一个教程是[城墙](https://catlikecoding.com/unity/tutorials/hex-map/part-10/)。

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-9/multiple-feature-types/multiple-feature-types.unitypackage)

[PDF](https://catlikecoding.com/unity/tutorials/hex-map/part-9/Hex-Map-9.pdf)

# Hex Map 10：城墙

发布于 2016-10-14

https://catlikecoding.com/unity/tutorials/hex-map/part-10/

*隔离单元格。*
*沿单元格边缘建造墙壁。*
*让河流和道路穿过。*
*避开水，与悬崖相连。*

本教程是关于[六边形地图](https://catlikecoding.com/unity/tutorials/hex-map/)系列的第十部分。这一次，我们将在单元格之间添加墙壁。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-10/tutorial-image.jpg)

*没有什么比一堵大墙更能表达欢迎了。*

## 1 编辑墙

为了支撑墙壁，我们必须知道把它们放在哪里。我们将把它们放在单元格之间，沿着连接它们的边缘。由于我们现有的功能被放置在单元的中心区域，我们不需要担心墙壁会穿过这些特征。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-10/editing-walls/wall-placement.png)

*沿边缘放置的墙。*

墙是地形特征，尽管很大。与其他特征一样，我们不会直接编辑它们。相反，我们编辑单元格。我们不会放置单独的墙段，我们会把整个单元格围起来。

### 1.1 围墙属性

为了支持有墙单元格，让我们向 `HexCell` 添加一个 `walled` 属性。这是一个简单的切换。因为墙被放置在单元格之间，所以我们必须刷新编辑过的单元格及其邻居。

```c#
	public bool Walled {
		get {
			return walled;
		}
		set {
			if (walled != value) {
				walled = value;
				Refresh();
			}
		}
	}
	
	bool walled;
```

### 1.2 编辑器切换

要调整单元格的墙状态，我们必须在 `HexMapEditor` 中添加对切换的支持。因此，添加另一个 `OptionalToggle` 字段和一个设置它的方法。

```c#
	OptionalToggle riverMode, roadMode, walledMode;
	
	…
	
	public void SetWalledMode (int mode) {
		walledMode = (OptionalToggle)mode;
	}
```

与河流和道路不同，墙壁不会从一个单元格传到另一个单元格。他们在他们之间。所以我们不必担心拖后腿。当墙切换处于活动状态时，只需根据切换设置当前单元格的墙状态。

```c#
	void EditCell (HexCell cell) {
		if (cell) {
			…
			if (roadMode == OptionalToggle.No) {
				cell.RemoveRoads();
			}
			if (walledMode != OptionalToggle.Ignore) {
				cell.Walled = walledMode == OptionalToggle.Yes;
			}
			if (isDrag) {
				…
			}
		}
	}
```

复制另一个切换的 UI 元素之一，并对其进行调整，使其控制墙状态。我将它们与其他功能一起放在 UI 面板中。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-10/editing-walls/ui.png)

*墙切换。*

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-10/editing-walls/editing-walls.unitypackage)

## 2 创建墙

因为墙壁遵循单元格的轮廓，所以它们没有固定的形状。因此，我们不能像对其他功能那样，简单地为它们使用预制件。相反，我们必须构建一个网格，就像我们对地形所做的那样。这意味着我们的块预制需要另一个 `HexMesh` 子对象。复制其其他网格子对象之一，并确保新的 *Walls* 对象投射阴影。除了顶点和三角形之外，它不需要任何东西，因此应禁用所有 `HexMesh` 选项。

![hierarchy](https://catlikecoding.com/unity/tutorials/hex-map/part-10/creating-walls/walls-hierarchy.png) ![inspector](https://catlikecoding.com/unity/tutorials/hex-map/part-10/creating-walls/walls-inspector.png)
*墙壁预制件子。(Walls prefab child.)*

墙壁是一种城市特征，这是有道理的，所以我为它们使用了红色的城市材料。

### 2.1 管理墙

因为墙是特征，所以它们是 `HexFeatureManager` 的责任。因此，给特征管理器一个 *Walls* 对象的引用，并让它调用 `Clear` 和 `Apply` 方法。

```c#
	public HexMesh walls;

	…

	public void Clear () {
		…
		walls.Clear();
	}

	public void Apply () {
		walls.Apply();
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-10/creating-walls/walls-features.png)

*连接到特征管理器的墙。*

> ***Walls* 不应该是 *Features* 的孩子吗？**
>
> 你可以这样排列物体，但这不是必需的。因为层次视图只显示预制根对象的直接子对象，所以我更喜欢将 *Walls* 保持为 *Hex Grid Chunk* 的直接子节点。

现在我们必须向管理器添加一个方法，以便可以向其中添加墙。由于墙存在于单元格之间的边上，因此需要知道相关的边顶点和单元格。`HexGridChunk` 将通过 `TriangulateConnection` 调用它，因此当前正在对单元格及其邻居之一进行三角剖分。从这个角度来看，当前单元格位于墙壁的近侧，其他单元格位于远侧。

```c#
	public void AddWall (
		EdgeVertices near, HexCell nearCell,
		EdgeVertices far, HexCell farCell
	) {
	}
```

在 `HexGridChunk.TriangulateConnection` 中在所有其他连接工作完成之后调用此新方法，就在我们继续进行到角落三角形之前。我们将由特征管理器决定是否应该实际放置墙。

```c#
	void TriangulateConnection (
		HexDirection direction, HexCell cell, EdgeVertices e1
	) {
		…

		if (cell.GetEdgeType(direction) == HexEdgeType.Slope) {
			…
		}
		else {
			…
		}

		features.AddWall(e1, cell, e2, neighbor);

		HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
		if (direction <= HexDirection.E && nextNeighbor != null) {
			…
		}
	}
```

### 2.2 构建墙段

整面墙将蜿蜒穿过多个单元格边缘。每条边只包含墙的一段。从近单元的角度来看，该段从边缘的左侧开始，在右侧结束。让我们在 `HexFeatureManager` 中添加一个单独的方法，该方法基于边角的四个顶点创建单个线段。

```c#
	void AddWallSegment (
		Vector3 nearLeft, Vector3 farLeft, Vector3 nearRight, Vector3 farRight
	) {
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-10/creating-walls/near-far.png)

*近侧和远侧。*

`AddWall` 可以使用边的第一个和最后一个顶点调用此方法。但是，只有当我们在有墙的单元格和没有墙的单元格之间建立连接时，才应该添加墙。哪种单元格在内部或外部并不重要，只是它们的状态不同。

```c#
	public void AddWall (
		EdgeVertices near, HexCell nearCell,
		EdgeVertices far, HexCell farCell
	) {
		if (nearCell.Walled != farCell.Walled) {
			AddWallSegment(near.v1, far.v1, near.v5, far.v5);
		}
	}
```

最简单的墙段是位于边缘中间的单个四边形。我们通过从近顶点到远顶点的中间插值来找到它的底部顶点。

```c#
	void AddWallSegment (
		Vector3 nearLeft, Vector3 farLeft, Vector3 nearRight, Vector3 farRight
	) {
		Vector3 left = Vector3.Lerp(nearLeft, farLeft, 0.5f);
		Vector3 right = Vector3.Lerp(nearRight, farRight, 0.5f);
	}
```

我们的墙应该有多高？让我们在 `HexMetrics` 中定义一下。我让它们高达一个标高。

```c#
	public const float wallHeight = 3f;
```

`HexFeatureManager.AddWallSegment` 可以使用此高度来定位四边形的第三个和第四个顶点，并将其添加到 `walls` 网格中。

```c#
		Vector3 left = Vector3.Lerp(nearLeft, farLeft, 0.5f);
		Vector3 right = Vector3.Lerp(nearRight, farRight, 0.5f);

		Vector3 v1, v2, v3, v4;
		v1 = v3 = left;
		v2 = v4 = right;
		v3.y = v4.y = left.y + HexMetrics.wallHeight;
		walls.AddQuad(v1, v2, v3, v4);
```

现在，您可以编辑墙，它们将显示为四边形条带。然而，你不会看到一堵完整的墙。每个四边形仅从一侧可见。它的脸朝向它被添加的单元格。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-10/creating-walls/single-sided.png)

*单面墙四边形。*

我们可以通过添加面向另一侧的第二个四边形来快速解决这个问题。

```c#
		walls.AddQuad(v1, v2, v3, v4);
		walls.AddQuad(v2, v1, v4, v3);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-10/creating-walls/two-sided.png)

*两面墙。*

现在可以看到整个墙壁，尽管在三个单元格相交的角落仍然有缝隙。我们稍后再填。

### 2.3 厚墙

虽然从两侧都可以看到墙壁，但它们没有任何厚度。墙壁实际上薄如纸，从某些视角几乎看不见。所以，让我们通过增加厚度来使它们坚固。让我们定义一下它们在 `HexMetrics` 中的厚度。我选择了 0.75 单位作为一个在我看来不错的任意值。

```c#
	public const float wallThickness = 0.75f;
```

为了使墙壁变厚，我们必须把两个四边形拉开。他们必须朝着相反的方向前进。一侧应向近边缘移动，另一侧应向远边缘移动。执行此操作的偏移向量只是 `far - near`，但为了保持墙的顶部平坦，我们应该将其 Y 分量设置为零。

因为我们必须对墙段的左右部分都这样做，所以让我们在 `HexMetrics` 中添加一个方法来计算这个偏移向量。

```c#
	public static Vector3 WallThicknessOffset (Vector3 near, Vector3 far) {
		Vector3 offset;
		offset.x = far.x - near.x;
		offset.y = 0f;
		offset.z = far.z - near.z;
		return offset;
	}
```

为了使墙保持在边缘的中心，沿此矢量移动的实际距离等于每侧厚度的一半。为了确保我们确实移动了所需的距离，在缩放之前对偏移向量进行归一化。

```c#
		return offset.normalized * (wallThickness * 0.5f);
```

在 `HexFeatureManager.AddWallSegment` 中使用此方法以调整四边形的位置。随着偏移向量从近到远，将其从近四边形中减去，然后将其添加到远四边形中。

```c#
		Vector3 left = Vector3.Lerp(nearLeft, farLeft, 0.5f);
		Vector3 right = Vector3.Lerp(nearRight, farRight, 0.5f);
		
		Vector3 leftThicknessOffset =
			HexMetrics.WallThicknessOffset(nearLeft, farLeft);
		Vector3 rightThicknessOffset =
			HexMetrics.WallThicknessOffset(nearRight, farRight);

		Vector3 v1, v2, v3, v4;
		v1 = v3 = left - leftThicknessOffset;
		v2 = v4 = right - rightThicknessOffset;
		v3.y = v4.y = left.y + HexMetrics.wallHeight;
		walls.AddQuad(v1, v2, v3, v4);

		v1 = v3 = left + leftThicknessOffset;
		v2 = v4 = right + rightThicknessOffset;
		v3.y = v4.y = left.y + HexMetrics.wallHeight;
		walls.AddQuad(v2, v1, v4, v3);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-10/creating-walls/offset-walls.png)

*带有偏移的墙。*

四边形现在是偏移的，尽管不是那么明显。阴影暴露了它。

> **壁厚真的均匀吗？**
>
> 如果远近偏移矢量都指向完全相同的方向，那么情况就会如此。当单元格壁围绕单元格弯曲时，情况显然并非如此。矢量指向彼此远离或朝向对方。因此，墙段的底部是梯形，而不是矩形。因此，它最终比我们配置的厚度薄一些。此外，由于单元格受到扰动，矢量之间的角度会发生变化，从而导致厚度不均匀。我们稍后会对此进行改进。

### 2.4 墙顶

为了使墙的厚度从上方可见，我们必须在墙上添加一个四边形。添加它的一个简单方法是记住第一个四边形的顶部两个顶点，并将它们与第二个四边形顶部两个连接起来。

```c#
		Vector3 v1, v2, v3, v4;
		v1 = v3 = left - leftThicknessOffset;
		v2 = v4 = right - rightThicknessOffset;
		v3.y = v4.y = left.y + HexMetrics.wallHeight;
		walls.AddQuad(v1, v2, v3, v4);

		Vector3 t1 = v3, t2 = v4;

		v1 = v3 = left + leftThicknessOffset;
		v2 = v4 = right + rightThicknessOffset;
		v3.y = v4.y = left.y + HexMetrics.wallHeight;
		walls.AddQuad(v2, v1, v4, v3);

		walls.AddQuad(t1, t2, v3, v4);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-10/creating-walls/wall-tops.png)

*有顶的墙。*

### 2.5 转弯

其余的间隙位于单元格的角落。为了填充这些，我们必须在单元格之间的三角形区域中添加一段。每个角落连接三个单元格。每个单元格都可以有壁或没有壁。所以有八种可能的配置。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-10/creating-walls/corner-configurations.png)

*拐角配置。*

我们只在具有不同墙壁状态的单元格之间放置墙壁。这将相关配置的数量减少到六个。在每个单元格中，有一个单元格位于壁曲线的内侧。让我们把这个单元格看作是墙弯曲的枢轴。从这个单元的角度来看，墙从与左单元共享的边缘开始，到与右单元共享的边结束。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-10/creating-walls/pivot.png)

*单元格角色。*

因此，我们必须创建一个 `AddWallSegment` 方法，该方法具有三个角顶点作为参数。虽然我们可以编写代码来三角剖分这个分段，但它实际上是另一个 `AddWallSegment` 方法的特例。枢轴同时扮演两个近顶点的角色。

```c#
	void AddWallSegment (
		Vector3 pivot, HexCell pivotCell,
		Vector3 left, HexCell leftCell,
		Vector3 right, HexCell rightCell
	) {
		AddWallSegment(pivot, left, pivot, right);
	}
```

接下来，为三个角顶点及其单元格创建一个 `AddWall` 方法变体。这种方法的工作是找出哪个角是枢轴（如果有的话）。因此，它必须考虑所有八种可能的配置，并为其中六种调用 `AddWallSegment`。

```c#
	public void AddWall (
		Vector3 c1, HexCell cell1,
		Vector3 c2, HexCell cell2,
		Vector3 c3, HexCell cell3
	) {
		if (cell1.Walled) {
			if (cell2.Walled) {
				if (!cell3.Walled) {
					AddWallSegment(c3, cell3, c1, cell1, c2, cell2);
				}
			}
			else if (cell3.Walled) {
				AddWallSegment(c2, cell2, c3, cell3, c1, cell1);
			}
			else {
				AddWallSegment(c1, cell1, c2, cell2, c3, cell3);
			}
		}
		else if (cell2.Walled) {
			if (cell3.Walled) {
				AddWallSegment(c1, cell1, c2, cell2, c3, cell3);
			}
			else {
				AddWallSegment(c2, cell2, c3, cell3, c1, cell1);
			}
		}
		else if (cell3.Walled) {
			AddWallSegment(c3, cell3, c1, cell1, c2, cell2);
		}
	}
```

要添加角段，请在 `HexGridChunk.TriangulateCorner` 末尾调用此方法。

```c#
	void TriangulateCorner (
		Vector3 bottom, HexCell bottomCell,
		Vector3 left, HexCell leftCell,
		Vector3 right, HexCell rightCell
	) {
		…
		
		features.AddWall(bottom, bottomCell, left, leftCell, right, rightCell);
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-10/creating-walls/wall-corners.png)

*有角的墙，但仍然有缝隙。*

### 2.6 缩小差距

墙中仍有间隙，因为墙段的标高不一致。虽然沿边的线段具有恒定的标高，但角线段位于两条不同的边之间。由于每条边可以有不同的标高，因此在拐角处会出现间隙。

要解决此问题，请调整 `AddWallSegment`，使其保持左右顶部顶点的 Y 坐标分离。

```c#
		float leftTop = left.y + HexMetrics.wallHeight;
		float rightTop = right.y + HexMetrics.wallHeight;

		Vector3 v1, v2, v3, v4;
		v1 = v3 = left - leftThicknessOffset;
		v2 = v4 = right - rightThicknessOffset;
		v3.y = leftTop;
		v4.y = rightTop;
		walls.AddQuad(v1, v2, v3, v4);

		Vector3 t1 = v3, t2 = v4;

		v1 = v3 = left + leftThicknessOffset;
		v2 = v4 = right + rightThicknessOffset;
		v3.y = leftTop;
		v4.y = rightTop;
		walls.AddQuad(v2, v1, v4, v3);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-10/creating-walls/wall-closed.png)

*封闭的墙壁。*

墙现在已关闭，但您可能仍然可以看到墙阴影中的缝隙。这是由平行光阴影设置的“法线偏移”引起的。当大于零时，阴影投射器的三角形将沿着曲面法线移动。这可以防止自阴影，但也会在三角形彼此背对的地方产生间隙。这会在薄几何体的阴影中产生可见的间隙，就像我们的墙壁一样。

通过将法线偏差减小到零，可以消除这些阴影伪影。或者，将墙的网格渲染器的“*投射阴影*”模式更改为“*双面*”。这迫使阴影投射器通过以渲染每个墙三角形的两侧，从而覆盖孔洞。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-10/creating-walls/no-normal-bias.png)

*再也没有阴影间隙。*

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-10/creating-walls/creating-walls.unitypackage)

## 3 阶地上的墙壁

目前，我们的墙壁相当笔直。在平坦的地形上，这并没有那么糟糕，但当墙壁与梯田重合时，看起来很奇怪。当墙两侧的单元之间存在一层高差时，就会发生这种情况。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-10/walls-on-terraces/straight.png)

*阶地上的直墙。*

### 3.1 追随边缘

与其为整个边创建单个线段，不如为边缘条的每个部分创建一个线段。我们可以通过在 `AddWall` 版本中为边调用 `AddWallSegment` 四次来实现这一点。

```c#
	public void AddWall (
		EdgeVertices near, HexCell nearCell,
		EdgeVertices far, HexCell farCell
	) {
		if (nearCell.Walled != farCell.Walled) {
			AddWallSegment(near.v1, far.v1, near.v2, far.v2);
			AddWallSegment(near.v2, far.v2, near.v3, far.v3);
			AddWallSegment(near.v3, far.v3, near.v4, far.v4);
			AddWallSegment(near.v4, far.v4, near.v5, far.v5);
		}
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-10/walls-on-terraces/twisting.png)

*扭曲的墙壁。*

墙现在遵循受扰边的形状。这与露台结合起来看起来好多了。它还可以在平坦的地形上制作更有趣的墙壁。

### 3.2 在地面上放置墙壁

当仔细观察阶台上的墙壁时，我们发现有一个问题。墙壁最终漂浮在地面上！倾斜的平边也是如此，但通常不那么明显。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-10/walls-on-terraces/floating.png)

*浮动墙。*

为了解决这个问题，我们必须降低墙壁。最简单的方法就是降低整面墙，使其顶部保持平坦。这将导致较高一侧的部分墙壁沉入地形中，这很好。

要降低墙壁，我们首先必须确定哪一侧最低，近侧还是远侧。我们可以只使用最低侧的高度，但我们不需要那么低。我们可以用低于 0.5 的偏移量从低 Y 坐标插值到高 Y 坐标。由于我们的墙壁很少延伸到最低的露台台阶之外，我们可以使用垂直梯田台阶的尺寸作为我们的偏移。梯田配置的不同壁厚可能需要另一个偏移。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-10/walls-on-terraces/lowered-wall.png)

*低墙。*

让我们在 `HexMetrics` 中添加一个 `WallLerp` 方法，该方法除了对近顶点和远顶点的 X 和 Z 坐标进行平均外，还负责插值。它基于 `TerraceLerp` 方法。

```c#
	public const float wallElevationOffset = verticalTerraceStepSize;
						
	…
						
	public static Vector3 WallLerp (Vector3 near, Vector3 far) {
		near.x += (far.x - near.x) * 0.5f;
		near.z += (far.z - near.z) * 0.5f;
		float v =
			near.y < far.y ? wallElevationOffset : (1f - wallElevationOffset);
		near.y += (far.y - near.y) * v;
		return near;
	}
```

让 `HexFeatureManager` 使用此方法确定左顶点和右顶点。

```c#
	void AddWallSegment (
		Vector3 nearLeft, Vector3 farLeft, Vector3 nearRight, Vector3 farRight
	) {
		Vector3 left = HexMetrics.WallLerp(nearLeft, farLeft);
		Vector3 right = HexMetrics.WallLerp(nearRight, farRight);

		…
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-10/walls-on-terraces/grounded.png)

*接地的墙。*

### 3.3 调整墙壁扰动

我们的墙壁现在可以很好地适应海拔差异。但它们仍然不完全匹配受扰的边缘，尽管它很接近。这是因为我们首先计算出墙的顶点，然后扰动它们。由于这些顶点位于近边顶点和远边顶点之间的某个位置，它们的扰动将略有不同。

墙壁不完全沿着边缘走并不是问题。然而，扰动墙的顶点会扰乱其相对均匀的厚度。如果我们使用受扰顶点定位墙，然后添加未受扰四边形，其厚度不应变化太大。

```c#
	void AddWallSegment (
		Vector3 nearLeft, Vector3 farLeft, Vector3 nearRight, Vector3 farRight
	) {
		nearLeft = HexMetrics.Perturb(nearLeft);
		farLeft = HexMetrics.Perturb(farLeft);
		nearRight = HexMetrics.Perturb(nearRight);
		farRight = HexMetrics.Perturb(farRight);

		…
		walls.AddQuadUnperturbed(v1, v2, v3, v4);

		…
		walls.AddQuadUnperturbed(v2, v1, v4, v3);

		walls.AddQuadUnperturbed(t1, t2, v3, v4);
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-10/walls-on-terraces/unperturbed-vertices.png)

*不受干扰的墙顶点。*

使用这种方法，我们的墙不再像以前那样紧贴边缘。但作为回报，它们的锯齿状更少，厚度也更一致。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-10/walls-on-terraces/consistent-thickness.png)

*壁厚更加一致。*

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-10/walls-on-terraces/walls-on-terraces.unitypackage)

## 4 墙壁开口

到目前为止，我们忽略了河流或道路可能穿过墙壁的可能性。当这种情况发生时，我们应该在墙上留出一个缺口，这样河流或道路就可以通过。

为了支持这一点，在 `AddWall` 中添加两个布尔参数，以指示是否有河流或道路穿过边缘。虽然我们可以区别对待它们，但让我们在这两种情况下都删除中间的两个部分。

```c#
	public void AddWall (
		EdgeVertices near, HexCell nearCell,
		EdgeVertices far, HexCell farCell,
		bool hasRiver, bool hasRoad
	) {
		if (nearCell.Walled != farCell.Walled) {
			AddWallSegment(near.v1, far.v1, near.v2, far.v2);
			if (hasRiver || hasRoad) {
				// Leave a gap.
			}
			else {
				AddWallSegment(near.v2, far.v2, near.v3, far.v3);
				AddWallSegment(near.v3, far.v3, near.v4, far.v4);
			}
			AddWallSegment(near.v4, far.v4, near.v5, far.v5);
		}
	}
```

现在是 `HexGridChunk.TriangulateConnection` 必须提供必要的数据。因为它之前已经需要相同的信息，所以让我们把它缓存在布尔变量中，只写一次相关的方法调用。

```c#
	void TriangulateConnection (
		HexDirection direction, HexCell cell, EdgeVertices e1
	) {
		…

		bool hasRiver = cell.HasRiverThroughEdge(direction);
		bool hasRoad = cell.HasRoadThroughEdge(direction);

		if (hasRiver) {
			…
		}

		if (cell.GetEdgeType(direction) == HexEdgeType.Slope) {
			TriangulateEdgeTerraces(e1, cell, e2, neighbor, hasRoad);
		}
		else {
			TriangulateEdgeStrip(e1, cell.Color, e2, neighbor.Color, hasRoad);
		}

		features.AddWall(e1, cell, e2, neighbor, hasRiver, hasRoad);
		…
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-10/wall-openings/gaps.png)

*墙上的缝隙，让河流和道路穿过。*

### 4.1 为墙壁加盖

新的间隙引入了墙结束的地方。我们必须用四边形盖住这些端点，这样我们就不会从墙的侧面看过去。为此，在 `HexFeatureManager` 中创建一个 `AddWallCap` 方法。它的工作方式类似于 `AddWallSegment`，除了它只需要一对远近顶点。让它添加一个从墙的近侧到远侧的四边形。

```c#
	void AddWallCap (Vector3 near, Vector3 far) {
		near = HexMetrics.Perturb(near);
		far = HexMetrics.Perturb(far);

		Vector3 center = HexMetrics.WallLerp(near, far);
		Vector3 thickness = HexMetrics.WallThicknessOffset(near, far);

		Vector3 v1, v2, v3, v4;

		v1 = v3 = center - thickness;
		v2 = v4 = center + thickness;
		v3.y = v4.y = center.y + HexMetrics.wallHeight;
		walls.AddQuadUnperturbed(v1, v2, v3, v4);
	}
```

当 `AddWall` 确定我们需要一个间隙时，在第二对和第四对边顶点之间添加一个帽。我们必须为第四个顶点对切换方向，否则该四边形最终将面向内部。

```c#
	public void AddWall (
		EdgeVertices near, HexCell nearCell,
		EdgeVertices far, HexCell farCell,
		bool hasRiver, bool hasRoad
	) {
		if (nearCell.Walled != farCell.Walled) {
			AddWallSegment(near.v1, far.v1, near.v2, far.v2);
			if (hasRiver || hasRoad) {
				AddWallCap(near.v2, far.v2);
				AddWallCap(far.v4, near.v4);
			}
			…
		}
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-10/wall-openings/capped.png)

*堵墙缝隙。*（*Capped wall gaps.*）

> **地图边缘的空白怎么办？**
>
> 你也可以采取措施盖住那里的墙。就我个人而言，我避免把墙放在地图的边缘。你不希望你的游戏太接近边缘。

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-10/wall-openings/wall-openings.unitypackage)

## 5 避开悬崖和水

最后，让我们考虑包含悬崖或水的边缘。因为悬崖实际上是巨大的墙壁，所以在悬崖中间再加一堵墙是没有意义的。而且看起来很糟糕。此外，水下墙也没什么意义。把海岸线围起来也不好看。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-10/avoiding-cliffs-and-water/cliffs-and-water.png)

*悬崖上的墙壁和水中的墙壁。*

我们可以在 `AddWall` 中通过额外的检查来消除这些不合适的边上的墙。这两个单元格都不能在水下，它们的共同边缘也不能是悬崖。

```c#
	public void AddWall (
		EdgeVertices near, HexCell nearCell,
		EdgeVertices far, HexCell farCell,
		bool hasRiver, bool hasRoad
	) {
		if (
			nearCell.Walled != farCell.Walled &&
			!nearCell.IsUnderwater && !farCell.IsUnderwater &&
			nearCell.GetEdgeType(farCell) != HexEdgeType.Cliff
		) {
			…
		}
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-10/avoiding-cliffs-and-water/removed-walls.png)

*沿边缘移除了有问题的墙壁，但角落仍然存在。*

### 5.1 拆除墙角

消除不需要的角段需要更多的工作。最容易避免的情况是枢轴单元在水下。这保证了没有任何相邻的墙段需要连接。

```c#
	void AddWallSegment (
		Vector3 pivot, HexCell pivotCell,
		Vector3 left, HexCell leftCell,
		Vector3 right, HexCell rightCell
	) {
		if (pivotCell.IsUnderwater) {
			return;
		}

		AddWallSegment(pivot, left, pivot, right);
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-10/avoiding-cliffs-and-water/underwater-pivot.png)

*不再有水下枢轴。*

现在我们必须看看另外两个单元格。如果其中一个在水下，或者通过悬崖连接到枢轴，那么沿着那个边缘就没有墙了。如果至少一侧是这样，那么这个角落就不应该有墙段。

独立确定是否有左墙或右墙。将结果放在布尔变量中，这样它们更容易推理。

```c#
		if (pivotCell.IsUnderwater) {
			return;
		}

		bool hasLeftWall = !leftCell.IsUnderwater &&
			pivotCell.GetEdgeType(leftCell) != HexEdgeType.Cliff;
		bool hasRighWall = !rightCell.IsUnderwater &&
			pivotCell.GetEdgeType(rightCell) != HexEdgeType.Cliff;

		if (hasLeftWall && hasRighWall) {
			AddWallSegment(pivot, left, pivot, right);
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-10/avoiding-cliffs-and-water/removed-corners.png)

*删除所有违规角落。*

### 5.2 加盖角

当左右边缘都没有墙时，我们就完成了。但是，当一个方向上有一堵墙时，这意味着墙上还有另一个缺口。所以我们必须限制它。

```c#
		if (hasLeftWall) {
			if (hasRighWall) {
				AddWallSegment(pivot, left, pivot, right);
			}
			else {
				AddWallCap(pivot, left);
			}
		}
		else if (hasRighWall) {
			AddWallCap(right, pivot);
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-10/avoiding-cliffs-and-water/capped.png)

*盖上墙壁。*

### 5.3 将墙壁与悬崖融为一体

有一种情况是，我们的墙看起来不太理想。当一堵墙到达悬崖底部时，它就结束了。因为悬崖不是完全垂直的，这在墙壁和悬崖面之间留下了一个狭窄的缝隙。这个问题在悬崖顶上并不存在。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-10/avoiding-cliffs-and-water/wall-cliff-gap.png)

*墙壁和悬崖面之间的缝隙。*

如果一堵墙一直延伸到悬崖面，不留任何缝隙，那就更好了。我们可以通过在墙的当前端和悬崖的角顶点之间添加一个额外的墙段来实现这一点。由于这一段的大部分最终隐藏在悬崖内，我们可以将悬崖内的壁厚减至零。因此，我们只需要创建一个楔形。两个四边形连接到一个点，上面是一个三角形。为此创建一个 `AddWallWedge` 方法。您可以通过复制 `AddWallCap` 并添加楔形点来实现。我已经标出了差异。

```c#
	void AddWallWedge (Vector3 near, Vector3 far, Vector3 point) {
		near = HexMetrics.Perturb(near);
		far = HexMetrics.Perturb(far);
		point = HexMetrics.Perturb(point);

		Vector3 center = HexMetrics.WallLerp(near, far);
		Vector3 thickness = HexMetrics.WallThicknessOffset(near, far);

		Vector3 v1, v2, v3, v4;
		Vector3 pointTop = point;
		point.y = center.y;

		v1 = v3 = center - thickness;
		v2 = v4 = center + thickness;
		v3.y = v4.y = pointTop.y = center.y + HexMetrics.wallHeight;

//		walls.AddQuadUnperturbed(v1, v2, v3, v4);
		walls.AddQuadUnperturbed(v1, point, v3, pointTop);
		walls.AddQuadUnperturbed(point, v2, pointTop, v4);
		walls.AddTriangleUnperturbed(pointTop, v3, v4);
	}
```

在角的 `AddWallSegment` 中，当只有一个方向上有墙，并且该墙的标高低于另一侧时，调用此方法。就在那时，我们遇到了悬崖。

```c#
		if (hasLeftWall) {
			if (hasRighWall) {
				AddWallSegment(pivot, left, pivot, right);
			}
			else if (leftCell.Elevation < rightCell.Elevation) {
				AddWallWedge(pivot, left, right);
			}
			else {
				AddWallCap(pivot, left);
			}
		}
		else if (hasRighWall) {
			if (rightCell.Elevation < leftCell.Elevation) {
				AddWallWedge(right, pivot, left);
			}
			else {
				AddWallCap(right, pivot);
			}
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-10/avoiding-cliffs-and-water/wall-wedges.png)

*连接悬崖的墙楔。*

下一个教程是[更多特征](https://catlikecoding.com/unity/tutorials/hex-map/part-11/)。

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-10/avoiding-cliffs-and-water/avoiding-cliffs-and-water.unitypackage)

[PDF](https://catlikecoding.com/unity/tutorials/hex-map/part-10/Hex-Map-10.pdf)

# Hex Map 11：更多特征

发布于 2016-11-20

https://catlikecoding.com/unity/tutorials/hex-map/part-11/

*在墙上添加塔楼。*
*通过桥梁连接河流上的道路。*
*支持大型特殊功能。*

这是关于[六边形地图](https://catlikecoding.com/unity/tutorials/hex-map/)的系列教程的第十一部分。它为我们的地形增添了墙塔、桥梁和特色。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-11/tutorial-image.jpg)

*特征丰富的景观。*

## 1 墙塔

我们在上一教程中添加了对墙的支持。它们是简单的直墙段，没有任何区别特征。现在，我们将通过在墙上添加塔楼来使墙壁更有趣。

墙段必须按程序创建，以适应地形。塔楼不需要这样。我们可以使用一个简单的预制件。

您可以使用两个具有红色城市材质的立方体创建一个简单的塔形。塔基宽 2 乘 2 个单位，高 4 个单位，所以它既比墙厚又高。在这个立方体上方，放置一个单位立方体来表示塔顶。与其他预制件一样，这些立方体不需要碰撞体。

因为塔模型由多个对象组成，所以将它们作为根对象的子对象。定位它们，使根的局部原点位于塔基。这样，我们就可以放置塔楼，而不必担心它们的高度。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-11/wall-towers/prefab.png)

*墙塔预制件。*

将此预制体的引用添加到 `HexFeatureManager` 并将其连接起来。

```c#
	public Transform wallTower;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-11/wall-towers/inspector.png)

*参考墙塔预制件。*

### 1.1 建造塔楼

让我们首先在每个墙段的中间放置一个塔。为此，请在 `AddWallSegment` 方法的末尾实例化一个塔。它的位置是线段左右点的平均值。

```c#
	void AddWallSegment (
		Vector3 nearLeft, Vector3 farLeft, Vector3 nearRight, Vector3 farRight
	) {
		…

		Transform towerInstance = Instantiate(wallTower);
		towerInstance.transform.localPosition = (left + right) * 0.5f;
		towerInstance.SetParent(container, false);
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-11/wall-towers/tower-instances.png)

*每个墙段一座塔。*

我们沿着墙有很多塔楼，但它们的方向没有变化。我们必须调整它们的旋转，使它们与墙壁对齐。因为我们有墙的左点和右点，所以我们知道哪个方向是正确的。我们可以用它来确定墙段的方向，从而确定其塔的方向。

我们可以为 `Transform.right` 属性指定一个向量，而不是自己计算旋转。Unity 的代码将负责调整对象的旋转，使其局部向右方向与提供的向量对齐。

```c#
		Transform towerInstance = Instantiate(wallTower);
		towerInstance.transform.localPosition = (left + right) * 0.5f;
		Vector3 rightDirection = right - left;
		rightDirection.y = 0f;
		towerInstance.transform.right = rightDirection;
		towerInstance.SetParent(container, false);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-11/wall-towers/towers-rotated.png)

*塔楼与墙壁对齐。*

> **如何设置 `Transform.right`？**
>
> 它使用 `Quaternion.FromToRotation` 方法导出旋转。这是属性的代码。
>
> ```c#
> public Vector3 right {
> 	get {
> 		return rotation * Vector3.right;
> 	}
> 	set {
> 		rotation = Quaternion.FromToRotation(Vector3.right, value);
> 	}
> }
> ```

### 1.2 更少的塔楼

每个墙段一个塔太多了。因此，让我们通过向 `AddWallSegment` 添加布尔参数，使添加塔成为可选操作。给它一个默认值 `false`。这将使所有的塔楼消失。

```c#
	void AddWallSegment (
		Vector3 nearLeft, Vector3 farLeft, Vector3 nearRight, Vector3 farRight,
		bool addTower = false
	) {
		…

		if (addTower) {
			Transform towerInstance = Instantiate(wallTower);
			towerInstance.transform.localPosition = (left + right) * 0.5f;
			Vector3 rightDirection = right - left;
			rightDirection.y = 0f;
			towerInstance.transform.right = rightDirection;
			towerInstance.SetParent(container, false);
		}
	}
```

让我们将塔楼限制在单元格角落的墙段位置。这将导致几座塔之间的距离相当规则。

```c#
	void AddWallSegment (
		Vector3 pivot, HexCell pivotCell,
		Vector3 left, HexCell leftCell,
		Vector3 right, HexCell rightCell
	) {
		…
				AddWallSegment(pivot, left, pivot, right, true);
		…
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-11/wall-towers/corner-towers.png)

*仅限单元格角落的塔楼。*

这看起来相当不错，但你可能想要比这更不规则的墙放置。与其他功能一样，我们可以使用哈希网格来决定是否在角落放置塔。为此，使用角的中心对网格进行采样，然后将其中一个哈希值与塔阈值进行比较。

```c#
				HexHash hash = HexMetrics.SampleHashGrid(
					(pivot + left + right) * (1f / 3f)
				);
				bool hasTower = hash.e < HexMetrics.wallTowerThreshold;
				AddWallSegment(pivot, left, pivot, right, hasTower);
```

阈值属于 `HexMetrics`。值为 0.5 时，大约一半的时间会生成塔，尽管你可能会得到有很多塔或根本没有塔的墙。

```c#
	public const float wallTowerThreshold = 0.5f;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-11/wall-towers/fewer-towers.png)

*偶尔有塔楼。*

### 1.3 斜坡上没有塔楼

我们目前正在放置塔楼，而不管地形的形状如何。然而，在斜坡上建造塔楼并没有多大意义。墙壁在那里成一个角度，可能会穿过塔顶。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-11/wall-towers/slopes-with-towers.png)

*斜坡上的塔楼。*

为了避免斜坡，请检查拐角的左右单元格是否具有相同的标高。只有这样，我们才能允许建造一座潜在的塔。

```c#
				bool hasTower = false;
				if (leftCell.Elevation == rightCell.Elevation) {
					HexHash hash = HexMetrics.SampleHashGrid(
						(pivot + left + right) * (1f / 3f)
					);
					hasTower = hash.e < HexMetrics.wallTowerThreshold;
				}
				AddWallSegment(pivot, left, pivot, right, hasTower);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-11/wall-towers/slopes-without-towers.png)

*斜墙上不再有塔楼。*

### 1.4 接地墙和塔

虽然我们避开了倾斜的墙壁，但墙壁两侧的地形仍然可以有不同的高度。墙可以沿着梯田延伸，同一标高的单元可以有不同的垂直调整。这可能会导致塔基浮动。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-11/wall-towers/towers-floating.png)

*浮塔。*

事实上，倾斜的墙壁也可以漂浮，尽管它不如塔楼那么明显。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-11/wall-towers/wall-floating.png)

*浮动墙。*

解决这个问题的一个简单方法是将墙壁和塔楼的基础延伸到地下。为此，请在 `HexMetrics` 中为墙添加 Y 偏移。一个单位就足够了。将墙的高度增加相同的量。

```c#
	public const float wallHeight = 4f;
						
	public const float wallYOffset = -1f;
```

调整 `HexMetrics.WallLerp`，因此在确定 Y 坐标时会考虑新的偏移。

```c#
		public static Vector3 WallLerp (Vector3 near, Vector3 far) {
		near.x += (far.x - near.x) * 0.5f;
		near.z += (far.z - near.z) * 0.5f;
		float v =
			near.y < far.y ? wallElevationOffset : (1f - wallElevationOffset);
		near.y += (far.y - near.y) * v + wallYOffset;
		return near;
	}
```

我们还必须调整预制塔，因为它的底座现在将位于地下一个单元。因此，将基础立方体的高度增加一个单位，并相应地调整立方体的局部位置。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-11/wall-towers/towers-grounded.png) ![img](https://catlikecoding.com/unity/tutorials/hex-map/part-11/wall-towers/wall-grounded.png)

*接地的墙壁和塔楼。*

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-11/wall-towers/wall-towers.unitypackage)

## 2 桥梁

到目前为止，我们有河流和道路，但道路无法过河。现在是我们增设桥梁的时候了。

从一个简单的缩放立方体开始，扮演桥梁预制件的角色。我们河流的宽度各不相同，但两侧道路中心之间大约有七个单位。因此，将其比例设置为类似于（3,1,7）的值。给它红色的城市材料，扔掉它的碰撞体。像墙塔一样，将立方体放在一个具有均匀比例的根对象内。这样，桥的实际几何形状就无关紧要了。

将桥预制件引用添加到 `HexFeatureManager`，并将预制件指定给它。

```c#
	public Transform wallTower, bridge;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-11/bridges/inspector.png)

*指定的预制桥梁。*

### 2.1 架设桥梁

要放置一个桥，我们需要一个 `HexFeatureManager.AddBridge` 方法。这座桥应该建在河流两侧的道路中心之间。

```c#
	public void AddBridge (Vector3 roadCenter1, Vector3 roadCenter2) {
		Transform instance = Instantiate(bridge);
		instance.localPosition = (roadCenter1 + roadCenter2) * 0.5f;
		instance.SetParent(container, false);
	}
```

我们将沿着不受干扰的道路中心经过，所以在放置桥梁之前，我们必须先扰乱它们。

```c#
		roadCenter1 = HexMetrics.Perturb(roadCenter1);
		roadCenter2 = HexMetrics.Perturb(roadCenter2);
		Transform instance = Instantiate(bridge);
```

为了正确对齐桥梁，我们可以使用与旋转墙塔相同的方法。在这种情况下，道路中心定义了桥梁的前向矢量。当我们呆在一个单元格内时，这个向量保证是水平的，所以我们不必将它的 Y 分量设置为零。

```c#
		Transform instance = Instantiate(bridge);
		instance.localPosition = (roadCenter1 + roadCenter2) * 0.5f;
		instance.forward = roadCenter2 - roadCenter1;
		instance.SetParent(container, false);
```

### 2.2 跨越笔直的河流

唯一需要桥梁的河流形态是直的和弯曲的。道路可以绕过终点，而曲折的道路只能在其一侧有道路。

让我们先处理直河。 `HexGridChunk.TriangulateRoadAdjacentToRiver` 内部，如果情况属实，则首先考虑在这些河流附近铺设道路。我们将在这里加大桥。

我们在河的一边。道路中心被拉离河流，然后单元格中心本身也被移动。为了找到对侧的道路中心，我们必须向相反方向拉动相同的量。这必须在修改中心本身之前完成。

```c#
	void TriangulateRoadAdjacentToRiver (
		HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e
	) {
		…
		else if (cell.IncomingRiver == cell.OutgoingRiver.Opposite()) {
			…
			roadCenter += corner * 0.5f;
			features.AddBridge(roadCenter, center - corner * 0.5f);
			center += corner * 0.25f;
		}
		…
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-11/bridges/straight-bridges.png)

*横跨笔直河流的桥梁。*

桥出现了！但是我们现在为每个没有河流流过的方向得到一个网桥实例。我们必须确保每个单元格只生成一个网桥示例。这可以通过选择相对于河流的一个方向来实现，从中生成一座桥。哪一个无关紧要。

```c#
			roadCenter += corner * 0.5f;
			if (cell.IncomingRiver == direction.Next()) {
				features.AddBridge(roadCenter, center - corner * 0.5f);
			}
			center += corner * 0.25f;
```

此外，只有在河两岸都有道路的情况下，我们才应该加一座桥。到目前为止，我们已经确定在这一边有一条路。所以我们必须检查另一边是否也有路。

```c#
			if (cell.IncomingRiver == direction.Next() && (
				cell.HasRoadThroughEdge(direction.Next2()) ||
				cell.HasRoadThroughEdge(direction.Opposite())
			)) {
				features.AddBridge(roadCenter, center - corner * 0.5f);
			}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-11/bridges/straight-bridges-roads.png)

*两侧道路之间的桥梁。*

### 2.3 连接弯曲的河流

横跨弯曲河流的桥梁工作方式相似，但拓扑结构略有不同。当我们在弯道外侧时，我们会加上这座桥。在最后一个 `else` 块中就是这种情况。在那里，中间方向用于偏移道路中心。我们需要使用这个偏移量两次，使用不同的比例，因此将其存储在变量中。

```c#
	void TriangulateRoadAdjacentToRiver (
		HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e
	) {
		…
		else {
			HexDirection middle;
			if (previousHasRiver) {
				middle = direction.Next();
			}
			else if (nextHasRiver) {
				middle = direction.Previous();
			}
			else {
				middle = direction;
			}
			if (
				!cell.HasRoadThroughEdge(middle) &&
				!cell.HasRoadThroughEdge(middle.Previous()) &&
				!cell.HasRoadThroughEdge(middle.Next())
			) {
				return;
			}
			Vector3 offset = HexMetrics.GetSolidEdgeMiddle(middle);
			roadCenter += offset * 0.25f;
		}

		…
	}
```

曲线外侧的偏移比例为 0.25，但内侧为 `HexMetrics.innerToOuter * 0.7f`。用这个来放置桥。

```c#
			Vector3 offset = HexMetrics.GetSolidEdgeMiddle(middle);
			roadCenter += offset * 0.25f;
			features.AddBridge(
				roadCenter,
				center - offset * (HexMetrics.innerToOuter * 0.7f)
			);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-11/bridges/curving-bridges.png)

*横跨弯曲河流的桥梁。*

我们必须再次防止重复的桥梁。我们可以通过从中间方向添加一座桥来实现这一点。

```c#
			Vector3 offset = HexMetrics.GetSolidEdgeMiddle(middle);
			roadCenter += offset * 0.25f;
			if (direction == middle) {
				features.AddBridge(
					roadCenter,
					center - offset * (HexMetrics.innerToOuter * 0.7f)
				);
			}
```

同样，我们必须确保对面也有一条路。

```c#
			if (
				direction == middle &&
				cell.HasRoadThroughEdge(direction.Opposite())
			) {
				features.AddBridge(
					roadCenter,
					center - offset * (HexMetrics.innerToOuter * 0.7f)
				);
			}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-11/bridges/curving-bridges-roads.png)

*两侧道路之间的桥梁。*

### 2.4 缩放桥梁

因为我们扰乱了地形，河流两侧道路中心之间的距离会有所不同。有时我们的桥太短，有时又太长。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-11/bridges/varying-river-width.png)

*不同的距离，但桥梁长度不变。*

尽管我们设计的桥梁长度为七个单位，但我们可以根据道路中心之间的实际距离来调整桥梁的规模。这意味着桥梁模型将变形。由于距离变化不大，这种变形可能比不匹配的桥梁更可接受。

为了进行正确的缩放，我们需要知道预制桥梁的设计长度。将此长度存储在 `HexMetrics` 中。

```c#
	public const float bridgeDesignLength = 7f;
```

现在，我们可以将桥梁实例的 Z 比例设置为道路中心之间的距离除以设计长度。由于桥梁预制件的根部具有均匀的比例，因此桥梁将被正确地拉伸。

```c#
	public void AddBridge (Vector3 roadCenter1, Vector3 roadCenter2) {
		roadCenter1 = HexMetrics.Perturb(roadCenter1);
		roadCenter2 = HexMetrics.Perturb(roadCenter2);
		Transform instance = Instantiate(bridge);
		instance.localPosition = (roadCenter1 + roadCenter2) * 0.5f;
		instance.forward = roadCenter2 - roadCenter1;
		float length = Vector3.Distance(roadCenter1, roadCenter2);
		instance.localScale = new Vector3(
			1f,	1f, length * (1f / HexMetrics.bridgeDesignLength)
		);
		instance.SetParent(container, false);
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-11/bridges/varying-bridge-length.png)

*不同的桥梁长度。*

### 2.5 桥梁设计

您可以使用更有趣的桥梁模型来代替单个立方体。例如，您可以使用三个缩放和旋转的立方体创建一个原始拱桥。当然，你可以创建更花哨的 3D 模型，包括道路碎片。但请记住，整个事情都会被挤压和拉伸一点。

![prefab](https://catlikecoding.com/unity/tutorials/hex-map/part-11/bridges/prefab.png) ![scene](https://catlikecoding.com/unity/tutorials/hex-map/part-11/bridges/arc-bridges.png)

*各种长度的拱桥。*

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-11/bridges/bridges.unitypackage)

## 3 特殊特征

到目前为止，我们的单元格可以包含城市、农场和植物特征。即使它们每个都有三个级别，与单元格的大小相比，这些特征都相对较小。如果我们想添加一个大型结构，比如城堡，该怎么办？

让我们在地形中添加一个特殊的特征类型。这些特征如此之大，以至于它们占据了整个单元格。这些功能中的每一个都是独一无二的，需要自己的预制件。例如，一个简单的城堡可以用一个大的中央立方体和四个角塔来建造。中心立方体的比例为（6,4,6），会产生一个相当大的城堡，它仍然适合高度变形的单元格。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-11/special-features/castle.png)

*城堡预制件。*

另一个特殊功能可能是金字塔，例如由三个堆叠的立方体制成。（8, 2.5, 8）是底部立方体的良好比例。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-11/special-features/ziggurat.png)

*金字塔预制件。*

特殊功能可以是任何东西，它们不必局限于架构。例如，一组高达 10 个单位的大树可以代表一个充满巨型植物群的单元格。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-11/special-features/megaflora.png)

*Megaflora 预制件。*

向 `HexFeatureManager` 添加一个数组以跟踪这些预制件。

```c#
	public Transform[] special;
```

首先将城堡添加到阵列中，然后是金字塔，然后是巨型植物。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-11/special-features/inspector.png)

*配置特殊特征。*

### 3.1 让单元格变得特别

`HexCell` 现在需要一个特殊的索引，以确定它具有的特殊功能（如果有的话）。

```c#
	int specialIndex;
```

与其他特征一样，给它一个属性来获取和设置此值。

```c#
	public int SpecialIndex {
		get {
			return specialIndex;
		}
		set {
			if (specialIndex != value) {
				specialIndex = value;
				RefreshSelfOnly();
			}
		}
	}
```

默认情况下，单元格不包含特殊特征。我们将用索引零来表示这一点。添加一个属性，该属性使用此方法来确定单元格是否特殊。

```c#
	public bool IsSpecial {
		get {
			return specialIndex > 0;
		}
	}
```

要编辑单元格，请在 `HexMapEditor` 中添加对特殊索引的支持。它的工作原理与城市、农场和植物特征的水平相同。

```c#
	int activeUrbanLevel, activeFarmLevel, activePlantLevel, activeSpecialIndex;

	…

	bool applyUrbanLevel, applyFarmLevel, applyPlantLevel, applySpecialIndex;
	
	…
	
	public void SetApplySpecialIndex (bool toggle) {
		applySpecialIndex = toggle;
	}

	public void SetSpecialIndex (float index) {
		activeSpecialIndex = (int)index;
	}
	
	…
	
	void EditCell (HexCell cell) {
		if (cell) {
			if (applyColor) {
				cell.Color = activeColor;
			}
			if (applyElevation) {
				cell.Elevation = activeElevation;
			}
			if (applyWaterLevel) {
				cell.WaterLevel = activeWaterLevel;
			}
			if (applySpecialIndex) {
				cell.SpecialIndex = activeSpecialIndex;
			}
			if (applyUrbanLevel) {
				cell.UrbanLevel = activeUrbanLevel;
			}
			…
		}
	}
```

在 UI 中添加滑块以控制特殊功能。由于我们有三个特征，请将滑块的范围设置为 0 - 3。零意味着没有特征，一是城堡，两是金字塔，三是巨型植物。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-11/special-features/ui.png)

*特殊滑块。*

### 3.2 添加特殊特征

我们现在可以为单元格分配特殊特征。为了使它们显示出来，我们必须向 `HexFeatureManager` 添加另一种方法。它只是实例化所需的特殊特征并将其放置在所需的位置。因为零是为没有特征保留的，所以在访问预制数组之前，我们必须从单元格的特殊索引中减去一。

```c#
	public void AddSpecialFeature (HexCell cell, Vector3 position) {
		Transform instance = Instantiate(special[cell.SpecialIndex - 1]);
		instance.localPosition = HexMetrics.Perturb(position);
		instance.SetParent(container, false);
	}
```

使用哈希网格为特征提供任意方向。

```c#
	public void AddSpecialFeature (HexCell cell, Vector3 position) {
		Transform instance = Instantiate(special[cell.SpecialIndex - 1]);
		instance.localPosition = HexMetrics.Perturb(position);
		HexHash hash = HexMetrics.SampleHashGrid(position);
		instance.localRotation = Quaternion.Euler(0f, 360f * hash.e, 0f);
		instance.SetParent(container, false);
	}
```

在 `HexGridChunk.Triangulate` 中对单元格进行三角剖分时，检查单元格是否具有特殊特征。如果是这样，调用我们的新方法，就像 `AddFeature` 一样。

```c#
	void Triangulate (HexCell cell) {
		for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
			Triangulate(d, cell);
		}
		if (!cell.IsUnderwater && !cell.HasRiver && !cell.HasRoads) {
			features.AddFeature(cell, cell.Position);
		}
		if (cell.IsSpecial) {
			features.AddSpecialFeature(cell, cell.Position);
		}
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-11/special-features/features.png)

*特殊特征，比常规特征大得多。*

### 3.3 避开河流

因为特殊特征位于单元格的中心，所以它们不能很好地与河流结合。它们最终漂浮在它们上面。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-11/special-features/on-rivers.png)

*河流上的特征。*

为了防止特殊特征被放置在河流顶部，请调整 `HexCell.SpecialIndex` 属性。仅当单元格没有河流时才更改索引。

```c#
	public int SpecialIndex {
		…
		set {
			if (specialIndex != value && !HasRiver) {
				specialIndex = value;
				RefreshSelfOnly();
			}
		}
	}
```

此外，在添加河流时，我们必须去掉任何特殊特征。河水会把他们冲走。这可以通过在 `HexCell.SetOutgoingRiver` 方法中将特殊索引设置为零来实现。

```c#
	public void SetOutgoingRiver (HexDirection direction) {
		…
		hasOutgoingRiver = true;
		outgoingRiver = direction;
		specialIndex = 0;

		neighbor.RemoveIncomingRiver();
		neighbor.hasIncomingRiver = true;
		neighbor.incomingRiver = direction.Opposite();
		neighbor.specialIndex = 0;

		SetRoad((int)direction, false);
	}
```

### 3.4 避开道路

就像河流一样，道路也不能很好地与特色相结合，尽管它没有那么糟糕。你可能会决定让道路保持原样。也许有些功能可以用于道路，而另一些则不能。所以你可以让它取决于功能。但我们会保持简单。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-11/special-features/on-roads.png)

*道路上的特征。*

在这种情况下，让我们让特征击败道路。因此，在调整特殊索引时，也要从单元格中删除所有道路。

```c#
	public int SpecialIndex {
		…
		set {
			if (specialIndex != value && !HasRiver) {
				specialIndex = value;
				RemoveRoads();
				RefreshSelfOnly();
			}
		}
	}
```

> **如果我们删除这个特殊特征呢？**
>
> 如果我们将索引设置为零，这意味着该单元格已经具有特殊特征。因此，它没有道路。所以我们不需要使用不同的方法。

这也意味着，在尝试添加道路时，我们必须进行额外的检查。仅当两个单元格都不特殊时才添加道路。

```c#
	public void AddRoad (HexDirection direction) {
		if (
			!roads[(int)direction] && !HasRiverThroughEdge(direction) &&
			!IsSpecial && !GetNeighbor(direction).IsSpecial &&
			GetElevationDifference(direction) <= 1
		) {
			SetRoad((int)direction, true);
		}
	}
```

### 3.5 避免其他特征

特殊特征也不会与其他特征类型混合。让它们重叠可能会变得相当混乱。同样，这可能因特殊特征而异，但我们将使用统一的方法。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-11/special-features/on-features.png)

*特征与其他特征相交。*

在这种情况下，让我们抑制次要特征，比如当它们最终沉入水下时。这一次，让我们在 `HexFeatureManager.AddFeature` 中执行检查。

```c#
	public void AddFeature (HexCell cell, Vector3 position) {
		if (cell.IsSpecial) {
			return;
		}

		…
	}
```

### 3.6 避免水

最后，还有水的问题。特殊功能能在水下生存吗？当我们抑制浸没单元中的次要特征时，让我们对特殊特征也做同样的处理。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-11/special-features/in-water.png)

*水中的特征。*

在 `HexGridChunk.Triangulate` 中，对常规和特殊特征进行相同的水下检查。

```c#
	void Triangulate (HexCell cell) {
		for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
			Triangulate(d, cell);
		}
		if (!cell.IsUnderwater && !cell.HasRiver && !cell.HasRoads) {
			features.AddFeature(cell, cell.Position);
		}
		if (!cell.IsUnderwater && cell.IsSpecial) {
			features.AddSpecialFeature(cell, cell.Position);
		}
	}
```

由于这两种 `if` 语句现在都检查单元格是否在水下，我们可以提取它，并只执行一次。

```c#
	void Triangulate (HexCell cell) {
		for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
			Triangulate(d, cell);
		}
		if (!cell.IsUnderwater) {
			if (!cell.HasRiver && !cell.HasRoads) {
				features.AddFeature(cell, cell.Position);
			}
			if (cell.IsSpecial) {
				features.AddSpecialFeature(cell, cell.Position);
			}
		}
	}
```

这应该有足够的特征类型可以使用。下一个教程是[保存和加载](https://catlikecoding.com/unity/tutorials/hex-map/part-12/)。

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-11/special-features/special-features.unitypackage)

[PDF](https://catlikecoding.com/unity/tutorials/hex-map/part-11/Hex-Map-11.pdf)

# Hex Map 12：保存和加载

发布于 2016-12-22

https://catlikecoding.com/unity/tutorials/hex-map/part-12/

*跟踪地形类型，而不是颜色。*
*创建一个文件。*
*将数据写入文件，然后读取。*
*序列化单元格数据。*
*减小文件大小。*

这是关于[六边形地图](https://catlikecoding.com/unity/tutorials/hex-map/)的系列教程的第 12 部分。到目前为止，我们可以创建相当有趣的地图。是时候保存他们了。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-12/tutorial-image.jpg)

*从 [test.map](https://catlikecoding.com/unity/tutorials/hex-map/part-12/test.map) 文件加载。*

## 1 地形类型

保存地图时，我们不需要存储在运行时跟踪的所有数据。例如，我们只需要记住单元格的标高。它的实际垂直位置是从中得出的，因此不需要存储。事实上，我们最好不要存储这些导出的指标。这样，即使稍后我们决定调整高程偏移，地图数据仍然有效。数据与展示分开。

同样，单元格的确切颜色也不需要存储。记住单元格是绿色的是可以的。但如果我们调整视觉风格，绿色的确切色调可能会改变。为此，我们可以存储颜色索引，而不是颜色本身。事实上，我们也可以在运行时将此索引存储在单元格中，而不是实际的颜色中。这使我们能够在以后升级到更高级的地形可视化。

### 1.1 移动颜色数组

如果单元格不再有颜色数据，那么它应该在其他地方可用。最方便的地方是 `HexMetrics`。那么，让我们为其添加一个颜色数组。

```c#
	public static Color[] colors;
```

与噪声等其他全局数据一样，我们可以通过 `HexGrid` 初始化这些颜色。

```c#
	public Color[] colors;
	
	…
	
	void Awake () {
		HexMetrics.noiseSource = noiseSource;
		HexMetrics.InitializeHashGrid(seed);
		HexMetrics.colors = colors;

		…
	}

	…

	void OnEnable () {
		if (!HexMetrics.noiseSource) {
			HexMetrics.noiseSource = noiseSource;
			HexMetrics.InitializeHashGrid(seed);
			HexMetrics.colors = colors;
		}
	}
```

由于我们将不再直接为单元格分配颜色，请删除默认颜色。

```c#
//	public Color defaultColor = Color.white;
	
	…
					
	void CreateCell (int x, int z, int i) {
		…

		HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
		cell.transform.localPosition = position;
		cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
//		cell.Color = defaultColor;

		…
	}
```

配置新颜色，使其与六边形地图编辑器的公共数组相匹配。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-12/terrain-type/colors.png)

*添加到网格中的颜色。*

### 1.2 重构单元格

从 `HexCell` 中删除颜色字段。相反，我们将存储一个索引。我们将使用更通用的地形类型索引，而不是颜色索引。

```c#
//	Color color;
	int terrainTypeIndex;
```

color 属性可以使用此索引检索适当的颜色。它不能再直接设置，因此请删除该部分。这将产生一个编译错误，我们很快就会修复。

```c#
	public Color Color {
		get {
			return HexMetrics.colors[terrainTypeIndex];
		}
//		set {
//			…
//		}
	}
```

添加新属性以获取和设置新的地形类型索引。

```c#
	public int TerrainTypeIndex {
		get {
			return terrainTypeIndex;
		}
		set {
			if (terrainTypeIndex != value) {
				terrainTypeIndex = value;
				Refresh();
			}
		}
	}
```

### 1.3 重构编辑器

在 HexMapEditor 中，删除所有处理颜色的代码。这也将修复编译错误。

```c#
//	public Color[] colors;

	…

//	Color activeColor;

	…

//	bool applyColor;

…

//	public void SelectColor (int index) {
//		applyColor = index >= 0;
//		if (applyColor) {
//			activeColor = colors[index];
//		}
//	}

…

//	void Awake () {
//		SelectColor(0);
//	}
	
	…
	
	void EditCell (HexCell cell) {
		if (cell) {
//			if (applyColor) {
//				cell.Color = activeColor;
//			}
			…
		}
	}
```

现在添加一个字段和方法来控制活动地形类型索引。

```c#
	int activeTerrainTypeIndex;
	
	…
	
	public void SetTerrainTypeIndex (int index) {
		activeTerrainTypeIndex = index;
	}
```

使用此方法替换现在缺少的 `SelectColor` 方法。将 UI 中的颜色小部件连接到 `SetTerrainTypeIndex`，保持其他一切不变。这意味着仍然使用负指数来表示不应更改颜色。

调整 `EditCell`，以便在适当的时候将地形类型索引指定给编辑的单元格。

```c#
	void EditCell (HexCell cell) {
		if (cell) {
			if (activeTerrainTypeIndex >= 0) {
				cell.TerrainTypeIndex = activeTerrainTypeIndex;
			}
			…
		}
	}
```

尽管我们已经从单元格中删除了颜色数据，但地图仍应像以前一样工作。唯一的区别是，默认颜色现在是数组中的第一个颜色，在我的例子中是黄色。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-12/terrain-type/new-default-color.png)

*黄色是新的默认值。*

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-12/terrain-type/terrain-type.unitypackage)

## 2 在文件中存储数据

我们将使用 `HexMapEditor` 来控制地图的保存和加载。创建两个方法来处理这个问题，暂时将它们留空。

```c#
	public void Save () {
	}

	public void Load () {
	}
```

通过 *GameObject / UI / Button* 向 UI 添加两个按钮。将它们连接到 `Save` 和 `Load` 方法，并为它们赋予适当的标签。我把它们放在右边面板的底部。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-12/storing-data-a-file/save-load-buttons.png)

*保存和加载按钮。*

### 2.1 文件位置

为了保存地图，我们必须把它存储在某个地方。与大多数游戏一样，我们会将数据存储在一个文件中。但是这个文件应该放在玩家的文件系统的哪个位置呢？答案取决于游戏运行在哪个操作系统上。每个操作系统都有不同的存储应用程序特定文件的约定。

我们不需要知道这些惯例。Unity 知道适当的路径，我们可以通过 `Application.persistentDataPath` 检索。您可以通过在“保存”中登录并在播放模式下单击按钮来检查它对您来说是什么。

```c#
	public void Save () {
		Debug.Log(Application.persistentDataPath);
	}
```

在桌面上，路径将包含公司和产品名称。编辑器和构建都使用此路径。您可以通过*编辑 / 项目设置 / Player* 调整名称。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-12/storing-data-a-file/player-settings.png)

*公司和产品名称。*

> **为什么我在 Mac 上找不到 Library 文件夹？**
>
> 库文件夹通常是隐藏的。如何使其可见取决于 OS X 版本。除非您有旧版本，否则请在 Finder 中选择您的主文件夹，然后转到“*显示视图选项*”。*库*文件夹有一个复选框。

> **WebGL 怎么样？**
>
> WebGL 游戏无法访问用户的文件系统。相反，所有文件操作都将被定向到内存中的文件系统。这对我们来说是透明的。但是，要持久化数据，您必须手动指示网页将数据刷新到浏览器的存储中。

### 2.2 创建文件

要创建文件，我们必须使用 `System.IO` 命名空间中的类。因此，在 `HexMapEditor` 类上方添加一个 `using` 语句。

```c#
using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;

public class HexMapEditor : MonoBehaviour {
	…
}
```

首先，我们需要创建文件的完整路径。我们将使用 `test.map` 作为文件名。它应该附加到持久数据路径上。我们是否必须在它们之间加上斜线或反斜线取决于平台。 `Path.Combine` 方法将为我们解决这个问题。

```c#
	public void Save () {
		string path = Path.Combine(Application.persistentDataPath, "test.map");
	}
```

接下来，我们必须访问此位置的文件。我们用文件来做这件事。开放式方法。因为我们想将数据写入此文件，所以必须使用其创建模式。这将在提供的路径上创建一个新文件，或者替换已经存在的文件。

```c#
		string path = Path.Combine(Application.persistentDataPath, "test.map");
		File.Open(path, FileMode.Create);
```

调用此方法的结果是一个链接到此文件的开放数据流。我们可以使用它将数据写入文件。一旦我们不再需要它，我们必须确保关闭流。

```c#
		string path = Path.Combine(Application.persistentDataPath, "test.map");
		Stream fileStream = File.Open(path, FileMode.Create);
		fileStream.Close();
```

此时，按“*保存*”按钮将在持久数据路径指定的文件夹中创建一个 *test.map* 文件。如果你检查这个文件，你会发现它是空的，大小为零字节。那是因为我们还没有写任何东西。

### 2.3 写入文件

要实际将数据放入文件中，我们需要一种将数据流式传输到文件的方法。最基本的方法是使用 `BinaryWriter`。这些对象允许我们将原始数据写入任何流。

使用我们的文件流作为参数创建一个新的 `BinaryWriter` 对象。关闭 writer 也将关闭它使用的流。因此，我们不再需要直接引用流。

```c#
		string path = Path.Combine(Application.persistentDataPath, "test.map");
		BinaryWriter writer =
			new BinaryWriter(File.Open(path, FileMode.Create));
		writer.Close();
```

我们可以使用 `BinaryWriter.Write` 方法将数据发送到流中。所有原始类型（如整数和浮点数）都有一个 `Write` 方法变体。它也可以写字符串。让我们试着写出整数 123。

```c#
		BinaryWriter writer =
			new BinaryWriter(File.Open(path, FileMode.Create));
		writer.Write(123);
		writer.Close();
```

按下“*保存*”按钮，再次检查 *test.map*。它的大小现在是四个字节。这是因为整数的大小是四个字节。

> **为什么我的文件浏览器报告文件占用了更多空间？**
>
> 这是因为文件系统以字节块为单位划分空间。它不跟踪单个字节。由于 *test.map* 目前只有四个字节，它最终占用了一块存储空间。

请注意，我们存储的是二进制数据，而不是人类可读的文本。所以，如果你在文本编辑器中打开我们的文件，你会看到胡言乱语。您可能会看到字符 *{*，后面跟着空字符或一些占位符字符。

您可以使用十六进制编辑器打开该文件。在这种情况下，你会看到 *7b 00 00 00*。这些是我们整数的四个字节，使用十六进制表示法显示。当使用常规十进制数时，这是 *123 0 0 0*。在二进制表示法中，第一个字节是 *01111011*。

*｛* 的 ASCII 码是 123，这就是为什么文本编辑器可能会显示该字符。ASCII 0 是空字符，与有效的可见字符不对应。

其他三个字节为零，因为我们写了一个小于 256 的数字。如果我们写 256，那么十六进制编辑器将显示 *00 01 00 00*。

> ***123* 不应该存储为 *00 00 00 7b* 吗？**
>
> `BinaryWriter` 使用小字节序约定来存储数字。这意味着首先写入最低有效字节。这是 Microsoft 在开发 .Net 框架时使用的惯例，之所以选择它，可能是因为 Intel CPU 本身使用小字节序格式。
>
> 另一种选择是大端序，它首先存储最高有效字节。这对应于我们如何对数字进行排序。123 是 123，因为我们采用大端序表示法。如果是小端序，123 将代表 321。

### 2.4 确保资源得到释放

关闭 writer 是很重要的。只要我们打开它，文件系统就会锁定文件，防止其他进程写入。如果我们忘记关闭它，我们也会把自己锁在外面。如果我们点击保存按钮两次，第二次打开流将失败。

我们可以为它创建一个 `using` 块，而不是手动关闭 writer。这定义了 writer 有效的作用域。一旦代码执行退出此作用域，编写器就会被丢弃，流也会被关闭。

```c#
		using (
			BinaryWriter writer =
				new BinaryWriter(File.Open(path, FileMode.Create))
		) {
			writer.Write(123);
		}
//		writer.Close();
```

这之所以有效，是因为 writer 和文件流类都实现了 `IDisposable` 接口。这些对象有一个 `Dispose` 方法，在退出 `using` 范围时隐式调用该方法。

`using` 的最大优点是，无论执行如何离开范围，它都能正常工作。早期的返回、异常和错误不会破坏它。它也很简洁。

### 2.5 检索数据

为了回读我们之前写的数据，我们必须在 `Load` 方法中放入代码。就像保存时一样，我们必须构造路径并打开文件流。不同的是，这次我们打开文件是为了读取，而不是写入。我们需要一个 `BinaryReader` 而不是一个 writer。

```c#
	public void Load () {
		string path = Path.Combine(Application.persistentDataPath, "test.map");
		using (
			BinaryReader reader =
				new BinaryReader(File.Open(path, FileMode.Open))
		) {
		}
	}
```

在这种情况下，我们可以使用简写方法 `File.OpenRead` 打开文件进行读取。

```c#
		using (BinaryReader reader = new BinaryReader(File.OpenRead(path))) {
		}
```

> **为什么我们写时不能使用 `File.OpenWrite`？**
>
> 该方法创建一个附加到现有文件的流，而不是替换它们。

在阅读时，我们必须明确我们检索的数据类型。要从流中读取整数，我们必须使用 `BinaryReader.ReadInt32`。该方法读取一个 32 位整数，即四个字节。

```c#
		using (BinaryReader reader = new BinaryReader(File.OpenRead(path))) {
			Debug.Log(reader.ReadInt32());
		}
```

请注意，要检索 *123*，只需读取一个字节即可。但这将在流中留下属于整数的三个字节。它也不适用于 0-255 范围之外的数字。所以不要那样做。

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-12/storing-data-a-file/storing-data-in-a-file.unitypackage)

## 3 写入和读取地图数据

在存储任何内容时，一个大问题是是否应该使用人类可读的格式。常见的人类可读格式是 JSON、XML 和具有一些自定义结构的纯 ASCII。这些文件可以在文本编辑器中打开、解析和编辑。它还可以使不同应用程序之间的数据共享更加容易。

然而，这种格式是有代价的。文件大小将比使用二进制数据时更大，有时甚至大得多。在编码和解码数据时，它们还会在执行时间和内存分配方面增加大量开销。

相比之下，二进制数据紧凑且快速。当你写大量数据时，这一点很重要。例如，在每次游戏回合自动保存大地图时。所以我们将坚持二进制。如果你能处理这个问题，你也可以处理更详细的格式。

> **自动序列化怎么样？**
>
> 就像 Unity 序列化数据一样，我们可以直接将可序列化的类写入流中。写入单个字段的细节将对我们隐藏。但是，我们不能直接序列化我们的单元格。它们是 `MonoBaviour` 类，里面有我们不想存储的东西。因此，我们需要使用一个单独的对象层次结构，这破坏了自动序列化的简单性。此外，未来的代码更改更难以这种方式支持。因此，我们坚持手动序列化的完全控制。这也迫使我们真正理解正在发生的事情。

为了序列化我们的地图，我们必须存储每个单元格的数据。要保存和加载单个单元格，请向 `HexCell` 添加 `Save` 和 `Load` 方法。由于他们需要一个作家或读者来完成他们的工作，请将这些作为参数添加。

```c#
using UnityEngine;
using System.IO;

public class HexCell : MonoBehaviour {
	
	…
	
	public void Save (BinaryWriter writer) {
	}

	public void Load (BinaryReader reader) {
	}
}
```

还将 `Save` 和 `Load` 方法添加到 `HexGrid` 中。这些方法只是迭代所有单元格，调用它们的 `Load` 和 `Save` 方法。

```c#
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class HexGrid : MonoBehaviour {

	…

	public void Save (BinaryWriter writer) {
		for (int i = 0; i < cells.Length; i++) {
			cells[i].Save(writer);
		}
	}

	public void Load (BinaryReader reader) {
		for (int i = 0; i < cells.Length; i++) {
			cells[i].Load(reader);
		}
	}
}
```

如果我们加载地图，则必须在单元格数据更改后刷新地图。为此，只需刷新所有块。

```c#
	public void Load (BinaryReader reader) {
		for (int i = 0; i < cells.Length; i++) {
			cells[i].Load(reader);
		}
		for (int i = 0; i < chunks.Length; i++) {
			chunks[i].Refresh();
		}
	}
```

最后，将 `HexMapEditor` 中的测试代码替换为网格的 `Save` 和 `Load` 方法的调用，并传递给编写器或读取器。

```c#
	public void Save () {
		string path = Path.Combine(Application.persistentDataPath, "test.map");
		using (
			BinaryWriter writer =
				new BinaryWriter(File.Open(path, FileMode.Create))
		) {
			hexGrid.Save(writer);
		}
	}

	public void Load () {
		string path = Path.Combine(Application.persistentDataPath, "test.map");
		using (BinaryReader reader = new BinaryReader(File.OpenRead(path))) {
			hexGrid.Load(reader);
		}
	}
```

### 3.1 保存地形类型

此时，再次保存会生成一个空文件，加载则什么也不做。让我们慢慢开始，只编写和加载 `HexCell` 的地形类型索引。

直接写入 `terrainTypeIndex` 字段，并直接将其分配好。我们不会使用这些房产。当我们显式刷新所有块时，不需要对属性进行 `Refresh` 调用。此外，由于我们只保存有效的地图，我们将假设我们加载的所有地图也是有效的。例如，我们不需要检查河流或道路是否允许通行。

```c#
	public void Save (BinaryWriter writer) {
		writer.Write(terrainTypeIndex);
	}
	
	public void Load (BinaryReader reader) {
		terrainTypeIndex = reader.ReadInt32();
	}
```

保存时，所有单元格的地形类型索引将一个接一个地写入文件。因为索引是一个整数，所以它的大小是四个字节。我的地图包含 300 个单元格，这意味着文件大小将为 1200 字节。

加载以与写入索引相同的顺序读回索引。如果保存后更改了单元格颜色，加载贴图将使颜色恢复到保存时的状态。因为我们没有存储其他任何东西，所以其他单元格数据保持不变。因此，加载将替换地形类型，但不会替换其高程、水位、特征等。

### 3.2 保存所有整数

保存地形类型索引是不够的。我们还必须存储所有其他单元格数据。让我们从所有整数字段开始。这些是地形类型指数、海拔、水位、城市水平、农场水平、植物水平和特殊指数。一定要按照你写的顺序读回。

```c#
	public void Save (BinaryWriter writer) {
		writer.Write(terrainTypeIndex);
		writer.Write(elevation);
		writer.Write(waterLevel);
		writer.Write(urbanLevel);
		writer.Write(farmLevel);
		writer.Write(plantLevel);
		writer.Write(specialIndex);
	}

	public void Load (BinaryReader reader) {
		terrainTypeIndex = reader.ReadInt32();
		elevation = reader.ReadInt32();
		waterLevel = reader.ReadInt32();
		urbanLevel = reader.ReadInt32();
		farmLevel = reader.ReadInt32();
		plantLevel = reader.ReadInt32();
		specialIndex = reader.ReadInt32();
	}
```

现在尝试保存和加载地图，同时在两者之间进行一些更改。我们包括的一切都按预期恢复了，除了海拔。这是因为更改标高时，单元格的垂直位置也应更新。我们可以通过将加载的标高指定给其属性，而不是其字段来实现这一点。但该属性执行我们不需要的额外工作。因此，让我们从 `Elevation` 设置器中提取更新单元格位置的代码，并将其放入单独的 `RefreshPosition` 方法中。我们对这段代码所做的唯一更改是将值替换为对高程字段的引用。

```c#
	void RefreshPosition () {
		Vector3 position = transform.localPosition;
		position.y = elevation * HexMetrics.elevationStep;
		position.y +=
			(HexMetrics.SampleNoise(position).y * 2f - 1f) *
			HexMetrics.elevationPerturbStrength;
		transform.localPosition = position;

		Vector3 uiPosition = uiRect.localPosition;
		uiPosition.z = -position.y;
		uiRect.localPosition = uiPosition;
	}
```

现在，我们可以在设置属性时调用此方法，也可以在加载高程数据后调用此方法。

```c#
	public int Elevation {
		…
		set {
			if (elevation == value) {
				return;
			}
			elevation = value;
			RefreshPosition();
			ValidateRivers();
			
			…
		}
	}
	
	…
	
	public void Load (BinaryReader reader) {
		terrainTypeIndex = reader.ReadInt32();
		elevation = reader.ReadInt32();
		RefreshPosition();
		…
	}
```

通过此更改，单元格将在加载时正确调整其可见标高。

### 3.3 保存所有数据

单元格是否有围墙，是否有流入或流出的河流，都存储在布尔字段中。我们可以把这些写成整数。除此之外，道路数据是一个由六个布尔值组成的数组，我们可以使用循环来编写。

```c#
	public void Save (BinaryWriter writer) {
		writer.Write(terrainTypeIndex);
		writer.Write(elevation);
		writer.Write(waterLevel);
		writer.Write(urbanLevel);
		writer.Write(farmLevel);
		writer.Write(plantLevel);
		writer.Write(specialIndex);
		writer.Write(walled);

		writer.Write(hasIncomingRiver);
		writer.Write(hasOutgoingRiver);

		for (int i = 0; i < roads.Length; i++) {
			writer.Write(roads[i]);
		}
	}
```

流入和流出河流的方向存储在 `HexDirection` 字段中。`HexDirection` 类型是一个枚举，内部存储为整数。因此，我们也可以使用显式转换将它们序列化为整数。

```c#
		writer.Write(hasIncomingRiver);
		writer.Write((int)incomingRiver);

		writer.Write(hasOutgoingRiver);
		writer.Write((int)outgoingRiver);
```

读取布尔值是用 `BinaryReader.ReadBoolean` 方法完成的。河流方向是整数，我们必须将其转换回 `HexDirection`。

```c#
	public void Load (BinaryReader reader) {
		terrainTypeIndex = reader.ReadInt32();
		elevation = reader.ReadInt32();
		RefreshPosition();
		waterLevel = reader.ReadInt32();
		urbanLevel = reader.ReadInt32();
		farmLevel = reader.ReadInt32();
		plantLevel = reader.ReadInt32();
		specialIndex = reader.ReadInt32();
		walled = reader.ReadBoolean();

		hasIncomingRiver = reader.ReadBoolean();
		incomingRiver = (HexDirection)reader.ReadInt32();

		hasOutgoingRiver = reader.ReadBoolean();
		outgoingRiver = (HexDirection)reader.ReadInt32();

		for (int i = 0; i < roads.Length; i++) {
			roads[i] = reader.ReadBoolean();
		}
	}
```

我们现在正在存储完全保存和恢复地图所需的所有细胞数据。每个单元格需要九个整数和九个布尔值。布尔值每个占用一个字节，所以我们最终每个单元格使用 45 个字节。因此，一个包含 300 个单元格的地图总共需要 13500 个字节。

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-12/writing-and-reading-map-data/writing-and-reading-map-data.unitypackage)

## 4 更小的文件大小

尽管对于 300 个单元来说 13500 个字节似乎并不多，但也许我们可以用更少的字节来凑合。毕竟，我们完全可以控制数据的序列化方式。让我们看看能否找到一种更紧凑的存储方式。

### 4.1 缩小数值范围

我们单元格的各种级别和索引都存储为整数。然而，它们只覆盖了一个很小的值范围。它们肯定都在 0-255 的范围内。这意味着只会使用每个整数的第一个字节。其他三个将始终为零。存储这些空字节是没有意义的。我们可以在写入流之前将整数转换为字节来丢弃它们。

```c#
		writer.Write((byte)terrainTypeIndex);
		writer.Write((byte)elevation);
		writer.Write((byte)waterLevel);
		writer.Write((byte)urbanLevel);
		writer.Write((byte)farmLevel);
		writer.Write((byte)plantLevel);
		writer.Write((byte)specialIndex);
		writer.Write(walled);

		writer.Write(hasIncomingRiver);
		writer.Write((byte)incomingRiver);

		writer.Write(hasOutgoingRiver);
		writer.Write((byte)outgoingRiver);
```

现在我们必须使用 `BinaryReader.ReadByte` 获取我们的数字。从字节到整数的转换是隐式的，因此我们不必添加显式转换。

```c#
		terrainTypeIndex = reader.ReadByte();
		elevation = reader.ReadByte();
		RefreshPosition();
		waterLevel = reader.ReadByte();
		urbanLevel = reader.ReadByte();
		farmLevel = reader.ReadByte();
		plantLevel = reader.ReadByte();
		specialIndex = reader.ReadByte();
		walled = reader.ReadBoolean();

		hasIncomingRiver = reader.ReadBoolean();
		incomingRiver = (HexDirection)reader.ReadByte();

		hasOutgoingRiver = reader.ReadBoolean();
		outgoingRiver = (HexDirection)reader.ReadByte();
```

这消除了每个整数三个字节，从而为每个单元格节省了 27 个字节。我们现在每个单元有 18 个字节，300 个单元总共有 5400 个字节。

请注意，此时旧地图数据已变得毫无意义。加载旧保存时，数据未对齐，您会把单元格弄乱。这是因为我们现在读取的数据比以前少。如果我们读取的数据比以前多，那么在尝试读取文件末尾以外的数据时就会出错。

当我们正在定义格式时，无法处理较旧的保存数据是可以的。但是，一旦我们确定了保存格式，我们就必须确保未来的代码始终能够读取它。即使我们更改了格式，理想情况下我们也应该能够读取旧格式。

### 4.2 合并River字节

此时，我们使用四个字节来存储河流数据，每个方向两个字节。对于一个方向，我们存储是否有河流，以及它流向哪个方向。

很明显，如果河流不存在，我们不必储存它的方向。这意味着没有河流的单元格需要少两个字节。实际上，无论是否存在，我们都可以在每个河流方向上使用一个字节。

有六个可能的方向，它们被存储为 0-5 范围内的数字。这只需要三个比特，因为在二进制中，数字 0 到 5 是 000、001、010、011、100 和 101。这使得单个字节中还有五个比特未被使用。我们可以用其中一个来存储河流是否存在。例如，我们可以使用第八位，它对应于数字 128。

为此，在将方向转换为字节之前，先将其添加 128。所以，如果有一条河向西北流，我们会写 133，二进制为 10000101。如果没有河流，我们只需写一个零字节。

这使得我们字节的四位仍然未使用，这很好。我们可以将两个河流方向合并到一个字节中，但这变得相当复杂。

```c#
//		writer.Write(hasIncomingRiver);
//		writer.Write((byte)incomingRiver);
		if (hasIncomingRiver) {
			writer.Write((byte)(incomingRiver + 128));
		}
		else {
			writer.Write((byte)0);
		}

//		writer.Write(hasOutgoingRiver);
//		writer.Write((byte)outgoingRiver);
		if (hasOutgoingRiver) {
			writer.Write((byte)(outgoingRiver + 128));
		}
		else {
			writer.Write((byte)0);
		}
```

为了解码河流数据，我们必须首先读回字节。如果它的值至少为 128，则意味着有一条河。要获得其方向，请在将其转换为 `HexDirection` 之前减去 128。

```c#
//		hasIncomingRiver = reader.ReadBoolean();
//		incomingRiver = (HexDirection)reader.ReadByte();
		byte riverData = reader.ReadByte();
		if (riverData >= 128) {
			hasIncomingRiver = true;
			incomingRiver = (HexDirection)(riverData - 128);
		}
		else {
			hasIncomingRiver = false;
		}

//		hasOutgoingRiver = reader.ReadBoolean();
//		outgoingRiver = (HexDirection)reader.ReadByte();
		riverData = reader.ReadByte();
		if (riverData >= 128) {
			hasOutgoingRiver = true;
			outgoingRiver = (HexDirection)(riverData - 128);
		}
		else {
			hasOutgoingRiver = false;
		}
```

这使我们每个单元格的字节数减少到 16 个。这可能不是一个很大的改进，但它是用来减小二进制数据大小的技巧之一。

### 4.3 以单个字节存储道路

我们可以使用类似的技巧来压缩我们的道路数据。我们有六个布尔值，可以存储在字节的前六位。因此，每个道路方向都由一个 2 的幂表示。它们是 1、2、4、8、16 和 32，在二进制中是 1、10、100、1000、10000 和 100000。

为了创建最后一个字节，我们必须设置与正在使用的道路方向相对应的位。我们可以使用 `<<` 运算符来获得方向的正确值。然后我们使用位 OR 运算符将它们合并在一起。例如，如果第一、第二、第三和第六条道路正在使用中，则最后一个字节将是 100111。

```c#
		int roadFlags = 0;
		for (int i = 0; i < roads.Length; i++) {
//			writer.Write(roads[i]);
			if (roads[i]) {
				roadFlags |= 1 << i;
			}
		}
		writer.Write((byte)roadFlags);
```

> **`<<` 是如何工作的？**
>
> 这是位左移运算符。它在左边取一个整数，并将其所有位向左移动。溢出被丢弃。它移动了多少步由右边的整数控制。因为数字是二进制的，向左移动所有位一步会使数字的值加倍。所以 `1 << n` 产生 2^n^，这正是我们想要的。

要获取道路的布尔值，我们必须检查其位是否已设置。为此，使用具有相应数字的位 AND 运算符屏蔽所有其他位。如果结果不为零，则设置位并存在道路。

```c#
		int roadFlags = reader.ReadByte();
		for (int i = 0; i < roads.Length; i++) {
			roads[i] = (roadFlags & (1 << i)) != 0;
		}
```

将六个字节压缩为一个字节后，我们现在每个单元格的字节数达到了 11 个。对于 300 个单元格，这只是 3300 字节。因此，随意处理一些字节使我们的文件大小减少了 75%。

### 4.4 为未来做准备

在我们声明保存格式完成之前，让我们添加一个小细节。在保存地图数据之前，让 `HexMapEditor` 写入整数零。

```c#
	public void Save () {
		string path = Path.Combine(Application.persistentDataPath, "test.map");
		using (
			BinaryWriter writer =
				new BinaryWriter(File.Open(path, FileMode.Create))
		) {
			writer.Write(0);
			hexGrid.Save(writer);
		}
	}
```

这将在我们的数据前添加四个空字节。因此，在加载地图之前，我们必须读取这四个字节。

```c#
	public void Load () {
		string path = Path.Combine(Application.persistentDataPath, "test.map");
		using (BinaryReader reader = new BinaryReader(File.OpenRead(path))) {
			reader.ReadInt32();
			hexGrid.Load(reader);
		}
	}
```

虽然这些字节目前毫无用处，但它们充当了一个标头，使未来的向后兼容性成为可能。如果我们没有添加这些零字节，前几个字节的内容将取决于地图的第一个单元格。这将使我们更难确定未来要处理的保存格式版本。现在我们可以简单地检查前四个字节。如果它们是空的，我们处理的是格式版本0。这取决于未来的版本是否会在其中添加其他内容。

因此，如果标头不为零，则我们正在处理某个未知版本。由于我们无法知道那里有什么数据，我们应该拒绝加载地图。

```c#
		using (BinaryReader reader = new BinaryReader(File.OpenRead(path))) {
			int header = reader.ReadInt32();
			if (header == 0) {
				hexGrid.Load(reader);
			}
			else {
				Debug.LogWarning("Unknown map format " + header);
			}
		}
```

下一个教程是[管理地图](https://catlikecoding.com/unity/tutorials/hex-map/part-13/)。

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-12/smaller-file-size/smaller-file-size.unitypackage)

[PDF](https://catlikecoding.com/unity/tutorials/hex-map/part-12/Hex-Map-12.pdf)

# Hex Map 13：管理地图

更新于 2020-08-24 发布于 2017-01-27

https://catlikecoding.com/unity/tutorials/hex-map/part-13/

*在游戏模式下创建新地图。*
*支持多种地图尺寸。*
*添加地图大小以保存数据。*
*保存并加载任意地图。*
*显示地图列表。*

这是关于[六边形地图](https://catlikecoding.com/unity/tutorials/hex-map/)的系列教程的第 13 部分。上一期使保存和加载地图数据成为可能。这一次，我们将添加对多种地图大小的支持，以及保存到不同的文件。

从现在开始，本教程系列将使用 Unity 5.5.0 制作。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-13/tutorial-image.jpg)

*地图库的[开始](https://catlikecoding.com/unity/tutorials/hex-map/part-13/large-island.map)。*

## 1 创建新地图

到目前为止，我们创建十六进制网格的唯一时间是加载场景时。现在，我们将使随时启动新地图成为可能。新地图将简单地替换当前地图。

当 `HexGrid` 唤醒时，它会初始化一些指标，然后计算出单元格计数并创建所需的块和单元格。通过创建一组新的块和单元格，我们创建了一个新的地图。因此，让我们将 `HexGrid.Awake` 分为两部分，原始初始化代码和公共 `CreateMap` 方法。

```c#
	void Awake () {
		HexMetrics.noiseSource = noiseSource;
		HexMetrics.InitializeHashGrid(seed);
		HexMetrics.colors = colors;
		CreateMap();
	}

	public void CreateMap () {
		cellCountX = chunkCountX * HexMetrics.chunkSizeX;
		cellCountZ = chunkCountZ * HexMetrics.chunkSizeZ;
		CreateChunks();
		CreateCells();
	}
```

在我们的 UI 中添加一个按钮来创建新地图。我把它做成一个大按钮，放在保存和加载按钮下面。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-13/creating-new-maps/new-map-button.png)

*新建地图按钮。*

将此按钮的*单击*事件连接到 `HexGrid` 对象的 `CreateMap` 方法。因此，我们不使用 *Hex Map Editor*，我们将直接调用 *Hex Grid* 对象的方法。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-13/creating-new-maps/on-click.png)

*单击即可创建地图。*

### 1.1 清除旧数据

单击“新建地图”按钮现在将创建一组新的块和单元格。但是，旧的不会自动删除。因此，我们最终得到了多个叠加的地图网格。为了防止这种情况，我们必须先处理掉旧物品。这可以通过在 `CreateMap` 开始时销毁所有当前块来实现。

```c#
	public void CreateMap () {
		if (chunks != null) {
			for (int i = 0; i < chunks.Length; i++) {
				Destroy(chunks[i].gameObject);
			}
		}

		…
	}
```

> **我们不能重用现有的对象吗？**
>
> 这是可能的，但从新鲜的块和细胞开始是最简单的。当我们支持多种地图尺寸时，尤其如此。此外，创建新地图是一种相对罕见的行为。优化在这里不是很重要。

> **我们能像那样在循环中把孩子们破坏（destroy）吗？**
>
> 当然。实际销毁将延迟到当前帧的更新阶段之后。

### 1.2 单元格而不是块的大小

我们目前通过 `HexGrid` 的 `chunkCountX` 和 `chunkCountZ` 字段设置地图的大小。但是，根据单元格指定地图大小要方便得多。这样，我们甚至可以在以后更改块大小，而不会影响地图的大小。那么，让我们交换一下单元格计数和块计数字段的角色。

```c#
//	public int chunkCountX = 4, chunkCountZ = 3;
	public int cellCountX = 20, cellCountZ = 15;

	…

//	int cellCountX, cellCountZ;
	int chunkCountX, chunkCountZ;

	…
	
	public void CreateMap () {
		…

//		cellCountX = chunkCountX * HexMetrics.chunkSizeX;
//		cellCountZ = chunkCountZ * HexMetrics.chunkSizeZ;
		chunkCountX = cellCountX / HexMetrics.chunkSizeX;
		chunkCountZ = cellCountZ / HexMetrics.chunkSizeZ;
		CreateChunks();
		CreateCells();
	}
```

这将导致编译错误，因为 `HexMapCamera` 使用块大小来限制其位置。调整 `HexMapCamera.ClampPosition` 因此它直接使用单元格计数，而这正是它所需要的。

```c#
	Vector3 ClampPosition (Vector3 position) {
		float xMax = (grid.cellCountX - 0.5f) * (2f * HexMetrics.innerRadius);
		position.x = Mathf.Clamp(position.x, 0f, xMax);

		float zMax = (grid.cellCountZ - 1) * (1.5f * HexMetrics.outerRadius);
		position.z = Mathf.Clamp(position.z, 0f, zMax);

		return position;
	}
```

我们的块大小是 5 乘 5 个单元格，我们使用 4 乘 3 个块作为默认的地图大小。因此，为了保持地图不变，我们必须使用 20 乘 15 个单元格。即使我们在代码中指定了默认值，我们的网格对象也不会自动使用这些值。这是因为这些字段已经存在，并且过去默认值为零。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-13/creating-new-maps/default-size.png)

*默认地图大小设置为 20 乘 15。*

### 1.3 任意地图大小

下一步是支持创建任何大小的地图，而不仅仅是默认大小的地图。为此，请向 `HexGrid.CreateMap` 添加一个 X 和 Z 参数。这些将取代现有的单元格计数。在 `Awake` 中，只需用当前的单元格计数调用它。

```c#
	void Awake () {
		HexMetrics.noiseSource = noiseSource;
		HexMetrics.InitializeHashGrid(seed);
		HexMetrics.colors = colors;
		CreateMap(cellCountX, cellCountZ);
	}

	public void CreateMap (int x, int z) {
		…

		cellCountX = x;
		cellCountZ = z;
		chunkCountX = cellCountX / HexMetrics.chunkSizeX;
		chunkCountZ = cellCountZ / HexMetrics.chunkSizeZ;
		CreateChunks();
		CreateCells();
	}
```

但是，这只适用于块大小的倍数的单元格计数。否则，整数除法将产生太少的块。虽然我们可以添加对仅部分填充单元格的块的支持，但让我们简单地禁止不适合我们块的大小。

我们可以使用 `%` 运算符来计算单元格计数除以块计数的余数。如果这不是零，则存在不匹配，我们不会创建新的地图。当我们这样做的时候，让我们也警惕零和负的大小。

```c#
	public void CreateMap (int x, int z) {
		if (
			x <= 0 || x % HexMetrics.chunkSizeX != 0 ||
			z <= 0 || z % HexMetrics.chunkSizeZ != 0
		) {
			Debug.LogError("Unsupported map size.");
			return;
		}
		
		…
	}
```

### 1.4 新建地图菜单

此时，“*新建地图*”按钮不再工作。这是因为 `HexGrid.CreateMap` 方法现在有两个参数。我们不能直接将 Unity 事件与这些方法联系起来。此外，为了支持多种地图尺寸，我们需要多个按钮。与其将所有这些按钮添加到我们的主 UI 中，不如创建一个单独的弹出菜单。

通过GameObject/UI/canvas向场景添加新画布。使用与现有画布相同的设置，只是其排序顺序应设置为1。这将确保它最终位于主编辑器 UI 之上。我制作了画布和新 UI 对象的事件系统子对象，以保持场景层次结构的整洁。

![canvas](https://catlikecoding.com/unity/tutorials/hex-map/part-13/creating-new-maps/canvas.png) ![hierarchy](https://catlikecoding.com/unity/tutorials/hex-map/part-13/creating-new-maps/ui-hierarchy.png)

*新建地图菜单画布。*

在“*新建地图菜单*”中添加一个覆盖整个屏幕的面板。其目的是在菜单打开时使背景变暗，并阻止光标与其他任何东西交互。通过清除其*源图像*并将其*颜色*设置为（0, 0, 0, 200)，我为其赋予了统一的颜色。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-13/creating-new-maps/background.png)

*背景图像设置。*

将菜单面板添加到画布的中心，就像*六边形地图编辑器*的面板一样。为小、中、大地图赋予描述性标签和按钮。也给它一个取消按钮，以防你改变主意。设计完成后，停用整个“*新建地图菜单*”。

![menu](https://catlikecoding.com/unity/tutorials/hex-map/part-13/creating-new-maps/menu.png) ![hierarchy](https://catlikecoding.com/unity/tutorials/hex-map/part-13/creating-new-maps/menu-hierarchy.png)

*新建地图菜单。*

要控制菜单，请创建一个 `NewMapMenu` 组件并将其添加到 *New Map Menu* 画布对象中。要创建新地图，我们需要访问我们的 *Hex Grid* 对象。所以给它一个公共字段，并将其连接起来。

```c#
using UnityEngine;

public class NewMapMenu : MonoBehaviour {

	public HexGrid hexGrid;
}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-13/creating-new-maps/new-map-menu.png)

*新建地图菜单组件。*

### 1.5 开幕和闭幕

我们可以通过激活和停用画布对象来打开和关闭弹出菜单。让我们在 `NewMapMenu` 中添加两个公共方法来处理这个问题。

```c#
	public void Open () {
		gameObject.SetActive(true);
	}

	public void Close () {
		gameObject.SetActive(false);
	}
```

现在将编辑器 UI 的 *New Map* 按钮连接到我们的 *New Map Menu* 对象的 `Open` 方法。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-13/creating-new-maps/on-click-open-menu.png)

*单击打开菜单。*

此外，将“*取消*”按钮连接到 `Close` 方法。这允许我们打开和关闭弹出菜单。

### 1.6 创建新地图

要实际创建新的地图，我们需要调用 *Hex Grid* 对象的 `CreateMap` 方法。此外，完成此操作后，我们应该关闭弹出菜单。在给定任意大小的情况下，向 `NewMapMenu` 添加一个方法来处理此问题。

```c#
	void CreateMap (int x, int z) {
		hexGrid.CreateMap(x, z);
		Close();
	}
```

此方法不必是公共的，因为无论如何我们都不能将其与按钮事件直接连接。相反，为每个按钮创建一个方法，该方法调用具有特定大小的 `CreateMap`。我使用 20 x 15 作为小地图，对应于默认的地图大小。我为中型地图选择了双倍的尺寸——40 乘 30，为大型地图再次选择了双倍尺寸。用各自的方法连接按钮。

```c#
	public void CreateSmallMap () {
		CreateMap(20, 15);
	}

	public void CreateMediumMap () {
		CreateMap(40, 30);
	}

	public void CreateLargeMap () {
		CreateMap(80, 60);
	}
```

### 1.7 锁定摄像头

我们现在可以使用弹出菜单创建三种不同尺寸的新地图！它工作得很好，但有一个细节我们应该注意。当“*新建地图菜单*”处于活动状态时，我们无法再与编辑器 UI 交互，也无法编辑单元格。但是，我们仍然可以控制相机。理想情况下，打开菜单时应锁定相机。

由于我们只有一个摄像头，快速实用的解决方案是向其添加一个静态 `Locked` 属性。这不是一个好的通用解决方案，但它足以满足我们简单的界面。这要求我们跟踪 `HexMapCamera` 中的静态实例，我们在相机唤醒时设置该实例。

```c#
	static HexMapCamera instance;
	
	…
	
	void OnEnable () {
		instance = this;
	}
```

`Locked` 属性可以是一个简单的仅限设置器的静态布尔属性。它所做的就是在 `HexMapCamera` 实例被锁定时禁用它，并在解锁时启用它。

```c#
	public static bool Locked {
		set {
			instance.enabled = !value;
		}
	}
```

现在是 `NewMapMenu.Open` 可以锁定相机，和 `NewMapMenu.Close` 可以解锁它。

```c#
	public void Open () {
		gameObject.SetActive(true);
		HexMapCamera.Locked = true;
	}

	public void Close () {
		gameObject.SetActive(false);
		HexMapCamera.Locked = false;
	}
```

### 1.8 保持有效的相机位置

相机还有另一个潜在的问题。当创建一个比当前地图小的新地图时，相机可能会超出地图的边界。它会一直保持这种状态，直到你试图移动相机。只有到那时，它才会被限制在新地图的边界内。

为了解决这个问题，我们可以在 `HexMapCamera` 中添加一个静态 `ValidatePosition` 方法。以零偏移调用实例的 `AdjustPosition` 方法将强制执行地图边界。如果相机已经在新地图的边界内，它就不会移动。

```c#
	public static void ValidatePosition () {
		instance.AdjustPosition(0f, 0f);
	}
```

在 `NewMapMenu.CreateMap` 中调用此方法，在创建新地图后。

```c#
	void CreateMap (int x, int z) {
		hexGrid.CreateMap(x, z);
		HexMapCamera.ValidatePosition();
		Close();
	}
```

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-13/creating-new-maps/creating-new-maps.unitypackage)

## 2 保存地图大小

虽然我们可以创建不同大小的地图，但在保存或加载时还没有考虑大小。这意味着加载地图将导致错误或无效地图，除非当前地图大小恰好与我们正在加载的地图匹配。

为了解决这个问题，我们必须在加载单元格数据之前创建一个具有适当大小的新地图。假设我们存储了一张小地图。在这种情况下，在 `HexGrid.Load` 的开头创建一个 20 乘 15 的地图将确保一切正常。

```c#
	public void Load (BinaryReader reader) {
		CreateMap(20, 15);

		for (int i = 0; i < cells.Length; i++) {
			cells[i].Load(reader);
		}
		for (int i = 0; i < chunks.Length; i++) {
			chunks[i].Refresh();
		}
	}
```

### 2.1 存储地图大小

当然，我们可以存储任何大小的地图。因此，通用的解决方案是在保存时在单元格数据之前写入地图大小。

```c#
	public void Save (BinaryWriter writer) {
		writer.Write(cellCountX);
		writer.Write(cellCountZ);
		
		for (int i = 0; i < cells.Length; i++) {
			cells[i].Save(writer);
		}
	}
```

然后，我们可以检索实际尺寸，并使用该尺寸创建具有正确尺寸的地图。

```c#
	public void Load (BinaryReader reader) {
		CreateMap(reader.ReadInt32(), reader.ReadInt32());

		…
	}
```

由于我们现在可以加载不同大小的地图，我们再次遇到了相机位置问题。通过在 `HexMapEditor.Load` 中加载地图后验证其位置来解决此问题。

```c#
	public void Load () {
		string path = Path.Combine(Application.persistentDataPath, "test.map");
		using (BinaryReader reader = new BinaryReader(File.OpenRead(path))) {
			int header = reader.ReadInt32();
			if (header == 0) {
				hexGrid.Load(reader, header);
				HexMapCamera.ValidatePosition();
			}
			else {
				Debug.LogWarning("Unknown map format " + header);
			}
		}
	}
```

### 2.2 新文件格式

虽然这种方法适用于我们从现在开始保存的地图，但它不适用于旧地图。相反，前面的教程将无法正确加载较新的地图文件。为了区分新旧格式，我们将增加标头整数。没有地图大小的旧保存格式是版本 0。地图大小的新格式是版本 1。所以保存时，`HexMapEditor.Save` 应写入 1 而不是 0。

```c#
	public void Save () {
		string path = Path.Combine(Application.persistentDataPath, "test.map");
		using (
			BinaryWriter writer =
				new BinaryWriter(File.Open(path, FileMode.Create))
		) {
			writer.Write(1);
			hexGrid.Save(writer);
		}
	}
```

从这一点开始，地图将保存为版本 1。如果你试图在上一教程的构建中打开它们，它将拒绝加载并抱怨未知的地图格式。事实上，当我们现在尝试加载这样的地图时，就会发生这种情况。我们必须调整 `HexMapEditor.Load` 方法，使其接受新版本。

```c#
	public void Load () {
		string path = Path.Combine(Application.persistentDataPath, "test.map");
		using (BinaryReader reader = new BinaryReader(File.OpenRead(path))) {
			int header = reader.ReadInt32();
			if (header == 1) {
				hexGrid.Load(reader);
				HexMapCamera.ValidatePosition();
			}
			else {
				Debug.LogWarning("Unknown map format " + header);
			}
		}
	}
```

### 2.3 向后兼容性

实际上，如果我们愿意，我们仍然可以加载版本 0 的地图，假设它们都有相同的 20 乘 15 的大小。因此，标头不必完全为 1，也可以为 0。由于每个版本都需要不同的方法，`HexMapEditor.Load` 必须将标头传递给 `HexGrid.Load`。

```c#
			if (header <= 1) {
				hexGrid.Load(reader, header);
				HexMapCamera.ValidatePosition();
			}
```

将 header 参数添加到 `HexGrid.Load` 中，然后使用它来决定要做什么。如果 header 至少为 1，则应读取地图大小数据。否则，使用旧的固定 20 乘 15 的地图尺寸，跳过读取尺寸数据。

```c#
	public void Load (BinaryReader reader, int header) {
		int x = 20, z = 15;
		if (header >= 1) {
			x = reader.ReadInt32();
			z = reader.ReadInt32();
		}
		CreateMap(x, z);

		…
	}
```

[version 0 map file](https://catlikecoding.com/unity/tutorials/hex-map/part-13/saving-map-size/test.map)

### 2.4 检查地图大小

就像创建新地图时一样，理论上我们最终加载的地图可能与我们的块大小不兼容。当这种情况发生时，我们应该中止加载地图。`HexGrid.CreateMap` 已拒绝创建地图并记录错误。为了将此信息传达给调用此方法的人，让它返回一个 bool，指示地图是否已创建。

```c#
	public bool CreateMap (int x, int z) {
		if (
			x <= 0 || x % HexMetrics.chunkSizeX != 0 ||
			z <= 0 || z % HexMetrics.chunkSizeZ != 0
		) {
			Debug.LogError("Unsupported map size.");
			return false;
		}

		…
		return true;
	}
```

现在，当地图创建失败时，`HexGrid.Load` 也可以中止。

```c#
	public void Load (BinaryReader reader, int header) {
		int x = 20, z = 15;
		if (header >= 1) {
			x = reader.ReadInt32();
			z = reader.ReadInt32();
		}
		if (!CreateMap(x, z)) {
			return;
		}

		…
	}
```

因为加载会覆盖现有单元格的所有数据，所以如果我们最终加载了相同大小的地图，我们实际上不必创建新的地图。所以可以跳过这一步。

```c#
		if (x != cellCountX || z != cellCountZ) {
			if (!CreateMap(x, z)) {
				return;
			}
		}
```

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-13/saving-map-size/saving-map-size.unitypackage)

## 3 文件管理

我们可以保存和加载不同大小的地图，但最终我们总是会在 *test.map* 中写入和读取。现在，我们将使使用多个文件成为可能。

我们将使用另一个弹出菜单来允许更高级的文件管理，而不是直接保存或加载地图。创建另一个类似于“*新建地图菜单*”的画布，但这次将其命名为“*保存加载菜单*”。此菜单将负责保存或加载地图，具体取决于将使用哪个按钮打开它。

我们将把“*保存加载菜单*”设计为保存菜单。稍后我们将动态地将其转换为加载菜单。它应该有一个背景和一个菜单面板、一个菜单标签和一个取消按钮，就像其他菜单一样。然后通过*游戏对象 / UI / 滚动视图*在菜单中添加滚动视图，以显示文件列表。在下面，通过 *GameObject / UI / Input Field* 输入一个输入字段，以指定新的地图名称。我们还需要一个操作按钮来保存地图。最后，让我们添加一个删除按钮来删除不再需要的地图。

![menu](https://catlikecoding.com/unity/tutorials/hex-map/part-13/file-management/save-menu.png) ![hierarchy](https://catlikecoding.com/unity/tutorials/hex-map/part-13/file-management/save-menu-hierarchy.png)

*保存加载菜单设计。*

默认的滚动视图允许水平和垂直滚动，但我们只需要一个垂直滚动列表。因此，禁用*水平*滚动并断开水平滚动条的连接。此外，我将“*运动类型*”设置为“夹紧（clamped）”并禁用“惯性”。这给了我们一个僵化的清单。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-13/file-management/scroll-rect.png)

*文件列表设置。*

删除我们的*文件列表*对象的*滚动条水平*子项，因为我们不需要它。然后调整*滚动条垂直*的大小，使其到达列表的底部。

您可以通过*名称输入*对象的*占位符*子对象调整其占位符文本。我使用了一个更具描述性的文本，但你也可以把它留空，去掉占位符。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-13/file-management/configured-menu.png)

*调整菜单设计。*

当我们完成设计后，停用菜单，使其默认隐藏。

### 3.1 控制菜单

为了使菜单工作，我们需要另一个脚本，在本例中是 `SaveLoadMenu`。与 `NewMapMenu` 一样，它需要引用网格以及 `Open` 和 `Close` 方法。

```c#
using UnityEngine;

public class SaveLoadMenu : MonoBehaviour {

	public HexGrid hexGrid;

	public void Open () {
		gameObject.SetActive(true);
		HexMapCamera.Locked = true;
	}

	public void Close () {
		gameObject.SetActive(false);
		HexMapCamera.Locked = false;
	}
}
```

将此组件添加到 *SaveLoadMenu* 中，并为其提供对网格对象的引用。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-13/file-management/saveloadmenu-component.png)

*SaveLoadMenu 组件。*

菜单将被打开以进行保存或加载。为了便于操作，请在 `Open` 方法中添加一个布尔参数。这表示菜单是否应处于保存模式。在字段中跟踪此模式，以便我们知道以后要执行哪个操作。

```c#
	bool saveMode;

	public void Open (bool saveMode) {
		this.saveMode = saveMode;
		gameObject.SetActive(true);
		HexMapCamera.Locked = true;
	}
```

现在，将*六边形地图编辑器*的“*保存*”和“*加载*”按钮连接到“*保存加载菜单*”对象的 `Open` 方法。仅检查“*保存*”按钮的布尔参数。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-13/file-management/editor-save-button.png)

*在保存模式下打开菜单。*

如果您还没有这样做，请将 *Cancel* 按钮的事件与 *Close* 方法挂钩。现在可以打开和关闭保存加载菜单。

### 3.2 更改外观

我们将菜单设计为保存菜单，但其模式取决于使用哪个按钮打开它。我们应该根据其模式更改菜单的外观。具体来说，我们应该更改菜单标签和操作按钮的标签。这意味着我们需要参考这些标签。

```c#
using UnityEngine;
using UnityEngine.UI;

public class SaveLoadMenu : MonoBehaviour {

	public Text menuLabel, actionButtonLabel;
	
	…
}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-13/file-management/labels.png)

*与标签连接。*

当菜单在保存模式下打开时，我们将使用现有的标签，即菜单的“*保存地图*”和操作按钮的“*保存*”。否则，我们将处于加载模式，并使用 *Load Map* 和 *Load*。

```c#
	public void Open (bool saveMode) {
		this.saveMode = saveMode;
		if (saveMode) {
			menuLabel.text = "Save Map";
			actionButtonLabel.text = "Save";
		}
		else {
			menuLabel.text = "Load Map";
			actionButtonLabel.text = "Load";
		}
		gameObject.SetActive(true);
		HexMapCamera.Locked = true;
	}
```

### 3.3 输入地图名称

暂时忽略文件列表，用户可以通过在输入字段中写入地图名称来指定要保存到哪个文件或从哪个文件加载。要检索此数据，我们需要一个对 *Name Input* 对象的 `InputField` 组件的引用。

```c#
	public InputField nameInput;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-13/file-management/name-input.png)

*与输入字段连接。*

不应要求用户写入地图文件的完整路径。只需要地图名称就可以了，不需要 *.map* 扩展名。让我们添加一个接受用户输入并为其构造正确路径的方法。当输入为空时，这是不可能的，因此在这种情况下，我们将返回 `null`。

```c#
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class SaveLoadMenu : MonoBehaviour {

	…

	string GetSelectedPath () {
		string mapName = nameInput.text;
		if (mapName.Length == 0) {
			return null;
		}
		return Path.Combine(Application.persistentDataPath, mapName + ".map");
	}
}
```

> **当用户输入无效字符时怎么办？**
>
> 如果用户输入了文件系统不支持的字符，我们可能会得到一个无效的路径。用户还可以输入路径分隔符，这允许他们从不受控制的位置进行保存和加载。
>
> 您可以使用输入字段的内容类型来控制允许的输入类型。例如，您可以将地图名称限制为仅包含字母数字字符，尽管这是相当有限的。您还可以使用自定义内容类型来精确指定允许和不允许的内容。

### 3.4 保存和加载

保存和加载现在由 `SaveLoadMenu` 负责。因此，将 `Save` 和 `Load` 方法从 `HexMapEditor` 移动到 `SaveLoadMenu`。它们不再需要公开，并且将使用路径参数而不是固定路径。

```c#
	void Save (string path) {
//		string path = Path.Combine(Application.persistentDataPath, "test.map");
		using (
			BinaryWriter writer =
			new BinaryWriter(File.Open(path, FileMode.Create))
		) {
			writer.Write(1);
			hexGrid.Save(writer);
		}
	}

	void Load (string path) {
//		string path = Path.Combine(Application.persistentDataPath, "test.map");
		using (BinaryReader reader = new BinaryReader(File.OpenRead(path))) {
			int header = reader.ReadInt32();
			if (header <= 1) {
				hexGrid.Load(reader, header);
				HexMapCamera.ValidatePosition();
			}
			else {
				Debug.LogWarning("Unknown map format " + header);
			}
		}
	}
```

因为我们现在正在加载任意文件，所以在尝试读取之前，最好确保文件确实存在。否则，我们将记录错误并中止。

```c#
	void Load (string path) {
		if (!File.Exists(path)) {
			Debug.LogError("File does not exist " + path);
			return;
		}
		…
	}
```

现在添加一个公共 `Action` 方法。它首先检索用户选择的路径。如果有路径，请根据需要保存到该路径或从该路径加载。然后关闭菜单。

```c#
	public void Action () {
		string path = GetSelectedPath();
		if (path == null) {
			return;
		}
		if (saveMode) {
			Save(path);
		}
		else {
			Load(path);
		}
		Close();
	}
```

将 *Action Button* 事件连接到此方法后，我们可以使用任意地图名称进行保存和加载。因为我们没有重置输入字段，所以所选名称将在下一次保存或加载操作中保持不变。当连续多次保存到同一文件或从同一文件加载时，这很方便，所以我们不需要更改。

### 3.5 地图项目

接下来，我们将用持久数据路径中的所有地图填充文件列表。当单击列表中的某个项目时，它将用于设置*名称输入*的文本。在 `SaveLoadMenu` 中添加一个公共方法以方便此操作。

```c#
	public void SelectItem (string name) {
		nameInput.text = name;
	}
```

我们需要一些东西来代表一个列表项。一个简单的按钮就可以了。创建一个并将其高度减少到 20 个单位，这样它就不会占用太多的垂直空间。它不应该看起来像一个按钮，因此清除其 *Image* 组件的 *Source Image* 引用。这将使它变成纯白色。此外，将其标签设置为使用左对齐，并确保文本和按钮左侧之间有一些空间。设计完成后，将其变成预制件。

![item](https://catlikecoding.com/unity/tutorials/hex-map/part-13/file-management/save-load-item.png) ![inspector](https://catlikecoding.com/unity/tutorials/hex-map/part-13/file-management/item-inspector.png)

*项目按钮。*

我们无法将按钮事件直接连接到“*新建地图菜单*”，因为它是一个预制件，在场景中还不存在。因此，该项目需要一个对菜单的引用，以便在单击时可以调用 `SelectItem` 方法。它还需要跟踪它所代表的地图名称，并设置其文本。创建一个小的 `SaveLoadItem` 组件来处理这个问题。

```c#
using UnityEngine;
using UnityEngine.UI;

public class SaveLoadItem : MonoBehaviour {

	public SaveLoadMenu menu;
	
	public string MapName {
		get {
			return mapName;
		}
		set {
			mapName = value;
			transform.GetChild(0).GetComponent<Text>().text = value;
		}
	}
	
	string mapName;
	
	public void Select () {
		menu.SelectItem(mapName);
	}
}
```

将组件添加到我们的项目中，并让按钮调用其 `Select` 方法。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-13/file-management/save-load-item-component.png)

*项目组件。*

### 3.6 填写列表

要填充列表，`SaveLoadMenu` 需要引用*文件列表*对象*视口*内的*内容*。它还需要一个对项目前言的引用。

```c#
	public RectTransform listContent;
	
	public SaveLoadItem itemPrefab;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-13/file-management/list-content.png)

*列表内容和预制连接。*

我们将使用一种新方法来填写这个列表。第一步是找出存在哪些地图文件。我们可以使用 `Directory.GetFiles` 方法获取一个包含目录中所有文件路径的数组。此方法有第二个参数，允许我们过滤文件。在我们的例子中，我们只想要与 `*.map` 匹配的文件。

```c#
	void FillList () {
		string[] paths =
			Directory.GetFiles(Application.persistentDataPath, "*.map");
	}
```

遗憾的是，文件的顺序无法得到保证。为了按字母顺序显示它们，我们必须使用 `System.Array.Sort` 对数组进行排序。

```c#
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class SaveLoadMenu : MonoBehaviour {

	…

	void FillList () {
		string[] paths =
			Directory.GetFiles(Application.persistentDataPath, "*.map");
		Array.Sort(paths);
	}

	…
}
```

接下来，我们为数组中的每个项目创建预制实例。将项目链接到菜单，设置其地图名称，并将其设置为列表内容的子项。

```c#
		Array.Sort(paths);
		for (int i = 0; i < paths.Length; i++) {
			SaveLoadItem item = Instantiate(itemPrefab);
			item.menu = this;
			item.MapName = paths[i];
			item.transform.SetParent(listContent, false);
		}
```

作为 `Directory.GetFiles` 返回文件的完整路径，我们必须对其进行清理。幸运的是，方便的 `Path.GetFileNameWithoutExtension` 方法正是我们需要的。

```c#
			item.MapName = Path.GetFileNameWithoutExtension(paths[i]);
```

在展示菜单之前，我们必须先填好清单。由于文件可能会发生变化，我们应该在每次打开菜单时都这样做。

```c#
	public void Open (bool saveMode) {
		…
		FillList();
		gameObject.SetActive(true);
		HexMapCamera.Locked = true;
	}
```

在多次填写列表时，我们必须确保在添加新项目之前删除所有旧项目。

```c#
	void FillList () {
		for (int i = 0; i < listContent.childCount; i++) {
			Destroy(listContent.GetChild(i).gameObject);
		}
		…
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-13/file-management/item-without-layout.png)

*没有布局的项目。*

### 3.7 项目布局

项目现在将显示在列表中，但它们最终会重叠并处于不利位置。要使它们形成垂直列表，请通过*组件 / 布局 / 垂直布局组*将*垂直布局组*组件添加到列表的 *Content* 对象中。

要使布局正确工作，请启用“*子控件大小*”和“*子强制展开*”的*宽度*。应禁用两个“*高度*”选项。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-13/file-management/vertical-layout-group.png) ![img](https://catlikecoding.com/unity/tutorials/hex-map/part-13/file-management/items-layout.png)

*使用垂直布局组。*

我们现在得到了一个很好的项目列表。然而，列表内容的大小并不能适应实际的项目数量。因此，滚动条永远不会改变大小。我们可以通过 *Component / Layout / Content Size Fitter* 向内容添加 *Content Size Fitter* 组件，使*内容*自动调整其大小。其*垂直安装*模式应设置为*首选*尺寸。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-13/file-management/content-size-fitter.png) ![img](https://catlikecoding.com/unity/tutorials/hex-map/part-13/file-management/items-fit.png)

*使用内容大小调整工具。*

现在，当只有少数项目时，滚动条将消失。当视口中的项目太多而无法容纳时，将显示适当大小的滚动条。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-13/file-management/items-scrollbar.png)

*此时会出现一个滚动条。*

### 3.8 删除地图

现在可以方便地使用许多地图文件。然而，在某些时候，你可能想摆脱一些地图。这就是*删除*按钮的作用。为它创建一个方法，并让按钮调用它。如果选择了路径，只需使用 `File.Delete` 将其删除。

```c#
	public void Delete () {
		string path = GetSelectedPath();
		if (path == null) {
			return;
		}
		File.Delete(path);
	}
```

我们应该再次确保我们使用的是实际存在的文件。如果没有，我们不应该试图删除它，但这并不保证会出错。

```c#
		if (File.Exists(path)) {
			File.Delete(path);
		}
```

删除地图后，我们不必关闭菜单。这使得删除一行中的多个文件，或者在保存或加载之前更容易。不过，我们应该在删除后清除*名称输入*，并刷新文件列表。

```c#
		if (File.Exists(path)) {
			File.Delete(path);
		}
		nameInput.text = "";
		FillList();
```

下一个教程是[地形纹理](https://catlikecoding.com/unity/tutorials/hex-map/part-14/)。

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-13/file-management/file-management.unitypackage)

[PDF](https://catlikecoding.com/unity/tutorials/hex-map/part-13/Hex-Map-13.pdf)