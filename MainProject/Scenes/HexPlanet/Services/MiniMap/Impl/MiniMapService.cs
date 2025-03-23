using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entities;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Utils.HexSphereGrid;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Services.MiniMap.Impl;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-23 10:09:06
public class MiniMapService(ITileService tileService, IPlanetSettingService planetSettingService) : IMiniMapService
{
    public ImageTexture GenerateRectMap()
    {
        var time = Time.GetTicksMsec();
        var img = Image.CreateEmpty(4096, 2048, false, Image.Format.Rgb8);
        img.LoadSvgFromString(GenerateRectMapSvg(4096, 2048));
        var imgTexture = ImageTexture.CreateFromImage(img);
        GD.Print($"Generate rect mini map in {Time.GetTicksMsec() - time} ms");
        return imgTexture;
    }

    private string GenerateRectMapSvg(int width, int height)
    {
        var svgBuilder = new StringBuilder();
        svgBuilder.Append("<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"").Append(width)
            .Append("\" height=\"").Append(height).Append("\">");
        foreach (var tile in tileService.GetAll())
            GeneratePolygonForTile(svgBuilder, width, height, tile);
        svgBuilder.Append("</svg>");
        return svgBuilder.ToString();
    }

    private void GeneratePolygonForTile(StringBuilder svgBuilder, int width, int height, Tile tile)
    {
        var uvs = tileService.GetCorners(tile, 1f)
            .Select(c => LongitudeLatitudeCoords.From(c).ToUv())
            .ToList();
        svgBuilder.Append("<polygon points=\"");
        if (tile.IsPentagon()
            && (tile.UnitCentroid.DistanceSquaredTo(Vector3.Up) < 0.0001f
                || tile.UnitCentroid.DistanceSquaredTo(Vector3.Down) < 0.0001f))
        {
            // 是否北极
            var isNorth = tile.UnitCentroid.DistanceSquaredTo(Vector3.Up) < 0.0001f;
            // 按 X 从小到大排序
            uvs.Sort((uv1, uv2) => uv1.X.CompareTo(uv2.X));
            AppendUvPointCoords(uvs[0] with { X = uvs[0].X + 1 });
            AppendUvPointCoords(isNorth ? Vector2.Right : Vector2.One);
            AppendUvPointCoords(isNorth ? Vector2.Zero : Vector2.Down);
            AppendUvPointCoords(uvs[^1] with { X = uvs[^1].X - 1 });
            foreach (var uv in uvs)
                AppendUvPointCoords(uv);
        }
        else
        {
            var minX = uvs[0].X;
            var maxX = uvs[0].X;
            for (var i = 1; i < uvs.Count; i++)
            {
                if (uvs[i].X > maxX) maxX = uvs[i].X;
                if (uvs[i].X < minX) minX = uvs[i].X;
            }

            if (maxX - minX > 0.8)
            {
                // 横跨大半个地球，说明是包覆边界上的地块。分左右两块全部绘制
                // 不知道为啥如果直接映射到 x + 1 / x - 1 的位置，两边都绘制会出现横纹
                // 所以现在超出 0 ~ 1 的点，都映射到 x = 1 或 x = 0 的边上
                for (var i = 0; i < uvs.Count; i++)
                    AppendUvRightPointCoords(i);
                for (var i = 0; i < uvs.Count; i++)
                    AppendUvLeftPointCoords(i);
            }
            else
                foreach (var uv in uvs)
                    AppendUvPointCoords(uv);
        }

        var fill = GetFillStr(tile);
        svgBuilder.Append("\" fill=\"").Append(fill).Append("\" stroke=\"").Append(fill).Append("\" stroke-width=\"1\"/>");
        return;

        void AppendUvPointCoords(Vector2 uv) =>
            svgBuilder.Append(width * uv.X).Append(',').Append(height * uv.Y).Append(' ');

        void AppendUvLeftPointCoords(int idx)
        {
            var uv = uvs[idx];
            if (uv.X < 0.8f)
            {
                AppendUvPointCoords(uv);
                return;
            }

            var newUv = uv - Vector2.Right;
            var preUv = uvs[tile.PreviousIdx(idx)];
            var nextUv = uvs[tile.NextIdx(idx)];
            if (preUv.X > 0.8f && nextUv.X > 0.8f)
                // 如果前后都是待映射到左侧的点
                return;

            if (preUv.X < 0.8f) // 如果前面是本来就在左侧的点
                AppendUvPointCoords(newUv.Lerp(preUv, -newUv.X / (preUv.X - newUv.X)));
            if (nextUv.X < 0.8f) // 如果后面是本来就在左侧的点
                AppendUvPointCoords(newUv.Lerp(nextUv, -newUv.X / (nextUv.X - newUv.X)));
        }

        void AppendUvRightPointCoords(int idx)
        {
            var uv = uvs[idx];
            if (uv.X > 0.2f)
            {
                AppendUvPointCoords(uv);
                return;
            }

            var newUv = uv + Vector2.Right;
            var preUv = uvs[tile.PreviousIdx(idx)];
            var nextUv = uvs[tile.NextIdx(idx)];
            if (preUv.X < 0.2f && nextUv.X < 0.2f)
                // 如果前后都是待映射到右侧的点
                return;

            if (preUv.X > 0.2f) // 如果前面是本来就在右侧的点
                AppendUvPointCoords(newUv.Lerp(preUv, (newUv.X - 1f) / (newUv.X - preUv.X)));
            if (nextUv.X > 0.2f) // 如果后面是本来就在右侧的点
                AppendUvPointCoords(newUv.Lerp(nextUv, (newUv.X - 1f) / (newUv.X - nextUv.X)));
        }
    }

    private static string GetFillStr(Tile tile)
    {
        if (tile.Data.IsUnderwater)
            return tile.Data.WaterLevel - tile.Data.Elevation > 1 ? "#2550A9" : "#217FBF";
        return tile.Data.TerrainTypeIndex switch
        {
            // 0 沙漠、1 草原、2 泥地、3 岩石、4 雪地
            0 => "#e0cb5f",
            1 => "#22bc35",
            2 => "#abac24",
            3 => "#752e0f",
            4 => "#c4c4c4",
            _ => throw new ArgumentException("unknown terrain type")
        };
    }
}