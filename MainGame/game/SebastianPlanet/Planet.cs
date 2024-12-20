using FrontEndToolFS.SebatianPlanet;
using Godot;

namespace ZeromaXPlayground.game.SebastianPlanet;

[Tool]
public partial class Planet : PlanetFS
{
    [Export]
    public bool Generate
    {
        get => generate;
        set
        {
            generate = value;
            GeneratePlanet();
        }
    }

    [Export]
    public bool AutoUpdate
    {
        get => autoUpdate;
        set => autoUpdate = value;
    }

    [Export]
    public int Resolution
    {
        get => resolution;
        set => resolution = value;
    }

    [ExportGroup("Color")]
    [Export]
    public Color PlanetColor
    {
        get => colorSettings.planetColor;
        set
        {
            colorSettings.planetColor = value;
            OnColorSettingsUpdated();
        }
    }

    [ExportGroup("Shape")]
    [Export]
    public float PlanetRadius
    {
        get => shapeSettings.planetRadius;
        set
        {
            shapeSettings.planetRadius = value;
            OnShapeSettingsUpdated();
        }
    }

    // 请忽略 IDE 冗余提示，需要保留此处和 partial
    public override void _Ready() => base._Ready();
}