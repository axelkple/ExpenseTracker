
namespace ExpenseTracker.Dtos;
public class ExpenseDto
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string? Currency { get; set; }
    public string? Description { get; set; }
    public DateTime ExpenseDate { get; set; }
    public DateTime CreatedAt { get; set; }

    public CategoryDto? Category { get; set; }
    public AccountDto? Account { get; set; }
    public PaymentMethodDto? PaymentMethod { get; set; }
    public List<TagDto> Tags { get; set; }
    public List<AttachmentDto> Attachments { get; set; }

    public bool IsRecurring { get; set; }
}