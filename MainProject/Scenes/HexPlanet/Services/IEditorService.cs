using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Structs;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Services;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-12 09:18
public interface IEditorService
{
    delegate void LabelModeChangedEvent(int labelMode);

    event LabelModeChangedEvent LabelModeChanged;

    delegate void EditModeChangedEvent(bool editMode);

    event EditModeChangedEvent EditModeChanged;
    int LabelMode { get; set; }
    HexTileDataOverrider TileOverrider { get; set; }
}