using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Dependency;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet;

public partial class HexPlanetGui : Control
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
    private TabBar _editTabBar;
    private GridContainer _editGrid;
    private OptionButton _colorOptionButton;
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

    #endregion

    #region services

    private ITileService _tileService;
    private IChunkService _chunkService;
    private ISelectViewService _selectViewService;

    #endregion

    private bool _applyColor;
    private Color _activeColor;
    private bool _applyElevation;
    private int _activeElevation;
    private bool _applyWaterLevel;
    private int _activeWaterLevel;
    private int _brushSize;
    private OptionalToggle _riverMode;
    private OptionalToggle _roadMode;

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
        _subViewportContainer = GetNode<SubViewportContainer>("%SubViewportContainer");
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
        _editTabBar = GetNode<TabBar>("%EditTabBar");
        _editGrid = GetNode<GridContainer>("%EditGrid");
        _colorOptionButton = GetNode<OptionButton>("%ColorOptionButton");
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

        _tileService = Context.GetBean<ITileService>();
        _chunkService = Context.GetBean<IChunkService>();
        _selectViewService = Context.GetBean<ISelectViewService>();

        _elevationVSlider.MaxValue = HexMetrics.ElevationStep;
        _elevationVSlider.TickCount = HexMetrics.ElevationStep + 1;
        _waterVSlider.MaxValue = HexMetrics.ElevationStep;
        _waterVSlider.TickCount = HexMetrics.ElevationStep + 1;

        SelectColor(0);
        UpdateNewPlanetInfo();
        _hexPlanetManager.NewPlanetGenerated += UpdateNewPlanetInfo;

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
                SelectColor(_colorOptionButton.Selected);
                SetElevation(_elevationVSlider.Value);
            }
            else
            {
                _applyColor = false;
                _applyElevation = false;
            }
        };

        _colorOptionButton.ItemSelected += SelectColor;
        _elevationVSlider.ValueChanged += SetElevation;
        _elevationCheckButton.Toggled += SetApplyElevation;
        _waterVSlider.ValueChanged += SetWaterLevel;
        _waterCheckButton.Toggled += SetApplyWaterLevel;
        _brushHSlider.ValueChanged += SetBrushSize;
        _riverOptionButton.ItemSelected += SetRiverMode;
        _roadOptionButton.ItemSelected += SetRoadMode;
    }

    private void SelectColor(long index)
    {
        _applyColor = index > 0;
        if (_applyColor)
        {
            _activeColor = _colors[index - 1];
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
                EditTiles(currentTile);
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
        if (_applyColor)
            _tileService.SetColor(tile, _activeColor);
        if (_applyElevation)
            _tileService.SetElevation(tile, _activeElevation);
        if (_applyWaterLevel)
            _tileService.SetWaterLevel(tile, _activeWaterLevel);
        if (_riverMode == OptionalToggle.No)
            _tileService.RemoveRiver(tile);
        if (_isDrag)
        {
            if (_riverMode == OptionalToggle.Yes)
                _tileService.SetOutgoingRiver(_previousTile, _dragTile);
            if (_roadMode == OptionalToggle.Yes)
                _tileService.AddRoad(_previousTile, _dragTile);
        }

        ChosenTileId = tile.Id; // 刷新 GUI 地块信息
    }
}