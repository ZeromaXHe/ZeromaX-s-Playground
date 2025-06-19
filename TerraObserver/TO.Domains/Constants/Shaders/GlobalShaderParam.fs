namespace TO.Domains.Shaders

/// 全局着色器参数
/// 作为对其他 .NET 公开的 F# 库，需要遵循《F# 组件设计准则 - 命名空间和类型设计 - 使用命名空间、类型和成员作为组件的主要组织结构》
/// https://learn.microsoft.com/zh-cn/dotnet/fsharp/style-guide/component-design-guidelines#use-namespaces-types-and-members-as-the-primary-organizational-structure-for-your-components
/// 
/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-28 23:10:28
[<AbstractClass; Sealed>]
type GlobalShaderParam =
    static member Radius = "radius"
    static member Divisions = "divisions"
    static member MaxHeight = "max_height"
    static member HexMapEditMode = "hex_map_edit_mode"
    static member HexTileData = "hex_tile_data"
    static member HexTileCivData = "hex_tile_civ_data"
    static member HexTileDataTexelSize = "hex_tile_data_texel_size"
    static member DirToSun = "dir_to_sun"

