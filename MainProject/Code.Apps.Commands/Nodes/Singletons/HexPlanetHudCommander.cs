using Commons.Utils;
using Commons.Utils.HexSphereGrid;
using Domains.Models.Entities.PlanetGenerates;
using Domains.Services.Abstractions.Nodes.Singletons;
using Domains.Services.Abstractions.Nodes.Singletons.Planets;
using Domains.Services.Abstractions.PlanetGenerates;
using Domains.Services.Abstractions.Uis;
using Godot;
using Infras.Readers.Abstractions.Nodes.Singletons;
using Infras.Readers.Abstractions.Nodes.Singletons.Planets;
using Infras.Writers.Abstractions.PlanetGenerates;
using Nodes.Abstractions;

namespace Apps.Commands.Nodes.Singletons;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 20:43:11
public class HexPlanetHudCommander
{
    private readonly IHexPlanetHudRepo _hexPlanetHudRepo;

    private readonly IChunkRepo _chunkRepo;
    private readonly ITileRepo _tileRepo;
    private readonly IPointRepo _pointRepo;
    private readonly IHexPlanetManagerRepo _hexPlanetManagerRepo;
    private readonly ISelectTileViewerRepo _selectTileViewerRepo;
    private readonly IMiniMapManagerRepo _miniMapManagerRepo;
    private readonly ILongitudeLatitudeRepo _longitudeLatitudeRepo;
    private readonly IOrbitCameraRepo _orbitCameraRepo;
    private readonly IHexPlanetManagerService _hexPlanetManagerService;
    private readonly IHexPlanetHudService _hexPlanetHudService;
    private readonly ITileService _tileService;
    private readonly IUnitManagerService _unitManagerService;
    private readonly IMiniMapManagerService _miniMapManagerService;
    private readonly IMiniMapService _miniMapService;

    public HexPlanetHudCommander(IHexPlanetHudRepo hexPlanetHudRepo, IChunkRepo chunkRepo, ITileRepo tileRepo,
        IPointRepo pointRepo, IHexPlanetManagerRepo hexPlanetManagerRepo, ISelectTileViewerRepo selectTileViewerRepo,
        IMiniMapManagerRepo miniMapManagerRepo, ILongitudeLatitudeRepo longitudeLatitudeRepo,
        IOrbitCameraRepo orbitCameraRepo, IHexPlanetManagerService hexPlanetManagerService,
        IHexPlanetHudService hexPlanetHudService, ITileService tileService, IUnitManagerService unitManagerService,
        IMiniMapManagerService miniMapManagerService, IMiniMapService miniMapService)
    {
        _hexPlanetHudRepo = hexPlanetHudRepo;
        _hexPlanetHudRepo.Ready += OnReady;
        _hexPlanetHudRepo.TreeExiting += OnTreeExiting;
        _hexPlanetHudRepo.Processed += OnProcessed;
        _hexPlanetHudRepo.ChosenTileChanged += OnChosenTileChanged;

        _chunkRepo = chunkRepo;
        _tileRepo = tileRepo;
        _pointRepo = pointRepo;
        _hexPlanetManagerRepo = hexPlanetManagerRepo;
        _selectTileViewerRepo = selectTileViewerRepo;
        _miniMapManagerRepo = miniMapManagerRepo;
        _longitudeLatitudeRepo = longitudeLatitudeRepo;
        _orbitCameraRepo = orbitCameraRepo;
        _hexPlanetManagerService = hexPlanetManagerService;
        _hexPlanetHudService = hexPlanetHudService;
        _tileService = tileService;
        _unitManagerService = unitManagerService;
        _miniMapManagerService = miniMapManagerService;
        _miniMapService = miniMapService;
    }

    public void ReleaseEvents()
    {
        _hexPlanetHudRepo.Ready -= OnReady;
        _hexPlanetHudRepo.TreeExiting -= OnTreeExiting;
        _hexPlanetHudRepo.Processed -= OnProcessed;
        _hexPlanetHudRepo.ChosenTileChanged -= OnChosenTileChanged;
    }

    private IHexPlanetHud Self => _hexPlanetHudRepo.Singleton!;

