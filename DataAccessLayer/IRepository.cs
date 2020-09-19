using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLayer
{
    public interface IRepository<T> where T : class
    {
        T GetById(object id);
        IEnumerable<T> GetAll();
        void Add(T entity);
        void AddRange(IEnumerable<T> entities);
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);

    }
}
