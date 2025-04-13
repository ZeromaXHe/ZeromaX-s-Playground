using Commons.Constants;
using Domains.Events.Tiles;
using Domains.Models.Entities.Civs;
using Domains.Models.Entities.PlanetGenerates;
using Domains.Models.Singletons.Planets;
using Domains.Models.ValueObjects.PlanetGenerates;
using Domains.Repos.PlanetGenerates;
using Domains.Services.Civs;
using Domains.Services.PlanetGenerates;
using Godot;

namespace Domains.Services.Shaders.Impl;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-03 09:14
public class TileShaderService : ITileShaderService
{
    private readonly ITileRepo _tileRepo;
    private readonly IUnitService _unitService;
    private readonly ICivService _civService;
    private readonly IPlanetConfig _planetConfig;

    public TileShaderService(ITileRepo tileRepo, IUnitService unitService, ICivService civService,
        IPlanetConfig planetConfig)
    {
        _tileRepo = tileRepo;
        _tileRepo.RefreshTerrainShader += RefreshTerrain;
        _tileRepo.ViewElevationChanged += ViewElevationChanged;
        _unitService = unitService;
        _civService = civService;
        _planetConfig = planetConfig;
    }

    private Image? _tileTexture; // 地块地理信息
    private Image? _tileCivTexture; // 地块占领文明
    private Color[] _tileTextureData = [];
    private Color[] _tileCivTextureData = [];
    private ImageTexture? _hexTileData;
    private ImageTexture? _hexTileCivData;
    private bool _enabled;
    private List<int> _transitioningTileIndices = [];
    private const float TransitionSpeed = 255;
    private bool _needsVisibilityReset;
    private bool[] _visibilityTransitions = [];

    public bool ImmediateMode { get; set; }

    public void Initialize()
    {
        // 地块数等于 20 * div * div / 2 + 2 = 10 * div ^ 2 + 2
        var x = _planetConfig.Divisions * 5;
        var z = _planetConfig.Divisions * 2 + 1; // 十二个五边形会导致余数
        _tileTexture = Image.CreateEmpty(x, z, false, Image.Format.Rgba8);
        _tileCivTexture = Image.CreateEmpty(x, z, false, Image.Format.Rgba8);
        _hexTileData = ImageTexture.CreateFromImage(_tileTexture);
        _hexTileCivData = ImageTexture.CreateFromImage(_tileCivTexture);
        RenderingServer.GlobalShaderParameterSet(GlobalShaderParam.HexTileData, Variant.CreateFrom(_hexTileData));
        RenderingServer.GlobalShaderParameterSet(GlobalShaderParam.HexTileCivData, Variant.CreateFrom(_hexTileCivData));
        RenderingServer.GlobalShaderParameterSet(GlobalShaderParam.HexTileDataTexelSize, new Vector4(1f / x, 1f / z, x, z));
        if (_tileTextureData.Length == 0 || _tileTextureData.Length != x * z)
        {
            _tileTextureData = new Color[x * z];
            _tileCivTextureData = new Color[x * z];
            _visibilityTransitions = new bool[x * z];
        }
        else
        {
            for (var i = 0; i < _tileTextureData.Length; i++)
            {
                _tileTextureData[i] = new Color(0f, 0f, 0f, 0f);
                _tileCivTextureData[i] = Colors.Black;
                _visibilityTransitions[i] = false;
            }
        }

        _transitioningTileIndices.Clear();
        _enabled = true;
    }

    public void RefreshCiv(int tileId)
    {
        var tile = _tileRepo.GetById(tileId)!;
        if (tile.CivId <= 0)
            _tileCivTextureData[tileId] = Colors.Black;
        else
        {
            var civ = _civService.GetById(tile.CivId)!;
            _tileCivTextureData[tileId] = civ.Color;
        }

        ChangePixel(_tileCivTexture!, tileId, _tileCivTextureData[tileId]);
        _enabled = true;
    }

    public void RefreshTerrain(int tileId)
    {
        var tile = _tileRepo.GetById(tileId)!;
        var data = _tileTextureData[tileId];
        data.B8 = tile.Data.IsUnderwater
            ? (int)(tile.Data.WaterSurfaceY * (255f / _planetConfig.MaxHeight))
            : 0;
        data.A8 = tile.Data.TerrainTypeIndex;
        _tileTextureData[tileId] = data;
        ChangePixel(_tileTexture!, tileId, data);
        _enabled = true;
    }

