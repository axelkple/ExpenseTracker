namespace ExpenseTracker.Models;
public class Attachment
{
    public int Id { get; set; }
    public int ExpenseId { get; set; }
    public string FileName { get; set; }
    public string FileUrl { get; set; }       // or FilePath if local storage
    public string ContentType { get; set; }
    public DateTime UploadedAt { get; set; }

    public Expense Expense { get; set; }
}