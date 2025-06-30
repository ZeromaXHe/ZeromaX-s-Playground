namespace TO.Domains.Types.PlanetHuds

open System.Collections.Generic

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-09 17:19:09
type HexTileDataOverrider() =
    [<DefaultValue>]
    val mutable EditMode: bool

    [<DefaultValue>]
    val mutable ApplyTerrain: bool

    [<DefaultValue>]
    val mutable ActiveTerrain: int

    [<DefaultValue>]
    val mutable ApplyElevation: bool

    [<DefaultValue>]
    val mutable ActiveElevation: int

    [<DefaultValue>]
    val mutable ApplyWaterLevel: bool

    [<DefaultValue>]
    val mutable ActiveWaterLevel: int

    [<DefaultValue>]
    val mutable BrushSize: int

    [<DefaultValue>]
    val mutable RiverMode: OptionalToggle

    [<DefaultValue>]
    val mutable RoadMode: OptionalToggle

    [<DefaultValue>]
    val mutable ApplyUrbanLevel: bool

    [<DefaultValue>]
    val mutable ActiveUrbanLevel: int

    [<DefaultValue>]
    val mutable ApplyFarmLevel: bool

    [<DefaultValue>]
    val mutable ActiveFarmLevel: int

    [<DefaultValue>]
    val mutable ApplyPlantLevel: bool

    [<DefaultValue>]
    val mutable ActivePlantLevel: int

    [<DefaultValue>]
    val mutable WalledMode: OptionalToggle

    [<DefaultValue>]
    val mutable ApplySpecialIndex: bool

    [<DefaultValue>]
    val mutable ActiveSpecialIndex: int

    member val OverrideTileIds = HashSet<int>()
