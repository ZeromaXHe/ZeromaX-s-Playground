namespace FrontEndToolFS.Tool

open Godot

type HexChunkRendererFS() =
    inherit MeshInstance3D()

    [<DefaultValue>]
    val mutable _renderedChunkId: int
    
    member this.UpdateMesh mesh =
        this.Mesh <- mesh

    override this._Ready() =
        // 好像 F# 的 _Ready 不会在假如场景树时自动调用？
        GD.Print $"Chunk {this._renderedChunkId} _ready"
        // this.UpdateMesh()