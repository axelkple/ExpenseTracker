using ExpenseTracker.Data;
using ExpenseTracker.Dtos;
using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;
namespace Expensetracker.Endpoints;

public static class AttachmentEndpoints
{
    const string GetAttachmentEndpointName = "GetAttachment";

    public static void MapAttachmentEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/attachments");

        // -------------------------------------------------------------
        // POST /attachments/upload (Upload File for an Expense)
        // -------------------------------------------------------------
        group.MapPost("/upload", async (
            IFormFile file, 
            int expenseId, 
            ExpenseTrackerContext dbContext, 
            IWebHostEnvironment environment) =>
        {
            if (file == null || file.Length == 0)
            {
                return Results.BadRequest("No file was uploaded.");
            }

            // 1. Validate Expense exists
            var expenseExists = await dbContext.Expenses.AnyAsync(e => e.Id == expenseId);
            if (!expenseExists)
            {
                return Results.BadRequest($"Expense with ID {expenseId} does not exist.");
            }

            // 2. Prepare upload directory (e.g., wwwroot/uploads)
            var uploadsFolder = Path.Combine(environment.ContentRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // 3. Prevent filename collisions by generating a unique file name
            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // 4. Save file to disk
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // 5. Save metadata to Database
            var attachment = new Attachment
            {
                FileName = file.FileName,
                FileUrl = uniqueFileName,
                ContentType = file.ContentType,
                UploadedAt = DateTime.UtcNow,
                ExpenseId = expenseId
            };

            dbContext.Attachments.Add(attachment);
            await dbContext.SaveChangesAsync();

            var dto = new AttachmentDto
            {
                Id = attachment.Id,
                FileName = attachment.FileName,
                FileUrl = attachment.FileUrl,
                ContentType = attachment.ContentType,
                UploadedAt = attachment.UploadedAt,
                ExpenseId = attachment.ExpenseId
            };

            return Results.CreatedAtRoute(GetAttachmentEndpointName, new { id = attachment.Id }, dto);
        })
        .DisableAntiforgery(); // Useful if testing from REST Client / Swagger without tokens

        // -------------------------------------------------------------
        // GET /attachments/{id} (Fetch Metadata)
        // -------------------------------------------------------------
        group.MapGet("/{id:int}", async (int id, ExpenseTrackerContext dbContext) =>
        {
            var attachment = await dbContext.Attachments
                .AsNoTracking()
                .Where(a => a.Id == id)
                .Select(a => new AttachmentDto
                {
                    Id = a.Id,
                    FileName = a.FileName,
                    FileUrl = a.FileUrl,
                    ContentType = a.ContentType,
                    UploadedAt = a.UploadedAt,
                    ExpenseId = a.ExpenseId
                })
                .FirstOrDefaultAsync();

            return attachment is null ? Results.NotFound() : Results.Ok(attachment);
        })
        .WithName(GetAttachmentEndpointName);

        // -------------------------------------------------------------
        // GET /attachments/{id}/download (Stream/Download File Content)
        // -------------------------------------------------------------
        group.MapGet("/{id:int}/download", async (int id, ExpenseTrackerContext dbContext, IWebHostEnvironment environment) =>
        {
            var attachment = await dbContext.Attachments.FindAsync(id);
            if (attachment is null)
            {
                return Results.NotFound();
            }

            var fullPath = Path.Combine(environment.ContentRootPath, "uploads", attachment.FileUrl);
            if (!File.Exists(fullPath))
            {
                return Results.NotFound("Physical file not found on server disk.");
            }

            // Stream file back to client
            return Results.File(
                path: fullPath, 
                contentType: attachment.ContentType ?? "application/octet-stream", 
                fileDownloadName: attachment.FileName
            );
        });

        // -------------------------------------------------------------
        // GET /attachments/expense/{expenseId} (List all attachments for an Expense)
        // -------------------------------------------------------------
        group.MapGet("/expense/{expenseId:int}", async (int expenseId, ExpenseTrackerContext dbContext) =>
        {
            var attachments = await dbContext.Attachments
                .AsNoTracking()
                .Where(a => a.ExpenseId == expenseId)
                .Select(a => new AttachmentDto
                {
                    Id = a.Id,
                    FileName = a.FileName,
                    FileUrl = a.FileUrl,
                    ContentType = a.ContentType,
                    UploadedAt = a.UploadedAt,
                    ExpenseId = a.ExpenseId
                })
                .ToListAsync();

            return Results.Ok(attachments);
        });

        // -------------------------------------------------------------
        // DELETE /attachments/{id} (Delete Metadata & Disk File)
        // -------------------------------------------------------------
        group.MapDelete("/{id:int}", async (int id, ExpenseTrackerContext dbContext, IWebHostEnvironment environment) =>
        {
            var attachment = await dbContext.Attachments.FindAsync(id);
            if (attachment is null)
            {
                return Results.NotFound();
            }

            // 1. Delete physical file from disk
            var fullPath = Path.Combine(environment.ContentRootPath, "uploads", attachment.FileUrl);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            // 2. Delete database record
            dbContext.Attachments.Remove(attachment);
            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });
    }
}