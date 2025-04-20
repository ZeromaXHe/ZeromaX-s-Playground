using Apps.Models.Responses;
using Apps.Queries.Contexts;
using Commons.Utils;
using Commons.Utils.HexSphereGrid;
using Domains.Models.Entities.PlanetGenerates;
using Domains.Services.Abstractions.Nodes;
using Domains.Services.Abstractions.PlanetGenerates;
using Domains.Services.Abstractions.Uis;
using Godot;
using Infras.Readers.Abstractions.Nodes.Singletons;
using Infras.Writers.Abstractions.PlanetGenerates;
using Nodes.Abstractions;
using Nodes.Abstractions.Planets;

namespace Apps.Queries.Applications.Uis.Impl;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-13 13:00:56
public class HexPlanetHudApp(
    IChunkRepo chunkRepo,
    ITileRepo tileRepo,
    IPointRepo pointRepo,
    IHexPlanetHudRepo hexPlanetHudRepo,
    IHexPlanetManagerRepo hexPlanetManagerRepo,
    IOrbitCameraRepo orbitCameraRepo,
    IHexPlanetHudService hexPlanetHudService,
    ITileService tileService,
    IMiniMapService miniMapService)
    : IHexPlanetHudApp
{
    #region 上下文节点

    private IHexPlanetHud? _hexPlanetHud;
    private IHexPlanetManager? _hexPlanetManager;
    private IMiniMapManager? _miniMapManager;
    private ISelectTileViewer? _selectTileViewer;
    private IUnitManager? _unitManager;
    private IOrbitCamera? _orbitCamera;

    public bool NodeReady { get; set; }

    public void OnReady()
    {
        NodeReady = true;
        // 初始化上下文节点
        _hexPlanetHud = NodeContext.Instance.GetSingleton<IHexPlanetHud>()!;
        _hexPlanetManager = NodeContext.Instance.GetSingleton<IHexPlanetManager>()!;
        _miniMapManager = NodeContext.Instance.GetSingleton<IMiniMapManager>()!;
        _selectTileViewer = NodeContext.Instance.GetSingleton<ISelectTileViewer>();
        _unitManager = NodeContext.Instance.GetSingleton<IUnitManager>();
        _orbitCamera = NodeContext.Instance.GetSingleton<IOrbitCamera>()!;

        // 按照指定的高程分割数量确定 UI
        hexPlanetHudService.InitElevationAndWaterVSlider();
        // 绑定事件
        orbitCameraRepo.Moved += OnCameraMoved;
        orbitCameraRepo.Transformed += OnCameraTransformed;
        _hexPlanetManager.NewPlanetGenerated += UpdateNewPlanetInfo;
        _hexPlanetManager.NewPlanetGenerated += InitMiniMap;

        // 初始化相机位置相关功能
        OnCameraMoved(_orbitCamera!.GetFocusBasePos(), 0f);
        OnCameraTransformed(_hexPlanetManager.GetViewport().GetCamera3D().GetGlobalTransform(), 0f);
        hexPlanetHudRepo.SetEditMode(_hexPlanetHud.EditCheckButton!.ButtonPressed);
        hexPlanetHudRepo.SetLabelMode(_hexPlanetHud.ShowLableOptionButton!.Selected);
        hexPlanetHudRepo.SetTerrain(0);
        UpdateNewPlanetInfo();
        InitSignals();

        _miniMapManager.Init(_orbitCamera!.GetFocusBasePos());
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
                hexPlanetHudRepo.SetTerrain(_hexPlanetHud!.TerrainOptionButton!.Selected);
                hexPlanetHudRepo.SetElevation(_hexPlanetHud!.ElevationVSlider!.Value);
            }
            else
            {
                hexPlanetHudRepo.SetApplyTerrain(false);
                hexPlanetHudRepo.SetApplyElevation(false);
            }
        };

        _hexPlanetHud!.EditCheckButton!.Toggled += hexPlanetHudRepo.SetEditMode;
        _hexPlanetHud!.ShowLableOptionButton!.ItemSelected += hexPlanetHudRepo.SetLabelMode;
        _hexPlanetHud!.TerrainOptionButton!.ItemSelected += hexPlanetHudRepo.SetTerrain;
        _hexPlanetHud!.ElevationVSlider!.ValueChanged += hexPlanetHudRepo.SetElevation;
        _hexPlanetHud!.ElevationCheckButton!.Toggled += hexPlanetHudRepo.SetApplyElevation;
        _hexPlanetHud!.WaterVSlider!.ValueChanged += hexPlanetHudRepo.SetWaterLevel;
        _hexPlanetHud!.WaterCheckButton!.Toggled += hexPlanetHudRepo.SetApplyWaterLevel;
        _hexPlanetHud!.BrushHSlider!.ValueChanged += hexPlanetHudRepo.SetBrushSize;
        _hexPlanetHud!.RiverOptionButton!.ItemSelected += hexPlanetHudRepo.SetRiverMode;
        _hexPlanetHud!.RoadOptionButton!.ItemSelected += hexPlanetHudRepo.SetRoadMode;
        _hexPlanetHud!.UrbanCheckButton!.Toggled += hexPlanetHudRepo.SetApplyUrbanLevel;
        _hexPlanetHud!.UrbanHSlider!.ValueChanged += hexPlanetHudRepo.SetUrbanLevel;
        _hexPlanetHud!.FarmCheckButton!.Toggled += hexPlanetHudRepo.SetApplyFarmLevel;
        _hexPlanetHud!.FarmHSlider!.ValueChanged += hexPlanetHudRepo.SetFarmLevel;
        _hexPlanetHud!.PlantCheckButton!.Toggled += hexPlanetHudRepo.SetApplyPlantLevel;
        _hexPlanetHud!.PlantHSlider!.ValueChanged += hexPlanetHudRepo.SetPlantLevel;
        _hexPlanetHud!.WallOptionButton!.ItemSelected += hexPlanetHudRepo.SetWalledMode;
        _hexPlanetHud!.SpecialFeatureCheckButton!.Toggled += hexPlanetHudRepo.SetApplySpecialIndex;
        _hexPlanetHud!.SpecialFeatureOptionButton!.ItemSelected += hexPlanetHudRepo.SetSpecialIndex;
    }

    private void InitMiniMap() =>
        _hexPlanetHud!.RectMap!.Texture = GenerateRectMap();

    private void UpdateNewPlanetInfo()
    {
        _hexPlanetHud!.RadiusLineEdit!.Text = $"{hexPlanetManagerRepo.Radius:F2}";
        _hexPlanetHud.DivisionLineEdit!.Text = $"{hexPlanetManagerRepo.Divisions}";
        _hexPlanetHud.ChunkDivisionLineEdit!.Text = $"{hexPlanetManagerRepo.ChunkDivisions}";
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

        var posLocal = _hexPlanetManager!.ToPlanetLocal(_orbitCamera!.GetFocusBasePos());
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
        NodeReady = false;
        // 事件解绑
        orbitCameraRepo.Moved -= OnCameraMoved;
        orbitCameraRepo.Transformed -= OnCameraTransformed;
        _hexPlanetManager!.NewPlanetGenerated -= UpdateNewPlanetInfo;
        _hexPlanetManager.NewPlanetGenerated -= InitMiniMap;
        // 上下文节点置空
        _hexPlanetManager = null;
        _hexPlanetHud = null;
        _selectTileViewer = null;
        _unitManager = null;
        _orbitCamera = null;
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
            if (_hexPlanetHud.TileOverrider.EditMode)
            {
                tileService.EditTiles(_hexPlanetHud.ChosenTile, _hexPlanetHud.TileOverrider, _hexPlanetHud.IsDrag,
                    _hexPlanetHud.PreviousTile, _hexPlanetHud.DragTile);
                _hexPlanetHud.ChosenTile = _hexPlanetHud.ChosenTile; // 刷新 GUI 地块信息
                // 编辑模式下绘制选择地块框
                _selectTileViewer!.SelectEditingTile(_hexPlanetHud.ChosenTile);
            }
            else if (Input.IsActionJustPressed("choose_unit"))
                _unitManager!.FindPath(_hexPlanetHud.ChosenTile);

            _hexPlanetHud.PreviousTile = _hexPlanetHud.ChosenTile;
        }
        else
        {
            if (!_hexPlanetHud.TileOverrider.EditMode)
                _unitManager!.FindPath(null);
            else
                // 清理选择地块框
                _selectTileViewer!.CleanEditingTile();
            _hexPlanetHud.PreviousTile = null;
        }
    }

    private void ValidateDrag(Tile currentTile)
    {
        _hexPlanetHud!.DragTile = currentTile;
        _hexPlanetHud.IsDrag = currentTile.IsNeighbor(_hexPlanetHud.PreviousTile!);
    }

    #endregion

    public TileInfoRespDto GetTileInfo(Tile tile) => new(pointRepo.GetSphereAxial(tile), hexPlanetManagerRepo.GetHeight(tile));

    public ImageTexture GenerateRectMap() => miniMapService.GenerateRectMap();
}