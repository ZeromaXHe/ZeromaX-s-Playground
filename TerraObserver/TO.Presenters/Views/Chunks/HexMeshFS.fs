namespace TO.Presenters.Views.Chunks

open System
open Godot

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-22 17:16:22
[<AbstractClass>]
type HexMeshFS() =
    inherit MeshInstance3D()
    let mutable vIdx = 0
    let surfaceTool = new SurfaceTool()
    // =====【Export】=====
    abstract UseCollider: bool with get, set
    abstract UseCellData: bool with get, set
    abstract UseUvCoordinates: bool with get, set
    abstract UseUv2Coordinates: bool with get, set
    abstract Smooth: bool with get, set

    member this.Clear() =
        // 清理之前的碰撞体
        for child in this.GetChildren() do
            child.QueueFree()

        surfaceTool.Begin Mesh.PrimitiveType.Triangles

        if not this.Smooth then
            surfaceTool.SetSmoothGroup UInt32.MaxValue

        if this.UseCellData then
            surfaceTool.SetCustomFormat(0, SurfaceTool.CustomFormat.RgbFloat)

    member this.Apply() =
        surfaceTool.GenerateNormals()
        this.Mesh <- surfaceTool.Commit()
        // 仅在游戏中生成碰撞体
        if not <| Engine.IsEditorHint() && this.UseCollider then
            this.CreateTrimeshCollision()

        surfaceTool.Clear() // 释放 SurfaceTool 中的内存
        vIdx <- 0

    member this.ShowMesh(mesh: Mesh) =
        this.Mesh <- mesh

        if this.UseCollider then
            // 更新碰撞体网格
            let mutable staticBody: StaticBody3D = null
            let mutable collision: CollisionShape3D = null

            if this.GetChildCount() = 0 then
                staticBody <- new StaticBody3D()
                this.AddChild staticBody
                collision <- new CollisionShape3D()
                staticBody.AddChild collision
            else
                staticBody <- this.GetChild<StaticBody3D>(0)
                collision <- staticBody.GetChild<CollisionShape3D>(0)

            collision.Shape <- mesh.CreateTrimeshShape()

    /// <summary>
    /// 绘制三角形
    /// </summary>
    /// <param name="vs">顶点数组 vertices</param>
    /// <param name="tws">地块权重 tWeights</param>
    /// <param name="uvs">UV</param>
    /// <param name="uvs2">UV2</param>
    /// <param name="tis">地块ID tileIds</param>
    member this.AddTriangle
        (vs: Vector3 array, tws: Color array, uvs: Vector2 array, uvs2: Vector2 array, tis: Vector3)
        =
        for i in 0..2 do
            if this.UseCellData && tws <> null then
                surfaceTool.SetColor tws[i]
                surfaceTool.SetCustom(0, Color(tis.X, tis.Y, tis.Z))

            if this.UseUvCoordinates && uvs <> null then
                surfaceTool.SetUV uvs[i]

            if this.UseUv2Coordinates && uvs2 <> null then
                surfaceTool.SetUV2 uvs2[i]

            surfaceTool.AddVertex vs[i]

        surfaceTool.AddIndex vIdx
        surfaceTool.AddIndex <| vIdx + 1
        surfaceTool.AddIndex <| vIdx + 2
        vIdx <- vIdx + 3

    // 用于实现 IHexMesh 接口，vs 后面的参数在接口中定义了默认参数
    member this.AddQuad(vs: Vector3 array, tws: Color array, uvs: Vector2 array, uvs2: Vector2 array, tis: Vector3) =
        for i in 0..3 do
            if this.UseCellData && tws <> null then
                surfaceTool.SetColor tws[i]
                surfaceTool.SetCustom(0, Color(tis.X, tis.Y, tis.Z))

            if this.UseUvCoordinates && uvs <> null then
                surfaceTool.SetUV uvs[i]

            if this.UseUv2Coordinates && uvs2 <> null then
                surfaceTool.SetUV2 uvs2[i]

            surfaceTool.AddVertex vs[i]

        surfaceTool.AddIndex vIdx
        surfaceTool.AddIndex <| vIdx + 2
        surfaceTool.AddIndex <| vIdx + 1
        surfaceTool.AddIndex <| vIdx + 1
        surfaceTool.AddIndex <| vIdx + 2
        surfaceTool.AddIndex <| vIdx + 3
        vIdx <- vIdx + 4
