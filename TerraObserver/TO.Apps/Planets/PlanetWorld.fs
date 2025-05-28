namespace TO.Apps.Planets

open System
open Godot
open TO.Apps.Planets.Services
open TO.Infras.Abstractions.Planets.Models.Tiles
open TO.Nodes.Abstractions.Planets.Models
open TO.Nodes.Abstractions.Planets.Views

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-15 14:33:15
type PlanetWorld(hexSphereService: HexSphereService) =
    let addFaceIndex (surfaceTool: SurfaceTool) i0 i1 i2 =
        surfaceTool.AddIndex i0
        surfaceTool.AddIndex i1
        surfaceTool.AddIndex i2

    let generateMesh (planet: IPlanet) (hexSphereConfigs: IHexSphereConfigs) (tiles: TileComponent seq) =
        for child in planet.GetChildren() do
            child.QueueFree()

        let meshIns = new MeshInstance3D()
        let surfaceTool = new SurfaceTool()
        surfaceTool.Begin Mesh.PrimitiveType.Triangles
        surfaceTool.SetSmoothGroup UInt32.MaxValue
        let mutable vi = 0

        for tile in tiles do
            surfaceTool.SetColor <| Color.FromHsv(GD.Randf(), GD.Randf(), GD.Randf())
            let points = tile.GetCorners hexSphereConfigs.Radius

            for point in points do
                surfaceTool.AddVertex point

            addFaceIndex surfaceTool vi <| vi + 1 <| vi + 2
            addFaceIndex surfaceTool vi <| vi + 2 <| vi + 3
            addFaceIndex surfaceTool vi <| vi + 3 <| vi + 4

            if points.Length > 5 then
                addFaceIndex surfaceTool vi <| vi + 4 <| vi + 5

            vi <- vi + points.Length

        surfaceTool.GenerateNormals()
        let material = new StandardMaterial3D()
        material.VertexColorUseAsAlbedo <- true
        surfaceTool.SetMaterial material
        meshIns.Mesh <- surfaceTool.Commit()
        planet.AddChild meshIns

    member this.DrawHexSphereMesh(planet: IPlanet, hexSphereConfigs: IHexSphereConfigs) =
        let time = Time.GetTicksMsec()

        GD.Print
            $"[===DrawHexSphereMesh===] radius {hexSphereConfigs.Radius},
        divisions {hexSphereConfigs.Divisions}, start at: {time}"

        hexSphereService.ClearOldData()
        let tiles = hexSphereService.InitHexSphere hexSphereConfigs
        generateMesh planet hexSphereConfigs tiles
