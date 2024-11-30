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

现在我们可以直观地识别每个单元格，让我们开始移动它们。我们知道，X 方向上相邻六边形单元之间的距离等于内半径的两倍。所以，让我们使用它。此外，到下一行单元格的距离应为外半径的 1.5 倍。

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

如果我们不能与六边形网格交互，那么它就不是很有趣。最基本的交互是触摸单元格，所以让我们添加对它的支持。现在，只需将此代码直接放入 `HexGrid` 中。一旦一切正常，我们就会将其移动到其他地方。

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

这还没有任何作用。我们需要在网格中添加一个碰撞体，这样光线就有东西可以击中。所以给 `HexMesh` 一个网格碰撞体。

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

现在我们可以触摸到正确的单元格了，是时候进行一些真正的互动了。让我们改变我们击中的每个单元格的颜色。为 `HexGrid` 提供可配置的默认和触摸单元格颜色。

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

*带有四种颜色的六边形地图编辑器。*

通过 *GameObject / UI / Panel* 在画布上添加一个面板来容纳颜色选择器。通过 *Components / UI/ Toggle Group* 给它一个切换组（toggle group）。把它做成一个小面板，放在屏幕的一个角落里。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-1/map-editor/panel-toggle-group.png)

*带切换组的彩色面板。*

现在，通过 *GameObject / UI / Toggle* 为面板填充每种颜色一个切换。目前，我们并不担心花哨的用户界面，只需要一个看起来足够好的手动设置。

![ui](https://catlikecoding.com/unity/tutorials/hex-map/part-1/map-editor/ui.png) ![hierarchy](https://catlikecoding.com/unity/tutorials/hex-map/part-1/map-editor/hierarchy.png)
*每种颜色一个切换。*

确保只打开第一个切换。还要将它们都作为切换组的一部分，这样在同一时间只会选择其中一个。最后，将它们连接到编辑器的 `SelectColor` 方法。您可以通过 *On Value Changed* 事件 UI 的加号按钮来执行此操作。选择六边形地图编辑器对象，然后从下拉列表中选择正确的方法。

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

*涂抹单元格过渡。*

## 1 单元格邻居

在我们混合单元格颜色之前，我们需要知道哪些单元格彼此相邻。每个单元格有六个邻居，我们可以用指南针方向来识别。方向为东北、东、东南、西南、西和西北。让我们为此创建一个枚举，并将其放入自己的脚本文件中。

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

我们可以在 `HexGrid.CreateCell` 中初始化邻居关系。当我们从左到右逐行浏览单元格时，我们知道哪些单元格已经创建。这些是我们可以连接的单元格。

最简单的是 E-W 连接。每行的第一个单元格没有西邻。但这一行中的所有其他单元格都是如此。这些邻居是在我们目前正在使用的单元格之前创建的。因此，我们可以将它们连接起来。

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

不幸的是，这将产生 `NullReferenceException`，因为我们的边界单元格没有六个邻居。当我们缺少邻居时，我们该怎么办？让我们务实一点，用单元格本身作为替代品。

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

在六边形的整个表面上混合会导致模糊的混乱。你再也看不清单个单元格了。我们可以通过仅在六边形边缘附近进行混合来大大改善这一点。这留下了一个内部六边形区域，颜色为纯色。

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

情况开始好转，但我们还没有达到目标。两个邻居之间的颜色混合会被边缘附近的单元格污染。为了防止这种情况，我们必须把梯形的角切掉，把它变成矩形。然后，它在单元格和相邻单元格之间形成一座桥梁，在两侧留下间隙。

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

现在我们有了很好的混合区域，我们可以给任何我们想要的大小。模糊或清晰的单元格边缘，这取决于你。但您会注意到，网格边界附近的混合仍然不正确。同样，我们将暂时搁置这一点，而是将注意力集中在另一个问题上。

> **但是颜色转换仍然很难看吗？**
>
> 这就是线性颜色混合的极限。事实上，平色并没有那么好。我们将在未来的教程中升级到地形材质，并进行一些更花哨的混合。

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-2/blend-regions/blend-regions.unitypackage)

## 4 熔合边缘

看看我们网格的拓扑结构。有哪些不同的形状？如果我们忽略边界，那么我们可以识别出三种不同的形状类型。有单色六边形、双色矩形和三色三角形。无论三个单元格在哪里相遇，你都会发现这三个形状。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-2/fusing-edges/three-different-shapes.png)

*三种视觉结构。*

因此，每两个六边形由一个矩形桥连接。每三个六边形由一个三角形连接。然而，我们以更复杂的方式进行三角剖分。我们目前使用两个四边形来连接一对六边形，而不仅仅是一个。我们用总共六个三角形来连接三个六边形。这似乎有些过分。此外，如果我们要直接与单个形状连接，我们就不需要进行任何颜色平均。因此，我们可以减少复杂性、工作量和三角形。

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

这些桥现在形成了六边形之间的直接连接。但我们仍然为每个连接生成两个，每个方向一个，它们重叠。因此，这两个单元格中只有一个需要在它们之间建立桥梁。

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

编辑单元格时，我们现在正在设置其颜色和标高。虽然您可以检查检查器以查看标高是否确实发生了变化，但三角剖分过程仍然会忽略它。

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

检查我们是否处于这种情况，如果是这样，请调用一个新方法 `TriangulateCornerTerraces`。之后，从该方法返回。将此复选框放在旧三角剖分代码之前，这样它将替换默认三角形。

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

这将使我们的梯田在单元格周围不受干扰地流动，直到它们遇到悬崖或地图的尽头。

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

*CSS 和 CSC 三角剖分。*

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

然而，这将产生一个奇怪的三角剖分。这是因为我们现在正在从上到下进行三角剖分。这导致我们的边界插值器为负，这是不正确的。解决方案是确保插值器始终为正。

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

*CCSR 和 CCSL 进行了三角剖分。*

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
*保持单元格平坦。*
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

现在很明显，我们扭曲了网格，但效果非常微妙。每个维度最多调整 1 个单位。因此，理论上的最大位移为 √3 ≈ 1.73 单位，如果发生这种情况，那将是极其罕见的。由于我们的单元格外半径为 10 个单位，因此扰动相对较小。

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

我们的新缩放还确保噪音需要一段时间才能消散。实际上，由于单元格的内径为 10√3 个单位，它在 X 维度上实际上永远不会完全平铺。然而，由于噪声的局部一致性，即使细节不匹配，你仍然可以在更大的范围内检测到重复的模式，大约每 20 个单元格一次。但这对于一张毫无特色的地图来说才是显而易见的。

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

我们可以对每个单元应用垂直扰动，而不是对每个顶点应用垂直扰动。这样，每个单元格都保持平坦，但单元格之间仍然存在差异。对高程扰动使用不同的尺度也是有意义的，因此在 `HexMetrics` 中添加一个尺度。1.5 个单位的强度提供了一些微妙的变化，大致相当于一个露台台阶的高度。

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

我们可以调整的另一个值是固体因子。如果我们增加它，扁平单元格中心将变得更大。这为未来的内容留下了更多的空间。当然，它们也会变得更加六边形。

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

调整 `Awake`，使单元格计数在需要之前从块计数中得出。将单元格的创建也放在自己的方法中，以保持 `Awake` 的整洁。

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

块的初始化类似于我们过去初始化六边形网格的方式。它在 `Awake` 中设置内容，在 `Start` 中进行三角剖分。它需要一个画布和网格的引用，以及一个单元格的数组。然而，它不会产生这些单元格。我们仍然会让网格这样做。

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

# Hex Map 6：河流

发布于 2016-06-21

https://catlikecoding.com/unity/tutorials/hex-map/part-6/

*将河流数据添加到单元格中。*
*支持拖动绘制河流。*
*修建河道。*
*每个块使用多个网格。*
*为列表创建一个通用池。*
*将流动的水三角形化并设置动画。*

本教程是关于[六边形地图](https://catlikecoding.com/unity/tutorials/hex-map/)系列的第六部分。上一部分是关于支持更大的地图。现在我们可以做到了，我们可以开始考虑更大的地形特征。在这种情况下，河流。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-6/tutorial-image.jpg)

*河流从山上流下来。*

## 1 有河流的单元格

有三种方法可以将河流添加到六边形网格中。第一种方法是让它们从一个单元格流到另一个单元格。《无尽帝国》（Endless Legend）就是这样做的。第二种方法是让它们在单元格之间流动，从一条边到另一条边。《文明5》就是这样做的。第三种方法是根本没有特殊的河流结构，而是使用水单元格来暗示它们。《奇迹时代3》就是这样做的。

在我们的例子中，单元格边缘已经被斜坡和悬崖占据了。这给河流留下了很小的空间。所以我们要让它们在单元格间流动。这意味着每个单元格要么没有河流，要么有河流流过，要么是河流的起点或终点。在有河流穿过的单元格中，它要么直行，要么一步转弯，要么两步转弯。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-6/cells-with-rivers/river-configurations.png)

*五种可能的河流形态。*

我们不会支持分叉或合并河流。这会使事情变得更加复杂，尤其是水流。我们也不会关心更大的水体。这些将在后面的教程中介绍。

### 1.1 追踪河流

一个有河流流过的单元格可以被视为既有流入的河流，也有流出的河流。如果它包含一条河流的起点，那么它只有一条流出的河流。如果它包含一条河的尽头，它只有一条流入的河。我们可以用两个布尔值将此信息存储在 `HexCell` 中。

```c#
	bool hasIncomingRiver, hasOutgoingRiver;
```

但这还不够。我们还需要知道这些河流的流向。对于一条流出的河流，这表明它要去哪里。对于一条流入的河流，这表明它来自哪里。

```c#
	bool hasIncomingRiver, hasOutgoingRiver;
	HexDirection incomingRiver, outgoingRiver;
```

在对单元格进行三角剖分时，我们需要这些信息，因此添加getter属性来访问它。我们不支持直接设置它们。稍后我们将为此添加一种不同的方法。

```c#
	public bool HasIncomingRiver {
		get {
			return hasIncomingRiver;
		}
	}

	public bool HasOutgoingRiver {
		get {
			return hasOutgoingRiver;
		}
	}

	public HexDirection IncomingRiver {
		get {
			return incomingRiver;
		}
	}

	public HexDirection OutgoingRiver {
		get {
			return outgoingRiver;
		}
	}
```

一个有用的问题是，无论具体情况如何，单元格中是否有河流。所以，让我们也为它添加一个属性。

```c#
	public bool HasRiver {
		get {
			return hasIncomingRiver || hasOutgoingRiver;
		}
	}
```

另一个典型的问题是，我们是否有河流的起点或终点。如果流入和流出河流的状态不同，那么情况就是这样。这就变成了另一种财产。

```c#
	public bool HasRiverBeginOrEnd {
		get {
			return hasIncomingRiver != hasOutgoingRiver;
		}
	}
```

最后，了解河流是否流经某个边缘是有用的，无论它是流入还是流出。

```c#
	public bool HasRiverThroughEdge (HexDirection direction) {
		return
			hasIncomingRiver && incomingRiver == direction ||
			hasOutgoingRiver && outgoingRiver == direction;
	}
```

### 1.2 清除河流

在担心如何向单元格中添加河流之前，让我们先支持删除它们。首先，一种只移除河流流出部分的方法。

如果一开始就没有一条流出的河流，那就没什么可做的了。否则，把它关掉并刷新一下。

```c#
	public void RemoveOutgoingRiver () {
		if (!hasOutgoingRiver) {
			return;
		}
		hasOutgoingRiver = false;
		Refresh();
	}
```

但这还不是全部。一条流出的河流必须流向某个地方。所以一定有一个邻居有一条来水。我们也必须摆脱这个。

```c#
	public void RemoveOutgoingRiver () {
		if (!hasOutgoingRiver) {
			return;
		}
		hasOutgoingRiver = false;
		Refresh();

		HexCell neighbor = GetNeighbor(outgoingRiver);
		neighbor.hasIncomingRiver = false;
		neighbor.Refresh();
	}
```

> **河流不能流出地图吗？**
>
> 虽然有可能支持，但我们不会。所以我们不必检查邻居是否存在。

从单元格中删除河流只会改变该单元格的外观。与编辑标高或颜色时不同，其邻居不受影响。所以我们只需要刷新单元格本身，而不是它的邻居。

```c#
	public void RemoveOutgoingRiver () {
		if (!hasOutgoingRiver) {
			return;
		}
		hasOutgoingRiver = false;
		RefreshSelfOnly();

		HexCell neighbor = GetNeighbor(outgoingRiver);
		neighbor.hasIncomingRiver = false;
		neighbor.RefreshSelfOnly();
	}
```

这个 `RefreshSelfOnly` 方法只是简单地刷新单元格的块。由于我们在初始化网格时没有更改河流，因此我们不必担心块是否已被分配。

```c#
	void RefreshSelfOnly () {
		chunk.Refresh();
	}
```

移除来水的方法与此相同。

```c#
	public void RemoveIncomingRiver () {
		if (!hasIncomingRiver) {
			return;
		}
		hasIncomingRiver = false;
		RefreshSelfOnly();

		HexCell neighbor = GetNeighbor(incomingRiver);
		neighbor.hasOutgoingRiver = false;
		neighbor.RefreshSelfOnly();
	}
```

而拆除整条河只意味着拆除流出和流入的河流部分。

```c#
	public void RemoveRiver () {
		RemoveOutgoingRiver();
		RemoveIncomingRiver();
	}
```

### 1.3 添加河流

为了支持河流的创建，我们只需要一种方法来设置单元格的流出河流。这应该覆盖任何之前的流出河流，并设置相应的流入河流。

首先，当河流已经存在时，什么也不用做。

```c#
	public void SetOutgoingRiver (HexDirection direction) {
		if (hasOutgoingRiver && outgoingRiver == direction) {
			return;
		}
	}
```

然后，我们必须确保在所需的方向上有一个邻居。此外，河流不能向上流动。所以如果邻居的海拔更高，我们就不得不中止。

```c#
		HexCell neighbor = GetNeighbor(direction);
		if (!neighbor || elevation < neighbor.elevation) {
			return;
		}
```

接下来，我们必须清理之前流出的河流。如果流入的河流与新流出的河流重叠，我们还必须将其移除。

```c#
		RemoveOutgoingRiver();
		if (hasIncomingRiver && incomingRiver == direction) {
			RemoveIncomingRiver();
		}
```

现在我们可以继续设置流出的河流了。

```c#
		hasOutgoingRiver = true;
		outgoingRiver = direction;
		RefreshSelfOnly();
```

在移除当前传入的河流（如果有的话）后，不要忘记设置另一个单元格的传入河流。

```c#
		neighbor.RemoveIncomingRiver();
		neighbor.hasIncomingRiver = true;
		neighbor.incomingRiver = direction.Opposite();
		neighbor.RefreshSelfOnly();
```

### 1.4 防止上游河流

虽然我们已经确保只能添加有效的河流，但其他行为仍可能导致无效的河流。当我们改变一个单元的高度时，我们必须再次强调河流只能向下流动。所有非法河流都必须清除。

```c#
	public int Elevation {
		get {
			return elevation;
		}
		set {
			…

			if (
				hasOutgoingRiver &&
				elevation < GetNeighbor(outgoingRiver).elevation
			) {
				RemoveOutgoingRiver();
			}
			if (
				hasIncomingRiver &&
				elevation > GetNeighbor(incomingRiver).elevation
			) {
				RemoveIncomingRiver();
			}

			Refresh();
		}
	}
```

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-6/cells-with-rivers/cells-with-rivers.unitypackage)

## 2 编辑河流

为了支持编辑河流，我们必须在 UI 中添加一个河流切换。实际上，我们需要支持三种编辑模式。要么忽略河流，要么添加河流，要么删除河流。我们可以使用一个简单的可选切换枚举来跟踪这一点。因为我们只在编辑器中使用它，所以您可以在 `HexMapEditor` 类中定义它，以及一个河流模式字段。

```c#
	enum OptionalToggle {
		Ignore, Yes, No
	}
	
	OptionalToggle riverMode;
```

我们需要一种通过 UI 调整河流模式的方法。

```c#
	public void SetRiverMode (int mode) {
		riverMode = (OptionalToggle)mode;
	}
```

要控制河流模式，请在 UI 中添加三个切换，并将它们组合成一个新的切换组，就像颜色一样。我调整了开关，使它们的标签位于复选框下方。这使得它们足够薄，可以将所有三个选项放在一行中。

> **为什么不使用下拉列表？**
>
> 如果你愿意，可以使用下拉菜单。不幸的是，Unity 的下拉列表在播放模式下无法处理重新编译。选项列表将丢失，这使得它在重新编译后无用。

### 2.1 检测拖动

要创建一条河，我们需要一个单元格和一个方向。目前，`HexMapEditor` 没有给我们这些信息。因此，我们必须添加从一个单元格拖动到另一个单元格的支持。

我们需要知道我们是否有一个有效的拖动，以及它的方向。为了检测拖动，我们还必须记住前一个单元格。

```c#
	bool isDrag;
	HexDirection dragDirection;
	HexCell previousCell;
```

最初，不拖动时，没有前一个单元格。因此，无论何时没有输入，或者我们不与地图交互，我们都必须将其设置为 `null`。

```c#
	void Update () {
		if (
			Input.GetMouseButton(0) &&
			!EventSystem.current.IsPointerOverGameObject()
		) {
			HandleInput();
		}
		else {
			previousCell = null;
		}
	}

	void HandleInput () {
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(inputRay, out hit)) {
			EditCells(hexGrid.GetCell(hit.point));
		}
		else {
			previousCell = null;
		}
	}
```

当前单元格是我们根据命中点找到的单元格。在我们完成对单元格的编辑后，该单元格将成为下一次更新的前一个单元格。

```c#
	void HandleInput () {
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(inputRay, out hit)) {
			HexCell currentCell = hexGrid.GetCell(hit.point);
			EditCells(currentCell);
			previousCell = currentCell;
		}
		else {
			previousCell = null;
		}
	}
```

确定当前单元格后，我们可以将其与前一个单元格（如果有的话）进行比较。如果我们最终得到两个不同的单元格，那么我们可能有一个有效的拖动，应该检查一下。否则，这当然不是拖动。

```c#
		if (Physics.Raycast(inputRay, out hit)) {
			HexCell currentCell = hexGrid.GetCell(hit.point);
			if (previousCell && previousCell != currentCell) {
				ValidateDrag(currentCell);
			}
			else {
				isDrag = false;
			}
			EditCells(currentCell);
			previousCell = currentCell;
		}
```

我们如何验证拖动？通过验证当前单元格是前一单元格的邻居。我们通过遍历其邻居来检查这一点。如果我们找到匹配，那么我们也会立即知道拖动方向。

```c#
	void ValidateDrag (HexCell currentCell) {
		for (
			dragDirection = HexDirection.NE;
			dragDirection <= HexDirection.NW;
			dragDirection++
		) {
			if (previousCell.GetNeighbor(dragDirection) == currentCell) {
				isDrag = true;
				return;
			}
		}
		isDrag = false;
	}
```

> **这不会产生紧张的拖动吗？**
>
> 当你沿着单元格边缘移动光标时，你可能会在这些单元格之间快速振荡。这确实会产生紧张的拖拽，但也没那么糟糕。
>
> 您可以通过记住之前的拖动来缓解这种情况。然后防止下一次拖动立即沿相反方向进行。

### 2.2 调整单元格

既然我们可以检测到拖拽，我们就可以设置流出的河流。我们也可以移除河流，我们不需要拖曳支撑。

```c#
	void EditCell (HexCell cell) {
		if (cell) {
			if (applyColor) {
				cell.Color = activeColor;
			}
			if (applyElevation) {
				cell.Elevation = activeElevation;
			}
			if (riverMode == OptionalToggle.No) {
				cell.RemoveRiver();
			}
			else if (isDrag && riverMode == OptionalToggle.Yes) {
				previousCell.SetOutgoingRiver(dragDirection);
			}
		}
	}
```

这将从上一个单元格向当前单元格绘制一条河流。但它忽略了画笔的大小。这可能是有道理的，但无论如何，让我们为所有被刷子覆盖的单元格画一条河。这可以通过相对于正在编辑的单元格进行操作来实现。在这种情况下，我们必须确保另一个单元格确实存在。

```c#
			else if (isDrag && riverMode == OptionalToggle.Yes) {
				HexCell otherCell = cell.GetNeighbor(dragDirection.Opposite());
				if (otherCell) {
					otherCell.SetOutgoingRiver(dragDirection);
				}
			}
```

我们现在能够编辑河流，尽管还不能看到它们。您可以通过使用调试检查器检查编辑的单元格来验证它是否有效。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-6/editing-rivers/cell-with-river-debug.png)

*具有河流，调试检视器的单元格。*

> **什么是调试检查器？**
>
> 您可以通过检查器的选项卡菜单在检查器的正常模式和调试模式之间切换。您可以通过选项卡右上角的图标访问它。在调试模式下，检查器显示原始对象数据。

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-6/editing-rivers/editing-rivers.unitypackage)

## 3 单元格间的河道

在对河流进行三角剖分时，我们必须考虑两个部分。这是河流的河道，还有流经其中的水。我们将首先修建河道，然后把水留到以后。

河流最简单的部分是流经单元格之间的连接处。目前，我们用三条四边形对该区域进行三角剖分。我们可以通过降低中间的四边形和增加两个通道墙来增加一个河道。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-6/river-channels-between-cells/edge-strip.png)

*在边缘地带加上一条河。*

如果是河流，这将需要两个额外的四边形，并产生一个有垂直墙的通道。另一种方法是始终使用四个四边形。然后我们可以降低中间的顶点，形成一个有倾斜墙壁的通道。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-6/river-channels-between-cells/edge-strip-four-quads.png)

*总是四个四边形。*

始终使用相同数量的四边形是方便的，所以让我们选择这个选项。

### 3.1 添加边顶点

从每条边三个四边形到四个四边形需要一个额外的边顶点。通过首先将 `v4` 重命名为 `v5`，然后将 `v3` 重命名为 `v4` 来重构 `EdgeVertices`。按此顺序执行可确保所有代码始终引用正确的顶点。为此，请使用编辑器的重命名或重构选项，以便在任何地方应用更改。否则，您将不得不手动遍历所有代码并进行调整。

```c#
	public Vector3 v1, v2, v4, v5;
```

重命名所有内容后，添加一个新的 `v3`。

```c#
	public Vector3 v1, v2, v3, v4, v5;
```

在构造函数中包含新顶点。它位于角顶点之间的一半。此外，其他顶点现在应该位于 ½ 和 ¾，而不是 ⅓ 和 ⅔。

