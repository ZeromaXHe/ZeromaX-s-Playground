using Domains.Models.Entities.PlanetGenerates;
using Domains.Models.ValueObjects.PlanetGenerates;
using Godot;

namespace Apps.Events;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-12 15:27:03
public class FeatureEvent
{
    public static FeatureEvent Instance { get; } = new();

    // 返回在显示特征事件时回传特征 MultiMesh ID
    public delegate int ShowFeatureEvent(Transform3D transform, FeatureType type, bool preview);

    public event ShowFeatureEvent? Shown;

    public static int EmitShown(Transform3D transform, FeatureType type, bool preview) =>
        Instance.Shown?.Invoke(transform, type, preview) ?? -1;

    public delegate void HideFeatureEvent(int id, FeatureType type, bool preview);

    public event HideFeatureEvent? Hidden;
    public static void EmitHidden(int id, FeatureType type, bool preview) => Instance.Hidden?.Invoke(id, type, preview);
}