    private void OnReady()
    {
        // 按照指定的高程分割数量确定 UI
        _hexPlanetHudService.InitElevationAndWaterVSlider();
        // 绑定事件
        _orbitCameraRepo.Moved += OnCameraMoved;
        _orbitCameraRepo.Transformed += OnCameraTransformed;
        _hexPlanetManagerRepo.NewPlanetGenerated += UpdateNewPlanetInfo;
        _hexPlanetManagerRepo.NewPlanetGenerated += InitMiniMap;

        // 初始化相机位置相关功能
        OnCameraMoved(_orbitCameraRepo.Singleton!.GetFocusBasePos(), 0f);
        OnCameraTransformed(_hexPlanetManagerRepo.Singleton!.GetViewport().GetCamera3D().GetGlobalTransform(), 0f);
        _hexPlanetHudRepo.SetEditMode(Self.EditCheckButton!.ButtonPressed);
        _hexPlanetHudRepo.SetLabelMode(Self.ShowLableOptionButton!.Selected);
        _hexPlanetHudRepo.SetTerrain(0);
        UpdateNewPlanetInfo();
        InitSignals();

        Self.RectMap!.Texture = _miniMapService.GenerateRectMap();
        _miniMapManagerService.Init(_orbitCameraRepo.Singleton!.GetFocusBasePos());
    }

    private void OnTreeExiting()
    {
        _orbitCameraRepo.Moved -= OnCameraMoved;
        _orbitCameraRepo.Transformed -= OnCameraTransformed;
        _hexPlanetManagerRepo.NewPlanetGenerated -= UpdateNewPlanetInfo;
        _hexPlanetManagerRepo.NewPlanetGenerated -= InitMiniMap;
    }

    private void OnProcessed(double obj)
    {
        if (Self.GetViewport().GuiGetHoveredControl() == Self.SubViewportContainer)
        {
            if (Input.IsMouseButtonPressed(MouseButton.Left))
            {
                HandleInput();
                return;
            }

            if (_hexPlanetManagerService.UpdateUiInEditMode())
                return;
        }
        else if (Self.GetViewport().GuiGetHoveredControl() == Self.MiniMapContainer
                 && Input.IsMouseButtonPressed(MouseButton.Left))
        {
            _miniMapManagerRepo.Singleton!.ClickOnMiniMap();
        }

        Self.PreviousTile = null;
    }

    private void OnChosenTileChanged(Tile? chosenTile)
    {
        if (chosenTile != null)
        {
            Self.IdLineEdit!.Text = chosenTile.Id.ToString();
            Self.ChunkLineEdit!.Text = chosenTile.ChunkId.ToString();
            var sa = _pointRepo.GetSphereAxial(chosenTile);
            Self.CoordsLineEdit!.Text = sa.ToString();
            Self.CoordsLineEdit.TooltipText = Self.CoordsLineEdit.Text;
            Self.HeightLineEdit!.Text = $"{_hexPlanetManagerRepo.GetHeight(chosenTile):F4}";
            Self.ElevationLineEdit!.Text = chosenTile.Data.Elevation.ToString();
            var lonLat = sa.ToLongitudeAndLatitude();
            Self.LonLineEdit!.Text = lonLat.GetLongitudeString();
            Self.LonLineEdit.TooltipText = Self.LonLineEdit.Text;
            Self.LatLineEdit!.Text = lonLat.GetLatitudeString();
            Self.LatLineEdit.TooltipText = Self.LatLineEdit.Text;
        }
        else
        {
            Self.IdLineEdit!.Text = "-";
            Self.ChunkLineEdit!.Text = "-";
            Self.CoordsLineEdit!.Text = "-";
            Self.CoordsLineEdit.TooltipText = null;
            Self.HeightLineEdit!.Text = "-";
            Self.ElevationLineEdit!.Text = "-";
            Self.LonLineEdit!.Text = "-";
            Self.LonLineEdit.TooltipText = ""; // 试了一下，null 和 "" 效果一样
            Self.LatLineEdit!.Text = "-";
            Self.LatLineEdit.TooltipText = null;
        }
    }

