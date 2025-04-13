using Domains.Bases;
using Domains.Models.Entities.PlanetGenerates;
using Domains.Models.ValueObjects.PlanetGenerates;
using Godot;

namespace Domains.Repos.PlanetGenerates;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-12 19:48:12
public interface IFeatureRepo : IRepository<Feature>
{
    Feature Add(FeatureType type, Transform3D transform, int tileId);
    IEnumerable<Feature> GetByTileId(int tileId);
    void DeleteByTileId(int tileId);
}