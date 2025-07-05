namespace TO.Domains.Functions.Maps

open Godot
open TO.Domains.Functions.HexGridCoords
open TO.Domains.Functions.HexSpheres.Components
open TO.Domains.Functions.HexSpheres.Components.Tiles
open TO.Domains.Types.Configs
open TO.Domains.Types.Friflos
open TO.Domains.Types.HexSpheres.Components
open TO.Domains.Types.HexSpheres.Components.Tiles
open TO.Domains.Types.Maps

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-07-05 11:41:05
module RealEarthLandGeneratorCommand =
    let createLand
        (env:
            'E
                when 'E :> IHexMapGeneratorQuery
                and 'E :> IPlanetConfigQuery
                and 'E :> IEntityStoreCommand
                and 'E :> ITileQuery)
        (landGen: IRealEarthLandGenerator)
        =
        let water = env.HexMapGenerator.DefaultWaterLevel
        let elevationStep = env.PlanetConfig.ElevationStep
        let mutable landCount = 0
        let worldMap = landGen.WorldMap.GetImage()

        env.ExecuteInCommandBuffer(fun cb ->
            for tile in env.GetAllTiles() do
                let mutable tileValue = tile |> Tile.value

                let lonLat =
                    env.GetSphereAxial tile |> SphereAxial.toLonLat |> LonLatCoords.toVector2

                let percentX = Mathf.Remap(lonLat.X, 180f, -180f, 0f, 1f) // 西经为正，所以这里得反一下
                let percentY = Mathf.Remap(lonLat.Y, 90f, -90f, 0f, 1f) // 北纬为正，所以这里得反一下
                let mutable x = int (4096f * percentX) // 宽度 4096

                if x >= 4096 then
                    x <- 4095 // 不知道为啥 Mathf.Clamp 限制不了…… 手动限制一下

                let mutable y = int (2048f * percentY) // 高度 2048

                if y >= 2048 then
                    y <- 2047 // 不知道为啥 Mathf.Clamp 限制不了…… 手动限制一下

                let mutable elevation = 0
                let color = worldMap.GetPixel(x, y)

                if color.R > 0.9f then
                    // 陆地
                    elevation <- int (color.G * float32 (elevationStep + 1 - water))

                    if elevation = elevationStep + 1 - water then
                        elevation <- elevation - 1

                    elevation <- elevation + water
                    landCount <- landCount + 1
                elif color.G > 0.1f then
                    // 湖区
                    // BUG: 现在的实现可能出现高于相邻地块的水面
                    elevation <- int (color.G * float32 (elevationStep + 1 - water))

                    if elevation = elevationStep + 1 - water then
                        elevation <- elevation - 1

                    elevation <- elevation + water - 1
                    tileValue <- tileValue |> TileValue.withWaterLevel (elevation + 1)
                else
                    // 海洋
                    elevation <- int (color.B * float32 water)

                    if elevation = water then
                        elevation <- elevation - 1

                tileValue <- tileValue |> TileValue.withElevation elevation
                cb.AddComponent<TileValue>(tile.Id, &tileValue))

        landCount