    private void InitSignals()
    {
        Self.WireframeCheckButton!.Toggled += toggle =>
            _hexPlanetManagerRepo.Singleton!.GetViewport()
                .SetDebugDraw(toggle ? Viewport.DebugDrawEnum.Wireframe : Viewport.DebugDrawEnum.Disabled);

        Self.CelestialMotionCheckButton!.Toggled += toggle =>
            _hexPlanetManagerRepo.Singleton!.PlanetRevolution = _hexPlanetManagerRepo.Singleton.PlanetRotation =
                _hexPlanetManagerRepo.Singleton.SatelliteRevolution =
                    _hexPlanetManagerRepo.Singleton.SatelliteRotation = toggle;
        Self.LatLonFixCheckButton!.Toggled += _longitudeLatitudeRepo.FixLatLon;
        Self.PlanetTabBar!.TabClicked += _ => Self.PlanetGrid!.Visible = !Self.PlanetGrid.Visible;

        Self.RadiusLineEdit!.TextSubmitted += text =>
        {
            if (float.TryParse(text, out var radius))
                _hexPlanetManagerRepo.Radius = radius;
            else
                Self.RadiusLineEdit.Text = $"{_hexPlanetManagerRepo.Radius:F2}";
        };

        Self.DivisionLineEdit!.TextSubmitted += text =>
        {
            if (int.TryParse(text, out var division))
                _hexPlanetManagerRepo.Divisions = division;
            else
                Self.DivisionLineEdit.Text = $"{_hexPlanetManagerRepo.Divisions}";
        };

        Self.ChunkDivisionLineEdit!.TextSubmitted += text =>
        {
            if (int.TryParse(text, out var chunkDivision))
                _hexPlanetManagerRepo.ChunkDivisions = chunkDivision;
            else
                Self.DivisionLineEdit.Text = $"{_hexPlanetManagerRepo.ChunkDivisions}";
        };

        Self.TileTabBar!.TabClicked += _ =>
        {
            var vis = !Self.TileVBox!.Visible;
            Self.TileVBox.Visible = vis;
            Self.TileGrid!.Visible = vis;
        };

        Self.EditTabBar!.TabClicked += _ =>
        {
            var vis = !Self.EditGrid!.Visible;
            Self.EditGrid.Visible = vis;
            if (vis)
            {
                _hexPlanetHudRepo.SetTerrain(Self.TerrainOptionButton!.Selected);
                _hexPlanetHudRepo.SetElevation(Self.ElevationVSlider!.Value);
            }
            else
            {
                _hexPlanetHudRepo.SetApplyTerrain(false);
                _hexPlanetHudRepo.SetApplyElevation(false);
            }
        };

        Self.EditCheckButton!.Toggled += _hexPlanetHudRepo.SetEditMode;
        Self.ShowLableOptionButton!.ItemSelected += _hexPlanetHudRepo.SetLabelMode;
        Self.TerrainOptionButton!.ItemSelected += _hexPlanetHudRepo.SetTerrain;
        Self.ElevationVSlider!.ValueChanged += _hexPlanetHudRepo.SetElevation;
        Self.ElevationCheckButton!.Toggled += _hexPlanetHudRepo.SetApplyElevation;
        Self.WaterVSlider!.ValueChanged += _hexPlanetHudRepo.SetWaterLevel;
        Self.WaterCheckButton!.Toggled += _hexPlanetHudRepo.SetApplyWaterLevel;
        Self.BrushHSlider!.ValueChanged += _hexPlanetHudRepo.SetBrushSize;
        Self.RiverOptionButton!.ItemSelected += _hexPlanetHudRepo.SetRiverMode;
        Self.RoadOptionButton!.ItemSelected += _hexPlanetHudRepo.SetRoadMode;
        Self.UrbanCheckButton!.Toggled += _hexPlanetHudRepo.SetApplyUrbanLevel;
        Self.UrbanHSlider!.ValueChanged += _hexPlanetHudRepo.SetUrbanLevel;
        Self.FarmCheckButton!.Toggled += _hexPlanetHudRepo.SetApplyFarmLevel;
        Self.FarmHSlider!.ValueChanged += _hexPlanetHudRepo.SetFarmLevel;
        Self.PlantCheckButton!.Toggled += _hexPlanetHudRepo.SetApplyPlantLevel;
        Self.PlantHSlider!.ValueChanged += _hexPlanetHudRepo.SetPlantLevel;
        Self.WallOptionButton!.ItemSelected += _hexPlanetHudRepo.SetWalledMode;
        Self.SpecialFeatureCheckButton!.Toggled += _hexPlanetHudRepo.SetApplySpecialIndex;
        Self.SpecialFeatureOptionButton!.ItemSelected += _hexPlanetHudRepo.SetSpecialIndex;
    }

