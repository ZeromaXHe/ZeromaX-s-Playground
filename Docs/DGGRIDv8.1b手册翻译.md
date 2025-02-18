# DGGRID 版本 8.1b

离散全球网格生成软件用户文档

Kevin Sahr

- 原 pdf 下载地址：https://discreteglobal.wpengine.com/wp-content/uploads/2024/01/dggridManualV81b.pdf
- 文档地址：https://discreteglobal.wpengine.com/publications/
- GitHub 仓库地址：https://github.com/sahrk/DGGRID

**Copyright © 2024 Kevin Sahr**



# 使用条款

此文档是 **DGGRID** 的一部分。

**DGGRID** 是免费软件：您可以根据免费软件基金会发布的 GNU Affero 通用公共许可证的条款重新发布和/或修改它，或许可证的版本3，或（您可以选择）任何以后的版本。

**DGGRID** 的分发是希望它是有用的，但没有任何保证；甚至没有对适销性或适合特定用途的默示保证。有关更多细节，请参阅 GNU Affero 通用公共许可证。

您应该收到一份 GNU Affero 通用公共许可证以及 DGGRID 源代码。如果没有，请参见<https://www.gnu.org/licenses/>。



# 致谢

**DGGRID** 主要是由 Kevin Sahr 用 C++ 写的。有关其他贡献者，请参阅源代码附带的文件 **CHANGELOG.md**。

最初的 **DGGRID** 规格是由（按字母顺序）开发的：罗斯·基斯特、托尼·奥尔森、芭芭拉·罗森鲍姆、凯文·萨尔、安·惠兰和丹尼斯·怀特。

**DGGRID** 的部分资金来自**美国环境保护署**、**行星风险公司**、**卡尔曼国际公司**、**鲁尔大学波鸿/地理观测项目**、**海龟保护协会**。以及**塔尔图大学景观地理信息学实验室**。

**DGGRID** 可以选择使用以下外部库（不包括在内）：

- 开源地理空间基金会的栅格和矢量地理空间数据格式的 GDAL 翻译库（见 gdal.org）

**DGGRID** 使用以下第三方库（包括在 **DGGRID** 源代码中）：

- 安格斯·约翰逊的克利伯库；参见 http://www.angusj.com。
- 乔治·马萨格利亚的多重携带“rng之母”伪随机数生成函数。
- gnomonic 投影代码改编自杰拉尔德·埃文登的 PROJ.4 库。
- Frank Warmerdam 的 Shapelib 库

github 源代码分发包含了关于构建 **DGGRID** 的指令。示例目录包含示例 **DGGRID** 元文件（包含关联的输入文件）。

**DGGRID** 版本 8.1b 于 2024 年 1 月 10 日发布

www.discreteglobalgrids.org



# 目录

1. 简介
2. Metafile 格式
3. 通用参数
4. 指定 DGG
5. 指定按单元格的网格输出
6. 网格生成：整个地球或剪切多面体
7. 指定点文件输入
8. 网格生成：点装箱
9. 装箱点值
10. 展示/缺漏装箱
11. 进行地址转换
12. 输出网格统计

- 附录 A：DGGRID Metafile 参数
- 附录 B：预设 DGG 类型的默认值
- 附录 C：DGG 地址表
- 附录 D：一些预设 ISEA DGG 的统计
  - 孔径 3：ISEA3H
  - 孔径 4：ISEA4H
  - PlanetRisk 网格
  - 混合孔径 4 和 3：ISEA43H
  - 注意
- 附录 E：EPA Superfund_500m DGG
- 附录 F：PlanetRisk DGG
- 附录 G：引用

# 1、简介

**DGGRID** 是一个命令行应用程序，设计用于生成和操作二十面体离散全局网格（DGGs）[Sahr 等人，2003]。**DGGRID** 的一次执行可以执行六个不同的操作中的一个：

