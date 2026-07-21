using ExpenseTracker.Models;

using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Data;

public class ExpenseTrackerContext : DbContext
{
     public ExpenseTrackerContext(DbContextOptions<ExpenseTrackerContext> options) : base(options)  {}

    public DbSet<User> Users { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<PaymentMethod> PaymentMethods { get; set; }
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<ExpenseTag> ExpenseTags { get; set; }
    public DbSet<Attachment> Attachments { get; set; }
    public DbSet<Budget> Budgets { get; set; }
    public DbSet<RecurringExpense> RecurringExpenses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ===== USER =====
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Email).IsRequired().HasMaxLength(256);
            entity.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(u => u.LastName).IsRequired().HasMaxLength(100);
            entity.Property(u => u.PasswordHash).IsRequired();
        });

        // ===== CATEGORY =====
        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(c => c.Name).IsRequired().HasMaxLength(100);

            entity.HasOne(c => c.ParentCategory)
                  .WithMany(c => c.SubCategories)
                  .HasForeignKey(c => c.ParentCategoryId)
                  .OnDelete(DeleteBehavior.Restrict); // prevent cascade cycles

            entity.HasOne<User>()
                  .WithMany()
                  .HasForeignKey(c => c.UserId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .IsRequired(false);

            entity.HasIndex(c => new { c.UserId, c.Name });
        });

        // ===== ACCOUNT =====
        modelBuilder.Entity<Account>(entity =>
        {
            entity.Property(a => a.Name).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Type).IsRequired().HasMaxLength(50);
            entity.Property(a => a.Currency).IsRequired().HasMaxLength(3);
            entity.Property(a => a.Balance).HasColumnType("decimal(18,2)");

            entity.HasOne(a => a.User)
                  .WithMany(u => u.Accounts)
                  .HasForeignKey(a => a.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ===== PAYMENT METHOD =====
        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.Property(p => p.Name).IsRequired().HasMaxLength(100);

            entity.HasOne<User>()
                  .WithMany()
                  .HasForeignKey(p => p.UserId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .IsRequired(false);
        });

        // ===== EXPENSE =====
        modelBuilder.Entity<Expense>(entity =>
        {
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.Currency).IsRequired().HasMaxLength(3);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ExpenseDate).IsRequired();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(e => e.User)
                  .WithMany(u => u.Expenses)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Category)
                  .WithMany(c => c.Expenses)
                  .HasForeignKey(e => e.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict); // don't delete expenses if category removed

            entity.HasOne(e => e.Account)
                  .WithMany(a => a.Expenses)
                  .HasForeignKey(e => e.AccountId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.PaymentMethod)
                  .WithMany(p => p.Expenses)
                  .HasForeignKey(e => e.PaymentMethodId)
                  .OnDelete(DeleteBehavior.SetNull)
                  .IsRequired(false);

            // common query patterns — index accordingly
            entity.HasIndex(e => new { e.UserId, e.ExpenseDate });
            entity.HasIndex(e => new { e.UserId, e.CategoryId });
            entity.HasIndex(e => new { e.UserId, e.AccountId });
        });

        // ===== TAG =====
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.Property(t => t.Name).IsRequired().HasMaxLength(50);
            entity.HasIndex(t => new { t.UserId, t.Name }).IsUnique();

            entity.HasOne<User>()
                  .WithMany()
                  .HasForeignKey(t => t.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ===== EXPENSE TAG (join table) =====
        modelBuilder.Entity<ExpenseTag>(entity =>
        {
            entity.HasKey(et => new { et.ExpenseId, et.TagId });

            entity.HasOne(et => et.Expense)
                  .WithMany(e => e.ExpenseTags)
                  .HasForeignKey(et => et.ExpenseId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(et => et.Tag)
                  .WithMany(t => t.ExpenseTags)
                  .HasForeignKey(et => et.TagId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ===== ATTACHMENT =====
        modelBuilder.Entity<Attachment>(entity =>
        {
            entity.Property(a => a.FileName).IsRequired().HasMaxLength(255);
            entity.Property(a => a.FileUrl).IsRequired();
            entity.Property(a => a.ContentType).HasMaxLength(100);
            entity.Property(a => a.UploadedAt).HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(a => a.Expense)
                  .WithMany(e => e.Attachments)
                  .HasForeignKey(a => a.ExpenseId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ===== BUDGET =====
        modelBuilder.Entity<Budget>(entity =>
        {
            entity.Property(b => b.Amount).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(b => b.Period).IsRequired().HasMaxLength(20);

            entity.HasOne(b => b.User)
                  .WithMany(u => u.Budgets)
                  .HasForeignKey(b => b.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(b => b.Category)
                  .WithMany()
                  .HasForeignKey(b => b.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict)
                  .IsRequired(false);

            entity.HasIndex(b => new { b.UserId, b.CategoryId, b.StartDate });
        });

        // ===== RECURRING EXPENSE =====
        modelBuilder.Entity<RecurringExpense>(entity =>
        {
            entity.Property(r => r.Amount).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(r => r.Frequency).IsRequired().HasMaxLength(20);
            entity.Property(r => r.Description).HasMaxLength(500);

            entity.HasOne(r => r.User)
                  .WithMany()
                  .HasForeignKey(r => r.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(r => r.Category)
                  .WithMany()
                  .HasForeignKey(r => r.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.Account)
                  .WithMany()
                  .HasForeignKey(r => r.AccountId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(r => new { r.UserId, r.IsActive, r.NextOccurrence });
        });

        // Optional: seed some default categories
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Food", UserId = null },
            new Category { Id = 2, Name = "Transport", UserId = null },
            new Category { Id = 3, Name = "Rent", UserId = null },
            new Category { Id = 4, Name = "Utilities", UserId = null },
            new Category { Id = 5, Name = "Entertainment", UserId = null }
        );
    }
}