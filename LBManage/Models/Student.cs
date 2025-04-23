using System;
using System.ComponentModel.DataAnnotations;

namespace LBManage.Models
{
    public class Student
    {
        [Key]
        public int StudentID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required, Phone]
        public string Phone { get; set; }

        public string Address { get; set; }

        [Required]
        public string EnrollmentNo { get; set; }

        [Required]
        public string Department { get; set; }

        [Required]
        public int? Year { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
