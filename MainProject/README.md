# 灵感

1. 自发光 Shader 可以参考[全息图着色器](https://www.bilibili.com/video/BV18e41197Yz)，用来做地球暗面的城市灯光？
1. 颜色选择时用 RAW 可以选超过 (1, 1, 1) 的值！



# 致谢

排名不分先后

1. Jasper Flick 的 [Catlike Coding 教程](https://catlikecoding.com/)（尤其是六边形地图教程，以及其他相关游戏开发教程。Copyright 2022 Jasper Flick，基于 [MIT0 许可](https://catlikecoding.com/unity/tutorials/license/)）
2. [Sebastian Lague](https://github.com/SebLague) 编程冒险等系列视频和开源项目
   1. **Solar-System**：Copyright (c) 2020 Sebastian Lague, [MIT 许可](https://github.com/SebLague/Solar-System/blob/Development/LICENSE)
   2. **Geographical-Adventures**：Copyright (c) 2024 Sebastian Lague，[MIT 许可](https://github.com/SebLague/Geographical-Adventures/blob/main/LICENSE)
   3. **Terraforming**：Copyright (c) 2021 Sebastian Lague，[MIT 许可](https://github.com/SebLague/Terraforming/blob/main/LICENSE)
3. athillion 的 [ProceduralPlanetGodot](https://github.com/athillion/ProceduralPlanetGodot) 开源项目（Copyright (c) 2022 Athillion Studios，[MIT 许可](https://github.com/athillion/ProceduralPlanetGodot/blob/main/LICENSE)）
4. Zylann 的 [godot_atmosphere_shader](https://github.com/Zylann/godot_atmosphere_shader) 开源项目与插件（Copyright (c) 2021 Marc Gilleron，[自定义许可证](https://github.com/Zylann/godot_atmosphere_shader/blob/master/LICENSE.md)），以及 [solar_system_demo](https://github.com/Zylann/solar_system_demo) 开源项目（Copyright (c) Marc Gilleron，[自定义许可证](https://github.com/Zylann/solar_system_demo/blob/master/LICENSE.md)）
   1. 其中，solar_system_demo 中包含了翻译到 Godot 4 的 [SIsilicon 的 Godot-Lens-Flare-Plugin 插件](https://github.com/SIsilicon/Godot-Lens-Flare-Plugin)，（Copyright (c) 2020 SIsilicon，[MIT 许可](https://github.com/SIsilicon/Godot-Lens-Flare-Plugin/blob/master/LICENSE)）
5. Visible earth 网站，基于其 [Image Use Policy](https://visibleearth.nasa.gov/image-use-policy) 使用
   1. [海洋测深高度图 gebco_08_rev_bath_21600x10800.png](https://visibleearth.nasa.gov/images/73963/bathymetry/73965l)（*Imagery by Jesse Allen, NASA's Earth Observatory, using data from the General Bathymetric Chart of the Oceans (GEBCO) produced by the British Oceanographic Data Centre.* Published July 21, 2005）
   2. [地形高度图 gebco_08_rev_elev_21600x10800.png](https://visibleearth.nasa.gov/images/73934/topography/84331l)（*Imagery by Jesse Allen, NASA's Earth Observatory, using data from the General Bathymetric Chart of the Oceans (GEBCO) produced by the British Oceanographic Data Centre.* Published July 21, 2005）




# 速查

## 渲染优先级

- 0 地面、城墙
- 6 河口
- 7 水岸
- 8 水体
- 9 河流
- 10 道路
- 11 大气层、体积云
- 12 编辑预览
- 12 经纬线
- 13 镜头眩光

## 数学计算

正二十面体，假设每个边长为 1，则：

- 除去南北极的上下两个五边形：
  - 点距离五边形中心距离 = 2 / √(10-2√5) = 0.8506508083520399
  - 边离五边形中心的距离 = √(点距离五边形中心距离^2^ - 0.5^2^) = 0.6881909602355867
  - 五边形中心离极点距离 = √(1 - 点距离五边形中心距离^2^) = 0.5257311121191337
  - 两五边形之间距离 = 2 - 2 * 五边形中心离极点距离 = 0.9485377757617326
  - 面距离球心的距离 = 两五边形之间距离 / 2 = 0.4742688878808663
  - 点距离球心的距离（外接球半径） = √(面距离球心的距离^2^ + 点距离五边形中心距离^2^) = 0.9739290404139989
  - 点的纬度角 = arcsin(面距离球心的距离 / 点距离球心的距离) = 0.508610984 弧度 = 29.141262794 角度
  - 边距离球心的距离 = √(点距离球心的距离^2^ - 0.5^2^) = 0.835785723592915
  - 连接共边两点和球心的夹角 = 2 * arcsin(0.5 / 点距离球心的距离) = 2 * 0.53912398 弧度 = 2 * 30.88952869度 = 61.77905738 度 = 1.07824796 弧度

## C#

1. **格式字符串**：
   1. 标准数字格式字符串：https://learn.microsoft.com/zh-cn/dotnet/standard/base-types/standard-numeric-format-strings

## 计算着色器

1. 关键 API：

   1. **RenderSceneData**：
      1. get_cam_projection() 返回用于渲染该帧的相机投影。
      2. get_cam_transform() 返回用于渲染该帧的相机变换。
      3. get_view_projection(view) 返回用于渲染该帧的每个视图的视图投影。
   2. **RenderSceneBuffersRD**:
      1. RID get_color_texture(msaa: bool = false) 返回渲染 3D 内容的颜色纹理。如果使用多视图，这将是一个包含所有视图的纹理数组。
      2. RID get_depth_texture(msaa: bool = false) 返回渲染 3D 内容的深度纹理。如果使用多视图，这将是一个包含所有视图的纹理数组。

2. 逆视图空间矩阵

   ```gdscript
   var proj_correction := Projection().create_depth_correction(true)
   var inv_projection_matrix := (proj_correction * scn_data.get_cam_projection()).inverse()
   var inv_view_matrix := Projection(scn_data.get_cam_transform())
   ```


## Git

清理 .git 文件夹

```shell
# 检查 git 仓库是否有孤悬节点（dangling blob），或者其他异常
git fsck
# 重整清理 git 文件夹
git gc --prune=now
```



# 玩法策划

1. 星球文明自动发展更迭部分
   1. 领土涂色、边界绘制
2. 玩家放置积累资源，干预星球文明部分
   1. 月背观察视角



# TODO

## 基础

- [ ] 《Fundamentals of Computer Graphics》
- [ ] 《Real-Time Rendering》
- [ ]  深入学习计算着色器和 Compositor

## 旧功能

### 我开发的

- [ ] 重复生成星球任务的互斥检测

### BUG FIX

- [ ] **网格生成**：目前地块连接和角落三角间存在接缝，估计是浮点数精度加上计算方向不同导致的
- [ ] **网格生成**：目前海面可能出现从中心到 3/4 边的接缝



## 新功能



- [ ] **GUI**：设置不可探索地块
- [ ] **GUI**：悬浮在地块上一定时间后显示的信息框
- [ ] **GUI**：相机移动时显示南北极指示



- [ ] **游戏内容**：领土颜色和边界
- [ ] **游戏内容**：国家名字显示



- [ ] **玩法策划**：文明科技树
- [ ] **玩法策划**：放置游戏系统设计



- [ ] **地图生成**：玩家自定义导入矩形地图（考虑要不要加个游戏内编辑器？）



- [ ] **DGGS**：求最短路径上的所有地块（划线）



- [ ] **小地图**：拉伸分辨率时，改变小地图大小
- [ ] **小地图**：摄像头位置需要更精确，以及添加缓动过渡效果
- [ ] **小地图**：摄像头朝向指示（还有点难，因为摄像头在移动过程中自然会相对于球面旋转）
- [ ] **小地图**：小地图按照当前位置移动 X 轴到居中
- [ ] **小地图**：切换正二十面体展开方式（平铺还是中心六向）
- [ ] **小地图**：切换2D、3D大小地图功能
- [ ] **小地图**：基本经纬度绘制



- [ ] **相机**：焦点处的 Box 换成一个飞船作为玩家观察聚焦点



- [ ] **美术效果**：更好看的天空盒（现在的太黑了）



# 正在开发



- [ ] **BUG FIX**：启动 Godot 引擎第一次时 HUD 场景报错，HexPlanetManager 无地形纹理。重载场景后正常

- [ ] **BUG FIX**：貌似初始的动态加载摄像机位置和实际后续位置使用焦点位置逻辑不一致，导致第一次启动编辑器边缘分块的地块标签初始化会找不到报错（实际视野比初始化的地块大），待排查原因。
- [ ] **BUG FIX**：连续多次点击小地图跳转后位置不正确
- [ ] **BUG FIX**：生成高和低于100半径星球时的最大高度、地表特征异常 
- [ ] **BUG FIX**：低半径时分块显示不出来
- [ ] **BUG FIX**：事件的内存泄漏问题（相应对象一直在增加），新建星球时卡顿问题（从大星球开始，即使生成很小的星球也会卡）
- [ ] **BUG FIX**：分块改造成对象池后，表面特征的 LOD 更新有问题（已观察到河流、植被 bug）
- [ ] **BUG FIX**：单位移动路径预览时，成本标签不显示



- [ ] **重构**：<u>取消全局事件总线的设计，转为获取单例或多例的 Node 的上下文，事件下沉到对应单例</u>



- [ ] **性能**：目前十万格星球在全球视角下，天体运动后20帧，不运动 60 帧，需要优化



- [ ] **动态加载**：大量地块标签的优化，降低 Draw call
- [ ] **动态加载**：合并低 LOD 的分块，降低 Draw call
  - [ ] <u>实现六边形四孔 DGGS 编码</u>
  - [ ] 处理 LOD 接缝



# 性能测试记录

## 2025-03-26

项目代码版本：92f3e8c8 —— feature(MainProject): 分块动态加载 - 地块标签采用对象池（但现在因为标签没有批处理，依然很吃性能）；每次重新生成星球时清空 LOD 网格缓存

| 地块细分 | 分块细分 | 地块总数 | 分块总数 | 性能数据                                                     | 备注                                             |
| -------- | -------- | -------- | -------- | ------------------------------------------------------------ | ------------------------------------------------ |
| 40       | 5        | 16k      | 252      | 内存 450MB ~ 700MB（随运动增长，然后貌似回收掉）；绘制对象总数 17.3k；绘制图元总数 710k；绘制调用总数 800 左右；相机运动中帧率 27 ~ 30，静止 60 | 最高的全球视角下，环球一圈以后的数据，天体运动中 |
| 40       | 10       | 16k      | 1002     | 内存 450MB ~ 700MB（随运动增长，然后貌似回收掉）；绘制对象总数 18.7k；绘制图元总数 700k；绘制调用总数 2.2k 左右；相机运动中帧率 37，静止 60 | 最高的全球视角下，环球一圈以后的数据，天体运动中 |
| 100      | 25       | 100k     | 6252     | 内存 1GB ~ 1.3GB（随运动增长，然后貌似回收掉）；绘制对象总数 112k；绘制图元总数 1.18m；绘制调用总数 11.7k 左右；相机静止中帧率 20（天体不运动时 60） | 最高的全球视角下，环球一圈以后的数据，天体运动中 |
| 100      | 15       | 100k     | 2252     | 内存 1GB（运动过程中比较稳定）；绘制对象总数 94.5k；绘制图元总数 1.22m；绘制调用总数 4.5k 左右；相机静止中帧率 32，运动中帧率 15 | 最高的全球视角下，环球一圈以后的数据，天体运动中 |

没理解：

1. 前两组为什么绘制调用低的时候反而帧率低了……大概规律就是当分块里的地块越多时，绘制调用越低，静态时性能越好，但运动时性能越差（怀疑和未采用异步限时优先队列来加载新分块有关）
2. 第三组为什么天体运动时和静止帧率相差这么大？（已排除的可能性：每帧写入太阳方向全局着色器变量）



# 已完成

## 2025-03-27

- [x] **重构**：分拆 GUI 和星球管理器的各大功能，理顺两者间的调用关系
- [x] **动态加载**：<u>搜索视野范围内的分块速度慢，需优化</u>（已采过先生成队列，后续处理的方式）
- [x] **动态加载**：<u>引入异步的限制耗时上限的优先队列</u>（初步实现异步，待细化机制）

## 2025-03-26

- [x] **动态加载**：不显示不运行的分块节点数量会影响每帧耗时，需要减少数量（貌似即使 `SetProcess(false)` 或者 `SetProcessMode(ProcessModeEnum.Disabled)` 都没用，它们也在后台耗费时间）；已改造成分块对象池
- [x] **动态加载**：LOD 网格缓存引入数量限制，控制缓存内存占据的大小
- [x] **轨道相机**：背光面灯光调弱
- [x] **重构**：分拆 ChunkManager 的各大功能：特征、动态加载、分块池

## 2025-03-23

- [x] **小地图**：将球面映射到矩形

## 2025-03-22

- [x] **美术效果**：地球公转、自转；卫星月球

## 2025-03-21

- [x] **GUI**：指南针（或者类似 Godot XYZ 的指示器）
- [x] **地图生成**：实现 Sebastian 根据现实地理数据生成真实地球地图的模式

## 2025-03-20

- [x] **地图生成**：实现 Sebastian 叠加噪声层原理生成地图的模式（这种才能使用计算着色器优化性能。目前基于 FastNoiseLite 实现，气候有些问题，待微调）

## 2025-03-19

- [x] **美术效果**：大气层
- [x] **美术效果**：体积云
- [ ] ~~HexPlanetManager 不显示特征？但 HexPlanetHud 显示（因为默认 EditMode 为 false，正常现象）~~

## 2025-03-16

- [x] **美术效果**：水面法线贴图
- [x] **美术效果**：水面波浪效果
- [x] **Shader**：球面 vec3 到平面材质的 UV 的映射（使用三平面投射法 Triplanar 处理）

## 2025-03-15

- [x] **动态加载**：动态加载的分块会显示未探索的特征的 BUG FIX
- [x] **动态加载**：LOD 网格缓存
- [x] 确认高程视野的逻辑是否正确（定位为坐标距离 bug）
- [x] 确认地图生成的逻辑是否正确（定位为坐标距离 bug）
- [x] **地图生成**：BUG FIX，感觉算法有问题，经常在南北极点生成孤零零的地块，而且能明显看到正二十面体面的分界（定位为坐标距离 bug）
- [x] **动态加载**：快速移动时无分块加载 BUG FIX

## 2025-03-14

- [x] **动态加载**：特征考虑如何转为 MultiMesh，实现降低 Draw call

- [x] **动态加载**：优化地块加载过程，保证视野范围内没有缺口
- [x] **动态加载**：地形的 LOD？生成远距离观察或未来 2D、3D 地图切换为小地图后的低分辨率地形，省略阶梯等细节

## 2025-03-13

- [x] **网格生成**：高细分时扰动过大的问题 bug fix
- [x] **探索视野**：高细分时视野 Shader 高度 bug fix
- [x] **网格生成**：高细分时地面破皮问题 fix
- [x] **GUI**：通过 FPS 防止枪穿模的 Shader，显示低于高度的地块预览
- [x] **GUI**：地块预览渲染网格应该保证六个边都渲染

## 2025-03-12

- [x] **重构**：多例场景下的依赖注入优化
- [x] **重构**：将 HexMetrics 中的属性、方法提取成 Service
- [x] **重构**：将 GUI、星球管理器设置的参数下沉到 Service

## 2025-03-11

- [x] **小地图**：点击小地图跳转
- [x] **动态加载**：根据分块与相机间的位置关系，动态懒加载网格，而不是一次性全部生成。

## 2025-03-10

- [x] **GUI**：基本经纬度绘制
- [x] **GUI**：相机移动时显示经纬度指示，淡入淡出效果
- [x] **DGGS**：地块经纬度（地形生成需要用）
- [x] **GUI**：固定经纬度显隐按钮

- [x] **GUI**：编辑模式下已选择地块的可视化
- [x] **小地图**：摄像头位置指示

## 2025-03-09

- [x] **GUI**：编辑地形前的预览效果（透明、高优先级显示）
- [ ] ~~**GUI**：编辑地形预览效果地面下方的显示~~（试过，但是效果不好，最终放弃了）
- [x] **GUI**：编辑地形预览效果使用不同材质

## 2025-03-08

- [x] **单位寻路**：当前会有优先队列 -1 数组越界的 bug
- [x] **GUI**：编辑模式下显示地块 ID、坐标

## 2025-03-06

- [x] **特征视野**：instance uniform 数量太多报错，需要改成 MultiMesh 的实现？（实际上改成了控制 Visible）

- [x] **单位寻路**：单位移动中途，继续移动单位的 bug——录视频时发现：已经走过的地方会留下单位霸占位置，也可能是 AStar3D 更新逻辑错了（其实是单位漏设置地块 Id 了）
- [x] **单位寻路**：探索视野外的单位寻路（貌似没有被禁止）——需要按照原教程方式自己实现搜索？而不是 AStar3D？（确实放弃了 AStar3D，但原因其实是没有检查地块的探索属性）
- [x] 鼠标中键移动鼠标
- [x] **DGGS**：求任意两地块最短距离（可视范围要用）

## 2025-03-05

- [x] 更新 Godot 4.4（阿牟说的动画播放器 bug、生命周期）
- [x] 4.4 怎么设置 Jolt 物理？(设置器里可以改) 
- [x] 墙面的可见性（写个新 Shader）
- [x] 特征的可见性（Shader 的 instance uniform）
- [x] 简易小地图