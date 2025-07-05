using Commons.Constants;
using Commons.Utils;
using Commons.Utils.HexSphereGrid;
using Domains.Services.Abstractions.PlanetGenerates;
using Godot;
using Infras.Writers.Abstractions.PlanetGenerates;

namespace Domains.Services.PlanetGenerates;

/// 目前 PointService 的定位是正二十面体的细分工作
/// 
/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-02-24 13:35
public class PointService(IFaceRepo faceRepo, IPointRepo pointRepo) : IPointService
{
    public void InitPointsAndFaces(bool chunky, int divisions)
    {
        var time = Time.GetTicksMsec();
        SubdivideIcosahedron(chunky, divisions);
        InitPointFaceIds(chunky);
        GD.Print($"--- InitPointsAndFaces for {(chunky ? "Chunk" : "Tile")} cost: {Time.GetTicksMsec() - time} ms");
    }

    private void InitPointFaceIds(bool chunky)
    {
        foreach (var face in faceRepo.GetAllByChunky(chunky))
        foreach (var p in face.TriVertices.Select(v => pointRepo.GetByPosition(chunky, v)!))
            p.FaceIds.Add(face.Id);
    }

    // 初始化 Point 和 Face
    private void SubdivideIcosahedron(bool chunky, int divisions)
    {
        var pn = IcosahedronConstants.Vertices[0]; // 北极点
        var ps = IcosahedronConstants.Vertices[6]; // 南极点
        // 轴坐标系（0,0）放在第一组竖列四面的北回归线最东端
        pointRepo.Add(chunky, pn, new SphereAxial(0, -divisions));
        pointRepo.Add(chunky, ps, new SphereAxial(-divisions, 2 * divisions));
        var edges = GenEdgeVectors(divisions, pn, ps);
        for (var col = 0; col < 5; col++)
        {
            InitNorthTriangle(chunky, edges, col, divisions);
            InitEquatorTwoTriangles(chunky, edges, col, divisions);
            InitSouthTriangle(chunky, edges, col, divisions);
        }
    }

    private Vector3[][] GenEdgeVectors(int divisions, Vector3 pn, Vector3 ps)
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
    private void InitNorthTriangle(bool chunky, Vector3[][] edges, int col, int divisions)
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
            pointRepo.Add(chunky, nowLine[0], new SphereAxial(-divisions * col, i == divisions ? 0 : i - divisions));
            for (var j = 0; j < i; j++)
            {
                if (j > 0)
                {
                    faceRepo.Add(chunky, [nowLine[j], preLine[j], preLine[j - 1]]);
                    pointRepo.Add(chunky, nowLine[j],
                        new SphereAxial(-divisions * col - j, i == divisions ? 0 : i - divisions));
                }

                faceRepo.Add(chunky, [preLine[j], nowLine[j], nowLine[j + 1]]);
            }

            preLine = nowLine;
        }
    }

    // 赤道两个面（第二、三面）的构造
    private void InitEquatorTwoTriangles(bool chunky, Vector3[][] edges, int col, int divisions)
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
            pointRepo.Add(chunky, nowLineEast[0], new SphereAxial(-divisions * col, i));
            for (var j = 0; j < i; j++)
            {
                if (j > 0)
                {
                    faceRepo.Add(chunky, [nowLineEast[j], preLineEast[j], preLineEast[j - 1]]);
                    if (i == divisions)
                        pointRepo.Add(chunky, nowLineEast[j], new SphereAxial(-divisions * col - j, i));
                    else
                        pointRepo.Add(chunky, nowLineEast[j], new SphereAxial(-divisions * col - j, i));
                }

                faceRepo.Add(chunky, [preLineEast[j], nowLineEast[j], nowLineEast[j + 1]]);
            }

            // 构造西边面（第二面）
            if (i < divisions)
                pointRepo.Add(chunky, nowLineWest[0], new SphereAxial(-divisions * col - i, i));
            for (var j = 0; j <= divisions - i; j++)
            {
                if (j > 0)
                {
                    faceRepo.Add(chunky, [preLineWest[j], nowLineWest[j - 1], nowLineWest[j]]);
                    if (j < divisions - i)
                        pointRepo.Add(chunky, nowLineWest[j], new SphereAxial(-divisions * col - i - j, i));
                }

                faceRepo.Add(chunky, [nowLineWest[j], preLineWest[j + 1], preLineWest[j]]);
            }

            preLineEast = nowLineEast;
            preLineWest = nowLineWest;
        }
    }

    // 构造南部的最后一面（列的第四面）
    private void InitSouthTriangle(bool chunky, Vector3[][] edges, int col, int divisions)
    {
        var nextCol = (col + 1) % 5;
        var southWest = edges[nextCol * 6 + 5]; // 向南方连接南极的靠西的边界
        var southEast = edges[col * 6 + 5]; // 向南方连接南极的靠东的边界
        var preLine = edges[col * 6 + 4]; // 南回归线的边（E -> W）
        for (var i = 1; i <= divisions; i++)
        {
            var nowLine = Math3dUtil.Subdivide(southEast[i], southWest[i], divisions - i);
            if (i < divisions)
                pointRepo.Add(chunky, nowLine[0], new SphereAxial(-divisions * col - i, divisions + i));
            for (var j = 0; j <= divisions - i; j++)
            {
                if (j > 0)
                {
                    faceRepo.Add(chunky, [preLine[j], nowLine[j - 1], nowLine[j]]);
                    if (j < divisions - i)
                        pointRepo.Add(chunky, nowLine[j], new SphereAxial(-divisions * col - i - j, divisions + i));
                }

                faceRepo.Add(chunky, [nowLine[j], preLine[j + 1], preLine[j]]);
            }

            preLine = nowLine;
        }
    }
}