```c#
	public EdgeVertices (Vector3 corner1, Vector3 corner2) {
		v1 = corner1;
		v2 = Vector3.Lerp(corner1, corner2, 0.25f);
		v3 = Vector3.Lerp(corner1, corner2, 0.5f);
		v4 = Vector3.Lerp(corner1, corner2, 0.75f);
		v5 = corner2;
	}
```

也将 `v3` 添加到 `TerraceLerp` 中。

```c#
	public static EdgeVertices TerraceLerp (
		EdgeVertices a, EdgeVertices b, int step)
	{
		EdgeVertices result;
		result.v1 = HexMetrics.TerraceLerp(a.v1, b.v1, step);
		result.v2 = HexMetrics.TerraceLerp(a.v2, b.v2, step);
		result.v3 = HexMetrics.TerraceLerp(a.v3, b.v3, step);
		result.v4 = HexMetrics.TerraceLerp(a.v4, b.v4, step);
		result.v5 = HexMetrics.TerraceLerp(a.v5, b.v5, step);
		return result;
	}
```

`HexMesh` 现在必须在其三角形边扇中包含额外的顶点。

```c#
	void TriangulateEdgeFan (Vector3 center, EdgeVertices edge, Color color) {
		AddTriangle(center, edge.v1, edge.v2);
		AddTriangleColor(color);
		AddTriangle(center, edge.v2, edge.v3);
		AddTriangleColor(color);
		AddTriangle(center, edge.v3, edge.v4);
		AddTriangleColor(color);
		AddTriangle(center, edge.v4, edge.v5);
		AddTriangleColor(color);
	}
```

还有它的四边形条带。

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
		AddQuad(e1.v4, e1.v5, e2.v4, e2.v5);
		AddQuadColor(c1, c2);
	}
```

![four](https://catlikecoding.com/unity/tutorials/hex-map/part-6/river-channels-between-cells/four-vertices.png) ![five](https://catlikecoding.com/unity/tutorials/hex-map/part-6/river-channels-between-cells/five-vertices.png)

*每条边四个顶点对五个顶点。*

### 3.2 河床高程

我们通过降低边的中间顶点来创建通道。这定义了河床的垂直位置。尽管每个单元的确切垂直位置都受到了干扰，但我们应该在具有相同高度的单元之间保持河床恒定。这确保了水不必向上游流动。此外，河床应足够低，即使在最垂直扰动的单元格下方也能保持，同时仍留有水的空间。

让我们在 `HexMetrics` 中定义这个偏移量，并将其表示为高程。一级偏移就足够了。

```c#
	public const float streamBedElevationOffset = -1f;
```

我们可以使用此度量向 `HexCell` 添加一个属性，以检索其河床的垂直位置。

```c#
	public float StreamBedY {
		get {
			return
				(elevation + HexMetrics.streamBedElevationOffset) *
				HexMetrics.elevationStep;
		}
	}
```

### 3.3 创建河道

当 `HexMesh` 对单元格的六个三角形部分之一进行三角剖分时，我们可以检测到是否有河流流过其边缘。如果是这样，我们现在可以将中间边缘顶点降低到河床的高度。

```c#
	void Triangulate (HexDirection direction, HexCell cell) {
		Vector3 center = cell.Position;
		EdgeVertices e = new EdgeVertices(
			center + HexMetrics.GetFirstSolidCorner(direction),
			center + HexMetrics.GetSecondSolidCorner(direction)
		);

		if (cell.HasRiverThroughEdge(direction)) {
			e.v3.y = cell.StreamBedY;
		}

		TriangulateEdgeFan(center, e, cell.Color);

		if (direction <= HexDirection.SE) {
			TriangulateConnection(direction, cell, e);
		}
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-6/river-channels-between-cells/adjusting-one-edge.png)

*调整中间边顶点。*

我们可以看到河流的最初痕迹，但我们也会在地形上看到洞。为了关闭它们，在对连接进行三角剖分时，我们还必须调整另一边。

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
			e1.v5 + bridge
		);

		if (cell.HasRiverThroughEdge(direction)) {
			e2.v3.y = neighbor.StreamBedY;
		}
		
		…
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-6/river-channels-between-cells/adjusting-both-edges.png)

*完整的边缘连接通道。*

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-6/river-channels-between-cells/river-channels-between-cells.unitypackage)

## 4 跨越单元格的河道

我们现在在单元格之间有了正确的河道。但是当一条河流流过一个单元格时，通道总是终止于它的中心。解决这个问题需要一些工作。让我们首先考虑一条河流从一侧直接流过一个单元格的情况。

如果没有河流，每个单元格部分都可以是一个简单的三角形扇形。但是当一条河流笔直地流过它时，我们必须插入一条河道。实际上，我们必须将中心顶点拉伸成一条线，从而将中间的两个三角形变成四边形。三角形扇形变成梯形。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-6/river-channels-across-cells/channel-across-triangle.png)

*把一个通道逼成三角形。*

这些通道将比通过单元格连接的通道长得多。当扰动顶点时，这将变得明显。因此，让我们通过在中心和边之间插入另一组边顶点，将梯形分成两段。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-6/river-channels-across-cells/triangulation.png)

*对通道进行三角剖分。*

因为有河流的三角剖分与没有河流的三角计算非常不同，所以让我们为它创建一个专用的方法。如果我们有河流，我们使用这种方法，否则我们继续使用三角扇。

```c#
	void Triangulate (HexDirection direction, HexCell cell) {
		Vector3 center = cell.Position;
		EdgeVertices e = new EdgeVertices(
			center + HexMetrics.GetFirstSolidCorner(direction),
			center + HexMetrics.GetSecondSolidCorner(direction)
		);

		if (cell.HasRiver) {
			if (cell.HasRiverThroughEdge(direction)) {
				e.v3.y = cell.StreamBedY;
				TriangulateWithRiver(direction, cell, center, e);
			}
		}
		else {
			TriangulateEdgeFan(center, e, cell.Color);
		}

		if (direction <= HexDirection.SE) {
			TriangulateConnection(direction, cell, e);
		}
	}
	
	void TriangulateWithRiver (
		HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e
	) {
	
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-6/river-channels-across-cells/holes.png)

*河流应该有的地方。*

为了更好地了解我们正在做的事情，现在禁用单元格扰动。

```c#
	public const float cellPerturbStrength = 0f; // 4f;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-6/river-channels-across-cells/unperturbed.png)

*不受干扰的顶点。*

### 4.1 直线三角剖分

为了在单元格部分创建一个笔直的通道，我们必须将中心拉伸成一条线。这条线需要与通道具有相同的宽度。我们可以通过从中心移动 ¼ 到上一部分的第一个角来找到左顶点。

```c#
	void TriangulateWithRiver (
		HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e
	) {
		Vector3 centerL = center +
			HexMetrics.GetFirstSolidCorner(direction.Previous()) * 0.25f;
	}
```

右顶点也是如此。在这种情况下，我们需要下一部分的第二个角。

```c#
		Vector3 centerL = center +
			HexMetrics.GetFirstSolidCorner(direction.Previous()) * 0.25f;
		Vector3 centerR = center +
			HexMetrics.GetSecondSolidCorner(direction.Next()) * 0.25f;
```

通过在中心和边之间创建边顶点，可以找到中线。

```c#
		EdgeVertices m = new EdgeVertices(
			Vector3.Lerp(centerL, e.v1, 0.5f),
			Vector3.Lerp(centerR, e.v5, 0.5f)
		);
```

接下来，调整中间边的中间顶点以及中心，使其成为通道底部。

```c#
		m.v3.y = center.y = e.v3.y;
```

现在我们可以使用 `TriangulateEdgeStrip` 来填充中线和边线之间的空间。

```c#
		TriangulateEdgeStrip(m, cell.Color, e, cell.Color);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-6/river-channels-across-cells/pinched-strip.png)

*被掐的河道。*

不幸的是，渠道似乎被挤压了。发生这种情况是因为中间边缘顶点靠得太近。为什么会发生这种情况？

如果我们考虑外边缘的长度为1，那么中心线的长度为½。由于中间边缘位于它们之间的一半，其长度必须为¾。

通道的宽度为½，应保持不变。由于中间边缘长度为¾，因此通道两侧仅剩下¼，即⅛。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-6/river-channels-across-cells/relative-lengths.png)

*相对长度。*

由于中间边缘的长度为 ¾，因此 ⅛ 相对于中间边缘长度变为 ⅙。这意味着它的第二个和第四个顶点应该使用六分之一而不是四分之一进行插值。

我们可以通过向 `EdgeVertices` 添加另一个构造函数来支持这种替代插值。让我们使用一个参数，而不是对 `v2` 和 `v4` 使用固定的插值。

```c#
	public EdgeVertices (Vector3 corner1, Vector3 corner2, float outerStep) {
		v1 = corner1;
		v2 = Vector3.Lerp(corner1, corner2, outerStep);
		v3 = Vector3.Lerp(corner1, corner2, 0.5f);
		v4 = Vector3.Lerp(corner1, corner2, 1f - outerStep);
		v5 = corner2;
	}
```

现在我们可以在 `HexMesh.TriangulateWithRiver` 中使用 ⅙。

```c#
		EdgeVertices m = new EdgeVertices(
			Vector3.Lerp(centerL, e.v1, 0.5f),
			Vector3.Lerp(centerR, e.v5, 0.5f),
			1f / 6f
		);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-6/river-channels-across-cells/straight-channels.png)

*直通道。*

河道拉直后，我们可以继续进行梯形的第二部分。在这种情况下，我们不能使用边缘条，我们必须手动操作。让我们先在边上创建三角形。

```c#
		AddTriangle(centerL, m.v1, m.v2);
		AddTriangleColor(cell.Color);
		AddTriangle(centerR, m.v4, m.v5);
		AddTriangleColor(cell.Color);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-6/river-channels-across-cells/side-triangles.png)

*边三角形。*

这看起来不错，所以用两个四边形填充剩余的空间，形成通道的最后一部分。

```c#
		AddTriangle(centerL, m.v1, m.v2);
		AddTriangleColor(cell.Color);
		AddQuad(centerL, center, m.v2, m.v3);
		AddQuadColor(cell.Color);
		AddQuad(center, centerR, m.v3, m.v4);
		AddQuadColor(cell.Color);
		AddTriangle(centerR, m.v4, m.v5);
		AddTriangleColor(cell.Color);
```

实际上，我们没有只需要一个参数的 `AddQuadColor` 替代方案。直到现在我们才需要一个。所以，就创造它吧。

```c#
	void AddQuadColor (Color color) {
		colors.Add(color);
		colors.Add(color);
		colors.Add(color);
		colors.Add(color);
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-6/river-channels-across-cells/complete-channels.png)

*完整的直通通道。*

### 4.2 三角剖分开始和结束

对只有河流起点或终点的部分进行三角剖分是足够不同的，因此需要采用自己的方法。因此，在 `Triangulate` 中检查它并调用适当的方法。

```c#
		if (cell.HasRiver) {
			if (cell.HasRiverThroughEdge(direction)) {
				e.v3.y = cell.StreamBedY;
				if (cell.HasRiverBeginOrEnd) {
					TriangulateWithRiverBeginOrEnd(direction, cell, center, e);
				}
				else {
					TriangulateWithRiver(direction, cell, center, e);
				}
			}
		}
```

在这种情况下，我们想在中心终止通道，但仍然需要两个步骤才能到达那里。因此，再次在中心和边之间创建一条中间边。因为我们确实想终止通道，所以它被压缩是可以的。

```c#
	void TriangulateWithRiverBeginOrEnd (
		HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e
	) {
		EdgeVertices m = new EdgeVertices(
			Vector3.Lerp(center, e.v1, 0.5f),
			Vector3.Lerp(center, e.v5, 0.5f)
		);
	}
```

为了确保河道不会变得太浅太快，我们仍然将中间顶点设置为河床高度。但中心不应该调整。

```c#
		m.v3.y = e.v3.y;
```

我们可以用一个边缘条和一个扇形进行三角剖分。

```c#
		TriangulateEdgeStrip(m, cell.Color, e, cell.Color);
		TriangulateEdgeFan(center, m, cell.Color);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-6/river-channels-across-cells/begin-and-end.png)

*起点和终点。*

### 4.3 一步转弯

接下来，让我们考虑急转弯，它在相邻的单元格之间呈之字形。我们也会在 `TriangulateWithRiver` 中处理这些问题。所以我们必须弄清楚我们正在使用哪种类型的河流。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-6/river-channels-across-cells/zigzag.png)

*曲折的河流。*

如果单元格中有一条河流穿过与我们工作的方向相反的方向，那么它一定是一条笔直的河流。在这种情况下，我们可以保持我们已经计算出的中心线。否则，让我们通过折叠中心线来恢复到单个点。

```c#
		Vector3 centerL, centerR;
		if (cell.HasRiverThroughEdge(direction.Opposite())) {
			centerL = center +
				HexMetrics.GetFirstSolidCorner(direction.Previous()) * 0.25f;
			centerR = center +
				HexMetrics.GetSecondSolidCorner(direction.Next()) * 0.25f;
		}
		else {
			centerL = centerR = center;
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-6/river-channels-across-cells/pinched-again.png)

*曲折的河道坍塌。*

我们可以通过检查单元格是否有河流穿过下一个或上一个单元格部分来检测急转弯。如果是这样，我们必须将中心线与该部分和相邻部分之间的边缘对齐。我们可以通过将线的适当一侧放置在中心和共享角之间的一半来实现这一点。然后，线的另一侧成为中心。

```c#
		if (cell.HasRiverThroughEdge(direction.Opposite())) {
			centerL = center +
				HexMetrics.GetFirstSolidCorner(direction.Previous()) * 0.25f;
			centerR = center +
				HexMetrics.GetSecondSolidCorner(direction.Next()) * 0.25f;
		}
		else if (cell.HasRiverThroughEdge(direction.Next())) {
			centerL = center;
			centerR = Vector3.Lerp(center, e.v5, 0.5f);
		}
		else if (cell.HasRiverThroughEdge(direction.Previous())) {
			centerL = Vector3.Lerp(center, e.v1, 0.5f);
			centerR = center;
		}
		else {
			centerL = centerR = center;
		}
```

在确定左右点的位置后，我们可以通过求平均值来确定最终的中心。

```c#
		if (cell.HasRiverThroughEdge(direction.Opposite())) {
			…
		}		
		center = Vector3.Lerp(centerL, centerR, 0.5f);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-6/river-channels-across-cells/twisted-center-edge.png)

*扭曲的中心边缘。*

虽然通道两侧的宽度相同，但看起来很窄。这是由中心线旋转 60° 引起的。这可以通过稍微增加中心线的宽度来缓解。使用 ⅔ 代替 ½ 进行插值。

```c#
		else if (cell.HasRiverThroughEdge(direction.Next())) {
			centerL = center;
			centerR = Vector3.Lerp(center, e.v5, 2f / 3f);
		}
		else if (cell.HasRiverThroughEdge(direction.Previous())) {
			centerL = Vector3.Lerp(center, e.v1, 2f / 3f);
			centerR = center;
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-6/river-channels-across-cells/zigzag-without-pinch.png)

*曲折无挤压。*

### 4.4 两步转弯

其余的案例位于曲折和笔直的河流之间。它们是两步旋转，产生平缓弯曲的河流。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-6/river-channels-across-cells/curves.png)

*弯曲的河流。*

为了区分这两种可能的方向，我们必须使用 `direction.Next().Next()`。但是，让我们通过向 `HexDirection` 添加 `Next2` 和 `Prevous2` 扩展方法来让它更方便一些。

```c#
	public static HexDirection Previous2 (this HexDirection direction) {
		direction -= 2;
		return direction >= HexDirection.NE ? direction : (direction + 6);
	}

	public static HexDirection Next2 (this HexDirection direction) {
		direction += 2;
		return direction <= HexDirection.NW ? direction : (direction - 6);
	}
```

回到 `HexMesh.TriangulateWithRiver`，我们现在可以通过使用方向来检测弯曲河流的 `direction.Next2()`。

```c#
		if (cell.HasRiverThroughEdge(direction.Opposite())) {
			centerL = center +
				HexMetrics.GetFirstSolidCorner(direction.Previous()) * 0.25f;
			centerR = center +
				HexMetrics.GetSecondSolidCorner(direction.Next()) * 0.25f;
		}
		else if (cell.HasRiverThroughEdge(direction.Next())) {
			centerL = center;
			centerR = Vector3.Lerp(center, e.v5, 2f / 3f);
		}
		else if (cell.HasRiverThroughEdge(direction.Previous())) {
			centerL = Vector3.Lerp(center, e.v1, 2f / 3f);
			centerR = center;
		}
		else if (cell.HasRiverThroughEdge(direction.Next2())) {
			centerL = centerR = center;
		}
		else {
			centerL = centerR = center;
		}
```

在最后两种情况下，我们必须将中心线推入位于曲线内侧的单元格部分。如果我们有一个指向实心边中间的向量，我们可以用它来定位端点。让我们假设我们有一种方法。

```c#
		else if (cell.HasRiverThroughEdge(direction.Next2())) {
			centerL = center;
			centerR = center +
				HexMetrics.GetSolidEdgeMiddle(direction.Next()) * 0.5f;
		}
		else {
			centerL = center +
				HexMetrics.GetSolidEdgeMiddle(direction.Previous()) * 0.5f;
			centerR = center;
		}
```

当然，现在我们必须在 `HexMetrics` 中添加这样的方法。它只需对两个相邻的角向量进行平均，并应用实因子。

```c#
	public static Vector3 GetSolidEdgeMiddle (HexDirection direction) {
		return
			(corners[(int)direction] + corners[(int)direction + 1]) *
			(0.5f * solidFactor);
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-6/river-channels-across-cells/pinched-curves.png)

*略微紧缩的曲线。*

我们的中心线现在正确地旋转了 30°。但它们不够长，导致通道略微紧缩。这是因为边中间比边角更靠近中心。它的距离等于内部实体半径，而不是外部实体半径。因此，我们采用了错误的比例。

我们已经在 `HexMetrics` 中执行了从外半径到内半径的转换。我们这里需要的是与之相反的东西。因此，让我们通过 `HexMetrics` 提供这两个转换因子。

```c#
	public const float outerToInner = 0.866025404f;
	public const float innerToOuter = 1f / outerToInner;

	public const float outerRadius = 10f;

	public const float innerRadius = outerRadius * outerToInner;
```

现在我们可以在 `HexMesh.TriangulateWithRiver` 中转换为正确的比例。由于它们的旋转，通道仍然会受到一些挤压，但远没有之字形那么极端。所以我们不必对此进行补偿。

```c#
		else if (cell.HasRiverThroughEdge(direction.Next2())) {
			centerL = center;
			centerR = center +
				HexMetrics.GetSolidEdgeMiddle(direction.Next()) *
				(0.5f * HexMetrics.innerToOuter);
		}
		else {
			centerL = center +
				HexMetrics.GetSolidEdgeMiddle(direction.Previous()) *
				(0.5f * HexMetrics.innerToOuter);
			centerR = center;
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-6/river-channels-across-cells/smooth-curves.png)

*平滑曲线。*

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-6/river-channels-across-cells/river-channels-across-cells.unitypackage)

## 5 在河流附近进行三角剖分

我们的河道现已完工。但我们还没有对包含河流的单元格的其他部分进行三角剖分。我们现在要堵住这些洞。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-6/triangulating-adjacent-to-rivers/holes-alongside-channels.png)

*通道旁边的孔。*

在 `Triangulate` 中，当单元格有一条河，但它不流过当前方向时，调用一个新方法。

```c#
		if (cell.HasRiver) {
			if (cell.HasRiverThroughEdge(direction)) {
				e.v3.y = cell.StreamBedY;
				if (cell.HasRiverBeginOrEnd) {
					TriangulateWithRiverBeginOrEnd(direction, cell, center, e);
				}
				else {
					TriangulateWithRiver(direction, cell, center, e);
				}
			}
			else {
				TriangulateAdjacentToRiver(direction, cell, center, e);
			}
		}
		else {
			TriangulateEdgeFan(center, e, cell.Color);
		}
```

在此方法中，用条带和扇形填充单元格三角形。我们不能满足于一个风扇，因为我们必须确保与包含河流的部分的中间边缘相匹配。

```c#
	void TriangulateAdjacentToRiver (
		HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e
	) {
		EdgeVertices m = new EdgeVertices(
			Vector3.Lerp(center, e.v1, 0.5f),
			Vector3.Lerp(center, e.v5, 0.5f)
		);
		
		TriangulateEdgeStrip(m, cell.Color, e, cell.Color);
		TriangulateEdgeFan(center, m, cell.Color);
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-6/triangulating-adjacent-to-rivers/filled-holes.png)

*在弯曲和笔直的河流上重叠。*

### 5.1 匹配河道

当然，我们必须确保我们使用的中心与河流部分使用的中心线相匹配。曲折很好，但弯曲和笔直的河流需要一些工作。因此，我们必须确定我们拥有哪种河流，以及它的相对方向。

让我们先检查一下我们是否在曲线的内侧。当前一个和下一个方向都包含河流时，情况就是这样。如果是这样，我们就必须将中心移向边缘。

```c#
		if (cell.HasRiverThroughEdge(direction.Next())) {
			if (cell.HasRiverThroughEdge(direction.Previous())) {
				center += HexMetrics.GetSolidEdgeMiddle(direction) *
					(HexMetrics.innerToOuter * 0.5f);
			}
		}

		EdgeVertices m = new EdgeVertices(
			Vector3.Lerp(center, e.v1, 0.5f),
			Vector3.Lerp(center, e.v5, 0.5f)
		);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-6/triangulating-adjacent-to-rivers/river-on-both-sides.png)

*修复了河流两岸流动时的问题。*

如果我们在下一个方向有一条河，但在上一个方向没有，那么检查它是否是一条直河。如果是这样，我们将不得不把中心移向我们的第一个坚实的角落。

