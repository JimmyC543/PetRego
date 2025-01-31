﻿using PetRego.DAL;
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

            await _ownerRepository.Update(owner);
        }

        public async Task<tblOwner> Remove(Guid id)
        {
            if (id == null)
            {
                throw new ArgumentNullException("Cannot remove blank entity");
            }
            var owner = await _ownerRepository.GetByIdAsync(id);
            if (owner == null)
            {
                throw new EntityNotFoundException("Could not find owner to delete");
            }
            await _petRepository.RemoveRange(owner.Pets);
            await _ownerRepository.Remove(owner);
            return owner;
        }

        public async Task RemoveRange(IEnumerable<Guid> ids)
        {
            if (!ids.Any())
            {
                return;
            }

            //Just in case there are duplicate ids sent in the request, we're only interested in the unique ones
            var uniqueIds = ids.Distinct();
            var entitiesToRemove = await _ownerRepository.FindAsync(x => uniqueIds.Contains(x.Id)).ConfigureAwait(false);

            if (entitiesToRemove.Count() != uniqueIds.Count())
            {
                throw new EntityNotFoundException("Can't remove all of the owners; at least one does not exist.");
            }

            //TODO: Wrap in a try/catch and rollback if either step fails.
            await _petRepository.RemoveRange(await _petRepository.FindAsync(p => uniqueIds.Contains(p.FKOwnerId.Value)));
            await _ownerRepository.RemoveRange(entitiesToRemove);
        }
    }
}
