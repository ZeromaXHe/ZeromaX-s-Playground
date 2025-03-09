using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Dependency;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Struct;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet;

public partial class HexPlanetHud : Control
{
    public HexPlanetHud() => InitServices();

    [Export] private HexPlanetManager _hexPlanetManager;

    #region on-ready 节点

    private SubViewportContainer _subViewportContainer;

    private CheckButton _wireframeCheckButton;

    // 小地图
    private MiniMapManager _miniMapManager;

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
        _miniMapManager = GetNode<MiniMapManager>("%MiniMapManager");
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

    private int _showLabelMode;
    private HexTileDataOverrider _tileOverrider = new();

    private void SetEditMode(bool toggle)
    {
        _tileOverrider = _tileOverrider with { EditMode = toggle };
        _hexPlanetManager.SetEditMode(toggle);
    }

    private void SetShowLabelMode(long mode)
    {
        _showLabelMode = (int)mode;
        _hexPlanetManager.SetShowLabelMode(_showLabelMode);
    }

    private void SelectTerrain(long index)
    {
        _tileOverrider = _tileOverrider with { ApplyTerrain = index > 0 };
        if (_tileOverrider.ApplyTerrain)
        {
            _tileOverrider = _tileOverrider with { ActiveTerrain = (int)index - 1 };
        }
    }

    private void SetElevation(double elevation)
    {
        _tileOverrider = _tileOverrider with { ActiveElevation = (int)elevation };
        _elevationValueLabel.Text = _tileOverrider.ActiveElevation.ToString();
    }

    private void SetApplyElevation(bool toggle) => _tileOverrider = _tileOverrider with { ApplyElevation = toggle };

    private void SetBrushSize(double brushSize)
    {
        RenderingServer.GlobalShaderParameterSet("editor_brush_size", (int)brushSize);
        _tileOverrider = _tileOverrider with { BrushSize = (int)brushSize };
        _selectViewService.SelectViewSize = _tileOverrider.BrushSize;
        _brushLabel.Text = $"笔刷大小：{_tileOverrider.BrushSize}";
    }

    private void SetRiverMode(long mode) => _tileOverrider = _tileOverrider with { RiverMode = (OptionalToggle)mode };
    private void SetRoadMode(long mode) => _tileOverrider = _tileOverrider with { RoadMode = (OptionalToggle)mode };
    private void SetApplyWaterLevel(bool toggle) => _tileOverrider = _tileOverrider with { ApplyWaterLevel = toggle };

    private void SetWaterLevel(double level)
    {
        _tileOverrider = _tileOverrider with { ActiveWaterLevel = (int)level };
        _waterValueLabel.Text = _tileOverrider.ActiveWaterLevel.ToString();
    }

    private void SetApplyUrbanLevel(bool toggle) => _tileOverrider = _tileOverrider with { ApplyUrbanLevel = toggle };
    private void SetUrbanLevel(double level) => _tileOverrider = _tileOverrider with { ActiveUrbanLevel = (int)level };
    private void SetApplyFarmLevel(bool toggle) => _tileOverrider = _tileOverrider with { ApplyFarmLevel = toggle };
    private void SetFarmLevel(double level) => _tileOverrider = _tileOverrider with { ActiveFarmLevel = (int)level };
    private void SetApplyPlantLevel(bool toggle) => _tileOverrider = _tileOverrider with { ApplyPlantLevel = toggle };
    private void SetPlantLevel(double level) => _tileOverrider = _tileOverrider with { ActivePlantLevel = (int)level };
    private void SetWalledMode(long mode) => _tileOverrider = _tileOverrider with { WalledMode = (OptionalToggle)mode };

    private void SetApplySpecialIndex(bool toggle) =>
        _tileOverrider = _tileOverrider with { ApplySpecialIndex = toggle };

