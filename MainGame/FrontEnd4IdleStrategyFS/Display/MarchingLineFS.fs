namespace FrontEnd4IdleStrategyFS.Display

open BackEnd4IdleStrategyFS.Game.DomainT
open Godot

type MarchingLineFS() as this =
    inherit Line2D()

    let _panelContainer = lazy this.GetNode<PanelContainer> "PanelContainer"

    let _populationLabel =
        lazy this.GetNode<Label> "PanelContainer/VBoxContainer/Population"

    let _progressBar =
        lazy this.GetNode<ProgressBar> "PanelContainer/VBoxContainer/ProgressBar"

    [<DefaultValue>]
    val mutable _speed: int

    [<DefaultValue>]
    val mutable _marchingArmyId: MarchingArmyId

    static member val idMap = Map.empty<MarchingArmyId, MarchingLineFS> with get, set

    static member ClearById id =
        match MarchingLineFS.idMap.TryFind id with
        | Some line ->
            MarchingLineFS.idMap.Remove id |> ignore
            line.QueueFree()
        | None -> ()

    member this.Init id population speed fromV toV color =
        MarchingLineFS.idMap <- MarchingLineFS.idMap.Add(id, this)
        this._marchingArmyId <- id
        this._speed <- speed
        // 线条
        this.Position <- fromV
        this.Points <- [| Vector2.Zero; toV - fromV |]
        // GD.Print $"""MarchingLine Init Points: {this.Points.Join ","}"""
        this.DefaultColor <- color
        // 信息栏
        _populationLabel.Value.Text <- $"{population}"
        _panelContainer.Value.Position <- (toV - fromV) / 2.0f - _panelContainer.Value.Size / 2.0f
        // 进度条
        _progressBar.Value.Value <- 0

        if toV.X < fromV.X then
            ProgressBar.FillModeEnum.EndToBegin |> int |> _progressBar.Value.SetFillMode

        let styleBoxFlat = new StyleBoxFlat()
        styleBoxFlat.BgColor <- color
        _progressBar.Value.Set("theme_override_styles/fill", Variant.CreateFrom(styleBoxFlat))

    override this._Process(delta) =
        if _progressBar.Value.Value < 100 then
            _progressBar.Value.Value <- _progressBar.Value.Value + float this._speed * delta |> min <| 100.0
