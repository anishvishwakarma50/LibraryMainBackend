using LBManage.Data;
using LBManage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace LBManage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibrarianController : ControllerBase
    {
        private readonly LibraryDbContext _context;
        private readonly PasswordHasher<string> _passwordHasher = new PasswordHasher<string>(); // FIX ✅

        public LibrarianController(LibraryDbContext context)
        {
            _context = context;
        }

        // ✅ Get all librarians
        [Authorize] // ✅ JWT token required for this endpoint
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Librarian>>> GetLibrarians()
        {
            return await _context.Librarians.ToListAsync();
        }

        // ✅ Get librarian by ID
        [HttpGet("by-email")]
        public async Task<ActionResult<Librarian>> GetLibrarian([FromQuery] string email)
        {
            var librarian = await _context.Librarians.FirstOrDefaultAsync(l => l.Email == email);

            if (librarian == null)
            {
                return NotFound(new { message = "librarian not found" });
            }

            return Ok(librarian);
        }

        // ✅ Get librarian by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Librarian>> GetLibrarian(int id)
        {
            var librarian = await _context.Librarians.FindAsync(id);
            if (librarian == null)
            {
                return NotFound();
            }
            return librarian;
        }

        // ✅ Register librarian (password hashed)
        [HttpPost("register")]
        public async Task<ActionResult<Librarian>> RegisterLibrarian([FromBody] Librarian librarian)
        {
            if (string.IsNullOrEmpty(librarian.PasswordHash))
            {
                return BadRequest("Password is required.");
            }

            // Hash password before storing ✅
            librarian.PasswordHash = _passwordHasher.HashPassword(null, librarian.PasswordHash);

            _context.Librarians.Add(librarian);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLibrarian), new { id = librarian.LibrarianID }, librarian);
        }


        // ✅ Logout
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Ok(new { Message = "Logged out successfully" });
        }

        // ✅ Check if librarian is logged in
        [HttpGet("checkLogin")]
        public IActionResult CheckLogin()
        {
            var librarianId = HttpContext.Session.GetInt32("LibrarianID");
            if (librarianId == null)
            {
                return Unauthorized(new { Message = "Not logged in" });
            }

            return Ok(new
            {
                LibrarianID = librarianId,
                LibrarianEmail = HttpContext.Session.GetString("LibrarianEmail")
            });
        }

        // ✅ Update librarian details
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLibrarian(int id, Librarian librarian)
        {
            if (id != librarian.LibrarianID)
            {
                return BadRequest();
            }

            var existingLibrarian = await _context.Librarians.FindAsync(id);
            if (existingLibrarian == null)
            {
                return NotFound();
            }

            existingLibrarian.Name = librarian.Name;
            existingLibrarian.Email = librarian.Email;
            existingLibrarian.Phone = librarian.Phone;
            existingLibrarian.Address = librarian.Address;
            existingLibrarian.HireDate = librarian.HireDate;

            // Update password if provided
            if (!string.IsNullOrEmpty(librarian.PasswordHash))
            {
                existingLibrarian.PasswordHash = _passwordHasher.HashPassword(null, librarian.PasswordHash);
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ✅ Delete librarian
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLibrarian(int id)
        {
            var librarian = await _context.Librarians.FindAsync(id);
            if (librarian == null)
            {
                return NotFound();
            }

            _context.Librarians.Remove(librarian);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    // ✅ Login request model
    public class LibrarianLoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

}