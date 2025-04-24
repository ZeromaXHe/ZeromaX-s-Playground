using System;
using Contexts;
using Domains.Models.Entities.PlanetGenerates;
using Domains.Models.ValueObjects.PlanetGenerates;
using Godot;
using GodotNodes.Abstractions.Addition;
using Nodes.Abstractions;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-02-17 15:49
public partial class HexPlanetHud : Control, IHexPlanetHud
{
    public HexPlanetHud()
    {
        Context.RegisterToHolder<IHexPlanetHud>(this);
    }

    public event Action<Tile?>? ChosenTileChanged;

    public NodeEvent NodeEvent { get; } = new(process: true);

    public override void _Ready()
    {
        InitOnReadyNodes();
    }

    public override void _Process(double delta) => NodeEvent.EmitProcessed(delta);

    #region on-ready 节点

    public SubViewportContainer? SubViewportContainer { get; private set; }

    public CheckButton? WireframeCheckButton { get; private set; }
    public CheckButton? CelestialMotionCheckButton { get; private set; }

    // 小地图
    public SubViewportContainer? MiniMapContainer { get; private set; }
    public Label? CamLatLonLabel { get; private set; }
    public CheckButton? LatLonFixCheckButton { get; private set; }

    // 指南针
    public PanelContainer? CompassPanel { get; private set; }

    // 矩形地图测试
    public TextureRect? RectMap { get; private set; }

    // 星球信息
    public TabBar? PlanetTabBar { get; private set; }
    public GridContainer? PlanetGrid { get; private set; }
    public LineEdit? RadiusLineEdit { get; private set; }
    public LineEdit? DivisionLineEdit { get; private set; }
    public LineEdit? ChunkDivisionLineEdit { get; private set; }

    // 地块信息
    public TabBar? TileTabBar { get; private set; }
    public VBoxContainer? TileVBox { get; private set; }
    public Label? ChunkCountLabel { get; private set; }
    public Label? TileCountLabel { get; private set; }
    public OptionButton? ShowLableOptionButton { get; private set; }
    public GridContainer? TileGrid { get; private set; }
    public LineEdit? IdLineEdit { get; private set; }
    public LineEdit? ChunkLineEdit { get; private set; }
    public LineEdit? CoordsLineEdit { get; private set; }
    public LineEdit? HeightLineEdit { get; private set; }
    public LineEdit? ElevationLineEdit { get; private set; }
    public LineEdit? LonLineEdit { get; private set; }
    public LineEdit? LatLineEdit { get; private set; }

    // 编辑功能
    public CheckButton? EditCheckButton { get; private set; }
    public TabBar? EditTabBar { get; private set; }
    public GridContainer? EditGrid { get; private set; }
    public OptionButton? TerrainOptionButton { get; private set; }
    public VSlider? ElevationVSlider { get; private set; }
    public CheckButton? ElevationCheckButton { get; private set; }
    public Label? ElevationValueLabel { get; private set; }
    public VSlider? WaterVSlider { get; private set; }
    public CheckButton? WaterCheckButton { get; private set; }
    public Label? WaterValueLabel { get; private set; }
    public Label? BrushLabel { get; private set; }
    public HSlider? BrushHSlider { get; private set; }
    public OptionButton? RiverOptionButton { get; private set; }
    public OptionButton? RoadOptionButton { get; private set; }
    public CheckButton? UrbanCheckButton { get; private set; }
    public HSlider? UrbanHSlider { get; private set; }
    public CheckButton? FarmCheckButton { get; private set; }
    public HSlider? FarmHSlider { get; private set; }
    public CheckButton? PlantCheckButton { get; private set; }
    public HSlider? PlantHSlider { get; private set; }
    public OptionButton? WallOptionButton { get; private set; }
    public CheckButton? SpecialFeatureCheckButton { get; private set; }
    public OptionButton? SpecialFeatureOptionButton { get; private set; }

