using Domains.Models.ValueObjects.PlanetGenerates;
using Godot;

namespace Domains.Services.Abstractions.Events.Events;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-12 15:27:03
public class FeatureEvent
{
    public static FeatureEvent Instance { get; } = new();

    // 返回在显示特征事件时回传特征 MultiMesh ID
    public delegate int ShowPreviewFeatureEvent(Transform3D transform, FeatureType type);

    public event ShowPreviewFeatureEvent? PreviewShown;

    public static int EmitPreviewShown(Transform3D transform, FeatureType type) =>
        Instance.PreviewShown?.Invoke(transform, type) ?? -1;

    public delegate int ShowMeshFeatureEvent(Transform3D transform, FeatureType type);

    public event ShowMeshFeatureEvent? MeshShown;

    public static int EmitMeshShown(Transform3D transform, FeatureType type) =>
        Instance.MeshShown?.Invoke(transform, type) ?? -1;

    public delegate void HidePreviewFeatureEvent(int id, FeatureType type);

    public event HidePreviewFeatureEvent? PreviewHidden;
    public static void EmitPreviewHidden(int id, FeatureType type) => Instance.PreviewHidden?.Invoke(id, type);

    public delegate void HideMeshFeatureEvent(int id, FeatureType type);

    public event HideMeshFeatureEvent? MeshHidden;
    public static void EmitMeshHidden(int id, FeatureType type) => Instance.MeshHidden?.Invoke(id, type);
}