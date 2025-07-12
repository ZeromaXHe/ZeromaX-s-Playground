namespace TO.Domains.Functions.Units

open Godot
open TO.Domains.Functions.HexSpheres.Components
open TO.Domains.Functions.HexSpheres.Components.Tiles
open TO.Domains.Types.Configs
open TO.Domains.Types.HexSpheres
open TO.Domains.Types.HexSpheres.Components
open TO.Domains.Types.Units

module HexUnitPathQuery =
    let getUnitPathProcess (path: IHexUnitPath) = path.PathFollow.Progress

    let getUnitPathProgressTileId (path: IHexUnitPath) =
        let mutable idx =
            System.Array.BinarySearch(path.Progresses, path.PathFollow.Progress)

        if idx < 0 then
            idx <- ~~~idx

        path.TileIds[idx]

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-07-11 09:47:11
module HexUnitPathCommand =
    // 有以下问题的话，请检查 Curve3D 的生成是否不连续：
    //
    // 1. 如果出现莫名奇妙的报错：莫名其妙的 look_at 并且报错
    // 很可能是因为 Curve3D in out 参数配的不对，导致曲线不连续导致的？改小 in out 就不报错了
    // E 0:00:08:0190   looking_at: The target vector can't be zero.
    // <C++ 错误>       Condition "p_target.is_zero_approx()" is true. Returning: Basis()
    // <C++ 源文件>      core/math/basis.cpp:1044 @ looking_at()
    //
    // 2. 移动过程中拿不到当前 Position，返回是无限的 NaN。而且 Basis 也是 NaN。原因应该就是上面导致的。
    // 奇怪的是赋值给单位后却可以使得他真的按那个位置移动，效果和使用 RemoteTransform3D 一样。
    // 但问题也是一样的：RemoteTransform3D 也是一样会使得 _unit.Position.IsFinite() 返回 true。
    let unitPathTaskStart
        (env: 'E when 'E :> ITileQuery and 'E :> IPlanetConfigQuery and 'E :> ICatlikeCodingNoiseQuery)
        =
        fun (tileIds: TileId array) (path: IHexUnitPath) ->
            path.TileIds <- tileIds
            let keyPoints = ResizeArray<Vector3>()
            // 转换为曲线
            let curve = new Curve3D()
            let radius = env.PlanetConfig.Radius
            let mutable fromTile = env.GetTile tileIds[0]
            let mutable fromHeight = env.GetHeight fromTile

            let mutable fromCentroid =
                fromTile |> Tile.unitCentroid |> TileUnitCentroid.scaled (radius + fromHeight)

            let mutable toTile = env.GetTile tileIds[1]
            let mutable toHeight = env.GetHeight toTile

            let mutable toCentroid =
                toTile |> Tile.unitCentroid |> TileUnitCentroid.scaled (radius + toHeight)

            let mutable fromIdx = Tile.getNeighborTileIdx fromTile toTile
            let mutable toIdx = Tile.getNeighborTileIdx toTile fromTile

            let mutable fromEdgeMid =
                fromTile
                |> Tile.unitCorners
                |> TileUnitCorners.getSolidEdgeMiddleWithRadius fromCentroid fromIdx (radius + fromHeight)

            let mutable toEdgeMid =
                toTile
                |> Tile.unitCorners
                |> TileUnitCorners.getSolidEdgeMiddleWithRadius toCentroid toIdx (radius + toHeight)
            // 需要注意下面 in out 入参 / 2f 的操作，用于避免 in out 入参太长（前后相交于 centroid），导致 Curve3D 不连续
            curve.AddPoint(fromCentroid, out = (fromEdgeMid - fromCentroid) / 2f)
            curve.AddPoint(fromEdgeMid, (fromCentroid - fromEdgeMid) / 2f, (toEdgeMid - fromEdgeMid) / 2f)
            curve.AddPoint(toEdgeMid, (fromEdgeMid - toEdgeMid) / 2f, (toCentroid - toEdgeMid) / 2f)
            keyPoints.Add <| fromEdgeMid.Lerp(toEdgeMid, 0.5f)

            for i in 1 .. tileIds.Length - 2 do
                fromTile <- toTile
                fromHeight <- toHeight
                fromCentroid <- toCentroid
                toTile <- env.GetTile tileIds[i + 1]
                toHeight <- env.GetHeight toTile
                toCentroid <- toTile |> Tile.unitCentroid |> TileUnitCentroid.scaled (radius + toHeight)
                fromIdx <- Tile.getNeighborTileIdx fromTile toTile
                toIdx <- Tile.getNeighborTileIdx toTile fromTile

                fromEdgeMid <-
                    fromTile
                    |> Tile.unitCorners
                    |> TileUnitCorners.getSolidEdgeMiddleWithRadius fromCentroid fromIdx (radius + fromHeight)

                toEdgeMid <-
                    toTile
                    |> Tile.unitCorners
                    |> TileUnitCorners.getSolidEdgeMiddleWithRadius toCentroid toIdx (radius + toHeight)

                curve.AddPoint(fromEdgeMid, (fromCentroid - fromEdgeMid) / 2f, (toEdgeMid - fromEdgeMid) / 2f)
                curve.AddPoint(toEdgeMid, (fromEdgeMid - toEdgeMid) / 2f, (toCentroid - toEdgeMid) / 2f)
                keyPoints.Add <| fromEdgeMid.Lerp(toEdgeMid, 0.5f)

            curve.AddPoint(toCentroid, (toEdgeMid - toCentroid) / 2f)
            path.Curve <- curve
            // 处理路径地块间的关键分割点
            path.Progresses <- keyPoints |> Seq.map path.Curve.GetClosestOffset |> Seq.toArray
            path.View.Visible <- true

    let moveSpeedByTile = 3.0 // 每 1s 走的地块格数

    let unitStartMove (unit: IHexUnit) (path: IHexUnitPath) =
        path.PathFollow.ProgressRatio <- 0f
        path.RemoteTransform.SetRemoteNode <| unit.GetPath()
        let duration = float path.Curve.PointCount / 2.0 / moveSpeedByTile
        let tween = path.GetTree().CreateTween()

        tween.TweenProperty(path.PathFollow, PathFollow3D.PropertyName.ProgressRatio.ToString(), 1, duration)
        |> ignore
        // _tween.Parallel().TweenMethod(Callable.From((Vector3 pos) => unit.AdjustMovingRotation(pos)), 0f, 1f, duration);
        tween.TweenCallback(
            Callable.From(fun () ->
                path.Working <- false
                path.View.Visible <- false
                path.TileIds <- null
                path.RemoteTransform.SetRemoteNode null
                unit.Path <- null)
        )
        |> ignore
