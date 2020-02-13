using PetRego.DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PetRego.BLL
{
    public interface IPetService
    {
        public Task<tblPet> GetByIdAsync(Guid id);
        public Task<IEnumerable<tblPet>> GetAllAsync();
        public Task<tblOwner> GetOwnerAsync(Guid petId);
        public Task<IEnumerable<tblPet>> FindAsync(Expression<Func<tblPet, bool>> predicate);
        public Task<tblPet> Add(tblPet entity);
        public Task AddRange(IEnumerable<tblPet> entities);
        public Task Update(tblPet entity);
        public Task<tblPet> Remove(Guid id);
        public Task RemoveRange(IEnumerable<Guid> ids);
    }
}
