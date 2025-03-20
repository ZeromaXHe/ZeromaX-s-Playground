using Godot;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Structs;

public struct HexHash
{
    public float A, B, C, D, E;

    public static HexHash Create()
    {
        HexHash hash;
        hash.A = GD.Randf() * 0.999f; // GD.Randf() 的范围是 [0f, 1f]，会取到 1f
        hash.B = GD.Randf() * 0.999f;
        hash.C = GD.Randf() * 0.999f;
        hash.D = GD.Randf() * 0.999f;
        hash.E = GD.Randf() * 0.999f;
        return hash;
    }
}