    private void InitOnReadyNodes()
    {
        SubViewportContainer = GetNode<SubViewportContainer>("%SubViewportContainer");
        WireframeCheckButton = GetNode<CheckButton>("%WireframeCheckButton");
        CelestialMotionCheckButton = GetNode<CheckButton>("%CelestialMotionCheckButton");
        // 小地图
        MiniMapContainer = GetNode<SubViewportContainer>("%MiniMapContainer");
        CamLatLonLabel = GetNode<Label>("%CamLatLonLabel");
        LatLonFixCheckButton = GetNode<CheckButton>("%LatLonFixCheckButton");
        // 指南针
        CompassPanel = GetNode<PanelContainer>("%CompassPanel");
        // 矩形地图测试
        RectMap = GetNode<TextureRect>("%RectMap");
        // 星球信息
        PlanetTabBar = GetNode<TabBar>("%PlanetTabBar");
        PlanetGrid = GetNode<GridContainer>("%PlanetGrid");
        RadiusLineEdit = GetNode<LineEdit>("%RadiusLineEdit");
        DivisionLineEdit = GetNode<LineEdit>("%DivisionLineEdit");
        ChunkDivisionLineEdit = GetNode<LineEdit>("%ChunkDivisionLineEdit");
        // 地块信息
        TileTabBar = GetNode<TabBar>("%TileTabBar");
        TileVBox = GetNode<VBoxContainer>("%TileVBox");
        ChunkCountLabel = GetNode<Label>("%ChunkCountLabel");
        TileCountLabel = GetNode<Label>("%TileCountLabel");
        ShowLableOptionButton = GetNode<OptionButton>("%ShowLabelOptionButton");
        TileGrid = GetNode<GridContainer>("%TileGrid");
        IdLineEdit = GetNode<LineEdit>("%IdLineEdit");
        ChunkLineEdit = GetNode<LineEdit>("%ChunkLineEdit");
        CoordsLineEdit = GetNode<LineEdit>("%CoordsLineEdit");
        HeightLineEdit = GetNode<LineEdit>("%HeightLineEdit");
        ElevationLineEdit = GetNode<LineEdit>("%ElevationLineEdit");
        LonLineEdit = GetNode<LineEdit>("%LonLineEdit");
        LatLineEdit = GetNode<LineEdit>("%LatLineEdit");
        // 编辑功能
        EditCheckButton = GetNode<CheckButton>("%EditCheckButton");
        EditTabBar = GetNode<TabBar>("%EditTabBar");
        EditGrid = GetNode<GridContainer>("%EditGrid");
        TerrainOptionButton = GetNode<OptionButton>("%TerrainOptionButton");
        ElevationVSlider = GetNode<VSlider>("%ElevationVSlider");
        ElevationCheckButton = GetNode<CheckButton>("%ElevationCheckButton");
        ElevationValueLabel = GetNode<Label>("%ElevationValueLabel");
        WaterVSlider = GetNode<VSlider>("%WaterVSlider");
        WaterCheckButton = GetNode<CheckButton>("%WaterCheckButton");
        WaterValueLabel = GetNode<Label>("%WaterValueLabel");
        BrushLabel = GetNode<Label>("%BrushLabel");
        BrushHSlider = GetNode<HSlider>("%BrushHSlider");
        RiverOptionButton = GetNode<OptionButton>("%RiverOptionButton");
        RoadOptionButton = GetNode<OptionButton>("%RoadOptionButton");
        UrbanCheckButton = GetNode<CheckButton>("%UrbanCheckButton");
        UrbanHSlider = GetNode<HSlider>("%UrbanHSlider");
        FarmCheckButton = GetNode<CheckButton>("%FarmCheckButton");
        FarmHSlider = GetNode<HSlider>("%FarmHSlider");
        PlantCheckButton = GetNode<CheckButton>("%PlantCheckButton");
        PlantHSlider = GetNode<HSlider>("%PlantHSlider");
        WallOptionButton = GetNode<OptionButton>("%WallOptionButton");
        SpecialFeatureCheckButton = GetNode<CheckButton>("%SpecialFeatureCheckButton");
        SpecialFeatureOptionButton = GetNode<OptionButton>("%SpecialFeatureOptionButton");
    }

    #endregion

    private Tile? _chosenTile;

    public Tile? ChosenTile
    {
        get => _chosenTile;
        set
        {
            _chosenTile = value;
            ChosenTileChanged?.Invoke(value);
        }
    }

    public bool IsDrag { get; set; }
    public Tile? DragTile { get; set; }
    public Tile? PreviousTile { get; set; }

    private int _labelMode;

    public int LabelMode { get; set; }
    public HexTileDataOverrider TileOverrider { get; set; }
}