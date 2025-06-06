# ZeromaX 的泰拉观测者（ZeromaX's Terra Observer）

放置战略模拟游戏《泰拉观测者》，基于 Godot 引擎 + F# / C# / GDScript 语言开发，使用模型视图控制器 MVC + 函数式编程 FP + 领域驱动设计 DDD + 实体组件系统 ECS 进行代码架构

开发进展欢迎在 B 站持续关注：https://space.bilibili.com/27867310

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

## 素材

- Space
  - **milkyway panorama eso0932a.jpg**：图片来源于 [ESO 网站](https://www.eso.org/public/images/eso0932a/)，版权归属 **Credit:** ESO/S. Brunier。图片基于 [CC 4.0 许可证](https://www.eso.org/public/outreach/copyright/)使用。
  - **euvi_aia304_2012_carrington_searchweb.png**：图片来源于 [NASA 网站](https://svs.gsfc.nasa.gov/30362)，版权归属 credit: NASA JPL。图片基于 [NASA 图像和媒体使用指南](https://www.nasa.gov/nasa-brand-center/images-and-media/)使用

