using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VBorsa_API.Core.Entities;
using VBorsa_API.Core.Entities.Common;
using VBorsa_API.Core.Entities.Identity;

namespace VBorsa_API.Persistence.Context;

public class VBorsaDbContext : IdentityDbContext<AppUser, AppRole, Guid>
{
    public VBorsaDbContext(DbContextOptions<VBorsaDbContext> options) : base(options)
    {
    }


    public DbSet<Symbol> Symbols { get; set; }
    public DbSet<Holding> Holdings { get; set; }
    public DbSet<Quote> Quotes { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Symbol>()
            .HasIndex(b => new { b.Code, b.AssetClass })
            .IsUnique();

        builder.Entity<Holding>()
            .HasIndex(h => new { h.UserId });

        builder.Entity<Quote>()
            .HasIndex(q => new { q.SymbolId });

        builder.Entity<Transaction>()
            .HasIndex(t => new { t.UserId });

        builder.Entity<Quote>()
            .Property(q => q.LastPrice)
            .HasPrecision(38, 12);

        builder.Entity<Transaction>()
            .Property(t => t.Price)
            .HasPrecision(38, 12);

        builder
            .Entity<Transaction>()
            .Property(t => t.Quantity)
            .HasPrecision(38, 12);

        builder
            .Entity<Holding>()
            .Property(h => h.Quantity)
            .HasPrecision(38, 12);

        builder.Entity<Holding>()
            .Property(h => h.AvgCost)
            .HasPrecision(38, 12);

        builder.Entity<Holding>()
            .HasOne(h => h.AppUser)
            .WithMany(u => u.Holdings)
            .HasForeignKey(h => h.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Holding>()
            .HasOne(h => h.Symbol)
            .WithMany()
            .HasForeignKey(h => h.SymbolId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Transaction>()
            .HasOne(t => t.AppUser)
            .WithMany(u => u.Transactions)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Transaction>()
            .HasOne(t => t.Symbol)
            .WithMany()
            .HasForeignKey(t => t.SymbolId)
            .OnDelete(DeleteBehavior.Restrict);

        base.OnModelCreating(builder);
    }


    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var datas = ChangeTracker
            .Entries<BaseEntity>();
        foreach (var data in datas)
            _ = data.State switch
            {
                EntityState.Added => data.Entity.CreateDateTime = DateTime.UtcNow,
                EntityState.Modified => data.Entity.UpdateDateTime = DateTime.UtcNow,
                _ => DateTime.UtcNow
            };
        return await base.SaveChangesAsync(cancellationToken);
    }
}