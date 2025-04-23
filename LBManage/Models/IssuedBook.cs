using LBManage.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class IssuedBook
{
    [Key] // ✅ Explicitly define primary key
    public int IssueID { get; set; }

    [ForeignKey("Book")]
    public int BookID { get; set; }

    [ForeignKey("Student")]
    public int StudentID { get; set; }

    public DateTime IssuedDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public decimal? FineAmount { get; set; }

    [ForeignKey("Librarian")]
    public int? IssuedByLibrarianID { get; set; }

    // ✅ Navigation properties ignore karo
    [JsonIgnore]
    public virtual Book? Book { get; set; }

    [JsonIgnore]
    public virtual Student? Student { get; set; }

    [JsonIgnore]
    public virtual Librarian? IssuedByLibrarian { get; set; }
}
