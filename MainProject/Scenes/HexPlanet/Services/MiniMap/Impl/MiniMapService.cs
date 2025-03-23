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

            if (maxX - minX > 0.5f)
            {
                // 横跨大半个地球，说明是包覆边界上的地块。分左右两块全部绘制
                foreach (var rightUv in uvs.Select(
                             uv => uv.X > 0.5f ? uv : uv + Vector2.Right))
                    AppendUvPointCoords(rightUv);
                // 分成两个 SVG 多边形标签。否则作为一个多边形绘制是错误的，会有覆盖半张地图的横纹
                var midFill = GetFillStr(tile);
                svgBuilder.Append("\" fill=\"").Append(midFill).Append("\" stroke=\"").Append(midFill)
                    .Append("\" stroke-width=\"1\"/>");
                svgBuilder.Append("<polygon points=\"");
                foreach (var leftUv in uvs.Select(
                             uv => uv.X < 0.5 ? uv : uv - Vector2.Right))
                    AppendUvPointCoords(leftUv);
            }
            else
                foreach (var uv in uvs)
                    AppendUvPointCoords(uv);
        }

        var fill = GetFillStr(tile);
        svgBuilder.Append("\" fill=\"").Append(fill).Append("\" stroke=\"").Append(fill)
            .Append("\" stroke-width=\"1\"/>");
        return;

        void AppendUvPointCoords(Vector2 uv) =>
            svgBuilder.Append(width * uv.X).Append(',').Append(height * uv.Y).Append(' ');
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