```c#
		if (cell.HasRiverThroughEdge(direction.Next())) {
			if (cell.HasRiverThroughEdge(direction.Previous())) {
				center += HexMetrics.GetSolidEdgeMiddle(direction) *
					(HexMetrics.innerToOuter * 0.5f);
			}
			else if (
				cell.HasRiverThroughEdge(direction.Previous2())
			) {
				center += HexMetrics.GetFirstSolidCorner(direction) * 0.25f;
			}
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-6/triangulating-adjacent-to-rivers/along-straight-river.png)

*固定了与直河重叠的一半。*

这解决了与直河相邻的一半地区的问题。最后一种情况是，当我们在前面的方向上有一条河，而且是直的。这需要将中心移向下一个坚实的角落。

```c#
		if (cell.HasRiverThroughEdge(direction.Next())) {
			if (cell.HasRiverThroughEdge(direction.Previous())) {
				center += HexMetrics.GetSolidEdgeMiddle(direction) *
					(HexMetrics.innerToOuter * 0.5f);
			}
			else if (
				cell.HasRiverThroughEdge(direction.Previous2())
			) {
				center += HexMetrics.GetFirstSolidCorner(direction) * 0.25f;
			}
		}
		else if (
			cell.HasRiverThroughEdge(direction.Previous()) &&
			cell.HasRiverThroughEdge(direction.Next2())
		) {
			center += HexMetrics.GetSecondSolidCorner(direction) * 0.25f;
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-6/triangulating-adjacent-to-rivers/no-more-mismatch.png)

*不再有重叠。*

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-6/triangulating-adjacent-to-rivers/triangulating-adjacent-to-rivers.unitypackage)

## 6 HexMesh 的推广

我们河道的三角剖分完成了！现在我们可以把它们装满水。因为水和陆地有很大不同，所以我们必须使用不同的网格、不同的顶点数据和不同的材质。如果我们能在陆地和水上都使用 `HexMesh`，那将非常方便。因此，让我们推广 `HexMesh`，将其转化为一个专用于网格数据的类，而不必关心它的确切用途。`HexGridChunk` 将接管对其单元格进行三角剖分的责任。

### 6.1 移动摄动（Perturb）方法

因为 `Perturb` 方法非常通用，并且将在多个地方使用，所以让我们将其移动到 `HexMetrics`。首先，将其名称重构为 `HexMetrics.Perturb`。这是一个无效的方法名，但它重构了所有代码以正确访问它。如果你的编辑器具有移动方法的特殊功能，你可以使用它。

一旦该方法进入 `HexMetrics`，将其设置为公共和静态，并固定其名称。

```c#
	public static Vector3 Perturb (Vector3 position) {
		Vector4 sample = /*HexMetrics.*/SampleNoise(position);
		position.x += (sample.x * 2f - 1f) * /*HexMetrics.*/cellPerturbStrength;
		position.z += (sample.z * 2f - 1f) * /*HexMetrics.*/cellPerturbStrength;
		return position;
	}
```

### 6.2 移动三角剖分方法

在 `HexGridChunk` 中，将 `hexMesh` 变量替换为公共 `terrain` 变量。

```c#
	public HexMesh terrain;
//	HexMesh hexMesh;

	void Awake () {
		gridCanvas = GetComponentInChildren<Canvas>();
//		hexMesh = GetComponentInChildren<HexMesh>();

		cells = new HexCell[HexMetrics.chunkSizeX * HexMetrics.chunkSizeZ];
		ShowUI(false);
	}
```

接下来，将 `HexMesh` 中的所有 `Add…` 方法重构为 `terrain.Add…`。然后将所有 `Triangulate…` 方法移动到 `HexGridChunk`。完成后，您可以修复 `HexMesh` 中的 `Add…` 方法名称并将其公开。结果是，所有复杂的三角剖分方法现在都在 `HexGridChunk` 中，而向网格添加内容的简单方法仍保留在 `HexMesh` 中。

我们还没做完。`HexGridChunk.LateUpdate` 现在必须调用自己的 `Triangulate` 方法。此外，它不再需要将单元格作为参数传递。因此，三角剖分可能会失去其参数。它应该将网格数据的清除和应用委托给 `HexMesh`。

```c#
	void LateUpdate () {
		Triangulate();
//		hexMesh.Triangulate(cells);
		enabled = false;
	}
	
	public void Triangulate (HexCell[] cells) {
		terrain.Clear();
//		hexMesh.Clear();
//		vertices.Clear();
//		colors.Clear();
//		triangles.Clear();
		for (int i = 0; i < cells.Length; i++) {
			Triangulate(cells[i]);
		}
		terrain.Apply();
//		hexMesh.vertices = vertices.ToArray();
//		hexMesh.colors = colors.ToArray();
//		hexMesh.triangles = triangles.ToArray();
//		hexMesh.RecalculateNormals();
//		meshCollider.sharedMesh = hexMesh;
	}
```

将所需的 `Clear` 和 `Apply` 方法添加到 `HexMesh`。

```c#
	public void Clear () {
		hexMesh.Clear();
		vertices.Clear();
		colors.Clear();
		triangles.Clear();
	}

	public void Apply () {
		hexMesh.SetVertices(vertices);
		hexMesh.SetColors(colors);
		hexMesh.SetTriangles(triangles, 0);
		hexMesh.RecalculateNormals();
		meshCollider.sharedMesh = hexMesh;
	}
```

> **`SetVertices`、`SetColors` 和 `SetTriangles` 是怎么回事？**
>
> 这些方法是 `Mesh` 类中最近添加的。它们允许您直接使用列表设置网格数据。这意味着在更新网格时，我们不再需要创建临时数组。

`SetTriangles` 方法有第二个整数参数，即子网格索引。由于我们不使用子网格，因此它始终为零。

最后，手动连接块预制件中的网格子对象。我们不能再自动执行此操作，因为我们很快就会添加第二个网格子节点。此外，将其重命名为 `Terrain` 以指示其用途。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-6/generalizing-hexmesh/assigning-terrain.png)

*指定地形。*

> **重命名预制件的子对象不起作用？**
>
> 更改预制件子对象的名称时，项目视图不会更新。您可以通过创建预制件的实例来更新它。调整实例，然后使用“应用”按钮将这些更改也推到预制件上。这是目前调整预制件对象层次结构的最佳方法。

### 6.3 池列表

尽管我们移动了相当多的代码，但我们的地图仍应像以前一样运行。为每个块添加另一个网格不应该改变这一点。但如果我们用当前的 `HexMesh` 这样做，就会出错。

问题是，我们假设一次只处理一个网格。这允许我们使用静态列表来存储临时网格数据。但是一旦我们加水，我们将同时处理两个网格。因此，我们不能再使用静态列表。

但是，我们不必为每个 `HexMesh` 实例返回一组列表。相反，我们可以使用静态列表池。默认情况下没有这样的池，所以让我们自己创建一个通用的列表池类。

```c#
public static class ListPool<T> {

}
```

> **`ListPool<T>` 是如何工作的？**
>
> 我们已经使用了很多泛型列表，比如 `List<int>` 用于整数列表。通过在 `ListPool` 类的声明后放置 `<T>`，我们表明它是一个泛型类。我们可以为通用部分使用任何名称，但通常只使用 `T` 作为模板。

我们可以使用堆栈来存储池列表的集合。我通常不使用堆栈，因为 Unity 不会序列化它们，但在这种情况下这并不重要。

```c#
using System.Collections.Generic;

public static class ListPool<T> {

	static Stack<List<T>> stack = new Stack<List<T>>();
}
```

> **`Stack<List<T>>` 是什么意思？**
>
> 这是嵌套泛型类型的一个例子。这意味着我们想要一堆列表。列表中的内容取决于池。

添加一个公共静态方法以从池中获取列表。如果堆栈不是空的，我们将弹出顶部列表并返回该列表。否则，我们会当场创建一个新列表。

```c#
	public static List<T> Get () {
		if (stack.Count > 0) {
			return stack.Pop();
		}
		return new List<T>();
	}
```

要真正重用列表，我们必须在完成列表后将其添加到池中。`ListPool` 将负责清除列表，然后将其推送到堆栈上。

```c#
	public static void Add (List<T> list) {
		list.Clear();
		stack.Push(list);
	}
```

现在我们可以在 `HexMesh` 中使用我们的池。将静态列表替换为非静态私有引用。将它们标记为 `NonSerialized`，这样 Unity 在重新编译时就不会费心保存它们。要么写 `System.NonSerialized` 或使用系统添加 `using System;` 在脚本的顶部。

```c#
	[NonSerialized] List<Vector3> vertices;
	[NonSerialized] List<Color> colors;
	[NonSerialized] List<int> triangles;

//	static List<Vector3> vertices = new List<Vector3>();
//	static List<Color> colors = new List<Color>();
//	static List<int> triangles = new List<int>();
```

由于网格在添加新数据之前已被清除，因此这里是从我们的池中获取列表的地方。

```c#
	public void Clear () {
		hexMesh.Clear();
		vertices = ListPool<Vector3>.Get();
		colors = ListPool<Color>.Get();
		triangles = ListPool<int>.Get();
	}
```

在应用网格数据后，我们不再需要它们，所以我们可以将它们添加到那里的池中。

```c#
	public void Apply () {
		hexMesh.SetVertices(vertices);
		ListPool<Vector3>.Add(vertices);
		hexMesh.SetColors(colors);
		ListPool<Color>.Add(colors);
		hexMesh.SetTriangles(triangles, 0);
		ListPool<int>.Add(triangles);
		hexMesh.RecalculateNormals();
		meshCollider.sharedMesh = hexMesh;
	}
```

无论我们同时填充多少个网格，这都能保证列表的重用。

### 6.4 可选碰撞器

虽然我们的地形需要一个碰撞体，但我们的河流并不需要。光线投射将简单地穿过水面，击中下面的河道。因此，让我们配置 `HexMesh` 是否有碰撞体。通过添加一个公共 `bool useCollider` 字段来实现这一点。为地形打开它。

```c#
	public bool useCollider;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-6/generalizing-hexmesh/use-collider.png)

*使用网格碰撞器。*

我们所要做的就是确保我们只在打开碰撞体时创建和分配碰撞体。

```c#
	void Awake () {
		GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
		if (useCollider) {
			meshCollider = gameObject.AddComponent<MeshCollider>();
		}
		hexMesh.name = "Hex Mesh";
	}

	public void Apply () {
		…
		if (useCollider) {
			meshCollider.sharedMesh = hexMesh;
		}

		…
	}
```

### 6.5 可选颜色

顶点颜色也可以是可选的。我们需要它们显示不同的地形类型，但水不会改变颜色。我们可以将它们设置为可选，就像我们将碰撞体设置为可选一样。

```c#
	public bool useCollider, useColors;

	public void Clear () {
		hexMesh.Clear();
		vertices = ListPool<Vector3>.Get();
		if (useColors) {
			colors = ListPool<Color>.Get();
		}
		triangles = ListPool<int>.Get();
	}

	public void Apply () {
		hexMesh.SetVertices(vertices);
		ListPool<Vector3>.Add(vertices);
		if (useColors) {
			hexMesh.SetColors(colors);
			ListPool<Color>.Add(colors);
		}
		…
	}
```

当然，地形确实使用顶点颜色，所以一定要打开它们。

*使用颜色。*

### 6.6 可选 UV

在此过程中，我们还可以添加对可选 UV 坐标的支持。虽然我们的地形不使用它们，但我们需要它们来取水。

```c#
	public bool useCollider, useColors, useUVCoordinates;

	[NonSerialized] List<Vector2> uvs;

	public void Clear () {
		hexMesh.Clear();
		vertices = ListPool<Vector3>.Get();
		if (useColors) {
			colors = ListPool<Color>.Get();
		}
		if (useUVCoordinates) {
			uvs = ListPool<Vector2>.Get();
		}
		triangles = ListPool<int>.Get();
	}

	public void Apply () {
		hexMesh.SetVertices(vertices);
		ListPool<Vector3>.Add(vertices);
		if (useColors) {
			hexMesh.SetColors(colors);
			ListPool<Color>.Add(colors);
		}
		if (useUVCoordinates) {
			hexMesh.SetUVs(0, uvs);
			ListPool<Vector2>.Add(uvs);
		}
		…
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-6/generalizing-hexmesh/not-using-uvs.png)

*不使用 UV 坐标。*

为了使其有用，请创建方法来添加三角形和四边形的 UV 坐标。

```c#
	public void AddTriangleUV (Vector2 uv1, Vector2 uv2, Vector2 uv3) {
		uvs.Add(uv1);
		uvs.Add(uv2);
		uvs.Add(uv3);
	}
	
	public void AddQuadUV (Vector2 uv1, Vector2 uv2, Vector2 uv3, Vector2 uv4) {
		uvs.Add(uv1);
		uvs.Add(uv2);
		uvs.Add(uv3);
		uvs.Add(uv4);
	}
```

让我们添加一个额外的 `AddQuadUV` 方法，以便方便地添加矩形 UV 区域。这是四边形及其纹理对齐的典型情况，我们的河水也是如此。

```c#
	public void AddQuadUV (float uMin, float uMax, float vMin, float vMax) {
		uvs.Add(new Vector2(uMin, vMin));
		uvs.Add(new Vector2(uMax, vMin));
		uvs.Add(new Vector2(uMin, vMax));
		uvs.Add(new Vector2(uMax, vMax));
	}
```

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-6/generalizing-hexmesh/generalizing-hexmesh.unitypackage)

## 7 流动的河流

终于到了造水的时候了！我们将使用代表水面的四边形来实现这一点。因为我们正在与河流合作，所以水必须流动。我们将使用 UV 坐标来指示河流的方向。为了可视化这一点，我们需要一个新的着色器。因此，创建一个新的标准着色器并将其命名为 `River`。调整它，使 UV 坐标位于红色和绿色反照率通道中。

```glsl
Shader "Custom/River" {
	…

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb * IN.color;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
			o.Albedo.rg = IN.uv_MainTex;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
```

在 `HexGridChunk` 中添加一个公共 `HexMesh rivers` 字段。清除并应用它，就像地形一样。

```c#
	public HexMesh terrain, rivers;
	
	public void Triangulate () {
		terrain.Clear();
		rivers.Clear();
		for (int i = 0; i < cells.Length; i++) {
			Triangulate(cells[i]);
		}
		terrain.Apply();
		rivers.Apply();
	}
```

> **即使我们没有河流，我们也会收到额外的抽签通知吗？**
>
> Unity 足够聪明，不会费心绘制空网格。因此，只有在有东西可看的情况下，才会绘制河流网格。

通过实例调整预制件，复制其地形对象，将其重命名为 `Rivers`，并将其连接起来。

![prefab](https://catlikecoding.com/unity/tutorials/hex-map/part-6/flowing-rivers/prefab-project.png) ![component](https://catlikecoding.com/unity/tutorials/hex-map/part-6/flowing-rivers/prefab-inspector.png)

*预制的有河流的块。*

创建一个使用我们新着色器的 `River` 材质，并确保 `Rivers` 对象使用它。还配置对象的六边形网格组件，使其使用 UV 坐标，但不使用顶点颜色，也不使用碰撞器。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-6/flowing-rivers/rivers-object.png)

*河流子对象。*

### 7.1 三角剖分水

在我们对水进行三角剖分之前，我们必须确定它的表面水平。让我们在 `HexMetrics` 中将其设置为高程偏移，就像河床一样。因为单元格的垂直扰动设置为半个高程偏移，所以让我们也将其用作河流表面偏移。这确保了水永远不会超过单元格的地形。

```c#
	public const float riverSurfaceElevationOffset = -0.5f;
```

> **它不应该稍微低一点吗？**
>
> 随机扰动实际上从未达到最大值，所以我们很好。当然，如果你愿意，你仍然可以把水面再降低一点。

向 `HexCell` 添加属性以检索其河流表面的垂直位置。

```c#
	public float RiverSurfaceY {
		get {
			return
				(elevation + HexMetrics.riverSurfaceElevationOffset) *
				HexMetrics.elevationStep;
		}
	}
```

现在我们可以在 `HexGridChunk` 中工作了！因为我们要创建多个河流四边形，所以让我们为此添加一个专用方法。给它四个顶点加一个高度作为参数。这允许它在添加四边形之前方便地一次设置所有四个顶点的垂直位置。

```c#
	void TriangulateRiverQuad (
		Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4,
		float y
	) {
		v1.y = v2.y = v3.y = v4.y = y;
		rivers.AddQuad(v1, v2, v3, v4);
	}
```

我们也将在此处添加四边形的 UV 坐标。我们只需从左到右，从下到上。

```c#
		rivers.AddQuad(v1, v2, v3, v4);
		rivers.AddQuadUV(0f, 1f, 0f, 1f);
```

`TriangulateWithRiver` 是我们将添加河流四边形的第一种方法。第一个四边形位于中心和中间之间。第二个位于中间和边缘之间。我们将简单地使用我们已经拥有的顶点。因为这些顶点将降低，所以水将部分地流到倾斜的通道壁下方。所以我们不必担心水边的确切位置。

```c#
	void TriangulateWithRiver (
		HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e
	) {
		…

		TriangulateRiverQuad(centerL, centerR, m.v2, m.v4, cell.RiverSurfaceY);
		TriangulateRiverQuad(m.v2, m.v4, e.v2, e.v4, cell.RiverSurfaceY);
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-6/flowing-rivers/first-water.png)

*水的最初迹象。*

> **为什么水的宽度会变化？**
>
> 这是因为单元格的高度受到干扰，而其河床和河面则没有。单元格高度越高，通道壁越陡越高。这使得水面处的河流变窄。

### 7.2 随波逐流

目前，UV 坐标与河流的流向不一致。我们必须保持一致。假设从下游看，U 坐标在河流左侧为 0，在右侧为 1。V坐标应该在河流流动的方向上从 0 到 1。

使用上述规范，我们的 UV 在对流出的河流进行三角剖分时是正确的。它们是错误的，在对流入的河流进行三角剖分时必须颠倒过来。为了便于实现这一点，请在 `TriangulateIverQuad` 中添加一个 `bool reversed` 参数。需要时使用它来反转 UV。

```c#
	void TriangulateRiverQuad (
		Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4,
		float y, bool reversed
	) {
		v1.y = v2.y = v3.y = v4.y = y;
		rivers.AddQuad(v1, v2, v3, v4);
		if (reversed) {
			rivers.AddQuadUV(1f, 0f, 1f, 0f);
		}
		else {
			rivers.AddQuadUV(0f, 1f, 0f, 1f);
		}
	}
```

在 `TriangulateWithRiver` 中，我们知道当我们处理一条流入的河流时，我们必须改变方向。

```c#
		bool reversed = cell.IncomingRiver == direction;
		TriangulateRiverQuad(
			centerL, centerR, m.v2, m.v4, cell.RiverSurfaceY, reversed
		);
		TriangulateRiverQuad(
			m.v2, m.v4, e.v2, e.v4, cell.RiverSurfaceY, reversed
		);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-6/flowing-rivers/consistent-direction.png)

*河流方向一致。*

### 7.3 河流开始和结束

在 `TriangulateWithRiverBeginOrEnd` 中，我们只需检查是否有河流流入，即可确定流向。然后我们可以在中间和边缘之间插入另一个河流四边形。

```c#
	void TriangulateWithRiverBeginOrEnd (
		HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e
	) {
		…

		bool reversed = cell.HasIncomingRiver;
		TriangulateRiverQuad(
			m.v2, m.v4, e.v2, e.v4, cell.RiverSurfaceY, reversed
		);
	}
```

中心和中间之间的部分是一个三角形，因此我们不能使用 `TrianglateIverQuad`。唯一显著的区别是中心顶点位于河流的中间。所以它的 U 坐标总是 ½。

```c#
		center.y = m.v2.y = m.v4.y = cell.RiverSurfaceY;
		rivers.AddTriangle(center, m.v2, m.v4);
		if (reversed) {
			rivers.AddTriangleUV(
				new Vector2(0.5f, 1f), new Vector2(1f, 0f), new Vector2(0f, 0f)
			);
		}
		else {
			rivers.AddTriangleUV(
				new Vector2(0.5f, 0f), new Vector2(0f, 1f), new Vector2(1f, 1f)
			);
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-6/flowing-rivers/begin-and-end.png)

*水在开始和结束。*

> **末端是否缺少一些水？**
>
> 因为四边形是由两个三角形组成的，所以当四边形不平坦时，它们的形状取决于它们的方向。因此，河流两侧河道壁的三角剖分是不对称的。在水面与渠道壁相交的地方，这一点变得更加明显。
>
> 你可以通过镜像四边形来消除这种差异。然而，这只是非常明显的，因为我们现在没有扰动顶点。一旦我们这样做，对称性无论如何都会被打破。

### 7.4 单元格间流动

在单元格之间加水时，我们必须注意海拔差异。为了让水顺着斜坡和悬崖流下，`TriangulateRiverQuad` 必须支持两个高度参数。那么，让我们添加第二个。

```c#
	void TriangulateRiverQuad (
		Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4,
		float y1, float y2, bool reversed
	) {
		v1.y = v2.y = y1;
		v3.y = v4.y = y2;
		rivers.AddQuad(v1, v2, v3, v4);
		if (reversed) {
			rivers.AddQuadUV(1f, 0f, 1f, 0f);
		}
		else {
			rivers.AddQuadUV(0f, 1f, 0f, 1f);
		}
	}
```

为了方便起见，我们还添加一个仍然接受一个高度的变体。它只是调用另一种方法。

```c#
	void TriangulateRiverQuad (
		Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4,
		float y, bool reversed
	) {
		TriangulateRiverQuad(v1, v2, v3, v4, y, y, reversed);
	}
```

现在我们也可以在 `TriangulateConnection` 中添加一个河流四边形。在单元格之间，我们无法立即知道我们正在处理的是哪种河流。为了确定我们是否需要倒车，我们必须检查是否有河流流入，以及它是否朝我们的方向前进。

```c#
		if (cell.HasRiverThroughEdge(direction)) {
			e2.v3.y = neighbor.StreamBedY;
			TriangulateRiverQuad(
				e1.v2, e1.v4, e2.v2, e2.v4,
				cell.RiverSurfaceY, neighbor.RiverSurfaceY,
				cell.HasIncomingRiver && cell.IncomingRiver == direction
			);
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-6/flowing-rivers/complete-river.png)

*完整的河流。*

### 7.5 拉伸 V 坐标

目前，我们在每个河段的 V 坐标从 0 到 1。所以每个单元格有四次。五次，如果我们也包括单元格之间的联系。无论我们用什么来给这条河上色，它都会重复很多次。

我们可以通过拉伸 V 坐标来减少这种重复，使它们在整个单元格中从 0 变为 1，再加上一个连接。这可以通过将 V 坐标每段增加 0.2 来实现。如果我们把 0.4 放在中心，它在中间变成 0.6，在边缘达到 0.8。然后，单元连接将其设置为 1。

如果河流流向相反，我们仍然将 0.4 放在中心，但它在中间变成 0.2，在边缘变成 0。如果我们继续进行蜂窝连接，它将在 -0.2 处结束。这很好，因为对于过滤器模式设置为重复的纹理，这相当于 0.8，就像 0 相当于 1 一样。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-6/flowing-rivers/v-flow.png)

