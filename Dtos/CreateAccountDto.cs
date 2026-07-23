using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Dtos;

public class CreateAccountDto
{

    [Required]
    public int UserId { get; set; }
    [Required, MaxLength(100)]
    public string Name { get; set; }

    [Required, MaxLength(50)]
    public string Type { get; set; }

    public decimal Balance { get; set; } = 0;

    [Required, MaxLength(3)]
    public string Currency { get; set; }

    public bool IsActive { get; set; }
}