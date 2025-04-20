using Domains.Models.Bases;
using Infras.Writers.Abstractions.Bases;

namespace Infras.Writers.Base;

public class Repository<T> : IRepository<T> where T : Entity
{
    private int _nextId = 1;
    private readonly Dictionary<int, T> _repo = new();
    public T? GetById(int id) => _repo.GetValueOrDefault(id);
    public IEnumerable<T> GetAll() => _repo.Values;
    public int GetCount() => _repo.Count;

    public void Delete(int id)
    {
        if (!_repo.TryGetValue(id, out var entity)) return;
        DeleteHook(entity);
        _repo.Remove(id);
    }

    // 实现接口的方法就不能是 protected 了，所以不在接口声明
    protected T Add(Func<int, T> factory)
    {
        var entity = factory.Invoke(_nextId++);
        _repo.Add(entity.Id, entity);
        AddHook(entity);
        return entity;
    }

    public void Truncate()
    {
        _nextId = 1;
        _repo.Clear();
        TruncateHook();
    }

    #region 钩子函数

    protected virtual void AddHook(T entity)
    {
    }

    protected virtual void DeleteHook(T entity)
    {
    }

    protected virtual void TruncateHook()
    {
    }

    #endregion
}