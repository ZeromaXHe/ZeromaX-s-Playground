using Infras.Readers.Abstractions.Nodes.Singletons;

namespace Apps.Commands.Nodes.Singletons.Planets;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 20:41:15
public class SelectTileViewerCommander
{
    private readonly IHexPlanetManagerRepo _hexPlanetManagerRepo;

    public SelectTileViewerCommander(IHexPlanetManagerRepo hexPlanetManagerRepo)
    {
        _hexPlanetManagerRepo = hexPlanetManagerRepo;
        _hexPlanetManagerRepo.Ready += OnReady;
        _hexPlanetManagerRepo.TreeExiting += OnTreeExiting;
        _hexPlanetManagerRepo.Processed += OnProcessed;
    }

    public void ReleaseEvents()
    {
        _hexPlanetManagerRepo.Ready -= OnReady;
        _hexPlanetManagerRepo.TreeExiting -= OnTreeExiting;
        _hexPlanetManagerRepo.Processed -= OnProcessed;
    }

    private bool NodeReady { get; set; }

    private void OnReady()
    {
        NodeReady = true;
        
    }
    
    private void OnTreeExiting()
    {
        NodeReady = false;
    }

    private void OnProcessed(double delta)
    {
        if (!NodeReady) return;
        
    }
}