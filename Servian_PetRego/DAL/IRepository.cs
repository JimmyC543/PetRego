using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PetRego.DAL
{
    public interface IRepository<T> where T : class
    {
        public T GetById(Guid id);
        public IEnumerable<T> GetAll();
        public void Add(T entity);
        public void AddRange(IEnumerable<T> entities);
        public IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
        public void Remove(T entity);
        public void RemoveRange(IEnumerable<T> entities);
    }
}
