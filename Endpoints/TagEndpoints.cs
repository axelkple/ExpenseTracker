using ExpenseTracker.Data;
using ExpenseTracker.Dtos;
using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;
namespace Expensetracker.Endpoints;

public static class TagEndpoints
{
    const string GetTagEndpointName = "GetTag";

    public static void MapTagEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/tags");

        // -------------------------------------------------------------
        // GET /tags
        // -------------------------------------------------------------
        group.MapGet("/", async (ExpenseTrackerContext dbContext) =>
        {
            var tags = await dbContext.Tags
                .AsNoTracking()
                .Select(t => new TagDto
                {
                    Id = t.Id,
                    Name = t.Name
                })
                .ToListAsync();

            return Results.Ok(tags);
        });

        // -------------------------------------------------------------
        // GET /tags/{id}
        // -------------------------------------------------------------
        group.MapGet("/{id:int}", async (int id, ExpenseTrackerContext dbContext) =>
        {
            var tag = await dbContext.Tags
                .AsNoTracking()
                .Where(t => t.Id == id)
                .Select(t => new TagDto
                {
                    Id = t.Id,
                    Name = t.Name
                })
                .FirstOrDefaultAsync();

            return tag is null ? Results.NotFound() : Results.Ok(tag);
        })
        .WithName(GetTagEndpointName);

        // -------------------------------------------------------------
        // POST /tags
        // -------------------------------------------------------------
        group.MapPost("/", async (CreateTagDto dto, ExpenseTrackerContext dbContext) =>
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return Results.BadRequest("Tag name cannot be empty.");
            }

            // Check if tag name already exists (case-insensitive)
            var exists = await dbContext.Tags
                .AnyAsync(t => t.Name.ToLower() == dto.Name.Trim().ToLower());

            if (exists)
            {
                return Results.BadRequest($"A tag named '{dto.Name}' already exists.");
            }

            var tag = new Tag
            {
                Name = dto.Name.Trim(),
                UserId = dto.UserId
            };

            dbContext.Tags.Add(tag);
            await dbContext.SaveChangesAsync();

            var tagDto = new TagDto
            {
                Id = tag.Id,
                Name = tag.Name,
                UserId = tag.UserId
                
            };

            return Results.CreatedAtRoute(GetTagEndpointName, new { id = tag.Id }, tagDto);
        });

        // -------------------------------------------------------------
        // PUT /tags/{id}
        // -------------------------------------------------------------
        group.MapPut("/{id:int}", async (int id, CreateTagDto dto, ExpenseTrackerContext dbContext) =>
        {
            var existingTag = await dbContext.Tags.FindAsync(id);
            if (existingTag is null) return Results.NotFound();

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return Results.BadRequest("Tag name cannot be empty.");
            }

            // Check for duplicate name on a different tag
            var duplicateExists = await dbContext.Tags
                .AnyAsync(t => t.Id != id && t.Name.ToLower() == dto.Name.Trim().ToLower());

            if (duplicateExists)
            {
                return Results.BadRequest($"Another tag named '{dto.Name}' already exists.");
            }

            existingTag.Name = dto.Name.Trim();
            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });

        // -------------------------------------------------------------
        // DELETE /tags/{id}
        // -------------------------------------------------------------
        group.MapDelete("/{id:int}", async (int id, ExpenseTrackerContext dbContext) =>
        {
            var tag = await dbContext.Tags.FindAsync(id);
            if (tag is null) return Results.NotFound();

            dbContext.Tags.Remove(tag);
            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });
    }
}