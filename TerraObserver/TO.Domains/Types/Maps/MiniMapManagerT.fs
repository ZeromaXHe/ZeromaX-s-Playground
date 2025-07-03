namespace TO.Domains.Types.Maps

open Friflo.Engine.ECS
open Godot
open TO.Domains.Types.Godots

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-07-03 14:45:03
[<Interface>]
[<AllowNullLiteral>] // 因为不是 Tool，所以编辑器内可能为空；这注解还有向上传染性，所有父接口也得加…… 向下不影响
type IMiniMapManager =
    inherit INode2D
    // =====【事件】=====
    abstract EmitClicked: Vector3 -> unit
    // =====【on-ready】=====
    abstract TerrainLayer: TileMapLayer
    abstract ColorLayer: TileMapLayer
    abstract Camera: Camera2D
    abstract CameraIcon: Sprite2D

[<Interface>]
type IMiniMapManagerQuery =
    abstract MiniMapManagerOpt: IMiniMapManager option

type ClickOnMiniMap = unit -> unit
type SyncCameraIconPos = Vector3 -> unit
type InitMiniMap = Vector3 -> unit
type RefreshMiniMapTile = Entity -> unit

[<Interface>]
type IMiniMapManagerCommand =
    abstract ClickOnMiniMap: ClickOnMiniMap
    abstract SyncCameraIconPos: SyncCameraIconPos
    abstract InitMiniMap: InitMiniMap
    abstract RefreshMiniMapTile: RefreshMiniMapTile
