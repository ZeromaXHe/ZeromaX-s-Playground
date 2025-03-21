using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Dependency;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Services;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Nodes.LandGenerators;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// 真实地球生成器
/// Author: Zhu XH
/// Date: 2025-03-21 19:35:33
[Tool]
public partial class RealEarthLandGenerator : Node
{
    public RealEarthLandGenerator() => InitServices();

    // 其实这里可以直接导入 Image, 在导入界面选择导入类型。但是导入 Image 的场景 tscn 文件会大得吓人……（等于直接按像素写一遍）
    // 这里是否改为 Image 需要权衡，目前 Texture2D.GetImage() 会耗时比较长，但是提前把 Image 加载到内存的话，消耗很大。
    [Export] private Texture2D LandMask { get; set; } // 陆地掩码图
    [Export] private Texture2D Bathymetry { get; set; } // 海洋测深图
    [Export] private Texture2D Topography { get; set; } // 地形图
    [Export] private Texture2D Lake { get; set; } // 湖区图

    #region 服务

    private ITileService _tileService;
    private IPlanetSettingService _planetSettingService;

    private void InitServices()
    {
        _tileService = Context.GetBean<ITileService>();
        _planetSettingService = Context.GetBean<IPlanetSettingService>();
    }

    #endregion

    public int CreateLand()
    {
        var water = _planetSettingService.DefaultWaterLevel;
        var elevationStep = _planetSettingService.ElevationStep;
        var landCount = 0;
        var landMaskImg = LandMask.GetImage();
        var bathymetryImg = Bathymetry.GetImage();
        var topographyImg = Topography.GetImage();
        var lakeImg = Lake.GetImage();
        foreach (var tile in _tileService.GetAll())
        {
            var sphereAxial = _tileService.GetSphereAxial(tile);
            var lonLat = sphereAxial.ToLongitudeAndLatitude().ToVector2();
            var percentX = Mathf.Remap(lonLat.X, 180f, -180f, 0f, 1f); // 西经为正，所以这里得反一下
            var percentY = Mathf.Remap(lonLat.Y, 90f, -90f, 0f, 1f); // 北纬为正，所以这里得反一下
            var x = (int)(16384 * percentX); // 除了湖区图宽度 8192，其他宽 16384
            if (x >= 16384)
                x = 16383; // 不知道为啥 Mathf.Clamp 限制不了…… 手动限制一下
            var y = (int)(8192 * percentY); // 除了湖区图高度 4096，其他高 8192
            if (y >= 8192)
                y = 8191; // 不知道为啥 Mathf.Clamp 限制不了…… 手动限制一下

            int elevation;
            if (landMaskImg.GetPixel(x, y) > Colors.LightGray)
            {
                // 陆地
                if (lakeImg.GetPixel(x / 2, y / 2) > Colors.LightGray)
                    // 湖区
                    elevation = water - 1;
                else
                {
                    elevation = (int)(topographyImg.GetPixel(x, y).R * (elevationStep + 1 - water));
                    if (elevation == elevationStep + 1 - water)
                        elevation--;
                    elevation += water;
                    landCount++;
                }
            }
            else
            {
                // 海洋
                elevation = (int)(bathymetryImg.GetPixel(x, y).R * water);
                if (elevation == water)
                    elevation--;
            }

            tile.Data = tile.Data with { Values = tile.Data.Values.WithElevation(elevation) };
        }

        return landCount;
    }
}