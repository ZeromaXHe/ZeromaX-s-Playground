namespace TO.Domains.Functions.Chunks

open Godot
open TO.Domains.Types.Chunks

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-07-09 14:03:09
module FeaturePreviewManagerCommand =
    let clearFeaturePreviewManagerOldData (env: #IFeaturePreviewManagerQuery) : ClearFeaturePreviewManagerOldData =
        fun () ->
            let this = env.FeaturePreviewManager
            this.PreviewCount <- 0
            this.EmptyPreviewIds.Clear()

            for child in this.GetChildren() do
                child.QueueFree()

    let private getPreviewOverrideMaterial (env: #IFeaturePreviewManagerQuery) =
        let this = env.FeaturePreviewManager

        function
        // 城市（红色）
        | FeatureType.UrbanHigh1
        | FeatureType.UrbanHigh2
        | FeatureType.UrbanMid1
        | FeatureType.UrbanMid2
        | FeatureType.UrbanLow1
        | FeatureType.UrbanLow2
        | FeatureType.Tower
        | FeatureType.Bridge
        | FeatureType.Castle
        | FeatureType.Ziggurat -> this.UrbanPreviewOverrideMaterial
        // 农田（黄绿色）
        | FeatureType.FarmHigh1
        | FeatureType.FarmHigh2
        | FeatureType.FarmMid1
        | FeatureType.FarmMid2
        | FeatureType.FarmLow1
        | FeatureType.FarmLow2 -> this.FarmPreviewOverrideMaterial
        // 植被（绿色）
        | FeatureType.PlantHigh1
        | FeatureType.PlantHigh2
        | FeatureType.PlantMid1
        | FeatureType.PlantMid2
        | FeatureType.PlantLow1
        | FeatureType.PlantLow2
        | FeatureType.MegaFlora -> this.PlantPreviewOverrideMaterial
        | featType -> failwith $"new type {nameof featType} no deal"

    let showFeaturePreview
        (env: 'E when 'E :> IFeatureQuery and 'E :> IFeaturePreviewManagerQuery)
        : ShowFeaturePreview =
        fun (transform: Transform3D) (featType: FeatureType) ->
            let mesh = (env.GetFeatureMultiMesh featType).Mesh
            let this = env.FeaturePreviewManager
            let mutable previewId = 0
            let mutable meshIns: MeshInstance3D = null

            if this.EmptyPreviewIds.Count = 0 then
                // 没有供复用的 MeshInstance3D，必须新建
                meshIns <- new MeshInstance3D()
                this.AddChild meshIns
                previewId <- this.PreviewCount
                this.PreviewCount <- this.PreviewCount + 1
            else
                // 复用已经存在的 MeshInstance3D
                previewId <- this.EmptyPreviewIds |> Seq.head
                // GetChild<MeshInstance3D>(previewId); 这个方法貌似是源生成器绑定的？没法用接口
                meshIns <- this.GetChild previewId :?> MeshInstance3D
                this.EmptyPreviewIds.Remove previewId |> ignore

            meshIns.Mesh <- mesh
            meshIns.MaterialOverride <- getPreviewOverrideMaterial env featType
            meshIns.Transform <- transform
            previewId

    let hideFeaturePreview (env: #IFeaturePreviewManagerQuery) : HideFeaturePreview =
        fun (id: int) ->
            let this = env.FeaturePreviewManager

            if id < this.PreviewCount then
                // 说明还没更新过星球
                let meshIns = this.GetChild id :?> MeshInstance3D
                meshIns.Mesh <- null
                this.EmptyPreviewIds.Add id |> ignore
