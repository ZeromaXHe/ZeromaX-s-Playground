# [返回主 Markdown](./CatlikeCoding网站翻译.md)



# [跳转系列独立 Markdown 16 ~ 21](./CatlikeCoding网站翻译-六边形地图16~21.md)



# Hex Map 22：高级视野

发布于 2017-10-14

https://catlikecoding.com/unity/tutorials/hex-map/part-22/

*平稳调整能见度。*
*使用标高来确定视线。*
*隐藏地图的边缘。*

这是关于[六边形地图](https://catlikecoding.com/unity/tutorials/hex-map/)的系列教程的第 22 部分。在添加了对探索的支持后，我们将升级我们的视觉计算和转换。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-22/tutorial-image.jpg)

*要看得远，就要走得高。*

## 1 可见性转换

一个单元格要么是可见的，要么是不可见的，因为它要么在一个单位的视野范围内，要么不在。尽管它看起来像一个单元在单元格之间移动需要一段时间，但它的视野会瞬间从一个单元格跳到另一个单元格。因此，其周围单元格的可见性会突然发生变化。该单位的运动看起来很平稳，而能见度的变化很突然。

理想情况下，能见度也会平稳变化。单元格在进入视野时会逐渐点亮，一旦不再可见，就会慢慢变暗。或者你更喜欢立即过渡？让我们向 `HexCellShaderData` 添加一个属性，以切换是否要立即转换。我们将使平滑过渡成为默认设置。

```c#
	public bool ImmediateMode { get; set; }
```

### 1.1 追踪过渡单元格

即使显示平滑过渡，实际的可见性数据仍然是二进制的。所以这纯粹是一种视觉效果。这意味着由 `HexCellShaderData` 来跟踪可见性转换。给它一个列表来跟踪正在转换的单元格。确保每次初始化后它都是空的。

```c#
using System.Collections.Generic;
using UnityEngine;

public class HexCellShaderData : MonoBehaviour {

	Texture2D cellTexture;
	Color32[] cellTextureData;

	List<HexCell> transitioningCells = new List<HexCell>();

	public bool ImmediateMode { get; set; }

	public void Initialize (int x, int z) {
		…

		transitioningCells.Clear();
		enabled = true;
	}
	
	…
}
```

目前，我们直接在 `RefreshVisibility` 中设置单元格数据。当立即模式处于活动状态时，这仍然是正确的。但如果不是，我们应该将该单元格添加到过渡单元格列表中。

```c#
	public void RefreshVisibility (HexCell cell) {
		int index = cell.Index;
		if (ImmediateMode) {
			cellTextureData[index].r = cell.IsVisible ? (byte)255 : (byte)0;
			cellTextureData[index].g = cell.IsExplored ? (byte)255 : (byte)0;
		}
		else {
			transitioningCells.Add(cell);
		}
		enabled = true;
	}
```

可见性现在似乎不再有效，因为我们还没有对列表中的单元格进行任何操作。

### 1.2 在过渡单元格中循环

我们不会立即将相关值设置为 255 或 0，而是逐渐增加或减少这些值。我们这样做的速度决定了过渡的平滑程度。我们不应该做得太快，也不应该太慢。一秒钟是良好的视觉效果和可玩性之间的良好折衷。让我们为此定义一个常数，这样很容易更改。

```c#
	const float transitionSpeed = 255f;
```

在 `LateUpdate` 中，我们现在可以通过将时间增量与速度相乘来确定应用于值的增量。这必须是一个整数，因为我们不知道它会有多大。异常的帧率下降可能会使增量大于 255。

此外，只要有单元格处于过渡期，我们就必须不断更新。因此，请确保在列表中有内容时，我们仍处于启用状态。

```c#
	void LateUpdate () {
		int delta = (int)(Time.deltaTime * transitionSpeed);

		cellTexture.SetPixels32(cellTextureData);
		cellTexture.Apply();
		enabled = transitioningCells.Count > 0;
	}
```

理论上也有可能获得非常高的帧率。再加上低转换速度，这可能会导致增量为零。为了保证进度，强制增量至少为1。

```c#
		int delta = (int)(Time.deltaTime * transitionSpeed);
		if (delta == 0) {
			delta = 1;
		}
```

在我们得到增量后，我们可以遍历所有正在转换的单元格并更新它们的数据。让我们假设我们有一个 `UpdateCellData` 方法，它有相关的单元格和 delta 作为参数。

```c#
		int delta = (int)(Time.deltaTime * transitionSpeed);
		if (delta == 0) {
			delta = 1;
		}
		for (int i = 0; i < transitioningCells.Count; i++) {
			UpdateCellData(transitioningCells[i], delta);
		}
```

在某个时候，一个单元格的转换应该完成。让我们假设该方法返回它是否仍在转换。当情况不再如此时，我们可以从列表中删除该单元格。之后，我们必须递减迭代器以不跳过任何单元格。

```c#
		for (int i = 0; i < transitioningCells.Count; i++) {
			if (!UpdateCellData(transitioningCells[i], delta)) {
				transitioningCells.RemoveAt(i--);
			}
		}
```

我们处理过渡单元格的顺序并不重要。因此，我们不必删除当前索引处的单元格，这会迫使 `RemoveAt` 移动它之后的所有单元格。相反，将最后一个单元格移动到当前索引，它们会删除最后一个。

```c#
			if (!UpdateCellData(transitioningCells[i], delta)) {
				transitioningCells[i--] =
					transitioningCells[transitioningCells.Count - 1];
				transitioningCells.RemoveAt(transitioningCells.Count - 1);
			}
```

现在我们必须创建 `UpdateCellData` 方法。它需要单元格的索引和数据来完成它的工作，所以从获取这些开始。它还必须确定此单元格是否仍需要进一步更新。默认情况下，我们将假设情况并非如此。工作完成后，必须应用调整后的数据，并返回仍在更新的状态。

```c#
	bool UpdateCellData (HexCell cell, int delta) {
		int index = cell.Index;
		Color32 data = cellTextureData[index];
		bool stillUpdating = false;

		cellTextureData[index] = data;
		return stillUpdating;
	}
```

### 1.3 更新单元格数据

此时，我们有一个正在过渡的单元格，或者可能已经完成。首先，让我们考虑一下单元格的探索状态。如果对单元格进行了探索，但它的 G 值还不是 255，那么它确实仍在转换中，所以要跟踪这一事实。

```c#
		bool stillUpdating = false;

		if (cell.IsExplored && data.g < 255) {
			stillUpdating = true;
		}

		cellTextureData[index] = data;
```

要推进转换，请将增量添加到单元格的 G 值中。算术运算不适用于字节，它们总是先转换为整数。所以总和是一个整数，必须转换为字节。

```c#
		if (cell.IsExplored && data.g < 255) {
			stillUpdating = true;
			int t = data.g + delta;
			data.g = (byte)t;
		}
```

但我们必须确保在类型转换前不超过 255。

```c#
			int t = data.g + delta;
			data.g = t >= 255 ? (byte)255 : (byte)t;
```

接下来，我们必须对可见性做同样的事情，即使用 R 值。

```c#
		if (cell.IsExplored && data.g < 255) {
			…
		}

		if (cell.IsVisible && data.r < 255) {
			stillUpdating = true;
			int t = data.r + delta;
			data.r = t >= 255 ? (byte)255 : (byte)t;
		}
```

由于单元格也可能再次变得不可见，我们还必须检查是否必须减小 R 值。当 R 大于零时，单元格不可见。

```c#
		if (cell.IsVisible) {
			if (data.r < 255) {
				stillUpdating = true;
				int t = data.r + delta;
				data.r = t >= 255 ? (byte)255 : (byte)t;
			}
		}
		else if (data.r > 0) {
			stillUpdating = true;
			int t = data.r - delta;
			data.r = t < 0 ? (byte)0 : (byte)t;
		}
```

现在 `UpdateCellData` 已完成，可见性转换功能正常。

*可见性转换。*

### 1.4 防止重复的过渡条目

虽然转换有效，但我们最终可能会在列表中出现重复条目。当单元格的可见性状态在转换过程中发生变化时，就会发生这种情况。例如，当一个单元格在一个单元的旅程中只在短时间内可见时。

重复条目的结果是，单元格的转换在每帧中会更新多次，这会导致转换速度更快，工作量也更多。我们可以通过在添加单元格之前检查单元格是否已在列表中来防止这种情况。但是，每次调用 `RefreshVisibility` 时搜索列表的成本很高，尤其是当许多单元格已经处于转换状态时。相反，让我们使用一个尚未使用的数据通道来存储一个单元格是否处于转换状态，如 B 值。将单元格添加到列表时，将此值设置为 255。然后只添加 B 值不是 255 的单元格。

```c#
	public void RefreshVisibility (HexCell cell) {
		int index = cell.Index;
		if (ImmediateMode) {
			cellTextureData[index].r = cell.IsVisible ? (byte)255 : (byte)0;
			cellTextureData[index].g = cell.IsExplored ? (byte)255 : (byte)0;
		}
		else if (cellTextureData[index].b != 255) {
			cellTextureData[index].b = 255;
			transitioningCells.Add(cell);
		}
		enabled = true;
	}
```

为了实现这一点，一旦单元格的转换完成，我们必须将 B 值设置回零。

```c#
	bool UpdateCellData (HexCell cell, int delta) {
		…

		if (!stillUpdating) {
			data.b = 0;
		}
		cellTextureData[index] = data;
		return stillUpdating;
	}
```

*无重复的过渡。*

### 1.5 立即加载可见性

即使在加载地图时，可见性变化现在也总是渐进的。这没有多大意义，因为地图表示一个单元格已经可见的状态，因此转换是不合适的。此外，启动大地图上许多可见单元格的过渡可能会在加载后减慢游戏速度。因此，让我们在加载单元格和单位之前，在 `HexGrid.Load` 中切换到即时模式。

```c#
	public void Load (BinaryReader reader, int header) {
		…

		cellShaderData.ImmediateMode = true;

		for (int i = 0; i < cells.Length; i++) {
			cells[i].Load(reader, header);
		}
		…
	}
```

这会覆盖原始的即时模式设置，无论它是什么。也许它总是被禁用，或者它被变成了一个配置选项。因此，请记住原始模式，并在工作完成后切换回该模式。

```c#
	public void Load (BinaryReader reader, int header) {
		…
		
		bool originalImmediateMode = cellShaderData.ImmediateMode;
		cellShaderData.ImmediateMode = true;

		…

		cellShaderData.ImmediateMode = originalImmediateMode;
	}
```

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-22/visibility-transitions/visibility-transitions.unitypackage)

## 2 基于高程的视野

到目前为止，我们只是为所有单元使用了三个固定的视图范围，但视野比这更复杂。一般来说，我们看不到东西有两个原因。首先，它前面还有别的东西，挡住了我们的视线。第二种是，有些东西在视觉上太小了，我们无法感知，要么是因为它真的很小，要么是很远。我们只关心视野障碍。

我们看不到地球的另一边是什么，因为地球挡住了我们的视线。我们只能看到地平线。因为行星大致是一个球体，我们的视角越高，我们能看到的表面就越多。所以地平线取决于海拔。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-22/vision-based-on-elevation/elevation-horizon.png)

*地平线取决于视图标高。*

我们单位的有限视野模拟了地球曲率引起的地平线效应。他们应该能看到多远取决于地球的大小和地图的比例尺。至少，这就是理由。我们限制视野的主要原因是为了游戏，即被称为战争迷雾的信息限制。但是，了解其背后的物理学原理，我们可以得出结论，一个高有利位置应该具有战略价值，因为它可以扩展视野，使人们能够越过较低的障碍物。然而，目前情况并非如此。

### 2.1 观景高度

为了考虑视觉的高度，我们需要知道一个单元格的高度，以便观察。这要么是它的正常高度，要么是它自己的水位，这取决于它是干燥的还是淹没的。让我们为 `HexCell` 添加一个方便的属性。

```c#
	public int ViewElevation {
		get {
			return elevation >= waterLevel ? elevation : waterLevel;
		}
	}
```

但是，如果视觉受到高程的影响，那么当单元格的视图高程发生变化时，可见性情况也可能发生变化。由于单元格可能已经或正在阻挡多个单位的视觉，因此确定需要改变的内容并非易事。更改后的单元格本身不可能解决这个问题，所以让它通知 `HexCellShaderData` 情况已经发生了变化。让我们假设 `HexCellShaderData` 有一个方便的 `ViewElevationChanged` 方法。在 `HexCell.Elevation` 时调用它。如果需要，可以设置标高。

```c#
	public int Elevation {
		get {
			return elevation;
		}
		set {
			if (elevation == value) {
				return;
			}
			int originalViewElevation = ViewElevation;
			elevation = value;
			if (ViewElevation != originalViewElevation) {
				ShaderData.ViewElevationChanged();
			}
			…
		}
	}
```

`WaterLevel` 也是如此。

```c#
	public int WaterLevel {
		get {
			return waterLevel;
		}
		set {
			if (waterLevel == value) {
				return;
			}
			int originalViewElevation = ViewElevation;
			waterLevel = value;
			if (ViewElevation != originalViewElevation) {
				ShaderData.ViewElevationChanged();
			}
			ValidateRivers();
			Refresh();
		}
	}
```

### 2.2 重置可见性

现在我们必须创建 `HexCellShaderData.ViewElevationChanged` 方法。弄清楚整体可见性情况是如何变化的是一个难题，尤其是当多个单元格一起改变时。所以我们在这件事上不会很聪明。相反，我们将安排重置所有单元格可见性。添加一个布尔字段来跟踪这是否是必需的。在方法内部，只需将其设置为 true 并启用组件。无论一次改变多少个单元格，都会导致一次重置。

```c#
	bool needsVisibilityReset;

	…

	public void ViewElevationChanged () {
		needsVisibilityReset = true;
		enabled = true;
	}
```

重置所有单元格的可见性需要访问它们，而 `HexCellShaderData` 没有访问权限。因此，让我们将这一责任委托给 `HexGrid`。这要求我们向 `HexCellShaderData` 添加一个引用网格的属性。然后我们可以在 `LateUpdate` 中使用它来请求重置。

```c#
	public HexGrid Grid { get; set; }
						
	…

	void LateUpdate () {
		if (needsVisibilityReset) {
			needsVisibilityReset = false;
			Grid.ResetVisibility();
		}

		…
	}
```

转到 `HexGrid`，在 `HexGrid.Awake` 中设置网格引用。创建着色器数据后唤醒。

```c#
	void Awake () {
		HexMetrics.noiseSource = noiseSource;
		HexMetrics.InitializeHashGrid(seed);
		HexUnit.unitPrefab = unitPrefab;
		cellShaderData = gameObject.AddComponent<HexCellShaderData>();
		cellShaderData.Grid = this;
		CreateMap(cellCountX, cellCountZ);
	}
```

`HexGrid` 还必须获得 `ResetVisibility` 方法来重置所有单元格。只需让它循环遍历单元格，并将重置委托给它们。

```c#
	public void ResetVisibility () {
		for (int i = 0; i < cells.Length; i++) {
			cells[i].ResetVisibility();
		}
	}
```

现在我们必须向 `HexCell` 添加一个 `ResetVisiblity` 方法。只需将单元格的可见性设置为零并触发可见性刷新。我们只需要在单元格的可见度实际上大于零时才需要这样做。

```c#
	public void ResetVisibility () {
		if (visibility > 0) {
			visibility = 0;
			ShaderData.RefreshVisibility(this);
		}
	}
```

重置所有可见性数据后，`HexGrid.ResetVisibility` 必须再次应用所有单位的视野，为此它需要知道每个单元的视觉范围。让我们假设这可以通过 `VisionRange` 属性获得。

```c#
	public void ResetVisibility () {
		for (int i = 0; i < cells.Length; i++) {
			cells[i].ResetVisibility();
		}
		for (int i = 0; i < units.Count; i++) {
			HexUnit unit = units[i];
			IncreaseVisibility(unit.Location, unit.VisionRange);
		}
	}
```

为了实现这一点，重构并将 `HexUnit.visionRange` 重命名为 `HexUnit.VisionRange` 并将其转化为一个属性。目前，它总是让我们得到常数 3，但这在未来会改变。

```c#
	public int VisionRange {
		get {
			return 3;
		}
	}
```

这将确保可见性数据在单元格的视图标高更改后重置并保持正确。但也有可能我们在游戏模式下更改视觉规则并触发重新编译。为了确保视觉自动调整，当我们检测到重新编译时，让我们在 `HexGrid.OnEnable` 中触发重置。

```c#
	void OnEnable () {
		if (!HexMetrics.noiseSource) {
			…
			ResetVisibility();
		}
	}
```

现在我们可以更改我们的视觉代码，并在保持游戏模式的同时看到结果。

### 2.3 拓展视野

视野的工作原理由 `HexGrid.GetVisibleCells` 决定。要使高度影响视野范围，我们可以简单地使用 `fromCell` 的视图高度，暂时覆盖提供的范围。这使得它很容易工作。

```c#
	List<HexCell> GetVisibleCells (HexCell fromCell, int range) {
		…

		range = fromCell.ViewElevation;
		fromCell.SearchPhase = searchFrontierPhase;
		fromCell.Distance = 0;
		searchFrontier.Enqueue(fromCell);
		…
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-22/vision-based-on-elevation/elevation-is-range.png)

*使用标高作为范围。*

### 2.4 阻挡视线

当所有其他单元格都处于标高零时，使用视图标高作为范围可以正常工作。但是，如果所有单元格的高度都与视点相同，那么我们最终的有效范围应该是零。此外，高海拔的单元格应该会阻挡视线，降低后面的单元格。目前也不是这样。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-22/vision-based-on-elevation/unblocked-vision.png)

*视野开阔。*

虽然确定视野的最正确方法是进行某种光线投射测试，但这会很快变得昂贵，并且仍然可能产生奇怪的结果。我们需要一种快速的方法，提供足够好的结果，但不一定是完美的。此外，视野规则简单、直观、易于玩家预测也很重要。

我们想要的解决方案是在确定何时可以看到单元格时，将邻居的视图高程添加到覆盖距离中。这有效地缩短了观察这些单元格时的视觉范围。当它们被跳过时，这也会阻止我们到达它们后面的单元格。

```c#
				int distance = current.Distance + 1;
				if (distance + neighbor.ViewElevation > range) {
					continue;
				}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-22/vision-based-on-elevation/blocked-vision.png)

*升高的单元格会阻碍视力。*

> **我们难道不应该看到远处的高单元格吗？**
>
> 就像山脉一样，你可以看到它们的斜坡与你仍然可以看到的单元格相邻。但是你从上面看不到山脉，所以你看不到单元格本身。

### 2.5 不看周围角落

现在看起来，高单元格会阻挡低单元格的视野，但有时视野似乎不应该通过。这是因为我们的搜索算法仍然找到了通往这些单元格的路径，绕过了阻挡单元格。因此，我们的视野似乎可以绕过障碍。为了防止这种情况，我们必须确保在确定单元格的可见性时只考虑最短路径。这可以通过跳过比这更长的路径来实现。

```c#
		HexCoordinates fromCoordinates = fromCell.coordinates;
		while (searchFrontier.Count > 0) {
			…

			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
				…

				int distance = current.Distance + 1;
				if (distance + neighbor.ViewElevation > range ||
					distance > fromCoordinates.DistanceTo(neighbor.coordinates)
				) {
					continue;
				}

				…
			}
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-22/vision-based-on-elevation/shortest-paths-only.png)

*只使用最短路径。*

这解决了大多数明显错误的情况。它对附近的单元格很有效，因为只有少数最短的路径可以到达这些单元格。更远的单元格有更多可能的路径，因此在远距离仍可能发生视野弯曲。如果视线范围仍然很短，附近的海拔差异也不太大，这就不会是问题。

最后，将视图仰角添加到提供的范围中，而不是替换它。该单元的固有范围表示其自身的高度、飞行高度或侦察潜力。

```c#
		range += fromCell.ViewElevation;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-22/vision-based-on-elevation/full-range.png)

*使用较低的视点进行全范围视野。*

因此，我们最终的视野规则是让视线沿着最短的路径到达视觉范围，并根据单元格相对于视点的视高差进行修改。当一个单元格超出范围时，它会阻挡通过它的所有路径。其结果是，视野畅通的高有利位置具有战略价值。

> **那么阻碍视觉的特征呢？**
>
> 我决定不让特征影响视觉，但例如，你可以让茂密的森林或墙壁为单元格添加有效的标高。这确实使玩家更难估计视觉规则。

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-22/vision-based-on-elevation/vision-based-on-elevation.unitypackage)

## 3 无法探索的单元格

关于视野的最后一个问题涉及地图的边缘。地形突然结束，没有过渡，边缘的单元格缺乏邻居。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-22/inexplorable-cells/obvious-map-edge.png)

*明显的地图边缘。*

理想情况下，未探索区域的视觉效果和地图边缘是相同的。我们可以通过在没有邻居的情况下添加三角边的特殊情况来实现这一点，但这需要额外的逻辑，我们必须处理缺失的单元格。所以这不是小事。另一种方法是强制地图上的边界单元格保持未探索状态，即使它们在部队的视线范围内。这种方法要简单得多，所以让我们继续下去。它还可以标记其他无法绘制的单元格，使不规则的地图边缘更容易实现。除此之外，隐藏的边缘单元可以使河流和道路看起来像是进入和离开地图，因为它们的终点不在视线范围内。你也可以让单位以这种方式进入或离开地图。

### 3.1 将单元格标记为可探索的

要指示单元格是可探索的，请向 `HexCell` 添加 `Explorable` 属性。

```c#
	public bool Explorable { get; set; }
```

现在，只有当一个单元格也是可探索的时，它才是可见的，因此调整 `IsVisible` 属性以考虑到这一点。

```c#
	public bool IsVisible {
		get {
			return visibility > 0 && Explorable;
		}
	}
```

`IsExplored` 也是如此。但是，我们为此使用了默认属性。我们必须将其转换为显式属性，以便能够调整其 getter 逻辑。

```c#
	public bool IsExplored {
		get {
			return explored && Explorable;
		}
		private set {
			explored = value;
		}
	}

	…

	bool explored;
```

### 3.2 隐藏地图边缘

隐藏矩形地图的边缘可以在 `HexGrid.CreateCell` 方法中完成。不在边缘的单元格是可探索的，而所有其他单元格都是不可探索的。

```c#
	void CreateCell (int x, int z, int i) {
		…

		HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
		cell.transform.localPosition = position;
		cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
		cell.Index = i;
		cell.ShaderData = cellShaderData;

		cell.Explorable =
			x > 0 && z > 0 && x < cellCountX - 1 && z < cellCountZ - 1;
		
		…
	}
```

现在我们的地图在边缘逐渐消失，伟大的超越是无法探索的。因此，我们地图的可探索尺寸在每个维度上减少了两个。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-22/inexplorable-cells/inexplorable-map-edge.png)

*无法绘制的地图边缘。*

> **让可探索状态可编辑怎么样？**
>
> 这是可能的，给你最大的灵活性。然后，您还必须将其添加到保存数据中。

### 3.3 无法探索的单元格会阻碍视野

最后，如果一个单元格不能被探索，那么它也应该阻挡视野。调整 `HexGrid.GetVisibleCells` 以考虑这一点。

```c#
				if (
					neighbor == null ||
					neighbor.SearchPhase > searchFrontierPhase ||
					!neighbor.Explorable
				) {
					continue;
				}
```

下一个教程是[生成土地](https://catlikecoding.com/unity/tutorials/hex-map/part-23/)。

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-22/inexplorable-cells/inexplorable-cells.unitypackage)

[PDF](https://catlikecoding.com/unity/tutorials/hex-map/part-22/Hex-Map-22.pdf)

# Hex Map 23：生成土地

发布于 2017-11-14

https://catlikecoding.com/unity/tutorials/hex-map/part-23/

*用生成的景观填充新地图。*
*把大块的土地举到水面上，再沉一些。*
*控制土地的面积、高度和不稳定程度。*
*支持多种配置选项以创建不同的地图。*
*使再次生成相同的地图成为可能。*

这是关于[六边形地图](https://catlikecoding.com/unity/tutorials/hex-map/)的系列教程的第 23 部分。这是介绍如何按程序生成地图的几个教程中的第一个。

本教程是用 Unity 2017.1.0 编写的。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-23/tutorial-image.jpg)

*生成的许多地图之一。*

## 1 生成地图

虽然我们可以手动创建任何我们喜欢的地图，但这可能需要很多时间。如果我们的应用程序可以通过为设计师生成地图来帮助他们开始，他们可以根据需要进行修改，那将是非常方便的。更进一步的是完全放弃手动设计，完全依靠应用程序本身为我们生成一张完整的地图。这将使我们每次都可以使用新地图玩游戏，确保每个新的游戏会话都是不同的。当探索是一个需要多次玩的游戏的重要组成部分时，事先不知道你要玩的地图的布局是至关重要的。为了使这一切成为可能，我们必须创建一个生成地图的算法。

你需要什么样的地图生成算法取决于你的应用程序需要的地图类型。没有一种最佳方法可以做到这一点，但在可信度和可玩性之间总是存在权衡。

可信度是指游戏玩家接受地图是可能的和真实的。这并不意味着地图必须看起来像是我们星球的一部分。它们可能是另一个星球，也可能是完全不同的现实。但如果它被认为代表了土质地形，它至少应该看起来有点像这个部分。

可玩性是指地图是否支持你想要的游戏体验。这往往与可信度相矛盾。例如，虽然山脉可能看起来很棒，但从逻辑上讲，它们也严重限制了部队的行动和视野。如果这不是你想要的，你就必须没有山脉，这可能会降低可信度，限制你游戏的表现力。或者，你可以保留山脉，但减少它们对游戏的影响，这也可能降低可信度。

除此之外，还有可行性。例如，你可以通过模拟板块构造、侵蚀、降雨、火山爆发、流星撞击、月球影响等来制作一个非常逼真的类地行星。但这需要很长时间才能开发出来。此外，生成这样一个星球可能需要一段时间，玩家不会喜欢在开始新游戏之前等待几分钟。因此，虽然模拟可以成为一种强大的工具，但它是有成本的。

游戏充满了可信度、可玩性和可行性之间的权衡。有时，这些权衡会被忽视，看起来很正常，或者是任意的、不一致的或令人不快的，这取决于制作游戏的人的选择和优先级。这不仅限于地图生成，但在开发程序地图生成器时，您必须非常注意这一点。你最终可能会花很多时间创建一个算法，生成漂亮的地图，而这些地图对你想要制作的游戏来说也是无用的。

在本系列教程中，我们将学习类地地形。它应该看起来很有趣，种类繁多，没有大的同质区域。地形的规模将很大，地图覆盖一个或多个大陆、海洋地区，甚至整个星球。我们希望对地理有合理的控制，包括陆地、气候、有多少地区以及地形有多崎岖。本教程将为陆地（landmass）奠定基础。

### 1.1 以编辑模式启动

由于我们将专注于地图而不是游戏，因此在编辑模式下直接启动我们的应用程序很方便。这样我们马上就能看到地图了。因此，调整 `HexMapEditor.Awake` 可将编辑模式设置为 true 并启用编辑模式着色器关键字。同时调整 GUI 中编辑切换的默认状态。

```c#
	void Awake () {
		terrainMaterial.DisableKeyword("GRID_ON");
		Shader.EnableKeyword("HEX_MAP_EDIT_MODE");
		SetEditMode(true);
	}
```

### 1.2 地图生成器

因为程序化地图生成需要相当多的代码，所以我们不会直接将其添加到 `HexGrid` 中。相反，我们将为它创建一个新的 `HexMapGenerator` 组件，使 `HexGrid` 不知道它。如果你愿意，这也使以后更容易切换到不同的算法。

生成器需要参考网格，所以给它一个公共字段。除此之外，添加一个公共的 `GenerateMap` 方法来完成算法的工作。将地图维度作为参数给它，然后让它使用这些参数来创建一个新的空地图。

```c#
using System.Collections.Generic;
using UnityEngine;

public class HexMapGenerator : MonoBehaviour {

	public HexGrid grid;

	public void GenerateMap (int x, int z) {
		grid.CreateMap(x, z);
	}
}
```

将带有 `HexMapGenerator` 组件的对象添加到场景中，并将其连接到网格。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-23/generating-maps/generator-object.png)

