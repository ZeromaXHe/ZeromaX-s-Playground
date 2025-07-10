namespace TO.Domains.Functions.Features

open TO.Domains.Types.Features

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-07-09 14:03:09
module FeatureMeshManagerCommand =
    let clearFeatureMashOldData (env: #IFeatureMeshManagerQuery): ClearFeatureMashOldData =
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
