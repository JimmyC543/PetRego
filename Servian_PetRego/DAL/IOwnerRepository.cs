using Servian_PetRego.DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servian_PetRego.DAL
{
    public interface IOwnerRepository : IRepository<tblOwner>
    {
        public IEnumerable<tblPet> GetPetsByOwnerId(Guid id);
    }
}
