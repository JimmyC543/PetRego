using Microsoft.EntityFrameworkCore;
using PetRego.DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetRego.DAL
{
    public class OwnerRepository : Repository<tblOwner>, IOwnerRepository
    {
        //TODO: Refactor and move _dbContext to Repository<TEntity> class?
        private readonly PetRegoDbContext _dbContext;
        public OwnerRepository(PetRegoDbContext context) : base(context)
        {
            _dbContext = context;
        }

        //Futures: This potentially belongs in the Business Layer, along with additional validation (etc) logic
        public async Task<IEnumerable<tblPet>> GetPetsByOwnerIdAsync(Guid id)
        {
            return (await _dbContext.Owners.FirstOrDefaultAsync(owner => owner.Id == id).ConfigureAwait(false))?.Pets ?? new List<tblPet>();
        }
    }
}
