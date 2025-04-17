using Apps.Contexts;
using Apps.Nodes.LandGenerators;
using Domains.Models.Singletons.Planets;
using Domains.Repos.PlanetGenerates;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Dependency;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Nodes.LandGenerators;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// 真实地球生成器
/// Author: Zhu XH
/// Date: 2025-03-21 19:35:33
[Tool]
public partial class RealEarthLandGenerator : Node, IRealEarthLandGenerator
{
    public RealEarthLandGenerator()
    {
        InitServices();
        NodeContext.Instance.RegisterSingleton<IRealEarthLandGenerator>(this);
    }

    // 其实这里可以直接导入 Image, 在导入界面选择导入类型。但是导入 Image 的场景 tscn 文件会大得吓人……（等于直接按像素写一遍）
    // 这里是否改为 Image 需要权衡，目前 Texture2D.GetImage() 会耗时比较长，但是提前把 Image 加载到内存的话，消耗很大。
    // 所以目前把四张图片压到同一张图片的 RGB 通道里了（陆地掩码（去除湖区） R，地形 G，海洋测深 B），并压缩大小到 4096x2048
    [Export] private Texture2D? WorldMap { get; set; }

    #region 服务和存储

    private IPointRepo? _pointRepo;
    private ITileRepo? _tileRepo;
    private IPlanetConfig? _planetConfig;

    private void InitServices()
    {
        _pointRepo = Context.GetBeanFromHolder<IPointRepo>();
        _tileRepo = Context.GetBeanFromHolder<ITileRepo>();
        _planetConfig = Context.GetBeanFromHolder<IPlanetConfig>();
    }

    #endregion

    public override void _ExitTree() => NodeContext.Instance.DestroySingleton<IRealEarthLandGenerator>();

    public int CreateLand()
    {
        var water = _planetConfig!.DefaultWaterLevel;
        var elevationStep = _planetConfig.ElevationStep;
        var landCount = 0;
        var worldMap = WorldMap!.GetImage();
        foreach (var tile in _tileRepo!.GetAll())
        {
            var sphereAxial = _pointRepo!.GetSphereAxial(tile);
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