namespace Apps.Models.Navigations;

public struct TileSearchData
{
    public int Distance;
    public int NextWithSamePriority;
    public int PathFrom;
    public int Heuristic;
    public int SearchPhase;

    public readonly int SearchPriority => Distance + Heuristic;
}