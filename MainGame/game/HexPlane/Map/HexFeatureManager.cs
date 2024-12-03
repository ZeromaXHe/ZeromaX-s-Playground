using FrontEndToolFS.Tool;
using Godot;
using Godot.Collections;

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
}