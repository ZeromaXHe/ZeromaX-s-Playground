using Domains.Models.Entities.PlanetGenerates;
using Domains.Models.ValueObjects.PlanetGenerates;
using Domains.Services.Abstractions.Nodes;
using Infras.Readers.Abstractions.Nodes.Singletons;
using Infras.Writers.Abstractions.PlanetGenerates;
using Nodes.Abstractions;

namespace Domains.Services.Nodes;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 20:20:41
public class EditPreviewChunkService(
    IEditPreviewChunkRepo editPreviewChunkRepo,
    IHexPlanetHudRepo hexPlanetHudRepo,
    ITileRepo tileRepo) : IEditPreviewChunkService
{
    private IEditPreviewChunk Self => editPreviewChunkRepo.Singleton!;
    public void Update(Tile? tile)
    {
        if (tile != null)
        {
            // 更新地块预览
            Self.Refresh(hexPlanetHudRepo.GetTileOverrider(),
                tileRepo.GetTilesInDistance(tile, hexPlanetHudRepo.GetTileOverrider().BrushSize));
            Self.Show();
        }
        else Self.Hide();
    }
}