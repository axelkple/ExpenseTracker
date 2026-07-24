namespace ExpenseTracker.Dtos;

public class BudgetDto
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    
    public CategoryDto Category { get; set; } // null = overall budget
    public decimal Amount { get; set; }
    public string Period { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}