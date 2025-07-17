namespace TO.Domains.Types.Chunks

open System.Collections.Generic
open Godot
open TO.Domains.Types.Godots

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
type ShowFeatureMesh = Transform3D -> FeatureType -> int
type HideFeatureMesh = int -> FeatureType -> unit

[<Interface>]
type IFeatureMeshManagerCommand =
    abstract ClearFeatureMashOldData: ClearFeatureMashOldData
    abstract ShowFeatureMesh: ShowFeatureMesh
    abstract HideFeatureMesh: HideFeatureMesh
