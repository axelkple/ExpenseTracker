namespace ExpenseTracker.Dtos;
public class RecurringExpenseDto
{
    public int Id { get; set; }
    public CategoryDto Category { get; set; }
    public AccountDto Account { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
    public string Frequency { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime NextOccurrence { get; set; }
    public bool IsActive { get; set; }
}