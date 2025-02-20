using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace ZeromaXPlayground.demo.GeoGrid;

/**
 * 参考：https://github.com/mocnik-science/geogrid.js/blob/main/src/geogrid.worker.js
 * 基于 MIT 协议使用，版权归原作者 mocnik-science/geogrid.js 项目所有
 * (Copyright (c) 2017-2020 Franz-Benjamin Mocnik)
 * 我（ZeromaXHe）将其修改为 C# 代码，并翻译部分注释为中文。
 */
public class FrontWorker
{
    // 帮助函数
    // const _postMessage = x => postMessage(JSON.stringify(x))
    // const log = (...message) => _postMessage({task: 'log', message: message.join(' ')})
    // const error = (...message) => {throw message.join(' ')}
    // const progress = percent => _postMessage({task: 'progress', percent: percent})
    // const debugStep = (title, percent) => _postMessage({task: 'debugStep', title: title, percent: percent})

    // message handler
    // onmessage = e => {
    //     const d = JSON.parse(e.data)
    //     switch (d.task) {
    //         case 'computeCells':
    //             _postMessage({
    //             task: 'resultComputeCells',
    //             cells: computeGeoJSON(d.json, d.bbox),
    //         })
    //             break
    //         case 'findCell':
    //             _postMessage({
    //             task: d.taskResult,
    //             cell: findCell(d.lat, d.lon),
    //             lat: d.lat,
    //             lon: d.lon,
    //         })
    //             break
    //         case 'findNeighbors':
    //             _postMessage({
    //             task: 'resultFindNeighbors',
    //             uid: d.uid,
    //             neighbors: findNeighbors(d.idLong),
    //         })
    //             break
    //     }
    // }

    private const double CacheSize = 2.5;

    // 缓存
    private readonly Dictionary<string, List<string>> _cacheNeighbors = new();
    private readonly Dictionary<string, List<(float, float)>> _cacheVertices = new();
    private int _resolution;
    private List<Cell> _dataAll;
    private List<Cell> _data;
    private Dictionary<string, Cell> _cells;
    private VpTree<Cell> _tree;

    public Cell CleanUpCell(Cell c)
    {
        if (c == null) return null;
        var cell = new Cell()
        {
            Id = c.Id,
            Lat = c.Lat,
            Lon = c.Lon,
            IsPentagon = c.IsPentagon
        };
        if (c.Filtered) cell.Vertices = c.Vertices;
        return cell;
    }

