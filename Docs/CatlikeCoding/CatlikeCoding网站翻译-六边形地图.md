# [返回主 Markdown](./CatlikeCoding网站翻译.md)

# Hex Map 1：创建六边形网格

发布于 2016-01-30

https://catlikecoding.com/unity/tutorials/hex-map/part-1/

*把正方形变成六边形。*
*将六边形网格三角形化。*
*使用立方体坐标。*
*与网格单元交互。*
*制作一个游戏内编辑器。*



本教程是关于六边形地图系列的第一部分。许多游戏使用六边形网格，尤其是战略游戏，包括《奇迹时代 3》（Age of Wonders 3）、《文明 5》（Civilization 5）和《无尽传奇》（Endless Legend）。我们将从基础开始，逐步添加功能，直到我们最终得到一个复杂的基于六边形的地形。

本教程假设您已经完成了*网格基础*系列，该系列从[程序网格](https://catlikecoding.com/unity/tutorials/procedural-grid/)开始。它是用 Unity 5.3.1 创建的。整个系列通过 Unity 的多个版本进行。最后一部分是用 Unity 2017.3.0p3 制作的。

现在还有 [Hex Map 项目](https://bitbucket.org/catlikecoding-projects/hex-map-project/src/master/)，它使 Hex Map 现代化并使用 URP。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-1/tutorial-image.png)

*一个基础六边形地图*

## 1 关于六边形

为什么使用六边形？如果你需要一个网格，只使用正方形是有意义的。正方形确实很容易绘制和定位，但它们也有缺点。看看网格中的一个正方形。然后看看它的邻居。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-1/about-hexagons/square-grid.png)

*一个正方形和它的邻居。*

总共有八个邻居。四个人可以通过穿过正方形的边缘到达。它们是水平和垂直的邻居。其他四个可以通过穿过正方形的一个角到达。这些是对角线上的邻居。

网格中相邻方形单元的中心之间的距离是多少？如果边长为 1，则水平和垂直相邻的答案为 1。但对于对角线邻居，答案是 √2。

这两种邻居之间的差异导致了复杂的情况。如果你使用离散运动，你如何处理对角线运动？你允许吗？如何打造更自然的外观？不同的游戏使用不同的方法，有不同的优缺点。一种方法是根本不使用方形网格，而是使用六边形。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-1/about-hexagons/hexagon-grid.png)

*六边形及其邻居。*

与正方形相比，六边形只有六个邻居，而不是八个。所有这些邻居都是边邻居。没有角邻居。所以只有一种邻居，这简化了很多事情。当然，六边形网格的构造不如正方形网格简单，但我们可以处理这个问题。

在开始之前，我们必须确定六边形的大小。让我们选择 10 个单位的边长。因为六边形由六个等边三角形组成的圆组成，所以从中心到任何角的距离也是 10。这定义了六边形单元的外半径。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-1/about-hexagons/hexagon.png)

*六边形的外半径和内半径。*

还有一个内半径，即从中心到每条边的距离。这个度量很重要，因为到每个邻居中心的距离等于这个值的两倍。内半径等于 $\frac{\sqrt 3} 2$ 乘以外半径，因此在我们的情况下为 $5 \sqrt 3$。让我们把这些指标放在一个静态类中，以便于访问。

```c#
using UnityEngine;

public static class HexMetrics {

	public const float outerRadius = 10f;

	public const float innerRadius = outerRadius * 0.866025404f;
}
```

> **你是如何得出内半径的？**
>
> 取六边形的六个三角形中的一个。内半径等于这个三角形的高度。通过将三角形拆分为两个直角三角形，你可以得到这个高度，然后你可以使用[毕达哥拉斯定理](https://en.wikipedia.org/wiki/Pythagorean_theorem)。
>
> 因此，对于边长 $e$，内半径为 $\sqrt{e^2 - (\frac{e^2}2)^2}=\sqrt{3\frac{e^2}4}=e\frac{\sqrt3}2 \approx 0.886e$.

当我们开始时，让我们定义六个角相对于单元格中心的位置。请注意，有两种方法可以确定六边形的方向。要么尖面朝上，要么平面朝上。我们将在顶部放一个角落。从这个角开始，按顺时针顺序添加其余部分。将它们放置在 XZ 平面中，使六边形与地面对齐。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-1/about-hexagons/orientations.png)

*可能的朝向。*

```c#
	public static Vector3[] corners = {
		new Vector3(0f, 0f, outerRadius),
		new Vector3(innerRadius, 0f, 0.5f * outerRadius),
		new Vector3(innerRadius, 0f, -0.5f * outerRadius),
		new Vector3(0f, 0f, -outerRadius),
		new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
		new Vector3(-innerRadius, 0f, 0.5f * outerRadius)
	};
```

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-1/about-hexagons/about-hexagons.unitypackage)

## 2 网格建设（Grid Construction）

要创建六边形网格，我们需要网格单元。为此目的创建一个 HexCell 组件。暂时留空，因为我们还没有使用任何单元格数据。

```c#
using UnityEngine;

public class HexCell : MonoBehaviour {
}
```

要真正简单地开始，请创建一个默认平面对象，将单元格组件添加到其中，并将其转换为预制件。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-1/grid-construction/hex-cell-plane.png)

*将平面用作六角单元预制件。*

接下来是网格（grid）。使用公共宽度、高度和单元格预制变量创建一个简单的组件。然后将带有此组件的游戏对象添加到场景中。

```c#
using UnityEngine;

public class HexGrid : MonoBehaviour {

	public int width = 6;
	public int height = 6;

	public HexCell cellPrefab;

}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-1/grid-construction/hex-grid.png)

*六边形网格对象。*

让我们从创建一个规则的方形网格开始，因为我们已经知道如何做到这一点。将单元格存储在数组中，以便我们以后可以访问它们。

由于默认平面为 10 乘 10 个单位，因此将每个单元格偏移该量。

```c#
	HexCell[] cells;

	void Awake () {
		cells = new HexCell[height * width];

		for (int z = 0, i = 0; z < height; z++) {
			for (int x = 0; x < width; x++) {
				CreateCell(x, z, i++);
			}
		}
	}
	
	void CreateCell (int x, int z, int i) {
		Vector3 position;
		position.x = x * 10f;
		position.y = 0f;
		position.z = z * 10f;

		HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
		cell.transform.SetParent(transform, false);
		cell.transform.localPosition = position;
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-1/grid-construction/square-grid-of-planes.png)

*正方形的平面网格。*

这为我们提供了一个无缝的方形网格。但哪个单元格在哪里？当然，这对我们来说很容易检查，但使用六边形会变得更加棘手。如果我们能一次看到所有单元格坐标，那将很方便。

### 2.1 显示坐标

通过 *GameObject / UI / Canvas* 将画布添加到场景中，并使其成为网格对象的子对象。由于这是一个纯粹的信息画布，请删除其光线投射器组件。您还可以删除自动添加到场景中的事件系统对象，因为我们还不需要它。

将“*渲染模式*”设置为“*世界空间*”，并围绕 X 轴旋转 90 度，使画布覆盖我们的网格。将其枢轴设置为零，并将其位置也设置为零。给它一个轻微的垂直偏移，这样它的内容就会显示在顶部。它的宽度和高度并不重要，因为我们会自己定位它的内容。您可以将它们设置为零，以删除场景视图中的大矩形。

最后，将“*每单位动态像素数*”增加到 10。这将确保文本对象使用体面的字体纹理分辨率。

![hierarchy](https://catlikecoding.com/unity/tutorials/hex-map/part-1/grid-construction/hierarchy.png)
![inspector](https://catlikecoding.com/unity/tutorials/hex-map/part-1/grid-construction/canvas.png)
*六边形网格坐标的画布。*

要显示坐标，请通过 *GameObject / UI / Text* 创建一个文本对象，并将其转换为预制件。确保其锚和枢轴居中，并将其尺寸设置为 5 乘 15。文本的对齐方式也应水平和垂直居中。将字体大小设置为 4。最后，我们不需要默认文本，也不会使用 *富文本（Rich Text）*。是否启用 *Raycast Target* 并不重要，因为我们的画布无论如何都不会这样做。

![transform and canvas components](https://catlikecoding.com/unity/tutorials/hex-map/part-1/grid-construction/label-part-1.png) ![text component](https://catlikecoding.com/unity/tutorials/hex-map/part-1/grid-construction/label-part-2.png)

*预制单元格标签。*

现在，我们的网格需要了解画布和预制件。添加  `using UnityEngine.UI;` 在脚本顶部，方便访问 [`UnityEngine.UI.Text`](http://docs.unity3d.com/Documentation/ScriptReference/UI.Text.html) 类型。标签 prefab 需要一个公共变量，而画布可以通过调用 [GetComponentInChildren](http://docs.unity3d.com/Documentation/ScriptReference/Component.GetComponentInChildren.html) 来找到。

```c#
	public Text cellLabelPrefab;

	Canvas gridCanvas;

	void Awake () {
		gridCanvas = GetComponentInChildren<Canvas>();
		
		…
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-1/grid-construction/grid-with-label-prefab.png)

*连接标签预制件。*

连接标签预制件后，我们可以实例化它们并显示单元格坐标。在 X 和 Z 之间放置一个换行符，这样它们就会出现在单独的行上。

```c#
	void CreateCell (int x, int z, int i) {
		…

		Text label = Instantiate<Text>(cellLabelPrefab);
		label.rectTransform.SetParent(gridCanvas.transform, false);
		label.rectTransform.anchoredPosition =
			new Vector2(position.x, position.z);
		label.text = x.ToString() + "\n" + z.ToString();
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-1/grid-construction/grid-with-labels.png)

*可见坐标。*

### 2.2 六边形位置

现在我们可以直观地识别每个细胞，让我们开始移动它们。我们知道，X 方向上相邻六边形单元之间的距离等于内半径的两倍。所以，让我们使用它。此外，到下一行单元格的距离应为外半径的 1.5 倍。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-1/grid-construction/neighbor-distances.png)

*六边形邻居的几何学。*

```c#
		position.x = x * (HexMetrics.innerRadius * 2f);
		position.y = 0f;
		position.z = z * (HexMetrics.outerRadius * 1.5f);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-1/grid-construction/hex-distances.png)

*使用六边形距离，无偏移。*

当然，连续的六边形行并不直接位于彼此之上。每行沿 X 轴偏移内半径。我们可以通过在乘以内半径的两倍之前将 Z 的一半加到 X 上来实现这一点。

```c#
		position.x = (x + z * 0.5f) * (HexMetrics.innerRadius * 2f);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-1/grid-construction/rhombus.png)

*正确的六边形位置会产生菱形网格。*

虽然这将我们的单元格放置在适当的六边形位置，但我们的网格现在填充了菱形而不是矩形。由于使用矩形网格更方便，让我们将单元格重新排成一行。我们通过取消部分偏移来实现这一点。每第二行，所有单元格都应再向后移动一步。在乘法之前，将 Z 的整数除以 2 就可以了。

```c#
		position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-1/grid-construction/rectangular-area.png)

*矩形区域中的六边形间距。*

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-1/grid-construction/grid-construction.unitypackage)

## 3 渲染六边形

正确定位单元格后，我们可以继续显示实际的六边形。我们必须先摆脱这些平面，所以从单元预制件中删除除 `HexCell` 之外的所有组件。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-1/rendering-hexagons/empty-cell-prefab.png)

*没有更多的平面。*

就像在[网格基础](https://catlikecoding.com/unity/tutorials/procedural-grid)教程中一样，我们将使用单个网格来渲染整个网格。但是，这次我们不会预先确定我们需要多少个顶点和三角形。我们将改用列表。

创建一个新的 `HexMesh` 组件来管理我们的网格。它需要一个网格过滤器和渲染器，有一个网格，并有顶点和三角形的列表。

```c#
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour {

	Mesh hexMesh;
	List<Vector3> vertices;
	List<int> triangles;

	void Awake () {
		GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
		hexMesh.name = "Hex Mesh";
		vertices = new List<Vector3>();
		triangles = new List<int>();
	}
}
```

使用此组件为我们的网格创建一个新的子对象。它将自动获得网格渲染器，但不会为其分配材质。因此，请向其添加默认材质。

![inspector](https://catlikecoding.com/unity/tutorials/hex-map/part-1/rendering-hexagons/hex-mesh.png) ![hierarchy](https://catlikecoding.com/unity/tutorials/hex-map/part-1/rendering-hexagons/hierarchy.png)

*六边形网格对象。*

现在 `HexGrid` 可以检索其六边形网格，就像它查找画布一样。

```c#
	HexMesh hexMesh;

	void Awake () {
		gridCanvas = GetComponentInChildren<Canvas>();
		hexMesh = GetComponentInChildren<HexMesh>();
		
		…
	}
```

网格唤醒后，它必须告诉网格对其单元格进行三角剖分。我们必须确保在六角形网格组件唤醒后也会发生这种情况。稍后调用 `Start` 时，让我们在那里执行。

```c#
	void Start () {
		hexMesh.Triangulate(cells);
	}
```

这个 `HexMesh.Triangulate` 三角剖分方法可以在任何时候调用，即使单元格之前已经进行了三角剖分。因此，我们应该从清除旧数据开始。然后遍历所有单元格，分别进行三角剖分。完成后，将生成的顶点和三角形指定给网格，最后重新计算网格法线。

```c#
	public void Triangulate (HexCell[] cells) {
		hexMesh.Clear();
		vertices.Clear();
		triangles.Clear();
		for (int i = 0; i < cells.Length; i++) {
			Triangulate(cells[i]);
		}
		hexMesh.vertices = vertices.ToArray();
		hexMesh.triangles = triangles.ToArray();
		hexMesh.RecalculateNormals();
	}
	
	void Triangulate (HexCell cell) {
	}
```

由于六边形是由三角形组成的，让我们创建一个方便的方法来添加一个三角形，给定三个顶点位置。它只是按顺序添加顶点。它还添加了这些顶点的索引以形成三角形。在将新顶点添加到顶点列表之前，第一个顶点的索引等于顶点列表的长度。因此，在添加顶点之前，请记住这一点。

```c#
	void AddTriangle (Vector3 v1, Vector3 v2, Vector3 v3) {
		int vertexIndex = vertices.Count;
		vertices.Add(v1);
		vertices.Add(v2);
		vertices.Add(v3);
		triangles.Add(vertexIndex);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 2);
	}
```

现在我们可以对单元格进行三角剖分。让我们从第一个三角形开始。它的第一个顶点是六边形的中心。另外两个顶点是相对于其中心的第一个和第二个角。

```c#
	void Triangulate (HexCell cell) {
		Vector3 center = cell.transform.localPosition;
		AddTriangle(
			center,
			center + HexMetrics.corners[0],
			center + HexMetrics.corners[1]
		);
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-1/rendering-hexagons/one-triangle-per-hex.png)

*每个单元格的第一个三角形。*

这行得通，所以遍历所有六个三角形。

```c#
		Vector3 center = cell.transform.localPosition;
		for (int i = 0; i < 6; i++) {
			AddTriangle(
				center,
				center + HexMetrics.corners[i],
				center + HexMetrics.corners[i + 1]
			);
		}
```

> **我们不能共享顶点吗？**
>
> 是的，我们可以。实际上，我们可以做得更好，只使用四个三角形来渲染一个六边形，而不是六个。但不这样做会让事情变得简单。现在这是个好主意，因为在以后的教程中，事情会变得更加复杂。此时优化顶点和三角形只会造成阻碍。

不幸的是，这会产生 `IndexOutOfRangeException`。这是因为最后一个三角形试图获取不存在的第七个角。当然，它应该向后包裹，并使用第一个角作为其最终顶点。或者，我们可以在 `HexMetrics.corners` 中复制第一个角，这样我们就不必担心越界。

```c#
	public static Vector3[] corners = {
		new Vector3(0f, 0f, outerRadius),
		new Vector3(innerRadius, 0f, 0.5f * outerRadius),
		new Vector3(innerRadius, 0f, -0.5f * outerRadius),
		new Vector3(0f, 0f, -outerRadius),
		new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
		new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
		new Vector3(0f, 0f, outerRadius)
	};
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-1/rendering-hexagons/complete-hexagons.png)

*完整的六边形。*

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-1/rendering-hexagons/rendering-hexagons.unitypackage)

## 4 六角坐标

让我们在六边形网格的背景下再次查看我们的单元格坐标。Z 坐标看起来很好，但 X 坐标呈之字形。这是将行偏移以覆盖矩形区域的副作用。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-1/hexagonal-coordinates/offset-diagram.png)

*偏移坐标，突出显示零线。*

在处理六边形时，这些偏移坐标不容易处理。让我们添加一个 `HexCoordinates` 结构，可以将其转换为不同的坐标系。使其可序列化，以便 Unity 可以存储它，这使它们在播放模式下能够在重新编译后幸存下来。此外，通过使用公共只读属性使这些坐标不可变。

```c#
using UnityEngine;

[System.Serializable]
public struct HexCoordinates {

	public int X { get; private set; }

	public int Z { get; private set; }

	public HexCoordinates (int x, int z) {
		X = x;
		Z = z;
	}
}
```

添加一个静态方法，使用常规偏移坐标创建一组坐标。现在，只需逐字复制这些坐标。

```c#
	public static HexCoordinates FromOffsetCoordinates (int x, int z) {
		return new HexCoordinates(x, z);
	}
}
```

还添加了方便的字符串转换方法。默认的 `ToString` 方法返回结构体的类型名，这没有用。覆盖它以返回单行上的坐标。还要添加一种方法，将坐标放在单独的线上，因为我们已经在使用这样的布局。

```c#
	public override string ToString () {
		return "(" + X.ToString() + ", " + Z.ToString() + ")";
	}

	public string ToStringOnSeparateLines () {
		return X.ToString() + "\n" + Z.ToString();
	}
```

现在我们可以给 `HexCell` 组件一组坐标。

```c#
public class HexCell : MonoBehaviour {

	public HexCoordinates coordinates;
}
```

调整 `HexGrid.CreateCell`，使其利用新的坐标。

```c#
		HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
		cell.transform.SetParent(transform, false);
		cell.transform.localPosition = position;
		cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
		
		Text label = Instantiate<Text>(cellLabelPrefab);
		label.rectTransform.SetParent(gridCanvas.transform, false);
		label.rectTransform.anchoredPosition =
			new Vector2(position.x, position.z);
		label.text = cell.coordinates.ToStringOnSeparateLines();
```

现在，让我们确定这些 X 坐标，使它们沿直线对齐。我们可以通过取消水平偏移来实现这一点。结果通常被称为轴向坐标。

```c#
	public static HexCoordinates FromOffsetCoordinates (int x, int z) {
		return new HexCoordinates(x - z / 2, z);
	}
```

![diagram](https://catlikecoding.com/unity/tutorials/hex-map/part-1/hexagonal-coordinates/axial-diagram.png) ![scene](https://catlikecoding.com/unity/tutorials/hex-map/part-1/hexagonal-coordinates/axial-coordinates.png)

*轴向坐标。*

这个二维坐标系使我们能够一致地描述四个方向上的运动和偏移。然而，剩下的两个方向仍然需要特殊处理。这表明存在第三个维度。事实上，如果我们水平翻转 X 维度，我们就会得到缺失的 Y 维度。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-1/hexagonal-coordinates/cube-diagram.png)

*将显示一个 Y 维度。*

由于这些 X 和 Y 维度相互镜像，如果保持 Z 恒定，将它们的坐标加在一起总是会产生相同的结果。事实上，如果你把所有三个坐标加在一起，你总是会得到零。如果你增加一个坐标，你必须减少另一个坐标。事实上，这产生了六种可能的运动方向。这些坐标通常被称为立方体坐标，因为它们是三维的，拓扑结构类似于立方体。

因为所有坐标加起来都是零，所以你总是可以从另外两个坐标中推导出每个坐标。由于我们已经存储了 X 和 Z 坐标，因此不需要存储 Y 坐标。我们可以包含一个按需计算它的属性，并在字符串方法中使用它。

```c#
	public int Y {
		get {
			return -X - Z;
		}
	}

	public override string ToString () {
		return "(" +
			X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + ")";
	}

	public string ToStringOnSeparateLines () {
		return X.ToString() + "\n" + Y.ToString() + "\n" + Z.ToString();
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-1/hexagonal-coordinates/cube-coordinates.png)

*立方体坐标。*

### 4.1 检视器中的坐标

在播放模式下选择一个网格单元格。事实证明，检视器（inspector）没有显示它的坐标。仅显示 `HexCell.cordinates` 的前缀标签。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-1/hexagonal-coordinates/inspector-empty-coordinates.png)

*检视器不显示坐标。*

虽然这没什么大不了的，但如果坐标确实显示出来，那就太好了。Unity 目前不显示坐标，因为它们没有标记为序列化字段。为此，我们必须为 X 和 Z 显式定义可序列化字段。

```c#
	[SerializeField]
	private int x, z;

	public int X {
		get {
			return x;
		}
	}

	public int Z {
		get {
			return z;
		}
	}

	public HexCoordinates (int x, int z) {
		this.x = x;
		this.z = z;
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-1/hexagonal-coordinates/inspector-editable-coordinates.png)

*丑陋且可编辑。*

现在显示了 X 和 Z 坐标，但它们是可编辑的，我们不希望这样，因为坐标应该是固定的。它们被展示在一起也不好看。

通过为 `HexCoordinates` 类型定义一个自定义属性抽屉，我们可以做得更好。创建一个 `HexCoordinatesDrawer` 脚本并将其放入 *Editor* 文件夹中，因为它是一个仅限编辑器的脚本。

该类应扩展 `UnityEditor.PropertyDrawer`，需要 `UnityEditor.CustomPropertyDrawer` 属性将其与正确的类型相关联。

```c#
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(HexCoordinates))]
public class HexCoordinatesDrawer : PropertyDrawer {
}
```

属性抽屉通过 `OnGUI` 方法呈现其内容。此方法提供了要在其中绘制的屏幕矩形、属性的序列化数据及其所属字段的标签。

```c#
	public override void OnGUI (
		Rect position, SerializedProperty property, GUIContent label
	) {
	}
