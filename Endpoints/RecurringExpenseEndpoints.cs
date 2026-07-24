using ExpenseTracker.Data;
using ExpenseTracker.Dtos;
using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;

public static class RecurringExpenseEndpoints
{
    const string GetRecurringExpenseEndpointName = "GetRecurringExpense";

    public static void MapRecurringExpenseEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/recurringexpenses");

        // -------------------------------------------------------------
        // GET /recurringexpenses
        // -------------------------------------------------------------
        group.MapGet("/", async (ExpenseTrackerContext dbContext) =>
        {
            var recurringExpenses = await dbContext.RecurringExpenses
                .AsNoTracking()
                .Select(re => new RecurringExpenseDto
                {
                    Id = re.Id,
                    Amount = re.Amount,
                    //Currency = re.Currency,
                    Description = re.Description,
                    Frequency = re.Frequency,
                    StartDate = re.StartDate,
                    EndDate = re.EndDate,
                    NextOccurrence = re.NextOccurrence,
                    IsActive = re.IsActive,

                    Category = re.Category == null ? null : new CategoryDto
                    {
                        Id = re.Category.Id,
                        Name = re.Category.Name
                    },
                    Account = re.Account == null ? null : new AccountDto
                    {
                        Id = re.Account.Id,
                        Name = re.Account.Name
                    },

                })
                .ToListAsync();

            return Results.Ok(recurringExpenses);
        });

        // -------------------------------------------------------------
        // GET /recurringexpenses/{id}
        // -------------------------------------------------------------
        group.MapGet("/{id:int}", async (int id, ExpenseTrackerContext dbContext) =>
        {
            var recurringExpense = await dbContext.RecurringExpenses
                .AsNoTracking()
                .Where(re => re.Id == id)
                .Select(re => new RecurringExpenseDto
                {
                    Id = re.Id,
                    Amount = re.Amount,
                    // Currency = re.Currency,
                    Description = re.Description,
                    Frequency = re.Frequency,
                    StartDate = re.StartDate,
                    EndDate = re.EndDate,
                    NextOccurrence = re.NextOccurrence,
                    IsActive = re.IsActive,
                    Category = re.Category == null ? null : new CategoryDto
                    {
                        Id = re.Category.Id,
                        Name = re.Category.Name
                    },
                    Account = re.Account == null ? null : new AccountDto
                    {
                        Id = re.Account.Id,
                        Name = re.Account.Name
                    },

                })
                .FirstOrDefaultAsync();

            return recurringExpense is null ? Results.NotFound() : Results.Ok(recurringExpense);
        })
        .WithName(GetRecurringExpenseEndpointName);

        // -------------------------------------------------------------
        // POST /recurringexpenses
        // -------------------------------------------------------------
        group.MapPost("/", async (CreateRecurringExpenseDto newExp, ExpenseTrackerContext dbContext) =>
        {
            // 1. Foreign Key Validations (Prevents SQLite 500 errors)
            var categoryExists = await dbContext.Categories.AnyAsync(c => c.Id == newExp.CategoryId);
            if (!categoryExists)
            {
                return Results.BadRequest($"Category with ID {newExp.CategoryId} does not exist.");
            }

            // 2. Validate Account
            var accountExists = await dbContext.Accounts.AnyAsync(a => a.Id == newExp.AccountId);
            if (!accountExists)
            {
                return Results.BadRequest($"Account with ID {newExp.AccountId} does not exist.");
            }





            // 2. Create the entity
            var recurringExpense = new RecurringExpense
            {
                Amount = newExp.Amount,
                //Currency = newExp.Currency,
                Description = newExp.Description,
                Frequency = newExp.Frequency,
                StartDate = newExp.StartDate,
                EndDate = newExp.EndDate,
                NextOccurrence = newExp.StartDate,
                CategoryId = newExp.CategoryId,
                AccountId = newExp.AccountId,
                IsActive = newExp.IsActive,
                UserId = newExp.UserId // I have to remove this due to authenfication

            };

            dbContext.RecurringExpenses.Add(recurringExpense);
            await dbContext.SaveChangesAsync();

            // 3. Re-fetch for DTO projection
            var createdDto = await dbContext.RecurringExpenses
                .AsNoTracking()
                .Where(re => re.Id == recurringExpense.Id)
                .Select(re => new RecurringExpenseDto
                {
                    Id = re.Id,
                    Amount = re.Amount,
                    // Currency = re.Currency,
                    Description = re.Description,
                    Frequency = re.Frequency,
                    StartDate = re.StartDate,
                    EndDate = re.EndDate,
                    NextOccurrence = re.NextOccurrence,
                    IsActive = re.IsActive,
                    Category = re.Category == null ? null : new CategoryDto
                    {
                        Id = re.Category.Id,
                        Name = re.Category.Name
                    },

                    Account = re.Account == null ? null : new AccountDto
                    {
                        Id = re.Account.Id,
                        Name = re.Account.Name,
                        Type = re.Account.Type,
                    }
                })
                .FirstAsync();

            return Results.CreatedAtRoute(GetRecurringExpenseEndpointName, new { id = recurringExpense.Id }, createdDto);
        });

        // -------------------------------------------------------------
        // PUT /recurringexpenses/{id}
        // -------------------------------------------------------------
        group.MapPut("/{id:int}", async (int id, UpdateRecurringExpenseDto updatedExp, ExpenseTrackerContext dbContext) =>
        {
            var existingExpense = await dbContext.RecurringExpenses.FindAsync(id);
            if (existingExpense is null) return Results.NotFound();

            // 1. Foreign Key Validations
            if (!await dbContext.Categories.AnyAsync(c => c.Id == existingExpense.CategoryId))
                return Results.BadRequest($"Category with ID {existingExpense.CategoryId} does not exist.");

            if (!await dbContext.Accounts.AnyAsync(a => a.Id == existingExpense.AccountId))
                return Results.BadRequest($"Account with ID {existingExpense.AccountId} does not exist.");


            // 2. Update properties
            existingExpense.Amount = updatedExp.Amount;
            //existingExpense.Currency = updatedExp.Currency;
            existingExpense.Description = updatedExp.Description;
            existingExpense.Frequency = updatedExp.Frequency;
            existingExpense.StartDate = updatedExp.StartDate;
            existingExpense.EndDate = updatedExp.EndDate;
            // existingExpense.CategoryId = updatedExp.CategoryId;
            //existingExpense.AccountId = updatedExp.AccountId;
            existingExpense.IsActive = updatedExp.IsActive;
            await dbContext.SaveChangesAsync();
            return Results.NoContent();
        });

        // -------------------------------------------------------------
        // DELETE /recurringexpenses/{id}
        // -------------------------------------------------------------
        group.MapDelete("/{id:int}", async (int id, ExpenseTrackerContext dbContext) =>
        {
            var existingExpense = await dbContext.RecurringExpenses.FindAsync(id);
            if (existingExpense is null) return Results.NotFound();

            dbContext.RecurringExpenses.Remove(existingExpense);
            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });
    }
}