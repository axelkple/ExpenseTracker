using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Dtos;

public class UpdateCategoryDto
{
    [Required, MaxLength(100)]
    public string Name { get; set; }

    [MaxLength(50)]
    public string Icon { get; set; }

    [MaxLength(7)]
    public string Color { get; set; }

    public int? ParentCategoryId { get; set; }
}