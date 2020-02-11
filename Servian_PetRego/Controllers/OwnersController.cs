using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetRego.DAL;
using PetRego.DAL.DataModels;

namespace PetRego.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OwnersController : ControllerBase
    {
        private readonly PetRegoDbContext _context;

        public OwnersController(PetRegoDbContext context)
        {
            _context = context;
        }

        // GET: api/Owners
        [HttpGet]
        public async Task<ActionResult<IEnumerable<tblOwner>>> GetOwners()
        {
            return await _context.Owners.ToListAsync();
        }

        // GET: api/Owners/5
        [HttpGet("{id}")]
        public async Task<ActionResult<tblOwner>> GettblOwner(Guid id)
        {
            var tblOwner = await _context.Owners.FindAsync(id);

            if (tblOwner == null)
            {
                return NotFound();
            }

            return tblOwner;
        }

        // PUT: api/Owners/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PuttblOwner(Guid id, tblOwner tblOwner)
        {
            if (id != tblOwner.Id)
            {
                return BadRequest();
            }

            _context.Entry(tblOwner).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!tblOwnerExists(id))
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
        public async Task<ActionResult<tblOwner>> PosttblOwner(tblOwner tblOwner)
        {
            _context.Owners.Add(tblOwner);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GettblOwner", new { id = tblOwner.Id }, tblOwner);
        }

        // DELETE: api/Owners/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<tblOwner>> DeletetblOwner(Guid id)
        {
            var tblOwner = await _context.Owners.FindAsync(id);
            if (tblOwner == null)
            {
                return NotFound();
            }

            _context.Owners.Remove(tblOwner);
            await _context.SaveChangesAsync();

            return tblOwner;
        }

        private bool tblOwnerExists(Guid id)
        {
            return _context.Owners.Any(e => e.Id == id);
        }
    }
}
