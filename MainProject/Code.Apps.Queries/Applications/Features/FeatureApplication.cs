using Apps.Queries.Abstractions.Features;
using Apps.Queries.Events;
using Domains.Models.Entities.PlanetGenerates;
using Infras.Writers.Abstractions.PlanetGenerates;

namespace Apps.Queries.Applications.Features;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-12 20:37:06
public class FeatureApplication(IFeatureRepo featureRepo) : IFeatureApplication
{
    public void HideFeatures(Tile tile, bool preview)
    {
        foreach (var feature in featureRepo.GetByTileId(tile.Id).Where(f => f.MeshId > -1))
        {
            FeatureEvent.EmitHidden(feature.MeshId, feature.Type, preview);
            feature.MeshId = -1;
        }
    }

    public void ClearFeatures(Tile tile, bool preview)
    {
        HideFeatures(tile, preview);
        featureRepo.DeleteByTileId(tile.Id);
    }

    public void ShowFeatures(Tile tile, bool onlyExplored, bool preview)
    {
        foreach (var feature in featureRepo.GetByTileId(tile.Id)
                     .Where(f => f.MeshId == -1 && (!onlyExplored || tile.Data.IsExplored)))
            feature.MeshId = FeatureEvent.EmitShown(feature.Transform, feature.Type, preview);
    }

    public void ExploreFeatures(Tile tile) => ShowFeatures(tile, true, false);
}