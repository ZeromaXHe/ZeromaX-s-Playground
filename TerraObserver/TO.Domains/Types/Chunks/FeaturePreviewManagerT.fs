namespace TO.Domains.Types.Chunks

open System.Collections.Generic
open Godot
open TO.Domains.Types.Godots

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-07-09 13:48:09
[<Interface>]
type IFeaturePreviewManager =
    inherit INode3D
    // =====【Export】=====
    abstract UrbanPreviewOverrideMaterial: Material
    abstract PlantPreviewOverrideMaterial: Material
    abstract FarmPreviewOverrideMaterial: Material
    // =====【普通属性】=====
    abstract PreviewCount: int with get, set
    abstract EmptyPreviewIds: int HashSet

[<Interface>]
type IFeaturePreviewManagerQuery =
    abstract FeaturePreviewManager: IFeaturePreviewManager

type ClearFeaturePreviewManagerOldData = unit -> unit
type ShowFeaturePreview = Transform3D -> FeatureType -> int
type HideFeaturePreview = int -> unit

[<Interface>]
type IFeaturePreviewManagerCommand =
    abstract ClearFeaturePreviewManagerOldData: ClearFeaturePreviewManagerOldData
    abstract ShowFeaturePreview: ShowFeaturePreview
    abstract HideFeaturePreview: HideFeaturePreview
