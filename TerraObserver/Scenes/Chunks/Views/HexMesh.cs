using System.Linq;
using Godot;
using TO.Abstractions.Views.Chunks;
using TO.Presenters.Views.Chunks;

namespace TerraObserver.Scenes.Chunks.Views;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-06 16:33:04
[Tool]
public partial class HexMesh : HexMeshFS, IHexMesh
{
    #region Export 属性

    [Export] public override bool UseCollider { get; set; }
    [Export] public override bool UseCellData { get; set; }
    [Export] public override bool UseUvCoordinates { get; set; }
    [Export] public override bool UseUv2Coordinates { get; set; }
    [Export] public override bool Smooth { get; set; }

    #endregion
}