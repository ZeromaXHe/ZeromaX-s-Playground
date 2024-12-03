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
    member val useColor = false with get, set
    member val useUvCoordinates = false with get, set
    member val useUv2Coordinates = false with get, set

    /// 未扰动的三角形
    member this.AddTriangleUnperturbed(vs: Vector3 array, ?cs: Color array, ?uvs: Vector2 array, ?uv2s: Vector2 array) =
        let cs = defaultArg cs Array.empty
        let uvs = defaultArg uvs Array.empty
        let uv2s = defaultArg uv2s Array.empty

        if this.useColor then
            this.surfaceTool.SetColor cs[0]

        if this.useUvCoordinates then
            this.surfaceTool.SetUV uvs[0]

        if this.useUv2Coordinates then
            this.surfaceTool.SetUV2 uv2s[0]

        this.surfaceTool.AddVertex vs[0]

        if this.useColor then
            this.surfaceTool.SetColor cs[1]

        if this.useUvCoordinates then
            this.surfaceTool.SetUV uvs[1]

        if this.useUv2Coordinates then
            this.surfaceTool.SetUV2 uv2s[1]

        this.surfaceTool.AddVertex vs[1]

        if this.useColor then
            this.surfaceTool.SetColor cs[2]

        if this.useUvCoordinates then
            this.surfaceTool.SetUV uvs[2]

        if this.useUv2Coordinates then
            this.surfaceTool.SetUV2 uv2s[2]

        this.surfaceTool.AddVertex vs[2]
        // Godot 渲染面方向和 Unity 相反
        this.surfaceTool.AddIndex <| vIndex
        this.surfaceTool.AddIndex <| vIndex + 2
        this.surfaceTool.AddIndex <| vIndex + 1
        vIndex <- vIndex + 3

    /// 三角形
    member this.AddTriangle(vs: Vector3 array, ?colors, ?uvs, ?uv2s) =
        let colors = defaultArg colors Array.empty
        let uvs = defaultArg uvs Array.empty
        let uv2s = defaultArg uv2s Array.empty
        let perturbVertices = vs |> Array.map HexMetrics.perturb
        this.AddTriangleUnperturbed(perturbVertices, colors, uvs, uv2s)

    /// 未扰动的四边形
    member this.AddQuadUnperturbed(vs: Vector3 array, ?cs: Color array, ?uvs: Vector2 array, ?uv2s: Vector2 array) =
        let cs = defaultArg cs Array.empty
        let uvs = defaultArg uvs Array.empty
        let uv2s = defaultArg uv2s Array.empty

        if this.useColor then
            this.surfaceTool.SetColor cs[0]

        if this.useUvCoordinates then
            this.surfaceTool.SetUV uvs[0]

        if this.useUv2Coordinates then
            this.surfaceTool.SetUV2 uv2s[0]

        this.surfaceTool.AddVertex vs[0]

        if this.useColor then
            this.surfaceTool.SetColor cs[1]

        if this.useUvCoordinates then
            this.surfaceTool.SetUV uvs[1]

        if this.useUv2Coordinates then
            this.surfaceTool.SetUV2 uv2s[1]

        this.surfaceTool.AddVertex vs[1]

        if this.useColor then
            this.surfaceTool.SetColor cs[2]

        if this.useUvCoordinates then
            this.surfaceTool.SetUV uvs[2]

        if this.useUv2Coordinates then
            this.surfaceTool.SetUV2 uv2s[2]

        this.surfaceTool.AddVertex vs[2]

        if this.useColor then
            this.surfaceTool.SetColor cs[3]

        if this.useUvCoordinates then
            this.surfaceTool.SetUV uvs[3]

        if this.useUv2Coordinates then
            this.surfaceTool.SetUV2 uv2s[3]

        this.surfaceTool.AddVertex vs[3]
        // Godot 渲染面方向和 Unity 相反
        this.surfaceTool.AddIndex <| vIndex
        this.surfaceTool.AddIndex <| vIndex + 1
        this.surfaceTool.AddIndex <| vIndex + 2
        this.surfaceTool.AddIndex <| vIndex + 2
        this.surfaceTool.AddIndex <| vIndex + 1
        this.surfaceTool.AddIndex <| vIndex + 3
        vIndex <- vIndex + 4

    /// 四边形
    member this.AddQuad(vs: Vector3 array, ?cs, ?uvs, ?uv2s) =
        let colors = defaultArg cs Array.empty
        let uvs = defaultArg uvs Array.empty
        let uv2s = defaultArg uv2s Array.empty
        let perturbVertices = vs |> Array.map HexMetrics.perturb
        this.AddQuadUnperturbed(perturbVertices, colors, uvs, uv2s)

    member this.Clear() =
        this.surfaceTool <- new SurfaceTool()
        vIndex <- 0
        this.surfaceTool.Begin(Mesh.PrimitiveType.Triangles)

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
