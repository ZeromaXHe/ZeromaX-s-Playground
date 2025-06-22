using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Godot;
using TO.Abstractions.Models.Planets;
using TO.Abstractions.Views.Chunks;
using TO.Domains.Enums.HexSpheres.Chunks;
using TO.Presenters.Views.Chunks;

namespace TerraObserver.Scenes.Chunks.Views;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-06 16:24:42
[Tool]
public partial class ChunkLoader : ChunkLoaderFS, IChunkLoader
{
    #region 生命周期

    public override void _Process(double delta) => Processed?.Invoke();

    #endregion

    #region 事件

    public event Action? Processed;
    public event Action<IHexGridChunk>? HexGridChunkGenerated;

    #region Export 属性

    [Export] private PackedScene? _gridChunkScene;

    #endregion

    #endregion
    public override IHexGridChunk GetUnusedChunk()
    {
        if (UnusedChunks is not null && UnusedChunks.Count != 0)
            return UnusedChunks!.Dequeue();
        // 没有空闲分块的话，初始化新的
        var hexGridChunk = _gridChunkScene!.Instantiate<HexGridChunk>();
        hexGridChunk.Name = $"HexGridChunk{GetChildCount()}";
        AddChild(hexGridChunk); // 必须先加入场景树，让 _Ready() 先于 Init() 执行
        HexGridChunkGenerated?.Invoke(hexGridChunk);
        return hexGridChunk;
    }
}