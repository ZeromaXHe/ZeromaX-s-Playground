using System;
using FrontEndToolFS.SebastianPlanet;
using Godot;
using ZeromaXPlayground.game.SebastianPlanet.Settings;

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

    [Export]
    public FaceRenderMask FaceRenderMask
    {
        get => faceRenderMask;
        set => faceRenderMask = value;
    }

    [Export]
    public ColorSettings ColorSettings
    {
        get => colorSettings as ColorSettings;
        set
        {
            colorSettings = value;
            OnColorSettingsUpdated();
        }
    }

    [Export]
    public ShapeSettings ShapeSettings
    {
        get => shapeSettings as ShapeSettings;
        set
        {
            shapeSettings = value;
            OnShapeSettingsUpdated();
        }
    }

    // 请忽略 IDE 冗余提示，需要保留此处和 partial
    public override void _Ready() => base._Ready();
}