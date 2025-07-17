namespace TO.Domains.Functions.Chunks

open Godot
open Friflo.Engine.ECS
open TO.Domains.Functions.HexMetrics
open TO.Domains.Functions.HexSpheres.Components
open TO.Domains.Functions.HexSpheres.Components.Tiles
open TO.Domains.Functions.Maths
open TO.Domains.Types.Chunks
open TO.Domains.Types.Configs
open TO.Domains.Types.Friflos
open TO.Domains.Types.HexSpheres

module Feature =
    let getHeight =
        function
        // 城市
        | FeatureType.UrbanHigh1 -> 2.5f
        | FeatureType.UrbanHigh2 -> 1.5f
        | FeatureType.UrbanMid1 -> 1f
        | FeatureType.UrbanMid2 -> 0.75f
        | FeatureType.UrbanLow1 -> 0.5f
        | FeatureType.UrbanLow2 -> 0.5f
        // 农田
        | FeatureType.FarmHigh1
        | FeatureType.FarmHigh2
        | FeatureType.FarmMid1
        | FeatureType.FarmMid2
        | FeatureType.FarmLow1
        | FeatureType.FarmLow2 -> 0.05f
        // 植被
        | FeatureType.PlantHigh1 -> 2.25f
        | FeatureType.PlantHigh2 -> 1.5f
        | FeatureType.PlantMid1 -> 1.5f
        | FeatureType.PlantMid2 -> 0.75f
        | FeatureType.PlantLow1 -> 1f
        | FeatureType.PlantLow2 -> 0.5f
        // 特殊
        | FeatureType.Tower -> 2f // 塔高 4f
        | FeatureType.Bridge -> 0.7f // 0.7f 是桥梁需要略微抬高一点
        | FeatureType.Castle -> 2f
        | FeatureType.Ziggurat -> 1.25f
        | FeatureType.MegaFlora -> 5f
        | featType -> failwith $"new type {nameof featType} no deal"

