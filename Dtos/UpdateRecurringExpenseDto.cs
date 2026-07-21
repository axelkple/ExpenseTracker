using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Dtos;

public class UpdateRecurringExpenseDto
{
    [Required, Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [MaxLength(500)]
    public string Description { get; set; }

    public DateTime? EndDate { get; set; }
}