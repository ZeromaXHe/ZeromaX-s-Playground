using System;
using Godot;
using Godot.Abstractions.Extensions.Cameras;
using Godot.Abstractions.Extensions.Geos;
using Godot.Abstractions.Extensions.Planets;
using TO.FSharp.Commons.Structs.HexSphereGrid;
using TO.FSharp.Commons.Utils;

namespace TerraObserver.Scenes.Uis.Views;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-05 19:08:02
public partial class PlanetHud : Control
{
    #region 依赖

    public IPlanet Planet { get; set; } = null!;

    public IOrbitCameraRig OrbitCameraRig
    {
        get => _orbitCameraRig;
        set
        {
            _orbitCameraRig = value;
            _orbitCameraRig.Transformed += OnOrbitCameraRigTransformed;
            _orbitCameraRig.Moved += OnOrbitCameraRigMoved;
            OnOrbitCameraRigMoved(_orbitCameraRig.GetFocusBasePos(), 0f);
        }
    }

    private IOrbitCameraRig _orbitCameraRig = null!;

    public ILonLatGrid LonLatGrid { get; set; } = null!;
    public ICelestialMotion CelestialMotion { get; set; } = null!;

    #endregion

    #region 事件和 Export 属性

    public event Action<int?>? ChosenTileIdChanged;

    #endregion

    #region 内部属性、变量

    private CheckButton? WireframeCheckButton { get; set; }
    private CheckButton? CelestialMotionCheckButton { get; set; }

    // 小地图
    private SubViewportContainer? MiniMapContainer { get; set; }
    private Label? CamLatLonLabel { get; set; }
    private CheckButton? LatLonFixCheckButton { get; set; }

    // 指南针
    private PanelContainer? CompassPanel { get; set; }

    // 矩形地图测试
    private TextureRect? RectMap { get; set; }

    // 星球信息
    private TabBar? PlanetTabBar { get; set; }
    private GridContainer? PlanetGrid { get; set; }
    private LineEdit? RadiusLineEdit { get; set; }
    private LineEdit? DivisionLineEdit { get; set; }
    private LineEdit? ChunkDivisionLineEdit { get; set; }

    // 地块信息
    private TabBar? TileTabBar { get; set; }
    private VBoxContainer? TileVBox { get; set; }
    private Label? ChunkCountLabel { get; set; }
    private Label? TileCountLabel { get; set; }
    private OptionButton? ShowLableOptionButton { get; set; }
    private GridContainer? TileGrid { get; set; }
    private LineEdit? IdLineEdit { get; set; }
    private LineEdit? ChunkLineEdit { get; set; }
    private LineEdit? CoordsLineEdit { get; set; }
    private LineEdit? HeightLineEdit { get; set; }
    private LineEdit? ElevationLineEdit { get; set; }
    private LineEdit? LonLineEdit { get; set; }
    private LineEdit? LatLineEdit { get; set; }

    // 编辑功能
    private CheckButton? EditCheckButton { get; set; }
    private TabBar? EditTabBar { get; set; }
    private GridContainer? EditGrid { get; set; }
    private OptionButton? TerrainOptionButton { get; set; }
    private VSlider? ElevationVSlider { get; set; }
    private CheckButton? ElevationCheckButton { get; set; }
    private Label? ElevationValueLabel { get; set; }
    private VSlider? WaterVSlider { get; set; }
    private CheckButton? WaterCheckButton { get; set; }
    private Label? WaterValueLabel { get; set; }
    private Label? BrushLabel { get; set; }
    private HSlider? BrushHSlider { get; set; }
    private OptionButton? RiverOptionButton { get; set; }
    private OptionButton? RoadOptionButton { get; set; }
    private CheckButton? UrbanCheckButton { get; set; }
    private HSlider? UrbanHSlider { get; set; }
    private CheckButton? FarmCheckButton { get; set; }
    private HSlider? FarmHSlider { get; set; }
    private CheckButton? PlantCheckButton { get; set; }
    private HSlider? PlantHSlider { get; set; }
    private OptionButton? WallOptionButton { get; set; }
    private CheckButton? SpecialFeatureCheckButton { get; set; }
    private OptionButton? SpecialFeatureOptionButton { get; set; }

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

    #endregion

    #region 生命周期

    public override void _Ready()
    {
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

        WireframeCheckButton!.Toggled += toggle =>
            Planet.GetViewport()
                .SetDebugDraw(toggle ? Viewport.DebugDrawEnum.Wireframe : Viewport.DebugDrawEnum.Disabled);
        CelestialMotionCheckButton!.Toggled += toggle =>
            CelestialMotion.PlanetRevolution = CelestialMotion.PlanetRotation =
                CelestialMotion.SatelliteRevolution = CelestialMotion.SatelliteRotation = toggle;
        LatLonFixCheckButton!.Toggled += toggle => LonLatGrid.FixFullVisibility = toggle; // 锁定经纬网的显示
        PlanetTabBar!.TabClicked += _ => PlanetGrid!.Visible = !PlanetGrid.Visible;
        RadiusLineEdit!.TextSubmitted += text =>
        {
            if (float.TryParse(text, out var radius))
                Planet.Radius = radius;
            else
                RadiusLineEdit.Text = $"{Planet.Radius:F2}";
        };

        DivisionLineEdit!.TextSubmitted += text =>
        {
            if (int.TryParse(text, out var division))
                Planet.Divisions = division;
            else
                DivisionLineEdit.Text = $"{Planet.Divisions}";
        };

        ChunkDivisionLineEdit!.TextSubmitted += text =>
        {
            if (int.TryParse(text, out var chunkDivision))
                Planet.ChunkDivisions = chunkDivision;
            else
                DivisionLineEdit.Text = $"{Planet.ChunkDivisions}";
        };

        TileTabBar!.TabClicked += _ =>
        {
            var vis = !TileVBox!.Visible;
            TileVBox.Visible = vis;
            TileGrid!.Visible = vis;
        };
    }

    #endregion


    private void OnOrbitCameraRigMoved(Vector3 pos, float _)
    {
        var longLat = LonLatCoords.From(pos);
        CamLatLonLabel!.Text = $"相机位置：{longLat}";
    }

    private void OnOrbitCameraRigTransformed(Transform3D transform, float _)
    {
        var northPolePoint = Vector3.Up;
        var posNormal = transform.Origin.Normalized();
        var dirNorth = Math3dUtil.DirectionBetweenPointsOnSphere(posNormal, northPolePoint);
        var angleToNorth = transform.Basis.Y.Slide(posNormal).SignedAngleTo(dirNorth, -posNormal);
        CompassPanel!.Rotation = angleToNorth;

        var posLocal = OrbitCameraRig.GetFocusBasePos();
        var longLat = LonLatCoords.From(posLocal);
        var rectMapMaterial = RectMap!.Material as ShaderMaterial;
        rectMapMaterial?.SetShaderParameter("lon", longLat.Longitude);
        rectMapMaterial?.SetShaderParameter("lat", longLat.Latitude);
        // rectMapMaterial?.SetShaderParameter("pos_normal", posLocal.Normalized()); // 非常奇怪，旋转时会改变……
        rectMapMaterial?.SetShaderParameter("angle_to_north", angleToNorth);
        // GD.Print($"lonLat: {longLat.Longitude}, {longLat.Latitude}; angleToNorth: {
        //     angleToNorth}; posNormal: {posNormal};");
    }
}