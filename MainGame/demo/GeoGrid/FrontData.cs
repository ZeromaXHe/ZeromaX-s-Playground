using System.Collections.Generic;

namespace ZeromaXPlayground.demo.GeoGrid;

/**
 * 参考：https://github.com/mocnik-science/geogrid.js/blob/main/src/geogrid.data.js
 * 基于 MIT 协议使用，版权归原作者 mocnik-science/geogrid.js 项目所有
 * (Copyright (c) 2017-2020 Franz-Benjamin Mocnik)
 * 我（ZeromaXHe）将其修改为 C# 代码，并翻译部分注释为中文。
 */
public class FrontData(object _options)
{
    private Dictionary<object, Dictionary<object, object>> _dataById;
    public List<Cell> Cells { get; set; } = [];
    private object _geoJSON;
    private object _geoJSONReduced;

    // 初始化插件
    public Dictionary<object, object> OverwriteColor { get; private set; } = new();
    public Dictionary<object, object> OverwriteSize { get; private set; } = new();
    public Dictionary<object, object> OverwriteContourColor { get; private set; } = new();
    public Dictionary<object, object> OverwriteContourWidth { get; private set; } = new();

    public void ResetGeoJSON()
    {
        _geoJSON = null;
        _geoJSONReduced = null;
    }

    // private float MinDataValue(object sourceN, object key)
    // {
    //     var min = float.MaxValue;
    //     foreach (var v in _dataById[sourceN])
    //     {
    //         if(v.)
    //     }
    // }
}