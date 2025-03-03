using System;
using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Constant;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Enum;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Repo;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service.Impl;

// 目前 PointService 的定位是正二十面体的细分工作
public class PointService(IFaceService faceService, IPointRepo pointRepo) : IPointService
{
    public void Truncate() => pointRepo.Truncate();

    // 切记 pointRepo 的数据相当于存储 Tile 生成前的准备工作，Tile 生成后即可释放全部 Point
    public void SubdivideIcosahedronForTiles(int divisions)
    {
        var time = Time.GetTicksMsec();
        SubdivideIcosahedron(divisions,
            (v, type, typeIdx) => pointRepo.Add(v, type, typeIdx),
            v => faceService.Add(v));
        InitPointFaceIds();
        GD.Print($"SubdivideIcosahedron cost: {Time.GetTicksMsec() - time} ms");
    }

    private void InitPointFaceIds()
    {
        foreach (var face in faceService.GetAll())
        foreach (var p in face.TriVertices.Select(pointRepo.GetByPosition))
        {
            if (p.FaceIds == null)
                p.FaceIds = [face.Id];
            else p.FaceIds.Add(face.Id);
        }
    }

    // 初始化 Point 和 Face
    public void SubdivideIcosahedron(int divisions, Action<Vector3, TileType, int> addPoint,
        Action<Vector3[]> addFace = null)
    {
        var pn = IcosahedronConstants.Vertices[0]; // 北极点
        var ps = IcosahedronConstants.Vertices[6]; // 南极点
        addPoint(pn, TileType.PoleVertices, 0);
        addPoint(ps, TileType.PoleVertices, 1);
        var edges = GenEdgeVectors(divisions, pn, ps, addPoint);
        for (var col = 0; col < 5; col++)
        {
            InitNorthTriangle(edges, col, divisions, addPoint, addFace);
            InitEquatorTwoTriangles(edges, col, divisions, addPoint, addFace);
            InitSouthTriangle(edges, col, divisions, addPoint, addFace);
        }
    }

    private Vector3[][] GenEdgeVectors(int divisions, Vector3 pn, Vector3 ps, Action<Vector3, TileType, int> addPoint)
    {
        var points = IcosahedronConstants.Vertices;
        var indices = IcosahedronConstants.Indices;
        var edges = new Vector3[30][]; // 30 条边
        // 初始化所有的边上的点位置
        for (var col = 0; col < 5; col++) // 5 个竖列四面组
        {
            // p1 到 p4 映射到平面上是竖列四面组中间的四个点，中间 Z 字型边按顺序连接：p2，p1，p3，p4
            var p1 = points[indices[col * 12 + 1]];
            var p2 = points[indices[col * 12 + 2]];
            var p3 = points[indices[col * 12 + 3]];
            var p4 = points[indices[col * 12 + 7]];
            // 每个竖列四面组有六个属于它的边（右边两个三趾鸡爪形的从上到下的边，列左边界的三条边不属于它）
            edges[col * 6] = Math3dUtil.Subdivide(pn, p1, divisions); // 从左上到右下
            edges[col * 6 + 1] = Math3dUtil.Subdivide(p1, p2, divisions); // 从右往左
            edges[col * 6 + 2] = Math3dUtil.Subdivide(p1, p3, divisions); // 从右上到左下
            edges[col * 6 + 3] = Math3dUtil.Subdivide(p1, p4, divisions); // 从左上到右下
            edges[col * 6 + 4] = Math3dUtil.Subdivide(p4, p3, divisions); // 从右往左
            edges[col * 6 + 5] = Math3dUtil.Subdivide(p4, ps, divisions); // 从右上到左下
        }

        return edges;
    }

    // 构造北部的第一个面
    private static void InitNorthTriangle(Vector3[][] edges, int col, int divisions,
        Action<Vector3, TileType, int> addPoint, Action<Vector3[]> addFace)
    {
        var nextCol = (col + 1) % 5;
        var northEast = edges[col * 6]; // 北极出来的靠东的边界
        var northWest = edges[nextCol * 6]; // 北极出来的靠西的边界
        var tropicOfCancer = edges[col * 6 + 1]; // 北回归线的边（E -> W）
        var preLine = new[] { northEast[0] }; // 初始为北极点
        for (var i = 1; i <= divisions; i++)
        {
            var nowLine = i == divisions
                ? tropicOfCancer
                : Math3dUtil.Subdivide(northEast[i], northWest[i], i);
            if (i == divisions)
                addPoint(nowLine[0], TileType.MidVertices, col * 2);
            else
                addPoint(nowLine[0], TileType.Edges, col * 6);
            for (var j = 0; j < i; j++)
            {
                if (j > 0)
                {
                    addFace?.Invoke([nowLine[j], preLine[j], preLine[j - 1]]);
                    if (i == divisions)
                        addPoint(nowLine[j], TileType.Edges, col * 6 + 1);
                    else
                        addPoint(nowLine[j], TileType.Faces, col * 4);
                }

                addFace?.Invoke([preLine[j], nowLine[j], nowLine[j + 1]]);
            }

            preLine = nowLine;
        }
    }

