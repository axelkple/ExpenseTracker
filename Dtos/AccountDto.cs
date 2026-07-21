
namespace ExpenseTracker.Dtos;
public class AccountDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public decimal Balance { get; set; }
    public string Currency { get; set; }
    public bool IsActive { get; set; }
}