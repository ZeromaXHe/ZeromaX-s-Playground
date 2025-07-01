namespace TO.Domains.Types.PlanetHuds

open System
open TO.Domains.Types.Godots

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-07-01 09:32:01
[<Interface>]
[<AllowNullLiteral>] // 因为不是 Tool，所以编辑器内可能为空；这注解还有向上传染性，所有父接口也得加…… 向下不影响
type ISelectTileViewer =
    inherit IMeshInstance3D
    abstract HoverTileId: int Nullable with get, set
    abstract SelectedTileId: int Nullable with get, set

[<Interface>]
type ISelectTileViewerQuery =
    abstract SelectTileViewerOpt: ISelectTileViewer option // 因为不是 Tool，所以编辑器内可能为空

type UpdateInEditMode = unit -> unit

[<Interface>]
type ISelectTileViewerCommand =
    abstract UpdateInEditMode: UpdateInEditMode
