using System;
using System.Collections.Generic;

namespace ZeromaXsPlaygroundProject.Scenes.Framework.Base;

public interface IRepository<out T> where T : AEntity
{
    T GetById(int id);
    IEnumerable<T> GetAll();
    int GetCount();
    void Truncate();
    void Delete(int id);
}