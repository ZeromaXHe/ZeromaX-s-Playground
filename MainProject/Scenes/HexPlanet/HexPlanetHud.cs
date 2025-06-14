using Apps.Applications.Uis;
using Apps.Events;
using Commons.Utils;
using Commons.Utils.HexSphereGrid;
using Domains.Models.Entities.PlanetGenerates;
using Domains.Services.Uis;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Dependency;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Nodes;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-02-17 15:49
public partial class HexPlanetHud : Control
{
    public HexPlanetHud() => InitApps();

    [Export] private HexPlanetManager _hexPlanetManager;

    #region on-ready 节点

    private SubViewportContainer _subViewportContainer;

    private CheckButton _wireframeCheckButton;
    private CheckButton _celestialMotionCheckButton;

    // 小地图
    private SubViewportContainer _miniMapContainer;
    private MiniMapManager _miniMapManager;
    private Label _camLatLonLabel;
    private CheckButton _latLonFixCheckButton;

    // 指南针
    private PanelContainer _compassPanel;

    // 矩形地图测试
    private TextureRect _rectMap;

    // 星球信息
    private TabBar _planetTabBar;
    private GridContainer _planetGrid;
    private LineEdit _radiusLineEdit;
    private LineEdit _divisionLineEdit;
    private LineEdit _chunkDivisionLineEdit;

    // 地块信息
    private TabBar _tileTabBar;
    private VBoxContainer _tileVBox;
    private Label _chunkCountLabel;
    private Label _tileCountLabel;
    private OptionButton _showLableOptionButton;
    private GridContainer _tileGrid;
    private LineEdit _idLineEdit;
    private LineEdit _chunkLineEdit;
    private LineEdit _coordsLineEdit;
    private LineEdit _heightLineEdit;
    private LineEdit _elevationLineEdit;
    private LineEdit _lonLineEdit;
    private LineEdit _latLineEdit;

    // 编辑功能
    private CheckButton _editCheckButton;
    private TabBar _editTabBar;
    private GridContainer _editGrid;
    private OptionButton _terrainOptionButton;
    private VSlider _elevationVSlider;
    private CheckButton _elevationCheckButton;
    private Label _elevationValueLabel;
    private VSlider _waterVSlider;
    private CheckButton _waterCheckButton;
    private Label _waterValueLabel;
    private Label _brushLabel;
    private HSlider _brushHSlider;
    private OptionButton _riverOptionButton;
    private OptionButton _roadOptionButton;
    private CheckButton _urbanCheckButton;
    private HSlider _urbanHSlider;
    private CheckButton _farmCheckButton;
    private HSlider _farmHSlider;
    private CheckButton _plantCheckButton;
    private HSlider _plantHSlider;
    private OptionButton _wallOptionButton;
    private CheckButton _specialFeatureCheckButton;
    private OptionButton _specialFeatureOptionButton;

