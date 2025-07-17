using System.Collections.Generic;
using Godot;
using TO.Domains.Types.Maps;

namespace TerraObserver.Scenes.Maps.Models;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-07-04 09:56:13
[Tool]
[GlobalClass]
public partial class ErosionLandGenerator : LandGenerator, IErosionLandGenerator
{
    #region Export 属性

    [ExportGroup("Catlike Coding 侵蚀算法设置")]
    [Export(PropertyHint.Range, "5, 95")]
    public int LandPercentage { get; set; } = 50;

    [Export(PropertyHint.Range, "20, 200")]
    public int ChunkSizeMin { get; set; } = 30;

    [Export(PropertyHint.Range, "20, 200")]
    public int ChunkSizeMax { get; set; } = 100;

    [Export(PropertyHint.Range, "0.0, 1.0")]
    public float HighRiseProbability { get; set; } = 0.25f;

    [Export(PropertyHint.Range, "0.0, 0.4")]
    public float SinkProbability { get; set; } = 0.2f;

    [Export(PropertyHint.Range, "0, 0.5")] public float JitterProbability { get; set; } = 0.25f;

    [Export(PropertyHint.Range, "0, 100")] public int ErosionPercentage { get; set; } = 50;

    #endregion

    #region 普通属性

    public List<MapRegion> Regions { get; } = [];

    #endregion
}