1. *通过剪切多边形生成网格*。生成 DGG 的单元格，要么覆盖整个地球表面，要么只覆盖地球表面的一组特定区域。
2. *从点生成网格*。生成 DGG 的单元格，要么覆盖整个地球表面，要么只覆盖地球表面的一组特定区域。
3. *点值装箱功能*。通过将该单元格中包含的值的算术平均值分配给每个 DGG 单元格，将一组与点位置相关的浮点值放到 DGG 的单元格中。
4. *展示/缺漏装箱*。给定一组输入文件，每个文件包含与特定类相关联的点位置，**DGGRID** 输出，对于 DGG 的每个单元格，一个向量，指示每个类是否在该单元格中存在。
5. *地址转换*。将位置文件从一个地址表单（如经度/纬度）转换为另一个地址表（如 DGG 单元格索引）。
6. *输出网格统计信息*。输出指定 DGG 的网格特性表。

**DGGRID** 被设计为从 Unix 命令行中运行。**DGGRID** 需要一个命令行参数，即“元文件”的名称，这是一个纯文本文件，描述了 **DGGRID** 在该运行中要执行的操作。因此，通过在命令行键入来执行 **DGGRID**：

```shell
dggrid metaFileName.meta
```

该元文件由一系列告诉 **DGGRID** 如何继续操作的键-值对组成。下一节将描述此元文件的格式。本文档中的其余部分提供了关于设置元文件参数以控制 **DGGRID** 的执行的更多详细信息。

# 2、Metafile 格式

元文件是一种文本文件，其中每行要么是注释、空行，要么是指定 **DGGRID** 参数值的键-值对。**DGGRID** 会忽略空白行。以“#”开头的行是注释，并且也会被 **DGGRID** 忽略。

参数由形式的单行指定：

*参数名 值*

参数名不区分大小写。一个参数可以是五种类型之一。这五种参数类型的合法值描述如下：

1. **boolean**。合法值是 **TRUE** 和 **FALSE**（区分大小写）。 
2. **integer**。任何整数都是合法值。
3. **double**。任何以十进制形式指定的实数都是一个合法值。 
4. **string**。参数名称后面的行的其余部分被解释为值。
5. **choice**。合法值由特定于该参数的有限关键字集之一组成。选择参数的值不区分大小写，但按照惯例，通常用所有大写字母书写。

某些参数仅用于特定操作或特定其他参数条件。所有参数都有一个默认值，如果没有指定任何值，则会使用该默认值。关于每个参数的详细信息将在以下章节和**附录 A** 中给出。在同一元文件中重复一个参数规范会覆盖先前指定的值；将使用特定参数的最后一个值。

请注意，**DGGRID** 以前版本的一些参数在代码中仍然很活跃，但在本文档中没有描述；这是因为这些参数还没有与这个测试版中的新功能完全集成。这些参数将在未来的版本中完全恢复。

有关 **DGGRID** 元文件的示例，请参见 **DGGRID** 源代码分发版中的 `examples` 目录。

# 3、通用参数

在本节中，我们将描述 **DGGRID** 每次运行时所使用的参数。

如**第 1 节**所述，**DGGRID** 的每次运行都由五种不同的操作模式之一组成。使用 **choice** 参数 `dggrid_operation` 指定该操作。此参数的允许值为：

- `GENERATE_GRID` - 从多边形执行网格生成（参见第 6 节）
- `GENERATE_GRID_FROM_POINTS` - 从点执行网格生成（参见第 8 节）
- `BIN_POINT_VALS` - 执行点值装箱（参见第 9 节）
- `BIN_POINT_PRESENCE` - 执行存在/缺席装箱（参见第 10 节）
- `TRANSFORM_POINTS` - 执行地址转换（参见第 11 节）
- `OUTPUT_STATS` - 输出网格特性表（参见第 12 节）

所有的操作模式都要求有一个 DGG 的规范。在**第 4 节**中描述了用于指定一个 DGG 的参数。

**integer** 参数 `precision` 指定在输出浮点数时使用的小数位 **DGGRID** 右侧的数字数，包括纬度/经度数值。

参数 `verbosity`、`pause_on_startup` 和 `pause_before_exit` 的存在仅是为了支持调试，用户可以忽略。这些参数如下所述。

**integer** 参数的 `verbosity` 用于控制由 **DGGRID** 打印的调试信息的量。有效的值是从 0 到 3。默认值 0 给出最小输出，其中包括所有活动参数设置的值。不建议您增加此值。

