namespace TO.Domains.Functions.Chunks

open Godot
open TO.Domains.Types.Chunks

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-07-09 14:03:09
module FeatureMeshManagerCommand =
    let clearFeatureMashOldData (env: #IFeatureMeshManagerQuery) : ClearFeatureMashOldData =
        fun () ->
            let this = env.FeatureMeshManager
            // 刷新 MultiMesh
            for multi in
                seq {
                    this.MultiUrbans
                    this.MultiFarms
                    this.MultiPlants
                }
                |> Seq.concat do
                multi.Multimesh.InstanceCount <- 10000
                multi.Multimesh.VisibleInstanceCount <- 0

            for multi in this.MultiSpecials do
                multi.Multimesh.InstanceCount <- 1000
                multi.Multimesh.VisibleInstanceCount <- 0

            this.MultiBridges.Multimesh.InstanceCount <- 3000
            this.MultiBridges.Multimesh.VisibleInstanceCount <- 0
            this.MultiTowers.Multimesh.InstanceCount <- 10000
            this.MultiTowers.Multimesh.VisibleInstanceCount <- 0
            // 清理 HidingIds
            for set in this.HidingIds.Values do
                set.Clear()

    let showFeatureMesh (env: 'E when 'E :> IFeatureQuery and 'E :> IFeatureMeshManagerQuery) : ShowFeatureMesh =
        fun (transform: Transform3D) (featType: FeatureType) ->
            let this = env.FeatureMeshManager
            let mesh = env.GetFeatureMultiMesh featType

            if this.HidingIds[featType].Count > 0 then
                // 如果有隐藏的实例，则可以考虑复用
                let popId = this.HidingIds[featType] |> Seq.head
                mesh.SetInstanceTransform(popId, transform)
                this.HidingIds[featType].Remove popId |> ignore
                popId
            elif mesh.VisibleInstanceCount = mesh.InstanceCount then
                GD.PrintErr $"MultiMesh is full of {mesh.InstanceCount} {featType}"
                -1
            else
                let id = mesh.VisibleInstanceCount
                mesh.SetInstanceTransform(id, transform)
                mesh.VisibleInstanceCount <- mesh.VisibleInstanceCount + 1
                id

    // 将特征缩小并放到球心，表示不可见
    let private hideTransform3D = Transform3D.Identity.Scaled(Vector3.One * 0.0001f)

    let hideFeatureMesh (env: 'E when 'E :> IFeatureQuery and 'E :> IFeatureMeshManagerQuery) : HideFeatureMesh =
        fun (id: int) (featType: FeatureType) ->
            let this = env.FeatureMeshManager
            let mesh = env.GetFeatureMultiMesh featType
            mesh.SetInstanceTransform(id, hideTransform3D)

            if mesh.VisibleInstanceCount - 1 = id then
                // 如果是最后一个，则可以考虑缩小可见实例数
                let mutable popId = id - 1

                while this.HidingIds[featType].Contains(id - 1) do
                    this.HidingIds[featType].Remove popId |> ignore
                    popId <- popId - 1

                mesh.VisibleInstanceCount <- popId + 1
            else
                this.HidingIds[featType].Add id |> ignore