    public void RefreshVisibility(int tileId)
    {
        if (ImmediateMode)
        {
            var tile = _tileRepo.GetById(tileId)!;
            _tileTextureData[tileId].R8 = tile.IsVisible ? 255 : 0;
            _tileTextureData[tileId].G8 = tile.Data.IsExplored ? 255 : 0;
            ChangePixel(_tileTexture!, tileId, _tileTextureData[tileId]);
        }
        else if (!_visibilityTransitions[tileId])
        {
            _visibilityTransitions[tileId] = true;
            _transitioningTileIndices.Add(tileId);
        }

        _enabled = true;
    }

    public void UpdateData(float delta)
    {
        if (_enabled)
        {
            if (_needsVisibilityReset)
            {
                _needsVisibilityReset = false;
                ResetVisibility();
            }

            var deltaSpeed = (int)(delta * TransitionSpeed);
            deltaSpeed = deltaSpeed == 0 ? 1 : deltaSpeed;
            var i = 0;
            while (i < _transitioningTileIndices.Count)
            {
                if (!UpdateTileData(_transitioningTileIndices[i], deltaSpeed))
                {
                    _transitioningTileIndices[i] = _transitioningTileIndices[^1];
                    _transitioningTileIndices.RemoveAt(_transitioningTileIndices.Count - 1);
                }
                else i++;
            }

            // 更新 Shader global uniform 变量（hex_cell_data）
            // 不能用这里的方法：RenderingServer.global_shader_parameter_get() 就没法在游戏循环里用
            // RenderingServer
            //     .GlobalShaderParameterGet("hex_cell_data")
            //     .As<ImageTexture>()
            //     .Update(cellTexture)
            _hexTileData!.Update(_tileTexture);
            _hexTileCivData!.Update(_tileCivTexture);
            _enabled = _transitioningTileIndices.Count > 0;
        }
    }

    private void ResetVisibility()
    {
        foreach (var tile in _tileRepo.GetAll())
        {
            if (tile.Visibility <= 0) continue;
            tile.Visibility = 0;
            RefreshVisibility(tile.Id);
        }

        foreach (var unit in _unitService.GetAll())
            TileShaderEvent.EmitRangeVisibilityIncreased(_tileRepo.GetById(unit.TileId)!, Unit.VisionRange);
    }

    public void IncreaseVisibility(Tile tile)
    {
        tile.Visibility++;
        if (tile.Visibility == 1)
        {
            tile.Data = tile.Data with { Flags = tile.Data.Flags.With(HexFlags.Explored) };
            RefreshVisibility(tile.Id);
            TileShaderEvent.EmitTileExplored(tile);
        }
    }

    public void DecreaseVisibility(Tile tile)
    {
        tile.Visibility--;
        if (tile.Visibility == 0)
            RefreshVisibility(tile.Id);
    }

    public void ViewElevationChanged(int tileId)
    {
        var tile = _tileRepo.GetById(tileId)!;
        _tileTextureData[tileId].B8 = tile.Data.IsUnderwater
            ? (int)(tile.Data.WaterSurfaceY * (255f / _planetConfig.MaxHeight))
            : 0;
        ChangePixel(_tileTexture!, tileId, _tileTextureData[tileId]);
        _needsVisibilityReset = true;
        _enabled = true;
    }

    private bool UpdateTileData(int id, int delta)
    {
        var data = _tileTextureData[id];
        var stillUpdating = false;
        var tile = _tileRepo.GetById(id)!;
        if (tile.Data.IsExplored && data.G8 < 255)
        {
            stillUpdating = true;
            var t = data.G8 + delta;
            data.G8 = t >= 255 ? 255 : t;
        }

        if (tile.IsVisible)
        {
            if (data.R8 < 255)
            {
                stillUpdating = true;
                var t = data.R8 + delta;
                data.R8 = t >= 255 ? 255 : t;
            }
        }
        else if (data.R8 > 0)
        {
            stillUpdating = true;
            var t = data.R8 - delta;
            data.R8 = t <= 0 ? 0 : t;
        }

        if (!stillUpdating)
            _visibilityTransitions[id] = false;
        _tileTextureData[id] = data;
        ChangePixel(_tileTexture!, id, data);
        return stillUpdating;
    }

    private static void ChangePixel(Image img, int tileId, Color data) =>
        img.SetPixel(tileId % img.GetWidth(), tileId / img.GetWidth(), data);
}