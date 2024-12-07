using FrontEndToolFS.Tool;
using Godot;
using Godot.Collections;

namespace ZeromaXPlayground.game.HexPlane.Map;

[Tool]
public partial class HexFeatureManager : HexFeatureManagerFS
{
    [Export]
    public Array<Array<PackedScene>> UrbanPrefabs
    {
        get => urbanPrefabs;
        set => urbanPrefabs = value;
    }

    [Export]
    public Array<Array<PackedScene>> FarmPrefabs
    {
        get => farmPrefabs;
        set => farmPrefabs = value;
    }

    [Export]
    public Array<Array<PackedScene>> PlantPrefabs
    {
        get => plantPrefabs;
        set => plantPrefabs = value;
    }

    [Export]
    public HexMeshFS Walls
    {
        get => walls;
        set => walls = value;
    }

    [Export]
    public PackedScene WallTower
    {
        get => wallTower;
        set => wallTower = value;
    }

    [Export]
    public PackedScene Bridge
    {
        get => bridge;
        set => bridge = value;
    }

    [Export]
    public PackedScene[] Special
    {
        get => special;
        set => special = value;
    }
}