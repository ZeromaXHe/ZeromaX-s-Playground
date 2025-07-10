namespace TO.Domains.Types.Chunks

open Friflo.Engine.ECS
open Godot
open TO.Domains.Types.HexSpheres

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
/// Date: 2025-07-10 13:35:10
[<Struct>]
type FeatureComponent =
    interface TileId IIndexedComponent with
        member this.GetIndexedValue() = this.TileId

    val Type: FeatureType
    val Transform: Transform3D
    val TileId: TileId
    val Preview: bool

    new(featType: FeatureType, transform: Transform3D, tileId: TileId, preview: bool) =
        { Type = featType
          Transform = transform
          TileId = tileId
          Preview = preview }

[<Struct>]
type FeatureMeshId =
    interface IComponent
    val MeshId: int
    new(meshId: int) = { MeshId = meshId }

type GetFeatureMultiMesh = FeatureType -> MultiMesh

[<Interface>]
type IFeatureQuery =
    abstract GetFeatureMultiMesh: GetFeatureMultiMesh

type AddFeature = FeatureType -> Transform3D -> TileId -> bool -> int
type ShowFeatures = bool -> bool -> Entity seq -> unit
type HideFeatures = TileId -> bool -> unit
type DeleteFeatures = TileId -> bool -> unit
type AddTower = TileId -> Vector3 -> Vector3 -> bool -> unit
type AddBridge = TileId -> Vector3 -> Vector3 -> bool -> unit
type AddSpecialFeature = IChunk -> Entity -> Vector3 -> unit
type AddNormalFeature = IChunk -> Entity -> Vector3 -> unit

[<Interface>]
type IFeatureCommand =
    abstract AddFeature: AddFeature
    abstract ShowFeatures: ShowFeatures
    abstract HideFeatures: HideFeatures
    abstract DeleteFeatures: DeleteFeatures
    abstract AddTower: AddTower
    abstract AddBridge: AddBridge
    abstract AddSpecialFeature: AddSpecialFeature
    abstract AddNormalFeature: AddNormalFeature
