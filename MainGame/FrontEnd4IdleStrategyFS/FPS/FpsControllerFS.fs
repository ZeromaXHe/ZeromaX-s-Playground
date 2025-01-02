namespace FrontEnd4IdleStrategyFS.FPS

open FrontEnd4IdleStrategyFS.Global
open Godot

type FpsControllerFS() as this =
    inherit CharacterBody3D()

    let mutable mouseInput = false
    let mutable mouseRotation = Vector3.Zero
    let mutable rotationInput = 0f
    let mutable tiltInput = 0f
    let mutable playerRotation = Vector3.Zero
    let mutable cameraRotation = Vector3.Zero
    let mutable isCrouching = false

    let updateCamera delta =
        // GD.Print $"updating camera... tilt:{tiltInput}, rotation:{rotationInput}"
        this.currentRotation <- rotationInput
        mouseRotation.X <- mouseRotation.X + tiltInput * delta
        mouseRotation.X <- Mathf.Clamp(mouseRotation.X, this.tiltLowerLimit, this.tiltUpperLimit)
        mouseRotation.Y <- mouseRotation.Y + rotationInput * delta

        playerRotation <- Vector3(0f, mouseRotation.Y, 0f)
        cameraRotation <- Vector3(mouseRotation.X, 0f, 0f)

        let mutable transform = this.cameraController.Transform
        transform.Basis <- Basis.FromEuler cameraRotation
        this.cameraController.Transform <- transform
        let mutable rotation = this.cameraController.Rotation
        rotation.Z <- 0f
        this.cameraController.Rotation <- rotation

        let mutable globalTransform = this.GlobalTransform
        globalTransform.Basis <- Basis.FromEuler playerRotation
        this.GlobalTransform <- globalTransform

        rotationInput <- 0f
        tiltInput <- 0f

    member val jumpVelocity = 4.5f with get, set
    member val mouseSensitivity = 0.5f with get, set
    member val tiltLowerLimit = Mathf.DegToRad(-90f) with get, set
    member val tiltUpperLimit = Mathf.DegToRad(90f) with get, set
    member val cameraController: Camera3D = null with get, set
    member val animationPlayer: AnimationPlayer = null with get, set
    member val crouchShapeCast: ShapeCast3D = null with get, set
    member val currentRotation = 0f with get, set

    member this.UpdateGravity delta =
        let mutable velocity = this.Velocity
        velocity.Y <- velocity.Y + this.GetGravity().Y * delta
        this.Velocity <- velocity

    member this.UpdateInput speed acceleration deceleration =
        let mutable velocity = this.Velocity
        // 获取输入方向并处理移动/减速
        // 作为好的实践，你应该使用自定义的游戏操作替换 UI 操作
        let inputDir =
            Input.GetVector("move_left", "move_right", "move_forward", "move_back")

        let direction =
            (this.Transform.Basis * Vector3(inputDir.X, 0f, inputDir.Y)).Normalized()

        if direction <> Vector3.Zero then
            velocity.X <- Mathf.Lerp(this.Velocity.X, direction.X * speed, acceleration)
            velocity.Z <- Mathf.Lerp(this.Velocity.Z, direction.Z * speed, acceleration)
        else
            velocity.X <- Mathf.MoveToward(velocity.X, 0f, deceleration)
            velocity.Z <- Mathf.MoveToward(velocity.Z, 0f, deceleration)

        this.Velocity <- velocity

    member this.UpdateVelocity() = this.MoveAndSlide() |> ignore

    override this._UnhandledInput event =
        mouseInput <-
            event :? InputEventMouseMotion
            && Input.GetMouseMode() = Input.MouseModeEnum.Captured

        if mouseInput then
            let e = event :?> InputEventMouseMotion
            rotationInput <- -e.Relative.X * this.mouseSensitivity
            tiltInput <- -e.Relative.Y * this.mouseSensitivity
    // GD.Print(Vector2(rotationInput, tiltInput))

    override this._Input event =
        if event.IsActionPressed "exit" then
            this.GetTree().Quit()

    override this._PhysicsProcess delta =
        FpsGlobalNodeFS.Instance.debug.AddProperty "MovementSpeed" (this.Velocity.Length()) 2
        FpsGlobalNodeFS.Instance.debug.AddProperty "MouseRotation" mouseRotation 3
        // 根据鼠标移动，更新摄像机移动
        updateCamera <| float32 delta

    override this._Ready() =
        // 获取鼠标输入
        Input.MouseMode <- Input.MouseModeEnum.Captured
        // 将 CharacterBody3D 添加为 Crouch ShapeCast 的碰撞检查排除项
        // 注意：这里不能传 this，否则无效（猜测可能是因为我们这里是 F# 代码？不知道 C# 可不可以直接 this）
        this.crouchShapeCast.AddException <| this.GetNode<CharacterBody3D> "."

        this.animationPlayer.add_AnimationStarted (fun animName ->
            if "Crouch".Equals animName then
                isCrouching <- not isCrouching)
