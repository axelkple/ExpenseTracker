using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Dtos;

public class CreateAttachmentDto
{
    [Required]
    public IFormFile File { get; set; }
}