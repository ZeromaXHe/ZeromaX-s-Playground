# [返回主 Markdown](./CatlikeCoding网站翻译.md)



# [跳转系列独立 Markdown 9 ~ 15](./CatlikeCoding网站翻译-六边形地图9~15.md)



# Hex Map 16：寻路

发布于2017-04-19

https://catlikecoding.com/unity/tutorials/hex-map/part-16/

*突出显示单元格。*
*选择搜索目的地。*
*找到最短的路径。*
*创建优先级队列。*

这是关于[六边形地图](https://catlikecoding.com/unity/tutorials/hex-map/)的系列教程的第 16 部分。在计算出单元格之间的距离后，我们继续寻找它们之间的路径。

从现在开始，Hex Map 教程是用 Unity 5.6.0 制作的。请注意，5.6 中有一个错误，在多个平台上的构建中破坏了纹理数组。解决方法是通过纹理数组的检查器启用 *Is Readable*。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-16/tutorial-image.jpg)

*计划一次旅行。*

## 1 突出显示单元格

要搜索两个单元格之间的路径，我们首先必须选择这些单元格。这不再是选择单个单元格并观察搜索在地图上传播的问题。例如，我们首先选择一个起始单元格，然后选择一个目标单元格。在做出这些选择后，突出显示它们会很方便。让我们添加这个功能。我们现在不会创造一种花哨或高效的突出显示方法，而只是一种快速的方法来帮助开发。

### 1.1 轮廓纹理

突出显示单元格的一种简单方法是为它们添加轮廓。最直接的方法是使用包含六边形轮廓的纹理。[这](https://catlikecoding.com/unity/tutorials/hex-map/part-16/highlighting-cells/cell-outline.png)就是这样的纹理。除了白色六边形轮廓外，它是透明的。通过使其变白，我们可以在以后根据需要对其进行着色。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-16/highlighting-cells/cell-outline.png)

*黑色背景上的单元格轮廓。*

导入纹理并将其*纹理类型*设置为 *Sprite*。它的 *Sprite 模式*是 *Single*，具有默认设置。因为它是纯白色纹理，我们不需要 *sRGB* 转换。alpha 通道代表透明度，因此启用 *Alpha* 即为*透明度*。我还启用了 mip 贴图，并将“*过滤模式*”设置为“*三线性*”，因为否则 mip 过渡对轮廓来说可能很明显。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-16/highlighting-cells/cell-outline-inspector.png)

*纹理导入设置。*

### 1.2 每个单元格一个精灵图

为每个单元格添加潜在轮廓的最快方法是为每个单元格分配自己的角色。创建一个新的游戏对象，并通过 *Component / UI / Image* 向其添加一个图像组件，并将我们的轮廓精灵分配给它。然后，在场景中放置一个 *Hex Cell Label* 预制实例，使精灵对象成为它的子对象，并将更改应用于预制件。然后删除该实例。

![hierarchy](https://catlikecoding.com/unity/tutorials/hex-map/part-16/highlighting-cells/sprite-hierarchy.png) ![inspector](https://catlikecoding.com/unity/tutorials/hex-map/part-16/highlighting-cells/sprite-inspector.png)

*突出显示预制子对象。*

现在每个单元格都有一个精灵，但它们太大了。要使轮廓适合单元格中心，请将精灵变换组件的宽度和高度更改为 17。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-16/highlighting-cells/highlight-sprites.png)

*突出显示精灵图，部分被地形遮挡。*

### 1.3 在万物之上作画

由于轮廓与单元边缘区域重叠，因此它通常位于地形几何体下方。这会导致轮廓的一部分消失。改变精灵的垂直位置可以防止小的海拔变化，但对于悬崖则不然。相反，我们能做的就是在其他一切之上画出轮廓。我们需要为此创建一个自定义的精灵着色器。复制 Unity 的默认精灵着色器并进行一些更改就足够了。

```glsl
Shader "Custom/Highlight" {
	Properties {
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		[HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
		[PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
		[PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
	}

	SubShader {
		Tags { 
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass {
			CGPROGRAM
			#pragma vertex SpriteVert
			#pragma fragment SpriteFrag
			#pragma target 2.0
			#pragma multi_compile_instancing
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#include "UnitySprites.cginc"
			ENDCG
		}
	}
}
```

第一个变化是忽略深度缓冲区，使 Z 测试始终成功。

```glsl
		ZWrite Off
		ZTest Always
```

第二个更改是在所有其他透明几何体之后绘制。在透明队列中添加 10 应该就足够了。

```c#
			"Queue"="Transparent+10"
```

创建使用此着色器的新材质。我们可以忽略它的所有属性，保留默认值。然后让我们的精灵使用这种材料预制。

![material](https://catlikecoding.com/unity/tutorials/hex-map/part-16/highlighting-cells/material-inspector.png) ![image](https://catlikecoding.com/unity/tutorials/hex-map/part-16/highlighting-cells/material-assigned.png)

*使用自定义精灵图材质。*

我们的突出显示现在总是可见的。即使一个单元格隐藏在更高的地形后面，它的轮廓仍然会绘制在其他一切之上。这可能不太美观，但它确保我们始终可以发现突出显示的单元格，这很有用。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-16/highlighting-cells/sprites-always.png)

*忽略深度缓冲区。*

### 1.4 控制突出显示

我们不希望所有单元格都一直高亮显示。事实上，我们一开始都不想强调这些问题。我们可以通过禁用 *Highlight* 预制对象的图像组件来实现这一点。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-16/highlighting-cells/highlight-disabled.png)

*禁用图像组件。*

要启用单元格的高亮显示，请向 `HexCell` 添加 `EnableHighlight` 方法。它必须获取其 `uiRect` 的唯一子项并启用其图像组件。同时创建 `DisableHighlight` 方法。

```c#
	public void DisableHighlight () {
		Image highlight = uiRect.GetChild(0).GetComponent<Image>();
		highlight.enabled = false;
	}
	
	public void EnableHighlight () {
		Image highlight = uiRect.GetChild(0).GetComponent<Image>();
		highlight.enabled = true;
	}
```

最后，我们还可以在启用时提供一种颜色来给高光着色。

```c#
	public void EnableHighlight (Color color) {
		Image highlight = uiRect.GetChild(0).GetComponent<Image>();
		highlight.color = color;
		highlight.enabled = true;
	}
```

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-16/highlighting-cells/highlighting-cells.unitypackage)

## 2 寻找路径

现在我们可以突出显示单元格，我们可以继续选择两个单元格，然后搜索它们之间的路径。首先，我们必须实际选择单元格，然后将搜索限制在查找路径上，最后显示该路径。

### 2.1 搜索开始

我们有两个不同的单元格可供选择，即搜索的起点和终点。假设要选择要搜索的单元格，您必须在单击时按住左 shift 键。这样做将用蓝色突出显示该单元格。我们必须保留这个单元格的引用，以便以后搜索。此外，当选择新的起始单元格时，应禁用旧单元格的突出显示。因此，在 `HexMapEditor` 中添加一个 `searchFromCell` 字段。

```c#
	HexCell previousCell, searchFromCell;
```

在 `HandleInput` 内部，我们可以使用 `Input.GetKey(KeyCode.LeftShift)` 用于检查换档键是否被按下。

```c#
			if (editMode) {
				EditCells(currentCell);
			}
			else if (Input.GetKey(KeyCode.LeftShift)) {
				if (searchFromCell) {
					searchFromCell.DisableHighlight();
				}
				searchFromCell = currentCell;
				searchFromCell.EnableHighlight(Color.blue);
			}
			else {
				hexGrid.FindDistancesTo(currentCell);
			}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-16/finding-a-path/searching-from.png)

*从哪里搜索。*

### 2.2 搜索目的地

我们现在不是寻找到一个单元格的所有距离，而是寻找两个特定单元格之间的路径。因此，将 `HexGrid.FindDistancesTo` 重命名为 `HexGrid.FindPath`，并为其提供第二个 `HexCell` 参数。同时调整 `Search` 方法。

```c#
	public void FindPath (HexCell fromCell, HexCell toCell) {
		StopAllCoroutines();
		StartCoroutine(Search(fromCell, toCell));
	}

	IEnumerator Search (HexCell fromCell, HexCell toCell) {
		for (int i = 0; i < cells.Length; i++) {
			cells[i].Distance = int.MaxValue;
		}

		WaitForSeconds delay = new WaitForSeconds(1 / 60f);
		List<HexCell> frontier = new List<HexCell>();
		fromCell.Distance = 0;
		frontier.Add(fromCell);
		…
	}
```

`HexMapEditor.HandleInput` 现在必须调用调整后的方法，使用 `searchFromCell` 和 `currentCell` 作为参数。此外，只有当我们知道从哪个单元格中搜索时，我们才能寻找路径。当目的地与起点相同时，我们不必费心寻找路径。

```c#
			if (editMode) {
				EditCells(currentCell);
			}
			else if (Input.GetKey(KeyCode.LeftShift)) {
				…
			}
			else if (searchFromCell && searchFromCell != currentCell) {
				hexGrid.FindPath(searchFromCell, currentCell);
			}
```

一旦我们开始搜索，我们应该首先删除所有之前的亮点。`HexGrid.Search` 也是如此，在重置距离时禁用高亮显示。由于这也会禁用起始单元格的高亮显示，请稍后再次启用它。此时，我们还可以突出显示目标单元格。让我们把它变成红色。

```c#
	IEnumerator Search (HexCell fromCell, HexCell toCell) {
		for (int i = 0; i < cells.Length; i++) {
			cells[i].Distance = int.MaxValue;
			cells[i].DisableHighlight();
		}
		fromCell.EnableHighlight(Color.blue);
		toCell.EnableHighlight(Color.red);
		
		…
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-16/finding-a-path/searching-to.png)

*潜在路径的端点。*

### 2.3 限制搜索

此时，我们的搜索算法仍然会计算从起始单元格到所有可到达单元格的距离。这已经没有必要了。一旦我们找到到目的地单元格的最终距离，我们就可以停下来。因此，当当前单元格是目标时，我们可以打破算法循环。

```c#
		while (frontier.Count > 0) {
			yield return delay;
			HexCell current = frontier[0];
			frontier.RemoveAt(0);

			if (current == toCell) {
				break;
			}

			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
				…
			}
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-16/finding-a-path/stopping.png)

*停在目的地。*

> **如果无法到达目的地怎么办？**
>
> 然后，算法继续进行，直到搜索到所有可到达的单元格。如果不能提前退出，它的行为将类似于旧的 `FindDistancesTo` 方法。

### 2.4 显示路径

我们可以找到路径起点和终点之间的距离，但我们还不知道实际的路径是什么。为此，我们必须跟踪每个单元格是如何到达的。我们如何做到这一点？

当将一个单元格添加到边界时，我们这样做是因为它是当前单元格的邻居。唯一的例外是起始单元格。所有其他单元格均通过当前单元格到达。如果我们跟踪每个单元格是从哪个单元格到达的，我们最终会得到一个单元格网络。特别是，一个以起始单元格为根的树网络。一旦我们到达目的地，我们就可以用它来构建一条路径。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-16/finding-a-path/path-tree.png)

*描述到中心的路径的树形网络。*

我们可以通过向 `HexCell` 添加另一个单元格引用来存储此信息。我们不需要序列化这些数据，所以让我们为其使用默认属性。

```c#
	public HexCell PathFrom { get; set; }
```

回到 `HexGrid.Search`，在将邻居的 `PathFrom` 添加到边界时，将其设置为当前单元格。当我们找到到邻居的较短路线时，我们还必须更改此引用。

```c#
				if (neighbor.Distance == int.MaxValue) {
					neighbor.Distance = distance;
					neighbor.PathFrom = current;
					frontier.Add(neighbor);
				}
				else if (distance < neighbor.Distance) {
					neighbor.Distance = distance;
					neighbor.PathFrom = current;
				}
```

到达目的地后，我们可以通过将这些引用追溯到起始单元格来可视化路径，并突出显示它们。

```c#
			if (current == toCell) {
				current = current.PathFrom;
				while (current != fromCell) {
					current.EnableHighlight(Color.white);
					current = current.PathFrom;
				}
				break;
			}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-16/finding-a-path/path.png)

*已找到一条路径。*

请注意，通常有多条最短路径。这取决于单元格被处理的顺序。有些路径可能看起来不错，有些可能看起来不好，但从来没有更短的路径。我们稍后再谈这个。

### 2.5 调整 Search 启动

一旦选择了起始单元格，更改目标单元格将触发新的搜索。选择新的起始单元格时也应如此。为了实现这一点，`HexMapEditor` 还必须记住目标单元格。

```c#
	HexCell previousCell, searchFromCell, searchToCell;
```

使用此字段，我们还可以在选择新开始时启动新的搜索。

```c#
			else if (Input.GetKey(KeyCode.LeftShift)) {
				if (searchFromCell) {
					searchFromCell.DisableHighlight();
				}
				searchFromCell = currentCell;
				searchFromCell.EnableHighlight(Color.blue);
				if (searchToCell) {
					hexGrid.FindPath(searchFromCell, searchToCell);
				}
			}
			else if (searchFromCell && searchFromCell != currentCell) {
				searchToCell = currentCell;
				hexGrid.FindPath(searchFromCell, searchToCell);
			}
```

此外，我们应该避免使起始单元格等于目标单元格。

```c#
			if (editMode) {
				EditCells(currentCell);
			}
			else if (
				Input.GetKey(KeyCode.LeftShift) && searchToCell != currentCell
			) {
				…
			}
```

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-16/finding-a-path/finding-a-path.unitypackage)

## 3 更智能的搜索

尽管我们的搜索算法找到了最短路径，但它花费了大量时间调查显然不属于该路径的单元格。至少对我们来说是显而易见的。该算法没有地图的高级视图。它看不出在某些方向上搜索是毫无意义的。它更喜欢沿着路走，即使它们远离目的地。我们能让它更聪明吗？

目前，在决定下一步处理哪个单元格时，我们只考虑单元格与开始的距离。如果我们想聪明地对待这件事，我们还必须考虑到到目的地的距离。不幸的是，我们还不知道这一点。但我们可以估算出剩余的距离。将其添加到单元距离中，可以指示通过该单元的总路径长度。然后，我们可以使用它来确定单元格的搜索优先级。

### 3.1 搜索启发式

