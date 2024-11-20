namespace FrontEnd4IdleStrategyFS.Display.HexGlobal

open System.Threading
open FrontEnd4IdleStrategyFS.Global
open FrontEnd4IdleStrategyFS.Global.Common
open Godot

type HexPlanetManagerFS() as this =
    inherit Node3D()

    let _globalNode = lazy this.GetNode<GlobalNodeFS> "/root/GlobalNode"
    let _hexChunkRenders = lazy this.GetNode<Node3D> "HexChunkRenders"

    [<DefaultValue>]
    val mutable _regenerate: bool

    // Perlin 噪声相关 Export
    [<DefaultValue>]
    val mutable _octaves: int

    [<DefaultValue>]
    val mutable _persistence: float32

    [<DefaultValue>]
    val mutable _lacunarity: float32

    [<DefaultValue>]
    val mutable _minHeight: float32

    [<DefaultValue>]
    val mutable _maxHeight: float32

    [<DefaultValue>]
    val mutable _noiseScaling: float32
    // 星球属性 Export
    [<DefaultValue>]
    val mutable _planetRadius: float32

    [<DefaultValue>]
    val mutable _subdivisions: int

    [<DefaultValue>]
    val mutable _chunkSubdivisions: int

    let updateRenderObjects () =
        if _hexChunkRenders.Value = null then
            GD.Print "HexChunkRenders is null"
        else
            _hexChunkRenders.Value.GetChildren() |> Seq.iter _.QueueFree()

        _globalNode.Value.HexGlobalEntry.Value.GeneratePlanetTilesAndChunks()

    override this._Ready() =
        // TODO: 临时测试用，后续要根据 Export 同步
        this._octaves <- 1
        this._persistence <- 0.5f
        this._lacunarity <- 2.0f
        this._minHeight <- 0.0f
        this._maxHeight <- 30.0f
        this._noiseScaling <- 100.0f
        this._planetRadius <- 100.0f
        this._subdivisions <- 3
        this._chunkSubdivisions <- 3

        _globalNode.Value.InitHexGlobal
            this._subdivisions
            this._chunkSubdivisions
            this._planetRadius
            this._maxHeight
            this._minHeight
            this._octaves
            this._noiseScaling
            this._lacunarity
            this._persistence

        _globalNode.Value.HexGlobalEntry.Value.ChunksAdded
        |> ObservableSyncContextUtil.subscribePost (fun chunks ->
            chunks
            |> List.iteri (fun i c ->
                let chunkRender = new HexChunkRendererFS()
                chunkRender.Name <- $"Chunk {i}"
                chunkRender.Position <- Vector3.Zero
                chunkRender._renderedChunkId <- c.Id
                // GD.Print $"Chunk {i} added"
                _hexChunkRenders.Value.AddChild chunkRender
                chunkRender.UpdateMesh() // 进入场景树时不会自动调用 _Ready？手动调用下
            ))

        // 必须在同步上下文中执行，否则 Init 内容不会被响应式编程 Subscribe 监听到（会比上面监听逻辑更早执行）
        SynchronizationContext.Current.Post((fun _ -> updateRenderObjects ()), null)
