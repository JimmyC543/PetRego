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
    public class OwnerServiceTests
    {

        private readonly IOwnerService _service;
        private readonly Mock<IOwnerRepository> _mockOwnerRepository;
        private readonly Mock<IPetRepository> _mockPetRepository;
        public OwnerServiceTests()
        {
            _mockOwnerRepository = new Mock<IOwnerRepository>();
            _mockPetRepository = new Mock<IPetRepository>();
            _service = new OwnerService(_mockOwnerRepository.Object, _mockPetRepository.Object);
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
            _mockOwnerRepository.Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<tblOwner> { new tblOwner { Id = Guid.NewGuid(), FirstName = "Pete", LastName = "Smith" } });

            Mock<IPetRepository> mockPetRepository = new Mock<IPetRepository>();

            //Act
            var result = _service.GetAllAsync().Result;

            //Assert
            _mockOwnerRepository.Verify(x => x.GetAllAsync(), Times.Once());
            Assert.Equal(1, result.Count());
            Assert.Contains("Pete", result.First().FirstName);
            Assert.Contains("Smith", result.First().LastName);
        }
        #endregion

        #region GetOwnerById tests

        [Theory]
        [InlineData("09ae6e94-3a0d-416c-b28b-7b1e0a710493")]
        public void GetOwnerById_ShouldSucceedWithValidId(string id)
        {
            //Arrange
            Guid guid = Guid.Parse(id);
            _mockOwnerRepository.Setup(x => x.GetByIdAsync(guid))
                .ReturnsAsync(new tblOwner { Id = guid, FirstName = "Pete", LastName = "Smith" });

            //Act
            var result = _service.GetByIdAsync(guid).Result;

            //Assert
            _mockOwnerRepository.Verify(x => x.GetByIdAsync(guid), Times.Once());
            Assert.Equal(guid, result.Id);
            Assert.Equal("Pete", result.FirstName);
            Assert.Equal("Smith", result.LastName);
        }

        [Theory]
        [InlineData("09ae6e94-3a0d-416c-b28b-7b1e0a710493")]
        public void GetOwnerById_ShouldReturnNullIfIdDoesntMatchs(string id)
        {
            //Arrange
            Guid wrongGuid = Guid.Parse("c3b922ac-a2bf-4b75-a9dc-5b3c44798264");
            Guid guid = Guid.Parse(id);
            _mockOwnerRepository.Setup(x => x.GetByIdAsync(guid))
                .ReturnsAsync(new tblOwner { Id = guid, FirstName = "Pete", LastName = "Smith" });

            //Act
            var result = _service.GetByIdAsync(wrongGuid).Result;

            //Assert
            _mockOwnerRepository.Verify(x => x.GetByIdAsync(wrongGuid), Times.Once());
            Assert.Null(result);
        }
        #endregion
        
        #region GetPetsByOwnerId tests

        [Theory]
        [InlineData("09ae6e94-3a0d-416c-b28b-7b1e0a710493", "09d199e8-fd8c-4bfb-8359-725d80f3f421", "Mittens", 1)]
        public void GetPetsByOwnerId_ShouldSucceedWithValidId(string ownerId, string petId, string petName, int animalTypeId)
        {
            //Arrange
            Guid ownerGuid = Guid.Parse(ownerId);
            Guid petGuid = Guid.Parse(petId);
            _mockOwnerRepository.Setup(x => x.GetPetsByOwnerIdAsync(ownerGuid))
                .ReturnsAsync(new List<tblPet> { new tblPet { Id = petGuid, Name = petName, FKAnimalTypeId = animalTypeId, FKOwnerId = ownerGuid } });

            //Act
            var result = _service.GetPetsAsync(ownerGuid).Result;

            //Assert
            _mockOwnerRepository.Verify(x => x.GetPetsByOwnerIdAsync(ownerGuid), Times.Once());
            Assert.Single(result);
            Assert.Equal(petGuid, result.First().Id);
            Assert.Equal(petName, result.First().Name);
            Assert.Equal(animalTypeId, result.First().FKAnimalTypeId);
            Assert.Equal(ownerGuid, result.First().FKOwnerId);
        }

        [Theory]
        [InlineData("09ae6e94-3a0d-416c-b28b-7b1e0a710493", "09d199e8-fd8c-4bfb-8359-725d80f3f421", "Mittens", 1)]
        public void GetPetsByOwnerId_ShouldReturnEmptyIEnumerableWithInvalidId(string ownerId, string petId, string petName, int animalTypeId)
        {
            //Arrange
            Guid wrongGuid = Guid.Parse("c3b922ac-a2bf-4b75-a9dc-5b3c44798264");
            Guid ownerGuid = Guid.Parse(ownerId);
            Guid petGuid = Guid.Parse(petId);
            _mockOwnerRepository.Setup(x => x.GetPetsByOwnerIdAsync(ownerGuid))
                .ReturnsAsync(new List<tblPet> { new tblPet { Id = petGuid, Name = petName, FKAnimalTypeId = animalTypeId, FKOwnerId = ownerGuid } });

            //Act
            var result = _service.GetPetsAsync(wrongGuid).Result;

            //Assert
            _mockOwnerRepository.Verify(x => x.GetPetsByOwnerIdAsync(ownerGuid), Times.Never());
            Assert.Empty(result);
            Assert.IsAssignableFrom<IEnumerable<tblPet>>(result);
        }
        #endregion

        #region Add tests
        [Theory]
        [InlineData("09ae6e94-3a0d-416c-b28b-7b1e0a710493", "09d199e8-fd8c-4bfb-8359-725d80f3f421", "Pete", "Smith")]
        public void Add_ShouldSucceed_WithUnusedId(string ownerId, string petId, string firstName, string lastName)
        {
            //Arrange
            Guid wrongGuid = Guid.Parse("c3b922ac-a2bf-4b75-a9dc-5b3c44798264");
            Guid ownerGuid = Guid.Parse(ownerId);
            _mockOwnerRepository.Setup(x => x.GetByIdAsync(ownerGuid))
                .ReturnsAsync((tblOwner)null);

            tblOwner owner = new tblOwner() { Id = ownerGuid, FirstName = firstName, LastName = lastName };
            //Act
            var result = _service.Add(owner);

            //Assert
            _mockOwnerRepository.Verify(x => x.GetByIdAsync(ownerGuid), Times.Once());
            Assert.True(result.IsCompletedSuccessfully);
        }

        [Fact]
        public void Add_ShouldFail_WithNullEntity()
        {
            //Arrange
            Guid guid = Guid.Parse("c3b922ac-a2bf-4b75-a9dc-5b3c44798264");
            _mockOwnerRepository.Setup(x => x.GetByIdAsync(guid))
                .ReturnsAsync((tblOwner)null);

            //Act
            var task = _service.Add(null);

            //Assert
            Assert.ThrowsAnyAsync<ArgumentNullException>(() => task);
            _mockOwnerRepository.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never());
            _mockOwnerRepository.Verify(x => x.Add(It.IsAny<tblOwner>()), Times.Never());
            Assert.False(task.IsCompletedSuccessfully);
        }

        [Theory]
        [InlineData("09ae6e94-3a0d-416c-b28b-7b1e0a710493", "Pete", "Smith")]
        public void Add_ShouldFail_IfOwnerAlreadyExists(string ownerId, string firstName, string lastName)
        {
            //Arrange
            Guid guid = Guid.Parse(ownerId);
            tblOwner fakeOwner = new tblOwner { Id = guid, FirstName = firstName, LastName = lastName };
            _mockOwnerRepository.Setup(x => x.GetByIdAsync(guid))
                .ReturnsAsync(fakeOwner);

            //Act
            var task = _service.Add(fakeOwner);

            //Assert
            Assert.ThrowsAnyAsync<InvalidOperationException>(() => task);
            _mockOwnerRepository.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Once());
            _mockOwnerRepository.Verify(x => x.Add(It.IsAny<tblOwner>()), Times.Never());
            Assert.False(task.IsCompletedSuccessfully);
        }
        #endregion

        #region AddRange tests

        [Theory]
        [InlineData("09ae6e94-3a0d-416c-b28b-7b1e0a710493", "Pete", "Smith", "09d199e8-fd8c-4bfb-8359-725d80f3f421", "Clark", "Kent")]
        public void AddRange_ShouldSucceed_WithUnusedIds(string ownerId1, string firstName1, string lastName1, string ownerId2, string firstName2, string lastName2)
        {
            //Arrange
            Guid wrongGuid = Guid.Parse("c3b922ac-a2bf-4b75-a9dc-5b3c44798264");
            Guid ownerGuid1 = Guid.Parse(ownerId1);
            Guid ownerGuid2 = Guid.Parse(ownerId2);
            _mockOwnerRepository.Setup(x => x.FindAsync(It.IsAny<Expression<Func<tblOwner, bool>>>()))
                .ReturnsAsync(Enumerable.Empty<tblOwner>());

            tblOwner owner1 = new tblOwner() { Id = ownerGuid1, FirstName = firstName1, LastName = lastName1 };
            tblOwner owner2 = new tblOwner() { Id = ownerGuid2, FirstName = firstName2, LastName = lastName2 };
            var entities = new tblOwner[] { owner1, owner2 };

            //Act
            var result = _service.AddRange(entities);

            //Assert
            _mockOwnerRepository.Verify(x => x.FindAsync(It.IsAny<Expression<Func<tblOwner, bool>>>()), Times.Once());
            _mockOwnerRepository.Verify(x => x.AddRange(entities), Times.Once());
            Assert.True(result.IsCompletedSuccessfully);
        }

        [Theory]
        [InlineData("09ae6e94-3a0d-416c-b28b-7b1e0a710493", "Pete", "Smith", "09d199e8-fd8c-4bfb-8359-725d80f3f421", "Clark", "Kent")]
        public void AddRange_ShouldFail_WhenAddingExistingEntities(string ownerId1, string firstName1, string lastName1, string ownerId2, string firstName2, string lastName2)
        {
            //Arrange
            Guid wrongGuid = Guid.Parse("c3b922ac-a2bf-4b75-a9dc-5b3c44798264");
            Guid ownerGuid1 = Guid.Parse(ownerId1);
            Guid ownerGuid2 = Guid.Parse(ownerId2);
            tblOwner owner1 = new tblOwner() { Id = ownerGuid1, FirstName = firstName1, LastName = lastName1 };
            tblOwner owner2 = new tblOwner() { Id = ownerGuid2, FirstName = firstName2, LastName = lastName2 };
            var entities = new tblOwner[] { owner1, owner2 };

            _mockOwnerRepository.Setup(x => x.FindAsync(It.IsAny<Expression<Func<tblOwner, bool>>>()))
                .ReturnsAsync(new tblOwner[] { owner1 });

            //Act
            var task = _service.AddRange(entities);

            //Assert
            _mockOwnerRepository.Verify(x => x.FindAsync(It.IsAny<Expression<Func<tblOwner, bool>>>()), Times.Once());
            Assert.ThrowsAsync<InvalidOperationException>(() => task);
            _mockOwnerRepository.Verify(x => x.AddRange(entities), Times.Never());
            Assert.False(task.IsCompletedSuccessfully);
        }
        #endregion

        #region Find tests

        [Fact]
        public void Find_ShouldSucceed()
        {
            //Arrange
            Guid guid = Guid.Parse("c3b922ac-a2bf-4b75-a9dc-5b3c44798264");
            _mockOwnerRepository.Setup(x => x.FindAsync(It.IsAny<Expression<Func<tblOwner, bool>>>()))
                .ReturnsAsync(Enumerable.Empty<tblOwner>());

            //Act
            var task = _service.FindAsync(x => x.FirstName == "Bill");

            //Assert
            _mockOwnerRepository.Verify(x => x.FindAsync(It.IsAny<Expression<Func<tblOwner, bool>>>()), Times.Once());
            Assert.True(task.IsCompletedSuccessfully);
        }
        #endregion

        #region Remove tests
        [Theory]
        [InlineData("09ae6e94-3a0d-416c-b28b-7b1e0a710493")]
        public void Remove_ShouldSucceed_WithMatchingId(string ownerId)
        {
            //Arrange
            Guid ownerGuid = Guid.Parse(ownerId);
            tblOwner existingOwner = new tblOwner() 
            { 
                Id = ownerGuid,
                FirstName = "bill",
                LastName = "ted"
            };
            existingOwner.Pets.Add(new tblPet
            {
                Id = Guid.Parse("84ffdbe4-27ca-4c0a-b2c8-aa7fb1e7c711"),
                Name = "Clifford",
                FKOwnerId = ownerGuid,
                FKAnimalTypeId = 1
            });

            _mockOwnerRepository.Reset();
            _mockOwnerRepository.Setup(x => x.GetByIdAsync(ownerGuid))
                .ReturnsAsync(existingOwner);

            //Act
            var result = _service.Remove(ownerGuid);

            //Assert
            _mockOwnerRepository.Verify(x => x.GetByIdAsync(ownerGuid), Times.Once());
            _mockPetRepository.Verify(x => x.RemoveRange(existingOwner.Pets), Times.Once());
            _mockOwnerRepository.Verify(x => x.Remove(existingOwner), Times.Once());
            Assert.True(result.IsCompletedSuccessfully);
        }

        [Theory]
        [InlineData("09ae6e94-3a0d-416c-b28b-7b1e0a710493", "Pete", "Smith")]
        public void Remove_ShouldFail_IfOwnerDoesntExists(string ownerId, string firstName, string lastName)
        {
            //Arrange
            Guid wrongGuid = Guid.Parse("c3b922ac-a2bf-4b75-a9dc-5b3c44798264");
            Guid guid = Guid.Parse(ownerId);
            tblOwner fakeOwner = new tblOwner { Id = guid, FirstName = firstName, LastName = lastName };
            _mockOwnerRepository.Setup(x => x.GetByIdAsync(guid))
                .ReturnsAsync((tblOwner)null);

            //Act
            var task = _service.Remove(guid);

            //Assert
            Assert.ThrowsAnyAsync<InvalidOperationException>(() => task);
            _mockOwnerRepository.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Once());
            _mockPetRepository.Verify(x => x.RemoveRange(It.IsAny<IEnumerable<tblPet>>()), Times.Never());
            _mockOwnerRepository.Verify(x => x.Remove(It.IsAny<tblOwner>()), Times.Never());
            Assert.False(task.IsCompletedSuccessfully);
        }

        #endregion

        #region RemoveRange tests

        [Theory]
        [InlineData("09ae6e94-3a0d-416c-b28b-7b1e0a710493", "Pete", "Smith", "09d199e8-fd8c-4bfb-8359-725d80f3f421", "Clark", "Kent")]
        public void RemoveRange_ShouldSucceed_WithMatchingIds(string ownerId1, string firstName1, string lastName1, string ownerId2, string firstName2, string lastName2)
        {
            //Arrange
            Guid ownerGuid1 = Guid.Parse(ownerId1);
            Guid ownerGuid2 = Guid.Parse(ownerId2);

            tblOwner owner1 = new tblOwner() { Id = ownerGuid1, FirstName = firstName1, LastName = lastName1 };
            tblOwner owner2 = new tblOwner() { Id = ownerGuid2, FirstName = firstName2, LastName = lastName2 };
            var entities = new tblOwner[] { owner1, owner2 };

            _mockOwnerRepository.Setup(x => x.FindAsync(It.IsAny<Expression<Func<tblOwner, bool>>>()))
                .ReturnsAsync(entities);

            //Act
            var result = _service.RemoveRange(entities.Select(x => x.Id));

            //Assert
            _mockOwnerRepository.Verify(x => x.FindAsync(It.IsAny<Expression<Func<tblOwner, bool>>>()), Times.Once());
            _mockPetRepository.Verify(x => x.FindAsync(It.IsAny<Expression<Func<tblPet, bool>>>()), Times.Once());
            _mockPetRepository.Verify(x => x.RemoveRange(It.IsAny<IEnumerable<tblPet>>()));
            _mockOwnerRepository.Verify(x => x.RemoveRange(entities), Times.Once());
            Assert.True(result.IsCompletedSuccessfully);
        }

        [Theory]
        [InlineData("09ae6e94-3a0d-416c-b28b-7b1e0a710493", "Pete", "Smith", "09d199e8-fd8c-4bfb-8359-725d80f3f421", "Clark", "Kent")]
        public void RemoveRange_ShouldFail_WhenRemovingAnyNonExistingEntities(string ownerId1, string firstName1, string lastName1, string ownerId2, string firstName2, string lastName2)
        {
            //Arrange
            Guid wrongGuid = Guid.Parse("c3b922ac-a2bf-4b75-a9dc-5b3c44798264");
            Guid ownerGuid1 = Guid.Parse(ownerId1);
            Guid ownerGuid2 = Guid.Parse(ownerId2);
            tblOwner owner1 = new tblOwner() { Id = ownerGuid1, FirstName = firstName1, LastName = lastName1 };
            tblOwner owner2 = new tblOwner() { Id = ownerGuid2, FirstName = firstName2, LastName = lastName2 };
            var entities = new tblOwner[] { owner1, owner2 };

            _mockOwnerRepository.Setup(x => x.FindAsync(It.IsAny<Expression<Func<tblOwner, bool>>>()))
                .ReturnsAsync(Enumerable.Empty<tblOwner>());

            //Act
            var task = _service.RemoveRange(entities.Select(x => x.Id));

            //Assert
            _mockOwnerRepository.Verify(x => x.FindAsync(It.IsAny<Expression<Func<tblOwner, bool>>>()), Times.Once());
            Assert.ThrowsAsync<InvalidOperationException>(() => task);
            _mockOwnerRepository.Verify(x => x.RemoveRange(entities), Times.Never());
            Assert.False(task.IsCompletedSuccessfully);
        }

        [Theory]
        [InlineData("09ae6e94-3a0d-416c-b28b-7b1e0a710493", "Pete", "Smith", "09d199e8-fd8c-4bfb-8359-725d80f3f421", "Clark", "Kent")]
        public void RemoveRange_ShouldFail_WhenRemovingAllNonExistingEntities(string ownerId1, string firstName1, string lastName1, string ownerId2, string firstName2, string lastName2)
        {
            //Arrange
            Guid wrongGuid = Guid.Parse("c3b922ac-a2bf-4b75-a9dc-5b3c44798264");
            Guid ownerGuid1 = Guid.Parse(ownerId1);
            Guid ownerGuid2 = Guid.Parse(ownerId2);
            tblOwner owner1 = new tblOwner() { Id = ownerGuid1, FirstName = firstName1, LastName = lastName1 };
            tblOwner owner2 = new tblOwner() { Id = ownerGuid2, FirstName = firstName2, LastName = lastName2 };
            var entities = new tblOwner[] { owner1, owner2 };

            _mockOwnerRepository.Setup(x => x.FindAsync(It.IsAny<Expression<Func<tblOwner, bool>>>()))
                .ReturnsAsync(new tblOwner[] { owner1 });

            //Act
            var task = _service.RemoveRange(entities.Select(x => x.Id));

            //Assert
            _mockOwnerRepository.Verify(x => x.FindAsync(It.IsAny<Expression<Func<tblOwner, bool>>>()), Times.Once());
            Assert.ThrowsAsync<InvalidOperationException>(() => task);
            _mockOwnerRepository.Verify(x => x.RemoveRange(entities), Times.Never());
            Assert.False(task.IsCompletedSuccessfully);
        }

        #endregion
    }
}
