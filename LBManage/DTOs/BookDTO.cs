namespace LBManage.DTOs
{
    public class BookDTO
    {
        public int BookID { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public string Publisher { get; set; }
        public string Edition { get; set; }
        public string Category { get; set; }
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }
        public string ShelfLocation { get; set; }
        public int AddedByLibrarianID { get; set; } // ✅ Only ID, not full object
        public DateTime AddedDate { get; set; }

        public IFormFile? PdfFile { get; set; }  // For uploaded soft copy
        public string? PdfLink { get; set; }     // For online link of the book
        public string? PdfFileName { get; set; }  // ⬅️ ✅ For GET responses
    }
}