*地图生成器对象。*

### 1.3 调整新地图菜单

我们将调整 `NewMapMenu`，使其除了创建空地图外，还可以生成地图。我们将通过一个布尔 `generateMaps` 字段来控制它的操作，该字段默认设置为 `true`。创建一个公共方法来设置此字段，就像我们为 `HexMapEditor` 的切换选项所做的那样。将相应的切换添加到菜单 UI 并将其连接到方法。

```c#
	bool generateMaps = true;

	public void ToggleMapGeneration (bool toggle) {
		generateMaps = toggle;
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-23/generating-maps/new-map-menu.png)

*带有切换的新地图菜单。*

为菜单提供地图生成器的参考。然后，在适当的时候，让它调用生成器的 `GenerateMap` 方法，而不是直接调用网格的 `CreateMap`。

```c#
	public HexMapGenerator mapGenerator;

	…

	void CreateMap (int x, int z) {
		if (generateMaps) {
			mapGenerator.GenerateMap(x, z);
		}
		else {
			hexGrid.CreateMap(x, z);
		}
		HexMapCamera.ValidatePosition();
		Close();
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-23/generating-maps/new-map-menu-inspector.png)

*连接到生成器。*

### 1.4 访问单元格

为了完成其工作，生成器需要访问网格的单元。`HexGrid` 已经有了公共 `GetCell` 方法，这些方法需要位置向量或六边形坐标。生成器不需要使用任何一个，所以让我们添加两个方便的 `HexGrid.GetCell` 方法，它们可以使用偏移坐标或单元格索引。

```c#
	public HexCell GetCell (int xOffset, int zOffset) {
		return cells[xOffset + zOffset * cellCountX];
	}
	
	public HexCell GetCell (int cellIndex) {
		return cells[cellIndex];
	}
```

现在 `HexMapGenerator` 可以直接检索单元格。例如，创建新地图后，使用偏移坐标将中间单元格列的地形设置为草地。

```c#
	public void GenerateMap (int x, int z) {
		grid.CreateMap(x, z);
		for (int i = 0; i < z; i++) {
			grid.GetCell(x / 2, i).TerrainTypeIndex = 1;
		}
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-23/generating-maps/generated-map.jpg)

*小地图上的草列。*

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-23/generating-maps/generating-maps.unitypackage)

## 2 创造土地

在生成地图时，我们从概念上讲是在没有任何土地的情况下开始的。你可以想象整个世界都被一个大海覆盖着。当海底的一部分被推到水面以上时，陆地就形成了。我们必须决定以这种方式创造了多少土地，它出现在哪里，以什么形状出现。

### 2.1 地形升高

我们从小处着手，在水面上开垦一块土地。为此创建一个 `RaiseTerrain` 方法，并使用参数控制块的大小。在 `GenerateMap` 中调用此方法，替换之前的测试代码。让我们从一小块土地开始，由七个单元格组成。

```c#
	public void GenerateMap (int x, int z) {
		grid.CreateMap(x, z);
//		for (int i = 0; i < z; i++) {
//			grid.GetCell(x / 2, i).TerrainTypeIndex = 1;
//		}
		RaiseTerrain(7);
	}

	void RaiseTerrain (int chunkSize) {}
```

现在，我们将简单地使用草地形类型来表示高地，初始的沙地地形代表海洋。我们将让 `RaiseTerrain` 抓取一个随机单元格并调整其地形类型，直到我们得到所需的土地数量。

要获取随机单元格，请添加一个 `GetRandomCell` 方法，该方法确定随机单元格索引并从网格中检索相应的单元格。

```c#
	void RaiseTerrain (int chunkSize) {
		for (int i = 0; i < chunkSize; i++) {
			GetRandomCell().TerrainTypeIndex = 1;
		}
	}

	HexCell GetRandomCell () {
		return grid.GetCell(Random.Range(0, grid.cellCountX * grid.cellCountZ));
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-23/creating-land/random-cells.jpg)

*七个随机的陆地单元格。*

由于我们最终可能需要大量的随机单元格，或者多次遍历所有单元格，让我们跟踪 `HexMapGenerator` 本身中的单元格数量。

```c#
	int cellCount;

	public void GenerateMap (int x, int z) {
		cellCount = x * z;
		…
	}

	…

	HexCell GetRandomCell () {
		return grid.GetCell(Random.Range(0, cellCount));
	}
```

### 2.2 创建单个块

当我们把七个随机单元格变成陆地时，它们可以在任何地方。它们很可能不会形成一块土地。我们也可能会多次拾取同一个单元格，最终得到的土地数量低于预期。为了解决这两个问题，只能无约束地选取第一个单元格。之后，我们必须只选择与我们之前选择的单元格相邻的单元格。这些限制与寻路非常相似，所以让我们在这里使用相同的方法。

为 `HexMapGenerator` 提供自己的优先级队列和搜索前沿相位计数器，就像 `HexGrid` 一样。

```c#
	HexCellPriorityQueue searchFrontier;

	int searchFrontierPhase;
```

在我们需要它之前，请确保优先级队列存在。

```c#
	public void GenerateMap (int x, int z) {
		cellCount = x * z;
		grid.CreateMap(x, z);
		if (searchFrontier == null) {
			searchFrontier = new HexCellPriorityQueue();
		}
		RaiseTerrain(7);
	}
```

创建新地图后，所有单元格的搜索边界为零。但是，如果我们在生成地图的同时搜索单元格，我们将在此过程中增加它们的搜索边界。如果我们进行大量搜索，它们可能会在 `HexGrid` 记录的搜索前沿阶段之前结束。这可能会打破单位的寻路。为防止这种情况，请在地图生成过程结束时将所有单元格的搜索阶段重置为零。

```c#
		RaiseTerrain(7);
		for (int i = 0; i < cellCount; i++) {
			grid.GetCell(i).SearchPhase = 0;
		}
```

`RaiseTerrain` 现在必须搜索合适的单元格，而不是随机选择所有单元格。这个过程与我们在 `HexGrid` 中搜索的方式非常相似。但是，我们永远不会访问单元格超过一次，所以我们可以通过将搜索边界阶段增加 1 而不是 2 来满足需求。然后用第一个随机单元初始化边界。除了像往常一样设置搜索阶段外，还要确保将其距离和启发式设置为零。

```c#
	void RaiseTerrain (int chunkSize) {
//		for (int i = 0; i < chunkSize; i++) {
//			GetRandomCell().TerrainTypeIndex = 1;
//		}
		searchFrontierPhase += 1;
		HexCell firstCell = GetRandomCell();
		firstCell.SearchPhase = searchFrontierPhase;
		firstCell.Distance = 0;
		firstCell.SearchHeuristic = 0;
		searchFrontier.Enqueue(firstCell);
	}
```

之后的搜索循环也很熟悉。除了继续直到边界为空，我们还应该在块达到所需大小时停止，所以要跟踪这一点。每次迭代，将下一个单元排成队列，设置其地形类型，增加大小，然后遍历该单元格的邻居。如果还没有添加，所有邻居都会被添加到边界。我们不需要做任何其他的比较或调整。一旦我们完成，一定要清理边境。

```c#
		searchFrontier.Enqueue(firstCell);

		int size = 0;
		while (size < chunkSize && searchFrontier.Count > 0) {
			HexCell current = searchFrontier.Dequeue();
			current.TerrainTypeIndex = 1;
			size += 1;

			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
				HexCell neighbor = current.GetNeighbor(d);
				if (neighbor && neighbor.SearchPhase < searchFrontierPhase) {
					neighbor.SearchPhase = searchFrontierPhase;
					neighbor.Distance = 0;
					neighbor.SearchHeuristic = 0;
					searchFrontier.Enqueue(neighbor);
				}
			}
		}
		searchFrontier.Clear();
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-23/creating-land/line-of-cells.jpg)

*一排单元格。*

我们现在得到一个所需大小的块。如果没有足够的单元格，它最终只会变小。由于边界的填充方式，它总是产生一排向西北移动的单元格。它只有在到达地图边缘时才会改变方向。

### 2.3 保持单元格在一起

大块的土地很少像一条线，即使它们是线，也不总是有相同的方向。要改变块的形状，我们必须改变单元格的优先级。我们可以使用第一个随机单元作为块的中心。然后，所有其他单元格的距离都是相对于该点的。这将赋予更靠近中心的单元格更高的优先级，这应该会导致块围绕其中心而不是成直线生长。

```c#
		searchFrontier.Enqueue(firstCell);
		HexCoordinates center = firstCell.coordinates;

		int size = 0;
		while (size < chunkSize && searchFrontier.Count > 0) {
			HexCell current = searchFrontier.Dequeue();
			current.TerrainTypeIndex = 1;
			size += 1;

			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
				HexCell neighbor = current.GetNeighbor(d);
				if (neighbor && neighbor.SearchPhase < searchFrontierPhase) {
					neighbor.SearchPhase = searchFrontierPhase;
					neighbor.Distance = neighbor.coordinates.DistanceTo(center);
					neighbor.SearchHeuristic = 0;
					searchFrontier.Enqueue(neighbor);
				}
			}
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-23/creating-land/clumped-cells.jpg)

*拥挤的单元格。*

事实上，我们的七个单元格现在总是整齐地排列在一个紧凑的六边形区域中，除非中心单元格恰好位于地图的边缘。让我们也用块大小 30 来试试。

```c#
		RaiseTerrain(30);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-23/creating-land/30-cells.jpg)

*一堆 30 个单元格。*

同样，我们总是得到相同的形状，尽管形成整齐六边形的单元格数量并不合适。因为块的半径更大，它也更有可能离地图边缘足够近，从而被迫形成不同的形状。

### 2.4 随机化土地形状

我们不希望所有块看起来都一样，所以让我们稍微打乱一下单元格优先级。每次我们向边界添加一个邻居单元格时，如果下一个 `Random.value` 数小于某个阈值，请将该单元的启发式设置为 1 而不是 0。让我们使用 0.5 作为阈值，这意味着最有可能有一半的单元格会受到影响。

```c#
					neighbor.Distance = neighbor.coordinates.DistanceTo(center);
					neighbor.SearchHeuristic = Random.value < 0.5f ? 1: 0;
					searchFrontier.Enqueue(neighbor);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-23/creating-land/randomized-chunk.jpg)

*抖动的大块。*

通过增加单元格的搜索启发式，我们确保它的访问时间比预期的要晚。这会导致距离中心一步远的其他单元格被更早地访问，除非它们的启发式也得到了提高。这意味着，如果我们将所有单元格的启发式增加相同的量，就不会有任何效果。因此，阈值 1 不会产生任何影响，就像阈值 0 一样。0.8 的阈值相当于 0.2。因此，0.5 的概率使搜索过程最为紧张。

哪种抖动概率最好取决于你瞄准的地形类型，所以让我们对其进行配置。在生成器中添加一个公共浮点数 `jitterProbability` 字段，将 `Range` 属性限制为0-0.5。给它一个默认值，等于其范围的平均值，即 0.25。这允许我们通过 Unity 检查器窗口配置生成器。

```c#
	[Range(0f, 0.5f)]
	public float jitterProbability = 0.25f;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-23/creating-land/jitter-probability.png)

*抖动概率。*

> **通过游戏内的用户界面进行配置怎么样？**
>
> 这是可能的，大多数游戏都是这样做的。在本教程中，我不会为它添加游戏内 UI，但这并不能阻止你。然而，我们最终将为我们的生成器提供相当多的配置选项。所以在设计 UI 时要记住这一点。你可以决定等到你知道所有的选择。此时，您可能还会决定使用不同的约束、不同的术语，并限制向玩家公开的选项。

现在使用这个概率而不是固定值来决定启发式是否应该设置为 1。

```c#
					neighbor.SearchHeuristic =
						Random.value < jitterProbability ? 1: 0;
```

我们使用启发式值 0 和 1。虽然你也可以使用更大的启发式值，但这会大大加剧块的变形，可能会把它变成一堆丝带。

### 2.5 提高多个块

我们不仅限于生产一块土地。例如，将 `RaiseTerrain` 的调用放在一个循环中，这样我们就得到了五个块。

```c#
		for (int i = 0; i < 5; i++) {
			RaiseTerrain(30);
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-23/creating-land/multiple-chunks.jpg)

*五块。*

尽管我们现在正在生成 5 个 30 大小的块，但我们不能保证获得 150 个单元的土地。由于每个块都是单独创建的，它们彼此不知道，因此可以重叠。这很好，因为它可以产生比一堆孤立的块更多样化的景观。

为了使土地更加多样化，我们还可以改变每块土地的大小。添加两个整数字段来控制允许的最小和最大块大小。给它们一个相当大的范围，比如 20 到 200。我已将默认最小值设置为 30，默认最大值设置为 100。

```c#
	[Range(20, 200)]
	public int chunkSizeMin = 30;

	[Range(20, 200)]
	public int chunkSizeMax = 100;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-23/creating-land/chunk-size.png)

*块大小范围。*

调用 `RaiseTerrain` 时，使用这些字段随机确定块大小。

```c#
			RaiseTerrain(Random.Range(chunkSizeMin, chunkSizeMax + 1));
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-23/creating-land/random-chunk-size.jpg)

*在中等大小的地图上，有五个大小随机的块。*

### 2.6 制造足够的土地

在这一点上，我们无法控制产生多少土地。虽然我们可以为块的数量添加一个配置选项，但块的大小仍然是随机的，它们可能会重叠一点或很多。因此，区块的数量并不能保证地图上有多少最终成为陆地。因此，让我们添加一个选项来直接控制土地百分比，用整数表示。由于 100% 的土地或水并不有趣，请将其范围设置为 5-95，默认值为 50。

```c#
	[Range(5, 95)]
	public int landPercentage = 50;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-23/creating-land/land-percentage.png)

*土地百分比。*

为了确保我们最终获得所需数量的土地，我们只需继续开垦大片土地，直到我们有足够的土地。这要求我们跟踪我们的进展，这使得土地生成变得更加复杂。因此，让我们用调用一个新的 `CreateLand` 方法来替换当前引发一些块的循环。该方法做的第一件事是计算有多少单元格必须变成陆地。这是我们的土地预算。

```c#
	public void GenerateMap (int x, int z) {
		…
//		for (int i = 0; i < 5; i++) {
//			RaiseTerrain(Random.Range(chunkSizeMin, chunkSizeMax + 1));
//		}
		CreateLand();
		for (int i = 0; i < cellCount; i++) {
			grid.GetCell(i).SearchPhase = 0;
		}
	}

	void CreateLand () {
		int landBudget = Mathf.RoundToInt(cellCount * landPercentage * 0.01f);
	}
```

只要还有土地预算，`CreateLand` 就会调用 `RaiseTerrain`。为防止超出预算，请调整 `RaiseTerrain`，使其将预算作为额外参数。一旦完成，它应该退还剩余的预算。

```c#
//	void RaiseTerrain (int chunkSize) {
	int RaiseTerrain (int chunkSize, int budget) {
		…
		return budget;
	}
```

每当一个单元格被移出边境变成陆地时，预算都应该减少。如果在那之后花费了全部预算，我们就必须中止搜索并缩短搜索时间。确保只有在当前单元格尚未成为陆地的情况下才这样做。

```c#
		while (size < chunkSize && searchFrontier.Count > 0) {
			HexCell current = searchFrontier.Dequeue();
			if (current.TerrainTypeIndex == 0) {
				current.TerrainTypeIndex = 1;
				if (--budget == 0) {
					break;
				}
			}
			size += 1;
			
			…
		}
```

现在，只要有预算，`CreateLand` 就可以继续抬升土地。

```c#
	void CreateLand () {
		int landBudget = Mathf.RoundToInt(cellCount * landPercentage * 0.01f);
		while (landBudget > 0) {
			landBudget = RaiseTerrain(
				Random.Range(chunkSizeMin, chunkSizeMax + 1), landBudget
			);
		}
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-23/creating-land/50-percent-land.jpg)

*地图上正好有一半是陆地。*

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-23/creating-land/creating-land.unitypackage)

## 3 用立面雕刻

土地不仅仅是一块仅由海岸线定义的平板。它也可能有不同的海拔，包括丘陵、山脉、山谷、湖泊等。由于缓慢移动的构造板块之间的相互作用，存在较大的海拔差异。虽然我们不打算模拟这个，但我们的大块土地有点像这些板块。我们的块不会移动，但它们确实重叠。这是我们可以利用的东西。

### 3.1 向上推土地

每一块都代表了从海底隆起的一部分陆地。因此，让我们始终递增 `RaiseTerrain` 中正在处理的当前单元格的标高，看看会发生什么。

```c#
			HexCell current = searchFrontier.Dequeue();
			current.Elevation += 1;
			if (current.TerrainTypeIndex == 0) {
				…
			}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-23/sculpting-with-elevation/elevation.jpg)

*高地。*

我们正在上升，但很难看清。我们可以通过为每个标高使用不同的地形类型，如地质分层，来让它变得明显。这只是为了让它变得明显，所以我们可以简单地使用标高作为地形索引。

>**当海拔超过地形类型的数量时会发生什么？**
>
>着色器将使用纹理数组中的最后一个纹理。在我们的例子中，雪是最后一种地形类型，所以我们会得到一条雪线。

让我们创建一个单独的 `SetTerrainType` 方法来一次性设置所有地形类型，而不是每次单元的高程变化时都更新其地形类型。

```c#
	void SetTerrainType () {
		for (int i = 0; i < cellCount; i++) {
			HexCell cell = grid.GetCell(i);
			cell.TerrainTypeIndex = cell.Elevation;
		}
	}
```

在创建土地后调用此方法。

```c#
	public void GenerateMap (int x, int z) {
		…
		CreateLand();
		SetTerrainType();
		…
	}
```

现在 `RaiseTerrain` 不再需要担心地形类型，可以专注于海拔。这需要改变其逻辑。当当前单元格的新高度为 1 时，它刚刚变成陆地，因此预算减少，这可能会结束该块的增长。

```c#
			HexCell current = searchFrontier.Dequeue();
			current.Elevation += 1;
			if (current.Elevation == 1 && --budget == 0) {
				break;
			}
			
//			if (current.TerrainTypeIndex == 0) {
//				current.TerrainTypeIndex = 1;
//				if (--budget == 0) {
//					break;
//				}
//			}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-23/sculpting-with-elevation/stratification.jpg)

*分层。*

### 3.2 加水

让我们通过将所有单元格的水位设置为 1 来明确哪些单元格是陆地还是水。在创建土地之前，在 `GenerateMap` 中执行此操作。

```c#
	public void GenerateMap (int x, int z) {
		cellCount = x * z;
		grid.CreateMap(x, z);
		if (searchFrontier == null) {
			searchFrontier = new HexCellPriorityQueue();
		}
		for (int i = 0; i < cellCount; i++) {
			grid.GetCell(i).WaterLevel = 1;
		}
		CreateLand();
		…
	}
```

现在我们可以使用所有地形类型来表示土地层。所有水下单元格都是沙子，最低的陆地单元格也是如此。这是通过从高程中减去水位并将其用作地形类型指数来实现的。

```c#
	void SetTerrainType () {
		for (int i = 0; i < cellCount; i++) {
			HexCell cell = grid.GetCell(i);
			if (!cell.IsUnderwater) {
				cell.TerrainTypeIndex = cell.Elevation - cell.WaterLevel;
			}
		}
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-23/sculpting-with-elevation/water.jpg)

*土地和水。*

### 3.3 提高水位

我们并不局限于 1 的固定水位。让我们通过一个范围为 1-5、默认值为 3 的公共字段进行配置。使用此级别初始化单元格。

```c#
	[Range(1, 5)]
	public int waterLevel = 3;
	
	…
	
	public void GenerateMap (int x, int z) {
		…
		for (int i = 0; i < cellCount; i++) {
			grid.GetCell(i).WaterLevel = waterLevel;
		}
		…
	}
```

![inspector](https://catlikecoding.com/unity/tutorials/hex-map/part-23/sculpting-with-elevation/water-level.png)
![scene](https://catlikecoding.com/unity/tutorials/hex-map/part-23/sculpting-with-elevation/too-much-water.jpg)

*水位3。*

当水位达到 3 时，我们最终得到的土地比预期的要少得多。这是因为 RaiseTerrain 仍然假设水位为 1。让我们解决这个问题。

```c#
			HexCell current = searchFrontier.Dequeue();
			current.Elevation += 1;
			if (current.Elevation == waterLevel && --budget == 0) {
				break;
			}
```

使用更高水位的效果是单元格不会立即变成陆地。当水位为2时，第一块仍然完全在水下。海底已经上升，但仍被淹没。只有当至少两个块重叠时，才会形成陆地。水位越高，需要堆放的大块土地就越多。其结果是，更高的水位使土地更加不稳定。此外，当需要更多的块时，它们更有可能堆积在已经存在的土地上，这使得山脉更常见，平地更罕见，就像使用较小的块一样。

![2](https://catlikecoding.com/unity/tutorials/hex-map/part-23/sculpting-with-elevation/water-level-2.jpg) ![3](https://catlikecoding.com/unity/tutorials/hex-map/part-23/sculpting-with-elevation/water-level-3.jpg) ![4](https://catlikecoding.com/unity/tutorials/hex-map/part-23/sculpting-with-elevation/water-level-4.jpg) ![5](https://catlikecoding.com/unity/tutorials/hex-map/part-23/sculpting-with-elevation/water-level-5.jpg)

*水位 2-5，始终有 50% 的土地。*

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-23/sculpting-with-elevation/sculpting-with-elevation.unitypackage)

## 4 垂直移动

到目前为止，我们一次只向上提升了一个标高，但并不局限于此。

### 4.1 高高抬起分块

虽然每个区块都会将其单元格的高度增加一级，但也可能出现悬崖。当两个块的边缘接触时，就会发生这种情况。这可能会形成孤立的悬崖，但更长的悬崖线很少见。通过将块的高度增加一步以上，我们可以使这些更常见。但我们应该只对其中的一小部分进行这样的操作。如果所有地块都是被高抬起，地形将变得非常难以导航。因此，让我们使用概率字段对其进行配置，默认值为 0.25。

```c#
	[Range(0f, 1f)]
	public float highRiseProbability = 0.25f;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-23/vertical-movement/high-rise-probability.png)

*高抬升概率高。*

虽然我们可以对高抬升使用任何高程增加，但它很快就会失控。2 的高差已经形成了悬崖，所以这就足够了。因为这使得跳过与水位相等的高度成为可能，所以我们必须改变我们确定单元格已经变成陆地的方式。如果它曾经低于水位，但现在处于同一水位或高于水位，那么我们就制作了一个新的陆地单元。

```c#
		int rise = Random.value < highRiseProbability ? 2 : 1;
		int size = 0;
		while (size < chunkSize && searchFrontier.Count > 0) {
			HexCell current = searchFrontier.Dequeue();
			int originalElevation = current.Elevation;
			current.Elevation = originalElevation + rise;
			if (
				originalElevation < waterLevel &&
				current.Elevation >= waterLevel && --budget == 0
			) {
				break;
			}
			size += 1;
			
			…
		}
```

![25](https://catlikecoding.com/unity/tutorials/hex-map/part-23/vertical-movement/high-rise-25.jpg) ![50](https://catlikecoding.com/unity/tutorials/hex-map/part-23/vertical-movement/high-rise-50.jpg) ![75](https://catlikecoding.com/unity/tutorials/hex-map/part-23/vertical-movement/high-rise-75.jpg) ![100](https://catlikecoding.com/unity/tutorials/hex-map/part-23/vertical-movement/high-rise-100.jpg)

*高上升概率 0.25、0.50、0.75 和 1。*

### 4.2 下沉的土地

土地并不总是上涨，有时也会下跌。当陆地下沉到足够低时，它就会淹没并消失。我们目前没有这样做。因为我们只向上推块，所以土地往往看起来像是一堆相当圆的区域混合在一起。如果我们有时向下推一块，我们最终会得到更多不同的形状。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-23/vertical-movement/large-map.jpg)

*大地图，没有沉没的土地。*

控制我们下沉的频率可以用另一个概率场来完成。因为下沉会破坏土地，我们应该始终让它不太可能下沉，而不是上升。否则，如果我们能达到预期的土地百分比，可能需要很长时间。因此，让我们使用最大吸收概率 0.4，默认值为 0.2。