```

从属性中提取 x 和 z 值，并使用这些值创建一组新的坐标。然后使用我们的 `HexCoordinates.ToString` 方法在指定位置绘制一个 GUI 标签。

```c#
	public override void OnGUI (
		Rect position, SerializedProperty property, GUIContent label
	) {
		HexCoordinates coordinates = new HexCoordinates(
			property.FindPropertyRelative("x").intValue,
			property.FindPropertyRelative("z").intValue
		);
		
		GUI.Label(position, coordinates.ToString());
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-1/hexagonal-coordinates/inspector-no-label.png)

*没有前缀标签的坐标。*

这显示了我们的坐标，但我们现在缺少字段名。这些名称通常使用 `EditorGUI.PrefixLabel` 方法绘制。作为奖励，它返回一个调整后的矩形，与此标签右侧的空间相匹配。

```c#
		position = EditorGUI.PrefixLabel(position, label);
		GUI.Label(position, coordinates.ToString());
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-1/hexagonal-coordinates/inspector-with-label.png)

*带标签的坐标。*

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-1/hexagonal-coordinates/hexagonal-coordinates.unitypackage)

## 5 触摸单元格

如果我们不能与十六进制网格交互，那么它就不是很有趣。最基本的交互是触摸单元格，所以让我们添加对它的支持。现在，只需将此代码直接放入 `HexGrid` 中。一旦一切正常，我们就会将其移动到其他地方。

要触摸单元格，我们可以从鼠标位置向场景中发射光线。我们可以使用与“[网格变形](https://catlikecoding.com/unity/tutorials/mesh-deformation)”教程中相同的方法。

```c#
	void Update () {
		if (Input.GetMouseButton(0)) {
			HandleInput();
		}
	}

	void HandleInput () {
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(inputRay, out hit)) {
			TouchCell(hit.point);
		}
	}
	
	void TouchCell (Vector3 position) {
		position = transform.InverseTransformPoint(position);
		Debug.Log("touched at " + position);
	}
```

这还没有任何作用。我们需要在网格中添加一个对撞机，这样光线就有东西可以击中。所以给 `HexMesh` 一个网格对撞机。

```c#
	MeshCollider meshCollider;

	void Awake () {
		GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
		meshCollider = gameObject.AddComponent<MeshCollider>();
		…
	}
```

完成三角剖分后，将网格分配给碰撞体。

```c#
	public void Triangulate (HexCell[] cells) {
		…
		meshCollider.sharedMesh = hexMesh;
	}
```

> **我们不能用箱式碰撞体（a box collider）吗？**
>
> 我们可以，但它不会完全符合我们网格的轮廓。我们的网格也不会长时间保持平坦，尽管这是未来教程的一部分。

我们现在可以接触网格了！但我们正在触碰哪个单元格？要知道这一点，我们必须将触摸位置转换为六边形坐标。这是 `HexCoordinates` 的一项任务，因此让我们声明它有一个静态的 `FromPosition` 方法。

```c#
	public void TouchCell (Vector3 position) {
		position = transform.InverseTransformPoint(position);
		HexCoordinates coordinates = HexCoordinates.FromPosition(position);
		Debug.Log("touched at " + coordinates.ToString());
	}
```

这种方法如何确定哪个坐标属于一个位置？我们可以从 x 除以六边形的水平宽度开始。因为 Y 坐标是 X 坐标的镜像，所以 x 的负值给了我们 y。

```c#
	public static HexCoordinates FromPosition (Vector3 position) {
		float x = position.x / (HexMetrics.innerRadius * 2f);
		float y = -x;
	}
```

当然，只有当 Z 为零时，我们才能得到正确的坐标。当我们沿着 Z 移动时，我们必须再次移动。每两行，我们应该向左移动一个完整的单位。

```c#
		float offset = position.z / (HexMetrics.outerRadius * 3f);
		x -= offset;
		y -= offset;
```

现在，我们的 x 和 y 值在每个单元格的中心都是整数。因此，通过将它们四舍五入为整数，我们应该得到坐标。我们也推导出 Z，然后构造最终坐标。

```c#
		int iX = Mathf.RoundToInt(x);
		int iY = Mathf.RoundToInt(y);
		int iZ = Mathf.RoundToInt(-x -y);

		return new HexCoordinates(iX, iZ);
```

结果看起来很有希望，但坐标是否正确？一些仔细的探索会发现，我们有时会得到不等于零的坐标！当这种情况发生时，让我们记录一个警告，以确保它真的发生了。

```c#
		if (iX + iY + iZ != 0) {
			Debug.LogWarning("rounding error!");
		}
		
		return new HexCoordinates(iX, iZ);
```

事实上，我们收到了警告。我们如何解决这个问题？它似乎只发生在六边形之间的边缘附近。所以四舍五入坐标会带来麻烦。哪个坐标在错误的方向上四舍五入？好吧，离单元格中心越远，舍入就越多。因此，假设四舍五入最多的坐标是不正确的是有道理的。

然后，解决方案变成丢弃具有最大舍入增量的坐标，并从其他两个坐标重建它。但是因为我们只需要 X 和 Z，所以我们不需要费心重建 Y。

```c#
		if (iX + iY + iZ != 0) {
			float dX = Mathf.Abs(x - iX);
			float dY = Mathf.Abs(y - iY);
			float dZ = Mathf.Abs(-x -y - iZ);

			if (dX > dY && dX > dZ) {
				iX = -iY - iZ;
			}
			else if (dZ > dY) {
				iZ = -iX - iY;
			}
		}
```

### 5.1 给六边形着色

现在我们可以触摸到正确的细胞了，是时候进行一些真正的互动了。让我们改变我们击中的每个单元格的颜色。为 `HexGrid` 提供可配置的默认和触摸单元格颜色。

```c#
	public Color defaultColor = Color.white;
	public Color touchedColor = Color.magenta;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-1/interaction/colors.png)

*单元格颜色选择。*

向 `HexCell` 添加公共颜色字段。

```c#
public class HexCell : MonoBehaviour {

	public HexCoordinates coordinates;

	public Color color;
}
```

在 `HexGrid.CreateCell` 中为其指定默认颜色。

```c#
	void CreateCell (int x, int z, int i) {
		…
		cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
		cell.color = defaultColor;
		…
	}
```

我们还必须向 `HexMesh` 添加颜色信息。

```c#
	List<Color> colors;

	void Awake () {
		…
		vertices = new List<Vector3>();
		colors = new List<Color>();
		…
	}

	public void Triangulate (HexCell[] cells) {
		hexMesh.Clear();
		vertices.Clear();
		colors.Clear();
		…
		hexMesh.vertices = vertices.ToArray();
		hexMesh.colors = colors.ToArray();
		…
	}
```

在三角剖分时，我们现在还必须为每个三角形添加颜色数据。为此添加一个单独的方法。

```c#
	void Triangulate (HexCell cell) {
		Vector3 center = cell.transform.localPosition;
		for (int i = 0; i < 6; i++) {
			AddTriangle(
				center,
				center + HexMetrics.corners[i],
				center + HexMetrics.corners[i + 1]
			);
			AddTriangleColor(cell.color);
		}
	}

	void AddTriangleColor (Color color) {
		colors.Add(color);
		colors.Add(color);
		colors.Add(color);
	}
```

回到 `HexGrid.TouchCell`。首先将单元格坐标转换为适当的数组索引。对于方形网格，这只是宽度的X加Z倍，但在我们的例子中，我们还必须添加半个 Z 偏移。然后抓取单元格，更改其颜色，并再次对网格进行三角剖分。

> **我们真的需要再次对整个网格进行三角剖分吗？**
>
> 我们可能会很聪明，但现在还不是进行此类优化的时候。在未来的教程中，网格将变得更加复杂。现在做出的任何假设和捷径在以后都会失效。这种蛮力方法总是有效的。

```c#
	public void TouchCell (Vector3 position) {
		position = transform.InverseTransformPoint(position);
		HexCoordinates coordinates = HexCoordinates.FromPosition(position);
		int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
		HexCell cell = cells[index];
		cell.color = touchedColor;
		hexMesh.Triangulate(cells);
	}
```

虽然我们现在可以给单元格上色，但我们还没有看到任何视觉变化。这是因为默认着色器不使用顶点颜色。我们必须做自己的。通过*“资源”/“创建”/“着色器”/“默认表面着色器”*创建新的默认着色器。它只需要两个改变。首先，将颜色数据添加到其输入结构中。其次，将反照率（albedo）乘以这个颜色。我们只关心 RGB 通道，因为我们的材质是不透明的。

```glsl
Shader "Custom/VertexColors" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float4 color : COLOR;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb * IN.color;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
```

创建使用此着色器的新材质，然后确保网格网格使用该材质。这将使单元格显示颜色。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-1/interaction/colored-cells.png)

*有色单元格。*

> **我得到奇怪的阴影伪影！**
>
> 在某些 Unity 版本中，自定义曲面着色器可能会遇到阴影问题。如果你得到难看的阴影抖动或条带，那么就会发生 Z 轴斗争。调整平行光的阴影偏置应该足以解决这个问题。

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-1/interaction/interaction.unitypackage)

## 6 地图编辑器

现在我们知道如何编辑颜色了，让我们升级到一个简单的游戏编辑器。此功能不在 `HexGrid` 的范围内，因此将 `TouchCell` 更改为具有额外颜色参数的公共方法。同时删除 `touchedColor` 字段。

```c#
	public void ColorCell (Vector3 position, Color color) {
		position = transform.InverseTransformPoint(position);
		HexCoordinates coordinates = HexCoordinates.FromPosition(position);
		int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
		HexCell cell = cells[index];
		cell.color = color;
		hexMesh.Triangulate(cells);
	}
```

创建一个 `HexMapEditor` 组件，并将 **`Update`** 和 `HandleInput` 方法移动到那里。给它一个公共字段来引用六边形网格、一个颜色数组和一个私有字段来跟踪活动颜色。最后，添加一个公共方法来选择颜色，并确保最初选择第一种颜色。

```c#
using UnityEngine;

public class HexMapEditor : MonoBehaviour {

	public Color[] colors;

	public HexGrid hexGrid;

	private Color activeColor;

	void Awake () {
		SelectColor(0);
	}

	void Update () {
		if (Input.GetMouseButton(0)) {
			HandleInput();
		}
	}

	void HandleInput () {
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(inputRay, out hit)) {
			hexGrid.ColorCell(hit.point, activeColor);
		}
	}

	public void SelectColor (int index) {
		activeColor = colors[index];
	}
}
```

添加另一个画布，这次保留其默认设置。向其中添加一个 `HexMapEditor` 组件，为其添加一些颜色，并连接六边形网格。这次我们确实需要一个事件系统对象，它再次自动创建。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-1/map-editor/canvas.png)

*带有四种颜色的六边形映射编辑器。*

通过 *GameObject / UI / Panel* 在画布上添加一个面板来容纳颜色选择器。通过 *Components / UI/ Toggle Group* 给它一个切换组（toggle group）。把它做成一个小面板，放在屏幕的一个角落里。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-1/map-editor/panel-toggle-group.png)

*带切换组的彩色面板。*

现在，通过 *GameObject / UI / Toggle* 为面板填充每种颜色一个切换。目前，我们并不担心花哨的用户界面，只需要一个看起来足够好的手动设置。

![ui](https://catlikecoding.com/unity/tutorials/hex-map/part-1/map-editor/ui.png) ![hierarchy](https://catlikecoding.com/unity/tutorials/hex-map/part-1/map-editor/hierarchy.png)
*每种颜色一个切换。*

确保只打开第一个切换。还要将它们都作为切换组的一部分，这样在同一时间只会选择其中一个。最后，将它们连接到编辑器的 `SelectColor` 方法。您可以通过 *On Value Changed* 事件 UI 的加号按钮来执行此操作。选择六边形映射编辑器对象，然后从下拉列表中选择正确的方法。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-1/map-editor/toggle.png)

*第一个开关（toggle）。*

此事件提供一个布尔参数，指示每次切换时切换是打开还是关闭。但我们不在乎。相反，我们必须手动提供一个整数参数，该参数对应于我们想要使用的颜色索引。因此，在第一次切换时将其设置为 0，在第二次切换时设置为1，依此类推。

> **何时调用切换事件方法？**
>
> 每次切换的状态发生变化时，它都会调用该方法。如果该方法有一个布尔参数，它将告诉我们切换是打开还是关闭。
>
> 由于我们的切换是一个组的一部分，选择不同的切换将首先关闭当前活动的切换，然后打开所选的切换。这意味着 `SelectColor` 将被调用两次。这没关系，因为第二次调用是我们关心的调用。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-1/map-editor/multi-colored-editing.png)

*用多种颜色画画。*

虽然 UI 功能正常，但有一个恼人的细节。要查看它，请移动面板，使其覆盖六边形网格。选择新颜色时，您还将绘制 UI 下方的单元格。因此，我们同时与 UI 和六边形网格进行交互。这是不可取的。

这可以通过询问事件系统是否检测到光标位于某个对象上方来解决。由于它只知道 UI 对象，这表明我们正在与 UI 交互。因此，只有当情况并非如此时，我们才应该自己处理输入。

```c#
using UnityEngine;
using UnityEngine.EventSystems;
	
	…
	
	void Update () {
		if (
			Input.GetMouseButton(0) &&
			!EventSystem.current.IsPointerOverGameObject()
		) {
			HandleInput();
		}
	}
```

下一个教程是[混合单元格颜色](https://catlikecoding.com/unity/tutorials/hex-map/part-2)。

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-1/map-editor/map-editor.unitypackage)

[PDF](https://catlikecoding.com/unity/tutorials/hex-map/part-1/Hex-Map-1.pdf)



# Hex Map 2：混合单元格颜色

发布于 2016-02-07

https://catlikecoding.com/unity/tutorials/hex-map/part-2/

*连接邻居。*
*在三角形之间插入颜色。*
*创建混合区域。*
*简化几何图形。*



本教程是关于[六边形地图](https://catlikecoding.com/unity/tutorials/hex-map/)系列的第二部分。上一期为我们的网格奠定了基础，并使我们能够编辑单元格。每个单元格都有自己的纯色。单元格之间的颜色变化是突然的。这次我们将引入过渡区，在相邻单元格的颜色之间进行混合。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-2/tutorial-image.png)

*涂抹细胞过渡。*

## 1 单元格邻居

在我们混合单元格颜色之前，我们需要知道哪些细胞彼此相邻。每个单元格有六个邻居，我们可以用指南针方向来识别。方向为东北、东、东南、西南、西和西北。让我们为此创建一个枚举，并将其放入自己的脚本文件中。

```c#
public enum HexDirection {
	NE, E, SE, SW, W, NW
}
```

> **什么是 `enum`？**
>
> 您可以使用 `enum` 定义枚举类型，这是一个有序的名称列表。此类型的变量可以具有以下名称之一作为其值。这些名称中的每一个都对应一个数字，默认情况下从零开始。每当您需要有限的命名选项列表时，它们都很有用。
>
> 在幕后，枚举只是整数。您可以对它们进行加法、减法，并将其转换为整数，然后再转换为整数。你也可以将它们声明为少数其他类型，但整数是常态。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-2/cell-neighbors/directions.png)

*六个邻居，六个方向。*

要存储这些邻居，请向 `HexCell` 添加一个数组。虽然我们可以将其公开，但我们会将其私有化，并使用方向提供访问方法。还要确保它序列化，以便连接在重新编译后仍然存在。

```c#
	[SerializeField]
	HexCell[] neighbors;
```

> **我们需要存储邻居连接吗？**
>
> 我们还可以通过坐标确定邻居，然后从网格中检索所需的单元格。但是，存储每个单元格的关系很简单，所以我们将这样做。

邻居阵列现在显示在检查器中。由于每个单元有六个邻居，因此将我们的*六边形单元*预制件的数组大小设置为 6。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-2/cell-neighbors/prefab.png)

*我们的预制件可容纳六个邻居。*

现在添加一个公共方法来在一个方向上检索单元格的邻居。由于方向始终在 0 和 5 之间，因此我们不需要检查索引是否位于数组的边界内。

```c#
	public HexCell GetNeighbor (HexDirection direction) {
		return neighbors[(int)direction];
	}
```

添加一个方法来设置邻居。

```c#
	public void SetNeighbor (HexDirection direction, HexCell cell) {
		neighbors[(int)direction] = cell;
	}
```

邻居关系是双向的。因此，当在一个方向上设置邻居时，立即将邻居设置在相反的方向上也是有意义的。

```c#
	public void SetNeighbor (HexDirection direction, HexCell cell) {
		neighbors[(int)direction] = cell;
		cell.neighbors[(int)direction.Opposite()] = this;
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-2/cell-neighbors/bidirectional.png)

*邻居在相反的方向。*

当然，这是假设我们可以为它的反面问一个方向。我们可以通过为 `HexDirection` 创建一个扩展方法来支持这一点。要获得相反的方向，请在原始方向上加 3。不过，这只适用于前三个方向，对于其他方向，我们必须减去 3。

```c#
public enum HexDirection {
	NE, E, SE, SW, W, NW
}

public static class HexDirectionExtensions {

	public static HexDirection Opposite (this HexDirection direction) {
		return (int)direction < 3 ? (direction + 3) : (direction - 3);
	}
}
```

> **什么是扩展方法？**
>
> 扩展方法是静态类中的静态方法，其行为类似于某种类型的实例方法。该类型可以是任何东西，类、接口、结构、基元值或枚举。扩展方法的第一个参数需要有 `this` 关键字。它定义了该方法将操作的类型和实例值。
>
> 这是否允许我们为所有内容添加方法？是的，就像你可以编写任何类型作为参数的静态方法一样。这是个好主意吗？如果使用得当，它是可以的。它是一种有用途的工具，但随意使用它会产生一种无序的混乱。

### 1.1 连接邻居

我们可以在 `HexGrid.CreateCell` 中初始化邻居关系。当我们从左到右逐行浏览单元格时，我们知道哪些单元格已经创建。这些是我们可以连接的细胞。

最简单的是 E-W 连接。每行的第一个单元格没有西邻。但这一行中的所有其他细胞都是如此。这些邻居是在我们目前正在使用的细胞之前创建的。因此，我们可以将它们连接起来。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-2/cell-neighbors/neighbors-e-w.png)

*创建单元格时从 E 连接到 W。*

```c#
	void CreateCell (int x, int z, int i) {
		…
		cell.color = defaultColor;

		if (x > 0) {
			cell.SetNeighbor(HexDirection.W, cells[i - 1]);
		}

		Text label = Instantiate<Text>(cellLabelPrefab);
		…
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-2/cell-neighbors/east-west-connected.png)

*东方和西方的邻居是相连的。*

我们还有两个双向连接要建立。由于这些位于不同的网格行之间，我们只能连接前一行。这意味着我们必须完全跳过第一行。

```c#
		if (x > 0) {
			cell.SetNeighbor(HexDirection.W, cells[i - 1]);
		}
		if (z > 0) {
		}
```

由于行是曲折的，因此必须区别对待。让我们先处理偶数行。由于这些行中的所有单元格都有一个 SE 邻居，我们可以连接到这些单元格。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-2/cell-neighbors/neighbors-even-nw-se.png)

*在偶数行上从西北向东南连接。*

```c#
		if (z > 0) {
			if ((z & 1) == 0) {
				cell.SetNeighbor(HexDirection.SE, cells[i - width]);
			}
		}
```

> **`z & 1` 是做什么的？**
>
> `&&` 是布尔 AND 运算符，`&` 是位 AND 运算符。它执行相同的逻辑，但对其操作数的每一对单独的位。因此，一对中的两个比特都需要为 1，结果才能为 1。例如，`10101010 & 00001111` 产生 `00001010`。
>
> 在内部，数字是二进制的。它们只使用 0 和 1。在二进制中，序列 1、2、3、4 被写成 1、10、11、100。正如你所看到的，偶数总是有 0 作为最低有效位。
>
> 我们使用二进制 AND 作为掩码，忽略除第一位之外的所有内容。如果结果为 0，则我们有一个偶数。

我们也可以连接到 SW 邻居。除了每行的第一个单元格，因为它没有一个单元格。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-2/cell-neighbors/neighbors-even-ne-sw.png)

*在偶数行上从 NE 连接到 SW。*

```c#
		if (z > 0) {
			if ((z & 1) == 0) {
				cell.SetNeighbor(HexDirection.SE, cells[i - width]);
				if (x > 0) {
					cell.SetNeighbor(HexDirection.SW, cells[i - width - 1]);
				}
			}
		}
```

奇数行遵循相同的逻辑，但相互呼应。一旦完成，我们电网中的所有邻居都会连接起来。

```c#
		if (z > 0) {
			if ((z & 1) == 0) {
				cell.SetNeighbor(HexDirection.SE, cells[i - width]);
				if (x > 0) {
					cell.SetNeighbor(HexDirection.SW, cells[i - width - 1]);
				}
			}
			else {
				cell.SetNeighbor(HexDirection.SW, cells[i - width]);
				if (x < width - 1) {
					cell.SetNeighbor(HexDirection.SE, cells[i - width + 1]);
				}
			}
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-2/cell-neighbors/all-connected.png)

*所有的邻居都连接在一起。*

当然，并不是每个单元格都与六个邻居相连。构成我们网格边界的单元格最终至少有两个，最多有五个邻居。这是我们必须意识到的。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-2/cell-neighbors/neighbor-count.png)

*每个单元格的邻居。*

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-2/cell-neighbors/cell-neighbors.unitypackage)

## 2 混合颜色

颜色混合将使每个单元格的三角剖分更加复杂。所以，让我们隔离对单个部分进行三角剖分的代码。既然我们现在有了方向，让我们用这些来标识零件，而不是数字索引。

```c#
	void Triangulate (HexCell cell) {
		for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
			Triangulate(d, cell);
		}
	}

	void Triangulate (HexDirection direction, HexCell cell) {
		Vector3 center = cell.transform.localPosition;
		AddTriangle(
			center,
			center + HexMetrics.corners[(int)direction],
			center + HexMetrics.corners[(int)direction + 1]
		);
		AddTriangleColor(cell.color);
	}
```

现在我们使用了方向，如果我们能用方向获取角点，而不必转换为索引，那就太好了。

```c#
		AddTriangle(
			center,
			center + HexMetrics.GetFirstCorner(direction),
			center + HexMetrics.GetSecondCorner(direction)
		);