*V 坐标流。*

为了支持这一点，我们必须向 `TriangulateRiverQuad` 添加另一个参数。

```c#
	void TriangulateRiverQuad (
		Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4,
		float y, float v, bool reversed
	) {
		TriangulateRiverQuad(v1, v2, v3, v4, y, y, v, reversed);
	}

	void TriangulateRiverQuad (
		Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4,
		float y1, float y2, float v, bool reversed
	) {
		…
	}
```

当方向没有反转时，我们只需使用四边形底部提供的坐标，并在其顶部添加 0.2。

```c#
		else {
			rivers.AddQuadUV(0f, 1f, v, v + 0.2f);
		}
```

我们可以通过从 0.8 和 0.6 中减去坐标来处理反转的（reversed）方向。

```c#
		if (reversed) {
			rivers.AddQuadUV(1f, 0f, 0.8f - v, 0.6f - v);
		}
```

现在我们必须提供正确的坐标，就像我们在处理一条外流的河流一样。首先在 `TriangulateWithRiver`。

```c#
		TriangulateRiverQuad(
			centerL, centerR, m.v2, m.v4, cell.RiverSurfaceY, 0.4f, reversed
		);
		TriangulateRiverQuad(
			m.v2, m.v4, e.v2, e.v4, cell.RiverSurfaceY, 0.6f, reversed
		);
```

然后在 `TriangulateConnection` 中。

```c#
			TriangulateRiverQuad(
				e1.v2, e1.v4, e2.v2, e2.v4,
				cell.RiverSurfaceY, neighbor.RiverSurfaceY, 0.8f,
				cell.HasIncomingRiver && cell.IncomingRiver == direction
			);
```

最后是 `TriangulateWithRiverBeginOrEnd`。

```c#
		TriangulateRiverQuad(
			m.v2, m.v4, e.v2, e.v4, cell.RiverSurfaceY, 0.6f, reversed
		);
		center.y = m.v2.y = m.v4.y = cell.RiverSurfaceY;
		rivers.AddTriangle(center, m.v2, m.v4);
		if (reversed) {
			rivers.AddTriangleUV(
				new Vector2(0.5f, 0.4f),
				new Vector2(1f, 0.2f), new Vector2(0f, 0.2f)
			);
		}
		else {
			rivers.AddTriangleUV(
				new Vector2(0.5f, 0.4f),
				new Vector2(0f, 0.6f), new Vector2(1f, 0.6f)
			);
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-6/flowing-rivers/stretched-v.png)

*拉伸 V 坐标。*

要正确查看 V 坐标环绕，请确保它在河流着色器中保持正值。

```c#
			if (IN.uv_MainTex.y < 0) {
				IN.uv_MainTex.y += 1;
			}
			o.Albedo.rg = IN.uv_MainTex;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-6/flowing-rivers/v-wrap.png)

*包裹的 V 坐标。*

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-6/flowing-rivers/flowing-rivers.unitypackage)

## 8 河流动画

在处理完 UV 坐标后，我们可以继续为河流设置动画。河流着色器会处理这个问题，所以我们不必不断更新网格。

在本教程中，我们不会创建花哨的河流着色器，稍后会介绍。现在，我们将用一个简单的效果来让您了解动画是如何工作的。

通过根据播放时间滑动 V 坐标来创建动画。Unity 通过 `_Time` 变量提供此功能。它的 Y 分量包含未修改的时间，我们将使用它。它的其他组成部分包含不同的时间尺度。

去掉 V 包装，因为我们不再需要它。相反，从 V 坐标中减去当前时间。这会使坐标向下滑动，从而产生河流向前流动的错觉。

```c#
//			if (IN.uv_MainTex.y < 0) {
//				IN.uv_MainTex.y += 1;
//			}
			IN.uv_MainTex.y -= _Time.y;
			o.Albedo.rg = IN.uv_MainTex;
```

一秒钟后，V 坐标将处处低于零，因此我们将不再看到差异。同样，由于重复纹理过滤模式，这很好。但要了解发生了什么，我们可以取 V 坐标的分数部分。

```c#
			IN.uv_MainTex.y -= _Time.y;
			IN.uv_MainTex.y = frac(IN.uv_MainTex.y);
			o.Albedo.rg = IN.uv_MainTex;
```

*动画 V 坐标。*

### 8.1 使用噪音

我们的河流现在充满活力，但在方向和速度上都有剧烈的转变。我们的 UV 模式使这一点非常明显，但使用更像水的模式时，更难检测到。因此，让我们对纹理进行采样，而不是显示原始 UV。我们可以使用已有的噪波纹理。对其进行采样，并将材质的颜色乘以第一个噪声通道。

```c#
		void surf (Input IN, inout SurfaceOutputStandard o) {
			float2 uv = IN.uv_MainTex;
			uv.y -= _Time.y;
			float4 noise = tex2D(_MainTex, uv);
			
			fixed4 c = _Color * noise.r;
			o.Albedo = c.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
```

将噪波纹理指定给河流材质，并确保其颜色为白色。

![inspector](https://catlikecoding.com/unity/tutorials/hex-map/part-6/animating-rivers/noise-inspector.png)
![scene](https://catlikecoding.com/unity/tutorials/hex-map/part-6/animating-rivers/noise-sample.png)

*使用噪波纹理。*

因为V坐标被拉伸了很多，所以噪波纹理也会沿着河流被拉伸。不幸的是，这并不能产生良好的流动。让我们试着以另一种方式拉伸它，通过大幅缩小 U 坐标。十六分之一应该能解决问题。这意味着我们只采样了一小段噪声纹理。

```c#
			float2 uv = IN.uv_MainTex;
			uv.x *= 0.0625;
			uv.y -= _Time.y;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-6/animating-rivers/stretched-u.png)

*拉伸 U 坐标。*

让我们也把流量减慢到每秒四分之一，这样纹理完成一个循环需要四秒钟。

```c#
			uv.y -= _Time.y * 0.25;
```

*流动的噪音。*

### 8.2 混合噪声

这看起来已经好多了，但模式总是保持不变。水不是这样的。

由于我们只使用了一小段噪声，我们可以通过在纹理上滑动该条来改变图案。这是通过向 U 坐标添加时间来实现的。我们必须确保缓慢地改变它，否则河流会出现侧向流动。让我们试试 0.005 的系数。这意味着模式循环需要 200 秒。

```c#
			uv.x = uv.x * 0.0625 + _Time.y * 0.005;
```

*滑动噪音。*

不幸的是，它看起来不太好。即使速度很慢，水仍然保持静止，滑动也很明显。我们可以通过组合两个噪声样本来隐藏滑动，使两个样本朝相反的方向滑动。如果我们使用稍微不同的值来移动第二个样本，它将产生一个微妙的变形动画。

为了确保我们永远不会重叠完全相同的噪声模式，请为第二个样本使用不同的通道。

```c#
			float2 uv = IN.uv_MainTex;
			uv.x = uv.x * 0.0625 + _Time.y * 0.005;
			uv.y -= _Time.y * 0.25;
			float4 noise = tex2D(_MainTex, uv);
			
			float2 uv2 = IN.uv_MainTex;
			uv2.x = uv2.x * 0.0625 - _Time.y * 0.0052;
			uv2.y -= _Time.y * 0.23;
			float4 noise2 = tex2D(_MainTex, uv2);
			
			fixed4 c = _Color * (noise.r * noise2.a);
```

*结合两种滑动噪声模式。*

### 8.3 半透明水

我们的模式看起来足够动态。下一步是使其半透明。

首先，确保水不会投下阴影。您可以通过预制件中 `Rivers` 对象的渲染器组件关闭它们。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-6/animating-rivers/no-shadow-casting.png)

*没有阴影投射。*

接下来，将着色器切换到透明模式。我们必须使用着色器标签来指示这一点。然后将 `alpha` 关键字添加到 `#pragma surface` 线。当我们这样做的时候，我们可以删除 `fullforwardshadows` 关键字，因为我们无论如何都不会投射阴影。

```glsl
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Standard alpha // fullforwardshadows
		#pragma target 3.0
```

现在我们要改变河流的颜色。不要将噪波与颜色相乘，而是将噪波添加到其中。然后使用 `saturate` 函数对结果进行箝位（clamp），这样我们就不会超过 1。

```glsl
			fixed4 c = saturate(_Color + noise.r * noise2.a);
```

这允许我们使用材质的颜色作为基础颜色。噪声将增加其亮度和不透明度。尝试使用不透明度相当低的蓝色。结果将是蓝色半透明的水，带有白色亮点。

![inspector](https://catlikecoding.com/unity/tutorials/hex-map/part-6/animating-rivers/colored.png)

*无色半透明的水。*

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-6/animating-rivers/animating-rivers.unitypackage)

## 9 微调（Tweaking）

现在一切似乎都在正常工作，是时候再次扰动顶点了。除了使单元格边缘变形外，这还会使我们的河流变得不规则。

```c#
	public const float cellPerturbStrength = 4f;
```

![unperturbed](https://catlikecoding.com/unity/tutorials/hex-map/part-6/tweaking/unperturbed.png) ![perturbed](https://catlikecoding.com/unity/tutorials/hex-map/part-6/tweaking/perturbed.png)

*不受干扰 vs. 受干扰。*

检查地形，看看扰动是否会导致任何问题。事实证明，他们确实如此！看看一些高高的瀑布。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-6/tweaking/waterfall-problem.png)

*水被悬崖夹着。*

从高瀑布落下的水可以消失在悬崖后面。当这种情况发生时，很明显，所以我们必须采取行动。

不太明显的是，瀑布可以是倾斜的，而不是直直的。虽然水不是这样工作的，但并不那么明显。你的大脑会想出一个解释，让它看起来很好。所以，让我们忽略这一点。

防止水消失的最简单方法是加深河道。这在水面和河床之间创造了更多的空间。这也使通道壁更加垂直，所以我们不想走得太远。让我们将 `HexMetrics.streamBedElevationOffset` 设置为 -1.75。这解决了大部分问题，而不会切割得太深。一些水仍然会被截断，但不是整个瀑布。

```c#
	public const float streamBedElevationOffset = -1.75f;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-6/tweaking/deeper-channels.png)

*更深的渠道。*

下一个教程是[道路](https://catlikecoding.com/unity/tutorials/hex-map/part-7/)。

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-6/tweaking/tweaking.unitypackage)

[PDF](https://catlikecoding.com/unity/tutorials/hex-map/part-6/Hex-Map-6.pdf)

# Hex Map 7：道路

发布于 2016-07-25

https://catlikecoding.com/unity/tutorials/hex-map/part-7/

*增加对道路的支持。*
*三角形道路。*
*将道路和河流结合起来。*
*让道路看起来崎岖不平。*

本教程是关于[六边形地图](https://catlikecoding.com/unity/tutorials/hex-map/)系列的第七部分。在第六部分中，我们在地形中添加了河流。这一次，我们将增加道路。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-7/tutorial-image.jpg)

*文明的最初迹象。*

## 1 带道路的单元格

就像河流一样，道路从一个单元格到另一个单元格，穿过单元格边缘的中间。最大的区别是道路没有流动的水，所以它们是双向的。此外，一个功能齐全的道路网络需要十字路口，因此我们将为每个单元支持两条以上的道路。

允许道路在所有六个方向上行驶意味着一个单元格可以包含从零到六条道路。这导致了 14 种可能的道路配置。这比河流的五种可能配置要多得多。为了使其可行，我们必须使用一种可以处理所有配置的通用方法。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-7/cells-with-roads/road-configurations.png)

*14 种可能的道路配置。*

### 1.1 跟踪道路

跟踪每个单元格的道路最直接的方法是使用布尔值数组。向 `HexCell` 添加一个私有数组字段并使其可序列化，这样我们就可以在检查器中看到它。通过单元预制件设置阵列大小，使其支持六条道路。

```c#
	[SerializeField]
	bool[] roads;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-7/cells-with-roads/prefab-inspector.png)

*单元格预制有六条道路。*

添加一种方法来检查单元格在某个方向上是否有道路。

```c#
	public bool HasRoadThroughEdge (HexDirection direction) {
		return roads[(int)direction];
	}
```

了解一个单元格是否至少有一条道路也很方便，因此为其添加一个属性。只需遍历数组，找到道路后立即返回 `true`。如果没有，返回 `false`。

```c#
	public bool HasRoads {
		get {
			for (int i = 0; i < roads.Length; i++) {
				if (roads[i]) {
					return true;
				}
			}
			return false;
		}
	}
```

### 1.2 拆除道路

就像河流一样，我们将添加一种从单元格中删除所有道路的方法。它是通过一个循环来关闭之前启用的每条道路的。

```c#
	public void RemoveRoads () {
		for (int i = 0; i < neighbors.Length; i++) {
			if (roads[i]) {
				roads[i] = false;
			}
		}
	}
```

当然，我们还必须禁用单元格邻居的相应道路。

```c#
			if (roads[i]) {
				roads[i] = false;
				neighbors[i].roads[(int)((HexDirection)i).Opposite()] = false;
			}
```

完成后，我们必须确保两个单元格都已刷新。由于道路是单元格的本地道路，我们只需刷新单元格本身，而不必刷新它们的邻居。

```c#
			if (roads[i]) {
				roads[i] = false;
				neighbors[i].roads[(int)((HexDirection)i).Opposite()] = false;
				neighbors[i].RefreshSelfOnly();
				RefreshSelfOnly();
			}
```

### 1.3 添加道路

添加道路就像删除道路一样。唯一的区别是我们将布尔值设置为 `true` 而不是 `false`。我们可以创建一个私有方法来实现这两种功能。然后，我们可以在添加和删除道路时使用它。

```c#
	public void AddRoad (HexDirection direction) {
		if (!roads[(int)direction]) {
			SetRoad((int)direction, true);
		}
	}

	public void RemoveRoads () {
		for (int i = 0; i < neighbors.Length; i++) {
			if (roads[i]) {
				SetRoad(i, false);
			}
		}
	}

	void SetRoad (int index, bool state) {
		roads[index] = state;
		neighbors[index].roads[(int)((HexDirection)index).Opposite()] = state;
		neighbors[index].RefreshSelfOnly();
		RefreshSelfOnly();
	}
```

我们不能让一条河和一条路都朝着同一个方向走。因此，在添加新道路之前，请确保有足够的空间。

```c#
	public void AddRoad (HexDirection direction) {
		if (!roads[(int)direction] && !HasRiverThroughEdge(direction)) {
			SetRoad((int)direction, true);
		}
	}
```

道路也不能与悬崖结合，因为它们太陡了。或者，也许你可以穿过低矮的悬崖，但不能穿过高耸的悬崖？为了确定这一点，我们可以创建一种方法，告诉我们某个方向上的高程差。

```c#
	public int GetElevationDifference (HexDirection direction) {
		int difference = elevation - GetNeighbor(direction).elevation;
		return difference >= 0 ? difference : -difference;
	}
```

现在我们可以强制要求，只有当高差足够小时，才会添加道路。我将把它限制在最大坡度，所以最大为1。

```c#
	public void AddRoad (HexDirection direction) {
		if (
			!roads[(int)direction] && !HasRiverThroughEdge(direction) &&
			GetElevationDifference(direction) <= 1
		) {
			SetRoad((int)direction, true);
		}
	}
```

### 1.4 删除无效道路

我们确保只有在允许的情况下才增加道路。现在我们必须确保在它们以后失效时将其删除。例如，在添加河流时。我们可以禁止将河流放置在道路上，但河流不会被道路阻挡。让他们把路冲走。

无论是否真的有一条路，我们只要把这条路设置为假就足够了。这将始终刷新两个单元格，因此我们不必再在 `SetOutgoingRiver` 中显式调用 `RefreshSelfOnly`。

```c#
	public void SetOutgoingRiver (HexDirection direction) {
		if (hasOutgoingRiver && outgoingRiver == direction) {
			return;
		}

		HexCell neighbor = GetNeighbor(direction);
		if (!neighbor || elevation < neighbor.elevation) {
			return;
		}

		RemoveOutgoingRiver();
		if (hasIncomingRiver && incomingRiver == direction) {
			RemoveIncomingRiver();
		}
		hasOutgoingRiver = true;
		outgoingRiver = direction;
//		RefreshSelfOnly();
		
		neighbor.RemoveIncomingRiver();
		neighbor.hasIncomingRiver = true;
		neighbor.incomingRiver = direction.Opposite();
//		neighbor.RefreshSelfOnly();
		
		SetRoad((int)direction, false);
	}
```

另一个可能使道路无效的操作是标高更改。在这种情况下，我们必须检查各个方向的道路。如果高差太大，必须拆除现有道路。

```c#
	public int Elevation {
		get {
			return elevation;
		}
		set {
			…

			for (int i = 0; i < roads.Length; i++) {
				if (roads[i] && GetElevationDifference((HexDirection)i) > 1) {
					SetRoad(i, false);
				}
			}

			Refresh();
		}
	}
```

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-7/cells-with-roads/cells-with-roads.unitypackage)

## 2 编辑道路

编辑道路就像编辑河流一样。因此，`HexMapEditor` 需要另一个可选的切换，以及一个设置其状态的附带方法。

```c#
	OptionalToggle riverMode, roadMode;

	public void SetRiverMode (int mode) {
		riverMode = (OptionalToggle)mode;
	}

	public void SetRoadMode (int mode) {
		roadMode = (OptionalToggle)mode;
	}
```

`EditCell` 方法现在也必须支持删除和添加道路。这意味着当发生拖动时，它有两种可能的操作。稍微重构代码，以便在有有效拖动时检查两个切换状态。

```c#
	void EditCell (HexCell cell) {
		if (cell) {
			if (applyColor) {
				cell.Color = activeColor;
			}
			if (applyElevation) {
				cell.Elevation = activeElevation;
			}
			if (riverMode == OptionalToggle.No) {
				cell.RemoveRiver();
			}
			if (roadMode == OptionalToggle.No) {
				cell.RemoveRoads();
			}
			if (isDrag) {
				HexCell otherCell = cell.GetNeighbor(dragDirection.Opposite());
				if (otherCell) {
					if (riverMode == OptionalToggle.Yes) {
						otherCell.SetOutgoingRiver(dragDirection);
					}
					if (roadMode == OptionalToggle.Yes) {
						otherCell.AddRoad(dragDirection);
					}
				}
			}
		}
	}
```

通过复制河流面板并调整切换调用的方法，可以快速将道路面板添加到UI中。

这将导致一个相当高的 UI。为了解决这个问题，我改变了彩色面板的布局，以匹配更紧凑的道路和河流面板。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-7/editing-roads/ui.png)

*UI 与道路。*

因为我现在使用两行三个选项的颜色，所以还有另一种颜色的空间。所以我为橙色添加了一个条目。

![inspector](https://catlikecoding.com/unity/tutorials/hex-map/part-7/editing-roads/colors-inspector.png) ![scene](https://catlikecoding.com/unity/tutorials/hex-map/part-7/editing-roads/colors-scene.png)

*五种颜色，黄色、绿色、蓝色、橙色和白色。*

现在可以编辑道路，尽管它们还不可见。您可以使用检查器来验证它是否有效。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-7/editing-roads/cell-with-roads.png)

*检查有道路的单元格。*

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-7/editing-roads/editing-roads.unitypackage)

## 3 三角剖分道路

为了可视化道路，我们必须对其进行三角剖分。这就像河流的水网，只是地形没有河道。

首先，创建一个新的标准着色器，该着色器再次使用 UV 坐标为路面着色。

```glsl
Shader "Custom/Road" {
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
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = fixed4(IN.uv_MainTex, 1, 1);
			o.Albedo = c.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
```

创建使用此着色器的道路材质。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-7/triangulating-roads/material.png)

*道路材料。*

然后调整预制块，使其获得另一个用于道路的六边形网格子对象。此网格不应投射阴影，只使用 UV 坐标。最快的方法（通过预制实例）是复制 `Rivers` 对象并更改其材质。

