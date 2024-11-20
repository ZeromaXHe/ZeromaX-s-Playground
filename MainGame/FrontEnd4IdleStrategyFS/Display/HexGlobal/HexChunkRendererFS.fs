namespace FrontEnd4IdleStrategyFS.Display.HexGlobal

open FrontEnd4IdleStrategyFS.Global
open Godot

type HexChunkRendererFS() as this =
    inherit MeshInstance3D()

    let _globalNode = lazy this.GetNode<GlobalNodeFS> "/root/GlobalNode"

    [<DefaultValue>]
    val mutable _renderedChunkId: int
    
    member this.UpdateMesh() =
        this.Mesh <- _globalNode.Value.HexGlobalEntry.Value.GetHexChunkMesh this._renderedChunkId

    override this._Ready() =
        // 好像 F# 的 _Ready 不会在假如场景树时自动调用？
        GD.Print $"Chunk {this._renderedChunkId} _ready"
        this.UpdateMesh()