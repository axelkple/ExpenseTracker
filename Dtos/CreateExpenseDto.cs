namespace ExpenseTracker.Dtos;
using System.ComponentModel.DataAnnotations;

public class CreateExpenseDto
{
    [Required]
    public int CategoryId { get; set; }

    [Required]
    public int AccountId { get; set; }

    public int? PaymentMethodId { get; set; }

    [Required, Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [Required, MaxLength(3)]
    public string Currency { get; set; }

    [MaxLength(500)]
    public string Description { get; set; }

    [Required]
    public DateTime ExpenseDate { get; set; }

    public List<int> TagIds { get; set; } = new();
}