namespace FrontEnd4IdleStrategyFS.FPS

open FrontEnd4IdleStrategyFS.Global
open Godot
open FrontEndCommonFS.Util

type FpsControllerFS() as this =
    inherit CharacterBody3D()
    let mutable speed = 0f
    let mutable mouseInput = false
    let mutable mouseRotation = Vector3.Zero
    let mutable rotationInput = 0f
    let mutable tiltInput = 0f
    let mutable playerRotation = Vector3.Zero
    let mutable cameraRotation = Vector3.Zero
    let mutable isCrouching = false

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

    let setMovementSpeed state =
        match state with
        | "default" -> speed <- this.speedDefault
        | "crouching" -> speed <- this.speedCrouch
        | _ -> ()

    let crouching state =
        if state then
            this.animationPlayer.Play("Crouch", 0, this.crouchSpeed)
            setMovementSpeed "crouching"
        else
            this.animationPlayer.Play("Crouch", 0, -this.crouchSpeed, true)
            setMovementSpeed "default"

    let toggleCrouch () =
        if isCrouching && not <| this.crouchShapeCast.IsColliding() then
            crouching false
        elif not isCrouching then
            crouching true

    let rec uncrouchCheck () =
        if not <| this.crouchShapeCast.IsColliding() then
            crouching false

        if this.crouchShapeCast.IsColliding() then
            async {
                let! _ =
                    this.ToSignal(this.GetTree().CreateTimer(0.1), Timer.SignalName.Timeout)
                    |> AwaitUtil.awaiterToTask
                    |> Async.AwaitTask
                // 如果蹲在障碍物下，需要间隔 0.1s 递归检测是否弹起
                uncrouchCheck ()
            }
            |> Async.Start

    member val speedDefault = 5f with get, set
    member val speedCrouch = 2f with get, set
    member val acceleration = 0.1f with get, set
    member val deceleration = 0.25f with get, set
    member val togCrouch = false with get, set // 貌似这里默认为 true 的话，场景里面为 false 会改不到实际值？
    member val jumpVelocity = 4.5f with get, set
    member val crouchSpeed = 7.0f with get, set
    member val mouseSensitivity = 0.5f with get, set
    member val tiltLowerLimit = Mathf.DegToRad(-90f) with get, set
    member val tiltUpperLimit = Mathf.DegToRad(90f) with get, set
    member val cameraController: Camera3D = null with get, set
    member val animationPlayer: AnimationPlayer = null with get, set
    member val crouchShapeCast: ShapeCast3D = null with get, set

    interface IPlayer with
        override this.speedDefault = this.speedDefault
        override this.Velocity = this.Velocity
        override this.IsOnFloor() = this.IsOnFloor()

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

        if event.IsActionPressed "crouch" && this.IsOnFloor() && this.togCrouch then
            toggleCrouch ()

        if
            event.IsActionPressed "crouch"
            && not isCrouching
            && this.IsOnFloor()
            && not this.togCrouch
        then
            crouching true

        if event.IsActionReleased "crouch" && not this.togCrouch then
            if not <| this.crouchShapeCast.IsColliding() then
                crouching false
            else
                uncrouchCheck ()

    override this._PhysicsProcess delta =
        FpsGlobalNodeFS.Instance.debug.AddProperty "MovementSpeed" speed 2
        FpsGlobalNodeFS.Instance.debug.AddProperty "MouseRotation" mouseRotation 3
        let mutable velocity = this.Velocity
        // 根据鼠标移动，更新摄像机移动
        updateCamera <| float32 delta
        // 增加重力
        if not <| this.IsOnFloor() then
            velocity <- velocity + this.GetGravity() * float32 delta

        // 处理跳跃
        if Input.IsActionJustPressed "jump" && this.IsOnFloor() && not isCrouching then
            velocity.Y <- this.jumpVelocity
        // 获取输入方向并处理移动/减速
        // 作为好的实践，你应该使用自定义的游戏操作替换 UI 操作
        let inputDir =
            Input.GetVector("move_left", "move_right", "move_forward", "move_back")

        let direction =
            (this.Transform.Basis * Vector3(inputDir.X, 0f, inputDir.Y)).Normalized()

        if direction <> Vector3.Zero then
            velocity.X <- Mathf.Lerp(this.Velocity.X, direction.X * speed, this.acceleration)
            velocity.Z <- Mathf.Lerp(this.Velocity.Z, direction.Z * speed, this.acceleration)
        else
            velocity.X <- Mathf.MoveToward(velocity.X, 0f, this.deceleration)
            velocity.Z <- Mathf.MoveToward(velocity.Z, 0f, this.deceleration)

        this.Velocity <- velocity
        this.MoveAndSlide() |> ignore

    override this._Ready() =
        FpsGlobalNodeFS.Instance.player <- this
        // 获取鼠标输入
        Input.MouseMode <- Input.MouseModeEnum.Captured
        // 设置默认速度
        speed <- this.speedDefault
        // 将 CharacterBody3D 添加为 Crouch ShapeCast 的碰撞检查排除项
        // 注意：这里不能传 this，否则无效（猜测可能是因为我们这里是 F# 代码？不知道 C# 可不可以直接 this）
        this.crouchShapeCast.AddException <| this.GetNode<CharacterBody3D> "."

        this.animationPlayer.add_AnimationStarted (fun animName ->
            if "Crouch".Equals animName then
                isCrouching <- not isCrouching)