    private void InitOnReadyNodes()
    {
        _subViewportContainer = GetNode<SubViewportContainer>("%SubViewportContainer");
        _wireframeCheckButton = GetNode<CheckButton>("%WireframeCheckButton");
        _celestialMotionCheckButton = GetNode<CheckButton>("%CelestialMotionCheckButton");
        // 小地图
        _miniMapContainer = GetNode<SubViewportContainer>("%MiniMapContainer");
        _miniMapManager = GetNode<MiniMapManager>("%MiniMapManager");
        _camLatLonLabel = GetNode<Label>("%CamLatLonLabel");
        _latLonFixCheckButton = GetNode<CheckButton>("%LatLonFixCheckButton");
        // 指南针
        _compassPanel = GetNode<PanelContainer>("%CompassPanel");
        // 矩形地图测试
        _rectMap = GetNode<TextureRect>("%RectMap");
        _rectMap.Texture = _hexPlanetHudApplication.GenerateRectMap();
        // 星球信息
        _planetTabBar = GetNode<TabBar>("%PlanetTabBar");
        _planetGrid = GetNode<GridContainer>("%PlanetGrid");
        _radiusLineEdit = GetNode<LineEdit>("%RadiusLineEdit");
        _divisionLineEdit = GetNode<LineEdit>("%DivisionLineEdit");
        _chunkDivisionLineEdit = GetNode<LineEdit>("%ChunkDivisionLineEdit");
        // 地块信息
        _tileTabBar = GetNode<TabBar>("%TileTabBar");
        _tileVBox = GetNode<VBoxContainer>("%TileVBox");
        _chunkCountLabel = GetNode<Label>("%ChunkCountLabel");
        _tileCountLabel = GetNode<Label>("%TileCountLabel");
        _showLableOptionButton = GetNode<OptionButton>("%ShowLabelOptionButton");
        _tileGrid = GetNode<GridContainer>("%TileGrid");
        _idLineEdit = GetNode<LineEdit>("%IdLineEdit");
        _chunkLineEdit = GetNode<LineEdit>("%ChunkLineEdit");
        _coordsLineEdit = GetNode<LineEdit>("%CoordsLineEdit");
        _heightLineEdit = GetNode<LineEdit>("%HeightLineEdit");
        _elevationLineEdit = GetNode<LineEdit>("%ElevationLineEdit");
        _lonLineEdit = GetNode<LineEdit>("%LonLineEdit");
        _latLineEdit = GetNode<LineEdit>("%LatLineEdit");
        // 编辑功能
        _editCheckButton = GetNode<CheckButton>("%EditCheckButton");
        _editTabBar = GetNode<TabBar>("%EditTabBar");
        _editGrid = GetNode<GridContainer>("%EditGrid");
        _terrainOptionButton = GetNode<OptionButton>("%TerrainOptionButton");
        _elevationVSlider = GetNode<VSlider>("%ElevationVSlider");
        _elevationCheckButton = GetNode<CheckButton>("%ElevationCheckButton");
        _elevationValueLabel = GetNode<Label>("%ElevationValueLabel");
        _waterVSlider = GetNode<VSlider>("%WaterVSlider");
        _waterCheckButton = GetNode<CheckButton>("%WaterCheckButton");
        _waterValueLabel = GetNode<Label>("%WaterValueLabel");
        _brushLabel = GetNode<Label>("%BrushLabel");
        _brushHSlider = GetNode<HSlider>("%BrushHSlider");
        _riverOptionButton = GetNode<OptionButton>("%RiverOptionButton");
        _roadOptionButton = GetNode<OptionButton>("%RoadOptionButton");
        _urbanCheckButton = GetNode<CheckButton>("%UrbanCheckButton");
        _urbanHSlider = GetNode<HSlider>("%UrbanHSlider");
        _farmCheckButton = GetNode<CheckButton>("%FarmCheckButton");
        _farmHSlider = GetNode<HSlider>("%FarmHSlider");
        _plantCheckButton = GetNode<CheckButton>("%PlantCheckButton");
        _plantHSlider = GetNode<HSlider>("%PlantHSlider");
        _wallOptionButton = GetNode<OptionButton>("%WallOptionButton");
        _specialFeatureCheckButton = GetNode<CheckButton>("%SpecialFeatureCheckButton");
        _specialFeatureOptionButton = GetNode<OptionButton>("%SpecialFeatureOptionButton");

        // 按照指定的高程分割数量确定 UI
        _elevationVSlider.MaxValue = _hexPlanetHudApplication.GetElevationStep();
        _elevationVSlider.TickCount = _hexPlanetHudApplication.GetElevationStep() + 1;
        _waterVSlider.MaxValue = _hexPlanetHudApplication.GetElevationStep();
        _waterVSlider.TickCount = _hexPlanetHudApplication.GetElevationStep() + 1;

        _hexPlanetManager.NewPlanetGenerated += UpdateNewPlanetInfo;
        _hexPlanetManager.NewPlanetGenerated += InitMiniMap;
        OrbitCameraEvent.Instance.Moved += OnCameraMoved;
        OrbitCameraEvent.Instance.Transformed += OnCameraTransformed;
    }

    private void InitMiniMap()
    {
        _miniMapManager.Init(_hexPlanetManager.GetOrbitCameraFocusPos());
        _rectMap.Texture = _hexPlanetHudApplication.GenerateRectMap();
    }

