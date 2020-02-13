using PetRego.DAL;
using PetRego.DAL.DataModels;
using PetRego.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PetRego.BLL
{
    public class OwnerService : IOwnerService
    {
        private readonly IOwnerRepository _ownerRepository;
        private readonly IPetRepository _petRepository;

        public OwnerService(IOwnerRepository ownerRepository, IPetRepository petRepository)
        {
            _ownerRepository = ownerRepository ?? throw new ArgumentNullException(nameof(ownerRepository));
            _petRepository = petRepository;
        }

        public async Task<tblOwner> Add(tblOwner entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("Cannot add blank owner");
            }
            if (await _ownerRepository.GetByIdAsync(entity.Id).ConfigureAwait(false) != null)
            {
                throw new InvalidOperationException("Owner already exists.");
            }

            return await _ownerRepository.Add(entity);
        }

        public async Task AddRange(IEnumerable<tblOwner> entities)
        {
            if ((await _ownerRepository.FindAsync(o => entities.Select(e => e.Id).Contains(o.Id)).ConfigureAwait(false)).Any())
            {
                throw new ArgumentException("One of the owners already exist.");
            }

            _ownerRepository.AddRange(entities);
        }

        public async Task<IEnumerable<tblOwner>> FindAsync(Expression<Func<tblOwner, bool>> predicate)
        {
            return await _ownerRepository.FindAsync(predicate).ConfigureAwait(false);
        }

        public async Task<IEnumerable<tblOwner>> GetAllAsync()
        {
            return await _ownerRepository.GetAllAsync().ConfigureAwait(false);
        }

        public async Task<tblOwner> GetByIdAsync(Guid id)
        {
            return await _ownerRepository.GetByIdAsync(id).ConfigureAwait(false);
        }

        public async Task<IEnumerable<tblPet>> GetPetsAsync(Guid ownerId)
        {
            return await _ownerRepository.GetPetsByOwnerIdAsync(ownerId).ConfigureAwait(false);
        }

        public async Task Update(tblOwner owner)
        {
            if (await _ownerRepository.GetByIdAsync(owner.Id) == null)
            {
                throw new EntityNotFoundException($"Could not find owner with id {owner.Id}");
            };

            _ownerRepository.Update(owner);
        }

        public async Task<tblOwner> Remove(Guid id)
        {
            if (id == null)
            {
                throw new ArgumentNullException("Cannot remove blank entity");
            }

            return await _ownerRepository.Remove(id);
        }

        public async Task RemoveRange(IEnumerable<Guid> ids)
        {
            if (!ids.Any())
            {
                return;
            }

            var entitiesToRemove = await _ownerRepository.FindAsync(x => ids.Contains(x.Id)).ConfigureAwait(false);

            if (entitiesToRemove.Count() != ids.Count())
            {
                throw new EntityNotFoundException("Can't remove all of the owners; at least one does not exist.");
            }

            //TODO: Wrap in a try/catch and rollback if either step fails.
            _petRepository.RemoveRange(await _petRepository.FindAsync(p => ids.Contains(p.FKOwnerId.Value)));
            _ownerRepository.RemoveRange(entitiesToRemove);
        }
    }
}
