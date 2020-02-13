using Microsoft.EntityFrameworkCore;
using PetRego.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PetRego.DAL
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly DbSet<TEntity> _entities;
        private readonly DbContext _dbContext;

        public Repository(DbContext dbContext)
        {
            //TODO: Null check?
            if (dbContext == null)
            {
                throw new ArgumentNullException(nameof(dbContext));
            }

            _dbContext = dbContext;
        }

        public virtual async Task<TEntity> GetByIdAsync(Guid id)
        {
            return await _dbContext.Set<TEntity>().FindAsync(id);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbContext.Set<TEntity>().AsNoTracking().ToListAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbContext.Set<TEntity>().Where(predicate).ToListAsync().ConfigureAwait(false);
        }

        public async Task<TEntity> Add(TEntity entity)
        {
            var savedEntity = _dbContext.Set<TEntity>().Add(entity).Entity;
            await _dbContext.SaveChangesAsync();
            return savedEntity;
        }

        public async Task AddRange(IEnumerable<TEntity> entities)
        {
            _dbContext.Set<TEntity>().AddRange(entities);
            await _dbContext.SaveChangesAsync();
        }

        public void Update(TEntity entity)
        {
            _dbContext.Set<TEntity>().Update(entity);
            _dbContext.SaveChanges();
        }

        public virtual async Task<TEntity> Remove(Guid id)
        {
            //TODO: Add logic to handle "not found" case
            TEntity entity = await _dbContext.Set<TEntity>().FindAsync(id);
            if (entity == null)
            {
                throw new EntityNotFoundException("Cannot remove record, as it doesn't exist.");
            }
            _dbContext.Set<TEntity>().Remove(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task RemoveRange(IEnumerable<TEntity> entities)
        {
            //TODO: Add logic to handle "not found" case
            _dbContext.Set<TEntity>().RemoveRange(entities);
            await _dbContext.SaveChangesAsync();
        }
    }
}
