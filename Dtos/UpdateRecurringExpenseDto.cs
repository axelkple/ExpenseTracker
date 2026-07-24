using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Dtos;

public class UpdateRecurringExpenseDto
{
    [Required, Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [MaxLength(500)]
    public string Description { get; set; }
    public string Frequency { get; set; }
  public bool IsActive { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}