当我们依赖于估计或猜测而不是确切已知的数据时，我们说我们使用了搜索启发式。这个启发式方法代表了我们对剩余距离的最佳猜测。我们必须为搜索的每个单元格确定这个值，因此将它的整数属性添加到 `HexCell` 中。我们不需要序列化它，所以我们可以用另一个默认属性来满足。

```c#
	public int SearchHeuristic { get; set; }
```

我们如何猜测剩余的距离？在最理想的情况下，会有一条路直通目的地。如果是这样，则该距离等于此单元格的坐标和目标单元格之间未修改的距离。让我们以此作为我们的启发。

由于启发式算法不依赖于到目前为止所走的路径，因此在搜索过程中它是恒定的。所以我们只需要计算一次，当 `HexGrid.Search` 在边界添加一个单元格时。

```c#
				if (neighbor.Distance == int.MaxValue) {
					neighbor.Distance = distance;
					neighbor.PathFrom = current;
					neighbor.SearchHeuristic =
						neighbor.coordinates.DistanceTo(toCell.coordinates);
					frontier.Add(neighbor);
				}
```

### 3.2 搜索优先级

从现在开始，我们将根据单元格距离加上启发式算法来确定搜索优先级。让我们为 `HexCell` 的这个值添加一个方便的属性。

```c#
	public int SearchPriority {
		get {
			return distance + SearchHeuristic;
		}
	}
```

要实现这一点，请调整 `HexGrid.Search`，使其使用此属性对边界进行排序。

```c#
				frontier.Sort(
					(x, y) => x.SearchPriority.CompareTo(y.SearchPriority)
				);
```

![without](https://catlikecoding.com/unity/tutorials/hex-map/part-16/smarter-searching/without-heuristic.png) ![with](https://catlikecoding.com/unity/tutorials/hex-map/part-16/smarter-searching/with-heuristic.png)

*无搜索与启发式搜索。*

### 3.3 可接受的启发式

使用我们新的搜索优先级，我们确实最终访问了更少的单元格。然而，在无特征的地图上，该算法仍然处理位于错误方向的单元格。这是因为我们的默认移动成本是每一步 5，而启发式算法每一步只增加 1。所以启发式的影响不是很强。

如果地图上的移动成本相同，那么我们在确定启发式算法时可以使用相同的成本。在我们的例子中，这将是我们目前的启发式时间乘以 5。这将大大减少加工单元格的数量。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-16/smarter-searching/heuristic-5.png)

*使用启发式 × 5。*

然而，如果地图上有道路，我们最终可能会高估剩余的距离。因此，该算法可能会出错，并产生一条实际上不是最短的路径。

![incorrect](https://catlikecoding.com/unity/tutorials/hex-map/part-16/smarter-searching/road-incorrect.png) ![correct](https://catlikecoding.com/unity/tutorials/hex-map/part-16/smarter-searching/road-correct.png)

*高估与可接受的启发式。*

为了保证我们找到最短的路径，我们必须确保我们永远不会高估剩余的距离。这被称为可接受的启发式。因为我们的最小移动成本是 1，所以在确定启发式算法时，我们别无选择，只能使用相同的成本。

从技术上讲，使用更低的成本是可以的，但这只会使启发式更弱。最低可能的启发式是零，这就是 Dijkstra 的算法。当启发式为非零时，该算法被称为 A*，发音为 A-star。

> **为什么它被称为 A*？**
>
> 在 Dijkstra 算法中添加启发式算法的想法最早是由 Nils Nilsson 提出的。他将其变体命名为 A1。后来，伯特伦·拉斐尔（Bertram Raphael）制作了一个更好的版本，他将其命名为 A2。在那之后，彼得·哈特（Peter Hart）用一个很好的启发式方法证明了 A2 是最优的，所以没有更好的版本了。这促使他将其命名为 `A*`，以表明永远不会有像 A3 或 A4 这样的改进。所以，是的，`A*` 算法是你能得到的最好的算法，但它只和它的启发式算法一样好。

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-16/smarter-searching/smarter-searching.unitypackage)

## 4 优先级队列

虽然 A* 是一个很好的算法，但我们的实现效率并不高。这是因为我们使用列表来存储边界，我们必须对每次迭代进行排序。正如[前面的教程](https://catlikecoding.com/unity/tutorials/hex-map/part-15/)中提到的，我们需要的是一个优先级队列，但没有标准的实现。现在让我们自己创建一个。

我们的队列必须支持基于优先级的入队和出队操作。它还必须支持更改已在队列中的单元格的优先级。理想情况下，我们在实现这一点的同时尽量减少搜索、排序和内存分配。我们也希望保持简单。

### 4.1 创建自定义队列

使用所需的公共方法创建一个新的 `HexCellPriorityQueue` 类。我们将使用一个简单的列表来跟踪队列的内容。此外，给它一个 `Clear` 方法来重置队列，这样我们就可以重用它。

```c#
using System.Collections.Generic;

public class HexCellPriorityQueue {

	List<HexCell> list = new List<HexCell>();

	public void Enqueue (HexCell cell) {
	}

	public HexCell Dequeue () {
		return null;
	}
	
	public void Change (HexCell cell) {
	}
	
	public void Clear () {
		list.Clear();
	}
}
```

我们将单元格优先级存储在单元格本身中。因此，在将单元格添加到队列之前，必须设置其优先级。但在优先级发生变化的情况下，了解旧的优先级可能是有用的。因此，让我们将其作为参数添加到 `Change` 中。

```c#
	public void Change (HexCell cell, int oldPriority) {
	}
```

了解队列中有多少单元格也很有用，因此为其添加 `Count` 属性。只需使用一个适当递增和递减的字段。

```c#
	int count = 0;

	public int Count {
		get {
			return count;
		}
	}

	public void Enqueue (HexCell cell) {
		count += 1;
	}

	public HexCell Dequeue () {
		count -= 1;
		return null;
	}
	
	…
	
	public void Clear () {
		list.Clear();
		count = 0;
	}
```

### 4.2 添加到队列

当一个单元格被添加到队列中时，让我们从简单地使用其优先级作为索引开始，将列表视为一个简单的数组。

```c#
	public void Enqueue (HexCell cell) {
		count += 1;
		int priority = cell.SearchPriority;
		list[priority] = cell;
	}
```

然而，只有当列表足够长时，这才有效，否则我们就会越界。我们可以通过在列表中添加虚拟元素来防止这种情况，直到它达到所需的长度。这些空元素不引用单元格，因此它们是通过向列表中添加 `null` 创建的。

```c#
		int priority = cell.SearchPriority;
		while (priority >= list.Count) {
			list.Add(null);
		}
		list[priority] = cell;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-16/priority-queue/list-with-holes.png)

*有洞的列表。*

但这只会为每个优先级存储一个单元格，而可能会有多个单元格。为了跟踪具有相同优先级的所有单元格，我们必须使用另一个列表。虽然我们可以按优先级使用实际列表，但我们也可以向 `HexCell` 添加一个属性将它们链接在一起。这使我们能够创建一个称为链表的单元格链。

```c#
	public HexCell NextWithSamePriority { get; set; }
```

要创建链，请使用 `HexCellPriorityQueue.Enqueue` 使新添加的单元格在替换之前以相同的优先级引用当前值。

```c#
		cell.NextWithSamePriority = list[priority];
		list[priority] = cell;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-16/priority-queue/chained-list.png)

*链接列表列表。*

### 4.3 从队列中删除

要从优先级队列中检索单元格，我们必须访问最低非空索引处的链表。因此，循环遍历列表，直到找到为止。如果没有找到，则队列为空，我们返回 `null`。

我们可以从找到的链中返回任何单元格，因为它们都有相同的优先级。最简单的方法是在链的开头返回单元格。

```c#
	public HexCell Dequeue () {
		count -= 1;
		for (int i = 0; i < list.Count; i++) {
			HexCell cell = list[i];
			if (cell != null) {
				return cell;
			}
		}
		return null;
	}
```

要保留对链其余部分的引用，请使用与新开始具有相同优先级的下一个单元格。如果只有一个单元格处于此优先级，则该元素将变为 `null`，并且将来会被跳过。

```c#
			if (cell != null) {
				list[i] = cell.NextWithSamePriority;
				return cell;
			}
```

### 4.4 跟踪最小值

这种方法有效，但每次检索单元格时都需要迭代列表。我们无法避免搜索最低的非空索引，但我们不必每次都从零开始。相反，我们可以跟踪最低优先级，并从那里开始搜索。最初，最小值实际上是无限的。

```c#
	int minimum = int.MaxValue;

	…
	
	public void Clear () {
		list.Clear();
		count = 0;
		minimum = int.MaxValue;
	}
```

将单元格添加到队列时，必要时调整最小值。

```c#
	public void Enqueue (HexCell cell) {
		count += 1;
		int priority = cell.SearchPriority;
		if (priority < minimum) {
			minimum = priority;
		}
		…
	}
```

当去量化时，使用最小值迭代列表，而不是从零开始。

```c#
	public HexCell Dequeue () {
		count -= 1;
		for (; minimum < list.Count; minimum++) {
			HexCell cell = list[minimum];
			if (cell != null) {
				list[minimum] = cell.NextWithSamePriority;
				return cell;
			}
		}
		return null;
	}
```

这大大减少了我们在优先级列表中循环的时间。

### 4.5 更改优先级

当单元格的优先级发生变化时，必须将其从当前所属的链表中删除。要做到这一点，我们必须沿着链条走，直到找到它。

首先，将旧优先级列表的头部声明为当前单元格，并跟踪下一个单元格。我们可以直接抓取下一个单元格，因为我们知道这个索引处至少有一个单元格。

```c#
	public void Change (HexCell cell, int oldPriority) {
		HexCell current = list[oldPriority];
		HexCell next = current.NextWithSamePriority;
	}
```

如果当前单元格是已更改的单元格，那么它就是头部单元格，我们可以将其剪切掉，就像我们将其从队列中删除一样。

```c#
		HexCell current = list[oldPriority];
		HexCell next = current.NextWithSamePriority;
		if (current == cell) {
			list[oldPriority] = next;
		}
```

如果没有，我们必须沿着这条链走，直到我们到达改变后的单元格前面的单元格。该单元格包含对已更改单元格的引用。

```c#
		if (current == cell) {
			list[oldPriority] = next;
		}
		else {
			while (next != cell) {
				current = next;
				next = current.NextWithSamePriority;
			}
		}
```

此时，我们可以跳过已更改的单元格，将其从链表中删除。

```c#
			while (next != cell) {
				current = next;
				next = current.NextWithSamePriority;
			}
			current.NextWithSamePriority = cell.NextWithSamePriority;
```

删除单元格后，必须再次添加，以便它最终出现在新优先级的列表中。

```c#
	public void Change (HexCell cell, int oldPriority) {
		…
		Enqueue(cell);
	}
```

`Enqueue` 方法递增计数，但我们实际上并没有添加新的单元格。所以我们必须减少计数来弥补这一点。

```c#
		Enqueue(cell);
		count -= 1;
```

### 4.6 使用队列

现在我们可以在 `HexGrid` 中使用我们的自定义优先级队列。我们可以使用一个单独的实例来进行所有搜索。

```c#
	HexCellPriorityQueue searchFrontier;
	
	…
	
	IEnumerator Search (HexCell fromCell, HexCell toCell) {
		if (searchFrontier == null) {
			searchFrontier = new HexCellPriorityQueue();
		}
		else {
			searchFrontier.Clear();
		}
		
		…
	}
```

`Search` 方法现在必须在开始循环之前 `fromCell` 中排队，每次迭代都从一个单元格中退出开始。这取代了旧的边境代码。

```c#
		WaitForSeconds delay = new WaitForSeconds(1 / 60f);
//		List<HexCell> frontier = new List<HexCell>();
		fromCell.Distance = 0;
//		frontier.Add(fromCell);
		searchFrontier.Enqueue(fromCell);
		while (searchFrontier.Count > 0) {
			yield return delay;
			HexCell current = searchFrontier.Dequeue();
//			frontier.RemoveAt(0);
			…
		}
```

调整添加和更改邻居的代码。在更改之前，一定要记住旧的优先级。

```c#
				if (neighbor.Distance == int.MaxValue) {
					neighbor.Distance = distance;
					neighbor.PathFrom = current;
					neighbor.SearchHeuristic =
						neighbor.coordinates.DistanceTo(toCell.coordinates);
//					frontier.Add(neighbor);
					searchFrontier.Enqueue(neighbor);
				}
				else if (distance < neighbor.Distance) {
					int oldPriority = neighbor.SearchPriority;
					neighbor.Distance = distance;
					neighbor.PathFrom = current;
					searchFrontier.Change(neighbor, oldPriority);
				}
```

最后，我们不再需要对边界进行排序。

```c#
//				frontier.Sort(
//					(x, y) => x.SearchPriority.CompareTo(y.SearchPriority)
//				);
```

*使用优先级队列进行搜索。*

如前所述，找到哪条最短路径取决于处理单元格的顺序。我们的队列产生的顺序与排序列表不同，因此您可以获得不同的路径。因为我们都在为每个优先级添加和删除链表的头部，所以它们起着堆栈而不是队列的作用。最后添加的单元格首先被处理。这种方法的一个副作用是算法倾向于曲折。这使得它更有可能产生曲折的路径。幸运的是，这样的路径往往看起来更好，所以这是一个很好的副作用。

