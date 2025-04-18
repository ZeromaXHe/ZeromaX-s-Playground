using Domains.Models.Entities.PlanetGenerates;
using Godot;
using GodotNodes.Abstractions;

namespace Nodes.Abstractions;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-16 15:28:16
public interface IHexPlanetHud: IControl
{
    #region on-ready 节点

    SubViewportContainer? SubViewportContainer { get; }

    CheckButton? WireframeCheckButton { get; }
    CheckButton? CelestialMotionCheckButton { get; }

    // 小地图
    SubViewportContainer? MiniMapContainer { get; }
    Label? CamLatLonLabel { get; }
    CheckButton? LatLonFixCheckButton { get; }

    // 指南针
    PanelContainer? CompassPanel { get; }

    // 矩形地图测试
    TextureRect? RectMap { get; }

    // 星球信息
    TabBar? PlanetTabBar { get; }
    GridContainer? PlanetGrid { get; }
    LineEdit? RadiusLineEdit { get; }
    LineEdit? DivisionLineEdit { get; }
    LineEdit? ChunkDivisionLineEdit { get; }

    // 地块信息
    TabBar? TileTabBar { get; }
    VBoxContainer? TileVBox { get; }
    Label? ChunkCountLabel { get; }
    Label? TileCountLabel { get; }
    OptionButton? ShowLableOptionButton { get; }
    GridContainer? TileGrid { get; }
    LineEdit? IdLineEdit { get; }
    LineEdit? ChunkLineEdit { get; }
    LineEdit? CoordsLineEdit { get; }
    LineEdit? HeightLineEdit { get; }
    LineEdit? ElevationLineEdit { get; }
    LineEdit? LonLineEdit { get; }
    LineEdit? LatLineEdit { get; }

    // 编辑功能
    CheckButton? EditCheckButton { get; }
    TabBar? EditTabBar { get; }
    GridContainer? EditGrid { get; }
    OptionButton? TerrainOptionButton { get; }
    VSlider? ElevationVSlider { get; }
    CheckButton? ElevationCheckButton { get; }
    Label? ElevationValueLabel { get; }
    VSlider? WaterVSlider { get; }
    CheckButton? WaterCheckButton { get; }
    Label? WaterValueLabel { get; }
    Label? BrushLabel { get; }
    HSlider? BrushHSlider { get; }
    OptionButton? RiverOptionButton { get; }
    OptionButton? RoadOptionButton { get; }
    CheckButton? UrbanCheckButton { get; }
    HSlider? UrbanHSlider { get; }
    CheckButton? FarmCheckButton { get; }
    HSlider? FarmHSlider { get; }
    CheckButton? PlantCheckButton { get; }
    HSlider? PlantHSlider { get; }
    OptionButton? WallOptionButton { get; }
    CheckButton? SpecialFeatureCheckButton { get; }
    OptionButton? SpecialFeatureOptionButton { get; }

    #endregion

    Tile? ChosenTile { get; set; }
    bool IsDrag { get; set; }
    Tile? DragTile {get; set; }
    Tile? PreviousTile {get; set; }
}