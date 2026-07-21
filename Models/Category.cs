namespace ExpenseTracker.Models;


public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }          // Food, Transport, Rent, etc.
    public string? Icon { get; set; }          // optional, for UI Mark as nullable
    public string? Color { get; set; }         // optional, for UI Mark as nullable
    public int? UserId { get; set; }          // null = system default category
    public int? ParentCategoryId { get; set; } // for subcategories

    // Navigation
    public Category ParentCategory { get; set; }
    public ICollection<Category> SubCategories { get; set; }
    public ICollection<Expense> Expenses { get; set; }
}