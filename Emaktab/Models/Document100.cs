using System.ComponentModel.DataAnnotations.Schema;

namespace Emaktab.Models
{
    public class Document100
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public DateTime upload_date { get; set; }
        public DateTime create_date { get; set; }
        public string? Comment { get; set; }
        public int DocumentTypeId { get; set; }

        public int user_id { get; set; }
        [ForeignKey("user_id")]
        public User? User { get; set; }

        // Faylning yo‘li saqlanadi
        public string? FilePath { get; set; }

        public DocStatus Status { get; set; }
        public int? status_id { get; set; }

        public int Score { get; set; }

        public int CategoryId { get; set; } = 1;
        public Category? Category { get; set; }

        [NotMapped]
        public IFormFile? UploadDocument { get; set; }
    }
}