```c#
	[Range(0f, 0.4f)]
	public float sinkProbability = 0.2f;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-23/vertical-movement/sink-probability.png)

*下沉概率。*

下沉一块类似于抬起一块，但有一些区别。因此，复制 `RaiseTerrain` 方法并将其名称更改为 `SinkTerrain`。我们不需要确定一个上升的量，而是需要一个下降的量，这可以使用相同的逻辑。同时，检查我们是否穿过水面的比较必须颠倒过来。此外，在下沉地形时，我们不受预算的限制。相反，每个失去的土地单元都会收回已花费的预算，所以我们应该增加它并继续下去。

```c#
	int SinkTerrain (int chunkSize, int budget) {
		…

		int sink = Random.value < highRiseProbability ? 2 : 1;
		int size = 0;
		while (size < chunkSize && searchFrontier.Count > 0) {
			HexCell current = searchFrontier.Dequeue();
			int originalElevation = current.Elevation;
			current.Elevation = originalElevation - sink;
			if (
				originalElevation >= waterLevel &&
				current.Elevation < waterLevel
//				&& --budget == 0
			) {
//				break;
				budget += 1;
			}
			size += 1;

			…
		}
		searchFrontier.Clear();
		return budget;
	}
```

`CreateLand` 循环中的每次迭代，我们现在应该根据下沉概率来提高或下沉一块土地。

```c#
	void CreateLand () {
		int landBudget = Mathf.RoundToInt(cellCount * landPercentage * 0.01f);
		while (landBudget > 0) {
			int chunkSize = Random.Range(chunkSizeMin, chunkSizeMax - 1);
			if (Random.value < sinkProbability) {
				landBudget = SinkTerrain(chunkSize, landBudget);
			}
			else {
				landBudget = RaiseTerrain(chunkSize, landBudget);
			}
		}
	}
```

> **那不应该是 `chunkSize + 1` 吗？**
>
> 是的，那是个 bug。这并没有太大区别，所以我决定保持原样，这样生成的地图的所有截图都是一样的。

![10](https://catlikecoding.com/unity/tutorials/hex-map/part-23/vertical-movement/sink-10.jpg) ![20](https://catlikecoding.com/unity/tutorials/hex-map/part-23/vertical-movement/sink-20.jpg) ![30](https://catlikecoding.com/unity/tutorials/hex-map/part-23/vertical-movement/sink-30.jpg) ![40](https://catlikecoding.com/unity/tutorials/hex-map/part-23/vertical-movement/sink-40.jpg)

*下沉概率为 0.1、0.2、0.3 和 0.4。*

### 4.3 限制高度

在这一点上，我们可能会堆叠许多块，有时是多个高抬升，其中一部分可能会下沉，然后再次上升。这可能会产生非常高的海拔，有时也会产生非常低的海拔，尤其是在需要高土地百分比的情况下。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-23/vertical-movement/extreme-elevation.jpg)

*海拔极高，90% 为陆地。*

为了控制标高，让我们添加一个可配置的最小值和最大值。合理的最小值可能在 -4 到 0 之间，而可接受的最大值可能在 6 到 10 之间。将默认值设置为 −2 和 8。手动编辑地图时，这些超出了允许的范围，因此您可能需要调整编辑器 UI 的滑块，也可能不需要。

```c#
	[Range(-4, 0)]
	public int elevationMinimum = -2;

	[Range(6, 10)]
	public int elevationMaximum = 8;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-23/vertical-movement/elevation-limits.png)

*最低和最高标高。*

在 `RaiseTerrain` 中，我们现在应该确保我们不会超过最大允许高度。我们将通过检查当前单元格的新高度是否会过高来实现这一点。如果是这样，我们跳过它，不调整它的高度，也不添加它的邻居。这将导致大块物避开已经处于最高点的区域，在它们周围生长。

```c#
			HexCell current = searchFrontier.Dequeue();
			int originalElevation = current.Elevation;
			int newElevation = originalElevation + rise;
			if (newElevation > elevationMaximum) {
				continue;
			}
			current.Elevation = newElevation;
			if (
				originalElevation < waterLevel &&
				newElevation >= waterLevel && --budget == 0
			) {
				break;
			}
			size += 1;
```

在 `SinkTerrain` 中执行同样的操作，但要达到最低海拔。

```c#
			HexCell current = searchFrontier.Dequeue();
			int originalElevation = current.Elevation;
			int newElevation = current.Elevation - sink;
			if (newElevation < elevationMinimum) {
				continue;
			}
			current.Elevation = newElevation;
			if (
				originalElevation >= waterLevel &&
				newElevation < waterLevel
			) {
				budget += 1;
			}
			size += 1;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-23/vertical-movement/limited-elevation.jpg)

*海拔高度有限，90% 为陆地。*

### 4.4 存储负标高

目前，我们的保存和加载代码无法处理负海拔。这是因为我们将标高存储为 byte。保存时，负数将转换为较大的正数。因此，保存并加载生成的地图可能会导致一些原本被淹没的非常高的单元格出现。

我们可以通过将其存储为整数而不是字节来支持负高度。然而，我们仍然不需要支持许多标高。我们还可以抵消我们存储的值，增加 127。这使得使用单个字节正确存储 -127~128 范围内的高程成为可能。让我们这样做。调整 `HexCell.Save` 相应地保存。

```c#
	public void Save (BinaryWriter writer) {
		writer.Write((byte)terrainTypeIndex);
		writer.Write((byte)(elevation + 127));
		…
	}
```

当我们改变了存储地图数据的方式时，将 `SaveLoadMenu.mapFileVersion` 递增到 4。

```c#
	const int mapFileVersion = 4;
```

最后，调整 `HexCell.Load` ，使其从版本 4 文件加载的立面中减去 127。

```c#
	public void Load (BinaryReader reader, int header) {
		terrainTypeIndex = reader.ReadByte();
		ShaderData.RefreshTerrain(this);
		elevation = reader.ReadByte();
		if (header >= 4) {
			elevation -= 127;
		}
		…
	}
```

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-23/vertical-movement/vertical-movement.unitypackage)

## 5 重建同一张地图

到目前为止，我们已经可以创建各种各样的地图。每次我们生成一个新的，结果都是随机的。我们只能通过配置选项控制地图的特征，而不能控制其确切形状。但有时我们想再次重建完全相同的地图。例如，与他人分享一张漂亮的地图。或者手动编辑后重新开始。它在开发过程中也很有用。所以，让我们让这成为可能。

### 5.1 使用种子

我们正在使用 `Random.Range` 和 `Random.value` 使地图生成过程不可预测。为了再次得到相同的伪随机序列，我们必须使用相同的种子值。我们之前在 `HexMetrics.InitializeHashGrid` 中已经使用过这种方法。它首先存储数字生成器的当前状态，用特定的种子对其进行初始化，然后将其恢复到旧状态。我们可以对 `HexMapGenerator.GenerateMap` 使用相同的方法。同样，我们记住旧状态，并在完成后恢复它，这样我们就不会影响使用 `Random` 的其他任何东西。

```c#
	public void GenerateMap (int x, int z) {
		Random.State originalRandomState = Random.state;
		…
		Random.state = originalRandomState;
	}
```

接下来，我们将公开用于生成最后一个地图的种子。这是通过公共整数字段完成的。

```c#
	public int seed;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-23/recreating-the-same-map/seed.png)

*展示种子。*

现在我们需要一个种子值来初始化 `Random`。为了创建随机地图，我们必须使用随机种子。最直接的方法似乎是使用 `Random.Range` 生成任意种子值。为了不影响原始随机状态，我们必须在存储它之后这样做。

```c#
	public void GenerateMap (int x, int z) {
		Random.State originalRandomState = Random.state;
		seed = Random.Range(0, int.MaxValue);
		Random.InitState(seed);
		
		…
	}
```

当我们完成后恢复随机状态时，如果我们立即生成另一个地图，我们最终将得到相同的种子值。此外，我们不知道原始随机状态是如何初始化的。因此，虽然它可以作为一个任意的起点，但我们需要更多的东西来随机化每次调用。

有多种方法可以初始化随机数生成器。在这种情况下，可以只组合几个变化很大的任意值，因此不太可能再次生成相同的地图。例如，让我们使用以滴答数表示的系统时间的较低 32 位，加上我们应用程序的当前运行时间。将这些值与位异或运算结合起来，这样我们就不会得到一个明显递增的数字。

```c#
		seed = Random.Range(0, int.MaxValue);
		seed ^= (int)System.DateTime.Now.Ticks;
		seed ^= (int)Time.unscaledTime;
		Random.InitState(seed);
```

由此产生的数字可能是负数，这对于公开暴露的种子来说并不好。我们可以通过用最大整数值逐位掩码它来强制它为正，这将符号位设置为零。

```c#
		seed ^= (int)Time.unscaledTime;
		seed &= int.MaxValue;
		Random.InitState(seed);
```

### 5.2 重复使用种子

我们仍在生成随机地图，但现在我们可以看到每个地图使用了哪个种子值。为了再次创建相同的地图，我们必须指示生成器重用其种子值，而不是创建新的种子值。我们将通过布尔字段添加一个切换来实现这一点。

```c#
	public bool useFixedSeed;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-23/recreating-the-same-map/fixed-seed.png)

*使用固定种子的选项。*

当应该使用固定种子时，我们只需跳过在 `GenerateMap` 中生成新种子。如果我们不手动编辑种子字段，这将导致再次生成完全相同的地图。

```c#
		Random.State originalRandomState = Random.state;
		if (!useFixedSeed) {
			seed = Random.Range(0, int.MaxValue);
			seed ^= (int)System.DateTime.Now.Ticks;
			seed ^= (int)Time.time;
			seed &= int.MaxValue;
		}
		Random.InitState(seed);
```

现在可以复制你喜欢的地图的种子值并将其存储在某个地方，以便以后再次生成。请记住，只有使用完全相同的生成器设置，才能获得相同的地图。所以地图大小相同，但也有其他所有配置选项。即使对其中一个概率进行微小的更改，也会产生完全不同的地图。所以除了种子，你还必须记住所有的设置。

![0](https://catlikecoding.com/unity/tutorials/hex-map/part-23/recreating-the-same-map/map-0.jpg) ![929396788](https://catlikecoding.com/unity/tutorials/hex-map/part-23/recreating-the-same-map/map-929396788.jpg)
*带有种子 0 和 929396788 的大地图，默认设置。*

下一个教程是[区域和侵蚀](https://catlikecoding.com/unity/tutorials/hex-map/part-24/)。

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-23/recreating-the-same-map/recreating-the-same-map.unitypackage)

[PDF](https://catlikecoding.com/unity/tutorials/hex-map/part-23/Hex-Map-23.pdf)

# Hex Map 24：区域和侵蚀

发布于 2017-12-14

https://catlikecoding.com/unity/tutorials/hex-map/part-24/

*在地图周围添加一圈水。*
*将地图拆分为多个区域。*
*通过侵蚀来磨掉悬崖。*
*移动土地以平滑地形。*

这是关于[六边形地图](https://catlikecoding.com/unity/tutorials/hex-map/)的系列教程的第 24 部分。在[上一部分](https://catlikecoding.com/unity/tutorials/hex-map/part-23/)中，我们为过程图生成奠定了基础。这一次，我们将限制土地可能出现的位置，并使其受到侵蚀的影响。

本教程是用 Unity 2017.1.0 编写的。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-24/tutorial-image.jpg)

*分割土地并使其变得平坦。*

## 1 地图边界

因为我们随机推起大块的土地，所以土地最终可能会接触到地图的边缘。这可能并不可取。一张以水为边界的地图包含一个天然屏障，可以让玩家远离边缘。因此，如果我们能防止土地上升到接近边缘的水位以上，那就太好了。

### 1.1 边界大小

应该允许土地离地图边缘有多近？这个问题没有通用的答案，所以让我们将其配置为可配置的。我们将通过在 `HexMapGenerator` 组件中添加两个滑块来实现这一点，一个用于 X 边的边界，另一个用于 Z 边的边界。这使得在一个维度上使用更宽的边框成为可能，或者只为单个维度设置边框。让我们使用从 0 到 10 个单元格的范围，其中 5 是这两个单元格的默认值。

```c#
	[Range(0, 10)]
	public int mapBorderX = 5;

	[Range(0, 10)]
	public int mapBorderZ = 5;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-24/map-border/map-border-sliders.png)

*地图边框滑块。*

### 1.2 约束块中心

没有边框，所有单元格都有效。当边界生效时，最小有效偏移坐标会增加，而最大有效坐标会减少。由于我们在生成块时需要知道有效范围，让我们用四个整数字段来跟踪这个范围。

```c#
	int xMin, xMax, zMin, zMax;
```

在 `GenerateMap` 中创建土地之前初始化约束。我们将使用这些值作为调用 `Random.Range` 的参数，因此最大值实际上是独占的。如果没有边界，它们等于维度的单元格数，所以不是负 1。

```c#
	public void GenerateMap (int x, int z) {
		…
		for (int i = 0; i < cellCount; i++) {
			grid.GetCell(i).WaterLevel = waterLevel;
		}
		xMin = mapBorderX;
		xMax = x - mapBorderX;
		zMin = mapBorderZ;
		zMax = z - mapBorderZ;
		CreateLand();
		…
	}
```

我们不会严格执行土地不会出现在边界边缘之外的规定，因为这只会造成硬边界。相反，我们将只约束用于开始生成块的单元格。因此，块的粗略中心受到约束，但块的一部分可以延伸到边界区域。这是通过调整 `GetRandomCell` 来实现的，使其在允许的偏移范围内拾取一个单元格。

```c#
	HexCell GetRandomCell () {
//		return grid.GetCell(Random.Range(0, cellCount));
		return grid.GetCell(Random.Range(xMin, xMax), Random.Range(zMin, zMax));
	}
```

![0x0](https://catlikecoding.com/unity/tutorials/hex-map/part-24/map-border/map-border-0.jpg) ![5x5](https://catlikecoding.com/unity/tutorials/hex-map/part-24/map-border/map-border-5.jpg) ![10x10](https://catlikecoding.com/unity/tutorials/hex-map/part-24/map-border/map-border-10.jpg) ![0x10](https://catlikecoding.com/unity/tutorials/hex-map/part-24/map-border/map-border-0-10.jpg)

*地图边框 0×0、5×5、10×10 和 0×10。*

在所有地图设置均为默认值的情况下，5 的边界将可靠地防止陆地接触地图边缘。然而，这并不能保证。土地可能会靠近边缘，有时会在多个地方碰到它。

陆地穿越整个边界区域的可能性取决于边界大小和最大块大小。没有抖动，块是六边形的。具有半径的全六边形 $r$ 包含 $3r^2+3r+1$ 个单元格。如果有半径等于边界大小的六边形，那么它们就可以穿过它。半径为 5 的完整六边形包含 91 个单元格。由于默认的最大值是每个块 100 个单元，这意味着陆地可以桥接 5 个单元格的间隙，特别是在有抖动的情况下。为了保证不会发生这种情况，请减小最大块大小或增加边界大小。

> **你如何得出六边形区域有多少个单元格？**
>
> 在半径 0 处，我们处理的是一个单独的单元格。这就是 1 的由来。在半径 1 处，中心周围还有六个额外的单元格，因此 $6 + 1$。你可以把这六个单元格看作是接触中心的六个三角形的尖端。在半径 2 处，这些三角形中添加了第二行，因此每个三角形多了两个单元格，总共 $6(1+2)+1$。在半径 3 处，添加了第三行，因此每个三角形多了三个单元格，总共为 $6(1+2+3)+1$，等等。所以一般来说，公式是 $6(\displaystyle\sum_{i=1}^r i)+1=6(\frac{r(r+1)}{2})+1=3r(r+1)+1=3r^2+3r+1$

为了清楚地看到这一点，您可以将块大小固定为 200。由于半径为 8 的完整六边形包含 217 个单元格，因此很可能会碰到地图边缘。至少，当使用默认边框大小 5 时。将边界增加到 10 将大大降低这种可能性。

![5](https://catlikecoding.com/unity/tutorials/hex-map/part-24/map-border/chunk-size-200-5.jpg) ![10](https://catlikecoding.com/unity/tutorials/hex-map/part-24/map-border/chunk-size-200-10.jpg)

*区块大小固定为 200，地图边界为 5 和 10。*

### 1.3 盘古大陆

请注意，当您在保持土地百分比不变的情况下增加地图边框时，您会强制土地在较小的区域内形成。因此，默认的大地图可能会产生一个巨大的陆地——一个超级大陆或泛大陆——可能还有几个小岛。增加边界大小将使这种情况更有可能发生，直到你几乎可以保证得到一个超级大陆。然而，当土地百分比过高时，大部分可用区域都会被填满，最终会形成一个看起来很矩形的陆地。为了防止这种情况发生，你可以降低土地百分比。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-24/map-border/pangea.jpg)

*40% 的土地有地图边框 10。*

> **盘古（Pangea）这个名字是从哪里来的？**
>
> 这是很久以前地球上最后一个已知的超级大陆的名字。这个名字也写在泛古陆上。它来源于希腊语“潘”（pan）和“盖亚”（Gaia）。它意味着整个地球母亲或整个土地。

### 1.4 防范不可能的地图

我们只需继续种植大块土地，直到达到所需的土地面积，就可以获得所需数量的土地。这是有效的，因为最终我们可以将每个单元格都提升到水位以上。然而，当使用地图边框时，可能无法到达每个单元格。当需要过高的土地百分比时，这将导致生成器试图——但失败了——永远地开垦更多的土地，陷入无限循环。这将冻结我们的应用程序，这不应该发生。

我们无法提前可靠地检测到不可能的配置，但我们可以防止无限循环。只需记录我们在 `CreateLand` 中循环了多少次。如果我们迭代了大量的次数，我们可能会陷入困境，应该停下来。

对于大型地图来说，多达一千次迭代似乎是可以接受的，但一万次确实是荒谬的。所以，让我们把它作为一个临界点。

```c#
	void CreateLand () {
		int landBudget = Mathf.RoundToInt(cellCount * landPercentage * 0.01f);
//		while (landBudget > 0) {
		for (int guard = 0; landBudget > 0 && guard < 10000; guard++) {
			int chunkSize = Random.Range(chunkSizeMin, chunkSizeMax - 1);
			…
		}
	}
```

如果我们最终得到一个退化的地图，那么经过 10000 次迭代就不会花那么多时间，因为大多数单元格会很快达到最大高度，从而阻止新的块生长。

即使在中止循环后，我们仍然有一个有效的地图。它只是没有理想的土地面积，看起来也不太有趣。让我们记录一个关于这一点的警告，报告我们没有用完多少土地预算。

```c#
void CreateLand () {
		…
		if (landBudget > 0) {
			Debug.LogWarning("Failed to use up " + landBudget + " land budget.");
		}
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-24/map-border/failed-to-use-land-budget.jpg)

*95% 的土地与地图边界为 10，未能用完预算。*

> **为什么失败的地图仍然具有多样性？**
>
> 海岸线多种多样，因为一旦出生区内的海拔过高，新的块体就无法向外生长。同样的概念可以防止大块土地生长成尚未达到最高海拔的小块土地，而这些小块土地恰好被错过了。此外，通过下沉块不断增加多样性。

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-24/map-border/map-border.unitypackage)

## 2 划分地图

现在我们有了地图边框，我们有效地将地图分为两个不同的区域。边界区域和块生成区域。由于生成区域才是真正重要的，我们可以将其视为单区域场景。该区域并没有覆盖整个地图。但如果这是可能的，没有什么能阻止我们将地图分割成多个不连接的生成区域。这将有可能迫使多个陆地独立形成，代表不同的大陆。

### 2.1 地图区域

让我们从用结构体表示单个地图区域开始。这使得与多个地区合作变得更加容易。为此创建一个 `MapRegion` 结构，它只包含边界字段。由于我们不会在 `HexMapGenerator` 之外使用此结构，因此我们可以在该类中将其定义为私有内部结构。然后，可以用单个 `MapRegion` 字段替换四个整数字段。

```c#
//	int xMin, xMax, zMin, zMax;
	struct MapRegion {
		public int xMin, xMax, zMin, zMax;
	}

	MapRegion region;
```

为了保持正常工作，我们现在必须在最小最大字段前加上 `region`。在 `GenerateMap` 中。

```c#
		region.xMin = mapBorderX;
		region.xMax = x - mapBorderX;
		region.zMin = mapBorderZ;
		region.zMax = z - mapBorderZ;
```

还有 `GetRandomCell`。

```c#
	HexCell GetRandomCell () {
		return grid.GetCell(
			Random.Range(region.xMin, region.xMax),
			Random.Range(region.zMin, region.zMax)
		);
	}
```

### 2.2 多个区域

要支持多个区域，请将单个 `MapRegion` 字段替换为区域列表。

```c#
//	MapRegion region;
	List<MapRegion> regions;
```

此时，添加一个专门的方法来创建区域是一个好主意。它应该创建所需的列表，或者如果已经有列表，则将其清除。之后，像我们之前所做的那样定义单个区域并将其添加到列表中。

```c#
	void CreateRegions () {
		if (regions == null) {
			regions = new List<MapRegion>();
		}
		else {
			regions.Clear();
		}

		MapRegion region;
		region.xMin = mapBorderX;
		region.xMax = grid.cellCountX - mapBorderX;
		region.zMin = mapBorderZ;
		region.zMax = grid.cellCountZ - mapBorderZ;
		regions.Add(region);
	}
```

在 `GenerateMap` 中调用此方法，而不是直接创建区域。

```c#
//		region.xMin = mapBorderX;
//		region.xMax = x - mapBorderX;
//		region.zMin = mapBorderZ;
//		region.zMax = z - mapBorderZ;
		CreateRegions();
		CreateLand();
```

要使 `GetRandomCell` 适用于任意区域，请为其指定一个 `MapRegion` 参数。

```c#
	HexCell GetRandomCell (MapRegion region) {
		return grid.GetCell(
			Random.Range(region.xMin, region.xMax),
			Random.Range(region.zMin, region.zMax)
		);
	}
```

`RaiseTerrain` 和 `SinkTerrain` 方法现在必须将正确的区域传递给 `GetRandomCell`。为此，它们还需要每个区域参数。

```c#
	int RaiseTerrain (int chunkSize, int budget, MapRegion region) {
		searchFrontierPhase += 1;
		HexCell firstCell = GetRandomCell(region);
		…
	}

	int SinkTerrain (int chunkSize, int budget, MapRegion region) {
		searchFrontierPhase += 1;
		HexCell firstCell = GetRandomCell(region);
		…
	}
```

`CreateLand` 方法必须确定要为哪个区域提升或下沉块。要平衡地区之间的土地，只需反复循环地区列表即可。

```c#
	void CreateLand () {
		int landBudget = Mathf.RoundToInt(cellCount * landPercentage * 0.01f);
		for (int guard = 0; landBudget > 0 && guard < 10000; guard++) {
			for (int i = 0; i < regions.Count; i++) {
				MapRegion region = regions[i];
				int chunkSize = Random.Range(chunkSizeMin, chunkSizeMax - 1);
				if (Random.value < sinkProbability) {
					landBudget = SinkTerrain(chunkSize, landBudget, region);
				}
				else {
					landBudget = RaiseTerrain(chunkSize, landBudget, region);
				}
			}
		}
		if (landBudget > 0) {
			Debug.LogWarning("Failed to use up " + landBudget + " land budget.");
		}
	}
```

然而，我们也应该注意均匀地分配大块的下沉。这可以通过确定我们是否同时对所有地区进行下沉来实现。

```c#
		for (int guard = 0; landBudget > 0 && guard < 10000; guard++) {
			bool sink = Random.value < sinkProbability;
			for (int i = 0; i < regions.Count; i++) {
				MapRegion region = regions[i];
				int chunkSize = Random.Range(chunkSizeMin, chunkSizeMax - 1);
//				if (Random.value < sinkProbability) {
				if (sink) {
					landBudget = SinkTerrain(chunkSize, landBudget, region);
				}
				else {
					landBudget = RaiseTerrain(chunkSize, landBudget, region);
				}
			}
		}
```

最后，为了确保我们准确地用完土地预算，一旦预算达到零，我们就必须停止这一过程。这可能发生在区域循环中的任何点。因此，将零预算检查转移到内环。事实上，我们可以将这种检查限制在土地被开垦之后，因为沉没一大块永远不会耗尽预算。完成后，我们可以直接退出 `CreateLand` 方法。

```c#
//		for (int guard = 0; landBudget > 0 && guard < 10000; guard++) {
		for (int guard = 0; guard < 10000; guard++) {
			bool sink = Random.value < sinkProbability;
			for (int i = 0; i < regions.Count; i++) {
				MapRegion region = regions[i];
				int chunkSize = Random.Range(chunkSizeMin, chunkSizeMax - 1);
					if (sink) {
					landBudget = SinkTerrain(chunkSize, landBudget, region);
				}
				else {
					landBudget = RaiseTerrain(chunkSize, landBudget, region);
					if (landBudget == 0) {
						return;
					}
				}
			}
		}
```

### 2.3 两个地区

虽然我们现在支持多个区域，但我们仍然只定义了一个区域。让我们通过调整 `CreateRegions` 来改变这一点，使其将地图垂直一分为二。为此，将我们添加的区域的 `xMax` 值减半。然后对 `xMin` 使用相同的值，并对 `xMax` 再次使用原始值，将其用作第二个区域。

```c#
		MapRegion region;
		region.xMin = mapBorderX;
		region.xMax = grid.cellCountX / 2;
		region.zMin = mapBorderZ;
		region.zMax = grid.cellCountZ - mapBorderZ;
		regions.Add(region);
		region.xMin = grid.cellCountX / 2;
		region.xMax = grid.cellCountX - mapBorderX;
		regions.Add(region);
```

此时生成地图没有任何区别。尽管我们定义了两个区域，但它们覆盖的区域与旧的单个区域相同。为了将它们分开，我们必须在它们之间留出空白。我们将通过为区域边界添加滑块来实现这一点，使用与地图边界相同的范围和默认值。

```c#
	[Range(0, 10)]
	public int regionBorder = 5;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-24/partitioning-the-map/region-border.png)

*区域边界滑块。*

由于陆地可以在区域之间的空间的两侧形成，因此更有可能在地图的边缘形成陆桥。为了解决这个问题，我们将使用区域边界在分割线和允许块开始的区域之间定义一个无生成区。这意味着相邻区域之间的距离是区域边界大小的两倍。

要应用区域边界，请将其从第一个区域的 `xMax` 中减去，然后将其添加到第二个区域的 `xMin` 中。