```

这需要在 `HexMetrics` 中添加两个静态方法。作为奖励，这允许我们将角数组设置为私有。

```c#
	static Vector3[] corners = {
		new Vector3(0f, 0f, outerRadius),
		new Vector3(innerRadius, 0f, 0.5f * outerRadius),
		new Vector3(innerRadius, 0f, -0.5f * outerRadius),
		new Vector3(0f, 0f, -outerRadius),
		new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
		new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
		new Vector3(0f, 0f, outerRadius)
	};

	public static Vector3 GetFirstCorner (HexDirection direction) {
		return corners[(int)direction];
	}

	public static Vector3 GetSecondCorner (HexDirection direction) {
		return corners[(int)direction + 1];
	}
```

### 2.1 每个三角形有多种颜色

现在是 `HexMesh.AddTriangleColor` 方法只有一个颜色参数。这只能产生一个纯色的三角形。让我们添加一个替代方案，为每个顶点提供单独的颜色。

```c#
	void AddTriangleColor (Color c1, Color c2, Color c3) {
		colors.Add(c1);
		colors.Add(c2);
		colors.Add(c3);
	}
```

现在我们可以开始混合颜色了！首先，对其他两个顶点使用邻居的颜色。

```c#
	void Triangulate (HexDirection direction, HexCell cell) {
		Vector3 center = cell.transform.localPosition;
		AddTriangle(
			center,
			center + HexMetrics.GetFirstCorner(direction),
			center + HexMetrics.GetSecondCorner(direction)
		);
		HexCell neighbor = cell.GetNeighbor(direction);
		AddTriangleColor(cell.color, neighbor.color, neighbor.color);
	}
```

不幸的是，这将产生 `NullReferenceException`，因为我们的边界单元格没有六个邻居。当我们缺少邻居时，我们该怎么办？让我们务实一点，用细胞本身作为替代品。

```c#
		HexCell neighbor = cell.GetNeighbor(direction) ?? cell;
```

> **`?? ` 是做什么用的？**
>
> 这被称为空聚（null-coalescing）操作符。简单地说，`a ?? b` 是 `a != null ? a : b` 的较短替代品。
>
> 这里有一些恶作剧，因为Unity在与组件进行比较时会进行自定义工作。这个运算符绕过了这一点，并与 `null` 进行了诚实的比较。但只有当你销毁物体时，这才是一个问题。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-2/blending-colors/direct-neighbor-colors.png)

*颜色混合，但不正确。*

> **坐标标签在哪里？**
>
> 它们仍然存在，但我已经为截图隐藏了 UI 层。

### 2.2 颜色平均

颜色混合是有效的，但我们目前的结果显然是不正确的。六边形边缘的颜色应该是两个相邻单元格的平均值。

```c#
		HexCell neighbor = cell.GetNeighbor(direction) ?? cell;
		Color edgeColor = (cell.color + neighbor.color) * 0.5f;
		AddTriangleColor(cell.color, edgeColor, edgeColor);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-2/blending-colors/edge-average-colors.png)

*沿边缘混合。*

虽然我们现在正在跨边缘混合，但我们仍然会得到清晰的颜色边界。这是因为六边形的每个顶点总共由三个六边形共享。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-2/blending-colors/three-neighbors.png)

*三个邻居，四种颜色。*

这意味着我们还必须考虑前一个和下一个方向的邻居。所以我们最终得到了四种颜色，分为两组，每组三种。

让我们向 `HexDirectionExtensions` 添加两个添加方法，以便轻松跳转到上一个和下一个方向。

```c#
	public static HexDirection Previous (this HexDirection direction) {
		return direction == HexDirection.NE ? HexDirection.NW : (direction - 1);
	}

	public static HexDirection Next (this HexDirection direction) {
		return direction == HexDirection.NW ? HexDirection.NE : (direction + 1);
	}
```

现在我们可以检索所有三个邻居，并执行两次三向混合。

```c#
		HexCell prevNeighbor = cell.GetNeighbor(direction.Previous()) ?? cell;
		HexCell neighbor = cell.GetNeighbor(direction) ?? cell;
		HexCell nextNeighbor = cell.GetNeighbor(direction.Next()) ?? cell;
		
		AddTriangleColor(
			cell.color,
			(cell.color + prevNeighbor.color + neighbor.color) / 3f,
			(cell.color + neighbor.color + nextNeighbor.color) / 3f
		);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-2/blending-colors/corner-average-colors.png)

*在角落里混合。*

这会产生正确的颜色过渡，除了沿着网格的边界。边界单元格对缺失邻居的颜色不一致，所以你仍然可以看到清晰的边界。总的来说，我们目前的方法并没有产生令人满意的结果。我们需要一个更好的战略。

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-2/blending-colors/blending-colors.unitypackage)

## 3 混合区域

在六边形的整个表面上混合会导致模糊的混乱。你再也看不清单个细胞了。我们可以通过仅在六边形边缘附近进行混合来大大改善这一点。这留下了一个内部六边形区域，颜色为纯色。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-2/blend-regions/blend-regions.png)

*实心核心，带有混合区域。*

与混合区域相比，这个实心区域应该有多大？不同的发行版导致不同的视觉效果。我们可以将该区域定义为外半径的一小部分。让我们把它变成 75%。这导致了两个新的指标，加起来是 100%。

```c#
	public const float solidFactor = 0.75f;
	
	public const float blendFactor = 1f - solidFactor;
```

有了新的实体因子，我们可以创建方法来检索实体内六边形的角。

```c#
	public static Vector3 GetFirstSolidCorner (HexDirection direction) {
		return corners[(int)direction] * solidFactor;
	}

	public static Vector3 GetSecondSolidCorner (HexDirection direction) {
		return corners[(int)direction + 1] * solidFactor;
	}
```

现在更改 `HexMesh.Triangulate`，因此它使用这些实心角而不是原始角。暂时保持颜色不变。

```c#
		AddTriangle(
			center,
			center + HexMetrics.GetFirstSolidCorner(direction),
			center + HexMetrics.GetSecondSolidCorner(direction)
		);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-2/blend-regions/solid-hexagons.png)

*实心六边形，无边。*

### 3.1 三角混合区域

我们需要通过缩小三角形来填补我们创造的空白。这个空间在每个方向上都是梯形的。我们可以使用四边形来覆盖它。因此，创建方法来添加四边形及其颜色。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-2/blend-regions/trapezoid.png)

*梯形边缘。*

```c#
	void AddQuad (Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4) {
		int vertexIndex = vertices.Count;
		vertices.Add(v1);
		vertices.Add(v2);
		vertices.Add(v3);
		vertices.Add(v4);
		triangles.Add(vertexIndex);
		triangles.Add(vertexIndex + 2);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 2);
		triangles.Add(vertexIndex + 3);
	}

	void AddQuadColor (Color c1, Color c2, Color c3, Color c4) {
		colors.Add(c1);
		colors.Add(c2);
		colors.Add(c3);
		colors.Add(c4);
	}
```

重新加工 `HexMesh.Triangulate`，使三角形得到单一颜色，四边形在纯色和两种角色之间混合。

```c#
	void Triangulate (HexDirection direction, HexCell cell) {
		Vector3 center = cell.transform.localPosition;
		Vector3 v1 = center + HexMetrics.GetFirstSolidCorner(direction);
		Vector3 v2 = center + HexMetrics.GetSecondSolidCorner(direction);

		AddTriangle(center, v1, v2);
		AddTriangleColor(cell.color);

		Vector3 v3 = center + HexMetrics.GetFirstCorner(direction);
		Vector3 v4 = center + HexMetrics.GetSecondCorner(direction);

		AddQuad(v1, v2, v3, v4);

		HexCell prevNeighbor = cell.GetNeighbor(direction.Previous()) ?? cell;
		HexCell neighbor = cell.GetNeighbor(direction) ?? cell;
		HexCell nextNeighbor = cell.GetNeighbor(direction.Next()) ?? cell;

		AddQuadColor(
			cell.color,
			cell.color,
			(cell.color + prevNeighbor.color + neighbor.color) / 3f,
			(cell.color + neighbor.color + nextNeighbor.color) / 3f
		);
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-2/blend-regions/blend-trapezoids.png)

*与梯形边混合。*

### 3.2 边缘桥

情况开始好转，但我们还没有达到目标。两个邻居之间的颜色混合会被边缘附近的细胞污染。为了防止这种情况，我们必须把梯形的角切掉，把它变成矩形。然后，它在细胞和相邻细胞之间形成一座桥梁，在两侧留下间隙。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-2/blend-regions/edge-bridge.png)

*边缘桥。*

我们可以通过从 `v1` 和 `v2` 开始，然后沿着桥一直移动到单元格的边缘来找到 `v3` 和 `v4` 的新位置。那么，这座桥的偏移量是多少？您可以通过取两个相关角之间的中点，然后应用混合因子来找到它。这是 `HexMetrics` 的工作。

```c#
	public static Vector3 GetBridge (HexDirection direction) {
		return (corners[(int)direction] + corners[(int)direction + 1]) *
			0.5f * blendFactor;
	}
```

回到 `HexMesh`，现在添加一个只需要两种颜色的 `AddQuadColor` 变体是有意义的。

```c#
	void AddQuadColor (Color c1, Color c2) {
		colors.Add(c1);
		colors.Add(c1);
		colors.Add(c2);
		colors.Add(c2);
	}
```

调整三角剖分，使其在邻居之间创建正确的混合桥梁。

```c#
		Vector3 bridge = HexMetrics.GetBridge(direction);
		Vector3 v3 = v1 + bridge;
		Vector3 v4 = v2 + bridge;

		AddQuad(v1, v2, v3, v4);

		HexCell prevNeighbor = cell.GetNeighbor(direction.Previous()) ?? cell;
		HexCell neighbor = cell.GetNeighbor(direction) ?? cell;
		HexCell nextNeighbor = cell.GetNeighbor(direction.Next()) ?? cell;

		AddQuadColor(cell.color, (cell.color + neighbor.color) * 0.5f);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-2/blend-regions/bridges-only.png)

*桥的颜色正确，有角缝。*

### 3.3 填补间隙

现在，无论三个单元格在哪里相遇，我们都会留下一个三角形的间隙。我们通过切掉梯形的三角形边来得到这些洞。让我们把这些三角形加回去。

首先考虑与前一个邻居连接的三角形。它的第一个顶点具有单元格的颜色。它的第二个顶点的颜色是三色混合。最后一个顶点的颜色与桥的一半相同。

```c#
		Color bridgeColor = (cell.color + neighbor.color) * 0.5f;
		AddQuadColor(cell.color, bridgeColor);

		AddTriangle(v1, center + HexMetrics.GetFirstCorner(direction), v3);
		AddTriangleColor(
			cell.color,
			(cell.color + prevNeighbor.color + neighbor.color) / 3f,
			bridgeColor
		);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-2/blend-regions/one-corner.png)

*快到了。*

最后，另一个三角形的工作方式相同，除了它的第二个顶点接触桥，而不是它的第三个顶点。

```c#
		AddTriangle(v2, v4, center + HexMetrics.GetSecondCorner(direction));
		AddTriangleColor(
			cell.color,
			bridgeColor,
			(cell.color + neighbor.color + nextNeighbor.color) / 3f
		);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-2/blend-regions/full-blending.png)

*完全填满。*

现在我们有了很好的混合区域，我们可以给任何我们想要的大小。模糊或清晰的细胞边缘，这取决于你。但您会注意到，网格边界附近的混合仍然不正确。同样，我们将暂时搁置这一点，而是将注意力集中在另一个问题上。

> **但是颜色转换仍然很难看吗？**
>
> 这就是线性颜色混合的极限。事实上，平色并没有那么好。我们将在未来的教程中升级到地形材质，并进行一些更花哨的混合。

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-2/blend-regions/blend-regions.unitypackage)

## 4 熔合边缘

看看我们网格的拓扑结构。有哪些不同的形状？如果我们忽略边界，那么我们可以识别出三种不同的形状类型。有单色六边形、双色矩形和三色三角形。无论三个细胞在哪里相遇，你都会发现这三个形状。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-2/fusing-edges/three-different-shapes.png)

*三种视觉结构。*

因此，每两个六边形由一个矩形桥连接。每三个六边形由一个三角形连接。然而，我们以更复杂的方式进行三角测量。我们目前使用两个四边形来连接一对六边形，而不仅仅是一个。我们用总共六个三角形来连接三个六边形。这似乎有些过分。此外，如果我们要直接与单个形状连接，我们就不需要进行任何颜色平均。因此，我们可以减少复杂性、工作量和三角形。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-2/fusing-edges/many-triangles.png)

*比需要的更复杂。*

> **我们为什么一开始不这样做？**
>
> 你一生中可能会问很多这个问题。那是因为后见之明来得晚。这是一个代码以逻辑方式进化的例子，直到获得了新的见解，从而导致了一种新的方法。这种顿悟往往发生在你认为自己已经完成之后。

### 4.1 直接桥梁

我们的边缘桥现在由两个四边形组成。为了使它们一路穿过下一个六边形，我们必须将桥的长度加倍。这意味着我们不再需要在 `HexMetrics.GetBridge` 中对两个角进行平均。相反，我们只需将它们相加，然后乘以混合因子。

```c#
	public static Vector3 GetBridge (HexDirection direction) {
		return (corners[(int)direction] + corners[(int)direction + 1]) *
			blendFactor;
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-2/fusing-edges/single-bridges.png)

*桥梁一直延伸，并相互重叠。*

这些桥现在形成了六边形之间的直接连接。但我们仍然为每个连接生成两个，每个方向一个，它们重叠。因此，这两个细胞中只有一个需要在它们之间建立桥梁。

让我们首先简化三角剖分代码。删除所有涉及边三角形和颜色混合的内容。然后将添加桥接四边形的代码移动到一个新方法中。将前两个顶点传递给此方法，这样我们就不必再次推导它们。

```c#
	void Triangulate (HexDirection direction, HexCell cell) {
		Vector3 center = cell.transform.localPosition;
		Vector3 v1 = center + HexMetrics.GetFirstSolidCorner(direction);
		Vector3 v2 = center + HexMetrics.GetSecondSolidCorner(direction);

		AddTriangle(center, v1, v2);
		AddTriangleColor(cell.color);

		TriangulateConnection(direction, cell, v1, v2);
	}

	void TriangulateConnection (
		HexDirection direction, HexCell cell, Vector3 v1, Vector3 v2
	) {
		HexCell neighbor = cell.GetNeighbor(direction) ?? cell;
		
		Vector3 bridge = HexMetrics.GetBridge(direction);
		Vector3 v3 = v1 + bridge;
		Vector3 v4 = v2 + bridge;

		AddQuad(v1, v2, v3, v4);
		AddQuadColor(cell.color, neighbor.color);
	}
```

现在我们可以很容易地限制连接的三角剖分。首先，只在处理 NE 连接时添加桥。

```c#
		if (direction == HexDirection.NE) {
			TriangulateConnection(direction, cell, v1, v2);
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-2/fusing-edges/ne-bridges.png)

*只有东北方向的桥梁。*

看起来我们只需在前三个方向上对它们进行三角剖分，就可以覆盖所有连接。所以是 NE、E 和 SE。

```c#
		if (direction <= HexDirection.SE) {
			TriangulateConnection(direction, cell, v1, v2);
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-2/fusing-edges/all-bridges.png)

*所有内部桥梁，一些边境桥梁。*

两个相邻单元格之间的所有连接现在都被覆盖了。但我们也有一些通往网格（grid）之外的桥梁。让我们摆脱这些，当我们最终没有邻居时，通过摆脱三角网连接。因此，我们不再用单元格本身替换缺失的邻居。

```c#
	void TriangulateConnection (
		HexDirection direction, HexCell cell, Vector3 v1, Vector3 v2
	) {
		HexCell neighbor = cell.GetNeighbor(direction);
		if (neighbor == null) {
			return;
		}
		
		…
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-2/fusing-edges/only-internal-bridges.png)

*只有内部桥梁。*

### 4.2 三角形连接

我们必须再次堵住三角孔。让我们对连接到下一个邻居的三角形执行此操作。再一次，只有当邻居确实存在时，我们才应该这样做。

```c#
	void TriangulateConnection (
		HexDirection direction, HexCell cell, Vector3 v1, Vector3 v2
	) {
		…

		HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
		if (nextNeighbor != null) {
			AddTriangle(v2, v4, v2);
			AddTriangleColor(cell.color, neighbor.color, nextNeighbor.color);
		}
	}
```

第三个顶点的位置是什么？我把 `v2` 作为占位符，但这显然是不正确的。当这些三角形的每条边都与一座桥相连时，我们可以通过沿着下一个邻居的桥来找到它。

```c#
			AddTriangle(v2, v4, v2 + HexMetrics.GetBridge(direction.Next()));
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-2/fusing-edges/all-connections.png)

*再一次，一个完整的三角剖分。*

我们完了吗？还没有，因为我们现在正在制作重叠的三角形。因为三个单元共享一个三角形连接，所以我们只需要为两个连接添加它们。所以只有 NE 和 E 可以。

```c#
		if (direction <= HexDirection.E && nextNeighbor != null) {
			AddTriangle(v2, v4, v2 + HexMetrics.GetBridge(direction.Next()));
			AddTriangleColor(cell.color, neighbor.color, nextNeighbor.color);
		}
```

下一个教程是[立面和阶地](https://catlikecoding.com/unity/tutorials/hex-map/part-3)。

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-2/fusing-edges/fusing-edges.unitypackage)

[PDF](https://catlikecoding.com/unity/tutorials/hex-map/part-2/Hex-Map-2.pdf)

# Hex Map 3：立面（Elevation）

https://catlikecoding.com/unity/tutorials/hex-map/part-3/

*为单元格添加标高（elevation）。*
*三角形斜坡。*
*插入阶地。*
*合并阶地和悬崖。*



本教程是关于[六边形地图](https://catlikecoding.com/unity/tutorials/hex-map/)系列的第三部分。这一次，我们将添加对不同标高（elevation）的支持，并在它们之间创建特殊的过渡。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-3/tutorial-image.png)

*立面和阶地。*

## 1 单元格高度

我们将地图划分为离散的单元格，以覆盖平坦的区域。现在，我们也将为每个单元格设置自己的标高。我们将使用离散标高，因此将其存储在 `HexCell` 中的整数字段中。

```c#
	public int elevation;
```

每个连续的升高台阶应该有多高？我们可以使用任何值，所以让我们将其定义为另一个 `HexMetrics` 常量。我将每一步使用五个单位，这会产生非常明显的过渡。对于实际的游戏，我可能会使用较小的步长。

```c#
	public const float elevationStep = 5f;
```

### 1.1 编辑单元格

到目前为止，我们只能编辑单元格的颜色，但现在我们也可以更改其标高。因此，`HexGrid.ColorCell` 方法已不再足够。此外，我们稍后可能会为每个单元格添加更多可编辑的选项。这需要一种新的编辑方法。

将 `ColorCell` 重命名为 `GetCell`，并使其在给定位置返回单元格，而不是设置其颜色。由于它现在不再改变任何东西，我们也不应该再立即对单元格进行三角剖分。

```c#
	public HexCell GetCell (Vector3 position) {
		position = transform.InverseTransformPoint(position);
		HexCoordinates coordinates = HexCoordinates.FromPosition(position);
		int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
		return cells[index];
	}
```

现在由编辑器来调整单元格。完成后，需要再次对网格进行三角剖分。添加一个公共 `HexGrid.Refresh` 方法来解决这个问题。

```c#
	public void Refresh () {
		hexMesh.Triangulate(cells);
	}
```

更改 `HexMapEditor`，使其与新方法一起工作。为其提供一个新的 `EditCell` 方法，该方法负责单元格的所有编辑，然后刷新网格。

```c#
	void HandleInput () {
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(inputRay, out hit)) {
			EditCell(hexGrid.GetCell(hit.point));
		}
	}

	void EditCell (HexCell cell) {
		cell.color = activeColor;
		hexGrid.Refresh();
	}
```

我们可以通过简单地将选定的标高指定给我们正在编辑的单元格来调整标高。

```c#
	int activeElevation;

	void EditCell (HexCell cell) {
		cell.color = activeColor;
		cell.elevation = activeElevation;
		hexGrid.Refresh();
	}
```

就像颜色一样，我们需要一种方法来设置活动标高，我们将链接到 UI。我们将使用滑块从标高范围中进行选择。由于滑块使用浮点数，我们的方法需要一个浮点数参数。我们只需将其转换为整数。

```c#
	public void SetElevation (float elevation) {
		activeElevation = (int)elevation;
	}
```

通过*游戏对象/创建/滑块*将滑块添加到画布上，并将其放置在颜色面板下方。将其设置为从下到上的垂直滑块，以便在视觉上与标高相匹配。将其限制为整数，并给它一个合理的范围，比如从 0 到 6。然后将其 *On Value Changed* 事件挂钩到我们的 *Hex Map Editor* 对象的 `SetElevation` 方法。确保从动态列表中选择该方法，以便使用滑块的值调用它。

![ui](https://catlikecoding.com/unity/tutorials/hex-map/part-3/cell-elevation/slider-ui.png) ![component](https://catlikecoding.com/unity/tutorials/hex-map/part-3/cell-elevation/slider-component.png)

*标高滑块。*

### 1.2 可视化立面

编辑单元格时，我们现在正在设置其颜色和标高。虽然您可以检查检查器以查看标高是否确实发生了变化，但三角测量过程仍然会忽略它。

我们所需要做的就是在单元格的标高发生变化时调整单元格的垂直局部位置。为了方便起见，让我们将 `HexCell.elevation` 设置为私有，并添加一个公共 `HexCell.Elevation` 属性。

```c#
	public int Elevation {
		get {
			return elevation;
		}
		set {
			elevation = value;
		}
	}
	
	int elevation;
```

现在，每当编辑单元格的标高时，我们都可以调整单元格的垂直位置。

```c#
		set {
			elevation = value;
			Vector3 position = transform.localPosition;
			position.y = value * HexMetrics.elevationStep;
			transform.localPosition = position;
		}
```

当然，这需要在 `HexMapEditor.EditCell` 中稍作调整。

```c#
	void EditCell (HexCell cell) {
		cell.color = activeColor;
		cell.Elevation = activeElevation;
		hexGrid.Refresh();
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-3/cell-elevation/visualization.png)

*不同高度的单元格。*

