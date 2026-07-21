
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Dtos;
public class CreatePaymentMethodDto
{
    [Required, MaxLength(100)]
    public string Name { get; set; }
}