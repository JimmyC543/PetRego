using PetRego.DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetRego.DAL
{
    public interface IPetRepository : IRepository<tblPet>
    {
        public Task<tblOwner> GetOwnerByPetIdAsync(Guid petId);
    }
}
