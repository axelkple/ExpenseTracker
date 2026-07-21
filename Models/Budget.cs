
namespace ExpenseTracker.Models;

public class Budget
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int? CategoryId { get; set; }   // null = overall budget
    public decimal Amount { get; set; }
    public string Period { get; set; }     // Monthly, Weekly, Yearly
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public User User { get; set; }
    public Category Category { get; set; }
}