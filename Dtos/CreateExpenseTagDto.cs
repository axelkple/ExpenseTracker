using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Dtos;

public class CreateExpenseTagDto
{
    [Required]
    public int UserId { get; set; } // I have to remove this due to authenfication
    [Required]
    public int ExpenseId { get; set; }
    [Required]
    public int TagId { get; set; }
}