![sorted list](https://catlikecoding.com/unity/tutorials/hex-map/part-16/smarter-searching/with-heuristic.png) ![priority queue](https://catlikecoding.com/unity/tutorials/hex-map/part-16/priority-queue/with-priority-queue.png)

*排序列表 vs. 优先级队列。*

下一个教程是[受限移动](https://catlikecoding.com/unity/tutorials/hex-map/part-17/)。

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-16/priority-queue/priority-queue.unitypackage)

[PDF](https://catlikecoding.com/unity/tutorials/hex-map/part-16/Hex-Map-16.pdf)

# Hex Map 17：受限移动

更新于 2019-03-12 发布于 2017-05-24

https://catlikecoding.com/unity/tutorials/hex-map/part-17/

*查找基于转弯的移动路径。*
*立即显示路径。*
*更高效地搜索。*
*只可视化路径。*

这是关于[六边形地图](https://catlikecoding.com/unity/tutorials/hex-map/)的系列教程的第 17 部分。这一次，我们将把行动分成几个回合，并尽快进行搜索。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-17/tutorial-image.jpg)

*几回合的旅程。*

## 1 回合制运动

使用六边形网格的策略游戏几乎总是回合制的。在地图上导航的单位速度有限，这限制了它们在一次转弯中可以移动的距离。

### 1.1 速度

为了支持有限的移动，让我们在 `HexGrid.FindPath` 和 `HexGrid.Search` 中添加一个 `speed` 整数参数。它定义了一回合的移动预算。

```c#
	public void FindPath (HexCell fromCell, HexCell toCell, int speed) {
		StopAllCoroutines();
		StartCoroutine(Search(fromCell, toCell, speed));
	}

	IEnumerator Search (HexCell fromCell, HexCell toCell, int speed) {
		…
	}
```

在游戏中，不同的单位类型通常会有不同的速度。骑兵速度快，步兵速度慢，等等。我们还没有单位，所以现在我们将使用固定速度。让我们使用 24。这是一个相当大的值，不能被 5 整除，这是我们的默认移动成本。在 `HexMapEditor.HandleInput` 中添加恒定速度作为 `FindPath` 的参数。

```c#
			if (editMode) {
				EditCells(currentCell);
			}
			else if (
				Input.GetKey(KeyCode.LeftShift) && searchToCell != currentCell
			) {
				if (searchFromCell) {
					searchFromCell.DisableHighlight();
				}
				searchFromCell = currentCell;
				searchFromCell.EnableHighlight(Color.blue);
				if (searchToCell) {
					hexGrid.FindPath(searchFromCell, searchToCell, 24);
				}
			}
			else if (searchFromCell && searchFromCell != currentCell) {
				searchToCell = currentCell;
				hexGrid.FindPath(searchFromCell, searchToCell, 24);
			}
```

### 1.2 回合

除了跟踪一条路径的总移动成本外，我们现在还必须知道沿着它需要多少回合。但我们不必为每个单元格存储这些信息。我们可以通过将行驶距离除以速度来推导它。由于这些是整数，我们使用整数除法。因此，最多 24 的总距离对应于回合 0。这意味着整个路径可以在当前回合行驶。如果目的地在距离 30 处，那么它的回合将是1。该部队在到达目的地之前，必须使用当前回合的所有移动和下一回合的部分移动。

让我们在 `HexGrid.Search` 中找出当前单元格及其邻居的回合。当前单元格的轮次可以计算一次，就在循环遍历邻居之前。一旦我们找到邻居的距离，就可以确定邻居的回合。

```c#
			int currentTurn = current.Distance / speed;

			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
				…
				int distance = current.Distance;
				if (current.HasRoadThroughEdge(d)) {
					distance += 1;
				}
				else if (current.Walled != neighbor.Walled) {
					continue;
				}
				else {
					distance += edgeType == HexEdgeType.Flat ? 5 : 10;
					distance += neighbor.UrbanLevel + neighbor.FarmLevel +
						neighbor.PlantLevel;
				}

				int turn = distance / speed;

				…
			}
```

### 1.3 迷失的动作

如果邻居的回合大于当前回合，那么我们已经通过了回合边界。如果进入邻居所需的移动为 1，则一切正常。但是，当进入邻居的成本更高时，情况就会变得更加复杂。

假设我们在一张没有特征的地图上移动，所以所有单元格都需要 5 次移动才能进入。我们的速度是 24。经过四个步骤，我们已经使用了 20 个移动预算，剩下 4 个。下一步需要 5 个动作，这比我们多了一个。现在我们该怎么办？

有两种方法可以处理这种情况。第一种是允许在当前回合中进入第五个单元格，即使我们没有足够的移动。第二种是在当前回合期间禁止移动，这意味着剩余的移动点无法使用并丢失。

哪种选择最好取决于游戏。一般来说，第一种方法适用于每回合只能移动几步的单位游戏，如《文明》游戏。这确保了每个单元每回合至少可以移动一个单元。如果单位每回合可以移动许多单元格，就像《奇迹时代》（Age of Wonders）或《韦诺之战》（Battle for Wesnoth）中那样，那么第二种方法效果更好。

当我们使用速度 24 时，让我们选择第二种选择。为了实现这一点，我们必须先隔离进入相邻小区的成本，然后再将其添加到当前距离中。

```c#
//				int distance = current.Distance;
				int moveCost;
				if (current.HasRoadThroughEdge(d)) {
					moveCost = 1;
				}
				else if (current.Walled != neighbor.Walled) {
					continue;
				}
				else {
					moveCost = edgeType == HexEdgeType.Flat ? 5 : 10;
					moveCost  += neighbor.UrbanLevel + neighbor.FarmLevel +
						neighbor.PlantLevel;
				}

				int distance = current.Distance + moveCost;
				int turn = distance / speed;
```

如果我们最终越过回合边界，那么我们首先会用完当前回合的所有移动。我们可以通过简单地将回合乘以速度来实现这一点。之后，我们加上移动成本。

```c#
				int distance = current.Distance + moveCost;
				int turn = distance / speed;
				if (turn > currentTurn) {
					distance = turn * speed + moveCost;
				}
```

这样做的结果是，我们在第四个单元格中结束了第一个回合，留下了 4 个未使用的移动点。这些浪费的点数被计入第五个单元的成本，因此它的距离将变为 29，而不是 25。因此，距离最终将比以前更长。例如，第十个单元格过去的距离为 50。但现在在我们到达那里之前，已经越过了两个回合边界，浪费了 8 个移动点，所以它的距离现在是 58。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-17/turn-based-movement/lost-movement.png)

*花费的时间比预期的要长。*

由于未使用的移动点被添加到单元格距离中，因此在确定最短路径时会将其考虑在内。最有效的路径浪费尽可能少的点。因此，不同的速度会导致不同的路径。

### 1.4 显示回合而不是距离

在玩游戏时，我们并不真正关心用于找到最短路径的距离值。我们感兴趣的是到达目的地需要多少回合。所以，让我们显示回合而不是距离。

首先，去掉 `HexCell` 中的 `UpdateDistanceLabel` 及其调用。

```c#
	public int Distance {
		get {
			return distance;
		}
		set {
			distance = value;
//			UpdateDistanceLabel();
		}
	}
	
	…
	
//	void UpdateDistanceLabel () {
//		UnityEngine.UI.Text label = uiRect.GetComponent<Text>();
//		label.text = distance == int.MaxValue ? "" : distance.ToString();
//	}
```

取而代之的是，向 `HexCell` 添加一个接受任意字符串的公共 `SetLabel` 方法。

```c#
	public void SetLabel (string text) {
		UnityEngine.UI.Text label = uiRect.GetComponent<Text>();
		label.text = text;
	}
```

在 `HexGrid.Search` 中清除单元格时使用此新方法。要隐藏标签，只需将其设置为 `null`。

```c#
		for (int i = 0; i < cells.Length; i++) {
			cells[i].Distance = int.MaxValue;
			cells[i].SetLabel(null);
			cells[i].DisableHighlight();
		}
```

然后将邻居的标签设置为它的回合。之后，我们将能够看到穿越整个路径需要多少额外的回合。

```c#
				if (neighbor.Distance == int.MaxValue) {
					neighbor.Distance = distance;
					neighbor.SetLabel(turn.ToString());
					neighbor.PathFrom = current;
					neighbor.SearchHeuristic =
						neighbor.coordinates.DistanceTo(toCell.coordinates);
					searchFrontier.Enqueue(neighbor);
				}
				else if (distance < neighbor.Distance) {
					int oldPriority = neighbor.SearchPriority;
					neighbor.Distance = distance;
					neighbor.SetLabel(turn.ToString());
					neighbor.PathFrom = current;
					searchFrontier.Change(neighbor, oldPriority);
				}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-17/turn-based-movement/turns.png)

*沿着路径移动需要的回合数。*

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-17/turn-based-movement/turn-based-movement.unitypackage)

## 2 即时路径

在玩游戏时，我们也不在乎寻路算法是如何找到方向的。我们希望立即看到我们请求的路径。现在我们可以确信我们的算法是有效的，所以让我们摆脱搜索可视化。

### 2.1 不再有协程

我们使用协程来缓慢地遍历我们的算法。我们不再需要这样做，所以去掉 `HexGrid` 中 `StartCoroutine` 和 `StopAllCoroutines` 的调用。相反，我们只是像常规方法一样调用 `Search`。

```c#
	public void Load (BinaryReader reader, int header) {
//		StopAllCoroutines();
		…
	}

	public void FindPath (HexCell fromCell, HexCell toCell, int speed) {
//		StopAllCoroutines();
//		StartCoroutine(Search(fromCell, toCell, speed));
		Search(fromCell, toCell, speed);
	}
```

由于我们不再将 `Search` 用作协程，因此它不再需要屈服，因此请删除该语句。这意味着我们还可以删除 `WaitForSeconds` 声明，并将方法的返回类型更改为 `void`。

```c#
	void Search (HexCell fromCell, HexCell toCell, int speed) {
		…

//		WaitForSeconds delay = new WaitForSeconds(1 / 60f);
		fromCell.Distance = 0;
		searchFrontier.Enqueue(fromCell);
		while (searchFrontier.Count > 0) {
//			yield return delay;
			HexCell current = searchFrontier.Dequeue();
			…
		}
	}
```

*立竿见影。*

### 2.2 时间搜索

我们现在可以立即找到路径，但我们能多快找到它们呢？短路径似乎是瞬时的，但大地图上的长路径可能会让人感觉迟钝。

让我们测量查找和显示路径所需的时间。我们可以使用分析器（profiler）来为搜索计时，但这有点矫枉过正，还会产生额外的开销。让我们改用 `Stopwatch` ，它位于 `System.Diagnostics` 命名空间中。由于我们只使用了很短的时间，我不会费心在脚本顶部添加一个 `using` 语句。

在执行搜索之前，创建一个新的秒表并启动它。搜索完成后，停止秒表并记录已过去的毫秒数。

```c#
	public void FindPath (HexCell fromCell, HexCell toCell, int speed) {
		System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
		sw.Start();
		Search(fromCell, toCell, speed);
		sw.Stop();
		Debug.Log(sw.ElapsedMilliseconds);
	}
```

让我们使用我们算法的最坏情况，即从大地图的左下角到右上角进行搜索。无特征地图是最糟糕的，因为这样算法将不得不处理地图的所有 4800 个单元格。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-17/instant-paths/worst-case.png)

*最坏情况搜索。*

所需的时间会有所不同，因为 Unity 编辑器不是您机器上运行的唯一进程。因此，尝试几次，以获得平均持续时间的指示。就我而言，大约需要 45 毫秒。这不是很快，相当于每秒 22.22 条路径，比如 22 pps。这意味着在计算此路径的帧上，游戏的帧速率也将降至最多 22 fps。这忽略了所有其他必须完成的工作，比如实际渲染帧。所以我们得到了一个非常明显的帧率下降，低于 20 fps。

在执行这样的性能测试时，请记住，Unity 编辑器中的性能不会像独立应用程序的性能那样好。如果我对构建执行相同的测试，平均只需要 15 毫秒。这是 66 个 pps，这要好得多。然而，这仍然是帧预算的一大部分，这将使帧速率降至 60 fps 以下。

> **我在哪里可以看到构建的调试日志？**
>
> Unity 应用程序会写入存储在系统某处的日志文件。它的位置取决于平台。有关如何在系统上查找日志文件，请参阅 Unity 的[日志文件](https://docs.unity3d.com/Manual/LogFiles.html)文档。

### 2.3 仅在需要时搜索

我们可以做的一个快速优化是确保我们只在需要时进行搜索。目前，我们按住鼠标按钮的每一帧都会启动一个新的搜索。因此，拖动时帧率将持续降低。我们可以通过在 `HexMapEditor.HandleInput` 中启动新的搜索来防止这种情况，当我们实际处理一个新的端点时。如果不是，当前可见的路径仍然有效。

```c#
			if (editMode) {
				EditCells(currentCell);
			}
			else if (
				Input.GetKey(KeyCode.LeftShift)  && searchToCell != currentCell
			) {
				if (searchFromCell != currentCell) {
					if (searchFromCell) {
						searchFromCell.DisableHighlight();
					}
					searchFromCell = currentCell;
					searchFromCell.EnableHighlight(Color.blue);
					if (searchToCell) {
						hexGrid.FindPath(searchFromCell, searchToCell, 24);
					}
				}
			}
			else if (searchFromCell && searchFromCell != currentCell) {
				if (searchToCell != currentCell) {
					searchToCell = currentCell;
					hexGrid.FindPath(searchFromCell, searchToCell, 24);
				}
			}
```

### 2.4 仅在路径上显示标签

显示回合标签的成本相当高，特别是因为我们没有使用优化的方法。对所有单元格都这样做肯定会减缓我们的速度。因此，让我们省略在 `HexGrid.Search` 中设置标签。

```c#
				if (neighbor.Distance == int.MaxValue) {
					neighbor.Distance = distance;
//					neighbor.SetLabel(turn.ToString());
					neighbor.PathFrom = current;
					neighbor.SearchHeuristic =
						neighbor.coordinates.DistanceTo(toCell.coordinates);
					searchFrontier.Enqueue(neighbor);
				}
				else if (distance < neighbor.Distance) {
					int oldPriority = neighbor.SearchPriority;
					neighbor.Distance = distance;
//					neighbor.SetLabel(turn.ToString());
					neighbor.PathFrom = current;
					searchFrontier.Change(neighbor, oldPriority);
				}
```

我们真的只需要看到找到的路径的信息。因此，当到达目的地时，计算回合并仅设置路径上单元格的标签。

```c#
			if (current == toCell) {
				current = current.PathFrom;
				while (current != fromCell) {
					int turn = current.Distance / speed;
					current.SetLabel(turn.ToString());
					current.EnableHighlight(Color.white);
					current = current.PathFrom;
				}
				break;
			}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-17/instant-paths/only-path-labels.png)

*仅显示路径上的单元格标签。*

现在我们只看到开始和目标单元格之间的单元格上的回合标签。但目标单元格是最重要的，所以我们也必须设置它的标签。我们可以通过在目标单元格而不是之前的单元格开始路径循环来实现这一点。这将把目标单元格的突出显示设置为白色而不是红色，因此将目标单元格的高亮显示移动到循环的正下方。

```c#
		fromCell.EnableHighlight(Color.blue);
//		toCell.EnableHighlight(Color.red);

		fromCell.Distance = 0;
		searchFrontier.Enqueue(fromCell);
		while (searchFrontier.Count > 0) {
			HexCell current = searchFrontier.Dequeue();

			if (current == toCell) {
//				current = current.PathFrom;
				while (current != fromCell) {
					int turn = current.Distance / speed;
					current.SetLabel(turn.ToString());
					current.EnableHighlight(Color.white);
					current = current.PathFrom;
				}
				toCell.EnableHighlight(Color.red);
				break;
			}
			
			…
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-17/instant-paths/destination-label.png)

*回合信息对目的地最为重要。*

经过这些更改，我最坏的情况现在在编辑器中改进为 23 毫秒，在独立构建中改进为 6 毫秒。这分别是 43 pps 和 166 pps，这是一个显著的改进。

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-17/instant-paths/instant-paths.unitypackage)

## 3 最智能的搜索

在[前面的教程](https://catlikecoding.com/unity/tutorials/hex-map/part-16/)中，我们通过实现 `A*` 算法使我们的搜索例程更加智能。然而，我们实际上还没有以最佳方式进行搜索。每次迭代，我们计算当前单元格到其所有邻居的距离。对于尚未或当前不属于搜索边界的单元格，这是正确的。但已经被带出边境的单元格不再需要考虑。这是因为我们已经找到了通往这些单元格的最短路径。跳过这些单元格正是正确的`A*` 实现所做的，所以我们也应该这样做。

### 3.1 单元格搜索阶段

我们如何知道一个单元格是否已经离开了边界？目前，我们无法确定这一点。因此，我们必须跟踪一个单元格处于搜索的哪个阶段。它要么还不在边境，要么是边境的一部分，要么是在边境后面。我们可以通过向 `HexCell` 添加一个简单的整数属性来跟踪这一点。

```c#
	public int SearchPhase { get; set; }
```

例如，0 表示尚未到达该单元格，1 表示该单元格当前位于边界，而 2 表示它已被移出边界。

### 3.2 进入边境

在 `HexGrid.Search` 中，我们可以将所有单元格重置为 0，并始终使用 1 作为边界。或者，我们可以为每次新搜索增加边界编号。这样，我们就不必费心重置单元格，只要我们每次将边界增加两个。

```c#
	int searchFrontierPhase;
						
	…

	void Search (HexCell fromCell, HexCell toCell, int speed) {
		searchFrontierPhase += 2;
		…
	}
```

现在，当我们将单元格添加到边界时，我们必须设置单元格的搜索阶段。这从起始单元格开始，当它被添加到边界时。

```c#
		fromCell.SearchPhase = searchFrontierPhase;
		fromCell.Distance = 0;
		searchFrontier.Enqueue(fromCell);
```

每当我们在边境增加一个邻居时。

```c#
				if (neighbor.Distance == int.MaxValue) {
					neighbor.SearchPhase = searchFrontierPhase;
					neighbor.Distance = distance;
					neighbor.PathFrom = current;
					neighbor.SearchHeuristic =
						neighbor.coordinates.DistanceTo(toCell.coordinates);
					searchFrontier.Enqueue(neighbor);
				}
```

### 3.3 检查边境

到目前为止，我们使用等于 `int.MaxValue` 的距离来检查单元格是否尚未添加到边界。我们现在可以将单元格的搜索阶段与当前前沿进行比较。

```c#
//				if (neighbor.Distance == int.MaxValue) {
				if (neighbor.SearchPhase < searchFrontierPhase) {
					neighbor.SearchPhase = searchFrontierPhase;
					neighbor.Distance = distance;
					neighbor.PathFrom = current;
					neighbor.SearchHeuristic =
						neighbor.coordinates.DistanceTo(toCell.coordinates);
					searchFrontier.Enqueue(neighbor);
				}
```

这意味着我们不再需要在搜索之前重置单元格距离。这意味着我们可以做更少的工作，这很好。

```c#
		for (int i = 0; i < cells.Length; i++) {
//			cells[i].Distance = int.MaxValue;
			cells[i].SetLabel(null);
			cells[i].DisableHighlight();
		}
```

### 3.4 走出边境

当一个单元格被移出边界时，我们通过递增其搜索阶段来表示这一点。这使它落后于当前的前沿，领先于下一个前沿。

```c#
		while (searchFrontier.Count > 0) {
			HexCell current = searchFrontier.Dequeue();
			current.SearchPhase += 1;
			
			…
		}
```

最后，我们可以跳过已移出边界的单元格，避免无意义的距离计算和比较。

```c#
			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
				HexCell neighbor = current.GetNeighbor(d);
				if (
					neighbor == null ||
					neighbor.SearchPhase > searchFrontierPhase
				) {
					continue;
				}
				…
			}
