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
