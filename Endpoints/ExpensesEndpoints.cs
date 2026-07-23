using ExpenseTracker.Data;
using ExpenseTracker.Dtos;
using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Endpoints;

public static class ExpensEndepoints
{
    const string GetExpenseEndpointName = "GetExpense";

    public static void MapExpenseEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/expenses");

        group.MapGet("/{id:int}", async (int id, ExpenseTrackerContext dbContext) =>
        {
            var expense = await dbContext.Expenses
                .Include(e => e.Category)
                .Include(e => e.Account)
                .Include(e => e.PaymentMethod)
                .Include(e => e.ExpenseTags)
                    .ThenInclude(et => et.Tag)
                .Include(e => e.Attachments)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (expense is null) return Results.NotFound();

            ExpenseDto expenseDto = new()
            {
                Id = expense.Id,
                Amount = expense.Amount,
                Currency = expense.Currency,
                Description = expense.Description,
                ExpenseDate = expense.ExpenseDate,
                CreatedAt = expense.CreatedAt,
                IsRecurring = expense.IsRecurring,
                Category = expense.Category == null ? null : new CategoryDto
                {
                    Id = expense.Category.Id,
                    Name = expense.Category.Name
                },
                Account = expense.Account == null ? null : new AccountDto
                {
                    Id = expense.Account.Id,
                    Name = expense.Account.Name
                },
                PaymentMethod = expense.PaymentMethod == null ? null : new PaymentMethodDto
                {
                    Id = expense.PaymentMethod.Id,
                    Name = expense.PaymentMethod.Name
                },
                Tags = expense.ExpenseTags
                    .Select(et => new TagDto
                    {
                        Id = et.Tag.Id,
                        Name = et.Tag.Name
                    })
                    .ToList(),
                Attachments = expense.Attachments
                    .Select(a => new AttachmentDto
                    {
                        Id = a.Id,
                        FileName = a.FileName,
                        FileUrl = a.FileUrl
                    })
                    .ToList()
            };

            return Results.Ok(expenseDto);
        })
        .WithName(GetExpenseEndpointName);
    }
}