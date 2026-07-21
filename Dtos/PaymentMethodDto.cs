namespace ExpenseTracker.Dtos;

public class PaymentMethodDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsSystemDefault { get; set; }
}