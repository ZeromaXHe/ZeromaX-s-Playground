namespace ProjectFS.HexPlanet

open Godot

type IcosahedronVisualFS() =
    inherit Node3D()

    override this._Ready() =
        let surfaceTool = new SurfaceTool()
        surfaceTool.Begin(Mesh.PrimitiveType.Triangles)
        let mutable vi = 0

        for v in Icosahedron.vertices do
            surfaceTool.AddVertex v

            let meshIns = new MeshInstance3D()
            let textMaterial = new StandardMaterial3D()

            textMaterial.AlbedoColor <-
                match vi with
                | 0 -> Colors.Red
                | 1 -> Colors.Green
                | 2 -> Colors.Blue
                | 3 -> Colors.Yellow
                | 4 -> Colors.Purple
                | 5 -> Colors.Cyan
                | 6 -> Colors.DarkGreen
                | 7 -> Colors.Brown
                | 8 -> Colors.Pink
                | 9 -> Colors.DarkGray
                | 10 -> Colors.YellowGreen
                | _ -> Colors.Magenta

            let mesh = new TextMesh()
            mesh.Text <- vi.ToString()
            mesh.Material <- textMaterial
            meshIns.Mesh <- mesh
            meshIns.Position <- v * 1.1f
            this.AddChild meshIns

            if Mathf.Abs(v.X) > 0.0001f || Mathf.Abs(v.Z) > 0.0001f then
                meshIns.LookAt(-meshIns.Position, Vector3.Up)
            else
                meshIns.LookAt(-meshIns.Position, Vector3.Forward * float32 (sign v.Y))

            vi <- vi + 1

        for i in Icosahedron.indices do
            surfaceTool.AddIndex i

        let material = new StandardMaterial3D()
        material.AlbedoColor <- Colors.White
        surfaceTool.SetMaterial material
        surfaceTool.GenerateNormals()

        let meshIns = new MeshInstance3D()
        meshIns.Mesh <- surfaceTool.Commit()
        this.AddChild meshIns
