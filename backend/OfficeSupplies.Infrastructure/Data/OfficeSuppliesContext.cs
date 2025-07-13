using Microsoft.EntityFrameworkCore;
using OfficeSupplies.Core.Entities;

namespace OfficeSupplies.Infrastructure.Data;

public class OfficeSuppliesContext : DbContext
{
    public OfficeSuppliesContext(DbContextOptions<OfficeSuppliesContext> options)
        : base(options)
    {
    }

    public DbSet<Department> Departments { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Department設定
        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DepartmentId);
            entity.Property(e => e.DepartmentCode).IsRequired().HasMaxLength(20);
            entity.HasIndex(e => e.DepartmentCode).IsUnique();
            entity.Property(e => e.DepartmentName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETDATE()");
        });

        // Item設定
        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.ItemId);
            entity.Property(e => e.ItemCode).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.ItemCode).IsUnique();
            entity.Property(e => e.ItemName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ItemDescription).HasMaxLength(500);
            entity.Property(e => e.Unit).IsRequired().HasMaxLength(20).HasDefaultValue("個");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETDATE()");
        });

        // Transaction設定
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId);
            entity.Property(e => e.TransactionType).IsRequired().HasMaxLength(10);
            entity.Property(e => e.ProcessedBy).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Remarks).HasMaxLength(500);
            entity.Property(e => e.CancelledBy).HasMaxLength(100);
            entity.Property(e => e.ProcessedAt).HasDefaultValueSql("GETDATE()");
            
            entity.HasOne(e => e.Item)
                .WithMany(i => i.Transactions)
                .HasForeignKey(e => e.ItemId);
                
            entity.HasOne(e => e.Department)
                .WithMany(d => d.Transactions)
                .HasForeignKey(e => e.DepartmentId);
                
            entity.HasIndex(e => e.ItemId);
            entity.HasIndex(e => e.DepartmentId);
            entity.HasIndex(e => e.ProcessedAt);
        });
    }
}