    public List<Cell> ComputeGeoJSON(MyJson json, BBox bBox)
    {
        // 处理错误
        if (json == null && _dataAll == null) throw new Exception("数据错误 - 没有数据");
        if (json != null && json.Error) throw new Exception($"数据错误 - {json.Message}");
        if (json != null)
        {
            // 保存分辨率
            _resolution = json.Resolution;
            // 解析单元格 ID
            // debugStep('parse cell IDs', 10)
            _dataAll = [];
            foreach (var d in json.Data)
            {
                if (!float.IsNaN(d.Lat)) _dataAll.Add(d);
                else
                {
                    var d2 = new Cell
                    {
                        Id = d.Id
                    };
                    d2.IsPentagon = d2.Id.StartsWith('-');
                    var idWithoutSign = d2.IsPentagon ? d2.Id[1..] : d2.Id;
                    if (idWithoutSign.Length % 2 == 0) idWithoutSign = "0" + idWithoutSign;
                    var numberOfDecimalPlaces = (idWithoutSign.Length - 2 - 5) / 2;
                    d2.Lat = int.Parse(idWithoutSign.Substring(2, numberOfDecimalPlaces + 2)) /
                             Mathf.Pow(10, numberOfDecimalPlaces);
                    d2.Lon = int.Parse(idWithoutSign[(2 + numberOfDecimalPlaces + 2)..]) /
                             Mathf.Pow(10, numberOfDecimalPlaces);
                    var partB = int.Parse(idWithoutSign[..2]);
                    if (partB is (>= 22 and < 44) or >= 66) d2.Lat *= -1;
                    if (partB >= 44) d2.Lon *= -1;
                    _dataAll.Add(d2);
                }
            }
        }

        // 通过重复使数据完整
        // debugStep('make data complete by repetition', 10)
        _data = [];
        if (bBox == null)
            bBox = new BBox
            {
                West = _dataAll.Select(d => d.Lon).Min(),
                East = _dataAll.Select(d => d.Lon).Max(),
                South = _dataAll.Select(d => d.Lat).Min(),
                North = _dataAll.Select(d => d.Lat).Max()
            };
        var minLonN = Mathf.Floor((bBox.West + 180) / 360);
        var maxLonN = Mathf.Ceil((bBox.East - 180) / 360);
        var diameterCell = Mathf.Pow(1 / Math.Sqrt(3), _resolution - 1) * 36 * 2 / 3;
        var west = bBox.West - diameterCell;
        var east = bBox.East + diameterCell;
        var south = bBox.South - diameterCell;
        var north = bBox.North + diameterCell;
        var repeatNumber = Mathf.CeilToInt((bBox.East - bBox.West) / 360);
        for (var i = minLonN; i < maxLonN; i++)
        {
            foreach (var d in _dataAll)
            {
                var lonNew = d.Lon + i * 360;
                if (west <= lonNew && lonNew <= east && south <= d.Lat && d.Lat <= north)
                {
                    var c = new Cell
                    {
                        Id = d.Id,
                        IdLong = $"{d.Id}_{i}",
                        Lat = d.Lat,
                        Lon = lonNew,
                        SinLat = Mathf.Sin(Mathf.DegToRad(d.Lat)),
                        CosLat = Mathf.Cos(Mathf.DegToRad(d.Lat)),
                        LonN = i,
                        IsPentagon = d.IsPentagon,
                        Vertices = _cacheVertices[d.Id]
                    };
                    _data.Add(c);
                }
            }
        }

        // 加载数据到树中
        // debugStep('load data into tree', 15)
        _tree = new VpTree<Cell>();
        _tree.Create(_data.ToArray(),
            (d0, d1) => Mathf.Acos(Mathf.Min(
                d0.SinLat * d1.SinLat + d0.CosLat * d1.CosLat * Mathf.Cos(Mathf.DegToRad(d1.Lon - d0.Lon)), 1)));
        // 找到单元格的邻居
        // debugStep('find neighbours for the cells', 20)
        _cells = new Dictionary<string, Cell>();
        foreach (var d in _data)
        {
            var numberOfNeighborsToLookFor = d.IsPentagon ? 5 : 6;
            var ns = d.IsPentagon ? null : _cacheNeighbors[d.IdLong];
            if (ns != null) d.Neighbors = ns;
            else
            {
                d.Neighbors = [];
                _tree.Search(d, 6 * (repeatNumber + 1) + 1, out var results, out var distances);
                var res = results.Skip(1).ToList();
                for (var i = 0; i < res.Count; i++)
                {
                    var n = _data[i];
                    if (n.Id != d.Id && Mathf.Abs(d.Lon - n.Lon) < 180) d.Neighbors.Add(n.IdLong);
                    if (d.Neighbors.Count >= numberOfNeighborsToLookFor) break;
                }
            }

            _cells[d.IdLong] = d;
        }

        // 过滤单元格 I
        // 通过邻居位置，过滤单元格
        // debugStep('filter cells I', 40)
        foreach (var (id, c) in _cells)
        {
            if (c.Vertices != null) continue;
            var numberOfMatchingNeighbors = 0;
            foreach (var id2 in c.Neighbors)
                if (_cells[id2] != null && _cells[id2].Neighbors.IndexOf(id) >= 0)
                    numberOfMatchingNeighbors++;
            if (numberOfMatchingNeighbors < (c.IsPentagon ? 5 : 6)) c.Filtered = false;
        }

        // 计算角度和顶点
        // debugStep('compute angles and vertices', 45)
        foreach (var (id, c) in _cells)
        {
            if (!c.Filtered || c.Vertices != null) continue;
            c.Angles = [];
            // 计算角度
            foreach (var id2 in c.Neighbors)
            {
                var n = _cells[id2];
                if (n == null) continue;
                var ncLon = Mathf.DegToRad(n.Lon - c.Lon);
                c.Angles.Add(new AngleElem
                {
                    Angle = Mathf.Atan2(Mathf.Sin(ncLon) * n.CosLat,
                        c.CosLat * n.SinLat - c.SinLat * n.CosLat * Mathf.Cos(ncLon)),
                    Lat = n.Lat,
                    Lon = n.Lon
                });
            }

            // 排序角度
            c.Angles.Sort((a, b) => a.Angle < b.Angle ? -1 : 0);
            // 寻找最高纬度
            var iMax = 0;
            var latMax = float.NaN;
            for (var i = 0; i < c.Angles.Count; i++)
            {
                if (float.IsNaN(latMax) || c.Angles[i].Lat > latMax)
                {
                    iMax = i;
                    latMax = c.Angles[i].Lat;
                }
            }

            // 计算顶点
            c.Vertices = [];
            for (var i = 0; i < iMax + c.Angles.Count; i++)
            {
                var n1 = c.Angles[i % c.Angles.Count];
                var n2 = c.Angles[(i + 1) % c.Angles.Count];
                c.Vertices.Add(((n1.Lon + n2.Lon + c.Lon) / 3, (n1.Lat + n2.Lat + c.Lat) / 3));
            }
        }

        // 过滤单元格 II
        // 通过它们的变形，过滤单元格
        // debugStep('filter cells II', 50)
        foreach (var (id, c) in _cells)
        {
            if (!c.Filtered || c.IsPentagon) continue;
            var filter = true;
            for (var i = 0; i < 6; i++)
            {
                var aBefore =
                    Mathf.Abs((c.Angles[i + 2 < 6 ? i + 2 : i - 4].Angle - c.Angles[i].Angle + Mathf.Tau) %
                        Mathf.Tau - Mathf.Pi);
                var a = Mathf.Abs((c.Angles[i + 3 < 6 ? i + 3 : i - 3].Angle - c.Angles[i].Angle + Mathf.Tau) %
                    Mathf.Tau - Mathf.Pi);
                var aAfter =
                    Mathf.Abs((c.Angles[i + 4 < 6 ? i + 4 : i - 2].Angle - c.Angles[i].Angle + Mathf.Tau) %
                        Mathf.Tau - Mathf.Pi);
                if (aBefore < a || aAfter < a)
                {
                    filter = false;
                    break;
                }
            }

            if (!filter) c.Filtered = false;
        }

        // 缓存邻居
        // debugStep('cache neighbours and vertices', 55)
        if (_cacheNeighbors.Count > CacheSize * _cells.Count) _cacheNeighbors.Clear();
        if (_cacheVertices.Count > CacheSize * _cells.Count) _cacheVertices.Clear();
        foreach (var (id, c) in _cells)
        {
            if (c.Filtered)
            {
                _cacheNeighbors[id] = c.Neighbors;
                _cacheVertices[id] = c.Vertices;
            }
        }

        // 清理单元格数据
        // debugStep('clean up data about cells', 60)
        var cells2 = _cells.Values.Select(CleanUpCell).ToList();
        // 发送数据到浏览器
        // debugStep('send data to browser', 62.5)
        return cells2;
    }

