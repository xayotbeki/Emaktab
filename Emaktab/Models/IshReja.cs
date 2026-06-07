using System.ComponentModel.DataAnnotations;

namespace Emaktab.Models;

public class IshReja
{
    public int Id { get; set; }

    public int UserId { get; set; }

    [Required]
    public string Nomi { get; set; }

    public string? FanNomi { get; set; }

    public string? Guruh { get; set; }

    public string? Tavsif { get; set; }

    public DateTime Sana { get; set; } = DateTime.Now;

    public bool Active { get; set; } = true;
}