using System;
using System.Collections.Generic;
using Godot;
using TerraObserver.Scenes.Planets.Models;
using TerraObserver.Scenes.Planets.Views;
using TO.Infras.Planets;

namespace TerraObserver.Scenes.Planets.Contexts;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-05-09 19:48
[Tool]
public partial class PlanetContext : Node
{
    #region export 变量

    [ExportGroup("Views 显示层")]
    [Export]
    public Planet? Planet
    {
        get => _planet;
        set
        {
            _planet = value;
            UpdateConfigurationWarnings();
        }
    }

    private Planet? _planet;

    [ExportGroup("Models 模型层")]
    [Export]
    public HexSphereConfigs? HexSphereConfigs
    {
        get => _hexSphereConfigs;
        set
        {
            if (_hexSphereConfigs != null)
                _hexSphereConfigs.ParamsChanged -= DrawHexSphereMesh;
            _hexSphereConfigs = value;
            if (_hexSphereConfigs != null)
                _hexSphereConfigs.ParamsChanged += DrawHexSphereMesh;
            UpdateConfigurationWarnings();
        }
    }

    private HexSphereConfigs? _hexSphereConfigs;

    [Export]
    public CatlikeCodingNoise? CatlikeCodingNoise
    {
        get => _catlikeCodingNoise;
        set
        {
            _catlikeCodingNoise = value;
            UpdateConfigurationWarnings();
        }
    }

    private CatlikeCodingNoise? _catlikeCodingNoise;

    public override string[] _GetConfigurationWarnings()
    {
        List<string> warnings = [];
        if (Planet == null)
            warnings.Add("显示层：Planet 为空;");
        if (HexSphereConfigs == null)
            warnings.Add("模型层: HexSphereSettings 为空;");
        if (CatlikeCodingNoise == null)
            warnings.Add("模型层: CatlikeCodingNoise 为空;");
        return warnings.ToArray();
    }

    #endregion

    private PlanetWorld _planetWorld = new();

    private bool NodeReady { get; set; }

    public override void _Ready()
    {
        NodeReady = true;
        DrawHexSphereMesh();
    }

    public override void _EnterTree()
    {
        if (HexSphereConfigs != null)
            HexSphereConfigs.ParamsChanged += DrawHexSphereMesh;
    }

    public override void _ExitTree()
    {
        if (HexSphereConfigs != null)
            HexSphereConfigs.ParamsChanged -= DrawHexSphereMesh;
    }

    private void DrawHexSphereMesh()
    {
        if (!NodeReady || HexSphereConfigs == null || Planet == null)
            return;
        var time = Time.GetTicksMsec();
        GD.Print($"[===DrawHexSphereMesh===] radius {HexSphereConfigs.Radius}, divisions {
            HexSphereConfigs.Divisions}, start at: {time}");
        _planetWorld.ClearOldData();
        var tiles = _planetWorld.InitHexSphere(HexSphereConfigs);

        foreach (var child in Planet.GetChildren())
            child.QueueFree();
        var meshIns = new MeshInstance3D();
        var surfaceTool = new SurfaceTool();
        surfaceTool.Begin(Mesh.PrimitiveType.Triangles);
        surfaceTool.SetSmoothGroup(uint.MaxValue);
        var vi = 0;
        foreach (var tile in tiles)
        {
            surfaceTool.SetColor(Color.FromHsv(GD.Randf(), GD.Randf(), GD.Randf()));
            // var heightMultiplier = (float)GD.RandRange(1, 1.05);
            var points = tile.GetCorners(HexSphereConfigs.Radius, 1f)!;
            // GD.Print($"points {vi}: {string.Join(", ", points)}");
            foreach (var point in points)
                surfaceTool.AddVertex(point);
            AddFaceIndex(vi, vi + 1, vi + 2, surfaceTool);
            AddFaceIndex(vi, vi + 2, vi + 3, surfaceTool);
            AddFaceIndex(vi, vi + 3, vi + 4, surfaceTool);
            if (points.Length > 5)
                AddFaceIndex(vi, vi + 4, vi + 5, surfaceTool);
            vi += points.Length;
        }

        surfaceTool.GenerateNormals();
        var material = new StandardMaterial3D();
        material.VertexColorUseAsAlbedo = true;
        surfaceTool.SetMaterial(material);
        meshIns.Mesh = surfaceTool.Commit();
        Planet.AddChild(meshIns);
        return;

        static void AddFaceIndex(int i0, int i1, int i2, SurfaceTool surfaceTool)
        {
            surfaceTool.AddIndex(i0);
            surfaceTool.AddIndex(i1);
            surfaceTool.AddIndex(i2);
        }
    }
}