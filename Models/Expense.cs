namespace ExpenseTracker.Models;

public class Expense
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CategoryId { get; set; }
    public int AccountId { get; set; }
    public int? PaymentMethodId { get; set; }

    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public string Description { get; set; }
    public DateTime ExpenseDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public bool IsRecurring { get; set; }
    public int? RecurringExpenseId { get; set; }

    // Navigation
    public User User { get; set; }
    public Category Category { get; set; }
    public Account Account { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public ICollection<ExpenseTag> ExpenseTags { get; set; }
    public ICollection<Attachment> Attachments { get; set; }
}