    private void UpdateNewPlanetInfo()
    {
        UpdatePlanetUi();
        ChosenTile = null;
    }

    private void OnCameraMoved(Vector3 pos, float delta)
    {
        var longLat = LongitudeLatitudeCoords.From(_hexPlanetManager.ToPlanetLocal(pos));
        _camLatLonLabel.Text = $"相机位置：{longLat}";
    }

    private void OnCameraTransformed(Transform3D transform, float delta)
    {
        var northPolePoint = Vector3.Up;
        var posNormal = transform.Origin.Normalized();
        var dirNorth = Math3dUtil.DirectionBetweenPointsOnSphere(posNormal, northPolePoint);
        var angleToNorth = transform.Basis.Y.Slide(posNormal).SignedAngleTo(dirNorth, -posNormal);
        _compassPanel.Rotation = angleToNorth;

        var posLocal = _hexPlanetManager.ToPlanetLocal(_hexPlanetManager.GetOrbitCameraFocusPos());
        var longLat = LongitudeLatitudeCoords.From(posLocal);
        var rectMapMaterial = _rectMap.Material as ShaderMaterial;
        rectMapMaterial?.SetShaderParameter("lon", longLat.Longitude);
        rectMapMaterial?.SetShaderParameter("lat", longLat.Latitude);
        // rectMapMaterial?.SetShaderParameter("pos_normal", posLocal.Normalized()); // 非常奇怪，旋转时会改变……
        rectMapMaterial?.SetShaderParameter("angle_to_north", angleToNorth);
        // GD.Print($"lonLat: {longLat.Longitude}, {longLat.Latitude}; angleToNorth: {
        //     angleToNorth}; posNormal: {posNormal};");
    }

    private void CleanNodeEventListeners()
    {
        _hexPlanetManager.NewPlanetGenerated -= UpdateNewPlanetInfo;
        _hexPlanetManager.NewPlanetGenerated -= InitMiniMap;
        OrbitCameraEvent.Instance.Moved -= OnCameraMoved;
        OrbitCameraEvent.Instance.Transformed -= OnCameraTransformed;
    }

    #endregion

    #region 应用服务

    private IHexPlanetHudApplication _hexPlanetHudApplication;

    private void InitApps()
    {
        _hexPlanetHudApplication = Context.GetBeanFromHolder<IHexPlanetHudApplication>();
    }

    #endregion

    #region 编辑功能

    private void SetElevation(double elevation)
    {
        _hexPlanetHudApplication.SetElevation(elevation);
        _elevationValueLabel.Text = _hexPlanetHudApplication.GetActiveElevation().ToString();
    }

    private void SetBrushSize(double brushSize)
    {
        _hexPlanetHudApplication.SetBrushSize(brushSize);
        _brushLabel.Text = $"笔刷大小：{_hexPlanetHudApplication.GetBrushSize()}";
    }

    private void SetWaterLevel(double level)
    {
        _hexPlanetHudApplication.SetWaterLevel(level);
        _waterValueLabel.Text = _hexPlanetHudApplication.GetActiveWaterLevel().ToString();
    }

    #endregion

    private Tile _chosenTile;

    private Tile ChosenTile
    {
        get => _chosenTile;
        set
        {
            _chosenTile = value;
            if (_chosenTile != null)
            {
                _idLineEdit.Text = _chosenTile.Id.ToString();
                _chunkLineEdit.Text = _chosenTile.ChunkId.ToString();
                var tileInfoResp = _hexPlanetHudApplication.GetTileInfo(_chosenTile);
                _coordsLineEdit.Text = tileInfoResp.SphereAxial.ToString();
                _coordsLineEdit.TooltipText = _coordsLineEdit.Text;
                _heightLineEdit.Text = $"{tileInfoResp.Height:F4}";
                _elevationLineEdit.Text = _chosenTile.Data.Elevation.ToString();
                var lonLat = tileInfoResp.SphereAxial.ToLongitudeAndLatitude();
                _lonLineEdit.Text = lonLat.GetLongitudeString();
                _lonLineEdit.TooltipText = _lonLineEdit.Text;
                _latLineEdit.Text = lonLat.GetLatitudeString();
                _latLineEdit.TooltipText = _latLineEdit.Text;
            }
            else
            {
                _idLineEdit.Text = "-";
                _chunkLineEdit.Text = "-";
                _coordsLineEdit.Text = "-";
                _coordsLineEdit.TooltipText = null;
                _heightLineEdit.Text = "-";
                _elevationLineEdit.Text = "-";
                _lonLineEdit.Text = "-";
                _lonLineEdit.TooltipText = ""; // 试了一下，null 和 "" 效果一样
                _latLineEdit.Text = "-";
                _latLineEdit.TooltipText = null;
            }
        }
    }

