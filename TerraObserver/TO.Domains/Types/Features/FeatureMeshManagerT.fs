namespace TO.Domains.Types.Features

open System.Collections.Generic
open Godot
open TO.Domains.Types.Godots

type FeatureType =
    // 城市
    | UrbanHigh1 = 0
    | UrbanHigh2 = 1
    | UrbanMid1 = 2
    | UrbanMid2 = 3
    | UrbanLow1 = 4
    | UrbanLow2 = 5
    // 农田
    | FarmHigh1 = 6
    | FarmHigh2 = 7
    | FarmMid1 = 8
    | FarmMid2 = 9
    | FarmLow1 = 10
    | FarmLow2 = 11
    // 植被
    | PlantHigh1 = 12
    | PlantHigh2 = 13
    | PlantMid1 = 14
    | PlantMid2 = 15
    | PlantLow1 = 16
    | PlantLow2 = 17
    // 特殊
    | Tower = 18
    | Bridge = 19
    | Castle = 20
    | Ziggurat = 21
    | MegaFlora = 22

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-07-09 13:47:09
[<Interface>]
type IFeatureMeshManager =
    inherit INode3D
    // =====【Export】=====
    abstract UrbanScenes: PackedScene array
    abstract FarmScenes: PackedScene array
    abstract PlantScenes: PackedScene array
    abstract WallTowerScene: PackedScene
    abstract BridgeScene: PackedScene
    abstract SpecialScenes: PackedScene array
    // =====【普通属性】=====
    abstract MultiUrbans: MultiMeshInstance3D array
    abstract MultiFarms: MultiMeshInstance3D array
    abstract MultiPlants: MultiMeshInstance3D array
    abstract MultiTowers: MultiMeshInstance3D
    abstract MultiBridges: MultiMeshInstance3D
    abstract MultiSpecials: MultiMeshInstance3D array
    abstract HidingIds: Dictionary<FeatureType, int HashSet>
    // =====【on-ready】=====
    abstract Urbans: Node3D
    abstract Farms: Node3D
    abstract Plants: Node3D
    abstract Others: Node3D

[<Interface>]
type IFeatureMeshManagerQuery =
    abstract FeatureMeshManager: IFeatureMeshManager

type ClearFeatureMashOldData = unit -> unit

[<Interface>]
type IFeatureMeshManagerCommand =
    abstract ClearFeatureMashOldData: ClearFeatureMashOldData
