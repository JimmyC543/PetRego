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
    //TODO: Add versioning!
    [ApiController]
    [Route("api/[controller]")]
    public class OwnersController : ControllerBase
    {
        private readonly IOwnerService _ownerService;

        public OwnersController(IOwnerService ownerService)
        {
            _ownerService = ownerService;
        }

        // GET: api/Owners
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<ActionResult<IEnumerable<OwnerVM>>> GetOwners()
        {
            var owners = await _ownerService.GetAllAsync().ConfigureAwait(false);

            //TODO: Add mapper classes to perform these sorts of conversions
            return Ok(owners.Select(o => new OwnerVM
            {
                OwnerId = o.Id,
                FirstName = o.FirstName,
                LastName = o.LastName,
                Pets = o.Pets.Select(p => new PetVM 
                { 
                    PetId = p.Id,
                    Name = p.Name,
                    AnimalTypeId = p.FKAnimalTypeId,
                    AnimalType = p.AnimalType.AnimalType,
                    OwnerId = p.FKOwnerId,
                    OwnersName = p.Owner.FullName
                })

            }));
        }
        [HttpGet]
        [ApiVersion("2.0")] //Use "X-Version": "2.0" in request header
        public async Task<ActionResult<IEnumerable<OwnerVM_v2_0>>> GetOwners_v2_0()
        {
            var owners = await _ownerService.GetAllAsync().ConfigureAwait(false);

            //TODO: Add mapper classes to perform these sorts of conversions
            return Ok(owners.Select(o => new OwnerVM_v2_0
            {
                OwnerId = o.Id,
                FirstName = o.FirstName,
                LastName = o.LastName,
                Pets = o.Pets.Select(p => new PetVM_v2_0 
                { 
                    PetId = p.Id,
                    Name = p.Name,
                    AnimalTypeId = p.FKAnimalTypeId,
                    AnimalType = p.AnimalType.AnimalType,
                    FoodSource = p.AnimalType.FoodSource,
                    OwnerId = p.FKOwnerId,
                    OwnersName = p.Owner.FullName
                })

            }));
        }

        // GET: api/Owners/a8eab20c-55bd-4526-a162-2ff8959b8862
        [HttpGet("{id}")]
        [ApiVersion("1.0")]
        public async Task<ActionResult<OwnerVM>> GetOwner(Guid id)
        {
            var owner = await _ownerService.GetByIdAsync(id).ConfigureAwait(false);

            if (owner == null)
            {
                return NotFound();
            }

            return Ok(new OwnerVM
            {
                OwnerId = owner.Id,
                FirstName = owner.FirstName,
                LastName = owner.LastName,
                Pets = owner.Pets.Select(p => new PetVM
                {
                    PetId = p.Id,
                    Name = p.Name,
                    AnimalTypeId = p.FKAnimalTypeId,
                    AnimalType = p.AnimalType.AnimalType,
                    OwnerId = p.FKOwnerId,
                    OwnersName = p.Owner.FullName
                })

            });
        }
        // GET: api/Owners/a8eab20c-55bd-4526-a162-2ff8959b8862
        [HttpGet("{id}")]
        [ApiVersion("2.0")] //Requires header "X-Version" to be "2.0" in the request.
        public async Task<ActionResult<OwnerVM_v2_0>> GetOwner_v2_0(Guid id)
        {
            var owner = await _ownerService.GetByIdAsync(id).ConfigureAwait(false);

            if (owner == null)
            {
                return NotFound();
            }

            return Ok(new OwnerVM_v2_0
            {
                OwnerId = owner.Id,
                FirstName = owner.FirstName,
                LastName = owner.LastName,
                Pets = owner.Pets.Select(p => new PetVM_v2_0
                {
                    PetId = p.Id,
                    Name = p.Name,
                    AnimalTypeId = p.FKAnimalTypeId,
                    AnimalType = p.AnimalType.AnimalType,
                    FoodSource = p.AnimalType.FoodSource,
                    OwnerId = p.FKOwnerId,
                    OwnersName = p.Owner.FullName
                })

            });
        }

        // GET: api/Owners/a8eab20c-55bd-4526-a162-2ff8959b8862/Pets
        [HttpGet("{id}/pets")]
        [ApiVersion("1.0")]
        public async Task<ActionResult<IEnumerable<PetVM>>> GetPetsByOwnerId(Guid id)
        {
            var pets = await _ownerService.GetPetsAsync(id).ConfigureAwait(false);


            return Ok(pets.Select(p => new PetVM 
            { PetId = p.Id,
                Name = p.Name,
                AnimalTypeId = p.FKAnimalTypeId,
                AnimalType = p.AnimalType.AnimalType,
                OwnerId = id,
                OwnersName = p.Owner.FullName
            }));
        }

        // PUT: api/Owners/a8eab20c-55bd-4526-a162-2ff8959b8862
        [HttpPut("{id}")]
        [ApiVersion("1.0")]
        public async Task<IActionResult> PutOwner(Guid id, OwnerVM ownerModel)
        {
            if (ownerModel == null || id != ownerModel.OwnerId)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            try
            {
                //await _context.SaveChangesAsync();
                await _ownerService.Update(new tblOwner
                {
                    Id = id,
                    FirstName = ownerModel.FirstName,
                    LastName = ownerModel.LastName
                });
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OwnerExists(id))
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

        // POST: api/Owners
        [HttpPost]
        [ApiVersion("1.0")]
        public async Task<ActionResult<tblOwner>> PostOwner([FromBody]OwnerVM owner)
        {
            //Validate model
            if (owner == null)
            {
                return BadRequest("No data provided.");
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var createdOwner = await _ownerService.Add(new tblOwner
            {
                FirstName = owner.FirstName,
                LastName = owner.LastName
            });

            owner.OwnerId = createdOwner.Id;
            return Ok(owner);
        }

        // DELETE: api/Owners/a8eab20c-55bd-4526-a162-2ff8959b8862
        [HttpDelete("{id}")]
        [ApiVersion("1.0")]
        public async Task<ActionResult<OwnerVM>> DeleteOwner(Guid id)
        {
            try
            {
                var deletedOwner = await _ownerService.Remove(id);
                return Ok(new OwnerVM
                {
                    OwnerId = deletedOwner.Id,
                    FirstName = deletedOwner.FirstName,
                    LastName = deletedOwner.LastName
                });
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException)
            {
                return Problem($"Unable to delete owner with id: {id}");
            }
            catch (Exception)
            {
                return Problem("A problem occurred while processing your request.");
            }
        }

        private bool OwnerExists(Guid id)
        {
            return _ownerService.GetByIdAsync(id) != null;
        }
    }
}
