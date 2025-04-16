using Apps.Contexts;
using Apps.Events;
using Apps.Models.Responses;
using Apps.Nodes;
using Commons.Constants;
using Commons.Utils;
using Commons.Utils.HexSphereGrid;
using Domains.Models.Entities.PlanetGenerates;
using Domains.Models.Singletons.Planets;
using Domains.Repos.PlanetGenerates;
using Domains.Services.Uis;
using Godot;

namespace Apps.Applications.Uis.Impl;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-13 13:00:56
public class HexPlanetHudApplication(
    IPlanetConfig planetConfig,
    IChunkRepo chunkRepo,
    ITileRepo tileRepo,
    IPointRepo pointRepo,
    IEditorService editorService,
    IMiniMapService miniMapService)
    : IHexPlanetHudApplication
{
    #region 上下文节点

    private IHexPlanetHud? _hexPlanetHud;
    private IHexPlanetManager? _hexPlanetManager;
    private IMiniMapManager? _miniMapManager;

    public void OnReady()
    {
        _hexPlanetHud = NodeContext.Instance.GetSingleton<IHexPlanetHud>()!;
        _hexPlanetManager = NodeContext.Instance.GetSingleton<IHexPlanetManager>()!;
        _miniMapManager = NodeContext.Instance.GetSingleton<IMiniMapManager>()!;

        // 按照指定的高程分割数量确定 UI
        _hexPlanetHud.ElevationVSlider!.MaxValue = planetConfig.ElevationStep;
        _hexPlanetHud.ElevationVSlider.TickCount = planetConfig.ElevationStep + 1;
        _hexPlanetHud.WaterVSlider!.MaxValue = planetConfig.ElevationStep;
        _hexPlanetHud.WaterVSlider.TickCount = planetConfig.ElevationStep + 1;
        // 信号绑定
        OrbitCameraEvent.Instance.Moved += OnCameraMoved;
        OrbitCameraEvent.Instance.Transformed += OnCameraTransformed;
        _hexPlanetManager.NewPlanetGenerated += UpdateNewPlanetInfo;
        _hexPlanetManager.NewPlanetGenerated += InitMiniMap;

        // 初始化相机位置相关功能
        OnCameraMoved(_hexPlanetManager!.GetOrbitCameraFocusPos(), 0f);
        OnCameraTransformed(_hexPlanetManager.GetViewport().GetCamera3D().GetGlobalTransform(), 0f);
        SetEditMode(_hexPlanetHud.EditCheckButton!.ButtonPressed);
        editorService.SetLabelMode(_hexPlanetHud.ShowLableOptionButton!.Selected);
        editorService.SetTerrain(0);
        UpdateNewPlanetInfo();
        InitSignals();

        _miniMapManager.Init(_hexPlanetManager.GetOrbitCameraFocusPos());
    }

    private void InitSignals()
    {
        _hexPlanetHud!.WireframeCheckButton!.Toggled += toggle =>
            _hexPlanetManager!.GetViewport()
                .SetDebugDraw(toggle ? Viewport.DebugDrawEnum.Wireframe : Viewport.DebugDrawEnum.Disabled);

        _hexPlanetHud!.CelestialMotionCheckButton!.Toggled += toggle =>
            _hexPlanetManager!.PlanetRevolution = _hexPlanetManager.PlanetRotation =
                _hexPlanetManager.SatelliteRevolution = _hexPlanetManager.SatelliteRotation = toggle;
        _hexPlanetHud!.LatLonFixCheckButton!.Toggled += _hexPlanetManager!.FixLatLon;
        _hexPlanetHud!.PlanetTabBar!.TabClicked +=
            _ => _hexPlanetHud!.PlanetGrid!.Visible = !_hexPlanetHud!.PlanetGrid.Visible;

        _hexPlanetHud!.RadiusLineEdit!.TextSubmitted += text =>
        {
            if (float.TryParse(text, out var radius))
                _hexPlanetManager.Radius = radius;
            else
                _hexPlanetHud!.RadiusLineEdit.Text = $"{_hexPlanetManager.Radius:F2}";
        };

        _hexPlanetHud!.DivisionLineEdit!.TextSubmitted += text =>
        {
            if (int.TryParse(text, out var division))
                _hexPlanetManager.Divisions = division;
            else
                _hexPlanetHud!.DivisionLineEdit.Text = $"{_hexPlanetManager.Divisions}";
        };

        _hexPlanetHud!.ChunkDivisionLineEdit!.TextSubmitted += text =>
        {
            if (int.TryParse(text, out var chunkDivision))
                _hexPlanetManager.ChunkDivisions = chunkDivision;
            else
                _hexPlanetHud!.DivisionLineEdit.Text = $"{_hexPlanetManager.ChunkDivisions}";
        };

        _hexPlanetHud!.TileTabBar!.TabClicked += _ =>
        {
            var vis = !_hexPlanetHud!.TileVBox!.Visible;
            _hexPlanetHud!.TileVBox.Visible = vis;
            _hexPlanetHud!.TileGrid!.Visible = vis;
        };

        _hexPlanetHud!.EditTabBar!.TabClicked += _ =>
        {
            var vis = !_hexPlanetHud!.EditGrid!.Visible;
            _hexPlanetHud!.EditGrid.Visible = vis;
            if (vis)
            {
                editorService.SetTerrain(_hexPlanetHud!.TerrainOptionButton!.Selected);
                SetElevation(_hexPlanetHud!.ElevationVSlider!.Value);
            }
            else
            {
                editorService.SetApplyTerrain(false);
                editorService.SetApplyElevation(false);
            }
        };

        _hexPlanetHud!.EditCheckButton!.Toggled += SetEditMode;
        _hexPlanetHud!.ShowLableOptionButton!.ItemSelected += editorService.SetLabelMode;
        _hexPlanetHud!.TerrainOptionButton!.ItemSelected += editorService.SetTerrain;
        _hexPlanetHud!.ElevationVSlider!.ValueChanged += SetElevation;
        _hexPlanetHud!.ElevationCheckButton!.Toggled += editorService.SetApplyElevation;
        _hexPlanetHud!.WaterVSlider!.ValueChanged += SetWaterLevel;
        _hexPlanetHud!.WaterCheckButton!.Toggled += editorService.SetApplyWaterLevel;
        _hexPlanetHud!.BrushHSlider!.ValueChanged += SetBrushSize;
        _hexPlanetHud!.RiverOptionButton!.ItemSelected += editorService.SetRiverMode;
        _hexPlanetHud!.RoadOptionButton!.ItemSelected += editorService.SetRoadMode;
        _hexPlanetHud!.UrbanCheckButton!.Toggled += editorService.SetApplyUrbanLevel;
        _hexPlanetHud!.UrbanHSlider!.ValueChanged += editorService.SetUrbanLevel;
        _hexPlanetHud!.FarmCheckButton!.Toggled += editorService.SetApplyFarmLevel;
        _hexPlanetHud!.FarmHSlider!.ValueChanged += editorService.SetFarmLevel;
        _hexPlanetHud!.PlantCheckButton!.Toggled += editorService.SetApplyPlantLevel;
        _hexPlanetHud!.PlantHSlider!.ValueChanged += editorService.SetPlantLevel;
        _hexPlanetHud!.WallOptionButton!.ItemSelected += editorService.SetWalledMode;
        _hexPlanetHud!.SpecialFeatureCheckButton!.Toggled += editorService.SetApplySpecialIndex;
        _hexPlanetHud!.SpecialFeatureOptionButton!.ItemSelected += editorService.SetSpecialIndex;
    }

    private void InitMiniMap() =>
        _hexPlanetHud!.RectMap!.Texture = GenerateRectMap();

    private void UpdateNewPlanetInfo()
    {
        _hexPlanetHud!.RadiusLineEdit!.Text = $"{planetConfig.Radius:F2}";
        _hexPlanetHud.DivisionLineEdit!.Text = $"{planetConfig.Divisions}";
        _hexPlanetHud.ChunkDivisionLineEdit!.Text = $"{planetConfig.ChunkDivisions}";
        _hexPlanetHud.ChunkCountLabel!.Text = $"分块总数：{chunkRepo.GetCount()}";
        _hexPlanetHud.TileCountLabel!.Text = $"地块总数：{tileRepo.GetCount()}";
        _hexPlanetHud.ChosenTile = null;
    }

    private void OnCameraMoved(Vector3 pos, float delta)
    {
        var longLat = LongitudeLatitudeCoords.From(_hexPlanetManager!.ToPlanetLocal(pos));
        _hexPlanetHud!.CamLatLonLabel!.Text = $"相机位置：{longLat}";
    }

    private void OnCameraTransformed(Transform3D transform, float delta)
    {
        var northPolePoint = Vector3.Up;
        var posNormal = transform.Origin.Normalized();
        var dirNorth = Math3dUtil.DirectionBetweenPointsOnSphere(posNormal, northPolePoint);
        var angleToNorth = transform.Basis.Y.Slide(posNormal).SignedAngleTo(dirNorth, -posNormal);
        _hexPlanetHud!.CompassPanel!.Rotation = angleToNorth;

        var posLocal = _hexPlanetManager!.ToPlanetLocal(_hexPlanetManager.GetOrbitCameraFocusPos());
        var longLat = LongitudeLatitudeCoords.From(posLocal);
        var rectMapMaterial = _hexPlanetHud!.RectMap!.Material as ShaderMaterial;
        rectMapMaterial?.SetShaderParameter("lon", longLat.Longitude);
        rectMapMaterial?.SetShaderParameter("lat", longLat.Latitude);
        // rectMapMaterial?.SetShaderParameter("pos_normal", posLocal.Normalized()); // 非常奇怪，旋转时会改变……
        rectMapMaterial?.SetShaderParameter("angle_to_north", angleToNorth);
        // GD.Print($"lonLat: {longLat.Longitude}, {longLat.Latitude}; angleToNorth: {
        //     angleToNorth}; posNormal: {posNormal};");
    }

    public void OnExitTree()
    {
        OrbitCameraEvent.Instance.Moved -= OnCameraMoved;
        OrbitCameraEvent.Instance.Transformed -= OnCameraTransformed;
        _hexPlanetManager!.NewPlanetGenerated -= UpdateNewPlanetInfo;
        _hexPlanetManager.NewPlanetGenerated -= InitMiniMap;

        _hexPlanetManager = null;
        _hexPlanetHud = null;
    }

    public void OnProcess(double delta)
    {
        if (_hexPlanetHud!.GetViewport().GuiGetHoveredControl() == _hexPlanetHud.SubViewportContainer)
        {
            if (Input.IsMouseButtonPressed(MouseButton.Left))
            {
                HandleInput();
                return;
            }

            if (_hexPlanetManager!.UpdateUiInEditMode())
                return;
        }
        else if (_hexPlanetHud.GetViewport().GuiGetHoveredControl() == _hexPlanetHud.MiniMapContainer
                 && Input.IsMouseButtonPressed(MouseButton.Left))
        {
            _miniMapManager!.ClickOnMiniMap();
        }

        _hexPlanetHud.PreviousTile = null;
    }

    private void HandleInput()
    {
        // 在 SubViewportContainer 上按下鼠标左键时，获取鼠标位置地块并更新
        _hexPlanetHud!.ChosenTile = _hexPlanetManager!.GetTileUnderCursor();
        if (_hexPlanetHud.ChosenTile != null)
        {
            if (_hexPlanetHud.PreviousTile != null && _hexPlanetHud.PreviousTile != _hexPlanetHud.ChosenTile)
                ValidateDrag(_hexPlanetHud.ChosenTile);
            else
                _hexPlanetHud.IsDrag = false;
            if (editorService.TileOverrider.EditMode)
            {
                editorService.EditTiles(_hexPlanetHud.ChosenTile, _hexPlanetHud.IsDrag,
                    _hexPlanetHud.PreviousTile, _hexPlanetHud.DragTile);
                _hexPlanetHud.ChosenTile = _hexPlanetHud.ChosenTile; // 刷新 GUI 地块信息
                // 编辑模式下绘制选择地块框
                _hexPlanetManager.SelectEditingTile(_hexPlanetHud.ChosenTile);
            }
            else if (Input.IsActionJustPressed("choose_unit"))
                _hexPlanetManager.FindPath(_hexPlanetHud.ChosenTile);

            _hexPlanetHud.PreviousTile = _hexPlanetHud.ChosenTile;
        }
        else
        {
            if (!editorService.TileOverrider.EditMode)
                _hexPlanetManager.FindPath(null);
            else
                // 清理选择地块框
                _hexPlanetManager.CleanEditingTile();
            _hexPlanetHud.PreviousTile = null;
        }
    }

    private void ValidateDrag(Tile currentTile)
    {
        _hexPlanetHud!.DragTile = currentTile;
        _hexPlanetHud.IsDrag = currentTile.IsNeighbor(_hexPlanetHud.PreviousTile!);
    }

    #endregion

    public TileInfoRespDto GetTileInfo(Tile tile) => new(pointRepo.GetSphereAxial(tile), tileRepo.GetHeight(tile));

    public ImageTexture GenerateRectMap() => miniMapService.GenerateRectMap();

    #region 编辑功能

    private void SetElevation(double elevation)
    {
        editorService.SetElevation(elevation);
        _hexPlanetHud!.ElevationValueLabel!.Text = editorService.TileOverrider.ActiveElevation.ToString();
    }

    private void SetBrushSize(double brushSize)
    {
        editorService.SetBrushSize(brushSize);
        _hexPlanetHud!.BrushLabel!.Text = $"笔刷大小：{editorService.TileOverrider.BrushSize}";
    }

    private void SetWaterLevel(double level)
    {
        editorService.SetWaterLevel(level);
        _hexPlanetHud!.WaterValueLabel!.Text = editorService.TileOverrider.ActiveWaterLevel.ToString();
    }

    public void SetEditMode(bool toggle)
    {
        editorService.SetEditMode(toggle);
        RenderingServer.GlobalShaderParameterSet(GlobalShaderParam.HexMapEditMode, toggle);
    }

    #endregion
}