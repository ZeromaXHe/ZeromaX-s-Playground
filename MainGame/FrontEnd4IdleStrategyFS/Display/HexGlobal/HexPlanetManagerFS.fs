namespace FrontEnd4IdleStrategyFS.Display.HexGlobal

open System
open System.Reflection
open System.Runtime.InteropServices
open System.Runtime.Loader
open System.Threading
open FrontEnd4IdleStrategyFS.Global.Common
open Godot

type HexPlanetManagerFS() as this =
    inherit Node3D()

    let _hexChunkRenders = lazy this.GetNode<Node3D> "HexChunkRenders"

    let mutable _hexGlobalEntry: HexEntry option = None

    let mutable _chunkAddedSub: IDisposable option = None

    let clearRenderObjects () =
        if _hexChunkRenders.Value = null then
            GD.Print "HexChunkRenders is null"
        else
            _hexChunkRenders.Value.GetChildren() |> Seq.iter _.QueueFree()

    let init () =
        _hexGlobalEntry <-
            HexEntry(
                this._subdivisions,
                this._chunkSubdivisions,
                this._planetRadius,
                this._maxHeight,
                this._minHeight,
                this._octaves,
                this._noiseScaling,
                this._lacunarity,
                this._persistence
            )
            |> Some

        _chunkAddedSub <-
            _hexGlobalEntry.Value.ChunksAdded
            |> ObservableSyncContextUtil.subscribePost (fun chunks ->
                chunks
                |> List.iteri (fun i c ->
                    let chunkRender = new HexChunkRendererFS(_hexGlobalEntry.Value)
                    chunkRender.Name <- $"Chunk {i}"
                    chunkRender.Position <- Vector3.Zero
                    chunkRender._renderedChunkId <- c.Id
                    // GD.Print $"Chunk {i} added"
                    _hexChunkRenders.Value.AddChild chunkRender
                    chunkRender.UpdateMesh() // 进入场景树时不会自动调用 _Ready？手动调用下
                ))
            |> Some

        // 必须在同步上下文中执行，否则 Init 内容不会被响应式编程 Subscribe 监听到（会比上面监听逻辑更早执行）
        SynchronizationContext.Current.Post(
            (fun _ ->
                clearRenderObjects ()
                _hexGlobalEntry.Value.GeneratePlanetTilesAndChunks()),
            null
        )

    let mutable _regenerate = false

    member this._Regenerate
        with get () = _regenerate
        and set value =
            if value then
                init ()
                _regenerate <- false

    // Perlin 噪声相关 Export
    member val _octaves: int = 1 with get, set
    member val _persistence: float32 = 0.5f with get, set
    member val _lacunarity: float32 = 2.0f with get, set
    member val _minHeight: float32 = 0.0f with get, set
    member val _maxHeight: float32 = 30.0f with get, set
    member val _noiseScaling: float32 = 100.0f with get, set
    // 星球属性 Export
    member val _planetRadius: float32 = 100.0f with get, set
    member val _subdivisions: int = 3 with get, set
    member val _chunkSubdivisions: int = 3 with get, set

    override this._Ready() =
        // TODO：这些都没用，感觉 C# 继承 F# 的方式写 Tool 完全无法避免报错：.NET: Failed to unload assemblies
        let handle = GCHandle.Alloc(this)
        // 不确定这里会不会调用
        let alc = AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly())
        GD.Print $"FS ALC: {alc} ALC is null? {alc = null}"
        alc.add_Unloading (fun assemblyLoadContext ->
            GD.Print "Start Unloading HexPlanetManagerFS"
            _chunkAddedSub |> Option.iter _.Dispose()
            _chunkAddedSub <- None
            _hexGlobalEntry <- None
            clearRenderObjects ()
            GD.Print "End Unloading HexPlanetManagerFS"
            // handle.Free()
        )

        init ()