module FeatureQuery =
    let getMultiMesh (env: #IFeatureMeshManagerQuery) : GetFeatureMultiMesh =
        fun (featType: FeatureType) ->
            let this = env.FeatureMeshManager

            match featType with
            | FeatureType.UrbanHigh1
            | FeatureType.UrbanHigh2
            | FeatureType.UrbanMid1
            | FeatureType.UrbanMid2
            | FeatureType.UrbanLow1
            | FeatureType.UrbanLow2 ->
                // 城市
                this.MultiUrbans[int featType - int FeatureType.UrbanHigh1].Multimesh
            | FeatureType.FarmHigh1
            | FeatureType.FarmHigh2
            | FeatureType.FarmMid1
            | FeatureType.FarmMid2
            | FeatureType.FarmLow1
            | FeatureType.FarmLow2 ->
                // 农田
                this.MultiFarms[int featType - int FeatureType.FarmHigh1].Multimesh
            | FeatureType.PlantHigh1
            | FeatureType.PlantHigh2
            | FeatureType.PlantMid1
            | FeatureType.PlantMid2
            | FeatureType.PlantLow1
            | FeatureType.PlantLow2 ->
                // 植被
                this.MultiPlants[int featType - int FeatureType.PlantHigh1].Multimesh
            // 特殊
            | FeatureType.Tower -> this.MultiTowers.Multimesh
            | FeatureType.Bridge -> this.MultiBridges.Multimesh
            | FeatureType.Castle
            | FeatureType.Ziggurat
            | FeatureType.MegaFlora -> this.MultiSpecials[int featType - int FeatureType.Castle].Multimesh
            | _ -> failwith $"new type {nameof featType} no deal"

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-07-10 13:52:10
module FeatureCommand =
    let add (env: #IEntityStoreQuery) : AddFeature =
        fun (featType: FeatureType) (transform: Transform3D) (tileId: TileId) (preview: bool) ->
            env.EntityStore
                .CreateEntity(FeatureComponent(featType, transform, tileId, preview), FeatureMeshId -1)
                .Id

    let showFeatures
        (env:
            'E
                when 'E :> IEntityStoreQuery
                and 'E :> IEntityStoreCommand
                and 'E :> IFeatureMeshManagerCommand
                and 'E :> IFeaturePreviewManagerCommand)
        : ShowFeatures =
        fun (onlyExplored: bool) (preview: bool) (tiles: Entity seq) ->
            env.ExecuteInCommandBuffer(fun cb ->
                for tile in tiles do
                    for f in env.EntityStore.ComponentIndex<FeatureComponent, TileId>()[tile.Id] do
                        let fComp = f.GetComponent<FeatureComponent>()
                        let fMeshId = f.GetComponent<FeatureMeshId>()

                        if
                            fMeshId.MeshId = -1
                            && (not onlyExplored || tile |> Tile.flag |> TileFlag.isExplored)
                            && fComp.Preview = preview
                        then
                            let newMeshId =
                                if preview then
                                    env.ShowFeaturePreview fComp.Transform fComp.Type
                                else
                                    env.ShowFeatureMesh fComp.Transform fComp.Type
                                |> FeatureMeshId

                            cb.AddComponent<FeatureMeshId>(f.Id, &newMeshId))

    let hideFeatures
        (env:
            'E
                when 'E :> IEntityStoreQuery
                and 'E :> IEntityStoreCommand
                and 'E :> IFeatureMeshManagerCommand
                and 'E :> IFeaturePreviewManagerCommand)
        : HideFeatures =
        fun (tileId: TileId) (preview: bool) ->
            env.ExecuteInCommandBuffer(fun cb ->
                for f in env.EntityStore.ComponentIndex<FeatureComponent, TileId>()[tileId] do
                    let fComp = f.GetComponent<FeatureComponent>()
                    let fMeshId = f.GetComponent<FeatureMeshId>()

                    if fMeshId.MeshId > -1 && fComp.Preview = preview then
                        if preview then
                            env.HideFeaturePreview fMeshId.MeshId
                        else
                            env.HideFeatureMesh fMeshId.MeshId fComp.Type

                        let newMeshId = FeatureMeshId -1
                        cb.AddComponent<FeatureMeshId>(f.Id, &newMeshId))

    let deleteFeatures (env: 'E when 'E :> IEntityStoreQuery and 'E :> IEntityStoreCommand) : DeleteFeatures =
        fun (tileId: TileId) (preview: bool) ->
            env.ExecuteInCommandBuffer(fun cb ->
                for f in env.EntityStore.ComponentIndex<FeatureComponent, TileId>()[tileId] do
                    if f.GetComponent<FeatureComponent>().Preview = preview then
                        cb.DeleteEntity(f.Id))

    let addTower (env: 'E when 'E :> IPlanetConfigQuery and 'E :> IFeatureCommand) : AddTower =
        fun (tileId: TileId) (left: Vector3) (right: Vector3) (preview: bool) ->
            let position = (left + right) * 0.5f

            let mutable transform =
                Math3dUtil.PlaceOnSphere(
                    Basis.Identity,
                    position,
                    Vector3.One * env.PlanetConfig.StandardScale,
                    Feature.getHeight FeatureType.Tower
                )

            let rightDirection = right - left

            transform <-
                transform.Rotated(
                    position.Normalized(),
                    transform.Basis.X.SignedAngleTo(rightDirection, position.Normalized())
                )
            // 不能采用 instance shader uniform 的解决方案，编辑器里会大量报错：
            // ERROR: servers/rendering/renderer_rd/storage_rd/material_storage.cpp:1791 - Condition "global_shader_uniforms.instance_buffer_pos.has(p_instance)" is true. Returning: -1
            // ERROR: Too many instances using shader instance variables. Increase buffer size in Project Settings.
            // tower.SetInstanceShaderParameter("tile_id", tile.Id);
            env.AddFeature FeatureType.Tower transform tileId preview |> ignore

    let addBridge
        (env: 'E when 'E :> ICatlikeCodingNoiseQuery and 'E :> IPlanetConfigQuery and 'E :> IFeatureCommand)
        : AddBridge =
        fun (tileId: TileId) (roadCenter1: Vector3) (roadCenter2: Vector3) (preview: bool) ->
            let roadCenter1 = env.Perturb roadCenter1
            let roadCenter2 = env.Perturb roadCenter2
            let position = (roadCenter1 + roadCenter2) * 0.5f
            let length = roadCenter1.DistanceTo roadCenter2
            let scale = env.PlanetConfig.StandardScale
            // 缩放需要沿着桥梁方向拉伸长度（X 轴）
            let mutable transform =
                Math3dUtil.PlaceOnSphere(
                    Basis.Identity,
                    position,
                    Vector3(length / HexMetrics.bridgeDesignLength, scale, scale),
                    Feature.getHeight FeatureType.Bridge
                )

            transform <-
                transform.Rotated(
                    position.Normalized(),
                    transform.Basis.X.SignedAngleTo(roadCenter2 - roadCenter1, position.Normalized())
                )

            env.AddFeature FeatureType.Bridge transform tileId preview |> ignore

    let addSpecialFeature
        (env:
            'E
                when 'E :> ITileOverriderQuery
                and 'E :> ICatlikeCodingNoiseQuery
                and 'E :> IPlanetConfigQuery
                and 'E :> IFeatureCommand)
        : AddSpecialFeature =
        fun (chunk: IChunk) (tile: Entity) (position: Vector3) ->
            let specialType =
                match env.GetOverrideSpecialIndex chunk tile with
                | 1 -> FeatureType.Castle
                | 2 -> FeatureType.Ziggurat
                | 3 -> FeatureType.MegaFlora
                | idx -> failwith $"Special feature index {idx} is invalid"

            let position = env.Perturb position

            let mutable transform =
                Math3dUtil.PlaceOnSphere(
                    Basis.Identity,
                    position,
                    Vector3.One * env.PlanetConfig.StandardScale,
                    Feature.getHeight specialType
                )

            let hash = env.SampleHashGrid position
            transform <- transform.Rotated(position.Normalized(), hash.E * Mathf.Tau)

            env.AddFeature specialType transform tile.Id <| chunk :? IEditPreviewChunk
            |> ignore

    let private featureThresholds =
        [| [| 0f; 0f; 0.4f |]; [| 0f; 0.4f; 0.6f |]; [| 0.4f; 0.6f; 0.8f |] |]

    let private pickFeatureSizeType (baseType: FeatureType) (level: int) (hash: float32) (choice: float32) =
        if level <= 0 then
            None
        else
            // let mutable res = None
            // let thresholds = featureThresholds[level - 1]
            //
            // for i in 0 .. thresholds.Length - 1 do
            //     if res.IsNone && hash < thresholds[i] then
            //         res <- Some <| enum<FeatureType> (int baseType + i * 2 + int (choice * 2f))
            //
            // res
            featureThresholds[level - 1]
            |> Array.tryFindIndex (fun t -> hash < t)
            |> Option.map (fun i -> enum<FeatureType> <| int baseType + i * 2 + int (choice * 2f))

    let addNormalFeature
        (env:
            'E
                when 'E :> ITileOverriderQuery
                and 'E :> ICatlikeCodingNoiseQuery
                and 'E :> IPlanetConfigQuery
                and 'E :> IFeatureCommand)
        =
        fun (chunk: IChunk) (tile: Entity) (position: Vector3) ->
            if not <| env.IsOverrideSpecial chunk tile then
                let hash = env.SampleHashGrid position

                let mutable typeOpt =
                    pickFeatureSizeType FeatureType.UrbanHigh1 (env.GetOverrideUrbanLevel chunk tile) hash.A hash.D

                let mutable otherTypeOpt =
                    pickFeatureSizeType FeatureType.FarmHigh1 (env.GetOverrideFarmLevel chunk tile) hash.B hash.D

                let mutable usedHash = hash.A

                if typeOpt.IsSome then
                    if otherTypeOpt.IsSome && hash.B < hash.A then
                        typeOpt <- otherTypeOpt
                        usedHash <- hash.B
                elif otherTypeOpt.IsSome then
                    typeOpt <- otherTypeOpt
                    usedHash <- hash.B

                otherTypeOpt <-
                    pickFeatureSizeType FeatureType.PlantHigh1 (env.GetOverridePlantLevel chunk tile) hash.C hash.D

                if typeOpt.IsSome then
                    if otherTypeOpt.IsSome && hash.C < usedHash then
                        typeOpt <- otherTypeOpt
                elif otherTypeOpt.IsSome then
                    typeOpt <- otherTypeOpt

                match typeOpt with
                | None -> ()
                | Some featType ->
                    let position = env.Perturb position

                    let transform =
                        Math3dUtil
                            .PlaceOnSphere(
                                Basis.Identity,
                                position,
                                Vector3.One * env.PlanetConfig.StandardScale,
                                Feature.getHeight featType
                            )
                            .Rotated(position.Normalized(), hash.E * Mathf.Tau) // 入参 axis 还是得用全局坐标

                    env.AddFeature featType transform tile.Id <| chunk :? IEditPreviewChunk
                    |> ignore
