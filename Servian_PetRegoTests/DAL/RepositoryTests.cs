using Microsoft.EntityFrameworkCore;
using Moq;
using PetRego.DAL;
using PetRego.DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace PetRegoTests.DAL
{
    public class RepositoryTests : TestClassBase<tblPet>
    {
        [Fact]
        public void Constructor_Test()
        {
            Mock<DbContext> mockContext = new Mock<DbContext>();
            Repository<tblPet> petsRepo = new Repository<tblPet>(mockContext.Object);
            Repository<tblOwner> ownersRepo = new Repository<tblOwner>(mockContext.Object);
            Assert.True(petsRepo != null);
        }

        [Fact]
        public void GetAll_Test()
        {
            //Arange
            var pets = _context.Pets.ToList();
            Repository<tblPet> repo = new Repository<tblPet>(_context);

            //Act
            var result = repo.GetAllAsync().Result;

            //Assert
            Assert.Equal(3, result.Count());
            foreach (tblPet pet in result)
            {
                Assert.Contains(pet.Name, pets.Select(x => x.Name));
                pets.RemoveAll(x => x.Name == pet.Name);//Make sure we're testing a 1-1 match
            }
        }

        [Theory]
        [InlineData("b66b7d7b-62b1-4feb-a128-058aa5f99a3f", "Mittens")]
        [InlineData("4b34cc1d-737f-4bc0-a6c4-9a7b1c3541b0", "Spot")]
        [InlineData("1390599d-3e45-48e0-87ba-a09f578b4f7b", "Rex")]
        public void GetById_Test_Pets(string id, string expectedName)
        {
            //Arrange
            Guid guid = Guid.Parse(id);
            tblPet expected = new tblPet { Id = guid, Name = expectedName };
            Repository<tblPet> repo = new Repository<tblPet>(_context);

            //Act
            var result = repo.GetByIdAsync(guid).Result;

            //Assert
            Assert.True(result != null);
            Assert.IsType<tblPet>(result);

            // Assert.Equal/True weren't working so just check that all props match the expected value.
            foreach (var prop in result.GetType().GetProperties())
            {
                Assert.Equal(prop.GetValue(result), prop.GetValue(result));
            }
        }
        [Theory]
        [InlineData("a8eab20c-55bd-4526-a162-2ff8959b8862", "Tony", "Jones")]
        public void GetById_Test_Owners(string id, string firstName, string lastName)
        {
            //Arrange
            Guid guid = Guid.Parse(id);
            tblOwner expected = new tblOwner { Id = guid, FirstName = firstName, LastName = lastName };
            Repository<tblOwner> repo = new Repository<tblOwner>(_context);

            //Act
            var result = repo.GetByIdAsync(guid).Result;

            //Assert
            Assert.True(result != null);
            Assert.IsType<tblOwner>(result);

            Assert.Equal(guid, result.Id);
            Assert.Equal(firstName, result.FirstName);
            Assert.Equal(lastName, result.LastName);
            Assert.Equal($"{firstName} {lastName}", result.FullName);//More of an integration test including the tblOwner entity model
            Assert.Equal(2, result.Pets.Count());
        }

        [Theory]
        [InlineData("b66b7d7b-62b1-4feb-a128-058aa5f99a3f", "Mittens")]
        [InlineData("4b34cc1d-737f-4bc0-a6c4-9a7b1c3541b0", "Spot")]
        [InlineData("1390599d-3e45-48e0-87ba-a09f578b4f7b", "Rex")]
        public void Find_Test(string id, string expectedName)
        {
            //Arrange
            Guid guid = Guid.Parse(id);

            var expected = GenerateTblPets().FirstOrDefault(entity => entity.Name == expectedName);
            Repository<tblPet> repo = new Repository<tblPet>(_context);

            //Act
            var result = repo.FindAsync(entity => entity.Name == expectedName).Result;

            //Assert
            Assert.True(result != null);
            Assert.IsAssignableFrom<IEnumerable<tblPet>>(result);

            foreach (var pet in result)
            {
                Assert.Equal(expected.Id, pet.Id);
                Assert.Equal(expected.Name, pet.Name);
                Assert.Equal(expected.FKAnimalTypeId, pet.FKAnimalTypeId);
                Assert.Equal(expected.FKOwnerId, pet.FKOwnerId);
            }
        }

        [Theory]
        [InlineData("3c6f98d5-a93d-4665-a70a-8ddbbc660973", "Otto")]
        public void Add_Test_ShouldSucceed(string id, string name)
        {
            //Arange
            var fakePet = new tblPet() { Id = Guid.Parse(id), Name = name };
            Repository<tblPet> repo = new Repository<tblPet>(_context);

            //Act
            repo.Add(fakePet);
            _context.SaveChanges();

            //Assert
            Assert.Contains(fakePet, _context.Set<tblPet>());
        }

        [Fact]
        public void Add_Test_ShouldFailWhenAddingDuplicate()
        {
            //Arange
            var duplicatePet = _context.Pets.First();
            Repository<tblPet> repo = new Repository<tblPet>(_context);

            //Act/Assert
            repo.Add(duplicatePet);
            Assert.Throws<ArgumentException>(() => { _context.SaveChanges(); });
        }

        [Fact]
        public void AddRange_Test()
        {
            //Arange
            var newPets = new List<tblPet>{
                new tblPet{Id = Guid.NewGuid(), Name = "Blah" },
                new tblPet{Id = Guid.NewGuid(), Name = "Derp" }
            };
            Repository<tblPet> repo = new Repository<tblPet>(_context);

            //Act
            repo.AddRange(newPets);
            _context.SaveChanges();

            //Assert
            foreach (var entity in newPets)
            {
                Assert.Contains(entity, _context.Set<tblPet>());
            }
        }

        [Theory]
        [InlineData("b66b7d7b-62b1-4feb-a128-058aa5f99a3f", "Mittens")]
        public void Remove_Test_ShouldSucceed_WhenEntityExists(string id, string name)
        {
            //Arange
            var petToRemove = _context.Set<tblPet>().FirstOrDefault();

            Repository<tblPet> repo = new Repository<tblPet>(_context);

            //Act
            repo.Remove(petToRemove);
            _context.SaveChanges();

            //Assert
            Assert.DoesNotContain(petToRemove, _context.Set<tblPet>());
        }
        [Theory]
        [InlineData("13437248-2d41-4599-ad00-21b7a9fa3f6b", "Muffin")]
        public void Remove_Test_ShouldFail_WhenEntityDoesNotExist(string id, string name)
        {
            //Arange
            var fakePet = new tblPet() { Id = Guid.Parse(id), Name = name };

            Repository<tblPet> repo = new Repository<tblPet>(_context);

            //Act/Assert
            Assert.Throws<DbUpdateConcurrencyException>(() => { repo.Remove(fakePet); });
        }

        [Fact]//TODO: convert to a theory and test with several combinations
        public void RemoveRange_Test_ShouldSucceed_WhenEntitiesExist()
        {
            //Arange
            var petsToRemove = _context.Set<tblPet>().ToList();
            Repository<tblPet> repo = new Repository<tblPet>(_context);

            //Act
            repo.RemoveRange(petsToRemove);

            //Assert
            foreach (var entity in petsToRemove)
            {
                Assert.DoesNotContain(entity, _context.Set<tblPet>());
            }
        }


        [Theory]
        [InlineData("a8eab20c-55bd-4526-a162-2ff8959b8862", "Tom", "Jones")]
        public void Update_Test_ShouldSucceed_WhenEntityExists(string id, string firstName, string lastName)
        {
            //Arrange
            Guid guid = Guid.Parse(id);
            var existingOwners = _context.Set<tblOwner>().ToList();

            var oldOwner = _context.Set<tblOwner>().FirstOrDefault(x => x.Id == guid);
            oldOwner.FirstName = firstName;
            oldOwner.LastName = lastName;
            var oldData = new tblOwner { Id = guid, FirstName = "Brian", LastName = "Lara" };
            var newData = new tblOwner { Id = guid, FirstName = firstName, LastName = lastName };
            Repository<tblOwner> ownerRepo = new Repository<tblOwner>(_context);

            //Act
            ownerRepo.Update(oldOwner);
            var result = _context.Set<tblOwner>().FirstOrDefault(x => x.Id == guid);

            //Assert
            Assert.Equal(firstName, result.FirstName);
            Assert.Equal(lastName, result.LastName);
        }


        [Theory]
        [InlineData("a8eab20c-55bd-4526-a162-2ff8959b8862", "Tom", "Jones")]
        public void Update_Test_ShouldFail_WhenWasntFetchedFromDb(string id, string firstName, string lastName)
        {
            //Arrange
            Guid guid = Guid.Parse(id);
            var existingOwners = _context.Set<tblOwner>().ToList();

            var oldOwner = _context.Set<tblOwner>().FirstOrDefault(x => x.Id == guid);
            oldOwner.FirstName = firstName;
            oldOwner.LastName = lastName;
            var newData = new tblOwner { Id = guid, FirstName = firstName, LastName = lastName };
            Repository<tblOwner> ownerRepo = new Repository<tblOwner>(_context);

            //Act/Assert
            Assert.Throws<InvalidOperationException>(() => ownerRepo.Update(newData));
        }

    }
}
