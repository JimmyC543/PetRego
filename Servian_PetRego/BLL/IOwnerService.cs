using PetRego.DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PetRego.BLL
{
    public interface IOwnerService
    {
        public Task<tblOwner> GetByIdAsync(Guid id);
        public Task<IEnumerable<tblOwner>> GetAllAsync();
        public Task<IEnumerable<tblPet>> GetPetsAsync(Guid ownerId);
        public Task<IEnumerable<tblOwner>> FindAsync(Expression<Func<tblOwner, bool>> predicate);
        public Task Add(tblOwner entity);
        public Task AddRange(IEnumerable<tblOwner> entities);
        public Task Remove(Guid id);
        public Task RemoveRange(IEnumerable<Guid> ids);
    }
}
