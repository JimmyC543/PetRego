using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Servian_PetRego.DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servian_PetRego.DAL
{
    public class PetRegoDbContext : DbContext
    {
        //TODO: try feeding in an IDbContextOptions argument, if possible
        public PetRegoDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<tblOwner> Owners { get; set; }
        public DbSet<tblPet> Pets { get; set; }
        public DbSet<LkpAnimalType> AnimalTypes { get; set; }
    }
}
