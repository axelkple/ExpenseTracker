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


    }




}