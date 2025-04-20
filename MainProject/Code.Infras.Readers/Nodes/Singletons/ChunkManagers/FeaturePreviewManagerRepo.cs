using Domains.Models.ValueObjects.PlanetGenerates;
using Godot;
using Infras.Readers.Abstractions.Nodes.Singletons.ChunkManagers;
using Infras.Readers.Bases;
using Nodes.Abstractions.ChunkManagers;

namespace Infras.Readers.Nodes.Singletons.ChunkManagers;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 19:29:52
public class FeaturePreviewManagerRepo : SingletonNodeRepo<IFeaturePreviewManager>, IFeaturePreviewManagerRepo
{
    public void OnHideFeature(int id, FeatureType type)
    {
        if (Singleton == null) return;
        if (id >= Singleton.PreviewCount) return; // 说明更新过星球
        ((MeshInstance3D)Singleton.GetChild(id)).Mesh = null;
        Singleton.EmptyPreviewIds.Add(id);
    }

    public int OnShowFeature(Transform3D transform, FeatureType type, Mesh mesh)
    {
        if (Singleton == null) return -1;
        MeshInstance3D meshIns;
        int previewId;
        if (Singleton.EmptyPreviewIds.Count == 0)
        {
            // 没有供复用的 MeshInstance3D，必须新建
            meshIns = new MeshInstance3D();
            Singleton.AddChild(meshIns);
            previewId = Singleton.PreviewCount++;
        }
        else
        {
            // 复用已经存在的 MeshInstance3D
            previewId = Singleton.EmptyPreviewIds.First();
            // GetChild<MeshInstance3D>(previewId); 这个方法貌似是源生成器绑定的？没法用接口
            meshIns = (MeshInstance3D)Singleton.GetChild(previewId);
            Singleton.EmptyPreviewIds.Remove(previewId);
        }

        meshIns.Mesh = mesh;
        meshIns.MaterialOverride = GetPreviewOverrideMaterial(type);
        meshIns.Transform = transform;
        return previewId;
    }
    
    private Material? GetPreviewOverrideMaterial(FeatureType type) => type switch
    {
        // 城市（红色）
        FeatureType.UrbanHigh1 or FeatureType.UrbanHigh2 or FeatureType.UrbanMid1 or FeatureType.UrbanMid2
            or FeatureType.UrbanLow1 or FeatureType.UrbanLow2 or FeatureType.Tower or FeatureType.Bridge
            or FeatureType.Castle or FeatureType.Ziggurat => Singleton!.UrbanPreviewOverrideMaterial,
        // 农田（黄绿色）
        FeatureType.FarmHigh1 or FeatureType.FarmHigh2 or FeatureType.FarmMid1 or FeatureType.FarmMid2
            or FeatureType.FarmLow1 or FeatureType.FarmLow2 => Singleton!.FarmPreviewOverrideMaterial,
        // 植被（绿色）
        FeatureType.PlantHigh1 or FeatureType.PlantHigh2 or FeatureType.PlantMid1 or FeatureType.PlantMid2
            or FeatureType.PlantLow1 or FeatureType.PlantLow2
            or FeatureType.MegaFlora => Singleton!.PlantPreviewOverrideMaterial,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, "new type no deal")
    };
}