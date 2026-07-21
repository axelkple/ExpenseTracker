using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Dtos;

public class UpdateBudgetDto
{
    [Required, Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    public DateTime? EndDate { get; set; }
}