    private bool _isDrag;
    private Tile _dragTile;
    private Tile _previousTile;

    public override void _Ready()
    {
        InitOnReadyNodes();
        // 初始化相机位置相关功能
        OnCameraMoved(_hexPlanetManager.GetOrbitCameraFocusPos(), 0f);
        OnCameraTransformed(_hexPlanetManager.GetViewport().GetCamera3D().GetGlobalTransform(), 0f);

        _hexPlanetHudApplication.SetEditMode(_editCheckButton.ButtonPressed);
        _hexPlanetHudApplication.SetLabelMode(_showLableOptionButton.Selected);
        _hexPlanetHudApplication.SetTerrain(0);
        UpdateNewPlanetInfo();
        InitSignals();

        _miniMapManager.Init(_hexPlanetManager.GetOrbitCameraFocusPos());
    }

    public override void _ExitTree() => CleanNodeEventListeners();

    private void InitSignals()
    {
        _wireframeCheckButton.Toggled += toggle =>
            _hexPlanetManager.GetViewport()
                .SetDebugDraw(toggle ? Viewport.DebugDrawEnum.Wireframe : Viewport.DebugDrawEnum.Disabled);

        _celestialMotionCheckButton.Toggled += toggle =>
            _hexPlanetManager.PlanetRevolution = _hexPlanetManager.PlanetRotation =
                _hexPlanetManager.SatelliteRevolution = _hexPlanetManager.SatelliteRotation = toggle;
        _latLonFixCheckButton.Toggled += _hexPlanetManager.FixLatLon;
        _planetTabBar.TabClicked += _ => _planetGrid.Visible = !_planetGrid.Visible;

        _radiusLineEdit.TextSubmitted += text =>
        {
            if (float.TryParse(text, out var radius))
                _hexPlanetManager.Radius = radius;
            else
                _radiusLineEdit.Text = $"{_hexPlanetManager.Radius:F2}";
        };

        _divisionLineEdit.TextSubmitted += text =>
        {
            if (int.TryParse(text, out var division))
                _hexPlanetManager.Divisions = division;
            else
                _divisionLineEdit.Text = $"{_hexPlanetManager.Divisions}";
        };

        _chunkDivisionLineEdit.TextSubmitted += text =>
        {
            if (int.TryParse(text, out var chunkDivision))
                _hexPlanetManager.ChunkDivisions = chunkDivision;
            else
                _divisionLineEdit.Text = $"{_hexPlanetManager.ChunkDivisions}";
        };

        _tileTabBar.TabClicked += _ =>
        {
            var vis = !_tileVBox.Visible;
            _tileVBox.Visible = vis;
            _tileGrid.Visible = vis;
        };

        _editTabBar.TabClicked += _ =>
        {
            var vis = !_editGrid.Visible;
            _editGrid.Visible = vis;
            if (vis)
            {
                _hexPlanetHudApplication.SetTerrain(_terrainOptionButton.Selected);
                SetElevation(_elevationVSlider.Value);
            }
            else
            {
                _hexPlanetHudApplication.SetApplyTerrain(false);
                _hexPlanetHudApplication.SetApplyElevation(false);
            }
        };

        _editCheckButton.Toggled += _hexPlanetHudApplication.SetEditMode;
        _showLableOptionButton.ItemSelected += _hexPlanetHudApplication.SetLabelMode;
        _terrainOptionButton.ItemSelected += _hexPlanetHudApplication.SetTerrain;
        _elevationVSlider.ValueChanged += SetElevation;
        _elevationCheckButton.Toggled += _hexPlanetHudApplication.SetApplyElevation;
        _waterVSlider.ValueChanged += SetWaterLevel;
        _waterCheckButton.Toggled += _hexPlanetHudApplication.SetApplyWaterLevel;
        _brushHSlider.ValueChanged += SetBrushSize;
        _riverOptionButton.ItemSelected += _hexPlanetHudApplication.SetRiverMode;
        _roadOptionButton.ItemSelected += _hexPlanetHudApplication.SetRoadMode;
        _urbanCheckButton.Toggled += _hexPlanetHudApplication.SetApplyUrbanLevel;
        _urbanHSlider.ValueChanged += _hexPlanetHudApplication.SetUrbanLevel;
        _farmCheckButton.Toggled += _hexPlanetHudApplication.SetApplyFarmLevel;
        _farmHSlider.ValueChanged += _hexPlanetHudApplication.SetFarmLevel;
        _plantCheckButton.Toggled += _hexPlanetHudApplication.SetApplyPlantLevel;
        _plantHSlider.ValueChanged += _hexPlanetHudApplication.SetPlantLevel;
        _wallOptionButton.ItemSelected += _hexPlanetHudApplication.SetWalledMode;
        _specialFeatureCheckButton.Toggled += _hexPlanetHudApplication.SetApplySpecialIndex;
        _specialFeatureOptionButton.ItemSelected += _hexPlanetHudApplication.SetSpecialIndex;
    }


