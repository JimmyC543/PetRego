using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetRego.BLL;
using PetRego.DAL;
using PetRego.DAL.DataModels;
using PetRego.Models;
using PetRego.Utilities;

namespace PetRego.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PetsController : ControllerBase
    {
        private readonly IPetService _petService;

        public PetsController(IPetService service)
        {
            _petService = service;
        }

        // GET: api/Pets
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<ActionResult<IEnumerable<PetVM>>> GetPets()
        {
            var entities = await _petService.GetAllAsync();
            return Ok(entities.Select(x => new PetVM
            {
                PetId = x.Id,
                Name = x.Name,
                AnimalType = x.AnimalType.AnimalType,
                AnimalTypeId = x.FKAnimalTypeId,
                OwnerId = x.FKOwnerId,
                OwnersName = x.Owner?.FullName
            }));
        }
        [HttpGet]
        [ApiVersion("2.0")] //Requires header "X-Version" to be 2.0 in the request.
        public async Task<ActionResult<IEnumerable<PetVM_v2_0>>> GetPets_v2_0()
        {
            var entities = await _petService.GetAllAsync();
            return Ok(entities.Select(x => new PetVM_v2_0 
            {
                PetId = x.Id,
                Name = x.Name,
                AnimalType = x.AnimalType.AnimalType,
                AnimalTypeId = x.FKAnimalTypeId,
                FoodSource = x.AnimalType.FoodSource,
                OwnerId = x.FKOwnerId,
                OwnersName = x.Owner?.FullName
            }));
        }

        // GET: api/Pets/60eca81b-6d1d-4d10-feec-08d7af75b053
        [HttpGet("{id}")]
        [ApiVersion("1.0")]
        public async Task<ActionResult<PetVM>> GetPet(Guid id)
        {
            var entity = await _petService.GetByIdAsync(id);

            if (entity == null)
            {
                return NotFound();
            }

            return new PetVM
            {
                PetId = entity.Id,
                Name = entity.Name,
                AnimalType = entity.AnimalType.AnimalType,
                AnimalTypeId = entity.FKAnimalTypeId,
                OwnerId = entity.FKOwnerId,
                OwnersName = entity.Owner?.FullName
            };
        }

        // GET: api/Pets/60eca81b-6d1d-4d10-feec-08d7af75b053
        [HttpGet("{id}")]
        [ApiVersion("2.0")] //Requires header "X-Version" to be 2.0 in the request.
        public async Task<ActionResult<PetVM_v2_0>> GetPet_v2_0(Guid id)
        {
            var entity = await _petService.GetByIdAsync(id);

            if (entity == null)
            {
                return NotFound();
            }

            return new PetVM_v2_0
            {
                PetId = entity.Id,
                Name = entity.Name,
                AnimalTypeId = entity.FKAnimalTypeId,
                OwnerId = entity.FKOwnerId,
                FoodSource = entity.AnimalType.FoodSource,
                OwnersName = entity.Owner?.FullName
            };
        }

        // GET: api/Pets/60eca81b-6d1d-4d10-feec-08d7af75b053/Food
        [HttpGet("{id}/food")]
        [ApiVersion("2.0")] //Requires header "X-Version" to be "2.0" in the request.
        public async Task<ActionResult<PetFoodVM>> GetSuitablePetFood(Guid id)
        {
            var entity = await _petService.GetByIdAsync(id);//.Pets.FindAsync(id);

            if (entity == null)
            {
                return NotFound();
            }

            return new PetFoodVM
            {
                PetId = entity.Id,
                FoodSource = entity.AnimalType.FoodSource
            };
        }

        // PUT: api/Pets/60eca81b-6d1d-4d10-feec-08d7af75b053
        [HttpPut("{id}")]
        [ApiVersion("1.0")]
        public async Task<IActionResult> PutPet(Guid id, PetCoreVM petModel)
        {
            //Validate model
            if (petModel == null)
            {
                return BadRequest("No data provided.");
            }
            if (id != petModel.PetId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            try
            {
                await _petService.Update(new tblPet
                {
                    Id = id,
                    Name = petModel.Name,
                    FKAnimalTypeId = petModel.AnimalTypeId.Value,
                    FKOwnerId = petModel.OwnerId
                });
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!(await PetExists(id)))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Pets
        [HttpPost]
        [ApiVersion("1.0")]
        public async Task<ActionResult<PetVM>> PostPet(PetCoreVM petModel)
        {
            //Validate model
            if (petModel == null)
            {
                return BadRequest("No data provided.");
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var createdPet = await _petService
                .Add(new tblPet
                {
                    Name = petModel.Name,
                    FKAnimalTypeId = petModel.AnimalTypeId.Value,
                    FKOwnerId = petModel.OwnerId
                });

            var result = new PetVM
            {
                PetId = createdPet.Id,
                Name = createdPet.Name,
                AnimalTypeId = createdPet.FKAnimalTypeId,
                AnimalType = createdPet.AnimalType.AnimalType,
                OwnerId = createdPet.FKOwnerId,
                OwnersName = createdPet.Owner?.FullName
            };
            return Ok(result);
        }

        // DELETE: api/Pets/60eca81b-6d1d-4d10-feec-08d7af75b053
        [HttpDelete("{id}")]
        [ApiVersion("1.0")]
        public async Task<ActionResult> DeletePet(Guid id)
        {
            try
            {
                var result = await _petService.Remove(id);
                return Ok(result);
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Problem($"Unable to delete pet with id: {id}.");
            }
            catch (Exception)
            {
                return Problem($"A problem occurred while processing your request.");
            }
        }

        private async Task<bool> PetExists(Guid id)
        {
            return (await _petService.FindAsync(e => e.Id == id)).Any();
        }
    }
}