将 **boolean** 参数 `pause_on_startup` 和/或 `pause_before_exit` 设置为 `TRUE` 会导致 **DGGRID** 在加载参数后立即或在程序终止之前暂停执行。这两个参数的默认值均为 `FALSE`。

# 4、指定 DGG

## 背景

如[Sahr 等人，2003 年]中所述，DGG 系统可以通过一组独立的设计选择来指定。第一个设计选择是所需的基多面体；**DGGRID** 可以生成有一个二十面体作为其基多面体的 DGGs。其余的主要设计选择是：

1. 基底多面体相对于地球的方向。 
2. 在基多面体的面（或面组）上对称定义的分层空间划分方法。这通常包括指定单元拓扑结构和孔径，它在顺序分辨率下决定单元之间的面积比。
3. 每个面与相应的球面之间的变换。
4. 分辨率（或递归分区的程度）。

**DGGRID** 的当前版本支持使用二十面体斯奈德等面积（ISEA）投影[斯奈德，1992]或巴克敏斯特·富勒[1975]的二十面体投影（由罗伯特·格雷[1995]和约翰·克里德[2008]分析开发）的 DGGs。**DGGRID** 可以生成具有三角形、菱形或六边形的单元格的网格。带有三角形或菱形拓扑的网格必须使用孔径为 4，而六边形网格可以使用孔径为 3、4、7，或这些孔径的任意混合序列。**DGGRID** 还支持专门设计的“预设” DGGs，包括美国环保局 **Superfund_500m** DGG 的混合孔径六角形网格（见**附录 E**）和 PlanetRisk DGG（见**附录 F**）。

下面给出了关于指定每个 DGG 设计选择的参数的详细信息，以及关于指定球形地球半径的讨论。

## 预设 DGG 类型

**DGGRID** 为您提供了许多预设的 DGG 类型。可以通过为 **choice** 参数 `dggs_type` 指定以下值之一来选择预设类型：

- `CUSTOM`（默认）- 表示网格参数将手动指定（见下文）
- `SUPERFUND` - **Superfund_500m** 网格（见**附录 E**）
- `PLANETRISK` - **PlanetRisk** 网格（见**附录 F**）
- `ISEA4T` - 三角形单元格的 ISEA 投影，孔径 4
- `ISEA4D` - 菱形单元格的 ISEA 投影，孔径 4
- `ISEA3H` - 六边形单元格的 ISEA 投影，孔径 3
- `ISEA4H` - 六边形单元格的 ISEA 投影，孔径 4
- `ISEA7H` - 六边形单元格的 ISEA 投影，孔径 7
- `ISEA43H` - 六边形单元格的 ISEA 投影，和孔径 4 分辨率然后孔径 3 分辨率的混合序列
- `FULLER4T` - 三角形单元格的 FULLER 投影，孔径为 4
- `FULLER4D` - 菱形单元格的 FULLER 投影，孔径为 4
- `FULLER3H` - 六边形单元格的 FULLER 投影，孔径为 3
- `FULLER4H` - 六边形单元格的 FULLER 投影，孔径为 4
- `FULLER7H` - 六边形单元格的 FULLER 投影，孔径为 7
- `FULLER43H` - 六边形单元格的 FULLER 投影，和孔径 4 分辨率然后孔径 3 分辨率的混合序列

每个预设的网格类型都为指定 DGG 的所有参数设置适当的值。每个预设网格类型的默认值在**附录 B** 中给出。这些默认预设值可以通过显式设置所需的各个参数来覆盖这些预设值，如下所述。特别是，请注意，所有预设网格类型都具有默认分辨率；应使用参数 `dggs_res_spec` 指定您所需的 DGG 分辨率（请参见下文）。

**附录 D** 给出了六角形 ISEA 预设 DGG 的个别分辨率的统计。

## 手动设置 DGG 参数

以下参数用于描述特定的 DGG 实例。

**1、指定方向**：通过将选择参数 `dggs_orient_specify_type` 分别设置为 `SPECIFIED`、`RANDOM` 或 `REGION_CENTER`，可以明确指定、随机确定或设置，使指定的点与二十面体顶点有最大距离。

