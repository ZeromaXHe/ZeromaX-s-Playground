using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Dependency;
using ZeromaXsPlaygroundProject.Scenes.Framework.GlobalNode;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Struct;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util.HexSphereGrid;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet;

public partial class HexPlanetHud : Control
{
    public HexPlanetHud() => InitServices();

    [Export] private HexPlanetManager _hexPlanetManager;

    #region on-ready 节点

    private SubViewportContainer _subViewportContainer;

    private CheckButton _wireframeCheckButton;

    // 小地图
    private SubViewportContainer _miniMapContainer;
    private MiniMapManager _miniMapManager;
    private Label _camLatLonLabel;
    private CheckButton _latLonFixCheckButton;

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
        // 小地图
        _miniMapContainer = GetNode<SubViewportContainer>("%MiniMapContainer");
        _miniMapManager = GetNode<MiniMapManager>("%MiniMapManager");
        _camLatLonLabel = GetNode<Label>("%CamLatLonLabel");
        _latLonFixCheckButton = GetNode<CheckButton>("%LatLonFixCheckButton");
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
        _elevationVSlider.MaxValue = _planetSettingService.ElevationStep;
        _elevationVSlider.TickCount = _planetSettingService.ElevationStep + 1;
        _waterVSlider.MaxValue = _planetSettingService.ElevationStep;
        _waterVSlider.TickCount = _planetSettingService.ElevationStep + 1;

