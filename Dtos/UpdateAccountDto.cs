using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Dtos;
public class UpdateAccountDto
{
    [Required, MaxLength(100)]
    public string Name { get; set; }

    public bool IsActive { get; set; }
}