using Domains.Models.Entities.PlanetGenerates;

namespace Apps.Queries.Abstractions.Features;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-12 20:36:12
public interface IFeatureApplication
{
    void HideFeatures(Tile tile, bool preview);
    void ClearFeatures(Tile tile, bool preview);
    void ShowFeatures(Tile tile, bool onlyExplored, bool preview);
    void ExploreFeatures(Tile tile);
}