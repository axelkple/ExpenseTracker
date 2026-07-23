using ExpenseTracker.Data;
using ExpenseTracker.Dtos;
using ExpenseTracker.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace ExpenseTracker.Endpoints;

public static class AccountEndpoints
{
    const string GetAccountEndpointName = "GetAccount";
    public static void MapAccountEndpoints(this WebApplication app)
    {

        var group = app.MapGroup("/accounts");


        // GET /user(by id)
        group.MapGet("/{id:int}", async (int id, ExpenseTrackerContext dbContext) =>
        {
            var account = await dbContext.Accounts
            .Include(account => account.User)
            .FirstOrDefaultAsync(account => account.Id == id);
            if (account is null) return Results.NotFound();

            AccountDto accountDto = new()
            {
                Id = account.Id,
                Name = account.Name,
                Type = account.Type,
                User = $"{account.User?.FirstName} {account.User?.LastName}",
                Balance = account.Balance,
                Currency = account.Currency,
                IsActive = account.IsActive
            };

            return Results.Ok(accountDto);
        })
        .WithName(GetAccountEndpointName);

        // GET /user(all)
        group.MapGet("/", async (ExpenseTrackerContext dbcontext) =>
       await dbcontext.Accounts
           .Include(account => account.User)
           .Select(account => new AccountDto
           {
               Id = account.Id,
               Name = account.Name,
               Type = account.Type,
               User = account.User != null
                   ? $"{account.User.FirstName} {account.User.LastName}"
                   : string.Empty,
               Balance = account.Balance,
               Currency = account.Currency,
               IsActive = account.IsActive,

           })
           .AsNoTracking()
           .ToListAsync());
        // POST
        group.MapPost("/", async (CreateAccountDto newAccount, ExpenseTrackerContext dbContext) =>
        {

            Account account = new()
            {
                Name = newAccount.Name,
                UserId = newAccount.UserId,
                Type = newAccount.Type,
                Balance = newAccount.Balance,
                Currency = newAccount.Currency,
                IsActive = newAccount.IsActive

            };

            dbContext.Accounts.Add(account);
            await dbContext.SaveChangesAsync();

            AccountDto accountDto = new()
            {
                Id = account.Id,
                Name = account.Name,
                Type = account.Type,
                Balance = account.Balance,
                Currency = account.Currency,
                IsActive = account.IsActive
            };

            return Results.CreatedAtRoute(
                GetAccountEndpointName,
                new { id = account.Id },
                accountDto
            );
        });


        // PUT/Account/id

        group.MapPut("/{id}", async (int id, UpdateAccountDto updateAccount, ExpenseTrackerContext dbcontext) =>
        {
            var existingAccount = await dbcontext.Accounts.FindAsync(id);
            if (existingAccount is null)
            {

                return Results.NoContent();
            }
            existingAccount.Name = updateAccount.Name;
            existingAccount.IsActive = updateAccount.IsActive;


            await dbcontext.SaveChangesAsync();

            return Results.NoContent();
        });

        // Gelete /Account(id)
        group.MapDelete("/{id}", async (int id, ExpenseTrackerContext dbcontext) =>
        {
            await dbcontext.Accounts.Where(account => account.Id == id).ExecuteDeleteAsync();

            return Results.NoContent();

        });
    }


}