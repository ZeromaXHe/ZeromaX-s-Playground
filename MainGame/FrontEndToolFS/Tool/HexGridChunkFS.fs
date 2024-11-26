namespace FrontEndToolFS.Tool

open FrontEndToolFS.HexPlane
open Godot

type HexGridChunkFS() as this =
    inherit Node3D()

    let _hexMesh = lazy this.GetNode<HexMeshFS>("HexMesh")
    let mutable cells: HexCellFS array = null
    let mutable anyCell = false

    interface IChunk with
        member this.Refresh() =
            // GD.Print $"{this.Name} Refresh"
            this.SetProcess true

    member this.AddCell index cell =
        anyCell <- true
        cells[index] <- cell
        cell.Chunk <- Some this
        this.AddChild cell

    override this._Ready() =
        cells <- Array.zeroCreate (HexMetrics.chunkSizeX * HexMetrics.chunkSizeZ)

    override this._Process _ =
        // 防止编辑器内循环报错，卡死
        if anyCell then
            _hexMesh.Value.Triangulate cells
        // 这里写法挺有意思，可以控制 _Process 不频繁调用
        this.SetProcess false
