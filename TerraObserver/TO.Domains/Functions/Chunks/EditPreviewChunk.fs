namespace TO.Domains.Functions.Chunks

open System
open TO.Domains.Types.Chunks
open TO.Domains.Types.HexSpheres
open TO.Domains.Types.HexSpheres.Components
open TO.Domains.Types.PlanetHuds

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-07-07 14:19:07
module EditPreviewChunkCommand =
    let private clearOldData (chunk: IEditPreviewChunk) =
        chunk.GetTerrain() |> HexMeshCommand.clear
        chunk.GetRivers() |> HexMeshCommand.clear
        chunk.GetRoads() |> HexMeshCommand.clear
        chunk.GetWater() |> HexMeshCommand.clear
        chunk.GetWaterShore() |> HexMeshCommand.clear
        chunk.GetEstuary() |> HexMeshCommand.clear
        chunk.GetWalls() |> HexMeshCommand.clear

    let private apply (chunk: IEditPreviewChunk) =
        chunk.GetTerrain() |> HexMeshCommand.apply
        chunk.GetRivers() |> HexMeshCommand.apply
        chunk.GetRoads() |> HexMeshCommand.apply
        chunk.GetWater() |> HexMeshCommand.apply
        chunk.GetWaterShore() |> HexMeshCommand.apply
        chunk.GetEstuary() |> HexMeshCommand.apply
        chunk.GetWalls() |> HexMeshCommand.apply

    let refreshEditPreview
        (env:
            'E
                when 'E :> IEditPreviewChunkQuery
                and 'E :> ITileQuery
                and 'E :> IPlanetHudQuery
                and 'E :> IChunkTriangulationCommand
                and 'E :> IFeatureCommand)
        : RefreshEditPreview =
        fun (hoverTileId: TileId Nullable) ->
            match env.EditPreviewChunkOpt with
            | Some editPreviewChunk ->
                if hoverTileId.HasValue then
                    editPreviewChunk.Show()

                    for tileId in editPreviewChunk.EditingTileIds do
                        env.HideFeatures tileId true
                        env.DeleteFeatures tileId true

                    editPreviewChunk.EditingTileIds.Clear()
                    let hoverTile = env.GetTile hoverTileId.Value

                    match env.PlanetHudOpt with
                    | Some planetHud ->
                        // 根据笔刷大小，获取所有周围地块 id，更新
                        let tiles = env.GetTilesInDistance hoverTile planetHud.BrushSize |> Seq.toArray

                        for tile in tiles do
                            editPreviewChunk.EditingTileIds.Add tile.Id |> ignore
                        // 更新陆地材质类型
                        let terrainMaterialIdx =
                            if not planetHud.ApplyTerrain then
                                0
                            else
                                planetHud.ActiveTerrain + 1

                        editPreviewChunk.GetTerrain().MaterialOverride <-
                            editPreviewChunk.TerrainMaterials[terrainMaterialIdx]
                        // 更新网格
                        clearOldData editPreviewChunk

                        for tile in tiles do
                            env.Triangulate editPreviewChunk tile

                        apply editPreviewChunk

                        if editPreviewChunk.Visible then
                            env.ShowFeatures false true tiles
                        else
                            for tile in tiles do
                                env.HideFeatures tile.Id true
                    | None -> ()
                else
                    editPreviewChunk.Hide()
                    editPreviewChunk.EditingTileIds.Clear()
            | None -> ()
