using Domains.Models.Bases;
using Domains.Models.ValueObjects.PlanetGenerates;
using Godot;

namespace Domains.Models.Entities.PlanetGenerates;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-12 19:41:38
public class Feature(FeatureType type, Transform3D transform, int tileId, bool preview, int id = -1) : Entity(id)
{
    public FeatureType Type { get; } = type;
    public Transform3D Transform { get; } = transform;
    public int TileId { get; } = tileId;
    public bool Preview { get; } = preview;
    public int MeshId { get; set; } = -1;
}