using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Structs;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Services.Impl;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-12 09:18
public class EditorService : IEditorService
{
    public event IEditorService.LabelModeChangedEvent LabelModeChanged;
    public event IEditorService.EditModeChangedEvent EditModeChanged;

    private int _labelMode;

    public int LabelMode
    {
        get => _labelMode;
        set
        {
            var before = _labelMode;
            _labelMode = value;
            if (before != _labelMode)
                LabelModeChanged?.Invoke(_labelMode);
        }
    }

    private HexTileDataOverrider _tileOverrider = new();

    public HexTileDataOverrider TileOverrider
    {
        get => _tileOverrider;
        set
        {
            var editMode = _tileOverrider.EditMode;
            _tileOverrider = value;
            if (_tileOverrider.EditMode != editMode)
                EditModeChanged?.Invoke(_tileOverrider.EditMode);
        }
    }
}