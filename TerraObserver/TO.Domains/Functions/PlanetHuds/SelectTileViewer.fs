namespace TO.Domains.Functions.PlanetHuds

open System
open Godot
open TO.Domains.Functions.HexSpheres.Components.Tiles
open TO.Domains.Functions.Maths
open TO.Domains.Types.Configs
open TO.Domains.Types.Friflos
open TO.Domains.Types.HexSpheres
open TO.Domains.Types.HexSpheres.Components.Tiles
open TO.Domains.Types.PlanetHuds

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-07-01 09:38:01
module SelectTileViewerCommand =
    let private getTileCollisionResult (viewer: ISelectTileViewer) =
        let spaceState = viewer.GetWorld3D().DirectSpaceState
        let camera = viewer.GetViewport().GetCamera3D()
        let mousePos = viewer.GetViewport().GetMousePosition()
        let originV = camera.ProjectRayOrigin mousePos
        let endV = originV + camera.ProjectRayNormal mousePos * 2000f
        let query = PhysicsRayQueryParameters3D.Create(originV, endV)
        spaceState.IntersectRay query

    let private getTileCollisionPositionUnderCursor viewer =
        let result = getTileCollisionResult viewer

        match result.TryGetValue("position") with
        | true, position -> Some <| position.AsVector3()
        | false, _ -> None

    // vi 对应此前 surfaceTool.AddVertex 的次数
    let private addHexFrame tileUnitCentroid tileUnitCorners color viewRadius (surfaceTool: SurfaceTool) vi =
        surfaceTool.SetColor color
        let centroid = tileUnitCentroid |> TileUnitCentroid.scaled viewRadius

        let points =
            tileUnitCorners
            |> TileUnitCorners.getCorners tileUnitCentroid.UnitCentroid viewRadius
            |> Seq.toArray

        for p in points do
            surfaceTool.AddVertex p
            surfaceTool.AddVertex <| centroid.Lerp(p, 0.85f)

        for i in 0 .. points.Length - 1 do
            let nextIdx = (i + 1) % points.Length

            if Math3dUtil.isRightVSeq Vector3.Zero centroid points[i] points[nextIdx] then
                surfaceTool.AddIndex <| vi + 2 * i + 1
                surfaceTool.AddIndex <| vi + 2 * i
                surfaceTool.AddIndex <| vi + 2 * nextIdx
                surfaceTool.AddIndex <| vi + 2 * nextIdx
                surfaceTool.AddIndex <| vi + 2 * nextIdx + 1
                surfaceTool.AddIndex <| vi + 2 * i + 1
            else
                surfaceTool.AddIndex <| vi + 2 * nextIdx + 1
                surfaceTool.AddIndex <| vi + 2 * nextIdx
                surfaceTool.AddIndex <| vi + 2 * i
                surfaceTool.AddIndex <| vi + 2 * i
                surfaceTool.AddIndex <| vi + 2 * i + 1
                surfaceTool.AddIndex <| vi + 2 * nextIdx + 1

        2 * points.Length

    let private generateMeshForEditMode
        (env:
            'E
                when 'E :> IHexSphereQuery
                and 'E :> IPlanetConfigQuery
                and 'E :> IEntityStoreQuery
                and 'E :> ICatlikeCodingNoiseQuery)
        (viewer: ISelectTileViewer)
        (selectedTileId: int Nullable)
        position
        =
        let hoverTileId =
            match env.SearchNearest position false with
            | Some hoverTileId -> Nullable<int> hoverTileId.Id
            | None -> Nullable()

        if hoverTileId.HasValue || selectedTileId.HasValue then
            if selectedTileId = viewer.SelectedTileId && hoverTileId = viewer.HoverTileId then
                // 编辑选取点和鼠标悬浮点都没变
                None
            else
                // GD.Print($"Generating New _selectTileViewer Mesh! {centerId}, position: {position}");
                viewer.SelectedTileId <- selectedTileId
                viewer.HoverTileId <- hoverTileId
                let surfaceTool = new SurfaceTool()
                surfaceTool.Begin Mesh.PrimitiveType.Triangles
                surfaceTool.SetSmoothGroup UInt32.MaxValue
                let planetConfig = env.PlanetConfig
                let mutable vi = 0

                if selectedTileId.HasValue then
                    let tile = env.GetEntityById selectedTileId.Value
                    let tileUnitCentroid = tile.GetComponent<TileUnitCentroid>()
                    let tileUnitCorners = tile.GetComponent<TileUnitCorners>()
                    let tileValue = tile.GetComponent<TileValue>()

                    vi <-
                        vi
                        + addHexFrame
                            tileUnitCentroid
                            tileUnitCorners
                            Colors.Aquamarine
                            (1.01f * (planetConfig.Radius + env.GetHeight tileValue tileUnitCentroid))
                            surfaceTool
                            vi

                if hoverTileId.HasValue then
                    let hoverTile = env.GetEntityById hoverTileId.Value
                    let hoverTileUnitCentroid = hoverTile.GetComponent<TileUnitCentroid>()
                    let hoverTileUnitCorners = hoverTile.GetComponent<TileUnitCorners>()
                    let mutable color = Colors.DarkGreen
                    color.A <- 0.8f
                    // TODO: 根据笔刷大小，绘制所有周围地块
                    // let tiles = tileRepo.GetTilesInDistance(hoverTile,
                    //     hexPlanetHudRepo.GetTileOverrider().BrushSize);
                    // for tile in tiles do
                    vi <-
                        vi
                        + addHexFrame
                            hoverTileUnitCentroid
                            hoverTileUnitCorners
                            color
                            (planetConfig.Radius + planetConfig.MaxHeight)
                            surfaceTool
                            vi

                Some <| surfaceTool.Commit()
        else
            GD.PrintErr $"centerId not found and no editing Tile! position: {position}"
            None

    let updateInEditMode (env: 'E when 'E :> IPlanetHudQuery and 'E :> ISelectTileViewerQuery) : UpdateInEditMode =
        fun () ->
            env.SelectTileViewerOpt
            |> Option.iter (fun viewer ->
                match getTileCollisionPositionUnderCursor viewer with
                | Some pos when env.PlanetHudOpt.IsSome ->
                    // 更新选择地块框
                    viewer.Show()

                    match generateMeshForEditMode env viewer env.PlanetHudOpt.Value.ChosenTileId pos with
                    | Some mesh -> viewer.Mesh <- mesh
                    | None -> ()
                | _ -> viewer.Hide())
