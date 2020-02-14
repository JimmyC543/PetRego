using Moq;
using PetRego.BLL;
using PetRego.DAL;
using PetRego.DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PetRegoTests.BLL
{
    public class PetServiceTests
    {

        private readonly IPetService _service;
        private readonly Mock<IPetRepository> _mockPetRepository;
        private readonly Mock<IOwnerRepository> _mockOwnerRepository;
        public PetServiceTests()
        {
            _mockPetRepository = new Mock<IPetRepository>();
            _mockOwnerRepository = new Mock<IOwnerRepository>();
            _service = new PetService(_mockPetRepository.Object, _mockOwnerRepository.Object);
        }

        [Fact]
        public void Constructor_Test()
        {
            Assert.True(_service != null);
        }

        #region GetAll tests
        [Fact]
        public void GetAll_Test()
        {
            //Arange
            var fakeOwnerId = Guid.NewGuid();
            _mockPetRepository.Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<tblPet> { new tblPet { Id = Guid.NewGuid(), Name = "Spot", FKAnimalTypeId = 1, FKOwnerId = fakeOwnerId } });

            Mock<IPetRepository> mockPetRepository = new Mock<IPetRepository>();

            //Act
            var result = _service.GetAllAsync().Result;

            //Assert
            _mockPetRepository.Verify(x => x.GetAllAsync(), Times.Once());
            Assert.Single(result);
            Assert.Contains("Spot", result.First().Name);
            Assert.Equal(1, result.First().FKAnimalTypeId);
            Assert.Equal(fakeOwnerId, result.First().FKOwnerId);
        }
        #endregion

        #region GetPetById tests

        [Theory]
        [InlineData("09ae6e94-3a0d-416c-b28b-7b1e0a710493")]
        public void GetPetById_ShouldSucceedWithValidId(string id)
        {
            //Arrange
            Guid guid = Guid.Parse(id);
            _mockPetRepository.Setup(x => x.GetByIdAsync(guid))
                .ReturnsAsync(new tblPet { Id = guid, Name = "Spot", FKAnimalTypeId = 1 });

            //Act
            var result = _service.GetByIdAsync(guid).Result;

            //Assert
            _mockPetRepository.Verify(x => x.GetByIdAsync(guid), Times.Once());
            Assert.Equal(guid, result.Id);
            Assert.Equal("Spot", result.Name);
            Assert.Equal(1, result.FKAnimalTypeId);
        }

        [Theory]
        [InlineData("09ae6e94-3a0d-416c-b28b-7b1e0a710493")]
        public void GetPetById_ShouldReturnNullIfIdDoesntMatchs(string id)
        {
            //Arrange
            Guid wrongGuid = Guid.Parse("c3b922ac-a2bf-4b75-a9dc-5b3c44798264");
            Guid guid = Guid.Parse(id);
            _mockPetRepository.Setup(x => x.GetByIdAsync(guid))
                .ReturnsAsync(new tblPet { Id = guid, Name = "Pete", FKAnimalTypeId = 1 });

            //Act
            var result = _service.GetByIdAsync(wrongGuid).Result;

            //Assert
            _mockPetRepository.Verify(x => x.GetByIdAsync(wrongGuid), Times.Once());
            Assert.Null(result);
        }
        #endregion

        #region GetOwnerByPetId tests

        [Theory]
        [InlineData("09d199e8-fd8c-4bfb-8359-725d80f3f421", "09ae6e94-3a0d-416c-b28b-7b1e0a710493", "Bob", "Jane")]
        public void GetOwnerByPetId_ShouldSucceedWithValidId(string petId, string ownerId, string firstName, string lastName)
        {
            //Arrange
            Guid petGuid = Guid.Parse(petId);
            Guid ownerGuid = Guid.Parse(ownerId);
            _mockPetRepository.Setup(x => x.GetOwnerByPetIdAsync(petGuid))
                .ReturnsAsync(new tblOwner { Id = ownerGuid, FirstName = firstName, LastName = lastName });

            //Act
            var result = _service.GetOwnerAsync(petGuid).Result;

            //Assert
            _mockPetRepository.Verify(x => x.GetOwnerByPetIdAsync(petGuid), Times.Once());
            Assert.NotNull(result);
            Assert.Equal(ownerGuid, result.Id);
            Assert.Equal(firstName, result.FirstName);
            Assert.Equal(lastName, result.LastName);
        }

        [Theory]
        [InlineData("09ae6e94-3a0d-416c-b28b-7b1e0a710493", "09d199e8-fd8c-4bfb-8359-725d80f3f421", "Travis", "Cloke")]
        public void GetOwnerByPetId_ShouldReturnEmptyIEnumerableWithInvalidId(string ownerId, string petId, string firstName, string lastName)
        {
            //Arrange
            Guid wrongGuid = Guid.Parse("c3b922ac-a2bf-4b75-a9dc-5b3c44798264");
            Guid petGuid = Guid.Parse(petId);
            Guid ownerGuid = Guid.Parse(ownerId);
            _mockPetRepository.Setup(x => x.GetOwnerByPetIdAsync(petGuid))
                .ReturnsAsync(new tblOwner { Id = ownerGuid, FirstName = firstName, LastName = lastName });

            //Act
            var result = _service.GetOwnerAsync(wrongGuid).Result;

            //Assert
            _mockPetRepository.Verify(x => x.GetOwnerByPetIdAsync(petGuid), Times.Never());
            Assert.Null(result);
        }
        #endregion

        #region Add tests
        [Theory]
        [InlineData("09ae6e94-3a0d-416c-b28b-7b1e0a710493", "09d199e8-fd8c-4bfb-8359-725d80f3f421", "Pete", 1)]
        public void Add_ShouldSucceed_WithUnusedId( string petId, string ownerId, string name, int fkAnimalTypeId)
        {
            //Arrange
            Guid wrongGuid = Guid.Parse("c3b922ac-a2bf-4b75-a9dc-5b3c44798264");
            Guid petGuid = Guid.Parse(petId);
            Guid ownerGuid = Guid.Parse(ownerId);
            tblPet pet = new tblPet() { Id = petGuid, Name = name, FKAnimalTypeId = fkAnimalTypeId, FKOwnerId = ownerGuid };

            _mockPetRepository.SetupSequence(x => x.GetByIdAsync(petGuid))
                .ReturnsAsync((tblPet)null)
                .ReturnsAsync(pet);
            _mockOwnerRepository.Setup(x => x.GetByIdAsync(ownerGuid))
                .ReturnsAsync(new tblOwner { Id = ownerGuid, FirstName = "Joe", LastName = "Bob" });
            _mockPetRepository.Setup(x => x.Add(pet))
                .ReturnsAsync(pet);

            //Act
            var result = _service.Add(pet);

            //Assert
            //_mockPetRepository.Verify(x => x.GetByIdAsync(petGuid), Times.Once());
            _mockOwnerRepository.Verify(x => x.GetByIdAsync(ownerGuid), Times.Once());
            _mockPetRepository.Verify(x => x.Add(pet), Times.Once());
            _mockPetRepository.Verify(x => x.GetByIdAsync(petGuid), Times.Exactly(2));
            Assert.True(result.IsCompletedSuccessfully);
        }
        [Theory]
        [InlineData("09ae6e94-3a0d-416c-b28b-7b1e0a710493", "09d199e8-fd8c-4bfb-8359-725d80f3f421", "Pete", 1)]
        public void Add_ShouldFail_WithMismatchingOwnerId( string petId, string ownerId, string name, int fkAnimalTypeId)
        {
            //Arrange
            Guid wrongGuid = Guid.Parse("c3b922ac-a2bf-4b75-a9dc-5b3c44798264");
            Guid petGuid = Guid.Parse(petId);
            Guid ownerGuid = Guid.Parse(ownerId);
            _mockPetRepository.Setup(x => x.GetByIdAsync(petGuid))
                .ReturnsAsync((tblPet)null);

            tblPet pet = new tblPet() { Id = petGuid, Name = name, FKAnimalTypeId = fkAnimalTypeId, FKOwnerId = ownerGuid };
            //Act
            var result = _service.Add(pet);

            //Assert
            _mockPetRepository.Verify(x => x.GetByIdAsync(petGuid), Times.Once());
            _mockOwnerRepository.Verify(x => x.GetByIdAsync(ownerGuid), Times.Once());
            Assert.ThrowsAsync<InvalidOperationException>(() => result);
            _mockPetRepository.Verify(x => x.Add(pet), Times.Never());
            Assert.False(result.IsCompletedSuccessfully);
        }

        [Fact]
        public void Add_ShouldFail_WithNullEntity()
        {
            //Arrange
            Guid guid = Guid.Parse("c3b922ac-a2bf-4b75-a9dc-5b3c44798264");
            _mockPetRepository.Setup(x => x.GetByIdAsync(guid))
                .ReturnsAsync((tblPet)null);

            //Act
            var task = _service.Add(null);

            //Assert
            Assert.ThrowsAnyAsync<ArgumentNullException>(() => task);
            _mockPetRepository.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never());
            _mockPetRepository.Verify(x => x.Add(It.IsAny<tblPet>()), Times.Never());
            Assert.False(task.IsCompletedSuccessfully);
        }

        [Theory]
        [InlineData("09ae6e94-3a0d-416c-b28b-7b1e0a710493", "Dundee", 1)]
        public void Add_ShouldFail_IfPetAlreadyExists(string petId, string name, int fkAnimalTypeId)
        {
            //Arrange
            Guid guid = Guid.Parse(petId);
            tblPet fakePet = new tblPet { Id = guid, Name = name, FKAnimalTypeId = fkAnimalTypeId };
            _mockPetRepository.Setup(x => x.GetByIdAsync(guid))
                .ReturnsAsync(fakePet);

            //Act
            var task = _service.Add(fakePet);

            //Assert
            Assert.ThrowsAnyAsync<InvalidOperationException>(() => task);
            _mockPetRepository.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Once());
            _mockPetRepository.Verify(x => x.Add(It.IsAny<tblPet>()), Times.Never());
            Assert.False(task.IsCompletedSuccessfully);
        }
        #endregion

        #region AddRange tests

        [Theory]
        [InlineData("09ae6e94-3a0d-416c-b28b-7b1e0a710493", "Spot", 1, "09d199e8-fd8c-4bfb-8359-725d80f3f421", "Otto", 2)]
        public void AddRange_ShouldSucceed_WithUnusedIds(string petId1, string name1, int fkAnimalTypeId1, string petId2, string name2, int fkAnimalTypeId2)
        {
            //Arrange
            Guid wrongGuid = Guid.Parse("c3b922ac-a2bf-4b75-a9dc-5b3c44798264");
            Guid ownerGuid1 = Guid.Parse(petId1);
            Guid ownerGuid2 = Guid.Parse(petId2);
            _mockPetRepository.Setup(x => x.FindAsync(It.IsAny<Expression<Func<tblPet, bool>>>()))
                .ReturnsAsync(Enumerable.Empty<tblPet>());

            tblPet owner1 = new tblPet() { Id = ownerGuid1, Name = name1, FKAnimalTypeId = fkAnimalTypeId1 };
            tblPet owner2 = new tblPet() { Id = ownerGuid2, Name = name2, FKAnimalTypeId = fkAnimalTypeId2 };
            var entities = new tblPet[] { owner1, owner2 };

            //Act
            var result = _service.AddRange(entities);

            //Assert
            _mockPetRepository.Verify(x => x.FindAsync(It.IsAny<Expression<Func<tblPet, bool>>>()), Times.Once());
            _mockPetRepository.Verify(x => x.AddRange(entities), Times.Once());
            Assert.True(result.IsCompletedSuccessfully);
        }

        [Theory]
        [InlineData("09ae6e94-3a0d-416c-b28b-7b1e0a710493", "Spot", 1, "09d199e8-fd8c-4bfb-8359-725d80f3f421", "Otto", 2)]
        public void AddRange_ShouldFail_WhenAddingExistingEntities(string petId1, string name1, int fkAnimalTypeId1, string petId2, string name2, int fkAnimalTypeId2)
        {
            //Arrange
            Guid wrongGuid = Guid.Parse("c3b922ac-a2bf-4b75-a9dc-5b3c44798264");
            Guid petGuid1 = Guid.Parse(petId1);
            Guid petGuid2 = Guid.Parse(petId2);

            tblPet pet1 = new tblPet() { Id = petGuid1, Name = name1, FKAnimalTypeId = fkAnimalTypeId1 };
            tblPet pet2 = new tblPet() { Id = petGuid2, Name = name2, FKAnimalTypeId = fkAnimalTypeId2 };
            var entities = new tblPet[] { pet1, pet2 };

            _mockPetRepository.Setup(x => x.FindAsync(It.IsAny<Expression<Func<tblPet, bool>>>()))
                .ReturnsAsync(new tblPet[] { pet1 });

            //Act
            var task = _service.AddRange(entities);

            //Assert
            _mockPetRepository.Verify(x => x.FindAsync(It.IsAny<Expression<Func<tblPet, bool>>>()), Times.Once());
            Assert.ThrowsAsync<InvalidOperationException>(() => task);
            _mockPetRepository.Verify(x => x.AddRange(entities), Times.Never());
            Assert.False(task.IsCompletedSuccessfully);
        }
        #endregion

        #region Find tests

        [Fact]
        public void Find_ShouldSucceed()
        {
            //Arrange
            Guid guid = Guid.Parse("c3b922ac-a2bf-4b75-a9dc-5b3c44798264");
            _mockPetRepository.Setup(x => x.FindAsync(It.IsAny<Expression<Func<tblPet, bool>>>()))
                .ReturnsAsync(Enumerable.Empty<tblPet>());

            //Act
            var task = _service.FindAsync(x => x.Name == "Spot");

            //Assert
            _mockPetRepository.Verify(x => x.FindAsync(It.IsAny<Expression<Func<tblPet, bool>>>()), Times.Once());
            Assert.True(task.IsCompletedSuccessfully);
        }
        #endregion

        #region Remove tests
        [Theory]
        [InlineData("09ae6e94-3a0d-416c-b28b-7b1e0a710493")]
        public void Remove_ShouldSucceed_WithMatchingId(string petId)
        {
            //Arrange
            Guid petGuid = Guid.Parse(petId);
            tblPet existingPet = new tblPet()
            {
                Id = petGuid,
                Name = "bill",
                FKAnimalTypeId = 0
            };

            _mockPetRepository.Setup(x => x.GetByIdAsync(petGuid))
                .ReturnsAsync(existingPet);

            //Act
            var result = _service.Remove(petGuid);

            //Assert
            _mockPetRepository.Verify(x => x.Remove(existingPet), Times.Once());
            Assert.True(result.IsCompletedSuccessfully);
        }

        [Theory]
        [InlineData("09ae6e94-3a0d-416c-b28b-7b1e0a710493", "Pete", 0)]
        public void Remove_ShouldFail_IfPetDoesntExists(string petId, string name, int fkAnimalTypeId)
        {
            //Arrange
            Guid wrongGuid = Guid.Parse("c3b922ac-a2bf-4b75-a9dc-5b3c44798264");
            Guid guid = Guid.Parse(petId);
            tblPet fakePet = new tblPet { Id = guid, Name = name, FKAnimalTypeId = fkAnimalTypeId };
            _mockPetRepository.Setup(x => x.GetByIdAsync(guid))
                .ReturnsAsync((tblPet)null);

            //Act
            var task = _service.Remove(guid);

            //Assert
            Assert.ThrowsAnyAsync<InvalidOperationException>(() => task);
            _mockPetRepository.Verify(x => x.Remove(It.IsAny<tblPet>()), Times.Never());
        }

        #endregion

        #region RemoveRange tests

        [Theory]
        [InlineData("09ae6e94-3a0d-416c-b28b-7b1e0a710493", "Pete", 0, "09d199e8-fd8c-4bfb-8359-725d80f3f421", "Clark", 1)]
        public void RemoveRange_ShouldSucceed_WithMatchingIds(string petId1, string name1, int fkAnimalTypeId1, string petId2, string name2, int fkAnimalTypeId2)
        {
            //Arrange
            Guid petGuid1 = Guid.Parse(petId1);
            Guid petGuid2 = Guid.Parse(petId2);

            tblPet pet1 = new tblPet() { Id = petGuid1, Name = name1, FKAnimalTypeId = fkAnimalTypeId1 };
            tblPet pet2 = new tblPet() { Id = petGuid2, Name = name2, FKAnimalTypeId = fkAnimalTypeId2 };
            var entities = new tblPet[] { pet1, pet2 };

            _mockPetRepository.Setup(x => x.FindAsync(It.IsAny<Expression<Func<tblPet, bool>>>()))
                .ReturnsAsync(entities);

            //Act
            var result = _service.RemoveRange(entities.Select(x => x.Id));

            //Assert
            _mockPetRepository.Verify(x => x.FindAsync(It.IsAny<Expression<Func<tblPet, bool>>>()), Times.Once());
            _mockPetRepository.Verify(x => x.RemoveRange(entities), Times.Once());
            Assert.True(result.IsCompletedSuccessfully);
        }

        [Theory]
        [InlineData("09ae6e94-3a0d-416c-b28b-7b1e0a710493", "Pete", 1, "09d199e8-fd8c-4bfb-8359-725d80f3f421", "Clark", 2)]
        public void RemoveRange_ShouldFail_WhenRemovingAnyNonExistingEntities(string petId1, string firstName1, int fkAnimalTypeId1, string petId2, string firstName2, int fkAnimalTypeId2)
        {
            //Arrange
            Guid wrongGuid = Guid.Parse("c3b922ac-a2bf-4b75-a9dc-5b3c44798264");
            Guid petGuid1 = Guid.Parse(petId1);
            Guid petGuid2 = Guid.Parse(petId2);
            tblPet pet1 = new tblPet() { Id = petGuid1, Name = firstName1, FKAnimalTypeId = fkAnimalTypeId1 };
            tblPet pet2 = new tblPet() { Id = petGuid2, Name = firstName2, FKAnimalTypeId = fkAnimalTypeId2 };
            var entities = new tblPet[] { pet1, pet2 };

            _mockPetRepository.Setup(x => x.FindAsync(It.IsAny<Expression<Func<tblPet, bool>>>()))
                .ReturnsAsync(Enumerable.Empty<tblPet>());

            //Act
            var task = _service.RemoveRange(entities.Select(x => x.Id));

            //Assert
            _mockPetRepository.Verify(x => x.FindAsync(It.IsAny<Expression<Func<tblPet, bool>>>()), Times.Once());
            Assert.ThrowsAsync<InvalidOperationException>(() => task);
            _mockPetRepository.Verify(x => x.RemoveRange(entities), Times.Never());
            Assert.False(task.IsCompletedSuccessfully);
        }

        [Theory]
        [InlineData("09ae6e94-3a0d-416c-b28b-7b1e0a710493", "Pete", 1, "09d199e8-fd8c-4bfb-8359-725d80f3f421", "Clark", 2)]
        public void RemoveRange_ShouldFail_WhenRemovingAllNonExistingEntities(string petId1, string name1, int fkAnimalTypeId1, string petId2, string name2, int fkAnimalTypeId2)
        {
            //Arrange
            Guid wrongGuid = Guid.Parse("c3b922ac-a2bf-4b75-a9dc-5b3c44798264");
            Guid petGuid1 = Guid.Parse(petId1);
            Guid petGuid2 = Guid.Parse(petId2);
            tblPet pet1 = new tblPet() { Id = petGuid1, Name = name1, FKAnimalTypeId = fkAnimalTypeId1 };
            tblPet pet2 = new tblPet() { Id = petGuid2, Name = name2, FKAnimalTypeId = fkAnimalTypeId2 };
            var entities = new tblPet[] { pet1, pet2 };

            _mockPetRepository.Setup(x => x.FindAsync(It.IsAny<Expression<Func<tblPet, bool>>>()))
                .ReturnsAsync(new tblPet[] { pet1 });

            //Act
            var task = _service.RemoveRange(entities.Select(x => x.Id));

            //Assert
            _mockPetRepository.Verify(x => x.FindAsync(It.IsAny<Expression<Func<tblPet, bool>>>()), Times.Once());
            Assert.ThrowsAsync<InvalidOperationException>(() => task);
            _mockPetRepository.Verify(x => x.RemoveRange(entities), Times.Never());
            Assert.False(task.IsCompletedSuccessfully);
        }

        #endregion
    }
}
