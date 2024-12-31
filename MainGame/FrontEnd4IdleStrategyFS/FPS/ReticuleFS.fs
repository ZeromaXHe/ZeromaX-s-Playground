namespace FrontEnd4IdleStrategyFS.FPS

open Godot

type ReticuleFS() as this =
    inherit CenterContainer()

    let adjustReticuleLines () =
        let vel = this.playerController.GetRealVelocity()
        let origin = Vector3.Zero
        let pos = Vector2.Zero
        let speed = origin.DistanceTo vel
        // 调整准星线条位置
        // Top
        this.reticuleLines[0].Position <-
            this.reticuleLines[0]
                .Position.Lerp(pos + Vector2(0f, -speed * this.reticuleDistance), this.reticuleSpeed)
        // Right
        this.reticuleLines[1].Position <-
            this.reticuleLines[1]
                .Position.Lerp(pos + Vector2(speed * this.reticuleDistance, 0f), this.reticuleSpeed)
        // Bottom
        this.reticuleLines[2].Position <-
            this.reticuleLines[2]
                .Position.Lerp(pos + Vector2(0f, speed * this.reticuleDistance), this.reticuleSpeed)
        // Left
        this.reticuleLines[3].Position <-
            this.reticuleLines[3]
                .Position.Lerp(pos + Vector2(-speed * this.reticuleDistance, 0f), this.reticuleSpeed)


    member val reticuleLines: Line2D array = null with get, set
    member val playerController: CharacterBody3D = null with get, set
    member val reticuleSpeed = 0.25f with get, set
    member val reticuleDistance = 2f with get, set
    member val dotRadius = 1f with get, set
    member val dotColor = Colors.White with get, set

    override this._Draw() =
        this.DrawCircle(Vector2(0f, 0f), this.dotRadius, this.dotColor)

    override this._Ready() = this.QueueRedraw()
    override this._Process _ = adjustReticuleLines ()
