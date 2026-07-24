namespace ExpenseTracker.Dtos;

public class AttachmentDto
{
    public int Id { get; set; }
    public string FileName { get; set; }
    public string FileUrl { get; set; }
    public string ContentType { get; set; }
    public DateTime UploadedAt { get; set; }
    public int ExpenseId { get; init; }
}