> **网格碰撞体是否会调整以匹配新的标高？**
>
> 旧版本的 Unity 要求在再次分配相同的网格之前将网格碰撞器设置为 null。它只是假设网格不会改变，因此只有不同的网格（或 null）触发了碰撞体刷新。这已经没有必要了。因此，我们目前的方法——在三角剖分后将网格重新分配给碰撞体——就足够了。

现在可以看到单元格升高，但有两个问题。首先，单元格标签在升高的单元格下方消失。其次，单元格之间的连接忽略了标高。让我们解决这个问题。

### 1.3 重新定位单元格标签

目前，单元格的 UI 标签只创建和定位一次，然后就被遗忘了。为了更新它们的垂直位置，我们必须跟踪它们。让我们给每个 `HexCell` 一个对其 UI 标签的 `RectTransform` 的引用，以便以后可以更新。

```c#
	public RectTransform uiRect;
```

在 `HexGrid.CreateCell` 的末尾分配它们。

```c#
	void CreateCell (int x, int z, int i) {
		…
		cell.uiRect = label.rectTransform;
	}
```

现在我们可以扩展 `HexCell.Elevation` 属性还可以调整其单元格 UI 的位置。因为旋转了六边形网格画布，所以标签必须在负 Z 方向上移动，而不是在正 Y 方向上移动。

```c#
		set {
			elevation = value;
			Vector3 position = transform.localPosition;
			position.y = value * HexMetrics.elevationStep;
			transform.localPosition = position;

			Vector3 uiPosition = uiRect.localPosition;
			uiPosition.z = elevation * -HexMetrics.elevationStep;
			uiRect.localPosition = uiPosition;
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-3/cell-elevation/elevated-labels.png)

*提升标签。*

### 1.4 创建坡度

接下来，我们必须将平面单元连接转换为斜率。这是在 `HexMesh.TriangulateConnection` 中完成的。在边缘连接的情况下，我们必须覆盖桥另一端的高度。

```c#
		Vector3 bridge = HexMetrics.GetBridge(direction);
		Vector3 v3 = v1 + bridge;
		Vector3 v4 = v2 + bridge;
		v3.y = v4.y = neighbor.Elevation * HexMetrics.elevationStep;
```

在拐角连接的情况下，我们必须对通往下一个邻居的桥做同样的事情。

```c#
		if (direction <= HexDirection.E && nextNeighbor != null) {
			Vector3 v5 = v2 + HexMetrics.GetBridge(direction.Next());
			v5.y = nextNeighbor.Elevation * HexMetrics.elevationStep;
			AddTriangle(v2, v4, v5);
			AddTriangleColor(cell.color, neighbor.color, nextNeighbor.color);
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-3/cell-elevation/elevated-connections.png)

*高架连接。*

我们现在支持不同高度的单元格，它们之间有正确倾斜的连接。但我们不要止步于此。我们要让这些斜坡更有趣。

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-3/cell-elevation/cell-elevation.unitypackage)

## 2 阶梯式边缘连接

直坡看起来没那么有趣。我们可以通过增加梯田将它们分成多个台阶。《无尽传奇（Endless Legend）》就是这样一款游戏。

例如，我们可以在每个斜坡上插入两个梯田。因此，一个大斜坡变成了三个小斜坡，中间有两个平坦区域。为了进行三角剖分，我们必须将每个连接分为五个步骤。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-3/terraced-edge-connections/terraces.png)

*斜坡上的两个梯田。*

我们可以在 `HexMetrics` 中定义每个坡度的梯田数量，并从中推导出台阶数量。

```c#
	public const int terracesPerSlope = 2;

	public const int terraceSteps = terracesPerSlope * 2 + 1;
```

理想情况下，我们可以简单地沿斜坡插值每一步。这并非完全微不足道，因为Y坐标只能在奇数步上变化，而不能在偶数步上变化。否则，我们就不会有平坦的露台。让我们在 `HexMetrics` 中添加一种特殊的插值方法来解决这个问题。

```c#
	public static Vector3 TerraceLerp (Vector3 a, Vector3 b, int step) {
		return a;
	}
```

如果我们知道插值步长是多少，水平插值就很简单了。

```c#
	public const float horizontalTerraceStepSize = 1f / terraceSteps;
	
	public static Vector3 TerraceLerp (Vector3 a, Vector3 b, int step) {
		float h = step * HexMetrics.horizontalTerraceStepSize;
		a.x += (b.x - a.x) * h;
		a.z += (b.z - a.z) * h;
		return a;
	}
```

> **两个值之间的插值是如何工作的？**
>
> 两个值之间的插值 $a$ 以及 $b$ 使用第三个插值器 $t$ 完成。如果 $t$ 为 0，则结果为 $a$。当它为 1 时，结果为 $b$. 当 $t$ 位于 0 和 1 之间的某个位置，$a$ 以及 $b$ 按比例混合。因此，插值结果的公式为 $(1 - t)a + tb$
>
> 注意到 $(1-t)a + tb = a - ta + tb = a + t(b - a)$。第三种形式将插值描述为一个从 $a$ 到 $b$ 沿着矢量 $(b - a)$ 的移动。它还需要少一次乘法来计算。

为了只在奇数步上调整 Y，我们可以使用 $\frac{step+1}2$。如果我们使用整数除法，它将把序列 1, 2, 3, 4 转换为 1, 1, 2, 2。

```c#
	public const float verticalTerraceStepSize = 1f / (terracesPerSlope + 1);
	
	public static Vector3 TerraceLerp (Vector3 a, Vector3 b, int step) {
		float h = step * HexMetrics.horizontalTerraceStepSize;
		a.x += (b.x - a.x) * h;
		a.z += (b.z - a.z) * h;
		float v = ((step + 1) / 2) * HexMetrics.verticalTerraceStepSize;
		a.y += (b.y - a.y) * v;
		return a;
	}
```

让我们也为颜色添加一种露台插值方法。只需插入，就像连接是平的一样。

```c#
	public static Color TerraceLerp (Color a, Color b, int step) {
		float h = step * HexMetrics.horizontalTerraceStepSize;
		return Color.Lerp(a, b, h);
	}
```

### 2.1 三角剖分

由于三角剖分边缘连接将变得更加复杂，请从 `HexMesh.TriangulateConnection` 中提取相关代码。三角连接并将其放入单独的方法中。我也会在注释中保留原始代码，以供以后参考。

```c#
	void TriangulateConnection (
		HexDirection direction, HexCell cell, Vector3 v1, Vector3 v2
	) {
		…
		Vector3 bridge = HexMetrics.GetBridge(direction);
		Vector3 v3 = v1 + bridge;
		Vector3 v4 = v2 + bridge;
		v3.y = v4.y = neighbor.Elevation * HexMetrics.elevationStep;

		TriangulateEdgeTerraces(v1, v2, cell, v3, v4, neighbor);
//		AddQuad(v1, v2, v3, v4);
//		AddQuadColor(cell.color, neighbor.color);
		…
	}

	void TriangulateEdgeTerraces (
		Vector3 beginLeft, Vector3 beginRight, HexCell beginCell,
		Vector3 endLeft, Vector3 endRight, HexCell endCell
	) {
		AddQuad(beginLeft, beginRight, endLeft, endRight);
		AddQuadColor(beginCell.color, endCell.color);
	}
```

让我们从这个过程的第一步开始。使用我们的特殊插值方法创建第一个四边形。这应该会产生一个比原始坡度更陡的短坡度。

```c#
	void TriangulateEdgeTerraces (
		Vector3 beginLeft, Vector3 beginRight, HexCell beginCell,
		Vector3 endLeft, Vector3 endRight, HexCell endCell
	) {
		Vector3 v3 = HexMetrics.TerraceLerp(beginLeft, endLeft, 1);
		Vector3 v4 = HexMetrics.TerraceLerp(beginRight, endRight, 1);
		Color c2 = HexMetrics.TerraceLerp(beginCell.color, endCell.color, 1);

		AddQuad(beginLeft, beginRight, v3, v4);
		AddQuadColor(beginCell.color, c2);
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-3/terraced-edge-connections/first-step.png)

*第一个露台台阶。*

现在立即跳到最后一步，跳过中间的所有内容。这将完成我们的边缘连接，尽管还没有形成正确的形状。

```c#
		AddQuad(beginLeft, beginRight, v3, v4);
		AddQuadColor(beginCell.color, c2);

		AddQuad(v3, v4, endLeft, endRight);
		AddQuadColor(c2, endCell.color);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-3/terraced-edge-connections/last-step.png)

*最后一个露台台阶。*

中间步骤可以添加一个循环。每一步，前面的最后两个顶点都会变成新的前两个顶点。颜色也是如此。然后计算新的向量和颜色，并添加另一个四边形。

```c#
		AddQuad(beginLeft, beginRight, v3, v4);
		AddQuadColor(beginCell.color, c2);

		for (int i = 2; i < HexMetrics.terraceSteps; i++) {
			Vector3 v1 = v3;
			Vector3 v2 = v4;
			Color c1 = c2;
			v3 = HexMetrics.TerraceLerp(beginLeft, endLeft, i);
			v4 = HexMetrics.TerraceLerp(beginRight, endRight, i);
			c2 = HexMetrics.TerraceLerp(beginCell.color, endCell.color, i);
			AddQuad(v1, v2, v3, v4);
			AddQuadColor(c1, c2);
		}

		AddQuad(v3, v4, endLeft, endRight);
		AddQuadColor(c2, endCell.color);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-3/terraced-edge-connections/all-terraces.png)

*中间的所有台阶。*

现在，所有边缘连接都有两个阶地，或者您选择将 `HexMetrics.terracesPerSlope` 设置为多少阶地。当然，我们还没有对角连接进行阶地处理。我们稍后再谈。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-3/terraced-edge-connections/always-terraces.png)

*所有边缘连接都是阶梯状的。*

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-3/terraced-edge-connections/terraced-edge-connections.unitypackage)

## 3 连接类型

将所有边缘连接转换为梯田可能不是一个好主意。当高差只有一个水平时，它看起来很好。但更大的差异会产生狭窄的梯田，梯田之间有很大的跳跃，这看起来没那么好。此外，扁平连接根本不需要呈阶梯状。

让我们将其形式化，并定义三种边类型。平坦、斜坡和悬崖。为此创建一个新的枚举。

```c#
public enum HexEdgeType {
	Flat, Slope, Cliff
}
```

我们如何确定我们正在处理什么样的连接？我们可以在 `HexMetrics` 中添加一种方法，基于两个标高进行推导。

```c#
	public static HexEdgeType GetEdgeType (int elevation1, int elevation2) {
	}
```

如果高度相同，我们就有一个平边。

```c#
	public static HexEdgeType GetEdgeType (int elevation1, int elevation2) {
		if (elevation1 == elevation2) {
			return HexEdgeType.Flat;
		}
	}
```

如果水平差恰好是一步，那么我们就有一个斜率。斜坡是向上还是向下并不重要。在所有其他情况下，我们都有悬崖。

```c#
	public static HexEdgeType GetEdgeType (int elevation1, int elevation2) {
		if (elevation1 == elevation2) {
			return HexEdgeType.Flat;
		}
		int delta = elevation2 - elevation1;
		if (delta == 1 || delta == -1) {
			return HexEdgeType.Slope;
		}
		return HexEdgeType.Cliff;
	}
```

让我们还添加一个方便的 `HexCell.GetEdgeType` 方法用于获取单元格在特定方向上的边类型。

```c#
	public HexEdgeType GetEdgeType (HexDirection direction) {
		return HexMetrics.GetEdgeType(
			elevation, neighbors[(int)direction].elevation
		);
	}
```

> **我们难道不应该检查一下那个方向上是否真的有邻居吗？**
>
> 您最终可能会在恰好位于地图边界的方向上请求边缘类型。在这种情况下，没有邻居，我们会得到一个 `NullReferenceException`。我们可以在方法内部检查这一点，如果是这样，我们就必须抛出某种异常。但这已经发生了，所以没有必要明确地这样做。也就是说，除非你想抛出一个自定义异常。
>
> 请注意，只有当我们已经知道我们没有处理边界边时，我们才会使用这种方法。如果我们在某个地方犯了错误，我们将得到 `NullReferenceException`。

### 3.1 将梯田限制在斜坡上

现在我们可以确定我们正在处理的连接类型，我们可以决定是否插入梯田。调整 `HexMesh.TriangulateConnection` ，因此它只为斜坡创建梯田。

```c#
		if (cell.GetEdgeType(direction) == HexEdgeType.Slope) {
			TriangulateEdgeTerraces(v1, v2, cell, v3, v4, neighbor);
		}
//		AddQuad(v1, v2, v3, v4);
//		AddQuadColor(cell.color, neighbor.color);
```

此时，我们可以重新激活之前注释掉的代码，以处理平地和悬崖。

```c#
		if (cell.GetEdgeType(direction) == HexEdgeType.Slope) {
			TriangulateEdgeTerraces(v1, v2, cell, v3, v4, neighbor);
		}
		else {
			AddQuad(v1, v2, v3, v4);
			AddQuadColor(cell.color, neighbor.color);
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-3/connection-types/slope-terraces.png)

*只有斜坡是梯田。*

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-3/connection-types/connection-types.unitypackage)

## 4 阶梯式转角连接

角连接比边连接更复杂，因为它们涉及三个单元，而不仅仅是两个单元。每个角都连接到三条边，可以是平坦的、斜坡的或悬崖的。因此，有许多可能的配置。与边缘连接一样，我们最好在 `HexMesh` 中添加一种新的三角剖分方法。

我们的新方法需要角三角形的顶点和连通的单元。为了使事情易于管理，让我们对连接进行排序，这样我们就知道哪个单元的高度最低。然后我们可以从下到左再到右工作。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-3/terraced-corner-connections/triangle.png)

*转角连接。*

```c#
	void TriangulateCorner (
		Vector3 bottom, HexCell bottomCell,
		Vector3 left, HexCell leftCell,
		Vector3 right, HexCell rightCell
	) {
		AddTriangle(bottom, left, right);
		AddTriangleColor(bottomCell.color, leftCell.color, rightCell.color);
	}
```

现在，`TriangulateConnection` 必须找出最低的单元格是什么。首先，检查被三角化的单元格是否低于其邻居，或者是否与最低单元格绑定。如果是这样，我们可以将其用作底部单元格。

```c#
	void TriangulateConnection (
		HexDirection direction, HexCell cell, Vector3 v1, Vector3 v2
	) {
		…
		
		HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
		if (direction <= HexDirection.E && nextNeighbor != null) {
			Vector3 v5 = v2 + HexMetrics.GetBridge(direction.Next());
			v5.y = nextNeighbor.Elevation * HexMetrics.elevationStep;
			
			if (cell.Elevation <= neighbor.Elevation) {
				if (cell.Elevation <= nextNeighbor.Elevation) {
					TriangulateCorner(v2, cell, v4, neighbor, v5, nextNeighbor);
				}
			}
		}
	}
```

如果最内层的检查失败，则意味着下一个邻居是最低的单元。我们必须逆时针旋转三角形，以保持其正确的方向。

```c#
			if (cell.Elevation <= neighbor.Elevation) {
				if (cell.Elevation <= nextNeighbor.Elevation) {
					TriangulateCorner(v2, cell, v4, neighbor, v5, nextNeighbor);
				}
				else {
					TriangulateCorner(v5, nextNeighbor, v2, cell, v4, neighbor);
				}
			}
```

如果第一次检查已经失败，它就变成了两个相邻单元格之间的竞争。如果边邻居是最低的，那么我们必须顺时针旋转，否则逆时针旋转。

```c#
			if (cell.Elevation <= neighbor.Elevation) {
				if (cell.Elevation <= nextNeighbor.Elevation) {
					TriangulateCorner(v2, cell, v4, neighbor, v5, nextNeighbor);
				}
				else {
					TriangulateCorner(v5, nextNeighbor, v2, cell, v4, neighbor);
				}
			}
			else if (neighbor.Elevation <= nextNeighbor.Elevation) {
				TriangulateCorner(v4, neighbor, v5, nextNeighbor, v2, cell);
			}
			else {
				TriangulateCorner(v5, nextNeighbor, v2, cell, v4, neighbor);
			}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-3/terraced-corner-connections/triangle-orientations.png)

*逆时针、否和顺时针旋转。*

### 4.1 斜坡三角剖分

要知道如何对一个角进行三角剖分，我们必须知道我们处理的是什么类型的边。为了方便起见，让我们在 `HexCell` 中添加另一种方便的方法来确定任意两个单元格之间的斜率。

```c#
	public HexEdgeType GetEdgeType (HexCell otherCell) {
		return HexMetrics.GetEdgeType(
			elevation, otherCell.elevation
		);
	}
```

在 `HexMesh.TriangulateCorner` 中使用此新方法，用于确定左右边的类型。

```c#
	void TriangulateCorner (
		Vector3 bottom, HexCell bottomCell,
		Vector3 left, HexCell leftCell,
		Vector3 right, HexCell rightCell
	) {
		HexEdgeType leftEdgeType = bottomCell.GetEdgeType(leftCell);
		HexEdgeType rightEdgeType = bottomCell.GetEdgeType(rightCell);

		AddTriangle(bottom, left, right);
		AddTriangleColor(bottomCell.color, leftCell.color, rightCell.color);
	}
```

如果两边都是斜坡，那么我们在左边和右边都有梯田。此外，由于底部单元格最低，我们知道这些斜率会上升。此外，这意味着左右单元格具有相同的标高，因此顶部边缘连接是平的。我们可以将这种情况识别为斜坡-斜坡-平坦（slope-slope-flat），简称 SSF。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-3/terraced-corner-connections/ssf.png)

*两个斜坡和一个平坦的 SSF*

检查我们是否处于这种情况，如果是这样，请调用一个新方法 `TriangulateCornerTerraces`。之后，从该方法返回。将此复选框放在旧三角测量代码之前，这样它将替换默认三角形。

```c#
	void TriangulateCorner (
		Vector3 bottom, HexCell bottomCell,
		Vector3 left, HexCell leftCell,
		Vector3 right, HexCell rightCell
	) {
		HexEdgeType leftEdgeType = bottomCell.GetEdgeType(leftCell);
		HexEdgeType rightEdgeType = bottomCell.GetEdgeType(rightCell);

		if (leftEdgeType == HexEdgeType.Slope) {
			if (rightEdgeType == HexEdgeType.Slope) {
				TriangulateCornerTerraces(
					bottom, bottomCell, left, leftCell, right, rightCell
				);
				return;
			}
		}

		AddTriangle(bottom, left, right);
		AddTriangleColor(bottomCell.color, leftCell.color, rightCell.color);
	}
	
	void TriangulateCornerTerraces (
		Vector3 begin, HexCell beginCell,
		Vector3 left, HexCell leftCell,
		Vector3 right, HexCell rightCell
	) {
		
	}
```

只要我们不在 `TriangulateCornerTerraces` 内部做任何事情，一些双坡角连接就会变成洞。一个单元格是否会变成一个洞取决于哪个单元格最终会成为底部单元格。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-3/terraced-corner-connections/hole.png)

*一个洞出现了。*

为了填补这个洞，我们必须把左右梯田连接起来。该方法与边缘连接相同，但位于三色三角形而不是双色四边形内。让我们再次从第一步开始，现在是一个三角形。

```c#
	void TriangulateCornerTerraces (
		Vector3 begin, HexCell beginCell,
		Vector3 left, HexCell leftCell,
		Vector3 right, HexCell rightCell
	) {
		Vector3 v3 = HexMetrics.TerraceLerp(begin, left, 1);
		Vector3 v4 = HexMetrics.TerraceLerp(begin, right, 1);
		Color c3 = HexMetrics.TerraceLerp(beginCell.color, leftCell.color, 1);
		Color c4 = HexMetrics.TerraceLerp(beginCell.color, rightCell.color, 1);

		AddTriangle(begin, v3, v4);
		AddTriangleColor(beginCell.color, c3, c4);
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-3/terraced-corner-connections/first-step.png)

*第一个三角形台阶。*

再次直接跳到最后一步。它是一个四边形，形成一个梯形。与边缘连接的唯一区别是，我们在这里处理的是四种不同的颜色，而不仅仅是两种。

```c#
		AddTriangle(begin, v3, v4);
		AddTriangleColor(beginCell.color, c3, c4);

		AddQuad(v3, v4, left, right);
		AddQuadColor(c3, c4, leftCell.color, rightCell.color);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-3/terraced-corner-connections/last-step.png)

*最后四步。*

中间的所有台阶也都是四边形。

```c#
		AddTriangle(begin, v3, v4);
		AddTriangleColor(beginCell.color, c3, c4);

		for (int i = 2; i < HexMetrics.terraceSteps; i++) {
			Vector3 v1 = v3;
			Vector3 v2 = v4;
			Color c1 = c3;
			Color c2 = c4;
			v3 = HexMetrics.TerraceLerp(begin, left, i);
			v4 = HexMetrics.TerraceLerp(begin, right, i);
			c3 = HexMetrics.TerraceLerp(beginCell.color, leftCell.color, i);
			c4 = HexMetrics.TerraceLerp(beginCell.color, rightCell.color, i);
			AddQuad(v1, v2, v3, v4);
			AddQuadColor(c1, c2, c3, c4);
		}

		AddQuad(v3, v4, left, right);
		AddQuadColor(c3, c4, leftCell.color, rightCell.color);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-3/terraced-corner-connections/all-steps.png)

*所有台阶。*

### 4.2 双斜坡变体

双斜坡情况有两个不同方向的变体，具体取决于哪个单元格最终成为底部单元格。我们可以通过检查左右组合斜坡平坦和平坡来找到它们。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-3/terraced-corner-connections/sfs-fss.png)

*SFS 和 FSS。*

如果右边是平的，那么我们必须从左边而不是底部开始梯田。如果左边是平的，那么我们必须从右边开始。

```c#
		if (leftEdgeType == HexEdgeType.Slope) {
			if (rightEdgeType == HexEdgeType.Slope) {
				TriangulateCornerTerraces(
					bottom, bottomCell, left, leftCell, right, rightCell
				);
				return;
			}
			if (rightEdgeType == HexEdgeType.Flat) {
				TriangulateCornerTerraces(
					left, leftCell, right, rightCell, bottom, bottomCell
				);
				return;
			}
		}
		if (rightEdgeType == HexEdgeType.Slope) {
			if (leftEdgeType == HexEdgeType.Flat) {
				TriangulateCornerTerraces(
					right, rightCell, bottom, bottomCell, left, leftCell
				);
				return;
			}
		}
```

