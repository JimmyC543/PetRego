using Microsoft.EntityFrameworkCore;
using Servian_PetRego.DAL;
using Servian_PetRego.DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Servian_PetRegoTests
{
    public abstract class TestClassBase<T> where T : class
    {
        protected readonly PetRegoDbContext _context;
        public TestClassBase()
        {
            DbContextOptions<PetRegoDbContext> options = new DbContextOptionsBuilder<PetRegoDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())//give a new name each time
                .Options;
            _context = new PetRegoDbContext(options);

            _context.Database.EnsureCreated();

            Seed(_context);
            //var entities = Sample();
            //_context.Set<T>().AddRange(entities);
            //_context.SaveChanges();

        }

        protected void Seed(PetRegoDbContext context)
        {

            var animalTypes = new List<LkpAnimalType>
            {
                new LkpAnimalType { Id = 1, AnimalType = "Dog", FoodSource = "Bones" },
                new LkpAnimalType { Id = 2, AnimalType = "Cat", FoodSource = "Fish" },
                new LkpAnimalType { Id = 3, AnimalType = "Chicken", FoodSource = "Corn" },
                new LkpAnimalType { Id = 4, AnimalType = "Snake", FoodSource = "Mice" }
            };
            _context.AnimalTypes.AddRange(animalTypes);


            var owners = new List<tblOwner>
            {
                new tblOwner { Id = Guid.Parse("a8eab20c-55bd-4526-a162-2ff8959b8862"), FirstName = "Tony", LastName = "Jones" },
                new tblOwner { Id = Guid.Parse("5ed785e7-32d6-4dc5-83cc-80f78881dbd9"), FirstName = "Leigh", LastName = "Sales" },
            };
            _context.Owners.AddRange(owners);

            var pets = new List<tblPet>
            {
                new tblPet { Id=Guid.Parse("b66b7d7b-62b1-4feb-a128-058aa5f99a3f"), Name = "Mittens", FKAnimalTypeId = 2, FKOwnerId = Guid.Parse("a8eab20c-55bd-4526-a162-2ff8959b8862") },
                new tblPet { Id=Guid.Parse("1390599d-3e45-48e0-87ba-a09f578b4f7b"), Name = "Rex", FKAnimalTypeId = 1, FKOwnerId = Guid.Parse("a8eab20c-55bd-4526-a162-2ff8959b8862") },
                new tblPet { Id=Guid.Parse("4b34cc1d-737f-4bc0-a6c4-9a7b1c3541b0"), Name = "Foghorn", FKAnimalTypeId = 3, FKOwnerId = Guid.Parse("5ed785e7-32d6-4dc5-83cc-80f78881dbd9") }
            };
            _context.Pets.AddRange(pets);

            _context.SaveChanges();
        }

        //protected virtual IEnumerable<T> Sample()
        //{
        //    return new[]
        //    {
        //        new tblPet { Id=Guid.Parse("b66b7d7b-62b1-4feb-a128-058aa5f99a3f"), Name = "Mittens" },
        //        new tblPet { Id=Guid.Parse("1390599d-3e45-48e0-87ba-a09f578b4f7b"), Name = "Rex" },
        //        new tblPet { Id=Guid.Parse("4b34cc1d-737f-4bc0-a6c4-9a7b1c3541b0"), Name = "Spot" }
        //    };
        //}

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
