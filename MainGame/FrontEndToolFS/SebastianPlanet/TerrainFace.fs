namespace FrontEndToolFS.SebastianPlanet

open System.Collections.Generic
open Godot
open Godot.Collections

type TerrainFace(shapeGenerator: ShapeGenerator, meshIns: MeshInstance3D, resolution: int, localUp: Vector3) =
    let arrayMesh = new Array()
    do arrayMesh.Resize(int Mesh.ArrayType.Max) |> ignore
    let mutable axisA = Vector3(localUp.Y, localUp.Z, localUp.X)
    let mutable axisB = localUp.Cross axisA

    member this.ConstructMesh() =
        let vertices: Vector3 array = Array.zeroCreate <| resolution * resolution
        let normals: Vector3 array = Array.zeroCreate <| resolution * resolution

        let triangles: int array =
            Array.zeroCreate <| (resolution - 1) * (resolution - 1) * 6

        let mutable triIndex = 0
        let uv = arrayMesh[int Mesh.ArrayType.TexUV]

        for y in 0 .. resolution - 1 do
            for x in 0 .. resolution - 1 do
                let i = x + y * resolution
                let percent = Vector2(float32 x, float32 y) / float32 (resolution - 1)

                let pointOnUnitCube =
                    localUp + (percent.X - 0.5f) * 2f * axisA + (percent.Y - 0.5f) * 2f * axisB

                let pointOnUnitSphere = pointOnUnitCube.Normalized()
                vertices[i] <- shapeGenerator.CalculatePointOnPlanet pointOnUnitSphere
                normals[i] <- vertices[i].Normalized()

                if x < resolution - 1 && y < resolution - 1 then
                    // 切记 Unity 的面方向和 Godot 相反，所以需要把每个三角形后两个点顺序互换
                    triangles[triIndex] <- i
                    triangles[triIndex + 1] <- i + resolution
                    triangles[triIndex + 2] <- i + resolution + 1
                    triangles[triIndex + 3] <- i
                    triangles[triIndex + 4] <- i + resolution + 1
                    triangles[triIndex + 5] <- i + 1
                    triIndex <- triIndex + 6

        arrayMesh.Clear()
        arrayMesh.Resize(int Mesh.ArrayType.Max) |> ignore
        arrayMesh[int Mesh.ArrayType.Vertex] <- vertices
        arrayMesh[int Mesh.ArrayType.Index] <- triangles
        arrayMesh[int Mesh.ArrayType.TexUV] <- uv
        arrayMesh[int Mesh.ArrayType.Normal] <- normals
        let mesh = new ArrayMesh()
        mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrayMesh)
        meshIns.Mesh <- mesh

    member this.UpdateUVs(colorGenerator: ColorGenerator) =
        let uv = List<Vector2>()

        for y in 0 .. resolution - 1 do
            for x in 0 .. resolution - 1 do
                let i = x + y * resolution
                let percent = Vector2(float32 x, float32 y) / float32 (resolution - 1)

                let pointOnUnitCube =
                    localUp + (percent.X - 0.5f) * 2f * axisA + (percent.Y - 0.5f) * 2f * axisB

                let pointOnUnitSphere = pointOnUnitCube.Normalized()
                uv.Add <| Vector2(colorGenerator.BiomePercentFromPoint pointOnUnitSphere, 0f)

        arrayMesh[int Mesh.ArrayType.TexUV] <- uv.ToArray()
        let mesh = new ArrayMesh()
        mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrayMesh)
        mesh.RegenNormalMaps() // BUG: 并不是 SurfaceTool 的生成法线，而是重新生成法线贴图？
        meshIns.Mesh <- mesh
