using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LBManage.Models
{
    public class Book
    {
        [Key]
        public int BookID { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Author { get; set; }

        [Required]
        public string ISBN { get; set; }

        public string Publisher { get; set; }

        public string Edition { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public int TotalCopies { get; set; }

        [Required]
        public int AvailableCopies { get; set; }

        public string ShelfLocation { get; set; }

        [ForeignKey("Librarian")]
        public int AddedByLibrarianID { get; set; }
        public Librarian AddedByLibrarian { get; set; }

        public DateTime AddedDate { get; set; } = DateTime.Now;

        public string? PdfFileName { get; set; }  // Uploaded file name
        public string? PdfLink { get; set; }      // External PDF link

    }
}
