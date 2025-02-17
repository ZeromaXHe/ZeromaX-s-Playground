using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet;

public partial class HexPlanetGui : Control
{
    [Export] private HexPlanetManager _hexPlanetManager;

    private SubViewportContainer _subViewportContainer;
    private LineEdit _radiusLineEdit;
    private LineEdit _divisionLineEdit;
    private Label _tileCountLabel;
    private LineEdit _idLineEdit;
    private LineEdit _heightLineEdit;

    private TileNode _chosenTileNode;

    private TileNode ChosenTileNode
    {
        get => _chosenTileNode;
        set
        {
            _chosenTileNode = value;
            if (_chosenTileNode != null)
            {
                _idLineEdit.Text = _chosenTileNode.Id.ToString();
                _heightLineEdit.Text = $"{Tile.GetById(_chosenTileNode.Id).Height:F2}";
                _heightLineEdit.Editable = true;
            }
            else
            {
                _idLineEdit.Text = "-";
                _heightLineEdit.Text = "-";
                _heightLineEdit.Editable = false;
            }
        }
    }

    public override void _Ready()
    {
        _subViewportContainer = GetNode<SubViewportContainer>("%SubViewportContainer");
        _radiusLineEdit = GetNode<LineEdit>("%RadiusLineEdit");
        _divisionLineEdit = GetNode<LineEdit>("%DivisionLineEdit");
        _tileCountLabel = GetNode<Label>("%TileCountLabel");
        _idLineEdit = GetNode<LineEdit>("%IdLineEdit");
        _heightLineEdit = GetNode<LineEdit>("%HeightLineEdit");

        UpdateNewPlanetInfo();
        _hexPlanetManager.NewPlanetGenerated += UpdateNewPlanetInfo;

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

        _heightLineEdit.TextSubmitted += text =>
        {
            if (float.TryParse(text, out var height))
                ChosenTileNode.UpdateHeight(height, _hexPlanetManager.Radius, _hexPlanetManager.HexSize);
            else
                _heightLineEdit.Text = $"{Tile.GetById(ChosenTileNode.Id).Height:F2}";
        };
    }

    private void UpdateNewPlanetInfo()
    {
        _radiusLineEdit.Text = $"{_hexPlanetManager.Radius:F2}";
        _divisionLineEdit.Text = $"{_hexPlanetManager.Divisions}";
        _tileCountLabel.Text = $"总数：{Tile.GetCount()}";
        ChosenTileNode = null;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left }
            && GetViewport().GuiGetHoveredControl() == _subViewportContainer)
        {
            // 在 SubViewportContainer 上按下鼠标左键时，获取鼠标位置地块并更新
            ChosenTileNode = _hexPlanetManager.GetTileNodeUnderCursor();
        }
    }
}