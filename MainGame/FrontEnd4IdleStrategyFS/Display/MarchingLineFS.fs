namespace FrontEnd4IdleStrategyFS.Display

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
    val mutable _marchingArmyId: int

    member this.Init id population fromV toV color =
        this._marchingArmyId <- id

        this._speed <-
            match population with
            | p when p < 10 -> 50 // 人数小于 10 人，2 秒后到达目的地
            | p when p < 50 -> 25 // 小于 50 人，4 秒后
            | p when p < 200 -> 15 // 小于 200 人，7 秒左右后
            | p when p < 1000 -> 10 // 小于 1000 人，10 秒后
            | _ -> 5 // 大于 1000 人，20 秒后
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
        if _progressBar.Value.Value >= 100 then
            this.QueueFree()
        else
            _progressBar.Value.Value <- _progressBar.Value.Value + float this._speed * delta