namespace FrontEnd4IdleStrategyFS.Display

open System.Threading
open BackEnd4IdleStrategyFS.Game.DomainT
open BackEnd4IdleStrategyFS.Game.EventT
open FrontEnd4IdleStrategyFS.Global.Common
open Godot
open FSharp.Control.Reactive
open FrontEnd4IdleStrategyFS.Global

type MapBoardFS() as this =
    inherit Node2D()

    let _globalNode = lazy this.GetNode<GlobalNodeFS> "/root/GlobalNode"

    let _baseTerrain = lazy this.GetNode<TileMapLayer> "BaseTerrain"
    let _feature = lazy this.GetNode<TileMapLayer> "Feature"
    let _territory = lazy this.GetNode<TileMapLayer> "Territory"
    let _marchingLines = lazy this.GetNode<Node2D> "MarchingLines"
    let _tileGuis = lazy this.GetNode<Control> "TileGUIs"

    let _tileGuiScene =
        GD.Load("res://game/inGame/map/scenes/TileGUI.tscn") :?> PackedScene

    let _marchingLineScene =
        GD.Load("res://game/inGame/map/scenes/MarchingLine.tscn") :?> PackedScene

    [<DefaultValue>]
    val mutable _playerCount: int

    let territorySrcId = 0

    let territoryAtlasCoords =
        [| Vector2I(0, 2)
           Vector2I(1, 2)
           Vector2I(2, 2)
           Vector2I(3, 2)
           Vector2I(0, 3)
           Vector2I(1, 3)
           Vector2I(2, 3)
           Vector2I(3, 3) |]

    /// 不能简写成下面形式，不然会提前触发 _territory 的 GetNode，拿不到节点
    /// let getTileCoordGlobalPosition =
    ///     _territory.Value.MapToLocal >> _territory.Value.ToGlobal
    let getTileCoordGlobalPosition coord =
        coord |> _territory.Value.MapToLocal |> _territory.Value.ToGlobal

    let showMarchingArmy (e: MarchingArmyAddedEvent) =
        let (PlayerId playerId) = e.PlayerId
        GD.Print $"AI 玩家 {playerId} 发出部队 {e.MarchingArmyId} 人数为 {e.Population}"
        // 初始化一次行军部队
        let marchingLine = _marchingLineScene.Instantiate<MarchingLineFS>()
        _marchingLines.Value.AddChild marchingLine

        let entry = _globalNode.Value.IdleStrategyEntry.Value

        marchingLine.Init
            e.MarchingArmyId
            e.Population
            (entry.MarchingSpeed e.Population)
            (entry.QueryTileById e.FromTileId
             |> _.Coord
             |> BackEndUtil.fromI
             |> getTileCoordGlobalPosition)
            (entry.QueryTileById e.ToTileId
             |> _.Coord
             |> BackEndUtil.fromI
             |> getTileCoordGlobalPosition)
            Constants.playerColors[playerId - 1] // Player 的 Id 从 1 开始，所以要减一

    let showTileGui id coord population =
        let tileGui = _tileGuiScene.Instantiate<TileGuiFS>()
        _tileGuis.Value.AddChild tileGui
        tileGui.Init id coord population (coord |> BackEndUtil.fromI |> getTileCoordGlobalPosition)

    override this._Ready() =
        // 清除用于编辑器中预览的单元格
        // _feature.Clear();
        _territory.Value.Clear()

        let globalNode = _globalNode.Value
        globalNode.InitIdleStrategyGame _baseTerrain.Value this._playerCount
        let entry = globalNode.IdleStrategyEntry.Value

        entry.GameTicked
        |> ObservableSyncContextUtil.subscribe (fun e -> GD.Print $"tick {e}")

        entry.GameFirstArmyGenerated
        |> ObservableSyncContextUtil.subscribe (fun _ -> GD.Print "第一次出兵！！！")

        entry.TileConquered
        |> ObservableSyncContextUtil.subscribePost (fun e ->
            TileGuiFS.ChangePopulationById e.TileId e.AfterPopulation

            match e.ConquerorId with
            | None ->
                _territory.Value.EraseCell <| BackEndUtil.fromI e.Coord
                TileGuiFS.ChangePopulationById e.TileId 0<Pop>
            | Some(PlayerId conquerorIdInt) ->
                // Player 的 Id 从 1 开始，所以要减一
                _territory.Value.SetCell(
                    BackEndUtil.fromI e.Coord,
                    territorySrcId,
                    territoryAtlasCoords[conquerorIdInt - 1]
                ))

        entry.TileConquered
        |> Observable.filter _.LoserId.IsNone
        |> ObservableSyncContextUtil.subscribePost (fun e ->
            GD.Print $"玩家 {e.ConquerorId} 占领无主地块 {e.Coord}"

            if not <| TileGuiFS.ContainsId e.TileId then
                showTileGui e.TileId e.Coord e.AfterPopulation)

        entry.TilePopulationChanged
        |> ObservableSyncContextUtil.subscribePost (fun e -> TileGuiFS.ChangePopulationById e.TileId e.AfterPopulation)

        entry.MarchingArmyAdded
        |> ObservableSyncContextUtil.subscribePost showMarchingArmy

        entry.MarchingArmyArrived
        |> ObservableSyncContextUtil.subscribePost (fun e -> MarchingLineFS.ClearById e.MarchingArmyId)

        // 必须在同步上下文中执行，否则 Init 内容不会被响应式编程 Subscribe 监听到（会比上面监听逻辑更早执行）
        SynchronizationContext.Current.Post((fun _ -> _globalNode.Value.IdleStrategyEntry.Value.Init()), null)
        GD.Print $"MapBoard 初始化完成! playerCount:{this._playerCount}"
