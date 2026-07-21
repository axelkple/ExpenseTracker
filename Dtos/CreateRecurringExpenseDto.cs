using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Dtos;

public class CreateRecurringExpenseDto
{
    [Required]
    public int CategoryId { get; set; }

    [Required]
    public int AccountId { get; set; }

    [Required, Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [MaxLength(500)]
    public string Description { get; set; }

    [Required, RegularExpression("Daily|Weekly|Monthly|Yearly")]
    public string Frequency { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }
}