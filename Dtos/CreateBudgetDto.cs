using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Dtos;

public class CreateBudgetDto
{
    public int? CategoryId { get; set; }

    [Required, Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [Required, RegularExpression("Weekly|Monthly|Yearly")]
    public string Period { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }
}