    private void InitMiniMap() =>
        Self.RectMap!.Texture = _miniMapService.GenerateRectMap();

    private void UpdateNewPlanetInfo()
    {
        Self.RadiusLineEdit!.Text = $"{_hexPlanetManagerRepo.Radius:F2}";
        Self.DivisionLineEdit!.Text = $"{_hexPlanetManagerRepo.Divisions}";
        Self.ChunkDivisionLineEdit!.Text = $"{_hexPlanetManagerRepo.ChunkDivisions}";
        Self.ChunkCountLabel!.Text = $"分块总数：{_chunkRepo.GetCount()}";
        Self.TileCountLabel!.Text = $"地块总数：{_tileRepo.GetCount()}";
        Self.ChosenTile = null;
    }

    private void OnCameraMoved(Vector3 pos, float delta)
    {
        var longLat = LongitudeLatitudeCoords.From(_hexPlanetManagerRepo.Singleton!.ToPlanetLocal(pos));
        Self.CamLatLonLabel!.Text = $"相机位置：{longLat}";
    }

    private void OnCameraTransformed(Transform3D transform, float delta)
    {
        var northPolePoint = Vector3.Up;
        var posNormal = transform.Origin.Normalized();
        var dirNorth = Math3dUtil.DirectionBetweenPointsOnSphere(posNormal, northPolePoint);
        var angleToNorth = transform.Basis.Y.Slide(posNormal).SignedAngleTo(dirNorth, -posNormal);
        Self.CompassPanel!.Rotation = angleToNorth;

        var posLocal = _hexPlanetManagerRepo.Singleton!.ToPlanetLocal(_orbitCameraRepo.Singleton!.GetFocusBasePos());
        var longLat = LongitudeLatitudeCoords.From(posLocal);
        var rectMapMaterial = Self.RectMap!.Material as ShaderMaterial;
        rectMapMaterial?.SetShaderParameter("lon", longLat.Longitude);
        rectMapMaterial?.SetShaderParameter("lat", longLat.Latitude);
        // rectMapMaterial?.SetShaderParameter("pos_normal", posLocal.Normalized()); // 非常奇怪，旋转时会改变……
        rectMapMaterial?.SetShaderParameter("angle_to_north", angleToNorth);
        // GD.Print($"lonLat: {longLat.Longitude}, {longLat.Latitude}; angleToNorth: {
        //     angleToNorth}; posNormal: {posNormal};");
    }

    private void HandleInput()
    {
        // 在 SubViewportContainer 上按下鼠标左键时，获取鼠标位置地块并更新
        Self.ChosenTile = _hexPlanetManagerService.GetTileUnderCursor();
        if (Self.ChosenTile != null)
        {
            if (Self.PreviousTile != null && Self.PreviousTile != Self.ChosenTile)
                ValidateDrag(Self.ChosenTile);
            else
                Self.IsDrag = false;
            if (Self.TileOverrider.EditMode)
            {
                _tileService.EditTiles(Self.ChosenTile, Self.TileOverrider, Self.IsDrag,
                    Self.PreviousTile, Self.DragTile);
                Self.ChosenTile = Self.ChosenTile; // 刷新 GUI 地块信息
                // 编辑模式下绘制选择地块框
                _selectTileViewerRepo.Singleton!.SelectEditingTile(Self.ChosenTile);
            }
            else if (Input.IsActionJustPressed("choose_unit"))
                _unitManagerService.FindPath(Self.ChosenTile);

            Self.PreviousTile = Self.ChosenTile;
        }
        else
        {
            if (!Self.TileOverrider.EditMode)
                _unitManagerService.FindPath(null);
            else
                // 清理选择地块框
                _selectTileViewerRepo.Singleton!.CleanEditingTile();
            Self.PreviousTile = null;
        }
    }

    private void ValidateDrag(Tile currentTile)
    {
        Self.DragTile = currentTile;
        Self.IsDrag = currentTile.IsNeighbor(Self.PreviousTile!);
    }
}