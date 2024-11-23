namespace FrontEndToolFS.Tool

open FrontEndToolFS.HexPlane
open Godot

type HexCellFS() =
    inherit MeshInstance3D()

    [<DefaultValue>]
    val mutable Coordinates: HexCoordinates
    
    let addTriangle v1 v2 v3 (surfaceTool: SurfaceTool) vIndex =
        surfaceTool.AddVertex v1
        surfaceTool.AddVertex v2
        surfaceTool.AddVertex v3
        surfaceTool.AddIndex <| vIndex
        surfaceTool.AddIndex <| vIndex + 1
        surfaceTool.AddIndex <| vIndex + 2
        vIndex + 3

    override this._Ready() =
        let surfaceTool = new SurfaceTool()
        surfaceTool.Begin(Mesh.PrimitiveType.Triangles)
        surfaceTool.SetUV Vector2.Zero

        // Godot 渲染面方向和 Unity 相反
        for i in 0..5 do
            addTriangle
                Vector3.Zero
                HexMetrics.conners[i + 1]
                HexMetrics.conners[i]
                surfaceTool
                (i * 3)
            |> ignore

        surfaceTool.GenerateNormals()
        this.Mesh <- surfaceTool.Commit()
