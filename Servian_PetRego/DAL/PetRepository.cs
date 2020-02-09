using Servian_PetRego.DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servian_PetRego.DAL
{
    public class PetRepository : Repository<tblPet>, IPetRepository
    {
        private readonly PetRegoDbContext _dbContext;
        public PetRepository(PetRegoDbContext context) : base(context)
        {
            _dbContext = context;
        }

        //Futures: This potentially belongs in the Business Layer, along with additional validation (etc) logic
        public tblOwner GetOwnerByPetId(Guid petId)
        {
            return _dbContext.Pets.FirstOrDefault(pet => pet.Id == petId)?.Owner;
        }
    }
}
