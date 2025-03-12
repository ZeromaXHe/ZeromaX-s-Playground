using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Struct;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service;

public interface IEditorService
{
    delegate void LabelModeChangedEvent(int labelMode);

    event LabelModeChangedEvent LabelModeChanged;

    delegate void EditModeChangedEvent(bool editMode);

    event EditModeChangedEvent EditModeChanged;
    int LabelMode { get; set; }
    HexTileDataOverrider TileOverrider { get; set; }
}