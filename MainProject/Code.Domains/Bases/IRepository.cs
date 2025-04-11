namespace Domains.Bases;

public interface IRepository<out T> where T : AEntity
{
    T? GetById(int id);
    IEnumerable<T> GetAll();
    int GetCount();
    void Truncate();
    void Delete(int id);
}