![hierarchy](https://catlikecoding.com/unity/tutorials/hex-map/part-7/triangulating-roads/hierarchy.png) ![inspector](https://catlikecoding.com/unity/tutorials/hex-map/part-7/triangulating-roads/roads.png)
*道路子对象。*

之后，在 `HexGridChunk` 中添加一个公共的 `HexMesh roads` 字段，并将其包含在 `Triangulate` 中。通过检查器将其连接到 `Roads` 对象。

```c#
	public HexMesh terrain, rivers, roads;
	
	public void Triangulate () {
		terrain.Clear();
		rivers.Clear();
		roads.Clear();
		for (int i = 0; i < cells.Length; i++) {
			Triangulate(cells[i]);
		}
		terrain.Apply();
		rivers.Apply();
		roads.Apply();
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-7/triangulating-roads/chunk.png)

*道路对象已连接。*

### 3.1 单元格间的道路

让我们首先考虑单元之间的路段。就像河流一样，道路将覆盖中间的两个四边形。我们将用道路四边形完全覆盖这些连接四边形，这样我们就可以使用相同的六个顶点位置。为此，在 `HexGridChunk` 中添加一个 `TriangulateRoadSegment` 方法。

```c#
	void TriangulateRoadSegment (
		Vector3 v1, Vector3 v2, Vector3 v3,
		Vector3 v4, Vector3 v5, Vector3 v6
	) {
		roads.AddQuad(v1, v2, v4, v5);
		roads.AddQuad(v2, v3, v5, v6);
	}
```

由于我们不必担心水流，我们不需要 V 坐标，所以我们只需在所有地方将其设置为零。我们可以使用 U 坐标来指示我们是在路中间还是在路边。让我们在中间设置为 1，在两侧设置为 0。

```c#
	void TriangulateRoadSegment (
		Vector3 v1, Vector3 v2, Vector3 v3,
		Vector3 v4, Vector3 v5, Vector3 v6
	) {
		roads.AddQuad(v1, v2, v4, v5);
		roads.AddQuad(v2, v3, v5, v6);
		roads.AddQuadUV(0f, 1f, 0f, 0f);
		roads.AddQuadUV(1f, 0f, 0f, 0f);
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-7/triangulating-roads/road-segment.png)

*单元格之间的路段。*

`TriangulateEdgeStrip` 是调用此方法的逻辑位置，但仅当实际存在道路时。在方法中添加一个布尔参数，这样我们就可以传达这些信息。

```c#
	void TriangulateEdgeStrip (
		EdgeVertices e1, Color c1,
		EdgeVertices e2, Color c2,
		bool hasRoad
	) {
		…
	}
```

当然，我们现在会遇到编译器错误，因为我们还没有提供这些信息。解决方案是在我们调用 `TriangulateEdgeStrip` 的所有地方添加 `false` 作为最终参数。但是，我们也可以声明此参数的默认值为 `false`。这将其转换为可选参数并解决编译错误。

```c#
	void TriangulateEdgeStrip (
		EdgeVertices e1, Color c1,
		EdgeVertices e2, Color c2,
		bool hasRoad = false
	) {
		…
	}
```

> **可选参数是如何工作的？**
>
> 将它们视为编写替代方法以填补缺失参数的简写。例如，方法
>
> ```c#
> int MyMethod (int x = 1, int y = 2) { return x + y; }
> ```
>
> 相当于三种方法
>
> ```c#
> int MyMethod (int x, int y) { return x + y; }
> 
> int MyMethod (int x) { return MyMethod(x, 2); }
> 
> int MyMethod () { return MyMethod(1, 2}; }
> ```
>
> 这里的顺序很重要。可选参数可以从右到左省略。最后一个参数首先被删除。它们总是紧随强制性参数之后。

要对道路进行三角剖分，如果需要，只需调用中间六个顶点的 `TriangulateRoadSegment`。

```c#
	void TriangulateEdgeStrip (
		EdgeVertices e1, Color c1,
		EdgeVertices e2, Color c2,
		bool hasRoad = false
	) {
		terrain.AddQuad(e1.v1, e1.v2, e2.v1, e2.v2);
		terrain.AddQuadColor(c1, c2);
		terrain.AddQuad(e1.v2, e1.v3, e2.v2, e2.v3);
		terrain.AddQuadColor(c1, c2);
		terrain.AddQuad(e1.v3, e1.v4, e2.v3, e2.v4);
		terrain.AddQuadColor(c1, c2);
		terrain.AddQuad(e1.v4, e1.v5, e2.v4, e2.v5);
		terrain.AddQuadColor(c1, c2);

		if (hasRoad) {
			TriangulateRoadSegment(e1.v2, e1.v3, e1.v4, e2.v2, e2.v3, e2.v4);
		}
	}
```

这就解决了扁平单元格的连接问题。为了支持梯田上的道路，我们还必须告诉 `TriangulateEdgeTerraces` 是否必须添加道路。它可以简单地将这些知识传递给 `TriangulateEdgeStrip`。

```c#
	void TriangulateEdgeTerraces (
		EdgeVertices begin, HexCell beginCell,
		EdgeVertices end, HexCell endCell,
		bool hasRoad
	) {
		EdgeVertices e2 = EdgeVertices.TerraceLerp(begin, end, 1);
		Color c2 = HexMetrics.TerraceLerp(beginCell.Color, endCell.Color, 1);

		TriangulateEdgeStrip(begin, beginCell.Color, e2, c2, hasRoad);

		for (int i = 2; i < HexMetrics.terraceSteps; i++) {
			EdgeVertices e1 = e2;
			Color c1 = c2;
			e2 = EdgeVertices.TerraceLerp(begin, end, i);
			c2 = HexMetrics.TerraceLerp(beginCell.Color, endCell.Color, i);
			TriangulateEdgeStrip(e1, c1, e2, c2, hasRoad);
		}

		TriangulateEdgeStrip(e2, c2, end, endCell.Color, hasRoad);
	}
```

在 `TriangulateConnection` 内部调用 `TriangulateEdgeTerraces`。在这里，我们可以确定是否真的有一条路穿过当前的方向。无论是对边缘进行三角剖分，还是对阶地进行三角剖切。

```c#
		if (cell.GetEdgeType(direction) == HexEdgeType.Slope) {
			TriangulateEdgeTerraces(
				e1, cell, e2, neighbor, cell.HasRoadThroughEdge(direction)
			);
		}
		else {
			TriangulateEdgeStrip(
				e1, cell.Color, e2, neighbor.Color,
				cell.HasRoadThroughEdge(direction)
			);
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-7/triangulating-roads/roads-between-cells.png)

*单元格之间的路段。*

### 3.2 在顶部渲染

绘制道路时，您将看到道路段在单元格之间弹出。这些部分的中间将是洋红色，在侧面过渡到蓝色。

然而，当你移动相机时，片段可能会闪烁，有时甚至完全消失。这是因为道路三角形与地形三角形完全重叠。最终呈现在顶部是任意的。解决这个问题需要两个步骤。

首先，我们希望始终在绘制地形后绘制道路。这是通过在绘制规则几何体后渲染它们，并将它们放入稍后的渲染队列中来实现的。

```glsl
		Tags {
			"RenderType"="Opaque"
			"Queue" = "Geometry+1"
		}
```

其次，我们希望确保道路绘制在位于相同位置的地形三角形之上。我们通过添加深度测试偏移来实现这一点。这让 GPU 将三角形视为比实际更靠近相机。

```glsl
		Tags {
			"RenderType"="Opaque"
			"Queue" = "Geometry+1"
		}
		LOD 200
		Offset -1, -1
```

### 3.3 单元格间的道路

在对河流进行三角剖分时，我们每个单元最多只需要处理两个河流方向。我们可以确定五种可能的情况，并对其进行不同的三角剖分，以创建行为良好的河流。然而，道路有 14 种可能的情况。我们不会对每种情况都使用不同的方法。相反，我们将以完全相同的方式处理六个单元方向中的每一个，而不管具体的道路配置如何。

当有一条路穿过一个单元部分时，我们会直接把它开到单元中心，而不会超出三角形区域。我们将从边缘向中心画一段路。然后，我们将使用两个三角形来覆盖到中心的其余部分。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-7/triangulating-roads/road-across-cell.png)

*将道路的一部分做成三角形。*

为了进行三角剖分，我们需要知道单元的中心、左右中间顶点和边缘顶点。添加具有相应参数的 `TriangulateRoad` 方法。

```c#
	void TriangulateRoad (
		Vector3 center, Vector3 mL, Vector3 mR, EdgeVertices e
	) {
	}
```

我们需要一个额外的顶点来构建道路段。它位于左右中间顶点之间。

```c#
	void TriangulateRoad (
		Vector3 center, Vector3 mL, Vector3 mR, EdgeVertices e
	) {
		Vector3 mC = Vector3.Lerp(mL, mR, 0.5f);
		TriangulateRoadSegment(mL, mC, mR, e.v2, e.v3, e.v4);
	}
```

现在我们还可以添加剩下的两个三角形。

```c#
		TriangulateRoadSegment(mL, mC, mR, e.v2, e.v3, e.v4);
		roads.AddTriangle(center, mL, mC);
		roads.AddTriangle(center, mC, mR);
```

我们还必须添加三角形的 UV 坐标。它们的两个顶点位于道路的中间，另一个位于道路的边缘。

```c#
		roads.AddTriangle(center, mL, mC);
		roads.AddTriangle(center, mC, mR);
		roads.AddTriangleUV(
			new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(1f, 0f)
		);
		roads.AddTriangleUV(
			new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(0f, 0f)
		);
```

现在，让我们只关注那些没有河流的单元格。在这些情况下，`Triangulate` 只是创建了一个边缘扇形。将此代码移动到它自己的方法中。然后，当实际上有一条路时，添加对 `TriangulateRoad` 的调用。通过在中心和两个角点之间插值，可以找到左右中间顶点。

```c#
	void Triangulate (HexDirection direction, HexCell cell) {
		…

		if (cell.HasRiver) {
			…
		}
		else {
			TriangulateWithoutRiver(direction, cell, center, e);
		}

		…
	}

	void TriangulateWithoutRiver (
		HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e
	) {
		TriangulateEdgeFan(center, e, cell.Color);
		
		if (cell.HasRoadThroughEdge(direction)) {
			TriangulateRoad(
				center,
				Vector3.Lerp(center, e.v1, 0.5f),
				Vector3.Lerp(center, e.v5, 0.5f),
				e
			);
		}
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-7/triangulating-roads/roads-across-cells.png)

*穿过单元格的道路。*

### 3.4 道路边缘

我们现在可以看到道路，但它们向单元格中心逐渐变细。因为我们没有检查我们正在处理的 14 种道路场景中的哪一种，所以我们无法移动道路中心以产生更令人愉悦的形状。我们可以做的是在单元格的其他部分添加额外的道路边缘。

当一个单元格中有道路穿过，但不在当前方向上时，添加一个道路边三角形。此三角形由中心以及左右中间顶点定义。在这种情况下，只有中心顶点位于道路的中间。另外两个顶点位于其边缘。

```c#
	void TriangulateRoadEdge (Vector3 center, Vector3 mL, Vector3 mR) {
		roads.AddTriangle(center, mL, mR);
		roads.AddTriangleUV(
			new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f)
		);
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-7/triangulating-roads/road-edge.png)

*道路边缘的一部分。*

我们应该对整条路还是只对一条边进行三角剖分，这取决于 `TriangulateRoad`。为此，它需要知道道路是否穿过当前单元格边缘的方向。因此，为此添加一个参数。

```c#
	void TriangulateRoad (
		Vector3 center, Vector3 mL, Vector3 mR,
		EdgeVertices e, bool hasRoadThroughCellEdge
	) {
		if (hasRoadThroughCellEdge) {
			Vector3 mC = Vector3.Lerp(mL, mR, 0.5f);
			TriangulateRoadSegment(mL, mC, mR, e.v2, e.v3, e.v4);
			roads.AddTriangle(center, mL, mC);
			roads.AddTriangle(center, mC, mR);
			roads.AddTriangleUV(
				new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(1f, 0f)
			);
			roads.AddTriangleUV(
				new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(0f, 0f)
			);
		}
		else {
			TriangulateRoadEdge(center, mL, mR);
		}
	}
```

现在，只要单元格有任何道路穿过，`TriangulateWithoutRiver` 就必须调用 `TriangulateRoad`。无论道路是否穿过当前边缘，它都必须通过。

```c#
	void TriangulateWithoutRiver (
		HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e
	) {
		TriangulateEdgeFan(center, e, cell.Color);

		if (cell.HasRoads) {
			TriangulateRoad(
				center,
				Vector3.Lerp(center, e.v1, 0.5f),
				Vector3.Lerp(center, e.v5, 0.5f),
				e, cell.HasRoadThroughEdge(direction)
			);
		}
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-7/triangulating-roads/bulging-roads.png)

*边缘完整的道路。*

### 3.5 平滑道路

我们的道路现已完工。不幸的是，这种方法会在单元格中心产生凸起。当左右顶点附近有道路时，将它们放置在中心和拐角之间的一半是可以的。但如果没有，就会导致凸起。为了解决这个问题，在这些情况下，我们可以将顶点放置在更靠近中心的位置。具体来说，通过使用 ¼ 而不是 ½ 进行插值。

让我们创建一个单独的方法来找出我们应该使用哪些插值器。由于有两个，我们可以将结果放在 `Vector2` 中。它的 X 分量是左点的插值器，它的 Y 分量是右点的插值者。

```c#
	Vector2 GetRoadInterpolators (HexDirection direction, HexCell cell) {
		Vector2 interpolators;
		return interpolators;
	}
```

如果有一条路朝当前方向走，我们可以把分数放在中间。

```c#
	Vector2 GetRoadInterpolators (HexDirection direction, HexCell cell) {
		Vector2 interpolators;
		if (cell.HasRoadThroughEdge(direction)) {
			interpolators.x = interpolators.y = 0.5f;
		}
		return interpolators;
	}
```

否则，这取决于。对于左点，当有一条路穿过前一个方向时，我们可以使用 ½。如果没有，我们应该使用 ¼。正确的点也是如此，但下一个方向也是如此。

```c#
	Vector2 GetRoadInterpolators (HexDirection direction, HexCell cell) {
		Vector2 interpolators;
		if (cell.HasRoadThroughEdge(direction)) {
			interpolators.x = interpolators.y = 0.5f;
		}
		else {
			interpolators.x =
				cell.HasRoadThroughEdge(direction.Previous()) ? 0.5f : 0.25f;
			interpolators.y =
				cell.HasRoadThroughEdge(direction.Next()) ? 0.5f : 0.25f;
		}
		return interpolators;
	}
```

现在我们可以使用这种新方法来确定使用哪些插值器。这将使道路变得平坦。

```c#
	void TriangulateWithoutRiver (
		HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e
	) {
		TriangulateEdgeFan(center, e, cell.Color);

		if (cell.HasRoads) {
			Vector2 interpolators = GetRoadInterpolators(direction, cell);
			TriangulateRoad(
				center,
				Vector3.Lerp(center, e.v1, interpolators.x),
				Vector3.Lerp(center, e.v5, interpolators.y),
				e, cell.HasRoadThroughEdge(direction)
			);
		}
	}
```

![detail](https://catlikecoding.com/unity/tutorials/hex-map/part-7/triangulating-roads/smooth-roads.png) ![map](https://catlikecoding.com/unity/tutorials/hex-map/part-7/triangulating-roads/road-network.png)

*道路平坦。*

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-7/triangulating-roads/triangulating-roads.unitypackage)

## 4 河流与道路的结合

在这一点上，我们有功能性的（functional）道路，但前提是没有河流。当一个单元格有一条河时，不会对任何道路进行三角剖分。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-7/combining-rivers-and-roads/no-roads.png)

*河流附近没有道路。*

让我们创建一个新的 `TriangulateLoadAdjacentToRiver` 方法来处理这种情况下的道路。给它通常的参数。在 `TriangulateAdjacentToRiver` 方法的开头调用它。

```c#
	void TriangulateAdjacentToRiver (
		HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e
	) {
		if (cell.HasRoads) {
			TriangulateRoadAdjacentToRiver(direction, cell, center, e);
		}

		…
	}

	void TriangulateRoadAdjacentToRiver (
		HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e
	) {
	}
```

首先，对没有河流的道路做同样的事情。检查道路是否穿过当前边，获取插值器，创建中间顶点，并调用 `TriangulateRoad`。但因为河流会阻碍我们，我们不得不把道路从它们身边推开。因此，道路的中心将位于不同的位置。我们将使用 `roadCenter`变量来保持这个新位置。它开始等于单元格中心。

```c#
void TriangulateRoadAdjacentToRiver (
		HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e
	) {
		bool hasRoadThroughEdge = cell.HasRoadThroughEdge(direction);
		Vector2 interpolators = GetRoadInterpolators(direction, cell);
		Vector3 roadCenter = center;
		Vector3 mL = Vector3.Lerp(roadCenter, e.v1, interpolators.x);
		Vector3 mR = Vector3.Lerp(roadCenter, e.v5, interpolators.y);
		TriangulateRoad(roadCenter, mL, mR, e, hasRoadThroughEdge);
	}
```

这将在有河流的单元格中修建部分道路。河流穿过的方向将在道路上留下缺口。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-7/combining-rivers-and-roads/roads-with-gaps.png)

*有缺口的道路。*

### 4.1 河流开始或结束

让我们首先考虑包含河流起点或终点的单元格。为了确保道路不与水重叠，我们必须将道路中心推离河流。要获取流入或流出河流的方向，请在 `HexCell` 中添加一个方便的属性。

```c#
	public HexDirection RiverBeginOrEndDirection {
		get {
			return hasIncomingRiver ? incomingRiver : outgoingRiver;
		}
	}
```

现在我们可以在 `HexGridChunk.TriangulateRoadAdjacentToRiver` 中使用此属性，将道路中心推向相反的方向。沿该方向向中间边缘移动三分之一的距离即可。

```c#
		bool hasRoadThroughEdge = cell.HasRoadThroughEdge(direction);
		Vector2 interpolators = GetRoadInterpolators(direction, cell);
		Vector3 roadCenter = center;

		if (cell.HasRiverBeginOrEnd) {
			roadCenter += HexMetrics.GetSolidEdgeMiddle(
				cell.RiverBeginOrEndDirection.Opposite()
			) * (1f / 3f);
		}

		Vector3 mL = Vector3.Lerp(roadCenter, e.v1, interpolators.x);
		Vector3 mR = Vector3.Lerp(roadCenter, e.v5, interpolators.y);
		TriangulateRoad(roadCenter, mL, mR, e, hasRoadThroughEdge);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-7/combining-rivers-and-roads/adjusted-roads.png)

*调整道路。*

接下来，我们必须缩小差距。当我们靠近河流时，我们通过添加额外的道路边缘三角形来实现这一点。如果前一个方向有一条河，那么我们在道路中心、单元格中心和左中点之间添加一个三角形。如果下一个方向有一条河，那么我们在道路中心、右中点和单元格中心之间添加一个三角形。

无论我们处理什么样的河流配置，我们都会这样做，所以把这段代码放在方法的末尾。

```c#
		Vector3 mL = Vector3.Lerp(roadCenter, e.v1, interpolators.x);
		Vector3 mR = Vector3.Lerp(roadCenter, e.v5, interpolators.y);
		TriangulateRoad(roadCenter, mL, mR, e, hasRoadThroughEdge);
		if (cell.HasRiverThroughEdge(direction.Previous())) {
			TriangulateRoadEdge(roadCenter, center, mL);
		}
		if (cell.HasRiverThroughEdge(direction.Next())) {
			TriangulateRoadEdge(roadCenter, mR, center);
		}
```

> **我们不能用 `else` 语句吗？**
>
> 这并非在所有情况下都有效。河流有可能同时从两个方向流过。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-7/combining-rivers-and-roads/roads-complete.png)

*完整的道路。*

### 4.2 直河

具有直河的单元格带来了额外的挑战，因为它们有效地将单元格中心一分为二。我们已经在增加额外的三角形来填补河流沿岸的空白，但我们也必须断开河流两侧的道路。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-7/combining-rivers-and-roads/straight-incorrect.png)

*道路与一条笔直的河流相交。*

如果单元格没有河流的起点或终点，我们可以检查流入和流出的河流是否方向相反。如果是这样，我们有一条笔直的河。

```c#
		if (cell.HasRiverBeginOrEnd) {
			roadCenter += HexMetrics.GetSolidEdgeMiddle(
				cell.RiverBeginOrEndDirection.Opposite()
			) * (1f / 3f);
		}
		else if (cell.IncomingRiver == cell.OutgoingRiver.Opposite()) {
		}
```

为了确定河流相对于当前方向的位置，我们必须检查相邻的方向。这条河要么在左边，要么在右边。正如我们在方法末尾所做的那样，将这些查询缓存在布尔变量中。这也使我们的代码更容易阅读。

```c#
		bool hasRoadThroughEdge = cell.HasRoadThroughEdge(direction);
		bool previousHasRiver = cell.HasRiverThroughEdge(direction.Previous());
		bool nextHasRiver = cell.HasRiverThroughEdge(direction.Next());
		Vector2 interpolators = GetRoadInterpolators(direction, cell);
		Vector3 roadCenter = center;

		if (cell.HasRiverBeginOrEnd) {
			roadCenter += HexMetrics.GetSolidEdgeMiddle(
				cell.RiverBeginOrEndDirection.Opposite()
			) * (1f / 3f);
		}
		else if (cell.IncomingRiver == cell.OutgoingRiver.Opposite()) {
			if (previousHasRiver) {
			}
			else {
			}
		}

		Vector3 mL = Vector3.Lerp(roadCenter, e.v1, interpolators.x);
		Vector3 mR = Vector3.Lerp(roadCenter, e.v5, interpolators.y);
		TriangulateRoad(roadCenter, mL, mR, e, hasRoadThroughEdge);
		if (previousHasRiver) {
			TriangulateRoadEdge(roadCenter, center, mL);
		}
		if (nextHasRiver) {
			TriangulateRoadEdge(roadCenter, mR, center);
```

我们必须将道路中心推向直接指向河流的拐角矢量。如果河流穿过前一个方向，那么这就是第二个坚实的拐角。否则，这将是第一个坚实的角落。

```c#
		else if (cell.IncomingRiver == cell.OutgoingRiver.Opposite()) {
			Vector3 corner;
			if (previousHasRiver) {
				corner = HexMetrics.GetSecondSolidCorner(direction);
			}
			else {
				corner = HexMetrics.GetFirstSolidCorner(direction);
			}
		}
```

为了改变道路，使其最终靠近河流，我们必须将道路中心向那个拐角移动一半。然后，我们还必须将单元格中心向该方向移动四分之一。

```c#
		else if (cell.IncomingRiver == cell.OutgoingRiver.Opposite()) {
			Vector3 corner;
			if (previousHasRiver) {
				corner = HexMetrics.GetSecondSolidCorner(direction);
			}
			else {
				corner = HexMetrics.GetFirstSolidCorner(direction);
			}
			roadCenter += corner * 0.5f;
			center += corner * 0.25f;
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-7/combining-rivers-and-roads/straight-adjusted.png)

*分开的道路。*

我们已经将这个单元格内的道路网络分开了。当河两岸都有道路时，这很好。但当一方没有道路时，我们最终会得到一条孤立的小路。这没有多大意义，所以让我们去掉这些部分。

确认有一条道路穿过当前方向。如果没有，请检查河流同一侧的另一个方向是否有道路。如果两者都没有道路穿过，在三角剖分之前跳出该方法。

```c#
			if (previousHasRiver) {
				if (
					!hasRoadThroughEdge &&
					!cell.HasRoadThroughEdge(direction.Next())
				) {
					return;
				}
				corner = HexMetrics.GetSecondSolidCorner(direction);
			}
			else {
				if (
					!hasRoadThroughEdge &&
					!cell.HasRoadThroughEdge(direction.Previous())
				) {
					return;
				}
				corner = HexMetrics.GetFirstSolidCorner(direction);
			}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-7/combining-rivers-and-roads/straight-pruned.png)

*修剪过的道路。*

> **桥梁呢？**
>
> 我们现在只局限于道路。桥梁和其他结构将在未来的教程中介绍。

### 4.3 曲折的河流

我们要处理的下一种河流是曲折的。这些河流不会分割道路网络，所以我们只需要移动道路中心。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-7/combining-rivers-and-roads/zigzag-incorrect.png)

*曲折的道路穿过。*

检查之字形的最简单方法是比较流入和流出河流的方向。如果它们相邻，那么我们就有一个锯齿形。这会导致两种可能的情况，具体取决于流向。

```c#
		if (cell.HasRiverBeginOrEnd) {
			…
		}
		else if (cell.IncomingRiver == cell.OutgoingRiver.Opposite()) {
			…
		}
		else if (cell.IncomingRiver == cell.OutgoingRiver.Previous()) {
		}
		else if (cell.IncomingRiver == cell.OutgoingRiver.Next()) {
		}
```

我们可以通过使用汇入河流方向的一个角来移动道路中心。它位于哪个角落取决于流向。以 0.2 的系数将道路中心推离该拐角。

```c#
		else if (cell.IncomingRiver == cell.OutgoingRiver.Previous()) {
			roadCenter -= HexMetrics.GetSecondCorner(cell.IncomingRiver) * 0.2f;
		}
		else if (cell.IncomingRiver == cell.OutgoingRiver.Next()) {
			roadCenter -= HexMetrics.GetFirstCorner(cell.IncomingRiver) * 0.2f;
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-7/combining-rivers-and-roads/zigzag-adjusted.png)

*这条路避开了曲折的道路。*

### 4.4 弯曲河流内部

最终的河流形态是一条平滑的曲线。就像一条笔直的河流，这条河也会切断道路。但在这种情况下，双方是不同的。我们将首先处理曲线的内部。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-7/combining-rivers-and-roads/curved-incorrect.png)

