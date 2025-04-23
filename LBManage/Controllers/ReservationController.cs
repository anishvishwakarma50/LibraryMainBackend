using LBManage.Data;
using LBManage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LBManage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public ReservationController(LibraryDbContext context)
        {
            _context = context;
        }

        // ✅ Get all reservations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetReservations()
        {
            return await _context.Reservations
                .Include(r => r.Book)
                .Include(r => r.Student)
                .ToListAsync();
        }

        // ✅ Get a specific reservation by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Reservation>> GetReservation(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Book)
                .Include(r => r.Student)
                .FirstOrDefaultAsync(r => r.ReservationID == id);

            if (reservation == null)
            {
                return NotFound();
            }

            return reservation;
        }

        // ✅ Create a new reservation
        [HttpPost]
        public async Task<ActionResult<Reservation>> CreateReservation(Reservation reservation)
        {
            reservation.RequestDate = DateTime.Now;
            reservation.Status = "Pending";

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReservation), new { id = reservation.ReservationID }, reservation);
        }

        // ✅ Approve a reservation (update status)
        [HttpPut("{id}/approve")]
        public async Task<IActionResult> ApproveReservation(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            reservation.Status = "Approved";
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ✅ Reject a reservation (update status)
        [HttpPut("{id}/reject")]
        public async Task<IActionResult> RejectReservation(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            reservation.Status = "Rejected";
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ✅ Delete a reservation
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ✅ Get all reservations of a student
        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetReservationsByStudent(int studentId)
        {
            var reservations = await _context.Reservations
                .Where(r => r.StudentID == studentId)
                .Include(r => r.Book)
                .ToListAsync();

            if (!reservations.Any())
            {
                return NotFound("No reservations found for this student.");
            }

            return reservations;
        }
    }
}
