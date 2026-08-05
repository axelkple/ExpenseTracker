using ExpenseTracker.Data;
using ExpenseTracker.Dtos;
using ExpenseTracker.Models;

using Microsoft.EntityFrameworkCore;

public static class PaymentMethodEndpoints
{
    const string GetPaymentMethodEndpointName = "GetPaymentMethod";
    public static void MapPaymentMethodEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/paymentmethod");
        // GET /Paymentmethod(by id)
        group.MapGet("/{id:int}", async (int id, ExpenseTrackerContext dbContext) =>
        {
            var paymentmethod = await dbContext.PaymentMethods.FindAsync(id);

            if (paymentmethod is null) return Results.NotFound();

            PaymentMethodDto paymentMethodDto = new()
            {
                Name = paymentmethod.Name,
                Id = paymentmethod.Id,
                //  IsSystemDefault = paymentmethod.UserId == null
            };

            return Results.Ok(paymentMethodDto);
        })
        .WithName(GetPaymentMethodEndpointName);

        // GET /paymentMethod(all)
        group.MapGet("/", async (ExpenseTrackerContext dbcontext) =>
       await dbcontext.PaymentMethods
            .AsNoTracking()
           .Select(paymentMethod => new PaymentMethodDto
           {
               Id = paymentMethod.Id,
               Name = paymentMethod.Name,
               //  IsSystemDefault = paymentMethod.UserId == null,


           })

           .ToListAsync());


        // POST
        group.MapPost("/", async (CreatePaymentMethodDto newPaymentMethod, ExpenseTrackerContext dbContext) =>
        {

            PaymentMethod paymentMethod = new()
            {
                Name = newPaymentMethod.Name,
                UserId = newPaymentMethod.UserId




            };

            dbContext.PaymentMethods.Add(paymentMethod);
            await dbContext.SaveChangesAsync();

            PaymentMethodDto paymentMethodDto = new()
            {
                Id = paymentMethod.Id,
                Name = paymentMethod.Name

            };
            return Results.CreatedAtRoute(
                GetPaymentMethodEndpointName,
                new { id = paymentMethod.Id },
                paymentMethodDto
            );
        });


        group.MapDelete("/{id:int}", async (int id, ExpenseTrackerContext dbContext) =>
      {
          var paymentMethod = await dbContext.PaymentMethods
              .FirstOrDefaultAsync(c => c.Id == id);

          if (paymentMethod is null)
          {
              return Results.NotFound();
          }

          // Prevent deletion if expenses are assigned to this category
          var hasExpenses = await dbContext.Expenses
              .AnyAsync(e => e.PaymentMethodId == id);

          if (hasExpenses)
          {
              return Results.BadRequest("Cannot delete this category because expenses are assigned to it.");
          }

          dbContext.PaymentMethods.Remove(paymentMethod);
          await dbContext.SaveChangesAsync();

          return Results.NoContent();
      });


       group.MapPut("/{id:int}", async (int id, CreatePaymentMethodDto createPaymentMethod, ExpenseTrackerContext dbContext) =>
        {
            var existingPaymentMethod = await dbContext.PaymentMethods
                .FirstOrDefaultAsync(c => c.Id == id);

            if (existingPaymentMethod is null)
            {
                return Results.NotFound();
            }

            // Prevent setting a category as its own parent
            // if (createPaymentMethod.ParentCategoryId == id)
            // {
            //     return Results.BadRequest("A category cannot be its own parent.");
            // }

            // Validate parent category exists if provided
            // if (createPaymentMethod.ParentCategoryId.HasValue)
            // {
            //     var parentExists = await dbContext.Categories
            //         .AnyAsync(c => c.Id == updatedCategory.ParentCategoryId.Value);

            //     if (!parentExists)
            //     {
            //         return Results.BadRequest($"Parent category with ID {updatedCategory.ParentCategoryId} does not exist.");
            //     }
            // }

            existingPaymentMethod.Name = createPaymentMethod.Name;
          //  existingPaymentMethod.Icon = createPaymentMethod.Icon;
      

            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });

    }

    


}




