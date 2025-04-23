using LBManage.Data;
using LBManage.Models;
using LBManage.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LBManage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuggestedBookController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public SuggestedBookController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: api/SuggestedBook
        [HttpGet]
        public async Task<IActionResult> GetAllSuggestions()
        {
            var suggestions = await _context.SuggestedBooks.Include(s => s.Student).ToListAsync();
            return Ok(suggestions);
        }

        // GET: api/SuggestedBook/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSuggestionById(int id)
        {
            var suggestion = await _context.SuggestedBooks.Include(s => s.Student).FirstOrDefaultAsync(s => s.SuggestionID == id);
            if (suggestion == null) return NotFound("Suggestion not found");
            return Ok(suggestion);
        }

        // POST: api/SuggestedBook
        [HttpPost]
        public async Task<IActionResult> AddSuggestion([FromBody] SuggestedBookDTO suggestionDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if Student exists
            var studentExists = await _context.Students.AnyAsync(s => s.StudentID == suggestionDto.StudentID);
            if (!studentExists)
                return NotFound("Student not found");

            // Convert DTO to Model
            var suggestion = new SuggestedBook
            {
                BookTitle = suggestionDto.BookTitle,
                Author = suggestionDto.Author,
                Course = suggestionDto.Course,
                Reason = suggestionDto.Reason,
                StudentID = suggestionDto.StudentID,
                SuggestedDate = DateTime.Now
            };

            _context.SuggestedBooks.Add(suggestion);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetSuggestionById), new { id = suggestion.SuggestionID }, suggestion);
        }


        // DELETE: api/SuggestedBook/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSuggestion(int id)
        {
            var suggestion = await _context.SuggestedBooks.FindAsync(id);
            if (suggestion == null) return NotFound("Suggestion not found");

            _context.SuggestedBooks.Remove(suggestion);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
