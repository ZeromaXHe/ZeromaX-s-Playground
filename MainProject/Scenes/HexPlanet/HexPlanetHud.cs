using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Dependency;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet;

public partial class HexPlanetHud : Control
{
    enum OptionalToggle
    {
        Ignore,
        Yes,
        No
    }

    [Export] private HexPlanetManager _hexPlanetManager;
    [Export] private Color[] _colors;

    #region on-ready 节点

    private SubViewportContainer _subViewportContainer;
    private CheckButton _wireframeCheckButton;

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
    private GridContainer _tileGrid;
    private LineEdit _idLineEdit;
    private LineEdit _chunkLineEdit;
    private LineEdit _heightLineEdit;
    private LineEdit _elevationLineEdit;

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
        _tileGrid = GetNode<GridContainer>("%TileGrid");
        _idLineEdit = GetNode<LineEdit>("%IdLineEdit");
        _chunkLineEdit = GetNode<LineEdit>("%ChunkLineEdit");
        _heightLineEdit = GetNode<LineEdit>("%HeightLineEdit");
        _elevationLineEdit = GetNode<LineEdit>("%ElevationLineEdit");
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
        _elevationVSlider.MaxValue = HexMetrics.ElevationStep;
        _elevationVSlider.TickCount = HexMetrics.ElevationStep + 1;
        _waterVSlider.MaxValue = HexMetrics.ElevationStep;
        _waterVSlider.TickCount = HexMetrics.ElevationStep + 1;
    }

    #endregion

    #region services

    private ITileService _tileService;
    private IChunkService _chunkService;
    private ISelectViewService _selectViewService;

    private void InitServices()
    {
        _tileService = Context.GetBean<ITileService>();
        _chunkService = Context.GetBean<IChunkService>();
        _selectViewService = Context.GetBean<ISelectViewService>();
    }

    #endregion

    #region 编辑功能

    private bool _editMode;
    private bool _applyTerrain;
    private int _activeTerrain;
    private bool _applyElevation;
    private int _activeElevation;
    private bool _applyWaterLevel;
    private int _activeWaterLevel;
    private int _brushSize;
    private OptionalToggle _riverMode;
    private OptionalToggle _roadMode;
    private bool _applyUrbanLevel;
    private int _activeUrbanLevel;
    private bool _applyFarmLevel;
    private int _activeFarmLevel;
    private bool _applyPlantLevel;
    private int _activePlantLevel;
    private OptionalToggle _walledMode;
    private bool _applySpecialIndex;
    private int _activeSpecialIndex;

    private void SetEditMode(bool toggle)
    {
        _editMode = toggle;
        _hexPlanetManager.ShowUi(!toggle);
    }

    private void SelectTerrain(long index)
    {
        _applyTerrain = index > 0;
        if (_applyTerrain)
        {
            _activeTerrain = (int)index - 1;
        }
    }

    private void SetElevation(double elevation)
    {
        _activeElevation = (int)elevation;
        _elevationValueLabel.Text = _activeElevation.ToString();
    }

    private void SetApplyElevation(bool toggle) => _applyElevation = toggle;

    private void SetBrushSize(double brushSize)
    {
        _brushSize = (int)brushSize;
        _selectViewService.SelectViewSize = _brushSize;
        _brushLabel.Text = $"笔刷大小：{_brushSize}";
    }

    private void SetRiverMode(long mode) => _riverMode = (OptionalToggle)mode;
    private void SetRoadMode(long mode) => _roadMode = (OptionalToggle)mode;
    private void SetApplyWaterLevel(bool toggle) => _applyWaterLevel = toggle;

    private void SetWaterLevel(double level)
    {
        _activeWaterLevel = (int)level;
        _waterValueLabel.Text = _activeWaterLevel.ToString();
    }

    private void SetApplyUrbanLevel(bool toggle) => _applyUrbanLevel = toggle;
    private void SetUrbanLevel(double level) => _activeUrbanLevel = (int)level;
    private void SetApplyFarmLevel(bool toggle) => _applyFarmLevel = toggle;
    private void SetFarmLevel(double level) => _activeFarmLevel = (int)level;
    private void SetApplyPlantLevel(bool toggle) => _applyPlantLevel = toggle;
    private void SetPlantLevel(double level) => _activePlantLevel = (int)level;
    private void SetWalledMode(long mode) => _walledMode = (OptionalToggle)mode;
    private void SetApplySpecialIndex(bool toggle) => _applySpecialIndex = toggle;
    private void SetSpecialIndex(long index) => _activeSpecialIndex = (int)index;

    #endregion

    private int? _chosenTileId;

    private int? ChosenTileId
    {
        get => _chosenTileId;
        set
        {
            _chosenTileId = value;
            if (_chosenTileId != null)
            {
                _idLineEdit.Text = _chosenTileId.ToString();
                var tile = _tileService.GetById((int)_chosenTileId);
                _chunkLineEdit.Text = tile.ChunkId.ToString();
                _heightLineEdit.Text = $"{_tileService.GetHeight(tile):F2}";
                _heightLineEdit.Editable = true;
                _elevationLineEdit.Text = tile.Elevation.ToString();
            }
            else
            {
                _idLineEdit.Text = "-";
                _chunkLineEdit.Text = "-";
                _heightLineEdit.Text = "-";
                _heightLineEdit.Editable = false;
                _elevationLineEdit.Text = "-";
            }
        }
    }

    private bool _isDrag;
    private Tile _dragTile;
    private Tile _previousTile;

    public override void _Ready()
    {
        InitOnReadyNodes();
        InitServices();

        SetEditMode(_editCheckButton.ButtonPressed);
        SelectTerrain(0);
        UpdateNewPlanetInfo();
        InitSignals();
    }

    private void InitSignals()
    {
        _hexPlanetManager.NewPlanetGenerated += UpdateNewPlanetInfo;

        _wireframeCheckButton.Toggled += toggle =>
            _hexPlanetManager.GetViewport()
                .SetDebugDraw(toggle ? Viewport.DebugDrawEnum.Wireframe : Viewport.DebugDrawEnum.Disabled);

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

        _heightLineEdit.TextSubmitted += text =>
        {
            if (_chosenTileId != null)
            {
                var chosenTileId = (int)_chosenTileId;
                if (float.TryParse(text, out var height))
                {
                    var tile = _tileService.GetById(chosenTileId);
                    if (Mathf.Abs(height - _tileService.GetHeight(tile)) < 0.0001f) return;
                    _tileService.SetHeight(tile, height);
                }
                else _heightLineEdit.Text = $"{_tileService.GetHeightById(chosenTileId):F2}";
            }
            else _heightLineEdit.Text = "-"; // 应该不会进入这个分支，控制了此时不可编辑
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
                _applyTerrain = false;
                _applyElevation = false;
            }
        };

        _editCheckButton.Toggled += SetEditMode;
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

    private void UpdateNewPlanetInfo()
    {
        _radiusLineEdit.Text = $"{_hexPlanetManager.Radius:F2}";
        _divisionLineEdit.Text = $"{_hexPlanetManager.Divisions}";
        _chunkDivisionLineEdit.Text = $"{_hexPlanetManager.ChunkDivisions}";
        _chunkCountLabel.Text = $"地块总数：{_chunkService.GetCount()}";
        _tileCountLabel.Text = $"分块总数：{_tileService.GetCount()}";
        ChosenTileId = null;
        _tileService.UnitHeight = _hexPlanetManager.Radius * HexMetrics.MaxHeightRadiusRatio / HexMetrics.ElevationStep;
    }

    public override void _Process(double delta)
    {
        if (Input.IsMouseButtonPressed(MouseButton.Left) &&
            GetViewport().GuiGetHoveredControl() == _subViewportContainer)
        {
            // 在 SubViewportContainer 上按下鼠标左键时，获取鼠标位置地块并更新
            ChosenTileId = _hexPlanetManager.GetTileIdUnderCursor();
            if (ChosenTileId != null)
            {
                var currentTile = _tileService.GetById((int)ChosenTileId);
                if (_previousTile != null && _previousTile != currentTile)
                    ValidateDrag(currentTile);
                else
                    _isDrag = false;
                if (_editMode)
                    EditTiles(currentTile);
                else
                    FindDistanceTo(currentTile);
                _previousTile = currentTile;
            }
            else
                _previousTile = null;
        }
        else
            _previousTile = null;
    }

    private void ValidateDrag(Tile currentTile)
    {
        _dragTile = currentTile;
        _isDrag = _tileService.IsNeighbor(currentTile, _previousTile);
    }

    private void EditTiles(Tile tile)
    {
        foreach (var t in _tileService.GetTilesInDistance(tile, _brushSize))
            EditTile(t);
    }

    private void EditTile(Tile tile)
    {
        if (_applyTerrain)
            _tileService.SetTerrainTypeIndex(tile, _activeTerrain);
        if (_applyElevation)
            _tileService.SetElevation(tile, _activeElevation);
        if (_applyWaterLevel)
            _tileService.SetWaterLevel(tile, _activeWaterLevel);
        if (_applySpecialIndex)
            _tileService.SetSpecialIndex(tile, _activeSpecialIndex);
        if (_applyUrbanLevel)
            _tileService.SetUrbanLevel(tile, _activeUrbanLevel);
        if (_applyFarmLevel)
            _tileService.SetFarmLevel(tile, _activeFarmLevel);
        if (_applyPlantLevel)
            _tileService.SetPlantLevel(tile, _activePlantLevel);
        if (_riverMode == OptionalToggle.No)
            _tileService.RemoveRiver(tile);
        if (_roadMode == OptionalToggle.No)
            _tileService.RemoveRoads(tile);
        if (_walledMode != OptionalToggle.Ignore)
            _tileService.SetWalled(tile, _walledMode == OptionalToggle.Yes);
        if (_isDrag)
        {
            if (_riverMode == OptionalToggle.Yes)
                _tileService.SetOutgoingRiver(_previousTile, _dragTile);
            if (_roadMode == OptionalToggle.Yes)
                _tileService.AddRoad(_previousTile, _dragTile);
        }

        ChosenTileId = tile.Id; // 刷新 GUI 地块信息
    }

    private void FindDistanceTo(Tile currentTile)
    {
        foreach (var tile in _tileService.GetAll())
        {
            _tileService.UpdateTileLabel(tile);
        }
    }
}