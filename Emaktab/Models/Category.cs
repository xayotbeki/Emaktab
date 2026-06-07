namespace Emaktab.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public int? ParentCategoryId { get; set; }
        public Category? ParentCategory { get; set; }

        public ICollection<Category>? SubCategories { get; set; }

        public int MaxScore { get; set; }

        public ICollection<Document100>? Documents { get; set; }
    }
}