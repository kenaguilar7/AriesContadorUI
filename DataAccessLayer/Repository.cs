using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLayer
{
    public class Repository<T> : IRepository<T> where T : class
    {
        public void Add(T entity)
        {
            
        }

        public void Remove(T entity)
        {
            throw new NotImplementedException();
        }

        public void Update(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
