using PetRego.DAL;
using PetRego.DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PetRego.BLL
{
    public class PetService : IPetService
    {
        private readonly IPetRepository _petRepository;
        private readonly IOwnerRepository _ownerRepository;

        public PetService(IPetRepository petRepository, IOwnerRepository ownerRepository)
        {
            _petRepository = petRepository ?? throw new ArgumentNullException(nameof(petRepository));
            _ownerRepository = ownerRepository ?? throw new ArgumentNullException(nameof(ownerRepository));
        }

        //TODO: "Get food for pet" method

        public async Task<tblPet> Add(tblPet entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("Cannot add blank pet");
            }
            if (await _petRepository.GetByIdAsync(entity.Id).ConfigureAwait(false) != null)
            {
                throw new ArgumentException("Pet already exists.");
            }
            if (entity.FKOwnerId.HasValue && await _ownerRepository.GetByIdAsync(entity.FKOwnerId.Value) == null)
            {
                throw new InvalidOperationException("Unable to add pet for owner that doesn't exist.");
            }

            var createdEntity = await _petRepository.Add(entity);

            return await _petRepository.GetByIdAsync(createdEntity.Id);
        }

        public async Task AddRange(IEnumerable<tblPet> entities)
        {
            if ((await _petRepository.FindAsync(o => entities.Select(e => e.Id).Contains(o.Id)).ConfigureAwait(false)).Any())
            {
                throw new ArgumentException("One of the pets already exist.");
            }

            _petRepository.AddRange(entities);
        }

        public async Task<IEnumerable<tblPet>> FindAsync(Expression<Func<tblPet, bool>> predicate)
        {
            return await _petRepository.FindAsync(predicate).ConfigureAwait(false);
        }

        public async Task<IEnumerable<tblPet>> GetAllAsync()
        {
            return await _petRepository.GetAllAsync().ConfigureAwait(false);
        }

        public async Task<tblPet> GetByIdAsync(Guid id)
        {
            return await _petRepository.GetByIdAsync(id).ConfigureAwait(false);
        }

        public async Task<tblOwner> GetOwnerAsync(Guid petId)
        {
            return await _petRepository.GetOwnerByPetIdAsync(petId).ConfigureAwait(false);
        }
        public async Task Update(tblPet pet)
        {
            _petRepository.Update(pet);
        }

        public async Task<tblPet> Remove(Guid id)
        {
            return await _petRepository.Remove(id);
        }

        public async Task RemoveRange(IEnumerable<Guid> ids)
        {
            if (!ids.Any())
            {
                return;
            }

            var entitiesToRemove = await _petRepository.FindAsync(x => ids.Contains(x.Id)).ConfigureAwait(false);

            if (entitiesToRemove.Count() != ids.Count())
            {
                throw new InvalidOperationException("Can't remove all of the pets; at least one does not exist.");
            }

            _petRepository.RemoveRange(entitiesToRemove);
        }
    }
}
