using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Dtos;

public class CreateTagDto
{
    [Required, MaxLength(50)]
    public string Name { get; set; }
}