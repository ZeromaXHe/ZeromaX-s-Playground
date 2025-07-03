namespace TO.Domains.Functions.Maps

open System.Text
open Friflo.Engine.ECS
open Godot
open TO.Domains.Functions.HexGridCoords
open TO.Domains.Functions.HexSpheres.Components
open TO.Domains.Functions.HexSpheres.Components.Tiles
open TO.Domains.Types.HexSpheres.Components

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-07-03 20:30:03
module RectMiniMapQuery =
    let private getFillStr (tile: Entity) =
        let tileValue = tile |> Tile.value

        if tileValue |> TileValue.isUnderwater then
            if TileValue.waterLevel tileValue - TileValue.elevation tileValue > 1 then
                "#2550A9"
            else
                "#217FBF"
        else
            // 0 沙漠、1 草原、2 泥地、3 岩石、4 雪地
            match tileValue |> TileValue.terrainTypeIndex with
            | 0 -> "#e0cb5f" // 0 沙漠
            | 1 -> "#22bc35" // 1 草原
            | 2 -> "#abac24" // 2 泥地
            | 3 -> "#752e0f" // 3 岩石
            | 4 -> "#c4c4c4" // 4 雪地
            | idx -> failwith $"unknown terrain type {idx}"

    let private generatePolygonForTile (svgBuilder: StringBuilder) (width: int) (height: int) (tile: Entity) =
        let appendUvPointCoords (uv: Vector2) =
            svgBuilder
                .Append(float32 width * uv.X)
                .Append(',')
                .Append(float32 height * uv.Y)
                .Append(' ')
            |> ignore

        let uvs =
            tile
            |> Tile.unitCorners
            |> TileUnitCorners.getSeq
            |> Seq.map (LonLatCoords.fromVector3 >> LonLatCoords.toUv)
            |> Seq.toArray

        svgBuilder.Append "<polygon points=\"" |> ignore

        if
            tile |> Tile.unitCorners |> _.Length = 5
            && ((tile |> Tile.unitCentroid).UnitCentroid.DistanceSquaredTo Vector3.Up < 0.0001f
                || (tile |> Tile.unitCentroid).UnitCentroid.DistanceSquaredTo Vector3.Down < 0.0001f)
        then
            // 是否北极
            let isNorth =
                (tile |> Tile.unitCentroid).UnitCentroid.DistanceSquaredTo Vector3.Up < 0.0001f
            // 按 X 从小到大排序
            let uvs = uvs |> Array.sortBy _.X
            let mutable uv0 = uvs[0]
            uv0.X <- uv0.X + 1f
            appendUvPointCoords uv0
            appendUvPointCoords (if isNorth then Vector2.Right else Vector2.One)
            appendUvPointCoords (if isNorth then Vector2.Zero else Vector2.Down)
            let mutable uvLast = uvs[uvs.Length - 1]
            uvLast.X <- uvLast.X - 1f
            appendUvPointCoords uvLast

            for uv in uvs do
                appendUvPointCoords uv
        else
            let mutable minX = uvs[0].X
            let mutable maxX = uvs[0].X

            for i in 1 .. uvs.Length - 1 do
                if uvs[i].X > maxX then
                    maxX <- uvs[i].X

                if uvs[i].X < minX then
                    minX <- uvs[i].X

            if maxX - minX > 0.5f then
                // 横跨大半个地球，说明是包覆边界上的地块。分左右两块全部绘制
                uvs
                |> Array.map (fun uv -> if uv.X > 0.5f then uv else uv + Vector2.Right)
                |> Array.iter appendUvPointCoords
                // 分成两个 SVG 多边形标签。否则作为一个多边形绘制是错误的，会有覆盖半张地图的横纹
                let midFill = getFillStr tile

                svgBuilder
                    .Append("\" fill=\"")
                    .Append(midFill)
                    .Append("\" stroke=\"")
                    .Append(midFill)
                    .Append("\" stroke-width=\"1\"/>")
                    .Append("<polygon points=\"")
                |> ignore

                uvs
                |> Array.map (fun uv -> if uv.X < 0.5f then uv else uv - Vector2.Right)
                |> Array.iter appendUvPointCoords
            else
                uvs |> Array.iter appendUvPointCoords

        let fill = getFillStr tile

        svgBuilder
            .Append("\" fill=\"")
            .Append(fill)
            .Append("\" stroke=\"")
            .Append(fill)
            .Append("\" stroke-width=\"1\"/>")
        |> ignore

    let private generateRectMapSvg (env: #ITileQuery) (width: int) (height: int) =
        let svgBuilder = StringBuilder()

        svgBuilder
            .Append("<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"")
            .Append(width)
            .Append("\" height=\"")
            .Append(height)
            .Append("\">")
        |> ignore

        for tile in env.GetAllTiles() do
            generatePolygonForTile svgBuilder width height tile

        svgBuilder.Append("</svg>").ToString()

    let generateRectMap env =
        let time = Time.GetTicksMsec()
        let img = Image.CreateEmpty(4096, 2048, false, Image.Format.Rgb8)
        generateRectMapSvg env 4096 2048 |> img.LoadSvgFromString |> ignore
        let imgTexture = ImageTexture.CreateFromImage img
        GD.Print($"Generate rect mini map in {Time.GetTicksMsec() - time} ms")
        imgTexture
