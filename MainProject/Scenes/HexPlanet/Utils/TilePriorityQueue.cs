using System.Collections.Generic;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Structs;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Utils;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-03 09:14
public class TilePriorityQueue(TileSearchData[] data)
{
    private readonly List<int> _list = [];
    private int _minimum = int.MaxValue;

    public void Enqueue(int tileId)
    {
        var priority = data[tileId].SearchPriority;
        if (priority < _minimum)
            _minimum = priority;
        while (priority >= _list.Count)
            _list.Add(-1);
        data[tileId].NextWithSamePriority = _list[priority];
        _list[priority] = tileId;
    }
    
    public bool TryDequeue(out int tileId)
    {
        for (; _minimum < _list.Count; _minimum++)
        {
            tileId = _list[_minimum];
            if (tileId >= 0)
            {
                _list[_minimum] = data[tileId].NextWithSamePriority;
                return true;
            }
        }
        tileId = -1;
        return false;
    }

    public void Change(int tileId, int oldPriority)
    {
        var current = _list[oldPriority];
        var next = data[current].NextWithSamePriority;
        if (current == tileId)
            _list[oldPriority] = next;
        else
        {
            while (next != tileId)
            {
                current = next;
                next = data[current].NextWithSamePriority;
            }
            data[current].NextWithSamePriority = data[tileId].NextWithSamePriority;
        }
        Enqueue(tileId);
    }
    
    public void Clear()
    {
        _list.Clear();
        _minimum = int.MaxValue;
    }
}