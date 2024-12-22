namespace FrontEndToolFS.SebastianPlanet

open Godot

type FaceRenderMask =
    | None = 0
    | Top = 1
    | Bottom = 2
    | Left = 4
    | Right = 8
    | Front = 16
    | Back = 32
    | All = 63

type PlanetFS() as this =
    inherit Node3D()
    let mutable _readyFin = false
    let mutable meshInstances: MeshInstance3D array = null
    let mutable terrainFaces: TerrainFace array = null
    let mutable shapeGenerator = Unchecked.defaultof<ShapeGenerator>
    let mutable colorGenerator = Unchecked.defaultof<ColorGenerator>

    let directions =
        [| Vector3.Up
           Vector3.Down
           Vector3.Left
           Vector3.Right
           Vector3.Forward
           Vector3.Back |]

    let initialize () =
        shapeGenerator <- ShapeGenerator(this.shapeSettings)
        shapeGenerator.UpdateSettings(this.shapeSettings)
        colorGenerator <- ColorGenerator(this.colorSettings)
        colorGenerator.UpdateSettings(this.colorSettings)

        if meshInstances = null || meshInstances.Length = 0 then
            meshInstances <- Array.zeroCreate 6

        terrainFaces <- Array.zeroCreate 6

        for i in 0..5 do
            if meshInstances[i] = null then
                let meshObj = new MeshInstance3D()
                meshObj.Name <- $"Mesh{i}"
                this.AddChild meshObj
                meshInstances[i] <- meshObj

            meshInstances[i].MaterialOverride <- this.colorSettings.planetMaterial
            terrainFaces[i] <- TerrainFace(shapeGenerator, meshInstances[i], this.resolution, directions[i])

            let renderFace =
                (this.faceRenderMask = FaceRenderMask.All)
                || (int this.faceRenderMask = (1 <<< i))

            meshInstances[i].Visible <- renderFace

            GD.Print
                $"Face {i} render mask: {this.faceRenderMask}, 1 <<< i: {1 <<< i}, visible: {meshInstances[i].Visible} = {renderFace}"

    let generateMesh () =
        for i in 0..5 do
            if meshInstances[i].Visible then
                terrainFaces[i].ConstructMesh colorGenerator

        colorGenerator.UpdateElevation shapeGenerator.elevationMinMax

    let generateColors changeUV =
        colorGenerator.UpdateColors()
        // Godot 生成法线只能用 SurfaceTool，而 SurfaceTool 又不能单独改 UV…… 尴尬
        // 只能这样重新生成 Mesh 来刷新 UV 了
        if changeUV then
            for i in 0..5 do
                if meshInstances[i].Visible then
                    terrainFaces[i].ConstructMesh colorGenerator

    member val resolution = 10 with get, set
    member val autoUpdate = false with get, set
    member val generate = true with get, set
    member val faceRenderMask = FaceRenderMask.None with get, set
    member val shapeSettings = ShapeSettings() with get, set
    member val colorSettings: ColorSettings = ColorSettings() with get, set

    member this.OnShapeSettingsUpdated() =
        // 必须加 _readyFin 控制，不然加载已保存场景的时候也会调用进来，怪不得这么卡……
        if _readyFin && this.autoUpdate then
            initialize ()
            generateMesh ()

    member this.OnColorSettingsUpdated() =
        // 必须加 _readyFin 控制，不然加载已保存场景的时候也会调用进来，怪不得这么卡……
        if _readyFin && this.autoUpdate then
            initialize ()
            generateColors true

    member this.GeneratePlanet() =
        initialize ()
        generateMesh ()
        generateColors false
        this.generate <- true

    member this.RefreshNoiseLayers() =
        this.shapeSettings.noiseLayers <- Array.init this.shapeSettings.layerCount (fun _ -> NoiseLayer())

    override this._Ready() =
        // 注意这里加载已保存场景时，每个 member val 貌似都会先按初始值执行一遍，再按 Export 值执行一遍
        this.GeneratePlanet()
        // 让初始化准备好前，加载各个 Export 值时不要重复调用生成 Mesh
        _readyFin <- true
