using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Struct;

public struct EdgeVertices(Vector3 corner1, Vector3 corner2, float outerStep = 0.25f)
{
    public Vector3 V1 = corner1,
        V2 = corner1.Lerp(corner2, outerStep),
        V3 = corner1.Lerp(corner2, 0.5f),
        V4 = corner1.Lerp(corner2, 1 - outerStep),
        V5 = corner2;

    public static EdgeVertices TerraceLerp(EdgeVertices a, EdgeVertices b, int step)
    {
        EdgeVertices result;
        result.V1 = HexMetrics.TerraceLerp(a.V1, b.V1, step);
        result.V2 = HexMetrics.TerraceLerp(a.V2, b.V2, step);
        result.V3 = HexMetrics.TerraceLerp(a.V3, b.V3, step);
        result.V4 = HexMetrics.TerraceLerp(a.V4, b.V4, step);
        result.V5 = HexMetrics.TerraceLerp(a.V5, b.V5, step);
        return result;
    }
}