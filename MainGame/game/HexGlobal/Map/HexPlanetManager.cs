// using FrontEnd4IdleStrategyFS.Display.HexGlobal;
// using Godot;
//
// namespace ZeromaXPlayground.game.HexGlobal.Map;
//
// [Tool]
// public partial class HexPlanetManager : HexPlanetManagerFS
// {
//     [Export]
//     private bool Regenerate
//     {
//         get => _Regenerate;
//         set => _Regenerate = value;
//     }
//
//     [Export(PropertyHint.Range, "1,10000")]
//     public float PlanetRadius
//     {
//         get => _planetRadius;
//         set => _planetRadius = value;
//     }
//
//     [Export(PropertyHint.Range, "0,7")]
//     public int Subdivisions
//     {
//         get => _subdivisions;
//         set => _subdivisions = value;
//     }
//
//     [Export(PropertyHint.Range, "0,6")]
//     public int ChunkSubdivisions
//     {
//         get => _chunkSubdivisions;
//         set => _chunkSubdivisions = value;
//     }
//
//     [Export]
//     public float MinHeight
//     {
//         get => _minHeight;
//         set => _minHeight = value;
//     }
//
//     [Export]
//     public float MaxHeight
//     {
//         get => _maxHeight;
//         set => _maxHeight = value;
//     }
//
//     [Export]
//     public float NoiseScaling
//     {
//         get => _noiseScaling;
//         set => _noiseScaling = value;
//     }
//
//     [Export(PropertyHint.Range, "1, 8")]
//     public int Octaves
//     {
//         get => _octaves;
//         set => _octaves = value;
//     }
//
//     [Export(PropertyHint.Range, "1, 10")]
//     public float Lacunarity
//     {
//         get => _lacunarity;
//         set => _lacunarity = value;
//     }
//
//     [Export(PropertyHint.Range, "0, 1")]
//     public float Persistence
//     {
//         get => _persistence;
//         set => _persistence = value;
//     }
//
//     // 必须保留此处和 partial，请忽略 IDE 建议 
//     public override void _Ready()
//     {
//         base._Ready();
//     }
// }

using System;
using System.Reactive.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Threading;
using FrontEnd4IdleStrategyFS.Display.HexGlobal;
using Godot;

namespace ZeromaXPlayground.game.HexGlobal.Map;

[Tool]
public partial class HexPlanetManager : Node3D
{
    [Export]
    private bool Regenerate
    {
        get => _regenerate;
        set
        {
            if (!value) return;
            Init();
            _regenerate = false;
        }
    }

    private bool _regenerate = false;

    // 星球属性 Export
    [Export(PropertyHint.Range, "1,10000")]
    public float PlanetRadius { get; set; } = 100.0f;

    [Export(PropertyHint.Range, "0,7")] public int Subdivisions { get; set; } = 3;
    [Export(PropertyHint.Range, "0,6")] public int ChunkSubdivisions { get; set; } = 3;

    // Perlin 噪声相关 Export

    [Export] public float MinHeight { get; set; } = 0.0f;
    [Export] public float MaxHeight { get; set; } = 30.0f;
    [Export] public float NoiseScaling { get; set; } = 100.0f;
    [Export(PropertyHint.Range, "1, 8")] public int Octaves { get; set; } = 1;
    [Export(PropertyHint.Range, "0, 1")] public float Persistence { get; set; } = 0.5f;
    [Export(PropertyHint.Range, "1, 10")] public float Lacunarity { get; set; } = 2.0f;

    private Node3D _hexChunkRenders;

    private HexEntry _hexEntry = null;

    private IDisposable _chunkAddedSub = null;

    public override void _Ready()
    {
        GD.Print("HexPlanetManager _Ready: Why print twice?!");
        _hexChunkRenders = GetNode<Node3D>("HexChunkRenders");

        // // 参考 Godot Issue #78513：https://github.com/godotengine/godot/issues/78513
        // // 使用一个 strong handle 来阻塞卸载
        // var handle = GCHandle.Alloc(this);
        //
        // // 不在此处向所处的程序集加载上下文中注册 Unload 事件的清理响应式编程监听的逻辑的话，会导致 [Tool] .NET 程序集卸载失败
        // // 从而报错：.NET: Failed to unload assemblies
        // var alcCs = AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly());
        // GD.Print($"C# alcCs: {alcCs?.ToString() ?? "Null alcCs"}, alcCs is null? {alcCs == null}");
        // alcCs!.Unloading += alc =>
        // {
        //     GD.Print("Start Unloading HexPlanetManager");
        //
        //     var alcFs = AssemblyLoadContext.GetLoadContext(typeof(HexPlanetManagerFS).Assembly);
        //     GD.Print($"F# alcFs: {alcFs?.ToString() ?? "Null alcFs"}, alcFs is null? {alcFs == null}");
        //     alcFs?.Unload();
        //
        //     ClearRenderObjects();
        //     _chunkAddedSub?.Dispose();
        //     _chunkAddedSub = null;
        //     _hexEntry = null;
        //     // handle.Free();
        //     GD.Print("End Unloading HexPlanetManager");
        // };

        Init();
    }

    private void Init()
    {
        _hexEntry = new HexEntry(Subdivisions, ChunkSubdivisions, PlanetRadius,
            MinHeight, MaxHeight, NoiseScaling, Octaves, Lacunarity, Persistence);

        // var godotSyncContext = SynchronizationContext.Current!;
        // _chunkAddedSub = _hexEntry.ChunksAdded
        //     .SubscribeOn(godotSyncContext)
        //     .Subscribe(chunks =>
        //         godotSyncContext.Post(_ =>
        //         {
        //             for (var i = 0; i < chunks.Length; i++)
        //             {
        //                 var chunkRender = new HexChunkRendererFS();
        //                 chunkRender.Name = $"Chunk {i}";
        //                 chunkRender.Position = Vector3.Zero;
        //                 chunkRender._renderedChunkId = chunks[i].Id;
        //                 // GD.Print($"Chunk {i} added")
        //                 chunkRender.Mesh = _hexEntry.GetHexChunkMesh(chunks[i].Id); // 进入场景树时不会自动调用 _Ready？手动调用下
        //                 _hexChunkRenders.AddChild(chunkRender);
        //             }
        //         }, null));

        ClearRenderObjects();

        var chunks = _hexEntry.GeneratePlanetTilesAndChunks();

        for (var i = 0; i < chunks.Length; i++)
        {
            var chunkRender = new HexChunkRendererFS();
            chunkRender.Name = $"Chunk {i}";
            chunkRender.Position = Vector3.Zero;
            chunkRender._renderedChunkId = chunks[i].Id;
            // GD.Print($"Chunk {i} added")
            chunkRender.Mesh = _hexEntry.GetHexChunkMesh(chunks[i].Id); // 进入场景树时不会自动调用 _Ready？手动调用下
            _hexChunkRenders.AddChild(chunkRender);
        }

        // 必须在同步上下文中执行，否则 Init 内容不会被响应式编程 Subscribe 监听到（会比上面监听逻辑更早执行）
        // godotSyncContext.Post(_ => _hexEntry.GeneratePlanetTilesAndChunks(), null);
    }

    private void ClearRenderObjects()
    {
        if (_hexChunkRenders == null)
            GD.Print("HexChunkRenders is null");
        else
            foreach (var child in _hexChunkRenders.GetChildren())
            {
                child.QueueFree();
            }
    }
}