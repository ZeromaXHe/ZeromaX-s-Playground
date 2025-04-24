using Godot;
using Infras.Readers.Abstractions.Nodes.Singletons;

namespace Apps.Commands.Nodes.Singletons;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 20:43:35
public class LongitudeLatitudeCommander
{
    private readonly ILongitudeLatitudeRepo _longitudeLatitudeRepo;

    private readonly IOrbitCameraRepo? _orbitCameraRepo;

    public LongitudeLatitudeCommander(ILongitudeLatitudeRepo longitudeLatitudeRepo, IOrbitCameraRepo orbitCameraRepo)
    {
        _longitudeLatitudeRepo = longitudeLatitudeRepo;
        _longitudeLatitudeRepo.Ready += OnReady;
        _longitudeLatitudeRepo.TreeExiting += OnTreeExiting;
        _longitudeLatitudeRepo.FixFullVisibilityChanged += OnFixFullVisibilityChanged;

        _orbitCameraRepo = orbitCameraRepo;
    }

    public void ReleaseEvents()
    {
        _longitudeLatitudeRepo.Ready -= OnReady;
        _longitudeLatitudeRepo.TreeExiting -= OnTreeExiting;
        _longitudeLatitudeRepo.FixFullVisibilityChanged -= OnFixFullVisibilityChanged;
    }

    private void OnReady()
    {
        if (!Engine.IsEditorHint())
            _orbitCameraRepo!.Moved += _longitudeLatitudeRepo.Singleton!.OnCameraMoved;
    }

    private void OnTreeExiting()
    {
        if (!Engine.IsEditorHint())
            _orbitCameraRepo!.Moved -= _longitudeLatitudeRepo.Singleton!.OnCameraMoved;
    }

    private void OnFixFullVisibilityChanged(bool value)
    {
        if (value)
            _orbitCameraRepo!.Moved -= _longitudeLatitudeRepo.Singleton!.OnCameraMoved;
        else
            _orbitCameraRepo!.Moved += _longitudeLatitudeRepo.Singleton!.OnCameraMoved;
    }
}