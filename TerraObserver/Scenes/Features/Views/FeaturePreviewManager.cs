using System.Collections.Generic;
using Godot;
using TO.Domains.Types.Features;

namespace TerraObserver.Scenes.Features.Views;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-07-09 14:07:07
[Tool]
public partial class FeaturePreviewManager : Node3D, IFeaturePreviewManager
{
    #region export 变量

    [Export] public Material? UrbanPreviewOverrideMaterial { get; private set; }
    [Export] public Material? PlantPreviewOverrideMaterial { get; private set; }
    [Export] public Material? FarmPreviewOverrideMaterial { get; private set; }

    #endregion

    #region 普通属性

    public int PreviewCount { get; set; }
    public HashSet<int> EmptyPreviewIds { get; } = [];

    #endregion
}