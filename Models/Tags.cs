namespace ExpenseTracker.Models;

public class Tag
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; }

    public ICollection<ExpenseTag> ExpenseTags { get; set; }
}

public class ExpenseTag  // join table
{
    public int ExpenseId { get; set; }
    public Expense Expense { get; set; }

    public int TagId { get; set; }
    public Tag Tag { get; set; }
}