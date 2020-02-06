﻿using Microsoft.EntityFrameworkCore;
using Moq;
using Servian_PetRego.DAL;
using Servian_PetRego.DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Servian_PetRegoTests.DAL
{
    public class RepositoryTests : TestClassBase<tblPet>
    {
        [Fact]
        public void Constructor_Test()
        {
            Mock<DbContext> mockContext = new Mock<DbContext>();
            Repository<tblPet> repo = new Repository<tblPet>(mockContext.Object);
            Assert.True(repo != null);
        }

        [Fact]
        public void GetAll_Test()
        {
            //Arange
            var pets = _context.Pets.ToList();
            Repository<tblPet> repo = new Repository<tblPet>(_context);

            //Act
            var result = repo.GetAll();

            //Assert
            Assert.Equal(3, result.Count());
            foreach(tblPet pet in result)
            {
                Assert.Contains(pet.Name, pets.Select(x => x.Name));
                pets.RemoveAll(x => x.Name == pet.Name);//Make sure we're testing a 1-1 match
            }
        }

        [Theory]
        [InlineData("b66b7d7b-62b1-4feb-a128-058aa5f99a3f", "Mittens")]
        [InlineData("4b34cc1d-737f-4bc0-a6c4-9a7b1c3541b0", "Spot")]
        [InlineData("1390599d-3e45-48e0-87ba-a09f578b4f7b", "Rex")]
        public void GetById_Test(string id, string expectedName)
        {
            //Arrange
            Guid guid = Guid.Parse(id);
            tblPet expected = new tblPet { Id = guid, Name = expectedName };
            Repository<tblPet> repo = new Repository<tblPet>(_context);

            //Act
            var result = repo.GetById(guid);

            //Assert
            Assert.True(result != null);
            Assert.IsType<tblPet>(result);

            // Assert.Equal/True weren't working so just check that all props match the expected value.
            foreach(var prop in result.GetType().GetProperties())
            {
                Assert.Equal(prop.GetValue(result), prop.GetValue(result));
            }
        }

        [Theory]
        [InlineData("b66b7d7b-62b1-4feb-a128-058aa5f99a3f", "Mittens")]
        [InlineData("4b34cc1d-737f-4bc0-a6c4-9a7b1c3541b0", "Spot")]
        [InlineData("1390599d-3e45-48e0-87ba-a09f578b4f7b", "Rex")]
        public void Find_Test(string id, string expectedName)
        {
            //Arrange
            Guid guid = Guid.Parse(id);
            tblPet expected = new tblPet { Id = guid, Name = expectedName };
            Repository<tblPet> repo = new Repository<tblPet>(_context);

            //Act
            var result = repo.Find(entity => entity.Name == expectedName);

            //Assert
            Assert.True(result != null);
            Assert.IsAssignableFrom<IEnumerable<tblPet>>(result);

            // Assert.Equal/True weren't working so just check that all props match the expected value.
            foreach(var prop in result.GetType().GetProperties())
            {
                Assert.Equal(prop.GetValue(result), prop.GetValue(result));
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

            //Act
            repo.Remove(fakePet);
            

            //Assert
            Assert.Throws<DbUpdateConcurrencyException>(() => { _context.SaveChanges(); });
        }

        [Fact]//TODO: convert to a theory and test with several combinations
        public void RemoveRange_Test_ShouldSucceed_WhenEntitiesExist()
        {
            //Arange
            var petsToRemove = _context.Set<tblPet>().ToList();

            Repository<tblPet> repo = new Repository<tblPet>(_context);

            //Act
            repo.RemoveRange(petsToRemove);
            _context.SaveChanges();

            //Assert
            foreach (var entity in petsToRemove)
            {
                Assert.DoesNotContain(entity, _context.Set<tblPet>());
            }
        }

        //protected override IEnumerable<tblPet> Sample()
        //{
        //    return new[]
        //    {
        //        new tblPet { Id=Guid.Parse("b66b7d7b-62b1-4feb-a128-058aa5f99a3f"), Name = "Mittens" },
        //        new tblPet { Id=Guid.Parse("1390599d-3e45-48e0-87ba-a09f578b4f7b"), Name = "Rex" },
        //        new tblPet { Id=Guid.Parse("4b34cc1d-737f-4bc0-a6c4-9a7b1c3541b0"), Name = "Spot" }
        //    };
        //}
    }
}