```

此时，我们的算法仍然会产生相同的结果，但效率更高。对我来说，最坏情况下的搜索现在在编辑器中需要 20 毫秒，在构建中需要 5 毫秒。

您还可以通过在单元格距离为计算机时递增计数器来记录算法处理单元格的次数。早些时候，我们的算法为最坏情况下的搜索计算了 28239 个距离。现在，使用我们完整的 A* 算法，我们只计算了 14120 个距离。减少了 50%。这对性能的影响有多大取决于计算移动成本的成本有多高。在我们的例子中，这不是那么多工作，所以在构建中的改进不是那么好，尽管在编辑器中仍然相当重要。

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-17/smartest-search/smartest-search.unitypackage)

## 4 清理道路

当启动新的搜索时，我们首先必须清理之前路径的可视化。现在，我们通过禁用高亮显示并删除网格中每个单元格的标签来实现这一点。这是一种严厉的方法。理想情况下，我们只重置前一条路径中的细胞。

### 4.1 仅搜索

让我们从完全删除 `Search` 中的可视化代码开始。它所要做的就是寻找一条路径，无论我们如何处理这些信息。

```c#
	void Search (HexCell fromCell, HexCell toCell, int speed) {
		searchFrontierPhase += 2;
		if (searchFrontier == null) {
			searchFrontier = new HexCellPriorityQueue();
		}
		else {
			searchFrontier.Clear();
		}

//		for (int i = 0; i < cells.Length; i++) {
//			cells[i].SetLabel(null);
//			cells[i].DisableHighlight();
//		}
//		fromCell.EnableHighlight(Color.blue);

		fromCell.SearchPhase = searchFrontierPhase;
		fromCell.Distance = 0;
		searchFrontier.Enqueue(fromCell);
		while (searchFrontier.Count > 0) {
			HexCell current = searchFrontier.Dequeue();
			current.SearchPhase += 1;

			if (current == toCell) {
//				while (current != fromCell) {
//					int turn = current.Distance / speed;
//					current.SetLabel(turn.ToString());
//					current.EnableHighlight(Color.white);
//					current = current.PathFrom;
//				}
//				toCell.EnableHighlight(Color.red);
//				break;
			}

			…
		}
	}
```

要传达 `Search` 是否找到了路径，请让它返回一个布尔值。

```c#
	bool Search (HexCell fromCell, HexCell toCell, int speed) {
		searchFrontierPhase += 2;
		if (searchFrontier == null) {
			searchFrontier = new HexCellPriorityQueue();
		}
		else {
			searchFrontier.Clear();
		}

		fromCell.SearchPhase = searchFrontierPhase;
		fromCell.Distance = 0;
		searchFrontier.Enqueue(fromCell);
		while (searchFrontier.Count > 0) {
			HexCell current = searchFrontier.Dequeue();
			current.SearchPhase += 1;

			if (current == toCell) {
				return true;
			}

			…
		}
		return false;
	}
```

### 4.2 记住这条路

当找到一条路时，我们必须记住它。这样，我们下次就可以清理它了。因此，要跟踪端点以及它们之间是否存在路径。

```c#
	HexCell currentPathFrom, currentPathTo;
	bool currentPathExists;
	
	…
	
	public void FindPath (HexCell fromCell, HexCell toCell, int speed) {
		System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
		sw.Start();
		currentPathFrom = fromCell;
		currentPathTo = toCell;
		currentPathExists = Search(fromCell, toCell, speed);
		sw.Stop();
		Debug.Log(sw.ElapsedMilliseconds);
	}
```

### 4.3 再次显示路径

我们可以使用我们记住的搜索数据再次可视化路径。为此创建一个新的 `ShowPath` 方法。它将从路径的末端循环到起点，突出显示单元格并将其标签设置为轮到单元格。我们需要知道这样做的速度，所以把它作为一个参数。如果我们没有路径，该方法将只突出显示端点。

```c#
	void ShowPath (int speed) {
		if (currentPathExists) {
			HexCell current = currentPathTo;
			while (current != currentPathFrom) {
				int turn = current.Distance / speed;
				current.SetLabel(turn.ToString());
				current.EnableHighlight(Color.white);
				current = current.PathFrom;
			}
		}
		currentPathFrom.EnableHighlight(Color.blue);
		currentPathTo.EnableHighlight(Color.red);
	}
```

搜索后，在 `FindPath` 中调用此方法。

```c#
		currentPathExists = Search(fromCell, toCell, speed);
		ShowPath(speed);
```

### 4.4 清理

我们再次看到路径，但它们不再被抹去。要清理它们，请创建 `ClearPath` 方法。它基本上是 `ShowPath` 的副本，只是它禁用了高亮显示和标签，而不是显示它们。这样做之后，它还应该清除我们记住的路径数据，因为它不再有效。

```c#
	void ClearPath () {
		if (currentPathExists) {
			HexCell current = currentPathTo;
			while (current != currentPathFrom) {
				current.SetLabel(null);
				current.DisableHighlight();
				current = current.PathFrom;
			}
			current.DisableHighlight();
			currentPathExists = false;
		}
		currentPathFrom = currentPathTo = null;
	}
```

除此之外，我们还必须确保在路径无效的情况下清除端点。

```c#
		if (currentPathExists) {
			…
		}
		else if (currentPathFrom) {
			currentPathFrom.DisableHighlight();
			currentPathTo.DisableHighlight();
		}
		currentPathFrom = currentPathTo = null;
```

通过这种方法，我们可以只访问必要的单元格来清除旧的路径可视化。地图的大小不再重要。在启动新搜索之前，在 `FindPath` 中调用它。

```c#
		sw.Start();
		ClearPath();
		currentPathFrom = fromCell;
		currentPathTo = toCell;
		currentPathExists = Search(fromCell, toCell, speed);
		if (currentPathExists) {
			ShowPath(speed);
		}
		sw.Stop();
```

此外，在创建新地图时，请确保清除路径。

```c#
	public bool CreateMap (int x, int z) {
		…

		ClearPath();
		if (chunks != null) {
			for (int i = 0; i < chunks.Length; i++) {
				Destroy(chunks[i].gameObject);
			}
		}

		…
	}
```

在加载另一张地图之前。

```c#
	public void Load (BinaryReader reader, int header) {
		ClearPath();
		…
	}
```

路径可视化再次得到清理，就像我们做出这一更改之前一样。现在我们使用了一种更有效的方法，我在编辑器中的最坏情况搜索时间缩短到了 14 毫秒。对于更聪明地清理来说，这是一个相当大的进步。在构建过程中，我现在只需要 3 毫秒。这是 333 个 pps，这使得我们的寻路绝对可以实时使用。

现在我们有了快速寻路，我们可以删除定时调试代码了。

```c#
	public void FindPath (HexCell fromCell, HexCell toCell, int speed) {
//		System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
//		sw.Start();
		ClearPath();
		currentPathFrom = fromCell;
		currentPathTo = toCell;
		currentPathExists = Search(fromCell, toCell, speed);
		ShowPath(speed);
//		sw.Stop();
//		Debug.Log(sw.ElapsedMilliseconds);
	}
```

下一个教程是“[单位](https://catlikecoding.com/unity/tutorials/hex-map/part-18/)”。

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-17/cleaning-the-path/cleaning-the-path.unitypackage)

[PDF](https://catlikecoding.com/unity/tutorials/hex-map/part-17/Hex-Map-17.pdf)

# Hex Map 18：单位

发布于 2017-06-23

https://catlikecoding.com/unity/tutorials/hex-map/part-18/

*将单位放在地图上。*
*保存并加载单位。*
*为单位找到路径。*
*移动单位。*

这是关于[六边形地图](https://catlikecoding.com/unity/tutorials/hex-map/)的系列教程的第18部分。现在我们已经弄清楚了如何进行寻路，让我们在地图上标出一些单位。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-18/tutorial-image.jpg)

*部队已经到达。*

## 1 创建单位

到目前为止，我们只研究了单元格及其不动的特征。单位是不同的，因为它们是可移动的。一个单位可以代表任何规模的任何东西，从一个人或一辆车到整个军队。在本教程中，我们将仅限于使用一种通用单元类型。一旦我们涵盖了这一点，我们将继续支持多种单元类型。

### 1.1 单位预制体

要使用单位，请创建新的 `HexUnit` 组件类型。现在先从一个空的 `MonoBehaviour` 开始，稍后我们将为其添加功能。

```c#
using UnityEngine;

public class HexUnit : MonoBehaviour {
}
```

使用此组件创建一个空游戏对象，该对象应成为预制件。这是一个单位的根对象。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-18/creating-units/prefab.png)

*单位预制体。*

添加一个三维模型，将该单元表示为子对象。我只是使用了一个缩放的立方体，我给了它一个蓝色的材质。根对象定义了单元的地面标高，因此相应地偏移了子对象。

![hierarchy](https://catlikecoding.com/unity/tutorials/hex-map/part-18/creating-units/hierarchy.png)
![cube](https://catlikecoding.com/unity/tutorials/hex-map/part-18/creating-units/cube.png)

*立方体孩子。*

给该装置一个碰撞体将使以后更容易选择它们。默认立方体的碰撞器工作正常。只要确保碰撞体能装在一个单元格里。

### 1.2 实例化单位

由于我们还没有游戏玩法，生产单位是在编辑模式下完成的。因此，`HexMapEditor` 有责任创建它们。它需要预制件来实现这一点，因此为它添加一个 `HexUnit unitPrefab` 字段并将其连接起来。

```c#
	public HexUnit unitPrefab;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-18/creating-units/editor.png)

*连接预制件。*

创建单位时，我们将把它们放置在光标下方的单元格上。`HandleInput` 具有在编辑地形时查找此单元格的代码。我们现在也需要它用于单位，所以让我们将相关代码移动到它自己的方法中。

```c#
	HexCell GetCellUnderCursor () {
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(inputRay, out hit)) {
			return hexGrid.GetCell(hit.point);
		}
		return null;
	}
```

现在我们可以在 `HandleInput` 中使用这种方法，简化它。

```c#
	void HandleInput () {
//		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
//		RaycastHit hit;
//		if (Physics.Raycast(inputRay, out hit)) {
//			HexCell currentCell = hexGrid.GetCell(hit.point);

		HexCell currentCell = GetCellUnderCursor();
		if (currentCell) {
			…
		}
		else {
			previousCell = null;
		}
	}
```

