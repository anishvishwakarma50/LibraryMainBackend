using System;
using System.ComponentModel.DataAnnotations;

namespace LBManage.Models
{
    public class Librarian
    {
        [Key]
        public int LibrarianID { get; set; }

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
        public DateTime HireDate { get; set; }
    }
}
