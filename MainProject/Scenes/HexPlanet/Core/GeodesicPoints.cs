using System.Collections.Generic;
using System.Linq;
using Godot;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Core;

public static class GeodesicPoints
{
    public static List<Vector3> GenPoints(int subdivides, float radius)
    {
        const float x = 0.525731112119133606f;
        const float z = 0.850650808352039932f;

        var vertices = new List<Vector3>(new Vector3[]
        {
            new(-x, 0.0f, z), new(x, 0.0f, z), new(-x, 0.0f, -z), new(x, 0.0f, -z),
            new(0.0f, z, x), new(0.0f, z, -x), new(0.0f, -z, x), new(0.0f, -z, -x),
            new(z, x, 0.0f), new(-z, x, 0.0f), new(z, -x, 0.0f), new(-z, -x, 0.0f),
        });

        // 这里顺序不影响，反正最后只输出 vertices
        var indices = new List<int>
        {
            1, 4, 0,
            4, 9, 0,
            4, 5, 9,
            8, 5, 4,
            1, 8, 4,
            1, 10, 8,
            10, 3, 8,
            8, 3, 5,
            3, 2, 5,
            3, 7, 2,
            3, 10, 7,
            10, 6, 7,
            6, 11, 7,
            6, 0, 11,
            6, 1, 0,
            10, 1, 6,
            11, 0, 9,
            2, 11, 9,
            5, 2, 9,
            11, 2, 7,
        };

        // var sqrt5 = Mathf.Sqrt(5f); // √5
        // var sqrt5divBy1 = Mathf.Sqrt(0.2f); // 1/√5
        //
        // var vertices = new List<Vector3>
        // {
        //     new(0f, 1f, 0f),
        //     new(2f * sqrt5divBy1, sqrt5divBy1, 0f),
        //     new((5f - sqrt5) / 10f, sqrt5divBy1, Mathf.Sqrt((5f + sqrt5) / 10f)),
        //     new((-5f - sqrt5) / 10f, sqrt5divBy1, Mathf.Sqrt((5f - sqrt5) / 10f)),
        //     new((-5f - sqrt5) / 10f, sqrt5divBy1, -Mathf.Sqrt((5f - sqrt5) / 10f)),
        //     new((5f - sqrt5) / 10f, sqrt5divBy1, -Mathf.Sqrt((5f + sqrt5) / 10f)),
        //     new(0f, -1f, 0f),
        //     new(-2f * sqrt5divBy1, -sqrt5divBy1, 0f),
        //     new((-5f + sqrt5) / 10f, -sqrt5divBy1, -Mathf.Sqrt((5f + sqrt5) / 10f)),
        //     new((5f + sqrt5) / 10f, -sqrt5divBy1, -Mathf.Sqrt((5f - sqrt5) / 10f)),
        //     new((5f + sqrt5) / 10f, -sqrt5divBy1, Mathf.Sqrt((5f - sqrt5) / 10f)),
        //     new((-5f + sqrt5) / 10f, -sqrt5divBy1, Mathf.Sqrt((5f + sqrt5) / 10f)),
        // };
        //
        // var indices =
        //     new List<int>
        //     {
        //         0, 1, 2,
        //         0, 2, 3,
        //         0, 3, 4,
        //         0, 4, 5,
        //         0, 5, 1,
        //         1, 9, 10,
        //         1, 10, 2,
        //         2, 10, 11,
        //         2, 11, 3,
        //         3, 11, 7,
        //         3, 7, 4,
        //         4, 7, 8,
        //         4, 8, 5,
        //         5, 8, 9,
        //         5, 9, 1,
        //         6, 8, 7,
        //         6, 9, 8,
        //         6, 10, 9,
        //         6, 11, 10,
        //         6, 7, 11,
        //     };

        // Make sure there is a vertex per index
        var flatVertices = new List<Vector3>();
        var flatIndices = new List<int>();
        for (var i = 0; i < indices.Count; i++)
        {
            flatVertices.Add(vertices[indices[i]]);
            flatIndices.Add(i);
        }

        vertices = flatVertices;
        indices = flatIndices;
        // GD.Print($"vertices.Count: {vertices.Count}, indices.Count: {indices.Count}");

        // Subdivide
        for (var i = 0; i < subdivides; i++)
        {
            SubdivideSphere(ref vertices, ref indices);
            // GD.Print($"i: {i} vertices.Count: {vertices.Count}, indices.Count: {indices.Count}");
        }

        // Scale
        for (var i = 0; i < vertices.Count; i++)
            vertices[i] *= radius;
        // GD.Print($"final vertices.Count: {vertices.Count}, indices.Count: {indices.Count}");
        // 必须自定义去重逻辑，不然 2 重细分开始会破皮现象
        return vertices.Distinct(new Vector3EqualityComparer()).ToList();
    }

    // C# 的比较器居然必须显式定义？不能 Lambda？
    public class Vector3EqualityComparer : IEqualityComparer<Vector3>
    {
        public bool Equals(Vector3 a, Vector3 b)
        {
            return a.IsEqualApprox(b);
        }

        // C# 这个 GetHashCode 的设计怎么这么脱裤子放屁……
        // 如果 GetHashCode 不相等，就直接不判断 Equals 了！
        public int GetHashCode(Vector3 vec) => 1;
    }


    private static void SubdivideSphere(ref List<Vector3> vertices, ref List<int> indices)
    {
        var newIndices = new List<int>();

        var triCount = indices.Count / 3;
        for (var tri = 0; tri < triCount; tri++)
        {
            // Get vertices of triangle we will be subdividing
            var oldVertIndex = (tri * 3);
            var idxA = indices[oldVertIndex + 0];
            var idxB = indices[oldVertIndex + 1];
            var idxC = indices[oldVertIndex + 2];
            var vA = vertices[idxA];
            var vB = vertices[idxB];
            var vC = vertices[idxC];

            // Find new vertices
            var vAB = vA.Lerp(vB, 0.5f).Normalized();
            var vBC = vB.Lerp(vC, 0.5f).Normalized();
            var vAC = vA.Lerp(vC, 0.5f).Normalized();

            // Add new vertices to vertices list
            var newVertIndex = vertices.Count;
            vertices.Add(vAB);
            vertices.Add(vBC);
            vertices.Add(vAC);

            // Add new indices
            newIndices.Add(newVertIndex + 0); // Middle Triangle
            newIndices.Add(newVertIndex + 1);
            newIndices.Add(newVertIndex + 2);

            newIndices.Add(newVertIndex + 2); // A triangle
            newIndices.Add(idxA);
            newIndices.Add(newVertIndex + 0);

            newIndices.Add(newVertIndex + 0); // B triangle
            newIndices.Add(idxB);
            newIndices.Add(newVertIndex + 1);

            newIndices.Add(newVertIndex + 1); // C triangle
            newIndices.Add(idxC);
            newIndices.Add(newVertIndex + 2);
        }

        indices = newIndices;
    }
}