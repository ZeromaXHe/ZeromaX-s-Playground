namespace FrontEndToolFS.SebatianPlanet

open Godot

type PlanetFS() as this =
    inherit Node3D()
    let mutable meshInstances: MeshInstance3D array = null
    let mutable terrainFaces: TerrainFace array = null
    let mutable shapeGenerator = Unchecked.defaultof<ShapeGenerator>

    let directions =
        [| Vector3.Up
           Vector3.Down
           Vector3.Left
           Vector3.Right
           Vector3.Forward
           Vector3.Back |]

    let initialize () =
        shapeGenerator <- ShapeGenerator(this.shapeSettings)

        if meshInstances = null || meshInstances.Length = 0 then
            meshInstances <- Array.zeroCreate 6

        terrainFaces <- Array.zeroCreate 6

        for i in 0..5 do
            if meshInstances[i] = null then
                let meshObj = new MeshInstance3D()
                meshObj.Name <- $"Mesh{i}"
                this.AddChild meshObj
                meshInstances[i] <- meshObj

            terrainFaces[i] <- TerrainFace(shapeGenerator, meshInstances[i], this.resolution, directions[i])

    let generateMesh () =
        terrainFaces |> Array.iter _.ConstructMesh()

    let generateColors () =
        meshInstances
        |> Array.iter (fun m ->
            let material = new StandardMaterial3D()
            material.AlbedoColor <- this.colorSettings.planetColor
            m.MaterialOverride <- material)

    member val resolution = 10 with get, set
    member val autoUpdate = true with get, set
    member val generate = true with get, set
    member val shapeSettings = ShapeSettings() with get, set
    member val colorSettings: ColorSettings = ColorSettings() with get, set

    member this.OnShapeSettingsUpdated() =
        if this.autoUpdate then
            initialize ()
            generateMesh ()

    member this.OnColorSettingsUpdated() =
        if this.autoUpdate then
            initialize ()
            generateColors ()

    member this.GeneratePlanet() =
        initialize ()
        generateMesh ()
        generateColors ()
        this.generate <- true

    override this._Ready() = this.GeneratePlanet()