*弯曲的河流与重叠的道路。*

当水流方向两侧都有一条河时，我们就在曲线内侧。

```c#
		else if (cell.IncomingRiver == cell.OutgoingRiver.Next()) {
			…
		}
		else if (previousHasRiver && nextHasRiver) {
		}
```

我们必须将道路中心拉向当前的单元格边缘，大大缩短道路。0.7 的系数很好。单元格中心也必须移动，移动系数为 0.5。

```c#
		else if (previousHasRiver && nextHasRiver) {
			Vector3 offset = HexMetrics.GetSolidEdgeMiddle(direction) *
				HexMetrics.innerToOuter;
			roadCenter += offset * 0.7f;
			center += offset * 0.5f;
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-7/combining-rivers-and-roads/curved-inside-adjusted.png)

*缩短道路。*

就像直河一样，我们必须修剪孤立的道路部分。在这种情况下，我们只需要检查当前的方向。

```c#
		else if (previousHasRiver && nextHasRiver) {
			if (!hasRoadThroughEdge) {
				return;
			}
			Vector3 offset = HexMetrics.GetSolidEdgeMiddle(direction) *
				HexMetrics.innerToOuter;
			roadCenter += offset * 0.7f;
			center += offset * 0.5f;
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-7/combining-rivers-and-roads/curved-inside-pruned.png)

*修剪过的道路。*

### 4.5 弯曲河流之外

在检查了所有之前的案例后，唯一剩下的可能性是我们在一条弯曲的河流的外面。外面有三个单元格部分。我们必须找到中间的方向。一旦我们得到它，我们可以将道路中心向该边缘移动 0.25 倍。

```c#
		else if (previousHasRiver && nextHasRiver) {
			…
		}
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
			roadCenter += HexMetrics.GetSolidEdgeMiddle(middle) * 0.25f;
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-7/combining-rivers-and-roads/curved-outside-adjusted.png)

*调整了外面的道路。*

作为最后一步，我们还必须修剪河这边的道路。最简单的方法是检查道路相对于中间的所有三个方向。如果没有路，中止。

```c#
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
			roadCenter += HexMetrics.GetSolidEdgeMiddle(middle) * 0.25f;
		}
```

![not pruned](https://catlikecoding.com/unity/tutorials/hex-map/part-7/combining-rivers-and-roads/curved-outside-not-pruned.png) ![pruned](https://catlikecoding.com/unity/tutorials/hex-map/part-7/combining-rivers-and-roads/curved-outside-pruned.png)

*修剪前后的道路。*

在涵盖了所有河流场景后，我们的河流和道路现在可以共存。河流忽略了道路，道路适应了河流。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-7/combining-rivers-and-roads/rivers-and-roads.png)

*结合河流和道路。*

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-7/combining-rivers-and-roads/combining-rivers-and-roads.unitypackage)

## 5 道路外观

到目前为止，我们已经使用道路的 UV 坐标作为其颜色。因为我们只改变了 U 坐标，所以我们真正可视化的是路中间和边缘之间的过渡。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-7/road-appearance/uv.png)

*显示 UV 坐标。*

现在我们确定道路已正确三角化，我们可以更改道路着色器，使其渲染出更像道路的东西。就像河流一样，这将是一个简单的可视化，没有什么花哨的。

我们将从道路使用纯色开始。只需使用材料的颜色。我把它弄红了。

```c#
		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = _Color;
			
			o.Albedo = c.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-7/road-appearance/red.png)

*红色的道路。*

这看起来已经好多了！但是，让我们继续将道路与地形混合，使用 U 坐标作为混合因子。

```c#
		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = _Color;
			float blend = IN.uv_MainTex.x;
			
			o.Albedo = c.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = blend;
		}
```

这似乎没有效果。这是因为我们的着色器是不透明的。现在它需要阿尔法混合（alpha-blended）。具体来说，我们需要一个混合贴花表面着色器。我们可以通过在 `#pragma surface` 指令中添加 `decal:blend` 来获得所需的着色器。

```glsl
		#pragma surface surf Standard fullforwardshadows decal:blend
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-7/road-appearance/decal.png)

*混合道路。*

这会产生从中间到边缘的平滑线性混合，看起来不太好。为了使其看起来像一条路，我们需要一个坚实的区域，然后快速过渡到不透明的区域。我们可以使用 `smoothstep` 函数来实现这一点。它将从 0 到 1 的线性级数转化为 S 曲线。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-7/road-appearance/smoothstep-curve.png)

*线性平滑台阶。*

`smoothstep` 函数有一个最小和最大参数，用于在任意范围内拟合曲线。超出此范围的输入被箝位，因此曲线变得平坦。让我们用 0.4 作为曲线的起点，0.7 作为终点。这意味着从 0 到 0.4 的 U 坐标将是完全透明的。从 0.7 到 1 的 U 坐标将完全不透明。过渡发生在 0.4 和 0.7 之间。

```c#
			float blend = IN.uv_MainTex.x;
			blend = smoothstep(0.4, 0.7, blend);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-7/road-appearance/smoothstep.png)

*在不透明和透明之间快速转换。*

### 5.1 噪声的道路

由于道路网格受到干扰，道路的宽度各不相同。因此，边缘过渡的宽度也会发生变化。有时是模糊的，有时是尖锐的。当我们把道路想象成土路或沙路时，这种变化是很好的。

让我们更进一步，在道路边缘添加一些噪音。这将使它们看起来更坚固，多边形更少。我们可以通过采样噪声纹理来实现这一点。我们可以使用世界 XZ 坐标对其进行采样，就像我们在扰动单元顶点时所做的那样。

要访问曲面着色器（surface shader）中的世界位置，请将 `float3 worldPos` 添加到输入结构中。

```glsl
		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
		};
```

现在，我们可以使用 `surf` 中的该位置对主要纹理进行采样。一定要缩小坐标，否则纹理会平铺得太快。

```glsl
			float4 noise = tex2D(_MainTex, IN.worldPos.xz * 0.025);
			fixed4 c = _Color;
			float blend = IN.uv_MainTex.x;
```

通过将 U 坐标乘以 `noise.x` 来扰动过渡。但由于噪声值平均为 0.5，这将抹去大部分道路。为了防止这种情况，在相乘之前将噪声加 0.5。

```glsl
			float blend = IN.uv_MainTex.x;
			blend *= noise.x + 0.5;
			blend = smoothstep(0.4, 0.7, blend);
```

![inspector](https://catlikecoding.com/unity/tutorials/hex-map/part-7/road-appearance/inspector.png) ![scene](https://catlikecoding.com/unity/tutorials/hex-map/part-7/road-appearance/perturbed.png)

*道路边缘受到干扰。*

总结一下，让我们也调整一下道路的颜色。这给道路带来了一丝污垢，以匹配它们凌乱的边缘。

将颜色乘以不同的噪声通道，比如 `noise.y`。这将是平均颜色的一半。由于这有点多，请稍微缩小噪声并添加一个常数，这样总数仍然可以达到 1。

```glsl
			fixed4 c = _Color * (noise.y * 0.75 + 0.25);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-7/road-appearance/colorized.png)

*凌乱的道路。*

下一个教程是[水](https://catlikecoding.com/unity/tutorials/hex-map/part-8/)。

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-7/road-appearance/road-appearance.unitypackage)

[PDF](https://catlikecoding.com/unity/tutorials/hex-map/part-7/Hex-Map-7.pdf)

# Hex Map 8：水

发布于 2016-08-23

https://catlikecoding.com/unity/tutorials/hex-map/part-8/

*给单元格加水。*
*把水面做成三角形。*
*用泡沫制造滨岸水。*
*让水和河流嬉戏。*

本教程是关于[六边形地图](https://catlikecoding.com/unity/tutorials/hex-map/)系列的第八部分。我们已经增加了对河流的支持。现在我们要完全淹没单元格。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-8/tutorial-image.jpg)

*水位正在上升。*

## 1 水位

支撑水最直接的方法是定义一个均匀的水位。所有高度低于该水平的单元格都会被淹没。但它更灵活地支撑不同高度的水，所以让我们让水位可变。这要求每个 `HexCell` 跟踪其水位。

```c#
	public int WaterLevel {
		get {
			return waterLevel;
		}
		set {
			if (waterLevel == value) {
				return;
			}
			waterLevel = value;
			Refresh();
		}
	}
	
	int waterLevel;
```

如果你愿意，你可以强制要求某些地形特征在水下不能存在。但我现在不会去。水下道路之类的东西很好。它可能代表了一个最近被淹没的地区。

### 1.1 浸没式单元格

现在我们有了水位，最重要的问题是单元格是否在水下。如果单元格的水位高于其高度，则单元格会被淹没。添加属性以检索此信息。

```c#
	public bool IsUnderwater {
		get {
			return waterLevel > elevation;
		}
	}
```

这意味着当水位和海拔相等时，单元格会上升到水面之上。所以实际的水面在这个高度以下的某个地方。就像河流的表面一样，让我们使用相同的偏移量 `HexMetrics.riverSurfaceElevationOffset`。将其名称更改为更通用的名称。

```c#
//	public const float riverSurfaceElevationOffset = -0.5f;
	public const float waterElevationOffset = -0.5f;
```

调整 `HexCell.RiverSurfaceY` 因此使用了新名称。然后为浸没式单元格的水面添加类似的属性。

```c#
	public float RiverSurfaceY {
		get {
			return
				(elevation + HexMetrics.waterElevationOffset) *
				HexMetrics.elevationStep;
		}
	}
	
	public float WaterSurfaceY {
		get {
			return
				(waterLevel + HexMetrics.waterElevationOffset) *
				HexMetrics.elevationStep;
		}
	}
```

### 1.2 编辑水

编辑单元格的水位就像编辑其高程一样。因此，`HexMapEditor` 必须跟踪活动水位，以及是否应该将其应用于单元格。

```c#
	int activeElevation;
	int activeWaterLevel;

	…
	
	bool applyElevation = true;
	bool applyWaterLevel = true;
	
	
```

添加将这些设置与 UI 连接的方法。

```c#
	public void SetApplyWaterLevel (bool toggle) {
		applyWaterLevel = toggle;
	}
	
	public void SetWaterLevel (float level) {
		activeWaterLevel = (int)level;
	}
```

并将水位包含在 `EditCell` 中。

```c#
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
			…
		}
	}
```

要将水位添加到UI中，请复制高程标签和滑块并对其进行调整。别忘了将他们的活动与正确的方法挂钩。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-8/water-level/ui.png)

*水位滑块。*

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-8/water-level/water-level.unitypackage)

## 2 三角剖分水

我们需要一个新的网格，用新的材料对水进行三角剖分。首先，通过复制 *River* 着色器来创建 *Water* 着色器。更改它，使其仅使用颜色属性。

```glsl
Shader "Custom/Water" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Standard alpha
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = _Color;
			o.Albedo = c.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
```

通过复制 *River* 材质并更改其着色器，使用此着色器创建新材质。保留噪波纹理，因为我们稍后会使用它。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-8/triangulating-water/material.png)

*水材质。*

通过复制 `Rivers` 子对象，将新的子对象添加到预制件中。它不需要 UV 坐标，应该使用 `Water` 材质。像往常一样，通过创建一个预制件实例，更改该实例，然后将更改应用于预制件来实现。然后，删除该实例。

![hierarchy](https://catlikecoding.com/unity/tutorials/hex-map/part-8/triangulating-water/hierarchy.png) ![inspector](https://catlikecoding.com/unity/tutorials/hex-map/part-8/triangulating-water/child-object.png)

*Water 子对象。*

接下来，在 `HexGridChunk` 中添加对水网格的支持。

```c#
	public HexMesh terrain, rivers, roads, water;

	public void Triangulate () {
		terrain.Clear();
		rivers.Clear();
		roads.Clear();
		water.Clear();
		for (int i = 0; i < cells.Length; i++) {
			Triangulate(cells[i]);
		}
		terrain.Apply();
		rivers.Apply();
		roads.Apply();
		water.Apply();
	}
```

并确保将其与实际的预制子对象连接。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-8/triangulating-water/component.png)

*水对象已连接。*

### 2.1 水六边形

当水形成第二层时，让我们按方向给它自己的三角剖分方法。我们只需要在单元格被淹没时调用它。

```c#
	void Triangulate (HexDirection direction, HexCell cell) {
		…

		if (cell.IsUnderwater) {
			TriangulateWater(direction, cell, center);
		}
	}

	void TriangulateWater (
		HexDirection direction, HexCell cell, Vector3 center
	) {
	}
```

就像河流一样，水位相同的单元格之间的水面高度没有变化。因此，看起来我们不需要复杂的边。一个简单的三角形就可以了。

```c#
	void TriangulateWater (
		HexDirection direction, HexCell cell, Vector3 center
	) {
		center.y = cell.WaterSurfaceY;
		Vector3 c1 = center + HexMetrics.GetFirstSolidCorner(direction);
		Vector3 c2 = center + HexMetrics.GetSecondSolidCorner(direction);

		water.AddTriangle(center, c1, c2);
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-8/triangulating-water/water-centers.png)

*水的六边形*

### 2.2 水连接

我们可以用一个四边形连接相邻的水单元格。

```c#
		water.AddTriangle(center, c1, c2);

		if (direction <= HexDirection.SE) {
			HexCell neighbor = cell.GetNeighbor(direction);
			if (neighbor == null || !neighbor.IsUnderwater) {
				return;
			}

			Vector3 bridge = HexMetrics.GetBridge(direction);
			Vector3 e1 = c1 + bridge;
			Vector3 e2 = c2 + bridge;

			water.AddQuad(c1, c2, e1, e2);
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-8/triangulating-water/water-edge-connections.png)

*水的边连接*

用一个三角形填满角落。

```c#
		if (direction <= HexDirection.SE) {
			…

			water.AddQuad(c1, c2, e1, e2);

			if (direction <= HexDirection.E) {
				HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
				if (nextNeighbor == null || !nextNeighbor.IsUnderwater) {
					return;
				}
				water.AddTriangle(
					c2, e2, c2 + HexMetrics.GetBridge(direction.Next())
				);
			}
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-8/triangulating-water/water-corner-connections.png)

*水的角连接。*

我们现在有了相邻时连接的水单元格。它们与海拔较高的干单元格之间留有间隙，但我们稍后会处理。

### 2.3 稳定的水位

我们假设相邻的浸没单元具有相同的水位。当情况确实如此时，事情看起来很好，但当我们违反这一假设时，事情就错了。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-8/triangulating-water/inconsistent-water-level.png)

*水位不一致。*

我们可以试着让水位保持在同一水平。例如，在调整浸没单元格的水位时，我们可以将变化传播到相邻单元格，以保持水位同步。然而，这个过程必须继续下去，直到遇到最终没有被淹没的单元格。这些单元格定义了水体的边界。

这种方法的危险在于，它可能会很快失控。不幸的编辑可能会淹没整个地图。然后，所有块都需要同时进行三角剖分，从而导致大的滞后尖峰。

所以，我们现在不要这样做。它可以是更高级编辑器的一个功能。目前，保持一致性取决于用户。

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-8/triangulating-water/triangulating-water.unitypackage)

## 3 水动画

让我们创造一些看起来有点像波浪的东西，而不是使用统一的颜色。与我们的其他着色器一样，我们目前的目标不是获得出色的视觉效果。只是一点波浪的迹象（a suggestion of waves）就足够了。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-8/animating-water/flat-color.png)

*非常平坦的水。*

让我们对河流做同样的事情。使用 world 位置采样一些噪声，并将其添加到均匀颜色中。要设置动画，请将时间添加到 V 坐标。

```glsl
		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
		};

		…

		void surf (Input IN, inout SurfaceOutputStandard o) {
			float2 uv = IN.worldPos.xz;
			uv.y += _Time.y;
			float4 noise = tex2D(_MainTex, uv * 0.025);
			float waves = noise.z;

			fixed4 c = saturate(_Color + waves);
			o.Albedo = c.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
```

*滚动的水，时间 ×10。*

### 3.1 两个方向

这看起来一点也不像波浪。让我们通过添加第二个噪声样本使其更复杂，这次将时间添加到 U 坐标上。使用不同的噪声通道，这样你最终会得到两种不同的模式。最后的波将这两个样本加在一起。

```glsl
			float2 uv1 = IN.worldPos.xz;
			uv1.y += _Time.y;
			float4 noise1 = tex2D(_MainTex, uv1 * 0.025);

			float2 uv2 = IN.worldPos.xz;
			uv2.x += _Time.y;
			float4 noise2 = tex2D(_MainTex, uv2 * 0.025);

			float waves = noise1.z + noise2.x;
```

将两个样本相加可以产生 0-2 范围内的结果，因此我们必须将其缩小到 0-1。我们可以使用 `smoothstep` 函数来创建更有趣的结果，而不是将波减半。我们将绘制 ¾ – 2 到 0 – 1 的地图，因此部分水面最终没有可见的波浪。

```glsl
			float waves = noise1.z + noise2.x;
			waves = smoothstep(0.75, 2, waves);
```

*两个方向，时间 ×10。*

### 3.2 混合波

很明显，我们有两种滚动噪声模式实际上并没有改变。如果模式改变，它将更有说服力。我们可以通过在噪声样本的不同通道之间进行插值来实现这一点。但我们不应该均匀地这样做，因为那样整个水面都会同时发生变化。这将是非常明显的。相反，我们将使用混合波。

我们通过创建沿对角线穿过水面的正弦波来产生混合波。我们通过将 X 和 Z 世界坐标加在一起并将其用作 `sin` 函数的输入来实现这一点。把它们缩小，这样我们就能得到相当大的乐队。还要添加时间，使其具有动画效果。

```glsl
			float blendWave =
				sin((IN.worldPos.x + IN.worldPos.z) * 0.1 + _Time.y);
```

正弦波在 -1 和 1 之间波动，但我们需要一个 0-1 的范围。我们可以通过摆平波浪到达那里。要单独查看结果，请将其用作输出，而不是调整后的颜色。

```glsl
			sin((IN.worldPos.x + IN.worldPos.z) * 0.1 + _Time.y);
			blendWave *= blendWave;

			float waves = noise1.z + noise2.x;
			waves = smoothstep(0.75, 2, waves);

			fixed4 c = blendWave; //saturate(_Color + waves);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-8/animating-water/blend-wave.png)

*混合波浪。*

为了使混合波不那么明显，从两个样本中添加一些噪声。

```glsl
			float blendWave = sin(
				(IN.worldPos.x + IN.worldPos.z) * 0.1 +
				(noise1.y + noise2.z) + _Time.y
			);
			blendWave *= blendWave;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-8/animating-water/blend-wave-perturbed.png)

*扰动的混合波。*

最后，对于我们的两个噪声样本，使用混合波在两个通道之间进行插值。使用四个不同的频道以获得最大的多样性。

```glsl
			float waves =
				lerp(noise1.z, noise1.w, blendWave) +
				lerp(noise2.x, noise2.y, blendWave);
			waves = smoothstep(0.75, 2, waves);

			fixed4 c = saturate(_Color + waves);
```

*波混合，时间 ×2。*

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-8/animating-water/animating-water.unitypackage)

## 4 近岸水域

我们已经完成了开阔水域的工作，但仍需填补沿岸水域的空白。因为我们必须与陆地的轮廓相匹配，所以岸边的水需要单独的方法。让我们将 `TriangulateWater` 分为两种方法，一种用于开阔水域，一种适用于近岸水域。要确定我们是否有海岸，我们必须查看邻居的单元格。因此，检索邻居停留在 `TriangulateWater` 中。如果有一个邻居，而且它不在水下，那么我们就在处理岸边的水。

```c#
	void TriangulateWater (
		HexDirection direction, HexCell cell, Vector3 center
	) {
		center.y = cell.WaterSurfaceY;

		HexCell neighbor = cell.GetNeighbor(direction);
		if (neighbor != null && !neighbor.IsUnderwater) {
			TriangulateWaterShore(direction, cell, neighbor, center);
		}
		else {
			TriangulateOpenWater(direction, cell, neighbor, center);
		}
	}

	void TriangulateOpenWater (
		HexDirection direction, HexCell cell, HexCell neighbor, Vector3 center
	) {
		Vector3 c1 = center + HexMetrics.GetFirstSolidCorner(direction);
		Vector3 c2 = center + HexMetrics.GetSecondSolidCorner(direction);

		water.AddTriangle(center, c1, c2);

		if (direction <= HexDirection.SE && neighbor != null) {
//			HexCell neighbor = cell.GetNeighbor(direction);
//			if (neighbor == null || !neighbor.IsUnderwater) {
//				return;
//			}
			
			Vector3 bridge = HexMetrics.GetBridge(direction);
			…
		}
	}

	void TriangulateWaterShore (
		HexDirection direction, HexCell cell, HexCell neighbor, Vector3 center
	) {
		
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-8/shore-water/shore-gaps.png)

*沿岸没有三角剖分。*

因为岸边受到扰动，我们也应该扰动岸边的水三角形。所以我们需要边顶点和三角形扇形。

```c#
	void TriangulateWaterShore (
		HexDirection direction, HexCell cell, HexCell neighbor, Vector3 center
	) {
		EdgeVertices e1 = new EdgeVertices(
			center + HexMetrics.GetFirstSolidCorner(direction),
			center + HexMetrics.GetSecondSolidCorner(direction)
		);
		water.AddTriangle(center, e1.v1, e1.v2);
		water.AddTriangle(center, e1.v2, e1.v3);
		water.AddTriangle(center, e1.v3, e1.v4);
		water.AddTriangle(center, e1.v4, e1.v5);
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-8/shore-water/shore-fans.png)

*三角扇沿着海岸。*

接下来是边缘条，就像普通地形一样。然而，我们不必只局限于某些方向。这是因为当我们面对海岸时，我们只会调用 `TriangulateWaterShore`，这总是需要一个条带。

