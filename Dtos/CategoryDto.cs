
namespace ExpenseTracker.Dtos;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Icon { get; set; }
    public string Color { get; set; }
    public int? ParentCategoryId { get; set; }
   // public bool IsSystemDefault { get; set; } // true if UserId is null
   // public List<CategoryDto> SubCategories { get; set; }
}