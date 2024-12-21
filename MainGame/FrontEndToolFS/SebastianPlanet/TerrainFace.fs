namespace FrontEndToolFS.SebastianPlanet

open Godot

type TerrainFace(shapeGenerator: ShapeGenerator, meshIns: MeshInstance3D, resolution: int, localUp: Vector3) =
    let surfaceTool = new SurfaceTool()
    let mutable axisA = Vector3(localUp.Y, localUp.Z, localUp.X)
    let mutable axisB = localUp.Cross axisA

    member this.ConstructMesh(colorGenerator: ColorGenerator) =
        surfaceTool.Clear()
        surfaceTool.Begin(Mesh.PrimitiveType.Triangles)

        for y in 0 .. resolution - 1 do
            for x in 0 .. resolution - 1 do
                let i = x + y * resolution
                let percent = Vector2(float32 x, float32 y) / float32 (resolution - 1)

                let pointOnUnitCube =
                    localUp + (percent.X - 0.5f) * 2f * axisA + (percent.Y - 0.5f) * 2f * axisB

                let pointOnUnitSphere = pointOnUnitCube.Normalized()
                this.UpdateUVs colorGenerator pointOnUnitSphere
                surfaceTool.AddVertex <| shapeGenerator.CalculatePointOnPlanet pointOnUnitSphere

                if x < resolution - 1 && y < resolution - 1 then
                    // 切记 Unity 的面方向和 Godot 相反，所以需要把每个三角形后两个点顺序互换
                    surfaceTool.AddIndex i
                    surfaceTool.AddIndex <| i + resolution
                    surfaceTool.AddIndex <| i + resolution + 1
                    surfaceTool.AddIndex i
                    surfaceTool.AddIndex <| i + resolution + 1
                    surfaceTool.AddIndex <| i + 1

        surfaceTool.GenerateNormals()
        let material = new StandardMaterial3D()
        material.AlbedoColor <- Colors.White
        surfaceTool.SetMaterial(material)
        meshIns.Mesh <- surfaceTool.Commit()

    member this.UpdateUVs(colorGenerator: ColorGenerator) pointOnUnitSphere =
        surfaceTool.SetUV <| Vector2(colorGenerator.BiomePercentFromPoint pointOnUnitSphere, 0f)
