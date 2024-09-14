using System;
using System.Collections.Generic;

namespace ZeromaXPlayground.game.inGame.map.scripts.utils.mySimSql;

public class QueryWrapper
{
    public List<Where> WhereList { get; } = new();

    public List<Order> OrderList { get; } = new();

    public QueryWrapper Eq(string field, Object val)
    {
        WhereList.Add(new Where(field, "=", val));
        return this;
    }

    public QueryWrapper OrderByAsc(string field)
    {
        OrderList.Add(new Order(field, false));
        return this;
    }

    public QueryWrapper OrderByDesc(string field)
    {
        OrderList.Add(new Order(field, true));
        return this;
    }
}

public class Where
{
    /**
     * 字段名
     */
    public string Field { get; }

    /**
     * 操作符
     */
    private string _op;

    /**
     * 条件入参
     */
    public Object Val { get; }

    public Where(string field, string op, Object val)
    {
        Field = field;
        _op = op;
        Val = val;
    }
}

public class Order
{
    /**
     * 字段名
     */
    private string _field;

    /**
     * 是否降序
     */
    private bool _desc;

    public Order(string field, bool desc)
    {
        _field = field;
        _desc = desc;
    }
}