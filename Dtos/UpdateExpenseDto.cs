using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Dtos;

public class UpdateExpenseDto
{
    [Required]
    public int CategoryId { get; set; }

    [Required]
    public int AccountId { get; set; }

    public int? PaymentMethodId { get; set; }

    [Required, Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [MaxLength(500)]
    public string Description { get; set; }

    [Required]
    public DateTime ExpenseDate { get; set; }

    public List<int> TagIds { get; set; } = new();
}