这将使我们的梯田在细胞周围不受干扰地流动，直到它们遇到悬崖或地图的尽头。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-3/terraced-corner-connections/all-simple-slopes.png)

*连续的梯田。*

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-3/terraced-corner-connections/terraced-corner-connections.unitypackage)

## 5 合并斜坡和悬崖

那么，当斜坡遇到悬崖时呢？如果我们知道左边是斜坡，右边是悬崖，那么上边会是什么？它不可能是平坦的，但也可能是斜坡或悬崖。

![two slopes](https://catlikecoding.com/unity/tutorials/hex-map/part-3/merging-slopes-and-cliffs/two-slopes-one-cliff.png) ![two cliffs](https://catlikecoding.com/unity/tutorials/hex-map/part-3/merging-slopes-and-cliffs/one-slope-two-cliffs.png) ![diagram](https://catlikecoding.com/unity/tutorials/hex-map/part-3/merging-slopes-and-cliffs/scs-scc.png)

*SCS 和 SCC。*

让我们添加一种新方法来同时处理两种斜坡悬崖情况。

```c#
	void TriangulateCornerTerracesCliff (
		Vector3 begin, HexCell beginCell,
		Vector3 left, HexCell leftCell,
		Vector3 right, HexCell rightCell
	) {
		
	}
```

当左边缘是斜坡时，必须将其作为 `TriangulateCorner` 中的最终选项调用。

```c#
		if (leftEdgeType == HexEdgeType.Slope) {
			if (rightEdgeType == HexEdgeType.Slope) {
				TriangulateCornerTerraces(
					bottom, bottomCell, left, leftCell, right, rightCell
				);
				return;
			}
			if (rightEdgeType == HexEdgeType.Flat) {
				TriangulateCornerTerraces(
					left, leftCell, right, rightCell, bottom, bottomCell
				);
				return;
			}
			TriangulateCornerTerracesCliff(
				bottom, bottomCell, left, leftCell, right, rightCell
			);
			return;
		}
		if (rightEdgeType == HexEdgeType.Slope) {
			if (leftEdgeType == HexEdgeType.Flat) {
				TriangulateCornerTerraces(
					right, rightCell, bottom, bottomCell, left, leftCell
				);
				return;
			}
		}
```

那么，我们如何对其进行三角剖分呢？这个问题可以分为两部分，底部和顶部。

### 5.1 底部

底部左侧有梯田，右侧有悬崖。我们必须以某种方式将它们合并。一个简单的方法是折叠梯田，让它们在右角相遇。这将使梯田逐渐向上变细。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-3/merging-slopes-and-cliffs/collapsing-terraces.png)

*坍塌的梯田。*

但我们实际上不想让它们在右角相遇，因为这会干扰顶部可能存在的梯田。此外，我们可能要处理一个非常高的悬崖，这会导致非常陡峭和薄的三角形。相反，我们将它们折叠到悬崖边的一个边界点。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-3/merging-slopes-and-cliffs/collapsing-at-boundary.png)

*在边界处坍塌。*

让我们将此边界点放置在底部单元格上方一个标高处。我们可以通过基于高程差的插值来找到它。

```c#
	void TriangulateCornerTerracesCliff (
		Vector3 begin, HexCell beginCell,
		Vector3 left, HexCell leftCell,
		Vector3 right, HexCell rightCell
	) {
		float b = 1f / (rightCell.Elevation - beginCell.Elevation);
		Vector3 boundary = Vector3.Lerp(begin, right, b);
		Color boundaryColor = Color.Lerp(beginCell.color, rightCell.color, b);
	}
```

为了验证我们是否正确，用一个三角形覆盖整个底部。

```c#
		float b = 1f / (rightCell.Elevation - beginCell.Elevation);
		Vector3 boundary = Vector3.Lerp(begin, right, b);
		Color boundaryColor = Color.Lerp(beginCell.color, rightCell.color, b);

		AddTriangle(begin, left, boundary);
		AddTriangleColor(beginCell.color, leftCell.color, boundaryColor);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-3/merging-slopes-and-cliffs/lower-triangle.png)

*下三角。*

有了正确的边界，我们就可以继续对梯田进行三角剖分。让我们再次从第一步开始。

```c#
		float b = 1f / (rightCell.Elevation - beginCell.Elevation);
		Vector3 boundary = Vector3.Lerp(begin, right, b);
		Color boundaryColor = Color.Lerp(beginCell.color, rightCell.color, b);

		Vector3 v2 = HexMetrics.TerraceLerp(begin, left, 1);
		Color c2 = HexMetrics.TerraceLerp(beginCell.color, leftCell.color, 1);

		AddTriangle(begin, v2, boundary);
		AddTriangleColor(beginCell.color, c2, boundaryColor);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-3/merging-slopes-and-cliffs/first-step.png)

*第一个坍缩台阶。*

这一次，最后一步也是三角形。

```c#
		AddTriangle(begin, v2, boundary);
		AddTriangleColor(beginCell.color, c2, boundaryColor);

		AddTriangle(v2, left, boundary);
		AddTriangleColor(c2, leftCell.color, boundaryColor);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-3/merging-slopes-and-cliffs/last-step.png)

*最后一个坍缩台阶。*

中间的所有台阶也是三角形。

```c#
		AddTriangle(begin, v2, boundary);
		AddTriangleColor(beginCell.color, c2, boundaryColor);

		for (int i = 2; i < HexMetrics.terraceSteps; i++) {
			Vector3 v1 = v2;
			Color c1 = c2;
			v2 = HexMetrics.TerraceLerp(begin, left, i);
			c2 = HexMetrics.TerraceLerp(beginCell.color, leftCell.color, i);
			AddTriangle(v1, v2, boundary);
			AddTriangleColor(c1, c2, boundaryColor);
		}

		AddTriangle(v2, left, boundary);
		AddTriangleColor(c2, leftCell.color, boundaryColor);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-3/merging-slopes-and-cliffs/all-steps.png)

*坍塌的梯田。*

> **我们不能把梯田保持水平吗？**
>
> 我们确实可以通过在起点和边界点之间插值来保持梯田平坦，而不是总是使用边界点。这将需要在梯田之间的斜坡上使用四边形。然而，这些四边形不会位于平面内，因为它们的左侧和右侧不会有相同的斜率。结果看起来会很混乱。

### 5.2 完成角落

底部完成后，我们可以看看顶部。如果顶部边缘是斜坡，我们还需要连接梯田和悬崖。因此，让我们将该代码移动到它自己的方法中。

```c#
	void TriangulateCornerTerracesCliff (
		Vector3 begin, HexCell beginCell,
		Vector3 left, HexCell leftCell,
		Vector3 right, HexCell rightCell
	) {
		float b = 1f / (rightCell.Elevation - beginCell.Elevation);
		Vector3 boundary = Vector3.Lerp(begin, right, b);
		Color boundaryColor = Color.Lerp(beginCell.color, rightCell.color, b);

		TriangulateBoundaryTriangle(
			begin, beginCell, left, leftCell, boundary, boundaryColor
		);
	}

	void TriangulateBoundaryTriangle (
		Vector3 begin, HexCell beginCell,
		Vector3 left, HexCell leftCell,
		Vector3 boundary, Color boundaryColor
	) {
		Vector3 v2 = HexMetrics.TerraceLerp(begin, left, 1);
		Color c2 = HexMetrics.TerraceLerp(beginCell.color, leftCell.color, 1);

		AddTriangle(begin, v2, boundary);
		AddTriangleColor(beginCell.color, c2, boundaryColor);

		for (int i = 2; i < HexMetrics.terraceSteps; i++) {
			Vector3 v1 = v2;
			Color c1 = c2;
			v2 = HexMetrics.TerraceLerp(begin, left, i);
			c2 = HexMetrics.TerraceLerp(beginCell.color, leftCell.color, i);
			AddTriangle(v1, v2, boundary);
			AddTriangleColor(c1, c2, boundaryColor);
		}

		AddTriangle(v2, left, boundary);
		AddTriangleColor(c2, leftCell.color, boundaryColor);
	}
```

现在完成顶部很简单。如果我们有一个斜率，添加一个旋转的边界三角形。否则，一个简单的三角形就足够了。

```c#
	void TriangulateCornerTerracesCliff (
		Vector3 begin, HexCell beginCell,
		Vector3 left, HexCell leftCell,
		Vector3 right, HexCell rightCell
	) {
		float b = 1f / (rightCell.Elevation - beginCell.Elevation);
		Vector3 boundary = Vector3.Lerp(begin, right, b);
		Color boundaryColor = Color.Lerp(beginCell.color, rightCell.color, b);
		
		TriangulateBoundaryTriangle(
			begin, beginCell, left, leftCell, boundary, boundaryColor
		);

		if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope) {
			TriangulateBoundaryTriangle(
				left, leftCell, right, rightCell, boundary, boundaryColor
			);
		}
		else {
			AddTriangle(left, right, boundary);
			AddTriangleColor(leftCell.color, rightCell.color, boundaryColor);
		}
	}
```

![double slope](https://catlikecoding.com/unity/tutorials/hex-map/part-3/merging-slopes-and-cliffs/double-slope.png) ![single slope](https://catlikecoding.com/unity/tutorials/hex-map/part-3/merging-slopes-and-cliffs/single-slope.png)

*完成两个部分的三角剖分。*

### 5.3 镜面案例

我们已经覆盖了斜坡悬崖案例。还有两个镜面案例，左边有悬崖。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-3/merging-slopes-and-cliffs/css-csc.png)

*CSS 和 CSC。*

该方法与以前相同，但由于方向不同，存在一些细微差异。复制 `TriangulateCornerTerracesCliff` 并相应调整。我只标出了差异。

```c#
	void TriangulateCornerCliffTerraces (
		Vector3 begin, HexCell beginCell,
		Vector3 left, HexCell leftCell,
		Vector3 right, HexCell rightCell
	) {
		float b = 1f / (leftCell.Elevation - beginCell.Elevation);
		Vector3 boundary = Vector3.Lerp(begin, left, b);
		Color boundaryColor = Color.Lerp(beginCell.color, leftCell.color, b);

		TriangulateBoundaryTriangle(
			right, rightCell, begin, beginCell, boundary, boundaryColor
		);

		if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope) {
			TriangulateBoundaryTriangle(
				left, leftCell, right, rightCell, boundary, boundaryColor
			);
		}
		else {
			AddTriangle(left, right, boundary);
			AddTriangleColor(leftCell.color, rightCell.color, boundaryColor);
		}
	}
```

将这些案例包含在 `TriangulateCorner` 中。

```c#
		if (leftEdgeType == HexEdgeType.Slope) {
			…
		}
		if (rightEdgeType == HexEdgeType.Slope) {
			if (leftEdgeType == HexEdgeType.Flat) {
				TriangulateCornerTerraces(
					right, rightCell, bottom, bottomCell, left, leftCell
				);
				return;
			}
			TriangulateCornerCliffTerraces(
				bottom, bottomCell, left, leftCell, right, rightCell
			);
			return;
		}
```

