using Domains.Services.Abstractions.Uis;
using Infras.Readers.Abstractions.Nodes.Singletons;

namespace Apps.Commands;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 14:53:49
public class HexPlanetManagerCommander
{
    private readonly IHexPlanetManagerRepo _hexPlanetManagerRepo;

    public HexPlanetManagerCommander(IHexPlanetManagerRepo hexPlanetManagerRepo)
    {
        _hexPlanetManagerRepo = hexPlanetManagerRepo;
        _hexPlanetManagerRepo.Ready += OnReady;
        _hexPlanetManagerRepo.TreeExiting += OnTreeExiting;
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
}