如果 `dggs_orient_specify_type` 设置为 `SPECIFIED`，则 DGG 方向由单个二十面体顶点的位置和从该顶点到相邻顶点的方位角决定。**double** 参数 `dggs_vert0_lon` 和 `dggs_vert0_lat` 用于指定顶点的位置，**double** 参数 `dggs_vert0_azimuth` 用于指定相邻顶点的方位角；所有或这些参数都是十进制度的。请注意，默认的 DGG 位置，它围绕赤道对称，只有一个二十面体顶点落在陆地上，被指定为：

```
dggs_vert0_lon 11.25 
dggs_vert0_lat 58.28252559 
dggs_vert0_azimuth 0.0
```

如果 `dggs_orient_specify_type` 被设置为 `RANDOM`，则 DGG 的方向是随机确定的。所有参数值（包括用于定位网格的随机生成的值和方位角）将为您的信息输出到字符串参数 `dggs_orient_output_file_name` 指定的文件中。通过 **choice** 参数 `rng_type` 和 **integer** 参数 `dggs_orient_rand_seed`，对网格方向的随机规范进行了一些控制。**choice** 参数 `rng_type` 表示要使用哪个伪随机数生成器。`RAND` 值表示 C 标准库 `rand/srand` 函数应该被使用。`MOTHER` 值（默认的）表明乔治·马萨格利亚的“Mother-of-all-RNGs”函数应该被使用。**DGGRID** 用于初始化伪随机数序列的种子值可以使用 **integer** 参数 `dggs_orient_rand_seed` 来设置。

如果当前的操作只涉及地球表面的一个小区域，那么通常便于定位网格，使感兴趣的区域不会出现二十面体顶点。可以通过将 `dggs_orient_specify_type` 设置为 `REGION_CENTER`，然后使用 **double** 参数 `region_center_lon` 和 `region_center_lat`（均以十进制度表示）指定该区域的中心点来指定这样的方向。

所有操作都要求至少指定一个 DGG。通过将 **integer** 参数 `dggs_num_placements` 设置为 1（默认值），可以使用单个 DGG。或者，您可以通过将 `dggs_num_placements` 设置为所需的数字，对多个 DGG 执行所需的操作。如果网格方向是随机选择的，这将对多个随机方向的网格执行所需的操作。每个网格的参数将根据 `dggs_orient_output_file_name` 的值输出到一个单独的文件中，并有一个额外的后缀表示网格号（0001 到 000n，其中 *n* 等于 `dggs_num_placements` 的值）。此后缀还将用于指定相应的输出文件（在正在执行的特定操作中指定）。注意，如果 `dggs_orient_specify_type` 设置为 `RANDOM` 之外的任何值，所有生成的网格将具有完全相同的方向。



**2、指定分层空间划分方法**：用于生成 DGG 的分层划分方法是通过选择网格拓扑和孔径（定义为给定 DGG 分辨率和下一个更精细的分辨率下单元之间的面积比例）来指定的。使用 **choice** 参数 `dggs_topology` 指定拓扑，其值之一为：`HEXAGON`（默认）、`TRIANGLE` 或 `DIAMOND`。

**DGGRID** 可以创建使用单孔径产生的网格，以及使用混合孔径 4 分辨率的混合孔径 3 分辨率产生的六角形网格，或者使用任意混合孔径分辨率序列的六角形网格。孔径序列的类型使用 **choice** 参数 `dggs_aperture_type` 来指定，其值分别为 `PURE`（默认）、`MIXED43` 或 `SEQUENCE`。

如果指定了一个 PURE 孔径类型，则用 **integer** 参数 `dggs_aperture` 指定所需的孔径。孔径的有效值取决于所选的拓扑结构。**DGGRID** 可以创建孔径为 3、4 或 7 的 `HEXAGON` DGG，以及孔径为 4 的 `DIAMOND` 和 `TRIANGLE` DGG。

如果指定了 `MIXED43` 孔径类型，则将忽略参数 `dggs_aperture`。相反，**integer** 参数 `dggs_num_aperture_4_res`（默认为 0）指定使用孔径 4 的分辨率数；剩余的网格分辨率（见下一节的网格分辨率）将使用孔径 3 生成。请注意，参数 `dggs_num_aperture_4_res` 将被忽略，除非 `dggs_aperture_type` 是 `MIXED43`。只有 `HEXAGON` 拓扑网格可以使用 `MIXED43` 孔径类型。