    // 赤道两个面（第二、三面）的构造
    private static void InitEquatorTwoTriangles(Vector3[][] edges, int col, int divisions,
        Action<Vector3, TileType, int> addPoint, Action<Vector3[]> addFace)
    {
        var nextCol = (col + 1) % 5;
        var equatorWest = edges[nextCol * 6 + 3]; // 向东南方斜跨赤道的靠西的边界
        var equatorMid = edges[col * 6 + 2]; // 向西南方斜跨赤道的中间的边
        var equatorEast = edges[col * 6 + 3]; // 向东南方斜跨赤道的靠东的边界
        var tropicOfCapricorn = edges[col * 6 + 4]; // 南回归线的边（E -> W）
        var preLineWest = edges[col * 6 + 1]; // 北回归线的边（E -> W）
        var preLineEast = new[] { equatorEast[0] };
        for (var i = 1; i <= divisions; i++)
        {
            var nowLineEast = i == divisions
                ? tropicOfCapricorn
                : Math3dUtil.Subdivide(equatorEast[i], equatorMid[i], i);
            var nowLineWest = Math3dUtil.Subdivide(equatorMid[i], equatorWest[i], divisions - i);
            // 构造东边面（第三面）
            if (i == divisions)
                addPoint(nowLineEast[0], TileType.MidVertices, col * 2 + 1);
            else
                addPoint(nowLineEast[0], TileType.Edges, col * 6 + 3);
            for (var j = 0; j < i; j++)
            {
                if (j > 0)
                {
                    addFace?.Invoke([nowLineEast[j], preLineEast[j], preLineEast[j - 1]]);
                    if (i == divisions)
                        addPoint(nowLineEast[j], TileType.Edges, col * 6 + 4);
                    else
                        addPoint(nowLineEast[j], TileType.Faces, col * 4 + 2);
                }

                addFace?.Invoke([preLineEast[j], nowLineEast[j], nowLineEast[j + 1]]);
            }

            // 构造西边面（第二面）
            if (i < divisions)
                addPoint(nowLineWest[0], TileType.Edges, col * 6 + 2);
            for (var j = 0; j <= divisions - i; j++)
            {
                if (j > 0)
                {
                    addFace?.Invoke([preLineWest[j], nowLineWest[j - 1], nowLineWest[j]]);
                    if (j < divisions - i)
                        addPoint(nowLineWest[j], TileType.Faces, col * 4 + 1);
                }

                addFace?.Invoke([nowLineWest[j], preLineWest[j + 1], preLineWest[j]]);
            }

            preLineEast = nowLineEast;
            preLineWest = nowLineWest;
        }
    }

    // 构造南部的最后一面（列的第四面）
    private static void InitSouthTriangle(Vector3[][] edges, int col, int divisions,
        Action<Vector3, TileType, int> addPoint, Action<Vector3[]> addFace)
    {
        var nextCol = (col + 1) % 5;
        var southWest = edges[nextCol * 6 + 5]; // 向南方连接南极的靠西的边界
        var southEast = edges[col * 6 + 5]; // 向南方连接南极的靠东的边界
        var preLine = edges[col * 6 + 4]; // 南回归线的边（E -> W）
        for (var i = 1; i <= divisions; i++)
        {
            var nowLine = Math3dUtil.Subdivide(southEast[i], southWest[i], divisions - i);
            if (i < divisions)
                addPoint(nowLine[0], TileType.Edges, col * 6 + 5);
            for (var j = 0; j <= divisions - i; j++)
            {
                if (j > 0)
                {
                    addFace?.Invoke([preLine[j], nowLine[j - 1], nowLine[j]]);
                    if (j < divisions - i)
                        addPoint(nowLine[j], TileType.Faces, col * 4 + 3);
                }

                addFace?.Invoke([nowLine[j], preLine[j + 1], preLine[j]]);
            }

            preLine = nowLine;
        }
    }
}