namespace FrontEnd4IdleStrategyFS.FPS

open Godot

type FpsControllerFS() as this =
    inherit CharacterBody3D()

    let speed = 5f
    let jumpVelocity = 4.5f
    let mutable mouseInput = false
    let mutable mouseRotation = Vector3.Zero
    let mutable rotationInput = 0f
    let mutable tiltInput = 0f
    let mutable playerRotation = Vector3.Zero
    let mutable cameraRotation = Vector3.Zero

    let updateCamera delta =
        // GD.Print $"updating camera... tilt:{tiltInput}, rotation:{rotationInput}"
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

    member val mouseSensitivity = 0.5f with get, set
    member val tiltLowerLimit = Mathf.DegToRad(-90f) with get, set
    member val tiltUpperLimit = Mathf.DegToRad(90f) with get, set
    member val cameraController: Camera3D = null with get, set

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
        let mutable velocity = this.Velocity
        // 增加重力
        if not <| this.IsOnFloor() then
            velocity <- velocity + this.GetGravity() * float32 delta

        updateCamera <| float32 delta

        // 处理跳跃
        if Input.IsActionJustPressed "jump" && this.IsOnFloor() then
            velocity.Y <- jumpVelocity
        // 获取输入方向并处理移动/减速
        // 作为好的实践，你应该使用自定义的游戏操作替换 UI 操作
        let inputDir =
            Input.GetVector("move_left", "move_right", "move_forward", "move_back")

        let direction =
            (this.Transform.Basis * Vector3(inputDir.X, 0f, inputDir.Y)).Normalized()

        if direction <> Vector3.Zero then
            velocity.X <- direction.X * speed
            velocity.Z <- direction.Z * speed
        else
            velocity.X <- Mathf.MoveToward(velocity.X, 0f, speed)
            velocity.Z <- Mathf.MoveToward(velocity.Z, 0f, speed)

        this.Velocity <- velocity
        this.MoveAndSlide() |> ignore

    override this._Ready() =
        Input.MouseMode <- Input.MouseModeEnum.Captured
