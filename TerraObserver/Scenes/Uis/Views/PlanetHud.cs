using System;
using Godot;
using TO.Abstractions.Models.Planets;
using TO.Abstractions.Views.Cameras;
using TO.Abstractions.Views.Geos;
using TO.Abstractions.Views.Planets;
using TO.Abstractions.Views.Uis;
using TO.Domains.Enums.Meshes;
using TO.Domains.Shaders;
using TO.Domains.Structs.HexSphereGrids;
using TO.Domains.Utils.Commons;
using TO.Presenters.Views.Uis;

namespace TerraObserver.Scenes.Uis.Views;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-05 19:08:02
public partial class PlanetHud : PlanetHudFS, IPlanetHud
{
    #region 依赖

    public IPlanet Planet { get; set; } = null!;
    public ILonLatGrid LonLatGrid { get; set; } = null!;
    public ICelestialMotion CelestialMotion { get; set; } = null!;

    #endregion

    #region 生命周期

    public override void _Ready()
    {
        base._Ready();
        CelestialMotionCheckButton.Toggled += toggle =>
            CelestialMotion.PlanetRevolution = CelestialMotion.PlanetRotation =
                CelestialMotion.SatelliteRevolution = CelestialMotion.SatelliteRotation = toggle;
        LonLatFixCheckButton.Toggled += toggle => LonLatGrid.FixFullVisibility = toggle; // 锁定经纬网的显示
        RadiusLineEdit.TextSubmitted += text =>
        {
            if (float.TryParse(text, out var radius))
                Planet.Radius = radius;
            else
                RadiusLineEdit.Text = $"{Planet.Radius:F2}";
        };

        DivisionLineEdit.TextSubmitted += text =>
        {
            if (int.TryParse(text, out var division))
                Planet.Divisions = division;
            else
                DivisionLineEdit.Text = $"{Planet.Divisions}";
        };

        ChunkDivisionLineEdit.TextSubmitted += text =>
        {
            if (int.TryParse(text, out var chunkDivision))
                Planet.ChunkDivisions = chunkDivision;
            else
                DivisionLineEdit.Text = $"{Planet.ChunkDivisions}";
        };
    }

    #endregion

    #region 事件

    public event Action<int?>? ChosenTileIdChanged;
    public override void EmitChosenTileIdChanged(int? v) => ChosenTileIdChanged?.Invoke(v);

    #endregion
}