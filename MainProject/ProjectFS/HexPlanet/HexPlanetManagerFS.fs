namespace ProjectFS.HexPlanet

open Godot
open ProjectFS.HexPlanet.Icosphere

[<AbstractClass>]
type HexPlanetManagerFS() =
    inherit Node3D()

    let mutable surfaceTool = new SurfaceTool()
    let mutable meshIns: MeshInstance3D = null
    let mutable database = Unchecked.defaultof<DataBase>
    let mutable oldRadius = 0f
    let mutable oldSubdivision = 0
    let mutable oldHexSize = 0f
    let mutable lastUpdated = 0f

    abstract Radius: float32 with get, set
    abstract Subdivision: int with get, set
    abstract HexSize: float32 with get, set

    member this.DrawHexPlanetMesh() =
        oldRadius <- this.Radius
        oldSubdivision <- this.Subdivision
        oldHexSize <- this.HexSize
        lastUpdated <- 0f

        database <- initHexasphere this.Radius this.Subdivision this.HexSize

        surfaceTool.Clear()
        surfaceTool.Begin(Mesh.PrimitiveType.Triangles)

        for v in database.MeshDetails.Vertices do
            surfaceTool.AddVertex v

        for idx in database.MeshDetails.Triangles do
            surfaceTool.AddIndex idx

        surfaceTool.GenerateNormals()
        meshIns.Mesh <- surfaceTool.Commit()

    override this._Ready() =
        meshIns <- new MeshInstance3D()
        this.AddChild meshIns
        this.DrawHexPlanetMesh()

    override this._Process(delta) =
        lastUpdated <- lastUpdated + float32 delta

        if
            lastUpdated >= 1f
            && (Mathf.Abs(oldRadius - this.Radius) > 0.001f
                || oldSubdivision <> this.Subdivision
                || Mathf.Abs(oldHexSize - this.HexSize) > 0.001f)
        then
            this.DrawHexPlanetMesh()
