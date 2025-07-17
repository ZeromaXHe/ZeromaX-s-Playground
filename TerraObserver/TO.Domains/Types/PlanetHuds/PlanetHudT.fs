namespace TO.Domains.Types.PlanetHuds

open System
open System.Collections.Generic
open Godot
open TO.Domains.Types.Godots

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-29 14:13:29
[<Interface>]
[<AllowNullLiteral>] // 因为 PlanetHud 不是 Tool，所以编辑器内可能为空；这注解还有向上传染性，所有父接口也得加…… 向下不影响
type IPlanetHud =
    inherit IControl
    // =====【on-ready】=====
    abstract MiniMapContainer: SubViewportContainer
    abstract CamLonLatLabel: Label
    abstract CompassPanel: PanelContainer
    abstract RectMap: TextureRect
    abstract RadiusLineEdit: LineEdit
    abstract DivisionLineEdit: LineEdit
    abstract ChunkDivisionLineEdit: LineEdit
    abstract LandGenOptionButton: OptionButton
    abstract ChunkCountLabel: Label
    abstract TileCountLabel: Label
    abstract IdLineEdit: LineEdit
    abstract ChunkLineEdit: LineEdit
    abstract CoordsLineEdit: LineEdit
    abstract HeightLineEdit: LineEdit
    abstract ElevationLineEdit: LineEdit
    abstract LonLineEdit: LineEdit
    abstract LatLineEdit: LineEdit
    abstract ElevationVSlider: VSlider
    abstract WaterVSlider: VSlider
    // =====【普通属性】=====
    abstract ChosenTileId: int Nullable with get, set
    abstract IsDrag: bool with get, set
    abstract DragTileId: int Nullable with get, set
    abstract PreviousTileId: int Nullable with get, set

    abstract LabelMode: int
    abstract EditMode: bool
    abstract ApplyTerrain: bool
    abstract ActiveTerrain: int
    abstract ApplyElevation: bool
    abstract ActiveElevation: int
    abstract ApplyWaterLevel: bool
    abstract ActiveWaterLevel: int
    abstract BrushSize: int
    abstract RiverMode: OptionalToggle
    abstract RoadMode: OptionalToggle
    abstract ApplyUrbanLevel: bool
    abstract ActiveUrbanLevel: int
    abstract ApplyFarmLevel: bool
    abstract ActiveFarmLevel: int
    abstract ApplyPlantLevel: bool
    abstract ActivePlantLevel: int
    abstract WalledMode: OptionalToggle
    abstract ApplySpecialIndex: bool
    abstract ActiveSpecialIndex: int

type GetLabelMode = unit -> int
type GetEditMode = unit -> bool

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-29 23:23:29
[<Interface>]
type IPlanetHudQuery =
    abstract PlanetHudOpt: IPlanetHud option // 因为 PlanetHud 不是 Tool，所以编辑器内可能为空
    abstract GetLabelMode: GetLabelMode
    abstract GetEditMode: GetEditMode

type OnOrbitCameraRigMoved = Vector3 -> float32 -> unit
type OnOrbitCameraRigTransformed = Transform3D -> unit
type InitElevationAndWaterVSlider = unit -> unit
type InitRectMiniMap = unit -> unit
type UpdateRadiusLineEdit = string -> unit
type UpdateDivisionLineEdit = bool -> string -> unit
type UpdateChosenTileInfo = IPlanetHud -> unit
type UpdateNewPlanetInfo = unit -> unit
type OnPlanetHudProcessed = unit -> unit

[<Interface>]
type IPlanetHudCommand =
    abstract OnOrbitCameraRigMoved: OnOrbitCameraRigMoved
    abstract OnOrbitCameraRigTransformed: OnOrbitCameraRigTransformed
    abstract InitElevationAndWaterVSlider: InitElevationAndWaterVSlider
    abstract InitRectMiniMap: InitRectMiniMap
    abstract UpdateRadiusLineEdit: UpdateRadiusLineEdit
    abstract UpdateDivisionLineEdit: UpdateDivisionLineEdit
    abstract UpdateChosenTileInfo: UpdateChosenTileInfo
    abstract UpdateNewPlanetInfo: UpdateNewPlanetInfo
    abstract OnPlanetHudProcessed: OnPlanetHudProcessed