```c#
		MapRegion region;
		region.xMin = mapBorderX;
		region.xMax = grid.cellCountX / 2 - regionBorder;
		region.zMin = mapBorderZ;
		region.zMax = grid.cellCountZ - mapBorderZ;
		regions.Add(region);
		region.xMin = grid.cellCountX / 2 + regionBorder;
		region.xMax = grid.cellCountX - mapBorderX;
		regions.Add(region);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-24/partitioning-the-map/two-regions-vertical.jpg)

*地图垂直分为两个区域。*

使用默认设置，这将生成具有两个明显独立区域的地图，尽管就像单个区域和一个大的地图边框一样，我们不能保证得到两个陆地。大多数时候，它将是两个大大陆，也许每个大陆都有几个岛屿。偶尔，一个地区最终会包含两个或多个大岛。有时，两大洲会通过陆桥连接起来。

当然，也可以水平分割地图，将方法替换为 X 和 Z 维度。让我们从两种可能的方向中随机选择一种。

```c#
		MapRegion region;
		if (Random.value < 0.5f) {
			region.xMin = mapBorderX;
			region.xMax = grid.cellCountX / 2 - regionBorder;
			region.zMin = mapBorderZ;
			region.zMax = grid.cellCountZ - mapBorderZ;
			regions.Add(region);
			region.xMin = grid.cellCountX / 2 + regionBorder;
			region.xMax = grid.cellCountX - mapBorderX;
			regions.Add(region);
		}
		else {
			region.xMin = mapBorderX;
			region.xMax = grid.cellCountX - mapBorderX;
			region.zMin = mapBorderZ;
			region.zMax = grid.cellCountZ / 2 - regionBorder;
			regions.Add(region);
			region.zMin = grid.cellCountZ / 2 + regionBorder;
			region.zMax = grid.cellCountZ - mapBorderZ;
			regions.Add(region);
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-24/partitioning-the-map/two-regions-horizontal.jpg)

*地图水平拆分为两个区域。*

因为我们使用的是宽地图，水平分割会产生更宽更薄的区域。这使得该地区更有可能最终出现多个断开的陆地。

### 2.4 多达四个地区

让我们配置区域的数量，支持 1 到 4 个区域。

```c#
	[Range(1, 4)]
	public int regionCount = 1;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-24/partitioning-the-map/region-count.png)

*区域数量滑块。*

我们可以使用 `switch` 语句来选择要执行的正确区域代码。首先，重新引入单个区域的代码，将其用作默认值，同时保留案例 2 中两个区域的代码。

```c#
		MapRegion region;
		switch (regionCount) {
		default:
			region.xMin = mapBorderX;
			region.xMax = grid.cellCountX - mapBorderX;
			region.zMin = mapBorderZ;
			region.zMax = grid.cellCountZ - mapBorderZ;
			regions.Add(region);
			break;
		case 2:
			if (Random.value < 0.5f) {
				region.xMin = mapBorderX;
				region.xMax = grid.cellCountX / 2 - regionBorder;
				region.zMin = mapBorderZ;
				region.zMax = grid.cellCountZ - mapBorderZ;
				regions.Add(region);
				region.xMin = grid.cellCountX / 2 + regionBorder;
				region.xMax = grid.cellCountX - mapBorderX;
				regions.Add(region);
			}
			else {
				region.xMin = mapBorderX;
				region.xMax = grid.cellCountX - mapBorderX;
				region.zMin = mapBorderZ;
				region.zMax = grid.cellCountZ / 2 - regionBorder;
				regions.Add(region);
				region.zMin = grid.cellCountZ / 2 + regionBorder;
				region.zMax = grid.cellCountZ - mapBorderZ;
				regions.Add(region);
			}
			break;
		}
```

> **什么是 `switch` 语句？**
>
> 它是编写序列 if-else-if-else 语句的替代方法。该 switch 应用于变量，标签用于指示要执行的代码。还有一个 `default` 标签，其功能类似于最后的 `else` 块。每个案例都必须通过 `break` 语句或 `return` 语句终止。
>
> 为了使 `switch` 块清晰易读，通常最好保持案例简短，最好是单个语句或方法调用。对于示例区域代码，我没有费心去做这件事，但如果你想制作更有趣的区域，我建议你使用单独的方法。例如：
>
> ```c#
> switch (regionCount) {
> 			default: CreateOneRegion(); break;
> 			case 2: CreateTwoRegions(); break;
> 			case 3: CreateThreeRegions(); break;
> 			case 4: CreateFourRegions(); break;
> 		}
> ```

三个区域的工作原理与两个区域相似，只是我们使用的是三分之一而不是一半。在这种情况下，水平分割会产生太窄的区域，因此我们只支持垂直分割。还要注意，我们最终得到的区域边界空间是两倍，因此生成块的空间比两个区域少。

```c#
		switch (regionCount) {
		default:
			…
			break;
		case 2:
			…
			break;
		case 3:
			region.xMin = mapBorderX;
			region.xMax = grid.cellCountX / 3 - regionBorder;
			region.zMin = mapBorderZ;
			region.zMax = grid.cellCountZ - mapBorderZ;
			regions.Add(region);
			region.xMin = grid.cellCountX / 3 + regionBorder;
			region.xMax = grid.cellCountX * 2 / 3 - regionBorder;
			regions.Add(region);
			region.xMin = grid.cellCountX * 2 / 3 + regionBorder;
			region.xMax = grid.cellCountX - mapBorderX;
			regions.Add(region);
			break;
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-24/partitioning-the-map/three-regions.jpg)

*三个地区。*

通过组合水平和垂直分割，可以在地图的每个角落创建一个区域，从而完成四个区域。

```c#
		switch (regionCount) {
		…
		case 4:
			region.xMin = mapBorderX;
			region.xMax = grid.cellCountX / 2 - regionBorder;
			region.zMin = mapBorderZ;
			region.zMax = grid.cellCountZ / 2 - regionBorder;
			regions.Add(region);
			region.xMin = grid.cellCountX / 2 + regionBorder;
			region.xMax = grid.cellCountX - mapBorderX;
			regions.Add(region);
			region.zMin = grid.cellCountZ / 2 + regionBorder;
			region.zMax = grid.cellCountZ - mapBorderZ;
			regions.Add(region);
			region.xMin = mapBorderX;
			region.xMax = grid.cellCountX / 2 - regionBorder;
			regions.Add(region);
			break;
		}
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-24/partitioning-the-map/four-regions.jpg)

*四个地区。*

我们在这里使用的方法是划分地图的最直接的方法。它生成了陆地面积大致相等的区域，其多样性可以通过其他地图生成设置进行控制。然而，至少相当明显的是，地图是沿着直线分割的。你想要的控制越多，结果看起来就越不自然。因此，如果出于游戏原因需要多个相当相等的区域，那也没关系。但如果你想要最多样化、最不受约束的土地，你就必须在一个地区凑合着。

话虽如此，还有其他方法来划分地图。你并不局限于直线。您也不限于使用相同大小的区域，也不需要用区域覆盖整个地图。你也可以留下洞。您还可以让区域重叠，或更改区域之间的土地分布。甚至可以为每个地区定义不同的生成器设置——尽管这更复杂——例如确保地图同时显示一个大大陆和一个群岛。

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-24/partitioning-the-map/partitioning-the-map.unitypackage)

## 3 侵蚀

到目前为止，我们生成的所有地图都显得相当粗糙和参差不齐。真实的地形可能看起来像这样，但随着时间的推移，它会变得更加平滑和光滑，其尖锐的特征会因侵蚀而消失。为了改进我们的地图，我们也应该应用这种侵蚀过程。我们将在创建粗糙的土地后，用另一种方法进行此操作。

```c#
	public void GenerateMap (int x, int z) {
		…
		CreateRegions();
		CreateLand();
		ErodeLand();
		SetTerrainType();
		…
	}
	
	…
	
	void ErodeLand () {}
```

### 3.1 侵蚀百分比

时间越长，侵蚀就越严重。因此，我们想要的侵蚀程度不是固定的，它必须是可配置的。至少，没有侵蚀，这是我们迄今为止生成的地图的情况。在最大程度上，存在完全侵蚀，这意味着进一步施加侵蚀力将不再改变地形。因此，侵蚀设置应为 0 到 100 的百分比，我们将使用 50 作为默认值。

```c#
	[Range(0, 100)]
	public int erosionPercentage = 50;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-24/erosion/erosion-slider.png)

*侵蚀滑块。*

### 3.2 寻找易侵蚀单元格

侵蚀使地形更加平坦。在我们的例子中，我们唯一真正陡峭的地形特征是悬崖。所以这些是我们侵蚀过程的目标。如果悬崖存在，侵蚀会使其缩小，直到最终变成斜坡。我们不会进一步夷平斜坡，因为那会产生无趣的地形。要做到这一点，我们必须弄清楚哪些单元格位于悬崖顶部，并降低它们的高度。这些是我们易受侵蚀的单元格。

让我们创建一个方法来确定一个单元格是否可被侵蚀。它通过查看单元格的邻居来做到这一点，直到找到足够大的高度差。由于悬崖需要至少两个高度差，如果一个或多个相邻单元位于其下方至少两步，则该单元是可侵蚀的。如果没有这样的相邻单元，则该单元格是不可侵蚀的。

```c#
	bool IsErodible (HexCell cell) {
		int erodibleElevation = cell.Elevation - 2;
		for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
			HexCell neighbor = cell.GetNeighbor(d);
			if (neighbor && neighbor.Elevation <= erodibleElevation) {
				return true;
			}
		}
		return false;
	}
```

我们可以在 `ErodeLand` 中使用这种方法遍历所有单元格，并在临时列表中跟踪所有易受侵蚀的单元格。

```c#
	void ErodeLand () {
		List<HexCell> erodibleCells = ListPool<HexCell>.Get();
		for (int i = 0; i < cellCount; i++) {
			HexCell cell = grid.GetCell(i);
			if (IsErodible(cell)) {
				erodibleCells.Add(cell);
			}
		}

		ListPool<HexCell>.Add(erodibleCells);
	}
```

一旦我们知道了可侵蚀单元格的总量，我们就可以使用侵蚀百分比来确定应该保留多少可侵蚀单元格。例如，如果百分比为 50，那么我们应该侵蚀单元格，直到最终得到原始量的一半。如果这个百分比是 100，我们不会停止，直到所有易受侵蚀的单元格都消失了。

```c#
	void ErodeLand () {
		List<HexCell> erodibleCells = ListPool<HexCell>.Get();
		for (int i = 0; i < cellCount; i++) {
			…
		}

		int targetErodibleCount =
			(int)(erodibleCells.Count * (100 - erosionPercentage) * 0.01f);

		ListPool<HexCell>.Add(erodibleCells);
	}
```

> **我们难道不应该只计算易受侵蚀的陆地单元格吗？**
>
> 水下也会发生侵蚀。侵蚀有不同的类型，但我们不必担心这些细节，可以用一种通用的方法来解决。

### 3.3 降低单元格数量

让我们从简单的（naive）开始，假设简单地降低易蚀单元格的高度就会使其不再易蚀。如果这是真的，我们只需继续从列表中随机选取单元格，降低它们的高度，然后将它们从列表中删除。我们重复这一过程，直到达到所需的可侵蚀单元格数量。

```c#
		int targetErodibleCount =
			(int)(erodibleCells.Count * (100 - erosionPercentage) * 0.01f);
		
		while (erodibleCells.Count > targetErodibleCount) {
			int index = Random.Range(0, erodibleCells.Count);
			HexCell cell = erodibleCells[index];

			cell.Elevation -= 1;

			erodibleCells.Remove(cell);
		}

		ListPool<HexCell>.Add(erodibleCells);
```

防止易受侵蚀单元格所需的搜索。删除，只需用列表中的最后一个单元格覆盖当前单元格，然后删除最后一个元素。反正我们也不在乎他们的顺序。

```c#
//			erodibleCells.Remove(cell);
			erodibleCells[index] = erodibleCells[erodibleCells.Count - 1];
			erodibleCells.RemoveAt(erodibleCells.Count - 1);
```

![0](https://catlikecoding.com/unity/tutorials/hex-map/part-24/erosion/lowering-0.jpg) ![100](https://catlikecoding.com/unity/tutorials/hex-map/part-24/erosion/lowering-100.jpg)

*幼稚的降低 0% 和 100% 的可蚀单元格，地图种子 1957632474。*

### 3.4 易侵蚀簿记

我们天真的方法确实会造成一些侵蚀，但还远远不够。这是因为单元格在其高度降低一次后可能仍然会受到侵蚀。因此，只有当单元格不再易受侵蚀时，才能将其移除。

```c#
			if (!IsErodible(cell)) {
				erodibleCells[index] = erodibleCells[erodibleCells.Count - 1];
				erodibleCells.RemoveAt(erodibleCells.Count - 1);
			}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-24/erosion/keeping-erodibles.jpg)

*100% 侵蚀，同时将易受侵蚀的单元格保留在列表中。*

这会产生更强烈的侵蚀，但在 100% 使用时，它仍然不能消除所有的悬崖。这是因为当一个单元格的高度降低时，它的邻居之一可能会变得易受侵蚀。因此，我们最终可能会得到比开始时更多的易受侵蚀的单元格。

放下单元格后，我们必须检查它的所有邻居。如果它们现在已经可以被侵蚀，但还没有被列入名单，我们就必须把它们添加进去。

```c#
			if (!IsErodible(cell)) {
				erodibleCells[index] = erodibleCells[erodibleCells.Count - 1];
				erodibleCells.RemoveAt(erodibleCells.Count - 1);
			}
			
			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
				HexCell neighbor = cell.GetNeighbor(d);
				if (
					neighbor && IsErodible(neighbor) &&
					!erodibleCells.Contains(neighbor)
				) {
					erodibleCells.Add(neighbor);
				}
			}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-24/erosion/fully-lowered.jpg)

*所有易受侵蚀的单元格都降低了。*

### 3.5 保护土地

我们的侵蚀过程现在可以继续下去，直到所有的悬崖都被消除。对土地的影响是巨大的。很多土地都消失了，我们最终得到的土地比例比预期的要低得多。这是因为我们从地图上删除了土地。

实际的侵蚀不会破坏物质。它把它从一个地方拿走，放在另一个地方。我们可以做同样的事情。每当我们降低一个单元格时，我们应该升高它的一个邻居。单层升高有效地迁移到较低的单元格。这保留了地图的总高程，只是使其变得平滑。

为了实现这一目标，我们必须决定将被侵蚀的物质转移到哪里。这是我们的侵蚀目标。让我们创建一种方法来确定目标，给定一个我们即将侵蚀的单元格。由于该单元格必须有一个悬崖，因此选择悬崖底部的单元格作为目标是有意义的。但易受侵蚀的单元格可能有多个悬崖。因此，让我们检查所有邻居，并将所有候选人列入临时名单，然后随机选择其中一个。

```c#
	HexCell GetErosionTarget (HexCell cell) {
		List<HexCell> candidates = ListPool<HexCell>.Get();
		int erodibleElevation = cell.Elevation - 2;
		for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
			HexCell neighbor = cell.GetNeighbor(d);
			if (neighbor && neighbor.Elevation <= erodibleElevation) {
				candidates.Add(neighbor);
			}
		}
		HexCell target = candidates[Random.Range(0, candidates.Count)];
		ListPool<HexCell>.Add(candidates);
		return target;
	}
```

在 `ErodeLand` 中，在挑选易蚀单元格后直接确定目标单元格。然后，单元格高度依次递减和递增。这可能会使目标单元格本身变得易受侵蚀，但当我们检查我们刚刚侵蚀的单元格的邻居时，这一点就被覆盖了。

```c#
			HexCell cell = erodibleCells[index];
			HexCell targetCell = GetErosionTarget(cell);

			cell.Elevation -= 1;
			targetCell.Elevation += 1;

			if (!IsErodible(cell)) {
				erodibleCells[index] = erodibleCells[erodibleCells.Count - 1];
				erodibleCells.RemoveAt(erodibleCells.Count - 1);
			}
```

因为我们已经提高了目标单元格，该单元格的一些邻居现在可能不再易受侵蚀。我们必须检查它们是否易受侵蚀。如果他们没有，但他们在名单上，那么我们必须把他们从名单上删除。

```c#
			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
				HexCell neighbor = cell.GetNeighbor(d);
				…
			}

			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
				HexCell neighbor = targetCell.GetNeighbor(d);
				if (
					neighbor && !IsErodible(neighbor) &&
					erodibleCells.Contains(neighbor)
				) {
					erodibleCells.Remove(neighbor);
				}
			}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-24/erosion/conserving-landmass.jpg)

*100%侵蚀，保护陆地。*

侵蚀现在将使地形更加平坦，降低一些地区，同时提高另一些地区。因此，陆地面积既可以增加，也可以减少。这可以在任何方向上将土地百分比调整几个百分点，但大的偏差很少见。因此，你施加的侵蚀越多，你对最终土地百分比的控制就越少。

### 3.6 侵蚀速度更快

虽然我们不需要太担心侵蚀算法的效率，但可以快速获得一些收益。首先，请注意，我们明确地检查我们侵蚀的单元格是否仍然可以被侵蚀。如果没有，我们会有效地将其从列表中删除。因此，在遍历目标单元格的邻居时，我们可以跳过检查此单元格。

```c#
			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
				HexCell neighbor = targetCell.GetNeighbor(d);
				if (
					neighbor && neighbor != cell && !IsErodible(neighbor) &&
					erodibleCells.Contains(neighbor)
				) {
					erodibleCells.Remove(neighbor);
				}
			}
```

其次，我们只需要费心检查目标单元格的邻居，如果它们之间曾经有悬崖，但现在没有了。只有当邻居现在比目标单元格高一步时，才会出现这种情况。如果是这样，那么邻居肯定在列表中，所以我们不必验证这一点，这意味着我们可以跳过不必要的搜索。

```c#
				HexCell neighbor = targetCell.GetNeighbor(d);
				if (
					neighbor && neighbor != cell &&
					neighbor.Elevation == targetCell.Elevation + 1 &&
					!IsErodible(neighbor)
//					&& erodibleCells.Contains(neighbor)
				) {
					erodibleCells.Remove(neighbor);
				}
```

第三，在检查易受侵蚀单元格的邻居时，我们可以使用类似的技巧。如果他们之间现在有悬崖，那么邻居就会被侵蚀。我们不需要调用 `IsErodible` 来发现这一点。

```c#
				HexCell neighbor = cell.GetNeighbor(d);
				if (
					neighbor && neighbor.Elevation == cell.Elevation + 2 &&
//					IsErodible(neighbor) &&
					!erodibleCells.Contains(neighbor)
				) {
					erodibleCells.Add(neighbor);
				}
```

然而，我们仍然需要检查目标单元格是否易受侵蚀，但上述循环现在不再处理这个问题。所以，对目标单元格明确地这样做。

```c#
			if (!IsErodible(cell)) {
				erodibleCells[index] = erodibleCells[erodibleCells.Count - 1];
				erodibleCells.RemoveAt(erodibleCells.Count - 1);
			}

			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
				…
			}

			if (IsErodible(targetCell) && !erodibleCells.Contains(targetCell)) {
				erodibleCells.Add(targetCell);
			}

			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
				…
			}
```

相对于最初形成的悬崖数量，我们现在可以相当快地应用侵蚀，达到我们想要的百分比。请注意，由于我们稍微更改了目标单元格添加到易受侵蚀列表的点，因此与优化之前相比，最终结果将有所变化。

![25](https://catlikecoding.com/unity/tutorials/hex-map/part-24/erosion/erosion-25.jpg) ![50](https://catlikecoding.com/unity/tutorials/hex-map/part-24/erosion/erosion-50.jpg) ![75](https://catlikecoding.com/unity/tutorials/hex-map/part-24/erosion/erosion-75.jpg) ![100](https://catlikecoding.com/unity/tutorials/hex-map/part-24/erosion/erosion-100.jpg)

*25%、50%、75% 和 100%侵蚀。*

还要注意的是，尽管海岸线会改变形状，但拓扑结构并没有从根本上改变。陆地往往保持连接或分离。只有小岛才能完全沉没。细节被平滑了，但整体形状保持不变。狭窄的连接可能会消失，但也可能会增长一点。狭窄的缝隙可能会填满，也可能会扩大一点。因此，侵蚀不会显著地将遥远的地区粘合在一起。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-24/erosion/four-regions-eroded.jpg)

*四个完全被侵蚀的地区，仍然分开。*

下一个教程是[水循环](https://catlikecoding.com/unity/tutorials/hex-map/part-25/)。

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-24/erosion/erosion.unitypackage)

[PDF](https://catlikecoding.com/unity/tutorials/hex-map/part-24/Hex-Map-24.pdf)

# Hex Map 25：水循环

发布于 2018-01-22

https://catlikecoding.com/unity/tutorials/hex-map/part-25/

*显示原始地图数据。*
*演化单元格气候。*
*创建部分水循环模拟。*

这是关于[六边形地图](https://catlikecoding.com/unity/tutorials/hex-map/)的系列教程的第 25 部分。[前一部分](https://catlikecoding.com/unity/tutorials/hex-map/part-24/)是关于地区和侵蚀的。这次我们要给土地增加水分。

本教程是用 Unity 2017.3.0 制作的。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-25/tutorial-image.jpg)

*利用水循环来确定生物群落。*

## 1 云

到目前为止，我们的地图生成算法只调整单元格的高度。单元格之间最大的区别在于它们是否被淹没。虽然我们还设置了不同的地形类型，但这只是高程的简单可视化。分配地形类型的更好方法是考虑当地气候。

地球的气候是一个非常复杂的系统。幸运的是，我们不必创建逼真的气候模拟。我们所需要的只是看起来足够自然的东西。气候最重要的方面是水循环，因为动植物需要液态水才能生存。温度也很重要，但这次我们将重点关注水，有效地保持全球温度恒定，同时改变湿度。

水循环描述了水如何在环境中流动。简单地说，水体蒸发，形成云层，云层产生雨水，雨水流回水体。这远不止于此，但模拟这些步骤可能已经足以在我们的地图上产生看似自然的水分布。

### 1.1 数据可视化

在我们进行实际模拟之前，如果我们能直接看到相关数据，那将是有用的。为此，我们将调整地形着色器。给它一个切换属性，这样我们就可以将其切换到数据可视化模式，显示原始地图数据，而不是通常的地形纹理。这是通过一个带有切换属性的浮点属性来实现的，该属性指定了一个关键字。这将使其在材质检查器中显示为复选框，该复选框控制是否设置了关键字。属性的实际名称并不重要，重要的是关键字，我们将使用 ***SHOW_MAP_DATA***。

```glsl
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Terrain Texture Array", 2DArray) = "white" {}
		_GridTex ("Grid Texture", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Specular ("Specular", Color) = (0.2, 0.2, 0.2)
		_BackgroundColor ("Background Color", Color) = (0,0,0)
		[Toggle(SHOW_MAP_DATA)] _ShowMapData ("Show Map Data", Float) = 0
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-25/clouds/show-map-data-toggle.png)

*切换以显示地图数据。*

添加着色器功能以启用对关键字的支持。

```glsl
		#pragma multi_compile _ GRID_ON
		#pragma multi_compile _ HEX_MAP_EDIT_MODE

		#pragma shader_feature SHOW_MAP_DATA
```

我们将使显示单个浮点值成为可能，就像其他地形数据一样。为了实现这一点，在定义关键字时，将 `mapData` 字段添加到 `Input` 结构中。

```glsl
		struct Input {
			float4 color : COLOR;
			float3 worldPos;
			float3 terrain;
			float4 visibility;

			#if defined(SHOW_MAP_DATA)
				float mapData;
			#endif
		};
```

在顶点程序中，我们将使用单元格数据的 Z 通道填充 `mapData`，像往常一样在单元格之间插值。

```glsl
		void vert (inout appdata_full v, out Input data) {
			…

			#if defined(SHOW_MAP_DATA)
				data.mapData = cell0.z * v.color.x + cell1.z * v.color.y +
					cell2.z * v.color.z;
			#endif
		}
```

当应该显示地图数据时，直接将其用作碎片的反照率，而不是正常颜色。将其与网格相乘，这样在可视化数据时网格仍然可以启用。

```glsl
		void surf (Input IN, inout SurfaceOutputStandardSpecular o) {
			…
			o.Albedo = c.rgb * grid * _Color * explored;
			#if defined(SHOW_MAP_DATA)
				o.Albedo = IN.mapData * grid;
			#endif
			…
		}
```

要实际将任何数据传输到着色器，我们必须向 `HexCellShaderData` 添加一个方法，将某些内容放入其纹理数据的蓝色通道中。数据是一个单一的浮点值，被限制在 0-1 的范围内。

```c#
	public void SetMapData (HexCell cell, float data) {
		cellTextureData[cell.Index].b =
			data < 0f ? (byte)0 : (data < 1f ? (byte)(data * 255f) : (byte)255);
		enabled = true;
	}
```

然而，这种方法干扰了我们的探索系统。蓝色数据分量的 255 值用于指示单元格的可见性正在转换。为了保持这一点，我们必须使用 byte 值 254 作为最大值。请注意，单位移动会清除地图数据，但这很好，因为我们只将其用于调试地图生成。

```c#
		cellTextureData[cell.Index].b =
			data < 0f ? (byte)0 : (data < 1f ? (byte)(data * 254f) : (byte)254);
```

同时向 `HexCell` 添加一个同名方法，该方法将请求传递给其着色器数据。

```c#
	public void SetMapData (float data) {
		ShaderData.SetMapData(this, data);
	}
```

要测试这是否有效，请调整 `HexMapGenerator.SetTerrainType`，因此它设置每个单元格的地图数据。让我们将标高可视化，从整数转换为 0-1 范围内的浮点数。这是通过从单元格的标高中减去标高最小值，然后除以标高最大值减去最小值来实现的。确保它是浮点数除法。

```c#
	void SetTerrainType () {
		for (int i = 0; i < cellCount; i++) {
			…
			cell.SetMapData(
				(cell.Elevation - elevationMinimum) /
				(float)(elevationMaximum - elevationMinimum)
			);
		}
	}
```

现在，通过切换***地形***材质资源的“***显示地图数据***”复选框，您应该能够在正常地形和数据可视化之间切换。

