using LBManage.Data;
using LBManage.DTOs;
using LBManage.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LBManage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public BookController(LibraryDbContext context)
        {
            _context = context;
        }

        // ✅ Get all books (returns only necessary data)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDTO>>> GetBooks()
        {
            var books = await _context.Books
                .Select(b => new BookDTO
                {
                    BookID = b.BookID,
                    Title = b.Title,
                    Author = b.Author,
                    ISBN = b.ISBN,
                    Publisher = b.Publisher,
                    Edition = b.Edition,
                    Category = b.Category,
                    TotalCopies = b.TotalCopies,
                    AvailableCopies = b.AvailableCopies,
                    ShelfLocation = b.ShelfLocation,
                    AddedByLibrarianID = b.AddedByLibrarianID, // ✅ Only ID, not full object
                    AddedDate = b.AddedDate,
                    PdfLink = b.PdfLink,
                    PdfFileName = b.PdfFileName
                })
                .ToListAsync();

            return books;
        }

        // ✅ Get book by ID
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDTO>> GetBook(int id)
        {
            var book = await _context.Books
                .Where(b => b.BookID == id)
                .Select(b => new BookDTO
                {
                    BookID = b.BookID,
                    Title = b.Title,
                    Author = b.Author,
                    ISBN = b.ISBN,
                    Publisher = b.Publisher,
                    Edition = b.Edition,
                    Category = b.Category,
                    TotalCopies = b.TotalCopies,
                    AvailableCopies = b.AvailableCopies,
                    ShelfLocation = b.ShelfLocation,
                    AddedByLibrarianID = b.AddedByLibrarianID,
                    AddedDate = b.AddedDate,
                    PdfLink = b.PdfLink,
                    PdfFileName = b.PdfFileName
                })
                .FirstOrDefaultAsync();

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Book>> CreateBook([FromForm] BookDTO bookDto) // NOTE: [FromForm] required for file
        {
            var book = new Book
            {
                Title = bookDto.Title,
                Author = bookDto.Author,
                ISBN = bookDto.ISBN,
                Publisher = bookDto.Publisher,
                Edition = bookDto.Edition,
                Category = bookDto.Category,
                TotalCopies = bookDto.TotalCopies,
                AvailableCopies = bookDto.AvailableCopies,
                ShelfLocation = bookDto.ShelfLocation,
                AddedByLibrarianID = bookDto.AddedByLibrarianID,
                AddedDate = bookDto.AddedDate,
                PdfLink = bookDto.PdfLink  // 🔗 save the link directly if available
            };

            // 📁 Save PDF File if uploaded
            if (bookDto.PdfFile != null && bookDto.PdfFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(bookDto.PdfFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await bookDto.PdfFile.CopyToAsync(stream);
                }

                book.PdfFileName = uniqueFileName; // store the filename in DB
            }

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBook), new { id = book.BookID }, book);
        }


        // ✅ Update a book
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, BookDTO bookDto)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            book.Title = bookDto.Title;
            book.Author = bookDto.Author;
            book.ISBN = bookDto.ISBN;
            book.Publisher = bookDto.Publisher;
            book.Edition = bookDto.Edition;
            book.Category = bookDto.Category;
            book.TotalCopies = bookDto.TotalCopies;
            book.AvailableCopies = bookDto.AvailableCopies;
            book.ShelfLocation = bookDto.ShelfLocation;
            book.AddedByLibrarianID = bookDto.AddedByLibrarianID;
            book.AddedDate = bookDto.AddedDate;

            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Books.Any(b => b.BookID == id))
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

        // ✅ Delete a book
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ✅ Get books by category
        [Authorize]
        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<BookDTO>>> GetBooksByCategory(string category)
        {
            var books = await _context.Books
                .Where(b => b.Category == category)
                .Select(b => new BookDTO
                {
                    BookID = b.BookID,
                    Title = b.Title,
                    Author = b.Author,
                    ISBN = b.ISBN,
                    Publisher = b.Publisher,
                    Edition = b.Edition,
                    Category = b.Category,
                    TotalCopies = b.TotalCopies,
                    AvailableCopies = b.AvailableCopies,
                    ShelfLocation = b.ShelfLocation,
                    AddedByLibrarianID = b.AddedByLibrarianID,
                    AddedDate = b.AddedDate
                })
                .ToListAsync();

            if (!books.Any())
            {
                return NotFound("No books found in this category.");
            }

            return books;
        }
    }
}
