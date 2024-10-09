using LanguageExt;
using LanguageExt.SomeHelp;

namespace BackEnd4IdleStrategy.Common.Base;

public class Repository<E> : IRepository<E> where E : Entity
{
    private int _nextId = 1;
    private readonly Dictionary<int, E> _idMap = new();

    protected E Init(Func<int, E> factory)
    {
        var entity = factory.Invoke(_nextId++);
        _idMap.Add(entity.Id, entity);
        return entity;
    }

    protected void Remove(int id)
    {
        _idMap.Remove(id);
    }

    public Option<E> GetById(int id)
    {
        return _idMap.TryGetValue(id, out var entity)
            ? entity.ToSome()
            : Option<E>.None;
    }

    public IEnumerable<E> GetAll()
    {
        return _idMap.Values;
    }
}