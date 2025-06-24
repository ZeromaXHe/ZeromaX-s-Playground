using Godot;
using Godot.Abstractions.Bases;

namespace TO.Abstractions.Views.Geos;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-04 10:53:04
public interface ILonLatGrid : INode3D
{
    #region Export 属性

    int LongitudeInterval { get; set; }
    int LatitudeInterval { get; set; }
    int Segments { get; set; }
    Material? LineMaterial { get; set; }
    Color NormalLineColor { get; set; }
    Color DeeperLineColor { get; set; }
    int DeeperLineInterval { get; set; }
    Color TropicColor { get; set; }
    Color CircleColor { get; set; }
    Color EquatorColor { get; set; }
    Color Degree90LongitudeColor { get; set; }
    Color MeridianColor { get; set; }
    bool DrawTropicOfCancer { get; set; }
    bool DrawTropicOfCapricorn { get; set; }
    bool DrawArcticCircle { get; set; }
    bool DrawAntarcticCircle { get; set; }
    float FullVisibilityTime { get; set; }

    bool FixFullVisibility { get; set; }

    #endregion

    #region 普通属性

    float Visibility { get; set; }
    bool FadeVisibility { get; set; }
    float Radius { get; set; }
    MeshInstance3D? MeshIns { get; set; }

    #endregion
}