    private void SetSpecialIndex(long index) =>
        _tileOverrider = _tileOverrider with { ActiveSpecialIndex = (int)index };

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
                _coordsLineEdit.Text = $"{_tileService.GetSphereAxial(_chosenTile)}";
                _coordsLineEdit.TooltipText = $"{_tileService.GetSphereAxial(_chosenTile)}";
                _heightLineEdit.Text = $"{_tileService.GetHeight(_chosenTile):F2}";
                _heightLineEdit.Editable = true;
                _elevationLineEdit.Text = _chosenTile.Data.Elevation.ToString();
            }
            else
            {
                _idLineEdit.Text = "-";
                _chunkLineEdit.Text = "-";
                _coordsLineEdit.Text = "-";
                _coordsLineEdit.TooltipText = null;
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

        SetEditMode(_editCheckButton.ButtonPressed);
        SetShowLabelMode(_showLableOptionButton.Selected);
        SelectTerrain(0);
        UpdateNewPlanetInfo();
        InitSignals();

        _miniMapManager.Init();
    }

    private void InitSignals()
    {
        _hexPlanetManager.NewPlanetGenerated += UpdateNewPlanetInfo;
        _hexPlanetManager.NewPlanetGenerated += _miniMapManager.Init;

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
                _tileOverrider.ApplyTerrain = false;
                _tileOverrider.ApplyElevation = false;
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

    private void UpdateNewPlanetInfo()
    {
        UpdatePlanetUi();
        ChosenTile = null;
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

            if (_tileOverrider.EditMode)
            {
                // 编辑模式下更新预览网格
                _hexPlanetManager.UpdateEditPreviewChunk(_tileOverrider);
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
            if (_tileOverrider.EditMode)
                EditTiles(ChosenTile);
            else if (Input.IsActionJustPressed("choose_unit"))
                _hexPlanetManager.FindPath(ChosenTile);
            _previousTile = ChosenTile;
        }
        else
        {
            if (!_tileOverrider.EditMode)
                _hexPlanetManager.FindPath(ChosenTile);
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
        foreach (var t in _tileService.GetTilesInDistance(tile, _tileOverrider.BrushSize))
            EditTile(t);
    }

    private void EditTile(Tile tile)
    {
        if (_tileOverrider.ApplyTerrain)
            _tileService.SetTerrainTypeIndex(tile, _tileOverrider.ActiveTerrain);
        if (_tileOverrider.ApplyElevation)
            _tileService.SetElevation(tile, _tileOverrider.ActiveElevation);
        if (_tileOverrider.ApplyWaterLevel)
            _tileService.SetWaterLevel(tile, _tileOverrider.ActiveWaterLevel);
        if (_tileOverrider.ApplySpecialIndex)
            _tileService.SetSpecialIndex(tile, _tileOverrider.ActiveSpecialIndex);
        if (_tileOverrider.ApplyUrbanLevel)
            _tileService.SetUrbanLevel(tile, _tileOverrider.ActiveUrbanLevel);
        if (_tileOverrider.ApplyFarmLevel)
            _tileService.SetFarmLevel(tile, _tileOverrider.ActiveFarmLevel);
        if (_tileOverrider.ApplyPlantLevel)
            _tileService.SetPlantLevel(tile, _tileOverrider.ActivePlantLevel);
        if (_tileOverrider.RiverMode == OptionalToggle.No)
            _tileService.RemoveRiver(tile);
        if (_tileOverrider.RoadMode == OptionalToggle.No)
            _tileService.RemoveRoads(tile);
        if (_tileOverrider.WalledMode != OptionalToggle.Ignore)
            _tileService.SetWalled(tile, _tileOverrider.WalledMode == OptionalToggle.Yes);
        if (_isDrag)
        {
            if (_tileOverrider.RiverMode == OptionalToggle.Yes)
                _tileService.SetOutgoingRiver(_previousTile, _dragTile);
            if (_tileOverrider.RoadMode == OptionalToggle.Yes)
                _tileService.AddRoad(_previousTile, _dragTile);
        }

        ChosenTile = tile; // 刷新 GUI 地块信息
    }
}