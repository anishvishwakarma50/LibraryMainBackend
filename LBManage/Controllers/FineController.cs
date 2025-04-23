using LBManage.Data;
using LBManage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LBManage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FineController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public FineController(LibraryDbContext context)
        {
            _context = context;
        }

        // ✅ Get all fines
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Fine>>> GetFines()
        {
            return await _context.Fines
                .Include(f => f.Student)
                .Include(f => f.IssuedBook)
                .ToListAsync();
        }

        // ✅ Get fine by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Fine>> GetFine(int id)
        {
            var fine = await _context.Fines
                .Include(f => f.Student)
                .Include(f => f.IssuedBook)
                .FirstOrDefaultAsync(f => f.FineID == id);

            if (fine == null)
            {
                return NotFound();
            }

            return fine;
        }

        // ✅ Create a new fine
        [HttpPost]
        public async Task<ActionResult<Fine>> CreateFine(Fine fine)
        {
            _context.Fines.Add(fine);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFine), new { id = fine.FineID }, fine);
        }

        // ✅ Update a fine
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFine(int id, Fine fine)
        {
            if (id != fine.FineID)
            {
                return BadRequest();
            }

            _context.Entry(fine).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Fines.Any(f => f.FineID == id))
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

        // ✅ Delete a fine
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFine(int id)
        {
            var fine = await _context.Fines.FindAsync(id);
            if (fine == null)
            {
                return NotFound();
            }

            _context.Fines.Remove(fine);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ✅ Mark fine as paid
        [HttpPut("pay/{id}")]
        public async Task<IActionResult> PayFine(int id)
        {
            var fine = await _context.Fines.FindAsync(id);
            if (fine == null)
            {
                return NotFound();
            }

            if (fine.PaidStatus)
            {
                return BadRequest("Fine is already paid.");
            }

            fine.PaidStatus = true;
            fine.PaidDate = DateTime.Now;

            _context.Entry(fine).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
