﻿using Microsoft.EntityFrameworkCore;
using PetRego.DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetRego.DAL
{
    public class PetRepository : Repository<tblPet>, IPetRepository
    {
        private readonly PetRegoDbContext _dbContext;
        public PetRepository(PetRegoDbContext context) : base(context)
        {
            _dbContext = context;
        }

        //Futures: This potentially belongs in the Business Layer, along with additional validation (etc) logic
        public async Task<tblOwner> GetOwnerByPetIdAsync(Guid petId)
        {
            return (await _dbContext.Pets.FirstOrDefaultAsync(pet => pet.Id == petId))?.Owner;
        }
    }
}
