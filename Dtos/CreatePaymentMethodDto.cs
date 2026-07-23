
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Dtos;
public class CreatePaymentMethodDto
{
    [Required, MaxLength(100)]
    public string Name { get; set; }
    public int? UserId { get; set; } 
   // public bool IsSystemDefault { get; set; }
     public int ExpenseId { get; set; }
}