![double slope](https://catlikecoding.com/unity/tutorials/hex-map/part-3/merging-slopes-and-cliffs/double-slope-mirrored.png) ![single slope](https://catlikecoding.com/unity/tutorials/hex-map/part-3/merging-slopes-and-cliffs/single-slope-mirrored.png)

*CSS 和 CSC 三角测量。*

### 5.4 双悬崖

唯一剩下的非平坦情况是底部单元格两侧都有悬崖的情况。这使得顶部边缘的所有选项都是开放的。它可以是平坦的、斜坡的或悬崖的。我们只对悬崖-悬崖-斜坡感兴趣，因为它是唯一一个有梯田的。

实际上，有两种不同的悬崖-悬崖-斜坡版本，取决于哪一侧更高。他们互相镜像。让我们将它们标识为 CCSR 和 CCSL。

![right higher](https://catlikecoding.com/unity/tutorials/hex-map/part-3/merging-slopes-and-cliffs/double-cliff-slope-right.png) ![left higher](https://catlikecoding.com/unity/tutorials/hex-map/part-3/merging-slopes-and-cliffs/double-cliff-slope-left.png) ![diagram](https://catlikecoding.com/unity/tutorials/hex-map/part-3/merging-slopes-and-cliffs/ccsr-ccsl.png)
*CCSR 和 CCSL。*

我们可以通过调用具有不同单元旋转的 `TriangulateCornerCliffTerraces` 和 `TriangulateCornerTerracesCliff` 方法，在 `TriangulateCorner` 中涵盖这两种情况。

```c#
		if (leftEdgeType == HexEdgeType.Slope) {
			…
		}
		if (rightEdgeType == HexEdgeType.Slope) {
			…
		}
		if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope) {
			if (leftCell.Elevation < rightCell.Elevation) {
				TriangulateCornerCliffTerraces(
					right, rightCell, bottom, bottomCell, left, leftCell
				);
			}
			else {
				TriangulateCornerTerracesCliff(
					left, leftCell, right, rightCell, bottom, bottomCell
				);
			}
			return;
		}
```

然而，这将产生一个奇怪的三角剖分。这是因为我们现在正在从上到下进行三角测量。这导致我们的边界插值器为负，这是不正确的。解决方案是确保插值器始终为正。

```c#
	void TriangulateCornerTerracesCliff (
		Vector3 begin, HexCell beginCell,
		Vector3 left, HexCell leftCell,
		Vector3 right, HexCell rightCell
	) {
		float b = 1f / (rightCell.Elevation - beginCell.Elevation);
		if (b < 0) {
			b = -b;
		}
		…
	}

	void TriangulateCornerCliffTerraces (
		Vector3 begin, HexCell beginCell,
		Vector3 left, HexCell leftCell,
		Vector3 right, HexCell rightCell
	) {
		float b = 1f / (leftCell.Elevation - beginCell.Elevation);
		if (b < 0) {
			b = -b;
		}
		…
	}
```

![right higher](https://catlikecoding.com/unity/tutorials/hex-map/part-3/merging-slopes-and-cliffs/double-cliff-slope-right-filled.png) ![left higher](https://catlikecoding.com/unity/tutorials/hex-map/part-3/merging-slopes-and-cliffs/double-cliff-slope-left-filled.png)

*CCSR 和 CCSL 进行了三角测量。*

### 5.5 清理

我们现在已经涵盖了所有需要特殊处理的案例，以确保梯田被正确地三角化。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-3/merging-slopes-and-cliffs/all-corners.png)

*带梯田的完整三角剖分。*
我们可以通过去掉 `return` 语句并使用 `else` 块来稍微清理 `TriangulateCorner`。

```c#
	void TriangulateCorner (
		Vector3 bottom, HexCell bottomCell,
		Vector3 left, HexCell leftCell,
		Vector3 right, HexCell rightCell
	) {
		HexEdgeType leftEdgeType = bottomCell.GetEdgeType(leftCell);
		HexEdgeType rightEdgeType = bottomCell.GetEdgeType(rightCell);

		if (leftEdgeType == HexEdgeType.Slope) {
			if (rightEdgeType == HexEdgeType.Slope) {
				TriangulateCornerTerraces(
					bottom, bottomCell, left, leftCell, right, rightCell
				);
			}
			else if (rightEdgeType == HexEdgeType.Flat) {
				TriangulateCornerTerraces(
					left, leftCell, right, rightCell, bottom, bottomCell
				);
			}
			else {
				TriangulateCornerTerracesCliff(
					bottom, bottomCell, left, leftCell, right, rightCell
				);
			}
		}
		else if (rightEdgeType == HexEdgeType.Slope) {
			if (leftEdgeType == HexEdgeType.Flat) {
				TriangulateCornerTerraces(
					right, rightCell, bottom, bottomCell, left, leftCell
				);
			}
			else {
				TriangulateCornerCliffTerraces(
					bottom, bottomCell, left, leftCell, right, rightCell
				);
			}
		}
		else if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope) {
			if (leftCell.Elevation < rightCell.Elevation) {
				TriangulateCornerCliffTerraces(
					right, rightCell, bottom, bottomCell, left, leftCell
				);
			}
			else {
				TriangulateCornerTerracesCliff(
					left, leftCell, right, rightCell, bottom, bottomCell
				);
			}
		}
		else {
			AddTriangle(bottom, left, right);
			AddTriangleColor(bottomCell.color, leftCell.color, rightCell.color);
		}
	}
```

最后的 `else` 块涵盖了我们尚未涵盖的所有剩余案例。这些案例是 FFF、CCF、CCCR 和 CCCL。它们都被一个三角形覆盖着。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-3/merging-slopes-and-cliffs/diagram.png)

*所有不同的案例。*

下一个教程是[“不规则性”](https://catlikecoding.com/unity/tutorials/hex-map/part-4/)。

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-3/merging-slopes-and-cliffs/merging-slopes-and-cliffs.unitypackage)

[PDF](https://catlikecoding.com/unity/tutorials/hex-map/part-3/Hex-Map-3.pdf)

# Hex Map 4：不规则性

发布于 2016-04-19

https://catlikecoding.com/unity/tutorials/hex-map/part-4/

*采样噪波纹理。*
*扰动顶点。*
*保持细胞平坦。*
*细分单元格边缘。*



本教程是关于[六边形地图](https://catlikecoding.com/unity/tutorials/hex-map/)系列的第四部分。到目前为止，我们的网格一直是一个严格的蜂窝。在本期中，我们将介绍不规则性，使我们的地图看起来更自然。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-4/tutorial-image.png)

*不再有规则的六边形。*

## 1 噪声

为了增加不规则性，我们需要随机化。但不是真正的随机性。我们希望在编辑地图时保持一致。否则，每次我们做出改变时，事情都会发生变化。因此，我们需要一种可再现的伪随机噪声。

Perlin 噪声是一个很好的候选者。它在任何时候都是可复制的。当多个频率组合在一起时，它也会产生噪声，这些噪声在长距离内变化很大，但在短距离内保持相当相似。这可以产生相对平滑的失真。靠得很近的点往往会粘在一起，而不是朝相反的方向扭曲。

我们可以通过编程生成 Perlin 噪声。[“噪波”](https://catlikecoding.com/unity/tutorials/noise/)教程解释了如何做到这一点。但我们也可以从预先生成的噪声纹理中采样。使用纹理的优点是，它比计算多频 Perlin 噪声更容易、更快。缺点是纹理占用了更多的内存，只覆盖了一小部分噪声。因此，它需要是瓷砖纹理，并且必须相当大才能使瓷砖不那么明显。

### 1.1 噪波纹理

我们将使用纹理，因此您现在不必学习[“噪波”](https://catlikecoding.com/unity/tutorials/noise/)教程。这意味着我们需要这样的纹理。这里有一个。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-4/noise/tiling-perlin-noise.png)

*平铺分形 Perlin 噪波纹理。*

上面的纹理包含平铺的多频 Perlin 噪声。这是一个平均值为 0.5 的灰度图像，极值接近 0 和 1。

但是等等，这只是每个点的一个值。如果我们想要 3D 失真，我们需要至少三个伪随机样本！因此，我们需要两个额外的纹理，每个纹理都有不同的噪声。

我们可以这样做，或者我们可以在每个颜色通道中存储不同的噪声值。这使我们能够在单个纹理中存储多达四种不同的噪声模式。这就是这样的纹理。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-4/noise/noise.png)

*四合一。*

获取此纹理并将其导入到 Unity 项目中。因为我们将通过代码对纹理进行采样，所以它必须是可读的。将*“纹理类型”*切换为*“高级”*，并启用*“读/写启用”*。这将把纹理数据保存在内存中，可以通过 C# 代码访问。请确保将*“格式”*设置为*“自动真彩色”*，否则这将不起作用。无论如何，我们都不想通过纹理压缩破坏我们的噪波模式。

我们可以禁用*生成 Mip 地图*，因为我们不需要它们。当我们这样做的时候，也启用*旁路 sRGB 采样*。我们不需要这个，但它是正确的。它表示纹理不包含 Gamma 空间中的颜色数据。

![inspector](https://catlikecoding.com/unity/tutorials/hex-map/part-4/noise/inspector.png) ![preview](https://catlikecoding.com/unity/tutorials/hex-map/part-4/noise/preview.png)

*导入的噪波纹理。*

> **sRGB 采样何时重要？**
>
> 如果我们在某个时候在着色器中使用噪波纹理，那会有所不同。使用线性渲染模式时，纹理采样会自动将颜色数据从 Gamma 转换到线性颜色空间。这会对我们的噪波纹理产生不正确的结果，所以我们不希望发生这种情况。

> **我的纹理导入设置看起来不一样吗？**
>
> 本教程编写后，它们已被更改。您应该使用默认的 2D 纹理设置，禁用 *sRGB（颜色纹理）*并将*“压缩”*设置为*“无”*。

### 1.2 采样噪声

让我们将噪声采样功能添加到 `HexMetrics` 中，这样它就可以在任何地方使用。这意味着 `HexMetrics` 在很大程度上参考了噪声纹理。

```c#
	public static Texture2D noiseSource;
```

因为它不是组件，所以我们无法通过编辑器将纹理分配给它。我们将简单地使用 `HexGrid` 作为中介。由于 `HexGrid` 是第一个采取行动的，如果我们在 `Awake` 方法开始时传递纹理，那就很好了。

```c#
	public Texture2D noiseSource;

	void Awake () {
		HexMetrics.noiseSource = noiseSource;

		…
	}
```

然而，在播放模式下，这种方法无法在重新编译后继续使用。Unity 不序列化静态变量。为了解决这个问题，也可以在 `OnEnable` 事件方法中重新分配纹理。此方法将在重新编译后被调用。

```c#
	void OnEnable () {
		HexMetrics.noiseSource = noiseSource;
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-4/noise/noise-assigned.png)

*指定噪波纹理。*

既然 `HexMetrics` 可以访问纹理，让我们为其添加一种方便的噪声采样方法。这种方法采用世界位置并生成一个包含四个噪声样本的 4D 向量。

```c#
	public static Vector4 SampleNoise (Vector3 position) {
	}
```

这些样本是通过使用双线性滤波对纹理进行采样而产生的，使用 X 和 Z 世界坐标作为 UV 坐标。由于我们的噪声源是二维的，我们忽略了第三世界坐标。如果我们的噪声源是 3D 的，那么我们也会使用Y世界坐标。

我们最终得到一种颜色，可以将其转换为 4D 矢量。这种转换可以是隐式的，这意味着我们可以直接返回颜色，而无需显式包含（`Vector4`）。

```c#
	public static Vector4 SampleNoise (Vector3 position) {
		return noiseSource.GetPixelBilinear(position.x, position.z);
	}
```

> **双线性滤波是如何工作的？**
>
> 有关 UV 坐标和纹理过滤的说明，请参见[渲染 2，着色器基础](https://catlikecoding.com/unity/tutorials/rendering/part-2/)教程。

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-4/noise/noise.unitypackage)

## 2 扰动端点

我们通过单独扰动每个顶点来扭曲我们的规则蜂窝网格。因此，让我们在 `HexMesh` 中添加一个 `Perturb` 方法来实现这一点。它取一个未扰动点，并返回一个扰动点。为此，它使用未受干扰的点对我们的噪声进行采样。

```c#
	Vector3 Perturb (Vector3 position) {
		Vector4 sample = HexMetrics.SampleNoise(position);
	}
```

让我们简单地将 X、Y 和 Z 噪声样本直接添加到点的相应坐标上，并将其用作结果。

```c#
	Vector3 Perturb (Vector3 position) {
		Vector4 sample = HexMetrics.SampleNoise(position);
		position.x += sample.x;
		position.y += sample.y;
		position.z += sample.z;
		return position;
	}
```

我们如何快速更改 `HexMesh`，使所有顶点都受到扰动？通过在 `AddTriangle` 和 `AddQuad` 中将每个顶点添加到顶点列表时对其进行调整。所以，让我们这样做。

```c#
	void AddTriangle (Vector3 v1, Vector3 v2, Vector3 v3) {
		int vertexIndex = vertices.Count;
		vertices.Add(Perturb(v1));
		vertices.Add(Perturb(v2));
		vertices.Add(Perturb(v3));
		…
	}

	void AddQuad (Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4) {
		int vertexIndex = vertices.Count;
		vertices.Add(Perturb(v1));
		vertices.Add(Perturb(v2));
		vertices.Add(Perturb(v3));
		vertices.Add(Perturb(v4));
		…
	}
```

> **四边形在扰动顶点后仍然是平的吗？**
>
> 他们很可能不是。它们由两个不再对齐的三角形组成。但是，由于这些三角形共享两个顶点，因此这些顶点的法线将被平滑。这意味着您不会看到两个三角形之间的急剧过渡。如果失真不太大，您仍然会认为四边形是平的。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-4/perturbing-vertices/perturbed-vertices.png)

*是否扰动顶点。*

它看起来没有太大变化，只是单元格标签似乎缺失了。这是因为我们将噪声样本添加到我们的点上，并且它们总是正的。因此，所有三角形最终都位于标签上方，使标签变得模糊。我们必须集中调整，这样他们就可以朝着任何一个方向前进。将噪声样本的范围从 0–1 更改为 -1–1。

```c#
	Vector3 Perturb (Vector3 position) {
		Vector4 sample = HexMetrics.SampleNoise(position);
		position.x += sample.x * 2f - 1f;
		position.y += sample.y * 2f - 1f;
		position.z += sample.z * 2f - 1f;
		return position;
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-4/perturbing-vertices/perturbed-centered.png)

*集中扰动。*

### 2.1 扰动强度

现在很明显，我们扭曲了网格，但效果非常微妙。每个维度最多调整 1 个单位。因此，理论上的最大位移为 √3 ≈ 1.73 单位，如果发生这种情况，那将是极其罕见的。由于我们的细胞外半径为 10 个单位，因此扰动相对较小。

解决方案是在 `HexMetrics` 中添加一个强度设置，这样我们就可以缩放扰动。让我们试试 5 的力量。这具有 √75 ≈ 8.66 单位的理论最大位移，这应该更明显。

```c#
	public const float cellPerturbStrength = 5f;
```

通过将强度与 `HexMesh.Perturb` 中的样本相乘来应用强度。

```c#
	Vector3 Perturb (Vector3 position) {
		Vector4 sample = HexMetrics.SampleNoise(position);
		position.x += (sample.x * 2f - 1f) * HexMetrics.cellPerturbStrength;
		position.y += (sample.y * 2f - 1f) * HexMetrics.cellPerturbStrength;
		position.z += (sample.z * 2f - 1f) * HexMetrics.cellPerturbStrength;
		return position;
	}
```

![strength](https://catlikecoding.com/unity/tutorials/hex-map/part-4/perturbing-vertices/perturbed-strength.png) ![incoheren](https://catlikecoding.com/unity/tutorials/hex-map/part-4/perturbing-vertices/perturbed-incoherent.png)

*增强力量。*

### 2.2 噪声等级

虽然网格在编辑前看起来很好，但一旦出现梯田，事情就会出错。它们的顶点在不同的方向上扭曲，导致混乱。使用 Perlin 噪声时不应出现这种情况。

问题的发生是因为我们直接使用世界坐标对噪声进行采样。这会导致纹理平铺每个单元，而我们的单元格比这大得多。实际上，纹理在任意位置被采样，破坏了它所具有的任何连贯性。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-4/perturbing-vertices/grid.png)

*10乘10的网格线与蜂窝重叠。*

我们必须缩放噪声采样，使纹理覆盖更大的区域。让我们将此比例添加到 `HexMetrics` 并将其设置为 0.003，然后按该因子缩放样本坐标。

```c#
	public const float noiseScale = 0.003f;

	public static Vector4 SampleNoise (Vector3 position) {
		return noiseSource.GetPixelBilinear(
			position.x * noiseScale,
			position.z * noiseScale
		);
	}
```

突然间，我们的纹理覆盖了 333⅓ 平方单位，其局部连贯性变得明显。

![scaled](https://catlikecoding.com/unity/tutorials/hex-map/part-4/perturbing-vertices/perturbed-scaled.png) ![coherent](https://catlikecoding.com/unity/tutorials/hex-map/part-4/perturbing-vertices/perturbed-coherent.png)
*缩放噪音。*

我们的新缩放还确保噪音需要一段时间才能消散。实际上，由于电池的内径为 10√3 个单位，它在 X 维度上实际上永远不会完全平铺。然而，由于噪声的局部一致性，即使细节不匹配，你仍然可以在更大的范围内检测到重复的模式，大约每 20 个单元格一次。但这对于一张毫无特色的地图来说才是显而易见的。

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-4/perturbing-vertices/perturbing-vertices.unitypackage)

## 3 平整单元格中心

扰动所有顶点可以使我们的地图看起来更自然，但也有一些问题。由于单元格现在不均匀，它们的标签与网格相交。梯田与悬崖相接处的网格上出现了裂缝。我们将把裂缝留到以后，专注于单元格表面。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-4/leveling-cell-centers/unleveled.png)

*不那么僵化，问题更多。*

解决交叉问题的最简单方法是保持单元中心平坦。只是不要调整 `HexMesh.Perturb` 中的 Y 坐标。

```c#
	Vector3 Perturb (Vector3 position) {
		Vector4 sample = HexMetrics.SampleNoise(position);
		position.x += (sample.x * 2f - 1f) * HexMetrics.cellPerturbStrength;
//		position.y += (sample.y * 2f - 1f) * HexMetrics.cellPerturbStrength;
		position.z += (sample.z * 2f - 1f) * HexMetrics.cellPerturbStrength;
		return position;
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-4/leveling-cell-centers/leveled.png)

*水平化单元格。*

这一变化使所有垂直位置保持不变，无论是单元中心还是露台台阶。请注意，这仅在 XZ 平面内将最大位移减小到 √50 ≈ 7.07。

这是一个不错的变化，因为它可以更容易地识别单个单元格，并防止梯田变得太乱。但一些垂直扰动仍然是好的。

### 3.1 扰动单元格升高

我们可以对每个单元应用垂直扰动，而不是对每个顶点应用垂直扰动。这样，每个细胞都保持平坦，但单元格之间仍然存在差异。对高程扰动使用不同的尺度也是有意义的，因此在 `HexMetrics` 中添加一个尺度。1.5 个单位的强度提供了一些微妙的变化，大致相当于一个露台台阶的高度。

```c#
	public const float elevationPerturbStrength = 1.5f;
```

调整 `HexCell.Elevation` 属性，以便将此扰动应用于单元格的垂直位置。

```c#
	public int Elevation {
		get {
			return elevation;
		}
		set {
			elevation = value;
			Vector3 position = transform.localPosition;
			position.y = value * HexMetrics.elevationStep;
			position.y +=
				(HexMetrics.SampleNoise(position).y * 2f - 1f) *
				HexMetrics.elevationPerturbStrength;
			transform.localPosition = position;

			Vector3 uiPosition = uiRect.localPosition;
			uiPosition.z = -position.y;
			uiRect.localPosition = uiPosition;
		}
	}
```

为了确保立即应用扰动，我们必须在 `HexGrid.CreateCell` 中明确设置每个单元格的标高。否则，网格将开始平坦。在创建 UI 后，在最后执行此操作。

```c#
	void CreateCell (int x, int z, int i) {
		…

		cell.Elevation = 0;
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-4/leveling-cell-centers/perturbed.png)

*受干扰的高地，有裂缝。*

### 3.2 使用相同的高度

网格中出现了很多裂缝，因为我们在对网格进行三角剖分时没有始终如一地使用相同的单元高度。让我们在 `HexCell` 中添加一个方便的属性来检索它的位置，这样我们就可以在任何地方使用它。

```c#
	public Vector3 Position {
		get {
			return transform.localPosition;
		}
	}
```

现在我们可以在 `HexMesh.Triangulate` 中使用该属性，以确定单元格的中心。

```c#
	void Triangulate (HexDirection direction, HexCell cell) {
		Vector3 center = cell.Position;
		…
	}
```

在确定相邻单元的垂直位置时，我们可以在 `TriangulateConnection` 中使用它。

```c#
	void TriangulateConnection (
		HexDirection direction, HexCell cell, Vector3 v1, Vector3 v2
	) {
		…

		Vector3 bridge = HexMetrics.GetBridge(direction);
		Vector3 v3 = v1 + bridge;
		Vector3 v4 = v2 + bridge;
		v3.y = v4.y = neighbor.Position.y;

		…

		HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
		if (direction <= HexDirection.E && nextNeighbor != null) {
			Vector3 v5 = v2 + HexMetrics.GetBridge(direction.Next());
			v5.y = nextNeighbor.Position.y;

			…
		}
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-4/leveling-cell-centers/consistent-heights.png)

*持续使用单元格提升。*

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-4/leveling-cell-centers/leveling-cell-centers.unitypackage)

## 4 细分单元格边

虽然我们的单元格变化很大，但它们仍然明显是六边形的。这本身不是问题，但我们可以做得更好。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-4/subdividing-cell-edges/hexagons.png)

*明显的六边形单元格。*

如果我们有更多的顶点，我们会看到更多的局部变化。因此，让我们通过在每对角之间的中间引入一个边顶点，将每个单元格边分成两部分。这意味着 `HexMesh.Triangulate` 必须加两个三角形，而不是一个三角形。

```c#
	void Triangulate (HexDirection direction, HexCell cell) {
		Vector3 center = cell.Position;
		Vector3 v1 = center + HexMetrics.GetFirstSolidCorner(direction);
		Vector3 v2 = center + HexMetrics.GetSecondSolidCorner(direction);

		Vector3 e1 = Vector3.Lerp(v1, v2, 0.5f);

		AddTriangle(center, v1, e1);
		AddTriangleColor(cell.color);
		AddTriangle(center, e1, v2);
		AddTriangleColor(cell.color);

		if (direction <= HexDirection.SE) {
			TriangulateConnection(direction, cell, v1, v2);
		}
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-4/subdividing-cell-edges/12-sides.png)

*十二面而不是六面。*

将顶点和三角形加倍，为我们的单元格边缘增加了更多的多样性。让我们通过将顶点增加三倍来使它们更加坚固。

```c#
		Vector3 e1 = Vector3.Lerp(v1, v2, 1f / 3f);
		Vector3 e2 = Vector3.Lerp(v1, v2, 2f / 3f);

		AddTriangle(center, v1, e1);
		AddTriangleColor(cell.color);
		AddTriangle(center, e1, e2);
		AddTriangleColor(cell.color);
		AddTriangle(center, e2, v2);
		AddTriangleColor(cell.color);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-4/subdividing-cell-edges/18-sides.png)

*18 面。*

### 4.1 细分边连接

当然，我们还必须细分边缘连接。因此，将新的边顶点传递给 `TriangulateConnection`。

```c#
		if (direction <= HexDirection.SE) {
			TriangulateConnection(direction, cell, v1, e1, e2, v2);
		}
```

将匹配参数添加到 `TriangulateConnection` 中，以便它可以处理额外的顶点。

```c#
	void TriangulateConnection (
		HexDirection direction, HexCell cell,
		Vector3 v1, Vector3 e1, Vector3 e2, Vector3 v2
	) {
	…
}
```

我们还需要为相邻单元计算额外的边顶点。我们可以在桥接到另一侧后计算这些值。

```c#
		Vector3 bridge = HexMetrics.GetBridge(direction);
		Vector3 v3 = v1 + bridge;
		Vector3 v4 = v2 + bridge;
		v3.y = v4.y = neighbor.Position.y;

		Vector3 e3 = Vector3.Lerp(v3, v4, 1f / 3f);
		Vector3 e4 = Vector3.Lerp(v3, v4, 2f / 3f);
```

接下来，我们必须调整边缘的三角剖分。暂时忽略梯田坡度，只需添加三个四边形而不是一个。

```c#
		if (cell.GetEdgeType(direction) == HexEdgeType.Slope) {
			TriangulateEdgeTerraces(v1, v2, cell, v3, v4, neighbor);
		}
		else {
			AddQuad(v1, e1, v3, e3);
			AddQuadColor(cell.color, neighbor.color);
			AddQuad(e1, e2, e3, e4);
			AddQuadColor(cell.color, neighbor.color);
			AddQuad(e2, v2, e4, v4);
			AddQuadColor(cell.color, neighbor.color);
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-4/subdividing-cell-edges/subdivided-connections.png)

*细分连接。*

### 4.2 捆绑边顶点

由于我们现在需要四个顶点来描述一条边，因此将它们捆绑在一起是有意义的。这比处理四个单独的顶点更方便。为此创建一个简单的 `EdgeVertices` 结构。它应该包含四个顶点，沿单元格边缘顺时针排列。

```c#
using UnityEngine;

public struct EdgeVertices {

	public Vector3 v1, v2, v3, v4;
}
```

> **它不需要序列化吗？**
>
> 我们只在三角剖分时使用这种结构。此时我们不会存储边顶点。因此，它不需要是可序列化的。

给它一个方便的构造函数方法，它负责计算中间边缘点。

```c#
	public EdgeVertices (Vector3 corner1, Vector3 corner2) {
		v1 = corner1;
		v2 = Vector3.Lerp(corner1, corner2, 1f / 3f);
		v3 = Vector3.Lerp(corner1, corner2, 2f / 3f);
		v4 = corner2;
	}
```

现在，我们可以在 `HexMesh` 中添加一个单独的三角剖分方法，在单元格的中心和其中一条边之间创建一个三角形扇形。

```c#
	void TriangulateEdgeFan (Vector3 center, EdgeVertices edge, Color color) {
		AddTriangle(center, edge.v1, edge.v2);
		AddTriangleColor(color);
		AddTriangle(center, edge.v2, edge.v3);
		AddTriangleColor(color);
		AddTriangle(center, edge.v3, edge.v4);
		AddTriangleColor(color);
	}
```

以及一种在两条边之间对四边形条进行三角剖分的方法。

```c#
	void TriangulateEdgeStrip (
		EdgeVertices e1, Color c1,
		EdgeVertices e2, Color c2
	) {
		AddQuad(e1.v1, e1.v2, e2.v1, e2.v2);
		AddQuadColor(c1, c2);
		AddQuad(e1.v2, e1.v3, e2.v2, e2.v3);
		AddQuadColor(c1, c2);
		AddQuad(e1.v3, e1.v4, e2.v3, e2.v4);
		AddQuadColor(c1, c2);
	}
```

这使我们能够简化 `Triangulate` 方法。

```c#
	void Triangulate (HexDirection direction, HexCell cell) {
		Vector3 center = cell.Position;
		EdgeVertices e = new EdgeVertices(
			center + HexMetrics.GetFirstSolidCorner(direction),
			center + HexMetrics.GetSecondSolidCorner(direction)
		);

		TriangulateEdgeFan(center, e, cell.color);

		if (direction <= HexDirection.SE) {
			TriangulateConnection(direction, cell, e);
		}
	}
```

转到 `TriangulateConnection`。我们现在可以使用 `TriangulateEdgeStrip`，但也必须进行其他一些替换。在我们第一次使用 `v1` 的地方，我们应该使用 `e1.v1`。同样地，`v2` 变成 `e1.v4`，`v3` 变成 `e2.v1`，`v4` 变成 `e2.v4`。

```c#
	void TriangulateConnection (
		HexDirection direction, HexCell cell, EdgeVertices e1
	) {
		HexCell neighbor = cell.GetNeighbor(direction);
		if (neighbor == null) {
			return;
		}

		Vector3 bridge = HexMetrics.GetBridge(direction);
		bridge.y = neighbor.Position.y - cell.Position.y;
		EdgeVertices e2 = new EdgeVertices(
			e1.v1 + bridge,
			e1.v4 + bridge
		);
		
		if (cell.GetEdgeType(direction) == HexEdgeType.Slope) {
			TriangulateEdgeTerraces(e1.v1, e1.v4, cell, e2.v1, e2.v4, neighbor);
		}
		else {
			TriangulateEdgeStrip(e1, cell.color, e2, neighbor.color);
		}
		
		HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
		if (direction <= HexDirection.E && nextNeighbor != null) {
			Vector3 v5 = e1.v4 + HexMetrics.GetBridge(direction.Next());
			v5.y = nextNeighbor.Position.y;

			if (cell.Elevation <= neighbor.Elevation) {
				if (cell.Elevation <= nextNeighbor.Elevation) {
					TriangulateCorner(
						e1.v4, cell, e2.v4, neighbor, v5, nextNeighbor
					);
				}
				else {
					TriangulateCorner(
						v5, nextNeighbor, e1.v4, cell, e2.v4, neighbor
					);
				}
			}
			else if (neighbor.Elevation <= nextNeighbor.Elevation) {
				TriangulateCorner(
					e2.v4, neighbor, v5, nextNeighbor, e1.v4, cell
				);
			}
			else {
				TriangulateCorner(
					v5, nextNeighbor, e1.v4, cell, e2.v4, neighbor
				);
			}
		}
```

### 4.3 细分梯田

我们还必须细分梯田。因此，将边传递给 `TriangulateEdgeTerraces`。

```c#
		if (cell.GetEdgeType(direction) == HexEdgeType.Slope) {
			TriangulateEdgeTerraces(e1, cell, e2, neighbor);
		}
```

现在我们必须调整 `TriangulateEdgeTerraces`，使其在边之间插值，而不是在顶点对之间插值。让我们假设 `EdgeVertices` 有一种方便的静态插值方法。这使我们能够简化 `TriangulateEdgeTerraces`，而不是使其更加复杂。

```c#
	void TriangulateEdgeTerraces (
		EdgeVertices begin, HexCell beginCell,
		EdgeVertices end, HexCell endCell
	) {
		EdgeVertices e2 = EdgeVertices.TerraceLerp(begin, end, 1);
		Color c2 = HexMetrics.TerraceLerp(beginCell.color, endCell.color, 1);

		TriangulateEdgeStrip(begin, beginCell.color, e2, c2);

		for (int i = 2; i < HexMetrics.terraceSteps; i++) {
			EdgeVertices e1 = e2;
			Color c1 = c2;
			e2 = EdgeVertices.TerraceLerp(begin, end, i);
			c2 = HexMetrics.TerraceLerp(beginCell.color, endCell.color, i);
			TriangulateEdgeStrip(e1, c1, e2, c2);
		}

		TriangulateEdgeStrip(e2, c2, end, endCell.color);
	}
```

`EdgeVertices.TerraceLerp` 方法只是在所有四对两个边顶点之间执行阶地插值。

```c#
	public static EdgeVertices TerraceLerp (
		EdgeVertices a, EdgeVertices b, int step)
	{
		EdgeVertices result;
		result.v1 = HexMetrics.TerraceLerp(a.v1, b.v1, step);
		result.v2 = HexMetrics.TerraceLerp(a.v2, b.v2, step);
		result.v3 = HexMetrics.TerraceLerp(a.v3, b.v3, step);
		result.v4 = HexMetrics.TerraceLerp(a.v4, b.v4, step);
		return result;
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-4/subdividing-cell-edges/subdivided-terraces.png)

*细分梯田。*

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-4/subdividing-cell-edges/subdividing-cell-edges.unitypackage)

## 5 重新连接悬崖和梯田

到目前为止，我们忽略了悬崖和梯田交汇处的裂缝。是时候处理这个问题了。让我们首先考虑悬崖-斜坡-斜坡（CSS）和斜坡-悬崖-斜坡（SCS）的情况。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-4/reconnecting-cliffs-and-terraces/holes.png)

*网格中的孔。*

出现问题的原因是边界顶点受到扰动。这意味着它们不再完全沿着悬崖的一侧，这会产生裂缝。这些洞可能并不总是很明显，但也可能非常明显。所以我们必须确保它们永远不会出现。

解决方案是不扰动边界顶点。这意味着我们需要控制一个点是否受到扰动。最简单的方法是创建一个完全不扰动顶点的 `AddTriangle` 替代方案。

```c#
	void AddTriangleUnperturbed (Vector3 v1, Vector3 v2, Vector3 v3) {
		int vertexIndex = vertices.Count;
		vertices.Add(v1);
		vertices.Add(v2);
		vertices.Add(v3);
		triangles.Add(vertexIndex);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 2);
	}
```

调整 `TriangulateBoundaryTriangle` ，使其使用此方法。这意味着它必须显式扰动除边界顶点之外的所有顶点。

```c#
	void TriangulateBoundaryTriangle (
		Vector3 begin, HexCell beginCell,
		Vector3 left, HexCell leftCell,
		Vector3 boundary, Color boundaryColor
	) {
		Vector3 v2 = HexMetrics.TerraceLerp(begin, left, 1);
		Color c2 = HexMetrics.TerraceLerp(beginCell.color, leftCell.color, 1);

		AddTriangleUnperturbed(Perturb(begin), Perturb(v2), boundary);
		AddTriangleColor(beginCell.color, c2, boundaryColor);

		for (int i = 2; i < HexMetrics.terraceSteps; i++) {
			Vector3 v1 = v2;
			Color c1 = c2;
			v2 = HexMetrics.TerraceLerp(begin, left, i);
			c2 = HexMetrics.TerraceLerp(beginCell.color, leftCell.color, i);
			AddTriangleUnperturbed(Perturb(v1), Perturb(v2), boundary);
			AddTriangleColor(c1, c2, boundaryColor);
		}

		AddTriangleUnperturbed(Perturb(v2), Perturb(left), boundary);
		AddTriangleColor(c2, leftCell.color, boundaryColor);
	}
```

请注意，因为我们没有使用 `v2` 来推导任何其他点，所以可以立即扰动它。这是一个简单的优化，它减少了代码，所以让我们这样做。

```c#
	void TriangulateBoundaryTriangle (
		Vector3 begin, HexCell beginCell,
		Vector3 left, HexCell leftCell,
		Vector3 boundary, Color boundaryColor
	) {
		Vector3 v2 = Perturb(HexMetrics.TerraceLerp(begin, left, 1));
		Color c2 = HexMetrics.TerraceLerp(beginCell.color, leftCell.color, 1);

		AddTriangleUnperturbed(Perturb(begin), v2, boundary);
		AddTriangleColor(beginCell.color, c2, boundaryColor);

		for (int i = 2; i < HexMetrics.terraceSteps; i++) {
			Vector3 v1 = v2;
			Color c1 = c2;
			v2 = Perturb(HexMetrics.TerraceLerp(begin, left, i));
			c2 = HexMetrics.TerraceLerp(beginCell.color, leftCell.color, i);
			AddTriangleUnperturbed(v1, v2, boundary);
			AddTriangleColor(c1, c2, boundaryColor);
		}

		AddTriangleUnperturbed(v2, Perturb(left), boundary);
		AddTriangleColor(c2, leftCell.color, boundaryColor);
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-4/reconnecting-cliffs-and-terraces/unperturbed.png)

*不受干扰的边界。*

这看起来更好，但我们还没有完成。在 `TriangulateCornerTerracesCliff` 方法中，通过在左右点之间插值来找到边界点。然而，这些要点尚未受到干扰。为了使边界点与最终悬崖相匹配，我们必须在受扰点之间进行插值。

```c#
		Vector3 boundary = Vector3.Lerp(Perturb(begin), Perturb(right), b);
```

`TriangulateCornerCliffTerraces` 方法也是如此。

```c#
		Vector3 boundary = Vector3.Lerp(Perturb(begin), Perturb(left), b);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-4/reconnecting-cliffs-and-terraces/closed.png)

*没有更多的洞。*

### 5.1 双崖与斜坡

剩下的问题案例是那些有两个悬崖和一个斜坡的案例。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-4/reconnecting-cliffs-and-terraces/double-cliff-slope.png)

*一个三角形造成的大洞*。

这个问题是通过对 `TriangulateCornerTerracesCliff` 末端 `else` 块中的单个三角形使用手动扰动来解决的。

```c#
		else {
			AddTriangleUnperturbed(Perturb(left), Perturb(right), boundary);
			AddTriangleColor(leftCell.color, rightCell.color, boundaryColor);
		}
```

`TriangulateCornerCliffTerraces` 也是如此。

```c#
		else {
			AddTriangleUnperturbed(Perturb(left), Perturb(right), boundary);
			AddTriangleColor(leftCell.color, rightCell.color, boundaryColor);
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-4/reconnecting-cliffs-and-terraces/double-cliff-closed.png)

*最后的裂缝消失了。*

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-4/reconnecting-cliffs-and-terraces/reconnecting-cliffs-and-terraces.unitypackage)

## 6 扭捏（Tweaking）

我们现在有一个完全正确的扰动网格。它的确切外观取决于特定的噪声、规模和扰动强度。在我们的例子中，扰动可能有点太强了。虽然不规则的外观很好，但我们不希望单元格与规则网格偏离太多。毕竟，我们仍然使用它来确定我们正在编辑的单元格。如果单元格的大小变化太大，以后将更难将内容放入其中。

![unperturbed](https://catlikecoding.com/unity/tutorials/hex-map/part-4/tweaking/unperturbed.png) ![perturbed](https://catlikecoding.com/unity/tutorials/hex-map/part-4/tweaking/perturbed.png)

*不受干扰vs.不受干扰。*

5 的单元格扰动强度似乎有点太大了。

*单元格扰动从 0 到 5。*

让我们将其减少到 4，使其更易于管理，而不会变得过于规则。这保证了最大 XZ 位移 √32 ≈ 5.66。

```c#
	public const float cellPerturbStrength = 4f;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-4/tweaking/perturbation-4.png)

*单元格扰动 4。*

我们可以调整的另一个值是固体因子。如果我们增加它，扁平细胞中心将变得更大。这为未来的内容留下了更多的空间。当然，它们也会变得更加六边形。

*固体系数为 0.75 至 0.95。*

固体因子略微增加到0.8将使我们以后的生活变得更容易。

```c#
	public const float solidFactor = 0.8f;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-4/tweaking/solid-factor.png)

*固体系数 0.8。*

最后，海拔之间的差异有点大。在检查网格是否正确生成时，这很方便，但我们已经完成了。让我们把它减少到每个露台台阶一个单位，所以 3。

```c#
	public const float elevationStep = 3f;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-4/tweaking/elevation-step-3.png)

*仰角(Elevation)步长减小到 3。*

我们还可以调整海拔扰动强度。但目前设置为 1.5，相当于半个仰角，这很好。

较小的标高步长也使使用我们的所有七个标高更实用。这使我们能够为地图增添更多的多样性。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-4/tweaking/more-elevation-levels.png)

*使用七个标高。*

下一个教程是[“大地图”](https://catlikecoding.com/unity/tutorials/hex-map/part-5/)。

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-4/tweaking/tweaking.unitypackage)

[PDF](https://catlikecoding.com/unity/tutorials/hex-map/part-4/Hex-Map-4.pdf)



# Hex Map 5：大地图

发布于 2016-05-24

https://catlikecoding.com/unity/tutorials/hex-map/part-5/

*将网格分割成块。*
*控制摄像头。*
*油漆颜色和标高分开。*
*使用较大的单元格刷。*



本教程是关于[六边形地图](https://catlikecoding.com/unity/tutorials/hex-map/)系列的第五部分。到目前为止，我们使用的是一张非常小的地图。是时候扩大规模了。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-5/tutorial-image.jpg)

*是时候做大了。*

## 1 网格块

我们不能把网格做得太大，因为我们遇到了单个网格所能容纳的极限。解决方案？使用多个网格。为此，我们必须将网格划分为多个块。我们将使用固定大小的矩形块。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-5/grid-chunks/chunks.png)

*将网格划分为 3 乘 3 段。*

让我们使用 5 乘 5 的块，即每个块 25 个单元格。在 `HexMetrics` 中定义它。

```c#
	public const int chunkSizeX = 5, chunkSizeZ = 5;
```

> **什么是好的块大小？**
>
> 这取决于。使用较大的块意味着您将拥有更少但更大的网格。这导致平局次数减少。但较小的块在截锥剔除中效果更好，这会导致绘制的三角形更少。务实的做法是选择一个尺寸，稍后进行微调。

现在我们不能再为网格使用任何大小，我们必须使用块大小的倍数。因此，让我们更改 `HexGrid`，使其以块而不是单个单元格定义其大小。默认情况下，将其设置为 4 乘 3 块，总共 12 个块和 300 个单元格。这给了我们一个很好的小测试图。

```c#
	public int chunkCountX = 4, chunkCountZ = 3;
```

我们仍将使用 `width` 和 `height`，但它们应该是私有的。我们还将这些字段重命名为 `cellCountX` 和 `cellCountZ`。使用编辑器一次性重命名这些变量的所有出现。现在，当我们处理块或单元格计数时，就很清楚了。

```c#
//	public int width = 6;
//	public int height = 6;
	
	int cellCountX, cellCountZ;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-5/grid-chunks/chunk-count.png)

*以块为单位指定大小。*

调整 `Awake`，使单元格计数在需要之前从块计数中得出。将细胞的创建也放在自己的方法中，以保持 `Awake` 的整洁。

```c#
	void Awake () {
		HexMetrics.noiseSource = noiseSource;

		gridCanvas = GetComponentInChildren<Canvas>();
		hexMesh = GetComponentInChildren<HexMesh>();

		cellCountX = chunkCountX * HexMetrics.chunkSizeX;
		cellCountZ = chunkCountZ * HexMetrics.chunkSizeZ;

		CreateCells();
	}

	void CreateCells () {
		cells = new HexCell[cellCountZ * cellCountX];

		for (int z = 0, i = 0; z < cellCountZ; z++) {
			for (int x = 0; x < cellCountX; x++) {
				CreateCell(x, z, i++);
			}
		}
	}
```

### 1.1 预制块

我们需要一个新的组件类型来表示我们的网格块。

```c#
using UnityEngine;
using UnityEngine.UI;

public class HexGridChunk : MonoBehaviour {
}
```

接下来，创建一个预制块。通过复制 *Hex Grid 对象*并将其重命名为 *Hex Grid Chunk* 来实现。删除其 `HexGrid` 组件，并为其提供 `HexGridChunk` 组件。然后将其转换为预制件，并从场景中删除该对象。

![project](https://catlikecoding.com/unity/tutorials/hex-map/part-5/grid-chunks/chunk-prefab-project.png)
![inspector](https://catlikecoding.com/unity/tutorials/hex-map/part-5/grid-chunks/chunk-prefab-inspector.png)

*预制块，有自己的画布和网格。*

由于 `HexGrid` 将实例化这些块，请给它一个块前缀的引用。

```c#
	public HexGridChunk chunkPrefab;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-5/grid-chunks/grid-with-chunk-prefab.png)

*现在有大块。*

实例化块看起来很像实例化单元格。用数组跟踪它们，并使用双循环填充它。

```c#
	HexGridChunk[] chunks;

	void Awake () {
		…

		CreateChunks();
		CreateCells();
	}

	void CreateChunks () {
		chunks = new HexGridChunk[chunkCountX * chunkCountZ];

		for (int z = 0, i = 0; z < chunkCountZ; z++) {
			for (int x = 0; x < chunkCountX; x++) {
				HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
				chunk.transform.SetParent(transform);
			}
		}
	}
```

块的初始化类似于我们过去初始化六边形网格的方式。它在 `Awake` 中设置内容，在 `Start` 中进行三角测量。它需要一个画布和网格的引用，以及一个单元格的数组。然而，它不会产生这些单元格。我们仍然会让网格这样做。

```c#
public class HexGridChunk : MonoBehaviour {

	HexCell[] cells;

	HexMesh hexMesh;
	Canvas gridCanvas;

	void Awake () {
		gridCanvas = GetComponentInChildren<Canvas>();
		hexMesh = GetComponentInChildren<HexMesh>();

		cells = new HexCell[HexMetrics.chunkSizeX * HexMetrics.chunkSizeZ];
	}
	
	void Start () {
		hexMesh.Triangulate(cells);
	}
}
```

### 1.2 将单元格分配给块

`HexGrid` 仍在创建所有单元格。这很好，但现在它必须将每个单元格添加到正确的块中，而不是用自己的网格和画布设置它们。

```c#
	void CreateCell (int x, int z, int i) {
		…

		HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
//		cell.transform.SetParent(transform, false);
		cell.transform.localPosition = position;
		cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
		cell.color = defaultColor;

		…

		Text label = Instantiate<Text>(cellLabelPrefab);
//		label.rectTransform.SetParent(gridCanvas.transform, false);
		label.rectTransform.anchoredPosition =
			new Vector2(position.x, position.z);
		label.text = cell.coordinates.ToStringOnSeparateLines();
		cell.uiRect = label.rectTransform;

		cell.Elevation = 0;

		AddCellToChunk(x, z, cell);
	}
	
	void AddCellToChunk (int x, int z, HexCell cell) {
	}
```

我们可以通过将 x 和 z 除以块大小来找到正确的块。

```c#
	void AddCellToChunk (int x, int z, HexCell cell) {
		int chunkX = x / HexMetrics.chunkSizeX;
		int chunkZ = z / HexMetrics.chunkSizeZ;
		HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];
	}
```

使用中间结果，我们还可以确定单元格块本地的索引。一旦我们有了这个，我们就可以将单元格添加到块中。

```c#
	void AddCellToChunk (int x, int z, HexCell cell) {
		int chunkX = x / HexMetrics.chunkSizeX;
		int chunkZ = z / HexMetrics.chunkSizeZ;
		HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];

		int localX = x - chunkX * HexMetrics.chunkSizeX;
		int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
		chunk.AddCell(localX + localZ * HexMetrics.chunkSizeX, cell);
	}
```

然后， `HexGridChunk.AddCell` 方法将单元格放入自己的数组中。然后，它设置单元格的父级及其 UI。

```c#
	public void AddCell (int index, HexCell cell) {
		cells[index] = cell;
		cell.transform.SetParent(transform, false);
		cell.uiRect.SetParent(gridCanvas.transform, false);
	}
```

### 1.3 清理

此时，`HexGrid` 可以摆脱其画布和六边形网格子对象和代码。

```c#
//	Canvas gridCanvas;
//	HexMesh hexMesh;

	void Awake () {
		HexMetrics.noiseSource = noiseSource;

//		gridCanvas = GetComponentInChildren<Canvas>();
//		hexMesh = GetComponentInChildren<HexMesh>();

		…
	}

//	void Start () {
//		hexMesh.Triangulate(cells);
//	}

//	public void Refresh () {
//		hexMesh.Triangulate(cells);
//	}
```

因为我们去掉了 `Refresh`，`HexMapEditor` 不应该再使用它。

```c#
	void EditCell (HexCell cell) {
		cell.color = activeColor;
		cell.Elevation = activeElevation;
//		hexGrid.Refresh();
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-5/grid-chunks/clean-hex-grid.png)

*清理六角网格。*

进入游戏模式（play mode）后，地图看起来仍然一样。但对象层次结构将有所不同。*Hex Grid* 现在生成子块对象，其中包含单元格及其网格和画布。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-5/grid-chunks/chunk-child-objects.png)

