using ZeromaXsPlaygroundProject.Scenes.Framework.Base;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;

public class Unit(int id = -1) : AEntity(id)
{
    public const int VisionRange = 3;
    public int TileId { get; set; }
}