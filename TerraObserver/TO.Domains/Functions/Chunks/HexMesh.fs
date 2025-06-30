namespace TO.Domains.Functions.Chunks

open System
open Godot
open TO.Domains.Types.Chunks

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-30 16:01:30
module HexMeshCommand =
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
