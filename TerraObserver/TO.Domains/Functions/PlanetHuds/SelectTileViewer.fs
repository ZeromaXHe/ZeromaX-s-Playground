namespace TO.Domains.Functions.PlanetHuds

open System
open Godot
open TO.Domains.Functions.HexSpheres.Components
open TO.Domains.Functions.HexSpheres.Components.Tiles
open TO.Domains.Functions.Maths
open TO.Domains.Types.Chunks
open TO.Domains.Types.Configs
open TO.Domains.Types.HexSpheres
open TO.Domains.Types.HexSpheres.Components
open TO.Domains.Types.PlanetHuds

module SelectTileViewerQuery =
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

    let getTileIdUnderCursor
        (env: 'E when 'E :> ISelectTileViewerQuery and 'E :> IHexSphereQuery)
        : GetTileIdUnderCursor =
        fun () ->
            match
                env.SelectTileViewerOpt
                |> Option.bind getTileCollisionPositionUnderCursor
                |> Option.bind (fun pos -> env.SearchNearest pos false)
            with
            | None -> Nullable()
            | Some tile -> Nullable tile.Id

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-07-01 09:38:01
module SelectTileViewerCommand =
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
                and 'E :> ITileQuery
                and 'E :> ICatlikeCodingNoiseQuery
                and 'E :> IPlanetHudQuery)
        (viewer: ISelectTileViewer)
        (selectedTileId: int Nullable)
        (hoverTileId: int Nullable)
        =
        // GD.Print($"Generating New _selectTileViewer Mesh! {centerId}, position: {position}");
        viewer.SelectedTileId <- selectedTileId
        viewer.HoverTileId <- hoverTileId
        let surfaceTool = new SurfaceTool()
        surfaceTool.Begin Mesh.PrimitiveType.Triangles
        surfaceTool.SetSmoothGroup UInt32.MaxValue
        let planetConfig = env.PlanetConfig
        let mutable vi = 0

        if selectedTileId.HasValue then
            let tile = env.GetTile selectedTileId.Value

            vi <-
                vi
                + addHexFrame
                    (Tile.unitCentroid tile)
                    (Tile.unitCorners tile)
                    Colors.Aquamarine
                    (1.01f * (planetConfig.Radius + env.GetHeight tile))
                    surfaceTool
                    vi

        if hoverTileId.HasValue then
            let hoverTile = env.GetTile hoverTileId.Value
            let mutable color = Colors.DarkGreen
            color.A <- 0.8f
            // 根据笔刷大小，绘制所有周围地块
            match env.PlanetHudOpt with
            | Some planetHud ->
                let tiles = env.GetTilesInDistance hoverTile planetHud.BrushSize

                for tile in tiles do
                    vi <-
                        vi
                        + addHexFrame
                            (Tile.unitCentroid tile)
                            (Tile.unitCorners tile)
                            color
                            (planetConfig.Radius + planetConfig.MaxHeight)
                            surfaceTool
                            vi
            | None -> ()

        surfaceTool.Commit()

    let updateInEditMode
        (env:
            'E
                when 'E :> IPlanetHudQuery
                and 'E :> ISelectTileViewerQuery
                and 'E :> IEditPreviewChunkQuery
                and 'E :> IEditPreviewChunkCommand)
        : UpdateInEditMode =
        fun () ->
            env.SelectTileViewerOpt
            |> Option.iter (fun viewer ->
                let hoverTileId = env.GetTileIdUnderCursor()

                let selectedTileId =
                    match env.PlanetHudOpt with
                    | Some planetHud -> planetHud.ChosenTileId
                    | None -> Nullable()

                if hoverTileId.HasValue || selectedTileId.HasValue then
                    // 更新选择地块框
                    viewer.Show()
                    let selectedChanged = selectedTileId <> viewer.SelectedTileId
                    let hoverChanged = hoverTileId <> viewer.HoverTileId
                    if selectedChanged || hoverChanged then
                        // 编辑选取点或鼠标悬浮点变了
                        viewer.Mesh <- generateMeshForEditMode env viewer selectedTileId hoverTileId

                    if hoverChanged then
                        // 鼠标悬浮点变了
                        env.RefreshEditPreview hoverTileId
                else
                    viewer.Hide()

                    match env.EditPreviewChunkOpt with
                    | Some editPreviewChunk -> editPreviewChunk.Show()
                    | None -> ())
