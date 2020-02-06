using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Servian_PetRego.DAL
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly DbSet<T> _entities;

        public Repository(DbContext dbContext)
        {
            //TODO: Null check?
            _entities = dbContext.Set<T>();
        }

        public T GetById(Guid id)
        {
            return _entities.Find(id);
        }

        public IEnumerable<T> GetAll()
        {
            return _entities.ToList();
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return _entities.Where(predicate);
        }

        public void Add(T entity)
        {
            _entities.Add(entity);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            _entities.AddRange(entities);
        }

        public void Remove(T entity)
        {
            //TODO: Add logic to handle "not found" case
            _entities.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            //TODO: Add logic to handle "not found" case
            _entities.RemoveRange(entities);
        }
    }
}