如果指定了 `SEQUENCE` 孔径类型，则将忽略参数 `dggs_aperture`。相反，DGGS 的孔径序列必须指定为由 3、4、7 字符串组成的 **string** 参数 `dggs_aperture_sequence`（默认“333333333333”）。 



**3、指定投影**：与 DGG 单元相关的规则多边形边界和点最初是在二十面体的平面上创建的；然后它们必须反向投影到球体上。用于此操作的期望投影由 **choice** 参数 `dggs_proj` 指定。有效值是 `ISEA`，它指定了二十面体斯奈德等面积投影[斯奈德，1992]，或 `FULLER`，它指定了 R.巴克明斯特·富勒[1975]的二十面体 Dymaxion 投影（由罗伯特·格雷[1995]和约翰·克里德[2008]分析开发）。ISEA 投影以相对较高的形状扭曲为代价，在球体上创建相等面积的细胞，而富勒投影在面积和形状扭曲之间取得平衡。参见格雷戈里等人[2008]，了解关于这些权衡的更详细的讨论。



**4、指定分辨率**：可以使用 **choice** 参数 `dggs_res_specify_type` 选择的三种方法之一来指定所需的 DGG 分辨率：

- `SPECIFIED`（默认值） - 通过设置整数参数 `dggs_res_spec`（默认值 9）显式指定所需的分辨率。
- `CELL_AREA` - 所需的分辨率设置为 DGG 分辨率，其平均单元格面积最接近由 **double** 参数 `dggs_res_specify_area`（以平方公里为单位）指定的面积。
- `INTERCELL_DISTANCE` - 所需的分辨率被设置为 DGG 分辨率，其近似的单元间距离最接近由 **double** 参数 `dggs_res_specify_intercell_distance` 指定的距离（以公里为单位）。请注意，单元间距离计算是在平面上执行的，因此仅作为相对度量有用。一些六边形 ISEA 预设网格类型的经验性单元间统计数据见**附录 D**。

如果指定了 `CELL_AREA` 或 `INTERCELL_DISTANCE`，则根据 **boolean** 参数 `dggs_res_specify_rnd_down` 的值，将期望的区域或单元间距离（如适用）四舍五入或向下最近的网格分辨率；`TRUE` 表示四舍五入，`FALSE` 表示四舍五入。所选的分辨率总是由 **DGGRID** 输出，以供您参考。单元面积和单元间距离的计算使用为接地半径指定的值（见下文**第 5 小节**）。

一般来说，**DGGRID** 将尝试生成最大分辨率为 **35** 的网格。对于 `dggs_aperture_type` 为 `SEQUENCE` 的网格，将尝试的最大分辨率 **DGGRID** 由 **string** 参数值 `dggs_aperture_sequence` 值的长度决定。如果将 `dggs_type` 指定为 `SUPERFUND`，则 `SPECIFIED` 是 `dggs_res_specify_type` 的唯一支持值，最大分辨率为 **9**。

然而，实际上可以成功生成的最大分辨率 **DGGRID** 是指定网格拓扑的函数，投影，数据类型的大小在机器上 **DGGRID** 编译和执行，和生成的网格区域的位置相对于底层的二十面体的面。当生成非常高分辨率的网格时，用户应该意识到，即使 **DGGRID** 报告了成功，也应该验证索引和输出单元的几何图形，以确保它们不会退化。 



**5、指定地球半径**：**choice** 参数 `proj_datum` 指定 **DGGRID** 将用于确定地球球面半径的基准。该参数的法定数值如下所示，以及它们所表示的地球半径：

- `WGS84_AUTHALIC_SPHERE`（默认）：6371.007180918475 公里
- `WGS84_MEAN_SPHERE`： 6371.0087714 公里
- `CUSTOM_SPHERE`：地球半径（公里）将从 **double** 参数 `proj_datum_radius` 中读取

注意，在大地坐标下生成网格几何的过程中*没有*使用地球半径；这样的生成是在单位球体上进行的。半径仅用于确定网格分辨率（当 `dggs_res_specify_type` 不是 `SPECIFIED` 时）和以公里为单位生成网格统计信息。



# 5、指定每个单元格的网格输出