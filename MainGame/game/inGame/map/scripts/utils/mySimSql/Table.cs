using System;
using System.Collections.Generic;
using System.Linq;

namespace ZeromaXPlayground.game.inGame.map.scripts.utils.mySimSql;

public class Table<T> where T : BaseDataObj
{
    private IdGenerator _idGenerator = new();
    private Dictionary<int, T> _idIndex = new();
    private Dictionary<string, Index<T, Object>> _indexes = new();

    public void CreateIndex(Index<T, Object> index)
    {
        _indexes[index.Name] = index;
    }

    public void Insert(T d)
    {
        d.Id = _idGenerator.Next();
        _idIndex[d.Id] = d;
        foreach (var index in _indexes.Values)
        {
            index.Add(d);
        }
    }

    public void DeleteById(int id)
    {
        if (!_idIndex.ContainsKey(id))
        {
            return;
        }

        var d = _idIndex[id];
        _idIndex.Remove(id);
        foreach (var index in _indexes.Values)
        {
            index.Remove(d);
        }
    }

    public void Truncate()
    {
        _idGenerator.Reset();
        _idIndex.Clear();
        foreach (var index in _indexes.Values)
        {
            index.Clear();
        }
    }

    public T QueryById(int id)
    {
        return _idIndex[id];
    }

    public List<T> QueryByIndexField(string field, Object val)
    {
        if (!_indexes.ContainsKey(field))
        {
            return null;
        }
        return _indexes[field].Get(val);
    }

    public List<T> QueryAll()
    {
        return _idIndex.Values.ToList();
    }

    public void UpdateById(T d)
    {
        // TODO: 目前是全量重写，没办法做到细分到字段的更新
        var pre = QueryById(d.Id);
        foreach (var index in _indexes.Values)
        {
            index.Remove(pre);
        }

        _idIndex[d.Id] = d;
        foreach (var index in _indexes.Values)
        {
            index.Add(d);
        }
    }

    public void UpdateFieldById(int id, string field, Action<T> setter)
    {
        var d = QueryById(id);
        if (_indexes.ContainsKey(field))
        {
            _indexes[field].Remove(d);
        }
        setter.Invoke(d);
        if (_indexes.ContainsKey(field))
        {
            _indexes[field].Add(d);
        }
    }

    /// TODO: 未实现。需要 C# 反射，暂时先不管，要用的话自己 QueryAll 写逻辑
    // public List<T> SelectList(QueryWrapper qw = null)
    // {
    //     if (qw == null)
    //     {
    //         return QueryAll();
    //     }
    //
    //     List<T> result = null;
    //     if (qw.WhereList.Count == 0)
    //     {
    //         result = QueryAll();
    //     }
    //     else
    //     {
    //         // 目前 where 里只有 eq 条件（=）
    //         foreach (var w in qw.WhereList)
    //         {
    //             if (_indexes.ContainsKey(w.Field))
    //             {
    //                 result = _indexes[w.Field].Get(w.Val);
    //                 break;
    //             }
    //         }
    //
    //         if (result == null)
    //         {
    //             result = QueryAll();
    //         }
    //         // TODO: 效率可能会很低，所以目前需要注意把可以快速缩小范围的索引字段的 eq 放前面
    //         foreach (var w in qw.WhereList)
    //         {
    //             result = result.Select(e => )
    //         }
    //     }
    // }
}
