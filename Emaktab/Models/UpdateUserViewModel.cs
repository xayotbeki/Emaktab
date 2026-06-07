namespace Emaktab.Models
{
    public class UpdateUserViewModel
    {
        public int Id { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? UserFio { get; set; }
        public string? UserEmail { get; set; }
        public string? UserLogin { get; set; }
        public IFormFile? Icon { get; set; }
        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }
    }
}