接下来，添加一个新的 `CreateUnit` 方法，该方法也使用 `GetCellUnderCursor`。如果有一个单元格，实例化一个新单位。

```c#
	void CreateUnit () {
		HexCell cell = GetCellUnderCursor();
		if (cell) {
			Instantiate(unitPrefab);
		}
	}
```

为了保持层次结构的整洁，让我们使用网格作为所有单位游戏对象的父级。

```c#
	void CreateUnit () {
		HexCell cell = GetCellUnderCursor();
		if (cell) {
			HexUnit unit = Instantiate(unitPrefab);
			unit.transform.SetParent(hexGrid.transform, false);
		}
	}
```

在 `HexMapEditor` 中添加创建单位支持的最简单方法是按键。调整 `Update` 方法，使其在按下 U 键时调用 `CreateUnit`。与 `HandleInput` 一样，只有当光标不在 GUI 元素之上时，才会发生这种情况。首先检查是否应该编辑地图，如果不应该，检查是否应该添加一个单位。如果是这样，请调用 `CreateUnit`。

```c#
	void Update () {
//		if (
//			Input.GetMouseButton(0) &&
//			!EventSystem.current.IsPointerOverGameObject()
//		) {
//			HandleInput();
//		}
//		else {
//			previousCell = null;
//		}

		if (!EventSystem.current.IsPointerOverGameObject()) {
			if (Input.GetMouseButton(0)) {
				HandleInput();
				return;
			}
			if (Input.GetKeyDown(KeyCode.U)) {
				CreateUnit();
				return;
			}
		}
		previousCell = null;
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-18/creating-units/instance.png)

*实例化的单位。*

### 1.3 定位单位

现在可以创建单位，但它们最终都位于地图的原点。下一步是把它们放在正确的地方。这要求部队知道自己的位置。因此，在 `HexUnit` 中添加一个 `Location` 属性，以标识它们所占用的单元格。设置时，调整单位的位置，使其与单元格的位置相匹配。

```c#
	public HexCell Location {
		get {
			return location;
		}
		set {
			location = value;
			transform.localPosition = value.Position;
		}
	}

	HexCell location;
```

现在是 `HexMapEditor.CreateUnit` 必须将光标下的单元格分配给该单位的位置。然后，这些单位将最终到达预期的地方。

```c#
	void CreateUnit () {
		HexCell cell = GetCellUnderCursor();
		if (cell) {
			HexUnit unit = Instantiate(unitPrefab);
			unit.transform.SetParent(hexGrid.transform, false);
			unit.Location = cell;
		}
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-18/creating-units/positioned.png)

*定位单位。*

### 1.4 单位朝向

目前，所有单元都有相同的方向，这看起来很僵硬。为了使事情变得生动，请向 `HexUnit` 添加一个 `Orientation` 属性。这是一个浮点数，表示单位围绕 Y 轴的旋转，单位为度。设置时，相应地调整单位的实际游戏对象旋转。

```c#
	public float Orientation {
		get {
			return orientation;
		}
		set {
			orientation = value;
			transform.localRotation = Quaternion.Euler(0f, value, 0f);
		}
	}

	float orientation;
```

在 `HexMapEditor.CreateUnit` 中，指定一个介于 0 和 360 度之间的随机旋转。

```c#
	void CreateUnit () {
		HexCell cell = GetCellUnderCursor();
		if (cell) {
			HexUnit unit = Instantiate(unitPrefab);
			unit.transform.SetParent(hexGrid.transform, false);
			unit.Location = cell;
			unit.Orientation = Random.Range(0f, 360f);
		}
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-18/creating-units/orientation.png)

*不同的单位方向。*

### 1.5 每个单元格一个单位

这些单元看起来不错，除非在同一位置创建了多个单元。然后我们得到重叠的立方体，这看起来很糟糕。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-18/creating-units/overlapping.png)

*重叠单位。*

有些游戏允许多个单位在同一个位置，而另一些则不允许这样做。由于每个单元格使用单个单位更容易，我们将选择此选项。这意味着，只有在当前单元格未被占用的情况下，我们才应该创建一个新单位。为了了解这一点，请在 `HexCell` 中添加一个默认的 `Unit` 属性。

```c#
	public HexUnit Unit { get; set; }
```

在 `HexUnit.Location` 中使用此属性。位置，使单元格意识到有一个单位站在上面。

```c#
	public HexCell Location {
		get {
			return location;
		}
		set {
			location = value;
			value.Unit = this;
			transform.localPosition = value.Position;
		}
	}
```

现在是 `HexMapEditor.CreateUnit` 可以检查当前单元格是否可用。

```c#
	void CreateUnit () {
		HexCell cell = GetCellUnderCursor();
		if (cell && !cell.Unit) {
			HexUnit unit = Instantiate(unitPrefab);
			unit.Location = cell;
			unit.Orientation = Random.Range(0f, 360f);
		}
	}
```

### 1.6 编辑已占用的单元格

虽然单位最初定位正确，但稍后编辑其位置时可能会出错。如果一个单元格的高度发生变化，占据它的单位最终要么会悬停在它上面，要么会沉入其中。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-18/creating-units/misplaced.png)

*浮动和沉没单元。*

解决方案是在更改后验证单位的位置。将此方法添加到 `HexUnit` 中。目前，我们只关心单位的位置，所以只需重新设置即可。

```c#
	public void ValidateLocation () {
		transform.localPosition = location.Position;
	}
```

每当刷新单元格时，我们都应该验证单位的位置，也就是调用 `HexCell` 的 `Refresh` 或 `RefreshSelfOnly` 方法时。当然，只有当单元格中确实有一个单位时，才需要这样做。

```c#
	void Refresh () {
		if (chunk) {
			chunk.Refresh();
			…
			if (Unit) {
				Unit.ValidateLocation();
			}
		}
	}

	void RefreshSelfOnly () {
		chunk.Refresh();
		if (Unit) {
			Unit.ValidateLocation();
		}
	}
```

### 1.7 移除单位

除了创建单位，能够摧毁它们也很有用。因此，在 `HexMapEditor` 中添加一个 `DestroyUnit` 方法。它必须检查光标下的单元格中是否有单位，如果有，则销毁该单位的游戏对象。

```c#
	void DestroyUnit () {
		HexCell cell = GetCellUnderCursor();
		if (cell && cell.Unit) {
			Destroy(cell.Unit.gameObject);
		}
	}
```

请注意，我们要穿过单元格才能到达单位。将鼠标悬停在包含一个单位的单元格上就足以与它进行交互。因此，单元不需要有碰撞体来工作。但是，给它们一个碰撞体可以更容易地指向它们，因为它会阻挡射线，否则这些射线最终会射到单位后面的单元格上。

让我们在 `Update` 中使用左 shift 加 U 键来触发单位的销毁，而不是创建一个单位。

```c#
			if (Input.GetKeyDown(KeyCode.U)) {
				if (Input.GetKey(KeyCode.LeftShift)) {
					DestroyUnit();
				}
				else {
					CreateUnit();
				}
				return;
			}
```

如果我们要创建和销毁许多单位，那么在处理一个单位时，让我们整理和清理属性。这意味着明确清除单元格的单位引用。为 `HexUnit` 添加一个 `Die` 方法来处理这个问题，并销毁它自己的游戏对象。

```c#
	public void Die () {
		location.Unit = null;
		Destroy(gameObject);
	}
```

在 `HexMapEditor.DestroyUnit` 中调用此方法，而不是直接销毁单位。

```c#
	void DestroyUnit () {
		HexCell cell = GetCellUnderCursor();
		if (cell && cell.Unit) {
//			Destroy(cell.Unit.gameObject);
			cell.Unit.Die();
		}
	}
```

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-18/creating-units/creating-units.unitypackage)

## 2 保存和加载单位

现在我们可以在地图上有单位了，我们必须在保存和加载过程中包含它们。我们有两种方法可以解决这个问题。第一种是在写入单元格时写入单位的数据，这样单元格和单位数据就会混合在一起。另一种方法是将单元格和单位数据分开。虽然第一种方法可能看起来更容易实现，但第二种方法为我们提供了更结构化的数据。将数据分开将使未来的工作更容易。

### 2.1 跟踪单位

为了把所有单位都保存在一起，我们必须跟踪他们。我们将通过向 `HexGrid` 添加一个单位列表来实现这一点。这个列表应该包含地图上的所有单位。

```c#
	List<HexUnit> units = new List<HexUnit>();
```

每当创建或加载新地图时，我们都必须删除地图上当前的所有单位。为了方便起见，创建一个 `ClearUnits` 方法，杀死列表中的所有内容并清空它。

```c#
	void ClearUnits () {
		for (int i = 0; i < units.Count; i++) {
			units[i].Die();
		}
		units.Clear();
	}
```

在 `CreateMap` 和 `Load` 中调用此方法。让我们在道路畅通后再这样做。

```c#
	public bool CreateMap (int x, int z) {
		…

		ClearPath();
		ClearUnits();
		…
	}
	
	…
	
	public void Load (BinaryReader reader, int header) {
		ClearPath();
		ClearUnits();
		…
	}
```

### 2.2 向网格添加单位

我们现在必须在创建新单位时将其添加到列表中。让我们为此定义一个 `AddUnit` 方法，该方法还负责定位单位并设置其父级。

```c#
	public void AddUnit (HexUnit unit, HexCell location, float orientation) {
		units.Add(unit);
		unit.transform.SetParent(transform, false);
		unit.Location = location;
		unit.Orientation = orientation;
	}
```

`HexMapEditor.CreatUnit` 现在可以用一个新实例化的单位、它的位置和随机方向调用 `AddUnit`。

```c#
	void CreateUnit () {
		HexCell cell = GetCellUnderCursor();
		if (cell && !cell.Unit) {
//			HexUnit unit = Instantiate(unitPrefab);
//			unit.transform.SetParent(hexGrid.transform, false);
//			unit.Location = cell;
//			unit.Orientation = Random.Range(0f, 360f);
			hexGrid.AddUnit(
				Instantiate(unitPrefab), cell, Random.Range(0f, 360f)
			);
		}
	}
```

### 2.3 从网格中删除单位

在 `HexGrid` 中添加一种删除单位的方法。只需从列表中删除该单位并告诉它死亡。

```c#
	public void RemoveUnit (HexUnit unit) {
		units.Remove(unit);
		unit.Die();
	}
```

在 `HexMapEditor.DestroyUnit` 中调用此方法，而不是直接杀死单位。

```c#
	void DestroyUnit () {
		HexCell cell = GetCellUnderCursor();
		if (cell && cell.Unit) {
//			cell.Unit.Die();
			hexGrid.RemoveUnit(cell.Unit);
		}
	}
```

### 2.4 保存单位

当我们将所有单元存储在一起时，我们必须记住它们占据了哪些单元格。最可靠的方法是存储它们的位置坐标。为了实现这一点，请在 `HexCoordinates` 中添加一个 `Save` 方法，该方法会写入其 X 和 Z 字段。

```c#
using UnityEngine;
using System.IO;

[System.Serializable]
public struct HexCoordinates {

	…
	
	public void Save (BinaryWriter writer) {
		writer.Write(x);
		writer.Write(z);
	}
}
```

`HexUnit` 的 `Save` 方法现在可以写入单位的坐标及其方向。这是我们目前掌握的所有单位数据。

```c#
using UnityEngine;
using System.IO;

public class HexUnit : MonoBehaviour {

	…

	public void Save (BinaryWriter writer) {
		location.coordinates.Save(writer);
		writer.Write(orientation);
	}
}
```

由于 `HexGrid` 跟踪单位，其 `Save` 方法将负责写入单位数据。首先写下有多少个单位，然后循环遍历这些单位。

```c#
	public void Save (BinaryWriter writer) {
		writer.Write(cellCountX);
		writer.Write(cellCountZ);

		for (int i = 0; i < cells.Length; i++) {
			cells[i].Save(writer);
		}

		writer.Write(units.Count);
		for (int i = 0; i < units.Count; i++) {
			units[i].Save(writer);
		}
	}
```

我们已经更改了保存的内容，因此请在 `SaveLoadMenu.Save` 中增加版本号到2。旧的加载代码仍然可以正常工作，因为它根本无法读取单元数据。但是，需要版本增加来传达文件中有单位数据。

```c#
	void Save (string path) {
		using (
			BinaryWriter writer =
			new BinaryWriter(File.Open(path, FileMode.Create))
		) {
			writer.Write(2);
			hexGrid.Save(writer);
		}
	}
```

### 2.5 加载单位

由于 `HexCoordinates` 是一个结构体，因此向其添加常规 `Load` 方法没有多大意义。相反，将其设置为读取并返回存储坐标的静态方法。

```c#
	public static HexCoordinates Load (BinaryReader reader) {
		HexCoordinates c;
		c.x = reader.ReadInt32();
		c.z = reader.ReadInt32();
		return c;
	}
```

因为单位的数量是可变的，所以我们没有预先存在的单位来加载数据。我们可以在加载数据之前创建新的单位实例，但这需要 `HexGrid` 在加载时实例化新的单元。最好把这件事留给 `HexUnit`。因此，我们将使用静态 `HexUnit.Load` 方法也是如此。让我们从读取单位数据开始。要读取方向的浮点数，请使用 `BinaryReader.ReadSingle` 方法。

> **为什么是单（single）？**
>
> float 类型表示四字节长的单精度浮点数。还有双精度数字，定义为double，长度为 8 个字节。这些在 Unity 中很少使用。

```c#
	public static void Load (BinaryReader reader) {
		HexCoordinates coordinates = HexCoordinates.Load(reader);
		float orientation = reader.ReadSingle();
	}
```

下一步是实例化一个新单位。但是，我们需要参考单位预制件。为了现在保持简单，让我们在 `HexUnit` 中为它添加一个静态字段。

```c#
	public static HexUnit unitPrefab;
```

要设置此引用，让我们再次滥用 `HexGrid`，就像我们对噪波纹理所做的那样。当我们支持多种单位类型时，我们将转向更好的方法。

```c#
	public HexUnit unitPrefab;

	…

	void Awake () {
		HexMetrics.noiseSource = noiseSource;
		HexMetrics.InitializeHashGrid(seed);
		HexUnit.unitPrefab = unitPrefab;
		CreateMap(cellCountX, cellCountZ);
	}

	…

	void OnEnable () {
		if (!HexMetrics.noiseSource) {
			HexMetrics.noiseSource = noiseSource;
			HexMetrics.InitializeHashGrid(seed);
			HexUnit.unitPrefab = unitPrefab;
		}
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-18/saving-and-loading-units/inspector.png)

