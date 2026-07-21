namespace ExpenseTracker.Models;

public class RecurringExpense
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CategoryId { get; set; }
    public int AccountId { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
    public string Frequency { get; set; }   // Daily, Weekly, Monthly, Yearly
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime NextOccurrence { get; set; }
    public bool IsActive { get; set; }

    public User User { get; set; }
    public Category Category { get; set; }
    public Account Account { get; set; }
}