using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLayer
{
    public class Repository<T> :  IRepository<T> where T : class
    {
        

        public void Add(T entity)
        {
            

            throw new NotImplementedException();
        }

        public void AddRange(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetAll()
        {
            throw new NotImplementedException();
        }

        public T GetById(object id)
        {
            throw new NotImplementedException();
        }

        public void Remove(T entity)
        {
            throw new NotImplementedException();
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
        }

        public void Update(T entity)
        {
            throw new NotImplementedException();
        }

        public void UpdateRange(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
        }
    }
}