*传递预制单位。*

连接字段后，我们不再需要 `HexMapEditor` 中的直接引用。它可以使用 `HexUnit.unitPrefab` 代替。

```c#
//	public HexUnit unitPrefab;

	…

	void CreateUnit () {
		HexCell cell = GetCellUnderCursor();
		if (cell && !cell.Unit) {
			hexGrid.AddUnit(
				Instantiate(HexUnit.unitPrefab), cell, Random.Range(0f, 360f)
			);
		}
	}
```

现在我们可以在 `HexUnit.Load` 中实例化一个新单位。或者返回它，我们可以使用加载的坐标和方向将其添加到网格中。添加一个 `HexGrid` 参数来实现这一点。

```c#
	public static void Load (BinaryReader reader, HexGrid grid) {
		HexCoordinates coordinates = HexCoordinates.Load(reader);
		float orientation = reader.ReadSingle();
		grid.AddUnit(
			Instantiate(unitPrefab), grid.GetCell(coordinates), orientation
		);
	}
```

在 `HexGrid.Load` 的末尾，读取单位计数并使用它来加载所有存储的单位，将其本身作为额外的参数传递。

```c#
	public void Load (BinaryReader reader, int header) {
		…

		int unitCount = reader.ReadInt32();
		for (int i = 0; i < unitCount; i++) {
			HexUnit.Load(reader, this);
		}
	}
```

当然，这只适用于至少版本 2 的保存文件，否则就没有可加载的单位。

```c#
		if (header >= 2) {
			int unitCount = reader.ReadInt32();
			for (int i = 0; i < unitCount; i++) {
				HexUnit.Load(reader, this);
			}
		}
```

我们现在可以正确加载版本 2 的文件，因此在 `SaveLoadMenu.Load` 中将支持的版本号增加到 2。

```c#
	void Load (string path) {
		if (!File.Exists(path)) {
			Debug.LogError("File does not exist " + path);
			return;
		}
		using (BinaryReader reader = new BinaryReader(File.OpenRead(path))) {
			int header = reader.ReadInt32();
			if (header <= 2) {
				hexGrid.Load(reader, header);
				HexMapCamera.ValidatePosition();
			}
			else {
				Debug.LogWarning("Unknown map format " + header);
			}
		}
	}
```

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-18/saving-and-loading-units/saving-and-loading-units.unitypackage)

## 3 移动单位

单位是可移动的，所以我们应该能够在地图上移动它们。我们已经有了寻路代码，但到目前为止，我们只对任意位置进行了测试。我们必须删除旧的测试 UI，并为控制单位创建一个新的 UI。

### 3.1 清理地图编辑器

沿着路径移动单位是实际游戏的一部分。它不属于地图编辑器。因此，在 `HexMapEditor` 中删除所有与寻路相关的代码。

```c#
//	HexCell previousCell, searchFromCell, searchToCell;
	HexCell previousCell;
	
	…
	
		void HandleInput () {
		HexCell currentCell = GetCellUnderCursor();
		if (currentCell) {
			if (previousCell && previousCell != currentCell) {
				ValidateDrag(currentCell);
			}
			else {
				isDrag = false;
			}
			if (editMode) {
				EditCells(currentCell);
			}
//			else if (
//				Input.GetKey(KeyCode.LeftShift) && searchToCell != currentCell
//			) {
//				if (searchFromCell != currentCell) {
//					if (searchFromCell) {
//						searchFromCell.DisableHighlight();
//					}
//					searchFromCell = currentCell;
//					searchFromCell.EnableHighlight(Color.blue);
//					if (searchToCell) {
//						hexGrid.FindPath(searchFromCell, searchToCell, 24);
//					}
//				}
//			}
//			else if (searchFromCell && searchFromCell != currentCell) {
//				if (searchToCell != currentCell) {
//					searchToCell = currentCell;
//					hexGrid.FindPath(searchFromCell, searchToCell, 24);
//				}
//			}
			previousCell = currentCell;
		}
		else {
			previousCell = null;
		}
	}
```

删除了这段代码后，我们没有理由在不处于编辑模式时保持编辑器处于活动状态。因此，我们可以简单地启用或禁用 `HexMapEditor` 组件，而不是使用字段来跟踪模式。编辑器也不需要再关心 UI 标签了。

```c#
//	bool editMode;
	
	…
	
	public void SetEditMode (bool toggle) {
//		editMode = toggle;
//		hexGrid.ShowUI(!toggle);
		enabled = toggle;
	}
	
	…
	
	void HandleInput () {
		HexCell currentCell = GetCellUnderCursor();
		if (currentCell) {
			if (previousCell && previousCell != currentCell) {
				ValidateDrag(currentCell);
			}
			else {
				isDrag = false;
			}
//			if (editMode) {
			EditCells(currentCell);
//			}
			previousCell = currentCell;
		}
		else {
			previousCell = null;
		}
	}
```

因为默认情况下我们没有处于地图编辑模式，所以在编辑器唤醒时禁用它。

```c#
	void Awake () {
		terrainMaterial.DisableKeyword("GRID_ON");
		SetEditMode(false);
	}
```

对于地图编辑和控制单位，我们都需要使用射线投射来查找光标下的当前单元格。也许以后还有其他事情。让我们将 `HexGrid` 中的射线投射逻辑移动到一个带有射线参数的新 `GetCell` 方法中。

```c#
	public HexCell GetCell (Ray ray) {
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit)) {
			return GetCell(hit.point);
		}
		return null;
	}
```

`HexMapEditor.GetCellUniderCursor` 可以简单地使用光标射线调用此方法。

```c#
	HexCell GetCellUnderCursor () {
		return
			hexGrid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
	}
```

### 3.2 游戏用户界面

我们将使用一个新组件来处理游戏模式 UI。此时，这只涉及选择和移动单位。为它创建一个新的 HexGameUI 组件类型。它只需要引用网格即可完成工作。

```c#
using UnityEngine;
using UnityEngine.EventSystems;

public class HexGameUI : MonoBehaviour {

	public HexGrid grid;
}
```

将此组件添加到 UI 层次结构中的新游戏对象中。它不需要有自己的对象，但这很明显，我们为游戏提供了一个单独的 UI。

![inspector](https://catlikecoding.com/unity/tutorials/hex-map/part-18/moving-units/inspector.png) ![hierarchy](https://catlikecoding.com/unity/tutorials/hex-map/part-18/moving-units/hierarchy.png)

*游戏 UI 对象。*

给 `HexGameUI` 一个 `SetEditMode` 方法，就像 `HexMapEditor` 一样。当我们不处于编辑模式时，应该启用游戏 UI。此外，这是切换标签的地方，因为游戏 UI 将使用路径。

```c#
	public void SetEditMode (bool toggle) {
		enabled = !toggle;
		grid.ShowUI(!toggle);
	}
```

将游戏 UI 的方法添加到编辑模式切换的事件列表中。这意味着当玩家更改模式时，这两种方法都将被调用。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-18/moving-units/event-methods.png)

*多种事件方法。*

### 3.3 跟踪当前单元格

根据发生的情况，HexGameUI 需要知道光标下方当前是哪个单元格。所以给它一个 currentCell 字段。

```c#
	HexCell currentCell;
```

创建一个 `UpdateCurrentCell` 方法，该方法使用 `HexGrid.GetCell` 和光标射线来更新字段。

```c#
	void UpdateCurrentCell () {
		currentCell =
			grid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
	}
```

更新当前单元格时，我们可能想知道它是否已更改。让 `UpdateCurrentCell` 返回是否是这种情况。

```c#
	bool UpdateCurrentCell () {
		HexCell cell =
			grid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
		if (cell != currentCell) {
			currentCell = cell;
			return true;
		}
		return false;
	}
```

### 3.4 选择单位

在移动一个单位之前，我们必须先选择一个，并跟踪它。因此，添加一个 `selectedUnit` 字段。

```c#
	HexUnit selectedUnit;
```

当我们尝试选择时，我们应该从更新当前单元格开始。如果存在当前单元格，则占据该单元格的单位将成为所选单位。如果单元格没有单位，那么我们最终没有选择单位。为此创建一个 `DoSelection` 方法。

```c#
	void DoSelection () {
		UpdateCurrentCell();
		if (currentCell) {
			selectedUnit = currentCell.Unit;
		}
	}
```

我们将支持通过常规鼠标点击来选择单位。因此，添加一个 `Update` 方法，当鼠标按钮 0 被激活时执行选择。当然，只有当光标不在 GUI 元素之上时，我们才会对此感到困扰。

```c#
	void Update () {
		if (!EventSystem.current.IsPointerOverGameObject()) {
			if (Input.GetMouseButtonDown(0)) {
				DoSelection();
			}
		}
	}
```

此时，我们可以通过单击鼠标一次选择一个单位。点击空白单元格将取消选择我们选择的任何单位。但我们还没有收到任何关于这方面的视觉反馈。

### 3.5 单位寻路

当选择一个单元时，我们可以使用它的位置作为寻路的起点。我们不需要再次按下按钮来激活此功能。相反，我们将自动找到并显示单元位置和当前单元格之间的路径。在 `Update` 中始终执行此操作，除非执行选择，如果我们有一个单元，则通过调用 `DoPathfinding` 方法。

```c#
	void Update () {
		if (!EventSystem.current.IsPointerOverGameObject()) {
			if (Input.GetMouseButtonDown(0)) {
				DoSelection();
			}
			else if (selectedUnit) {
				DoPathfinding();
			}
		}
	}
```

`DoPathfinding` 只是更新当前单元格，如果有目标，则调用 `HexGrid.FindPath`。我们将再次使用 24 的固定速度。

```c#
	void DoPathfinding () {
		UpdateCurrentCell();
		grid.FindPath(selectedUnit.Location, currentCell, 24);
	}
```

请注意，我们不必每次更新都找到新的路径，只需在当前单元格发生变化时才能找到。

```c#
	void DoPathfinding () {
		if (UpdateCurrentCell()) {
			grid.FindPath(selectedUnit.Location, currentCell, 24);
		}
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-18/moving-units/pathfinding.png)

*为一个单位寻找路径。*

我们现在看到，在选择一个单位时移动光标时会出现路径，这也使选择哪个单位变得明显。然而，这些路径并不总是被正确地清理干净。首先，如果光标在地图外，让我们清除旧路径。

```c#
	void DoPathfinding () {
		if (UpdateCurrentCell()) {
			if (currentCell) {
				grid.FindPath(selectedUnit.Location, currentCell, 24);
			}
			else {
				grid.ClearPath();
			}
		}
	}
```

当然，这需要 `HexGrid.ClearPath` 是公开的，所以进行调整。

```c#
	public void ClearPath () {
		…
	}
```

其次，在做出选择时，要清除旧路。

```c#
	void DoSelection () {
		grid.ClearPath();
		UpdateCurrentCell();
		if (currentCell) {
			selectedUnit = currentCell.Unit;
		}
	}
```

最后，在更改编辑模式时清除路径。

```c#
	public void SetEditMode (bool toggle) {
		enabled = !toggle;
		grid.ShowUI(!toggle);
		grid.ClearPath();
	}
```

### 3.6 仅搜索有效目的地

我们并不总是能找到一条路，因为有时无法到达目标单元格。那很好。但有时目标单元格本身无效。例如，我们决定路径不能包括水下单元格。但这可能取决于单位。因此，让我们向 `HexUnit` 添加一个方法，该方法告诉一个单元格是否是有效的目的地。水下单元格不是。

```c#
	public bool IsValidDestination (HexCell cell) {
		return !cell.IsUnderwater;
	}
```

此外，我们只支持每个单元格一个单位。因此，如果目标单元格被占用，它也是无效的。

```c#
	public bool IsValidDestination (HexCell cell) {
		return !cell.IsUnderwater && !cell.Unit;
	}
```

在 `HexGameUI.DoPathfinding` 中使用此方法来忽略无效目的地。

```c#
	void DoPathfinding () {
		if (UpdateCurrentCell()) {
			if (currentCell && selectedUnit.IsValidDestination(currentCell)) {
				grid.FindPath(selectedUnit.Location, currentCell, 24);
			}
			else {
				grid.ClearPath();
			}
		}
	}
```

### 3.7 移动到目的地

如果我们有一条有效的路径，那么应该可以将单位移动到目的地。`HexGrid` 知道这是否属实。让它通过新的只读 `HasPath` 属性公开此信息。

```c#
	public bool HasPath {
		get {
			return currentPathExists;
		}
	}
```

要移动该单位，请在 `HexGameUI` 中添加 `DoMove` 方法。当发出移动命令并且我们选择了一个单位时，将调用此方法。因此，它应该检查我们是否有路径，如果有，则更改单位的位置。目前，我们将直接将单位传送到目的地。我们将在后面的教程中让它实际遍历路径。

```c#
	void DoMove () {
		if (grid.HasPath) {
			selectedUnit.Location = currentCell;
			grid.ClearPath();
		}
	}
```

让我们使用鼠标右键单击——鼠标按钮 1——来命令移动。当我们有选择时，请检查一下。另一种选择是进行寻路。

```c#
	void Update () {
		if (!EventSystem.current.IsPointerOverGameObject()) {
			if (Input.GetMouseButtonDown(0)) {
				DoSelection();
			}
			else if (selectedUnit) {
				if (Input.GetMouseButtonDown(1)) {
					DoMove();
				}
				else {
					DoPathfinding();
				}
			}
		}
	}
```

现在我们可以移动单位了！但他们有时拒绝找到通往某些单元格的途径。具体来说，过去有一个单位的单元格。这是因为 `HexUnit` 在设置新位置时不会更新其旧位置。要解决此问题，请清除其旧位置的单位引用。

```c#
	public HexCell Location {
		get {
			return location;
		}
		set {
			if (location) {
				location.Unit = null;
			}
			location = value;
			value.Unit = this;
			transform.localPosition = value.Position;
		}
	}
```

### 3.8 避免单位

Pathfind 现在工作正常，单位可以在地图上传送。虽然它们不能移动到已经有单位的单元格中，但恰好位于路径上的单位会被忽略。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-18/moving-units/ignoring-units.png)

*忽略路径上的单位。*

同一派系的单位通常能够相互移动，但我们还没有单位派系。因此，让我们将所有单位视为无关联的，相互阻碍。我们可以通过跳过 `HexGrid.Search` 中已占用的单元格来实现这一点。

```c#
				if (
					neighbor == null ||
					neighbor.SearchPhase > searchFrontierPhase
				) {
					continue;
				}
				if (neighbor.IsUnderwater || neighbor.Unit) {
					continue;
				}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-18/moving-units/avoiding-units.png)

