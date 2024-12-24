namespace FrontEndToolFS.PdcxsShadertoy

open Godot

type ShadertoyTestFS() as this =
    inherit Control()

    let test06 = lazy this.GetNode<TextureRect> "Test06"

    let mutable test06ShaderMat: ShaderMaterial = null

    override this._Ready() =
        test06ShaderMat <- test06.Value.Material :?> ShaderMaterial

    override this._Process _ =
        if Engine.IsEditorHint() then
            // TODO：不知道怎么从编辑器里获取鼠标坐标…… 所以 Shadertoy iMouse 无法在编辑器中查看，目前只有启动场景才行。
            // https://github.com/godotengine/godot-proposals/issues/3376
            // 感觉好像得作为 EditorPlugin 插件来写，但那样有点麻烦…… 暂时先偷懒吧
            let coords = EditorInterface.Singleton.GetEditorViewport2D().GetMousePosition()
            // let coords = this.GetViewport().GetMousePosition()
            test06ShaderMat.SetShaderParameter("iMouse", Vector3(coords.X, coords.Y, 1.0f))
        else
            let coords = this.GetLocalMousePosition()

            let mouseLeft =
                if Input.IsMouseButtonPressed MouseButton.Left then
                    1.0f
                else
                    0.0f

            test06ShaderMat.SetShaderParameter("iMouse", Vector3(coords.X, coords.Y, mouseLeft))
