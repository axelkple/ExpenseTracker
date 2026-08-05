using ExpenseTracker.Data;
using ExpenseTracker.Dtos;
using ExpenseTracker.Models;

using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Endpoints;

public static class UsersEndpoints
{
    const string GetUserEndpointName = "GetUser";
    public static void MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/users");

        // GET /user(by id)
        group.MapGet("/{id:int}", async (int id, ExpenseTrackerContext dbContext) =>
        {
            var user = await dbContext.Users.FindAsync(id);
            if (user is null) return Results.NotFound();

            UserDto userDto = new()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                CreatedAt = user.CreatedAt
            };

            return Results.Ok(userDto);
        })
        .WithName(GetUserEndpointName);

        // GET /user(all)
        group.MapGet("/", async (ExpenseTrackerContext dbcontext) =>
        await dbcontext.Users
            .AsNoTracking()
            .Select(User => new UserDto
            {
                Id = User.Id,
                FirstName = User.FirstName,
                LastName = User.LastName,
                Email = User.Email,
                CreatedAt = User.CreatedAt,
                
            })
            .ToListAsync());
         // POST user
        group.MapPost("/", async (CreateUserDto newUser, ExpenseTrackerContext dbContext) =>
        {
            bool emailExists = await dbContext.Users.AnyAsync(u => u.Email == newUser.Email);
            if (emailExists)
            {
                return Results.Conflict(new { message = "Email is already registered." });
            }
            User user = new()
            {
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                Email = newUser.Email,
                PasswordHash = newUser.Password,
                Expenses = new List<Expense>(),
                Budgets = new List<Budget>(),
                Accounts = new List<Account>()
            };

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            UserDto userDto = new()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                CreatedAt = user.CreatedAt
            };


            return Results.CreatedAtRoute(
                GetUserEndpointName,
                new { id = user.Id },
                userDto
            );
        });

        // PUT/games/id

        group.MapPut("/{id}", async (int id, UpdateUserDto updateUser, ExpenseTrackerContext dbcontext) =>
        {
            var existingUser = await dbcontext.Users.FindAsync(id);
            if (existingUser is null)
            {

                return Results.NoContent();
            }
            existingUser.FirstName = updateUser.FirstName;
            existingUser.LastName = updateUser.LastName;
            existingUser.Email= updateUser.Email;


            await dbcontext.SaveChangesAsync();

            return Results.NoContent();
        });

        // Gelete /user(id)
        group.MapDelete("/{id}", async (int id, ExpenseTrackerContext dbcontext) =>
        {
            await dbcontext.Users.Where(user => user.Id == id).ExecuteDeleteAsync();

            return Results.NoContent();

        });
    }

}