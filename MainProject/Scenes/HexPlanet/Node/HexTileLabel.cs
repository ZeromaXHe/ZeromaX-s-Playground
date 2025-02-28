using Godot;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node;

[Tool]
public partial class HexTileLabel : Node3D
{
    public Label3D Label { get; private set; }

    public override void _Ready()
    {
        Label = GetNode<Label3D>("%Label");
    }
}