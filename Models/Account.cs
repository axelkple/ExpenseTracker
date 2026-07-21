namespace ExpenseTracker.Models;

public class Account
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; }          
    public string Type { get; set; }          
    public decimal Balance { get; set; }
    public string Currency { get; set; }      
    public bool IsActive { get; set; }

    // Navigation
    public User User { get; set; }
    public ICollection<Expense> Expenses { get; set; }
}