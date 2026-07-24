using ExpenseTracker.Data;
using ExpenseTracker.Dtos;
using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;
namespace Expensetracker.Endpoints;

public static class ExpensesEndpoints
{
    const string GetExpenseEndpointName = "GetExpense";

    public static void MapExpenseEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/expenses");

        // -------------------------------------------------------------
        // GET /expenses (Fetch All Expenses with Projection)
        // -------------------------------------------------------------
        group.MapGet("/", async (ExpenseTrackerContext dbContext) =>
{
    var expenses = await dbContext.Expenses
        .AsNoTracking()
        .Select(e => new ExpenseDto
        {
            Id = e.Id,
            Amount = e.Amount,
            Currency = e.Currency,
            Description = e.Description,
            ExpenseDate = e.ExpenseDate,
            CreatedAt = e.CreatedAt,
            IsRecurring = e.IsRecurring,

            Category = e.Category == null ? null : new CategoryDto
            {
                Id = e.Category.Id,
                Name = e.Category.Name
            },

            Account = e.Account == null ? null : new AccountDto
            {
                Id = e.Account.Id,
                Name = e.Account.Name
            },

            PaymentMethod = e.PaymentMethod == null ? null : new PaymentMethodDto
            {
                Id = e.PaymentMethod.Id,
                Name = e.PaymentMethod.Name,
                //  IsSystemDefault = e.PaymentMethod.UserId == null
            },

            Tags = e.ExpenseTags.Select(et => new TagDto
            {
                Id = et.Tag.Id,
                Name = et.Tag.Name
            }).ToList(),

            Attachments = e.Attachments.Select(a => new AttachmentDto
            {
                Id = a.Id,
                FileUrl = a.FileUrl
            }).ToList()
        })
        .ToListAsync();

    return Results.Ok(expenses);
});

