namespace FrontEndCommonFS.DataStructure

open Godot

// TODO：现在的实现有些问题，比如：插入的时候不会检查非常近的点的去重；Vector3 不是 comparison，所以只能用 float32 三元组代替
module Octree =
    let maxDepth = 5

    let maxPointsPerLeaf = 6

    type Leaf<'v> =
        { BackBottomLeft: Vector3
          FrontUpperRight: Vector3
          Center: Vector3
          Size: Vector3
          Depth: int
          // 叶子节点存储点坐标
          Points: Map<float32 * float32 * float32, 'v> }

    type Node<'v> =
        { BackBottomLeft: Vector3
          FrontUpperRight: Vector3
          Center: Vector3
          Size: Vector3
          Depth: int
          // 中间节点存储子八叉树
          // 现在命名是按 Unity 版的叫法，实际 Godot 里面 front 和 back 和命名是反的
          BblOctree: Tree<'v>
          FblOctree: Tree<'v>
          BtlOctree: Tree<'v>
          FtlOctree: Tree<'v>
          BbrOctree: Tree<'v>
          FbrOctree: Tree<'v>
          BtrOctree: Tree<'v>
          FtrOctree: Tree<'v> }

    and Tree<'v> =
        | Leaf of Leaf<'v>
        | Node of Node<'v>

    /// 初始化叶子节点
    let init backBottomLeft frontUpperRight depth points : Leaf<'v> =
        { BackBottomLeft = backBottomLeft
          FrontUpperRight = frontUpperRight
          Center = (backBottomLeft + frontUpperRight) / 2.0f
          Size = frontUpperRight - backBottomLeft
          Depth = depth
          Points = points }

    /// 默认初始化叶子节点
    /// 深度为 0，Points 为空
    let initDefault backBottomLeft frontUpperRight : Tree<'v> =
        Leaf <| init backBottomLeft frontUpperRight 0 Map.empty

    /// 分割叶子节点
    let split (leaf: Leaf<'v>) =
        let xOffset = leaf.Size.X / 2.0f * Vector3.Right
        let yOffset = leaf.Size.Y / 2.0f * Vector3.Up
        let zOffset = leaf.Size.Z / 2.0f * Vector3.Back

        // 现在命名是按 Unity 版的叫法，实际 Godot 里面 front 和 back 和命名是反的
        let bblOctree =
            init
                leaf.BackBottomLeft
                leaf.Center
                (leaf.Depth + 1)
                (leaf.Points
                 |> Map.filter (fun (x, y, z) _ -> x <= leaf.Center.X && y <= leaf.Center.Y && z <= leaf.Center.Z))

        let fblOctree =
            init
                (leaf.BackBottomLeft + zOffset)
                (leaf.Center + zOffset)
                (leaf.Depth + 1)
                (leaf.Points
                 |> Map.filter (fun (x, y, z) _ -> x <= leaf.Center.X && y <= leaf.Center.Y && z > leaf.Center.Z))

        let btlOctree =
            init
                (leaf.BackBottomLeft + yOffset)
                (leaf.Center + yOffset)
                (leaf.Depth + 1)
                (leaf.Points
                 |> Map.filter (fun (x, y, z) _ -> x <= leaf.Center.X && y > leaf.Center.Y && z <= leaf.Center.Z))

        let ftlOctree =
            init
                (leaf.BackBottomLeft + yOffset + zOffset)
                (leaf.Center + yOffset + zOffset)
                (leaf.Depth + 1)
                (leaf.Points
                 |> Map.filter (fun (x, y, z) _ -> x <= leaf.Center.X && y > leaf.Center.Y && z > leaf.Center.Z))

        let bbrOctree =
            init
                (leaf.BackBottomLeft + xOffset)
                (leaf.Center + xOffset)
                (leaf.Depth + 1)
                (leaf.Points
                 |> Map.filter (fun (x, y, z) _ -> x > leaf.Center.X && y <= leaf.Center.Y && z <= leaf.Center.Z))

        let fbrOctree =
            init
                (leaf.BackBottomLeft + xOffset + zOffset)
                (leaf.Center + xOffset + zOffset)
                (leaf.Depth + 1)
                (leaf.Points
                 |> Map.filter (fun (x, y, z) _ -> x > leaf.Center.X && y <= leaf.Center.Y && z > leaf.Center.Z))

        let btrOctree =
            init
                (leaf.BackBottomLeft + xOffset + yOffset)
                (leaf.Center + xOffset + yOffset)
                (leaf.Depth + 1)
                (leaf.Points
                 |> Map.filter (fun (x, y, z) _ -> x > leaf.Center.X && y > leaf.Center.Y && z <= leaf.Center.Z))

        let ftrOctree =
            init
                (leaf.BackBottomLeft + xOffset + yOffset + zOffset)
                (leaf.Center + xOffset + yOffset + zOffset)
                (leaf.Depth + 1)
                (leaf.Points
                 |> Map.filter (fun (x, y, z) _ -> x > leaf.Center.X && y > leaf.Center.Y && z > leaf.Center.Z))

        { BackBottomLeft = leaf.BackBottomLeft
          FrontUpperRight = leaf.FrontUpperRight
          Center = leaf.Center
          Size = leaf.Size
          Depth = leaf.Depth
          BblOctree = Leaf bblOctree
          FblOctree = Leaf fblOctree
          BtlOctree = Leaf btlOctree
          FtlOctree = Leaf ftlOctree
          BbrOctree = Leaf bbrOctree
          FbrOctree = Leaf fbrOctree
          BtrOctree = Leaf btrOctree
          FtrOctree = Leaf ftrOctree }

    /// 插入点
    let rec insertPoint key point =
        function
        | Leaf leaf ->
            if leaf.Points.Count < maxPointsPerLeaf || leaf.Depth >= maxDepth then
                Leaf
                    { leaf with
                        Points = leaf.Points.Add(key, point) }
            else
                let node = split leaf
                insertPointToNode key point node
        | Node node -> insertPointToNode key point node

    and insertPointToNode key point node =
        let x, y, z = key
        // 注意：Unity 和 Godot 前后是反的，Godot 的 Back，Unity 的 Front
        if x > node.Center.X then
            if y > node.Center.Y then
                if z > node.Center.Z then
                    Node
                        { node with
                            FtrOctree = insertPoint key point node.FtrOctree }
                else // z <= node.Center.Z
                    Node
                        { node with
                            BtrOctree = insertPoint key point node.BtrOctree }
            else if z > node.Center.Z then // y <= node.Center.Y
                Node
                    { node with
                        FbrOctree = insertPoint key point node.FbrOctree }
            else
                Node
                    { node with
                        BbrOctree = insertPoint key point node.BbrOctree }
        else if y > node.Center.Y then // x <= node.Center.X
            if z > node.Center.Z then
                Node
                    { node with
                        FtlOctree = insertPoint key point node.FtlOctree }
            else
                Node
                    { node with
                        BtlOctree = insertPoint key point node.BtlOctree }
        else if z > node.Center.Z then
            Node
                { node with
                    FblOctree = insertPoint key point node.FblOctree }
        else
            Node
                { node with
                    BblOctree = insertPoint key point node.BblOctree }

    let boxIntersectsBox (boxACenter: Vector3) (boxASize: Vector3) (boxBCenter: Vector3) (boxBSize: Vector3) =
        boxACenter.X - boxASize.X <= boxBCenter.X + boxBSize.X
        && boxACenter.X + boxASize.X >= boxBCenter.X - boxBSize.X
        && boxACenter.Y - boxASize.Y <= boxBCenter.Y + boxBSize.Y
        && boxACenter.Y + boxASize.Y >= boxBCenter.Y - boxBSize.Y
        && boxACenter.Z - boxASize.Z <= boxBCenter.Z + boxBSize.Z
        && boxACenter.Z + boxASize.Z >= boxBCenter.Z - boxBSize.Z

    let pointWithinBox (boxCenter: Vector3) (boxSize: Vector3) point =
        let x, y, z = point

        x <= boxCenter.X + boxSize.X
        && x >= boxCenter.X - boxSize.X
        && y <= boxCenter.Y + boxSize.Y
        && y >= boxCenter.Y - boxSize.Y
        && z <= boxCenter.Z + boxSize.Z
        && z >= boxCenter.Z - boxSize.Z

    /// 在指定中心和尺寸内查找所有点
    let rec getPoints center size =
        function
        | Leaf octree when not <| boxIntersectsBox center size octree.Center octree.Size -> Seq.empty
        | Leaf leaf ->
            leaf.Points
            |> Map.filter (fun k _ -> pointWithinBox center size k)
            |> Map.toSeq
            |> Seq.map snd
        | Node octree when not <| boxIntersectsBox center size octree.Center octree.Size -> Seq.empty
        | Node node ->
            getPoints center size node.BblOctree |> Seq.append
            <| getPoints center size node.FblOctree
            |> Seq.append
            <| getPoints center size node.BtlOctree
            |> Seq.append
            <| getPoints center size node.FtlOctree
            |> Seq.append
            <| getPoints center size node.BbrOctree
            |> Seq.append
            <| getPoints center size node.FbrOctree
            |> Seq.append
            <| getPoints center size node.BtrOctree
            |> Seq.append
            <| getPoints center size node.FtrOctree