*在游戏模式下的子块。*

单元格标签可能有问题。我们最初将标签的宽度设置为 5。这足以显示两个符号，这对于我们到目前为止使用的小地图来说很好。但现在我们可以得到类似 -10 的坐标，它有三个符号。这些将不适合并被切断。要解决此问题，请将单元格标签宽度增加到 10，甚至更多。

![inspector](https://catlikecoding.com/unity/tutorials/hex-map/part-5/grid-chunks/cell-label-width.png) ![scene](https://catlikecoding.com/unity/tutorials/hex-map/part-5/grid-chunks/4x3-chunks.png)

*更宽的单元格标签。*

我们现在可以创建更大的地图！当我们在启动时生成整个网格时，可能需要一段时间才能创建巨大的地图。但一旦完成，你就有很大的空间可以玩。

### 1.4 修复编辑

现在编辑似乎不起作用，因为我们不再刷新网格。我们必须刷新单个块，所以让我们在 `HexGridChunk` 中添加一个 `Refresh` 方法。

```c#
	public void Refresh () {
		hexMesh.Triangulate(cells);
	}
```

我们什么时候调用这个方法？我们过去每次都会刷新整个网格，因为只有一个网格。但现在我们有很多块。与其每次都刷新它们，如果我们只刷新那些已经更改的块，效率会高得多。否则，编辑大型地图将变得非常缓慢。

我们如何知道要刷新哪个块？一个简单的方法是确保每个单元格知道它属于哪个块。然后，单元格可以在块发生更改时刷新它。因此，请为 `HexCell` 提供其块的引用。

```c#
	public HexGridChunk chunk;
```

`HexGridChunk` 可以在添加时将自己分配给单元格。

```c#
	public void AddCell (int index, HexCell cell) {
		cells[index] = cell;
		cell.chunk = this;
		cell.transform.SetParent(transform, false);
		cell.uiRect.SetParent(gridCanvas.transform, false);
	}
```

连接好后，也可以向 `HexCell` 添加一个 `Refresh` 方法。每当刷新单元格时，它只会刷新其块。

```c#
	void Refresh () {
		chunk.Refresh();
	}
```

我们不需要使 `HexCell.Refresh` 公共（public），因为单元格本身最清楚它何时发生了变化。例如，在调整了标高之后。

```c#
	public int Elevation {
		get {
			return elevation;
		}
		set {
			…
			Refresh();
		}
	}
```

实际上，只有当其标高设置为不同的值时，它才需要刷新。如果我们稍后为它指定相同的标高，它甚至不需要重新计算任何东西。因此，我们可以在设定器开始时退出。

```c#
	public int Elevation {
		get {
			return elevation;
		}
		set {
			if (elevation == value) {
				return;
			}
			…
		}
	}
```

但是，第一次将标高设置为零时，这也将跳过计算，因为这是网格的默认标高。为了防止这种情况，请确保初始值永远不会被使用。

```c#
	int elevation = int.MinValue;
```

> **什么是 `int.MinValue`？**
>
> 它是整数可以具有的最小值。由于 C# 整数是 32 位数字，因此有 2^32^ 个可能的整数，在正数和负数之间除以零。一个比特用于指示数字是否为负数。
>
> 最小值为 −2^31^ = −2147483648。我们永远不会使用那个标高！
>
> 最大值为 2^31^ − 1 = 2147483647。由于零，它比 2^31^ 少一。

为了检测单元格颜色的变化，我们还必须将其转换为属性。将其重命名为大写的 `Color`，然后将其转换为具有私有 `color` 变量的属性。默认颜色值为透明黑色，这很好。

```c#
	public Color Color {
		get {
			return color;
		}
		set {
			if (color == value) {
				return;
			}
			color = value;
			Refresh();
		}
	}

	Color color;
```

现在，当进入播放模式时，我们会遇到空引用异常。这是因为在将单元格分配给其块之前，我们将颜色和标高设置为默认值。此时我们不刷新块是可以的，因为我们将在初始化完成后对它们进行三角剖分。换句话说，只有在分配了块的情况下才刷新块。

```c#
	void Refresh () {
		if (chunk) {
			chunk.Refresh();
		}
	}
```

我们可以再次编辑单元格！然而，有一个问题。沿块边界绘制时可能会出现接缝。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-5/grid-chunks/incorrect-editing.png)

*块边界错误。*

这是有道理的，因为当一个单元格发生变化时，与邻居的所有连接也会发生变化。这些邻居最终可能会变成不同的群体。最简单的解决方案是刷新所有邻居的块，如果它们不同的话。

```c#
	void Refresh () {
		if (chunk) {
			chunk.Refresh();
			for (int i = 0; i < neighbors.Length; i++) {
				HexCell neighbor = neighbors[i];
				if (neighbor != null && neighbor.chunk != chunk) {
					neighbor.chunk.Refresh();
				}
			}
		}
	}
```

虽然这有效，但我们最终可能会多次刷新单个块。一旦我们开始一次绘制多个单元格，情况只会变得更糟。

但我们不必在刷新块时立即进行三角剖分。相反，我们可以注意到需要更新，并在编辑完成后进行三角剖分。

因为 `HexGridChunk` 不做任何其他事情，所以我们可以使用它的启用状态来发出需要更新的信号。每当刷新时，我们都会启用该组件。多次启用它不会改变任何事情。稍后，组件将被更新。我们将在该点进行三角剖分，并再次禁用该组件。

我们将使用 `LateUpdate` 而不是 `Update`，这样我们就可以确保在当前帧的编辑完成后进行三角剖分。

```c#
	public void Refresh () {
//		hexMesh.Triangulate(cells);
		enabled = true;
	}

	void LateUpdate () {
		hexMesh.Triangulate(cells);
		enabled = false;
	}
```

> **`Update` 和 `LateUpdate` 有什么区别？**
>
> 每一帧，启用组件的 `Update` 方法都会在某个时刻以任意顺序被调用。完成后，`LateUpdate` 方法也会发生同样的情况。因此，有两个更新步骤，一个早期步骤和一个晚期步骤。

因为我们的组件默认启用，所以我们不再需要在 `Start` 中显式地进行三角剖分。所以我们可以删除这种方法。

```c#
//	void Start () {
//		hexMesh.Triangulate(cells);
//	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-5/grid-chunks/20x20-chunks.png)

*20 乘 20 块，包含 10000 个单元格。*

### 1.5 共享列表

尽管我们已经大大改变了网格三角化的方式，但 `HexMesh` 仍然是一样的。它只需要一组单元格，就能完成它的工作。是否存在单个或多个六边形网格并不重要。但我们还没有考虑使用多个。也许我们可以做出改进？

`HexMesh` 使用的列表实际上是临时缓冲区。它们仅在三角剖分时使用。一次对一个块进行三角剖分。因此，我们实际上只需要一组列表，而不是每个六边形网格对象一组。我们可以通过使列表保持静态来实现。

```c#
	static List<Vector3> vertices = new List<Vector3>();
	static List<Color> colors = new List<Color>();
	static List<int> triangles = new List<int>();

	void Awake () {
		GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
		meshCollider = gameObject.AddComponent<MeshCollider>();
		hexMesh.name = "Hex Mesh";
//		vertices = new List<Vector3>();
//		colors = new List<Color>();
//		triangles = new List<int>();
	}
```

> **静态列表有什么不同吗？**
>
> 这是一个简单的更改，反映了列表的使用方式。所以这是值得做的，尽管我们现在不应该太担心性能。
>
> 它更有效率，因为共享列表时需要更少的内存分配。当使用 20 乘 20 的块时，它节省了超过 100 MB。

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-5/grid-chunks/grid-chunks.unitypackage)

## 2 相机控制

拥有一张大地图是件好事，但如果我们看不见它，那就没什么用了。为了到达整个地图，我们必须移动相机。缩放也会非常方便。所以，让我们创建一个允许这样做的相机装备。

创建一个空对象并将其命名为 *Hex Map Camera*。重置其变换组件，使其位于原点，而不进行任何旋转或缩放调整。给它一个名为 *Swivel* 的子对象，并给它一个子对象 *Stick*。将主摄像头设置为操纵杆的子摄像头，并重置其变换组件。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-5/camera-controls/hierarchy.png)

*摄影机层次结构。*

旋转（swivel）的工作是控制相机看地图的角度。将其旋转（rotation）设置为（45, 0, 0）。操纵杆控制相机的距离。将其位置设置为（0，0，-45）。

现在我们需要一个组件来控制这个钻机（rig）。将此组件指定给摄影机层次的根。给它一个旋转（swivel）和摇杆（stick）的参考，在 `Awake` 中取回它们。

```c#
using UnityEngine;

