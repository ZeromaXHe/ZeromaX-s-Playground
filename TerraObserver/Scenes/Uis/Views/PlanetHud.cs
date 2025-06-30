using System;
using Godot;
using TO.Domains.Functions.Shaders;
using TO.Domains.Types.PlanetHuds;

namespace TerraObserver.Scenes.Uis.Views;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-05 19:08:02
public partial class PlanetHud : Control, IPlanetHud
{
    #region 事件

    public event Action<int?>? ChosenTileIdChanged;
    public event Action<bool>? CelestialMotionCheckButtonToggled;
    public event Action<bool>? LonLatFixCheckButtonToggled;
    public event Action<string>? RadiusLineEditTextSubmitted;
    public event Action<string>? DivisionLineEditTextSubmitted;
    public event Action<string>? ChunkDivisionLineEditTextSubmitted;

    #endregion

    #region 普通属性

    private int? _chosenTileId;

    public int? ChosenTileId
    {
        get => _chosenTileId;
        set
        {
            _chosenTileId = value;
            ChosenTileIdChanged?.Invoke(value);
        }
    }

    public bool IsDrag { get; set; }
    public int? DragTileId { get; set; }
    public int? PreviousTileId { get; set; }
    public int LabelMode { get; set; }
    public HexTileDataOverrider TileOverrider { get; set; } = new();

    #endregion

    #region on-ready

    private CheckButton WireframeCheckButton { get; set; } = null!;
    private CheckButton CelestialMotionCheckButton { get; set; } = null!;

    // 小地图
    private SubViewportContainer MiniMapContainer { get; set; } = null!;
    public Label CamLonLatLabel { get; private set; } = null!;
    private CheckButton LonLatFixCheckButton { get; set; } = null!;

    // 指南针
    public PanelContainer CompassPanel { get; private set; } = null!;

    // 矩形地图测试
    public TextureRect RectMap { get; private set; } = null!;

    // 星球信息
    private TabBar PlanetTabBar { get; set; } = null!;
    private GridContainer PlanetGrid { get; set; } = null!;
    public LineEdit RadiusLineEdit { get;  private set; } = null!;
    public LineEdit DivisionLineEdit { get;  private set; } = null!;
    public LineEdit ChunkDivisionLineEdit { get;  private set; } = null!;

    // 地块信息
    private TabBar TileTabBar { get; set; } = null!;
    private VBoxContainer TileVBox { get; set; } = null!;
    private Label ChunkCountLabel { get; set; } = null!;
    private Label TileCountLabel { get; set; } = null!;
    private OptionButton ShowLableOptionButton { get; set; } = null!;
    private GridContainer TileGrid { get; set; } = null!;
    private LineEdit IdLineEdit { get; set; } = null!;
    private LineEdit ChunkLineEdit { get; set; } = null!;
    private LineEdit CoordsLineEdit { get; set; } = null!;
    private LineEdit HeightLineEdit { get; set; } = null!;
    private LineEdit ElevationLineEdit { get; set; } = null!;
    private LineEdit LonLineEdit { get; set; } = null!;
    private LineEdit LatLineEdit { get; set; } = null!;

    // 编辑功能
    private CheckButton EditCheckButton { get; set; } = null!;
    private TabBar EditTabBar { get; set; } = null!;
    private GridContainer EditGrid { get; set; } = null!;
    private OptionButton TerrainOptionButton { get; set; } = null!;
    public VSlider ElevationVSlider { get; private set; } = null!;
    private CheckButton ElevationCheckButton { get; set; } = null!;
    private Label ElevationValueLabel { get; set; } = null!;
    public VSlider WaterVSlider { get; private set; } = null!;
    private CheckButton WaterCheckButton { get; set; } = null!;
    private Label WaterValueLabel { get; set; } = null!;
    private Label BrushLabel { get; set; } = null!;
    private HSlider BrushHSlider { get; set; } = null!;
    private OptionButton RiverOptionButton { get; set; } = null!;
    private OptionButton RoadOptionButton { get; set; } = null!;
    private CheckButton UrbanCheckButton { get; set; } = null!;
    private HSlider UrbanHSlider { get; set; } = null!;
    private CheckButton FarmCheckButton { get; set; } = null!;
    private HSlider FarmHSlider { get; set; } = null!;
    private CheckButton PlantCheckButton { get; set; } = null!;
    private HSlider PlantHSlider { get; set; } = null!;
    private OptionButton WallOptionButton { get; set; } = null!;
    private CheckButton SpecialFeatureCheckButton { get; set; } = null!;
    private OptionButton SpecialFeatureOptionButton { get; set; } = null!;

    #endregion

    #region 生命周期