    private void UpdatePlanetUi()
    {
        var planetInfoResp = _hexPlanetHudApplication.GetPlanetInfo();
        _radiusLineEdit.Text = $"{planetInfoResp.Radius:F2}";
        _divisionLineEdit.Text = $"{planetInfoResp.Divisions}";
        _chunkDivisionLineEdit.Text = $"{planetInfoResp.ChunkDivisions}";
        _chunkCountLabel.Text = $"分块总数：{planetInfoResp.ChunkCount}";
        _tileCountLabel.Text = $"地块总数：{planetInfoResp.TileCount}";
    }

    public override void _Process(double delta)
    {
        if (GetViewport().GuiGetHoveredControl() == _subViewportContainer)
        {
            if (Input.IsMouseButtonPressed(MouseButton.Left))
            {
                HandleInput();
                return;
            }

            if (_hexPlanetManager.UpdateUiInEditMode())
                return;
        }
        else if (GetViewport().GuiGetHoveredControl() == _miniMapContainer
                 && Input.IsMouseButtonPressed(MouseButton.Left))
        {
            _miniMapManager.ClickOnMiniMap();
        }

        _previousTile = null;
    }

    private void HandleInput()
    {
        // 在 SubViewportContainer 上按下鼠标左键时，获取鼠标位置地块并更新
        ChosenTile = _hexPlanetManager.GetTileUnderCursor();
        if (ChosenTile != null)
        {
            if (_previousTile != null && _previousTile != ChosenTile)
                ValidateDrag(ChosenTile);
            else
                _isDrag = false;
            if (_hexPlanetHudApplication.GetEditMode())
            {
                _hexPlanetHudApplication.EditTiles(ChosenTile, _isDrag, _previousTile, _dragTile);
                ChosenTile = _chosenTile; // 刷新 GUI 地块信息
                // 编辑模式下绘制选择地块框
                _hexPlanetManager.SelectEditingTile(ChosenTile);
            }
            else if (Input.IsActionJustPressed("choose_unit"))
                _hexPlanetManager.FindPath(ChosenTile);

            _previousTile = ChosenTile;
        }
        else
        {
            if (!_hexPlanetHudApplication.GetEditMode())
                _hexPlanetManager.FindPath(ChosenTile);
            else
                // 清理选择地块框
                _hexPlanetManager.CleanEditingTile();
            _previousTile = null;
        }
    }

    private void ValidateDrag(Tile currentTile)
    {
        _dragTile = currentTile;
        _isDrag = currentTile.IsNeighbor(_previousTile);
    }
}