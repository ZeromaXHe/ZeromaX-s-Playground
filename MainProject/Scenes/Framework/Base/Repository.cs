using System;
using System.Collections.Generic;

namespace ZeromaXsPlaygroundProject.Scenes.Framework.Base;

public abstract class Repository<T> : IRepository<T> where T : AEntity
{
    private int _nextId = 1;
    protected readonly Dictionary<int, T> Repo = new();
    public T GetById(int id) => Repo.GetValueOrDefault(id);
    public IEnumerable<T> GetAll() => Repo.Values;
    public int GetCount() => Repo.Count;
    public void Delete(int id) => Repo.Remove(id);

    // 实现接口的方法就不能是 protected 了，所以不在接口声明
    protected T Add(Func<int, T> factory)
    {
        var entity = factory.Invoke(_nextId++);
        Repo.Add(entity.Id, entity);
        AddHook(entity);
        return entity;
    }

    public void Truncate()
    {
        _nextId = 1;
        Repo.Clear();
        TruncateHook();
    }

    // 钩子函数
    protected abstract void AddHook(T entity);
    protected abstract void TruncateHook();
}