    public override void _Ready()
    {
        WireframeCheckButton = GetNode<CheckButton>("%WireframeCheckButton");
        CelestialMotionCheckButton = GetNode<CheckButton>("%CelestialMotionCheckButton");
        // 小地图
        MiniMapContainer = GetNode<SubViewportContainer>("%MiniMapContainer");
        CamLonLatLabel = GetNode<Label>("%CamLonLatLabel");
        LonLatFixCheckButton = GetNode<CheckButton>("%LonLatFixCheckButton");
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

        SetEditMode(EditCheckButton.ButtonPressed);
        SetLabelMode(ShowLableOptionButton.Selected);
        SetTerrain(0);

        WireframeCheckButton.Toggled += toggle =>
            GetViewport().SetDebugDraw(toggle ? Viewport.DebugDrawEnum.Wireframe : Viewport.DebugDrawEnum.Disabled);
        base._Ready();
        CelestialMotionCheckButton.Toggled += toggle => CelestialMotionCheckButtonToggled?.Invoke(toggle);
        LonLatFixCheckButton.Toggled += toggle => LonLatFixCheckButtonToggled?.Invoke(toggle); // 锁定经纬网的显示
        PlanetTabBar.TabClicked += _ => PlanetGrid.Visible = !PlanetGrid.Visible;
        RadiusLineEdit.TextSubmitted += text => RadiusLineEditTextSubmitted?.Invoke(text);
        DivisionLineEdit.TextSubmitted += text => DivisionLineEditTextSubmitted?.Invoke(text);
        ChunkDivisionLineEdit.TextSubmitted += text => ChunkDivisionLineEditTextSubmitted?.Invoke(text);

        TileTabBar.TabClicked += _ =>
        {
            var vis = !TileVBox.Visible;
            TileVBox.Visible = vis;
            TileGrid.Visible = vis;
        };

        EditTabBar.TabClicked += _ =>
        {
            var vis = !EditGrid.Visible;
            EditGrid.Visible = vis;
            if (vis)
            {
                SetTerrain(TerrainOptionButton.Selected);
                SetElevation(ElevationVSlider.Value);
            }
            else
            {
                SetApplyTerrain(false);
                SetApplyElevation(false);
            }
        };
        EditCheckButton.Toggled += SetEditMode;
        ShowLableOptionButton.ItemSelected += SetLabelMode;
        TerrainOptionButton.ItemSelected += SetTerrain;
        ElevationVSlider.ValueChanged += SetElevation;
        ElevationCheckButton.Toggled += SetApplyElevation;
        WaterVSlider.ValueChanged += SetWaterLevel;
        WaterCheckButton.Toggled += SetApplyWaterLevel;
        BrushHSlider.ValueChanged += SetBrushSize;
        RiverOptionButton.ItemSelected += SetRiverMode;
        RoadOptionButton.ItemSelected += SetRoadMode;
        UrbanCheckButton.Toggled += SetApplyUrbanLevel;
        UrbanHSlider.ValueChanged += SetUrbanLevel;
        FarmCheckButton.Toggled += SetApplyFarmLevel;
        FarmHSlider.ValueChanged += SetFarmLevel;
        PlantCheckButton.Toggled += SetApplyPlantLevel;
        PlantHSlider.ValueChanged += SetPlantLevel;
        WallOptionButton.ItemSelected += SetWalledMode;
        SpecialFeatureCheckButton.Toggled += SetApplySpecialIndex;
        SpecialFeatureOptionButton.ItemSelected += SetSpecialIndex;
    }

    #endregion

    private void SetEditMode(bool toggle)
    {
        var editMode = TileOverrider.EditMode;
        TileOverrider.EditMode = toggle;
        // if (toggle != editMode)
        //     EditModeChanged?.Invoke(toggle);
        RenderingServer.GlobalShaderParameterSet(GlobalShaderParam.HexMapEditMode, toggle);
    }

    private void SetLabelMode(long mode)
    {
        var before = LabelMode;
        var intMode = (int)mode;
        LabelMode = intMode;
        // if (before != intMode)
        //     LabelModeChanged?.Invoke(intMode);
    }

    private void SetTerrain(long index)
    {
        TileOverrider.ApplyTerrain = index > 0;
        if (TileOverrider.ApplyTerrain)
            TileOverrider.ActiveTerrain = (int)index - 1;
    }

    private void SetApplyTerrain(bool toggle) => TileOverrider.ApplyTerrain = toggle;

    private void SetElevation(double elevation)
    {
        TileOverrider.ActiveElevation = (int)elevation;
        ElevationValueLabel.Text = TileOverrider.ActiveElevation.ToString();
    }

    private void SetApplyElevation(bool toggle) => TileOverrider.ApplyElevation = toggle;

    private void SetBrushSize(double brushSize)
    {
        TileOverrider.BrushSize = (int)brushSize;
        BrushLabel.Text = $"笔刷大小：{TileOverrider.BrushSize}";
    }

    private void SetRiverMode(long mode) => TileOverrider.RiverMode = (OptionalToggle)mode;
    private void SetRoadMode(long mode) => TileOverrider.RoadMode = (OptionalToggle)mode;
    private void SetApplyWaterLevel(bool toggle) => TileOverrider.ApplyWaterLevel = toggle;

    private void SetWaterLevel(double level)
    {
        TileOverrider.ActiveWaterLevel = (int)level;
        WaterValueLabel.Text = TileOverrider.ActiveWaterLevel.ToString();
    }

    private void SetApplyUrbanLevel(bool toggle) => TileOverrider.ApplyUrbanLevel = toggle;
    private void SetUrbanLevel(double level) => TileOverrider.ActiveUrbanLevel = (int)level;
    private void SetApplyFarmLevel(bool toggle) => TileOverrider.ApplyFarmLevel = toggle;
    private void SetFarmLevel(double level) => TileOverrider.ActiveFarmLevel = (int)level;
    private void SetApplyPlantLevel(bool toggle) => TileOverrider.ApplyPlantLevel = toggle;
    private void SetPlantLevel(double level) => TileOverrider.ActivePlantLevel = (int)level;
    private void SetWalledMode(long mode) => TileOverrider.WalledMode = (OptionalToggle)mode;
    private void SetApplySpecialIndex(bool toggle) => TileOverrider.ApplySpecialIndex = toggle;
    private void SetSpecialIndex(long index) => TileOverrider.ActiveSpecialIndex = (int)index;
}