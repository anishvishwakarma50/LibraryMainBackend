using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LBManage.Models
{
    public class Reservation
    {
        [Key]
        public int ReservationID { get; set; }

        [Required, ForeignKey("Book")]
        public int BookID { get; set; }
        public Book Book { get; set; }

        [Required, ForeignKey("Student")]
        public int StudentID { get; set; }
        public Student Student { get; set; }

        [Required]
        public DateTime RequestDate { get; set; }

        [Required]
        public string Status { get; set; } = "Pending";
    }
}
