namespace FrontEndToolFS.Tool

open FrontEndToolFS.HexGlobal
open Godot

type HexPlanetManagerFS() as this =
    inherit Node3D()

    let _hexChunkRenders = lazy this.GetNode<Node3D> "HexChunkRenders"

    let mutable _hexGlobalEntry: HexEntry option = None

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
                this._minHeight,
                this._maxHeight,
                this._noiseScaling,
                this._octaves,
                this._lacunarity,
                this._persistence
            )
            |> Some

        clearRenderObjects ()

        _hexGlobalEntry.Value.GeneratePlanetTilesAndChunks()
        |> List.iteri (fun i c ->
            let chunkRender = new MeshInstance3D()
            chunkRender.Name <- $"Chunk {i}"
            chunkRender.Position <- Vector3.Zero
            chunkRender.Mesh <- _hexGlobalEntry.Value.GetHexChunkMesh c.Id
            // GD.Print $"Chunk {i} added"
            _hexChunkRenders.Value.AddChild chunkRender)

    let mutable _regenerate = false
    member this._Regenerate
        with get () = _regenerate
        and set value =
            if value then
                init ()
                _regenerate <- false

    // Perlin 噪声相关 Export
    member val _octaves: int = 1 with get, set
    member val _lacunarity: float32 = 2.0f with get, set
    member val _persistence: float32 = 0.5f with get, set
    member val _minHeight: float32 = 0.0f with get, set
    member val _maxHeight: float32 = 30.0f with get, set
    member val _noiseScaling: float32 = 100.0f with get, set
    // 星球属性 Export
    member val _planetRadius: float32 = 100.0f with get, set
    member val _subdivisions: int = 3 with get, set
    member val _chunkSubdivisions: int = 3 with get, set

    override this._Ready() =
        GD.Print "HexPlanetManagerFS _Ready" // 不能理解为什么这个在 [Tool] 中第一次总是打印两次
        init ()
