# [返回主 Markdown](./CatlikeCoding网站翻译.md)



# [跳转系列独立 Markdown 22 ~ 27](./CatlikeCoding网站翻译-六边形地图22~27.md)



# Hex Map 2.0.0：URP

发布于 2023-04-11

https://catlikecoding.com/unity/hex-map/2-0-0/

*切换到“渲染图 API”。*
*使用通道（passes）对工作进行分区。*
*好好利用分析。*

![img](https://catlikecoding.com/unity/hex-map/2-0-0/tutorial-image.jpg)

*可以生成随机地图，包括生物群落和河流。*

本教程使用 Unity 2021.3.11.f1编写，遵循 Hex Map 27。它总结了向 URP 的转换。

## 1 URP 设置

Hex Map 最初是在 SRP 存在之前制作的，因此它使用了现在所谓的内置 RP。此版本将其转换为使用 URP。

从内置到 URP 的初始转换将曲面着色器逐字转换为具有自定义功能节点的着色器图，将更改保持在最低限度。这是未来可以改进 URP 着色器的起点。

导入通用 RP 包并创建新的 URP 设置资源，这也将创建附带的渲染器资源。然后调整它，使其适合我们的项目。

### 1.1 渲染

有一个渲染器，我将其命名为 ***URP 渲染器***资源。

仅 ***SRP Batcher*** 启用，其他所有功能均禁用。***存储操作（Store Actions）***设置为***自动***。

此时，***动态批处理***可以被视为传统技术。SRP 批处理器在渲染队列的帮助下对渲染器进行分组，做得很好。

### 1.2 质量

***HDR*** 被禁用，因为六边形地图完全是 LDR。

***抗锯齿（MSAA）***设置为 4x，***渲染比例***设置为 1。当然，你可以改变这一点，以质量换取性能。

### 1.3 照明

***主光/影分辨率***设置为 2048，用于两个 1024 级联贴图。可以降低它以提高性能。

由于“六边形地图”仅使用单个平行光，因此“***附加灯光***”保持默认值。

***反射探测器***未被使用，因此其选项被禁用。同样，***混合照明***也被禁用，因为没有烘焙照明。也不使用***灯光层***。

### 1.4 阴影

配置方向阴影，使其在很远的地方可见，特写和附近的高海拔地形分辨率更高。这是通过将***最大距离***设置为 150、***级联计数***设置为 2 以及将***分割*** 1 设置为 35% 来实现的。***最后一个边界***设置为 5%，以提供一个小的淡入淡出区域。请注意，与 URP 文档和检查器所指示的不同，淡入淡出区域是整个阴影距离的百分比。

***深度偏差***为 1 就足够了。“***法线偏移***”必须为零，否则地形阴影中会出现孔洞。

***启用软阴影***是因为它们看起来比硬阴影更好，但可以禁用以提高性能。

“***保守包围球体（Conservative Enclosing Sphere）***”被禁用，因为它会导致在最大距离之外渲染太多的阴影投射者，使最后一个级联几乎毫无用处。

### 1.5 后处理

目前没有使用 post-FX。***卷更新模式***设置为***通过脚本***，从不更新，因此避免了卷系统开销。

### 1.6 URP 渲染器

应配置单个 ***URP 渲染器数据***资源，以便使用没有深度启动的正向渲染路径。这种方法适用于相当简单的 Hex Map 视觉效果。

***本机 RenderPass***被禁用，因为这只会在使用延迟渲染或类似的复杂方法时为 TBDR GPU 提供好处。

启用***透明接收阴影***，以便水和道路接收阴影。

***后处理***已禁用，并且没有其他渲染功能。

### 1.7 主摄像头

单个摄影机位于场景中 ***Hex Map camera*** 游戏对象层次结构的最深层。它设置为使用依赖 URP 设置和渲染器资源，不覆盖任何内容。

请注意，***渲染/抗锯齿***设置为“***无抗锯齿***”，但这是指 FXAA 或 SMAA 等 post-FX AA。***MSAA*** 设置位于“***输出***”下，因为它会影响渲染目标类型。

### 1.8 环境

Hex Map 依赖于统一的纯色背景来隐藏未探索的区域。除此之外，相机总是往下看，这样天空就永远看不见了。因此，***背景类型***设置为***纯色***。***背景***颜色为 RGB 68615B。

## 2 材质转换

从内置 RP 到 URP 的转换意味着“***材质***”文件夹的内容发生了很大变化。唯一的例外是 ***Highlights*** 子文件夹，它保持不变，因为 Unity UI 与 RP 无关。

### 2.1 着色器关键字

我重命名了自定义着色器关键字。`GRID_ON` 变为 `_SHOW_GRID`，`HEX_MAP_EDIT_MODE` 获得了下划线前缀。

### 2.2 HLSL 文件

将 ***HexMetrics*** 和 ***HexCellData*** 的文件扩展名从 `cginc` 更改为 `hlsl`。***HexMetrics*** 的内容保持不变。调整***HexCellData***，使其使用新的宏进行纹理化，其编辑模式通过布尔函数参数控制，该参数通过基于关键字的着色器图提供。还可以使用 Unity 的纹理宏。

```glsl
TEXTURE2D(_HexCellData);
SAMPLER(sampler_HexCellData);
float4 _HexCellData_TexelSize;

float4 FilterCellData (float4 data, bool editMode) {
	if (editMode) {
		data.xy = 1;
	}
	return data;
}

float4 GetCellData (float3 uv2, int index, bool editMode) {
	float2 uv;
	uv.x = (uv2[index] + 0.5) * _HexCellData_TexelSize.x;
	float row = floor(uv.x);
	uv.x -= row;
	uv.y = (row + 0.5) * _HexCellData_TexelSize.y;
	float4 data = SAMPLE_TEXTURE2D_LOD(_HexCellData, sampler_HexCellData, uv, 0);
	data.w *= 255;
	return FilterCellData(data, editMode);
}

float4 GetCellData (float2 cellDataCoordinates, bool editMode) {
	float2 uv = cellDataCoordinates + 0.5;
	uv.x *= _HexCellData_TexelSize.x;
	uv.y *= _HexCellData_TexelSize.y;
	return FilterCellData(
		SAMPLE_TEXTURE2D_LOD(_HexCellData, sampler_HexCellData, uv, 0), editMode
	);
}
```

所有着色器图都将使用两个自定义函数，一个用于顶点阶段，另一个用于片段状态。匹配的 HLSL 函数名为 `GetVertexCellData` 和 `GetFragmentData`，需要时会添加后缀。这些都放在 ***CustomFunctions*** HLSL 文件中，除了地形和特征着色器图，它们都有自己的 HLSL 文件。

提醒一下，每个单元的可见性和探索数据是通过全局纹理提供的，除特征外的所有材质都依赖于自定义网格顶点数据。单元格索引存储在 UV2 UVW 中。单元格权重存储在顶点颜色 RGB 中。`GetVertexCellData` 使用此信息来确定每个顶点的可见性，这是观察和探索状态的组合。它为地形输出四个组件，为所有其他材质输出两个组件。Terrain 还输出地形索引。

`GetFragmentData` 所需的输入因着色器图而异，但它始终需要插值的可见性数据。它输出一个基础颜色，在需要时加上单独的阿尔法，以及一个探索因子。此因子用于隐藏未探索的单元格。如何做到这一点因着色器图而异。

## 3 着色器图

每种材质类型都有自己的着色器图，并且其旧的曲面着色器将被删除。请参阅每个图的项目存储库。

### 3.1 地形

地形最为复杂，因为它需要混合最多三个相邻单元的地形纹理。它是一种不透明的材料，通过关闭所有照明并切换到与背景匹配的发射颜色来隐藏未探索的单元格。

### 3.2 特征

特征材质是唯一不依赖于自定义网格数据的材质。相反，它依赖于世界位置和网格坐标偏移纹理来检索每个顶点的单元格数据。它以与地形相同的方式隐藏未探索的部分。

### 3.3 道路

道路材料是透明的，位于地形之上。为了避免 Z 轴冲突，添加了一个顶点偏移，将顶点稍微拉向相机。阿尔法和探索都被用来淡出道路。UV0 用于控制道路不透明度。使用渲染队列 ***Transparent-10***，在绘制所有其他透明材质之前绘制道路。

### 3.4 河口、河流、水边和水

水材料的褪色方式与道路相同。UV0 和 UV1 用于控制河流流量和岸线。

不同的水材质被放置在从***透明-9*** 到***透明-6*** 的单独渲染队列中。这样做是为了帮助 SRP 批处理器达到最高效率。它确保 SRP 批处理器不会在不同材料之间来回切换。如果所有内容都在同一队列中，则由于深度排序，批处理将被破坏。确切的绘制顺序并不重要，除了河流应该最后绘制，因为瀑布是唯一重叠的水。

下一个教程是 [Hex Map 2.1.0](https://catlikecoding.com/unity/hex-map/2-1-0/)。

[license](https://catlikecoding.com/unity/tutorials/license/)

[repository](https://bitbucket.org/catlikecoding-projects/hex-map-project/src/dd281b44a663c9bc0334e1f53c7107c0893d4d16/?at=release%2F2.0.0)

[PDF](https://catlikecoding.com/unity/hex-map/2-0-0/Hex-Map-2-0-0.pdf)

# Hex Map 2.1.0：代码清理

发布于 2022-12-28

https://catlikecoding.com/unity/hex-map/2-1-0/

*重构代码。*
*修复单位选择。*
*禁用默认地图的包装。*

![img](https://catlikecoding.com/unity/hex-map/2-1-0/tutorial-image.jpg)

*这一切是如何开始的。*

本教程使用 Unity 2021.3.16.f1 编写，跟随着 [Hex Map 2.0.0](https://catlikecoding.com/unity/hex-map/2-0-0/)。

## 1 代码重组

这个版本是关于系统地更改代码以匹配现代 Unity 和 C# 风格。未来将进行更多这样的更改，但如果纯粹是风格上的更改，则不会公开。此外，还添加了顶级公共类型和公共成员的代码文档。教程中不会显示代码文档。

### 1.1 配置字段和属性

所有公共配置字段都变为私有和可序列化的。在其包含类之外引用的非静态公共字段已成为公共属性。

### 1.2 HexFeatureCollection

`HexFeatureCollection` 类型仅由 `HexFeatureManager` 使用，因此我删除了 ***HexFeatureCollection*** C# 资产，并将其设置为管理器的嵌套类型。

```c#
public class HexFeatureManager : MonoBehaviour {

	[System.Serializable]
	public struct HexFeatureCollection { … }
}
```

## 2 修复和更改

### 2.1 Unity 选择

过去，在进入播放模式后，可以直接在编辑模式下选择单位。播放时切换编辑模式摆脱了这种情况。这可以通过禁用场景中游戏 UI 游戏对象的 `HexGameUI` 组件来修复，使其初始状态正确。

### 2.2 默认地图

`cellCountX`、`cellCountZ` 和 `wrapping` 配置字段已从 `HexGrid` 中删除。它们已被属性替换，初始映射已硬编码为 20×15，在 `Awake` 中没有包装。此地图过去启用了包裹功能，但由于其尺寸较小，最好不要包裹。

下一个教程是 [Hex Map 2.2.0](https://catlikecoding.com/unity/hex-map/2-2-0/)。

[license](https://catlikecoding.com/unity/tutorials/license/)

[repository](https://bitbucket.org/catlikecoding-projects/hex-map-project/src/3686557af812dcb6bfe90238f1eacb8e65b00bd3/?at=release%2F2.1.0)

[PDF](https://catlikecoding.com/unity/hex-map/2-1-0/Hex-Map-2-1-0.pdf)

# Hex Map 2.2.0：单元格视觉升级

发布于 2023-03-10

https://catlikecoding.com/unity/hex-map/2-2-0/

*根据浸没情况对单元格进行着色。*
*在着色器中分析性地导出单元格数据。*
*编辑时突出显示单元格。*

![img](https://catlikecoding.com/unity/hex-map/2-2-0/tutorial-image.jpg)

*展示改进的视觉效果。*

本教程使用 Unity 2021.3.20.f1 编写，跟随着 [Hex Map 2.1.0](https://catlikecoding.com/unity/hex-map/2-1-0/)。

## 1 着色器数据

单元格的可见性变化是动画化的，因此战争迷雾效果可以平滑调整。我们存储了单元是否在发送到着色器的顶点数据的 B 通道中转换。着色器中不需要这些数据，但由于它未被使用，我们将其存储在那里。但现在我们要把这个渠道用于其他事情，所以我们需要改变我们的方法。

### 1.1 可见性转换

向 `HexCellShaderData` 添加一个布尔数组以跟踪可见性转换，并在 `Initialize` 方法中对其进行初始化。

```c#
	bool[] visibilityTransitions;

	…
	
	public void Initialize (int x, int z) {
		…

		if (cellTextureData == null || cellTextureData.Length != x * z) {
			cellTextureData = new Color32[x * z];
			visibilityTransitions = new bool[x * z];
		}
		else {
			for (int i = 0; i < cellTextureData.Length; i++) {
				cellTextureData[i] = new Color32(0, 0, 0, 0);
				visibilityTransitions[i] = false;
			}
		}

		transitioningCells.Clear();
		enabled = true;
	}
```

使 `RefreshVisiblity` 使用该数组，而不是检查并将 B 通道设置为 255。让 `UpdateCell` 也使用数组。

```c#
	public void RefreshVisibility (HexCell cell) {
		…
		//else if (cellTextureData[index].b != 255) {
		//	cellTextureData[index].b = 255;
		else if (!visibilityTransitions[index]) {
			visibilityTransitions[index] = true;
			transitioningCells.Add(cell);
		}
		enabled = true;
	}

	…

	bool UpdateCellData (HexCell cell, int delta) {
		…

		if (!stillUpdating) {
			//data.b = 0;
			visibilityTransitions[index] = false;
		}
		cellTextureData[index] = data;
		return stillUpdating;
	}
```

还有一个 `SetMapData` 方法，该方法当前未使用，但过去和现在都可以用于向着色器发送调试数据。由于过渡指示，它被限制为 254，但现在可以使用全范围。

```c#
			data < 0f ? (byte)0 : (data < 1f ? (byte)(data * 255f) : (byte)255);
```

### 1.2 水面

现在我们有了可用的 B 通道，我们将使用它将单元格的水高度发送到着色器。设置 `RefreshTerrain`。如果单元格在水下，则抓住它的 `WaterSurfaceY` 并将其存储在 B 通道中，这样它就可以支持高达 30 个单位的水位。如果单元格不在水下，则将其设置为零。

支持高达 30 个单位的水面。

```c#
	public void RefreshTerrain (HexCell cell) {
		//cellTextureData[cell.Index] = (byte)cell.TerrainTypeIndex;
		Color32 data = cellTextureData[cell.Index];
		data.b = cell.IsUnderwater ? (byte)(cell.WaterSurfaceY * (255f / 30f)) : (byte)0;
		data.a = (byte)cell.TerrainTypeIndex;
		cellTextureData[cell.Index] = data;
		enabled = true;
	}
```

当单元格的视图标高发生变化时，我们还需要更新此数据，因此给 `ViewElevationChanged` 一个单元格参数，并在那里设置水位。

```c#
	public void ViewElevationChanged (HexCell cell) {
		cellTextureData[cell.Index].b = cell.IsUnderwater ?
			(byte)(cell.WaterSurfaceY * (255f / 30f)) : (byte)0;
		needsVisibilityReset = true;
		enabled = true;
	}
```

调整 `HexCell` 的 `Elevation` 和 `WaterLevel` 属性，使其提供新的参数。此外，只需始终调用 `ViewElevationChanged`，而不是检查是否有更改，以确保水位始终设置正确。

```c#
	public int Elevation {
		get => elevation;
		set {
			…
			//if (ViewElevation != originalViewElevation) {
			//	ShaderData.ViewElevationChanged();
			//}
			ShaderData.ViewElevationChanged(this);
			…
		}
	}

	public int WaterLevel {
		get => waterLevel;
		set {
			…
			//if (ViewElevation != originalViewElevation) {
			//	ShaderData.ViewElevationChanged();
			//}
			ShaderData.ViewElevationChanged(this);
			…
		}
	}
```

现在，我们还应该延迟调用 `RefreshTerrain`，直到在 `Load` 中检索到所有必需的单元格数据，所以让我们将其移动到方法的末尾。

```c#
	public void Load (BinaryReader reader, int header) {
		terrainTypeIndex = reader.ReadByte();
		//ShaderData.RefreshTerrain(this);
		…

		IsExplored = header >= 3 ? reader.ReadBoolean() : false;
		ShaderData.RefreshTerrain(this);
		ShaderData.RefreshVisibility(this);
	}
```

### 1.3 潜水可视化

我们现在可以使用单元格的水面高度根据其淹没程度对其进行着色，为水下区域增加更多的视觉深度。为此，我们需要对 ***Terrain*** HLSL 代码进行一些更改。首先，我们必须通过自定义插值器将水数据传递给片段程序。为此，我们使用 `GetVertexCellData_float` 的现有 `Terrain` 输出，将其更改为 4D 矢量，用检索到的单元的最高水位填充其 W 分量，并按比例放大以覆盖 30 个单元的范围。

```glsl
void GetVertexCellData_float (
	…
	out float4 Terrain,
	…
) {
	…
	Terrain.z = cell2.w;
	Terrain.w = max(max(cell0.b, cell1.b), cell2.b) * 30.0;

	…
}
```

创建一个 `ColorizeSubmers` 函数，将蓝色滤镜应用于基础颜色。它还需要知道表面和水的 Y 坐标。将滤镜（0.25、0.25、0.75）应用于颜色，并根据淹没深度在 15 个单位范围内逐渐淡化。

```glsl
float3 ColorizeSubmergence (float3 baseColor, float surfaceY, float waterY) {
	float submergence = waterY - max(surfaceY, 0);
	float3 colorFilter = float3(0.25, 0.25, 0.75);
	float filterRange = 1.0 / 15.0;
	return baseColor * lerp(1.0, colorFilter, saturate(submergence * filterRange));
}
```

在检索单元格颜色后，使用该功能根据新的地形数据调整基础颜色。

```c#
void GetFragmentData_float (
	…
	float4 Terrain,
	…
) {
	float4 c = …;

	BaseColor = ColorizeSubmergence(c.rgb, WorldPosition.y, Terrain.w);

	…
	
	//BaseColor = c.rgb * grid.rgb;
	BaseColor *= grid.rgb;
	Exploration = Visibility.w;
}
```

更改 `GetTerrainColor`，使其可以继续将地形作为 4D 矢量使用。我还将其 `Color` 参数重命名为 `Weights`，以使其使用更清晰。

```glsl
float4 GetTerrainColor (
	…
	float4 Terrain,
	float3 Weights,
	…
) {
	…
	return c * (Weights[index] * Visibility[index]);
}
```

要执行此操作，请调整 ***Terrain*** 着色器图，以使用4D矢量作为自定义 ***Terrain*** 插值器。

![uncolorized](https://catlikecoding.com/unity/hex-map/2-2-0/shader-data/submergence-uncolorized.jpg) ![colorized](https://catlikecoding.com/unity/hex-map/2-2-0/shader-data/submergence-colorized.jpg)

*没有和有基于淹没的着色。*

## 2 分析网格

到目前为止，我们已经使用纹理将六边形网格投影到地图上。这种方法的一个缺点是，当纹理垂直拉伸时，当网格线投影在悬崖或梯田上时，它看起来不太好。因此，我们将用着色器根据片段的世界位置生成的分析网格替换基于纹理的网格。

### 2.1 六边形空间

我们首先介绍六边形空间的概念。这与世界空间 XZ 平面相同，但按比例缩放，使东西相邻单元中心之间的距离为一个单位。我们可以通过除以由内外半径因子缩放的外六边形半径的两倍，将世界转换为六边形空间。为 ***HexMetrics.hlsl*** 添加一个名为 `WorldToHexSpace` 的转换函数。

```glsl
float2 WoldToHexSpace (float2 p) {
	return p * (1.0 / (2.0 * OUTER_RADIUS * OUTER_TO_INNER));
}
```

包括 ***HexCellData.hlsl*** 顶部的指标。

```glsl
#include "HexMetrics.hlsl"
```

为防止重复包含，请从 ***Terrain.hlsl*** 和 ***Water.hlsl*** 中删除显式指标包含。

```glsl
//#include "HexMetrics.hlsl"
```

然后移动包含水的部分，直到 ***CustomFunctions.hlsl*** 中的六边形单元格数据之后。

```glsl
//#include "Water.hlsl"
#include "HexCellData.hlsl"
#include "Water.hlsl"
```

### 2.2 六边形网格数据

为了方便使用六边形网格数据，请在 ***HexCellData.hlsl*** 中添加 `HexGridData` 结构类型。给它一个单元格中心、单元格偏移坐标——这将是近似的，但足以对单元格数据进行采样——单元格 UV 坐标以备将来使用，单元格边缘到中心的六边形距离，以及用于平滑线条的距离平滑值。

```glsl
float4 GetCellData (float2 cellDataCoordinates, bool editMode) { … }

struct HexGridData {
	// Cell center in hex space.
	float2 cellCenter;

	// Approximate cell offset coordinates. Good enough for point-filtered sampling.
	float2 cellOffsetCoordinates;

	// For potential future use. U covers entire cell, V wraps a bit.
	float2 cellUV;

	// Hexagonal distance to cell center, 0 at center, 1 at edges.
	float distanceToCenter;

	// Smoothstep smoothing for cell center distance transitions.
	// Based on screen-space derivatives.
	float distanceSmoothing;
};
```

为了基于这些数据创建抗锯齿线，我们引入了三个 smoothstep 函数，并将其作为成员添加到 `HexGridData` 中。第一个是 `Smoothstep01`。它有一个阈值参数，用于根据到中心的六边形距离从 0 逐步变为 1，并通过两个方向上的距离平滑范围进行平滑。第二个是 `Smoothstep10`，它做同样的事情，但方向相反。第三个是 `SmoothstepRange`，它在给定内外阈值的情况下，从 0 到 1，再回到 0。

```glsl
struct HexGridData {
	…

	// Smoothstep from 0 to 1 at cell center distance threshold.
	float Smoothstep01 (float threshold) {
		return smoothstep(
			threshold - distanceSmoothing,
			threshold + distanceSmoothing,
			distanceToCenter
		);
	}

	// Smoothstep from 1 to 0 at cell center distance threshold.
	float Smoothstep10 (float threshold) {
		return smoothstep(
			threshold + distanceSmoothing,
			threshold - distanceSmoothing,
			distanceToCenter
		);
	}

	// Smoothstep from 0 to 1 inside cell center distance range.
	float SmoothstepRange (float innerThreshold, float outerThreshold) {
		return Smoothstep01(innerThreshold) * Smoothstep10(outerThreshold);
	}
};
```

### 2.3 从位置到单元格

为了找到网格数据，我们需要知道从中心到最近边缘的六边形距离。为此创建一个 `HexagonalCenterToEdgeDistance` 函数，该函数将六边形空间中的位置转换为该距离，假设单元格的中心位于原点。由于六边形在两个维度上都是对称的，我们可以通过取给定位置的绝对值将问题空间缩减到仅正象限。然后，我们通过取位置和归一化的六边形角边向量的点积来找到到其角边的距离。该矢量指向 NE 方向，（1，√3）到达两步外的邻居。

![img](https://catlikecoding.com/unity/hex-map/2-2-0/analytical-grid/hex-angled-edge-vector.png)

*六角边矢量三角形。*

要同时考虑垂直边，请考虑结果和 X 坐标的最大值。由于这是在六边形空间中，从中心到边缘的最大距离为 ½，因此我们将其加倍以获得所需的 0-1 范围。

```glsl
#define HEX_ANGLED_EDGE_VECTOR float2(1, sqrt(3))

// Calculate hexagonal center-edge distance for point relative to center in hex space.
// 0 at cell center and 1 at edges.
float HexagonalCenterToEdgeDistance (float2 p) {
	// Reduce problem to one quadrant.
	p = abs(p);
	// Calculate distance to angled edge.
	float d = dot(p, normalize(HEX_ANGLED_EDGE_VECTOR));
	// Incorporate distance to vertical edge.
	d = max(d, p.x);
	// Double to increase range from center to edge to 0-1.
	return 2 * d;
}
```

为了在六边形空间中找到离点最近的单元格中心，我们添加了一个 `HexModulo` 函数。它通过减去由位置地板除以相同向量缩放的六边形角边向量来实现这一点。

```glsl
// Calculate hex-based modulo to find position vector.
float2 HexModulo (float2 p) {
	return p - HEX_ANGLED_EDGE_VECTOR * floor(p / HEX_ANGLED_EDGE_VECTOR);
}
```

为了获得最终的六边形网格数据，引入一个具有世界 XZ 位置参数的 `GetHexGridData` 函数，该函数将其转换为六边形空间。然后，有两个候选单元位置最接近。第一个是通过取位置的 hex modulo 来找到的。第二种方法是相同的，除了在取模之前，通过减去六边形角边缘向量的一半，对角线偏移一个单元格。然后从两者中减去相同的偏移量，使其与我们的网格对齐。这些是从单元格中心到十六进制位置的向量。最小的那个就是我们需要的那个。

```glsl
// Get hex grid data analytically derived from world-space XZ position.
HexGridData GetHexGridData (float2 worldPositionXZ) {
	float2 p = WoldToHexSpace(worldPositionXZ);
	
	// Vectors from nearest two cell centers to position.
	float2 gridOffset = HEX_ANGLED_EDGE_VECTOR * 0.5;
	float2 a = HexModulo(p) - gridOffset;
	float2 b = HexModulo(p - gridOffset) - gridOffset;
	bool aIsNearest = dot(a, a) < dot(b, b);

	float2 vectorFromCenterToPosition = aIsNearest ? a : b;

	HexGridData d;
	return d;
}
```

现在我们可以填充网格数据。通过从六边形位置减去我们找到的向量来找到单元格中心。

单元格偏移 X 坐标是单元格中心 X，如果第一个单元格候选最终最接近，则偏移 −½，因此它与行的锯齿形偏移相匹配。单元偏移 Y 坐标是单元中心 Y 除以内外半径因子。这些偏移坐标并不精确，但足以满足我们的目的。

单元 UV 坐标与找到的向量相同，加上 ½ 将其与单元中心对齐。我们目前不使用它，但可以用来给单元格纹理。

通过将向量传递给 `HexagonalCenterToEdgeDistance` 来计算到中心的距离。

对于距离平滑，我们使用距离到中心的 `fwidth` 来使过渡大约为两个像素宽。

```glsl
	HexGridData d;
	d.cellCenter = p - vectorFromCenterToPosition;
	d.cellOffsetCoordinates.x = d.cellCenter.x - (aIsNearest ? 0.5 : 0.0);
	d.cellOffsetCoordinates.y = d.cellCenter.y / OUTER_TO_INNER;
	d.cellUV = vectorFromCenterToPosition + 0.5;
	d.distanceToCenter = HexagonalCenterToEdgeDistance(vectorFromCenterToPosition);
	d.distanceSmoothing = fwidth(d.distanceToCenter);
	return d;
```

### 2.4 锐利的网格线

要创建清晰的分析网格线，请在 ***Terrain.hlsl*** 中添加一个 `ApplyGrid` 函数，其中包含基础颜色和六边形网格数据的参数。使用阈值为 0.965 的 `SmoothStep10` 将颜色减少到 20%。

```glsl
// Apply an 80% darkening grid outline at hex center distance 0.965-1.
float3 ApplyGrid (float3 baseColor, HexGridData h) {
	return baseColor * (0.2 + 0.8 * h.Smoothstep10(0.965));
}
```

从 `GetFragmentData_float` 中删除 `GridTexture` 参数，并使其使用新函数显示网格。

```glsl
void GetFragmentData_float (
	…
	//UnityTexture2D GridTexture,
	…
) {
	…

	//float4 grid = 1;
	HexGridData hgd = GetHexGridData(WorldPosition.xz);

	if (ShowGrid) {
		//float2 gridUV = WorldPosition.xz;
		//gridUV.x *= 1 / (4 * 8.66025404);
		//gridUV.y *= 1 / (2 * 15.0);
		//grid = GridTexture.Sample(GridTexture.samplerstate, gridUV);
		BaseColor = ApplyGrid(BaseColor, hgd);
	}

	…
	
	//BaseColor *= grid.rgb;
	Exploration = Visibility.w;
}
```

同时从 ***Terrain*** 着色器图中删除纹理，并删除 ***Grid*** 纹理资源。

![texture](https://catlikecoding.com/unity/hex-map/2-2-0/analytical-grid/grid-texture.jpg) ![analytical](https://catlikecoding.com/unity/hex-map/2-2-0/analytical-grid/grid-analytical.jpg)

*基于纹理和分析的网格线。*

### 2.5 特征可见性

我们还可以在 ***Feature.hlsl*** 的顶点函数中使用新的六边形网格数据来查找单元格偏移坐标，删除其 `GridCoordinatesTexture` 参数。

```glsl
void GetVertexCellData_float (
	//UnityTexture2D GridCoordinatesTexture,
	…
) {
	//float2 gridUV = WorldPosition.xz;
	//gridUV.x *= 1 / (4 * 8.66025404);
	//gridUV.y *= 1 / (2 * 15.0);
	//float2 cellDataCoordinates = floor(gridUV.xy) + GridCoordinatesTexture.SampleLevel(
	//	GridCoordinatesTexture.samplerstate, gridUV, 0
	//).rg;
	//cellDataCoordinates *= 2;
	HexGridData hgd = GetHexGridData(WorldPosition.xz);
	float4 cellData = GetCellData(hgd.cellOffsetCoordinates, EditMode);

	…
}
```

同时从 ***Feature*** 着色器图中删除纹理，并删除 ***Grid Coordinates*** 纹理资源。除了不再需要纹理外，这还修复了一些由非常靠近某些单元格边缘的不正确偏移解释引起的可见性伪影。

![texture](https://catlikecoding.com/unity/hex-map/2-2-0/analytical-grid/features-texture.jpg) ![analytical](https://catlikecoding.com/unity/hex-map/2-2-0/analytical-grid/features-analytical.jpg)

*基于纹理和分析的特征可见性。*

## 3 突出显示单元格

六边形单元格数据也允许我们在着色器中创建其他效果。让我们在编辑地图时使用它来突出显示受影响的单元格。

### 3.1 单元格突出显示数据

为了知道哪些单元格受到影响，我们必须向 GPU 发送一些突出显示数据。至少我们需要六边形空间中的画笔中心 XZ 坐标。添加属性以将这些坐标检索到 `HexCoordinates`。

```c#
	public float HexX => X + Z / 2 + ((Z & 1) == 0 ? 0f : 0.5f);

	public float HexZ => Z * HexMetrics.outerToInner;
```

我们将通过设置名为 ***_CellHighlighting*** 的全局 4D 矢量着色器属性，使 `HexMapEditor` 将高亮数据传递给着色器。如果使用环绕地图，其 XY 分量包含六边形坐标，其 Z 分量包含方形画笔大小加 ½，其 W 分量包含环绕大小。在带有单元格参数的新 `UpdateCellHighlightData` 方法中设置此数据。如果单元格为 `null`，则通过单独的 `ClearCellHighlightData` 方法清除数据，该方法将向量设置为（0，0，-1，0）。

```c#
	static int cellHighlightingId = Shader.PropertyToID("_CellHighlighting");
	
	…
	
	void UpdateCellHighlightData (HexCell cell) {
		if (cell == null) {
			ClearCellHighlightData();
			return;
		}

		// Works up to brush size 6.
		Shader.SetGlobalVector(
			cellHighlightingId,
			new Vector4(
				cell.Coordinates.HexX,
				cell.Coordinates.HexZ,
				brushSize * brushSize + 0.5f,
				HexMetrics.wrapSize
			)
		);
	}

	void ClearCellHighlightData () =>
		Shader.SetGlobalVector(cellHighlightingId, new Vector4(0f, 0f, -1f, 0f));
```

在 `Update` 中，如果 UI 未声明光标并且未按下主鼠标按钮，则使用光标下的单元格调用 `UpdateCellHighlightingData`。如果光标被 UI 占用，则调用 `ClearHighlightData`。同时，使用 `HandleInput` 末尾的当前单元格调用 `UpdateCellHighlightData`。

```c#
	void Update () {
		if (!EventSystem.current.IsPointerOverGameObject()) {
			if (Input.GetMouseButton(0)) {
				HandleInput();
				return;
			}
			else {
				// Potential optimization: only do this if camera or cursor has changed.
				UpdateCellHighlightData(GetCellUnderCursor());
			}
			…
		}
		else {
			ClearCellHighlightData();
		}
		previousCell = null;
	}
	
	…
	
	void HandleInput () {
		…
		UpdateCellHighlightData(currentCell);
	}
```

### 3.2 显示突出显示的单元格

要检查着色器中的单元格是否突出显示，请在 ***HexCellData.hlsl*** 中的 `HexGridData` 中添加 `IsHighlighted` 函数。要突出显示的单元格的相对正象限位置是通过将突出显示的六边形坐标的绝对值减去单元格中心来找到的。

然后，如果启用了世界包裹，请检查 X 位置是否超过包裹大小的一半，如果是，请包裹一次。当包装被禁用时，我们也可以这样做，因为在这种情况下，包装大小为零。

如果相对位置矢量的平方大小小于突出显示的 Z 分量，即编辑半径的平方加 ½，则应突出显示该单元格。这个简单的圆形阈值检查适用于 6 号画笔。这是可以接受的，因为我们最大的刷子尺寸只有 4 个。

```glsl
	// Is highlighed if square distance from cell to highlight center is below threshold.
	// Works up to brush size 6.
	bool IsHighlighted () {
		float2 cellToHighlight = abs(_CellHighlighting.xy - cellCenter);

		// Adjust for world X wrapping if needed.
		if (cellToHighlight.x > _CellHighlighting.w * 0.5) {
			cellToHighlight.x -= _CellHighlighting.w;
		}

		return dot(cellToHighlight, cellToHighlight) < _CellHighlighting.z;
	}
```

我们通过添加一个新的 `ApplyHighlight` 函数来应用 ***Terrain.hlsl*** 中的高亮显示，该函数具有基色和六边形网格数据的参数。通过使用阈值为 0.68 和 0.8 的 `SmoothstepRange` 绘制白色六边形轮廓来突出显示。

```glsl
// Apply a white outline at hex center distance 0.68-0.8.
float3 ApplyHighlight (float3 baseColor, HexGridData h) {
	return saturate(h.SmoothstepRange(0.68, 0.8) + baseColor.rgb);
}
```

在 `GetFragmentData_float` 中显示网格后，如果需要，应用高亮显示。

```c#
	if (ShowGrid) {
		BaseColor = ApplyGrid(BaseColor, hgd);
	}

	if (hgd.IsHighlighted()) {
		BaseColor = ApplyHighlight(BaseColor, hgd);
	}
```

![img](https://catlikecoding.com/unity/hex-map/2-2-0/highlighting-cells/cell-highlighting.jpg)

*突出显示受地形编辑影响的单元格。*

下一个教程是 [Hex Map 2.3.0](https://catlikecoding.com/unity/hex-map/2-3-0/)。

[license](https://catlikecoding.com/unity/tutorials/license/)

[repository](https://bitbucket.org/catlikecoding-projects/hex-map-project/src/665c9a8069420d3646369a9854177392f676a0f0/?at=release%2F2.2.0)

[PDF](https://catlikecoding.com/unity/hex-map/2-2-0/Hex-Map-2-2-0.pdf)

# Hex Map 2.3.0：精简单元格

发布于 2023-10-04

https://catlikecoding.com/unity/hex-map/2-3-0/

*将单元格数据存储在位标志中。*
*消除每个单元格的数组。*
*通过网格检索邻居。*

![img](https://catlikecoding.com/unity/hex-map/2-3-0/tutorial-image.jpg)

*当地图覆盖多个大陆时，你开始关心内存使用情况。*

本教程使用 Unity 2021.3.24f1 编写，跟随 [Hex Map 2.2.0](https://catlikecoding.com/unity/hex-map/2-2-0/)。

## 1 道路

六边形地图的每个单元格都有自己的游戏对象，该对象又将其数据存储在字段和数组中。这对于小地图很好，但对于较大的地图，单元格的内存大小会出现问题。在本教程中，我们将大幅减少单元格的内存占用，最终目标是在未来的版本中完全消除单元格游戏对象。

我们将压缩的第一个数据是道路，它们目前存储在每个单元格的数组中。我们将用单个位标志字段替换该数组，类似于[迷宫项目](https://catlikecoding.com/unity/maze/)中用于单元格标志的字段。

### 1.1 六边形标志

创建一个 HexFlags 标志枚举类型，其中六位表示道路可能存在的方向，以及所有道路的掩码和空状态。为它提供扩展方法，以检查给定掩码的任何、所有或没有位是否已设置，以及获取设置或不设置掩码的标志的方法。

```c#
[System.Flags]
public enum HexFlags
{
	Empty = 0,

	RoadNE = 0b000001,
	RoadE  = 0b000010,
	RoadSE = 0b000100,
	RoadSW = 0b001000,
	RoadW  = 0b010000,
	RoadNW = 0b100000,
	
	Roads = 0b111111
}

public static class HexFlagsExtensions
{
	public static bool HasAny (this HexFlags flags, HexFlags mask) => (flags & mask) != 0;

	public static bool HasAll (this HexFlags flags, HexFlags mask) =>
		(flags & mask) == mask;

	public static bool HasNone (this HexFlags flags, HexFlags mask) =>
		(flags & mask) == 0;

	public static HexFlags With (this HexFlags flags, HexFlags mask) => flags | mask;

	public static HexFlags Without (this HexFlags flags, HexFlags mask) => flags & ~mask;
}
```

对于六边形地图，我们还添加了 `Has`、`With` 和 `Without` 方法，这些方法相对于起始位在特定方向上起作用。这些方法对扩展类是私有的。

```c#
	static bool Has (this HexFlags flags, HexFlags start, HexDirection direction) =>
		((int)flags & ((int)start << (int)direction)) != 0;

	static HexFlags With (this HexFlags flags, HexFlags start, HexDirection direction) =>
		flags | (HexFlags)((int)start << (int)direction);

	static HexFlags Without (
		this HexFlags flags, HexFlags start, HexDirection direction
	) =>
		flags & ~(HexFlags)((int)start << (int)direction);
```

我们使用它们来公开特定于道路方向的方法。

```c#
	public static bool HasRoad (this HexFlags flags, HexDirection direction) =>
		flags.Has(HexFlags.RoadNE, direction);

	public static HexFlags WithRoad (this HexFlags flags, HexDirection direction) =>
		flags.With(HexFlags.RoadNE, direction);
	
	public static HexFlags WithoutRoad (this HexFlags flags, HexDirection direction) =>
		flags.Without(HexFlags.RoadNE, direction);
```

### 1.2 标志而不是数组

在 `HexCell` 中添加一个标志字段。

```c#
	HexFlags flags;
```

我们现在将开始用这些标志替换道路数组的使用。首先是 `HasRoads` 属性，它可以简单地检查是否设置了任何道路标志。

```c#
	public bool HasRoads => flags.HasAny(HexFlags.Roads);
```

第二种是 `HasRoadsThroughEdge` 方法，它可以使用 `HasRoad` 标志方法来检查它。

```c#
	public bool HasRoadThroughEdge (HexDirection direction) => flags.HasRoad(direction);
```

接下来，我们将删除 `SetRoad` 方法，并将其替换为显式的 `RemoveRoad` 方法，因为添加道路只在一个地方完成。

```c#
	//void SetRoad (int index, bool state) { … }
	
	void RemoveRoad (HexDirection direction)
	{
		flags = flags.WithoutRoad(direction);
		HexCell neighbor = GetNeighbor(direction);
		neighbor.flags = neighbor.flags.WithoutRoad(direction.Opposite());
		neighbor.RefreshSelfOnly();
		RefreshSelfOnly();
	}
```

调整 `AddRoad` 和 `RemoveRoads` 以使用这些标志。

```c#
	public void AddRoad (HexDirection direction)
	{
		if (
			!flags.HasRoad(direction) && !HasRiverThroughEdge(direction) &&
			!IsSpecial && !GetNeighbor(direction).IsSpecial &&
			GetElevationDifference(direction) <= 1
		)
		{
			//SetRoad((int)direction, true);
			flags = flags.WithRoad(direction);
			HexCell neighbor = GetNeighbor(direction);
			neighbor.flags = neighbor.flags.WithRoad(direction.Opposite());
			neighbor.RefreshSelfOnly();
			RefreshSelfOnly();
		}
	}
	
	public void RemoveRoads ()
	{
		for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
		{
			if (flags.HasRoad(d))
			{
				RemoveRoad(d);
			}
		}
	}
```

同时调整 `Elevation` 设置器。

```c#
	public int Elevation
	{
		get => elevation;
		set
		{
			…

			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
			{
				if (flags.HasRoad(d) && GetElevationDifference(d) > 1)
				{
					RemoveRoad(d);
				}
			}

			Refresh();
		}
	}
```

我们还必须调整道路数据的保存和加载。由于我们已经将它们存储为位标志，因此代码变得更简单，因为直接强制转换是可能的。

```c#
	public void Save (BinaryWriter writer)
	{
		…
		
		//int roadFlags = 0;
		//for (int i = 0; i < roads.Length; i++) { … }
		writer.Write((byte)(flags & HexFlags.Roads));
		writer.Write(IsExplored);
	}
	
	public void Load (BinaryReader reader, int header)
	{
		…
		
		//int roadFlags = reader.ReadByte();
		//for (int i = 0; i < roads.Length; i++) { … }
		flags |= (HexFlags)reader.ReadByte();

		IsExplored = header >= 3 ? reader.ReadBoolean() : false;
		ShaderData.RefreshTerrain(this);
		ShaderData.RefreshVisibility(this);
	}
```

最后，我们删除了道路数组字段。由于消除了通过数组的间接性，单元现在变得更小，甚至性能也更高。

```c#
	//[SerializeField]
	//bool[] roads;
```

## 2 河流

我们为道路所做的，也可以为河流所做。我们将河流数据存储在两个布尔值和两个方向字段中，而不是一个数组中，但在我们的单个标志字段中，所有河流数据都有足够的空间。

### 2.1 河标志

使用 `HexFlags` 的接下来的六位表示输入的河流方向，之后的六位用于输出的河流方向。还要为流入、流出和任何河流定义掩码。请注意，这种数据结构允许每个单元格有多条流入和流出的河流，但我们的六边形地图不支持这一点。

```c#
	Roads = 0b111111,

	RiverInNE = 0b000001_000000,
	RiverInE  = 0b000010_000000,
	RiverInSE = 0b000100_000000,
	RiverInSW = 0b001000_000000,
	RiverInW  = 0b010000_000000,
	RiverInNW = 0b100000_000000,

	RiverIn = 0b111111_000000,

	RiverOutNE = 0b000001_000000_000000,
	RiverOutE  = 0b000010_000000_000000,
	RiverOutSE = 0b000100_000000_000000,
	RiverOutSW = 0b001000_000000_000000,
	RiverOutW  = 0b010000_000000_000000,
	RiverOutNW = 0b100000_000000_000000,

	RiverOut = 0b111111_000000_000000,

	River = 0b111111_111111_000000
```

为河流添加扩展方法，就像为道路添加扩展方法一样。

```c#
	public static bool HasRiverIn (this HexFlags flags, HexDirection direction) =>
		flags.Has(HexFlags.RiverInNE, direction);

	public static HexFlags WithRiverIn (this HexFlags flags, HexDirection direction) =>
		flags.With(HexFlags.RiverInNE, direction);

	public static HexFlags WithoutRiverIn (this HexFlags flags, HexDirection direction) =>
		flags.Without(HexFlags.RiverInNE, direction);

	public static bool HasRiverOut (this HexFlags flags, HexDirection direction) =>
		flags.Has(HexFlags.RiverOutNE, direction);

	public static HexFlags WithRiverOut (this HexFlags flags, HexDirection direction) =>
		flags.With(HexFlags.RiverOutNE, direction);

	public static HexFlags WithoutRiverOut (
		this HexFlags flags, HexDirection direction
	) =>
		flags.Without(HexFlags.RiverOutNE, direction);
```

我们还需要获取河流方向，因此添加一个私有方法将标志转换为方向，并将其转换为最低的六位。这假设六个比特中只有一个被设置。

```c#
	static HexDirection ToDirection (this HexFlags flags, int shift) =>
		 (((int)flags >> shift) & 0b111111) switch
		 {
			 0b000001 => HexDirection.NE,
			 0b000010 => HexDirection.E,
			 0b000100 => HexDirection.SE,
			 0b001000 => HexDirection.SW,
			 0b010000 => HexDirection.W,
			 _ => HexDirection.NW
		 };
```

使用该方法，添加公共方法以获取进出河流的方向。

```c#
	public static HexDirection RiverInDirection (this HexFlags flags) =>
		flags.ToDirection(6);

	public static HexDirection RiverOutDirection (this HexFlags flags) =>
		flags.ToDirection(12);
```

### 2.2 标志而非字段

这一次，我们首先移除旧的河字段。

```c#
	//bool hasIncomingRiver, hasOutgoingRiver;
	//HexDirection incomingRiver, outgoingRiver;
```

调整 `HasIncomingRiver`、`HasOutgoingRiver` 和 `HasRiver` 属性。

```c#
	public bool HasIncomingRiver => flags.HasAny(HexFlags.RiverIn);

	public bool HasOutgoingRiver => flags.HasAny(HexFlags.RiverOut);

	public bool HasRiver => flags.HasAny(HexFlags.River);
```

调整 `HasRiverBeginOrEnd` 属性以使用这些属性，而不是删除的字段。

```c#
	public bool HasRiverBeginOrEnd => HasIncomingRiver != HasOutgoingRiver;
```

删除 `RiverBeginOrEndDirection` 属性，因为删除字段后，获取河流方向不再是易事。

```c#
	//public HexDirection RiverBeginOrEndDirection =>
		//hasIncomingRiver ? incomingRiver : outgoingRiver;
```

我们保留并调整明确获取流入或流出河流方向的属性。

```c#
	public HexDirection IncomingRiver => flags.RiverInDirection();

	public HexDirection OutgoingRiver => flags.RiverOutDirection();
```

`HasRiverThroughEdge` 方法也需要更新。

```c#
	public bool HasRiverThroughEdge (HexDirection direction) =>
		flags.HasRiverIn(direction) || flags.HasRiverOut(direction);
```

我们添加了一个新的 `HasIncomingRiverThroughEdge` 方法，该方法根据给定的方向检查标志。

```c#
	public bool HasIncomingRiverThroughEdge (HexDirection direction) =>
		flags.HasRiverIn(direction);
```

接下来，调整 `RemoveIncomingRiver` 和 `RemoveOutgoingRiver` 以使用标志。

```c#
	public void RemoveIncomingRiver ()
	{
		if (!HasIncomingRiver)
		{
			return;
		}
		//hasIncomingRiver = false;
		//RefreshSelfOnly();
		
		HexCell neighbor = GetNeighbor(IncomingRiver);
		//neighbor.hasOutgoingRiver = false;
		flags = flags.Without(HexFlags.RiverIn);
		neighbor.flags = neighbor.flags.Without(HexFlags.RiverOut);
		neighbor.RefreshSelfOnly();
		RefreshSelfOnly();
	}

	public void RemoveOutgoingRiver ()
	{
		if (!HasOutgoingRiver)
		{
			return;
		}
		//hasOutgoingRiver = false;
		//RefreshSelfOnly();
		
		HexCell neighbor = GetNeighbor(OutgoingRiver);
		//neighbor.hasIncomingRiver = false;
		flags = flags.Without(HexFlags.RiverOut);
		neighbor.flags = neighbor.flags.Without(HexFlags.RiverIn);
		neighbor.RefreshSelfOnly();
		RefreshSelfOnly();
	}
```

随后是 `SetOutgoingRiver`。

```c#
	public void SetOutgoingRiver (HexDirection direction)
	{
		if (flags.HasRiverOut(direction))
		{
			return;
		}

		…

		RemoveOutgoingRiver();
		if (flags.HasRiverIn(direction))
		{
			RemoveIncomingRiver();
		}
		//hasOutgoingRiver = true;
		//outgoingRiver = direction;
		
		flags = flags.WithRiverOut(direction);
		specialIndex = 0;
		neighbor.RemoveIncomingRiver();
		//neighbor.hasIncomingRiver = true;
		//neighbor.incomingRiver = direction.Opposite();
		neighbor.flags = neighbor.flags.WithRiverIn(direction.Opposite());
		neighbor.specialIndex = 0;

		RemoveRoad(direction);
	}
```

`ValidateRiver` 现在也必须使用河流属性。

```c#
	void ValidateRivers ()
	{
		if (HasOutgoingRiver && !IsValidRiverDestination(GetNeighbor(OutgoingRiver)))
		{
			RemoveOutgoingRiver();
		}
		if (
			HasIncomingRiver && !GetNeighbor(IncomingRiver).IsValidRiverDestination(this)
		)
		{
			RemoveIncomingRiver();
		}
	}
```

我们保持保存格式不变，因此 `Save` 只需切换到使用属性即可。

```c#
		if (HasIncomingRiver)
		{
			writer.Write((byte)(IncomingRiver + 128));
		}
		else
		{
			writer.Write((byte)0);
		}

		if (HasOutgoingRiver)
		{
			writer.Write((byte)(OutgoingRiver + 128));
		}
		else
		{
			writer.Write((byte)0);
		}
```

而 `Load` 必须设置标志。

```c#
		byte riverData = reader.ReadByte();
		if (riverData >= 128)
		{
			//hasIncomingRiver = true;
			//incomingRiver = (HexDirection)(riverData - 128);
			flags = flags.WithRiverIn((HexDirection)(riverData - 128));
		}
		//else { … }

		riverData = reader.ReadByte();
		if (riverData >= 128)
		{
			//hasOutgoingRiver = true;
			//outgoingRiver = (HexDirection)(riverData - 128);
			flags = flags.WithRiverOut((HexDirection)(riverData - 128));
		}
		//else { … }
```

### 2.3 块状物

我们还必须调整 `HexGridChunk` 以使用新方法。让 `TriangulateWaterShore` 使用新的 `HasIncomingRiverThroughEdge` 方法。

```c#
	void TriangulateWaterShore (
		HexDirection direction, HexCell cell, HexCell neighbor, Vector3 center
	) {
		…

		if (cell.HasRiverThroughEdge(direction)) {
			TriangulateEstuary(
				e1, e2, cell.HasIncomingRiverThroughEdge(direction), indices
			);
		}
		else { … }

		…
	}
```

更改 `TriangulateLoadAdjacentToRiver`，使其缓存河流方向，因为这些不再是简单的字段查找。

```c#
	void TriangulateRoadAdjacentToRiver (
		HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e
	) {
		…

		HexDirection riverIn = cell.IncomingRiver, riverOut = cell.OutgoingRiver;

		if (cell.HasRiverBeginOrEnd) {
			roadCenter += HexMetrics.GetSolidEdgeMiddle(
				(cell.HasIncomingRiver ? riverIn : riverOut).Opposite()
			) * (1f / 3f);
		}
		else if (riverIn == riverOut.Opposite()) {
			…
			roadCenter += corner * 0.5f;
			if (riverIn == direction.Next() && (
				cell.HasRoadThroughEdge(direction.Next2()) ||
				cell.HasRoadThroughEdge(direction.Opposite())
			)) {
				features.AddBridge(roadCenter, center - corner * 0.5f);
			}
			center += corner * 0.25f;
		}
		else if (riverIn == riverOut.Previous()) {
			roadCenter -= HexMetrics.GetSecondCorner(riverIn) * 0.2f;
		}
		else if (riverIn == riverOut.Next()) {
			roadCenter -= HexMetrics.GetFirstCorner(riverIn) * 0.2f;
		}
		…
	}
```

让 `TriangulateWithRiver` 也使用 `HasIncomingRiverThroughEdge`。

```c#
	void TriangulateWithRiver (
		HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e
	) {
		…

		if (!cell.IsUnderwater) {
			bool reversed = cell.HasIncomingRiverThroughEdge(direction);
			…
		}
	}
```

`TriangulateConnection` 也是如此。

```c#
	void TriangulateConnection (
		HexDirection direction, HexCell cell, EdgeVertices e1
	) {
		…

		if (hasRiver) {
			…

			if (!cell.IsUnderwater) {
				if (!neighbor.IsUnderwater) {
					TriangulateRiverQuad(
						e1.v2, e1.v4, e2.v2, e2.v4,
						cell.RiverSurfaceY, neighbor.RiverSurfaceY, 0.8f,
						cell.HasIncomingRiverThroughEdge(direction),
						indices
					);
				}
				…
			}
			…
		}

		…
	}
```

## 3 邻居

邻居数据也存储在一个数组中，我们接下来将删除它。虽然我们可以在标志中存储邻居是否存在，但我们不会，因为它们通常确实存在。

### 3.1 穿越网格

如果不直接引用其邻居，HexCell 需要引用六边形网格。为此给它一个属性。

```c#
	public HexGrid Grid
	{ get; set; }
```

我们对 `HexGrid` 进行了轻微的调整，巩固了 `GetCell` 的边界检查，使其只有两条出口路径，而不是三条。

```c#
	public HexCell GetCell (HexCoordinates coordinates)
	{
		int z = coordinates.Z;
		//if (z < 0 || z >= CellCountZ) {
			//return null;
		//}
		int x = coordinates.X + z / 2;
		if (z < 0 || z >= CellCountZ || x < 0 || x >= CellCountX)
		{
			return null;
		}
		return cells[x + z * CellCountX];
	}
```

我们添加了一个 `TryGetCell` 方法来替代 `GetCell`，它的性能略高，使用起来也更方便。

```c#
	public bool TryGetCell (HexCoordinates coordinates, out HexCell cell)
	{
		int z = coordinates.Z;
		int x = coordinates.X + z / 2;
		if (z < 0 || z >= CellCountZ || x < 0 || x >= CellCountX)
		{
			cell = null;
			return false;
		}
		cell = cells[x + z * CellCountX];
		return true;
	}
```

此外，在 `HexCoordinates` 中添加一个 `Step` 方法，可以轻松获取直接邻居的六边形坐标。

```c#
	public HexCoordinates Step (HexDirection direction) => direction switch
	{
		HexDirection.NE => new HexCoordinates(x, z + 1),
		HexDirection.E => new HexCoordinates(x + 1, z),
		HexDirection.SE => new HexCoordinates(x + 1, z - 1),
		HexDirection.SW => new HexCoordinates(x, z - 1),
		HexDirection.W => new HexCoordinates(x - 1, z),
		_ => new HexCoordinates(x - 1, z + 1)
	};
```

### 3.2 不再有数组

从 `HexCell` 中删除邻居数组。

```c#
	//[SerializeField]
	//HexCell[] neighbors;
```

调整其 `GetNeighbor` 方法以遍历网格，并包含 `TryGetNeighbor` 变体。通过网格与通过邻居数组相当。找到邻居坐标需要更多的工作，但现在所有东西都通过一个中心点，而不是通过分散的单独数组，因此内存访问得到了改善。

```c#
	public HexCell GetNeighbor (HexDirection direction) =>
		Grid.GetCell(Coordinates.Step(direction));
		
	public bool TryGetNeighbor (HexDirection direction, out HexCell cell) =>
		Grid.TryGetCell(Coordinates.Step(direction), out cell);
```

删除 `SetNeighbor` 方法，因为它不再适用。

```c#
	//public void SetNeighbor (HexDirection direction, HexCell cell) { … }
```

`GetEdgeType` 现在必须使用 `GetNeighbor`。我们仍然假设邻居存在，因为只有这样才能调用此方法。

```c#
	public HexEdgeType GetEdgeType (HexDirection direction) => HexMetrics.GetEdgeType(
		elevation, GetNeighbor(direction).elevation
	);
```

同时调整 `Refresh`，使其使用 `TryGetNeighbor`。

```c#
	void Refresh ()
	{
		if (Chunk)
		{
			Chunk.Refresh();
			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
			{
				//HexCell neighbor = neighbors[i];
				if (TryGetNeighbor(d, out HexCell neighbor) && neighbor.Chunk != Chunk)
				{
					neighbor.Chunk.Refresh();
				}
			}
			…
		}
	}
```

### 3.3 网格

`HexGrid.CreateCell` 现在必须设置单元的网格，不再需要连接邻居。

```c#
	void CreateCell (int x, int z, int i)
	{
		…

		HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
		cell.Grid = this;
		…
		
		//if (x > 0) {
			//cell.SetNeighbor(HexDirection.W, cells[i - 1]);
			//…
		//}
		//if (z > 0) {
			//…
		//}
		
		Text label = Instantiate<Text>(cellLabelPrefab);
		…
	}
```

同时调整 `Search`，使其使用 `TryGetNeighbor`。

```c#
	bool Search (HexCell fromCell, HexCell toCell, HexUnit unit)
	{
		…
		while (searchFrontier.Count > 0)
		{
			…
			
			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
			{
				//HexCell neighbor = current.GetNeighbor(d);
				if (
					//neighbor == null ||
					!current.TryGetNeighbor(d, out HexCell neighbor) ||
					neighbor.SearchPhase > searchFrontierPhase
				)
				{
					continue;
				}
				…
			}
		}
		return false;
	}
```

`GetVisibleCells` 也是如此。

```c#
	List<HexCell> GetVisibleCells (HexCell fromCell, int range)
	{
		…
		while (searchFrontier.Count > 0)
		{
			…

			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
			{
				//HexCell neighbor = current.GetNeighbor(d);
				if (
					//neighbor == null ||
					!current.TryGetNeighbor(d, out HexCell neighbor) ||
					neighbor.SearchPhase > searchFrontierPhase ||
					!neighbor.Explorable
				)
				{
					continue;
				}
				
				…
			}
		}
		return visibleCells;
	}
```

### 3.4 块

调整 `HexGridChunk`，使其也依赖于 `TryGetNeighbor`，从 `TriangulateWater` 开始。

```c#
	void TriangulateWater (
		HexDirection direction, HexCell cell, Vector3 center
	) {
		center.y = cell.WaterSurfaceY;

		//HexCell neighbor = cell.GetNeighbor(direction);
		if (
			cell.TryGetNeighbor(direction, out HexCell neighbor) && !neighbor.IsUnderwater
		) {
			TriangulateWaterShore(direction, cell, neighbor, center);
		}
		…
	}
```

对 `TriangulateOpenWater`、`TriangulateWaterShore` 和 `TriangulateConnection` 执行相同的操作。

### 3.5 地图生成器

`HexMapGenerator` 也可以使用 `TryGetNeighbor`。`RaiseTerrain` 排名第一。

```c#
	int RaiseTerrain (int chunkSize, int budget, MapRegion region) {
		…
		while (size < chunkSize && searchFrontier.Count > 0) {
			…

			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
				//HexCell neighbor = current.GetNeighbor(d);
				if (
					current.TryGetNeighbor(d, out HexCell neighbor) &&
					neighbor.SearchPhase < searchFrontierPhase
				) {
					…
				}
			}
		}
		…
	}
```

还有 `SinkTerrain`、`ErodeLand`、`IsErodible`、`GetErosionTarget`、`EvolveClimate`、`CreateRivers`、`CreateRiver` 和 `SetTerrainType`。

### 3.6 地图编辑器

`HexMapEditor` 还有一个地方，我们可以在 `EditCell` 中使用 `TryGetNeighbor`。

```c#
	void EditCell (HexCell cell)
	{
		if (cell)
		{
			…
			if (
				isDrag &&
				cell.TryGetNeighbor(dragDirection.Opposite(), out HexCell otherCell)
			) {
				//HexCell otherCell = cell.GetNeighbor(dragDirection.Opposite());
				//if (otherCell) {
				if (riverMode == OptionalToggle.Yes)
				{
					otherCell.SetOutgoingRiver(dragDirection);
				}
				if (roadMode == OptionalToggle.Yes)
				{
					otherCell.AddRoad(dragDirection);
				}
				//}
			}
		}
	}
```

## 4 墙壁与探索

在本教程中，我们转化为标志的最终数据是墙和探索状态。这些是布尔字段，占用的空间与我们的单个标志字段相同，但每个字段可以容纳一个比特。

### 4.1 墙壁

为 `HexFlags` 添加一些墙。

```c#
	River = 0b111111_111111_000000,

	Walled = 0b1_000000_000000_000000
```

然后从 `HexCell` 中移除 `walled` 字段。

```c#
	//bool walled;
```

调整 `Walled` 属性，使其使用位标志。

```c#
	public bool Walled
	{
		get => flags.HasAny(HexFlags.Walled);
		set
		{
			HexFlags newFlags =
				value ? flags.With(HexFlags.Walled) : flags.Without(HexFlags.Walled);
			if (flags != newFlags)
			{
				//walled = value;
				flags = newFlags;
				Refresh();
			}
		}
	}
```

将 `Save` 更改为使用该属性。

```c#
		writer.Write(Walled);
```

和 `Load`，以便在需要时设置标志。

```c#
		//walled = reader.ReadBoolean();
		if (reader.ReadBoolean())
		{
			flags = flags.With(HexFlags.Walled);
		}
```

### 4.2 探索

为 `HexFlags` 添加已探索和可探索状态的位。

```c#
	Walled = 0b1_000000_000000_000000,

	Explored   = 0b010_000000_000000_000000,
	Explorable = 0b100_000000_000000_000000
```

然后从 `HexCell` 中删除 `explored` 字段。

```c#
	//bool explored;
```

更改 `IsExplored` 属性以使用标志。

```c#
	public bool IsExplored
	{
		get => flags.HasAll(HexFlags.Explored | HexFlags.Explorable);
		private set => flags = value ?
			flags.With(HexFlags.Explored) : flags.Without(HexFlags.Explored);
	}
```

`Explorable` 是一个独立的属性，但现在它也必须引用标志。

```c#
	public bool Explorable
	{
		get => flags.HasAny(HexFlags.Explorable);
		set => flags = value ?
			flags.With(HexFlags.Explorable) : flags.Without(HexFlags.Explorable);
	}
```

创建地图时设置单元格是否可探索。因此，当 `Load` 被调用时，它已经被适当地设置了。为了明确这一点，请在加载开始时清除所有标志，但可探索的标志除外。

```c#
	public void Load (BinaryReader reader, int header)
	{
		flags &= HexFlags.Explorable;
		…
	}
```

我们的单元格在这一点上变得更为精简。我们将在未来继续努力。

下一个教程是 [Hex Map 3.0.0](https://catlikecoding.com/unity/hex-map/3-0-0/)。

[license](https://catlikecoding.com/unity/tutorials/license/)

[repository](https://bitbucket.org/catlikecoding-projects/hex-map-project/src/ffa341353ffa5cf8f7c6d06e0d8e148298f2c70f/?at=release%2F2.3.0)

[PDF](https://catlikecoding.com/unity/hex-map/2-3-0/Hex-Map-2-3-0.pdf)

# Hex Map 3.0.0：不再有单元格游戏对象

发布于 2023-11-09

https://catlikecoding.com/unity/hex-map/3-0-0/

*升级到 Unity 2022。*
*只能将单元格存储在一个地方。*
*消除单元格游戏对象。*

![img](https://catlikecoding.com/unity/hex-map/3-0-0/tutorial-image.jpg)

*完全缩小时的阴影。*

本教程使用 Unity 2022.3.12f1 编写，遵循 Hex Map 2.3.0。

## 1 项目升级

在本教程中，我们将从 2021 LTS 升级到 2022 LTS。具体到 Unity 2022.3.12f1、URP 14.0.9 和 Burst 1.8.9。尽管该项目将继续正常运行，但我们认为这是一个破坏向后兼容性的更改，因此将我们的主发布版本增加到 3。

我们得到的唯一警告是，我们必须升级道路着色器图中的变换节点。我们可以简单地做到这一点，这没什么区别。

### 1.1 输入播放模式选项

从现在开始，我还启用了***项目设置/编辑器/输入播放模式选项***，禁用了域和场景重新加载。该项目在这些设置下运行良好，这加快了在编辑器中进入播放模式的速度。

### 1.2 阴影

我已将 URP 资源的“***阴影/最大距离***”增加到 400，以便始终具有阴影，即使在完全缩小时也是如此。***深度偏差***已减小到 0.5，以避免过多的平移（peter-panning）。这是一个性能测试，对于某些系统来说可能太重，在这种情况下，您可以将最大值设置回 150。

我一直禁用“***保守包围球体***”，因为它会导致渲染许多无用的阴影。问题是这个设置不适合最大距离。更好的阴影调整是未来需要研究的事情。

我已经关闭了所有***农场***预制件的阴影投射，因为它们太平了，反正阴影都看不见。当许多农场都在视野中时，这可以节省大量（批处理）的平局调用。

### 1.3 地形纹理数组

从 Unity 没有纹理数组的导入选项开始，地形纹理数组就是用自定义检查器制作的。现在，这得到了适当的支持，我已经将单个纹理和自定义资源替换为作为纹理数组导入的单个图集。

![img](https://catlikecoding.com/unity/hex-map/3-0-0/project-upgrade/terrain-texture-array.png)

*单个图集中的所有五种地形纹理。*

这也解决了导出到移动设备时地形纹理损坏的问题，因为 DXT1 压缩格式已被烘焙到资源中。现在，它将使用构建目标的适当格式导出。

### 1.4 代码清理

我已经完成了从早期版本开始的 C# 代码风格现代化。

***Scripts/Editor*** 文件夹及其内的脚本已被删除。这是纹理数组导出器和六边形坐标抽屉（hex coordinates drawer），两者都不再使用。

未使用的本地原始 `originalViewElevation` 变量已从 `HexCell.Elevation` 和 `HexCell.WaterLevel` 属性 setter 中删除。未使用的 `direction` 参数已从 `HexGridChunk.TriangulateWithRiverBeginOrEnd` 方法及其调用中删除。

当位置和旋转都设置在一起时，使用了 `Transform.SetLocalPositionAndRotation` 方法，`HexFeatureManager.AddFeature` 和 `HexFeatureManager.AddSpecialFeature` 方法就是这种情况。

## 2 更多单元格工作

在前面的教程中，我们开始精简 `HexCell`。这次我们继续这个过程，不再让它扩展 `MonoBehaviour`，也不再使用单元格游戏对象。在我们采取这一步骤之前，我们必须在整个项目中进行一些更改。由于 `HexCell` 将变成一个可序列化的类，我们必须确保单元格只存储在一个地方，否则在热重新加载后会得到重复的单元格。

### 2.1 着色器数据

我们首先关注 `HexCellShaderData`。我们首先向 `HexGrid` 添加 `ShaderData` 属性。

```c#
	public HexCellShaderData ShaderData => cellShaderData;
```

这允许我们从 `HexCell` 中删除相同的属性，因为我们现在可以从网格中检索它，而不是在每个单元格中存储对它的引用。

```c#
	//public HexCellShaderData ShaderData
	//{ get; set; }
```

将 `ShaderData` 的所有用法替换为 `Grid.ShaderData`，以保持一切正常工作。同时从 `HexGrid.CreateCell` 中删除对已删除属性的赋值。

```c#
		//cell.ShaderData = cellShaderData;
```

接下来，重构 `HexCellShaderData` 的 `transitioningCells` 列表，将其重命名为 `transitioningCellIndices`，并将其元素类型更改为 `int`。从现在开始，它将存储单元格索引，而不是直接引用单元格。

```c#
	List<int> transitioningCellIndices = new();
```

在 `RefreshVisibility` 中，我们现在必须存储单元格的索引。

```c#
			transitioningCellIndices.Add(cell.Index);
```

并将 `UpdateCellData` 的第一个参数也更改为索引。我们现在必须从网格中获取单元格以访问其数据。

```c#
	bool UpdateCellData(int index, int delta)
	{
		//int index = cell.Index;
		HexCell cell = Grid.GetCell(index);
		…
	}
```

当需要访问单元格数据时，这确实增加了一定程度的间接性，但开销很小，不明显。

### 2.2 索引路径

`HexCell` 也不应该直接引用其他单元格。其用于寻路的 `PathFrom` 属性仍然如此。将其替换为 `PathFromIndex` 属性。

```c#
	//public HexCell PathFrom
	//{ get; set; }

	public int PathFromIndex
	{ get; set; }
```

然后调整 `HexGrid.GetPath`，使其循环继续工作。

```c#
		for (HexCell c = currentPathTo; c != currentPathFrom; c = cells[c.PathFromIndex])
		{
			path.Add(c);
		}
```

同时修复 `ClearPath` 和 `ShowPath`。

```c#
				//current = current.PathFrom;
				current = cells[current.PathFromIndex];
```

还有 `Search` 中的两项任务。

```c#
					//neighbor.PathFrom = current;
					neighbor.PathFromIndex = current.Index;
```

我们也可以为 `HexCell.NextWithSamePriority` 做这件事，但由于它仅在搜索时使用，我们可以将其支持字段设置为非序列化。整个寻路系统应该在未来进行重构，所以我们现在不会做得很漂亮。

```c#
	[field: System.NonSerialized]
	public HexCell NextWithSamePriority
	{ get; set; }
```

出于同样的原因，我们不需要调整 `HexCellPriorityQueue`，因为它本身无法承受热重新加载。

### 2.3 路径

下一步是用索引替代项替换 `HexGrid` 的 `currentPathFrom` 和 `currentPathTo` 字段。我们将在所有地方使用 -1 来表示缺少单元格索引，替换 `null`。

```c#
	//HexCell currentPathFrom, currentPathTo;
	int currentPathFromIndex = -1, currentPathToIndex = -1;
```

更改 `GetPath`，使其与索引一起工作并返回索引列表。

```c#
	public List<int> GetPath()
	{
		…
		List<int> path = ListPool<int>.Get();
		for (HexCell c = cells[currentPathToIndex];
			c.Index != currentPathFromIndex;
			c = cells[c.PathFromIndex])
		{
			path.Add(c.Index);
		}
		path.Add(currentPathFromIndex);
		…
	}
```

同时调整 `ClearPath`、`ShowPath` 和 `FindPath`。

```c#
	public void ClearPath()
	{
		if (currentPathExists)
		{
			HexCell current = cells[currentPathToIndex];
			while (current.Index != currentPathFromIndex)
			{
				…
			}
			…
		}
		else if (currentPathFromIndex >= 0)
		{
			cells[currentPathFromIndex].DisableHighlight();
			cells[currentPathToIndex].DisableHighlight();
		}
		currentPathFromIndex = currentPathToIndex = -1;
	}

	void ShowPath(int speed)
	{
		if (currentPathExists)
		{
			HexCell current = cells[currentPathToIndex];
			while (current.Index != currentPathFromIndex)
			{
				…
			}
		}
		cells[currentPathFromIndex].EnableHighlight(Color.blue);
		cells[currentPathToIndex].EnableHighlight(Color.red);
	}

	public void FindPath(HexCell fromCell, HexCell toCell, HexUnit unit)
	{
		ClearPath();
		currentPathFromIndex = fromCell.Index;
		currentPathToIndex = toCell.Index;
		…
	}
```

### 2.4 单位

`HexUnit` 跟踪其位置和当前旅行位置，作为对单元格的直接引用。也用索引替换它们。其 `pathToTravel` 字段也必须成为索引列表。

```c#
	//HexCell location, currentTravelLocation;
	int locationCellIndex = -1, currentTravelLocationCellIndex = -1;
	
	…
	
	List<int> pathToTravel;
```

调整其 `Location` 属性，使其在内部与位置索引一起工作。同时调整 `ValidateLocation`。

```c#
	public HexCell Location
	{
		get => Grid.GetCell(locationCellIndex);
		set
		{
			if (locationCellIndex >= 0)
			{
				HexCell location = Grid.GetCell(locationCellIndex);
				Grid.DecreaseVisibility(location, VisionRange);
				location.Unit = null;
			}
			locationCellIndex = value.Index;
			…
		}
	}
	
	…
	
	public void ValidateLocation() =>
		transform.localPosition = Grid.GetCell(locationCellIndex).Position;
```

`Travel` 方法现在必须与索引列表一起使用。

```c#
	public void Travel(List<int> path)
	{
		HexCell location = Grid.GetCell(locationCellIndex);
		location.Unit = null;
		location = Grid.GetCell(path[^1]);
		locationCellIndex = location.Index;
		…
	}
```

> **`^1` 的作用是什么？**
>
> 它是列表最后一个索引的简写，因此列表的长度减 1。

同时调整 `TravelPath`，使其与索引兼容。我们可以改变更多的方法来传递单元格索引，而不是把单元格放在这里，但我们把它留到以后，做最少的工作来保持功能。`HexCell` 的当前状态仍然是暂时的。一旦我们为未来的单元格得出最终的数据表示，我们就可以进行一次清理代码的过程。

```c#
	IEnumerator TravelPath()
	{
		Vector3 a, b, c = Grid.GetCell(pathToTravel[0]).Position;
		yield return LookAt(Grid.GetCell(pathToTravel[1]).Position);

		if (currentTravelLocationCellIndex < 0)
		{
			currentTravelLocationCellIndex = pathToTravel[0];
		}
		HexCell currentTravelLocation = Grid.GetCell(currentTravelLocationCellIndex);
		…
		for (int i = 1; i < pathToTravel.Count; i++)
		{
			currentTravelLocation = Grid.GetCell(pathToTravel[i]);
			currentTravelLocationCellIndex = currentTravelLocation.Index;
			a = c;
			b = Grid.GetCell(pathToTravel[i - 1]).Position;

			…
			Grid.IncreaseVisibility(Grid.GetCell(pathToTravel[i]), VisionRange);

			…
			Grid.DecreaseVisibility(Grid.GetCell(pathToTravel[i]), VisionRange);
			t -= 1f;
		}
		currentTravelLocationCellIndex = -1;

		HexCell location = Grid.GetCell(locationCellIndex);
		…
		…
		ListPool<int>.Add(pathToTravel);
		pathToTravel = null;
	}
```

`Die` 和 `Save` 方法也必须进行调整。由于死亡的单位总是有一个位置，我们可以在这里取消对有效位置的检查。

```c#
	public void Die()
	{
		HexCell location = Grid.GetCell(locationCellIndex);
		//if (location)
		//{
		Grid.DecreaseVisibility(location, VisionRange);
		//}
		…
	}
	
	…
	
	public void Save(BinaryWriter writer)
	{
		Grid.GetCell(locationCellIndex).Coordinates.Save(writer);
		writer.Write(orientation);
	}
```

最后一个需要调整的 `HexUnit` 方法是 `OnEnable`。

```c#
	void OnEnable()
	{
		if (locationCellIndex >= 0)
		{
			HexCell location = Grid.GetCell(locationCellIndex);
			transform.localPosition = location.Position;
			if (currentTravelLocationCellIndex >= 0)
			{
				HexCell currentTravelLocation =
					Grid.GetCell(currentTravelLocationCellIndex);
				Grid.IncreaseVisibility(location, VisionRange);
				Grid.DecreaseVisibility(currentTravelLocation, VisionRange);
				currentTravelLocationCellIndex = -1;
			}
		}
	}
```

我还删除了用于路径调试可视化的旧注释掉的代码，因为它现在已经失效了。

### 2.5 游戏用户界面

`HexGameUI` 跟踪当前单元格，该单元格也应成为索引。

```c#
	//HexCell currentCell;
	int currentCellIndex = -1;
```

更新 `DoSelection` 和 `DoPathfinding` 以使用索引。

```c#
	void DoSelection()
	{
		grid.ClearPath();
		UpdateCurrentCell();
		if (currentCellIndex >= 0)
		{
			selectedUnit = grid.GetCell(currentCellIndex).Unit;
		}
	}

	void DoPathfinding()
	{
		if (UpdateCurrentCell())
		{
			if (currentCellIndex >= 0 &&
				selectedUnit.IsValidDestination(grid.GetCell(currentCellIndex)))
			{
				grid.FindPath(
					selectedUnit.Location, grid.GetCell(currentCellIndex), selectedUnit);
			}
			else
			{
				grid.ClearPath();
			}
		}
	}
```

还有 `UpdateCurrentCell`。在这种情况下，我们必须插入一个 `null` 检查，将其替换为 -1。

```c#
	bool UpdateCurrentCell()
	{
		HexCell cell = grid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
		int index = cell ? cell.Index : -1;
		if (index != currentCellIndex)
		{
			currentCellIndex = index;
			return true;
		}
		return false;
	}
```

### 2.6 地图编辑器

`HexMapEditor` 还引用了用于拖动事件的前一个单元格。也用索引替换它。

```c#
	//HexCell previousCell;
	int previousCellIndex = -1;
```

调整所有依赖于前一个单元格的代码。

```c#
	void Update()
	{
		…
		previousCellIndex = -1;
	}
	
	…
	
	void HandleInput()
	{
		HexCell currentCell = GetCellUnderCursor();
		if (currentCell)
		{
			if (previousCellIndex >= 0 && previousCellIndex != currentCell.Index)
			{
				ValidateDrag(currentCell);
			}
			else
			{
				isDrag = false;
			}
			EditCells(currentCell);
			previousCellIndex = currentCell.Index;
		}
		else
		{
			previousCellIndex = -1;
		}
		UpdateCellHighlightData(currentCell);
	}
	
	…
	
	void ValidateDrag(HexCell currentCell)
	{
		for (…)
		{
			if (hexGrid.GetCell(previousCellIndex).GetNeighbor(dragDirection) ==
				currentCell)
			{
				isDrag = true;
				return;
			}
		}
		isDrag = false;
	}
```

### 2.7 网格块

最后，`HexGridChunk` 将其所有单元格存储在一个数组中。这需要成为一个索引数组。首先，为网格赋予一个属性。

```c#
	public HexGrid Grid
	{ get; set; }
```

在 `HexGrid.CreateChunk` 中设置它。

```c#
				HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
				chunk.transform.SetParent(columns[x], false);
				chunk.Grid = this;
```

然后更改 `HexGridChunk` 中的数组和使用它的所有代码。

```c#
	//HexCell[] cells;
	int[] cellIndices;

	Canvas gridCanvas;

	void Awake()
	{
		gridCanvas = GetComponentInChildren<Canvas>();
		cellIndices = new int[HexMetrics.chunkSizeX * HexMetrics.chunkSizeZ];
	}

	public void AddCell(int index, HexCell cell)
	{
		//cells[index] = cell;
		cellIndices[index] = cell.Index;
		…
	}

	…
	
	public void Triangulate()
	{
		…
		for (int i = 0; i < cellIndices.Length; i++)
		{
			Triangulate(Grid.GetCell(cellIndices[i]));
		}
		…
	}
```

### 2.8 不再是游戏对象

现在我们已经清理了代码，单元格只存储在网格的数组中，忽略了瞬态搜索数据，因此一旦单元格不再是 Unity 组件，热重新加载就不会创建重复的单元格。

首先，删除 ***Hex Cell*** 预制件。然后使 `HexCell` 不再扩展 `MonoBehaviour`，而是使其可序列化。

```c#
[System.Serializable]
public class HexCell //: MonoBehaviour
{ … }
```

其次，由于单元格不再具有 `Transform` 组件，因此将 `Position` 转换为 auto 属性，并使用该属性代替 `RefreshPosition` 中的旧局部位置。

```c#
	public Vector3 Position //=> transform.localPosition;
	{ get; set; }

…

	void RefreshPosition()
	{
		Vector3 position = Position;
		…
		Position = position;

		…
	}
```

第三，代码中有很多地方是 Unity 从 `UnityEngine.Object` 隐式转换为布尔值，用于检测单元格的存在。为了保持所有这些工作，请向执行 `null` 检查的布尔运算符方法添加一个隐式转换。

```c#
	public static implicit operator bool(HexCell cell) => cell != null;
```

第四，`HexGridChunk.AddCell` 不再需要设置单元格的父级，因为 Unity 的层次结构中不再存在单元格。

```c#
		//cell.transform.SetParent(transform, false);
```

最后，从 `HexGrid` 中删除 `cellPrefab` 配置字段，并调整 `CreateCell`，使其创建一个新的 `HexCell` 对象并设置其位置。

```c#
	//[SerializeField]
	//HexCell cellPrefab;
	
	…
	
	void CreateCell(int x, int z, int i)
	{
		…

		var cell = cells[i] = new HexCell();
		cell.Grid = this;
		cell.Position = position;
		…
	}
```

我们的项目应该仍然完全一样，但现在没有单元格游戏对象。这让我们离让单元格与 Burst 作业兼容又近了一步，但我们还没有做到。请注意，每个单元格仍然有 UI 游戏对象，这些对象是单独存在的。

下一个教程是 [Hex Map 3.1.0](https://catlikecoding.com/unity/hex-map/3-1-0/)。

[license](https://catlikecoding.com/unity/tutorials/license/)

[repository](https://bitbucket.org/catlikecoding-projects/hex-map-project/src/d61dc821ca56b301c339e4df6ff066fbf972bc0e/?at=release%2F3.0.0)

[PDF](https://catlikecoding.com/unity/hex-map/3-0-0/Hex-Map-3-0-0.pdf)

# Hex Map 3.1.0：提取六边形值

发布于 2023-12-14

https://catlikecoding.com/unity/hex-map/3-1-0/

*进一步简化单元格。*
*在一个整数中存储七个值。*

![img](https://catlikecoding.com/unity/hex-map/3-1-0/tutorial-image.jpg)

*很久以前，单元格开始小而简单。*

本教程使用 Unity 2022.3.15f1 编写，紧跟着 [Hex Map 3.0.0](https://catlikecoding.com/unity/hex-map/3-0-0/)。

## 1 更多代码清理

在前面的教程中，我们将 `HexCell` 从 `MonoBehaviour` 降级为常规类。我们继续努力，进一步简化它，清理一些代码，并修复一个错误。

我不会在本教程中显示它，但我更改了 C# 代码，使所有行的最大宽度为 80 个字符。这使得在编辑或比较代码修订时，使用并排打开的两个代码视口更容易。我还将 C# 使用的代码风格应用于 HLSL。

### 1.1 保存和加载六边形标志

让我们首先将用于保存和加载存储在 `HexFlags` 中的数据的代码移动到其扩展方法中。我们必须在 ***HexFlags.cs*** 文件中使用这个 `System.IO` 命名空间。

```c#
using System.IO;
```

添加一个包含 `HexCell.Save` 中代码副本的 `Save` 扩展方法。然后删除与 `HexFlags` 数据无关的代码，使其直接访问标志。

```c#
	public static void Save(this HexFlags flags, BinaryWriter writer)
	{
		//…
		writer.Write(flags.HasAny(HexFlags.Walled));

		if (flags.HasAny(HexFlags.RiverIn))
		{
			writer.Write((byte)(flags.RiverInDirection() + 128));
		}
		else
		{
			writer.Write((byte)0);
		}

		if (flags.HasAny(HexFlags.RiverOut))
		{
			writer.Write((byte)(flags.RiverOutDirection() + 128));
		}
		else
		{
			writer.Write((byte)0);
		}

		writer.Write((byte)(flags & HexFlags.Roads));
		writer.Write(flags.HasAll(HexFlags.Explored | HexFlags.Explorable));
	}
```

现在我们可以通过将工作转发到标记来简化 `HexCell.Save`。

```c#
	public void Save(BinaryWriter writer)
	{
		…
		//writer.Write(Walled);
		//…
		//writer.Write(IsExplored);

		flags.Save(writer);
	}
```

同样，为 `HexFlags` 添加一个 `Load` 扩展方法，其中包含以相同方式修改代码的 `HexCell.Load` 的副本。使其成为一种基于基值的扩展方法，用于保持单元格的可探索状态。与 `HexFlags` 一样，它返回结果，在这种情况下是加载的标志。

```c#
	public static HexFlags Load(
		this HexFlags basis, BinaryReader reader, int header)
	{
		HexFlags flags = basis & HexFlags.Explorable;
		//…
		
		if (reader.ReadBoolean())
		{
			flags = flags.With(HexFlags.Walled);
		}

		byte riverData = reader.ReadByte();
		if (riverData >= 128)
		{
			flags = flags.WithRiverIn((HexDirection)(riverData - 128));
		}

		riverData = reader.ReadByte();
		if (riverData >= 128)
		{
			flags = flags.WithRiverOut((HexDirection)(riverData - 128));
		}

		flags |= (HexFlags)reader.ReadByte();

		//IsExplored = header >= 3 && reader.ReadBoolean();
		if (header >= 3 && reader.ReadBoolean())
		{
			flags = flags.With(HexFlags.Explored);
		}
		return flags;
	}
```

现在我们还可以简化 `HexCell.Load`。

```c#
	public void Load(BinaryReader reader, int header)
	{
		//flags &= HexFlags.Explorable;
		…

		flags = flags.Load(reader, header);
		RefreshPosition();
		…

		//if (reader.ReadBoolean())
		//{
			//flags = flags.With(HexFlags.Walled);
		//}
		//…
		//IsExplored = header >= 3 && reader.ReadBoolean();
		Grid.ShaderData.RefreshTerrain(this);
		Grid.ShaderData.RefreshVisibility(this);
	}
```

> **为什么不直接保存并加载标志值？**
>
> 一旦我们确定了最终的单元格结构，我们将在将来切换到新的安全格式时使用这种方法。但目前我们保持当前的格式。

### 1.2 六边形单元格简化

我们通过做一些更改进一步简化了 `HexCell`。首先，`RemoveIncomingRiver`、`RemoveOutgoingRiver` 和 `GetElevationDifference` 仅由单元格本身使用，因此我们不再将其 `public`。

其次，我们将在 `HexCell` 中的所有地方都只使用 getter 属性，而不是访问 `terrainTypeIndex` 等字段。我们还将在 `Load` 中通过中间变量指定正确的标高，而不是临时存储偏移值。这使得压缩所有这些值成为可能，我们很快就会这样做。我只显示 `Load` 中最后一位的代码更改。

```c#
		flags &= HexFlags.Explorable;
		TerrainTypeIndex = reader.ReadByte();
		int elevation = reader.ReadByte();
		if (header >= 4)
		{
			elevation -= 127;
		}
		Elevation = elevation;
		RefreshPosition();
	
```

第三，删除 `RefreshSelfOnly` 方法，转而直接调用 `Chunk.Refresh`。在这些情况下，根本不需要刷新单位位置，所以我们取消了它。

第四，删除私人 `IsExplored` 设置器。我们只在 `IncreaseVisibility` 中将其设置为 `true`，所以我们直接在那里设置标志数据。

```c#
	public void IncreaseVisibility()
	{
		visibility += 1;
		if (visibility == 1)
		{
			//IsExplored = true;
			flags = flags.With(HexFlags.Explored);
			Grid.ShaderData.RefreshVisibility(this);
		}
	}
```

第五，从 `HexCell` 和 `HexCellShaderData` 中删除未使用的调试方法 `SetMapData`。

最后，删除带有 `HexDirection` 参数的 `GetEdgeType` 方法。它在 `HexGridChunk.TriangulateConnection` 中只使用一次，我们可以使用具有 `HexCell` 参数的另一个版本，将其传递给邻居。

```c#
		if (cell.GetEdgeType(neighbor) == HexEdgeType.Slope)
		{
			TriangulateEdgeTerraces(e1, cell, e2, neighbor, hasRoad);
		}
```

### 1.3 保存和加载修复

地图的保存和加载代码中存在一个错误，这可能会导致地图在保存时丢失。当将地图保存到加载速度足够快的同一文件时，可能会发生共享违规，从而终止保存过程。这是因为我们依赖垃圾收集器来处理写入器和读取器对象。

修复方法是调整 `SaveLoadMenu`，以便在我们不再需要时立即处理写入器和读取器。最简单的方法是依靠使用模式。在这种情况下，我们可以在 `Save` 中的 `writer` 变量声明前添加 `using` 关键字，在 `Load` 中的 `reader` 变量声明前也添加 `using` 关键字。这可确保在方法完成时立即关闭并释放文件。

```c#
	void Save (string path)
	{
		using var writer = new BinaryWriter(File.Open(path, FileMode.Create));
		…
	}

	void Load(string path)
	{
		…
		using var reader = new BinaryReader(File.OpenRead(path));
		…
	}
```

> **`using` 有什么作用？**
>
> 我们使用的是它的最短版本，它隐式地为变量创建一个 `using` 作用域块，直到方法返回。它确保在方法返回之前，无论是正常还是通过抛出异常，都会对变量调用 `Dispose`。

## 2 分组六边形值

早些时候，我们引入了一个 `HexValues` 枚举类型，它在单个 `int` 中压缩了多个位标志，大大减小了 `HexCell` 的大小。我们将再次做同样的事情，通过在一个 `int` 中存储其他七个值来进一步减小单元格大小。

### 2.1 六边形值结构

创建一个新的可序列化的 `HexValues` 结构，该结构封装一个私有的 `int value` 变量。我们将把不同的值存储到它的位中。这些值不是标志，而是范围缩小的整数。因此，每个存储值只占用几个比特。为此，创建一个具有私有 `int value` 字段的可序列化 `HexValues` 结构。

```c#
using System.IO;

[System.Serializable]
public struct HexValues
{
	int values;
}
```

我们的想法是，我们将其视为一种不可变的值类型，如 `HexFlags` 或常规 `int`，因此所有方法都是 `readonly`。然而，为了支持热重载所需的 Unity 序列化，我们不能使用 `readonly`。您的代码编辑器可能会建议将我们的 `values` 字段标记为 `readonly`，因此让我们禁用该警告。

```c#
#pragma warning disable IDE0044 // Add readonly modifier
	int values;
#pragma warning restore IDE0044 // Add readonly modifier
```

为了从 `values` 中提取值，我们添加了一个私有的 `Get` 方法，该方法具有位掩码和移位量作为参数。我们使用这些来移位位，使所需的位在右侧——最低有效位——然后屏蔽它们以返回正确的位。

```c#
	readonly int Get(int mask, int shift) => (values >> shift) & mask;
```

为了设置一个值，我们添加了一个私有的 `With` 方法，该方法的值及其掩码和移位作为参数包含在内。我们使用这些来清除 `values` 目标位，并插入给定的值，适当地屏蔽并向左移动。这用于返回新值。

```c#
	readonly HexValues With(int value, int mask, int shift) => new()
	{
		values = (values & ~(mask << shift)) | ((value & mask) << shift)
	};
```

请注意，如果我们假设给定的值始终在正确的范围内，我们可以跳过屏蔽它，但为了安全起见，我们还是屏蔽它吧。

### 2.2 高程

我们将在 `HexValues` 中存储的第一个值是单元格高度。我们将把它存储在最右侧的五个位中。五位给了我们一个 0 到 31 的范围，这已经足够了。此操作的位掩码为 `0b11111`，其最大允许值为 31，移位为零。使用此选项创建使用 `Get` 的公共 `Elevation` getter属性。

```c#
	public readonly int Elevation => Get(31, 0);
```

但海拔可能是负的。为了支持这一点，让我们将范围移动 15，使其变为 -15~16，这仍然是足够的，而不改变存储的值。我们通过在得到它时减去 15 来做到这一点。

```c#
	public readonly int Elevation => Get(31, 0) - 15;
```

我们不能使用 setter 属性，因为我们将 `HexValues` 视为不可变的。因此，我们添加了一个公共 `WithElevation` 方法，该方法返回插入给定高程值的 `HexValues` 数据。在这种情况下，我们必须在存储高程之前将其添加 15。

```c#
	public readonly HexValues WithElevation(int value) =>
		With(value + 15, 31, 0);
```

### 2.3 级别

使用相同的方法包括水位、城市级别、农场级别和工厂级别。将水位设置为与标高相同的范围，因此其掩码为31。将其存放在立面旁边，因此移动了 5。其他级别各有四种可能的状态，因此它们的掩码为 3。移动它们，使它们一个接一个地放置。

```c#
	public readonly int WaterLevel => Get(31, 5);

	public readonly HexValues WithWaterLevel(int value) => With(value, 31, 5);
	
	public readonly int UrbanLevel => Get(3, 10);

	public readonly HexValues WithUrbanLevel(int value) => With(value, 3, 10);

	public readonly int FarmLevel => Get(3, 12);

	public readonly HexValues WithFarmLevel(int value) => With(value, 3, 12);

	public readonly int PlantLevel => Get(3, 14);

	public readonly HexValues WithPlantLevel(int value) => With(value, 3, 14);
```

### 2.4 索引

我们存储的最后两个值是特殊索引和地形类型索引。地形类型索引上升到 255，我们将对特殊索引使用相同的范围。

```c#
	public readonly int SpecialIndex => Get(255, 16);

	public readonly HexValues WithSpecialIndex(int index) =>
		With(index, 255, 16);
	
	public readonly int TerrainTypeIndex => Get(255, 24);
	
	public readonly HexValues WithTerrainTypeIndex(int index) =>
		With(index, 255, 24);
```

现在我们已经使用了所有 32 位。数据格式看起来像 `TTTTTTTTT SSSSSS PPFFUUWW WWWEEEE`。然而，在向右移动时，我们必须注意不要将最左侧的位视为整数符号位。我们可以通过执行逻辑转换来实现这一点。在 C# 11 中，我们可以使用 `>>>` 来实现这一点，但 Unity 还不支持，所以我们只能使用显式的 unsigned shift，强制转换为 `uint`。

```c#
	readonly int Get(int mask, int shift) =>
		(int)((uint)values >> shift) & mask;
```

> **如果我们不使用逻辑移位，会发生什么？**
>
> 然后我们会执行正确的算术移位。它保留了整数的符号，这意味着向右移动负整数将插入 1 而不是 0。

### 2.2 保存和加载

让我们还添加保存和加载 `HexValues` 的方法，同样保持相同的保存格式。

```c#
	public readonly void Save(BinaryWriter writer)
	{
		writer.Write((byte)TerrainTypeIndex);
		writer.Write((byte)(Elevation + 127));
		writer.Write((byte)WaterLevel);
		writer.Write((byte)UrbanLevel);
		writer.Write((byte)FarmLevel);
		writer.Write((byte)PlantLevel);
		writer.Write((byte)SpecialIndex);
	}
	
	public static HexValues Load(BinaryReader reader, int header)
	{
		HexValues values = default;
		values = values.WithTerrainTypeIndex(reader.ReadByte());
		int elevation = reader.ReadByte();
		if (header >= 4)
		{
			elevation -= 127;
		}
		values = values.WithElevation(elevation);
		values = values.WithWaterLevel(reader.ReadByte());
		values = values.WithUrbanLevel(reader.ReadByte());
		values = values.WithFarmLevel(reader.ReadByte());
		values = values.WithPlantLevel(reader.ReadByte());
		return values.WithSpecialIndex(reader.ReadByte());
	}
```

### 2.6 六边形单元格

要使用压缩值，请将 `HexCell` 中的七个字段替换为单个 `HexValues` 字段。

```c#
	//int terrainTypeIndex;

	//int elevation = int.MinValue;
	//int waterLevel;

	//int urbanLevel, farmLevel, plantLevel;

	//int specialIndex;
	
	HexValues values;
```

然后根据需要调整删除字段的所有使用情况。此时，它们只能在各自的 `HexCell` 属性中使用。我只显示了 `Elevation` 所需的更改。

```c#
	public int Elevation
	{
		get => values.Elevation;
		set
		{
			if (values.Elevation == value)
			{
				return;
			}
			//elevation = value;
			values = values.WithElevation(value);
			…
		}
	}
```

最后一步是将值保存并加载到 `HexValues`。`HexCell` 的 `Save` 和 `Load` 方法现在已经大大简化。

```c#
	public void Save(BinaryWriter writer)
	{
		values.Save(writer);
		flags.Save(writer);
	}

	public void Load(BinaryReader reader, int header)
	{
		//…
		values = HexValues.Load(reader, header);
		flags = flags.Load(reader, header);
		RefreshPosition();
		//…
		Grid.ShaderData.RefreshTerrain(this);
		Grid.ShaderData.RefreshVisibility(this);
	}
```

我们越来越接近微小而简单的单元格，但我们还没有做到。在未来的教程中，还有更多的工作要做。

下一个教程是 [Hex Map 3.2.0](https://catlikecoding.com/unity/hex-map/3-2-0/)。

[license](https://catlikecoding.com/unity/tutorials/license/)

[repository](https://bitbucket.org/catlikecoding-projects/hex-map-project/src/126c29c5638baf152469d4230d87cca273e3f0ee/?at=release%2F3.1.0)

[PDF](https://catlikecoding.com/unity/hex-map/3-1-0/Hex-Map-3-1-0.pdf)

# Hex Map 3.2.0：分离搜索数据

发布于 2024-02-29

https://catlikecoding.com/unity/hex-map/3-2-0/

![img](https://catlikecoding.com/unity/hex-map/3-2-0/tutorial-image.jpg)

*单元格没有意识到这一点，但寻路和可见性仍然有效。*

本教程使用 Unity 2022.3.20f1 编写，遵循 [Hex Map 3.1.0](https://catlikecoding.com/unity/hex-map/3-1-0/)。

## 1 单元格搜索数据

在前两个教程中，我们已经大大减小了 `HexCell` 的大小，但它仍然包含许多与其当前状态没有直接关系的数据。这一次，我们将从中提取仅由我们的搜索算法使用的所有数据。

我们的搜索算法是 A* 用于寻路，对地图生成和可见性确定进行了轻微调整。我们使用优先级队列，其实现基于桶列表，特别是 LIFO 堆栈列表，因为它为六边形网格生成最佳路径。堆栈被实现为链表。

### 1.1 搜索数据结构

搜索数据由六条数据组成，目前都存储在 `HexCell` 中。我们创建了一个新的 `HexCellSearchData` 结构来存储它们。虽然我们可以像压缩其他单元格数据一样压缩这些值，但我们会保持简单，并将它们定义为单独的公共字段。未来可以进行进一步的优化。结构体必须是可序列化的，这样它才能在热重新加载中幸存下来。

搜索数据由距离、对具有相同优先级的下一个单元格的引用、对路径来源的单元格的引用，搜索启发式和搜索阶段组成。这个想法是，在某个时候，我们将不再使用游戏对象作为单元格，所以我们将使用单元格索引而不是对象引用。因此，所有字段都是整数。还有一个属性可以获得搜索优先级，即距离和启发式的总和。

```c#
[System.Serializable]
public struct HexCellSearchData
{
	public int distance;
	
	public int nextWithSamePriority;

	public int pathFrom;

	public int heuristic;

	public int searchPhase;

	public readonly int SearchPriority => distance + heuristic;
}
```

我们不会按单元格存储搜索数据，而是将其存储在 `HexGrid` 中的一个数组中，与单元格数组放在一起。

```c#
	HexCellSearchData[] searchData;

	…
	
	void CreateCells()
	{
		cells = new HexCell[CellCountZ * CellCountX];
		searchData = new HexCellSearchData[cells.Length];

		…
	}
```

### 1.2 路径

`GetPath` 方法始终返回单元格索引列表。现在我们可以更改它，使其根本不访问单元格，只处理搜索数据。

```c#
	public List<int> GetPath()
	{
		…
		List<int> path = ListPool<int>.Get();
		for (int i = currentPathToIndex;
			i != currentpathFrom;
			i = searchData[i].pathFrom)
		{
			path.Add(i);
		}
		…
	}
```

`ClearPath` 和 `ShowPath` 方法仍然需要访问单元格本身以更改其可视化，但现在应该使用新的距离和路径数据。

```c#
	public void ClearPath()
	{
		…
				current.SetLabel(null);
				current.DisableHighlight();
				current = cells[searchData[current.Index].pathFrom];
		…
	}

	void ShowPath(int speed)
	{
		…
				int turn = (searchData[current.Index].distance - 1) / speed;
				current.SetLabel(turn.ToString());
				current.EnableHighlight(Color.white);
				current = cells[searchData[current.Index].pathFrom];
		…
	}
```

这暂时中断了寻路，但一旦我们完全重构了搜索算法以使用新数据，它就会恢复工作。

### 1.3 优先级队列

下一步是更改 `HexCellPriorityQueue`，使其也仅适用于索引，而不是单元格本身。这要求它访问 `HexGrid` 存储的搜索数据，因此给它一个引用网格的字段和一个初始化它的构造函数。

```c#
	readonly List<int> list = new();

	readonly HexGrid grid;

	public HexCellPriorityQueue(HexGrid grid) => this.grid = grid;
```

要访问搜索数据数组，请在 `HexGrid` 中为其添加一个公共 getter 属性。

```c#
	public HexCellSearchData[] SearchData => searchData;
```

在我们更改 `HexCellPriorityQueue` 的同时，让我们也去掉它的 `count` 字段，因为如果我们稍微改变一下出列方法，它就不是真正必要的了。

```c#
	//int count = 0;
	
	…
	
	public void Clear()
	{
		//count = 0;
		…
	}
```

调整 `Enqueue`，使其存储单元格索引并使用网格的搜索数据。使用 -1 表示空优先级，替换 `null`。

```c#
	public void Enqueue(int cellIndex)
	{
		//count += 1;
		int priority = grid.SearchData[cellIndex].SearchPriority;
		if (priority < minimum)
		{
			minimum = priority;
		}
		while (priority >= list.Count)
		{
			list.Add(-1);
		}
		grid.SearchData[cellIndex].nextWithSamePriority = list[priority];
		list[priority] = cellIndex;
	}
```

调整 `Dequeue` 以匹配，并将其重构为 `TryDequeue`，将单元格索引作为输出参数，返回是否找到索引。

```c#
	public bool TryDequeue(out int cellIndex)
	{
		//count -= 1;
		for (; minimum < list.Count; minimum++)
		{
			cellIndex = list[minimum];
			if (cellIndex >= 0)
			{
				list[minimum] = grid.SearchData[cellIndex].nextWithSamePriority;
				return true;
			}
		}
		cellIndex = -1;
		return false;
	}
```

给 `Change` 同样的处理，再次用索引替换单元格引用，并使用网格的搜索数据。

```c#
	public void Change(int cellIndex, int oldPriority)
	{
		int current = list[oldPriority];
		int next = grid.SearchData[current].nextWithSamePriority;
		if (current == cellIndex)
		{
			list[oldPriority] = next;
		}
		else
		{
			while (next != cellIndex)
			{
				current = next;
				next = grid.SearchData[current].nextWithSamePriority;
			}
			grid.SearchData[current].nextWithSamePriority =
				grid.SearchData[cellIndex].nextWithSamePriority;
		}
		Enqueue(cellIndex);
	}
```

这些更改会导致编译失败，直到我们更新了搜索代码以使用新方法，我们接下来会这样做。

### 1.4 搜索

我们从主要的寻路方法 `HexGrid.Search` 开始。第一步是在创建网格时将网格本身传递给搜索边界。让我们通过始终清除搜索边界来简化代码，即使是在第一次创建网格时也是如此。然后，我们为第一个单元格设置搜索数据，通过创建新的 `HexCellSearchData` 值进行简化。之后，我们将第一个索引排队，而不是对单元格本身的引用。

```c#
		int speed = unit.Speed;
		searchFrontierPhase += 2;
		//if (searchFrontier == null)
		//{
		searchFrontier ??= new HexCellPriorityQueue(this);
		//}
		//else
		//{
		searchFrontier.Clear();
		//}

		searchData[fromCell.Index] = new HexCellSearchData
		{
			searchPhase = searchFrontierPhase
		};
		//fromCell.Distance = 0;
		searchFrontier.Enqueue(fromCell.Index);
```

只要出列成功，A* 搜索循环就会继续，而不是检查队列的计数。然后，我们检索当前单元格，计算其距离一次，并使用新的搜索数据增加其搜索阶段。

```c#
		while (searchFrontier.TryDequeue(out int currentIndex))
		{
			HexCell current = cells[currentIndex];
			int currentDistance = searchData[currentIndex].distance;
			searchData[currentIndex].searchPhase += 1;

			if (current == toCell)
			{
				return true;
			}

			int currentTurn = (currentDistance - 1) / speed;

			…
		}
```

当循环遍历一个单元格的邻居时，我们必须延迟检查邻居的搜索阶段，直到我们可以检索到它的数据。

```c#
				if (!current.TryGetNeighbor(d, out HexCell neighbor) /||
					//neighbor.SearchPhase > searchFrontier)
				{
					continue;
				}
				HexCellSearchData neighborData = searchData[neighbor.Index];
				if (neighborData.searchPhase > searchFrontierPhase ||
					!unit.IsValidDestination(neighbor))
				{
					continue;
				}
```

其余的代码仍然做同样的事情，除了它现在应该处理索引和网格的搜索数据。

```c#
				int distance = currentDistance + moveCost;
				int turn = (distance - 1) / speed;
				if (turn > currentTurn)
				{
					distance = turn * speed + moveCost;
				}

				if (neighborData.searchPhase < searchFrontierPhase)
				{
					searchData[neighbor.Index] = new HexCellSearchData
					{
						searchPhase = searchFrontierPhase,
						distance = distance,
						pathFrom = currentIndex,
						heuristic = neighbor.Coordinates.DistanceTo(
							toCell.Coordinates)
					};
					searchFrontier.Enqueue(neighbor.Index);
				}
				else if (distance < neighborData.distance)
				{
					//int oldPriority = neighbor.SearchPriority;
					searchData[neighbor.Index].distance = distance;
					searchData[neighbor.Index].pathFrom = currentIndex;
					searchFrontier.Change(
						neighbor.Index, neighborData.SearchPriority);
				}
```

### 1.5 生成地图

接下来我们修复 `HexMapGenerator`。在创建网格的搜索边界时，将网格传递到 `GenerateMap` 中的新队列，并切换到重置网格的搜索数据。

```c#
	public void GenerateMap(int x, int z, bool wrapping)
	{
		…
		searchFrontier ??= new HexCellPriorityQueue(grid);
		…
		for (int i = 0; i < cellCount; i++)
		{
			grid.SearchData[i].searchPhase = 0;
		}

		Random.state = originalRandomState;
	}
```

`RaiseTerrain` 执行一个更简单的搜索变体，以选择要提升的本地单元格组。我们只需使用其他值的默认值设置第一个单元格的搜索阶段。

```c#
		searchFrontierPhase += 1;
		HexCell firstCell = GetRandomCell(region);
		grid.SearchData[firstCell.Index] = new HexCellSearchData
		{
			searchPhase = searchFrontierPhase
		};
		//firstCell.Distance = 0;
		//firstCell.SearchHeuristic = 0;
		searchFrontier.Enqueue(firstCell.Index);
```

同样，搜索循环必须切换到新的出列方法，并使用网格的搜索数据。

```c#
		while (size < chunkSize && searchFrontier.TryDequeue(out int index))
		{
			HexCell current = grid.GetCell(index);
			…

			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
			{
				if (current.TryGetNeighbor(d, out HexCell neighbor) &&
					grid.SearchData[neighbor.Index].searchPhase <
						searchFrontierPhase)
				{
					grid.SearchData[neighbor.Index] = new HexCellSearchData
					{
						searchPhase = searchFrontierPhase,
						distance = neighbor.Coordinates.DistanceTo(center),
						heuristic = Random.value < jitterProbability ? 1 : 0
					};
					searchFrontier.Enqueue(neighbor.Index);
				}
			}
		}
```

对 `LowerTerrain` 进行相同的更改。之后，我们重构了传统的搜索代码，但我们的项目仍然无法编译，因为我们在确定可见性时也执行了不同类型的搜索。

## 2 单元格可见性

单元格可见性是通过在地图中搜索来确定的。当我们将搜索数据从单元格移动到网格时，让我们也将单元格可见性数据移动到那里。这是有道理的，因为一个单元格当前是否可见不仅取决于单元格本身，还取决于单位，网格同时管理这两者。

### 2.1 通过网格实现可见性

在 `HexGrid` 中添加一个整数数组字段以跟踪单元格可见性。

```c#
	int[] cellVisibility;

	…

	void CreateCells()
	{
		cells = new HexCell[CellCountZ * CellCountX];
		searchData = new HexCellSearchData[cells.Length];
		cellVisibility = new int[cells.Length];

		…
	}
```

还添加一个公共方法来检查给定索引的单元格是否可见。目前，单元格具有 `IsVisible` 属性，该属性还可以检查它们是否可探索，但这不是必需的，因此我们将不再在此处包含该检查。

```c#
	public bool IsCellVisible(int cellIndex) => cellVisibility[cellIndex] > 0;
```

调整 `HexCellShaderData.RefreshVisibility`，因此它依赖于网格来检查单元格的可见性。

```c#
			cellTextureData[index].r =
				Grid.IsCellVisible(cell.Index) ? (byte)255 : (byte)0;
```

对 `HexCellShaderData.UpdateCellData` 执行相同的操作。

```c#
		if (Grid.IsCellVisible(cell.Index))
		{
			if (data.r < 255)
			{
				…
			}
		}
```

### 2.2 更改可见性

更改单元格可见性现在由 `HexGrid` 全权负责。但是，当单元格首次可见时，应将其标记为已探索。引入公共 `HexCell.MarkAsExplored` 方法使这成为可能。

```c#
	public void MarkAsExplored() => flags = flags.With(HexFlags.Explored);
```

现在调整 `HexGrid.IndexVisibility`，使其完成 `HexCell.IncreaseVisibility` 过去做的工作。增加所有找到的单元格的可见性，如果它们的可见性增加到 1，则标记并刷新它们。

```c#
	public void IncreaseVisibility(HexCell fromCell, int range)
	{
		List<HexCell> cells = GetVisibleCells(fromCell, range);
		for (int i = 0; i < cells.Count; i++)
		{
			//cells[i].IncreaseVisibility();
			if (++cellVisibility[cells[i].Index] == 1)
			{
				cells[i].MarkAsExplored();
				cellShaderData.RefreshVisibility(cells[i]);
			}
		}
		ListPool<HexCell>.Add(cells);
	}
```

以相同的方式调整 `DecreaseVisibility` 和 `ResetVisibility`。

```c#
	public void DecreaseVisibility(HexCell fromCell, int range)
	{
		List<HexCell> cells = GetVisibleCells(fromCell, range);
		for (int i = 0; i < cells.Count; i++)
		{
			//cells[i].DecreaseVisibility();
			if (--cellVisibility[cells[i].Index] == 0)
			{
				cellShaderData.RefreshVisibility(cells[i]);
			}
		}
		ListPool<HexCell>.Add(cells);
	}

	public void ResetVisibility()
	{
		for (int i = 0; i < cells.Length; i++)
		{
			//cells[i].ResetVisibility();
			if (cellVisibility[i] > 0)
			{
				cellVisibility[i] = 0;
				cellShaderData.RefreshVisibility(cells[i]);
			}
		}
		…
	}
```

我们必须更改的最后一种方法是 `GetVisibleCells`。像其他搜索方法一样调整其开头。唯一值得注意的区别是，我们应该确保数据的路径不会被意外清除，因为可能需要清除当前路径。因此，我们将简单地复制现有值。不这样做可能会导致 `ClearPath` 失败，可能会陷入无限循环。

```c#
		searchFrontierPhase += 2;
		//if (searchFrontier == null)
		//{
		searchFrontier ??= new HexCellPriorityQueue(this);
		//}
		//else
		//{
		searchFrontier.Clear();
		//}
		
		range += fromCell.ViewElevation;
		searchData[fromCell.Index] = new HexCellSearchData
		{
			searchPhase = searchFrontierPhase,
			pathFrom = searchData[fromCell.Index].pathFrom
		};
		//fromCell.Distance = 0;
		searchFrontier.Enqueue(fromCell.Index);
```

像以前一样更改其搜索循环。

```c#
		while (searchFrontier.TryDequeue(out int currentIndex))
		{
			HexCell current = cells[currentIndex];
			searchData[currentIndex].searchPhase += 1;
			visibleCells.Add(current);
			
			…
		}
```

并调整邻居循环以处理搜索数据。我们必须再次确保我们不会改变数据的路径。

```c#
			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
			{
				if (!current.TryGetNeighbor(d, out HexCell neighbor)) //||
				{
					continue;
				}
				HexCellSearchData neighborData = searchData[neighbor.Index];
				if (neighborData.searchPhase > searchFrontierPhase ||
					!neighbor.Explorable)
				{
					continue;
				}

				int distance = searchData[currentIndex].distance + 1;
				if (distance + neighbor.ViewElevation > range ||
					distance > fromCoordinates.DistanceTo(neighbor.Coordinates))
				{
					continue;
				}

				if (neighborData.searchPhase < searchFrontierPhase)
				{
					searchData[neighbor.Index] = new HexCellSearchData
					{
						searchPhase = searchFrontierPhase,
						distance = distance,
						pathFrom = neighborData.pathFrom
					};
					//neighbor.heuristic = 0;
					searchFrontier.Enqueue(neighbor.Index);
				}
				else if (distance < searchData[neighbor.Index].distance)
				{
					//int oldPriority = neighbor.SearchPriority;
					searchData[neighbor.Index].distance = distance;
					searchFrontier.Change(
						neighbor.Index, neighborData.SearchPriority);
				}
			}
```

### 2.3 清理

此时，我们的项目应该再次编译并像以前一样运行，但现在搜索数据和可见性与单元格分离。这允许我们从 `HexCell` 中删除相当多的代码。可以删除 `IsVisible`、`Distance`、`PathFromIndex`、`SearchHeuristic`、`SearchPriority`、`SearchPhase` 和 `NextWithSamePriority` 属性。`distance` 和 `visibility` 字段也是如此。最后是 `IncreaseVisibility`、 `DecreaseVisibility` 和 `ResetVisibility` 方法。

下一个教程是 [Hex Map 3.3.0](https://catlikecoding.com/unity/hex-map/3-3-0/)。

[license](https://catlikecoding.com/unity/tutorials/license/)

[repository](https://bitbucket.org/catlikecoding-projects/hex-map-project/src/0b563ca9db05ec0d1700d9f57240844ca3990adf/?at=release%2F3.2.0)

[PDF](https://catlikecoding.com/unity/hex-map/3-2-0/Hex-Map-3-2-0.pdf)

# Hex Map 3.3.0：六边形单元格数据

发布于 2024-04-09

https://catlikecoding.com/unity/hex-map/3-3-0/

![img](https://catlikecoding.com/unity/hex-map/3-3-0/tutorial-image.jpg)

*生成和三角剖分地图，而不使用单元格对象。*

本教程使用 Unity 2022.3.22f1 编写，接着 [Hex Map 3.2.0](https://catlikecoding.com/unity/hex-map/3-2-0/)。

## 1 单元格数据

我们正在努力摆脱 `HexCell` 类。这将允许我们在某个时候将一些代码转换为 Burst 作业。这一次，我们从生成和可视化地图的代码中删除了对类的依赖。

### 1.1 六边形单元格数据

我们将把大部分单元格数据捆绑在一个名为 `HexCellData` 的新结构类型中。为 `HexFlags`、`HexValues` 和 `HexCoordinates` 指定公共字段。这些数据在生成和可视化地图时一起使用。因此，此结构的大小等于四个 32 位值，因为坐标存储为两个整数，所以总共十六个字节。

为了简化从 `HexCell` 到 `HexCellData` 的转换，我们在类中包含了一些 getter 属性和方法，它们都是只读的，如下所示。

```c#
[System.Serializable]
public struct HexCellData
{
	public HexFlags flags;

	public HexValues values;

	public HexCoordinates coordinates;

	public readonly int Elevation => values.Elevation;

	public readonly int WaterLevel => values.WaterLevel;

	public readonly int TerrainTypeIndex => values.TerrainTypeIndex;

	public readonly int UrbanLevel => values.UrbanLevel;

	public readonly int FarmLevel => values.FarmLevel;

	public readonly int PlantLevel => values.PlantLevel;

	public readonly int SpecialIndex => values.SpecialIndex;
	
	public readonly bool Walled => flags.HasAny(HexFlags.Walled);

	public readonly bool HasRoads => flags.HasAny(HexFlags.Roads);

	public readonly bool IsExplored =>
		flags.HasAll(HexFlags.Explored | HexFlags.Explorable);

	public readonly bool IsSpecial => values.SpecialIndex > 0;

	public readonly bool IsUnderwater => values.WaterLevel > values.Elevation;

	public readonly bool HasIncomingRiver => flags.HasAny(HexFlags.RiverIn);

	public readonly bool HasOutgoingRiver => flags.HasAny(HexFlags.RiverOut);

	public readonly bool HasRiver => flags.HasAny(HexFlags.River);

	public readonly bool HasRiverBeginOrEnd =>
		HasIncomingRiver != HasOutgoingRiver;
	
	public readonly HexDirection IncomingRiver => flags.RiverInDirection();

	public readonly HexDirection OutgoingRiver => flags.RiverOutDirection();
	
	public readonly float StreamBedY =>
		(values.Elevation + HexMetrics.streamBedElevationOffset) *
		HexMetrics.elevationStep;

	public readonly float RiverSurfaceY =>
		(values.Elevation + HexMetrics.waterElevationOffset) *
		HexMetrics.elevationStep;

	public readonly float WaterSurfaceY =>
		(values.WaterLevel + HexMetrics.waterElevationOffset) *
		HexMetrics.elevationStep;
	
	public readonly int ViewElevation =>
		Elevation >= WaterLevel ? Elevation : WaterLevel;
	
	public readonly HexEdgeType GetEdgeType(HexCellData otherCell) =>
		HexMetrics.GetEdgeType(values.Elevation, otherCell.values.Elevation);
	
	public readonly bool HasIncomingRiverThroughEdge(HexDirection direction) =>
		flags.HasRiverIn(direction);
	
	public readonly bool HasRiverThroughEdge(HexDirection direction) =>
		flags.HasRiverIn(direction) || flags.HasRiverOut(direction);

	public readonly bool HasRoadThroughEdge(HexDirection direction) =>
		flags.HasRoad(direction);
}
```

### 1.2 将数据移动到数组

我们在 `HexCellData` 中定义的数据将不再存储在 `HexCell` 中。相反，我们将此数据移动到 `HexGrid` 中的一个可公开访问的 `CellData` 数组中。在我们进行此操作的同时，让我们也引入一个可公共访问的 `CellPositions` 数组，以单独存储不太频繁访问的单元格位置。

```c#
	public HexCellData[] CellData
	{ get; private set; }

	public Vector3[] CellPositions
	{ get; private set; }
	
	…

	void CreateCells()
	{
		cells = new HexCell[CellCountZ * CellCountX];
		CellData = new HexCellData[cells.Length];
		CellPositions = new Vector3[cells.Length];
		…
	}
	
	…

	void CreateCell(int x, int z, int i)
	{
		…

		var cell = cells[i] = new HexCell();
		cell.Grid = this;
		CellPositions[i] = position;
		CellData[i].coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
		…
	}
```

将 `HexCell` 的 `flags` 和 `values` 字段重构为属性，这些属性充当存储在网格数组中的数据的代理。

```c#
	HexFlags Flags
	{
		get => Grid.CellData[Index].flags;
		set => Grid.CellData[Index].flags = value;
	}

	HexValues Values
	{
		get => Grid.CellData[Index].values;
		set => Grid.CellData[Index].values = value;
	}
```

对 `Coordinates` 和 `Position` 执行相同的操作，使其成为只读属性。唯一更新位置的地方可以直接设置位置数组的数据。

```c#
	public HexCoordinates Coordinates => Grid.CellData[Index].coordinates;
	
	…
	
	public Vector3 Position => Grid.CellPositions[Index];
	
	…

	void RefreshPosition()
	{
		…
		Grid.CellPositions[Index] = position;

		…
	}
```

## 2 从类切换到结构

在本教程中，我们不会完全消除 `HexCell` 的使用，我们只限于地图生成和可视化代码。这包括 `HexCellShaderData`、`HexFeatureManager`、`HexGridChunk` 和 `HexMapGenerator`，以及需要更改的任何支持代码。

### 2.1 六边形单元格着色器数据

我们从 `HexCellShaderData` 开始。它有三个具有 `HexCell` 参数的方法。我们将把这些参数更改为单元格索引，并在方法中检索所需的 `HexCellData`。首先是 `RefreshTerrain`。

```c#
	public void RefreshTerrain(int cellIndex)
	{
		HexCellData cell = Grid.CellData[cellIndex];
		Color32 data = cellTextureData[cellIndex];
		data.b = cell.IsUnderwater ?
			(byte)(cell.WaterSurfaceY * (255f / 30f)) : (byte)0;
		data.a = (byte)cell.TerrainTypeIndex;
		cellTextureData[cellIndex] = data;
		enabled = true;
	}
```

其次是 `RefreshVisibility`，它只需要在一个地方访问单元格数据，所以我们不需要将其存储在变量中。

```c#
	public void RefreshVisibility(int cellIndex)
	{
		//int index = cell.Index;
		if (ImmediateMode)
		{
			cellTextureData[cellIndex].r = Grid.IsCellVisible(cellIndex) ?
				(byte)255 : (byte)0;
			cellTextureData[cellIndex].g = Grid.CellData[cellIndex].IsExplored ?
				(byte)255 : (byte)0;
		}
		else if (!visibilityTransitions[cellIndex])
		{
			visibilityTransitions[cellIndex] = true;
			transitioningCellIndices.Add(cellIndex);
		}
		enabled = true;
	}
```

第三是 `ViewElevationChanged`。

```c#
	public void ViewElevationChanged(int cellIndex)
	{
		HexCellData cell = Grid.CellData[cellIndex];
		cellTextureData[cellIndex].b = cell.IsUnderwater ?
			(byte)(cell.WaterSurfaceY * (255f / 30f)) : (byte)0;
		needsVisibilityReset = true;
		enabled = true;
	}
```

我们还必须更改 `UpdateCellData`，以便它检索 `HexCellData`。更改后，`HexCellShaderData` 不再依赖于 `HexCell`。

```c#
	bool UpdateCellData(int index, int delta)
	{
		//HexCell cell = Grid.GetCell(index);
		Color32 data = cellTextureData[index];
		bool stillUpdating = false;

		if (Grid.CellData[index].IsExplored && data.g < 255)
		{
			…
		}

		if (Grid.IsCellVisible(index))
		{
			…
		}
		…
	}
```

现在，我们必须在 `HexCell` 中修复这些方法的调用，方法是在所有使用这些方法的地方用 `Index` 替换 `this` 参数。我们还必须修复 `HexGrid` 中增加、减少和重置可见性的方法。

```c#
	public void IncreaseVisibility(HexCell fromCell, int range)
	{
		List<HexCell> cells = GetVisibleCells(fromCell, range);
		for (int i = 0; i < cells.Count; i++)
		{
			int cellIndex = cells[i].Index;
			if (++cellVisibility[cellIndex] == 1)
			{
				cells[i].MarkAsExplored();
				cellShaderData.RefreshVisibility(cellIndex);
			}
		}
		ListPool<HexCell>.Add(cells);
	}

	public void DecreaseVisibility(HexCell fromCell, int range)
	{
		List<HexCell> cells = GetVisibleCells(fromCell, range);
		for (int i = 0; i < cells.Count; i++)
		{
			int cellIndex = cells[i].Index;
			if (--cellVisibility[cellIndex] == 0)
			{
				cellShaderData.RefreshVisibility(cellIndex);
			}
		}
		ListPool<HexCell>.Add(cells);
	}

	public void ResetVisibility()
	{
		for (int i = 0; i < cells.Length; i++)
		{
			if (cellVisibility[i] > 0)
			{
				cellVisibility[i] = 0;
				cellShaderData.RefreshVisibility(i);
			}
		}
		…
	}
```

### 2.2 三角剖分

我们将使三角剖分地图的代码依赖于 `HexCellData` 而不是 `HexCell`。首先，在某些情况下，在对水进行三角剖分时，我们需要知道单元格的列索引。我们不将索引存储在单元格数据中，也不必这样做，因为我们可以从单元格坐标中推导出它。为此，在 `HexCoordinates` 中添加 `ColumnIndex` 属性。

```c#
	public readonly int ColumnIndex => (x + z / 2) / HexMetrics.chunkSizeX;
```

其次，在 `HexFeatureManager` 中，我们可以简单地用 `HexCellData` 替换所有 `HexCell` 参数类型，而无需更改该类中的任何其他代码。

第三，我们将直接使用单元格坐标而不是单元格，因此我们重构 `HexGrid.TryGetCell` 以获取单元格索引。如果没有找到索引，则应将其设置为 -1。

```c#
	public bool TryGetCellIndex(HexCoordinates coordinates, out int cellIndex)
	{
		int z = coordinates.Z;
		int x = coordinates.X + z / 2;
		if (z < 0 || z >= CellCountZ || x < 0 || x >= CellCountX)
		{
			cellIndex = -1;
			return false;
		}
		cellIndex = x + z * CellCountX;
		return true;
	}
```

现在我们转到 `HexGridChunk`，它进行三角剖分。我们将不得不调整很多方法，我们将从上到下进行调整。

起点是使用 `HexCell` 参数进行三角剖分，我们将其替换为单元格索引参数。这要求我们检索单元格数据，以替换旧的单元格引用和单元格位置。然后，我们将索引和位置作为额外参数传递给带方向的 `Triangulate` 变量。

```c#
	public void Triangulate()
	{
		…
		for (int i = 0; i < cellIndices.Length; i++)
		{
			Triangulate(cellIndices[i]);
		}
		…
	}

	void Triangulate(int cellIndex)
	{
		HexCellData cell = Grid.CellData[cellIndex];
		Vector3 cellPosition = Grid.CellPositions[cellIndex];
		for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
		{
			Triangulate(d, cell, cellIndex, cellPosition);
		}
		if (!cell.IsUnderwater)
		{
			if (!cell.HasRiver && !cell.HasRoads)
			{
				features.AddFeature(cell, cellPosition);
			}
			if (cell.IsSpecial)
			{
				features.AddSpecialFeature(cell, cellPosition);
			}
		}
	}
```

调整基于方向的 `Triangulate` 方法的参数列表。我们还将把单元格索引作为额外参数传递给我们在这里调用的所有其他三角测量方法。在 `TriangulateConnection` 的情况下，我们还传递中心位置的 Y 坐标。

```c#
	void Triangulate(
		HexDirection direction,
		HexCellData cell,
		int cellIndex,
		Vector3 center)
	{
		var e = new EdgeVertices(
			center + HexMetrics.GetFirstSolidCorner(direction),
			center + HexMetrics.GetSecondSolidCorner(direction));

		if (cell.HasRiver)
		{
			if (cell.HasRiverThroughEdge(direction))
			{
				e.v3.y = cell.StreamBedY;
				if (cell.HasRiverBeginOrEnd)
				{
					TriangulateWithRiverBeginOrEnd(cell, cellIndex, center, e);
				}
				else
				{
					TriangulateWithRiver(direction, cell, cellIndex, center, e);
				}
			}
			else
			{
				TriangulateAdjacentToRiver(
					direction, cell, cellIndex, center, e);
			}
		}
		else
		{
			TriangulateWithoutRiver(direction, cell, cellIndex, center, e);
			if (!cell.IsUnderwater && !cell.HasRoadThroughEdge(direction))
			{
				features.AddFeature(
					cell, (center + e.v1 + e.v5) * (1f / 3f));
			}
		}

		if (direction <= HexDirection.SE)
		{
			TriangulateConnection(direction, cell, cellIndex, center.y, e);
		}

		if (cell.IsUnderwater)
		{
			TriangulateWater(direction, cell, cellIndex, center);
		}
	}
```

更深入地说，我们首先转向 `TriangulateWater`。根据需要调整其参数列表，然后让它获取邻居坐标，并使用这些坐标检查是否有不在水下的邻居。如果是这样，在对水边进行三角剖分时，还要传递单元格索引和邻居的列索引。否则，在对开阔水域进行三角剖分时，请提供单元坐标、单元索引和邻居索引作为额外参数。

```c#
	void TriangulateWater(
		HexDirection direction,
		HexCellData cell,
		int cellIndex,
		Vector3 center)
	{
		center.y = cell.WaterSurfaceY;
		HexCoordinates neighborCoordinates = cell.coordinates.Step(direction);
		if (Grid.TryGetCellIndex(neighborCoordinates, out int neighborIndex) &&
			!Grid.CellData[neighborIndex].IsUnderwater)
		{
			TriangulateWaterShore(
				direction, cell, cellIndex, neighborIndex,
				neighborCoordinates.ColumnIndex, center);
		}
		else
		{
			TriangulateOpenWater(
				cell.coordinates, direction, cellIndex, neighborIndex, center);
		}
	}
```

接下来，我们调整 `TriangulateOpenWater`。再次更改其参数列表并使用新数据。我们现在通过将邻居的索引与 -1 进行比较来检查邻居的存在。此外，访问下一个邻居会变得有点冗长，必须明确地获取朝下一个相邻邻居的方向行走的坐标。

```c#
	void TriangulateOpenWater(
		HexCoordinates coordinates,
		HexDirection direction,
		int cellIndex,
		int neighborIndex,
		Vector3 center)
	{
		…
		indices.x = indices.y = indices.z = cellIndex;
		water.AddTriangleCellData(indices, weights1);

		if (direction <= HexDirection.SE && neighborIndex != -1)
		{
			…
			indices.y = neighborIndex;
			water.AddQuadCellData(indices, weights1, weights2);

			if (direction <= HexDirection.E)
			{
				if (!Grid.TryGetCellIndex(
					coordinates.Step(direction.Next()),
					out int nextNeighborIndex) ||
					!Grid.CellData[nextNeighborIndex].IsUnderwater)
				{
					return;
				}
				water.AddTriangle(
					c2, e2, c2 + HexMetrics.GetWaterBridge(direction.Next()));
				indices.z = nextNeighborIndex;
				water.AddTriangleCellData(
					indices, weights1, weights2, weights3);
			}
		}
	}
```

`TriangulateWaterShore` 需要更多的改变，但它们具有相似的性质。我们在这里的多个地方访问相同的数据，由于现在必须提取或计算这些数据，让我们将其存储在临时变量中。

```c#
	void TriangulateWaterShore(
		HexDirection direction,
		HexCellData cell,
		int cellIndex,
		int neighborIndex,
		int neighborColumnIndex,
		Vector3 center)
	{
		…
		indices.x = indices.z = cellIndex;
		indices.y = neighborIndex;
		…

		Vector3 center2 = Grid.CellPositions[neighborIndex];
		int cellColumnIndex = cell.coordinates.ColumnIndex;
		if (neighborColumnIndex < cellColumnIndex - 1)
		{
			center2.x += HexMetrics.wrapSize * HexMetrics.innerDiameter;
		}
		else if (neighborColumnIndex > cellColumnIndex + 1)
		{
			center2.x -= HexMetrics.wrapSize * HexMetrics.innerDiameter;
		}
		…

		HexCoordinates nextNeighborCoordinates = cell.coordinates.Step(
			direction.Next());
		if (Grid.TryGetCellIndex(
			nextNeighborCoordinates, out int nextNeighborIndex))
		{
			Vector3 center3 = Grid.CellPositions[nextNeighborIndex];
			bool nextNeighborIsUnderwater =
				Grid.CellData[nextNeighborIndex].IsUnderwater;
			int nextNeighborColumnIndex = nextNeighborCoordinates.ColumnIndex;
			if (nextNeighborColumnIndex < cellColumnIndex - 1)
			{
				center3.x += HexMetrics.wrapSize * HexMetrics.innerDiameter;
			}
			else if (nextNeighborColumnIndex > cellColumnIndex + 1)
			{
				center3.x -= HexMetrics.wrapSize * HexMetrics.innerDiameter;
			}
			Vector3 v3 = center3 + (nextNeighborIsUnderwater ?
				HexMetrics.GetFirstWaterCorner(direction.Previous()) :
				HexMetrics.GetFirstSolidCorner(direction.Previous()));
			v3.y = center.y;
			waterShore.AddTriangle(e1.v5, e2.v5, v3);
			waterShore.AddTriangleUV(
				new Vector2(0f, 0f),
				new Vector2(0f, 1f),
				new Vector2(0f, nextNeighborIsUnderwater ? 0f : 1f));
			indices.z = nextNeighborIndex;
			waterShore.AddTriangleCellData(
				indices, weights1, weights2, weights3);
		}
	}
```

`TriangulateWithoutRiver` 只需要最小的更改，将 `HexCell` 替换为 `HexCellData` 并添加新的单元格索引参数。`GetRoadInterpolators` 方法只需要将其单元格参数类型更改为 `HexCellData`。

```c#
	void TriangulateWithoutRiver(
		HexDirection direction,
		HexCellData cell,
		int cellIndex,
		Vector3 center,
		EdgeVertices e)
	{
		TriangulateEdgeFan(center, e, cellIndex);

		if (cell.HasRoads)
		{
			Vector2 interpolators = GetRoadInterpolators(direction, cell);
			TriangulateRoad(
				center,
				Vector3.Lerp(center, e.v1, interpolators.x),
				Vector3.Lerp(center, e.v5, interpolators.y),
				e, cell.HasRoadThroughEdge(direction), cellIndex);
		}
	}

	Vector2 GetRoadInterpolators(HexDirection direction, HexCellData cell) { … }
```

除了将单元格参数类型更改为 `HexCellData` 外，`TriangulateAdjacentToRiver` 还只需要与索引相关的更改。

```c#
	void TriangulateAdjacentToRiver(
		HexDirection direction,
		HexCellData cell,
		int cellIndex,
		Vector3 center,
		EdgeVertices e)
	{
		if (cell.HasRoads)
		{
			TriangulateRoadAdjacentToRiver(
				direction, cell, cellIndex, center, e);
		}

		…

		TriangulateEdgeStrip(
			m, weights1, cellIndex,
			e, weights1, cellIndex);
		TriangulateEdgeFan(center, m, cellIndex);

		…
	}
```

 `TriangulateRoadAdjacentToRiver` 也是如此。

```c#
	void TriangulateRoadAdjacentToRiver(
		HexDirection direction,
		HexCellData cell,
		int cellIndex,
		Vector3 center,
		EdgeVertices e)
	{
		…
		TriangulateRoad(roadCenter, mL, mR, e, hasRoadThroughEdge, cellIndex);
		if (previousHasRiver)
		{
			TriangulateRoadEdge(roadCenter, center, mL, cellIndex);
		}
		if (nextHasRiver)
		{
			TriangulateRoadEdge(roadCenter, mR, center, cellIndex);
		}
	}
```

以及 `TriangulateWithRiverBeginOrEnd`.

```c#
	void TriangulateWithRiverBeginOrEnd(
		HexCellData cell, int cellIndex, Vector3 center, EdgeVertices e)
	{
		…

		TriangulateEdgeStrip(
			m, weights1, cellIndex,
			e, weights1, cellIndex);
		TriangulateEdgeFan(center, m, cellIndex);

		if (!cell.IsUnderwater)
		{
			bool reversed = cell.HasIncomingRiver;
			Vector3 indices;
			indices.x = indices.y = indices.z = cellIndex;
			…
		}
	}
```

以及 `TriangulateWithRiver`

```c#
	void TriangulateWithRiver(
		HexDirection direction,
		HexCellData cell,
		int cellIndex,
		Vector3 center,
		EdgeVertices e)
	{
		…

		TriangulateEdgeStrip(
			m, weights1, cellIndex,
			e, weights1, cellIndex);

		…

		Vector3 indices;
		indices.x = indices.y = indices.z = cellIndex;
		…
	}
```

`TriangulateConnection` 更为复杂。它为中心Y坐标获取一个参数。对于每个角点，我们在调用 `TriangulateCorner` 时必须传递适当的单元格索引和数据，在调用 `TrianglateEdgeTerraces` 时只传递索引。

```c#
	void TriangulateConnection(
		HexDirection direction,
		HexCellData cell,
		int cellIndex,
		float centerY,
		EdgeVertices e1)
	{
		if (!Grid.TryGetCellIndex(
			cell.coordinates.Step(direction), out int neighborIndex))
		{
			return;
		}
		HexCellData neighbor = Grid.CellData[neighborIndex];
		Vector3 bridge = HexMetrics.GetBridge(direction);
		bridge.y = Grid.CellPositions[neighborIndex].y - centerY;
		…

		if (hasRiver)
		{
			e2.v3.y = neighbor.StreamBedY;
			Vector3 indices;
			indices.x = indices.z = cellIndex;
			indices.y = neighborIndex;

			…
		}

		if (cell.GetEdgeType(neighbor) == HexEdgeType.Slope)
		{
			TriangulateEdgeTerraces(e1, cellIndex, e2, neighborIndex, hasRoad);
		}
		else
		{
			TriangulateEdgeStrip(
				e1, weights1, cellIndex,
				e2, weights2, neighborIndex, hasRoad);
		}

		features.AddWall(e1, cell, e2, neighbor, hasRiver, hasRoad);

		if (direction <= HexDirection.E &&
			Grid.TryGetCellIndex(
				cell.coordinates.Step(direction.Next()),
				out int nextNeighborIndex))
		{
			HexCellData nextNeighbor = Grid.CellData[nextNeighborIndex];
			Vector3 v5 = e1.v5 + HexMetrics.GetBridge(direction.Next());
			v5.y = Grid.CellPositions[nextNeighborIndex].y;

			if (cell.Elevation <= neighbor.Elevation)
			{
				if (cell.Elevation <= nextNeighbor.Elevation)
				{
					TriangulateCorner(
						e1.v5, cellIndex, cell,
						e2.v5, neighborIndex, neighbor,
						v5, nextNeighborIndex, nextNeighbor);
				}
				else
				{
					TriangulateCorner(
						v5, nextNeighborIndex, nextNeighbor,
						e1.v5, cellIndex, cell,
						e2.v5, neighborIndex, neighbor);
				}
			}
			else if (neighbor.Elevation <= nextNeighbor.Elevation)
			{
				TriangulateCorner(
					e2.v5, neighborIndex, neighbor,
					v5, nextNeighborIndex, nextNeighbor,
					e1.v5, cellIndex, cell);
			}
			else {
				TriangulateCorner(
					v5, nextNeighborIndex, nextNeighbor,
					e1.v5, cellIndex, cell,
					e2.v5, neighborIndex, neighbor);
			}
		}
	}
```

调整 `TriangulateCorner` 的参数列表以匹配。将索引传递给 `TriangulateCornerTerraces` 而不是单元格，并且除了其他三角剖分方法调用的单元格数据外，还要这样做。

```c#
	void TriangulateCorner(
		Vector3 bottom, int bottomCellIndex, HexCellData bottomCell,
		Vector3 left, int leftCellIndex, HexCellData leftCell,
		Vector3 right, int rightCellIndex, HexCellData rightCell)
	{
		…

		if (leftEdgeType == HexEdgeType.Slope)
		{
			if (rightEdgeType == HexEdgeType.Slope)
			{
				TriangulateCornerTerraces(
					bottom, bottomCellIndex,
					left, leftCellIndex,
					right, rightCellIndex);
			}
			else if (rightEdgeType == HexEdgeType.Flat)
			{
				TriangulateCornerTerraces(
					left, leftCellIndex,
					right, rightCellIndex,
					bottom, bottomCellIndex);
			}
			else
			{
				TriangulateCornerTerracesCliff(
					bottom, bottomCellIndex, bottomCell,
					left, leftCellIndex, leftCell,
					right, rightCellIndex, rightCell);
			}
		}
		else if (rightEdgeType == HexEdgeType.Slope)
		{
			if (leftEdgeType == HexEdgeType.Flat)
			{
				TriangulateCornerTerraces(
					right, rightCellIndex,
					bottom, bottomCellIndex,
					left, leftCellIndex);
			}
			else
			{
				TriangulateCornerCliffTerraces(
					bottom, bottomCellIndex, bottomCell,
					left, leftCellIndex, leftCell,
					right, rightCellIndex, rightCell);
			}
		}
		else if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope)
		{
			if (leftCell.Elevation < rightCell.Elevation)
			{
				TriangulateCornerCliffTerraces(
					right, rightCellIndex, rightCell,
					bottom, bottomCellIndex, bottomCell,
					left, leftCellIndex, leftCell);
			}
			else
			{
				TriangulateCornerTerracesCliff(
					left, leftCellIndex, leftCell,
					right, rightCellIndex, rightCell,
					bottom, bottomCellIndex, bottomCell);
			}
		}
		else
		{
			terrain.AddTriangle(bottom, left, right);
			Vector3 indices;
			indices.x = bottomCellIndex;
			indices.y = leftCellIndex;
			indices.z = rightCellIndex;
			terrain.AddTriangleCellData(indices, weights1, weights2, weights3);
		}

		features.AddWall(
			bottom, bottomCell, left, leftCell, right, rightCell);
	}
```

`TriangulateEdgeTerraces` 现在可以直接使用单元格索引。

```c#
	void TriangulateEdgeTerraces(
		EdgeVertices begin, int beginCellIndex,
		EdgeVertices end, int endCellIndex,
		bool hasRoad)
	{
		…
		float i1 = beginCellIndex;
		float i2 = endCellIndex;

		…
	}
```

 `TriangulateCornerTerraces` 也是如此。

```c#
	void TriangulateCornerTerraces(
		Vector3 begin, int beginCellIndex,
		Vector3 left, int leftCellIndex,
		Vector3 right, int rightCellIndex)
	{
		…
		indices.x = beginCellIndex;
		indices.y = leftCellIndex;
		indices.z = rightCellIndex;

		…
	}
```

 `TriangulateCornerTerracesCliff` 也是如此，尽管它也需要单元格数据。

```c#
	void TriangulateCornerTerracesCliff(
		Vector3 begin, int beginCellIndex, HexCellData beginCell,
		Vector3 left, int leftCellIndex, HexCellData leftCell,
		Vector3 right, int rightCellIndex, HexCellData rightCell)
	{
		…
		indices.x = beginCellIndex;
		indices.y = leftCellIndex;
		indices.z = rightCellIndex;

		…
	}
```

 `TriangulateCornerCliffTerraces` 也是如此。

```c#
	void TriangulateCornerCliffTerraces(
		Vector3 begin, int beginCellIndex, HexCellData beginCell,
		Vector3 left, int leftCellIndex, HexCellData leftCell,
		Vector3 right, int rightCellIndex, HexCellData rightCell)
	{
		…
		indices.x = beginCellIndex;
		indices.y = leftCellIndex;
		indices.z = rightCellIndex;

		…
	}
```

这涵盖了所有三角剖分代码。为了从 `HexGridChunk` 中完全删除对 `HexCell` 的所有依赖，我们还需要调整 `AddCell`。将其 `HexCell` 参数替换为单元格 UI 的单元格索引和 `RectTransform` 引用。然后不再在此处设置单元格块。

```c#
	public void AddCell(int index, int cellIndex, RectTransform cellUI)
	{
		cellIndices[index] = cellIndex;
		//cell.Chunk = this;
		cellUI.SetParent(gridCanvas.transform, false);
	}
```

调整 `HexGrid.AddCellToChunk`，以便在此处设置单元格的块，并将正确的参数传递给 `AddCell`。

```c#
	void AddCellToChunk(int x, int z, HexCell cell)
	{
		…
		cell.Chunk = chunk;
		chunk.AddCell(
			localX + localZ * HexMetrics.chunkSizeX, cell.Index, cell.UIRect);
	}
```

### 2.3 六边形地图生成器

我们采用的最后一个类是 `HexMapGenerator`。我们首先重构 `HexGrid.GetCell(int xOffset, int zOffset)`，使其返回一个单元格索引。此方法仅由生成器使用。

```c#
	public int GetCellIndex(int xOffset, int zOffset) =>
		xOffset + zOffset * CellCountX;
```

然后我们可以重构 `HexMapGenerator.GetCell` 也一样。

```c#
	int GetRandomCellIndex (MapRegion region) => grid.GetCellIndex(
		Random.Range(region.xMin, region.xMax),
		Random.Range(region.zMin, region.zMax));
```

此外，因为从现在开始，我们直接设置单元格数据，而不是通过 `HexCell`，单元格及其可视化将不再自动刷新。为了解决这个问题，我们引入了一个公共的 `HexCell.RefreshAll` 方法执行完全刷新。

```c#
	public void RefreshAll()
	{
		RefreshPosition();
		Grid.ShaderData.RefreshTerrain(Index);
		Grid.ShaderData.RefreshVisibility(Index);
	}
```

更改 `HexMapGenerator.GenerateMap` 可以在设置水位时直接修改单元格数据，并在生成地图后对所有单元格调用 `RefreshAll`。这是唯一一个仍然依赖 `HexCell` 的地方。

```c#
	public void GenerateMap(int x, int z, bool wrapping)
	{
		…
		for (int i = 0; i < cellCount; i++)
		{
			grid.CellData[i].values = grid.CellData[i].values.WithWaterLevel(
				waterLevel);
		}
		…
		for (int i = 0; i < cellCount; i++)
		{
			grid.SearchData[i].searchPhase = 0;
			grid.GetCell(i).RefreshAll();
		}

		Random.state = originalRandomState;
	}
```

接下来是生成代码，我们从 `RaiseTerrain` 开始。更改它，使其与单元格索引一起工作，检索单元格数据，并直接调整单元格数据数组中的标高。

```c#
	int RaiseTerrain(int chunkSize, int budget, MapRegion region)
	{
		searchFrontierPhase += 1;
		int firstCellIndex = GetRandomCellIndex(region);
		grid.SearchData[firstCellIndex] = new HexCellSearchData
		{
			searchPhase = searchFrontierPhase
		};
		searchFrontier.Enqueue(firstCellIndex);
		HexCoordinates center = grid.CellData[firstCellIndex].coordinates;

		…
		while (size < chunkSize && searchFrontier.TryDequeue(out int index))
		{
			HexCellData current = grid.CellData[index];
			…
			grid.CellData[index].values =
				current.values.WithElevation(newElevation);
			…

			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
			{
				if (grid.TryGetCellIndex(
					current.coordinates.Step(d), out int neighborIndex) &&
					grid.SearchData[neighborIndex].searchPhase <
						searchFrontierPhase)
				{
					grid.SearchData[neighborIndex] = new HexCellSearchData
					{
						searchPhase = searchFrontierPhase,
						distance = grid.CellData[neighborIndex].coordinates.
							DistanceTo(center),
						heuristic = Random.value < jitterProbability ? 1 : 0
					};
					searchFrontier.Enqueue(neighborIndex);
				}
			}
		}
		…
	}
```

`SinkTerrain` 几乎完全相同，应该以相同的方式进行更改。

接下来是 `ErodeLand`，它需要进行一些更改，但它们与 `RaiseTerrain` 的更改相似。此外，我们从使用单元格列表切换到单元格索引列表，侵蚀目标也成为一个索引。现在，我们将单元格索引和单元格提升传递给 `GetErosionTarget` 和 `IsErodible`。此外，递增和递减海拔的代码必须变得更加冗长。

```c#
	void ErodeLand()
	{
		List<int> erodibleIndices = ListPool<int>.Get();
		for (int i = 0; i < cellCount; i++)
		{
			//HexCell cell = grid.GetCell(i);
			if (IsErodible(i, grid.CellData[i].Elevation))
			{
				erodibleIndices.Add(i);
			}
		}

		int targetErodibleCount =
			(int)(erodibleIndices.Count * (100 - erosionPercentage) * 0.01f);
		
		while (erodibleIndices.Count > targetErodibleCount)
		{
			int index = Random.Range(0, erodibleIndices.Count);
			int cellIndex = erodibleIndices[index];
			HexCellData cell = grid.CellData[cellIndex];
			int targetCellIndex = GetErosionTarget(cellIndex, cell.Elevation);
			
			//cell.Elevation -= 1;
			grid.CellData[cellIndex].values = cell.values =
				cell.values.WithElevation(cell.Elevation - 1);

			//targetCell.Elevation += 1;
			HexCellData targetCell = grid.CellData[targetCellIndex];
			grid.CellData[targetCellIndex].values = targetCell.values =
				targetCell.values.WithElevation(targetCell.Elevation + 1);

			if (!IsErodible(cellIndex, cell.Elevation))
			{
				int lastIndex = erodibleIndices.Count - 1;
				erodibleIndices[index] = erodibleIndices[lastIndex];
				erodibleIndices.RemoveAt(lastIndex);
			}

			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
			{
				if (grid.TryGetCellIndex(
					cell.coordinates.Step(d), out int neighborIndex) &&
					grid.CellData[neighborIndex].Elevation ==
						cell.Elevation + 2 &&
					!erodibleIndices.Contains(neighborIndex))
				{
					erodibleIndices.Add(neighborIndex);
				}
			}

			if (IsErodible(targetCellIndex, targetCell.Elevation) &&
				!erodibleIndices.Contains(targetCellIndex))
			{
				erodibleIndices.Add(targetCellIndex);
			}

			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
			{
				if (grid.TryGetCellIndex(
					targetCell.coordinates.Step(d), out int neighborIndex) &&
					neighborIndex != cellIndex &&
					grid.CellData[neighborIndex].Elevation ==
						targetCell.Elevation + 1 &&
					!IsErodible(
						neighborIndex, grid.CellData[neighborIndex].Elevation))
				{
					erodibleIndices.Remove(neighborIndex);
				}
			}
		}

		ListPool<int>.Add(erodibleIndices);
	}
```

将 `IsErodible` 的参数列表更改为匹配，然后使用单元格索引检索用于循环遍历其邻居的单元格坐标。

```c#
	bool IsErodible(int cellIndex, int cellElevation)
	{
		int erodibleElevation = cellElevation - 2;
		HexCoordinates coordinates = grid.CellData[cellIndex].coordinates;
		for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
		{
			if (grid.TryGetCellIndex(
				coordinates.Step(d), out int neighborIndex) &&
				grid.CellData[neighborIndex].Elevation <= erodibleElevation)
			{
				return true;
			}
		}
		return false;
	}
```

同样更改 `GetErosionTarget`，现在返回一个单元格索引。

```c#
	int GetErosionTarget (int cellIndex, int cellElevation)
	{
		List<int> candidates = ListPool<int>.Get();
		int erodibleElevation = cellElevation - 2;
		HexCoordinates coordinates = grid.CellData[cellIndex].coordinates;
		for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
		{
			if (grid.TryGetCellIndex(
				coordinates.Step(d), out int neighborIndex) &&
				grid.CellData[neighborIndex].Elevation <= erodibleElevation
			)
			{
				candidates.Add(neighborIndex);
			}
		}
		int target = candidates[Random.Range(0, candidates.Count)];
		ListPool<int>.Add(candidates);
		return target;
	}
```

接下来是 `EvolveClimate`，它也应该转向使用单元格数据和索引。

```c#
	void EvolveClimate(int cellIndex)
	{
		HexCellData cell = grid.CellData[cellIndex];
		…
		for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
		{
			if (!grid.TryGetCellIndex(
				cell.coordinates.Step(d), out int neighborIndex))
			{
				continue;
			}
			ClimateData neighborClimate = nextClimate[neighborIndex];
			…

			int elevationDelta = grid.CellData[neighborIndex].ViewElevation -
				cell.ViewElevation;
			…

			nextClimate[neighborIndex] = neighborClimate;
		}

		…
	}
```

更改 `CreateRivers` 以使用单元格索引。

```c#
	void CreateRivers()
	{
		List<int> riverOrigins = ListPool<int>.Get();
		for (int i = 0; i < cellCount; i++)
		{
			HexCellData cell = grid.CellData[i];
			…
			if (weight > 0.75f)
			{
				riverOrigins.Add(i);
				riverOrigins.Add(i);
			}
			if (weight > 0.5f)
			{
				riverOrigins.Add(i);
			}
			if (weight > 0.25f)
			{
				riverOrigins.Add(i);
			}
		}

		int riverBudget = Mathf.RoundToInt(landCells * riverPercentage * 0.01f);
		while (riverBudget > 0 && riverOrigins.Count > 0)
		{
			int index = Random.Range(0, riverOrigins.Count);
			int lastIndex = riverOrigins.Count - 1;
			int originIndex = riverOrigins[index];
			HexCellData origin = grid.CellData[originIndex];
			riverOrigins[index] = riverOrigins[lastIndex];
			riverOrigins.RemoveAt(lastIndex);

			if (!origin.HasRiver)
			{
				bool isValidOrigin = true;
				for (HexDirection d = HexDirection.NE;
					d <= HexDirection.NW; d++)
				{
					if (grid.TryGetCellIndex(
						origin.coordinates.Step(d), out int neighborIndex) &&
						(grid.CellData[neighborIndex].HasRiver ||
							grid.CellData[neighborIndex].IsUnderwater))
					{
						isValidOrigin = false;
						break;
					}
				}
				if (isValidOrigin)
				{
					riverBudget -= CreateRiver(originIndex);
				}
			}
		}

		…

		ListPool<int>.Add(riverOrigins);
	}
```

并调整 `CreateRiver` 以匹配。我们现在必须明确地正确设置两个单元格的标志，而不是在单元格上调用 `SetOutgoingRiver`。这里不需要 `SetOutgoingRiver` 完成的验证工作，因为我们只在没有障碍物的情况下创建河流连接。

```c#
	int CreateRiver(int originIndex)
	{
		int length = 1;
		int cellIndex = originIndex;
		HexCellData cell = grid.CellData[cellIndex];
		HexDirection direction = HexDirection.NE;
		while (!cell.IsUnderwater)
		{
			int minNeighborElevation = int.MaxValue;
			flowDirections.Clear();
			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
			{
				if (!grid.TryGetCellIndex(
					cell.coordinates.Step(d), out int neighborIndex))
				{
					continue;
				}
				HexCellData neighbor = grid.CellData[neighborIndex];

				…

				if (neighborIndex == originIndex || neighbor.HasIncomingRiver)
				{
					continue;
				}

				…

				if (neighbor.HasOutgoingRiver)
				{
					//cell.SetOutgoingRiver(d);
					grid.CellData[cellIndex].flags = cell.flags.WithRiverOut(d);
					grid.CellData[neighborIndex].flags =
						neighbor.flags.WithRiverIn(d.Opposite());
					return length;
				}

				…
			}

			if (flowDirections.Count == 0)
			{
				…

				if (minNeighborElevation >= cell.Elevation)
				{
					//cell.WaterLevel = minNeighborElevation;
					cell.values = cell.values.WithWaterLevel(
						minNeighborElevation);
					if (minNeighborElevation == cell.Elevation)
					{
						//cell.Elevation = minNeighborElevation - 1;
						cell.values = cell.values.WithElevation(
							minNeighborElevation - 1);
					}
					grid.CellData[cellIndex].values = cell.values;
				}
				break;
			}

			direction = flowDirections[Random.Range(0, flowDirections.Count)];
			//cell.SetOutgoingRiver(direction);
			cell.flags = cell.flags.WithRiverOut(direction);
			grid.TryGetCellIndex(
				cell.coordinates.Step(direction), out int outIndex);
			grid.CellData[outIndex].flags =
				grid.CellData[outIndex].flags.WithRiverIn(direction.Opposite());

			length += 1;

			if (minNeighborElevation >= cell.Elevation &&
				Random.value < extraLakeProbability)
			{
				//cell.WaterLevel = cell.Elevation;
				//cell.Elevation -= 1;
				cell.values = cell.values.WithWaterLevel(cell.Elevation);
				cell.values = cell.values.WithElevation(cell.Elevation - 1);
			}
			grid.CellData[cellIndex] = cell;
			cellIndex = outIndex;
			cell = grid.CellData[cellIndex];
		}
		return length;
	}
```

转换的最后生成步骤是 `SetTerrainType`。使其与单元格数据一起工作，并将单元格索引传递给 `DeterminedTemperature`。

```c#
	void SetTerrainType()
	{
		…
		
		for (int i = 0; i < cellCount; i++)
		{
			HexCellData cell = grid.CellData[i];
			float temperature = DetermineTemperature(i, cell);
			float moisture = climate[i].moisture;
			if (!cell.IsUnderwater)
			{
				…

				//cell.TerrainTypeIndex = cellBiome.terrain;
				//cell.PlantLevel = cellBiome.plant;
				grid.CellData[i].values = cell.values.
					WithTerrainTypeIndex(cellBiome.terrain).
					WithPlantLevel(cellBiome.plant);
			}
			else
			{
				int terrain;
				if (cell.Elevation == waterLevel - 1)
				{
					int cliffs = 0, slopes = 0;
					for (HexDirection d = HexDirection.NE;
						d <= HexDirection.NW; d++)
					{
						if (!grid.TryGetCellIndex(
							cell.coordinates.Step(d), out int neighborIndex))
						{
							continue;
						}
						int delta = grid.CellData[neighborIndex].Elevation -
							cell.WaterLevel;
						…
					}

					…
				}
				…
				//cell.TerrainTypeIndex = terrain;
				grid.CellData[i].values =
					cell.values.WithTerrainTypeIndex(terrain);
			}
		}
	}
```

`DetermineTemperature` 需要单元格索引来获得其位置。

```c#
	float DetermineTemperature(int cellIndex, HexCellData cell)
	{
		…

		float jitter = HexMetrics.SampleNoise(
			grid.CellPositions[cellIndex] * 0.1f)[temperatureJitterChannel];

		…
	}
```

### 2.4 六边形单元格清理

我们现在已经尽可能地从生成和可视化地图的代码中消除了对 `HexCell` 的依赖。生成地图后刷新单元格对象时，仍然可以访问单元格对象的唯一位置。

此时，`HexCell` 的多个成员不再被访问，所以让我们删除它们。它们是 `HasRiverBeginOrEnd`、`HasRoads`、`StreamBedY`、`RiverSurfaceY`、`WaterSurfaceY` 和 `HasIncomingRiverThroughEdge`。

下一个教程是 [Hex Map 3.4.0](https://catlikecoding.com/unity/hex-map/3-4-0/)。

[license](https://catlikecoding.com/unity/tutorials/license/)

[repository](https://bitbucket.org/catlikecoding-projects/hex-map-project/src/d0211871c63d80fea55e57748be158f5ba4b41af/?at=release%2F3.3.0)

[PDF](https://catlikecoding.com/unity/hex-map/3-3-0/Hex-Map-3-3-0.pdf)

# Hex Map 3.4.0：单元格结构

发布于 2024-04-09

https://catlikecoding.com/unity/hex-map/3-4-0/

![img](https://catlikecoding.com/unity/hex-map/3-4-0/tutorial-image.jpg)

*不含单元格物体。*

本教程使用 Unity 2022.3.38f1 编写，跟着 [Hex Map 3.3.0](https://catlikecoding.com/unity/hex-map/3-3-0/)。

## 1 移动责任

我们继续寻求摆脱单元格对象。这次我们将实现这一目标，尽管我们没有完全删除 `HexCell` 类型。

### 1.1 标志和值

为了减小 `HexCell` 的代码大小，我们将去掉所有现在只转发给标志和值的属性。其他代码应该直接访问这些代码。为了实现这一点，`HexCell` 的 `Flags` 和 `Values` 属性必须公开。

```c#
	public HexFlags Flags { … }
	
	public HexValues Values { … }
```

为了使这更方便，`HexFlags` 使用 `HasRiver` 方法来指示给定方向上是否存在河流。

```c#
	public static bool HasRiver(
		this HexFlags flags, HexDirection direction) =>
		flags.HasRiverIn(direction) || flags.HasRiverOut(direction);
```

还将 `ViewElevation` 和 `IsUnderwater` 属性添加到 `HexValues` 中。我们将简单地使用 `Mathf.Max` 以确定视图标高。

```c#
using UnityEngine;
…
	
	public readonly int ViewElevation => Mathf.Max(Elevation, WaterLevel);

	public readonly bool IsUnderwater => WaterLevel > Elevation;
```

### 1.2 将剩余数据移动到网格

我们将从单元格中删除几乎所有剩余的数据。就像我们之前对其他单元格数据所做的那样，我们将把它存储在 `HexGrid` 中的数组中。

将可公开访问的 `CellUnits` 数组添加到网格中。

```c#
	public HexUnit[] CellUnits
	{ get; private set; }
```

还为单元格引用提供数组，以引用其块和 UI rect。但我们将这些数据保密，因为我们将确保除了网格之外，其他任何东西都不需要访问这些数据。

```c#
	HexGridChunk[] cellGridChunks;

	RectTransform[] cellUIRects;
```

在 `CreateCells` 中创建这些数组以及其他数组。让我们也使用 `CellData` 的长度作为所有长度的参考，而不是 `cells` 数组长度，因为我们稍后会删除后者。

```c#
		CellData = new HexCellData[CellCountZ * CellCountX];
		CellPositions = new Vector3[CellData.Length];
		cellUIRects = new RectTransform[CellData.Length];
		cellGridChunks = new HexGridChunk[CellData.Length];
		CellUnits = new HexUnit[CellData.Length];
		searchData = new HexCellSearchData[CellData.Length];
		cellVisibility = new int[CellData.Length];
```

现在我们要修改 `CreateCell`，使其直接设置单元格的可探索标志和标高，并将 UI rect 和 chunk 引用存储在数组中，而不是单元格中。我们还将该方法与其正下方的 `AddCellToChunk` 合并。

```c#
	void CreateCell(int x, int z, int i)
	{
		…
		// if (Wrapping) { … } else { … }

		bool explorable = Wrapping ?
			z > 0 && z < CellCountZ - 1 :
			x > 0 && z > 0 && x < CellCountX - 1 && z < CellCountZ - 1;
		cell.Flags = explorable ?
			cell.Flags.With(HexFlags.Explorable) :
			cell.Flags.Without(HexFlags.Explorable);

		Text label = Instantiate(cellLabelPrefab);
		label.rectTransform.anchoredPosition =
			new Vector2(position.x, position.z);
		RectTransform rect = cellUIRects[i] = label.rectTransform;
		
		//cell.Elevation = 0;
		cell.Values = cell.Values.WithElevation(0);

		//AddCellToChunk(x, z, cell);
	//}

	//void AddCellToChunk(int x, int z, HexCell cell)
	//{
		int chunkX = x / HexMetrics.chunkSizeX;
		int chunkZ = z / HexMetrics.chunkSizeZ;
		HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];

		int localX = x - chunkX * HexMetrics.chunkSizeX;
		int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
		cellGridChunks[i] = chunk;
		chunk.AddCell(localX + localZ * HexMetrics.chunkSizeX, i, rect);
	}
```

### 1.3 刷新

设置单元格高度也应刷新单元格位置。我们将负责此操作的方法从单元格复制到网格，使其能够直接访问数组。因此，它只需要一个单元格索引参数。我们将其公开，以便其他人也可以触发刷新。

```c#
	public void RefreshCellPosition (int cellIndex)
	{
		Vector3 position = CellPositions[cellIndex];
		position.y = CellData[cellIndex].Elevation * HexMetrics.elevationStep;
		position.y +=
			(HexMetrics.SampleNoise(position).y * 2f - 1f) *
			HexMetrics.elevationPerturbStrength;
		CellPositions[cellIndex] = position;

		RectTransform rectTransform = cellUIRects[cellIndex];
		Vector3 uiPosition = rectTransform.localPosition;
		uiPosition.z = -position.y;
		rectTransform.localPosition = uiPosition;
	}
```

设置标高后，在 `CreateCell` 中调用它。

```c#
		cell.Values = cell.Values.WithElevation(0);
		RefreshCellPosition(i);
```

让我们还添加公共方法来触发整个单元格的刷新。对于一个包含其依赖项的单元格，包括其邻居和单位。

```c#
	public void RefreshCell(int cellIndex) =>
		cellGridChunks[cellIndex].Refresh();

	public void RefreshCellWithDependents (int cellIndex)
	{
		HexGridChunk chunk = cellGridChunks[cellIndex];
		chunk.Refresh();
		HexCoordinates coordinates = CellData[cellIndex].coordinates;
		for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
		{
			if (TryGetCellIndex(coordinates.Step(d), out int neighborIndex))
			{
				HexGridChunk neighborChunk = cellGridChunks[neighborIndex];
				if (chunk != neighborChunk)
				{
					neighborChunk.Refresh();
				}
			}
		}
		HexUnit unit = CellUnits[cellIndex];
		if (unit)
		{
			unit.ValidateLocation();
		}
	}
```

最后，还有一种新的 `RefreshAllCells` 方法，可以对所有单元格进行完全刷新。

```c#
	public void RefreshAllCells()
	{
		for (int i = 0; i < CellData.Length; i++)
		{
			SearchData[i].searchPhase = 0;
			RefreshCellPosition(i);
			ShaderData.RefreshTerrain(i);
			ShaderData.RefreshVisibility(i);
		}
	}
```

此方法适用于 `HexMapGenerator.GenerateMap`，它必须刷新所有单元格，现在只需调用新方法即可完成。

```c#
		//for (int i = 0; i < cellCount; i++)
		//{
			//grid.SearchData[i].searchPhase = 0;
			//grid.GetCell(i).RefreshAll();
		//}
		grid.RefreshAllCells();
```

### 1.4 保存和加载

我们还将保存和加载单元格的责任转移到网格中。在 `Save` 中，我们只需编写每个单元格的值和标志。我们现在根据 `CellData` 的长度而不是 `cells` 的长度进行循环。

```c#
		for (int i = 0; i < CellData.Length; i++)
		{
			//cells[i].Save(writer);
			HexCellData data = CellData[i];
			data.values.Save(writer);
			data.flags.Save(writer);
		}
```

以相同的方式调整 `Load`。在这里，我们还必须刷新位置、地形和能见度。

```c#
		for (int i = 0; i < CellData.Length; i++)
		{
			//cells[i].Load(reader, header);
			HexCellData data = CellData[i];
			data.values = HexValues.Load(reader, header);
			data.flags = data.flags.Load(reader, header);
			CellData[i] = data;
			RefreshCellPosition(i);
			ShaderData.RefreshTerrain(i);
			ShaderData.RefreshVisibility(i);
		}
```

### 1.5 标签和突出显示

单元格将忘记其标签和突出显示，因为它们是单独 UI 的一部分。将 `SetLabel`、`DisableHighlight` 和 `EnableHighlight` 方法复制到 `HexGrid`，并使其适应单元格索引参数和 UI rect 数组。这些方法可以保持私有，因为只有网格需要使用它们。

```c#
	void SetLabel(int cellIndex, string text) =>
		cellUIRects[cellIndex].GetComponent<Text>().text = text;

	void DisableHighlight(int cellIndex) =>
		cellUIRects[cellIndex].GetChild(0).GetComponent<Image>().enabled =
			false;

	void EnableHighlight(int cellIndex, Color color)
	{
		Image highlight =
			cellUIRects[cellIndex].GetChild(0).GetComponent<Image>();
		highlight.color = color;
		highlight.enabled = true;
	}
```

接下来，调整 `ClearPath` 和 `ShowPath`，使其使用网格本身的方法。这也意味着他们不再需要检索单元格，可以专门使用单元格索引，不再需要检索单元格。

```c#
	public void ClearPath()
	{
		if (currentPathExists)
		{
			//HexCell current = cells[currentPathToIndex];
			int currentIndex = currentPathToIndex;
			while (currentIndex != currentPathFromIndex)
			{
				current.SetLabel(null);
				current.DisableHighlight();
				current = cells[searchData[current.Index].pathFrom];
				SetLabel(currentIndex, null);
				DisableHighlight(currentIndex);
				currentIndex = searchData[currentIndex].pathFrom;
			}
			//current.DisableHighlight();
			DisableHighlight(currentIndex);
			currentPathExists = false;
		}
		else if (currentPathFromIndex >= 0)
		{
			//cells[currentPathFromIndex].DisableHighlight();
			//cells[currentPathToIndex].DisableHighlight();
			DisableHighlight(currentPathFromIndex);
			DisableHighlight(currentPathToIndex);
		}
		currentPathFromIndex = currentPathToIndex = -1;
	}

	void ShowPath(int speed)
	{
		if (currentPathExists)
		{
			//HexCell current = cells[currentPathToIndex];
			int currentIndex = currentPathToIndex;
			while (currentIndex != currentPathFromIndex)
			{
				int turn = (searchData[currentIndex].distance - 1) / speed;
				//current.SetLabel(turn.ToString());
				//current.EnableHighlight(Color.white);
				SetLabel(currentIndex, turn.ToString());
				EnableHighlight(currentIndex, Color.white);
				currentIndex = searchData[currentIndex].pathFrom;
			}
		}
		EnableHighlight(currentPathFromIndex, Color.blue);
		EnableHighlight(currentPathToIndex, Color.red);
	}
```

### 1.6 可见性

我们还将对可见性代码进行一些细微的更改。首先，`IncreaseVisibility` 将直接设置探索标志。

```c#
				//cells[i].MarkAsExplored();
				HexCell c = cells[i];
				c.Flags = c.Flags.With(HexFlags.Explored);
				cellShaderData.RefreshVisibility(cellIndex);
```

其次，`ResetVisibility` 将根据 `cellVisibility` 的长度循环。

```c#
		for (int i = 0; i < cellVisibility.Length; i++) { … }
```

第三，`GetVisibibleCells` 现在也将直接使用访问值和标志。

```c#
		range += fromCell.Values.ViewElevation;
		…
				if (currentData.searchPhase > searchFrontierPhase ||
					//!neighbor.Explorable
					neighbor.Flags.HasNone(HexFlags.Explorable))
				…
				if (distance + neighbor.Values.ViewElevation > range ||
					distance > fromCoordinates.DistanceTo(neighbor.Coordinates))
```

### 1.7 单位

接下来是 `HexUnit`，由于我们最近进行了更改，我们可以从 `Location` setter 中的单元格坐标中获取列索引，而不是从单元格本身获取。

```c#
			Grid.MakeChildOfColumn(transform, value.Coordinates.ColumnIndex);
```

`TravelPath` 也是如此。

```c#
		int currentColumn = currentTravelLocation.Coordinates.ColumnIndex;
		…
			int nextColumn = currentTravelLocation.Coordinates.ColumnIndex;
```

调整 `IsValidDestination`，使其访问单元格标志和值。

```c#
	public bool IsValidDestination(HexCell cell) =>
		cell.Flags.HasAll(HexFlags.Explored | HexFlags.Explorable) &&
		!cell.Values.IsUnderwater && !cell.Unit;
```

对 `GetMoveCost` 也这样做。我们还将直接访问 `HexMetrics` 以获取边缘类型，而不是向单元格请求。

```c#
		HexEdgeType edgeType = HexMetrics.GetEdgeType(
			fromCell.Values.Elevation, toCell.Values.Elevation);
		if (edgeType == HexEdgeType.Cliff)
		{
			return -1;
		}
		int moveCost;
		if (fromCell.Flags.HasRoad(direction))
		{
			moveCost = 1;
		}
		else if (fromCell.Flags.HasAny(HexFlags.Walled) !=
			toCell.Flags.HasAny(HexFlags.Walled))
		{
			return -1;
		}
		else
		{
			moveCost = edgeType == HexEdgeType.Flat ? 5 : 10;
			HexValues v = toCell.Values;
			moveCost += v.UrbanLevel + v.FarmLevel + v.PlantLevel;
		}
```

## 2 缩小单元格

我们已经将数据移出了单元格，并消除了其中的责任。现在是时候清理 `HexCell` 了，去掉不再需要的东西，并对其余部分进行一些重组。

### 2.1 单元格重构

首先，我们保留 `Unit` 属性，但将其转发到网格的数组，这样单元格就不再包含对其单位的直接引用。

```c#
	public HexUnit Unit
	{
		get => Grid.CellUnits[index];
		set => Grid.CellUnits[index] = value;
	}
```

保留 `Grid`、`Index`、`Position`、`Unit`、`Flags` 和 `Values` 属性，但删除所有其他不再需要的 getter。我们将所有 setter 重构为具有值参数的方法。第一个是 `SetElevation`，它取代了 `Elevation` setter。

```c#
	public void SetElevation (int elevation)
	{
		if (Values.Elevation != elevation)
		{
			Values = Values.WithElevation(elevation);
			Grid.ShaderData.ViewElevationChanged(index);
			Grid.RefreshCellPosition(index);
			ValidateRivers();
			HexFlags flags = Flags;
			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
			{
				if (flags.HasRoad(d))
				{
					HexCell neighbor = GetNeighbor(d);
					if (Mathf.Abs(elevation - neighbor.Values.Elevation) > 1)
					{
						RemoveRoad(d);
					}
				}
			}
			Grid.RefreshCellWithDependents(index);
		}
	}
```

然后是 `SetWaterLevel`。

```c#
	public void SetWaterLevel (int waterLevel)
	{
		if (Values.WaterLevel != waterLevel)
		{
			Values = Values.WithWaterLevel(waterLevel);
			Grid.ShaderData.ViewElevationChanged(index);
			ValidateRivers();
			Grid.RefreshCellWithDependents(index);
		}
	}
```

其次是 `SetUrbanLevel`。

```c#
	public void SetUrbanLevel (int urbanLevel)
	{
		if (Values.UrbanLevel != urbanLevel)
		{
			Values = Values.WithUrbanLevel(urbanLevel);
			Refresh();
		}
	}
```

以及看起来相似的 `SetFarmLevel` 和 `SetPlantLevel`。之后是 `SetSpecialIndex`。

```c#
	public void SetSpecialIndex (int specialIndex)
	{
		if (Values.SpecialIndex != specialIndex &&
			Flags.HasNone(HexFlags.River))
		{
			Values = Values.WithSpecialIndex(specialIndex);
			RemoveRoads();
			Refresh();
		}
	}
```

然后 `SetWalled`。

```c#
	public void SetWalled (bool walled)
	{
		HexFlags flags = Flags;
		HexFlags newFlags = walled ?
			flags.With(HexFlags.Walled) : flags.Without(HexFlags.Walled);
		if (flags != newFlags)
		{
			Flags = newFlags;
			Grid.RefreshCellWithDependents(index);
		}
	}
```

最后是 `SetTerrainTypeIndex`。

```c#
	public void SetTerrainTypeIndex (int terrainTypeIndex)
	{
		if (Values.TerrainTypeIndex != terrainTypeIndex)
		{
			Values = Values.WithTerrainTypeIndex(terrainTypeIndex);
			Grid.ShaderData.RefreshTerrain(index);
		}
	}
```

我们在 `HexCell` 中保留了这些设置功能，因为它们在更改单元格内容时会处理单元格间的依赖关系。只有地图编辑器可以做到这一点，但改变单元格的潜在游戏代码也可以依赖它。

`MarkAsExplored`、`GetEdgeType` 和 `HasRiverThroughEdge` 方法将不再使用，因此请将其删除。

接下来，调整 `RemoveIncomingRiver` 和 `RemoveOutgoingRiver` 以直接使用标志。此外，我们不再引用单元格的块，因为刷新将由网格完成。我们将调用单元格上的简单 `Refresh` 方法，稍后我们将对其进行更改。

```c#
	void RemoveIncomingRiver()
	{
		//if (!HasIncomingRiver) { … }
		if (Flags.HasAny(HexFlags.RiverIn))
		{
			HexCell neighbor = GetNeighbor(Flags.RiverInDirection());
			Flags = Flags.Without(HexFlags.RiverIn);
			neighbor.Flags = neighbor.Flags.Without(HexFlags.RiverOut);
			neighbor.Refresh();
			Refresh();
		}
	}
	
	void RemoveOutgoingRiver()
	{
		//if (!HasOutgoingRiver) { … }
		if (Flags.HasAny(HexFlags.RiverOut))
		{
			HexCell neighbor = GetNeighbor(Flags.RiverOutDirection());
			Flags = Flags.Without(HexFlags.RiverOut);
			neighbor.Flags = neighbor.Flags.Without(HexFlags.RiverIn);
			neighbor.Refresh();
			Refresh();
		}
	}
```

我们需要知道一条河是否可以在多个地方从一个单元格流向另一个单元格，所以让我们添加一个私有的静态 `CanRiverFlow` 方法来检查这一点，给定两个单元格的值。

```c#
	static bool CanRiverFlow (HexValues from, HexValues to) =>
		from.Elevation >= to.Elevation || from.WaterLevel == to.Elevation;
```

调整 `SetOutgoingRiver` 以使用该方法，并直接处理值。

```c#
	public void SetOutgoingRiver (HexDirection direction)
	{
		if (Flags.HasRiverOut(direction))
		{
			return;
		}

		HexCell neighbor = GetNeighbor(direction);
		if (!CanRiverFlow(Values, neighbor.Values))
		{
			return;
		}

		RemoveOutgoingRiver();
		if (Flags.HasRiverIn(direction))
		{
			RemoveIncomingRiver();
		}

		Flags = Flags.WithRiverOut(direction);
		//SpecialIndex = 0;
		Values = Values.WithSpecialIndex(0);
		neighbor.RemoveIncomingRiver();
		neighbor.Flags = neighbor.Flags.WithRiverIn(direction.Opposite());
		//neighbor.SpecialIndex = 0;
		neighbor.Values = neighbor.Values.WithSpecialIndex(0);

		RemoveRoad(direction);
	}
```

类似地调整 `AddRoad`、`RemoveRoads` 和 `RemoveRoad`。

```c#
	public void AddRoad(HexDirection direction)
	{
		HexFlags flags = Flags;
		HexCell neighbor = GetNeighbor(direction);
		if (
			!flags.HasRoad(direction) && !flags.HasRiver(direction) &&
			Values.SpecialIndex == 0 && neighbor.Values.SpecialIndex == 0 &&
			Mathf.Abs(Values.Elevation - neighbor.Values.Elevation) <= 1
		)
		{
			Flags = flags.WithRoad(direction);
			neighbor.Flags = neighbor.Flags.WithRoad(direction.Opposite());
			neighbor.Refresh();
			Refresh();
		}
	}
	
	public void RemoveRoads()
	{
		HexFlags flags = Flags;
		for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
		{
			if (flags.HasRoad(d))
			{
				RemoveRoad(d);
			}
		}
	}

	void RemoveRoad(HexDirection direction)
	{
		Flags = Flags.WithoutRoad(direction);
		HexCell neighbor = GetNeighbor(direction);
		neighbor.Flags = neighbor.Flags.WithoutRoad(direction.Opposite());
		neighbor.Refresh();
		Refresh();
	}
```

调整 `ValidateRivers` 以使用标志。

```c#
	void ValidateRivers()
	{
		HexFlags flags = Flags;
		if (flags.HasAny(HexFlags.RiverOut) &&
			!CanRiverFlow(Values, GetNeighbor(flags.RiverOutDirection()).Values)
		)
		{
			RemoveOutgoingRiver();
		}
		if (flags.HasAny(HexFlags.RiverIn) &&
			!CanRiverFlow(GetNeighbor(flags.RiverInDirection()).Values, Values))
		{
			RemoveIncomingRiver();
		}
	}
```

从现在开始，`Refresh` 将简单地转发到网格的 `RefreshCell` 方法。

```c#
	void Refresh() => grid.RefreshCell(Index);
```

最后，删除 `RefreshPosition`、`RefreshAll`、`Save`、`Load`、`SetLabel`、`DisableHighlight` 和 `EnableHighlight`，因为这些功能现在是网格的一部分。

### 2.2 地图编辑器

我们必须对 `HexMapEditor` 进行一些更改。首先，我们现在必须调用单元格的 `SetTerrainTypeIndex` 方法，将活动地形类型索引传递给它，而不是在 `EditCell` 中设置单元格的 `TerrainTypeIndex` 属性。

```c#
				//cell.TerrainTypeIndex = activeTerrainTypeIndex;
				cell.SetTerrainTypeIndex(activeTerrainTypeIndex);
```

我们必须对所有其他编辑案例进行类似的更改。

除此之外，在 `UpdateCellHighlightData` 中，我们仍然会将单元格与 `null` 进行比较，以查看它是否是无效的编辑目标。我们应该更改此设置，以检查单元格的计算结果是否为 `false`，就像我们在其他地方所做的那样。

```c#
		//if (cell == null)
		if (!cell)
```

### 2.3 转换为结构

我们现在终于要删除 `HexCell` 对象了。但是，我们保留它的类型，将其更改为结构类型。

```c#
public struct HexCell
```

这将使仍然依赖 `HexCell` 的当前代码保持功能，为存储在网格中的数据提供一个方便的外观。我们仍然需要存储在单元格中的唯一数据是它的索引和对网格的引用。我们为这些字段引入了字段，以明确存储的内容。

```c#
#pragma warning disable IDE0044 // Add readonly modifier
	int index;

	HexGrid grid;
#pragma warning restore IDE0044 // Add readonly modifier
```

> **为什么不让网格在全局范围内可访问？**
>
> 保留参考可以在未来支持多个网格。例如，对于多个断开连接的区域或世界图层。

给它一个构造函数方法来初始化两者。

```c#
	public HexCell(int index, HexGrid grid)
	{
		this.index = index;
		this.grid = grid;
	}
```

所有引用 `Index` 属性的单元格代码现在都可以直接访问 `index` 字段。不过，我们确实保留了公共 getter，所以其他代码仍然可以访问该索引。它现在成为只读属性。

```c#
	public readonly int Index => index;
```

单元外的代码不需要访问其网格，因此删除 `Grid` 属性。所有单元格代码都应该直接访问 `grid` 字段。

现在单元格不再是对象，隐式转换为布尔值必须改变。我们不检查单元格是否为 `null`，而是检查其网格。其想法是，无效单元格与默认结构值匹配，默认结构值没有网格。

```c#
	public static implicit operator bool(HexCell cell) => cell.grid != null;
```

最后，将除这两个字段之外的所有内容标记为只读。由于 Unity 的热重载限制，字段本身不能是只读的。出于同样的原因，`HexValues` 中的 `values` 字段也不是只读的。

### 2.4 单元格相等性

某些代码依赖于检查单元格是否相等。为了使该代码与单元格结构一起工作，我们必须包含自定义 == 和 != 操作符。如果单元格的索引和网格都相同，则单元格是相等的。

```c#
	public static bool operator ==(HexCell a, HexCell b) =>
		a.index == b.index && a.grid == b.grid;
	
	public static bool operator !=(HexCell a, HexCell b) =>
		a.index != b.index || a.grid != b.grid;
```

如果我们重载这些运算符，编译器会坚持要求我们也重写具有 `object` 参数的 `Equals` 方法，以防用带框的值进行相等性检查。

```c#
	public readonly override bool Equals(object obj) =>
		obj is HexCell cell && this == cell;
```

> **拆箱代码是如何工作的？**
>
> 是 ` obj is HexCell ? this == (HexCell)obj : false` 的简写

我们还被要求重写 `GetHashCode` 方法。我们只需将索引和网格的哈希值进行二进制 XOR 运算。

```c#
	public readonly override int GetHashCode() =>
		grid != null ? index.GetHashCode() ^ grid.GetHashCode() : 0;
```

### 2.5 网格

既然单元格不再是对象，网格就不需要再跟踪它们了，所以删除它的 `cells` 数组。

```c#
	//HexCell[] cells;
	
	…
	
	void CreateCells()
	{
		//cells = new HexCell[CellCountZ * CellCountX];
		…
	}
```

当在具有 `Ray` 参数的 `GetCell` 方法中找不到单元格时，我们不能再返回 `null`。因此，我们返回默认的结构值，该值隐式计算为 `false`，因为它没有网格。

```c#
	public HexCell GetCell(Ray ray)
	{
		…
		return default;
	}
```

具有 `HexCoordinates` 参数的 `GetCell` 方法也是如此。此外，我们不会从数组中返回一个单元格，而是返回一个新的结构值，其中包含单元格索引和对网格的引用。

```c#
	public HexCell GetCell(HexCoordinates coordinates)
	{
		…
		if (z < 0 || z >= CellCountZ || x < 0 || x >= CellCountX)
		{
			return default;
		}
		return new HexCell(x + z * CellCountX, this);
	}
```

以相同的方式使用 `HexCoordinates` 调整 `TryGetCell`。

```c#
	public bool TryGetCell(HexCoordinates coordinates, out HexCell cell)
	{
		…
		if (z < 0 || z >= CellCountZ || x < 0 || x >= CellCountX)
		{
			cell = default;
			return false;
		}
		cell = new HexCell(x + z * CellCountX, this);
		return true;
	}
```

`GetCell` 也有一个单元格索引。

```c#
	public HexCell GetCell(int cellIndex) => new(cellIndex, this);
```

`CreateCell` 现在还必须使用新的结构值。

```c#
		//var cell = cells[i] = new HexCell();
		var cell = new HexCell(i, this);
```

`Search` 和 `GetVisibleCells` 也是如此。

```c#
			//HexCell current = cells[currentIndex];
			var current = new HexCell(currentIndex, this);
```

`HexCell` 对象现在终于消失了。我们只剩下一个小的 `HexCell` 结构，它在一些地方使用。

[license](https://catlikecoding.com/unity/tutorials/license/)

[repository](https://bitbucket.org/catlikecoding-projects/hex-map-project/src/2399393cdf64ad7d83eaff456f1207aa214356e2/?at=release%2F3.4.0)

[PDF](https://catlikecoding.com/unity/hex-map/3-4-0/Hex-Map-3-4-0.pdf)
