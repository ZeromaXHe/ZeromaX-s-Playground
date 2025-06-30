namespace TO.Domains.Types.PlanetHuds

open Godot
open TO.Domains.Types.Godots

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-29 14:13:29
[<Interface>]
[<AllowNullLiteral>] // 因为 PlanetHud 不是 Tool，所以编辑器内可能为空；这注解还有向上传染性，所有父接口也得加…… 向下不影响
type IPlanetHud =
    inherit IControl
    // =====【on-ready】=====
    abstract CamLonLatLabel: Label
    abstract CompassPanel: PanelContainer
    abstract RectMap: TextureRect
    abstract RadiusLineEdit: LineEdit
    abstract DivisionLineEdit: LineEdit
    abstract ChunkDivisionLineEdit: LineEdit
    abstract ElevationVSlider: VSlider
    abstract WaterVSlider: VSlider
