namespace FrontEnd4IdleStrategyFS.FPS

open FrontEnd4IdleStrategyFS.Global
open Godot

type DebugPanelFS() as this =
    inherit PanelContainer()
    let propertyContainer = lazy this.GetNode<VBoxContainer> "%PropertyContainer"
    let fpsGlobalNode = lazy this.GetNode<FpsGlobalNodeFS> "/root/FpsGlobalNode"
    let mutable framesPerSecond: string = null

    let addProperty (title: string) value order =
        // 尝试寻找名字一样的标签节点
        let mutable target = propertyContainer.Value.FindChild(title, true, false)

        if target = null then
            let property = new Label()
            target <- property
            propertyContainer.Value.AddChild property
            property.Name <- title // 设置名字为 title
            property.Text <- title + ":" + string value
        elif this.Visible then
            let property = target :?> Label
            property.Text <- title + ":" + string value
            propertyContainer.Value.MoveChild(target, order)

    interface IDebug with
        override this.AddProperty title value order = addProperty title value order

    override this._Input event =
        // 切换调试面板
        if event.IsActionPressed "debug" then
            this.Visible <- not this.Visible

    override this._Ready() =
        fpsGlobalNode.Value.debug <- this
        this.Visible <- false

    override this._Process(delta) =
        if this.Visible then
            // 使用 delta 来获取近似每秒帧率，并舍入到两位小数
            // 注意：取消 VSync 如果 fps 卡在 60
            // 获取帧率
            // framesPerSecond <- string <| Engine.GetFramesPerSecond() // 每秒更新一次
            framesPerSecond <- $"%.2f{1.0 / delta}"
            fpsGlobalNode.Value.debug.AddProperty "FPS" framesPerSecond 0