![normal](https://catlikecoding.com/unity/tutorials/hex-map/part-25/clouds/map-1208905299.jpg) ![map data](https://catlikecoding.com/unity/tutorials/hex-map/part-25/clouds/showing-elevation.jpg)

*地图 1208905299，正常地形和高程可视化。*

### 1.2 创造气候

为了模拟气候，我们必须跟踪气候数据。由于我们的地图由离散的单元格组成，每个单元格都有自己的局部气候。创建一个 `ClimateData` 结构体来包含所有相关数据。虽然我们可以将这些数据添加到单元格本身，但我们只会在生成地图时使用它。所以我们将把它分开存储。这意味着我们可以在 `HexMapGenerator` 中定义这个结构，就像 `MapRegion` 一样。我们将从只跟踪云开始，这可以用一个浮点数字段来完成。

```c#
	struct ClimateData {
		public float clouds;
	}
```

添加一个列表来跟踪所有单元格的气候数据。

```c#
	List<ClimateData> climate = new List<ClimateData>();
```

现在我们需要一种方法来创建地图的气候。它应该从清除气候列表开始，然后为每个单元格添加一个项目。初始气候数据只是零，我们通过 `ClimateData` 的默认构造函数获得。

```c#
	void CreateClimate () {
		climate.Clear();
		ClimateData initialData = new ClimateData();
		for (int i = 0; i < cellCount; i++) {
			climate.Add(initialData);
		}
	}
```

气候必须在土地被侵蚀后和地形类型确定之前创造。事实上，侵蚀主要是由空气和水的运动引起的，这是气候的一部分，但我们不会模拟这一点。

```c#
	public void GenerateMap (int x, int z) {
		…
		CreateRegions();
		CreateLand();
		ErodeLand();
		CreateClimate();
		SetTerrainType();
		…
	}
```

更改 `SetTerrainType`，这样我们就可以看到云数据而不是单元格高度。起初，这看起来像一张黑色的地图。

```c#
	void SetTerrainType () {
		for (int i = 0; i < cellCount; i++) {
			…
			cell.SetMapData(climate[i].clouds);
		}
	}
```

### 1.3 气候演变

我们气候模拟的第一步是蒸发。应该蒸发多少水？让我们用滑块来控制它。0 表示完全没有蒸发，1 表示最大蒸发。我们将使用 0.5 作为默认值。

```c#
	[Range(0f, 1f)]
	public float evaporation = 0.5f;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-25/clouds/evaporation-slider.png)

*蒸发滑块。*

让我们创建另一种专门用于进化单个单元格气候的方法。将单元格的索引作为参数，并使用它来检索相关单元格及其气候数据。如果单元格在水下，那么我们正在处理一个应该蒸发的水体。我们将立即将蒸汽转化为云——忽略露点和冷凝——因此直接将蒸发量添加到单元格的云值中。一旦我们完成，将气候数据复制回列表。

```c#
	void EvolveClimate (int cellIndex) {
		HexCell cell = grid.GetCell(cellIndex);
		ClimateData cellClimate = climate[cellIndex];
		
		if (cell.IsUnderwater) {
			cellClimate.clouds += evaporation;
		}

		climate[cellIndex] = cellClimate;
	}
```

在 `CreateClimate` 中为每个单元格调用此方法。

```c#
	void CreateClimate () {
		…

		for (int i = 0; i < cellCount; i++) {
			EvolveClimate(i);
		}
	}
```

只做一次是不够的。为了创建复杂的模拟，我们必须多次进化单元格气候。我们越经常这样做，结果就越精细。我们只需选择一个固定的量，让我们使用 40 个周期。

```c#
		for (int cycle = 0; cycle < 40; cycle++) {
			for (int i = 0; i < cellCount; i++) {
				EvolveClimate(i);
			}
		}
```

因为现在我们只会增加水下单元格上方的云层，我们最终会得到黑色的土地和白色的水体。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-25/clouds/evaporation.jpg)

*水上蒸发。*

### 1.4 云层消散

云不会永远停留在一个地方，尤其是当越来越多的水不断蒸发时。压力差导致空气移动，表现为风，这也使云层移动。

如果没有主导风向，平均而言，一个单元的云将向各个方向均匀分散，最终到达该单元的邻居。由于新的云将在下一个周期中生成，让我们将当前单元中的所有云分配给其邻居。因此，每个邻居都获得了该单元格云的六分之一，之后局部云降至零。

```c#
		if (cell.IsUnderwater) {
			cellClimate.clouds += evaporation;
		}

		float cloudDispersal = cellClimate.clouds * (1f / 6f);
		cellClimate.clouds = 0f;

		climate[cellIndex] = cellClimate;
```

要实际将云添加到邻居中，请遍历它们，检索它们的气候数据，增加它们的云值，并将其复制回列表。

```c#
		float cloudDispersal = cellClimate.clouds * (1f / 6f);
		for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
			HexCell neighbor = cell.GetNeighbor(d);
			if (!neighbor) {
				continue;
			}
			ClimateData neighborClimate = climate[neighbor.Index];
			neighborClimate.clouds += cloudDispersal;
			climate[neighbor.Index] = neighborClimate;
		}
		cellClimate.clouds = 0f;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-25/clouds/dispersed-clouds.jpg)

*驱散云层。*

这会产生一张几乎白色的地图。这是因为每个周期，所有水下单元格都会为全球气候增加更多的云。在第一个周期之后，水旁边的陆地单元格现在也有一些云需要消散。这个过程会一直持续到地图的大部分被云层覆盖。在默认设置的地图 1208905299 的情况下，只有东北大陆块的内部尚未完全覆盖。

请注意，我们的水体可以产生无限量的云。水位不是我们气候模拟的一部分。事实上，水体之所以持续存在，只是因为水以与蒸发速度大致相同的速度流回水体。所以我们只是在模拟部分水循环。这很好，但我们应该意识到，这意味着模拟运行的时间越长，气候中添加的水就越多。目前，唯一的失水发生在地图边缘，分散的云层被不存在的邻居淹没。

您可以在地图顶部看到水分流失，尤其是右上角的单元格。最后一个单元格根本没有云，因为它是最后一个演化的单元格。它还没有收到邻居的任何云。

> **难道不是所有的单元格气候都应该平行进化吗？**
>
> 是的，这将产生最一致的模拟。现在，由于单元顺序，云在一个周期内向整个地图的北部和东部分布，但向南和西部仅一步之遥。然而，这种不对称性在 40 个周期内得到了平滑。它只在地图的边缘非常明显。稍后我们将切换到并行进化。

### 1.5 降水量

水不可能永远存在。在某个时候，它会回落。这通常以雨的形式发生，但也可能是雪、冰雹或雨夹雪。一般来说，这被称为降水。云层消失的程度和发生的速度差异很大，但我们将简单地使用一个可配置的全球降水因子。值为0表示完全没有降水，而值为 1 表示所有云层立即消失。让我们使用 0.25 作为默认值。这意味着每个周期四分之一的云层都会被移除。

```c#
	[Range(0f, 1f)]
	public float precipitationFactor = 0.25f;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-25/clouds/precipidation-slider.png)

*降水系数滑块。*

我们将模拟蒸发后和云层消散前的降水。这意味着从水体中蒸发的一部分水会立即沉淀，因此分散的云层数量会减少。在陆地上，降水会导致云层消失。

```c#
		if (cell.IsUnderwater) {
			cellClimate.clouds += evaporation;
		}

		float precipitation = cellClimate.clouds * precipitationFactor;
		cellClimate.clouds -= precipitation;

		float cloudDispersal = cellClimate.clouds * (1f / 6f);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-25/clouds/disappearing-clouds.jpg)

*消失的云。*

现在，我们每个周期都要消除 25% 的云层，陆地再次大部分变黑。云层只向内陆移动了几步，就变得不明显了。

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-25/clouds/clouds.unitypackage)

## 2 潮湿度

虽然降水消除了云层，但它不应该从气候中去除水分。落在地上后，水仍然存在，只是处于另一种状态。它可以以多种形式存在，我们简单地将其抽象为潮湿度（moisture）。

### 2.1 追踪潮湿度

我们将通过跟踪云和潮湿这两种水分状态来增强我们的气候模型。为了支持这一点，请在 `ClimateData` 中添加一个 `moisture` 字段。

```c#
	struct ClimateData {
		public float clouds, moisture;
	}
```

在最一般的形式中，蒸发是将潮湿度转化为云的过程，至少在我们的简单气候模型中是这样。这意味着蒸发量不应该是一个恒定值，而是另一个因子。因此，重构将 `evaporation` 重命名为 `evaporationFactor`。

```c#
	[Range(0f, 1f)]
	public float evaporationFactor = 0.5f;
```

当一个单元格在水下时，我们简单地宣布它的含水量（moisture）为 1。这意味着蒸发量等于蒸发系数。但我们现在也可以从陆地单元格中蒸发。在这种情况下，我们必须计算蒸发量，将其从潮湿度中减去，再将其加到云中。之后，降水被添加到潮湿度中。

```c#
		if (cell.IsUnderwater) {
			cellClimate.moisture = 1f;
			cellClimate.clouds += evaporationFactor;
		}
		else {
			float evaporation = cellClimate.moisture * evaporationFactor;
			cellClimate.moisture -= evaporation;
			cellClimate.clouds += evaporation;
		}
		
		float precipitation = cellClimate.clouds * precipitationFactor;
		cellClimate.clouds -= precipitation;
		cellClimate.moisture += precipitation;
```

因为云层现在由陆地上的蒸发维持，所以它们能够进一步向内陆移动。现在大部分土地都是灰色的。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-25/moisture/clouds.jpg)

*水汽蒸发的云。*

让我们调整 `SetTerrainType`，使其显示潮湿度而不是云，因为这是我们稍后将用来确定地形类型的。

```c#
			cell.SetMapData(climate[i].moisture);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-25/moisture/moisture.jpg)

*显示潮湿度。*

此时，潮湿度看起来与云非常相似——除了所有水下单元格都是白色的——但这种情况很快就会改变。

### 2.2 径流

蒸发并不是潮湿离开单元格的唯一方式。水循环表明，添加到土地上的大部分水分（moisture）最终会再次进入水体。最明显的方式是，水在重力的作用下流过陆地。我们在模拟中不考虑实际的河流，而是使用可配置的径流因子（runoff factor）。这代表了流向较低地区的排水部分。让我们默认抽掉 25%。

```c#
	[Range(0f, 1f)]
	public float runoffFactor = 0.25f;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-25/moisture/runoff-slider.png)

*滑行滑块。*

> **我们不会创造河流吗？**
>
> 我们将在未来的教程中根据我们产生的气候添加它们。

径流就像云层消散一样，有三个不同之处。首先，并非所有单元格的潮湿度都被去除。其次，它输送的是水分（moisture），而不是云。第三，它只向下，所以只对海拔较低的邻居开放。径流系数描述了如果所有邻居都较低，水分会流失多少，但通常会更少。这意味着只有当我们找到一个较低的邻居时，我们才能降低单元格的湿度。

```c#
		float cloudDispersal = cellClimate.clouds * (1f / 6f);
		float runoff = cellClimate.moisture * runoffFactor * (1f / 6f);
		for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
			HexCell neighbor = cell.GetNeighbor(d);
			if (!neighbor) {
				continue;
			}
			ClimateData neighborClimate = climate[neighbor.Index];
			neighborClimate.clouds += cloudDispersal;

			int elevationDelta = neighbor.Elevation - cell.Elevation;
			if (elevationDelta < 0) {
				cellClimate.moisture -= runoff;
				neighborClimate.moisture += runoff;
			}
			
			climate[neighbor.Index] = neighborClimate;
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-25/moisture/runoff.jpg)

*水流向较低的地面。*

我们最终得到的潮湿度分布更加多样化，因为较高的单元格会向较低的单元格流失水分（moisture）。我们还看到沿海单元格中的水分要少得多，因为它们会排入水下单元格。为了减轻这种影响，我们还应该使用水位来确定单元格是否较低，而不是使用视图标高。

```c#
			int elevationDelta = neighbor.ViewElevation - cell.ViewElevation;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-25/moisture/runoff-view-elevation.jpg)

*使用视图高度。*

### 2.3 渗漏

水不仅向下流动。它还会扩散，渗透到平坦的地形上，并被水体附近的土地吸收。这可能是一种微妙的效果，但有助于平滑水分的分布，所以让我们也将其添加到我们的模拟中。使用 0.125 作为默认值，给它自己的可配置因子。

```c#
	[Range(0f, 1f)]
	public float seepageFactor = 0.125f;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-25/moisture/seepage-slider.png)

*渗流滑块。*

渗流与径流相同，除了当邻居的视图高程与单元格本身相同时适用。

```c#
		float runoff = cellClimate.moisture * runoffFactor * (1f / 6f);
		float seepage = cellClimate.moisture * seepageFactor * (1f / 6f);
		for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
			…

			int elevationDelta = neighbor.ViewElevation - cell.ViewElevation;
			if (elevationDelta < 0) {
				cellClimate.moisture -= runoff;
				neighborClimate.moisture += runoff;
			}
			else if (elevationDelta == 0) {
				cellClimate.moisture -= seepage;
				neighborClimate.moisture += seepage;
			}

			climate[neighbor.Index] = neighborClimate;
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-25/moisture/seepage.jpg)

*有点渗水。*

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-25/moisture/moisture.unitypackage)

## 3 雨影

虽然我们目前对水循环有一个不错的模拟，但看起来并不太有趣。这是因为它不包含雨影，雨影是气候差异最显著的表现之一。雨影描述了与附近地区相比，降雨量严重不足的地区。这些地区的存在是因为山脉阻止了云层到达。这需要高山和盛行风向。

### 3.1 风

让我们首先在模拟中添加一个主导风向。虽然地球上的主导风向变化很大，但我们将使用可配置的全球风向。让我们使用西北作为默认值。除此之外，还可以配置风的强度，从 1 到 10，默认值为 4。

```c#
	public HexDirection windDirection = HexDirection.NW;
	
	[Range(1f, 10f)]
	public float windStrength = 4f;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-25/rain-shadows/wind-settings.png)

*风向和风力。*

主导风的强度相对于均匀的云层扩散表示。当风力为 1 时，分散在所有方向上都是相等的。当它为 2 时，风向的扩散是其他方向的两倍，以此类推。我们可以通过改变云扩散计算的除数来实现这一点。它应该是五加上风力，而不是六。

```c#
		float cloudDispersal = cellClimate.clouds * (1f / (5f + windStrength));
```

此外，风向决定了风吹的方向。因此，我们需要使用与之相反的方向作为主要的扩散方向。

```c#
		HexDirection mainDispersalDirection = windDirection.Opposite();
		float cloudDispersal = cellClimate.clouds * (1f / (5f + windStrength));
```

现在我们可以检查邻居是否位于主扩散方向。如果是这样，我们应该把云的消散乘以风力。

```c#
			ClimateData neighborClimate = climate[neighbor.Index];
			if (d == mainDispersalDirection) {
				neighborClimate.clouds += cloudDispersal * windStrength;
			}
			else {
				neighborClimate.clouds += cloudDispersal;
			}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-25/rain-shadows/northwest-wind.jpg)

*西北风，4 级。*

主导风增加了水分在陆地上分布的方向性。风越大，这种影响就越极端。

### 3.2 海拔高度

雨影的第二个要素是山脉。我们对山没有严格的分类，大自然也没有。重要的是海拔高度。从本质上讲，当空气流过一座山时，它会被向上推动，冷却，可以容纳更少的水，这迫使空气在通过这座山之前降水。结果是另一侧的空气干燥，因此形成了雨影。

关键在于，空气越高，它能容纳的水就越少。我们可以在模拟中通过强制每个单元格的最大云值来表示这一点。单元格的视图标高越高，此最大值应越低。最直接的方法是将最大值设置为 1 减去视图标高除以标高最大值。实际上，让我们除以最大值加 1。这使得即使在最高的单元格上，也有一点云仍然会流动。我们将在降水后、消散（dispersal）前执行这一最大值。

```c#
		float precipitation = cellClimate.clouds * precipitationFactor;
		cellClimate.clouds -= precipitation;
		cellClimate.moisture += precipitation;

		float cloudMaximum = 1f - cell.ViewElevation / (elevationMaximum + 1f);
		
		HexDirection mainDispersalDirection = windDirection.Opposite();
```

如果我们最终得到的云比允许的多，只需将多余的云转化为水分。这有效地迫使额外的降水，就像真正的山脉一样。

```c#
		float cloudMaximum = 1f - cell.ViewElevation / (elevationMaximum + 1f);
		if (cellClimate.clouds > cloudMaximum) {
			cellClimate.moisture += cellClimate.clouds - cloudMaximum;
			cellClimate.clouds = cloudMaximum;
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-25/rain-shadows/rain-shadows.jpg)

*高海拔造成的雨影。*

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-25/rain-shadows/rain-shadows.unitypackage)

## 4 完成模拟

在这一点上，我们有一个不错的部分水循环模拟。让我们稍微整理一下，然后用它来确定单元格的地形类型。

### 4.1 平行评估

正如前面提到的，我们进化单元格的顺序会影响模拟的结果。理想情况下，情况并非如此，我们有效地并行演化了所有单元格。我们可以通过将当前演化步骤的所有变化应用于第二个气候列表 `nextClimate` 来实现这一点。

```c#
	List<ClimateData> climate = new List<ClimateData>();
	List<ClimateData> nextClimate = new List<ClimateData>();
```

清除并初始化此列表，就像另一个列表一样。然后在每个循环后交换列表。这使得模拟在使用哪个列表以及当前和下一个气候数据之间交替进行。

```c#
	void CreateClimate () {
		climate.Clear();
		nextClimate.Clear();
		ClimateData initialData = new ClimateData();
		for (int i = 0; i < cellCount; i++) {
			climate.Add(initialData);
			nextClimate.Add(initialData);
		}

		for (int cycle = 0; cycle < 40; cycle++) {
			for (int i = 0; i < cellCount; i++) {
				EvolveClimate(i);
			}
			List<ClimateData> swap = climate;
			climate = nextClimate;
			nextClimate = swap;
		}
	}
```

当一个单元格影响其邻居的气候时，它应该调整邻居的下一个气候数据，而不是当前的数据。

```c#
		for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
			HexCell neighbor = cell.GetNeighbor(d);
			if (!neighbor) {
				continue;
			}
			ClimateData neighborClimate = nextClimate[neighbor.Index];
			…

			nextClimate[neighbor.Index] = neighborClimate;
		}
```

与其将单元格的气候数据复制回当前的气候列表，不如检索其下一个气候数据，将当前的湿度添加到其中，并将其复制到下一个列表。之后，重置当前列表的数据，使其在下一个周期中保持新鲜。

```c#
//		cellClimate.clouds = 0f;

		ClimateData nextCellClimate = nextClimate[cellIndex];
		nextCellClimate.moisture += cellClimate.moisture;
		nextClimate[cellIndex] = nextCellClimate;
		climate[cellIndex] = new ClimateData();
```

当我们这样做的时候，让我们也强制潮湿度级别最大为 1，这样陆地单元格就不会比水下单元格更湿润。

```c#
		nextCellClimate.moisture += cellClimate.moisture;
		if (nextCellClimate.moisture > 1f) {
			nextCellClimate.moisture = 1f;
		}
		nextClimate[cellIndex] = nextCellClimate;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-25/finishing-the-simulation/parallel-evaluation.jpg)

*平行评估。*

### 4.2 初始湿度

我们的模拟结果可能会有太多的旱地，尤其是在陆地比例很高时。为了改善这一点，我们可以添加一个可配置的起始湿度水平，默认值为 0.1。

```c#
	[Range(0f, 1f)]
	public float startingMoisture = 0.1f;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-25/finishing-the-simulation/starting-moisture-slider.png)

*启动湿度滑块，位于顶部。*

将此值用于初始气候列表的湿度，但不用于下一个列表。

```c#
		ClimateData initialData = new ClimateData();
		initialData.moisture = startingMoisture;
		ClimateData clearData = new ClimateData();
		for (int i = 0; i < cellCount; i++) {
			climate.Add(initialData);
			nextClimate.Add(clearData);
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-25/finishing-the-simulation/starting-moisture.jpg)

*含起始水分。*

### 4.3 设置生物特征

最后，我们使用湿度而不是海拔来设置单元地形类型。让我们用雪来代表极度干燥的土地，用沙子代表干旱地区，用石头代表之后，用草代表相当湿润，用泥代表浸泡和水下的单元格。最简单的方法是只使用五个 0.2 波段。

```c#
	void SetTerrainType () {
		for (int i = 0; i < cellCount; i++) {
			HexCell cell = grid.GetCell(i);
			float moisture = climate[i].moisture;
			if (!cell.IsUnderwater) {
				if (moisture < 0.2f) {
					cell.TerrainTypeIndex = 4;
				}
				else if (moisture < 0.4f) {
					cell.TerrainTypeIndex = 0;
				}
				else if (moisture < 0.6f) {
					cell.TerrainTypeIndex = 3;
				}
				else if (moisture < 0.8f) {
					cell.TerrainTypeIndex = 1;
				}
				else {
					cell.TerrainTypeIndex = 2;
				}
			}
			else {
				cell.TerrainTypeIndex = 2;
			}
			cell.SetMapData(moisture);
		}
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-25/finishing-the-simulation/biomes.jpg)

*生物特征。*

使用甚至分布（distribution）都不会产生好的结果，也不符合自然。使用0.05、0.12、0.28和0.85等阈值可以获得更好的结果。

```c#
				if (moisture < 0.05f) {
					cell.TerrainTypeIndex = 4;
				}
				else if (moisture < 0.12f) {
					cell.TerrainTypeIndex = 0;
				}
				else if (moisture < 0.28f) {
					cell.TerrainTypeIndex = 3;
				}
				else if (moisture < 0.85f) {
					cell.TerrainTypeIndex = 1;
				}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-25/finishing-the-simulation/tweaked-biomes.jpg)

*调整了生物群落。*

下一个教程是[生物群落与河流](https://catlikecoding.com/unity/tutorials/hex-map/part-26/)。

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-25/finishing-the-simulation/finishing-the-simulation.unitypackage)