public class HexMapCamera : MonoBehaviour {

	Transform swivel, stick;

	void Awake () {
		swivel = transform.GetChild(0);
		stick = swivel.GetChild(0);
	}
}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-5/camera-controls/inspector.png)

*六边形地图相机。*

### 2.1 缩放

我们将支持的第一个功能是缩放。我们可以使用浮动变量来跟踪当前的缩放级别。值为 0 表示完全缩小，而值为 1 表示完全放大。让我们从最大缩放开始。

```c#
	float zoom = 1f;
```

缩放通常使用鼠标滚轮或类似的输入法完成。我们可以通过使用默认的*鼠标滚轮*输入轴来实现。添加一个 `Update` 方法，检查是否有任何输入增量，如果有，则调用一个方法来调整缩放。

```c#
	void Update () {
		float zoomDelta = Input.GetAxis("Mouse ScrollWheel");
		if (zoomDelta != 0f) {
			AdjustZoom(zoomDelta);
		}
	}
	
	void AdjustZoom (float delta) {
	}
```

要调整缩放级别，只需将增量添加到其中，然后夹紧以将其保持在 0-1 范围内。

```c#
	void AdjustZoom (float delta) {
		zoom = Mathf.Clamp01(zoom + delta);
	}
```

当我们放大和缩小时，相机的距离应该相应地改变。我们通过调整操纵杆的 Z 位置来实现这一点。添加两个公共浮动，以配置操纵杆在最小和最大缩放时的位置。当我们在一张相对较小的地图上开发时，将它们设置为 -250 和 -45。

```c#
	public float stickMinZoom, stickMaxZoom;
```

调整缩放后，根据新的缩放值在这两个值之间进行线性插值。然后更新操纵杆的位置。

```c#
	void AdjustZoom (float delta) {
		zoom = Mathf.Clamp01(zoom + delta);

		float distance = Mathf.Lerp(stickMinZoom, stickMaxZoom, zoom);
		stick.localPosition = new Vector3(0f, 0f, distance);
	}
```

![inspector](https://catlikecoding.com/unity/tutorials/hex-map/part-5/camera-controls/stick-min-max.png)

*粘最小值和最大值。*

缩放现在有效，但目前没有那么有用。通常，游戏在缩小时会将相机转换为自上而下的视图。我们可以通过旋转旋转来实现。因此，也要为旋转（swivel）添加 min 和 max 变量。将它们设置为 90 和 45。

```c#
	public float swivelMinZoom, swivelMaxZoom;
```

就像手臂位置一样，插值以找到合适的变焦角度。然后设置旋转接头的旋转。

```c#
	void AdjustZoom (float delta) {
		zoom = Mathf.Clamp01(zoom + delta);

		float distance = Mathf.Lerp(stickMinZoom, stickMaxZoom, zoom);
		stick.localPosition = new Vector3(0f, 0f, distance);

		float angle = Mathf.Lerp(swivelMinZoom, swivelMaxZoom, zoom);
		swivel.localRotation = Quaternion.Euler(angle, 0f, 0f);
	}
```

![inspector](https://catlikecoding.com/unity/tutorials/hex-map/part-5/camera-controls/swivel-min-max.png)

*旋转最小值和最大值。*

您可以通过调整滚轮输入设置的灵敏度来调整缩放的变化速度。您可以通过*编辑/项目设置/输入*找到它。例如，将其从默认值 0.1 减小到 0.025 会导致更慢、更平滑的缩放体验。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-5/camera-controls/scroll-wheel-input.png)

*滚轮输入设置。*

### 2.2 移动

接下来是相机的移动。我们必须在 `Update` 中检查 X 和 Z 方向的移动，就像我们在缩放时一样。为此，我们可以使用默认的*水平*和*垂直*输入轴。这允许使用箭头键和 WASD 键移动相机。

```c#
	void Update () {
		float zoomDelta = Input.GetAxis("Mouse ScrollWheel");
		if (zoomDelta != 0f) {
			AdjustZoom(zoomDelta);
		}

		float xDelta = Input.GetAxis("Horizontal");
		float zDelta = Input.GetAxis("Vertical");
		if (xDelta != 0f || zDelta != 0f) {
			AdjustPosition(xDelta, zDelta);
		}
	}

	void AdjustPosition (float xDelta, float zDelta) {
	}
```

最直接的方法是获取相机装备的当前位置，将 X 和 Z 增量添加到其中，并将结果分配回装备（rig）的位置。

```c#
	void AdjustPosition (float xDelta, float zDelta) {
		Vector3 position = transform.localPosition;
		position += new Vector3(xDelta, 0f, zDelta);
		transform.localPosition = position;
	}
```

当我们按住箭头或 WASD 键时，这会使相机移动，但速度并不一致。这取决于帧率。为了确定移动的距离，我们使用了时间增量以及所需的移动速度。因此，添加一个公共 `moveSpeed` 变量并将其设置为 100。然后将该值和时间增量计入位置增量。

```c#
	public float moveSpeed;
						
	void AdjustPosition (float xDelta, float zDelta) {
		float distance = moveSpeed * Time.deltaTime;
		
		Vector3 position = transform.localPosition;
		position += new Vector3(xDelta, 0f, zDelta) * distance;
		transform.localPosition = position;
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-5/camera-controls/move-speed.png)

*移动速度。*

我们现在可以沿 X 轴或 Z 轴以恒定速度移动。但当同时沿对角线移动时，我们最终会走得更快。为了解决这个问题，我们必须对 delta 向量进行归一化。这使我们能够将其作为一个方向。

```c#
	void AdjustPosition (float xDelta, float zDelta) {
		Vector3 direction = new Vector3(xDelta, 0f, zDelta).normalized;
		float distance = moveSpeed * Time.deltaTime;

		Vector3 position = transform.localPosition;
		position += direction * distance;
		transform.localPosition = position;
	}
```

对角线移动现在是正确的，但突然之间，在我们释放所有按键后，相机继续移动了相当长的一段时间。这是因为当按下按键时，输入轴不会立即跳到其极值。相反，他们需要一段时间才能到达那里。当你松开钥匙时也是如此。轴需要一段时间才能归零。然而，由于我们对输入值进行了归一化处理，我们始终保持最大速度。

我们可以调整输入设置以消除延迟，但它们给输入带来了一种平滑的感觉，值得保留。我们能做的就是将最极端的轴值作为阻尼因子应用于运动。

```c#
		Vector3 direction = new Vector3(xDelta, 0f, zDelta).normalized;
		float damping = Mathf.Max(Mathf.Abs(xDelta), Mathf.Abs(zDelta));
		float distance = moveSpeed * damping * Time.deltaTime;
```

*阻尼的运动。*

现在，移动效果很好，至少在放大时是这样。缩小时，它太慢了。缩小时我们应该加快速度。这可以通过将我们的单一 `moveSpeed` 变量替换为最小和最大缩放的两个变量，并进行插值来实现。将它们设置为 400 和 100。

```c#
//	public float moveSpeed;
	public float moveSpeedMinZoom, moveSpeedMaxZoom;

	void AdjustPosition (float xDelta, float zDelta) {
		Vector3 direction = new Vector3(xDelta, 0f, zDelta).normalized;
		float damping = Mathf.Max(Mathf.Abs(xDelta), Mathf.Abs(zDelta));
		float distance =
			Mathf.Lerp(moveSpeedMinZoom, moveSpeedMaxZoom, zoom) *
			damping * Time.deltaTime;

		Vector3 position = transform.localPosition;
		position += direction * distance;
		transform.localPosition = position;
	}
```

![inspector](https://catlikecoding.com/unity/tutorials/hex-map/part-5/camera-controls/move-speed-zoomed.png)

*移动速度随缩放级别而变化。*

现在我们可以在地图上快速移动了！事实上，我们也可以远远超出地图的边缘。这是不可取的。相机应该留在地图内。为了强制执行这一点，我们必须知道地图的边界，因此我们需要一个对网格的引用。添加一个并将其连接起来。

```c#
	public HexGrid grid;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-5/camera-controls/grid.png)

*需要查询网格大小。*

找到新位置后，用新方法夹紧。

```c#
	void AdjustPosition (float xDelta, float zDelta) {
		…
		transform.localPosition = ClampPosition(position);
	}
	
	Vector3 ClampPosition (Vector3 position) {
		return position;
	}
```

X 位置的最小值为零，最大值由地图大小定义。

```c#
	Vector3 ClampPosition (Vector3 position) {
		float xMax =
			grid.chunkCountX * HexMetrics.chunkSizeX *
			(2f * HexMetrics.innerRadius);
		position.x = Mathf.Clamp(position.x, 0f, xMax);

		return position;
	}
```

Z 位置也是如此。

```c#
	Vector3 ClampPosition (Vector3 position) {
		float xMax =
			grid.chunkCountX * HexMetrics.chunkSizeX *
			(2f * HexMetrics.innerRadius);
		position.x = Mathf.Clamp(position.x, 0f, xMax);

		float zMax =
			grid.chunkCountZ * HexMetrics.chunkSizeZ *
			(1.5f * HexMetrics.outerRadius);
		position.z = Mathf.Clamp(position.z, 0f, zMax);

		return position;
	}
```

事实上，这有点不准确。原点位于单元格的中心，而不是左侧。因此，我们希望相机也停在最右侧单元格的中心。为此，从 X 最大值中减去半个单元格。

```c#
		float xMax =
			(grid.chunkCountX * HexMetrics.chunkSizeX - 0.5f) *
			(2f * HexMetrics.innerRadius);
		position.x = Mathf.Clamp(position.x, 0f, xMax);
```

出于同样的原因，Z 最大值也必须减小。因为指标有点不同，我们必须减去一个完整的单元格。

```c#
		float zMax =
			(grid.chunkCountZ * HexMetrics.chunkSizeZ - 1) *
			(1.5f * HexMetrics.outerRadius);
		position.z = Mathf.Clamp(position.z, 0f, zMax);
```

除了一个小细节，我们已经完成了动作。有时 UI 会响应箭头键，这最终会在移动相机的同时移动滑块。当用户界面在您单击它并仍用光标悬停在它上面后认为自己处于活动状态时，就会发生这种情况。

您可以阻止 UI 监听按键输入。这是通过指示 *EventSystem* 对象不发送导航事件来实现的。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-5/camera-controls/no-navigation-events.png)

*不再有导航事件。*

### 2.3 旋转

想看看悬崖后面是什么吗？如果我们能转动相机，那肯定会很方便！所以，让我们添加这个功能。

我们处于什么变焦级别对旋转来说并不重要，所以一个速度就足够了。添加一个公共 `rotationSpeed` 变量并将其设置为 180 度。通过采样旋转轴来检查更新中的旋转增量，并在需要时调整旋转。

```c#
	public float rotationSpeed;

	void Update () {
		float zoomDelta = Input.GetAxis("Mouse ScrollWheel");
		if (zoomDelta != 0f) {
			AdjustZoom(zoomDelta);
		}

		float rotationDelta = Input.GetAxis("Rotation");
		if (rotationDelta != 0f) {
			AdjustRotation(rotationDelta);
		}

		float xDelta = Input.GetAxis("Horizontal");
		float zDelta = Input.GetAxis("Vertical");
		if (xDelta != 0f || zDelta != 0f) {
			AdjustPosition(xDelta, zDelta);
		}
	}

	void AdjustRotation (float delta) {
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-5/camera-controls/rotation-speed.png)

*旋转速度。*

实际上，没有默认的*旋转（Rotation）*轴。我们必须自己创造它。转到输入设置并复制最顶部的*垂直(Vertical)*条目。将副本的名称更改为 *Rotation*，并将其键设置为 QE 以及逗号和圆点。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-5/camera-controls/rotation-input.png)

*旋转输入轴。*

> **我下载了一个 unitypackage，为什么它没有这个输入？**
>
> 输入设置是项目范围内的设置。因此，它们不包括在 unity 包中。幸运的是，很容易自己添加。如果你不这样做，你会得到一个异常，抱怨缺少输入轴。

跟踪旋转角度并在 `AdjustRotation` 中进行调整。然后旋转整个摄影机装备。

```c#
	float rotationAngle;
	
	void AdjustRotation (float delta) {
		rotationAngle += delta * rotationSpeed * Time.deltaTime;
		transform.localRotation = Quaternion.Euler(0f, rotationAngle, 0f);
	}
```

由于一个完整的圆是 360 度，请包裹旋转角度，使其保持在 0 到 360 度之间。

```c#
	void AdjustRotation (float delta) {
		rotationAngle += delta * rotationSpeed * Time.deltaTime;
		if (rotationAngle < 0f) {
			rotationAngle += 360f;
		}
		else if (rotationAngle >= 360f) {
			rotationAngle -= 360f;
		}
		transform.localRotation = Quaternion.Euler(0f, rotationAngle, 0f);
	}
```

*旋转动作。*

旋转现在有效。当你尝试它时，你会注意到这个动作是绝对的。因此，旋转 180 度后，运动与您的预期相反。如果移动是相对于相机的视角进行的，那么它会更加用户友好。我们可以通过将当前旋转与运动方向相乘来实现这一点。

```c#
	void AdjustPosition (float xDelta, float zDelta) {
		Vector3 direction =
			transform.localRotation *
			new Vector3(xDelta, 0f, zDelta).normalized;
		…
	}
```

*相对运动。*

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-5/camera-controls/camera-controls.unitypackage)

## 3 高级编辑

现在我们有了一个更大的地图可以使用，我们的地图编辑器工具可以使用升级。一次编辑一个单元格是相当有限的，因此较大的画笔大小是一个好主意。如果我们能决定只画颜色或立面，让另一个保持原样，那也会很方便。

### 3.1 可选颜色和高度

我们可以通过在切换组中添加一个空白选项来使颜色可选。复制其中一个颜色开关，并将其标签更改为---或其他表示它不是颜色的东西。然后将其 *On Value Changed* 事件的参数设置为 -1。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-5/advanced-editing/not-a-color-index.png)

*不是有效的颜色索引。*

当然，对于我们的颜色数组来说，这是一个无效的索引。我们可以使用它来确定是否应该为单元格应用颜色。

```c#
	bool applyColor;

	public void SelectColor (int index) {
		applyColor = index >= 0;
		if (applyColor) {
			activeColor = colors[index];
		}
	}
	
	void EditCell (HexCell cell) {
		if (applyColor) {
			cell.Color = activeColor;
		}
		cell.Elevation = activeElevation;
	}
```

高度是通过滑块控制的，所以我们不能真正在其中建立一个开关。相反，我们可以使用单独的开关来打开和关闭立面编辑。默认情况下将其设置为打开。

```c#
	bool applyElevation = true;
	
	void EditCell (HexCell cell) {
		if (applyColor) {
			cell.Color = activeColor;
		}
		if (applyElevation) {
			cell.Elevation = activeElevation;
		}
	}
```

在 UI 中添加新的立面切换。我还将所有内容放在一个新面板中，并将立面滑块设置为水平，使 UI 更整洁。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-5/advanced-editing/optional-color-elevation.png)

*可选颜色和标高。*

要切换立面，我们需要一种新的方法将其与 UI 连接。

```c#
	public void SetApplyElevation (bool toggle) {
		applyElevation = toggle;
	}
```

当您将其连接到立面切换时，请确保使用方法列表顶部的动态 bool 方法。正确的版本不会在检查器中显示复选框。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-5/advanced-editing/set-apply-elevation.png)

*传递立面切换的状态。*

现在，您可以选择只绘制颜色或只绘制立面。或者两者都有，像往常一样。你甚至可以选择两者都不画画，但现在这不是很有用。

*在颜色和标高之间切换。*

> **为什么选择颜色时仰角会关闭？**
>
> 当所有开关都属于同一个开关组时，就会发生这种情况。您可能复制了一个颜色选择开关并对其进行了调整，但没有清除其开关组。

### 3.2 刷子尺寸

要支持可变画笔大小，请添加一个 `brushSize` 整数变量和一个通过 UI 设置它的方法。我们将使用滑块，所以我们必须再次从 float 转换为 int。

```c#
	int brushSize;

	public void SetBrushSize (float size) {
		brushSize = (int)size;
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-5/advanced-editing/brush-size-slider.png)

*画笔大小滑块。*

您可以通过复制立面滑块来创建新滑块。将其最大值更改为 4，并将其连接到正确的方法。我也给它贴了个标签。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-5/advanced-editing/brush-size-slider-inspector.png)

*画笔大小滑块设置。*

现在我们可以编辑多个单元格，我们应该使用 `EditCells` 方法。此方法将负责为所有受影响的单元格调用 `EditCell`。最初选择的单元格将作为画笔的中心。

```c#
	void HandleInput () {
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(inputRay, out hit)) {
			EditCells(hexGrid.GetCell(hit.point));
		}
	}

	void EditCells (HexCell center) {
	}
	
	void EditCell (HexCell cell) {
		…
	}
```

画笔大小定义了编辑效果的半径。在半径 0 处，它只是中心单元格。在半径 1 处，它是中心及其邻居。在半径 2 处，它也包括其直接邻居的邻居。等等。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-5/advanced-editing/brush-diagram.png)

*半径 3 以内。*

要编辑所有单元格，我们必须遍历它们。首先，我们需要中心的 X 和 Z 坐标。

```c#
	void EditCells (HexCell center) {
		int centerX = center.coordinates.X;
		int centerZ = center.coordinates.Z;
	}
```

我们通过减去半径来找到最小的 Z 坐标。这定义了第零行。从那一行开始，让我们循环直到覆盖了中心的那一行。

```c#
	void EditCells (HexCell center) {
		int centerX = center.coordinates.X;
		int centerZ = center.coordinates.Z;

		for (int r = 0, z = centerZ - brushSize; z <= centerZ; z++, r++) {
		}
	}
```

底部行的第一个单元格与中心单元格具有相同的 X 坐标。该坐标随着行号的增加而减小。

最后一个单元格的 X 坐标总是等于中心加上半径。

现在我们可以遍历每一行，并通过它们的坐标检索单元格。

```c#
		for (int r = 0, z = centerZ - brushSize; z <= centerZ; z++, r++) {
			for (int x = centerX - r; x <= centerX + brushSize; x++) {
				EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
			}
		}
```

我们还没有带坐标参数的 `HexGrid.GetCell` 方法，所以创建它。转换为偏移坐标并检索单元格。

```c#
	public HexCell GetCell (HexCoordinates coordinates) {
		int z = coordinates.Z;
		int x = coordinates.X + z / 2;
		return cells[x + z * cellCountX];
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-5/advanced-editing/brush-bottom-half.png)

*刷子的下半部分，尺寸 2。*

画笔的其余部分可以通过从上到下到中心的循环来覆盖。在这种情况下，逻辑被镜像，中心行应该被排除在外。

```c#
	void EditCells (HexCell center) {
		int centerX = center.coordinates.X;
		int centerZ = center.coordinates.Z;

		for (int r = 0, z = centerZ - brushSize; z <= centerZ; z++, r++) {
			for (int x = centerX - r; x <= centerX + brushSize; x++) {
				EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
			}
		}
		for (int r = 0, z = centerZ + brushSize; z > centerZ; z--, r++) {
			for (int x = centerX - brushSize; x <= centerX + r; x++) {
				EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
			}
		}
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-5/advanced-editing/brush-complete.png)

*整个刷子，尺寸 2。*

这是有效的，除非我们的画笔最终延伸到网格的边界之外。当这种情况发生时，你会得到一个索引超出范围的异常。为了防止这种情况，请检查 `HexGrid.GetCell` 中的边界，并在请求不存在的单元格时返回 null。

```c#
	public HexCell GetCell (HexCoordinates coordinates) {
		int z = coordinates.Z;
		if (z < 0 || z >= cellCountZ) {
			return null;
		}
		int x = coordinates.X + z / 2;
		if (x < 0 || x >= cellCountX) {
			return null;
		}
		return cells[x + z * cellCountX];
	}
```

为了防止空引用异常，`HexMapEditor` 在编辑之前应该检查它是否真的有一个单元格。

```c#
	void EditCell (HexCell cell) {
		if (cell) {
			if (applyColor) {
				cell.Color = activeColor;
			}
			if (applyElevation) {
				cell.Elevation = activeElevation;
			}
		}
	}
```

*使用多种画笔尺寸。*

### 3.3 切换单元格标签

大多数时候，你可能不需要看到单元格标签。所以，让我们把它们变成可选的。由于每个块都管理自己的画布，请向 `HexGridChunk` 添加一个 `ShowUI` 方法。当 UI 应该可见时，激活画布。否则，将其停用。

```c#
	public void ShowUI (bool visible) {
		gridCanvas.gameObject.SetActive(visible);
	}
```

让我们默认隐藏 UI。

```c#
	void Awake () {
		gridCanvas = GetComponentInChildren<Canvas>();
		hexMesh = GetComponentInChildren<HexMesh>();

		cells = new HexCell[HexMetrics.chunkSizeX * HexMetrics.chunkSizeZ];
		ShowUI(false);
	}
```

当整个地图的 UI 被切换时，也要向 `HexGrid` 添加一个 `ShowUI` 方法。它只是将请求传递给它的块。

```c#
	public void ShowUI (bool visible) {
		for (int i = 0; i < chunks.Length; i++) {
			chunks[i].ShowUI(visible);
		}
	}
```

`HexMapEditor` 也获得了相同的方法，将请求转发到网格。

```c#
	public void ShowUI (bool visible) {
		hexGrid.ShowUI(visible);
	}
```

最后，我们可以在 UI 中添加一个切换并将其连接起来。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-5/advanced-editing/labels-toggle.png)

*标签切换。*

下一个教程是[河流](https://catlikecoding.com/unity/tutorials/hex-map/part-6)。

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-5/advanced-editing/advanced-editing.unitypackage)

[PDF](https://catlikecoding.com/unity/tutorials/hex-map/part-5/Hex-Map-5.pdf)