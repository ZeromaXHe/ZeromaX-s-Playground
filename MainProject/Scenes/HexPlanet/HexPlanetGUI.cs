using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet;

public partial class HexPlanetGui : Control
{
    [Export] private HexPlanetManager _hexPlanetManager;
    [Export] private Color[] _colors;

    #region on-ready nodes

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
    private Label _brushLabel;
    private HSlider _brushHSlider;

    #endregion

    private bool _applyColor;
    private Color _activeColor;
    private bool _applyElevation;
    private int _activeElevation;
    private int _brushSize;

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
                var tile = Tile.GetById((int)_chosenTileId);
                _chunkLineEdit.Text = tile.ChunkId.ToString();
                _heightLineEdit.Text = $"{tile.Height:F2}";
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
        _brushLabel = GetNode<Label>("%BrushLabel");
        _brushHSlider = GetNode<HSlider>("%BrushHSlider");

        _elevationVSlider.MaxValue = HexMetrics.ElevationStep;
        _elevationVSlider.TickCount = HexMetrics.ElevationStep + 1;
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
                    var tile = Tile.GetById(chosenTileId);
                    if (Mathf.Abs(height - tile.Height) < 0.0001f) return;
                    tile.Height = height;
                    _hexPlanetManager.UpdateMesh(tile);
                }
                else _heightLineEdit.Text = $"{Tile.GetById(chosenTileId).Height:F2}";
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
        _brushHSlider.ValueChanged += SetBrushSize;
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
        _hexPlanetManager.SelectViewSize = _brushSize;
        _brushLabel.Text = $"笔刷大小：{_brushSize}";
    }

    private void UpdateNewPlanetInfo()
    {
        _radiusLineEdit.Text = $"{_hexPlanetManager.Radius:F2}";
        _divisionLineEdit.Text = $"{_hexPlanetManager.Divisions}";
        _chunkDivisionLineEdit.Text = $"{_hexPlanetManager.ChunkDivisions}";
        _chunkCountLabel.Text = $"地块总数：{Chunk.GetCount()}";
        _tileCountLabel.Text = $"分块总数：{Tile.GetCount()}";
        ChosenTileId = null;
        Tile.UnitHeight = _hexPlanetManager.Radius * HexMetrics.MaxHeightRadiusRatio / HexMetrics.ElevationStep;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left }
            && GetViewport().GuiGetHoveredControl() == _subViewportContainer)
        {
            // 在 SubViewportContainer 上按下鼠标左键时，获取鼠标位置地块并更新
            ChosenTileId = _hexPlanetManager.GetTileIdUnderCursor();
            if (ChosenTileId != null)
                EditTiles(Tile.GetById((int)ChosenTileId));
        }
    }
    
    private void EditTiles(Tile tile)
    {
        foreach (var t in tile.GetTilesInDistance(_brushSize))
            EditTile(t);
    }
    

    private void EditTile(Tile tile)
    {
        var changed = false;
        if (_applyColor && _activeColor != tile.Color)
        {
            tile.Color = _activeColor;
            changed = true;
        }

        if (_applyElevation && _activeElevation != tile.Elevation)
        {
            tile.Elevation = _activeElevation;
            changed = true;
        }

        if (changed)
            _hexPlanetManager.UpdateMesh(tile);
        ChosenTileId = tile.Id; // 刷新 GUI 地块信息
    }
}