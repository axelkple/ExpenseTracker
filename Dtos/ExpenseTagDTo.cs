namespace ExpenseTracker.Dtos;

public class ExpenseTagDto
{
    public int Id { get; set; }
    public int ExpenseId { get; set; }
    public int TagId { get; set; }
    public string TagName { get; set; }

}