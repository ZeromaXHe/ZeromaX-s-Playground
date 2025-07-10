namespace TO.Domains.Functions.Chunks

open System
open Godot
open TO.Domains.Types.Chunks

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-19 19:30:19
module private HexMesh =
    let weights1 = Colors.Red
    let weights2 = Colors.Green
    let weights3 = Colors.Blue
    let triArr<'T> (c: 'T) = [| c; c; c |]
    let quadArr<'T> (c: 'T) = [| c; c; c; c |]
    let quad2Arr<'T> (c1: 'T) (c2: 'T) = [| c1; c1; c2; c2 |]

    let quadUv (uMin: float32) (uMax: float32) (vMin: float32) (vMax: float32) =
        [| Vector2(uMin, vMin)
           Vector2(uMax, vMin)
           Vector2(uMin, vMax)
           Vector2(uMax, vMax) |]

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-30 16:01:30
module private HexMeshCommand =
    let clear (this: IHexMesh) =
        // 清理之前的碰撞体
        for child in this.GetChildren() do
            child.QueueFree()

        this.SurfaceTool.Begin Mesh.PrimitiveType.Triangles

        if not this.Smooth then
            this.SurfaceTool.SetSmoothGroup UInt32.MaxValue

        if this.UseCellData then
            this.SurfaceTool.SetCustomFormat(0, SurfaceTool.CustomFormat.RgbFloat)

    let apply (this: IHexMesh) =
        this.SurfaceTool.GenerateNormals()
        this.Mesh <- this.SurfaceTool.Commit()
        // 仅在游戏中生成碰撞体
        if not <| Engine.IsEditorHint() && this.UseCollider then
            this.CreateTrimeshCollision()

        this.SurfaceTool.Clear() // 释放 SurfaceTool 中的内存
        this.VIdx <- 0

    let showMesh (mesh: Mesh) (this: IHexMesh) =
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
                staticBody <- this.GetChild 0 :?> StaticBody3D
                collision <- staticBody.GetChild 0 :?> CollisionShape3D

            collision.Shape <- mesh.CreateTrimeshShape()
