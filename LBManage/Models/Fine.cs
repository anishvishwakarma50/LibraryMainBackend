using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LBManage.Models
{
    public class Fine
    {
        [Key]
        public int FineID { get; set; }

        [Required, ForeignKey("Student")]
        public int StudentID { get; set; }
        public Student Student { get; set; }

        [Required, ForeignKey("IssuedBook")]
        public int IssueID { get; set; }
        public IssuedBook IssuedBook { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public bool PaidStatus { get; set; } = false;

        public DateTime? PaidDate { get; set; }
    }
}