*避开单位。*

下一个教程是[运动动画](https://catlikecoding.com/unity/tutorials/hex-map/part-19/)。

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-18/moving-units/moving-units.unitypackage)

[PDF](https://catlikecoding.com/unity/tutorials/hex-map/part-18/Hex-Map-18.pdf)

# Hex Map 19：运动动画

发布于 2017-07-22

https://catlikecoding.com/unity/tutorials/hex-map/part-19/

*使单位在单元格之间移动。*
*可视化一下走过的路。*
*沿曲线移动单位。*
*让部队看看他们要去哪里。*

这是关于[六边形地图](https://catlikecoding.com/unity/tutorials/hex-map/)的系列教程的第 19 部分。这一次，我们将让部队沿着路径行进，而不是传送。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-19/tutorial-image.jpg)

*部队正在途中。*

## 1 走一条路

在[前面的教程](https://catlikecoding.com/unity/tutorials/hex-map/part-18/)中，我们添加了单位以及移动它们的能力。尽管我们使用寻路来检测单位的有效目的地，但在发出移动命令时，我们将其传送。为了使它们真正遵循找到的路径，我们需要跟踪这条路径，并创建一个动画过程，使单位从一个单元格移动到另一个单元格。因为很难通过观看动画来准确地看到一个单位是如何移动的，所以我们还将使用小控件（gizmos）来可视化行进的路径。但在我们继续之前，我们应该修复一个 bug。

### 1.1 回合 Bug

由于之前的疏忽，我们错误地计算了到达单元格的回合。现在，我们通过将累计距离除以单位速度来确定回合，$t = \frac{d}{s}$，丢弃剩余部分。当进入一个单元格最终完全消耗了回合的所有剩余动作时，就会出错。

例如，当每一步的成本为 1，速度为 3 时，我们可以每回合移动三个单元格。然而，我们目前的计算只允许在第一个回合走两步，因为第三步 $t = \frac{d}{s}=\frac{3}{3}=1$.

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-19/traveling-a-path/incorrect-turns.png)

*不正确回合的累积移动成本，速度 3。*

为了正确计算回合，我们必须将回合边界从起始单元格移动一步。我们可以通过在确定回合之前将距离减少一个来实现这一点。然后第三步的回合变成 $t=\frac{2}{3}=0$ 了。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-19/traveling-a-path/correct-turns.png)

*正确回合。*

我们可以通过将回合计算改为 $t=\frac{d-1}{s}$。在 `HexGrid.Search` 中进行此调整。

```c#
	bool Search (HexCell fromCell, HexCell toCell, int speed) {
		…
		while (searchFrontier.Count > 0) {
			…

			int currentTurn = (current.Distance - 1) / speed;

			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
				…

				int distance = current.Distance + moveCost;
				int turn = (distance - 1) / speed;
				if (turn > currentTurn) {
					distance = turn * speed + moveCost;
				}

				…
			}
		}
		return false;
	}
```

同时调整回合标签。

```c#
	void ShowPath (int speed) {
		if (currentPathExists) {
			HexCell current = currentPathTo;
			while (current != currentPathFrom) {
				int turn = (current.Distance - 1) / speed;
				…
			}
		}
		…
	}
```

请注意，采用这种方法，起始单元格的回合数为 -1。这很好，因为我们不显示它，搜索算法仍然有效。

### 1.2 检索路径

走一条路是一个单位的工作。为了做到这一点，它需要知道走哪条路。`HexGrid` 有这个信息，所以给它一个方法，以单元格列表的形式检索当前路径。如果确实有路径，它可以从列表池中获取一个并返回。

```c#
	public List<HexCell> GetPath () {
		if (!currentPathExists) {
			return null;
		}
		List<HexCell> path = ListPool<HexCell>.Get();
		return path;
	}
```

按照从目的地到起点的路径引用来填充列表，就像我们在可视化它时所做的那样。

```c#
		List<HexCell> path = ListPool<HexCell>.Get();
		for (HexCell c = currentPathTo; c != currentPathFrom; c = c.PathFrom) {
			path.Add(c);
		}
		return path;
```

在这种情况下，我们需要整个路径，其中也包括起始单元格。

```c#
		for (HexCell c = currentPathTo; c != currentPathFrom; c = c.PathFrom) {
			path.Add(c);
		}
		path.Add(currentPathFrom);
		return path;
```

我们现在有了相反顺序的路径。虽然我们可以处理这个问题，但这并不直观。所以，让我们反转列表，使其从开始到目的地排序。

```c#
		path.Add(currentPathFrom);
		path.Reverse();
		return path;
```

### 1.3 请求旅行

现在，我们可以向 `HexUnit` 添加一个方法，使其遵循一条路径。最初，我们只是让它传送到目的地。我们不会立即将列表放回其池中，因为我们将使用它一段时间。

```c#
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class HexUnit : MonoBehaviour {

	…

	public void Travel (List<HexCell> path) {
		Location = path[path.Count - 1];
	}

	…
}
```

要实际请求旅程，请更改 `HexGameUI.DoMove`，因此它使用当前路径调用新方法，而不是直接设置单位的位置。

```c#
	void DoMove () {
		if (grid.HasPath) {
//			selectedUnit.Location = currentCell;
			selectedUnit.Travel(grid.GetPath());
			grid.ClearPath();
		}
	}
```

### 1.4 可视化路径

在开始制作单位动画之前，让我们检查路径是否正确。我们将通过让 `HexUnit` 记住它应该行进的路径来实现这一点，这样它就可以使用小工具来可视化它。

```c#
	List<HexCell> pathToTravel;

	…

	public void Travel (List<HexCell> path) {
		Location = path[path.Count - 1];
		pathToTravel = path;
	}
```

添加一个 `OnDrawGizmos`方法来显示应该行进的最后一条路径（如果有的话）。如果该单元尚未移动，则路径应为 `null`。但由于 Unity 的编辑时序列化，在播放模式下重新编译后，它也可能是一个空列表。

```c#
	void OnDrawGizmos () {
		if (pathToTravel == null || pathToTravel.Count == 0) {
			return;
		}
	}
```

显示路径的最简单方法是为其中的每个单元格绘制一个小控件球体。半径为 2 个单位的球体清晰可见，不会造成太大阻碍。

```c#
	void OnDrawGizmos () {
		if (pathToTravel == null || pathToTravel.Count == 0) {
			return;
		}

		for (int i = 0; i < pathToTravel.Count; i++) {
			Gizmos.DrawSphere(pathToTravel[i].Position, 2f);
		}
	}
```

因为我们显示了每个单元的路径，所以我们可以一次看到它们的所有最新路径。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-19/traveling-a-path/gizmos.png)

*小控件显示了上次走过的路径。*

为了更好地指示单元格是如何连接的，在循环中前一个单元格和当前单元格之间的线上绘制多个球体。这要求我们从第二个单元格开始这一过程。球体可以通过线性插值放置，增量为 0.1，因此我们得到每段 10 个球体。

```c#
		for (int i = 1; i < pathToTravel.Count; i++) {
			Vector3 a = pathToTravel[i - 1].Position;
			Vector3 b = pathToTravel[i].Position;
			for (float t = 0f; t < 1f; t += 0.1f) {
				Gizmos.DrawSphere(Vector3.Lerp(a, b, t), 2f);
			}
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-19/traveling-a-path/more-obvious-paths.png)

*更明显的路径。*

### 1.5 沿着路径滑

我们可以使用相同的方法来移动单位。让我们为此创建一个协程。设置单位的位置，而不是绘制小控件。使用时间增量，而不是固定的 0.1 增量。并产生每次迭代。这将在一秒钟内将该单位从一个单元格移动到下一个单元格。

```c#
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class HexUnit : MonoBehaviour {

	…
	
	IEnumerator TravelPath () {
		for (int i = 1; i < pathToTravel.Count; i++) {
			Vector3 a = pathToTravel[i - 1].Position;
			Vector3 b = pathToTravel[i].Position;
			for (float t = 0f; t < 1f; t += Time.deltaTime) {
				transform.localPosition = Vector3.Lerp(a, b, t);
				yield return null;
			}
		}
	}
	
	…
}
```

在 `Travel` 方法结束时启动协程。但在此之前，停止所有现有的协程。这确保了我们不会同时运行两个，这可能会产生非常奇怪的结果。

```c#
	public void Travel (List<HexCell> path) {
		Location = path[path.Count - 1];
		pathToTravel = path;
		StopAllCoroutines();
		StartCoroutine(TravelPath());
	}
```

每秒只移动一个单元格是相当缓慢的。玩家不想在玩游戏时等那么久。您可以将单位移动速度设置为配置选项，但现在我们只使用一个常数。我将其设置为每秒四个单元格，这相当快，但仍然足够慢，你可以看到发生了什么。

```c#
	const float travelSpeed = 4f;

	…

	IEnumerator TravelPath () {
		for (int i = 1; i < pathToTravel.Count; i++) {
			Vector3 a = pathToTravel[i - 1].Position;
			Vector3 b = pathToTravel[i].Position;
			for (float t = 0f; t < 1f; t += Time.deltaTime * travelSpeed) {
				transform.localPosition = Vector3.Lerp(a, b, t);
				yield return null;
			}
		}
	}
```

就像我们可以同时可视化多条路径一样，我们也可以让多个单位同时运行。就游戏状态而言，移动仍然是传送。动画纯粹是视觉上的。部队立即占领其目标单元格。你甚至可以在它们到达之前找到路径并开始新的旅程。在这种情况下，它们将视觉上传送到新路径的起点。您可以通过在移动时锁定单元甚至整个 UI 来防止这种情况，但在开发和测试移动时，这种快速响应非常方便。

*旅行单位。*

> **海拔差异呢？**
>
> 因为我们在单位位置之间进行插值，所以我们也插值了单元的垂直位置。由于这与实际几何形状不符，该装置在移动时最终会在地形上方盘旋，在地形下方下沉。由于动画速度很快，而且通常从远处看，这通常在视觉上并不明显。玩家们正忙于玩游戏，而不是仔细观察单个单位的动作。如果你分析不同高度的策略游戏，例如《无尽帝国》（Endless Legend），你会发现它们的单位也会悬停和下沉。如果他们能逃脱惩罚，我们也能。

### 1.6 编译后的位置

协程的一个缺点是，在播放模式下，它们无法在重新编译后存活。虽然游戏状态总是正确的，但如果你碰巧在它们移动时触发了重新编译，这可能会导致它们在最后一条路径上被卡在某个地方。为了缓解这种情况，让我们确保重新编译后的单元始终位于正确的位置。这可以通过更新他们在 `OnEnable` 中的位置来实现。

```c#
	void OnEnable () {
		if (location) {
			transform.localPosition = location.Position;
		}
	}
```

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-19/traveling-a-path/traveling-a-path.unitypackage)

## 2 流动运动

直接从单元格中心到单元格中心会产生方向突然变化的刚性运动。这对许多游戏来说都很好，但当需要至少有点逼真的动作时，这是不可接受的。所以，让我们修改我们的方法，生产出看起来更有机的东西。

### 2.1 从边缘到边缘

一个单位从单元格的中心开始它的旅程。它行进到单元格边缘的中间，然后进入下一个单元格。它可以直接到达它必须穿过的下一个边缘，而不是移动到该单元格的中心。实际上，该单位在必须改变方向时会走捷径。这对除路径端点之外的所有单元格都是可能的。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-19/flowing-movement/edge-to-edge-cases.png)

*从边缘到边缘的三种方法。*

让我们调整 `OnDrawGizmos` 以显示此方法生成的路径。它必须在单元格边缘之间进行插值，这可以通过平均相邻单元格的位置来找到。我们只需要每次迭代计算一条边，重用前一次迭代中的一条边。这样，我们也可以通过简单地使用它的位置来使它为起始单元格工作。

```c#
	void OnDrawGizmos () {
		if (pathToTravel == null || pathToTravel.Count == 0) {
			return;
		}

		Vector3 a, b = pathToTravel[0].Position;

		for (int i = 1; i < pathToTravel.Count; i++) {
//			Vector3 a = pathToTravel[i - 1].Position;
//			Vector3 b = pathToTravel[i].Position;
			a = b;
			b = (pathToTravel[i - 1].Position + pathToTravel[i].Position) * 0.5f;
			for (float t = 0f; t < 1f; t += 0.1f) {
				Gizmos.DrawSphere(Vector3.Lerp(a, b, t), 2f);
			}
		}
	}
```

为了到达目标单元格的中心，我们必须使用该单元格的位置作为终点，而不是边缘。我们可以在循环中为这种情况添加一个检查，但这是一个如此简单的循环，只需稍作调整即可复制代码，这可能会更清晰。

```c#
	void OnDrawGizmos () {
		…
		
		for (int i = 1; i < pathToTravel.Count; i++) {
			…
		}

		a = b;
		b = pathToTravel[pathToTravel.Count - 1].Position;
		for (float t = 0f; t < 1f; t += 0.1f) {
			Gizmos.DrawSphere(Vector3.Lerp(a, b, t), 2f);
		}
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-19/flowing-movement/edge-to-edge.png)

*基于边的路径。*

由此产生的路径之字形减少，最大转弯角度从 120° 减小到 90°。这可以被视为一种改进，因此将相同的更改应用于 `TravelPath` 协程，以查看其动画效果。

```c#
	IEnumerator TravelPath () {
		Vector3 a, b = pathToTravel[0].Position;

		for (int i = 1; i < pathToTravel.Count; i++) {
//			Vector3 a = pathToTravel[i - 1].Position;
//			Vector3 b = pathToTravel[i].Position;
			a = b;
			b = (pathToTravel[i - 1].Position + pathToTravel[i].Position) * 0.5f;
			for (float t = 0f; t < 1f; t += Time.deltaTime * travelSpeed) {
				transform.localPosition = Vector3.Lerp(a, b, t);
				yield return null;
			}
		}

		a = b;
		b = pathToTravel[pathToTravel.Count - 1].Position;
		for (float t = 0f; t < 1f; t += Time.deltaTime * travelSpeed) {
			transform.localPosition = Vector3.Lerp(a, b, t);
			yield return null;
		}
	}
```

