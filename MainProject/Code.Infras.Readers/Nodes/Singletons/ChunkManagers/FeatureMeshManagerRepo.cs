using Domains.Models.ValueObjects.PlanetGenerates;
using Godot;
using Infras.Readers.Abstractions.Nodes.Singletons.ChunkManagers;
using Infras.Readers.Bases;
using Nodes.Abstractions.ChunkManagers;

namespace Infras.Readers.Nodes.Singletons.ChunkManagers;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 19:27:43
public class FeatureMeshManagerRepo : SingletonNodeRepo<IFeatureMeshManager>, IFeatureMeshManagerRepo
{
    // 将特征缩小并放到球心，表示不可见
    private static readonly Transform3D HideTransform3D = Transform3D.Identity.Scaled(Vector3.One * 0.0001f);

    public void OnHideFeature(int id, FeatureType type)
    {
        if (Singleton == null) return;
        var mesh = GetMultiMesh(type);
        mesh.SetInstanceTransform(id, HideTransform3D);
        if (mesh.VisibleInstanceCount - 1 == id) // 如果是最后一个，则可以考虑缩小可见实例数
        {
            var popId = id - 1;
            while (Singleton!.HidingIds[type].Contains(id - 1))
            {
                Singleton.HidingIds[type].Remove(popId);
                popId--;
            }

            mesh.VisibleInstanceCount = popId + 1;
        }
        else
            Singleton!.HidingIds[type].Add(id);
    }

    public int OnShowFeature(Transform3D transform, FeatureType type)
    {
        if (Singleton == null) return -1;
        var mesh = GetMultiMesh(type);
        if (Singleton!.HidingIds[type].Count > 0)
        {
            // 如果有隐藏的实例，则可以考虑复用
            var popId = Singleton.HidingIds[type].First();
            mesh.SetInstanceTransform(popId, transform);
            Singleton.HidingIds[type].Remove(popId);
            return popId;
        }

        if (mesh.VisibleInstanceCount == mesh.InstanceCount)
        {
            GD.PrintErr($"MultiMesh is full of {mesh.InstanceCount} {type}");
            return -1;
        }

        var id = mesh.VisibleInstanceCount;
        mesh.SetInstanceTransform(id, transform);
        mesh.VisibleInstanceCount++;
        return id;
    }

    public MultiMesh GetMultiMesh(FeatureType type) => type switch
    {
        // 城市
        FeatureType.UrbanHigh1 or FeatureType.UrbanHigh2
            or FeatureType.UrbanMid1 or FeatureType.UrbanMid2
            or FeatureType.UrbanLow1 or FeatureType.UrbanLow2 =>
            Singleton!.MultiUrbans![type - FeatureType.UrbanHigh1].Multimesh,
        // 农田
        FeatureType.FarmHigh1 or FeatureType.FarmHigh2
            or FeatureType.FarmMid1 or FeatureType.FarmMid2
            or FeatureType.FarmLow1 or FeatureType.FarmLow2 =>
            Singleton!.MultiFarms![type - FeatureType.FarmHigh1].Multimesh,

        // 植被
        FeatureType.PlantHigh1 or FeatureType.PlantHigh2
            or FeatureType.PlantMid1 or FeatureType.PlantMid2
            or FeatureType.PlantLow1 or FeatureType.PlantLow2 =>
            Singleton!.MultiPlants![type - FeatureType.PlantHigh1].Multimesh,

        // 特殊
        FeatureType.Tower => Singleton!.MultiTowers!.Multimesh,
        FeatureType.Bridge => Singleton!.MultiBridges!.Multimesh,
        FeatureType.Castle or FeatureType.Ziggurat or FeatureType.MegaFlora =>
            Singleton!.MultiSpecials![type - FeatureType.Castle].Multimesh,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, "new type no deal")
    };
}