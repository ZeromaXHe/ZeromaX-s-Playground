using Apps.Queries.Abstractions.Features;
using Domains.Models.Entities.PlanetGenerates;
using Domains.Services.Abstractions.Events.Events;
using Domains.Services.Abstractions.Shaders;
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
            if (preview)
                FeatureEvent.EmitPreviewHidden(feature.MeshId, feature.Type);
            else
                FeatureEvent.EmitMeshHidden(feature.MeshId, feature.Type);
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
            feature.MeshId = preview
                ? FeatureEvent.EmitPreviewShown(feature.Transform, feature.Type)
                : FeatureEvent.EmitMeshShown(feature.Transform, feature.Type);
    }

    public void ExploreFeatures(Tile tile) => ShowFeatures(tile, true, false);
}