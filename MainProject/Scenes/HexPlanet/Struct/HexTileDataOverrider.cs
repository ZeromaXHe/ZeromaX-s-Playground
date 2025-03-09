using System.Collections.Generic;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Struct;

public enum OptionalToggle
{
    Ignore,
    Yes,
    No
}

public struct HexTileDataOverrider
{
    public bool EditMode;
    public bool ApplyTerrain;
    public int ActiveTerrain;
    public bool ApplyElevation;
    public int ActiveElevation;
    public bool ApplyWaterLevel;
    public int ActiveWaterLevel;
    public int BrushSize;
    public OptionalToggle RiverMode;
    public OptionalToggle RoadMode;
    public bool ApplyUrbanLevel;
    public int ActiveUrbanLevel;
    public bool ApplyFarmLevel;
    public int ActiveFarmLevel;
    public bool ApplyPlantLevel;
    public int ActivePlantLevel;
    public OptionalToggle WalledMode;
    public bool ApplySpecialIndex;
    public int ActiveSpecialIndex;
    public HashSet<Tile> OverrideTiles = [];

    public HexTileDataOverrider()
    {
    }
}