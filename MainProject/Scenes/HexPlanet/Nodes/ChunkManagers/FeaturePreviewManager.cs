using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Nodes.ChunkManagers;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-26 21:28:04
[Tool]
public partial class FeaturePreviewManager : Node3D
{
    [Export] private Material _urbanPreviewOverrideMaterial;
    [Export] private Material _plantPreviewOverrideMaterial;
    [Export] private Material _farmPreviewOverrideMaterial;

    private Material GetPreviewOverrideMaterial(FeatureType type) => type switch
    {
        // 城市（红色）
        FeatureType.UrbanHigh1 or FeatureType.UrbanHigh2 or FeatureType.UrbanMid1 or FeatureType.UrbanMid2
            or FeatureType.UrbanLow1 or FeatureType.UrbanLow2 or FeatureType.Tower or FeatureType.Bridge
            or FeatureType.Castle or FeatureType.Ziggurat => _urbanPreviewOverrideMaterial,
        // 农田（黄绿色）
        FeatureType.FarmHigh1 or FeatureType.FarmHigh2 or FeatureType.FarmMid1 or FeatureType.FarmMid2
            or FeatureType.FarmLow1 or FeatureType.FarmLow2 => _farmPreviewOverrideMaterial,
        // 植被（绿色）
        FeatureType.PlantHigh1 or FeatureType.PlantHigh2 or FeatureType.PlantMid1 or FeatureType.PlantMid2
            or FeatureType.PlantLow1 or FeatureType.PlantLow2
            or FeatureType.MegaFlora => _plantPreviewOverrideMaterial,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, "new type no deal")
    };

    public void ClearForData()
    {
        _previewCount = 0;
        _emptyPreviewIds.Clear();
        foreach (var child in GetChildren())
            child.QueueFree();
    }

    private int _previewCount;
    private readonly HashSet<int> _emptyPreviewIds = [];

    public void OnHideFeature(int id, FeatureType type)
    {
        if (id >= _previewCount) return; // 说明更新过星球
        GetChild<MeshInstance3D>(id).Mesh = null;
        _emptyPreviewIds.Add(id);
    }

    public int OnShowFeature(Transform3D transform, FeatureType type, Mesh mesh)
    {
        MeshInstance3D meshIns;
        int previewId;
        if (_emptyPreviewIds.Count == 0)
        {
            // 没有供复用的 MeshInstance3D，必须新建
            meshIns = new MeshInstance3D();
            AddChild(meshIns);
            previewId = _previewCount++;
        }
        else
        {
            // 复用已经存在的 MeshInstance3D
            previewId = _emptyPreviewIds.First();
            meshIns = GetChild<MeshInstance3D>(previewId);
            _emptyPreviewIds.Remove(previewId);
        }

        meshIns.Mesh = mesh;
        meshIns.MaterialOverride = GetPreviewOverrideMaterial(type);
        meshIns.Transform = transform;
        return previewId;
    }
}