namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Structs;

public struct TileSearchData
{
    public int Distance;
    public int NextWithSamePriority;
    public int PathFrom;
    public int Heuristic;
    public int SearchPhase;

    public readonly int SearchPriority => Distance + Heuristic;
}