using Domains.Services.Abstractions.Nodes.LandGenerators;
using Godot;
using Infras.Readers.Abstractions.Nodes.Singletons;
using Infras.Readers.Abstractions.Nodes.Singletons.LandGenerators;
using Infras.Writers.Abstractions.PlanetGenerates;
using Nodes.Abstractions.LandGenerators;

namespace Domains.Services.Nodes.LandGenerators;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 20:15:31
public class RealEarthLandGeneratorService(
    IRealEarthLandGeneratorRepo realEarthLandGeneratorRepo,
    IHexPlanetManagerRepo hexPlanetManagerRepo,
    ITileRepo tileRepo,
    IPointRepo pointRepo) : IRealEarthLandGeneratorService
{
    private IRealEarthLandGenerator Self => realEarthLandGeneratorRepo.Singleton!;

    public int CreateLand()
    {
        var water = hexPlanetManagerRepo.DefaultWaterLevel;
        var elevationStep = hexPlanetManagerRepo.ElevationStep;
        var landCount = 0;
        var worldMap = Self.WorldMap!.GetImage();
        foreach (var tile in tileRepo.GetAll())
        {
            var sphereAxial = pointRepo.GetSphereAxial(tile);
            var lonLat = sphereAxial.ToLongitudeAndLatitude().ToVector2();
            var percentX = Mathf.Remap(lonLat.X, 180f, -180f, 0f, 1f); // 西经为正，所以这里得反一下
            var percentY = Mathf.Remap(lonLat.Y, 90f, -90f, 0f, 1f); // 北纬为正，所以这里得反一下
            var x = (int)(4096 * percentX); // 宽度 4096
            if (x >= 4096)
                x = 4095; // 不知道为啥 Mathf.Clamp 限制不了…… 手动限制一下
            var y = (int)(2048 * percentY); // 高度 2048
            if (y >= 2048)
                y = 2047; // 不知道为啥 Mathf.Clamp 限制不了…… 手动限制一下

            int elevation;
            var color = worldMap.GetPixel(x, y);
            if (color.R > 0.9f)
            {
                // 陆地
                elevation = (int)(color.G * (elevationStep + 1 - water));
                if (elevation == elevationStep + 1 - water)
                    elevation--;
                elevation += water;
                landCount++;
            }
            else if (color.G > 0.1f)
            {
                // 湖区
                // BUG: 现在的实现可能出现高于相邻地块的水面
                elevation = (int)(color.G * (elevationStep + 1 - water));
                if (elevation == elevationStep + 1 - water)
                    elevation--;
                elevation += water - 1;
                tile.Data = tile.Data with { Values = tile.Data.Values.WithWaterLevel(elevation + 1) };
            }
            else
            {
                // 海洋
                elevation = (int)(color.B * water);
                if (elevation == water)
                    elevation--;
            }

            tile.Data = tile.Data with { Values = tile.Data.Values.WithElevation(elevation) };
        }

        return landCount;
    }
}