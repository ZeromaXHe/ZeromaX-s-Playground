namespace TO.Domains.Types.Chunks

open System.Runtime.InteropServices
open Godot
open TO.Domains.Types.Godots

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-29 13:38:29
[<Interface>]
type IHexMesh =
    inherit IMeshInstance3D
    // =====【Export】=====
    abstract UseCollider: bool with get
    abstract UseCellData: bool with get
    abstract UseUvCoordinates: bool with get
    abstract UseUv2Coordinates: bool with get
    abstract Smooth: bool with get
    // =====【普通属性】=====
    abstract SurfaceTool: SurfaceTool with get, set
    abstract VIdx: int with get, set

    abstract AddTriangle:
        vs: Vector3 array *
        [<Optional; DefaultParameterValue(null: Color array)>] tws: Color array *
        [<Optional; DefaultParameterValue(null: Vector2 array)>] uvs: Vector2 array *
        [<Optional; DefaultParameterValue(null: Vector2 array)>] uvs2: Vector2 array *
        [<Optional; DefaultParameterValue(Vector3())>] tis: Vector3 ->
            unit

    abstract AddQuad:
        vs: Vector3 array *
        [<Optional; DefaultParameterValue(null: Color array)>] tws: Color array *
        [<Optional; DefaultParameterValue(null: Vector2 array)>] uvs: Vector2 array *
        [<Optional; DefaultParameterValue(null: Vector2 array)>] uvs2: Vector2 array *
        [<Optional; DefaultParameterValue(Vector3())>] tis: Vector3 ->
            unit
