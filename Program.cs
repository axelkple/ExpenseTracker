using Expensetracker.Endpoints;
using ExpenseTracker.Data;
using ExpenseTracker.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddValidation();
builder.AddExpenseTrackerDb();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy => policy.WithOrigins("http://localhost:5173")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.AllowTrailingCommas = true;
    });

// 2. If using Minimal APIs (or both):
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.AllowTrailingCommas = true;
});

var app = builder.Build();
app.UseCors("AllowReactApp");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();

}


app.UseHttpsRedirection();

app.MapUserEndpoints();
app.MapAccountEndpoints();
app.MapExpenseEndpoints();
app.MapCategoryEndpoints();
app.MapRecurringExpenseEndpoints();
app.MapAttachmentEndpoints();
app.MapPaymentMethodEndpoints();
app.MapBudgetEndpoints();
app.MapTagEndpoints();
app.MapExpenseTagEndpoints();
app.MigrateDb();
app.Run();
