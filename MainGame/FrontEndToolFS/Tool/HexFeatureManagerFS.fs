namespace FrontEndToolFS.Tool

open FrontEndToolFS.HexPlane
open Godot
open Godot.Collections

type HexFeatureManagerFS() as this =
    inherit Node3D()

    let mutable _container: Node3D = null

    let pickPrefab (prefabs: PackedScene Array Array) level hash choice =
        if level > 0 then
            HexMetrics.getFeatureThresholds <| level - 1
            |> Array.tryFindIndex (fun threshold -> hash < threshold)
            |> Option.map (fun i -> prefabs[i][int <| choice * float32 prefabs[i].Count])
        else
            None

    /// 特征场景
    /// 这种嵌套的数组不能用 .NET 的 array，必须用 Godot.Collections.Array。否则编辑器编译不通过
    /// 而且这样写以后，其实编辑器里是判断不了 PackedScene 类型的，必须自己手动选成 Object（Godot.GodotObject 或其他派生类型）再用。
    [<DefaultValue>]
    val mutable urbanPrefabs: PackedScene Array Array

    [<DefaultValue>]
    val mutable farmPrefabs: PackedScene Array Array

    [<DefaultValue>]
    val mutable plantPrefabs: PackedScene Array Array

    member this.Clear() =
        if _container <> null then
            _container.QueueFree()

        _container <- new Node3D()
        this.AddChild _container

    member this.Apply() = ()

    member this.AddFeature (cell: HexCellFS) (position: Vector3) =
        let hash = HexMetrics.sampleHashGrid position

        let mutable usedHash = hash.a
        let mutable prefab = pickPrefab this.urbanPrefabs cell.UrbanLevel hash.a hash.d
        let farmOpt = pickPrefab this.farmPrefabs cell.FarmLevel hash.b hash.d

        if prefab.IsSome then
            if farmOpt.IsSome && hash.b < usedHash then
                prefab <- farmOpt
                usedHash <- hash.b
        elif farmOpt.IsSome then
            prefab <- farmOpt
            usedHash <- hash.b

        let plantOpt = pickPrefab this.plantPrefabs cell.PlantLevel hash.c hash.d

        if prefab.IsSome then
            if plantOpt.IsSome && hash.c < usedHash then
                prefab <- plantOpt
        elif plantOpt.IsSome then
            prefab <- plantOpt

        prefab
        |> Option.iter (fun prefab ->
            let cube = prefab.Instantiate<CsgBox3D>()
            cube.Position <- HexMetrics.perturb <| position + Vector3.Up * 0.5f * cube.Size
            cube.RotationDegrees <- Vector3(0f, 360f * hash.e, 0f)
            _container.AddChild cube)
