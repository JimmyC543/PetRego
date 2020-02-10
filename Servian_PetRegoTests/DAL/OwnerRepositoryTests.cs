using PetRego.DAL;
using PetRego.DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace PetRegoTests.DAL
{
    public class OwnerRepositoryTests : TestClassBase<tblOwner>
    {
        [Theory]
        [InlineData("a8eab20c-55bd-4526-a162-2ff8959b8862", "b66b7d7b-62b1-4feb-a128-058aa5f99a3f", "Rex")]
        public void GePetsByOwnerId_ShouldPass_WhenIdExists(string ownerId, string petId, string name)
        {
            //Arrange
            Guid ownerGuid = Guid.Parse(ownerId);
            Guid petGuid = Guid.Parse(petId);
            IEnumerable<tblPet> expected = GenerateTblPets().Where(owner => owner.Id == ownerGuid);

            IOwnerRepository repo = new OwnerRepository(_context);

            //Act
            var result = repo.GetPetsByOwnerIdAsync(petGuid).Result;

            //Assert
            Assert.True(result != null);
            Assert.IsAssignableFrom<IEnumerable<tblPet>>(result);

            Assert.Equal(expected.Count(), result.Count());
            foreach (var entity in result)
            {
                var expectedEntity = expected.FirstOrDefault(x => x.Id == entity.Id);
                Assert.True(expectedEntity != null);
                Assert.Equal(expectedEntity.Id, entity.Id);
                Assert.Equal(expectedEntity.Name, entity.Name);
                Assert.Equal(expectedEntity.FKOwnerId, entity.FKOwnerId);
                Assert.Equal(expectedEntity.FKAnimalTypeId, entity.FKAnimalTypeId);
            }
        }

        [Theory]
        [InlineData("00000000-0000-0000-0000-000000000000")]
        public void GetPetsByOwnerId_ShouldReturnEmptyIEnumerable_WhenIdDoesNotExist(string ownerId)
        {
            //Arrange
            Guid ownerGuid = Guid.Parse(ownerId);
            IOwnerRepository repo = new OwnerRepository(_context);

            //Act
            var result = repo.GetPetsByOwnerIdAsync(ownerGuid).Result;

            //Assert
            Assert.True(result != null);
            Assert.IsAssignableFrom<IEnumerable<tblPet>>(result);
            Assert.Empty(result);
        }
    }
}