[PDF](https://catlikecoding.com/unity/tutorials/hex-map/part-25/Hex-Map-25.pdf)

# Hex Map 26：生物群系与河流

发布于 2018-02-20

https://catlikecoding.com/unity/tutorials/hex-map/part-26/

*河流起源于高湿的单元格。*
*创建一个简单的温度模型。*
*对单元格使用生物群落矩阵，然后对其进行调整。*

这是关于[六边形地图](https://catlikecoding.com/unity/tutorials/hex-map/)的系列教程的第 26 部分。[上一部分](https://catlikecoding.com/unity/tutorials/hex-map/part-25/)在我们的地图生成算法中添加了部分水循环。这一次，我们将用河流和温度来补充它，并为单元格分配更多有趣的生物群落。

本教程是用 Unity 2017.3.0p3 编写的。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-26/tutorial-image.jpg)

*热和水给地图带来了生机。*

## 1 生成河流

河流是水循环的结果。基本上，它们是由径流通过侵蚀形成的。这表明我们可以根据单元格的径流来添加河流。然而，这并不能保证我们得到任何看起来像实际河流的东西。一旦我们开始一条河流，它应该尽可能地继续流动，可能会穿过许多单元格。这不适合我们的水循环模拟，它对单元格并行运行。此外，你通常希望控制地图上有多少条河流。

因为河流如此不同，我们将分别生成它们。我们将使用水循环模拟的结果来确定河流的位置，但我们不会让河流反过来影响模拟。

> **有时河流的流向是错误的？**
>
> 我们的 `TriangulateWaterShore` 方法中有一个错误，很少出现。这发生在河流的终点，在改变流向后。问题是，我们只检查了当前方向是否与来水方向匹配。当我们处理河流的起点时，这会出错。解决方案是还要检查该单元是否真的有河流流入。我也把这个修复程序放在了 [Rivers](https://catlikecoding.com/unity/tutorials/hex-map/part-6/) 教程中。
>
> ```c#
> 	void TriangulateWaterShore (
> 		HexDirection direction, HexCell cell, HexCell neighbor, Vector3 center
> 	) {
> 		…
> 
> 		if (cell.HasRiverThroughEdge(direction)) {
> 			TriangulateEstuary(
> 				e1, e2,
> 				cell.HasIncomingRiver && cell.IncomingRiver == direction, indices
> 			);
> 		}
> 		…
> 	}
> ```

### 1.1 高和湿

在我们的地图上，一个单元格要么有河流，要么没有。它们也不能分支或合并。事实上，河流比这灵活得多，但我们只能用这种近似值来表示较大的河流。我们必须确定的最重要的事实是，一条大河从哪里开始，我们必须随机选择。

因为河流需要水，所以河流的起源必须在一个有大量水分的单元格中。但这还不够。河流顺流而下，所以理想情况下，发源地也有很高的海拔。一个单元格高于水位越高，它就越适合作为河流起源的候选者。通过将单元格的标高除以最大标高，我们可以将其可视化为地图数据。要使其相对于水位，请在除法前将其从两个高程中减去。

```c#
	void SetTerrainType () {
		for (int i = 0; i < cellCount; i++) {
			…
			float data =
				(float)(cell.Elevation - waterLevel) /
				(elevationMaximum - waterLevel);
			cell.SetMapData(data);
		}
	}
```

![moisture](https://catlikecoding.com/unity/tutorials/hex-map/part-26/generating-rivers/moisture.jpg) ![elevation](https://catlikecoding.com/unity/tutorials/hex-map/part-26/generating-rivers/elevation.jpg)

*水分和海拔高度。默认大地图 1208905299。*

最好的候选者是那些同时具有高湿度和高海拔的单元格。我们可以通过将这些标准相乘来组合它们。结果是河流起源的适合度或重量。

```c#
			float data =
				moisture * (cell.Elevation - waterLevel) /
				(elevationMaximum - waterLevel);
			cell.SetMapData(data);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-26/generating-rivers/river-origin-weights.jpg)

*河流起源的权重。*

理想情况下，我们会使用这些权重来偏置原始单元格的随机选择。虽然我们可以构建一个适当加权的列表并从中进行选择，但这并不简单，而且会减缓生成过程。我们可以做一个更简单的重要性分类，区分四个级别。主要候选人的权重超过 0.75。优秀的候选人的权重在 0.5 以上。仍然可以接受的候选人的权重高于 0.25。所有其他单元格都被取消资格。让我们可视化一下那是什么样子。

```c#
			float data =
				moisture * (cell.Elevation - waterLevel) /
				(elevationMaximum - waterLevel);
			if (data > 0.75f) {
				cell.SetMapData(1f);
			}
			else if (data > 0.5f) {
				cell.SetMapData(0.5f);
			}
			else if (data > 0.25f) {
				cell.SetMapData(0.25f);
			}
//			cell.SetMapData(data);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-26/generating-rivers/river-origin-categories.jpg)

*河流起源权重类别。*

通过这种分类方案，我们最终可能会得到来自地图上较高和较湿润地区的河流。但河流仍有可能在地势较低或较干燥的地区形成，从而提供多样性。

添加一个 `CreateRivers` 方法，该方法使用这些条件填充单元格列表。可接受的单元格将添加到此列表中一次，良好单元格将添加两次，素数候选单元格将添加四次。水下的单元格总是不合格的，所以我们可以跳过检查。

```c#
	void CreateRivers () {
		List<HexCell> riverOrigins = ListPool<HexCell>.Get();
		for (int i = 0; i < cellCount; i++) {
			HexCell cell = grid.GetCell(i);
			if (cell.IsUnderwater) {
				continue;
			}
			ClimateData data = climate[i];
			float weight =
				data.moisture * (cell.Elevation - waterLevel) /
				(elevationMaximum - waterLevel);
			if (weight > 0.75f) {
				riverOrigins.Add(cell);
				riverOrigins.Add(cell);
			}
			if (weight > 0.5f) {
				riverOrigins.Add(cell);
			}
			if (weight > 0.25f) {
				riverOrigins.Add(cell);
			}
		}

		ListPool<HexCell>.Add(riverOrigins);
	}
```

此方法必须在 `CreateClimate` 之后调用，因此我们有可用的湿度数据。

```c#
	public void GenerateMap (int x, int z) {
		…
		CreateRegions();
		CreateLand();
		ErodeLand();
		CreateClimate();
		CreateRivers();
		SetTerrainType();
		…
	}
```

分类完成后，我们可以摆脱它的地图数据可视化。

```c#
	void SetTerrainType () {
		for (int i = 0; i < cellCount; i++) {
			…
//			float data =
//				moisture * (cell.Elevation - waterLevel) /
//				(elevationMaximum - waterLevel);
//			if (data > 0.6f) {
//				cell.SetMapData(1f);
//			}
//			else if (data > 0.4f) {
//				cell.SetMapData(0.5f);
//			}
//			else if (data > 0.2f) {
//				cell.SetMapData(0.25f);
//			}
		}
	}
```

### 1.2 河流预算

有多少条河流是理想的？这应该是可配置的。由于河流的长度各不相同，因此通过河流预算来控制这一点是最有意义的，该预算规定了一条河流应该包含多少土地单元。让我们用百分比表示，最大值为20%，默认值为10%。与土地百分比一样，这是一个目标金额，而不是保证。我们最终可能会发现候选人太少，或者河流太短，无法覆盖所需的土地面积。这就是为什么最大百分比不应该太高。

```c#
	[Range(0, 20)]
	public int riverPercentage = 10;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-26/generating-rivers/river-percentage.png)

*河流百分比滑块。*

为了能够确定以单元格数量表示的河流预算，我们必须记住 `CreateLand` 中生成了多少土地单元格。

```c#
	int cellCount, landCells;
	…
	
	void CreateLand () {
		int landBudget = Mathf.RoundToInt(cellCount * landPercentage * 0.01f);
		landCells = landBudget;
		for (int guard = 0; guard < 10000; guard++) {
			…
		}
		if (landBudget > 0) {
			Debug.LogWarning("Failed to use up " + landBudget + " land budget.");
			landCells -= landBudget;
		}
	}
```

在 `CreateRivers` 中，现在可以像在 `CreateLand` 中一样计算河流预算。

```c#
	void CreateRivers () {
		List<HexCell> riverOrigins = ListPool<HexCell>.Get();
		for (int i = 0; i < cellCount; i++) {
			…
		}

		int riverBudget = Mathf.RoundToInt(landCells * riverPercentage * 0.01f);
		
		ListPool<HexCell>.Add(riverOrigins);
	}
```

接下来，只要我们还有预算和来源，就继续从来源列表中挑选和删除随机单元格。如果我们没有用完预算，也要记录一个警告。

```c#
		int riverBudget = Mathf.RoundToInt(landCells * riverPercentage * 0.01f);
		while (riverBudget > 0 && riverOrigins.Count > 0) {
			int index = Random.Range(0, riverOrigins.Count);
			int lastIndex = riverOrigins.Count - 1;
			HexCell origin = riverOrigins[index];
			riverOrigins[index] = riverOrigins[lastIndex];
			riverOrigins.RemoveAt(lastIndex);
		}
		
		if (riverBudget > 0) {
			Debug.LogWarning("Failed to use up river budget.");
		}
```

除此之外，还添加了一种实际创建河流的方法。它需要原始单元格作为参数，并在完成后返回河流的长度。从只返回长度为零的方法存根开始。

```c#
	int CreateRiver (HexCell origin) {
		int length = 0;
		return length;
	}
```

在我们刚刚添加到 `CreateRivers` 的循环结束时调用此方法，使用它来减少剩余预算。确保我们只在所选单元格还没有流过新河的情况下创建新河。

```c#
		while (riverBudget > 0 && riverOrigins.Count > 0) {
			…

			if (!origin.HasRiver) {
				riverBudget -= CreateRiver(origin);
			}
		}
```

### 1.3 流动的河流

让河流流向大海或其他水体似乎很简单。当我们从它的起源开始时，我们立即从长度 1 开始。之后，选择一个随机邻居，流入其中，并增加长度。继续这样做，直到我们最终进入水下单元格。

```c#
	int CreateRiver (HexCell origin) {
		int length = 1;
		HexCell cell = origin;
		while (!cell.IsUnderwater) {
			HexDirection direction = (HexDirection)Random.Range(0, 6);
			cell.SetOutgoingRiver(direction);
			length += 1;
			cell = cell.GetNeighbor(direction);
		}
		return length;
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-26/generating-rivers/haphazard-rivers.jpg)

*Haphazard 河。*

这种天真的方法的结果是随意放置的河流碎片，主要是因为我们最终取代了以前产生的河流。它甚至可能导致错误，因为我们甚至不检查邻居是否真的存在。我们必须遍历所有方向，并验证我们在那里有邻居。如果是这样，将此方向添加到潜在流向列表中，但前提是该邻居还没有河流流过。然后从该列表中随机选择一个方向。

```c#
	List<HexDirection> flowDirections = new List<HexDirection>();
	
	…
	
	int CreateRiver (HexCell origin) {
		int length = 1;
		HexCell cell = origin;
		while (!cell.IsUnderwater) {
			flowDirections.Clear();
			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
				HexCell neighbor = cell.GetNeighbor(d);
				if (!neighbor || neighbor.HasRiver) {
					continue;
				}
				flowDirections.Add(d);
			}

			HexDirection direction =
//				(HexDirection)Random.Range(0, 6);
				flowDirections[Random.Range(0, flowDirections.Count)];
			cell.SetOutgoingRiver(direction);
			length += 1;
			cell = cell.GetNeighbor(direction);
		}
		return length;
	}
```

通过这种新方法，我们最终可能会得到零个可用的流向。当这种情况发生时，河流无法继续流动，我们不得不中止。如果此时长度等于 1，则意味着我们无法流出原始单元格，因此根本不可能有河流。在这种情况下，河流的长度为零。

```c#
			flowDirections.Clear();
			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
				…
			}

			if (flowDirections.Count == 0) {
				return length > 1 ? length : 0;
			}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-26/generating-rivers/preserved-rivers.jpg)

*保存完好的河流。*

### 1.4 流下山

我们现在正在保护已经形成的河流，但最终仍会留下孤立的河流碎片。这是因为到目前为止，我们忽略了海拔。每次我们让河流流向更高的海拔，`HexCell.SetOutgoingRiver` 正确地中止了尝试，这导致了我们河流的不连续性。因此，我们还必须跳过会导致河流向上流动的方向。

```c#
				if (!neighbor || neighbor.HasRiver) {
					continue;
				}

				int delta = neighbor.Elevation - cell.Elevation;
				if (delta > 0) {
					continue;
				}
				
				flowDirections.Add(d);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-26/generating-rivers/downhill-rivers.jpg)

*河流顺流而下。*

这消除了许多河流碎片，尽管我们仍然得到了一些。从这一点来看，去除最难看的河流是一个精致的问题。首先，河流更喜欢尽快下坡。不能保证他们走尽可能短的路线，但很可能。为了模拟这一点，在下坡方向上，在列表中额外增加三次。

```c#
				if (delta > 0) {
					continue;
				}

				if (delta < 0) {
					flowDirections.Add(d);
					flowDirections.Add(d);
					flowDirections.Add(d);
				}
				flowDirections.Add(d);
```

### 1.5 避免急转弯

除了喜欢下坡，流水也有动力。河流更有可能笔直前进或缓慢弯曲，而不是突然急转弯。我们可以通过追踪河流的最后流向来引入这种偏见。如果潜在的流向与此方向没有太大偏差，请再次将其添加到列表中。这在起源处不是问题，所以在这种情况下只需再次添加即可。

```c#
	int CreateRiver (HexCell origin) {
		int length = 1;
		HexCell cell = origin;
		HexDirection direction = HexDirection.NE;
		while (!cell.IsUnderwater) {
			flowDirections.Clear();
			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
				…

				if (delta < 0) {
					flowDirections.Add(d);
					flowDirections.Add(d);
					flowDirections.Add(d);
				}
				if (
					length == 1 ||
					(d != direction.Next2() && d != direction.Previous2())
				) {
					flowDirections.Add(d);
				}
				flowDirections.Add(d);
			}

			if (flowDirections.Count == 0) {
				return length > 1 ? length : 0;
			}

//			HexDirection direction =
			direction = flowDirections[Random.Range(0, flowDirections.Count)];
			cell.SetOutgoingRiver(direction);
			length += 1;
			cell = cell.GetNeighbor(direction);
		}
		return length;
	}
```

这使得曲折的河流不太可能出现，因为它们很难看。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-26/generating-rivers/fewer-sharp-turns.jpg)

*急转弯更少。*

### 1.6 合并河流

有时，一条河流最终会在之前创建的河流的源头附近流动。除非那条河发源于更高的海拔，否则我们可以决定让新河流入旧河。结果是一条较长的河流，而不是附近的两条较短的河流。

要做到这一点，只有当邻居有流入的河流，或者是当前河流的源头时，才跳过邻居。然后，在我们确定这不是一个上坡方向后，检查是否有一条流出的河流。如果是这样，我们发现了一条古老的河流起源。因为这种情况相当罕见，所以不要费心检查其他潜在的邻居来源，并立即合并河流。

```c#
				HexCell neighbor = cell.GetNeighbor(d);
//				if (!neighbor || neighbor.HasRiver) {
//					continue;
//				}
				if (!neighbor || neighbor == origin || neighbor.HasIncomingRiver) {
					continue;
				}

				int delta = neighbor.Elevation - cell.Elevation;
				if (delta > 0) {
					continue;
				}

				if (neighbor.HasOutgoingRiver) {
					cell.SetOutgoingRiver(d);
					return length;
				}
```

![disconnected](https://catlikecoding.com/unity/tutorials/hex-map/part-26/generating-rivers/disconnected.jpg) ![connected](https://catlikecoding.com/unity/tutorials/hex-map/part-26/generating-rivers/connected.jpg)

*合并前后的河流。*

### 1.7 保持距离

因为高质量的起源候选者往往聚集在一起，我们最终会得到河流群。此外，我们最终可能会发现河流发源于水体附近，从而形成单步河流。我们可以通过取消与河流或水体相邻的来源的资格来分散来源。通过在 `CreateRivers` 中循环所选原点的邻居来完成此操作。如果我们发现一个有问题的邻居，则来源无效，我们应该跳过它。

```c#
		while (riverBudget > 0 && riverOrigins.Count > 0) {
			int index = Random.Range(0, riverOrigins.Count);
			int lastIndex = riverOrigins.Count - 1;
			HexCell origin = riverOrigins[index];
			riverOrigins[index] = riverOrigins[lastIndex];
			riverOrigins.RemoveAt(lastIndex);

			if (!origin.HasRiver) {
				bool isValidOrigin = true;
				for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
					HexCell neighbor = origin.GetNeighbor(d);
					if (neighbor && (neighbor.HasRiver || neighbor.IsUnderwater)) {
						isValidOrigin = false;
						break;
					}
				}
				if (isValidOrigin) {
					riverBudget -= CreateRiver(origin);
				}
			}
```

虽然河流最终仍可能并排流动，但它们现在往往覆盖更大的面积。

![clustered](https://catlikecoding.com/unity/tutorials/hex-map/part-26/generating-rivers/clustered.jpg) ![spread out](https://catlikecoding.com/unity/tutorials/hex-map/part-26/generating-rivers/spread-out.jpg)

*没有和保持距离。*

### 1.8 以湖结束

并非所有的河流都能汇入水体。有些人被困在山谷中或被其他河流堵塞。这不是一个大问题，因为有许多真正的河流似乎也消失了。例如，这可能是因为它们在地下流动，因为它们扩散到沼泽地区，或者因为它们干涸。我们的河流无法想象这一点，所以它们只是结束了。

话虽如此，我们应该尽量减少此类事件的发生。虽然我们不能合并河流或让它们向上流动，但我们可以让它们在湖泊中结束，这更常见，看起来更好。为此，`CreateRiver` 必须在单元格卡住时提高单元格的水位。这是否可能取决于该单元格邻居的最小海拔高度。因此，在调查邻居时要跟踪这一点，这需要一点代码重组。

```c#
		while (!cell.IsUnderwater) {
			int minNeighborElevation = int.MaxValue;
			flowDirections.Clear();
			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
				HexCell neighbor = cell.GetNeighbor(d);
//				if (!neighbor || neighbor == origin || neighbor.HasIncomingRiver) {
//					continue;
//				}
				if (!neighbor) {
					continue;
				}

				if (neighbor.Elevation < minNeighborElevation) {
					minNeighborElevation = neighbor.Elevation;
				}

				if (neighbor == origin || neighbor.HasIncomingRiver) {
					continue;
				}

				int delta = neighbor.Elevation - cell.Elevation;
				if (delta > 0) {
					continue;
				}

				…
			}

			…
		}
```

如果我们被困住了，首先检查我们是否还在原点。如果是这样，只需中止河流。否则，检查所有邻居是否至少与当前单元格一样高。如果是这样的话，那么我们可以把水位提高到这个水平。这将形成一个单一单元格湖，除非单元格的高度处于同一水平。如果是这样，只需将标高设置为水位以下。

![without](https://catlikecoding.com/unity/tutorials/hex-map/part-26/generating-rivers/without-lake.jpg) ![with](https://catlikecoding.com/unity/tutorials/hex-map/part-26/generating-rivers/with-lake.jpg)

*河流的尽头有湖也有湖。在这种情况下，河流百分比为 20。*

请注意，我们现在可以使用水位以上的水下单元来生成地图，表示海平面以上的湖泊。

### 1.9 额外湖泊

我们还可以在没有被困住的时候建造湖泊。这将导致河流流入一个湖泊的出口。当没有卡住时，可以通过将水位升高到单元格的当前高度，然后降低单元格高度来创建湖泊。这仅在最小邻居标高至少等于当前单元标高时有效。在进入下一个单元格之前，在河流循环的尽头这样做。

```c#
		while (!cell.IsUnderwater) {
			…
			
			if (minNeighborElevation >= cell.Elevation) {
				cell.WaterLevel = cell.Elevation;
				cell.Elevation -= 1;
			}
			
			cell = cell.GetNeighbor(direction);
		}
```

![without](https://catlikecoding.com/unity/tutorials/hex-map/part-26/generating-rivers/without-extra-lakes.jpg) ![with](https://catlikecoding.com/unity/tutorials/hex-map/part-26/generating-rivers/with-extra-lakes.jpg)

*没有和有额外的湖泊。*

虽然一些湖泊很漂亮，但如果不加以限制，这种方法可能会产生太多的湖泊。因此，让我们为额外的湖泊添加一个可配置的概率，默认值为 0.25。

```c#
	[Range(0f, 1f)]
	public float extraLakeProbability = 0.25f;
```

如果可能的话，这控制了产生额外湖泊的概率。

```c#
			if (
				minNeighborElevation >= cell.Elevation &&
				Random.value < extraLakeProbability
			) {
				cell.WaterLevel = cell.Elevation;
				cell.Elevation -= 1;
			}
```

![inspector](https://catlikecoding.com/unity/tutorials/hex-map/part-26/generating-rivers/extra-lake-probability.png)
![scene](https://catlikecoding.com/unity/tutorials/hex-map/part-26/generating-rivers/some-extra-lakes.jpg)

*一些额外的湖泊。*

> **创建大于一个单元格的湖泊怎么样？**
>
> 你可以通过让它们在水下的单元格附近形成更大的湖泊，只要它们有正确的水位。然而，这也有缺点。它可以形成河流环路，水从水体中流出，然后再流回水体。这些循环可能很长也可能很短，但它们总是显而易见且不正确的。你也可能最终看到河床沿着大湖底部蜿蜒，这看起来很奇怪。

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-26/generating-rivers/generating-rivers.unitypackage)

## 2 温度

水只是决定单元格生物群落的一个因素。另一个非常重要的因素是温度。虽然我们可以像水一样模拟温度流动和扩散，但我们只需要一个复杂的因素来创造一个有趣的气候。因此，我们将保持温度的简单性，每个单元格测定一次。

### 2.1 纬度温度

纬度对温度的影响最为深远。赤道是热的，两极是冷的，两者之间有一个逐渐的过渡。让我们创建一个 `DetermineTemperature` 方法，返回给定单元格的温度。首先，我们将简单地使用单元格的 Z 坐标除以 Z 维度作为纬度，然后直接将其用作温度。

```c#
	float DetermineTemperature (HexCell cell) {
		float latitude = (float)cell.coordinates.Z / grid.cellCountZ;
		return latitude;
	}
```

在 `SetTerrainType` 中确定温度并将其用作地图数据。

```c#
	void SetTerrainType () {
		for (int i = 0; i < cellCount; i++) {
			HexCell cell = grid.GetCell(i);
			float temperature = DetermineTemperature(cell);
			cell.SetMapData(temperature);
			float moisture = climate[i].moisture;
			…
		}
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-26/temperature/latitude-temperature.jpg)

*纬度作为温度，南半球。*

我们得到的是从下到上增加的线性温度梯度。我们可以用它来表示南半球，极地地区在底部，赤道在顶部。但我们不需要覆盖整个半球。我们可以通过使用较小的温差或根本没有温差来表示较小的区域。为此，我们将使低温和高温可配置。我们将使用极端值作为默认值，将这些温度定义在 0-1 的范围内。

```c#
	[Range(0f, 1f)]
	public float lowTemperature = 0f;

	[Range(0f, 1f)]
	public float highTemperature = 1f;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-26/temperature/low-high-temperatures.png)

*温度滑块。*

使用纬度作为插值器，通过线性插值应用温度范围。当我们将纬度表示为 0 到 1 的值时，我们可以使用 `Mathf.LerpUnclamped`。

```c#
	float DetermineTemperature (HexCell cell) {
		float latitude = (float)cell.coordinates.Z / grid.cellCountZ;
		float temperature =
			Mathf.LerpUnclamped(lowTemperature, highTemperature, latitude);
		return temperature;
	}
```

请注意，我们不会费心强制要求低温确实低于高温。如果你愿意，你可以改变温度。

### 2.2 半球

我们现在可以通过翻转温度来代表南半球，也可能是北半球。但使用单独的配置选项在半球之间切换要方便得多。让我们为此创建一个枚举和字段。这样，我们还可以包含一个覆盖两个半球的选项，我们将将其设置为默认选项。

```c#
	public enum HemisphereMode {
		Both, North, South
	}

	public HemisphereMode hemisphere;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-26/temperature/hemisphere-choice.png)

*半球选择。*

如果我们想要北半球，我们可以通过从 1 中减去纬度来简单地反转纬度。为了覆盖两个半球，我们需要确保两极位于地图的顶部和底部，而赤道位于在中间。我们可以通过将纬度加倍来做到这一点，这正确地照顾了下半球，但使上半球从 1 变为 2。为了纠正这一点，当纬度超过 1 时，从 2 中减去纬度。

```c#
	float DetermineTemperature (HexCell cell) {
		float latitude = (float)cell.coordinates.Z / grid.cellCountZ;
		if (hemisphere == HemisphereMode.Both) {
			latitude *= 2f;
			if (latitude > 1f) {
				latitude = 2f - latitude;
			}
		}
		else if (hemisphere == HemisphereMode.North) {
			latitude = 1f - latitude;
		}

		float temperature =
			Mathf.LerpUnclamped(lowTemperature, highTemperature, latitude);
		return temperature;
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-26/temperature/both-hemispheres.jpg)

*两个半球。*

请注意，这也使得通过使用比高温更热的低温来创建赤道寒冷、两极炎热的异国情调地图成为可能。

### 2.3 越高越冷

除了纬度，海拔对温度也有明显的影响。平均而言，你走得越高，天气就越冷。我们可以将其转化为一个因素，就像我们对河流起源候选者所做的那样。在这种情况下，我们使用单元格的视图立面。此外，该系数随高度而减小，因此相对于水位，它是 1 减去海拔高度除以最大值。为防止因子在最高级别始终降至零，请将除数加 1。然后使用这个系数来缩放温度。

```c#
		float temperature =
			Mathf.LerpUnclamped(lowTemperature, highTemperature, latitude);

		temperature *= 1f - (cell.ViewElevation - waterLevel) /
			(elevationMaximum - waterLevel + 1f);

		return temperature;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-26/temperature/elevation.jpg)

*海拔影响温度。*

### 2.4 温度波动

通过添加随机温度波动，我们可以使温度梯度的简单性稍微不那么明显。一点随机性可以让它看起来更真实，但太多会让它看起来很随意。让我们将此温度抖动的强度设置为可配置的，表示为最大温度偏差，默认值为 0.1。

```c#
	[Range(0f, 1f)]
	public float temperatureJitter = 0.1f;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-26/temperature/jitter.png)

*温度抖动滑块。*

这些波动应该是平稳的，局部变化很小。我们可以使用我们的噪波纹理。所以调用 `HexMetrics.SampleNoise`，以单元格的位置为参数，按 0.1 缩放。让我们抓住 W 通道，将其居中，并按抖动因子对其进行缩放。然后将这个值加到我们之前确定的温度上。

```c#
		temperature *= 1f - (cell.ViewElevation - waterLevel) /
			(elevationMaximum - waterLevel + 1f);

		temperature +=
			(HexMetrics.SampleNoise(cell.Position * 0.1f).w * 2f - 1f) *
			temperatureJitter;

		return temperature;
```

![0.1](https://catlikecoding.com/unity/tutorials/hex-map/part-26/temperature/jittered-01.jpg) ![1](https://catlikecoding.com/unity/tutorials/hex-map/part-26/temperature/jittered-10.jpg)

*温度抖动设置为 0.1 和 1。*

我们可以通过随机选择使用四个噪声通道中的哪一个，为每个地图的抖动增加一些变化。在 `SetTerrainType` 中确定一次通道，然后在 `DeterminedTemperature` 中索引颜色通道。

```c#
	int temperatureJitterChannel;
	
	…
	
	void SetTerrainType () {
		temperatureJitterChannel = Random.Range(0, 4);
		for (int i = 0; i < cellCount; i++) {
			…
		}
	}
	
	float DetermineTemperature (HexCell cell) {
		…

		float jitter =
			HexMetrics.SampleNoise(cell.Position * 0.1f)[temperatureJitterChannel];

		temperature += (jitter * 2f - 1f) * temperatureJitter;

		return temperature;
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-26/temperature/different-jitter.jpg)

*不同温度抖动，最大强度。*

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-26/temperature/temperature.unitypackage)

## 3 生物群系

现在我们有了水分和温度数据，我们可以创建一个生物群落矩阵。通过对这个矩阵进行索引，我们可以将生物群落分配给所有单元格，与仅使用一个数据维度相比，可以创建更复杂的景观。

### 3.1 生物群系矩阵

有很多气候模型，但我们不会使用任何一个。我们只会保持简单，只关心看起来合理的东西。干燥意味着沙漠——冷或热——我们将使用沙子。寒冷潮湿意味着下雪。炎热潮湿意味着有很多植物，所以草。在这两者之间，我们有泰加林（taiga）或苔原（tundra），我们将用灰色的泥质来代表它们。一个 4×4 的矩阵应该为这些生物群落之间的过渡提供足够的空间。

以前，我们根据五个湿度带指定了地形类型。我们只需删除最干燥的带（高达 0.05），保留其他带。对于温度带，我们将使用高达 0.1、0.3、0.6 及以上的温度带。在静态数组中定义这些值，以便于参考。

```c#
	static float[] temperatureBands = { 0.1f, 0.3f, 0.6f };
					
	static float[] moistureBands = { 0.12f, 0.28f, 0.85f };
```

虽然我们只根据生物群落设置了地形类型，但我们也可以用它来确定其他事情。因此，让我们在 `HexMapGenerator` 中定义一个 `Biome` 结构来表示单个生物群落的配置。目前，它只包含地形索引和适当的构造函数方法。

```c#
	struct Biome {
		public int terrain;
		
		public Biome (int terrain) {
			this.terrain = terrain;
		}
	}
```

使用此结构创建一个包含我们的矩阵数据的静态数组。我们将使用湿度作为 X 维度，温度作为 Y 维度。用雪填充最低温度的一排，用苔原填充第二排，用草填充另外两排。然后将最干燥的列更改为沙漠，覆盖温度选择。

```c#
	static Biome[] biomes = {
		new Biome(0), new Biome(4), new Biome(4), new Biome(4),
		new Biome(0), new Biome(2), new Biome(2), new Biome(2),
		new Biome(0), new Biome(1), new Biome(1), new Biome(1),
		new Biome(0), new Biome(1), new Biome(1), new Biome(1)
	};
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-26/biomes/biome-matrix.png)

*生物群落矩阵，带 1D 数组索引。*

### 3.2 确定生物特征

要确定 `SetTerrainType` 中的单元格生物群落，请遍历温度和湿度带，以确定我们需要的矩阵索引。使用它们检索正确的生物群落并设置单元格的地形类型。

```c#
	void SetTerrainType () {
		temperatureJitterChannel = Random.Range(0, 4);
		for (int i = 0; i < cellCount; i++) {
			HexCell cell = grid.GetCell(i);
			float temperature = DetermineTemperature(cell);
//			cell.SetMapData(temperature);
			float moisture = climate[i].moisture;
			if (!cell.IsUnderwater) {
//				if (moisture < 0.05f) {
//					cell.TerrainTypeIndex = 4;
//				}
//				…
//				else {
//					cell.TerrainTypeIndex = 2;
//				}
				int t = 0;
				for (; t < temperatureBands.Length; t++) {
					if (temperature < temperatureBands[t]) {
						break;
					}
				}
				int m = 0;
				for (; m < moistureBands.Length; m++) {
					if (moisture < moistureBands[m]) {
						break;
					}
				}
				Biome cellBiome = biomes[t * 4 + m];
				cell.TerrainTypeIndex = cellBiome.terrain;
			}
			else {
				cell.TerrainTypeIndex = 2;
			}
		}
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-26/biomes/biome-terrain.jpg)

*基于生物群落矩阵的地形。*

### 3.3 调整生物群系

我们不限于矩阵中定义的生物群落。例如，矩阵将所有干燥的生物群落定义为沙质沙漠。但并非所有干燥的沙漠都充满了沙子。还有许多岩石沙漠，看起来很不一样。所以，让我们把沙漠单元格的一部分变成岩石。我们只需根据海拔高度来做这件事，理由是松散的沙子在较低的海拔地区发现，而在较高的海拔地区，你会遇到大部分裸露的岩石。

假设如果一个单元格的高度比水位更接近最高高度，沙子就会变成岩石。这是岩石沙漠高程线，我们可以在 `SetTerrainType` 开始时计算。然后，如果我们遇到一个有沙子的单元格，并且它的海拔足够高，就把它的生物群落地形变成岩石。

```c#
	void SetTerrainType () {
		temperatureJitterChannel = Random.Range(0, 4);
		int rockDesertElevation =
			elevationMaximum - (elevationMaximum - waterLevel) / 2;
		
		for (int i = 0; i < cellCount; i++) {
			…
			if (!cell.IsUnderwater) {
				…
				Biome cellBiome = biomes[t * 4 + m];

				if (cellBiome.terrain == 0) {
					if (cell.Elevation >= rockDesertElevation) {
						cellBiome.terrain = 3;
					}
				}

				cell.TerrainTypeIndex = cellBiome.terrain;
			}
			else {
				cell.TerrainTypeIndex = 2;
			}
		}
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-26/biomes/rock-deserts.jpg)

*沙石沙漠。*

另一个基于海拔的调整是强制处于最高海拔的单元格变成雪盖，无论它们有多暖和，只要它们不太干燥。这使得雪盖更有可能出现在炎热潮湿的赤道附近。

```c#
				if (cellBiome.terrain == 0) {
					if (cell.Elevation >= rockDesertElevation) {
						cellBiome.terrain = 3;
					}
				}
				else if (cell.Elevation == elevationMaximum) {
					cellBiome.terrain = 4;
				}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-26/biomes/snowcaps.jpg)

*最高海拔的雪帽。*

### 3.4 植物

现在我们已经处理了地形类型，让我们的生物群落决定单元格的植物水平。这要求我们在 `Biome` 中添加一个植物字段，并将其包含在构造函数中。

```c#
	struct Biome {
		public int terrain, plant;

		public Biome (int terrain, int plant) {
			this.terrain = terrain;
			this.plant = plant;
		}
	}
```

最寒冷、最干燥的生物群落根本没有植物。除此之外，天气越暖和越潮湿，我们得到的植物就越多。第二个水分列只得到最热行的植物水平 1，所以 [0,0,0,1]。除雪外，第三列将级别增加 1，因此 [0,1,1,2]。最湿的列再次增加它们，因此 [0,2,2,3]。调整 `biomes` 数组以包括这种植物配置。

```c#
	static Biome[] biomes = {
		new Biome(0, 0), new Biome(4, 0), new Biome(4, 0), new Biome(4, 0),
		new Biome(0, 0), new Biome(2, 0), new Biome(2, 1), new Biome(2, 2),
		new Biome(0, 0), new Biome(1, 0), new Biome(1, 1), new Biome(1, 2),
		new Biome(0, 0), new Biome(1, 1), new Biome(1, 2), new Biome(1, 3)
	};
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-26/biomes/biome-matrix-plants.png)

*植物水平的生物群落基质。*

现在我们也可以设置单元格的植物水平。

```c#
				cell.TerrainTypeIndex = cellBiome.terrain;
				cell.PlantLevel = cellBiome.plant;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-26/biomes/plants.jpg)

*有植物的生物群落。*

> **这些植物看起来和以前不一样了吗？**
>
> 我稍微放大了大多数植物预制件，以便从远处更好地看到它们。两个低植物预制件的比例为（1, 2, 1）和（0.75, 1, 0.75）。中等为（1.5，3，1.5）和（2，1.5，2）。最高的是（2，4.5，2）和（2.5，3，2.5）。
>
> 我还把植物的颜色调暗了一点，以便更好地与纹理结合，使用（13，114，0）。

我们也可以调整生物群落的植物水平。首先，我们应该确保它们不会出现在雪地上，我们可能已经对雪地进行了调整。其次，如果水位还没有达到最高点，让我们也增加河流沿岸的植物等级。

```c#
				if (cellBiome.terrain == 4) {
					cellBiome.plant = 0;
				}
				else if (cellBiome.plant < 3 && cell.HasRiver) {
					cellBiome.plant += 1;
				}

				cell.TerrainTypeIndex = cellBiome.terrain;
				cell.PlantLevel = cellBiome.plant;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-26/biomes/tweaked-plants.jpg)

*调整植物。*

### 3.5 水下生物群落

到目前为止，我们完全忽略了水下单元格。让我们也给这些单元格增加一些种类，而不是对所有单元格都使用泥浆。一个简单的基于海拔的方法应该已经带来了更有趣的东西。例如，让我们用草来制作海拔低于水位一步的单元格。让我们也用草来建造比这更高的单元格，这些单元格是由河流形成的湖泊。负海拔的单元格位于深处，让我们用岩石来做这件事。所有其他单元格都可以保持泥浆状态。

```c#
	void SetTerrainType () {
			…
			if (!cell.IsUnderwater) {
				…
			}
			else {
				int terrain;
				if (cell.Elevation == waterLevel - 1) {
					terrain = 1;
				}
				else if (cell.Elevation >= waterLevel) {
					terrain = 1;
				}
				else if (cell.Elevation < 0) {
					terrain = 3;
				}
				else {
					terrain = 2;
				}
				cell.TerrainTypeIndex = terrain;
			}
		}
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-26/biomes/underwater.jpg)

*水下多样性。*

让我们为沿海的水下单元格添加更多细节。这些细胞至少有一个邻居在水面上。如果这样的细胞很浅，它可能有一个海滩。或者，如果它靠近悬崖，那么悬崖是一个主要的视觉特征，我们可以用岩石代替。

要弄清楚这一点，请检查水位以下一步的单元格的邻居。数一数有多少悬崖和斜坡与陆地邻居相连。

```c#
				if (cell.Elevation == waterLevel - 1) {
					int cliffs = 0, slopes = 0;
					for (
						HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++
					) {
						HexCell neighbor = cell.GetNeighbor(d);
						if (!neighbor) {
							continue;
						}
						int delta = neighbor.Elevation - cell.WaterLevel;
						if (delta == 0) {
							slopes += 1;
						}
						else if (delta > 0) {
							cliffs += 1;
						}
					}
					terrain = 1;
				}
```

现在我们可以利用这些信息对单元格进行分类。首先，如果它的一半以上的邻居是陆地，那么我们就在处理一个湖泊或小海湾。让我们为这些单元格使用草纹理。如果不是这样，那么如果我们有悬崖，我们就用岩石。否则，如果我们有斜坡，就用沙子来建造海滩。唯一的另一种选择是远离海岸的浅水区，我们将坚持用草。

```c#
				if (cell.Elevation == waterLevel - 1) {
					int cliffs = 0, slopes = 0;
					for (
						HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++
					) {
						…
					}
					if (cliffs + slopes > 3) {
						terrain = 1;
					}
					else if (cliffs > 0) {
						terrain = 3;
					}
					else if (slopes > 0) {
						terrain = 0;
					}
					else {
						terrain = 1;
					}
				}
```

![map](https://catlikecoding.com/unity/tutorials/hex-map/part-26/biomes/coastal.jpg) ![detail](https://catlikecoding.com/unity/tutorials/hex-map/part-26/biomes/coastal-detail.jpg)

*沿海多样性。*

作为最后的调整，让我们确保在最冷的温度带内不会出现绿色的水下单元格。用泥代替这些细胞。

```c#
				if (terrain == 1 && temperature < temperatureBands[0]) {
					terrain = 2;
				}
				cell.TerrainTypeIndex = terrain;
```

我们现在有能力生成看起来相当有趣和自然的随机地图，并有很多配置选项。下一个教程是[包覆](https://catlikecoding.com/unity/tutorials/hex-map/part-27/)。

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-26/biomes/biomes.unitypackage)

[PDF](https://catlikecoding.com/unity/tutorials/hex-map/part-26/Hex-Map-26.pdf)

# Hex Map 27：包覆

发布于 2018-03-13

https://catlikecoding.com/unity/tutorials/hex-map/part-27/

*将地图拆分为可以移动的列。*
*把地图放在相机的中心。*
*把所有东西都包起来。*

这是关于[六边形地图](https://catlikecoding.com/unity/tutorials/hex-map/)的系列教程的第 27 部分。[上一部分](https://catlikecoding.com/unity/tutorials/hex-map/part-26/)完成了程序地形生成器。在最后一期中，我们通过连接东边缘和西边缘来添加对包裹地图的支持。

本教程是用 Unity 2017.3.0p3 编写的。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-27/tutorial-image.jpg)

*包裹使世界变圆。*

## 1 包覆地图

我们的地图可以用来表示不同大小的区域，但它们总是被限制为矩形。我们可以为一个岛屿或整个大陆绘制地图，但不能为整个星球绘制地图。行星是球形的，没有硬边界来阻挡其表面的旅行。继续朝一个方向前进，在某个时候，你会回到起点。

我们不能将六边形网格包裹在球体周围，这样的瓷砖是不可能的。最好的近似方法是使用二十面体拓扑，它要求十二个单元是五边形。然而，将网格包裹在圆柱体周围是可能的，不会出现扭曲或异常。这只是一个连接地图东西两侧的问题。除了包覆逻辑，其他一切都可以保持不变。

圆柱体不能很好地近似球体，因为它不能表示极点。但这并没有阻止许多游戏使用东西向包装来表示行星地图。极地地区根本不是可玩区域的一部分。

> **把南北也包起来怎么样？**
>
> 如果你把东西和南北都包起来，你最终会得到一个环面的拓扑结构。因此，这不是球体的有效表示，尽管有些游戏使用了这种包裹方法。本教程仅涵盖东西向包裹，但您也可以使用相同的方法添加南北向包裹。它只需要更多的工作和其他指标。

有两种方法可以实现圆柱形包裹。第一种方法是将地图做成圆柱形，弯曲其表面和上面的所有东西，使东西两侧相互接触。你不再在平坦的表面上玩，而是在一个真正的圆柱体上玩。第二种方法是坚持使用平面地图，并使用传送或复制来使包裹工作。大多数游戏都使用第二种方式，我们也会使用。

### 1.1 可选包覆

你是否想要一张包裹地图取决于你是选择局部还是行星尺度。我们可以通过将包装设置为可选来支持这两种方式。在“创建新地图”菜单中添加一个新的切换，使其成为可能，并将包裹作为默认选项。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-27/wrapping-maps/new-map-menu.png)

*带有环绕选项的新地图菜单。*

在 `NewMapMenu` 中添加一个字段来跟踪此选项，以及一个更改它的方法。让新的切换在状态更改时调用此方法。

```c#
	bool wrapping = true;

	…

	public void ToggleWrapping (bool toggle) {
		wrapping = toggle;
	}
```

当请求新地图时，传递它是否应该包覆。

```c#
	void CreateMap (int x, int z) {
		if (generateMaps) {
			mapGenerator.GenerateMap(x, z, wrapping);
		}
		else {
			hexGrid.CreateMap(x, z, wrapping);
		}
		HexMapCamera.ValidatePosition();
		Close();
	}
```

调整 `HexMapGenerator.GenerateMap` 接受这个新参数，然后将其传递给 `HexGrid.CreateMap`。

```c#
	public void GenerateMap (int x, int z, bool wrapping) {
		…
		grid.CreateMap(x, z, wrapping);
		…
	}
```

`HexGrid` 应该知道它当前是否正在包覆，所以给它一个字段，并让 `CreateMap` 设置它。其他类需要根据网格是否包覆来更改其逻辑，因此将该字段公开。这也使得可以通过检查器设置默认值。

```c#
	public int cellCountX = 20, cellCountZ = 15;
	
	public bool wrapping;

	…

	public bool CreateMap (int x, int z, bool wrapping) {
		…

		cellCountX = x;
		cellCountZ = z;
		this.wrapping = wrapping;
		…
	}
```

`HexGrid` 在两个地方调用自己的 `CreateMap`。我们可以只使用它自己的字段作为包装参数。

```c#
	void Awake () {
		…
		CreateMap(cellCountX, cellCountZ, wrapping);
	}
	
	…
	
	public void Load (BinaryReader reader, int header) {
		…
		if (x != cellCountX || z != cellCountZ) {
			if (!CreateMap(x, z, wrapping)) {
				return;
			}
		}

		…
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-27/wrapping-maps/grid-wrapping.png)

*网格包裹切换，默认启用。*

### 1.2 保存和加载

因为包覆是按地图定义的，所以也应该保存和加载。这意味着我们必须调整保存文件格式，因此在 `SaveLoadMenu` 中增加版本常数。

```c#
	const int mapFileVersion = 5;
```

保存时，让 `HexGrid` 在地图维度后简单地写入包覆布尔值。

```c#
	public void Save (BinaryWriter writer) {
		writer.Write(cellCountX);
		writer.Write(cellCountZ);
		writer.Write(wrapping);

		…
	}
```

加载时，只有在我们必须纠正文件版本时才能读取它。如果没有，我们有一张旧地图，所以它不会包裹。将此信息存储在局部变量中，并将其与正确的包装状态进行比较。如果它不同，我们就不能重用现有的地图拓扑，就像我们加载不同的维度一样。

```c#
	public void Load (BinaryReader reader, int header) {
		ClearPath();
		ClearUnits();
		int x = 20, z = 15;
		if (header >= 1) {
			x = reader.ReadInt32();
			z = reader.ReadInt32();
		}
		bool wrapping = header >= 5 ? reader.ReadBoolean() : false;
		if (x != cellCountX || z != cellCountZ || this.wrapping != wrapping) {
			if (!CreateMap(x, z, wrapping)) {
				return;
			}
		}

		…
	}
```

### 1.3 包覆指标（Metrics）

包覆地图需要对逻辑进行一些更改，例如在计算距离时。因此，这可能会影响没有直接引用网格的代码。与其一直将此信息作为参数传递，不如将其添加到 `HexMetrics` 中。引入一个静态整数，其中包含与地图宽度匹配的包裹大小。如果它大于零，那么我们就有一个包装地图。添加一个方便的属性来检查这一点。

```c#
	public static int wrapSize;

	public static bool Wrapping {
		get {
			return wrapSize > 0;
		}
	}
```

每次调用 `HexGrid.CreateMap` 时，我们都必须设置包装大小。

```c#
	public bool CreateMap (int x, int z, bool wrapping) {
		…
		this.wrapping = wrapping;
		HexMetrics.wrapSize = wrapping ? cellCountX : 0;
		…
	}
```

由于此数据在游玩模式下无法重新编译，请在 `OnEnable` 中设置它。

```c#
	void OnEnable () {
		if (!HexMetrics.noiseSource) {
			HexMetrics.noiseSource = noiseSource;
			HexMetrics.InitializeHashGrid(seed);
			HexUnit.unitPrefab = unitPrefab;
			HexMetrics.wrapSize = wrapping ? cellCountX : 0;
			ResetVisibility();
		}
	}
```

### 1.4 单元格宽度

在处理包覆地图时，我们将处理很多沿 X 维度的位置，以单元格宽度为单位进行测量。虽然我们可以使用 `HexMetrics.innerRadius * 2f` 来实现这一点，但如果我们不必一直加乘法，那就很方便了。因此，让我们添加一个额外的 `HexMetrics.innerDiameter` 常数。

```c#
	public const float innerRadius = outerRadius * outerToInner;

	public const float innerDiameter = innerRadius * 2f;
```

我们已经可以在三个地方使用直径。首先，在 `HexGrid.CreateCell` 中，定位新单元格时。

```c#
	void CreateCell (int x, int z, int i) {
		Vector3 position;
		position.x = (x + z * 0.5f - z / 2) * HexMetrics.innerDiameter;
		…
	}
```

其次，在 `HexMapCamera` 中，夹紧相机的位置时。

```c#
	Vector3 ClampPosition (Vector3 position) {
		float xMax = (grid.cellCountX - 0.5f) * HexMetrics.innerDiameter;
		position.x = Mathf.Clamp(position.x, 0f, xMax);

		…
	}
```

在从位置转换为坐标时，也可以使用 `HexCoordinates`。

```c#
	public static HexCoordinates FromPosition (Vector3 position) {
		float x = position.x / HexMetrics.innerDiameter;
		…
	}
```

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-27/wrapping-maps/wrapping-maps.unitypackage)

## 2 将地图居中

当地图没有包裹时，它有一个明确的东边缘和西边缘，因此也有一个清晰的水平中心。包装图的情况并非如此。它没有东西边缘，所以也没有中心。或者，我们可以说中心是相机恰好所在的地方。这很有用，因为我们希望地图始终以我们的视角为中心。那么，无论我们在哪里，我们都看不到地图的东边缘或西边缘。

### 2.1 块列

为了使地图可视化以相机为中心，我们必须根据相机的移动改变物体的位置。如果它向西移动，我们必须把目前远东地区的东西移到远西侧。相反的方向也是如此。

理想情况下，每当相机移动到相邻的单元格列时，我们会立即将最远的单元格列移植到另一侧。然而，我们不需要如此精确。相反，我们可以移植整个地图块。这允许我们移动地图的一部分，而无需更改任何网格。

由于我们将同时移动整列块，让我们通过为每组创建一个列父对象来对它们进行分组。将这些对象的数组添加到 `HexGrid` 中，并在 `CreateChunks` 中对其进行初始化。我们只将它们用作容器，因此我们只需要跟踪对其 `Transform` 组件的引用。就像块一样，它们的初始位置都在网格的局部原点。

```c#
	Transform[] columns;
	
	…
	
	void CreateChunks () {
		columns = new Transform[chunkCountX];
		for (int x = 0; x < chunkCountX; x++) {
			columns[x] = new GameObject("Column").transform;
			columns[x].SetParent(transform, false);
		}
		
		…
	}
```

Chunk 现在应该成为相应列的子项，而不是网格。

```c#
	void CreateChunks () {
		…
		
		chunks = new HexGridChunk[chunkCountX * chunkCountZ];
		for (int z = 0, i = 0; z < chunkCountZ; z++) {
			for (int x = 0; x < chunkCountX; x++) {
				HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
				chunk.transform.SetParent(columns[x], false);
			}
		}
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-27/centering-the-map/columns.png)

*成列的块状物。*

因为所有块现在都是列的子级，所以我们可以直接销毁所有列，而不是 `CreateMap` 中的块。这也将摆脱块的孩子。

```c#
	public bool CreateMap (int x, int z, bool wrapping) {
		…
		if (columns != null) {
			for (int i = 0; i < columns.Length; i++) {
				Destroy(columns[i].gameObject);
			}
		}

		…
	}
```

### 2.2 传送列

在 `HexGrid` 中添加一个新的 `CenterMap` 方法，并将 X 位置作为参数。通过将位置除以块宽度（单位）将其转换为列索引。这是相机当前所在列的索引，这意味着它应该是地图的中心列。

```c#
	public void CenterMap (float xPosition) {
		int centerColumnIndex = (int)
			(xPosition / (HexMetrics.innerDiameter * HexMetrics.chunkSizeX));
	}
```

我们只需在中心列索引更改时调整地图可视化。所以，让我们在字段里追踪它。使用默认值 -1，创建地图时，新地图将始终居中。

```c#
	int currentCenterColumnIndex = -1;
	
	…
	
	public bool CreateMap (int x, int z, bool wrapping) {
		…
		this.wrapping = wrapping;
		currentCenterColumnIndex = -1;
		…
	}
	
	…
	
	public void CenterMap (float xPosition) {
		int centerColumnIndex = (int)
			(xPosition / (HexMetrics.innerDiameter * HexMetrics.chunkSizeX));
		
		if (centerColumnIndex == currentCenterColumnIndex) {
			return;
		}
		currentCenterColumnIndex = centerColumnIndex;
	}
```

现在我们知道了中心列索引，我们也可以通过简单地减去和加一半的列来确定最小和最大索引。当我们使用整数时，当我们有奇数列时，这会完美地工作。在偶数的情况下，不可能有一个完全居中的列，因此其中一个索引会离得太远。这会导致最远地图边缘方向上的单列偏差，但这不是问题。

```c#
		currentCenterColumnIndex = centerColumnIndex;

		int minColumnIndex = centerColumnIndex - chunkCountX / 2;
		int maxColumnIndex = centerColumnIndex + chunkCountX / 2;
```

请注意，这些索引可以为负或大于自然最大列索引。如果相机最终靠近地图的自然中心，则最小值仅为零。我们的工作是移动列，使其与这些相对指数对齐。我们通过调整循环中每列的局部 X 坐标来实现这一点。

```c#
		int minColumnIndex = centerColumnIndex - chunkCountX / 2;
		int maxColumnIndex = centerColumnIndex + chunkCountX / 2;

		Vector3 position;
		position.y = position.z = 0f;
		for (int i = 0; i < columns.Length; i++) {
			position.x = 0f;
			columns[i].localPosition = position;
		}
```

对于每一列，检查其索引是否小于最小索引。如果是这样，它离中心太远了。它必须传送到地图的另一边。这是通过使其 X 坐标等于地图宽度来实现的。同样，如果列的索引大于最大索引，则它离中心太远，必须向另一个方向传送。

```c#
		for (int i = 0; i < columns.Length; i++) {
			if (i < minColumnIndex) {
				position.x = chunkCountX *
					(HexMetrics.innerDiameter * HexMetrics.chunkSizeX);
			}
			else if (i > maxColumnIndex) {
				position.x = chunkCountX *
					-(HexMetrics.innerDiameter * HexMetrics.chunkSizeX);
			}
			else {
				position.x = 0f;
			}
			columns[i].localPosition = position;
		}
```

### 2.3 移动相机

更改 `HexMapCamera.AdjustPosition`，使其在处理包装地图时调用 `WrapPosition` 而不是 `ClampPosition`。最初，只需将新的 `WrapPosition` 方法设置为 `ClampPosition` 的副本，唯一的区别是它在最后调用 `CenterMap`。

```c#
	void AdjustPosition (float xDelta, float zDelta) {
		…
		transform.localPosition =
			grid.wrapping ? WrapPosition(position) : ClampPosition(position);
	}

	…

	Vector3 WrapPosition (Vector3 position) {
		float xMax = (grid.cellCountX - 0.5f) * HexMetrics.innerDiameter;
		position.x = Mathf.Clamp(position.x, 0f, xMax);

		float zMax = (grid.cellCountZ - 1) * (1.5f * HexMetrics.outerRadius);
		position.z = Mathf.Clamp(position.z, 0f, zMax);

		grid.CenterMap(position.x);
		return position;
	}
```

要确保映射立即开始居中，请在 `OnEnable` 中调用 `ValidatePosition`。

```c#
	void OnEnable () {
		instance = this;
		ValidatePosition();
	}
```

*在地图居中的同时左右移动。*

当我们仍在夹紧相机的运动时，地图现在试图保持在相机的中心，根据需要传送块列。当使用小地图和缩小视图时，这一点很明显，但在大地图上，传送块在相机的视野之外。地图的原始东西边缘之所以明显，是因为它们之间还没有三角剖分。

若要也包裹相机，请在 `WrapPosition` 中取消对其 X 坐标的夹紧。相反，当 X 低于零时，继续按地图宽度增加 X，当它大于地图宽度时，继续减小 X。

```c#
	Vector3 WrapPosition (Vector3 position) {
//		float xMax = (grid.cellCountX - 0.5f) * HexMetrics.innerDiameter;
//		position.x = Mathf.Clamp(position.x, 0f, xMax);
		float width = grid.cellCountX * HexMetrics.innerDiameter;
		while (position.x < 0f) {
			position.x += width;
		}
		while (position.x > width) {
			position.x -= width;
		}

		float zMax = (grid.cellCountZ - 1) * (1.5f * HexMetrics.outerRadius);
		position.z = Mathf.Clamp(position.z, 0f, zMax);

		grid.CenterMap(position.x);
		return position;
	}
```

*包裹着相机在地图上移动。*

### 2.4 包裹着色器纹理

除了三角剖分间隙外，相机的包裹在游戏视图中应该不明显。然而，当这种情况发生时，一半的地形和水域会发生视觉变化。这是因为我们使用世界位置对这些纹理进行采样。突然传送一个块会改变纹理对齐。

我们可以通过确保纹理以块大小的倍数平铺来解决这个问题。块大小来自 `HexMetrics` 中的常量，因此让我们创建一个 ***HexMetrics.cginc*** 着色器包含文件，并将相关定义放入其中。基础平铺比例由块大小和外部单元格半径得出。如果你碰巧使用了不同的指标，你也必须调整这个文件。

```glsl
#define OUTER_TO_INNER 0.866025404
#define OUTER_RADIUS 10
#define CHUNK_SIZE_X 5
#define TILING_SCALE (1 / (CHUNK_SIZE_X * 2 * OUTER_RADIUS / OUTER_TO_INNER))
```

这导致瓷砖比例为 0.00866025404。如果我们使用该值的整数倍，则纹理不会受到块传送的影响。此外，一旦我们正确地对它们的连接进行三角剖分，东边缘和西边缘地图边缘的纹理将无缝对齐。

我们在 ***Terrain*** 着色器中使用了 0.02 作为 UV 比例。我们可以使用两倍的瓷砖比例，即 0.01732050808。它比以前小了一点，稍微放大了纹理，但在视觉上并没有明显的变化。

```glsl
		#include "../HexMetrics.cginc"
		#include "../HexCellData.cginc"

		…

		float4 GetTerrainColor (Input IN, int index) {
			float3 uvw = float3(
				IN.worldPos.xz * (2 * TILING_SCALE),
				IN.terrain[index]
			);
			…
		}
```

我们在 ***Roads*** 着色器中使用 0.025 作为噪声 UV。我们可以使用三倍的平铺比例，即 0.02598076212，这是一个非常接近的匹配。

```c#
		#include "HexMetrics.cginc"
		#include "HexCellData.cginc"

		…

		void surf (Input IN, inout SurfaceOutputStandardSpecular o) {
			float4 noise =
				tex2D(_MainTex, IN.worldPos.xz * (3 * TILING_SCALE));
			…
		}
```

最后，在 ***Water.cginc*** 中，我们使用 0.015 表示泡沫，0.025 表示波浪。我们可以再次将平铺比例替换为两倍和三倍。

```c#
#include "HexMetrics.cginc"

float Foam (float shore, float2 worldXZ, sampler2D noiseTex) {
	shore = sqrt(shore) * 0.9;

	float2 noiseUV = worldXZ + _Time.y * 0.25;
	float4 noise = tex2D(noiseTex, noiseUV * (2 * TILING_SCALE));

	…
}

…

float Waves (float2 worldXZ, sampler2D noiseTex) {
	float2 uv1 = worldXZ;
	uv1.y += _Time.y;
	float4 noise1 = tex2D(noiseTex, uv1 * (3 * TILING_SCALE)); 

	float2 uv2 = worldXZ;
	uv2.x += _Time.y;
	float4 noise2 = tex2D(noiseTex, uv2 * (3 * TILING_SCALE));

	…
}
```

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-27/centering-the-map/centering-the-map.unitypackage)

## 3 连接东西方

此时，我们包裹地图的唯一视觉线索是最东端和最西端列之间的小间隙。存在这种差距是因为我们目前没有对非包裹贴图相对侧的单元格之间的边和角连接进行三角剖分。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-27/connecting-east-and-west/gap.png)

*边缘间隙。*

### 3.1 包裹邻居

为了对东西连接进行三角剖分，我们必须使地图两侧的单元格彼此相邻。我们目前没有这样做，因为在 `HexGrid.CreateCell` 中，如果前一个单元格的 X 索引大于零，我们只会与它建立 E-W 关系。要包装这种关系，我们还必须在启用包装时将行的最后一个单元格与同一行的第一个单元格连接起来。

```c#
	void CreateCell (int x, int z, int i) {
		…

		if (x > 0) {
			cell.SetNeighbor(HexDirection.W, cells[i - 1]);
			if (wrapping && x == cellCountX - 1) {
				cell.SetNeighbor(HexDirection.E, cells[i - x]);
			}
		}
		…
	}
```

在建立了 E-W 邻居关系后，我们现在可以在间隙上进行部分三角剖分。边缘连接并不完美，因为扰动没有正确平铺。我们稍后再处理。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-27/connecting-east-and-west/e-w.png)

*E-W 连接。*

我们还必须包装东北-西南关系。我们可以通过将每一偶数行的第一个单元格与前一行的最后一个单元格连接来实现这一点。这只是前一个单元格。

```c#
		if (z > 0) {
			if ((z & 1) == 0) {
				cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX]);
				if (x > 0) {
					cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX - 1]);
				}
				else if (wrapping) {
					cell.SetNeighbor(HexDirection.SW, cells[i - 1]);
				}
			}
			else {
				…
			}
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-27/connecting-east-and-west/ne-sw.png)

