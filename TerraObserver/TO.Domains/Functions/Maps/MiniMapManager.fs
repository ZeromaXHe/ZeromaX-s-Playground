namespace TO.Domains.Functions.Maps

open System
open Friflo.Engine.ECS
open Godot
open TO.Domains.Functions.HexGridCoords
open TO.Domains.Functions.HexSpheres.Components
open TO.Domains.Functions.HexSpheres.Components.Tiles
open TO.Domains.Types.Configs
open TO.Domains.Types.HexGridCoords
open TO.Domains.Types.HexSpheres
open TO.Domains.Types.HexSpheres.Components
open TO.Domains.Types.Maps

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-07-03 14:49:03
module MiniMapManagerCommand =
    let clickOnMiniMap (env: #IMiniMapManagerQuery) : ClickOnMiniMap =
        fun () ->
            match env.MiniMapManagerOpt with
            | None -> ()
            | Some this ->
                let mousePos = this.TerrainLayer.GetLocalMousePosition()
                let mapVec = this.TerrainLayer.LocalToMap mousePos
                let sa = SphereAxial(mapVec.X, mapVec.Y)

                if sa |> SphereAxial.isValid then
                    sa
                    |> SphereAxial.toLonLat
                    |> LonLatCoords.toDirectionVector3
                    |> this.EmitClicked
    // 标准摄像机对应 Divisions = 10
    let private standardCamPos = Vector2(-345f, 75f)
    let private standardCamZoom = Vector2(0.4f, 0.4f)

    let private updateCamera (env: #IPlanetConfigQuery) (camera: Camera2D) =
        let divisions = env.PlanetConfig.Divisions
        camera.Position <- standardCamPos / 10f * float32 divisions
        camera.Zoom <- standardCamZoom * 10f / float32 divisions

    let private edgeAtlas (sphereAxial: SphereAxial) =
        match sphereAxial |> SphereAxial.column with
        | 0 -> Nullable <| Vector2I(0, 3)
        | 1 -> Nullable <| Vector2I(0, 2)
        | 2 -> Nullable <| Vector2I(1, 3)
        | 3 -> Nullable <| Vector2I(1, 2)
        | 4 -> Nullable <| Vector2I(2, 2)
        | _ -> Nullable()

    let private terrainAtlas (tile: Entity) =
        let tileValue = tile |> Tile.value

        if tileValue |> TileValue.isUnderwater then
            if TileValue.waterLevel tileValue - TileValue.elevation tileValue > 1 then
                Nullable <| Vector2I(0, 1)
            else
                Nullable <| Vector2I(1, 1)
        else
            match tileValue |> TileValue.terrainTypeIndex with
            | 0 -> Nullable <| Vector2I(3, 0) // 0 沙漠
            | 1 -> Nullable <| Vector2I(0, 0) // 1 草原
            | 2 -> Nullable <| Vector2I(2, 0) // 2 泥地
            | 3 -> Nullable <| Vector2I(3, 1) // 3 岩石
            | 4 -> Nullable <| Vector2I(2, 1) // 4 雪地
            | _ -> Nullable()

    let syncCameraIconPos
        (env: 'E when 'E :> IHexSphereQuery and 'E :> ITileQuery and 'E :> IMiniMapManagerQuery)
        : SyncCameraIconPos =
        fun (pos: Vector3) ->
            match env.MiniMapManagerOpt with
            | None -> ()
            | Some this ->
                let tileOpt = env.SearchNearest pos false

                match tileOpt with
                | None -> GD.PrintErr $"未找到摄像机对应地块：{pos}"
                | Some tile ->
                    let sa = tile |> env.GetSphereAxial
                    // TODO: 缓动，以及更精确的位置转换
                    this.CameraIcon.GlobalPosition <-
                        sa.Coords
                        |> AxialCoords.toVector2I
                        |> this.TerrainLayer.MapToLocal
                        |> this.TerrainLayer.ToGlobal

    let initMiniMap (env: 'E when 'E :> IMiniMapManagerQuery and 'E :> ITileQuery) : InitMiniMap =
        fun (orbitCamPos: Vector3) ->
            match env.MiniMapManagerOpt with
            | None -> ()
            | Some this ->
                syncCameraIconPos env orbitCamPos
                updateCamera env this.Camera
                this.TerrainLayer.Clear()
                this.ColorLayer.Clear()

                env.GetAllTiles()
                |> Seq.iter (fun tile ->
                    let sphereAxial = env.GetSphereAxial tile
                    this.TerrainLayer.SetCell(sphereAxial.Coords |> AxialCoords.toVector2I, 0, terrainAtlas tile)

                    match sphereAxial.Type with
                    | SphereAxialTypeEnum.PoleVertices
                    | SphereAxialTypeEnum.MidVertices ->
                        this.ColorLayer.SetCell(sphereAxial.Coords |> AxialCoords.toVector2I, 0, Vector2I(2, 3))
                    | SphereAxialTypeEnum.EdgesSpecial when sphereAxial.TypeIdx % 6 = 0 || sphereAxial.TypeIdx % 6 = 5 ->
                        this.ColorLayer.SetCell(sphereAxial.Coords |> AxialCoords.toVector2I, 0, edgeAtlas sphereAxial)
                    | _ -> ())

    let refreshMiniMapTile (env: 'E when 'E :> IMiniMapManagerQuery and 'E :> ITileQuery): RefreshMiniMapTile =
        fun (tile: Entity) ->
            let sa = env.GetSphereAxial tile

            match env.MiniMapManagerOpt with
            | None -> ()
            | Some this -> this.TerrainLayer.SetCell(sa.Coords |> AxialCoords.toVector2I, 0, terrainAtlas tile)
