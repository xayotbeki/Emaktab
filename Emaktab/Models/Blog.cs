using System.ComponentModel.DataAnnotations.Schema;

namespace Emaktab.Models
{
    public class Blog
    {
        public int Id { get; set; }

        public int create_userId { get; set; }
        public User? User { get; set; }
        public LanguageType LanguageType { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Comment { get; set; }
        public DateOnly CreateDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        [NotMapped]
        public DocStatus Status { get; set; } = DocStatus.Saqlangan;
        public int StatusId { get; set; }
    }

    public enum LanguageType
    {
        Uzbek = 0,
        Russian,
        English
    }
}