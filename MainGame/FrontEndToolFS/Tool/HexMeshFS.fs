namespace FrontEndToolFS.Tool

open FrontEndToolFS.HexPlane
open Godot
open Microsoft.FSharp.Core

type HexMeshFS() =
    inherit MeshInstance3D()

    let mutable vIndex = 0

    [<DefaultValue>]
    val mutable private surfaceTool: SurfaceTool

    member val useCollider = true with get, set
    member val useColor = true with get, set
    member val useUvCoordinates = false with get, set

    /// 未扰动的三角形
    member this.AddTriangleUnperturbed (vertices: Vector3 array) (colors: Color array) (uvs: Vector2 array) =

        if this.useColor then
            this.surfaceTool.SetColor colors[0]

        if this.useUvCoordinates then
            this.surfaceTool.SetUV uvs[0]

        this.surfaceTool.AddVertex vertices[0]

        if this.useColor then
            this.surfaceTool.SetColor colors[1]

        if this.useUvCoordinates then
            this.surfaceTool.SetUV uvs[1]

        this.surfaceTool.AddVertex vertices[1]

        if this.useColor then
            this.surfaceTool.SetColor colors[2]

        if this.useUvCoordinates then
            this.surfaceTool.SetUV uvs[2]

        this.surfaceTool.AddVertex vertices[2]
        // Godot 渲染面方向和 Unity 相反
        this.surfaceTool.AddIndex <| vIndex
        this.surfaceTool.AddIndex <| vIndex + 2
        this.surfaceTool.AddIndex <| vIndex + 1
        vIndex <- vIndex + 3

    /// 三角形
    member this.AddTriangle (vertices: Vector3 array) (colors: Color array) (uvs: Vector2 array) =
        if this.useColor then
            this.surfaceTool.SetColor colors[0]

        if this.useUvCoordinates then
            this.surfaceTool.SetUV uvs[0]

        this.surfaceTool.AddVertex <| HexMetrics.perturb vertices[0]

        if this.useColor then
            this.surfaceTool.SetColor colors[1]

        if this.useUvCoordinates then
            this.surfaceTool.SetUV uvs[1]

        this.surfaceTool.AddVertex <| HexMetrics.perturb vertices[1]

        if this.useColor then
            this.surfaceTool.SetColor colors[2]

        if this.useUvCoordinates then
            this.surfaceTool.SetUV uvs[2]

        this.surfaceTool.AddVertex <| HexMetrics.perturb vertices[2]
        // Godot 渲染面方向和 Unity 相反
        this.surfaceTool.AddIndex <| vIndex
        this.surfaceTool.AddIndex <| vIndex + 2
        this.surfaceTool.AddIndex <| vIndex + 1
        vIndex <- vIndex + 3

    /// 四边形
    member this.AddQuad (vertices: Vector3 array) (colors: Color array) (uvs: Vector2 array) =
        if this.useColor then
            this.surfaceTool.SetColor colors[0]

        if this.useUvCoordinates then
            this.surfaceTool.SetUV uvs[0]

        this.surfaceTool.AddVertex <| HexMetrics.perturb vertices[0]

        if this.useColor then
            this.surfaceTool.SetColor colors[1]

        if this.useUvCoordinates then
            this.surfaceTool.SetUV uvs[1]

        this.surfaceTool.AddVertex <| HexMetrics.perturb vertices[1]

        if this.useColor then
            this.surfaceTool.SetColor colors[2]

        if this.useUvCoordinates then
            this.surfaceTool.SetUV uvs[2]

        this.surfaceTool.AddVertex <| HexMetrics.perturb vertices[2]

        if this.useColor then
            this.surfaceTool.SetColor colors[3]

        if this.useUvCoordinates then
            this.surfaceTool.SetUV uvs[3]

        this.surfaceTool.AddVertex <| HexMetrics.perturb vertices[3]
        // Godot 渲染面方向和 Unity 相反
        this.surfaceTool.AddIndex <| vIndex
        this.surfaceTool.AddIndex <| vIndex + 1
        this.surfaceTool.AddIndex <| vIndex + 2
        this.surfaceTool.AddIndex <| vIndex + 2
        this.surfaceTool.AddIndex <| vIndex + 1
        this.surfaceTool.AddIndex <| vIndex + 3
        vIndex <- vIndex + 4

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
