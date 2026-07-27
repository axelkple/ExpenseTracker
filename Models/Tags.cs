namespace ExpenseTracker.Models;

public class Tag
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; }

    public ICollection<ExpenseTag> ExpenseTags { get; set; }
}

