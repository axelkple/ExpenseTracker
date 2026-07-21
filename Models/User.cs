namespace ExpenseTracker.Models;


public class User
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public required ICollection<Expense> Expenses { get; set; }
    public required ICollection<Budget> Budgets { get; set; }
    public required ICollection<Account> Accounts { get; set; }
}