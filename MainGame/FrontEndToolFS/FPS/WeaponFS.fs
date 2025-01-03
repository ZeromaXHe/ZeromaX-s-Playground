namespace FrontEndToolFS.FPS

open FrontEndCommonFS.Util
open Godot
open Microsoft.FSharp.Core

type IWeapons =
    abstract Name: StringName
    abstract Position: Vector3
    abstract Rotation: Vector3
    abstract Scale1: Vector3
    abstract Position2: Vector3
    abstract Rotation2: Vector3
    abstract Scale2: Vector3

    abstract SwayMin: Vector2
    abstract SwayMax: Vector2
    abstract SwaySpeedPosition: float32
    abstract SwaySpeedRotation: float32
    abstract SwayAmountPosition: float32
    abstract SwayAmountRotation: float32
    abstract IdleSwayAdjustment: float32
    abstract IdleSwayRotationStrength: float32
    abstract RandomSwayAmount: float32

    abstract Mesh: Mesh
    abstract ShadowMesh: Mesh
    abstract Mesh2: Mesh
    abstract ShadowMesh2: Mesh
    abstract Shadow: bool
    abstract DamageAmount: float32

type WeaponFS() as this =
    inherit Node3D()

    let weaponMesh = lazy this.GetNode<MeshInstance3D> "WeaponMesh"
    let weaponMesh2 = lazy this.GetNode<MeshInstance3D> "WeaponMesh/WeaponMesh2"
    let weaponShadow = lazy this.GetNode<MeshInstance3D> "WeaponShadow"
    let weaponShadow2 = lazy this.GetNode<MeshInstance3D> "WeaponShadow/WeaponShadow2"
    let mutable mouseMovement = Vector2.Zero
    let mutable randomSwayX = 0f
    let mutable randomSwayY = 0f
    let mutable randomSwayAmount = 0f
    let mutable time = 0f
    let mutable idleSwayAdjustment = 0f
    let mutable idleSwayRotationStrength = 0f
    let mutable _weaponType = Unchecked.defaultof<IWeapons>
    let mutable _reset = false

    let loadWeapon () =
        weaponMesh.Value.Mesh <- _weaponType.Mesh
        weaponShadow.Value.Mesh <- _weaponType.ShadowMesh
        weaponMesh2.Value.Mesh <- _weaponType.Mesh2
        weaponShadow2.Value.Mesh <- _weaponType.ShadowMesh2
        weaponMesh.Value.Position <- _weaponType.Position
        weaponShadow.Value.Position <- _weaponType.Position
        weaponMesh2.Value.Position <- _weaponType.Position2
        weaponShadow2.Value.Position <- _weaponType.Position2
        weaponMesh.Value.RotationDegrees <- _weaponType.Rotation
        weaponShadow.Value.RotationDegrees <- _weaponType.Rotation
        weaponMesh2.Value.RotationDegrees <- _weaponType.Rotation2
        weaponShadow2.Value.RotationDegrees <- _weaponType.Rotation2
        weaponShadow.Value.Visible <- _weaponType.Shadow
        weaponShadow2.Value.Visible <- _weaponType.Shadow

        idleSwayAdjustment <- _weaponType.IdleSwayAdjustment
        idleSwayRotationStrength <- _weaponType.IdleSwayRotationStrength
        randomSwayAmount <- _weaponType.RandomSwayAmount

    let getSwayNoise () =
        let playerPosition =
            if Engine.IsEditorHint() then
                Vector3.Zero
            else
                this.player.GlobalPosition

        this.swayNoise.Noise.GetNoise2D(playerPosition.X, playerPosition.Y)

    let positionRotationChanger randX randY rotStrength delta =
        let mutable position = weaponMesh.Value.Position

        position.X <-
            Mathf.Lerp(
                position.X,
                _weaponType.Position.X
                - (mouseMovement.X * _weaponType.SwayAmountPosition + randX) * delta,
                _weaponType.SwaySpeedPosition
            )

        position.Y <-
            Mathf.Lerp(
                position.Y,
                _weaponType.Position.Y
                + (mouseMovement.Y * _weaponType.SwayAmountPosition + randY) * delta,
                _weaponType.SwaySpeedPosition
            )

        weaponMesh.Value.Position <- position
        let mutable rotDeg = weaponMesh.Value.RotationDegrees

        rotDeg.Y <-
            Mathf.Lerp(
                rotDeg.Y,
                _weaponType.Rotation.Y
                + (mouseMovement.X * _weaponType.SwayAmountRotation + randY * rotStrength) * delta,
                _weaponType.SwaySpeedRotation
            )

        rotDeg.X <-
            Mathf.Lerp(
                rotDeg.X,
                _weaponType.Rotation.X
                - (mouseMovement.Y * _weaponType.SwayAmountRotation + randX * rotStrength) * delta,
                _weaponType.SwaySpeedRotation
            )

        weaponMesh.Value.RotationDegrees <- rotDeg

    let swayWeapon delta isIdle =
        mouseMovement <- mouseMovement.Clamp(_weaponType.SwayMin, _weaponType.SwayMax)

        if isIdle then
            let swayRandom = getSwayNoise ()
            let swayRandomAdjusted = swayRandom * idleSwayAdjustment

            time <- time + delta * (this.swaySpeed + swayRandom)
            randomSwayX <- Mathf.Sin(time * 1.5f + swayRandomAdjusted) / randomSwayAmount
            randomSwayY <- Mathf.Sin(time - swayRandomAdjusted) / randomSwayAmount

            positionRotationChanger randomSwayX randomSwayY idleSwayRotationStrength delta
        else
            positionRotationChanger 0f 0f 0f delta

    member this.weaponType
        with get () = _weaponType
        and set v =
            _weaponType <- v
            // C#/F# 涉及到编辑器工具在编译时就各种乱七八糟的 bug，烦死了
            // 判空也没有用，编译就是会清空属性，但其实加载已保存场景又正常……
            // 参考 issue：https://github.com/godotengine/godot-proposals/issues/9001
            if _weaponType.Mesh <> null && this.IsNodeReady() && Engine.IsEditorHint() then
                loadWeapon ()

    member val swayNoise: NoiseTexture2D = null with get, set
    member val swaySpeed = 1.2f with get, set

    member val weapon1 = Unchecked.defaultof<IWeapons> with get, set
    member val weapon2 = Unchecked.defaultof<IWeapons> with get, set
    member val player: CharacterBody3D = null with get, set

    member this.reset
        with get () = _reset
        and set v =
            _reset <- v

            if this.IsNodeReady() && Engine.IsEditorHint() then
                loadWeapon ()

    override this._Input event =
        if event.IsActionPressed "weapon1" then
            _weaponType <- this.weapon1
            loadWeapon ()

        if event.IsActionPressed "weapon2" then
            _weaponType <- this.weapon2
            loadWeapon ()

        if event :? InputEventMouseMotion then
            mouseMovement <- (event :?> InputEventMouseMotion).Relative

    override this._PhysicsProcess delta =
        if this.swayNoise <> null && this.swayNoise.Noise <> null then // 防止编译过程中报错，.NET 这点很烦
            swayWeapon (float32 delta) true

    override this._Ready() =
        if this.Owner <> null && this.Owner <> this then
            async {
                let! _ =
                    this.ToSignal(this.Owner, Node.SignalName.Ready)
                    |> AwaitUtil.awaiterToTask
                    |> Async.AwaitTask

                GD.Print "await Weapon Owner _Ready"
            }
            |> Async.StartImmediate // 必须本线程才能监听 Ready

        loadWeapon ()
