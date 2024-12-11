namespace FrontEndToolFS.Tool

open FrontEndToolFS.HexPlane
open Godot
open Microsoft.FSharp.Core

type HexMeshFS() =
    inherit MeshInstance3D()

    let mutable vIndex = 0

    [<DefaultValue>]
    val mutable private surfaceTool: SurfaceTool

    member val useCollider = false with get, set
    member val useCellData = false with get, set
    member val useUvCoordinates = false with get, set
    member val useUv2Coordinates = false with get, set

    /// 未扰动的三角形
    member this.AddTriangleUnperturbed
        (
            vs: Vector3 array,
            ?cw: Color array,
            ?uvs: Vector2 array,
            ?uv2s: Vector2 array,
            ?ci: Vector3
        ) =
        let cellWeights = defaultArg cw Array.empty
        let uvs = defaultArg uvs Array.empty
        let uv2s = defaultArg uv2s Array.empty
        let cellIndices = defaultArg ci Vector3.Zero

        let addVertex index =
            if this.useCellData then
                this.surfaceTool.SetColor cellWeights[index]
                this.surfaceTool.SetCustom(0, Color(cellIndices.X, cellIndices.Y, cellIndices.Z))

            if this.useUvCoordinates then
                this.surfaceTool.SetUV uvs[index]

            if this.useUv2Coordinates then
                this.surfaceTool.SetUV2 uv2s[index]

            this.surfaceTool.AddVertex vs[index]

        [ 0..2 ] |> List.iter addVertex
        // Godot 渲染面方向和 Unity 相反
        this.surfaceTool.AddIndex <| vIndex
        this.surfaceTool.AddIndex <| vIndex + 2
        this.surfaceTool.AddIndex <| vIndex + 1
        vIndex <- vIndex + 3

    /// 三角形
    member this.AddTriangle(vs: Vector3 array, ?cw, ?uvs, ?uv2s, ?ci) =
        let cellWeights = defaultArg cw Array.empty
        let uvs = defaultArg uvs Array.empty
        let uv2s = defaultArg uv2s Array.empty
        let cellIndices = defaultArg ci Vector3.Zero
        let perturbVertices = vs |> Array.map HexMetrics.perturb
        this.AddTriangleUnperturbed(perturbVertices, cellWeights, uvs, uv2s, cellIndices)

    /// 未扰动的四边形
    member this.AddQuadUnperturbed
        (
            vs: Vector3 array,
            ?cw: Color array,
            ?uvs: Vector2 array,
            ?uv2s: Vector2 array,
            ?ci: Vector3
        ) =
        let cellWeights = defaultArg cw Array.empty
        let uvs = defaultArg uvs Array.empty
        let uv2s = defaultArg uv2s Array.empty
        let cellIndices = defaultArg ci Vector3.Zero

        let addVertex index =
            if this.useCellData then
                this.surfaceTool.SetColor cellWeights[index]
                this.surfaceTool.SetCustom(0, Color(cellIndices.X, cellIndices.Y, cellIndices.Z))

            if this.useUvCoordinates then
                this.surfaceTool.SetUV uvs[index]

            if this.useUv2Coordinates then
                this.surfaceTool.SetUV2 uv2s[index]

            this.surfaceTool.AddVertex vs[index]

        [ 0..3 ] |> List.iter addVertex
        // Godot 渲染面方向和 Unity 相反
        this.surfaceTool.AddIndex <| vIndex
        this.surfaceTool.AddIndex <| vIndex + 1
        this.surfaceTool.AddIndex <| vIndex + 2
        this.surfaceTool.AddIndex <| vIndex + 2
        this.surfaceTool.AddIndex <| vIndex + 1
        this.surfaceTool.AddIndex <| vIndex + 3
        vIndex <- vIndex + 4

    /// 四边形
    member this.AddQuad(vs: Vector3 array, ?cw, ?uvs, ?uv2s, ?ci) =
        let cellWeights = defaultArg cw Array.empty
        let uvs = defaultArg uvs Array.empty
        let uv2s = defaultArg uv2s Array.empty
        let cellIndices = defaultArg ci Vector3.Zero
        let perturbVertices = vs |> Array.map HexMetrics.perturb
        this.AddQuadUnperturbed(perturbVertices, cellWeights, uvs, uv2s, cellIndices)

    member this.Clear() =
        this.surfaceTool <- new SurfaceTool()
        vIndex <- 0
        this.surfaceTool.Begin(Mesh.PrimitiveType.Triangles)

        if this.useCellData then
            this.surfaceTool.SetCustomFormat(0, SurfaceTool.CustomFormat.RgbFloat)

    member this.Apply() =
        // this.surfaceTool.Deindex()
        this.surfaceTool.GenerateNormals()
        // this.surfaceTool.GenerateTangents()
        // let material = new StandardMaterial3D()
        // material.VertexColorUseAsAlbedo <- true
        // material.Roughness <- 0.5f
        // material.Metallic <- 0.0f
        // surfaceTool.SetMaterial(material)
        this.Mesh <- this.surfaceTool.Commit()
        // 仅在游戏中生成碰撞体（不然编辑器里执行会给场景新增 StaticBody 子节点，以及下面的碰撞体）
        if not <| Engine.IsEditorHint() && this.useCollider then
            this.GetChildren() |> Seq.iter _.QueueFree() // 清理之前生成的碰撞体
            this.CreateTrimeshCollision()