*NE–SW 连接。*

最后，在第一行之外的每一奇数行的末尾建立 SE-NW 连接。这些单元格将连接到前一行的第一个单元格。

```c#
		if (z > 0) {
			if ((z & 1) == 0) {
				…
			}
			else {
				cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX]);
				if (x < cellCountX - 1) {
					cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX + 1]);
				}
				else if (wrapping) {
					cell.SetNeighbor(
						HexDirection.SE, cells[i - cellCountX * 2 + 1]
					);
				}
			}
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-27/connecting-east-and-west/se-nw.png)

*东南-西北连接。*

### 3.2 包裹噪音

为了使间隙完美，我们必须确保用于扰动顶点位置的噪声在地图的东边缘和西边缘匹配。我们可以使用与着色器相同的技巧，但用于扰动的噪声尺度为 0.003。我们必须大幅放大它以使其平铺，这将使扰动更加不稳定。

另一种方法是不平铺噪波，而是在地图边缘交叉淡化噪波。如果我们在单个单元的宽度上交叉衰减，那么扰动将平滑过渡，没有不连续性。该区域的噪声将被稍微平滑，从远处看，变化会突然出现，但当用于一点顶点扰动时，这并不明显。

> **温度抖动怎么办？**
>
> 在生成映射时，我们还使用相同的噪声来抖动温度。突然的交叉衰减在这里可能更明显，但只有在使用强抖动时。由于抖动只是为了增加一点微妙的变化，因此这种限制是可以接受的。如果你想要强烈的抖动，你必须跨越更大的距离。

如果我们不包装地图，我们可以在 `HexMetrics.SampleNoise` 中只取一个样本。但在包装时，我们必须添加交叉渐变。因此，在返回样本之前，先将其存储在变量中。

```c#
	public static Vector4 SampleNoise (Vector3 position) {
		Vector4 sample = noiseSource.GetPixelBilinear(
			position.x * noiseScale,
			position.z * noiseScale
		);
		return sample;
	}
