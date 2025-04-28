using Domains.Services.Abstractions.Nodes.IdInstances;
using Domains.Services.Abstractions.Nodes.Singletons.ChunkManagers;
using Domains.Services.Abstractions.Shaders;
using Godot;
using Infras.Readers.Abstractions.Nodes.Singletons;
using Infras.Readers.Abstractions.Nodes.Singletons.ChunkManagers;
using Nodes.Abstractions.ChunkManagers;

namespace Apps.Commands.Nodes.Singletons.ChunkManagers;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 20:33:13
public class ChunkLoaderCommander
{
    private readonly IChunkLoaderRepo _chunkLoaderRepo;
    private readonly IChunkLoaderService _chunkLoaderService;

    private readonly IHexGridChunkService _hexGridChunkService;
    private readonly ITileShaderService _tileShaderService;
    private readonly IOrbitCameraRepo _orbitCameraRepo;

    public ChunkLoaderCommander(IChunkLoaderRepo chunkLoaderRepo, IChunkLoaderService chunkLoaderService,
        IHexGridChunkService hexGridChunkService, ITileShaderService tileShaderService,
        IOrbitCameraRepo orbitCameraRepo)
    {
        _chunkLoaderRepo = chunkLoaderRepo;
        _chunkLoaderRepo.Ready += OnReady;
        _chunkLoaderRepo.Processed += OnProcessed;
        _chunkLoaderRepo.TreeExiting += OnTreeExiting;
        _chunkLoaderService = chunkLoaderService;

        _hexGridChunkService = hexGridChunkService;
        _tileShaderService = tileShaderService;
        _tileShaderService.TileExplored += _hexGridChunkService.ExploreFeatures;
        _orbitCameraRepo = orbitCameraRepo;
    }

    public void ReleaseEvents()
    {
        _chunkLoaderRepo.Ready -= OnReady;
        _chunkLoaderRepo.Processed -= OnProcessed;
        _chunkLoaderRepo.TreeExiting -= OnTreeExiting;
        _tileShaderService.TileExplored -= _hexGridChunkService.ExploreFeatures;
    }

    private IChunkLoader? _self;

    private void OnReady()
    {
        _self = _chunkLoaderRepo.Singleton!;
        if (!Engine.IsEditorHint())
        {
            _orbitCameraRepo.Transformed += _chunkLoaderService.UpdateInsightChunks;
        }
    }

    private void OnProcessed(double delta) =>
        _self?.OnProcessed(delta, _hexGridChunkService.ShowChunk, _hexGridChunkService.HideChunk);

    private void OnTreeExiting()
    {
        // 不小心忽视了事件的解绑，会在编辑器下"重载已保存场景"时出问题报错！
        // （比如地图生成器逻辑会发出分块刷新信号，这时候老的场景代码貌似还在内存里，
        // 它接到事件后处理时，字典里的 Chunk 场景都已经释放，不存在了所以报错）
        // （对于新的场景，新分块字典里没数据，没有问题）
        // ERROR: /root/godot/modules/mono/glue/GodotSharp/GodotSharp/Core/NativeInterop/ExceptionUtils.cs:113 - System.ObjectDisposedException: Cannot access a disposed object.
        // ERROR: Object name: 'ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node.HexGridChunk'.
        // 【切记】所以这里需要在退出场景树时清理事件监听！！！
        if (!Engine.IsEditorHint())
        {
            _orbitCameraRepo.Transformed -= _chunkLoaderService.UpdateInsightChunks;
        }

        _self = null;
    }
}