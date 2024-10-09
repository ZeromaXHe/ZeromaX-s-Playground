using LanguageExt;

namespace BackEnd4IdleStrategy.Common.Base;

public interface IRepository<E> where E : Entity
{
    Option<E> GetById(int id);
    IEnumerable<E> GetAll();
}