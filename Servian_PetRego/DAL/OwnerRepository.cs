using Servian_PetRego.DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servian_PetRego.DAL
{
    public class OwnerRepository : Repository<tblOwner>, IOwnerRepository
    {
        //TODO: Refactor and move _dbContext to Repository<TEntity> class?
        private readonly PetRegoDbContext _dbContext;
        public OwnerRepository(PetRegoDbContext context) : base(context)
        {
            _dbContext = context;
        }

        public IEnumerable<tblPet> GetPetsByOwnerId(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
