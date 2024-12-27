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
                let unscaledElevation = shapeGenerator.CalculateUnscaledElevation pointOnUnitSphere
                vertices[i] <- pointOnUnitSphere * shapeGenerator.GetScaledElevation unscaledElevation

                if x < resolution - 1 && y < resolution - 1 then
                    // 切记 Unity 的面方向和 Godot 相反，所以需要把每个三角形后两个点顺序互换
                    triangles[triIndex] <- i
                    triangles[triIndex + 1] <- i + resolution
                    triangles[triIndex + 2] <- i + resolution + 1
                    triangles[triIndex + 3] <- i
                    triangles[triIndex + 4] <- i + resolution + 1
                    triangles[triIndex + 5] <- i + 1
                    triIndex <- triIndex + 6

        // ArrayMesh 不能自动生成法线，所以手动计算法线
        // 算法参考：Godot 官方文档《使用 MeshDataTool》
        // https://docs.godotengine.org/zh-cn/4.x/tutorials/3d/procedural_geometry/meshdatatool.html
        // 但是因为本身是一次性生成静态网格，没必要用 MeshDataTool，直接 ArrayMesh 原生数组即可。
        // 这里原理就是把顶点所在的所有面的单位法向量相加，然后再正则化，就是顶点的单位法向量
        for i in 0 .. (triangles.Length / 3 - 1) do
            // 三角面点索引
            let a = triangles[3 * i]
            let b = triangles[3 * i + 1]
            let c = triangles[3 * i + 2]
            // 获取端点
            let ap = vertices[a]
            let bp = vertices[b]
            let cp = vertices[c]
            // 计算面的法线
            let n = (bp - cp).Cross(ap - bp).Normalized()
            normals[a] <- normals[a] + n
            normals[b] <- normals[b] + n
            normals[c] <- normals[c] + n

        for i in 0 .. normals.Length - 1 do
            normals[i] <- normals[i].Normalized()

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
                let unscaledElevation = shapeGenerator.CalculateUnscaledElevation pointOnUnitSphere
                uv.Add <| Vector2(colorGenerator.BiomePercentFromPoint pointOnUnitSphere, unscaledElevation)

        arrayMesh[int Mesh.ArrayType.TexUV] <- uv.ToArray()
        let mesh = new ArrayMesh()
        mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrayMesh)
        meshIns.Mesh <- mesh