```c#
		water.AddTriangle(center, e1.v4, e1.v5);
		
		Vector3 bridge = HexMetrics.GetBridge(direction);
		EdgeVertices e2 = new EdgeVertices(
			e1.v1 + bridge,
			e1.v5 + bridge
		);
		water.AddQuad(e1.v1, e1.v2, e2.v1, e2.v2);
		water.AddQuad(e1.v2, e1.v3, e2.v2, e2.v3);
		water.AddQuad(e1.v3, e1.v4, e2.v3, e2.v4);
		water.AddQuad(e1.v4, e1.v5, e2.v4, e2.v5);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-8/shore-water/shore-edge.png)

*沿着海岸的边缘地带。*

同样，我们每次也必须添加一个角三角形。

```c#
		water.AddQuad(e1.v4, e1.v5, e2.v4, e2.v5);

		HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
		if (nextNeighbor != null) {
			water.AddTriangle(
				e1.v5, e2.v5, e1.v5 + HexMetrics.GetBridge(direction.Next())
			);
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-8/shore-water/shore-corners.png)

*沿海岸边角。*

我们现在有了完整的岸水。其中一部分总是位于地形网格下方，因此没有间隙。

### 4.1 海岸 UV

我们可以这样离开，但如果岸边的水有一些额外的视觉效果，那就更有趣了。泡沫效果，越靠近海岸，效果越强。为了支持这一点，着色器必须知道碎片离海岸有多近。我们可以通过 UV 坐标提供这些信息。

我们的开阔水域没有 UV 坐标，也不需要任何泡沫。这只适用于靠近海岸的水域。因此，这两种水的要求截然不同。为每种类型赋予自己的网格是有意义的。因此，为 `HexGridChunk` 添加对另一个网格对象的支持。

```c#
	public HexMesh terrain, rivers, roads, water, waterShore;
	
	public void Triangulate () {
		terrain.Clear();
		rivers.Clear();
		roads.Clear();
		water.Clear();
		waterShore.Clear();
		for (int i = 0; i < cells.Length; i++) {
			Triangulate(cells[i]);
		}
		terrain.Apply();
		rivers.Apply();
		roads.Apply();
		water.Apply();
		waterShore.Apply();
	}
```

`TriangulateWaterShore` 将使用这种新网格。

```c#
	void TriangulateWaterShore (
		HexDirection direction, HexCell cell, HexCell neighbor, Vector3 center
	) {
		…
		waterShore.AddQuad(e1.v1, e1.v2, e2.v1, e2.v2);
		waterShore.AddQuad(e1.v2, e1.v3, e2.v2, e2.v3);
		waterShore.AddQuad(e1.v3, e1.v4, e2.v3, e2.v4);
		waterShore.AddQuad(e1.v4, e1.v5, e2.v4, e2.v5);

		HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
		if (nextNeighbor != null) {
			waterShore.AddTriangle(
				e1.v5, e2.v5, e1.v5 + HexMetrics.GetBridge(direction.Next())
			);
		}
	}
```

复制水对象，将其与预制件连接，并将其配置为使用 UV 坐标。还可以通过复制现有的水着色器和材质，为近岸水创建着色器和材质。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-8/shore-water/water-shore-object.png)

*水岸对象和材质，带 UV。*

调整“*水岸*”着色器，使其显示 UV 坐标而不是水。

```glsl
			fixed4 c = fixed4(IN.uv_MainTex, 1, 1);
```

由于我们还没有设置坐标，它将产生纯色。这很容易看出，水岸确实使用了单独的网格和材质。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-8/shore-water/shore-mesh.png)

*单独的网格用于水边。*

让我们把关于海岸的信息放在 V 坐标系中。在水面侧设置为 0，在陆地侧设置为 1。由于我们不需要进行任何其他通信，因此所有 U 坐标都可以为 0。

```c#
		waterShore.AddQuad(e1.v1, e1.v2, e2.v1, e2.v2);
		waterShore.AddQuad(e1.v2, e1.v3, e2.v2, e2.v3);
		waterShore.AddQuad(e1.v3, e1.v4, e2.v3, e2.v4);
		waterShore.AddQuad(e1.v4, e1.v5, e2.v4, e2.v5);
		waterShore.AddQuadUV(0f, 0f, 0f, 1f);
		waterShore.AddQuadUV(0f, 0f, 0f, 1f);
		waterShore.AddQuadUV(0f, 0f, 0f, 1f);
		waterShore.AddQuadUV(0f, 0f, 0f, 1f);

		HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
		if (nextNeighbor != null) {
			waterShore.AddTriangle(
				e1.v5, e2.v5, e1.v5 + HexMetrics.GetBridge(direction.Next())
			);
			waterShore.AddTriangleUV(
				new Vector2(0f, 0f),
				new Vector2(0f, 1f),
				new Vector2(0f, 0f)
			);
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-8/shore-water/shore-transition-wrong.png)

*海岸过渡，不正确。*

上面的代码适用于边，但对于某些角会出错。如果下一个邻居在水下，则当前的方法是正确的。但是当下一个邻居不在水下时，三角形的第三个顶点在陆地下。

```c#
			waterShore.AddTriangleUV(
				new Vector2(0f, 0f),
				new Vector2(0f, 1f),
				new Vector2(0f, nextNeighbor.IsUnderwater ? 0f : 1f)
			);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-8/shore-water/shore-transition-correct.png)

*海岸过渡，正确。*

### 4.2 海岸泡沫

既然岸过渡是正确的，我们可以用它们来创造一些泡沫效果。最简单的方法是将岸值添加到均匀颜色（the uniform color）中。

```glsl
		void surf (Input IN, inout SurfaceOutputStandard o) {
			float shore = IN.uv_MainTex.y;
			
			float foam = shore;

			fixed4 c = saturate(_Color + foam);
			o.Albedo = c.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-8/shore-water/foam-linear.png)

*线性泡沫。*

为了使它更有趣，将一个平方正弦波加入其中。

```glsl
			float foam = sin(shore * 10);
			foam *= foam * shore;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-8/shore-water/foam-sine.png)

*褪色的平方正弦泡沫。*

让泡沫前缘在靠近海岸时变大。这可以通过在使用岸值之前取其平方根来实现。

```glsl
			float shore = IN.uv_MainTex.y;
			shore = sqrt(shore);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-8/shore-water/foam-sqrt.png)

*靠近海岸的泡沫会变得更强。*

添加一些失真，使其看起来更自然。越靠近海岸，变形就越弱。这样，它将更好地与海岸线相匹配。

```glsl
			float2 noiseUV = IN.worldPos.xz;
			float4 noise = tex2D(_MainTex, noiseUV * 0.015);

			float distortion = noise.x * (1 - shore);
			float foam = sin((shore + distortion) * 10);
			foam *= foam * shore;
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-8/shore-water/foam-distorted.png)

*被扰动的泡沫。*

当然，还要为这一切设置动画，包括正弦波和失真。

```glsl
			float2 noiseUV = IN.worldPos.xz + _Time.y * 0.25;
			float4 noise = tex2D(_MainTex, noiseUV * 0.015);

			float distortion = noise.x * (1 - shore);
			float foam = sin((shore + distortion) * 10 - _Time.y);
			foam *= foam * shore;
```

*动画泡沫。*

除了前进的泡沫，还有后退的泡沫。让我们添加一个沿相反方向移动的第二个正弦波来模拟这一点。让它稍微弱一点，并给它一个时间偏移。最后的泡沫是这两个正弦波中的最大值。

```glsl
			float distortion1 = noise.x * (1 - shore);
			float foam1 = sin((shore + distortion1) * 10 - _Time.y);
			foam1 *= foam1;

			float distortion2 = noise.y * (1 - shore);
			float foam2 = sin((shore + distortion2) * 10 + _Time.y + 2);
			foam2 *= foam2 * 0.7;

			float foam = max(foam1, foam2) * shore;
```

*前进和后退的泡沫。*

### 4.3 波浪和泡沫

开阔水域和近岸水域之间存在剧烈的过渡，因为开阔水域的波浪不包括在水岸中。为了解决这个问题，我们还必须将这些波浪包含在 *Water Shore* 着色器中。

与其复制 wave 代码，不如把它放在 *Water.cginc* 包含文件中。事实上，把泡沫和波浪的代码都放进去，每个都是一个单独的函数。

> **着色器包含文件是如何工作的？**
>
> 创建自己的着色器包含文件将在[“渲染5，多个灯光”](https://catlikecoding.com/unity/tutorials/rendering/part-5/)中介绍。

```glsl
float Foam (float shore, float2 worldXZ, sampler2D noiseTex) {
//	float shore = IN.uv_MainTex.y;
	shore = sqrt(shore);

	float2 noiseUV = worldXZ + _Time.y * 0.25;
	float4 noise = tex2D(noiseTex, noiseUV * 0.015);

	float distortion1 = noise.x * (1 - shore);
	float foam1 = sin((shore + distortion1) * 10 - _Time.y);
	foam1 *= foam1;

	float distortion2 = noise.y * (1 - shore);
	float foam2 = sin((shore + distortion2) * 10 + _Time.y + 2);
	foam2 *= foam2 * 0.7;

	return max(foam1, foam2) * shore;
}

float Waves (float2 worldXZ, sampler2D noiseTex) {
	float2 uv1 = worldXZ;
	uv1.y += _Time.y;
	float4 noise1 = tex2D(noiseTex, uv1 * 0.025);

	float2 uv2 = worldXZ;
	uv2.x += _Time.y;
	float4 noise2 = tex2D(noiseTex, uv2 * 0.025);

	float blendWave = sin(
		(worldXZ.x + worldXZ.y) * 0.1 +
		(noise1.y + noise2.z) + _Time.y
	);
	blendWave *= blendWave;

	float waves =
		lerp(noise1.z, noise1.w, blendWave) +
		lerp(noise2.x, noise2.y, blendWave);
	return smoothstep(0.75, 2, waves);
}
```

调整 *Water* 着色器，使其使用我们的新包含文件。

```glsl
		#include "Water.cginc"

		sampler2D _MainTex;

		…

		void surf (Input IN, inout SurfaceOutputStandard o) {
			float waves = Waves(IN.worldPos.xz, _MainTex);

			fixed4 c = saturate(_Color + waves);
			o.Albedo = c.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
```

在“*水岸*”着色器中，计算泡沫和波浪值。然后，当海浪接近岸边时，它们会逐渐消失。最终的结果是泡沫和波浪的最大值。

```glsl
		#include "Water.cginc"

		sampler2D _MainTex;

		…

		void surf (Input IN, inout SurfaceOutputStandard o) {
			float shore = IN.uv_MainTex.y;
			float foam = Foam(shore, IN.worldPos.xz, _MainTex);
			float waves = Waves(IN.worldPos.xz, _MainTex);
			waves *= 1 - shore;

			fixed4 c = saturate(_Color + max(foam, waves));
			o.Albedo = c.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
```

*将泡沫和波浪混合在一起。*

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-8/shore-water/shore-water.unitypackage)

## 5 更多滨水

部分水边网格最终隐藏在地形网格之下。当只有一小部分被隐藏时，这很好。不幸的是，陡峭的悬崖最终掩盖了大部分岸边的水，因此也掩盖了泡沫。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-8/more-shore-water/problem.png)

*岸边水大多被隐藏。*

我们可以通过扩大海岸带的面积来解决这个问题。这可以通过减小水六边形的半径来实现。为此，`HexMetrics` 需要一个水因子来配合其固体因子，以及获得水角的方法。

固体因子为 0.8。为了将水管连接的尺寸增加一倍，我们必须将水系数设置为 0.6。

```c#
	public const float waterFactor = 0.6f;
	
	public static Vector3 GetFirstWaterCorner (HexDirection direction) {
		return corners[(int)direction] * waterFactor;
	}

	public static Vector3 GetSecondWaterCorner (HexDirection direction) {
		return corners[(int)direction + 1] * waterFactor;
	}
```

在 `HexGridChunk` 中使用这些新方法来找到水角。

```c#
	void TriangulateOpenWater (
		HexDirection direction, HexCell cell, HexCell neighbor, Vector3 center
	) {
		Vector3 c1 = center + HexMetrics.GetFirstWaterCorner(direction);
		Vector3 c2 = center + HexMetrics.GetSecondWaterCorner(direction);

		…
	}
	
	void TriangulateWaterShore (
		HexDirection direction, HexCell cell, HexCell neighbor, Vector3 center
	) {
		EdgeVertices e1 = new EdgeVertices(
			center + HexMetrics.GetFirstWaterCorner(direction),
			center + HexMetrics.GetSecondWaterCorner(direction)
		);
		…
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-8/more-shore-water/water-corners.png)

*使用水角。*

水六边形之间的距离确实增加了一倍。现在 `HexMetrics` 还必须提供一种水桥方法。

```c#
	public const float waterBlendFactor = 1f - waterFactor;
	
	public static Vector3 GetWaterBridge (HexDirection direction) {
		return (corners[(int)direction] + corners[(int)direction + 1]) *
			waterBlendFactor;
	}
```

调整 `HexGridChunk` 以使用新方法。

```c#
	void TriangulateOpenWater (
		HexDirection direction, HexCell cell, HexCell neighbor, Vector3 center
	) {
		…

		if (direction <= HexDirection.SE && neighbor != null) {
			Vector3 bridge = HexMetrics.GetWaterBridge(direction);
			…

			if (direction <= HexDirection.E) {
				…
				water.AddTriangle(
					c2, e2, c2 + HexMetrics.GetWaterBridge(direction.Next())
				);
			}
		}
	}

	void TriangulateWaterShore (
		HexDirection direction, HexCell cell, HexCell neighbor, Vector3 center
	) {
		…
		
		Vector3 bridge = HexMetrics.GetWaterBridge(direction);
		…

		HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
		if (nextNeighbor != null) {
			waterShore.AddTriangle(
				e1.v5, e2.v5, e1.v5 +
					HexMetrics.GetWaterBridge(direction.Next())
			);
			…
		}
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-8/more-shore-water/water-bridges.png)

*长长的水桥。*

### 5.1 在水和固体边缘之间

虽然这为我们提供了更多的泡沫空间，但现在还有更大一部分泡沫隐藏在地形之下。理想情况下，我们可以在水侧使用水边，在陆地侧使用实心边。

当从水角开始时，我们不能用一座简单的桥来找到相反的实心边缘。相反，我们可以从邻居的中心向后工作。调整 `TriangulateWaterShore` ，使其使用这种新方法。

```c#
//		Vector3 bridge = HexMetrics.GetWaterBridge(direction);
		Vector3 center2 = neighbor.Position;
		center2.y = center.y;
		EdgeVertices e2 = new EdgeVertices(
			center2 + HexMetrics.GetSecondSolidCorner(direction.Opposite()),
			center2 + HexMetrics.GetFirstSolidCorner(direction.Opposite())
		);
		…

		HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
		if (nextNeighbor != null) {
			Vector3 center3 = nextNeighbor.Position;
			center3.y = center.y;
			waterShore.AddTriangle(
				e1.v5, e2.v5, center3 +
					HexMetrics.GetFirstSolidCorner(direction.Previous())
			);
			…
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-8/more-shore-water/corners-wrong.png)

*边缘拐角不正确。*

这是可行的，除了我们再次需要考虑角三角形的两种情况。

```c#
		HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
		if (nextNeighbor != null) {
//			Vector3 center3 = nextNeighbor.Position;
//			center3.y = center.y;
			Vector3 v3 = nextNeighbor.Position + (nextNeighbor.IsUnderwater ?
				HexMetrics.GetFirstWaterCorner(direction.Previous()) :
				HexMetrics.GetFirstSolidCorner(direction.Previous()));
			v3.y = center.y;
			waterShore.AddTriangle(e1.v5, e2.v5, v3);
			waterShore.AddTriangleUV(
				new Vector2(0f, 0f),
				new Vector2(0f, 1f),
				new Vector2(0f, nextNeighbor.IsUnderwater ? 0f : 1f)
			);
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-8/more-shore-water/corners-correct.png)

*纠正边角。*

这工作得很好，除了当大部分泡沫可见时，泡沫会变得非常明显。为了补偿，您可以通过缩小着色器中的岸值来使效果稍微减弱。

```glsl
	shore = sqrt(shore) * 0.9;
```

*最终泡沫。*

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-8/more-shore-water/more-shore-water.unitypackage)

## 6 水下河流

我们的水是完整的，至少在没有河流流入的情况下是这样。由于水和河流目前相互忽视，河流将流过水下。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-8/underwater-rivers/rivers-and-water.png)

*河流流入水中。*

半透明对象的渲染顺序取决于它们与摄影机的距离。最近的对象最后渲染，这确保了它们最终位于顶部。当你移动相机时，这意味着有时河流，有时水会流到另一条河上。让我们从使渲染顺序一致开始。河流应该画在水面上，这样瀑布才能被正确地展示出来。我们可以通过更改 *River* 着色器的队列来强制执行此操作。

```glsl
Tags { "RenderType"="Transparent" "Queue"="Transparent+1" }
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-8/underwater-rivers/rivers-on-top.png)

*最后画河流。*

### 6.1 隐藏水下河流

虽然河床存在于水下是可以的，而且水实际上可能会流过它，但我们不应该看到这些水。尤其是不在实际水面上渲染。我们可以通过确保只在当前单元格不在水下时添加河段来去除水下河流的水。

```c#
	void TriangulateWithRiverBeginOrEnd (
		HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e
	) {
		…

		if (!cell.IsUnderwater) {
			bool reversed = cell.HasIncomingRiver;
			…
		}
	}
	
	void TriangulateWithRiver (
		HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e
	) {
		…

		if (!cell.IsUnderwater) {
			bool reversed = cell.IncomingRiver == direction;
			…
		}
	}
```

对于 `TriangulateConnection`，让我们从仅在当前单元格和相邻单元格都不在水下时添加河段开始。

```c#
		if (cell.HasRiverThroughEdge(direction)) {
			e2.v3.y = neighbor.StreamBedY;

			if (!cell.IsUnderwater && !neighbor.IsUnderwater) {
				TriangulateRiverQuad(
					e1.v2, e1.v4, e2.v2, e2.v4,
					cell.RiverSurfaceY, neighbor.RiverSurfaceY, 0.8f,
					cell.HasIncomingRiver && cell.IncomingRiver == direction
				);
			}
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-8/underwater-rivers/no-underwater-rivers.png)

*不再有水下河流。*

### 6.2 瀑布

水下河流已经消失了，但现在我们在河流与水面相遇的地方看到了缺口。与水处于同一水平的河流会出现小间隙或重叠。但最明显的是，来自更高海拔的河流缺乏瀑布。让我们先处理这些。

瀑布的河段过去穿过水面。它最终部分在水面上，部分在水面下。我们必须保持部分高于水位，丢弃其余部分。这需要一些工作，所以让我们为此创建一个单独的方法。

新方法需要四个顶点、两个河流水位和水位。我们将对齐它，这样我们就可以沿着瀑布的流向往下看。因此，前两个顶点以及顶部的左侧和右侧，然后是底部的顶点。

```c#
	void TriangulateWaterfallInWater (
		Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4,
		float y1, float y2, float waterY
	) {
		v1.y = v2.y = y1;
		v3.y = v4.y = y2;
		rivers.AddQuad(v1, v2, v3, v4);
		rivers.AddQuadUV(0f, 1f, 0.8f, 1f);
	}
```

在 `TriangulateConnection` 中调用此方法，当邻居最终在水下时，我们有一个瀑布。

```c#
			if (!cell.IsUnderwater) {
				if (!neighbor.IsUnderwater) {
					TriangulateRiverQuad(
						e1.v2, e1.v4, e2.v2, e2.v4,
						cell.RiverSurfaceY, neighbor.RiverSurfaceY, 0.8f,
						cell.HasIncomingRiver && cell.IncomingRiver == direction
					);
				}
				else if (cell.Elevation > neighbor.WaterLevel) {
					TriangulateWaterfallInWater(
						e1.v2, e1.v4, e2.v2, e2.v4,
						cell.RiverSurfaceY, neighbor.RiverSurfaceY,
						neighbor.WaterSurfaceY
					);
				}
			}
```

当当前单元格在水下而邻居不在水下时，我们还必须处理相反方向的瀑布。

```c#
			if (!cell.IsUnderwater) {
				…
			}
			else if (
				!neighbor.IsUnderwater &&
				neighbor.Elevation > cell.WaterLevel
			) {
				TriangulateWaterfallInWater(
					e2.v4, e2.v2, e1.v4, e1.v2,
					neighbor.RiverSurfaceY, cell.RiverSurfaceY,
					cell.WaterSurfaceY
				);
			}
```

这再次产生了原始的河流四边形。接下来，我们必须调整 `TriangulateWaterfallInWater`，使其将底部顶点拉到水位。不幸的是，仅仅调整它们的 Y 坐标是不够的。这会把瀑布从悬崖上拉下来，从而形成缺口。相反，我们必须通过插值将底部顶点拉向顶部顶点。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-8/underwater-rivers/interpolation.png)

*插值。*

要向上移动底部顶点，请将它们在水面以下的距离除以瀑布的高度。这给了我们插值值。

```c#
		v1.y = v2.y = y1;
		v3.y = v4.y = y2;
		float t = (waterY - y2) / (y1 - y2);
		v3 = Vector3.Lerp(v3, v1, t);
		v4 = Vector3.Lerp(v4, v2, t);
		rivers.AddQuad(v1, v2, v3, v4);
		rivers.AddQuadUV(0f, 1f, 0.8f, 1f);
```

结果是一个更短的瀑布，但方向仍然相同。但是，由于底部顶点的位置发生了变化，它们将受到与原始顶点不同的扰动。这意味着最终结果仍然与原始瀑布不匹配。为了解决这个问题，我们必须在插值之前手动扰动顶点，然后添加一个未扰动的四边形。

```c#
		v1.y = v2.y = y1;
		v3.y = v4.y = y2;
		v1 = HexMetrics.Perturb(v1);
		v2 = HexMetrics.Perturb(v2);
		v3 = HexMetrics.Perturb(v3);
		v4 = HexMetrics.Perturb(v4);
		float t = (waterY - y2) / (y1 - y2);
		v3 = Vector3.Lerp(v3, v1, t);
		v4 = Vector3.Lerp(v4, v2, t);
		rivers.AddQuadUnperturbed(v1, v2, v3, v4);
		rivers.AddQuadUV(0f, 1f, 0.8f, 1f);
```

虽然我们已经有了一种添加三角形的不受干扰的变体方法，但我们实际上还没有一种用于四边形的方法。因此，添加所需的 `HexMesh.AddQuadUnperturbed` 方法。

