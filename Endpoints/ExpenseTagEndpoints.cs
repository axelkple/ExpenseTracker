using ExpenseTracker.Data;
using ExpenseTracker.Dtos;
using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;
namespace Expensetracker.Endpoints;

public static class ExpenseTagEndpoints
{
    public static void MapExpenseTagEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/expense-tags");

        // -------------------------------------------------------------
        // POST /expense-tags (Attach single tag to expense)
        // -------------------------------------------------------------
        group.MapPost("/", async (CreateExpenseTagDto dto, ExpenseTrackerContext dbContext) =>
        {
            // 1. Validate Expense exists
            var expenseExists = await dbContext.Expenses.AnyAsync(e => e.Id == dto.ExpenseId);
            if (!expenseExists)
            {
                return Results.BadRequest($"Expense with ID {dto.ExpenseId} does not exist.");
            }

            // 2. Validate Tag exists
            var tag = await dbContext.Tags.FindAsync(dto.TagId);
            if (tag is null)
            {
                return Results.BadRequest($"Tag with ID {dto.TagId} does not exist.");
            }

            // 3. Check if relationship already exists
            var alreadyTagged = await dbContext.ExpenseTags
                .AnyAsync(et => et.ExpenseId == dto.ExpenseId && et.TagId == dto.TagId);

            if (alreadyTagged)
            {
                return Results.BadRequest($"Expense {dto.ExpenseId} already has tag '{tag.Name}'.");
            }

            // 4. Create junction record
            var expenseTag = new ExpenseTag
            {
                ExpenseId = dto.ExpenseId,
                TagId = dto.TagId
            };

            dbContext.ExpenseTags.Add(expenseTag);
            await dbContext.SaveChangesAsync();

            var responseDto = new ExpenseTagDto
            {
                ExpenseId = dto.ExpenseId,
                TagId = dto.TagId,
                TagName = tag.Name
            };
            return Results.Created($"/expense-tags/{dto.ExpenseId}/{dto.TagId}", responseDto);
        });


        // -------------------------------------------------------------
        // GET /expense-tags/expense/{expenseId} (List all tags for an Expense)
        // -------------------------------------------------------------
        group.MapGet("/expense/{expenseId:int}", async (int expenseId, ExpenseTrackerContext dbContext) =>
        {
            var tags = await dbContext.ExpenseTags
     .AsNoTracking()
     .Where(et => et.ExpenseId == expenseId)
     .Select(et => new ExpenseTagDto
     {
         ExpenseId = et.ExpenseId,
         TagId = et.TagId,
         TagName = et.Tag.Name
     })
     .ToListAsync();

            return Results.Ok(tags);
        });

        // -------------------------------------------------------------
        // DELETE /expense-tags/{expenseId}/{tagId} (Remove tag from expense)
        // -------------------------------------------------------------
        group.MapDelete("/{expenseId:int}/{tagId:int}", async (int expenseId, int tagId, ExpenseTrackerContext dbContext) =>
        {
            var expenseTag = await dbContext.ExpenseTags
                .FirstOrDefaultAsync(et => et.ExpenseId == expenseId && et.TagId == tagId);

            if (expenseTag is null)
            {
                return Results.NotFound($"No tag link found for Expense {expenseId} and Tag {tagId}.");
            }

            dbContext.ExpenseTags.Remove(expenseTag);
            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });
    }
}