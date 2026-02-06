using Microsoft.EntityFrameworkCore;
using Settlr.Models.Entities;

namespace Settlr.Data.DbContext;

public class ApplicationDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<GroupMember> GroupMembers => Set<GroupMember>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<ExpenseSplit> ExpenseSplits => Set<ExpenseSplit>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).IsRequired().HasMaxLength(150);
            entity.Property(x => x.Email).IsRequired().HasMaxLength(256);
            entity.Property(x => x.PasswordHash).IsRequired().HasMaxLength(256);
            entity.Property(x => x.CreatedAt).HasDefaultValueSql("NOW()");
            entity.HasIndex(x => x.Email).IsUnique();
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).IsRequired().HasMaxLength(150);
            entity.Property(x => x.CreatedAt).HasDefaultValueSql("NOW()");
            entity.HasOne(x => x.CreatedByUser)
                .WithMany(x => x.GroupsCreated)
                .HasForeignKey(x => x.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<GroupMember>(entity =>
        {
            entity.HasKey(x => new { x.GroupId, x.UserId });
            entity.Property(x => x.JoinedAt).HasDefaultValueSql("NOW()");
            entity.HasOne(x => x.Group)
                .WithMany(x => x.Members)
                .HasForeignKey(x => x.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.User)
                .WithMany(x => x.GroupMemberships)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Expense>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Amount).HasColumnType("numeric(18,2)");
            entity.Property(x => x.Description).IsRequired().HasMaxLength(500);
            entity.Property(x => x.CreatedAt).HasDefaultValueSql("NOW()");
            entity.HasOne(x => x.Group)
                .WithMany(x => x.Expenses)
                .HasForeignKey(x => x.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Payer)
                .WithMany(x => x.ExpensesPaid)
                .HasForeignKey(x => x.PayerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ExpenseSplit>(entity =>
        {
            entity.HasKey(x => new { x.ExpenseId, x.UserId });
            entity.Property(x => x.Amount).HasColumnType("numeric(18,2)");
            entity.HasOne(x => x.Expense)
                .WithMany(x => x.Splits)
                .HasForeignKey(x => x.ExpenseId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.User)
                .WithMany(x => x.ExpenseSplits)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