```c#
	public void AddQuadUnperturbed (
		Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4
	) {
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
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-8/underwater-rivers/waterfalls.png)

*瀑布止于水面。*

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-8/underwater-rivers/underwater-rivers.unitypackage)

## 7 河口（Estuaries）

当河流在与水面相同的高度流动时，河流网会接触到岸边网。如果这是一条流入大海或海洋的河流，这就是河流与潮汐相遇的地方。因此，我们将这些地区称为河口。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-8/estuaries/river-meets-foam.png)

*河流与海岸相接，没有顶点扰动。*

目前河口有两个问题。首先，河流四边形连接第 2 条和第 4 条边顶点，跳过第 3 条。由于水边确实使用了第三个顶点，因此最终可能会产生间隙或重叠。我们可以通过调整河口的几何形状来解决这个问题。

第二个问题是泡沫和河流材料之间存在剧烈的过渡。为了解决这个问题，我们需要另一种材料，它融合了河流和水岸效应。

这意味着河口需要特殊处理，所以让我们为它们创建一种单独的方法。当有河流沿水流方向流动时，应在 `TriangulateWaterShore` 中调用它。

```c#
	void TriangulateWaterShore (
		HexDirection direction, HexCell cell, HexCell neighbor, Vector3 center
	) {
		…

		if (cell.HasRiverThroughEdge(direction)) {
			TriangulateEstuary(e1, e2);
		}
		else {
			waterShore.AddQuad(e1.v1, e1.v2, e2.v1, e2.v2);
			waterShore.AddQuad(e1.v2, e1.v3, e2.v2, e2.v3);
			waterShore.AddQuad(e1.v3, e1.v4, e2.v3, e2.v4);
			waterShore.AddQuad(e1.v4, e1.v5, e2.v4, e2.v5);
			waterShore.AddQuadUV(0f, 0f, 0f, 1f);
			waterShore.AddQuadUV(0f, 0f, 0f, 1f);
			waterShore.AddQuadUV(0f, 0f, 0f, 1f);
			waterShore.AddQuadUV(0f, 0f, 0f, 1f);
		}

		…
	}

	void TriangulateEstuary (EdgeVertices e1, EdgeVertices e2) {
	}
```

混合这两种效果的区域不需要填充整个边缘条。梯形形状就足够了。所以我们可以在两边使用两个水边三角形。

```c#
	void TriangulateEstuary (EdgeVertices e1, EdgeVertices e2) {
		waterShore.AddTriangle(e2.v1, e1.v2, e1.v1);
		waterShore.AddTriangle(e2.v5, e1.v5, e1.v4);
		waterShore.AddTriangleUV(
			new Vector2(0f, 1f), new Vector2(0f, 0f), new Vector2(0f, 0f)
		);
		waterShore.AddTriangleUV(
			new Vector2(0f, 1f), new Vector2(0f, 0f), new Vector2(0f, 0f)
		);
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-8/estuaries/hole.png)

*混合区域的梯形孔。*

### 7.1 UV2 坐标

要创建河流效果，我们需要 UV 坐标。但是要创建泡沫效果，我们还需要 UV 坐标。因此，当混合两者时，我们最终需要两组 UV 坐标。幸运的是，Unity 的网格最多可以支持四个 UV 集。我们只需在 `HexMesh` 中添加对第二组的支持。

```c#
	public bool useCollider, useColors, useUVCoordinates, useUV2Coordinates;

	[NonSerialized] List<Vector2> uvs, uv2s;
	
	public void Clear () {
		…
		if (useUVCoordinates) {
			uvs = ListPool<Vector2>.Get();
		}
		if (useUV2Coordinates) {
			uv2s = ListPool<Vector2>.Get();
		}
		triangles = ListPool<int>.Get();
	}

	public void Apply () {
		…
		if (useUVCoordinates) {
			hexMesh.SetUVs(0, uvs);
			ListPool<Vector2>.Add(uvs);
		}
		if (useUV2Coordinates) {
			hexMesh.SetUVs(1, uv2s);
			ListPool<Vector2>.Add(uv2s);
		}
		…
	}
```

要添加到第二个 UV 集，请复制 UV 方法并按照预期进行调整。

```c#
	public void AddTriangleUV2 (Vector2 uv1, Vector2 uv2, Vector3 uv3) {
		uv2s.Add(uv1);
		uv2s.Add(uv2);
		uv2s.Add(uv3);
	}
	
	public void AddQuadUV2 (Vector2 uv1, Vector2 uv2, Vector3 uv3, Vector3 uv4) {
		uv2s.Add(uv1);
		uv2s.Add(uv2);
		uv2s.Add(uv3);
		uv2s.Add(uv4);
	}

	public void AddQuadUV2 (float uMin, float uMax, float vMin, float vMax) {
		uv2s.Add(new Vector2(uMin, vMin));
		uv2s.Add(new Vector2(uMax, vMin));
		uv2s.Add(new Vector2(uMin, vMax));
		uv2s.Add(new Vector2(uMax, vMax));
	}
```

### 7.2 河流着色器函数

因为我们将在两个着色器中使用河流效果，所以将代码从*河流*着色器移动到*水*包含文件中的新函数。

```glsl
float River (float2 riverUV, sampler2D noiseTex) {
	float2 uv = riverUV;
	uv.x = uv.x * 0.0625 + _Time.y * 0.005;
	uv.y -= _Time.y * 0.25;
	float4 noise = tex2D(noiseTex, uv);

	float2 uv2 = riverUV;
	uv2.x = uv2.x * 0.0625 - _Time.y * 0.0052;
	uv2.y -= _Time.y * 0.23;
	float4 noise2 = tex2D(noiseTex, uv2);
	
	return noise.x * noise2.w;
}
```

调整*河流*着色器以使用此新函数。

```glsl
		#include "Water.cginc"

		sampler2D _MainTex;

		…

		void surf (Input IN, inout SurfaceOutputStandard o) {
			float river = River(IN.uv_MainTex, _MainTex);
			
			fixed4 c = saturate(_Color + river);
			…
		}
```

### 7.3 河口对象

为 `HexGridChunk` 添加对河口网格对象的支持。

```c#
	public HexMesh terrain, rivers, roads, water, waterShore, estuaries;
	
	public void Triangulate () {
		terrain.Clear();
		rivers.Clear();
		roads.Clear();
		water.Clear();
		waterShore.Clear();
		estuaries.Clear();
		for (int i = 0; i < cells.Length; i++) {
			Triangulate(cells[i]);
		}
		terrain.Apply();
		rivers.Apply();
		roads.Apply();
		water.Apply();
		waterShore.Apply();
		estuaries.Apply();
	}
```

通过复制和调整河岸的材质和对象，创建河口着色器、材质和对象。将其与块连接，并确保它同时使用 UV 和 UV2 坐标。

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-8/estuaries/inspector.png)

*河口对象。*

### 7.4 三角剖分河口

我们可以通过在河流末端和水边中间放置一个三角形来解决间隙或重叠问题。因为我们的河口着色器是水岸着色器的复制品，所以设置 UV 坐标以匹配泡沫效果。

```c#
	void TriangulateEstuary (EdgeVertices e1, EdgeVertices e2) {
		…

		estuaries.AddTriangle(e1.v3, e2.v2, e2.v4);
		estuaries.AddTriangleUV(
			new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(0f, 1f)
		);
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-8/estuaries/middle-triangle.png)

*中间三角形。*

我们可以通过在中间三角形的两侧添加一个四边形来填充整个梯形。

```c#
		estuaries.AddQuad(e1.v2, e1.v3, e2.v1, e2.v2);
		estuaries.AddTriangle(e1.v3, e2.v2, e2.v4);
		estuaries.AddQuad(e1.v3, e1.v4, e2.v4, e2.v5);
		
		estuaries.AddQuadUV(0f, 0f, 0f, 1f);
		estuaries.AddTriangleUV(
			new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(0f, 1f)
		);
		estuaries.AddQuadUV(0f, 0f, 0f, 1f);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-8/estuaries/trapezoid.png)

*完整梯形。*

让我们旋转左侧四边形的方向，使其对角线连接更短，最终得到对称几何体。

```c#
		estuaries.AddQuad(e2.v1, e1.v2, e2.v2, e1.v3);
		estuaries.AddTriangle(e1.v3, e2.v2, e2.v4);
		estuaries.AddQuad(e1.v3, e1.v4, e2.v4, e2.v5);
		
		estuaries.AddQuadUV(
			new Vector2(0f, 1f), new Vector2(0f, 0f),
			new Vector2(0f, 1f), new Vector2(0f, 0f)
		);
//		estuaries.AddQuadUV(0f, 0f, 0f, 1f);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-8/estuaries/rotated-quad.png)

*旋转四边形对称几何体*

### 7.5 河流流量

为了支持河流效果，我们需要添加 UV2 坐标。中间三角形的底部位于河流的中部，因此其 U 坐标应为 0.5。当河流流向水边时，其左点的 U 坐标为 1，右点的 U 座标为 0。将 Y 坐标设置为 0 和 1，以匹配流向。

```c#
		estuaries.AddTriangleUV2(
			new Vector2(0.5f, 1f), new Vector2(1f, 0f), new Vector2(0f, 0f)
		);
```

三角形任一侧的四边形应与此方向匹配。对于超出河流宽度的点，保持相同的 U 坐标。

```c#
		estuaries.AddQuadUV2(
			new Vector2(1f, 0f), new Vector2(1f, 1f),
			new Vector2(1f, 0f), new Vector2(0.5f, 1f)
		);
		estuaries.AddTriangleUV2(
			new Vector2(0.5f, 1f), new Vector2(1f, 0f), new Vector2(0f, 0f)
		);
		estuaries.AddQuadUV2(
			new Vector2(0.5f, 1f), new Vector2(0f, 1f),
			new Vector2(0f, 0f), new Vector2(0f, 0f)
		);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-8/estuaries/trapezoid-uv2.png)

*梯形 UV2。*

要检查我们是否正确设置了 UV2 坐标，请让*河口(Estuary)*着色器将其可视化。我们可以通过将 `float2 uv2_MainTex` 添加到其输入结构中来访问这些坐标。

```glsl
		struct Input {
			float2 uv_MainTex;
			float2 uv2_MainTex;
			float3 worldPos;
		};

		…

		void surf (Input IN, inout SurfaceOutputStandard o) {
			float shore = IN.uv_MainTex.y;
			float foam = Foam(shore, IN.worldPos.xz, _MainTex);
			float waves = Waves(IN.worldPos.xz, _MainTex);
			waves *= 1 - shore;

			fixed4 c = fixed4(IN.uv2_MainTex, 1, 1);
			…
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-8/estuaries/uv2.png)

*UV2 坐标。*

看起来不错。我们可以用它来创建河流效果。

```glsl
		void surf (Input IN, inout SurfaceOutputStandard o) {
			…

			float river = River(IN.uv2_MainTex, _MainTex);

			fixed4 c = saturate(_Color + river);
			…
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-8/estuaries/river.png)

*使用 UV2 创建河流效果。*

我们设计河流，以便在对单元格之间的连接进行三角剖分时，河流的 V 坐标从 0.8 到 1。因此，我们也应该在这里使用这个范围，而不是从 0 到 1。然而，岸连接比常规单元格连接大 50%。因此，为了最好地匹配河流流量，我们必须从 0.8 到 1.1。

```c#
		estuaries.AddQuadUV2(
			new Vector2(1f, 0.8f), new Vector2(1f, 1.1f),
			new Vector2(1f, 0.8f), new Vector2(0.5f, 1.1f)
		);
		estuaries.AddTriangleUV2(
			new Vector2(0.5f, 1.1f),
			new Vector2(1f, 0.8f),
			new Vector2(0f, 0.8f)
		);
		estuaries.AddQuadUV2(
			new Vector2(0.5f, 1.1f), new Vector2(0f, 1.1f),
			new Vector2(0f, 0.8f), new Vector2(0f, 0.8f)
		);
```

![diagram](https://catlikecoding.com/unity/tutorials/hex-map/part-8/estuaries/uv-synchronized.png)
![scene](https://catlikecoding.com/unity/tutorials/hex-map/part-8/estuaries/river-synchronized.png)

*河流和河口同步流动。*

### 7.6 调整流量

现在，河水继续沿直线流动。但当水流入更大的区域时，它会扩散开来。流量将弯曲。我们可以通过扭曲 UV2 坐标来模拟这一点。

不要在河流宽度之外保持顶部 U 坐标不变，而是将其偏移 0.5。最左边的点变成 1.5，最右边的点变成 -0.5。

同时，通过移动底部左右点的 U 坐标来扩大流量。将左侧的从 1 更改为 0.7，将右侧的从 0 更改为 0.3。

```c#
		estuaries.AddQuadUV2(
			new Vector2(1.5f, 0.8f), new Vector2(0.7f, 1.1f),
			new Vector2(1f, 0.8f), new Vector2(0.5f, 1.1f)
		);
		…
		estuaries.AddQuadUV2(
			new Vector2(0.5f, 1.1f), new Vector2(0.3f, 1.1f),
			new Vector2(0f, 0.8f), new Vector2(-0.5f, 0.8f)
		);
```

![diagram](https://catlikecoding.com/unity/tutorials/hex-map/part-8/estuaries/uv-widening.png)
![scene](https://catlikecoding.com/unity/tutorials/hex-map/part-8/estuaries/river-widening.png)

*拓宽河流流量。*

要完成弯曲效果，请调整相同四个点的 V 坐标。随着水从出水口流出，将顶点的 V 坐标增加到 1。为了产生更好的曲线，将底部两点的 V 坐标增加到 1.15。

```c#
		estuaries.AddQuadUV2(
			new Vector2(1.5f, 1f), new Vector2(0.7f, 1.15f),
			new Vector2(1f, 0.8f), new Vector2(0.5f, 1.1f)
		);
		estuaries.AddTriangleUV2(
			new Vector2(0.5f, 1.1f),
			new Vector2(1f, 0.8f),
			new Vector2(0f, 0.8f)
		);
		estuaries.AddQuadUV2(
			new Vector2(0.5f, 1.1f), new Vector2(0.3f, 1.15f),
			new Vector2(0f, 0.8f), new Vector2(-0.5f, 1f)
		);
```

![diagram](https://catlikecoding.com/unity/tutorials/hex-map/part-8/estuaries/uv-curving.png)
![scene](https://catlikecoding.com/unity/tutorials/hex-map/part-8/estuaries/river-curving.png)

*弯曲的河流。*

### 7.7 河流与海岸的交融

剩下的就是将海岸和河流的效果融合在一起。为此，我们将使用线性插值，以 shore 值作为插值器。

```glsl
			float shoreWater = max(foam, waves);

			float river = River(IN.uv2_MainTex, _MainTex);

			float water = lerp(shoreWater, river, IN.uv_MainTex.y);

			fixed4 c = saturate(_Color + water);
```

虽然这应该有效，但您可能会遇到编译错误。编译器抱怨重新定义了 `_MainTex_ST`。这是由于 Unity 的表面着色器编译器中的一个错误，该错误是由同时使用 `uv_MainTex` 和 `uv2_MainTex` 引起的。我们必须找到一个解决办法。

我们必须手动传递次级 UV 坐标，而不是依赖 `uv2_MainTex`。为此，请将 `uv2_MainTex` 重命名为 `riverUV`。然后向着色器添加一个顶点函数，为其分配坐标。

```glsl
		#pragma surface surf Standard alpha vertex:vert
		…
		
		struct Input {
			float2 uv_MainTex;
			float2 riverUV;
			float3 worldPos;
		};

		…

		void vert (inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.riverUV = v.texcoord1.xy;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			…

			float river = River(IN.riverUV, _MainTex);

			…
		}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-8/estuaries/shore-lerp.png)

*基于海岸的插值。*

除了顶部最左侧和最右侧的顶点外，插值都有效。这条河应该在那些地方消失了。因此，我们不能使用岸值。我们必须使用另一个值，在这两个顶点处设置为0。幸运的是，我们仍然有第一个UV集的U坐标可用，因此我们可以将此值存储在那里。

```c#
		estuaries.AddQuadUV(
			new Vector2(0f, 1f), new Vector2(0f, 0f),
			new Vector2(1f, 1f), new Vector2(0f, 0f)
		);
		estuaries.AddTriangleUV(
			new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(1f, 1f)
		);
		estuaries.AddQuadUV(
			new Vector2(0f, 0f), new Vector2(0f, 0f),
			new Vector2(1f, 1f), new Vector2(0f, 1f)
		);
//		estuaries.AddQuadUV(0f, 0f, 0f, 1f);
```

然后切换到 U 通道以在着色器中插值。

```glsl
			float water = lerp(shoreWater, river, IN.uv_MainTex.x);
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-8/estuaries/blended.png)

*正确的混合。*

我们的河口现在融合了不断拓宽的河流、岸边的水和泡沫。虽然它不能与瀑布完全匹配，但这种效果与瀑布结合在一起看起来也很好。

*河口在动。*

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-8/estuaries/estuaries.unitypackage)

## 8 从水体中流出的河流

虽然我们有河流流入水体，但我们并不支持河流向另一个方向流动。由于许多湖泊都有河流流出，这是我们应该补充的。

当一条河流从水体中流出时，它实际上是流向更高的海拔。这目前无效。我们必须破例，当水位与目标单元格的高度相匹配时，我们才允许这样做。让我们向 `HexCell` 添加一个私有方法，使用我们的新标准检查邻居是否是流出河流的有效目的地。

```c#
	bool IsValidRiverDestination (HexCell neighbor) {
		return neighbor && (
			elevation >= neighbor.elevation || waterLevel == neighbor.elevation
		);
	}
```

使用此新方法确定是否允许设置外流河流。

```c#
	public void SetOutgoingRiver (HexDirection direction) {
		if (hasOutgoingRiver && outgoingRiver == direction) {
			return;
		}

		HexCell neighbor = GetNeighbor(direction);
//		if (!neighbor || elevation < neighbor.elevation) {
		if (!IsValidRiverDestination(neighbor)) {
			return;
		}

		RemoveOutgoingRiver();
		…
	}
```

我们还必须在改变海拔或水位时验证河流。创建一个处理此问题的私有方法。

```c#
	void ValidateRivers () {
		if (
			hasOutgoingRiver &&
			!IsValidRiverDestination(GetNeighbor(outgoingRiver))
		) {
			RemoveOutgoingRiver();
		}
		if (
			hasIncomingRiver &&
			!GetNeighbor(incomingRiver).IsValidRiverDestination(this)
		) {
			RemoveIncomingRiver();
		}
	}
```

在 `Elevation` 和 `WaterLevel` 属性中利用此新方法。

```c#
	public int Elevation {
		…
		set {
			…

//			if (
//				hasOutgoingRiver &&
//				elevation < GetNeighbor(outgoingRiver).elevation
//			) {
//				RemoveOutgoingRiver();
//			}
//			if (
//				hasIncomingRiver &&
//				elevation > GetNeighbor(incomingRiver).elevation
//			) {
//				RemoveIncomingRiver();
//			}
			ValidateRivers();

			…
		}
	}

	public int WaterLevel {
		…
		set {
			if (waterLevel == value) {
				return;
			}
			waterLevel = value;
			ValidateRivers();
			Refresh();
		}
	}
```

![img](https://catlikecoding.com/unity/tutorials/hex-map/part-8/rivers-flowing-out-of-water-bodies/both-in-and-out.png)

*流入和流出湖泊。*

### 8.1 逆流

我们创建了 `HexGridChunk.TriangulateEstuary` 在假设河流只流入水体的情况下进行研究。因此，河流的流向总是相同的。当我们处理一条从水中流出的河流时，我们必须逆流而上。这要求 `TriangulateEstuary` 知道流向。因此，给它一个布尔参数，定义我们是否正在处理一条传入的河流。

```c#
	void TriangulateEstuary (
		EdgeVertices e1, EdgeVertices e2, bool incomingRiver
	) {
	…
}
```

当 `TriangulateWaterShore` 调用该方法时，传递此信息。

```c#
		if (cell.HasRiverThroughEdge(direction)) {
			TriangulateEstuary(e1, e2, cell.IncomingRiver == direction);
		}
```

现在，我们必须通过更改 UV2 坐标来逆转河流的流向。对于流出的河流，必须镜像 U 坐标 0.5 变为 1.5，0 变为 1，1 变为 0，1.5 变为 -0.5。

V 坐标不那么直接。回顾我们如何处理反向河流连接，0.8 应变为 0，1 应变为 -0.2。这意味着 1.1 变为 -0.3，1.15 变为 -0.35。

由于每种情况下的 UV2 坐标都非常不同，让我们为它们使用单独的代码。

```c#
	void TriangulateEstuary (
		EdgeVertices e1, EdgeVertices e2, bool incomingRiver
	) {
		…

		if (incomingRiver) {
			estuaries.AddQuadUV2(
				new Vector2(1.5f, 1f), new Vector2(0.7f, 1.15f),
				new Vector2(1f, 0.8f), new Vector2(0.5f, 1.1f)
			);
			estuaries.AddTriangleUV2(
				new Vector2(0.5f, 1.1f),
				new Vector2(1f, 0.8f),
				new Vector2(0f, 0.8f)
			);
			estuaries.AddQuadUV2(
				new Vector2(0.5f, 1.1f), new Vector2(0.3f, 1.15f),
				new Vector2(0f, 0.8f), new Vector2(-0.5f, 1f)
			);
		}
		else {
			estuaries.AddQuadUV2(
				new Vector2(-0.5f, -0.2f), new Vector2(0.3f, -0.35f),
				new Vector2(0f, 0f), new Vector2(0.5f, -0.3f)
			);
			estuaries.AddTriangleUV2(
				new Vector2(0.5f, -0.3f),
				new Vector2(0f, 0f),
				new Vector2(1f, 0f)
			);
			estuaries.AddQuadUV2(
				new Vector2(0.5f, -0.3f), new Vector2(0.7f, -0.35f),
				new Vector2(1f, 0f), new Vector2(1.5f, -0.2f)
			);
		}
	}
```

*正确的河流流量。*

下一个教程是[地形特征](https://catlikecoding.com/unity/tutorials/hex-map/part-9/)。

[unitypackage](https://catlikecoding.com/unity/tutorials/hex-map/part-8/rivers-flowing-out-of-water-bodies/rivers-flowing-out-of-water-bodies.unitypackage)

[PDF](https://catlikecoding.com/unity/tutorials/hex-map/part-8/Hex-Map-8.pdf)

# [跳转系列独立 Markdown 9 ~ 15](./CatlikeCoding网站翻译-六边形地图9~15.md)