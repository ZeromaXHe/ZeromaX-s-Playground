namespace FrontEndToolFS.Tool

open FrontEndCommonFS.Util
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

    /// 墙
    [<DefaultValue>]
    val mutable walls: HexMeshFS

    /// 塔楼
    [<DefaultValue>]
    val mutable wallTower: PackedScene

    /// 桥梁
    [<DefaultValue>]
    val mutable bridge: PackedScene

    /// 特殊特征
    [<DefaultValue>]
    val mutable special: PackedScene array

    let addWallCap near far =
        let near = HexMetrics.perturb near
        let far = HexMetrics.perturb far
        let center = HexMetrics.wallLerp near far
        let thickness = HexMetrics.wallThicknessOffset near far
        let v1 = center - thickness
        let v2 = center + thickness
        let v3 = Vector3Util.changeY v1 <| center.Y + HexMetrics.wallHeight
        let v4 = Vector3Util.changeY v2 <| center.Y + HexMetrics.wallHeight
        this.walls.AddQuadUnperturbed [| v1; v2; v3; v4 |]

    let addWallWedge near far point =
        let near = HexMetrics.perturb near
        let far = HexMetrics.perturb far
        let point = HexMetrics.perturb point
        let center = HexMetrics.wallLerp near far
        let thickness = HexMetrics.wallThicknessOffset near far
        let pointTop = Vector3Util.changeY point <| center.Y + HexMetrics.wallHeight
        let point = Vector3Util.changeY point center.Y
        let v1 = center - thickness
        let v2 = center + thickness
        let v3 = Vector3Util.changeY v1 pointTop.Y
        let v4 = Vector3Util.changeY v2 pointTop.Y
        this.walls.AddQuadUnperturbed [| v1; point; v3; pointTop |]
        this.walls.AddQuadUnperturbed [| point; v2; pointTop; v4 |]
        this.walls.AddTriangleUnperturbed [| pointTop; v3; v4 |]

    let addWallSegment nearLeft farLeft nearRight farRight addTower =
        let nearLeft = HexMetrics.perturb nearLeft
        let farLeft = HexMetrics.perturb farLeft
        let nearRight = HexMetrics.perturb nearRight
        let farRight = HexMetrics.perturb farRight

        let left = HexMetrics.wallLerp nearLeft farLeft
        let right = HexMetrics.wallLerp nearRight farRight

        let leftThicknessOffset = HexMetrics.wallThicknessOffset nearLeft farLeft
        let rightThicknessOffset = HexMetrics.wallThicknessOffset nearRight farRight

        let leftTop = left.Y + HexMetrics.wallHeight
        let rightTop = right.Y + HexMetrics.wallHeight

        let v1 = left - leftThicknessOffset
        let v2 = right - rightThicknessOffset
        let v3 = Vector3Util.changeY v1 leftTop
        let v4 = Vector3Util.changeY v2 rightTop
        this.walls.AddQuadUnperturbed [| v1; v2; v3; v4 |]

        let t1 = v3
        let t2 = v4
        let v1 = left + leftThicknessOffset
        let v2 = right + rightThicknessOffset
        let v3 = Vector3Util.changeY v1 leftTop
        let v4 = Vector3Util.changeY v2 rightTop
        this.walls.AddQuadUnperturbed [| v2; v1; v4; v3 |]
        this.walls.AddQuadUnperturbed [| t1; t2; v3; v4 |]

        if addTower then
            let tower = this.wallTower.Instantiate<CsgBox3D>()
            tower.Position <- (left + right) * 0.5f + tower.Position
            let rightDirection = Vector3Util.changeY <| right - left <| 0f
            tower.RotateY <| rightDirection.AngleTo Vector3.Right
            _container.AddChild tower

    let addWallSegment2 pivot (pivotCell: HexCellFS) left (leftCell: HexCellFS) right (rightCell: HexCellFS) =
        if not pivotCell.IsUnderWater then
            let hasLeftWall =
                not leftCell.IsUnderWater && pivotCell.GetEdgeType leftCell <> HexEdgeType.Cliff

            let hasRightWall =
                not rightCell.IsUnderWater
                && pivotCell.GetEdgeType rightCell <> HexEdgeType.Cliff

            if hasLeftWall then
                if hasRightWall then
                    let hash = HexMetrics.sampleHashGrid <| (pivot + left + right) * (1f / 3f)

                    let hasTower =
                        leftCell.Elevation = rightCell.Elevation
                        && hash.e < HexMetrics.wallTowerThreshold

                    addWallSegment pivot left pivot right hasTower
                elif leftCell.Elevation < rightCell.Elevation then
                    addWallWedge pivot left right
                else
                    addWallCap pivot left
            elif hasRightWall then
                if rightCell.Elevation < leftCell.Elevation then
                    addWallWedge right pivot left
                else
                    addWallCap right pivot

    member this.Clear() =
        if _container <> null then
            _container.QueueFree()

        _container <- new Node3D()
        this.AddChild _container

        this.walls.Clear()

    member this.Apply() = this.walls.Apply()

    member this.AddFeature (cell: HexCellFS) (position: Vector3) =
        if not cell.IsSpecial then
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

    member this.AddWall
        (near: EdgeVertices)
        (nearCell: HexCellFS)
        (far: EdgeVertices)
        (farCell: HexCellFS)
        hasRiver
        hasRoad
        =
        if
            nearCell.Walled <> farCell.Walled
            && not nearCell.IsUnderWater
            && not farCell.IsUnderWater
            && nearCell.GetEdgeType farCell <> HexEdgeType.Cliff
        then
            addWallSegment near.v1 far.v1 near.v2 far.v2 false

            if hasRiver || hasRoad then
                addWallCap near.v2 far.v2
                addWallCap far.v4 near.v4
            else
                addWallSegment near.v2 far.v2 near.v3 far.v3 false
                addWallSegment near.v3 far.v3 near.v4 far.v4 false

            addWallSegment near.v4 far.v4 near.v5 far.v5 false

    member this.AddWall2 c1 (cell1: HexCellFS) c2 (cell2: HexCellFS) c3 (cell3: HexCellFS) =
        if cell1.Walled then
            if cell2.Walled then
                if not cell3.Walled then
                    addWallSegment2 c3 cell3 c1 cell1 c2 cell2
            elif cell3.Walled then
                addWallSegment2 c2 cell2 c3 cell3 c1 cell1
            else
                addWallSegment2 c1 cell1 c2 cell2 c3 cell3
        elif cell2.Walled then
            if cell3.Walled then
                addWallSegment2 c1 cell1 c2 cell2 c3 cell3
            else
                addWallSegment2 c2 cell2 c3 cell3 c1 cell1
        elif cell3.Walled then
            addWallSegment2 c3 cell3 c1 cell1 c2 cell2

    /// 桥
    /// <param name="roadCenter1">桥一端的中心</param>>
    /// <param name="roadCenter2">桥另一端的中心</param>>
    member this.AddBridge roadCenter1 roadCenter2 =
        let roadCenter1 = HexMetrics.perturb roadCenter1
        let roadCenter2 = HexMetrics.perturb roadCenter2
        let instance = this.bridge.Instantiate<CsgBox3D>()
        instance.Position <- (roadCenter1 + roadCenter2) * 0.5f + instance.Position
        let length = roadCenter1.DistanceTo roadCenter2
        instance.Scale <- Vector3(length * (1f / HexMetrics.bridgeDesignLength), 1f, 1f)
        _container.AddChild instance
        // instance.RotateY <| (roadCenter2 - roadCenter1).AngleTo Vector3.Right // 区分不了 1、2 哪个是外侧，不行
        // 这里因为 Unity API 和 Godot 差距较大，所以用这种方法先把桥洞对准桥的两端，再旋转 90 度
        instance.LookAt(instance.GlobalPosition + roadCenter2 - roadCenter1)
        instance.RotateY <| Mathf.Pi / 2f

    /// 特殊特征
    member this.AddSpecialFeature (cell: HexCellFS) position =
        let instance = this.special[cell.SpecialIndex - 1].Instantiate<CsgBox3D>()
        instance.Position <- HexMetrics.perturb position + instance.Position
        let hash = HexMetrics.sampleHashGrid position
        instance.RotationDegrees <- Vector3(0f, 360f * hash.e, 0f)
        _container.AddChild instance
