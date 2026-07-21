using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Dtos;

public class CreateCategoryDto
{
    [Required, MaxLength(100)]
    public string Name { get; set; }

    [MaxLength(50)]
    public string Icon { get; set; }

    [MaxLength(7)] // hex color
    public string Color { get; set; }

    public int? ParentCategoryId { get; set; }
}