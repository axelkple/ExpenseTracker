using ExpenseTracker.Data;
using ExpenseTracker.Dtos;
using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;
namespace Expensetracker.Endpoints;

public static class BudgetEndpoints
{
    const string GetBudgetEndpointName = "GetBudget";

    public static void MapBudgetEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/budgets");

        // -------------------------------------------------------------
        // GET /budgets
        // -------------------------------------------------------------
        group.MapGet("/", async (ExpenseTrackerContext dbContext) =>
        {
            var budgets = await dbContext.Budgets
                .AsNoTracking()
                .Select(b => new BudgetDto
                {
                    Id = b.Id,
                    UserId = b.UserId,
                    //CategoryId = b.CategoryId,
                    Amount = b.Amount,
                    Period = b.Period,
                    StartDate = b.StartDate,
                    EndDate = b.EndDate,

                    // Calculates actual spending within the budget's date window
                    // ActualSpent = dbContext.Expenses
                    //     .Where(e => (b.CategoryId == null || e.CategoryId == b.CategoryId)
                    //              && e.ExpenseDate >= b.StartDate
                    //              && (b.EndDate == null || e.ExpenseDate <= b.EndDate))
                    //     .Sum(e => e.Amount),

                    Category = b.Category == null ? null : new CategoryDto
                    {
                        Id = b.Category.Id,
                        Name = b.Category.Name
                    }
                })
                .ToListAsync();

            return Results.Ok(budgets);
        });

        // -------------------------------------------------------------
        // GET /budgets/{id}
        // -------------------------------------------------------------
        group.MapGet("/{id:int}", async (int id, ExpenseTrackerContext dbContext) =>
        {
            var budget = await dbContext.Budgets
                .AsNoTracking()
                .Where(b => b.Id == id)
                .Select(b => new BudgetDto
                {
                    Id = b.Id,
                    UserId = b.UserId,
                   // CategoryId = b.CategoryId,
                    Amount = b.Amount,
                    Period = b.Period,
                    StartDate = b.StartDate,
                    EndDate = b.EndDate,

                    // ActualSpent = dbContext.Expenses
                    //     .Where(e => (b.CategoryId == null || e.CategoryId == b.CategoryId)
                    //              && e.ExpenseDate >= b.StartDate
                    //              && (b.EndDate == null || e.ExpenseDate <= b.EndDate))
                    //     .Sum(e => e.Amount),

                    Category = b.Category == null ? null : new CategoryDto
                    {
                        Id = b.Category.Id,
                        Name = b.Category.Name
                    }
                })
                .FirstOrDefaultAsync();

            return budget is null ? Results.NotFound() : Results.Ok(budget);
        })
        .WithName(GetBudgetEndpointName);

        // -------------------------------------------------------------
        // POST /budgets
        // -------------------------------------------------------------
        group.MapPost("/", async (CreateBudgetDto dto, ExpenseTrackerContext dbContext) =>
        {
            // Hardcoded UserId for now (replace with httpContext.User claim when authentication is enabled)
            int currentUserId = 1;

            // 1. Validate Category exists if specified
            if (dto.CategoryId.HasValue)
            {
                var categoryExists = await dbContext.Categories.AnyAsync(c => c.Id == dto.CategoryId.Value);
                if (!categoryExists)
                {
                    return Results.BadRequest($"Category with ID {dto.CategoryId} does not exist.");
                }
            }

            // 2. Prevent duplicate overlapping active budgets for the same category/period
            var duplicateExists = await dbContext.Budgets.AnyAsync(b =>
                b.UserId == currentUserId &&
                b.CategoryId == dto.CategoryId &&
                b.Period == dto.Period &&
                b.StartDate == dto.StartDate);

            if (duplicateExists)
            {
                return Results.BadRequest("A budget for this category, period, and start date already exists.");
            }

            // 3. Create Entity
            var budget = new Budget
            {
                UserId = currentUserId,
                CategoryId = dto.CategoryId,
                Amount = dto.Amount,
                Period = dto.Period,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
            };

            dbContext.Budgets.Add(budget);
            await dbContext.SaveChangesAsync();

            // 4. Return DTO response
            var createdDto = new BudgetDto
            {
                Id = budget.Id,
                UserId = budget.UserId,
             //   CategoryId = budget.CategoryId,
                Amount = budget.Amount,
                //ActualSpent = 0,
                Period = budget.Period,
                StartDate = budget.StartDate,
                EndDate = budget.EndDate,
                Category = budget.CategoryId.HasValue
                    ? await dbContext.Categories
                        .Where(c => c.Id == budget.CategoryId)
                        .Select(c => new CategoryDto { Id = c.Id, Name = c.Name })
                        .FirstOrDefaultAsync()
                    : null
            };

            return Results.CreatedAtRoute(GetBudgetEndpointName, new { id = budget.Id }, createdDto);
        });

        // -------------------------------------------------------------
        // PUT /budgets/{id}
        // -------------------------------------------------------------
        group.MapPut("/{id:int}", async (int id, UpdateBudgetDto dto, ExpenseTrackerContext dbContext) =>
        {
            var existingBudget = await dbContext.Budgets.FindAsync(id);
            if (existingBudget is null) return Results.NotFound();

            // Update only allowed fields based on UpdateBudgetDto
            existingBudget.Amount = dto.Amount;
            existingBudget.EndDate = dto.EndDate;

            await dbContext.SaveChangesAsync();
            return Results.NoContent();
        });

        // -------------------------------------------------------------
        // DELETE /budgets/{id}
        // -------------------------------------------------------------
        group.MapDelete("/{id:int}", async (int id, ExpenseTrackerContext dbContext) =>
        {
            var budget = await dbContext.Budgets.FindAsync(id);
            if (budget is null) return Results.NotFound();

            dbContext.Budgets.Remove(budget);
            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });
    }
}