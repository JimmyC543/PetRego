using PetRego.DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetRego.DAL
{
    public interface IOwnerRepository : IRepository<tblOwner>
    {
        public Task<IEnumerable<tblPet>> GetPetsByOwnerIdAsync(Guid id);
    }
}