        // -------------------------------------------------------------
        // GET /expenses/{id} (Fetch Single Expense by ID)
        // -------------------------------------------------------------
        group.MapGet("/{id:int}", async (int id, ExpenseTrackerContext dbContext) =>
 {
     var expense = await dbContext.Expenses
         .AsNoTracking()
         .Where(e => e.Id == id)
         .Select(e => new ExpenseDto
         {
             Id = e.Id,
             Amount = e.Amount,
             Currency = e.Currency,
             Description = e.Description,
             ExpenseDate = e.ExpenseDate,
             CreatedAt = e.CreatedAt,
             IsRecurring = e.IsRecurring,

             Category = e.Category == null ? null : new CategoryDto
             {
                 Id = e.Category.Id,
                 Name = e.Category.Name
             },

             Account = e.Account == null ? null : new AccountDto
             {
                 Id = e.Account.Id,
                 Name = e.Account.Name
             },

             PaymentMethod = e.PaymentMethod == null ? null : new PaymentMethodDto
             {
                 Id = e.PaymentMethod.Id,
                 Name = e.PaymentMethod.Name,
                 // IsSystemDefault = e.PaymentMethod.UserId == null
             },

             Tags = e.ExpenseTags.Select(et => new TagDto
             {
                 Id = et.Tag.Id,
                 Name = et.Tag.Name
             }).ToList(),

             Attachments = e.Attachments.Select(a => new AttachmentDto
             {
                 Id = a.Id,
                 FileUrl = a.FileUrl
             }).ToList()
         })
         .FirstOrDefaultAsync();

     return expense is null ? Results.NotFound() : Results.Ok(expense);
 })
 .WithName(GetExpenseEndpointName);
        // -------------------------------------------------------------
        // POST /expenses (Create Expense)
        // -------------------------------------------------------------
        group.MapPost("/", async (CreateExpenseDto newExpense, ExpenseTrackerContext dbContext) =>
        {
            var categoryExists = await dbContext.Categories.AnyAsync(c => c.Id == newExpense.CategoryId);
            if (!categoryExists)
            {
                return Results.BadRequest($"Category with ID {newExpense.CategoryId} does not exist.");
            }

            // Check if Account exists
            var accountExists = await dbContext.Accounts.AnyAsync(a => a.Id == newExpense.AccountId);
            if (!accountExists)
            {
                return Results.BadRequest($"Account with ID {newExpense.AccountId} does not exist.");
            }

            // Check if PaymentMethod exists (if provided)
            if (newExpense.PaymentMethodId.HasValue)
            {
                var paymentMethodExists = await dbContext.PaymentMethods.AnyAsync(p => p.Id == newExpense.PaymentMethodId.Value);
                if (!paymentMethodExists)
                {
                    return Results.BadRequest($"PaymentMethod with ID {newExpense.PaymentMethodId} does not exist.");
                }
            }
            var expense = new Expense
            {
                Amount = newExpense.Amount,
                Currency = newExpense.Currency,
                Description = newExpense.Description,
                ExpenseDate = newExpense.ExpenseDate,
                CreatedAt = DateTime.UtcNow,
                IsRecurring = newExpense.IsRecurring,
                CategoryId = newExpense.CategoryId,
                AccountId = newExpense.AccountId,
                PaymentMethodId = newExpense.PaymentMethodId,
                UserId = newExpense.UserId // I have to remove this due to authenfication
            };

            dbContext.Expenses.Add(expense);
            await dbContext.SaveChangesAsync();

            // Refetch to construct response with related navigation properties loaded
            var createdExpenseDto = await dbContext.Expenses
                .AsNoTracking()
                .Where(e => e.Id == expense.Id)
                .Select(e => new ExpenseDto
                {
                    Id = e.Id,
                    Amount = e.Amount,
                    Currency = e.Currency,
                    Description = e.Description,
                    ExpenseDate = e.ExpenseDate,
                    CreatedAt = e.CreatedAt,
                    IsRecurring = e.IsRecurring,

                    Category = e.Category == null ? null : new CategoryDto
                    {
                        Id = e.Category.Id,
                        Name = e.Category.Name
                    },


                    Account = e.Account == null ? null : new AccountDto
                    {
                        Id = e.Account.Id,
                        Name = e.Account.Name
                    },

                    PaymentMethod = e.PaymentMethod == null ? null : new PaymentMethodDto
                    {
                        Id = e.PaymentMethod.Id,
                        Name = e.PaymentMethod.Name,
                        // IsSystemDefault = e.PaymentMethod.UserId == null
                    },

                    Tags = new List<TagDto>(),
                    Attachments = new List<AttachmentDto>()
                })
                .FirstAsync();

            return Results.CreatedAtRoute(GetExpenseEndpointName, new { id = expense.Id }, createdExpenseDto);
        });

        // -------------------------------------------------------------
        // PUT /expenses/{id} (Update Existing Expense)
        // -------------------------------------------------------------
        group.MapPut("/{id:int}", async (int id, UpdateExpenseDto updatedExpense, ExpenseTrackerContext dbContext) =>
        {
            var existingExpense = await dbContext.Expenses
                .FirstOrDefaultAsync(e => e.Id == id);

            if (existingExpense is null)
            {
                return Results.NotFound();
            }

            existingExpense.Amount = updatedExpense.Amount;
            existingExpense.Currency = updatedExpense.Currency;
            existingExpense.Description = updatedExpense.Description;
            existingExpense.ExpenseDate = updatedExpense.ExpenseDate;
            existingExpense.IsRecurring = updatedExpense.IsRecurring;
            existingExpense.CategoryId = updatedExpense.CategoryId;
            existingExpense.AccountId = updatedExpense.AccountId;
            existingExpense.PaymentMethodId = updatedExpense.PaymentMethodId;

            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });

        // -------------------------------------------------------------
        // DELETE /expenses/{id} (Delete Expense)
        // -------------------------------------------------------------
        group.MapDelete("/{id:int}", async (int id, ExpenseTrackerContext dbContext) =>
        {
            var expense = await dbContext.Expenses
                .FirstOrDefaultAsync(e => e.Id == id);

            if (expense is null)
            {
                return Results.NotFound();
            }

            dbContext.Expenses.Remove(expense);
            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });
    }
}