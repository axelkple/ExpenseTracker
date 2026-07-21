namespace ExpenseTracker.Models;


public class PaymentMethod
{
    public int Id { get; set; }
    public string Name { get; set; }  // Cash, Credit Card, Debit Card, UPI, etc.
    public int? UserId { get; set; }  // null = system default

    public ICollection<Expense> Expenses { get; set; }
}