using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PetRego.DAL
{
    public interface IRepository<T> where T : class
    {
        public Task<T> GetByIdAsync(Guid id);
        public Task<IEnumerable<T>> GetAllAsync();
        public Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        public Task<T> Add(T entity);
        public Task AddRange(IEnumerable<T> entities);
        public void Update(T entity);
        public void Remove(T entity);
        public void RemoveRange(IEnumerable<T> entities);
    }
}