        _hexPlanetManager.NewPlanetGenerated += UpdateNewPlanetInfo;
        _hexPlanetManager.NewPlanetGenerated += InitMiniMap;
        EventBus.Instance.CameraMoved += OnCameraMoved;
    }

    private void InitMiniMap() => _miniMapManager.Init(_hexPlanetManager.GetOrbitCameraFocusPos());

    private void UpdateNewPlanetInfo()
    {
        UpdatePlanetUi();
        ChosenTile = null;
    }

    private void OnCameraMoved(Vector3 pos, float delta) =>
        _camLatLonLabel.Text = $"相机位置：{LongitudeLatitudeCoords.From(pos)}";

    private void CleanNodeEventListeners()
    {
        _hexPlanetManager.NewPlanetGenerated -= UpdateNewPlanetInfo;
        _hexPlanetManager.NewPlanetGenerated -= InitMiniMap;
        EventBus.Instance.CameraMoved -= OnCameraMoved;
    }

    #endregion

    #region services

    private ITileService _tileService;
    private IChunkService _chunkService;
    private ISelectViewService _selectViewService;
    private IPlanetSettingService _planetSettingService;
    private IEditorService _editorService;

    private void InitServices()
    {
        _tileService = Context.GetBean<ITileService>();
        _chunkService = Context.GetBean<IChunkService>();
        _selectViewService = Context.GetBean<ISelectViewService>();
        _planetSettingService = Context.GetBean<IPlanetSettingService>();
        _editorService = Context.GetBean<IEditorService>();
    }

    #endregion

    #region 编辑功能

    private HexTileDataOverrider TileOverrider
    {
        get => _editorService.TileOverrider;
        set => _editorService.TileOverrider = value;
    }

    private int ShowLabelMode
    {
        get => _editorService.LabelMode;
        set => _editorService.LabelMode = value;
    }

    private void SetEditMode(bool toggle)
    {
        TileOverrider = TileOverrider with { EditMode = toggle };
        RenderingServer.GlobalShaderParameterSet("hex_map_edit_mode", toggle);
    }

    private void SetShowLabelMode(long mode) => ShowLabelMode = (int)mode;

    private void SelectTerrain(long index)
    {
        TileOverrider = TileOverrider with { ApplyTerrain = index > 0 };
        if (TileOverrider.ApplyTerrain)
        {
            TileOverrider = TileOverrider with { ActiveTerrain = (int)index - 1 };
        }
    }

    private void SetElevation(double elevation)
    {
        TileOverrider = TileOverrider with { ActiveElevation = (int)elevation };
        _elevationValueLabel.Text = TileOverrider.ActiveElevation.ToString();
    }

    private void SetApplyElevation(bool toggle) => TileOverrider = TileOverrider with { ApplyElevation = toggle };

    private void SetBrushSize(double brushSize)
    {
        TileOverrider = TileOverrider with { BrushSize = (int)brushSize };
        _selectViewService.SelectViewSize = TileOverrider.BrushSize;
        _brushLabel.Text = $"笔刷大小：{TileOverrider.BrushSize}";
    }

    private void SetRiverMode(long mode) => TileOverrider = TileOverrider with { RiverMode = (OptionalToggle)mode };
    private void SetRoadMode(long mode) => TileOverrider = TileOverrider with { RoadMode = (OptionalToggle)mode };
    private void SetApplyWaterLevel(bool toggle) => TileOverrider = TileOverrider with { ApplyWaterLevel = toggle };

    private void SetWaterLevel(double level)
    {
        TileOverrider = TileOverrider with { ActiveWaterLevel = (int)level };
        _waterValueLabel.Text = TileOverrider.ActiveWaterLevel.ToString();
    }

    private void SetApplyUrbanLevel(bool toggle) => TileOverrider = TileOverrider with { ApplyUrbanLevel = toggle };
    private void SetUrbanLevel(double level) => TileOverrider = TileOverrider with { ActiveUrbanLevel = (int)level };
    private void SetApplyFarmLevel(bool toggle) => TileOverrider = TileOverrider with { ApplyFarmLevel = toggle };
    private void SetFarmLevel(double level) => TileOverrider = TileOverrider with { ActiveFarmLevel = (int)level };
    private void SetApplyPlantLevel(bool toggle) => TileOverrider = TileOverrider with { ApplyPlantLevel = toggle };
    private void SetPlantLevel(double level) => TileOverrider = TileOverrider with { ActivePlantLevel = (int)level };
    private void SetWalledMode(long mode) => TileOverrider = TileOverrider with { WalledMode = (OptionalToggle)mode };
    private void SetApplySpecialIndex(bool toggle) => TileOverrider = TileOverrider with { ApplySpecialIndex = toggle };
    private void SetSpecialIndex(long index) => TileOverrider = TileOverrider with { ActiveSpecialIndex = (int)index };

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
                var sa = _tileService.GetSphereAxial(_chosenTile);
                _coordsLineEdit.Text = sa.ToString();
                _coordsLineEdit.TooltipText = _coordsLineEdit.Text;
                _heightLineEdit.Text = $"{_tileService.GetHeight(_chosenTile):F4}";
                _elevationLineEdit.Text = _chosenTile.Data.Elevation.ToString();
                var lonLat = sa.ToLongitudeAndLatitude();
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

        SetEditMode(_editCheckButton.ButtonPressed);
        SetShowLabelMode(_showLableOptionButton.Selected);
        SelectTerrain(0);
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
                SelectTerrain(_terrainOptionButton.Selected);
                SetElevation(_elevationVSlider.Value);
            }
            else
            {
                TileOverrider = TileOverrider with { ApplyTerrain = false };
                TileOverrider = TileOverrider with { ApplyElevation = false };
            }
        };

        _editCheckButton.Toggled += SetEditMode;
        _showLableOptionButton.ItemSelected += SetShowLabelMode;
        _terrainOptionButton.ItemSelected += SelectTerrain;
        _elevationVSlider.ValueChanged += SetElevation;
        _elevationCheckButton.Toggled += SetApplyElevation;
        _waterVSlider.ValueChanged += SetWaterLevel;
        _waterCheckButton.Toggled += SetApplyWaterLevel;
        _brushHSlider.ValueChanged += SetBrushSize;
        _riverOptionButton.ItemSelected += SetRiverMode;
        _roadOptionButton.ItemSelected += SetRoadMode;
        _urbanCheckButton.Toggled += SetApplyUrbanLevel;
        _urbanHSlider.ValueChanged += SetUrbanLevel;
        _farmCheckButton.Toggled += SetApplyFarmLevel;
        _farmHSlider.ValueChanged += SetFarmLevel;
        _plantCheckButton.Toggled += SetApplyPlantLevel;
        _plantHSlider.ValueChanged += SetPlantLevel;
        _wallOptionButton.ItemSelected += SetWalledMode;
        _specialFeatureCheckButton.Toggled += SetApplySpecialIndex;
        _specialFeatureOptionButton.ItemSelected += SetSpecialIndex;
    }


    private void UpdatePlanetUi()
    {
        _radiusLineEdit.Text = $"{_hexPlanetManager.Radius:F2}";
        _divisionLineEdit.Text = $"{_hexPlanetManager.Divisions}";
        _chunkDivisionLineEdit.Text = $"{_hexPlanetManager.ChunkDivisions}";
        _chunkCountLabel.Text = $"分块总数：{_chunkService.GetCount()}";
        _tileCountLabel.Text = $"地块总数：{_tileService.GetCount()}";
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

            if (TileOverrider.EditMode)
            {
                // 编辑模式下更新预览网格
                _hexPlanetManager.UpdateEditPreviewChunk(TileOverrider);
                if (Input.IsActionJustPressed("destroy_unit"))
                {
                    _hexPlanetManager.DestroyUnit();
                    return;
                }

                if (Input.IsActionJustPressed("create_unit"))
                {
                    _hexPlanetManager.CreateUnit();
                    return;
                }
            }
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
            if (TileOverrider.EditMode)
            {
                EditTiles(ChosenTile);
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
            if (!TileOverrider.EditMode)
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
        _isDrag = _tileService.IsNeighbor(currentTile, _previousTile);
    }

    private void EditTiles(Tile tile)
    {
        foreach (var t in _tileService.GetTilesInDistance(tile, TileOverrider.BrushSize))
            EditTile(t);
    }

    private void EditTile(Tile tile)
    {
        if (TileOverrider.ApplyTerrain)
            _tileService.SetTerrainTypeIndex(tile, TileOverrider.ActiveTerrain);
        if (TileOverrider.ApplyElevation)
            _tileService.SetElevation(tile, TileOverrider.ActiveElevation);
        if (TileOverrider.ApplyWaterLevel)
            _tileService.SetWaterLevel(tile, TileOverrider.ActiveWaterLevel);
        if (TileOverrider.ApplySpecialIndex)
            _tileService.SetSpecialIndex(tile, TileOverrider.ActiveSpecialIndex);
        if (TileOverrider.ApplyUrbanLevel)
            _tileService.SetUrbanLevel(tile, TileOverrider.ActiveUrbanLevel);
        if (TileOverrider.ApplyFarmLevel)
            _tileService.SetFarmLevel(tile, TileOverrider.ActiveFarmLevel);
        if (TileOverrider.ApplyPlantLevel)
            _tileService.SetPlantLevel(tile, TileOverrider.ActivePlantLevel);
        if (TileOverrider.RiverMode == OptionalToggle.No)
            _tileService.RemoveRiver(tile);
        if (TileOverrider.RoadMode == OptionalToggle.No)
            _tileService.RemoveRoads(tile);
        if (TileOverrider.WalledMode != OptionalToggle.Ignore)
            _tileService.SetWalled(tile, TileOverrider.WalledMode == OptionalToggle.Yes);
        if (_isDrag)
        {
            if (TileOverrider.RiverMode == OptionalToggle.Yes)
                _tileService.SetOutgoingRiver(_previousTile, _dragTile);
            if (TileOverrider.RoadMode == OptionalToggle.Yes)
                _tileService.AddRoad(_previousTile, _dragTile);
        }
    }
}