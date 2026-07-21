using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;
namespace ExpenseTracker.Data;

public static class DataExtensions
{

    public static void MigrateDb(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var DbContext = scope.ServiceProvider.GetRequiredService<ExpenseTrackerContext>();
        DbContext.Database.Migrate();
    }
    public static void AddExpenseTrackerDb(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<ExpenseTrackerContext>(options =>
     options.UseSqlite(builder.Configuration.GetConnectionString("ExpenseTracker")));

    }
}