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

namespace PetRego.Controllers
{
    //TODO: Add versioning!
    [ApiController]
    [Route("api/[controller]")]
    public class OwnersController : ControllerBase
    {
        //private readonly PetRegoDbContext _context;
        private readonly IOwnerService _ownerService;

        public OwnersController(/*PetRegoDbContext context,*/ IOwnerService ownerService)
        {
            //_context = context;
            _ownerService = ownerService;
        }

        // GET: api/Owners
        [HttpGet]
        public async Task<ActionResult<IEnumerable<tblOwner>>> GetOwners()
        {
            var owners = await _ownerService.GetAllAsync().ConfigureAwait(false);
            return Ok(owners);
            //return await _context.Owners.ToListAsync();
        }

        // GET: api/Owners/a8eab20c-55bd-4526-a162-2ff8959b8862
        [HttpGet("{id}")]
        public async Task<ActionResult<tblOwner>> GetOwner(Guid id)
        {
            //var owner = await _context.Owners.FindAsync(id);
            var owner = await _ownerService.GetByIdAsync(id).ConfigureAwait(false);

            if (owner == null)
            {
                return NotFound();
            }

            return Ok(owner);
        }

        // GET: api/Owners/a8eab20c-55bd-4526-a162-2ff8959b8862/Pets
        [HttpGet("{id}/pets")]
        public async Task<ActionResult<tblPet>> GetPetsByOwnerId(Guid id)
        {
            var pets = await _ownerService.GetPetsAsync(id).ConfigureAwait(false);

            return Ok(pets);
        }

        // PUT: api/Owners/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOwner(Guid id, tblOwner owner)
        {
            if (owner == null || id != owner.Id)
            {
                return BadRequest();
            }

            //_context.Entry(owner).State = EntityState.Modified;

            try
            {
                //await _context.SaveChangesAsync();
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
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<tblOwner>> PostOwner(tblOwner owner)
        {
            //_context.Owners.Add(owner);
            //await _context.SaveChangesAsync();

            return CreatedAtAction("GettblOwner", new { id = owner.Id }, owner);
        }

        // DELETE: api/Owners/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<tblOwner>> DeleteOwner(Guid id)
        {
            //var owner = await _ownerService.GetByIdAsync(id).ConfigureAwait(false);
            //if (owner == null)
            //{
            //    return NotFound();
            //}

            _ownerService.Remove(id);
            //await _context.SaveChangesAsync();

            //return owner;
            return Ok();
        }

        private bool OwnerExists(Guid id)
        {
            return _ownerService.GetByIdAsync(id) != null;
        }
    }
}
