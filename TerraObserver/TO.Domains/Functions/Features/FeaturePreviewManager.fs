namespace TO.Domains.Functions.Features

open TO.Domains.Types.Features

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