```

包装时，我们需要第二个采样来混合。我们将在地图的东侧执行转换，因此第二个样本必须在西侧采集。

```c#
		Vector4 sample = noiseSource.GetPixelBilinear(
			position.x * noiseScale,
			position.z * noiseScale
		);

		if (Wrapping && position.x < innerDiameter) {
			Vector4 sample2 = noiseSource.GetPixelBilinear(
				(position.x + wrapSize * innerDiameter) * noiseScale,
				position.z * noiseScale
			);
		}
```

交叉渐变是通过简单的线性插值完成的，从西侧到东侧，横跨单个单元格的宽度。

```c#
		if (Wrapping && position.x < innerDiameter) {
			Vector4 sample2 = noiseSource.GetPixelBilinear(
				(position.x + wrapSize * innerDiameter) * noiseScale,
				position.z * noiseScale
			);
			sample = Vector4.Lerp(
				sample2, sample, position.x * (1f / innerDiameter)
			);
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-27/connecting-east-and-west/blending.png)

*混合噪音，不完美*

结果并不完全匹配。这是因为东侧的部分单元格具有负 X 坐标。为了远离这个区域，让我们将过渡区域向西移动半个单元格宽度。

```c#
		if (Wrapping && position.x < innerDiameter * 1.5f) {
			Vector4 sample2 = noiseSource.GetPixelBilinear(
				(position.x + wrapSize * innerDiameter) * noiseScale,
				position.z * noiseScale
			);
			sample = Vector4.Lerp(
				sample2, sample, position.x * (1f / innerDiameter) - 0.5f
			);
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-27/connecting-east-and-west/blending-offset.png)

*正确的交叉渐变。*

### 3.3 编辑单元格

现在看来我们已经进行了正确的三角剖分，让我们确保可以在地图上和包裹接缝上的任何地方进行编辑。事实证明，传送的块上的坐标是错误的，较大的画笔被接缝切掉了。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-27/connecting-east-and-west/incomplete-brush.jpg)

*刷子被剪掉了。*

为了解决这个问题，我们必须让 `HexCoordinates` 知道包装。我们可以通过在构造函数方法中验证 X 坐标来实现这一点。我们知道，轴向 X 坐标是从 X 偏移坐标减去 Z 坐标的一半得出的。我们可以利用这些知识进行转换，并检查偏移坐标是否低于零。如果是这样，我们有一个超出展开地图东侧的坐标。由于我们在每个方向上传送最多一半的地图，因此只需将包裹大小添加到 X 一次即可。当偏移坐标大于包裹尺寸时，我们必须减去。

```c#
	public HexCoordinates (int x, int z) {
		if (HexMetrics.Wrapping) {
			int oX = x + z / 2;
			if (oX < 0) {
				x += HexMetrics.wrapSize;
			}
			else if (oX >= HexMetrics.wrapSize) {
				x -= HexMetrics.wrapSize;
			}
		}
		this.x = x;
		this.z = z;
	}
```

> **有时在地图底部或顶部编辑时会出现错误？**
>
> 当由于扰动，光标最终位于地图之外的单元格行中时，就会发生这种情况。这是一个错误，因为我们没有使用向量参数验证 `HexGrid.GetCell`中的坐标。修复方法是依赖于以坐标为参数的 `GetCell` 方法，该方法执行所需的检查。
>
> ```c#
> 	public HexCell GetCell (Vector3 position) {
> 		position = transform.InverseTransformPoint(position);
> 		HexCoordinates coordinates = HexCoordinates.FromPosition(position);
> //		int index =
> //			coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;
> //		return cells[index];
> 		return GetCell(coordinates);
> 	}
> ```

### 3.4 包裹海岸

三角剖分法适用于地形，但似乎东西向接缝缺少水岸边缘。他们实际上没有失踪，但他们没有包装。它们被翻转并一直延伸到地图的另一边。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-27/connecting-east-and-west/missing-water-edge.jpg)

*缺少水岸。*

这是因为我们在对岸边水域进行三角剖分时使用了邻居的位置。为了解决这个问题，我们必须检测到我们正在与地图另一边的邻居打交道。为了方便起见，我们将为 `HexCell` 添加一个单元格列索引的属性。

```c#
	public int ColumnIndex { get; set; }
```

在 `HexGrid.CreateCell` 中分配此索引。它只是等于 X 偏移坐标除以块大小。

```c#
	void CreateCell (int x, int z, int i) {
		…
		cell.Index = i;
		cell.ColumnIndex = x / HexMetrics.chunkSizeX;
		…
	}
```

现在我们可以检测到我们正在 `HexGridChunk.TriangulateWaterShore` 中包装，通过比较当前单元格及其相邻单元格的列索引。如果邻居的列索引小一步以上，那么我们在西侧，而邻居在东侧。所以我们必须把西边的邻居包起来。相反，对于另一个方向。

```c#
		Vector3 center2 = neighbor.Position;
		if (neighbor.ColumnIndex < cell.ColumnIndex - 1) {
			center2.x += HexMetrics.wrapSize * HexMetrics.innerDiameter;
		}
		else if (neighbor.ColumnIndex > cell.ColumnIndex + 1) {
			center2.x -= HexMetrics.wrapSize * HexMetrics.innerDiameter;
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-27/connecting-east-and-west/shore-edges.jpg)

*海岸边缘，但不是角落。*

这解决了岸边的问题，但还没有解决角落的问题。我们也必须对下一个邻居做同样的事情。

```c#
		if (nextNeighbor != null) {
			Vector3 center3 = nextNeighbor.Position;
			if (nextNeighbor.ColumnIndex < cell.ColumnIndex - 1) {
				center3.x += HexMetrics.wrapSize * HexMetrics.innerDiameter;
			}
			else if (nextNeighbor.ColumnIndex > cell.ColumnIndex + 1) {
				center3.x -= HexMetrics.wrapSize * HexMetrics.innerDiameter;
			}
			Vector3 v3 = center3 + (nextNeighbor.IsUnderwater ?
				HexMetrics.GetFirstWaterCorner(direction.Previous()) :
				HexMetrics.GetFirstSolidCorner(direction.Previous()));
			…
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-27/connecting-east-and-west/shore-complete.jpg)

*正确包裹海岸。*

### 3.5 地图生成

地图的东西两侧是否连接也会影响地图的生成。当地图包裹时，生成算法也会包裹。这将导致不同的贴图，但当使用非零的***地图边框 X*** 时，包裹并不明显。

![without](https://catlikecoding.com/unity/tutorials/hex-map/part-27/connecting-east-and-west/generated-without-wrapping.jpg) ![with](https://catlikecoding.com/unity/tutorials/hex-map/part-27/connecting-east-and-west/generated-with-wrapping.jpg)

*默认大地图 1208905299，无包裹和有包裹。*

在包装时，使用 ***Map Border X*** 是没有意义的。但我们不能简单地取消它，因为这会合并区域。包装时，我们可以使用 ***RegionBorder***。

通过在所有情况下将 `mapBorderX` 的使用替换为 `borderX` 来创建区域调整 `HexMapGenerator.CreateRegions`。这个新变量将等于 `regionBorder` 或 `mapBorderX`，具体取决于地图是否正在包裹。我只展示了下面第一个案例的变化。

```c#
		int borderX = grid.wrapping ? regionBorder : mapBorderX;
		MapRegion region;
		switch (regionCount) {
		default:
			region.xMin = borderX;
			region.xMax = grid.cellCountX - borderX;
			region.zMin = mapBorderZ;
			region.zMax = grid.cellCountZ - mapBorderZ;
			regions.Add(region);
			break;
		…
		}
```

这会使这些地区分开，但只有当地图的东侧和西侧实际上有不同的地区时，这才是必要的。有两种情况并非如此。首先，当只有一个区域时。第二，当有两个区域水平分割地图时。在这些情况下，我们可以将 `borderX` 设置为零，允许陆地穿过东西向接缝。

```c#
		switch (regionCount) {
		default:
			if (grid.wrapping) {
				borderX = 0;
			}
			region.xMin = borderX;
			region.xMax = grid.cellCountX - borderX;
			region.zMin = mapBorderZ;
			region.zMax = grid.cellCountZ - mapBorderZ;
			regions.Add(region);
			break;
		case 2:
			if (Random.value < 0.5f) {
				…
			}
			else {
				if (grid.wrapping) {
					borderX = 0;
				}
				region.xMin = borderX;
				region.xMax = grid.cellCountX - borderX;
				region.zMin = mapBorderZ;
				region.zMax = grid.cellCountZ / 2 - regionBorder;
				regions.Add(region);
				region.zMin = grid.cellCountZ / 2 + regionBorder; 
				region.zMax = grid.cellCountZ - mapBorderZ;
				regions.Add(region);
			}
			break;
		…
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-27/connecting-east-and-west/wrapping-region.jpg)

*单包裹区域。*

乍一看，这可能看起来很好，但实际上沿着接缝存在不连续性。当将“侵蚀百分比”设置为零时，这一点变得更加明显。

![seam](https://catlikecoding.com/unity/tutorials/hex-map/part-27/connecting-east-and-west/terrain-seam.jpg) ![seam detail](https://catlikecoding.com/unity/tutorials/hex-map/part-27/connecting-east-and-west/terrain-seam-detail.jpg)

*不使用侵蚀揭示了地形接缝。*

不连续性的发生是因为接缝阻碍了地形块的生长。单元格到块中心的距离用于确定哪些单元格首先添加，而地图另一侧的单元格最终会非常远，所以它们几乎永远不会被包含在内。这当然是不正确的。我们必须使 `HexCoordinates.DistanceTo` 要知道包装地图。

我们通过将沿三个轴的绝对距离相加并将结果减半来计算 `HexCoordinates` 之间的距离。Z 距离始终正确，但 X 和 Y 距离可能会受到包裹的影响。那么，让我们从分别计算 X+Y 开始。

```c#
	public int DistanceTo (HexCoordinates other) {
//		return
//			((x < other.x ? other.x - x : x - other.x) +
//			(Y < other.Y ? other.Y - Y : Y - other.Y) +
//			(z < other.z ? other.z - z : z - other.z)) / 2;
		
		int xy =
			(x < other.x ? other.x - x : x - other.x) +
			(Y < other.Y ? other.Y - Y : Y - other.Y);

		return (xy + (z < other.z ? other.z - z : z - other.z)) / 2;
	}
```

确定包裹是否会为任意单元格产生较小的距离并不简单，因此让我们简单地计算当我们将另一个坐标包裹到西侧时的 X+Y。如果最终小于原始的 X+Y，请使用它。

```c#
		int xy =
			(x < other.x ? other.x - x : x - other.x) +
			(Y < other.Y ? other.Y - Y : Y - other.Y);

		if (HexMetrics.Wrapping) {
			other.x += HexMetrics.wrapSize;
			int xyWrapped =
				(x < other.x ? other.x - x : x - other.x) +
				(Y < other.Y ? other.Y - Y : Y - other.Y);
			if (xyWrapped < xy) {
				xy = xyWrapped;
			}
		}
```

如果这不会导致更短的距离，那么另一个方向的包裹可能更短，所以也要检查一下。

```c#
		if (HexMetrics.Wrapping) {
			other.x += HexMetrics.wrapSize;
			int xyWrapped =
				(x < other.x ? other.x - x : x - other.x) +
				(Y < other.Y ? other.Y - Y : Y - other.Y);
			if (xyWrapped < xy) {
				xy = xyWrapped;
			}
			else {
				other.x -= 2 * HexMetrics.wrapSize;
				xyWrapped =
					(x < other.x ? other.x - x : x - other.x) +
					(Y < other.Y ? other.Y - Y : Y - other.Y);
				if (xyWrapped < xy) {
					xy = xyWrapped;
				}
			}
		}
```

现在我们总是以包装图上的最短距离结束。地形块不再被接缝阻挡，使陆地有可能包裹起来。

![without](https://catlikecoding.com/unity/tutorials/hex-map/part-27/connecting-east-and-west/wrapped-without-erosion.jpg) ![with](https://catlikecoding.com/unity/tutorials/hex-map/part-27/connecting-east-and-west/wrapped-with-erosion.jpg)

*正确包裹地形，无侵蚀和有侵蚀。*

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-27/connecting-east-and-west/connecting-east-and-west.unitypackage)

## 4 环游世界

现在已经介绍了地图生成和三角剖分，剩下的就是检查单位、探索和可见性。

### 4.1 可探索接缝

当我们试图在世界各地移动一个单位时遇到的第一个障碍是无法探索的地图边缘。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-27/traveling-around-the-world/blocking-seam.jpg)

*无法探索地图接缝。*

地图边缘的单元格不可探索，以隐藏地图的突然结束。但当地图覆盖时，只需标记北和南单元格，而不必标记东和西单元格。调整 `HexGrid.CreateCell` 以考虑这一点。

```c#
		if (wrapping) {
			cell.Explorable = z > 0 && z < cellCountZ - 1;
		}
		else {
			cell.Explorable =
				x > 0 && z > 0 && x < cellCountX - 1 && z < cellCountZ - 1;
		}
```

### 4.2 特征可见性

接下来，让我们检查接缝的可见性是否正确。它适用于地形，但不适用于地形特征。看起来包裹的特征会获取最后一个未包裹的单元格的可见性。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-27/traveling-around-the-world/feature-visibility-incorrect.jpg)

*特征可见性不正确。*

这是因为 `HexCellShaderData` 使用的纹理的包裹模式设置为夹紧（clamp）。解决方案就是将其夹紧模式设置为重复。但我们只需要对 U 坐标执行此操作，因此在 `Initialize` 中分别设置 `wrappModeU` 和 `wrapModeV`。

```c#
	public void Initialize (int x, int z) {
		if (cellTexture) {
			cellTexture.Resize(x, z);
		}
		else {
			cellTexture = new Texture2D(
				x, z, TextureFormat.RGBA32, false, true
			);
			cellTexture.filterMode = FilterMode.Point;
//			cellTexture.wrapMode = TextureWrapMode.Clamp;
			cellTexture.wrapModeU = TextureWrapMode.Repeat;
			cellTexture.wrapModeV = TextureWrapMode.Clamp;
			Shader.SetGlobalTexture("_HexCellData", cellTexture);
		}
		…
	}
```

### 4.3 单位和列

另一个问题是，单位目前没有包装。当他们所在的列被重新定位时，他们会留在原地。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-27/traveling-around-the-world/unit-on-wrong-size.jpg)

*单位没有包好，站在错误的一边。*

这可以通过使单位成为列的子项来解决，就像块一样。首先，不要再让它们成为 `HexGrid.AddUnit` 中网格的直接子节点。

```c#
	public void AddUnit (HexUnit unit, HexCell location, float orientation) {
		units.Add(unit);
		unit.Grid = this;
//		unit.transform.SetParent(transform, false);
		unit.Location = location;
		unit.Orientation = orientation;
	}
```

因为单位会移动，它们可能会最终出现在不同的列中，这意味着我们必须更改它们的父级。为了实现这一点，请向 `HexGrid` 添加一个公共 `MakeChildOfColumn` 方法，并将子对象的 `Transform` 组件和列索引作为参数。

```c#
	public void MakeChildOfColumn (Transform child, int columnIndex) {
		child.SetParent(columns[columnIndex], false);
	}
```

当 `HexUnit.Location` 属性被设置时调用此方法。

```c#
	public HexCell Location {
		…
		set {
			…
			Grid.MakeChildOfColumn(transform, value.ColumnIndex);
		}
	}
```

这涉及到单位的创建。我们还必须确保他们在旅行时移动到正确的列。这要求我们跟踪 `HexUnit.TravelPath` 中的当前列索引。在该方法的开头，它是路径开头单元格的列索引，或者如果重新编译中断了旅行，则是当前的列索引。

```c#
	IEnumerator TravelPath () {
		Vector3 a, b, c = pathToTravel[0].Position;
		yield return LookAt(pathToTravel[1].Position);

//		Grid.DecreaseVisibility(
//			currentTravelLocation ? currentTravelLocation : pathToTravel[0],
//			VisionRange
//		);
		if (!currentTravelLocation) {
			currentTravelLocation = pathToTravel[0];
		}
		Grid.DecreaseVisibility(currentTravelLocation, VisionRange);
		int currentColumn = currentTravelLocation.ColumnIndex;

		…
	}
```

在旅程的每次迭代中，检查下一列索引是否不同，如果不同，则调整单位的父级。

```c#
		int currentColumn = currentTravelLocation.ColumnIndex;

		float t = Time.deltaTime * travelSpeed;
		for (int i = 1; i < pathToTravel.Count; i++) {
			…
			Grid.IncreaseVisibility(pathToTravel[i], VisionRange);

			int nextColumn = currentTravelLocation.ColumnIndex;
			if (currentColumn != nextColumn) {
				Grid.MakeChildOfColumn(transform, nextColumn);
				currentColumn = nextColumn;
			}

			…
		}
```

这使得单位像块一样包裹起来。然而，当在地图接缝上移动时，单位还没有包裹。相反，他们突然朝着错误的方向前进。无论接缝位于何处，都会发生这种情况，但当它们在整个地图上奔跑时，这种情况最为显著。

*在地图上跳跃。*

在这里，我们可以使用与水岸相同的方法，除了这次我们包覆单位行进的曲线。当下一列向东包裹时，我们也将曲线向东传送，对另一个方向也是如此。我们必须调整曲线的 `a` 和 `b` 控制点，这也照顾到 `c` 控制点。

```c#
		for (int i = 1; i < pathToTravel.Count; i++) {
			currentTravelLocation = pathToTravel[i];
			a = c;
			b = pathToTravel[i - 1].Position;
//			c = (b + currentTravelLocation.Position) * 0.5f;
//			Grid.IncreaseVisibility(pathToTravel[i], VisionRange);

			int nextColumn = currentTravelLocation.ColumnIndex;
			if (currentColumn != nextColumn) {
				if (nextColumn < currentColumn - 1) {
					a.x -= HexMetrics.innerDiameter * HexMetrics.wrapSize;
					b.x -= HexMetrics.innerDiameter * HexMetrics.wrapSize;
				}
				else if (nextColumn > currentColumn + 1) {
					a.x += HexMetrics.innerDiameter * HexMetrics.wrapSize;
					b.x += HexMetrics.innerDiameter * HexMetrics.wrapSize;
				}
				Grid.MakeChildOfColumn(transform, nextColumn);
				currentColumn = nextColumn;
			}

			c = (b + currentTravelLocation.Position) * 0.5f;
			Grid.IncreaseVisibility(pathToTravel[i], VisionRange);

			…
		}
```

*包裹运动。*

我们必须调整的最后一件事是单位的初始旋转，当它面对它将要前往的第一个单元格时。如果该单元格恰好位于东西接缝的另一侧，那么单元最终会朝错误的方向看。

当地图被包裹时，有两种方法可以查看不在正北或正南的点。你可以向东或向西看。朝着与点最近距离匹配的方向看是有意义的，因为这也是行进方向，所以让我们在 `LookAt` 中使用它。

包裹时，检查 X 维度中的相对距离。如果它小于地图宽度的负一半，那么我们应该向西看，这是通过将点向西包裹来实现的。否则，如果距离大于地图宽度的一半，那么我们应该向东环绕。

```c#
	IEnumerator LookAt (Vector3 point) {
		if (HexMetrics.Wrapping) {
			float xDistance = point.x - transform.localPosition.x;
			if (xDistance < -HexMetrics.innerRadius * HexMetrics.wrapSize) {
				point.x += HexMetrics.innerDiameter * HexMetrics.wrapSize;
			}
			else if (xDistance > HexMetrics.innerRadius * HexMetrics.wrapSize) {
				point.x -= HexMetrics.innerDiameter * HexMetrics.wrapSize;
			}
		}

		…
	}
```

此时，我们有一个功能齐全的包覆地图。这也结束了 Hex Map 系列。正如一些早期教程中提到的，可以涵盖更多主题，但它们并不特定于六边形地图。我可能会在他们自己的未来系列中介绍他们。享受你的地图！

> **我下载了最终的软件包，但在播放模式下出现了旋转错误？**
>
> 这是因为相机使用自定义***旋转***轴。您需要添加此轴。有关详细信息，请参阅[第 5 部分“大图”](https://catlikecoding.com/unity/tutorials/hex-map/part-5/)。

> **我下载了最终的软件包，得到的图形比截图还难看？**
>
> 我已经将我的项目设置为使用线性颜色空间。Gamme空间使其更加明亮。

> **我下载了最终的软件包，并且总是生成相同的地图？**
>
> 生成器设置为始终使用相同的固定种子 1208905299，这是大多数屏幕截图使用的种子。禁用“***使用固定种子***”使其随机。

现在还有 [Hex Map 项目](https://catlikecoding.com/unity/hex-map/)，它使 Hex Map 现代化并使用 URP。

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-27/traveling-around-the-world/traveling-around-the-world.unitypackage)

[PDF](https://catlikecoding.com/unity/tutorials/hex-map/part-27/Hex-Map-27.pdf)

# [跳转项目独立 Markdown 2.0.0 ~ 3.4.0](./CatlikeCoding网站翻译-六边形地图项目2.0.0~3.4.0.md)