*以不同的速度旅行。*

通过偷工减料，我们使路径段的长度取决于方向的变化。但我们已经定义了每秒单元格的速度。结果是，该单位的速度现在变化不稳定。

### 2.2 遵循曲线

当单位穿过单元格边界时，方向和速度的瞬时变化看起来并不好。逐渐改变方向会更好。我们可以通过让我们的单位遵循曲线而不是直线来支持这一点。我们可以使用 Beziér 曲线来实现这一点。具体来说，我们可以使用二次 Beziér 曲线，将单元格中心作为中间控制点。这样，相邻曲线的切线就会相互镜像，这意味着整个路径将是一条连续的平滑曲线。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-19/flowing-movement/edge-to-edge-curves.png)

*从边到边的曲线。*

创建一个 `Bezier` 实用程序类，其中包含一个在二次 Beziér 曲线上获取点的方法。如“[曲线和样条曲线](https://catlikecoding.com/unity/tutorials/curves-and-splines/)”教程中所述，其公式为 $(1-t)^2 A + 2(1-t)tB + t^2C$，这里 $A$ 到 $C$ 是控制点和 $t$ 是插值器。

```c#
using UnityEngine;

public static class Bezier {

	public static Vector3 GetPoint (Vector3 a, Vector3 b, Vector3 c, float t) {
		float r = 1f - t;
		return r * r * a + 2f * r * t * b + t * t * c;
	}
}
```

> **`GetPoint` 不应该将 `t` 钳制为 0-1 吗？**
>
> 因为我们只在 0-1 范围内使用插值器，所以不必费心对其进行箝位。在实践中，通过曲线插值时几乎总是这样。如果你愿意，你可以制作一个不约束 `t` 参数的 `GetPointClamped` 版本。或者将其设置为默认行为，并将上述方法的名称更改为 `GetPointUnclamped`。

要在 `OnDrawGizmos` 中显示弯曲路径，我们必须跟踪三个点而不是两个点。额外的点是我们正在迭代的单元格的中心，它的索引为 `i - 1`，因为循环从 1 开始。一旦我们有了所有的点，我们就可以用 `Bezier.GetPoint` 替换 `Vector3.Lerp`。

对于起点和终点单元格，我们可以简单地将单元格中心用作终点和中点。

```c#
	void OnDrawGizmos () {
		if (pathToTravel == null || pathToTravel.Count == 0) {
			return;
		}

		Vector3 a, b, c = pathToTravel[0].Position;

		for (int i = 1; i < pathToTravel.Count; i++) {
			a = c;
			b = pathToTravel[i - 1].Position;
			c = (b + pathToTravel[i].Position) * 0.5f;
			for (float t = 0f; t < 1f; t += Time.deltaTime * travelSpeed) {
				Gizmos.DrawSphere(Bezier.GetPoint(a, b, c, t), 2f);
			}
		}

		a = c;
		b = pathToTravel[pathToTravel.Count - 1].Position;
		c = b;
		for (float t = 0f; t < 1f; t += 0.1f) {
			Gizmos.DrawSphere(Bezier.GetPoint(a, b, c, t), 2f);
		}
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-19/flowing-movement/bezier.png)

*用 Beziér 曲线制成的路径。*

这条弯曲的小路看起来好多了。将相同的更改应用于 `TravelPath`，并观察使用此方法的单位动画效果。

```c#
	IEnumerator TravelPath () {
		Vector3 a, b, c = pathToTravel[0].Position;

		for (int i = 1; i < pathToTravel.Count; i++) {
			a = c;
			b = pathToTravel[i - 1].Position;
			c = (b + pathToTravel[i].Position) * 0.5f;
			for (float t = 0f; t < 1f; t += Time.deltaTime * travelSpeed) {
				transform.localPosition = Bezier.GetPoint(a, b, c, t);
				yield return null;
			}
		}

		a = c;
		b = pathToTravel[pathToTravel.Count - 1].Position;
		c = b;
		for (float t = 0f; t < 1f; t += Time.deltaTime * travelSpeed) {
			transform.localPosition = Bezier.GetPoint(a, b, c, t);
			yield return null;
		}
	}
```

*通过弯道行驶。*

即使单位的速度不是恒定的，动画也很平滑。由于相邻段的曲线切线匹配，因此速度没有不连续性。速度的变化是渐进的，当一个单位穿过一个单元格时发生，当它改变方向时速度会减慢。如果它直线前进，速度是恒定的。该装置也以零速度开始和结束其旅程。这模仿了真实的动作是如何工作的，所以让我们就这样吧。

### 2.3 追踪时间

到目前为止，我们已经从 0 开始迭代每个分段，一直持续到 1。当递增固定数量时，这很好，但我们的迭代取决于时间增量。当一个分段的迭代结束时，我们很可能会超过 1 一定量，具体取决于时间增量。对于高帧率来说，这并不明显，但当帧率较低时，由于时间损失，可能会导致卡顿。

为了防止时间损失，我们必须将剩余时间从一个片段转移到下一个片段。这可以通过在整个行程中跟踪 `t` 来实现，而不仅仅是每个分段。然后在每段末尾，从中减去 1。

```c#
	IEnumerator TravelPath () {
		Vector3 a, b, c = pathToTravel[0].Position;

		float t = 0f;
		for (int i = 1; i < pathToTravel.Count; i++) {
			a = c;
			b = pathToTravel[i - 1].Position;
			c = (b + pathToTravel[i].Position) * 0.5f;
			for (; t < 1f; t += Time.deltaTime * travelSpeed) {
				transform.localPosition = Bezier.GetPoint(a, b, c, t);
				yield return null;
			}
			t -= 1f;
		}

		a = c;
		b = pathToTravel[pathToTravel.Count - 1].Position;
		c = b;
		for (; t < 1f; t += Time.deltaTime * travelSpeed) {
			transform.localPosition = Bezier.GetPoint(a, b, c, t);
			yield return null;
		}
	}
```

当我们这样做的时候，我们也可以在旅程开始时考虑时间增量。这意味着我们立即开始移动，而不是在一帧内保持静止。

```c#
		float t = Time.deltaTime * travelSpeed;
```

最后，我们不会在路径应该完成的时间结束，而只是在它之前。再一次，差异有多大取决于帧率。因此，让我们确保该单元最终准确到达目的地。

```c#
	IEnumerator TravelPath () {
		…

		transform.localPosition = location.Position;
	}
```

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-19/flowing-movement/flowing-movement.unitypackage)

## 3 动画定向

虽然这些单位遵循一条很好的曲线，但它们不会改变方向以匹配其行进方向。因此，它们似乎在滑动。为了使其看起来像实际的运动，我们也必须旋转它们。

### 3.1 展望未来

与“[曲线和样条线](https://catlikecoding.com/unity/tutorials/curves-and-splines/)”教程中一样，我们可以使用曲线的导数来确定单位的方向。二次 Beziér 曲线导数的公式为 $2((1-t)(B-A)+t(C-B))$. 在 `Bezier` 中添加一种计算方法。

```c#
	public static Vector3 GetDerivative (
		Vector3 a, Vector3 b, Vector3 c, float t
	) {
		return 2f * ((1f - t) * (b - a) + t * (c - b));
	}
```

导数矢量与行进方向对齐。我们可以使用 `Quaternion.LookRotation` 方法将其转换为我们单位的旋转。对 `HexUnit.TravelPath` 中的每一步都这样做。

```c#
				transform.localPosition = Bezier.GetPoint(a, b, c, t);
				Vector3 d = Bezier.GetDerivative(a, b, c, t);
				transform.localRotation = Quaternion.LookRotation(d);
				yield return null;
		
		…
		
			transform.localPosition = Bezier.GetPoint(a, b, c, t);
			Vector3 d = Bezier.GetDerivative(a, b, c, t);
			transform.localRotation = Quaternion.LookRotation(d);
			yield return null;
```

> **这不是在道路的起点失败了吗？**
>
> 旅程从起始单元格的中心到边缘的曲线开始。我们使 $A$ 和 $B$ 曲线上的点相等，所以我们得到了很好的加速度。然而，这意味着当 $t = 0$ 导数向量为零， `Quaternion.LookRotation` 不能使用。所以，是的，如果我们第一部分以 $t = 0$ 开始的话，该方法无法使用。但我们没有。我们立即从时间增量开始，所以 $t > 0$，所以它总是有效的。
>
> 这也不是最终曲线末端的问题，因为我们强制执行 $t < 1$

与该单元的位置不同，它在旅程结束时的方向是否完美并不重要。然而，我们确实需要确保它的方向与它的最终旋转相匹配。为此，一旦我们完成，使其方向等于其 Y 旋转。

```c#
		transform.localPosition = location.Position;
		orientation = transform.localRotation.eulerAngles.y;
```

现在，单位正面对着它们前进的确切方向，包括水平和垂直方向。这意味着它们在上坡和下坡时会前后倾斜。为了使它们保持直立，在使用方向向量的 Y 分量来确定单位的旋转之前，将其强制为零。

```c#
				Vector3 d = Bezier.GetDerivative(a, b, c, t);
				d.y = 0f;
				transform.localRotation = Quaternion.LookRotation(d);
		
		…
		
			Vector3 d = Bezier.GetDerivative(a, b, c, t);
			d.y = 0f;
			transform.localRotation = Quaternion.LookRotation(d);
```

*旅行时面朝前方。*

### 3.2 看东西

单位在整个旅程中都面向前方，但在此之前，他们可能会朝着不同的方向看。在这种情况下，它们会立即改变方向。如果我们让他们在开始移动之前转向他们的行进方向，看起来会更好。

查看某些内容在其他情况下也可能有用，因此让我们创建一个 `LookAt` 方法，使单元改变方向以查看特定点。可以使用 `Transform.LookAt` 方法设置所需的旋转，在我们确保点与单位具有相同的垂直位置之后。之后，我们可以提取单位的方向。

```c#
	void LookAt (Vector3 point) {
		point.y = transform.localPosition.y;
		transform.LookAt(point);
		orientation = transform.localRotation.eulerAngles.y;
	}
```

为了使单位真正旋转，我们将把我们的方法变成另一个以固定速度旋转的协程。旋转速度也可以配置，但我们将再次使用常数。旋转应该很快，大约每秒180°。

```c#
	const float rotationSpeed = 180f;
	
	…

	IEnumerator LookAt (Vector3 point) {
		…
	}
```

无需为旋转加速而烦恼，因为这在视觉上并不明显。我们可以简单地在两个方向之间进行插值。不幸的是，这并不像在两个数字之间插值那么简单，因为角度是圆形的。例如，从 350° 到 10° 应导致顺时针旋转 20°，但直接插值将逆时针旋转 340°。

创建正确旋转的最简单方法是使用球面插值在两个四元数之间进行插值。这会导致尽可能短的旋转。为此，检索开始和目标四元数，并使用 `Quaternion.Slerp` 在它们之间进行转换。

```c#
	IEnumerator LookAt (Vector3 point) {
		point.y = transform.localPosition.y;
		Quaternion fromRotation = transform.localRotation;
		Quaternion toRotation =
			Quaternion.LookRotation(point - transform.localPosition);
		
		for (float t = Time.deltaTime; t < 1f; t += Time.deltaTime) {
			transform.localRotation =
				Quaternion.Slerp(fromRotation, toRotation, t);
			yield return null;
		}

		transform.LookAt(point);
		orientation = transform.localRotation.eulerAngles.y;
	}
```

这是可行的，但插值始终从 0 到 1，与旋转角度无关。为了确保角速度均匀，我们必须随着旋转角度的增加而减慢插值速度。

```c#
		Quaternion fromRotation = transform.localRotation;
		Quaternion toRotation =
			Quaternion.LookRotation(point - transform.localPosition);
		float angle = Quaternion.Angle(fromRotation, toRotation);
		float speed = rotationSpeed / angle;
		
		for (
			float t = Time.deltaTime * speed;
			t < 1f;
			t += Time.deltaTime * speed
		) {
			transform.localRotation =
				Quaternion.Slerp(fromRotation, toRotation, t);
			yield return null;
		}
```

知道角度后，如果旋转结果为零，我们可以完全跳过旋转。

```c#
		float angle = Quaternion.Angle(fromRotation, toRotation);

		if (angle > 0f) {
			float speed = rotationSpeed / angle;
			for (
				…
			) {
				…
			}
		}
```

现在，我们可以在 `TravelPath` 中添加单位旋转，只需在移动前用第二个单元格的位置生成 `LookAt` 即可。Unity 将自动启动 `LookAt` 协程，`TravelPath` 将等待其完成。

```c#
	IEnumerator TravelPath () {
		Vector3 a, b, c = pathToTravel[0].Position;
		yield return LookAt(pathToTravel[1].Position);

		float t = Time.deltaTime * travelSpeed;
		…
	}
```

尝试此操作时，该单位将传送到目的地，在那里旋转，然后传送回路径的起点并从那里旅行。这是因为我们在启动 `TravelPath` 协程之前分配了 `Location` 属性。为了摆脱传送，我们可以将单位的位置恢复到 `TravelPath` 开始时的起始单元格。

```c#
		Vector3 a, b, c = pathToTravel[0].Position;
		transform.localPosition = c;
		yield return LookAt(pathToTravel[1].Position);
```

*移动前先旋转。*

### 3.3 清理

对我们的运动感到满意，我们可以摆脱 `OnDrawGizmos` 方法。要么删除它，要么注释掉它，以防将来可能想看到路径。

```c#
//	void OnDrawGizmos () {
//		…
//	}
```

由于我们不再需要记住我们走了哪条路，我们可以在 `TravelPath` 的末尾发布单元格列表。

```c#
	IEnumerator TravelPath () {
		…
		ListPool<HexCell>.Add(pathToTravel);
		pathToTravel = null;
	}
```

> **那么实际的单位动画呢？**
>
> 因为我使用一个简单的立方体作为一个单位，所以没有其他东西可以制作动画。但是当使用 3D 模型时，您会希望为它们提供逼真的运动动画。你不需要非常花哨的动画。战略游戏可以为他们的小单位提供简单的效果。一个空闲加上一个移动的动画就足够了，使用 Mecanim 在两者之间进行混合，通过 `TravelPath` 进行控制。

下一个教程是[战争迷雾](https://catlikecoding.com/unity/tutorials/hex-map/part-20/)。

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-19/animating-orientation/animating-orientation.unitypackage)

[PDF](https://catlikecoding.com/unity/tutorials/hex-map/part-19/Hex-Map-19.pdf)