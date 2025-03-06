using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service;

public interface ITileShaderService
{
    delegate void TileExploredEvent(int tileId);

    event TileExploredEvent TileExplored;
    void Initialize();
    void RefreshTerrain(int tileId);
    void RefreshVisibility(int tileId);
    void IncreaseVisibility(Tile fromTile, int range);
    void DecreaseVisibility(Tile fromTile, int range);
    void UpdateData(float delta);
}