using System;
using System.Collections.Generic;
using Godot;

namespace ZeromaXPlayground.game.inGame.map.scripts.utils.mySimSql;

public class Index<T, TF> where T : BaseDataObj
{
    public enum Type
    {
        Unique, // 唯一
        Normal, // 普通
    }

    public string Name { get; }
    
    private readonly Dictionary<TF, List<T>> _dict = new();
    private readonly Func<T, TF> _fieldGetter;
    private readonly Type _type;

    public Index(string name, Type type, Func<T, TF> fieldGetter)
    {
        Name = name;
        _type = type;
        _fieldGetter = fieldGetter;
    }

    public void Add(T elem)
    {
        var col = _fieldGetter.Invoke(elem);
        switch (_type)
        {
            case Type.Normal:
                if (_dict.ContainsKey(col))
                {
                    _dict[col] = new List<T>();
                }

                break;
            case Type.Unique:
                if (_dict.ContainsKey(col))
                {
                    GD.PrintErr($"MySimSQL | unique index: {Name} found a collision {col}");
                    return;
                }

                _dict[col] = new List<T>();
                break;
        }

        _dict[col].Add(elem);
    }

    public void Remove(T elem)
    {
        var col = _fieldGetter.Invoke(elem);
        switch (_type)
        {
            case Type.Normal:
                if (_dict.ContainsKey(col))
                {
                    _dict[col].Remove(elem);
                    if (_dict[col].Count == 0)
                    {
                        _dict.Remove(col);
                    }
                }

                break;
            case Type.Unique:
                if (_dict.ContainsKey(col))
                {
                    _dict.Remove(col);
                }

                break;
        }
    }

    public void Clear()
    {
        _dict.Clear();
    }

    public List<T> Get(TF col)
    {
        if (_dict.ContainsKey(col))
        {
            return _dict[col];
        }
        else
        {
            return new List<T>();
        }
    }
}