﻿using PetRego.DAL;
using PetRego.DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace PetRegoTests.DAL
{
    public class PetRepositoryTests : TestClassBase<tblPet>
    {

        [Theory]
        [InlineData("b66b7d7b-62b1-4feb-a128-058aa5f99a3f", "a8eab20c-55bd-4526-a162-2ff8959b8862", "Tony", "Jones", 2)]
        public void GetOwnerByPetId_ShouldPass_WhenIdExists(string petId, string ownerId, string firstName, string lastName, int petCount)
        {
            //Arrange
            Guid petGuid = Guid.Parse(petId);
            var expected = new tblOwner { Id = Guid.Parse(ownerId), FirstName = firstName, LastName = lastName };
            IPetRepository repo = new PetRepository(_context);

            //Act
            var result = repo.GetOwnerByPetIdAsync(petGuid).Result;

            //Assert
            Assert.True(expected != null);
            Assert.True(result != null);
            Assert.IsType<tblOwner>(result);

            // Assert.Equal/True weren't working so just check that all props match the expected value.
            Assert.Equal(expected.Id, result.Id);
            Assert.Equal(expected.FirstName, result.FirstName);
            Assert.Equal(expected.LastName, result.LastName);
            Assert.Equal(petCount, result.Pets.Count);//TODO: Assert that the pets are as expected...
        }

        [Theory]
        [InlineData("00000000-0000-0000-0000-000000000000")]
        public void GetOwnerByPetId_ShouldReturnNull_WhenIdDoesNotExist(string petId)
        {
            //Arrange
            Guid petGuid = Guid.Parse(petId);
            IPetRepository repo = new PetRepository(_context);

            //Act
            var result = repo.GetOwnerByPetIdAsync(petGuid).Result;

            //Assert
            Assert.True(result == null);
        }
    }
}