    public Cell FindCell(float lat, float lon)
    {
        _tree.Search(
            new Cell
            {
                Lat = lat,
                Lon = lon,
                SinLat = Mathf.Sin(Mathf.DegToRad(lat)),
                CosLat = Mathf.Cos(Mathf.DegToRad(lat))
            }, 1, out var results, out _);
        return results[0];
    }

    public List<Cell> FindNeighbors(string idLong)
    {
        return _cacheNeighbors.TryGetValue(idLong, out var res)
            ? res.Select(idLong2 => CleanUpCell(_cells[idLong2])).ToList()
            : null;
    }
}

public class MyJson
{
    public bool Error { get; set; }
    public string Message { get; set; }
    public int Resolution { get; set; }
    public List<Cell> Data { get; set; }
}

public class BBox
{
    public float West { get; set; }
    public float East { get; set; }
    public float South { get; set; }
    public float North { get; set; }
}

public class Cell
{
    public string Id { get; set; }
    public string IdLong { get; set; }
    public float Lat { get; set; } = float.NaN;
    public float Lon { get; set; } = float.NaN;
    public float SinLat { get; set; } = float.NaN;
    public float CosLat { get; set; } = float.NaN;
    public float LonN { get; set; } = float.NaN;
    public bool IsPentagon { get; set; }
    public bool Filtered { get; set; }
    public List<(float, float)> Vertices { get; set; }
    public List<string> Neighbors { get; set; }
    public List<AngleElem> Angles { get; set; }
}

public class AngleElem
{
    public float Angle { get; set; }
    public float Lat { get; set; } = float.NaN;
    public float Lon { get; set; } = float.NaN;
}