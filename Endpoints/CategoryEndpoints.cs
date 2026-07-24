using ExpenseTracker.Data;
using ExpenseTracker.Dtos;
using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;
namespace Expensetracker.Endpoints;

public static class CategoryEndpoints
{
    const string GetCategoryEndpointName = "GetCategory";

    public static void MapCategoryEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/categories");

        // -------------------------------------------------------------
        // GET /categories (Fetch All Categories)
        // -------------------------------------------------------------
        group.MapGet("/", async (ExpenseTrackerContext dbContext) =>
        {
            var categories = await dbContext.Categories
                .AsNoTracking()
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Icon = c.Icon,
                    Color = c.Color,
                    ParentCategoryId = c.ParentCategoryId
                })
                .ToListAsync();

            return Results.Ok(categories);
        });

        // -------------------------------------------------------------
        // GET /categories/{id} (Fetch Single Category by ID)
        // -------------------------------------------------------------
        group.MapGet("/{id:int}", async (int id, ExpenseTrackerContext dbContext) =>
        {
            var category = await dbContext.Categories
                .AsNoTracking()
                .Where(c => c.Id == id)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Icon = c.Icon,
                    Color = c.Color,
                    ParentCategoryId = c.ParentCategoryId
                })
                .FirstOrDefaultAsync();

            return category is null ? Results.NotFound() : Results.Ok(category);
        })
        .WithName(GetCategoryEndpointName);

        // -------------------------------------------------------------
        // POST /categories (Create New Category)
        // -------------------------------------------------------------
        group.MapPost("/", async (CreateCategoryDto newCategory, ExpenseTrackerContext dbContext) =>
        {
            // Validate parent category exists if provided
            if (newCategory.ParentCategoryId.HasValue)
            {
                var parentExists = await dbContext.Categories
                    .AnyAsync(c => c.Id == newCategory.ParentCategoryId.Value);

                if (!parentExists)
                {
                    return Results.BadRequest($"Parent category with ID {newCategory.ParentCategoryId} does not exist.");
                }
            }

            var category = new Category
            {
                Name = newCategory.Name,
                Icon = newCategory.Icon,
                Color = newCategory.Color,
                ParentCategoryId = newCategory.ParentCategoryId
            };

            dbContext.Categories.Add(category);
            await dbContext.SaveChangesAsync();

            var categoryDto = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Icon = category.Icon,
                Color = category.Color,
                ParentCategoryId = category.ParentCategoryId
            };

            return Results.CreatedAtRoute(GetCategoryEndpointName, new { id = category.Id }, categoryDto);
        });

        // -------------------------------------------------------------
        // PUT /categories/{id} (Update Existing Category)
        // -------------------------------------------------------------
        group.MapPut("/{id:int}", async (int id, UpdateCategoryDto updatedCategory, ExpenseTrackerContext dbContext) =>
        {
            var existingCategory = await dbContext.Categories
                .FirstOrDefaultAsync(c => c.Id == id);

            if (existingCategory is null)
            {
                return Results.NotFound();
            }

            // Prevent setting a category as its own parent
            if (updatedCategory.ParentCategoryId == id)
            {
                return Results.BadRequest("A category cannot be its own parent.");
            }

            // Validate parent category exists if provided
            if (updatedCategory.ParentCategoryId.HasValue)
            {
                var parentExists = await dbContext.Categories
                    .AnyAsync(c => c.Id == updatedCategory.ParentCategoryId.Value);

                if (!parentExists)
                {
                    return Results.BadRequest($"Parent category with ID {updatedCategory.ParentCategoryId} does not exist.");
                }
            }

            existingCategory.Name = updatedCategory.Name;
            existingCategory.Icon = updatedCategory.Icon;
            existingCategory.Color = updatedCategory.Color;
            existingCategory.ParentCategoryId = updatedCategory.ParentCategoryId;

            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });

        // -------------------------------------------------------------
        // DELETE /categories/{id} (Delete Category)
        // -------------------------------------------------------------
        group.MapDelete("/{id:int}", async (int id, ExpenseTrackerContext dbContext) =>
        {
            var category = await dbContext.Categories
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category is null)
            {
                return Results.NotFound();
            }

            // Prevent deletion if expenses are assigned to this category
            var hasExpenses = await dbContext.Expenses
                .AnyAsync(e => e.CategoryId == id);

            if (hasExpenses)
            {
                return Results.BadRequest("Cannot delete this category because expenses are assigned to it.");
            }

            dbContext.Categories.Remove(category);
            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });
    }
}