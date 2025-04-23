using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LBManage.Models
{
    public class SuggestedBook
    {
        [Key]
        public int SuggestionID { get; set; }

        [Required]
        public string BookTitle { get; set; }

        [Required]
        public string Author { get; set; }

        [Required]
        public string Course { get; set; }

        [Required]
        public string Reason { get; set; }

        [Required]
        public int StudentID { get; set; }

        [ForeignKey("StudentID")]
        public Student Student { get; set; }

        public DateTime SuggestedDate { get; set; } = DateTime.Now;
    }
}
