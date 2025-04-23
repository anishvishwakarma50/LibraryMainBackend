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
    public class IssuedBookController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public IssuedBookController(LibraryDbContext context)
        {
            _context = context;
        }

        // ✅ Get all issued books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<IssuedBook>>> GetIssuedBooks()
        {
            return await _context.IssuedBooks
                .Include(ib => ib.Book)
                .Include(ib => ib.Student)
                .Include(ib => ib.IssuedByLibrarian)
                .ToListAsync();
        }

        // ✅ Get issued book by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<IssuedBook>> GetIssuedBook(int id)
        {
            var issuedBook = await _context.IssuedBooks
                .Include(ib => ib.Book)
                .Include(ib => ib.Student)
                .Include(ib => ib.IssuedByLibrarian)
                .FirstOrDefaultAsync(ib => ib.IssueID == id);

            if (issuedBook == null)
            {
                return NotFound();
            }

            return issuedBook;
        }

        // ✅ Issue a new book
        [HttpPost]
        public async Task<ActionResult<IssuedBook>> IssueBook(IssuedBook issuedBook)
        {
            issuedBook.IssuedDate = DateTime.Now;
            issuedBook.DueDate = DateTime.Now.AddDays(14);

            var book = await _context.Books.FindAsync(issuedBook.BookID);
            var student = await _context.Students.FindAsync(issuedBook.StudentID);

            if (book == null || student == null)
            {
                return BadRequest("Invalid BookID or StudentID.");
            }

            if (book.AvailableCopies <= 0)
            {
                return BadRequest("Book not available.");
            }

            // ✅ Navigation properties NULL rakho
            issuedBook.Book = null;
            issuedBook.Student = null;
            issuedBook.IssuedByLibrarian = null;

            _context.IssuedBooks.Add(issuedBook);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetIssuedBook), new { id = issuedBook.IssueID }, issuedBook);
        }




        // ✅ Return a book (update return date and check fine)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateIssuedBook(int id, IssuedBook updatedIssuedBook)
        {
            if (id != updatedIssuedBook.IssueID)
            {
                return BadRequest("IssuedBook ID mismatch.");
            }

            var existingIssuedBook = await _context.IssuedBooks.FindAsync(id);
            if (existingIssuedBook == null)
            {
                return NotFound("Issued book record not found.");
            }

            // ✅ Update only the necessary fields
            existingIssuedBook.DueDate = updatedIssuedBook.DueDate;
            existingIssuedBook.ReturnDate = updatedIssuedBook.ReturnDate;
            existingIssuedBook.FineAmount = updatedIssuedBook.FineAmount;

            _context.Entry(existingIssuedBook).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "An error occurred while updating the issued book.");
            }

            return NoContent(); // ✅ Successfully updated
        }


        // ✅ Delete an issued book record
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIssuedBook(int id)
        {
            var issuedBook = await _context.IssuedBooks.FindAsync(id);
            if (issuedBook == null)
            {
                return NotFound();
            }

            _context.IssuedBooks.Remove(issuedBook);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ✅ Get issued books of a student
        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<IEnumerable<IssuedBook>>> GetIssuedBooksByStudent(int studentId)
        {
            var issuedBooks = await _context.IssuedBooks
                .Where(ib => ib.StudentID == studentId)
                .Include(ib => ib.Book)
                .ToListAsync();

            if (!issuedBooks.Any())
            {
                return NotFound("No issued books for this student.");
            }

            return issuedBooks;
        }
    }
}
